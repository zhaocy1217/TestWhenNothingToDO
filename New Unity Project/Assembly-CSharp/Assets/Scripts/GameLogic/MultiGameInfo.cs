namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.GameKernal;
    using Assets.Scripts.GameSystem;
    using CSProtocol;
    using System;

    public abstract class MultiGameInfo : GameInfoBase
    {
        protected MultiGameInfo()
        {
        }

        public override void EndGame()
        {
            base.EndGame();
        }

        public override void OnLoadingProgress(float Progress)
        {
            if (!Singleton<WatchController>.GetInstance().IsWatching)
            {
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x43b);
                msg.stPkgData.stMultGameLoadProcessReq.wProcess = Convert.ToUInt16((float) (Progress * 100f));
                Singleton<NetworkModule>.GetInstance().SendGameMsg(ref msg, 0);
            }
            CUILoadingSystem.OnSelfLoadProcess(Progress);
        }

        public override void PreBeginPlay()
        {
            base.PreBeginPlay();
            this.PreparePlayer();
            this.ResetSynchrConfig();
            this.LoadAllTeamActors();
        }

        protected virtual void PreparePlayer()
        {
            MultiGameContext gameContext = base.GameContext as MultiGameContext;
            DebugHelper.Assert(gameContext != null);
            if (gameContext != null)
            {
                if (Singleton<GamePlayerCenter>.instance.GetAllPlayers().Count > 0)
                {
                }
                Singleton<GamePlayerCenter>.instance.ClearAllPlayers();
                uint playerId = 0;
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < gameContext.MessageRef.astCampInfo[i].dwPlayerNum; j++)
                    {
                        uint dwObjId = gameContext.MessageRef.astCampInfo[i].astCampPlayerInfo[j].stPlayerInfo.dwObjId;
                        Player player = Singleton<GamePlayerCenter>.GetInstance().GetPlayer(dwObjId);
                        DebugHelper.Assert(player == null, "你tm肯定在逗我");
                        if ((playerId == 0) && ((i + 1) == 1))
                        {
                            playerId = dwObjId;
                        }
                        bool isComputer = gameContext.MessageRef.astCampInfo[i].astCampPlayerInfo[j].stPlayerInfo.bObjType == 2;
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
                                if (Convert.ToBoolean(gameContext.MessageRef.stDeskInfo.bIsWarmBattle))
                                {
                                    uid = gameContext.MessageRef.astCampInfo[i].astCampPlayerInfo[j].stPlayerInfo.stDetail.stPlayerOfNpc.ullFakeUid;
                                    dwFakeLogicWorldID = gameContext.MessageRef.astCampInfo[i].astCampPlayerInfo[j].stPlayerInfo.stDetail.stPlayerOfNpc.dwFakeLogicWorldID;
                                    level = gameContext.MessageRef.astCampInfo[i].astCampPlayerInfo[j].stPlayerInfo.stDetail.stPlayerOfNpc.dwFakePvpLevel;
                                    openId = string.Empty;
                                }
                                else
                                {
                                    uid = 0L;
                                    dwFakeLogicWorldID = 0;
                                    level = gameContext.MessageRef.astCampInfo[i].astCampPlayerInfo[j].stPlayerInfo.dwLevel;
                                    openId = string.Empty;
                                }
                            }
                            else
                            {
                                uid = gameContext.MessageRef.astCampInfo[i].astCampPlayerInfo[j].stPlayerInfo.stDetail.stPlayerOfAcnt.ullUid;
                                dwFakeLogicWorldID = (uint) gameContext.MessageRef.astCampInfo[i].astCampPlayerInfo[j].stPlayerInfo.stDetail.stPlayerOfAcnt.iLogicWorldID;
                                level = gameContext.MessageRef.astCampInfo[i].astCampPlayerInfo[j].stPlayerInfo.dwLevel;
                                openId = Utility.UTF8Convert(gameContext.MessageRef.astCampInfo[i].astCampPlayerInfo[j].szOpenID);
                                vipLv = gameContext.MessageRef.astCampInfo[i].astCampPlayerInfo[j].stPlayerInfo.stDetail.stPlayerOfAcnt.stGameVip.dwCurLevel;
                                honorId = gameContext.MessageRef.astCampInfo[i].astCampPlayerInfo[j].stPlayerInfo.stDetail.stPlayerOfAcnt.iHonorID;
                                honorLevel = gameContext.MessageRef.astCampInfo[i].astCampPlayerInfo[j].stPlayerInfo.stDetail.stPlayerOfAcnt.iHonorLevel;
                            }
                            GameIntimacyData intimacyData = null;
                            if (!isComputer)
                            {
                                ulong num2;
                                uint num4;
                                CSDT_CAMPPLAYERINFO csdt_campplayerinfo = gameContext.MessageRef.astCampInfo[i].astCampPlayerInfo[j];
                                if (csdt_campplayerinfo == null)
                                {
                                    continue;
                                }
                                ulong ullUid = csdt_campplayerinfo.stPlayerInfo.stDetail.stPlayerOfAcnt.ullUid;
                                int iLogicWorldID = csdt_campplayerinfo.stPlayerInfo.stDetail.stPlayerOfAcnt.iLogicWorldID;
                                byte bIntimacyRelationPrior = csdt_campplayerinfo.stIntimacyRelation.bIntimacyRelationPrior;
                                COMDT_INTIMACY_RELATION_DATA stIntimacyRelationData = csdt_campplayerinfo.stIntimacyRelation.stIntimacyRelationData;
                                ulong ulluid = (ulong) (num2 = 0L);
                                uint worldId = num4 = 0;
                                string str = string.Empty;
                                string str2 = string.Empty;
                                for (int m = 0; m < stIntimacyRelationData.wIntimacyRelationNum; m++)
                                {
                                    COMDT_INTIMACY_RELATION comdt_intimacy_relation = stIntimacyRelationData.astIntimacyRelationList[m];
                                    if (comdt_intimacy_relation != null)
                                    {
                                        for (int n = 0; n < 2; n++)
                                        {
                                            for (int num20 = 0; num20 < gameContext.MessageRef.astCampInfo[n].dwPlayerNum; num20++)
                                            {
                                                CSDT_CAMPPLAYERINFO csdt_campplayerinfo2 = gameContext.MessageRef.astCampInfo[n].astCampPlayerInfo[num20];
                                                if (gameContext.MessageRef.astCampInfo[n].astCampPlayerInfo[num20].stPlayerInfo.bObjType != 2)
                                                {
                                                    ulong num21 = csdt_campplayerinfo2.stPlayerInfo.stDetail.stPlayerOfAcnt.ullUid;
                                                    int num22 = csdt_campplayerinfo2.stPlayerInfo.stDetail.stPlayerOfAcnt.iLogicWorldID;
                                                    if (((csdt_campplayerinfo2 != null) && ((num21 != ullUid) || (num22 != iLogicWorldID))) && ((comdt_intimacy_relation.ullUid == num21) && (comdt_intimacy_relation.dwLogicWorldId == num22)))
                                                    {
                                                        string str4 = StringHelper.UTF8BytesToString(ref csdt_campplayerinfo2.stPlayerInfo.szName);
                                                        if (comdt_intimacy_relation.bIntimacyState == bIntimacyRelationPrior)
                                                        {
                                                            ulluid = num21;
                                                            worldId = (uint) num22;
                                                            str = str4;
                                                        }
                                                        else
                                                        {
                                                            num2 = num21;
                                                            num4 = (uint) num22;
                                                            str2 = str4;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                string str5 = StringHelper.UTF8BytesToString(ref csdt_campplayerinfo.stPlayerInfo.szName);
                                if ((ulluid != 0) && (worldId != 0))
                                {
                                    string title = string.Empty;
                                    switch (bIntimacyRelationPrior)
                                    {
                                        case 1:
                                        {
                                            string[] textArray1 = new string[] { str };
                                            title = Singleton<CTextManager>.instance.GetText("IntimacyShowLoadGay", textArray1);
                                            break;
                                        }
                                        case 2:
                                        {
                                            string[] textArray2 = new string[] { str };
                                            title = Singleton<CTextManager>.instance.GetText("IntimacyShowLoadLover", textArray2);
                                            break;
                                        }
                                    }
                                    intimacyData = new GameIntimacyData((COM_INTIMACY_STATE) bIntimacyRelationPrior, ulluid, worldId, title);
                                    object[] args = new object[] { str5, (COM_INTIMACY_STATE) bIntimacyRelationPrior, (COM_INTIMACY_STATE) bIntimacyRelationPrior, str };
                                    string str7 = string.Format("----FR, 局内亲密度, 玩家:{0}, 优先选择的关系:{1} --- finded 关系:{2}, 对方名字:{3}", args);
                                }
                                else if ((num2 != 0) && (num4 != 0))
                                {
                                    byte num23 = 0;
                                    switch (bIntimacyRelationPrior)
                                    {
                                        case 1:
                                            num23 = 2;
                                            break;

                                        case 2:
                                            num23 = 1;
                                            break;
                                    }
                                    string text = string.Empty;
                                    if (num23 == 1)
                                    {
                                        string[] textArray3 = new string[] { str2 };
                                        text = Singleton<CTextManager>.instance.GetText("IntimacyShowLoadGay", textArray3);
                                    }
                                    if (num23 == 2)
                                    {
                                        string[] textArray4 = new string[] { str2 };
                                        text = Singleton<CTextManager>.instance.GetText("IntimacyShowLoadLover", textArray4);
                                    }
                                    intimacyData = new GameIntimacyData((COM_INTIMACY_STATE) num23, num2, num4, text);
                                    object[] objArray2 = new object[] { str5, (COM_INTIMACY_STATE) bIntimacyRelationPrior, (COM_INTIMACY_STATE) num23, str2 };
                                    string str9 = string.Format("----FR, 局内亲密度, 玩家:{0}, 优先选择的关系:{1} --- finded 关系:{2}, 对方名字:{3}", objArray2);
                                }
                            }
                            player = Singleton<GamePlayerCenter>.GetInstance().AddPlayer(dwObjId, (COM_PLAYERCAMP) (i + 1), gameContext.MessageRef.astCampInfo[i].astCampPlayerInfo[j].stPlayerInfo.bPosOfCamp, level, isComputer, Utility.UTF8Convert(gameContext.MessageRef.astCampInfo[i].astCampPlayerInfo[j].stPlayerInfo.szName), 0, (int) dwFakeLogicWorldID, uid, vipLv, openId, gameContext.MessageRef.astCampInfo[i].astCampPlayerInfo[j].dwGradeOfRank, gameContext.MessageRef.astCampInfo[i].astCampPlayerInfo[j].dwClassOfRank, honorId, honorLevel, intimacyData);
                            DebugHelper.Assert(player != null, "创建player失败");
                            if (player != null)
                            {
                                player.isGM = gameContext.MessageRef.astCampInfo[i].astCampPlayerInfo[j].bIsGM > 0;
                            }
                        }
                        for (int k = 0; k < gameContext.MessageRef.astCampInfo[i].astCampPlayerInfo[j].stPlayerInfo.astChoiceHero.Length; k++)
                        {
                            COMDT_CHOICEHERO comdt_choicehero = gameContext.MessageRef.astCampInfo[i].astCampPlayerInfo[j].stPlayerInfo.astChoiceHero[k];
                            int dwHeroID = (int) comdt_choicehero.stBaseInfo.stCommonInfo.dwHeroID;
                            if (dwHeroID != 0)
                            {
                                bool flag2 = ((comdt_choicehero.stBaseInfo.stCommonInfo.dwMaskBits & 4) > 0) && ((comdt_choicehero.stBaseInfo.stCommonInfo.dwMaskBits & 1) == 0);
                                if ((gameContext.MessageRef.astCampInfo[i].astCampPlayerInfo[j].stPlayerInfo.bObjType != 1) || !flag2)
                                {
                                }
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
                gameContext.levelContext.m_isWarmBattle = Convert.ToBoolean(gameContext.MessageRef.stDeskInfo.bIsWarmBattle);
                gameContext.SaveServerData();
            }
        }

        protected virtual void ResetSynchrConfig()
        {
            MultiGameContext gameContext = base.GameContext as MultiGameContext;
            DebugHelper.Assert(gameContext != null);
            Singleton<FrameSynchr>.GetInstance().SetSynchrConfig(gameContext.MessageRef.dwKFrapsFreqMs, gameContext.MessageRef.bKFrapsLater, gameContext.MessageRef.bPreActFrap, gameContext.MessageRef.dwRandomSeed);
        }

        public override void StartFight()
        {
            base.StartFight();
        }
    }
}

