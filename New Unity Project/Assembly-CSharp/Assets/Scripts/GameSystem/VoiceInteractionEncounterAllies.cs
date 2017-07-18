namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.GameLogic.GameKernal;
    using ResData;
    using System;
    using System.Collections.Generic;

    [VoiceInteraction(2)]
    public class VoiceInteractionEncounterAllies : VoiceInteraction
    {
        private bool CheckActor(ref PoolObjHandle<ActorRoot> InTestActor, ref PoolObjHandle<ActorRoot> HostActor)
        {
            List<PoolObjHandle<ActorRoot>>.Enumerator enumerator = Singleton<GameObjMgr>.instance.HeroActors.GetEnumerator();
            while (enumerator.MoveNext())
            {
                PoolObjHandle<ActorRoot> current = enumerator.Current;
                if ((((((current != InTestActor) && (current != 0)) && (current.handle.IsSelfCamp((ActorRoot) InTestActor) && (current.handle.HorizonMarker != null))) && base.ValidateTriggerActor(ref current)) && current.handle.HorizonMarker.IsVisibleFor(HostActor.handle.TheActorMeta.ActorCamp)) && this.CheckTriggerDistance(ref InTestActor, ref current))
                {
                    this.TryTrigger(ref InTestActor, ref current, ref InTestActor);
                    return true;
                }
            }
            return false;
        }

        public override void Init(ResVoiceInteraction InInteractionCfg)
        {
            base.Init(InInteractionCfg);
            Singleton<CTimerManager>.instance.AddTimer(0x3e8, 0, new CTimer.OnTimeUpHandler(this.OnCheckEncounter), false);
        }

        private void OnCheckEncounter(int TimeSeq)
        {
            if (this.ForwardCheck())
            {
                Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
                if (((hostPlayer != null) && (hostPlayer.Captain != 0)) && (hostPlayer.Captain.handle.HorizonMarker != null))
                {
                    List<PoolObjHandle<ActorRoot>>.Enumerator enumerator = Singleton<GameObjMgr>.instance.HeroActors.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        PoolObjHandle<ActorRoot> current = enumerator.Current;
                        if ((((current != 0) && (current.handle.TheActorMeta.ConfigId == base.groupID)) && ((current.handle.HorizonMarker != null) && current.handle.HorizonMarker.IsVisibleFor(hostPlayer.Captain.handle.TheActorMeta.ActorCamp))) && this.CheckActor(ref current, ref hostPlayer.Captain))
                        {
                            return;
                        }
                    }
                }
            }
        }

        public override void Unit()
        {
            Singleton<CTimerManager>.instance.RemoveTimer(new CTimer.OnTimeUpHandler(this.OnCheckEncounter), false);
            base.Unit();
        }
    }
}

