namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using ResData;
    using System;

    [Energy(EnergyType.MagicResource)]
    public class Magic : BaseEnergyLogic
    {
        public override void Init(PoolObjHandle<ActorRoot> _actor)
        {
            base.energyType = EnergyType.MagicResource;
            base.Init(_actor);
        }

        public override void ResetEpValue(int epPercent)
        {
            this._actorEp = (this.actor.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_MAXEP].totalValue * epPercent) / 0x2710;
        }

        protected override void UpdateEpValue()
        {
            this._actorEp += this.actor.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_EPRECOVER].totalValue / 5;
        }
    }
}

