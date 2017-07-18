namespace Assets.Scripts.GameLogic
{
    using ResData;
    using System;
    using System.Runtime.CompilerServices;

    [StarConditionAttrContext(15)]
    internal class StarConditionMonsterTrigger : StarCondition
    {
        [CompilerGenerated]
        private bool <bCachedResults>k__BackingField;
        [CompilerGenerated]
        private int <TriggerCount>k__BackingField;

        private void CheckResult()
        {
            bool flag = SmartCompare.Compare<int>(this.TriggerCount, this.targetCount, this.operation);
            if (flag != this.bCachedResults)
            {
                this.bCachedResults = flag;
                this.TriggerChangedEvent();
            }
        }

        public override void Dispose()
        {
            Singleton<TriggerEventSys>.instance.OnActorEnter -= new TriggerEventDelegate(this.OnActorEnter);
            Singleton<TriggerEventSys>.instance.OnActorLeave -= new TriggerEventDelegate(this.OnActorLeave);
            base.Dispose();
        }

        public override void Initialize(ResDT_ConditionInfo InConditionInfo)
        {
            base.Initialize(InConditionInfo);
            this.TriggerCount = 0;
            Singleton<TriggerEventSys>.instance.OnActorEnter += new TriggerEventDelegate(this.OnActorEnter);
            Singleton<TriggerEventSys>.instance.OnActorLeave += new TriggerEventDelegate(this.OnActorLeave);
            this.bCachedResults = SmartCompare.Compare<int>(this.TriggerCount, this.targetCount, this.operation);
        }

        private void OnActorEnter(AreaEventTrigger sourceTrigger, object param)
        {
            ActorRoot root = param as ActorRoot;
            if ((((root != null) && (sourceTrigger != null)) && ((sourceTrigger.ID == this.targetTriggerID) && (root.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster))) && (root.TheActorMeta.ConfigId == this.targetMonsterID))
            {
                this.TriggerCount++;
                this.CheckResult();
            }
        }

        private void OnActorLeave(AreaEventTrigger sourceTrigger, object param)
        {
        }

        public bool bCachedResults
        {
            [CompilerGenerated]
            get
            {
                return this.<bCachedResults>k__BackingField;
            }
            [CompilerGenerated]
            protected set
            {
                this.<bCachedResults>k__BackingField = value;
            }
        }

        public bool isTriggerd
        {
            get
            {
                return (base.ConditionInfo.KeyDetail[3] == 1);
            }
        }

        private int targetCount
        {
            get
            {
                return base.ConditionInfo.ValueDetail[0];
            }
        }

        public int targetMonsterID
        {
            get
            {
                return base.ConditionInfo.KeyDetail[1];
            }
        }

        public int targetTriggerID
        {
            get
            {
                return base.ConditionInfo.KeyDetail[2];
            }
        }

        public int TriggerCount
        {
            [CompilerGenerated]
            get
            {
                return this.<TriggerCount>k__BackingField;
            }
            [CompilerGenerated]
            protected set
            {
                this.<TriggerCount>k__BackingField = value;
            }
        }

        public override int[] values
        {
            get
            {
                return new int[] { this.TriggerCount };
            }
        }
    }
}

