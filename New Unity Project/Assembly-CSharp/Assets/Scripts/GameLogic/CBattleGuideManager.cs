namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.GameKernal;
    using Assets.Scripts.GameSystem;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    internal class CBattleGuideManager : Singleton<CBattleGuideManager>
    {
        private bool _PauseGame;
        [CompilerGenerated]
        private static CTimer.OnTimeUpHandler <>f__am$cache28;
        public static readonly uint AdvanceGuideLevelID = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x37).dwConfValue;
        public bool bReEntry;
        public bool bTrainingAdv;
        public static bool CanSelectHeroOnTrainLevelEntry = false;
        private float coinFrom;
        private LTDescr coinLtd;
        private float coinTo;
        private Text coinTweenText;
        private CUIFormScript curOpenForm;
        private const float ExpBarWidth = 220.3f;
        private float expFrom;
        private LTDescr expLtd;
        private float expTo;
        private RectTransform expTweenRect;
        private string[] GuideFormPathList = new string[0x22];
        private static readonly uint GuideLevel1v1AD = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_1V1_GUIDE_LEVEL_AD);
        private static readonly uint GuideLevel1v1AP = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_1V1_GUIDE_LEVEL_AP);
        private static readonly uint GuideLevel1v1Tank = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_1V1_GUIDE_LEVEL);
        private static readonly uint GuideLevel5v5AD = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_5V5_GUIDE_LEVEL_AD);
        private static readonly uint GuideLevel5v5AP = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_5V5_GUIDE_LEVEL_AP);
        private static readonly uint GuideLevel5v5Tank = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_5V5_GUIDE_LEVEL);
        public static readonly uint GuideLevelID3v3 = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x75).dwConfValue;
        public static readonly uint GuideLevelIDCasting = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x77).dwConfValue;
        public static readonly uint GuideLevelIDJungle = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x79).dwConfValue;
        public const string HALL_INTRO_FORM_PATH = "UGUI/Form/System/Newbie/Form_IntroHall.prefab";
        private const string INTRO_33_FORM_PATH = "UGUI/Form/System/Newbie/Form_Intro_3V3.prefab";
        private static uint lvUpGrade = 0;
        private NewbieBannerIntroDialog m_BannerIntroDialog;
        private bool m_bWin;
        public EBattleGuideFormType m_lastFormType;
        private Dictionary<object, int> m_pauseGameStack = new Dictionary<object, int>();
        public static bool ms_bOldPlayerFormOpened = false;
        private static string NEWBIE_GOLDAWARD_FORM_PATH = "UGUI/Form/System/Newbie/Form_AwardNewbie";
        public const string OLD_PLAYER_FIRST_FORM_PATH = "UGUI/Form/System/Newbie/Form_OldTip.prefab";
        private static string PROFIT_FORM_NAME = "UGUI/Form/System/Newbie/Form_NewbieProfit.prefab";
        public const string SETTLE_FORM_PATH = "UGUI/Form/System/Newbie/Form_NewbieSettle.prefab";
        private LTDescr symbolCoinfLtd;
        private float symbolCoinFrom;
        private float symbolCoinTo;
        private Text symbolCoinTweenText;
        public static int TIME_OUT = 0x7530;
        private COMDT_REWARD_DETAIL TrainLevelReward = new COMDT_REWARD_DETAIL();
        public static string TUTORIAL_END_FORM_PATH = "UGUI/Form/System/Newbie/Form_End_Tutorial.prefab";
        private const float TweenTime = 2f;
        public static readonly uint Warm1v1SpecialLevelId = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x93).dwConfValue;

        public void CloseFormShared(EBattleGuideFormType inFormType)
        {
            Singleton<CTimerManager>.GetInstance().RemoveTimer(new CTimer.OnTimeUpHandler(CBattleGuideManager.TimeOutDelegate));
            this.UpdateGamePausing(false);
            string formPath = this.GuideFormPathList[(int) inFormType];
            Singleton<CUIManager>.GetInstance().CloseForm(formPath);
            if (Singleton<GameStateCtrl>.instance.isBattleState)
            {
                if (inFormType == EBattleGuideFormType.SkillGesture2)
                {
                    List<GameObject> list = Singleton<BattleSkillHudControl>.GetInstance().QuerySkillButtons(SkillSlotType.SLOT_SKILL_2, false);
                    if (list.Count > 0)
                    {
                        GameObject obj2 = list[0];
                        this.showSkillButton(obj2.get_transform().FindChild("Present").get_gameObject(), true);
                    }
                }
                else if (inFormType == EBattleGuideFormType.SkillGesture2Cancel)
                {
                    List<GameObject> list2 = Singleton<BattleSkillHudControl>.GetInstance().QuerySkillButtons(SkillSlotType.SLOT_SKILL_2, false);
                    if (list2.Count > 0)
                    {
                        GameObject obj3 = list2[0];
                        this.showSkillButton(obj3.get_transform().FindChild("Present").get_gameObject(), true);
                    }
                }
                else if (inFormType == EBattleGuideFormType.SkillGesture3)
                {
                    List<GameObject> list3 = Singleton<BattleSkillHudControl>.GetInstance().QuerySkillButtons(SkillSlotType.SLOT_SKILL_3, false);
                    if (list3.Count > 0)
                    {
                        GameObject obj4 = list3[0];
                        this.showSkillButton(obj4.get_transform().FindChild("Present").get_gameObject(), true);
                    }
                }
            }
        }

        private void CloseSettle()
        {
            Singleton<CUIManager>.GetInstance().CloseForm("UGUI/Form/System/Newbie/Form_NewbieSettle.prefab");
            Singleton<EventRouter>.instance.BroadCastEvent("CheckNewbieIntro");
        }

        private void DoCoinAndExpTween()
        {
            try
            {
                if ((this.coinTweenText != null) && (this.coinTweenText.get_gameObject() != null))
                {
                    this.coinLtd = LeanTween.value(this.coinTweenText.get_gameObject(), delegate (float value) {
                        if ((this.coinTweenText != null) && (this.coinTweenText.get_gameObject() != null))
                        {
                            this.coinTweenText.set_text(string.Format("+{0}", value.ToString("N0")));
                            if (value >= this.coinTo)
                            {
                                this.DoCoinTweenEnd();
                            }
                        }
                    }, this.coinFrom, this.coinTo, 2f);
                }
                if ((this.symbolCoinTweenText != null) && (this.symbolCoinTweenText.get_gameObject() != null))
                {
                    this.symbolCoinfLtd = LeanTween.value(this.coinTweenText.get_gameObject(), delegate (float value) {
                        if ((this.symbolCoinTweenText != null) && (this.symbolCoinTweenText.get_gameObject() != null))
                        {
                            this.symbolCoinTweenText.set_text(string.Format("+{0}", value.ToString("N0")));
                            if (value >= this.symbolCoinTo)
                            {
                                this.DoSymbolCoinTweenEnd();
                            }
                        }
                    }, this.symbolCoinFrom, this.symbolCoinTo, 2f);
                }
                if ((this.expTweenRect != null) && (this.expTweenRect.get_gameObject() != null))
                {
                    this.expLtd = LeanTween.value(this.expTweenRect.get_gameObject(), delegate (float value) {
                        if ((this.expTweenRect != null) && (this.expTweenRect.get_gameObject() != null))
                        {
                            this.expTweenRect.set_sizeDelta(new Vector2(value * 220.3f, this.expTweenRect.get_sizeDelta().y));
                            if (value >= this.expTo)
                            {
                                this.DoExpTweenEnd();
                            }
                        }
                    }, this.expFrom, this.expTo, 2f);
                }
            }
            catch (Exception exception)
            {
                object[] inParameters = new object[] { exception.Message };
                DebugHelper.Assert(false, "Exceptin in DoCoinAndExpTween, {0}", inParameters);
            }
        }

        public void DoCoinTweenEnd()
        {
            if ((this.coinLtd != null) && (this.coinTweenText != null))
            {
                this.coinTweenText.set_text(string.Format("+{0}", this.coinTo.ToString("N0")));
                COMDT_ACNT_INFO acntInfo = Singleton<BattleStatistic>.GetInstance().acntInfo;
                if (Singleton<BattleStatistic>.GetInstance().multiDetail != null)
                {
                    CUICommonSystem.AppendMultipleText(this.coinTweenText, CUseable.GetMultiple(acntInfo.dwPvpSettleBaseCoin, ref Singleton<BattleStatistic>.GetInstance().multiDetail, 0, -1));
                }
                this.coinLtd.cancel();
                this.coinLtd = null;
                this.coinTweenText = null;
            }
        }

        private void DoExpTweenEnd()
        {
            if ((this.expTweenRect != null) && (this.expLtd != null))
            {
                this.expTweenRect.set_sizeDelta(new Vector2(this.expTo * 220.3f, this.expTweenRect.get_sizeDelta().y));
                this.expLtd.cancel();
                this.expLtd = null;
                this.expTweenRect = null;
            }
            if (lvUpGrade > 1)
            {
                CUIEvent event3 = new CUIEvent();
                event3.m_eventID = enUIEventID.Settle_OpenLvlUp;
                event3.m_eventParams.tag = ((int) lvUpGrade) - 1;
                event3.m_eventParams.tag2 = (int) lvUpGrade;
                CUIEvent uiEvent = event3;
                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uiEvent);
            }
            lvUpGrade = 0;
        }

        public void DoSymbolCoinTweenEnd()
        {
            if ((this.symbolCoinfLtd != null) && (this.symbolCoinTweenText != null))
            {
                this.symbolCoinTweenText.set_text(string.Format("+{0}", this.symbolCoinTo.ToString("N0")));
                this.symbolCoinfLtd.cancel();
                this.symbolCoinfLtd = null;
                this.symbolCoinTweenText = null;
            }
        }

        public static bool EnableHeroVictoryTips()
        {
            bool flag = false;
            ResGlobalInfo info = null;
            GameDataMgr.svr2CltCfgDict.TryGetValue(9, out info);
            if (info != null)
            {
                flag = Convert.ToBoolean(info.dwConfValue);
            }
            return flag;
        }

        private void EnterAdvanceGuide(CUIEvent uiEvent)
        {
            uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x90).dwConfValue;
            Singleton<LobbyLogic>.GetInstance().ReqStartGuideLevel((int) AdvanceGuideLevelID, dwConfValue);
        }

        public static bool GetGuide1v1HeroIDAndLevelID(uint heroType, out uint heroId, out int levelId)
        {
            if (heroType == GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_NEWBIE_RECOMMEND_HEROTYPE1))
            {
                heroId = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_1V1_GUIDE_HERO);
                levelId = (int) GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_1V1_GUIDE_LEVEL);
                return true;
            }
            if (heroType == GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_NEWBIE_RECOMMEND_HEROTYPE2))
            {
                heroId = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_1V1_GUIDE_HERO_AD);
                levelId = (int) GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_1V1_GUIDE_LEVEL_AD);
                return true;
            }
            if (heroType == GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_NEWBIE_RECOMMEND_HEROTYPE3))
            {
                heroId = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_1V1_GUIDE_HERO_AP);
                levelId = (int) GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_1V1_GUIDE_LEVEL_AP);
                return true;
            }
            heroId = 0;
            levelId = 0;
            return false;
        }

        public static bool GetGuide5v5HeroIDAndLevelID(uint heroType, out uint heroId, out int levelId)
        {
            if (heroType == GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_NEWBIE_RECOMMEND_HEROTYPE1))
            {
                heroId = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_5V5_GUIDE_HERO);
                levelId = (int) GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_5V5_GUIDE_LEVEL);
                return true;
            }
            if (heroType == GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_NEWBIE_RECOMMEND_HEROTYPE2))
            {
                heroId = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_5V5_GUIDE_HERO_AD);
                levelId = (int) GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_5V5_GUIDE_LEVEL_AD);
                return true;
            }
            if (heroType == GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_NEWBIE_RECOMMEND_HEROTYPE3))
            {
                heroId = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_5V5_GUIDE_HERO_AP);
                levelId = (int) GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_5V5_GUIDE_LEVEL_AP);
                return true;
            }
            heroId = 0;
            levelId = 0;
            return false;
        }

        public override void Init()
        {
            base.Init();
            for (int i = 0; i < this.GuideFormPathList.Length; i++)
            {
                string str = string.Format("Newbie/Form_{0}.prefab", ((EBattleGuideFormType) i).ToString("G"));
                this.GuideFormPathList[i] = "UGUI/Form/System/" + str;
            }
            this.m_BannerIntroDialog = new NewbieBannerIntroDialog();
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Newbie_CloseIntroForm, new CUIEventManager.OnUIEventHandler(this.onCloseIntro));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Newbie_CloseIntroForm2, new CUIEventManager.OnUIEventHandler(this.onCloseIntro2));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Newbie_CloseGestureGuide, new CUIEventManager.OnUIEventHandler(this.onCloseGesture));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Newbie_CloseJoyStickGuide, new CUIEventManager.OnUIEventHandler(this.onCloseJoyStick));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Newbie_CloseSettle, new CUIEventManager.OnUIEventHandler(this.onCloseSettle));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Newbie_ConfirmAdvanceGuide, new CUIEventManager.OnUIEventHandler(this.EnterAdvanceGuide));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Newbie_RejectAdvanceGuide, new CUIEventManager.OnUIEventHandler(this.RejectAdvanceGuide));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Newbie_CloseTyrantAlert, new CUIEventManager.OnUIEventHandler(this.onCloseTyrantAlert));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Newbie_CloseTyrantTip, new CUIEventManager.OnUIEventHandler(this.onCloseTyrantTip));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Newbie_CloseTyrantTip2, new CUIEventManager.OnUIEventHandler(this.onCloseTyrantTip2));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Newbie_CloseSkillGesture, new CUIEventManager.OnUIEventHandler(this.onCloseSkillGesture));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Newbie_OldPlayerFirstFormClose, new CUIEventManager.OnUIEventHandler(this.onOldPlayerFirstFormClose));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnSkillButtonUp, new CUIEventManager.OnUIEventHandler(this.OnSkillButtonUp));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Newbie_BannerIntroDlg_ClickPrePage, new CUIEventManager.OnUIEventHandler(this.OnClickToPrePage));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Newbie_BannerIntroDlg_ClickNextPage, new CUIEventManager.OnUIEventHandler(this.OnClickToNextPage));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Newbie_BannerIntroDlg_DragStart, new CUIEventManager.OnUIEventHandler(this.OnDragStart));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Newbie_BannerIntroDlg_DragEnd, new CUIEventManager.OnUIEventHandler(this.OnDragEnd));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Newbie_BannerIntroDlg_ClickConfirm, new CUIEventManager.OnUIEventHandler(this.OnDialogClickConfirm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Newbie_BannerIntroDlg_Close, new CUIEventManager.OnUIEventHandler(this.OnDialogClose));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Newbie_BannerIntroDlg_OnMoveTimeUp, new CUIEventManager.OnUIEventHandler(this.onMoveTimeUp));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Newbie_ClickVictoryTips, new CUIEventManager.OnUIEventHandler(this.onClickVictoryTips));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Newbie_ClickProfitContinue, new CUIEventManager.OnUIEventHandler(this.onClickProfitContinue));
        }

        public static bool Is1v1GuideLevel(int levelId)
        {
            if (((levelId != GuideLevel1v1Tank) && (levelId != GuideLevel1v1AD)) && (levelId != GuideLevel1v1AP))
            {
                return false;
            }
            return true;
        }

        public static bool Is5v5GuideLevel(int levelId)
        {
            if (((levelId != GuideLevel5v5Tank) && (levelId != GuideLevel5v5AD)) && (levelId != GuideLevel5v5AP))
            {
                return false;
            }
            return true;
        }

        public static bool IsCastingGuideLevel(int levelId)
        {
            return (levelId == GuideLevelIDCasting);
        }

        private void onClickProfitContinue(CUIEvent uiEvent)
        {
            if (!this.bReEntry)
            {
                this.OpenNewbieSettle();
            }
            else
            {
                this.OpenTrainLevelSettle();
            }
        }

        private void OnClickToNextPage(CUIEvent uiEvent)
        {
            this.m_BannerIntroDialog.MoveToNextPage();
        }

        private void OnClickToPrePage(CUIEvent uiEvent)
        {
            this.m_BannerIntroDialog.MoveToPrePage();
        }

        private void onClickVictoryTips(CUIEvent uiEvt)
        {
            uint tagUInt = uiEvt.m_eventParams.tagUInt;
            CUICommonSystem.OpenUrl(uiEvt.m_eventParams.tagStr, true, 0);
            uiEvt.m_srcWidget.get_transform().get_parent().FindChild("Panel_Guide").get_gameObject().CustomSetActive(false);
        }

        private void onCloseAwardTips(CUIEvent evt)
        {
            MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.onEntryTrainLevelEntry, new uint[0]);
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Guide_CloseTrainLevel_Award, new CUIEventManager.OnUIEventHandler(this.onCloseAwardTips));
        }

        private void onCloseGesture(CUIEvent uiEvent)
        {
            this.CloseFormShared(EBattleGuideFormType.Gesture);
        }

        private void onCloseIntro(CUIEvent uiEvent)
        {
            this.CloseFormShared(EBattleGuideFormType.Intro);
        }

        private void onCloseIntro2(CUIEvent uiEvent)
        {
            this.CloseFormShared(EBattleGuideFormType.Intro_2);
        }

        private void onCloseJoyStick(CUIEvent uiEvent)
        {
            if (Singleton<CUIManager>.GetInstance().GetForm(this.QueryFormPath(EBattleGuideFormType.JoyStick)) != null)
            {
                this.CloseFormShared(EBattleGuideFormType.JoyStick);
            }
        }

        private void onCloseSettle(CUIEvent uiEvent)
        {
            DynamicShadow.DisableAllDynamicShadows();
            uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x36).dwConfValue;
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            DebugHelper.Assert(curLvelContext != null, "Battle Level Context is NULL!!");
            if ((dwConfValue != 0) && (curLvelContext.m_mapID != AdvanceGuideLevelID))
            {
                Singleton<CUIManager>.instance.OpenForm("UGUI/Form/System/Newbie/Form_Intro_3V3.prefab", false, true);
            }
            else
            {
                this.CloseSettle();
            }
        }

        private void onCloseSkillGesture(CUIEvent uiEvent)
        {
            if (uiEvent != null)
            {
                SkillSlotType skillSlotType = uiEvent.m_eventParams.m_skillSlotType;
                if (this.m_lastFormType == EBattleGuideFormType.SkillGesture2Cancel)
                {
                    string formPath = this.GuideFormPathList[(int) this.m_lastFormType];
                    CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(formPath);
                    if (form != null)
                    {
                        form.Hide(enFormHideFlag.HideByCustom, true);
                        List<GameObject> list = Singleton<BattleSkillHudControl>.GetInstance().QuerySkillButtons(SkillSlotType.SLOT_SKILL_2, false);
                        if (list.Count > 0)
                        {
                            GameObject obj2 = list[0];
                            this.showSkillButton(obj2.get_transform().FindChild("Present").get_gameObject(), true);
                        }
                    }
                }
                else
                {
                    this.CloseFormShared(this.m_lastFormType);
                }
                Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
                if (hostPlayer != null)
                {
                    PoolObjHandle<ActorRoot> captain = hostPlayer.Captain;
                    if ((captain != 0) && (captain.handle.EffectControl != null))
                    {
                        captain.handle.EffectControl.EndSkillGestureEffect();
                    }
                }
                List<GameObject> list2 = Singleton<BattleSkillHudControl>.GetInstance().QuerySkillButtons(skillSlotType, false);
                if ((list2 != null) && (list2.Count > 0))
                {
                    GameObject obj3 = list2[0];
                    if (obj3 != null)
                    {
                        Transform transform = obj3.get_transform().FindChild("Present");
                        DebugHelper.Assert(transform != null);
                        if (transform != null)
                        {
                            transform.get_gameObject().CustomSetActive(true);
                        }
                    }
                }
            }
        }

        private void onCloseTyrantAlert(CUIEvent uiEvent)
        {
            this.CloseFormShared(EBattleGuideFormType.BaojunAlert);
        }

        private void onCloseTyrantTip(CUIEvent uiEvent)
        {
            this.CloseFormShared(EBattleGuideFormType.BaojunTips);
        }

        private void onCloseTyrantTip2(CUIEvent uiEvent)
        {
            this.CloseFormShared(EBattleGuideFormType.BaojunTips2);
        }

        private void OnDialogClickConfirm(CUIEvent uiEvent)
        {
            int tag = uiEvent.m_eventParams.tag;
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                if ((tag != 0) && this.m_BannerIntroDialog.CanSetClientBit())
                {
                    masterRoleInfo.SetClientBits(tag, true, true);
                }
                this.m_BannerIntroDialog.Confirm();
            }
        }

        private void OnDialogClose(CUIEvent uiEvent)
        {
            this.m_BannerIntroDialog.Clear();
        }

        private void OnDragEnd(CUIEvent uiEvent)
        {
            this.m_BannerIntroDialog.DragEnd(uiEvent.m_pointerEventData.get_position());
        }

        private void OnDragStart(CUIEvent uiEvent)
        {
            this.m_BannerIntroDialog.DragStart(uiEvent.m_pointerEventData.get_position());
        }

        private void onMoveTimeUp(CUIEvent uiEvent)
        {
            this.m_BannerIntroDialog.AutoMove();
        }

        private void onOldPlayerFirstFormClose(CUIEvent uiEvent)
        {
            if (uiEvent != null)
            {
                Singleton<CUIManager>.instance.CloseForm(uiEvent.m_srcFormScript);
            }
        }

        private void OnSkillButtonUp(CUIEvent uiEvent)
        {
            if (this.m_lastFormType == EBattleGuideFormType.SkillGesture2Cancel)
            {
                if (Singleton<CBattleSystem>.GetInstance().FightForm.m_skillButtonManager.IsIndicatorInCancelArea())
                {
                    this.CloseFormShared(this.m_lastFormType);
                }
                else
                {
                    string formPath = this.GuideFormPathList[(int) this.m_lastFormType];
                    CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(formPath);
                    if (form != null)
                    {
                        form.Appear(enFormHideFlag.HideByCustom, true);
                        List<GameObject> list = Singleton<BattleSkillHudControl>.GetInstance().QuerySkillButtons(SkillSlotType.SLOT_SKILL_2, false);
                        if (list.Count > 0)
                        {
                            GameObject obj2 = list[0];
                            this.showSkillButton(obj2.get_transform().FindChild("Present").get_gameObject(), false);
                        }
                    }
                }
            }
        }

        private void OnSkillGestEffTimer(int inTimeSeq)
        {
            PoolObjHandle<ActorRoot> captain = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer().Captain;
            SkillSlotType curSkillSlotType = Singleton<CBattleSystem>.GetInstance().FightForm.GetCurSkillSlotType();
            if (this.m_lastFormType == EBattleGuideFormType.SkillGesture2)
            {
                if (curSkillSlotType == SkillSlotType.SLOT_SKILL_2)
                {
                    return;
                }
                if ((captain != 0) && (captain.handle.EffectControl != null))
                {
                    captain.handle.EffectControl.StartSkillGestureEffect();
                }
            }
            else if (this.m_lastFormType == EBattleGuideFormType.SkillGesture2Cancel)
            {
                if (curSkillSlotType == SkillSlotType.SLOT_SKILL_2)
                {
                    return;
                }
                if ((captain != 0) && (captain.handle.EffectControl != null))
                {
                    captain.handle.EffectControl.StartSkillGestureEffectCancel();
                }
            }
            else if (this.m_lastFormType == EBattleGuideFormType.SkillGesture3)
            {
                if (curSkillSlotType == SkillSlotType.SLOT_SKILL_3)
                {
                    return;
                }
                if ((captain != 0) && (captain.handle.EffectControl != null))
                {
                    captain.handle.EffectControl.StartSkillGestureEffect3();
                }
            }
            Singleton<CTimerManager>.instance.RemoveTimer(inTimeSeq);
        }

        public bool OpenBannerDlgByBannerGuideId(uint Id, CUIEvent uiEventParam = new CUIEvent(), bool bShowChooseGuideNextTime = false)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            NewbieGuideBannerGuideConf dataByKey = GameDataMgr.newbieBannerGuideDatabin.GetDataByKey(Id);
            if ((masterRoleInfo == null) || (dataByKey == null))
            {
                return false;
            }
            int inIndex = NewbieGuideManager.ConvertNewbieBitToClientBit(dataByKey.dwGuideBit);
            if ((inIndex != 0) && (masterRoleInfo.IsClientBitsSet(inIndex) || CSysDynamicBlock.bNewbieBlocked))
            {
                return false;
            }
            int imgNum = 0;
            for (int i = 0; i < dataByKey.astPicPath.Length; i++)
            {
                if (dataByKey.astPicPath[i].dwID == 0)
                {
                    break;
                }
                imgNum++;
            }
            string[] imgPath = new string[imgNum];
            for (int j = 0; j < imgNum; j++)
            {
                imgPath[j] = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Newbie_Dir, dataByKey.astPicPath[j].dwID.ToString());
            }
            this.OpenBannerIntroDialog(inIndex, imgPath, imgNum, uiEventParam, dataByKey.szTitleName, dataByKey.szBtnName, true, bShowChooseGuideNextTime);
            return true;
        }

        public void OpenBannerIntroDialog(int clientBit, string[] imgPath, int imgNum, CUIEvent uiEventParam = new CUIEvent(), string title = new string(), string btnName = new string(), bool bAutoMove = true, bool bShowChooseGuideNextTime = false)
        {
            this.m_BannerIntroDialog.OpenForm(clientBit, imgPath, imgNum, uiEventParam, title, btnName, bAutoMove, bShowChooseGuideNextTime);
        }

        public void OpenFormShared(EBattleGuideFormType inFormType, int delayTime = 0, bool bPauseGame = true)
        {
            if ((inFormType > EBattleGuideFormType.Invalid) && (inFormType < EBattleGuideFormType.Count))
            {
                Singleton<CUILoadingSystem>.instance.HideLoading();
                this.curOpenForm = Singleton<CUIManager>.GetInstance().OpenForm(this.GuideFormPathList[(int) inFormType], false, true);
                if ((delayTime != 0) && (this.curOpenForm != null))
                {
                    Transform transform = this.curOpenForm.get_transform().FindChild("Panel_Interactable");
                    if (transform != null)
                    {
                        transform.get_gameObject().CustomSetActive(false);
                        Singleton<CTimerManager>.GetInstance().AddTimer(delayTime, 1, new CTimer.OnTimeUpHandler(this.ShowInteractable));
                    }
                }
                Singleton<CTimerManager>.GetInstance().AddTimer(TIME_OUT, 1, new CTimer.OnTimeUpHandler(CBattleGuideManager.TimeOutDelegate));
                switch (inFormType)
                {
                    case EBattleGuideFormType.BigMapGuide:
                    case EBattleGuideFormType.SelectModeGuide:
                    case EBattleGuideFormType.BaojunAlert:
                    case EBattleGuideFormType.SkillGesture2Cancel:
                        break;

                    default:
                        this.UpdateGamePausing(bPauseGame);
                        break;
                }
                if (Singleton<GameStateCtrl>.instance.isBattleState)
                {
                    if (inFormType == EBattleGuideFormType.SkillGesture2)
                    {
                        List<GameObject> list = Singleton<BattleSkillHudControl>.GetInstance().QuerySkillButtons(SkillSlotType.SLOT_SKILL_2, false);
                        if (list.Count > 0)
                        {
                            GameObject obj2 = list[0];
                            this.showSkillButton(obj2.get_transform().FindChild("Present").get_gameObject(), false);
                            Singleton<CTimerManager>.instance.AddTimer(500, 1, new CTimer.OnTimeUpHandler(this.OnSkillGestEffTimer), false);
                        }
                    }
                    else if (inFormType == EBattleGuideFormType.SkillGesture2Cancel)
                    {
                        List<GameObject> list2 = Singleton<BattleSkillHudControl>.GetInstance().QuerySkillButtons(SkillSlotType.SLOT_SKILL_2, false);
                        if (list2.Count > 0)
                        {
                            GameObject obj3 = list2[0];
                            this.showSkillButton(obj3.get_transform().FindChild("Present").get_gameObject(), false);
                            Singleton<CTimerManager>.instance.AddTimer(500, 1, new CTimer.OnTimeUpHandler(this.OnSkillGestEffTimer), false);
                            GameObject obj4 = GameObject.Find("Design");
                            if (obj4 != null)
                            {
                                GlobalTrigger component = obj4.GetComponent<GlobalTrigger>();
                                if (component != null)
                                {
                                    component.BindSkillCancelListener();
                                }
                            }
                        }
                    }
                    else if (inFormType == EBattleGuideFormType.SkillGesture3)
                    {
                        List<GameObject> list3 = Singleton<BattleSkillHudControl>.GetInstance().QuerySkillButtons(SkillSlotType.SLOT_SKILL_3, false);
                        if (list3.Count > 0)
                        {
                            GameObject obj5 = list3[0];
                            this.showSkillButton(obj5.get_transform().FindChild("Present").get_gameObject(), false);
                            Singleton<CTimerManager>.instance.AddTimer(500, 1, new CTimer.OnTimeUpHandler(this.OnSkillGestEffTimer), false);
                            GameObject obj6 = GameObject.Find("Design");
                            if (obj6 != null)
                            {
                                GlobalTrigger trigger2 = obj6.GetComponent<GlobalTrigger>();
                                if (trigger2 != null)
                                {
                                    trigger2.UnbindSkillCancelListener();
                                }
                            }
                        }
                    }
                }
                this.m_lastFormType = inFormType;
            }
        }

        private void OpenNewbieSettle()
        {
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                if (Is1v1GuideLevel(curLvelContext.m_mapID))
                {
                    if (!this.bReEntry)
                    {
                        uint firstHeroId = masterRoleInfo.GetFirstHeroId();
                        this.ShowNewbiePassedHero(firstHeroId, true);
                        if (<>f__am$cache28 == null)
                        {
                            <>f__am$cache28 = delegate (int seq) {
                                Singleton<CNewbieAchieveSys>.GetInstance().ShowAchieve(enNewbieAchieve.COM_ACNT_CLIENT_BITS_TYPE_GET_HERO);
                            };
                        }
                        Singleton<CTimerManager>.GetInstance().AddTimer(0x3e8, 1, <>f__am$cache28);
                    }
                }
                else if (Is5v5GuideLevel(curLvelContext.m_mapID))
                {
                    uint heroId = masterRoleInfo.GetGuideLevel2FadeHeroId();
                    this.ShowNewbiePassedHero(heroId, false);
                }
                else if (IsCastingGuideLevel(curLvelContext.m_mapID))
                {
                    CUIFormScript formScript = Singleton<CUIManager>.GetInstance().OpenForm(NEWBIE_GOLDAWARD_FORM_PATH, false, true);
                    if (formScript != null)
                    {
                        Transform transform = formScript.get_transform().FindChild("itemCell");
                        if (transform != null)
                        {
                            CUseable itemUseable = CUseableManager.CreateCoinUseable(RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_PVPCOIN, (int) GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_CORONA_COIN));
                            CUICommonSystem.SetItemCell(formScript, transform.get_gameObject(), itemUseable, true, false, false, false);
                        }
                    }
                }
            }
        }

        public void OpenOldPlayerFirstForm()
        {
            if ((!ms_bOldPlayerFormOpened && MonoSingleton<NewbieGuideManager>.GetInstance().newbieGuideEnable) && (Singleton<CUIManager>.instance.GetForm("UGUI/Form/System/Newbie/Form_OldTip.prefab") == null))
            {
                Singleton<CUIManager>.instance.OpenForm("UGUI/Form/System/Newbie/Form_OldTip.prefab", false, true);
                ms_bOldPlayerFormOpened = true;
            }
        }

        public void OpenSettle()
        {
            if (this.bReEntry)
            {
                this.UpdateGamePausing(false);
                Singleton<CMatchingSystem>.GetInstance().OpenPvPEntry(enPvPEntryFormWidget.GuideBtnGroup);
            }
            this.ShowPersonalProfit(true);
        }

        private void OpenTrainLevelSettle()
        {
            DebugHelper.Assert(Singleton<BattleLogic>.GetInstance().GetCurLvelContext() != null, "Battle Level Context is NULL!!");
            if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo() != null)
            {
                Singleton<CMatchingSystem>.GetInstance().OpenPvPEntry(enPvPEntryFormWidget.GuideBtnGroup);
                ListView<CUseable> useableListFromReward = CUseableManager.GetUseableListFromReward(this.TrainLevelReward);
                ListView<CUseable> inList = new ListView<CUseable>();
                int count = useableListFromReward.Count;
                for (int i = 0; i < count; i++)
                {
                    if ((useableListFromReward[i].MapRewardType != COM_REWARDS_TYPE.COM_REWARDS_TYPE_HONOUR) && (useableListFromReward[i].MapRewardType != COM_REWARDS_TYPE.COM_REWARDS_TYPE_SYMBOLCOIN))
                    {
                        inList.Add(useableListFromReward[i]);
                    }
                }
                if (inList.Count != 0)
                {
                    Singleton<CUIManager>.GetInstance().OpenAwardTip(LinqS.ToArray<CUseable>(inList), Singleton<CTextManager>.GetInstance().GetText("TrainLevel_Settel_Tile0"), true, enUIEventID.Battle_Guide_CloseTrainLevel_Award, false, false, "Form_Award");
                    Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Guide_CloseTrainLevel_Award, new CUIEventManager.OnUIEventHandler(this.onCloseAwardTips));
                }
            }
        }

        public void PauseGame(object sender, bool bEffectTimeScale = true)
        {
            if (sender != null)
            {
                if (this.m_pauseGameStack.ContainsKey(sender))
                {
                    Dictionary<object, int> dictionary;
                    object obj2;
                    int num = dictionary[obj2];
                    (dictionary = this.m_pauseGameStack)[obj2 = sender] = num + 1;
                }
                else
                {
                    this.m_pauseGameStack.Add(sender, 1);
                }
                this.UpdateGamePause(bEffectTimeScale);
            }
        }

        public string QueryFormPath(EBattleGuideFormType inFormType)
        {
            return this.GuideFormPathList[(int) inFormType];
        }

        private void RejectAdvanceGuide(CUIEvent uiEvent)
        {
            this.CloseSettle();
        }

        public void resetPause()
        {
            this.m_pauseGameStack.Clear();
            this.bPauseGame = false;
            Singleton<FrameSynchr>.instance.SetSynchrRunning(!this.bPauseGame);
            Time.set_timeScale(1f);
        }

        public void ResumeGame(object sender)
        {
            if ((sender != null) && this.m_pauseGameStack.ContainsKey(sender))
            {
                Dictionary<object, int> dictionary;
                object obj2;
                int num = dictionary[obj2];
                (dictionary = this.m_pauseGameStack)[obj2 = sender] = --num;
                if (num == 0)
                {
                    this.m_pauseGameStack.Remove(sender);
                }
                this.UpdateGamePause(true);
            }
        }

        private void SetExpProfit(CUIFormScript profitForm)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                ResAcntPvpExpInfo dataByKey = GameDataMgr.acntPvpExpDatabin.GetDataByKey((uint) ((byte) masterRoleInfo.PvpLevel));
                if (dataByKey != null)
                {
                    COMDT_ACNT_INFO acntInfo = Singleton<BattleStatistic>.GetInstance().acntInfo;
                    if (acntInfo != null)
                    {
                        GameObject obj2 = profitForm.m_formWidgets[1];
                        obj2.get_transform().FindChild("PlayerName").get_gameObject().GetComponent<Text>().set_text(masterRoleInfo.Name);
                        obj2.get_transform().FindChild("PlayerLv").get_gameObject().GetComponent<Text>().set_text(string.Format("Lv.{0}", masterRoleInfo.PvpLevel));
                        obj2.get_transform().FindChild("PvpExpTxt").get_gameObject().GetComponent<Text>().set_text(string.Format("{0}/{1}", acntInfo.dwPvpExp, dataByKey.dwNeedExp));
                        obj2.get_transform().FindChild("AddPvpExpTxt").get_gameObject().GetComponent<Text>().set_text(string.Format("+{0}", acntInfo.dwPvpSettleExp));
                        obj2.get_transform().FindChild("Bar").get_gameObject().CustomSetActive(acntInfo.dwPvpSettleExp != 0);
                        if (acntInfo.dwPvpSettleExp <= 0)
                        {
                            CUICommonSystem.SetObjActive(obj2.get_transform().FindChild("text"), false);
                        }
                        RectTransform component = obj2.get_transform().FindChild("PvpExpSliderBg/BasePvpExpSlider").get_gameObject().GetComponent<RectTransform>();
                        RectTransform transform2 = obj2.get_transform().FindChild("PvpExpSliderBg/AddPvpExpSlider").get_gameObject().GetComponent<RectTransform>();
                        if (acntInfo.dwPvpSettleExp > 0)
                        {
                            Singleton<CSoundManager>.GetInstance().PostEvent("UI_count_jingyan", null);
                        }
                        int num = (int) (acntInfo.dwPvpExp - acntInfo.dwPvpSettleExp);
                        lvUpGrade = (num >= 0) ? 0 : acntInfo.dwPvpLv;
                        float num2 = Mathf.Max(0f, ((float) num) / ((float) dataByKey.dwNeedExp));
                        float num3 = Mathf.Max(0f, ((num >= 0) ? ((float) acntInfo.dwPvpSettleExp) : ((float) acntInfo.dwPvpExp)) / ((float) dataByKey.dwNeedExp));
                        component.set_sizeDelta(new Vector2(num2 * 220.3f, component.get_sizeDelta().y));
                        transform2.set_sizeDelta(new Vector2(num2 * 220.3f, transform2.get_sizeDelta().y));
                        this.expFrom = num2;
                        this.expTo = num2 + num3;
                        this.expTweenRect = transform2;
                        component.get_gameObject().CustomSetActive(num >= 0);
                        CUIHttpImageScript script = obj2.get_transform().FindChild("HeadImage").GetComponent<CUIHttpImageScript>();
                        Image image = obj2.get_transform().FindChild("NobeIcon").GetComponent<Image>();
                        Image image2 = obj2.get_transform().FindChild("HeadFrame").GetComponent<Image>();
                        if (!CSysDynamicBlock.bSocialBlocked)
                        {
                            string headUrl = masterRoleInfo.HeadUrl;
                            script.SetImageUrl(headUrl);
                            MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(image, (int) masterRoleInfo.GetNobeInfo().stGameVipClient.dwCurLevel, false);
                            MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(image2, (int) masterRoleInfo.GetNobeInfo().stGameVipClient.dwHeadIconId);
                        }
                        else
                        {
                            MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(image, 0, false);
                        }
                    }
                }
            }
        }

        private void SetGoldCoinProfit(CUIFormScript profitForm)
        {
            if (Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo() != null)
            {
                COMDT_ACNT_INFO acntInfo = Singleton<BattleStatistic>.GetInstance().acntInfo;
                if (acntInfo != null)
                {
                    GameObject obj2 = profitForm.m_formWidgets[2];
                    Text component = obj2.get_transform().FindChild("GoldNum").GetComponent<Text>();
                    component.set_text("+0");
                    this.coinFrom = 0f;
                    uint num = 0;
                    for (int i = 0; i < this.TrainLevelReward.bNum; i++)
                    {
                        if (this.TrainLevelReward.astRewardDetail[i].bType == 11)
                        {
                            num += this.TrainLevelReward.astRewardDetail[i].stRewardInfo.dwPvpCoin;
                        }
                    }
                    num += acntInfo.dwPvpSettleCoin;
                    if (num <= 0)
                    {
                        CUICommonSystem.SetObjActive(obj2.get_transform().FindChild("text"), false);
                    }
                    this.coinTo = num;
                    this.coinTweenText = component;
                }
            }
        }

        private void SetMapInfo(CUIFormScript profitForm)
        {
            GameObject obj2 = profitForm.m_formWidgets[6];
            obj2.CustomSetActive(true);
            Text component = obj2.get_transform().FindChild("GameType").GetComponent<Text>();
            Text text2 = obj2.get_transform().FindChild("MapName").GetComponent<Text>();
            string text = Singleton<CTextManager>.instance.GetText("Battle_Settle_Game_Type_Train");
            string szName = string.Empty;
            SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
            if (curLvelContext != null)
            {
                uint mapID = (uint) curLvelContext.m_mapID;
                ResLevelCfgInfo dataByKey = GameDataMgr.levelDatabin.GetDataByKey(mapID);
                if (dataByKey != null)
                {
                    szName = dataByKey.szName;
                }
                component.set_text(text);
                text2.set_text(szName);
            }
        }

        private void SetSymbolCoinProfit(CUIFormScript profitForm)
        {
            GameObject obj2 = profitForm.m_formWidgets[3];
            Text component = obj2.get_transform().FindChild("CoinNum").GetComponent<Text>();
            component.set_text("+0");
            this.symbolCoinFrom = 0f;
            uint num = 0;
            for (int i = 0; i < this.TrainLevelReward.bNum; i++)
            {
                if (this.TrainLevelReward.astRewardDetail[i].bType == 14)
                {
                    num += this.TrainLevelReward.astRewardDetail[i].stRewardInfo.dwSymbolCoin;
                }
            }
            this.symbolCoinTo = num;
            this.symbolCoinTweenText = component;
        }

        public void ShowBuyEuipPanel(bool bShow)
        {
            Singleton<CUIManager>.GetInstance().GetForm(FightForm.s_battleUIForm).GetWidget(0x2d).CustomSetActive(bShow);
        }

        private void ShowInteractable(int timerSequence)
        {
            Singleton<CTimerManager>.instance.RemoveTimer(new CTimer.OnTimeUpHandler(this.ShowInteractable));
            if (this.curOpenForm != null)
            {
                Transform transform = this.curOpenForm.get_transform().FindChild("Panel_Interactable");
                if (transform != null)
                {
                    transform.get_gameObject().CustomSetActive(true);
                    CUIAnimatorScript component = transform.GetComponent<CUIAnimatorScript>();
                    if (component != null)
                    {
                        component.PlayAnimator("Interactable_Enabled");
                    }
                }
            }
        }

        private void ShowNewbiePassedHero(uint heroId, bool bImage1)
        {
            CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/System/Newbie/Form_NewbieSettle.prefab", false, true);
            if (script != null)
            {
                DynamicShadow.EnableDynamicShow(script.get_gameObject(), true);
            }
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if ((masterRoleInfo != null) && (script != null))
            {
                int heroWearSkinId = (int) masterRoleInfo.GetHeroWearSkinId(heroId);
                string objectName = CUICommonSystem.GetHeroPrefabPath(heroId, heroWearSkinId, true).ObjectName;
                GameObject model = script.get_transform().Find("3DImage").get_gameObject().GetComponent<CUI3DImageScript>().AddGameObject(objectName, false, false);
                CHeroAnimaSystem instance = Singleton<CHeroAnimaSystem>.GetInstance();
                instance.Set3DModel(model);
                instance.InitAnimatList();
                instance.InitAnimatSoundList(heroId, (uint) heroWearSkinId);
                instance.OnModePlayAnima("Cheer");
                Transform transform = script.get_transform().FindChild("Panel_NewHero/MaskBlack/Text");
                if (transform != null)
                {
                    Text component = transform.GetComponent<Text>();
                    if (component != null)
                    {
                        ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(heroId);
                        if (dataByKey != null)
                        {
                            component.set_text(StringHelper.UTF8BytesToString(ref dataByKey.szName));
                        }
                    }
                }
                if (bImage1)
                {
                    Transform transform2 = script.get_transform().FindChild("Panel_NewHero/Image1");
                    if (transform2 != null)
                    {
                        transform2.get_gameObject().CustomSetActive(true);
                    }
                }
                else
                {
                    Transform transform3 = script.get_transform().FindChild("Panel_NewHero/Image2");
                    if (transform3 != null)
                    {
                        transform3.get_gameObject().CustomSetActive(true);
                    }
                }
            }
        }

        public void ShowPersonalProfit(bool win)
        {
            if (Singleton<CUIManager>.GetInstance().GetForm(PROFIT_FORM_NAME) == null)
            {
                CUIFormScript profitForm = Singleton<CUIManager>.GetInstance().OpenForm(PROFIT_FORM_NAME, false, true);
                if (profitForm != null)
                {
                    GameObject obj2 = profitForm.m_formWidgets[0];
                    obj2.get_transform().FindChild("Win").get_gameObject().CustomSetActive(win);
                    obj2.get_transform().FindChild("Lose").get_gameObject().CustomSetActive(!win);
                    this.SetExpProfit(profitForm);
                    this.SetGoldCoinProfit(profitForm);
                    this.SetSymbolCoinProfit(profitForm);
                    this.SetMapInfo(profitForm);
                    this.DoCoinAndExpTween();
                }
            }
        }

        private void showSkillButton(GameObject skillPresent, bool bShow)
        {
            if (skillPresent != null)
            {
                CanvasGroup component = null;
                component = skillPresent.GetComponent<CanvasGroup>();
                if (component == null)
                {
                    component = skillPresent.AddComponent<CanvasGroup>();
                }
                if (bShow)
                {
                    component.set_alpha(1f);
                }
                else
                {
                    component.set_alpha(0f);
                }
            }
        }

        public void StartSettle(COMDT_SETTLE_RESULT_DETAIL detail)
        {
            this.TrainLevelReward = detail.stReward;
            this.m_bWin = detail.stGameInfo.bGameResult == 1;
            Singleton<CBattleSystem>.GetInstance().FightForm.ShowWinLosePanel(this.m_bWin);
        }

        public static void TimeOutDelegate(int timerSequence)
        {
            Singleton<CBattleGuideManager>.GetInstance().CloseFormShared(Singleton<CBattleGuideManager>.GetInstance().m_lastFormType);
        }

        public override void UnInit()
        {
            this.m_BannerIntroDialog = null;
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Newbie_CloseIntroForm, new CUIEventManager.OnUIEventHandler(this.onCloseIntro));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Newbie_CloseIntroForm2, new CUIEventManager.OnUIEventHandler(this.onCloseIntro2));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Newbie_CloseGestureGuide, new CUIEventManager.OnUIEventHandler(this.onCloseGesture));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Newbie_CloseJoyStickGuide, new CUIEventManager.OnUIEventHandler(this.onCloseJoyStick));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Newbie_CloseSettle, new CUIEventManager.OnUIEventHandler(this.onCloseSettle));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Newbie_ConfirmAdvanceGuide, new CUIEventManager.OnUIEventHandler(this.EnterAdvanceGuide));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Newbie_RejectAdvanceGuide, new CUIEventManager.OnUIEventHandler(this.RejectAdvanceGuide));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Newbie_CloseTyrantAlert, new CUIEventManager.OnUIEventHandler(this.onCloseTyrantAlert));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Newbie_CloseTyrantTip, new CUIEventManager.OnUIEventHandler(this.onCloseTyrantTip));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Newbie_CloseTyrantTip2, new CUIEventManager.OnUIEventHandler(this.onCloseTyrantTip2));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Newbie_CloseSkillGesture, new CUIEventManager.OnUIEventHandler(this.onCloseSkillGesture));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Newbie_OldPlayerFirstFormClose, new CUIEventManager.OnUIEventHandler(this.onOldPlayerFirstFormClose));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnSkillButtonUp, new CUIEventManager.OnUIEventHandler(this.OnSkillButtonUp));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Newbie_BannerIntroDlg_ClickPrePage, new CUIEventManager.OnUIEventHandler(this.OnClickToPrePage));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Newbie_BannerIntroDlg_ClickNextPage, new CUIEventManager.OnUIEventHandler(this.OnClickToNextPage));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Newbie_BannerIntroDlg_DragStart, new CUIEventManager.OnUIEventHandler(this.OnDragStart));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Newbie_BannerIntroDlg_DragEnd, new CUIEventManager.OnUIEventHandler(this.OnDragEnd));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Newbie_BannerIntroDlg_ClickConfirm, new CUIEventManager.OnUIEventHandler(this.OnDialogClickConfirm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Newbie_BannerIntroDlg_Close, new CUIEventManager.OnUIEventHandler(this.OnDialogClose));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Newbie_BannerIntroDlg_OnMoveTimeUp, new CUIEventManager.OnUIEventHandler(this.onMoveTimeUp));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Newbie_ClickVictoryTips, new CUIEventManager.OnUIEventHandler(this.onClickVictoryTips));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Newbie_ClickProfitContinue, new CUIEventManager.OnUIEventHandler(this.onClickProfitContinue));
            base.UnInit();
        }

        private void UpdateGamePause(bool bEffectTimeScale)
        {
            this.bPauseGame = this.m_pauseGameStack.Count != 0;
            Singleton<FrameSynchr>.instance.SetSynchrRunning(!this.bPauseGame);
            if (bEffectTimeScale)
            {
                DebugHelper.Assert(!Singleton<FrameSynchr>.instance.bActive, "frame synchr active forbid set timeScale");
                Time.set_timeScale(!this.bPauseGame ? ((float) 1) : ((float) 0));
            }
        }

        private void UpdateGamePausing(bool bPauseGame)
        {
            if (bPauseGame)
            {
                this.PauseGame(this, false);
                Singleton<CUIParticleSystem>.GetInstance().ClearAll(true);
            }
            else
            {
                this.ResumeGame(this);
            }
        }

        public bool bPauseGame
        {
            get
            {
                return this._PauseGame;
            }
            private set
            {
                if (value)
                {
                    Singleton<GameInput>.instance.StopInput();
                }
                this._PauseGame = value;
            }
        }

        public enum EBattleGuideFormType
        {
            Invalid,
            Intro,
            Gesture,
            JoyStick,
            Intro_2,
            BaojunAlert,
            BaojunTips,
            BaojunTips2,
            SkillGesture2,
            SkillGesture3,
            SkillGesture2Cancel,
            BigSkill,
            BlueBuff,
            Duke,
            Grass,
            Heal,
            Intro_5V5,
            Intro_Jungle,
            Intro_LunPan,
            Map_01,
            Map_02,
            Map_03,
            RedBuff,
            Tower,
            XiaoLong,
            Map_BlueBuff,
            Map_RedBuff,
            BigMapGuide,
            SelectModeGuide,
            CustomRecomEquipIntro,
            FingerMovement,
            ADBigSkill,
            APBigSkill,
            BloodReturn,
            Count
        }

        protected enum enNewbieProfitWidgets
        {
            CoinInfo = 2,
            ExpInfo = 1,
            GuildInfo = 4,
            GuildPointMaxTip = 7,
            LadderInfo = 5,
            None = -1,
            PvpMapInfo = 6,
            SymbolCoinInfo = 3,
            WinLoseTitle = 0
        }
    }
}

