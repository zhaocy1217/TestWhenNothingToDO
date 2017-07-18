using com.tencent.pandora;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnityEngine_UI_ButtonWrap
{
    private static Type classType = typeof(Button);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _CreateUnityEngine_UI_Button(IntPtr L)
    {
        LuaDLL.luaL_error(L, "UnityEngine.UI.Button class does not have a constructor function");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_onClick(IntPtr L)
    {
        Button luaObject = (Button) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name onClick");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index onClick on a nil value");
            }
        }
        LuaScriptMgr.PushObject(L, luaObject.get_onClick());
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
        Button button = (Button) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Button");
        PointerEventData data = (PointerEventData) LuaScriptMgr.GetNetObject(L, 2, typeof(PointerEventData));
        button.OnPointerClick(data);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int OnSubmit(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Button button = (Button) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Button");
        BaseEventData data = (BaseEventData) LuaScriptMgr.GetNetObject(L, 2, typeof(BaseEventData));
        button.OnSubmit(data);
        return 0;
    }

    public static void Register(IntPtr L)
    {
        LuaMethod[] regs = new LuaMethod[] { new LuaMethod("OnPointerClick", new LuaCSFunction(UnityEngine_UI_ButtonWrap.OnPointerClick)), new LuaMethod("OnSubmit", new LuaCSFunction(UnityEngine_UI_ButtonWrap.OnSubmit)), new LuaMethod("New", new LuaCSFunction(UnityEngine_UI_ButtonWrap._CreateUnityEngine_UI_Button)), new LuaMethod("GetClassType", new LuaCSFunction(UnityEngine_UI_ButtonWrap.GetClassType)), new LuaMethod("__eq", new LuaCSFunction(UnityEngine_UI_ButtonWrap.Lua_Eq)) };
        LuaField[] fields = new LuaField[] { new LuaField("onClick", new LuaCSFunction(UnityEngine_UI_ButtonWrap.get_onClick), new LuaCSFunction(UnityEngine_UI_ButtonWrap.set_onClick)) };
        LuaScriptMgr.RegisterLib(L, "UnityEngine.UI.Button", typeof(Button), regs, fields, typeof(Selectable));
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_onClick(IntPtr L)
    {
        Button luaObject = (Button) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name onClick");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index onClick on a nil value");
            }
        }
        luaObject.set_onClick((Button.ButtonClickedEvent) LuaScriptMgr.GetNetObject(L, 3, typeof(Button.ButtonClickedEvent)));
        return 0;
    }
}

