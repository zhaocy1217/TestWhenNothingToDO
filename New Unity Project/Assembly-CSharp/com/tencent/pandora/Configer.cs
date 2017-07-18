namespace com.tencent.pandora
{
    using System;
    using System.Net;

    internal class Configer
    {
        public static string m_CurHotUpdatePath = string.Empty;
        public static int STATIC_BUY_SUCC = 14;
        public static int STATIC_CLICK_BUY = 12;
        public static int STATIC_CLOSE = 5;
        public static int STATIC_DETAIL = 7;
        public static int STATIC_GOODS_SHOW = 15;
        public static int STATIC_NO_MONEY = 10;
        public static int STATIC_OPEN = 4;
        public static int STATIC_SHOW = 1;
        public static int STATIC_SURE_BUY = 13;
        public static int STATIC_TO_ADD_MONEY = 11;
        public static string strCtrFlagTotalSwitch = string.Empty;
        public static string strIP = string.Empty;
        public static string strSDKVer = "6L_HP_V2";
        public static string strSendLogFlag = string.Empty;

        public static string GetIP()
        {
            try
            {
                if (strIP != string.Empty)
                {
                    return strIP;
                }
                IPAddress[] hostAddresses = Dns.GetHostAddresses(Dns.GetHostName());
                if (hostAddresses.Length > 0)
                {
                    strIP = hostAddresses[0].ToString();
                    return strIP;
                }
                return string.Empty;
            }
            catch (Exception exception)
            {
                Logger.e(exception.Message);
                return "-";
            }
        }

        public enum ErrorCode
        {
            ACTION = 2,
            BROKER_SUCC = 0,
            ER_ATM_CAP = 0x3f0,
            ER_ATM_CONNECT = 0x3ef,
            ER_BROKER_CAP = 0x3ed,
            ER_BROKER_CONNECT = 0x3ea,
            ER_CLOUD_CAP = 0x3ee,
            ER_CLOUD_CONNECT = 0x3eb,
            ER_CLOUD_RETURN = 0x3f1,
            ER_CODE = 1,
            ER_DNS_ERROR = 0x3f2,
            ER_DOWNLOAD_ERROR = 0x3e9,
            ER_SHOW_PANEL = 0x3ec
        }
    }
}

