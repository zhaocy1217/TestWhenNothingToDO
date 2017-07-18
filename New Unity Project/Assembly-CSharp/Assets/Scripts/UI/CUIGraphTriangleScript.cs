namespace Assets.Scripts.UI
{
    using System;
    using UnityEngine;

    public class CUIGraphTriangleScript : CUIGraphBaseScript
    {
        public override void Initialize(CUIFormScript formScript)
        {
            base.Initialize(formScript);
        }

        protected override void OnDraw()
        {
            GL.PushMatrix();
            GL.LoadPixelMatrix();
            GL.Begin(4);
            GL.Color(base.color);
            for (int i = 0; i < base.m_vertexs.Length; i++)
            {
                if ((i + 2) < base.m_vertexs.Length)
                {
                    GL.Vertex3(base.m_vertexs[i].x, base.m_vertexs[i].y, base.m_vertexs[i].z);
                    GL.Vertex3(base.m_vertexs[i + 1].x, base.m_vertexs[i + 1].y, base.m_vertexs[i + 1].z);
                    GL.Vertex3(base.m_vertexs[i + 2].x, base.m_vertexs[i + 2].y, base.m_vertexs[i + 2].z);
                }
                i += 2;
            }
            GL.End();
            GL.PopMatrix();
        }
    }
}

