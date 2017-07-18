using com.tencent.pandora;
using System;
using UnityEngine;

public class TrackedReferenceWrap
{
    private static Type classType = typeof(TrackedReference);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _CreateTrackedReference(IntPtr L)
    {
        LuaDLL.luaL_error(L, "TrackedReference class does not have a constructor function");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Equals(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        TrackedReference varObject = LuaScriptMgr.GetVarObject(L, 1) as TrackedReference;
        object obj2 = LuaScriptMgr.GetVarObject(L, 2);
        bool b = (varObject == null) ? (obj2 == null) : varObject.Equals(obj2);
        LuaScriptMgr.Push(L, b);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetClassType(IntPtr L)
    {
        LuaScriptMgr.Push(L, classType);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetHashCode(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        int hashCode = ((TrackedReference) LuaScriptMgr.GetTrackedObjectSelf(L, 1, "TrackedReference")).GetHashCode();
        LuaScriptMgr.Push(L, hashCode);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Lua_Eq(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        TrackedReference luaObject = LuaScriptMgr.GetLuaObject(L, 1) as TrackedReference;
        TrackedReference reference2 = LuaScriptMgr.GetLuaObject(L, 2) as TrackedReference;
        bool b = luaObject == reference2;
        LuaScriptMgr.Push(L, b);
        return 1;
    }

    public static void Register(IntPtr L)
    {
        LuaMethod[] regs = new LuaMethod[] { new LuaMethod("Equals", new LuaCSFunction(TrackedReferenceWrap.Equals)), new LuaMethod("GetHashCode", new LuaCSFunction(TrackedReferenceWrap.GetHashCode)), new LuaMethod("New", new LuaCSFunction(TrackedReferenceWrap._CreateTrackedReference)), new LuaMethod("GetClassType", new LuaCSFunction(TrackedReferenceWrap.GetClassType)), new LuaMethod("__eq", new LuaCSFunction(TrackedReferenceWrap.Lua_Eq)) };
        LuaField[] fields = new LuaField[0];
        LuaScriptMgr.RegisterLib(L, "UnityEngine.TrackedReference", typeof(TrackedReference), regs, fields, typeof(object));
    }
}

