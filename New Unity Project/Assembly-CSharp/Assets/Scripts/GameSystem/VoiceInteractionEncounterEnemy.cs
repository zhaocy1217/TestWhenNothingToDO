namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using ResData;
    using System;
    using System.Collections.Generic;

    [VoiceInteraction(3)]
    public class VoiceInteractionEncounterEnemy : VoiceInteraction
    {
        private bool CheckActor(ref PoolObjHandle<ActorRoot> InTestActor)
        {
            List<PoolObjHandle<ActorRoot>>.Enumerator enumerator = Singleton<GameObjMgr>.instance.HeroActors.GetEnumerator();
            while (enumerator.MoveNext())
            {
                PoolObjHandle<ActorRoot> current = enumerator.Current;
                if ((((((current != InTestActor) && (current != 0)) && ((current.handle.HorizonMarker != null) && !current.handle.IsSelfCamp((ActorRoot) InTestActor))) && (base.ValidateTriggerActor(ref current) && current.handle.HorizonMarker.IsVisibleFor(InTestActor.handle.TheActorMeta.ActorCamp))) && InTestActor.handle.HorizonMarker.IsVisibleFor(current.handle.TheActorMeta.ActorCamp)) && this.CheckTriggerDistance(ref InTestActor, ref current))
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
                List<PoolObjHandle<ActorRoot>>.Enumerator enumerator = Singleton<GameObjMgr>.instance.HeroActors.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    PoolObjHandle<ActorRoot> current = enumerator.Current;
                    if (((current != 0) && (current.handle.TheActorMeta.ConfigId == base.groupID)) && ((current.handle.HorizonMarker != null) && this.CheckActor(ref current)))
                    {
                        return;
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

