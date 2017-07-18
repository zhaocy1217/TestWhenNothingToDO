using com.tencent.pandora;
using System;
using UnityEngine;

public class TouchPhaseWrap
{
    private static LuaMethod[] enums = new LuaMethod[] { new LuaMethod("Began", new LuaCSFunction(TouchPhaseWrap.GetBegan)), new LuaMethod("Moved", new LuaCSFunction(TouchPhaseWrap.GetMoved)), new LuaMethod("Stationary", new LuaCSFunction(TouchPhaseWrap.GetStationary)), new LuaMethod("Ended", new LuaCSFunction(TouchPhaseWrap.GetEnded)), new LuaMethod("Canceled", new LuaCSFunction(TouchPhaseWrap.GetCanceled)), new LuaMethod("IntToEnum", new LuaCSFunction(TouchPhaseWrap.IntToEnum)) };

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetBegan(IntPtr L)
    {
        LuaScriptMgr.Push(L, (Enum) ((TouchPhase) 0));
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetCanceled(IntPtr L)
    {
        LuaScriptMgr.Push(L, (Enum) ((TouchPhase) 4));
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetEnded(IntPtr L)
    {
        LuaScriptMgr.Push(L, (Enum) ((TouchPhase) 3));
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetMoved(IntPtr L)
    {
        LuaScriptMgr.Push(L, (Enum) ((TouchPhase) 1));
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetStationary(IntPtr L)
    {
        LuaScriptMgr.Push(L, (Enum) ((TouchPhase) 2));
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int IntToEnum(IntPtr L)
    {
        int num = (int) LuaDLL.lua_tonumber(L, 1);
        TouchPhase phase = (TouchPhase) num;
        LuaScriptMgr.Push(L, (Enum) phase);
        return 1;
    }

    public static void Register(IntPtr L)
    {
        LuaScriptMgr.RegisterLib(L, "UnityEngine.TouchPhase", typeof(TouchPhase), enums);
    }
}

