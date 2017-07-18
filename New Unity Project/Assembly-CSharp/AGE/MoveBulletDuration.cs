namespace AGE
{
    using Assets.Scripts.Common;
    using System;

    [EventCategory("MMGame/Skill")]
    public class MoveBulletDuration : DurationCondition
    {
        public int acceleration;
        public bool bAdjustSpeed;
        public bool bBulletUseDir;
        public bool bMoveRotate = true;
        public bool bReachDestStop = true;
        public bool bResetMoveDistance;
        public bool bUseIndicatorDir;
        private MoveBulletDurationContext Context = new MoveBulletDurationContext();
        [ObjectTemplate(new Type[] {  })]
        public int destId = -1;
        public int distance = 0xc350;
        public int gravity;
        public ActorMoveType MoveType;
        public VInt3 offsetDir = VInt3.zero;
        [ObjectTemplate(new Type[] {  })]
        public int targetId;
        public VInt3 targetPosition;
        public int velocity = 0x3a98;

        public override bool Check(Action _action, Track _track)
        {
            return this.Context.stopCondtion;
        }

        public override BaseEvent Clone()
        {
            MoveBulletDuration duration = ClassObjPool<MoveBulletDuration>.Get();
            duration.CopyData(this);
            return duration;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            MoveBulletDuration duration = src as MoveBulletDuration;
            this.targetId = duration.targetId;
            this.destId = duration.destId;
            this.MoveType = duration.MoveType;
            this.targetPosition = duration.targetPosition;
            this.offsetDir = duration.offsetDir;
            this.velocity = duration.velocity;
            this.distance = duration.distance;
            this.gravity = duration.gravity;
            this.bMoveRotate = duration.bMoveRotate;
            this.bAdjustSpeed = duration.bAdjustSpeed;
            this.acceleration = duration.acceleration;
            this.bBulletUseDir = duration.bBulletUseDir;
            this.bUseIndicatorDir = duration.bUseIndicatorDir;
            this.bReachDestStop = duration.bReachDestStop;
            this.bResetMoveDistance = duration.bResetMoveDistance;
            this.Context.CopyData(ref duration.Context);
        }

        public override void Enter(Action _action, Track _track)
        {
            this.Context.Reset(this);
            this.Context.Enter(_action);
            base.Enter(_action, _track);
        }

        public override void Leave(Action _action, Track _track)
        {
            base.Leave(_action, _track);
            this.Context.Leave(_action, _track);
        }

        public override void OnUse()
        {
            base.OnUse();
            this.offsetDir = VInt3.zero;
        }

        public override void Process(Action _action, Track _track, int _localTime)
        {
            this.Context.Process(_action, _track, _localTime);
            base.Process(_action, _track, _localTime);
        }

        public override bool SupportEditMode()
        {
            return true;
        }
    }
}

