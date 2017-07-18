using com.tencent.pandora;
using System;
using System.Collections.Generic;
using UnityEngine;

public class com_tencent_pandora_PanelManagerWrap
{
    private static Type classType = typeof(PanelManager);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _Createcom_tencent_pandora_PanelManager(IntPtr L)
    {
        LuaDLL.luaL_error(L, "com.tencent.pandora.PanelManager class does not have a constructor function");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int ClosePanel(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        PanelManager manager = (PanelManager) LuaScriptMgr.GetUnityObjectSelf(L, 1, "com.tencent.pandora.PanelManager");
        string luaString = LuaScriptMgr.GetLuaString(L, 2);
        manager.ClosePanel(luaString);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int CreatePanel(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 3);
        PanelManager manager = (PanelManager) LuaScriptMgr.GetUnityObjectSelf(L, 1, "com.tencent.pandora.PanelManager");
        string luaString = LuaScriptMgr.GetLuaString(L, 2);
        LuaFunction luaFunction = LuaScriptMgr.GetLuaFunction(L, 3);
        manager.CreatePanel(luaString, luaFunction);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_pages(IntPtr L)
    {
        PanelManager luaObject = (PanelManager) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name pages");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index pages on a nil value");
            }
        }
        LuaScriptMgr.PushObject(L, luaObject.pages);
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
        LuaMethod[] regs = new LuaMethod[] { new LuaMethod("CreatePanel", new LuaCSFunction(com_tencent_pandora_PanelManagerWrap.CreatePanel)), new LuaMethod("ClosePanel", new LuaCSFunction(com_tencent_pandora_PanelManagerWrap.ClosePanel)), new LuaMethod("New", new LuaCSFunction(com_tencent_pandora_PanelManagerWrap._Createcom_tencent_pandora_PanelManager)), new LuaMethod("GetClassType", new LuaCSFunction(com_tencent_pandora_PanelManagerWrap.GetClassType)), new LuaMethod("__eq", new LuaCSFunction(com_tencent_pandora_PanelManagerWrap.Lua_Eq)) };
        LuaField[] fields = new LuaField[] { new LuaField("pages", new LuaCSFunction(com_tencent_pandora_PanelManagerWrap.get_pages), new LuaCSFunction(com_tencent_pandora_PanelManagerWrap.set_pages)) };
        LuaScriptMgr.RegisterLib(L, "com.tencent.pandora.PanelManager", typeof(PanelManager), regs, fields, typeof(View));
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_pages(IntPtr L)
    {
        PanelManager luaObject = (PanelManager) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name pages");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index pages on a nil value");
            }
        }
        luaObject.pages = (Dictionary<string, GameObject>) LuaScriptMgr.GetNetObject(L, 3, typeof(Dictionary<string, GameObject>));
        return 0;
    }
}

