namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Framework;
    using System;
    using UnityEngine;

    public class GeoPolygon : MonoBehaviour
    {
        private VInt3 _boundMax;
        private VInt3 _boundMin;
        private VInt2[] _vertecesXZ;
        public bool CheckInByBound;

        public VInt2 GetRandomPoint(int index)
        {
            VInt2 num;
            if (this._vertecesXZ.Length <= 1)
            {
                return VInt2.zero;
            }
            index = index % this._vertecesXZ.Length;
            if (index == 0)
            {
                num = this._vertecesXZ[this._vertecesXZ.Length - 1];
            }
            else
            {
                num = this._vertecesXZ[index - 1];
            }
            VInt2 num2 = this._vertecesXZ[index];
            VInt2 num3 = num2 - num;
            ushort nMax = 0x2710;
            int num6 = FrameRandom.Random(nMax);
            return (num + new VInt2((num3.x * num6) / nMax, (num3.y * num6) / nMax));
        }

        public bool IntersectSegment(ref VInt3 segSrc, ref VInt3 segVec, ref VInt3 nearPoint, ref VInt3 pointProj)
        {
            if (((this._vertecesXZ != null) && (this._vertecesXZ.Length >= 2)) && (this.IsInBoundXZ(segSrc) || this.IsInBoundXZ(segSrc + segVec)))
            {
                VInt2 num3;
                VInt2 num4;
                VInt2 xz = segSrc.xz;
                VInt2 num2 = segVec.xz;
                if (IntMath.SegIntersectPlg(ref xz, ref num2, this._vertecesXZ, out num3, out num4))
                {
                    nearPoint.x = num3.x;
                    nearPoint.z = num3.y;
                    pointProj.x = num4.x;
                    pointProj.z = num4.y;
                    return true;
                }
            }
            return false;
        }

        public bool IsInBoundXZ(VInt3 p)
        {
            return ((((this._boundMin.x <= p.x) && (p.x <= this._boundMax.x)) && (this._boundMin.z <= p.z)) && (p.z <= this._boundMax.z));
        }

        public bool IsPointIn(VInt3 pnt)
        {
            return this.IsPointIn(ref pnt);
        }

        public bool IsPointIn(ref VInt3 pnt)
        {
            return (((this._vertecesXZ != null) && this.IsInBoundXZ(pnt)) && IntMath.PointInPolygon(ref pnt.xz, this._vertecesXZ));
        }

        private void Start()
        {
            GeoVertex[] componentsInChildren = base.get_gameObject().GetComponentsInChildren<GeoVertex>(true);
            this._vertecesXZ = new VInt2[componentsInChildren.Length];
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                VInt3 num2 = componentsInChildren[i].get_transform().get_position();
                this._vertecesXZ[i] = num2.xz;
                if (i == 0)
                {
                    this._boundMin = num2;
                    this._boundMax = num2;
                }
                else
                {
                    if (num2.x < this._boundMin.x)
                    {
                        this._boundMin.x = num2.x;
                    }
                    if (num2.y < this._boundMin.y)
                    {
                        this._boundMin.y = num2.y;
                    }
                    if (num2.z < this._boundMin.z)
                    {
                        this._boundMin.z = num2.z;
                    }
                    if (num2.x > this._boundMax.x)
                    {
                        this._boundMax.x = num2.x;
                    }
                    if (num2.y > this._boundMax.y)
                    {
                        this._boundMax.y = num2.y;
                    }
                    if (num2.z > this._boundMax.z)
                    {
                        this._boundMax.z = num2.z;
                    }
                }
            }
        }

        public VInt3 BoundMax
        {
            get
            {
                return this._boundMax;
            }
        }

        public VInt3 BoundMin
        {
            get
            {
                return this._boundMin;
            }
        }

        public VInt2[] VertecesXZ
        {
            get
            {
                return this._vertecesXZ;
            }
        }
    }
}

