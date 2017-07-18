using com.tencent.pandora;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class com_tencent_pandora_ResourceManagerWrap
{
    private static Type classType = typeof(ResourceManager);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _Createcom_tencent_pandora_ResourceManager(IntPtr L)
    {
        LuaDLL.luaL_error(L, "com.tencent.pandora.ResourceManager class does not have a constructor function");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int AssemplyUI(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        ResourceManager manager = (ResourceManager) LuaScriptMgr.GetUnityObjectSelf(L, 1, "com.tencent.pandora.ResourceManager");
        GameObject goParent = LuaScriptMgr.GetUnityObject(L, 2, typeof(GameObject));
        manager.AssemplyUI(goParent);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_atlasUpdate(IntPtr L)
    {
        ResourceManager luaObject = (ResourceManager) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name atlasUpdate");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index atlasUpdate on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.atlasUpdate);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_blUIResUnLoad(IntPtr L)
    {
        ResourceManager luaObject = (ResourceManager) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name blUIResUnLoad");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index blUIResUnLoad on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.blUIResUnLoad);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_luaFiles(IntPtr L)
    {
        ResourceManager luaObject = (ResourceManager) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name luaFiles");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index luaFiles on a nil value");
            }
        }
        LuaScriptMgr.PushObject(L, luaObject.luaFiles);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_m_atlasPath(IntPtr L)
    {
        ResourceManager luaObject = (ResourceManager) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name m_atlasPath");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index m_atlasPath on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.m_atlasPath);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_m_fontPath(IntPtr L)
    {
        ResourceManager luaObject = (ResourceManager) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name m_fontPath");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index m_fontPath on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.m_fontPath);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_mdictAtlas(IntPtr L)
    {
        ResourceManager luaObject = (ResourceManager) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name mdictAtlas");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index mdictAtlas on a nil value");
            }
        }
        LuaScriptMgr.PushObject(L, luaObject.mdictAtlas);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_mdictFont(IntPtr L)
    {
        ResourceManager luaObject = (ResourceManager) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name mdictFont");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index mdictFont on a nil value");
            }
        }
        LuaScriptMgr.PushObject(L, luaObject.mdictFont);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_passTimeSpan(IntPtr L)
    {
        ResourceManager luaObject = (ResourceManager) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name passTimeSpan");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index passTimeSpan on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.passTimeSpan);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_resPath(IntPtr L)
    {
        ResourceManager luaObject = (ResourceManager) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name resPath");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index resPath on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.resPath);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_startTimer(IntPtr L)
    {
        ResourceManager luaObject = (ResourceManager) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name startTimer");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index startTimer on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.startTimer);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_useSA(IntPtr L)
    {
        ResourceManager luaObject = (ResourceManager) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name useSA");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index useSA on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.useSA);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetClassType(IntPtr L)
    {
        LuaScriptMgr.Push(L, classType);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int getPlatformDesc(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 0);
        string str = ResourceManager.getPlatformDesc();
        LuaScriptMgr.Push(L, str);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int LoadBundleRes(IntPtr L)
    {
        <LoadBundleRes>c__AnonStorey5A storeya = new <LoadBundleRes>c__AnonStorey5A();
        storeya.L = L;
        LuaScriptMgr.CheckArgsCount(storeya.L, 3);
        ResourceManager manager = (ResourceManager) LuaScriptMgr.GetUnityObjectSelf(storeya.L, 1, "com.tencent.pandora.ResourceManager");
        string luaString = LuaScriptMgr.GetLuaString(storeya.L, 2);
        Action<AssetBundle> reFunc = null;
        if (LuaDLL.lua_type(storeya.L, 3) != LuaTypes.LUA_TFUNCTION)
        {
            reFunc = (Action<AssetBundle>) LuaScriptMgr.GetNetObject(storeya.L, 3, typeof(Action<AssetBundle>));
        }
        else
        {
            <LoadBundleRes>c__AnonStorey59 storey = new <LoadBundleRes>c__AnonStorey59();
            storey.<>f__ref$90 = storeya;
            storey.func = LuaScriptMgr.GetLuaFunction(storeya.L, 3);
            reFunc = new Action<AssetBundle>(storey.<>m__28);
        }
        manager.LoadBundleRes(luaString, reFunc, null);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int LoadLua(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        ((ResourceManager) LuaScriptMgr.GetUnityObjectSelf(L, 1, "com.tencent.pandora.ResourceManager")).LoadLua();
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int LoadUIRes(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        ResourceManager manager = (ResourceManager) LuaScriptMgr.GetUnityObjectSelf(L, 1, "com.tencent.pandora.ResourceManager");
        string luaString = LuaScriptMgr.GetLuaString(L, 2);
        manager.LoadUIRes(luaString);
        return 0;
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
        LuaMethod[] regs = new LuaMethod[] { new LuaMethod("SetResPath", new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.SetResPath)), new LuaMethod("LoadUIRes", new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.LoadUIRes)), new LuaMethod("AssemplyUI", new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.AssemplyUI)), new LuaMethod("LoadLua", new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.LoadLua)), new LuaMethod("LoadBundleRes", new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.LoadBundleRes)), new LuaMethod("getPlatformDesc", new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.getPlatformDesc)), new LuaMethod("New", new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap._Createcom_tencent_pandora_ResourceManager)), new LuaMethod("GetClassType", new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.GetClassType)), new LuaMethod("__eq", new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.Lua_Eq)) };
        LuaField[] fields = new LuaField[] { new LuaField("resPath", new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.get_resPath), new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.set_resPath)), new LuaField("useSA", new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.get_useSA), new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.set_useSA)), new LuaField("atlasUpdate", new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.get_atlasUpdate), new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.set_atlasUpdate)), new LuaField("passTimeSpan", new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.get_passTimeSpan), new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.set_passTimeSpan)), new LuaField("blUIResUnLoad", new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.get_blUIResUnLoad), new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.set_blUIResUnLoad)), new LuaField("startTimer", new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.get_startTimer), new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.set_startTimer)), new LuaField("luaFiles", new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.get_luaFiles), new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.set_luaFiles)), new LuaField("mdictFont", new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.get_mdictFont), new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.set_mdictFont)), new LuaField("mdictAtlas", new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.get_mdictAtlas), new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.set_mdictAtlas)), new LuaField("m_fontPath", new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.get_m_fontPath), new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.set_m_fontPath)), new LuaField("m_atlasPath", new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.get_m_atlasPath), new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.set_m_atlasPath)) };
        LuaScriptMgr.RegisterLib(L, "com.tencent.pandora.ResourceManager", typeof(ResourceManager), regs, fields, typeof(View));
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_atlasUpdate(IntPtr L)
    {
        ResourceManager luaObject = (ResourceManager) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name atlasUpdate");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index atlasUpdate on a nil value");
            }
        }
        luaObject.atlasUpdate = LuaScriptMgr.GetBoolean(L, 3);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_blUIResUnLoad(IntPtr L)
    {
        ResourceManager luaObject = (ResourceManager) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name blUIResUnLoad");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index blUIResUnLoad on a nil value");
            }
        }
        luaObject.blUIResUnLoad = LuaScriptMgr.GetBoolean(L, 3);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_luaFiles(IntPtr L)
    {
        ResourceManager luaObject = (ResourceManager) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name luaFiles");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index luaFiles on a nil value");
            }
        }
        luaObject.luaFiles = (Dictionary<string, TextAsset>) LuaScriptMgr.GetNetObject(L, 3, typeof(Dictionary<string, TextAsset>));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_m_atlasPath(IntPtr L)
    {
        ResourceManager luaObject = (ResourceManager) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name m_atlasPath");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index m_atlasPath on a nil value");
            }
        }
        luaObject.m_atlasPath = LuaScriptMgr.GetString(L, 3);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_m_fontPath(IntPtr L)
    {
        ResourceManager luaObject = (ResourceManager) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name m_fontPath");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index m_fontPath on a nil value");
            }
        }
        luaObject.m_fontPath = LuaScriptMgr.GetString(L, 3);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_mdictAtlas(IntPtr L)
    {
        ResourceManager luaObject = (ResourceManager) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name mdictAtlas");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index mdictAtlas on a nil value");
            }
        }
        luaObject.mdictAtlas = (Dictionary<string, Sprite>) LuaScriptMgr.GetNetObject(L, 3, typeof(Dictionary<string, Sprite>));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_mdictFont(IntPtr L)
    {
        ResourceManager luaObject = (ResourceManager) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name mdictFont");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index mdictFont on a nil value");
            }
        }
        luaObject.mdictFont = (Dictionary<string, Object>) LuaScriptMgr.GetNetObject(L, 3, typeof(Dictionary<string, Object>));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_passTimeSpan(IntPtr L)
    {
        ResourceManager luaObject = (ResourceManager) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name passTimeSpan");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index passTimeSpan on a nil value");
            }
        }
        luaObject.passTimeSpan = (float) LuaScriptMgr.GetNumber(L, 3);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_resPath(IntPtr L)
    {
        ResourceManager luaObject = (ResourceManager) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name resPath");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index resPath on a nil value");
            }
        }
        luaObject.resPath = LuaScriptMgr.GetString(L, 3);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_startTimer(IntPtr L)
    {
        ResourceManager luaObject = (ResourceManager) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name startTimer");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index startTimer on a nil value");
            }
        }
        luaObject.startTimer = LuaScriptMgr.GetBoolean(L, 3);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_useSA(IntPtr L)
    {
        ResourceManager luaObject = (ResourceManager) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name useSA");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index useSA on a nil value");
            }
        }
        luaObject.useSA = LuaScriptMgr.GetBoolean(L, 3);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int SetResPath(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        ((ResourceManager) LuaScriptMgr.GetUnityObjectSelf(L, 1, "com.tencent.pandora.ResourceManager")).SetResPath();
        return 0;
    }

    [CompilerGenerated]
    private sealed class <LoadBundleRes>c__AnonStorey59
    {
        internal com_tencent_pandora_ResourceManagerWrap.<LoadBundleRes>c__AnonStorey5A <>f__ref$90;
        internal LuaFunction func;

        internal void <>m__28(AssetBundle param0)
        {
            int oldTop = this.func.BeginPCall();
            LuaScriptMgr.Push(this.<>f__ref$90.L, (Object) param0);
            this.func.PCall(oldTop, 1);
            this.func.EndPCall(oldTop);
        }
    }

    [CompilerGenerated]
    private sealed class <LoadBundleRes>c__AnonStorey5A
    {
        internal IntPtr L;
    }
}

