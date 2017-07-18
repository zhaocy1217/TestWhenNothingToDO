namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using System;

    [EventCategory("MMGame/Skill")]
    public class SkillCDTriggerDuration : DurationCondition
    {
        public int abortReduceTime;
        public bool bAbortReduce;
        private SkillComponent skillControl;
        public SkillSlotType slotType;
        private PoolObjHandle<ActorRoot> targetActor;
        [ObjectTemplate(new Type[] {  })]
        public int targetId = -1;
        public bool useSlotType;

        public override BaseEvent Clone()
        {
            SkillCDTriggerDuration duration = ClassObjPool<SkillCDTriggerDuration>.Get();
            duration.CopyData(this);
            return duration;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            SkillCDTriggerDuration duration = src as SkillCDTriggerDuration;
            this.targetId = duration.targetId;
            this.useSlotType = duration.useSlotType;
            this.slotType = duration.slotType;
            this.abortReduceTime = duration.abortReduceTime;
            this.bAbortReduce = duration.bAbortReduce;
        }

        public override void Leave(Action _action, Track _track)
        {
            base.Leave(_action, _track);
            this.targetActor = _action.GetActorHandle(this.targetId);
            if (this.targetActor == 0)
            {
                if (ActionManager.Instance.isPrintLog)
                {
                }
            }
            else
            {
                this.skillControl = this.targetActor.handle.SkillControl;
                if (this.skillControl == null)
                {
                    if (ActionManager.Instance.isPrintLog)
                    {
                    }
                }
                else
                {
                    SkillSlot slot = null;
                    if (!this.useSlotType)
                    {
                        this.StartSkillContextCD(_action, ref slot);
                    }
                    else
                    {
                        this.StartSkillSlotCD(ref slot);
                    }
                    if (((slot != null) && this.bAbortReduce) && ((_track.curTime <= base.End) && !_track.Loop))
                    {
                        slot.ChangeSkillCD(this.abortReduceTime);
                    }
                }
            }
        }

        public override void OnUse()
        {
            base.OnUse();
            this.targetId = -1;
            this.targetActor.Release();
            this.skillControl = null;
        }

        private void StartSkillContextCD(Action _action, ref SkillSlot slot)
        {
            SkillUseContext refParamObject = _action.refParams.GetRefParamObject<SkillUseContext>("SkillContext");
            if (((refParamObject != null) && this.skillControl.TryGetSkillSlot(refParamObject.SlotType, out slot)) && (slot != null))
            {
                slot.StartSkillCD();
            }
        }

        private void StartSkillSlotCD(ref SkillSlot slot)
        {
            if (this.skillControl.TryGetSkillSlot(this.slotType, out slot) && (slot != null))
            {
                slot.StartSkillCD();
            }
        }
    }
}

