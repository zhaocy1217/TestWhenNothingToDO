namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.GameLogic.GameKernal;
    using ResData;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class VoiceInteractionSys : MonoSingleton<VoiceInteractionSys>
    {
        private bool bActiveSys;
        private StarSystemFactory Factory = new StarSystemFactory(typeof(VoiceInteractionAttribute), typeof(VoiceInteraction));
        private Dictionary<int, HashSet<int>> HeroStatInfo = new Dictionary<int, HashSet<int>>();
        private DictionaryView<int, ListView<VoiceInteraction>> Interactions = new DictionaryView<int, ListView<VoiceInteraction>>();

        private bool CheckReceiver(ResVoiceInteraction InInteractionCfg, HashSet<int> InOwnerCamps)
        {
            int hostPlayerCamp = (int) Singleton<GamePlayerCenter>.instance.hostPlayerCamp;
            if (InInteractionCfg.bSpecialReceive != 0)
            {
                for (int i = 0; i < InInteractionCfg.SpecialReceiveConditions.Length; i++)
                {
                    if (InInteractionCfg.SpecialReceiveConditions[i] == 0)
                    {
                        break;
                    }
                    int inCfgID = (int) InInteractionCfg.SpecialReceiveConditions[i];
                    if (InInteractionCfg.bReceiveType == 100)
                    {
                        if (this.HasReceiver(inCfgID))
                        {
                            return true;
                        }
                    }
                    else if (InInteractionCfg.bReceiveType == 0)
                    {
                        if (this.HasReceiverInCamp(inCfgID, hostPlayerCamp))
                        {
                            return true;
                        }
                    }
                    else if ((InInteractionCfg.bReceiveType == 1) && this.HasReceiverNotInCamp(inCfgID, hostPlayerCamp))
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (InInteractionCfg.bReceiveType == 100)
                {
                    return true;
                }
                if (InInteractionCfg.bReceiveType == 0)
                {
                    if (InOwnerCamps.Contains(hostPlayerCamp))
                    {
                        return true;
                    }
                }
                else if (InInteractionCfg.bReceiveType == 1)
                {
                    HashSet<int>.Enumerator enumerator = InOwnerCamps.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        if (enumerator.get_Current() != hostPlayerCamp)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private void Clear()
        {
            DictionaryView<int, ListView<VoiceInteraction>>.Enumerator enumerator = this.Interactions.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<int, ListView<VoiceInteraction>> current = enumerator.Current;
                ListView<VoiceInteraction> view = current.Value;
                for (int i = 0; i < view.Count; i++)
                {
                    view[i].Unit();
                }
            }
            this.Interactions.Clear();
            this.HeroStatInfo.Clear();
        }

        private VoiceInteraction CreateVoiceInteraction(ResVoiceInteraction InInteractionCfg)
        {
            VoiceInteraction interaction = this.Factory.Create(InInteractionCfg.bTriggerType) as VoiceInteraction;
            object[] inParameters = new object[] { InInteractionCfg.dwConfigID, InInteractionCfg.bTriggerType };
            DebugHelper.Assert(interaction != null, "Failed create Interaction for {0}:{1}", inParameters);
            return interaction;
        }

        private void FilterInteractionCfg(ResVoiceInteraction InInteractionCfg)
        {
            HashSet<int> set = null;
            if (this.HeroStatInfo.TryGetValue((int) InInteractionCfg.dwGroupID, out set) && this.CheckReceiver(InInteractionCfg, set))
            {
                VoiceInteraction item = this.CreateVoiceInteraction(InInteractionCfg);
                if (item != null)
                {
                    item.Init(InInteractionCfg);
                    ListView<VoiceInteraction> view = null;
                    if (!this.Interactions.TryGetValue(item.groupID, out view))
                    {
                        view = new ListView<VoiceInteraction>();
                        this.Interactions.Add(item.groupID, view);
                    }
                    view.Add(item);
                }
            }
        }

        private bool HasReceiver(int InCfgID)
        {
            return this.HeroStatInfo.ContainsKey(InCfgID);
        }

        private bool HasReceiverInCamp(int InCfgID, int InTestCamp)
        {
            HashSet<int> set = null;
            return (this.HeroStatInfo.TryGetValue(InCfgID, out set) && set.Contains(InTestCamp));
        }

        private bool HasReceiverNotInCamp(int InCfgID, int InTestCamp)
        {
            HashSet<int> set = null;
            if (this.HeroStatInfo.TryGetValue(InCfgID, out set))
            {
                HashSet<int>.Enumerator enumerator = set.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (enumerator.get_Current() != InTestCamp)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        protected override void Init()
        {
            base.Init();
            Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightStart, new RefAction<DefaultGameEventParam>(this.onFightStart));
        }

        private void LateUpdate()
        {
            if (this.bActiveSys && !MonoSingleton<Reconnection>.GetInstance().isProcessingRelayRecover)
            {
                DictionaryView<int, ListView<VoiceInteraction>>.Enumerator enumerator = this.Interactions.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    KeyValuePair<int, ListView<VoiceInteraction>> current = enumerator.Current;
                    ListView<VoiceInteraction> view = current.Value;
                    int priority = -1;
                    int num2 = 0;
                    for (int i = 0; i < view.Count; i++)
                    {
                        VoiceInteraction interaction = view[i];
                        if (interaction.isBeginTrigger && (interaction.priority > priority))
                        {
                            priority = interaction.priority;
                            num2++;
                        }
                    }
                    if (num2 > 0)
                    {
                        float inGroupTriggerTime = Time.get_realtimeSinceStartup();
                        for (int j = 0; j < view.Count; j++)
                        {
                            VoiceInteraction interaction2 = view[j];
                            if (interaction2.isBeginTrigger && (interaction2.priority == priority))
                            {
                                interaction2.DoTrigger();
                            }
                            interaction2.FinishTrigger(inGroupTriggerTime);
                        }
                    }
                }
            }
        }

        protected override void OnDestroy()
        {
            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightStart, new RefAction<DefaultGameEventParam>(this.onFightStart));
            base.OnDestroy();
        }

        public void OnEndGame()
        {
            this.Clear();
            this.bActiveSys = false;
        }

        private void onFightStart(ref DefaultGameEventParam prm)
        {
            this.Clear();
            this.bActiveSys = Singleton<BattleLogic>.instance.GetCurLvelContext().IsMobaModeWithOutGuide();
            if (this.bActiveSys)
            {
                this.StartupSys();
            }
        }

        private void StartupSys()
        {
            List<Player> allPlayers = Singleton<GamePlayerCenter>.instance.GetAllPlayers();
            for (int i = 0; i < allPlayers.Count; i++)
            {
                Player player = allPlayers[i];
                if (player.Captain != 0)
                {
                    int configId = player.Captain.handle.TheActorMeta.ConfigId;
                    int actorCamp = (int) player.Captain.handle.TheActorMeta.ActorCamp;
                    HashSet<int> set = null;
                    if (!this.HeroStatInfo.TryGetValue(configId, out set))
                    {
                        set = new HashSet<int>();
                        this.HeroStatInfo.Add(configId, set);
                    }
                    if (!set.Contains(actorCamp))
                    {
                        set.Add(actorCamp);
                    }
                }
            }
            GameDataMgr.voiceInteractionDatabin.Accept(new Action<ResVoiceInteraction>(this.FilterInteractionCfg));
        }
    }
}

