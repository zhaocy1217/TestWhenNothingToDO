namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using System;

    [Energy(EnergyType.FuryResource)]
    public class Fury : BaseEnergyLogic
    {
        protected override bool Fit()
        {
            return (base.Fit() && !this.actor.handle.ActorControl.IsInBattle);
        }

        public override void Init(PoolObjHandle<ActorRoot> _actor)
        {
            base.energyType = EnergyType.FuryResource;
            base.Init(_actor);
        }

        public override void ResetEpValue(int epPercent)
        {
            this._actorEp = 0;
        }

        protected override void UpdateEpValue()
        {
            this._actorEp += base.cfgData.iRecAmount;
        }
    }
}

