namespace com.tencent.pandora
{
    using com.tencent.pandora.MiniJSON;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Net;
    using System.Runtime.CompilerServices;

    public class NetLogic
    {
        private List<IPAddress> atmIPAddresses = new List<IPAddress>();
        private Queue atmPendingRequests = Queue.Synchronized(new Queue());
        private static ushort atmReportPort = 0x163c;
        private long atmUniqueSocketId = -1L;
        private string[] brokerAltIPs = new string[0];
        private string brokerHost = string.Empty;
        private List<IPAddress> brokerIPAddresses = new List<IPAddress>();
        private Queue brokerPendingRequests = Queue.Synchronized(new Queue());
        private ushort brokerPort;
        private long brokerUniqueSocketId = -1L;
        private Dictionary<string, IPHostEntry> dnsCache = new Dictionary<string, IPHostEntry>();
        private Queue downloadRequestQueue = Queue.Synchronized(new Queue());
        private bool isAtmConnecting;
        private bool isBrokerConnecting;
        private bool isDownloadingPaused;
        private int lastConnectAtmTime = -1;
        private int lastConnectBrokerTime = -1;
        private int lastTryAtmIPAddressIndex;
        private int lastTryBorkerIPAddressIndex;
        private Queue logicDriveQueue = Queue.Synchronized(new Queue());
        private NetFrame netFrame = new NetFrame();
        private Queue resultQueue = Queue.Synchronized(new Queue());
        private static uint streamReportSeqNo;

        public void AddDownload(string url, int size, string md5, string destFile, int curRedirectionTimes, Action<int, Dictionary<string, object>> action)
        {
            Logger.DEBUG(url);
            DownloadRequest request = new DownloadRequest();
            request.url = url;
            request.size = size;
            request.md5 = md5;
            request.destFile = destFile;
            request.curRedirectionTimes = curRedirectionTimes;
            request.action = action;
            this.downloadRequestQueue.Enqueue(request);
        }

        public void AsyncGetHostEntry(string host, Action<IPHostEntry> resultAction)
        {
            <AsyncGetHostEntry>c__AnonStorey80 storey = new <AsyncGetHostEntry>c__AnonStorey80();
            storey.resultAction = resultAction;
            storey.<>f__this = this;
            Logger.DEBUG(host);
            try
            {
                IPAddress address = null;
                if (IPAddress.TryParse(host, out address))
                {
                    IPHostEntry entry = new IPHostEntry();
                    entry.AddressList = new IPAddress[] { address };
                    storey.resultAction(entry);
                }
                else if (this.dnsCache.ContainsKey(host))
                {
                    IPHostEntry entry2 = this.dnsCache[host];
                    storey.resultAction(entry2);
                }
                else
                {
                    Action<IAsyncResult> action = new Action<IAsyncResult>(storey.<>m__94);
                    Logger.DEBUG(host + " begin");
                    Dns.BeginGetHostEntry(host, new AsyncCallback(action.Invoke), host);
                }
            }
            catch (Exception exception)
            {
                storey.resultAction(null);
                Logger.ERROR(exception.Message + ":" + exception.StackTrace);
            }
        }

        public void CallBroker(uint callId, string jsonData, int cmdId)
        {
            Logger.DEBUG(string.Empty);
            try
            {
                PendingRequest request = new PendingRequest();
                request.createTime = Utils.NowSeconds();
                request.seqNo = callId;
                Dictionary<string, object> dictionary = new Dictionary<string, object>();
                UserData userData = Pandora.Instance.GetUserData();
                dictionary["seq_id"] = callId;
                dictionary["cmd_id"] = cmdId;
                dictionary["type"] = 1;
                dictionary["from_ip"] = "10.0.0.108";
                dictionary["process_id"] = 1;
                dictionary["mod_id"] = 10;
                dictionary["version"] = Pandora.Instance.GetSDKVersion();
                dictionary["body"] = jsonData;
                dictionary["app_id"] = userData.sAppId;
                string rawData = Json.Serialize(dictionary);
                byte[] sourceArray = Convert.FromBase64String(MinizLib.Compress(rawData.Length, rawData));
                byte[] bytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(sourceArray.Length));
                byte[] destinationArray = new byte[bytes.Length + sourceArray.Length];
                Array.Copy(bytes, 0, destinationArray, 0, bytes.Length);
                Array.Copy(sourceArray, 0, destinationArray, bytes.Length, sourceArray.Length);
                request.data = destinationArray;
                this.brokerPendingRequests.Enqueue(request);
            }
            catch (Exception exception)
            {
                Logger.ERROR(exception.StackTrace);
            }
        }

        private void CheckAtmSession()
        {
            if (this.atmIPAddresses.Count == 0)
            {
                <CheckAtmSession>c__AnonStorey7A storeya = new <CheckAtmSession>c__AnonStorey7A();
                storeya.<>f__this = this;
                storeya.atmHost = "jsonatm.broker.tplay.qq.com";
                Action<IPHostEntry> resultAction = new Action<IPHostEntry>(storeya.<>m__8D);
                this.lastConnectAtmTime = Utils.NowSeconds();
                this.AsyncGetHostEntry(storeya.atmHost, resultAction);
            }
            else
            {
                try
                {
                    Action<int, Dictionary<string, object>> statusChangedAction = new Action<int, Dictionary<string, object>>(this, (IntPtr) this.<CheckAtmSession>m__8E);
                    this.lastConnectAtmTime = Utils.NowSeconds();
                    this.lastTryAtmIPAddressIndex = (this.lastTryAtmIPAddressIndex + 1) % this.atmIPAddresses.Count;
                    if (this.SpawnTCPSession(this.atmIPAddresses[this.lastTryAtmIPAddressIndex], atmReportPort, new ATMHandler(statusChangedAction)) > 0L)
                    {
                        Logger.DEBUG(string.Empty);
                        this.isAtmConnecting = true;
                    }
                    else
                    {
                        Logger.ERROR(string.Empty);
                    }
                }
                catch (Exception exception)
                {
                    Logger.ERROR(exception.StackTrace);
                }
            }
        }

        private void CheckBrokerSession()
        {
            if (this.brokerIPAddresses.Count == 0)
            {
                this.lastConnectBrokerTime = Utils.NowSeconds();
            }
            else
            {
                try
                {
                    Action<int, Dictionary<string, object>> statusChangedAction = new Action<int, Dictionary<string, object>>(this, (IntPtr) this.<CheckBrokerSession>m__8F);
                    Action<int, Dictionary<string, object>> packetRecvdAction = new Action<int, Dictionary<string, object>>(this, (IntPtr) this.<CheckBrokerSession>m__90);
                    this.lastConnectBrokerTime = Utils.NowSeconds();
                    this.lastTryBorkerIPAddressIndex = (this.lastTryBorkerIPAddressIndex + 1) % this.brokerIPAddresses.Count;
                    if (this.SpawnTCPSession(this.brokerIPAddresses[this.lastTryBorkerIPAddressIndex], this.brokerPort, new BrokerHandler(statusChangedAction, packetRecvdAction)) > 0L)
                    {
                        Logger.DEBUG(this.brokerIPAddresses[this.lastTryBorkerIPAddressIndex].ToString());
                        this.isBrokerConnecting = true;
                    }
                    else
                    {
                        Logger.ERROR(string.Empty);
                    }
                }
                catch (Exception exception)
                {
                    Logger.ERROR(exception.StackTrace);
                }
            }
        }

        public void Close(long uniqueSocketId)
        {
            Logger.DEBUG(uniqueSocketId.ToString());
            this.netFrame.Close(uniqueSocketId);
        }

        public void Destroy()
        {
            Logger.DEBUG(string.Empty);
            this.netFrame.Destroy();
        }

        private void DownloadFile(string url, int size, string md5, string destFile, int curRedirectionTimes, Action<int, Dictionary<string, object>> action)
        {
            <DownloadFile>c__AnonStorey7D storeyd = new <DownloadFile>c__AnonStorey7D();
            storeyd.url = url;
            storeyd.size = size;
            storeyd.md5 = md5;
            storeyd.destFile = destFile;
            storeyd.curRedirectionTimes = curRedirectionTimes;
            storeyd.action = action;
            storeyd.<>f__this = this;
            Logger.DEBUG(storeyd.url);
            try
            {
                <DownloadFile>c__AnonStorey7E storeye = new <DownloadFile>c__AnonStorey7E();
                storeye.<>f__ref$125 = storeyd;
                storeye.<>f__this = this;
                storeye.uri = new Uri(storeyd.url);
                Action<IPHostEntry> resultAction = new Action<IPHostEntry>(storeye.<>m__93);
                this.AsyncGetHostEntry(storeye.uri.Host, resultAction);
            }
            catch (Exception exception)
            {
                storeyd.action.Invoke(-1, new Dictionary<string, object>());
                Logger.ERROR(exception.Message + ":" + exception.StackTrace);
            }
        }

        public void Drive()
        {
            if (!this.isDownloadingPaused)
            {
                this.netFrame.Drive();
                while (this.resultQueue.Count > 0)
                {
                    Message message = this.resultQueue.Dequeue() as Message;
                    if (message.action != null)
                    {
                        message.action.Invoke(message.status, message.content);
                    }
                }
                int num = Utils.NowSeconds();
                if (((this.atmUniqueSocketId < 0L) && !this.isAtmConnecting) && ((this.lastConnectAtmTime + 5) < num))
                {
                    this.CheckAtmSession();
                }
                if (((this.brokerUniqueSocketId < 0L) && !this.isBrokerConnecting) && ((this.lastConnectBrokerTime + 5) < num))
                {
                    this.CheckBrokerSession();
                }
                this.TrySendAtmReport();
                this.TrySendBrokerRequest();
                while (this.logicDriveQueue.Count > 0)
                {
                    Message message2 = this.logicDriveQueue.Dequeue() as Message;
                    if (message2.action != null)
                    {
                        message2.action.Invoke(message2.status, message2.content);
                    }
                }
                while (this.downloadRequestQueue.Count > 0)
                {
                    <Drive>c__AnonStorey79 storey = new <Drive>c__AnonStorey79();
                    storey.<>f__this = this;
                    storey.request = this.downloadRequestQueue.Dequeue() as DownloadRequest;
                    Action<int, Dictionary<string, object>> action = new Action<int, Dictionary<string, object>>(storey, (IntPtr) this.<>m__8C);
                    this.DownloadFile(storey.request.url, storey.request.size, storey.request.md5, storey.request.destFile, storey.request.curRedirectionTimes, action);
                }
            }
        }

        public void EnqueueDrive(Message msg)
        {
            this.logicDriveQueue.Enqueue(msg);
        }

        public void EnqueueResult(Message msg)
        {
            this.resultQueue.Enqueue(msg);
        }

        public void GetRemoteConfig(Action<int, Dictionary<string, object>> action)
        {
            <GetRemoteConfig>c__AnonStorey7C storeyc = new <GetRemoteConfig>c__AnonStorey7C();
            storeyc.action = action;
            storeyc.<>f__this = this;
            Logger.DEBUG(string.Empty);
            try
            {
                <GetRemoteConfig>c__AnonStorey7B storeyb = new <GetRemoteConfig>c__AnonStorey7B();
                storeyb.<>f__ref$124 = storeyc;
                storeyb.<>f__this = this;
                storeyb.configUrl = "http://apps.game.qq.com/cgi-bin/api/tplay/" + Pandora.Instance.GetUserData().sAppId + "_cloud.cgi";
                Logger.DEBUG(storeyb.configUrl);
                storeyb.uri = new Uri(storeyb.configUrl);
                Action<IPHostEntry> resultAction = new Action<IPHostEntry>(storeyb.<>m__92);
                this.AsyncGetHostEntry(storeyb.uri.Host, resultAction);
            }
            catch (Exception exception)
            {
                storeyc.action.Invoke(-1, new Dictionary<string, object>());
                Logger.ERROR(exception.Message + ":" + exception.StackTrace);
            }
        }

        public void Init()
        {
            this.netFrame.Init();
        }

        public void Logout()
        {
            Logger.DEBUG(string.Empty);
            this.netFrame.Reset();
            this.resultQueue.Clear();
            this.isDownloadingPaused = false;
            this.atmIPAddresses.Clear();
            this.lastTryAtmIPAddressIndex = 0;
            this.atmUniqueSocketId = -1L;
            this.lastConnectAtmTime = -1;
            this.isAtmConnecting = false;
            this.atmPendingRequests.Clear();
            this.brokerPort = 0;
            this.brokerHost = string.Empty;
            this.brokerAltIPs = new string[0];
            this.brokerIPAddresses.Clear();
            this.lastTryBorkerIPAddressIndex = 0;
            this.brokerUniqueSocketId = -1L;
            this.lastConnectBrokerTime = -1;
            this.isBrokerConnecting = false;
            this.brokerPendingRequests.Clear();
            this.downloadRequestQueue.Clear();
            this.logicDriveQueue.Clear();
        }

        public void ProcessBrokerResponse(int status, Dictionary<string, object> content)
        {
            Logger.DEBUG(string.Empty);
            try
            {
                long num = (long) content["type"];
                if (num == 1L)
                {
                    Logger.ERROR("recv invalid type[" + num.ToString() + "] from broker");
                }
                else if (num == 2L)
                {
                    int num2 = (int) ((long) content["cmd_id"]);
                    uint callId = (uint) ((long) content["seq_id"]);
                    switch (num2)
                    {
                        case 0x1389:
                        {
                            Logger.DEBUG("recv statistics rsp, seqId[" + callId.ToString() + "]");
                            string json = content["body"] as string;
                            Dictionary<string, object> dictionary = Json.Deserialize(json) as Dictionary<string, object>;
                            int num4 = (int) ((long) dictionary["ret"]);
                            if (num4 != 0)
                            {
                                Logger.ERROR(string.Concat(new object[] { "recv error statistics rsp, ret[", num4.ToString(), "] errMsg[", dictionary["err_msg"] }) + "]");
                            }
                            return;
                        }
                        case 0x2328:
                        {
                            Logger.DEBUG("recv lua request rsp, seqId[" + callId.ToString() + "]");
                            Dictionary<string, object> dictionary2 = Json.Deserialize(content["body"] as string) as Dictionary<string, object>;
                            dictionary2["netRet"] = 0;
                            string result = Json.Serialize(dictionary2);
                            CSharpInterface.ExecCallback(callId, result);
                            return;
                        }
                    }
                    Logger.ERROR("recv invalid cmdId[" + num2.ToString() + "] from broker");
                }
                else
                {
                    Logger.ERROR("recv invalid type[" + num.ToString() + "] from broker");
                }
            }
            catch (Exception exception)
            {
                Logger.ERROR(exception.StackTrace);
            }
        }

        public int SendPacket(long uniqueSocketId, byte[] content)
        {
            Logger.DEBUG(uniqueSocketId.ToString());
            return this.netFrame.SendPacket(uniqueSocketId, content);
        }

        public void SetBroker(ushort port, string host, string ip1, string ip2)
        {
            Logger.DEBUG("port=" + port.ToString() + " host=" + host + " ip1=" + ip1 + " ip2=" + ip2);
            this.brokerPort = port;
            this.brokerHost = host;
            this.brokerAltIPs = new string[] { ip1, ip2 };
            Action<IPHostEntry> resultAction = delegate (IPHostEntry entry) {
                try
                {
                    Logger.DEBUG(this.brokerHost + " dns done");
                    if ((entry != null) && (entry.AddressList.Length > 0))
                    {
                        foreach (IPAddress address in entry.AddressList)
                        {
                            this.brokerIPAddresses.Add(address);
                        }
                    }
                    else
                    {
                        foreach (string str in this.brokerAltIPs)
                        {
                            if (str.Length > 0)
                            {
                                this.brokerIPAddresses.Add(IPAddress.Parse(str));
                            }
                        }
                    }
                    this.lastConnectBrokerTime = 0;
                }
                catch (Exception exception)
                {
                    Logger.ERROR(exception.Message + ": " + exception.StackTrace);
                }
            };
            this.AsyncGetHostEntry(this.brokerHost, resultAction);
        }

        public void SetDownloadingPaused(bool status)
        {
            this.isDownloadingPaused = status;
        }

        public long SpawnTCPSession(IPAddress addr, ushort port, TCPSocketHandler handler)
        {
            Logger.DEBUG(string.Empty);
            return this.netFrame.AsyncConnect(addr, port, handler);
        }

        public void StreamReport(string jsonData)
        {
            Logger.DEBUG(string.Empty);
            try
            {
                PendingRequest request = new PendingRequest();
                request.createTime = Utils.NowSeconds();
                request.seqNo = streamReportSeqNo++;
                Dictionary<string, object> dictionary = new Dictionary<string, object>();
                UserData userData = Pandora.Instance.GetUserData();
                dictionary["seq_id"] = request.seqNo;
                dictionary["cmd_id"] = 0x1388;
                dictionary["type"] = 1;
                dictionary["from_ip"] = "10.0.0.108";
                dictionary["process_id"] = 1;
                dictionary["mod_id"] = 10;
                dictionary["version"] = Pandora.Instance.GetSDKVersion();
                dictionary["body"] = jsonData;
                dictionary["app_id"] = userData.sAppId;
                string rawData = Json.Serialize(dictionary);
                byte[] sourceArray = Convert.FromBase64String(MinizLib.Compress(rawData.Length, rawData));
                byte[] bytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(sourceArray.Length));
                byte[] destinationArray = new byte[bytes.Length + sourceArray.Length];
                Array.Copy(bytes, 0, destinationArray, 0, bytes.Length);
                Array.Copy(sourceArray, 0, destinationArray, bytes.Length, sourceArray.Length);
                request.data = destinationArray;
                this.atmPendingRequests.Enqueue(request);
            }
            catch (Exception exception)
            {
                Logger.ERROR(exception.StackTrace);
            }
        }

        private void TrySendAtmReport()
        {
            if (this.atmUniqueSocketId > 0L)
            {
                while (this.atmPendingRequests.Count > 0)
                {
                    Logger.DEBUG(string.Empty);
                    PendingRequest request = this.atmPendingRequests.Dequeue() as PendingRequest;
                    this.netFrame.SendPacket(this.atmUniqueSocketId, request.data);
                }
            }
            else
            {
                int num = Utils.NowSeconds();
                while (this.atmPendingRequests.Count > 0)
                {
                    PendingRequest request2 = this.atmPendingRequests.Peek() as PendingRequest;
                    if (num <= (request2.createTime + 10))
                    {
                        break;
                    }
                    Logger.WARN(string.Empty);
                    this.atmPendingRequests.Dequeue();
                }
            }
        }

        private void TrySendBrokerRequest()
        {
            if (this.brokerUniqueSocketId > 0L)
            {
                while (this.brokerPendingRequests.Count > 0)
                {
                    Logger.DEBUG(string.Empty);
                    PendingRequest request = this.brokerPendingRequests.Dequeue() as PendingRequest;
                    this.netFrame.SendPacket(this.brokerUniqueSocketId, request.data);
                }
            }
            else
            {
                int num = Utils.NowSeconds();
                while (this.brokerPendingRequests.Count > 0)
                {
                    PendingRequest request2 = this.brokerPendingRequests.Peek() as PendingRequest;
                    if (num <= (request2.createTime + 5))
                    {
                        break;
                    }
                    Logger.WARN(string.Empty);
                    this.brokerPendingRequests.Dequeue();
                }
            }
        }

        [CompilerGenerated]
        private sealed class <AsyncGetHostEntry>c__AnonStorey80
        {
            internal NetLogic <>f__this;
            internal Action<IPHostEntry> resultAction;

            internal void <>m__94(IAsyncResult ar)
            {
                try
                {
                    <AsyncGetHostEntry>c__AnonStorey7F storeyf = new <AsyncGetHostEntry>c__AnonStorey7F();
                    storeyf.<>f__ref$128 = this;
                    string asyncState = (string) ar.AsyncState;
                    Logger.DEBUG(asyncState + " end");
                    storeyf.entry = Dns.EndGetHostEntry(ar);
                    Logger.DEBUG(asyncState + " end, entry.AddressList.Length=" + storeyf.entry.AddressList.Length.ToString());
                    Action<int, Dictionary<string, object>> action = new Action<int, Dictionary<string, object>>(storeyf, (IntPtr) this.<>m__95);
                    Message msg = new Message();
                    msg.status = 0;
                    msg.content["host"] = asyncState;
                    msg.content["entry"] = storeyf.entry;
                    msg.content["resultAction"] = this.resultAction;
                    msg.action = action;
                    this.<>f__this.EnqueueDrive(msg);
                }
                catch (Exception exception)
                {
                    Logger.ERROR(exception.Message + ":" + exception.StackTrace);
                }
            }

            private sealed class <AsyncGetHostEntry>c__AnonStorey7F
            {
                internal NetLogic.<AsyncGetHostEntry>c__AnonStorey80 <>f__ref$128;
                internal IPHostEntry entry;

                internal void <>m__95(int status, Dictionary<string, object> content)
                {
                    string str = content["host"] as string;
                    IPHostEntry entry = content["entry"] as IPHostEntry;
                    Action<IPHostEntry> action = content["resultAction"] as Action<IPHostEntry>;
                    this.<>f__ref$128.<>f__this.dnsCache[str] = this.entry;
                    action(this.entry);
                }
            }
        }

        [CompilerGenerated]
        private sealed class <CheckAtmSession>c__AnonStorey7A
        {
            internal NetLogic <>f__this;
            internal string atmHost;

            internal void <>m__8D(IPHostEntry entry)
            {
                Logger.DEBUG(this.atmHost + " dns done");
                if ((entry != null) && (entry.AddressList.Length > 0))
                {
                    foreach (IPAddress address in entry.AddressList)
                    {
                        this.<>f__this.atmIPAddresses.Add(address);
                    }
                }
                else
                {
                    string[] textArray1 = new string[] { "101.226.129.205", "140.206.160.193", "182.254.10.86", "115.25.209.29", "117.144.245.201" };
                    foreach (string str in textArray1)
                    {
                        this.<>f__this.atmIPAddresses.Add(IPAddress.Parse(str));
                    }
                }
                this.<>f__this.lastConnectAtmTime = 0;
            }
        }

        [CompilerGenerated]
        private sealed class <DownloadFile>c__AnonStorey7D
        {
            internal NetLogic <>f__this;
            internal Action<int, Dictionary<string, object>> action;
            internal int curRedirectionTimes;
            internal string destFile;
            internal string md5;
            internal int size;
            internal string url;
        }

        [CompilerGenerated]
        private sealed class <DownloadFile>c__AnonStorey7E
        {
            internal NetLogic.<DownloadFile>c__AnonStorey7D <>f__ref$125;
            internal NetLogic <>f__this;
            internal Uri uri;

            internal void <>m__93(IPHostEntry entry)
            {
                Logger.DEBUG(this.<>f__ref$125.url + " dns done");
                if ((entry != null) && (entry.AddressList.Length > 0))
                {
                    IPAddress[] addressList = entry.AddressList;
                    int index = new Random().Next(addressList.Length);
                    if (this.<>f__this.SpawnTCPSession(addressList[index], (ushort) this.uri.Port, new DownloadHandler(this.<>f__ref$125.url, this.<>f__ref$125.size, this.<>f__ref$125.md5, this.<>f__ref$125.destFile, this.<>f__ref$125.curRedirectionTimes, this.<>f__ref$125.action)) > 0L)
                    {
                        Logger.DEBUG(addressList[index].ToString());
                    }
                    else
                    {
                        Logger.ERROR(string.Empty);
                        this.<>f__ref$125.action.Invoke(-1, new Dictionary<string, object>());
                    }
                }
                else
                {
                    Logger.ERROR(string.Empty);
                    this.<>f__ref$125.action.Invoke(-1, new Dictionary<string, object>());
                }
            }
        }

        [CompilerGenerated]
        private sealed class <Drive>c__AnonStorey79
        {
            internal NetLogic <>f__this;
            internal NetLogic.DownloadRequest request;

            internal void <>m__8C(int downloadRet, Dictionary<string, object> content)
            {
                if (downloadRet == 100)
                {
                    Logger.DEBUG(string.Empty);
                    string url = content["locationUrl"] as string;
                    this.<>f__this.AddDownload(url, this.request.size, this.request.md5, this.request.destFile, this.request.curRedirectionTimes + 1, this.request.action);
                }
                else
                {
                    Logger.DEBUG(string.Empty);
                    Message msg = new Message();
                    msg.status = downloadRet;
                    msg.action = this.request.action;
                    msg.content = content;
                    this.<>f__this.EnqueueResult(msg);
                }
            }
        }

        [CompilerGenerated]
        private sealed class <GetRemoteConfig>c__AnonStorey7B
        {
            internal NetLogic.<GetRemoteConfig>c__AnonStorey7C <>f__ref$124;
            internal NetLogic <>f__this;
            internal string configUrl;
            internal Uri uri;

            internal void <>m__92(IPHostEntry entry)
            {
                Logger.DEBUG(this.configUrl + " dns done");
                if ((entry != null) && (entry.AddressList.Length > 0))
                {
                    IPAddress[] addressList = entry.AddressList;
                    int index = new Random().Next(addressList.Length);
                    if (this.<>f__this.SpawnTCPSession(addressList[index], (ushort) this.uri.Port, new ConfigHandler(this.<>f__ref$124.action, this.configUrl)) > 0L)
                    {
                        Logger.DEBUG(addressList[index].ToString());
                    }
                    else
                    {
                        Logger.ERROR(string.Empty);
                        this.<>f__ref$124.action.Invoke(-1, new Dictionary<string, object>());
                    }
                }
                else
                {
                    Logger.ERROR(string.Empty);
                    this.<>f__ref$124.action.Invoke(-1, new Dictionary<string, object>());
                }
            }
        }

        [CompilerGenerated]
        private sealed class <GetRemoteConfig>c__AnonStorey7C
        {
            internal NetLogic <>f__this;
            internal Action<int, Dictionary<string, object>> action;
        }

        public class DownloadRequest
        {
            public Action<int, Dictionary<string, object>> action;
            public int curRedirectionTimes;
            public string destFile = string.Empty;
            public string md5 = string.Empty;
            public int size;
            public string url = string.Empty;
        }
    }
}

