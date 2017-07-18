namespace Assets.Scripts.UI
{
    using System;
    using UnityEngine;

    public class CUICanvasScript : CUIComponent
    {
        private Canvas m_Canvas;
        public bool m_isNeedMaskParticle;

        public override void Appear()
        {
            base.Appear();
            CUIUtility.SetGameObjectLayer(base.get_gameObject(), 5);
        }

        public override void Hide()
        {
            base.Hide();
            CUIUtility.SetGameObjectLayer(base.get_gameObject(), 0x1f);
        }

        public override void Initialize(CUIFormScript formScript)
        {
            if (!base.m_isInitialized)
            {
                this.m_Canvas = base.GetComponent<Canvas>();
                base.Initialize(formScript);
            }
        }

        public override void SetSortingOrder(int sortingOrder)
        {
            if ((this.m_Canvas != null) && this.m_isNeedMaskParticle)
            {
                this.m_Canvas.set_overrideSorting(true);
                this.m_Canvas.set_sortingOrder(sortingOrder + 1);
            }
        }
    }
}

