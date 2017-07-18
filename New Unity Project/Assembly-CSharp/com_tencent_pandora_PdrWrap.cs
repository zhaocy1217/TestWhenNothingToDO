using com.tencent.pandora;
using System;
using UnityEngine;

public class com_tencent_pandora_PdrWrap
{
    private static Type classType = typeof(Pdr);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _Createcom_tencent_pandora_Pdr(IntPtr L)
    {
        LuaDLL.luaL_error(L, "com.tencent.pandora.Pdr class does not have a constructor function");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetClassType(IntPtr L)
    {
        LuaScriptMgr.Push(L, classType);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetTempPath(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        string tempPath = ((Pdr) LuaScriptMgr.GetUnityObjectSelf(L, 1, "com.tencent.pandora.Pdr")).GetTempPath();
        LuaScriptMgr.Push(L, tempPath);
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
        LuaMethod[] regs = new LuaMethod[] { new LuaMethod("GetTempPath", new LuaCSFunction(com_tencent_pandora_PdrWrap.GetTempPath)), new LuaMethod("New", new LuaCSFunction(com_tencent_pandora_PdrWrap._Createcom_tencent_pandora_Pdr)), new LuaMethod("GetClassType", new LuaCSFunction(com_tencent_pandora_PdrWrap.GetClassType)), new LuaMethod("__eq", new LuaCSFunction(com_tencent_pandora_PdrWrap.Lua_Eq)) };
        LuaField[] fields = new LuaField[0];
        LuaScriptMgr.RegisterLib(L, "com.tencent.pandora.Pdr", typeof(Pdr), regs, fields, typeof(MonoBehaviour));
    }
}

