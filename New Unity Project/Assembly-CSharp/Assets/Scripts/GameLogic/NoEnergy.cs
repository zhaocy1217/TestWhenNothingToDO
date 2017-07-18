namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using System;

    [Energy(EnergyType.NoneResource)]
    public class NoEnergy : BaseEnergyLogic
    {
        public override void Init(PoolObjHandle<ActorRoot> _actor)
        {
            base.energyType = EnergyType.NoneResource;
            base.Init(_actor);
        }

        public override void UpdateLogic(int _delta)
        {
        }
    }
}

