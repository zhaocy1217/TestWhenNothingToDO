using UnityEngine;

public class SCollisionComponent : MonoBehaviour
{
    [HideInInspector, SerializeField]
    public VInt3 Pos = VInt3.zero;
    public CollisionShapeType shapeType = CollisionShapeType.Sphere;
    [HideInInspector, SerializeField]
    public VInt3 Size = new VInt3(500, 500, 500);
    [SerializeField, HideInInspector]
    public VInt3 Size2 = new VInt3(0, 0, 0);

    public VCollisionShape CreateShape()
    {
        DebugHelper.Assert((!Singleton<BattleLogic>.instance.isFighting || Singleton<GameLogic>.instance.bInLogicTick) || Singleton<FrameSynchr>.instance.isCmdExecuting);
        VCollisionShape shape = null;
        switch (this.shapeType)
        {
            case CollisionShapeType.Box:
            {
                VCollisionBox box = new VCollisionBox();
                box.Size = this.Size;
                box.Pos = this.Pos;
                return box;
            }
            case CollisionShapeType.Sphere:
            {
                VCollisionSphere sphere = new VCollisionSphere();
                sphere.Pos = this.Pos;
                sphere.Radius = this.Size.x;
                return sphere;
            }
            case CollisionShapeType.CylinderSector:
            {
                VCollisionCylinderSector sector = new VCollisionCylinderSector();
                sector.Pos = this.Pos;
                sector.Radius = this.Size.x;
                sector.Height = this.Size.y;
                sector.Degree = this.Size.z;
                sector.Rotation = this.Size2.x;
                return shape;
            }
        }
        return shape;
    }
}

