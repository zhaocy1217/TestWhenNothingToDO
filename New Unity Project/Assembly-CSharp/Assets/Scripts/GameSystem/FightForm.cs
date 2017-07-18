namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.GameLogic.GameKernal;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class FightForm : IBattleForm
    {
        private CUIFormScript _formBuffSkillScript = null;
        private CUIFormScript _formCameraDragPanel;
        private CUIFormScript _formCameraMoveScript = null;
        private CUIFormScript _formEnemyHeroAtkScript = null;
        private CUIFormScript _formJoystickScript = null;
        private CUIFormScript _formSceneScript = null;
        public CUIFormScript _formScript = null;
        private CUIFormScript _formSkillBtnScript = null;
        private CUIFormScript _formSkillCursorScript = null;
        private CUIJoystickScript _joystick;
        private uint _lastFps;
        private BattleTaskView battleTaskView;
        private GameObject BuffDesc;
        private HeroHeadHud heroHeadHud;
        private Text m_AdTxt;
        private Text m_ApTxt;
        private BattleMisc m_battleMisc;
        private bool m_bOpenMic;
        private bool m_bOpenSpeak;
        private bool m_bShowRealPing;
        private int m_displayPing;
        private BattleDragonView m_dragonView;
        public CEnemyHeroAtkBtn m_enemyHeroAtkBtn;
        private Image m_EpImg;
        private BackToCityProcessBar m_goBackProcessBar;
        private HeroInfoPanel m_heroInfoPanel;
        private PoolObjHandle<ActorRoot> m_HideSelectedHero;
        private CHostHeroDeadInfo m_hostHeroDeadInfo;
        private Image m_HpImg;
        private Text m_HpTxt;
        public bool m_isInBattle;
        private bool m_isSkillDecShow;
        private Text m_MgcDefTxt;
        private GameObject m_objHeroHead;
        private Transform m_OpeankBigMap;
        private Transform m_OpeankSpeakAnim;
        private Transform m_OpenMicObj;
        private Transform m_OpenMicTipObj;
        private Text m_OpenMicTipText;
        private Transform m_OpenSpeakerObj;
        private Transform m_OpenSpeakerTipObj;
        private Text m_OpenSpeakerTipText;
        private CanvasGroup m_panelHeroCanvas;
        private GameObject m_panelHeroInfo;
        private Text m_PhyDefTxt;
        private PoolObjHandle<ActorRoot> m_selectedHero;
        private CBattleShowBuffDesc m_showBuffDesc;
        private SignalPanel m_signalPanel;
        public CSkillButtonManager m_skillButtonManager;
        public ListView<SLOTINFO> m_SkillSlotList = new ListView<SLOTINFO>();
        private int m_Vocetimer;
        private int m_VocetimerFirst;
        private int m_VoiceMictime;
        private int m_VoiceTipsShowTime = 0x7d0;
        public Image m_wifiIcon;
        public int m_wifiIconCheckMaxTicks = 3;
        public int m_wifiIconCheckTicks = 3;
        public Text m_wifiText;
        public int m_wifiTimerInterval = 2;
        private string microphone_path = "UGUI/Sprite/Battle/Battle_btn_Microphone.prefab";
        private string no_microphone_path = "UGUI/Sprite/Battle/Battle_btn_No_Microphone.prefab";
        private string no_voiceIcon_path = "UGUI/Sprite/Battle/Battle_btn_No_voice.prefab";
        public static string s_battleCameraMove = "UGUI/Form/Battle/Part/Form_Battle_Part_CameraMove.prefab";
        public static string s_battleDragonTipForm = "UGUI/Form/Battle/Form_Battle_Dragon_Tips.prefab";
        public static string s_battleJoystick = "UGUI/Form/Battle/Part/Form_Battle_Part_Joystick.prefab";
        public static string s_battleScene = "UGUI/Form/Battle/Part/Form_Battle_Part_Scene.prefab";
        public static string s_battleSkillCursor = "UGUI/Form/Battle/Part/Form_Battle_Part_SkillCursor.prefab";
        public static string s_battleUIForm = "UGUI/Form/Battle/Form_Battle.prefab";
        public static string s_buffSkillFormPath = "UGUI/Form/Battle/Part/Form_Battle_Part_BuffSkill.prefab";
        public static string s_cameraDragPanelPath = "UGUI/Form/Battle/Part/Form_Battle_Part_CameraDragPanel";
        public static string s_enemyHeroAtkFormPath = "UGUI/Form/Battle/Part/Form_Battle_Part_EnemyHeroAtk.prefab";
        public static string s_skillBtnFormPath = "UGUI/Form/Battle/Part/Form_Battle_Part_SkillBtn.prefab";
        public ScoreBoard scoreBoard;
        private ScoreboardPvE scoreboardPvE;
        private GameObject skillTipDesc;
        private SoldierWave soldierWaveView;
        private CStarEvalPanel starEvalPanel;
        private float timeEnergyShortage;
        private float timeNoSkillTarget;
        private float timeSkillBeanShortage;
        private float timeSkillCooldown;
        private CTreasureHud treasureHud;
        public Vector2 UI_world_Factor_Big;
        public Vector2 UI_world_Factor_Small;
        private string voiceIcon_path = "UGUI/Sprite/Battle/Battle_btn_voice.prefab";
        public Vector2 world_UI_Factor_Big;
        public Vector2 world_UI_Factor_Small;

        private void Battle_ActivateForm(CUIEvent uiEvent)
        {
            if (this._formScript != null)
            {
                this._formScript.Appear(enFormHideFlag.HideByCustom, true);
            }
            if (this._formSkillCursorScript != null)
            {
                this._formSkillCursorScript.Appear(enFormHideFlag.HideByCustom, true);
            }
            if (this._formJoystickScript != null)
            {
                this._formJoystickScript.Appear(enFormHideFlag.HideByCustom, true);
            }
            if (this._formSceneScript != null)
            {
                this._formSceneScript.Appear(enFormHideFlag.HideByCustom, true);
            }
            if (this._formCameraMoveScript != null)
            {
                this._formCameraMoveScript.Appear(enFormHideFlag.HideByCustom, true);
            }
            if (this._formSkillBtnScript != null)
            {
                this._formSkillBtnScript.Appear(enFormHideFlag.HideByCustom, true);
            }
            if (this._formBuffSkillScript != null)
            {
                this._formBuffSkillScript.Appear(enFormHideFlag.HideByCustom, true);
            }
            if (this._formEnemyHeroAtkScript != null)
            {
                this._formEnemyHeroAtkScript.Appear(enFormHideFlag.HideByCustom, true);
            }
            if (this._formCameraDragPanel != null)
            {
                this._formCameraDragPanel.Appear(enFormHideFlag.HideByCustom, true);
            }
        }

        private void BattleOpenSpeak(CUIEvent uiEvent, bool bInit = false)
        {
            if (uiEvent != null)
            {
                CPlayerBehaviorStat.Plus(CPlayerBehaviorStat.BehaviorType.Battle_Voice_OpenSpeak);
            }
            if (this.m_OpenSpeakerTipObj != null)
            {
                if (!CFakePvPHelper.bInFakeSelect && MonoSingleton<VoiceSys>.GetInstance().IsBattleSupportVoice())
                {
                    if (!MonoSingleton<VoiceSys>.GetInstance().GlobalVoiceSetting)
                    {
                        if (!bInit)
                        {
                            if (this.m_OpenSpeakerTipText != null)
                            {
                                this.m_OpenSpeakerTipText.set_text(MonoSingleton<VoiceSys>.GetInstance().m_Voice_Server_Not_Open_Tips);
                            }
                            this.m_OpenSpeakerTipObj.get_gameObject().CustomSetActive(true);
                            Singleton<CTimerManager>.instance.ResumeTimer(this.m_Vocetimer);
                        }
                        return;
                    }
                    if (!MonoSingleton<VoiceSys>.GetInstance().IsInVoiceRoom())
                    {
                        if (this.m_OpenSpeakerTipText != null)
                        {
                            this.m_OpenSpeakerTipText.set_text(MonoSingleton<VoiceSys>.GetInstance().m_Voice_Cannot_JoinVoiceRoom);
                        }
                        this.m_OpenSpeakerTipObj.get_gameObject().CustomSetActive(true);
                        Singleton<CTimerManager>.instance.ResumeTimer(this.m_Vocetimer);
                        return;
                    }
                }
                if (this.m_bOpenSpeak)
                {
                    if (this.m_OpenSpeakerTipText != null)
                    {
                        this.m_OpenSpeakerTipText.set_text(MonoSingleton<VoiceSys>.GetInstance().m_Voice_Battle_CloseSpeaker);
                    }
                    MonoSingleton<VoiceSys>.GetInstance().ClosenSpeakers();
                    MonoSingleton<VoiceSys>.GetInstance().CloseMic();
                    this.m_bOpenMic = false;
                    if (this.m_OpenSpeakerObj != null)
                    {
                        CUIUtility.SetImageSprite(this.m_OpenSpeakerObj.GetComponent<Image>(), this.no_voiceIcon_path, null, true, false, false, false);
                    }
                    if (this.m_OpenMicObj != null)
                    {
                        CUIUtility.SetImageSprite(this.m_OpenMicObj.GetComponent<Image>(), this.no_microphone_path, null, true, false, false, false);
                    }
                }
                else
                {
                    if (this.m_OpenSpeakerTipText != null)
                    {
                        this.m_OpenSpeakerTipText.set_text(MonoSingleton<VoiceSys>.GetInstance().m_Voice_Battle_OpenSpeaker);
                    }
                    MonoSingleton<VoiceSys>.GetInstance().OpenSpeakers();
                    if (this.m_OpenSpeakerObj != null)
                    {
                        CUIUtility.SetImageSprite(this.m_OpenSpeakerObj.GetComponent<Image>(), this.voiceIcon_path, null, true, false, false, false);
                    }
                }
                this.m_bOpenSpeak = !this.m_bOpenSpeak;
                if (this.m_bOpenSpeak)
                {
                    if (!GameSettings.EnableVoice)
                    {
                        GameSettings.EnableVoice = true;
                    }
                    if (bInit)
                    {
                        if (MonoSingleton<VoiceSys>.GetInstance().UseMicOnUser)
                        {
                            this.OnBattleOpenMic(null);
                        }
                    }
                    else
                    {
                        this.OnBattleOpenMic(null);
                    }
                }
                else if (GameSettings.EnableVoice)
                {
                    GameSettings.EnableVoice = false;
                }
                this.m_OpenSpeakerTipObj.get_gameObject().CustomSetActive(true);
                Singleton<CTimerManager>.instance.ResumeTimer(this.m_Vocetimer);
            }
            if (this.m_OpeankSpeakAnim != null)
            {
                this.m_OpeankSpeakAnim.get_gameObject().CustomSetActive(false);
            }
        }

        public void BattleStart()
        {
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            if ((this.scoreBoard != null) && this.scoreBoard.IsShown())
            {
                this.scoreBoard.RegiseterEvent();
                this.scoreBoard.Show();
            }
            if (!curLvelContext.IsMobaMode())
            {
                Utility.FindChild(this._formScript.get_gameObject(), "HeroHeadHud").CustomSetActive(true);
                this.heroHeadHud = Utility.FindChild(this._formScript.get_gameObject(), "HeroHeadHud").GetComponent<HeroHeadHud>();
                this.heroHeadHud.Init();
                this._formScript.GetWidget(0x48).CustomSetActive(false);
            }
            else
            {
                Utility.FindChild(this._formScript.get_gameObject(), "HeroHeadHud").CustomSetActive(false);
                GameObject widget = this._formScript.GetWidget(0x48);
                widget.CustomSetActive(true);
                this.m_heroInfoPanel = new HeroInfoPanel();
                this.m_heroInfoPanel.Init(widget);
            }
            this.ResetSkillButtonManager(Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain, false, SkillSlotType.SLOT_SKILL_COUNT);
            if (this.m_skillButtonManager != null)
            {
                this.m_skillButtonManager.InitializeCampHeroInfo(this._formScript);
            }
            SLevelContext context2 = Singleton<BattleLogic>.instance.GetCurLvelContext();
            SkillButton button = this.GetButton(SkillSlotType.SLOT_SKILL_4);
            SkillButton button2 = this.GetButton(SkillSlotType.SLOT_SKILL_5);
            SkillButton button3 = this.GetButton(SkillSlotType.SLOT_SKILL_7);
            SkillButton button4 = this.GetButton(SkillSlotType.SLOT_SKILL_6);
            if (((((context2 != null) && (button2 != null)) && ((button2.m_button != null) && (button4 != null))) && (((button4.m_button != null) && (button3 != null)) && ((button3.m_button != null) && (button != null)))) && (button.m_button != null))
            {
                bool flag = false;
                if (Singleton<CFunctionUnlockSys>.instance.FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_ADDEDSKILL) && context2.IsMobaModeWithOutGuide())
                {
                    button2.m_button.CustomSetActive(true);
                }
                else
                {
                    button2.m_button.CustomSetActive(false);
                    bool flag2 = false;
                    if (curLvelContext.IsGameTypeGuide())
                    {
                        if (CBattleGuideManager.Is5v5GuideLevel(curLvelContext.m_mapID))
                        {
                            flag2 = true;
                        }
                    }
                    else if (curLvelContext.IsMobaModeWithOutGuide() && (curLvelContext.m_pvpPlayerNum == 10))
                    {
                        flag2 = true;
                    }
                    if (flag2)
                    {
                        button4.m_button.get_transform().set_position(button.m_button.get_transform().get_position());
                        button4.m_skillIndicatorFixedPosition = button.m_skillIndicatorFixedPosition;
                    }
                    button.m_button.get_transform().set_position(button3.m_button.get_transform().get_position());
                    button.m_skillIndicatorFixedPosition = button3.m_skillIndicatorFixedPosition;
                    button3.m_button.get_transform().set_position(button2.m_button.get_transform().get_position());
                    button3.m_skillIndicatorFixedPosition = button2.m_skillIndicatorFixedPosition;
                    flag = true;
                }
                button3.m_button.CustomSetActive(context2.m_bEnableOrnamentSlot);
                if (!context2.m_bEnableOrnamentSlot)
                {
                    flag = true;
                    button4.m_button.get_transform().set_position(button.m_button.get_transform().get_position());
                    button4.m_skillIndicatorFixedPosition = button.m_skillIndicatorFixedPosition;
                    button.m_button.get_transform().set_position(button3.m_button.get_transform().get_position());
                    button.m_skillIndicatorFixedPosition = button3.m_skillIndicatorFixedPosition;
                }
                if (flag && (this._formScript.m_sgameGraphicRaycaster != null))
                {
                    this._formScript.m_sgameGraphicRaycaster.UpdateTiles();
                }
            }
            if (this.m_OpeankBigMap != null)
            {
                MiniMapSysUT.RefreshMapPointerBig(this.m_OpeankBigMap.get_gameObject());
            }
            if (this.m_OpenMicObj != null)
            {
                MiniMapSysUT.RefreshMapPointerBig(this.m_OpenMicObj.get_gameObject());
            }
            if (this.m_OpenSpeakerObj != null)
            {
                MiniMapSysUT.RefreshMapPointerBig(this.m_OpenSpeakerObj.get_gameObject());
            }
        }

        public void ChangeSpeakerBtnState()
        {
            if (this.m_isInBattle && (this.m_bOpenSpeak != GameSettings.EnableVoice))
            {
                this.OnBattleOpenSpeaker(null);
            }
        }

        private void CheckAndUpdateLearnSkill(PoolObjHandle<ActorRoot> hero)
        {
            if (hero != 0)
            {
                for (int i = 1; i <= 3; i++)
                {
                    if (Singleton<BattleLogic>.GetInstance().IsMatchLearnSkillRule(hero, (SkillSlotType) i))
                    {
                        this.UpdateLearnSkillBtnState(i, true);
                    }
                    else
                    {
                        this.UpdateLearnSkillBtnState(i, false);
                    }
                }
            }
        }

        public void ClearSkillLvlStates(int iSkillSlotType)
        {
            SkillButton button = this.GetButton((SkillSlotType) iSkillSlotType);
            if (button != null)
            {
                GameObject skillLvlImg = button.GetSkillLvlImg(1);
                if (skillLvlImg != null)
                {
                    ListView<GameObject> view = new ListView<GameObject>();
                    Transform transform = skillLvlImg.get_transform().get_parent();
                    int count = transform.get_childCount();
                    for (int i = 0; i < count; i++)
                    {
                        GameObject item = transform.GetChild(i).get_gameObject();
                        if (item.get_name().Contains("SkillLvlImg") && item.get_activeSelf())
                        {
                            view.Add(item);
                        }
                    }
                    count = view.Count;
                    for (int j = 0; j < count; j++)
                    {
                        view[j].CustomSetActive(false);
                    }
                    view.Clear();
                }
            }
        }

        public void CloseForm()
        {
            Singleton<CUIManager>.GetInstance().CloseForm(s_battleUIForm);
        }

        public void DisableCameraDragPanelForRevive()
        {
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            if ((this._formScript != null) && (curLvelContext != null))
            {
                GameObject obj2 = Utility.FindChild(this._formCameraDragPanel.get_gameObject(), "CameraDragPanel");
                if (curLvelContext.IsMobaMode())
                {
                    obj2.CustomSetActive(false);
                }
                if (curLvelContext.IsMobaMode())
                {
                    PoolObjHandle<ActorRoot> captain = Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain;
                    if ((captain != 0) && (this._formScript != null))
                    {
                        Transform transform = (obj2 == null) ? null : obj2.get_transform().Find("panelDeadInfo");
                        this.RefreshDeadMaskState(captain, false);
                        if (transform != null)
                        {
                            Transform transform2 = transform.Find("Timer");
                            if (transform2 != null)
                            {
                                CUITimerScript component = transform2.GetComponent<CUITimerScript>();
                                if (component != null)
                                {
                                    component.EndTimer();
                                }
                            }
                            transform.get_gameObject().CustomSetActive(false);
                        }
                    }
                }
            }
        }

        public void DisableUIEvent()
        {
            if ((this._formScript != null) && (this._formScript.get_gameObject() != null))
            {
                GraphicRaycaster component = this._formScript.get_gameObject().GetComponent<GraphicRaycaster>();
                if (component != null)
                {
                    component.set_enabled(false);
                }
            }
        }

        public void EnableCameraDragPanelForDead()
        {
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            if ((this._formScript != null) && (curLvelContext != null))
            {
                GameObject obj2 = Utility.FindChild(this._formCameraDragPanel.get_gameObject(), "CameraDragPanel");
                if (curLvelContext.IsMobaMode() && (obj2 != null))
                {
                    obj2.CustomSetActive(true);
                }
                if (curLvelContext.IsMobaMode())
                {
                    PoolObjHandle<ActorRoot> captain = Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain;
                    if ((captain != 0) && (this._formScript != null))
                    {
                        this.RefreshDeadMaskState(captain, true);
                        if (obj2 != null)
                        {
                            Transform transform = obj2.get_transform().Find("panelDeadInfo");
                            if (transform != null)
                            {
                                Transform transform2 = transform.Find("Timer");
                                if (transform2 != null)
                                {
                                    CUITimerScript component = transform2.GetComponent<CUITimerScript>();
                                    if (component != null)
                                    {
                                        float time = captain.handle.ActorControl.ReviveCooldown * 0.001f;
                                        component.SetTotalTime(time);
                                        component.StartTimer();
                                        this.OnReviveTimerChange(null);
                                    }
                                }
                                transform.get_gameObject().CustomSetActive(true);
                            }
                        }
                    }
                }
            }
        }

        public void EndGoBackProcessBar()
        {
            if (this.m_goBackProcessBar != null)
            {
                this.m_goBackProcessBar.End();
            }
        }

        public BattleMisc GetBattleMisc()
        {
            return this.m_battleMisc;
        }

        public GameObject GetBigMapDragonContainer()
        {
            return this._formScript.GetWidget(60);
        }

        public SkillButton GetButton(SkillSlotType skillSlotType)
        {
            return ((this.m_skillButtonManager == null) ? null : this.m_skillButtonManager.GetButton(skillSlotType));
        }

        public SkillSlotType GetCurSkillSlotType()
        {
            return this.m_skillButtonManager.GetCurSkillSlotType();
        }

        public int GetDisplayPing()
        {
            return this.m_displayPing;
        }

        public GameObject GetEquipSkillCancleArea()
        {
            if (this._formSkillCursorScript == null)
            {
                return null;
            }
            return this._formSkillCursorScript.GetWidget(3);
        }

        public CUIJoystickScript GetJoystick()
        {
            return this._joystick;
        }

        public CUIFormScript GetJoystickFormScript()
        {
            return this._formJoystickScript;
        }

        public GameObject GetMoveJoystick()
        {
            if (this._formJoystickScript == null)
            {
                return null;
            }
            return this._formJoystickScript.GetWidget(0);
        }

        public SignalPanel GetSignalPanel()
        {
            return this.m_signalPanel;
        }

        public CUIFormScript GetSkillBtnFormScript()
        {
            return this._formSkillBtnScript;
        }

        public GameObject GetSkillCancleArea()
        {
            if (this._formSkillCursorScript == null)
            {
                return null;
            }
            return this._formSkillCursorScript.GetWidget(2);
        }

        public GameObject GetSkillCursor(enSkillJoystickMode mode)
        {
            if (this._formSkillCursorScript == null)
            {
                return null;
            }
            if (mode == enSkillJoystickMode.General)
            {
                return this._formSkillCursorScript.GetWidget(0);
            }
            return this._formSkillCursorScript.GetWidget(1);
        }

        public CUIFormScript GetSkillCursorFormScript()
        {
            return this._formSkillCursorScript;
        }

        public void HideHeroInfoPanel(bool bIsFromSetting = false)
        {
            if (this.m_panelHeroInfo == null)
            {
                if (this._formScript == null)
                {
                    return;
                }
                this.m_panelHeroInfo = Utility.FindChild(this._formScript.get_gameObject(), "PanelHeroInfo");
                if (this.m_panelHeroInfo == null)
                {
                    return;
                }
            }
            if (this.m_panelHeroCanvas == null)
            {
                this.m_panelHeroCanvas = this.m_panelHeroInfo.GetComponent<CanvasGroup>();
            }
            if (this.m_panelHeroCanvas != null)
            {
                this.m_panelHeroCanvas.set_alpha(0f);
            }
            if (Singleton<CBattleSelectTarget>.GetInstance() != null)
            {
                Singleton<CBattleSelectTarget>.GetInstance().CloseForm();
            }
            if (this.m_selectedHero != 0)
            {
                if (bIsFromSetting)
                {
                    this.m_HideSelectedHero = this.m_selectedHero;
                }
                this.m_selectedHero.Release();
            }
        }

        public void HideSkillDescInfo()
        {
            if (this.skillTipDesc == null)
            {
                this.skillTipDesc = this._formSkillBtnScript.GetWidget(0x1a);
            }
            if (this.skillTipDesc != null)
            {
                this.skillTipDesc.CustomSetActive(false);
                this.m_isSkillDecShow = false;
            }
        }

        private void InitWifiInfo()
        {
            GameObject widget = this._formScript.GetWidget(0x24);
            GameObject obj3 = this._formScript.GetWidget(0x25);
            ResGlobalInfo info = new ResGlobalInfo();
            if (GameDataMgr.svr2CltCfgDict.TryGetValue(30, out info))
            {
                this.m_bShowRealPing = info.dwConfValue != 0;
            }
            else
            {
                this.m_bShowRealPing = false;
            }
            if (obj3 != null)
            {
                this.m_wifiIcon = obj3.GetComponent<Image>();
            }
            if (widget != null)
            {
                this.m_wifiText = widget.get_transform().GetComponent<Text>();
            }
            GameObject obj4 = this._formScript.GetWidget(0x23);
            if (obj4 != null)
            {
                Transform transform = obj4.get_transform().Find("WifiTimer");
                if (transform != null)
                {
                    CUITimerScript component = transform.GetComponent<CUITimerScript>();
                    if (component != null)
                    {
                        component.SetOnChangedIntervalTime((float) this.m_wifiTimerInterval);
                    }
                }
            }
        }

        private bool IsSkillTipsActive()
        {
            if ((this.skillTipDesc == null) && (this._formScript != null))
            {
                this.skillTipDesc = this._formSkillBtnScript.GetWidget(0x1a);
            }
            return ((this.skillTipDesc != null) && this.skillTipDesc.get_activeSelf());
        }

        public void LateUpdate()
        {
            if (this.m_isInBattle)
            {
                if (this.scoreBoard != null)
                {
                    this.scoreBoard.LateUpdate();
                }
                if (this.m_enemyHeroAtkBtn != null)
                {
                    this.m_enemyHeroAtkBtn.LateUpdate();
                }
            }
        }

        public void OnAcceptTrusteeship(CUIEvent uiEvent)
        {
            this.SendTrusteeshipResult(uiEvent.m_eventParams.commonUInt32Param1, ACCEPT_AIPLAYER_RSP.ACCEPT_AIPLAEYR_YES);
        }

        private void onActorDead(ref GameDeadEventParam prm)
        {
            if (ActorHelper.IsHostCtrlActor(ref prm.src) && (this.m_skillButtonManager != null))
            {
                this.m_skillButtonManager.SkillButtonUp(this._formScript);
            }
        }

        private void OnActorGoldCoinInBattleChanged(PoolObjHandle<ActorRoot> actor, int changeValue, bool isIncome, PoolObjHandle<ActorRoot> target)
        {
            if ((Singleton<BattleLogic>.GetInstance().m_LevelContext.IsMobaMode() && ((actor != 0) && ActorHelper.IsHostCtrlActor(ref actor))) && (this._formScript != null))
            {
                GameObject widget = this._formScript.GetWidget(0x2e);
                if (widget != null)
                {
                    Text component = widget.GetComponent<Text>();
                    int goldCoinInBattle = 0;
                    if ((actor != 0) && (actor.handle.ValueComponent != null))
                    {
                        goldCoinInBattle = actor.handle.ValueComponent.GetGoldCoinInBattle();
                    }
                    if (component != null)
                    {
                        component.set_text(goldCoinInBattle.ToString());
                    }
                }
            }
        }

        private void OnActorHurtAbsorb(ref DefaultGameEventParam _param)
        {
            if ((_param.src != 0) && ActorHelper.IsHostActor(ref _param.src))
            {
                Singleton<CBattleSystem>.GetInstance().CreateOtherFloatText(enOtherFloatTextContent.Absorb, (Vector3) _param.src.handle.location, new string[0]);
            }
            else if ((_param.atker != 0) && ActorHelper.IsHostActor(ref _param.atker))
            {
                Singleton<CBattleSystem>.GetInstance().CreateOtherFloatText(enOtherFloatTextContent.Absorb, (Vector3) _param.src.handle.location, new string[0]);
            }
        }

        private void OnActorImmune(ref DefaultGameEventParam _param)
        {
            if ((_param.src != 0) && ActorHelper.IsHostActor(ref _param.src))
            {
                Singleton<CBattleSystem>.GetInstance().CreateOtherFloatText(enOtherFloatTextContent.Immunity, (Vector3) _param.src.handle.location, new string[0]);
            }
            else if ((_param.atker != 0) && ActorHelper.IsHostActor(ref _param.atker))
            {
                Singleton<CBattleSystem>.GetInstance().CreateOtherFloatText(enOtherFloatTextContent.Immunity, (Vector3) _param.src.handle.location, new string[0]);
            }
        }

        private void OnBattleAtkSelectHeroBtnDown(CUIEvent uiEvent)
        {
            SkillButton button = this.GetButton(SkillSlotType.SLOT_SKILL_0);
            if ((button != null) && !button.bDisableFlag)
            {
                Singleton<LockModeKeySelector>.GetInstance().OnHandleClickSelectTargetBtn(AttackTargetType.ATTACK_TARGET_HERO);
                Singleton<CUIParticleSystem>.GetInstance().AddParticle(CUIParticleSystem.s_particleSkillBtnEffect_Path, 0.5f, uiEvent.m_srcWidget, uiEvent.m_srcFormScript);
                Singleton<CSoundManager>.GetInstance().PostEvent("UI_signal_Change_hero", null);
            }
        }

        private void OnBattleAtkSelectSoldierBtnDown(CUIEvent uiEvent)
        {
            SkillButton button = this.GetButton(SkillSlotType.SLOT_SKILL_0);
            if ((button != null) && !button.bDisableFlag)
            {
                Singleton<LockModeKeySelector>.GetInstance().OnHandleClickSelectTargetBtn(AttackTargetType.ATTACK_TARGET_SOLDIER);
                Singleton<CUIParticleSystem>.GetInstance().AddParticle(CUIParticleSystem.s_particleSkillBtnEffect_Path, 0.5f, uiEvent.m_srcWidget, uiEvent.m_srcFormScript);
                Singleton<CSoundManager>.GetInstance().PostEvent("UI_signal_Change_xiaobing", null);
            }
        }

        private void OnBattleEquipBoughtEffectPlayEnd(CUIEvent uiEvent)
        {
            uiEvent.m_srcWidget.CustomSetActive(false);
        }

        private void OnBattleEquipQuicklyBuy(CUIEvent uiEvent)
        {
            if (this._formScript != null)
            {
                GameObject widget = this._formScript.GetWidget(50 + uiEvent.m_eventParams.battleEquipPar.m_indexInQuicklyBuyList);
                if (widget != null)
                {
                    widget.CustomSetActive(true);
                    CUIAnimationScript component = widget.GetComponent<CUIAnimationScript>();
                    if (component != null)
                    {
                        component.PlayAnimation("Battle_UI_ZhuangBei_01", true);
                    }
                }
                GameObject obj3 = this._formScript.GetWidget(0x34 + uiEvent.m_eventParams.battleEquipPar.m_indexInQuicklyBuyList);
                if (obj3 != null)
                {
                    obj3.CustomSetActive(true);
                    CUIAnimationScript script2 = obj3.GetComponent<CUIAnimationScript>();
                    if (script2 != null)
                    {
                        script2.PlayAnimation("Battle_UI_ZhuangBei_01", true);
                    }
                }
            }
        }

        private void OnBattleHeroSkillTipOpen(SkillSlot skillSlot, Vector3 Pos, uint heroId)
        {
            if (null != this._formScript)
            {
                if (this.skillTipDesc == null)
                {
                    this.skillTipDesc = Utility.FindChild(this._formScript.get_gameObject(), "Panel_SkillTip");
                    if (this.skillTipDesc == null)
                    {
                        return;
                    }
                }
                if (skillSlot != null)
                {
                    PoolObjHandle<ActorRoot> captain = Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain;
                    if (captain != 0)
                    {
                        IHeroData data = CHeroDataFactory.CreateHeroData((uint) captain.handle.TheActorMeta.ConfigId);
                        Text component = this.skillTipDesc.get_transform().Find("skillNameText").GetComponent<Text>();
                        component.set_text(StringHelper.UTF8BytesToString(ref skillSlot.SkillObj.cfgData.szSkillName));
                        Text text2 = this.skillTipDesc.get_transform().Find("SkillDescribeText").GetComponent<Text>();
                        ValueDataInfo[] actorValue = captain.handle.ValueComponent.mActorValue.GetActorValue();
                        if ((text2 != null) && (skillSlot.SkillObj.cfgData.szSkillDesc.Length > 0))
                        {
                            text2.set_text(CUICommonSystem.GetSkillDesc(skillSlot.SkillObj.cfgData.szSkillDesc, actorValue, skillSlot.GetSkillLevel(), captain.handle.ValueComponent.actorSoulLevel, heroId));
                        }
                        Text text3 = this.skillTipDesc.get_transform().Find("SkillCDText").GetComponent<Text>();
                        string[] args = new string[] { (skillSlot.GetSkillCDMax() / 0x3e8).ToString() };
                        text3.set_text(Singleton<CTextManager>.instance.GetText("Skill_Cool_Down_Tips", args));
                        string[] textArray2 = new string[] { skillSlot.NextSkillEnergyCostTotal().ToString() };
                        text3.get_transform().Find("SkillEnergyCostText").GetComponent<Text>().set_text(Singleton<CTextManager>.instance.GetText(EnergyCommon.GetEnergyShowText((uint) captain.handle.ValueComponent.mActorValue.EnergyType, EnergyShowType.CostValue), textArray2));
                        ushort[] skillEffectType = skillSlot.SkillObj.cfgData.SkillEffectType;
                        GameObject obj2 = null;
                        for (int i = 1; i <= 2; i++)
                        {
                            obj2 = component.get_transform().Find(string.Format("EffectNode{0}", i)).get_gameObject();
                            if ((i <= skillEffectType.Length) && (skillEffectType[i - 1] != 0))
                            {
                                obj2.CustomSetActive(true);
                                obj2.GetComponent<Image>().SetSprite(CSkillData.GetEffectSlotBg((SkillEffectType) skillEffectType[i - 1]), this._formScript, true, false, false, false);
                                obj2.get_transform().Find("Text").GetComponent<Text>().set_text(CSkillData.GetEffectDesc((SkillEffectType) skillEffectType[i - 1]));
                            }
                            else
                            {
                                obj2.CustomSetActive(false);
                            }
                        }
                        Vector3 vector = Pos;
                        vector.x -= 4f;
                        vector.y += 4f;
                        this.skillTipDesc.get_transform().set_position(vector);
                        this.skillTipDesc.CustomSetActive(true);
                        this.m_isSkillDecShow = true;
                    }
                }
            }
        }

        private void OnBattleLastHitBtnDown(CUIEvent uiEvent)
        {
            if (this.m_skillButtonManager != null)
            {
                this.m_skillButtonManager.LastHitButtonDown(uiEvent.m_srcFormScript);
            }
        }

        private void OnBattleLastHitBtnUp(CUIEvent uiEvent)
        {
            if (this.m_skillButtonManager != null)
            {
                this.m_skillButtonManager.LastHitButtonUp(uiEvent.m_srcFormScript);
            }
        }

        private void OnBattleLearnSkillButtonClick(CUIEvent uiEvent)
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
            if (hostPlayer != null)
            {
                PoolObjHandle<ActorRoot> captain = hostPlayer.Captain;
                if ((captain != 0) && (((uiEvent != null) && (uiEvent.m_srcWidget != null)) && ((uiEvent.m_srcWidget.get_transform() != null) && (uiEvent.m_srcWidget.get_transform().get_parent() != null))))
                {
                    string str = uiEvent.m_srcWidget.get_transform().get_parent().get_name();
                    int index = int.Parse(str.Substring(str.Length - 1));
                    if ((index >= 1) && (index <= 3))
                    {
                        byte bSkillLvl = 0;
                        if ((captain.handle.SkillControl != null) && (captain.handle.SkillControl.SkillSlotArray[index] != null))
                        {
                            bSkillLvl = (byte) captain.handle.SkillControl.SkillSlotArray[index].GetSkillLevel();
                        }
                        this.SendLearnSkillCommand(captain, (SkillSlotType) index, bSkillLvl);
                        Singleton<CSoundManager>.GetInstance().PostEvent("UI_junei_ani_jinengxuexi", null);
                        Transform transform = uiEvent.m_srcWidget.get_transform().get_parent().Find("LearnEffect");
                        if (transform != null)
                        {
                            transform.get_gameObject().CustomSetActive(true);
                            CUIAnimationScript component = transform.get_gameObject().GetComponent<CUIAnimationScript>();
                            if (component != null)
                            {
                                component.PlayAnimation("Battle_UI_Skill_01", true);
                            }
                        }
                    }
                }
            }
        }

        private void OnBattleOpenMic(CUIEvent uiEvent)
        {
            bool flag = true;
            if (uiEvent == null)
            {
                flag = false;
            }
            if (flag)
            {
                CPlayerBehaviorStat.Plus(CPlayerBehaviorStat.BehaviorType.Battle_Voice_OpenMic);
            }
            if (this.m_OpenMicTipObj != null)
            {
                if (!this.m_bOpenSpeak)
                {
                    if (this.m_OpenMicTipText != null)
                    {
                        this.m_OpenMicTipText.set_text(MonoSingleton<VoiceSys>.GetInstance().m_Voice_Battle_FIrstOPenSpeak);
                    }
                    if (flag)
                    {
                        this.m_OpenMicTipObj.get_gameObject().CustomSetActive(true);
                    }
                    Singleton<CTimerManager>.instance.ResumeTimer(this.m_VoiceMictime);
                }
                else
                {
                    if (!CFakePvPHelper.bInFakeSelect && MonoSingleton<VoiceSys>.GetInstance().IsBattleSupportVoice())
                    {
                        if (!MonoSingleton<VoiceSys>.GetInstance().GlobalVoiceSetting)
                        {
                            if (this.m_OpenMicTipText != null)
                            {
                                this.m_OpenMicTipText.set_text(MonoSingleton<VoiceSys>.GetInstance().m_Voice_Server_Not_Open_Tips);
                            }
                            if (flag)
                            {
                                this.m_OpenMicTipObj.get_gameObject().CustomSetActive(true);
                            }
                            Singleton<CTimerManager>.instance.ResumeTimer(this.m_VoiceMictime);
                            return;
                        }
                        if (!MonoSingleton<VoiceSys>.GetInstance().IsInVoiceRoom())
                        {
                            if (this.m_OpenMicTipText != null)
                            {
                                this.m_OpenMicTipText.set_text(MonoSingleton<VoiceSys>.GetInstance().m_Voice_Cannot_JoinVoiceRoom);
                            }
                            if (flag)
                            {
                                this.m_OpenMicTipObj.get_gameObject().CustomSetActive(true);
                            }
                            Singleton<CTimerManager>.instance.ResumeTimer(this.m_VoiceMictime);
                            return;
                        }
                    }
                    if (this.m_bOpenMic)
                    {
                        if (this.m_OpenMicTipText != null)
                        {
                            this.m_OpenMicTipText.set_text(MonoSingleton<VoiceSys>.GetInstance().m_Voice_Battle_CloseMic);
                        }
                        MonoSingleton<VoiceSys>.GetInstance().CloseMic();
                        if (this.m_OpenMicObj != null)
                        {
                            CUIUtility.SetImageSprite(this.m_OpenMicObj.GetComponent<Image>(), this.no_microphone_path, null, true, false, false, false);
                        }
                    }
                    else
                    {
                        if (this.m_OpenMicTipText != null)
                        {
                            this.m_OpenMicTipText.set_text(MonoSingleton<VoiceSys>.GetInstance().m_Voice_Battle_OpenMic);
                        }
                        MonoSingleton<VoiceSys>.GetInstance().OpenMic();
                        if (this.m_OpenMicObj != null)
                        {
                            CUIUtility.SetImageSprite(this.m_OpenMicObj.GetComponent<Image>(), this.microphone_path, null, true, false, false, false);
                        }
                    }
                    this.m_bOpenMic = !this.m_bOpenMic;
                    if (flag)
                    {
                        this.m_OpenMicTipObj.get_gameObject().CustomSetActive(true);
                    }
                    Singleton<CTimerManager>.instance.ResumeTimer(this.m_VoiceMictime);
                }
            }
        }

        private void OnBattleOpenSpeaker(CUIEvent uiEvent)
        {
            this.BattleOpenSpeak(uiEvent, false);
        }

        public void OnBattlePauseGame(CUIEvent uiEvent)
        {
            Singleton<CBattleGuideManager>.instance.PauseGame(this, true);
            if (this._formScript != null)
            {
                this._formScript.GetWidget(0x27).CustomSetActive(false);
                this._formScript.GetWidget(40).CustomSetActive(true);
                this._formScript.GetWidget(0x29).CustomSetActive(true);
            }
        }

        public void OnBattleResumeGame(CUIEvent uiEvent)
        {
            Singleton<CBattleGuideManager>.instance.ResumeGame(this);
            if (this._formScript != null)
            {
                this._formScript.GetWidget(40).CustomSetActive(false);
                this._formScript.GetWidget(0x27).CustomSetActive(true);
                this._formScript.GetWidget(0x29).CustomSetActive(false);
            }
        }

        private void OnBattleSkillBtnHold(CUIEvent uiEvent)
        {
            if (!this.m_isSkillDecShow && !this.m_skillButtonManager.CurrentSkillTipsResponed)
            {
                this.OnBattleSkillDecShow(uiEvent);
            }
            else if (this.m_isSkillDecShow && this.m_skillButtonManager.CurrentSkillTipsResponed)
            {
                this.HideSkillDescInfo();
            }
        }

        private void OnBattleSkillButtonDown(CUIEvent uiEvent)
        {
            if (this.m_signalPanel != null)
            {
                this.m_signalPanel.CancelSelectedSignalButton();
            }
            stUIEventParams par = new stUIEventParams();
            par = uiEvent.m_eventParams;
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Newbie_CloseSkillGesture, par);
            if (this.IsSkillTipsActive())
            {
                this.HideSkillDescInfo();
            }
            this.m_skillButtonManager.SkillButtonDown(uiEvent.m_srcFormScript, uiEvent.m_eventParams.m_skillSlotType, uiEvent.m_pointerEventData.get_position());
        }

        private void OnBattleSkillButtonDragged(CUIEvent uiEvent)
        {
            this.m_skillButtonManager.DragSkillButton(uiEvent.m_srcFormScript, uiEvent.m_eventParams.m_skillSlotType, uiEvent.m_pointerEventData.get_position());
        }

        private void OnBattleSkillButtonHoldEnd(CUIEvent uiEvent)
        {
            if (this.m_isSkillDecShow)
            {
                this.HideSkillDescInfo();
                this.m_skillButtonManager.SkillButtonUp(uiEvent.m_srcFormScript, uiEvent.m_eventParams.m_skillSlotType, false, new Vector2());
            }
        }

        private void OnBattleSkillButtonUp(CUIEvent uiEvent)
        {
            this.m_skillButtonManager.SkillButtonUp(uiEvent.m_srcFormScript, uiEvent.m_eventParams.m_skillSlotType, true, uiEvent.m_pointerEventData.get_position());
        }

        private void OnBattleSkillDecShow(CUIEvent uiEvent)
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
            if ((hostPlayer != null) && (hostPlayer.Captain != 0))
            {
                int num;
                SkillSlot slot;
                string str = uiEvent.m_srcWidget.get_transform().get_parent().get_name();
                Vector3 pos = uiEvent.m_srcWidget.get_transform().get_parent().get_transform().get_position();
                if (!int.TryParse(str.Substring(str.Length - 1), out num))
                {
                    str = uiEvent.m_srcWidget.get_transform().get_name();
                    pos = uiEvent.m_srcWidget.get_transform().get_position();
                    if (!int.TryParse(str.Substring(str.Length - 1), out num))
                    {
                        return;
                    }
                }
                SkillSlotType type = (SkillSlotType) num;
                if (hostPlayer.Captain.handle.SkillControl.TryGetSkillSlot(type, out slot))
                {
                    string str2 = Utility.UTF8Convert(slot.SkillObj.cfgData.szSkillDesc);
                    this.OnBattleHeroSkillTipOpen(slot, pos, (uint) hostPlayer.Captain.handle.TheActorMeta.ConfigId);
                }
            }
        }

        private void OnBattleSkillDisableAlert(CUIEvent uiEvent)
        {
            this.m_skillButtonManager.OnBattleSkillDisableAlert(uiEvent.m_eventParams.m_skillSlotType);
        }

        private void OnBattleSkillDisableBtnDown(CUIEvent uiEvent)
        {
            this.OnBattleSkillDecShow(uiEvent);
        }

        private void OnBattleSkillDisableBtnUp(CUIEvent uiEvent)
        {
            this.HideSkillDescInfo();
        }

        private void OnBattleSkillLevelUpEffectPlayEnd(CUIEvent uiEvent)
        {
            uiEvent.m_srcWidget.CustomSetActive(false);
        }

        private void OnBigMap_Open_BigMap(CUIEvent uievent)
        {
        }

        public void OnCancelTrusteeship(CUIEvent uiEvent)
        {
            this.SendTrusteeshipResult(uiEvent.m_eventParams.commonUInt32Param1, ACCEPT_AIPLAYER_RSP.ACCEPT_AIPLAYER_NO);
        }

        private void onChangeOperateMode(CUIEvent uiEvent)
        {
        }

        private void OnClearLockTarget(ref LockTargetEventParam prm)
        {
            this.HideHeroInfoPanel(false);
        }

        private void OnClearTarget(ref SelectTargetEventParam prm)
        {
            this.HideHeroInfoPanel(false);
        }

        private void OnClickBattleScene(CUIEvent uievent)
        {
            Singleton<LockModeScreenSelector>.GetInstance().OnClickBattleScene(uievent.m_pointerEventData.get_position());
            Singleton<TeleportTargetSelector>.GetInstance().OnClickBattleScene(uievent.m_pointerEventData.get_position());
        }

        public void OnCloseHeorInfoPanel(CUIEvent uiEvent)
        {
            Singleton<CBattleHeroInfoPanel>.GetInstance().Hide();
        }

        private void OnCommon_BattleShowOrHideWifiInfo(CUIEvent uiEvent)
        {
            GameObject widget = this._formScript.GetWidget(0x25);
            DebugHelper.Assert(widget != null);
            if (widget != null)
            {
                widget.CustomSetActive(!widget.get_activeInHierarchy());
            }
        }

        private void OnCommon_BattleWifiCheckTimer(CUIEvent uiEvent)
        {
            if (this._formScript != null)
            {
                this.m_displayPing = !this.m_bShowRealPing ? Singleton<FrameSynchr>.GetInstance().GameSvrPing : Singleton<FrameSynchr>.instance.RealSvrPing;
                this.m_displayPing = (this.m_displayPing <= 100) ? this.m_displayPing : ((((this.m_displayPing - 100) * 7) / 10) + 100);
                this.m_displayPing = Mathf.Clamp(this.m_displayPing, 0, 460);
                uint index = 0;
                if (this.m_displayPing == 0)
                {
                    this.m_displayPing = 10;
                    SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
                    if (((curLvelContext != null) && curLvelContext.m_isWarmBattle) && curLvelContext.IsGameTypeComBat())
                    {
                        int num2 = Random.Range(0, 10);
                        if (num2 == 0)
                        {
                            this.m_displayPing = 50 + Random.Range(0, 100);
                        }
                        else if (num2 < 3)
                        {
                            this.m_displayPing = 50 + Random.Range(0, 50);
                        }
                        else if (num2 < 6)
                        {
                            this.m_displayPing = 50 + Random.Range(0, 30);
                        }
                        else
                        {
                            this.m_displayPing = 50 + Random.Range(0, 15);
                        }
                    }
                }
                if (this.m_displayPing < 100)
                {
                    index = 2;
                }
                else if (this.m_displayPing < 200)
                {
                    index = 1;
                }
                else
                {
                    index = 0;
                }
                if ((this.m_wifiIcon != null) && (this.m_wifiIconCheckTicks >= this.m_wifiIconCheckMaxTicks))
                {
                    NetworkReachability reachability = Application.get_internetReachability();
                    if (reachability == null)
                    {
                        this.m_wifiIcon.SetSprite(CUIUtility.s_Sprite_System_Wifi_Dir + CLobbySystem.s_noNetStateName, this._formScript, true, false, false, false);
                    }
                    else if (reachability == 2)
                    {
                        this.m_wifiIcon.SetSprite(CUIUtility.s_Sprite_System_Wifi_Dir + CLobbySystem.s_wifiStateName[index], this._formScript, true, false, false, false);
                    }
                    else if (reachability == 1)
                    {
                        this.m_wifiIcon.SetSprite(CUIUtility.s_Sprite_System_Wifi_Dir + CLobbySystem.s_netStateName[index], this._formScript, true, false, false, false);
                    }
                    this.m_wifiIconCheckTicks = 0;
                }
                else
                {
                    this.m_wifiIconCheckTicks++;
                }
                if (this.m_wifiText != null)
                {
                    this.m_wifiText.set_text(string.Format("{0}ms", this.m_displayPing));
                    this.m_wifiText.set_color(CLobbySystem.s_WifiStateColor[index]);
                }
            }
        }

        private void onConfirmReturnLobby(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.GetInstance().CloseForm(CSettingsSys.SETTING_FORM);
            if (Singleton<BattleLogic>.GetInstance().GetCurLvelContext().IsGameTypeGuide() && !Singleton<CBattleGuideManager>.instance.bTrainingAdv)
            {
                Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Tutorial_Level_Qiut_Tip"), false, 1.5f, null, new object[0]);
            }
            else
            {
                if (Singleton<CBattleGuideManager>.instance.bPauseGame)
                {
                    Singleton<CBattleGuideManager>.instance.ResumeGame(this);
                }
                if (Singleton<LobbyLogic>.instance.inMultiGame)
                {
                    Singleton<LobbyLogic>.instance.ReqMultiGameRunaway();
                }
                else
                {
                    Singleton<BattleLogic>.instance.DoFightOver(false);
                    Singleton<BattleLogic>.instance.SingleReqLoseGame();
                }
            }
        }

        public void OnDragonTipFormClose(CUIEvent cuiEvent)
        {
            Singleton<CUIManager>.instance.CloseForm(s_battleDragonTipForm);
        }

        public void OnDragonTipFormOpen(CUIEvent cuiEvent)
        {
            CUIFormScript script = Singleton<CUIManager>.instance.OpenForm(s_battleDragonTipForm, false, true);
            Text component = Utility.FindChild(script.get_gameObject(), "DragonBuffx1Text").GetComponent<Text>();
            Text text2 = Utility.FindChild(script.get_gameObject(), "DragonBuffx2Text").GetComponent<Text>();
            Text text3 = Utility.FindChild(script.get_gameObject(), "DragonBuffx3Text").GetComponent<Text>();
            ResSkillCombineCfgInfo dataByKey = GameDataMgr.skillCombineDatabin.GetDataByKey((long) Singleton<BattleLogic>.instance.GetDragonBuffId(RES_SKILL_SRC_TYPE.RES_SKILL_SRC_DRAGON_ONE));
            ResSkillCombineCfgInfo info2 = GameDataMgr.skillCombineDatabin.GetDataByKey((long) Singleton<BattleLogic>.instance.GetDragonBuffId(RES_SKILL_SRC_TYPE.RES_SKILL_SRC_DRAGON_TWO));
            ResSkillCombineCfgInfo info3 = GameDataMgr.skillCombineDatabin.GetDataByKey((long) Singleton<BattleLogic>.instance.GetDragonBuffId(RES_SKILL_SRC_TYPE.RES_SKILL_SRC_DRAGON_THREE));
            if (dataByKey != null)
            {
                component.set_text(Utility.UTF8Convert(dataByKey.szSkillCombineDesc));
            }
            if (info2 != null)
            {
                text2.set_text(Utility.UTF8Convert(info2.szSkillCombineDesc));
            }
            if (info3 != null)
            {
                text3.set_text(Utility.UTF8Convert(info3.szSkillCombineDesc));
            }
        }

        private void OnDropCamera(CUIEvent uiEvent)
        {
            MonoSingleton<CameraSystem>.GetInstance().MoveCamera(-uiEvent.m_pointerEventData.get_delta().x, -uiEvent.m_pointerEventData.get_delta().y);
        }

        private void OnFormAppear(CUIEvent uiEvent)
        {
            if (this._formJoystickScript != null)
            {
                this._formJoystickScript.Appear(enFormHideFlag.HideByCustom, true);
            }
            if (this._formSkillCursorScript != null)
            {
                this._formSkillCursorScript.Appear(enFormHideFlag.HideByCustom, true);
            }
            if (this._formSceneScript != null)
            {
                this._formSceneScript.Appear(enFormHideFlag.HideByCustom, true);
            }
            if (this._formCameraMoveScript != null)
            {
                this._formCameraMoveScript.Appear(enFormHideFlag.HideByCustom, true);
            }
            if (this._formSkillBtnScript != null)
            {
                this._formSkillBtnScript.Appear(enFormHideFlag.HideByCustom, true);
            }
            if (this._formBuffSkillScript != null)
            {
                this._formBuffSkillScript.Appear(enFormHideFlag.HideByCustom, true);
            }
            if (this._formEnemyHeroAtkScript != null)
            {
                this._formEnemyHeroAtkScript.Appear(enFormHideFlag.HideByCustom, true);
            }
            if (this._formCameraDragPanel != null)
            {
                this._formCameraDragPanel.Appear(enFormHideFlag.HideByCustom, true);
            }
        }

        private void OnFormClosed(CUIEvent uiEvent)
        {
            Singleton<CBattleSystem>.GetInstance().OnFormClosed();
            this.UnregisterEvents();
            Singleton<InBattleMsgMgr>.instance.Clear();
            this.m_isInBattle = false;
            this.m_bOpenSpeak = false;
            this.m_bOpenMic = false;
            MonoSingleton<VoiceSys>.GetInstance().LeaveRoom();
            Singleton<CTimerManager>.instance.RemoveTimer(this.m_Vocetimer);
            Singleton<CTimerManager>.instance.RemoveTimer(this.m_VoiceMictime);
            Singleton<CTimerManager>.instance.RemoveTimer(this.m_VocetimerFirst);
            this.m_SkillSlotList.Clear();
            if (this.scoreBoard != null)
            {
                this.scoreBoard.Clear();
                this.scoreBoard = null;
            }
            if (this.m_heroInfoPanel != null)
            {
                this.m_heroInfoPanel.Clear();
                this.m_heroInfoPanel = null;
            }
            if (this.scoreboardPvE != null)
            {
                this.scoreboardPvE.Clear();
                this.scoreboardPvE = null;
            }
            if (this.treasureHud != null)
            {
                this.treasureHud.Clear();
                this.treasureHud = null;
            }
            if (this.starEvalPanel != null)
            {
                this.starEvalPanel.Clear();
                this.starEvalPanel = null;
            }
            if (this.battleTaskView != null)
            {
                this.battleTaskView.Clear();
                this.battleTaskView = null;
            }
            if (this.soldierWaveView != null)
            {
                this.soldierWaveView.Clear();
                this.soldierWaveView = null;
            }
            if (this.heroHeadHud != null)
            {
                this.heroHeadHud.Clear();
                this.heroHeadHud = null;
            }
            if (this.m_dragonView != null)
            {
                this.m_dragonView.Clear();
                this.m_dragonView = null;
            }
            if (this.m_signalPanel != null)
            {
                this.m_signalPanel.Clear();
                this.m_signalPanel = null;
            }
            if (this.m_battleMisc != null)
            {
                this.m_battleMisc.Uninit();
                this.m_battleMisc.Clear();
                this.m_battleMisc = null;
            }
            if (this.m_goBackProcessBar != null)
            {
                this.m_goBackProcessBar.Uninit();
                this.m_goBackProcessBar = null;
            }
            if (this.m_skillButtonManager != null)
            {
                this.m_skillButtonManager.Clear();
                this.m_skillButtonManager = null;
            }
            this._joystick = null;
            this.m_bOpenSpeak = false;
            this.m_bOpenMic = false;
            this.m_VocetimerFirst = 0;
            this.m_Vocetimer = 0;
            this.m_VoiceMictime = 0;
            this.m_OpenSpeakerObj = null;
            this.m_OpenSpeakerTipObj = null;
            this.m_OpenSpeakerTipText = null;
            this.m_OpeankSpeakAnim = null;
            this.m_OpeankBigMap = null;
            this.m_OpenMicObj = null;
            this.m_OpenMicTipObj = null;
            this.m_OpenMicTipText = null;
            this.m_wifiIcon = null;
            this.m_wifiText = null;
            Singleton<CUIManager>.GetInstance().CloseForm(CSettingsSys.SETTING_FORM);
            Singleton<CUIManager>.GetInstance().CloseForm(s_battleJoystick);
            Singleton<CUIManager>.GetInstance().CloseForm(s_battleSkillCursor);
            Singleton<CUIManager>.GetInstance().CloseForm(s_battleScene);
            Singleton<CUIManager>.GetInstance().CloseForm(s_battleCameraMove);
            Singleton<CUIManager>.GetInstance().CloseForm(string.Format("{0}{1}", "UGUI/Form/Common/", "Form_SmallMessageBox.prefab"));
            Singleton<CBattleHeroInfoPanel>.GetInstance().Hide();
            this._formScript = null;
            if (this.m_showBuffDesc != null)
            {
                this.m_showBuffDesc.UnInit();
            }
            if (this.m_enemyHeroAtkBtn != null)
            {
                this.m_enemyHeroAtkBtn.UnInit();
            }
            if (this.m_hostHeroDeadInfo != null)
            {
                this.m_hostHeroDeadInfo.UnInit();
            }
        }

        private void OnFormHide(CUIEvent uiEvent)
        {
            if (this._formJoystickScript != null)
            {
                this._formJoystickScript.Hide(enFormHideFlag.HideByCustom, true);
            }
            if (this._formSkillCursorScript != null)
            {
                this._formSkillCursorScript.Hide(enFormHideFlag.HideByCustom, true);
            }
            if (this._formSceneScript != null)
            {
                this._formSceneScript.Hide(enFormHideFlag.HideByCustom, true);
            }
            if (this._formCameraMoveScript != null)
            {
                this._formCameraMoveScript.Hide(enFormHideFlag.HideByCustom, true);
            }
            if (this._formSkillBtnScript != null)
            {
                this._formSkillBtnScript.Hide(enFormHideFlag.HideByCustom, true);
            }
            if (this._formBuffSkillScript != null)
            {
                this._formBuffSkillScript.Hide(enFormHideFlag.HideByCustom, true);
            }
            if (this._formEnemyHeroAtkScript != null)
            {
                this._formEnemyHeroAtkScript.Hide(enFormHideFlag.HideByCustom, true);
            }
            if (this._formCameraDragPanel != null)
            {
                this._formCameraDragPanel.Hide(enFormHideFlag.HideByCustom, true);
            }
            if (this.m_hostHeroDeadInfo != null)
            {
                this.m_hostHeroDeadInfo.OnDeadInfoFormClose(null);
            }
        }

        private void OnGameSettingCommonAttackTypeChange(CommonAttactType byAtkType)
        {
            if (Singleton<BattleLogic>.instance.isRuning)
            {
                this.m_skillButtonManager.SetCommonAtkBtnState(byAtkType);
            }
        }

        private void OnGameSettingLastHitModeChange(LastHitMode mode)
        {
            if (Singleton<BattleLogic>.instance.isRuning && (this.m_skillButtonManager != null))
            {
                this.m_skillButtonManager.SetLastHitBtnState(mode);
            }
        }

        private void onHeroEnergyChange(PoolObjHandle<ActorRoot> actor, int curVal, int maxVal)
        {
            if (this._formScript != null)
            {
                Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
                if (hostPlayer != null)
                {
                    PoolObjHandle<ActorRoot> captain = hostPlayer.Captain;
                    if ((actor != 0) && (actor == captain))
                    {
                        for (int i = 1; i <= 3; i++)
                        {
                            SkillSlot slot = actor.handle.SkillControl.SkillSlotArray[i];
                            SkillButton button = this.GetButton((SkillSlotType) i);
                            if ((slot != null) && slot.CanEnableSkillSlotByEnergy())
                            {
                                if (!slot.IsEnergyEnough)
                                {
                                    if (!button.bDisableFlag)
                                    {
                                        this.m_skillButtonManager.SetEnergyDisableButton((SkillSlotType) i);
                                    }
                                }
                                else if (button.bDisableFlag)
                                {
                                    this.m_skillButtonManager.SetEnableButton((SkillSlotType) i);
                                }
                            }
                        }
                    }
                    this.OnHeroEpChange(actor, curVal, maxVal);
                }
            }
        }

        private void onHeroEnergyMax(PoolObjHandle<ActorRoot> actor, int curVal, int maxVal)
        {
            if (this._formScript != null)
            {
                Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
                if (hostPlayer != null)
                {
                    PoolObjHandle<ActorRoot> captain = hostPlayer.Captain;
                    if ((actor != 0) && (actor == captain))
                    {
                        for (int i = 1; i <= 3; i++)
                        {
                            SkillSlot slot = actor.handle.SkillControl.SkillSlotArray[i];
                            SkillButton button = this.GetButton((SkillSlotType) i);
                            if (((slot != null) && slot.CanEnableSkillSlotByEnergy()) && button.bDisableFlag)
                            {
                                this.m_skillButtonManager.SetEnableButton((SkillSlotType) i);
                            }
                        }
                    }
                }
            }
        }

        private void OnHeroEpChange(PoolObjHandle<ActorRoot> hero, int iCurVal, int iMaxVal)
        {
            if ((this.m_selectedHero != 0) && (hero == this.m_selectedHero))
            {
                this.UpdateEpInfo();
            }
        }

        private void OnHeroHpChange(PoolObjHandle<ActorRoot> hero, int iCurVal, int iMaxVal)
        {
            if (((iCurVal <= 0) && (Singleton<GamePlayerCenter>.GetInstance().HostPlayerId != 0)) && (hero == Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain))
            {
                this.HideHeroInfoPanel(false);
            }
            if ((this.m_selectedHero != 0) && (hero == this.m_selectedHero))
            {
                this.UpdateHpInfo();
            }
        }

        private void OnHeroSkillLvlup(PoolObjHandle<ActorRoot> hero, byte bSlotType, byte bSkillLevel)
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
            if ((hostPlayer != null) && (hostPlayer.Captain == hero))
            {
                this.UpdateSkillLvlState(bSlotType, bSkillLevel);
                this.CheckAndUpdateLearnSkill(hero);
                if (bSkillLevel == 1)
                {
                    this.ResetSkillButtonManager(hero, true, (SkillSlotType) bSlotType);
                }
            }
        }

        private void OnHeroSoulLvlChange(PoolObjHandle<ActorRoot> hero, int level)
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
            if (hostPlayer != null)
            {
                PoolObjHandle<ActorRoot> captain = hostPlayer.Captain;
                if (((captain != 0) && (hero != 0)) && (((hero.handle.ActorAgent != null) && !hero.handle.ActorAgent.IsAutoAI()) && (captain == hero)))
                {
                    this.CheckAndUpdateLearnSkill(hero);
                }
            }
        }

        private void OnLockTarget(ref LockTargetEventParam prm)
        {
            if (!GameSettings.EnableHeroInfo)
            {
                this.m_HideSelectedHero = Singleton<GameObjMgr>.instance.GetActor(prm.lockTargetID);
            }
            else
            {
                Player hostPlayer = null;
                if (Singleton<GamePlayerCenter>.GetInstance().HostPlayerId != 0)
                {
                    hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
                    if (hostPlayer == null)
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
                if (hostPlayer.GetOperateMode() == OperateMode.LockMode)
                {
                    this.ShowTargetInfoByTargetId(prm.lockTargetID);
                }
            }
        }

        private void onMultiHashNotSync(CUIEvent uiEvent)
        {
            ConnectorParam connectionParam = Singleton<NetworkModule>.instance.gameSvr.GetConnectionParam();
            Singleton<GameBuilder>.instance.EndGame();
            if (connectionParam != null)
            {
                Singleton<NetworkModule>.instance.InitGameServerConnect(connectionParam);
            }
        }

        public void OnOpenHeorInfoPanel(CUIEvent uiEvent)
        {
            CPlayerBehaviorStat.Plus(CPlayerBehaviorStat.BehaviorType.Battle_ButtonViewSkillInfo);
            Singleton<CBattleHeroInfoPanel>.GetInstance().Show();
        }

        private void OnPlayerBlindess(ref ActorSkillEventParam _param)
        {
            if (_param.src != 0)
            {
                Singleton<CBattleSystem>.GetInstance().CreateOtherFloatText(enOtherFloatTextContent.Blindess, (Vector3) _param.src.handle.location, new string[0]);
            }
        }

        private void OnPlayerCancelLimitSkill(ref DefaultSkillEventParam _param)
        {
            if (this._formScript != null)
            {
                this.m_skillButtonManager.CancelLimitButton(_param.slot);
            }
        }

        private void OnPlayerEnergyShortage(ref ActorSkillEventParam _param)
        {
            if (_param.src != 0)
            {
                float num = Time.get_time();
                if (((num - this.timeEnergyShortage) > 2f) && (_param.src.handle.ValueComponent != null))
                {
                    int energyType = (int) _param.src.handle.ValueComponent.mActorValue.EnergyType;
                    Singleton<CBattleSystem>.GetInstance().CreateOtherFloatText(EnergyCommon.GetShortageText(energyType), (Vector3) _param.src.handle.location, new string[0]);
                    this.timeEnergyShortage = num;
                }
            }
        }

        private void OnPlayerLimitSkill(ref DefaultSkillEventParam _param)
        {
            if (this._formScript != null)
            {
                this.m_skillButtonManager.SetLimitButton(_param.slot);
            }
        }

        private void OnPlayerNoSkillTarget(ref ActorSkillEventParam _param)
        {
            if (_param.src != 0)
            {
                float num = Time.get_time();
                if ((num - this.timeNoSkillTarget) > 2f)
                {
                    Singleton<CBattleSystem>.GetInstance().CreateOtherFloatText(enOtherFloatTextContent.NoTarget, (Vector3) _param.src.handle.location, new string[0]);
                    this.timeNoSkillTarget = num;
                }
            }
        }

        private void OnPlayerProtectDisappear(ref ActorSkillEventParam _param)
        {
            if (_param.src != 0)
            {
                Singleton<CBattleSystem>.GetInstance().CreateOtherFloatText(enOtherFloatTextContent.ShieldDisappear, (Vector3) _param.src.handle.location, new string[0]);
            }
        }

        private void OnPlayerSkillBeanChanged(ref DefaultSkillEventParam _param)
        {
            if ((this._formScript != null) && ((_param.actor != 0) && ActorHelper.IsHostCtrlActor(ref _param.actor)))
            {
                this.m_skillButtonManager.UpdateButtonBeanNum(_param.slot, _param.param);
            }
        }

        private void OnPlayerSkillBeanShortage(ref ActorSkillEventParam _param)
        {
            if (_param.src != 0)
            {
                float num = Time.get_time();
                if ((num - this.timeSkillBeanShortage) > 2f)
                {
                    Singleton<CBattleSystem>.GetInstance().CreateOtherFloatText(enOtherFloatTextContent.BeanShortage, (Vector3) _param.src.handle.location, new string[0]);
                    this.timeSkillBeanShortage = num;
                }
            }
        }

        private void OnPlayerSkillCDChanged(ref DefaultSkillEventParam _param)
        {
            if ((this._formScript != null) && ((_param.actor != 0) && ActorHelper.IsHostCtrlActor(ref _param.actor)))
            {
                this.m_skillButtonManager.UpdateButtonCD(_param.slot, _param.param);
            }
        }

        private void OnPlayerSkillCDEnd(ref DefaultSkillEventParam _param)
        {
            if (this._formScript != null)
            {
                this.m_skillButtonManager.SetButtonCDOver(_param.slot, true);
            }
        }

        private void OnPlayerSkillCDStart(ref DefaultSkillEventParam _param)
        {
            if (this._formScript != null)
            {
                this.m_skillButtonManager.SetButtonCDStart(_param.slot);
            }
        }

        private void OnPlayerSkillChanged(ref ChangeSkillEventParam _param)
        {
            if (this._formScript != null)
            {
                this.m_skillButtonManager.ChangeSkill(_param.slot, ref _param);
            }
        }

        private void OnPlayerSkillCooldown(ref ActorSkillEventParam _param)
        {
            if (_param.src != 0)
            {
                float num = Time.get_time();
                if ((num - this.timeSkillCooldown) > 2f)
                {
                    Singleton<CBattleSystem>.GetInstance().CreateOtherFloatText(enOtherFloatTextContent.InCooldown, (Vector3) _param.src.handle.location, new string[0]);
                    this.timeSkillCooldown = num;
                }
            }
        }

        private void OnPlayerSkillDisable(ref DefaultSkillEventParam _param)
        {
            if (this._formScript != null)
            {
                this.m_skillButtonManager.SetDisableButton(_param.slot);
            }
        }

        private void OnPlayerSkillEnable(ref DefaultSkillEventParam _param)
        {
            if (this._formScript != null)
            {
                this.m_skillButtonManager.SetEnableButton(_param.slot);
            }
        }

        private void OnPlayerSkillRecovered(ref DefaultSkillEventParam _param)
        {
            if (this._formScript != null)
            {
                this.m_skillButtonManager.RecoverSkill(_param.slot, ref _param);
            }
        }

        private void OnPlayerUpdateSkill(ref DefaultSkillEventParam _param)
        {
            if ((this._formScript != null) && (_param.slot != SkillSlotType.SLOT_SKILL_0))
            {
                PoolObjHandle<ActorRoot> captain = Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain;
                this.m_skillButtonManager.Initialise(captain, true, _param.slot);
            }
        }

        public void onQuitAppClick()
        {
            SGameApplication.Quit();
        }

        private void onQuitGame(CUIEvent uiEvent)
        {
            Utility.FindChild(this._formScript.get_gameObject(), "SysMenu").CustomSetActive(false);
            SGameApplication.Quit();
        }

        private void onReturnGame(CUIEvent uiEvent)
        {
            Utility.FindChild(this._formScript.get_gameObject(), "SysMenu").CustomSetActive(false);
        }

        private void onReturnLobby(CUIEvent uiEvent)
        {
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            if (curLvelContext != null)
            {
                if (curLvelContext.IsMultilModeWithWarmBattle())
                {
                    Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("多人对战不能退出游戏。"), false, 1.5f, null, new object[0]);
                }
                else
                {
                    this.onConfirmReturnLobby(null);
                }
            }
        }

        private void OnReviveTimerChange(CUIEvent uiEvent)
        {
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            if ((this._formScript != null) && (curLvelContext != null))
            {
                Text component = Utility.FindChild(this._formCameraDragPanel.get_gameObject(), "CameraDragPanel").get_transform().Find("panelDeadInfo/lblReviveTime").GetComponent<Text>();
                Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
                if (hostPlayer != null)
                {
                    PoolObjHandle<ActorRoot> captain = hostPlayer.Captain;
                    if (captain != 0)
                    {
                        float num = captain.handle.ActorControl.ReviveCooldown * 0.001f;
                        component.set_text(string.Format("{0}", Mathf.FloorToInt(num + 0.2f)));
                    }
                }
            }
        }

        private void OnSelectTarget(ref SelectTargetEventParam prm)
        {
            if (!GameSettings.EnableHeroInfo)
            {
                this.m_HideSelectedHero = Singleton<GameObjMgr>.instance.GetActor(prm.commonAttackTargetID);
            }
            else
            {
                Player hostPlayer = null;
                if (Singleton<GamePlayerCenter>.GetInstance().HostPlayerId != 0)
                {
                    hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
                    if (hostPlayer == null)
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
                if (hostPlayer.GetOperateMode() == OperateMode.DefaultMode)
                {
                    this.ShowTargetInfoByTargetId(prm.commonAttackTargetID);
                }
            }
        }

        public void OnSwitchAutoAI(ref DefaultGameEventParam param)
        {
            if (((Singleton<GamePlayerCenter>.instance != null) && (Singleton<GamePlayerCenter>.instance.GetHostPlayer() != null)) && ((Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain != 0) && (param.src == Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain)))
            {
                GameObject obj2 = (this._formScript == null) ? null : Utility.FindChild(this._formScript.get_gameObject(), "PanelBtn/ToggleAutoBtn");
                if (obj2 != null)
                {
                    Transform transform = obj2.get_transform().Find("imgAuto");
                    if (transform != null)
                    {
                        transform.get_gameObject().CustomSetActive(Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain.handle.ActorControl.m_isAutoAI);
                        MonoSingleton<DialogueProcessor>.GetInstance().bAutoNextPage = Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain.handle.ActorControl.m_isAutoAI;
                    }
                }
            }
        }

        public void OnSwitchHeroInfoPanel(CUIEvent uiEvent)
        {
        }

        public void OnToggleAutoAI(CUIEvent uiEvent)
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
            if (((hostPlayer != null) && (hostPlayer.Captain != 0)) && (hostPlayer.Captain.handle.ActorControl != null))
            {
                FrameCommand<SwitchActorAutoAICommand> command = FrameCommandFactory.CreateFrameCommand<SwitchActorAutoAICommand>();
                command.cmdData.IsAutoAI = !hostPlayer.Captain.handle.ActorControl.m_isAutoAI ? ((sbyte) 1) : ((sbyte) 0);
                command.Send();
                uiEvent.m_srcWidget.get_gameObject().get_transform().Find("imgAuto").get_gameObject().CustomSetActive(!hostPlayer.Captain.handle.ActorControl.m_isAutoAI);
                MonoSingleton<DialogueProcessor>.GetInstance().bAutoNextPage = command.cmdData.IsAutoAI != 0;
            }
        }

        public void OnToggleFreeCamera(CUIEvent uiEvent)
        {
            MonoSingleton<CameraSystem>.instance.ToggleFreeCamera();
        }

        public void OnUpdateDragonUI(int delta)
        {
            if (this.m_dragonView != null)
            {
                this.m_dragonView.UpdateDragon(delta);
            }
        }

        private void onUseSkill(ref ActorSkillEventParam prm)
        {
            if (ActorHelper.IsHostCtrlActor(ref prm.src))
            {
                int configId = Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain.handle.TheActorMeta.ConfigId;
                SLOTINFO item = null;
                for (int i = 0; i < this.m_SkillSlotList.Count; i++)
                {
                    if (this.m_SkillSlotList[i].id == configId)
                    {
                        item = this.m_SkillSlotList[i];
                        break;
                    }
                }
                if (item == null)
                {
                    item = new SLOTINFO();
                    item.id = configId;
                    this.m_SkillSlotList.Add(item);
                }
                item.m_SKillSlot[(int) prm.slot]++;
            }
        }

        private void OnVoiceMicTimeEnd(int timersequence)
        {
            if (this.m_OpenMicTipObj != null)
            {
                Singleton<CTimerManager>.instance.PauseTimer(this.m_VoiceMictime);
                Singleton<CTimerManager>.instance.ResetTimer(this.m_VoiceMictime);
                this.m_OpenMicTipObj.get_gameObject().CustomSetActive(false);
            }
        }

        private void OnVoiceTimeEnd(int timersequence)
        {
            if (this.m_OpenSpeakerTipObj != null)
            {
                Singleton<CTimerManager>.instance.PauseTimer(this.m_Vocetimer);
                Singleton<CTimerManager>.instance.ResetTimer(this.m_Vocetimer);
                this.m_OpenSpeakerTipObj.get_gameObject().CustomSetActive(false);
            }
        }

        private void OnVoiceTimeEndFirst(int timersequence)
        {
            if (this.m_OpenSpeakerTipObj != null)
            {
                Singleton<CTimerManager>.instance.PauseTimer(this.m_Vocetimer);
                Singleton<CTimerManager>.instance.ResetTimer(this.m_Vocetimer);
                this.m_OpenSpeakerTipObj.get_gameObject().CustomSetActive(false);
            }
            if (this.m_OpeankSpeakAnim != null)
            {
                this.m_OpeankSpeakAnim.get_gameObject().CustomSetActive(false);
            }
        }

        public bool OpenForm()
        {
            this._formCameraDragPanel = Singleton<CUIManager>.GetInstance().OpenForm(s_cameraDragPanelPath, false, true);
            this._formEnemyHeroAtkScript = Singleton<CUIManager>.GetInstance().OpenForm(s_enemyHeroAtkFormPath, false, true);
            this._formBuffSkillScript = Singleton<CUIManager>.GetInstance().OpenForm(s_buffSkillFormPath, false, true);
            this._formSkillBtnScript = Singleton<CUIManager>.GetInstance().OpenForm(s_skillBtnFormPath, false, true);
            this._formScript = Singleton<CUIManager>.GetInstance().OpenForm(s_battleUIForm, false, true);
            this._formSkillCursorScript = Singleton<CUIManager>.GetInstance().OpenForm(s_battleSkillCursor, false, true);
            this._formSceneScript = Singleton<CUIManager>.GetInstance().OpenForm(s_battleScene, false, true);
            this._formCameraMoveScript = Singleton<CUIManager>.GetInstance().OpenForm(s_battleCameraMove, false, true);
            this._formJoystickScript = Singleton<CUIManager>.GetInstance().OpenForm(s_battleJoystick, false, true);
            if ((((null == this._formScript) || (null == this._formJoystickScript)) || ((null == this._formSkillCursorScript) || (this._formSceneScript == null))) || (((this._formCameraMoveScript == null) || (null == this._formSkillBtnScript)) || (((this._formBuffSkillScript == null) || (this._formEnemyHeroAtkScript == null)) || (this._formCameraDragPanel == null))))
            {
                return false;
            }
            this.m_isInBattle = true;
            this.m_SkillSlotList.Clear();
            this.InitWifiInfo();
            Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            this.m_skillButtonManager = new CSkillButtonManager();
            SGameGraphicRaycaster component = this._formScript.GetComponent<SGameGraphicRaycaster>();
            if (component != null)
            {
                DebugHelper.Assert(component.raycastMode == SGameGraphicRaycaster.RaycastMode.Sgame_tile, "Form_Battle RayCast Mode 应该设置为Sgame_tile,请检查...");
            }
            this.m_Vocetimer = Singleton<CTimerManager>.instance.AddTimer(this.m_VoiceTipsShowTime, -1, new CTimer.OnTimeUpHandler(this.OnVoiceTimeEnd));
            Singleton<CTimerManager>.instance.PauseTimer(this.m_Vocetimer);
            Singleton<CTimerManager>.instance.ResetTimer(this.m_Vocetimer);
            this.m_VoiceMictime = Singleton<CTimerManager>.instance.AddTimer(this.m_VoiceTipsShowTime, -1, new CTimer.OnTimeUpHandler(this.OnVoiceMicTimeEnd));
            Singleton<CTimerManager>.instance.PauseTimer(this.m_VoiceMictime);
            Singleton<CTimerManager>.instance.ResetTimer(this.m_VoiceMictime);
            this.m_OpenSpeakerObj = this._formScript.get_transform().Find("MapPanel/Mini/Voice_OpenSpeaker");
            this.m_OpenSpeakerTipObj = this._formScript.get_transform().Find("MapPanel/Mini/Voice_OpenSpeaker/info");
            this.m_OpeankSpeakAnim = this._formScript.get_transform().Find("MapPanel/Mini/Voice_OpenSpeaker/voice_anim");
            this.m_OpeankBigMap = this._formScript.get_transform().Find("MapPanel/Mini/Button_OpenBigMap");
            this.m_OpenMicObj = this._formScript.get_transform().Find("MapPanel/Mini/Voice_OpenMic");
            this.m_OpenMicTipObj = this._formScript.get_transform().Find("MapPanel/Mini/Voice_OpenMic/info");
            this.m_OpenSpeakerTipText = this.m_OpenSpeakerTipObj.Find("Text").GetComponent<Text>();
            if (this.m_OpeankBigMap != null)
            {
                this.m_OpeankBigMap.get_gameObject().CustomSetActive(true);
            }
            if (this.m_OpenMicTipObj != null)
            {
                this.m_OpenMicTipObj.get_gameObject().CustomSetActive(false);
                this.m_OpenMicTipText = this.m_OpenMicTipObj.Find("Text").GetComponent<Text>();
            }
            if (this.m_OpenSpeakerObj != null)
            {
                this.m_OpenSpeakerObj.get_gameObject().CustomSetActive(true);
            }
            if (this.m_OpenMicObj != null)
            {
                this.m_OpenMicObj.get_gameObject().CustomSetActive(true);
            }
            try
            {
                MonoSingleton<VoiceSys>.GetInstance().UpdateMyVoiceIcon(0);
                if (MonoSingleton<VoiceSys>.GetInstance().IsUseVoiceSysSetting)
                {
                    this.BattleOpenSpeak(null, true);
                }
                else
                {
                    MonoSingleton<VoiceSys>.GetInstance().ClosenSpeakers();
                    MonoSingleton<VoiceSys>.GetInstance().CloseMic();
                }
            }
            catch (Exception exception)
            {
                object[] inParameters = new object[] { exception.Message, exception.StackTrace };
                DebugHelper.Assert(false, "exception for closen speakers... {0} {1}", inParameters);
            }
            this.SetJoyStickMoveType(GameSettings.JoyStickMoveType);
            this.SetJoyStickShowType(GameSettings.JoyStickShowType);
            this.SetFpsShowType(GameSettings.FpsShowType);
            this.SetCameraMoveMode(GameSettings.TheCameraMoveType);
            this.m_goBackProcessBar = new BackToCityProcessBar();
            this.m_goBackProcessBar.Init(Utility.FindChild(this._formScript.get_gameObject(), "GoBackProcessBar"));
            this.treasureHud = new CTreasureHud();
            this.treasureHud.Init(Utility.FindChild(this._formScript.get_gameObject(), "TreasurePanel"));
            this.treasureHud.Hide();
            this.starEvalPanel = new CStarEvalPanel();
            this.starEvalPanel.Init(Utility.FindChild(this._formScript.get_gameObject(), "StarEvalPanel"));
            this.starEvalPanel.reset();
            this.m_battleMisc = new BattleMisc();
            this.m_battleMisc.Init(Utility.FindChild(this._formScript.get_gameObject(), "mis"), this._formScript);
            this.battleTaskView = new BattleTaskView();
            this.battleTaskView.Init(Utility.FindChild(this._formScript.get_gameObject(), "TaskView"));
            GameObject obj2 = Utility.FindChild(this._formScript.get_gameObject(), "PanelBtn/ToggleAutoBtn");
            obj2.CustomSetActive(!curLvelContext.IsMobaMode());
            if ((Singleton<BattleLogic>.GetInstance().GetCurLvelContext().m_pveLevelType == RES_LEVEL_TYPE.RES_LEVEL_TYPE_GUIDE) || !Singleton<BattleLogic>.GetInstance().GetCurLvelContext().canAutoAI)
            {
                obj2.CustomSetActive(false);
            }
            GameObject widget = this._formScript.GetWidget(0x4b);
            if (widget != null)
            {
                Singleton<CReplayKitSys>.GetInstance().InitReplayKit(widget.get_transform(), false, true);
            }
            Utility.FindChild(this._formScript.get_gameObject(), "PanelBtn/btnViewBattleInfo").CustomSetActive(curLvelContext.IsMobaMode());
            GameObject obj5 = Utility.FindChild(this._formScript.get_gameObject(), "panelTopRight/SignalPanel");
            this.m_signalPanel = new SignalPanel();
            this.m_signalPanel.Init(this._formScript, this._formScript.GetWidget(6), null, curLvelContext.IsMobaMode());
            Singleton<InBattleMsgMgr>.instance.InitView(Utility.FindChild(this._formScript.get_gameObject(), "panelTopRight/SignalPanel/Button_Chat"), this._formScript);
            if (!curLvelContext.IsMobaMode())
            {
                if (this.m_OpenSpeakerObj != null)
                {
                    this.m_OpenSpeakerObj.get_gameObject().CustomSetActive(false);
                }
                if (this.m_OpenMicObj != null)
                {
                    this.m_OpenMicObj.get_gameObject().CustomSetActive(false);
                }
                if (this.m_OpeankSpeakAnim != null)
                {
                    this.m_OpeankSpeakAnim.get_gameObject().CustomSetActive(false);
                }
                if (this.m_OpeankBigMap != null)
                {
                    this.m_OpeankBigMap.get_gameObject().CustomSetActive(false);
                }
            }
            obj5.CustomSetActive(curLvelContext.IsMobaMode());
            if (curLvelContext.m_pveLevelType == RES_LEVEL_TYPE.RES_LEVEL_TYPE_GUIDE)
            {
                if (this.m_OpenSpeakerObj != null)
                {
                    this.m_OpenSpeakerObj.get_gameObject().CustomSetActive(false);
                }
                if (this.m_OpenMicObj != null)
                {
                    this.m_OpenMicObj.get_gameObject().CustomSetActive(false);
                }
            }
            if (curLvelContext.m_pveLevelType == RES_LEVEL_TYPE.RES_LEVEL_TYPE_DEFEND)
            {
                this.soldierWaveView = new SoldierWave();
                this.soldierWaveView.Init(Utility.FindChild(this._formScript.get_gameObject(), "WaveStatistics"));
                this.soldierWaveView.Show();
            }
            GameObject obj6 = Utility.FindChild(this._formScript.get_gameObject(), "ScoreBoard");
            GameObject obj7 = Utility.FindChild(this._formScript.get_gameObject(), "ScoreBoardPvE");
            GameObject obj8 = Utility.FindChild(this._formScript.get_gameObject(), "MapPanel/DragonInfo");
            if (curLvelContext.IsMobaMode())
            {
                this.scoreBoard = new ScoreBoard();
                this.scoreBoard.Init(obj6);
                this.scoreBoard.RegiseterEvent();
                this.scoreBoard.Show();
            }
            else
            {
                obj6.CustomSetActive(false);
                if (curLvelContext.IsGameTypeAdventure())
                {
                    this.scoreboardPvE = new ScoreboardPvE();
                    this.scoreboardPvE.Init(obj7);
                    this.scoreboardPvE.Show();
                }
            }
            if (Singleton<BattleLogic>.instance.m_dragonSpawn != null)
            {
                if (curLvelContext.IsMobaModeWithOutGuide() && (curLvelContext.m_pvpPlayerNum == 10))
                {
                    obj8.CustomSetActive(false);
                }
                else
                {
                    obj8.CustomSetActive(true);
                    this.m_dragonView = new BattleDragonView();
                    this.m_dragonView.Init(obj8, Singleton<BattleLogic>.instance.m_dragonSpawn);
                }
            }
            else
            {
                obj8.CustomSetActive(false);
            }
            GameObject obj9 = this._formScript.GetWidget(0x27);
            GameObject obj10 = this._formScript.GetWidget(40);
            GameObject obj11 = this._formScript.GetWidget(0x29);
            if (curLvelContext.IsMultilModeWithWarmBattle() || curLvelContext.IsGameTypeArena())
            {
                obj9.CustomSetActive(false);
            }
            else
            {
                obj9.CustomSetActive(true);
            }
            obj10.CustomSetActive(false);
            obj11.CustomSetActive(false);
            if (this._formScript != null)
            {
                this._formScript.Hide(enFormHideFlag.HideByCustom, true);
            }
            if (this._formJoystickScript != null)
            {
                this._formJoystickScript.Hide(enFormHideFlag.HideByCustom, true);
            }
            if (this._formSkillCursorScript != null)
            {
                this._formSkillCursorScript.Hide(enFormHideFlag.HideByCustom, true);
            }
            if (this._formSceneScript != null)
            {
                this._formSceneScript.Hide(enFormHideFlag.HideByCustom, true);
            }
            if (this._formCameraMoveScript != null)
            {
                this._formCameraMoveScript.Hide(enFormHideFlag.HideByCustom, true);
            }
            if (this._formSkillBtnScript != null)
            {
                this._formSkillBtnScript.Hide(enFormHideFlag.HideByCustom, true);
            }
            if (this._formBuffSkillScript != null)
            {
                this._formBuffSkillScript.Hide(enFormHideFlag.HideByCustom, true);
            }
            if (this._formEnemyHeroAtkScript != null)
            {
                this._formEnemyHeroAtkScript.Hide(enFormHideFlag.HideByCustom, true);
            }
            if (this._formCameraDragPanel != null)
            {
                this._formCameraDragPanel.Hide(enFormHideFlag.HideByCustom, true);
            }
            if (hostPlayer != null)
            {
                this.SetDeadMaskState(hostPlayer.Captain);
            }
            Singleton<InBattleMsgMgr>.instance.RegInBattleEvent();
            if (CSysDynamicBlock.bUnfinishBlock)
            {
                Utility.FindChild(this._formScript.get_gameObject(), "panelTopRight/SignalPanel/Button_Chat").CustomSetActive(false);
            }
            this.m_showBuffDesc = new CBattleShowBuffDesc();
            this.m_showBuffDesc.Init(Utility.FindChild(this._formBuffSkillScript.get_gameObject(), "BuffSkill"));
            this.m_enemyHeroAtkBtn = new CEnemyHeroAtkBtn();
            this.m_enemyHeroAtkBtn.Init(Utility.FindChild(this._formEnemyHeroAtkScript.get_gameObject(), "EnemyHeroAtk"));
            GameObject obj13 = this._formScript.GetWidget(0x2d);
            if (obj13 != null)
            {
                obj13.CustomSetActive(curLvelContext.IsMobaMode());
            }
            GameObject obj14 = this._formScript.GetWidget(0x2e);
            if (obj14 != null)
            {
                Text text = obj14.GetComponent<Text>();
                if (text != null)
                {
                    text.set_text(0.ToString());
                }
            }
            this._joystick = this._formJoystickScript.GetWidget(0).get_transform().GetComponent<CUIJoystickScript>();
            this.m_hostHeroDeadInfo = new CHostHeroDeadInfo();
            if (this.m_hostHeroDeadInfo != null)
            {
                this.m_hostHeroDeadInfo.Init();
            }
            this.RegisterEvents();
            return true;
        }

        public void OpenHeroInfoPanel()
        {
            if (this.m_HideSelectedHero != 0)
            {
                this.ShowHeroInfoPanel(this.m_HideSelectedHero);
            }
        }

        public static void Preload(ref ActorPreloadTab preloadTab)
        {
            preloadTab.AddMesh(CUIParticleSystem.s_particleSkillBtnEffect_Path);
        }

        private void RefreshDeadMaskState(PoolObjHandle<ActorRoot> captain, bool bDead)
        {
            if ((captain != 0) && captain.handle.TheStaticData.TheBaseAttribute.DeadControl)
            {
                GameObject widget = this._formJoystickScript.GetWidget(1);
                if (widget != null)
                {
                    widget.CustomSetActive(bDead);
                }
            }
        }

        private void RegisterEvents()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_MultiHashInvalid, new CUIEventManager.OnUIEventHandler(this.onMultiHashNotSync));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_ActivateForm, new CUIEventManager.OnUIEventHandler(this.Battle_ActivateForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnCloseForm, new CUIEventManager.OnUIEventHandler(this.OnFormClosed));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnHideForm, new CUIEventManager.OnUIEventHandler(this.OnFormHide));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnAppearForm, new CUIEventManager.OnUIEventHandler(this.OnFormAppear));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OpenSysMenu, new CUIEventManager.OnUIEventHandler(this.ShowSysMenu));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_SysReturnLobby, new CUIEventManager.OnUIEventHandler(this.onReturnLobby));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_ConfirmSysReturnLobby, new CUIEventManager.OnUIEventHandler(this.onConfirmReturnLobby));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_SysQuitGame, new CUIEventManager.OnUIEventHandler(this.onQuitGame));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_SysReturnGame, new CUIEventManager.OnUIEventHandler(this.onReturnGame));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnOpMode, new CUIEventManager.OnUIEventHandler(this.onChangeOperateMode));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_SwitchAutoAI, new CUIEventManager.OnUIEventHandler(this.OnToggleAutoAI));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_ChgFreeCamera, new CUIEventManager.OnUIEventHandler(this.OnToggleFreeCamera));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_HeroInfoSwitch, new CUIEventManager.OnUIEventHandler(this.OnSwitchHeroInfoPanel));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_HeroInfoPanelOpen, new CUIEventManager.OnUIEventHandler(this.OnOpenHeorInfoPanel));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_HeroInfoPanelClose, new CUIEventManager.OnUIEventHandler(this.OnCloseHeorInfoPanel));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_FPSAndLagUpdate, new CUIEventManager.OnUIEventHandler(this.UpdateFpsAndLag));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnOpenDragonTip, new CUIEventManager.OnUIEventHandler(this.OnDragonTipFormOpen));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnCloseDragonTip, new CUIEventManager.OnUIEventHandler(this.OnDragonTipFormClose));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnSkillButtonDown, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillButtonDown));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnSkillButtonUp, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillButtonUp));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Disable_Alert, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillDisableAlert));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnSkillButtonDragged, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillButtonDragged));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Disable_Down, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillDisableBtnDown));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Disable_Up, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillDisableBtnUp));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnSkillButtonHold, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillBtnHold));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnSkillButtonHoldEnd, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillButtonHoldEnd));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnAtkSelectHeroDown, new CUIEventManager.OnUIEventHandler(this.OnBattleAtkSelectHeroBtnDown));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnAtkSelectSoldierDown, new CUIEventManager.OnUIEventHandler(this.OnBattleAtkSelectSoldierBtnDown));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnLastHitButtonDown, new CUIEventManager.OnUIEventHandler(this.OnBattleLastHitBtnDown));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnLastHitButtonUp, new CUIEventManager.OnUIEventHandler(this.OnBattleLastHitBtnUp));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_LearnSkillBtn_Click, new CUIEventManager.OnUIEventHandler(this.OnBattleLearnSkillButtonClick));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_SkillLevelUpEffectPlayEnd, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillLevelUpEffectPlayEnd));
            Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int, int>("HeroEnergyChange", new Action<PoolObjHandle<ActorRoot>, int, int>(this, (IntPtr) this.onHeroEnergyChange));
            Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int, int>("HeroEnergyMax", new Action<PoolObjHandle<ActorRoot>, int, int>(this, (IntPtr) this.onHeroEnergyMax));
            Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int, int>("HeroHpChange", new Action<PoolObjHandle<ActorRoot>, int, int>(this, (IntPtr) this.OnHeroHpChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Common_BattleWifiCheckTimer, new CUIEventManager.OnUIEventHandler(this.OnCommon_BattleWifiCheckTimer));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Common_BattleShowOrHideWifiInfo, new CUIEventManager.OnUIEventHandler(this.OnCommon_BattleShowOrHideWifiInfo));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_CameraDraging, new CUIEventManager.OnUIEventHandler(this.OnDropCamera));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_RevivieTimer, new CUIEventManager.OnUIEventHandler(this.OnReviveTimerChange));
            Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<ChangeSkillEventParam>(GameSkillEventDef.Event_ChangeSkill, new GameSkillEvent<ChangeSkillEventParam>(this.OnPlayerSkillChanged));
            Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_RecoverSkill, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerSkillRecovered));
            Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<DefaultSkillEventParam>(GameSkillEventDef.AllEvent_ChangeSkillCD, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerSkillCDChanged));
            Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_SkillCDStart, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerSkillCDStart));
            Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_SkillCDEnd, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerSkillCDEnd));
            Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_ChangeSkillBean, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerSkillBeanChanged));
            Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_EnableSkill, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerSkillEnable));
            Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_DisableSkill, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerSkillDisable));
            Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<ActorSkillEventParam>(GameSkillEventDef.Event_ProtectDisappear, new GameSkillEvent<ActorSkillEventParam>(this.OnPlayerProtectDisappear));
            Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<ActorSkillEventParam>(GameSkillEventDef.Event_SkillCooldown, new GameSkillEvent<ActorSkillEventParam>(this.OnPlayerSkillCooldown));
            Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<ActorSkillEventParam>(GameSkillEventDef.Enent_EnergyShortage, new GameSkillEvent<ActorSkillEventParam>(this.OnPlayerEnergyShortage));
            Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<ActorSkillEventParam>(GameSkillEventDef.Event_SkillBeanShortage, new GameSkillEvent<ActorSkillEventParam>(this.OnPlayerSkillBeanShortage));
            Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<ActorSkillEventParam>(GameSkillEventDef.Event_NoSkillTarget, new GameSkillEvent<ActorSkillEventParam>(this.OnPlayerNoSkillTarget));
            Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<ActorSkillEventParam>(GameSkillEventDef.AllEvent_Blindess, new GameSkillEvent<ActorSkillEventParam>(this.OnPlayerBlindess));
            Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_LimitSkill, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerLimitSkill));
            Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_CancelLimitSkill, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerCancelLimitSkill));
            Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_UpdateSkillUI, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerUpdateSkill));
            Singleton<GameEventSys>.GetInstance().AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorImmune, new RefAction<DefaultGameEventParam>(this.OnActorImmune));
            Singleton<GameEventSys>.GetInstance().AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorHurtAbsorb, new RefAction<DefaultGameEventParam>(this.OnActorHurtAbsorb));
            Singleton<GameEventSys>.GetInstance().AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_AutoAISwitch, new RefAction<DefaultGameEventParam>(this.OnSwitchAutoAI));
            Singleton<GameEventSys>.GetInstance().AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
            Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<ActorSkillEventParam>(GameSkillEventDef.AllEvent_UseSkill, new GameSkillEvent<ActorSkillEventParam>(this.onUseSkill));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Pause_Game, new CUIEventManager.OnUIEventHandler(this.OnBattlePauseGame));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Resume_Game, new CUIEventManager.OnUIEventHandler(this.OnBattleResumeGame));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BattleEquip_RecommendEquip_Buy, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipQuicklyBuy));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_EquipBoughtEffectPlayEnd, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipBoughtEffectPlayEnd));
            Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int, bool, PoolObjHandle<ActorRoot>>("HeroGoldCoinInBattleChange", new Action<PoolObjHandle<ActorRoot>, int, bool, PoolObjHandle<ActorRoot>>(this, (IntPtr) this.OnActorGoldCoinInBattleChanged));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Trusteeship_Accept, new CUIEventManager.OnUIEventHandler(this.OnAcceptTrusteeship));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Trusteeship_Cancel, new CUIEventManager.OnUIEventHandler(this.OnCancelTrusteeship));
            Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int>("HeroSoulLevelChange", new Action<PoolObjHandle<ActorRoot>, int>(this, (IntPtr) this.OnHeroSoulLvlChange));
            Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, byte, byte>("HeroSkillLevelUp", new Action<PoolObjHandle<ActorRoot>, byte, byte>(this, (IntPtr) this.OnHeroSkillLvlup));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.VOICE_OpenSpeaker, new CUIEventManager.OnUIEventHandler(this.OnBattleOpenSpeaker));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.VOICE_OpenMic, new CUIEventManager.OnUIEventHandler(this.OnBattleOpenMic));
            Singleton<EventRouter>.GetInstance().AddEventHandler<CommonAttactType>(EventID.GAME_SETTING_COMMONATTACK_TYPE_CHANGE, new Action<CommonAttactType>(this.OnGameSettingCommonAttackTypeChange));
            Singleton<EventRouter>.GetInstance().AddEventHandler<LastHitMode>(EventID.GAME_SETTING_LASTHIT_MODE_CHANGE, new Action<LastHitMode>(this.OnGameSettingLastHitModeChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Click_Scene, new CUIEventManager.OnUIEventHandler(this.OnClickBattleScene));
            this.SetCommonAttackTargetEvent();
        }

        public void ResetHostPlayerSkillIndicatorSensitivity()
        {
            if (Singleton<BattleLogic>.GetInstance().isFighting)
            {
                Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
                if (hostPlayer != null)
                {
                    PoolObjHandle<ActorRoot> captain = hostPlayer.Captain;
                    if ((captain != 0) && (captain.handle.SkillControl != null))
                    {
                        float spd = 0f;
                        float angularSpd = 0f;
                        GameSettings.GetLunPanSensitivity(out spd, out angularSpd);
                        for (int i = 0; i < captain.handle.SkillControl.SkillSlotArray.Length; i++)
                        {
                            SkillSlot slot = captain.handle.SkillControl.SkillSlotArray[i];
                            if ((slot != null) && (slot.skillIndicator != null))
                            {
                                slot.skillIndicator.SetIndicatorSpeed(spd, angularSpd);
                            }
                        }
                    }
                }
            }
        }

        public void ResetSkillButtonManager(PoolObjHandle<ActorRoot> actor, bool bInitSpecifiedButton = false, SkillSlotType specifiedType = 10)
        {
            if ((actor.handle != null) && (this.m_skillButtonManager != null))
            {
                this.m_skillButtonManager.Initialise(actor, bInitSpecifiedButton, specifiedType);
                Singleton<EventRouter>.GetInstance().BroadCastEvent("ResetSkillButtonManager");
            }
        }

        private void SendLearnSkillCommand(PoolObjHandle<ActorRoot> actor, SkillSlotType enmSkillSlotType, byte bSkillLvl)
        {
            FrameCommand<LearnSkillCommand> command = FrameCommandFactory.CreateFrameCommand<LearnSkillCommand>();
            command.cmdData.dwHeroID = actor.handle.ObjID;
            command.cmdData.bSkillLevel = bSkillLvl;
            command.cmdData.bSlotType = (byte) enmSkillSlotType;
            command.Send();
        }

        public void SendTrusteeshipResult(uint objID, ACCEPT_AIPLAYER_RSP trusteeshipRsp)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x415);
            msg.stPkgData.stIsAcceptAiPlayerRsp.dwAiPlayerObjID = objID;
            msg.stPkgData.stIsAcceptAiPlayerRsp.bResult = (byte) trusteeshipRsp;
            Singleton<NetworkModule>.GetInstance().SendGameMsg(ref msg, 0);
        }

        public void SetButtonHighLight(GameObject button, bool highLight)
        {
            this.m_skillButtonManager.SetButtonHighLight(button, highLight);
        }

        public void SetCameraMoveMode(CameraMoveType cameraMoveType)
        {
            if (this._formCameraMoveScript != null)
            {
                bool bActive = false;
                bool flag2 = false;
                switch (cameraMoveType)
                {
                    case CameraMoveType.Close:
                        bActive = false;
                        flag2 = false;
                        break;

                    case CameraMoveType.JoyStick:
                        bActive = true;
                        flag2 = false;
                        break;

                    case CameraMoveType.Slide:
                        bActive = false;
                        flag2 = true;
                        break;
                }
                SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
                if ((curLvelContext != null) && !curLvelContext.m_isCanRightJoyStickCameraDrag)
                {
                    bActive = false;
                    flag2 = false;
                }
                GameObject widget = this._formCameraMoveScript.GetWidget(0);
                GameObject obj3 = this._formCameraMoveScript.GetWidget(1);
                if (widget != null)
                {
                    widget.CustomSetActive(bActive);
                }
                if (obj3 != null)
                {
                    obj3.CustomSetActive(flag2);
                }
            }
        }

        public void SetCommonAttackTargetEvent()
        {
            Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<SelectTargetEventParam>(GameSkillEventDef.Event_SelectTarget, new GameSkillEvent<SelectTargetEventParam>(this.OnSelectTarget));
            Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<SelectTargetEventParam>(GameSkillEventDef.Event_ClearTarget, new GameSkillEvent<SelectTargetEventParam>(this.OnClearTarget));
            Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<LockTargetEventParam>(GameSkillEventDef.Event_LockTarget, new GameSkillEvent<LockTargetEventParam>(this.OnLockTarget));
            Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<LockTargetEventParam>(GameSkillEventDef.Event_ClearLockTarget, new GameSkillEvent<LockTargetEventParam>(this.OnClearLockTarget));
        }

        private void SetDeadMaskState(PoolObjHandle<ActorRoot> captain)
        {
            if ((captain != 0) && captain.handle.TheStaticData.TheBaseAttribute.DeadControl)
            {
                GameObject obj2 = Utility.FindChild(this._formCameraDragPanel.get_gameObject(), "CameraDragPanel");
                if (obj2 != null)
                {
                    Image component = obj2.GetComponent<Image>();
                    if (component != null)
                    {
                        component.set_enabled(false);
                    }
                }
            }
        }

        public void SetDragonUINum(COM_PLAYERCAMP camp, byte num)
        {
            if (this.m_dragonView != null)
            {
                this.m_dragonView.SetDrgonNum(camp, num);
            }
        }

        public void SetFpsShowType(int showType)
        {
            if (this._formScript != null)
            {
                bool bActive = showType == 1;
                this._formScript.GetWidget(0x20).CustomSetActive(bActive);
            }
        }

        public void SetJoyStickMoveType(int moveType)
        {
            if (this._formJoystickScript != null)
            {
                CUIJoystickScript component = this._formJoystickScript.GetWidget(0).get_transform().GetComponent<CUIJoystickScript>();
                if (component != null)
                {
                    if ((moveType == 0) || CCheatSystem.IsJoystickForceMoveable())
                    {
                        component.m_isAxisMoveable = true;
                    }
                    else
                    {
                        component.m_isAxisMoveable = false;
                    }
                }
            }
        }

        public void SetJoyStickShowType(int showType)
        {
            if (this._formScript != null)
            {
                if (showType == 0)
                {
                    this.m_skillButtonManager.SetSkillIndicatorMode(enSkillIndicatorMode.FixedPosition);
                }
                else
                {
                    this.m_skillButtonManager.SetSkillIndicatorMode(enSkillIndicatorMode.General);
                }
            }
        }

        public void SetLearnBtnHighLight(GameObject button, bool highLight)
        {
            this.m_skillButtonManager.SetlearnBtnHighLight(button, highLight);
        }

        private void SetSelectedHeroInfo(PoolObjHandle<ActorRoot> hero)
        {
            if (this.m_panelHeroInfo == null)
            {
                this.m_panelHeroInfo = Utility.FindChild(this._formScript.get_gameObject(), "PanelHeroInfo");
            }
            if ((this.m_panelHeroInfo != null) && (hero != 0))
            {
                if (this.m_objHeroHead == null)
                {
                    this.m_objHeroHead = Utility.FindChild(this.m_panelHeroInfo, "HeroHead");
                }
                if (this.m_objHeroHead != null)
                {
                    uint configId = (uint) hero.handle.TheActorMeta.ConfigId;
                    KillDetailInfo detail = new KillDetailInfo();
                    detail.Killer = hero;
                    KillInfo info2 = KillNotifyUT.Convert_DetailInfo_KillInfo(detail);
                    this.m_objHeroHead.get_transform().Find("imageIcon").GetComponent<Image>().SetSprite(info2.KillerImgSrc, this._formScript, true, false, false, false);
                }
                Singleton<CBattleSelectTarget>.GetInstance().OpenForm(hero);
            }
        }

        public void ShowArenaTimer()
        {
            GameObject obj2 = Utility.FindChild(this._formScript.get_gameObject(), "ArenaTimer63s");
            if (obj2 != null)
            {
                Transform transform = obj2.get_transform().Find("Timer");
                if (transform != null)
                {
                    CUITimerScript component = transform.GetComponent<CUITimerScript>();
                    if (component != null)
                    {
                        component.ReStartTimer();
                    }
                }
                obj2.CustomSetActive(true);
            }
        }

        public void ShowHeroInfoPanel(PoolObjHandle<ActorRoot> hero)
        {
            if (this.m_panelHeroInfo == null)
            {
                this.m_panelHeroInfo = Utility.FindChild(this._formScript.get_gameObject(), "PanelHeroInfo");
                if (this.m_panelHeroInfo != null)
                {
                    this.m_panelHeroInfo.CustomSetActive(true);
                }
            }
            if (this.m_panelHeroCanvas == null)
            {
                this.m_panelHeroCanvas = this.m_panelHeroInfo.GetComponent<CanvasGroup>();
            }
            if (((this.m_panelHeroInfo != null) && (this.m_panelHeroCanvas != null)) && ((hero != 0) && (hero.handle.ValueComponent != null)))
            {
                this.m_selectedHero = hero;
                this.SetSelectedHeroInfo(hero);
                this.m_panelHeroCanvas.set_alpha(1f);
            }
        }

        private void ShowSkillDescInfo(string strSkillDesc, Vector3 Pos)
        {
            if (this.BuffDesc == null)
            {
                this.BuffDesc = Utility.FindChild(this._formScript.get_gameObject(), "SkillDesc");
                if (this.BuffDesc == null)
                {
                    return;
                }
            }
            GameObject obj2 = Utility.FindChild(this.BuffDesc, "Text");
            if (obj2 != null)
            {
                Text component = obj2.GetComponent<Text>();
                component.set_text(strSkillDesc);
                float num = component.get_preferredHeight();
                Image componetInChild = Utility.GetComponetInChild<Image>(this.BuffDesc, "bg");
                if (componetInChild != null)
                {
                    Vector2 vector = componetInChild.get_rectTransform().get_sizeDelta();
                    num += ((componetInChild.get_gameObject().get_transform().get_localPosition().y - component.get_gameObject().get_transform().get_localPosition().y) * 2f) + 10f;
                    componetInChild.get_rectTransform().set_sizeDelta(new Vector2(vector.x, num));
                    RectTransform transform = this.BuffDesc.GetComponent<RectTransform>();
                    if (transform != null)
                    {
                        transform.set_sizeDelta(componetInChild.get_rectTransform().get_sizeDelta());
                    }
                    Vector3 vector2 = Pos;
                    vector2.x -= 4f;
                    vector2.y += 4f;
                    this.BuffDesc.get_transform().set_position(vector2);
                    this.BuffDesc.CustomSetActive(true);
                    this.m_isSkillDecShow = true;
                }
            }
        }

        public void ShowSysMenu(CUIEvent uiEvent)
        {
            Singleton<CUIParticleSystem>.instance.Hide(null);
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Settings_OpenForm);
        }

        private void ShowTargetInfoByTargetId(uint uilockTargetID)
        {
            uint objID = uilockTargetID;
            if (Singleton<GameObjMgr>.instance != null)
            {
                PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.instance.GetActor(objID);
                if (actor != 0)
                {
                    this.ShowHeroInfoPanel(actor);
                }
            }
        }

        public void ShowTaskView(bool show)
        {
            if (this.battleTaskView != null)
            {
                this.battleTaskView.Visible = show;
            }
        }

        public void ShowVoiceTips()
        {
            if (this.m_OpenSpeakerTipObj != null)
            {
                if (!MonoSingleton<VoiceSys>.GetInstance().IsUseVoiceSysSetting)
                {
                    this.m_OpenSpeakerTipObj.get_gameObject().CustomSetActive(true);
                    if (this.m_OpenSpeakerTipText != null)
                    {
                        this.m_OpenSpeakerTipText.set_text(MonoSingleton<VoiceSys>.GetInstance().m_Voice_Battle_FirstTips);
                    }
                    if (this.m_OpeankSpeakAnim != null)
                    {
                        this.m_OpeankSpeakAnim.get_gameObject().CustomSetActive(true);
                    }
                    this.m_VocetimerFirst = Singleton<CTimerManager>.instance.AddTimer(0x2710, 1, new CTimer.OnTimeUpHandler(this.OnVoiceTimeEndFirst));
                }
                else
                {
                    this.m_OpenSpeakerTipObj.get_gameObject().CustomSetActive(false);
                    if (this.m_OpeankSpeakAnim != null)
                    {
                        this.m_OpeankSpeakAnim.get_gameObject().CustomSetActive(false);
                    }
                }
            }
        }

        public void ShowWinLosePanel(bool bWin)
        {
            Singleton<WinLose>.GetInstance().ShowPanel(bWin);
            Singleton<CRecordUseSDK>.instance.DoFightOver();
        }

        public void StartGoBackProcessBar(uint startTime, uint totalTime, string text)
        {
            if (this.m_goBackProcessBar != null)
            {
                this.m_goBackProcessBar.Start(startTime, totalTime, text);
            }
        }

        private void UnRegisteredCommonAttackTargetEvent()
        {
            Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<SelectTargetEventParam>(GameSkillEventDef.Event_SelectTarget, new GameSkillEvent<SelectTargetEventParam>(this.OnSelectTarget));
            Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<SelectTargetEventParam>(GameSkillEventDef.Event_ClearTarget, new GameSkillEvent<SelectTargetEventParam>(this.OnClearTarget));
            Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<LockTargetEventParam>(GameSkillEventDef.Event_LockTarget, new GameSkillEvent<LockTargetEventParam>(this.OnLockTarget));
            Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<LockTargetEventParam>(GameSkillEventDef.Event_ClearLockTarget, new GameSkillEvent<LockTargetEventParam>(this.OnClearLockTarget));
        }

        private void UnregisterEvents()
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_MultiHashInvalid, new CUIEventManager.OnUIEventHandler(this.onMultiHashNotSync));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_ActivateForm, new CUIEventManager.OnUIEventHandler(this.Battle_ActivateForm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnCloseForm, new CUIEventManager.OnUIEventHandler(this.OnFormClosed));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnHideForm, new CUIEventManager.OnUIEventHandler(this.OnFormHide));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnAppearForm, new CUIEventManager.OnUIEventHandler(this.OnFormAppear));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OpenSysMenu, new CUIEventManager.OnUIEventHandler(this.ShowSysMenu));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_SysReturnLobby, new CUIEventManager.OnUIEventHandler(this.onReturnLobby));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_ConfirmSysReturnLobby, new CUIEventManager.OnUIEventHandler(this.onConfirmReturnLobby));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_SysQuitGame, new CUIEventManager.OnUIEventHandler(this.onQuitGame));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_SysReturnGame, new CUIEventManager.OnUIEventHandler(this.onReturnGame));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnOpMode, new CUIEventManager.OnUIEventHandler(this.onChangeOperateMode));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_SwitchAutoAI, new CUIEventManager.OnUIEventHandler(this.OnToggleAutoAI));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_ChgFreeCamera, new CUIEventManager.OnUIEventHandler(this.OnToggleFreeCamera));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_HeroInfoSwitch, new CUIEventManager.OnUIEventHandler(this.OnSwitchHeroInfoPanel));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_HeroInfoPanelOpen, new CUIEventManager.OnUIEventHandler(this.OnOpenHeorInfoPanel));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_HeroInfoPanelClose, new CUIEventManager.OnUIEventHandler(this.OnCloseHeorInfoPanel));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_FPSAndLagUpdate, new CUIEventManager.OnUIEventHandler(this.UpdateFpsAndLag));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnOpenDragonTip, new CUIEventManager.OnUIEventHandler(this.OnDragonTipFormOpen));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnCloseDragonTip, new CUIEventManager.OnUIEventHandler(this.OnDragonTipFormClose));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnSkillButtonDown, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillButtonDown));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnSkillButtonUp, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillButtonUp));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Disable_Alert, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillDisableAlert));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnSkillButtonDragged, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillButtonDragged));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Disable_Down, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillDisableBtnDown));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Disable_Up, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillDisableBtnUp));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnSkillButtonHold, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillBtnHold));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnSkillButtonHoldEnd, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillButtonHoldEnd));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnAtkSelectHeroDown, new CUIEventManager.OnUIEventHandler(this.OnBattleAtkSelectHeroBtnDown));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnAtkSelectSoldierDown, new CUIEventManager.OnUIEventHandler(this.OnBattleAtkSelectSoldierBtnDown));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnLastHitButtonDown, new CUIEventManager.OnUIEventHandler(this.OnBattleLastHitBtnDown));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnLastHitButtonUp, new CUIEventManager.OnUIEventHandler(this.OnBattleLastHitBtnUp));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_LearnSkillBtn_Click, new CUIEventManager.OnUIEventHandler(this.OnBattleLearnSkillButtonClick));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_SkillLevelUpEffectPlayEnd, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillLevelUpEffectPlayEnd));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int>("HeroSoulLevelChange", new Action<PoolObjHandle<ActorRoot>, int>(this, (IntPtr) this.OnHeroSoulLvlChange));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, byte, byte>("HeroSkillLevelUp", new Action<PoolObjHandle<ActorRoot>, byte, byte>(this, (IntPtr) this.OnHeroSkillLvlup));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int, int>("HeroEnergyChange", new Action<PoolObjHandle<ActorRoot>, int, int>(this, (IntPtr) this.onHeroEnergyChange));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int, int>("HeroEnergyMax", new Action<PoolObjHandle<ActorRoot>, int, int>(this, (IntPtr) this.onHeroEnergyMax));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int, int>("HeroHpChange", new Action<PoolObjHandle<ActorRoot>, int, int>(this, (IntPtr) this.OnHeroHpChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Common_BattleWifiCheckTimer, new CUIEventManager.OnUIEventHandler(this.OnCommon_BattleWifiCheckTimer));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Common_BattleShowOrHideWifiInfo, new CUIEventManager.OnUIEventHandler(this.OnCommon_BattleShowOrHideWifiInfo));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_CameraDraging, new CUIEventManager.OnUIEventHandler(this.OnDropCamera));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_RevivieTimer, new CUIEventManager.OnUIEventHandler(this.OnReviveTimerChange));
            Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<ChangeSkillEventParam>(GameSkillEventDef.Event_ChangeSkill, new GameSkillEvent<ChangeSkillEventParam>(this.OnPlayerSkillChanged));
            Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_RecoverSkill, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerSkillRecovered));
            Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<DefaultSkillEventParam>(GameSkillEventDef.AllEvent_ChangeSkillCD, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerSkillCDChanged));
            Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_SkillCDStart, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerSkillCDStart));
            Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_SkillCDEnd, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerSkillCDEnd));
            Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_ChangeSkillBean, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerSkillBeanChanged));
            Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_EnableSkill, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerSkillEnable));
            Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_DisableSkill, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerSkillDisable));
            Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<ActorSkillEventParam>(GameSkillEventDef.Event_ProtectDisappear, new GameSkillEvent<ActorSkillEventParam>(this.OnPlayerProtectDisappear));
            Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<ActorSkillEventParam>(GameSkillEventDef.Event_SkillCooldown, new GameSkillEvent<ActorSkillEventParam>(this.OnPlayerSkillCooldown));
            Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<ActorSkillEventParam>(GameSkillEventDef.Enent_EnergyShortage, new GameSkillEvent<ActorSkillEventParam>(this.OnPlayerEnergyShortage));
            Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<ActorSkillEventParam>(GameSkillEventDef.Event_SkillBeanShortage, new GameSkillEvent<ActorSkillEventParam>(this.OnPlayerSkillBeanShortage));
            Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<ActorSkillEventParam>(GameSkillEventDef.Event_NoSkillTarget, new GameSkillEvent<ActorSkillEventParam>(this.OnPlayerNoSkillTarget));
            Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<ActorSkillEventParam>(GameSkillEventDef.AllEvent_Blindess, new GameSkillEvent<ActorSkillEventParam>(this.OnPlayerBlindess));
            Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_LimitSkill, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerLimitSkill));
            Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_CancelLimitSkill, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerCancelLimitSkill));
            Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_UpdateSkillUI, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerUpdateSkill));
            Singleton<GameEventSys>.GetInstance().RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorImmune, new RefAction<DefaultGameEventParam>(this.OnActorImmune));
            Singleton<GameEventSys>.GetInstance().RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorHurtAbsorb, new RefAction<DefaultGameEventParam>(this.OnActorHurtAbsorb));
            Singleton<GameEventSys>.GetInstance().RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_AutoAISwitch, new RefAction<DefaultGameEventParam>(this.OnSwitchAutoAI));
            Singleton<GameEventSys>.GetInstance().RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
            Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<ActorSkillEventParam>(GameSkillEventDef.AllEvent_UseSkill, new GameSkillEvent<ActorSkillEventParam>(this.onUseSkill));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Pause_Game, new CUIEventManager.OnUIEventHandler(this.OnBattlePauseGame));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Resume_Game, new CUIEventManager.OnUIEventHandler(this.OnBattleResumeGame));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BattleEquip_RecommendEquip_Buy, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipQuicklyBuy));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_EquipBoughtEffectPlayEnd, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipBoughtEffectPlayEnd));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int, bool, PoolObjHandle<ActorRoot>>("HeroGoldCoinInBattleChange", new Action<PoolObjHandle<ActorRoot>, int, bool, PoolObjHandle<ActorRoot>>(this, (IntPtr) this.OnActorGoldCoinInBattleChanged));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Trusteeship_Accept, new CUIEventManager.OnUIEventHandler(this.OnAcceptTrusteeship));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Trusteeship_Cancel, new CUIEventManager.OnUIEventHandler(this.OnCancelTrusteeship));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.VOICE_OpenSpeaker, new CUIEventManager.OnUIEventHandler(this.OnBattleOpenSpeaker));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.VOICE_OpenMic, new CUIEventManager.OnUIEventHandler(this.OnBattleOpenMic));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<CommonAttactType>(EventID.GAME_SETTING_COMMONATTACK_TYPE_CHANGE, new Action<CommonAttactType>(this.OnGameSettingCommonAttackTypeChange));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<LastHitMode>(EventID.GAME_SETTING_LASTHIT_MODE_CHANGE, new Action<LastHitMode>(this.OnGameSettingLastHitModeChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Click_Scene, new CUIEventManager.OnUIEventHandler(this.OnClickBattleScene));
            this.UnRegisteredCommonAttackTargetEvent();
        }

        public void Update()
        {
            if (this.m_isInBattle)
            {
                if (this.scoreBoard != null)
                {
                    this.scoreBoard.Update();
                }
                if (this.m_heroInfoPanel != null)
                {
                    this.m_heroInfoPanel.Update();
                }
                if (this.scoreboardPvE != null)
                {
                    this.scoreboardPvE.Update();
                }
                if (this.soldierWaveView != null)
                {
                    this.soldierWaveView.Update();
                }
                if (this.m_signalPanel != null)
                {
                    this.m_signalPanel.Update();
                }
                if (Singleton<InBattleMsgMgr>.instance != null)
                {
                    Singleton<InBattleMsgMgr>.instance.Update();
                }
                if (Singleton<CBattleSystem>.instance.TheMinimapSys != null)
                {
                    Singleton<CBattleSystem>.instance.TheMinimapSys.Update();
                }
                if (Singleton<TeleportTargetSelector>.GetInstance() != null)
                {
                    Singleton<TeleportTargetSelector>.GetInstance().Update();
                }
            }
        }

        public void UpdateAdValueInfo()
        {
            if (this.m_selectedHero != 0)
            {
                int totalValue = this.m_selectedHero.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYATKPT].totalValue;
                if (this.m_AdTxt == null)
                {
                    GameObject obj2 = Utility.FindChild(this.m_panelHeroInfo, "InfoPanel/TxtPanel/AdTxt");
                    if (obj2 != null)
                    {
                        this.m_AdTxt = obj2.GetComponent<Text>();
                    }
                }
                if (this.m_AdTxt != null)
                {
                    this.m_AdTxt.set_text(totalValue.ToString());
                }
            }
        }

        public void UpdateApValueInfo()
        {
            if (this.m_selectedHero != 0)
            {
                int totalValue = this.m_selectedHero.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCATKPT].totalValue;
                if (this.m_ApTxt == null)
                {
                    GameObject obj2 = Utility.FindChild(this.m_panelHeroInfo, "InfoPanel/TxtPanel/ApTxt");
                    if (obj2 != null)
                    {
                        this.m_ApTxt = obj2.GetComponent<Text>();
                    }
                }
                if (this.m_ApTxt != null)
                {
                    this.m_ApTxt.set_text(totalValue.ToString());
                }
            }
        }

        public void UpdateEpInfo()
        {
            if (this.m_selectedHero != 0)
            {
                int actorEp = this.m_selectedHero.handle.ValueComponent.actorEp;
                int totalValue = this.m_selectedHero.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_MAXEP].totalValue;
                if (this.m_EpImg == null)
                {
                    GameObject obj2 = Utility.FindChild(this.m_panelHeroInfo, "EpImg");
                    if (obj2 != null)
                    {
                        this.m_EpImg = obj2.GetComponent<Image>();
                    }
                }
                if (this.m_EpImg != null)
                {
                    float num3 = 0f;
                    if (totalValue > 0)
                    {
                        num3 = ((float) actorEp) / ((float) totalValue);
                    }
                    this.m_EpImg.CustomFillAmount(num3);
                }
            }
        }

        protected void UpdateFpsAndLag(CUIEvent uiEvent)
        {
            if (this._formScript != null)
            {
                GameObject widget = this._formScript.GetWidget(0x20);
                uint fFps = (uint) GameFramework.m_fFps;
                if (widget != null)
                {
                    Text component = widget.get_transform().FindChild("FPSText").get_gameObject().GetComponent<Text>();
                    component.set_text(string.Format("FPS {0}", fFps));
                    if (CheatCommandCommonEntry.CPU_CLOCK_ENABLE)
                    {
                        component.set_text(string.Format("FPS {0}\n{1}Mhz-{2}Mhz", fFps, Utility.GetCpuCurrentClock(), Utility.GetCpuMinClock()));
                    }
                }
                this._lastFps = fFps;
            }
        }

        public void UpdateGoBackProcessBar(uint curTime)
        {
            if (this.m_goBackProcessBar != null)
            {
                this.m_goBackProcessBar.Update(curTime);
            }
        }

        public void UpdateHpInfo()
        {
            if (this.m_selectedHero != 0)
            {
                int actorHp = this.m_selectedHero.handle.ValueComponent.actorHp;
                int totalValue = this.m_selectedHero.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalValue;
                if (this.m_HpImg == null)
                {
                    GameObject obj2 = Utility.FindChild(this.m_panelHeroInfo, "HpImg");
                    if (obj2 != null)
                    {
                        this.m_HpImg = obj2.GetComponent<Image>();
                    }
                }
                if (this.m_HpImg != null)
                {
                    this.m_HpImg.CustomFillAmount(((float) actorHp) / ((float) totalValue));
                }
                if (this.m_HpTxt == null)
                {
                    GameObject obj3 = Utility.FindChild(this.m_panelHeroInfo, "HpTxt");
                    if (obj3 != null)
                    {
                        this.m_HpTxt = obj3.GetComponent<Text>();
                    }
                }
                if (this.m_HpTxt != null)
                {
                    string str = string.Format("{0}/{1}", actorHp, totalValue);
                    this.m_HpTxt.set_text(str);
                }
            }
        }

        private void UpdateLearnSkillBtnState(int iSkillSlotType, bool bIsShow)
        {
            SkillButton button = this.GetButton((SkillSlotType) iSkillSlotType);
            if (button != null)
            {
                GameObject learnSkillButton = button.GetLearnSkillButton();
                if (learnSkillButton != null)
                {
                    learnSkillButton.CustomSetActive(bIsShow);
                    Singleton<EventRouter>.GetInstance().BroadCastEvent<int, bool>("HeroSkillLearnButtonStateChange", iSkillSlotType, bIsShow);
                }
            }
        }

        public void UpdateLogic(int delta)
        {
            if (this.m_isInBattle)
            {
                this.m_skillButtonManager.UpdateLogic(delta);
                if (this.m_showBuffDesc != null)
                {
                    this.m_showBuffDesc.UpdateBuffCD(delta);
                }
                Singleton<CBattleSelectTarget>.GetInstance().Update(this.m_selectedHero);
            }
        }

        public void UpdateMgcDefValueInfo()
        {
            if (this.m_selectedHero != 0)
            {
                int totalValue = this.m_selectedHero.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCDEFPT].totalValue;
                GameObject obj2 = Utility.FindChild(this.m_panelHeroInfo, "InfoPanel/TxtPanel/MgcDefTxt");
                if (obj2 != null)
                {
                    this.m_MgcDefTxt = obj2.GetComponent<Text>();
                }
                if (this.m_MgcDefTxt != null)
                {
                    this.m_MgcDefTxt.set_text(totalValue.ToString());
                }
            }
        }

        public void UpdatePhyDefValueInfo()
        {
            if (this.m_selectedHero != 0)
            {
                int totalValue = this.m_selectedHero.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYDEFPT].totalValue;
                if (this.m_PhyDefTxt == null)
                {
                    GameObject obj2 = Utility.FindChild(this.m_panelHeroInfo, "InfoPanel/TxtPanel/PhyDefTxt");
                    if (obj2 != null)
                    {
                        this.m_PhyDefTxt = obj2.GetComponent<Text>();
                    }
                }
                if (this.m_PhyDefTxt != null)
                {
                    this.m_PhyDefTxt.set_text(totalValue.ToString());
                }
            }
        }

        private void UpdateSkillLvlState(int iSkillSlotType, int iSkillLvl)
        {
            SkillButton button = this.GetButton((SkillSlotType) iSkillSlotType);
            if (button != null)
            {
                GameObject skillLvlImg = button.GetSkillLvlImg(iSkillLvl);
                if (skillLvlImg != null)
                {
                    skillLvlImg.CustomSetActive(true);
                }
            }
        }

        public CUIFormScript FormScript
        {
            get
            {
                return this._formScript;
            }
        }

        public bool OpenSpeakInBattle
        {
            get
            {
                return this.m_bOpenSpeak;
            }
        }

        public CUIContainerScript TextHudContainer
        {
            get
            {
                if (this._formScript != null)
                {
                    GameObject widget = this._formScript.GetWidget(0x18);
                    if (widget != null)
                    {
                        return widget.GetComponent<CUIContainerScript>();
                    }
                }
                return null;
            }
        }
    }
}

