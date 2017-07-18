using com.tencent.pandora;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class com_tencent_pandora_GetNewsImageWrap
{
    private static Type classType = typeof(GetNewsImage);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _Createcom_tencent_pandora_GetNewsImage(IntPtr L)
    {
        LuaDLL.luaL_error(L, "com.tencent.pandora.GetNewsImage class does not have a constructor function");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int BreakCoroutine(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        ((GetNewsImage) LuaScriptMgr.GetUnityObjectSelf(L, 1, "com.tencent.pandora.GetNewsImage")).BreakCoroutine();
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int CacheImage(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        GetNewsImage image = (GetNewsImage) LuaScriptMgr.GetUnityObjectSelf(L, 1, "com.tencent.pandora.GetNewsImage");
        string luaString = LuaScriptMgr.GetLuaString(L, 2);
        image.CacheImage(luaString);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int downloadTotalCnt(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        int d = ((GetNewsImage) LuaScriptMgr.GetUnityObjectSelf(L, 1, "com.tencent.pandora.GetNewsImage")).downloadTotalCnt();
        LuaScriptMgr.Push(L, d);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_dicFailImgs(IntPtr L)
    {
        LuaScriptMgr.PushObject(L, GetNewsImage.dicFailImgs);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_dicSuccImgs(IntPtr L)
    {
        LuaScriptMgr.PushObject(L, GetNewsImage.dicSuccImgs);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_isLoading(IntPtr L)
    {
        GetNewsImage luaObject = (GetNewsImage) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name isLoading");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index isLoading on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.isLoading);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_iSourceImgHeight(IntPtr L)
    {
        GetNewsImage luaObject = (GetNewsImage) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name iSourceImgHeight");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index iSourceImgHeight on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.iSourceImgHeight);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_iSourceImgWidth(IntPtr L)
    {
        GetNewsImage luaObject = (GetNewsImage) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name iSourceImgWidth");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index iSourceImgWidth on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.iSourceImgWidth);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_isResize(IntPtr L)
    {
        GetNewsImage luaObject = (GetNewsImage) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name isResize");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index isResize on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.isResize);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_ListPreToLoading(IntPtr L)
    {
        GetNewsImage luaObject = (GetNewsImage) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name ListPreToLoading");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index ListPreToLoading on a nil value");
            }
        }
        LuaScriptMgr.PushObject(L, luaObject.ListPreToLoading);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_m_nDownLoadNumber(IntPtr L)
    {
        GetNewsImage luaObject = (GetNewsImage) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name m_nDownLoadNumber");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index m_nDownLoadNumber on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.m_nDownLoadNumber);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_picDic(IntPtr L)
    {
        GetNewsImage luaObject = (GetNewsImage) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name picDic");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index picDic on a nil value");
            }
        }
        LuaScriptMgr.PushObject(L, luaObject.picDic);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_waitTimeOut(IntPtr L)
    {
        GetNewsImage luaObject = (GetNewsImage) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name waitTimeOut");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index waitTimeOut on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.waitTimeOut);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetClassType(IntPtr L)
    {
        LuaScriptMgr.Push(L, classType);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int getImageCacheFile(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        GetNewsImage image = (GetNewsImage) LuaScriptMgr.GetUnityObjectSelf(L, 1, "com.tencent.pandora.GetNewsImage");
        string luaString = LuaScriptMgr.GetLuaString(L, 2);
        string str = image.getImageCacheFile(luaString);
        LuaScriptMgr.Push(L, str);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int isImageCached(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        GetNewsImage image = (GetNewsImage) LuaScriptMgr.GetUnityObjectSelf(L, 1, "com.tencent.pandora.GetNewsImage");
        string luaString = LuaScriptMgr.GetLuaString(L, 2);
        bool b = image.isImageCached(luaString);
        LuaScriptMgr.Push(L, b);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int IsImgDownSucc(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        int d = GetNewsImage.IsImgDownSucc(LuaScriptMgr.GetLuaString(L, 1));
        LuaScriptMgr.Push(L, d);
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
    private static int PreDownImg(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        ((GetNewsImage) LuaScriptMgr.GetUnityObjectSelf(L, 1, "com.tencent.pandora.GetNewsImage")).PreDownImg();
        return 0;
    }

    public static void Register(IntPtr L)
    {
        LuaMethod[] regs = new LuaMethod[] { new LuaMethod("IsImgDownSucc", new LuaCSFunction(com_tencent_pandora_GetNewsImageWrap.IsImgDownSucc)), new LuaMethod("setPreFix", new LuaCSFunction(com_tencent_pandora_GetNewsImageWrap.setPreFix)), new LuaMethod("isImageCached", new LuaCSFunction(com_tencent_pandora_GetNewsImageWrap.isImageCached)), new LuaMethod("getImageCacheFile", new LuaCSFunction(com_tencent_pandora_GetNewsImageWrap.getImageCacheFile)), new LuaMethod("SetLuaFileName", new LuaCSFunction(com_tencent_pandora_GetNewsImageWrap.SetLuaFileName)), new LuaMethod("PreDownImg", new LuaCSFunction(com_tencent_pandora_GetNewsImageWrap.PreDownImg)), new LuaMethod("SetAsyncImageAll", new LuaCSFunction(com_tencent_pandora_GetNewsImageWrap.SetAsyncImageAll)), new LuaMethod("downloadTotalCnt", new LuaCSFunction(com_tencent_pandora_GetNewsImageWrap.downloadTotalCnt)), new LuaMethod("BreakCoroutine", new LuaCSFunction(com_tencent_pandora_GetNewsImageWrap.BreakCoroutine)), new LuaMethod("resetnumber", new LuaCSFunction(com_tencent_pandora_GetNewsImageWrap.resetnumber)), new LuaMethod("CacheImage", new LuaCSFunction(com_tencent_pandora_GetNewsImageWrap.CacheImage)), new LuaMethod("New", new LuaCSFunction(com_tencent_pandora_GetNewsImageWrap._Createcom_tencent_pandora_GetNewsImage)), new LuaMethod("GetClassType", new LuaCSFunction(com_tencent_pandora_GetNewsImageWrap.GetClassType)), new LuaMethod("__eq", new LuaCSFunction(com_tencent_pandora_GetNewsImageWrap.Lua_Eq)) };
        LuaField[] fields = new LuaField[] { new LuaField("waitTimeOut", new LuaCSFunction(com_tencent_pandora_GetNewsImageWrap.get_waitTimeOut), new LuaCSFunction(com_tencent_pandora_GetNewsImageWrap.set_waitTimeOut)), new LuaField("iSourceImgWidth", new LuaCSFunction(com_tencent_pandora_GetNewsImageWrap.get_iSourceImgWidth), new LuaCSFunction(com_tencent_pandora_GetNewsImageWrap.set_iSourceImgWidth)), new LuaField("iSourceImgHeight", new LuaCSFunction(com_tencent_pandora_GetNewsImageWrap.get_iSourceImgHeight), new LuaCSFunction(com_tencent_pandora_GetNewsImageWrap.set_iSourceImgHeight)), new LuaField("isLoading", new LuaCSFunction(com_tencent_pandora_GetNewsImageWrap.get_isLoading), new LuaCSFunction(com_tencent_pandora_GetNewsImageWrap.set_isLoading)), new LuaField("isResize", new LuaCSFunction(com_tencent_pandora_GetNewsImageWrap.get_isResize), new LuaCSFunction(com_tencent_pandora_GetNewsImageWrap.set_isResize)), new LuaField("ListPreToLoading", new LuaCSFunction(com_tencent_pandora_GetNewsImageWrap.get_ListPreToLoading), new LuaCSFunction(com_tencent_pandora_GetNewsImageWrap.set_ListPreToLoading)), new LuaField("dicSuccImgs", new LuaCSFunction(com_tencent_pandora_GetNewsImageWrap.get_dicSuccImgs), new LuaCSFunction(com_tencent_pandora_GetNewsImageWrap.set_dicSuccImgs)), new LuaField("dicFailImgs", new LuaCSFunction(com_tencent_pandora_GetNewsImageWrap.get_dicFailImgs), new LuaCSFunction(com_tencent_pandora_GetNewsImageWrap.set_dicFailImgs)), new LuaField("m_nDownLoadNumber", new LuaCSFunction(com_tencent_pandora_GetNewsImageWrap.get_m_nDownLoadNumber), new LuaCSFunction(com_tencent_pandora_GetNewsImageWrap.set_m_nDownLoadNumber)), new LuaField("picDic", new LuaCSFunction(com_tencent_pandora_GetNewsImageWrap.get_picDic), new LuaCSFunction(com_tencent_pandora_GetNewsImageWrap.set_picDic)) };
        LuaScriptMgr.RegisterLib(L, "com.tencent.pandora.GetNewsImage", typeof(GetNewsImage), regs, fields, typeof(MonoBehaviour));
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int resetnumber(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        ((GetNewsImage) LuaScriptMgr.GetUnityObjectSelf(L, 1, "com.tencent.pandora.GetNewsImage")).resetnumber();
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_dicFailImgs(IntPtr L)
    {
        GetNewsImage.dicFailImgs = (Dictionary<string, bool>) LuaScriptMgr.GetNetObject(L, 3, typeof(Dictionary<string, bool>));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_dicSuccImgs(IntPtr L)
    {
        GetNewsImage.dicSuccImgs = (Dictionary<string, bool>) LuaScriptMgr.GetNetObject(L, 3, typeof(Dictionary<string, bool>));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_isLoading(IntPtr L)
    {
        GetNewsImage luaObject = (GetNewsImage) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name isLoading");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index isLoading on a nil value");
            }
        }
        luaObject.isLoading = LuaScriptMgr.GetBoolean(L, 3);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_iSourceImgHeight(IntPtr L)
    {
        GetNewsImage luaObject = (GetNewsImage) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name iSourceImgHeight");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index iSourceImgHeight on a nil value");
            }
        }
        luaObject.iSourceImgHeight = (int) LuaScriptMgr.GetNumber(L, 3);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_iSourceImgWidth(IntPtr L)
    {
        GetNewsImage luaObject = (GetNewsImage) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name iSourceImgWidth");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index iSourceImgWidth on a nil value");
            }
        }
        luaObject.iSourceImgWidth = (int) LuaScriptMgr.GetNumber(L, 3);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_isResize(IntPtr L)
    {
        GetNewsImage luaObject = (GetNewsImage) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name isResize");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index isResize on a nil value");
            }
        }
        luaObject.isResize = LuaScriptMgr.GetBoolean(L, 3);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_ListPreToLoading(IntPtr L)
    {
        GetNewsImage luaObject = (GetNewsImage) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name ListPreToLoading");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index ListPreToLoading on a nil value");
            }
        }
        luaObject.ListPreToLoading = (List<string>) LuaScriptMgr.GetNetObject(L, 3, typeof(List<string>));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_m_nDownLoadNumber(IntPtr L)
    {
        GetNewsImage luaObject = (GetNewsImage) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name m_nDownLoadNumber");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index m_nDownLoadNumber on a nil value");
            }
        }
        luaObject.m_nDownLoadNumber = (int) LuaScriptMgr.GetNumber(L, 3);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_picDic(IntPtr L)
    {
        GetNewsImage luaObject = (GetNewsImage) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name picDic");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index picDic on a nil value");
            }
        }
        luaObject.picDic = (Dictionary<string, Texture>) LuaScriptMgr.GetNetObject(L, 3, typeof(Dictionary<string, Texture>));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_waitTimeOut(IntPtr L)
    {
        GetNewsImage luaObject = (GetNewsImage) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name waitTimeOut");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index waitTimeOut on a nil value");
            }
        }
        luaObject.waitTimeOut = (float) LuaScriptMgr.GetNumber(L, 3);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int SetAsyncImageAll(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 4);
        GetNewsImage image = (GetNewsImage) LuaScriptMgr.GetUnityObjectSelf(L, 1, "com.tencent.pandora.GetNewsImage");
        string luaString = LuaScriptMgr.GetLuaString(L, 2);
        Image tex = LuaScriptMgr.GetUnityObject(L, 3, typeof(Image));
        string nowFileName = LuaScriptMgr.GetLuaString(L, 4);
        image.SetAsyncImageAll(luaString, tex, nowFileName);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int SetLuaFileName(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        GetNewsImage image = (GetNewsImage) LuaScriptMgr.GetUnityObjectSelf(L, 1, "com.tencent.pandora.GetNewsImage");
        string luaString = LuaScriptMgr.GetLuaString(L, 2);
        image.SetLuaFileName(luaString);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int setPreFix(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        GetNewsImage image = (GetNewsImage) LuaScriptMgr.GetUnityObjectSelf(L, 1, "com.tencent.pandora.GetNewsImage");
        int number = (int) LuaScriptMgr.GetNumber(L, 2);
        image.setPreFix(number);
        return 0;
    }
}

