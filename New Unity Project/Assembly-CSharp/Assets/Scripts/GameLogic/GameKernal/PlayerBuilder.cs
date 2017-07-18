namespace Assets.Scripts.GameLogic.GameKernal
{
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.GameLogic.DataCenter;
    using Assets.Scripts.GameSystem;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class PlayerBuilder
    {
        public void BuildMultiGamePlayers(SCPKG_MULTGAME_BEGINLOAD svrGameInfo)
        {
            if (Singleton<GamePlayerCenter>.instance.GetAllPlayers().Count > 0)
            {
            }
            Singleton<GamePlayerCenter>.instance.ClearAllPlayers();
            uint playerId = 0;
            for (int i = 0; i < 2; i++)
            {
                for (int k = 0; k < svrGameInfo.astCampInfo[i].dwPlayerNum; k++)
                {
                    uint dwObjId = svrGameInfo.astCampInfo[i].astCampPlayerInfo[k].stPlayerInfo.dwObjId;
                    Player player = Singleton<GamePlayerCenter>.GetInstance().GetPlayer(dwObjId);
                    DebugHelper.Assert(player == null, "你tm肯定在逗我");
                    if ((playerId == 0) && ((i + 1) == 1))
                    {
                        playerId = dwObjId;
                    }
                    bool isComputer = svrGameInfo.astCampInfo[i].astCampPlayerInfo[k].stPlayerInfo.bObjType == 2;
                    if (player == null)
                    {
                        ulong uid = 0L;
                        uint dwFakeLogicWorldID = 0;
                        uint level = 1;
                        string openId = string.Empty;
                        uint vipLv = 0;
                        int honorId = 0;
                        int honorLevel = 0;
                        if (isComputer)
                        {
                            if (Convert.ToBoolean(svrGameInfo.stDeskInfo.bIsWarmBattle))
                            {
                                uid = svrGameInfo.astCampInfo[i].astCampPlayerInfo[k].stPlayerInfo.stDetail.stPlayerOfNpc.ullFakeUid;
                                dwFakeLogicWorldID = svrGameInfo.astCampInfo[i].astCampPlayerInfo[k].stPlayerInfo.stDetail.stPlayerOfNpc.dwFakeLogicWorldID;
                                level = svrGameInfo.astCampInfo[i].astCampPlayerInfo[k].stPlayerInfo.stDetail.stPlayerOfNpc.dwFakePvpLevel;
                                openId = string.Empty;
                            }
                            else
                            {
                                uid = 0L;
                                dwFakeLogicWorldID = 0;
                                level = svrGameInfo.astCampInfo[i].astCampPlayerInfo[k].stPlayerInfo.dwLevel;
                                openId = string.Empty;
                            }
                        }
                        else
                        {
                            uid = svrGameInfo.astCampInfo[i].astCampPlayerInfo[k].stPlayerInfo.stDetail.stPlayerOfAcnt.ullUid;
                            dwFakeLogicWorldID = (uint) svrGameInfo.astCampInfo[i].astCampPlayerInfo[k].stPlayerInfo.stDetail.stPlayerOfAcnt.iLogicWorldID;
                            level = svrGameInfo.astCampInfo[i].astCampPlayerInfo[k].stPlayerInfo.dwLevel;
                            openId = Utility.UTF8Convert(svrGameInfo.astCampInfo[i].astCampPlayerInfo[k].szOpenID);
                            vipLv = svrGameInfo.astCampInfo[i].astCampPlayerInfo[k].stPlayerInfo.stDetail.stPlayerOfAcnt.stGameVip.dwCurLevel;
                            honorId = svrGameInfo.astCampInfo[i].astCampPlayerInfo[k].stPlayerInfo.stDetail.stPlayerOfAcnt.iHonorID;
                            honorLevel = svrGameInfo.astCampInfo[i].astCampPlayerInfo[k].stPlayerInfo.stDetail.stPlayerOfAcnt.iHonorLevel;
                        }
                        player = Singleton<GamePlayerCenter>.GetInstance().AddPlayer(dwObjId, (COM_PLAYERCAMP) (i + 1), svrGameInfo.astCampInfo[i].astCampPlayerInfo[k].stPlayerInfo.bPosOfCamp, level, isComputer, Utility.UTF8Convert(svrGameInfo.astCampInfo[i].astCampPlayerInfo[k].stPlayerInfo.szName), 0, (int) dwFakeLogicWorldID, uid, vipLv, openId, svrGameInfo.astCampInfo[i].astCampPlayerInfo[k].dwGradeOfRank, svrGameInfo.astCampInfo[i].astCampPlayerInfo[k].dwClassOfRank, honorId, honorLevel, null);
                        if (player != null)
                        {
                            player.isGM = svrGameInfo.astCampInfo[i].astCampPlayerInfo[k].bIsGM > 0;
                        }
                    }
                    for (int m = 0; m < svrGameInfo.astCampInfo[i].astCampPlayerInfo[k].stPlayerInfo.astChoiceHero.Length; m++)
                    {
                        COMDT_CHOICEHERO comdt_choicehero = svrGameInfo.astCampInfo[i].astCampPlayerInfo[k].stPlayerInfo.astChoiceHero[m];
                        int dwHeroID = (int) comdt_choicehero.stBaseInfo.stCommonInfo.dwHeroID;
                        if (dwHeroID != 0)
                        {
                            bool flag2 = ((comdt_choicehero.stBaseInfo.stCommonInfo.dwMaskBits & 4) > 0) && ((comdt_choicehero.stBaseInfo.stCommonInfo.dwMaskBits & 1) == 0);
                            if (player != null)
                            {
                                player.AddHero((uint) dwHeroID);
                            }
                        }
                    }
                }
            }
            if (Singleton<WatchController>.GetInstance().IsWatching)
            {
                Singleton<GamePlayerCenter>.GetInstance().SetHostPlayer(playerId);
            }
            else if (Singleton<GameReplayModule>.HasInstance() && Singleton<GameReplayModule>.instance.isReplay)
            {
                Singleton<GamePlayerCenter>.GetInstance().SetHostPlayer(playerId);
            }
            else
            {
                Player playerByUid = Singleton<GamePlayerCenter>.GetInstance().GetPlayerByUid(Singleton<CRoleInfoManager>.instance.masterUUID);
                DebugHelper.Assert(playerByUid != null, "load multi game but hostPlayer is null");
                Singleton<GamePlayerCenter>.GetInstance().SetHostPlayer(playerByUid.PlayerId);
            }
            Singleton<ActorDataCenter>.instance.ClearHeroServerData();
            for (int j = 0; j < svrGameInfo.astCampInfo.Length; j++)
            {
                CSDT_CAMPINFO csdt_campinfo = svrGameInfo.astCampInfo[j];
                int num14 = Mathf.Min(csdt_campinfo.astCampPlayerInfo.Length, (int) csdt_campinfo.dwPlayerNum);
                for (int n = 0; n < num14; n++)
                {
                    COMDT_PLAYERINFO stPlayerInfo = csdt_campinfo.astCampPlayerInfo[n].stPlayerInfo;
                    Singleton<ActorDataCenter>.instance.AddHeroesServerData(stPlayerInfo.dwObjId, stPlayerInfo.astChoiceHero);
                }
            }
        }

        public void BuildSoloGamePlayers(SCPKG_STARTSINGLEGAMERSP svrGameInfo)
        {
            Singleton<ActorDataCenter>.instance.ClearHeroServerData();
            if (Singleton<GamePlayerCenter>.instance.GetAllPlayers().Count > 0)
            {
            }
            Singleton<GamePlayerCenter>.instance.ClearAllPlayers();
            if ((svrGameInfo.stDetail.stSingleGameSucc != null) && (svrGameInfo.stDetail.stSingleGameSucc.bNum >= 1))
            {
                this.DoNew9SlotCalc(svrGameInfo);
                int num = Mathf.Min(svrGameInfo.stDetail.stSingleGameSucc.bNum, svrGameInfo.stDetail.stSingleGameSucc.astFighter.Length);
                for (int i = 0; i < num; i++)
                {
                    COMDT_PLAYERINFO comdt_playerinfo = svrGameInfo.stDetail.stSingleGameSucc.astFighter[i];
                    if (CheatCommandReplayEntry.heroPerformanceTest)
                    {
                        comdt_playerinfo.astChoiceHero = svrGameInfo.stDetail.stSingleGameSucc.astFighter[0].astChoiceHero;
                    }
                    if (comdt_playerinfo.bObjType != 0)
                    {
                        ulong uid = 0L;
                        uint dwFakeLogicWorldID = 0;
                        uint level = 1;
                        int honorId = 0;
                        int honorLevel = 0;
                        uint gradeOfRank = 0;
                        if (comdt_playerinfo.bObjType == 2)
                        {
                            if ((svrGameInfo.bGameType == 1) && Convert.ToBoolean(svrGameInfo.stGameParam.stSingleGameRspOfCombat.bIsWarmBattle))
                            {
                                uid = comdt_playerinfo.stDetail.stPlayerOfNpc.ullFakeUid;
                                dwFakeLogicWorldID = comdt_playerinfo.stDetail.stPlayerOfNpc.dwFakeLogicWorldID;
                                level = comdt_playerinfo.stDetail.stPlayerOfNpc.dwFakePvpLevel;
                            }
                            else
                            {
                                uid = 0L;
                                dwFakeLogicWorldID = 0;
                                level = comdt_playerinfo.dwLevel;
                            }
                        }
                        else
                        {
                            uid = (comdt_playerinfo.bObjType != 1) ? ((ulong) 0L) : comdt_playerinfo.stDetail.stPlayerOfAcnt.ullUid;
                            dwFakeLogicWorldID = (comdt_playerinfo.bObjType != 1) ? 0 : ((uint) comdt_playerinfo.stDetail.stPlayerOfAcnt.iLogicWorldID);
                            level = comdt_playerinfo.dwLevel;
                            honorId = comdt_playerinfo.stDetail.stPlayerOfAcnt.iHonorID;
                            honorLevel = comdt_playerinfo.stDetail.stPlayerOfAcnt.iHonorLevel;
                            gradeOfRank = (svrGameInfo.stGameParam.stSingleGameRspOfCombat != null) ? svrGameInfo.stGameParam.stSingleGameRspOfCombat.bGradeOfRank : 0;
                        }
                        uint vipLv = 0;
                        if (comdt_playerinfo.stDetail.stPlayerOfAcnt != null)
                        {
                            vipLv = comdt_playerinfo.stDetail.stPlayerOfAcnt.stGameVip.dwCurLevel;
                        }
                        Player player = Singleton<GamePlayerCenter>.GetInstance().AddPlayer(comdt_playerinfo.dwObjId, (COM_PLAYERCAMP) comdt_playerinfo.bObjCamp, comdt_playerinfo.bPosOfCamp, level, comdt_playerinfo.bObjType != 1, StringHelper.UTF8BytesToString(ref comdt_playerinfo.szName), 0, (int) dwFakeLogicWorldID, uid, vipLv, null, gradeOfRank, 0, honorId, honorLevel, null);
                        if (player != null)
                        {
                            for (int j = 0; j < comdt_playerinfo.astChoiceHero.Length; j++)
                            {
                                uint dwHeroID = comdt_playerinfo.astChoiceHero[j].stBaseInfo.stCommonInfo.dwHeroID;
                                player.AddHero(dwHeroID);
                            }
                            player.isGM = LobbyMsgHandler.isHostGMAcnt;
                        }
                        Singleton<ActorDataCenter>.instance.AddHeroesServerData(comdt_playerinfo.dwObjId, comdt_playerinfo.astChoiceHero);
                    }
                    if (comdt_playerinfo.bObjType == 1)
                    {
                        Singleton<GamePlayerCenter>.GetInstance().SetHostPlayer(comdt_playerinfo.dwObjId);
                    }
                }
            }
        }

        private void Calc9SlotHeroStandingPosition(CSDT_BATTLE_PLAYER_BRIEF stBattlePlayer)
        {
            Calc9SlotHeroData[] heroes = new Calc9SlotHeroData[3];
            IGameActorDataProvider actorDataProvider = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.StaticLobbyDataProvider);
            ActorStaticData actorData = new ActorStaticData();
            ActorMeta actorMeta = new ActorMeta();
            if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo() != null)
            {
                for (int i = 0; (i < stBattlePlayer.astFighter[0].astChoiceHero.Length) && (i < 3); i++)
                {
                    uint dwHeroID = stBattlePlayer.astFighter[0].astChoiceHero[i].stBaseInfo.stCommonInfo.dwHeroID;
                    if (dwHeroID != 0)
                    {
                        actorMeta.ConfigId = (int) dwHeroID;
                        actorDataProvider.GetActorStaticData(ref actorMeta, ref actorData);
                        heroes[i].Level = 1;
                        heroes[i].Quality = 1;
                        heroes[i].RecommendPos = actorData.TheHeroOnlyInfo.RecommendStandPos;
                        heroes[i].Ability = (uint) CHeroDataFactory.CreateHeroData(dwHeroID).combatEft;
                        heroes[i].ConfigId = dwHeroID;
                        heroes[i].selected = false;
                        heroes[i].BornIndex = -1;
                    }
                }
                this.ImpCalc9SlotHeroStandingPosition(ref heroes);
                for (int j = 0; (j < stBattlePlayer.astFighter[0].astChoiceHero.Length) && (j < 3); j++)
                {
                    stBattlePlayer.astFighter[0].astChoiceHero[j].stHeroExtral.iHeroPos = heroes[j].BornIndex;
                }
                for (int k = 0; (k < stBattlePlayer.astFighter[1].astChoiceHero.Length) && (k < 3); k++)
                {
                    uint id = stBattlePlayer.astFighter[1].astChoiceHero[k].stBaseInfo.stCommonInfo.dwHeroID;
                    if (id != 0)
                    {
                        actorMeta.ConfigId = (int) id;
                        actorDataProvider.GetActorStaticData(ref actorMeta, ref actorData);
                        heroes[k].Level = stBattlePlayer.astFighter[1].astChoiceHero[k].stBaseInfo.stCommonInfo.wLevel;
                        heroes[k].Quality = stBattlePlayer.astFighter[1].astChoiceHero[k].stBaseInfo.stCommonInfo.stQuality.wQuality;
                        heroes[k].RecommendPos = actorData.TheHeroOnlyInfo.RecommendStandPos;
                        heroes[k].Ability = (uint) CHeroDataFactory.CreateHeroData(id).combatEft;
                        heroes[k].ConfigId = id;
                        heroes[k].selected = false;
                        heroes[k].BornIndex = -1;
                    }
                }
                this.ImpCalc9SlotHeroStandingPosition(ref heroes);
                for (int m = 0; (m < stBattlePlayer.astFighter[1].astChoiceHero.Length) && (m < 3); m++)
                {
                    stBattlePlayer.astFighter[1].astChoiceHero[m].stHeroExtral.iHeroPos = heroes[m].BornIndex;
                }
            }
        }

        protected void DoNew9SlotCalc(SCPKG_STARTSINGLEGAMERSP inMessage)
        {
            if ((inMessage.bGameType == 8) || (inMessage.bGameType == 7))
            {
                this.Calc9SlotHeroStandingPosition(inMessage.stDetail.stSingleGameSucc);
            }
        }

        private List<int> HasPositionHero(ref Calc9SlotHeroData[] heroes, RES_HERO_RECOMMEND_POSITION pos)
        {
            List<int> list = new List<int>();
            for (int i = 0; i < 3; i++)
            {
                if (heroes[i].RecommendPos == pos)
                {
                    list.Add(i);
                }
            }
            return list;
        }

        private void ImpCalc9SlotHeroStandingPosition(ref Calc9SlotHeroData[] heroes)
        {
            List<int> list = this.HasPositionHero(ref heroes, RES_HERO_RECOMMEND_POSITION.RES_HERO_RECOMMEND_POSITION_T_FRONT);
            int index = 0;
            switch (list.Count)
            {
                case 1:
                    for (int i = 0; i < 3; i++)
                    {
                        if (heroes[i].RecommendPos == 0)
                        {
                            heroes[i].selected = true;
                            heroes[i].BornIndex = 1;
                            break;
                        }
                    }
                    index = this.WhoIsBestHero(ref heroes);
                    heroes[index].selected = true;
                    if (heroes[index].RecommendPos == 1)
                    {
                        heroes[index].BornIndex = 3;
                        index = this.WhoIsBestHero(ref heroes);
                        heroes[index].selected = true;
                        heroes[index].BornIndex = (heroes[index].RecommendPos != 1) ? 8 : 5;
                    }
                    else
                    {
                        heroes[index].BornIndex = 8;
                        index = this.WhoIsBestHero(ref heroes);
                        heroes[index].selected = true;
                        heroes[index].BornIndex = (heroes[index].RecommendPos != 1) ? 6 : 3;
                    }
                    return;

                case 2:
                    for (int j = 0; j < 3; j++)
                    {
                        if (heroes[j].RecommendPos == 1)
                        {
                            heroes[j].selected = true;
                            heroes[j].BornIndex = 3;
                            break;
                        }
                        if (heroes[j].RecommendPos == 2)
                        {
                            heroes[j].selected = true;
                            heroes[j].BornIndex = 6;
                            break;
                        }
                    }
                    break;

                case 3:
                    index = this.WhoIsBestHero(ref heroes);
                    heroes[index].selected = true;
                    heroes[index].BornIndex = 1;
                    index = this.WhoIsBestHero(ref heroes);
                    heroes[index].selected = true;
                    heroes[index].BornIndex = 0;
                    index = this.WhoIsBestHero(ref heroes);
                    heroes[index].selected = true;
                    heroes[index].BornIndex = 2;
                    return;

                default:
                    switch (this.HasPositionHero(ref heroes, RES_HERO_RECOMMEND_POSITION.RES_HERO_RECOMMEND_POSITION_T_CENTER).Count)
                    {
                        case 1:
                            for (int k = 0; k < 3; k++)
                            {
                                if (heroes[k].RecommendPos == 1)
                                {
                                    heroes[k].selected = true;
                                    heroes[k].BornIndex = 1;
                                    break;
                                }
                            }
                            index = this.WhoIsBestHero(ref heroes);
                            heroes[index].selected = true;
                            heroes[index].BornIndex = 8;
                            index = this.WhoIsBestHero(ref heroes);
                            heroes[index].selected = true;
                            heroes[index].BornIndex = 6;
                            return;

                        case 2:
                            for (int m = 0; m < 3; m++)
                            {
                                if (heroes[m].RecommendPos == 2)
                                {
                                    heroes[m].selected = true;
                                    heroes[m].BornIndex = 3;
                                    break;
                                }
                            }
                            index = this.WhoIsBestHero(ref heroes);
                            heroes[index].selected = true;
                            heroes[index].BornIndex = 1;
                            index = this.WhoIsBestHero(ref heroes);
                            heroes[index].selected = true;
                            heroes[index].BornIndex = 0;
                            return;

                        case 3:
                            index = this.WhoIsBestHero(ref heroes);
                            heroes[index].selected = true;
                            heroes[index].BornIndex = 1;
                            index = this.WhoIsBestHero(ref heroes);
                            heroes[index].selected = true;
                            heroes[index].BornIndex = 0;
                            index = this.WhoIsBestHero(ref heroes);
                            heroes[index].selected = true;
                            heroes[index].BornIndex = 2;
                            return;
                    }
                    index = this.WhoIsBestHero(ref heroes);
                    heroes[index].selected = true;
                    heroes[index].BornIndex = 4;
                    index = this.WhoIsBestHero(ref heroes);
                    heroes[index].selected = true;
                    heroes[index].BornIndex = 3;
                    index = this.WhoIsBestHero(ref heroes);
                    heroes[index].selected = true;
                    heroes[index].BornIndex = 5;
                    return;
            }
            index = this.WhoIsBestHero(ref heroes);
            heroes[index].selected = true;
            heroes[index].BornIndex = 1;
            index = this.WhoIsBestHero(ref heroes);
            heroes[index].selected = true;
            heroes[index].BornIndex = 0;
        }

        private bool IsBetterHero(ref Calc9SlotHeroData heroe1, ref Calc9SlotHeroData heroe2)
        {
            return (((heroe1.ConfigId > 0) && !heroe1.selected) && (((((heroe2.ConfigId == 0) || heroe2.selected) || (heroe1.Ability > heroe2.Ability)) || ((heroe1.Ability == heroe2.Ability) && (heroe1.Level > heroe2.Level))) || (((heroe1.Ability == heroe2.Ability) && (heroe1.Level == heroe2.Level)) && (heroe1.Quality >= heroe2.Quality))));
        }

        private int WhoIsBestHero(ref Calc9SlotHeroData[] heroes)
        {
            if (this.IsBetterHero(ref heroes[0], ref heroes[1]) && this.IsBetterHero(ref heroes[0], ref heroes[2]))
            {
                return 0;
            }
            if (this.IsBetterHero(ref heroes[1], ref heroes[0]) && this.IsBetterHero(ref heroes[1], ref heroes[2]))
            {
                return 1;
            }
            return 2;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Calc9SlotHeroData
        {
            public uint ConfigId;
            public int RecommendPos;
            public uint Ability;
            public uint Level;
            public int Quality;
            public int BornIndex;
            public bool selected;
        }
    }
}

