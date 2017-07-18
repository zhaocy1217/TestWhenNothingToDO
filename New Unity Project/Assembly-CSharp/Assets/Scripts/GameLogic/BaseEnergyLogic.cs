namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using ResData;
    using System;

    public class BaseEnergyLogic
    {
        private CrypticInt32 _nObjCurEp = 1;
        protected PoolObjHandle<ActorRoot> actor;
        protected ResHeroEnergyInfo cfgData;
        protected int deltaTime;
        public EnergyType energyType = EnergyType.NoneResource;

        public virtual void EquipmentAddition()
        {
        }

        protected virtual bool Fit()
        {
            return (((this.deltaTime <= 0) && !this.actor.handle.ActorControl.GetNoAbilityFlag(ObjAbilityType.ObjAbility_RecoverEnergy)) && !this.actor.handle.ActorControl.IsDeadState);
        }

        public virtual void Init(PoolObjHandle<ActorRoot> _actor)
        {
            this.actor = _actor;
            this.cfgData = GameDataMgr.heroEnergyDatabin.GetDataByKey((uint) this.energyType);
            this.deltaTime = 0;
        }

        protected virtual void ResetDeltaTime()
        {
            this.deltaTime = this.cfgData.iRecFrequency;
        }

        public virtual void ResetEpValue(int epPercent)
        {
        }

        public virtual void SetEpValue(int value)
        {
            this._actorEp = value;
        }

        public virtual void Uninit()
        {
        }

        protected virtual void UpdateEpValue()
        {
        }

        public virtual void UpdateLogic(int _delta)
        {
            if (this.deltaTime > 0)
            {
                this.deltaTime -= _delta;
                this.deltaTime = (this.deltaTime <= 0) ? 0 : this.deltaTime;
            }
            if (this.Fit())
            {
                this.UpdateEpValue();
                this.ResetDeltaTime();
            }
        }

        public virtual int _actorEp
        {
            get
            {
                return (int) this._nObjCurEp;
            }
            protected set
            {
                int num = (value <= this.actor.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_MAXEP].totalValue) ? ((value >= 0) ? value : 0) : this.actor.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_MAXEP].totalValue;
                if (this._nObjCurEp != num)
                {
                    this._nObjCurEp = num;
                    Singleton<EventRouter>.GetInstance().BroadCastEvent<PoolObjHandle<ActorRoot>, int, int>("HeroEnergyChange", this.actor, num, this.actorEpTotal);
                }
                if (num == this.actor.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_MAXEP].totalValue)
                {
                    Singleton<EventRouter>.GetInstance().BroadCastEvent<PoolObjHandle<ActorRoot>, int, int>("HeroEnergyMax", this.actor, num, this.actorEpTotal);
                }
            }
        }

        public virtual int actorEpRecTotal
        {
            get
            {
                return this.actor.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_EPRECOVER].totalValue;
            }
        }

        public virtual int actorEpTotal
        {
            get
            {
                return this.actor.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_MAXEP].totalValue;
            }
        }
    }
}

