namespace Assets.Scripts.GameLogic.GameKernal
{
    using Assets.Scripts.Common;
    using CSProtocol;
    using System;

    public class GameBuilderEx : Singleton<GameBuilderEx>
    {
        private BaseBuilderHelper _curGameBuilder;
        private readonly BaseBuilderHelper _multiGameBuilder = new MultiGameBuilderHelper();
        private readonly BaseBuilderHelper _soloGameBuilder = new SoloGameBuilderHelper();

        public void BuildGame(ProtocolObject svrInfo)
        {
            this._curGameBuilder = null;
            if (svrInfo is SCPKG_STARTSINGLEGAMERSP)
            {
                this._curGameBuilder = this._soloGameBuilder;
            }
            else if (svrInfo is SCPKG_MULTGAME_BEGINLOAD)
            {
                this._curGameBuilder = this._multiGameBuilder;
            }
            if (this._curGameBuilder != null)
            {
                this._curGameBuilder.BuildGameContext(svrInfo);
                this._curGameBuilder.BuildGamePlayer(svrInfo);
                this._curGameBuilder.PreLoad();
                this._curGameBuilder.Load();
            }
        }

        public void EndGame()
        {
            if (this._curGameBuilder != null)
            {
                this._curGameBuilder.EndGame();
            }
        }

        public float LastLoadingTime
        {
            get
            {
                return this._curGameBuilder.LastLoadingTime;
            }
        }
    }
}

