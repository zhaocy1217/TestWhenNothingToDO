namespace Assets.Scripts.UI
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class CUIGraphLineScript : CUIGraphBaseScript
    {
        private float _curPathLen;
        private int _drawPathIndex;
        private float _fixPathLen;
        private float _lastDrawTime;
        public float drawSpeed;
        public float thickness = 1f;

        private static void GLLine(ref Vector3 start, ref Vector3 end, float thickness = 1f)
        {
            if (thickness <= 1f)
            {
                GL.Vertex3(start.x, start.y, start.z);
                GL.Vertex3(end.x, end.y, end.z);
            }
            else
            {
                Vector3 vector = end - start;
                Vector3 vector2 = vector.get_normalized();
                Vector3 vector3 = new Vector3(vector2.y, -vector2.x);
                Vector3 vector4 = (Vector3) (start + ((vector3 * thickness) * 0.5f));
                Vector3 vector5 = vector4 + vector;
                Vector3 vector6 = vector5 - ((Vector3) (vector3 * thickness));
                Vector3 vector7 = vector6 - vector;
                GL.Vertex3(vector4.x, vector4.y, vector4.z);
                GL.Vertex3(vector5.x, vector5.y, vector5.z);
                GL.Vertex3(vector6.x, vector6.y, vector6.z);
                GL.Vertex3(vector7.x, vector7.y, vector7.z);
            }
        }

        public override void Initialize(CUIFormScript formScript)
        {
            base.Initialize(formScript);
        }

        protected override void OnDraw()
        {
            if (base.vertexChanged)
            {
                base.vertexChanged = false;
                this._curPathLen = 0f;
                this._fixPathLen = 0f;
                this._drawPathIndex = (this.drawSpeed <= 0f) ? base.m_vertexs.Length : 1;
                this._lastDrawTime = Time.get_time();
            }
            GL.PushMatrix();
            GL.LoadPixelMatrix();
            GL.Begin((this.thickness > 1f) ? 7 : 1);
            GL.Color(base.color);
            for (int i = 1; i < this._drawPathIndex; i++)
            {
                GLLine(ref base.m_vertexs[i - 1], ref base.m_vertexs[i], this.thickness);
            }
            if (this._drawPathIndex < base.m_vertexs.Length)
            {
                float num2 = (Time.get_time() - this._lastDrawTime) * this.drawSpeed;
                this._lastDrawTime = Time.get_time();
                this._curPathLen += num2;
                while (this._drawPathIndex < base.m_vertexs.Length)
                {
                    Vector3 start = base.m_vertexs[this._drawPathIndex - 1];
                    Vector3 end = base.m_vertexs[this._drawPathIndex];
                    float num3 = (end - start).get_magnitude();
                    if ((this._fixPathLen + num3) > this._curPathLen)
                    {
                        break;
                    }
                    this._fixPathLen += num3;
                    this._drawPathIndex++;
                    GLLine(ref start, ref end, this.thickness);
                }
                if (this._drawPathIndex < base.m_vertexs.Length)
                {
                    Vector3 vector3 = base.m_vertexs[this._drawPathIndex - 1];
                    Vector3 vector4 = base.m_vertexs[this._drawPathIndex];
                    float num4 = this._curPathLen - this._fixPathLen;
                    Vector3 vector7 = vector4 - vector3;
                    Vector3 vector5 = vector3 + ((Vector3) (vector7.get_normalized() * num4));
                    GLLine(ref vector3, ref vector5, this.thickness);
                }
            }
            GL.End();
            GL.PopMatrix();
        }
    }
}

