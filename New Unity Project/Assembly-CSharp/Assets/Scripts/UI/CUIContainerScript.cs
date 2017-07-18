namespace Assets.Scripts.UI
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class CUIContainerScript : CUIComponent
    {
        private const int c_elementMaxAmount = 200;
        private string m_elementName;
        private GameObject m_elementTemplate;
        public int m_prepareElementAmount;
        private List<GameObject> m_unusedElements = new List<GameObject>();
        private int m_usedElementAmount;
        private GameObject[] m_usedElements = new GameObject[200];

        public int GetElement()
        {
            if ((this.m_elementTemplate != null) && (this.m_usedElementAmount < 200))
            {
                GameObject obj2 = null;
                if (this.m_unusedElements.Count > 0)
                {
                    obj2 = this.m_unusedElements[0];
                    this.m_unusedElements.RemoveAt(0);
                }
                else
                {
                    obj2 = base.Instantiate(this.m_elementTemplate);
                    obj2.set_name(this.m_elementName);
                    base.InitializeComponent(obj2.get_gameObject());
                }
                obj2.SetActive(true);
                for (int i = 0; i < 200; i++)
                {
                    if (this.m_usedElements[i] == null)
                    {
                        this.m_usedElements[i] = obj2;
                        this.m_usedElementAmount++;
                        return i;
                    }
                }
            }
            return -1;
        }

        public GameObject GetElement(int sequence)
        {
            if ((sequence < 0) || (sequence >= 200))
            {
                return null;
            }
            return ((this.m_usedElements[sequence] != null) ? this.m_usedElements[sequence].get_gameObject() : null);
        }

        public override void Initialize(CUIFormScript formScript)
        {
            if (!base.m_isInitialized)
            {
                base.Initialize(formScript);
                for (int i = 0; i < base.get_gameObject().get_transform().get_childCount(); i++)
                {
                    GameObject obj2 = base.get_gameObject().get_transform().GetChild(i).get_gameObject();
                    if (this.m_elementTemplate == null)
                    {
                        this.m_elementTemplate = obj2;
                        this.m_elementName = obj2.get_name();
                        this.m_elementTemplate.set_name(this.m_elementName + "_Template");
                        if (this.m_elementTemplate.get_activeSelf())
                        {
                            this.m_elementTemplate.SetActive(false);
                        }
                    }
                    obj2.SetActive(false);
                }
                if (this.m_prepareElementAmount > 0)
                {
                    for (int j = 0; j < this.m_prepareElementAmount; j++)
                    {
                        GameObject item = base.Instantiate(this.m_elementTemplate);
                        item.get_gameObject().set_name(this.m_elementName);
                        base.InitializeComponent(item.get_gameObject());
                        if (item.get_activeSelf())
                        {
                            item.SetActive(false);
                        }
                        if (item.get_transform().get_parent() != base.get_gameObject().get_transform())
                        {
                            item.get_transform().SetParent(base.get_gameObject().get_transform(), true);
                            item.get_transform().set_localScale(Vector3.get_one());
                        }
                        this.m_unusedElements.Add(item);
                    }
                }
            }
        }

        protected override void OnDestroy()
        {
            this.m_elementTemplate = null;
            this.m_usedElements = null;
            this.m_unusedElements.Clear();
            this.m_unusedElements = null;
            base.OnDestroy();
        }

        public void RecycleAllElement()
        {
            if ((this.m_elementTemplate != null) && (this.m_usedElementAmount > 0))
            {
                for (int i = 0; i < 200; i++)
                {
                    if (this.m_usedElements[i] != null)
                    {
                        this.m_usedElements[i].SetActive(false);
                        if (this.m_usedElements[i].get_transform().get_parent() != base.get_gameObject().get_transform())
                        {
                            this.m_usedElements[i].get_transform().SetParent(base.get_gameObject().get_transform(), true);
                            this.m_usedElements[i].get_transform().set_localScale(Vector3.get_one());
                        }
                        this.m_unusedElements.Add(this.m_usedElements[i]);
                        this.m_usedElements[i] = null;
                        this.m_usedElementAmount--;
                    }
                }
            }
        }

        public void RecycleElement(int sequence)
        {
            if (((this.m_elementTemplate != null) && (sequence >= 0)) && (sequence < 200))
            {
                GameObject item = this.m_usedElements[sequence];
                this.m_usedElements[sequence] = null;
                if (item != null)
                {
                    item.SetActive(false);
                    if (item.get_transform().get_parent() != base.get_gameObject().get_transform())
                    {
                        item.get_transform().SetParent(base.get_gameObject().get_transform(), true);
                        item.get_transform().set_localScale(Vector3.get_one());
                    }
                    this.m_unusedElements.Add(item);
                    this.m_usedElementAmount--;
                }
            }
        }

        public void RecycleElement(GameObject elementObject)
        {
            if ((this.m_elementTemplate != null) && (elementObject != null))
            {
                GameObject item = elementObject;
                for (int i = 0; i < 200; i++)
                {
                    if (this.m_usedElements[i] == item)
                    {
                        this.m_usedElements[i] = null;
                        this.m_usedElementAmount--;
                        break;
                    }
                }
                item.SetActive(false);
                if (item.get_transform().get_parent() != base.get_gameObject().get_transform())
                {
                    item.get_transform().SetParent(base.get_gameObject().get_transform(), true);
                    item.get_transform().set_localScale(Vector3.get_one());
                }
                this.m_unusedElements.Add(item);
            }
        }
    }
}

