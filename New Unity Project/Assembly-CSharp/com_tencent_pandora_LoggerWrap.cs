using com.tencent.pandora;
using System;
using UnityEngine;

public class com_tencent_pandora_LoggerWrap
{
    private static Type classType = typeof(Logger);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _Createcom_tencent_pandora_Logger(IntPtr L)
    {
        LuaDLL.luaL_error(L, "com.tencent.pandora.Logger class does not have a constructor function");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int DEBUG(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        Logger.DEBUG(LuaScriptMgr.GetLuaString(L, 1));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int ERROR(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        Logger.ERROR(LuaScriptMgr.GetLuaString(L, 1));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetClassType(IntPtr L)
    {
        LuaScriptMgr.Push(L, classType);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetThreadId(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 0);
        string threadId = Logger.GetThreadId();
        LuaScriptMgr.Push(L, threadId);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int INFO(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        Logger.INFO(LuaScriptMgr.GetLuaString(L, 1));
        return 0;
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
        LuaMethod[] regs = new LuaMethod[] { new LuaMethod("GetThreadId", new LuaCSFunction(com_tencent_pandora_LoggerWrap.GetThreadId)), new LuaMethod("DEBUG", new LuaCSFunction(com_tencent_pandora_LoggerWrap.DEBUG)), new LuaMethod("INFO", new LuaCSFunction(com_tencent_pandora_LoggerWrap.INFO)), new LuaMethod("ERROR", new LuaCSFunction(com_tencent_pandora_LoggerWrap.ERROR)), new LuaMethod("WARN", new LuaCSFunction(com_tencent_pandora_LoggerWrap.WARN)), new LuaMethod("REPORT", new LuaCSFunction(com_tencent_pandora_LoggerWrap.REPORT)), new LuaMethod("New", new LuaCSFunction(com_tencent_pandora_LoggerWrap._Createcom_tencent_pandora_Logger)), new LuaMethod("GetClassType", new LuaCSFunction(com_tencent_pandora_LoggerWrap.GetClassType)), new LuaMethod("__eq", new LuaCSFunction(com_tencent_pandora_LoggerWrap.Lua_Eq)) };
        LuaField[] fields = new LuaField[0];
        LuaScriptMgr.RegisterLib(L, "com.tencent.pandora.Logger", typeof(Logger), regs, fields, typeof(MonoBehaviour));
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int REPORT(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 3);
        string luaString = LuaScriptMgr.GetLuaString(L, 1);
        int number = (int) LuaScriptMgr.GetNumber(L, 2);
        int returnCode = (int) LuaScriptMgr.GetNumber(L, 3);
        Logger.REPORT(luaString, number, returnCode);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int WARN(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        Logger.WARN(LuaScriptMgr.GetLuaString(L, 1));
        return 0;
    }
}

