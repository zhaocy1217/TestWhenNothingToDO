namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using System;

    [Energy(EnergyType.SpeedResource)]
    public class Speed : BaseEnergyLogic
    {
        public override void Init(PoolObjHandle<ActorRoot> _actor)
        {
            base.energyType = EnergyType.SpeedResource;
            base.Init(_actor);
        }

        public override void ResetEpValue(int epPercent)
        {
            this._actorEp = 0;
        }

        public override void UpdateLogic(int _delta)
        {
        }
    }
}

