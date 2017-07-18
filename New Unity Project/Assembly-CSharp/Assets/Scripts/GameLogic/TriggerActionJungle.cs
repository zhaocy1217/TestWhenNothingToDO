namespace Assets.Scripts.GameLogic
{
    using AGE;
    using Assets.Scripts.Common;
    using CSProtocol;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class TriggerActionJungle : TriggerActionBase
    {
        public TriggerActionJungle(TriggerActionWrapper inWrapper, int inTriggerId) : base(inWrapper, inTriggerId)
        {
        }

        public override void Destroy()
        {
        }

        private void ModifyHorizonMarks(PoolObjHandle<ActorRoot> src, ITrigger inTrigger, bool enterOrLeave)
        {
            <ModifyHorizonMarks>c__AnonStorey46 storey = new <ModifyHorizonMarks>c__AnonStorey46();
            storey.src = src;
            if (storey.src != 0)
            {
                int num = !enterOrLeave ? -1 : 1;
                List<PoolObjHandle<ActorRoot>> actors = (inTrigger as AreaEventTrigger).GetActors(new Func<PoolObjHandle<ActorRoot>, bool>(storey, (IntPtr) this.<>m__1F));
                for (int i = 0; i < actors.Count; i++)
                {
                    PoolObjHandle<ActorRoot> handle = actors[i];
                    handle.handle.HorizonMarker.AddShowMark(storey.src.handle.TheActorMeta.ActorCamp, HorizonConfig.ShowMark.Jungle, num * 1);
                    storey.src.handle.HorizonMarker.AddShowMark(handle.handle.TheActorMeta.ActorCamp, HorizonConfig.ShowMark.Jungle, num * 1);
                }
                COM_PLAYERCAMP[] othersCmp = BattleLogic.GetOthersCmp(storey.src.handle.TheActorMeta.ActorCamp);
                for (int j = 0; j < othersCmp.Length; j++)
                {
                    storey.src.handle.HorizonMarker.AddHideMark(othersCmp[j], HorizonConfig.HideMark.Jungle, num * 1, false);
                }
            }
        }

        public override RefParamOperator TriggerEnter(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ITrigger inTrigger)
        {
            if (!FogOfWar.enable)
            {
                ActorInGrassParam prm = new ActorInGrassParam(src, true);
                Singleton<GameEventSys>.instance.SendEvent<ActorInGrassParam>(GameEventDef.Event_ActorInGrass, ref prm);
                this.ModifyHorizonMarks(src, inTrigger, true);
            }
            return null;
        }

        public override void TriggerLeave(PoolObjHandle<ActorRoot> src, ITrigger inTrigger)
        {
            if (!FogOfWar.enable)
            {
                ActorInGrassParam prm = new ActorInGrassParam(src, false);
                Singleton<GameEventSys>.instance.SendEvent<ActorInGrassParam>(GameEventDef.Event_ActorInGrass, ref prm);
                this.ModifyHorizonMarks(src, inTrigger, false);
            }
        }

        public override void TriggerUpdate(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ITrigger inTrigger)
        {
        }

        [CompilerGenerated]
        private sealed class <ModifyHorizonMarks>c__AnonStorey46
        {
            internal PoolObjHandle<ActorRoot> src;

            internal bool <>m__1F(PoolObjHandle<ActorRoot> enr)
            {
                return (enr.handle.TheActorMeta.ActorCamp != this.src.handle.TheActorMeta.ActorCamp);
            }
        }
    }
}

