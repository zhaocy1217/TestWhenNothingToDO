using Assets.Scripts.UI;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

public class NetworkAccelerator
{
    private static string key = "827006BE-64F7-4082-B252-33ACF328A3A5";
    private static int KEY_GET_ACCEL_EFFECT = 0x6b;
    private static int KEY_GET_ACCEL_STAT = 0x66;
    private static int KEY_GET_NETDELAY = 100;
    private static int KEY_LOG_LEVLE = 0x134;
    public static int LOG_LEVEL_DEBUG = 1;
    public static int LOG_LEVEL_ERROR = 4;
    public static int LOG_LEVEL_FATAL = 5;
    public static int LOG_LEVEL_INFO = 2;
    public static int LOG_LEVEL_WARNING = 3;
    public static string PLAYER_IS_USE_ACC = "PLAYER_IS_USE_ACC";
    public static string PLAYER_PREF_AUTO_NET_ACC = "AUTO_NET_ACC";
    public static string PLAYER_PREF_NET_ACC = "NET_ACC";
    private static bool s_enabled;
    private static bool s_enabledPrepare;
    private static bool s_inited;
    private static bool s_started;

    public static void ChangeLogLevel(int level)
    {
        if (s_inited)
        {
            long num = Mathf.Clamp(level, 1, 5);
            AndroidJavaClass class2 = new AndroidJavaClass("com.subao.gamemaster.GameMaster");
            if (class2 != null)
            {
                object[] objArray1 = new object[] { KEY_LOG_LEVLE, num };
                class2.CallStatic("setLong", objArray1);
            }
        }
    }

    public static void ClearUDPCache()
    {
        if (s_inited)
        {
            AndroidJavaClass class2 = new AndroidJavaClass("com.subao.gamemaster.GameMaster");
            if (class2 != null)
            {
                class2.CallStatic("clearUDPCache", new object[0]);
            }
        }
    }

    public static bool getAccelRecommendation()
    {
        if ((!s_inited || !enabled) || started)
        {
            return false;
        }
        AndroidJavaClass class2 = new AndroidJavaClass("com.subao.gamemaster.GameMaster");
        bool flag = false;
        if (class2 != null)
        {
            flag = class2.CallStatic<int>("getAccelRecommendation", new object[0]) == 1;
            Debug.Log("getAccelRecommendation :" + flag);
        }
        return flag;
    }

    public static long GetDelay()
    {
        if (!s_started)
        {
            return -1L;
        }
        AndroidJavaClass class2 = new AndroidJavaClass("com.subao.gamemaster.GameMaster");
        long num = -1L;
        if (class2 != null)
        {
            object[] objArray1 = new object[] { KEY_GET_NETDELAY };
            num = class2.CallStatic<long>("getLong", objArray1);
        }
        return num;
    }

    public static string GetEffect()
    {
        if (!s_started)
        {
            return null;
        }
        AndroidJavaClass class2 = new AndroidJavaClass("com.subao.gamemaster.GameMaster");
        string str = null;
        if (class2 != null)
        {
            object[] objArray1 = new object[] { KEY_GET_ACCEL_EFFECT };
            str = class2.CallStatic<string>("getString", objArray1);
        }
        return str;
    }

    public static int GetNetType()
    {
        int num = -1;
        if (s_inited)
        {
            AndroidJavaClass class2 = new AndroidJavaClass("com.subao.gamemaster.GameMaster");
            if (class2 != null)
            {
                num = class2.CallStatic<int>("getCurrentConnectionType", new object[0]);
            }
        }
        return num;
    }

    public static void Init(bool bforceUse = false)
    {
        <Init>c__AnonStorey3E storeye = new <Init>c__AnonStorey3E();
        Debug.Log("Begin Network Acc");
        Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.NetworkAccelerator_TurnOn, new CUIEventManager.OnUIEventHandler(NetworkAccelerator.OnEventTurnOn));
        Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.NetworkAccelerator_Ignore, new CUIEventManager.OnUIEventHandler(NetworkAccelerator.OnEventTurnIgore));
        s_enabledPrepare = true;
        if (!bforceUse)
        {
            if (!IsUseACC())
            {
                Debug.Log("NetAcc player close acc");
                return;
            }
        }
        else
        {
            SetUseACC(true);
        }
        s_enabled = true;
        Debug.Log("NetAcc key:" + key);
        Debug.Log("NetAcc enable & java begin");
        storeye.GMContext = null;
        using (AndroidJavaClass class2 = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            storeye.GMContext = class2.GetStatic<AndroidJavaObject>("currentActivity");
        }
        storeye.GMClass = new AndroidJavaClass("com.subao.gamemaster.GameMaster");
        if (storeye.GMClass != null)
        {
            object[] objArray1 = new object[] { new AndroidJavaRunnable(storeye, (IntPtr) this.<>m__B) };
            storeye.GMContext.Call("runOnUiThread", objArray1);
        }
    }

    public static bool isAccerating()
    {
        if (!s_started)
        {
            return false;
        }
        AndroidJavaClass class2 = new AndroidJavaClass("com.subao.gamemaster.GameMaster");
        bool flag = false;
        if (class2 != null)
        {
            flag = class2.CallStatic<bool>("isUDPProxy", new object[0]);
        }
        return flag;
    }

    public static bool IsAutoNetAccConfigOpen()
    {
        return (PlayerPrefs.GetInt(PLAYER_PREF_AUTO_NET_ACC, 0) > 0);
    }

    public static bool IsNetAccConfigOpen()
    {
        return (PlayerPrefs.GetInt(PLAYER_PREF_NET_ACC, 0) > 0);
    }

    public static bool IsUseACC()
    {
        return (PlayerPrefs.GetInt(PLAYER_IS_USE_ACC, 0) <= 0);
    }

    private static void OnEventTurnIgore(CUIEvent uiEvent)
    {
        Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("NetACCIgore", null, true);
    }

    private static void OnEventTurnOn(CUIEvent uiEvent)
    {
        SetNetAccConfig(true);
        Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("NetACCturnOK", null, true);
    }

    public static void OnNetDelay(int millis)
    {
        if (s_inited && !started)
        {
            AndroidJavaClass class2 = new AndroidJavaClass("com.subao.gamemaster.GameMaster");
            if (class2 != null)
            {
                object[] objArray1 = new object[] { millis };
                class2.CallStatic("onNetDelay", objArray1);
            }
        }
    }

    public static void SetAutoNetAccConfig(bool open)
    {
        PlayerPrefs.SetInt(PLAYER_PREF_AUTO_NET_ACC, !open ? 0 : 1);
        PlayerPrefs.Save();
    }

    public static void SetEchoPort(int port)
    {
        Debug.Log("Set UD Echo Port to :" + port);
        if (s_inited)
        {
            AndroidJavaClass class2 = new AndroidJavaClass("com.subao.gamemaster.GameMaster");
            if (class2 != null)
            {
                object[] objArray1 = new object[] { port };
                class2.CallStatic("setUdpEchoPort", objArray1);
            }
            Debug.Log("Set UD Echo Port Success!");
        }
    }

    public static void SetNetAccConfig(bool open)
    {
        if (open)
        {
            Start();
        }
        else
        {
            Stop();
        }
        PlayerPrefs.SetInt(PLAYER_PREF_NET_ACC, !open ? 0 : 1);
        PlayerPrefs.Save();
    }

    public static void setRecommendationGameIP(string ip, int port)
    {
        Debug.Log(string.Concat(new object[] { "setRecommendationGameIP :", ip, ", port :", port }));
        if (s_inited)
        {
            AndroidJavaClass class2 = new AndroidJavaClass("com.subao.gamemaster.GameMaster");
            if (class2 != null)
            {
                object[] objArray2 = new object[] { ip, port };
                class2.CallStatic("setRecommendationGameIP", objArray2);
            }
            Debug.Log("Set setRecommendationGameIP Success!");
        }
    }

    public static void SetUseACC(bool bUse)
    {
        PlayerPrefs.SetInt(PLAYER_IS_USE_ACC, !bUse ? 1 : 0);
        PlayerPrefs.Save();
    }

    private static bool Start()
    {
        if (s_inited)
        {
            if (!enabled)
            {
                return false;
            }
            if (s_started)
            {
                return s_started;
            }
            AndroidJavaClass class2 = new AndroidJavaClass("com.subao.gamemaster.GameMaster");
            bool flag = false;
            if (class2 != null)
            {
                object[] objArray1 = new object[] { 0 };
                flag = class2.CallStatic<bool>("start", objArray1);
            }
            if (flag)
            {
                Debug.Log("Start GameMaster Success!");
                s_started = true;
            }
            else
            {
                Debug.LogError("Start GameMaster Fail!");
            }
        }
        return s_started;
    }

    private static bool Stop()
    {
        if (s_inited)
        {
            if (!s_started)
            {
                return s_started;
            }
            AndroidJavaClass class2 = new AndroidJavaClass("com.subao.gamemaster.GameMaster");
            bool flag = false;
            if (class2 != null)
            {
                flag = class2.CallStatic<bool>("stop", new object[0]);
            }
            if (flag)
            {
                Debug.Log("Stop GameMaster Success!");
                ClearUDPCache();
                s_started = false;
            }
            else
            {
                Debug.LogError("Stop GameMaster Fail!");
            }
        }
        return s_started;
    }

    public static bool enabled
    {
        get
        {
            return s_enabled;
        }
    }

    public static bool EnableForGSDK
    {
        get
        {
            return s_enabled;
        }
    }

    public static bool SettingUIEnbaled
    {
        get
        {
            return s_enabledPrepare;
        }
    }

    public static bool started
    {
        get
        {
            return s_started;
        }
    }

    [CompilerGenerated]
    private sealed class <Init>c__AnonStorey3E
    {
        internal AndroidJavaClass GMClass;
        internal AndroidJavaObject GMContext;

        internal void <>m__B()
        {
            object[] objArray1 = new object[] { this.GMContext, 1, NetworkAccelerator.key, "KingsGlory", "libapollo.so", 0x32c9 };
            int num = this.GMClass.CallStatic<int>("init", objArray1);
            if (num >= 0)
            {
                Debug.Log("Initialize GameMaster Success!");
                NetworkAccelerator.s_inited = true;
                NetworkAccelerator.ChangeLogLevel(NetworkAccelerator.LOG_LEVEL_ERROR);
            }
            else
            {
                Debug.LogError("Initialize GameMaster Fail!, ret:" + num);
            }
        }
    }
}

