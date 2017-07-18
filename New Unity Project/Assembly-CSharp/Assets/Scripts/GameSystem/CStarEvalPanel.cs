namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.UI;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class CStarEvalPanel
    {
        private GameObject arrowIcon;
        private bool bPanelOpen;
        private Text[] conditionTexts = new Text[3];
        private GameObject m_Obj;
        private Image PanelIcon;
        private GameObject taskPanel;

        public void Clear()
        {
            this.m_Obj = null;
            this.taskPanel = null;
            this.arrowIcon = null;
            this.PanelIcon = null;
            this.conditionTexts = new Text[3];
            this.DeinitEvent();
        }

        private void DeinitEvent()
        {
            Singleton<StarSystem>.GetInstance().OnEvaluationChanged -= new OnEvaluationChangedDelegate(this.OnEvaluationChange);
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Adv_OpenTaskPanel, new CUIEventManager.OnUIEventHandler(this.openTaskPanel));
            Singleton<EventRouter>.instance.RemoveEventHandler(EventID.StarSystemInitialized, new Action(this, (IntPtr) this.reset));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_TaskPanel_SlideEnd, new CUIEventManager.OnUIEventHandler(this.onTaskPanelSlideEnd));
        }

        public void Hide()
        {
            if (this.m_Obj != null)
            {
                this.m_Obj.get_gameObject().CustomSetActive(false);
            }
        }

        public void Init(GameObject obj)
        {
            this.m_Obj = obj;
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            if ((curLvelContext.IsMobaMode() || curLvelContext.IsGameTypeGuide()) || (curLvelContext.IsGameTypeBurning() || curLvelContext.IsGameTypeArena()))
            {
                this.Hide();
            }
            else
            {
                this.Show();
                this.bPanelOpen = false;
                this.taskPanel = this.m_Obj.get_transform().Find("TaskPanel").get_gameObject();
                this.arrowIcon = this.m_Obj.get_transform().Find("Image").get_gameObject();
                this.taskPanel.CustomSetActive(false);
                this.PanelIcon = this.m_Obj.get_transform().Find("icon").GetComponent<Image>();
                this.initEvent();
                UT.If_Null_Error<GameObject>(this.m_Obj);
                UT.If_Null_Error<GameObject>(this.taskPanel);
                UT.If_Null_Error<Image>(this.PanelIcon);
            }
        }

        private void initEvent()
        {
            Singleton<StarSystem>.GetInstance().OnEvaluationChanged += new OnEvaluationChangedDelegate(this.OnEvaluationChange);
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Adv_OpenTaskPanel, new CUIEventManager.OnUIEventHandler(this.openTaskPanel));
            Singleton<EventRouter>.instance.AddEventHandler(EventID.StarSystemInitialized, new Action(this, (IntPtr) this.reset));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_TaskPanel_SlideEnd, new CUIEventManager.OnUIEventHandler(this.onTaskPanelSlideEnd));
        }

        public void OnEvaluationChange(IStarEvaluation InStarEvaluation, IStarCondition InStarCondition)
        {
            if (this.conditionTexts[InStarEvaluation.index] != null)
            {
                this.conditionTexts[InStarEvaluation.index].set_text(InStarEvaluation.description);
            }
            Transform transform = this.m_Obj.get_transform().Find("TaskPanel").get_transform();
            if (transform != null)
            {
                if (InStarEvaluation.isSuccess)
                {
                    transform.Find(string.Format("Condition{0}", InStarEvaluation.index + 1)).GetComponent<Text>().set_color(Color.get_green());
                }
                else
                {
                    transform.Find(string.Format("Condition{0}", InStarEvaluation.index + 1)).GetComponent<Text>().set_color(Color.get_white());
                }
            }
        }

        private void onTaskPanelSlideEnd(CUIEvent uiEvt)
        {
            if (uiEvt.m_eventParams.tag == 5)
            {
                this.taskPanel.CustomSetActive(false);
                this.arrowIcon.CustomSetActive(false);
            }
        }

        public void openTaskPanel(CUIEvent uiEvent)
        {
            if (!this.bPanelOpen)
            {
                this.taskPanel.CustomSetActive(true);
                this.arrowIcon.CustomSetActive(true);
            }
            stUIEventParams eventParams = new stUIEventParams();
            eventParams.tag = !this.bPanelOpen ? 6 : 5;
            this.taskPanel.GetComponent<CUIAnimatorScript>().SetUIEvent(enAnimatorEventType.AnimatorEnd, enUIEventID.Battle_TaskPanel_SlideEnd, eventParams);
            this.taskPanel.GetComponent<Animator>().Play(!this.bPanelOpen ? "Form_Battle_EvalPlanel_in" : "Form_Battle_EvalPlanel_out");
            this.arrowIcon.GetComponent<Animator>().Play(!this.bPanelOpen ? "Form_Battle_EvalPlanel_in2" : "Form_Battle_EvalPlanel_out2");
            this.bPanelOpen = !this.bPanelOpen;
        }

        public void reset()
        {
            Transform transform = this.m_Obj.get_transform().Find("TaskPanel").get_transform();
            ListView<IStarEvaluation>.Enumerator enumerator = Singleton<StarSystem>.GetInstance().GetEnumerator();
            while (enumerator.MoveNext())
            {
                this.conditionTexts[enumerator.Current.index] = transform.Find(string.Format("Condition{0}", enumerator.Current.index + 1)).GetComponent<Text>();
                this.conditionTexts[enumerator.Current.index].set_text(enumerator.Current.description);
                if (enumerator.Current.isSuccess)
                {
                    this.conditionTexts[enumerator.Current.index].set_color(Color.get_green());
                }
                else
                {
                    this.conditionTexts[enumerator.Current.index].set_color(Color.get_white());
                }
            }
            UT.If_Null_Error<Text[]>(this.conditionTexts);
        }

        public void Show()
        {
            if (this.m_Obj != null)
            {
                this.m_Obj.get_gameObject().CustomSetActive(true);
            }
        }
    }
}

