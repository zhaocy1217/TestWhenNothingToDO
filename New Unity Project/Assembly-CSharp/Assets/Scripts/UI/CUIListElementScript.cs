namespace Assets.Scripts.UI
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class CUIListElementScript : CUIComponent
    {
        public bool m_autoAddUIEventScript = true;
        private CanvasGroup m_canvasGroup;
        private string m_dataTag;
        [HideInInspector]
        public Color m_defaultColor;
        [HideInInspector]
        public ImageAlphaTexLayout m_defaultLayout;
        [HideInInspector]
        public Vector2 m_defaultSize;
        [HideInInspector]
        public Sprite m_defaultSprite;
        [HideInInspector]
        public Color m_defaultTextColor;
        private Image m_image;
        [HideInInspector]
        public int m_index;
        [HideInInspector]
        public enUIEventID m_onEnableEventID;
        public stUIEventParams m_onEnableEventParams;
        [HideInInspector]
        public enPivotType m_pivotType = enPivotType.LeftTop;
        public stRect m_rect;
        public ImageAlphaTexLayout m_selectedLayout;
        public Sprite m_selectedSprite;
        public GameObject m_selectFrontObj;
        public Color m_selectTextColor = new Color(1f, 1f, 1f, 1f);
        public Text m_textObj;
        public bool m_useSetActiveForDisplay = true;
        public OnSelectedDelegate onSelected;

        public virtual void ChangeDisplay(bool selected)
        {
            if ((this.m_image != null) && (this.m_selectedSprite != null))
            {
                if (selected)
                {
                    this.m_image.set_sprite(this.m_selectedSprite);
                    this.m_image.set_color(new Color(this.m_defaultColor.r, this.m_defaultColor.g, this.m_defaultColor.b, 255f));
                }
                else
                {
                    this.m_image.set_sprite(this.m_defaultSprite);
                    this.m_image.set_color(this.m_defaultColor);
                }
                if (this.m_image is Image2)
                {
                    Image2 image = this.m_image as Image2;
                    image.alphaTexLayout = !selected ? this.m_defaultLayout : this.m_selectedLayout;
                }
            }
            if (this.m_selectFrontObj != null)
            {
                this.m_selectFrontObj.CustomSetActive(selected);
            }
            if (this.m_textObj != null)
            {
                this.m_textObj.set_color(!selected ? this.m_defaultTextColor : this.m_selectTextColor);
            }
        }

        public void Disable()
        {
            if (this.m_useSetActiveForDisplay)
            {
                base.get_gameObject().CustomSetActive(false);
            }
            else
            {
                this.m_canvasGroup.set_alpha(0f);
                this.m_canvasGroup.set_blocksRaycasts(false);
            }
        }

        protected void DispatchOnEnableEvent()
        {
            if (this.m_onEnableEventID != enUIEventID.None)
            {
                CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
                uIEvent.m_eventID = this.m_onEnableEventID;
                uIEvent.m_eventParams = this.m_onEnableEventParams;
                uIEvent.m_srcFormScript = base.m_belongedFormScript;
                uIEvent.m_srcWidgetBelongedListScript = base.m_belongedListScript;
                uIEvent.m_srcWidgetIndexInBelongedList = base.m_indexInlist;
                uIEvent.m_srcWidget = base.get_gameObject();
                uIEvent.m_srcWidgetScript = this;
                uIEvent.m_pointerEventData = null;
                base.DispatchUIEvent(uIEvent);
            }
        }

        public void Enable(CUIListScript belongedList, int index, string name, ref stRect rect, bool selected)
        {
            base.m_belongedListScript = belongedList;
            this.m_index = index;
            base.get_gameObject().set_name(name + "_" + index.ToString());
            if (this.m_useSetActiveForDisplay)
            {
                base.get_gameObject().CustomSetActive(true);
            }
            else
            {
                this.m_canvasGroup.set_alpha(1f);
                this.m_canvasGroup.set_blocksRaycasts(true);
            }
            this.SetComponentBelongedList(base.get_gameObject());
            this.SetRect(ref rect);
            this.ChangeDisplay(selected);
            this.DispatchOnEnableEvent();
        }

        public string GetDataTag()
        {
            return this.m_dataTag;
        }

        protected virtual Vector2 GetDefaultSize()
        {
            return new Vector2(((RectTransform) base.get_gameObject().get_transform()).get_rect().get_width(), ((RectTransform) base.get_gameObject().get_transform()).get_rect().get_height());
        }

        public override void Initialize(CUIFormScript formScript)
        {
            if (!base.m_isInitialized)
            {
                base.Initialize(formScript);
                this.m_image = base.get_gameObject().GetComponent<Image>();
                if (this.m_image != null)
                {
                    this.m_defaultSprite = this.m_image.get_sprite();
                    this.m_defaultColor = this.m_image.get_color();
                    if (this.m_image is Image2)
                    {
                        Image2 image = this.m_image as Image2;
                        this.m_defaultLayout = image.alphaTexLayout;
                    }
                }
                if (this.m_autoAddUIEventScript && (base.get_gameObject().GetComponent<CUIEventScript>() == null))
                {
                    base.get_gameObject().AddComponent<CUIEventScript>().Initialize(formScript);
                }
                if (!this.m_useSetActiveForDisplay)
                {
                    this.m_canvasGroup = base.get_gameObject().GetComponent<CanvasGroup>();
                    if (this.m_canvasGroup == null)
                    {
                        this.m_canvasGroup = base.get_gameObject().AddComponent<CanvasGroup>();
                    }
                }
                this.m_defaultSize = this.GetDefaultSize();
                this.InitRectTransform();
                if (this.m_textObj != null)
                {
                    this.m_defaultTextColor = this.m_textObj.get_color();
                }
            }
        }

        private void InitRectTransform()
        {
            RectTransform transform = base.get_gameObject().get_transform() as RectTransform;
            transform.set_anchorMin(new Vector2(0f, 1f));
            transform.set_anchorMax(new Vector2(0f, 1f));
            transform.set_pivot((this.m_pivotType != enPivotType.Centre) ? new Vector2(0f, 1f) : new Vector2(0.5f, 0.5f));
            transform.set_sizeDelta(this.m_defaultSize);
            transform.set_localRotation(Quaternion.get_identity());
            transform.set_localScale(new Vector3(1f, 1f, 1f));
        }

        protected override void OnDestroy()
        {
            this.m_selectFrontObj = null;
            this.m_selectedSprite = null;
            this.m_defaultSprite = null;
            this.m_textObj = null;
            this.m_image = null;
            this.onSelected = null;
            this.m_canvasGroup = null;
            base.OnDestroy();
        }

        public void OnSelected(BaseEventData baseEventData)
        {
            base.m_belongedListScript.SelectElement(this.m_index, true);
        }

        public void SetComponentBelongedList(GameObject gameObject)
        {
            CUIComponent[] components = gameObject.GetComponents<CUIComponent>();
            if ((components != null) && (components.Length > 0))
            {
                for (int j = 0; j < components.Length; j++)
                {
                    components[j].SetBelongedList(base.m_belongedListScript, this.m_index);
                }
            }
            for (int i = 0; i < gameObject.get_transform().get_childCount(); i++)
            {
                this.SetComponentBelongedList(gameObject.get_transform().GetChild(i).get_gameObject());
            }
        }

        public void SetDataTag(string dataTag)
        {
            this.m_dataTag = dataTag;
        }

        public void SetOnEnableEvent(enUIEventID eventID)
        {
            this.m_onEnableEventID = eventID;
        }

        public void SetOnEnableEvent(enUIEventID eventID, stUIEventParams eventParams)
        {
            this.m_onEnableEventID = eventID;
            this.m_onEnableEventParams = eventParams;
        }

        public void SetRect(ref stRect rect)
        {
            this.m_rect = rect;
            RectTransform transform = base.get_gameObject().get_transform() as RectTransform;
            transform.set_sizeDelta(new Vector2((float) this.m_rect.m_width, (float) this.m_rect.m_height));
            if (this.m_pivotType == enPivotType.Centre)
            {
                transform.set_anchoredPosition(rect.m_center);
            }
            else
            {
                transform.set_anchoredPosition(new Vector2((float) rect.m_left, (float) rect.m_top));
            }
        }

        public delegate void OnSelectedDelegate();
    }
}

