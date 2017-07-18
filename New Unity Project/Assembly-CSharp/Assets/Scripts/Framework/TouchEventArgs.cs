namespace Assets.Scripts.Framework
{
    using System;
    using UnityEngine;

    public class TouchEventArgs : EventArgs
    {
        private int Index;
        private Touch TouchEvent;

        public TouchEventArgs(Touch InTouch, int InIndex)
        {
            this.TouchEvent = InTouch;
            this.Index = InIndex;
        }

        public virtual Vector2 deltaPosition
        {
            get
            {
                return this.TouchEvent.get_deltaPosition();
            }
        }

        public virtual float deltaTime
        {
            get
            {
                return this.TouchEvent.get_deltaTime();
            }
        }

        public virtual int fingerId
        {
            get
            {
                return this.TouchEvent.get_fingerId();
            }
        }

        public virtual int index
        {
            get
            {
                return this.Index;
            }
        }

        public virtual TouchPhase phase
        {
            get
            {
                return this.TouchEvent.get_phase();
            }
        }

        public virtual Vector2 position
        {
            get
            {
                return this.TouchEvent.get_position();
            }
        }

        public virtual int tapCount
        {
            get
            {
                return this.TouchEvent.get_tapCount();
            }
        }
    }
}

