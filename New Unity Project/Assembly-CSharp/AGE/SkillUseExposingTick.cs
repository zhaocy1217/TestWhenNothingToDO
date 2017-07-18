namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using CSProtocol;
    using System;
    using System.Collections.Generic;

    [EventCategory("MMGame/Skill")]
    public class SkillUseExposingTick : TickEvent
    {
        [ObjectTemplate(new Type[] {  })]
        public int BeneficiaryId = -1;
        public int ExposeDuration = 0x7d0;
        public int InvolveRange = 0x2710;
        [ObjectTemplate(new Type[] {  })]
        public int targetId = -1;

        private bool CheckInvolvingEnemyHero(ActorRoot InActor, int srchR)
        {
            long num = srchR * srchR;
            COM_PLAYERCAMP actorCamp = InActor.TheActorMeta.ActorCamp;
            List<PoolObjHandle<ActorRoot>> heroActors = Singleton<GameObjMgr>.instance.HeroActors;
            int count = heroActors.Count;
            for (int i = 0; i < count; i++)
            {
                PoolObjHandle<ActorRoot> handle = heroActors[i];
                if (handle != 0)
                {
                    ActorRoot root = handle.handle;
                    if (root.TheActorMeta.ActorCamp != actorCamp)
                    {
                        VInt3 num5 = root.location - InActor.location;
                        if (num5.sqrMagnitudeLong2D < num)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public override BaseEvent Clone()
        {
            SkillUseExposingTick tick = ClassObjPool<SkillUseExposingTick>.Get();
            tick.CopyData(this);
            return tick;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            SkillUseExposingTick tick = src as SkillUseExposingTick;
            this.targetId = tick.targetId;
            this.BeneficiaryId = tick.BeneficiaryId;
        }

        public override void OnUse()
        {
            base.OnUse();
            this.targetId = -1;
            this.BeneficiaryId = -1;
            this.ExposeDuration = 0x7d0;
            this.InvolveRange = 0x2710;
        }

        public override void Process(Action _action, Track _track)
        {
            PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.targetId);
            if (actorHandle == 0)
            {
                return;
            }
            if (this.ExposeDuration <= 0)
            {
                return;
            }
            ActorRoot handle = actorHandle.handle;
            if (handle.HorizonMarker == null)
            {
                return;
            }
            COM_PLAYERCAMP actorCamp = handle.TheActorMeta.ActorCamp;
            COM_PLAYERCAMP attackeeCamp = COM_PLAYERCAMP.COM_PLAYERCAMP_MID;
            PoolObjHandle<ActorRoot> handle2 = _action.GetActorHandle(this.BeneficiaryId);
            if (handle2 != 0)
            {
                attackeeCamp = handle2.handle.TheActorMeta.ActorCamp;
            }
            else
            {
                switch (actorCamp)
                {
                    case COM_PLAYERCAMP.COM_PLAYERCAMP_1:
                        attackeeCamp = COM_PLAYERCAMP.COM_PLAYERCAMP_2;
                        goto Label_0090;

                    case COM_PLAYERCAMP.COM_PLAYERCAMP_2:
                        attackeeCamp = COM_PLAYERCAMP.COM_PLAYERCAMP_1;
                        goto Label_0090;
                }
            }
        Label_0090:
            if (attackeeCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_MID)
            {
                return;
            }
            if ((this.InvolveRange <= 0) || this.CheckInvolvingEnemyHero(handle, this.InvolveRange))
            {
                handle.HorizonMarker.ExposeAsAttacker(attackeeCamp, this.ExposeDuration);
            }
        }
    }
}

