namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using System;

    [Energy(EnergyType.EnergyResource)]
    public class Energy : BaseEnergyLogic
    {
        public override void Init(PoolObjHandle<ActorRoot> _actor)
        {
            base.energyType = EnergyType.EnergyResource;
            base.Init(_actor);
        }

        protected override void UpdateEpValue()
        {
            this._actorEp += base.cfgData.iRecAmount;
        }
    }
}

