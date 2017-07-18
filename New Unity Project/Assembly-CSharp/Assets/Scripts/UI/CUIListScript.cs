namespace Assets.Scripts.UI
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class CUIListScript : CUIComponent
    {
        public bool m_alwaysDispatchSelectedChangeEvent;
        [HideInInspector]
        public bool m_autoAdjustScrollAreaSize;
        public bool m_autoCenteredBothSides;
        public bool m_autoCenteredElements;
        protected GameObject m_content;
        protected RectTransform m_contentRectTransform;
        protected Vector2 m_contentSize;
        public int m_elementAmount;
        protected Vector2 m_elementDefaultSize;
        public float m_elementLayoutOffset;
        protected string m_elementName;
        protected ListView<CUIListElementScript> m_elementScripts;
        public Vector2 m_elementSpacing;
        protected List<stRect> m_elementsRect;
        protected List<Vector2> m_elementsSize;
        protected GameObject m_elementTemplate;
        [HideInInspector]
        public string m_externalElementPrefabPath;
        public GameObject m_extraContent;
        public float m_fSpeed = 0.2f;
        public bool m_isDisableScrollRect;
        private bool m_isExtraContentEnabled = true;
        protected Vector2 m_lastContentPosition;
        protected int m_lastSelectedElementIndex = -1;
        [HideInInspector]
        public enUIEventID m_listScrollChangedEventID;
        public stUIEventParams m_listScrollChangedEventParams;
        [HideInInspector]
        public enUIEventID m_listSelectChangedEventID;
        public stUIEventParams m_listSelectChangedEventParams;
        public enUIListType m_listType;
        [HideInInspector]
        public Vector2 m_scrollAreaSize;
        protected Scrollbar m_scrollBar;
        public bool m_scrollExternal;
        [HideInInspector]
        public ScrollRect m_scrollRect;
        [HideInInspector]
        public Vector2 m_scrollRectAreaMaxSize = new Vector2(100f, 100f);
        protected int m_selectedElementIndex = -1;
        protected ListView<CUIListElementScript> m_unUsedElementScripts;
        [HideInInspector]
        public bool m_useExternalElement;
        public bool m_useOptimized;
        public GameObject m_ZeroTipsObj;

        protected CUIListElementScript CreateElement(int index, ref stRect rect)
        {
            CUIListElementScript item = null;
            if (this.m_unUsedElementScripts.Count > 0)
            {
                item = this.m_unUsedElementScripts[0];
                this.m_unUsedElementScripts.RemoveAt(0);
            }
            else if (this.m_elementTemplate != null)
            {
                GameObject root = base.Instantiate(this.m_elementTemplate);
                root.get_transform().SetParent(this.m_content.get_transform());
                root.get_transform().set_localScale(Vector3.get_one());
                base.InitializeComponent(root);
                item = root.GetComponent<CUIListElementScript>();
            }
            if (item != null)
            {
                item.Enable(this, index, this.m_elementName, ref rect, this.IsSelectedIndex(index));
                this.m_elementScripts.Add(item);
            }
            return item;
        }

        protected void DetectScroll()
        {
            if (this.m_contentRectTransform.get_anchoredPosition() != this.m_lastContentPosition)
            {
                this.m_lastContentPosition = this.m_contentRectTransform.get_anchoredPosition();
                this.DispatchScrollChangedEvent();
            }
        }

        protected void DispatchElementSelectChangedEvent()
        {
            if (this.m_listSelectChangedEventID != enUIEventID.None)
            {
                CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
                uIEvent.m_eventID = this.m_listSelectChangedEventID;
                uIEvent.m_eventParams = this.m_listSelectChangedEventParams;
                uIEvent.m_srcFormScript = base.m_belongedFormScript;
                uIEvent.m_srcWidgetBelongedListScript = base.m_belongedListScript;
                uIEvent.m_srcWidgetIndexInBelongedList = base.m_indexInlist;
                uIEvent.m_srcWidget = base.get_gameObject();
                uIEvent.m_srcWidgetScript = this;
                uIEvent.m_pointerEventData = null;
                base.DispatchUIEvent(uIEvent);
            }
        }

        protected void DispatchScrollChangedEvent()
        {
            if (this.m_listScrollChangedEventID != enUIEventID.None)
            {
                CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
                uIEvent.m_eventID = this.m_listScrollChangedEventID;
                uIEvent.m_eventParams = this.m_listScrollChangedEventParams;
                uIEvent.m_srcFormScript = base.m_belongedFormScript;
                uIEvent.m_srcWidgetBelongedListScript = base.m_belongedListScript;
                uIEvent.m_srcWidgetIndexInBelongedList = base.m_indexInlist;
                uIEvent.m_srcWidget = base.get_gameObject();
                uIEvent.m_srcWidgetScript = this;
                uIEvent.m_pointerEventData = null;
                base.DispatchUIEvent(uIEvent);
            }
        }

        public void EnableExtraContent(bool isEnabled)
        {
            this.m_isExtraContentEnabled = isEnabled;
        }

        public Vector2 GetContentPosition()
        {
            return this.m_lastContentPosition;
        }

        public Vector2 GetContentSize()
        {
            return this.m_contentSize;
        }

        public Vector2 GetEelementDefaultSize()
        {
            return this.m_elementDefaultSize;
        }

        public CUIListElementScript GetElemenet(int index)
        {
            if ((index >= 0) && (index < this.m_elementAmount))
            {
                if (!this.m_useOptimized)
                {
                    return this.m_elementScripts[index];
                }
                for (int i = 0; i < this.m_elementScripts.Count; i++)
                {
                    if (this.m_elementScripts[i].m_index == index)
                    {
                        return this.m_elementScripts[i];
                    }
                }
            }
            return null;
        }

        public int GetElementAmount()
        {
            return this.m_elementAmount;
        }

        public CUIListElementScript GetLastSelectedElement()
        {
            return this.GetElemenet(this.m_lastSelectedElementIndex);
        }

        public int GetLastSelectedIndex()
        {
            return this.m_lastSelectedElementIndex;
        }

        public Vector2 GetScrollAreaSize()
        {
            return this.m_scrollAreaSize;
        }

        public CUIListElementScript GetSelectedElement()
        {
            return this.GetElemenet(this.m_selectedElementIndex);
        }

        public int GetSelectedIndex()
        {
            return this.m_selectedElementIndex;
        }

        public void HideExtraContent()
        {
            if ((this.m_extraContent != null) && this.m_isExtraContentEnabled)
            {
                this.m_extraContent.CustomSetActive(false);
            }
        }

        public override void Initialize(CUIFormScript formScript)
        {
            if (!base.m_isInitialized)
            {
                base.Initialize(formScript);
                this.m_selectedElementIndex = -1;
                this.m_lastSelectedElementIndex = -1;
                this.m_scrollRect = base.GetComponentInChildren<ScrollRect>(base.get_gameObject());
                if (this.m_scrollRect != null)
                {
                    this.m_scrollRect.set_enabled(false);
                    RectTransform transform = this.m_scrollRect.get_transform() as RectTransform;
                    this.m_scrollAreaSize = new Vector2(transform.get_rect().get_width(), transform.get_rect().get_height());
                    this.m_content = this.m_scrollRect.get_content().get_gameObject();
                }
                this.m_scrollBar = base.GetComponentInChildren<Scrollbar>(base.get_gameObject());
                if ((this.m_listType == enUIListType.Vertical) || (this.m_listType == enUIListType.VerticalGrid))
                {
                    if (this.m_scrollBar != null)
                    {
                        this.m_scrollBar.set_direction(2);
                    }
                    DebugHelper.Assert(this.m_scrollRect != null);
                    if (this.m_scrollRect != null)
                    {
                        this.m_scrollRect.set_horizontal(false);
                        this.m_scrollRect.set_vertical(true);
                        this.m_scrollRect.set_horizontalScrollbar(null);
                        this.m_scrollRect.set_verticalScrollbar(this.m_scrollBar);
                    }
                }
                else if ((this.m_listType == enUIListType.Horizontal) || (this.m_listType == enUIListType.HorizontalGrid))
                {
                    if (this.m_scrollBar != null)
                    {
                        this.m_scrollBar.set_direction(0);
                    }
                    DebugHelper.Assert(this.m_scrollRect != null);
                    if (this.m_scrollRect != null)
                    {
                        this.m_scrollRect.set_horizontal(true);
                        this.m_scrollRect.set_vertical(false);
                        this.m_scrollRect.set_horizontalScrollbar(this.m_scrollBar);
                        this.m_scrollRect.set_verticalScrollbar(null);
                    }
                }
                this.m_elementScripts = new ListView<CUIListElementScript>();
                this.m_unUsedElementScripts = new ListView<CUIListElementScript>();
                if (this.m_useOptimized && (this.m_elementsRect == null))
                {
                    this.m_elementsRect = new List<stRect>();
                }
                CUIListElementScript component = null;
                if (this.m_useExternalElement)
                {
                    GameObject content = null;
                    if (this.m_externalElementPrefabPath != null)
                    {
                        content = Singleton<CResourceManager>.GetInstance().GetResource(this.m_externalElementPrefabPath, typeof(GameObject), enResourceType.UIPrefab, false, false).m_content;
                        if (content != null)
                        {
                            component = content.GetComponent<CUIListElementScript>();
                        }
                    }
                    if ((component != null) && (content != null))
                    {
                        component.Initialize(formScript);
                        this.m_elementTemplate = content;
                        this.m_elementName = content.get_name();
                        this.m_elementDefaultSize = component.m_defaultSize;
                    }
                }
                else
                {
                    component = base.GetComponentInChildren<CUIListElementScript>(base.get_gameObject());
                    if (component != null)
                    {
                        component.Initialize(formScript);
                        this.m_elementTemplate = component.get_gameObject();
                        this.m_elementName = component.get_gameObject().get_name();
                        this.m_elementDefaultSize = component.m_defaultSize;
                        if (this.m_elementTemplate != null)
                        {
                            this.m_elementTemplate.set_name(this.m_elementName + "_Template");
                        }
                    }
                }
                if (this.m_elementTemplate != null)
                {
                    CUIListElementScript script2 = this.m_elementTemplate.GetComponent<CUIListElementScript>();
                    if ((script2 != null) && script2.m_useSetActiveForDisplay)
                    {
                        this.m_elementTemplate.CustomSetActive(false);
                    }
                    else
                    {
                        if (!this.m_elementTemplate.get_activeSelf())
                        {
                            this.m_elementTemplate.SetActive(true);
                        }
                        CanvasGroup group = this.m_elementTemplate.GetComponent<CanvasGroup>();
                        if (group == null)
                        {
                            group = this.m_elementTemplate.AddComponent<CanvasGroup>();
                        }
                        group.set_alpha(0f);
                        group.set_blocksRaycasts(false);
                    }
                }
                if (this.m_content != null)
                {
                    this.m_contentRectTransform = this.m_content.get_transform() as RectTransform;
                    this.m_contentRectTransform.set_pivot(new Vector2(0f, 1f));
                    this.m_contentRectTransform.set_anchorMin(new Vector2(0f, 1f));
                    this.m_contentRectTransform.set_anchorMax(new Vector2(0f, 1f));
                    this.m_contentRectTransform.set_anchoredPosition(Vector2.get_zero());
                    this.m_contentRectTransform.set_localRotation(Quaternion.get_identity());
                    this.m_contentRectTransform.set_localScale(new Vector3(1f, 1f, 1f));
                    this.m_lastContentPosition = this.m_contentRectTransform.get_anchoredPosition();
                }
                if (this.m_extraContent != null)
                {
                    RectTransform transform2 = this.m_extraContent.get_transform() as RectTransform;
                    transform2.set_pivot(new Vector2(0f, 1f));
                    transform2.set_anchorMin(new Vector2(0f, 1f));
                    transform2.set_anchorMax(new Vector2(0f, 1f));
                    transform2.set_anchoredPosition(Vector2.get_zero());
                    transform2.set_localRotation(Quaternion.get_identity());
                    transform2.set_localScale(Vector3.get_one());
                    if (this.m_elementTemplate != null)
                    {
                        transform2.set_sizeDelta(new Vector2((this.m_elementTemplate.get_transform() as RectTransform).get_rect().get_width(), transform2.get_sizeDelta().y));
                    }
                    if ((transform2.get_parent() != null) && (this.m_content != null))
                    {
                        transform2.get_parent().SetParent(this.m_content.get_transform());
                    }
                    this.m_extraContent.SetActive(false);
                }
                this.m_isExtraContentEnabled = true;
                if (this.m_elementTemplate != null)
                {
                    this.SetElementAmount(this.m_elementAmount);
                }
            }
        }

        public bool IsElementInScrollArea(int index)
        {
            if ((index < 0) || (index >= this.m_elementAmount))
            {
                return false;
            }
            stRect rect = !this.m_useOptimized ? this.m_elementScripts[index].m_rect : this.m_elementsRect[index];
            return this.IsRectInScrollArea(ref rect);
        }

        public bool IsNeedScroll()
        {
            return ((this.m_contentSize.x > this.m_scrollAreaSize.x) || (this.m_contentSize.y > this.m_scrollAreaSize.y));
        }

        protected bool IsRectInScrollArea(ref stRect rect)
        {
            Vector2 vector = Vector2.get_zero();
            vector.x = this.m_contentRectTransform.get_anchoredPosition().x + rect.m_left;
            vector.y = this.m_contentRectTransform.get_anchoredPosition().y + rect.m_top;
            return ((((vector.x + rect.m_width) >= 0f) && (vector.x <= this.m_scrollAreaSize.x)) && (((vector.y - rect.m_height) <= 0f) && (vector.y >= -this.m_scrollAreaSize.y)));
        }

        public virtual bool IsSelectedIndex(int index)
        {
            return (this.m_selectedElementIndex == index);
        }

        protected stRect LayoutElement(int index, ref Vector2 contentSize, ref Vector2 offset)
        {
            stRect rect = new stRect();
            rect.m_width = (((this.m_elementsSize != null) && (this.m_listType != enUIListType.Vertical)) && ((this.m_listType != enUIListType.VerticalGrid) && (this.m_listType != enUIListType.HorizontalGrid))) ? ((int) this.m_elementsSize[index].x) : ((int) this.m_elementDefaultSize.x);
            rect.m_height = (((this.m_elementsSize != null) && (this.m_listType != enUIListType.Horizontal)) && ((this.m_listType != enUIListType.VerticalGrid) && (this.m_listType != enUIListType.HorizontalGrid))) ? ((int) this.m_elementsSize[index].y) : ((int) this.m_elementDefaultSize.y);
            rect.m_left = (int) offset.x;
            rect.m_top = (int) offset.y;
            rect.m_right = rect.m_left + rect.m_width;
            rect.m_bottom = rect.m_top - rect.m_height;
            rect.m_center = new Vector2(rect.m_left + (rect.m_width * 0.5f), rect.m_top - (rect.m_height * 0.5f));
            if (rect.m_right > contentSize.x)
            {
                contentSize.x = rect.m_right;
            }
            if (-rect.m_bottom > contentSize.y)
            {
                contentSize.y = -rect.m_bottom;
            }
            switch (this.m_listType)
            {
                case enUIListType.Vertical:
                    offset.y -= rect.m_height + this.m_elementSpacing.y;
                    return rect;

                case enUIListType.Horizontal:
                    offset.x += rect.m_width + this.m_elementSpacing.x;
                    return rect;

                case enUIListType.VerticalGrid:
                    offset.x += rect.m_width + this.m_elementSpacing.x;
                    if ((offset.x + rect.m_width) > this.m_scrollAreaSize.x)
                    {
                        offset.x = 0f;
                        offset.y -= rect.m_height + this.m_elementSpacing.y;
                    }
                    return rect;

                case enUIListType.HorizontalGrid:
                    offset.y -= rect.m_height + this.m_elementSpacing.y;
                    if ((-offset.y + rect.m_height) > this.m_scrollAreaSize.y)
                    {
                        offset.y = 0f;
                        offset.x += rect.m_width + this.m_elementSpacing.x;
                    }
                    return rect;
            }
            return rect;
        }

        public void MoveContent(Vector2 offset)
        {
            if (this.m_contentRectTransform != null)
            {
                Vector2 vector = (this.m_content.get_transform() as RectTransform).get_anchoredPosition() + offset;
                if (offset.x != 0f)
                {
                    if (vector.x > 0f)
                    {
                        vector.x = 0f;
                    }
                    else if ((vector.x + this.m_contentSize.x) < this.m_scrollAreaSize.x)
                    {
                        vector.x = this.m_scrollAreaSize.x - this.m_contentSize.x;
                    }
                }
                if (offset.y != 0f)
                {
                    if (vector.y < 0f)
                    {
                        vector.y = 0f;
                    }
                    else if ((this.m_contentSize.y - vector.y) < this.m_scrollAreaSize.y)
                    {
                        vector.y = this.m_contentSize.y - this.m_scrollAreaSize.y;
                    }
                }
                this.m_contentRectTransform.set_anchoredPosition(vector);
            }
        }

        public void MoveElementInScrollArea(int index, bool moveImmediately)
        {
            if ((index >= 0) && (index < this.m_elementAmount))
            {
                Vector2 vector = Vector2.get_zero();
                Vector2 vector2 = Vector2.get_zero();
                stRect rect = !this.m_useOptimized ? this.m_elementScripts[index].m_rect : this.m_elementsRect[index];
                vector2.x = this.m_contentRectTransform.get_anchoredPosition().x + rect.m_left;
                vector2.y = this.m_contentRectTransform.get_anchoredPosition().y + rect.m_top;
                if (vector2.x < 0f)
                {
                    vector.x = -vector2.x;
                }
                else if ((vector2.x + rect.m_width) > this.m_scrollAreaSize.x)
                {
                    vector.x = this.m_scrollAreaSize.x - (vector2.x + rect.m_width);
                }
                if (vector2.y > 0f)
                {
                    vector.y = -vector2.y;
                }
                else if ((vector2.y - rect.m_height) < -this.m_scrollAreaSize.y)
                {
                    vector.y = -this.m_scrollAreaSize.y - (vector2.y - rect.m_height);
                }
                if (moveImmediately)
                {
                    this.m_contentRectTransform.set_anchoredPosition(this.m_contentRectTransform.get_anchoredPosition() + vector);
                }
                else
                {
                    Vector2 to = this.m_contentRectTransform.get_anchoredPosition() + vector;
                    LeanTween.value(base.get_gameObject(), delegate (Vector2 pos) {
                        this.m_contentRectTransform.set_anchoredPosition(pos);
                    }, this.m_contentRectTransform.get_anchoredPosition(), to, this.m_fSpeed);
                }
            }
        }

        protected override void OnDestroy()
        {
            if (LeanTween.IsInitialised())
            {
                LeanTween.cancel(base.get_gameObject());
            }
            this.m_ZeroTipsObj = null;
            this.m_elementTemplate = null;
            this.m_scrollRect = null;
            this.m_content = null;
            this.m_contentRectTransform = null;
            this.m_scrollBar = null;
            this.m_extraContent = null;
            if (this.m_elementScripts != null)
            {
                this.m_elementScripts.Clear();
                this.m_elementScripts = null;
            }
            if (this.m_unUsedElementScripts != null)
            {
                this.m_unUsedElementScripts.Clear();
                this.m_unUsedElementScripts = null;
            }
            base.OnDestroy();
        }

        protected virtual void ProcessElements()
        {
            this.m_contentSize = Vector2.get_zero();
            Vector2 offset = Vector2.get_zero();
            if ((this.m_listType == enUIListType.Vertical) || (this.m_listType == enUIListType.VerticalGrid))
            {
                offset.y += this.m_elementLayoutOffset;
            }
            else
            {
                offset.x += this.m_elementLayoutOffset;
            }
            for (int i = 0; i < this.m_elementAmount; i++)
            {
                stRect item = this.LayoutElement(i, ref this.m_contentSize, ref offset);
                if (this.m_useOptimized)
                {
                    if (i < this.m_elementsRect.Count)
                    {
                        this.m_elementsRect[i] = item;
                    }
                    else
                    {
                        this.m_elementsRect.Add(item);
                    }
                }
                if (!this.m_useOptimized || this.IsRectInScrollArea(ref item))
                {
                    this.CreateElement(i, ref item);
                }
            }
            if (this.m_extraContent != null)
            {
                if ((this.m_elementAmount > 0) && this.m_isExtraContentEnabled)
                {
                    this.ProcessExtraContent(ref this.m_contentSize, offset);
                }
                else
                {
                    this.m_extraContent.CustomSetActive(false);
                }
            }
            this.ResizeContent(ref this.m_contentSize, false);
        }

        private void ProcessExtraContent(ref Vector2 contentSize, Vector2 offset)
        {
            RectTransform transform = this.m_extraContent.get_transform() as RectTransform;
            transform.set_anchoredPosition(offset);
            this.m_extraContent.CustomSetActive(true);
            if ((this.m_listType == enUIListType.Horizontal) || (this.m_listType == enUIListType.HorizontalGrid))
            {
                contentSize.x += transform.get_rect().get_width() + this.m_elementSpacing.x;
            }
            else
            {
                contentSize.y += transform.get_rect().get_height() + this.m_elementSpacing.y;
            }
        }

        protected void ProcessUnUsedElement()
        {
            if ((this.m_unUsedElementScripts != null) && (this.m_unUsedElementScripts.Count > 0))
            {
                for (int i = 0; i < this.m_unUsedElementScripts.Count; i++)
                {
                    this.m_unUsedElementScripts[i].Disable();
                }
            }
        }

        protected void RecycleElement(bool disableElement)
        {
            while (this.m_elementScripts.Count > 0)
            {
                CUIListElementScript item = this.m_elementScripts[0];
                this.m_elementScripts.RemoveAt(0);
                if (disableElement)
                {
                    item.Disable();
                }
                this.m_unUsedElementScripts.Add(item);
            }
        }

        protected void RecycleElement(CUIListElementScript elementScript, bool disableElement)
        {
            if (disableElement)
            {
                elementScript.Disable();
            }
            this.m_elementScripts.Remove(elementScript);
            this.m_unUsedElementScripts.Add(elementScript);
        }

        public void ResetContentPosition()
        {
            if (LeanTween.IsInitialised())
            {
                LeanTween.cancel(base.get_gameObject());
            }
            if (this.m_contentRectTransform != null)
            {
                this.m_contentRectTransform.set_anchoredPosition(Vector2.get_zero());
            }
        }

        protected virtual void ResizeContent(ref Vector2 size, bool resetPosition)
        {
            float num = 0f;
            float num2 = 0f;
            if (this.m_autoAdjustScrollAreaSize)
            {
                Vector2 scrollAreaSize = this.m_scrollAreaSize;
                this.m_scrollAreaSize = size;
                if (this.m_scrollAreaSize.x > this.m_scrollRectAreaMaxSize.x)
                {
                    this.m_scrollAreaSize.x = this.m_scrollRectAreaMaxSize.x;
                }
                if (this.m_scrollAreaSize.y > this.m_scrollRectAreaMaxSize.y)
                {
                    this.m_scrollAreaSize.y = this.m_scrollRectAreaMaxSize.y;
                }
                Vector2 vector2 = this.m_scrollAreaSize - scrollAreaSize;
                if (vector2 != Vector2.get_zero())
                {
                    RectTransform transform = base.get_gameObject().get_transform() as RectTransform;
                    if (transform.get_anchorMin() == transform.get_anchorMax())
                    {
                        transform.set_sizeDelta(transform.get_sizeDelta() + vector2);
                    }
                }
            }
            else if (this.m_autoCenteredElements)
            {
                if ((this.m_listType == enUIListType.Vertical) && (size.y < this.m_scrollAreaSize.y))
                {
                    num2 = (size.y - this.m_scrollAreaSize.y) / 2f;
                    if (this.m_autoCenteredBothSides)
                    {
                        num = (this.m_scrollAreaSize.x - size.x) / 2f;
                    }
                }
                else if ((this.m_listType == enUIListType.Horizontal) && (size.x < this.m_scrollAreaSize.x))
                {
                    num = (this.m_scrollAreaSize.x - size.x) / 2f;
                    if (this.m_autoCenteredBothSides)
                    {
                        num2 = (size.y - this.m_scrollAreaSize.y) / 2f;
                    }
                }
                else
                {
                    if ((this.m_listType != enUIListType.VerticalGrid) || (size.x >= this.m_scrollAreaSize.x))
                    {
                        if ((this.m_listType != enUIListType.HorizontalGrid) || (size.y >= this.m_scrollAreaSize.y))
                        {
                            goto Label_02F1;
                        }
                        float num6 = size.y + this.m_elementSpacing.y;
                        while (true)
                        {
                            float num5 = num6 + this.m_elementDefaultSize.y;
                            if (num5 > this.m_scrollAreaSize.y)
                            {
                                num2 = (size.y - this.m_scrollAreaSize.y) / 2f;
                                goto Label_02F1;
                            }
                            size.y = num5;
                            num6 = num5 + this.m_elementSpacing.y;
                        }
                    }
                    float num4 = size.x + this.m_elementSpacing.x;
                    while (true)
                    {
                        float num3 = num4 + this.m_elementDefaultSize.x;
                        if (num3 > this.m_scrollAreaSize.x)
                        {
                            break;
                        }
                        size.x = num3;
                        num4 = num3 + this.m_elementSpacing.x;
                    }
                    num = (this.m_scrollAreaSize.x - size.x) / 2f;
                }
            }
        Label_02F1:
            if (size.x < this.m_scrollAreaSize.x)
            {
                size.x = this.m_scrollAreaSize.x;
            }
            if (size.y < this.m_scrollAreaSize.y)
            {
                size.y = this.m_scrollAreaSize.y;
            }
            if (this.m_contentRectTransform != null)
            {
                this.m_contentRectTransform.set_sizeDelta(size);
                if (resetPosition)
                {
                    this.m_contentRectTransform.set_anchoredPosition(Vector2.get_zero());
                }
                if (this.m_autoCenteredElements)
                {
                    if (num != 0f)
                    {
                        this.m_contentRectTransform.set_anchoredPosition(new Vector2(num, this.m_contentRectTransform.get_anchoredPosition().y));
                    }
                    if (num2 != 0f)
                    {
                        this.m_contentRectTransform.set_anchoredPosition(new Vector2(this.m_contentRectTransform.get_anchoredPosition().x, num2));
                    }
                }
            }
        }

        public virtual void SelectElement(int index, bool isDispatchSelectedChangeEvent = true)
        {
            this.m_lastSelectedElementIndex = this.m_selectedElementIndex;
            this.m_selectedElementIndex = index;
            if (this.m_lastSelectedElementIndex == this.m_selectedElementIndex)
            {
                if (this.m_alwaysDispatchSelectedChangeEvent && isDispatchSelectedChangeEvent)
                {
                    this.DispatchElementSelectChangedEvent();
                }
            }
            else
            {
                if (this.m_lastSelectedElementIndex >= 0)
                {
                    CUIListElementScript elemenet = this.GetElemenet(this.m_lastSelectedElementIndex);
                    if (elemenet != null)
                    {
                        elemenet.ChangeDisplay(false);
                    }
                }
                if (this.m_selectedElementIndex >= 0)
                {
                    CUIListElementScript script2 = this.GetElemenet(this.m_selectedElementIndex);
                    if (script2 != null)
                    {
                        script2.ChangeDisplay(true);
                        if (script2.onSelected != null)
                        {
                            script2.onSelected();
                        }
                    }
                }
                if (isDispatchSelectedChangeEvent)
                {
                    this.DispatchElementSelectChangedEvent();
                }
            }
        }

        public void SetElementAmount(int amount)
        {
            this.SetElementAmount(amount, null);
        }

        public virtual void SetElementAmount(int amount, List<Vector2> elementsSize)
        {
            if (amount < 0)
            {
                amount = 0;
            }
            if ((elementsSize == null) || (amount == elementsSize.Count))
            {
                this.RecycleElement(false);
                this.m_elementAmount = amount;
                this.m_elementsSize = elementsSize;
                this.ProcessElements();
                this.ProcessUnUsedElement();
                if (this.m_ZeroTipsObj != null)
                {
                    if (amount <= 0)
                    {
                        this.m_ZeroTipsObj.SetActive(true);
                    }
                    else
                    {
                        this.m_ZeroTipsObj.SetActive(false);
                    }
                }
            }
        }

        public void SetElementSelectChangedEvent(enUIEventID eventID)
        {
            this.m_listSelectChangedEventID = eventID;
        }

        public void SetElementSelectChangedEvent(enUIEventID eventID, stUIEventParams eventParams)
        {
            this.m_listSelectChangedEventID = eventID;
            this.m_listSelectChangedEventParams = eventParams;
        }

        public void SetExternalElementPrefab(string externalElmtPrefPath)
        {
            CUIListElementScript component = null;
            GameObject content = null;
            if (externalElmtPrefPath != null)
            {
                content = Singleton<CResourceManager>.GetInstance().GetResource(externalElmtPrefPath, typeof(GameObject), enResourceType.UIPrefab, false, false).m_content;
                if (content != null)
                {
                    component = content.GetComponent<CUIListElementScript>();
                }
            }
            if ((component != null) && (content != null))
            {
                component.Initialize(base.m_belongedFormScript);
                this.m_elementTemplate = content;
                this.m_elementName = content.get_name();
                this.m_elementDefaultSize = component.m_defaultSize;
            }
            if (this.m_elementTemplate != null)
            {
                this.SetElementAmount(this.m_elementAmount);
            }
        }

        public void ShowExtraContent()
        {
            if ((this.m_extraContent != null) && this.m_isExtraContentEnabled)
            {
                this.m_extraContent.CustomSetActive(true);
            }
        }

        protected virtual void Update()
        {
            if ((base.m_belongedFormScript == null) || !base.m_belongedFormScript.IsClosed())
            {
                if (this.m_useOptimized)
                {
                    this.UpdateElementsScroll();
                }
                if (((this.m_scrollRect != null) && !this.m_scrollExternal) && !this.m_isDisableScrollRect)
                {
                    if ((this.m_contentSize.x > this.m_scrollAreaSize.x) || (this.m_contentSize.y > this.m_scrollAreaSize.y))
                    {
                        if (!this.m_scrollRect.get_enabled())
                        {
                            this.m_scrollRect.set_enabled(true);
                        }
                    }
                    else if (((Mathf.Abs(this.m_contentRectTransform.get_anchoredPosition().x) < 0.001) && (Mathf.Abs(this.m_contentRectTransform.get_anchoredPosition().y) < 0.001)) && this.m_scrollRect.get_enabled())
                    {
                        this.m_scrollRect.set_enabled(false);
                    }
                    this.DetectScroll();
                }
            }
        }

        protected void UpdateElementsScroll()
        {
            int num = 0;
            while (num < this.m_elementScripts.Count)
            {
                if (!this.IsRectInScrollArea(ref this.m_elementScripts[num].m_rect))
                {
                    this.RecycleElement(this.m_elementScripts[num], true);
                }
                else
                {
                    num++;
                }
            }
            for (int i = 0; i < this.m_elementAmount; i++)
            {
                stRect rect = this.m_elementsRect[i];
                if (this.IsRectInScrollArea(ref rect))
                {
                    bool flag = false;
                    for (int j = 0; j < this.m_elementScripts.Count; j++)
                    {
                        if (this.m_elementScripts[j].m_index == i)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        this.CreateElement(i, ref rect);
                    }
                }
            }
        }
    }
}

