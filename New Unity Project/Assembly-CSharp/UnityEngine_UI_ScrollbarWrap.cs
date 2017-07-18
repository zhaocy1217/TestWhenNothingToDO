using com.tencent.pandora;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnityEngine_UI_ScrollbarWrap
{
    private static Type classType = typeof(Scrollbar);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _CreateUnityEngine_UI_Scrollbar(IntPtr L)
    {
        LuaDLL.luaL_error(L, "UnityEngine.UI.Scrollbar class does not have a constructor function");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int FindSelectableOnDown(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        Selectable selectable = ((Scrollbar) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Scrollbar")).FindSelectableOnDown();
        LuaScriptMgr.Push(L, (Object) selectable);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int FindSelectableOnLeft(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        Selectable selectable = ((Scrollbar) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Scrollbar")).FindSelectableOnLeft();
        LuaScriptMgr.Push(L, (Object) selectable);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int FindSelectableOnRight(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        Selectable selectable = ((Scrollbar) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Scrollbar")).FindSelectableOnRight();
        LuaScriptMgr.Push(L, (Object) selectable);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int FindSelectableOnUp(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        Selectable selectable = ((Scrollbar) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Scrollbar")).FindSelectableOnUp();
        LuaScriptMgr.Push(L, (Object) selectable);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_direction(IntPtr L)
    {
        Scrollbar luaObject = (Scrollbar) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name direction");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index direction on a nil value");
            }
        }
        LuaScriptMgr.Push(L, (Enum) luaObject.get_direction());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_handleRect(IntPtr L)
    {
        Scrollbar luaObject = (Scrollbar) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name handleRect");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index handleRect on a nil value");
            }
        }
        LuaScriptMgr.Push(L, (Object) luaObject.get_handleRect());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_numberOfSteps(IntPtr L)
    {
        Scrollbar luaObject = (Scrollbar) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name numberOfSteps");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index numberOfSteps on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_numberOfSteps());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_onValueChanged(IntPtr L)
    {
        Scrollbar luaObject = (Scrollbar) LuaScriptMgr.GetLuaObject(L, 1);
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
        LuaScriptMgr.PushObject(L, luaObject.get_onValueChanged());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_size(IntPtr L)
    {
        Scrollbar luaObject = (Scrollbar) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name size");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index size on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_size());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_value(IntPtr L)
    {
        Scrollbar luaObject = (Scrollbar) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name value");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index value on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_value());
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
    private static int OnBeginDrag(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Scrollbar scrollbar = (Scrollbar) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Scrollbar");
        PointerEventData data = (PointerEventData) LuaScriptMgr.GetNetObject(L, 2, typeof(PointerEventData));
        scrollbar.OnBeginDrag(data);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int OnDrag(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Scrollbar scrollbar = (Scrollbar) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Scrollbar");
        PointerEventData data = (PointerEventData) LuaScriptMgr.GetNetObject(L, 2, typeof(PointerEventData));
        scrollbar.OnDrag(data);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int OnInitializePotentialDrag(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Scrollbar scrollbar = (Scrollbar) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Scrollbar");
        PointerEventData data = (PointerEventData) LuaScriptMgr.GetNetObject(L, 2, typeof(PointerEventData));
        scrollbar.OnInitializePotentialDrag(data);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int OnMove(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Scrollbar scrollbar = (Scrollbar) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Scrollbar");
        AxisEventData data = (AxisEventData) LuaScriptMgr.GetNetObject(L, 2, typeof(AxisEventData));
        scrollbar.OnMove(data);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int OnPointerDown(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Scrollbar scrollbar = (Scrollbar) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Scrollbar");
        PointerEventData data = (PointerEventData) LuaScriptMgr.GetNetObject(L, 2, typeof(PointerEventData));
        scrollbar.OnPointerDown(data);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int OnPointerUp(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Scrollbar scrollbar = (Scrollbar) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Scrollbar");
        PointerEventData data = (PointerEventData) LuaScriptMgr.GetNetObject(L, 2, typeof(PointerEventData));
        scrollbar.OnPointerUp(data);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Rebuild(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Scrollbar scrollbar = (Scrollbar) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Scrollbar");
        CanvasUpdate update = (CanvasUpdate) ((int) LuaScriptMgr.GetNetObject(L, 2, typeof(CanvasUpdate)));
        scrollbar.Rebuild(update);
        return 0;
    }

    public static void Register(IntPtr L)
    {
        LuaMethod[] regs = new LuaMethod[] { new LuaMethod("Rebuild", new LuaCSFunction(UnityEngine_UI_ScrollbarWrap.Rebuild)), new LuaMethod("OnBeginDrag", new LuaCSFunction(UnityEngine_UI_ScrollbarWrap.OnBeginDrag)), new LuaMethod("OnDrag", new LuaCSFunction(UnityEngine_UI_ScrollbarWrap.OnDrag)), new LuaMethod("OnPointerDown", new LuaCSFunction(UnityEngine_UI_ScrollbarWrap.OnPointerDown)), new LuaMethod("OnPointerUp", new LuaCSFunction(UnityEngine_UI_ScrollbarWrap.OnPointerUp)), new LuaMethod("OnMove", new LuaCSFunction(UnityEngine_UI_ScrollbarWrap.OnMove)), new LuaMethod("FindSelectableOnLeft", new LuaCSFunction(UnityEngine_UI_ScrollbarWrap.FindSelectableOnLeft)), new LuaMethod("FindSelectableOnRight", new LuaCSFunction(UnityEngine_UI_ScrollbarWrap.FindSelectableOnRight)), new LuaMethod("FindSelectableOnUp", new LuaCSFunction(UnityEngine_UI_ScrollbarWrap.FindSelectableOnUp)), new LuaMethod("FindSelectableOnDown", new LuaCSFunction(UnityEngine_UI_ScrollbarWrap.FindSelectableOnDown)), new LuaMethod("OnInitializePotentialDrag", new LuaCSFunction(UnityEngine_UI_ScrollbarWrap.OnInitializePotentialDrag)), new LuaMethod("SetDirection", new LuaCSFunction(UnityEngine_UI_ScrollbarWrap.SetDirection)), new LuaMethod("New", new LuaCSFunction(UnityEngine_UI_ScrollbarWrap._CreateUnityEngine_UI_Scrollbar)), new LuaMethod("GetClassType", new LuaCSFunction(UnityEngine_UI_ScrollbarWrap.GetClassType)), new LuaMethod("__eq", new LuaCSFunction(UnityEngine_UI_ScrollbarWrap.Lua_Eq)) };
        LuaField[] fields = new LuaField[] { new LuaField("handleRect", new LuaCSFunction(UnityEngine_UI_ScrollbarWrap.get_handleRect), new LuaCSFunction(UnityEngine_UI_ScrollbarWrap.set_handleRect)), new LuaField("direction", new LuaCSFunction(UnityEngine_UI_ScrollbarWrap.get_direction), new LuaCSFunction(UnityEngine_UI_ScrollbarWrap.set_direction)), new LuaField("value", new LuaCSFunction(UnityEngine_UI_ScrollbarWrap.get_value), new LuaCSFunction(UnityEngine_UI_ScrollbarWrap.set_value)), new LuaField("size", new LuaCSFunction(UnityEngine_UI_ScrollbarWrap.get_size), new LuaCSFunction(UnityEngine_UI_ScrollbarWrap.set_size)), new LuaField("numberOfSteps", new LuaCSFunction(UnityEngine_UI_ScrollbarWrap.get_numberOfSteps), new LuaCSFunction(UnityEngine_UI_ScrollbarWrap.set_numberOfSteps)), new LuaField("onValueChanged", new LuaCSFunction(UnityEngine_UI_ScrollbarWrap.get_onValueChanged), new LuaCSFunction(UnityEngine_UI_ScrollbarWrap.set_onValueChanged)) };
        LuaScriptMgr.RegisterLib(L, "UnityEngine.UI.Scrollbar", typeof(Scrollbar), regs, fields, typeof(Selectable));
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_direction(IntPtr L)
    {
        Scrollbar luaObject = (Scrollbar) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name direction");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index direction on a nil value");
            }
        }
        luaObject.set_direction((Scrollbar.Direction) ((int) LuaScriptMgr.GetNetObject(L, 3, typeof(Scrollbar.Direction))));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_handleRect(IntPtr L)
    {
        Scrollbar luaObject = (Scrollbar) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name handleRect");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index handleRect on a nil value");
            }
        }
        luaObject.set_handleRect((RectTransform) LuaScriptMgr.GetUnityObject(L, 3, typeof(RectTransform)));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_numberOfSteps(IntPtr L)
    {
        Scrollbar luaObject = (Scrollbar) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name numberOfSteps");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index numberOfSteps on a nil value");
            }
        }
        luaObject.set_numberOfSteps((int) LuaScriptMgr.GetNumber(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_onValueChanged(IntPtr L)
    {
        Scrollbar luaObject = (Scrollbar) LuaScriptMgr.GetLuaObject(L, 1);
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
        luaObject.set_onValueChanged((Scrollbar.ScrollEvent) LuaScriptMgr.GetNetObject(L, 3, typeof(Scrollbar.ScrollEvent)));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_size(IntPtr L)
    {
        Scrollbar luaObject = (Scrollbar) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name size");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index size on a nil value");
            }
        }
        luaObject.set_size((float) LuaScriptMgr.GetNumber(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_value(IntPtr L)
    {
        Scrollbar luaObject = (Scrollbar) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name value");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index value on a nil value");
            }
        }
        luaObject.set_value((float) LuaScriptMgr.GetNumber(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int SetDirection(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 3);
        Scrollbar scrollbar = (Scrollbar) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Scrollbar");
        Scrollbar.Direction direction = (Scrollbar.Direction) ((int) LuaScriptMgr.GetNetObject(L, 2, typeof(Scrollbar.Direction)));
        bool boolean = LuaScriptMgr.GetBoolean(L, 3);
        scrollbar.SetDirection(direction, boolean);
        return 0;
    }
}

