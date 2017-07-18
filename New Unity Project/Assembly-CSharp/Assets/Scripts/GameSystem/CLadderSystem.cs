namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.UI;

    [MessageHandlerClass]
    public class CLadderSystem : Singleton<CLadderSystem>
    {
        private COMDT_RANKDETAIL currentRankDetail;
        private List<COMDT_RANK_CURSEASON_FIGHT_RECORD> currentSeasonGames;
        public static readonly string FORM_AD_PREFAB = "UGUI/Form/System/IDIPNotice/Form_LobbyADForm.prefab";
        public static readonly string FORM_LADDER_ENTRY = "UGUI/Form/System/PvP/Ladder/Form_LadderEntry.prefab";
        public static readonly string FORM_LADDER_GAMEINFO = "UGUI/Form/System/PvP/Ladder/Form_LadderGameInfo.prefab";
        public static readonly string FORM_LADDER_HISTORY = "UGUI/Form/System/PvP/Ladder/Form_LadderHistory.prefab";
        public static readonly string FORM_LADDER_KING = "UGUI/Form/System/PvP/Ladder/Form_LadderKing.prefab";
        public static readonly string FORM_LADDER_RECENT = "UGUI/Form/System/PvP/Ladder/Form_RecentLadderMatch.prefab";
        public static readonly string FORM_LADDER_REWARD = "UGUI/Form/System/PvP/Ladder/Form_LadderReward.prefab";
        private List<COMDT_RANK_PASTSEASON_FIGHT_RECORD> historySeasonData;
        private static string image_name = "UGUI/Sprite/Dynamic/Competition/million";
        public const ushort LADDER_BRAVE_SCORE_RULE_ID = 0x16;
        public const ushort LADDER_RULE_ID = 1;
        public const string LadderLatestShowKingFormTimePrefKey = "Ladder_LatestShowKingFormTimePrefKey";
        public const int MAX_NEED_SCORE = 5;
        public static int MAX_RANK_LEVEL;
        public const int NUM_LADDER_BY_TEAM_2 = 2;
        public const int NUM_LADDER_BY_TEAM_5 = 5;
        public static uint REQ_HERO_NUM;
        public static uint REQ_PLAYER_LEVEL;

        [MessageHandler(0xb65)]
        public static void AddCurrentSeasonRecord(CSPkg msg)
        {
            Singleton<CLadderSystem>.GetInstance().AddRecentGameData(msg.stPkgData.stNtfAddCurSeasonRecord.stRecord);
        }

        [MessageHandler(0xb66)]
        public static void AddHistorySeasonRecord(CSPkg msg)
        {
            Singleton<CLadderSystem>.GetInstance().AddRecentSeasonData(msg.stPkgData.stNtfAddPastSeasonRecord.stRecord);
        }

        private void AddRecentGameData(COMDT_RANK_CURSEASON_FIGHT_RECORD gameData)
        {
            if (this.currentSeasonGames == null)
            {
                this.currentSeasonGames = new List<COMDT_RANK_CURSEASON_FIGHT_RECORD>();
            }
            this.currentSeasonGames.Add(gameData);
            this.currentSeasonGames.Sort(new Comparison<COMDT_RANK_CURSEASON_FIGHT_RECORD>(CLadderSystem.ComparisonGameData));
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(FORM_LADDER_GAMEINFO);
            if (form != null)
            {
                CLadderView.SetGameInfoRecentPanel(form, this.currentRankDetail, this.currentSeasonGames);
            }
            CUIFormScript script2 = Singleton<CUIManager>.GetInstance().GetForm(FORM_LADDER_RECENT);
            if (script2 != null)
            {
                CLadderView.InitLadderRecent(script2, this.currentSeasonGames);
            }
        }

        private void AddRecentSeasonData(COMDT_RANK_PASTSEASON_FIGHT_RECORD gameData)
        {
            if (this.historySeasonData == null)
            {
                this.historySeasonData = new List<COMDT_RANK_PASTSEASON_FIGHT_RECORD>();
            }
            this.historySeasonData.Add(gameData);
            this.historySeasonData.Sort(new Comparison<COMDT_RANK_PASTSEASON_FIGHT_RECORD>(CLadderSystem.ComparisonHistoryData));
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(FORM_LADDER_HISTORY);
            if (form != null)
            {
                CLadderView.InitLadderHistory(form, this.historySeasonData);
            }
        }

        private void BeginMatch()
        {
            CMatchingSystem.ReqStartSingleMatching(GetRankBattleMapID(), false, COM_BATTLE_MAP_TYPE.COM_BATTLE_MAP_TYPE_RANK);
        }

        private bool CanOpenLadderEntry()
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo == null)
            {
                return false;
            }
            if (masterRoleInfo.PvpLevel < REQ_PLAYER_LEVEL)
            {
                object[] replaceArr = new object[] { REQ_PLAYER_LEVEL };
                Singleton<CUIManager>.GetInstance().OpenTips("Activity_Open", true, 1f, null, replaceArr);
                return false;
            }
            if (Singleton<CMatchingSystem>.GetInstance().IsInMatching)
            {
                Singleton<CUIManager>.GetInstance().OpenTips("PVP_Matching", true, 1.5f, null, new object[0]);
                return false;
            }
            if (!this.IsInCredit())
            {
                Singleton<CUIManager>.GetInstance().OpenTips("Credit_Forbid_Ladder", true, 1.5f, null, new object[0]);
                return false;
            }
            if (!IsInSeason())
            {
                Singleton<CUIManager>.GetInstance().OpenTips("Rank_Not_In_Season", true, 1.5f, null, new object[0]);
                return false;
            }
            if (this.IsQualified() && (masterRoleInfo.m_rankGrade <= 0))
            {
                Singleton<CUIManager>.GetInstance().OpenTips("Ladder_Data_Error", false, 1.5f, null, new object[0]);
                return false;
            }
            return true;
        }

        private static int ComparisonGameData(COMDT_RANK_CURSEASON_FIGHT_RECORD a, COMDT_RANK_CURSEASON_FIGHT_RECORD b)
        {
            if (a.dwFightTime > b.dwFightTime)
            {
                return -1;
            }
            if (a.dwFightTime < b.dwFightTime)
            {
                return 1;
            }
            return 0;
        }

        private static int ComparisonHistoryData(COMDT_RANK_PASTSEASON_FIGHT_RECORD a, COMDT_RANK_PASTSEASON_FIGHT_RECORD b)
        {
            if (a.dwSeaEndTime > b.dwSeaEndTime)
            {
                return -1;
            }
            if (a.dwSeaEndTime < b.dwSeaEndTime)
            {
                return 1;
            }
            return 0;
        }

        public static int ConvertEloToRank(uint elo)
        {
            int count = GameDataMgr.rankGradeDatabin.count;
            int num2 = (int) (elo / 0x10);
            int num3 = count;
            for (int i = 1; i <= count; i++)
            {
                ResRankGradeConf dataByKey = GameDataMgr.rankGradeDatabin.GetDataByKey((long) i);
                num2 -= (int) dataByKey.dwGradeUpNeedScore;
                if (num2 <= 0)
                {
                    return i;
                }
            }
            return num3;
        }

        public uint GetBraveScoreMax(uint rankGrade)
        {
            <GetBraveScoreMax>c__AnonStorey68 storey = new <GetBraveScoreMax>c__AnonStorey68();
            storey.rankGrade = rankGrade;
            ResRankGradeConf conf = GameDataMgr.rankGradeDatabin.FindIf(new Func<ResRankGradeConf, bool>(storey, (IntPtr) this.<>m__6E));
            if (conf != null)
            {
                return conf.dwAddStarScore;
            }
            return 0;
        }

        public uint GetContinuousWinCountForExtraStar()
        {
            return this.GetContinuousWinCountForExtraStar(Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_rankGrade);
        }

        public uint GetContinuousWinCountForExtraStar(uint rankGrade)
        {
            ResRankGradeConf dataByKey = GameDataMgr.rankGradeDatabin.GetDataByKey(rankGrade);
            if (dataByKey != null)
            {
                return dataByKey.dwConWinCnt;
            }
            return 0;
        }

        public COMDT_RANKDETAIL GetCurrentRankDetail()
        {
            return this.currentRankDetail;
        }

        public static int GetCurXingByEloAndRankLv(uint elo, uint lv)
        {
            int count = GameDataMgr.rankGradeDatabin.count;
            int num2 = (int) (elo / 0x10);
            if (lv >= count)
            {
                return ((num2 < 0xa3) ? 0 : (num2 - 0xa3));
            }
            for (int i = 1; i < count; i++)
            {
                ResRankGradeConf dataByKey = GameDataMgr.rankGradeDatabin.GetDataByKey((long) i);
                if (num2 <= dataByKey.dwGradeUpNeedScore)
                {
                    return num2;
                }
                num2 -= (int) dataByKey.dwGradeUpNeedScore;
            }
            return num2;
        }

        public byte GetHistorySeasonGrade(ulong time)
        {
            if (Singleton<CLadderSystem>.GetInstance().historySeasonData != null)
            {
                for (int i = 0; i < Singleton<CLadderSystem>.GetInstance().historySeasonData.Count; i++)
                {
                    if ((Singleton<CLadderSystem>.GetInstance().historySeasonData[i].dwSeaStartTime <= time) && (time < Singleton<CLadderSystem>.GetInstance().historySeasonData[i].dwSeaEndTime))
                    {
                        return Singleton<CLadderSystem>.GetInstance().historySeasonData[i].bGrade;
                    }
                }
            }
            return 0;
        }

        public string GetLadderSeasonName(ulong time)
        {
            foreach (KeyValuePair<uint, ResRankSeasonConf> pair in GameDataMgr.rankSeasonDict)
            {
                if ((pair.Value.ullStartTime <= time) && (time < pair.Value.ullEndTime))
                {
                    return pair.Value.szSeasonName;
                }
            }
            return string.Empty;
        }

        public static uint GetRankBattleMapID()
        {
            uint result = 0;
            uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_Ladder_Normal"), out result);
            return result;
        }

        public static byte GetRankBigGrade(byte rankGrade)
        {
            ResRankGradeConf dataByKey = GameDataMgr.rankGradeDatabin.GetDataByKey((uint) rankGrade);
            if (dataByKey != null)
            {
                return dataByKey.bBelongBigGrade;
            }
            return 0;
        }

        private void GetRankData()
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0xa32);
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        [MessageHandler(0xa33)]
        public static void GetRankInfoRsp(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            Singleton<CLadderSystem>.GetInstance().OpenLadderEntry();
        }

        public string GetRewardDesc(byte rankGrade)
        {
            byte rankBigGrade = GetRankBigGrade(rankGrade);
            int num2 = GameDataMgr.rankGradeDatabin.Count();
            for (int i = 0; i < num2; i++)
            {
                ResRankGradeConf dataByIndex = GameDataMgr.rankGradeDatabin.GetDataByIndex(i);
                if ((dataByIndex != null) && (dataByIndex.bBelongBigGrade == rankBigGrade))
                {
                    return dataByIndex.szRewardDesc;
                }
            }
            return string.Empty;
        }

        public uint GetSelfBraveScoreMax()
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if ((masterRoleInfo != null) && (masterRoleInfo.m_rankGrade > 0))
            {
                return this.GetBraveScoreMax(masterRoleInfo.m_rankGrade);
            }
            return 0;
        }

        public CUseable GetSkinRewardUseable()
        {
            uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 300).dwConfValue;
            return CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN, dwConfValue, 0);
        }

        public override void Init()
        {
            base.Init();
            REQ_PLAYER_LEVEL = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 100).dwConfValue;
            REQ_HERO_NUM = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x65).dwConfValue;
            MAX_RANK_LEVEL = GameDataMgr.rankGradeDatabin.Count();
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_OpenLadder, new CUIEventManager.OnUIEventHandler(this.OnOpenLadder));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ladder_StartMatching, new CUIEventManager.OnUIEventHandler(this.OnLadder_BeginMatch));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ladder_ShowHistory, new CUIEventManager.OnUIEventHandler(this.OnLadder_ShowHistory));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ladder_ShowRecent, new CUIEventManager.OnUIEventHandler(this.OnLadder_ShowRecent));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ladder_ShowRules, new CUIEventManager.OnUIEventHandler(this.OnLadder_ShowRules));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ladder_ShowBraveScoreRule, new CUIEventManager.OnUIEventHandler(this.OnLadder_ShowBraveScoreRule));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ladder_ShowGameInfo, new CUIEventManager.OnUIEventHandler(this.OnLadder_ShowGameInfo));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ladder_ExpandHistoryItem, new CUIEventManager.OnUIEventHandler(this.OnLadder_ExpandHistoryItem));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ladder_ShrinkHistoryItem, new CUIEventManager.OnUIEventHandler(this.OnLadder_ShrinkHistoryItem));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ladder_ConfirmSeasonRank, new CUIEventManager.OnUIEventHandler(this.OnLadder_ConfirmSeasonRank));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ladder_ReqGetSeasonReward, new CUIEventManager.OnUIEventHandler(this.OnLadder_ReqGetSeasonReward));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ladder_GetSeasonRewardDone, new CUIEventManager.OnUIEventHandler(this.OnLadder_GetSeasonRewardDone));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ladder_OnEntryFormOpened, new CUIEventManager.OnUIEventHandler(this.OnLadder_EntryFormOpened));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ladder_OnEntryFormClosed, new CUIEventManager.OnUIEventHandler(this.OnLadder_EntryFormClosed));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ladder_OnClickBpGuide, new CUIEventManager.OnUIEventHandler(this.OnLadder_OnClickBpGuide));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_ADButton, new CUIEventManager.OnUIEventHandler(this.OnMatching_ADButton));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_ADForm_Close, new CUIEventManager.OnUIEventHandler(this.OnMatching_ADForm_Close));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ladder_OnClickShowRecentUsedHero, new CUIEventManager.OnUIEventHandler(this.OnClickShowRecentUsedHero));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ladder_OnClickHideRecentUsedHero, new CUIEventManager.OnUIEventHandler(this.OnClickHideRecentUsedHero));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ladder_GetSkinReward, new CUIEventManager.OnUIEventHandler(this.OnLadder_GetSkinReward));
            Singleton<EventRouter>.GetInstance().AddEventHandler<uint, uint, uint>("HeroSkinAdd", new Action<uint, uint, uint>(this, (IntPtr) this.OnNtyAddSkin));
        }

        public bool IsCanGetSkinReward()
        {
            uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x13c).dwConfValue;
            return (((Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_rankGrade >= dwConfValue) && (this.currentRankDetail != null)) && (this.currentRankDetail.dwTotalFightCnt > 0));
        }

        public bool IsCurSeason(ulong time)
        {
            DictionaryView<uint, ResRankSeasonConf> rankSeasonDict = GameDataMgr.rankSeasonDict;
            if (rankSeasonDict != null)
            {
                DictionaryView<uint, ResRankSeasonConf>.Enumerator enumerator = rankSeasonDict.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    KeyValuePair<uint, ResRankSeasonConf> current = enumerator.Current;
                    if (current.Value.ullStartTime == Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_rankCurSeasonStartTime)
                    {
                        KeyValuePair<uint, ResRankSeasonConf> pair2 = enumerator.Current;
                        if (pair2.Value.ullStartTime <= time)
                        {
                            KeyValuePair<uint, ResRankSeasonConf> pair3 = enumerator.Current;
                            if (time < pair3.Value.ullEndTime)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        private bool IsFirstShowLadderKingFormBeforeDailyRefreshTime()
        {
            uint todayStartTimeSeconds = Utility.GetTodayStartTimeSeconds();
            int @int = PlayerPrefs.GetInt("Ladder_LatestShowKingFormTimePrefKey");
            int currentUTCTime = CRoleInfo.GetCurrentUTCTime();
            if ((todayStartTimeSeconds <= @int) && (@int <= currentUTCTime))
            {
                return false;
            }
            return true;
        }

        public bool IsGotSkinReward()
        {
            uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 300).dwConfValue;
            return Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().IsHaveHeroSkin(dwConfValue, false);
        }

        public bool IsHaveFightRecord(bool isSelf, int rankGrade, int rankStar)
        {
            if (isSelf)
            {
                if (this.currentRankDetail != null)
                {
                    return ((this.currentRankDetail.dwTotalFightCnt > 0) && (this.currentRankDetail.bState == 1));
                }
            }
            else if ((rankGrade > 1) || (rankStar > 0))
            {
                return true;
            }
            return false;
        }

        public bool IsInCredit()
        {
            ResGlobalInfo info2;
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            return (((masterRoleInfo != null) && GameDataMgr.svr2CltCfgDict.TryGetValue(5, out info2)) && (masterRoleInfo.creditScore > info2.dwConfValue));
        }

        public static bool IsInSeason()
        {
            ulong currentUTCTime = (ulong) CRoleInfo.GetCurrentUTCTime();
            foreach (KeyValuePair<uint, ResRankSeasonConf> pair in GameDataMgr.rankSeasonDict)
            {
                if ((currentUTCTime >= pair.Value.ullStartTime) && (currentUTCTime <= pair.Value.ullEndTime))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsLevelQualified()
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            return ((masterRoleInfo != null) && (masterRoleInfo.PvpLevel >= REQ_PLAYER_LEVEL));
        }

        public static bool IsMaxRankGrade(byte rankGrade)
        {
            return (rankGrade >= MAX_RANK_LEVEL);
        }

        public bool IsNeedPromptSkinReward()
        {
            return (this.IsCanGetSkinReward() && !this.IsGotSkinReward());
        }

        public bool IsQualified()
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            return ((masterRoleInfo != null) && ((masterRoleInfo.PvpLevel >= REQ_PLAYER_LEVEL) && (masterRoleInfo.GetHaveHeroCountWithoutBanHeroID(false, 3, GetRankBattleMapID()) >= REQ_HERO_NUM)));
        }

        public static bool IsRecentUsedHeroMaskSet(ref uint CtrlMask, COM_RECENT_USED_HERO_MASK mask)
        {
            return ((((ulong) CtrlMask) & ((long) mask)) > 0L);
        }

        public bool IsShowButtonIn5()
        {
            bool flag = false;
            ResGlobalInfo info = new ResGlobalInfo();
            if (GameDataMgr.svr2CltCfgDict.TryGetValue(0x15, out info))
            {
                flag = info.dwConfValue > 0;
            }
            return flag;
        }

        private bool IsShowLadderKingForm()
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            return (CLadderView.IsSuperKing(masterRoleInfo.m_rankGrade, (byte) masterRoleInfo.m_rankClass) && this.IsFirstShowLadderKingFormBeforeDailyRefreshTime());
        }

        public bool IsUseBpMode()
        {
            return this.IsUseBpMode(Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_rankGrade);
        }

        public bool IsUseBpMode(byte rankGrade)
        {
            ResGlobalInfo info = null;
            return (((GameDataMgr.svr2CltCfgDict != null) && GameDataMgr.svr2CltCfgDict.TryGetValue(14, out info)) && ((info != null) && (rankGrade >= info.dwConfValue)));
        }

        public bool IsValidGrade(int grade)
        {
            return ((grade > 0) && (grade <= MAX_RANK_LEVEL));
        }

        private void OnClickHideRecentUsedHero(CUIEvent uiEvent)
        {
            if (uiEvent.m_eventParams.togleIsOn)
            {
                this.ReqHideRecentUsedHero(true);
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if (masterRoleInfo != null)
                {
                    SetRecentUsedHeroMask(ref masterRoleInfo.recentUseHero.dwCtrlMask, COM_RECENT_USED_HERO_MASK.COM_RECENT_USED_HERO_HIDE, true);
                }
                CMiShuSystem.SendUIClickToServer(enUIClickReprotID.rp_CloseRecentUseHero);
            }
        }

        private void OnClickShowRecentUsedHero(CUIEvent uiEvent)
        {
            if (uiEvent.m_eventParams.togleIsOn)
            {
                this.ReqHideRecentUsedHero(false);
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if (masterRoleInfo != null)
                {
                    SetRecentUsedHeroMask(ref masterRoleInfo.recentUseHero.dwCtrlMask, COM_RECENT_USED_HERO_MASK.COM_RECENT_USED_HERO_HIDE, false);
                }
                CMiShuSystem.SendUIClickToServer(enUIClickReprotID.rp_OpenRecentUseHero);
            }
        }

        private void OnLadder_BeginMatch(CUIEvent uiEvent)
        {
            Button button = (uiEvent.m_srcWidget == null) ? null : uiEvent.m_srcWidget.GetComponent<Button>();
            if ((button != null) && button.get_interactable())
            {
                if (Singleton<CMatchingSystem>.GetInstance().IsInMatching)
                {
                    Singleton<CUIManager>.GetInstance().OpenTips("PVP_Matching", true, 1.5f, null, new object[0]);
                }
                else
                {
                    this.BeginMatch();
                }
            }
        }

        private void OnLadder_ConfirmSeasonRank(CUIEvent uiEvent)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                CLadderView.ShowSeasonEndGetRewardForm(masterRoleInfo.m_rankSeasonHighestGrade);
            }
        }

        private void OnLadder_EntryFormClosed(CUIEvent uiEvent)
        {
        }

        private void OnLadder_EntryFormOpened(CUIEvent uiEvent)
        {
        }

        private void OnLadder_ExpandHistoryItem(CUIEvent uiEvent)
        {
            if ((uiEvent.m_srcWidget != null) && (uiEvent.m_srcWidget.get_transform().get_parent() != null))
            {
                Transform transform = uiEvent.m_srcWidget.get_transform().get_parent().get_parent();
                if (transform != null)
                {
                    CLadderView.OnHistoryItemChange(transform.get_gameObject(), true);
                    CUIEventScript component = uiEvent.m_srcWidget.GetComponent<CUIEventScript>();
                    if (component != null)
                    {
                        component.m_onClickEventID = enUIEventID.Ladder_ShrinkHistoryItem;
                    }
                }
            }
        }

        private void OnLadder_GetSeasonRewardDone(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.GetInstance().CloseForm(FORM_LADDER_REWARD);
        }

        private void OnLadder_GetSkinReward(CUIEvent uiEvent)
        {
            this.ReqGetLadderRewardSkin();
        }

        private void OnLadder_OnClickBpGuide(CUIEvent uiEvent)
        {
            Singleton<CBattleGuideManager>.GetInstance().OpenBannerDlgByBannerGuideId(9, null, false);
        }

        private void OnLadder_ReqGetSeasonReward(CUIEvent uiEvent)
        {
            this.ReqGetSeasonReward();
        }

        private void OnLadder_ShowBraveScoreRule(CUIEvent uiEvent)
        {
            ResRuleText dataByKey = GameDataMgr.s_ruleTextDatabin.GetDataByKey((uint) 0x16);
            if (dataByKey != null)
            {
                string title = StringHelper.UTF8BytesToString(ref dataByKey.szTitle);
                string info = StringHelper.UTF8BytesToString(ref dataByKey.szContent);
                Singleton<CUIManager>.GetInstance().OpenInfoForm(title, info);
            }
        }

        private void OnLadder_ShowGameInfo(CUIEvent uiEvent)
        {
            CUIFormScript gameInfoForm = Singleton<CUIManager>.GetInstance().OpenForm(FORM_LADDER_GAMEINFO, false, true);
            if (gameInfoForm != null)
            {
                CLadderView.InitLadderGameInfo(gameInfoForm, this.currentRankDetail, this.currentSeasonGames);
            }
        }

        private void OnLadder_ShowHistory(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().OpenForm(FORM_LADDER_HISTORY, false, true);
            if (form != null)
            {
                CLadderView.InitLadderHistory(form, this.historySeasonData);
            }
        }

        private void OnLadder_ShowRecent(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().OpenForm(FORM_LADDER_RECENT, false, true);
            if (form != null)
            {
                CLadderView.InitLadderRecent(form, this.currentSeasonGames);
            }
        }

        private void OnLadder_ShowRules(CUIEvent uiEvent)
        {
            ResRuleText dataByKey = GameDataMgr.s_ruleTextDatabin.GetDataByKey((uint) 1);
            if (dataByKey != null)
            {
                string title = StringHelper.UTF8BytesToString(ref dataByKey.szTitle);
                string info = StringHelper.UTF8BytesToString(ref dataByKey.szContent);
                Singleton<CUIManager>.GetInstance().OpenInfoForm(title, info);
            }
        }

        private void OnLadder_ShrinkHistoryItem(CUIEvent uiEvent)
        {
            if ((uiEvent.m_srcWidget != null) && (uiEvent.m_srcWidget.get_transform().get_parent() != null))
            {
                Transform transform = uiEvent.m_srcWidget.get_transform().get_parent().get_parent();
                if (transform != null)
                {
                    CLadderView.OnHistoryItemChange(transform.get_gameObject(), false);
                    CUIEventScript component = uiEvent.m_srcWidget.GetComponent<CUIEventScript>();
                    if (component != null)
                    {
                        component.m_onClickEventID = enUIEventID.Ladder_ExpandHistoryItem;
                    }
                }
            }
        }

        private void OnMatching_ADButton(CUIEvent uiEvent)
        {
            CUIFormScript formScript = Singleton<CUIManager>.GetInstance().OpenForm(FORM_AD_PREFAB, false, true);
            Image component = formScript.get_transform().Find("Panel/Image").GetComponent<Image>();
            component.SetSprite(image_name, formScript, true, false, false, false);
            component.get_gameObject().CustomSetActive(true);
        }

        private void OnMatching_ADForm_Close(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.GetInstance().CloseForm(FORM_AD_PREFAB);
        }

        [MessageHandler(0x1455)]
        public static void OnMatchTeamDestroyNtf(CSPkg msg)
        {
            byte bReason = msg.stPkgData.stMatchTeamDestroyNtf.stDetail.bReason;
            if (bReason == 20)
            {
                string str = UT.Bytes2String(msg.stPkgData.stMatchTeamDestroyNtf.stDetail.stReasonDetail.szLeaveAcntName);
                string[] args = new string[] { str };
                string text = Singleton<CTextManager>.instance.GetText("Err_Invite_Result_3", args);
                Singleton<CUIManager>.instance.OpenTips(text, false, 1.5f, null, new object[0]);
                Singleton<EventRouter>.instance.BroadCastEvent<byte, string>(EventID.INVITE_TEAM_ERRCODE_NTF, bReason, string.Empty);
            }
            else
            {
                Singleton<CUIManager>.instance.OpenTips(string.Format("CSProtocolMacros.SCID_MATCHTEAM_DESTROY_NTF bReason = {0} ", bReason), false, 1.5f, null, new object[0]);
            }
        }

        private void OnNtyAddSkin(uint heroId, uint skinId, uint addReason)
        {
            if (addReason == 7)
            {
                enFormPriority priority = enFormPriority.Priority3;
                CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(Singleton<SettlementSystem>.GetInstance()._profitFormName);
                if (form != null)
                {
                    priority = form.m_priority + 1;
                }
                CUICommonSystem.ShowNewHeroOrSkin(heroId, skinId, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKIN, false, null, priority, 0, 0);
            }
        }

        private void OnOpenLadder(CUIEvent uiEvent)
        {
            Button button = (uiEvent.m_srcWidget == null) ? null : uiEvent.m_srcWidget.GetComponent<Button>();
            if (button != null)
            {
                if (!button.get_interactable())
                {
                    if (!this.IsLevelQualified())
                    {
                        object[] replaceArr = new object[] { REQ_PLAYER_LEVEL };
                        Singleton<CUIManager>.GetInstance().OpenTips("Activity_Open", true, 1f, null, replaceArr);
                    }
                    else if (!Singleton<SCModuleControl>.instance.GetActiveModule(COM_CLIENT_PLAY_TYPE.COM_CLIENT_PLAY_LADDER))
                    {
                        Singleton<CUIManager>.instance.OpenMessageBox(Singleton<SCModuleControl>.instance.PvpAndPvpOffTips, false);
                    }
                }
                else
                {
                    Singleton<CNewbieAchieveSys>.GetInstance().trackFlag = CNewbieAchieveSys.TrackFlag.None;
                    this.GetRankData();
                }
            }
            else if (!this.IsLevelQualified())
            {
                object[] objArray2 = new object[] { REQ_PLAYER_LEVEL };
                Singleton<CUIManager>.GetInstance().OpenTips("Activity_Open", true, 1f, null, objArray2);
            }
            else if (!Singleton<SCModuleControl>.instance.GetActiveModule(COM_CLIENT_PLAY_TYPE.COM_CLIENT_PLAY_LADDER))
            {
                Singleton<CUIManager>.instance.OpenMessageBox(Singleton<SCModuleControl>.instance.PvpAndPvpOffTips, false);
            }
            else
            {
                this.GetRankData();
            }
        }

        [MessageHandler(0xb64)]
        public static void OnReceiveRankSeasonReward(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            if (msg.stPkgData.stGetRankRewardRsp.bErrCode == 0)
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if ((masterRoleInfo != null) && (Singleton<CLadderSystem>.GetInstance().currentRankDetail != null))
                {
                    Singleton<SettlementSystem>.GetInstance().SetLadderDisplayOldAndNewGrade(1, 1, masterRoleInfo.m_rankSeasonHighestGrade, Singleton<CLadderSystem>.GetInstance().currentRankDetail.dwScore);
                    Singleton<SettlementSystem>.GetInstance().ShowLadderSettleFormWithoutSettle();
                }
                Singleton<CLadderSystem>.GetInstance().currentRankDetail.bGetReward = 1;
            }
            else
            {
                string strContent = string.Empty;
                if (msg.stPkgData.stGetRankRewardRsp.bErrCode == 1)
                {
                    strContent = Singleton<CTextManager>.GetInstance().GetText("GETRANKREWARD_ERR_STATE_INVALID");
                }
                else if (msg.stPkgData.stGetRankRewardRsp.bErrCode == 2)
                {
                    strContent = Singleton<CTextManager>.GetInstance().GetText("GETRANKREWARD_ERR_STATE_HaveGet");
                }
                else if (msg.stPkgData.stGetRankRewardRsp.bErrCode == 3)
                {
                    strContent = Singleton<CTextManager>.GetInstance().GetText("GETRANKREWARD_ERR_STATE_Others");
                }
                else
                {
                    strContent = Singleton<CTextManager>.GetInstance().GetText("GETRANKREWARD_ERR_STATE_None");
                }
                Singleton<CUIManager>.GetInstance().OpenMessageBox(strContent, false);
            }
        }

        private void OpenLadderEntry()
        {
            bool flag = ((this.currentRankDetail != null) && (this.currentRankDetail.bState == 2)) && (this.currentRankDetail.bGetReward == 0);
            if (!this.CanOpenLadderEntry())
            {
                if (flag)
                {
                    CLadderView.InitRewardForm(Singleton<CUIManager>.GetInstance().OpenForm(FORM_LADDER_REWARD, false, true), ref this.currentRankDetail);
                }
            }
            else
            {
                CUIFormScript form = Singleton<CUIManager>.GetInstance().OpenForm(FORM_LADDER_ENTRY, false, true);
                if (form != null)
                {
                    CLadderView.InitLadderEntry(form, ref this.currentRankDetail, this.IsQualified());
                    this.PromptSkinRewardIfNeed();
                    if (flag)
                    {
                        CLadderView.InitRewardForm(Singleton<CUIManager>.GetInstance().OpenForm(FORM_LADDER_REWARD, false, true), ref this.currentRankDetail);
                    }
                    if (this.IsShowLadderKingForm())
                    {
                        CLadderView.InitKingForm(Singleton<CUIManager>.GetInstance().OpenForm(FORM_LADDER_KING, false, true), ref this.currentRankDetail);
                        PlayerPrefs.SetInt("Ladder_LatestShowKingFormTimePrefKey", CRoleInfo.GetCurrentUTCTime());
                    }
                    bool show = this.IsShowButtonIn5();
                    CLadderView.ShowRankButtonIn5(form, show);
                }
            }
        }

        public void PromptSkinRewardIfNeed()
        {
            if (this.IsNeedPromptSkinReward())
            {
                CUseable[] items = new CUseable[] { this.GetSkinRewardUseable() };
                string text = Singleton<CTextManager>.GetInstance().GetText("Ladder_Get_Skin_Reward_Msg_Title");
                Singleton<CUIManager>.GetInstance().OpenAwardTip(items, text, true, enUIEventID.Ladder_GetSkinReward, false, false, "Form_Award");
            }
        }

        [MessageHandler(0xb62)]
        public static void ReceiveRankHistoryInfo(CSPkg msg)
        {
            if (Singleton<CLadderSystem>.GetInstance().historySeasonData == null)
            {
                Singleton<CLadderSystem>.GetInstance().historySeasonData = new List<COMDT_RANK_PASTSEASON_FIGHT_RECORD>();
            }
            Singleton<CLadderSystem>.GetInstance().historySeasonData.Clear();
            for (int i = 0; i < msg.stPkgData.stRankPastSeasonHistory.bNum; i++)
            {
                Singleton<CLadderSystem>.GetInstance().historySeasonData.Add(msg.stPkgData.stRankPastSeasonHistory.astRecord[i]);
            }
            Singleton<CLadderSystem>.GetInstance().historySeasonData.Sort(new Comparison<COMDT_RANK_PASTSEASON_FIGHT_RECORD>(CLadderSystem.ComparisonHistoryData));
        }

        [MessageHandler(0xa29)]
        public static void ReceiveRankInfo(CSPkg msg)
        {
            SCPKG_UPDRANKINFO_NTF stUpdateRankInfo = msg.stPkgData.stUpdateRankInfo;
            Singleton<CLadderSystem>.GetInstance().UpdateRankInfo(ref stUpdateRankInfo);
        }

        [MessageHandler(0xb61)]
        public static void ReceiveRankSeasonInfo(CSPkg msg)
        {
            if (Singleton<CLadderSystem>.GetInstance().currentSeasonGames == null)
            {
                Singleton<CLadderSystem>.GetInstance().currentSeasonGames = new List<COMDT_RANK_CURSEASON_FIGHT_RECORD>();
            }
            Singleton<CLadderSystem>.GetInstance().currentSeasonGames.Clear();
            for (int i = 0; i < msg.stPkgData.stRankCurSeasonHistory.bNum; i++)
            {
                Singleton<CLadderSystem>.GetInstance().currentSeasonGames.Add(msg.stPkgData.stRankCurSeasonHistory.astRecord[i]);
            }
            Singleton<CLadderSystem>.GetInstance().currentSeasonGames.Sort(new Comparison<COMDT_RANK_CURSEASON_FIGHT_RECORD>(CLadderSystem.ComparisonGameData));
        }

        private void ReqGetLadderRewardSkin()
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x14d4);
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        private void ReqGetSeasonReward()
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0xb63);
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        private void ReqHideRecentUsedHero(bool bHide)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x157c);
            msg.stPkgData.stShowRecentUsedHeroReq.bTurnOn = Convert.ToByte(!bHide);
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false) && (masterRoleInfo != null))
            {
                SetRecentUsedHeroMask(ref masterRoleInfo.recentUseHero.dwCtrlMask, COM_RECENT_USED_HERO_MASK.COM_RECENT_USED_HERO_HIDE, bHide);
            }
        }

        public static void SetRecentUsedHeroMask(ref uint CtrlMask, COM_RECENT_USED_HERO_MASK mask, bool bOpen)
        {
            if (bOpen)
            {
                CtrlMask = (uint) (((COM_RECENT_USED_HERO_MASK) CtrlMask) | mask);
            }
            else
            {
                CtrlMask = (uint) (((COM_RECENT_USED_HERO_MASK) CtrlMask) & ~mask);
            }
        }

        public override void UnInit()
        {
            this.currentRankDetail = null;
            if (this.currentSeasonGames != null)
            {
                this.currentSeasonGames.Clear();
                this.currentSeasonGames = null;
            }
            if (this.historySeasonData != null)
            {
                this.historySeasonData.Clear();
                this.historySeasonData = null;
            }
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Matching_OpenLadder, new CUIEventManager.OnUIEventHandler(this.OnOpenLadder));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ladder_StartMatching, new CUIEventManager.OnUIEventHandler(this.OnLadder_BeginMatch));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ladder_ShowHistory, new CUIEventManager.OnUIEventHandler(this.OnLadder_ShowHistory));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ladder_ShowRecent, new CUIEventManager.OnUIEventHandler(this.OnLadder_ShowRecent));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ladder_ShowRules, new CUIEventManager.OnUIEventHandler(this.OnLadder_ShowRules));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ladder_ExpandHistoryItem, new CUIEventManager.OnUIEventHandler(this.OnLadder_ExpandHistoryItem));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ladder_ShrinkHistoryItem, new CUIEventManager.OnUIEventHandler(this.OnLadder_ShrinkHistoryItem));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ladder_ConfirmSeasonRank, new CUIEventManager.OnUIEventHandler(this.OnLadder_ConfirmSeasonRank));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ladder_ReqGetSeasonReward, new CUIEventManager.OnUIEventHandler(this.OnLadder_ReqGetSeasonReward));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ladder_GetSeasonRewardDone, new CUIEventManager.OnUIEventHandler(this.OnLadder_GetSeasonRewardDone));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Matching_ADButton, new CUIEventManager.OnUIEventHandler(this.OnMatching_ADButton));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Matching_ADForm_Close, new CUIEventManager.OnUIEventHandler(this.OnMatching_ADForm_Close));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ladder_OnClickShowRecentUsedHero, new CUIEventManager.OnUIEventHandler(this.OnClickShowRecentUsedHero));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ladder_OnClickHideRecentUsedHero, new CUIEventManager.OnUIEventHandler(this.OnClickHideRecentUsedHero));
            base.UnInit();
        }

        private void UpdateRankInfo(ref SCPKG_UPDRANKINFO_NTF newData)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                masterRoleInfo.m_rankGrade = newData.bCurGrade;
                masterRoleInfo.m_rankClass = newData.dwCurClass;
                masterRoleInfo.m_rankSeasonHighestGrade = newData.stRankInfo.bMaxSeasonGrade;
                masterRoleInfo.m_rankSeasonHighestClass = newData.stRankInfo.dwMaxSeasonClass;
                masterRoleInfo.m_rankHistoryHighestGrade = newData.bMaxGradeOfRank;
                masterRoleInfo.m_rankHistoryHighestClass = newData.stRankInfo.dwTopClassOfRank;
                masterRoleInfo.m_rankCurSeasonStartTime = newData.stRankInfo.dwSeasonStartTime;
            }
            this.currentRankDetail = newData.stRankInfo;
            bool flag = (this.currentRankDetail.bState == 2) && (this.currentRankDetail.bGetReward == 0);
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(FORM_LADDER_ENTRY);
            if (form != null)
            {
                CLadderView.InitLadderEntry(form, ref this.currentRankDetail, this.IsQualified());
                if (flag)
                {
                    CLadderView.InitRewardForm(Singleton<CUIManager>.GetInstance().OpenForm(FORM_LADDER_REWARD, false, true), ref this.currentRankDetail);
                }
            }
        }

        [CompilerGenerated]
        private sealed class <GetBraveScoreMax>c__AnonStorey68
        {
            internal uint rankGrade;

            internal bool <>m__6E(ResRankGradeConf x)
            {
                return (x.bGrade == this.rankGrade);
            }
        }
    }
}

