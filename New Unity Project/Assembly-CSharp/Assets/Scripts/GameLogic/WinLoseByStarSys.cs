namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Framework;
    using ResData;
    using System;
    using System.Runtime.CompilerServices;

    public class WinLoseByStarSys : Singleton<WinLoseByStarSys>
    {
        [CompilerGenerated]
        private bool <bStarted>k__BackingField;
        [CompilerGenerated]
        private uint <CurLevelTimeDuration>k__BackingField;
        public IStarEvaluation LoserEvaluation;
        public IStarEvaluation WinnerEvaluation;

        public event OnEvaluationChangedDelegate OnEvaluationChanged;

        public event OnEvaluationChangedDelegate OnFailureEvaluationChanged;

        public void Clear()
        {
            if (this.WinnerEvaluation != null)
            {
                this.WinnerEvaluation.Dispose();
                this.WinnerEvaluation = null;
            }
            if (this.LoserEvaluation != null)
            {
                this.LoserEvaluation.Dispose();
                this.LoserEvaluation = null;
            }
            Singleton<GameEventSys>.instance.RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_PostActorDead, new RefAction<GameDeadEventParam>(this.OnActorDeath));
            Singleton<GameEventSys>.instance.RmvEventHandler<SCampScoreUpdateParam>(GameEventDef.Event_CampScoreUpdated, new RefAction<SCampScoreUpdateParam>(this.OnCampScoreUpdated));
            this.CurLevelTimeDuration = 0;
            this.bStarted = false;
        }

        private IStarEvaluation CreateStar(ResEvaluateStarInfo ConditionDetail)
        {
            StarEvaluation evaluation = new StarEvaluation();
            evaluation.OnChanged += new OnEvaluationChangedDelegate(this.OnEvaluationChangedInner);
            evaluation.Index = 0;
            evaluation.Initialize(ConditionDetail);
            return evaluation;
        }

        public void EndGame()
        {
            WinLoseByStarSys instance = Singleton<WinLoseByStarSys>.instance;
            instance.OnEvaluationChanged = (OnEvaluationChangedDelegate) Delegate.Remove(instance.OnEvaluationChanged, new OnEvaluationChangedDelegate(BattleLogic.OnWinStarSysChanged));
            WinLoseByStarSys local2 = Singleton<WinLoseByStarSys>.instance;
            local2.OnFailureEvaluationChanged = (OnEvaluationChangedDelegate) Delegate.Remove(local2.OnFailureEvaluationChanged, new OnEvaluationChangedDelegate(BattleLogic.OnLoseStarSysChanged));
            Singleton<WinLoseByStarSys>.instance.Clear();
        }

        private void OnActorDeath(ref GameDeadEventParam prm)
        {
            if (this.WinnerEvaluation != null)
            {
                this.WinnerEvaluation.OnActorDeath(ref prm);
            }
            if (this.LoserEvaluation != null)
            {
                this.LoserEvaluation.OnActorDeath(ref prm);
            }
        }

        private void OnCampScoreUpdated(ref SCampScoreUpdateParam prm)
        {
            if (this.WinnerEvaluation != null)
            {
                this.WinnerEvaluation.OnCampScoreUpdated(ref prm);
            }
            if (this.LoserEvaluation != null)
            {
                this.LoserEvaluation.OnCampScoreUpdated(ref prm);
            }
        }

        private void OnEvaluationChangedInner(IStarEvaluation InStarEvaluation, IStarCondition InStarCondition)
        {
            if (InStarEvaluation == this.WinnerEvaluation)
            {
                if (this.OnEvaluationChanged != null)
                {
                    this.OnEvaluationChanged(InStarEvaluation, InStarCondition);
                }
            }
            else if ((InStarEvaluation == this.LoserEvaluation) && (this.OnFailureEvaluationChanged != null))
            {
                this.OnFailureEvaluationChanged(InStarEvaluation, InStarCondition);
            }
        }

        public bool Reset(SLevelContext levelContext, bool bMultiGame)
        {
            this.Clear();
            bool flag = false;
            if (levelContext.IsMobaModeWithOutGuide())
            {
                this.CurLevelTimeDuration = levelContext.m_timeDuration;
                if (levelContext.m_addWinCondStarId != 0)
                {
                    ResEvaluateStarInfo dataByKey = GameDataMgr.addWinLoseCondDatabin.GetDataByKey(levelContext.m_addWinCondStarId);
                    DebugHelper.Assert(dataByKey != null);
                    if (dataByKey != null)
                    {
                        this.WinnerEvaluation = this.CreateStar(dataByKey);
                        DebugHelper.Assert(this.WinnerEvaluation != null, "我擦，怎会没有？");
                        flag = true;
                    }
                }
                if (levelContext.m_addLoseCondStarId != 0)
                {
                    ResEvaluateStarInfo conditionDetail = GameDataMgr.addWinLoseCondDatabin.GetDataByKey(levelContext.m_addLoseCondStarId);
                    DebugHelper.Assert(conditionDetail != null);
                    if (conditionDetail != null)
                    {
                        this.LoserEvaluation = this.CreateStar(conditionDetail);
                        DebugHelper.Assert(this.LoserEvaluation != null, "我擦，怎会没有？");
                        flag = true;
                    }
                }
            }
            return flag;
        }

        public void Start()
        {
            if (this.WinnerEvaluation != null)
            {
                this.WinnerEvaluation.Start();
            }
            if (this.LoserEvaluation != null)
            {
                this.LoserEvaluation.Start();
            }
            Singleton<GameEventSys>.instance.AddEventHandler<GameDeadEventParam>(GameEventDef.Event_PostActorDead, new RefAction<GameDeadEventParam>(this.OnActorDeath));
            Singleton<GameEventSys>.instance.AddEventHandler<SCampScoreUpdateParam>(GameEventDef.Event_CampScoreUpdated, new RefAction<SCampScoreUpdateParam>(this.OnCampScoreUpdated));
            this.bStarted = true;
        }

        public void StartFight()
        {
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            if ((curLvelContext != null) && Singleton<WinLoseByStarSys>.instance.Reset(curLvelContext, false))
            {
                WinLoseByStarSys instance = Singleton<WinLoseByStarSys>.instance;
                instance.OnEvaluationChanged = (OnEvaluationChangedDelegate) Delegate.Combine(instance.OnEvaluationChanged, new OnEvaluationChangedDelegate(BattleLogic.OnWinStarSysChanged));
                WinLoseByStarSys local2 = Singleton<WinLoseByStarSys>.instance;
                local2.OnFailureEvaluationChanged = (OnEvaluationChangedDelegate) Delegate.Combine(local2.OnFailureEvaluationChanged, new OnEvaluationChangedDelegate(BattleLogic.OnLoseStarSysChanged));
                Singleton<WinLoseByStarSys>.instance.Start();
            }
        }

        public bool bStarted
        {
            [CompilerGenerated]
            get
            {
                return this.<bStarted>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<bStarted>k__BackingField = value;
            }
        }

        public uint CurLevelTimeDuration
        {
            [CompilerGenerated]
            get
            {
                return this.<CurLevelTimeDuration>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<CurLevelTimeDuration>k__BackingField = value;
            }
        }

        public bool isFailure
        {
            get
            {
                return ((this.LoserEvaluation != null) && (this.LoserEvaluation.status == StarEvaluationStatus.Success));
            }
        }

        public bool isSuccess
        {
            get
            {
                return ((this.WinnerEvaluation != null) && (this.WinnerEvaluation.status == StarEvaluationStatus.Success));
            }
        }
    }
}

