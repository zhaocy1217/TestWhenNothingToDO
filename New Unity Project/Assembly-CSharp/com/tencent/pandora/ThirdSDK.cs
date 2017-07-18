namespace com.tencent.pandora
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class ThirdSDK
    {
        private static ThirdSDK instance;

        public void buyCallBack(string a, int i, int j, string obj)
        {
            Logger.d("buy call back ~~:" + a);
            Logger.d("get action list i~~:" + i);
            Logger.d("get action list j~~:" + j);
            string openid = string.Empty;
            string offerId = string.Empty;
            string pf = string.Empty;
            string goodsTokenUrl = string.Empty;
            string payToken = string.Empty;
            string str6 = string.Empty;
            string acctype = string.Empty;
            Dictionary<string, object> dictionary = Json.Deserialize(a) as Dictionary<string, object>;
            string json = string.Empty;
            if (dictionary.ContainsKey("resp"))
            {
                Logger.d("resp");
                Logger.d(dictionary["resp"].ToString());
                if (json != null)
                {
                    json = dictionary["resp"].ToString();
                    Logger.d("set value " + json);
                }
                else
                {
                    Logger.d("resp is null ");
                }
            }
            Dictionary<string, object> dictionary2 = Json.Deserialize(json) as Dictionary<string, object>;
            int num = Convert.ToInt32(dictionary2["ret"]);
            Logger.d("iRet is " + num);
            string str9 = dictionary2["msg"] as string;
            if (dictionary2.ContainsKey("offerId"))
            {
                Logger.d("offerId");
                Logger.d(dictionary2["offerId"].ToString());
                if (offerId != null)
                {
                    offerId = dictionary2["offerId"].ToString();
                    Logger.d("set value " + offerId);
                }
                else
                {
                    Logger.d("offerId is null ");
                }
            }
            if (dictionary2.ContainsKey("pf"))
            {
                Logger.d("pf");
                Logger.d(dictionary2["pf"].ToString());
                if (pf != null)
                {
                    pf = dictionary2["pf"].ToString();
                    Logger.d("pf value " + pf);
                }
                else
                {
                    Logger.d("pf is null ");
                }
            }
            if (dictionary2.ContainsKey("urlParams"))
            {
                Logger.d("goodsTokenUrl");
                Logger.d(dictionary2["urlParams"].ToString());
                if (goodsTokenUrl != null)
                {
                    goodsTokenUrl = dictionary2["urlParams"].ToString();
                    Logger.d("goodsTokenUrl value " + goodsTokenUrl);
                }
                else
                {
                    Logger.d("goodsTokenUrl is null ");
                }
            }
            if (dictionary2.ContainsKey("serial"))
            {
                Logger.d("serial");
                Logger.d(dictionary2["serial"].ToString());
                if (str6 != null)
                {
                    Logger.d("serial value " + dictionary2["serial"].ToString());
                }
                else
                {
                    Logger.d("serial is null ");
                }
            }
            openid = CUserData.user.strOpenId;
            acctype = CUserData.user.acctype;
            if ("wx".Equals(acctype))
            {
                payToken = CUserData.user.sAccessToken;
            }
            else
            {
                payToken = CUserData.user.pay_token;
                if (NetProxcy.tokenkey != string.Empty)
                {
                    payToken = NetProxcy.tokenkey;
                }
            }
            Logger.d("XXXXXXXX midas pay:offerid=" + offerId + ",pf=" + pf + ",goodsTokenUrl=" + goodsTokenUrl + ",acctype=" + acctype + ",openkey=" + payToken);
            bool bTest = false;
            if ("0".Equals(NetProxcy.mds_op))
            {
                bTest = true;
            }
            this.midasPay(bTest, offerId, pf, goodsTokenUrl, acctype, payToken, "1", "pfKey", openid, obj, "midaspayCallBack");
            if (num == 0)
            {
                NetProxcy.GetActionList(string.Empty);
            }
        }

        public void BuyGoods(string strGoodsId, string iActionId, string payType, int iNum)
        {
            try
            {
                int iFlag = 8;
                Logger.d("start to buy goods:" + strGoodsId);
                int iCostType = 2;
                string sData = new CReqInfoDataBuilder().getGPMBuyPayReqJson(iActionId, iCostType, payType, strGoodsId, iNum);
                Logger.d("GPM PAY:" + sData);
                Logger.d("strJson.Length:" + sData.Length);
                Logger.d("get GpmPay return:" + NetProxcy.GpmPay(sData, sData.Length, iFlag));
            }
            catch (Exception exception)
            {
                Logger.e("error2:" + exception.Message);
                Logger.e(exception.StackTrace);
            }
        }

        public static ThirdSDK GetInstance()
        {
            if (instance == null)
            {
                instance = new ThirdSDK();
            }
            return instance;
        }

        public void midasPay(bool bTest, string offerId, string pf, string goodsTokenUrl, string accType, string payToken, string zoneId, string pfKey, string openid, string obj, string method)
        {
            using (AndroidJavaClass class2 = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (AndroidJavaObject obj2 = class2.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    Logger.d("serali33" + obj2);
                    AndroidJavaObject @static = class2.GetStatic<AndroidJavaObject>("currentActivity");
                    Logger.d("serali44");
                    AndroidJavaClass class3 = new AndroidJavaClass("com.tencent.pandora.pay.PandoraPay");
                    Logger.d("serali55");
                    object[] objArray1 = new object[] { @static, bTest, offerId, pf, goodsTokenUrl, accType, payToken, zoneId, pfKey, openid, obj, method };
                    class3.CallStatic("midasPay", objArray1);
                    Logger.d("serali44");
                }
            }
        }
    }
}

