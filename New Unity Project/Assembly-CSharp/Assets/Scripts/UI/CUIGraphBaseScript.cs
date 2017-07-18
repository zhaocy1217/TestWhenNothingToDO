namespace Assets.Scripts.UI
{
    using System;
    using UnityEngine;

    public class CUIGraphBaseScript : CUIComponent
    {
        public int cameraDepth = s_depth;
        public Color color = Color.get_white();
        private Camera m_camera;
        [SerializeField]
        protected Vector3[] m_vertexs;
        public static readonly int s_cullingMask = LayerMask.NameToLayer("UI");
        public static readonly int s_depth = 11;
        private static Material s_lineMaterial = null;
        protected bool vertexChanged;

        public Camera GetCamera()
        {
            return this.m_camera;
        }

        public override void Initialize(CUIFormScript formScript)
        {
            base.Initialize(formScript);
            this.m_camera = base.GetComponent<Camera>();
            if (base.get_camera() == null)
            {
                this.m_camera = base.get_gameObject().AddComponent<Camera>();
                base.get_camera().set_depth((float) s_depth);
                base.get_camera().set_cullingMask(s_cullingMask);
                base.get_camera().set_clearFlags(3);
            }
            if (s_lineMaterial == null)
            {
                s_lineMaterial = new Material("Shader \"Lines/Colored Blended\" {SubShader { Pass {     Blend SrcAlpha OneMinusSrcAlpha     ZWrite Off Cull Off Fog { Mode Off }     BindChannels {      Bind \"vertex\", vertex Bind \"color\", color }} } }");
                s_lineMaterial.set_hideFlags(13);
                s_lineMaterial.get_shader().set_hideFlags(13);
            }
        }

        protected override void OnDestroy()
        {
            this.m_camera = null;
            base.OnDestroy();
        }

        protected virtual void OnDraw()
        {
        }

        private void OnPostRender()
        {
            if ((this.m_vertexs != null) && (s_lineMaterial != null))
            {
                s_lineMaterial.SetPass(0);
                this.OnDraw();
            }
        }

        public void SetVertexs(Vector3[] vertexs)
        {
            if (vertexs != null)
            {
                this.m_vertexs = new Vector3[vertexs.Length];
                for (int i = 0; i < vertexs.Length; i++)
                {
                    this.m_vertexs[i] = new Vector3(vertexs[i].x, vertexs[i].y, 0f);
                }
                this.vertexChanged = true;
            }
        }
    }
}

