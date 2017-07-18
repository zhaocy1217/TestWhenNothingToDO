namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [MessageHandlerClass]
    internal class CInviteSystem : Singleton<CInviteSystem>
    {
        private ListView<COMDT_FRIEND_INFO> allFriendList_internal;
        public int gameTimer = -1;
        private uint lastRefreshLBSTime;
        private ListView<GuildMemInfo> m_allGuildMemberList;
        private BeInviteMenu m_beInviteForm = new BeInviteMenu();
        private stInviteInfo m_inviteInfo;
        private COM_INVITE_JOIN_TYPE m_inviteType;
        private bool m_isFirstlySelectGuildMemberTab = true;
        private bool m_isNeedRefreshGuildMemberPanel = true;
        private ListView<InviteState> m_stateList = new ListView<InviteState>();
        public static string PATH_INVITE_FORM = "UGUI/Form/System/PvP/Form_InviteFriend.prefab";
        private const int REFRESH_GUILD_MEMBER_GAME_STATE_SECONDS = 10;
        private const int REFRESH_GUILD_MEMBER_GAME_STATE_WAIT_MILLISECONDS = 0xbb8;
        public static bool s_isInviteFriendImmidiately;
        private static uint s_refreshLBSTime;
        private const int WEIGHT_INTIMACY_LEVEL = 0x4e20;
        private const int WEIGHT_ONLINE = 0x1312d00;
        private const int WEIGHT_PVP_LEVEL = 1;
        private const int WEIGHT_RANK_LEVEL = 50;
        private const int WEIGHT_VIP_LEVEL = 0x3e8;

        private void AddInviteStateList(ulong uid, uint time, enInviteState state)
        {
            for (int i = 0; i < this.m_stateList.Count; i++)
            {
                if (uid == this.m_stateList[i].uid)
                {
                    this.m_stateList[i].uid = uid;
                    this.m_stateList[i].time = time;
                    this.m_stateList[i].state = state;
                    return;
                }
            }
            InviteState item = new InviteState();
            item.uid = uid;
            item.time = time;
            item.state = state;
            this.m_stateList.Add(item);
        }

        private void ChangeInviteStateList(ulong uid, enInviteState state)
        {
            for (int i = 0; i < this.m_stateList.Count; i++)
            {
                if (uid == this.m_stateList[i].uid)
                {
                    this.m_stateList[i].state = state;
                    return;
                }
            }
        }

        public void CheckInviteListGameTimer()
        {
            if (this.gameTimer == -1)
            {
                this.gameTimer = Singleton<CTimerManager>.instance.AddTimer(0xfde8, 0, new CTimer.OnTimeUpHandler(this.OnInviteListGameTimer));
            }
        }

        public void Clear()
        {
            this.lastRefreshLBSTime = 0;
        }

        public void ClearInviteListGameTimer()
        {
            if (this.gameTimer != -1)
            {
                Singleton<CTimerManager>.instance.RemoveTimer(this.gameTimer);
                this.gameTimer = -1;
            }
        }

        public void CloseInviteForm()
        {
            Singleton<CUIManager>.GetInstance().CloseForm(PATH_INVITE_FORM);
            Singleton<CInviteSystem>.instance.ClearInviteListGameTimer();
            if (this.m_inviteType == COM_INVITE_JOIN_TYPE.COM_INVITE_JOIN_TEAM)
            {
                Singleton<CChatController>.instance.model.channelMgr.Clear(EChatChannel.Team, 0L, 0);
                Singleton<CChatController>.instance.model.channelMgr.SetChatTab(CChatChannelMgr.EChatTab.Normal);
                Singleton<CChatController>.instance.ShowPanel(true, false);
                Singleton<CChatController>.instance.view.UpView(false);
                Singleton<CChatController>.instance.model.sysData.ClearEntryText();
            }
        }

        public ListView<GuildMemInfo> CreateGuildMemberInviteList()
        {
            ListView<GuildMemInfo> guildMemberInfos = CGuildHelper.GetGuildMemberInfos();
            ListView<GuildMemInfo> view2 = new ListView<GuildMemInfo>();
            view2.AddRange(guildMemberInfos);
            for (int i = view2.Count - 1; i >= 0; i--)
            {
                if ((CGuildHelper.IsSelf(view2[i].stBriefInfo.uulUid) || Singleton<CFriendContoller>.instance.model.IsBlack(view2[i].stBriefInfo.uulUid, (uint) view2[i].stBriefInfo.dwLogicWorldId)) || (IsGuildMatchInvite() && CGuildHelper.IsGuildMatchReachMatchCntLimit(view2[i].GuildMatchInfo.bWeekMatchCnt)))
                {
                    view2.RemoveAt(i);
                }
            }
            return view2;
        }

        private void DispatchInviteUIEvent()
        {
            if (s_isInviteFriendImmidiately)
            {
                CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
                uIEvent.m_eventParams = new stUIEventParams();
                uIEvent.m_eventParams.tag = (int) this.m_inviteInfo.joinType;
                uIEvent.m_eventParams.commonUInt64Param1 = this.m_inviteInfo.playerUid;
                uIEvent.m_eventParams.tag2 = (int) this.m_inviteInfo.playerLogicWorldId;
                if (this.m_inviteInfo.objSrc == CInviteView.enInviteListTab.Friend)
                {
                    uIEvent.m_eventID = enUIEventID.Invite_SendInviteFriend;
                }
                else if (this.m_inviteInfo.objSrc == CInviteView.enInviteListTab.GuildMember)
                {
                    uIEvent.m_eventID = enUIEventID.Invite_SendInviteGuildMember;
                }
                else if (this.m_inviteInfo.objSrc == CInviteView.enInviteListTab.LBS)
                {
                    uIEvent.m_eventID = enUIEventID.Invite_SendInviteLBS;
                    uIEvent.m_eventParams.tag3 = this.m_inviteInfo.gameEntity;
                }
                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uIEvent);
                this.m_inviteInfo.Clear();
            }
        }

        public ListView<COMDT_FRIEND_INFO> GetAllFriendList()
        {
            return this.m_allFriendList;
        }

        public ListView<GuildMemInfo> GetAllGuildMemberList()
        {
            return this.m_allGuildMemberList;
        }

        private ListView<GuildMemInfo> GetGuildMemberInviteList()
        {
            return (!IsGuildMatchInvite() ? this.m_allGuildMemberList : Singleton<CGuildMatchSystem>.GetInstance().GetGuildMemberInviteList());
        }

        public string GetInviteFriendName(ulong friendUid, uint friendLogicWorldId)
        {
            if (this.m_allFriendList != null)
            {
                for (int i = 0; i < this.m_allFriendList.Count; i++)
                {
                    if ((friendUid == this.m_allFriendList[i].stUin.ullUid) && (friendLogicWorldId == this.m_allFriendList[i].stUin.dwLogicWorldId))
                    {
                        return StringHelper.UTF8BytesToString(ref this.m_allFriendList[i].szUserName);
                    }
                }
            }
            return string.Empty;
        }

        public string GetInviteGuildMemberName(ulong guildMemberUid)
        {
            if (this.m_allGuildMemberList != null)
            {
                for (int i = 0; i < this.m_allGuildMemberList.Count; i++)
                {
                    if (guildMemberUid == this.m_allGuildMemberList[i].stBriefInfo.uulUid)
                    {
                        return this.m_allGuildMemberList[i].stBriefInfo.sName;
                    }
                }
            }
            return string.Empty;
        }

        private static string GetInviteRoomFailReason(string fName, int errCode, string refuseReason = "")
        {
            string str = string.Empty;
            switch (errCode)
            {
                case 11:
                    return string.Format(Singleton<CTextManager>.GetInstance().GetText("PVP_Can_Not_Find_Friend"), fName);

                case 12:
                {
                    COMDT_FRIEND_INFO comdt_friend_info = Singleton<CFriendContoller>.instance.model.getFriendByName(fName, CFriendModel.FriendType.GameFriend);
                    if (comdt_friend_info == null)
                    {
                        comdt_friend_info = Singleton<CFriendContoller>.instance.model.getFriendByName(fName, CFriendModel.FriendType.SNS);
                    }
                    if (comdt_friend_info != null)
                    {
                        comdt_friend_info.bIsOnline = 0;
                        Singleton<EventRouter>.GetInstance().BroadCastEvent("Friend_Game_State_Refresh");
                    }
                    return string.Format(Singleton<CTextManager>.GetInstance().GetText("PVP_Friend_Off_Line"), fName);
                }
                case 13:
                    return string.Format(Singleton<CTextManager>.GetInstance().GetText("PVP_Gaming_Tip"), fName);

                case 14:
                    return string.Format(Singleton<CTextManager>.GetInstance().GetText("PVP_Invite_Refuse"), fName, refuseReason);

                case 15:
                case 0x10:
                case 0x11:
                case 0x12:
                case 0x13:
                case 20:
                case 0x19:
                case 0x1a:
                case 0x1b:
                case 0x1c:
                    return str;

                case 0x15:
                    return Singleton<CTextManager>.GetInstance().GetText("Err_Version_Different");

                case 0x16:
                    return Singleton<CTextManager>.GetInstance().GetText("Err_Version_Higher");

                case 0x17:
                    return Singleton<CTextManager>.GetInstance().GetText("Err_Version_Lower");

                case 0x18:
                    return Singleton<CTextManager>.GetInstance().GetText("Err_ENTERTAINMENT_Lock");

                case 0x1d:
                    return Singleton<CTextManager>.GetInstance().GetText("CS_ROOMERR_PLAT_CHANNEL_CLOSE");

                case 30:
                    return Singleton<CTextManager>.GetInstance().GetText("Err_Room_Ban_Pick_Hero_Limit_Accept");
            }
            return str;
        }

        private enInviteState GetInviteState(ulong uid)
        {
            for (int i = 0; i < this.m_stateList.Count; i++)
            {
                if (uid == this.m_stateList[i].uid)
                {
                    return this.m_stateList[i].state;
                }
            }
            return enInviteState.None;
        }

        public string GetInviteStateStr(ulong uid, bool isGuildMatchInvite = false)
        {
            enInviteState inviteState = this.GetInviteState(uid);
            if ((inviteState == enInviteState.None) || ((inviteState == enInviteState.BeRejcet) && isGuildMatchInvite))
            {
                return string.Format("<color=#00ff00>{0}</color>", Singleton<CTextManager>.instance.GetText("Common_Online"));
            }
            switch (inviteState)
            {
                case enInviteState.Invited:
                    return string.Format("<color=#ffffff>{0}</color>", Singleton<CTextManager>.instance.GetText("Guild_Has_Invited"));

                case enInviteState.BeRejcet:
                    return string.Format("<color=#ff0000>{0}</color>", Singleton<CTextManager>.instance.GetText("Invite_Friend_Tips_2"));
            }
            return string.Empty;
        }

        private static string GetInviteString(SCPKG_INVITE_JOIN_GAME_REQ msg)
        {
            string str = Utility.UTF8Convert(msg.stInviterInfo.szName);
            string text = string.Empty;
            string str3 = string.Empty;
            string str4 = string.Empty;
            uint dwRelationMask = msg.stInviterInfo.dwRelationMask;
            if ((dwRelationMask & 1) > 0)
            {
                text = Singleton<CTextManager>.instance.GetText("Invite_Src_Type_1");
            }
            else if ((dwRelationMask & 2) > 0)
            {
                text = Singleton<CTextManager>.instance.GetText("Invite_Src_Type_4");
            }
            else if ((dwRelationMask & 4) > 0)
            {
                text = Singleton<CTextManager>.instance.GetText("Invite_Src_Type_5");
            }
            else
            {
                text = Singleton<CTextManager>.instance.GetText("Invite_Src_Type_1");
            }
            if (msg.bInviteType == 1)
            {
                ResDT_LevelCommonInfo pvpMapCommonInfo = CLevelCfgLogicManager.GetPvpMapCommonInfo(msg.stInviteDetail.stRoomDetail.bMapType, msg.stInviteDetail.stRoomDetail.dwMapId);
                if (pvpMapCommonInfo != null)
                {
                    string[] textArray1 = new string[] { (pvpMapCommonInfo.bMaxAcntNum / 2).ToString(), (pvpMapCommonInfo.bMaxAcntNum / 2).ToString(), Utility.UTF8Convert(pvpMapCommonInfo.szName) };
                    str4 = Singleton<CTextManager>.instance.GetText("Invite_Map_Desc", textArray1);
                }
                str3 = Singleton<CTextManager>.GetInstance().GetText("Invite_Match_Type_4");
            }
            else if (msg.bInviteType == 2)
            {
                ResDT_LevelCommonInfo info2 = CLevelCfgLogicManager.GetPvpMapCommonInfo(msg.stInviteDetail.stTeamDetail.bMapType, msg.stInviteDetail.stTeamDetail.dwMapId);
                if (info2 != null)
                {
                    string[] textArray2 = new string[] { (info2.bMaxAcntNum / 2).ToString(), (info2.bMaxAcntNum / 2).ToString(), Utility.UTF8Convert(info2.szName) };
                    str4 = Singleton<CTextManager>.instance.GetText("Invite_Map_Desc", textArray2);
                }
                if (msg.stInviteDetail.stTeamDetail.bMapType == 3)
                {
                    str3 = Singleton<CTextManager>.GetInstance().GetText("Invite_Match_Type_1");
                }
                else if (msg.stInviteDetail.stTeamDetail.bPkAI == 1)
                {
                    str3 = Singleton<CTextManager>.GetInstance().GetText("Invite_Match_Type_2");
                }
                else
                {
                    str3 = Singleton<CTextManager>.GetInstance().GetText("Invite_Match_Type_3");
                }
            }
            string[] args = new string[] { str, text, str3, str4 };
            return Singleton<CTextManager>.instance.GetText("Be_Invited_Tips", args);
        }

        private static string GetInviteTeamFailReason(string fName, int errCode, uint timePunished = 0, uint punishType = 1, string refuseReason = "")
        {
            string str2;
            string str3;
            string str = string.Empty;
            switch (errCode)
            {
                case 3:
                    return string.Format(Singleton<CTextManager>.GetInstance().GetText("PVP_Can_Not_Find_Friend"), fName);

                case 4:
                {
                    COMDT_FRIEND_INFO comdt_friend_info = Singleton<CFriendContoller>.instance.model.getFriendByName(fName, CFriendModel.FriendType.GameFriend);
                    if (comdt_friend_info == null)
                    {
                        comdt_friend_info = Singleton<CFriendContoller>.instance.model.getFriendByName(fName, CFriendModel.FriendType.SNS);
                    }
                    if (comdt_friend_info != null)
                    {
                        comdt_friend_info.bIsOnline = 0;
                        Singleton<EventRouter>.GetInstance().BroadCastEvent("Friend_Game_State_Refresh");
                    }
                    return string.Format(Singleton<CTextManager>.GetInstance().GetText("PVP_Friend_Off_Line"), fName);
                }
                case 5:
                    return string.Format(Singleton<CTextManager>.GetInstance().GetText("PVP_Gaming_Tip"), fName);

                case 6:
                    return string.Format(Singleton<CTextManager>.GetInstance().GetText("PVP_Invite_Refuse"), fName, refuseReason);

                case 7:
                case 8:
                case 10:
                case 11:
                case 13:
                case 0x16:
                case 0x18:
                case 0x19:
                    return str;

                case 9:
                    return Singleton<CTextManager>.GetInstance().GetText("Err_Team_Member_Full");

                case 12:
                    return Singleton<CTextManager>.GetInstance().GetText("Err_Invite_Result_4");

                case 14:
                    return Singleton<CTextManager>.GetInstance().GetText("Err_Version_Higher");

                case 15:
                    return Singleton<CTextManager>.GetInstance().GetText("Err_Version_Lower");

                case 0x10:
                    return Singleton<CTextManager>.GetInstance().GetText("Err_ENTERTAINMENT_Lock");

                case 0x11:
                    str2 = string.Format("{0}分{1}秒", timePunished / 60, timePunished % 60);
                    str3 = string.Empty;
                    switch (punishType)
                    {
                        case 1:
                            str3 = "PVP_Invite_Punished";
                            break;

                        case 2:
                            str3 = "PVP_Invite_HangUpPunished";
                            break;

                        case 3:
                            str3 = "PVP_Invite_CreditPunished";
                            break;
                    }
                    str3 = "PVP_Invite_Punished";
                    break;

                case 0x12:
                    return Singleton<CTextManager>.GetInstance().GetText("Err_Invite_Result_1");

                case 0x13:
                    return Singleton<CTextManager>.GetInstance().GetText("Err_Invite_Result_2");

                case 20:
                    return Singleton<CTextManager>.GetInstance().GetText("Err_Invite_Result_3");

                case 0x15:
                    return Singleton<CTextManager>.GetInstance().GetText("Err_Invite_Result_5");

                case 0x17:
                    return Singleton<CTextManager>.GetInstance().GetText("Invite_Err_Credit_Limit");

                case 0x1a:
                    return Singleton<CTextManager>.GetInstance().GetText("COM_MATCHTEAMEERR_PLAT_CHANNEL_CLOSE");

                case 0x1b:
                    return Singleton<CTextManager>.GetInstance().GetText("COM_MATCHTEAMEERR_OBING");

                default:
                    return str;
            }
            return string.Format(Singleton<CTextManager>.GetInstance().GetText(str3), fName, str2);
        }

        public CUIListElementScript GetListItem(string username)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(PATH_INVITE_FORM);
            if (form != null)
            {
                CUIListScript component = form.get_transform().Find("Panel_Friend/List").get_gameObject().GetComponent<CUIListScript>();
                for (int i = 0; i < component.m_elementAmount; i++)
                {
                    CUIListElementScript elemenet = component.GetElemenet(i);
                    if ((elemenet != null) && (elemenet.get_gameObject().get_transform().Find("PlayerName").GetComponent<Text>().get_text() == username))
                    {
                        return elemenet;
                    }
                }
            }
            return null;
        }

        private uint GetNextInviteSec(ulong uid, uint time)
        {
            uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x9c).dwConfValue;
            for (int i = 0; i < this.m_stateList.Count; i++)
            {
                if (uid == this.m_stateList[i].uid)
                {
                    return ((this.m_stateList[i].time + dwConfValue) - time);
                }
            }
            return 0;
        }

        private static int GuildMemberComparison(GuildMemInfo l, GuildMemInfo r)
        {
            uint num = 0;
            uint num2 = 0;
            if (CGuildHelper.IsMemberOnline(l))
            {
                if (l.GameState == COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_IDLE)
                {
                    num = 3;
                }
                else if (l.GameState == COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_TEAM)
                {
                    num = 2;
                }
                else
                {
                    num = 1;
                }
            }
            if (CGuildHelper.IsMemberOnline(r))
            {
                if (r.GameState == COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_IDLE)
                {
                    num2 = 3;
                }
                else if (r.GameState == COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_TEAM)
                {
                    num2 = 2;
                }
                else
                {
                    num2 = 1;
                }
            }
            return (int) ((((0x1312d00 * (num2 - num)) + (0x3e8 * (r.stBriefInfo.stVip.level - l.stBriefInfo.stVip.level))) + (50 * (r.stBriefInfo.dwClassOfRank - l.stBriefInfo.dwClassOfRank))) + (1 * (r.stBriefInfo.dwLevel - l.stBriefInfo.dwLevel)));
        }

        private bool InInviteCdList(ulong uid, uint time)
        {
            for (int i = 0; i < this.m_stateList.Count; i++)
            {
                if (uid == this.m_stateList[i].uid)
                {
                    return ((time - this.m_stateList[i].time) < GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x9c).dwConfValue);
                }
            }
            return false;
        }

        public override void Init()
        {
            base.Init();
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Invite_SendInviteFriend, new CUIEventManager.OnUIEventHandler(this.OnInvite_SendInviteFriend));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Invite_SendInviteGuildMember, new CUIEventManager.OnUIEventHandler(this.OnInvite_SendInviteGuildMember));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Invite_SendInviteLBS, new CUIEventManager.OnUIEventHandler(this.OnInvite_SendInviteLBS));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Invite_AcceptInvite, new CUIEventManager.OnUIEventHandler(this.OnInvite_Accept));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Invite_RejectInvite, new CUIEventManager.OnUIEventHandler(this.OnInvite_Reject));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Invite_TimeOut, new CUIEventManager.OnUIEventHandler(this.OnInvateFriendTimeOut));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Invite_FriendListElementEnable, new CUIEventManager.OnUIEventHandler(this.OnInvite_FriendListElementEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Invite_GuildMemberListElementEnable, new CUIEventManager.OnUIEventHandler(this.OnInvite_GuildMemberListElementEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Invite_LBSListElementEnable, new CUIEventManager.OnUIEventHandler(this.OnInvite_LBSListElementEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Invite_TabChange, new CUIEventManager.OnUIEventHandler(this.OnInvite_TabChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Invite_RefreshGameStateTimeout, new CUIEventManager.OnUIEventHandler(this.OnInvite_RefreshGameStateTimeout));
            Singleton<EventRouter>.GetInstance().AddEventHandler<byte, string>(EventID.INVITE_ROOM_ERRCODE_NTF, new Action<byte, string>(this, (IntPtr) this.OnInviteRoomErrCodeNtf));
            Singleton<EventRouter>.GetInstance().AddEventHandler<byte, string>(EventID.INVITE_TEAM_ERRCODE_NTF, new Action<byte, string>(this, (IntPtr) this.OnInviteTeamErrCodeNtf));
            Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_ADD_NTF", new Action<CSPkg>(this.OnFriendChange));
            Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_Delete_NTF", new Action<CSPkg>(this.OnFriendChange));
            Singleton<EventRouter>.GetInstance().AddEventHandler<CFriendModel.FriendType, ulong, uint, bool>("Chat_Friend_Online_Change", new Action<CFriendModel.FriendType, ulong, uint, bool>(this, (IntPtr) this.OnFriendOnlineChg));
            Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.Friend_Game_State_Change, new Action(this, (IntPtr) this.OnFriendOnlineChg));
            Singleton<EventRouter>.GetInstance().AddEventHandler("Friend_Game_State_Refresh", new Action(this, (IntPtr) this.OnFriendOnlineChg));
            Singleton<EventRouter>.GetInstance().AddEventHandler("Friend_LBS_User_Refresh", new Action(this, (IntPtr) this.OnLBSListChg));
        }

        private static int InviteFriendsSort(COMDT_FRIEND_INFO l, COMDT_FRIEND_INFO r)
        {
            bool flag;
            bool flag2;
            ushort num3;
            ushort num4;
            CFriendModel.EIntimacyType type;
            CFriendModel.EIntimacyType type2;
            uint num = 0;
            uint num2 = 0;
            if (l.bIsOnline == 1)
            {
                COM_ACNT_GAME_STATE state = COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_IDLE;
                CFriendModel.FriendInGame friendInGaming = Singleton<CFriendContoller>.instance.model.GetFriendInGaming(l.stUin.ullUid, l.stUin.dwLogicWorldId);
                if (friendInGaming != null)
                {
                    state = friendInGaming.State;
                }
                if (state == COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_IDLE)
                {
                    num = 3;
                }
                else if (state == COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_TEAM)
                {
                    num = 2;
                }
                else
                {
                    num = 1;
                }
            }
            if (r.bIsOnline == 1)
            {
                COM_ACNT_GAME_STATE com_acnt_game_state2 = COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_IDLE;
                CFriendModel.FriendInGame game2 = Singleton<CFriendContoller>.instance.model.GetFriendInGaming(r.stUin.ullUid, r.stUin.dwLogicWorldId);
                if (game2 != null)
                {
                    com_acnt_game_state2 = game2.State;
                }
                if (com_acnt_game_state2 == COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_IDLE)
                {
                    num2 = 3;
                }
                else if (com_acnt_game_state2 == COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_TEAM)
                {
                    num2 = 2;
                }
                else
                {
                    num2 = 1;
                }
            }
            CFriendModel model = Singleton<CFriendContoller>.instance.model;
            model.GetFriendIntimacy(r.stUin.ullUid, r.stUin.dwLogicWorldId, out num3, out type, out flag);
            model.GetFriendIntimacy(l.stUin.ullUid, l.stUin.dwLogicWorldId, out num4, out type2, out flag2);
            return (int) (((((0x1312d00 * (num2 - num)) + (0x4e20 * (num3 - num4))) + (0x3e8 * (r.stGameVip.dwCurLevel - l.stGameVip.dwCurLevel))) + (50 * (r.dwRankClass - l.dwRankClass))) + (1 * (r.dwPvpLvl - l.dwPvpLvl)));
        }

        public bool IsCanBeInvited(ulong inviterUid, uint inviterLogicWorldId)
        {
            if (!Utility.IsCanShowPrompt())
            {
                return false;
            }
            if (Singleton<CFriendContoller>.instance.model.IsBlack(inviterUid, inviterLogicWorldId))
            {
                return false;
            }
            return true;
        }

        public static bool IsGuildMatchInvite()
        {
            return (Singleton<CUIManager>.GetInstance().GetForm(CGuildMatchSystem.GuildMatchFormPath) != null);
        }

        public void Load_Config_Text()
        {
            if (this.m_beInviteForm != null)
            {
                this.m_beInviteForm.LoadConfig();
            }
        }

        private void OnFriendChange(CSPkg msg)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(PATH_INVITE_FORM);
            if (form != null)
            {
                this.SortAllFriendList();
                CInviteView.SetInviteFriendData(form, this.m_inviteType);
            }
        }

        private void OnFriendOnlineChg()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(PATH_INVITE_FORM);
            if (form != null)
            {
                this.SortAllFriendList();
                CInviteView.SetInviteFriendData(form, this.m_inviteType);
            }
        }

        private void OnFriendOnlineChg(CFriendModel.FriendType type, ulong ullUid, uint dwLogicWorldId, bool bOffline)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(PATH_INVITE_FORM);
            if (form != null)
            {
                this.SortAllFriendList();
                CInviteView.SetInviteFriendData(form, this.m_inviteType);
            }
        }

        [MessageHandler(0x7e4)]
        public static void OnGetInvited(CSPkg msg)
        {
            if (Singleton<CInviteSystem>.GetInstance().IsCanBeInvited(msg.stPkgData.stInviteJoinGameReq.stInviterInfo.ullUid, msg.stPkgData.stInviteJoinGameReq.stInviterInfo.dwLogicWorldID))
            {
                Singleton<CInviteSystem>.instance.ShowNewBeingInvitedUI(msg.stPkgData.stInviteJoinGameReq);
            }
        }

        private void OnInvateFriendTimeOut(CUIEvent uiEvent)
        {
            if (uiEvent.m_srcFormScript != null)
            {
                Singleton<CUIManager>.GetInstance().CloseForm(uiEvent.m_srcFormScript);
                Singleton<CMailSys>.instance.AddFriendInviteMail(uiEvent, CMailSys.enProcessInviteType.NoProcess);
            }
        }

        private void OnInvite_Accept(CUIEvent uiEvent)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x7e5);
            msg.stPkgData.stInviteJoinGameRsp.bIndex = (byte) uiEvent.m_eventParams.tag;
            msg.stPkgData.stInviteJoinGameRsp.bResult = 0;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
            Singleton<WatchController>.GetInstance().Stop();
            Singleton<CUIManager>.GetInstance().CloseMessageBox();
            Singleton<CMailSys>.instance.AddFriendInviteMail(uiEvent, CMailSys.enProcessInviteType.Accept);
        }

        private void OnInvite_FriendListElementEnable(CUIEvent uiEvent)
        {
            int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
            GameObject srcWidget = uiEvent.m_srcWidget;
            if ((srcWidgetIndexInBelongedList >= 0) && (srcWidgetIndexInBelongedList < this.m_allFriendList.Count))
            {
                CInviteView.UpdateFriendListElement(srcWidget, this.m_allFriendList[srcWidgetIndexInBelongedList]);
            }
        }

        private void OnInvite_GuildMemberListElementEnable(CUIEvent uiEvent)
        {
            int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
            GameObject srcWidget = uiEvent.m_srcWidget;
            if ((srcWidgetIndexInBelongedList >= 0) && (srcWidgetIndexInBelongedList < this.m_allGuildMemberList.Count))
            {
                CInviteView.UpdateGuildMemberListElement(srcWidget, this.m_allGuildMemberList[srcWidgetIndexInBelongedList], false);
            }
        }

        private void OnInvite_LBSListElementEnable(CUIEvent uiEvent)
        {
            int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
            GameObject srcWidget = uiEvent.m_srcWidget;
            ListView<CSDT_LBS_USER_INFO> lBSList = Singleton<CFriendContoller>.instance.model.GetLBSList(CFriendModel.LBSGenderType.Both);
            if ((lBSList != null) && ((srcWidgetIndexInBelongedList >= 0) && (srcWidgetIndexInBelongedList < lBSList.Count)))
            {
                CInviteView.UpdateLBSListElement(srcWidget, lBSList[srcWidgetIndexInBelongedList]);
            }
        }

        private void OnInvite_RefreshGameStateTimeout(CUIEvent uiEvent)
        {
            this.SendGetGuildMemberGameStateReq();
        }

        private void OnInvite_Reject(CUIEvent uiEvent)
        {
            if (this.m_beInviteForm != null)
            {
                this.m_beInviteForm.ShowRefuse();
            }
        }

        private void OnInvite_SendInviteFriend(CUIEvent uiEvent)
        {
            COM_INVITE_JOIN_TYPE tag = (COM_INVITE_JOIN_TYPE) uiEvent.m_eventParams.tag;
            ulong uid = uiEvent.m_eventParams.commonUInt64Param1;
            uint logicWorldId = (uint) uiEvent.m_eventParams.tag2;
            uint currentUTCTime = (uint) CRoleInfo.GetCurrentUTCTime();
            if (this.InInviteCdList(uid, currentUTCTime))
            {
                object[] replaceArr = new object[] { this.GetNextInviteSec(uid, currentUTCTime) };
                Singleton<CUIManager>.instance.OpenTips("Invite_Friend_Tips_1", true, 1f, null, replaceArr);
            }
            else
            {
                bool flag = Singleton<CFriendContoller>.instance.model.IsGameFriend(uid, logicWorldId);
                CFriendModel.FriendType friendType = !flag ? CFriendModel.FriendType.SNS : CFriendModel.FriendType.GameFriend;
                byte num4 = !flag ? ((byte) 2) : ((byte) 1);
                COMDT_FRIEND_INFO comdt_friend_info = Singleton<CFriendContoller>.instance.model.getFriendByUid(uid, friendType, 0);
                if (comdt_friend_info != null)
                {
                    switch (tag)
                    {
                        case COM_INVITE_JOIN_TYPE.COM_INVITE_JOIN_ROOM:
                        {
                            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x7e1);
                            msg.stPkgData.stInviteFriendJoinRoomReq.stFriendInfo.ullUid = comdt_friend_info.stUin.ullUid;
                            msg.stPkgData.stInviteFriendJoinRoomReq.stFriendInfo.dwLogicWorldId = comdt_friend_info.stUin.dwLogicWorldId;
                            msg.stPkgData.stInviteFriendJoinRoomReq.bFriendType = num4;
                            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
                            break;
                        }
                        case COM_INVITE_JOIN_TYPE.COM_INVITE_JOIN_TEAM:
                        {
                            CSPkg pkg2 = NetworkModule.CreateDefaultCSPKG(0x7e8);
                            pkg2.stPkgData.stInviteFriendJoinTeamReq.stFriendInfo.ullUid = comdt_friend_info.stUin.ullUid;
                            pkg2.stPkgData.stInviteFriendJoinTeamReq.stFriendInfo.dwLogicWorldId = comdt_friend_info.stUin.dwLogicWorldId;
                            pkg2.stPkgData.stInviteFriendJoinTeamReq.bFriendType = num4;
                            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref pkg2, false);
                            break;
                        }
                    }
                    this.SetGameStateTextWidget(uiEvent.m_srcWidget);
                    this.AddInviteStateList(uid, currentUTCTime, enInviteState.Invited);
                }
            }
        }

        private void OnInvite_SendInviteGuildMember(CUIEvent uiEvent)
        {
            int tag = uiEvent.m_eventParams.tag;
            ulong num2 = uiEvent.m_eventParams.commonUInt64Param1;
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x7f2);
            msg.stPkgData.stInviteGuildMemberJoinReq.iInviteType = tag;
            msg.stPkgData.stInviteGuildMemberJoinReq.ullInviteeUid = num2;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
            if (uiEvent.m_srcWidget != null)
            {
                uiEvent.m_srcWidget.CustomSetActive(false);
                this.SetGameStateTextWidget(uiEvent.m_srcWidget);
            }
        }

        private void OnInvite_SendInviteLBS(CUIEvent uiEvent)
        {
            COM_INVITE_JOIN_TYPE tag = (COM_INVITE_JOIN_TYPE) uiEvent.m_eventParams.tag;
            ulong uid = uiEvent.m_eventParams.commonUInt64Param1;
            uint num2 = (uint) uiEvent.m_eventParams.tag2;
            uint num3 = (uint) uiEvent.m_eventParams.tag3;
            uint currentUTCTime = (uint) CRoleInfo.GetCurrentUTCTime();
            if (this.InInviteCdList(uid, currentUTCTime))
            {
                object[] replaceArr = new object[] { this.GetNextInviteSec(uid, currentUTCTime) };
                Singleton<CUIManager>.instance.OpenTips("Invite_Friend_Tips_1", true, 1f, null, replaceArr);
            }
            else
            {
                switch (tag)
                {
                    case COM_INVITE_JOIN_TYPE.COM_INVITE_JOIN_ROOM:
                    {
                        CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x7e1);
                        msg.stPkgData.stInviteFriendJoinRoomReq.stFriendInfo.ullUid = uid;
                        msg.stPkgData.stInviteFriendJoinRoomReq.stFriendInfo.dwLogicWorldId = num2;
                        msg.stPkgData.stInviteFriendJoinRoomReq.bFriendType = 3;
                        msg.stPkgData.stInviteFriendJoinRoomReq.dwGameSvrEntity = num3;
                        Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
                        break;
                    }
                    case COM_INVITE_JOIN_TYPE.COM_INVITE_JOIN_TEAM:
                    {
                        CSPkg pkg2 = NetworkModule.CreateDefaultCSPKG(0x7e8);
                        pkg2.stPkgData.stInviteFriendJoinTeamReq.stFriendInfo.ullUid = uid;
                        pkg2.stPkgData.stInviteFriendJoinTeamReq.stFriendInfo.dwLogicWorldId = num2;
                        pkg2.stPkgData.stInviteFriendJoinTeamReq.bFriendType = 3;
                        pkg2.stPkgData.stInviteFriendJoinTeamReq.dwGameSvrEntity = num3;
                        Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref pkg2, false);
                        break;
                    }
                }
                this.SetGameStateTextWidget(uiEvent.m_srcWidget);
                this.AddInviteStateList(uid, currentUTCTime, enInviteState.Invited);
            }
        }

        private void OnInvite_TabChange(CUIEvent uiEvent)
        {
            CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
            CUIListScript component = uiEvent.m_srcWidget.GetComponent<CUIListScript>();
            GameObject widget = srcFormScript.GetWidget(0);
            GameObject obj3 = srcFormScript.GetWidget(1);
            GameObject obj4 = srcFormScript.GetWidget(10);
            switch (CInviteView.GetInviteListTab(component.GetSelectedIndex()))
            {
                case CInviteView.enInviteListTab.Friend:
                    widget.CustomSetActive(true);
                    obj3.CustomSetActive(false);
                    obj4.CustomSetActive(false);
                    break;

                case CInviteView.enInviteListTab.GuildMember:
                    obj3.CustomSetActive(true);
                    widget.CustomSetActive(false);
                    obj4.CustomSetActive(false);
                    if (this.m_isFirstlySelectGuildMemberTab)
                    {
                        this.SendGetGuildMemberGameStateReq();
                        CUITimerScript timer = srcFormScript.GetWidget(8).GetComponent<CUITimerScript>();
                        this.SetAndStartRefreshGuildMemberGameStateTimer(timer);
                        this.m_isFirstlySelectGuildMemberTab = false;
                    }
                    break;

                case CInviteView.enInviteListTab.LBS:
                {
                    obj3.CustomSetActive(false);
                    widget.CustomSetActive(false);
                    obj4.CustomSetActive(true);
                    CInviteView.SetLBSData(srcFormScript, this.m_inviteType);
                    uint currentUTCTime = (uint) CRoleInfo.GetCurrentUTCTime();
                    if ((currentUTCTime - this.lastRefreshLBSTime) > RefreshLBSTime)
                    {
                        this.lastRefreshLBSTime = currentUTCTime;
                        CUIEvent event2 = new CUIEvent();
                        event2.m_eventID = enUIEventID.Friend_LBS_Refresh;
                        event2.m_eventParams.tag = 1;
                        Singleton<CUIEventManager>.instance.DispatchUIEvent(event2);
                    }
                    break;
                }
            }
        }

        private void OnInviteErrCodeNtf(COM_INVITE_JOIN_TYPE inviteType, byte errorCode, string userName)
        {
            if (((inviteType == COM_INVITE_JOIN_TYPE.COM_INVITE_JOIN_ROOM) && (errorCode == 14)) || ((inviteType == COM_INVITE_JOIN_TYPE.COM_INVITE_JOIN_TEAM) && (errorCode == 6)))
            {
                COMDT_FRIEND_INFO comdt_friend_info = Singleton<CFriendContoller>.instance.model.getFriendByName(userName, CFriendModel.FriendType.GameFriend);
                if (comdt_friend_info == null)
                {
                    comdt_friend_info = Singleton<CFriendContoller>.instance.model.getFriendByName(userName, CFriendModel.FriendType.SNS);
                }
                if (comdt_friend_info != null)
                {
                    CFriendModel.FriendInGame friendInGaming = Singleton<CFriendContoller>.instance.model.GetFriendInGaming(comdt_friend_info.stUin.ullUid, comdt_friend_info.stUin.dwLogicWorldId);
                    if (friendInGaming != null)
                    {
                        userName = CInviteView.ConnectPlayerNameAndNickName(Utility.BytesConvert(userName), friendInGaming.NickName);
                    }
                    CUIListElementScript listItem = this.GetListItem(userName);
                    if (listItem != null)
                    {
                        listItem.get_transform().FindChild("Online").GetComponent<Text>().set_text(string.Format("<color=#ff0000>{0}</color>", Singleton<CTextManager>.instance.GetText("Invite_Friend_Tips_2")));
                    }
                    this.ChangeInviteStateList(comdt_friend_info.stUin.ullUid, enInviteState.BeRejcet);
                }
            }
        }

        [MessageHandler(0x7e2)]
        public static void OnInviteFriendRoom(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            byte bErrcode = msg.stPkgData.stInviteFriendJoinRoomRsp.bErrcode;
            string str = StringHelper.UTF8BytesToString(ref msg.stPkgData.stInviteFriendJoinRoomRsp.szFriendName);
            if (bErrcode == 20)
            {
                DateTime banTime = MonoSingleton<IDIPSys>.GetInstance().GetBanTime(COM_ACNT_BANTIME_TYPE.COM_ACNT_BANTIME_BANPLAYPVP);
                object[] args = new object[] { banTime.Year, banTime.Month, banTime.Day, banTime.Hour, banTime.Minute };
                string strContent = string.Format("您被禁止竞技！截止时间为{0}年{1}月{2}日{3}时{4}分", args);
                Singleton<CUIManager>.GetInstance().OpenMessageBox(strContent, false);
            }
            else
            {
                string refuseReason = CUIUtility.RemoveEmoji(Utility.UTF8Convert(msg.stPkgData.stInviteFriendJoinRoomRsp.szDenyReason));
                string str4 = GetInviteRoomFailReason(StringHelper.UTF8BytesToString(ref msg.stPkgData.stInviteFriendJoinRoomRsp.szFriendName), bErrcode, refuseReason);
                if (bErrcode == 14)
                {
                    Singleton<CUIManager>.GetInstance().OpenTips(str4, false, 3f, null, new object[0]);
                }
                else
                {
                    Singleton<CUIManager>.GetInstance().OpenTips(str4, false, 1.5f, null, new object[0]);
                }
            }
            Singleton<EventRouter>.instance.BroadCastEvent<byte, string>(EventID.INVITE_ROOM_ERRCODE_NTF, bErrcode, str);
        }

        [MessageHandler(0x7e9)]
        public static void OnInviteFriendTeam(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            byte bErrcode = msg.stPkgData.stInviteFriendJoinTeamRsp.bErrcode;
            string fName = StringHelper.UTF8BytesToString(ref msg.stPkgData.stInviteFriendJoinTeamRsp.szFriendName);
            uint timePunished = 0;
            uint punishType = 1;
            if (msg.stPkgData.stInviteFriendJoinTeamRsp.bErrcode == 0x11)
            {
                timePunished = msg.stPkgData.stInviteFriendJoinTeamRsp.dwParam;
                punishType = msg.stPkgData.stInviteFriendJoinTeamRsp.dwParam2;
            }
            string refuseReason = CUIUtility.RemoveEmoji(Utility.UTF8Convert(msg.stPkgData.stInviteFriendJoinTeamRsp.szDenyReason));
            string strContent = GetInviteTeamFailReason(fName, bErrcode, timePunished, punishType, refuseReason);
            if (bErrcode == 6)
            {
                Singleton<CUIManager>.GetInstance().OpenTips(strContent, false, 3f, null, new object[0]);
            }
            else
            {
                Singleton<CUIManager>.GetInstance().OpenTips(strContent, false, 1.5f, null, new object[0]);
            }
            Singleton<EventRouter>.instance.BroadCastEvent<byte, string>(EventID.INVITE_TEAM_ERRCODE_NTF, bErrcode, fName);
        }

        public void OnInviteListGameTimer(int index)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(PATH_INVITE_FORM);
            if (form != null)
            {
                CUIListScript component = form.get_transform().Find("Title/ListTab").GetComponent<CUIListScript>();
                if (component != null)
                {
                    switch (CInviteView.GetInviteListTab(component.GetSelectedIndex()))
                    {
                        case CInviteView.enInviteListTab.Friend:
                            CInviteView.SetInviteFriendData(form, this.m_inviteType);
                            break;

                        case CInviteView.enInviteListTab.GuildMember:
                            CInviteView.SetInviteGuildMemberData(form);
                            break;
                    }
                }
            }
            else
            {
                this.ClearInviteListGameTimer();
            }
        }

        private void OnInviteRoomErrCodeNtf(byte errorCode, string userName)
        {
            this.OnInviteErrCodeNtf(COM_INVITE_JOIN_TYPE.COM_INVITE_JOIN_ROOM, errorCode, userName);
        }

        private void OnInviteTeamErrCodeNtf(byte errorCode, string userName)
        {
            this.OnInviteErrCodeNtf(COM_INVITE_JOIN_TYPE.COM_INVITE_JOIN_TEAM, errorCode, userName);
        }

        private void OnLBSListChg()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(PATH_INVITE_FORM);
            if (form != null)
            {
                CInviteView.SetLBSData(form, this.m_inviteType);
            }
        }

        private void OnRefreshGuildMemberGameStateTimeout(int timerSequence)
        {
            if (this.m_isNeedRefreshGuildMemberPanel)
            {
                this.RefreshGuildMemberInvitePanel();
                this.m_isNeedRefreshGuildMemberPanel = false;
            }
        }

        public void OpenInviteForm(COM_INVITE_JOIN_TYPE inviteType)
        {
            this.m_stateList.Clear();
            this.m_isFirstlySelectGuildMemberTab = true;
            this.SortAllFriendList();
            this.m_allGuildMemberList = this.CreateGuildMemberInviteList();
            CUIFormScript form = Singleton<CUIManager>.GetInstance().OpenForm(PATH_INVITE_FORM, false, true);
            if (form != null)
            {
                this.m_inviteType = inviteType;
                CInviteView.InitListTab(form);
                CInviteView.SetInviteFriendData(form, inviteType);
                this.DispatchInviteUIEvent();
            }
            if (this.m_inviteType == COM_INVITE_JOIN_TYPE.COM_INVITE_JOIN_TEAM)
            {
                Singleton<CChatController>.instance.model.channelMgr.Clear(EChatChannel.Team, 0L, 0);
                Singleton<CChatController>.instance.model.channelMgr.SetChatTab(CChatChannelMgr.EChatTab.Team);
                Singleton<CChatController>.instance.ShowPanel(true, false);
                Singleton<CChatController>.instance.view.UpView(true);
                Singleton<CChatController>.instance.model.sysData.ClearEntryText();
            }
        }

        [MessageHandler(0x7f4)]
        public static void ReceiveGetGuildMemberGameStateRsp(CSPkg msg)
        {
            SCPKG_GET_GUILD_MEMBER_GAME_STATE_RSP stGetGuildMemberGameStateRsp = msg.stPkgData.stGetGuildMemberGameStateRsp;
            Singleton<CInviteSystem>.GetInstance().RefreshGuildMemberGameState(stGetGuildMemberGameStateRsp);
            Singleton<EventRouter>.instance.BroadCastEvent("MAIL_GUILD_MEM_UPDATE");
        }

        private void RefreshGuildMemberGameState(SCPKG_GET_GUILD_MEMBER_GAME_STATE_RSP rsp)
        {
            this.SetGuildMemberInviteListGameState(rsp, this.GetGuildMemberInviteList());
            Singleton<CInviteSystem>.GetInstance().RefreshGuildMemberInvitePanel();
            Singleton<CInviteSystem>.GetInstance().m_isNeedRefreshGuildMemberPanel = false;
        }

        private void RefreshGuildMemberInvitePanel()
        {
            if (IsGuildMatchInvite())
            {
                Singleton<CGuildMatchSystem>.GetInstance().RefreshGuildMatchGuildMemberInvitePanel();
            }
            else
            {
                this.RefreshNormalGuildMemberInvitePanel();
            }
        }

        private void RefreshNormalGuildMemberInvitePanel()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(PATH_INVITE_FORM);
            if (form != null)
            {
                this.SortAllGuildMemberList();
                CInviteView.SetInviteGuildMemberData(form);
            }
        }

        public void SendGetGuildMemberGameStateReq()
        {
            ListView<GuildMemInfo> guildMemberInviteList = this.GetGuildMemberInviteList();
            if (guildMemberInviteList == null)
            {
                DebugHelper.Assert(false, "guildMemberInviteList is null!!!");
            }
            else
            {
                this.SendSendGetGuildMemberGameStateReqRaw(guildMemberInviteList);
                this.m_isNeedRefreshGuildMemberPanel = true;
                Singleton<CTimerManager>.GetInstance().AddTimer(0xbb8, 1, new CTimer.OnTimeUpHandler(this.OnRefreshGuildMemberGameStateTimeout));
            }
        }

        public void SendSendGetGuildMemberGameStateReqRaw(ListView<GuildMemInfo> guildMemberList)
        {
            if (guildMemberList != null)
            {
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x7f3);
                CSPKG_GET_GUILD_MEMBER_GAME_STATE_REQ stGetGuildMemberGameStateReq = msg.stPkgData.stGetGuildMemberGameStateReq;
                int index = 0;
                for (int i = 0; i < guildMemberList.Count; i++)
                {
                    if (CGuildHelper.IsMemberOnline(guildMemberList[i]))
                    {
                        stGetGuildMemberGameStateReq.MemberUid[index] = guildMemberList[i].stBriefInfo.uulUid;
                        index++;
                    }
                }
                stGetGuildMemberGameStateReq.iMemberCnt = index;
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
            }
        }

        public void SetAndStartRefreshGuildMemberGameStateTimer(CUITimerScript timer)
        {
            timer.SetTotalTime(1000f);
            timer.SetOnChangedIntervalTime(10f);
            timer.StartTimer();
        }

        private void SetGameStateTextWidget(GameObject srcWidget)
        {
            if (srcWidget != null)
            {
                Transform transform = srcWidget.get_transform().get_parent();
                if (transform != null)
                {
                    Transform transform2 = transform.Find("Online");
                    if (transform2 != null)
                    {
                        transform2.GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("Guild_Has_Invited"));
                    }
                }
            }
        }

        public void SetGuildMemberInviteListGameState(SCPKG_GET_GUILD_MEMBER_GAME_STATE_RSP rsp, ListView<GuildMemInfo> guildMemberList)
        {
            if (guildMemberList != null)
            {
                for (int i = 0; i < rsp.iMemberCnt; i++)
                {
                    for (int j = 0; j < guildMemberList.Count; j++)
                    {
                        if (CGuildHelper.IsMemberOnline(guildMemberList[j]) && (rsp.astMemberInfo[i].ullUid == guildMemberList[j].stBriefInfo.uulUid))
                        {
                            guildMemberList[j].GameState = (COM_ACNT_GAME_STATE) rsp.astMemberInfo[i].bGameState;
                            guildMemberList[j].dwGameStartTime = rsp.astMemberInfo[i].dwGameStartTime;
                        }
                    }
                }
            }
        }

        public void ShowNewBeingInvitedUI(SCPKG_INVITE_JOIN_GAME_REQ info)
        {
            this.m_beInviteForm.Open(info);
        }

        public void SortAllFriendList()
        {
            this.allFriendList_internal = Singleton<CFriendContoller>.GetInstance().model.GetAllFriend(false);
            this.allFriendList_internal.Sort(new Comparison<COMDT_FRIEND_INFO>(CInviteSystem.InviteFriendsSort));
        }

        private void SortAllGuildMemberList()
        {
            if (this.m_allGuildMemberList != null)
            {
                this.m_allGuildMemberList.Sort(new Comparison<GuildMemInfo>(CGuildHelper.GuildMemberComparisonForInvite));
            }
        }

        public override void UnInit()
        {
            base.UnInit();
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Invite_SendInviteFriend, new CUIEventManager.OnUIEventHandler(this.OnInvite_SendInviteFriend));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Invite_SendInviteGuildMember, new CUIEventManager.OnUIEventHandler(this.OnInvite_SendInviteGuildMember));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Invite_SendInviteLBS, new CUIEventManager.OnUIEventHandler(this.OnInvite_SendInviteLBS));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Invite_AcceptInvite, new CUIEventManager.OnUIEventHandler(this.OnInvite_Accept));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Invite_RejectInvite, new CUIEventManager.OnUIEventHandler(this.OnInvite_Reject));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Invite_TimeOut, new CUIEventManager.OnUIEventHandler(this.OnInvateFriendTimeOut));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Invite_FriendListElementEnable, new CUIEventManager.OnUIEventHandler(this.OnInvite_FriendListElementEnable));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Invite_GuildMemberListElementEnable, new CUIEventManager.OnUIEventHandler(this.OnInvite_GuildMemberListElementEnable));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Invite_LBSListElementEnable, new CUIEventManager.OnUIEventHandler(this.OnInvite_LBSListElementEnable));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Invite_TabChange, new CUIEventManager.OnUIEventHandler(this.OnInvite_TabChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Invite_RefreshGameStateTimeout, new CUIEventManager.OnUIEventHandler(this.OnInvite_RefreshGameStateTimeout));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<byte, string>(EventID.INVITE_ROOM_ERRCODE_NTF, new Action<byte, string>(this, (IntPtr) this.OnInviteRoomErrCodeNtf));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<byte, string>(EventID.INVITE_TEAM_ERRCODE_NTF, new Action<byte, string>(this, (IntPtr) this.OnInviteTeamErrCodeNtf));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<CSPkg>("Friend_ADD_NTF", new Action<CSPkg>(this.OnFriendChange));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<CSPkg>("Friend_Delete_NTF", new Action<CSPkg>(this.OnFriendChange));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<CFriendModel.FriendType, ulong, uint, bool>("Chat_Friend_Online_Change", new Action<CFriendModel.FriendType, ulong, uint, bool>(this, (IntPtr) this.OnFriendOnlineChg));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.Friend_Game_State_Change, new Action(this, (IntPtr) this.OnFriendOnlineChg));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler("Friend_Game_State_Refresh", new Action(this, (IntPtr) this.OnFriendOnlineChg));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler("Friend_LBS_User_Refresh", new Action(this, (IntPtr) this.OnLBSListChg));
        }

        public stInviteInfo InviteInfo
        {
            get
            {
                return this.m_inviteInfo;
            }
            set
            {
                this.m_inviteInfo = value;
            }
        }

        public COM_INVITE_JOIN_TYPE InviteType
        {
            get
            {
                return this.m_inviteType;
            }
        }

        private ListView<COMDT_FRIEND_INFO> m_allFriendList
        {
            get
            {
                if (this.allFriendList_internal == null)
                {
                    this.SortAllFriendList();
                }
                return this.allFriendList_internal;
            }
        }

        public static uint RefreshLBSTime
        {
            get
            {
                if (s_refreshLBSTime == 0)
                {
                    ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0xd8);
                    if (dataByKey == null)
                    {
                        s_refreshLBSTime = 60;
                    }
                    else
                    {
                        s_refreshLBSTime = dataByKey.dwConfValue;
                    }
                }
                return s_refreshLBSTime;
            }
        }

        private enum enInviteState
        {
            None,
            Invited,
            BeRejcet
        }

        private class InviteState
        {
            public CInviteSystem.enInviteState state;
            public uint time;
            public ulong uid;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct stInviteInfo
        {
            public ulong playerUid;
            public uint playerLogicWorldId;
            public COM_INVITE_JOIN_TYPE joinType;
            public CInviteView.enInviteListTab objSrc;
            public int gameEntity;
            public int maxTeamNum;
            public void Clear()
            {
                this.playerUid = 0L;
                this.playerLogicWorldId = 0;
                this.joinType = COM_INVITE_JOIN_TYPE.COM_INVITE_JOIN_NULL;
                this.objSrc = CInviteView.enInviteListTab.Friend;
                this.gameEntity = 0;
                this.maxTeamNum = 0;
            }
        }
    }
}

