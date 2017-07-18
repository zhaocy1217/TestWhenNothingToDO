using com.tencent.pandora;
using System;
using UnityEngine;

public class SkinnedMeshRendererWrap
{
    private static Type classType = typeof(SkinnedMeshRenderer);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _CreateSkinnedMeshRenderer(IntPtr L)
    {
        if (LuaDLL.lua_gettop(L) == 0)
        {
            SkinnedMeshRenderer renderer = new SkinnedMeshRenderer();
            LuaScriptMgr.Push(L, (Object) renderer);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: SkinnedMeshRenderer.New");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int BakeMesh(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        SkinnedMeshRenderer renderer = (SkinnedMeshRenderer) LuaScriptMgr.GetUnityObjectSelf(L, 1, "SkinnedMeshRenderer");
        Mesh mesh = LuaScriptMgr.GetUnityObject(L, 2, typeof(Mesh));
        renderer.BakeMesh(mesh);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_bones(IntPtr L)
    {
        SkinnedMeshRenderer luaObject = (SkinnedMeshRenderer) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name bones");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index bones on a nil value");
            }
        }
        LuaScriptMgr.PushArray(L, luaObject.get_bones());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_localBounds(IntPtr L)
    {
        SkinnedMeshRenderer luaObject = (SkinnedMeshRenderer) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name localBounds");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index localBounds on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_localBounds());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_quality(IntPtr L)
    {
        SkinnedMeshRenderer luaObject = (SkinnedMeshRenderer) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name quality");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index quality on a nil value");
            }
        }
        LuaScriptMgr.Push(L, (Enum) luaObject.get_quality());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_rootBone(IntPtr L)
    {
        SkinnedMeshRenderer luaObject = (SkinnedMeshRenderer) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name rootBone");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index rootBone on a nil value");
            }
        }
        LuaScriptMgr.Push(L, (Object) luaObject.get_rootBone());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_sharedMesh(IntPtr L)
    {
        SkinnedMeshRenderer luaObject = (SkinnedMeshRenderer) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name sharedMesh");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index sharedMesh on a nil value");
            }
        }
        LuaScriptMgr.Push(L, (Object) luaObject.get_sharedMesh());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_updateWhenOffscreen(IntPtr L)
    {
        SkinnedMeshRenderer luaObject = (SkinnedMeshRenderer) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name updateWhenOffscreen");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index updateWhenOffscreen on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_updateWhenOffscreen());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetBlendShapeWeight(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        SkinnedMeshRenderer renderer = (SkinnedMeshRenderer) LuaScriptMgr.GetUnityObjectSelf(L, 1, "SkinnedMeshRenderer");
        int number = (int) LuaScriptMgr.GetNumber(L, 2);
        float blendShapeWeight = renderer.GetBlendShapeWeight(number);
        LuaScriptMgr.Push(L, blendShapeWeight);
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
        LuaMethod[] regs = new LuaMethod[] { new LuaMethod("BakeMesh", new LuaCSFunction(SkinnedMeshRendererWrap.BakeMesh)), new LuaMethod("GetBlendShapeWeight", new LuaCSFunction(SkinnedMeshRendererWrap.GetBlendShapeWeight)), new LuaMethod("SetBlendShapeWeight", new LuaCSFunction(SkinnedMeshRendererWrap.SetBlendShapeWeight)), new LuaMethod("New", new LuaCSFunction(SkinnedMeshRendererWrap._CreateSkinnedMeshRenderer)), new LuaMethod("GetClassType", new LuaCSFunction(SkinnedMeshRendererWrap.GetClassType)), new LuaMethod("__eq", new LuaCSFunction(SkinnedMeshRendererWrap.Lua_Eq)) };
        LuaField[] fields = new LuaField[] { new LuaField("bones", new LuaCSFunction(SkinnedMeshRendererWrap.get_bones), new LuaCSFunction(SkinnedMeshRendererWrap.set_bones)), new LuaField("rootBone", new LuaCSFunction(SkinnedMeshRendererWrap.get_rootBone), new LuaCSFunction(SkinnedMeshRendererWrap.set_rootBone)), new LuaField("quality", new LuaCSFunction(SkinnedMeshRendererWrap.get_quality), new LuaCSFunction(SkinnedMeshRendererWrap.set_quality)), new LuaField("sharedMesh", new LuaCSFunction(SkinnedMeshRendererWrap.get_sharedMesh), new LuaCSFunction(SkinnedMeshRendererWrap.set_sharedMesh)), new LuaField("updateWhenOffscreen", new LuaCSFunction(SkinnedMeshRendererWrap.get_updateWhenOffscreen), new LuaCSFunction(SkinnedMeshRendererWrap.set_updateWhenOffscreen)), new LuaField("localBounds", new LuaCSFunction(SkinnedMeshRendererWrap.get_localBounds), new LuaCSFunction(SkinnedMeshRendererWrap.set_localBounds)) };
        LuaScriptMgr.RegisterLib(L, "UnityEngine.SkinnedMeshRenderer", typeof(SkinnedMeshRenderer), regs, fields, typeof(Renderer));
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_bones(IntPtr L)
    {
        SkinnedMeshRenderer luaObject = (SkinnedMeshRenderer) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name bones");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index bones on a nil value");
            }
        }
        luaObject.set_bones(LuaScriptMgr.GetArrayObject<Transform>(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_localBounds(IntPtr L)
    {
        SkinnedMeshRenderer luaObject = (SkinnedMeshRenderer) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name localBounds");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index localBounds on a nil value");
            }
        }
        luaObject.set_localBounds(LuaScriptMgr.GetBounds(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_quality(IntPtr L)
    {
        SkinnedMeshRenderer luaObject = (SkinnedMeshRenderer) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name quality");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index quality on a nil value");
            }
        }
        luaObject.set_quality((SkinQuality) ((int) LuaScriptMgr.GetNetObject(L, 3, typeof(SkinQuality))));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_rootBone(IntPtr L)
    {
        SkinnedMeshRenderer luaObject = (SkinnedMeshRenderer) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name rootBone");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index rootBone on a nil value");
            }
        }
        luaObject.set_rootBone((Transform) LuaScriptMgr.GetUnityObject(L, 3, typeof(Transform)));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_sharedMesh(IntPtr L)
    {
        SkinnedMeshRenderer luaObject = (SkinnedMeshRenderer) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name sharedMesh");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index sharedMesh on a nil value");
            }
        }
        luaObject.set_sharedMesh((Mesh) LuaScriptMgr.GetUnityObject(L, 3, typeof(Mesh)));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_updateWhenOffscreen(IntPtr L)
    {
        SkinnedMeshRenderer luaObject = (SkinnedMeshRenderer) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name updateWhenOffscreen");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index updateWhenOffscreen on a nil value");
            }
        }
        luaObject.set_updateWhenOffscreen(LuaScriptMgr.GetBoolean(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int SetBlendShapeWeight(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 3);
        SkinnedMeshRenderer renderer = (SkinnedMeshRenderer) LuaScriptMgr.GetUnityObjectSelf(L, 1, "SkinnedMeshRenderer");
        int number = (int) LuaScriptMgr.GetNumber(L, 2);
        float num2 = (float) LuaScriptMgr.GetNumber(L, 3);
        renderer.SetBlendShapeWeight(number, num2);
        return 0;
    }
}

