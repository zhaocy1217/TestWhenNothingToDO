namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.GameKernal;
    using ResData;
    using System;
    using System.Runtime.InteropServices;

    public class LockTargetAttackMode : BaseAttackMode
    {
        private uint lockTargetID;

        public override bool CancelCommonAttackMode()
        {
            if (base.actor.SkillControl.SkillUseCache != null)
            {
                base.actor.SkillControl.SkillUseCache.SetCommonAttackMode(false);
            }
            return true;
        }

        public void ClearTargetID()
        {
            LockTargetEventParam param = new LockTargetEventParam(this.lockTargetID);
            Singleton<GameSkillEventSys>.GetInstance().SendEvent<LockTargetEventParam>(GameSkillEventDef.Event_ClearLockTarget, base.GetActor(), ref param, GameSkillEventChannel.Channel_HostCtrlActor);
            this.lockTargetID = 0;
        }

        public override uint CommonAttackSearchEnemy(int srchR)
        {
            SkillCache skillUseCache = null;
            if (base.commonAttackEnemyHeroTargetID > 0)
            {
                PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(base.commonAttackEnemyHeroTargetID);
                if ((actor == 0) || actor.handle.ActorControl.IsDeadState)
                {
                    this.ClearTargetID();
                    base.SetEnemyHeroAttackTargetID(0);
                    return 0;
                }
                this.SetLockTargetID(base.commonAttackEnemyHeroTargetID, true);
                return base.commonAttackEnemyHeroTargetID;
            }
            uint selectID = 0;
            bool bSearched = false;
            selectID = base.ExecuteSearchTraget(srchR, ref bSearched);
            if (bSearched)
            {
                if (!base.IsValidTargetID(selectID))
                {
                    skillUseCache = base.actor.SkillControl.SkillUseCache;
                    if ((skillUseCache != null) && !skillUseCache.GetSpecialCommonAttack())
                    {
                        this.CancelCommonAttackMode();
                        selectID = 0;
                    }
                }
                if (selectID == 0)
                {
                    this.ClearTargetID();
                    return selectID;
                }
                this.SetLockTargetID(selectID, false);
            }
            return selectID;
        }

        public uint GetLockTargetID()
        {
            return this.lockTargetID;
        }

        public bool IsValidLockTargetID(uint _targetID)
        {
            bool flag = false;
            if (_targetID > 0)
            {
                PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(_targetID);
                flag = ((((actor != 0) && !actor.handle.ObjLinker.Invincible) && (!actor.handle.ActorControl.IsDeadState && !base.actor.IsSelfCamp((ActorRoot) actor))) && actor.handle.HorizonMarker.IsVisibleFor(base.actor.TheActorMeta.ActorCamp)) && actor.handle.AttackOrderReady;
                if (!flag)
                {
                    return flag;
                }
                long num = 0x2710L;
                num *= num;
                VInt3 num2 = base.actor.ActorControl.actorLocation - actor.handle.location;
                if (num2.sqrMagnitudeLong2D > num)
                {
                    return false;
                }
            }
            return flag;
        }

        public bool IsValidSkillTargetID(uint _targetID, uint _targetMask)
        {
            if (_targetID <= 0)
            {
                return false;
            }
            PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(_targetID);
            return ((actor != 0) && ((_targetMask & (((int) 1) << actor.handle.TheActorMeta.ActorType)) <= 0L));
        }

        protected override uint LastHitAttackSearchTarget(int srchR, SelectEnemyType type, ref bool bSearched)
        {
            uint lockTargetID = this.lockTargetID;
            if (base.IsValidTargetID(lockTargetID) && !base.TargetType(lockTargetID, ActorTypeDef.Actor_Type_Hero))
            {
                bSearched = false;
                return lockTargetID;
            }
            bSearched = true;
            int searchRange = 0;
            if (type == SelectEnemyType.SelectLowHp)
            {
                searchRange = base.actor.ActorControl.AttackRange;
                lockTargetID = Singleton<AttackModeTargetSearcher>.GetInstance().SearchLowestHpTarget(ref this.actorPtr, searchRange, 0, false, SearchTargetPriority.LastHit);
                if ((lockTargetID > 0) && !base.TargetType(lockTargetID, ActorTypeDef.Actor_Type_Hero))
                {
                    return lockTargetID;
                }
                searchRange = base.actor.ActorControl.SearchRange;
                lockTargetID = Singleton<AttackModeTargetSearcher>.GetInstance().SearchLowestHpTarget(ref this.actorPtr, searchRange, 0, false, SearchTargetPriority.LastHit);
                if ((lockTargetID > 0) && !base.TargetType(lockTargetID, ActorTypeDef.Actor_Type_Hero))
                {
                    return lockTargetID;
                }
                searchRange = base.actor.ActorControl.SearchRange;
                return Singleton<AttackModeTargetSearcher>.GetInstance().SearchNearestTarget(ref this.actorPtr, searchRange, 0, true, SearchTargetPriority.LastHit);
            }
            searchRange = base.actor.ActorControl.SearchRange;
            return Singleton<AttackModeTargetSearcher>.GetInstance().SearchNearestTarget(ref this.actorPtr, searchRange, 0, true, SearchTargetPriority.LastHit);
        }

        protected override uint LastHitModeCommonAttackSearchTarget(int srchR, SelectEnemyType type, ref bool bSearched)
        {
            uint lockTargetID = this.lockTargetID;
            if (base.IsValidTargetID(lockTargetID) && base.TargetType(lockTargetID, ActorTypeDef.Actor_Type_Hero))
            {
                bSearched = false;
                return lockTargetID;
            }
            bSearched = true;
            int searchRange = 0;
            if (type == SelectEnemyType.SelectLowHp)
            {
                searchRange = base.actor.ActorControl.AttackRange;
                lockTargetID = Singleton<AttackModeTargetSearcher>.GetInstance().SearchLowestHpTarget(ref this.actorPtr, searchRange, 0, false, SearchTargetPriority.CommonAttack);
                if ((lockTargetID > 0) && base.TargetType(lockTargetID, ActorTypeDef.Actor_Type_Hero))
                {
                    return lockTargetID;
                }
                searchRange = base.actor.ActorControl.SearchRange;
                lockTargetID = Singleton<AttackModeTargetSearcher>.GetInstance().SearchLowestHpTarget(ref this.actorPtr, searchRange, 0, false, SearchTargetPriority.CommonAttack);
                if ((lockTargetID > 0) && base.TargetType(lockTargetID, ActorTypeDef.Actor_Type_Hero))
                {
                    return lockTargetID;
                }
                searchRange = base.actor.ActorControl.SearchRange;
                return Singleton<AttackModeTargetSearcher>.GetInstance().SearchNearestTarget(ref this.actorPtr, searchRange, 0, true, SearchTargetPriority.CommonAttack);
            }
            searchRange = base.actor.ActorControl.SearchRange;
            return Singleton<AttackModeTargetSearcher>.GetInstance().SearchNearestTarget(ref this.actorPtr, searchRange, 0, true, SearchTargetPriority.CommonAttack);
        }

        protected override uint NormalModeCommonAttackSearchTarget(int srchR, SelectEnemyType type, ref bool bSearched)
        {
            uint lockTargetID = this.lockTargetID;
            if (base.IsValidTargetID(lockTargetID))
            {
                bSearched = false;
                return lockTargetID;
            }
            if (type == SelectEnemyType.SelectLowHp)
            {
                lockTargetID = Singleton<AttackModeTargetSearcher>.GetInstance().SearchLowestHpTarget(ref this.actorPtr, srchR, 0, true, SearchTargetPriority.CommonAttack);
            }
            else
            {
                lockTargetID = Singleton<AttackModeTargetSearcher>.GetInstance().SearchNearestTarget(ref this.actorPtr, srchR, 0, true, SearchTargetPriority.CommonAttack);
            }
            bSearched = true;
            return lockTargetID;
        }

        public override void OnDead()
        {
            this.ClearTargetID();
        }

        public override void OnUse()
        {
            base.OnUse();
            this.lockTargetID = 0;
            base.commonAttackEnemyHeroTargetID = 0;
        }

        public override VInt3 SelectSkillDirection(SkillSlot _slot)
        {
            Skill skill = (_slot.NextSkillObj == null) ? _slot.SkillObj : _slot.NextSkillObj;
            return (VInt3) _slot.skillIndicator.GetUseSkillDirection();
        }

        public override bool SelectSkillPos(SkillSlot _slot, out VInt3 _position)
        {
            Skill skill = (_slot.NextSkillObj == null) ? _slot.SkillObj : _slot.NextSkillObj;
            if (_slot.skillIndicator.IsAllowUseSkill())
            {
                _position = _slot.skillIndicator.GetUseSkillPosition();
                return true;
            }
            _position = VInt3.zero;
            return false;
        }

        public override uint SelectSkillTarget(SkillSlot _slot)
        {
            int maxSearchDistance = 0;
            uint dwSkillTargetFilter = 0;
            ActorRoot useSkillTargetLockAttackMode = null;
            uint lockTargetID = this.lockTargetID;
            SelectEnemyType selectLowHp = SelectEnemyType.SelectLowHp;
            Player ownerPlayer = ActorHelper.GetOwnerPlayer(ref this.actorPtr);
            SkillSelectControl instance = Singleton<SkillSelectControl>.GetInstance();
            Skill skill = (_slot.NextSkillObj == null) ? _slot.SkillObj : _slot.NextSkillObj;
            dwSkillTargetFilter = skill.cfgData.dwSkillTargetFilter;
            if (skill.cfgData.bSkillTargetRule == 2)
            {
                return base.actor.ObjID;
            }
            useSkillTargetLockAttackMode = _slot.skillIndicator.GetUseSkillTargetLockAttackMode();
            if (useSkillTargetLockAttackMode != null)
            {
                if (this.IsValidLockTargetID(useSkillTargetLockAttackMode.ObjID))
                {
                    lockTargetID = useSkillTargetLockAttackMode.ObjID;
                    Singleton<NetLockAttackTarget>.GetInstance().SendLockAttackTarget(lockTargetID);
                }
                return lockTargetID;
            }
            if (!this.IsValidLockTargetID(this.lockTargetID))
            {
                if (ownerPlayer != null)
                {
                    selectLowHp = ownerPlayer.AttackTargetMode;
                }
                if (skill.AppointType == SkillRangeAppointType.Target)
                {
                    maxSearchDistance = skill.GetMaxSearchDistance(_slot.GetSkillLevel());
                }
                else
                {
                    maxSearchDistance = (int) skill.cfgData.iMaxAttackDistance;
                }
                if (selectLowHp == SelectEnemyType.SelectLowHp)
                {
                    lockTargetID = Singleton<AttackModeTargetSearcher>.GetInstance().SearchLowestHpTarget(ref this.actorPtr, maxSearchDistance, dwSkillTargetFilter, true, SearchTargetPriority.CommonAttack);
                }
                else
                {
                    lockTargetID = Singleton<AttackModeTargetSearcher>.GetInstance().SearchNearestTarget(ref this.actorPtr, maxSearchDistance, dwSkillTargetFilter, true, SearchTargetPriority.CommonAttack);
                }
                if (this.IsValidLockTargetID(lockTargetID))
                {
                    Singleton<NetLockAttackTarget>.GetInstance().SendLockAttackTarget(lockTargetID);
                }
                return lockTargetID;
            }
            if (!this.IsValidSkillTargetID(lockTargetID, dwSkillTargetFilter))
            {
                lockTargetID = 0;
            }
            return lockTargetID;
        }

        public void SetLockTargetID(uint _targetID, bool bIsFromAttackEnemyHero = false)
        {
            if (!bIsFromAttackEnemyHero && (base.commonAttackEnemyHeroTargetID != 0))
            {
                base.commonAttackEnemyHeroTargetID = 0;
            }
            if (this.lockTargetID != _targetID)
            {
                this.lockTargetID = _targetID;
                LockTargetEventParam param = new LockTargetEventParam(this.lockTargetID);
                Singleton<GameSkillEventSys>.GetInstance().SendEvent<LockTargetEventParam>(GameSkillEventDef.Event_LockTarget, base.GetActor(), ref param, GameSkillEventChannel.Channel_HostCtrlActor);
            }
        }

        public override void UpdateLogic(int nDelta)
        {
            if ((this.lockTargetID != 0) && !this.IsValidLockTargetID(this.lockTargetID))
            {
                this.ClearTargetID();
            }
            if (((base.actorPtr != 0) && (this.actorPtr.handle.ActorAgent != null)) && (this.actorPtr.handle.ActorAgent.m_wrapper != null))
            {
                ObjBehaviMode myBehavior = this.actorPtr.handle.ActorAgent.m_wrapper.myBehavior;
                if (((myBehavior != ObjBehaviMode.Normal_Attack) && ((myBehavior <= ObjBehaviMode.UseSkill_0) || (myBehavior >= ObjBehaviMode.UseSkill_7))) && (base.commonAttackEnemyHeroTargetID != 0))
                {
                    base.commonAttackEnemyHeroTargetID = 0;
                }
            }
        }
    }
}

