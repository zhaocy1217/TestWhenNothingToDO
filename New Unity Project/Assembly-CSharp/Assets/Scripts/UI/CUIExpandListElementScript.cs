namespace Assets.Scripts.UI
{
    using System;
    using UnityEngine;

    public class CUIExpandListElementScript : CUIListElementScript
    {
        [HideInInspector]
        public Vector2 m_expandedSize;
        public Vector2 m_retractedSize = new Vector2(-1f, -1f);

        public override void ChangeDisplay(bool selected)
        {
        }

        protected override Vector2 GetDefaultSize()
        {
            if (this.m_retractedSize.x <= 0f)
            {
                this.m_retractedSize.x = ((RectTransform) base.get_gameObject().get_transform()).get_rect().get_width();
            }
            if (this.m_retractedSize.y <= 0f)
            {
                this.m_retractedSize.y = ((RectTransform) base.get_gameObject().get_transform()).get_rect().get_height();
            }
            return this.m_retractedSize;
        }

        protected Vector2 GetExpandedSize()
        {
            return new Vector2((base.get_gameObject().get_transform() as RectTransform).get_rect().get_width(), (base.get_gameObject().get_transform() as RectTransform).get_rect().get_height());
        }

        public override void Initialize(CUIFormScript formScript)
        {
            if (!base.m_isInitialized)
            {
                this.m_expandedSize = this.GetExpandedSize();
                base.Initialize(formScript);
            }
        }
    }
}

