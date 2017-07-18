namespace Assets.Scripts.Framework
{
    using CSProtocol;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    [MessageHandlerClass]
    public class FrameSynchr : Singleton<FrameSynchr>, IGameModule
    {
        private bool _bActive;
        private int _CurPkgDelay;
        [CompilerGenerated]
        private uint <CurFrameNum>k__BackingField;
        [CompilerGenerated]
        private uint <EndFrameNum>k__BackingField;
        [CompilerGenerated]
        private bool <isCmdExecuting>k__BackingField;
        [CompilerGenerated]
        private ulong <LogicFrameTick>k__BackingField;
        [CompilerGenerated]
        private int <nJitterDelay>k__BackingField;
        [CompilerGenerated]
        private long <nMultiFrameDelta>k__BackingField;
        [CompilerGenerated]
        private float <startFrameTime>k__BackingField;
        private int AvgFrameDelay;
        private uint backstepFrameCounter;
        private bool bRunning;
        public bool bShowJitterChart;
        public uint CacheSetLater;
        private Queue<IFrameCommand> commandQueue = new Queue<IFrameCommand>();
        private uint EndBlockWaitNum;
        private const int FrameDelay_Limit = 200;
        public uint FrameDelta = 0x42;
        private byte frameSpeed = 1;
        public int GameSvrPing;
        private const float JitterCoverage = 0.85f;
        private int JitterDamper;
        private int JitterDelay;
        private uint KeyFrameRate = 1;
        public int m_Abnormal_PingCount;
        public uint m_AvePing;
        public int m_execMoveCmdAverage;
        public uint m_execMoveCmdMax;
        public ulong m_ExecMoveCmdTotalCount;
        public uint m_LastPing;
        public long m_LastReceiveHeartSeq;
        public uint m_MaxPing;
        public uint m_MinPing = uint.MaxValue;
        public uint[] m_MoveCMDReceiveTime = new uint[0x100];
        public uint[] m_MoveCMDSendTime = new uint[0x100];
        public float m_NetAccPingTimeBegin;
        public bool m_NetAccSetIPAndPortInvoked;
        public Ping m_ping;
        public long m_ping150;
        public long m_ping150to300;
        public long m_ping300Count;
        public int m_PingAverage;
        public uint m_PingIdx;
        public long m_pingLost;
        public List<uint> m_pingRecords = new List<uint>();
        public float m_PingTimeBegin;
        public int m_PingVariance;
        public List<uint> m_pingWobble = new List<uint>();
        public uint m_realpingStartTime;
        public int m_receiveMoveCmdAverage;
        public uint m_receiveMoveCmdMax;
        public ulong m_receiveMoveCmdtotalCount;
        public long m_SendHeartSeq;
        public const byte MAX_FRAME_SPEED = 8;
        public const byte MIN_FRAME_SPEED = 1;
        public int nDriftFactor = 4;
        public int PauseCancelCount;
        public uint PreActFrames = 5;
        private uint ServerSeed = 0x3039;
        private const int StatDelayCnt = 30;
        private int StatDelayIdx;
        private int[] StatDelaySet = new int[30];
        public uint SvrFrameDelta = 0x42;
        private uint SvrFrameIndex;
        public uint SvrFrameLater;
        public int tryCount;
        private uint uCommandId;

        public void CacheFrameLater(uint frameLater)
        {
            this.CacheSetLater = frameLater;
        }

        public void CalcBackstepTimeSinceStart(uint inSvrNum)
        {
            if (!Singleton<WatchController>.instance.IsRelayCast && (this.backstepFrameCounter != inSvrNum))
            {
                float num = Time.get_realtimeSinceStartup();
                ulong num2 = inSvrNum * this.SvrFrameDelta;
                float num3 = num - (num2 * 0.001f);
                float num4 = num3 - this.startFrameTime;
                if (num4 < 0f)
                {
                    this.startFrameTime = num3;
                }
                this.backstepFrameCounter = inSvrNum;
            }
        }

        public int CalculateJitterDelay(long nDelayMs)
        {
            this._CurPkgDelay = (nDelayMs <= 0L) ? 0 : ((int) nDelayMs);
            this.AvgFrameDelay = ((0x1d * this.AvgFrameDelay) + this._CurPkgDelay) / 30;
            return this.AvgFrameDelay;
        }

        private void ClearMoveCMDTime()
        {
            this.m_receiveMoveCmdAverage = 0;
            this.m_execMoveCmdAverage = 0;
            this.m_receiveMoveCmdMax = 0;
            this.m_execMoveCmdMax = 0;
            this.m_receiveMoveCmdtotalCount = 0L;
            this.m_ExecMoveCmdTotalCount = 0L;
            for (int i = 0; i < this.m_MoveCMDSendTime.Length; i++)
            {
                this.m_MoveCMDSendTime[i] = 0;
            }
            for (int j = 0; j < this.m_MoveCMDReceiveTime.Length; j++)
            {
                this.m_MoveCMDReceiveTime[j] = 0;
            }
        }

        private CreatorDelegate GetCreator(Type InType)
        {
            MethodInfo[] methods = InType.GetMethods();
            for (int i = 0; (methods != null) && (i < methods.Length); i++)
            {
                MethodInfo method = methods[i];
                if (method.IsStatic)
                {
                    object[] customAttributes = method.GetCustomAttributes(typeof(FrameCommandCreatorAttribute), true);
                    for (int j = 0; j < customAttributes.Length; j++)
                    {
                        if (customAttributes[j] is FrameCommandCreatorAttribute)
                        {
                            return (CreatorDelegate) Delegate.CreateDelegate(typeof(CreatorDelegate), method);
                        }
                    }
                }
            }
            return null;
        }

        private CreatorCSSyncDelegate GetCSSyncCreator(Type InType)
        {
            MethodInfo[] methods = InType.GetMethods();
            for (int i = 0; (methods != null) && (i < methods.Length); i++)
            {
                MethodInfo method = methods[i];
                if (method.IsStatic)
                {
                    object[] customAttributes = method.GetCustomAttributes(typeof(FrameCommandCreatorAttribute), true);
                    for (int j = 0; j < customAttributes.Length; j++)
                    {
                        if (customAttributes[j] is FrameCommandCreatorAttribute)
                        {
                            return (CreatorCSSyncDelegate) Delegate.CreateDelegate(typeof(CreatorCSSyncDelegate), method);
                        }
                    }
                }
            }
            return null;
        }

        public override void Init()
        {
            FrameCommandFactory.PrepareRegisterCommand();
            Type[] types = typeof(FrameSynchr).Assembly.GetTypes();
            for (int i = 0; (types != null) && (i < types.Length); i++)
            {
                Type inType = types[i];
                object[] customAttributes = inType.GetCustomAttributes(typeof(FrameCommandClassAttribute), true);
                for (int j = 0; j < customAttributes.Length; j++)
                {
                    FrameCommandClassAttribute attribute = customAttributes[j] as FrameCommandClassAttribute;
                    if (attribute != null)
                    {
                        CreatorDelegate creator = this.GetCreator(inType);
                        if (creator != null)
                        {
                            FrameCommandFactory.RegisterCommandCreator(attribute.ID, inType, creator);
                        }
                    }
                }
                object[] objArray2 = inType.GetCustomAttributes(typeof(FrameCSSYNCCommandClassAttribute), true);
                for (int k = 0; k < objArray2.Length; k++)
                {
                    FrameCSSYNCCommandClassAttribute attribute2 = objArray2[k] as FrameCSSYNCCommandClassAttribute;
                    if (attribute2 != null)
                    {
                        CreatorCSSyncDelegate cSSyncCreator = this.GetCSSyncCreator(inType);
                        if (cSSyncCreator != null)
                        {
                            FrameCommandFactory.RegisterCSSyncCommandCreator(attribute2.ID, inType, cSSyncCreator);
                        }
                    }
                }
                object[] objArray3 = inType.GetCustomAttributes(typeof(FrameSCSYNCCommandClassAttribute), true);
                for (int m = 0; m < objArray3.Length; m++)
                {
                    FrameSCSYNCCommandClassAttribute attribute3 = objArray3[m] as FrameSCSYNCCommandClassAttribute;
                    if (attribute3 != null)
                    {
                        FrameCommandFactory.RegisterSCSyncCommandCreator(attribute3.ID, inType, null);
                    }
                }
            }
            this.ResetSynchr();
        }

        [MessageHandler(0x4ec)]
        public static void onRelaySvrPingMsg(CSPkg msg)
        {
            long dwSeqNo = msg.stPkgData.stRelaySvrPing.dwSeqNo;
            if (Singleton<FrameSynchr>.instance.m_LastReceiveHeartSeq <= 0L)
            {
                Singleton<FrameSynchr>.instance.m_LastReceiveHeartSeq = dwSeqNo;
            }
            else
            {
                long num2 = dwSeqNo - Singleton<FrameSynchr>.instance.m_LastReceiveHeartSeq;
                if (num2 >= 2L)
                {
                    FrameSynchr instance = Singleton<FrameSynchr>.instance;
                    instance.m_pingLost += num2;
                }
                Singleton<FrameSynchr>.instance.m_LastReceiveHeartSeq = dwSeqNo;
            }
            if (Singleton<FrameSynchr>.instance.bActive && Singleton<FrameSynchr>.instance.bRunning)
            {
                uint item = ((uint) (Time.get_realtimeSinceStartup() * 1000f)) - msg.stPkgData.stRelaySvrPing.dwTime;
                if (Singleton<FrameSynchr>.GetInstance().m_SendHeartSeq == (dwSeqNo + 1L))
                {
                    Singleton<FrameSynchr>.instance.m_pingRecords.Add(item);
                    FrameSynchr local2 = Singleton<FrameSynchr>.instance;
                    local2.m_PingIdx++;
                    Singleton<FrameSynchr>.instance.m_AvePing = ((Singleton<FrameSynchr>.instance.m_AvePing * (Singleton<FrameSynchr>.instance.m_PingIdx - 1)) + item) / Singleton<FrameSynchr>.instance.m_PingIdx;
                    Singleton<FrameSynchr>.instance.m_pingWobble.Add((uint) Mathf.Abs((float) (item - Singleton<FrameSynchr>.instance.m_AvePing)));
                    if (item > Singleton<FrameSynchr>.instance.m_MaxPing)
                    {
                        Singleton<FrameSynchr>.instance.m_MaxPing = item;
                    }
                    if (item < Singleton<FrameSynchr>.instance.m_MinPing)
                    {
                        Singleton<FrameSynchr>.instance.m_MinPing = item;
                    }
                    if (item > 300)
                    {
                        FrameSynchr local3 = Singleton<FrameSynchr>.instance;
                        local3.m_ping300Count += 1L;
                    }
                    if ((item >= 150) && (item <= 300))
                    {
                        FrameSynchr local4 = Singleton<FrameSynchr>.instance;
                        local4.m_ping150to300 += 1L;
                    }
                    else if (item < 150)
                    {
                        FrameSynchr local5 = Singleton<FrameSynchr>.instance;
                        local5.m_ping150 += 1L;
                    }
                }
                Singleton<FrameSynchr>.instance.GameSvrPing = (((int) item) + Singleton<FrameSynchr>.instance.GameSvrPing) / 2;
                SetNetAccIPAndPort();
                if ((NetworkAccelerator.enabled && !NetworkAccelerator.started) && ((Time.get_unscaledTime() - Singleton<FrameSynchr>.instance.m_NetAccPingTimeBegin) > 1f))
                {
                    Singleton<FrameSynchr>.instance.m_NetAccPingTimeBegin = Time.get_unscaledTime();
                    NetworkAccelerator.OnNetDelay((int) item);
                }
                if ((Time.get_unscaledTime() - Singleton<FrameSynchr>.instance.m_PingTimeBegin) > 5f)
                {
                    Singleton<FrameSynchr>.instance.m_PingTimeBegin = Time.get_unscaledTime();
                    if (Singleton<FrameSynchr>.instance.m_LastPing == 0)
                    {
                        Singleton<FrameSynchr>.instance.m_LastPing = item;
                    }
                    if (Math.Abs((long) (Singleton<FrameSynchr>.instance.m_LastPing - item)) > 100L)
                    {
                        FrameSynchr local6 = Singleton<FrameSynchr>.instance;
                        local6.m_Abnormal_PingCount++;
                        Singleton<FrameSynchr>.instance.m_LastPing = item;
                    }
                }
                if ((Singleton<FrameSynchr>.instance.m_pingRecords.Count > 100) && (Singleton<FrameSynchr>.instance.GameSvrPing > 300))
                {
                    RealPing();
                }
            }
            msg.Release();
        }

        [MessageHandler(0x14d5)]
        public static void OnServerPingClientMsg(CSPkg msg)
        {
            if (Singleton<BattleLogic>.instance.isRuning)
            {
                CSPkg pkg = NetworkModule.CreateDefaultCSPKG(0x14d6);
                Singleton<NetworkModule>.GetInstance().SendGameMsg(ref pkg, 0);
            }
        }

        public void PingVariance()
        {
            this.m_PingAverage = 0;
            this.m_PingVariance = 0;
            if ((this.m_pingRecords != null) && (this.m_pingRecords.Count > 0))
            {
                double num = 0.0;
                double num2 = 0.0;
                for (int i = 0; i < this.m_pingRecords.Count; i++)
                {
                    num += (double) this.m_pingRecords[i];
                }
                num2 = num / ((double) this.m_pingRecords.Count);
                this.m_PingAverage = (int) num2;
                num2 = 0.0;
                num = 0.0;
                for (int j = 0; j < this.m_pingRecords.Count; j++)
                {
                    num += Math.Pow(((double) this.m_pingRecords[j]) - num2, 2.0);
                }
                num2 = num / ((double) this.m_pingRecords.Count);
                this.m_PingVariance = (int) num2;
            }
        }

        public void PushFrameCommand(IFrameCommand command)
        {
            command.cmdId = this.NewCommandId;
            if (this.bActive)
            {
                command.OnReceive();
            }
            else
            {
                command.frameNum = this.CurFrameNum;
            }
            this.commandQueue.Enqueue(command);
        }

        private static void RealPing()
        {
            if (((Singleton<FrameSynchr>.instance.m_realpingStartTime <= 0) && (Singleton<FrameSynchr>.instance.m_ping == null)) && !string.IsNullOrEmpty(ApolloConfig.loginOnlyIp))
            {
                Singleton<FrameSynchr>.instance.m_realpingStartTime = (uint) (Time.get_realtimeSinceStartup() * 1000f);
                Singleton<FrameSynchr>.instance.m_ping = new Ping(ApolloConfig.loginOnlyIp);
            }
        }

        public void ReportPingToBeacon()
        {
            List<KeyValuePair<string, string>> events = new List<KeyValuePair<string, string>>();
            events.Add(new KeyValuePair<string, string>("WorldID", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString()));
            events.Add(new KeyValuePair<string, string>("platform", Singleton<ApolloHelper>.GetInstance().CurPlatform.ToString()));
            if (NetworkAccelerator.started)
            {
                if (NetworkAccelerator.isAccerating())
                {
                    events.Add(new KeyValuePair<string, string>("AccState", "Acc"));
                }
                else
                {
                    events.Add(new KeyValuePair<string, string>("AccState", "Direct"));
                }
            }
            else
            {
                events.Add(new KeyValuePair<string, string>("AccState", "Off"));
            }
            this.m_PingAverage = 0;
            this.m_PingVariance = 0;
            if ((this.m_pingRecords != null) && (this.m_pingRecords.Count > 100))
            {
                double num = 0.0;
                double num2 = 0.0;
                for (int i = 0; i < this.m_pingRecords.Count; i++)
                {
                    num += (double) this.m_pingRecords[i];
                }
                num2 = num / ((double) this.m_pingRecords.Count);
                int num4 = Mathf.FloorToInt(((float) num2) / 100f) * 100;
                this.m_PingAverage = num4;
                Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("PingAverage_" + num4.ToString(), events, true);
                num = 0.0;
                for (int j = 0; j < this.m_pingRecords.Count; j++)
                {
                    num += Math.Pow(((double) this.m_pingRecords[j]) - num2, 2.0);
                }
                num2 = num / ((double) this.m_pingRecords.Count);
                num4 = Mathf.FloorToInt(((float) num2) / 1000f) * 0x3e8;
                this.m_PingVariance = num4;
                if (num4 <= 0x2710)
                {
                    Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("PingVariance_" + num4.ToString(), events, true);
                }
                else
                {
                    Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("PingVariance_>10000", events, true);
                }
            }
            int num6 = 0;
            if ((this.m_pingWobble != null) && (this.m_pingWobble.Count > 100))
            {
                int num7 = 0;
                for (int k = 0; k < this.m_pingWobble.Count; k++)
                {
                    if (Mathf.Abs((float) ((double) this.m_pingWobble[k])) <= 100f)
                    {
                        num7++;
                    }
                }
                num6 = Mathf.FloorToInt(((((float) num7) / ((float) this.m_pingWobble.Count)) * 100f) / 10f) * 10;
                Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("PingWobble<100_" + num6.ToString() + "%", events, true);
            }
            if (NetworkAccelerator.started && NetworkAccelerator.isAccerating())
            {
                PlayerPrefs.SetInt("ACC_PING_AVERAGE", this.m_PingAverage);
                PlayerPrefs.SetInt("ACC_PING_VARIANCE", this.m_PingVariance);
                PlayerPrefs.SetInt("ACC_PING_WOBBLE", num6);
            }
            else
            {
                PlayerPrefs.SetInt("NOACC_PING_AVERAGE", this.m_PingAverage);
                PlayerPrefs.SetInt("NOACC_PING_VARIANCE", this.m_PingVariance);
                PlayerPrefs.SetInt("NOACC_PING_WOBBLE", num6);
            }
            if (PlayerPrefs.HasKey("ACC_PING_AVERAGE") && PlayerPrefs.HasKey("NOACC_PING_AVERAGE"))
            {
                List<KeyValuePair<string, string>> list2 = new List<KeyValuePair<string, string>>();
                list2.Add(new KeyValuePair<string, string>("WorldID", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString()));
                list2.Add(new KeyValuePair<string, string>("platform", Singleton<ApolloHelper>.GetInstance().CurPlatform.ToString()));
                float num9 = ((float) PlayerPrefs.GetInt("NOACC_PING_AVERAGE")) / ((float) PlayerPrefs.GetInt("ACC_PING_AVERAGE"));
                list2.Add(new KeyValuePair<string, string>("pingAccRate", num9.ToString()));
                int num10 = PlayerPrefs.GetInt("NOACC_PING_AVERAGE") - PlayerPrefs.GetInt("ACC_PING_AVERAGE");
                list2.Add(new KeyValuePair<string, string>("pingAccDiff", num10.ToString()));
                float num11 = ((float) PlayerPrefs.GetInt("NOACC_PING_VARIANCE")) / ((float) PlayerPrefs.GetInt("ACC_PING_VARIANCE"));
                list2.Add(new KeyValuePair<string, string>("pingVarienceRate", num11.ToString()));
                float num12 = ((float) PlayerPrefs.GetInt("ACC_PING_WOBBLE")) / ((float) PlayerPrefs.GetInt("NOACC_PING_WOBBLE"));
                list2.Add(new KeyValuePair<string, string>("pingWobbleRate", num12.ToString()));
                list2.Add(new KeyValuePair<string, string>("g_version ", "Service" + CVersion.GetResourceVersion()));
                Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("NetAccStatistics", list2, true);
            }
        }

        private void ReqKeyFrameLaterModify(uint nLater)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x448);
            msg.stPkgData.stKFrapsLaterChgReq.bKFrapsLater = (byte) nLater;
            Singleton<NetworkModule>.instance.SendGameMsg(ref msg, 0);
        }

        public void ResetStartTime()
        {
            if (this.bActive)
            {
                this.startFrameTime = ((Time.get_realtimeSinceStartup() * this.frameSpeed) - (this.LogicFrameTick * 0.001f)) / ((float) this.frameSpeed);
            }
            else
            {
                this.startFrameTime = (Time.get_time() - (this.LogicFrameTick * 0.001f)) + Time.get_deltaTime();
            }
        }

        public void ResetSynchr()
        {
            this.bActive = false;
            this.bRunning = true;
            this.FrameDelta = 0x21;
            this.CurFrameNum = 0;
            this.EndFrameNum = 0;
            this.LogicFrameTick = 0L;
            this.EndBlockWaitNum = 0;
            this.PreActFrames = 5;
            this.SvrFrameDelta = this.FrameDelta;
            this.SvrFrameLater = 0;
            this.SvrFrameIndex = 0;
            this.CacheSetLater = 0;
            this.KeyFrameRate = 1;
            this.frameSpeed = 1;
            this.GameSvrPing = 0;
            this._CurPkgDelay = 0;
            this.AvgFrameDelay = 0;
            this.JitterDelay = 0;
            this.JitterDamper = 0;
            this.StatDelayIdx = 0;
            Array.Clear(this.StatDelaySet, 0, this.StatDelaySet.Length);
            this.NewCommandId = 0;
            this.startFrameTime = Time.get_time();
            this.backstepFrameCounter = 0;
            this.commandQueue.Clear();
            this.m_MaxPing = 0;
            this.m_MinPing = uint.MaxValue;
            this.m_AvePing = 0;
            this.m_PingIdx = 0;
            this.m_pingRecords.Clear();
            this.m_pingWobble.Clear();
            this.ClearMoveCMDTime();
            this.m_realpingStartTime = 0;
            this.m_ping = null;
            this.m_NetAccSetIPAndPortInvoked = false;
            this.m_PingTimeBegin = 0f;
            this.m_NetAccPingTimeBegin = 0f;
            this.m_LastPing = 0;
            this.m_Abnormal_PingCount = 0;
            this.m_ping300Count = 0L;
            this.m_ping150to300 = 0L;
            this.m_pingLost = 0L;
            this.m_SendHeartSeq = 0L;
            this.m_LastReceiveHeartSeq = 0L;
            this.m_ping150 = 0L;
            this.PauseCancelCount = 0;
        }

        public void ResetSynchrSeed()
        {
            FrameRandom.ResetSeed(this.ServerSeed);
        }

        public bool SetKeyFrameIndex(uint svrNum)
        {
            this.SvrFrameIndex = svrNum;
            this.EndFrameNum = (svrNum + this.SvrFrameLater) * this.KeyFrameRate;
            this.CalcBackstepTimeSinceStart(svrNum);
            return true;
        }

        private static void SetNetAccIPAndPort()
        {
            if ((!Singleton<FrameSynchr>.instance.m_NetAccSetIPAndPortInvoked && (ApolloConfig.echoPort != 0)) && !string.IsNullOrEmpty(ApolloConfig.loginOnlyIp))
            {
                NetworkAccelerator.setRecommendationGameIP(ApolloConfig.loginOnlyIp, ApolloConfig.echoPort);
                Singleton<FrameSynchr>.instance.m_NetAccSetIPAndPortInvoked = true;
            }
        }

        public void SetSynchrConfig(uint svrDelta, uint frameLater, uint preActNum, uint randSeed)
        {
            this.SvrFrameDelta = svrDelta;
            this.SvrFrameLater = 0;
            this.CacheSetLater = this.SvrFrameLater;
            this.KeyFrameRate = 1;
            this.PreActFrames = preActNum;
            this.ServerSeed = randSeed;
        }

        public void SetSynchrRunning(bool bRun)
        {
            this.bRunning = bRun;
        }

        public void StartSynchr()
        {
            this.bActive = true;
            this.bRunning = false;
            this.SvrFrameIndex = 0;
            this.FrameDelta = this.SvrFrameDelta / this.KeyFrameRate;
            this.CurFrameNum = 0;
            this.EndFrameNum = 0;
            this.LogicFrameTick = 0L;
            this.EndBlockWaitNum = 0;
            this.frameSpeed = 1;
            this.GameSvrPing = 0;
            this._CurPkgDelay = 0;
            this.AvgFrameDelay = 0;
            this.JitterDelay = 0;
            this.JitterDamper = 0;
            this.StatDelayIdx = 0;
            Array.Clear(this.StatDelaySet, 0, this.StatDelaySet.Length);
            this.commandQueue.Clear();
            this.NewCommandId = 0;
            this.startFrameTime = Time.get_realtimeSinceStartup();
            this.backstepFrameCounter = 0;
        }

        public void SwitchSynchrLocal()
        {
            if (this.bActive)
            {
                this.bActive = false;
                this.ResetStartTime();
            }
        }

        public void UpdateFrame()
        {
            if (this.bActive)
            {
                if (this.bRunning)
                {
                    this.UpdateMultiFrame();
                    UpdatePing();
                }
            }
            else if (this.bRunning)
            {
                this.UpdateSingleFrame();
            }
        }

        private void UpdateFrameLater()
        {
            if (this.CacheSetLater != this.SvrFrameLater)
            {
                this.SvrFrameLater = 0;
                this.EndFrameNum = (this.SvrFrameIndex + this.SvrFrameLater) * this.KeyFrameRate;
            }
        }

        private void UpdateMultiFrame()
        {
            Singleton<GameLogic>.GetInstance().UpdateTails();
            int num = (int) ((this.EndFrameNum - this.CurFrameNum) / this.nDriftFactor);
            this.tryCount = Mathf.Clamp(num, 1, 100);
            int tryCount = this.tryCount;
            long num4 = (long) ((Time.get_realtimeSinceStartup() - this.startFrameTime) * 1000.0);
            long nDelayMs = num4 - ((this.SvrFrameIndex + 1) * this.SvrFrameDelta);
            this.nJitterDelay = this.CalculateJitterDelay(nDelayMs);
            num4 *= this.frameSpeed;
            while (tryCount > 0)
            {
                long num6 = this.CurFrameNum * this.FrameDelta;
                this.nMultiFrameDelta = num4 - num6;
                this.nMultiFrameDelta -= this.nJitterDelay;
                if (this.nMultiFrameDelta >= this.FrameDelta)
                {
                    if (this.CurFrameNum >= this.EndFrameNum)
                    {
                        this.EndBlockWaitNum++;
                        tryCount = 0;
                        continue;
                    }
                    this.EndBlockWaitNum = 0;
                    this.CurFrameNum++;
                    this.LogicFrameTick += this.FrameDelta;
                    this.isCmdExecuting = true;
                    while (this.commandQueue.Count > 0)
                    {
                        IFrameCommand command = this.commandQueue.Peek();
                        uint num7 = (command.frameNum + this.SvrFrameLater) * this.KeyFrameRate;
                        if (num7 > this.CurFrameNum)
                        {
                            break;
                        }
                        command.frameNum = num7;
                        this.commandQueue.Dequeue().ExecCommand();
                    }
                    this.isCmdExecuting = false;
                    Singleton<GameLogic>.GetInstance().UpdateLogic((int) this.FrameDelta, (tryCount == 1) && (this.nMultiFrameDelta < (2L * this.FrameDelta)));
                    tryCount--;
                    continue;
                }
                tryCount = 0;
            }
        }

        private static void UpdatePing()
        {
            if ((Singleton<FrameSynchr>.instance.m_ping != null) && Singleton<FrameSynchr>.instance.m_ping.get_isDone())
            {
                uint num = ((uint) (Time.get_realtimeSinceStartup() * 1000f)) - Singleton<FrameSynchr>.instance.m_realpingStartTime;
                Singleton<FrameSynchr>.instance.m_ping = null;
            }
        }

        private void UpdateSingleFrame()
        {
            Singleton<GameLogic>.GetInstance().UpdateTails();
            long num = (long) (Time.get_deltaTime() * 1000f);
            num = Mathf.Clamp((int) num, 0, 100);
            this.CurFrameNum++;
            this.LogicFrameTick += (ulong) num;
            this.isCmdExecuting = true;
            while (this.commandQueue.Count > 0)
            {
                this.commandQueue.Dequeue().ExecCommand();
            }
            this.isCmdExecuting = false;
            if (num > 0L)
            {
                Singleton<GameLogic>.GetInstance().UpdateLogic((int) num, false);
            }
        }

        public bool bActive
        {
            get
            {
                return this._bActive;
            }
            set
            {
                if (this._bActive != value)
                {
                    Singleton<GameLogic>.instance.UpdateTails();
                    this._bActive = value;
                }
            }
        }

        public uint CurFrameNum
        {
            [CompilerGenerated]
            get
            {
                return this.<CurFrameNum>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<CurFrameNum>k__BackingField = value;
            }
        }

        public uint EndFrameNum
        {
            [CompilerGenerated]
            get
            {
                return this.<EndFrameNum>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<EndFrameNum>k__BackingField = value;
            }
        }

        public byte FrameSpeed
        {
            get
            {
                return this.frameSpeed;
            }
            set
            {
                this.frameSpeed = (byte) Mathf.Clamp(value, 1, 8);
                if (this._bActive)
                {
                    this.ResetStartTime();
                }
            }
        }

        public bool isCmdExecuting
        {
            [CompilerGenerated]
            get
            {
                return this.<isCmdExecuting>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<isCmdExecuting>k__BackingField = value;
            }
        }

        public bool isRunning
        {
            get
            {
                return this.bRunning;
            }
        }

        public ulong LogicFrameTick
        {
            [CompilerGenerated]
            get
            {
                return this.<LogicFrameTick>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<LogicFrameTick>k__BackingField = value;
            }
        }

        public uint NewCommandId
        {
            get
            {
                this.uCommandId++;
                return this.uCommandId;
            }
            set
            {
                this.uCommandId = value;
            }
        }

        public int nJitterDelay
        {
            [CompilerGenerated]
            get
            {
                return this.<nJitterDelay>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<nJitterDelay>k__BackingField = value;
            }
        }

        public long nMultiFrameDelta
        {
            [CompilerGenerated]
            get
            {
                return this.<nMultiFrameDelta>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<nMultiFrameDelta>k__BackingField = value;
            }
        }

        public int RealSvrPing
        {
            get
            {
                return (this.GameSvrPing + this.AvgFrameDelay);
            }
        }

        public float startFrameTime
        {
            [CompilerGenerated]
            get
            {
                return this.<startFrameTime>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<startFrameTime>k__BackingField = value;
            }
        }

        public uint svrLogicFrameNum
        {
            get
            {
                return this.SvrFrameIndex;
            }
        }
    }
}

