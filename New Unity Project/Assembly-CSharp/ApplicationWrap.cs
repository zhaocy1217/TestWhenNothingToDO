using com.tencent.pandora;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ApplicationWrap
{
    private static Type classType = typeof(Application);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _CreateApplication(IntPtr L)
    {
        if (LuaDLL.lua_gettop(L) == 0)
        {
            Application o = new Application();
            LuaScriptMgr.PushObject(L, o);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Application.New");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int CancelQuit(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 0);
        Application.CancelQuit();
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int CanStreamedLevelBeLoaded(IntPtr L)
    {
        int num = LuaDLL.lua_gettop(L);
        if ((num == 1) && LuaScriptMgr.CheckTypes(L, 1, typeof(string)))
        {
            bool b = Application.CanStreamedLevelBeLoaded(LuaScriptMgr.GetString(L, 1));
            LuaScriptMgr.Push(L, b);
            return 1;
        }
        if ((num == 1) && LuaScriptMgr.CheckTypes(L, 1, typeof(int)))
        {
            int num2 = (int) LuaDLL.lua_tonumber(L, 1);
            bool flag2 = Application.CanStreamedLevelBeLoaded(num2);
            LuaScriptMgr.Push(L, flag2);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Application.CanStreamedLevelBeLoaded");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int CaptureScreenshot(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 1:
                Application.CaptureScreenshot(LuaScriptMgr.GetLuaString(L, 1));
                return 0;

            case 2:
            {
                string luaString = LuaScriptMgr.GetLuaString(L, 1);
                int number = (int) LuaScriptMgr.GetNumber(L, 2);
                Application.CaptureScreenshot(luaString, number);
                return 0;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Application.CaptureScreenshot");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int ExternalCall(IntPtr L)
    {
        int num = LuaDLL.lua_gettop(L);
        string luaString = LuaScriptMgr.GetLuaString(L, 1);
        object[] objArray = LuaScriptMgr.GetParamsObject(L, 2, num - 1);
        Application.ExternalCall(luaString, objArray);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_absoluteURL(IntPtr L)
    {
        LuaScriptMgr.Push(L, Application.get_absoluteURL());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_backgroundLoadingPriority(IntPtr L)
    {
        LuaScriptMgr.Push(L, (Enum) Application.get_backgroundLoadingPriority());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_dataPath(IntPtr L)
    {
        LuaScriptMgr.Push(L, Application.get_dataPath());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_genuine(IntPtr L)
    {
        LuaScriptMgr.Push(L, Application.get_genuine());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_genuineCheckAvailable(IntPtr L)
    {
        LuaScriptMgr.Push(L, Application.get_genuineCheckAvailable());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_internetReachability(IntPtr L)
    {
        LuaScriptMgr.Push(L, (Enum) Application.get_internetReachability());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_isConsolePlatform(IntPtr L)
    {
        LuaScriptMgr.Push(L, Application.get_isConsolePlatform());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_isEditor(IntPtr L)
    {
        LuaScriptMgr.Push(L, Application.get_isEditor());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_isLoadingLevel(IntPtr L)
    {
        LuaScriptMgr.Push(L, Application.get_isLoadingLevel());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_isMobilePlatform(IntPtr L)
    {
        LuaScriptMgr.Push(L, Application.get_isMobilePlatform());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_isPlaying(IntPtr L)
    {
        LuaScriptMgr.Push(L, Application.get_isPlaying());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_isWebPlayer(IntPtr L)
    {
        LuaScriptMgr.Push(L, Application.get_isWebPlayer());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_levelCount(IntPtr L)
    {
        LuaScriptMgr.Push(L, Application.get_levelCount());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_loadedLevel(IntPtr L)
    {
        LuaScriptMgr.Push(L, Application.get_loadedLevel());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_loadedLevelName(IntPtr L)
    {
        LuaScriptMgr.Push(L, Application.get_loadedLevelName());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_persistentDataPath(IntPtr L)
    {
        LuaScriptMgr.Push(L, Application.get_persistentDataPath());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_platform(IntPtr L)
    {
        LuaScriptMgr.Push(L, (Enum) Application.get_platform());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_runInBackground(IntPtr L)
    {
        LuaScriptMgr.Push(L, Application.get_runInBackground());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_srcValue(IntPtr L)
    {
        LuaScriptMgr.Push(L, Application.get_srcValue());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_streamedBytes(IntPtr L)
    {
        LuaScriptMgr.Push(L, Application.get_streamedBytes());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_streamingAssetsPath(IntPtr L)
    {
        LuaScriptMgr.Push(L, Application.get_streamingAssetsPath());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_systemLanguage(IntPtr L)
    {
        LuaScriptMgr.Push(L, (Enum) Application.get_systemLanguage());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_targetFrameRate(IntPtr L)
    {
        LuaScriptMgr.Push(L, Application.get_targetFrameRate());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_temporaryCachePath(IntPtr L)
    {
        LuaScriptMgr.Push(L, Application.get_temporaryCachePath());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_unityVersion(IntPtr L)
    {
        LuaScriptMgr.Push(L, Application.get_unityVersion());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_webSecurityEnabled(IntPtr L)
    {
        LuaScriptMgr.Push(L, Application.get_webSecurityEnabled());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_webSecurityHostUrl(IntPtr L)
    {
        LuaScriptMgr.Push(L, Application.get_webSecurityHostUrl());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetClassType(IntPtr L)
    {
        LuaScriptMgr.Push(L, classType);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetStreamProgressForLevel(IntPtr L)
    {
        int num = LuaDLL.lua_gettop(L);
        if ((num == 1) && LuaScriptMgr.CheckTypes(L, 1, typeof(string)))
        {
            float streamProgressForLevel = Application.GetStreamProgressForLevel(LuaScriptMgr.GetString(L, 1));
            LuaScriptMgr.Push(L, streamProgressForLevel);
            return 1;
        }
        if ((num == 1) && LuaScriptMgr.CheckTypes(L, 1, typeof(int)))
        {
            int num3 = (int) LuaDLL.lua_tonumber(L, 1);
            float d = Application.GetStreamProgressForLevel(num3);
            LuaScriptMgr.Push(L, d);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Application.GetStreamProgressForLevel");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int HasProLicense(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 0);
        bool b = Application.HasProLicense();
        LuaScriptMgr.Push(L, b);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int HasUserAuthorization(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        UserAuthorization authorization = (UserAuthorization) ((int) LuaScriptMgr.GetNetObject(L, 1, typeof(UserAuthorization)));
        bool b = Application.HasUserAuthorization(authorization);
        LuaScriptMgr.Push(L, b);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int LoadLevel(IntPtr L)
    {
        int num = LuaDLL.lua_gettop(L);
        if ((num == 1) && LuaScriptMgr.CheckTypes(L, 1, typeof(string)))
        {
            Application.LoadLevel(LuaScriptMgr.GetString(L, 1));
            return 0;
        }
        if ((num == 1) && LuaScriptMgr.CheckTypes(L, 1, typeof(int)))
        {
            int num2 = (int) LuaDLL.lua_tonumber(L, 1);
            Application.LoadLevel(num2);
            return 0;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Application.LoadLevel");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int LoadLevelAdditive(IntPtr L)
    {
        int num = LuaDLL.lua_gettop(L);
        if ((num == 1) && LuaScriptMgr.CheckTypes(L, 1, typeof(string)))
        {
            Application.LoadLevelAdditive(LuaScriptMgr.GetString(L, 1));
            return 0;
        }
        if ((num == 1) && LuaScriptMgr.CheckTypes(L, 1, typeof(int)))
        {
            int num2 = (int) LuaDLL.lua_tonumber(L, 1);
            Application.LoadLevelAdditive(num2);
            return 0;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Application.LoadLevelAdditive");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int LoadLevelAdditiveAsync(IntPtr L)
    {
        int num = LuaDLL.lua_gettop(L);
        if ((num == 1) && LuaScriptMgr.CheckTypes(L, 1, typeof(string)))
        {
            AsyncOperation o = Application.LoadLevelAdditiveAsync(LuaScriptMgr.GetString(L, 1));
            LuaScriptMgr.PushObject(L, o);
            return 1;
        }
        if ((num == 1) && LuaScriptMgr.CheckTypes(L, 1, typeof(int)))
        {
            int num2 = (int) LuaDLL.lua_tonumber(L, 1);
            AsyncOperation operation2 = Application.LoadLevelAdditiveAsync(num2);
            LuaScriptMgr.PushObject(L, operation2);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Application.LoadLevelAdditiveAsync");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int LoadLevelAsync(IntPtr L)
    {
        int num = LuaDLL.lua_gettop(L);
        if ((num == 1) && LuaScriptMgr.CheckTypes(L, 1, typeof(string)))
        {
            AsyncOperation o = Application.LoadLevelAsync(LuaScriptMgr.GetString(L, 1));
            LuaScriptMgr.PushObject(L, o);
            return 1;
        }
        if ((num == 1) && LuaScriptMgr.CheckTypes(L, 1, typeof(int)))
        {
            int num2 = (int) LuaDLL.lua_tonumber(L, 1);
            AsyncOperation operation2 = Application.LoadLevelAsync(num2);
            LuaScriptMgr.PushObject(L, operation2);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Application.LoadLevelAsync");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int OpenURL(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        Application.OpenURL(LuaScriptMgr.GetLuaString(L, 1));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Quit(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 0);
        Application.Quit();
        return 0;
    }

    public static void Register(IntPtr L)
    {
        LuaMethod[] regs = new LuaMethod[] { 
            new LuaMethod("Quit", new LuaCSFunction(ApplicationWrap.Quit)), new LuaMethod("CancelQuit", new LuaCSFunction(ApplicationWrap.CancelQuit)), new LuaMethod("LoadLevel", new LuaCSFunction(ApplicationWrap.LoadLevel)), new LuaMethod("LoadLevelAsync", new LuaCSFunction(ApplicationWrap.LoadLevelAsync)), new LuaMethod("LoadLevelAdditiveAsync", new LuaCSFunction(ApplicationWrap.LoadLevelAdditiveAsync)), new LuaMethod("LoadLevelAdditive", new LuaCSFunction(ApplicationWrap.LoadLevelAdditive)), new LuaMethod("GetStreamProgressForLevel", new LuaCSFunction(ApplicationWrap.GetStreamProgressForLevel)), new LuaMethod("CanStreamedLevelBeLoaded", new LuaCSFunction(ApplicationWrap.CanStreamedLevelBeLoaded)), new LuaMethod("CaptureScreenshot", new LuaCSFunction(ApplicationWrap.CaptureScreenshot)), new LuaMethod("HasProLicense", new LuaCSFunction(ApplicationWrap.HasProLicense)), new LuaMethod("ExternalCall", new LuaCSFunction(ApplicationWrap.ExternalCall)), new LuaMethod("OpenURL", new LuaCSFunction(ApplicationWrap.OpenURL)), new LuaMethod("RegisterLogCallback", new LuaCSFunction(ApplicationWrap.RegisterLogCallback)), new LuaMethod("RegisterLogCallbackThreaded", new LuaCSFunction(ApplicationWrap.RegisterLogCallbackThreaded)), new LuaMethod("RequestUserAuthorization", new LuaCSFunction(ApplicationWrap.RequestUserAuthorization)), new LuaMethod("HasUserAuthorization", new LuaCSFunction(ApplicationWrap.HasUserAuthorization)), 
            new LuaMethod("New", new LuaCSFunction(ApplicationWrap._CreateApplication)), new LuaMethod("GetClassType", new LuaCSFunction(ApplicationWrap.GetClassType))
         };
        LuaField[] fields = new LuaField[] { 
            new LuaField("loadedLevel", new LuaCSFunction(ApplicationWrap.get_loadedLevel), null), new LuaField("loadedLevelName", new LuaCSFunction(ApplicationWrap.get_loadedLevelName), null), new LuaField("isLoadingLevel", new LuaCSFunction(ApplicationWrap.get_isLoadingLevel), null), new LuaField("levelCount", new LuaCSFunction(ApplicationWrap.get_levelCount), null), new LuaField("streamedBytes", new LuaCSFunction(ApplicationWrap.get_streamedBytes), null), new LuaField("isPlaying", new LuaCSFunction(ApplicationWrap.get_isPlaying), null), new LuaField("isEditor", new LuaCSFunction(ApplicationWrap.get_isEditor), null), new LuaField("isWebPlayer", new LuaCSFunction(ApplicationWrap.get_isWebPlayer), null), new LuaField("platform", new LuaCSFunction(ApplicationWrap.get_platform), null), new LuaField("isMobilePlatform", new LuaCSFunction(ApplicationWrap.get_isMobilePlatform), null), new LuaField("isConsolePlatform", new LuaCSFunction(ApplicationWrap.get_isConsolePlatform), null), new LuaField("runInBackground", new LuaCSFunction(ApplicationWrap.get_runInBackground), new LuaCSFunction(ApplicationWrap.set_runInBackground)), new LuaField("dataPath", new LuaCSFunction(ApplicationWrap.get_dataPath), null), new LuaField("streamingAssetsPath", new LuaCSFunction(ApplicationWrap.get_streamingAssetsPath), null), new LuaField("persistentDataPath", new LuaCSFunction(ApplicationWrap.get_persistentDataPath), null), new LuaField("temporaryCachePath", new LuaCSFunction(ApplicationWrap.get_temporaryCachePath), null), 
            new LuaField("srcValue", new LuaCSFunction(ApplicationWrap.get_srcValue), null), new LuaField("absoluteURL", new LuaCSFunction(ApplicationWrap.get_absoluteURL), null), new LuaField("unityVersion", new LuaCSFunction(ApplicationWrap.get_unityVersion), null), new LuaField("webSecurityEnabled", new LuaCSFunction(ApplicationWrap.get_webSecurityEnabled), null), new LuaField("webSecurityHostUrl", new LuaCSFunction(ApplicationWrap.get_webSecurityHostUrl), null), new LuaField("targetFrameRate", new LuaCSFunction(ApplicationWrap.get_targetFrameRate), new LuaCSFunction(ApplicationWrap.set_targetFrameRate)), new LuaField("systemLanguage", new LuaCSFunction(ApplicationWrap.get_systemLanguage), null), new LuaField("backgroundLoadingPriority", new LuaCSFunction(ApplicationWrap.get_backgroundLoadingPriority), new LuaCSFunction(ApplicationWrap.set_backgroundLoadingPriority)), new LuaField("internetReachability", new LuaCSFunction(ApplicationWrap.get_internetReachability), null), new LuaField("genuine", new LuaCSFunction(ApplicationWrap.get_genuine), null), new LuaField("genuineCheckAvailable", new LuaCSFunction(ApplicationWrap.get_genuineCheckAvailable), null)
         };
        LuaScriptMgr.RegisterLib(L, "UnityEngine.Application", typeof(Application), regs, fields, typeof(object));
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int RegisterLogCallback(IntPtr L)
    {
        <RegisterLogCallback>c__AnonStorey48 storey = new <RegisterLogCallback>c__AnonStorey48();
        storey.L = L;
        LuaScriptMgr.CheckArgsCount(storey.L, 1);
        Application.LogCallback callback = null;
        if (LuaDLL.lua_type(storey.L, 1) != LuaTypes.LUA_TFUNCTION)
        {
            callback = (Application.LogCallback) LuaScriptMgr.GetNetObject(storey.L, 1, typeof(Application.LogCallback));
        }
        else
        {
            <RegisterLogCallback>c__AnonStorey47 storey2 = new <RegisterLogCallback>c__AnonStorey47();
            storey2.<>f__ref$72 = storey;
            storey2.func = LuaScriptMgr.GetLuaFunction(storey.L, 1);
            callback = new Application.LogCallback(storey2, (IntPtr) this.<>m__1B);
        }
        Application.RegisterLogCallback(callback);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int RegisterLogCallbackThreaded(IntPtr L)
    {
        <RegisterLogCallbackThreaded>c__AnonStorey4A storeya = new <RegisterLogCallbackThreaded>c__AnonStorey4A();
        storeya.L = L;
        LuaScriptMgr.CheckArgsCount(storeya.L, 1);
        Application.LogCallback callback = null;
        if (LuaDLL.lua_type(storeya.L, 1) != LuaTypes.LUA_TFUNCTION)
        {
            callback = (Application.LogCallback) LuaScriptMgr.GetNetObject(storeya.L, 1, typeof(Application.LogCallback));
        }
        else
        {
            <RegisterLogCallbackThreaded>c__AnonStorey49 storey = new <RegisterLogCallbackThreaded>c__AnonStorey49();
            storey.<>f__ref$74 = storeya;
            storey.func = LuaScriptMgr.GetLuaFunction(storeya.L, 1);
            callback = new Application.LogCallback(storey, (IntPtr) this.<>m__1C);
        }
        Application.RegisterLogCallbackThreaded(callback);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int RequestUserAuthorization(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        UserAuthorization authorization = (UserAuthorization) ((int) LuaScriptMgr.GetNetObject(L, 1, typeof(UserAuthorization)));
        AsyncOperation o = Application.RequestUserAuthorization(authorization);
        LuaScriptMgr.PushObject(L, o);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_backgroundLoadingPriority(IntPtr L)
    {
        Application.set_backgroundLoadingPriority((ThreadPriority) ((int) LuaScriptMgr.GetNetObject(L, 3, typeof(ThreadPriority))));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_runInBackground(IntPtr L)
    {
        Application.set_runInBackground(LuaScriptMgr.GetBoolean(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_targetFrameRate(IntPtr L)
    {
        Application.set_targetFrameRate((int) LuaScriptMgr.GetNumber(L, 3));
        return 0;
    }

    [CompilerGenerated]
    private sealed class <RegisterLogCallback>c__AnonStorey47
    {
        internal ApplicationWrap.<RegisterLogCallback>c__AnonStorey48 <>f__ref$72;
        internal LuaFunction func;

        internal void <>m__1B(string param0, string param1, LogType param2)
        {
            int oldTop = this.func.BeginPCall();
            LuaScriptMgr.Push(this.<>f__ref$72.L, param0);
            LuaScriptMgr.Push(this.<>f__ref$72.L, param1);
            LuaScriptMgr.Push(this.<>f__ref$72.L, (Enum) param2);
            this.func.PCall(oldTop, 3);
            this.func.EndPCall(oldTop);
        }
    }

    [CompilerGenerated]
    private sealed class <RegisterLogCallback>c__AnonStorey48
    {
        internal IntPtr L;
    }

    [CompilerGenerated]
    private sealed class <RegisterLogCallbackThreaded>c__AnonStorey49
    {
        internal ApplicationWrap.<RegisterLogCallbackThreaded>c__AnonStorey4A <>f__ref$74;
        internal LuaFunction func;

        internal void <>m__1C(string param0, string param1, LogType param2)
        {
            int oldTop = this.func.BeginPCall();
            LuaScriptMgr.Push(this.<>f__ref$74.L, param0);
            LuaScriptMgr.Push(this.<>f__ref$74.L, param1);
            LuaScriptMgr.Push(this.<>f__ref$74.L, (Enum) param2);
            this.func.PCall(oldTop, 3);
            this.func.EndPCall(oldTop);
        }
    }

    [CompilerGenerated]
    private sealed class <RegisterLogCallbackThreaded>c__AnonStorey4A
    {
        internal IntPtr L;
    }
}

