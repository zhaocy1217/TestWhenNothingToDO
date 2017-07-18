namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.GameSystem;
    using System;

    [EventCategory("MMGame/Skill")]
    public class MoveCityProcessBarDuration : DurationEvent
    {
        private PoolObjHandle<ActorRoot> actorObj;
        public string key = string.Empty;
        [ObjectTemplate(new Type[] {  })]
        public int targetId;

        public override BaseEvent Clone()
        {
            MoveCityProcessBarDuration duration = ClassObjPool<MoveCityProcessBarDuration>.Get();
            duration.CopyData(this);
            return duration;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            MoveCityProcessBarDuration duration = src as MoveCityProcessBarDuration;
            this.targetId = duration.targetId;
            this.key = duration.key;
        }

        public override void Enter(Action _action, Track _track)
        {
            base.Enter(_action, _track);
            this.actorObj = _action.GetActorHandle(this.targetId);
            if (this.actorObj != 0)
            {
                if (ActorHelper.IsHostCtrlActor(ref this.actorObj))
                {
                    uint logicFrameTick = (uint) Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
                    uint length = (uint) base.length;
                    if (Singleton<CBattleSystem>.GetInstance().FightForm != null)
                    {
                        string text = !string.IsNullOrEmpty(this.key) ? Singleton<CTextManager>.GetInstance().GetText(this.key) : string.Empty;
                        Singleton<CBattleSystem>.GetInstance().FightForm.StartGoBackProcessBar(logicFrameTick, length, text);
                    }
                }
                if (ActorHelper.IsHostCampActor(ref this.actorObj))
                {
                    BackCityCom_3DUI.ShowBack2City(this.actorObj);
                }
            }
        }

        public override void Leave(Action _action, Track _track)
        {
            base.Leave(_action, _track);
            if (ActorHelper.IsHostCtrlActor(ref this.actorObj) && (Singleton<CBattleSystem>.GetInstance().FightForm != null))
            {
                Singleton<CBattleSystem>.GetInstance().FightForm.EndGoBackProcessBar();
            }
            if (ActorHelper.IsHostCampActor(ref this.actorObj))
            {
                BackCityCom_3DUI.HideBack2City(this.actorObj);
            }
        }

        public override void OnUse()
        {
            base.OnUse();
            this.targetId = 0;
            this.key = string.Empty;
            this.actorObj.Release();
        }

        public override void Process(Action _action, Track _track, int _localTime)
        {
            base.Process(_action, _track, _localTime);
            if (ActorHelper.IsHostCtrlActor(ref this.actorObj) && (Singleton<CBattleSystem>.GetInstance().FightForm != null))
            {
                uint logicFrameTick = (uint) Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
                Singleton<CBattleSystem>.GetInstance().FightForm.UpdateGoBackProcessBar(logicFrameTick);
            }
        }

        public override bool SupportEditMode()
        {
            return true;
        }
    }
}

