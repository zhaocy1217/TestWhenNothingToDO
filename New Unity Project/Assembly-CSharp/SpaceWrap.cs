using com.tencent.pandora;
using System;
using UnityEngine;

public class SpaceWrap
{
    private static LuaMethod[] enums = new LuaMethod[] { new LuaMethod("World", new LuaCSFunction(SpaceWrap.GetWorld)), new LuaMethod("Self", new LuaCSFunction(SpaceWrap.GetSelf)), new LuaMethod("IntToEnum", new LuaCSFunction(SpaceWrap.IntToEnum)) };

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetSelf(IntPtr L)
    {
        LuaScriptMgr.Push(L, (Enum) ((Space) 1));
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetWorld(IntPtr L)
    {
        LuaScriptMgr.Push(L, (Enum) ((Space) 0));
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int IntToEnum(IntPtr L)
    {
        int num = (int) LuaDLL.lua_tonumber(L, 1);
        Space space = (Space) num;
        LuaScriptMgr.Push(L, (Enum) space);
        return 1;
    }

    public static void Register(IntPtr L)
    {
        LuaScriptMgr.RegisterLib(L, "UnityEngine.Space", typeof(Space), enums);
    }
}

