namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using System;
    using UnityEngine;

    [EventCategory("MMGame/Skill")]
    internal class MoveActorDuration : DurationCondition
    {
        public int acceleration;
        private PoolObjHandle<ActorRoot> actor_;
        [ObjectTemplate(new Type[] {  })]
        public int actorId;
        public bool bForbidMoveFollowing;
        public bool bRecordPosition;
        public bool bUseRecordPosition;
        [ObjectTemplate(new Type[] {  })]
        public int destId = -1;
        public VInt3 destPos = VInt3.zero;
        private VInt3 dir = VInt3.zero;
        private bool done_;
        public bool enableRotate = true;
        private VInt3 finalPos = VInt3.zero;
        private Quaternion fromRot = Quaternion.get_identity();
        public bool IgnoreCollision;
        private int lastLerpMoveSpeed;
        private int lastMoveSpeed;
        private int lastTime_;
        public int minMoveDistance;
        public VInt3 moveDir = VInt3.zero;
        public int moveDistance;
        public int moveSpeed;
        private int moveTick;
        public ActorMoveType moveType;
        public int rotationTime;
        private VInt3 srcPos = VInt3.zero;
        public bool teleport;
        private Quaternion toRot = Quaternion.get_identity();

        private void ActionMoveLerp(ActorRoot actor, uint nDelta, bool bReset)
        {
            if (actor != null)
            {
                if (this.done_ || this.teleport)
                {
                    actor.myTransform.set_position((Vector3) actor.location);
                }
                else
                {
                    VInt3 num3;
                    VInt groundY = 0;
                    VInt3 dir = this.dir;
                    if (!this.shouldUseAcceleration)
                    {
                        num3 = dir.NormalizeTo((int) ((this.moveSpeed * nDelta) / 0x3e8));
                    }
                    else
                    {
                        long num4 = (this.lastLerpMoveSpeed * nDelta) + ((long) ((((this.acceleration * nDelta) * nDelta) / ((ulong) 2L)) / ((ulong) 0x3e8L)));
                        num4 /= 0x3e8L;
                        num3 = dir.NormalizeTo((int) num4);
                        this.lastLerpMoveSpeed += ((int) (this.acceleration * nDelta)) / 0x3e8;
                    }
                    Vector3 vector = actor.myTransform.get_position();
                    if (!this.IgnoreCollision)
                    {
                        num3 = PathfindingUtility.MoveLerp(actor, (VInt3) vector, num3, out groundY);
                    }
                    if (actor.MovementComponent.isFlying)
                    {
                        float y = vector.y;
                        actor.myTransform.set_position(actor.myTransform.get_position() + ((Vector3) num3));
                        Vector3 vector2 = actor.myTransform.get_position();
                        vector2.y = y;
                        actor.myTransform.set_position(vector2);
                    }
                    else
                    {
                        actor.myTransform.set_position(actor.myTransform.get_position() + ((Vector3) num3));
                    }
                }
            }
        }

        public override bool Check(Action _action, Track _track)
        {
            return this.done_;
        }

        public override BaseEvent Clone()
        {
            MoveActorDuration duration = ClassObjPool<MoveActorDuration>.Get();
            duration.CopyData(this);
            return duration;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            MoveActorDuration duration = src as MoveActorDuration;
            this.destId = duration.destId;
            this.actorId = duration.actorId;
            this.destPos = duration.destPos;
            this.moveDir = duration.moveDir;
            this.moveType = duration.moveType;
            this.moveDistance = duration.moveDistance;
            this.minMoveDistance = duration.minMoveDistance;
            this.moveSpeed = duration.moveSpeed;
            this.acceleration = duration.acceleration;
            this.lastMoveSpeed = duration.lastMoveSpeed;
            this.lastLerpMoveSpeed = duration.lastLerpMoveSpeed;
            this.enableRotate = duration.enableRotate;
            this.rotationTime = duration.rotationTime;
            this.teleport = duration.teleport;
            this.IgnoreCollision = duration.IgnoreCollision;
            this.bRecordPosition = duration.bRecordPosition;
            this.bUseRecordPosition = duration.bUseRecordPosition;
            this.bForbidMoveFollowing = duration.bForbidMoveFollowing;
            this.done_ = duration.done_;
            this.fromRot = duration.fromRot;
            this.toRot = duration.toRot;
            this.dir = duration.dir;
            this.moveTick = duration.moveTick;
            this.lastTime_ = duration.lastTime_;
            this.actor_ = duration.actor_;
        }

        public override void Enter(Action _action, Track _track)
        {
            this.done_ = false;
            this.lastTime_ = 0;
            base.Enter(_action, _track);
            this.actor_ = _action.GetActorHandle(this.actorId);
            if ((this.actor_ != 0) && ((this.teleport || (this.moveSpeed != 0)) || (this.acceleration != 0)))
            {
                this.srcPos = this.actor_.handle.location;
                if (this.bForbidMoveFollowing)
                {
                    this.actor_.handle.ActorControl.AddNoAbilityFlag(ObjAbilityType.ObjAbility_MoveProtect);
                }
                if (this.bUseRecordPosition && (this.actor_.handle.SkillControl != null))
                {
                    this.destPos = this.actor_.handle.SkillControl.RecordPosition;
                    this.dir = this.destPos - this.srcPos;
                    VInt3 num10 = this.destPos - this.srcPos;
                    int num2 = num10.magnitude2D;
                    this.moveDistance += num2;
                }
                else if (this.moveType == ActorMoveType.Target)
                {
                    int num3;
                    PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.destId);
                    if ((this.actor_ == 0) || (actorHandle == 0))
                    {
                        this.actor_.Release();
                        return;
                    }
                    if (actorHandle.handle.ActorControl.GetNoAbilityFlag(ObjAbilityType.ObjAbility_MoveProtect) && (actorHandle.handle.TheActorMeta.ActorCamp != this.actor_.handle.TheActorMeta.ActorCamp))
                    {
                        num3 = 0;
                    }
                    else
                    {
                        this.dir = actorHandle.handle.location - this.srcPos;
                        VInt3 num11 = actorHandle.handle.location - this.srcPos;
                        num3 = num11.magnitude2D;
                    }
                    this.moveDistance += num3;
                }
                else if (this.moveType == ActorMoveType.Position)
                {
                    this.dir = this.destPos - this.srcPos;
                    VInt3 num12 = this.destPos - this.srcPos;
                    int minMoveDistance = num12.magnitude2D;
                    if (minMoveDistance < this.minMoveDistance)
                    {
                        minMoveDistance = this.minMoveDistance;
                    }
                    this.moveDistance += minMoveDistance;
                }
                else if (this.moveType == ActorMoveType.Directional)
                {
                    this.dir = this.actor_.handle.forward;
                }
                this.dir.y = 0;
                this.actor_.handle.ActorControl.TerminateMove();
                if (this.bRecordPosition && (this.actor_.handle.SkillControl != null))
                {
                    this.actor_.handle.SkillControl.RecordPosition = this.actor_.handle.location;
                }
                if (!this.actor_.handle.isMovable || (this.dir.sqrMagnitudeLong <= 1L))
                {
                    this.actor_.Release();
                    this.done_ = true;
                }
                else
                {
                    VInt3 dir = this.dir;
                    if (!this.bUseRecordPosition)
                    {
                        this.finalPos = this.srcPos + dir.NormalizeTo(this.moveDistance);
                    }
                    else
                    {
                        this.finalPos = this.actor_.handle.SkillControl.RecordPosition;
                        if (this.teleport)
                        {
                            this.actor_.handle.ActorControl.AddNoAbilityFlag(ObjAbilityType.ObjAbility_MoveProtect);
                        }
                    }
                    if (!PathfindingUtility.IsValidTarget((ActorRoot) this.actor_, this.finalPos))
                    {
                        bool bResult = false;
                        if (this.moveType == ActorMoveType.Directional)
                        {
                            this.IgnoreCollision = false;
                        }
                        else
                        {
                            VInt3 pos = PathfindingUtility.FindValidTarget((ActorRoot) this.actor_, this.finalPos, this.actor_.handle.location, 0x2710, out bResult);
                            if (!bResult)
                            {
                                this.IgnoreCollision = false;
                            }
                            else
                            {
                                VInt groundY = 0;
                                PathfindingUtility.GetGroundY(pos, out groundY);
                                pos.y = groundY.i;
                                this.finalPos = pos;
                                VInt3 num13 = this.finalPos - this.actor_.handle.location;
                                this.moveDistance = num13.magnitude2D;
                            }
                        }
                    }
                    if (!this.shouldUseAcceleration)
                    {
                        this.moveTick = (this.moveDistance * 0x3e8) / this.moveSpeed;
                    }
                    else
                    {
                        long moveDistance = this.moveDistance;
                        long acceleration = this.acceleration;
                        long moveSpeed = this.moveSpeed;
                        this.moveTick = (int) IntMath.Divide((long) ((IntMath.Sqrt((moveSpeed * moveSpeed) + ((2L * acceleration) * moveDistance)) - moveSpeed) * 0x3e8L), acceleration);
                        this.lastMoveSpeed = this.moveSpeed;
                        this.lastLerpMoveSpeed = this.moveSpeed;
                    }
                    this.actor_.handle.ActorControl.AddNoAbilityFlag(ObjAbilityType.ObjAbility_Move);
                    this.actor_.handle.ObjLinker.AddCustomMoveLerp(new CustomMoveLerpFunc(this.ActionMoveLerp));
                    this.fromRot = this.actor_.handle.rotation;
                    this.actor_.handle.MovementComponent.SetRotate(this.dir, true);
                    if (this.rotationTime > 0)
                    {
                        this.toRot = Quaternion.LookRotation((Vector3) this.actor_.handle.forward);
                    }
                    else
                    {
                        this.actor_.handle.rotation = Quaternion.LookRotation((Vector3) this.actor_.handle.forward);
                    }
                }
            }
        }

        public override void Leave(Action _action, Track _track)
        {
            if (this.actor_ != 0)
            {
                if (this.bForbidMoveFollowing)
                {
                    this.actor_.handle.ActorControl.RmvNoAbilityFlag(ObjAbilityType.ObjAbility_MoveProtect);
                }
                if (this.IgnoreCollision)
                {
                    bool flag = false;
                    _action.refParams.GetRefParam("_HitTargetHero", ref flag);
                    if (!flag)
                    {
                        this.SetFinalPos();
                    }
                    else
                    {
                        VInt3 zero = VInt3.zero;
                        _action.refParams.GetRefParam("_HitTargetHeroPos", ref zero);
                        if (!PathfindingUtility.IsValidTarget((ActorRoot) this.actor_, zero))
                        {
                            this.SetFinalPos();
                        }
                        else
                        {
                            this.actor_.handle.location = zero;
                        }
                    }
                }
                this.actor_.handle.ObjLinker.RmvCustomMoveLerp(new CustomMoveLerpFunc(this.ActionMoveLerp));
                this.actor_.handle.ActorControl.RmvNoAbilityFlag(ObjAbilityType.ObjAbility_Move);
                this.done_ = true;
            }
        }

        public override void OnUse()
        {
            base.OnUse();
            this.destId = -1;
            this.actorId = 0;
            this.destPos = VInt3.zero;
            this.moveDir = VInt3.zero;
            this.moveType = ActorMoveType.Target;
            this.moveDistance = 0;
            this.moveSpeed = 0;
            this.acceleration = 0;
            this.enableRotate = true;
            this.rotationTime = 0;
            this.teleport = false;
            this.IgnoreCollision = false;
            this.done_ = false;
            this.fromRot = Quaternion.get_identity();
            this.toRot = Quaternion.get_identity();
            this.srcPos = VInt3.zero;
            this.finalPos = VInt3.zero;
            this.dir = VInt3.zero;
            this.moveTick = 0;
            this.lastTime_ = 0;
            this.actor_.Release();
            this.lastMoveSpeed = 0;
            this.lastLerpMoveSpeed = 0;
            this.bForbidMoveFollowing = false;
        }

        public override void Process(Action _action, Track _track, int _localTime)
        {
            if (!this.done_ && (this.actor_ != 0))
            {
                bool flag = this.lastTime_ < this.rotationTime;
                int moveTick = _localTime - this.lastTime_;
                this.lastTime_ = _localTime;
                if (this.teleport)
                {
                    this.Teleport();
                }
                else
                {
                    VInt3 num4;
                    if (flag)
                    {
                        float num2 = Mathf.Min(1f, (float) (_localTime / this.rotationTime));
                        Quaternion quaternion = Quaternion.Slerp(this.fromRot, this.toRot, num2);
                        this.actor_.handle.rotation = quaternion;
                    }
                    if ((this.moveTick - moveTick) <= 0)
                    {
                        moveTick = this.moveTick;
                        this.done_ = true;
                    }
                    else
                    {
                        this.moveTick -= moveTick;
                    }
                    VInt3 dir = this.dir;
                    if (!this.shouldUseAcceleration)
                    {
                        num4 = dir.NormalizeTo((this.moveSpeed * moveTick) / 0x3e8);
                    }
                    else
                    {
                        long num5 = (this.lastMoveSpeed * moveTick) + ((((this.acceleration * moveTick) * moveTick) / 2L) / 0x3e8L);
                        num5 /= 0x3e8L;
                        num4 = dir.NormalizeTo((int) num5);
                        this.lastMoveSpeed += (this.acceleration * moveTick) / 0x3e8;
                    }
                    VInt groundY = this.actor_.handle.groundY;
                    if (!this.IgnoreCollision)
                    {
                        num4 = PathfindingUtility.Move(this.actor_.handle, num4, out groundY, out this.actor_.handle.hasReachedNavEdge, null);
                    }
                    if (this.actor_.handle.MovementComponent.isFlying)
                    {
                        int y = this.actor_.handle.location.y;
                        ActorRoot handle = this.actor_.handle;
                        handle.location += num4;
                        VInt3 location = this.actor_.handle.location;
                        location.y = y;
                        this.actor_.handle.location = location;
                    }
                    else
                    {
                        ActorRoot local2 = this.actor_.handle;
                        local2.location += num4;
                    }
                    this.actor_.handle.groundY = groundY;
                    base.Process(_action, _track, _localTime);
                }
            }
        }

        private void SetFinalPos()
        {
            VInt groundY = 0;
            PathfindingUtility.GetGroundY(this.finalPos, out groundY);
            this.finalPos.y = groundY.i;
            this.actor_.handle.location = this.finalPos;
        }

        public void Teleport()
        {
            VInt groundY = this.actor_.handle.groundY;
            VInt3 delta = this.dir.NormalizeTo(this.moveDistance);
            VInt3 target = this.actor_.handle.location + delta;
            bool bResult = false;
            if (PathfindingUtility.IsValidTarget((ActorRoot) this.actor_, target))
            {
                PathfindingUtility.GetGroundY(target, out groundY);
                target.y = groundY.i;
                this.actor_.handle.location = target;
            }
            else
            {
                VInt3 pos = PathfindingUtility.FindValidTarget((ActorRoot) this.actor_, target, this.actor_.handle.location, 0x2710, out bResult);
                if (bResult)
                {
                    PathfindingUtility.GetGroundY(pos, out groundY);
                    pos.y = groundY.i;
                    this.actor_.handle.location = pos;
                }
                else
                {
                    delta = PathfindingUtility.Move(this.actor_.handle, delta, out groundY, out this.actor_.handle.hasReachedNavEdge, null);
                    ActorRoot handle = this.actor_.handle;
                    handle.location += delta;
                }
            }
            if (this.bUseRecordPosition)
            {
                this.actor_.handle.ActorControl.RmvNoAbilityFlag(ObjAbilityType.ObjAbility_MoveProtect);
            }
            this.actor_.handle.groundY = groundY;
            this.done_ = true;
        }

        public bool shouldUseAcceleration
        {
            get
            {
                return ((this.moveType == ActorMoveType.Directional) && (this.acceleration != 0));
            }
        }
    }
}

