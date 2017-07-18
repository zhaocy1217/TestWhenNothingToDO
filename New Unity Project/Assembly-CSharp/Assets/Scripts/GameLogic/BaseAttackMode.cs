namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.GameKernal;
    using System;
    using System.Runtime.InteropServices;

    public class BaseAttackMode : LogicComponent
    {
        protected CommonAttackButtonType commonAttackButtonType = CommonAttackButtonType.CommonAttackButton;
        protected uint commonAttackEnemyHeroTargetID;

        public virtual bool CancelCommonAttackMode()
        {
            return false;
        }

        public virtual uint CommonAttackSearchEnemy(int srchR)
        {
            return 0;
        }

        protected uint ExecuteSearchTraget(int srchR, ref bool bSearched)
        {
            Player ownerPlayer = ActorHelper.GetOwnerPlayer(ref this.actorPtr);
            SelectEnemyType selectLowHp = SelectEnemyType.SelectLowHp;
            LastHitMode none = LastHitMode.None;
            if (ownerPlayer != null)
            {
                selectLowHp = ownerPlayer.AttackTargetMode;
                none = ownerPlayer.useLastHitMode;
            }
            if (none == LastHitMode.None)
            {
                return this.NormalModeCommonAttackSearchTarget(srchR, selectLowHp, ref bSearched);
            }
            if (this.commonAttackButtonType == CommonAttackButtonType.CommonAttackButton)
            {
                return this.LastHitModeCommonAttackSearchTarget(srchR, selectLowHp, ref bSearched);
            }
            return this.LastHitAttackSearchTarget(srchR, selectLowHp, ref bSearched);
        }

        public uint GetEnemyHeroAttackTargetID()
        {
            return this.commonAttackEnemyHeroTargetID;
        }

        protected bool IsValidTargetID(uint selectID)
        {
            bool flag = false;
            if (selectID > 0)
            {
                PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(selectID);
                flag = ((((actor != 0) && !actor.handle.ObjLinker.Invincible) && (!actor.handle.ActorControl.IsDeadState && !base.actor.IsSelfCamp((ActorRoot) actor))) && actor.handle.HorizonMarker.IsVisibleFor(base.actor.TheActorMeta.ActorCamp)) && actor.handle.AttackOrderReady;
                if (!flag)
                {
                    return flag;
                }
                Skill nextSkill = base.actor.ActorControl.GetNextSkill(SkillSlotType.SLOT_SKILL_0);
                if (nextSkill == null)
                {
                    return flag;
                }
                long maxSearchDistance = nextSkill.GetMaxSearchDistance(0);
                if (((actor == 0) || (actor.handle.shape == null)) || ((actor.handle.ActorAgent == null) || (nextSkill.cfgData == null)))
                {
                    return false;
                }
                maxSearchDistance += actor.handle.shape.AvgCollisionRadius;
                maxSearchDistance *= maxSearchDistance;
                VInt3 num2 = base.actor.ActorControl.actorLocation - actor.handle.location;
                if (num2.sqrMagnitudeLong2D > maxSearchDistance)
                {
                    return false;
                }
            }
            return flag;
        }

        protected virtual uint LastHitAttackSearchTarget(int srchR, SelectEnemyType type, ref bool bSearched)
        {
            return 0;
        }

        protected virtual uint LastHitModeCommonAttackSearchTarget(int srchR, SelectEnemyType type, ref bool bSearched)
        {
            return 0;
        }

        protected virtual uint NormalModeCommonAttackSearchTarget(int srchR, SelectEnemyType type, ref bool bSearched)
        {
            return 0;
        }

        public virtual void OnDead()
        {
        }

        public virtual VInt3 SelectSkillDirection(SkillSlot _slot)
        {
            return VInt3.one;
        }

        public virtual bool SelectSkillPos(SkillSlot _slot, out VInt3 _position)
        {
            _position = VInt3.zero;
            return false;
        }

        public virtual uint SelectSkillTarget(SkillSlot _slot)
        {
            return 0;
        }

        public void SetCommonButtonType(sbyte type)
        {
            this.commonAttackButtonType = (CommonAttackButtonType) type;
        }

        public void SetEnemyHeroAttackTargetID(uint uiTargetId)
        {
            this.commonAttackEnemyHeroTargetID = uiTargetId;
        }

        protected bool TargetType(uint selectID, ActorTypeDef ActorType)
        {
            bool flag = false;
            if (selectID > 0)
            {
                PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(selectID);
                flag = (actor != 0) && (actor.handle.TheActorMeta.ActorType == ActorType);
            }
            return flag;
        }
    }
}

