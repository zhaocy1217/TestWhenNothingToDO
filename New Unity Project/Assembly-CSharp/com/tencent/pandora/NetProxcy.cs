namespace com.tencent.pandora
{
    using AOT;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public class NetProxcy
    {
        public static bool haveSOData = false;
        public static NetProxcy instance = null;
        private static bool isInitLua = false;
        public static Queue listInfoSO = new Queue();
        private static string md5val = string.Empty;
        public static string mds_op = string.Empty;
        private static string strActionListFile = "action_json.dat";
        public static string tokenkey = string.Empty;

        [DllImport("pandora")]
        private static extern void AttendAct(string sData, int iLength, int iFlag);
        [MonoPInvokeCallback(typeof(CallBack))]
        public static void DoCallBack(string jsonFromSO, int jsonLength, int iFlag)
        {
            if (iFlag == 1)
            {
                Logger.d("in Connect:" + jsonFromSO);
                listInfoSO.Enqueue(new infoFromSo(jsonFromSO, jsonLength, iFlag));
                PraseConnect(jsonFromSO, jsonLength, iFlag);
            }
            else
            {
                listInfoSO.Enqueue(new infoFromSo(jsonFromSO, jsonLength, iFlag));
                Logger.d("iFlag: " + iFlag.ToString() + ", json from so enqueue:" + jsonFromSO);
            }
        }

        public static void GetActionList(string jsonExtend = "")
        {
            try
            {
                Logger.d("start to get actlist");
                string msg = new CReqInfoDataBuilder().getActionListReqJson(md5val, jsonExtend);
                Logger.d(msg);
                Logger.d("strJson.Length:" + msg.Length);
                Logger.d("get act list return:" + GetActList(msg, msg.Length, 3));
            }
            catch (Exception exception)
            {
                Logger.e("error:" + exception.Message);
                Logger.e(exception.StackTrace);
            }
        }

        public static void GetActionListCache()
        {
            Logger.d("获取活动列表信息");
            try
            {
                string strData = string.Empty;
                ArrayList list = FileUtils.LoadFile(CUserData.user.strOpenId + "_" + strActionListFile);
                if ((list != null) && (list.Count > 0))
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        strData = strData + list[i].ToString();
                    }
                }
                Logger.d("getContent:" + strData);
                if (strData != string.Empty)
                {
                    parseActionData(strData, false);
                }
            }
            catch (Exception exception)
            {
                Logger.e("get cache error:" + exception.Message);
            }
        }

        [DllImport("pandora")]
        private static extern int GetActList(string sData, int iLength, int iFlag);
        [DllImport("pandora", CallingConvention=CallingConvention.Cdecl)]
        public static extern int GetCallBackData(CallBack callFunConnect);
        public static NetProxcy GetInstance()
        {
            if (instance == null)
            {
                instance = new NetProxcy();
            }
            return instance;
        }

        [DllImport("pandora")]
        public static extern int GpmPay(string sData, int iLength, int iFlag);
        [DllImport("pandora")]
        private static extern void InitPushDataCallback(int iFlag, CallBack callFun);
        public void InitSocket()
        {
            U3dCloseSocket();
            string sData = new CReqInfoDataBuilder().getInitSocketReqJson(md5val);
            Logger.d("in InitSocket......：" + sData);
            InitTcpSocket(1, sData, sData.Length, 2, 5);
        }

        [DllImport("pandora", CallingConvention=CallingConvention.Cdecl)]
        public static extern int InitTcpSocket(int iFlag, string sData, int iLength, int iLoginFlag, int iGetListFlag);
        public void midaspayCallBack(string callback)
        {
            Logger.d("XXXXXXXXXX midaspayCallBack:" + callback);
            listInfoSO.Enqueue(new infoFromSo(callback, callback.Length, 9));
        }

        public static int parseActionData(string strData, bool blNeedCache)
        {
            try
            {
                Dictionary<string, object> dictionary = Json.Deserialize(strData) as Dictionary<string, object>;
                if (dictionary == null)
                {
                    return -1;
                }
                Logger.d("flag 1");
                if (dictionary.ContainsKey("md5val"))
                {
                    Logger.d("set value");
                    Logger.d(dictionary["md5val"].ToString());
                    if (md5val != null)
                    {
                        md5val = dictionary["md5val"].ToString();
                        Logger.d("set value ");
                    }
                    else
                    {
                        Logger.d("md5 is null ");
                    }
                }
                return 0;
            }
            catch (Exception exception)
            {
                Logger.d("parse error:" + exception.Message);
                Logger.e(exception.StackTrace);
                return -1;
            }
        }

        public static int PraseConnect(string strContent, int iLen, int iFlag)
        {
            try
            {
                Dictionary<string, object> dictionary = Json.Deserialize(strContent) as Dictionary<string, object>;
                if (dictionary.ContainsKey("iPdrLibRet"))
                {
                    int num = Convert.ToInt32(dictionary["iPdrLibRet"] as string);
                    int num2 = -1;
                    if (dictionary.ContainsKey("ret"))
                    {
                        num2 = Convert.ToInt32(dictionary["ret"] as string);
                    }
                    if (num == 0)
                    {
                        if (dictionary.ContainsKey("totalSwitch"))
                        {
                            string str = dictionary["totalSwitch"] as string;
                            if (((dictionary["ret"].ToString() != "0") && (dictionary["ret"].ToString() != "1")) && (dictionary["ret"].ToString() != "2"))
                            {
                                Logger.LogNetError(0x3f1, "cgi ret return exception:" + dictionary["ret"].ToString());
                            }
                            else
                            {
                                Logger.d("cgi ret return: " + dictionary["ret"].ToString());
                            }
                            if (dictionary.ContainsKey("curr_lua_dir"))
                            {
                                Configer.m_CurHotUpdatePath = dictionary["curr_lua_dir"].ToString();
                                if (Configer.m_CurHotUpdatePath != string.Empty)
                                {
                                    Configer.m_CurHotUpdatePath = Configer.m_CurHotUpdatePath + "/res/";
                                }
                                Logger.d("m_CurHotUpdatePath:" + Configer.m_CurHotUpdatePath);
                            }
                            else
                            {
                                Logger.d("current has not curr_lua_dir");
                            }
                            if (dictionary.ContainsKey("lua_dir"))
                            {
                            }
                            if (!isInitLua && (str == "1"))
                            {
                                isInitLua = true;
                                Logger.d("init Lua ");
                                Pandora.GetInstance().InitGameManager();
                                Logger.d("init Lua end");
                            }
                            Configer.strCtrFlagTotalSwitch = dictionary["totalSwitch"] as string;
                            if (dictionary.ContainsKey("fakeLoginInfo"))
                            {
                                string str2 = dictionary["fakeLoginInfo"] as string;
                                tokenkey = str2;
                            }
                            if (dictionary.ContainsKey("isNetLog"))
                            {
                                string str3 = dictionary["isNetLog"] as string;
                                if (str3 == "1")
                                {
                                    Configer.strSendLogFlag = "1";
                                }
                                else
                                {
                                    Configer.strSendLogFlag = "0";
                                }
                            }
                            if (dictionary.ContainsKey("isDebug"))
                            {
                                string str4 = dictionary["isDebug"] as string;
                                if (str4.Equals("1"))
                                {
                                    Logger.LOG_LEVEL = 6;
                                }
                                else
                                {
                                    Logger.LOG_LEVEL = 0;
                                }
                            }
                            if (dictionary.ContainsKey("mds_op"))
                            {
                                mds_op = dictionary["mds_op"] as string;
                            }
                        }
                        else
                        {
                            Logger.d("totalSwitch no data");
                        }
                    }
                    else
                    {
                        Logger.d("connect iRet:" + num.ToString());
                    }
                }
                else
                {
                    Logger.d("no para iPdrLibRet");
                }
            }
            catch (Exception exception)
            {
                Logger.e("connect fail,retry:" + exception.ToString());
            }
            return 0;
        }

        [DllImport("pandora")]
        private static extern int SendLogin(string sData, int iLength, int iFlag);
        public static void SendLogReport(int logLevel, int reportType, int toReturnCode, string logMsg)
        {
        }

        public static void SendPandoraLibCmd(int iCmdId, string sData, int iLength, int iFlag)
        {
            Logger.d("enter C# SendPandoraLibCmd");
            SendPdrLibCmd(iCmdId, sData, iLength, iFlag);
        }

        [DllImport("pandora", CallingConvention=CallingConvention.Cdecl)]
        public static extern int SendPdrLibCmd(int iCmdId, string sData, int iLength, int iFlag);
        [DllImport("pandora")]
        private static extern int SendSDKLogReport(string sData, int iLenght, int iFlag);
        [DllImport("pandora")]
        private static extern int StaticReport(string sData, int iLength, int iFlag);
        public static void StaticReport(int iModuleId, int iChannelId, int iActionId, int iReportType, int iJumpType, string strJumpUrl, string strGoodsId, int iGoodsNum, int iGoodFee, int iMoneyType)
        {
            try
            {
                string sData = new CReqInfoDataBuilder().staticReportReqJson(iModuleId, iChannelId, iActionId, iReportType, iJumpType, strJumpUrl, strGoodsId, iGoodsNum, iGoodFee, iMoneyType);
                Logger.d("StaticReport:" + sData);
                Logger.d("strJson.Length:" + sData.Length);
                Logger.d("get StaticReport return:" + StaticReport(sData, sData.Length, 6));
            }
            catch (Exception exception)
            {
                Logger.d("in exeption");
                Logger.e("error2:" + exception.Message);
                Logger.e(exception.StackTrace);
            }
        }

        [DllImport("pandora")]
        public static extern int U3dCloseSocket();

        public delegate void CallBack([MarshalAs(UnmanagedType.LPStr)] string a, int i, int j);

        private enum CallBackFuncType
        {
            ACTIONLIST = 3,
            ATTEND = 5,
            CONNECT = 1,
            FirstGet = 10,
            GPM_PAY = 8,
            LOG = 7,
            LOGIN = 2,
            PUSH = 4,
            STATIC = 6
        }

        public class infoFromSo
        {
            public int iFlagSO;
            public string jsonFromSO = string.Empty;
            public int jsonFromSOLength;

            public infoFromSo(string json, int jsonLenth, int reqFlag)
            {
                this.jsonFromSO = json;
                this.jsonFromSOLength = jsonLenth;
                this.iFlagSO = reqFlag;
            }
        }
    }
}

