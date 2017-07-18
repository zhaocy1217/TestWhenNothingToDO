namespace Assets.Scripts.UI
{
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class CUI3DTouchEventScript : CUIMiniEventScript
    {
        public float m_3DTouchStrength = 4f;
        private Vector3[] m_corners = new Vector3[4];
        private bool m_isDown;
        private uint m_onTouchedEventDispatchedCount;
        [HideInInspector]
        public enUIEventID m_onTouchedEventID;
        [NonSerialized]
        public stUIEventParams m_onTouchedEventParams;
        public string[] m_onTouchedWwiseEvents = new string[0];
        private PointerEventData m_touchedPointerEventData;

        public override void Initialize(CUIFormScript formScript)
        {
            if (!base.m_isInitialized)
            {
                base.Initialize(formScript);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
        }
    }
}

