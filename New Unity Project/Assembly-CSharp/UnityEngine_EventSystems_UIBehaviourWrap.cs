using com.tencent.pandora;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnityEngine_EventSystems_UIBehaviourWrap
{
    private static Type classType = typeof(UIBehaviour);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _CreateUnityEngine_EventSystems_UIBehaviour(IntPtr L)
    {
        LuaDLL.luaL_error(L, "UnityEngine.EventSystems.UIBehaviour class does not have a constructor function");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetClassType(IntPtr L)
    {
        LuaScriptMgr.Push(L, classType);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int IsActive(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        bool b = ((UIBehaviour) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.EventSystems.UIBehaviour")).IsActive();
        LuaScriptMgr.Push(L, b);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int IsDestroyed(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        bool b = ((UIBehaviour) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.EventSystems.UIBehaviour")).IsDestroyed();
        LuaScriptMgr.Push(L, b);
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
        LuaMethod[] regs = new LuaMethod[] { new LuaMethod("IsActive", new LuaCSFunction(UnityEngine_EventSystems_UIBehaviourWrap.IsActive)), new LuaMethod("IsDestroyed", new LuaCSFunction(UnityEngine_EventSystems_UIBehaviourWrap.IsDestroyed)), new LuaMethod("New", new LuaCSFunction(UnityEngine_EventSystems_UIBehaviourWrap._CreateUnityEngine_EventSystems_UIBehaviour)), new LuaMethod("GetClassType", new LuaCSFunction(UnityEngine_EventSystems_UIBehaviourWrap.GetClassType)), new LuaMethod("__eq", new LuaCSFunction(UnityEngine_EventSystems_UIBehaviourWrap.Lua_Eq)) };
        LuaField[] fields = new LuaField[0];
        LuaScriptMgr.RegisterLib(L, "UnityEngine.EventSystems.UIBehaviour", typeof(UIBehaviour), regs, fields, typeof(MonoBehaviour));
    }
}

