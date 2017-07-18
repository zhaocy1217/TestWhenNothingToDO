using com.tencent.pandora;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnityEngine_UI_ToggleWrap
{
    private static Type classType = typeof(Toggle);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _CreateUnityEngine_UI_Toggle(IntPtr L)
    {
        LuaDLL.luaL_error(L, "UnityEngine.UI.Toggle class does not have a constructor function");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_graphic(IntPtr L)
    {
        Toggle luaObject = (Toggle) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name graphic");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index graphic on a nil value");
            }
        }
        LuaScriptMgr.Push(L, (Object) luaObject.graphic);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_group(IntPtr L)
    {
        Toggle luaObject = (Toggle) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name group");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index group on a nil value");
            }
        }
        LuaScriptMgr.Push(L, (Object) luaObject.get_group());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_isOn(IntPtr L)
    {
        Toggle luaObject = (Toggle) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name isOn");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index isOn on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_isOn());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_onValueChanged(IntPtr L)
    {
        Toggle luaObject = (Toggle) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name onValueChanged");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index onValueChanged on a nil value");
            }
        }
        LuaScriptMgr.PushObject(L, luaObject.onValueChanged);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_toggleTransition(IntPtr L)
    {
        Toggle luaObject = (Toggle) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name toggleTransition");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index toggleTransition on a nil value");
            }
        }
        LuaScriptMgr.Push(L, (Enum) luaObject.toggleTransition);
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
    private static int OnPointerClick(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Toggle toggle = (Toggle) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Toggle");
        PointerEventData data = (PointerEventData) LuaScriptMgr.GetNetObject(L, 2, typeof(PointerEventData));
        toggle.OnPointerClick(data);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int OnSubmit(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Toggle toggle = (Toggle) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Toggle");
        BaseEventData data = (BaseEventData) LuaScriptMgr.GetNetObject(L, 2, typeof(BaseEventData));
        toggle.OnSubmit(data);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Rebuild(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Toggle toggle = (Toggle) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Toggle");
        CanvasUpdate update = (CanvasUpdate) ((int) LuaScriptMgr.GetNetObject(L, 2, typeof(CanvasUpdate)));
        toggle.Rebuild(update);
        return 0;
    }

    public static void Register(IntPtr L)
    {
        LuaMethod[] regs = new LuaMethod[] { new LuaMethod("Rebuild", new LuaCSFunction(UnityEngine_UI_ToggleWrap.Rebuild)), new LuaMethod("OnPointerClick", new LuaCSFunction(UnityEngine_UI_ToggleWrap.OnPointerClick)), new LuaMethod("OnSubmit", new LuaCSFunction(UnityEngine_UI_ToggleWrap.OnSubmit)), new LuaMethod("New", new LuaCSFunction(UnityEngine_UI_ToggleWrap._CreateUnityEngine_UI_Toggle)), new LuaMethod("GetClassType", new LuaCSFunction(UnityEngine_UI_ToggleWrap.GetClassType)), new LuaMethod("__eq", new LuaCSFunction(UnityEngine_UI_ToggleWrap.Lua_Eq)) };
        LuaField[] fields = new LuaField[] { new LuaField("toggleTransition", new LuaCSFunction(UnityEngine_UI_ToggleWrap.get_toggleTransition), new LuaCSFunction(UnityEngine_UI_ToggleWrap.set_toggleTransition)), new LuaField("graphic", new LuaCSFunction(UnityEngine_UI_ToggleWrap.get_graphic), new LuaCSFunction(UnityEngine_UI_ToggleWrap.set_graphic)), new LuaField("onValueChanged", new LuaCSFunction(UnityEngine_UI_ToggleWrap.get_onValueChanged), new LuaCSFunction(UnityEngine_UI_ToggleWrap.set_onValueChanged)), new LuaField("group", new LuaCSFunction(UnityEngine_UI_ToggleWrap.get_group), new LuaCSFunction(UnityEngine_UI_ToggleWrap.set_group)), new LuaField("isOn", new LuaCSFunction(UnityEngine_UI_ToggleWrap.get_isOn), new LuaCSFunction(UnityEngine_UI_ToggleWrap.set_isOn)) };
        LuaScriptMgr.RegisterLib(L, "UnityEngine.UI.Toggle", typeof(Toggle), regs, fields, typeof(Selectable));
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_graphic(IntPtr L)
    {
        Toggle luaObject = (Toggle) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name graphic");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index graphic on a nil value");
            }
        }
        luaObject.graphic = LuaScriptMgr.GetUnityObject(L, 3, typeof(Graphic));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_group(IntPtr L)
    {
        Toggle luaObject = (Toggle) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name group");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index group on a nil value");
            }
        }
        luaObject.set_group((ToggleGroup) LuaScriptMgr.GetUnityObject(L, 3, typeof(ToggleGroup)));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_isOn(IntPtr L)
    {
        Toggle luaObject = (Toggle) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name isOn");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index isOn on a nil value");
            }
        }
        luaObject.set_isOn(LuaScriptMgr.GetBoolean(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_onValueChanged(IntPtr L)
    {
        Toggle luaObject = (Toggle) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name onValueChanged");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index onValueChanged on a nil value");
            }
        }
        luaObject.onValueChanged = (Toggle.ToggleEvent) LuaScriptMgr.GetNetObject(L, 3, typeof(Toggle.ToggleEvent));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_toggleTransition(IntPtr L)
    {
        Toggle luaObject = (Toggle) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name toggleTransition");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index toggleTransition on a nil value");
            }
        }
        luaObject.toggleTransition = (Toggle.ToggleTransition) ((int) LuaScriptMgr.GetNetObject(L, 3, typeof(Toggle.ToggleTransition)));
        return 0;
    }
}

