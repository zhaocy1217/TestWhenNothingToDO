namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [MessageHandlerClass]
    public class CGuildSystem : Singleton<CGuildSystem>
    {
        public const int GuildNameCharacterUpperLimit = 7;
        public const int GuildRankItemCountPerPage = 20;
        private CGuildInfoController m_infoController;
        private CGuildListController m_listController;
        private CGuildModel m_Model;
        public const int MaxGuildLevel = 0x13;
        public const int NumberNotInRank = 0;
        public const byte PREPAREGUILD_FRIEND_LIMIT = 10;
        public static uint[] s_coinProfitPercentage;
        public static bool s_isApplyAndRecommendListEmpty = true;
        public static bool s_isGuildHighestMatchScore;
        public static bool s_isGuildMaxGrade;
        public static uint s_lastByGameRankpoint;
        public static uint s_rankpointProfitMax = 200;
        public static uint s_showCoinProfitTipMaxLevel = 10;
        public const int SpecialGuildIconNum = 3;

        public static bool CanBeAppointedToViceChairman(COM_PLAYER_GUILD_STATE memberGuildState)
        {
            return ((memberGuildState == COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_ELDER) || (memberGuildState == COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_MEMBER));
        }

        public bool CanInvite(COMDT_FRIEND_INFO info)
        {
            return (((Utility.IsSameLogicWorldWithSelf((int) info.stUin.dwLogicWorldId) && this.IsInNormalGuild()) && (HasManageAuthority() && (info.bGuildState == 0))) && (info.dwPvpLvl >= CGuildHelper.GetGuildMemberMinPvpLevel()));
        }

        public bool CanRecommend(COMDT_FRIEND_INFO info)
        {
            return (((Utility.IsSameLogicWorldWithSelf((int) info.stUin.dwLogicWorldId) && this.IsInNormalGuild()) && (!HasManageAuthority() && (info.bGuildState == 0))) && (info.dwPvpLvl >= CGuildHelper.GetGuildMemberMinPvpLevel()));
        }

        public static bool CanRecommendSelfAsChairman()
        {
            return (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.guildState == COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_VICE_CHAIRMAN);
        }

        public void Clear()
        {
            if (this.m_Model != null)
            {
                for (int i = 0; i < 4; i++)
                {
                    this.m_Model.RankpointRankLastGottenTimes[i] = 0;
                    this.m_Model.RankpointRankGottens[i] = false;
                }
                this.m_Model.IsLocalDataInited = false;
                this.m_Model.ClearAppliedGuildDic();
                this.m_Model.ClearInvitedFriendDic();
            }
        }

        private static RankpointRankInfo CreateRankpointRankInfo(CSDT_RANKING_LIST_ITEM_INFO info, enGuildRankpointRankListType rankListType)
        {
            if (info.dwRankNo == 0)
            {
                return CGuildHelper.CreatePlayerGuildRankpointRankInfo(rankListType);
            }
            RankpointRankInfo info2 = new RankpointRankInfo();
            info2.guildId = ulong.Parse(StringHelper.UTF8BytesToString(ref info.szOpenID));
            info2.rankNo = info.dwRankNo;
            info2.rankScore = info.dwRankScore;
            info2.guildHeadId = info.stExtraInfo.stDetailInfo.stGuildRankPoint.dwGuildHeadID;
            info2.guildName = StringHelper.UTF8BytesToString(ref info.stExtraInfo.stDetailInfo.stGuildRankPoint.szGuildName);
            info2.guildLevel = info.stExtraInfo.stDetailInfo.stGuildRankPoint.bGuildLevel;
            info2.memberNum = info.stExtraInfo.stDetailInfo.stGuildRankPoint.bMemberNum;
            info2.star = info.stExtraInfo.stDetailInfo.stGuildRankPoint.dwStar;
            return info2;
        }

        private static stGuildBriefInfo GetLocalGuildBriefInfo(COMDT_GUILD_BRIEF_INFO protocolInfo)
        {
            stGuildBriefInfo info = new stGuildBriefInfo();
            info.uulUid = protocolInfo.ullGuildID;
            info.sName = StringHelper.UTF8BytesToString(ref protocolInfo.szName);
            info.bLevel = protocolInfo.bLevel;
            info.bMemberNum = protocolInfo.bMemberNum;
            info.dwHeadId = protocolInfo.dwHeadID;
            info.sBulletin = StringHelper.UTF8BytesToString(ref protocolInfo.szNotice);
            info.dwSettingMask = protocolInfo.dwSettingMask;
            info.LevelLimit = protocolInfo.bLevelLimit;
            info.GradeLimit = protocolInfo.bGradeLimit;
            return info;
        }

        public static bool HasAppointMatchLeaderAuthority()
        {
            return (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.guildState == COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_CHAIRMAN);
        }

        public static bool HasAppointViceChairmanAuthority()
        {
            return (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.guildState == COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_CHAIRMAN);
        }

        public static bool HasFireMemberAuthority(COM_PLAYER_GUILD_STATE memberGuildState)
        {
            COM_PLAYER_GUILD_STATE guildState = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.guildState;
            if (guildState == COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_CHAIRMAN)
            {
                if (((memberGuildState == COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_VICE_CHAIRMAN) || (memberGuildState == COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_ELDER)) || (memberGuildState == COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_MEMBER))
                {
                    return true;
                }
            }
            else if ((guildState == COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_VICE_CHAIRMAN) && ((memberGuildState == COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_ELDER) || (memberGuildState == COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_MEMBER)))
            {
                return true;
            }
            return false;
        }

        public static bool HasGuildNameChangeAuthority()
        {
            return (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.guildState == COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_CHAIRMAN);
        }

        public static bool HasGuildSettingAuthority()
        {
            COM_PLAYER_GUILD_STATE guildState = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.guildState;
            return ((guildState == COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_CHAIRMAN) || (guildState == COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_VICE_CHAIRMAN));
        }

        public bool HasInvited(ulong uid)
        {
            int inviteTimeInfoByUid = this.m_Model.GetInviteTimeInfoByUid(uid);
            if (inviteTimeInfoByUid == -1)
            {
                return false;
            }
            int dwConfValue = (int) GameDataMgr.guildMiscDatabin.GetDataByKey((uint) 12).dwConfValue;
            return (CRoleInfo.GetCurrentUTCTime() < (inviteTimeInfoByUid + dwConfValue));
        }

        public static bool HasManageAuthority()
        {
            COM_PLAYER_GUILD_STATE guildState = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.guildState;
            return ((guildState == COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_CHAIRMAN) || (guildState == COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_VICE_CHAIRMAN));
        }

        public static bool HasManageQQGroupAuthority()
        {
            return (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.guildState == COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_CHAIRMAN);
        }

        public bool HasRecommended(ulong uid)
        {
            int recommendTimeInfoByUid = this.m_Model.GetRecommendTimeInfoByUid(uid);
            if (recommendTimeInfoByUid == -1)
            {
                return false;
            }
            int dwConfValue = (int) GameDataMgr.guildMiscDatabin.GetDataByKey((uint) 13).dwConfValue;
            return (CRoleInfo.GetCurrentUTCTime() < (recommendTimeInfoByUid + dwConfValue));
        }

        public static bool HasTransferPositionAuthority()
        {
            return (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.guildState == COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_CHAIRMAN);
        }

        public static bool HasWirteGuildMailAuthority()
        {
            COM_PLAYER_GUILD_STATE guildState = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.guildState;
            return ((guildState == COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_CHAIRMAN) || (guildState == COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_VICE_CHAIRMAN));
        }

        public override void Init()
        {
            base.Init();
            this.m_Model = Singleton<CGuildModel>.GetInstance();
            this.m_listController = Singleton<CGuildListController>.GetInstance();
            this.m_infoController = Singleton<CGuildInfoController>.GetInstance();
            this.InitSomeDatabin();
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_HyperLink_Click, new CUIEventManager.OnUIEventHandler(this.On_OpenForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_OpenForm, new CUIEventManager.OnUIEventHandler(this.On_OpenForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_CloseForm, new CUIEventManager.OnUIEventHandler(this.On_CloseForm));
            Singleton<EventRouter>.GetInstance().AddEventHandler<SCPKG_GET_RANKING_LIST_RSP>("Guild_Get_Power_Ranking", new Action<SCPKG_GET_RANKING_LIST_RSP>(this.OnGetPowerRanking));
            Singleton<EventRouter>.GetInstance().AddEventHandler<SCPKG_GET_RANKING_LIST_RSP, enGuildRankpointRankListType>("Guild_Get_Rankpoint_Ranking", new Action<SCPKG_GET_RANKING_LIST_RSP, enGuildRankpointRankListType>(this, (IntPtr) this.OnGetRankpointRanking));
            Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.LOBBY_STATE_LEAVE, new Action(this, (IntPtr) this.OnLobbyStateLeave));
            Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.NAMECHANGE_PLAYER_NAME_CHANGE, new Action(this, (IntPtr) this.OnPlayerNameChange));
        }

        private void InitSomeDatabin()
        {
            s_showCoinProfitTipMaxLevel = GameDataMgr.guildMiscDatabin.GetDataByKey((uint) 0x23).dwConfValue;
            s_rankpointProfitMax = GameDataMgr.guildMiscDatabin.GetDataByKey((uint) 40).dwConfValue;
            int count = GameDataMgr.guildLevelDatabin.count;
            s_coinProfitPercentage = new uint[count];
            for (int i = 0; i < count; i++)
            {
                ResGuildLevel dataByKey = GameDataMgr.guildLevelDatabin.GetDataByKey((long) (i + 1));
                if (dataByKey != null)
                {
                    s_coinProfitPercentage[i] = dataByKey.dwGameGoldBuffRate / 100;
                }
                else
                {
                    s_coinProfitPercentage[i] = 0;
                }
            }
        }

        public static bool IsError(byte bResult)
        {
            switch (((COM_GUILD_ERRORCODE) bResult))
            {
                case COM_GUILD_ERRORCODE.COM_GUILD_SUCCESS:
                    return false;

                case COM_GUILD_ERRORCODE.COM_GUILD_ERR_NAME_DUP:
                    Singleton<CUIManager>.GetInstance().OpenTips("GUILD_ERR_NAME_DUP", true, 1.5f, null, new object[0]);
                    return true;

                case COM_GUILD_ERRORCODE.COM_GUILD_ERR_DB:
                    Singleton<CUIManager>.GetInstance().OpenTips("GUILD_ERR_DB", true, 1.5f, null, new object[0]);
                    return true;

                case COM_GUILD_ERRORCODE.COM_GUILD_ERR_CREATE_QUEUE_FULL:
                    Singleton<CUIManager>.GetInstance().OpenTips("GUILD_ERR_CREATE_QUEUE_FULL", true, 1.5f, null, new object[0]);
                    return true;

                case COM_GUILD_ERRORCODE.COM_GUILD_ERR_NO_PREPARE_GUILD:
                    Singleton<CUIManager>.GetInstance().OpenTips("GUILD_ERR_NO_PREPARE_GUILD", true, 1.5f, null, new object[0]);
                    return true;

                case COM_GUILD_ERRORCODE.COM_GUILD_ERR_MEMBER_FULL:
                    Singleton<CUIManager>.GetInstance().OpenTips("GUILD_ERR_MEMBER_FULL", true, 1.5f, null, new object[0]);
                    return true;

                case COM_GUILD_ERRORCODE.COM_GUILD_ERR_HAS_INVITED:
                    Singleton<CUIManager>.GetInstance().OpenTips("GUILD_ERR_HAS_INVITED", true, 1.5f, null, new object[0]);
                    return true;

                case COM_GUILD_ERRORCODE.COM_GUILD_ERR_HAS_RECOMMEND:
                    Singleton<CUIManager>.GetInstance().OpenTips("GUILD_ERR_HAS_RECOMMEND", true, 1.5f, null, new object[0]);
                    return true;

                case COM_GUILD_ERRORCODE.COM_GUILD_ERR_HAS_GUILD:
                    Singleton<CUIManager>.GetInstance().OpenTips("GUILD_ERR_HAS_GUILD", true, 1.5f, null, new object[0]);
                    return true;

                case COM_GUILD_ERRORCODE.COM_GUILD_ERR_GUILD_NOT_EXIST:
                    Singleton<CUIManager>.GetInstance().OpenTips("GUILD_ERR_GUILD_NOT_EXIST", true, 1.5f, null, new object[0]);
                    return true;

                case COM_GUILD_ERRORCODE.COM_GUILD_ERR_LEVEL_LIMIT:
                    Singleton<CUIManager>.GetInstance().OpenTips("GUILD_ERR_LEVEL_LIMIT", true, 1.5f, null, new object[0]);
                    return true;

                case COM_GUILD_ERRORCODE.COM_GUILD_ERR_INVITED_EXPIRE:
                    Singleton<CUIManager>.GetInstance().OpenTips("GUILD_ERR_INVITED_EXPIRE", true, 1.5f, null, new object[0]);
                    return true;

                case COM_GUILD_ERRORCODE.COM_GUILD_ERR_UPGRADE:
                    Singleton<CUIManager>.GetInstance().OpenTips("GUILD_ERR_UPGRADE", true, 1.5f, null, new object[0]);
                    return true;

                case COM_GUILD_ERRORCODE.COM_GUILD_ERR_FIRE_CNT_LIMIT:
                    Singleton<CUIManager>.GetInstance().OpenTips("GUILD_ERR_FIRE_CNT_LIMIT", true, 1.5f, null, new object[0]);
                    return true;

                case COM_GUILD_ERRORCODE.COM_GUILD_ERR_DEAL_SELF_RECOM_FAIL:
                    Singleton<CUIManager>.GetInstance().OpenTips("Guild_Err_Deal_Self_Recommend_Fail", true, 1.5f, null, new object[0]);
                    return true;

                case COM_GUILD_ERRORCODE.COM_GUILD_ERR_DONATE_CNT_LIMIT:
                    Singleton<CUIManager>.GetInstance().OpenTips("Guild_Err_Donate_Count_Limit", true, 1.5f, null, new object[0]);
                    return true;

                case COM_GUILD_ERRORCODE.COM_GUILD_ERR_DUPLICATE_GET_DIVIDEND:
                    Singleton<CUIManager>.GetInstance().OpenTips("Guild_Err_Duplicate_Has_Got_Dividend", true, 1.5f, null, new object[0]);
                    return true;

                case COM_GUILD_ERRORCODE.COM_GUILD_ERR_NAME_ILLEGAL:
                    Singleton<CUIManager>.GetInstance().OpenTips("Guild_Name_Contain_Invalid_Character", true, 1.5f, null, new object[0]);
                    return true;

                case COM_GUILD_ERRORCODE.COM_GUILD_ERR_NOTICE_ILLEGAL:
                    Singleton<CUIManager>.GetInstance().OpenTips("Guild_Bulletin_Contain_Invalid_Character", true, 1.5f, null, new object[0]);
                    return true;

                case COM_GUILD_ERRORCODE.COM_GUILD_ERR_FACTORY_LEVEL_LIMIT:
                    Singleton<CUIManager>.GetInstance().OpenTips("Guild_Symbol_Err_Factory_Level_Limit", true, 1.5f, null, new object[0]);
                    return true;

                case COM_GUILD_ERRORCODE.COM_GUILD_ERR_CONSTRUCT_NOT_ENOUGH:
                    Singleton<CUIManager>.GetInstance().OpenTips("Guild_Symbol_Err_Construct_Not_Enough", true, 1.5f, null, new object[0]);
                    return true;

                case COM_GUILD_ERRORCODE.COM_GUILD_ERR_HIS_CONSTRUCT_NOT_ENOUGH:
                    Singleton<CUIManager>.GetInstance().OpenTips("Guild_Symbol_Err_History_Construct_Not_Enough", true, 1.5f, null, new object[0]);
                    return true;

                case COM_GUILD_ERRORCODE.COM_GUILD_ERR_UPGRADE_GUILD_FAIL:
                    Singleton<CUIManager>.GetInstance().OpenTips("GUILD_ERR_UPGRADE_GUILD_FAIL", true, 1.5f, null, new object[0]);
                    return true;

                case COM_GUILD_ERRORCODE.COM_GUILD_ERR_HAS_SIGNIN:
                    Singleton<CUIManager>.GetInstance().OpenTips("Guild_Signed", true, 1.5f, null, new object[0]);
                    return true;

                case COM_GUILD_ERRORCODE.COM_GUILD_ERR_NOT_SAME_LOGICWORLD:
                    Singleton<CUIManager>.GetInstance().OpenTips("GUILD_ERR_NOT_SAME_LOGICWORLD", true, 1.5f, null, new object[0]);
                    return true;

                case COM_GUILD_ERRORCODE.COM_GUILD_ERR_MATCH_OTHER:
                    Singleton<CUIManager>.GetInstance().OpenTips("GUILD_ERR_MATCH_OTHER", true, 1.5f, null, new object[0]);
                    return true;

                case COM_GUILD_ERRORCODE.COM_GUILD_ERR_MATCH_MEMBER_CNT:
                    Singleton<CUIManager>.GetInstance().OpenTips("GUILD_ERR_MATCH_MEMBER_CNT", true, 1.5f, null, new object[0]);
                    return true;

                case COM_GUILD_ERRORCODE.COM_GUILD_ERR_MATCH_MEMBER_NOT_READY:
                    Singleton<CUIManager>.GetInstance().OpenTips("GUILD_ERR_MATCH_MEMBER_NOT_READY", true, 1.5f, null, new object[0]);
                    return true;

                case COM_GUILD_ERRORCODE.COM_GUILD_ERR_MATCH_MEMBER_VERSION:
                    Singleton<CUIManager>.GetInstance().OpenTips("GUILD_ERR_MATCH_MEMBER_VERSION", true, 1.5f, null, new object[0]);
                    return true;

                case COM_GUILD_ERRORCODE.COM_GUILD_ERR_MATCH_CNT_LIMIT:
                    Singleton<CUIManager>.GetInstance().OpenTips("GUILD_ERR_MATCH_CNT_LIMIT", true, 1.5f, null, new object[0]);
                    return true;

                case COM_GUILD_ERRORCODE.COM_GUILD_ERR_MATCH_TEAM_FULL:
                    Singleton<CUIManager>.GetInstance().OpenTips("GUILD_ERR_MATCH_TEAM_FULL", true, 1.5f, null, new object[0]);
                    return true;

                case COM_GUILD_ERRORCODE.COM_GUILD_ERR_GRADE_LIMIT:
                    Singleton<CUIManager>.GetInstance().OpenTips("GUILD_ERR_MATCH_GRADE_LIMIT", true, 1.5f, null, new object[0]);
                    return true;

                case COM_GUILD_ERRORCODE.COM_GUILD_ERR_GUILD_MATCHING:
                    Singleton<CUIManager>.GetInstance().OpenTips("GUILD_ERR_GUILD_MATCHING", true, 1.5f, null, new object[0]);
                    return true;

                case COM_GUILD_ERRORCODE.COM_GUILD_ERR_JOIN_TIME_LIMIT:
                    Singleton<CUIManager>.GetInstance().OpenTips("GUILD_ERR_JOIN_TIME_LIMIT", true, 1.5f, null, new object[0]);
                    return true;

                case COM_GUILD_ERRORCODE.COM_GUILD_ERR_TEAM_LEADER_FULL:
                    Singleton<CUIManager>.GetInstance().OpenTips("GUILD_ERR_TEAM_LEADER_FULL", true, 1.5f, null, new object[0]);
                    return true;

                case COM_GUILD_ERRORCODE.COM_GUILD_ERR_SEND_GUILD_RECRUIT_CD:
                    Singleton<CUIManager>.GetInstance().OpenTips("GUILD_ERR_SEND_GUILD_RECRUIT_CD", true, 1.5f, null, new object[0]);
                    return true;

                case COM_GUILD_ERRORCODE.COM_GUILD_ERR_APPLY_EXIST:
                    Singleton<CUIManager>.GetInstance().OpenTips("GUILD_ERR_APPLY_EXIST", true, 1.5f, null, new object[0]);
                    return true;
            }
            Singleton<CUIManager>.GetInstance().OpenTips("GUILD_ERR_UNKNOWN", true, 1.5f, null, new object[0]);
            return true;
        }

        public bool IsInNormalGuild()
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            return (((masterRoleInfo != null) && (masterRoleInfo.m_baseGuildInfo != null)) && IsInNormalGuild(masterRoleInfo.m_baseGuildInfo.guildState));
        }

        public static bool IsInNormalGuild(COM_PLAYER_GUILD_STATE playerGuildState)
        {
            return ((((playerGuildState == COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_CHAIRMAN) || (playerGuildState == COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_VICE_CHAIRMAN)) || (playerGuildState == COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_ELDER)) || (playerGuildState == COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_MEMBER));
        }

        public bool IsInPrepareGuild()
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if ((masterRoleInfo == null) || (masterRoleInfo.m_baseGuildInfo == null))
            {
                return false;
            }
            return ((masterRoleInfo.m_baseGuildInfo.guildState == COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_PREPARE_CREATE) || (masterRoleInfo.m_baseGuildInfo.guildState == COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_PREPARE_JOIN));
        }

        private void On_CloseForm(CUIEvent uiEvent)
        {
        }

        private void On_OpenForm(CUIEvent uiEvent)
        {
            if (GameDataMgr.guildMiscDatabin.GetDataByKey((uint) 0x17).dwConfValue == 0)
            {
                Singleton<CUIManager>.GetInstance().OpenTips("Common_System_Not_Open_Tip", true, 1.5f, null, new object[0]);
            }
            else if (Singleton<CFunctionUnlockSys>.instance.FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_UNION))
            {
                if (this.IsInNormalGuild())
                {
                    this.m_infoController.OpenForm();
                }
                else
                {
                    this.m_listController.OpenForm();
                }
            }
            else
            {
                ResSpecialFucUnlock dataByKey = GameDataMgr.specialFunUnlockDatabin.GetDataByKey((uint) 13);
                Singleton<CUIManager>.instance.OpenTips(Utility.UTF8Convert(dataByKey.szLockedTip), false, 1.5f, null, new object[0]);
            }
        }

        public void OnGetPowerRanking(SCPKG_GET_RANKING_LIST_RSP rsp)
        {
            ListView<GuildInfo> view = new ListView<GuildInfo>();
            for (byte i = 0; i < rsp.stRankingListDetail.stOfSucc.dwItemNum; i = (byte) (i + 1))
            {
                GuildInfo item = new GuildInfo();
                CSDT_RANKING_LIST_ITEM_INFO csdt_ranking_list_item_info = rsp.stRankingListDetail.stOfSucc.astItemDetail[i];
                item.briefInfo.uulUid = ulong.Parse(StringHelper.UTF8BytesToString(ref csdt_ranking_list_item_info.szOpenID));
                item.briefInfo.Rank = csdt_ranking_list_item_info.dwRankNo;
                item.briefInfo.Ability = csdt_ranking_list_item_info.dwRankScore;
                item.briefInfo.dwHeadId = csdt_ranking_list_item_info.stExtraInfo.stDetailInfo.stGuildPower.dwGuildHeadID;
                item.briefInfo.sName = StringHelper.UTF8BytesToString(ref csdt_ranking_list_item_info.stExtraInfo.stDetailInfo.stGuildPower.szGuildName);
                item.briefInfo.bLevel = csdt_ranking_list_item_info.stExtraInfo.stDetailInfo.stGuildPower.bGuildLevel;
                item.briefInfo.bMemberNum = csdt_ranking_list_item_info.stExtraInfo.stDetailInfo.stGuildPower.bMemberNum;
                item.briefInfo.dwSettingMask = csdt_ranking_list_item_info.stExtraInfo.stDetailInfo.stGuildPower.dwSettingMask;
                item.briefInfo.LevelLimit = csdt_ranking_list_item_info.stExtraInfo.stDetailInfo.stGuildPower.bLimitLevel;
                item.briefInfo.GradeLimit = csdt_ranking_list_item_info.stExtraInfo.stDetailInfo.stGuildPower.bLimitGrade;
                item.chairman.stBriefInfo.szHeadUrl = StringHelper.UTF8BytesToString(ref csdt_ranking_list_item_info.stExtraInfo.stDetailInfo.stGuildPower.szChairManHeadUrl);
                item.chairman.stBriefInfo.sName = StringHelper.UTF8BytesToString(ref csdt_ranking_list_item_info.stExtraInfo.stDetailInfo.stGuildPower.szChairManName);
                item.chairman.stBriefInfo.dwLevel = csdt_ranking_list_item_info.stExtraInfo.stDetailInfo.stGuildPower.dwChairManLv;
                item.chairman.stBriefInfo.stVip.score = csdt_ranking_list_item_info.stExtraInfo.stDetailInfo.stGuildPower.stChairManVip.dwScore;
                item.chairman.stBriefInfo.stVip.level = csdt_ranking_list_item_info.stExtraInfo.stDetailInfo.stGuildPower.stChairManVip.dwCurLevel;
                item.chairman.stBriefInfo.stVip.headIconId = csdt_ranking_list_item_info.stExtraInfo.stDetailInfo.stGuildPower.stChairManVip.dwHeadIconId;
                item.briefInfo.sBulletin = StringHelper.UTF8BytesToString(ref csdt_ranking_list_item_info.stExtraInfo.stDetailInfo.stGuildPower.szGuildNotice);
                item.star = csdt_ranking_list_item_info.stExtraInfo.stDetailInfo.stGuildPower.dwStar;
                item.RankInfo.totalRankPoint = csdt_ranking_list_item_info.stExtraInfo.stDetailInfo.stGuildPower.dwTotalRankPoint;
                view.Add(item);
            }
            bool flag = CGuildHelper.IsFirstGuildListPage(rsp);
            Singleton<EventRouter>.GetInstance().BroadCastEvent<ListView<GuildInfo>, bool>("Receive_Guild_List_Success", view, flag);
        }

        public void OnGetRankpointRanking(SCPKG_GET_RANKING_LIST_RSP rsp, enGuildRankpointRankListType rankListType)
        {
            CSDT_RANKING_LIST_SUCC stOfSucc = rsp.stRankingListDetail.stOfSucc;
            if ((rankListType == enGuildRankpointRankListType.CurrentWeek) || (rankListType == enGuildRankpointRankListType.LastWeek))
            {
                rankListType = (stOfSucc.bImage != 0) ? enGuildRankpointRankListType.LastWeek : enGuildRankpointRankListType.CurrentWeek;
            }
            RefreshRankpointRankInfoList(rankListType, stOfSucc.dwItemNum, stOfSucc.astItemDetail);
            if (Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Guild/Form_Guild_RankpointRank.prefab") != null)
            {
                if ((rankListType == enGuildRankpointRankListType.CurrentWeek) || (rankListType == enGuildRankpointRankListType.LastWeek))
                {
                    Singleton<CGuildInfoView>.GetInstance().RefreshRankpointRankList(null);
                }
                else
                {
                    Singleton<CGuildInfoView>.GetInstance().RefreshRankpointSeasonRankList(null);
                }
            }
        }

        public void OnLobbyStateLeave()
        {
            s_isGuildMaxGrade = CGuildHelper.IsGuildMaxGrade();
            s_isGuildHighestMatchScore = CGuildHelper.IsGuildHighestMatchScore();
        }

        private void OnPlayerNameChange()
        {
            GuildInfo currentGuildInfo = this.m_Model.CurrentGuildInfo;
            GuildMemInfo playerGuildMemberInfo = CGuildHelper.GetPlayerGuildMemberInfo();
            if ((currentGuildInfo != null) && (playerGuildMemberInfo != null))
            {
                string name = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().Name;
                if (currentGuildInfo.chairman.stBriefInfo.uulUid == playerGuildMemberInfo.stBriefInfo.uulUid)
                {
                    currentGuildInfo.chairman.stBriefInfo.sName = name;
                }
                playerGuildMemberInfo.stBriefInfo.sName = name;
            }
        }

        [MessageHandler(0x8af)]
        public static void ReceiveApplyJoinGuildNtf(CSPkg msg)
        {
            int currentUTCTime = CRoleInfo.GetCurrentUTCTime();
            COMDT_GUILD_MEMBER_BRIEF_INFO stApplyInfo = msg.stPkgData.stJoinGuildApplyNtf.stApplyInfo;
            stApplicantInfo applicant = new stApplicantInfo();
            applicant.stBriefInfo.uulUid = stApplyInfo.ullUid;
            applicant.stBriefInfo.dwGameEntity = stApplyInfo.dwGameEntity;
            applicant.stBriefInfo.szHeadUrl = StringHelper.UTF8BytesToString(ref stApplyInfo.szHeadUrl);
            applicant.stBriefInfo.dwLevel = stApplyInfo.dwLevel;
            applicant.stBriefInfo.dwAbility = stApplyInfo.dwAbility;
            applicant.stBriefInfo.dwLogicWorldId = stApplyInfo.iLogicWorldID;
            applicant.stBriefInfo.sName = StringHelper.UTF8BytesToString(ref stApplyInfo.szName);
            applicant.stBriefInfo.stVip.score = stApplyInfo.stVip.dwScore;
            applicant.stBriefInfo.stVip.level = stApplyInfo.stVip.dwCurLevel;
            applicant.stBriefInfo.stVip.headIconId = stApplyInfo.stVip.dwHeadIconId;
            applicant.stBriefInfo.dwScoreOfRank = stApplyInfo.dwScoreOfRank;
            applicant.stBriefInfo.dwClassOfRank = stApplyInfo.dwClassOfRank;
            applicant.dwApplyTime = currentUTCTime;
            Singleton<CGuildSystem>.GetInstance().Model.AddApplicant(applicant);
            Singleton<EventRouter>.GetInstance().BroadCastEvent("Guild_New_Applicant");
            s_isApplyAndRecommendListEmpty = false;
        }

        [MessageHandler(0x8ae)]
        public static void ReceiveApplyJoinGuildRsp(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            stAppliedGuildInfo info = new stAppliedGuildInfo();
            COMDT_GUILD_BRIEF_INFO stGuildBriefInfo = msg.stPkgData.stApplyJoinGuildRsp.stGuildBriefInfo;
            if (IsError(msg.stPkgData.stApplyJoinGuildRsp.bResult))
            {
                Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.guildState = Singleton<CGuildSystem>.GetInstance().Model.m_PlayerGuildLastState;
                if ((msg.stPkgData.stApplyJoinGuildRsp.bResult == 2) && (msg.stPkgData.stApplyJoinGuildRsp.dwApplyTime != 0))
                {
                    info.stBriefInfo = GetLocalGuildBriefInfo(stGuildBriefInfo);
                    info.dwApplyTime = msg.stPkgData.stApplyJoinGuildRsp.dwApplyTime;
                    Singleton<CGuildSystem>.GetInstance().Model.AddAppliedGuildInfo(info, true);
                }
                Singleton<EventRouter>.GetInstance().BroadCastEvent<stAppliedGuildInfo>("Receive_Apply_Guild_Join_Failed", info);
            }
            else
            {
                info.stBriefInfo = GetLocalGuildBriefInfo(stGuildBriefInfo);
                info.dwApplyTime = msg.stPkgData.stApplyJoinGuildRsp.dwApplyTime;
                Singleton<CGuildSystem>.GetInstance().Model.AddAppliedGuildInfo(info, true);
                Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_extGuildInfo.bApplyJoinGuildNum = (byte) (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_extGuildInfo.bApplyJoinGuildNum + 1);
                Singleton<CGuildSystem>.GetInstance().Model.m_PlayerGuildLastState = COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_NULL;
                Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.guildState = COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_NULL;
                Singleton<EventRouter>.GetInstance().BroadCastEvent<stAppliedGuildInfo>("Receive_Apply_Guild_Join_Success", info);
            }
        }

        [MessageHandler(0x8ac)]
        public static void ReceiveApplyListRsp(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            SCPKG_GET_GUILD_APPLY_LIST_RSP stGetGuildApplyListRsp = msg.stPkgData.stGetGuildApplyListRsp;
            if (!Singleton<CGuildInfoView>.GetInstance().IsShow())
            {
                if (stGetGuildApplyListRsp.bApplyCnt > 0)
                {
                    s_isApplyAndRecommendListEmpty = false;
                }
            }
            else
            {
                int currentUTCTime = CRoleInfo.GetCurrentUTCTime();
                stApplicantInfo item = new stApplicantInfo();
                List<stApplicantInfo> applicantList = new List<stApplicantInfo>();
                for (byte i = 0; i < stGetGuildApplyListRsp.bApplyCnt; i = (byte) (i + 1))
                {
                    COMDT_GUILD_MEMBER_BRIEF_INFO comdt_guild_member_brief_info = stGetGuildApplyListRsp.astApplyInfo[i];
                    item.stBriefInfo.uulUid = comdt_guild_member_brief_info.ullUid;
                    item.stBriefInfo.dwLogicWorldId = comdt_guild_member_brief_info.iLogicWorldID;
                    item.stBriefInfo.sName = StringHelper.UTF8BytesToString(ref comdt_guild_member_brief_info.szName);
                    item.stBriefInfo.dwLevel = comdt_guild_member_brief_info.dwLevel;
                    item.stBriefInfo.dwAbility = comdt_guild_member_brief_info.dwAbility;
                    item.stBriefInfo.dwGameEntity = comdt_guild_member_brief_info.dwGameEntity;
                    item.stBriefInfo.szHeadUrl = StringHelper.UTF8BytesToString(ref comdt_guild_member_brief_info.szHeadUrl);
                    item.stBriefInfo.stVip.score = comdt_guild_member_brief_info.stVip.dwScore;
                    item.stBriefInfo.stVip.level = comdt_guild_member_brief_info.stVip.dwCurLevel;
                    item.stBriefInfo.stVip.headIconId = comdt_guild_member_brief_info.stVip.dwHeadIconId;
                    item.stBriefInfo.dwScoreOfRank = comdt_guild_member_brief_info.dwScoreOfRank;
                    item.stBriefInfo.dwClassOfRank = comdt_guild_member_brief_info.dwClassOfRank;
                    item.stBriefInfo.gender = comdt_guild_member_brief_info.bGender;
                    item.dwApplyTime = currentUTCTime;
                    applicantList.Add(item);
                }
                Singleton<CGuildInfoController>.GetInstance().OnReceiveApplyListSuccess(applicantList);
            }
        }

        [MessageHandler(0x8c5)]
        public static void ReceiveBuildingLevelChangeNtf(CSPkg msg)
        {
            SCPKG_GUILD_BUILDING_LEVEL_CHANGE_NTF stGuildBuildingLvChgNtf = msg.stPkgData.stGuildBuildingLvChgNtf;
            GuildBuildingInfo buildingInfo = new GuildBuildingInfo();
            buildingInfo.type = (RES_GUILD_BUILDING_TYPE) stGuildBuildingLvChgNtf.bBuildingType;
            buildingInfo.level = stGuildBuildingLvChgNtf.bCurLevel;
            Singleton<CGuildSystem>.GetInstance().Model.SetBuildingInfoList(buildingInfo);
            if (buildingInfo.type == RES_GUILD_BUILDING_TYPE.RES_GUILD_BUILDING_TYPE_HALL)
            {
                Singleton<CGuildSystem>.GetInstance().Model.CurrentGuildInfo.briefInfo.bLevel = buildingInfo.level;
                Singleton<CGuildInfoView>.GetInstance().RefreshInfoPanelGuildMemberCount();
                Singleton<CGuildInfoView>.GetInstance().RefreshProfitPanel();
            }
        }

        [MessageHandler(0x8c4)]
        public static void ReceiveBuildingUpgradeRsp(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            SCPKG_GUILD_BUILDING_UPGRADE_RSP stGuildBuildingUpgradeRsp = msg.stPkgData.stGuildBuildingUpgradeRsp;
            if (!IsError(stGuildBuildingUpgradeRsp.bResult))
            {
                string[] args = new string[] { CGuildHelper.GetBuildingName(stGuildBuildingUpgradeRsp.bBuildingType), stGuildBuildingUpgradeRsp.bCurLevel.ToString(), stGuildBuildingUpgradeRsp.dwCostMoney.ToString() };
                string text = Singleton<CTextManager>.GetInstance().GetText("Guild_Guild_Building_Upgrade_Success_Tip", args);
                Singleton<CUIManager>.GetInstance().OpenTips(text, false, 1.5f, null, new object[0]);
            }
        }

        [MessageHandler(0x8e1)]
        public static void ReceiveChgGuildHeadIdRsp(CSPkg msg)
        {
            SCPKG_CHG_GUILD_HEADID_RSP stChgGuildHeadIDRsp = msg.stPkgData.stChgGuildHeadIDRsp;
            if (!IsError(stChgGuildHeadIDRsp.bResult))
            {
                Singleton<CGuildSystem>.GetInstance().Model.CurrentGuildInfo.briefInfo.dwHeadId = stChgGuildHeadIDRsp.dwHeadID;
                Singleton<EventRouter>.GetInstance().BroadCastEvent("Guild_Setting_Modify_Icon_Success");
            }
        }

        [MessageHandler(0x8e3)]
        public static void ReceiveChgGuildNoticeRsp(CSPkg msg)
        {
            SCPKG_CHG_GUILD_NOTICE_RSP stChgGuildNoticeRsp = msg.stPkgData.stChgGuildNoticeRsp;
            if (!IsError(stChgGuildNoticeRsp.bResult))
            {
                Singleton<CGuildSystem>.GetInstance().Model.CurrentGuildInfo.briefInfo.sBulletin = StringHelper.UTF8BytesToString(ref stChgGuildNoticeRsp.szNotice);
                Singleton<EventRouter>.GetInstance().BroadCastEvent("Guild_Modify_Bulletin_Success");
            }
        }

        [MessageHandler(0x8c0)]
        public static void ReceiveDealGuildInviteRsp(CSPkg msg)
        {
            IsError(msg.stPkgData.stDealGuildInviteRsp.bResult);
        }

        [MessageHandler(0x8ea)]
        public static void ReceiveGetGroupGuildIdNtf(CSPkg msg)
        {
            GuildInfo currentGuildInfo = Singleton<CGuildSystem>.GetInstance().Model.CurrentGuildInfo;
            if (currentGuildInfo != null)
            {
                currentGuildInfo.groupGuildId = msg.stPkgData.stGetGroupGuildIDNtf.dwGroupGuildID;
                Debug.Log("Got 32 bit guild id from server: " + currentGuildInfo.groupGuildId);
                if (HasManageQQGroupAuthority())
                {
                    Singleton<CGuildInfoView>.GetInstance().BindQQGroup();
                }
            }
        }

        [MessageHandler(0x8d7)]
        public static void ReceiveGetGuildDividendRsp(CSPkg msg)
        {
            if (!IsError(msg.stPkgData.stGetGuildDividendRsp.bResult))
            {
                uint weekDividend = Singleton<CGuildModel>.GetInstance().GetPlayerGuildMemberInfo().WeekDividend;
                string[] args = new string[] { weekDividend.ToString() };
                string text = Singleton<CTextManager>.GetInstance().GetText("Guild_Get_Dividend_Success", args);
                Singleton<CUIManager>.GetInstance().OpenTips(text, false, 1.5f, null, new object[0]);
            }
        }

        [MessageHandler(0x8ee)]
        public static void ReceiveGetGuildEventRsp(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            Singleton<CGuildInfoView>.GetInstance().OpenLogForm(msg.stPkgData.stGuildEventRsp);
        }

        [MessageHandler(0xa38)]
        public static void ReceiveGetPlayerGuildRankInfoRsp(CSPkg msg)
        {
            SCPKG_GET_SPECIAL_GUILD_RANK_INFO_RSP stGetSpecialGuildRankInfoRsp = msg.stPkgData.stGetSpecialGuildRankInfoRsp;
            if (stGetSpecialGuildRankInfoRsp.bNumberType == 0x10)
            {
                RankpointRankInfo info = CreateRankpointRankInfo(stGetSpecialGuildRankInfoRsp.stItemDetail, enGuildRankpointRankListType.SeasonSelf);
                Singleton<CGuildInfoView>.GetInstance().SetRankpointPlayerGuildRank(info, null);
            }
            else if ((stGetSpecialGuildRankInfoRsp.bNumberType >= 0x33) || (stGetSpecialGuildRankInfoRsp.bNumberType <= 0x3b))
            {
                Singleton<EventRouter>.GetInstance().BroadCastEvent<SCPKG_GET_SPECIAL_GUILD_RANK_INFO_RSP>("UnionRank_Get_Rank_Team_Account_Info", stGetSpecialGuildRankInfoRsp);
            }
        }

        [MessageHandler(0xa36)]
        public static void ReceiveGetRankListBySpecialScoreRsp(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            SCPKG_GET_RANKLIST_BY_SPECIAL_SCORE_RSP stGetRankListBySpecialScoreRsp = msg.stPkgData.stGetRankListBySpecialScoreRsp;
            if (stGetRankListBySpecialScoreRsp.bNumberType == 0x10)
            {
                RefreshRankpointRankInfoList(enGuildRankpointRankListType.SeasonSelf, stGetRankListBySpecialScoreRsp.dwItemNum, stGetRankListBySpecialScoreRsp.astItemDetail);
                Singleton<CGuildInfoView>.GetInstance().RefreshRankpointSeasonRankList(null);
            }
        }

        [MessageHandler(0x8a6)]
        public static void ReceiveGuildAddNtf(CSPkg msg)
        {
            COMDT_GUILD_INFO stGuildInfo = msg.stPkgData.stAddGuildNtf.stGuildInfo;
            GuildInfo currentGuildInfo = Singleton<CGuildSystem>.GetInstance().Model.CurrentGuildInfo;
            currentGuildInfo.briefInfo = GetLocalGuildBriefInfo(stGuildInfo.stBriefInfo);
            currentGuildInfo.uulCreatedTime = stGuildInfo.ullBuildTime;
            currentGuildInfo.dwActive = stGuildInfo.dwActive;
            currentGuildInfo.dwCoinPool = stGuildInfo.dwCoinPool;
            currentGuildInfo.dwGuildMoney = stGuildInfo.dwGuildMoney;
            currentGuildInfo.listMemInfo.Clear();
            int num = Mathf.Min(stGuildInfo.stBriefInfo.bMemberNum, stGuildInfo.astMemberInfo.Length);
            for (byte i = 0; i < num; i = (byte) (i + 1))
            {
                GuildMemInfo item = new GuildMemInfo();
                item.stBriefInfo.uulUid = stGuildInfo.astMemberInfo[i].stBriefInfo.ullUid;
                item.stBriefInfo.dwLogicWorldId = stGuildInfo.astMemberInfo[i].stBriefInfo.iLogicWorldID;
                item.stBriefInfo.szHeadUrl = StringHelper.UTF8BytesToString(ref stGuildInfo.astMemberInfo[i].stBriefInfo.szHeadUrl);
                item.stBriefInfo.dwLevel = stGuildInfo.astMemberInfo[i].stBriefInfo.dwLevel;
                item.stBriefInfo.sName = StringHelper.UTF8BytesToString(ref stGuildInfo.astMemberInfo[i].stBriefInfo.szName);
                item.stBriefInfo.dwGameEntity = stGuildInfo.astMemberInfo[i].stBriefInfo.dwGameEntity;
                item.dwConstruct = stGuildInfo.astMemberInfo[i].dwConstruct;
                item.enPosition = (COM_PLAYER_GUILD_STATE) stGuildInfo.astMemberInfo[i].bPosition;
                item.TotalContruct = stGuildInfo.astMemberInfo[i].dwTotalConstruct;
                item.CurrActive = stGuildInfo.astMemberInfo[i].dwCurrActive;
                item.WeekActive = stGuildInfo.astMemberInfo[i].dwWeekActive;
                item.DonateCnt = stGuildInfo.astMemberInfo[i].bDonateCnt;
                item.DonateNum = stGuildInfo.astMemberInfo[i].dwDonateNum;
                item.WeekDividend = stGuildInfo.astMemberInfo[i].dwWeekDividend;
                item.RankInfo.killCnt = stGuildInfo.astMemberInfo[i].stRankInfo.dwKillCnt;
                item.RankInfo.deadCnt = stGuildInfo.astMemberInfo[i].stRankInfo.dwDeadCnt;
                item.RankInfo.assistCnt = stGuildInfo.astMemberInfo[i].stRankInfo.dwAssistCnt;
                item.RankInfo.weekRankPoint = stGuildInfo.astMemberInfo[i].stRankInfo.dwWeekRankPoint;
                item.RankInfo.byGameRankPoint = stGuildInfo.astMemberInfo[i].stRankInfo.dwGameRP;
                item.RankInfo.isSigned = stGuildInfo.astMemberInfo[i].stRankInfo.bSignIn > 0;
                item.stBriefInfo.stVip.score = stGuildInfo.astMemberInfo[i].stBriefInfo.stVip.dwScore;
                item.stBriefInfo.stVip.level = stGuildInfo.astMemberInfo[i].stBriefInfo.stVip.dwCurLevel;
                item.stBriefInfo.stVip.headIconId = stGuildInfo.astMemberInfo[i].stBriefInfo.stVip.dwHeadIconId;
                item.stBriefInfo.dwScoreOfRank = stGuildInfo.astMemberInfo[i].stBriefInfo.dwScoreOfRank;
                item.stBriefInfo.dwClassOfRank = stGuildInfo.astMemberInfo[i].stBriefInfo.dwClassOfRank;
                item.LastLoginTime = stGuildInfo.astMemberInfo[i].dwLastLoginTime;
                item.JoinTime = stGuildInfo.astMemberInfo[i].dwJoinTime;
                SetLocalMemberGuildMatchInfo(item.GuildMatchInfo, stGuildInfo.astMemberInfo[i].stGuildMatchInfo);
                if (item.stBriefInfo.uulUid == Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID)
                {
                    Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.guildState = item.enPosition;
                    s_lastByGameRankpoint = item.RankInfo.byGameRankPoint;
                }
                if (item.enPosition == COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_CHAIRMAN)
                {
                    currentGuildInfo.chairman = item;
                }
                currentGuildInfo.listMemInfo.Add(item);
            }
            currentGuildInfo.listBuildingInfo.Clear();
            int num3 = Mathf.Min(stGuildInfo.bBuildingCnt, stGuildInfo.astBuildingInfo.Length);
            for (int j = 0; j < num3; j++)
            {
                GuildBuildingInfo info3 = new GuildBuildingInfo();
                info3.type = (RES_GUILD_BUILDING_TYPE) stGuildInfo.astBuildingInfo[j].bBuildingType;
                info3.level = stGuildInfo.astBuildingInfo[j].bLevel;
                currentGuildInfo.listBuildingInfo.Add(info3);
            }
            currentGuildInfo.RankInfo.totalRankPoint = stGuildInfo.stRankInfo.dwTotalRankPoint;
            currentGuildInfo.RankInfo.seasonStartTime = stGuildInfo.stRankInfo.dwSeasonStartTime;
            currentGuildInfo.RankInfo.weekRankPoint = stGuildInfo.stRankInfo.dwWeekRankPoint;
            currentGuildInfo.star = stGuildInfo.dwStar;
            currentGuildInfo.groupGuildId = stGuildInfo.dwGroupGuildID;
            currentGuildInfo.groupOpenId = StringHelper.UTF8BytesToString(ref stGuildInfo.szGroupOpenID);
            SetLocalGuildMatchInfo(currentGuildInfo.GuildMatchInfo, stGuildInfo.stGuildMatchInfo);
            SetLocalGuildMatchObInfo(currentGuildInfo.GuildMatchObInfos, stGuildInfo);
            Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.uulUid = currentGuildInfo.briefInfo.uulUid;
            Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.name = currentGuildInfo.briefInfo.sName;
            Singleton<CGuildSystem>.GetInstance().Model.m_PlayerGuildLastState = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.guildState;
            Singleton<CGuildSystem>.GetInstance().Model.RemoveAppliedGuildInfo(currentGuildInfo.briefInfo.uulUid);
            Singleton<EventRouter>.GetInstance().BroadCastEvent<GuildInfo>("Guild_Create_Or_Add_Success", currentGuildInfo);
            Singleton<EventRouter>.GetInstance().BroadCastEvent("Guild_Enter_Guild");
        }

        [MessageHandler(0x8ca)]
        public static void ReceiveGuildAppointPositionRsp(CSPkg msg)
        {
            if (!IsError(msg.stPkgData.stAppointPositionRsp.bResult))
            {
                Singleton<CUIManager>.GetInstance().OpenTips("Guild_Appoint_Success", true, 1.5f, null, new object[0]);
            }
        }

        [MessageHandler(0x8df)]
        public static void ReceiveGuildConstructChgRsp(CSPkg msg)
        {
            GuildMemInfo playerGuildMemberInfo = Singleton<CGuildSystem>.GetInstance().Model.GetPlayerGuildMemberInfo();
            DebugHelper.Assert(playerGuildMemberInfo != null, "CGuildSystem.ReceiveGuildConstructChgRsp(): playerGuildMemberInfo is null!!!");
            if (playerGuildMemberInfo != null)
            {
                playerGuildMemberInfo.dwConstruct = msg.stPkgData.stGuildConstructChg.dwCurConstruct;
            }
        }

        [MessageHandler(0x8d8)]
        public static void ReceiveGuildCrossDayRsp(CSPkg msg)
        {
            SCPKG_GUILD_CROSS_DAY_NTF stGuildCrossDayNtf = msg.stPkgData.stGuildCrossDayNtf;
            Singleton<CGuildModel>.GetInstance().CurrentGuildInfo.dwGuildMoney = stGuildCrossDayNtf.dwGuildMoney;
            CGuildHelper.SetHallBuildingLevel(stGuildCrossDayNtf.bBuildingHallLevel);
            Singleton<CGuildModel>.GetInstance().CurrentGuildInfo.dwActive = stGuildCrossDayNtf.dwGuildActive;
            GuildMemInfo playerGuildMemberInfo = Singleton<CGuildModel>.GetInstance().GetPlayerGuildMemberInfo();
            playerGuildMemberInfo.CurrActive = stGuildCrossDayNtf.dwCurrActive;
            playerGuildMemberInfo.DonateCnt = stGuildCrossDayNtf.bDonateCnt;
            playerGuildMemberInfo.RankInfo.maxRankPoint = stGuildCrossDayNtf.dwMaxRankPoint;
            playerGuildMemberInfo.RankInfo.isSigned = false;
            playerGuildMemberInfo.RankInfo.byGameRankPoint = 0;
            s_lastByGameRankpoint = 0;
        }

        [MessageHandler(0x8d9)]
        public static void ReceiveGuildCrossWeekRsp(CSPkg msg)
        {
            SCPKG_GUILD_CROSS_WEEK_NTF stGuildCrossWeekNtf = msg.stPkgData.stGuildCrossWeekNtf;
            Singleton<CGuildModel>.GetInstance().CurrentGuildInfo.dwCoinPool = stGuildCrossWeekNtf.dwCoinPool;
            Singleton<CGuildModel>.GetInstance().GetPlayerGuildMemberInfo().WeekActive = stGuildCrossWeekNtf.dwWeekActive;
            Singleton<CGuildModel>.GetInstance().GetPlayerGuildMemberInfo().WeekDividend = stGuildCrossWeekNtf.dwWeekDividend;
        }

        [MessageHandler(0x8d5)]
        public static void ReceiveGuildDonateRsp(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            if (!IsError(msg.stPkgData.stGuildDonateRsp.bResult))
            {
                string donateSuccessTip = CGuildHelper.GetDonateSuccessTip(Singleton<CGuildModel>.GetInstance().CurrentDonateType);
                Singleton<CUIManager>.GetInstance().OpenTips(donateSuccessTip, false, 1.5f, null, new object[0]);
                SCPKG_GUILD_DONATE_RSP stGuildDonateRsp = msg.stPkgData.stGuildDonateRsp;
                Singleton<CGuildSystem>.GetInstance().Model.CurrentGuildInfo.dwGuildMoney = stGuildDonateRsp.dwGuildMoney;
                Singleton<CGuildSystem>.GetInstance().Model.CurrentGuildInfo.dwCoinPool = stGuildDonateRsp.dwCoinPool;
                GuildMemInfo playerGuildMemberInfo = Singleton<CGuildSystem>.GetInstance().Model.GetPlayerGuildMemberInfo();
                if (playerGuildMemberInfo != null)
                {
                    playerGuildMemberInfo.DonateCnt = stGuildDonateRsp.bDonateCnt;
                    playerGuildMemberInfo.dwConstruct = stGuildDonateRsp.dwConstruct;
                    playerGuildMemberInfo.TotalContruct = stGuildDonateRsp.dwTotalConstruct;
                }
            }
        }

        [MessageHandler(0x8ce)]
        public static void ReceiveGuildFireMemberNtf(CSPkg msg)
        {
            if (msg.stPkgData.stFireGuildMemberNtf.ullUid == Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID)
            {
                Singleton<CUIManager>.GetInstance().OpenTips("Guild_You_Have_Been_Fired", true, 1.5f, null, new object[0]);
                Singleton<CGuildInfoView>.GetInstance().CloseForm();
                Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.guildState = COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_NULL;
                Singleton<EventRouter>.GetInstance().BroadCastEvent("Guild_Leave_Guild");
            }
            else
            {
                ListView<GuildMemInfo> listMemInfo = Singleton<CGuildSystem>.GetInstance().Model.CurrentGuildInfo.listMemInfo;
                for (int i = listMemInfo.Count - 1; i >= 0; i--)
                {
                    if (msg.stPkgData.stFireGuildMemberNtf.ullUid == listMemInfo[i].stBriefInfo.uulUid)
                    {
                        listMemInfo.RemoveAt(i);
                        Singleton<CGuildSystem>.GetInstance().Model.CurrentGuildInfo.briefInfo.bMemberNum = (byte) (Singleton<CGuildSystem>.GetInstance().Model.CurrentGuildInfo.briefInfo.bMemberNum - 1);
                        break;
                    }
                }
                Singleton<CFriendContoller>.instance.model.SetGameFriendGuildState(msg.stPkgData.stFireGuildMemberNtf.ullUid, COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_NULL);
                Singleton<CGuildInfoView>.GetInstance().RefreshInfoPanel();
                Singleton<CGuildInfoView>.GetInstance().RefreshMemberPanel();
            }
        }

        [MessageHandler(0x8cd)]
        public static void ReceiveGuildFireMemberRsp(CSPkg msg)
        {
            if (!IsError(msg.stPkgData.stFireGuildMemberRsp.bResult))
            {
                ulong ullUid = msg.stPkgData.stFireGuildMemberRsp.ullUid;
                string sName = Singleton<CGuildSystem>.GetInstance().Model.GetGuildMemberInfoByUid(ullUid).stBriefInfo.sName;
                string[] args = new string[] { sName };
                Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Guild_Fire_Member_Success", args), true, 1.5f, null, new object[0]);
            }
        }

        [MessageHandler(0x89e)]
        public static void ReceiveGuildInfo(CSPkg msg)
        {
            if (IsError(msg.stPkgData.stGetGuildInfoRsp.bResult))
            {
                Singleton<EventRouter>.GetInstance().BroadCastEvent("Receive_Guild_Info_Failed");
            }
            else
            {
                COMDT_GUILD_INFO stGuildInfo = msg.stPkgData.stGetGuildInfoRsp.stGuildInfo;
                GuildInfo info = new GuildInfo();
                info.briefInfo = GetLocalGuildBriefInfo(stGuildInfo.stBriefInfo);
                info.uulCreatedTime = stGuildInfo.ullBuildTime;
                info.dwActive = stGuildInfo.dwActive;
                info.dwCoinPool = stGuildInfo.dwCoinPool;
                info.dwGuildMoney = stGuildInfo.dwGuildMoney;
                info.listMemInfo.Clear();
                int num = Mathf.Min(stGuildInfo.stBriefInfo.bMemberNum, stGuildInfo.astMemberInfo.Length);
                for (byte i = 0; i < num; i = (byte) (i + 1))
                {
                    GuildMemInfo item = new GuildMemInfo();
                    COMDT_GUILD_MEMBER_INFO comdt_guild_member_info = stGuildInfo.astMemberInfo[i];
                    item.stBriefInfo.uulUid = comdt_guild_member_info.stBriefInfo.ullUid;
                    item.stBriefInfo.dwLogicWorldId = comdt_guild_member_info.stBriefInfo.iLogicWorldID;
                    item.stBriefInfo.szHeadUrl = StringHelper.UTF8BytesToString(ref comdt_guild_member_info.stBriefInfo.szHeadUrl);
                    item.stBriefInfo.dwLevel = comdt_guild_member_info.stBriefInfo.dwLevel;
                    item.stBriefInfo.dwAbility = comdt_guild_member_info.stBriefInfo.dwAbility;
                    item.stBriefInfo.sName = StringHelper.UTF8BytesToString(ref comdt_guild_member_info.stBriefInfo.szName);
                    item.stBriefInfo.dwGameEntity = comdt_guild_member_info.stBriefInfo.dwGameEntity;
                    item.dwConstruct = comdt_guild_member_info.dwConstruct;
                    item.enPosition = (COM_PLAYER_GUILD_STATE) comdt_guild_member_info.bPosition;
                    item.TotalContruct = comdt_guild_member_info.dwTotalConstruct;
                    item.CurrActive = comdt_guild_member_info.dwCurrActive;
                    item.WeekActive = comdt_guild_member_info.dwWeekActive;
                    item.DonateCnt = comdt_guild_member_info.bDonateCnt;
                    item.DonateNum = comdt_guild_member_info.dwDonateNum;
                    item.WeekDividend = comdt_guild_member_info.dwWeekDividend;
                    item.RankInfo.maxRankPoint = comdt_guild_member_info.stRankInfo.dwMaxRankPoint;
                    item.RankInfo.totalRankPoint = comdt_guild_member_info.stRankInfo.dwTotalRankPoint;
                    item.RankInfo.killCnt = comdt_guild_member_info.stRankInfo.dwKillCnt;
                    item.RankInfo.deadCnt = comdt_guild_member_info.stRankInfo.dwDeadCnt;
                    item.RankInfo.assistCnt = comdt_guild_member_info.stRankInfo.dwAssistCnt;
                    item.RankInfo.weekRankPoint = comdt_guild_member_info.stRankInfo.dwWeekRankPoint;
                    item.RankInfo.byGameRankPoint = comdt_guild_member_info.stRankInfo.dwGameRP;
                    item.RankInfo.isSigned = comdt_guild_member_info.stRankInfo.bSignIn > 0;
                    item.stBriefInfo.stVip.score = comdt_guild_member_info.stBriefInfo.stVip.dwScore;
                    item.stBriefInfo.stVip.level = comdt_guild_member_info.stBriefInfo.stVip.dwCurLevel;
                    item.stBriefInfo.stVip.headIconId = comdt_guild_member_info.stBriefInfo.stVip.dwHeadIconId;
                    item.stBriefInfo.dwScoreOfRank = comdt_guild_member_info.stBriefInfo.dwScoreOfRank;
                    item.stBriefInfo.dwClassOfRank = comdt_guild_member_info.stBriefInfo.dwClassOfRank;
                    item.LastLoginTime = comdt_guild_member_info.dwLastLoginTime;
                    item.JoinTime = comdt_guild_member_info.dwJoinTime;
                    SetLocalMemberGuildMatchInfo(item.GuildMatchInfo, comdt_guild_member_info.stGuildMatchInfo);
                    if (item.stBriefInfo.uulUid == Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID)
                    {
                        Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.guildState = item.enPosition;
                        s_lastByGameRankpoint = item.RankInfo.byGameRankPoint;
                        Singleton<EventRouter>.GetInstance().BroadCastEvent<bool>("Guild_Sign_State_Changed", item.RankInfo.isSigned);
                    }
                    if (item.enPosition == COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_CHAIRMAN)
                    {
                        info.chairman = item;
                    }
                    info.listMemInfo.Add(item);
                }
                info.listBuildingInfo.Clear();
                int num3 = Mathf.Min(stGuildInfo.bBuildingCnt, stGuildInfo.astBuildingInfo.Length);
                for (int j = 0; j < num3; j++)
                {
                    GuildBuildingInfo info3 = new GuildBuildingInfo();
                    info3.type = (RES_GUILD_BUILDING_TYPE) stGuildInfo.astBuildingInfo[j].bBuildingType;
                    info3.level = stGuildInfo.astBuildingInfo[j].bLevel;
                    info.listBuildingInfo.Add(info3);
                }
                info.listSelfRecommendInfo.Clear();
                if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.guildState == COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_CHAIRMAN)
                {
                    int num5 = Mathf.Min(stGuildInfo.bSelfRecommendCnt, stGuildInfo.astSelfRecommendInfo.Length);
                    for (int k = 0; k < num5; k++)
                    {
                        GuildSelfRecommendInfo info4 = new GuildSelfRecommendInfo();
                        info4.uid = stGuildInfo.astSelfRecommendInfo[k].ullUid;
                        info4.time = stGuildInfo.astSelfRecommendInfo[k].dwTime;
                        info.listSelfRecommendInfo.Add(info4);
                    }
                }
                info.RankInfo.totalRankPoint = stGuildInfo.stRankInfo.dwTotalRankPoint;
                info.RankInfo.seasonStartTime = stGuildInfo.stRankInfo.dwSeasonStartTime;
                info.RankInfo.weekRankPoint = stGuildInfo.stRankInfo.dwWeekRankPoint;
                Singleton<CNameChangeSystem>.GetInstance().SetGuildNameChangeCount((int) stGuildInfo.dwChangeNameCnt);
                info.star = stGuildInfo.dwStar;
                info.groupGuildId = stGuildInfo.dwGroupGuildID;
                info.groupOpenId = StringHelper.UTF8BytesToString(ref stGuildInfo.szGroupOpenID);
                SetLocalGuildMatchInfo(info.GuildMatchInfo, stGuildInfo.stGuildMatchInfo);
                SetLocalGuildMatchObInfo(info.GuildMatchObInfos, stGuildInfo);
                s_isGuildMaxGrade = CGuildHelper.IsGuildMaxGrade();
                s_isGuildHighestMatchScore = CGuildHelper.IsGuildHighestMatchScore();
                Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
                Singleton<EventRouter>.GetInstance().BroadCastEvent<GuildInfo>("Receive_Guild_Info_Success", info);
                Singleton<EventRouter>.GetInstance().BroadCastEvent<COMDT_GUILD_REWARDPOINT_LIST>("UnionRank_Get_Rank_Tram_Account_Point", msg.stPkgData.stGetGuildInfoRsp.stGuildPoint);
            }
        }

        [MessageHandler(0x8b6)]
        public static void ReceiveGuildInviteRsp(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            stInvitedFriend friend = new stInvitedFriend();
            if (IsError(msg.stPkgData.stGuildInviteRsp.bResult))
            {
                if (msg.stPkgData.stGuildInviteRsp.bResult == 6)
                {
                    friend.uulUid = msg.stPkgData.stGuildInviteRsp.ullBeInviteUid;
                    friend.dwInviteTime = (int) msg.stPkgData.stGuildInviteRsp.dwInviteTime;
                    Singleton<CGuildSystem>.GetInstance().Model.AddInvitedFriend(friend, true);
                    Singleton<CUIManager>.GetInstance().OpenTips("Guild_Friend_Has_Been_Invited_Tip", true, 1.5f, null, new object[0]);
                    Singleton<EventRouter>.GetInstance().BroadCastEvent("Guild_Invite_Success");
                }
            }
            else
            {
                friend.uulUid = msg.stPkgData.stGuildInviteRsp.ullBeInviteUid;
                friend.dwInviteTime = (int) msg.stPkgData.stGuildInviteRsp.dwInviteTime;
                Singleton<CGuildSystem>.GetInstance().Model.AddInvitedFriend(friend, true);
                Singleton<CGuildSystem>.GetInstance().Model.AddInviteTimeInfo(friend.uulUid, friend.dwInviteTime);
                Singleton<CUIManager>.GetInstance().OpenTips("Guild_Invite_Success_Tip", true, 1.5f, null, new object[0]);
                Singleton<EventRouter>.GetInstance().BroadCastEvent("Guild_Invite_Success");
            }
        }

        [MessageHandler(0x8cb)]
        public static void ReceiveGuildPositionChgNtf(CSPkg msg)
        {
            int bCount = msg.stPkgData.stGuildPositionChgNtf.bCount;
            COMDT_MEMBER_POSITION[] astChgInfo = msg.stPkgData.stGuildPositionChgNtf.astChgInfo;
            ListView<GuildMemInfo> listMemInfo = Singleton<CGuildSystem>.GetInstance().Model.CurrentGuildInfo.listMemInfo;
            for (int i = 0; i < bCount; i++)
            {
                for (int j = 0; j < listMemInfo.Count; j++)
                {
                    if (astChgInfo[i].ullUid == listMemInfo[j].stBriefInfo.uulUid)
                    {
                        listMemInfo[j].enPosition = (COM_PLAYER_GUILD_STATE) astChgInfo[i].bPosition;
                        if (listMemInfo[j].enPosition == COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_CHAIRMAN)
                        {
                            Singleton<CGuildSystem>.GetInstance().Model.CurrentGuildInfo.chairman = listMemInfo[j];
                        }
                        if (astChgInfo[i].ullUid == Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID)
                        {
                            Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.guildState = (COM_PLAYER_GUILD_STATE) astChgInfo[i].bPosition;
                        }
                    }
                }
            }
            Singleton<CGuildInfoView>.GetInstance().RefreshInfoPanel();
            Singleton<CGuildInfoView>.GetInstance().RefreshMemberPanel();
        }

        [MessageHandler(0x8b3)]
        public static void ReceiveGuildQuitRsp(CSPkg msg)
        {
            if (IsError(msg.stPkgData.stQuitGuildRsp.bResult))
            {
                Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
                Singleton<EventRouter>.GetInstance().BroadCastEvent("Guild_Quit_Failed");
            }
            else
            {
                Singleton<CGuildSystem>.GetInstance().Model.m_PlayerGuildLastState = COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_NULL;
                Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.guildState = COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_NULL;
                Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.uulUid = 0L;
                Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.name = string.Empty;
                Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_extGuildInfo.dwLastQuitGuildTime = (uint) CRoleInfo.GetCurrentUTCTime();
                Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
                Singleton<EventRouter>.GetInstance().BroadCastEvent("Guild_Quit_Success");
                Singleton<EventRouter>.GetInstance().BroadCastEvent("Guild_Leave_Guild");
            }
        }

        [MessageHandler(0x8dc)]
        public static void ReceiveGuildRankResetNtf(CSPkg msg)
        {
            Singleton<CGuildSystem>.GetInstance().ResetRankpointAllRankInfo();
        }

        [MessageHandler(0x8bc)]
        public static void ReceiveGuildRecommendNtf(CSPkg msg)
        {
            COMDT_GUILD_RECOMMEND_INFO stRecommendInfo = msg.stPkgData.stGuildRecommendNtf.stRecommendInfo;
            stRecommendInfo info = new stRecommendInfo();
            info.uid = stRecommendInfo.ullUid;
            info.logicWorldID = stRecommendInfo.iLogicWorldID;
            info.headUrl = StringHelper.UTF8BytesToString(ref stRecommendInfo.szHeadUrl);
            info.name = StringHelper.UTF8BytesToString(ref stRecommendInfo.szName);
            info.level = stRecommendInfo.dwLevel;
            info.ability = stRecommendInfo.dwAbility;
            info.recommendName = StringHelper.UTF8BytesToString(ref stRecommendInfo.szRecommendName);
            info.stVip.score = stRecommendInfo.stVip.dwScore;
            info.stVip.level = stRecommendInfo.stVip.dwCurLevel;
            info.stVip.headIconId = stRecommendInfo.stVip.dwHeadIconId;
            info.rankScore = stRecommendInfo.dwScoreOfRank;
            info.rankClass = stRecommendInfo.dwRankClass;
            info.gender = stRecommendInfo.bGender;
            Singleton<CGuildSystem>.GetInstance().Model.AddRecommendInfo(info);
            s_isApplyAndRecommendListEmpty = false;
        }

        [MessageHandler(0x8bb)]
        public static void ReceiveGuildRecommendRsp(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            if (!IsError(msg.stPkgData.stGuildRecommendRsp.bResult))
            {
                Singleton<CGuildSystem>.GetInstance().Model.AddRecommendTimeInfo(msg.stPkgData.stGuildRecommendRsp.ullAcntUid, (int) msg.stPkgData.stGuildRecommendRsp.dwRecommendTime);
                Singleton<CUIManager>.GetInstance().OpenTips("Guild_Recommend_Success_Tip", true, 1.5f, null, new object[0]);
                Singleton<EventRouter>.GetInstance().BroadCastEvent("Guild_Recommend_Success");
            }
        }

        [MessageHandler(0x8e8)]
        public static void ReceiveGuildSeasonResetNtf(CSPkg msg)
        {
            GuildInfo currentGuildInfo = Singleton<CGuildSystem>.GetInstance().Model.CurrentGuildInfo;
            if (currentGuildInfo != null)
            {
                SCPKG_GUILD_SEASON_RESET_NTF stGuildSeasonResetNtf = msg.stPkgData.stGuildSeasonResetNtf;
                currentGuildInfo.RankInfo.totalRankPoint = 0;
                currentGuildInfo.GuildMatchInfo.dwScore = stGuildSeasonResetNtf.dwGuildMatchScore;
                currentGuildInfo.GuildMatchInfo.dwLastSeasonRankNo = stGuildSeasonResetNtf.dwGuildMatchLastSeasonRankNo;
                for (int i = 0; i < currentGuildInfo.listMemInfo.Count; i++)
                {
                    GuildMemInfo info2 = currentGuildInfo.listMemInfo[i];
                    info2.RankInfo.totalRankPoint = 0;
                    info2.GuildMatchInfo.dwScore = 0;
                }
            }
        }

        [MessageHandler(0x8d1)]
        public static void ReceiveGuildSelfCommendNtf(CSPkg msg)
        {
            if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.guildState == COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_CHAIRMAN)
            {
                ulong ullUid = msg.stPkgData.stGuildSelfRecommemdNtf.stSelfRecommend.ullUid;
                uint dwTime = msg.stPkgData.stGuildSelfRecommemdNtf.stSelfRecommend.dwTime;
                Singleton<CGuildSystem>.GetInstance().Model.AddSelfRecommendInfo(ullUid, dwTime);
            }
        }

        [MessageHandler(0x8d0)]
        public static void ReceiveGuildSelfCommendRsp(CSPkg msg)
        {
            if (!IsError(msg.stPkgData.stGuildSelfRecommemdRsp.bResult))
            {
                Singleton<CUIManager>.GetInstance().OpenTips("Guild_Self_Recommend_Success", true, 1.5f, null, new object[0]);
            }
        }

        [MessageHandler(0x8aa)]
        public static void ReceiveGuildSettingModifyRsp(CSPkg msg)
        {
            SCPKG_MODIFY_GUILD_SETTING_RSP stModifyGuildSettingRsp = msg.stPkgData.stModifyGuildSettingRsp;
            if (IsError(stModifyGuildSettingRsp.bResult))
            {
                Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
                Singleton<EventRouter>.GetInstance().BroadCastEvent("Guild_Setting_Modify_Failed");
            }
            else
            {
                Singleton<CGuildSystem>.GetInstance().Model.CurrentGuildInfo.briefInfo.dwSettingMask = stModifyGuildSettingRsp.dwSettingMask;
                Singleton<CGuildSystem>.GetInstance().Model.CurrentGuildInfo.briefInfo.LevelLimit = stModifyGuildSettingRsp.bLimitLevel;
                Singleton<CGuildSystem>.GetInstance().Model.CurrentGuildInfo.briefInfo.GradeLimit = stModifyGuildSettingRsp.bLimitGrade;
                Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
                Singleton<EventRouter>.GetInstance().BroadCastEvent<uint>("Guild_Setting_Modify_Success", stModifyGuildSettingRsp.dwSettingMask);
            }
        }

        [MessageHandler(0x8e7)]
        public static void ReceiveGuildSignInRsp(CSPkg msg)
        {
            if (!IsError((byte) msg.stPkgData.stGuildSignInRsp.iResult))
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if (masterRoleInfo != null)
                {
                    int num = (masterRoleInfo.m_rankGrade <= 0) ? 1 : masterRoleInfo.m_rankGrade;
                    ResRankGradeConf dataByKey = GameDataMgr.rankGradeDatabin.GetDataByKey((long) num);
                    if (dataByKey != null)
                    {
                        uint dwGuildSignInPoint = dataByKey.dwGuildSignInPoint;
                        string[] args = new string[] { dwGuildSignInPoint.ToString() };
                        string text = Singleton<CTextManager>.GetInstance().GetText("Guild_Sign_Success_Tip", args);
                        Singleton<CGuildInfoView>.GetInstance().OpenSignSuccessForm(text);
                        Singleton<EventRouter>.GetInstance().BroadCastEvent<bool>("Guild_Sign_State_Changed", true);
                    }
                }
            }
        }

        [MessageHandler(0x14d3)]
        public static void ReceiveGuildWeekResetNtf(CSPkg msg)
        {
            GuildInfo currentGuildInfo = Singleton<CGuildSystem>.GetInstance().Model.CurrentGuildInfo;
            if (currentGuildInfo != null)
            {
                SCPKG_GUILD_MATCH_WEEK_RESET_NTF stGuildMatchWeekResetNtf = msg.stPkgData.stGuildMatchWeekResetNtf;
                currentGuildInfo.GuildMatchInfo.dwWeekScore = 0;
                currentGuildInfo.GuildMatchInfo.dwLastRankNo = stGuildMatchWeekResetNtf.dwGuildMatchLastRankNo;
                for (int i = 0; i < currentGuildInfo.listMemInfo.Count; i++)
                {
                    GuildMemInfo info2 = currentGuildInfo.listMemInfo[i];
                    info2.GuildMatchInfo.bContinueWin = 0;
                    info2.GuildMatchInfo.dwWeekScore = 0;
                    info2.GuildMatchInfo.bWeekMatchCnt = 0;
                }
            }
        }

        [MessageHandler(0x8a7)]
        public static void ReceiveMemberOnlineNtf(CSPkg msg)
        {
            SCPKG_MEMBER_ONLINE_NTF stMemberOnlineNtf = msg.stPkgData.stMemberOnlineNtf;
            GuildMemInfo guildMemberInfoByUid = Singleton<CGuildSystem>.GetInstance().Model.GetGuildMemberInfoByUid(stMemberOnlineNtf.ullMemberUid);
            if (guildMemberInfoByUid != null)
            {
                guildMemberInfoByUid.stBriefInfo.dwGameEntity = stMemberOnlineNtf.dwGameEntity;
                guildMemberInfoByUid.LastLoginTime = stMemberOnlineNtf.dwLastLoginTime;
                if (stMemberOnlineNtf.dwGameEntity == 0)
                {
                    guildMemberInfoByUid.GuildMatchInfo.bIsReady = 0;
                    Singleton<CGuildMatchSystem>.GetInstance().SetTeamMemberReadyState(stMemberOnlineNtf.ullMemberUid, false);
                    Singleton<CGuildMatchSystem>.GetInstance().RefreshTeamList();
                }
            }
        }

        [MessageHandler(0x8b4)]
        public static void ReceiveMemberQuitNtf(CSPkg msg)
        {
            ulong ullQuitUid = msg.stPkgData.stQuitGuildNtf.ullQuitUid;
            for (int i = Singleton<CGuildSystem>.GetInstance().Model.CurrentGuildInfo.listMemInfo.Count - 1; i >= 0; i--)
            {
                GuildMemInfo info = Singleton<CGuildSystem>.GetInstance().Model.CurrentGuildInfo.listMemInfo[i];
                if (info.stBriefInfo.uulUid == ullQuitUid)
                {
                    Singleton<CGuildSystem>.GetInstance().Model.CurrentGuildInfo.listMemInfo.RemoveAt(i);
                    Singleton<CGuildSystem>.GetInstance().Model.CurrentGuildInfo.briefInfo.bMemberNum = (byte) (Singleton<CGuildSystem>.GetInstance().Model.CurrentGuildInfo.briefInfo.bMemberNum - 1);
                }
            }
            Singleton<EventRouter>.GetInstance().BroadCastEvent("Guild_Member_Quit");
        }

        [MessageHandler(0x8db)]
        public static void ReceiveMemberRankPointNtf(CSPkg msg)
        {
            SCPKG_MEMBER_RANK_POINT_NTF stMemberRankPointNtf = msg.stPkgData.stMemberRankPointNtf;
            GuildMemInfo guildMemberInfoByUid = Singleton<CGuildModel>.GetInstance().GetGuildMemberInfoByUid(stMemberRankPointNtf.ullMemberUid);
            if (guildMemberInfoByUid != null)
            {
                guildMemberInfoByUid.RankInfo.maxRankPoint = stMemberRankPointNtf.dwMaxRankPoint;
                guildMemberInfoByUid.RankInfo.totalRankPoint = stMemberRankPointNtf.dwTotalRankPoint;
                guildMemberInfoByUid.RankInfo.weekRankPoint = stMemberRankPointNtf.dwWeekRankPoint;
                if (CGuildHelper.IsSelf(stMemberRankPointNtf.ullMemberUid))
                {
                    s_lastByGameRankpoint = guildMemberInfoByUid.RankInfo.byGameRankPoint;
                }
                guildMemberInfoByUid.RankInfo.byGameRankPoint = stMemberRankPointNtf.dwGameRP;
            }
            else
            {
                DebugHelper.Assert(false, "CGuildSystem.ReceiveMemberRankPointNtf(): info is null!!!");
            }
            Singleton<CGuildModel>.GetInstance().CurrentGuildInfo.RankInfo.totalRankPoint = stMemberRankPointNtf.dwGuildRankPoint;
            Singleton<CGuildModel>.GetInstance().CurrentGuildInfo.RankInfo.weekRankPoint = stMemberRankPointNtf.dwGuildWeekRankPoint;
        }

        [MessageHandler(0x8da)]
        public static void ReceiveMemberTopKDANtf(CSPkg msg)
        {
            SCPKG_MEMBER_TOP_KDA_NTF stMemberTopKDANtf = msg.stPkgData.stMemberTopKDANtf;
            GuildMemInfo guildMemberInfoByUid = Singleton<CGuildModel>.GetInstance().GetGuildMemberInfoByUid(stMemberTopKDANtf.ullMemberUid);
            if (guildMemberInfoByUid == null)
            {
                object[] inParameters = new object[] { stMemberTopKDANtf.ullMemberUid };
                DebugHelper.Assert(false, "CGuildSystem.ReceiveMemberTopKDANtf(): info is null, rsp.ullMemberUid={0}", inParameters);
            }
            else
            {
                guildMemberInfoByUid.RankInfo.killCnt = stMemberTopKDANtf.dwKillCnt;
                guildMemberInfoByUid.RankInfo.deadCnt = stMemberTopKDANtf.dwDeadCnt;
                guildMemberInfoByUid.RankInfo.assistCnt = stMemberTopKDANtf.dwAssistCnt;
            }
        }

        [MessageHandler(0x8b0)]
        public static void ReceiveNewMemberNtf(CSPkg msg)
        {
            COMDT_GUILD_MEMBER_INFO stNewMember = msg.stPkgData.stNewMemberJoinGuildNtf.stNewMember;
            GuildInfo currentGuildInfo = Singleton<CGuildSystem>.GetInstance().Model.CurrentGuildInfo;
            GuildMemInfo item = new GuildMemInfo();
            item.stBriefInfo.uulUid = stNewMember.stBriefInfo.ullUid;
            item.stBriefInfo.dwGameEntity = stNewMember.stBriefInfo.dwGameEntity;
            item.stBriefInfo.szHeadUrl = StringHelper.UTF8BytesToString(ref stNewMember.stBriefInfo.szHeadUrl);
            item.stBriefInfo.dwLevel = stNewMember.stBriefInfo.dwLevel;
            item.stBriefInfo.dwLogicWorldId = stNewMember.stBriefInfo.iLogicWorldID;
            item.stBriefInfo.sName = StringHelper.UTF8BytesToString(ref stNewMember.stBriefInfo.szName);
            item.dwConstruct = stNewMember.dwConstruct;
            item.enPosition = (COM_PLAYER_GUILD_STATE) stNewMember.bPosition;
            item.TotalContruct = stNewMember.dwTotalConstruct;
            item.CurrActive = stNewMember.dwCurrActive;
            item.WeekActive = stNewMember.dwWeekActive;
            item.DonateCnt = stNewMember.bDonateCnt;
            item.DonateNum = stNewMember.dwDonateNum;
            item.WeekDividend = stNewMember.dwWeekDividend;
            item.RankInfo.maxRankPoint = stNewMember.stRankInfo.dwMaxRankPoint;
            item.RankInfo.totalRankPoint = stNewMember.stRankInfo.dwTotalRankPoint;
            item.RankInfo.killCnt = stNewMember.stRankInfo.dwKillCnt;
            item.RankInfo.deadCnt = stNewMember.stRankInfo.dwDeadCnt;
            item.RankInfo.assistCnt = stNewMember.stRankInfo.dwAssistCnt;
            item.RankInfo.weekRankPoint = stNewMember.stRankInfo.dwWeekRankPoint;
            item.RankInfo.byGameRankPoint = stNewMember.stRankInfo.dwGameRP;
            item.RankInfo.isSigned = stNewMember.stRankInfo.bSignIn > 0;
            item.stBriefInfo.stVip.score = stNewMember.stBriefInfo.stVip.dwScore;
            item.stBriefInfo.stVip.level = stNewMember.stBriefInfo.stVip.dwCurLevel;
            item.stBriefInfo.stVip.headIconId = stNewMember.stBriefInfo.stVip.dwHeadIconId;
            item.stBriefInfo.dwScoreOfRank = stNewMember.stBriefInfo.dwScoreOfRank;
            item.stBriefInfo.dwClassOfRank = stNewMember.stBriefInfo.dwClassOfRank;
            item.LastLoginTime = stNewMember.dwLastLoginTime;
            item.JoinTime = stNewMember.dwJoinTime;
            SetLocalMemberGuildMatchInfo(item.GuildMatchInfo, stNewMember.stGuildMatchInfo);
            if (!currentGuildInfo.listMemInfo.Contains(item))
            {
                currentGuildInfo.listMemInfo.Add(item);
                currentGuildInfo.briefInfo.bMemberNum = (byte) (currentGuildInfo.briefInfo.bMemberNum + 1);
            }
            Singleton<EventRouter>.GetInstance().BroadCastEvent("Guild_New_Member");
        }

        [MessageHandler(0x8a8)]
        public static void ReceivePrepareGuildBreakNtf(CSPkg msg)
        {
            Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.guildState = COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_NULL;
            Singleton<CGuildSystem>.GetInstance().Model.ResetCurrentPrepareGuildInfo();
        }

        [MessageHandler(0x8a2)]
        public static void ReceivePrepareGuildCreateRsp(CSPkg msg)
        {
            SCPKG_CREATE_GUILD_RSP stCreateGuildRsp = msg.stPkgData.stCreateGuildRsp;
            if (IsError(stCreateGuildRsp.bResult))
            {
                Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.guildState = Singleton<CGuildSystem>.instance.Model.m_PlayerGuildLastState;
                Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
                Singleton<EventRouter>.GetInstance().BroadCastEvent("Receive_PrepareGuild_Create_Failed");
            }
            else
            {
                Singleton<CGuildSystem>.GetInstance().Model.m_PlayerGuildLastState = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.guildState = COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_PREPARE_CREATE;
                Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.uulUid = stCreateGuildRsp.stGuildInfo.stBriefInfo.ullGuildID;
                PrepareGuildInfo currentPrepareGuildInfo = Singleton<CGuildSystem>.GetInstance().Model.CurrentPrepareGuildInfo;
                currentPrepareGuildInfo.stBriefInfo.uulUid = stCreateGuildRsp.stGuildInfo.stBriefInfo.ullGuildID;
                currentPrepareGuildInfo.stBriefInfo.dwLogicWorldId = stCreateGuildRsp.stGuildInfo.stBriefInfo.iLogicWorldID;
                currentPrepareGuildInfo.stBriefInfo.sName = StringHelper.UTF8BytesToString(ref stCreateGuildRsp.stGuildInfo.stBriefInfo.szName);
                currentPrepareGuildInfo.stBriefInfo.bMemCnt = stCreateGuildRsp.stGuildInfo.stBriefInfo.bMemberNum;
                currentPrepareGuildInfo.stBriefInfo.dwHeadId = stCreateGuildRsp.stGuildInfo.stBriefInfo.dwHeadID;
                currentPrepareGuildInfo.stBriefInfo.sBulletin = StringHelper.UTF8BytesToString(ref stCreateGuildRsp.stGuildInfo.stBriefInfo.szNotice);
                currentPrepareGuildInfo.stBriefInfo.dwRequestTime = stCreateGuildRsp.stGuildInfo.stBriefInfo.dwRequestTime;
                currentPrepareGuildInfo.stBriefInfo.IsOnlyFriend = Convert.ToBoolean(stCreateGuildRsp.stGuildInfo.stBriefInfo.bIsOnlyFriend);
                currentPrepareGuildInfo.stBriefInfo.stCreatePlayer.uulUid = stCreateGuildRsp.stGuildInfo.stBriefInfo.stCreatePlayer.ullUid;
                currentPrepareGuildInfo.stBriefInfo.stCreatePlayer.dwLogicWorldId = stCreateGuildRsp.stGuildInfo.stBriefInfo.stCreatePlayer.iLogicWorldID;
                currentPrepareGuildInfo.stBriefInfo.stCreatePlayer.szHeadUrl = StringHelper.UTF8BytesToString(ref stCreateGuildRsp.stGuildInfo.stBriefInfo.stCreatePlayer.szHeadUrl);
                currentPrepareGuildInfo.stBriefInfo.stCreatePlayer.dwLevel = stCreateGuildRsp.stGuildInfo.stBriefInfo.stCreatePlayer.dwLevel;
                currentPrepareGuildInfo.stBriefInfo.stCreatePlayer.sName = StringHelper.UTF8BytesToString(ref stCreateGuildRsp.stGuildInfo.stBriefInfo.stCreatePlayer.szName);
                currentPrepareGuildInfo.stBriefInfo.stCreatePlayer.dwGameEntity = stCreateGuildRsp.stGuildInfo.stBriefInfo.stCreatePlayer.dwGameEntity;
                currentPrepareGuildInfo.stBriefInfo.stCreatePlayer.stVip.score = stCreateGuildRsp.stGuildInfo.stBriefInfo.stCreatePlayer.stVip.dwScore;
                currentPrepareGuildInfo.stBriefInfo.stCreatePlayer.stVip.level = stCreateGuildRsp.stGuildInfo.stBriefInfo.stCreatePlayer.stVip.dwCurLevel;
                currentPrepareGuildInfo.stBriefInfo.stCreatePlayer.stVip.headIconId = stCreateGuildRsp.stGuildInfo.stBriefInfo.stCreatePlayer.stVip.dwHeadIconId;
                currentPrepareGuildInfo.m_MemList.Clear();
                int num = Mathf.Min(stCreateGuildRsp.stGuildInfo.stBriefInfo.bMemberNum, stCreateGuildRsp.stGuildInfo.astMemberInfo.Length);
                for (byte i = 0; i < num; i = (byte) (i + 1))
                {
                    GuildMemInfo item = new GuildMemInfo();
                    item.stBriefInfo.uulUid = stCreateGuildRsp.stGuildInfo.astMemberInfo[i].ullUid;
                    item.stBriefInfo.dwLogicWorldId = stCreateGuildRsp.stGuildInfo.astMemberInfo[i].iLogicWorldID;
                    item.stBriefInfo.szHeadUrl = StringHelper.UTF8BytesToString(ref stCreateGuildRsp.stGuildInfo.astMemberInfo[i].szHeadUrl);
                    item.stBriefInfo.dwLevel = stCreateGuildRsp.stGuildInfo.astMemberInfo[i].dwLevel;
                    item.stBriefInfo.sName = StringHelper.UTF8BytesToString(ref stCreateGuildRsp.stGuildInfo.astMemberInfo[i].szName);
                    item.stBriefInfo.dwGameEntity = stCreateGuildRsp.stGuildInfo.astMemberInfo[i].dwGameEntity;
                    item.stBriefInfo.stVip.score = stCreateGuildRsp.stGuildInfo.astMemberInfo[i].stVip.dwScore;
                    item.stBriefInfo.stVip.level = stCreateGuildRsp.stGuildInfo.astMemberInfo[i].stVip.dwCurLevel;
                    item.stBriefInfo.stVip.headIconId = stCreateGuildRsp.stGuildInfo.astMemberInfo[i].stVip.dwHeadIconId;
                    currentPrepareGuildInfo.m_MemList.Add(item);
                }
                Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
                Singleton<EventRouter>.GetInstance().BroadCastEvent<PrepareGuildInfo>("Receive_PrepareGuild_Create_Success", currentPrepareGuildInfo);
            }
        }

        [MessageHandler(0x8a0)]
        public static void ReceivePrepareGuildInfo(CSPkg msg)
        {
            if (IsError(msg.stPkgData.stGetPrepareGuildInfoRsp.bResult))
            {
                Singleton<EventRouter>.GetInstance().BroadCastEvent("Receive_PrepareGuild_Info_Failed");
            }
            else
            {
                COMDT_PREPARE_GUILD_INFO stGuildInfo = msg.stPkgData.stGetPrepareGuildInfoRsp.stGuildInfo;
                PrepareGuildInfo currentPrepareGuildInfo = Singleton<CGuildSystem>.GetInstance().Model.CurrentPrepareGuildInfo;
                currentPrepareGuildInfo.stBriefInfo.uulUid = stGuildInfo.stBriefInfo.ullGuildID;
                currentPrepareGuildInfo.stBriefInfo.dwLogicWorldId = stGuildInfo.stBriefInfo.iLogicWorldID;
                currentPrepareGuildInfo.stBriefInfo.sName = StringHelper.UTF8BytesToString(ref stGuildInfo.stBriefInfo.szName);
                currentPrepareGuildInfo.stBriefInfo.bMemCnt = stGuildInfo.stBriefInfo.bMemberNum;
                currentPrepareGuildInfo.stBriefInfo.dwHeadId = stGuildInfo.stBriefInfo.dwHeadID;
                currentPrepareGuildInfo.stBriefInfo.dwRequestTime = stGuildInfo.stBriefInfo.dwRequestTime;
                currentPrepareGuildInfo.stBriefInfo.sBulletin = StringHelper.UTF8BytesToString(ref stGuildInfo.stBriefInfo.szNotice);
                currentPrepareGuildInfo.stBriefInfo.IsOnlyFriend = Convert.ToBoolean(stGuildInfo.stBriefInfo.bIsOnlyFriend);
                currentPrepareGuildInfo.stBriefInfo.stCreatePlayer.uulUid = stGuildInfo.stBriefInfo.stCreatePlayer.ullUid;
                currentPrepareGuildInfo.stBriefInfo.stCreatePlayer.dwGameEntity = stGuildInfo.stBriefInfo.stCreatePlayer.dwGameEntity;
                currentPrepareGuildInfo.stBriefInfo.stCreatePlayer.szHeadUrl = StringHelper.UTF8BytesToString(ref stGuildInfo.stBriefInfo.stCreatePlayer.szHeadUrl);
                currentPrepareGuildInfo.stBriefInfo.stCreatePlayer.dwLevel = stGuildInfo.stBriefInfo.stCreatePlayer.dwLevel;
                currentPrepareGuildInfo.stBriefInfo.stCreatePlayer.dwLogicWorldId = stGuildInfo.stBriefInfo.stCreatePlayer.iLogicWorldID;
                currentPrepareGuildInfo.stBriefInfo.stCreatePlayer.sName = StringHelper.UTF8BytesToString(ref stGuildInfo.stBriefInfo.stCreatePlayer.szName);
                currentPrepareGuildInfo.stBriefInfo.stCreatePlayer.stVip.score = stGuildInfo.stBriefInfo.stCreatePlayer.stVip.dwScore;
                currentPrepareGuildInfo.stBriefInfo.stCreatePlayer.stVip.level = stGuildInfo.stBriefInfo.stCreatePlayer.stVip.dwCurLevel;
                currentPrepareGuildInfo.stBriefInfo.stCreatePlayer.stVip.headIconId = stGuildInfo.stBriefInfo.stCreatePlayer.stVip.dwHeadIconId;
                currentPrepareGuildInfo.m_MemList.Clear();
                int num = Mathf.Min(stGuildInfo.stBriefInfo.bMemberNum, stGuildInfo.astMemberInfo.Length);
                for (byte i = 0; i < num; i = (byte) (i + 1))
                {
                    GuildMemInfo item = new GuildMemInfo();
                    item.stBriefInfo.uulUid = stGuildInfo.astMemberInfo[i].ullUid;
                    item.stBriefInfo.dwLogicWorldId = stGuildInfo.astMemberInfo[i].iLogicWorldID;
                    item.stBriefInfo.szHeadUrl = StringHelper.UTF8BytesToString(ref stGuildInfo.astMemberInfo[i].szHeadUrl);
                    item.stBriefInfo.dwLevel = stGuildInfo.astMemberInfo[i].dwLevel;
                    item.stBriefInfo.sName = StringHelper.UTF8BytesToString(ref stGuildInfo.astMemberInfo[i].szName);
                    item.stBriefInfo.dwGameEntity = stGuildInfo.astMemberInfo[i].dwGameEntity;
                    item.stBriefInfo.stVip.score = stGuildInfo.astMemberInfo[i].stVip.dwScore;
                    item.stBriefInfo.stVip.level = stGuildInfo.astMemberInfo[i].stVip.dwCurLevel;
                    item.stBriefInfo.stVip.headIconId = stGuildInfo.astMemberInfo[i].stVip.dwHeadIconId;
                    currentPrepareGuildInfo.m_MemList.Add(item);
                }
                Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
                Singleton<EventRouter>.GetInstance().BroadCastEvent<PrepareGuildInfo>("Receive_PrepareGuild_Info_Success", currentPrepareGuildInfo);
            }
        }

        [MessageHandler(0x8a4)]
        public static void ReceivePrepareGuildJoinRsp(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            if (IsError(msg.stPkgData.stJoinPrepareGuildRsp.bResult))
            {
                Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.guildState = Singleton<CGuildSystem>.GetInstance().Model.m_PlayerGuildLastState;
                Singleton<EventRouter>.GetInstance().BroadCastEvent<PrepareGuildInfo>("Receive_PrepareGuild_Join_Rsp", null);
            }
            else
            {
                COMDT_PREPARE_GUILD_INFO stGuildInfo = msg.stPkgData.stJoinPrepareGuildRsp.stGuildInfo;
                PrepareGuildInfo currentPrepareGuildInfo = Singleton<CGuildSystem>.GetInstance().Model.CurrentPrepareGuildInfo;
                currentPrepareGuildInfo.stBriefInfo.uulUid = stGuildInfo.stBriefInfo.ullGuildID;
                currentPrepareGuildInfo.stBriefInfo.dwLogicWorldId = stGuildInfo.stBriefInfo.iLogicWorldID;
                currentPrepareGuildInfo.stBriefInfo.sName = StringHelper.UTF8BytesToString(ref stGuildInfo.stBriefInfo.szName);
                currentPrepareGuildInfo.stBriefInfo.bMemCnt = stGuildInfo.stBriefInfo.bMemberNum;
                currentPrepareGuildInfo.stBriefInfo.dwHeadId = stGuildInfo.stBriefInfo.dwHeadID;
                currentPrepareGuildInfo.stBriefInfo.sBulletin = StringHelper.UTF8BytesToString(ref stGuildInfo.stBriefInfo.szNotice);
                currentPrepareGuildInfo.stBriefInfo.dwRequestTime = stGuildInfo.stBriefInfo.dwRequestTime;
                currentPrepareGuildInfo.stBriefInfo.IsOnlyFriend = Convert.ToBoolean(stGuildInfo.stBriefInfo.bIsOnlyFriend);
                currentPrepareGuildInfo.stBriefInfo.stCreatePlayer.uulUid = stGuildInfo.stBriefInfo.stCreatePlayer.ullUid;
                currentPrepareGuildInfo.stBriefInfo.stCreatePlayer.dwLogicWorldId = stGuildInfo.stBriefInfo.stCreatePlayer.iLogicWorldID;
                currentPrepareGuildInfo.stBriefInfo.stCreatePlayer.szHeadUrl = StringHelper.UTF8BytesToString(ref stGuildInfo.stBriefInfo.stCreatePlayer.szHeadUrl);
                currentPrepareGuildInfo.stBriefInfo.stCreatePlayer.dwLevel = stGuildInfo.stBriefInfo.stCreatePlayer.dwLevel;
                currentPrepareGuildInfo.stBriefInfo.stCreatePlayer.sName = StringHelper.UTF8BytesToString(ref stGuildInfo.stBriefInfo.stCreatePlayer.szName);
                currentPrepareGuildInfo.stBriefInfo.stCreatePlayer.dwGameEntity = stGuildInfo.stBriefInfo.stCreatePlayer.dwGameEntity;
                currentPrepareGuildInfo.stBriefInfo.stCreatePlayer.stVip.score = stGuildInfo.stBriefInfo.stCreatePlayer.stVip.dwScore;
                currentPrepareGuildInfo.stBriefInfo.stCreatePlayer.stVip.level = stGuildInfo.stBriefInfo.stCreatePlayer.stVip.dwCurLevel;
                currentPrepareGuildInfo.stBriefInfo.stCreatePlayer.stVip.headIconId = stGuildInfo.stBriefInfo.stCreatePlayer.stVip.dwHeadIconId;
                currentPrepareGuildInfo.m_MemList.Clear();
                int num = Mathf.Min(stGuildInfo.stBriefInfo.bMemberNum, stGuildInfo.astMemberInfo.Length);
                for (byte i = 0; i < num; i = (byte) (i + 1))
                {
                    GuildMemInfo item = new GuildMemInfo();
                    item.stBriefInfo.uulUid = stGuildInfo.astMemberInfo[i].ullUid;
                    item.stBriefInfo.dwLogicWorldId = stGuildInfo.astMemberInfo[i].iLogicWorldID;
                    item.stBriefInfo.szHeadUrl = StringHelper.UTF8BytesToString(ref stGuildInfo.astMemberInfo[i].szHeadUrl);
                    item.stBriefInfo.dwLevel = stGuildInfo.astMemberInfo[i].dwLevel;
                    item.stBriefInfo.sName = StringHelper.UTF8BytesToString(ref stGuildInfo.astMemberInfo[i].szName);
                    item.stBriefInfo.dwGameEntity = stGuildInfo.astMemberInfo[i].dwGameEntity;
                    item.stBriefInfo.stVip.score = stGuildInfo.astMemberInfo[i].stVip.dwScore;
                    item.stBriefInfo.stVip.level = stGuildInfo.astMemberInfo[i].stVip.dwCurLevel;
                    item.stBriefInfo.stVip.headIconId = stGuildInfo.astMemberInfo[i].stVip.dwHeadIconId;
                    currentPrepareGuildInfo.m_MemList.Add(item);
                }
                Singleton<CGuildSystem>.GetInstance().Model.m_PlayerGuildLastState = COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_PREPARE_JOIN;
                Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.guildState = COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_PREPARE_JOIN;
                Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.uulUid = currentPrepareGuildInfo.stBriefInfo.uulUid;
                Singleton<EventRouter>.GetInstance().BroadCastEvent<PrepareGuildInfo>("Receive_PrepareGuild_Join_Success", currentPrepareGuildInfo);
            }
        }

        [MessageHandler(0x89c)]
        public static void ReceivePrepareGuildList(CSPkg msg)
        {
            SCPKG_GET_PREPARE_GUILD_LIST_RSP stGetPrepareGuildListRsp = msg.stPkgData.stGetPrepareGuildListRsp;
            ListView<PrepareGuildInfo> view = new ListView<PrepareGuildInfo>();
            for (byte i = 0; i < stGetPrepareGuildListRsp.bGuildNum; i = (byte) (i + 1))
            {
                COMDT_PREPARE_GUILD_BRIEF_INFO comdt_prepare_guild_brief_info = stGetPrepareGuildListRsp.astGuildList[i];
                PrepareGuildInfo item = new PrepareGuildInfo();
                item.stBriefInfo.uulUid = comdt_prepare_guild_brief_info.ullGuildID;
                item.stBriefInfo.dwLogicWorldId = comdt_prepare_guild_brief_info.iLogicWorldID;
                item.stBriefInfo.sName = StringHelper.UTF8BytesToString(ref comdt_prepare_guild_brief_info.szName);
                item.stBriefInfo.bMemCnt = comdt_prepare_guild_brief_info.bMemberNum;
                item.stBriefInfo.dwHeadId = comdt_prepare_guild_brief_info.dwHeadID;
                item.stBriefInfo.dwRequestTime = comdt_prepare_guild_brief_info.dwRequestTime;
                item.stBriefInfo.sBulletin = StringHelper.UTF8BytesToString(ref comdt_prepare_guild_brief_info.szNotice);
                item.stBriefInfo.IsOnlyFriend = Convert.ToBoolean(comdt_prepare_guild_brief_info.bIsOnlyFriend);
                item.stBriefInfo.stCreatePlayer.uulUid = comdt_prepare_guild_brief_info.stCreatePlayer.ullUid;
                item.stBriefInfo.stCreatePlayer.dwLogicWorldId = comdt_prepare_guild_brief_info.stCreatePlayer.iLogicWorldID;
                item.stBriefInfo.stCreatePlayer.dwLevel = comdt_prepare_guild_brief_info.stCreatePlayer.dwLevel;
                item.stBriefInfo.stCreatePlayer.sName = StringHelper.UTF8BytesToString(ref comdt_prepare_guild_brief_info.stCreatePlayer.szName);
                item.stBriefInfo.stCreatePlayer.szHeadUrl = StringHelper.UTF8BytesToString(ref comdt_prepare_guild_brief_info.stCreatePlayer.szHeadUrl);
                item.stBriefInfo.stCreatePlayer.stVip.score = comdt_prepare_guild_brief_info.stCreatePlayer.stVip.dwScore;
                item.stBriefInfo.stCreatePlayer.stVip.level = comdt_prepare_guild_brief_info.stCreatePlayer.stVip.dwCurLevel;
                item.stBriefInfo.stCreatePlayer.stVip.headIconId = comdt_prepare_guild_brief_info.stCreatePlayer.stVip.dwHeadIconId;
                if (comdt_prepare_guild_brief_info.dwRequestTime < Singleton<CGuildSystem>.GetInstance().Model.m_PrepareGuildOldestRequestTime)
                {
                    Singleton<CGuildSystem>.GetInstance().Model.m_PrepareGuildOldestRequestTime = comdt_prepare_guild_brief_info.dwRequestTime;
                }
                view.Add(item);
            }
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            Singleton<EventRouter>.GetInstance().BroadCastEvent<ListView<PrepareGuildInfo>, uint, byte, byte>("Receive_PrepareGuild_List_Success", view, stGetPrepareGuildListRsp.dwTotalCnt, stGetPrepareGuildListRsp.bPageID, stGetPrepareGuildListRsp.bGuildNum);
        }

        [MessageHandler(0x8be)]
        public static void ReceiveRecommendListRsp(CSPkg msg)
        {
            stRecommendInfo item = new stRecommendInfo();
            List<stRecommendInfo> recommendList = new List<stRecommendInfo>();
            SCPKG_GET_GUILD_RECOMMEND_LIST_RSP stGetGuildRecommendListRsp = msg.stPkgData.stGetGuildRecommendListRsp;
            if (!Singleton<CGuildInfoView>.GetInstance().IsShow())
            {
                if (stGetGuildRecommendListRsp.dwTotalCnt > 0)
                {
                    s_isApplyAndRecommendListEmpty = false;
                }
            }
            else
            {
                for (int i = 0; i < stGetGuildRecommendListRsp.bCount; i++)
                {
                    COMDT_GUILD_RECOMMEND_INFO comdt_guild_recommend_info = stGetGuildRecommendListRsp.astRecommendInfo[i];
                    item.uid = comdt_guild_recommend_info.ullUid;
                    item.logicWorldID = comdt_guild_recommend_info.iLogicWorldID;
                    item.headUrl = StringHelper.UTF8BytesToString(ref comdt_guild_recommend_info.szHeadUrl);
                    item.name = StringHelper.UTF8BytesToString(ref comdt_guild_recommend_info.szName);
                    item.level = comdt_guild_recommend_info.dwLevel;
                    item.ability = comdt_guild_recommend_info.dwAbility;
                    item.recommendName = StringHelper.UTF8BytesToString(ref comdt_guild_recommend_info.szRecommendName);
                    item.stVip.score = comdt_guild_recommend_info.stVip.dwScore;
                    item.stVip.level = comdt_guild_recommend_info.stVip.dwCurLevel;
                    item.stVip.headIconId = comdt_guild_recommend_info.stVip.dwHeadIconId;
                    item.rankScore = comdt_guild_recommend_info.dwScoreOfRank;
                    item.rankClass = comdt_guild_recommend_info.dwRankClass;
                    item.gender = comdt_guild_recommend_info.bGender;
                    recommendList.Add(item);
                }
                Singleton<CGuildInfoController>.GetInstance().OnReceiveRecommendListSuccess(recommendList);
            }
        }

        [MessageHandler(0x8b8)]
        public static void ReceiveSearchGuildRsp(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            SCPKG_SEARCH_GUILD_RSP stSearchGuildRsp = msg.stPkgData.stSearchGuildRsp;
            if (!IsError(stSearchGuildRsp.bResult))
            {
                GuildInfo info = new GuildInfo();
                CSDT_RANKING_LIST_ITEM_INFO stGuildInfo = stSearchGuildRsp.stGuildInfo;
                info.briefInfo.uulUid = ulong.Parse(StringHelper.UTF8BytesToString(ref stGuildInfo.szOpenID));
                info.briefInfo.Rank = stGuildInfo.dwRankNo;
                info.briefInfo.Ability = stGuildInfo.dwRankScore;
                COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_POWER stGuildPower = stGuildInfo.stExtraInfo.stDetailInfo.stGuildPower;
                info.briefInfo.bLevel = stGuildPower.bGuildLevel;
                info.briefInfo.bMemberNum = stGuildPower.bMemberNum;
                info.briefInfo.dwHeadId = stGuildPower.dwGuildHeadID;
                info.briefInfo.sBulletin = StringHelper.UTF8BytesToString(ref stGuildPower.szGuildNotice);
                info.briefInfo.sName = StringHelper.UTF8BytesToString(ref stGuildPower.szGuildName);
                info.briefInfo.dwSettingMask = stGuildPower.dwSettingMask;
                info.briefInfo.LevelLimit = stGuildPower.bLimitLevel;
                info.briefInfo.GradeLimit = stGuildPower.bLimitGrade;
                info.chairman.stBriefInfo.sName = StringHelper.UTF8BytesToString(ref stGuildPower.szChairManName);
                info.chairman.stBriefInfo.szHeadUrl = StringHelper.UTF8BytesToString(ref stGuildPower.szChairManHeadUrl);
                info.chairman.stBriefInfo.dwLevel = stGuildPower.dwChairManLv;
                info.chairman.stBriefInfo.stVip.score = stGuildPower.stChairManVip.dwScore;
                info.chairman.stBriefInfo.stVip.level = stGuildPower.stChairManVip.dwCurLevel;
                info.chairman.stBriefInfo.stVip.headIconId = stGuildPower.stChairManVip.dwHeadIconId;
                info.star = stGuildPower.dwStar;
                info.RankInfo.totalRankPoint = stGuildPower.dwTotalRankPoint;
                Singleton<EventRouter>.GetInstance().BroadCastEvent<GuildInfo, int>("Receive_Guild_Search_Success", info, stSearchGuildRsp.bSearchType);
            }
        }

        [MessageHandler(0x8c2)]
        public static void ReceiveSearchPrepareGuildRsp(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            SCPKG_SEARCH_PREGUILD_RSP stSearchPreGuildRsp = msg.stPkgData.stSearchPreGuildRsp;
            if (!IsError(stSearchPreGuildRsp.bResult))
            {
                COMDT_PREPARE_GUILD_BRIEF_INFO stPreGuildBriefInfo = stSearchPreGuildRsp.stPreGuildBriefInfo;
                PrepareGuildInfo info = new PrepareGuildInfo();
                info.stBriefInfo.uulUid = stPreGuildBriefInfo.ullGuildID;
                info.stBriefInfo.dwLogicWorldId = stPreGuildBriefInfo.iLogicWorldID;
                info.stBriefInfo.sName = StringHelper.UTF8BytesToString(ref stPreGuildBriefInfo.szName);
                info.stBriefInfo.bMemCnt = stPreGuildBriefInfo.bMemberNum;
                info.stBriefInfo.dwHeadId = stPreGuildBriefInfo.dwHeadID;
                info.stBriefInfo.dwRequestTime = stPreGuildBriefInfo.dwRequestTime;
                info.stBriefInfo.sBulletin = StringHelper.UTF8BytesToString(ref stPreGuildBriefInfo.szNotice);
                info.stBriefInfo.stCreatePlayer.uulUid = stPreGuildBriefInfo.stCreatePlayer.ullUid;
                info.stBriefInfo.stCreatePlayer.dwLogicWorldId = stPreGuildBriefInfo.stCreatePlayer.iLogicWorldID;
                info.stBriefInfo.stCreatePlayer.dwLevel = stPreGuildBriefInfo.stCreatePlayer.dwLevel;
                info.stBriefInfo.stCreatePlayer.sName = StringHelper.UTF8BytesToString(ref stPreGuildBriefInfo.stCreatePlayer.szName);
                info.stBriefInfo.stCreatePlayer.szHeadUrl = StringHelper.UTF8BytesToString(ref stPreGuildBriefInfo.stCreatePlayer.szHeadUrl);
                info.stBriefInfo.stCreatePlayer.stVip.score = stPreGuildBriefInfo.stCreatePlayer.stVip.dwScore;
                info.stBriefInfo.stCreatePlayer.stVip.level = stPreGuildBriefInfo.stCreatePlayer.stVip.dwCurLevel;
                info.stBriefInfo.stCreatePlayer.stVip.headIconId = stPreGuildBriefInfo.stCreatePlayer.stVip.dwHeadIconId;
                Singleton<EventRouter>.GetInstance().BroadCastEvent<PrepareGuildInfo, int>("Receive_Search_Prepare_Guild_Success", info, stSearchPreGuildRsp.bSearchType);
            }
        }

        [MessageHandler(0x597)]
        public static void ReceiveSendGuildMailRsp(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            Singleton<CUIManager>.GetInstance().CloseForm(CMailSys.MAIL_WRITE_FORM_PATH);
            SCPKG_SEND_GUILD_MAIL_RSP stSendGuildMailRsp = msg.stPkgData.stSendGuildMailRsp;
            if (stSendGuildMailRsp.iResult == 0)
            {
                Singleton<CUIManager>.GetInstance().OpenTips("Mail_Send_Success", true, 1.5f, null, new object[0]);
                CGuildHelper.SetSendGuildMailCnt(stSendGuildMailRsp.bSendGuildMailCnt);
            }
            else
            {
                Singleton<CUIManager>.GetInstance().OpenTips(Utility.ProtErrCodeToStr(0x597, stSendGuildMailRsp.iResult), false, 1.5f, null, new object[0]);
            }
        }

        [MessageHandler(0x8f0)]
        public static void ReceiveSendGuildRecruitRsp(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            if (!IsError(msg.stPkgData.stSendGuildRecruitRsp.bErrCode))
            {
                Singleton<CUIManager>.GetInstance().OpenTips("Guild_Recruit_Send_Recruit_Success", true, 1.5f, null, new object[0]);
            }
        }

        [MessageHandler(0x8ec)]
        public static void ReceiveSetGuildGroupOpenIdNtf(CSPkg msg)
        {
            GuildInfo currentGuildInfo = Singleton<CGuildSystem>.GetInstance().Model.CurrentGuildInfo;
            if (currentGuildInfo != null)
            {
                currentGuildInfo.groupOpenId = StringHelper.UTF8BytesToString(ref msg.stPkgData.stSetGuildGroupOpenIDNtf.szGroupOpenID);
                Debug.Log("Got guildOpenId from server: " + currentGuildInfo.groupOpenId);
            }
        }

        [MessageHandler(0x8e5)]
        public static void ReceiveUpgradeGuildByDianQuanRsp(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            if (!IsError(msg.stPkgData.stUpgradeGuildByCouponsRsp.bResult))
            {
                Singleton<CUIManager>.GetInstance().OpenTips("Guild_Extend_Member_Limit_Success_Tip", true, 1.5f, null, new object[0]);
            }
        }

        private static void RefreshRankpointRankInfoList(enGuildRankpointRankListType rankListType, uint itemNum, CSDT_RANKING_LIST_ITEM_INFO[] itemDetails)
        {
            Singleton<CGuildModel>.GetInstance().RankpointRankGottens[(int) rankListType] = true;
            Singleton<CGuildModel>.GetInstance().RankpointRankLastGottenTimes[(int) rankListType] = CRoleInfo.GetCurrentUTCTime();
            ListView<RankpointRankInfo> view = Singleton<CGuildModel>.GetInstance().RankpointRankInfoLists[(int) rankListType];
            view.Clear();
            for (int i = 0; i < itemNum; i++)
            {
                CSDT_RANKING_LIST_ITEM_INFO csdt_ranking_list_item_info = itemDetails[i];
                RankpointRankInfo item = CreateRankpointRankInfo(csdt_ranking_list_item_info, rankListType);
                view.Add(item);
            }
        }

        public void RequestGuildInfo()
        {
            if (this.IsInNormalGuild())
            {
                this.m_infoController.RequestGuildInfo();
                this.m_infoController.RequestApplyList(0);
            }
        }

        private void ResetRankpointAllRankInfo()
        {
            this.m_Model.CurrentGuildInfo.RankInfo.weekRankPoint = 0;
            for (int i = 0; i < this.m_Model.CurrentGuildInfo.listMemInfo.Count; i++)
            {
                this.m_Model.CurrentGuildInfo.listMemInfo[i].RankInfo.weekRankPoint = 0;
            }
        }

        public void SearchGuild(ulong guildId, string guildName, byte searchSrc, bool isPrepareGuild)
        {
            CSPkg pkg;
            Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
            if (isPrepareGuild)
            {
                pkg = NetworkModule.CreateDefaultCSPKG(0x8c1);
                pkg.stPkgData.stSearchPreGuildReq.ullGuildID = guildId;
                StringHelper.StringToUTF8Bytes(guildName, ref pkg.stPkgData.stSearchPreGuildReq.szGuildName);
                pkg.stPkgData.stSearchPreGuildReq.bSearchType = searchSrc;
            }
            else
            {
                pkg = NetworkModule.CreateDefaultCSPKG(0x8b7);
                pkg.stPkgData.stSearchGuildReq.ullGuildID = guildId;
                StringHelper.StringToUTF8Bytes(guildName, ref pkg.stPkgData.stSearchGuildReq.szGuildName);
                pkg.stPkgData.stSearchGuildReq.bSearchType = searchSrc;
            }
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref pkg, false);
        }

        private static void SetLocalGuildMatchInfo(COMDT_GUILD_MATCH_INFO localInfo, COMDT_GUILD_MATCH_INFO protocolInfo)
        {
            localInfo.dwScore = protocolInfo.dwScore;
            localInfo.dwWeekScore = protocolInfo.dwWeekScore;
            localInfo.dwLastRankNo = protocolInfo.dwLastRankNo;
            localInfo.dwUpdRankNoTime = protocolInfo.dwUpdRankNoTime;
            localInfo.dwLastSeasonRankNo = protocolInfo.dwLastSeasonRankNo;
        }

        private static void SetLocalGuildMatchObInfo(ListView<COMDT_GUILD_MATCH_OB_INFO> localInfo, COMDT_GUILD_INFO protocolGuildInfo)
        {
            localInfo.Clear();
            int bOBMatchCnt = protocolGuildInfo.bOBMatchCnt;
            COMDT_GUILD_MATCH_OB_INFO[] astGuildMatchOBInfo = protocolGuildInfo.astGuildMatchOBInfo;
            for (int i = 0; i < bOBMatchCnt; i++)
            {
                localInfo.Add(astGuildMatchOBInfo[i]);
            }
        }

        private static void SetLocalMemberGuildMatchInfo(COMDT_MEMBER_GUILD_MATCH_INFO localInfo, COMDT_MEMBER_GUILD_MATCH_INFO protocolInfo)
        {
            localInfo.bIsLeader = protocolInfo.bIsLeader;
            localInfo.ullTeamLeaderUid = protocolInfo.ullTeamLeaderUid;
            localInfo.dwScore = protocolInfo.dwScore;
            localInfo.bContinueWin = protocolInfo.bContinueWin;
            localInfo.dwWeekScore = protocolInfo.dwWeekScore;
            localInfo.bIsReady = protocolInfo.bIsReady;
            localInfo.bWeekMatchCnt = protocolInfo.bWeekMatchCnt;
        }

        public CGuildModel Model
        {
            get
            {
                return this.m_Model;
            }
        }

        public enum enGuildSearchSrc
        {
            Normal,
            From_Mail_Hyper_Link,
            From_Chat_Hyper_Link
        }
    }
}

