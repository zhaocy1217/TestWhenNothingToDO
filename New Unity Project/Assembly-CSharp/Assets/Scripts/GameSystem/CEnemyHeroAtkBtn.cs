namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.GameLogic.GameKernal;
    using Assets.Scripts.UI;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class CEnemyHeroAtkBtn
    {
        private BTN_INFO[] m_arrBtnInfo = new BTN_INFO[5];
        private static string[] m_arrHeroBtnNames = new string[] { "EnemyHeroBtn_0", "EnemyHeroBtn_1", "EnemyHeroBtn_2", "EnemyHeroBtn_3", "EnemyHeroBtn_4", "EnemyHeroBtn_5" };
        private LineRenderer m_attackLinker;
        private bool m_bIsMobaMode;
        private bool m_bLastSkillUsed = true;
        private float m_fDrawLinkerMissBtnIndexFrameTime = -1f;
        private const float m_fDrawLinkerMissBtnIndexFrameTimeMax = 500f;
        private PoolObjHandle<ActorRoot> m_hostActor;
        private int m_iCurDrawLinkerBtnIndex = -1;
        private int m_iCurEnemyPlayerCount;
        private int m_iCurTargetEnemyBtnIndex = -1;
        private int m_iEnemyDistanceMax = 0x3840;
        private const int m_iEnemyPlayerCountMax = 5;
        private int m_iLastDrawLinkerBtnIndex = -1;
        private int m_iLastSkillTargetEnemyBtnIndex = -1;
        private int m_iLastTargetEnemyBtnIndex = -1;
        private GameObject m_objDirection;
        private GameObject m_objEnemyHeroDirectionPrefab;
        private GameObject m_objLinker;
        private GameObject m_objPanelEnemyHeroAtk;
        private GameObject m_objSelectedImg;
        private SkillSlotType m_ShowGuidPrefabSkillSlot = SkillSlotType.SLOT_SKILL_VALID;
        public const string m_strEnemyHeroAttackEffectHomePath = "Prefab_Skill_Effects/Common_Effects/";
        public const string m_strEnemyHeroAttackPrefabName = "EnemyHeroAttack";
        private static Camera m_UI3DCamera = null;
        private GameObject m_ui3dRes;

        private SkillSlotType BehaviorToSkillSlotType(ObjBehaviMode behaviMode)
        {
            SkillSlotType type = SkillSlotType.SLOT_SKILL_VALID;
            switch (behaviMode)
            {
                case ObjBehaviMode.Normal_Attack:
                    return SkillSlotType.SLOT_SKILL_0;

                case ObjBehaviMode.Attack_Move:
                case ObjBehaviMode.Attack_Path:
                case ObjBehaviMode.Attack_Lock:
                case ObjBehaviMode.UseSkill_0:
                    return type;

                case ObjBehaviMode.UseSkill_1:
                    return SkillSlotType.SLOT_SKILL_1;

                case ObjBehaviMode.UseSkill_2:
                    return SkillSlotType.SLOT_SKILL_2;

                case ObjBehaviMode.UseSkill_3:
                    return SkillSlotType.SLOT_SKILL_3;
            }
            return type;
        }

        private int GetBtnIndexByActor(PoolObjHandle<ActorRoot> actorPtr)
        {
            for (int i = 0; i < this.m_iCurEnemyPlayerCount; i++)
            {
                if (actorPtr == this.m_arrBtnInfo[i].actorPtr)
                {
                    return i;
                }
            }
            return -1;
        }

        private int GetDrawAttackLinkerBtnIndex()
        {
            int iCurTargetEnemyBtnIndex = this.m_iCurTargetEnemyBtnIndex;
            if (((this.m_hostActor != 0) && (this.m_hostActor.handle.ActorAgent != null)) && ((this.m_hostActor.handle.ActorAgent.m_wrapper != null) && (iCurTargetEnemyBtnIndex < 0)))
            {
                ObjBehaviMode myBehavior = this.m_hostActor.handle.ActorAgent.m_wrapper.myBehavior;
                if ((myBehavior >= ObjBehaviMode.UseSkill_1) && (myBehavior <= ObjBehaviMode.UseSkill_3))
                {
                    SkillSlot slot;
                    SkillSlotType type = this.BehaviorToSkillSlotType(myBehavior);
                    if ((this.m_hostActor.handle.SkillControl != null) && this.m_hostActor.handle.SkillControl.TryGetSkillSlot(type, out slot))
                    {
                        Skill skill = (slot.NextSkillObj == null) ? slot.SkillObj : slot.NextSkillObj;
                        if (((skill != null) && (skill.cfgData != null)) && ((skill.cfgData.bRangeAppointType == 1) && slot.CanUseSkillWithEnemyHeroSelectMode()))
                        {
                            iCurTargetEnemyBtnIndex = this.GetBtnIndexByActor(this.m_hostActor.handle.ActorAgent.m_wrapper.myTarget);
                        }
                    }
                    return iCurTargetEnemyBtnIndex;
                }
                if (((myBehavior == ObjBehaviMode.Normal_Attack) && (this.m_hostActor.handle.ActorAgent.m_wrapper.myTarget != 0)) && ((this.m_hostActor.handle.ActorAgent.m_wrapper.myTarget.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero) && this.m_hostActor.handle.IsEnemyCamp((ActorRoot) this.m_hostActor.handle.ActorAgent.m_wrapper.myTarget)))
                {
                    iCurTargetEnemyBtnIndex = this.GetBtnIndexByActor(this.m_hostActor.handle.ActorAgent.m_wrapper.myTarget);
                }
            }
            return iCurTargetEnemyBtnIndex;
        }

        public uint GetLockedEnemyHeroObjId()
        {
            uint objID = 0;
            if (((this.m_iCurTargetEnemyBtnIndex >= 0) && (this.m_iCurTargetEnemyBtnIndex < this.m_iCurEnemyPlayerCount)) && (this.m_arrBtnInfo[this.m_iCurTargetEnemyBtnIndex].actorPtr != 0))
            {
                objID = this.m_arrBtnInfo[this.m_iCurTargetEnemyBtnIndex].actorPtr.handle.ObjID;
            }
            return objID;
        }

        private void HandleOnEnemyHeroAtkBtnUp()
        {
            if (Singleton<CBattleSystem>.GetInstance().FightForm != null)
            {
                CSkillButtonManager skillButtonManager = Singleton<CBattleSystem>.GetInstance().FightForm.m_skillButtonManager;
                if (skillButtonManager != null)
                {
                    skillButtonManager.SendUseCommonAttack(0, 0);
                }
                this.SetEnemyHeroBtnSize(this.m_iCurTargetEnemyBtnIndex, false);
                this.m_iLastTargetEnemyBtnIndex = this.m_iCurTargetEnemyBtnIndex;
                this.m_iCurTargetEnemyBtnIndex = -1;
            }
        }

        public void HideEnemyHeroHeadBtn()
        {
            for (int i = 0; i < this.m_iCurEnemyPlayerCount; i++)
            {
                this.SetBtnStateByBtnInfo(i, false);
            }
        }

        public void Init(GameObject objPanelEnemyHeroAtk)
        {
            SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
            if ((curLvelContext != null) && curLvelContext.IsMobaMode())
            {
                this.m_bIsMobaMode = true;
                Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_EnemyHeroAtkBtn_Down, new CUIEventManager.OnUIEventHandler(this.OnEnemyHeroAtkBtnDown));
                Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_EnemyHeroAtkBtn_Up, new CUIEventManager.OnUIEventHandler(this.OnEnemyHeroAtkBtnUp));
                Singleton<EventRouter>.GetInstance().AddEventHandler<uint, bool>("ActorVisibleToHostPlayerChnage", new Action<uint, bool>(this, (IntPtr) this.OnActorVisibleToHostPlayerChance));
                Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int, int>("HeroHpChange", new Action<PoolObjHandle<ActorRoot>, int, int>(this, (IntPtr) this.OnHeroHpChange));
                Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorRevive, new RefAction<DefaultGameEventParam>(this.OnActorRevive));
                Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightStart, new RefAction<DefaultGameEventParam>(this.OnFightStart));
                this.m_objPanelEnemyHeroAtk = objPanelEnemyHeroAtk;
                this.m_hostActor = Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain;
                this.m_iEnemyDistanceMax = (int) GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_ENEMYATKBTN_ENEMYDISTANCEMAX);
                int globeValue = (int) GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_HORIZON_RADIUS);
                this.m_iEnemyDistanceMax = (this.m_iEnemyDistanceMax >= globeValue) ? globeValue : this.m_iEnemyDistanceMax;
                this.m_iCurTargetEnemyBtnIndex = -1;
                this.m_iLastTargetEnemyBtnIndex = -1;
                this.m_iLastSkillTargetEnemyBtnIndex = -1;
                this.m_bLastSkillUsed = true;
            }
        }

        private void InitBtnInfo()
        {
            List<PoolObjHandle<ActorRoot>> list = ActorHelper.FilterActors(Singleton<GameObjMgr>.instance.HeroActors, new ActorFilterDelegate(Singleton<BattleLogic>.instance.FilterEnemyActor));
            this.m_iCurEnemyPlayerCount = list.Count;
            if (this.m_iCurEnemyPlayerCount > 5)
            {
                this.m_iCurEnemyPlayerCount = 5;
            }
            m_UI3DCamera = Singleton<Camera_UI3D>.GetInstance().GetCurrentCamera();
            this.m_ui3dRes = Singleton<CGameObjectPool>.GetInstance().GetGameObject("Prefab_Skill_Effects/Common_Effects/EnemyHeroAttack", enResourceType.BattleScene);
            float screenScaleValue = 1f;
            if (Singleton<CBattleSystem>.instance.FightFormScript != null)
            {
                screenScaleValue = Singleton<CBattleSystem>.instance.FightFormScript.GetScreenScaleValue();
            }
            if (this.m_ui3dRes != null)
            {
                this.m_ui3dRes.get_transform().SetParent(m_UI3DCamera.get_transform(), true);
                if (this.m_objSelectedImg == null)
                {
                    this.m_objSelectedImg = this.m_ui3dRes.get_transform().FindChild("selected").get_gameObject();
                    if (this.m_objSelectedImg != null)
                    {
                        Sprite3D component = this.m_objSelectedImg.GetComponent<Sprite3D>();
                        if (component != null)
                        {
                            component.width *= screenScaleValue;
                            component.height *= screenScaleValue;
                        }
                        this.m_objDirection = this.m_objSelectedImg.get_transform().FindChild("direction").get_gameObject();
                        if (this.m_objDirection != null)
                        {
                            Vector3 vector = this.m_objDirection.get_transform().get_position();
                            vector.y *= screenScaleValue;
                            this.m_objDirection.get_transform().set_position(vector);
                            component = this.m_objDirection.GetComponent<Sprite3D>();
                            if (component != null)
                            {
                                component.width *= screenScaleValue;
                                component.height *= screenScaleValue;
                            }
                        }
                    }
                }
                if (this.m_attackLinker == null)
                {
                    this.m_objLinker = this.m_ui3dRes.get_transform().FindChild("linker").get_gameObject();
                    if (this.m_objLinker != null)
                    {
                        this.m_attackLinker = this.m_objLinker.GetComponent<LineRenderer>();
                        if ((this.m_attackLinker != null) && (m_UI3DCamera != null))
                        {
                            this.m_attackLinker.SetVertexCount(2);
                            this.m_attackLinker.set_useWorldSpace(true);
                        }
                    }
                }
            }
            for (int i = 0; i < this.m_iCurEnemyPlayerCount; i++)
            {
                Transform transform = this.m_objPanelEnemyHeroAtk.get_transform().FindChild(m_arrHeroBtnNames[i]);
                PoolObjHandle<ActorRoot> handle = list[i];
                string heroSkinPic = CSkinInfo.GetHeroSkinPic((uint) handle.handle.TheActorMeta.ConfigId, 0);
                string prefabPath = CUIUtility.s_Sprite_Dynamic_BustCircle_Dir + heroSkinPic;
                Image image = transform.GetComponent<Image>();
                if (image != null)
                {
                    image.SetSprite(prefabPath, Singleton<CBattleSystem>.GetInstance().FormScript, true, false, false, false);
                }
                if (this.m_ui3dRes != null)
                {
                    GameObject obj2 = this.m_ui3dRes.get_transform().FindChild("hp_" + i).get_gameObject();
                    if (obj2 != null)
                    {
                        Sprite3D sprited2 = obj2.GetComponent<Sprite3D>();
                        if (sprited2 != null)
                        {
                            sprited2.width *= screenScaleValue;
                            sprited2.height *= screenScaleValue;
                        }
                        GameObject obj3 = obj2.get_transform().FindChild("hp").get_gameObject();
                        if (obj3 != null)
                        {
                            Sprite3D sprited3 = obj3.GetComponent<Sprite3D>();
                            sprited3.width *= screenScaleValue;
                            sprited3.height *= screenScaleValue;
                            this.m_arrBtnInfo[i] = new BTN_INFO(transform, list[i], ENM_ENEMY_HERO_STATE.ENM_ENEMY_HERO_STATE_TOOFAR, true, obj2, sprited3);
                        }
                    }
                }
            }
        }

        public void LateUpdate()
        {
            try
            {
                this.UpdateAndDrawAttackLinker3D();
                this.UpdateHeroAtkBtnDistance();
            }
            catch (Exception exception)
            {
                object[] inParameters = new object[] { exception.Message, exception.StackTrace };
                DebugHelper.Assert(false, "exception in DrawAttackLinker:{0}, \n {1}", inParameters);
            }
        }

        public void OnActorRevive(ref DefaultGameEventParam prm)
        {
            if (GameSettings.ShowEnemyHeroHeadBtnMode && ((prm.src != 0) && (prm.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)))
            {
                if (prm.src.handle.TheActorMeta.ActorCamp != this.m_hostActor.handle.TheActorMeta.ActorCamp)
                {
                    int btnIndexByActor = this.GetBtnIndexByActor(prm.src);
                    if ((btnIndexByActor >= 0) && (btnIndexByActor < this.m_iCurEnemyPlayerCount))
                    {
                        this.m_arrBtnInfo[btnIndexByActor].heroState &= -5;
                        this.UpdateHeroAtkBtnWithDistance(btnIndexByActor);
                    }
                }
                else if (prm.src == this.m_hostActor)
                {
                    for (int i = 0; i < this.m_iCurEnemyPlayerCount; i++)
                    {
                        this.UpdateHeroAtkBtnWithHostState(i, false);
                        this.UpdateHeroAtkBtnWithDistance(i);
                    }
                }
            }
        }

        private void OnActorVisibleToHostPlayerChance(uint uiObjId, bool bVisible)
        {
            if (GameSettings.ShowEnemyHeroHeadBtnMode)
            {
                PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.instance.GetActor(uiObjId);
                if (((actor != 0) && (actor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)) && ((this.m_hostActor != 0) && (this.m_hostActor.handle.TheActorMeta.ActorCamp != actor.handle.TheActorMeta.ActorCamp)))
                {
                    this.UpdateHeroAtkBtnWithVisible(actor, bVisible);
                }
            }
        }

        private void OnEnemyHeroAtkBtnDown(CUIEvent uiEvent)
        {
            if (uiEvent.m_srcWidget != null)
            {
                int index = int.Parse(uiEvent.m_srcWidget.get_name().Substring(uiEvent.m_srcWidget.get_name().IndexOf("_") + 1));
                if ((index >= 0) && (index < this.m_iCurEnemyPlayerCount))
                {
                    if (this.m_arrBtnInfo[index].actorPtr != 0)
                    {
                        this.m_iCurTargetEnemyBtnIndex = index;
                        uint objID = this.m_arrBtnInfo[index].actorPtr.handle.ObjID;
                        if (Singleton<CBattleSystem>.GetInstance().FightForm != null)
                        {
                            CSkillButtonManager skillButtonManager = Singleton<CBattleSystem>.GetInstance().FightForm.m_skillButtonManager;
                            if (skillButtonManager != null)
                            {
                                skillButtonManager.SendUseCommonAttack(1, objID);
                            }
                        }
                    }
                    if ((this.m_iLastTargetEnemyBtnIndex >= 0) && (this.m_iLastTargetEnemyBtnIndex < this.m_iCurEnemyPlayerCount))
                    {
                        this.SetEnemyHeroBtnHighlight(this.m_iLastTargetEnemyBtnIndex, false);
                    }
                    this.SetEnemyHeroBtnHighlight(this.m_iCurTargetEnemyBtnIndex, true);
                    this.SetEnemyHeroBtnSize(this.m_iCurTargetEnemyBtnIndex, true);
                }
            }
        }

        private void OnEnemyHeroAtkBtnUp(CUIEvent uiEvent)
        {
            this.HandleOnEnemyHeroAtkBtnUp();
        }

        private void OnFightStart(ref DefaultGameEventParam prm)
        {
            this.InitBtnInfo();
        }

        private void OnHeroDead(PoolObjHandle<ActorRoot> hero)
        {
            if (hero != 0)
            {
                if (ActorHelper.IsHostEnemyActor(ref hero))
                {
                    int btnIndexByActor = this.GetBtnIndexByActor(hero);
                    if ((btnIndexByActor >= 0) && (btnIndexByActor < this.m_iCurEnemyPlayerCount))
                    {
                        this.m_arrBtnInfo[btnIndexByActor].heroState |= 4;
                        this.SetBtnStateByBtnInfo(btnIndexByActor);
                    }
                }
                else if (hero == this.m_hostActor)
                {
                    for (int i = 0; i < this.m_iCurEnemyPlayerCount; i++)
                    {
                        this.UpdateHeroAtkBtnWithHostState(i, true);
                    }
                    this.m_iLastTargetEnemyBtnIndex = -1;
                    this.m_iCurTargetEnemyBtnIndex = -1;
                    this.m_iLastSkillTargetEnemyBtnIndex = -1;
                }
            }
        }

        private void OnHeroHpChange(PoolObjHandle<ActorRoot> hero, int iCurVal, int iMaxVal)
        {
            if (GameSettings.ShowEnemyHeroHeadBtnMode)
            {
                if ((hero != 0) && ActorHelper.IsHostEnemyActor(ref hero))
                {
                    int btnIndexByActor = this.GetBtnIndexByActor(hero);
                    this.SetBtnHpFillAmount(btnIndexByActor);
                }
                if (iCurVal <= 0)
                {
                    this.OnHeroDead(hero);
                }
            }
        }

        public void OnSkillBtnDrag(CUIFormScript formScript, SkillSlotType skillSlotType, Vector2 dragScreenPosition, bool bIsInCancelArea)
        {
            bool flag = false;
            if (((this.m_hostActor != 0) && (this.m_hostActor.handle.ActorAgent != null)) && (this.m_hostActor.handle.ActorAgent.m_wrapper != null))
            {
                BaseAttackMode currentAttackMode = this.m_hostActor.handle.ActorAgent.m_wrapper.GetCurrentAttackMode();
                uint enemyHeroAttackTargetID = 0;
                if (currentAttackMode != null)
                {
                    enemyHeroAttackTargetID = currentAttackMode.GetEnemyHeroAttackTargetID();
                }
                if (enemyHeroAttackTargetID > 0)
                {
                    flag = true;
                }
                SkillSlot slot = null;
                if (Singleton<GamePlayerCenter>.GetInstance().HostPlayerId != 0)
                {
                    Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
                    if (((hostPlayer != null) && (hostPlayer.Captain != 0)) && (hostPlayer.Captain.handle.SkillControl != null))
                    {
                        hostPlayer.Captain.handle.SkillControl.TryGetSkillSlot(skillSlotType, out slot);
                    }
                }
                if (bIsInCancelArea)
                {
                    if (!flag)
                    {
                        this.SetEnemyHeroBtnHighlight(this.m_iCurTargetEnemyBtnIndex, false);
                    }
                    this.m_iCurTargetEnemyBtnIndex = -1;
                    if (slot != null)
                    {
                        slot.skillIndicator.SetFixedWarnPrefabShow(true);
                        slot.skillIndicator.SetGuildPrefabShow(false);
                        this.m_ShowGuidPrefabSkillSlot = SkillSlotType.SLOT_SKILL_VALID;
                    }
                }
                else
                {
                    bool flag2 = false;
                    for (int i = 0; i < this.m_iCurEnemyPlayerCount; i++)
                    {
                        if (((this.m_arrBtnInfo[i].heroState == 0) && (Singleton<CBattleSystem>.GetInstance().FightForm != null)) && ((Singleton<CBattleSystem>.GetInstance().FightForm.m_skillButtonManager != null) && Singleton<CBattleSystem>.GetInstance().FightForm.m_skillButtonManager.IsSkillCursorInTargetArea(formScript, ref dragScreenPosition, this.m_arrBtnInfo[i].btnTransform.get_gameObject())))
                        {
                            if (i != this.m_iCurTargetEnemyBtnIndex)
                            {
                                if ((this.m_iCurTargetEnemyBtnIndex != -1) && !flag)
                                {
                                    this.SetEnemyHeroBtnHighlight(this.m_iCurTargetEnemyBtnIndex, false);
                                }
                                if (!flag)
                                {
                                    this.SetEnemyHeroBtnHighlight(i, true);
                                }
                                this.m_iCurTargetEnemyBtnIndex = i;
                                if (slot != null)
                                {
                                    slot.skillIndicator.SetFixedWarnPrefabShow(false);
                                    slot.skillIndicator.SetGuildPrefabShow(true);
                                    slot.skillIndicator.SetEffectPrefabShow(false);
                                    this.m_ShowGuidPrefabSkillSlot = SkillSlotType.SLOT_SKILL_VALID;
                                }
                            }
                            flag2 = true;
                            break;
                        }
                    }
                    if (!flag2)
                    {
                        if (!flag)
                        {
                            this.SetEnemyHeroBtnHighlight(this.m_iCurTargetEnemyBtnIndex, false);
                        }
                        if (slot != null)
                        {
                            slot.skillIndicator.SetFixedWarnPrefabShow(false);
                            slot.skillIndicator.SetGuildPrefabShow(false);
                            this.m_ShowGuidPrefabSkillSlot = SkillSlotType.SLOT_SKILL_VALID;
                        }
                        this.m_iCurTargetEnemyBtnIndex = -1;
                    }
                }
            }
        }

        public void OnSkillBtnUp(SkillSlotType skillSlotType, enSkillJoystickMode mode = 0)
        {
            uint objID = 0;
            if (((this.m_iCurTargetEnemyBtnIndex >= 0) && (this.m_iCurTargetEnemyBtnIndex < this.m_iCurEnemyPlayerCount)) && (this.m_arrBtnInfo[this.m_iCurTargetEnemyBtnIndex].actorPtr != 0))
            {
                objID = this.m_arrBtnInfo[this.m_iCurTargetEnemyBtnIndex].actorPtr.handle.ObjID;
                this.SetEnemyHeroBtnHighlight(this.m_iCurTargetEnemyBtnIndex, false);
                this.m_iLastSkillTargetEnemyBtnIndex = this.m_iCurTargetEnemyBtnIndex;
                this.m_iCurTargetEnemyBtnIndex = -1;
                this.m_bLastSkillUsed = false;
            }
            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
            if ((hostPlayer != null) && (hostPlayer.Captain != 0))
            {
                if (objID != 0)
                {
                    SkillSlot slot;
                    hostPlayer.Captain.handle.SkillControl.RequestUseSkillSlot(skillSlotType, enSkillJoystickMode.EnemyHeroSelect, objID);
                    if (hostPlayer.Captain.handle.SkillControl.TryGetSkillSlot(skillSlotType, out slot))
                    {
                        slot.skillIndicator.SetFixedPrefabShow(false);
                        slot.skillIndicator.SetGuildPrefabShow(false);
                        slot.skillIndicator.SetGuildWarnPrefabShow(false);
                        this.m_ShowGuidPrefabSkillSlot = skillSlotType;
                    }
                }
                else
                {
                    hostPlayer.Captain.handle.SkillControl.RequestUseSkillSlot(skillSlotType, mode, objID);
                }
            }
        }

        public static void Preload(ref ActorPreloadTab preloadTab)
        {
            preloadTab.AddParticle("Prefab_Skill_Effects/Common_Effects/EnemyHeroAttack.prefab");
        }

        private void SetBtnHpFillAmount(int iBtnIndex)
        {
            if (((iBtnIndex >= 0) && (iBtnIndex < this.m_iCurEnemyPlayerCount)) && ((this.m_arrBtnInfo[iBtnIndex].btnTransform != null) && (this.m_arrBtnInfo[iBtnIndex].actorPtr != 0)))
            {
                float num = ((float) this.m_arrBtnInfo[iBtnIndex].actorPtr.handle.ValueComponent.actorHp) / ((float) this.m_arrBtnInfo[iBtnIndex].actorPtr.handle.ValueComponent.actorHpTotal);
                Transform btnTransform = this.m_arrBtnInfo[iBtnIndex].btnTransform;
                if (btnTransform != null)
                {
                    Sprite3D objHp = this.m_arrBtnInfo[iBtnIndex].objHp;
                    Image component = btnTransform.GetComponent<Image>();
                    if ((objHp != null) && (component != null))
                    {
                        if ((num < 0.3) && this.m_arrBtnInfo[iBtnIndex].bIsUseBlueHpImage)
                        {
                            objHp.spriteName = "Battle_HP_Red_Ring";
                            component.set_color(CUIUtility.s_Color_EnemyHero_Button_PINK);
                            this.m_arrBtnInfo[iBtnIndex].bIsUseBlueHpImage = false;
                        }
                        else if ((num >= 0.3) && !this.m_arrBtnInfo[iBtnIndex].bIsUseBlueHpImage)
                        {
                            objHp.spriteName = "Battle_HP_Blue_Ring";
                            component.set_color(CUIUtility.s_Color_White);
                            this.m_arrBtnInfo[iBtnIndex].bIsUseBlueHpImage = true;
                        }
                        objHp.fillAmount = num;
                    }
                }
            }
        }

        private void SetBtnStateByBtnInfo(int iBtnIndex)
        {
            if (((iBtnIndex >= 0) && (iBtnIndex < this.m_iCurEnemyPlayerCount)) && ((this.m_arrBtnInfo != null) && (this.m_arrBtnInfo[iBtnIndex].btnTransform != null)))
            {
                if (this.m_arrBtnInfo[iBtnIndex].heroState != 0)
                {
                    if (this.m_arrBtnInfo[iBtnIndex].btnTransform.get_gameObject().get_activeSelf())
                    {
                        this.m_arrBtnInfo[iBtnIndex].btnTransform.get_gameObject().CustomSetActive(false);
                        if (this.m_arrBtnInfo[iBtnIndex].objUI3DHp != null)
                        {
                            this.m_arrBtnInfo[iBtnIndex].objUI3DHp.get_gameObject().CustomSetActive(false);
                        }
                        this.SetEnemyHeroBtnHighlight(iBtnIndex, false);
                        if (iBtnIndex == this.m_iCurTargetEnemyBtnIndex)
                        {
                            this.HandleOnEnemyHeroAtkBtnUp();
                            this.m_iCurTargetEnemyBtnIndex = -1;
                        }
                        else if ((((this.m_hostActor != 0) && (this.m_hostActor.handle.ActorAgent != null)) && ((this.m_hostActor.handle.ActorAgent.m_wrapper.myBehavior == ObjBehaviMode.Normal_Attack) && (this.m_hostActor.handle.ActorAgent.m_wrapper.myTarget != 0))) && (((this.m_arrBtnInfo[iBtnIndex].actorPtr != 0) && (this.m_hostActor.handle.ActorAgent.m_wrapper.myTarget.handle.ObjID == this.m_arrBtnInfo[iBtnIndex].actorPtr.handle.ObjID)) && (Singleton<CBattleSystem>.GetInstance().FightForm != null)))
                        {
                            CSkillButtonManager skillButtonManager = Singleton<CBattleSystem>.GetInstance().FightForm.m_skillButtonManager;
                            if (skillButtonManager != null)
                            {
                                skillButtonManager.SendUseCommonAttack(1, 0);
                                skillButtonManager.SendUseCommonAttack(0, 0);
                            }
                        }
                    }
                }
                else if (!this.m_arrBtnInfo[iBtnIndex].btnTransform.get_gameObject().get_activeSelf())
                {
                    this.SetBtnHpFillAmount(iBtnIndex);
                    this.SetEnemyHeroBtnAlpha(iBtnIndex, 0.6f);
                    this.m_arrBtnInfo[iBtnIndex].btnTransform.get_gameObject().CustomSetActive(true);
                    if (this.m_arrBtnInfo[iBtnIndex].objUI3DHp != null)
                    {
                        this.m_arrBtnInfo[iBtnIndex].objUI3DHp.get_gameObject().CustomSetActive(true);
                    }
                    CUICommonSystem.PlayAnimation(this.m_arrBtnInfo[iBtnIndex].btnTransform, enSkillButtonAnimationName.CD_End.ToString());
                }
            }
        }

        private void SetBtnStateByBtnInfo(int iBtnIndex, bool bIsShow)
        {
            if (((iBtnIndex >= 0) && (iBtnIndex < this.m_iCurEnemyPlayerCount)) && (this.m_arrBtnInfo[iBtnIndex].btnTransform != null))
            {
                this.m_arrBtnInfo[iBtnIndex].btnTransform.get_gameObject().CustomSetActive(bIsShow);
                if (this.m_arrBtnInfo[iBtnIndex].objUI3DHp != null)
                {
                    this.m_arrBtnInfo[iBtnIndex].objUI3DHp.get_gameObject().CustomSetActive(bIsShow);
                }
                if (bIsShow)
                {
                    this.SetBtnHpFillAmount(iBtnIndex);
                    CUICommonSystem.PlayAnimation(this.m_arrBtnInfo[iBtnIndex].btnTransform, enSkillButtonAnimationName.CD_End.ToString());
                }
                else if (this.m_iCurDrawLinkerBtnIndex == iBtnIndex)
                {
                    this.HandleOnEnemyHeroAtkBtnUp();
                    this.m_iCurTargetEnemyBtnIndex = -1;
                    this.m_iLastTargetEnemyBtnIndex = -1;
                    if (this.m_attackLinker != null)
                    {
                        this.m_attackLinker.get_gameObject().CustomSetActive(false);
                    }
                    if (this.m_objSelectedImg != null)
                    {
                        this.m_objSelectedImg.get_gameObject().CustomSetActive(false);
                    }
                }
            }
        }

        private void SetEnemyHeroBtnAlpha(int iBtnIndex, float fAlpha)
        {
            if (((iBtnIndex >= 0) && (iBtnIndex < this.m_iCurEnemyPlayerCount)) && (this.m_arrBtnInfo[iBtnIndex].btnTransform != null))
            {
                Transform btnTransform = this.m_arrBtnInfo[iBtnIndex].btnTransform;
                if (btnTransform != null)
                {
                    Image component = btnTransform.get_gameObject().GetComponent<Image>();
                    if (component != null)
                    {
                        Color color = component.get_color();
                        color.a = fAlpha;
                        component.set_color(color);
                    }
                }
            }
        }

        private void SetEnemyHeroBtnHighlight(int iBtnIndex, bool bIsHighlight)
        {
            if ((((iBtnIndex >= 0) && (iBtnIndex < this.m_iCurEnemyPlayerCount)) && (!bIsHighlight || this.m_arrBtnInfo[iBtnIndex].btnTransform.get_gameObject().get_activeSelf())) && (this.m_objSelectedImg != null))
            {
                if (!bIsHighlight)
                {
                    if (this.m_objSelectedImg.get_gameObject().get_activeSelf())
                    {
                        this.m_objSelectedImg.CustomSetActive(false);
                        if (this.m_objSelectedImg != null)
                        {
                            this.m_objSelectedImg.CustomSetActive(false);
                        }
                        if (this.m_objLinker != null)
                        {
                            this.m_objLinker.get_gameObject().CustomSetActive(false);
                        }
                    }
                    this.SetEnemyHeroBtnAlpha(iBtnIndex, 0.6f);
                }
                else if (bIsHighlight && (this.m_arrBtnInfo[iBtnIndex].btnTransform != null))
                {
                    this.m_objSelectedImg.get_transform().set_position(this.m_arrBtnInfo[iBtnIndex].btnTransform.get_position());
                    if (!this.m_objSelectedImg.get_gameObject().get_activeSelf())
                    {
                        this.m_objSelectedImg.get_gameObject().CustomSetActive(true);
                        if (this.m_objSelectedImg != null)
                        {
                            this.m_objSelectedImg.CustomSetActive(true);
                        }
                        if (this.m_objLinker != null)
                        {
                            this.m_objLinker.get_gameObject().CustomSetActive(true);
                        }
                    }
                    this.SetEnemyHeroBtnAlpha(iBtnIndex, 1f);
                }
            }
        }

        private void SetEnemyHeroBtnSize(int iBtnIndex, bool bIsSmall)
        {
            float num = 1f;
            if ((iBtnIndex >= 0) && (iBtnIndex < this.m_iCurEnemyPlayerCount))
            {
                if (bIsSmall)
                {
                    num = 0.8f;
                }
                if (this.m_arrBtnInfo[iBtnIndex].btnTransform != null)
                {
                    Vector3 vector = this.m_arrBtnInfo[iBtnIndex].btnTransform.get_localScale();
                    vector.x = num;
                    vector.y = num;
                    this.m_arrBtnInfo[iBtnIndex].btnTransform.set_localScale(vector);
                }
                if (this.m_arrBtnInfo[iBtnIndex].objUI3DHp != null)
                {
                    Vector3 vector2 = this.m_arrBtnInfo[iBtnIndex].objUI3DHp.get_transform().get_localScale();
                    vector2.x = num;
                    vector2.y = num;
                    this.m_arrBtnInfo[iBtnIndex].objUI3DHp.get_transform().set_localScale(vector2);
                }
                if (this.m_objSelectedImg != null)
                {
                    Vector3 vector3 = this.m_objSelectedImg.get_transform().get_localScale();
                    vector3.x = num;
                    vector3.y = num;
                    this.m_objSelectedImg.get_transform().set_localScale(vector3);
                }
            }
        }

        public void ShowEnemyHeroHeadBtn()
        {
            for (int i = 0; i < this.m_iCurEnemyPlayerCount; i++)
            {
                if (((this.m_arrBtnInfo[i].actorPtr != 0) && this.m_arrBtnInfo[i].actorPtr.handle.Visible) && ((this.m_arrBtnInfo[i].actorPtr.handle.ValueComponent != null) && (this.m_arrBtnInfo[i].actorPtr.handle.ValueComponent.actorHp > 0)))
                {
                    this.UpdateHeroAtkBtnWithDistance(i);
                }
            }
        }

        public void UnInit()
        {
            if (this.m_bIsMobaMode)
            {
                Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_EnemyHeroAtkBtn_Down, new CUIEventManager.OnUIEventHandler(this.OnEnemyHeroAtkBtnDown));
                Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_EnemyHeroAtkBtn_Up, new CUIEventManager.OnUIEventHandler(this.OnEnemyHeroAtkBtnUp));
                Singleton<EventRouter>.GetInstance().RemoveEventHandler<uint, bool>("ActorVisibleToHostPlayerChnage", new Action<uint, bool>(this, (IntPtr) this.OnActorVisibleToHostPlayerChance));
                Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int, int>("HeroHpChange", new Action<PoolObjHandle<ActorRoot>, int, int>(this, (IntPtr) this.OnHeroHpChange));
                Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorRevive, new RefAction<DefaultGameEventParam>(this.OnActorRevive));
                Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightStart, new RefAction<DefaultGameEventParam>(this.OnFightStart));
                this.m_objPanelEnemyHeroAtk = null;
                if (this.m_hostActor != 0)
                {
                    this.m_hostActor.Release();
                }
                this.m_objLinker = null;
                this.m_objSelectedImg = null;
                this.m_attackLinker = null;
                this.m_objDirection = null;
                this.m_arrBtnInfo = null;
                if (this.m_ui3dRes != null)
                {
                    Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(this.m_ui3dRes);
                }
            }
        }

        private void UpdateAndDrawAttackLinker3D()
        {
            if (GameSettings.ShowEnemyHeroHeadBtnMode)
            {
                int drawAttackLinkerBtnIndex = this.GetDrawAttackLinkerBtnIndex();
                if (((drawAttackLinkerBtnIndex < 0) || (drawAttackLinkerBtnIndex > this.m_iCurEnemyPlayerCount)) && ((this.m_iLastDrawLinkerBtnIndex >= 0) && (this.m_iLastDrawLinkerBtnIndex < this.m_iCurEnemyPlayerCount)))
                {
                    if (this.m_fDrawLinkerMissBtnIndexFrameTime < 0f)
                    {
                        this.m_fDrawLinkerMissBtnIndexFrameTime = Singleton<FrameSynchr>.instance.LogicFrameTick;
                    }
                    else if ((Singleton<FrameSynchr>.instance.LogicFrameTick - this.m_fDrawLinkerMissBtnIndexFrameTime) >= 500f)
                    {
                        this.SetEnemyHeroBtnHighlight(this.m_iLastDrawLinkerBtnIndex, false);
                        this.m_iLastDrawLinkerBtnIndex = -1;
                        this.m_fDrawLinkerMissBtnIndexFrameTime = -1f;
                        return;
                    }
                    drawAttackLinkerBtnIndex = this.m_iLastDrawLinkerBtnIndex;
                }
                if ((drawAttackLinkerBtnIndex >= 0) && (drawAttackLinkerBtnIndex < this.m_iCurEnemyPlayerCount))
                {
                    if (this.m_iLastDrawLinkerBtnIndex != drawAttackLinkerBtnIndex)
                    {
                        this.SetEnemyHeroBtnHighlight(this.m_iLastDrawLinkerBtnIndex, false);
                        this.m_iLastDrawLinkerBtnIndex = drawAttackLinkerBtnIndex;
                    }
                    this.SetEnemyHeroBtnHighlight(drawAttackLinkerBtnIndex, true);
                    this.m_iCurDrawLinkerBtnIndex = drawAttackLinkerBtnIndex;
                    PoolObjHandle<ActorRoot> actorPtr = this.m_arrBtnInfo[drawAttackLinkerBtnIndex].actorPtr;
                    if (((((actorPtr != 0) && !actorPtr.handle.ActorControl.IsDeadState) && ((this.m_hostActor != 0) && !this.m_hostActor.handle.ActorControl.IsDeadState)) && ((this.m_objSelectedImg != null) && (this.m_attackLinker != null))) && (((this.m_objSelectedImg != null) && (this.m_attackLinker != null)) && (m_UI3DCamera != null)))
                    {
                        float num2 = actorPtr.handle.CharInfo.iBulletHeight * 0.001f;
                        Vector3 vector = actorPtr.handle.myTransform.get_position();
                        vector.y += num2;
                        Vector3 vector2 = Camera.get_main().WorldToScreenPoint(vector);
                        Vector3 worldPoint = this.m_arrBtnInfo[drawAttackLinkerBtnIndex].btnTransform.get_position();
                        Vector3 vector4 = CUIUtility.WorldToScreenPoint(Singleton<CUIManager>.instance.FormCamera, worldPoint);
                        vector2.z = vector4.z = MiniMapSysUT.UI3D_Depth;
                        Vector3 vector5 = m_UI3DCamera.ScreenToWorldPoint(vector2);
                        Vector3 vector6 = m_UI3DCamera.ScreenToWorldPoint(vector4);
                        this.m_objSelectedImg.get_transform().set_position(vector6);
                        Vector2 vector7 = vector2 - vector4;
                        float num3 = Mathf.Atan2(vector7.y, vector7.x) * 57.29578f;
                        Quaternion quaternion = Quaternion.AngleAxis(num3 - 90f, Vector3.get_forward());
                        this.m_objSelectedImg.get_transform().set_rotation(quaternion);
                        this.m_attackLinker.SetPosition(0, this.m_objDirection.get_transform().get_position());
                        this.m_attackLinker.SetPosition(1, vector5);
                        if ((this.m_objSelectedImg != null) && !this.m_objSelectedImg.get_gameObject().get_activeSelf())
                        {
                            this.SetEnemyHeroBtnHighlight(drawAttackLinkerBtnIndex, true);
                        }
                        Singleton<Camera_UI3D>.GetInstance().GetCurrentCanvas().RefreshLayout(null);
                    }
                }
            }
        }

        private void UpdateHeroAtkBtnDistance()
        {
            if (GameSettings.ShowEnemyHeroHeadBtnMode && (this.m_hostActor != 0))
            {
                List<PoolObjHandle<ActorRoot>> heroActors = Singleton<GameObjMgr>.instance.HeroActors;
                int count = heroActors.Count;
                for (int i = 0; i < count; i++)
                {
                    PoolObjHandle<ActorRoot> hero = heroActors[i];
                    if (hero.handle.TheActorMeta.ActorCamp != this.m_hostActor.handle.TheActorMeta.ActorCamp)
                    {
                        this.UpdateHeroAtkBtnWithDistance(hero);
                    }
                }
            }
        }

        private void UpdateHeroAtkBtnWithDistance(PoolObjHandle<ActorRoot> hero)
        {
            int btnIndexByActor = this.GetBtnIndexByActor(hero);
            this.UpdateHeroAtkBtnWithDistance(btnIndexByActor);
        }

        private void UpdateHeroAtkBtnWithDistance(int iBtnIndex)
        {
            if ((((this.m_arrBtnInfo != null) && (this.m_hostActor != 0)) && ((iBtnIndex >= 0) && (iBtnIndex < this.m_iCurEnemyPlayerCount))) && ((((this.m_arrBtnInfo[iBtnIndex].heroState & 4) == 0) && ((this.m_arrBtnInfo[iBtnIndex].heroState & 2) == 0)) && (this.m_arrBtnInfo[iBtnIndex].actorPtr != 0)))
            {
                VInt3 num2 = this.m_hostActor.handle.location - this.m_arrBtnInfo[iBtnIndex].actorPtr.handle.location;
                if (num2.magnitude2D < this.m_iEnemyDistanceMax)
                {
                    this.m_arrBtnInfo[iBtnIndex].heroState &= -9;
                }
                else
                {
                    this.m_arrBtnInfo[iBtnIndex].heroState |= 8;
                    if (iBtnIndex == this.m_iLastTargetEnemyBtnIndex)
                    {
                        this.m_iLastTargetEnemyBtnIndex = -1;
                    }
                }
                this.SetBtnStateByBtnInfo(iBtnIndex);
            }
        }

        private void UpdateHeroAtkBtnWithHostState(int iBtnIndex, bool bDead)
        {
            if ((iBtnIndex >= 0) && (iBtnIndex < this.m_iCurEnemyPlayerCount))
            {
                if (bDead)
                {
                    this.m_arrBtnInfo[iBtnIndex].heroState |= 0x10;
                }
                else
                {
                    this.m_arrBtnInfo[iBtnIndex].heroState &= -17;
                }
                this.SetBtnStateByBtnInfo(iBtnIndex);
            }
        }

        private void UpdateHeroAtkBtnWithVisible(PoolObjHandle<ActorRoot> hero, bool bVisible)
        {
            int btnIndexByActor = this.GetBtnIndexByActor(hero);
            this.UpdateHeroAtkBtnWithVisible(btnIndexByActor, bVisible);
        }

        private void UpdateHeroAtkBtnWithVisible(int iBtnIndex, bool bVisible)
        {
            if ((iBtnIndex >= 0) && (iBtnIndex < this.m_iCurEnemyPlayerCount))
            {
                if (bVisible)
                {
                    this.m_arrBtnInfo[iBtnIndex].heroState &= -3;
                }
                else
                {
                    this.m_arrBtnInfo[iBtnIndex].heroState |= 2;
                }
                this.SetBtnStateByBtnInfo(iBtnIndex);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct BTN_INFO
        {
            public Transform btnTransform;
            public PoolObjHandle<ActorRoot> actorPtr;
            public int heroState;
            public bool bIsUseBlueHpImage;
            public Vector3 btnScreenPos;
            public Vector3 btnUI3DWorldPos;
            public GameObject objUI3DHp;
            public Sprite3D objHp;
            public BTN_INFO(Transform _btnTransform, PoolObjHandle<ActorRoot> _actorPtr, CEnemyHeroAtkBtn.ENM_ENEMY_HERO_STATE _heroState, bool _bIsUseBlueHpImage, GameObject _objUI3DHp, Sprite3D _objHp)
            {
                this.btnTransform = _btnTransform;
                this.actorPtr = _actorPtr;
                this.heroState = ((int) 1) << _heroState;
                this.bIsUseBlueHpImage = _bIsUseBlueHpImage;
                this.btnScreenPos = CUIUtility.WorldToScreenPoint(Singleton<CUIManager>.instance.FormCamera, _btnTransform.get_position());
                this.btnScreenPos.z = MiniMapSysUT.UI3D_Depth;
                this.btnUI3DWorldPos = CEnemyHeroAtkBtn.m_UI3DCamera.ScreenToWorldPoint(this.btnScreenPos);
                this.objUI3DHp = _objUI3DHp;
                this.objUI3DHp.get_transform().set_position(this.btnUI3DWorldPos);
                this.objHp = _objHp;
            }
        }

        private enum ENM_ENEMY_HERO_STATE
        {
            ENM_ENEMY_HERO_STATE_SHOW,
            ENM_ENEMY_HERO_STATE_NOTVISIBLE,
            ENM_ENEMY_HERO_STATE_NOTALIVE,
            ENM_ENEMY_HERO_STATE_TOOFAR,
            ENM_ENEMY_HERO_STATE_HOSTDEAD
        }
    }
}

