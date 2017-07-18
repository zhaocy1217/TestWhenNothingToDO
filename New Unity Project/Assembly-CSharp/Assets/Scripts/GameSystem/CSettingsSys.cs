namespace Assets.Scripts.GameSystem
{
    using Apollo;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.UI;
    using CSProtocol;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    internal class CSettingsSys : Singleton<CSettingsSys>
    {
        private SettingType[] _availableSettingTypes;
        private int _availableSettingTypesCnt;
        private CUIFormScript _form;
        private CUIListScript _tabList;
        private readonly string PLAYCEHOLDER = "---";
        private static Color s_sliderFillColor = new Color(0f, 0.7843137f, 1f, 1f);
        public static string SETTING_FORM = "UGUI/Form/System/Settings/Form_Settings.prefab";

        private void ChangeImageColor(Transform imageTransform, Color color)
        {
            if (imageTransform != null)
            {
                Image component = imageTransform.GetComponent<Image>();
                if (component != null)
                {
                    component.set_color(color);
                }
            }
        }

        private void ChangeText(CUIListElementScript element, string InText)
        {
            DebugHelper.Assert(element != null);
            if (element != null)
            {
                Transform transform = element.get_gameObject().get_transform();
                DebugHelper.Assert(transform != null);
                Transform transform2 = transform.FindChild("Text");
                DebugHelper.Assert(transform2 != null);
                Text text = (transform2 == null) ? null : transform2.GetComponent<Text>();
                DebugHelper.Assert(text != null);
                if (text != null)
                {
                    text.set_text(InText);
                }
            }
        }

        private CUISliderEventScript GetSliderBarScript(GameObject bar)
        {
            Transform transform = bar.get_transform().FindChild("Slider");
            if (transform != null)
            {
                return transform.GetComponent<CUISliderEventScript>();
            }
            return bar.GetComponent<CUISliderEventScript>();
        }

        public override void Init()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_OpenForm, new CUIEventManager.OnUIEventHandler(this.onOpenSetting));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_ReqLogout, new CUIEventManager.OnUIEventHandler(this.onReqLogout));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_ConfirmLogout, new CUIEventManager.OnUIEventHandler(this.onConfirmLogout));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_SettingTypeChange, new CUIEventManager.OnUIEventHandler(this.OnSettingTabChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseSetting));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_OpenNetworkAccelerator, new CUIEventManager.OnUIEventHandler(this.OnNetAccChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_AutomaticOpenNetworkAccelerator, new CUIEventManager.OnUIEventHandler(this.OnAutoNetAccChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_PrivacyPolicy, new CUIEventManager.OnUIEventHandler(this.OnClickPrivacyPolicy));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_TermOfService, new CUIEventManager.OnUIEventHandler(this.OnClickTermOfService));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_Contract, new CUIEventManager.OnUIEventHandler(this.OnClickContract));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_UpdateTimer, new CUIEventManager.OnUIEventHandler(this.OnUpdateTimer));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_SurrenderCDReady, new CUIEventManager.OnUIEventHandler(this.OnSurrenderCDReady));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_ConfirmQuality_Accept, new CUIEventManager.OnUIEventHandler(this.onQualitySettingAccept));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_ConfirmQuality_Cancel, new CUIEventManager.OnUIEventHandler(this.onQualitySettingCancel));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_ClickMoveCameraGuide, new CUIEventManager.OnUIEventHandler(this.onClickMoveCameraGuide));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_ClickSkillCancleTypeHelp, new CUIEventManager.OnUIEventHandler(this.onClickSkillCancleTypeHelp));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_OnClickHeroInfoBarHelp, new CUIEventManager.OnUIEventHandler(this.onClickHeroInfoBarHelp));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_OnClickLastHitModeHelp, new CUIEventManager.OnUIEventHandler(this.onClickLastHitModeHelp));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_OnClickLockEnemyHeroHelp, new CUIEventManager.OnUIEventHandler(this.onClickLockEnemyHeroHelp));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_OnClickShowEnemyHeroHelp, new CUIEventManager.OnUIEventHandler(this.onClickShowEnemyHeroHelp));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_OnClickBPHeroSelectHelpBtn, new CUIEventManager.OnUIEventHandler(this.OnClickBPHeroSelectHelpBtn));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_OnSmartCastChange, new CUIEventManager.OnUIEventHandler(this.OnSmartCastChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_OnLunPanCastChange, new CUIEventManager.OnUIEventHandler(this.OnLunPanCastChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_OnPickNearestChange, new CUIEventManager.OnUIEventHandler(CSettingsSys.OnPickNearestChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_OnPickMinHpChange, new CUIEventManager.OnUIEventHandler(CSettingsSys.OnPickMinHpChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_OnCommonAttackType1Change, new CUIEventManager.OnUIEventHandler(CSettingsSys.OnCommonAttackType1Change));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_OnCommonAttackType2Change, new CUIEventManager.OnUIEventHandler(CSettingsSys.OnCommonAttackType2Change));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_OnSkillCanleType1Change, new CUIEventManager.OnUIEventHandler(CSettingsSys.OnSkillCanleType1Change));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_OnSkillCanleType2Change, new CUIEventManager.OnUIEventHandler(CSettingsSys.OnSkillCanleType2Change));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_OnJoyStickMoveChange, new CUIEventManager.OnUIEventHandler(CSettingsSys.OnJoyStickMoveChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_OnJoyStickNoMoveChange, new CUIEventManager.OnUIEventHandler(CSettingsSys.OnJoyStickNoMoveChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_OnRightJoyStickBtnLocChange, new CUIEventManager.OnUIEventHandler(CSettingsSys.OnRightJoyStickBtnLocChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_OnRightJoyStickFingerLocChange, new CUIEventManager.OnUIEventHandler(CSettingsSys.OnRightJoyStickFingerLocChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_Slider_onLunpanSensitivityChange, new CUIEventManager.OnUIEventHandler(this.OnSensitivityChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_Toggle_OnLockEnemyHeroChange, new CUIEventManager.OnUIEventHandler(this.OnLockEnemyHeroChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_Toggle_OnShowEnemyHeroChange, new CUIEventManager.OnUIEventHandler(this.OnShowEnemyHeroChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_OnNoneLastHitModeChange, new CUIEventManager.OnUIEventHandler(CSettingsSys.OnNoneLastHitModeChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_OnLastHitModeChange, new CUIEventManager.OnUIEventHandler(CSettingsSys.OnLastHitModeChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_Toggle_OnNotLockEnemyHeroChange, new CUIEventManager.OnUIEventHandler(this.OnNotLockEnemyHeroChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_Toggle_OnNotShowEnemyHeroChange, new CUIEventManager.OnUIEventHandler(this.OnNotShowEnemyHeroChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_Slider_On3DTouchBarChange, new CUIEventManager.OnUIEventHandler(this.On3DTouchBarChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_Slider_onModelLODChange, new CUIEventManager.OnUIEventHandler(this.OnModeLODChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_Slider_onParticleLODChange, new CUIEventManager.OnUIEventHandler(this.OnParticleLODChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_Slider_onSkillTipChange, new CUIEventManager.OnUIEventHandler(this.OnSkillTipChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_Slider_onFpsChange, new CUIEventManager.OnUIEventHandler(this.OnFpsChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_Slider_onInputChatChange, new CUIEventManager.OnUIEventHandler(this.OnInputChatChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_Slider_onMoveCameraChange, new CUIEventManager.OnUIEventHandler(this.OnMoveCameraChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_Slider_onHDBarChange, new CUIEventManager.OnUIEventHandler(this.OnHDBarChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_CameraHeight, new CUIEventManager.OnUIEventHandler(this.OnCameraHeightChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_Slider_onCameraSensitivityChange, new CUIEventManager.OnUIEventHandler(this.OnCameraSensitivityChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_Slider_OnHeroInfoBarChange, new CUIEventManager.OnUIEventHandler(this.OnHeroInfoBarChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_PauseMultiGame, new CUIEventManager.OnUIEventHandler(this.OnPauseMultiGame));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_Toggle_OnClickHeroSelectSortTypeDefault, new CUIEventManager.OnUIEventHandler(this.OnClickHeroSelectSortTypeDefault));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_Toggle_OnClickHeroSelectSortTypeProficiency, new CUIEventManager.OnUIEventHandler(this.OnClickHeroSelectSortTypeProficiency));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_Slider_3DTouchChange, new CUIEventManager.OnUIEventHandler(this.On3DTouchChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_Slider_onMusicChange, new CUIEventManager.OnUIEventHandler(this.OnMiusicChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_Slider_onSoundEffectChange, new CUIEventManager.OnUIEventHandler(this.OnSoundEffectChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_Slider_onVoiceChange, new CUIEventManager.OnUIEventHandler(this.OnVoiceChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_Slider_onVibrateChange, new CUIEventManager.OnUIEventHandler(this.OnVibrateChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_Slider_onMusicSensitivityChange, new CUIEventManager.OnUIEventHandler(this.OnSensitivityChangeMusic));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_Slider_onSoundSensitivityChange, new CUIEventManager.OnUIEventHandler(this.OnSensitivityChangeSound));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_Slider_onVoiceSensitivityChange, new CUIEventManager.OnUIEventHandler(this.OnSensitivityChangeVoice));
            this._availableSettingTypes = new SettingType[8];
            this._availableSettingTypesCnt = 0;
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_ReplayKitCourse, new CUIEventManager.OnUIEventHandler(this.onClickReplayKitCourse));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_OnClickReplayKitHelp, new CUIEventManager.OnUIEventHandler(this.onClickReplayKitHelp));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_Slider_onReplayKitEnableChange, new CUIEventManager.OnUIEventHandler(this.OnReplayKitEnableChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_Slider_onReplayKitEnableAutoModeChange, new CUIEventManager.OnUIEventHandler(this.OnReplayKitEnableAutoModeChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_KingTimeCourse, new CUIEventManager.OnUIEventHandler(this.OnClickOBFormOpen));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_Slider_onRecordKingTimeEnableChange, new CUIEventManager.OnUIEventHandler(this.OnRecordKingTimeEnableChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_Slider_onSecurePwdEnableChange, new CUIEventManager.OnUIEventHandler(this.OnSecurePwdEnableChange));
            Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.SECURE_PWD_STATUS_CHANGE, new Action(this, (IntPtr) this.OnSecurePwdStatusChange));
            Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.SECURE_PWD_OP_CANCEL, new Action(this, (IntPtr) this.OnSecurePwdStatusChange));
        }

        private void InitBasic()
        {
            if (Singleton<BattleLogic>.GetInstance().isRuning)
            {
                Singleton<CUINewFlagSystem>.GetInstance().DelNewFlag(this._form.m_formWidgets[0x42].get_transform().FindChild("Text").get_gameObject(), enNewFlagKey.New_SettingShowTargetInfo_V1, false);
            }
        }

        private void InitOperation()
        {
            Singleton<CUINewFlagSystem>.GetInstance().DelNewFlag(this._form.m_formWidgets[0x4f].get_transform().FindChild("Text").get_gameObject(), enNewFlagKey.New_Setting_LockEnemyHero_V1, false);
            Singleton<CUINewFlagSystem>.GetInstance().DelNewFlag(this._form.m_formWidgets[0x4d].get_transform().FindChild("Text").get_gameObject(), enNewFlagKey.New_Setting_ShowEnemyHeroBtn_V1, false);
            Singleton<CUINewFlagSystem>.GetInstance().DelNewFlag(this._form.m_formWidgets[0x4e].get_transform().FindChild("Text").get_gameObject(), enNewFlagKey.New_Setting_LastHitMode_V1, false);
        }

        private void InitRecorderWidget()
        {
            GameObject obj2 = this._form.m_formWidgets[0x39];
            if (obj2 != null)
            {
                if (!Singleton<CRecordUseSDK>.instance.GetRecorderGlobalCfgEnableFlag())
                {
                    obj2.CustomSetActive(false);
                }
                else
                {
                    Transform transform = obj2.get_transform().FindChild("Text");
                    if (transform != null)
                    {
                        Text component = transform.get_gameObject().GetComponent<Text>();
                        if (component != null)
                        {
                            component.set_text(Singleton<CTextManager>.GetInstance().GetText("RecordKingTimeName"));
                        }
                    }
                    Transform transform2 = obj2.get_transform().FindChild("Desc");
                    if (transform2 != null)
                    {
                        Text text2 = transform2.get_gameObject().GetComponent<Text>();
                        if (text2 != null)
                        {
                            text2.set_text(Singleton<CTextManager>.GetInstance().GetText("RecordKingTimeDesc"));
                        }
                    }
                    CUISliderEventScript sliderBarScript = this.GetSliderBarScript(obj2);
                    int num = !GameSettings.EnableKingTimeMode ? 0 : 1;
                    if ((sliderBarScript != null) && (((int) sliderBarScript.value) != num))
                    {
                        sliderBarScript.value = num;
                    }
                    obj2.CustomSetActive(true);
                }
            }
        }

        private void InitReplayKitSetting()
        {
            GameObject obj2 = this._form.m_formWidgets[0x56];
            GameObject obj3 = this._form.m_formWidgets[0x57];
            Text component = null;
            if (obj2 != null)
            {
                component = obj2.GetComponent<Text>();
            }
            Text text2 = null;
            if (obj3 != null)
            {
                text2 = obj3.GetComponent<Text>();
            }
            if (component != null)
            {
                component.set_text(Singleton<CTextManager>.GetInstance().GetText("ReplayKit_Enable_Desc"));
            }
            if (text2 != null)
            {
                text2.set_text(Singleton<CTextManager>.GetInstance().GetText("ReplayKit_Mode_Desc"));
            }
            this.GetSliderBarScript(this._form.m_formWidgets[0x54]).value = !GameSettings.EnableReplayKit ? ((float) 0) : ((float) 1);
            this.GetSliderBarScript(this._form.m_formWidgets[0x55]).value = !GameSettings.EnableReplayKitAutoMode ? ((float) 0) : ((float) 1);
        }

        private void InitSecurePwdSetting()
        {
            CSecurePwdSystem instance = Singleton<CSecurePwdSystem>.GetInstance();
            bool flag = instance.EnableStatus == PwdStatus.Enable;
            bool flag2 = instance.CloseStatus == PwdCloseStatus.Open;
            this.GetSliderBarScript(this._form.m_formWidgets[0x3d]).value = !flag ? ((float) 0) : ((float) 1);
            if (flag2)
            {
                this._form.m_formWidgets[0x3e].CustomSetActive(false);
                this._form.m_formWidgets[0x3f].CustomSetActive(true);
                CUITimerScript component = this._form.m_formWidgets[0x40].GetComponent<CUITimerScript>();
                if (component == null)
                {
                    this._form.m_formWidgets[0x3f].CustomSetActive(false);
                }
                else
                {
                    int leftTime = Singleton<CTimerManager>.GetInstance().GetLeftTime(Singleton<CSecurePwdSystem>.GetInstance().CloseTimerSeq);
                    if (leftTime > 0)
                    {
                        this._form.m_formWidgets[0x41].CustomSetActive(false);
                        this._form.m_formWidgets[0x48].CustomSetActive(true);
                        component.SetTotalTime((float) leftTime);
                        component.ReStartTimer();
                    }
                    else
                    {
                        this._form.m_formWidgets[0x3f].CustomSetActive(false);
                        this._form.m_formWidgets[0x41].CustomSetActive(true);
                        this._form.m_formWidgets[0x48].CustomSetActive(false);
                    }
                }
            }
            else
            {
                if (flag)
                {
                    this._form.m_formWidgets[0x3e].CustomSetActive(true);
                    this._form.m_formWidgets[0x41].CustomSetActive(false);
                    this._form.m_formWidgets[0x48].CustomSetActive(true);
                }
                else
                {
                    this._form.m_formWidgets[0x3e].CustomSetActive(false);
                    this._form.m_formWidgets[0x41].CustomSetActive(true);
                    this._form.m_formWidgets[0x48].CustomSetActive(false);
                }
                this._form.m_formWidgets[0x3f].CustomSetActive(false);
            }
        }

        private void InitSelectHero()
        {
            Singleton<CUINewFlagSystem>.GetInstance().DelNewFlag(this._form.m_formWidgets[80].get_transform().FindChild("Text").get_gameObject(), enNewFlagKey.New_Setting_HeroSelectHeroSortType_V1, false);
        }

        private void InitVoiceSetting()
        {
            this.ShowSoundSettingLevel(GameSettings.EnableMusic, null, SettingFormWidget.MusicLevel);
            this.ShowSoundSettingLevel(GameSettings.EnableSound, null, SettingFormWidget.SoundEffectLevel);
            this.ShowSoundSettingLevel(GameSettings.EnableVoice, null, SettingFormWidget.VoiceEffectLevel);
        }

        private void InitWidget(string formPath)
        {
            this._form = Singleton<CUIManager>.GetInstance().OpenForm(formPath, false, true);
            bool isRuning = Singleton<BattleLogic>.instance.isRuning;
            this._form.m_formWidgets[0x37].CustomSetActive(!isRuning);
            this._form.m_formWidgets[0x38].CustomSetActive(isRuning);
            if (isRuning)
            {
                this._tabList = this._form.m_formWidgets[0x12].GetComponent<CUIListScript>();
            }
            else
            {
                this._tabList = this._form.m_formWidgets[3].GetComponent<CUIListScript>();
            }
            DebugHelper.Assert(this._tabList != null);
            this.SetAvailableTabs();
            DebugHelper.Assert(this._availableSettingTypesCnt != 0, "Available Setting Type Array's Length Is 0 ?!");
            this._tabList.SetElementAmount(this._availableSettingTypesCnt);
            for (int i = 0; i < this._availableSettingTypesCnt; i++)
            {
                SettingType type = this._availableSettingTypes[i];
                CUIListElementScript elemenet = this._tabList.GetElemenet(i);
                switch (type)
                {
                    case SettingType.Basic:
                        this.ChangeText(elemenet, "基础设置");
                        Singleton<CUINewFlagSystem>.GetInstance().AddNewFlag(elemenet.get_gameObject(), enNewFlagKey.New_BasicSettingTab_V2, enNewFlagPos.enTopRight, 1f, 0f, 0f, enNewFlagType.enRedDot);
                        break;

                    case SettingType.Operation:
                        this.ChangeText(elemenet, "操作设置");
                        Singleton<CUINewFlagSystem>.GetInstance().AddNewFlag(elemenet.get_gameObject(), enNewFlagKey.New_OperationTab_V1, enNewFlagPos.enTopRight, 1f, 0f, 0f, enNewFlagType.enRedDot);
                        break;

                    case SettingType.SelectHero:
                        this.ChangeText(elemenet, "选将设置");
                        break;

                    case SettingType.VoiceSetting:
                        this.ChangeText(elemenet, "音效设置");
                        break;

                    case SettingType.NetAcc:
                        this.ChangeText(elemenet, "网络加速");
                        break;

                    case SettingType.KingTime:
                        this.ChangeText(elemenet, "录像设置");
                        Singleton<CUINewFlagSystem>.GetInstance().AddNewFlag(elemenet.get_gameObject(), enNewFlagKey.New_KingTimeTab_V2, enNewFlagPos.enTopRight, 1f, 0f, 0f, enNewFlagType.enRedDot);
                        break;

                    case SettingType.SecurePwd:
                        this.ChangeText(elemenet, "二级密码");
                        break;

                    case SettingType.ReplayKit:
                        this.ChangeText(elemenet, Singleton<CTextManager>.GetInstance().GetText("ReplayKit_Label_Name"));
                        break;
                }
            }
            this._tabList.SelectElement(0, true);
            if (isRuning)
            {
                this._form.m_formWidgets[0x36].CustomSetActive(false);
                this._form.m_formWidgets[0x35].CustomSetActive(true);
                this.GetSliderBarScript(this._form.m_formWidgets[0x16]).value = GameSettings.FpsShowType;
                this.GetSliderBarScript(this._form.m_formWidgets[0x2a]).value = (float) GameSettings.TheCameraMoveType;
                this.GetSliderBarScript(this._form.m_formWidgets[0x42]).value = !GameSettings.EnableHeroInfo ? ((float) 0) : ((float) 1);
                Singleton<CUINewFlagSystem>.GetInstance().AddNewFlag(this._form.m_formWidgets[0x42].get_transform().FindChild("Text").get_gameObject(), enNewFlagKey.New_SettingShowTargetInfo_V1, enNewFlagPos.enTopRight, 0.8f, 6.6f, 9.5f, enNewFlagType.enNewFlag);
                this.UpdateCameraSensitivitySlider();
                SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
                GameObject obj2 = this._form.m_formWidgets[0];
                if (((obj2 != null) && (curLvelContext != null)) && curLvelContext.IsMultilModeWithWarmBattle())
                {
                    obj2.CustomSetActive(false);
                }
                else
                {
                    obj2.CustomSetActive(true);
                }
                this.ValidatePauseState();
                GameObject widget = this._form.GetWidget(0x1a);
                if (widget == null)
                {
                    return;
                }
                if (Singleton<CSurrenderSystem>.instance.CanSurrender())
                {
                    widget.CustomSetActive(true);
                    GameObject p = widget;
                    if (p == null)
                    {
                        return;
                    }
                    Button component = p.GetComponent<Button>();
                    if (component == null)
                    {
                        return;
                    }
                    GameObject obj5 = Utility.FindChild(p, "CountDown");
                    if (obj5 == null)
                    {
                        return;
                    }
                    CUITimerScript componetInChild = Utility.GetComponetInChild<CUITimerScript>(obj5, "timerSurrender");
                    if (componetInChild == null)
                    {
                        return;
                    }
                    uint time = 0;
                    if (Singleton<CSurrenderSystem>.instance.InSurrenderCD(out time))
                    {
                        obj5.CustomSetActive(true);
                        CUICommonSystem.SetButtonEnable(component, false, false, true);
                        componetInChild.SetTotalTime((float) time);
                        componetInChild.ReStartTimer();
                    }
                    else
                    {
                        obj5.CustomSetActive(false);
                        CUICommonSystem.SetButtonEnable(component, true, true, true);
                    }
                }
                else
                {
                    widget.CustomSetActive(false);
                }
            }
            else
            {
                this._form.m_formWidgets[0x36].CustomSetActive(true);
                this._form.m_formWidgets[0x35].CustomSetActive(false);
                this._form.m_formWidgets[0x45].CustomSetActive(false);
                this.SetHDBarShow();
                CUISliderEventScript sliderBarScript = this.GetSliderBarScript(this._form.m_formWidgets[6]);
                CUISliderEventScript script4 = this.GetSliderBarScript(this._form.m_formWidgets[7]);
                sliderBarScript.value = sliderBarScript.MaxValue - GameSettings.ModelLOD;
                script4.value = script4.MaxValue - GameSettings.ParticleLOD;
                this.GetSliderBarScript(this._form.m_formWidgets[0x31]).value = !GameSettings.EnableHDMode ? ((float) 0) : ((float) 1);
                if (GameSettings.HeroSelectHeroViewSortType == CMallSortHelper.HeroViewSortType.Name)
                {
                    this._form.m_formWidgets[70].GetComponent<Toggle>().set_isOn(true);
                    this._form.m_formWidgets[0x47].GetComponent<Toggle>().set_isOn(false);
                }
                else
                {
                    this._form.m_formWidgets[70].GetComponent<Toggle>().set_isOn(false);
                    this._form.m_formWidgets[0x47].GetComponent<Toggle>().set_isOn(true);
                }
                Text text = this._form.m_formWidgets[9].get_transform().FindChild("Text").get_gameObject().GetComponent<Text>();
                ApolloAccountInfo accountInfo = Singleton<ApolloHelper>.GetInstance().GetAccountInfo(false);
                if (accountInfo != null)
                {
                    if (accountInfo.Platform == ApolloPlatform.QQ)
                    {
                        text.set_text(Singleton<CTextManager>.GetInstance().GetText("Common_Login_QQ"));
                    }
                    else if (accountInfo.Platform == ApolloPlatform.Wechat)
                    {
                        text.set_text(Singleton<CTextManager>.GetInstance().GetText("Common_Login_Weixin"));
                    }
                    else if (accountInfo.Platform == ApolloPlatform.WTLogin)
                    {
                        text.set_text(Singleton<CTextManager>.GetInstance().GetText("Common_Login_PC"));
                    }
                    else if (accountInfo.Platform == ApolloPlatform.Guest)
                    {
                        text.set_text(Singleton<CTextManager>.GetInstance().GetText("Common_Login_Guest"));
                    }
                }
            }
            this.GetSliderBarScript(this._form.m_formWidgets[0x10]).value = GameSettings.CameraHeight;
            this.GetSliderBarScript(this._form.m_formWidgets[8]).value = !GameSettings.EnableOutline ? ((float) 0) : ((float) 1);
            this.GetSliderBarScript(this._form.m_formWidgets[0x26]).value = GameSettings.InBattleInputChatEnable;
            this.GetSliderBarScript(this._form.m_formWidgets[0x52]).value = !GameSettings.Unity3DTouchEnable ? ((float) 0) : ((float) 1);
            this.Set3DTouchBarShow();
            Singleton<CUINewFlagSystem>.GetInstance().AddNewFlag(this._form.m_formWidgets[80].get_transform().FindChild("Text").get_gameObject(), enNewFlagKey.New_Setting_HeroSelectHeroSortType_V1, enNewFlagPos.enTopCenter, 0.8f, 80f, 10f, enNewFlagType.enNewFlag);
            if (GameSettings.TheCastType == CastType.LunPanCast)
            {
                this._form.m_formWidgets[11].GetComponent<Toggle>().set_isOn(false);
                this._form.m_formWidgets[12].GetComponent<Toggle>().set_isOn(true);
                this.LunPanSettingsStatusChange(true);
            }
            else
            {
                this._form.m_formWidgets[11].GetComponent<Toggle>().set_isOn(true);
                this._form.m_formWidgets[12].GetComponent<Toggle>().set_isOn(false);
                this.LunPanSettingsStatusChange(false);
            }
            this._form.m_formWidgets[14].GetComponent<Toggle>().set_isOn(GameSettings.TheSelectType == SelectEnemyType.SelectNearest);
            this._form.m_formWidgets[15].GetComponent<Toggle>().set_isOn(GameSettings.TheSelectType == SelectEnemyType.SelectLowHp);
            this._form.m_formWidgets[0x18].GetComponent<Toggle>().set_isOn(GameSettings.TheCommonAttackType == CommonAttactType.Type1);
            this._form.m_formWidgets[0x19].GetComponent<Toggle>().set_isOn(GameSettings.TheCommonAttackType == CommonAttactType.Type2);
            this._form.m_formWidgets[0x49].GetComponent<Toggle>().set_isOn(GameSettings.TheLastHitMode == LastHitMode.None);
            this._form.m_formWidgets[0x4a].GetComponent<Toggle>().set_isOn(GameSettings.TheLastHitMode == LastHitMode.LastHit);
            this._form.m_formWidgets[20].GetComponent<Toggle>().set_isOn(GameSettings.JoyStickShowType == 0);
            this._form.m_formWidgets[0x15].GetComponent<Toggle>().set_isOn(GameSettings.JoyStickShowType == 1);
            this._form.m_formWidgets[0x27].GetComponent<Toggle>().set_isOn(GameSettings.TheSkillCancleType == SkillCancleType.AreaCancle);
            this._form.m_formWidgets[40].GetComponent<Toggle>().set_isOn(GameSettings.TheSkillCancleType == SkillCancleType.DisitanceCancle);
            this.GetSliderBarScript(this._form.m_formWidgets[13]).value = GameSettings.LunPanSensitivity;
            this._form.m_formWidgets[0x43].GetComponent<Toggle>().set_isOn(GameSettings.LunPanLockEnemyHeroMode);
            this._form.m_formWidgets[0x44].GetComponent<Toggle>().set_isOn(!GameSettings.LunPanLockEnemyHeroMode);
            this._form.m_formWidgets[0x4b].GetComponent<Toggle>().set_isOn(GameSettings.ShowEnemyHeroHeadBtnMode);
            this._form.m_formWidgets[0x4c].GetComponent<Toggle>().set_isOn(!GameSettings.ShowEnemyHeroHeadBtnMode);
            Singleton<CUINewFlagSystem>.GetInstance().AddNewFlag(this._form.m_formWidgets[0x4f].get_transform().FindChild("Text").get_gameObject(), enNewFlagKey.New_Setting_LockEnemyHero_V1, enNewFlagPos.enTopLeft, 0.8f, 122.5f, 6.6f, enNewFlagType.enNewFlag);
            Singleton<CUINewFlagSystem>.GetInstance().AddNewFlag(this._form.m_formWidgets[0x4d].get_transform().FindChild("Text").get_gameObject(), enNewFlagKey.New_Setting_ShowEnemyHeroBtn_V1, enNewFlagPos.enTopCenter, 0.8f, 83f, 6.6f, enNewFlagType.enNewFlag);
            Singleton<CUINewFlagSystem>.GetInstance().AddNewFlag(this._form.m_formWidgets[0x4e].get_transform().FindChild("Text").get_gameObject(), enNewFlagKey.New_Setting_LastHitMode_V1, enNewFlagPos.enTopCenter, 0.8f, 83f, 6.6f, enNewFlagType.enNewFlag);
            this.GetSliderBarScript(this._form.m_formWidgets[0x1c]).value = GameSettings.MusicEffectLevel * 0.01f;
            this.GetSliderBarScript(this._form.m_formWidgets[0x1d]).value = GameSettings.SoundEffectLevel * 0.01f;
            this.GetSliderBarScript(this._form.m_formWidgets[30]).value = GameSettings.VoiceEffectLevel * 0.01f;
            this.GetSliderBarScript(this._form.m_formWidgets[4]).value = !GameSettings.EnableMusic ? ((float) 0) : ((float) 1);
            this.GetSliderBarScript(this._form.m_formWidgets[5]).value = !GameSettings.EnableSound ? ((float) 0) : ((float) 1);
            this.GetSliderBarScript(this._form.m_formWidgets[0x17]).value = !GameSettings.EnableVoice ? ((float) 0) : ((float) 1);
            this.GetSliderBarScript(this._form.m_formWidgets[0x29]).value = !GameSettings.EnableVibrate ? ((float) 0) : ((float) 1);
            this.InitVoiceSetting();
            this.GetSliderBarScript(this._form.m_formWidgets[0x23]).value = !NetworkAccelerator.IsNetAccConfigOpen() ? ((float) 0) : ((float) 1);
            this.GetSliderBarScript(this._form.m_formWidgets[0x24]).value = !NetworkAccelerator.IsAutoNetAccConfigOpen() ? ((float) 0) : ((float) 1);
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                this._form.m_formWidgets[0x58].GetComponent<Toggle>().set_isOn(!CLadderSystem.IsRecentUsedHeroMaskSet(ref masterRoleInfo.recentUseHero.dwCtrlMask, COM_RECENT_USED_HERO_MASK.COM_RECENT_USED_HERO_HIDE));
                this._form.m_formWidgets[0x59].GetComponent<Toggle>().set_isOn(CLadderSystem.IsRecentUsedHeroMaskSet(ref masterRoleInfo.recentUseHero.dwCtrlMask, COM_RECENT_USED_HERO_MASK.COM_RECENT_USED_HERO_HIDE));
            }
            if (CSysDynamicBlock.bLobbyEntryBlocked)
            {
                Transform transform = this._form.get_transform().FindChild("BasicSetting/BasicLobbyOnlyWidgets/Layout/Panelelem2/HelpMe");
                if (transform != null)
                {
                    transform.get_gameObject().CustomSetActive(false);
                }
            }
        }

        private void LunPanSettingsStatusChange(bool bEnable)
        {
            this.SliderStatusChange(bEnable, null, SettingFormWidget.OpLunPanSensi);
            this.ToggleStatusChange(bEnable, SettingFormWidget.RightJoyStickBtnLoc);
            this.ToggleStatusChange(bEnable, SettingFormWidget.RightJoyStickFingerLoc);
            this.ToggleStatusChange(bEnable, SettingFormWidget.LunPanLock);
            this.ToggleStatusChange(bEnable, SettingFormWidget.EnemyHeroLock);
        }

        private void On3DTouchBarChange(CUIEvent uiEvent)
        {
            GameSettings.Unity3DTouchEnable = uiEvent.m_eventParams.sliderValue == 1f;
        }

        private void On3DTouchChange(CUIEvent uiEvent)
        {
            GameSettings.Unity3DTouchEnable = uiEvent.m_eventParams.sliderValue == 1f;
        }

        private void OnAutoNetAccChange(CUIEvent uiEvent)
        {
            int sliderValue = (int) uiEvent.m_eventParams.sliderValue;
            int num2 = !NetworkAccelerator.IsAutoNetAccConfigOpen() ? 0 : 1;
            NetworkAccelerator.SetAutoNetAccConfig(sliderValue > 0);
            if ((num2 != sliderValue) && NetworkAccelerator.IsAutoNetAccConfigOpen())
            {
                this.GetSliderBarScript(this._form.m_formWidgets[0x23]).value = 1f;
            }
            this.SetUseACC();
        }

        private void OnCameraHeightChange(CUIEvent uiEvent)
        {
            GameSettings.CameraHeight = Convert.ToInt32(uiEvent.m_eventParams.sliderValue);
        }

        private void OnCameraSensitivityChange(CUIEvent uiEvent)
        {
            GameSettings.SetCurCameraSensitivity(uiEvent.m_eventParams.sliderValue);
            this.UpdateCameraSensitivitySlider();
        }

        private void OnClickBPHeroSelectHelpBtn(CUIEvent uiEvent)
        {
            Singleton<CBattleGuideManager>.GetInstance().OpenBannerDlgByBannerGuideId(0x11, null, false);
        }

        private void OnClickContract(CUIEvent uiEvent)
        {
            CUICommonSystem.OpenUrl("http://game.qq.com/contract.shtml", false, 0);
        }

        private void onClickHeroInfoBarHelp(CUIEvent uiEvent)
        {
            Singleton<CBattleGuideManager>.GetInstance().OpenBannerDlgByBannerGuideId(10, null, false);
        }

        private void OnClickHeroSelectSortTypeDefault(CUIEvent uiEvent)
        {
            if (uiEvent.m_eventParams.togleIsOn)
            {
                GameSettings.HeroSelectHeroViewSortType = CMallSortHelper.HeroViewSortType.Name;
            }
        }

        private void OnClickHeroSelectSortTypeProficiency(CUIEvent uiEvent)
        {
            if (uiEvent.m_eventParams.togleIsOn)
            {
                GameSettings.HeroSelectHeroViewSortType = CMallSortHelper.HeroViewSortType.Proficiency;
            }
        }

        private void onClickKingTimeHelp(CUIEvent uiEvent)
        {
            Singleton<CBattleGuideManager>.GetInstance().OpenBannerDlgByBannerGuideId(8, null, false);
        }

        private void onClickLastHitModeHelp(CUIEvent uiEvent)
        {
            Singleton<CBattleGuideManager>.GetInstance().OpenBannerDlgByBannerGuideId(12, null, false);
        }

        private void onClickLockEnemyHeroHelp(CUIEvent uiEvent)
        {
            Singleton<CBattleGuideManager>.GetInstance().OpenBannerDlgByBannerGuideId(13, null, false);
        }

        private void onClickMoveCameraGuide(CUIEvent uiEvent)
        {
            Singleton<CBattleGuideManager>.GetInstance().OpenBannerDlgByBannerGuideId(1, null, false);
        }

        private void OnClickOBFormOpen(CUIEvent uiEvent)
        {
            if (this._form != null)
            {
                Singleton<CUIManager>.instance.CloseForm(this._form);
            }
            Singleton<COBSystem>.instance.OnOBFormOpen(uiEvent);
        }

        private void OnClickPrivacyPolicy(CUIEvent uiEvent)
        {
            CUICommonSystem.OpenUrl("http://www.tencent.com/en-us/zc/privacypolicy.shtml", false, 0);
        }

        private void onClickReplayKitCourse(CUIEvent uiEvent)
        {
            string text = Singleton<CTextManager>.GetInstance().GetText("ReplayKit_Course_Link");
            if (!string.IsNullOrEmpty(text))
            {
                CUICommonSystem.OpenUrl(text, false, 0);
            }
        }

        private void onClickReplayKitHelp(CUIEvent uiEvent)
        {
            Singleton<CBattleGuideManager>.GetInstance().OpenBannerDlgByBannerGuideId(8, null, false);
        }

        private void onClickShowEnemyHeroHelp(CUIEvent uiEvent)
        {
            Singleton<CBattleGuideManager>.GetInstance().OpenBannerDlgByBannerGuideId(14, null, false);
        }

        private void onClickSkillCancleTypeHelp(CUIEvent uiEvent)
        {
            Singleton<CBattleGuideManager>.GetInstance().OpenBannerDlgByBannerGuideId(6, null, false);
        }

        private void OnClickTermOfService(CUIEvent uiEvent)
        {
            CUICommonSystem.OpenUrl("http://www.tencent.com/en-us/zc/termsofservice.shtml", false, 0);
        }

        protected void OnCloseSetting(CUIEvent uiEvent)
        {
            this._availableSettingTypesCnt = 0;
            this.UnInitWidget();
            GameSettings.Save();
        }

        private static void OnCommonAttackType1Change(CUIEvent uiEvent)
        {
            if (uiEvent.m_eventParams.togleIsOn)
            {
                GameSettings.TheCommonAttackType = CommonAttactType.Type1;
            }
        }

        private static void OnCommonAttackType2Change(CUIEvent uiEvent)
        {
            if (uiEvent.m_eventParams.togleIsOn)
            {
                sliderMoveCameraAdjustment();
                GameSettings.TheCommonAttackType = CommonAttactType.Type2;
            }
        }

        private void onConfirmLogout(CUIEvent uiEvent)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x3f8);
            msg.stPkgData.stGameLogoutReq.iLogoutType = 0;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        private void OnFpsChange(CUIEvent uiEvent)
        {
            GameSettings.FpsShowType = (int) uiEvent.m_eventParams.sliderValue;
        }

        private void OnHDBarChange(CUIEvent uiEvent)
        {
            GameSettings.EnableHDMode = uiEvent.m_eventParams.sliderValue == 1f;
        }

        private void OnHeroInfoBarChange(CUIEvent uiEvent)
        {
            GameSettings.EnableHeroInfo = uiEvent.m_eventParams.sliderValue == 1f;
        }

        private void OnInputChatChange(CUIEvent uiEvent)
        {
            GameSettings.InBattleInputChatEnable = (int) uiEvent.m_eventParams.sliderValue;
        }

        private static void OnJoyStickMoveChange(CUIEvent uiEvent)
        {
            if (uiEvent.m_eventParams.togleIsOn)
            {
                GameSettings.JoyStickMoveType = 0;
            }
        }

        private static void OnJoyStickNoMoveChange(CUIEvent uiEvent)
        {
            if (uiEvent.m_eventParams.togleIsOn)
            {
                GameSettings.JoyStickMoveType = 1;
            }
        }

        private static void OnLastHitModeChange(CUIEvent uiEvent)
        {
            if (uiEvent.m_eventParams.togleIsOn)
            {
                GameSettings.TheLastHitMode = LastHitMode.LastHit;
            }
        }

        private void OnLockEnemyHeroChange(CUIEvent uiEvent)
        {
            if (uiEvent.m_eventParams.togleIsOn)
            {
                GameSettings.LunPanLockEnemyHeroMode = true;
            }
        }

        private void OnLunPanCastChange(CUIEvent uiEvent)
        {
            if (uiEvent.m_eventParams.togleIsOn)
            {
                this.LunPanSettingsStatusChange(true);
                GameSettings.TheCastType = CastType.LunPanCast;
            }
        }

        private void OnMiusicChange(CUIEvent uiEvent)
        {
            GameSettings.EnableMusic = uiEvent.m_eventParams.sliderValue == 1f;
            this.ShowSoundSettingLevel(GameSettings.EnableMusic, null, SettingFormWidget.MusicLevel);
        }

        private void OnModeLODChange(CUIEvent uiEvent)
        {
            int num = Convert.ToInt32(uiEvent.m_eventParams.sliderValue);
            if ((((uiEvent.m_srcWidgetScript as CUISliderEventScript).MaxValue - num) < GameSettings.ModelLOD) && (PlayerPrefs.GetInt("degrade", 0) == 1))
            {
                stUIEventParams par = new stUIEventParams();
                par.tag = 0;
                par.tag2 = num;
                Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("Setting_Quality_Confirm"), enUIEventID.Settings_ConfirmQuality_Accept, enUIEventID.Settings_ConfirmQuality_Cancel, par, false);
            }
            else
            {
                CUISliderEventScript srcWidgetScript = uiEvent.m_srcWidgetScript as CUISliderEventScript;
                if (srcWidgetScript != null)
                {
                    GameSettings.ModelLOD = (uiEvent.m_srcWidgetScript as CUISliderEventScript).MaxValue - num;
                }
            }
        }

        private void OnMoveCameraChange(CUIEvent uiEvent)
        {
            int sliderValue = (int) uiEvent.m_eventParams.sliderValue;
            if ((GameSettings.TheCommonAttackType == CommonAttactType.Type2) && (sliderValue == 1))
            {
                sliderValue = 2;
            }
            GameSettings.TheCameraMoveType = (CameraMoveType) sliderValue;
            this.UpdateCameraSensitivitySlider();
        }

        private void OnNetAccChange(CUIEvent uiEvent)
        {
            NetworkAccelerator.SetNetAccConfig(Convert.ToInt32(uiEvent.m_eventParams.sliderValue) > 0);
            this.SetUseACC();
        }

        private static void OnNoneLastHitModeChange(CUIEvent uiEvent)
        {
            if (uiEvent.m_eventParams.togleIsOn)
            {
                GameSettings.TheLastHitMode = LastHitMode.None;
            }
        }

        private void OnNotLockEnemyHeroChange(CUIEvent uiEvent)
        {
            if (uiEvent.m_eventParams.togleIsOn)
            {
                GameSettings.LunPanLockEnemyHeroMode = false;
            }
        }

        private void OnNotShowEnemyHeroChange(CUIEvent uiEvent)
        {
            if (uiEvent.m_eventParams.togleIsOn)
            {
                GameSettings.ShowEnemyHeroHeadBtnMode = false;
                if ((Singleton<CBattleSystem>.instance.FightForm != null) && (Singleton<CBattleSystem>.instance.FightForm.m_enemyHeroAtkBtn != null))
                {
                    Singleton<CBattleSystem>.instance.FightForm.m_enemyHeroAtkBtn.HideEnemyHeroHeadBtn();
                }
            }
        }

        private void onOpenSetting(CUIEvent uiEvent)
        {
            this.InitWidget(SETTING_FORM);
            Singleton<CUINewFlagSystem>.instance.SetNewFlagForSettingEntry(false);
        }

        private void OnParticleLODChange(CUIEvent uiEvent)
        {
            int num = Convert.ToInt32(uiEvent.m_eventParams.sliderValue);
            if ((((uiEvent.m_srcWidgetScript as CUISliderEventScript).MaxValue - num) < GameSettings.ParticleLOD) && (PlayerPrefs.GetInt("degrade", 0) == 1))
            {
                stUIEventParams par = new stUIEventParams();
                par.tag = 1;
                par.tag2 = num;
                Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("Setting_Quality_Confirm"), enUIEventID.Settings_ConfirmQuality_Accept, enUIEventID.Settings_ConfirmQuality_Cancel, par, false);
            }
            else
            {
                CUISliderEventScript srcWidgetScript = uiEvent.m_srcWidgetScript as CUISliderEventScript;
                if (srcWidgetScript != null)
                {
                    GameSettings.ParticleLOD = (uiEvent.m_srcWidgetScript as CUISliderEventScript).MaxValue - num;
                }
            }
        }

        private void OnPauseMultiGame(CUIEvent uiEvent)
        {
            PauseControl pauseControl = Singleton<CBattleSystem>.GetInstance().pauseControl;
            if (pauseControl != null)
            {
                pauseControl.RequestPause(true);
                this._form.Close();
            }
        }

        private static void OnPickMinHpChange(CUIEvent uiEvent)
        {
            if (uiEvent.m_eventParams.togleIsOn)
            {
                GameSettings.TheSelectType = SelectEnemyType.SelectLowHp;
            }
        }

        private static void OnPickNearestChange(CUIEvent uiEvent)
        {
            if (uiEvent.m_eventParams.togleIsOn)
            {
                GameSettings.TheSelectType = SelectEnemyType.SelectNearest;
            }
        }

        private void onQualitySettingAccept(CUIEvent uiEvent)
        {
            stUIEventParams eventParams = uiEvent.m_eventParams;
            switch (eventParams.tag)
            {
                case 0:
                    GameSettings.ModelLOD = this.GetSliderBarScript(this._form.m_formWidgets[6]).MaxValue - eventParams.tag2;
                    break;

                case 1:
                    GameSettings.ParticleLOD = this.GetSliderBarScript(this._form.m_formWidgets[7]).MaxValue - eventParams.tag2;
                    break;

                case 2:
                    GameSettings.EnableOutline = eventParams.tag2 != 0;
                    break;
            }
            PlayerPrefs.SetInt("degrade", 0);
            PlayerPrefs.Save();
            Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Force_Modify_Quality", null, true);
        }

        private void onQualitySettingCancel(CUIEvent uiEvent)
        {
            Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Give_Up_Modify_Quality", null, true);
            CUISliderEventScript sliderBarScript = this.GetSliderBarScript(this._form.m_formWidgets[6]);
            CUISliderEventScript script2 = this.GetSliderBarScript(this._form.m_formWidgets[7]);
            sliderBarScript.value = sliderBarScript.MaxValue - GameSettings.ModelLOD;
            script2.value = script2.MaxValue - GameSettings.ParticleLOD;
            this.GetSliderBarScript(this._form.m_formWidgets[8]).value = !GameSettings.EnableOutline ? ((float) 0) : ((float) 1);
        }

        public void OnRecordKingTimeEnableChange(CUIEvent uiEvent)
        {
            bool flag = uiEvent.m_eventParams.sliderValue == 1f;
            bool enableKingTimeMode = GameSettings.EnableKingTimeMode;
            if (enableKingTimeMode != flag)
            {
                GameSettings.EnableKingTimeMode = flag;
                if (flag && !enableKingTimeMode)
                {
                    if (GameSettings.EnableKingTimeMode)
                    {
                        GameSettings.EnableReplayKit = false;
                    }
                    Singleton<CRecordUseSDK>.instance.OpenRecorderCheck(this._form.m_formWidgets[0x39]);
                }
            }
        }

        private void OnReplayKitEnableAutoModeChange(CUIEvent uiEvent)
        {
            if (((uiEvent.m_eventParams.sliderValue != 1f) ? 0 : 1) != 0)
            {
                if ((!Singleton<CReplayKitSys>.GetInstance().Enable || !Singleton<CReplayKitSys>.GetInstance().Cap) || !GameSettings.EnableReplayKit)
                {
                    this.GetSliderBarScript(this._form.m_formWidgets[0x55]).value = 0f;
                    Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("ReplayKit_Enable_First"), false, 1.5f, null, new object[0]);
                    return;
                }
                Singleton<CReplayKitSys>.GetInstance().CheckStorage(true);
            }
            GameSettings.EnableReplayKitAutoMode = uiEvent.m_eventParams.sliderValue == 1f;
        }

        private void OnReplayKitEnableChange(CUIEvent uiEvent)
        {
            bool flag = uiEvent.m_eventParams.sliderValue == 1f;
            if (flag && (!Singleton<CReplayKitSys>.GetInstance().Enable || !Singleton<CReplayKitSys>.GetInstance().Cap))
            {
                this.GetSliderBarScript(this._form.m_formWidgets[0x54]).value = 0f;
                Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("ReplayKit_Not_Support"), false, 1.5f, null, new object[0]);
            }
            else if (Singleton<CReplayKitSys>.GetInstance().CheckStorage(true) == CReplayKitSys.StorageStatus.Disable)
            {
                this.GetSliderBarScript(this._form.m_formWidgets[0x54]).value = 0f;
            }
            else
            {
                GameSettings.EnableReplayKit = flag;
                if (GameSettings.EnableReplayKit)
                {
                    GameSettings.EnableKingTimeMode = false;
                }
                if (!GameSettings.EnableReplayKit)
                {
                    this.GetSliderBarScript(this._form.m_formWidgets[0x55]).value = 0f;
                }
            }
        }

        private void onReqLogout(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("Common_Exit_Tip"), enUIEventID.Settings_ConfirmLogout, enUIEventID.None, false);
        }

        private static void OnRightJoyStickBtnLocChange(CUIEvent uiEvent)
        {
            if (uiEvent.m_eventParams.togleIsOn)
            {
                GameSettings.JoyStickShowType = 0;
            }
        }

        private static void OnRightJoyStickFingerLocChange(CUIEvent uiEvent)
        {
            if (uiEvent.m_eventParams.togleIsOn)
            {
                GameSettings.JoyStickShowType = 1;
            }
        }

        private void OnSecurePwdEnableChange(CUIEvent uiEvent)
        {
            bool flag = uiEvent.m_eventParams.sliderValue == 1f;
            CSecurePwdSystem instance = Singleton<CSecurePwdSystem>.GetInstance();
            if (flag)
            {
                if (instance.EnableStatus != PwdStatus.Enable)
                {
                    Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.SecurePwd_OpenSetPwdForm);
                }
            }
            else if (instance.EnableStatus != PwdStatus.Disable)
            {
                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.SecurePwd_OpenClosePwdForm);
            }
        }

        private void OnSecurePwdStatusChange()
        {
            if (this._form != null)
            {
                this.InitSecurePwdSetting();
            }
        }

        private void OnSensitivityChange(CUIEvent uiEvent)
        {
            GameSettings.LunPanSensitivity = uiEvent.m_eventParams.sliderValue;
        }

        private void OnSensitivityChangeMusic(CUIEvent uiEvent)
        {
            GameSettings.MusicEffectLevel = uiEvent.m_eventParams.sliderValue * 100f;
        }

        private void OnSensitivityChangeSound(CUIEvent uiEvent)
        {
            GameSettings.SoundEffectLevel = uiEvent.m_eventParams.sliderValue * 100f;
        }

        private void OnSensitivityChangeVoice(CUIEvent uiEvent)
        {
            GameSettings.VoiceEffectLevel = uiEvent.m_eventParams.sliderValue * 100f;
        }

        protected void OnSettingTabChange(CUIEvent uiEvent)
        {
            if ((this._form != null) && (this._tabList != null))
            {
                int selectedIndex = this._tabList.GetSelectedIndex();
                if ((selectedIndex >= 0) && (selectedIndex < this._availableSettingTypesCnt))
                {
                    switch (this._availableSettingTypes[selectedIndex])
                    {
                        case SettingType.Basic:
                            this._form.m_formWidgets[1].CustomSetActive(true);
                            this._form.m_formWidgets[2].CustomSetActive(false);
                            this._form.m_formWidgets[0x1b].CustomSetActive(false);
                            this._form.m_formWidgets[0x25].CustomSetActive(false);
                            this._form.m_formWidgets[0x2b].CustomSetActive(false);
                            this._form.m_formWidgets[60].CustomSetActive(false);
                            this._form.m_formWidgets[0x51].CustomSetActive(false);
                            this._form.m_formWidgets[0x53].CustomSetActive(false);
                            this.InitBasic();
                            break;

                        case SettingType.Operation:
                            this._form.m_formWidgets[1].CustomSetActive(false);
                            this._form.m_formWidgets[2].CustomSetActive(true);
                            this._form.m_formWidgets[0x1b].CustomSetActive(false);
                            this._form.m_formWidgets[0x25].CustomSetActive(false);
                            this._form.m_formWidgets[0x2b].CustomSetActive(false);
                            this._form.m_formWidgets[60].CustomSetActive(false);
                            this._form.m_formWidgets[0x51].CustomSetActive(false);
                            this._form.m_formWidgets[0x53].CustomSetActive(false);
                            this.InitOperation();
                            Singleton<CUINewFlagSystem>.GetInstance().DelNewFlag(this._tabList.GetElemenet(selectedIndex).get_gameObject(), enNewFlagKey.New_OperationTab_V1, true);
                            break;

                        case SettingType.SelectHero:
                            this._form.m_formWidgets[1].CustomSetActive(false);
                            this._form.m_formWidgets[2].CustomSetActive(false);
                            this._form.m_formWidgets[0x1b].CustomSetActive(false);
                            this._form.m_formWidgets[0x25].CustomSetActive(false);
                            this._form.m_formWidgets[0x2b].CustomSetActive(false);
                            this._form.m_formWidgets[60].CustomSetActive(false);
                            this._form.m_formWidgets[0x51].CustomSetActive(true);
                            this._form.m_formWidgets[0x53].CustomSetActive(false);
                            this.InitSelectHero();
                            break;

                        case SettingType.VoiceSetting:
                            this._form.m_formWidgets[1].CustomSetActive(false);
                            this._form.m_formWidgets[2].CustomSetActive(false);
                            this._form.m_formWidgets[0x1b].CustomSetActive(true);
                            this._form.m_formWidgets[0x25].CustomSetActive(false);
                            this._form.m_formWidgets[0x2b].CustomSetActive(false);
                            this._form.m_formWidgets[60].CustomSetActive(false);
                            this._form.m_formWidgets[0x51].CustomSetActive(false);
                            this._form.m_formWidgets[0x53].CustomSetActive(false);
                            break;

                        case SettingType.NetAcc:
                            this._form.m_formWidgets[1].CustomSetActive(false);
                            this._form.m_formWidgets[2].CustomSetActive(false);
                            this._form.m_formWidgets[0x1b].CustomSetActive(false);
                            this._form.m_formWidgets[0x25].CustomSetActive(true);
                            this._form.m_formWidgets[0x2b].CustomSetActive(false);
                            this._form.m_formWidgets[60].CustomSetActive(false);
                            this._form.m_formWidgets[0x51].CustomSetActive(false);
                            this._form.m_formWidgets[0x53].CustomSetActive(false);
                            break;

                        case SettingType.KingTime:
                            this._form.m_formWidgets[1].CustomSetActive(false);
                            this._form.m_formWidgets[2].CustomSetActive(false);
                            this._form.m_formWidgets[0x1b].CustomSetActive(false);
                            this._form.m_formWidgets[0x25].CustomSetActive(false);
                            this._form.m_formWidgets[0x2b].CustomSetActive(true);
                            this._form.m_formWidgets[60].CustomSetActive(false);
                            this._form.m_formWidgets[0x51].CustomSetActive(false);
                            this._form.m_formWidgets[0x53].CustomSetActive(false);
                            this.InitRecorderWidget();
                            Singleton<CUINewFlagSystem>.GetInstance().DelNewFlag(this._tabList.GetElemenet(selectedIndex).get_gameObject(), enNewFlagKey.New_KingTimeTab_V2, true);
                            if (CSysDynamicBlock.bLobbyEntryBlocked)
                            {
                                Transform transform = this._form.m_formWidgets[0x2b].get_transform().FindChild("Button_Course");
                                if (transform != null)
                                {
                                    transform.get_gameObject().CustomSetActive(false);
                                }
                            }
                            break;

                        case SettingType.SecurePwd:
                            this._form.m_formWidgets[1].CustomSetActive(false);
                            this._form.m_formWidgets[2].CustomSetActive(false);
                            this._form.m_formWidgets[0x1b].CustomSetActive(false);
                            this._form.m_formWidgets[0x25].CustomSetActive(false);
                            this._form.m_formWidgets[0x2b].CustomSetActive(false);
                            this._form.m_formWidgets[60].CustomSetActive(true);
                            this._form.m_formWidgets[0x51].CustomSetActive(false);
                            this._form.m_formWidgets[0x53].CustomSetActive(false);
                            this.InitSecurePwdSetting();
                            break;

                        case SettingType.ReplayKit:
                            this._form.m_formWidgets[1].CustomSetActive(false);
                            this._form.m_formWidgets[2].CustomSetActive(false);
                            this._form.m_formWidgets[0x1b].CustomSetActive(false);
                            this._form.m_formWidgets[0x25].CustomSetActive(false);
                            this._form.m_formWidgets[0x2b].CustomSetActive(false);
                            this._form.m_formWidgets[60].CustomSetActive(false);
                            this._form.m_formWidgets[0x51].CustomSetActive(false);
                            this._form.m_formWidgets[0x53].CustomSetActive(true);
                            this.InitReplayKitSetting();
                            break;
                    }
                }
            }
        }

        private void OnShowEnemyHeroChange(CUIEvent uiEvent)
        {
            if (uiEvent.m_eventParams.togleIsOn)
            {
                GameSettings.ShowEnemyHeroHeadBtnMode = true;
                if ((Singleton<CBattleSystem>.instance.FightForm != null) && (Singleton<CBattleSystem>.instance.FightForm.m_enemyHeroAtkBtn != null))
                {
                    Singleton<CBattleSystem>.instance.FightForm.m_enemyHeroAtkBtn.ShowEnemyHeroHeadBtn();
                }
            }
        }

        private static void OnSkillCanleType1Change(CUIEvent uiEvent)
        {
            if (uiEvent.m_eventParams.togleIsOn)
            {
                GameSettings.TheSkillCancleType = SkillCancleType.AreaCancle;
            }
        }

        private static void OnSkillCanleType2Change(CUIEvent uiEvent)
        {
            if (uiEvent.m_eventParams.togleIsOn)
            {
                GameSettings.TheSkillCancleType = SkillCancleType.DisitanceCancle;
            }
        }

        private void OnSkillTipChange(CUIEvent uiEvent)
        {
            int sliderValue = (int) uiEvent.m_eventParams.sliderValue;
            if (((sliderValue != 0) && !GameSettings.EnableOutline) && (PlayerPrefs.GetInt("degrade", 0) == 1))
            {
                stUIEventParams par = new stUIEventParams();
                par.tag = 2;
                par.tag2 = sliderValue;
                Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("Setting_Quality_Confirm"), enUIEventID.Settings_ConfirmQuality_Accept, enUIEventID.Settings_ConfirmQuality_Cancel, par, false);
            }
            else
            {
                GameSettings.EnableOutline = sliderValue != 0;
            }
        }

        private void OnSmartCastChange(CUIEvent uiEvent)
        {
            if (uiEvent.m_eventParams.togleIsOn)
            {
                this.LunPanSettingsStatusChange(false);
                GameSettings.TheCastType = CastType.SmartCast;
            }
        }

        private void OnSoundEffectChange(CUIEvent uiEvent)
        {
            GameSettings.EnableSound = uiEvent.m_eventParams.sliderValue == 1f;
            this.ShowSoundSettingLevel(GameSettings.EnableSound, null, SettingFormWidget.SoundEffectLevel);
        }

        private void OnSurrenderCDReady(CUIEvent uiEvent)
        {
            if (Singleton<BattleLogic>.instance.isRuning && (this._form != null))
            {
                GameObject widget = this._form.GetWidget(0x1a);
                if (widget != null)
                {
                    GameObject p = widget;
                    if (p != null)
                    {
                        Button component = p.GetComponent<Button>();
                        if (component != null)
                        {
                            GameObject obj4 = Utility.FindChild(p, "CountDown");
                            if (obj4 != null)
                            {
                                obj4.CustomSetActive(false);
                                CUICommonSystem.SetButtonEnable(component, true, true, true);
                            }
                        }
                    }
                }
            }
        }

        private void OnUpdateTimer(CUIEvent uiEvent)
        {
            if ((this._form != null) && NetworkAccelerator.SettingUIEnbaled)
            {
                int netType = NetworkAccelerator.GetNetType();
                string pLAYCEHOLDER = this.PLAYCEHOLDER;
                switch (netType)
                {
                    case 1:
                        pLAYCEHOLDER = "WIFI";
                        break;

                    case 2:
                        pLAYCEHOLDER = "2G";
                        break;

                    case 3:
                        pLAYCEHOLDER = "3G";
                        break;

                    case 4:
                        pLAYCEHOLDER = "4G";
                        break;
                }
                this._form.m_formWidgets[0x21].GetComponent<Text>().set_text(pLAYCEHOLDER);
                SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
                if ((curLvelContext == null) || !curLvelContext.IsMobaMode())
                {
                    if (NetworkAccelerator.started)
                    {
                        this._form.m_formWidgets[0x22].GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("NETWORK_ACCELERATOR_LEBEL_NETWORK_DETAIL_ACC_STATE_UNKNOWN"));
                    }
                    else
                    {
                        this._form.m_formWidgets[0x22].GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("NETWORK_ACCELERATOR_LEBEL_NETWORK_DETAIL_ACC_STATE_CLOSE"));
                    }
                    this._form.m_formWidgets[0x1f].GetComponent<Text>().set_text(this.PLAYCEHOLDER);
                }
                else
                {
                    if (Singleton<CBattleSystem>.GetInstance().FightForm != null)
                    {
                        this._form.m_formWidgets[0x1f].GetComponent<Text>().set_text(Singleton<CBattleSystem>.GetInstance().FightForm.GetDisplayPing() + "ms");
                    }
                    if (!NetworkAccelerator.started)
                    {
                        this._form.m_formWidgets[0x22].GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("NETWORK_ACCELERATOR_LEBEL_NETWORK_DETAIL_ACC_STATE_CLOSE"));
                    }
                    else if (Singleton<FrameSynchr>.GetInstance().bActive && NetworkAccelerator.isAccerating())
                    {
                        this._form.m_formWidgets[0x22].GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("NETWORK_ACCELERATOR_LEBEL_NETWORK_DETAIL_ACC_STATE_ACC"));
                    }
                    else
                    {
                        this._form.m_formWidgets[0x22].GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("NETWORK_ACCELERATOR_LEBEL_NETWORK_DETAIL_ACC_STATE_DIRECT"));
                    }
                }
            }
        }

        private void OnVibrateChange(CUIEvent uiEvent)
        {
            GameSettings.EnableVibrate = uiEvent.m_eventParams.sliderValue == 1f;
        }

        private void OnVoiceChange(CUIEvent uiEvent)
        {
            GameSettings.EnableVoice = uiEvent.m_eventParams.sliderValue == 1f;
            if (Singleton<CBattleSystem>.GetInstance().FightForm != null)
            {
                Singleton<CBattleSystem>.GetInstance().FightForm.ChangeSpeakerBtnState();
            }
            this.ShowSoundSettingLevel(GameSettings.EnableVoice, null, SettingFormWidget.VoiceEffectLevel);
        }

        private void Set3DTouchBarShow()
        {
            if (this._form != null)
            {
                this._form.m_formWidgets[0x52].CustomSetActive(GameSettings.Supported3DTouch);
            }
        }

        private void SetAvailableTabs()
        {
            this._availableSettingTypesCnt = 0;
            int num = 8;
            for (int i = 0; i < num; i++)
            {
                SettingType type = (SettingType) i;
                switch (type)
                {
                    case SettingType.Basic:
                    case SettingType.Operation:
                    case SettingType.VoiceSetting:
                        this._availableSettingTypes[this._availableSettingTypesCnt++] = type;
                        break;

                    case SettingType.SelectHero:
                    case SettingType.SecurePwd:
                        if (!Singleton<BattleLogic>.instance.isRuning)
                        {
                            this._availableSettingTypes[this._availableSettingTypesCnt++] = type;
                        }
                        break;

                    case SettingType.NetAcc:
                        if (NetworkAccelerator.SettingUIEnbaled)
                        {
                            this._availableSettingTypes[this._availableSettingTypesCnt++] = type;
                        }
                        break;

                    case SettingType.KingTime:
                        if ((!Singleton<BattleLogic>.instance.isRuning && Singleton<CRecordUseSDK>.instance.GetRecorderGlobalCfgEnableFlag()) && !CSysDynamicBlock.bLobbyEntryBlocked)
                        {
                            this._availableSettingTypes[this._availableSettingTypesCnt++] = type;
                        }
                        break;
                }
            }
        }

        private void SetHDBarShow()
        {
            if (this._form != null)
            {
                GameObject obj2 = this._form.m_formWidgets[0x31];
                bool bActive = GameSettings.SupportHDMode();
                if (Singleton<BattleLogic>.HasInstance())
                {
                    bActive &= !Singleton<BattleLogic>.GetInstance().isRuning;
                }
                obj2.CustomSetActive(bActive);
                obj2.get_transform().get_parent().get_gameObject().CustomSetActive(bActive);
            }
        }

        private void SetUseACC()
        {
            if (NetworkAccelerator.IsNetAccConfigOpen() || NetworkAccelerator.IsAutoNetAccConfigOpen())
            {
                NetworkAccelerator.SetUseACC(true);
            }
            if (!NetworkAccelerator.IsNetAccConfigOpen() && !NetworkAccelerator.IsAutoNetAccConfigOpen())
            {
                NetworkAccelerator.SetUseACC(false);
            }
        }

        private void ShowSoundSettingLevel(bool bEnable, Slider soundObj, SettingFormWidget widgetEnum)
        {
            if (bEnable)
            {
                if ((soundObj == null) && (this._form != null))
                {
                    soundObj = this._form.m_formWidgets[(int) widgetEnum].get_transform().FindChild("Slider").get_gameObject().GetComponent<Slider>();
                }
                if (soundObj != null)
                {
                    soundObj.set_interactable(true);
                    Transform transform = soundObj.get_transform().Find("Background");
                    if (transform != null)
                    {
                        Image component = transform.GetComponent<Image>();
                        if (component != null)
                        {
                            component.set_color(CUIUtility.s_Color_White);
                        }
                    }
                    transform = soundObj.get_transform().Find("Handle Slide Area/Handle");
                    if (transform != null)
                    {
                        Image image2 = transform.GetComponent<Image>();
                        if (image2 != null)
                        {
                            image2.set_color(CUIUtility.s_Color_White);
                        }
                    }
                    transform = soundObj.get_transform().FindChild("Fill Area/Fill");
                    if (transform != null)
                    {
                        Image image3 = transform.GetComponent<Image>();
                        if (image3 != null)
                        {
                            image3.set_color(s_sliderFillColor);
                        }
                    }
                }
            }
            else
            {
                if ((soundObj == null) && (this._form != null))
                {
                    soundObj = this._form.m_formWidgets[(int) widgetEnum].get_transform().FindChild("Slider").get_gameObject().GetComponent<Slider>();
                }
                if (soundObj != null)
                {
                    soundObj.set_interactable(false);
                    Transform transform2 = soundObj.get_transform().Find("Background");
                    if (transform2 != null)
                    {
                        Image image4 = transform2.GetComponent<Image>();
                        if (image4 != null)
                        {
                            image4.set_color(CUIUtility.s_Color_GrayShader);
                        }
                    }
                    transform2 = soundObj.get_transform().Find("Handle Slide Area/Handle");
                    if (transform2 != null)
                    {
                        Image image5 = transform2.GetComponent<Image>();
                        if (image5 != null)
                        {
                            image5.set_color(CUIUtility.s_Color_GrayShader);
                        }
                    }
                    transform2 = soundObj.get_transform().FindChild("Fill Area/Fill");
                    if (transform2 != null)
                    {
                        Image image6 = transform2.GetComponent<Image>();
                        if (image6 != null)
                        {
                            image6.set_color(CUIUtility.s_Color_Grey);
                        }
                    }
                }
            }
        }

        private static void sliderMoveCameraAdjustment()
        {
            CSettingsSys instance = Singleton<CSettingsSys>.GetInstance();
            CUISliderEventScript script = (instance._form == null) ? null : instance._form.m_formWidgets[0x2a].get_transform().FindChild("Slider").GetComponent<CUISliderEventScript>();
            if (Singleton<BattleLogic>.instance.isRuning)
            {
                if ((script != null) && (script.value == 1f))
                {
                    CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
                    uIEvent.m_eventID = enUIEventID.Settings_Slider_onMoveCameraChange;
                    uIEvent.m_eventParams.sliderValue = 2f;
                    Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uIEvent);
                }
            }
            else if (GameSettings.TheCameraMoveType == CameraMoveType.JoyStick)
            {
                GameSettings.TheCameraMoveType = CameraMoveType.Slide;
            }
        }

        private void SliderStatusChange(bool bEnable, Slider slliderObj, SettingFormWidget widgetEnum)
        {
            if ((slliderObj == null) && (this._form != null))
            {
                slliderObj = this._form.m_formWidgets[(int) widgetEnum].get_transform().FindChild("Slider").get_gameObject().GetComponent<Slider>();
            }
            if (slliderObj != null)
            {
                slliderObj.set_interactable(bEnable);
                Color color = !bEnable ? CUIUtility.s_Color_GrayShader : CUIUtility.s_Color_White;
                Transform imageTransform = slliderObj.get_transform().Find("Background");
                this.ChangeImageColor(imageTransform, color);
                imageTransform = slliderObj.get_transform().Find("Handle Slide Area/Handle");
                this.ChangeImageColor(imageTransform, color);
                imageTransform = slliderObj.get_transform().Find("Fill Area/Fill");
                this.ChangeImageColor(imageTransform, !bEnable ? CUIUtility.s_Color_Grey : s_sliderFillColor);
            }
        }

        private void ToggleStatusChange(bool bEnable, SettingFormWidget widgetEnum)
        {
            Toggle component = this._form.m_formWidgets[(int) widgetEnum].GetComponent<Toggle>();
            if (component != null)
            {
                component.set_interactable(bEnable);
                Color color = !bEnable ? new Color(0f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 1f);
                Transform imageTransform = component.get_transform().Find("Background/Bg");
                this.ChangeImageColor(imageTransform, color);
                imageTransform = component.get_transform().Find("Background/Checkmark");
                this.ChangeImageColor(imageTransform, color);
            }
        }

        public override void UnInit()
        {
            base.UnInit();
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_OpenForm, new CUIEventManager.OnUIEventHandler(this.onOpenSetting));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_ReqLogout, new CUIEventManager.OnUIEventHandler(this.onReqLogout));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_ConfirmLogout, new CUIEventManager.OnUIEventHandler(this.onConfirmLogout));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_SettingTypeChange, new CUIEventManager.OnUIEventHandler(this.OnSettingTabChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseSetting));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_OpenNetworkAccelerator, new CUIEventManager.OnUIEventHandler(this.OnNetAccChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_AutomaticOpenNetworkAccelerator, new CUIEventManager.OnUIEventHandler(this.OnAutoNetAccChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_PrivacyPolicy, new CUIEventManager.OnUIEventHandler(this.OnClickPrivacyPolicy));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_TermOfService, new CUIEventManager.OnUIEventHandler(this.OnClickTermOfService));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_Contract, new CUIEventManager.OnUIEventHandler(this.OnClickContract));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_UpdateTimer, new CUIEventManager.OnUIEventHandler(this.OnUpdateTimer));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_SurrenderCDReady, new CUIEventManager.OnUIEventHandler(this.OnSurrenderCDReady));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_ConfirmQuality_Accept, new CUIEventManager.OnUIEventHandler(this.onQualitySettingAccept));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_ConfirmQuality_Cancel, new CUIEventManager.OnUIEventHandler(this.onQualitySettingCancel));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_ClickMoveCameraGuide, new CUIEventManager.OnUIEventHandler(this.onClickMoveCameraGuide));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_ClickSkillCancleTypeHelp, new CUIEventManager.OnUIEventHandler(this.onClickSkillCancleTypeHelp));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_OnClickHeroInfoBarHelp, new CUIEventManager.OnUIEventHandler(this.onClickHeroInfoBarHelp));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_OnClickLastHitModeHelp, new CUIEventManager.OnUIEventHandler(this.onClickLastHitModeHelp));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_OnClickLockEnemyHeroHelp, new CUIEventManager.OnUIEventHandler(this.onClickLockEnemyHeroHelp));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_OnClickShowEnemyHeroHelp, new CUIEventManager.OnUIEventHandler(this.onClickShowEnemyHeroHelp));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_OnClickBPHeroSelectHelpBtn, new CUIEventManager.OnUIEventHandler(this.OnClickBPHeroSelectHelpBtn));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_OnSmartCastChange, new CUIEventManager.OnUIEventHandler(this.OnSmartCastChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_OnLunPanCastChange, new CUIEventManager.OnUIEventHandler(this.OnLunPanCastChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_OnPickNearestChange, new CUIEventManager.OnUIEventHandler(CSettingsSys.OnPickNearestChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_OnPickMinHpChange, new CUIEventManager.OnUIEventHandler(CSettingsSys.OnPickMinHpChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_OnCommonAttackType1Change, new CUIEventManager.OnUIEventHandler(CSettingsSys.OnCommonAttackType1Change));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_OnCommonAttackType2Change, new CUIEventManager.OnUIEventHandler(CSettingsSys.OnCommonAttackType2Change));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_OnSkillCanleType1Change, new CUIEventManager.OnUIEventHandler(CSettingsSys.OnSkillCanleType1Change));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_OnSkillCanleType2Change, new CUIEventManager.OnUIEventHandler(CSettingsSys.OnSkillCanleType2Change));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_OnJoyStickMoveChange, new CUIEventManager.OnUIEventHandler(CSettingsSys.OnJoyStickMoveChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_OnJoyStickNoMoveChange, new CUIEventManager.OnUIEventHandler(CSettingsSys.OnJoyStickNoMoveChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_OnRightJoyStickBtnLocChange, new CUIEventManager.OnUIEventHandler(CSettingsSys.OnRightJoyStickBtnLocChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_OnRightJoyStickFingerLocChange, new CUIEventManager.OnUIEventHandler(CSettingsSys.OnRightJoyStickFingerLocChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_Slider_onLunpanSensitivityChange, new CUIEventManager.OnUIEventHandler(this.OnSensitivityChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_Toggle_OnLockEnemyHeroChange, new CUIEventManager.OnUIEventHandler(this.OnLockEnemyHeroChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_Toggle_OnShowEnemyHeroChange, new CUIEventManager.OnUIEventHandler(this.OnShowEnemyHeroChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_OnNoneLastHitModeChange, new CUIEventManager.OnUIEventHandler(CSettingsSys.OnNoneLastHitModeChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_OnLastHitModeChange, new CUIEventManager.OnUIEventHandler(CSettingsSys.OnLastHitModeChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_Toggle_OnNotLockEnemyHeroChange, new CUIEventManager.OnUIEventHandler(this.OnNotLockEnemyHeroChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_Toggle_OnNotShowEnemyHeroChange, new CUIEventManager.OnUIEventHandler(this.OnNotShowEnemyHeroChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_Slider_On3DTouchBarChange, new CUIEventManager.OnUIEventHandler(this.On3DTouchBarChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_Slider_onModelLODChange, new CUIEventManager.OnUIEventHandler(this.OnModeLODChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_Slider_onParticleLODChange, new CUIEventManager.OnUIEventHandler(this.OnParticleLODChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_Slider_onSkillTipChange, new CUIEventManager.OnUIEventHandler(this.OnSkillTipChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_Slider_onFpsChange, new CUIEventManager.OnUIEventHandler(this.OnFpsChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_Slider_onInputChatChange, new CUIEventManager.OnUIEventHandler(this.OnInputChatChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_Slider_onMoveCameraChange, new CUIEventManager.OnUIEventHandler(this.OnMoveCameraChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_Slider_onHDBarChange, new CUIEventManager.OnUIEventHandler(this.OnHDBarChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_CameraHeight, new CUIEventManager.OnUIEventHandler(this.OnCameraHeightChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_Slider_onCameraSensitivityChange, new CUIEventManager.OnUIEventHandler(this.OnCameraSensitivityChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_Slider_OnHeroInfoBarChange, new CUIEventManager.OnUIEventHandler(this.OnHeroInfoBarChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_PauseMultiGame, new CUIEventManager.OnUIEventHandler(this.OnPauseMultiGame));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_Toggle_OnClickHeroSelectSortTypeDefault, new CUIEventManager.OnUIEventHandler(this.OnClickHeroSelectSortTypeDefault));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_Toggle_OnClickHeroSelectSortTypeProficiency, new CUIEventManager.OnUIEventHandler(this.OnClickHeroSelectSortTypeProficiency));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_Slider_3DTouchChange, new CUIEventManager.OnUIEventHandler(this.On3DTouchChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_Slider_onMusicChange, new CUIEventManager.OnUIEventHandler(this.OnMiusicChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_Slider_onSoundEffectChange, new CUIEventManager.OnUIEventHandler(this.OnSoundEffectChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_Slider_onVoiceChange, new CUIEventManager.OnUIEventHandler(this.OnVoiceChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_Slider_onVibrateChange, new CUIEventManager.OnUIEventHandler(this.OnVibrateChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_Slider_onMusicSensitivityChange, new CUIEventManager.OnUIEventHandler(this.OnSensitivityChangeMusic));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_Slider_onSoundSensitivityChange, new CUIEventManager.OnUIEventHandler(this.OnSensitivityChangeSound));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_Slider_onVoiceSensitivityChange, new CUIEventManager.OnUIEventHandler(this.OnSensitivityChangeVoice));
            this._availableSettingTypes = null;
            this._availableSettingTypesCnt = 0;
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_ReplayKitCourse, new CUIEventManager.OnUIEventHandler(this.onClickReplayKitCourse));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_OnClickReplayKitHelp, new CUIEventManager.OnUIEventHandler(this.onClickReplayKitHelp));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_Slider_onReplayKitEnableChange, new CUIEventManager.OnUIEventHandler(this.OnReplayKitEnableChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_Slider_onReplayKitEnableAutoModeChange, new CUIEventManager.OnUIEventHandler(this.OnReplayKitEnableAutoModeChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_KingTimeCourse, new CUIEventManager.OnUIEventHandler(this.OnClickOBFormOpen));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_Slider_onRecordKingTimeEnableChange, new CUIEventManager.OnUIEventHandler(this.OnRecordKingTimeEnableChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_Slider_onSecurePwdEnableChange, new CUIEventManager.OnUIEventHandler(this.OnSecurePwdEnableChange));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.SECURE_PWD_STATUS_CHANGE, new Action(this, (IntPtr) this.OnSecurePwdStatusChange));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.SECURE_PWD_OP_CANCEL, new Action(this, (IntPtr) this.OnSecurePwdStatusChange));
        }

        private void UnInitWidget()
        {
            if (this._tabList != null)
            {
                Singleton<CUINewFlagSystem>.GetInstance().DelNewFlag(this._tabList.GetElemenet(0).get_gameObject(), enNewFlagKey.New_BasicSettingTab_V2, true);
            }
            this._tabList = null;
            this._form = null;
        }

        private void UpdateCameraSensitivitySlider()
        {
            float curCameraSensitivity = GameSettings.GetCurCameraSensitivity();
            if (curCameraSensitivity < 0f)
            {
                this.SliderStatusChange(false, null, SettingFormWidget.CameraSensitivity);
                this.GetSliderBarScript(this._form.m_formWidgets[0x30]).value = curCameraSensitivity;
            }
            else
            {
                this.SliderStatusChange(true, null, SettingFormWidget.CameraSensitivity);
                this.GetSliderBarScript(this._form.m_formWidgets[0x30]).value = curCameraSensitivity;
            }
        }

        private void ValidatePauseState()
        {
            GameObject obj2 = this._form.m_formWidgets[0x45];
            if (obj2 != null)
            {
                PauseControl pauseControl = Singleton<CBattleSystem>.GetInstance().pauseControl;
                if (pauseControl != null)
                {
                    bool enablePause = pauseControl.EnablePause;
                    obj2.CustomSetActive(enablePause);
                    if (enablePause)
                    {
                        Text componetInChild = Utility.GetComponetInChild<Text>(obj2, "Text");
                        string text = Singleton<CTextManager>.GetInstance().GetText("PauseGame");
                        if (pauseControl.MaxAllowTimes == 0xff)
                        {
                            componetInChild.set_text(text);
                        }
                        else
                        {
                            componetInChild.set_text(string.Concat(new object[] { text, "(", pauseControl.CurPauseTimes, "/", pauseControl.MaxAllowTimes, ")" }));
                        }
                        bool canPause = pauseControl.CanPause;
                        CUICommonSystem.SetButtonEnable(obj2.GetComponent<Button>(), canPause, canPause, true);
                    }
                }
                else
                {
                    obj2.CustomSetActive(false);
                }
            }
        }

        protected enum SettingType
        {
            Basic,
            Operation,
            SelectHero,
            VoiceSetting,
            NetAcc,
            KingTime,
            SecurePwd,
            ReplayKit,
            TypeCount
        }
    }
}

