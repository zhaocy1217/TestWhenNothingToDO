namespace Assets.Scripts.UI
{
    using Assets.Scripts.Framework;
    using System;
    using UnityEngine;

    public class CUIParticleScript : CUIComponent
    {
        public bool m_isFixScaleToForm;
        public bool m_isFixScaleToParticleSystem;
        private int m_rendererCount;
        private Renderer[] m_renderers;
        public string m_resPath = string.Empty;

        public override void Appear()
        {
            base.Appear();
            CUIUtility.SetGameObjectLayer(base.get_gameObject(), 5);
        }

        public override void Hide()
        {
            base.Hide();
            CUIUtility.SetGameObjectLayer(base.get_gameObject(), 0x1f);
        }

        public override void Initialize(CUIFormScript formScript)
        {
            if (!base.m_isInitialized)
            {
                this.LoadRes();
                this.InitializeRenderers();
                base.Initialize(formScript);
                if (this.m_isFixScaleToForm)
                {
                    this.ResetScale();
                }
                if (this.m_isFixScaleToParticleSystem)
                {
                    this.ResetParticleScale();
                }
                if (base.m_belongedFormScript.IsHided())
                {
                    this.Hide();
                }
            }
        }

        private void InitializeParticleScaler(GameObject gameObject, float scale)
        {
            ParticleScaler component = gameObject.GetComponent<ParticleScaler>();
            if (component == null)
            {
                component = gameObject.AddComponent<ParticleScaler>();
            }
            if (component.particleScale != scale)
            {
                component.particleScale = scale;
                component.alsoScaleGameobject = false;
                component.CheckAndApplyScale();
            }
        }

        private void InitializeRenderers()
        {
            this.m_renderers = new Renderer[100];
            this.m_rendererCount = 0;
            CUIUtility.GetComponentsInChildren<Renderer>(base.get_gameObject(), this.m_renderers, ref this.m_rendererCount);
        }

        private void LoadRes()
        {
            if (!string.IsNullOrEmpty(this.m_resPath))
            {
                string str;
                if (GameSettings.ParticleQuality == SGameRenderQuality.Low)
                {
                    string[] textArray1 = new string[] { CUIUtility.s_Particle_Dir, this.m_resPath, "/", this.m_resPath, "_low.prefeb" };
                    str = string.Concat(textArray1);
                }
                else if (GameSettings.ParticleQuality == SGameRenderQuality.Medium)
                {
                    string[] textArray2 = new string[] { CUIUtility.s_Particle_Dir, this.m_resPath, "/", this.m_resPath, "_mid.prefeb" };
                    str = string.Concat(textArray2);
                }
                else
                {
                    string[] textArray3 = new string[] { CUIUtility.s_Particle_Dir, this.m_resPath, "/", this.m_resPath, ".prefeb" };
                    str = string.Concat(textArray3);
                }
                GameObject content = Singleton<CResourceManager>.GetInstance().GetResource(str, typeof(GameObject), enResourceType.UIPrefab, false, false).m_content as GameObject;
                if ((content != null) && (base.get_gameObject().get_transform().get_childCount() == 0))
                {
                    GameObject obj3 = Object.Instantiate(content) as GameObject;
                    obj3.get_transform().SetParent(base.get_gameObject().get_transform());
                    obj3.get_transform().set_localPosition(Vector3.get_zero());
                    obj3.get_transform().set_localRotation(Quaternion.get_identity());
                    obj3.get_transform().set_localScale(Vector3.get_one());
                }
            }
        }

        public void LoadRes(string resName)
        {
            if (base.m_isInitialized)
            {
                this.m_resPath = resName;
                this.LoadRes();
                this.InitializeRenderers();
                this.SetSortingOrder(base.m_belongedFormScript.GetSortingOrder());
                if (this.m_isFixScaleToForm)
                {
                    this.ResetScale();
                }
                if (this.m_isFixScaleToParticleSystem)
                {
                    this.ResetParticleScale();
                }
                if (base.m_belongedFormScript.IsHided())
                {
                    this.Hide();
                }
            }
        }

        protected override void OnDestroy()
        {
            this.m_renderers = null;
            base.OnDestroy();
        }

        private void ResetParticleScale()
        {
            if (base.m_belongedFormScript != null)
            {
                float scale = 1f;
                RectTransform component = base.m_belongedFormScript.GetComponent<RectTransform>();
                if (base.m_belongedFormScript.m_canvasScaler.get_matchWidthOrHeight() == 0f)
                {
                    scale = (component.get_rect().get_width() / component.get_rect().get_height()) / (base.m_belongedFormScript.m_canvasScaler.get_referenceResolution().x / base.m_belongedFormScript.m_canvasScaler.get_referenceResolution().y);
                }
                else if (base.m_belongedFormScript.m_canvasScaler.get_matchWidthOrHeight() == 1f)
                {
                }
                this.InitializeParticleScaler(base.get_gameObject(), scale);
            }
        }

        private void ResetScale()
        {
            float num = 1f / base.m_belongedFormScript.get_gameObject().get_transform().get_localScale().x;
            base.get_gameObject().get_transform().set_localScale(new Vector3(num, num, 0f));
        }

        public override void SetSortingOrder(int sortingOrder)
        {
            base.SetSortingOrder(sortingOrder);
            for (int i = 0; i < this.m_rendererCount; i++)
            {
                this.m_renderers[i].set_sortingOrder(sortingOrder);
            }
        }
    }
}

