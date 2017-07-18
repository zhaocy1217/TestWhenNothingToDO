namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    internal class CSettingsSlider
    {
        [CompilerGenerated]
        private static SliderValueChanged <>f__am$cache8;
        private GameObject m_BarObj;
        private int m_DescribeCount;
        private Text[] m_Describes;
        private Text m_Handletext;
        private Slider m_Slider;
        private enSliderKind m_SliderKind;
        private int m_value;
        public SliderValueChanged onValueChangedHander;

        public CSettingsSlider(SliderValueChanged valueChangeDelegate)
        {
            if (<>f__am$cache8 == null)
            {
                <>f__am$cache8 = new SliderValueChanged(CSettingsSlider.<CSettingsSlider>m__85);
            }
            this.onValueChangedHander = <>f__am$cache8;
            this.onValueChangedHander = (SliderValueChanged) Delegate.Combine(this.onValueChangedHander, valueChangeDelegate);
        }

        [CompilerGenerated]
        private static void <CSettingsSlider>m__85(int, enSliderKind)
        {
        }

        public void initPanel(GameObject barObj, enSliderKind kind)
        {
            this.m_BarObj = barObj;
            this.m_SliderKind = kind;
            this.m_DescribeCount = barObj.get_transform().Find("Slider/Background").get_childCount();
            this.m_Describes = new Text[this.m_DescribeCount];
            for (int i = 0; i < this.m_DescribeCount; i++)
            {
                this.m_Describes[i] = barObj.get_transform().Find(string.Format("Slider/Background/Text{0}", i + 1)).GetComponent<Text>();
            }
            this.m_Handletext = this.m_BarObj.get_transform().Find("Slider/Handle Slide Area/Handle/Text").GetComponent<Text>();
            this.m_Slider = this.m_BarObj.get_transform().Find("Slider").GetComponent<Slider>();
            this.m_Slider.get_onValueChanged().RemoveAllListeners();
            this.m_Slider.get_onValueChanged().AddListener(new UnityAction<float>(this, (IntPtr) this.onSliderChange));
        }

        public void onSliderChange(float value)
        {
            if (((this.m_SliderKind == enSliderKind.Slider_MoveCamera) && (GameSettings.TheCommonAttackType == CommonAttactType.Type2)) && (((int) value) == 1))
            {
                value = 2f;
            }
            if ((value >= 0f) && (value < this.m_DescribeCount))
            {
                this.value = (int) value;
                this.onValueChangedHander(this.value, this.m_SliderKind);
            }
        }

        public bool Enabled
        {
            get
            {
                return ((this.m_Slider != null) && this.m_Slider.get_interactable());
            }
            set
            {
                if (this.m_Slider != null)
                {
                    this.m_Slider.set_interactable(value);
                }
            }
        }

        public int MaxValue
        {
            get
            {
                if (this.m_Slider != null)
                {
                    return (int) this.m_Slider.get_maxValue();
                }
                return 0;
            }
        }

        public int value
        {
            get
            {
                if (this.m_Slider != null)
                {
                    return (int) this.m_Slider.get_value();
                }
                return -1;
            }
            set
            {
                this.m_value = value;
                if (((this.m_Slider != null) && (this.m_value <= this.m_Slider.get_maxValue())) && (this.m_value >= 0))
                {
                    this.m_Slider.set_value((float) this.m_value);
                    this.m_Handletext.set_text(this.m_Describes[this.m_value].get_text());
                }
            }
        }
    }
}

