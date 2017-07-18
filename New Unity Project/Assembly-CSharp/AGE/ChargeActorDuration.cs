namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using System;
    using UnityEngine;

    [EventCategory("MMGame/Skill")]
    public class ChargeActorDuration : DurationCondition
    {
        public int acceleration;
        private int curLerpSpeed;
        private int curSpeed;
        private VInt3 dir = VInt3.zero;
        private bool done_;
        public int lastDistance;
        private int lastTime_;
        public int maxMoveSpeed;
        public int moveSpeed;
        private PoolObjHandle<ActorRoot> targetActor;
        [ObjectTemplate(new Type[] {  })]
        public int targetID = -1;
        private PoolObjHandle<ActorRoot> triggerActor;
        [ObjectTemplate(new Type[] {  })]
        public int triggerID = -1;

        private void ActionMoveLerp(ActorRoot actor, uint nDelta, bool bReset)
        {
            if (actor != null)
            {
                if (this.done_)
                {
                    actor.myTransform.set_position((Vector3) actor.location);
                }
                else
                {
                    VInt3 dir = this.dir;
                    long num4 = (this.curLerpSpeed * nDelta) + ((long) ((((this.acceleration * nDelta) * nDelta) / ((ulong) 2L)) / ((ulong) 0x3e8L)));
                    this.curLerpSpeed = Mathf.Min(this.curLerpSpeed + ((int) ((this.acceleration * nDelta) / ((ulong) 0x3e8L))), this.maxMoveSpeed);
                    num4 /= 0x3e8L;
                    VInt3 num3 = dir.NormalizeTo((int) num4);
                    actor.myTransform.set_position(actor.myTransform.get_position() + ((Vector3) num3));
                }
            }
        }

        public override bool Check(Action _action, Track _track)
        {
            return this.done_;
        }

        public override BaseEvent Clone()
        {
            ChargeActorDuration duration = ClassObjPool<ChargeActorDuration>.Get();
            duration.CopyData(this);
            return duration;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            ChargeActorDuration duration = src as ChargeActorDuration;
            this.triggerID = duration.triggerID;
            this.targetID = duration.targetID;
            this.moveSpeed = duration.moveSpeed;
            this.lastDistance = duration.lastDistance;
            this.maxMoveSpeed = duration.maxMoveSpeed;
            this.acceleration = duration.acceleration;
        }

        public override void Enter(Action _action, Track _track)
        {
            base.Enter(_action, _track);
            this.lastTime_ = 0;
            this.done_ = false;
            this.triggerActor = _action.GetActorHandle(this.triggerID);
            this.targetActor = _action.GetActorHandle(this.targetID);
            if (((this.triggerActor != 0) && (this.targetActor != 0)) && ((this.moveSpeed != 0) || (this.acceleration != 0)))
            {
                this.curSpeed = this.moveSpeed;
                this.curLerpSpeed = this.moveSpeed;
                this.triggerActor.handle.ActorControl.AddNoAbilityFlag(ObjAbilityType.ObjAbility_Move);
                this.triggerActor.handle.ObjLinker.AddCustomMoveLerp(new CustomMoveLerpFunc(this.ActionMoveLerp));
                this.dir = this.targetActor.handle.location - this.triggerActor.handle.location;
            }
        }

        public override void Leave(Action _action, Track _track)
        {
            base.Leave(_action, _track);
            if (((this.triggerActor != 0) && (this.targetActor != 0)) && (this.moveSpeed != 0))
            {
                this.done_ = true;
                this.triggerActor.handle.ActorControl.RmvNoAbilityFlag(ObjAbilityType.ObjAbility_Move);
                this.triggerActor.handle.ObjLinker.RmvCustomMoveLerp(new CustomMoveLerpFunc(this.ActionMoveLerp));
                if (!PathfindingUtility.IsValidTarget(this.triggerActor.handle, this.triggerActor.handle.location))
                {
                    bool bResult = false;
                    VInt3 pos = PathfindingUtility.FindValidTarget(this.triggerActor.handle, this.targetActor.handle.location, this.triggerActor.handle.location, 0x2710, out bResult);
                    VInt groundY = 0;
                    PathfindingUtility.GetGroundY(pos, out groundY);
                    pos.y = groundY.i;
                    this.triggerActor.handle.location = pos;
                }
            }
        }

        public override void OnUse()
        {
            base.OnUse();
            this.triggerID = -1;
            this.targetID = -1;
            this.moveSpeed = 0;
            this.lastDistance = 0;
            this.curSpeed = 0;
            this.curLerpSpeed = 0;
            this.triggerActor.Release();
            this.targetActor.Release();
        }

        public override void Process(Action _action, Track _track, int _localTime)
        {
            base.Process(_action, _track, _localTime);
            if ((((this.triggerActor != 0) && (this.targetActor != 0)) && ((this.moveSpeed != 0) || (this.acceleration != 0))) && !this.done_)
            {
                VInt3 location = this.triggerActor.handle.location;
                VInt3 target = this.targetActor.handle.location;
                int num3 = _localTime - this.lastTime_;
                this.lastTime_ = _localTime;
                VInt3 num4 = target - location;
                long magnitude = (this.curSpeed * num3) + ((((this.acceleration * num3) * num3) / 2L) / 0x3e8L);
                this.curSpeed = Mathf.Min(this.curSpeed + ((this.acceleration * num3) / 0x3e8), this.maxMoveSpeed);
                this.curLerpSpeed = this.curSpeed;
                magnitude /= 0x3e8L;
                if (magnitude >= (num4.magnitude - this.lastDistance))
                {
                    magnitude = Mathf.Max(num4.magnitude - this.lastDistance, 0);
                    this.done_ = true;
                    target = location + num4.NormalizeTo((int) magnitude);
                    if (!PathfindingUtility.IsValidTarget(this.triggerActor.handle, target))
                    {
                        bool bResult = false;
                        VInt3 pos = PathfindingUtility.FindValidTarget(this.triggerActor.handle, target, this.triggerActor.handle.location, 0x2710, out bResult);
                        VInt groundY = 0;
                        PathfindingUtility.GetGroundY(pos, out groundY);
                        pos.y = groundY.i;
                        target = pos;
                        num4 = target - this.triggerActor.handle.location;
                        magnitude = num4.magnitude;
                    }
                }
                this.dir = num4;
                if (num4 != VInt3.zero)
                {
                    this.triggerActor.handle.rotation = Quaternion.LookRotation((Vector3) num4);
                }
                ActorRoot handle = this.triggerActor.handle;
                handle.location += num4.NormalizeTo((int) magnitude);
            }
        }
    }
}

