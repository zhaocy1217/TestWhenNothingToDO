namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.GameSystem;
    using System;

    public class SingleGameInfo : GameInfoBase
    {
        public override void EndGame()
        {
            base.EndGame();
        }

        public override void OnLoadingProgress(float Progress)
        {
            CUILoadingSystem.OnSelfLoadProcess(Progress);
        }

        public override void PostBeginPlay()
        {
            base.PostBeginPlay();
        }

        public override void PreBeginPlay()
        {
            this.LoadAllTeamActors();
        }

        public override void StartFight()
        {
            base.StartFight();
        }
    }
}

