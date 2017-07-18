namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using System;
    using UnityEngine;

    [EventCategory("MMGame/Skill")]
    internal class RotateActorDuration : DurationCondition
    {
        private PoolObjHandle<ActorRoot> actorTarget;
        private bool bNeedRotate;
        private int curRotateSpd;
        private VInt3 destDir = VInt3.zero;
        private int lastTime;
        public int rotateSpeed;
        [ObjectTemplate(new Type[] {  })]
        public int targetId;

        public void ActionRotateLerp(ActorRoot actor, uint nDelta)
        {
            if (((actor != null) && this.bNeedRotate) && (this.curRotateSpd != 0))
            {
                int degree = (int) (((nDelta * this.curRotateSpd) * 10) / 0x3e8);
                Quaternion quaternion = Quaternion.LookRotation((Vector3) actor.forward.RotateY(degree));
                actor.myTransform.set_rotation(Quaternion.RotateTowards(actor.myTransform.get_rotation(), quaternion, (Mathf.Abs(this.curRotateSpd) * nDelta) * 0.001f));
            }
        }

        public override BaseEvent Clone()
        {
            RotateActorDuration duration = ClassObjPool<RotateActorDuration>.Get();
            duration.CopyData(this);
            return duration;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            RotateActorDuration duration = src as RotateActorDuration;
            this.targetId = duration.targetId;
            this.rotateSpeed = duration.rotateSpeed;
            this.lastTime = duration.lastTime;
            this.actorTarget = duration.actorTarget;
            this.destDir = duration.destDir;
            this.bNeedRotate = duration.bNeedRotate;
            this.curRotateSpd = duration.curRotateSpd;
        }

        public override void Enter(Action _action, Track _track)
        {
            base.Enter(_action, _track);
            this.actorTarget = _action.GetActorHandle(this.targetId);
            this.lastTime = 0;
            if (this.actorTarget != 0)
            {
                this.bNeedRotate = false;
                this.curRotateSpd = 0;
                this.actorTarget.handle.ObjLinker.AddCustomRotateLerp(new CustomRotateLerpFunc(this.ActionRotateLerp));
            }
        }

        public override void Leave(Action _action, Track _track)
        {
            if (this.actorTarget != 0)
            {
                this.actorTarget.handle.ObjLinker.RmvCustomRotateLerp(new CustomRotateLerpFunc(this.ActionRotateLerp));
                this.curRotateSpd = 0;
                this.bNeedRotate = false;
            }
        }

        public override void OnUse()
        {
            base.OnUse();
            this.targetId = 0;
            this.rotateSpeed = 0;
            this.lastTime = 0;
            this.actorTarget.Release();
            this.destDir = VInt3.zero;
            this.bNeedRotate = false;
            this.curRotateSpd = 0;
        }

        public override void Process(Action _action, Track _track, int _localTime)
        {
            if (this.actorTarget != 0)
            {
                int num = _localTime - this.lastTime;
                this.lastTime = _localTime;
                if (this.actorTarget.handle.ActorControl.curMoveCommand != null)
                {
                    FrameCommand<MoveDirectionCommand> curMoveCommand = (FrameCommand<MoveDirectionCommand>) this.actorTarget.handle.ActorControl.curMoveCommand;
                    VInt3 forward = this.actorTarget.handle.forward;
                    this.destDir = VInt3.right.RotateY(curMoveCommand.cmdData.Degree);
                    if (this.destDir != forward)
                    {
                        this.bNeedRotate = true;
                        this.curRotateSpd = this.rotateSpeed;
                        int num3 = (this.destDir.x * forward.z) - (forward.x * this.destDir.z);
                        if (num3 == 0)
                        {
                            if (VInt3.Dot(this.destDir, forward) >= 0)
                            {
                                return;
                            }
                            this.curRotateSpd = this.rotateSpeed;
                        }
                        else if (num3 < 0)
                        {
                            this.curRotateSpd = -this.rotateSpeed;
                        }
                        VFactor radians = VInt3.AngleInt(this.destDir, forward);
                        VFactor factor2 = (VFactor) ((((VFactor.pi * num) * this.curRotateSpd) / 180L) / 0x3e8L);
                        if (radians <= factor2)
                        {
                            forward = forward.RotateY(ref radians);
                            this.bNeedRotate = false;
                        }
                        else
                        {
                            forward = forward.RotateY(ref factor2);
                        }
                        this.actorTarget.handle.MovementComponent.SetRotate(forward, true);
                    }
                }
                else
                {
                    this.destDir = this.actorTarget.handle.forward;
                    this.bNeedRotate = false;
                    this.curRotateSpd = 0;
                }
                base.Process(_action, _track, _localTime);
            }
        }
    }
}

