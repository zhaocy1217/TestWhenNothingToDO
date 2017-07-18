namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.GameLogic.GameKernal;
    using CSProtocol;
    using System;
    using UnityEngine;

    public class TestGameContext : GameContextBase
    {
        public TestGameContext(ref SCPKG_STARTSINGLEGAMERSP InMessage)
        {
            Singleton<ActorDataCenter>.instance.ClearHeroServerData();
            Player player = Singleton<GamePlayerCenter>.GetInstance().AddPlayer(1, COM_PLAYERCAMP.COM_PLAYERCAMP_1, 0, 1, false, "test", 0, 0, 0L, 0, null, 0, 0, 0, 0, null);
            Singleton<GamePlayerCenter>.GetInstance().SetHostPlayer(1);
            player.AddHero(InMessage.stDetail.stSingleGameSucc.astFighter[0].astChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID);
            COMDT_CHOICEHERO[] serverHeroInfos = new COMDT_CHOICEHERO[] { new COMDT_CHOICEHERO() };
            serverHeroInfos[0].stBaseInfo.stCommonInfo.dwHeroID = InMessage.stDetail.stSingleGameSucc.astFighter[0].astChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID;
            serverHeroInfos[0].stBaseInfo.stCommonInfo.wSkinID = InMessage.stDetail.stSingleGameSucc.astFighter[0].astChoiceHero[0].stBaseInfo.stCommonInfo.wSkinID;
            Singleton<ActorDataCenter>.instance.AddHeroesServerData(1, serverHeroInfos);
            Singleton<GamePlayerCenter>.GetInstance().AddPlayer(2, COM_PLAYERCAMP.COM_PLAYERCAMP_2, 0, 1, true, string.Empty, 0, 0, 0L, 0, null, 0, 0, 0, 0, null).AddHero(0xa6);
            serverHeroInfos = new COMDT_CHOICEHERO[] { new COMDT_CHOICEHERO() };
            serverHeroInfos[0].stBaseInfo.stCommonInfo.dwHeroID = 0xa6;
            Singleton<ActorDataCenter>.instance.AddHeroesServerData(2, serverHeroInfos);
            base.LevelContext = new SLevelContext();
            base.LevelContext.SetGameType(COM_GAME_TYPE.COM_SINGLE_GAME_OF_ADVENTURE);
            base.LevelContext.m_levelArtistFileName = PlayerPrefs.GetString("PrevewMapArt");
            base.levelContext.m_levelDesignFileName = PlayerPrefs.GetString("PrevewMapDesigner");
            base.levelContext.m_levelDifficulty = 1;
        }

        public override GameInfoBase CreateGameInfo()
        {
            SingleGameInfo info = new SingleGameInfo();
            info.Initialize(this);
            return info;
        }
    }
}

