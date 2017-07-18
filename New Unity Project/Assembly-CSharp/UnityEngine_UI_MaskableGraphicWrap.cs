using com.tencent.pandora;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UnityEngine_UI_MaskableGraphicWrap
{
    private static Type classType = typeof(MaskableGraphic);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _CreateUnityEngine_UI_MaskableGraphic(IntPtr L)
    {
        LuaDLL.luaL_error(L, "UnityEngine.UI.MaskableGraphic class does not have a constructor function");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_maskable(IntPtr L)
    {
        MaskableGraphic luaObject = (MaskableGraphic) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name maskable");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index maskable on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_maskable());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_material(IntPtr L)
    {
        MaskableGraphic luaObject = (MaskableGraphic) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name material");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index material on a nil value");
            }
        }
        LuaScriptMgr.Push(L, (Object) luaObject.get_material());
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

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int ParentMaskStateChanged(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        ((MaskableGraphic) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.MaskableGraphic")).ParentMaskStateChanged();
        return 0;
    }

    public static void Register(IntPtr L)
    {
        LuaMethod[] regs = new LuaMethod[] { new LuaMethod("ParentMaskStateChanged", new LuaCSFunction(UnityEngine_UI_MaskableGraphicWrap.ParentMaskStateChanged)), new LuaMethod("SetMaterialDirty", new LuaCSFunction(UnityEngine_UI_MaskableGraphicWrap.SetMaterialDirty)), new LuaMethod("New", new LuaCSFunction(UnityEngine_UI_MaskableGraphicWrap._CreateUnityEngine_UI_MaskableGraphic)), new LuaMethod("GetClassType", new LuaCSFunction(UnityEngine_UI_MaskableGraphicWrap.GetClassType)), new LuaMethod("__eq", new LuaCSFunction(UnityEngine_UI_MaskableGraphicWrap.Lua_Eq)) };
        LuaField[] fields = new LuaField[] { new LuaField("maskable", new LuaCSFunction(UnityEngine_UI_MaskableGraphicWrap.get_maskable), new LuaCSFunction(UnityEngine_UI_MaskableGraphicWrap.set_maskable)), new LuaField("material", new LuaCSFunction(UnityEngine_UI_MaskableGraphicWrap.get_material), new LuaCSFunction(UnityEngine_UI_MaskableGraphicWrap.set_material)) };
        LuaScriptMgr.RegisterLib(L, "UnityEngine.UI.MaskableGraphic", typeof(MaskableGraphic), regs, fields, typeof(Graphic));
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_maskable(IntPtr L)
    {
        MaskableGraphic luaObject = (MaskableGraphic) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name maskable");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index maskable on a nil value");
            }
        }
        luaObject.set_maskable(LuaScriptMgr.GetBoolean(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_material(IntPtr L)
    {
        MaskableGraphic luaObject = (MaskableGraphic) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name material");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index material on a nil value");
            }
        }
        luaObject.set_material((Material) LuaScriptMgr.GetUnityObject(L, 3, typeof(Material)));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int SetMaterialDirty(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        ((MaskableGraphic) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.MaskableGraphic")).SetMaterialDirty();
        return 0;
    }
}

