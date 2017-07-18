using com.tencent.pandora;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnityEngine_UI_SelectableWrap
{
    private static Type classType = typeof(Selectable);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _CreateUnityEngine_UI_Selectable(IntPtr L)
    {
        LuaDLL.luaL_error(L, "UnityEngine.UI.Selectable class does not have a constructor function");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int FindSelectable(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Selectable selectable = (Selectable) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Selectable");
        Vector3 vector = LuaScriptMgr.GetVector3(L, 2);
        Selectable selectable2 = selectable.FindSelectable(vector);
        LuaScriptMgr.Push(L, (Object) selectable2);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int FindSelectableOnDown(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        Selectable selectable2 = ((Selectable) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Selectable")).FindSelectableOnDown();
        LuaScriptMgr.Push(L, (Object) selectable2);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int FindSelectableOnLeft(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        Selectable selectable2 = ((Selectable) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Selectable")).FindSelectableOnLeft();
        LuaScriptMgr.Push(L, (Object) selectable2);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int FindSelectableOnRight(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        Selectable selectable2 = ((Selectable) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Selectable")).FindSelectableOnRight();
        LuaScriptMgr.Push(L, (Object) selectable2);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int FindSelectableOnUp(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        Selectable selectable2 = ((Selectable) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Selectable")).FindSelectableOnUp();
        LuaScriptMgr.Push(L, (Object) selectable2);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_allSelectables(IntPtr L)
    {
        LuaScriptMgr.PushObject(L, Selectable.get_allSelectables());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_animationTriggers(IntPtr L)
    {
        Selectable luaObject = (Selectable) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name animationTriggers");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index animationTriggers on a nil value");
            }
        }
        LuaScriptMgr.PushObject(L, luaObject.get_animationTriggers());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_animator(IntPtr L)
    {
        Selectable luaObject = (Selectable) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name animator");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index animator on a nil value");
            }
        }
        LuaScriptMgr.Push(L, (Object) luaObject.get_animator());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_colors(IntPtr L)
    {
        Selectable luaObject = (Selectable) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name colors");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index colors on a nil value");
            }
        }
        LuaScriptMgr.PushValue(L, luaObject.get_colors());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_image(IntPtr L)
    {
        Selectable luaObject = (Selectable) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name image");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index image on a nil value");
            }
        }
        LuaScriptMgr.Push(L, (Object) luaObject.get_image());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_interactable(IntPtr L)
    {
        Selectable luaObject = (Selectable) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name interactable");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index interactable on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_interactable());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_navigation(IntPtr L)
    {
        Selectable luaObject = (Selectable) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name navigation");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index navigation on a nil value");
            }
        }
        LuaScriptMgr.PushValue(L, luaObject.get_navigation());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_spriteState(IntPtr L)
    {
        Selectable luaObject = (Selectable) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name spriteState");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index spriteState on a nil value");
            }
        }
        LuaScriptMgr.PushValue(L, luaObject.get_spriteState());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_targetGraphic(IntPtr L)
    {
        Selectable luaObject = (Selectable) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name targetGraphic");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index targetGraphic on a nil value");
            }
        }
        LuaScriptMgr.Push(L, (Object) luaObject.get_targetGraphic());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_transition(IntPtr L)
    {
        Selectable luaObject = (Selectable) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name transition");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index transition on a nil value");
            }
        }
        LuaScriptMgr.Push(L, (Enum) luaObject.get_transition());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetClassType(IntPtr L)
    {
        LuaScriptMgr.Push(L, classType);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int IsInteractable(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        bool b = ((Selectable) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Selectable")).IsInteractable();
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
    private static int OnDeselect(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Selectable selectable = (Selectable) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Selectable");
        BaseEventData data = (BaseEventData) LuaScriptMgr.GetNetObject(L, 2, typeof(BaseEventData));
        selectable.OnDeselect(data);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int OnMove(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Selectable selectable = (Selectable) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Selectable");
        AxisEventData data = (AxisEventData) LuaScriptMgr.GetNetObject(L, 2, typeof(AxisEventData));
        selectable.OnMove(data);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int OnPointerDown(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Selectable selectable = (Selectable) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Selectable");
        PointerEventData data = (PointerEventData) LuaScriptMgr.GetNetObject(L, 2, typeof(PointerEventData));
        selectable.OnPointerDown(data);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int OnPointerEnter(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Selectable selectable = (Selectable) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Selectable");
        PointerEventData data = (PointerEventData) LuaScriptMgr.GetNetObject(L, 2, typeof(PointerEventData));
        selectable.OnPointerEnter(data);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int OnPointerExit(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Selectable selectable = (Selectable) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Selectable");
        PointerEventData data = (PointerEventData) LuaScriptMgr.GetNetObject(L, 2, typeof(PointerEventData));
        selectable.OnPointerExit(data);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int OnPointerUp(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Selectable selectable = (Selectable) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Selectable");
        PointerEventData data = (PointerEventData) LuaScriptMgr.GetNetObject(L, 2, typeof(PointerEventData));
        selectable.OnPointerUp(data);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int OnSelect(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Selectable selectable = (Selectable) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Selectable");
        BaseEventData data = (BaseEventData) LuaScriptMgr.GetNetObject(L, 2, typeof(BaseEventData));
        selectable.OnSelect(data);
        return 0;
    }

    public static void Register(IntPtr L)
    {
        LuaMethod[] regs = new LuaMethod[] { 
            new LuaMethod("IsInteractable", new LuaCSFunction(UnityEngine_UI_SelectableWrap.IsInteractable)), new LuaMethod("FindSelectable", new LuaCSFunction(UnityEngine_UI_SelectableWrap.FindSelectable)), new LuaMethod("FindSelectableOnLeft", new LuaCSFunction(UnityEngine_UI_SelectableWrap.FindSelectableOnLeft)), new LuaMethod("FindSelectableOnRight", new LuaCSFunction(UnityEngine_UI_SelectableWrap.FindSelectableOnRight)), new LuaMethod("FindSelectableOnUp", new LuaCSFunction(UnityEngine_UI_SelectableWrap.FindSelectableOnUp)), new LuaMethod("FindSelectableOnDown", new LuaCSFunction(UnityEngine_UI_SelectableWrap.FindSelectableOnDown)), new LuaMethod("OnMove", new LuaCSFunction(UnityEngine_UI_SelectableWrap.OnMove)), new LuaMethod("OnPointerDown", new LuaCSFunction(UnityEngine_UI_SelectableWrap.OnPointerDown)), new LuaMethod("OnPointerUp", new LuaCSFunction(UnityEngine_UI_SelectableWrap.OnPointerUp)), new LuaMethod("OnPointerEnter", new LuaCSFunction(UnityEngine_UI_SelectableWrap.OnPointerEnter)), new LuaMethod("OnPointerExit", new LuaCSFunction(UnityEngine_UI_SelectableWrap.OnPointerExit)), new LuaMethod("OnSelect", new LuaCSFunction(UnityEngine_UI_SelectableWrap.OnSelect)), new LuaMethod("OnDeselect", new LuaCSFunction(UnityEngine_UI_SelectableWrap.OnDeselect)), new LuaMethod("Select", new LuaCSFunction(UnityEngine_UI_SelectableWrap.Select)), new LuaMethod("New", new LuaCSFunction(UnityEngine_UI_SelectableWrap._CreateUnityEngine_UI_Selectable)), new LuaMethod("GetClassType", new LuaCSFunction(UnityEngine_UI_SelectableWrap.GetClassType)), 
            new LuaMethod("__eq", new LuaCSFunction(UnityEngine_UI_SelectableWrap.Lua_Eq))
         };
        LuaField[] fields = new LuaField[] { new LuaField("allSelectables", new LuaCSFunction(UnityEngine_UI_SelectableWrap.get_allSelectables), null), new LuaField("navigation", new LuaCSFunction(UnityEngine_UI_SelectableWrap.get_navigation), new LuaCSFunction(UnityEngine_UI_SelectableWrap.set_navigation)), new LuaField("transition", new LuaCSFunction(UnityEngine_UI_SelectableWrap.get_transition), new LuaCSFunction(UnityEngine_UI_SelectableWrap.set_transition)), new LuaField("colors", new LuaCSFunction(UnityEngine_UI_SelectableWrap.get_colors), new LuaCSFunction(UnityEngine_UI_SelectableWrap.set_colors)), new LuaField("spriteState", new LuaCSFunction(UnityEngine_UI_SelectableWrap.get_spriteState), new LuaCSFunction(UnityEngine_UI_SelectableWrap.set_spriteState)), new LuaField("animationTriggers", new LuaCSFunction(UnityEngine_UI_SelectableWrap.get_animationTriggers), new LuaCSFunction(UnityEngine_UI_SelectableWrap.set_animationTriggers)), new LuaField("targetGraphic", new LuaCSFunction(UnityEngine_UI_SelectableWrap.get_targetGraphic), new LuaCSFunction(UnityEngine_UI_SelectableWrap.set_targetGraphic)), new LuaField("interactable", new LuaCSFunction(UnityEngine_UI_SelectableWrap.get_interactable), new LuaCSFunction(UnityEngine_UI_SelectableWrap.set_interactable)), new LuaField("image", new LuaCSFunction(UnityEngine_UI_SelectableWrap.get_image), new LuaCSFunction(UnityEngine_UI_SelectableWrap.set_image)), new LuaField("animator", new LuaCSFunction(UnityEngine_UI_SelectableWrap.get_animator), null) };
        LuaScriptMgr.RegisterLib(L, "UnityEngine.UI.Selectable", typeof(Selectable), regs, fields, typeof(UIBehaviour));
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Select(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        ((Selectable) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Selectable")).Select();
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_animationTriggers(IntPtr L)
    {
        Selectable luaObject = (Selectable) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name animationTriggers");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index animationTriggers on a nil value");
            }
        }
        luaObject.set_animationTriggers((AnimationTriggers) LuaScriptMgr.GetNetObject(L, 3, typeof(AnimationTriggers)));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_colors(IntPtr L)
    {
        Selectable luaObject = (Selectable) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name colors");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index colors on a nil value");
            }
        }
        luaObject.set_colors((ColorBlock) LuaScriptMgr.GetNetObject(L, 3, typeof(ColorBlock)));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_image(IntPtr L)
    {
        Selectable luaObject = (Selectable) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name image");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index image on a nil value");
            }
        }
        luaObject.set_image((Image) LuaScriptMgr.GetUnityObject(L, 3, typeof(Image)));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_interactable(IntPtr L)
    {
        Selectable luaObject = (Selectable) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name interactable");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index interactable on a nil value");
            }
        }
        luaObject.set_interactable(LuaScriptMgr.GetBoolean(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_navigation(IntPtr L)
    {
        Selectable luaObject = (Selectable) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name navigation");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index navigation on a nil value");
            }
        }
        luaObject.set_navigation((Navigation) LuaScriptMgr.GetNetObject(L, 3, typeof(Navigation)));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_spriteState(IntPtr L)
    {
        Selectable luaObject = (Selectable) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name spriteState");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index spriteState on a nil value");
            }
        }
        luaObject.set_spriteState((SpriteState) LuaScriptMgr.GetNetObject(L, 3, typeof(SpriteState)));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_targetGraphic(IntPtr L)
    {
        Selectable luaObject = (Selectable) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name targetGraphic");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index targetGraphic on a nil value");
            }
        }
        luaObject.set_targetGraphic((Graphic) LuaScriptMgr.GetUnityObject(L, 3, typeof(Graphic)));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_transition(IntPtr L)
    {
        Selectable luaObject = (Selectable) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name transition");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index transition on a nil value");
            }
        }
        luaObject.set_transition((Selectable.Transition) ((int) LuaScriptMgr.GetNetObject(L, 3, typeof(Selectable.Transition))));
        return 0;
    }
}

