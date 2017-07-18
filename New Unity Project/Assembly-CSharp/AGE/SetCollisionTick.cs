namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using System;

    [EventCategory("MMGame/Skill")]
    public class SetCollisionTick : TickEvent
    {
        private PoolObjHandle<ActorRoot> actor;
        public int Degree = 160;
        public int Height = 500;
        public VInt3 Pos = VInt3.zero;
        public int Radius = 0x3e8;
        public int RadiusGrowthValue;
        public int Rotation;
        public int SectorRadius = 0x3e8;
        public VInt3 Size = VInt3.one;
        public VInt3 SizeGrowthValue = VInt3.zero;
        [ObjectTemplate(new Type[] {  })]
        public int targetId = -1;
        public ColliderType type;

        public override BaseEvent Clone()
        {
            SetCollisionTick tick = ClassObjPool<SetCollisionTick>.Get();
            tick.CopyData(this);
            return tick;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            SetCollisionTick tick = src as SetCollisionTick;
            this.targetId = tick.targetId;
            this.type = tick.type;
            this.Pos = tick.Pos;
            this.Size = tick.Size;
            this.Radius = tick.Radius;
            this.SectorRadius = tick.SectorRadius;
            this.Degree = tick.Degree;
            this.Rotation = tick.Rotation;
            this.Height = tick.Height;
            this.SizeGrowthValue = tick.SizeGrowthValue;
            this.RadiusGrowthValue = tick.RadiusGrowthValue;
        }

        private static T GetCollisionShape<T>(ActorRoot actorRoot, CollisionShapeType shapeType) where T: VCollisionShape, new()
        {
            if ((actorRoot.shape != null) && (actorRoot.shape.GetShapeType() == shapeType))
            {
                return (actorRoot.shape as T);
            }
            T local = Activator.CreateInstance<T>();
            local.Born(actorRoot);
            return local;
        }

        public override void OnRelease()
        {
            base.OnRelease();
            this.actor.Release();
        }

        public override void OnUse()
        {
            base.OnUse();
            this.targetId = -1;
            this.type = ColliderType.Box;
            this.Pos = VInt3.zero;
            this.Size = VInt3.one;
            this.Radius = 0x3e8;
            this.SectorRadius = 0x3e8;
            this.Degree = 160;
            this.Rotation = 0;
            this.Height = 500;
            this.SizeGrowthValue = VInt3.zero;
            this.RadiusGrowthValue = 0;
        }

        public override void Process(Action _action, Track _track)
        {
            this.actor = _action.GetActorHandle(this.targetId);
            if (this.actor == 0)
            {
                if (ActionManager.Instance.isPrintLog)
                {
                }
            }
            else
            {
                SkillSlot slot;
                ActorRoot actorRoot = this.actor.handle;
                SkillUseContext refParamObject = _action.refParams.GetRefParamObject<SkillUseContext>("SkillContext");
                PoolObjHandle<ActorRoot> originator = refParamObject.Originator;
                int num = 0;
                if (((originator != 0) && (refParamObject != null)) && (originator.handle.SkillControl.TryGetSkillSlot(refParamObject.SlotType, out slot) && (slot != null)))
                {
                    num = slot.GetSkillLevel() - 1;
                }
                if (num < 0)
                {
                    num = 0;
                }
                if (this.type == ColliderType.Box)
                {
                    VCollisionBox collisionShape = GetCollisionShape<VCollisionBox>(actorRoot, CollisionShapeType.Box);
                    collisionShape.Pos = this.Pos;
                    collisionShape.Size = new VInt3(this.Size.x + (this.SizeGrowthValue.x * num), this.Size.y + (this.SizeGrowthValue.y * num), this.Size.z + (this.SizeGrowthValue.z * num));
                    collisionShape.dirty = true;
                    collisionShape.ConditionalUpdateShape();
                }
                else if (this.type == ColliderType.Sphere)
                {
                    VCollisionSphere sphere = GetCollisionShape<VCollisionSphere>(actorRoot, CollisionShapeType.Sphere);
                    sphere.Pos = this.Pos;
                    sphere.Radius = this.Radius + (this.RadiusGrowthValue * num);
                    sphere.dirty = true;
                    sphere.ConditionalUpdateShape();
                }
                else if (this.type == ColliderType.CylinderSector)
                {
                    VCollisionCylinderSector sector = GetCollisionShape<VCollisionCylinderSector>(actorRoot, CollisionShapeType.CylinderSector);
                    sector.Pos = this.Pos;
                    sector.Radius = this.SectorRadius;
                    sector.Height = this.Height;
                    sector.Degree = this.Degree;
                    sector.Rotation = this.Rotation;
                    sector.dirty = true;
                    sector.ConditionalUpdateShape();
                }
            }
        }

        public override void ProcessBlend(Action _action, Track _track, TickEvent _prevEvent, float _blendWeight)
        {
        }

        public enum ColliderType
        {
            Box,
            Sphere,
            CylinderSector
        }
    }
}

