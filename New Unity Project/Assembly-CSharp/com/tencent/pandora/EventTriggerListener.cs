namespace com.tencent.pandora
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class EventTriggerListener : EventTrigger
    {
        public VoidDelegate onClick;
        public VoidDelegate onDown;
        public VoidDelegate onUp;

        public static EventTriggerListener Get(GameObject go)
        {
            EventTriggerListener component = go.GetComponent<EventTriggerListener>();
            if (component == null)
            {
                component = go.AddComponent<EventTriggerListener>();
            }
            return component;
        }

        public static EventTriggerListener Get(Transform transform)
        {
            EventTriggerListener component = transform.GetComponent<EventTriggerListener>();
            if (component == null)
            {
                component = transform.get_gameObject().AddComponent<EventTriggerListener>();
            }
            return component;
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (this.onClick != null)
            {
                this.onClick(base.get_gameObject());
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (this.onDown != null)
            {
                this.onDown(base.get_gameObject());
            }
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (this.onUp != null)
            {
                this.onUp(base.get_gameObject());
            }
        }

        public delegate void VoidDelegate(GameObject go);
    }
}

