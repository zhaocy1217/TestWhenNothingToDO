namespace com.tencent.gsdk
{
    using System;
    using UnityEngine;

    internal class GSDKUtils
    {
        private static bool isDebug = true;

        public static void Logger(string s)
        {
            if (isDebug)
            {
                MonoBehaviour.print("GSDKTag " + s);
            }
        }
    }
}

