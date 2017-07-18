namespace Assets.Scripts.GameLogic.GameKernal
{
    using Apollo;
    using Assets.Scripts.Framework;
    using CSProtocol;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class GameReportor
    {
        private readonly List<KeyValuePair<string, string>> _eventsLoadingTime = new List<KeyValuePair<string, string>>();

        public void DoApolloReport()
        {
            string openID = Singleton<ApolloHelper>.GetInstance().GetOpenID();
            int mapId = Singleton<GameContextEx>.GetInstance().GameContextCommonInfo.MapId;
            COM_GAME_TYPE gameType = Singleton<GameContextEx>.GetInstance().GameContextCommonInfo.GameType;
            List<KeyValuePair<string, string>> events = new List<KeyValuePair<string, string>>();
            events.Add(new KeyValuePair<string, string>("g_version", CVersion.GetAppVersion()));
            events.Add(new KeyValuePair<string, string>("WorldID", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString()));
            events.Add(new KeyValuePair<string, string>("platform", Singleton<ApolloHelper>.GetInstance().CurPlatform.ToString()));
            events.Add(new KeyValuePair<string, string>("openid", openID));
            events.Add(new KeyValuePair<string, string>("GameType", gameType.ToString()));
            events.Add(new KeyValuePair<string, string>("MapID", mapId.ToString()));
            events.Add(new KeyValuePair<string, string>("LoadingTime", Singleton<GameBuilderEx>.GetInstance().LastLoadingTime.ToString()));
            Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Service_LoadingBattle", events, true);
            List<KeyValuePair<string, string>> list2 = new List<KeyValuePair<string, string>>();
            list2.Add(new KeyValuePair<string, string>("g_version", CVersion.GetAppVersion()));
            list2.Add(new KeyValuePair<string, string>("WorldID", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString()));
            list2.Add(new KeyValuePair<string, string>("platform", Singleton<ApolloHelper>.GetInstance().CurPlatform.ToString()));
            list2.Add(new KeyValuePair<string, string>("openid", openID));
            list2.Add(new KeyValuePair<string, string>("totaltime", Singleton<CHeroSelectBaseSystem>.instance.m_fOpenHeroSelectForm.ToString()));
            list2.Add(new KeyValuePair<string, string>("gameType", gameType.ToString()));
            list2.Add(new KeyValuePair<string, string>("role_list", string.Empty));
            list2.Add(new KeyValuePair<string, string>("errorCode", string.Empty));
            list2.Add(new KeyValuePair<string, string>("error_msg", string.Empty));
            Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Service_Login_EnterGame", list2, true);
            float num2 = Singleton<FrameSynchr>.GetInstance().LogicFrameTick * 0.001f;
            Singleton<FrameSynchr>.GetInstance().PingVariance();
            List<KeyValuePair<string, string>> list3 = new List<KeyValuePair<string, string>>();
            list3.Add(new KeyValuePair<string, string>("g_version", CVersion.GetAppVersion()));
            list3.Add(new KeyValuePair<string, string>("WorldID", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString()));
            list3.Add(new KeyValuePair<string, string>("platform", Singleton<ApolloHelper>.GetInstance().CurPlatform.ToString()));
            list3.Add(new KeyValuePair<string, string>("openid", openID));
            list3.Add(new KeyValuePair<string, string>("GameType", gameType.ToString()));
            list3.Add(new KeyValuePair<string, string>("MapID", mapId.ToString()));
            list3.Add(new KeyValuePair<string, string>("Max_FPS", Singleton<CBattleSystem>.GetInstance().m_MaxBattleFPS.ToString()));
            list3.Add(new KeyValuePair<string, string>("Min_FPS", Singleton<CBattleSystem>.GetInstance().m_MinBattleFPS.ToString()));
            float num3 = -1f;
            if (Singleton<CBattleSystem>.GetInstance().m_BattleFPSCount > 0f)
            {
                num3 = Singleton<CBattleSystem>.GetInstance().m_AveBattleFPS / Singleton<CBattleSystem>.GetInstance().m_BattleFPSCount;
            }
            list3.Add(new KeyValuePair<string, string>("Avg_FPS", num3.ToString()));
            list3.Add(new KeyValuePair<string, string>("Ab_FPS_time", Singleton<BattleLogic>.GetInstance().m_Ab_FPS_time.ToString()));
            list3.Add(new KeyValuePair<string, string>("Abnormal_FPS", Singleton<BattleLogic>.GetInstance().m_Abnormal_FPS_Count.ToString()));
            list3.Add(new KeyValuePair<string, string>("Less10FPSCount", Singleton<BattleLogic>.GetInstance().m_fpsCunt10.ToString()));
            list3.Add(new KeyValuePair<string, string>("Less18FPSCount", Singleton<BattleLogic>.GetInstance().m_fpsCunt18.ToString()));
            list3.Add(new KeyValuePair<string, string>("Ab_4FPS_time", Singleton<BattleLogic>.GetInstance().m_Ab_4FPS_time.ToString()));
            list3.Add(new KeyValuePair<string, string>("Abnormal_4FPS", Singleton<BattleLogic>.GetInstance().m_Abnormal_4FPS_Count.ToString()));
            list3.Add(new KeyValuePair<string, string>("Min_Ping", Singleton<FrameSynchr>.instance.m_MinPing.ToString()));
            list3.Add(new KeyValuePair<string, string>("Max_Ping", Singleton<FrameSynchr>.instance.m_MaxPing.ToString()));
            list3.Add(new KeyValuePair<string, string>("Avg_Ping", Singleton<FrameSynchr>.instance.m_AvePing.ToString()));
            list3.Add(new KeyValuePair<string, string>("Abnormal_Ping", Singleton<FrameSynchr>.instance.m_Abnormal_PingCount.ToString()));
            list3.Add(new KeyValuePair<string, string>("Ping300", Singleton<FrameSynchr>.instance.m_ping300Count.ToString()));
            list3.Add(new KeyValuePair<string, string>("Ping150to300", Singleton<FrameSynchr>.instance.m_ping150to300.ToString()));
            list3.Add(new KeyValuePair<string, string>("Ping150", Singleton<FrameSynchr>.instance.m_ping150.ToString()));
            list3.Add(new KeyValuePair<string, string>("LostpingCount", Singleton<FrameSynchr>.instance.m_pingLost.ToString()));
            list3.Add(new KeyValuePair<string, string>("PingSeqCount", Singleton<FrameSynchr>.instance.m_LastReceiveHeartSeq.ToString()));
            list3.Add(new KeyValuePair<string, string>("PingVariance", Singleton<FrameSynchr>.instance.m_PingVariance.ToString()));
            list3.Add(new KeyValuePair<string, string>("Battle_Time", num2.ToString()));
            list3.Add(new KeyValuePair<string, string>("BattleSvr_Reconnect", Singleton<NetworkModule>.GetInstance().m_GameReconnetCount.ToString()));
            list3.Add(new KeyValuePair<string, string>("GameSvr_Reconnect", Singleton<NetworkModule>.GetInstance().m_lobbyReconnetCount.ToString()));
            list3.Add(new KeyValuePair<string, string>("music", GameSettings.EnableMusic.ToString()));
            list3.Add(new KeyValuePair<string, string>("quality", GameSettings.RenderQuality.ToString()));
            list3.Add(new KeyValuePair<string, string>("status", "1"));
            list3.Add(new KeyValuePair<string, string>("Quality_Mode", GameSettings.ModelLOD.ToString()));
            list3.Add(new KeyValuePair<string, string>("Quality_Particle", GameSettings.ParticleLOD.ToString()));
            list3.Add(new KeyValuePair<string, string>("receiveMoveCmdAverage", Singleton<FrameSynchr>.instance.m_receiveMoveCmdAverage.ToString()));
            list3.Add(new KeyValuePair<string, string>("receiveMoveCmdMax", Singleton<FrameSynchr>.instance.m_receiveMoveCmdMax.ToString()));
            list3.Add(new KeyValuePair<string, string>("execMoveCmdAverage", Singleton<FrameSynchr>.instance.m_execMoveCmdAverage.ToString()));
            list3.Add(new KeyValuePair<string, string>("execMoveCmdMax", Singleton<FrameSynchr>.instance.m_execMoveCmdMax.ToString()));
            list3.Add(new KeyValuePair<string, string>("LOD_Down", Singleton<BattleLogic>.GetInstance().m_iAutoLODState.ToString()));
            if (NetworkAccelerator.started)
            {
                if (NetworkAccelerator.isAccerating())
                {
                    list3.Add(new KeyValuePair<string, string>("AccState", "Acc"));
                }
                else
                {
                    list3.Add(new KeyValuePair<string, string>("AccState", "Direct"));
                }
            }
            else
            {
                list3.Add(new KeyValuePair<string, string>("AccState", "Off"));
            }
            int num4 = 0;
            if (MonoSingleton<VoiceSys>.GetInstance().UseSpeak && MonoSingleton<VoiceSys>.GetInstance().UseMic)
            {
                num4 = 2;
            }
            else if (MonoSingleton<VoiceSys>.GetInstance().UseSpeak)
            {
                num4 = 1;
            }
            list3.Add(new KeyValuePair<string, string>("Mic", num4.ToString()));
            Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Service_PVPBattle_Summary", list3, true);
            this._eventsLoadingTime.Clear();
            try
            {
                float num5 = ((float) Singleton<BattleLogic>.GetInstance().m_fpsCunt10) / ((float) Singleton<BattleLogic>.GetInstance().m_fpsCount);
                int num6 = Mathf.CeilToInt((num5 * 100f) / 10f) * 10;
                float num7 = ((float) (Singleton<BattleLogic>.GetInstance().m_fpsCunt18 + Singleton<BattleLogic>.GetInstance().m_fpsCunt10)) / ((float) Singleton<BattleLogic>.GetInstance().m_fpsCount);
                int num8 = Mathf.CeilToInt((num7 * 100f) / 10f) * 10;
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x1388);
                msg.stPkgData.stCltPerformance.iMapID = mapId;
                msg.stPkgData.stCltPerformance.iPlayerCnt = Singleton<GamePlayerCenter>.instance.GetAllPlayers().Count;
                msg.stPkgData.stCltPerformance.chModelLOD = (sbyte) GameSettings.ModelLOD;
                msg.stPkgData.stCltPerformance.chParticleLOD = (sbyte) GameSettings.ParticleLOD;
                msg.stPkgData.stCltPerformance.chCameraHeight = (sbyte) GameSettings.CameraHeight;
                msg.stPkgData.stCltPerformance.chEnableOutline = !GameSettings.EnableOutline ? ((sbyte) 0) : ((sbyte) 1);
                msg.stPkgData.stCltPerformance.iFps10PercentNum = num6;
                msg.stPkgData.stCltPerformance.iFps18PercentNum = num8;
                msg.stPkgData.stCltPerformance.iAveFps = (int) Singleton<CBattleSystem>.GetInstance().m_AveBattleFPS;
                msg.stPkgData.stCltPerformance.iPingAverage = Singleton<FrameSynchr>.instance.m_PingAverage;
                msg.stPkgData.stCltPerformance.iPingVariance = Singleton<FrameSynchr>.instance.m_PingVariance;
                Utility.StringToByteArray(SystemInfo.get_deviceModel(), ref msg.stPkgData.stCltPerformance.szDeviceModel);
                Utility.StringToByteArray(SystemInfo.get_graphicsDeviceName(), ref msg.stPkgData.stCltPerformance.szGPUName);
                msg.stPkgData.stCltPerformance.iCpuCoreNum = SystemInfo.get_processorCount();
                msg.stPkgData.stCltPerformance.iSysMemorySize = SystemInfo.get_systemMemorySize();
                msg.stPkgData.stCltPerformance.iAvailMemory = DeviceCheckSys.GetAvailMemory();
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
            }
            catch (Exception exception)
            {
                Debug.Log(exception.Message);
            }
        }

        public void PrepareReport()
        {
            this._eventsLoadingTime.Clear();
            ApolloAccountInfo accountInfo = Singleton<ApolloHelper>.GetInstance().GetAccountInfo(false);
            DebugHelper.Assert(accountInfo != null, "account info is null");
            this._eventsLoadingTime.Add(new KeyValuePair<string, string>("OpenID", (accountInfo == null) ? "0" : accountInfo.OpenId));
            this._eventsLoadingTime.Add(new KeyValuePair<string, string>("LevelID", Singleton<GameContextEx>.GetInstance().GameContextCommonInfo.MapId.ToString()));
            this._eventsLoadingTime.Add(new KeyValuePair<string, string>("isPVPLevel", Singleton<GameContextEx>.GetInstance().IsMobaMode().ToString()));
            this._eventsLoadingTime.Add(new KeyValuePair<string, string>("isPVPMode", Singleton<GameContextEx>.GetInstance().IsMobaMode().ToString()));
            this._eventsLoadingTime.Add(new KeyValuePair<string, string>("bLevelNo", Singleton<GameContextEx>.GetInstance().GameContextSoloInfo.LevelNo.ToString()));
        }
    }
}

