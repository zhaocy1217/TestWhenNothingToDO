using com.tencent.pandora;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

public class UnityEngine_Events_UnityEventBaseWrap
{
    private static Type classType = typeof(UnityEventBase);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _CreateUnityEngine_Events_UnityEventBase(IntPtr L)
    {
        LuaDLL.luaL_error(L, "UnityEngine.Events.UnityEventBase class does not have a constructor function");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetClassType(IntPtr L)
    {
        LuaScriptMgr.Push(L, classType);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetPersistentEventCount(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        int persistentEventCount = ((UnityEventBase) LuaScriptMgr.GetNetObjectSelf(L, 1, "UnityEngine.Events.UnityEventBase")).GetPersistentEventCount();
        LuaScriptMgr.Push(L, persistentEventCount);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetPersistentMethodName(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        UnityEventBase base2 = (UnityEventBase) LuaScriptMgr.GetNetObjectSelf(L, 1, "UnityEngine.Events.UnityEventBase");
        int number = (int) LuaScriptMgr.GetNumber(L, 2);
        string persistentMethodName = base2.GetPersistentMethodName(number);
        LuaScriptMgr.Push(L, persistentMethodName);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetPersistentTarget(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        UnityEventBase base2 = (UnityEventBase) LuaScriptMgr.GetNetObjectSelf(L, 1, "UnityEngine.Events.UnityEventBase");
        int number = (int) LuaScriptMgr.GetNumber(L, 2);
        Object persistentTarget = base2.GetPersistentTarget(number);
        LuaScriptMgr.Push(L, persistentTarget);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetValidMethodInfo(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 3);
        object varObject = LuaScriptMgr.GetVarObject(L, 1);
        string luaString = LuaScriptMgr.GetLuaString(L, 2);
        Type[] arrayObject = LuaScriptMgr.GetArrayObject<Type>(L, 3);
        MethodInfo o = UnityEventBase.GetValidMethodInfo(varObject, luaString, arrayObject);
        LuaScriptMgr.PushObject(L, o);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Lua_ToString(IntPtr L)
    {
        object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject != null)
        {
            LuaScriptMgr.Push(L, luaObject.ToString());
        }
        else
        {
            LuaScriptMgr.Push(L, "Table: UnityEngine.Events.UnityEventBase");
        }
        return 1;
    }

    public static void Register(IntPtr L)
    {
        LuaMethod[] regs = new LuaMethod[] { new LuaMethod("GetPersistentEventCount", new LuaCSFunction(UnityEngine_Events_UnityEventBaseWrap.GetPersistentEventCount)), new LuaMethod("GetPersistentTarget", new LuaCSFunction(UnityEngine_Events_UnityEventBaseWrap.GetPersistentTarget)), new LuaMethod("GetPersistentMethodName", new LuaCSFunction(UnityEngine_Events_UnityEventBaseWrap.GetPersistentMethodName)), new LuaMethod("SetPersistentListenerState", new LuaCSFunction(UnityEngine_Events_UnityEventBaseWrap.SetPersistentListenerState)), new LuaMethod("RemoveAllListeners", new LuaCSFunction(UnityEngine_Events_UnityEventBaseWrap.RemoveAllListeners)), new LuaMethod("ToString", new LuaCSFunction(UnityEngine_Events_UnityEventBaseWrap.ToString)), new LuaMethod("GetValidMethodInfo", new LuaCSFunction(UnityEngine_Events_UnityEventBaseWrap.GetValidMethodInfo)), new LuaMethod("New", new LuaCSFunction(UnityEngine_Events_UnityEventBaseWrap._CreateUnityEngine_Events_UnityEventBase)), new LuaMethod("GetClassType", new LuaCSFunction(UnityEngine_Events_UnityEventBaseWrap.GetClassType)), new LuaMethod("__tostring", new LuaCSFunction(UnityEngine_Events_UnityEventBaseWrap.Lua_ToString)) };
        LuaField[] fields = new LuaField[0];
        LuaScriptMgr.RegisterLib(L, "UnityEngine.Events.UnityEventBase", typeof(UnityEventBase), regs, fields, typeof(object));
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int RemoveAllListeners(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        ((UnityEventBase) LuaScriptMgr.GetNetObjectSelf(L, 1, "UnityEngine.Events.UnityEventBase")).RemoveAllListeners();
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int SetPersistentListenerState(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 3);
        UnityEventBase base2 = (UnityEventBase) LuaScriptMgr.GetNetObjectSelf(L, 1, "UnityEngine.Events.UnityEventBase");
        int number = (int) LuaScriptMgr.GetNumber(L, 2);
        UnityEventCallState state = (UnityEventCallState) ((int) LuaScriptMgr.GetNetObject(L, 3, typeof(UnityEventCallState)));
        base2.SetPersistentListenerState(number, state);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int ToString(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        string str = ((UnityEventBase) LuaScriptMgr.GetNetObjectSelf(L, 1, "UnityEngine.Events.UnityEventBase")).ToString();
        LuaScriptMgr.Push(L, str);
        return 1;
    }
}

