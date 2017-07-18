namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using ResData;
    using System;

    [Energy(EnergyType.MadnessResource)]
    public class Madness : BaseEnergyLogic
    {
        public override void Init(PoolObjHandle<ActorRoot> _actor)
        {
            base.energyType = EnergyType.MadnessResource;
            base.Init(_actor);
        }

        protected override void UpdateEpValue()
        {
            this._actorEp += this.actor.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_EPRECOVER].totalValue / 5;
        }
    }
}

