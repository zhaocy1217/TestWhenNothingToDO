namespace com.tencent.pandora
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Security.Cryptography;
    using System.Text;

    public class DownloadHandler : TCPSocketHandler
    {
        private static byte[] contentLengthField = Encoding.UTF8.GetBytes("Content-Length");
        private int curRedirectTimes;
        private string destFile = string.Empty;
        private int detecedContentLength = -1;
        private int detectedHeaderLength = -1;
        private int fileSizeBeforeDownload;
        private static byte[] httpHeaderEnd = Encoding.UTF8.GetBytes("\r\n\r\n");
        private static int kMaxHttpHeaderLength = 0x2000;
        private static int kMaxRedirectTimes = 3;
        private string locationUrl = string.Empty;
        private string md5 = string.Empty;
        private static byte[] redirectionField = Encoding.UTF8.GetBytes("Location");
        private int size = -1;
        private static byte[] successStatusLine1_1 = Encoding.UTF8.GetBytes("HTTP/1.1 206 Partial Content");
        private Action<int, Dictionary<string, object>> theAction;
        private string tmpFile = string.Empty;
        private string url = string.Empty;

        public DownloadHandler(string url, int size, string md5, string destFile, int redirectTimes, Action<int, Dictionary<string, object>> action)
        {
            this.url = url;
            this.size = size;
            this.md5 = md5;
            this.destFile = destFile;
            this.theAction = action;
            this.curRedirectTimes = redirectTimes;
            this.tmpFile = Pandora.Instance.GetTempPath() + "/" + Path.GetFileName(destFile);
        }

        public override int DetectPacketSize(byte[] receivedData, int dataLen)
        {
            if (this.detectedHeaderLength < 0)
            {
                for (int j = 0; j < ((dataLen - httpHeaderEnd.Length) + 1); j++)
                {
                    bool flag = true;
                    for (int k = 0; k < httpHeaderEnd.Length; k++)
                    {
                        if (receivedData[j + k] != httpHeaderEnd[k])
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        this.detectedHeaderLength = j + httpHeaderEnd.Length;
                        break;
                    }
                }
            }
            if (this.detectedHeaderLength < 0)
            {
                if (dataLen > kMaxHttpHeaderLength)
                {
                    Logger.ERROR(string.Empty);
                    return -1;
                }
                return 0;
            }
            bool flag2 = true;
            for (int i = 0; i < successStatusLine1_1.Length; i++)
            {
                if ((i >= dataLen) || (receivedData[i] != successStatusLine1_1[i]))
                {
                    flag2 = false;
                }
            }
            if (flag2)
            {
                if (this.detecedContentLength < 0)
                {
                    this.detecedContentLength = this.GetContentLength(receivedData, this.detectedHeaderLength);
                }
                if (this.detecedContentLength > 0)
                {
                    try
                    {
                        FileInfo info = new FileInfo(this.tmpFile);
                        int length = (int) info.Length;
                        int num5 = length - this.fileSizeBeforeDownload;
                        int num6 = dataLen - this.detectedHeaderLength;
                        if (num6 > num5)
                        {
                            FileStream stream = new FileStream(this.tmpFile, FileMode.Append, FileAccess.Write);
                            stream.Write(receivedData, this.detectedHeaderLength + num5, num6 - num5);
                            stream.Close();
                        }
                        if ((this.detectedHeaderLength + this.detecedContentLength) <= dataLen)
                        {
                            return (this.detectedHeaderLength + this.detecedContentLength);
                        }
                        return 0;
                    }
                    catch (Exception exception)
                    {
                        Logger.ERROR(exception.StackTrace);
                        return -2;
                    }
                }
                Logger.ERROR(string.Empty);
                return -2;
            }
            this.locationUrl = this.GetLocationUrl(receivedData, this.detectedHeaderLength);
            if (this.locationUrl.Length > 0)
            {
                return this.detectedHeaderLength;
            }
            Logger.ERROR(string.Empty);
            return -3;
        }

        private int GetContentLength(byte[] receivedData, int dataLen)
        {
            Logger.DEBUG(string.Empty);
            int num = -1;
            int index = -1;
            int num3 = -1;
            for (int i = 0; i < ((dataLen - contentLengthField.Length) + 1); i++)
            {
                bool flag = true;
                for (int k = 0; k < contentLengthField.Length; k++)
                {
                    if (receivedData[i + k] != contentLengthField[k])
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    num = i;
                    break;
                }
            }
            if (num < 0)
            {
                Logger.WARN(string.Empty);
                return -1;
            }
            for (int j = num + contentLengthField.Length; j < dataLen; j++)
            {
                if ((receivedData[j] >= 0x30) && (receivedData[j] <= 0x39))
                {
                    if (index < 0)
                    {
                        index = j;
                    }
                }
                else if ((index >= 0) && (num3 < 0))
                {
                    num3 = j;
                    break;
                }
            }
            if (((index < 0) || (num3 < 0)) || (index >= num3))
            {
                Logger.ERROR(string.Empty);
                return -1;
            }
            try
            {
                int num7 = Convert.ToInt32(Encoding.UTF8.GetString(receivedData, index, num3 - index));
                if (num7 > 0)
                {
                    return num7;
                }
                Logger.ERROR(string.Empty);
                return -1;
            }
            catch (Exception exception)
            {
                Logger.ERROR(exception.StackTrace);
                return -1;
            }
        }

        private string GetLocationUrl(byte[] receivedData, int dataLen)
        {
            Logger.DEBUG(string.Empty);
            int num = -1;
            int index = -1;
            int num3 = -1;
            for (int i = 0; i < ((dataLen - redirectionField.Length) + 1); i++)
            {
                bool flag = true;
                for (int k = 0; k < redirectionField.Length; k++)
                {
                    if (receivedData[i + k] != redirectionField[k])
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    num = i;
                    break;
                }
            }
            if (num < 0)
            {
                Logger.WARN(string.Empty);
                return string.Empty;
            }
            for (int j = num + redirectionField.Length; j < dataLen; j++)
            {
                if (((receivedData[j] != 0x3a) && (receivedData[j] != 0x20)) && (index < 0))
                {
                    index = j;
                }
                if ((index >= 0) && (receivedData[j] == 13))
                {
                    num3 = j;
                    break;
                }
            }
            if (((index < 0) || (num3 < 0)) || (index >= num3))
            {
                Logger.ERROR(string.Empty);
                return string.Empty;
            }
            string formatMsg = Encoding.UTF8.GetString(receivedData, index, num3 - index);
            Logger.DEBUG(formatMsg);
            return formatMsg;
        }

        public override void OnClose()
        {
            Logger.DEBUG(string.Empty);
            Message msg = new Message();
            msg.status = -1;
            msg.action = this.theAction;
            Pandora.Instance.GetNetLogic().EnqueueDrive(msg);
        }

        public override void OnConnected()
        {
            <OnConnected>c__AnonStorey77 storey = new <OnConnected>c__AnonStorey77();
            storey.<>f__this = this;
            Logger.DEBUG(string.Empty);
            storey.uniqueSocketId = base.GetUniqueSocketId();
            Action<int, Dictionary<string, object>> action = new Action<int, Dictionary<string, object>>(storey, (IntPtr) this.<>m__8A);
            Message msg = new Message();
            msg.status = 0;
            msg.action = action;
            Pandora.Instance.GetNetLogic().EnqueueDrive(msg);
        }

        public override void OnReceived(Packet thePacket)
        {
            Logger.DEBUG(string.Empty);
            if (this.locationUrl.Length > 0)
            {
                Logger.DEBUG(this.locationUrl);
                if (this.curRedirectTimes <= kMaxRedirectTimes)
                {
                    Logger.DEBUG(this.locationUrl);
                    Dictionary<string, object> dictionary = new Dictionary<string, object>();
                    dictionary["locationUrl"] = this.locationUrl;
                    Message msg = new Message();
                    msg.status = 100;
                    msg.action = this.theAction;
                    msg.content = dictionary;
                    Pandora.Instance.GetNetLogic().EnqueueDrive(msg);
                    this.theAction = null;
                }
            }
            else
            {
                try
                {
                    Logger.DEBUG(this.url);
                    FileInfo info = new FileInfo(this.tmpFile);
                    int length = (int) info.Length;
                    FileStream inputStream = new FileStream(this.tmpFile, FileMode.Open);
                    byte[] buffer = new MD5CryptoServiceProvider().ComputeHash(inputStream);
                    inputStream.Close();
                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < buffer.Length; i++)
                    {
                        builder.Append(buffer[i].ToString("X2"));
                    }
                    string str = builder.ToString();
                    string str2 = "pandora20151019";
                    string s = str2 + str;
                    byte[] bytes = Encoding.UTF8.GetBytes(s);
                    MemoryStream stream2 = new MemoryStream();
                    stream2.Seek(0L, SeekOrigin.Begin);
                    stream2.Write(bytes, 0, bytes.Length);
                    stream2.Seek(0L, SeekOrigin.Begin);
                    buffer = new MD5CryptoServiceProvider().ComputeHash(stream2);
                    stream2.Dispose();
                    builder.Remove(0, str.Length);
                    for (int j = 0; j < buffer.Length; j++)
                    {
                        builder.Append(buffer[j].ToString("X2"));
                    }
                    string str4 = builder.ToString();
                    if (((this.size == 0) || (this.size == length)) && ((this.md5 == string.Empty) || (this.md5 == str4)))
                    {
                        if (File.Exists(this.destFile))
                        {
                            File.Delete(this.destFile);
                        }
                        File.Move(this.tmpFile, this.destFile);
                        Message message2 = new Message();
                        message2.status = 0;
                        message2.action = this.theAction;
                        Pandora.Instance.GetNetLogic().EnqueueDrive(message2);
                        this.theAction = null;
                    }
                }
                catch (Exception exception)
                {
                    Logger.ERROR(exception.StackTrace);
                }
                try
                {
                    if (File.Exists(this.tmpFile))
                    {
                        File.Delete(this.tmpFile);
                    }
                }
                catch (Exception exception2)
                {
                    Logger.ERROR(exception2.StackTrace);
                }
            }
            long uniqueSocketId = base.GetUniqueSocketId();
            Pandora.Instance.GetNetLogic().Close(uniqueSocketId);
        }

        [CompilerGenerated]
        private sealed class <OnConnected>c__AnonStorey77
        {
            internal DownloadHandler <>f__this;
            internal long uniqueSocketId;

            internal void <>m__8A(int status, Dictionary<string, object> content)
            {
                try
                {
                    if (!File.Exists(this.<>f__this.tmpFile))
                    {
                        File.Create(this.<>f__this.tmpFile).Close();
                    }
                    FileInfo info = new FileInfo(this.<>f__this.tmpFile);
                    this.<>f__this.fileSizeBeforeDownload = (int) info.Length;
                    if ((this.<>f__this.size != 0) && (this.<>f__this.fileSizeBeforeDownload >= this.<>f__this.size))
                    {
                        File.Delete(this.<>f__this.tmpFile);
                        File.Create(this.<>f__this.tmpFile).Close();
                        this.<>f__this.fileSizeBeforeDownload = 0;
                    }
                    Uri uri = new Uri(this.<>f__this.url);
                    string[] textArray1 = new string[] { "GET ", uri.AbsolutePath, " HTTP/1.1\r\nHost:", uri.Host, "\r\nAccept:*/*\r\nUser-Agent:Pandora(", Pandora.Instance.GetSDKVersion(), ")\r\nRange:bytes=", this.<>f__this.fileSizeBeforeDownload.ToString(), "-\r\nConnection: keep-alive\r\n\r\n" };
                    string formatMsg = string.Concat(textArray1);
                    Logger.DEBUG(formatMsg);
                    byte[] bytes = Encoding.UTF8.GetBytes(formatMsg);
                    Pandora.Instance.GetNetLogic().SendPacket(this.uniqueSocketId, bytes);
                }
                catch (Exception exception)
                {
                    Pandora.Instance.GetNetLogic().Close(this.uniqueSocketId);
                    Logger.ERROR(exception.StackTrace);
                }
            }
        }
    }
}

