namespace Assets.Scripts.GameLogic
{
    using AGE;
    using Assets.Scripts.Common;
    using System;

    public class TriggerActionShenFu : TriggerActionBase
    {
        public TriggerActionShenFu(TriggerActionWrapper inWrapper, int inTriggerId) : base(inWrapper, inTriggerId)
        {
        }

        public override void OnCoolDown(ITrigger inTrigger)
        {
            Singleton<ShenFuSystem>.instance.OnShenfuHalt((uint) base.UpdateUniqueId, (AreaEventTrigger) inTrigger, this);
        }

        public override void OnTriggerStart(ITrigger inTrigger)
        {
            Singleton<ShenFuSystem>.instance.OnShenfuStart((uint) base.UpdateUniqueId, (AreaEventTrigger) inTrigger, this);
        }

        public override void Stop()
        {
            Singleton<ShenFuSystem>.instance.OnShenFuStopped(this);
        }

        public override RefParamOperator TriggerEnter(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ITrigger inTrigger)
        {
            return null;
        }

        public override void TriggerUpdate(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ITrigger inTrigger)
        {
            Singleton<ShenFuSystem>.instance.OnShenFuEffect(src, (uint) base.UpdateUniqueId, (AreaEventTrigger) inTrigger, this);
            if (src != 0)
            {
                Singleton<EventRouter>.instance.BroadCastEvent<COM_PLAYERCAMP, uint, uint>(EventID.BATTLE_SHENFU_EFFECT_CHANGED, src.handle.TheActorMeta.ActorCamp, src.handle.TheActorMeta.PlayerId, (uint) ((AreaEventTrigger) inTrigger).ID);
            }
        }
    }
}

