namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using System;

    [EventCategory("MMGame/Skill")]
    internal class HurtTriggerDuration : DurationCondition
    {
        private bool bFirstTrigger;
        private SkillUseContext context;
        private int iLastTime;
        private int iLocalTime;
        public int iMaxTriggerCount;
        private int iTriggerCount;
        public int iTriggerInterval;
        [AssetReference(AssetRefType.SkillCombine)]
        public int iTriggerSkillCombineID;
        public SkillSlotType slotType;
        private PoolObjHandle<ActorRoot> targetActor;
        [ObjectTemplate(new Type[] {  })]
        public int targetID;

        public override BaseEvent Clone()
        {
            HurtTriggerDuration duration = ClassObjPool<HurtTriggerDuration>.Get();
            duration.CopyData(this);
            return duration;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            HurtTriggerDuration duration = src as HurtTriggerDuration;
            this.targetID = duration.targetID;
            this.iTriggerInterval = duration.iTriggerInterval;
            this.iTriggerSkillCombineID = duration.iTriggerSkillCombineID;
            this.slotType = duration.slotType;
            this.iMaxTriggerCount = duration.iMaxTriggerCount;
        }

        public override void Enter(Action _action, Track _track)
        {
            base.Enter(_action, _track);
            this.targetActor = _action.GetActorHandle(this.targetID);
            this.bFirstTrigger = true;
            this.iLastTime = 0;
            this.iLocalTime = 0;
            this.iTriggerCount = 0;
            this.context = _action.refParams.GetRefParamObject<SkillUseContext>("SkillContext");
            Singleton<GameEventSys>.instance.AddEventHandler<HurtEventResultInfo>(GameEventDef.Event_ActorDamage, new RefAction<HurtEventResultInfo>(this.OnActorDamage));
        }

        public override void Leave(Action _action, Track _track)
        {
            base.Leave(_action, _track);
            this.context = null;
            Singleton<GameEventSys>.instance.RmvEventHandler<HurtEventResultInfo>(GameEventDef.Event_ActorDamage, new RefAction<HurtEventResultInfo>(this.OnActorDamage));
        }

        private void OnActorDamage(ref HurtEventResultInfo info)
        {
            if ((((((this.iMaxTriggerCount <= 0) || (this.iTriggerCount < this.iMaxTriggerCount)) && (info.src == this.targetActor)) && (info.hpChanged < 0)) && (this.slotType == info.hurtInfo.atkSlot)) && (this.bFirstTrigger || ((this.iLocalTime - this.iLastTime) > this.iTriggerInterval)))
            {
                this.iLastTime = this.iLastTime;
                this.iTriggerCount++;
                this.TriggerAction();
            }
        }

        public override void OnUse()
        {
            base.OnUse();
            this.targetID = 0;
            this.iTriggerSkillCombineID = 0;
            this.targetActor.Release();
            this.bFirstTrigger = false;
            this.iLastTime = 0;
            this.iLocalTime = 0;
            this.iTriggerCount = 0;
            this.context = null;
        }

        public override void Process(Action _action, Track _track, int _localTime)
        {
            base.Process(_action, _track, _localTime);
            this.iLocalTime = _localTime;
        }

        private void TriggerAction()
        {
            if (((this.targetActor != 0) && (this.context != null)) && (this.iTriggerSkillCombineID > 0))
            {
                this.targetActor.handle.SkillControl.SpawnBuff(this.targetActor, this.context, this.iTriggerSkillCombineID, false);
            }
        }
    }
}

