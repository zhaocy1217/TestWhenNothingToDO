using com.tencent.pandora;
using System;
using UnityEngine;

public class AnimationBlendModeWrap
{
    private static LuaMethod[] enums = new LuaMethod[] { new LuaMethod("Blend", new LuaCSFunction(AnimationBlendModeWrap.GetBlend)), new LuaMethod("Additive", new LuaCSFunction(AnimationBlendModeWrap.GetAdditive)), new LuaMethod("IntToEnum", new LuaCSFunction(AnimationBlendModeWrap.IntToEnum)) };

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetAdditive(IntPtr L)
    {
        LuaScriptMgr.Push(L, (Enum) ((AnimationBlendMode) 1));
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetBlend(IntPtr L)
    {
        LuaScriptMgr.Push(L, (Enum) ((AnimationBlendMode) 0));
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int IntToEnum(IntPtr L)
    {
        int num = (int) LuaDLL.lua_tonumber(L, 1);
        AnimationBlendMode mode = (AnimationBlendMode) num;
        LuaScriptMgr.Push(L, (Enum) mode);
        return 1;
    }

    public static void Register(IntPtr L)
    {
        LuaScriptMgr.RegisterLib(L, "UnityEngine.AnimationBlendMode", typeof(AnimationBlendMode), enums);
    }
}

