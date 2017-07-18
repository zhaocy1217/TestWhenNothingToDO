namespace Assets.Scripts.UI
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class CUIAnimationScript : CUIComponent
    {
        private Animation m_animation;
        private AnimationState m_currentAnimationState;
        private float m_currentAnimationTime;
        [HideInInspector]
        public enUIEventID[] m_eventIDs = new enUIEventID[Enum.GetValues(typeof(enAnimationEventType)).Length];
        public stUIEventParams[] m_eventParams = new stUIEventParams[Enum.GetValues(typeof(enAnimationEventType)).Length];

        public void DispatchAnimationEvent(enAnimationEventType animationEventType)
        {
            if (this.m_eventIDs[(int) animationEventType] != enUIEventID.None)
            {
                CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
                uIEvent.m_srcFormScript = base.m_belongedFormScript;
                uIEvent.m_srcWidget = base.get_gameObject();
                uIEvent.m_srcWidgetScript = this;
                uIEvent.m_srcWidgetBelongedListScript = base.m_belongedListScript;
                uIEvent.m_srcWidgetIndexInBelongedList = base.m_indexInlist;
                uIEvent.m_pointerEventData = null;
                uIEvent.m_eventID = this.m_eventIDs[(int) animationEventType];
                uIEvent.m_eventParams = this.m_eventParams[(int) animationEventType];
                base.DispatchUIEvent(uIEvent);
            }
        }

        public string GetCurrentAnimation()
        {
            return ((this.m_currentAnimationState != null) ? this.m_currentAnimationState.get_name() : null);
        }

        public override void Initialize(CUIFormScript formScript)
        {
            if (!base.m_isInitialized)
            {
                base.Initialize(formScript);
                this.m_animation = base.get_gameObject().GetComponent<Animation>();
                if (((this.m_animation != null) && this.m_animation.get_playAutomatically()) && (this.m_animation.get_clip() != null))
                {
                    this.m_currentAnimationState = this.m_animation.get_Item(this.m_animation.get_clip().get_name());
                    this.m_currentAnimationTime = 0f;
                    this.DispatchAnimationEvent(enAnimationEventType.AnimationStart);
                }
            }
        }

        public bool IsAnimationStopped(string animationName)
        {
            if ((!string.IsNullOrEmpty(animationName) && (this.m_currentAnimationState != null)) && (this.m_currentAnimationTime != 0f))
            {
                return !string.Equals(this.m_currentAnimationState.get_name(), animationName);
            }
            return true;
        }

        protected override void OnDestroy()
        {
            this.m_animation = null;
            this.m_currentAnimationState = null;
            base.OnDestroy();
        }

        public void PlayAnimation(string animName, bool forceRewind)
        {
            if (((this.m_currentAnimationState == null) || !this.m_currentAnimationState.get_name().Equals(animName)) || forceRewind)
            {
                if (this.m_currentAnimationState != null)
                {
                    this.m_animation.Stop(this.m_currentAnimationState.get_name());
                    this.m_currentAnimationState = null;
                    this.m_currentAnimationTime = 0f;
                }
                this.m_currentAnimationState = this.m_animation.get_Item(animName);
                this.m_currentAnimationTime = 0f;
                if (this.m_currentAnimationState != null)
                {
                    this.m_animation.Play(animName);
                    this.DispatchAnimationEvent(enAnimationEventType.AnimationStart);
                }
            }
        }

        public void SetAnimationEvent(enAnimationEventType animationEventType, enUIEventID eventId, [Optional] stUIEventParams eventParams)
        {
            this.m_eventIDs[(int) animationEventType] = eventId;
            this.m_eventParams[(int) animationEventType] = eventParams;
        }

        public void SetAnimationSpeed(string animName, float speed)
        {
            if ((this.m_animation != null) && (this.m_animation.get_Item(animName) != null))
            {
                this.m_animation.get_Item(animName).set_speed(speed);
            }
        }

        public void StopAnimation(string animName)
        {
            if ((this.m_currentAnimationState != null) && this.m_currentAnimationState.get_name().Equals(animName))
            {
                this.m_animation.Stop(animName);
                this.DispatchAnimationEvent(enAnimationEventType.AnimationEnd);
                this.m_currentAnimationState = null;
                this.m_currentAnimationTime = 0f;
            }
        }

        private void Update()
        {
            if ((((base.m_belongedFormScript == null) || !base.m_belongedFormScript.IsClosed()) && (this.m_currentAnimationState != null)) && (((this.m_currentAnimationState.get_wrapMode() != 2) && (this.m_currentAnimationState.get_wrapMode() != 4)) && (this.m_currentAnimationState.get_wrapMode() != 8)))
            {
                if (this.m_currentAnimationTime >= this.m_currentAnimationState.get_length())
                {
                    this.DispatchAnimationEvent(enAnimationEventType.AnimationEnd);
                    this.m_currentAnimationState = null;
                    this.m_currentAnimationTime = 0f;
                }
                else
                {
                    this.m_currentAnimationTime += Time.get_deltaTime();
                }
            }
        }
    }
}

