namespace com.tencent.pandora
{
    using com.tencent.pandora.MiniJSON;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class MidasUtil : MonoBehaviour
    {
        public static bool AndroidPay(string jsonParams)
        {
            try
            {
                <AndroidPay>c__AnonStorey78 storey = new <AndroidPay>c__AnonStorey78();
                storey.env = string.Empty;
                storey.offerId = string.Empty;
                storey.openId = string.Empty;
                storey.openKey = string.Empty;
                storey.sessionId = string.Empty;
                storey.sessionType = string.Empty;
                storey.zoneId = string.Empty;
                storey.pf = string.Empty;
                storey.pfKey = string.Empty;
                storey.goodsTokenUrl = string.Empty;
                Dictionary<string, object> dictionary = new Dictionary<string, object>();
                dictionary = Json.Deserialize(jsonParams) as Dictionary<string, object>;
                if (dictionary.ContainsKey("env"))
                {
                    storey.env = dictionary["env"] as string;
                }
                if (dictionary.ContainsKey("offerId"))
                {
                    storey.offerId = dictionary["offerId"] as string;
                }
                if (dictionary.ContainsKey("openId"))
                {
                    storey.openId = dictionary["openId"] as string;
                }
                if (dictionary.ContainsKey("openKey"))
                {
                    storey.openKey = dictionary["openKey"] as string;
                }
                if (dictionary.ContainsKey("sessionId"))
                {
                    storey.sessionId = dictionary["sessionId"] as string;
                }
                if (dictionary.ContainsKey("sessionType"))
                {
                    storey.sessionType = dictionary["sessionType"] as string;
                }
                if (dictionary.ContainsKey("zoneId"))
                {
                    storey.zoneId = dictionary["zoneId"] as string;
                }
                if (dictionary.ContainsKey("pf"))
                {
                    storey.pf = dictionary["pf"] as string;
                }
                if (dictionary.ContainsKey("pfKey"))
                {
                    storey.pfKey = dictionary["pfKey"] as string;
                }
                if (dictionary.ContainsKey("goodsTokenUrl"))
                {
                    storey.goodsTokenUrl = dictionary["goodsTokenUrl"] as string;
                }
                storey.midasWrap = new AndroidJavaClass("com.tencent.pandora.MidasWrap");
                storey.activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                object[] objArray1 = new object[] { new AndroidJavaRunnable(storey, (IntPtr) this.<>m__8B) };
                storey.activity.Call("runOnUiThread", objArray1);
            }
            catch (Exception exception)
            {
                Logger.ERROR(exception.Message);
                return false;
            }
            return true;
        }

        public void PayCallback(string jsonResult)
        {
            CSharpInterface.NotifyAndroidPayFinish(jsonResult);
        }

        private void Start()
        {
        }

        private void Update()
        {
        }

        [CompilerGenerated]
        private sealed class <AndroidPay>c__AnonStorey78
        {
            internal AndroidJavaObject activity;
            internal string env;
            internal string goodsTokenUrl;
            internal AndroidJavaClass midasWrap;
            internal string offerId;
            internal string openId;
            internal string openKey;
            internal string pf;
            internal string pfKey;
            internal string sessionId;
            internal string sessionType;
            internal string zoneId;

            internal void <>m__8B()
            {
                object[] objArray1 = new object[] { this.activity, this.env, this.offerId, this.openId, this.openKey, this.sessionId, this.sessionType, this.zoneId, this.pf, this.pfKey, this.goodsTokenUrl, "Pandora GameObject", "PayCallback" };
                this.midasWrap.CallStatic("launchPay", objArray1);
            }
        }
    }
}

