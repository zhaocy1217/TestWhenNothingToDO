namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.UI;
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class UI3DEventMgr
    {
        private ListView<UI3DEventCom> m_evtComsEyes = new ListView<UI3DEventCom>();
        private ListView<UI3DEventCom> m_evtComsHeros = new ListView<UI3DEventCom>();
        private ListView<UI3DEventCom> m_evtComsTowers = new ListView<UI3DEventCom>();
        private const float SkillEventSizeScale = 3f;

        public void Clear()
        {
            this.m_evtComsHeros.Clear();
            this.m_evtComsTowers.Clear();
            this.m_evtComsEyes.Clear();
        }

        private void DispatchEvent(UI3DEventCom eventScript, PointerEventData pointerEventData)
        {
            if ((eventScript != null) && (pointerEventData != null))
            {
                CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
                uIEvent.m_eventID = eventScript.m_eventID;
                uIEvent.m_eventParams = eventScript.m_eventParams;
                uIEvent.m_pointerEventData = pointerEventData;
                if (Singleton<CUIEventManager>.GetInstance() != null)
                {
                    Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uIEvent);
                }
            }
        }

        public bool HandleClickEvent(PointerEventData pointerEventData)
        {
            for (int i = 0; i < this.m_evtComsHeros.Count; i++)
            {
                UI3DEventCom eventScript = this.m_evtComsHeros[i];
                if (((eventScript != null) && !eventScript.isDead) && eventScript.m_screenSize.Contains(pointerEventData.get_pressPosition()))
                {
                    this.DispatchEvent(eventScript, pointerEventData);
                    return true;
                }
            }
            for (int j = 0; j < this.m_evtComsTowers.Count; j++)
            {
                UI3DEventCom com2 = this.m_evtComsTowers[j];
                if (((com2 != null) && !com2.isDead) && com2.m_screenSize.Contains(pointerEventData.get_pressPosition()))
                {
                    this.DispatchEvent(com2, pointerEventData);
                    return true;
                }
            }
            return false;
        }

        public bool HandleSkillClickEvent(PointerEventData pointerEventData)
        {
            float maxValue = float.MaxValue;
            UI3DEventCom eventScript = null;
            for (int i = 0; i < this.m_evtComsTowers.Count; i++)
            {
                UI3DEventCom com2 = this.m_evtComsTowers[i];
                Rect screenSize = com2.m_screenSize;
                screenSize.set_size((Vector2) (screenSize.get_size() * 3f));
                float num3 = (screenSize.get_size().x <= screenSize.get_size().y) ? screenSize.get_size().y : screenSize.get_size().x;
                num3 *= num3;
                if (com2 != null)
                {
                    float num4 = (com2.m_screenSize.get_center() - pointerEventData.get_pressPosition()).get_sqrMagnitude();
                    if ((num3 >= num4) && (num4 < maxValue))
                    {
                        maxValue = num4;
                        eventScript = com2;
                    }
                }
            }
            for (int j = 0; j < this.m_evtComsEyes.Count; j++)
            {
                UI3DEventCom com3 = this.m_evtComsEyes[j];
                Rect rect2 = com3.m_screenSize;
                rect2.set_size((Vector2) (rect2.get_size() * 3f));
                float num6 = (rect2.get_size().x <= rect2.get_size().y) ? rect2.get_size().y : rect2.get_size().x;
                num6 *= num6;
                if (com3 != null)
                {
                    float num7 = (com3.m_screenSize.get_center() - pointerEventData.get_pressPosition()).get_sqrMagnitude();
                    if ((num6 >= num7) && (num7 < maxValue))
                    {
                        maxValue = num7;
                        eventScript = com3;
                    }
                }
            }
            if (eventScript != null)
            {
                this.DispatchEvent(eventScript, pointerEventData);
                return true;
            }
            return false;
        }

        public void Init()
        {
        }

        public void Register(UI3DEventCom com, EventComType comType)
        {
            com.isDead = false;
            switch (comType)
            {
                case EventComType.Hero:
                    if (!this.m_evtComsHeros.Contains(com))
                    {
                        this.m_evtComsHeros.Add(com);
                    }
                    break;

                case EventComType.Tower:
                    if (!this.m_evtComsTowers.Contains(com))
                    {
                        this.m_evtComsTowers.Add(com);
                    }
                    break;

                case EventComType.Eye:
                    if (!this.m_evtComsEyes.Contains(com))
                    {
                        this.m_evtComsEyes.Add(com);
                    }
                    break;
            }
        }

        public void Register(UI3DEventCom com, bool bHero)
        {
            com.isDead = false;
            if (bHero)
            {
                if (!this.m_evtComsHeros.Contains(com))
                {
                    this.m_evtComsHeros.Add(com);
                }
            }
            else if (!this.m_evtComsTowers.Contains(com))
            {
                this.m_evtComsTowers.Add(com);
            }
        }

        public void UnRegister(UI3DEventCom com)
        {
            this.m_evtComsHeros.Remove(com);
            this.m_evtComsTowers.Remove(com);
            this.m_evtComsEyes.Remove(com);
        }

        public int EyeComCount
        {
            get
            {
                return this.m_evtComsEyes.Count;
            }
        }

        public int HeroComCount
        {
            get
            {
                return this.m_evtComsHeros.Count;
            }
        }

        public int TowerComCount
        {
            get
            {
                return this.m_evtComsTowers.Count;
            }
        }

        public enum EventComType
        {
            Hero,
            Tower,
            Eye
        }
    }
}

