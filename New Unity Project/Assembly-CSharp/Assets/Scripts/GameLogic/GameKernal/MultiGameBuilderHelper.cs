namespace Assets.Scripts.GameLogic.GameKernal
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameSystem;
    using CSProtocol;
    using System;

    internal class MultiGameBuilderHelper : BaseBuilderHelper
    {
        public override void BuildGameContext(ProtocolObject svrInfo)
        {
            SCPKG_MULTGAME_BEGINLOAD svrGameInfo = (SCPKG_MULTGAME_BEGINLOAD) svrInfo;
            Singleton<GameContextEx>.GetInstance().InitMultiGame(svrGameInfo);
        }

        public override void BuildGamePlayer(ProtocolObject svrInfo)
        {
            SCPKG_MULTGAME_BEGINLOAD svrGameInfo = (SCPKG_MULTGAME_BEGINLOAD) svrInfo;
            Singleton<GameContextEx>.GetInstance().InitMultiGame(svrGameInfo);
            base.PlayerBuilder.BuildMultiGamePlayers(svrGameInfo);
        }

        public override void OnGameLoadProgress(float progress)
        {
            if (!Singleton<WatchController>.GetInstance().IsWatching)
            {
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x43b);
                msg.stPkgData.stMultGameLoadProcessReq.wProcess = Convert.ToUInt16((float) (progress * 100f));
                Singleton<NetworkModule>.GetInstance().SendGameMsg(ref msg, 0);
            }
            CUILoadingSystem.OnSelfLoadProcess(progress);
        }

        public override void PreLoad()
        {
            base.PreLoad();
            Singleton<FrameSynchr>.GetInstance().SetSynchrConfig(Singleton<GameContextEx>.GetInstance().GameContextMobaInfo.KFrapsFreqMs, Singleton<GameContextEx>.GetInstance().GameContextMobaInfo.KFrapsLater, Singleton<GameContextEx>.GetInstance().GameContextMobaInfo.PreActFrap, Singleton<GameContextEx>.GetInstance().GameContextMobaInfo.RandomSeed);
        }
    }
}

