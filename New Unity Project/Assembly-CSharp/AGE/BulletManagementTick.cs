namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using System;

    [EventCategory("MMGame/Skill")]
    public class BulletManagementTick : TickEvent
    {
        public bool bIgnoreLimit;
        public int iLifeTime;
        [ObjectTemplate(new Type[] {  })]
        public int targetId;

        public override BaseEvent Clone()
        {
            BulletManagementTick tick = ClassObjPool<BulletManagementTick>.Get();
            tick.CopyData(this);
            return tick;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            BulletManagementTick tick = src as BulletManagementTick;
            this.targetId = tick.targetId;
            this.bIgnoreLimit = tick.bIgnoreLimit;
            this.iLifeTime = tick.iLifeTime;
        }

        public override void OnUse()
        {
            base.OnUse();
            this.targetId = 0;
            this.bIgnoreLimit = false;
            this.iLifeTime = 0;
        }

        public override void Process(Action _action, Track _track)
        {
            PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.targetId);
            BaseSkill refParamObject = _action.refParams.GetRefParamObject<BaseSkill>("SkillObj");
            if (refParamObject != null)
            {
                BulletSkill skill2 = refParamObject as BulletSkill;
                if (skill2 != null)
                {
                    if (this.bIgnoreLimit)
                    {
                        skill2.IgnoreUpperLimit();
                    }
                    skill2.lifeTime = this.iLifeTime;
                }
            }
        }

        public override bool SupportEditMode()
        {
            return true;
        }
    }
}

