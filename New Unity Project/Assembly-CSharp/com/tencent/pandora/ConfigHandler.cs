namespace com.tencent.pandora
{
    using com.tencent.pandora.MiniJSON;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Text;

    public class ConfigHandler : TCPSocketHandler
    {
        private string configUrl = string.Empty;
        private static byte[] contentLengthField = Encoding.UTF8.GetBytes("Content-Length");
        private int detectedHeaderLength = -1;
        private static byte[] httpHeaderEnd = Encoding.UTF8.GetBytes("\r\n\r\n");
        private static int kMaxHttpHeaderLength = 0x2000;
        private static byte[] successStatusLine1_1 = Encoding.UTF8.GetBytes("HTTP/1.1 200 OK");
        private Action<int, Dictionary<string, object>> theAction;

        public ConfigHandler(Action<int, Dictionary<string, object>> action, string configUrl)
        {
            this.theAction = action;
            this.configUrl = configUrl;
        }

        public override int DetectPacketSize(byte[] receivedData, int dataLen)
        {
            Logger.DEBUG(string.Empty);
            int num = -1;
            for (int i = 0; i < ((dataLen - httpHeaderEnd.Length) + 1); i++)
            {
                bool flag = true;
                for (int n = 0; n < httpHeaderEnd.Length; n++)
                {
                    if (receivedData[i + n] != httpHeaderEnd[n])
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    this.detectedHeaderLength = i + httpHeaderEnd.Length;
                    num = i;
                    break;
                }
            }
            if (num < 0)
            {
                if (dataLen > kMaxHttpHeaderLength)
                {
                    Logger.ERROR(string.Empty);
                    return -1;
                }
                return 0;
            }
            for (int j = 0; (j < dataLen) && (j < successStatusLine1_1.Length); j++)
            {
                if ((j >= dataLen) || (receivedData[j] != successStatusLine1_1[j]))
                {
                    Logger.ERROR(string.Empty);
                    return -2;
                }
            }
            int num5 = -1;
            int index = -1;
            int num7 = -1;
            for (int k = 0; k < ((dataLen - contentLengthField.Length) + 1); k++)
            {
                bool flag2 = true;
                for (int num9 = 0; num9 < contentLengthField.Length; num9++)
                {
                    if (receivedData[k + num9] != contentLengthField[num9])
                    {
                        flag2 = false;
                        break;
                    }
                }
                if (flag2)
                {
                    num5 = k;
                    break;
                }
            }
            if (num5 < 0)
            {
                Logger.ERROR(string.Empty);
                return -3;
            }
            for (int m = num5 + contentLengthField.Length; m < dataLen; m++)
            {
                if ((receivedData[m] >= 0x30) && (receivedData[m] <= 0x39))
                {
                    if (index < 0)
                    {
                        index = m;
                    }
                }
                else if ((index >= 0) && (num7 < 0))
                {
                    num7 = m;
                }
            }
            if (((index < 0) || (num7 < 0)) || (index >= num7))
            {
                Logger.ERROR(string.Empty);
                return -3;
            }
            try
            {
                int num11 = Convert.ToInt32(Encoding.UTF8.GetString(receivedData, index, num7 - index));
                if (num11 > 0)
                {
                    return (this.detectedHeaderLength + num11);
                }
                Logger.ERROR(string.Empty);
                return -3;
            }
            catch (Exception exception)
            {
                Logger.ERROR(exception.StackTrace);
                return -3;
            }
        }

        public override void OnClose()
        {
            Logger.DEBUG(string.Empty);
            Message msg = new Message();
            msg.status = -1;
            msg.action = this.theAction;
            Pandora.Instance.GetNetLogic().EnqueueResult(msg);
        }

        public override void OnConnected()
        {
            <OnConnected>c__AnonStorey76 storey = new <OnConnected>c__AnonStorey76();
            storey.<>f__this = this;
            Logger.DEBUG(string.Empty);
            storey.uniqueSocketId = base.GetUniqueSocketId();
            Action<int, Dictionary<string, object>> action = new Action<int, Dictionary<string, object>>(storey, (IntPtr) this.<>m__89);
            Message msg = new Message();
            msg.status = 0;
            msg.action = action;
            Pandora.Instance.GetNetLogic().EnqueueResult(msg);
        }

        public override void OnReceived(Packet thePacket)
        {
            Logger.DEBUG(string.Empty);
            try
            {
                int length = thePacket.theContent.Length - this.detectedHeaderLength;
                byte[] destinationArray = new byte[length];
                Array.Copy(thePacket.theContent, this.detectedHeaderLength, destinationArray, 0, length);
                Dictionary<string, object> dictionary = Json.Deserialize(Encoding.UTF8.GetString(destinationArray)) as Dictionary<string, object>;
                string s = dictionary["data"] as string;
                string formatMsg = MsdkTea.Decode(Convert.FromBase64String(s));
                if (formatMsg.Length > 0)
                {
                    Logger.DEBUG(formatMsg);
                    Dictionary<string, object> dictionary2 = Json.Deserialize(formatMsg) as Dictionary<string, object>;
                    Message msg = new Message();
                    msg.status = 0;
                    msg.action = this.theAction;
                    msg.content = dictionary2;
                    Pandora.Instance.GetNetLogic().EnqueueResult(msg);
                    this.theAction = null;
                }
            }
            catch (Exception exception)
            {
                Logger.ERROR(exception.StackTrace);
            }
            long uniqueSocketId = base.GetUniqueSocketId();
            Pandora.Instance.GetNetLogic().Close(uniqueSocketId);
        }

        [CompilerGenerated]
        private sealed class <OnConnected>c__AnonStorey76
        {
            internal ConfigHandler <>f__this;
            internal long uniqueSocketId;

            internal void <>m__89(int status, Dictionary<string, object> content)
            {
                try
                {
                    UserData userData = Pandora.Instance.GetUserData();
                    string[] textArray1 = new string[] { 
                        "openid=", userData.sOpenId, "&partition=", userData.sPartition, "&gameappversion=", userData.sGameVer, "&areaid=", userData.sArea, "&appid=", userData.sAppId, "&acctype=", userData.sAcountType, "&platid=", userData.sPlatID, "&sdkversion=", Pandora.Instance.GetSDKVersion(), 
                        "&_pdr_time=", Utils.NowSeconds().ToString()
                     };
                    string str = string.Concat(textArray1);
                    Dictionary<string, string> dictionary = new Dictionary<string, string>();
                    dictionary["openid"] = userData.sOpenId;
                    dictionary["partition"] = userData.sPartition;
                    dictionary["gameappversion"] = userData.sGameVer;
                    dictionary["areaid"] = userData.sArea;
                    dictionary["acctype"] = userData.sAcountType;
                    dictionary["platid"] = userData.sPlatID;
                    dictionary["appid"] = userData.sAppId;
                    dictionary["sdkversion"] = Pandora.Instance.GetSDKVersion();
                    string str3 = MsdkTea.Encode(Json.Serialize(dictionary));
                    string str4 = "{\"data\":\"" + str3 + "\",\"encrypt\" : \"true\"}";
                    Uri uri = new Uri(this.<>f__this.configUrl);
                    string[] textArray2 = new string[] { "POST ", uri.AbsolutePath, "?", str, " HTTP/1.1\r\nHost:", uri.Host, "\r\nAccept:*/*\r\nUser-Agent:Pandora(", Pandora.Instance.GetSDKVersion(), ")\r\nContent-Length:", str4.Length.ToString(), "\r\nConnection: keep-alive\r\n\r\n", str4 };
                    string formatMsg = string.Concat(textArray2);
                    Logger.DEBUG(formatMsg);
                    byte[] bytes = Encoding.UTF8.GetBytes(formatMsg);
                    Pandora.Instance.GetNetLogic().SendPacket(this.uniqueSocketId, bytes);
                }
                catch (Exception exception)
                {
                    Logger.ERROR(exception.Message);
                    Pandora.Instance.GetNetLogic().Close(this.uniqueSocketId);
                }
            }
        }
    }
}

