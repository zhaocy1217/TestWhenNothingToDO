using com.tencent.pandora;
using System;
using UnityEngine;

public class RenderTextureWrap
{
    private static Type classType = typeof(RenderTexture);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _CreateRenderTexture(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 3:
            {
                int number = (int) LuaScriptMgr.GetNumber(L, 1);
                int num3 = (int) LuaScriptMgr.GetNumber(L, 2);
                int num4 = (int) LuaScriptMgr.GetNumber(L, 3);
                RenderTexture texture = new RenderTexture(number, num3, num4);
                LuaScriptMgr.Push(L, (Object) texture);
                return 1;
            }
            case 4:
            {
                int num5 = (int) LuaScriptMgr.GetNumber(L, 1);
                int num6 = (int) LuaScriptMgr.GetNumber(L, 2);
                int num7 = (int) LuaScriptMgr.GetNumber(L, 3);
                RenderTextureFormat format = (RenderTextureFormat) ((int) LuaScriptMgr.GetNetObject(L, 4, typeof(RenderTextureFormat)));
                RenderTexture texture2 = new RenderTexture(num5, num6, num7, format);
                LuaScriptMgr.Push(L, (Object) texture2);
                return 1;
            }
            case 5:
            {
                int num8 = (int) LuaScriptMgr.GetNumber(L, 1);
                int num9 = (int) LuaScriptMgr.GetNumber(L, 2);
                int num10 = (int) LuaScriptMgr.GetNumber(L, 3);
                RenderTextureFormat format2 = (RenderTextureFormat) ((int) LuaScriptMgr.GetNetObject(L, 4, typeof(RenderTextureFormat)));
                RenderTextureReadWrite write = (RenderTextureReadWrite) ((int) LuaScriptMgr.GetNetObject(L, 5, typeof(RenderTextureReadWrite)));
                RenderTexture texture3 = new RenderTexture(num8, num9, num10, format2, write);
                LuaScriptMgr.Push(L, (Object) texture3);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: RenderTexture.New");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Create(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        bool b = ((RenderTexture) LuaScriptMgr.GetUnityObjectSelf(L, 1, "RenderTexture")).Create();
        LuaScriptMgr.Push(L, b);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int DiscardContents(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 1:
                ((RenderTexture) LuaScriptMgr.GetUnityObjectSelf(L, 1, "RenderTexture")).DiscardContents();
                return 0;

            case 3:
            {
                RenderTexture texture2 = (RenderTexture) LuaScriptMgr.GetUnityObjectSelf(L, 1, "RenderTexture");
                bool boolean = LuaScriptMgr.GetBoolean(L, 2);
                bool flag2 = LuaScriptMgr.GetBoolean(L, 3);
                texture2.DiscardContents(boolean, flag2);
                return 0;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: RenderTexture.DiscardContents");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_active(IntPtr L)
    {
        LuaScriptMgr.Push(L, (Object) RenderTexture.get_active());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_antiAliasing(IntPtr L)
    {
        RenderTexture luaObject = (RenderTexture) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name antiAliasing");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index antiAliasing on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_antiAliasing());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_colorBuffer(IntPtr L)
    {
        RenderTexture luaObject = (RenderTexture) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name colorBuffer");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index colorBuffer on a nil value");
            }
        }
        LuaScriptMgr.PushValue(L, luaObject.get_colorBuffer());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_depth(IntPtr L)
    {
        RenderTexture luaObject = (RenderTexture) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name depth");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index depth on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_depth());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_depthBuffer(IntPtr L)
    {
        RenderTexture luaObject = (RenderTexture) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name depthBuffer");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index depthBuffer on a nil value");
            }
        }
        LuaScriptMgr.PushValue(L, luaObject.get_depthBuffer());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_enableRandomWrite(IntPtr L)
    {
        RenderTexture luaObject = (RenderTexture) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name enableRandomWrite");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index enableRandomWrite on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_enableRandomWrite());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_format(IntPtr L)
    {
        RenderTexture luaObject = (RenderTexture) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name format");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index format on a nil value");
            }
        }
        LuaScriptMgr.Push(L, (Enum) luaObject.get_format());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_generateMips(IntPtr L)
    {
        RenderTexture luaObject = (RenderTexture) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name generateMips");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index generateMips on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_generateMips());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_height(IntPtr L)
    {
        RenderTexture luaObject = (RenderTexture) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name height");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index height on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_height());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_isCubemap(IntPtr L)
    {
        RenderTexture luaObject = (RenderTexture) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name isCubemap");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index isCubemap on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_isCubemap());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_isPowerOfTwo(IntPtr L)
    {
        RenderTexture luaObject = (RenderTexture) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name isPowerOfTwo");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index isPowerOfTwo on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_isPowerOfTwo());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_isVolume(IntPtr L)
    {
        RenderTexture luaObject = (RenderTexture) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name isVolume");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index isVolume on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_isVolume());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_sRGB(IntPtr L)
    {
        RenderTexture luaObject = (RenderTexture) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name sRGB");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index sRGB on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_sRGB());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_useMipMap(IntPtr L)
    {
        RenderTexture luaObject = (RenderTexture) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name useMipMap");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index useMipMap on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_useMipMap());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_volumeDepth(IntPtr L)
    {
        RenderTexture luaObject = (RenderTexture) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name volumeDepth");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index volumeDepth on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_volumeDepth());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_width(IntPtr L)
    {
        RenderTexture luaObject = (RenderTexture) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name width");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index width on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_width());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetClassType(IntPtr L)
    {
        LuaScriptMgr.Push(L, classType);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetTemporary(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 2:
            {
                int number = (int) LuaScriptMgr.GetNumber(L, 1);
                int num3 = (int) LuaScriptMgr.GetNumber(L, 2);
                RenderTexture temporary = RenderTexture.GetTemporary(number, num3);
                LuaScriptMgr.Push(L, (Object) temporary);
                return 1;
            }
            case 3:
            {
                int num4 = (int) LuaScriptMgr.GetNumber(L, 1);
                int num5 = (int) LuaScriptMgr.GetNumber(L, 2);
                int num6 = (int) LuaScriptMgr.GetNumber(L, 3);
                RenderTexture texture2 = RenderTexture.GetTemporary(num4, num5, num6);
                LuaScriptMgr.Push(L, (Object) texture2);
                return 1;
            }
            case 4:
            {
                int num7 = (int) LuaScriptMgr.GetNumber(L, 1);
                int num8 = (int) LuaScriptMgr.GetNumber(L, 2);
                int num9 = (int) LuaScriptMgr.GetNumber(L, 3);
                RenderTextureFormat format = (RenderTextureFormat) ((int) LuaScriptMgr.GetNetObject(L, 4, typeof(RenderTextureFormat)));
                RenderTexture texture3 = RenderTexture.GetTemporary(num7, num8, num9, format);
                LuaScriptMgr.Push(L, (Object) texture3);
                return 1;
            }
            case 5:
            {
                int num10 = (int) LuaScriptMgr.GetNumber(L, 1);
                int num11 = (int) LuaScriptMgr.GetNumber(L, 2);
                int num12 = (int) LuaScriptMgr.GetNumber(L, 3);
                RenderTextureFormat format2 = (RenderTextureFormat) ((int) LuaScriptMgr.GetNetObject(L, 4, typeof(RenderTextureFormat)));
                RenderTextureReadWrite write = (RenderTextureReadWrite) ((int) LuaScriptMgr.GetNetObject(L, 5, typeof(RenderTextureReadWrite)));
                RenderTexture texture4 = RenderTexture.GetTemporary(num10, num11, num12, format2, write);
                LuaScriptMgr.Push(L, (Object) texture4);
                return 1;
            }
            case 6:
            {
                int num13 = (int) LuaScriptMgr.GetNumber(L, 1);
                int num14 = (int) LuaScriptMgr.GetNumber(L, 2);
                int num15 = (int) LuaScriptMgr.GetNumber(L, 3);
                RenderTextureFormat format3 = (RenderTextureFormat) ((int) LuaScriptMgr.GetNetObject(L, 4, typeof(RenderTextureFormat)));
                RenderTextureReadWrite write2 = (RenderTextureReadWrite) ((int) LuaScriptMgr.GetNetObject(L, 5, typeof(RenderTextureReadWrite)));
                int num16 = (int) LuaScriptMgr.GetNumber(L, 6);
                RenderTexture texture5 = RenderTexture.GetTemporary(num13, num14, num15, format3, write2, num16);
                LuaScriptMgr.Push(L, (Object) texture5);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: RenderTexture.GetTemporary");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetTexelOffset(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        Vector2 texelOffset = ((RenderTexture) LuaScriptMgr.GetUnityObjectSelf(L, 1, "RenderTexture")).GetTexelOffset();
        LuaScriptMgr.Push(L, texelOffset);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int IsCreated(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        bool b = ((RenderTexture) LuaScriptMgr.GetUnityObjectSelf(L, 1, "RenderTexture")).IsCreated();
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
    private static int MarkRestoreExpected(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        ((RenderTexture) LuaScriptMgr.GetUnityObjectSelf(L, 1, "RenderTexture")).MarkRestoreExpected();
        return 0;
    }

    public static void Register(IntPtr L)
    {
        LuaMethod[] regs = new LuaMethod[] { new LuaMethod("GetTemporary", new LuaCSFunction(RenderTextureWrap.GetTemporary)), new LuaMethod("ReleaseTemporary", new LuaCSFunction(RenderTextureWrap.ReleaseTemporary)), new LuaMethod("Create", new LuaCSFunction(RenderTextureWrap.Create)), new LuaMethod("Release", new LuaCSFunction(RenderTextureWrap.Release)), new LuaMethod("IsCreated", new LuaCSFunction(RenderTextureWrap.IsCreated)), new LuaMethod("DiscardContents", new LuaCSFunction(RenderTextureWrap.DiscardContents)), new LuaMethod("MarkRestoreExpected", new LuaCSFunction(RenderTextureWrap.MarkRestoreExpected)), new LuaMethod("SetGlobalShaderProperty", new LuaCSFunction(RenderTextureWrap.SetGlobalShaderProperty)), new LuaMethod("GetTexelOffset", new LuaCSFunction(RenderTextureWrap.GetTexelOffset)), new LuaMethod("SupportsStencil", new LuaCSFunction(RenderTextureWrap.SupportsStencil)), new LuaMethod("New", new LuaCSFunction(RenderTextureWrap._CreateRenderTexture)), new LuaMethod("GetClassType", new LuaCSFunction(RenderTextureWrap.GetClassType)), new LuaMethod("__eq", new LuaCSFunction(RenderTextureWrap.Lua_Eq)) };
        LuaField[] fields = new LuaField[] { new LuaField("width", new LuaCSFunction(RenderTextureWrap.get_width), new LuaCSFunction(RenderTextureWrap.set_width)), new LuaField("height", new LuaCSFunction(RenderTextureWrap.get_height), new LuaCSFunction(RenderTextureWrap.set_height)), new LuaField("depth", new LuaCSFunction(RenderTextureWrap.get_depth), new LuaCSFunction(RenderTextureWrap.set_depth)), new LuaField("isPowerOfTwo", new LuaCSFunction(RenderTextureWrap.get_isPowerOfTwo), new LuaCSFunction(RenderTextureWrap.set_isPowerOfTwo)), new LuaField("sRGB", new LuaCSFunction(RenderTextureWrap.get_sRGB), null), new LuaField("format", new LuaCSFunction(RenderTextureWrap.get_format), new LuaCSFunction(RenderTextureWrap.set_format)), new LuaField("useMipMap", new LuaCSFunction(RenderTextureWrap.get_useMipMap), new LuaCSFunction(RenderTextureWrap.set_useMipMap)), new LuaField("generateMips", new LuaCSFunction(RenderTextureWrap.get_generateMips), new LuaCSFunction(RenderTextureWrap.set_generateMips)), new LuaField("isCubemap", new LuaCSFunction(RenderTextureWrap.get_isCubemap), new LuaCSFunction(RenderTextureWrap.set_isCubemap)), new LuaField("isVolume", new LuaCSFunction(RenderTextureWrap.get_isVolume), new LuaCSFunction(RenderTextureWrap.set_isVolume)), new LuaField("volumeDepth", new LuaCSFunction(RenderTextureWrap.get_volumeDepth), new LuaCSFunction(RenderTextureWrap.set_volumeDepth)), new LuaField("antiAliasing", new LuaCSFunction(RenderTextureWrap.get_antiAliasing), new LuaCSFunction(RenderTextureWrap.set_antiAliasing)), new LuaField("enableRandomWrite", new LuaCSFunction(RenderTextureWrap.get_enableRandomWrite), new LuaCSFunction(RenderTextureWrap.set_enableRandomWrite)), new LuaField("colorBuffer", new LuaCSFunction(RenderTextureWrap.get_colorBuffer), null), new LuaField("depthBuffer", new LuaCSFunction(RenderTextureWrap.get_depthBuffer), null), new LuaField("active", new LuaCSFunction(RenderTextureWrap.get_active), new LuaCSFunction(RenderTextureWrap.set_active)) };
        LuaScriptMgr.RegisterLib(L, "UnityEngine.RenderTexture", typeof(RenderTexture), regs, fields, typeof(Texture));
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Release(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        ((RenderTexture) LuaScriptMgr.GetUnityObjectSelf(L, 1, "RenderTexture")).Release();
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int ReleaseTemporary(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        RenderTexture texture = LuaScriptMgr.GetUnityObject(L, 1, typeof(RenderTexture));
        RenderTexture.ReleaseTemporary(texture);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_active(IntPtr L)
    {
        RenderTexture.set_active((RenderTexture) LuaScriptMgr.GetUnityObject(L, 3, typeof(RenderTexture)));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_antiAliasing(IntPtr L)
    {
        RenderTexture luaObject = (RenderTexture) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name antiAliasing");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index antiAliasing on a nil value");
            }
        }
        luaObject.set_antiAliasing((int) LuaScriptMgr.GetNumber(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_depth(IntPtr L)
    {
        RenderTexture luaObject = (RenderTexture) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name depth");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index depth on a nil value");
            }
        }
        luaObject.set_depth((int) LuaScriptMgr.GetNumber(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_enableRandomWrite(IntPtr L)
    {
        RenderTexture luaObject = (RenderTexture) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name enableRandomWrite");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index enableRandomWrite on a nil value");
            }
        }
        luaObject.set_enableRandomWrite(LuaScriptMgr.GetBoolean(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_format(IntPtr L)
    {
        RenderTexture luaObject = (RenderTexture) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name format");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index format on a nil value");
            }
        }
        luaObject.set_format((RenderTextureFormat) ((int) LuaScriptMgr.GetNetObject(L, 3, typeof(RenderTextureFormat))));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_generateMips(IntPtr L)
    {
        RenderTexture luaObject = (RenderTexture) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name generateMips");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index generateMips on a nil value");
            }
        }
        luaObject.set_generateMips(LuaScriptMgr.GetBoolean(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_height(IntPtr L)
    {
        RenderTexture luaObject = (RenderTexture) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name height");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index height on a nil value");
            }
        }
        luaObject.set_height((int) LuaScriptMgr.GetNumber(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_isCubemap(IntPtr L)
    {
        RenderTexture luaObject = (RenderTexture) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name isCubemap");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index isCubemap on a nil value");
            }
        }
        luaObject.set_isCubemap(LuaScriptMgr.GetBoolean(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_isPowerOfTwo(IntPtr L)
    {
        RenderTexture luaObject = (RenderTexture) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name isPowerOfTwo");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index isPowerOfTwo on a nil value");
            }
        }
        luaObject.set_isPowerOfTwo(LuaScriptMgr.GetBoolean(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_isVolume(IntPtr L)
    {
        RenderTexture luaObject = (RenderTexture) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name isVolume");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index isVolume on a nil value");
            }
        }
        luaObject.set_isVolume(LuaScriptMgr.GetBoolean(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_useMipMap(IntPtr L)
    {
        RenderTexture luaObject = (RenderTexture) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name useMipMap");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index useMipMap on a nil value");
            }
        }
        luaObject.set_useMipMap(LuaScriptMgr.GetBoolean(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_volumeDepth(IntPtr L)
    {
        RenderTexture luaObject = (RenderTexture) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name volumeDepth");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index volumeDepth on a nil value");
            }
        }
        luaObject.set_volumeDepth((int) LuaScriptMgr.GetNumber(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_width(IntPtr L)
    {
        RenderTexture luaObject = (RenderTexture) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name width");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index width on a nil value");
            }
        }
        luaObject.set_width((int) LuaScriptMgr.GetNumber(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int SetGlobalShaderProperty(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        RenderTexture texture = (RenderTexture) LuaScriptMgr.GetUnityObjectSelf(L, 1, "RenderTexture");
        string luaString = LuaScriptMgr.GetLuaString(L, 2);
        texture.SetGlobalShaderProperty(luaString);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int SupportsStencil(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        RenderTexture texture = LuaScriptMgr.GetUnityObject(L, 1, typeof(RenderTexture));
        bool b = RenderTexture.SupportsStencil(texture);
        LuaScriptMgr.Push(L, b);
        return 1;
    }
}

