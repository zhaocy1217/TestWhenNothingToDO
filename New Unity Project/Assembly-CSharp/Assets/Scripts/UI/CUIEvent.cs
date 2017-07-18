namespace Assets.Scripts.UI
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class CUIEvent
    {
        [CompilerGenerated]
        private int <m_srcWidgetIndexInBelongedList>k__BackingField;
        public enUIEventID m_eventID;
        public stUIEventParams m_eventParams;
        public bool m_inUse;
        public PointerEventData m_pointerEventData;
        public CUIFormScript m_srcFormScript;
        public GameObject m_srcWidget;
        public CUIListScript m_srcWidgetBelongedListScript;
        public CUIComponent m_srcWidgetScript;

        public void Clear()
        {
            this.m_srcFormScript = null;
            this.m_srcWidget = null;
            this.m_srcWidgetScript = null;
            this.m_srcWidgetBelongedListScript = null;
            this.m_srcWidgetIndexInBelongedList = -1;
            this.m_pointerEventData = null;
            this.m_eventID = enUIEventID.None;
            this.m_inUse = false;
        }

        public int m_srcWidgetIndexInBelongedList
        {
            [CompilerGenerated]
            get
            {
                return this.<m_srcWidgetIndexInBelongedList>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.<m_srcWidgetIndexInBelongedList>k__BackingField = value;
            }
        }
    }
}

