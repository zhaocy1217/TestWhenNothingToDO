using com.tencent.pandora;
using System;
using UnityEngine;

public class AnimationClipWrap
{
    private static Type classType = typeof(AnimationClip);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _CreateAnimationClip(IntPtr L)
    {
        if (LuaDLL.lua_gettop(L) == 0)
        {
            AnimationClip clip = new AnimationClip();
            LuaScriptMgr.Push(L, (Object) clip);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: AnimationClip.New");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int AddEvent(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        AnimationClip clip = (AnimationClip) LuaScriptMgr.GetUnityObjectSelf(L, 1, "AnimationClip");
        AnimationEvent event2 = (AnimationEvent) LuaScriptMgr.GetNetObject(L, 2, typeof(AnimationEvent));
        clip.AddEvent(event2);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int ClearCurves(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        ((AnimationClip) LuaScriptMgr.GetUnityObjectSelf(L, 1, "AnimationClip")).ClearCurves();
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int EnsureQuaternionContinuity(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        ((AnimationClip) LuaScriptMgr.GetUnityObjectSelf(L, 1, "AnimationClip")).EnsureQuaternionContinuity();
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_frameRate(IntPtr L)
    {
        AnimationClip luaObject = (AnimationClip) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name frameRate");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index frameRate on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_frameRate());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_length(IntPtr L)
    {
        AnimationClip luaObject = (AnimationClip) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name length");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index length on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_length());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_localBounds(IntPtr L)
    {
        AnimationClip luaObject = (AnimationClip) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name localBounds");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index localBounds on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_localBounds());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_wrapMode(IntPtr L)
    {
        AnimationClip luaObject = (AnimationClip) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name wrapMode");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index wrapMode on a nil value");
            }
        }
        LuaScriptMgr.Push(L, (Enum) luaObject.get_wrapMode());
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
        LuaMethod[] regs = new LuaMethod[] { new LuaMethod("SetCurve", new LuaCSFunction(AnimationClipWrap.SetCurve)), new LuaMethod("EnsureQuaternionContinuity", new LuaCSFunction(AnimationClipWrap.EnsureQuaternionContinuity)), new LuaMethod("ClearCurves", new LuaCSFunction(AnimationClipWrap.ClearCurves)), new LuaMethod("AddEvent", new LuaCSFunction(AnimationClipWrap.AddEvent)), new LuaMethod("New", new LuaCSFunction(AnimationClipWrap._CreateAnimationClip)), new LuaMethod("GetClassType", new LuaCSFunction(AnimationClipWrap.GetClassType)), new LuaMethod("__eq", new LuaCSFunction(AnimationClipWrap.Lua_Eq)) };
        LuaField[] fields = new LuaField[] { new LuaField("length", new LuaCSFunction(AnimationClipWrap.get_length), null), new LuaField("frameRate", new LuaCSFunction(AnimationClipWrap.get_frameRate), new LuaCSFunction(AnimationClipWrap.set_frameRate)), new LuaField("wrapMode", new LuaCSFunction(AnimationClipWrap.get_wrapMode), new LuaCSFunction(AnimationClipWrap.set_wrapMode)), new LuaField("localBounds", new LuaCSFunction(AnimationClipWrap.get_localBounds), new LuaCSFunction(AnimationClipWrap.set_localBounds)) };
        LuaScriptMgr.RegisterLib(L, "UnityEngine.AnimationClip", typeof(AnimationClip), regs, fields, typeof(Object));
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_frameRate(IntPtr L)
    {
        AnimationClip luaObject = (AnimationClip) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name frameRate");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index frameRate on a nil value");
            }
        }
        luaObject.set_frameRate((float) LuaScriptMgr.GetNumber(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_localBounds(IntPtr L)
    {
        AnimationClip luaObject = (AnimationClip) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name localBounds");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index localBounds on a nil value");
            }
        }
        luaObject.set_localBounds(LuaScriptMgr.GetBounds(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_wrapMode(IntPtr L)
    {
        AnimationClip luaObject = (AnimationClip) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name wrapMode");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index wrapMode on a nil value");
            }
        }
        luaObject.set_wrapMode((WrapMode) ((int) LuaScriptMgr.GetNetObject(L, 3, typeof(WrapMode))));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int SetCurve(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 5);
        AnimationClip clip = (AnimationClip) LuaScriptMgr.GetUnityObjectSelf(L, 1, "AnimationClip");
        string luaString = LuaScriptMgr.GetLuaString(L, 2);
        Type typeObject = LuaScriptMgr.GetTypeObject(L, 3);
        string str2 = LuaScriptMgr.GetLuaString(L, 4);
        AnimationCurve curve = (AnimationCurve) LuaScriptMgr.GetNetObject(L, 5, typeof(AnimationCurve));
        clip.SetCurve(luaString, typeObject, str2, curve);
        return 0;
    }
}

