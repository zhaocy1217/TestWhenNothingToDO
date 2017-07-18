namespace com.tencent.gsdk
{
    using System;
    using UnityEngine;

    internal class GSDKAndroidObserver : AndroidJavaProxy
    {
        private static GSDKAndroidObserver instance = new GSDKAndroidObserver();

        private GSDKAndroidObserver() : base("com.tencent.gsdk.GSDKObserver")
        {
        }

        public void OnStartSpeedNotify(int type, int flag, string desc)
        {
            StartSpeedRet ret = new StartSpeedRet(type, flag, desc);
            GSDK.notify(ret);
        }

        public static GSDKAndroidObserver Instance
        {
            get
            {
                return instance;
            }
        }
    }
}

