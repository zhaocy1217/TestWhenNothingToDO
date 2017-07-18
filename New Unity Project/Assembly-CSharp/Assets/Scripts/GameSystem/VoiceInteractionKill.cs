namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.GameLogic.GameKernal;
    using ResData;
    using System;

    [VoiceInteraction(0)]
    public class VoiceInteractionKill : VoiceInteraction
    {
        public override void Init(ResVoiceInteraction InInteractionCfg)
        {
            base.Init(InInteractionCfg);
            Singleton<GameEventSys>.instance.AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
        }

        private void onActorDead(ref GameDeadEventParam prm)
        {
            if (this.ForwardCheck() && ((((prm.orignalAtker != 0) && (prm.orignalAtker.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)) && ((prm.orignalAtker.handle.TheActorMeta.ConfigId == base.groupID) && (prm.src != 0))) && (((prm.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero) && this.CheckTriggerDistance(ref prm.orignalAtker, ref prm.src)) && base.ValidateTriggerActor(ref prm.src))))
            {
                Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
                if (((hostPlayer != null) && (hostPlayer.Captain != 0)) && this.CheckReceiveDistance(ref hostPlayer.Captain, ref prm.src))
                {
                    this.TryTrigger(ref prm.orignalAtker, ref prm.src, ref prm.orignalAtker);
                }
            }
        }

        public override void Unit()
        {
            Singleton<GameEventSys>.instance.RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
            base.Unit();
        }
    }
}

