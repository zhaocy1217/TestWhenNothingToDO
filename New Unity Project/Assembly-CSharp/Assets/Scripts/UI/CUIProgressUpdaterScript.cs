namespace Assets.Scripts.UI
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class CUIProgressUpdaterScript : CUIComponent
    {
        [Range(0f, 1f)]
        public float m_endFillAmount = 1f;
        public float m_fillAmountPerSecond = 1f;
        public enFillDirection m_fillDirection;
        [HideInInspector]
        public enUIEventID m_fillEndEventID;
        private float m_fillRate;
        private Image m_image;
        private bool m_isRunning;
        [Range(0f, 1f)]
        public float m_startFillAmount;
        [HideInInspector]
        private float m_targetFillAmount;

        private void DispatchFillEndEvent()
        {
            if (this.m_fillEndEventID != enUIEventID.None)
            {
                CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
                uIEvent.m_srcFormScript = base.m_belongedFormScript;
                uIEvent.m_srcWidget = base.get_gameObject();
                uIEvent.m_srcWidgetScript = this;
                uIEvent.m_srcWidgetBelongedListScript = base.m_belongedListScript;
                uIEvent.m_srcWidgetIndexInBelongedList = base.m_indexInlist;
                uIEvent.m_pointerEventData = null;
                uIEvent.m_eventID = this.m_fillEndEventID;
                uIEvent.m_eventParams = new stUIEventParams();
                base.DispatchUIEvent(uIEvent);
            }
        }

        public override void Initialize(CUIFormScript formScript)
        {
            if (!base.m_isInitialized)
            {
                this.m_image = base.get_gameObject().GetComponent<Image>();
                if ((this.m_image != null) && (this.m_image.get_type() == 3))
                {
                    if (this.m_startFillAmount >= this.m_endFillAmount)
                    {
                    }
                    this.m_fillRate = (this.m_endFillAmount - this.m_startFillAmount) / 1f;
                    this.m_isRunning = false;
                    base.Initialize(formScript);
                }
            }
        }

        protected override void OnDestroy()
        {
            this.m_image = null;
            base.OnDestroy();
        }

        public void StartFill(float targetFillAmount, enFillDirection fillDirection = 0, float curFillAmount = -1f)
        {
            this.m_targetFillAmount = Mathf.Clamp(targetFillAmount, this.m_startFillAmount, this.m_endFillAmount);
            this.m_fillDirection = fillDirection;
            if (curFillAmount >= 0f)
            {
                this.m_image.set_fillAmount(curFillAmount);
            }
            this.m_isRunning = true;
        }

        protected virtual void Update()
        {
            if ((this.m_isRunning && (this.m_image != null)) && (this.m_image.get_type() == 3))
            {
                if (this.m_fillDirection == enFillDirection.Clockwise)
                {
                    float num = this.m_image.get_fillAmount() + ((this.m_fillAmountPerSecond * this.m_fillRate) * Time.get_deltaTime());
                    this.m_image.set_fillAmount(num);
                    if (this.m_image.get_fillAmount() >= this.m_targetFillAmount)
                    {
                        this.m_isRunning = false;
                        this.DispatchFillEndEvent();
                    }
                }
                else if (this.m_fillDirection == enFillDirection.CounterClockwise)
                {
                    float num2 = this.m_image.get_fillAmount() - ((this.m_fillAmountPerSecond * this.m_fillRate) * Time.get_deltaTime());
                    this.m_image.set_fillAmount(num2);
                    if (this.m_image.get_fillAmount() <= this.m_targetFillAmount)
                    {
                        this.m_isRunning = false;
                        this.DispatchFillEndEvent();
                    }
                }
            }
        }

        public enum enFillDirection
        {
            Clockwise,
            CounterClockwise
        }
    }
}

