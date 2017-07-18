namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    [MessageHandlerClass]
    public class CGuildMatchSystem : Singleton<CGuildMatchSystem>
    {
        public static readonly string GuildMatchFormPath = "UGUI/Form/System/Guild/Form_Guild_Match.prefab";
        public static readonly string GuildMatchRecordFormPath = "UGUI/Form/System/Guild/Form_Guild_Match_Record.prefab";
        private const int GuildMatchRuleTextIndex = 15;
        public const int InviteMessageBoxAutoCloseTimeSeconds = 10;
        private CUIFormScript m_form;
        private ListView<GuildMemInfo> m_guildMemberInviteList;
        private ListView<GuildMemInfo> m_guildMemberScoreList;
        private CSDT_RANKING_LIST_ITEM_INFO[] m_guildSeasonScores;
        private CSDT_RANKING_LIST_ITEM_INFO[] m_guildWeekScores;
        public bool m_isGuildMatchBtnClicked;
        private bool m_isNeedRequestNewRecord = true;
        private CUIFormScript m_matchRecordForm;
        private COMDT_GUILD_MATCH_HISTORY_INFO[] m_matchRecords;
        private List<KeyValuePair<ulong, ListView<TeamPlayerInfo>>> m_teamInfos;
        public const int NeedRequestNewRecordTimeMilliSeconds = 0x493e0;
        public const int RemindButtonCdSeconds = 10;
        public const int TeamSlotCount = 5;

        private void AddMemberToTeam(ulong memberNewTeamLeaderUid, GuildMemInfo memberInfo)
        {
            if ((this.m_teamInfos != null) && (memberInfo != null))
            {
                for (int i = 0; i < this.m_teamInfos.Count; i++)
                {
                    KeyValuePair<ulong, ListView<TeamPlayerInfo>> pair2 = this.m_teamInfos[i];
                    if (memberNewTeamLeaderUid == pair2.Key)
                    {
                        KeyValuePair<ulong, ListView<TeamPlayerInfo>> pair3 = this.m_teamInfos[i];
                        if (!this.FindAndReplaceEmptyPlayerSlot(pair3.Value, memberInfo))
                        {
                            TeamPlayerInfo item = this.CreateTeamPlayerInfoObj(memberInfo);
                            KeyValuePair<ulong, ListView<TeamPlayerInfo>> pair4 = this.m_teamInfos[i];
                            if (pair4.Value.Count < 5)
                            {
                                KeyValuePair<ulong, ListView<TeamPlayerInfo>> pair5 = this.m_teamInfos[i];
                                pair5.Value.Add(item);
                            }
                        }
                        if (CGuildHelper.IsSelf(memberInfo.stBriefInfo.uulUid) && (i != 0))
                        {
                            KeyValuePair<ulong, ListView<TeamPlayerInfo>> pair = this.m_teamInfos[i];
                            this.m_teamInfos.RemoveAt(i);
                            this.m_teamInfos.Insert(0, pair);
                        }
                        return;
                    }
                }
            }
        }

        public void Clear()
        {
            if (this.m_teamInfos != null)
            {
                this.m_teamInfos.Clear();
                this.m_teamInfos = null;
            }
            if (this.m_guildMemberInviteList != null)
            {
                this.m_guildMemberInviteList.Clear();
                this.m_guildMemberInviteList = null;
            }
            if (this.m_guildMemberScoreList != null)
            {
                this.m_guildMemberScoreList.Clear();
                this.m_guildMemberScoreList = null;
            }
            this.m_guildSeasonScores = null;
            this.m_guildWeekScores = null;
            this.m_matchRecords = null;
            this.m_isNeedRequestNewRecord = true;
        }

        public void CreateGuildMatchAllTeams()
        {
            if (this.m_teamInfos != null)
            {
                this.m_teamInfos.Clear();
                this.m_teamInfos = null;
            }
            this.m_teamInfos = new List<KeyValuePair<ulong, ListView<TeamPlayerInfo>>>();
            ListView<GuildMemInfo> guildMemberInfos = CGuildHelper.GetGuildMemberInfos();
            if (guildMemberInfos != null)
            {
                for (int i = 0; i < guildMemberInfos.Count; i++)
                {
                    if (guildMemberInfos[i].GuildMatchInfo.ullTeamLeaderUid > 0L)
                    {
                        bool flag = false;
                        for (int j = 0; j < this.m_teamInfos.Count; j++)
                        {
                            KeyValuePair<ulong, ListView<TeamPlayerInfo>> pair = this.m_teamInfos[j];
                            if (pair.Key == guildMemberInfos[i].GuildMatchInfo.ullTeamLeaderUid)
                            {
                                KeyValuePair<ulong, ListView<TeamPlayerInfo>> pair2 = this.m_teamInfos[j];
                                if (this.IsTeamLeader(guildMemberInfos[i].stBriefInfo.uulUid, pair2.Key))
                                {
                                    TeamPlayerInfo item = this.CreateTeamPlayerInfoObj(guildMemberInfos[i]);
                                    KeyValuePair<ulong, ListView<TeamPlayerInfo>> pair3 = this.m_teamInfos[j];
                                    pair3.Value.Insert(0, item);
                                }
                                else
                                {
                                    KeyValuePair<ulong, ListView<TeamPlayerInfo>> pair4 = this.m_teamInfos[j];
                                    if (!this.FindAndReplaceEmptyPlayerSlot(pair4.Value, guildMemberInfos[i]))
                                    {
                                        TeamPlayerInfo info2 = this.CreateTeamPlayerInfoObj(guildMemberInfos[i]);
                                        KeyValuePair<ulong, ListView<TeamPlayerInfo>> pair5 = this.m_teamInfos[j];
                                        if (pair5.Value.Count < 5)
                                        {
                                            KeyValuePair<ulong, ListView<TeamPlayerInfo>> pair6 = this.m_teamInfos[j];
                                            pair6.Value.Add(info2);
                                        }
                                    }
                                }
                                flag = true;
                                break;
                            }
                        }
                        if (!flag)
                        {
                            this.CreateNewTeam(guildMemberInfos[i]);
                        }
                    }
                }
            }
        }

        private void CreateNewTeam(GuildMemInfo guildMemberInfo)
        {
            if ((guildMemberInfo != null) && (this.m_teamInfos != null))
            {
                ListView<TeamPlayerInfo> view = new ListView<TeamPlayerInfo>();
                view.Add(this.CreateTeamPlayerInfoObj(guildMemberInfo));
                KeyValuePair<ulong, ListView<TeamPlayerInfo>> item = new KeyValuePair<ulong, ListView<TeamPlayerInfo>>(guildMemberInfo.GuildMatchInfo.ullTeamLeaderUid, view);
                if (this.IsSameTeamWithSelf(item.Key))
                {
                    this.m_teamInfos.Insert(0, item);
                }
                else
                {
                    this.m_teamInfos.Add(item);
                }
            }
        }

        private TeamPlayerInfo CreateTeamPlayerInfoObj(GuildMemInfo guildMemInfo)
        {
            return new TeamPlayerInfo(guildMemInfo.stBriefInfo.uulUid, guildMemInfo.stBriefInfo.sName, guildMemInfo.stBriefInfo.szHeadUrl, Convert.ToBoolean(guildMemInfo.GuildMatchInfo.bIsReady));
        }

        private bool FindAndReplaceEmptyPlayerSlot(ListView<TeamPlayerInfo> teamPlayerInfos, GuildMemInfo memberInfo)
        {
            for (int i = 0; i < teamPlayerInfos.Count; i++)
            {
                if (teamPlayerInfos[i].Uid == 0)
                {
                    teamPlayerInfos[i].Uid = memberInfo.stBriefInfo.uulUid;
                    teamPlayerInfos[i].Name = memberInfo.stBriefInfo.sName;
                    teamPlayerInfos[i].HeadUrl = memberInfo.stBriefInfo.szHeadUrl;
                    teamPlayerInfos[i].IsReady = Convert.ToBoolean(memberInfo.GuildMatchInfo.bIsReady);
                    return true;
                }
            }
            return false;
        }

        private int GetCurLeaderNum()
        {
            ListView<GuildMemInfo> guildMemberInfos = CGuildHelper.GetGuildMemberInfos();
            int num = 0;
            for (int i = 0; i < guildMemberInfos.Count; i++)
            {
                if (Convert.ToBoolean(guildMemberInfos[i].GuildMatchInfo.bIsLeader))
                {
                    num++;
                }
            }
            return num;
        }

        public List<COBSystem.stOBGuild> GetGuidMatchObInfo()
        {
            List<COBSystem.stOBGuild> list = new List<COBSystem.stOBGuild>();
            GuildInfo currentGuildInfo = Singleton<CGuildModel>.GetInstance().CurrentGuildInfo;
            if ((currentGuildInfo != null) && (currentGuildInfo.GuildMatchObInfos != null))
            {
                for (int i = 0; i < currentGuildInfo.GuildMatchObInfos.Count; i++)
                {
                    if (currentGuildInfo.GuildMatchObInfos[i].dwBeginTime > 0)
                    {
                        for (int j = 0; j < currentGuildInfo.GuildMatchObInfos[i].astHeroInfo.Length; j++)
                        {
                            GuildMemInfo guildMemberInfoByUid = CGuildHelper.GetGuildMemberInfoByUid(currentGuildInfo.GuildMatchObInfos[i].astHeroInfo[j].ullUid);
                            if (guildMemberInfoByUid != null)
                            {
                                COBSystem.stOBGuild item = new COBSystem.stOBGuild();
                                item.obUid = currentGuildInfo.GuildMatchObInfos[i].ullUid;
                                item.playerUid = currentGuildInfo.GuildMatchObInfos[i].astHeroInfo[j].ullUid;
                                item.playerName = guildMemberInfoByUid.stBriefInfo.sName;
                                item.teamName = this.GetTeamName(currentGuildInfo.GuildMatchObInfos[i].ullUid);
                                item.headUrl = CGuildHelper.GetHeadUrl(guildMemberInfoByUid.stBriefInfo.szHeadUrl);
                                item.dwHeroID = currentGuildInfo.GuildMatchObInfos[i].astHeroInfo[j].dwHeroID;
                                item.dwStartTime = currentGuildInfo.GuildMatchObInfos[i].dwBeginTime;
                                item.bGrade = (byte) CLadderSystem.ConvertEloToRank(guildMemberInfoByUid.stBriefInfo.dwScoreOfRank);
                                item.dwClass = guildMemberInfoByUid.stBriefInfo.dwClassOfRank;
                                item.dwObserveNum = currentGuildInfo.GuildMatchObInfos[i].dwOBCnt;
                                list.Add(item);
                            }
                        }
                    }
                }
            }
            return list;
        }

        public ListView<GuildMemInfo> GetGuildMemberInviteList()
        {
            return this.m_guildMemberInviteList;
        }

        private CSDT_RANKING_LIST_ITEM_INFO[] GetGuildScoreRankInfo()
        {
            return (!this.IsCurrentSeasonScoreTab() ? this.m_guildWeekScores : this.m_guildSeasonScores);
        }

        private string GetMatchStartedTimeStr(ulong teamLeaderUid)
        {
            ListView<COMDT_GUILD_MATCH_OB_INFO> guildMatchObInfos = Singleton<CGuildModel>.GetInstance().CurrentGuildInfo.GuildMatchObInfos;
            uint dwBeginTime = 0;
            if (guildMatchObInfos != null)
            {
                for (int i = 0; i < guildMatchObInfos.Count; i++)
                {
                    if (teamLeaderUid == guildMatchObInfos[i].ullUid)
                    {
                        dwBeginTime = guildMatchObInfos[i].dwBeginTime;
                        break;
                    }
                }
            }
            if (dwBeginTime > 0)
            {
                uint num3 = ((uint) CRoleInfo.GetCurrentUTCTime()) - dwBeginTime;
                TimeSpan span = new TimeSpan(num3 * 0x989680L);
                string[] args = new string[] { ((int) span.TotalMinutes).ToString() };
                return Singleton<CTextManager>.GetInstance().GetText("GuildMatch_Status_Game_Started", args);
            }
            return string.Empty;
        }

        private ulong GetObUid(ulong teamLeaderUid)
        {
            ListView<COMDT_GUILD_MATCH_OB_INFO> guildMatchObInfos = Singleton<CGuildModel>.GetInstance().CurrentGuildInfo.GuildMatchObInfos;
            if (guildMatchObInfos != null)
            {
                for (int i = 0; i < guildMatchObInfos.Count; i++)
                {
                    if (teamLeaderUid == guildMatchObInfos[i].ullUid)
                    {
                        return teamLeaderUid;
                    }
                }
            }
            return 0L;
        }

        private string GetReachMatchCntLimitTip()
        {
            return Singleton<CTextManager>.GetInstance().GetText(!CGuildHelper.IsGuildMatchLeaderPosition(Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID) ? "GuildMatch_Normal_Player_Match_Times_Reach_Limit" : "GuildMatch_Leader_Match_Times_Reach_Limit");
        }

        private uint GetSelfGuildScoreRankNo()
        {
            CSDT_RANKING_LIST_ITEM_INFO[] guildScoreRankInfo = this.GetGuildScoreRankInfo();
            if (guildScoreRankInfo != null)
            {
                ulong guildUid = CGuildHelper.GetGuildUid();
                for (int i = 0; i < guildScoreRankInfo.Length; i++)
                {
                    if (guildUid == ulong.Parse(StringHelper.UTF8BytesToString(ref guildScoreRankInfo[i].szOpenID)))
                    {
                        return guildScoreRankInfo[i].dwRankNo;
                    }
                }
            }
            return 0;
        }

        private int GetSelfIndexInGuildMemberScoreList()
        {
            if (this.m_guildMemberScoreList != null)
            {
                for (int i = 0; i < this.m_guildMemberScoreList.Count; i++)
                {
                    if (this.m_guildMemberScoreList[i].stBriefInfo.uulUid == Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        private string GetTeamName(ulong teamLeaderUid)
        {
            GuildMemInfo guildMemberInfoByUid = CGuildHelper.GetGuildMemberInfoByUid(teamLeaderUid);
            if (guildMemberInfoByUid != null)
            {
                return guildMemberInfoByUid.stBriefInfo.sName;
            }
            return string.Empty;
        }

        private int GetTotalLeaderNum()
        {
            ResGuildLevel dataByKey = GameDataMgr.guildLevelDatabin.GetDataByKey((long) CGuildHelper.GetGuildLevel());
            if (dataByKey != null)
            {
                return dataByKey.bTeamCnt;
            }
            return 0;
        }

        public override void Init()
        {
            base.Init();
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_OnMatchFormOpened, new CUIEventManager.OnUIEventHandler(this.OnMatchFormOpened));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_OnMatchFormClosed, new CUIEventManager.OnUIEventHandler(this.OnMatchFormClosed));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_OpenMatchForm, new CUIEventManager.OnUIEventHandler(this.OnOpenMatchForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_OpenMatchRecordForm, new CUIEventManager.OnUIEventHandler(this.OnOpenMatchRecordForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_StartGame, new CUIEventManager.OnUIEventHandler(this.OnStartGame));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_OBGame, new CUIEventManager.OnUIEventHandler(this.OnOBGame));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_ReadyGame, new CUIEventManager.OnUIEventHandler(this.OnReadyGame));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_CancelReadyGame, new CUIEventManager.OnUIEventHandler(this.OnCancelReadyGame));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_RankTabChanged, new CUIEventManager.OnUIEventHandler(this.OnRankTabChanged));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_GuildScoreListElementEnabled, new CUIEventManager.OnUIEventHandler(this.OnGuildScoreListElementEnabled));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_MemberScoreListElementEnabled, new CUIEventManager.OnUIEventHandler(this.OnMemberScoreListElementEnabled));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_MemberInviteListElementEnabled, new CUIEventManager.OnUIEventHandler(this.OnMemberInviteListElementEnabled));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_Invite, new CUIEventManager.OnUIEventHandler(this.OnInvite));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_Kick, new CUIEventManager.OnUIEventHandler(this.OnKick));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_KickConfirm, new CUIEventManager.OnUIEventHandler(this.OnKickConfirm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_AppointOrCancelLeader, new CUIEventManager.OnUIEventHandler(this.OnAppointOrCancelLeader));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_AppointOrCancelLeaderConfirm, new CUIEventManager.OnUIEventHandler(this.OnAppointOrCancelLeaderConfirm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_Accept_Invite, new CUIEventManager.OnUIEventHandler(this.OnAcceptInvite));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_Refuse_Invite, new CUIEventManager.OnUIEventHandler(this.OnRefuseInvite));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_RefreshGameStateTimeout, new CUIEventManager.OnUIEventHandler(this.OnRefreshGameStateTimeout));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_ObWaitingTimeout, new CUIEventManager.OnUIEventHandler(this.OnObWaitingTimeout));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_RecordListElementEnabled, new CUIEventManager.OnUIEventHandler(this.OnRecordListElementEnabled));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_RankSubTitleSliderValueChanged, new CUIEventManager.OnUIEventHandler(this.OnSubRankSliderValueChanged));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_OpenMatchFormAndReadyGame, new CUIEventManager.OnUIEventHandler(this.OnOpenMatchFormAndReadyGame));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_Remind_Ready, new CUIEventManager.OnUIEventHandler(this.OnRemindReady));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_Open_Rule, new CUIEventManager.OnUIEventHandler(this.OnOpenRule));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_Team_List_Element_Enabled, new CUIEventManager.OnUIEventHandler(this.OnTeamListElementEnabled));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_InviteConfirm, new CUIEventManager.OnUIEventHandler(this.OnInviteConfirm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Match_RemindButtonCdOver, new CUIEventManager.OnUIEventHandler(this.OnRemindButtonCdOver));
            Singleton<EventRouter>.GetInstance().AddEventHandler<SCPKG_GET_RANKING_LIST_RSP>("Guild_Get_Guild_Match_Season_Rank", new Action<SCPKG_GET_RANKING_LIST_RSP>(this.OnGetGuildMatchSeasonRank));
            Singleton<EventRouter>.GetInstance().AddEventHandler<SCPKG_GET_RANKING_LIST_RSP>("Guild_Get_Guild_Match_Week_Rank", new Action<SCPKG_GET_RANKING_LIST_RSP>(this.OnGetGuildMatchWeekRank));
        }

        private void InitMemberInviteList()
        {
            if (this.m_form != null)
            {
                this.m_guildMemberInviteList = Singleton<CInviteSystem>.GetInstance().CreateGuildMemberInviteList();
                this.m_guildMemberInviteList.Sort(new Comparison<GuildMemInfo>(CGuildHelper.GuildMemberComparisonForInvite));
                Singleton<CInviteSystem>.GetInstance().SendGetGuildMemberGameStateReq();
                CUITimerScript component = this.m_form.GetWidget(15).GetComponent<CUITimerScript>();
                Singleton<CInviteSystem>.GetInstance().SetAndStartRefreshGuildMemberGameStateTimer(component);
            }
        }

        private void InitRankTabList()
        {
            if (this.m_form != null)
            {
                ListView<string> view = new ListView<string>();
                view.Add(Singleton<CTextManager>.GetInstance().GetText("GuildMatch_GuildMatchGuildScore"));
                view.Add(Singleton<CTextManager>.GetInstance().GetText("GuildMatch_GuildMatchMemberScore"));
                if (this.IsSelfBelongedTeamLeader())
                {
                    view.Add(Singleton<CTextManager>.GetInstance().GetText("Common_Invite"));
                }
                CUIListScript component = this.m_form.GetWidget(4).GetComponent<CUIListScript>();
                component.SetElementAmount(view.Count);
                for (int i = 0; i < view.Count; i++)
                {
                    component.GetElemenet(i).get_transform().Find("txtName").GetComponent<Text>().set_text(view[i]);
                }
                if (this.IsSelfBelongedTeamLeader())
                {
                    component.SelectElement(2, true);
                    this.InitMemberInviteList();
                }
                else
                {
                    component.SelectElement(0, true);
                }
            }
        }

        private bool IsCurrentSeasonScoreTab()
        {
            if (this.m_form == null)
            {
                return false;
            }
            return (((int) this.m_form.GetWidget(0x19).GetComponent<Slider>().get_value()) == 0);
        }

        public bool IsInGuildMatchTime()
        {
            uint dwConfValue = GameDataMgr.guildMiscDatabin.GetDataByKey((uint) 0x2f).dwConfValue;
            return (CUICommonSystem.GetMatchOpenState(RES_BATTLE_MAP_TYPE.RES_BATTLE_MAP_TYPE_GUILDMATCH, dwConfValue).matchState == enMatchOpenState.enMatchOpen_InActiveTime);
        }

        public bool IsInObDelayedTime(ulong obUid)
        {
            uint num;
            return this.IsInObDelayedTime(obUid, out num);
        }

        private bool IsInObDelayedTime(ulong teamLeaderUid, out uint obWaitingTime)
        {
            ListView<COMDT_GUILD_MATCH_OB_INFO> guildMatchObInfos = Singleton<CGuildModel>.GetInstance().CurrentGuildInfo.GuildMatchObInfos;
            uint dwBeginTime = 0;
            if (guildMatchObInfos != null)
            {
                for (int i = 0; i < guildMatchObInfos.Count; i++)
                {
                    if (teamLeaderUid == guildMatchObInfos[i].ullUid)
                    {
                        dwBeginTime = guildMatchObInfos[i].dwBeginTime;
                        break;
                    }
                }
            }
            if (dwBeginTime > 0)
            {
                uint num3 = ((uint) CRoleInfo.GetCurrentUTCTime()) - dwBeginTime;
                uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0xd7).dwConfValue;
                if (dwConfValue > num3)
                {
                    obWaitingTime = dwConfValue - num3;
                    return true;
                }
            }
            obWaitingTime = 0;
            return false;
        }

        public bool IsInTeam(ulong playerTeamLeaderUid, ulong teamLeaderUid)
        {
            return (playerTeamLeaderUid == teamLeaderUid);
        }

        private bool IsLeaderNumFull()
        {
            int curLeaderNum = this.GetCurLeaderNum();
            int totalLeaderNum = this.GetTotalLeaderNum();
            return (curLeaderNum >= totalLeaderNum);
        }

        private bool IsMemberInOtherTeam(ulong invitedMemberUid, ref string teamName)
        {
            GuildMemInfo guildMemberInfoByUid = CGuildHelper.GetGuildMemberInfoByUid(invitedMemberUid);
            if (((guildMemberInfoByUid != null) && (guildMemberInfoByUid.GuildMatchInfo.ullTeamLeaderUid > 0L)) && (guildMemberInfoByUid.GuildMatchInfo.ullTeamLeaderUid != Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID))
            {
                teamName = this.GetTeamName(guildMemberInfoByUid.GuildMatchInfo.ullTeamLeaderUid);
                return true;
            }
            return false;
        }

        private bool IsNeedOpenGuildMatchForm(SCPKG_GUILD_MATCH_MEMBER_CHG_NTF ntf)
        {
            if ((Singleton<CUIManager>.GetInstance().GetForm(GuildMatchFormPath) == null) && Utility.IsCanShowPrompt())
            {
                for (int i = 0; i < ntf.bCnt; i++)
                {
                    if ((ntf.astChgInfo[i].ullUid == Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID) && (ntf.astChgInfo[i].ullTeamLeaderUid > 0L))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool IsNeedRefreshRankTab()
        {
            if (this.m_form != null)
            {
                int elementAmount = this.m_form.GetWidget(4).GetComponent<CUIListScript>().GetElementAmount();
                if (((elementAmount == 2) && this.IsSelfBelongedTeamLeader()) || ((elementAmount == 3) && !this.IsSelfBelongedTeamLeader()))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsReadyForGame(ListView<TeamPlayerInfo> teamPlayerInfos, ulong playerUid)
        {
            if (teamPlayerInfos != null)
            {
                for (int i = 0; i < teamPlayerInfos.Count; i++)
                {
                    if (teamPlayerInfos[i].Uid == playerUid)
                    {
                        return teamPlayerInfos[i].IsReady;
                    }
                }
            }
            return false;
        }

        private bool IsSameTeamWithSelf(ulong playerTeamLeaderUid)
        {
            return (playerTeamLeaderUid == CGuildHelper.GetPlayerGuildMemberInfo().GuildMatchInfo.ullTeamLeaderUid);
        }

        private bool IsSelfBelongedTeamLeader()
        {
            return (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID == CGuildHelper.GetPlayerGuildMemberInfo().GuildMatchInfo.ullTeamLeaderUid);
        }

        public bool IsSelfInAnyTeam()
        {
            ulong playerUllUID = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID;
            if (this.m_teamInfos != null)
            {
                for (int i = 0; i < this.m_teamInfos.Count; i++)
                {
                    KeyValuePair<ulong, ListView<TeamPlayerInfo>> pair;
                    KeyValuePair<ulong, ListView<TeamPlayerInfo>> pair2;
                    int num3 = 0;
                    goto Label_0054;
                Label_0029:
                    pair = this.m_teamInfos[i];
                    if (pair.Value[num3].Uid == playerUllUID)
                    {
                        return true;
                    }
                    num3++;
                Label_0054:
                    pair2 = this.m_teamInfos[i];
                    if (num3 < pair2.Value.Count)
                    {
                        goto Label_0029;
                    }
                }
            }
            return false;
        }

        private bool IsSlotOccupied(TeamPlayerInfo playerInfo)
        {
            return ((playerInfo != null) && (playerInfo.Uid > 0L));
        }

        private bool IsTeamAllPlayerReadyForGame(ListView<TeamPlayerInfo> teamPlayerInfos, ulong teamLeaderUid)
        {
            if (teamPlayerInfos == null)
            {
                return false;
            }
            if (teamPlayerInfos.Count < 5)
            {
                return false;
            }
            for (int i = 0; i < 5; i++)
            {
                if ((teamPlayerInfos[i].Uid == 0) || (!teamPlayerInfos[i].IsReady && !this.IsTeamLeader(teamPlayerInfos[i].Uid, teamLeaderUid)))
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsTeamFull(ulong teamLeaderUid)
        {
            if (this.m_teamInfos != null)
            {
                for (int i = 0; i < this.m_teamInfos.Count; i++)
                {
                    KeyValuePair<ulong, ListView<TeamPlayerInfo>> pair3;
                    KeyValuePair<ulong, ListView<TeamPlayerInfo>> pair4;
                    KeyValuePair<ulong, ListView<TeamPlayerInfo>> pair = this.m_teamInfos[i];
                    if (pair.Key != teamLeaderUid)
                    {
                        goto Label_00A2;
                    }
                    KeyValuePair<ulong, ListView<TeamPlayerInfo>> pair2 = this.m_teamInfos[i];
                    if (pair2.Value.Count < 5)
                    {
                        return false;
                    }
                    int num2 = 0;
                    goto Label_0080;
                Label_0054:
                    pair3 = this.m_teamInfos[i];
                    if (!this.IsSlotOccupied(pair3.Value[num2]))
                    {
                        return false;
                    }
                    num2++;
                Label_0080:
                    pair4 = this.m_teamInfos[i];
                    if (num2 < pair4.Value.Count)
                    {
                        goto Label_0054;
                    }
                    return true;
                Label_00A2:;
                }
            }
            return false;
        }

        private bool IsTeamLeader(ulong playerUid, ulong teamLeaderUid)
        {
            return (playerUid == teamLeaderUid);
        }

        private void OnAcceptInvite(CUIEvent uiEvent)
        {
            this.RequestDealGuildMatchMemberInvite(uiEvent.m_eventParams.commonUInt64Param1, true);
        }

        private void OnAppointOrCancelLeader(CUIEvent uiEvent)
        {
            if (!CGuildSystem.HasAppointMatchLeaderAuthority())
            {
                Singleton<CUIManager>.GetInstance().OpenTips("Guild_Appoint_You_Have_No_Authority", true, 1.5f, null, new object[0]);
            }
            else
            {
                GuildMemInfo currentSelectedMemberInfo = Singleton<CGuildModel>.GetInstance().CurrentSelectedMemberInfo;
                if (currentSelectedMemberInfo == null)
                {
                    DebugHelper.Assert(false, "guildMemInfo is null!!!");
                }
                else
                {
                    if (!CGuildHelper.IsGuildMatchLeaderPosition(currentSelectedMemberInfo))
                    {
                        if (this.IsLeaderNumFull())
                        {
                            Singleton<CUIManager>.GetInstance().OpenTips("GuildMatch_Leader_Full", true, 1.5f, null, new object[0]);
                            return;
                        }
                        if (CGuildHelper.IsInGuildMatchJoinLimitTime(currentSelectedMemberInfo))
                        {
                            Singleton<CUIManager>.GetInstance().OpenTips("GuildMatch_Member_Join_Time_Limit_Appoint_Leader_Tip", true, 1.5f, null, new object[0]);
                            return;
                        }
                    }
                    string[] args = new string[] { currentSelectedMemberInfo.stBriefInfo.sName };
                    string text = Singleton<CTextManager>.GetInstance().GetText(!CGuildHelper.IsGuildMatchLeaderPosition(currentSelectedMemberInfo) ? "GuildMatch_Apooint_Leader_Confirm" : "GuildMatch_Cancel_Leader_Confirm", args);
                    Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(text, enUIEventID.Guild_Match_AppointOrCancelLeaderConfirm, enUIEventID.None, false);
                }
            }
        }

        private void OnAppointOrCancelLeaderConfirm(CUIEvent uiEvent)
        {
            GuildMemInfo currentSelectedMemberInfo = Singleton<CGuildModel>.GetInstance().CurrentSelectedMemberInfo;
            if (currentSelectedMemberInfo == null)
            {
                DebugHelper.Assert(false, "guildMemInfo is null!!!");
            }
            else
            {
                this.RequestChangeGuildMatchLeader(currentSelectedMemberInfo.stBriefInfo.uulUid, !CGuildHelper.IsGuildMatchLeaderPosition(currentSelectedMemberInfo));
            }
        }

        private void OnCancelReadyGame(CUIEvent uiEvent)
        {
            this.RequestSetGuildMatchReady(false);
        }

        private void OnGetGuildMatchSeasonRank(SCPKG_GET_RANKING_LIST_RSP rsp)
        {
            this.m_guildSeasonScores = new CSDT_RANKING_LIST_ITEM_INFO[rsp.stRankingListDetail.stOfSucc.dwItemNum];
            for (int i = 0; i < rsp.stRankingListDetail.stOfSucc.dwItemNum; i++)
            {
                this.m_guildSeasonScores[i] = rsp.stRankingListDetail.stOfSucc.astItemDetail[i];
            }
            if (this.IsCurrentSeasonScoreTab())
            {
                this.RefreshGuildScoreRankListAndSelfGuildScorePanel();
            }
        }

        private void OnGetGuildMatchWeekRank(SCPKG_GET_RANKING_LIST_RSP rsp)
        {
            this.m_guildWeekScores = new CSDT_RANKING_LIST_ITEM_INFO[rsp.stRankingListDetail.stOfSucc.dwItemNum];
            for (int i = 0; i < rsp.stRankingListDetail.stOfSucc.dwItemNum; i++)
            {
                this.m_guildWeekScores[i] = rsp.stRankingListDetail.stOfSucc.astItemDetail[i];
            }
            if (!this.IsCurrentSeasonScoreTab())
            {
                this.RefreshGuildScoreRankListAndSelfGuildScorePanel();
            }
        }

        private void OnGuildScoreListElementEnabled(CUIEvent uiEvent)
        {
            this.SetGuildScoreListElement(uiEvent.m_srcWidget, uiEvent.m_srcWidgetIndexInBelongedList);
        }

        private void OnInvite(CUIEvent uiEvent)
        {
            if (this.IsTeamFull(Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID))
            {
                Singleton<CUIManager>.GetInstance().OpenTips("GuildMatch_Team_Member_Full", true, 1.5f, null, new object[0]);
            }
            else
            {
                ulong uid = uiEvent.m_eventParams.commonUInt64Param1;
                GuildMemInfo guildMemberInfoByUid = CGuildHelper.GetGuildMemberInfoByUid(uid);
                if ((guildMemberInfoByUid != null) && CGuildHelper.IsInGuildMatchJoinLimitTime(guildMemberInfoByUid))
                {
                    Singleton<CUIManager>.GetInstance().OpenTips("GuildMatch_Member_Join_Time_Limit_Join_Match_Tip", true, 1.5f, null, new object[0]);
                }
                else
                {
                    string teamName = string.Empty;
                    if (this.IsMemberInOtherTeam(uid, ref teamName))
                    {
                        string[] args = new string[] { teamName };
                        string text = Singleton<CTextManager>.GetInstance().GetText("GuildMatch_Member_Already_In_Other_Team", args);
                        stUIEventParams par = new stUIEventParams();
                        par.commonUInt64Param1 = uid;
                        par.commonGameObject = uiEvent.m_srcWidget;
                        Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(text, enUIEventID.Guild_Match_InviteConfirm, enUIEventID.None, par, false);
                    }
                    else
                    {
                        this.RealInvite(uid, uiEvent.m_srcWidget);
                    }
                }
            }
        }

        private void OnInviteConfirm(CUIEvent uiEvent)
        {
            this.RealInvite(uiEvent.m_eventParams.commonUInt64Param1, uiEvent.m_eventParams.commonGameObject);
        }

        private void OnKick(CUIEvent uiEvent)
        {
            stUIEventParams par = new stUIEventParams();
            par.commonUInt64Param1 = uiEvent.m_eventParams.commonUInt64Param1;
            par.commonBool = uiEvent.m_eventParams.commonBool;
            bool commonBool = uiEvent.m_eventParams.commonBool;
            Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText(!commonBool ? "GuildMatch_Kick_Confirm_Msg" : "GuildMatch_Leave_Team_Confirm_Msg"), enUIEventID.Guild_Match_KickConfirm, enUIEventID.None, par, false);
        }

        private void OnKickConfirm(CUIEvent uiEvent)
        {
            ulong memberUid = uiEvent.m_eventParams.commonUInt64Param1;
            if (uiEvent.m_eventParams.commonBool)
            {
                this.RequestLeaveGuildMatchTeam();
            }
            else
            {
                this.RequestKickGuildMatchMember(memberUid);
            }
        }

        private void OnMatchFormClosed(CUIEvent uiEvent)
        {
            CChatUT.LeaveGuildMatch();
        }

        private void OnMatchFormOpened(CUIEvent uiEvent)
        {
            CChatUT.EnterGuildMatch();
        }

        private void OnMemberInviteListElementEnabled(CUIEvent uiEvent)
        {
            if (this.m_guildMemberInviteList != null)
            {
                int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
                GameObject srcWidget = uiEvent.m_srcWidget;
                if ((srcWidgetIndexInBelongedList >= 0) && (srcWidgetIndexInBelongedList < this.m_guildMemberInviteList.Count))
                {
                    CInviteView.UpdateGuildMemberListElement(srcWidget, this.m_guildMemberInviteList[srcWidgetIndexInBelongedList], true);
                }
            }
        }

        private void OnMemberScoreListElementEnabled(CUIEvent uiEvent)
        {
            this.SetMemberScoreListElement(uiEvent.m_srcWidget, uiEvent.m_srcWidgetIndexInBelongedList);
        }

        private void OnOBGame(CUIEvent uiEvent)
        {
            this.RequestObGuildMatch(uiEvent.m_eventParams.commonUInt64Param1);
        }

        private void OnObWaitingTimeout(CUIEvent uiEvent)
        {
            CUICommonSystem.SetButtonEnable(uiEvent.m_srcWidget.get_transform().get_parent().GetComponent<Button>(), true, true, true);
        }

        private void OnOpenMatchForm(CUIEvent uiEvent)
        {
            Singleton<CBattleGuideManager>.instance.OpenBannerDlgByBannerGuideId(11, null, false);
            this.m_isGuildMatchBtnClicked = true;
            Singleton<CGuildInfoView>.GetInstance().DelGuildMatchBtnNewFlag();
            this.OpenMatchForm(false);
        }

        private void OnOpenMatchFormAndReadyGame(CUIEvent uiEvent)
        {
            this.OpenMatchForm(true);
        }

        private void OnOpenMatchRecordForm(CUIEvent uiEvent)
        {
            if (this.m_isNeedRequestNewRecord)
            {
                this.m_isNeedRequestNewRecord = false;
                Singleton<CTimerManager>.GetInstance().AddTimer(0x493e0, 1, new CTimer.OnTimeUpHandler(this.OnRequestNewRecordTimeout));
                this.RequestGetGuildMatchHistory();
            }
            else
            {
                this.OpenMatchRecordForm();
            }
        }

        private void OnOpenRule(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.GetInstance().OpenInfoForm(15);
        }

        private void OnRankTabChanged(CUIEvent uiEvent)
        {
            CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
            if (srcFormScript != null)
            {
                CUIListScript component = srcFormScript.GetWidget(4).GetComponent<CUIListScript>();
                GameObject widget = srcFormScript.GetWidget(6);
                GameObject obj3 = srcFormScript.GetWidget(7);
                GameObject obj4 = srcFormScript.GetWidget(8);
                GameObject obj5 = srcFormScript.GetWidget(0x11);
                GameObject obj6 = srcFormScript.GetWidget(9);
                GameObject obj7 = srcFormScript.GetWidget(0x16);
                switch (component.GetSelectedIndex())
                {
                    case 0:
                        widget.CustomSetActive(true);
                        obj3.CustomSetActive(false);
                        obj4.CustomSetActive(false);
                        obj5.CustomSetActive(true);
                        obj6.CustomSetActive(false);
                        obj7.CustomSetActive(true);
                        this.RefreshGuildScoreRankList();
                        this.SetSelfGuildScorePanel(obj5);
                        this.RefreshSubRankPanel(enRankTab.GuildScore);
                        break;

                    case 1:
                        obj3.CustomSetActive(true);
                        widget.CustomSetActive(false);
                        obj4.CustomSetActive(false);
                        obj5.CustomSetActive(false);
                        obj6.CustomSetActive(true);
                        obj7.CustomSetActive(true);
                        this.RefreshMemberScoreRankList();
                        this.SetMemberScoreListElement(obj6, this.GetSelfIndexInGuildMemberScoreList());
                        this.RefreshSubRankPanel(enRankTab.MemberScore);
                        break;

                    case 2:
                        obj4.CustomSetActive(true);
                        widget.CustomSetActive(false);
                        obj3.CustomSetActive(false);
                        obj5.CustomSetActive(false);
                        obj6.CustomSetActive(false);
                        obj7.CustomSetActive(false);
                        break;
                }
            }
        }

        private void OnReadyGame(CUIEvent uiEvent)
        {
            if (CGuildHelper.IsGuildMatchReachMatchCntLimit(CGuildHelper.GetPlayerGuildMemberInfo().GuildMatchInfo.bWeekMatchCnt))
            {
                Singleton<CUIManager>.GetInstance().OpenTips(this.GetReachMatchCntLimitTip(), false, 1.5f, null, new object[0]);
            }
            else
            {
                if (!this.IsInGuildMatchTime())
                {
                    Singleton<CUIManager>.GetInstance().OpenTips("GuildMatch_Not_Open", true, 1.5f, null, new object[0]);
                }
                this.RequestSetGuildMatchReady(true);
            }
        }

        private void OnRecordListElementEnabled(CUIEvent uiEvent)
        {
            GameObject srcWidget = uiEvent.m_srcWidget;
            if (((srcWidget != null) && (this.m_matchRecords != null)) && ((uiEvent.m_srcWidgetIndexInBelongedList >= 0) && (uiEvent.m_srcWidgetIndexInBelongedList < this.m_matchRecords.Length)))
            {
                Transform transform = srcWidget.get_transform();
                COMDT_GUILD_MATCH_HISTORY_INFO comdt_guild_match_history_info = this.m_matchRecords[(this.m_matchRecords.Length - 1) - uiEvent.m_srcWidgetIndexInBelongedList];
                if (comdt_guild_match_history_info != null)
                {
                    transform.Find("txtMatchTime").GetComponent<Text>().set_text(Utility.GetUtcToLocalTimeStringFormat((ulong) comdt_guild_match_history_info.dwMatchTime, Singleton<CTextManager>.GetInstance().GetText("Common_DateTime_Format2")));
                    CUIListScript component = transform.Find("MatchMemberList").GetComponent<CUIListScript>();
                    component.SetElementAmount(5);
                    int num = 1;
                    for (int i = 0; i < comdt_guild_match_history_info.bMemNum; i++)
                    {
                        CUIListElementScript elemenet;
                        if (Convert.ToBoolean(comdt_guild_match_history_info.astMemInfo[i].bIsTeamLeader))
                        {
                            elemenet = component.GetElemenet(0);
                        }
                        else
                        {
                            elemenet = component.GetElemenet(num++);
                        }
                        this.SetMatchRecordMemberListElement(elemenet, comdt_guild_match_history_info.astMemInfo[i]);
                    }
                    transform.Find("txtMatchScore").GetComponent<Text>().set_text(comdt_guild_match_history_info.iScore.ToString());
                }
            }
        }

        private void OnRefreshGameStateTimeout(CUIEvent uiEvent)
        {
            this.RequestGetGuildMemberGameState();
        }

        private void OnRefuseInvite(CUIEvent uiEvent)
        {
            this.RequestDealGuildMatchMemberInvite(uiEvent.m_eventParams.commonUInt64Param1, false);
        }

        private void OnRemindButtonCdOver(CUIEvent uiEvent)
        {
            if (uiEvent.m_srcWidget != null)
            {
                Button component = uiEvent.m_srcWidget.GetComponent<Button>();
                if (component != null)
                {
                    CUICommonSystem.SetButtonEnable(component, true, true, true);
                }
            }
        }

        private void OnRemindReady(CUIEvent uiEvent)
        {
            this.RequestGuildMatchRemind(uiEvent.m_eventParams.commonUInt64Param1);
            GameObject srcWidget = uiEvent.m_srcWidget;
            CUICommonSystem.SetButtonEnable(srcWidget.GetComponent<Button>(), false, false, true);
            CUITimerScript component = srcWidget.GetComponent<CUITimerScript>();
            if (component == null)
            {
                component = srcWidget.AddComponent<CUITimerScript>();
            }
            component.SetTotalTime(10f);
            component.SetTimerEventId(enTimerEventType.TimeUp, enUIEventID.Guild_Match_RemindButtonCdOver);
            component.StartTimer();
        }

        private void OnRequestNewRecordTimeout(int timerSequence)
        {
            this.m_isNeedRequestNewRecord = true;
        }

        private void OnStartGame(CUIEvent uiEvent)
        {
            if (CGuildHelper.IsGuildMatchReachMatchCntLimit(CGuildHelper.GetPlayerGuildMemberInfo().GuildMatchInfo.bWeekMatchCnt))
            {
                Singleton<CUIManager>.GetInstance().OpenTips(this.GetReachMatchCntLimitTip(), false, 1.5f, null, new object[0]);
            }
            else
            {
                this.RequestStartGuildMatch();
            }
        }

        private void OnSubRankSliderValueChanged(CUIEvent uiEvent)
        {
            CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
            if (srcFormScript != null)
            {
                Text component = srcFormScript.GetWidget(0x18).GetComponent<Text>();
                int sliderValue = (int) uiEvent.m_eventParams.sliderValue;
                component.set_text(Singleton<CTextManager>.GetInstance().GetText((sliderValue != 0) ? "GuildMatch_Slider_Text_Week" : "GuildMatch_Slider_Text_Season"));
                CUIListScript script2 = srcFormScript.GetWidget(4).GetComponent<CUIListScript>();
                if (script2.GetSelectedIndex() == 0)
                {
                    this.RefreshGuildScoreRankList();
                    GameObject widget = srcFormScript.GetWidget(0x11);
                    this.SetSelfGuildScorePanel(widget);
                }
                else if (script2.GetSelectedIndex() == 1)
                {
                    this.RefreshMemberScoreRankList();
                    GameObject listElementGo = srcFormScript.GetWidget(9);
                    this.SetMemberScoreListElement(listElementGo, this.GetSelfIndexInGuildMemberScoreList());
                }
            }
        }

        private void OnTeamListElementEnabled(CUIEvent uiEvent)
        {
            if (this.m_teamInfos != null)
            {
                int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
                if ((srcWidgetIndexInBelongedList >= 0) && (srcWidgetIndexInBelongedList < this.m_teamInfos.Count))
                {
                    CUIListElementScript srcWidgetScript = uiEvent.m_srcWidgetScript as CUIListElementScript;
                    KeyValuePair<ulong, ListView<TeamPlayerInfo>> pair = this.m_teamInfos[srcWidgetIndexInBelongedList];
                    KeyValuePair<ulong, ListView<TeamPlayerInfo>> pair2 = this.m_teamInfos[srcWidgetIndexInBelongedList];
                    this.SetTeamListElement(srcWidgetScript, pair.Key, pair2.Value);
                }
            }
        }

        private void OpenMatchForm(bool isReadyGame = false)
        {
            this.m_form = Singleton<CUIManager>.GetInstance().OpenForm(GuildMatchFormPath, false, true);
            this.RefreshGuildMatchForm();
            this.RequestGetGuildMatchSeasonRank();
            this.RequestGetGuildMatchWeekRank();
            if (isReadyGame)
            {
                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Guild_Match_ReadyGame);
            }
        }

        private void OpenMatchRecordForm()
        {
            this.m_matchRecordForm = Singleton<CUIManager>.GetInstance().OpenForm(GuildMatchRecordFormPath, false, true);
            this.RefreshMatchRecordForm();
        }

        private void PromptKickedFromTeamTip(ulong memberUid)
        {
            if (CGuildHelper.IsSelf(memberUid) && Utility.IsCanShowPrompt())
            {
                Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("GuildMatch_Kicked_Tip"), true, 1.5f, null, new object[0]);
            }
        }

        private void RealInvite(ulong invitedMemberUid, GameObject inviteBtnGo)
        {
            this.RequestInviteGuildMatchMember(invitedMemberUid);
            CInviteView.SetInvitedRelatedWidgets(inviteBtnGo, inviteBtnGo.get_transform().get_parent().Find("Online").GetComponent<Text>());
        }

        [MessageHandler(0x14b6)]
        public static void ReceiveChangeGuildMatchLeaderNtf(CSPkg msg)
        {
            Singleton<CGuildModel>.GetInstance().SetGuildMatchMemberInfo(msg.stPkgData.stChgGuildMatchLeaderNtf);
            Singleton<CGuildMatchSystem>.GetInstance().SetTeamInfo(msg.stPkgData.stChgGuildMatchLeaderNtf);
            Singleton<CPlayerInfoSystem>.GetInstance().SetAppointMatchLeaderBtn();
            Singleton<CGuildInfoView>.GetInstance().RefreshMemberPanel();
            Singleton<CGuildMatchSystem>.GetInstance().RefreshTeamList();
            if (Singleton<CGuildMatchSystem>.GetInstance().IsNeedRefreshRankTab())
            {
                Singleton<CGuildMatchSystem>.GetInstance().InitRankTabList();
            }
            if (CGuildSystem.HasAppointMatchLeaderAuthority())
            {
                Singleton<CUIManager>.GetInstance().OpenTips("GuildMatch_Appoint_Or_Leader_Success", true, 1.5f, null, new object[0]);
            }
        }

        [MessageHandler(0x14c9)]
        public static void ReceiveChangeGuildMatchLeaderRsp(CSPkg msg)
        {
            if (CGuildSystem.IsError(msg.stPkgData.stChgGuildMatchLeaderRsp.bErrorCode))
            {
            }
        }

        [MessageHandler(0x14c8)]
        public static void ReceiveGetGuildMatchHistoryRsp(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            SCPKG_GET_GUILD_MATCH_HISTORY_RSP stGetGuildMatchHistoryRsp = msg.stPkgData.stGetGuildMatchHistoryRsp;
            Singleton<CGuildMatchSystem>.GetInstance().m_matchRecords = new COMDT_GUILD_MATCH_HISTORY_INFO[stGetGuildMatchHistoryRsp.bMatchNum];
            for (int i = 0; i < Singleton<CGuildMatchSystem>.GetInstance().m_matchRecords.Length; i++)
            {
                Singleton<CGuildMatchSystem>.GetInstance().m_matchRecords[i] = stGetGuildMatchHistoryRsp.astMatchInfo[i];
            }
            Singleton<CGuildMatchSystem>.GetInstance().OpenMatchRecordForm();
        }

        [MessageHandler(0x14bd)]
        public static void ReceiveGuildMatchMemberChangeNtf(CSPkg msg)
        {
            Singleton<CGuildModel>.GetInstance().SetGuildMatchMemberInfo(msg.stPkgData.stGuildMatchMemberChgNtf);
            Singleton<CGuildMatchSystem>.GetInstance().SetTeamInfo(msg.stPkgData.stGuildMatchMemberChgNtf);
            if (Singleton<CGuildMatchSystem>.GetInstance().IsNeedOpenGuildMatchForm(msg.stPkgData.stGuildMatchMemberChgNtf))
            {
                Singleton<CGuildMatchSystem>.GetInstance().OpenMatchForm(false);
            }
            else
            {
                Singleton<CGuildMatchSystem>.GetInstance().RefreshTeamList();
                if (Singleton<CGuildMatchSystem>.GetInstance().IsNeedRefreshRankTab())
                {
                    Singleton<CGuildMatchSystem>.GetInstance().InitRankTabList();
                }
            }
        }

        [MessageHandler(0x14ba)]
        public static void ReceiveGuildMatchMemberInviteRsp(CSPkg msg)
        {
            SCPKG_GUILD_MATCH_MEMBER_INVITE_RSP stGuildMatchMemberInviteRsp = msg.stPkgData.stGuildMatchMemberInviteRsp;
            if ((stGuildMatchMemberInviteRsp.bErrorCode == 30) && (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID == stGuildMatchMemberInviteRsp.ullInviter))
            {
                GuildMemInfo guildMemberInfoByUid = CGuildHelper.GetGuildMemberInfoByUid(stGuildMatchMemberInviteRsp.ullInvitee);
                string[] args = new string[] { guildMemberInfoByUid.stBriefInfo.sName };
                string text = Singleton<CTextManager>.GetInstance().GetText("GuildMatch_Invitee_Refuse", args);
                Singleton<CUIManager>.GetInstance().OpenTips(text, false, 1.5f, null, new object[0]);
            }
        }

        [MessageHandler(0x14c4)]
        public static void ReceiveGuildMatchObInfoChg(CSPkg msg)
        {
            GuildInfo currentGuildInfo = Singleton<CGuildModel>.GetInstance().CurrentGuildInfo;
            if ((currentGuildInfo != null) && (currentGuildInfo.GuildMatchObInfos != null))
            {
                COMDT_GUILD_MATCH_OB_INFO stChgInfo = msg.stPkgData.stGuildMatchOBInfoChg.stChgInfo;
                ulong ullUid = stChgInfo.ullUid;
                uint dwBeginTime = stChgInfo.dwBeginTime;
                bool flag = false;
                for (int i = currentGuildInfo.GuildMatchObInfos.Count - 1; i >= 0; i--)
                {
                    if (currentGuildInfo.GuildMatchObInfos[i].ullUid == ullUid)
                    {
                        if (dwBeginTime > 0)
                        {
                            currentGuildInfo.GuildMatchObInfos[i].dwBeginTime = dwBeginTime;
                            currentGuildInfo.GuildMatchObInfos[i].dwOBCnt = stChgInfo.dwOBCnt;
                            currentGuildInfo.GuildMatchObInfos[i].astHeroInfo = new COMDT_GUILD_MATCH_PLAYER_HERO[5];
                            for (int j = 0; j < 5; j++)
                            {
                                currentGuildInfo.GuildMatchObInfos[i].astHeroInfo[j] = new COMDT_GUILD_MATCH_PLAYER_HERO();
                                currentGuildInfo.GuildMatchObInfos[i].astHeroInfo[j].ullUid = stChgInfo.astHeroInfo[j].ullUid;
                                currentGuildInfo.GuildMatchObInfos[i].astHeroInfo[j].dwHeroID = stChgInfo.astHeroInfo[j].dwHeroID;
                            }
                        }
                        else
                        {
                            currentGuildInfo.GuildMatchObInfos.RemoveAt(i);
                        }
                        flag = true;
                    }
                }
                if (!flag)
                {
                    currentGuildInfo.GuildMatchObInfos.Add(stChgInfo);
                }
            }
        }

        [MessageHandler(0x14cb)]
        public static void ReceiveGuildMatchRemindNtf(CSPkg msg)
        {
            if (Utility.IsCanShowPrompt())
            {
                Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("Guild_Match_Remind_Msg"), enUIEventID.Guild_Match_OpenMatchFormAndReadyGame, enUIEventID.None, false);
            }
        }

        [MessageHandler(0x14c2)]
        public static void ReceiveGuildMatchScoreChangeNtf(CSPkg msg)
        {
            Singleton<CGuildModel>.GetInstance().SetGuildMatchScore(msg.stPkgData.stGuildMatchScoreChgNtf);
            Singleton<CGuildMatchSystem>.GetInstance().RefreshGuildMatchScore();
        }

        [MessageHandler(0x14cd)]
        public static void ReceiveGuildOBCountRsp(CSPkg msg)
        {
            Singleton<COBSystem>.GetInstance().SetGuildMatchOBCount(msg.stPkgData.stGetGuildMatchOBCntRsp);
        }

        [MessageHandler(0x14b8)]
        public static void ReceiveInviteGuildMatchMemberNtf(CSPkg msg)
        {
            SCPKG_INVITE_GUILD_MATCH_MEMBER_NTF stInviteGuildMatchMemberNtf = msg.stPkgData.stInviteGuildMatchMemberNtf;
            if (Singleton<CInviteSystem>.GetInstance().IsCanBeInvited(stInviteGuildMatchMemberNtf.ullTeamLeaderUid, (uint) CGuildHelper.GetGuildLogicWorldId()))
            {
                GuildMemInfo guildMemberInfoByUid = CGuildHelper.GetGuildMemberInfoByUid(stInviteGuildMatchMemberNtf.ullTeamLeaderUid);
                if (guildMemberInfoByUid != null)
                {
                    stUIEventParams par = new stUIEventParams();
                    par.commonUInt64Param1 = stInviteGuildMatchMemberNtf.ullTeamLeaderUid;
                    string[] args = new string[] { guildMemberInfoByUid.stBriefInfo.sName };
                    Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancelAndAutoClose(Singleton<CTextManager>.GetInstance().GetText("GuildMatch_Invited_Msg", args), enUIEventID.Guild_Match_Accept_Invite, enUIEventID.Guild_Match_Refuse_Invite, par, false, 10, enUIEventID.None);
                }
            }
        }

        [MessageHandler(0x14c6)]
        public static void ReceiveObGuildMatchRsp(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            if (msg.stPkgData.stOBGuildMatchRsp.iResult == 0)
            {
                if (Singleton<WatchController>.GetInstance().StartObserve(msg.stPkgData.stOBGuildMatchRsp.stTgwinfo))
                {
                    Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
                }
            }
            else
            {
                Singleton<CUIManager>.instance.OpenTips(string.Format("OB_Error_{0}", msg.stPkgData.stOBGuildMatchRsp.iResult), true, 1.5f, null, new object[0]);
            }
        }

        [MessageHandler(0x14c1)]
        public static void ReceiveSetGuildMatchReadyNtf(CSPkg msg)
        {
            Singleton<CGuildModel>.GetInstance().SetGuildMatchMemberReadyState(msg.stPkgData.stSetGuildMatchReadyNtf);
            Singleton<CGuildMatchSystem>.GetInstance().SetTeamMemberReadyState(msg.stPkgData.stSetGuildMatchReadyNtf);
            Singleton<CGuildMatchSystem>.GetInstance().RefreshTeamList();
        }

        [MessageHandler(0x14c0)]
        public static void ReceiveSetGuildMatchReadyRsp(CSPkg msg)
        {
            CGuildSystem.IsError(msg.stPkgData.stSetGuildMatchReadyRsp.bErrorCode);
        }

        [MessageHandler(0x14c3)]
        public static void ReceiveStartGuildMatchRsp(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            CGuildSystem.IsError(msg.stPkgData.stStartGuildMatchRsp.bErrorCode);
        }

        private void RefreshGuildHead()
        {
            if (this.m_form != null)
            {
                Image component = this.m_form.GetWidget(0).GetComponent<Image>();
                string prefabPath = CUIUtility.s_Sprite_Dynamic_GuildHead_Dir + Singleton<CGuildModel>.GetInstance().CurrentGuildInfo.briefInfo.dwHeadId;
                component.SetSprite(prefabPath, this.m_form, true, false, false, false);
            }
        }

        private void RefreshGuildMatchForm()
        {
            this.RefreshGuildHead();
            this.RefreshGuildName();
            this.RefreshGuildMatchScore();
            this.RefreshTeamList();
            this.RefreshGuildMatchLeftMatchCnt();
            this.RefreshGuildMatchOpenTime();
            this.InitRankTabList();
        }

        public void RefreshGuildMatchGuildMemberInvitePanel()
        {
            if (this.m_guildMemberInviteList != null)
            {
                this.m_guildMemberInviteList.Sort(new Comparison<GuildMemInfo>(CGuildHelper.GuildMemberComparisonForInvite));
                this.SetInviteGuildMemberData();
            }
        }

        private void RefreshGuildMatchLeftMatchCnt()
        {
            if (this.m_form != null)
            {
                GuildMemInfo playerGuildMemberInfo = CGuildHelper.GetPlayerGuildMemberInfo();
                if (playerGuildMemberInfo == null)
                {
                    DebugHelper.Assert(false, "selfMemInfo is null!!!");
                }
                int guildMatchLeftCntInCurRound = CGuildHelper.GetGuildMatchLeftCntInCurRound(playerGuildMemberInfo.GuildMatchInfo.bWeekMatchCnt);
                Text component = this.m_form.GetWidget(0x10).GetComponent<Text>();
                if (guildMatchLeftCntInCurRound <= 0)
                {
                    component.set_text(string.Concat(new object[] { "<color=red>", guildMatchLeftCntInCurRound, "</color>", Singleton<CTextManager>.GetInstance().GetText("Common_Times") }));
                }
                else
                {
                    component.set_text(string.Concat(new object[] { "<color=white>", guildMatchLeftCntInCurRound, "</color>", Singleton<CTextManager>.GetInstance().GetText("Common_Times") }));
                }
            }
        }

        private void RefreshGuildMatchOpenTime()
        {
            if (this.m_form != null)
            {
                ResRewardMatchTimeInfo info = null;
                uint dwConfValue = GameDataMgr.guildMiscDatabin.GetDataByKey((uint) 0x2f).dwConfValue;
                GameDataMgr.matchTimeInfoDict.TryGetValue(GameDataMgr.GetDoubleKey(6, dwConfValue), out info);
                this.m_form.GetWidget(14).GetComponent<Text>().set_text(info.szTimeTips);
            }
        }

        private void RefreshGuildMatchScore()
        {
            if (this.m_form != null)
            {
                this.m_form.GetWidget(2).GetComponent<Text>().set_text(Singleton<CGuildModel>.GetInstance().CurrentGuildInfo.GuildMatchInfo.dwScore.ToString());
            }
        }

        private void RefreshGuildName()
        {
            if (this.m_form != null)
            {
                this.m_form.GetWidget(1).GetComponent<Text>().set_text(CGuildHelper.GetGuildName());
            }
        }

        private void RefreshGuildScoreRankList()
        {
            if (this.m_form != null)
            {
                CUIListScript component = this.m_form.GetWidget(6).GetComponent<CUIListScript>();
                CSDT_RANKING_LIST_ITEM_INFO[] guildScoreRankInfo = this.GetGuildScoreRankInfo();
                int amount = (guildScoreRankInfo != null) ? guildScoreRankInfo.Length : 0;
                component.SetElementAmount(amount);
            }
        }

        private void RefreshGuildScoreRankListAndSelfGuildScorePanel()
        {
            if ((this.m_form != null) && (this.m_form.GetWidget(4).GetComponent<CUIListScript>().GetSelectedIndex() == 0))
            {
                this.RefreshGuildScoreRankList();
                GameObject widget = this.m_form.GetWidget(0x11);
                this.SetSelfGuildScorePanel(widget);
            }
        }

        public void RefreshInviteGuildMemberList(int allGuildMemberLen)
        {
            if (this.m_form != null)
            {
                this.m_form.GetWidget(8).GetComponent<CUIListScript>().SetElementAmount(allGuildMemberLen);
            }
        }

        private void RefreshMatchRecordForm()
        {
            if (((this.m_matchRecordForm != null) && (this.m_matchRecords != null)) && (this.m_matchRecords.Length != 0))
            {
                this.m_matchRecordForm.GetWidget(0).GetComponent<CUIListScript>().SetElementAmount(this.m_matchRecords.Length);
            }
        }

        private void RefreshMemberScoreRankList()
        {
            if (this.m_form != null)
            {
                if (this.m_guildMemberScoreList == null)
                {
                    this.m_guildMemberScoreList = new ListView<GuildMemInfo>();
                }
                this.m_guildMemberScoreList.Clear();
                ListView<GuildMemInfo> guildMemberInfos = CGuildHelper.GetGuildMemberInfos();
                this.m_guildMemberScoreList.AddRange(guildMemberInfos);
                if (this.IsCurrentSeasonScoreTab())
                {
                    this.m_guildMemberScoreList.Sort(new Comparison<GuildMemInfo>(this.SortGuildMemberSeasonScoreList));
                }
                else
                {
                    this.m_guildMemberScoreList.Sort(new Comparison<GuildMemInfo>(this.SortGuildMemberWeekScoreList));
                }
                this.m_form.GetWidget(7).GetComponent<CUIListScript>().SetElementAmount(this.m_guildMemberScoreList.Count);
            }
        }

        public void RefreshMoreLeaderPanel()
        {
            GameObject widget = this.m_form.GetWidget(0x1a);
            int curLeaderNum = this.GetCurLeaderNum();
            int totalLeaderNum = this.GetTotalLeaderNum();
            if (CGuildSystem.HasAppointMatchLeaderAuthority() && (curLeaderNum < totalLeaderNum))
            {
                widget.CustomSetActive(true);
                string[] args = new string[] { curLeaderNum.ToString(), totalLeaderNum.ToString() };
                widget.GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("Guild_Match_Appoint_More_Team_Leader_Tip", args));
            }
            else
            {
                widget.CustomSetActive(false);
            }
        }

        private void RefreshSubRankPanel(enRankTab rankTab)
        {
            if (this.m_form != null)
            {
                Text component = this.m_form.GetWidget(0x17).GetComponent<Text>();
                if (rankTab == enRankTab.GuildScore)
                {
                    component.set_text(Singleton<CTextManager>.GetInstance().GetText("GuildMatch_Guild_Score_Rank"));
                }
                else if (rankTab == enRankTab.MemberScore)
                {
                    component.set_text(Singleton<CTextManager>.GetInstance().GetText("GuildMatch_Member_Score_Rank"));
                }
            }
        }

        public void RefreshTeamList()
        {
            if ((this.m_form != null) && (this.m_teamInfos != null))
            {
                this.m_form.GetWidget(3).GetComponent<CUIListScript>().SetElementAmount(this.m_teamInfos.Count);
                this.RefreshMoreLeaderPanel();
            }
        }

        private void RemoveMemberFromOldTeam(ulong memberUid)
        {
            if (this.m_teamInfos != null)
            {
                for (int i = this.m_teamInfos.Count - 1; i >= 0; i--)
                {
                    KeyValuePair<ulong, ListView<TeamPlayerInfo>> pair2;
                    KeyValuePair<ulong, ListView<TeamPlayerInfo>> pair4;
                    KeyValuePair<ulong, ListView<TeamPlayerInfo>> pair = this.m_teamInfos[i];
                    if (this.IsTeamLeader(memberUid, pair.Key))
                    {
                        this.m_teamInfos.RemoveAt(i);
                        this.PromptKickedFromTeamTip(memberUid);
                        goto Label_00CF;
                    }
                    int num2 = 0;
                    goto Label_00AF;
                Label_005E:
                    pair2 = this.m_teamInfos[i];
                    if (memberUid == pair2.Value[num2].Uid)
                    {
                        KeyValuePair<ulong, ListView<TeamPlayerInfo>> pair3 = this.m_teamInfos[i];
                        this.RemoveMemberFromTeam(pair3.Value[num2]);
                        this.PromptKickedFromTeamTip(memberUid);
                    }
                    num2++;
                Label_00AF:
                    pair4 = this.m_teamInfos[i];
                    if (num2 < pair4.Value.Count)
                    {
                        goto Label_005E;
                    }
                Label_00CF:;
                }
            }
        }

        private void RemoveMemberFromTeam(TeamPlayerInfo teamPlayerInfo)
        {
            teamPlayerInfo.Uid = 0L;
            teamPlayerInfo.Name = string.Empty;
            teamPlayerInfo.HeadUrl = string.Empty;
            teamPlayerInfo.IsReady = false;
        }

        private void RequestChangeGuildMatchLeader(ulong leaderUid, bool isAppoint)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x14b5);
            msg.stPkgData.stChgGuildMatchLeaderReq.ullUid = leaderUid;
            msg.stPkgData.stChgGuildMatchLeaderReq.bAppoint = Convert.ToByte(isAppoint);
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        private void RequestDealGuildMatchMemberInvite(ulong teamLeaderUid, bool isAgree)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x14b9);
            msg.stPkgData.stDealGuildMatchMemberInvite.ullTeamLeaderUid = teamLeaderUid;
            msg.stPkgData.stDealGuildMatchMemberInvite.bAgree = Convert.ToByte(isAgree);
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        private void RequestGetGuildMatchHistory()
        {
            Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x14c7);
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        public void RequestGetGuildMatchSeasonRank()
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0xa2a);
            msg.stPkgData.stGetRankingListReq.iStart = 1;
            msg.stPkgData.stGetRankingListReq.bNumberType = 0x42;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        public void RequestGetGuildMatchWeekRank()
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0xa2a);
            msg.stPkgData.stGetRankingListReq.iStart = 1;
            msg.stPkgData.stGetRankingListReq.bNumberType = 0x43;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        private void RequestGetGuildMemberGameState()
        {
            if (this.m_guildMemberInviteList == null)
            {
                DebugHelper.Assert(false, "m_guildMemberInviteList is null!!!");
            }
            else
            {
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x7f3);
                CSPKG_GET_GUILD_MEMBER_GAME_STATE_REQ stGetGuildMemberGameStateReq = msg.stPkgData.stGetGuildMemberGameStateReq;
                int index = 0;
                for (int i = 0; i < this.m_guildMemberInviteList.Count; i++)
                {
                    if (CGuildHelper.IsMemberOnline(this.m_guildMemberInviteList[i]))
                    {
                        stGetGuildMemberGameStateReq.MemberUid[index] = this.m_guildMemberInviteList[i].stBriefInfo.uulUid;
                        index++;
                    }
                }
                stGetGuildMemberGameStateReq.iMemberCnt = index;
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
            }
        }

        public void RequestGuildMatchRemind(ulong remindUid)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x14ca);
            msg.stPkgData.stGuildMatchRemindReq.ullRemindUid = remindUid;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        public void RequestGuildOBCount()
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x14cc);
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        private void RequestInviteGuildMatchMember(ulong memberUid)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x14b7);
            msg.stPkgData.stInviteGuildMatchMemberReq.ullUid = memberUid;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        private void RequestKickGuildMatchMember(ulong memberUid)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x14bb);
            msg.stPkgData.stKickGuildMatchMemberReq.ullUid = memberUid;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        private void RequestLeaveGuildMatchTeam()
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x14bc);
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        public void RequestObGuildMatch(ulong obUid)
        {
            Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x14c5);
            msg.stPkgData.stOBGuildMatchReq.ullOBUid = obUid;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        private void RequestSetGuildMatchReady(bool isReady)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x14bf);
            msg.stPkgData.stSetGuildMatchReadyReq.bIsReady = Convert.ToByte(isReady);
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        private void RequestStartGuildMatch()
        {
            Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x14be);
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        private void SetGuildScoreListElement(GameObject listElementGo, int guildScoreListIndex)
        {
            if (listElementGo != null)
            {
                CSDT_RANKING_LIST_ITEM_INFO[] guildScoreRankInfo = this.GetGuildScoreRankInfo();
                if (guildScoreRankInfo != null)
                {
                    if ((guildScoreListIndex < 0) || (guildScoreListIndex >= guildScoreRankInfo.Length))
                    {
                        DebugHelper.Assert(false, "guildScoreListIndex out of range: " + guildScoreListIndex);
                    }
                    else
                    {
                        CSDT_RANKING_LIST_ITEM_INFO csdt_ranking_list_item_info = guildScoreRankInfo[guildScoreListIndex];
                        if (csdt_ranking_list_item_info != null)
                        {
                            Transform transform = listElementGo.get_transform();
                            Transform rankTransform = transform.Find("rank");
                            Image component = transform.Find("imgHeadBg/imgHead").GetComponent<Image>();
                            Text text = transform.Find("txtName").GetComponent<Text>();
                            Text text2 = transform.Find("txtScore").GetComponent<Text>();
                            CUICommonSystem.SetRankDisplay((uint) (guildScoreListIndex + 1), rankTransform);
                            component.SetSprite(CUIUtility.s_Sprite_Dynamic_GuildHead_Dir + csdt_ranking_list_item_info.stExtraInfo.stDetailInfo.stGuildMatch.dwGuildHeadID, this.m_form, true, false, false, false);
                            text.set_text(StringHelper.UTF8BytesToString(ref csdt_ranking_list_item_info.stExtraInfo.stDetailInfo.stGuildMatch.szGuildName));
                            text2.set_text(csdt_ranking_list_item_info.dwRankScore.ToString());
                        }
                    }
                }
            }
        }

        public void SetInviteGuildMemberData()
        {
            if (this.m_form != null)
            {
                ListView<GuildMemInfo> guildMemberInviteList = this.GetGuildMemberInviteList();
                if (guildMemberInviteList != null)
                {
                    int count = guildMemberInviteList.Count;
                    int num2 = 0;
                    this.RefreshInviteGuildMemberList(count);
                    for (int i = 0; i < count; i++)
                    {
                        if (CGuildHelper.IsMemberOnline(guildMemberInviteList[i]))
                        {
                            num2++;
                        }
                    }
                    string[] args = new string[] { num2.ToString(), count.ToString() };
                    this.m_form.GetWidget(5).GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("Common_Online_Member", args));
                }
            }
        }

        private void SetMatchRecordMemberListElement(CUIListElementScript memberListElement, COMDT_GUILD_MATCH_HISTORY_MEMBER_INFO recordMemberInfo)
        {
            if (memberListElement != null)
            {
                Transform transform = memberListElement.get_transform();
                transform.Find("imgHead").GetComponent<CUIHttpImageScript>().SetImageUrl(CGuildHelper.GetHeadUrl(StringHelper.UTF8BytesToString(ref recordMemberInfo.szHeadUrl)));
                transform.Find("txtName").GetComponent<Text>().set_text(StringHelper.UTF8BytesToString(ref recordMemberInfo.szName));
                transform.Find("imgLeaderMark").get_gameObject().CustomSetActive(Convert.ToBoolean(recordMemberInfo.bIsTeamLeader));
            }
        }

        private void SetMemberScoreListElement(GameObject listElementGo, int guildMemberScoreListIndex)
        {
            if ((listElementGo != null) && (this.m_guildMemberScoreList != null))
            {
                if ((guildMemberScoreListIndex < 0) || (guildMemberScoreListIndex >= this.m_guildMemberScoreList.Count))
                {
                    DebugHelper.Assert(false, "guildMemberScoreListIndex out of range: " + guildMemberScoreListIndex);
                }
                else
                {
                    GuildMemInfo info = this.m_guildMemberScoreList[guildMemberScoreListIndex];
                    if (info != null)
                    {
                        Transform transform = listElementGo.get_transform();
                        Transform rankTransform = transform.Find("rank");
                        CUIHttpImageScript component = transform.Find("imgHeadBg/imgHead").GetComponent<CUIHttpImageScript>();
                        Image image = transform.Find("NobeIcon").GetComponent<Image>();
                        Image image2 = transform.Find("imgHeadBg/NobeImag").GetComponent<Image>();
                        Text text = transform.Find("txtName").GetComponent<Text>();
                        Text text2 = transform.Find("txtScore").GetComponent<Text>();
                        CUICommonSystem.SetRankDisplay((uint) (guildMemberScoreListIndex + 1), rankTransform);
                        component.SetImageUrl(CGuildHelper.GetHeadUrl(info.stBriefInfo.szHeadUrl));
                        MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(image, CGuildHelper.GetNobeLevel(info.stBriefInfo.uulUid, info.stBriefInfo.stVip.level), false);
                        MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(image2, CGuildHelper.GetNobeHeadIconId(info.stBriefInfo.uulUid, info.stBriefInfo.stVip.headIconId));
                        text.set_text(info.stBriefInfo.sName);
                        text2.set_text(!this.IsCurrentSeasonScoreTab() ? info.GuildMatchInfo.dwWeekScore.ToString() : info.GuildMatchInfo.dwScore.ToString());
                    }
                }
            }
        }

        private void SetPlayerListElement(CUIListElementScript playerListElement, TeamPlayerInfo playerInfo, ulong teamLeaderUid)
        {
            if (playerListElement != null)
            {
                Transform transform = playerListElement.get_transform();
                GameObject obj2 = transform.Find("imgQuestion").get_gameObject();
                GameObject obj3 = transform.Find("btnKick").get_gameObject();
                GameObject obj4 = transform.Find("imgHead").get_gameObject();
                GameObject obj5 = transform.Find("txtPlayerName").get_gameObject();
                GameObject obj6 = transform.Find("imgLeader").get_gameObject();
                GameObject obj7 = transform.Find("imgReady").get_gameObject();
                if (this.IsSlotOccupied(playerInfo))
                {
                    obj2.CustomSetActive(false);
                    obj4.CustomSetActive(true);
                    obj5.CustomSetActive(true);
                    obj6.CustomSetActive(playerListElement.m_index == 0);
                    Text component = obj5.GetComponent<Text>();
                    component.set_text(playerInfo.Name);
                    if (CGuildHelper.IsSelf(playerInfo.Uid))
                    {
                        component.set_color(CUIUtility.s_Text_Color_Self);
                    }
                    else
                    {
                        component.set_color(CUIUtility.s_Text_Color_White);
                    }
                    obj4.GetComponent<CUIHttpImageScript>().SetImageUrl(CGuildHelper.GetHeadUrl(playerInfo.HeadUrl));
                    bool flag = playerInfo.Uid == Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID;
                    bool flag2 = this.IsTeamLeader(Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID, teamLeaderUid);
                    obj7.CustomSetActive((this.IsSameTeamWithSelf(teamLeaderUid) && !this.IsTeamLeader(playerInfo.Uid, teamLeaderUid)) && playerInfo.IsReady);
                    if ((flag || flag2) && (!this.IsTeamLeader(playerInfo.Uid, teamLeaderUid) && !playerInfo.IsReady))
                    {
                        obj3.CustomSetActive(true);
                        CUIEventScript script2 = obj3.GetComponent<CUIEventScript>();
                        script2.m_onClickEventParams.commonUInt64Param1 = playerInfo.Uid;
                        script2.m_onClickEventParams.commonBool = flag;
                    }
                    else
                    {
                        obj3.CustomSetActive(false);
                    }
                }
                else
                {
                    obj2.CustomSetActive(true);
                    obj3.CustomSetActive(false);
                    obj4.CustomSetActive(false);
                    obj5.CustomSetActive(false);
                    obj6.CustomSetActive(false);
                    obj7.CustomSetActive(false);
                }
            }
        }

        private void SetSelfGuildScorePanel(GameObject listElementGo)
        {
            if (listElementGo != null)
            {
                Transform transform = listElementGo.get_transform();
                Transform rankTransform = transform.Find("rank");
                Image component = transform.Find("imgHeadBg/imgHead").GetComponent<Image>();
                Text text = transform.Find("txtName").GetComponent<Text>();
                Text text2 = transform.Find("txtScore").GetComponent<Text>();
                CUICommonSystem.SetRankDisplay(this.GetSelfGuildScoreRankNo(), rankTransform);
                component.SetSprite(CGuildHelper.GetGuildHeadPath(), this.m_form, true, false, false, false);
                text.set_text(CGuildHelper.GetGuildName());
                text2.set_text(!this.IsCurrentSeasonScoreTab() ? CGuildHelper.GetGuildMatchWeekScore().ToString() : CGuildHelper.GetGuildMatchSeasonScore().ToString());
            }
        }

        public void SetTeamInfo(SCPKG_CHG_GUILD_MATCH_LEADER_NTF ntf)
        {
            if (this.m_teamInfos == null)
            {
                DebugHelper.Assert(false, "m_teamInfos is null!!!");
            }
            else if (ntf.ullTeamLeaderUid > 0L)
            {
                for (int i = 0; i < this.m_teamInfos.Count; i++)
                {
                    KeyValuePair<ulong, ListView<TeamPlayerInfo>> pair = this.m_teamInfos[i];
                    if (this.IsTeamLeader(ntf.ullTeamLeaderUid, pair.Key))
                    {
                        return;
                    }
                }
                GuildMemInfo guildMemberInfoByUid = CGuildHelper.GetGuildMemberInfoByUid(ntf.ullUid);
                this.CreateNewTeam(guildMemberInfoByUid);
            }
            else
            {
                for (int j = 0; j < this.m_teamInfos.Count; j++)
                {
                    KeyValuePair<ulong, ListView<TeamPlayerInfo>> pair2 = this.m_teamInfos[j];
                    if (this.IsTeamLeader(ntf.ullUid, pair2.Key))
                    {
                        this.m_teamInfos.RemoveAt(j);
                        break;
                    }
                }
            }
        }

        public void SetTeamInfo(SCPKG_GUILD_MATCH_MEMBER_CHG_NTF ntf)
        {
            if (this.m_teamInfos == null)
            {
                DebugHelper.Assert(false, "m_teamInfos is null!!!");
            }
            else
            {
                for (int i = 0; i < ntf.bCnt; i++)
                {
                    ulong ullUid = ntf.astChgInfo[i].ullUid;
                    ulong ullTeamLeaderUid = ntf.astChgInfo[i].ullTeamLeaderUid;
                    this.RemoveMemberFromOldTeam(ullUid);
                    if (ullUid == ullTeamLeaderUid)
                    {
                        this.CreateNewTeam(CGuildHelper.GetGuildMemberInfoByUid(ullUid));
                    }
                    else if (ullTeamLeaderUid > 0L)
                    {
                        this.AddMemberToTeam(ullTeamLeaderUid, CGuildHelper.GetGuildMemberInfoByUid(ullUid));
                    }
                }
            }
        }

        private void SetTeamListElement(CUIListElementScript teamListElement, ulong teamLeaderUid, ListView<TeamPlayerInfo> teamPlayerInfos)
        {
            if ((teamListElement != null) && (teamPlayerInfos != null))
            {
                Transform transform = teamListElement.get_transform();
                if (teamPlayerInfos[0] != null)
                {
                    string[] args = new string[] { teamPlayerInfos[0].Name };
                    transform.Find("imgTeamTitleBg/txtTeamName").GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("GuildMatch_Team_Name", args));
                }
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                GuildMemInfo playerGuildMemberInfo = CGuildHelper.GetPlayerGuildMemberInfo();
                Transform transform2 = transform.Find("imgTeamTitleBg/pnlStatusAndOperation");
                GameObject obj2 = transform2.Find("btnStartGame").get_gameObject();
                GameObject obj3 = transform2.Find("btnReadyGame").get_gameObject();
                GameObject obj4 = transform2.Find("btnCancelReadyGame").get_gameObject();
                GameObject obj5 = transform2.Find("btnOBGame").get_gameObject();
                GameObject obj6 = transform2.Find("TagTeamStatusBlue").get_gameObject();
                GameObject obj7 = transform2.Find("TagTeamStatusYellow").get_gameObject();
                GameObject obj8 = transform2.Find("txtTeamStatus").get_gameObject();
                obj8.CustomSetActive(true);
                Text component = obj8.GetComponent<Text>();
                obj6.CustomSetActive(false);
                obj7.CustomSetActive(true);
                if (this.IsInTeam(playerGuildMemberInfo.GuildMatchInfo.ullTeamLeaderUid, teamLeaderUid))
                {
                    if (this.IsTeamLeader(masterRoleInfo.playerUllUID, teamLeaderUid))
                    {
                        obj2.CustomSetActive(true);
                        obj3.CustomSetActive(false);
                        obj4.CustomSetActive(false);
                        component.set_text(Singleton<CTextManager>.GetInstance().GetText("GuildMatch_Status_Prepare"));
                        bool isEnable = this.IsTeamAllPlayerReadyForGame(teamPlayerInfos, teamLeaderUid);
                        CUICommonSystem.SetButtonEnable(obj2.GetComponent<Button>(), isEnable, isEnable, true);
                    }
                    else if (this.IsReadyForGame(teamPlayerInfos, masterRoleInfo.playerUllUID))
                    {
                        obj2.CustomSetActive(false);
                        obj3.CustomSetActive(false);
                        obj4.CustomSetActive(true);
                        component.set_text(Singleton<CTextManager>.GetInstance().GetText("GuildMatch_Status_In_Prepare"));
                    }
                    else
                    {
                        obj2.CustomSetActive(false);
                        obj3.CustomSetActive(true);
                        obj4.CustomSetActive(false);
                        component.set_text(Singleton<CTextManager>.GetInstance().GetText("GuildMatch_Status_Please_Prepare"));
                    }
                    obj5.CustomSetActive(false);
                }
                else
                {
                    obj2.CustomSetActive(false);
                    obj3.CustomSetActive(false);
                    obj4.CustomSetActive(false);
                    ulong obUid = this.GetObUid(teamLeaderUid);
                    obj5.CustomSetActive(obUid > 0L);
                    uint obWaitingTime = 0;
                    bool flag2 = this.IsInObDelayedTime(teamLeaderUid, out obWaitingTime);
                    if (flag2)
                    {
                        CUICommonSystem.SetButtonEnable(obj5.GetComponent<Button>(), false, false, true);
                        CUITimerScript script = obj5.get_transform().Find("obWaitingTimer").GetComponent<CUITimerScript>();
                        script.SetTotalTime((float) obWaitingTime);
                        script.StartTimer();
                    }
                    if (obUid > 0L)
                    {
                        obj5.GetComponent<CUIEventScript>().m_onClickEventParams.commonUInt64Param1 = obUid;
                    }
                    if (flag2)
                    {
                        component.set_text(string.Empty);
                    }
                    else
                    {
                        string matchStartedTimeStr = this.GetMatchStartedTimeStr(teamLeaderUid);
                        if (string.IsNullOrEmpty(matchStartedTimeStr))
                        {
                            matchStartedTimeStr = Singleton<CTextManager>.GetInstance().GetText("GuildMatch_Status_Prepare");
                        }
                        else
                        {
                            obj6.CustomSetActive(true);
                            obj7.CustomSetActive(false);
                        }
                        component.set_text(matchStartedTimeStr);
                    }
                }
                GuildMemInfo guildMemberInfoByUid = CGuildHelper.GetGuildMemberInfoByUid(teamLeaderUid);
                GameObject obj9 = transform.Find("imgContinuousWin").get_gameObject();
                if (guildMemberInfoByUid.GuildMatchInfo.bContinueWin > 0)
                {
                    obj9.CustomSetActive(true);
                    obj9.get_transform().Find("txtContinuousWin").GetComponent<Text>().set_text(guildMemberInfoByUid.GuildMatchInfo.bContinueWin + Singleton<CTextManager>.GetInstance().GetText("Common_Continues_Win"));
                }
                else
                {
                    obj9.CustomSetActive(false);
                }
                transform.Find("imgTeamScore/txtTeamScore").GetComponent<Text>().set_text(guildMemberInfoByUid.GuildMatchInfo.dwScore.ToString());
                CUIListScript script3 = transform.Find("PlayerList").GetComponent<CUIListScript>();
                script3.SetElementAmount(5);
                for (int i = 0; i < 5; i++)
                {
                    this.SetPlayerListElement(script3.GetElemenet(i), (i >= teamPlayerInfos.Count) ? null : teamPlayerInfos[i], teamLeaderUid);
                }
            }
        }

        public void SetTeamMemberReadyState(SCPKG_SET_GUILD_MATCH_READY_NTF ntf)
        {
            if (this.m_teamInfos == null)
            {
                DebugHelper.Assert(false, "m_teamInfos is null!!!");
            }
            else
            {
                bool flag = false;
                for (int i = 0; i < ntf.bCnt; i++)
                {
                    for (int j = 0; j < this.m_teamInfos.Count; j++)
                    {
                        KeyValuePair<ulong, ListView<TeamPlayerInfo>> pair;
                        KeyValuePair<ulong, ListView<TeamPlayerInfo>> pair3;
                        int num3 = 0;
                        goto Label_009D;
                    Label_002E:
                        pair = this.m_teamInfos[j];
                        if (pair.Value[num3].Uid == ntf.astInfo[i].ullUid)
                        {
                            KeyValuePair<ulong, ListView<TeamPlayerInfo>> pair2 = this.m_teamInfos[j];
                            pair2.Value[num3].IsReady = Convert.ToBoolean(ntf.astInfo[i].bIsReady);
                            flag = true;
                            goto Label_00BD;
                        }
                        num3++;
                    Label_009D:
                        pair3 = this.m_teamInfos[j];
                        if (num3 < pair3.Value.Count)
                        {
                            goto Label_002E;
                        }
                    Label_00BD:
                        if (flag)
                        {
                            break;
                        }
                    }
                }
            }
        }

        public void SetTeamMemberReadyState(ulong teamMemberUid, bool isReady)
        {
            if (this.m_teamInfos == null)
            {
                DebugHelper.Assert(false, "m_teamInfos is null!!!");
            }
            else
            {
                for (int i = 0; i < this.m_teamInfos.Count; i++)
                {
                    KeyValuePair<ulong, ListView<TeamPlayerInfo>> pair;
                    KeyValuePair<ulong, ListView<TeamPlayerInfo>> pair3;
                    int num2 = 0;
                    goto Label_006F;
                Label_0025:
                    pair = this.m_teamInfos[i];
                    if (pair.Value[num2].Uid == teamMemberUid)
                    {
                        KeyValuePair<ulong, ListView<TeamPlayerInfo>> pair2 = this.m_teamInfos[i];
                        pair2.Value[num2].IsReady = isReady;
                        return;
                    }
                    num2++;
                Label_006F:
                    pair3 = this.m_teamInfos[i];
                    if (num2 < pair3.Value.Count)
                    {
                        goto Label_0025;
                    }
                }
            }
        }

        private int SortGuildMemberSeasonScoreList(GuildMemInfo info1, GuildMemInfo info2)
        {
            return -info1.GuildMatchInfo.dwScore.CompareTo(info2.GuildMatchInfo.dwScore);
        }

        private int SortGuildMemberWeekScoreList(GuildMemInfo info1, GuildMemInfo info2)
        {
            return -info1.GuildMatchInfo.dwWeekScore.CompareTo(info2.GuildMatchInfo.dwScore);
        }

        public enum enGuildMatchFormWidget
        {
            GuildHead_Image,
            GuildName_Text,
            GuildMatchScore_Text,
            Team_List,
            RankTab_List,
            OnlineNum_Text,
            GuildScore_List,
            MemberScore_List,
            Invite_List,
            SelfMemberRank_Panel,
            SelfMemberRankRank_Panel,
            SelfMemberRankHead_Panel,
            SelfMemberRankName_Panel,
            SelfMemberRankScore_Panel,
            MatchOpenTime_Text,
            RefreshGameState_Timer,
            LeftMatchCnt_Text,
            SelfGuildRank_Panel,
            SelfGuildRankRank_Panel,
            SelfGuildRankHead_Panel,
            SelfGuildRankName_Panel,
            SelfGuildRankScore_Panel,
            RankSubTitle_Panel,
            RankSubTitle_Text,
            RankSubTitleSlider_Text,
            RankSubTitle_Slider,
            MoreLeader_Text
        }

        public enum enGuildMatchRecordFormWidget
        {
            Record_List
        }

        public enum enRankTab
        {
            GuildScore,
            MemberScore,
            MemberInvite,
            Count
        }

        public class TeamPlayerInfo
        {
            public string HeadUrl;
            public bool IsReady;
            public string Name;
            public ulong Uid;

            public TeamPlayerInfo(ulong uid, string name, string headUrl, bool isReady)
            {
                this.Uid = uid;
                this.Name = name;
                this.HeadUrl = headUrl;
                this.IsReady = isReady;
            }
        }
    }
}

