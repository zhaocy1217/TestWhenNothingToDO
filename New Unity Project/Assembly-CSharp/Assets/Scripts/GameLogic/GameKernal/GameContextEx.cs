namespace Assets.Scripts.GameLogic.GameKernal
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.GameSystem;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public class GameContextEx : Singleton<GameContextEx>
    {
        private List<Camp> _enemyCampList = new List<Camp>();
        private CommonGameContextData _gameContextCommonData = new CommonGameContextData();
        private MobaData _gameContextMobaData = new MobaData();
        private SoloGameData _gameContextSoloData = new SoloGameData();
        private List<Camp> _validCampList = new List<Camp>();

        private void _InitMultiGame(COM_GAME_TYPE GameType, int mapType, uint mapID, bool isWarmBattle = false, int warmAiLv = 0)
        {
            ResDT_LevelCommonInfo pvpMapCommonInfo = this.GetPvpMapCommonInfo(mapType, mapID);
            this._gameContextCommonData.GameType = GameType;
            this._gameContextCommonData.IsMultiPlayerGame = true;
            this._gameContextCommonData.MapId = (int) mapID;
            this._gameContextCommonData.IsMobaGame = true;
            this._gameContextCommonData.SelectHeroType = (enSelectType) pvpMapCommonInfo.stPickRuleInfo.bPickType;
            this._gameContextCommonData.LevelDifficulty = 1;
            this._gameContextCommonData.HeroAiType = (RES_LEVEL_HEROAITYPE) pvpMapCommonInfo.iHeroAIType;
            this._gameContextCommonData.SoulGrowActive = true;
            this._gameContextCommonData.BaseReviveTime = 0x3a98;
            this._gameContextCommonData.DynamicPropertyConfigId = pvpMapCommonInfo.dwDynamicPropertyCfg;
            this._gameContextCommonData.MiniMapPath = pvpMapCommonInfo.szThumbnailPath;
            this._gameContextCommonData.BigMapPath = pvpMapCommonInfo.szBigMapPath;
            this._gameContextCommonData.MapWidth = pvpMapCommonInfo.iMapWidth;
            this._gameContextCommonData.MapHeight = pvpMapCommonInfo.iMapHeight;
            this._gameContextCommonData.BigMapWidth = pvpMapCommonInfo.iBigMapWidth;
            this._gameContextCommonData.BigMapHeight = pvpMapCommonInfo.iBigMapHeight;
            this._gameContextCommonData.MapFOWScale = pvpMapCommonInfo.fBigMapFowScale;
            this._gameContextCommonData.BigMapFOWScale = pvpMapCommonInfo.fBigMapFowScale;
            this._gameContextCommonData.MapName = pvpMapCommonInfo.szName;
            this._gameContextCommonData.MapDesignFileName = pvpMapCommonInfo.szDesignFileName;
            this._gameContextCommonData.MapArtistFileName = pvpMapCommonInfo.szArtistFileName;
            this._gameContextCommonData.MusicStartEvent = pvpMapCommonInfo.szMusicStartEvent;
            this._gameContextCommonData.MusicEndEvent = pvpMapCommonInfo.szMusicEndEvent;
            this._gameContextCommonData.AmbientSoundEvent = pvpMapCommonInfo.szBankResourceName;
            this._gameContextCommonData.MusicBankResName = pvpMapCommonInfo.szAmbientSoundEvent;
            this._gameContextCommonData.HorizonEnableMethod = Horizon.EnableMethod.EnableAll;
            this._gameContextCommonData.SoulId = pvpMapCommonInfo.dwSoulID;
            this._gameContextCommonData.SoulAllocId = pvpMapCommonInfo.dwSoulAllocId;
            this._gameContextCommonData.ExtraMapSkillId = pvpMapCommonInfo.iExtraSkillId;
            this._gameContextCommonData.ExtraMapSkill2Id = pvpMapCommonInfo.iExtraSkill2Id;
            this._gameContextCommonData.ExtraPassiveSkillId = pvpMapCommonInfo.iExtraPassiveSkillId;
            this._gameContextCommonData.OrnamentSkillId = pvpMapCommonInfo.iOrnamentSkillId;
            this._gameContextCommonData.OrnamentSwitchCd0 = pvpMapCommonInfo.iOrnamentSwitchCD;
            this._gameContextCommonData.EnableFOW = pvpMapCommonInfo.bIsEnableFow > 0;
            this._gameContextCommonData.EnableShopHorizonTab = pvpMapCommonInfo.bIsEnableOrnamentSlot > 0;
            this._gameContextCommonData.EnableOrnamentSlot = pvpMapCommonInfo.bIsEnableShopHorizonTab > 0;
            this._gameContextCommonData.CanRightJoyStickCameraDrag = pvpMapCommonInfo.bSupportCameraDrag > 0;
            this._gameContextCommonData.CameraFlip = pvpMapCommonInfo.bCameraFlip > 0;
            this._gameContextMobaData.IsValid = true;
            this._gameContextMobaData.MapType = mapType;
            this._gameContextMobaData.OriginalGoldCoinInBattle = pvpMapCommonInfo.wOriginalGoldCoinInBattle;
            this._gameContextMobaData.PlayerNum = pvpMapCommonInfo.bMaxAcntNum;
            this._gameContextMobaData.BattleEquipLimit = pvpMapCommonInfo.bBattleEquipLimit > 0;
            this._gameContextMobaData.HeadPtsMaxLimit = pvpMapCommonInfo.bHeadPtsUpperLimit;
            this._gameContextMobaData.BirthLevelConfig = pvpMapCommonInfo.bBirthLevelConfig;
            this._gameContextMobaData.IsShowHonor = pvpMapCommonInfo.bShowHonor > 0;
            this._gameContextMobaData.CooldownReduceMaxLimit = pvpMapCommonInfo.dwCooldownReduceUpperLimit;
            this._gameContextMobaData.IsOpenExpCompensate = pvpMapCommonInfo.bIsOpenExpCompensate > 0;
            this._gameContextMobaData.ExpCompensateInfo = pvpMapCommonInfo.astExpCompensateDetail;
            this._gameContextMobaData.SoldierActivateDelay = pvpMapCommonInfo.iSoldierActivateDelay;
            this._gameContextMobaData.SoldierActivateCountDelay1 = pvpMapCommonInfo.iSoldierActivateCountDelay1;
            this._gameContextMobaData.SoldierActivateCountDelay2 = pvpMapCommonInfo.iSoldierActivateCountDelay2;
            this._gameContextMobaData.TimeDuration = pvpMapCommonInfo.dwTimeDuration;
            this._gameContextMobaData.AddWinCondStarId = pvpMapCommonInfo.dwAddWinCondStarId;
            this._gameContextMobaData.AddLoseCondStarId = pvpMapCommonInfo.dwAddLoseCondStarId;
            this._gameContextMobaData.SecondName = string.Empty;
            this._gameContextMobaData.GameMatchName = pvpMapCommonInfo.szGameMatchName;
            this._gameContextMobaData.IsWarmBattle = false;
            this._gameContextMobaData.WarmHeroAiDiffInfo = null;
            this._gameContextMobaData.IsWarmBattle = isWarmBattle;
            this._gameContextMobaData.WarmHeroAiDiffInfo = !isWarmBattle ? null : GameDataMgr.battleDynamicDifficultyDB.GetDataByKey((long) warmAiLv);
            if (mapType == 4)
            {
                ResEntertainmentLevelInfo dataByKey = GameDataMgr.entertainLevelDatabin.GetDataByKey(mapID);
                this._gameContextMobaData.EntertainmentMapSubType = (RES_ENTERTAINMENT_MAP_SUB_TYPE) dataByKey.bEntertainmentSubType;
            }
            else if (mapType == 5)
            {
                ResRewardMatchLevelInfo info3 = GameDataMgr.uinionBattleLevelDatabin.GetDataByKey(mapID);
                this._gameContextMobaData.SecondName = info3.szMatchName;
            }
        }

        private void _InitSingleGame(SCPKG_STARTSINGLEGAMERSP svrGameInfo)
        {
            this._gameContextCommonData.GameType = (COM_GAME_TYPE) svrGameInfo.bGameType;
            ResLevelCfgInfo levelCfg = null;
            if (this._gameContextCommonData.GameType == COM_GAME_TYPE.COM_SINGLE_GAME_OF_GUIDE)
            {
                levelCfg = GameDataMgr.levelDatabin.GetDataByKey((long) svrGameInfo.iLevelId);
            }
            else if (this._gameContextCommonData.GameType == COM_GAME_TYPE.COM_SINGLE_GAME_OF_ADVENTURE)
            {
                levelCfg = GameDataMgr.levelDatabin.GetDataByKey((long) svrGameInfo.iLevelId);
                this._gameContextCommonData.LevelDifficulty = Singleton<CAdventureSys>.instance.currentDifficulty;
            }
            else if (this._gameContextCommonData.GameType == COM_GAME_TYPE.COM_SINGLE_GAME_OF_BURNING)
            {
                levelCfg = GameDataMgr.burnMap.GetDataByKey((long) Singleton<BurnExpeditionController>.GetInstance().model.Get_LevelID(Singleton<BurnExpeditionController>.GetInstance().model.curSelect_LevelIndex));
            }
            else if (this._gameContextCommonData.GameType == COM_GAME_TYPE.COM_SINGLE_GAME_OF_ARENA)
            {
                levelCfg = GameDataMgr.arenaLevelDatabin.GetDataByKey((long) svrGameInfo.iLevelId);
            }
            else if (this._gameContextCommonData.GameType == COM_GAME_TYPE.COM_SINGLE_GAME_OF_COMBAT)
            {
                uint dwMapId = svrGameInfo.stGameParam.stSingleGameRspOfCombat.dwMapId;
                byte bMapType = svrGameInfo.stGameParam.stSingleGameRspOfCombat.bMapType;
                bool isWarmBattle = svrGameInfo.stGameParam.stSingleGameRspOfCombat.bIsWarmBattle > 0;
                byte bAILevel = svrGameInfo.stGameParam.stSingleGameRspOfCombat.bAILevel;
                this._InitMultiGame((COM_GAME_TYPE) svrGameInfo.bGameType, bMapType, dwMapId, isWarmBattle, bAILevel);
                return;
            }
            this._SetSingleGame(levelCfg);
        }

        private void _InitSynchrConfig(SCPKG_MULTGAME_BEGINLOAD svrGameInfo)
        {
            this._gameContextMobaData.RandomSeed = svrGameInfo.dwRandomSeed;
            this._gameContextMobaData.KFrapsLater = svrGameInfo.bKFrapsLater;
            this._gameContextMobaData.KFrapsFreqMs = svrGameInfo.dwKFrapsFreqMs;
            this._gameContextMobaData.PreActFrap = svrGameInfo.bPreActFrap;
        }

        private void _ResetAllInfo()
        {
            this._gameContextCommonData.GameType = COM_GAME_TYPE.COM_GAME_TYPE_MAX;
            this._gameContextCommonData.IsMultiPlayerGame = false;
            this._gameContextCommonData.MapId = 0;
            this._gameContextCommonData.IsMobaGame = false;
            this._gameContextCommonData.SelectHeroType = enSelectType.enNull;
            this._gameContextCommonData.LevelDifficulty = 0;
            this._gameContextCommonData.HeroAiType = RES_LEVEL_HEROAITYPE.RES_LEVEL_HEROAITYPE_NULL;
            this._gameContextCommonData.SoulGrowActive = false;
            this._gameContextCommonData.BaseReviveTime = 0x3a98;
            this._gameContextCommonData.DynamicPropertyConfigId = 0;
            this._gameContextCommonData.MiniMapPath = string.Empty;
            this._gameContextCommonData.BigMapPath = string.Empty;
            this._gameContextCommonData.MapWidth = 0;
            this._gameContextCommonData.MapHeight = 0;
            this._gameContextCommonData.BigMapWidth = 0;
            this._gameContextCommonData.BigMapHeight = 0;
            this._gameContextCommonData.MapFOWScale = 1f;
            this._gameContextCommonData.BigMapFOWScale = 1f;
            this._gameContextCommonData.MapName = string.Empty;
            this._gameContextCommonData.MapDesignFileName = string.Empty;
            this._gameContextCommonData.MapArtistFileName = string.Empty;
            this._gameContextCommonData.MusicStartEvent = string.Empty;
            this._gameContextCommonData.MusicEndEvent = string.Empty;
            this._gameContextCommonData.AmbientSoundEvent = string.Empty;
            this._gameContextCommonData.MusicBankResName = string.Empty;
            this._gameContextCommonData.HorizonEnableMethod = Horizon.EnableMethod.INVALID;
            this._gameContextCommonData.SoulId = 0;
            this._gameContextCommonData.SoulAllocId = 0;
            this._gameContextCommonData.ExtraMapSkillId = 0;
            this._gameContextCommonData.ExtraMapSkill2Id = 0;
            this._gameContextCommonData.ExtraPassiveSkillId = 0;
            this._gameContextCommonData.OrnamentSkillId = 0;
            this._gameContextCommonData.OrnamentSwitchCd0 = 0;
            this._gameContextCommonData.EnableFOW = false;
            this._gameContextCommonData.EnableShopHorizonTab = false;
            this._gameContextCommonData.EnableOrnamentSlot = false;
            this._gameContextCommonData.CanRightJoyStickCameraDrag = false;
            this._gameContextCommonData.CameraFlip = false;
            this._gameContextMobaData.IsValid = false;
            this._gameContextMobaData.MapType = -1;
            this._gameContextMobaData.OriginalGoldCoinInBattle = 0;
            this._gameContextMobaData.PlayerNum = 0;
            this._gameContextMobaData.BattleEquipLimit = false;
            this._gameContextMobaData.HeadPtsMaxLimit = 0;
            this._gameContextMobaData.BirthLevelConfig = 0;
            this._gameContextMobaData.IsShowHonor = false;
            this._gameContextMobaData.CooldownReduceMaxLimit = 0;
            this._gameContextMobaData.IsOpenExpCompensate = false;
            this._gameContextMobaData.ExpCompensateInfo = null;
            this._gameContextMobaData.SoldierActivateDelay = 0;
            this._gameContextMobaData.SoldierActivateCountDelay1 = 0;
            this._gameContextMobaData.SoldierActivateCountDelay2 = 0;
            this._gameContextMobaData.TimeDuration = 0;
            this._gameContextMobaData.AddWinCondStarId = 0;
            this._gameContextMobaData.AddLoseCondStarId = 0;
            this._gameContextMobaData.SecondName = string.Empty;
            this._gameContextMobaData.GameMatchName = string.Empty;
            this._gameContextMobaData.IsWarmBattle = false;
            this._gameContextMobaData.WarmHeroAiDiffInfo = null;
            this._gameContextMobaData.RandomSeed = 0;
            this._gameContextMobaData.KFrapsLater = 0;
            this._gameContextMobaData.KFrapsFreqMs = 0;
            this._gameContextMobaData.PreActFrap = 0;
            this._gameContextSoloData.IsValid = false;
            this._gameContextSoloData.PveLevelType = RES_LEVEL_TYPE.RES_LEVEL_TYPE_PVP;
            this._gameContextSoloData.ChapterNo = 0;
            this._gameContextSoloData.LevelNo = 0;
            this._gameContextSoloData.FinDialogId = 0;
            this._gameContextSoloData.PreDialogId = 0;
            this._gameContextSoloData.FailureDialogId = 0;
            this._gameContextSoloData.IsShowTrainingHelper = false;
            this._gameContextSoloData.CanAutoGame = false;
            this._gameContextSoloData.ReviveTimeMax = 0;
            this._gameContextSoloData.LoseCondition = 0;
            this._gameContextSoloData.MapBuffs = null;
            this._gameContextSoloData.StarDetail = null;
            this._gameContextSoloData.ReviveInfo = null;
        }

        private void _SetSingleGame(ResLevelCfgInfo levelCfg)
        {
            if (levelCfg != null)
            {
                this._gameContextCommonData.MapId = levelCfg.iCfgID;
                this._gameContextCommonData.IsMobaGame = levelCfg.iLevelType == 0;
                this._gameContextCommonData.SelectHeroType = enSelectType.enMutile;
                this._gameContextCommonData.LevelDifficulty = Math.Max(1, this._gameContextCommonData.LevelDifficulty);
                this._gameContextCommonData.HeroAiType = (RES_LEVEL_HEROAITYPE) levelCfg.iHeroAIType;
                this._gameContextCommonData.SoulGrowActive = levelCfg.bSoulGrow > 0;
                this._gameContextCommonData.BaseReviveTime = (int) levelCfg.dwReviveTime;
                this._gameContextCommonData.DynamicPropertyConfigId = levelCfg.dwDynamicPropertyCfg;
                this._gameContextCommonData.MiniMapPath = levelCfg.szThumbnailPath;
                this._gameContextCommonData.BigMapPath = levelCfg.szBigMapPath;
                this._gameContextCommonData.MapWidth = levelCfg.iMapWidth;
                this._gameContextCommonData.MapHeight = levelCfg.iMapHeight;
                this._gameContextCommonData.BigMapWidth = levelCfg.iBigMapWidth;
                this._gameContextCommonData.BigMapHeight = levelCfg.iBigMapHeight;
                this._gameContextCommonData.MapFOWScale = 1f;
                this._gameContextCommonData.BigMapFOWScale = 1f;
                this._gameContextCommonData.MapName = levelCfg.szName;
                this._gameContextCommonData.MapDesignFileName = levelCfg.szDesignFileName;
                this._gameContextCommonData.MapArtistFileName = levelCfg.szArtistFileName;
                this._gameContextCommonData.MusicStartEvent = levelCfg.szMusicStartEvent;
                this._gameContextCommonData.MusicEndEvent = levelCfg.szMusicEndEvent;
                this._gameContextCommonData.AmbientSoundEvent = levelCfg.szAmbientSoundEvent;
                this._gameContextCommonData.MusicBankResName = levelCfg.szBankResourceName;
                this._gameContextCommonData.HorizonEnableMethod = !this._gameContextCommonData.IsMobaGame ? ((Horizon.EnableMethod) levelCfg.bEnableHorizon) : Horizon.EnableMethod.EnableAll;
                this._gameContextCommonData.SoulId = levelCfg.dwSoulID;
                this._gameContextCommonData.SoulAllocId = levelCfg.dwSoulAllocId;
                this._gameContextCommonData.ExtraMapSkillId = levelCfg.iExtraSkillId;
                this._gameContextCommonData.ExtraMapSkill2Id = levelCfg.iExtraSkill2Id;
                this._gameContextCommonData.ExtraPassiveSkillId = levelCfg.iExtraPassiveSkillId;
                this._gameContextCommonData.OrnamentSkillId = 0;
                this._gameContextCommonData.OrnamentSwitchCd0 = 0;
                this._gameContextCommonData.EnableFOW = false;
                this._gameContextCommonData.EnableShopHorizonTab = false;
                this._gameContextCommonData.EnableOrnamentSlot = false;
                this._gameContextCommonData.CanRightJoyStickCameraDrag = levelCfg.bSupportCameraDrag > 0;
                this._gameContextCommonData.CameraFlip = false;
                this._gameContextSoloData.IsValid = true;
                this._gameContextSoloData.PveLevelType = (RES_LEVEL_TYPE) levelCfg.iLevelType;
                this._gameContextSoloData.ChapterNo = levelCfg.iChapterId;
                this._gameContextSoloData.LevelNo = levelCfg.bLevelNo;
                this._gameContextSoloData.FinDialogId = levelCfg.iPassDialogId;
                this._gameContextSoloData.PreDialogId = levelCfg.iPreDialogId;
                this._gameContextSoloData.FailureDialogId = levelCfg.iFailureDialogId;
                this._gameContextSoloData.IsShowTrainingHelper = levelCfg.bShowTrainingHelper > 0;
                this._gameContextSoloData.CanAutoGame = levelCfg.bIsOpenAutoAI == 0;
                this._gameContextSoloData.ReviveTimeMax = levelCfg.bReviveTimeMax;
                this._gameContextSoloData.LoseCondition = levelCfg.iLoseCondition;
                this._gameContextSoloData.MapBuffs = levelCfg.astMapBuffs;
                this._gameContextSoloData.StarDetail = levelCfg.astStarDetail;
                this._gameContextSoloData.ReviveInfo = levelCfg.astReviveInfo;
            }
        }

        public ReadonlyContext<Camp> GetAllValidCamps(Camp camp, out uint validCount)
        {
            validCount = 1;
            return new ReadonlyContext<Camp>(this._validCampList);
        }

        public ReadonlyContext<Camp> GetEnemyCamps(Camp camp, out uint validCount)
        {
            validCount = 1;
            return new ReadonlyContext<Camp>(this._enemyCampList);
        }

        public COM_GAME_TYPE GetGameType()
        {
            return this._gameContextCommonData.GameType;
        }

        public ResDT_LevelCommonInfo GetPvpMapCommonInfo(int mapType, uint mapId)
        {
            ResDT_LevelCommonInfo stLevelCommonInfo = null;
            switch (mapType)
            {
                case 1:
                    stLevelCommonInfo = GameDataMgr.pvpLevelDatabin.GetDataByKey(mapId).stLevelCommonInfo;
                    break;

                case 2:
                    stLevelCommonInfo = GameDataMgr.cpLevelDatabin.GetDataByKey(mapId).stLevelCommonInfo;
                    break;

                case 3:
                    stLevelCommonInfo = GameDataMgr.rankLevelDatabin.GetDataByKey(mapId).stLevelCommonInfo;
                    break;

                case 4:
                    stLevelCommonInfo = GameDataMgr.entertainLevelDatabin.GetDataByKey(mapId).stLevelCommonInfo;
                    break;

                case 5:
                    stLevelCommonInfo = GameDataMgr.uinionBattleLevelDatabin.GetDataByKey(mapId).stLevelCommonInfo;
                    break;
            }
            if (stLevelCommonInfo == null)
            {
            }
            return stLevelCommonInfo;
        }

        public enSelectType GetSelectHeroType()
        {
            return this._gameContextCommonData.SelectHeroType;
        }

        public uint GetValidCampCount()
        {
            return 3;
        }

        public Camp GetValidFirstCamp()
        {
            return Camp.CAMP_MID;
        }

        public Camp GetValidLastCamp()
        {
            return Camp.CAMP_2;
        }

        public override void Init()
        {
            this._ResetAllInfo();
            this.InitCampData();
        }

        private void InitCampData()
        {
            for (int i = 0; i < 3; i++)
            {
                this._validCampList.Add(Camp.CAMP_1);
                this._enemyCampList.Add(Camp.CAMP_1);
            }
        }

        public void InitMultiGame(SCPKG_MULTGAME_BEGINLOAD svrGameInfo)
        {
            this._ResetAllInfo();
            if (svrGameInfo != null)
            {
                uint dwMapId = svrGameInfo.stDeskInfo.dwMapId;
                byte bMapType = svrGameInfo.stDeskInfo.bMapType;
                bool isWarmBattle = svrGameInfo.stDeskInfo.bIsWarmBattle > 0;
                byte bAILevel = svrGameInfo.stDeskInfo.bAILevel;
                this._InitMultiGame((COM_GAME_TYPE) svrGameInfo.bGameType, bMapType, dwMapId, isWarmBattle, bAILevel);
                this._InitSynchrConfig(svrGameInfo);
            }
        }

        public void InitSingleGame(SCPKG_STARTSINGLEGAMERSP svrGameInfo)
        {
            this._ResetAllInfo();
            if (svrGameInfo != null)
            {
                this._InitSingleGame(svrGameInfo);
            }
        }

        public bool IsChasmChaosBattle()
        {
            return (this._gameContextCommonData.MapId == 0x15f91);
        }

        public bool IsFireHolePlayMode()
        {
            return ((this._gameContextCommonData.GameType == COM_GAME_TYPE.COM_MULTI_GAME_OF_ENTERTAINMENT) && (this._gameContextMobaData.EntertainmentMapSubType == RES_ENTERTAINMENT_MAP_SUB_TYPE.RES_ENTERTAINMENT_MAP_SUB_TYPE_FIRE_HOLE));
        }

        public bool IsGameTypeActivity()
        {
            return (this._gameContextCommonData.GameType == COM_GAME_TYPE.COM_SINGLE_GAME_OF_ACTIVITY);
        }

        public bool IsGameTypeAdventure()
        {
            return (this._gameContextCommonData.GameType == COM_GAME_TYPE.COM_SINGLE_GAME_OF_ADVENTURE);
        }

        public bool IsGameTypeArena()
        {
            return (this._gameContextCommonData.GameType == COM_GAME_TYPE.COM_SINGLE_GAME_OF_ARENA);
        }

        public bool IsGameTypeBurning()
        {
            return (this._gameContextCommonData.GameType == COM_GAME_TYPE.COM_SINGLE_GAME_OF_BURNING);
        }

        public bool IsGameTypeComBat()
        {
            return (this._gameContextCommonData.GameType == COM_GAME_TYPE.COM_SINGLE_GAME_OF_COMBAT);
        }

        public bool IsGameTypeEntertainment()
        {
            return (this._gameContextCommonData.GameType == COM_GAME_TYPE.COM_MULTI_GAME_OF_ENTERTAINMENT);
        }

        public bool IsGameTypeGuide()
        {
            return (this._gameContextCommonData.GameType == COM_GAME_TYPE.COM_SINGLE_GAME_OF_GUIDE);
        }

        public bool IsGameTypeLadder()
        {
            return (this._gameContextCommonData.GameType == COM_GAME_TYPE.COM_MULTI_GAME_OF_LADDER);
        }

        public bool IsGameTypePvpMatch()
        {
            return (this._gameContextCommonData.GameType == COM_GAME_TYPE.COM_MULTI_GAME_OF_PVP_MATCH);
        }

        public bool IsGameTypePvpRoom()
        {
            return (this._gameContextCommonData.GameType == COM_GAME_TYPE.COM_MULTI_GAME_OF_PVP_ROOM);
        }

        public bool IsGameTypeRewardMatch()
        {
            return (this._gameContextCommonData.GameType == COM_GAME_TYPE.COM_MULTI_GAME_OF_REWARDMATCH);
        }

        public bool IsLuanDouPlayMode()
        {
            return ((this._gameContextCommonData.GameType == COM_GAME_TYPE.COM_MULTI_GAME_OF_ENTERTAINMENT) && (this._gameContextMobaData.EntertainmentMapSubType == RES_ENTERTAINMENT_MAP_SUB_TYPE.RES_ENTERTAINMENT_MAP_SUB_TYPE_CHAOS_BATTLE));
        }

        public bool IsMobaMode()
        {
            return this._gameContextCommonData.IsMobaGame;
        }

        public bool IsMobaModeWithOutGuide()
        {
            return (this.IsMobaMode() && !this.IsGameTypeGuide());
        }

        public bool IsMultilModeWithoutWarmBattle()
        {
            return (this._gameContextCommonData.IsMobaGame && !this._gameContextMobaData.IsWarmBattle);
        }

        public bool IsMultilModeWithWarmBattle()
        {
            return (this._gameContextMobaData.IsWarmBattle || this.IsMultilModeWithoutWarmBattle());
        }

        public bool IsMultiPlayerGame()
        {
            return this._gameContextCommonData.IsMultiPlayerGame;
        }

        public virtual bool IsSoulGrow()
        {
            return this._gameContextCommonData.SoulGrowActive;
        }

        public void Test()
        {
            uint validCount = 1;
            ReadonlyContext<Camp> enemyCamps = this.GetEnemyCamps(Camp.CAMP_1, out validCount);
            for (int i = 0; i < enemyCamps.Count; i++)
            {
            }
        }

        public CommonGameContextData GameContextCommonInfo
        {
            get
            {
                return this._gameContextCommonData;
            }
        }

        public MobaData GameContextMobaInfo
        {
            get
            {
                return this._gameContextMobaData;
            }
        }

        public SoloGameData GameContextSoloInfo
        {
            get
            {
                return this._gameContextSoloData;
            }
        }

        public enum Camp
        {
            CAMP_MID,
            CAMP_1,
            CAMP_2,
            CAMP_MAX
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CommonGameContextData
        {
            public COM_GAME_TYPE GameType;
            public bool IsMultiPlayerGame;
            public int MapId;
            public bool IsMobaGame;
            public enSelectType SelectHeroType;
            public int LevelDifficulty;
            public RES_LEVEL_HEROAITYPE HeroAiType;
            public bool SoulGrowActive;
            public int BaseReviveTime;
            public uint DynamicPropertyConfigId;
            public string MiniMapPath;
            public string BigMapPath;
            public int MapWidth;
            public int MapHeight;
            public int BigMapWidth;
            public int BigMapHeight;
            public float MapFOWScale;
            public float BigMapFOWScale;
            public string MapName;
            public string MapDesignFileName;
            public string MapArtistFileName;
            public string MusicStartEvent;
            public string MusicEndEvent;
            public string AmbientSoundEvent;
            public string MusicBankResName;
            public Horizon.EnableMethod HorizonEnableMethod;
            public uint SoulId;
            public uint SoulAllocId;
            public int ExtraMapSkillId;
            public int ExtraMapSkill2Id;
            public int ExtraPassiveSkillId;
            public int OrnamentSkillId;
            public int OrnamentSwitchCd0;
            public bool EnableFOW;
            public bool EnableShopHorizonTab;
            public bool EnableOrnamentSlot;
            public bool CanRightJoyStickCameraDrag;
            public bool CameraFlip;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MobaData
        {
            public bool IsValid;
            public int MapType;
            public RES_ENTERTAINMENT_MAP_SUB_TYPE EntertainmentMapSubType;
            public ushort OriginalGoldCoinInBattle;
            public int PlayerNum;
            public bool BattleEquipLimit;
            public int HeadPtsMaxLimit;
            public int BirthLevelConfig;
            public bool IsShowHonor;
            public uint CooldownReduceMaxLimit;
            public bool IsOpenExpCompensate;
            public ResDT_ExpCompensateInfo[] ExpCompensateInfo;
            public int SoldierActivateDelay;
            public int SoldierActivateCountDelay1;
            public int SoldierActivateCountDelay2;
            public uint TimeDuration;
            public uint AddWinCondStarId;
            public uint AddLoseCondStarId;
            public string SecondName;
            public string GameMatchName;
            public bool IsWarmBattle;
            public ResBattleDynamicDifficulty WarmHeroAiDiffInfo;
            public byte KFrapsLater;
            public uint KFrapsFreqMs;
            public byte PreActFrap;
            public uint RandomSeed;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SoloGameData
        {
            public bool IsValid;
            public RES_LEVEL_TYPE PveLevelType;
            public int ChapterNo;
            public int LevelNo;
            public int FinDialogId;
            public int PreDialogId;
            public int FailureDialogId;
            public bool IsShowTrainingHelper;
            public bool CanAutoGame;
            public int ReviveTimeMax;
            public int LoseCondition;
            public ResDT_MapBuff[] MapBuffs;
            public ResDT_IntParamArrayNode[] StarDetail;
            public ResDT_PveReviveInfo[] ReviveInfo;
        }
    }
}

