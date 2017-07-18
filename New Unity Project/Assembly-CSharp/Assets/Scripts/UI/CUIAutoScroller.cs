namespace Assets.Scripts.UI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class CUIAutoScroller : CUIComponent
    {
        public GameObject m_content;
        private RectTransform m_contentRectTransform;
        private bool m_isScrollRunning;
        public int m_loop = 1;
        private int m_loopCnt;
        public int m_scrollSpeed = 1;
        private const int TargetFrameRate = 30;

        private void DispatchScrollFinishEvent()
        {
            CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
            uIEvent.m_eventID = enUIEventID.UIComponent_AutoScroller_Scroll_Finish;
            uIEvent.m_srcFormScript = base.m_belongedFormScript;
            uIEvent.m_srcWidget = base.get_gameObject();
            uIEvent.m_srcWidgetScript = this;
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uIEvent);
        }

        public override void Initialize(CUIFormScript formScript)
        {
            if (!base.m_isInitialized)
            {
                if (this.m_content != null)
                {
                    this.m_contentRectTransform = this.m_content.get_transform() as RectTransform;
                }
                base.Initialize(formScript);
            }
        }

        public bool IsScrollRunning()
        {
            return this.m_isScrollRunning;
        }

        protected override void OnDestroy()
        {
            this.m_content = null;
            this.m_contentRectTransform = null;
            base.OnDestroy();
        }

        private void ResetContentTransform()
        {
            if (this.m_contentRectTransform != null)
            {
                this.m_contentRectTransform.set_pivot(new Vector2(0f, 0.5f));
                this.m_contentRectTransform.set_anchorMin(new Vector2(0f, 0.5f));
                this.m_contentRectTransform.set_anchorMax(new Vector2(0f, 0.5f));
                this.m_contentRectTransform.set_anchoredPosition(new Vector2((base.get_transform() as RectTransform).get_rect().get_width(), 0f));
            }
        }

        public void SetText(string contentText)
        {
            if (this.m_content != null)
            {
                Text component = this.m_content.GetComponent<Text>();
                if (component != null)
                {
                    component.set_text(contentText);
                }
            }
        }

        public void StartAutoScroll(bool bForce = false)
        {
            if (bForce || !this.m_isScrollRunning)
            {
                this.m_loopCnt = this.m_loop;
                this.m_isScrollRunning = true;
                this.ResetContentTransform();
                base.StartCoroutine("UpdateScroll");
            }
        }

        public void StopAutoScroll()
        {
            if (this.m_isScrollRunning)
            {
                this.m_isScrollRunning = false;
                base.StopCoroutine("UpdateScroll");
                this.ResetContentTransform();
            }
        }

        [DebuggerHidden]
        private IEnumerator UpdateScroll()
        {
            <UpdateScroll>c__Iterator36 iterator = new <UpdateScroll>c__Iterator36();
            iterator.<>f__this = this;
            return iterator;
        }

        [CompilerGenerated]
        private sealed class <UpdateScroll>c__Iterator36 : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal CUIAutoScroller <>f__this;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        if (this.<>f__this.m_contentRectTransform == null)
                        {
                            goto Label_0172;
                        }
                        break;

                    case 1:
                        break;

                    default:
                        goto Label_0179;
                }
                if (this.<>f__this.m_contentRectTransform.get_anchoredPosition().x > -this.<>f__this.m_contentRectTransform.get_rect().get_width())
                {
                    this.<>f__this.m_contentRectTransform.set_anchoredPosition(new Vector2(this.<>f__this.m_contentRectTransform.get_anchoredPosition().x - ((Time.get_deltaTime() * 30f) * this.<>f__this.m_scrollSpeed), this.<>f__this.m_contentRectTransform.get_anchoredPosition().y));
                    if (this.<>f__this.m_contentRectTransform.get_anchoredPosition().x <= -this.<>f__this.m_contentRectTransform.get_rect().get_width())
                    {
                        if (this.<>f__this.m_loopCnt > 0)
                        {
                            this.<>f__this.m_loopCnt--;
                        }
                        if (this.<>f__this.m_loopCnt != 0)
                        {
                            this.<>f__this.ResetContentTransform();
                        }
                    }
                    this.$current = null;
                    this.$PC = 1;
                    return true;
                }
                this.<>f__this.m_isScrollRunning = false;
                this.<>f__this.DispatchScrollFinishEvent();
            Label_0172:
                this.$PC = -1;
            Label_0179:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }
    }
}

