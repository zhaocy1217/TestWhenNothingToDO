namespace Assets.Scripts.UI
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class CUISliderEventScript : CUIComponent
    {
        private int m_DescribeCount;
        private Text[] m_Describes;
        private Text m_Handletext;
        [HideInInspector]
        public enUIEventID m_onValueChangedEventID;
        private Slider m_slider;
        private float m_value;

        private void DispatchSliderEvent()
        {
            if (this.m_onValueChangedEventID != enUIEventID.None)
            {
                CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
                uIEvent.m_srcFormScript = base.m_belongedFormScript;
                uIEvent.m_srcWidget = base.get_gameObject();
                uIEvent.m_srcWidgetScript = this;
                uIEvent.m_srcWidgetBelongedListScript = base.m_belongedListScript;
                uIEvent.m_srcWidgetIndexInBelongedList = base.m_indexInlist;
                uIEvent.m_pointerEventData = null;
                uIEvent.m_eventID = this.m_onValueChangedEventID;
                uIEvent.m_eventParams.sliderValue = this.value;
                base.DispatchUIEvent(uIEvent);
            }
        }

        public Slider GetSlider()
        {
            return this.m_slider;
        }

        public override void Initialize(CUIFormScript formScript)
        {
            if (!base.m_isInitialized)
            {
                this.m_slider = base.get_gameObject().GetComponent<Slider>();
                if (this.m_slider != null)
                {
                    this.m_DescribeCount = base.get_transform().Find("Background").get_childCount();
                    this.m_Describes = new Text[this.m_DescribeCount];
                    for (int i = 0; i < this.m_DescribeCount; i++)
                    {
                        this.m_Describes[i] = base.get_transform().Find(string.Format("Background/Text{0}", i + 1)).GetComponent<Text>();
                    }
                    this.m_Handletext = base.get_transform().Find("Handle Slide Area/Handle/Text").GetComponent<Text>();
                    this.m_slider.get_onValueChanged().RemoveAllListeners();
                    this.m_slider.get_onValueChanged().AddListener(new UnityAction<float>(this, (IntPtr) this.OnSliderValueChanged));
                    base.Initialize(formScript);
                }
            }
        }

        protected override void OnDestroy()
        {
            this.m_slider = null;
            this.m_Describes = null;
            this.m_Handletext = null;
            base.OnDestroy();
        }

        private void OnSliderValueChanged(float value)
        {
            if ((value >= 0f) && (value < this.m_DescribeCount))
            {
                this.value = value;
                this.DispatchSliderEvent();
            }
        }

        public bool Enabled
        {
            get
            {
                return ((this.m_slider != null) && this.m_slider.get_interactable());
            }
            set
            {
                if (this.m_slider != null)
                {
                    this.m_slider.set_interactable(value);
                }
            }
        }

        public int MaxValue
        {
            get
            {
                if (this.m_slider != null)
                {
                    return (int) this.m_slider.get_maxValue();
                }
                return 0;
            }
        }

        public float value
        {
            get
            {
                if (this.m_slider != null)
                {
                    return this.m_slider.get_value();
                }
                return -1f;
            }
            set
            {
                this.m_value = value;
                if (((this.m_slider != null) && (this.m_value <= this.m_slider.get_maxValue())) && (this.m_value >= 0f))
                {
                    this.m_slider.set_value(this.m_value);
                    this.m_Handletext.set_text(this.m_Describes[(int) this.m_value].get_text());
                }
            }
        }
    }
}

