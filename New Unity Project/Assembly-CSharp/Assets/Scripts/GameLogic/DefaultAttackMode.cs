namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using ResData;
    using System;
    using System.Runtime.InteropServices;

    public class DefaultAttackMode : BaseAttackMode
    {
        private uint commonAttackTargetID;
        private uint showInfoTargetID;

        public override bool CancelCommonAttackMode()
        {
            if (base.actor.SkillControl.SkillUseCache != null)
            {
                base.actor.SkillControl.SkillUseCache.SetCommonAttackMode(false);
                this.ClearCommonAttackTarget();
            }
            return true;
        }

        public void ClearCommonAttackTarget()
        {
            if (this.commonAttackTargetID != 0)
            {
                this.commonAttackTargetID = 0;
            }
        }

        public void ClearShowTargetInfo()
        {
            if (this.showInfoTargetID != 0)
            {
                if (ActorHelper.IsHostActor(ref this.actorPtr))
                {
                    SelectTargetEventParam param = new SelectTargetEventParam(this.showInfoTargetID);
                    Singleton<GameSkillEventSys>.GetInstance().SendEvent<SelectTargetEventParam>(GameSkillEventDef.Event_ClearTarget, base.GetActor(), ref param, GameSkillEventChannel.Channel_HostCtrlActor);
                }
                this.showInfoTargetID = 0;
            }
        }

        public override uint CommonAttackSearchEnemy(int srchR)
        {
            SkillCache skillUseCache = null;
            if (base.commonAttackEnemyHeroTargetID > 0)
            {
                PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(base.commonAttackEnemyHeroTargetID);
                if ((actor == 0) || actor.handle.ActorControl.IsDeadState)
                {
                    this.ClearCommonAttackTarget();
                    base.SetEnemyHeroAttackTargetID(0);
                    return 0;
                }
                this.SetCommonAttackTarget(base.commonAttackEnemyHeroTargetID, true);
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
                    this.ClearCommonAttackTarget();
                    return selectID;
                }
                this.SetCommonAttackTarget(selectID, false);
            }
            return selectID;
        }

        protected override uint LastHitAttackSearchTarget(int srchR, SelectEnemyType type, ref bool bSearched)
        {
            uint commonAttackTargetID = this.commonAttackTargetID;
            if (base.IsValidTargetID(commonAttackTargetID) && !base.TargetType(commonAttackTargetID, ActorTypeDef.Actor_Type_Hero))
            {
                this.SetCommonAttackTarget(commonAttackTargetID, false);
                bSearched = false;
                return commonAttackTargetID;
            }
            bSearched = true;
            int searchRange = 0;
            if (type == SelectEnemyType.SelectLowHp)
            {
                searchRange = base.actor.ActorControl.AttackRange;
                commonAttackTargetID = Singleton<AttackModeTargetSearcher>.GetInstance().SearchLowestHpTarget(ref this.actorPtr, searchRange, 0, false, SearchTargetPriority.LastHit);
                if ((commonAttackTargetID > 0) && !base.TargetType(commonAttackTargetID, ActorTypeDef.Actor_Type_Hero))
                {
                    return commonAttackTargetID;
                }
                searchRange = base.actor.ActorControl.SearchRange;
                commonAttackTargetID = Singleton<AttackModeTargetSearcher>.GetInstance().SearchLowestHpTarget(ref this.actorPtr, searchRange, 0, false, SearchTargetPriority.LastHit);
                if ((commonAttackTargetID > 0) && !base.TargetType(commonAttackTargetID, ActorTypeDef.Actor_Type_Hero))
                {
                    return commonAttackTargetID;
                }
                searchRange = base.actor.ActorControl.SearchRange;
                return Singleton<AttackModeTargetSearcher>.GetInstance().SearchNearestTarget(ref this.actorPtr, searchRange, 0, true, SearchTargetPriority.LastHit);
            }
            searchRange = base.actor.ActorControl.SearchRange;
            return Singleton<AttackModeTargetSearcher>.GetInstance().SearchNearestTarget(ref this.actorPtr, searchRange, 0, true, SearchTargetPriority.LastHit);
        }

        protected override uint LastHitModeCommonAttackSearchTarget(int srchR, SelectEnemyType type, ref bool bSearched)
        {
            uint commonAttackTargetID = this.commonAttackTargetID;
            if (base.IsValidTargetID(commonAttackTargetID) && base.TargetType(commonAttackTargetID, ActorTypeDef.Actor_Type_Hero))
            {
                this.SetCommonAttackTarget(commonAttackTargetID, false);
                bSearched = false;
                return commonAttackTargetID;
            }
            bSearched = true;
            int searchRange = 0;
            if (type == SelectEnemyType.SelectLowHp)
            {
                searchRange = base.actor.ActorControl.AttackRange;
                commonAttackTargetID = Singleton<AttackModeTargetSearcher>.GetInstance().SearchLowestHpTarget(ref this.actorPtr, searchRange, 0, false, SearchTargetPriority.CommonAttack);
                if ((commonAttackTargetID > 0) && base.TargetType(commonAttackTargetID, ActorTypeDef.Actor_Type_Hero))
                {
                    return commonAttackTargetID;
                }
                searchRange = base.actor.ActorControl.SearchRange;
                commonAttackTargetID = Singleton<AttackModeTargetSearcher>.GetInstance().SearchLowestHpTarget(ref this.actorPtr, searchRange, 0, false, SearchTargetPriority.CommonAttack);
                if ((commonAttackTargetID > 0) && base.TargetType(commonAttackTargetID, ActorTypeDef.Actor_Type_Hero))
                {
                    return commonAttackTargetID;
                }
                searchRange = base.actor.ActorControl.SearchRange;
                return Singleton<AttackModeTargetSearcher>.GetInstance().SearchNearestTarget(ref this.actorPtr, searchRange, 0, true, SearchTargetPriority.CommonAttack);
            }
            searchRange = base.actor.ActorControl.SearchRange;
            return Singleton<AttackModeTargetSearcher>.GetInstance().SearchNearestTarget(ref this.actorPtr, searchRange, 0, true, SearchTargetPriority.CommonAttack);
        }

        protected override uint NormalModeCommonAttackSearchTarget(int srchR, SelectEnemyType type, ref bool bSearched)
        {
            uint commonAttackTargetID = this.commonAttackTargetID;
            if (base.IsValidTargetID(commonAttackTargetID))
            {
                this.SetCommonAttackTarget(commonAttackTargetID, false);
                bSearched = false;
                return commonAttackTargetID;
            }
            if (type == SelectEnemyType.SelectLowHp)
            {
                commonAttackTargetID = Singleton<CommonAttackSearcher>.GetInstance().CommonAttackSearchLowestHpTarget(base.actor.ActorControl, srchR);
            }
            else
            {
                commonAttackTargetID = Singleton<CommonAttackSearcher>.GetInstance().CommonAttackSearchNearestTarget(base.actor.ActorControl, srchR);
            }
            bSearched = true;
            return commonAttackTargetID;
        }

        public override void OnDead()
        {
            this.ClearCommonAttackTarget();
        }

        public override void OnUse()
        {
            base.OnUse();
            this.commonAttackTargetID = 0;
            this.showInfoTargetID = 0;
            base.commonAttackEnemyHeroTargetID = 0;
        }

        public override VInt3 SelectSkillDirection(SkillSlot _slot)
        {
            SkillSelectControl instance = Singleton<SkillSelectControl>.GetInstance();
            Skill skill = (_slot.NextSkillObj == null) ? _slot.SkillObj : _slot.NextSkillObj;
            if (Singleton<GameInput>.GetInstance().IsSmartUse())
            {
                return instance.SelectTargetDir((SkillTargetRule) skill.cfgData.bSkillTargetRule, _slot);
            }
            return _slot.skillIndicator.GetUseSkillDirection();
        }

        public override bool SelectSkillPos(SkillSlot _slot, out VInt3 _position)
        {
            bool bTarget = false;
            SkillSelectControl instance = Singleton<SkillSelectControl>.GetInstance();
            Skill skill = (_slot.NextSkillObj == null) ? _slot.SkillObj : _slot.NextSkillObj;
            if (Singleton<GameInput>.GetInstance().IsSmartUse())
            {
                _position = instance.SelectTargetPos((SkillTargetRule) skill.cfgData.bSkillTargetRule, _slot, out bTarget);
                return bTarget;
            }
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
            ActorRoot useSkillTargetDefaultAttackMode;
            SkillSelectControl instance = Singleton<SkillSelectControl>.GetInstance();
            Skill skill = (_slot.NextSkillObj == null) ? _slot.SkillObj : _slot.NextSkillObj;
            if (Singleton<GameInput>.GetInstance().IsSmartUse() || (skill.cfgData.bSkillTargetRule == 2))
            {
                useSkillTargetDefaultAttackMode = instance.SelectTarget((SkillTargetRule) skill.cfgData.bSkillTargetRule, _slot);
            }
            else
            {
                useSkillTargetDefaultAttackMode = _slot.skillIndicator.GetUseSkillTargetDefaultAttackMode();
            }
            if ((useSkillTargetDefaultAttackMode != null) && base.IsValidTargetID(useSkillTargetDefaultAttackMode.ObjID))
            {
                this.SetShowTargetInfo(useSkillTargetDefaultAttackMode.ObjID);
            }
            return ((useSkillTargetDefaultAttackMode == null) ? 0 : useSkillTargetDefaultAttackMode.ObjID);
        }

        public void SetCommonAttackTarget(uint _targetID, bool bIsFromAttackEnemyHero = false)
        {
            if (!bIsFromAttackEnemyHero && (base.commonAttackEnemyHeroTargetID != 0))
            {
                base.commonAttackEnemyHeroTargetID = 0;
            }
            if (this.commonAttackTargetID != _targetID)
            {
                this.commonAttackTargetID = _targetID;
                this.SetShowTargetInfo(_targetID);
            }
        }

        public void SetShowTargetInfo(uint _TargetID)
        {
            this.showInfoTargetID = _TargetID;
            if (ActorHelper.IsHostActor(ref this.actorPtr))
            {
                SelectTargetEventParam param = new SelectTargetEventParam(this.showInfoTargetID);
                Singleton<GameSkillEventSys>.GetInstance().SendEvent<SelectTargetEventParam>(GameSkillEventDef.Event_SelectTarget, base.GetActor(), ref param, GameSkillEventChannel.Channel_HostCtrlActor);
            }
        }

        public override void UpdateLogic(int nDelta)
        {
            if ((this.showInfoTargetID != 0) && !base.IsValidTargetID(this.showInfoTargetID))
            {
                this.ClearShowTargetInfo();
            }
            if (base.actorPtr != 0)
            {
                ObjWrapper actorControl = base.actor.ActorControl;
                if (actorControl != null)
                {
                    ObjBehaviMode myBehavior = actorControl.myBehavior;
                    if ((myBehavior != ObjBehaviMode.Normal_Attack) && ((myBehavior <= ObjBehaviMode.UseSkill_0) || (myBehavior >= ObjBehaviMode.UseSkill_7)))
                    {
                        base.commonAttackEnemyHeroTargetID = 0;
                    }
                }
            }
        }
    }
}

