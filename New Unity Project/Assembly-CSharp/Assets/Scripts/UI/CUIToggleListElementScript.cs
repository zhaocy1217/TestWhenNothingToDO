namespace Assets.Scripts.UI
{
    using System;
    using UnityEngine.UI;

    public class CUIToggleListElementScript : CUIListElementScript
    {
        private Toggle m_toggle;

        public override void ChangeDisplay(bool selected)
        {
            base.ChangeDisplay(selected);
            if (this.m_toggle != null)
            {
                this.m_toggle.set_isOn(selected);
            }
        }

        public override void Initialize(CUIFormScript formScript)
        {
            if (!base.m_isInitialized)
            {
                base.Initialize(formScript);
                this.m_toggle = base.GetComponentInChildren<Toggle>(base.get_gameObject());
                if (this.m_toggle != null)
                {
                    this.m_toggle.set_interactable(false);
                }
            }
        }

        protected override void OnDestroy()
        {
            this.m_toggle = null;
            base.OnDestroy();
        }
    }
}

