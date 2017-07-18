namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using System;

    [EventCategory("MMGame/Skill")]
    public class SetNextSkillTargetTick : TickEvent
    {
        public bool clear;
        [ObjectTemplate(new Type[] {  })]
        public int nextSkillTargetID = -1;
        public SkillSlotType slotType;
        [ObjectTemplate(new Type[] {  })]
        public int targetId = -1;

        public override BaseEvent Clone()
        {
            SetNextSkillTargetTick tick = ClassObjPool<SetNextSkillTargetTick>.Get();
            tick.CopyData(this);
            return tick;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            SetNextSkillTargetTick tick = src as SetNextSkillTargetTick;
            this.targetId = tick.targetId;
            this.nextSkillTargetID = tick.nextSkillTargetID;
            this.slotType = tick.slotType;
            this.clear = tick.clear;
        }

        public override void OnUse()
        {
            base.OnUse();
            this.targetId = -1;
            this.nextSkillTargetID = -1;
            this.slotType = SkillSlotType.SLOT_SKILL_0;
            this.clear = false;
        }

        public override void Process(Action _action, Track _track)
        {
            base.Process(_action, _track);
            PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.targetId);
            if (actorHandle != 0)
            {
                SkillSlot skillSlot = actorHandle.handle.SkillControl.GetSkillSlot(this.slotType);
                if (skillSlot != null)
                {
                    if (this.clear)
                    {
                        skillSlot.NextSkillTargetIDs.Clear();
                    }
                    else
                    {
                        PoolObjHandle<ActorRoot> handle2 = _action.GetActorHandle(this.nextSkillTargetID);
                        if ((handle2 != 0) && (skillSlot.NextSkillTargetIDs.IndexOf(handle2.handle.ObjID) < 0))
                        {
                            skillSlot.NextSkillTargetIDs.Add(handle2.handle.ObjID);
                        }
                    }
                }
            }
        }
    }
}

