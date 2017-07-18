namespace Assets.Scripts.UI
{
    using System;
    using UnityEngine;

    public class CUIComponent : MonoBehaviour
    {
        [HideInInspector]
        public CUIFormScript m_belongedFormScript;
        [HideInInspector]
        public CUIListScript m_belongedListScript;
        [HideInInspector]
        public int m_indexInlist;
        protected bool m_isInitialized;
        public GameObject[] m_widgets = new GameObject[0];

        public virtual void Appear()
        {
        }

        public virtual void Close()
        {
        }

        protected void DispatchUIEvent(CUIEvent uiEvent)
        {
            if (Singleton<CUIEventManager>.GetInstance() != null)
            {
                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uiEvent);
            }
        }

        protected T GetComponentInChildren<T>(GameObject go) where T: Component
        {
            T component = go.GetComponent<T>();
            if (component != null)
            {
                return component;
            }
            for (int i = 0; i < go.get_transform().get_childCount(); i++)
            {
                component = this.GetComponentInChildren<T>(go.get_transform().GetChild(i).get_gameObject());
                if (component != null)
                {
                    return component;
                }
            }
            return null;
        }

        public GameObject GetWidget(int index)
        {
            if ((index >= 0) && (index < this.m_widgets.Length))
            {
                return this.m_widgets[index];
            }
            return null;
        }

        public virtual void Hide()
        {
        }

        public virtual void Initialize(CUIFormScript formScript)
        {
            if (!this.m_isInitialized)
            {
                this.m_belongedFormScript = formScript;
                if (this.m_belongedFormScript != null)
                {
                    this.m_belongedFormScript.AddUIComponent(this);
                    this.SetSortingOrder(this.m_belongedFormScript.GetSortingOrder());
                }
                this.m_isInitialized = true;
            }
        }

        protected void InitializeComponent(GameObject root)
        {
            CUIComponent[] components = root.GetComponents<CUIComponent>();
            if ((components != null) && (components.Length > 0))
            {
                for (int j = 0; j < components.Length; j++)
                {
                    components[j].Initialize(this.m_belongedFormScript);
                }
            }
            for (int i = 0; i < root.get_transform().get_childCount(); i++)
            {
                this.InitializeComponent(root.get_transform().GetChild(i).get_gameObject());
            }
        }

        protected GameObject Instantiate(GameObject srcGameObject)
        {
            GameObject obj2 = Object.Instantiate(srcGameObject) as GameObject;
            obj2.get_transform().SetParent(srcGameObject.get_transform().get_parent());
            RectTransform transform = srcGameObject.get_transform() as RectTransform;
            RectTransform transform2 = obj2.get_transform() as RectTransform;
            if ((transform != null) && (transform2 != null))
            {
                transform2.set_pivot(transform.get_pivot());
                transform2.set_anchorMin(transform.get_anchorMin());
                transform2.set_anchorMax(transform.get_anchorMax());
                transform2.set_offsetMin(transform.get_offsetMin());
                transform2.set_offsetMax(transform.get_offsetMax());
                transform2.set_localPosition(transform.get_localPosition());
                transform2.set_localRotation(transform.get_localRotation());
                transform2.set_localScale(transform.get_localScale());
            }
            return obj2;
        }

        protected virtual void OnDestroy()
        {
            this.m_belongedFormScript = null;
            this.m_belongedListScript = null;
            this.m_widgets = null;
        }

        public void SetBelongedList(CUIListScript belongedListScript, int index)
        {
            this.m_belongedListScript = belongedListScript;
            this.m_indexInlist = index;
        }

        public virtual void SetSortingOrder(int sortingOrder)
        {
        }
    }
}

