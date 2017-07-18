namespace Assets.Scripts.Framework
{
    using Apollo;
    using Assets.Scripts.UI;
    using CSProtocol;
    using System;
    using System.Collections.Generic;
    using tsf4g_tdr_csharp;
    using UnityEngine;

    public class GameConnector : BaseConnector
    {
        private NetworkState changedNetState;
        private IFrameCommand[] cmdRedundancyQueue = new IFrameCommand[3];
        private List<CSPkg> confirmSendQueue = new List<CSPkg>();
        public NetworkReachability curNetworkReachability;
        private List<CSPkg> gameMsgSendQueue = new List<CSPkg>();
        private int nBuffSize = 0x32000;
        public const int nCmdRedQueueCount = 3;
        private int nCmdRedQueueIndex;
        private bool netStateChanged;
        private ReconnectPolicy reconPolicy = new ReconnectPolicy();
        private byte[] szSendBuffer = new byte[0x32000];

        public void CleanUp()
        {
            this.gameMsgSendQueue.Clear();
            this.confirmSendQueue.Clear();
            this.reconPolicy.StopPolicy();
            this.ClearBuffer();
            this.nCmdRedQueueIndex = 0;
            Array.Clear(this.cmdRedundancyQueue, 0, this.cmdRedundancyQueue.Length);
        }

        private void ClearBuffer()
        {
            this.szSendBuffer.Initialize();
        }

        protected override void DealConnectClose(ApolloResult result)
        {
        }

        protected override void DealConnectError(ApolloResult result)
        {
            this.reconPolicy.StartPolicy(result, 6);
            MonoSingleton<Reconnection>.instance.QueryIsRelayGaming(result);
            List<KeyValuePair<string, string>> events = new List<KeyValuePair<string, string>>();
            events.Add(new KeyValuePair<string, string>("g_version", CVersion.GetAppVersion()));
            events.Add(new KeyValuePair<string, string>("WorldID", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString()));
            events.Add(new KeyValuePair<string, string>("platform", Singleton<ApolloHelper>.GetInstance().CurPlatform.ToString()));
            events.Add(new KeyValuePair<string, string>("openid", "NULL"));
            events.Add(new KeyValuePair<string, string>("status", "0"));
            events.Add(new KeyValuePair<string, string>("type", "challenge"));
            events.Add(new KeyValuePair<string, string>("errorCode", result.ToString()));
            Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Service_SvrConnectFail", events, true);
        }

        protected override void DealConnectFail(ApolloResult result, ApolloLoginInfo loginInfo)
        {
            this.reconPolicy.StartPolicy(result, 6);
            MonoSingleton<Reconnection>.instance.QueryIsRelayGaming(result);
            List<KeyValuePair<string, string>> events = new List<KeyValuePair<string, string>>();
            events.Add(new KeyValuePair<string, string>("g_version", CVersion.GetAppVersion()));
            events.Add(new KeyValuePair<string, string>("WorldID", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString()));
            events.Add(new KeyValuePair<string, string>("platform", Singleton<ApolloHelper>.GetInstance().CurPlatform.ToString()));
            events.Add(new KeyValuePair<string, string>("openid", "NULL"));
            events.Add(new KeyValuePair<string, string>("status", "0"));
            events.Add(new KeyValuePair<string, string>("type", "challenge"));
            events.Add(new KeyValuePair<string, string>("errorCode", result.ToString()));
            Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Service_SvrConnectFail", events, true);
        }

        protected override void DealConnectSucc()
        {
            this.reconPolicy.StopPolicy();
            Singleton<ReconnectIpSelect>.instance.SetRelaySuccessUrl(base.initParam.ip);
            MonoSingleton<Reconnection>.GetInstance().OnConnectSuccess();
            List<KeyValuePair<string, string>> events = new List<KeyValuePair<string, string>>();
            events.Add(new KeyValuePair<string, string>("g_version", CVersion.GetAppVersion()));
            events.Add(new KeyValuePair<string, string>("WorldID", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString()));
            events.Add(new KeyValuePair<string, string>("platform", Singleton<ApolloHelper>.GetInstance().CurPlatform.ToString()));
            events.Add(new KeyValuePair<string, string>("openid", "NULL"));
            events.Add(new KeyValuePair<string, string>("status", "0"));
            events.Add(new KeyValuePair<string, string>("type", "challenge"));
            events.Add(new KeyValuePair<string, string>("errorCode", "SUCC"));
            Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Service_SvrConnectFail", events, true);
        }

        public void Disconnect()
        {
            ApolloNetworkService.Intance.NetworkChangedEvent -= new Apollo.NetworkStateChanged(this.NetworkStateChanged);
            base.DestroyConnector();
            this.reconPolicy.StopPolicy();
            this.reconPolicy.SetConnector(null, null, 0);
            base.initParam = null;
        }

        ~GameConnector()
        {
            base.DestroyConnector();
            this.reconPolicy = null;
        }

        private void FlushSendCmd(IFrameCommand inCmd)
        {
            IFrameCommand cmd = null;
            IFrameCommand command2 = null;
            IFrameCommand command3 = null;
            if (inCmd != null)
            {
                cmd = inCmd;
                command2 = this.cmdRedundancyQueue[this.nCmdRedQueueIndex];
                command3 = this.cmdRedundancyQueue[((this.nCmdRedQueueIndex - 1) + 3) % 3];
            }
            else
            {
                cmd = this.cmdRedundancyQueue[this.nCmdRedQueueIndex];
                command2 = this.cmdRedundancyQueue[((this.nCmdRedQueueIndex - 1) + 3) % 3];
                command3 = this.cmdRedundancyQueue[((this.nCmdRedQueueIndex - 2) + 3) % 3];
            }
            if ((cmd != null) && ((inCmd != null) || (cmd.sendCnt < 3)))
            {
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x3ec);
                msg.stPkgData.stGamingUperMsg.bNum = 0;
                this.PackCmd2Msg(ref cmd, msg.stPkgData.stGamingUperMsg.astUperInfo[msg.stPkgData.stGamingUperMsg.bNum]);
                CSPKG_GAMING_UPERMSG stGamingUperMsg = msg.stPkgData.stGamingUperMsg;
                stGamingUperMsg.bNum = (byte) (stGamingUperMsg.bNum + 1);
                cmd.sendCnt = (byte) (cmd.sendCnt + 1);
                if (((command2 != null) && (command2.sendCnt < 3)) && ((command2.frameNum + 10) > Time.get_frameCount()))
                {
                    this.PackCmd2Msg(ref command2, msg.stPkgData.stGamingUperMsg.astUperInfo[msg.stPkgData.stGamingUperMsg.bNum]);
                    CSPKG_GAMING_UPERMSG cspkg_gaming_upermsg2 = msg.stPkgData.stGamingUperMsg;
                    cspkg_gaming_upermsg2.bNum = (byte) (cspkg_gaming_upermsg2.bNum + 1);
                    command2.sendCnt = (byte) (command2.sendCnt + 1);
                    if (((command3 != null) && (command3.sendCnt < 3)) && ((command3.frameNum + 10) > Time.get_frameCount()))
                    {
                        this.PackCmd2Msg(ref command3, msg.stPkgData.stGamingUperMsg.astUperInfo[msg.stPkgData.stGamingUperMsg.bNum]);
                        CSPKG_GAMING_UPERMSG cspkg_gaming_upermsg3 = msg.stPkgData.stGamingUperMsg;
                        cspkg_gaming_upermsg3.bNum = (byte) (cspkg_gaming_upermsg3.bNum + 1);
                        command3.sendCnt = (byte) (command3.sendCnt + 1);
                    }
                }
                this.SendPackage(msg);
                msg.Release();
            }
        }

        public void ForceReconnect()
        {
            this.reconPolicy.UpdatePolicy(true);
        }

        public void HandleMsg(CSPkg msg)
        {
            if ((msg.stPkgHead.dwMsgID == 0x433) || (msg.stPkgHead.dwMsgID == 0x435))
            {
                Singleton<GameReplayModule>.instance.CacheRecord(msg);
            }
            NetMsgDelegate msgHandler = Singleton<NetworkModule>.instance.GetMsgHandler(msg.stPkgHead.dwMsgID);
            if (msgHandler != null)
            {
                msgHandler(msg);
            }
        }

        public void HandleSending()
        {
            if (base.connected)
            {
                this.FlushSendCmd(null);
                for (int i = 0; i < this.confirmSendQueue.Count; i++)
                {
                    CSPkg msg = this.confirmSendQueue[i];
                    if ((Singleton<GameLogic>.instance.GameRunningTick - msg.stPkgHead.dwSvrPkgSeq) > 0x1388)
                    {
                        this.SendPackage(msg);
                        msg.stPkgHead.dwSvrPkgSeq = Singleton<GameLogic>.instance.GameRunningTick;
                    }
                }
                while (base.connected && (this.gameMsgSendQueue.Count > 0))
                {
                    CSPkg pkg2 = this.gameMsgSendQueue[0];
                    if (!this.SendPackage(pkg2))
                    {
                        break;
                    }
                    if (pkg2.stPkgHead.dwReserve > 0)
                    {
                        pkg2.stPkgHead.dwSvrPkgSeq = Singleton<GameLogic>.instance.GameRunningTick;
                        this.confirmSendQueue.Add(pkg2);
                    }
                    else
                    {
                        pkg2.Release();
                    }
                    this.gameMsgSendQueue.RemoveAt(0);
                }
            }
            else
            {
                MonoSingleton<Reconnection>.instance.UpdateReconnect();
            }
        }

        public void ImmeSendCmd(ref IFrameCommand cmd)
        {
            this.FlushSendCmd(cmd);
            if (((cmd.cmdType == 0x80) || (cmd.cmdType == 0x81)) || ((cmd.cmdType == 130) || (cmd.cmdType == 0x84)))
            {
                this.nCmdRedQueueIndex = ++this.nCmdRedQueueIndex % 3;
                this.cmdRedundancyQueue[this.nCmdRedQueueIndex] = cmd;
            }
        }

        public bool Init(ConnectorParam para)
        {
            this.reconPolicy.SetConnector(this, new tryReconnectDelegate(this.onTryReconnect), 8);
            ApolloNetworkService.Intance.NetworkChangedEvent -= new Apollo.NetworkStateChanged(this.NetworkStateChanged);
            ApolloNetworkService.Intance.NetworkChangedEvent += new Apollo.NetworkStateChanged(this.NetworkStateChanged);
            this.curNetworkReachability = Application.get_internetReachability();
            return base.CreateConnector(para);
        }

        private void NetworkStateChanged(NetworkState state)
        {
            this.changedNetState = state;
            this.netStateChanged = true;
        }

        private uint onTryReconnect(uint nCount, uint nMax)
        {
            string connectUrl = Singleton<ReconnectIpSelect>.instance.GetConnectUrl(ConnectorType.Relay, nCount);
            base.initParam.SetVip(connectUrl);
            if (nCount >= 2)
            {
                try
                {
                    MonoSingleton<Reconnection>.GetInstance().ShowReconnectMsgAlert(((int) nCount) - 1, ((int) nMax) - 1);
                }
                catch (Exception exception)
                {
                    object[] inParameters = new object[] { exception.Message, exception.StackTrace };
                    DebugHelper.Assert(false, "Exception In GameConnector Try Reconnect, {0} {1}", inParameters);
                }
            }
            if (nCount == 2)
            {
                List<KeyValuePair<string, string>> events = new List<KeyValuePair<string, string>>();
                events.Add(new KeyValuePair<string, string>("g_version", CVersion.GetAppVersion()));
                events.Add(new KeyValuePair<string, string>("WorldID", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString()));
                events.Add(new KeyValuePair<string, string>("platform", Singleton<ApolloHelper>.GetInstance().CurPlatform.ToString()));
                events.Add(new KeyValuePair<string, string>("ping", Singleton<FrameSynchr>.instance.GameSvrPing.ToString()));
                if (Application.get_internetReachability() == 1)
                {
                    events.Add(new KeyValuePair<string, string>("Network", "3G or 4G"));
                }
                else if (Application.get_internetReachability() == 2)
                {
                    events.Add(new KeyValuePair<string, string>("Network", "WIFI"));
                }
                else
                {
                    events.Add(new KeyValuePair<string, string>("Network", "NoSignal"));
                }
                events.Add(new KeyValuePair<string, string>("FrameNum", Singleton<FrameSynchr>.instance.CurFrameNum.ToString()));
                events.Add(new KeyValuePair<string, string>("IsFighting", Singleton<BattleLogic>.instance.isFighting.ToString()));
                Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("GameConnector.onTryReconnect", events, true);
            }
            NetworkModule instance = Singleton<NetworkModule>.GetInstance();
            instance.m_GameReconnetCount++;
            return nCount;
        }

        private void PackCmd2Msg(ref IFrameCommand cmd, CSDT_GAMING_UPER_INFO msg)
        {
            if (cmd.isCSSync)
            {
                msg.bType = 2;
                msg.dwCmdSeq = cmd.cmdId;
                msg.stUperDt.construct((long) msg.bType);
                msg.stUperDt.stCSInfo.stCSSyncDt.construct((long) cmd.cmdType);
                cmd.TransProtocol(msg.stUperDt.stCSInfo);
            }
            else
            {
                msg.bType = 1;
                msg.dwCmdSeq = cmd.cmdId;
                msg.stUperDt.construct((long) msg.bType);
                msg.stUperDt.stCCInfo.construct();
                FRAME_CMD_PKG frame_cmd_pkg = FrameCommandFactory.CreateCommandPKG(cmd);
                cmd.TransProtocol(frame_cmd_pkg);
                int usedSize = 0;
                TdrError.ErrorType type = frame_cmd_pkg.pack(ref msg.stUperDt.stCCInfo.szBuff, 0x40, ref usedSize, 0);
                msg.stUperDt.stCCInfo.wLen = (ushort) usedSize;
                DebugHelper.Assert(type == TdrError.ErrorType.TDR_NO_ERROR);
                frame_cmd_pkg.Release();
            }
        }

        public void PushSendMsg(CSPkg msg)
        {
            this.gameMsgSendQueue.Add(msg);
        }

        public CSPkg RecvPackage()
        {
            if (base.connected && (base.connector != null))
            {
                byte[] buffer;
                int num;
                if (base.connector.ReadUdpData(out buffer, out num) == ApolloResult.Success)
                {
                    int usedSize = 0;
                    CSPkg pkg = CSPkg.New();
                    if ((pkg.unpack(ref buffer, num, ref usedSize, 0) == TdrError.ErrorType.TDR_NO_ERROR) && (usedSize > 0))
                    {
                        return pkg;
                    }
                }
                if (base.connector.ReadData(out buffer, out num) == ApolloResult.Success)
                {
                    int num3 = 0;
                    CSPkg pkg2 = CSPkg.New();
                    if ((pkg2.unpack(ref buffer, num, ref num3, 0) == TdrError.ErrorType.TDR_NO_ERROR) && (num3 > 0))
                    {
                        int index = 0;
                        while (index < this.confirmSendQueue.Count)
                        {
                            CSPkg pkg3 = this.confirmSendQueue[index];
                            if ((pkg3.stPkgHead.dwReserve > 0) && (pkg3.stPkgHead.dwReserve == pkg2.stPkgHead.dwMsgID))
                            {
                                pkg3.Release();
                                this.confirmSendQueue.RemoveAt(index);
                            }
                            else
                            {
                                index++;
                            }
                        }
                        return pkg2;
                    }
                }
            }
            return null;
        }

        public bool SendPackage(CSPkg msg)
        {
            if (!base.connected || (base.connector == null))
            {
                return false;
            }
            int usedSize = 0;
            if (msg.pack(ref this.szSendBuffer, this.nBuffSize, ref usedSize, 0) != TdrError.ErrorType.TDR_NO_ERROR)
            {
                return false;
            }
            if (!base.initParam.bIsUDP || ((msg.stPkgHead.dwMsgID != 0x3ec) && (msg.stPkgHead.dwMsgID != 0x4ec)))
            {
                return (base.connector.WriteData(this.szSendBuffer, usedSize) == ApolloResult.Success);
            }
            return (base.connector.WriteUdpData(this.szSendBuffer, usedSize) == ApolloResult.Success);
        }

        public void Update()
        {
            this.reconPolicy.UpdatePolicy(false);
            if (this.netStateChanged)
            {
                if (this.changedNetState == NetworkState.NotReachable)
                {
                    Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(Singleton<CTextManager>.GetInstance().GetText("NetworkConnecting"), 10, enUIEventID.None);
                }
                else
                {
                    Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
                }
                this.netStateChanged = false;
            }
        }
    }
}

