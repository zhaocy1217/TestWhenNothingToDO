using com.tencent.pandora;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnityEngine_UI_ScrollRectWrap
{
    private static Type classType = typeof(ScrollRect);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _CreateUnityEngine_UI_ScrollRect(IntPtr L)
    {
        LuaDLL.luaL_error(L, "UnityEngine.UI.ScrollRect class does not have a constructor function");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_content(IntPtr L)
    {
        ScrollRect luaObject = (ScrollRect) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name content");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index content on a nil value");
            }
        }
        LuaScriptMgr.Push(L, (Object) luaObject.get_content());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_decelerationRate(IntPtr L)
    {
        ScrollRect luaObject = (ScrollRect) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name decelerationRate");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index decelerationRate on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_decelerationRate());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_elasticity(IntPtr L)
    {
        ScrollRect luaObject = (ScrollRect) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name elasticity");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index elasticity on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_elasticity());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_horizontal(IntPtr L)
    {
        ScrollRect luaObject = (ScrollRect) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name horizontal");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index horizontal on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_horizontal());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_horizontalNormalizedPosition(IntPtr L)
    {
        ScrollRect luaObject = (ScrollRect) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name horizontalNormalizedPosition");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index horizontalNormalizedPosition on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_horizontalNormalizedPosition());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_horizontalScrollbar(IntPtr L)
    {
        ScrollRect luaObject = (ScrollRect) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name horizontalScrollbar");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index horizontalScrollbar on a nil value");
            }
        }
        LuaScriptMgr.Push(L, (Object) luaObject.get_horizontalScrollbar());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_inertia(IntPtr L)
    {
        ScrollRect luaObject = (ScrollRect) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name inertia");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index inertia on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_inertia());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_movementType(IntPtr L)
    {
        ScrollRect luaObject = (ScrollRect) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name movementType");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index movementType on a nil value");
            }
        }
        LuaScriptMgr.Push(L, (Enum) luaObject.get_movementType());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_normalizedPosition(IntPtr L)
    {
        ScrollRect luaObject = (ScrollRect) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name normalizedPosition");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index normalizedPosition on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_normalizedPosition());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_onValueChanged(IntPtr L)
    {
        ScrollRect luaObject = (ScrollRect) LuaScriptMgr.GetLuaObject(L, 1);
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
    private static int get_scrollSensitivity(IntPtr L)
    {
        ScrollRect luaObject = (ScrollRect) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name scrollSensitivity");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index scrollSensitivity on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_scrollSensitivity());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_velocity(IntPtr L)
    {
        ScrollRect luaObject = (ScrollRect) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name velocity");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index velocity on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_velocity());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_vertical(IntPtr L)
    {
        ScrollRect luaObject = (ScrollRect) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name vertical");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index vertical on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_vertical());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_verticalNormalizedPosition(IntPtr L)
    {
        ScrollRect luaObject = (ScrollRect) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name verticalNormalizedPosition");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index verticalNormalizedPosition on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_verticalNormalizedPosition());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_verticalScrollbar(IntPtr L)
    {
        ScrollRect luaObject = (ScrollRect) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name verticalScrollbar");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index verticalScrollbar on a nil value");
            }
        }
        LuaScriptMgr.Push(L, (Object) luaObject.get_verticalScrollbar());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetClassType(IntPtr L)
    {
        LuaScriptMgr.Push(L, classType);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int IsActive(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        bool b = ((ScrollRect) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.ScrollRect")).IsActive();
        LuaScriptMgr.Push(L, b);
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
        ScrollRect rect = (ScrollRect) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.ScrollRect");
        PointerEventData data = (PointerEventData) LuaScriptMgr.GetNetObject(L, 2, typeof(PointerEventData));
        rect.OnBeginDrag(data);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int OnDrag(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        ScrollRect rect = (ScrollRect) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.ScrollRect");
        PointerEventData data = (PointerEventData) LuaScriptMgr.GetNetObject(L, 2, typeof(PointerEventData));
        rect.OnDrag(data);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int OnEndDrag(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        ScrollRect rect = (ScrollRect) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.ScrollRect");
        PointerEventData data = (PointerEventData) LuaScriptMgr.GetNetObject(L, 2, typeof(PointerEventData));
        rect.OnEndDrag(data);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int OnInitializePotentialDrag(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        ScrollRect rect = (ScrollRect) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.ScrollRect");
        PointerEventData data = (PointerEventData) LuaScriptMgr.GetNetObject(L, 2, typeof(PointerEventData));
        rect.OnInitializePotentialDrag(data);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int OnScroll(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        ScrollRect rect = (ScrollRect) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.ScrollRect");
        PointerEventData data = (PointerEventData) LuaScriptMgr.GetNetObject(L, 2, typeof(PointerEventData));
        rect.OnScroll(data);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Rebuild(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        ScrollRect rect = (ScrollRect) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.ScrollRect");
        CanvasUpdate update = (CanvasUpdate) ((int) LuaScriptMgr.GetNetObject(L, 2, typeof(CanvasUpdate)));
        rect.Rebuild(update);
        return 0;
    }

    public static void Register(IntPtr L)
    {
        LuaMethod[] regs = new LuaMethod[] { new LuaMethod("Rebuild", new LuaCSFunction(UnityEngine_UI_ScrollRectWrap.Rebuild)), new LuaMethod("IsActive", new LuaCSFunction(UnityEngine_UI_ScrollRectWrap.IsActive)), new LuaMethod("StopMovement", new LuaCSFunction(UnityEngine_UI_ScrollRectWrap.StopMovement)), new LuaMethod("OnScroll", new LuaCSFunction(UnityEngine_UI_ScrollRectWrap.OnScroll)), new LuaMethod("OnInitializePotentialDrag", new LuaCSFunction(UnityEngine_UI_ScrollRectWrap.OnInitializePotentialDrag)), new LuaMethod("OnBeginDrag", new LuaCSFunction(UnityEngine_UI_ScrollRectWrap.OnBeginDrag)), new LuaMethod("OnEndDrag", new LuaCSFunction(UnityEngine_UI_ScrollRectWrap.OnEndDrag)), new LuaMethod("OnDrag", new LuaCSFunction(UnityEngine_UI_ScrollRectWrap.OnDrag)), new LuaMethod("New", new LuaCSFunction(UnityEngine_UI_ScrollRectWrap._CreateUnityEngine_UI_ScrollRect)), new LuaMethod("GetClassType", new LuaCSFunction(UnityEngine_UI_ScrollRectWrap.GetClassType)), new LuaMethod("__eq", new LuaCSFunction(UnityEngine_UI_ScrollRectWrap.Lua_Eq)) };
        LuaField[] fields = new LuaField[] { new LuaField("content", new LuaCSFunction(UnityEngine_UI_ScrollRectWrap.get_content), new LuaCSFunction(UnityEngine_UI_ScrollRectWrap.set_content)), new LuaField("horizontal", new LuaCSFunction(UnityEngine_UI_ScrollRectWrap.get_horizontal), new LuaCSFunction(UnityEngine_UI_ScrollRectWrap.set_horizontal)), new LuaField("vertical", new LuaCSFunction(UnityEngine_UI_ScrollRectWrap.get_vertical), new LuaCSFunction(UnityEngine_UI_ScrollRectWrap.set_vertical)), new LuaField("movementType", new LuaCSFunction(UnityEngine_UI_ScrollRectWrap.get_movementType), new LuaCSFunction(UnityEngine_UI_ScrollRectWrap.set_movementType)), new LuaField("elasticity", new LuaCSFunction(UnityEngine_UI_ScrollRectWrap.get_elasticity), new LuaCSFunction(UnityEngine_UI_ScrollRectWrap.set_elasticity)), new LuaField("inertia", new LuaCSFunction(UnityEngine_UI_ScrollRectWrap.get_inertia), new LuaCSFunction(UnityEngine_UI_ScrollRectWrap.set_inertia)), new LuaField("decelerationRate", new LuaCSFunction(UnityEngine_UI_ScrollRectWrap.get_decelerationRate), new LuaCSFunction(UnityEngine_UI_ScrollRectWrap.set_decelerationRate)), new LuaField("scrollSensitivity", new LuaCSFunction(UnityEngine_UI_ScrollRectWrap.get_scrollSensitivity), new LuaCSFunction(UnityEngine_UI_ScrollRectWrap.set_scrollSensitivity)), new LuaField("horizontalScrollbar", new LuaCSFunction(UnityEngine_UI_ScrollRectWrap.get_horizontalScrollbar), new LuaCSFunction(UnityEngine_UI_ScrollRectWrap.set_horizontalScrollbar)), new LuaField("verticalScrollbar", new LuaCSFunction(UnityEngine_UI_ScrollRectWrap.get_verticalScrollbar), new LuaCSFunction(UnityEngine_UI_ScrollRectWrap.set_verticalScrollbar)), new LuaField("onValueChanged", new LuaCSFunction(UnityEngine_UI_ScrollRectWrap.get_onValueChanged), new LuaCSFunction(UnityEngine_UI_ScrollRectWrap.set_onValueChanged)), new LuaField("velocity", new LuaCSFunction(UnityEngine_UI_ScrollRectWrap.get_velocity), new LuaCSFunction(UnityEngine_UI_ScrollRectWrap.set_velocity)), new LuaField("normalizedPosition", new LuaCSFunction(UnityEngine_UI_ScrollRectWrap.get_normalizedPosition), new LuaCSFunction(UnityEngine_UI_ScrollRectWrap.set_normalizedPosition)), new LuaField("horizontalNormalizedPosition", new LuaCSFunction(UnityEngine_UI_ScrollRectWrap.get_horizontalNormalizedPosition), new LuaCSFunction(UnityEngine_UI_ScrollRectWrap.set_horizontalNormalizedPosition)), new LuaField("verticalNormalizedPosition", new LuaCSFunction(UnityEngine_UI_ScrollRectWrap.get_verticalNormalizedPosition), new LuaCSFunction(UnityEngine_UI_ScrollRectWrap.set_verticalNormalizedPosition)) };
        LuaScriptMgr.RegisterLib(L, "UnityEngine.UI.ScrollRect", typeof(ScrollRect), regs, fields, typeof(UIBehaviour));
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_content(IntPtr L)
    {
        ScrollRect luaObject = (ScrollRect) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name content");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index content on a nil value");
            }
        }
        luaObject.set_content((RectTransform) LuaScriptMgr.GetUnityObject(L, 3, typeof(RectTransform)));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_decelerationRate(IntPtr L)
    {
        ScrollRect luaObject = (ScrollRect) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name decelerationRate");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index decelerationRate on a nil value");
            }
        }
        luaObject.set_decelerationRate((float) LuaScriptMgr.GetNumber(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_elasticity(IntPtr L)
    {
        ScrollRect luaObject = (ScrollRect) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name elasticity");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index elasticity on a nil value");
            }
        }
        luaObject.set_elasticity((float) LuaScriptMgr.GetNumber(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_horizontal(IntPtr L)
    {
        ScrollRect luaObject = (ScrollRect) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name horizontal");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index horizontal on a nil value");
            }
        }
        luaObject.set_horizontal(LuaScriptMgr.GetBoolean(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_horizontalNormalizedPosition(IntPtr L)
    {
        ScrollRect luaObject = (ScrollRect) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name horizontalNormalizedPosition");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index horizontalNormalizedPosition on a nil value");
            }
        }
        luaObject.set_horizontalNormalizedPosition((float) LuaScriptMgr.GetNumber(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_horizontalScrollbar(IntPtr L)
    {
        ScrollRect luaObject = (ScrollRect) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name horizontalScrollbar");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index horizontalScrollbar on a nil value");
            }
        }
        luaObject.set_horizontalScrollbar((Scrollbar) LuaScriptMgr.GetUnityObject(L, 3, typeof(Scrollbar)));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_inertia(IntPtr L)
    {
        ScrollRect luaObject = (ScrollRect) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name inertia");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index inertia on a nil value");
            }
        }
        luaObject.set_inertia(LuaScriptMgr.GetBoolean(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_movementType(IntPtr L)
    {
        ScrollRect luaObject = (ScrollRect) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name movementType");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index movementType on a nil value");
            }
        }
        luaObject.set_movementType((ScrollRect.MovementType) ((int) LuaScriptMgr.GetNetObject(L, 3, typeof(ScrollRect.MovementType))));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_normalizedPosition(IntPtr L)
    {
        ScrollRect luaObject = (ScrollRect) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name normalizedPosition");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index normalizedPosition on a nil value");
            }
        }
        luaObject.set_normalizedPosition(LuaScriptMgr.GetVector2(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_onValueChanged(IntPtr L)
    {
        ScrollRect luaObject = (ScrollRect) LuaScriptMgr.GetLuaObject(L, 1);
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
        luaObject.set_onValueChanged((ScrollRect.ScrollRectEvent) LuaScriptMgr.GetNetObject(L, 3, typeof(ScrollRect.ScrollRectEvent)));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_scrollSensitivity(IntPtr L)
    {
        ScrollRect luaObject = (ScrollRect) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name scrollSensitivity");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index scrollSensitivity on a nil value");
            }
        }
        luaObject.set_scrollSensitivity((float) LuaScriptMgr.GetNumber(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_velocity(IntPtr L)
    {
        ScrollRect luaObject = (ScrollRect) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name velocity");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index velocity on a nil value");
            }
        }
        luaObject.set_velocity(LuaScriptMgr.GetVector2(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_vertical(IntPtr L)
    {
        ScrollRect luaObject = (ScrollRect) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name vertical");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index vertical on a nil value");
            }
        }
        luaObject.set_vertical(LuaScriptMgr.GetBoolean(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_verticalNormalizedPosition(IntPtr L)
    {
        ScrollRect luaObject = (ScrollRect) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name verticalNormalizedPosition");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index verticalNormalizedPosition on a nil value");
            }
        }
        luaObject.set_verticalNormalizedPosition((float) LuaScriptMgr.GetNumber(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_verticalScrollbar(IntPtr L)
    {
        ScrollRect luaObject = (ScrollRect) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name verticalScrollbar");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index verticalScrollbar on a nil value");
            }
        }
        luaObject.set_verticalScrollbar((Scrollbar) LuaScriptMgr.GetUnityObject(L, 3, typeof(Scrollbar)));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int StopMovement(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        ((ScrollRect) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.ScrollRect")).StopMovement();
        return 0;
    }
}

