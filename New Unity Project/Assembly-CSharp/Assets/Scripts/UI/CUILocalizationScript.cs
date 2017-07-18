namespace Assets.Scripts.UI
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class CUILocalizationScript : CUIComponent
    {
        [HideInInspector]
        public string m_key;
        private Text m_text;

        public override void Initialize(CUIFormScript formScript)
        {
            if (!base.m_isInitialized)
            {
                base.Initialize(formScript);
                this.m_text = base.get_gameObject().GetComponent<Text>();
                this.SetDisplay();
            }
        }

        protected override void OnDestroy()
        {
            this.m_text = null;
            base.OnDestroy();
        }

        public void SetDisplay()
        {
            if (((this.m_text != null) && !string.IsNullOrEmpty(this.m_key)) && Singleton<CTextManager>.GetInstance().IsTextLoaded())
            {
                this.m_text.set_text(Singleton<CTextManager>.GetInstance().GetText(this.m_key));
            }
        }

        public void SetKey(string key)
        {
            this.m_key = key;
            this.SetDisplay();
        }
    }
}

