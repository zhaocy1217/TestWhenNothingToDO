namespace Assets.Scripts.GameLogic
{
    using AGE;
    using Assets.Scripts.Common;
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public abstract class TriggerActionBase
    {
        [CompilerGenerated]
        private int <TriggerId>k__BackingField;
        public int ActiveTime;
        public bool bAtker;
        public bool bEnable;
        public bool bSrc;
        public bool bStopWhenLeaving;
        public int EnterUniqueId;
        public int LeaveUniqueId;
        public int Offset_x;
        public int Offset_y;
        public GameObject[] RefObjList;
        public AreaEventTrigger.STimingAction[] TimingActionsInter;
        public int TotalTime;
        public int UpdateUniqueId;

        public TriggerActionBase(TriggerActionWrapper inWrapper, int inTriggerId)
        {
            this.RefObjList = inWrapper.RefObjList;
            this.TimingActionsInter = inWrapper.TimingActionsInter;
            this.EnterUniqueId = inWrapper.EnterUniqueId;
            this.LeaveUniqueId = inWrapper.LeaveUniqueId;
            this.UpdateUniqueId = inWrapper.UpdateUniqueId;
            this.bEnable = inWrapper.bEnable;
            this.bStopWhenLeaving = inWrapper.bStopWhenLeaving;
            this.bSrc = inWrapper.bSrc;
            this.bAtker = inWrapper.bAtker;
            this.TotalTime = inWrapper.TotalTime;
            this.ActiveTime = inWrapper.ActiveTime;
            this.Offset_x = inWrapper.Offset_x;
            this.Offset_y = inWrapper.Offset_y;
            this.TriggerId = inTriggerId;
        }

        public void AppendRefObj(GameObject[] inRefObjList)
        {
            if ((inRefObjList != null) && (inRefObjList.Length != 0))
            {
                this.RefObjList = inRefObjList;
            }
        }

        public virtual void Destroy()
        {
            this.Stop();
        }

        public virtual void OnCoolDown(ITrigger inTrigger)
        {
        }

        public virtual void OnTriggerStart(ITrigger inTrigger)
        {
        }

        public virtual void Stop()
        {
        }

        public abstract RefParamOperator TriggerEnter(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ITrigger inTrigger);
        public virtual void TriggerLeave(PoolObjHandle<ActorRoot> src, ITrigger inTrigger)
        {
        }

        public virtual void TriggerUpdate(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ITrigger inTrigger)
        {
        }

        public int TriggerId
        {
            [CompilerGenerated]
            get
            {
                return this.<TriggerId>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<TriggerId>k__BackingField = value;
            }
        }
    }
}

