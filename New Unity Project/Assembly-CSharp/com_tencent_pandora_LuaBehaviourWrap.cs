using com.tencent.pandora;
using System;
using UnityEngine;

public class com_tencent_pandora_LuaBehaviourWrap
{
    private static Type classType = typeof(LuaBehaviour);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _Createcom_tencent_pandora_LuaBehaviour(IntPtr L)
    {
        LuaDLL.luaL_error(L, "com.tencent.pandora.LuaBehaviour class does not have a constructor function");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int AddClick(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 3);
        LuaBehaviour behaviour = (LuaBehaviour) LuaScriptMgr.GetUnityObjectSelf(L, 1, "com.tencent.pandora.LuaBehaviour");
        GameObject go = LuaScriptMgr.GetUnityObject(L, 2, typeof(GameObject));
        LuaFunction luaFunction = LuaScriptMgr.GetLuaFunction(L, 3);
        behaviour.AddClick(go, luaFunction);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int AddUGUIClick(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 3);
        LuaBehaviour behaviour = (LuaBehaviour) LuaScriptMgr.GetUnityObjectSelf(L, 1, "com.tencent.pandora.LuaBehaviour");
        GameObject go = LuaScriptMgr.GetUnityObject(L, 2, typeof(GameObject));
        LuaFunction luaFunction = LuaScriptMgr.GetLuaFunction(L, 3);
        behaviour.AddUGUIClick(go, luaFunction);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int ClearClick(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        ((LuaBehaviour) LuaScriptMgr.GetUnityObjectSelf(L, 1, "com.tencent.pandora.LuaBehaviour")).ClearClick();
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetClassType(IntPtr L)
    {
        LuaScriptMgr.Push(L, classType);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetGameObject(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        LuaBehaviour behaviour = (LuaBehaviour) LuaScriptMgr.GetUnityObjectSelf(L, 1, "com.tencent.pandora.LuaBehaviour");
        string luaString = LuaScriptMgr.GetLuaString(L, 2);
        GameObject gameObject = behaviour.GetGameObject(luaString);
        LuaScriptMgr.Push(L, (Object) gameObject);
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
    private static int OnInit(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 3);
        LuaBehaviour behaviour = (LuaBehaviour) LuaScriptMgr.GetUnityObjectSelf(L, 1, "com.tencent.pandora.LuaBehaviour");
        AssetBundle bundle = LuaScriptMgr.GetUnityObject(L, 2, typeof(AssetBundle));
        string luaString = LuaScriptMgr.GetLuaString(L, 3);
        behaviour.OnInit(bundle, luaString);
        return 0;
    }

    public static void Register(IntPtr L)
    {
        LuaMethod[] regs = new LuaMethod[] { new LuaMethod("OnInit", new LuaCSFunction(com_tencent_pandora_LuaBehaviourWrap.OnInit)), new LuaMethod("GetGameObject", new LuaCSFunction(com_tencent_pandora_LuaBehaviourWrap.GetGameObject)), new LuaMethod("AddClick", new LuaCSFunction(com_tencent_pandora_LuaBehaviourWrap.AddClick)), new LuaMethod("AddUGUIClick", new LuaCSFunction(com_tencent_pandora_LuaBehaviourWrap.AddUGUIClick)), new LuaMethod("ClearClick", new LuaCSFunction(com_tencent_pandora_LuaBehaviourWrap.ClearClick)), new LuaMethod("New", new LuaCSFunction(com_tencent_pandora_LuaBehaviourWrap._Createcom_tencent_pandora_LuaBehaviour)), new LuaMethod("GetClassType", new LuaCSFunction(com_tencent_pandora_LuaBehaviourWrap.GetClassType)), new LuaMethod("__eq", new LuaCSFunction(com_tencent_pandora_LuaBehaviourWrap.Lua_Eq)) };
        LuaField[] fields = new LuaField[0];
        LuaScriptMgr.RegisterLib(L, "com.tencent.pandora.LuaBehaviour", typeof(LuaBehaviour), regs, fields, typeof(View));
    }
}

