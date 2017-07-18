namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.GameLogic.DataCenter;
    using Assets.Scripts.GameLogic.GameKernal;
    using System;
    using System.Collections.Generic;

    public abstract class BasicGameInfo : IGameInfo
    {
        protected IGameContext GameContext;

        protected BasicGameInfo()
        {
        }

        public virtual void EndGame()
        {
        }

        public virtual bool Initialize(IGameContext InGameContext)
        {
            DebugHelper.Assert(InGameContext != null);
            this.GameContext = InGameContext;
            return (this.GameContext != null);
        }

        protected virtual void LoadAllTeamActors()
        {
            List<Player>.Enumerator enumerator = Singleton<GamePlayerCenter>.instance.GetAllPlayers().GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current != null)
                {
                    ReadonlyContext<uint> allHeroIds = enumerator.Current.GetAllHeroIds();
                    for (int i = 0; i < allHeroIds.Count; i++)
                    {
                        ActorMeta meta2 = new ActorMeta();
                        meta2.ActorCamp = enumerator.Current.PlayerCamp;
                        meta2.ConfigId = allHeroIds[i];
                        meta2.PlayerId = enumerator.Current.PlayerId;
                        ActorMeta actorMeta = meta2;
                        MonoSingleton<GameLoader>.instance.AddActor(ref actorMeta);
                    }
                }
            }
        }

        public virtual void OnLoadingProgress(float Progress)
        {
        }

        public virtual void PostBeginPlay()
        {
            Singleton<BattleLogic>.GetInstance().PrepareFight();
            if (!Singleton<LobbyLogic>.instance.inMultiGame)
            {
                Singleton<FrameSynchr>.GetInstance().ResetSynchr();
                bool flag = false;
                SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
                Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
                if (((curLvelContext != null) && (curLvelContext.PreDialogId > 0)) && ((hostPlayer != null) && (hostPlayer.Captain != 0)))
                {
                    flag = true;
                    MonoSingleton<DialogueProcessor>.instance.PlayDrama(curLvelContext.PreDialogId, hostPlayer.Captain.handle.gameObject, hostPlayer.Captain.handle.gameObject, true);
                }
                if (!flag)
                {
                    Singleton<BattleLogic>.GetInstance().DoBattleStart();
                }
                else
                {
                    Singleton<BattleLogic>.GetInstance().BindFightPrepareFinListener();
                }
            }
            else if (!Singleton<GameReplayModule>.GetInstance().isReplay)
            {
                Singleton<LobbyLogic>.GetInstance().ReqStartMultiGame();
            }
            SoldierRegion.bFirstSpawnEvent = true;
        }

        public virtual void PreBeginPlay()
        {
        }

        public virtual void ReduceDamage(ref HurtDataInfo HurtInfo)
        {
        }

        public virtual void StartFight()
        {
        }

        public IGameContext gameContext
        {
            get
            {
                return this.GameContext;
            }
        }
    }
}

