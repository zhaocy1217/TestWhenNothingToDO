namespace com.tencent.pandora
{
    using com.tencent.pandora.MiniJSON;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using UnityEngine;

    public class WeChatLink
    {
        private Action<Dictionary<string, string>> callbackForGame;
        private int connectTimeOut = 400;
        private bool firstReceive = true;
        private int haveRcvdLen;
        public static readonly WeChatLink Instance = new WeChatLink();
        private string kSDKVersion = "YXZJ-0.1";
        private int packetDataLen;
        private byte[] receivedByte;
        private int sendRcvTimeOut = 0xbb8;
        private List<string> urlList = new List<string>();
        private Dictionary<string, string> userData = new Dictionary<string, string>();

        private void AsynCallBroker(Socket socket, string message)
        {
            if ((socket != null) && (message != string.Empty))
            {
                Debug.Log(string.Empty);
                try
                {
                    int num = 0x2328;
                    Dictionary<string, object> dictionary = new Dictionary<string, object>();
                    dictionary["seq_id"] = 1;
                    dictionary["cmd_id"] = num;
                    dictionary["type"] = 1;
                    dictionary["from_ip"] = "10.0.0.108";
                    dictionary["process_id"] = 1;
                    dictionary["mod_id"] = 10;
                    dictionary["version"] = this.kSDKVersion;
                    dictionary["body"] = message;
                    dictionary["app_id"] = this.userData["sAppId"];
                    string rawData = Json.Serialize(dictionary);
                    byte[] sourceArray = Convert.FromBase64String(MinizLib.Compress(rawData.Length, rawData));
                    byte[] bytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(sourceArray.Length));
                    byte[] destinationArray = new byte[bytes.Length + sourceArray.Length];
                    Array.Copy(bytes, 0, destinationArray, 0, bytes.Length);
                    Array.Copy(sourceArray, 0, destinationArray, bytes.Length, sourceArray.Length);
                    IAsyncResult asyncResult = socket.BeginSend(destinationArray, 0, destinationArray.Length, SocketFlags.None, null, null);
                    WaitHandle asyncWaitHandle = asyncResult.AsyncWaitHandle;
                    try
                    {
                        if (!asyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds((double) this.sendRcvTimeOut)))
                        {
                            Debug.Log("Send Time Out:" + this.sendRcvTimeOut.ToString());
                            this.CloseSocket(socket);
                        }
                        else
                        {
                            try
                            {
                                int num3 = socket.EndSend(asyncResult);
                                if (asyncResult.IsCompleted && (num3 == destinationArray.Length))
                                {
                                    Debug.Log(string.Format("客户端发送消息:{0}", rawData));
                                    this.AsynRecive(socket);
                                }
                                else
                                {
                                    this.CloseSocket(socket);
                                }
                            }
                            catch (Exception exception)
                            {
                                Debug.Log(exception.Message);
                                this.CloseSocket(socket);
                            }
                        }
                    }
                    catch (Exception exception2)
                    {
                        Debug.Log(exception2.Message);
                        this.CloseSocket(socket);
                    }
                    finally
                    {
                        asyncWaitHandle.Close();
                    }
                }
                catch (Exception exception3)
                {
                    Debug.Log(string.Format("异常信息：{0}", exception3.Message));
                    this.CloseSocket(socket);
                }
            }
        }

        private void AsynRecive(Socket socket)
        {
            Debug.Log(string.Empty);
            StateObject state = new StateObject();
            state.workSocket = socket;
            try
            {
                IAsyncResult asyncResult = socket.BeginReceive(state.buffer, 0, 0x400, SocketFlags.None, null, state);
                WaitHandle asyncWaitHandle = asyncResult.AsyncWaitHandle;
                try
                {
                    if (!asyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds((double) this.sendRcvTimeOut)))
                    {
                        Debug.Log("Receive Time Out:" + this.sendRcvTimeOut.ToString());
                        this.CloseSocket(socket);
                    }
                    else
                    {
                        this.ReceiveCallback(asyncResult);
                    }
                }
                catch (Exception exception)
                {
                    Debug.Log(exception.Message);
                    this.CloseSocket(socket);
                }
                finally
                {
                    asyncWaitHandle.Close();
                }
            }
            catch (Exception exception2)
            {
                Debug.Log(exception2.Message);
                this.CloseSocket(socket);
            }
        }

        public void BeginGetGameZoneUrl(Dictionary<string, string> userDataDict, Action<Dictionary<string, string>> action)
        {
            this.userData = userDataDict;
            this.callbackForGame = action;
            string ipString = "wzry.broker.tplay.qq.com";
            int port = 0x163c;
            List<IPAddress> list = new List<IPAddress>();
            if (ipString.Length > 0)
            {
                try
                {
                    IPAddress address = null;
                    if (IPAddress.TryParse(ipString, out address))
                    {
                        list.Add(address);
                    }
                    else
                    {
                        foreach (IPAddress address2 in Dns.GetHostEntry(ipString).AddressList)
                        {
                            list.Add(address2);
                        }
                    }
                }
                catch (Exception exception)
                {
                    Debug.Log(exception.Message);
                    this.NotifyShowZone(false);
                }
            }
            if (list.Count > 0)
            {
                int num3 = new Random().Next(list.Count);
                IPEndPoint remoteEP = new IPEndPoint(list[num3], port);
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, this.sendRcvTimeOut);
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, this.sendRcvTimeOut);
                Debug.Log("Begin Connet");
                IAsyncResult asyncResult = socket.BeginConnect(remoteEP, null, null);
                WaitHandle asyncWaitHandle = asyncResult.AsyncWaitHandle;
                try
                {
                    if (!asyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds((double) this.connectTimeOut)))
                    {
                        Debug.Log("Connect Time Out:" + this.connectTimeOut.ToString());
                        this.CloseSocket(socket);
                    }
                    else
                    {
                        try
                        {
                            socket.EndConnect(asyncResult);
                            if (asyncResult.IsCompleted)
                            {
                                string actReqJson = this.GetActReqJson();
                                this.AsynCallBroker(socket, actReqJson);
                            }
                        }
                        catch (Exception exception2)
                        {
                            Debug.Log(exception2.Message);
                            this.CloseSocket(socket);
                        }
                    }
                }
                catch (Exception exception3)
                {
                    Debug.Log(exception3.Message);
                    this.CloseSocket(socket);
                }
                finally
                {
                    asyncWaitHandle.Close();
                }
            }
        }

        private void CloseSocket(Socket socket)
        {
            try
            {
                socket.Close();
            }
            catch (SocketException exception)
            {
                Debug.Log(exception.Message);
            }
            catch (ObjectDisposedException exception2)
            {
                Debug.Log(exception2.Message);
            }
            finally
            {
                this.NotifyShowZone(false);
            }
        }

        private string GetActReqJson()
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary["seq_id"] = "1";
            dictionary["cmd_id"] = "10000";
            dictionary["msg_type"] = "1";
            dictionary["sdk_version"] = this.kSDKVersion;
            dictionary["game_app_id"] = this.userData["sAppId"];
            dictionary["channel_id"] = "23029";
            dictionary["plat_id"] = this.userData["sPlatID"];
            dictionary["area_id"] = this.userData["sArea"];
            dictionary["patition_id"] = this.userData["sPartition"];
            dictionary["open_id"] = this.userData["sOpenId"];
            dictionary["role_id"] = this.userData["sRoleId"];
            dictionary["act_style"] = "4";
            dictionary["timestamp"] = Utils.NowSeconds();
            Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
            dictionary2["md5_val"] = string.Empty;
            Dictionary<string, object> dictionary3 = new Dictionary<string, object>();
            dictionary3["head"] = dictionary;
            dictionary3["body"] = dictionary2;
            string str = Json.Serialize(dictionary3);
            Debug.Log(str);
            return str;
        }

        private void HandleRcvData(StateObject so, int readLen)
        {
            if ((this.haveRcvdLen + readLen) < this.packetDataLen)
            {
                this.haveRcvdLen += readLen;
                Array.Copy(so.buffer, this.receivedByte, readLen);
                this.AsynRecive(so.workSocket);
            }
            else
            {
                Array.Copy(so.buffer, this.receivedByte, (int) (this.packetDataLen - this.haveRcvdLen));
                this.haveRcvdLen = this.packetDataLen;
                this.ParseActData();
                this.CloseSocket(so.workSocket);
            }
        }

        private void NotifyShowZone(bool bShow)
        {
            if (this.callbackForGame != null)
            {
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                if (bShow)
                {
                    dictionary.Add("showGameZone", "1");
                }
                else
                {
                    dictionary.Add("showGameZone", "0");
                }
                this.callbackForGame(dictionary);
            }
            this.Reset();
        }

        private void ParseActData()
        {
            try
            {
                string encodedCompressedData = Convert.ToBase64String(this.receivedByte, 4, this.receivedByte.Length - 4);
                byte[] bytes = Convert.FromBase64String(MinizLib.UnCompress(encodedCompressedData.Length, encodedCompressedData));
                string json = Encoding.UTF8.GetString(bytes);
                Debug.Log(json);
                Dictionary<string, object> dictionary = new Dictionary<string, object>();
                dictionary = Json.Deserialize(json) as Dictionary<string, object>;
                if ((dictionary != null) && dictionary.ContainsKey("body"))
                {
                    string str4 = dictionary["body"] as string;
                    Debug.Log(str4);
                    Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
                    dictionary2 = Json.Deserialize(str4) as Dictionary<string, object>;
                    if ((dictionary2 != null) && dictionary2.ContainsKey("ret"))
                    {
                        Debug.Log(string.Empty);
                        if (dictionary2["ret"].ToString().Equals("0") && dictionary2.ContainsKey("resp"))
                        {
                            string str6 = dictionary2["resp"] as string;
                            Debug.Log(str6);
                            Dictionary<string, object> dictionary3 = new Dictionary<string, object>();
                            dictionary3 = Json.Deserialize(str6) as Dictionary<string, object>;
                            if ((dictionary3 != null) && dictionary3.ContainsKey("body"))
                            {
                                string str7 = dictionary3["body"] as string;
                                Debug.Log(str7);
                                Dictionary<string, object> dictionary4 = new Dictionary<string, object>();
                                dictionary4 = Json.Deserialize(str7) as Dictionary<string, object>;
                                if ((dictionary4 != null) && dictionary4.ContainsKey("online_msg_info"))
                                {
                                    Dictionary<string, object> dictionary5 = dictionary4["online_msg_info"] as Dictionary<string, object>;
                                    if ((dictionary5 != null) && dictionary5.ContainsKey("act_list"))
                                    {
                                        List<object> list = dictionary5["act_list"] as List<object>;
                                        foreach (object obj2 in list)
                                        {
                                            Dictionary<string, object> dictionary6 = obj2 as Dictionary<string, object>;
                                            if (((dictionary6 != null) && dictionary6.ContainsKey("jump_url")) && !string.IsNullOrEmpty(dictionary6["jump_url"].ToString()))
                                            {
                                                Debug.Log("获取到Url: " + dictionary6["jump_url"].ToString());
                                                this.urlList.Add(dictionary6["jump_url"].ToString());
                                            }
                                        }
                                        if (list.Count > 0)
                                        {
                                            this.NotifyShowZone(true);
                                        }
                                        else
                                        {
                                            this.NotifyShowZone(false);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                this.NotifyShowZone(false);
                Debug.Log(exception.StackTrace);
            }
        }

        private void ReceiveCallback(IAsyncResult asyncResult)
        {
            Debug.Log(string.Empty);
            StateObject asyncState = (StateObject) asyncResult.AsyncState;
            Socket workSocket = asyncState.workSocket;
            try
            {
                int readLen = workSocket.EndReceive(asyncResult);
                if (readLen > 0)
                {
                    if (this.firstReceive)
                    {
                        if (readLen > 4)
                        {
                            this.firstReceive = false;
                            byte[] destinationArray = new byte[4];
                            Array.Copy(asyncState.buffer, destinationArray, 4);
                            this.packetDataLen = BitConverter.ToInt32(destinationArray, 0);
                            this.packetDataLen = IPAddress.NetworkToHostOrder(this.packetDataLen) + 4;
                            this.receivedByte = new byte[this.packetDataLen];
                            this.HandleRcvData(asyncState, readLen);
                        }
                        else
                        {
                            this.CloseSocket(workSocket);
                        }
                    }
                    else
                    {
                        this.HandleRcvData(asyncState, readLen);
                    }
                }
            }
            catch (Exception exception)
            {
                Debug.Log(exception.Message);
                this.CloseSocket(workSocket);
            }
        }

        private void Reset()
        {
            Debug.Log(string.Empty);
            this.callbackForGame = null;
            this.userData = new Dictionary<string, string>();
            this.firstReceive = true;
            this.packetDataLen = 0;
            this.haveRcvdLen = 0;
            this.receivedByte = null;
            this.urlList = new List<string>();
        }

        private class StateObject
        {
            public byte[] buffer = new byte[0x400];
            public const int BUFFER_SIZE = 0x400;
            public Socket workSocket;
        }
    }
}

