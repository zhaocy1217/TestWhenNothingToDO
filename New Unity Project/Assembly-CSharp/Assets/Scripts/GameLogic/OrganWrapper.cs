namespace Assets.Scripts.GameLogic
{
    using AGE;
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using CSProtocol;
    using Pathfinding;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class OrganWrapper : ObjWrapper
    {
        private GameObject[] _aroundEffects;
        private int _aroundEnemyMonsterCount;
        private LineRenderer _attackLinker;
        private int _attackOneTargetCounter;
        private int _attackOneTargetCounterLast;
        private PoolObjHandle<ActorRoot> _myLastTarget = new PoolObjHandle<ActorRoot>();
        private static Vector3 _myselfHeightVec = new Vector3(0f, 3f, 0f);
        [CompilerGenerated]
        private static Comparison<PoolObjHandle<ActorRoot>> <>f__am$cache10;
        [CompilerGenerated]
        private ResOrganCfgInfo <cfgInfo>k__BackingField;
        private int antiHiddenTimer;
        private AreaCheck attackAreaCheck;
        private long attackDistSqr;
        public const string AttackLinkerPrefab = "AttackLinker";
        private OrganHitEffect HitEffect;
        private bool m_bFirstAttacked;
        [NonSerialized, HideInInspector]
        private NavmeshCut navmeshCut;
        private int nOutCombatHpRecoveryTick;
        public const string OrganAroundEffectHomePath = "Prefab_Characters/Prefab_Organ/AroundEffect/";
        private List<PoolObjHandle<ActorRoot>> TarEyeList_;

        private bool ActorMarkFilter(ref PoolObjHandle<ActorRoot> _inActor)
        {
            if (((_inActor != 0) && (((base.actor.TheStaticData.TheOrganOnlyInfo.OrganType == 1) || (base.actor.TheStaticData.TheOrganOnlyInfo.OrganType == 2)) || (base.actor.TheStaticData.TheOrganOnlyInfo.OrganType == 4))) && (!_inActor.handle.ActorControl.IsDeadState && !base.actor.ActorControl.IsDeadState))
            {
                if (FogOfWar.enable)
                {
                    VInt3 worldLoc = new VInt3(_inActor.handle.location.x, _inActor.handle.location.z, 0);
                    if (Singleton<GameFowManager>.instance.IsSurfaceCellVisible(worldLoc, base.actor.TheActorMeta.ActorCamp))
                    {
                        VInt3 num6 = base.actor.location - _inActor.handle.location;
                        if (num6.sqrMagnitudeLong2D <= this.attackDistSqr)
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    VInt3 num7 = base.actor.location - _inActor.handle.location;
                    if (num7.sqrMagnitudeLong2D <= this.attackDistSqr)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void ActorMarkProcess(PoolObjHandle<ActorRoot> _inActor, AreaCheck.ActorAction _action)
        {
            if (_inActor != 0)
            {
                switch (_inActor.handle.TheActorMeta.ActorType)
                {
                    case ActorTypeDef.Actor_Type_Monster:
                        if (_action == AreaCheck.ActorAction.Enter)
                        {
                            if (++this._aroundEnemyMonsterCount == 1)
                            {
                                ValueDataInfo info = base.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYDEFPT] - base.actor.TheStaticData.TheOrganOnlyInfo.NoEnemyAddPhyDef;
                                info = base.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCDEFPT] - base.actor.TheStaticData.TheOrganOnlyInfo.NoEnemyAddMgcDef;
                            }
                        }
                        else if ((_action == AreaCheck.ActorAction.Leave) && (--this._aroundEnemyMonsterCount == 0))
                        {
                            ValueDataInfo info2 = base.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYDEFPT] + base.actor.TheStaticData.TheOrganOnlyInfo.NoEnemyAddPhyDef;
                            info2 = base.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCDEFPT] + base.actor.TheStaticData.TheOrganOnlyInfo.NoEnemyAddMgcDef;
                        }
                        break;

                    case ActorTypeDef.Actor_Type_Hero:
                    case ActorTypeDef.Actor_Type_EYE:
                        if (_action == AreaCheck.ActorAction.Enter)
                        {
                            _inActor.handle.HorizonMarker.AddShowMark(COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT, HorizonConfig.ShowMark.Organ, 1);
                        }
                        else if (_action == AreaCheck.ActorAction.Leave)
                        {
                            _inActor.handle.HorizonMarker.AddShowMark(COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT, HorizonConfig.ShowMark.Organ, -1);
                        }
                        break;
                }
            }
        }

        public override void Born(ActorRoot owner)
        {
            base.Born(owner);
            base.actor.isMovable = false;
            base.actor.isRotatable = false;
            VCollisionShape.InitActorCollision(base.actor);
            this.navmeshCut = base.gameObject.GetComponent<NavmeshCut>();
            if (this.navmeshCut != null)
            {
                this.navmeshCut.set_enabled(true);
            }
            this._aroundEffects = new GameObject[3];
            this.attackAreaCheck = new AreaCheck(new ActorFilterDelegate(this.ActorMarkFilter), new AreaCheck.ActorProcess(this.ActorMarkProcess), Singleton<GameObjMgr>.GetInstance().GetCampActors(base.actor.GiveMyEnemyCamp()));
            this._aroundEnemyMonsterCount = 0;
            this.cfgInfo = OrganDataHelper.GetDataCfgInfoByCurLevelDiff(base.actor.TheActorMeta.ConfigId);
            if (this.cfgInfo != null)
            {
                base.actorSubType = this.cfgInfo.bOrganType;
            }
        }

        private void ClearAntiHidden()
        {
            List<PoolObjHandle<ActorRoot>> fakeTrueEyes = Singleton<GameObjMgr>.instance.FakeTrueEyes;
            int count = this.TarEyeList_.Count;
            if (count > 0)
            {
                for (int i = count - 1; i >= 0; i--)
                {
                    if (!fakeTrueEyes.Contains(this.TarEyeList_[i]))
                    {
                        this.TarEyeList_.RemoveAt(i);
                    }
                }
            }
            this.TarEyeList_.Clear();
        }

        private void DrawAttackLinker()
        {
            if (((base.myTarget != 0) && !base.myTarget.handle.ActorControl.IsDeadState) && !base.actor.ActorControl.IsDeadState)
            {
                if (null == this._attackLinker)
                {
                    GameObject content = Singleton<CResourceManager>.GetInstance().GetResource("Prefab_Characters/Prefab_Organ/AroundEffect/AttackLinker", typeof(GameObject), enResourceType.BattleScene, false, false).m_content as GameObject;
                    if (content != null)
                    {
                        this._attackLinker = ((GameObject) Object.Instantiate(content)).GetComponent<LineRenderer>();
                        if (this._attackLinker != null)
                        {
                            this._attackLinker.get_transform().SetParent(base.actor.myTransform);
                            this._attackLinker.SetVertexCount(2);
                            this._attackLinker.set_useWorldSpace(true);
                            this._attackLinker.SetPosition(0, base.actor.myTransform.get_position() + _myselfHeightVec);
                        }
                    }
                }
                if (null != this._attackLinker)
                {
                    float num = base.myTarget.handle.CharInfo.iBulletHeight * 0.001f;
                    this._attackLinker.SetPosition(1, base.myTarget.handle.myTransform.get_position() + new Vector3(0f, num, 0f));
                    if (!this._attackLinker.get_gameObject().get_activeSelf())
                    {
                        this._attackLinker.get_gameObject().SetActive(true);
                    }
                }
            }
            else if ((null != this._attackLinker) && this._attackLinker.get_gameObject().get_activeSelf())
            {
                this._attackLinker.get_gameObject().SetActive(false);
            }
        }

        public override void Fight()
        {
            base.Fight();
            this.AddNoAbilityFlag(ObjAbilityType.ObjAbility_Move);
            this.AddNoAbilityFlag(ObjAbilityType.ObjAbility_ImmuneCrit);
            FrameCommand<AttackPositionCommand> cmd = FrameCommandFactory.CreateFrameCommand<AttackPositionCommand>();
            cmd.cmdId = 1;
            cmd.cmdData.WorldPos = base.actor.location;
            base.CmdAttackMoveToDest(cmd, base.actor.location);
            if (this.isTower)
            {
                this.HitEffect.Reset(this);
            }
            base.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_PHYARMORHURT_RATE].baseValue = base.actor.TheStaticData.TheOrganOnlyInfo.PhyArmorHurtRate;
            this._aroundEnemyMonsterCount = 0;
            ValueDataInfo info = base.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYDEFPT] + base.actor.TheStaticData.TheOrganOnlyInfo.NoEnemyAddPhyDef;
            info = base.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCDEFPT] + base.actor.TheStaticData.TheOrganOnlyInfo.NoEnemyAddMgcDef;
            if (base.actor.HorizonMarker != null)
            {
                if (FogOfWar.enable)
                {
                    if ((this.GetActorSubType() == 1) || (this.GetActorSubType() == 4))
                    {
                        base.actor.HorizonMarker.SightRadius = Horizon.QueryFowTowerSightRadius();
                    }
                    this.TarEyeList_ = new List<PoolObjHandle<ActorRoot>>();
                }
                else
                {
                    base.actor.HorizonMarker.SightRadius = base.actor.TheStaticData.TheOrganOnlyInfo.HorizonRadius;
                }
            }
        }

        private static PoolObjHandle<ActorRoot> FindEyeTarget(List<PoolObjHandle<ActorRoot>> _srcList)
        {
            if (_srcList.Count == 0)
            {
                return new PoolObjHandle<ActorRoot>();
            }
            if (_srcList.Count != 1)
            {
                if (<>f__am$cache10 == null)
                {
                    <>f__am$cache10 = delegate (PoolObjHandle<ActorRoot> a, PoolObjHandle<ActorRoot> b) {
                        EyeWrapper actorControl = (EyeWrapper) a.handle.ActorControl;
                        EyeWrapper wrapper2 = (EyeWrapper) b.handle.ActorControl;
                        int lifeTime = actorControl.LifeTime;
                        int num2 = wrapper2.LifeTime;
                        int targetHpRate = HitTriggerDurationContext.GetTargetHpRate(a);
                        int num4 = HitTriggerDurationContext.GetTargetHpRate(b);
                        if (targetHpRate < num4)
                        {
                            return -1;
                        }
                        if (targetHpRate > num4)
                        {
                            return 1;
                        }
                        if (lifeTime > num2)
                        {
                            return -1;
                        }
                        if (lifeTime < num2)
                        {
                            return 1;
                        }
                        return 0;
                    };
                }
                _srcList.Sort(<>f__am$cache10);
            }
            return _srcList[0];
        }

        public int GetAttackCounter(PoolObjHandle<ActorRoot> inActor)
        {
            int num = (this._attackOneTargetCounter <= 0) ? (!(inActor == this._myLastTarget) ? 0 : this._attackOneTargetCounterLast) : this._attackOneTargetCounter;
            this._attackOneTargetCounterLast = 0;
            return num;
        }

        public override string GetTypeName()
        {
            return "OrganWrapper";
        }

        public override void Init()
        {
            base.Init();
            SkillSlot skillSlot = base.actor.SkillControl.GetSkillSlot(SkillSlotType.SLOT_SKILL_0);
            if (skillSlot != null)
            {
                this.attackDistSqr = (long) skillSlot.SkillObj.cfgData.iMaxAttackDistance;
                this.attackDistSqr *= this.attackDistSqr;
            }
        }

        public override void LateUpdate(int delta)
        {
            try
            {
                this.DrawAttackLinker();
            }
            catch (Exception exception)
            {
                object[] inParameters = new object[] { exception.Message, exception.StackTrace };
                DebugHelper.Assert(false, "exception in DrawAttackLinker:{0}, \n {1}", inParameters);
            }
        }

        protected override void OnDead()
        {
            Singleton<CSoundManager>.GetInstance().PostEvent("Set_Theme", null);
            if (base.actor.TheStaticData.TheOrganOnlyInfo.DeadEnemySoldier > 0)
            {
                SoldierRegion soldierRegion = Singleton<BattleLogic>.GetInstance().mapLogic.GetSoldierRegion(base.actor.GiveMyEnemyCamp(), base.actor.TheStaticData.TheOrganOnlyInfo.AttackRouteID);
                if (soldierRegion != null)
                {
                    soldierRegion.SwitchWave(base.actor.TheStaticData.TheOrganOnlyInfo.DeadEnemySoldier);
                }
            }
            if (FogOfWar.enable)
            {
                this.ClearAntiHidden();
            }
            base.OnDead();
        }

        protected override void OnHpChange()
        {
            if (this.isTower)
            {
                this.HitEffect.OnHpChanged(this);
            }
            base.OnHpChange();
        }

        public override void OnMyTargetSwitch()
        {
            this._myLastTarget = base.myTarget;
            this._attackOneTargetCounterLast = this._attackOneTargetCounter;
            this._attackOneTargetCounter = 0;
        }

        protected override void OnRevive()
        {
            base.OnRevive();
            base.EnableRVO(true);
        }

        public override void OnUse()
        {
            base.OnUse();
            this.navmeshCut = null;
            this._attackOneTargetCounter = 0;
            this._attackOneTargetCounterLast = 0;
            this._myLastTarget = new PoolObjHandle<ActorRoot>();
            this.nOutCombatHpRecoveryTick = 0;
            this._aroundEffects = null;
            this._attackLinker = null;
            _myselfHeightVec = new Vector3(0f, 3f, 0f);
            this.attackAreaCheck = null;
            this._aroundEnemyMonsterCount = 0;
            this.m_bFirstAttacked = false;
            this.HitEffect = new OrganHitEffect();
            this.cfgInfo = null;
            this.antiHiddenTimer = 0;
            this.TarEyeList_ = null;
            this.attackDistSqr = 0L;
        }

        public static void Preload(ref ActorPreloadTab preloadTab)
        {
            for (int i = 0; i < 3; i++)
            {
                preloadTab.AddParticle("Prefab_Characters/Prefab_Organ/AroundEffect/" + ((OrganAroundEffect) i).ToString());
            }
            preloadTab.AddParticle("Prefab_Characters/Prefab_Organ/AroundEffect/AttackLinker");
        }

        public override bool RealUseSkill(SkillSlotType InSlot)
        {
            if (base.RealUseSkill(InSlot))
            {
                this._attackOneTargetCounter++;
                this._attackOneTargetCounterLast = 0;
                return true;
            }
            return false;
        }

        public override void Revive(bool auto)
        {
            base.Revive(auto);
            base.SetObjBehaviMode(ObjBehaviMode.Attack_Move);
        }

        public void ShowAroundEffect(OrganAroundEffect oae, bool showOrHide, bool hideOther, float scale = 1f)
        {
            if (this._aroundEffects != null)
            {
                int index = (int) oae;
                GameObject obj2 = this._aroundEffects[index];
                if (showOrHide && (null == obj2))
                {
                    GameObject content = Singleton<CResourceManager>.GetInstance().GetResource("Prefab_Characters/Prefab_Organ/AroundEffect/" + oae.ToString(), typeof(GameObject), enResourceType.BattleScene, false, false).m_content as GameObject;
                    if (content != null)
                    {
                        obj2 = Object.Instantiate(content);
                        DebugHelper.Assert(obj2 != null);
                        if (obj2 != null)
                        {
                            Transform transform = obj2.get_transform();
                            if (transform != null)
                            {
                                transform.SetParent(base.actor.myTransform);
                                transform.set_localPosition(Vector3.get_zero());
                                transform.set_localScale(Vector3.get_one());
                                transform.set_localRotation(Quaternion.get_identity());
                            }
                            ParticleScaler[] componentsInChildren = obj2.GetComponentsInChildren<ParticleScaler>(true);
                            for (int i = 0; i < componentsInChildren.Length; i++)
                            {
                                componentsInChildren[i].particleScale = scale;
                                componentsInChildren[i].CheckAndApplyScale();
                            }
                        }
                        this._aroundEffects[index] = obj2;
                    }
                }
                if ((null != obj2) && (obj2.get_activeSelf() != showOrHide))
                {
                    obj2.SetActive(showOrHide);
                    if (showOrHide && (oae == OrganAroundEffect.HostPlayerInAndHit))
                    {
                        Singleton<CSoundManager>.GetInstance().PostEvent("UI_Prompt_fangyuta_atk", null);
                    }
                }
                if (hideOther)
                {
                    for (int j = 0; j < this._aroundEffects.Length; j++)
                    {
                        if (((j != index) && (null != this._aroundEffects[j])) && this._aroundEffects[j].get_activeSelf())
                        {
                            this._aroundEffects[j].SetActive(false);
                        }
                    }
                }
            }
        }

        public override int TakeDamage(ref HurtDataInfo hurt)
        {
            if (!this.m_bFirstAttacked)
            {
                this.m_bFirstAttacked = true;
                if (hurt.atker.handle.TheActorMeta.ActorCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
                {
                    Singleton<EventRouter>.instance.BroadCastEvent<COM_PLAYERCAMP>(EventID.CampTowerFirstAttackTime, COM_PLAYERCAMP.COM_PLAYERCAMP_2);
                }
                else if (hurt.atker.handle.TheActorMeta.ActorCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_2)
                {
                    Singleton<EventRouter>.instance.BroadCastEvent<COM_PLAYERCAMP>(EventID.CampTowerFirstAttackTime, COM_PLAYERCAMP.COM_PLAYERCAMP_1);
                }
            }
            hurt.iReduceDamage = 0;
            if (((hurt.atker != 0) && (hurt.atker.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)) && (hurt.atkSlot == SkillSlotType.SLOT_SKILL_0))
            {
                if ((this.cfgInfo.iBlockHeroAtkDamageMSec == 0) || (this.cfgInfo.iBlockHeroAtkDamageMSec >= Singleton<FrameSynchr>.instance.LogicFrameTick))
                {
                    hurt.iReduceDamage += this.cfgInfo.iBlockHeroAtkDamage;
                }
                if ((this._aroundEnemyMonsterCount == 0) && ((this.cfgInfo.iNoEnemyBlockHeroAtkDamageMSec == 0) || (this.cfgInfo.iNoEnemyBlockHeroAtkDamageMSec >= Singleton<FrameSynchr>.instance.LogicFrameTick)))
                {
                    hurt.iReduceDamage += this.cfgInfo.iNoEnemyBlockHeroAtkDamage;
                }
            }
            return base.TakeDamage(ref hurt);
        }

        private void UpdateAntiHiddenEyeHurt(int inDelta)
        {
            if (!base.IsDeadState && (((base.actorSubType == 1) || (base.actorSubType == 4)) || (base.actorSubType == 2)))
            {
                GlobalConfig instance = MonoSingleton<GlobalConfig>.instance;
                int defenseAntiHiddenFrameInterval = instance.DefenseAntiHiddenFrameInterval;
                this.antiHiddenTimer += inDelta;
                if ((this.antiHiddenTimer >= instance.DefenseAntiHiddenInterval) && ((((ulong) base.actor.ObjID) % ((long) defenseAntiHiddenFrameInterval)) == (((ulong) Singleton<FrameSynchr>.instance.CurFrameNum) % ((long) defenseAntiHiddenFrameInterval))))
                {
                    this.antiHiddenTimer = 0;
                    List<PoolObjHandle<ActorRoot>> fakeTrueEyes = Singleton<GameObjMgr>.instance.FakeTrueEyes;
                    int count = fakeTrueEyes.Count;
                    for (int i = 0; i < count; i++)
                    {
                        PoolObjHandle<ActorRoot> item = fakeTrueEyes[i];
                        if ((item != 0) && !this.TarEyeList_.Contains(item))
                        {
                            ActorRoot handle = item.handle;
                            if (handle.IsEnemyCamp(base.actor))
                            {
                                VInt3 worldLoc = new VInt3(handle.location.x, handle.location.z, 0);
                                if (Singleton<GameFowManager>.instance.IsSurfaceCellVisible(worldLoc, base.actor.TheActorMeta.ActorCamp))
                                {
                                    VInt3 num9 = handle.location - base.actor.location;
                                    if (num9.sqrMagnitudeLong2D < this.attackDistSqr)
                                    {
                                        this.TarEyeList_.Add(item);
                                    }
                                }
                            }
                        }
                    }
                    int num5 = this.TarEyeList_.Count;
                    if (num5 > 0)
                    {
                        for (int j = num5 - 1; j >= 0; j--)
                        {
                            if (!fakeTrueEyes.Contains(this.TarEyeList_[j]))
                            {
                                this.TarEyeList_.RemoveAt(j);
                            }
                        }
                    }
                    PoolObjHandle<ActorRoot> inTargetActor = FindEyeTarget(this.TarEyeList_);
                    if (inTargetActor != 0)
                    {
                        new BufConsumer(instance.DefenseAntiHiddenHurtId, inTargetActor, base.actorPtr).Use();
                    }
                }
            }
        }

        public override void UpdateLogic(int nDelta)
        {
            base.actor.ActorAgent.UpdateLogic(nDelta);
            base.UpdateLogic(nDelta);
            if (this.attackAreaCheck != null)
            {
                this.attackAreaCheck.UpdateLogic(base.actor.ObjID);
            }
            if (base.IsInBattle)
            {
                this.nOutCombatHpRecoveryTick = 0;
            }
            else
            {
                this.nOutCombatHpRecoveryTick += nDelta;
                if (this.nOutCombatHpRecoveryTick >= 0x3e8)
                {
                    int nAddHp = (this.cfgInfo.iOutBattleHPAdd * base.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalValue) / 0x2710;
                    base.ReviveHp(nAddHp);
                    this.nOutCombatHpRecoveryTick -= 0x3e8;
                }
            }
            if (FogOfWar.enable)
            {
                this.UpdateAntiHiddenEyeHurt(nDelta);
            }
        }

        public ResOrganCfgInfo cfgInfo
        {
            [CompilerGenerated]
            get
            {
                return this.<cfgInfo>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<cfgInfo>k__BackingField = value;
            }
        }

        public override int CfgReviveCD
        {
            get
            {
                return 0x7fffffff;
            }
        }

        public bool isTower
        {
            get
            {
                return ((base.actor != null) && ((base.actor.TheStaticData.TheOrganOnlyInfo.OrganType == 1) || (base.actor.TheStaticData.TheOrganOnlyInfo.OrganType == 4)));
            }
        }
    }
}

