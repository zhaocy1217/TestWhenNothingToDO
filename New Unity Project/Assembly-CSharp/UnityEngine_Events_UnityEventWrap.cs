using com.tencent.pandora;
using System;
using System.Runtime.CompilerServices;
using UnityEngine.Events;

public class UnityEngine_Events_UnityEventWrap
{
    private static Type classType = typeof(UnityEvent);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _CreateUnityEngine_Events_UnityEvent(IntPtr L)
    {
        if (LuaDLL.lua_gettop(L) == 0)
        {
            UnityEvent o = new UnityEvent();
            LuaScriptMgr.PushObject(L, o);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: UnityEngine.Events.UnityEvent.New");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int AddListener(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        UnityEvent event2 = (UnityEvent) LuaScriptMgr.GetNetObjectSelf(L, 1, "UnityEngine.Events.UnityEvent");
        UnityAction action = null;
        if (LuaDLL.lua_type(L, 2) != LuaTypes.LUA_TFUNCTION)
        {
            action = (UnityAction) LuaScriptMgr.GetNetObject(L, 2, typeof(UnityAction));
        }
        else
        {
            <AddListener>c__AnonStorey4F storeyf = new <AddListener>c__AnonStorey4F();
            storeyf.func = LuaScriptMgr.GetLuaFunction(L, 2);
            action = new UnityAction(storeyf, (IntPtr) this.<>m__1F);
        }
        event2.AddListener(action);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetClassType(IntPtr L)
    {
        LuaScriptMgr.Push(L, classType);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Invoke(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        ((UnityEvent) LuaScriptMgr.GetNetObjectSelf(L, 1, "UnityEngine.Events.UnityEvent")).Invoke();
        return 0;
    }

    public static void Register(IntPtr L)
    {
        LuaMethod[] regs = new LuaMethod[] { new LuaMethod("AddListener", new LuaCSFunction(UnityEngine_Events_UnityEventWrap.AddListener)), new LuaMethod("RemoveListener", new LuaCSFunction(UnityEngine_Events_UnityEventWrap.RemoveListener)), new LuaMethod("Invoke", new LuaCSFunction(UnityEngine_Events_UnityEventWrap.Invoke)), new LuaMethod("New", new LuaCSFunction(UnityEngine_Events_UnityEventWrap._CreateUnityEngine_Events_UnityEvent)), new LuaMethod("GetClassType", new LuaCSFunction(UnityEngine_Events_UnityEventWrap.GetClassType)) };
        LuaField[] fields = new LuaField[0];
        LuaScriptMgr.RegisterLib(L, "UnityEngine.Events.UnityEvent", typeof(UnityEvent), regs, fields, typeof(UnityEventBase));
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int RemoveListener(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        UnityEvent event2 = (UnityEvent) LuaScriptMgr.GetNetObjectSelf(L, 1, "UnityEngine.Events.UnityEvent");
        UnityAction action = null;
        if (LuaDLL.lua_type(L, 2) != LuaTypes.LUA_TFUNCTION)
        {
            action = (UnityAction) LuaScriptMgr.GetNetObject(L, 2, typeof(UnityAction));
        }
        else
        {
            <RemoveListener>c__AnonStorey50 storey = new <RemoveListener>c__AnonStorey50();
            storey.func = LuaScriptMgr.GetLuaFunction(L, 2);
            action = new UnityAction(storey, (IntPtr) this.<>m__20);
        }
        event2.RemoveListener(action);
        return 0;
    }

    [CompilerGenerated]
    private sealed class <AddListener>c__AnonStorey4F
    {
        internal LuaFunction func;

        internal void <>m__1F()
        {
            this.func.Call();
        }
    }

    [CompilerGenerated]
    private sealed class <RemoveListener>c__AnonStorey50
    {
        internal LuaFunction func;

        internal void <>m__20()
        {
            this.func.Call();
        }
    }
}

