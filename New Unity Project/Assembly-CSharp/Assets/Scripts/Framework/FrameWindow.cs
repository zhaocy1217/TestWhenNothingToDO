namespace Assets.Scripts.Framework
{
    using CSProtocol;
    using System;
    using tsf4g_tdr_csharp;
    using UnityEngine;

    [MessageHandlerClass]
    public class FrameWindow : Singleton<FrameWindow>, IGameModule
    {
        private uint _aliveFlags;
        private int _aliveFrameCount;
        private uint _basFrqNo;
        private uint _begFrqNo;
        private static byte _frameExceptionCounter;
        private uint _maxFrqNo;
        private CSDT_FRAPBOOT_INFO[] _receiveWindow;
        private uint _repairBegNo;
        private int _repairCounter;
        private int _repairTimes;
        private uint _sendCmdSeq;
        private bool _showChart;
        private int _timeoutCounter;
        private int _timeoutFrameOffset;
        private int _timeoutFrameStep;
        private int _timeoutTimes;
        private const int ALIVE_THRESHOLD_FRAME_OF32 = 10;
        public const uint FRQ_WIN_LEN = 900;
        public const int MAX_TIMEOUT_TIMES = 5;

        public FrameWindow()
        {
            this.Reset();
        }

        private CSDT_FRAPBOOT_INFO _FetchFBI(uint frqNo)
        {
            uint index = this._FrqNoToWinIdx_(frqNo);
            CSDT_FRAPBOOT_INFO csdt_frapboot_info = this._receiveWindow[index];
            this._receiveWindow[index] = null;
            return csdt_frapboot_info;
        }

        private uint _FrqNoToWinIdx_(uint theFrqNo)
        {
            return ((theFrqNo - this._basFrqNo) % 900);
        }

        private static void HandleAIChgSyncCommand(uint dwFrqNo, CSDT_FRAPBOOT_AISTATE AIState)
        {
            IFrameCommand command = null;
            FrameCommand<AutoAIChgCommand> command2 = FrameCommandFactory.CreateSCSyncFrameCommand<AutoAIChgCommand>();
            command2.cmdData.m_autoType = AIState.bType;
            command2.cmdData.m_playerID = AIState.dwPlayerObjID;
            command = command2;
            if (command != null)
            {
                command.playerID = AIState.dwPlayerObjID;
                command.frameNum = dwFrqNo;
                Singleton<FrameSynchr>.GetInstance().PushFrameCommand(command);
            }
            else
            {
                _frameExceptionCounter = (byte) (_frameExceptionCounter + 1);
                if (_frameExceptionCounter <= 30)
                {
                    BuglyAgent.ReportException(new Exception("CreateFrameCommandException"), "create aiChange frame command error!");
                }
            }
        }

        private static void HandleAssistChgSyncCommand(uint dwFrqNo, CSDT_FRAPBOOT_ASSISTSTATE assistChgState)
        {
            IFrameCommand command = null;
            FrameCommand<AssistStateChgCommand> command2 = FrameCommandFactory.CreateSCSyncFrameCommand<AssistStateChgCommand>();
            command2.cmdData.m_chgType = assistChgState.bType;
            command2.cmdData.m_aiPlayerID = assistChgState.dwAiPlayerObjID;
            command2.cmdData.m_masterPlayerID = assistChgState.dwMasterObjID;
            command = command2;
            if (command != null)
            {
                command.playerID = assistChgState.dwAiPlayerObjID;
                command.frameNum = dwFrqNo;
                Singleton<FrameSynchr>.GetInstance().PushFrameCommand(command);
            }
            else
            {
                _frameExceptionCounter = (byte) (_frameExceptionCounter + 1);
                if (_frameExceptionCounter <= 30)
                {
                    BuglyAgent.ReportException(new Exception("CreateFrameCommandException"), "create assistChange frame command error!");
                }
            }
        }

        private static void HandleClientClientSyncCommand(uint dwFrqNo, CSDT_FRAPBOOT_CC ccSynDt)
        {
            int usedSize = 0;
            FRAME_CMD_PKG msg = FRAME_CMD_PKG.New();
            TdrError.ErrorType type = msg.unpack(ref ccSynDt.stSyncInfo.szBuff, ccSynDt.stSyncInfo.wLen, ref usedSize, 0);
            DebugHelper.Assert(type == TdrError.ErrorType.TDR_NO_ERROR);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                IFrameCommand command = FrameCommandFactory.CreateFrameCommand(ref msg);
                if (command != null)
                {
                    command.playerID = ccSynDt.dwObjID;
                    command.frameNum = dwFrqNo;
                    Singleton<FrameSynchr>.GetInstance().PushFrameCommand(command);
                }
                else
                {
                    _frameExceptionCounter = (byte) (_frameExceptionCounter + 1);
                    if (_frameExceptionCounter <= 30)
                    {
                        BuglyAgent.ReportException(new Exception("CreateFrameCommandException"), "create ccSync frame command error!");
                    }
                }
            }
            else
            {
                _frameExceptionCounter = (byte) (_frameExceptionCounter + 1);
                if (_frameExceptionCounter <= 30)
                {
                    BuglyAgent.ReportException(new Exception("TdrUnpackException"), "CCSync unpack error!");
                }
            }
            msg.Release();
        }

        private static void HandleClientServerSyncCommand(uint dwFrqNo, CSDT_FRAPBOOT_CS csSynDt)
        {
            IFrameCommand command = FrameCommandFactory.CreateFrameCommandByCSSyncInfo(ref csSynDt.stSyncInfo);
            if (command != null)
            {
                command.playerID = csSynDt.dwObjID;
                command.frameNum = dwFrqNo;
                Singleton<FrameSynchr>.GetInstance().PushFrameCommand(command);
            }
            else
            {
                _frameExceptionCounter = (byte) (_frameExceptionCounter + 1);
                if (_frameExceptionCounter <= 30)
                {
                    BuglyAgent.ReportException(new Exception("CreateFrameCommandException"), "create csSync frame command error!");
                }
            }
        }

        private static void HandleClientStateSyncCommand(uint dwFrqNo, CSDT_FRAPBOOT_ACNTSTATE stateSyncDt)
        {
            IFrameCommand command = null;
            switch (stateSyncDt.bStateChgType)
            {
                case 1:
                    command = FrameCommandFactory.CreateSCSyncFrameCommand<SvrDisconnectCommand>();
                    break;

                case 2:
                    command = FrameCommandFactory.CreateSCSyncFrameCommand<SvrReconnectCommand>();
                    break;

                case 3:
                    command = FrameCommandFactory.CreateSCSyncFrameCommand<SvrRunawayCommand>();
                    break;
            }
            if (command != null)
            {
                command.playerID = stateSyncDt.dwObjID;
                command.frameNum = dwFrqNo;
                Singleton<FrameSynchr>.GetInstance().PushFrameCommand(command);
            }
            else
            {
                _frameExceptionCounter = (byte) (_frameExceptionCounter + 1);
                if (_frameExceptionCounter <= 30)
                {
                    BuglyAgent.ReportException(new Exception("CreateFrameCommandException"), "create stateChange frame command error!");
                }
            }
        }

        public static void HandleFraqBootInfo(CSDT_FRAPBOOT_INFO fbid)
        {
            if (Singleton<FrameSynchr>.GetInstance().SetKeyFrameIndex(fbid.dwKFrapsNo))
            {
                Singleton<GameReplayModule>.instance.SetKFraqNo(fbid.dwKFrapsNo);
                if (fbid.bNum > 0)
                {
                    Singleton<GameReplayModule>.instance.CacheRecord(fbid);
                }
                for (int i = 0; i < fbid.bNum; i++)
                {
                    CSDT_FRAPBOOT_DETAIL csdt_frapboot_detail = fbid.astBootInfo[i];
                    switch (csdt_frapboot_detail.bType)
                    {
                        case 1:
                            HandleClientClientSyncCommand(fbid.dwKFrapsNo, csdt_frapboot_detail.stDetail.stCCBoot);
                            break;

                        case 2:
                            HandleClientServerSyncCommand(fbid.dwKFrapsNo, csdt_frapboot_detail.stDetail.stCSBoot);
                            break;

                        case 3:
                            HandleClientStateSyncCommand(fbid.dwKFrapsNo, csdt_frapboot_detail.stDetail.stAcntState);
                            break;

                        case 4:
                            HandleAssistChgSyncCommand(fbid.dwKFrapsNo, csdt_frapboot_detail.stDetail.stAssistState);
                            break;

                        case 5:
                            HandleAIChgSyncCommand(fbid.dwKFrapsNo, csdt_frapboot_detail.stDetail.stAiState);
                            break;

                        case 6:
                            HandleGameOverCommand(fbid.dwKFrapsNo, csdt_frapboot_detail.stDetail.stGameOverNtf);
                            break;

                        case 7:
                            HandleGamePauseCommand(fbid.dwKFrapsNo, csdt_frapboot_detail.stDetail.stPause);
                            break;
                    }
                }
            }
            fbid.Release();
        }

        private static bool HandleFraqBootSingle(SCPKG_FRAPBOOT_SINGLE fbi)
        {
            CSDT_FRAPBOOT_INFO fbid = CSDT_FRAPBOOT_INFO.New();
            int usedSize = 0;
            return (((fbid.unpack(ref fbi.szInfoBuff, fbi.wLen, ref usedSize, 0) == TdrError.ErrorType.TDR_NO_ERROR) && (usedSize > 0)) && Singleton<FrameWindow>.GetInstance().SetFrqWin(fbid));
        }

        private static void HandleGameOverCommand(uint dwFrqNo, CSDT_FRAPBOOT_GAMEOVERNTF OverNtf)
        {
            IFrameCommand command = null;
            FrameCommand<SvrNtfGameOverCommand> command2 = FrameCommandFactory.CreateSCSyncFrameCommand<SvrNtfGameOverCommand>();
            command2.cmdData.m_bWinCamp = OverNtf.bWinCamp;
            command = command2;
            if (command != null)
            {
                command.frameNum = dwFrqNo;
                Singleton<FrameSynchr>.GetInstance().PushFrameCommand(command);
            }
        }

        private static void HandleGamePauseCommand(uint dwFrqNo, CSDT_FRAPBOOT_PAUSE pauseNtf)
        {
            FrameCommand<PauseResumeGameCommand> command = FrameCommandFactory.CreateSCSyncFrameCommand<PauseResumeGameCommand>();
            command.cmdData.PauseCommand = pauseNtf.bType;
            command.cmdData.PauseByCamp = pauseNtf.bCamp;
            command.cmdData.CampPauseTimes = new byte[pauseNtf.szCampPauseNum.Length];
            Buffer.BlockCopy(pauseNtf.szCampPauseNum, 0, command.cmdData.CampPauseTimes, 0, pauseNtf.szCampPauseNum.Length);
            IFrameCommand command2 = command;
            if (command2 != null)
            {
                command2.frameNum = dwFrqNo;
                Singleton<FrameSynchr>.GetInstance().PushFrameCommand(command2);
            }
        }

        [MessageHandler(0x40b)]
        public static void onFrapBootInfoMultipleNtf(CSPkg msg)
        {
            SCPKG_FRAPBOOTINFO stFrapBootInfo = msg.stPkgData.stFrapBootInfo;
            for (int i = 0; i < stFrapBootInfo.bSpareNum; i++)
            {
                if (HandleFraqBootSingle(stFrapBootInfo.astSpareFrap[i]))
                {
                    MonoSingleton<Reconnection>.GetInstance().UpdateCachedLen(msg);
                    break;
                }
            }
            Singleton<FrameWindow>.GetInstance().SampleFrameSpare(stFrapBootInfo.bSpareNum);
            msg.Release();
        }

        [MessageHandler(0x40a)]
        public static void onFrapBootInfoSingleNtf(CSPkg msg)
        {
            if (HandleFraqBootSingle(msg.stPkgData.stFrapBootSingle))
            {
                MonoSingleton<Reconnection>.GetInstance().UpdateCachedLen(msg);
            }
            msg.Release();
        }

        private void RefreshTimeout()
        {
            this._aliveFrameCount = Time.get_frameCount();
            this._timeoutCounter = 0;
            this._aliveFlags |= ((uint) 1) << (Time.get_frameCount() % 0x20);
            if (this._timeoutTimes > 0)
            {
                int num = 0;
                for (int i = 0; i < 0x20; i++)
                {
                    if ((this._aliveFlags & (((int) 1) << i)) > 0)
                    {
                        num++;
                    }
                }
                if (num > 10)
                {
                    this._timeoutTimes = 0;
                }
            }
        }

        private void RequestRepairFraqBootInfo()
        {
            if (this._maxFrqNo > this._begFrqNo)
            {
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x40c);
                CSPKG_REQUESTFRAPBOOTSINGLE stReqFrapBootSingle = msg.stPkgData.stReqFrapBootSingle;
                stReqFrapBootSingle.bNum = 0;
                for (uint i = 0; i < 900; i++)
                {
                    if ((stReqFrapBootSingle.bNum >= stReqFrapBootSingle.KFrapsNo.Length) || (stReqFrapBootSingle.bNum > ((this._maxFrqNo - this._begFrqNo) + 2)))
                    {
                        break;
                    }
                    uint theFrqNo = this._begFrqNo + i;
                    if (this._receiveWindow[this._FrqNoToWinIdx_(theFrqNo)] == null)
                    {
                        byte num3;
                        stReqFrapBootSingle.bNum = (byte) ((num3 = stReqFrapBootSingle.bNum) + 1);
                        stReqFrapBootSingle.KFrapsNo[num3] = theFrqNo;
                    }
                }
                Singleton<NetworkModule>.GetInstance().SendGameMsg(ref msg, 0);
            }
        }

        private void RequestTimeoutFraqBootInfo()
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x40d);
            msg.stPkgData.stReqFrapBootTimeout.dwCurKFrapsNo = this._begFrqNo;
            Singleton<NetworkModule>.GetInstance().SendGameMsg(ref msg, 0);
        }

        public void Reset()
        {
            this._receiveWindow = new CSDT_FRAPBOOT_INFO[900];
            this._basFrqNo = 0;
            this._begFrqNo = 1;
            this._maxFrqNo = 0;
            this._repairCounter = 0;
            this._repairBegNo = 0;
            this._repairTimes = 0;
            this._timeoutCounter = 0;
            this._timeoutTimes = 0;
            this._aliveFlags = 0;
            this._aliveFrameCount = 0;
            this._timeoutFrameStep = 3;
            this._timeoutFrameOffset = 1;
            _frameExceptionCounter = 0;
        }

        public void ResetSendCmdSeq()
        {
            this._sendCmdSeq = 0;
        }

        private void SampleFrameSpare(int frameSpare)
        {
            if (this._showChart)
            {
                MonoSingleton<RealTimeChart>.instance.AddSample("FramePackageSpare", (float) frameSpare);
            }
        }

        public bool SendGameCmd(IFrameCommand cmd, bool bMultiGame)
        {
            if (Singleton<WatchController>.GetInstance().IsWatching)
            {
                return false;
            }
            if (Singleton<NetworkModule>.GetInstance().isOnlineMode)
            {
                if (bMultiGame)
                {
                    if (Singleton<NetworkModule>.GetInstance().gameSvr.connected)
                    {
                        cmd.cmdId = this.NewSendCmdSeq;
                        cmd.frameNum = (uint) Time.get_frameCount();
                        cmd.sendCnt = 0;
                        Singleton<NetworkModule>.GetInstance().gameSvr.ImmeSendCmd(ref cmd);
                    }
                }
                else
                {
                    Singleton<FrameSynchr>.GetInstance().PushFrameCommand(cmd);
                }
            }
            return true;
        }

        private bool SetFrqWin(CSDT_FRAPBOOT_INFO fbid)
        {
            if (this._aliveFrameCount != Time.get_frameCount())
            {
                this.RefreshTimeout();
            }
            bool flag = false;
            uint dwKFrapsNo = fbid.dwKFrapsNo;
            if (dwKFrapsNo > this._maxFrqNo)
            {
                this._maxFrqNo = dwKFrapsNo;
            }
            if ((this._begFrqNo <= dwKFrapsNo) && (dwKFrapsNo < (this._begFrqNo + 900)))
            {
                this._receiveWindow[this._FrqNoToWinIdx_(dwKFrapsNo)] = fbid;
                if (Singleton<FrameSynchr>.GetInstance().bActive)
                {
                    CSDT_FRAPBOOT_INFO csdt_frapboot_info = null;
                    while ((csdt_frapboot_info = this._FetchFBI(this._begFrqNo)) != null)
                    {
                        if ((++this._begFrqNo % 900) == 0)
                        {
                            this._basFrqNo = this._begFrqNo;
                        }
                        HandleFraqBootInfo(csdt_frapboot_info);
                        flag = true;
                    }
                }
                return flag;
            }
            if (dwKFrapsNo > this._begFrqNo)
            {
                MonoSingleton<Reconnection>.GetInstance().RequestRelaySyncCacheFrames(false);
            }
            return flag;
        }

        public void ToggleShowFrameSpareChart()
        {
            this._showChart = !this._showChart;
            if (this._showChart)
            {
                MonoSingleton<RealTimeChart>.instance.isVisible = true;
                MonoSingleton<RealTimeChart>.instance.AddTrack("FramePackageSpare", Color.get_yellow(), true, 0f, 5f).isVisiable = true;
            }
            else
            {
                MonoSingleton<RealTimeChart>.instance.isVisible = false;
                MonoSingleton<RealTimeChart>.instance.RemoveTrack("FramePackageSpare");
            }
        }

        public void UpdateFrame()
        {
            if (!Singleton<WatchController>.GetInstance().IsRelayCast)
            {
                this._aliveFlags &= (uint) ~(((int) 1) << (Time.get_frameCount() % 0x20));
                if (Singleton<FrameSynchr>.GetInstance().bActive && !MonoSingleton<Reconnection>.GetInstance().isProcessingRelayRecover)
                {
                    if ((this._maxFrqNo > this._begFrqNo) && (this._receiveWindow[this._FrqNoToWinIdx_(this._begFrqNo)] == null))
                    {
                        if (this._repairBegNo != this._begFrqNo)
                        {
                            this._repairBegNo = this._begFrqNo;
                            this.RequestRepairFraqBootInfo();
                            this._repairTimes = 0;
                            this._repairCounter = 0;
                        }
                        else if (++this._repairCounter > ((2 ^ this._repairTimes) * this._timeoutFrameStep))
                        {
                            this.RequestRepairFraqBootInfo();
                            this._repairCounter = 0;
                            this._repairTimes++;
                        }
                    }
                    if (((this._timeoutTimes < 5) && (this._begFrqNo > 1)) && (++this._timeoutCounter > (((2 ^ this._timeoutTimes) * this._timeoutFrameStep) + this._timeoutFrameOffset)))
                    {
                        this.RequestTimeoutFraqBootInfo();
                        this._timeoutCounter = 0;
                        this._timeoutTimes++;
                    }
                }
            }
        }

        private uint NewSendCmdSeq
        {
            get
            {
                return ++this._sendCmdSeq;
            }
        }
    }
}

