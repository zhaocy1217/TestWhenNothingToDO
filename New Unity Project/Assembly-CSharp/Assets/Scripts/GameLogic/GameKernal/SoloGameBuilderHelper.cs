namespace Assets.Scripts.GameLogic.GameKernal
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameSystem;
    using CSProtocol;
    using System;

    internal class SoloGameBuilderHelper : BaseBuilderHelper
    {
        public override void BuildGameContext(ProtocolObject svrInfo)
        {
            SCPKG_STARTSINGLEGAMERSP svrGameInfo = (SCPKG_STARTSINGLEGAMERSP) svrInfo;
            Singleton<GameContextEx>.GetInstance().InitSingleGame(svrGameInfo);
        }

        public override void BuildGamePlayer(ProtocolObject svrInfo)
        {
            SCPKG_STARTSINGLEGAMERSP svrGameInfo = (SCPKG_STARTSINGLEGAMERSP) svrInfo;
            base.PlayerBuilder.BuildSoloGamePlayers(svrGameInfo);
        }

        public override void OnGameLoadProgress(float progress)
        {
            CUILoadingSystem.OnSelfLoadProcess(progress);
        }
    }
}

