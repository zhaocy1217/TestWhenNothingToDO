namespace com.tencent.pandora
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class GetLuckyShopInfo : MonoBehaviour
    {
        private static DateTime dtLastUpdate = DateTime.Now;
        private static int iBuyRet = 0;
        private bool iCloseFlag;
        private int iGetActionListRet;
        private static GetLuckyShopInfo instance = null;
        public bool iReportOpen;
        public static bool isBuyFinish = false;
        public static bool isReGoodsFresh = false;
        private static int iTryTimes = 0;
        private static string strBuyMsg = string.Empty;
        private string strGetActionListMsg = string.Empty;

        public int buyGoodsEnd(int iRet, string strMsg)
        {
            Logger.d(string.Concat(new object[] { "buyGoodsFinish iRet:", iRet, ",Msg:", strMsg }));
            iBuyRet = iRet;
            strBuyMsg = strMsg;
            isBuyFinish = true;
            return 0;
        }

        public static void Clean()
        {
        }

        public void GetActionList()
        {
        }

        public static void GetActionListCache()
        {
            FileUtils.InitPath();
        }

        public int getGoodsListFinish(int iRet, string strMsg)
        {
            isReGoodsFresh = true;
            dtLastUpdate = DateTime.Now;
            this.iGetActionListRet = iRet;
            this.strGetActionListMsg = strMsg;
            return 0;
        }

        public static GetLuckyShopInfo GetInstance()
        {
            if (Pandora.stopConnectAll)
            {
                return null;
            }
            if (object.ReferenceEquals(instance, null))
            {
                GameObject obj2 = new GameObject("PandoraData");
                obj2.get_transform().set_parent(Pandora.GetInstance().get_gameObject().get_transform());
                instance = obj2.AddComponent<GetLuckyShopInfo>();
                return instance;
            }
            return instance;
        }

        public int Init()
        {
            FileUtils.InitPath();
            if (Configer.strCtrFlagTotalSwitch.Equals(string.Empty))
            {
                Logger.d("InitSocket");
                NetProxcy.GetInstance().InitSocket();
            }
            else if (Configer.strCtrFlagTotalSwitch.Equals("1"))
            {
                Logger.d("getActionList");
            }
            else
            {
                return -1;
            }
            return 0;
        }

        private void OnApplicationQuit()
        {
            Logger.d("GetLuckyShopInfo OnApplicationQuit");
            try
            {
                this.iCloseFlag = true;
            }
            catch (Exception exception)
            {
                Logger.d("GetLuckyShopInfo:" + exception.Message);
            }
            instance = null;
        }

        public void OnDestroy()
        {
            Logger.d("distroy GetLuckyShopInfo");
        }

        public void SendLogReport(int logLevel, int reportType, int toReturnCode, string logMsg)
        {
            NetProxcy.SendLogReport(logLevel, reportType, toReturnCode, logMsg);
        }

        private void SendStaticReport(int iModuleId, int iReportType, int iActionId, int iJumpType, string strJumpUrl, string strGoodsId, int iGoodsNum, int iGoodFee, int iMoneyType)
        {
            Logger.d(string.Concat(new object[] { "-------------iReportType:", iReportType, ",goodsId:", strGoodsId }));
            NetProxcy.StaticReport(iModuleId, 0, iActionId, iReportType, iJumpType, strJumpUrl, strGoodsId, iGoodsNum, iGoodFee, iMoneyType);
        }

        private void Start()
        {
        }

        private void Update()
        {
        }

        [DebuggerHidden]
        private IEnumerator WaitForCloseSocket()
        {
            return new <WaitForCloseSocket>c__Iterator1();
        }

        [CompilerGenerated]
        private sealed class <WaitForCloseSocket>c__Iterator1 : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal int <i>__0;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.<i>__0 = 0;
                        break;

                    case 1:
                        this.<i>__0++;
                        break;

                    default:
                        goto Label_0084;
                }
                if (this.<i>__0 < 12)
                {
                    this.$current = new WaitForSeconds(0.2f);
                    this.$PC = 1;
                    return true;
                }
                if (!Pandora.isPannelOpen)
                {
                    Logger.d("start to self clean in GetLuckyShop");
                    GetLuckyShopInfo.Clean();
                }
                this.$PC = -1;
            Label_0084:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }
    }
}

