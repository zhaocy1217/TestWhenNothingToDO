namespace Assets.Scripts.UI
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class CUIToggleEventScript : CUIComponent
    {
        [HideInInspector]
        public enUIEventID m_onValueChangedEventID;
        private Toggle m_toggle;

        public override void Initialize(CUIFormScript formScript)
        {
            if (!base.m_isInitialized)
            {
                this.m_toggle = base.get_gameObject().GetComponent<Toggle>();
                this.m_toggle.onValueChanged.RemoveAllListeners();
                this.m_toggle.onValueChanged.AddListener(new UnityAction<bool>(this, (IntPtr) this.OnToggleValueChanged));
                Transform transform = base.get_gameObject().get_transform().FindChild("Label");
                if (transform != null)
                {
                    if (this.m_toggle.get_isOn())
                    {
                        transform.GetComponent<Text>().set_color(CUIUtility.s_Color_White);
                    }
                    else
                    {
                        transform.GetComponent<Text>().set_color(CUIUtility.s_Text_Color_ListElement_Normal);
                    }
                }
                base.Initialize(formScript);
            }
        }

        protected override void OnDestroy()
        {
            this.m_toggle = null;
            base.OnDestroy();
        }

        private void OnToggleValueChanged(bool isOn)
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
                uIEvent.m_eventParams.togleIsOn = isOn;
                Transform transform = base.get_gameObject().get_transform().FindChild("Label");
                if (transform != null)
                {
                    if (isOn)
                    {
                        transform.GetComponent<Text>().set_color(CUIUtility.s_Color_White);
                    }
                    else
                    {
                        transform.GetComponent<Text>().set_color(CUIUtility.s_Text_Color_ListElement_Normal);
                    }
                }
                base.DispatchUIEvent(uIEvent);
            }
        }
    }
}

