using com.tencent.pandora;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UnityEngine_UI_RawImageWrap
{
    private static Type classType = typeof(RawImage);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _CreateUnityEngine_UI_RawImage(IntPtr L)
    {
        LuaDLL.luaL_error(L, "UnityEngine.UI.RawImage class does not have a constructor function");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_mainTexture(IntPtr L)
    {
        RawImage luaObject = (RawImage) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name mainTexture");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index mainTexture on a nil value");
            }
        }
        LuaScriptMgr.Push(L, (Object) luaObject.get_mainTexture());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_texture(IntPtr L)
    {
        RawImage luaObject = (RawImage) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name texture");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index texture on a nil value");
            }
        }
        LuaScriptMgr.Push(L, (Object) luaObject.get_texture());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_uvRect(IntPtr L)
    {
        RawImage luaObject = (RawImage) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name uvRect");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index uvRect on a nil value");
            }
        }
        LuaScriptMgr.PushValue(L, luaObject.get_uvRect());
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
        LuaMethod[] regs = new LuaMethod[] { new LuaMethod("SetNativeSize", new LuaCSFunction(UnityEngine_UI_RawImageWrap.SetNativeSize)), new LuaMethod("New", new LuaCSFunction(UnityEngine_UI_RawImageWrap._CreateUnityEngine_UI_RawImage)), new LuaMethod("GetClassType", new LuaCSFunction(UnityEngine_UI_RawImageWrap.GetClassType)), new LuaMethod("__eq", new LuaCSFunction(UnityEngine_UI_RawImageWrap.Lua_Eq)) };
        LuaField[] fields = new LuaField[] { new LuaField("mainTexture", new LuaCSFunction(UnityEngine_UI_RawImageWrap.get_mainTexture), null), new LuaField("texture", new LuaCSFunction(UnityEngine_UI_RawImageWrap.get_texture), new LuaCSFunction(UnityEngine_UI_RawImageWrap.set_texture)), new LuaField("uvRect", new LuaCSFunction(UnityEngine_UI_RawImageWrap.get_uvRect), new LuaCSFunction(UnityEngine_UI_RawImageWrap.set_uvRect)) };
        LuaScriptMgr.RegisterLib(L, "UnityEngine.UI.RawImage", typeof(RawImage), regs, fields, typeof(MaskableGraphic));
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_texture(IntPtr L)
    {
        RawImage luaObject = (RawImage) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name texture");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index texture on a nil value");
            }
        }
        luaObject.set_texture((Texture) LuaScriptMgr.GetUnityObject(L, 3, typeof(Texture)));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_uvRect(IntPtr L)
    {
        RawImage luaObject = (RawImage) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name uvRect");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index uvRect on a nil value");
            }
        }
        luaObject.set_uvRect((Rect) LuaScriptMgr.GetNetObject(L, 3, typeof(Rect)));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int SetNativeSize(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        ((RawImage) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.RawImage")).SetNativeSize();
        return 0;
    }
}

