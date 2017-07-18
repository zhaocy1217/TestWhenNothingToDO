namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using System;

    [EventCategory("MMGame/Skill")]
    public class SetNextSkillTargetDuration : DurationCondition
    {
        private PoolObjHandle<ActorRoot> nextSkillObj;
        [ObjectTemplate(new Type[] {  })]
        public int nextSkillTargetID = -1;
        public SkillSlotType slotType;
        [ObjectTemplate(new Type[] {  })]
        public int targetId = -1;

        public override BaseEvent Clone()
        {
            SetNextSkillTargetDuration duration = ClassObjPool<SetNextSkillTargetDuration>.Get();
            duration.CopyData(this);
            return duration;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            SetNextSkillTargetDuration duration = src as SetNextSkillTargetDuration;
            this.targetId = duration.targetId;
            this.nextSkillTargetID = duration.nextSkillTargetID;
            this.slotType = duration.slotType;
        }

        public override void Enter(Action _action, Track _track)
        {
            base.Enter(_action, _track);
            PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.targetId);
            if (actorHandle != 0)
            {
                this.nextSkillObj = _action.GetActorHandle(this.nextSkillTargetID);
                if (this.nextSkillObj != 0)
                {
                    SkillSlot skillSlot = actorHandle.handle.SkillControl.GetSkillSlot(this.slotType);
                    if (((skillSlot != null) && (this.nextSkillTargetID > 0)) && (skillSlot.NextSkillTargetIDs.IndexOf(this.nextSkillObj.handle.ObjID) < 0))
                    {
                        skillSlot.NextSkillTargetIDs.Add(this.nextSkillObj.handle.ObjID);
                    }
                }
            }
        }

        public override void Leave(Action _action, Track _track)
        {
            base.Leave(_action, _track);
            if (this.nextSkillObj != 0)
            {
                this.RemoveTarget(_action, this.nextSkillObj);
            }
        }

        public override void OnUse()
        {
            base.OnUse();
            this.targetId = -1;
            this.nextSkillTargetID = -1;
            this.slotType = SkillSlotType.SLOT_SKILL_0;
            this.nextSkillObj.Release();
        }

        public override void Process(Action _action, Track _track, int _localTime)
        {
            base.Process(_action, _track, _localTime);
            if ((this.nextSkillObj != 0) && this.nextSkillObj.handle.ActorControl.IsDeadState)
            {
                this.RemoveTarget(_action, this.nextSkillObj);
                this.nextSkillObj.Release();
            }
        }

        private void RemoveTarget(Action _action, PoolObjHandle<ActorRoot> nextObj)
        {
            PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.targetId);
            if (actorHandle != 0)
            {
                SkillSlot skillSlot = actorHandle.handle.SkillControl.GetSkillSlot(this.slotType);
                if (skillSlot != null)
                {
                    int index = skillSlot.NextSkillTargetIDs.IndexOf(nextObj.handle.ObjID);
                    if (index >= 0)
                    {
                        skillSlot.NextSkillTargetIDs.RemoveAt(index);
                    }
                }
            }
        }
    }
}

