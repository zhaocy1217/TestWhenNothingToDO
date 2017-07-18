namespace AGE
{
    using Assets.Scripts.Common;
    using System;

    [EventCategory("MMGame/Skill")]
    public class ReviveTimerTick : TickEvent
    {
        [ObjectTemplate(new Type[] {  })]
        public int targetId;
        public int yOffset;

        public override BaseEvent Clone()
        {
            ReviveTimerTick tick = ClassObjPool<ReviveTimerTick>.Get();
            tick.CopyData(this);
            return tick;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            ReviveTimerTick tick = src as ReviveTimerTick;
            this.targetId = tick.targetId;
            this.yOffset = tick.yOffset;
        }

        public override void OnUse()
        {
            base.OnUse();
            this.targetId = 0;
            this.yOffset = 0;
        }

        public override void Process(Action _action, Track _track)
        {
            PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.targetId);
            if (((actorHandle != 0) && (actorHandle.handle.HudControl != null)) && ((actorHandle.handle.ActorControl != null) && actorHandle.handle.ActorControl.IsDeadState))
            {
                int reviveTotalTime = actorHandle.handle.ActorControl.GetReviveTotalTime();
                actorHandle.handle.HudControl.ShowReviveTimer(this.yOffset, reviveTotalTime);
            }
        }

        public override bool SupportEditMode()
        {
            return true;
        }
    }
}

