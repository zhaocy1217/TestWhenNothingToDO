using com.tencent.pandora;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UnityEngine_UI_VerticalLayoutGroupWrap
{
    private static Type classType = typeof(VerticalLayoutGroup);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _CreateUnityEngine_UI_VerticalLayoutGroup(IntPtr L)
    {
        LuaDLL.luaL_error(L, "UnityEngine.UI.VerticalLayoutGroup class does not have a constructor function");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int CalculateLayoutInputHorizontal(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        ((VerticalLayoutGroup) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.VerticalLayoutGroup")).CalculateLayoutInputHorizontal();
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int CalculateLayoutInputVertical(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        ((VerticalLayoutGroup) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.VerticalLayoutGroup")).CalculateLayoutInputVertical();
        return 0;
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
        LuaMethod[] regs = new LuaMethod[] { new LuaMethod("CalculateLayoutInputHorizontal", new LuaCSFunction(UnityEngine_UI_VerticalLayoutGroupWrap.CalculateLayoutInputHorizontal)), new LuaMethod("CalculateLayoutInputVertical", new LuaCSFunction(UnityEngine_UI_VerticalLayoutGroupWrap.CalculateLayoutInputVertical)), new LuaMethod("SetLayoutHorizontal", new LuaCSFunction(UnityEngine_UI_VerticalLayoutGroupWrap.SetLayoutHorizontal)), new LuaMethod("SetLayoutVertical", new LuaCSFunction(UnityEngine_UI_VerticalLayoutGroupWrap.SetLayoutVertical)), new LuaMethod("New", new LuaCSFunction(UnityEngine_UI_VerticalLayoutGroupWrap._CreateUnityEngine_UI_VerticalLayoutGroup)), new LuaMethod("GetClassType", new LuaCSFunction(UnityEngine_UI_VerticalLayoutGroupWrap.GetClassType)), new LuaMethod("__eq", new LuaCSFunction(UnityEngine_UI_VerticalLayoutGroupWrap.Lua_Eq)) };
        LuaField[] fields = new LuaField[0];
        LuaScriptMgr.RegisterLib(L, "UnityEngine.UI.VerticalLayoutGroup", typeof(VerticalLayoutGroup), regs, fields, typeof(HorizontalOrVerticalLayoutGroup));
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int SetLayoutHorizontal(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        ((VerticalLayoutGroup) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.VerticalLayoutGroup")).SetLayoutHorizontal();
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int SetLayoutVertical(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        ((VerticalLayoutGroup) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.VerticalLayoutGroup")).SetLayoutVertical();
        return 0;
    }
}

