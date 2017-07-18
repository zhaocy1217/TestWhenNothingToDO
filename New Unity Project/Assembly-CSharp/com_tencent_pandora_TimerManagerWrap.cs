using com.tencent.pandora;
using System;
using UnityEngine;

public class com_tencent_pandora_TimerManagerWrap
{
    private static Type classType = typeof(TimerManager);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _Createcom_tencent_pandora_TimerManager(IntPtr L)
    {
        LuaDLL.luaL_error(L, "com.tencent.pandora.TimerManager class does not have a constructor function");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int AddTimerEvent(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        TimerManager manager = (TimerManager) LuaScriptMgr.GetUnityObjectSelf(L, 1, "com.tencent.pandora.TimerManager");
        TimerInfo info = (TimerInfo) LuaScriptMgr.GetNetObject(L, 2, typeof(TimerInfo));
        manager.AddTimerEvent(info);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_Interval(IntPtr L)
    {
        TimerManager luaObject = (TimerManager) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name Interval");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index Interval on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.Interval);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetClassType(IntPtr L)
    {
        LuaScriptMgr.Push(L, classType);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Lua_Eq(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Object luaObject = LuaScriptMgr.GetLuaObject(L, 1) as Object;
        Object obj3 = LuaScriptMgr.GetLuaObject(L, 2) as Object;
        bool b = luaObject == obj3;
        LuaScriptMgr.Push(L, b);
        return 1;
    }

    public static void Register(IntPtr L)
    {
        LuaMethod[] regs = new LuaMethod[] { new LuaMethod("StartTimer", new LuaCSFunction(com_tencent_pandora_TimerManagerWrap.StartTimer)), new LuaMethod("StopTimer", new LuaCSFunction(com_tencent_pandora_TimerManagerWrap.StopTimer)), new LuaMethod("AddTimerEvent", new LuaCSFunction(com_tencent_pandora_TimerManagerWrap.AddTimerEvent)), new LuaMethod("RemoveTimerEvent", new LuaCSFunction(com_tencent_pandora_TimerManagerWrap.RemoveTimerEvent)), new LuaMethod("StopTimerEvent", new LuaCSFunction(com_tencent_pandora_TimerManagerWrap.StopTimerEvent)), new LuaMethod("ResumeTimerEvent", new LuaCSFunction(com_tencent_pandora_TimerManagerWrap.ResumeTimerEvent)), new LuaMethod("New", new LuaCSFunction(com_tencent_pandora_TimerManagerWrap._Createcom_tencent_pandora_TimerManager)), new LuaMethod("GetClassType", new LuaCSFunction(com_tencent_pandora_TimerManagerWrap.GetClassType)), new LuaMethod("__eq", new LuaCSFunction(com_tencent_pandora_TimerManagerWrap.Lua_Eq)) };
        LuaField[] fields = new LuaField[] { new LuaField("Interval", new LuaCSFunction(com_tencent_pandora_TimerManagerWrap.get_Interval), new LuaCSFunction(com_tencent_pandora_TimerManagerWrap.set_Interval)) };
        LuaScriptMgr.RegisterLib(L, "com.tencent.pandora.TimerManager", typeof(TimerManager), regs, fields, typeof(View));
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int RemoveTimerEvent(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        TimerManager manager = (TimerManager) LuaScriptMgr.GetUnityObjectSelf(L, 1, "com.tencent.pandora.TimerManager");
        TimerInfo info = (TimerInfo) LuaScriptMgr.GetNetObject(L, 2, typeof(TimerInfo));
        manager.RemoveTimerEvent(info);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int ResumeTimerEvent(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        TimerManager manager = (TimerManager) LuaScriptMgr.GetUnityObjectSelf(L, 1, "com.tencent.pandora.TimerManager");
        TimerInfo info = (TimerInfo) LuaScriptMgr.GetNetObject(L, 2, typeof(TimerInfo));
        manager.ResumeTimerEvent(info);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_Interval(IntPtr L)
    {
        TimerManager luaObject = (TimerManager) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name Interval");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index Interval on a nil value");
            }
        }
        luaObject.Interval = (float) LuaScriptMgr.GetNumber(L, 3);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int StartTimer(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        TimerManager manager = (TimerManager) LuaScriptMgr.GetUnityObjectSelf(L, 1, "com.tencent.pandora.TimerManager");
        float number = (float) LuaScriptMgr.GetNumber(L, 2);
        manager.StartTimer(number);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int StopTimer(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        ((TimerManager) LuaScriptMgr.GetUnityObjectSelf(L, 1, "com.tencent.pandora.TimerManager")).StopTimer();
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int StopTimerEvent(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        TimerManager manager = (TimerManager) LuaScriptMgr.GetUnityObjectSelf(L, 1, "com.tencent.pandora.TimerManager");
        TimerInfo info = (TimerInfo) LuaScriptMgr.GetNetObject(L, 2, typeof(TimerInfo));
        manager.StopTimerEvent(info);
        return 0;
    }
}

