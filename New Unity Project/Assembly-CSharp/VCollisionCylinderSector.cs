using Assets.Scripts.GameSystem;
using CSProtocol;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

[Serializable]
public class VCollisionCylinderSector : VCollisionShape
{
    [SerializeField, HideInInspector]
    private int degree = 90;
    [SerializeField, HideInInspector]
    private int height = 500;
    private VInt3 leftDir = VInt3.forward;
    [SerializeField, HideInInspector]
    private VInt3 localPos = VInt3.zero;
    [SerializeField, HideInInspector]
    private int radius = 500;
    private VInt3 rightDir = VInt3.forward;
    [HideInInspector, SerializeField]
    private int rotation;
    private VInt3 worldPos = VInt3.zero;

    public VCollisionCylinderSector()
    {
        base.dirty = true;
    }

    public override bool AcceptFowVisibilityCheck(COM_PLAYERCAMP inHostCamp, GameFowManager fowMgr)
    {
        return GameFowCollector.VisitFowVisibilityCheck(this, base.owner, inHostCamp, fowMgr);
    }

    private static int CalcSide(ref VInt3 point, ref VInt3 lineStart, ref VInt3 lineDir)
    {
        return ((lineDir.x * (point.z - lineStart.z)) - ((point.x - lineStart.x) * lineDir.z));
    }

    private static VInt3 ClosestPoint(ref VInt3 point, ref VInt3 lineStart, ref VInt3 lineDir, int lineLen)
    {
        long m = IntMath.Clamp(VInt3.DotXZLong((VInt3) (point - lineStart), (VInt3) lineDir), 0L, (long) (lineLen * 0x3e8));
        return (IntMath.Divide(lineDir, m, 0xf4240L) + lineStart);
    }

    public override bool EdgeIntersects(VCollisionBox s)
    {
        return false;
    }

    public override bool EdgeIntersects(VCollisionCylinderSector s)
    {
        return false;
    }

    public override bool EdgeIntersects(VCollisionSphere s)
    {
        return this.Intersects(s);
    }

    public override void GetAabb2D(out VInt2 origin, out VInt2 size)
    {
        origin = this.worldPos.xz;
        origin.x -= this.radius;
        origin.y -= this.radius;
        size.x = this.radius + this.radius;
        size.y = size.x;
    }

    public override CollisionShapeType GetShapeType()
    {
        return CollisionShapeType.CylinderSector;
    }

    public override bool Intersects(VCollisionBox obb)
    {
        return false;
    }

    public override bool Intersects(VCollisionCylinderSector s)
    {
        return false;
    }

    public override bool Intersects(VCollisionSphere s)
    {
        base.ConditionalUpdateShape();
        s.ConditionalUpdateShape();
        VInt3 worldPos = s.WorldPos;
        int worldRadius = s.WorldRadius;
        int num3 = this.height >> 1;
        int num4 = this.worldPos.y - num3;
        int num5 = this.worldPos.y + num3;
        if (((worldPos.y + worldRadius) > num4) && ((worldPos.y - worldRadius) < num5))
        {
            int num6 = worldRadius;
            if ((worldPos.y > num5) || (worldPos.y < num4))
            {
                int num7 = (worldPos.y <= num5) ? (num4 - worldPos.y) : (worldPos.y - num5);
                int num8 = (worldRadius * worldRadius) - (num7 * num7);
                DebugHelper.Assert(num8 >= 0);
                num6 = IntMath.Sqrt((long) num8);
            }
            long num9 = num6 + this.radius;
            VInt3 num12 = this.worldPos - worldPos;
            if (num12.sqrMagnitudeLong2D >= (num9 * num9))
            {
                return false;
            }
            int num10 = worldRadius * worldRadius;
            VInt3 num13 = ClosestPoint(ref worldPos, ref this.worldPos, ref this.leftDir, this.radius) - worldPos;
            if (num13.sqrMagnitudeLong2D <= num10)
            {
                return true;
            }
            VInt3 num14 = ClosestPoint(ref worldPos, ref this.worldPos, ref this.rightDir, this.radius) - worldPos;
            if (num14.sqrMagnitudeLong2D <= num10)
            {
                return true;
            }
            if (this.degree <= 180)
            {
                if ((CalcSide(ref worldPos, ref this.worldPos, ref this.leftDir) <= 0) && (CalcSide(ref worldPos, ref this.worldPos, ref this.rightDir) >= 0))
                {
                    return true;
                }
            }
            else if ((CalcSide(ref worldPos, ref this.worldPos, ref this.leftDir) <= 0) || (CalcSide(ref worldPos, ref this.worldPos, ref this.rightDir) >= 0))
            {
                return true;
            }
        }
        return false;
    }

    public override void UpdateShape(VInt3 location, VInt3 forward)
    {
        VFactor factor;
        VFactor factor2;
        VCollisionSphere.UpdatePosition(ref this.worldPos, ref this.localPos, ref location, ref forward);
        if (this.rotation != 0)
        {
            forward = forward.RotateY(this.rotation);
        }
        IntMath.sincos(out factor, out factor2, (long) (0x13a * Mathf.Clamp(this.degree, 1, 360)), 0x8ca0L);
        long num = factor2.nom * factor.den;
        long num2 = factor2.den * factor.nom;
        long b = factor2.den * factor.den;
        this.rightDir.x = (int) IntMath.Divide((long) ((forward.x * num) + (forward.z * num2)), b);
        this.rightDir.z = (int) IntMath.Divide((long) ((-forward.x * num2) + (forward.z * num)), b);
        this.rightDir.y = 0;
        num2 = -num2;
        this.leftDir.x = (int) IntMath.Divide((long) ((forward.x * num) + (forward.z * num2)), b);
        this.leftDir.z = (int) IntMath.Divide((long) ((-forward.x * num2) + (forward.z * num)), b);
        this.leftDir.y = 0;
        this.rightDir.Normalize();
        this.leftDir.Normalize();
        base.dirty = false;
    }

    public override void UpdateShape(VInt3 location, VInt3 forward, int moveDelta)
    {
    }

    public override int AvgCollisionRadius
    {
        get
        {
            return this.radius;
        }
    }

    [CollisionProperty]
    public int Degree
    {
        get
        {
            return this.degree;
        }
        set
        {
            this.degree = Mathf.Clamp(value, 1, 360);
            base.dirty = true;
        }
    }

    [CollisionProperty]
    public int Height
    {
        get
        {
            return this.height;
        }
        set
        {
            this.height = Mathf.Max(0, value);
            base.dirty = true;
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
            return this.radius;
        }
        set
        {
            this.radius = value;
            base.dirty = true;
        }
    }

    [CollisionProperty]
    public int Rotation
    {
        get
        {
            return this.rotation;
        }
        set
        {
            this.rotation = value;
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
}

