namespace com.tencent.gsdk
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class GSDK
    {
        private static AndroidJavaClass sGSDKPlatformClass;

        private static  event GSDKObserver sSpeedNotifyEvent;

        public static void EndSpeed(string vip, int vport)
        {
            object[] objArray1 = new object[] { vip, vport };
            sGSDKPlatformClass.CallStatic("GSDKEndSpeed", objArray1);
        }

        public static SpeedInfo GetSpeedInfo(string vip, int vport)
        {
            object[] objArray1 = new object[] { vip, vport };
            AndroidJavaObject obj2 = sGSDKPlatformClass.CallStatic<AndroidJavaObject>("GSDKGetSpeedInfo", objArray1);
            SpeedInfo info = new SpeedInfo();
            info.state = obj2.Get<int>("state");
            info.netType = obj2.Get<int>("netType");
            info.delay = obj2.Get<int>("delay");
            return info;
        }

        public static void GoBack()
        {
            sGSDKPlatformClass.CallStatic("GSDKGoBack", new object[0]);
        }

        public static void GoFront()
        {
            sGSDKPlatformClass.CallStatic("GSDKGoFront", new object[0]);
        }

        public static void Init(string appid, bool debug)
        {
            GSDKUtils.Logger("android init");
            sGSDKPlatformClass = new AndroidJavaClass("com.tencent.gsdk.GSDKPlatform");
            object[] objArray1 = new object[] { appid, debug };
            sGSDKPlatformClass.CallStatic("GSDKInit", objArray1);
            object[] objArray2 = new object[] { GSDKAndroidObserver.Instance };
            sGSDKPlatformClass.CallStatic("GSDKSetObserver", objArray2);
        }

        internal static void notify(StartSpeedRet ret)
        {
            if (sSpeedNotifyEvent != null)
            {
                sSpeedNotifyEvent(ret);
            }
        }

        public static void SetObserver(GSDKObserver d)
        {
            if (d != null)
            {
                sSpeedNotifyEvent = (GSDKObserver) Delegate.Combine(sSpeedNotifyEvent, d);
            }
        }

        public static void SetUserName(int plat, string openid)
        {
            object[] objArray1 = new object[] { plat, openid };
            sGSDKPlatformClass.CallStatic("GSDKSetUserName", objArray1);
        }

        public static void StartSpeed(string vip, int vport, int htype, string hookModules, int zoneid, int reserved)
        {
            object[] objArray1 = new object[] { vip, vport, htype, hookModules, zoneid, reserved };
            sGSDKPlatformClass.CallStatic("GSDKStartSpeed", objArray1);
        }

        public delegate void GSDKObserver(StartSpeedRet ret);
    }
}

