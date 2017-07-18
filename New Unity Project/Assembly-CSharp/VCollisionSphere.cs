using Assets.Scripts.GameSystem;
using CSProtocol;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

[Serializable]
public class VCollisionSphere : VCollisionShape
{
    [HideInInspector, SerializeField]
    private VInt3 localPos = VInt3.zero;
    [SerializeField, HideInInspector]
    private int localRadius = 500;
    private VInt3 worldPos = VInt3.zero;
    private int worldRadius = 500;

    public VCollisionSphere()
    {
        base.dirty = true;
    }

    public override bool AcceptFowVisibilityCheck(COM_PLAYERCAMP inHostCamp, GameFowManager fowMgr)
    {
        return GameFowCollector.VisitFowVisibilityCheck(this, base.owner, inHostCamp, fowMgr);
    }

    public override bool EdgeIntersects(VCollisionBox s)
    {
        return false;
    }

    public override bool EdgeIntersects(VCollisionCylinderSector cs)
    {
        return cs.EdgeIntersects(this);
    }

    public override bool EdgeIntersects(VCollisionSphere s)
    {
        base.ConditionalUpdateShape();
        s.ConditionalUpdateShape();
        long num = this.worldRadius + s.worldRadius;
        long num2 = this.worldRadius - s.worldRadius;
        VInt3 num4 = this.worldPos - s.worldPos;
        long num3 = num4.sqrMagnitudeLong2D;
        return ((num3 <= (num * num)) && (num3 >= (num2 * num2)));
    }

    public override void GetAabb2D(out VInt2 origin, out VInt2 size)
    {
        origin = this.WorldPos.xz;
        origin.x -= this.localRadius;
        origin.y -= this.localRadius;
        size.x = this.localRadius + this.localRadius;
        size.y = size.x;
    }

    public override CollisionShapeType GetShapeType()
    {
        return CollisionShapeType.Sphere;
    }

    public override bool Intersects(VCollisionBox obb)
    {
        return obb.Intersects(this);
    }

    public override bool Intersects(VCollisionCylinderSector cs)
    {
        return cs.Intersects(this);
    }

    public override bool Intersects(VCollisionSphere s)
    {
        base.ConditionalUpdateShape();
        s.ConditionalUpdateShape();
        long num = this.worldRadius + s.worldRadius;
        VInt3 num2 = this.worldPos - s.worldPos;
        return (num2.sqrMagnitudeLong <= (num * num));
    }

    public static void UpdatePosition(ref VInt3 worldPos, ref VInt3 localPos, ref VInt3 location, ref VInt3 forward)
    {
        if ((localPos.x == 0) && (localPos.z == 0))
        {
            worldPos.x = localPos.x + location.x;
            worldPos.y = localPos.y + location.y;
            worldPos.z = localPos.z + location.z;
        }
        else
        {
            VInt3 up = VInt3.up;
            VInt3 rhs = forward;
            VInt3 num3 = VInt3.Cross(ref up, ref rhs);
            VInt3 trans = location;
            worldPos = IntMath.Transform(ref localPos, ref num3, ref up, ref rhs, ref trans);
        }
    }

    public override void UpdateShape(VInt3 location, VInt3 forward)
    {
        UpdatePosition(ref this.worldPos, ref this.localPos, ref location, ref forward);
        this.worldRadius = this.localRadius;
        base.dirty = false;
    }

    public override void UpdateShape(VInt3 location, VInt3 forward, int moveDelta)
    {
    }

    public override int AvgCollisionRadius
    {
        get
        {
            return this.WorldRadius;
        }
    }

    [CollisionProperty]
    public VInt3 Pos
    {
        get
        {
            return this.localPos;
        }
        set
        {
            this.localPos = value;
            base.dirty = false;
        }
    }

    [CollisionProperty]
    public int Radius
    {
        get
        {
            return this.localRadius;
        }
        set
        {
            this.localRadius = value;
            base.dirty = true;
        }
    }

    public VInt3 WorldPos
    {
        get
        {
            base.ConditionalUpdateShape();
            return this.worldPos;
        }
    }

    public int WorldRadius
    {
        get
        {
            base.ConditionalUpdateShape();
            return this.worldRadius;
        }
    }
}

