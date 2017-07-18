namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using System;

    public class ActorRootSlot
    {
        private PoolObjHandle<ActorRoot> childActorRoot;
        private VInt distance;
        private VInt3 prePosition;
        private VInt3 translation;

        public ActorRootSlot(PoolObjHandle<ActorRoot> _child, VInt3 _parentPos)
        {
            this.distance = 0;
            this.prePosition = VInt3.zero;
            this.translation = VInt3.zero;
            this.childActorRoot = _child;
            this.prePosition = _parentPos;
        }

        public ActorRootSlot(PoolObjHandle<ActorRoot> _child, VInt3 _parentPos, VInt3 _trans)
        {
            this.distance = 0;
            this.prePosition = VInt3.zero;
            this.translation = VInt3.zero;
            this.translation = _trans;
            this.prePosition = _parentPos;
            this.distance = this.translation.magnitude;
            this.childActorRoot = _child;
        }

        public void Update(ActorRoot _parent)
        {
            if (this.childActorRoot != 0)
            {
                VInt3 num = _parent.location + this.translation;
                if ((this.translation.x != 0) || (this.translation.z != 0))
                {
                    VInt3 forward = VInt3.forward;
                    VFactor radians = VInt3.AngleInt(_parent.forward, forward);
                    int num4 = (_parent.forward.x * forward.z) - (forward.x * _parent.forward.z);
                    if (num4 < 0)
                    {
                        radians = VFactor.twoPi - radians;
                    }
                    VInt3 num2 = this.translation.RotateY(ref radians);
                    num = _parent.location + num2.NormalizeTo(this.distance.i);
                    num.y += this.translation.y;
                }
                this.childActorRoot.handle.location = num;
                this.childActorRoot.handle.forward = _parent.forward;
                this.UpdateMoveDelta(num);
            }
        }

        private void UpdateMoveDelta(VInt3 _newPos)
        {
            if (this.childActorRoot.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Bullet)
            {
                BulletWrapper actorControl = this.childActorRoot.handle.ActorControl as BulletWrapper;
                if (((actorControl != null) && actorControl.GetMoveCollisiong()) && (this.prePosition != _newPos))
                {
                    VInt3 num = _newPos - this.prePosition;
                    actorControl.SetMoveDelta(num.magnitude2D);
                    this.prePosition = _newPos;
                }
            }
        }
    }
}

