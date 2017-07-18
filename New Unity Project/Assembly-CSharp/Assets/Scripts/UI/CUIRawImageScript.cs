namespace Assets.Scripts.UI
{
    using System;
    using UnityEngine;

    public class CUIRawImageScript : CUIComponent
    {
        private const int c_uiRawLayer = 15;
        private GameObject m_rawRootObject;
        private Camera m_renderTextureCamera;

        public void AddGameObject(string name, GameObject rawObject, Vector3 position, Quaternion rotation, Vector3 scaler)
        {
            if (this.m_rawRootObject != null)
            {
                this.SetRawObjectLayer(rawObject, LayerMask.NameToLayer("UIRaw"));
                rawObject.set_name(name);
                rawObject.get_transform().SetParent(this.m_rawRootObject.get_transform());
                rawObject.get_transform().set_localPosition(position);
                rawObject.get_transform().set_localRotation(rotation);
                rawObject.get_transform().set_localScale(scaler);
            }
        }

        public override void Appear()
        {
            base.Appear();
            CUIUtility.SetGameObjectLayer(this.m_rawRootObject, 15);
        }

        public GameObject GetGameObject(string name)
        {
            if (this.m_rawRootObject == null)
            {
                return null;
            }
            for (int i = 0; i < this.m_rawRootObject.get_transform().get_childCount(); i++)
            {
                GameObject obj3 = this.m_rawRootObject.get_transform().GetChild(i).get_gameObject();
                if (obj3.get_name().Equals(name))
                {
                    return obj3;
                }
            }
            return null;
        }

        public override void Hide()
        {
            base.Hide();
            CUIUtility.SetGameObjectLayer(this.m_rawRootObject, 0x1f);
        }

        public override void Initialize(CUIFormScript formScript)
        {
            if (!base.m_isInitialized)
            {
                base.Initialize(formScript);
                this.m_renderTextureCamera = base.GetComponentInChildren<Camera>(base.get_gameObject());
                if (this.m_renderTextureCamera != null)
                {
                    Transform transform = this.m_renderTextureCamera.get_gameObject().get_transform().FindChild("RawRoot");
                    if (transform != null)
                    {
                        this.m_rawRootObject = transform.get_gameObject();
                    }
                }
            }
        }

        protected override void OnDestroy()
        {
            this.m_renderTextureCamera = null;
            this.m_rawRootObject = null;
            base.OnDestroy();
        }

        public GameObject RemoveGameObject(string name)
        {
            if (this.m_rawRootObject != null)
            {
                for (int i = 0; i < this.m_rawRootObject.get_transform().get_childCount(); i++)
                {
                    GameObject obj2 = this.m_rawRootObject.get_transform().GetChild(i).get_gameObject();
                    if (obj2.get_name().Equals(name))
                    {
                        obj2.get_transform().SetParent(null);
                        return obj2;
                    }
                }
            }
            return null;
        }

        public void SetRawObjectLayer(GameObject rawObject, int layer)
        {
            rawObject.set_layer(layer);
            for (int i = 0; i < rawObject.get_transform().get_childCount(); i++)
            {
                this.SetRawObjectLayer(rawObject.get_transform().GetChild(i).get_gameObject(), layer);
            }
        }
    }
}

