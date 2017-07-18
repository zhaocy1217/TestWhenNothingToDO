namespace Assets.Scripts.GameSystem
{
    using Apollo;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    internal class CLobbySystem : Singleton<CLobbySystem>
    {
        private static bool _autoPoped = false;
        private GameObject _rankingBtn;
        [CompilerGenerated]
        private static bool <IsPlatChannelOpen>k__BackingField;
        private GameObject achievement_btn;
        private GameObject addSkill_btn;
        public static bool AutoPopAllow = true;
        private GameObject bag_btn;
        private GameObject hero_btn;
        public static string Ladder_BtnRes_PATH = (CUIUtility.s_Sprite_System_Lobby_Dir + "LadderBtnDynamic.prefab");
        public static string LOBBY_FORM_PATH = "UGUI/Form/System/Lobby/Form_Lobby.prefab";
        public static string LOBBY_FUN_UNLOCK_PATH = "UGUI/Form/System/Lobby/Form_FunUnLock.prefab";
        private bool m_bInLobby;
        private DictionaryView<int, GameObject> m_Btns;
        private Text m_lblDiamond;
        private Text m_lblDianquan;
        private Text m_lblGlodCoin;
        private CUIFormScript m_LobbyForm;
        private Text m_ping;
        private Text m_PlayerName;
        private Image m_PvpExpImg;
        private Text m_PvpExpTxt;
        private Text m_PvpLevel;
        private GameObject m_QQbuluBtn;
        private CUIFormScript m_RankingBtnForm;
        private RankingSystem.RankingSubView m_rankingType = RankingSystem.RankingSubView.Friend;
        private GameObject m_SysEntry;
        private CUIFormScript m_SysEntryForm;
        private GameObject m_textMianliu;
        private GameObject m_wifiIcon;
        public int m_wifiIconCheckMaxTicks = 6;
        public int m_wifiIconCheckTicks = -1;
        private GameObject m_wifiInfo;
        private int myRankingNo = -1;
        public bool NeedRelogin;
        public static string Pvp_BtnRes_PATH = (CUIUtility.s_Sprite_System_Lobby_Dir + "PvpBtnDynamic.prefab");
        private ListView<COMDT_FRIEND_INFO> rankFriendList;
        public static string RANKING_BTN_FORM_PATH = "UGUI/Form/System/Lobby/Form_Lobby_RankingBtn.prefab";
        public static uint s_CoinShowMaxValue = 0xf1b30;
        public static uint s_CoinShowStepValue = 0x2710;
        public static string[] s_netStateName = new string[] { "Net_1", "Net_2", "Net_3" };
        public static string s_noNetStateName = "NoNet";
        public static Color[] s_WifiStateColor = new Color[] { Color.get_red(), Color.get_yellow(), Color.get_green() };
        public static string[] s_wifiStateName = new string[] { "Wifi_1", "Wifi_2", "Wifi_3" };
        private GameObject social_btn;
        private GameObject symbol_btn;
        public static string SYSENTRY_FORM_PATH = "UGUI/Form/System/Lobby/Form_Lobby_SysTray.prefab";
        private GameObject task_btn;

        public void AddRedDot(enSysEntryID sysEntryId, enRedDotPos redDotPos = 2, int count = 0)
        {
            if (this.m_Btns != null)
            {
                GameObject obj2;
                this.m_Btns.TryGetValue((int) sysEntryId, out obj2);
                CUICommonSystem.AddRedDot(obj2, redDotPos, count);
            }
        }

        public void AddRedDotEx(enSysEntryID sysEntryId, enRedDotPos redDotPos = 2, int alertNum = 0)
        {
            if (this.m_Btns != null)
            {
                GameObject obj2;
                this.m_Btns.TryGetValue((int) sysEntryId, out obj2);
                CUICommonSystem.AddRedDot(obj2, redDotPos, alertNum);
            }
        }

        private void AutoPopup1_IDIP()
        {
            if (MonoSingleton<IDIPSys>.GetInstance().RedPotState)
            {
                Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.IDIP_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnIDIPClose));
                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.IDIP_OpenForm);
            }
            else
            {
                this.AutoPopup2_Activity();
            }
        }

        private void AutoPopup2_Activity()
        {
            if (Singleton<ActivitySys>.GetInstance().CheckReadyForDot(RES_WEAL_ENTRANCE_TYPE.RES_WEAL_ENTRANCE_TYPE_ACTIVITY))
            {
                Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Activity_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnActivityClose));
                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Activity_OpenForm);
            }
        }

        public void Check_Enable(RES_SPECIALFUNCUNLOCK_TYPE type)
        {
            bool bShow = Singleton<CFunctionUnlockSys>.instance.TipsHasShow(type);
            if (!bShow && !Singleton<CFunctionUnlockSys>.instance.IsTypeHasCondition(type))
            {
                bShow = true;
            }
            this.SetEnable(type, bShow);
        }

        private void CheckGotoWebEntryRedDot()
        {
            if (CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Lobby_GongLueEntry) && (this.m_LobbyForm != null))
            {
                Transform transform = this.m_LobbyForm.get_transform().Find("Popup/SignBtn");
                if (transform != null)
                {
                    CUIRedDotSystem.AddRedDot(transform.get_gameObject(), enRedDotPos.enTopRight, 0);
                }
            }
            if (CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Lobby_BattleHomeWeb) && (this.m_LobbyForm != null))
            {
                Transform transform2 = this.m_LobbyForm.get_transform().Find("Popup/BattleWebHome");
                if (transform2 != null)
                {
                    CUIRedDotSystem.AddRedDot(transform2.get_gameObject(), enRedDotPos.enTopRight, 0);
                }
            }
        }

        private bool checkIsHaveRedDot()
        {
            DictionaryView<int, GameObject>.Enumerator enumerator = this.m_Btns.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<int, GameObject> current = enumerator.Current;
                if (CUICommonSystem.IsHaveRedDot(current.Value))
                {
                    return true;
                }
            }
            return false;
        }

        public void CheckMianLiu()
        {
            if (this.m_textMianliu != null)
            {
                if (MonoSingleton<CTongCaiSys>.instance.IsLianTongIp() && MonoSingleton<CTongCaiSys>.instance.IsCanUseTongCai())
                {
                    this.m_textMianliu.CustomSetActive(true);
                }
                else
                {
                    this.m_textMianliu.CustomSetActive(false);
                }
            }
        }

        private void CheckNewbieIntro(int timerSeq)
        {
            if (!this.PopupNewbieIntro() && !_autoPoped)
            {
                _autoPoped = true;
            }
        }

        public void CheckWifi()
        {
            if (((this.m_wifiIcon != null) && (this.m_wifiInfo != null)) && (this.m_ping != null))
            {
                int lobbyPing = (int) Singleton<NetworkModule>.GetInstance().lobbyPing;
                lobbyPing = (lobbyPing <= 100) ? lobbyPing : ((((lobbyPing - 100) * 7) / 10) + 100);
                lobbyPing = Mathf.Clamp(lobbyPing, 0, 460);
                uint index = 0;
                if (lobbyPing < 100)
                {
                    index = 2;
                }
                else if (lobbyPing < 200)
                {
                    index = 1;
                }
                else
                {
                    index = 0;
                }
                if ((this.m_wifiIconCheckTicks == -1) || (this.m_wifiIconCheckTicks >= this.m_wifiIconCheckMaxTicks))
                {
                    NetworkReachability reachability = Application.get_internetReachability();
                    if (reachability == null)
                    {
                        CUICommonSystem.PlayAnimator(this.m_wifiIcon, s_noNetStateName);
                    }
                    else if (reachability == 2)
                    {
                        CUICommonSystem.PlayAnimator(this.m_wifiIcon, s_wifiStateName[index]);
                    }
                    else if (reachability == 1)
                    {
                        CUICommonSystem.PlayAnimator(this.m_wifiIcon, s_netStateName[index]);
                    }
                    this.m_wifiIconCheckTicks = 0;
                }
                else
                {
                    this.m_wifiIconCheckTicks++;
                }
                if ((this.m_wifiInfo != null) && this.m_wifiInfo.get_activeInHierarchy())
                {
                    this.m_ping.set_text(lobbyPing + "ms");
                }
            }
        }

        public void Clear()
        {
            this.m_rankingType = RankingSystem.RankingSubView.Friend;
        }

        public void DelRedDot(enSysEntryID sysEntryId)
        {
            if (this.m_Btns != null)
            {
                GameObject obj2;
                this.m_Btns.TryGetValue((int) sysEntryId, out obj2);
                CUICommonSystem.DelRedDot(obj2);
            }
        }

        public void FullShow()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(SYSENTRY_FORM_PATH);
            if (form != null)
            {
                Transform transform = form.get_transform();
                transform.Find("PlayerBtn/MailBtn").get_gameObject().CustomSetActive(true);
                transform.Find("PlayerBtn/SettingBtn").get_gameObject().CustomSetActive(true);
                transform.Find("PlayerBtn/FriendBtn").get_gameObject().CustomSetActive(true);
            }
        }

        public string GetCoinString(uint coinValue)
        {
            string str = coinValue.ToString();
            if (coinValue > s_CoinShowMaxValue)
            {
                int num = (int) (coinValue / s_CoinShowStepValue);
                str = string.Format("{0}万", num);
            }
            return str;
        }

        public override void Init()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Lobby_OpenLobbyForm, new CUIEventManager.OnUIEventHandler(this.onOpenLobby));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.WEB_OpenURL, new CUIEventManager.OnUIEventHandler(this.onOpenWeb));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Common_WifiCheckTimer, new CUIEventManager.OnUIEventHandler(this.onCommon_WifiCheckTimer));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Common_ShowOrHideWifiInfo, new CUIEventManager.OnUIEventHandler(this.onCommon_ShowOrHideWifiInfo));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Lobby_ConfirmErrExit, new CUIEventManager.OnUIEventHandler(this.onErrorExit));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Lobby_LobbyFormShow, new CUIEventManager.OnUIEventHandler(this.Lobby_LobbyFormShow));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Lobby_LobbyFormHide, new CUIEventManager.OnUIEventHandler(this.Lobby_LobbyFormHide));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Lobby_Close, new CUIEventManager.OnUIEventHandler(this.OnCloseForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Lobby_PrepareFight_Sub, new CUIEventManager.OnUIEventHandler(this.OnPrepareFight_Sub));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Lobby_PrepareFight_Origin, new CUIEventManager.OnUIEventHandler(this.OnPrepareFight_Origin));
            Singleton<EventRouter>.instance.AddEventHandler("MasterAttributesChanged", new Action(this, (IntPtr) this.UpdatePlayerData));
            Singleton<EventRouter>.instance.AddEventHandler("TaskUpdated", new Action(this, (IntPtr) this.OnTaskUpdate));
            Singleton<EventRouter>.instance.AddEventHandler("Friend_LobbyIconRedDot_Refresh", new Action(this, (IntPtr) this.OnFriendSysIconUpdate));
            Singleton<EventRouter>.instance.AddEventHandler(EventID.Mall_Entry_Add_RedDotCheck, new Action(this, (IntPtr) this.OnCheckRedDotByServerVersionWithLobby));
            Singleton<EventRouter>.instance.AddEventHandler(EventID.Mall_Entry_Del_RedDotCheck, new Action(this, (IntPtr) this.OnCheckDelMallEntryRedDot));
            Singleton<EventRouter>.instance.AddEventHandler("MailUnReadNumUpdate", new Action(this, (IntPtr) this.OnMailUnReadUpdate));
            Singleton<EventRouter>.instance.AddEventHandler(EventID.ACHIEVE_TROPHY_REWARD_INFO_STATE_CHANGE, new Action(this, (IntPtr) this.OnAchieveStateUpdate));
            Singleton<EventRouter>.instance.AddEventHandler(EventID.SymbolEquipSuc, new Action(this, (IntPtr) this.OnCheckSymbolEquipAlert));
            Singleton<EventRouter>.instance.AddEventHandler(EventID.BAG_ITEMS_UPDATE, new Action(this, (IntPtr) this.OnBagItemsUpdate));
            Singleton<EventRouter>.instance.AddEventHandler("MasterPvpLevelChanged", new Action(this, (IntPtr) this.OnCheckSymbolEquipAlert));
            Singleton<EventRouter>.instance.AddEventHandler<bool>("Guild_Sign_State_Changed", new Action<bool>(this.OnGuildSignStateChanged));
            Singleton<ActivitySys>.GetInstance().OnStateChange += new ActivitySys.StateChangeDelegate(this.ValidateActivitySpot);
            Singleton<EventRouter>.instance.AddEventHandler("IDIPNOTICE_UNREAD_NUM_UPDATE", new Action(this, (IntPtr) this.ValidateActivitySpot));
            Singleton<EventRouter>.instance.AddEventHandler("MasterJifenChanged", new Action(this, (IntPtr) this.ValidateActivitySpot));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.IDIP_QQVIP_OpenWealForm, new CUIEventManager.OnUIEventHandler(this.OpenQQVIPWealForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.WEB_OpenHome, new CUIEventManager.OnUIEventHandler(this.OpenWebHome));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Lobby_UnlockAnimation_End, new CUIEventManager.OnUIEventHandler(this.On_Lobby_UnlockAnimation_End));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Lobby_MysteryShopClose, new CUIEventManager.OnUIEventHandler(this.On_Lobby_MysteryShopClose));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.GameCenter_OpenWXRight, new CUIEventManager.OnUIEventHandler(this.OpenWXGameCenterRightForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.GameCenter_OpenQQRight, new CUIEventManager.OnUIEventHandler(this.OpenQQGameCenterRightForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Lobby_RankingListElementEnable, new CUIEventManager.OnUIEventHandler(this.OnRankingListElementEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.GameCenter_OpenGuestRight, new CUIEventManager.OnUIEventHandler(this.OpenGuestGameCenterRightForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Lobby_GotoBattleWebHome, new CUIEventManager.OnUIEventHandler(this.OnGotoBattleWebHome));
            Singleton<EventRouter>.GetInstance().AddEventHandler("CheckNewbieIntro", new Action(this, (IntPtr) this.OnCheckNewbieIntro));
            Singleton<EventRouter>.GetInstance().AddEventHandler("VipInfoHadSet", new Action(this, (IntPtr) this.UpdateQQVIPState));
            Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.NOBE_STATE_CHANGE, new Action(this, (IntPtr) this.UpdateNobeIcon));
            Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.NOBE_STATE_HEAD_CHANGE, new Action(this, (IntPtr) this.UpdateNobeHeadIdx));
            Singleton<EventRouter>.GetInstance().AddEventHandler("MasterPvpLevelChanged", new Action(this, (IntPtr) this.OnPlayerLvlChange));
            Singleton<EventRouter>.GetInstance().AddEventHandler("Rank_Friend_List", new Action(this, (IntPtr) this.RefreshRankList));
            Singleton<EventRouter>.GetInstance().AddEventHandler<RankingSystem.RankingSubView>("Rank_List", new Action<RankingSystem.RankingSubView>(this.RefreshRankList));
            Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.NAMECHANGE_PLAYER_NAME_CHANGE, new Action(this, (IntPtr) this.OnPlayerNameChange));
            Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.HEAD_IMAGE_FLAG_CHANGE, new Action(this, (IntPtr) this.UpdatePlayerData));
            Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.LOBBY_PURE_LOBBY_SHOW, new Action(this, (IntPtr) this.OnPureLobbyShow));
            Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.GAMER_REDDOT_CHANGE, new Action(this, (IntPtr) this.UpdatePlayerData));
        }

        private void InitForm(CUIFormScript form)
        {
            Transform transform = form.get_transform();
            this.m_PlayerName = transform.Find("PlayerHead/NameGroup/PlayerName").GetComponent<Text>();
            this.m_PvpLevel = transform.Find("PlayerHead/pvpLevel").GetComponent<Text>();
            this.m_PvpExpImg = transform.Find("PlayerHead/pvpExp/expBg/imgExp").GetComponent<Image>();
            this.m_PvpExpTxt = transform.Find("PlayerHead/pvpExp/expBg/txtExp").GetComponent<Text>();
            this.hero_btn = transform.Find("LobbyBottom/SysEntry/HeroBtn").get_gameObject();
            this.symbol_btn = transform.Find("LobbyBottom/SysEntry/SymbolBtn").get_gameObject();
            this.bag_btn = transform.Find("LobbyBottom/SysEntry/BagBtn").get_gameObject();
            this.task_btn = transform.Find("LobbyBottom/Newbie").get_gameObject();
            this.social_btn = transform.Find("LobbyBottom/SysEntry/SocialBtn").get_gameObject();
            this.addSkill_btn = transform.Find("LobbyBottom/SysEntry/AddedSkillBtn").get_gameObject();
            this.achievement_btn = transform.Find("LobbyBottom/SysEntry/AchievementBtn").get_gameObject();
            this.Check_Enable(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_HERO);
            this.Check_Enable(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_SYMBOL);
            this.Check_Enable(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_BAG);
            this.Check_Enable(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_TASK);
            this.Check_Enable(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_UNION);
            this.Check_Enable(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_FRIEND);
            this.Check_Enable(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_ADDEDSKILL);
            this.Check_Enable(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_ACHIEVEMENT);
            Transform transform2 = transform.Find("BtnCon/PvpBtn");
            Transform transform3 = transform.Find("BtnCon/LadderBtn");
            if (transform2 != null)
            {
                CUICommonSystem.LoadUIPrefab(Pvp_BtnRes_PATH, "PvpBtnDynamic", transform2.get_gameObject(), form);
            }
            if (transform3 != null)
            {
                CUICommonSystem.LoadUIPrefab(Ladder_BtnRes_PATH, "LadderBtnDynamic", transform3.get_gameObject(), form);
            }
            if (CSysDynamicBlock.bLobbyEntryBlocked)
            {
                Transform transform4 = transform.Find("Popup");
                if (transform4 != null)
                {
                    transform4.get_gameObject().CustomSetActive(false);
                }
                Transform transform5 = transform.Find("BtnCon/CompetitionBtn");
                if (transform5 != null)
                {
                    transform5.get_gameObject().CustomSetActive(false);
                }
                if (this.task_btn != null)
                {
                    this.task_btn.CustomSetActive(false);
                }
                Transform transform6 = transform.Find("DiamondPayBtn");
                if (transform6 != null)
                {
                    transform6.get_gameObject().CustomSetActive(false);
                }
                Transform transform7 = transform.Find("Popup/BattleWebHome");
                if (transform7 != null)
                {
                    transform7.get_gameObject().CustomSetActive(false);
                }
            }
            Button component = transform.Find("BtnCon/LadderBtn").GetComponent<Button>();
            if (component != null)
            {
                component.set_interactable(Singleton<CLadderSystem>.GetInstance().IsLevelQualified());
                Transform transform8 = component.get_transform().Find("Lock");
                if (transform8 != null)
                {
                    transform8.get_gameObject().CustomSetActive(!component.get_interactable());
                    transform8.SetAsLastSibling();
                }
            }
            Button button2 = transform.FindChild("BtnCon/UnionBtn").GetComponent<Button>();
            if (button2 != null)
            {
                bool bActive = Singleton<CUnionBattleEntrySystem>.GetInstance().IsUnionFuncLocked();
                button2.set_interactable(!bActive);
                button2.get_transform().FindChild("Lock").get_gameObject().CustomSetActive(bActive);
            }
            GameObject obj3 = transform.Find("PlayerHead/pvpExp/expEventPanel").get_gameObject();
            if (obj3 != null)
            {
                CUIEventScript script = obj3.GetComponent<CUIEventScript>();
                if (script == null)
                {
                    script = obj3.AddComponent<CUIEventScript>();
                    script.Initialize(form);
                }
                CUseable useable = CUseableManager.CreateVirtualUseable(enVirtualItemType.enExp, 0);
                stUIEventParams eventParams = new stUIEventParams();
                eventParams.iconUseable = useable;
                eventParams.tag = 3;
                script.SetUIEvent(enUIEventType.Down, enUIEventID.Tips_ItemInfoOpen, eventParams);
                script.SetUIEvent(enUIEventType.HoldEnd, enUIEventID.Tips_ItemInfoClose, eventParams);
                script.SetUIEvent(enUIEventType.Click, enUIEventID.Tips_ItemInfoClose, eventParams);
                script.SetUIEvent(enUIEventType.DragEnd, enUIEventID.Tips_ItemInfoClose, eventParams);
            }
            RefreshDianQuanPayButton(false);
            GameObject widget = form.GetWidget(7);
            if (CSysDynamicBlock.bLobbyEntryBlocked)
            {
                widget.CustomSetActive(false);
            }
            else
            {
                Text text = form.GetWidget(8).GetComponent<Text>();
                if (GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0xcc).dwConfValue > 0)
                {
                    widget.CustomSetActive(IsPlatChannelOpen);
                    text.set_text(Singleton<CTextManager>.GetInstance().GetText("CrossPlat_Plat_Channel_Open_Lobby_Msg"));
                }
                else
                {
                    widget.CustomSetActive(!IsPlatChannelOpen);
                    text.set_text(Singleton<CTextManager>.GetInstance().GetText("CrossPlat_Plat_Channel_Not_Open_Lobby_Msg"));
                }
            }
        }

        private void InitOther(CUIFormScript m_FormScript)
        {
            Singleton<CTimerManager>.GetInstance().AddTimer(50, 1, new CTimer.OnTimeUpHandler(this.CheckNewbieIntro));
            this.ProcessQQVIP(m_FormScript);
            this.UpdateGameCenterState(m_FormScript);
            MonoSingleton<NobeSys>.GetInstance().ShowDelayNobeTipsInfo();
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if ((masterRoleInfo != null) && (masterRoleInfo.m_licenseInfo != null))
            {
                masterRoleInfo.m_licenseInfo.ReviewLicenseList();
            }
        }

        private void InitRankingBtnForm()
        {
            if (this.m_RankingBtnForm == null)
            {
                DebugHelper.Assert(false, "m_RankingBtnForm cannot be null!!!");
            }
            else
            {
                this._rankingBtn = this.m_RankingBtnForm.GetWidget(0);
                if ((this._rankingBtn != null) && CSysDynamicBlock.bSocialBlocked)
                {
                    this._rankingBtn.CustomSetActive(false);
                }
                this.RefreshRankList();
            }
        }

        private void InitSysEntryForm(CUIFormScript form)
        {
            Transform transform = form.get_gameObject().get_transform();
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            this.m_lblGlodCoin = transform.FindChild("PlayerBtn/GoldCoin/Text").GetComponent<Text>();
            this.m_lblDianquan = transform.FindChild("PlayerBtn/Dianquan/Text").GetComponent<Text>();
            this.m_lblDiamond = transform.FindChild("PlayerBtn/Diamond/Text").GetComponent<Text>();
            this.m_wifiIcon = form.GetWidget(0);
            this.m_wifiInfo = form.GetWidget(1);
            this.m_ping = form.GetWidget(2).GetComponent<Text>();
            this.m_textMianliu = form.GetWidget(9);
            this.m_lblGlodCoin.set_text(this.GetCoinString(masterRoleInfo.GoldCoin));
            this.m_lblDianquan.set_text(this.GetCoinString((uint) masterRoleInfo.DianQuan));
            this.m_lblDiamond.set_text(this.GetCoinString(masterRoleInfo.Diamond));
            GameObject obj2 = transform.Find("PlayerBtn/GoldCoin").get_gameObject();
            if (obj2 != null)
            {
                CUIEventScript component = obj2.GetComponent<CUIEventScript>();
                if (component == null)
                {
                    component = obj2.AddComponent<CUIEventScript>();
                    component.Initialize(form);
                }
                CUseable useable = CUseableManager.CreateCoinUseable(RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_PVPCOIN, (int) masterRoleInfo.GoldCoin);
                stUIEventParams eventParams = new stUIEventParams();
                eventParams.iconUseable = useable;
                eventParams.tag = 3;
                component.SetUIEvent(enUIEventType.Down, enUIEventID.Tips_ItemInfoOpen, eventParams);
                component.SetUIEvent(enUIEventType.HoldEnd, enUIEventID.Tips_ItemInfoClose, eventParams);
                component.SetUIEvent(enUIEventType.Click, enUIEventID.Tips_ItemInfoClose, eventParams);
                component.SetUIEvent(enUIEventType.DragEnd, enUIEventID.Tips_ItemInfoClose, eventParams);
            }
            GameObject obj3 = transform.Find("PlayerBtn/Diamond").get_gameObject();
            if (obj3 != null)
            {
                CUIEventScript script2 = obj3.GetComponent<CUIEventScript>();
                if (script2 == null)
                {
                    script2 = obj3.AddComponent<CUIEventScript>();
                    script2.Initialize(form);
                }
                CUseable useable2 = CUseableManager.CreateCoinUseable(RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_DIAMOND, (int) masterRoleInfo.Diamond);
                stUIEventParams params2 = new stUIEventParams();
                params2.iconUseable = useable2;
                params2.tag = 3;
                script2.SetUIEvent(enUIEventType.Down, enUIEventID.Tips_ItemInfoOpen, params2);
                script2.SetUIEvent(enUIEventType.HoldEnd, enUIEventID.Tips_ItemInfoClose, params2);
                script2.SetUIEvent(enUIEventType.Click, enUIEventID.Tips_ItemInfoClose, params2);
                script2.SetUIEvent(enUIEventType.DragEnd, enUIEventID.Tips_ItemInfoClose, params2);
            }
            if (!ApolloConfig.payEnabled)
            {
                Transform transform2 = transform.Find("PlayerBtn/Dianquan/Button");
                if (transform2 != null)
                {
                    transform2.get_gameObject().CustomSetActive(false);
                }
            }
            this.m_SysEntry = this.m_LobbyForm.get_gameObject().get_transform().Find("LobbyBottom/SysEntry").get_gameObject();
            this.m_Btns = new DictionaryView<int, GameObject>();
            this.m_Btns.Add(0, this.m_SysEntry.get_transform().Find("HeroBtn").get_gameObject());
            this.m_Btns.Add(1, this.m_SysEntry.get_transform().Find("SymbolBtn").get_gameObject());
            this.m_Btns.Add(2, this.m_SysEntry.get_transform().Find("AchievementBtn").get_gameObject());
            this.m_Btns.Add(3, this.m_SysEntry.get_transform().Find("BagBtn").get_gameObject());
            this.m_Btns.Add(5, this.m_SysEntry.get_transform().Find("SocialBtn").get_gameObject());
            this.m_Btns.Add(6, form.get_transform().Find("PlayerBtn/FriendBtn").get_gameObject());
            this.m_Btns.Add(7, this.m_SysEntry.get_transform().Find("AddedSkillBtn").get_gameObject());
            this.m_Btns.Add(8, form.get_transform().Find("PlayerBtn/MailBtn").get_gameObject());
            this.m_Btns.Add(9, Utility.FindChild(this.m_LobbyForm.get_gameObject(), "Popup/ActBtn"));
            this.m_Btns.Add(10, Utility.FindChild(this.m_LobbyForm.get_gameObject(), "Popup/BoardBtn"));
            this.m_Btns.Add(4, this.m_LobbyForm.get_gameObject().get_transform().Find("LobbyBottom/Newbie/RedDotPanel").get_gameObject());
        }

        public bool IsInLobbyForm()
        {
            return this.m_bInLobby;
        }

        private void Lobby_LobbyFormHide(CUIEvent uiEvent)
        {
            if (this.m_bInLobby)
            {
                this.MiniShow();
            }
        }

        private void Lobby_LobbyFormShow(CUIEvent uiEvent)
        {
            if (this.m_bInLobby)
            {
                this.FullShow();
                CUICommonSystem.CloseCommonTips();
                CUICommonSystem.CloseUseableTips();
                Singleton<CMiShuSystem>.instance.CheckActPlayModeTipsForLobby();
                Singleton<CChatController>.instance.ShowPanel(true, false);
                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Lobby_PrepareFight_Sub);
                if (!string.IsNullOrEmpty(Singleton<CFriendContoller>.instance.IntimacyUpInfo))
                {
                    Singleton<CUIManager>.GetInstance().OpenTips(string.Format(UT.GetText("Intimacy_UpInfo"), Singleton<CFriendContoller>.instance.IntimacyUpInfo, Singleton<CFriendContoller>.instance.IntimacyUpValue), false, 1.5f, null, new object[0]);
                    Singleton<CFriendContoller>.instance.IntimacyUpInfo = string.Empty;
                    Singleton<CFriendContoller>.instance.IntimacyUpValue = 0;
                }
                if (!MonoSingleton<NewbieGuideManager>.GetInstance().isNewbieGuiding)
                {
                    Singleton<CAchievementSystem>.GetInstance().ProcessMostRecentlyDoneAchievements(false);
                    Singleton<CAchievementSystem>.GetInstance().ProcessPvpBanMsg();
                }
                this.OnFriendSysIconUpdate();
            }
        }

        public void MiniShow()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(SYSENTRY_FORM_PATH);
            if (form != null)
            {
                Transform transform = form.get_transform();
                transform.Find("PlayerBtn/MailBtn").get_gameObject().CustomSetActive(false);
                transform.Find("PlayerBtn/SettingBtn").get_gameObject().CustomSetActive(false);
                transform.Find("PlayerBtn/FriendBtn").get_gameObject().CustomSetActive(false);
            }
        }

        private void On_Lobby_MysteryShopClose(CUIEvent uiEvent)
        {
            GameObject obj2 = Utility.FindChild(uiEvent.m_srcFormScript.get_gameObject(), "Popup/BoardBtn/MysteryShop");
            Debug.LogWarning(string.Format("mystery shop icon on close:{0}", obj2));
            obj2.CustomSetActive(false);
        }

        private void On_Lobby_UnlockAnimation_End(CUIEvent uievent)
        {
            Singleton<CUIManager>.instance.CloseForm(LOBBY_FUN_UNLOCK_PATH);
            Singleton<CSoundManager>.instance.PostEvent("UI_hall_system_back", null);
            this.SetEnable((RES_SPECIALFUNCUNLOCK_TYPE) uievent.m_eventParams.tag, true);
        }

        private void OnAchieveStateUpdate()
        {
            if (CAchieveInfo2.GetMasterAchieveInfo().HasRewardNotGot())
            {
                this.AddRedDot(enSysEntryID.AchievementBtn, enRedDotPos.enTopRight, 0);
            }
            else
            {
                this.DelRedDot(enSysEntryID.AchievementBtn);
            }
        }

        private void OnActivityClose(CUIEvent uiEvt)
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Activity_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnActivityClose));
        }

        private void OnBagItemsUpdate()
        {
            this.ValidateActivitySpot();
            this.OnCheckSymbolEquipAlert();
        }

        private void OnCheckDelMallEntryRedDot()
        {
            if ((((!CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Mall_HeroTab) && !CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Mall_HeroSkinTab)) && (!CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Mall_SymbolTab) && !CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Mall_SaleTab))) && ((!CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Mall_LotteryTab) && !CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Mall_RecommendTab)) && (!CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Mall_MysteryTab) || !CUIRedDotSystem.IsShowRedDotByLogic(enRedID.Mall_MysteryTab)))) && (!CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Mall_BoutiqueTab) && !CUIRedDotSystem.IsShowRedDotByLogic(enRedID.Mall_SymbolTab)))
            {
                this.DelRedDot(enSysEntryID.MallBtn);
            }
        }

        private void OnCheckNewbieIntro()
        {
            Singleton<CTimerManager>.GetInstance().AddTimer(100, 1, delegate (int seq) {
                this.PopupNewbieIntro();
            });
        }

        private void OnCheckRedDotByServerVersionWithLobby()
        {
            if ((((CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Mall_HeroTab) || CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Mall_HeroSkinTab)) || (CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Mall_SymbolTab) || CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Mall_SaleTab))) || ((CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Mall_LotteryTab) || CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Mall_RecommendTab)) || (CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Mall_MysteryTab) && CUIRedDotSystem.IsShowRedDotByLogic(enRedID.Mall_MysteryTab)))) || (CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Mall_BoutiqueTab) || CUIRedDotSystem.IsShowRedDotByLogic(enRedID.Mall_SymbolTab)))
            {
                this.AddRedDot(enSysEntryID.MallBtn, enRedDotPos.enTopRight, 0);
            }
            this.CheckGotoWebEntryRedDot();
        }

        public void OnCheckSymbolEquipAlert()
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                int num;
                uint num2;
                if (masterRoleInfo.m_symbolInfo.CheckAnyWearSymbol(out num, out num2, 0))
                {
                    this.AddRedDot(enSysEntryID.SymbolBtn, enRedDotPos.enTopRight, 0);
                }
                else
                {
                    this.DelRedDot(enSysEntryID.SymbolBtn);
                }
            }
        }

        private void OnCheckUpdateClientVersion()
        {
            if (Singleton<LobbyLogic>.instance.NeedUpdateClient)
            {
                Singleton<LobbyLogic>.instance.NeedUpdateClient = false;
                Singleton<CUIManager>.GetInstance().OpenMessageBox(Singleton<CTextManager>.GetInstance().GetText("VersionIsLow"), enUIEventID.None, false);
            }
        }

        protected void OnCloseForm(CUIEvent uiEvt)
        {
            this.m_bInLobby = false;
            this.m_LobbyForm = null;
            this.m_SysEntryForm = null;
            this.m_RankingBtnForm = null;
            this.m_PlayerName = null;
            this.m_PvpLevel = null;
            this.m_PvpExpImg = null;
            this.m_PvpExpTxt = null;
            this.hero_btn = null;
            this.symbol_btn = null;
            this.bag_btn = null;
            this.task_btn = null;
            this.social_btn = null;
            this.addSkill_btn = null;
            this.achievement_btn = null;
            Singleton<CUIManager>.GetInstance().CloseForm(SYSENTRY_FORM_PATH);
            Singleton<CUIManager>.GetInstance().CloseForm(RANKING_BTN_FORM_PATH);
            this.UnInitWidget();
        }

        private void onCommon_ShowOrHideWifiInfo(CUIEvent uiEvent)
        {
            if (this.m_bInLobby)
            {
                this.ShowOrHideWifiInfo();
            }
        }

        private void onCommon_WifiCheckTimer(CUIEvent uiEvent)
        {
            if (this.m_bInLobby)
            {
                this.CheckWifi();
                this.CheckMianLiu();
            }
        }

        private void onErrorExit(CUIEvent uiEvent)
        {
            SGameApplication.Quit();
        }

        private void OnFriendSysIconUpdate()
        {
            int dataCount = Singleton<CFriendContoller>.GetInstance().model.GetDataCount(CFriendModel.FriendType.RequestFriend);
            int num2 = Singleton<CFriendContoller>.GetInstance().model.GetDataCount(CFriendModel.FriendType.MentorRequestList);
            bool flag = Singleton<CFriendContoller>.GetInstance().model.FRData.HasRedDot();
            bool flag2 = Singleton<CTaskSys>.instance.IsMentorTaskRedDot();
            if (((dataCount > 0) || (num2 > 0)) || (flag || flag2))
            {
                this.AddRedDot(enSysEntryID.FriendBtn, enRedDotPos.enTopRight, 0);
            }
            else
            {
                this.DelRedDot(enSysEntryID.FriendBtn);
            }
        }

        private void OnGotoBattleWebHome(CUIEvent uiEvent)
        {
            ulong playerUllUID = 0L;
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                playerUllUID = masterRoleInfo.playerUllUID;
            }
            string platformArea = CUICommonSystem.GetPlatformArea();
            string text = Singleton<CTextManager>.instance.GetText("HttpUrl_BattleWebHome");
            CUICommonSystem.OpenUrl(string.Concat(new object[] { text, "?partition=", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID, "&roleid=", playerUllUID, "&area=", platformArea }), true, 0);
            Singleton<CUINewFlagSystem>.instance.SetNewFlagForMatch(false);
            if (CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Lobby_BattleHomeWeb))
            {
                CUIRedDotSystem.SetRedDotViewByVersion(enRedID.Lobby_BattleHomeWeb);
                if (this.m_LobbyForm != null)
                {
                    Transform transform = this.m_LobbyForm.get_transform().Find("Popup/BattleWebHome");
                    if (transform != null)
                    {
                        CUIRedDotSystem.DelRedDot(transform.get_gameObject());
                    }
                }
            }
        }

        private void OnGuildSignStateChanged(bool isSigned)
        {
            if (isSigned)
            {
                if (!Singleton<CGuildMatchSystem>.GetInstance().IsInGuildMatchTime() || Singleton<CGuildMatchSystem>.GetInstance().m_isGuildMatchBtnClicked)
                {
                    this.DelRedDot(enSysEntryID.GuildBtn);
                }
            }
            else
            {
                this.AddRedDot(enSysEntryID.GuildBtn, enRedDotPos.enTopRight, 0);
            }
        }

        private void OnIDIPClose(CUIEvent uiEvt)
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.IDIP_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnIDIPClose));
            this.AutoPopup2_Activity();
        }

        private void OnMailUnReadUpdate()
        {
            int unReadMailCount = Singleton<CMailSys>.instance.GetUnReadMailCount(true);
            if (this.m_LobbyForm != null)
            {
                if (unReadMailCount > 0)
                {
                    this.AddRedDot(enSysEntryID.MailBtn, enRedDotPos.enTopRight, 0);
                }
                else
                {
                    this.DelRedDot(enSysEntryID.MailBtn);
                }
            }
        }

        private void onOpenLobby(CUIEvent uiEvent)
        {
            this.m_LobbyForm = Singleton<CUIManager>.GetInstance().OpenForm(LOBBY_FORM_PATH, false, true);
            this.m_SysEntryForm = Singleton<CUIManager>.GetInstance().OpenForm(SYSENTRY_FORM_PATH, false, true);
            this.m_RankingBtnForm = Singleton<CUIManager>.GetInstance().OpenForm(RANKING_BTN_FORM_PATH, false, true);
            this.m_bInLobby = true;
            this.InitForm(this.m_LobbyForm);
            this.InitRankingBtnForm();
            this.InitOther(this.m_LobbyForm);
            this.InitSysEntryForm(this.m_SysEntryForm);
            this.UpdatePlayerData();
            this.OnFriendSysIconUpdate();
            this.OnTaskUpdate();
            this.ValidateActivitySpot();
            this.OnMailUnReadUpdate();
            this.OnCheckSymbolEquipAlert();
            this.OnCheckUpdateClientVersion();
            Singleton<EventRouter>.instance.BroadCastEvent(EventID.Mall_Entry_Add_RedDotCheck);
            Singleton<EventRouter>.instance.BroadCastEvent(EventID.Mall_Set_Free_Draw_Timer);
            Singleton<CMiShuSystem>.instance.CheckMiShuTalk(true);
            Singleton<CMiShuSystem>.instance.OnCheckFirstWin(null);
            Singleton<CMiShuSystem>.instance.CheckActPlayModeTipsForLobby();
            Singleton<CUINewFlagSystem>.instance.ShowNewFlagForBeizhanEntry();
            Singleton<CUINewFlagSystem>.instance.ShowNewFlagForMishuEntry();
            Singleton<CUINewFlagSystem>.instance.SetNewFlagFormCustomEquipShow(true);
            Singleton<CUINewFlagSystem>.instance.SetNewFlagForFriendEntry(true);
            Singleton<CUINewFlagSystem>.instance.SetNewFlagForMessageBtnEntry(true);
            Singleton<CUINewFlagSystem>.instance.SetNewFlagForOBBtn(true);
            Singleton<CUINewFlagSystem>.instance.SetNewFlagForMatch(true);
            Singleton<CUINewFlagSystem>.instance.SetNewFlagForSettingEntry(true);
            Singleton<CUINewFlagSystem>.instance.ShowNewFlagForAchievementEntry();
            if (Singleton<CLoginSystem>.GetInstance().m_fLoginBeginTime > 0f)
            {
                List<KeyValuePair<string, string>> events = new List<KeyValuePair<string, string>>();
                events.Add(new KeyValuePair<string, string>("g_version", CVersion.GetAppVersion()));
                events.Add(new KeyValuePair<string, string>("WorldID", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString()));
                events.Add(new KeyValuePair<string, string>("platform", Singleton<ApolloHelper>.GetInstance().CurPlatform.ToString()));
                events.Add(new KeyValuePair<string, string>("openid", "NULL"));
                float num = Time.get_time() - Singleton<CLoginSystem>.GetInstance().m_fLoginBeginTime;
                events.Add(new KeyValuePair<string, string>("totaltime", num.ToString()));
                events.Add(new KeyValuePair<string, string>("errorCode", "0"));
                events.Add(new KeyValuePair<string, string>("error_msg", "0"));
                Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Service_Login_InLobby", events, true);
                Singleton<CLoginSystem>.GetInstance().m_fLoginBeginTime = 0f;
            }
            if (this.NeedRelogin)
            {
                this.NeedRelogin = false;
                LobbyMsgHandler.PopupRelogin();
            }
        }

        private void onOpenWeb(CUIEvent uiEvent)
        {
            string strUrl = "http://www.qq.com";
            CUICommonSystem.OpenUrl(strUrl, true, 0);
        }

        private void OnPlayerLvlChange()
        {
            if (this.m_LobbyForm != null)
            {
                Transform transform = this.m_LobbyForm.get_transform();
                Button component = transform.Find("BtnCon/LadderBtn").GetComponent<Button>();
                if (component != null)
                {
                    component.set_interactable(Singleton<CLadderSystem>.GetInstance().IsLevelQualified());
                    Transform transform2 = component.get_transform().Find("Lock");
                    if (transform2 != null)
                    {
                        transform2.get_gameObject().CustomSetActive(!component.get_interactable());
                        transform2.SetAsLastSibling();
                    }
                }
                Button button2 = transform.FindChild("BtnCon/UnionBtn").GetComponent<Button>();
                if (button2 != null)
                {
                    bool bActive = Singleton<CUnionBattleEntrySystem>.GetInstance().IsUnionFuncLocked();
                    button2.set_interactable(!bActive);
                    button2.get_transform().FindChild("Lock").get_gameObject().CustomSetActive(bActive);
                }
            }
        }

        private void OnPlayerNameChange()
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if ((this.m_PlayerName != null) && (masterRoleInfo != null))
            {
                this.m_PlayerName.set_text(masterRoleInfo.Name);
            }
        }

        protected void OnPrepareFight_Origin(CUIEvent uiEvt)
        {
            Transform transform = this.m_LobbyForm.get_transform().Find("LobbyBottom/SysEntry/ChatBtn_sub");
            Transform transform2 = this.m_LobbyForm.get_transform().Find("LobbyBottom/SysEntry/ChatBtn");
            if (transform != null)
            {
                transform.get_gameObject().CustomSetActive(true);
            }
            if (transform2 != null)
            {
                transform2.get_gameObject().CustomSetActive(false);
            }
            Singleton<CUINewFlagSystem>.instance.HideNewFlagForBeizhanEntry();
        }

        protected void OnPrepareFight_Sub(CUIEvent uiEvt)
        {
            Transform transform = this.m_LobbyForm.get_transform().Find("LobbyBottom/SysEntry/ChatBtn_sub");
            Transform transform2 = this.m_LobbyForm.get_transform().Find("LobbyBottom/SysEntry/ChatBtn");
            if (transform != null)
            {
                transform.get_gameObject().CustomSetActive(false);
            }
            if (transform2 != null)
            {
                transform2.get_gameObject().CustomSetActive(true);
            }
        }

        private void OnPureLobbyShow()
        {
            if (Singleton<CGuildSystem>.GetInstance().IsInNormalGuild())
            {
                if (Singleton<CGuildMatchSystem>.GetInstance().IsInGuildMatchTime() && !Singleton<CGuildMatchSystem>.GetInstance().m_isGuildMatchBtnClicked)
                {
                    this.AddRedDot(enSysEntryID.GuildBtn, enRedDotPos.enTopRight, 0);
                }
                else if (CGuildHelper.IsPlayerSigned())
                {
                    this.DelRedDot(enSysEntryID.GuildBtn);
                }
            }
        }

        private void OnRankingListElementEnable(CUIEvent uiEvent)
        {
            int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
            GameObject srcWidget = uiEvent.m_srcWidget;
            if (this.m_rankingType == RankingSystem.RankingSubView.Friend)
            {
                this.OnUpdateRankingFriendElement(srcWidget, srcWidgetIndexInBelongedList);
            }
            else if (this.m_rankingType == RankingSystem.RankingSubView.All)
            {
                this.OnUpdateRankingAllElement(srcWidget, srcWidgetIndexInBelongedList);
            }
        }

        private void OnTaskUpdate()
        {
            CTaskModel model = Singleton<CTaskSys>.instance.model;
            model.task_Data.Sort(RES_TASK_TYPE.RES_TASKTYPE_HEROWAKE);
            model.task_Data.Sort(RES_TASK_TYPE.RES_TASKTYPE_USUAL);
            model.task_Data.Sort(RES_TASK_TYPE.RES_TASKTYPE_MAIN);
            int count = Singleton<CTaskSys>.instance.model.GetMainTask_RedDotCount() + Singleton<CTaskSys>.instance.model.task_Data.GetTask_Count(enTaskTab.TAB_USUAL, CTask.State.Have_Done);
            if (count > 0)
            {
                this.AddRedDot(enSysEntryID.TaskBtn, enRedDotPos.enTopRight, count);
            }
            else
            {
                this.DelRedDot(enSysEntryID.TaskBtn);
            }
        }

        private void OnUpdateRankingAllElement(GameObject go, int index)
        {
            CSDT_RANKING_LIST_SUCC rankList = Singleton<RankingSystem>.instance.GetRankList(RankingSystem.RankingType.Ladder);
            if (((rankList != null) && (go != null)) && (index < rankList.astItemDetail.Length))
            {
                string serverUrl = string.Empty;
                Transform transform = null;
                Transform transform2 = null;
                Transform transform3 = null;
                transform = go.get_transform().Find("HeadIcon");
                transform2 = go.get_transform().Find("HeadbgNo1");
                transform3 = go.get_transform().Find("123No");
                if (((transform != null) && (transform2 != null)) && (transform3 != null))
                {
                    serverUrl = Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(ref rankList.astItemDetail[index].stExtraInfo.stDetailInfo.stLadderPoint.szHeadUrl);
                    transform2.get_gameObject().CustomSetActive(index == 0);
                    RankingView.SetUrlHeadIcon(transform.get_gameObject(), serverUrl);
                    transform3.GetChild(0).get_gameObject().CustomSetActive(0 == index);
                    transform3.GetChild(1).get_gameObject().CustomSetActive(1 == index);
                    transform3.GetChild(2).get_gameObject().CustomSetActive(2 == index);
                }
                int dwHeadIconId = (int) rankList.astItemDetail[index].stExtraInfo.stDetailInfo.stLadderPoint.stGameVip.dwHeadIconId;
                Image componetInChild = Utility.GetComponetInChild<Image>(go, "NobeImag");
                if (componetInChild != null)
                {
                    MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(componetInChild, dwHeadIconId);
                }
                Transform transform4 = go.get_transform().Find("QQVipIcon");
                if (transform4 != null)
                {
                    this.SetQQVip(transform4.get_gameObject(), false, (int) rankList.astItemDetail[index].stExtraInfo.stDetailInfo.stLadderPoint.dwVipLevel);
                }
            }
        }

        private void OnUpdateRankingFriendElement(GameObject go, int index)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            string serverUrl = string.Empty;
            GameObject headIcon = null;
            GameObject obj3 = null;
            GameObject obj4 = null;
            int num = (index <= this.myRankingNo) ? 0 : -1;
            Transform transform = go.get_transform();
            headIcon = transform.Find("HeadIcon").get_gameObject();
            obj3 = transform.get_transform().Find("HeadbgNo1").get_gameObject();
            obj4 = transform.get_transform().Find("123No").get_gameObject();
            int headIdx = 0;
            if (index == this.myRankingNo)
            {
                if (masterRoleInfo != null)
                {
                    serverUrl = masterRoleInfo.HeadUrl;
                    headIdx = (int) masterRoleInfo.GetNobeInfo().stGameVipClient.dwHeadIconId;
                    GameObject qQVipIcon = transform.get_transform().Find("QQVipIcon").get_gameObject();
                    this.SetQQVip(qQVipIcon, true, 0);
                }
            }
            else if ((index + num) < this.rankFriendList.Count)
            {
                serverUrl = Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(ref this.rankFriendList[index + num].szHeadUrl);
                headIdx = (int) this.rankFriendList[index + num].stGameVip.dwHeadIconId;
                GameObject obj6 = transform.get_transform().Find("QQVipIcon").get_gameObject();
                this.SetQQVip(obj6, false, (int) this.rankFriendList[index + num].dwQQVIPMask);
            }
            obj3.CustomSetActive(index == 0);
            obj4.get_transform().GetChild(0).get_gameObject().CustomSetActive(0 == index);
            obj4.get_transform().GetChild(1).get_gameObject().CustomSetActive(1 == index);
            obj4.get_transform().GetChild(2).get_gameObject().CustomSetActive(2 == index);
            Image component = transform.get_transform().Find("NobeImag").GetComponent<Image>();
            if (component != null)
            {
                MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(component, headIdx);
            }
            RankingView.SetUrlHeadIcon(headIcon, serverUrl);
        }

        private void OpenGuestGameCenterRightForm(CUIEvent uiEvent)
        {
            this.ShowPlatformRight();
        }

        private void OpenQQGameCenterRightForm(CUIEvent uiEvent)
        {
            this.ShowPlatformRight();
        }

        private void OpenQQVIPWealForm(CUIEvent uiEvent)
        {
            string formPath = string.Format("{0}{1}", "UGUI/Form/System/", "IDIPNotice/Form_QQVipPrivilege.prefab");
            CUIFormScript formScript = Singleton<CUIManager>.GetInstance().OpenForm(formPath, false, true);
            if (formScript != null)
            {
                Singleton<QQVipWidget>.instance.SetData(formScript.get_gameObject(), formScript);
            }
        }

        private void OpenWebHome(CUIEvent uiEvent)
        {
            ulong playerUllUID = 0L;
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                playerUllUID = masterRoleInfo.playerUllUID;
            }
            string platformArea = CUICommonSystem.GetPlatformArea();
            CUICommonSystem.OpenUrl(string.Concat(new object[] { "http://yxzj.qq.com/ingame/all/index.shtml?partition=", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID, "&roleid=", playerUllUID, "&area=", platformArea }), true, 0);
            if (CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Lobby_GongLueEntry))
            {
                CUIRedDotSystem.SetRedDotViewByVersion(enRedID.Lobby_GongLueEntry);
                if (this.m_LobbyForm != null)
                {
                    Transform transform = this.m_LobbyForm.get_transform().Find("Popup/SignBtn");
                    if (transform != null)
                    {
                        CUIRedDotSystem.DelRedDot(transform.get_gameObject());
                    }
                }
            }
        }

        private void OpenWXGameCenterRightForm(CUIEvent uiEvent)
        {
            this.ShowPlatformRight();
        }

        public void Play_UnLock_Animation(RES_SPECIALFUNCUNLOCK_TYPE type)
        {
            string str = string.Empty;
            switch (type)
            {
                case RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_UNION:
                    str = "SocialBtn";
                    goto Label_00AE;

                case RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_TASK:
                    str = "TaskBtn";
                    goto Label_00AE;

                case RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_HERO:
                    str = "HeroBtn";
                    goto Label_00AE;

                case RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_BAG:
                    str = "BagBtn";
                    goto Label_00AE;

                case RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_FRIEND:
                    str = "FriendBtn";
                    goto Label_00AE;

                case RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_ADDEDSKILL:
                    str = "AddedSkillBtn";
                    goto Label_00AE;

                case RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_SYMBOL:
                    str = "SymbolBtn";
                    break;

                case RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_ACHIEVEMENT:
                    str = "AchievementBtn";
                    break;
            }
        Label_00AE:
            if (string.IsNullOrEmpty(str))
            {
                return;
            }
            stUIEventParams eventParams = new stUIEventParams();
            eventParams.tag = (int) type;
            CUIAnimatorScript component = Singleton<CUIManager>.instance.OpenForm(LOBBY_FUN_UNLOCK_PATH, false, true).GetComponent<CUIAnimatorScript>();
            component.SetUIEvent(enAnimatorEventType.AnimatorEnd, enUIEventID.Lobby_UnlockAnimation_End, eventParams);
            component.PlayAnimator(str);
            Singleton<CSoundManager>.instance.PostEvent("UI_hall_system_unlock", null);
        }

        private bool PopupNewbieIntro()
        {
            if (CSysDynamicBlock.bNewbieBlocked)
            {
                return true;
            }
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            DebugHelper.Assert(masterRoleInfo != null, "Master Role info is NULL!");
            if (((masterRoleInfo != null) && !masterRoleInfo.IsNewbieAchieveSet(0x54)) && (Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Newbie/Form_NewbieSettle.prefab") == null))
            {
                masterRoleInfo.SetNewbieAchieve(0x54, true, true);
                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Wealfare_CloseForm);
                return true;
            }
            return false;
        }

        private void ProcessQQVIP(CUIFormScript form)
        {
            if (null != form)
            {
                Transform transform = form.get_transform().Find("VIPGroup/QQVIpBtn");
                GameObject obj2 = null;
                if (transform != null)
                {
                    obj2 = transform.get_gameObject();
                }
                GameObject obj3 = Utility.FindChild(form.get_gameObject(), "PlayerHead/NameGroup/QQVipIcon");
                if ((ApolloConfig.platform == ApolloPlatform.QQ) || (ApolloConfig.platform == ApolloPlatform.WTLogin))
                {
                    if (CSysDynamicBlock.bLobbyEntryBlocked)
                    {
                        obj2.CustomSetActive(false);
                        obj3.CustomSetActive(false);
                    }
                    else
                    {
                        obj2.CustomSetActive(true);
                        if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo() != null)
                        {
                            MonoSingleton<NobeSys>.GetInstance().SetMyQQVipHead(obj3.GetComponent<Image>());
                        }
                    }
                }
                else
                {
                    obj2.CustomSetActive(false);
                    obj3.CustomSetActive(false);
                }
            }
        }

        public static void RefreshDianQuanPayButton(bool notifyFromSvr = false)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(LOBBY_FORM_PATH);
            if (form != null)
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                GameObject btn = form.get_transform().Find("DiamondPayBtn").get_gameObject();
                CUIEventScript component = btn.GetComponent<CUIEventScript>();
                CTextManager instance = Singleton<CTextManager>.GetInstance();
                if (!masterRoleInfo.IsGuidedStateSet(0x16))
                {
                    CUICommonSystem.SetButtonName(btn, instance.GetText("Pay_Btn_FirstPay"));
                    component.SetUIEvent(enUIEventType.Click, enUIEventID.Pay_OpenFirstPayPanel);
                    CUICommonSystem.DelRedDot(btn);
                }
                else if (!masterRoleInfo.IsGuidedStateSet(0x17))
                {
                    CUICommonSystem.SetButtonName(btn, instance.GetText("Pay_Btn_FirstPay"));
                    component.SetUIEvent(enUIEventType.Click, enUIEventID.Pay_OpenFirstPayPanel);
                    CUICommonSystem.AddRedDot(btn, enRedDotPos.enTopRight, 0);
                }
                else if (!masterRoleInfo.IsGuidedStateSet(0x18))
                {
                    CUICommonSystem.SetButtonName(btn, instance.GetText("Pay_Btn_Renewal"));
                    component.SetUIEvent(enUIEventType.Click, enUIEventID.Pay_OpenRenewalPanel);
                    CUICommonSystem.DelRedDot(btn);
                }
                else if (!masterRoleInfo.IsGuidedStateSet(0x19))
                {
                    CUICommonSystem.SetButtonName(btn, instance.GetText("Pay_Btn_Renewal"));
                    component.SetUIEvent(enUIEventType.Click, enUIEventID.Pay_OpenRenewalPanel);
                    CUICommonSystem.AddRedDot(btn, enRedDotPos.enTopRight, 0);
                }
                else if (masterRoleInfo.IsClientBitsSet(0))
                {
                    CUICommonSystem.SetButtonName(btn, instance.GetText("GotoTehuiShopName"));
                    component.SetUIEvent(enUIEventType.Click, enUIEventID.Pay_TehuiShop);
                }
                else if (notifyFromSvr)
                {
                    masterRoleInfo.SetClientBits(0, true, false);
                    RefreshDianQuanPayButton(false);
                }
                else
                {
                    btn.CustomSetActive(false);
                }
            }
        }

        private void RefreshRankList()
        {
            if (this._rankingBtn != null)
            {
                CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(this._rankingBtn, "RankingList");
                int amount = 0;
                if (this.m_rankingType == RankingSystem.RankingSubView.Friend)
                {
                    this.myRankingNo = Singleton<RankingSystem>.instance.GetMyFriendRankNo();
                    if (this.myRankingNo != -1)
                    {
                        this.rankFriendList = Singleton<CFriendContoller>.instance.model.GetSortedRankingFriendList(COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_LADDER_POINT);
                        if (this.rankFriendList != null)
                        {
                            amount = this.rankFriendList.Count + 1;
                            componetInChild.SetElementAmount(amount);
                            CUIListElementScript elemenet = null;
                            GameObject go = null;
                            for (int i = 0; i < amount; i++)
                            {
                                elemenet = componetInChild.GetElemenet(i);
                                if (elemenet != null)
                                {
                                    go = elemenet.get_gameObject();
                                    if (go != null)
                                    {
                                        this.OnUpdateRankingFriendElement(go, i);
                                    }
                                }
                            }
                        }
                    }
                }
                else if (this.m_rankingType == RankingSystem.RankingSubView.All)
                {
                    CSDT_RANKING_LIST_SUCC rankList = Singleton<RankingSystem>.instance.GetRankList(RankingSystem.RankingType.Ladder);
                    if (rankList != null)
                    {
                        amount = (int) rankList.dwItemNum;
                        componetInChild.SetElementAmount(amount);
                        CUIListElementScript script3 = null;
                        GameObject obj3 = null;
                        for (int j = 0; j < amount; j++)
                        {
                            script3 = componetInChild.GetElemenet(j);
                            if (script3 != null)
                            {
                                obj3 = script3.get_gameObject();
                                if (obj3 != null)
                                {
                                    this.OnUpdateRankingAllElement(obj3, j);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void RefreshRankList(RankingSystem.RankingSubView rankingType)
        {
            this.m_rankingType = rankingType;
            this.RefreshRankList();
        }

        private void SetEnable(RES_SPECIALFUNCUNLOCK_TYPE type, bool bShow)
        {
            GameObject obj2 = null;
            if (type == RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_HERO)
            {
                obj2 = this.hero_btn;
            }
            else if (type == RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_SYMBOL)
            {
                obj2 = this.symbol_btn;
            }
            else if (type == RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_BAG)
            {
                obj2 = this.bag_btn;
            }
            else if (type == RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_TASK)
            {
                obj2 = this.task_btn;
            }
            else if (type == RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_UNION)
            {
                obj2 = this.social_btn;
            }
            else if (type == RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_ADDEDSKILL)
            {
                obj2 = this.addSkill_btn;
            }
            else if (type == RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_ACHIEVEMENT)
            {
                obj2 = this.achievement_btn;
            }
            else
            {
                obj2 = null;
            }
            if (obj2 != null)
            {
                obj2.CustomSetActive(bShow);
            }
        }

        private void SetQQVip(GameObject QQVipIcon, bool bSelf, int mask = 0)
        {
            if (QQVipIcon != null)
            {
                if ((ApolloConfig.platform == ApolloPlatform.QQ) || (ApolloConfig.platform == ApolloPlatform.WTLogin))
                {
                    if (CSysDynamicBlock.bLobbyEntryBlocked)
                    {
                        QQVipIcon.CustomSetActive(false);
                    }
                    else if (bSelf)
                    {
                        if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo() != null)
                        {
                            MonoSingleton<NobeSys>.GetInstance().SetMyQQVipHead(QQVipIcon.GetComponent<Image>());
                        }
                    }
                    else
                    {
                        MonoSingleton<NobeSys>.GetInstance().SetOtherQQVipHead(QQVipIcon.GetComponent<Image>(), mask);
                    }
                }
                else
                {
                    QQVipIcon.CustomSetActive(false);
                }
            }
        }

        public void SetTopBarPriority(enFormPriority prioRity)
        {
            if (this.m_SysEntryForm != null)
            {
                CUIFormScript component = this.m_SysEntryForm.GetComponent<CUIFormScript>();
                if (component != null)
                {
                    component.SetPriority(prioRity);
                }
            }
        }

        public void ShowHideRankingBtn(bool show)
        {
            if (this._rankingBtn != null)
            {
                if (CSysDynamicBlock.bSocialBlocked)
                {
                    this._rankingBtn.CustomSetActive(false);
                }
                else
                {
                    this._rankingBtn.CustomSetActive(show);
                }
            }
        }

        public void ShowOrHideWifiInfo()
        {
            if (this.m_wifiInfo != null)
            {
                this.m_wifiInfo.CustomSetActive(!this.m_wifiInfo.get_activeInHierarchy());
            }
            this.CheckWifi();
        }

        private void ShowPlatformRight()
        {
            if (Singleton<ApolloHelper>.GetInstance().CurPlatform == ApolloPlatform.Guest)
            {
                string formPath = string.Format("{0}{1}", "UGUI/Form/System/", "GameCenter/Form_GuestGameCenter.prefab");
                Singleton<CUIManager>.GetInstance().OpenForm(formPath, false, true);
            }
            else if (Singleton<ApolloHelper>.GetInstance().CurPlatform == ApolloPlatform.QQ)
            {
                string str2 = string.Format("{0}{1}", "UGUI/Form/System/", "GameCenter/Form_QQGameCenter.prefab");
                Singleton<CUIManager>.GetInstance().OpenForm(str2, false, true);
            }
            else if (Singleton<ApolloHelper>.GetInstance().CurPlatform == ApolloPlatform.Wechat)
            {
                string str3 = string.Format("{0}{1}", "UGUI/Form/System/", "GameCenter/Form_WXGameCenter.prefab");
                Singleton<CUIManager>.GetInstance().OpenForm(str3, false, true);
            }
        }

        private void StartAutoPopupChain(int timerSeq)
        {
            AutoPopAllow &= !MonoSingleton<NewbieGuideManager>.GetInstance().isNewbieGuiding;
            if (AutoPopAllow)
            {
                this.AutoPopup1_IDIP();
            }
        }

        public override void UnInit()
        {
            base.UnInit();
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Lobby_OpenLobbyForm, new CUIEventManager.OnUIEventHandler(this.onOpenLobby));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.WEB_OpenURL, new CUIEventManager.OnUIEventHandler(this.onOpenWeb));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Common_WifiCheckTimer, new CUIEventManager.OnUIEventHandler(this.onCommon_WifiCheckTimer));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Common_ShowOrHideWifiInfo, new CUIEventManager.OnUIEventHandler(this.onCommon_ShowOrHideWifiInfo));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Lobby_LobbyFormShow, new CUIEventManager.OnUIEventHandler(this.Lobby_LobbyFormShow));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Lobby_LobbyFormHide, new CUIEventManager.OnUIEventHandler(this.Lobby_LobbyFormHide));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Lobby_ConfirmErrExit, new CUIEventManager.OnUIEventHandler(this.onErrorExit));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Lobby_Close, new CUIEventManager.OnUIEventHandler(this.OnCloseForm));
            Singleton<EventRouter>.instance.RemoveEventHandler("MasterAttributesChanged", new Action(this, (IntPtr) this.UpdatePlayerData));
            Singleton<EventRouter>.instance.RemoveEventHandler("TaskUpdated", new Action(this, (IntPtr) this.OnTaskUpdate));
            Singleton<EventRouter>.instance.RemoveEventHandler("Friend_LobbyIconRedDot_Refresh", new Action(this, (IntPtr) this.OnFriendSysIconUpdate));
            Singleton<EventRouter>.instance.RemoveEventHandler(EventID.Mall_Entry_Add_RedDotCheck, new Action(this, (IntPtr) this.OnCheckRedDotByServerVersionWithLobby));
            Singleton<EventRouter>.instance.RemoveEventHandler(EventID.Mall_Entry_Del_RedDotCheck, new Action(this, (IntPtr) this.OnCheckDelMallEntryRedDot));
            Singleton<EventRouter>.instance.RemoveEventHandler("MailUnReadNumUpdate", new Action(this, (IntPtr) this.OnMailUnReadUpdate));
            Singleton<EventRouter>.instance.RemoveEventHandler(EventID.ACHIEVE_TROPHY_REWARD_INFO_STATE_CHANGE, new Action(this, (IntPtr) this.OnAchieveStateUpdate));
            Singleton<EventRouter>.instance.RemoveEventHandler(EventID.SymbolEquipSuc, new Action(this, (IntPtr) this.OnCheckSymbolEquipAlert));
            Singleton<EventRouter>.instance.RemoveEventHandler(EventID.BAG_ITEMS_UPDATE, new Action(this, (IntPtr) this.OnBagItemsUpdate));
            Singleton<EventRouter>.instance.RemoveEventHandler("MasterPvpLevelChanged", new Action(this, (IntPtr) this.OnCheckSymbolEquipAlert));
            Singleton<ActivitySys>.GetInstance().OnStateChange -= new ActivitySys.StateChangeDelegate(this.ValidateActivitySpot);
            Singleton<EventRouter>.instance.RemoveEventHandler("IDIPNOTICE_UNREAD_NUM_UPDATE", new Action(this, (IntPtr) this.ValidateActivitySpot));
            Singleton<EventRouter>.instance.RemoveEventHandler("MasterJifenChanged", new Action(this, (IntPtr) this.ValidateActivitySpot));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.IDIP_QQVIP_OpenWealForm, new CUIEventManager.OnUIEventHandler(this.OpenQQVIPWealForm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.WEB_OpenHome, new CUIEventManager.OnUIEventHandler(this.OpenWebHome));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Lobby_UnlockAnimation_End, new CUIEventManager.OnUIEventHandler(this.On_Lobby_UnlockAnimation_End));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Lobby_MysteryShopClose, new CUIEventManager.OnUIEventHandler(this.On_Lobby_MysteryShopClose));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.GameCenter_OpenWXRight, new CUIEventManager.OnUIEventHandler(this.OpenWXGameCenterRightForm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Lobby_RankingListElementEnable, new CUIEventManager.OnUIEventHandler(this.OnRankingListElementEnable));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.GameCenter_OpenGuestRight, new CUIEventManager.OnUIEventHandler(this.OpenGuestGameCenterRightForm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Lobby_GotoBattleWebHome, new CUIEventManager.OnUIEventHandler(this.OnGotoBattleWebHome));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.NOBE_STATE_CHANGE, new Action(this, (IntPtr) this.UpdateNobeIcon));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.NOBE_STATE_HEAD_CHANGE, new Action(this, (IntPtr) this.UpdateNobeHeadIdx));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler("MasterPvpLevelChanged", new Action(this, (IntPtr) this.OnPlayerLvlChange));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler("Rank_Friend_List", new Action(this, (IntPtr) this.RefreshRankList));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<RankingSystem.RankingSubView>("Rank_List", new Action<RankingSystem.RankingSubView>(this.RefreshRankList));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.HEAD_IMAGE_FLAG_CHANGE, new Action(this, (IntPtr) this.UpdatePlayerData));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler("CheckNewbieIntro", new Action(this, (IntPtr) this.OnCheckNewbieIntro));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler("VipInfoHadSet", new Action(this, (IntPtr) this.UpdateQQVIPState));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.LOBBY_PURE_LOBBY_SHOW, new Action(this, (IntPtr) this.OnPureLobbyShow));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.GAMER_REDDOT_CHANGE, new Action(this, (IntPtr) this.UpdatePlayerData));
        }

        private void UnInitWidget()
        {
            this._rankingBtn = null;
        }

        private void UpdateGameCenterState(CUIFormScript form)
        {
            if (null != form)
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                COM_PRIVILEGE_TYPE privilegeType = (masterRoleInfo != null) ? masterRoleInfo.m_privilegeType : COM_PRIVILEGE_TYPE.COM_PRIVILEGE_TYPE_NONE;
                GameObject obj2 = Utility.FindChild(form.get_gameObject(), "VIPGroup/WXGameCenterBtn");
                GameObject obj3 = Utility.FindChild(form.get_gameObject(), "PlayerHead/NameGroup/WXGameCenterIcon");
                MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(obj2, privilegeType, ApolloPlatform.Wechat, false, CSysDynamicBlock.bLobbyEntryBlocked);
                MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(obj3, privilegeType, ApolloPlatform.Wechat, true, false);
                GameObject obj4 = Utility.FindChild(form.get_gameObject(), "VIPGroup/QQGameCenterBtn");
                GameObject obj5 = Utility.FindChild(form.get_gameObject(), "PlayerHead/NameGroup/QQGameCenterIcon");
                MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(obj4, privilegeType, ApolloPlatform.QQ, false, CSysDynamicBlock.bLobbyEntryBlocked);
                MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(obj5, privilegeType, ApolloPlatform.QQ, true, false);
                GameObject obj6 = Utility.FindChild(form.get_gameObject(), "VIPGroup/GuestGameCenterBtn");
                GameObject obj7 = Utility.FindChild(form.get_gameObject(), "PlayerHead/NameGroup/GuestGameCenterIcon");
                MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(obj6, privilegeType, ApolloPlatform.Guest, false, CSysDynamicBlock.bLobbyEntryBlocked);
                MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(obj7, privilegeType, ApolloPlatform.Guest, true, false);
            }
        }

        private void UpdateNobeHeadIdx()
        {
            int dwHeadIconId = (int) MonoSingleton<NobeSys>.GetInstance().m_vipInfo.stGameVipClient.dwHeadIconId;
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (((this.m_LobbyForm != null) && this.m_LobbyForm.get_gameObject().get_activeSelf()) && (masterRoleInfo != null))
            {
                Image component = this.m_LobbyForm.GetWidget(3).GetComponent<Image>();
                if (component != null)
                {
                    MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(component, dwHeadIconId);
                    MonoSingleton<NobeSys>.GetInstance().SetHeadIconBkEffect(component, dwHeadIconId, this.m_LobbyForm, 0.7f);
                }
                this.RefreshRankList();
            }
        }

        private void UpdateNobeIcon()
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (((this.m_LobbyForm != null) && this.m_LobbyForm.get_gameObject().get_activeSelf()) && (masterRoleInfo != null))
            {
                GameObject widget = this.m_LobbyForm.GetWidget(2);
                if (widget != null)
                {
                    CUIHttpImageScript component = widget.GetComponent<CUIHttpImageScript>();
                    MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component.GetComponent<Image>(), (int) masterRoleInfo.GetNobeInfo().stGameVipClient.dwCurLevel, false);
                    Image image = this.m_LobbyForm.GetWidget(3).GetComponent<Image>();
                    MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(image, (int) masterRoleInfo.GetNobeInfo().stGameVipClient.dwHeadIconId);
                    MonoSingleton<NobeSys>.GetInstance().SetHeadIconBkEffect(image, (int) masterRoleInfo.GetNobeInfo().stGameVipClient.dwHeadIconId, this.m_LobbyForm, 0.7f);
                }
            }
        }

        private void UpdatePlayerData()
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            if ((masterRoleInfo != null) && (this.m_LobbyForm != null))
            {
                if (this.m_PlayerName != null)
                {
                    this.m_PlayerName.set_text(masterRoleInfo.Name);
                }
                if (this.m_PvpExpImg != null)
                {
                    this.m_PvpExpImg.CustomFillAmount(CPlayerProfile.Divide(masterRoleInfo.PvpExp, masterRoleInfo.PvpNeedExp));
                    this.m_PvpExpTxt.set_text(masterRoleInfo.PvpExp + "/" + masterRoleInfo.PvpNeedExp);
                }
                if (this.m_PvpLevel != null)
                {
                    string text = Singleton<CTextManager>.GetInstance().GetText("ranking_PlayerLevel");
                    if ((!string.IsNullOrEmpty(text) && (this.m_PvpLevel.get_text() != null)) && (masterRoleInfo != null))
                    {
                        this.m_PvpLevel.set_text(string.Format(text, masterRoleInfo.PvpLevel));
                    }
                }
                if (!CSysDynamicBlock.bSocialBlocked)
                {
                    if (((this.m_LobbyForm != null) && this.m_LobbyForm.get_gameObject().get_activeSelf()) && (masterRoleInfo != null))
                    {
                        GameObject widget = this.m_LobbyForm.GetWidget(2);
                        if ((widget != null) && !string.IsNullOrEmpty(masterRoleInfo.HeadUrl))
                        {
                            CUIHttpImageScript component = widget.GetComponent<CUIHttpImageScript>();
                            component.SetImageUrl(masterRoleInfo.HeadUrl);
                            MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component.GetComponent<Image>(), (int) masterRoleInfo.GetNobeInfo().stGameVipClient.dwCurLevel, false);
                            Image image = this.m_LobbyForm.GetWidget(3).GetComponent<Image>();
                            MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(image, (int) masterRoleInfo.GetNobeInfo().stGameVipClient.dwHeadIconId);
                            MonoSingleton<NobeSys>.GetInstance().SetHeadIconBkEffect(image, (int) masterRoleInfo.GetNobeInfo().stGameVipClient.dwHeadIconId, this.m_LobbyForm, 0.7f);
                            bool flag = (Singleton<HeadIconSys>.instance.UnReadFlagNum > 0) || masterRoleInfo.ShowGameRedDot;
                            GameObject target = Utility.FindChild(widget, "RedDot");
                            if (target != null)
                            {
                                if (flag)
                                {
                                    CUICommonSystem.AddRedDot(target, enRedDotPos.enTopRight, 0);
                                }
                                else
                                {
                                    CUICommonSystem.DelRedDot(target);
                                }
                            }
                        }
                    }
                }
                else if ((this.m_LobbyForm != null) && this.m_LobbyForm.get_gameObject().get_activeSelf())
                {
                    GameObject obj4 = this.m_LobbyForm.GetWidget(2);
                    if (obj4 != null)
                    {
                        CUIHttpImageScript script2 = obj4.GetComponent<CUIHttpImageScript>();
                        if (script2 != null)
                        {
                            MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(script2.GetComponent<Image>(), 0, false);
                        }
                    }
                }
                if (this.m_lblGlodCoin != null)
                {
                    this.m_lblGlodCoin.set_text(this.GetCoinString(masterRoleInfo.GoldCoin));
                }
                if (this.m_lblDianquan != null)
                {
                    this.m_lblDianquan.set_text(this.GetCoinString((uint) masterRoleInfo.DianQuan));
                }
                if (this.m_lblDiamond != null)
                {
                    this.m_lblDiamond.set_text(this.GetCoinString(masterRoleInfo.Diamond));
                }
            }
        }

        private void UpdateQQVIPState()
        {
            if (this.m_bInLobby && (this.m_LobbyForm != null))
            {
                Transform transform = this.m_LobbyForm.get_transform();
                Transform transform2 = transform.Find("VIPGroup/QQVIpBtn");
                GameObject obj2 = null;
                if (transform2 != null)
                {
                    obj2 = transform2.get_gameObject();
                }
                Transform transform3 = transform.Find("PlayerHead/NameGroup/QQVipIcon");
                GameObject obj3 = null;
                GameObject obj4 = null;
                if (transform3 != null)
                {
                    obj3 = transform3.get_gameObject();
                }
                if ((ApolloConfig.platform == ApolloPlatform.QQ) || (ApolloConfig.platform == ApolloPlatform.WTLogin))
                {
                    if (CSysDynamicBlock.bLobbyEntryBlocked)
                    {
                        obj2.CustomSetActive(false);
                        obj3.CustomSetActive(false);
                        obj4.CustomSetActive(false);
                    }
                    else
                    {
                        obj2.CustomSetActive(true);
                        CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                        if (masterRoleInfo != null)
                        {
                            if (masterRoleInfo.HasVip(0x10))
                            {
                                obj3.CustomSetActive(false);
                                obj4.CustomSetActive(true);
                            }
                            else if (masterRoleInfo.HasVip(1))
                            {
                                obj3.CustomSetActive(true);
                                obj4.CustomSetActive(false);
                            }
                            else
                            {
                                obj3.CustomSetActive(false);
                                obj4.CustomSetActive(false);
                            }
                        }
                    }
                }
                else
                {
                    obj2.CustomSetActive(false);
                    obj3.CustomSetActive(false);
                    obj4.CustomSetActive(false);
                }
            }
        }

        public void ValidateActivitySpot()
        {
            if (this.m_bInLobby)
            {
                if (Singleton<ActivitySys>.GetInstance().CheckReadyForDot(RES_WEAL_ENTRANCE_TYPE.RES_WEAL_ENTRANCE_TYPE_ACTIVITY))
                {
                    uint reveivableRedDot = Singleton<ActivitySys>.GetInstance().GetReveivableRedDot(RES_WEAL_ENTRANCE_TYPE.RES_WEAL_ENTRANCE_TYPE_ACTIVITY);
                    this.AddRedDotEx(enSysEntryID.ActivityBtn, enRedDotPos.enTopRight, (int) reveivableRedDot);
                }
                else if (MonoSingleton<IDIPSys>.GetInstance().HaveUpdateList)
                {
                    this.AddRedDotEx(enSysEntryID.ActivityBtn, enRedDotPos.enTopRight, 0);
                }
                else if (MonoSingleton<PandroaSys>.GetInstance().ShowRedPoint)
                {
                    this.AddRedDotEx(enSysEntryID.ActivityBtn, enRedDotPos.enTopRight, 0);
                }
                else
                {
                    this.DelRedDot(enSysEntryID.ActivityBtn);
                }
            }
        }

        public static bool IsPlatChannelOpen
        {
            [CompilerGenerated]
            get
            {
                return <IsPlatChannelOpen>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                <IsPlatChannelOpen>k__BackingField = value;
            }
        }

        public enum enSysEntryFormWidget
        {
            WifiIcon,
            WifiInfo,
            WifiPing,
            GlodCoin,
            Dianquan,
            MailBtn,
            SettingBtn,
            Wifi_Bg,
            FriendBtn,
            MianLiuTxt
        }

        public enum LobbyFormWidget
        {
            HeadImgBack = 3,
            LoudSpeakerRolling = 5,
            LoudSpeakerRollingBg = 6,
            None = -1,
            PlatChannel = 7,
            PlatChannelText = 8,
            RankingBtn = 1,
            Reserve = 0,
            Rolling = 4,
            SnsHead = 2
        }

        public enum LobbyRankingBtnFormWidget
        {
            RankingBtnPanel
        }
    }
}

