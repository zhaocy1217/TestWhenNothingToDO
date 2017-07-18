namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.GameLogic.GameKernal;
    using Assets.Scripts.UI;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class CSkillButtonManager
    {
        private SkillButton[] _skillButtons = new SkillButton[s_maxButtonCount];
        private const float c_directionalSkillIndicatorRespondMinRadius = 30f;
        private const string c_skillBtnFormPath = "UGUI/Form/Battle/Part/Form_Battle_Part_SkillBtn.prefab";
        private const float c_skillICancleRadius = 270f;
        private const float c_skillIndicatorMoveRadius = 30f;
        private const float c_skillIndicatorRadius = 110f;
        private const float c_skillIndicatorRespondMinRadius = 15f;
        private const string c_skillJoystickTargetNormalBorderPath = "UGUI/Sprite/Battle/Battle_skillFrameBg_new.prefab";
        private const string c_skillJoystickTargetSelectedBorderPath = "UGUI/Sprite/Battle/Battle_ComboCD.prefab";
        private stCampHeroInfo[] m_campHeroInfos = new stCampHeroInfo[0];
        private bool m_commonAtkSlide;
        private Vector2 m_currentSkillDownScreenPosition = Vector2.get_zero();
        private bool m_currentSkillIndicatorEnabled;
        private bool m_currentSkillIndicatorInCancelArea;
        private bool m_currentSkillIndicatorJoystickEnabled;
        private bool m_currentSkillIndicatorResponed;
        private Vector2 m_currentSkillIndicatorScreenPosition = Vector2.get_zero();
        private enSkillJoystickMode m_currentSkillJoystickMode;
        private int m_currentSkillJoystickSelectedIndex = -1;
        private SkillSlotType m_currentSkillSlotType = SkillSlotType.SLOT_SKILL_COUNT;
        private bool m_currentSkillTipsResponed;
        private uint m_CurTargtNum;
        private enSkillIndicatorMode m_skillIndicatorMode = enSkillIndicatorMode.FixedPosition;
        private const string m_strKillNotifyBlueRingPath = "UGUI/Sprite/Battle/LockEnemy/Battle_KillNotify_Blue_ring";
        private const string m_strKillNotifyRedRingPath = "UGUI/Sprite/Battle/LockEnemy/Battle_KillNotify_Red_ring";
        private stCampHeroInfo[] m_targetInfos = new stCampHeroInfo[0];
        private static int MaxTargetNum = 4;
        private static byte s_maxButtonCount = 10;
        private static enSkillButtonFormWidget[] s_skillBeanTexts = new enSkillButtonFormWidget[] { enSkillButtonFormWidget.None, enSkillButtonFormWidget.Text_Skill_1_Bean, enSkillButtonFormWidget.Text_Skill_2_Bean, enSkillButtonFormWidget.Text_Skill_3_Bean, enSkillButtonFormWidget.Text_Skill_4_Bean, enSkillButtonFormWidget.Text_Skill_5_Bean, enSkillButtonFormWidget.Text_Skill_6_Bean, enSkillButtonFormWidget.Text_Skill_7_Bean, enSkillButtonFormWidget.Text_Skill_9_Bean, enSkillButtonFormWidget.Text_Skill_10_Bean, enSkillButtonFormWidget.None };
        private static enSkillButtonFormWidget[] s_skillButtons = new enSkillButtonFormWidget[] { enSkillButtonFormWidget.Button_Attack, enSkillButtonFormWidget.Button_Skill_1, enSkillButtonFormWidget.Button_Skill_2, enSkillButtonFormWidget.Button_Skill_3, enSkillButtonFormWidget.Button_Recover, enSkillButtonFormWidget.Button_Talent, enSkillButtonFormWidget.Button_Skill_6, enSkillButtonFormWidget.Button_Skill_Ornament, enSkillButtonFormWidget.Button_Skill_9, enSkillButtonFormWidget.Button_Skill_10, enSkillButtonFormWidget.None };
        private static enSkillButtonFormWidget[] s_skillCDTexts = new enSkillButtonFormWidget[] { enSkillButtonFormWidget.None, enSkillButtonFormWidget.Text_Skill_1_CD, enSkillButtonFormWidget.Text_Skill_2_CD, enSkillButtonFormWidget.Text_Skill_3_CD, enSkillButtonFormWidget.Text_Skill_4_CD, enSkillButtonFormWidget.Text_Skill_5_CD, enSkillButtonFormWidget.Text_Skill_6_CD, enSkillButtonFormWidget.Text_Skill_Ornament_CD, enSkillButtonFormWidget.Text_Skill_9_CD, enSkillButtonFormWidget.Text_Skill_10_CD, enSkillButtonFormWidget.None };
        private static Color s_skillCursorBGColorBlue = new Color(0.1686275f, 0.7607843f, 1f, 1f);
        private static Color s_skillCursorBGColorRed = new Color(0.972549f, 0.1843137f, 0.1843137f, 1f);

        public CSkillButtonManager()
        {
            Singleton<GameEventSys>.instance.AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
            Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorRevive, new RefAction<DefaultGameEventParam>(this.onActorRevive));
            Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_CaptainSwitch, new RefAction<DefaultGameEventParam>(this.OnCaptainSwitched));
            Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int, int>("HeroHpChange", new Action<PoolObjHandle<ActorRoot>, int, int>(this, (IntPtr) this.OnHeroHpChange));
        }

        public void CancelLimitButton(SkillSlotType skillSlotType)
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
            SkillButton button = this.GetButton(skillSlotType);
            DebugHelper.Assert(button != null);
            if (button != null)
            {
                button.bLimitedFlag = false;
                if (skillSlotType == SkillSlotType.SLOT_SKILL_0)
                {
                    if (!button.bDisableFlag)
                    {
                        GameObject animationPresent = button.GetAnimationPresent();
                        if (animationPresent != null)
                        {
                            Image component = animationPresent.GetComponent<Image>();
                            if (component != null)
                            {
                                component.set_color(CUIUtility.s_Color_Full);
                            }
                        }
                        GameObject disableButton = button.GetDisableButton();
                        if (disableButton != null)
                        {
                            disableButton.CustomSetActive(false);
                        }
                        this.SetSelectTargetBtnState(false);
                    }
                }
                else
                {
                    if (button.m_button != null)
                    {
                        SkillSlot slot;
                        CUIEventScript script = button.m_button.GetComponent<CUIEventScript>();
                        if (((script != null) && (hostPlayer != null)) && ((hostPlayer.Captain != 0) && hostPlayer.Captain.handle.SkillControl.TryGetSkillSlot(skillSlotType, out slot)))
                        {
                            if (slot.EnableButtonFlag)
                            {
                                script.set_enabled(true);
                            }
                            else
                            {
                                script.set_enabled(false);
                            }
                        }
                    }
                    GameObject obj4 = button.GetAnimationPresent().get_transform().Find("disableFrame").get_gameObject();
                    DebugHelper.Assert(obj4 != null);
                    if (obj4 != null)
                    {
                        obj4.CustomSetActive(false);
                    }
                    if (!button.bDisableFlag)
                    {
                        GameObject obj5 = button.GetDisableButton();
                        if (obj5 != null)
                        {
                            CUIEventScript script2 = obj5.GetComponent<CUIEventScript>();
                            if ((script2 != null) && script2.ClearInputStatus())
                            {
                                Singleton<CBattleSystem>.GetInstance().FightForm.HideSkillDescInfo();
                            }
                            obj5.CustomSetActive(false);
                        }
                        CUICommonSystem.PlayAnimation(button.GetAnimationPresent(), enSkillButtonAnimationName.normal.ToString(), false);
                    }
                }
            }
        }

        public void CancelUseSkillSlot(SkillSlotType skillSlotType, enSkillJoystickMode mode = 0)
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
            if ((hostPlayer != null) && (hostPlayer.Captain != 0))
            {
                hostPlayer.Captain.handle.SkillControl.HostCancelUseSkillSlot(skillSlotType, mode);
            }
            if (((mode == enSkillJoystickMode.MapSelect) || (mode == enSkillJoystickMode.MapSelectOther)) && (Singleton<CBattleSystem>.instance.TheMinimapSys.CurMapType() == MinimapSys.EMapType.Skill))
            {
                Singleton<CBattleSystem>.instance.TheMinimapSys.Switch(MinimapSys.EMapType.Mini);
            }
        }

        public void ChangeSkill(SkillSlotType skillSlotType, ref ChangeSkillEventParam skillParam)
        {
            if ((skillSlotType > SkillSlotType.SLOT_SKILL_0) && (skillParam.skillID > 0))
            {
                SkillButton button = this._skillButtons[(int) skillSlotType];
                if (button != null)
                {
                    button.ChangeSkillIcon(skillParam.skillID);
                }
                this.SetComboEffect(skillSlotType, skillParam.changeTime, skillParam.changeTime);
            }
        }

        private void ChangeSkillCursorBGSprite(CUIFormScript battleFormScript, GameObject skillJoystick, bool isSkillCursorInCancelArea)
        {
            if (skillJoystick != null)
            {
                Image component = skillJoystick.GetComponent<Image>();
                if (component != null)
                {
                    component.set_color(GetCursorBGColor(isSkillCursorInCancelArea));
                }
            }
        }

        private void ChangeSkillJoystickSelectedTarget(CUIFormScript battleFormScript, GameObject skillJoystick, int selectedIndex)
        {
            if (this.m_currentSkillJoystickSelectedIndex != selectedIndex)
            {
                int currentSkillJoystickSelectedIndex = this.m_currentSkillJoystickSelectedIndex;
                this.m_currentSkillJoystickSelectedIndex = selectedIndex;
                this.SetSkillIndicatorSelectedTarget(this.m_currentSkillJoystickSelectedIndex);
                if ((battleFormScript != null) && (skillJoystick != null))
                {
                    CUIComponent component = skillJoystick.GetComponent<CUIComponent>();
                    if ((component != null) && (component.m_widgets != null))
                    {
                        if ((this.m_currentSkillJoystickSelectedIndex >= 0) && (this.m_currentSkillJoystickSelectedIndex < this.m_CurTargtNum))
                        {
                            this.SetSkillJoystickTargetHead(battleFormScript, component.m_widgets[this.m_currentSkillJoystickSelectedIndex], true);
                            Transform transform = skillJoystick.get_transform().FindChild("HighLight");
                            if (transform != null)
                            {
                                transform.get_gameObject().CustomSetActive(true);
                                transform.set_eulerAngles(new Vector3(0f, 0f, (float) (0x2d * this.m_currentSkillJoystickSelectedIndex)));
                            }
                        }
                        else
                        {
                            Transform transform2 = skillJoystick.get_transform().FindChild("HighLight");
                            if (transform2 != null)
                            {
                                transform2.get_gameObject().CustomSetActive(false);
                            }
                        }
                        if ((currentSkillJoystickSelectedIndex >= 0) && (currentSkillJoystickSelectedIndex < this.m_CurTargtNum))
                        {
                            this.SetSkillJoystickTargetHead(battleFormScript, component.m_widgets[currentSkillJoystickSelectedIndex], false);
                        }
                    }
                }
            }
        }

        public void Clear()
        {
            if (this._skillButtons != null)
            {
                for (int i = 0; i < this._skillButtons.Length; i++)
                {
                    if (this._skillButtons[i] != null)
                    {
                        this._skillButtons[i].Clear();
                    }
                }
            }
            Singleton<GameEventSys>.instance.RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorRevive, new RefAction<DefaultGameEventParam>(this.onActorRevive));
            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_CaptainSwitch, new RefAction<DefaultGameEventParam>(this.OnCaptainSwitched));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int, int>("HeroHpChange", new Action<PoolObjHandle<ActorRoot>, int, int>(this, (IntPtr) this.OnHeroHpChange));
        }

        public bool ClearButtonInput(SkillSlotType CurSlotType)
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
            if ((hostPlayer != null) && (hostPlayer.Captain != 0))
            {
                for (int i = 0; i < 10; i++)
                {
                    if (CurSlotType != i)
                    {
                        SkillSlot slot;
                        if (!hostPlayer.Captain.handle.SkillControl.TryGetSkillSlot((SkillSlotType) i, out slot))
                        {
                            return false;
                        }
                        SkillButton button = this.GetButton((SkillSlotType) i);
                        if (button != null)
                        {
                            if (button.m_button != null)
                            {
                                CUIEventScript component = button.m_button.GetComponent<CUIEventScript>();
                                if (component != null)
                                {
                                    component.ClearInputStatus();
                                }
                            }
                            GameObject disableButton = button.GetDisableButton();
                            if (disableButton != null)
                            {
                                CUIEventScript script2 = disableButton.GetComponent<CUIEventScript>();
                                if (script2 != null)
                                {
                                    script2.ClearInputStatus();
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }

        public void CommonAtkSlide(CUIFormScript battleFormScript, Vector2 screenPosition)
        {
            if (GameSettings.TheCommonAttackType == CommonAttactType.Type2)
            {
                SkillButton button = this.GetButton(SkillSlotType.SLOT_SKILL_0);
                CUIFormScript skillBtnFormScript = Singleton<CBattleSystem>.GetInstance().FightForm.GetSkillBtnFormScript();
                if (((button != null) && !button.bDisableFlag) && (skillBtnFormScript != null))
                {
                    GameObject widget = skillBtnFormScript.GetWidget(0x18);
                    GameObject targetObj = skillBtnFormScript.GetWidget(9);
                    if (this.IsSkillCursorInTargetArea(battleFormScript, ref screenPosition, widget))
                    {
                        Singleton<LockModeKeySelector>.GetInstance().OnHandleClickSelectTargetBtn(AttackTargetType.ATTACK_TARGET_HERO);
                        Singleton<CUIParticleSystem>.GetInstance().AddParticle(CUIParticleSystem.s_particleSkillBtnEffect_Path, 0.5f, widget, battleFormScript);
                        Singleton<CSoundManager>.GetInstance().PostEvent("UI_signal_Change_hero", null);
                    }
                    else if (this.IsSkillCursorInTargetArea(battleFormScript, ref screenPosition, targetObj))
                    {
                        Singleton<LockModeKeySelector>.GetInstance().OnHandleClickSelectTargetBtn(AttackTargetType.ATTACK_TARGET_SOLDIER);
                        Singleton<CUIParticleSystem>.GetInstance().AddParticle(CUIParticleSystem.s_particleSkillBtnEffect_Path, 0.5f, targetObj, battleFormScript);
                        Singleton<CSoundManager>.GetInstance().PostEvent("UI_signal_Change_xiaobing", null);
                    }
                }
            }
        }

        public void DisableSkillCursor(CUIFormScript battleFormScript, SkillSlotType skillSlotType)
        {
            this.m_currentSkillIndicatorEnabled = false;
            this.m_currentSkillIndicatorJoystickEnabled = false;
            this.m_currentSkillIndicatorResponed = false;
            this.m_currentSkillTipsResponed = false;
            this.m_currentSkillIndicatorInCancelArea = false;
            DebugHelper.Assert(battleFormScript != null);
            if (battleFormScript != null)
            {
                this.DisableSkillJoystick(battleFormScript, this.m_currentSkillJoystickMode);
                if (GameSettings.TheSkillCancleType == SkillCancleType.AreaCancle)
                {
                    GameObject equipSkillCancleArea = null;
                    if (skillSlotType == SkillSlotType.SLOT_SKILL_9)
                    {
                        equipSkillCancleArea = Singleton<CBattleSystem>.GetInstance().FightForm.GetEquipSkillCancleArea();
                    }
                    else
                    {
                        equipSkillCancleArea = Singleton<CBattleSystem>.GetInstance().FightForm.GetSkillCancleArea();
                    }
                    if (equipSkillCancleArea != null)
                    {
                        equipSkillCancleArea.CustomSetActive(false);
                    }
                }
            }
        }

        private void DisableSkillJoystick(CUIFormScript battleFormScript, enSkillJoystickMode skillJoystickMode)
        {
            if (battleFormScript != null)
            {
                if (skillJoystickMode == enSkillJoystickMode.General)
                {
                    GameObject skillCursor = Singleton<CBattleSystem>.GetInstance().FightForm.GetSkillCursor(enSkillJoystickMode.General);
                    if ((skillCursor != null) && skillCursor.get_activeSelf())
                    {
                        skillCursor.CustomSetActive(false);
                        RectTransform transform = skillCursor.get_transform().FindChild("Cursor") as RectTransform;
                        if (transform != null)
                        {
                            transform.set_anchoredPosition(Vector2.get_zero());
                        }
                    }
                }
                else if (IsSelectedTargetJoyStick(skillJoystickMode))
                {
                    GameObject obj3 = Singleton<CBattleSystem>.GetInstance().FightForm.GetSkillCursor(enSkillJoystickMode.SelectTarget);
                    if ((obj3 != null) && obj3.get_activeSelf())
                    {
                        obj3.CustomSetActive(false);
                        RectTransform transform2 = obj3.get_transform().FindChild("Cursor") as RectTransform;
                        if (transform2 != null)
                        {
                            transform2.set_anchoredPosition(Vector2.get_zero());
                        }
                    }
                }
            }
        }

        public void DragSkillButton(CUIFormScript formScript, SkillSlotType skillSlotType, Vector2 dragScreenPosition)
        {
            if (skillSlotType == SkillSlotType.SLOT_SKILL_0)
            {
                this.m_commonAtkSlide = true;
            }
            if ((this.m_currentSkillSlotType == skillSlotType) && this.m_currentSkillIndicatorEnabled)
            {
                bool currentSkillIndicatorInCancelArea = this.m_currentSkillIndicatorInCancelArea;
                if (formScript != null)
                {
                    this.SetSkillBtnDragFlag(skillSlotType, true);
                    GameObject skillCursor = Singleton<CBattleSystem>.GetInstance().FightForm.GetSkillCursor(this.m_currentSkillJoystickMode);
                    Vector2 cursor = this.MoveSkillCursor(formScript, skillCursor, skillSlotType, this.m_currentSkillJoystickMode, ref dragScreenPosition, out this.m_currentSkillIndicatorInCancelArea);
                    if (currentSkillIndicatorInCancelArea != this.m_currentSkillIndicatorInCancelArea)
                    {
                        this.ChangeSkillCursorBGSprite(formScript, skillCursor, this.m_currentSkillIndicatorInCancelArea);
                    }
                    if (this.m_currentSkillJoystickMode == enSkillJoystickMode.General)
                    {
                        SkillSlot slot;
                        if (((GameSettings.TheCastType == CastType.LunPanCast) && GameSettings.ShowEnemyHeroHeadBtnMode) && (GameSettings.LunPanLockEnemyHeroMode && Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer().Captain.handle.SkillControl.TryGetSkillSlot(skillSlotType, out slot)))
                        {
                            Skill skill = (slot.NextSkillObj == null) ? slot.SkillObj : slot.NextSkillObj;
                            if ((((skill != null) && (skill.cfgData != null)) && ((skill.cfgData.bRangeAppointType == 1) && slot.CanUseSkillWithEnemyHeroSelectMode())) && ((Singleton<CBattleSystem>.instance.FightForm != null) && (Singleton<CBattleSystem>.instance.FightForm.m_enemyHeroAtkBtn != null)))
                            {
                                Singleton<CBattleSystem>.instance.FightForm.m_enemyHeroAtkBtn.OnSkillBtnDrag(formScript, skillSlotType, dragScreenPosition, this.m_currentSkillIndicatorInCancelArea);
                                return;
                            }
                        }
                        this.MoveSkillCursorInScene(skillSlotType, ref cursor, this.m_currentSkillIndicatorInCancelArea);
                        if (skillSlotType == SkillSlotType.SLOT_SKILL_3)
                        {
                            MinimapSkillIndicator_3DUI.UpdateIndicator(ref cursor, true);
                            MinimapSkillIndicator_3DUI.SetIndicatorColor(!this.m_currentSkillIndicatorInCancelArea);
                        }
                    }
                    else if ((this.m_currentSkillJoystickMode == enSkillJoystickMode.SelectTarget) && (this.m_currentSkillJoystickSelectedIndex != -1))
                    {
                        uint objectID = this.m_targetInfos[this.m_currentSkillJoystickSelectedIndex].m_objectID;
                        PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(objectID);
                        if (actor != 0)
                        {
                            MonoSingleton<CameraSystem>.GetInstance().SetFocusActor(actor);
                        }
                    }
                }
            }
        }

        private void EnableLastHitBtn(bool _bEnable)
        {
            CUIFormScript skillBtnFormScript = Singleton<CBattleSystem>.GetInstance().FightForm.GetSkillBtnFormScript();
            if (skillBtnFormScript != null)
            {
                GameObject widget = skillBtnFormScript.GetWidget(0x19);
                if (widget != null)
                {
                    Color color = !_bEnable ? CUIUtility.s_Color_DisableGray : CUIUtility.s_Color_Full;
                    GameObject obj3 = Utility.FindChild(widget, "disable");
                    if (obj3 != null)
                    {
                        obj3.CustomSetActive(!_bEnable);
                    }
                    GameObject obj4 = Utility.FindChild(widget, "Present");
                    if (obj4 != null)
                    {
                        Image component = obj4.GetComponent<Image>();
                        if (component != null)
                        {
                            component.set_color(color);
                        }
                    }
                }
            }
        }

        public void EnableSkillCursor(CUIFormScript battleFormScript, ref Vector2 screenPosition, bool enableSkillCursorJoystick, SkillSlotType skillSlotType, SkillSlot useSlot, bool isSkillCanBeCancled)
        {
            this.m_currentSkillIndicatorEnabled = true;
            this.m_currentSkillIndicatorResponed = false;
            this.m_currentSkillTipsResponed = false;
            if (enableSkillCursorJoystick)
            {
                this.m_currentSkillIndicatorJoystickEnabled = true;
                if (battleFormScript != null)
                {
                    if (this.m_currentSkillJoystickMode == enSkillJoystickMode.General)
                    {
                        this.DisableSkillJoystick(battleFormScript, enSkillJoystickMode.SelectTarget);
                        GameObject skillCursor = Singleton<CBattleSystem>.GetInstance().FightForm.GetSkillCursor(enSkillJoystickMode.General);
                        if (skillCursor != null)
                        {
                            skillCursor.CustomSetActive(true);
                            Vector3 skillIndicatorFixedPosition = this.GetButton(skillSlotType).m_skillIndicatorFixedPosition;
                            if ((this.m_skillIndicatorMode == enSkillIndicatorMode.General) || (skillIndicatorFixedPosition == Vector3.get_zero()))
                            {
                                skillCursor.get_transform().set_position(CUIUtility.ScreenToWorldPoint(battleFormScript.GetCamera(), screenPosition, skillCursor.get_transform().get_position().z));
                                this.m_currentSkillIndicatorScreenPosition = screenPosition;
                            }
                            else
                            {
                                skillCursor.get_transform().set_position(skillIndicatorFixedPosition);
                                this.m_currentSkillIndicatorScreenPosition = CUIUtility.WorldToScreenPoint(battleFormScript.GetCamera(), skillIndicatorFixedPosition);
                            }
                        }
                        this.ChangeSkillCursorBGSprite(battleFormScript, skillCursor, this.m_currentSkillIndicatorInCancelArea);
                    }
                    else if (IsSelectedTargetJoyStick(this.m_currentSkillJoystickMode))
                    {
                        this.DisableSkillJoystick(battleFormScript, enSkillJoystickMode.General);
                        if (this.m_currentSkillJoystickMode == enSkillJoystickMode.SelectTarget)
                        {
                            this.ResetSkillTargetJoyStickHeadToCampHero();
                        }
                        else if (this.m_currentSkillJoystickMode == enSkillJoystickMode.SelectNextSkillTarget)
                        {
                            this.ResetSkillTargetJoyStickHeadToTargets(useSlot.NextSkillTargetIDs);
                        }
                        GameObject obj3 = Singleton<CBattleSystem>.GetInstance().FightForm.GetSkillCursor(enSkillJoystickMode.SelectTarget);
                        if (obj3 != null)
                        {
                            obj3.CustomSetActive(true);
                            obj3.get_transform().set_position(this.GetButton(skillSlotType).m_button.get_transform().get_position());
                            this.m_currentSkillIndicatorScreenPosition = CUIUtility.WorldToScreenPoint(battleFormScript.GetCamera(), obj3.get_transform().get_position());
                            CUIAnimationScript component = obj3.GetComponent<CUIAnimationScript>();
                            if (component != null)
                            {
                                component.PlayAnimation("Head_In2", true);
                            }
                            this.ResetSkillJoystickSelectedTarget(battleFormScript);
                            this.ChangeSkillCursorBGSprite(battleFormScript, obj3, this.m_currentSkillIndicatorInCancelArea);
                        }
                    }
                    else if (this.m_currentSkillJoystickMode == enSkillJoystickMode.MapSelect)
                    {
                        if (Singleton<TeleportTargetSelector>.GetInstance().m_ClickDownStatus)
                        {
                            MonoSingleton<CameraSystem>.instance.ToggleFreeDragCamera(false);
                        }
                        this.DisableSkillJoystick(battleFormScript, enSkillJoystickMode.General);
                        this.DisableSkillJoystick(battleFormScript, enSkillJoystickMode.SelectTarget);
                        Singleton<CBattleSystem>.instance.TheMinimapSys.Switch(MinimapSys.EMapType.Skill, skillSlotType);
                    }
                    else if (this.m_currentSkillJoystickMode == enSkillJoystickMode.MapSelectOther)
                    {
                        this.DisableSkillJoystick(battleFormScript, enSkillJoystickMode.General);
                        this.DisableSkillJoystick(battleFormScript, enSkillJoystickMode.SelectTarget);
                        if (Singleton<TeleportTargetSelector>.GetInstance().m_ClickDownStatus)
                        {
                            Singleton<TeleportTargetSelector>.GetInstance().m_bConfirmed = true;
                        }
                        else
                        {
                            Singleton<CBattleSystem>.instance.TheMinimapSys.Switch(MinimapSys.EMapType.Skill, skillSlotType);
                        }
                    }
                }
            }
            if ((battleFormScript != null) && (GameSettings.TheSkillCancleType == SkillCancleType.AreaCancle))
            {
                GameObject skillCancleArea = null;
                if (skillSlotType != SkillSlotType.SLOT_SKILL_9)
                {
                    skillCancleArea = Singleton<CBattleSystem>.GetInstance().FightForm.GetSkillCancleArea();
                }
                else
                {
                    skillCancleArea = Singleton<CBattleSystem>.GetInstance().FightForm.GetEquipSkillCancleArea();
                }
                if (skillCancleArea != null)
                {
                    skillCancleArea.CustomSetActive(isSkillCanBeCancled);
                }
            }
        }

        ~CSkillButtonManager()
        {
        }

        public SkillButton GetButton(SkillSlotType skillSlotType)
        {
            int index = (int) skillSlotType;
            if ((index < 0) || (index >= this._skillButtons.Length))
            {
                return null;
            }
            SkillButton button = this._skillButtons[index];
            if (button == null)
            {
                FightForm fightForm = Singleton<CBattleSystem>.GetInstance().FightForm;
                if (fightForm != null)
                {
                    CUIFormScript skillBtnFormScript = fightForm.GetSkillBtnFormScript();
                    button = new SkillButton();
                    button.m_button = (skillBtnFormScript != null) ? skillBtnFormScript.GetWidget((int) s_skillButtons[index]) : null;
                    button.m_cdText = (skillBtnFormScript != null) ? skillBtnFormScript.GetWidget((int) s_skillCDTexts[index]) : null;
                    button.m_beanText = (skillBtnFormScript != null) ? skillBtnFormScript.GetWidget((int) s_skillBeanTexts[index]) : null;
                    this._skillButtons[index] = button;
                }
                if (button.m_button != null)
                {
                    Transform transform = button.m_button.get_transform().FindChild("IndicatorPosition");
                    if (transform != null)
                    {
                        button.m_skillIndicatorFixedPosition = transform.get_position();
                    }
                }
            }
            return button;
        }

        private int GetCampHeroInfosIndexByObjId(uint uiObjId)
        {
            for (int i = 0; i < this.m_campHeroInfos.Length; i++)
            {
                if (this.m_campHeroInfos[i].m_objectID == uiObjId)
                {
                    return i;
                }
            }
            return -1;
        }

        public SkillSlotType GetCurSkillSlotType()
        {
            return this.m_currentSkillSlotType;
        }

        public static Color GetCursorBGColor(bool cancel)
        {
            return (!cancel ? s_skillCursorBGColorBlue : s_skillCursorBGColorRed);
        }

        private static void GetJoystickHeadAreaFan(ref stFan headAreaFan, GameObject head, GameObject preHead, GameObject backHead)
        {
            if ((preHead == null) && (backHead == null))
            {
                headAreaFan.m_minRadian = 0f;
                headAreaFan.m_maxRadian = 0f;
            }
            float radian = GetRadian((head.get_transform() as RectTransform).get_anchoredPosition());
            if (preHead != null)
            {
                headAreaFan.m_minRadian = GetRadian((Vector2) (((head.get_transform() as RectTransform).get_anchoredPosition() + (preHead.get_transform() as RectTransform).get_anchoredPosition()) / 2f));
                if (backHead == null)
                {
                    headAreaFan.m_maxRadian = radian + (radian - headAreaFan.m_minRadian);
                    return;
                }
            }
            if (backHead != null)
            {
                headAreaFan.m_maxRadian = GetRadian((Vector2) (((head.get_transform() as RectTransform).get_anchoredPosition() + (backHead.get_transform() as RectTransform).get_anchoredPosition()) / 2f));
                if (preHead == null)
                {
                    headAreaFan.m_minRadian = radian - (headAreaFan.m_maxRadian - radian);
                }
            }
        }

        private static void GetJoystickHeadAreaInScreen(ref Rect targetArea, CUIFormScript formScript, RectTransform joystickRectTransform, RectTransform targetRectTransform)
        {
            if ((joystickRectTransform == null) || (targetRectTransform == null))
            {
                targetArea = new Rect(0f, 0f, 0f, 0f);
            }
            else
            {
                Vector2 vector = new Vector2(formScript.ChangeFormValueToScreen(targetRectTransform.get_anchoredPosition().x), formScript.ChangeFormValueToScreen(targetRectTransform.get_anchoredPosition().y));
                float num = formScript.ChangeFormValueToScreen(targetRectTransform.get_rect().get_width());
                float num2 = formScript.ChangeFormValueToScreen(targetRectTransform.get_rect().get_height());
                float num3 = vector.x - (num / 2f);
                float num4 = vector.y - (num2 / 2f);
                targetArea = new Rect(num3, num4, num, num2);
            }
        }

        private static float GetRadian(Vector2 point)
        {
            float num = Mathf.Atan2(point.y, point.x);
            if (num < 0f)
            {
                num += 6.283185f;
            }
            return num;
        }

        public enSkillJoystickMode GetSkillJoystickMode(SkillSlotType skillSlotType)
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
            if ((hostPlayer != null) && (hostPlayer.Captain != 0))
            {
                SkillSlot slot = null;
                if (hostPlayer.Captain.handle.SkillControl.TryGetSkillSlot(skillSlotType, out slot) && (slot != null))
                {
                    Skill skill = (slot.NextSkillObj == null) ? slot.SkillObj : slot.NextSkillObj;
                    if ((skill != null) && (skill.cfgData != null))
                    {
                        return (enSkillJoystickMode) skill.cfgData.bWheelType;
                    }
                }
            }
            return enSkillJoystickMode.General;
        }

        public bool HasMapSlectTargetSkill(out SkillSlotType slotType)
        {
            for (int i = 0; i < s_maxButtonCount; i++)
            {
                SkillSlotType skillSlotType = (SkillSlotType) i;
                switch (this.GetSkillJoystickMode(skillSlotType))
                {
                    case enSkillJoystickMode.MapSelect:
                    case enSkillJoystickMode.MapSelectOther:
                        slotType = skillSlotType;
                        return true;
                }
            }
            slotType = SkillSlotType.SLOT_SKILL_COUNT;
            return false;
        }

        public void Initialise(PoolObjHandle<ActorRoot> actor, bool bInitSpecifiedButton = false, SkillSlotType specifiedType = 10)
        {
            if (((actor != 0) && (actor.handle.SkillControl != null)) && (actor.handle.SkillControl.SkillSlotArray != null))
            {
                SkillSlot[] skillSlotArray = actor.handle.SkillControl.SkillSlotArray;
                for (int i = 0; i < s_maxButtonCount; i++)
                {
                    SkillSlotType skillSlotType = (SkillSlotType) i;
                    if (bInitSpecifiedButton && (skillSlotType != specifiedType))
                    {
                        goto Label_08AC;
                    }
                    SkillButton button = this.GetButton(skillSlotType);
                    SkillSlot slot = skillSlotArray[i];
                    if ((!MinimapSkillIndicator_3DUI.IsIndicatorInited() && (skillSlotType == SkillSlotType.SLOT_SKILL_3)) && !Singleton<WatchController>.GetInstance().IsWatching)
                    {
                        MinimapSkillIndicator_3DUI.InitIndicator(slot.SkillObj.cfgData.szMapIndicatorNormalPrefab, slot.SkillObj.cfgData.szMapIndicatorRedPrefab, (float) slot.SkillObj.cfgData.iSmallMapIndicatorHeight, (float) slot.SkillObj.cfgData.iBigMapIndicatorHeight);
                    }
                    DebugHelper.Assert(button != null);
                    if (button == null)
                    {
                        goto Label_08AC;
                    }
                    button.bDisableFlag = (slot != null) && !slot.EnableButtonFlag;
                    if (actor.handle.SkillControl.IsIngnoreDisableSkill((SkillSlotType) i) && (actor.handle.SkillControl.ForceDisableSkill[i] == 0))
                    {
                        button.bLimitedFlag = false;
                    }
                    else
                    {
                        button.bLimitedFlag = actor.handle.SkillControl.DisableSkill[i] >= 1;
                    }
                    SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
                    if (skillSlotType == SkillSlotType.SLOT_SKILL_6)
                    {
                        if (curLvelContext.IsGameTypeGuide())
                        {
                            if ((CBattleGuideManager.Is5v5GuideLevel(curLvelContext.m_mapID) && (slot != null)) && ((slot.SkillObj != null) && (slot.SkillObj.cfgData != null)))
                            {
                                goto Label_021F;
                            }
                            button.m_button.CustomSetActive(false);
                            goto Label_08AC;
                        }
                        if ((!curLvelContext.IsMobaModeWithOutGuide() || (curLvelContext.m_pvpPlayerNum != 10)) || (((slot == null) || (slot.SkillObj == null)) || (slot.SkillObj.cfgData == null)))
                        {
                            button.m_button.CustomSetActive(false);
                            goto Label_08AC;
                        }
                    }
                Label_021F:
                    if ((skillSlotType == SkillSlotType.SLOT_SKILL_7) && !curLvelContext.m_bEnableOrnamentSlot)
                    {
                        button.m_button.CustomSetActive(false);
                        button.m_cdText.CustomSetActive(false);
                    }
                    else
                    {
                        if ((Singleton<BattleLogic>.instance.GetCurLvelContext().IsGameTypeBurning() || Singleton<BattleLogic>.instance.GetCurLvelContext().IsGameTypeArena()) || Singleton<BattleLogic>.instance.GetCurLvelContext().IsGameTypeAdventure())
                        {
                            if ((Singleton<BattleLogic>.instance.GetCurLvelContext().IsGameTypeBurning() || Singleton<BattleLogic>.instance.GetCurLvelContext().IsGameTypeArena()) && ((skillSlotType == SkillSlotType.SLOT_SKILL_4) || (skillSlotType == SkillSlotType.SLOT_SKILL_5)))
                            {
                                if ((button.m_button != null) && (button.m_cdText != null))
                                {
                                    button.m_button.CustomSetActive(false);
                                    button.m_cdText.CustomSetActive(false);
                                }
                                goto Label_08AC;
                            }
                            if ((skillSlotType >= SkillSlotType.SLOT_SKILL_1) && (skillSlotType <= SkillSlotType.SLOT_SKILL_3))
                            {
                                if (button.m_button != null)
                                {
                                    GameObject skillLvlFrameImg = button.GetSkillLvlFrameImg(true);
                                    if (skillLvlFrameImg != null)
                                    {
                                        skillLvlFrameImg.CustomSetActive(false);
                                    }
                                    GameObject obj3 = button.GetSkillLvlFrameImg(false);
                                    if (obj3 != null)
                                    {
                                        obj3.CustomSetActive(false);
                                    }
                                    GameObject skillFrameImg = button.GetSkillFrameImg();
                                    if (skillFrameImg != null)
                                    {
                                        skillFrameImg.CustomSetActive(true);
                                    }
                                }
                                if (slot != null)
                                {
                                    int dwConfValue = 0;
                                    switch (skillSlotType)
                                    {
                                        case SkillSlotType.SLOT_SKILL_1:
                                        case SkillSlotType.SLOT_SKILL_2:
                                            dwConfValue = (int) GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x8e).dwConfValue;
                                            break;

                                        default:
                                            if (skillSlotType == SkillSlotType.SLOT_SKILL_3)
                                            {
                                                dwConfValue = (int) GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x8f).dwConfValue;
                                            }
                                            break;
                                    }
                                    slot.SetSkillLevel(dwConfValue);
                                }
                            }
                        }
                        if (button.m_button != null)
                        {
                            CUIEventScript component = button.m_button.GetComponent<CUIEventScript>();
                            CUIEventScript script2 = button.GetDisableButton().GetComponent<CUIEventScript>();
                            if (slot != null)
                            {
                                component.set_enabled(true);
                                script2.set_enabled(true);
                                switch (skillSlotType)
                                {
                                    case SkillSlotType.SLOT_SKILL_1:
                                    case SkillSlotType.SLOT_SKILL_2:
                                    case SkillSlotType.SLOT_SKILL_3:
                                        if (slot.EnableButtonFlag)
                                        {
                                            component.set_enabled(true);
                                        }
                                        else
                                        {
                                            component.set_enabled(false);
                                        }
                                        break;
                                }
                                if (button.GetDisableButton() != null)
                                {
                                    if (slot.GetSkillLevel() > 0)
                                    {
                                        this.SetEnableButton(skillSlotType);
                                        if (((actor.handle.ActorControl != null) && actor.handle.ActorControl.IsDeadState) || ((slot.SlotType != SkillSlotType.SLOT_SKILL_0) && !slot.IsCDReady))
                                        {
                                            this.SetDisableButton(skillSlotType);
                                        }
                                    }
                                    else
                                    {
                                        this.SetDisableButton(skillSlotType);
                                    }
                                }
                                if (button.m_button != null)
                                {
                                    button.m_button.CustomSetActive(true);
                                }
                                if (button.m_cdText != null)
                                {
                                    button.m_cdText.CustomSetActive(true);
                                }
                                if (skillSlotType != SkillSlotType.SLOT_SKILL_0)
                                {
                                    Image image = button.m_button.get_transform().Find("Present/SkillImg").GetComponent<Image>();
                                    image.get_gameObject().CustomSetActive(true);
                                    image.SetSprite(CUIUtility.s_Sprite_Dynamic_Skill_Dir + slot.SkillObj.IconName, Singleton<CBattleSystem>.GetInstance().FightFormScript, true, false, false, false);
                                    if (((skillSlotType == SkillSlotType.SLOT_SKILL_4) || (skillSlotType == SkillSlotType.SLOT_SKILL_5)) || ((skillSlotType == SkillSlotType.SLOT_SKILL_6) || (skillSlotType == SkillSlotType.SLOT_SKILL_7)))
                                    {
                                        Transform transform = button.m_button.get_transform().Find("lblName");
                                        if (transform != null)
                                        {
                                            if (slot.SkillObj.cfgData != null)
                                            {
                                                transform.GetComponent<Text>().set_text(slot.SkillObj.cfgData.szSkillName);
                                            }
                                            transform.get_gameObject().CustomSetActive(true);
                                        }
                                    }
                                    if (actor.handle.SkillControl.IsDisableSkillSlot(skillSlotType))
                                    {
                                        this.SetLimitButton(skillSlotType);
                                    }
                                    else if ((slot.GetSkillLevel() > 0) && slot.IsEnergyEnough)
                                    {
                                        this.CancelLimitButton(skillSlotType);
                                    }
                                    if (slot.GetSkillLevel() > 0)
                                    {
                                        this.UpdateButtonCD(skillSlotType, (int) slot.CurSkillCD);
                                    }
                                    else if (button.m_cdText != null)
                                    {
                                        button.m_cdText.CustomSetActive(false);
                                    }
                                }
                                component.m_onDownEventParams.m_skillSlotType = skillSlotType;
                                component.m_onUpEventParams.m_skillSlotType = skillSlotType;
                                component.m_onHoldEventParams.m_skillSlotType = skillSlotType;
                                component.m_onHoldEndEventParams.m_skillSlotType = skillSlotType;
                                component.m_onDragStartEventParams.m_skillSlotType = skillSlotType;
                                component.m_onDragEventParams.m_skillSlotType = skillSlotType;
                                script2.m_onClickEventParams.m_skillSlotType = skillSlotType;
                                if (slot.skillChangeEvent.IsDisplayUI())
                                {
                                    this.SetComboEffect(skillSlotType, slot.skillChangeEvent.GetEffectTIme(), slot.skillChangeEvent.GetEffectMaxTime());
                                }
                                else if (!curLvelContext.IsMobaMode())
                                {
                                    this.SetComboEffect(skillSlotType, slot.skillChangeEvent.GetEffectTIme(), slot.skillChangeEvent.GetEffectMaxTime());
                                }
                            }
                            else
                            {
                                component.set_enabled(false);
                                script2.set_enabled(false);
                                if (button.GetDisableButton() != null)
                                {
                                    this.SetDisableButton(skillSlotType);
                                }
                                if (button.m_cdText != null)
                                {
                                    button.m_cdText.CustomSetActive(false);
                                }
                                if (skillSlotType == SkillSlotType.SLOT_SKILL_7)
                                {
                                    Transform transform2 = button.m_button.get_transform().Find("Present/SkillImg");
                                    if (transform2 != null)
                                    {
                                        transform2.get_gameObject().CustomSetActive(false);
                                    }
                                    Transform transform3 = button.m_button.get_transform().Find("lblName");
                                    if (transform3 != null)
                                    {
                                        transform3.get_gameObject().CustomSetActive(false);
                                    }
                                }
                            }
                            if (button.m_beanText != null)
                            {
                                if ((((skillSlotType >= SkillSlotType.SLOT_SKILL_1) && (skillSlotType <= SkillSlotType.SLOT_SKILL_3)) && ((slot != null) && slot.IsUnLock())) && slot.bConsumeBean)
                                {
                                    button.m_beanText.GetComponent<Text>().set_text(slot.skillBeanAmount.ToString());
                                    button.m_beanText.CustomSetActive(true);
                                }
                                else
                                {
                                    button.m_beanText.CustomSetActive(false);
                                }
                                if (button.m_button != null)
                                {
                                    button.m_beanText.get_transform().set_position(button.m_button.get_transform().get_position());
                                }
                            }
                            if (((skillSlotType == SkillSlotType.SLOT_SKILL_9) || (skillSlotType == SkillSlotType.SLOT_SKILL_10)) && !bInitSpecifiedButton)
                            {
                                button.m_button.CustomSetActive(false);
                                button.m_cdText.CustomSetActive(false);
                            }
                        }
                    }
                Label_08AC:;
                }
            }
        }

        public void InitializeCampHeroInfo(CUIFormScript battleFormScript)
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
            if (hostPlayer != null)
            {
                SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
                if (curLvelContext != null)
                {
                    int index = 0;
                    if (curLvelContext.IsMobaMode())
                    {
                        List<Player> allCampPlayers = Singleton<GamePlayerCenter>.GetInstance().GetAllCampPlayers(hostPlayer.PlayerCamp);
                        if (allCampPlayers != null)
                        {
                            this.m_campHeroInfos = new stCampHeroInfo[allCampPlayers.Count - 1];
                            for (int i = 0; i < allCampPlayers.Count; i++)
                            {
                                if (allCampPlayers[i] != hostPlayer)
                                {
                                    this.m_campHeroInfos[index].m_headIconPath = CUIUtility.s_Sprite_Dynamic_BustCircle_Dir + CSkinInfo.GetHeroSkinPic((uint) allCampPlayers[i].Captain.handle.TheActorMeta.ConfigId, 0);
                                    this.m_campHeroInfos[index].m_objectID = allCampPlayers[i].Captain.handle.ObjID;
                                    index++;
                                }
                            }
                        }
                    }
                    else
                    {
                        ReadonlyContext<PoolObjHandle<ActorRoot>> allHeroes = hostPlayer.GetAllHeroes();
                        int count = allHeroes.Count;
                        if (count > 0)
                        {
                            this.m_campHeroInfos = new stCampHeroInfo[count - 1];
                            for (int j = 0; j < count; j++)
                            {
                                if (allHeroes[j] != hostPlayer.Captain)
                                {
                                    PoolObjHandle<ActorRoot> handle = allHeroes[j];
                                    this.m_campHeroInfos[index].m_headIconPath = CUIUtility.s_Sprite_Dynamic_BustCircle_Dir + CSkinInfo.GetHeroSkinPic((uint) handle.handle.TheActorMeta.ConfigId, 0);
                                    PoolObjHandle<ActorRoot> handle2 = allHeroes[j];
                                    this.m_campHeroInfos[index].m_objectID = handle2.handle.ObjID;
                                    index++;
                                }
                            }
                        }
                    }
                    this.m_currentSkillJoystickSelectedIndex = -1;
                    if (battleFormScript != null)
                    {
                        GameObject skillCursor = Singleton<CBattleSystem>.GetInstance().FightForm.GetSkillCursor(enSkillJoystickMode.SelectTarget);
                        if ((Singleton<CBattleSystem>.GetInstance().FightForm.GetSkillCursorFormScript() != null) && (skillCursor != null))
                        {
                            CUIComponent component = skillCursor.GetComponent<CUIComponent>();
                            if ((component != null) && (component.m_widgets != null))
                            {
                                MaxTargetNum = component.m_widgets.Length;
                                this.m_targetInfos = new stCampHeroInfo[MaxTargetNum];
                                for (int k = 0; k < component.m_widgets.Length; k++)
                                {
                                    GameObject obj3 = component.m_widgets[k];
                                    if (obj3 != null)
                                    {
                                        obj3.CustomSetActive(true);
                                        obj3.get_transform().set_localScale(new Vector3(1f, 1f, 1f));
                                        GetJoystickHeadAreaInScreen(ref this.m_targetInfos[k].m_headAreaInScreen, battleFormScript, skillCursor.get_transform() as RectTransform, obj3.get_transform() as RectTransform);
                                        GetJoystickHeadAreaFan(ref this.m_targetInfos[k].m_headAreaFan, obj3, (k != 0) ? component.m_widgets[k - 1] : null, (k != (component.m_widgets.Length - 1)) ? component.m_widgets[k + 1] : null);
                                    }
                                }
                            }
                            skillCursor.CustomSetActive(false);
                        }
                    }
                }
            }
        }

        public bool IsIndicatorInCancelArea()
        {
            return this.m_currentSkillIndicatorInCancelArea;
        }

        private static bool IsSelectedTargetJoyStick(enSkillJoystickMode mode)
        {
            return ((mode == enSkillJoystickMode.SelectTarget) || (mode == enSkillJoystickMode.SelectNextSkillTarget));
        }

        private bool IsSkillCursorInCanceledArea(CUIFormScript battleFormScript, ref Vector2 screenPosition, SkillSlotType skillSlotType)
        {
            if (GameSettings.TheSkillCancleType == SkillCancleType.AreaCancle)
            {
                GameObject targetObj = null;
                if (skillSlotType != SkillSlotType.SLOT_SKILL_9)
                {
                    targetObj = Singleton<CBattleSystem>.GetInstance().FightForm.GetSkillCancleArea();
                }
                else
                {
                    targetObj = Singleton<CBattleSystem>.GetInstance().FightForm.GetEquipSkillCancleArea();
                }
                return this.IsSkillCursorInTargetArea(battleFormScript, ref screenPosition, targetObj);
            }
            Vector2 vector = screenPosition - this.m_currentSkillDownScreenPosition;
            return (battleFormScript.ChangeScreenValueToForm(vector.get_magnitude()) > 270f);
        }

        public bool IsSkillCursorInTargetArea(CUIFormScript battleFormScript, ref Vector2 screenPosition, GameObject targetObj)
        {
            DebugHelper.Assert(battleFormScript != null, "battleFormScript!=null");
            if (battleFormScript != null)
            {
                DebugHelper.Assert((targetObj != null) && (targetObj.get_transform() is RectTransform), "targetObj != null && targetObj.transform is RectTransform");
                if (((targetObj != null) && targetObj.get_activeInHierarchy()) && (targetObj.get_transform() is RectTransform))
                {
                    Vector2 vector = CUIUtility.WorldToScreenPoint(battleFormScript.GetCamera(), targetObj.get_transform().get_position());
                    float num = battleFormScript.ChangeFormValueToScreen((targetObj.get_transform() as RectTransform).get_sizeDelta().x);
                    float num2 = battleFormScript.ChangeFormValueToScreen((targetObj.get_transform() as RectTransform).get_sizeDelta().y);
                    Rect rect = new Rect(vector.x - (num / 2f), vector.y - (num2 / 2f), num, num2);
                    return rect.Contains(screenPosition);
                }
            }
            return false;
        }

        public bool IsUseSkillCursorJoystick(SkillSlotType skillSlotType)
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
            return (((hostPlayer != null) && (hostPlayer.Captain != 0)) && hostPlayer.Captain.handle.SkillControl.IsUseSkillJoystick(skillSlotType));
        }

        public void LastHitButtonDown(CUIFormScript formScript)
        {
            this.SendUseCommonAttack(2, 0);
        }

        public void LastHitButtonUp(CUIFormScript formScript)
        {
            this.SendUseCommonAttack(0, 0);
        }

        public Vector2 MoveSkillCursor(CUIFormScript battleFormScript, GameObject skillJoystick, SkillSlotType skillSlotType, enSkillJoystickMode skillJoystickMode, ref Vector2 screenPosition, out bool skillCanceled)
        {
            skillCanceled = this.IsSkillCursorInCanceledArea(battleFormScript, ref screenPosition, skillSlotType);
            if (!this.m_currentSkillIndicatorJoystickEnabled)
            {
                return Vector2.get_zero();
            }
            if (!this.m_currentSkillIndicatorResponed)
            {
                Vector2 vector = screenPosition - this.m_currentSkillDownScreenPosition;
                if (battleFormScript.ChangeScreenValueToForm(vector.get_magnitude()) > 15f)
                {
                    this.m_currentSkillIndicatorResponed = true;
                }
            }
            if (!this.m_currentSkillTipsResponed)
            {
                Vector2 vector2 = screenPosition - this.m_currentSkillDownScreenPosition;
                if (battleFormScript.ChangeScreenValueToForm(vector2.get_magnitude()) > 30f)
                {
                    this.m_currentSkillTipsResponed = true;
                }
            }
            if (!this.m_currentSkillIndicatorResponed)
            {
                return Vector2.get_zero();
            }
            Vector2 vector3 = screenPosition - this.m_currentSkillIndicatorScreenPosition;
            Vector2 vector4 = vector3;
            float num = vector3.get_magnitude();
            float num2 = num;
            num2 = battleFormScript.ChangeScreenValueToForm(num);
            vector4.x = battleFormScript.ChangeScreenValueToForm(vector3.x);
            vector4.y = battleFormScript.ChangeScreenValueToForm(vector3.y);
            if (num2 > 110f)
            {
                vector4 = (Vector2) (vector4.get_normalized() * 110f);
            }
            if (skillJoystick != null)
            {
                Transform transform = skillJoystick.get_transform().FindChild("Cursor");
                if (transform != null)
                {
                    (transform as RectTransform).set_anchoredPosition(vector4);
                }
            }
            if (skillJoystickMode == enSkillJoystickMode.General)
            {
                Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
                if (((hostPlayer != null) && (hostPlayer.Captain != 0)) && ((hostPlayer.Captain.handle.SkillControl.SkillSlotArray[(int) skillSlotType].SkillObj.cfgData.bRangeAppointType == 3) && (num2 < 30f)))
                {
                    return Vector2.get_zero();
                }
            }
            else if (IsSelectedTargetJoyStick(skillJoystickMode))
            {
                int selectedIndex = this.SkillJoystickSelectTarget(battleFormScript, skillJoystick, ref screenPosition);
                this.ChangeSkillJoystickSelectedTarget(battleFormScript, skillJoystick, selectedIndex);
            }
            return (Vector2) (vector4 / 110f);
        }

        public void MoveSkillCursorInScene(SkillSlotType skillSlotType, ref Vector2 cursor, bool isSkillCursorInCancelArea)
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
            if ((hostPlayer != null) && (hostPlayer.Captain != 0))
            {
                hostPlayer.Captain.handle.SkillControl.SelectSkillTarget(skillSlotType, cursor, isSkillCursorInCancelArea);
            }
        }

        private void onActorDead(ref GameDeadEventParam prm)
        {
            PoolObjHandle<ActorRoot> captain = Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain;
            if ((captain == prm.src) && (!captain.handle.TheStaticData.TheBaseAttribute.DeadControl || captain.handle.ActorControl.IsEnableReviveContext()))
            {
                for (int i = 0; i < 10; i++)
                {
                    this.SetDisableButton((SkillSlotType) i);
                }
            }
        }

        private void onActorRevive(ref DefaultGameEventParam prm)
        {
            PoolObjHandle<ActorRoot> captain = Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain;
            if ((captain == prm.src) && (!captain.handle.TheStaticData.TheBaseAttribute.DeadControl || captain.handle.ActorControl.IsEnableReviveContext()))
            {
                for (int i = 0; i < 10; i++)
                {
                    this.SetEnableButton((SkillSlotType) i);
                }
            }
        }

        public void OnBattleSkillDisableAlert(SkillSlotType skillSlotType)
        {
            SkillSlot slot;
            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
            if (((hostPlayer != null) && (hostPlayer.Captain != 0)) && (hostPlayer.Captain.handle.SkillControl.TryGetSkillSlot(skillSlotType, out slot) && slot.IsUnLock()))
            {
                if (!slot.IsCDReady)
                {
                    slot.SendSkillCooldownEvent();
                }
                else if (!slot.IsEnergyEnough)
                {
                    slot.SendSkillShortageEvent();
                }
                else if (!slot.IsSkillBeanEnough)
                {
                    slot.SendSkillBeanShortageEvent();
                }
            }
        }

        private void OnCaptainSwitched(ref DefaultGameEventParam prm)
        {
            Singleton<CBattleSystem>.GetInstance().FightForm.ResetSkillButtonManager(prm.src, false, SkillSlotType.SLOT_SKILL_COUNT);
            this.ResetPickHeroInfo(prm.src);
        }

        private void OnHeroHpChange(PoolObjHandle<ActorRoot> hero, int iCurVal, int iMaxVal)
        {
            if ((this.m_currentSkillIndicatorJoystickEnabled && IsSelectedTargetJoyStick(this.m_currentSkillJoystickMode)) && ((hero != 0) && ActorHelper.IsCaptainActor(ref hero)))
            {
                float fHpRate = ((float) iCurVal) / ((float) iMaxVal);
                this.SetJoystickHeroHpFill(hero, fHpRate);
            }
        }

        public static void Preload(ref ActorPreloadTab result)
        {
        }

        public void ReadyUseSkillSlot(SkillSlotType skillSlotType)
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
            if ((hostPlayer != null) && (hostPlayer.Captain != 0))
            {
                hostPlayer.Captain.handle.SkillControl.ReadyUseSkillSlot(skillSlotType);
            }
        }

        public void RecoverSkill(SkillSlotType skillSlotType, ref DefaultSkillEventParam skillParam)
        {
            if ((skillSlotType > SkillSlotType.SLOT_SKILL_0) && (skillParam.param > 0))
            {
                SkillButton button = this._skillButtons[(int) skillSlotType];
                if (button != null)
                {
                    button.ChangeSkillIcon(skillParam.param);
                }
                this.SetComboEffect(skillSlotType, 0, 0);
            }
        }

        public void RequestUseSkillSlot(SkillSlotType skillSlotType, enSkillJoystickMode mode = 0, uint objID = 0)
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
            if ((hostPlayer != null) && (hostPlayer.Captain != 0))
            {
                hostPlayer.Captain.handle.SkillControl.RequestUseSkillSlot(skillSlotType, mode, objID);
            }
        }

        private void ResetPickHeroInfo(PoolObjHandle<ActorRoot> actor)
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            if ((hostPlayer != null) && (curLvelContext != null))
            {
                if (!curLvelContext.IsMobaMode())
                {
                    ReadonlyContext<PoolObjHandle<ActorRoot>> allHeroes = hostPlayer.GetAllHeroes();
                    int count = allHeroes.Count;
                    int index = 0;
                    if (count > 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            if (allHeroes[i] != actor)
                            {
                                PoolObjHandle<ActorRoot> handle = allHeroes[i];
                                this.m_campHeroInfos[index].m_headIconPath = CUIUtility.s_Sprite_Dynamic_BustCircle_Dir + CSkinInfo.GetHeroSkinPic((uint) handle.handle.TheActorMeta.ConfigId, 0);
                                PoolObjHandle<ActorRoot> handle2 = allHeroes[i];
                                this.m_campHeroInfos[index].m_objectID = handle2.handle.ObjID;
                                index++;
                            }
                        }
                    }
                }
                this.m_currentSkillJoystickSelectedIndex = -1;
                this.ResetSkillTargetJoyStickHeadToCampHero();
            }
        }

        private void ResetSkillJoystickSelectedTarget(CUIFormScript battleFormScript)
        {
            this.m_currentSkillJoystickSelectedIndex = -1;
            this.SetSkillIndicatorSelectedTarget(this.m_currentSkillJoystickSelectedIndex);
            if (battleFormScript != null)
            {
                GameObject skillCursor = Singleton<CBattleSystem>.GetInstance().FightForm.GetSkillCursor(enSkillJoystickMode.SelectTarget);
                if (skillCursor != null)
                {
                    CUIComponent component = skillCursor.GetComponent<CUIComponent>();
                    if ((component != null) && (component.m_widgets != null))
                    {
                        for (int i = 0; i < this.m_CurTargtNum; i++)
                        {
                            this.SetSkillJoystickTargetHead(battleFormScript, component.m_widgets[i], false);
                            PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(this.m_targetInfos[i].m_objectID);
                            if ((actor != 0) && (actor.handle.ValueComponent != null))
                            {
                                float fHpRate = ((float) actor.handle.ValueComponent.actorHp) / ((float) actor.handle.ValueComponent.actorHpTotal);
                                this.SetJoystickHeroHpFill(component.m_widgets[i], fHpRate);
                            }
                        }
                    }
                    Transform transform = skillCursor.get_transform().FindChild("HighLight");
                    if (transform != null)
                    {
                        transform.get_gameObject().CustomSetActive(false);
                    }
                }
            }
        }

        public void ResetSkillTargetJoyStickHeadToCampHero()
        {
            GameObject skillCursor = Singleton<CBattleSystem>.GetInstance().FightForm.GetSkillCursor(enSkillJoystickMode.SelectTarget);
            CUIFormScript skillCursorFormScript = Singleton<CBattleSystem>.GetInstance().FightForm.GetSkillCursorFormScript();
            if ((skillCursorFormScript != null) && (skillCursor != null))
            {
                CUIComponent component = skillCursor.GetComponent<CUIComponent>();
                DebugHelper.Assert(MaxTargetNum >= this.m_campHeroInfos.Length, "目标数大于轮盘支持的最大英雄数!");
                this.m_CurTargtNum = (uint) Mathf.Min(MaxTargetNum, this.m_campHeroInfos.Length);
                if ((component != null) && (component.m_widgets != null))
                {
                    for (int i = 0; i < component.m_widgets.Length; i++)
                    {
                        GameObject obj3 = component.m_widgets[i];
                        if (obj3 != null)
                        {
                            if (i >= this.m_campHeroInfos.Length)
                            {
                                this.m_targetInfos[i].m_objectID = 0;
                                obj3.CustomSetActive(false);
                            }
                            else
                            {
                                this.m_targetInfos[i].m_objectID = this.m_campHeroInfos[i].m_objectID;
                                obj3.CustomSetActive(true);
                                obj3.get_transform().set_localScale(new Vector3(1f, 1f, 1f));
                                CUIComponent component2 = obj3.GetComponent<CUIComponent>();
                                if (((component2 != null) && (component2.m_widgets != null)) && (component2.m_widgets.Length >= 2))
                                {
                                    GameObject obj4 = component2.m_widgets[0];
                                    if (obj4 != null)
                                    {
                                        Image image = obj4.GetComponent<Image>();
                                        if (image != null)
                                        {
                                            image.SetSprite(this.m_campHeroInfos[i].m_headIconPath, skillCursorFormScript, true, false, false, false);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void ResetSkillTargetJoyStickHeadToTargets(ListValueView<uint> targetIDs)
        {
            GameObject skillCursor = Singleton<CBattleSystem>.GetInstance().FightForm.GetSkillCursor(enSkillJoystickMode.SelectTarget);
            CUIFormScript skillCursorFormScript = Singleton<CBattleSystem>.GetInstance().FightForm.GetSkillCursorFormScript();
            if ((skillCursorFormScript != null) && (skillCursor != null))
            {
                CUIComponent component = skillCursor.GetComponent<CUIComponent>();
                DebugHelper.Assert(MaxTargetNum >= targetIDs.Count, "目标数大于轮盘支持的最大英雄数!");
                this.m_CurTargtNum = (uint) Mathf.Min(MaxTargetNum, targetIDs.Count);
                if ((component != null) && (component.m_widgets != null))
                {
                    for (int i = 0; i < component.m_widgets.Length; i++)
                    {
                        GameObject obj3 = component.m_widgets[i];
                        if (obj3 != null)
                        {
                            if (i >= targetIDs.Count)
                            {
                                this.m_targetInfos[i].m_objectID = 0;
                                obj3.CustomSetActive(false);
                            }
                            else
                            {
                                this.m_targetInfos[i].m_objectID = targetIDs[i];
                                obj3.CustomSetActive(true);
                                obj3.get_transform().set_localScale(new Vector3(1f, 1f, 1f));
                                CUIComponent component2 = obj3.GetComponent<CUIComponent>();
                                if (((component2 != null) && (component2.m_widgets != null)) && (component2.m_widgets.Length >= 2))
                                {
                                    PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(targetIDs[i]);
                                    string prefabPath = CUIUtility.s_Sprite_Dynamic_BustCircle_Dir + CSkinInfo.GetHeroSkinPic((uint) actor.handle.TheActorMeta.ConfigId, 0);
                                    GameObject obj4 = component2.m_widgets[0];
                                    if (obj4 != null)
                                    {
                                        Image image = obj4.GetComponent<Image>();
                                        if (image != null)
                                        {
                                            image.SetSprite(prefabPath, skillCursorFormScript, true, false, false, false);
                                        }
                                    }
                                    GameObject obj5 = component2.m_widgets[1];
                                    if (obj5 != null)
                                    {
                                        Image image2 = obj5.GetComponent<Image>();
                                        if (image2 != null)
                                        {
                                            image2.SetSprite("UGUI/Sprite/Battle/Battle_skillFrameBg_new.prefab", skillCursorFormScript, true, false, false, false);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void SelectedMapTarget(uint targetObjId)
        {
            SkillSlotType type;
            if (this.HasMapSlectTargetSkill(out type))
            {
                this.RequestUseSkillSlot(type, enSkillJoystickMode.MapSelect, targetObjId);
            }
        }

        public void SelectedMapTarget(SkillSlotType skillSlotType, uint targetObjId)
        {
            this.RequestUseSkillSlot(skillSlotType, enSkillJoystickMode.MapSelect, targetObjId);
        }

        public void SendUseCommonAttack(sbyte Start, uint uiRealObjID = 0)
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
            if (((hostPlayer != null) && (hostPlayer.Captain != 0)) && !hostPlayer.Captain.handle.ActorControl.IsDeadState)
            {
                FrameCommand<UseCommonAttackCommand> command = FrameCommandFactory.CreateCSSyncFrameCommand<UseCommonAttackCommand>();
                command.cmdData.Start = Start;
                command.cmdData.uiRealObjID = uiRealObjID;
                command.Send();
            }
        }

        public void SetButtonCDOver(SkillSlotType skillSlotType, bool isPlayMusic = true)
        {
            if ((skillSlotType != SkillSlotType.SLOT_SKILL_0) && this.SetEnableButton(skillSlotType))
            {
                SkillButton button = this.GetButton(skillSlotType);
                GameObject target = (button == null) ? null : button.GetAnimationCD();
                CUICommonSystem.PlayAnimation(target, enSkillButtonAnimationName.CD_End.ToString(), false);
                if (isPlayMusic)
                {
                    Singleton<CSoundManager>.GetInstance().PlayBattleSound2D("UI_prompt_jineng");
                }
            }
        }

        public void SetButtonCDStart(SkillSlotType skillSlotType)
        {
            if (skillSlotType != SkillSlotType.SLOT_SKILL_0)
            {
                this.SetDisableButton(skillSlotType);
                SkillButton button = this.GetButton(skillSlotType);
                GameObject target = (button == null) ? null : button.GetAnimationCD();
                CUICommonSystem.PlayAnimation(target, enSkillButtonAnimationName.CD_Star.ToString(), false);
            }
        }

        public void SetButtonFlowLight(GameObject button, bool highLight)
        {
            Transform transform = button.get_transform().FindChild("Present/highlighter");
            if (transform != null)
            {
                transform.get_gameObject().CustomSetActive(highLight);
            }
        }

        public void SetButtonHighLight(SkillSlotType skillSlotType, bool highLight)
        {
            SkillButton button = this.GetButton(skillSlotType);
            if ((button != null) && (button.m_button != null))
            {
                this.SetButtonHighLight(button.m_button, highLight);
            }
        }

        public void SetButtonHighLight(GameObject button, bool highLight)
        {
            Transform transform = button.get_transform().FindChild("Present/highlighter");
            if (transform != null)
            {
                transform.get_gameObject().CustomSetActive(highLight);
            }
        }

        private void SetComboEffect(SkillSlotType skillSlotType, int leftTime, int totalTime)
        {
            SkillButton button = this.GetButton(skillSlotType);
            if ((button != null) && (null != button.m_button))
            {
                button.effectTimeTotal = totalTime;
                button.effectTimeLeft = leftTime;
                GameObject obj2 = Utility.FindChildSafe(button.m_button, "Present/comboCD");
                if (obj2 != null)
                {
                    if ((button.effectTimeLeft > 0) && (button.effectTimeTotal > 0))
                    {
                        obj2.CustomSetActive(true);
                        button.effectTimeImage = obj2.GetComponent<Image>();
                    }
                    else
                    {
                        obj2.CustomSetActive(false);
                        button.effectTimeImage = null;
                    }
                }
            }
        }

        public void SetCommonAtkBtnState(CommonAttactType byAtkType)
        {
            CUIFormScript script = (Singleton<CBattleSystem>.GetInstance().FightForm != null) ? Singleton<CBattleSystem>.GetInstance().FightForm.GetSkillBtnFormScript() : null;
            if (script != null)
            {
                GameObject widget = script.GetWidget(0x18);
                GameObject obj3 = script.GetWidget(9);
                if (((widget != null) && (obj3 != null)) && (obj3.GetComponent<CUIEventScript>() != null))
                {
                    if (byAtkType == CommonAttactType.Type1)
                    {
                        widget.CustomSetActive(false);
                        obj3.CustomSetActive(false);
                    }
                    else if (byAtkType == CommonAttactType.Type2)
                    {
                        widget.CustomSetActive(true);
                        obj3.CustomSetActive(true);
                        bool bActive = false;
                        SkillButton button = this.GetButton(SkillSlotType.SLOT_SKILL_0);
                        if (button != null)
                        {
                            GameObject disableButton = button.GetDisableButton();
                            if (disableButton != null)
                            {
                                bActive = disableButton.get_activeSelf();
                            }
                        }
                        this.SetSelectTargetBtnState(bActive);
                    }
                    Singleton<EventRouter>.GetInstance().BroadCastEvent("CommonAttack_Type_Changed");
                }
            }
        }

        public void SetDisableButton(SkillSlotType skillSlotType)
        {
            CUIFormScript fightFormScript = Singleton<CBattleSystem>.GetInstance().FightFormScript;
            if (fightFormScript != null)
            {
                if (this.m_currentSkillSlotType == skillSlotType)
                {
                    this.SkillButtonUp(fightFormScript, skillSlotType, false, new Vector2());
                }
                SkillButton button = this.GetButton(skillSlotType);
                if (button != null)
                {
                    if (button.m_button != null)
                    {
                        CUIEventScript component = button.m_button.GetComponent<CUIEventScript>();
                        if (component != null)
                        {
                            if (component.ClearInputStatus())
                            {
                                Singleton<CBattleSystem>.GetInstance().FightForm.HideSkillDescInfo();
                            }
                            component.set_enabled(false);
                        }
                    }
                    button.bDisableFlag = true;
                    if (skillSlotType != SkillSlotType.SLOT_SKILL_0)
                    {
                        CUICommonSystem.PlayAnimation(button.GetAnimationPresent(), enSkillButtonAnimationName.disable.ToString(), false);
                    }
                    else
                    {
                        GameObject animationPresent = button.GetAnimationPresent();
                        if (animationPresent != null)
                        {
                            Image image = animationPresent.GetComponent<Image>();
                            if (image != null)
                            {
                                image.set_color(CUIUtility.s_Color_DisableGray);
                            }
                        }
                        GameObject obj4 = button.GetDisableButton();
                        if (obj4 != null)
                        {
                            obj4.CustomSetActive(true);
                        }
                        this.SetSelectTargetBtnState(true);
                        this.EnableLastHitBtn(false);
                    }
                    GameObject disableButton = button.GetDisableButton();
                    if (disableButton != null)
                    {
                        disableButton.CustomSetActive(true);
                    }
                }
            }
        }

        public bool SetEnableButton(SkillSlotType skillSlotType)
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
            if ((hostPlayer != null) && (hostPlayer.Captain != 0))
            {
                SkillSlot slot;
                if (!hostPlayer.Captain.handle.SkillControl.TryGetSkillSlot(skillSlotType, out slot))
                {
                    return false;
                }
                if (skillSlotType == SkillSlotType.SLOT_SKILL_0)
                {
                    if (hostPlayer.Captain.handle.ActorControl.IsDeadState)
                    {
                        return false;
                    }
                }
                else if (!slot.EnableButtonFlag)
                {
                    return false;
                }
            }
            SkillButton button = this.GetButton(skillSlotType);
            if (button != null)
            {
                button.bDisableFlag = false;
                if (button.bLimitedFlag)
                {
                    return false;
                }
                if (button.m_button != null)
                {
                    CUIEventScript component = button.m_button.GetComponent<CUIEventScript>();
                    if (component != null)
                    {
                        if (component.ClearInputStatus())
                        {
                            Singleton<CBattleSystem>.GetInstance().FightForm.HideSkillDescInfo();
                        }
                        component.set_enabled(true);
                    }
                }
                if (skillSlotType != SkillSlotType.SLOT_SKILL_0)
                {
                    CUICommonSystem.PlayAnimation(button.GetAnimationPresent(), enSkillButtonAnimationName.normal.ToString(), false);
                }
                else if (skillSlotType == SkillSlotType.SLOT_SKILL_0)
                {
                    GameObject animationPresent = button.GetAnimationPresent();
                    if (animationPresent != null)
                    {
                        Image image = animationPresent.GetComponent<Image>();
                        if (image != null)
                        {
                            image.set_color(CUIUtility.s_Color_Full);
                        }
                    }
                    this.SetSelectTargetBtnState(false);
                    this.EnableLastHitBtn(true);
                }
                GameObject disableButton = button.GetDisableButton();
                if (disableButton != null)
                {
                    CUIEventScript script2 = disableButton.GetComponent<CUIEventScript>();
                    if (script2 != null)
                    {
                        if (script2.ClearInputStatus())
                        {
                            Singleton<CBattleSystem>.GetInstance().FightForm.HideSkillDescInfo();
                        }
                        script2.set_enabled(true);
                    }
                    disableButton.CustomSetActive(false);
                }
            }
            return true;
        }

        public void SetEnergyDisableButton(SkillSlotType skillSlotType)
        {
            if (Singleton<CBattleSystem>.GetInstance().FightFormScript != null)
            {
                SkillButton button = this.GetButton(skillSlotType);
                if (button != null)
                {
                    if (button.m_button != null)
                    {
                        CUIEventScript component = button.m_button.GetComponent<CUIEventScript>();
                        if (component != null)
                        {
                            component.set_enabled(false);
                        }
                    }
                    button.bDisableFlag = true;
                    CUICommonSystem.PlayAnimation(button.GetAnimationPresent(), enSkillButtonAnimationName.disable.ToString(), false);
                    GameObject disableButton = button.GetDisableButton();
                    if (disableButton != null)
                    {
                        disableButton.CustomSetActive(true);
                    }
                }
            }
        }

        private void SetJoystickHeroHpFill(PoolObjHandle<ActorRoot> hero, float fHpRate)
        {
            if (hero != 0)
            {
                GameObject skillCursor = Singleton<CBattleSystem>.GetInstance().FightForm.GetSkillCursor(enSkillJoystickMode.SelectTarget);
                if (skillCursor != null)
                {
                    CUIComponent component = skillCursor.GetComponent<CUIComponent>();
                    if (((component != null) && (component.m_widgets != null)) && (component.m_widgets.Length >= this.m_targetInfos.Length))
                    {
                        for (int i = 0; i < component.m_widgets.Length; i++)
                        {
                            if (i >= this.m_targetInfos.Length)
                            {
                                break;
                            }
                            if (this.m_targetInfos[i].m_objectID == hero.handle.ObjID)
                            {
                                this.SetJoystickHeroHpFill(component.m_widgets[i], fHpRate);
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void SetJoystickHeroHpFill(GameObject head, float fHpRate)
        {
            if (head != null)
            {
                CUIComponent component = head.GetComponent<CUIComponent>();
                if (((component != null) && (component.m_widgets != null)) && (component.m_widgets.Length >= 2))
                {
                    GameObject obj2 = component.m_widgets[0];
                    GameObject obj3 = component.m_widgets[1];
                    if ((obj3 != null) && (obj2 != null))
                    {
                        Image image = obj2.GetComponent<Image>();
                        Image image2 = obj3.GetComponent<Image>();
                        if (image2 != null)
                        {
                            float num = image2.get_fillAmount();
                            if ((fHpRate < 0.3) && (num >= 0.3))
                            {
                                image2.SetSprite("UGUI/Sprite/Battle/LockEnemy/Battle_KillNotify_Red_ring", Singleton<CBattleSystem>.GetInstance().FormScript, true, false, false, false);
                                if (image != null)
                                {
                                    image.set_color(CUIUtility.s_Color_EnemyHero_Button_PINK);
                                }
                            }
                            else if ((fHpRate >= 0.3) && (num < 0.3))
                            {
                                image2.SetSprite("UGUI/Sprite/Battle/LockEnemy/Battle_KillNotify_Blue_ring", Singleton<CBattleSystem>.GetInstance().FormScript, true, false, false, false);
                                if (image != null)
                                {
                                    image.set_color(CUIUtility.s_Color_White);
                                }
                            }
                            else if (((fHpRate <= 0f) && (num > 0f)) && (image != null))
                            {
                                image.set_color(CUIUtility.s_Color_DisableGray);
                            }
                            image2.CustomFillAmount(fHpRate);
                        }
                    }
                }
            }
        }

        public void SetLastHitBtnState(LastHitMode mode)
        {
            CUIFormScript script = (Singleton<CBattleSystem>.GetInstance().FightForm != null) ? Singleton<CBattleSystem>.GetInstance().FightForm.GetSkillBtnFormScript() : null;
            if (script != null)
            {
                GameObject widget = script.GetWidget(0x19);
                if (widget != null)
                {
                    widget.CustomSetActive(mode == LastHitMode.LastHit);
                }
            }
        }

        public void SetlearnBtnHighLight(GameObject learnBtn, bool highLight)
        {
            Transform transform = learnBtn.get_transform().FindChild("highlighter");
            if (transform != null)
            {
                transform.get_gameObject().CustomSetActive(highLight);
            }
        }

        public void SetLimitButton(SkillSlotType skillSlotType)
        {
            CUIFormScript fightFormScript = Singleton<CBattleSystem>.GetInstance().FightFormScript;
            if (fightFormScript != null)
            {
                if (this.m_currentSkillSlotType == skillSlotType)
                {
                    this.SkillButtonUp(fightFormScript, skillSlotType, false, new Vector2());
                }
                SkillButton button = this.GetButton(skillSlotType);
                DebugHelper.Assert(button != null);
                if (button != null)
                {
                    button.bLimitedFlag = true;
                    if (skillSlotType != SkillSlotType.SLOT_SKILL_0)
                    {
                        if (button.m_button != null)
                        {
                            CUIEventScript component = button.m_button.GetComponent<CUIEventScript>();
                            if (component != null)
                            {
                                if (component.ClearInputStatus())
                                {
                                    Singleton<CBattleSystem>.GetInstance().FightForm.HideSkillDescInfo();
                                }
                                component.set_enabled(false);
                            }
                        }
                        GameObject obj2 = button.GetAnimationPresent().get_transform().Find("disableFrame").get_gameObject();
                        DebugHelper.Assert(obj2 != null);
                        if (obj2 != null)
                        {
                            obj2.CustomSetActive(true);
                        }
                        GameObject disableButton = button.GetDisableButton();
                        if (disableButton != null)
                        {
                            disableButton.CustomSetActive(true);
                        }
                        CUICommonSystem.PlayAnimation(button.GetAnimationPresent(), enSkillButtonAnimationName.disable.ToString(), false);
                    }
                }
            }
        }

        private void SetSelectTargetBtnState(bool bActive)
        {
            if (GameSettings.TheCommonAttackType == CommonAttactType.Type2)
            {
                CUIFormScript script = (Singleton<CBattleSystem>.GetInstance().FightForm != null) ? Singleton<CBattleSystem>.GetInstance().FightForm.GetSkillBtnFormScript() : null;
                if (script != null)
                {
                    GameObject widget = script.GetWidget(0x18);
                    GameObject obj3 = script.GetWidget(9);
                    if ((widget != null) && (obj3 != null))
                    {
                        Color color = CUIUtility.s_Color_Full;
                        if (bActive)
                        {
                            color = CUIUtility.s_Color_DisableGray;
                        }
                        GameObject obj4 = obj3.get_transform().FindChild("disable").get_gameObject();
                        if (obj4 != null)
                        {
                            obj4.CustomSetActive(bActive);
                        }
                        GameObject obj5 = obj3.get_transform().FindChild("Present").get_gameObject();
                        if (obj5 != null)
                        {
                            Image component = obj5.GetComponent<Image>();
                            if (component != null)
                            {
                                component.set_color(color);
                            }
                        }
                        obj4 = widget.get_transform().FindChild("disable").get_gameObject();
                        if (obj4 != null)
                        {
                            obj4.CustomSetActive(bActive);
                        }
                        obj5 = widget.get_transform().FindChild("Present").get_gameObject();
                        if (obj5 != null)
                        {
                            Image image2 = obj5.GetComponent<Image>();
                            if (image2 != null)
                            {
                                image2.set_color(color);
                            }
                        }
                    }
                }
            }
        }

        private void SetSkillBtnDragFlag(SkillSlotType slotType, bool bDrag)
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
            if ((hostPlayer != null) && (hostPlayer.Captain != 0))
            {
                SkillSlot skillSlot = hostPlayer.Captain.handle.SkillControl.GetSkillSlot(slotType);
                if ((skillSlot != null) && (skillSlot.skillIndicator != null))
                {
                    skillSlot.skillIndicator.SetSkillBtnDrag(bDrag);
                }
            }
        }

        public void SetSkillButtuonActive(SkillSlotType skillSlotType, bool active)
        {
            SkillButton button = this._skillButtons[(int) skillSlotType];
            if (button != null)
            {
                button.m_button.CustomSetActive(active);
                button.m_cdText.GetComponent<Text>().set_enabled(active);
            }
        }

        public void SetSkillIndicatorMode(enSkillIndicatorMode indicaMode)
        {
            this.m_skillIndicatorMode = indicaMode;
        }

        private void SetSkillIndicatorSelectedTarget(int index)
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
            if ((hostPlayer != null) && (hostPlayer.Captain != 0))
            {
                SkillSlot skillSlot = hostPlayer.Captain.handle.SkillControl.GetSkillSlot(this.m_currentSkillSlotType);
                if ((skillSlot != null) && (skillSlot.skillIndicator != null))
                {
                    if (((index < 0) || (index >= this.m_targetInfos.Length)) || (this.m_targetInfos[index].m_objectID == 0))
                    {
                        skillSlot.skillIndicator.SetUseSkillTarget(null);
                    }
                    else
                    {
                        PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(this.m_targetInfos[this.m_currentSkillJoystickSelectedIndex].m_objectID);
                        if (actor != 0)
                        {
                            skillSlot.skillIndicator.SetUseSkillTarget(actor.handle);
                        }
                        else
                        {
                            skillSlot.skillIndicator.SetUseSkillTarget(null);
                        }
                    }
                }
            }
        }

        private void SetSkillJoystickTargetHead(CUIFormScript battleFormScript, GameObject head, bool selected)
        {
            if (head != null)
            {
                head.get_transform().set_localScale(new Vector3(!selected ? 1f : 1.3f, !selected ? 1f : 1.3f, !selected ? 1f : 1.3f));
            }
        }

        public void SkillButtonDown(CUIFormScript formScript, SkillSlotType skillSlotType, Vector2 downScreenPosition)
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
            if ((hostPlayer != null) && (hostPlayer.Captain != 0))
            {
                int skillLevel = 0;
                if (((hostPlayer.Captain.handle.SkillControl != null) && (skillSlotType >= SkillSlotType.SLOT_SKILL_0)) && ((skillSlotType < SkillSlotType.SLOT_SKILL_COUNT) && (hostPlayer.Captain.handle.SkillControl.SkillSlotArray[(int) skillSlotType] != null)))
                {
                    skillLevel = hostPlayer.Captain.handle.SkillControl.SkillSlotArray[(int) skillSlotType].GetSkillLevel();
                }
                if (skillLevel <= 0)
                {
                    return;
                }
            }
            if (this.m_currentSkillSlotType != SkillSlotType.SLOT_SKILL_COUNT)
            {
                this.SkillButtonUp(formScript, this.m_currentSkillSlotType, false, new Vector2());
            }
            this.SetSkillBtnDragFlag(skillSlotType, false);
            Singleton<CBattleSystem>.instance.TheMinimapSys.ClearMapSkillStatus();
            this.m_currentSkillSlotType = skillSlotType;
            this.m_currentSkillDownScreenPosition = downScreenPosition;
            this.m_currentSkillIndicatorEnabled = false;
            this.m_currentSkillIndicatorJoystickEnabled = false;
            this.m_currentSkillIndicatorInCancelArea = false;
            this.m_currentSkillJoystickMode = this.GetSkillJoystickMode(skillSlotType);
            this.m_commonAtkSlide = false;
            GameObject animationPresent = this.GetButton(skillSlotType).GetAnimationPresent();
            if (hostPlayer != null)
            {
                if (skillSlotType == SkillSlotType.SLOT_SKILL_0)
                {
                    this.SendUseCommonAttack(1, 0);
                    Singleton<CUIParticleSystem>.GetInstance().AddParticle(CUIParticleSystem.s_particleSkillBtnEffect_Path, 0.5f, animationPresent, formScript);
                }
                else
                {
                    if (skillSlotType != SkillSlotType.SLOT_SKILL_0)
                    {
                        CUICommonSystem.PlayAnimation(animationPresent, enSkillButtonAnimationName.pressDown.ToString(), false);
                    }
                    this.ReadyUseSkillSlot(skillSlotType);
                    this.EnableSkillCursor(formScript, ref downScreenPosition, this.IsUseSkillCursorJoystick(skillSlotType), skillSlotType, hostPlayer.Captain.handle.SkillControl.SkillSlotArray[(int) skillSlotType], skillSlotType != SkillSlotType.SLOT_SKILL_0);
                    if (((skillSlotType == SkillSlotType.SLOT_SKILL_3) && (hostPlayer != null)) && ((hostPlayer.Captain != 0) && (hostPlayer.Captain.handle.SkillControl != null)))
                    {
                        GameObject prefabEffectObj = hostPlayer.Captain.handle.SkillControl.GetPrefabEffectObj(skillSlotType);
                        if (prefabEffectObj != null)
                        {
                            MinimapSkillIndicator_3DUI.SetIndicator(ref prefabEffectObj.get_transform().get_forward(), true);
                            MinimapSkillIndicator_3DUI.SetIndicatorColor(!this.m_currentSkillIndicatorInCancelArea);
                        }
                    }
                }
            }
        }

        public void SkillButtonUp(CUIFormScript formScript)
        {
            if ((this.m_currentSkillSlotType != SkillSlotType.SLOT_SKILL_COUNT) && (formScript != null))
            {
                this.SkillButtonUp(formScript, this.m_currentSkillSlotType, false, new Vector2());
            }
        }

        public void SkillButtonUp(CUIFormScript formScript, SkillSlotType skillSlotType, bool isTriggeredActively, [Optional] Vector2 screenPosition)
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
            if (((hostPlayer == null) || (this.m_currentSkillSlotType != skillSlotType)) || (formScript == null))
            {
                return;
            }
            if (hostPlayer.Captain != 0)
            {
                int skillLevel = 0;
                if (((hostPlayer.Captain.handle.SkillControl != null) && (skillSlotType >= SkillSlotType.SLOT_SKILL_0)) && ((skillSlotType < SkillSlotType.SLOT_SKILL_COUNT) && (hostPlayer.Captain.handle.SkillControl.SkillSlotArray[(int) skillSlotType] != null)))
                {
                    skillLevel = hostPlayer.Captain.handle.SkillControl.SkillSlotArray[(int) skillSlotType].GetSkillLevel();
                }
                if (skillLevel <= 0)
                {
                    return;
                }
            }
            SkillButton button = this.GetButton(skillSlotType);
            if (button == null)
            {
                return;
            }
            GameObject animationPresent = button.GetAnimationPresent();
            if (skillSlotType == SkillSlotType.SLOT_SKILL_0)
            {
                if (this.m_commonAtkSlide)
                {
                    this.CommonAtkSlide(formScript, screenPosition);
                    this.m_commonAtkSlide = false;
                }
                this.SendUseCommonAttack(0, 0);
                goto Label_0297;
            }
            if (skillSlotType != SkillSlotType.SLOT_SKILL_0)
            {
                CUICommonSystem.PlayAnimation(animationPresent, enSkillButtonAnimationName.pressUp.ToString(), false);
            }
            if (isTriggeredActively && !this.m_currentSkillIndicatorInCancelArea)
            {
                if (skillSlotType != SkillSlotType.SLOT_SKILL_0)
                {
                    SkillSlot slot;
                    enSkillJoystickMode skillJoystickMode = this.GetSkillJoystickMode(skillSlotType);
                    switch (skillJoystickMode)
                    {
                        case enSkillJoystickMode.MapSelect:
                        case enSkillJoystickMode.MapSelectOther:
                            goto Label_0273;
                    }
                    uint objID = 0;
                    if ((((GameSettings.TheCastType == CastType.LunPanCast) && GameSettings.ShowEnemyHeroHeadBtnMode) && (GameSettings.LunPanLockEnemyHeroMode && (hostPlayer.Captain != 0))) && ((hostPlayer.Captain.handle.SkillControl != null) && hostPlayer.Captain.handle.SkillControl.TryGetSkillSlot(skillSlotType, out slot)))
                    {
                        Skill skill = (slot.NextSkillObj == null) ? slot.SkillObj : slot.NextSkillObj;
                        if ((((skill != null) && (skill.cfgData != null)) && ((skill.cfgData.bRangeAppointType == 1) && slot.CanUseSkillWithEnemyHeroSelectMode())) && ((skillJoystickMode == enSkillJoystickMode.General) && ((objID = Singleton<CBattleSystem>.instance.FightForm.m_enemyHeroAtkBtn.GetLockedEnemyHeroObjId()) > 0)))
                        {
                            Singleton<CBattleSystem>.instance.FightForm.m_enemyHeroAtkBtn.OnSkillBtnUp(skillSlotType, this.m_currentSkillJoystickMode);
                        }
                    }
                    if (objID == 0)
                    {
                        if (this.m_currentSkillJoystickSelectedIndex != -1)
                        {
                            objID = this.m_targetInfos[this.m_currentSkillJoystickSelectedIndex].m_objectID;
                        }
                        this.RequestUseSkillSlot(skillSlotType, this.m_currentSkillJoystickMode, objID);
                    }
                }
            }
            else
            {
                this.CancelUseSkillSlot(skillSlotType, this.m_currentSkillJoystickMode);
            }
        Label_0273:
            if (this.m_currentSkillIndicatorEnabled)
            {
                this.DisableSkillCursor(formScript, skillSlotType);
            }
            if (this.m_currentSkillSlotType == SkillSlotType.SLOT_SKILL_3)
            {
                MinimapSkillIndicator_3DUI.CancelIndicator();
            }
        Label_0297:
            if (IsSelectedTargetJoyStick(this.m_currentSkillJoystickMode) && (this.m_currentSkillJoystickSelectedIndex >= 0))
            {
                this.m_currentSkillJoystickSelectedIndex = -1;
                this.SetSkillIndicatorSelectedTarget(this.m_currentSkillJoystickSelectedIndex);
            }
            this.m_currentSkillSlotType = SkillSlotType.SLOT_SKILL_COUNT;
            this.m_currentSkillDownScreenPosition = Vector2.get_zero();
        }

        private int SkillJoystickSelectTarget(CUIFormScript battleFormScript, GameObject skillJoystick, ref Vector2 screenPosition)
        {
            Vector2 point = screenPosition - this.m_currentSkillIndicatorScreenPosition;
            if ((battleFormScript != null) && (battleFormScript.ChangeScreenValueToForm(point.get_magnitude()) <= 270f))
            {
                float radian = GetRadian(point);
                if ((battleFormScript != null) && (skillJoystick != null))
                {
                    CUIComponent component = skillJoystick.GetComponent<CUIComponent>();
                    if (((component != null) && (component.m_widgets != null)) && (component.m_widgets.Length >= this.m_targetInfos.Length))
                    {
                        for (int i = 0; i < this.m_targetInfos.Length; i++)
                        {
                            GameObject obj2 = component.m_widgets[i];
                            if (((obj2 != null) && obj2.get_activeSelf()) && ((0 != 0) || ((radian >= this.m_targetInfos[i].m_headAreaFan.m_minRadian) && (radian <= this.m_targetInfos[i].m_headAreaFan.m_maxRadian))))
                            {
                                return i;
                            }
                        }
                    }
                }
            }
            return -1;
        }

        public void UpdateButtonBeanNum(SkillSlotType skillSlotType, int value)
        {
            SkillButton button = this.GetButton(skillSlotType);
            if (value > 0)
            {
                this.SetEnableButton(skillSlotType);
            }
            else
            {
                this.SetEnergyDisableButton(skillSlotType);
            }
            if (button.m_beanText != null)
            {
                Text component = button.m_beanText.GetComponent<Text>();
                if (component != null)
                {
                    component.set_text(SimpleNumericString.GetNumeric(value));
                }
                button.m_beanText.CustomSetActive(true);
            }
        }

        public void UpdateButtonCD(SkillSlotType skillSlotType, int cd)
        {
            SkillButton button = this.GetButton(skillSlotType);
            if (cd <= 0)
            {
                this.SetEnableButton(skillSlotType);
            }
            this.UpdateButtonCDText((button == null) ? null : button.m_button, (button == null) ? null : button.m_cdText, cd);
        }

        private void UpdateButtonCDText(GameObject button, GameObject cdText, int cd)
        {
            if (cdText != null)
            {
                if (cd <= 0)
                {
                    cdText.CustomSetActive(false);
                }
                else
                {
                    if ((button != null) && button.get_activeSelf())
                    {
                        cdText.CustomSetActive(true);
                    }
                    Text component = cdText.GetComponent<Text>();
                    if (component != null)
                    {
                        component.set_text(SimpleNumericString.GetNumeric(Mathf.CeilToInt((float) (cd / 0x3e8)) + 1));
                    }
                }
            }
            if ((button != null) && (cdText != null))
            {
                cdText.get_transform().set_position(button.get_transform().get_position());
            }
        }

        public void UpdateLogic(int delta)
        {
            for (int i = 0; i < this._skillButtons.Length; i++)
            {
                SkillButton button = this._skillButtons[i];
                if ((button != null) && (null != button.effectTimeImage))
                {
                    button.effectTimeLeft -= delta;
                    if (button.effectTimeLeft < 0)
                    {
                        button.effectTimeLeft = 0;
                    }
                    button.effectTimeImage.CustomFillAmount(((float) button.effectTimeLeft) / ((float) button.effectTimeTotal));
                    if (button.effectTimeLeft <= 0)
                    {
                        button.effectTimeTotal = 0;
                        button.effectTimeImage.get_gameObject().CustomSetActive(false);
                        button.effectTimeImage = null;
                    }
                }
            }
        }

        public bool CurrentSkillIndicatorResponed
        {
            get
            {
                return this.m_currentSkillIndicatorResponed;
            }
        }

        public bool CurrentSkillTipsResponed
        {
            get
            {
                return this.m_currentSkillTipsResponed;
            }
        }
    }
}

