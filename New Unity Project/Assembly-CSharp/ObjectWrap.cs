using com.tencent.pandora;
using System;
using UnityEngine;

public class ObjectWrap
{
    private static Type classType = typeof(Object);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _CreateObject(IntPtr L)
    {
        if (LuaDLL.lua_gettop(L) == 0)
        {
            Object obj2 = new Object();
            LuaScriptMgr.Push(L, obj2);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Object.New");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Destroy(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 1:
            {
                Object luaObject = (Object) LuaScriptMgr.GetLuaObject(L, 1);
                LuaScriptMgr.__gc(L);
                Object.Destroy(luaObject);
                return 0;
            }
            case 2:
            {
                Object obj3 = (Object) LuaScriptMgr.GetLuaObject(L, 1);
                float number = (float) LuaScriptMgr.GetNumber(L, 2);
                Object.Destroy(obj3, number);
                return 0;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Object.Destroy");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int DestroyImmediate(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 1:
            {
                Object luaObject = (Object) LuaScriptMgr.GetLuaObject(L, 1);
                LuaScriptMgr.__gc(L);
                Object.DestroyImmediate(luaObject);
                return 0;
            }
            case 2:
            {
                Object obj3 = (Object) LuaScriptMgr.GetLuaObject(L, 1);
                bool boolean = LuaScriptMgr.GetBoolean(L, 2);
                Object.DestroyImmediate(obj3, boolean);
                return 0;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Object.DestroyImmediate");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int DestroyObject(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 1:
            {
                Object luaObject = (Object) LuaScriptMgr.GetLuaObject(L, 1);
                LuaScriptMgr.__gc(L);
                Object.DestroyObject(luaObject);
                return 0;
            }
            case 2:
            {
                Object obj3 = (Object) LuaScriptMgr.GetLuaObject(L, 1);
                float number = (float) LuaScriptMgr.GetNumber(L, 2);
                Object.DestroyObject(obj3, number);
                return 0;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Object.DestroyObject");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int DontDestroyOnLoad(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        Object.DontDestroyOnLoad(LuaScriptMgr.GetUnityObject(L, 1, typeof(Object)));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Equals(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Object varObject = LuaScriptMgr.GetVarObject(L, 1) as Object;
        object obj3 = LuaScriptMgr.GetVarObject(L, 2);
        bool b = (varObject == null) ? (obj3 == null) : varObject.Equals(obj3);
        LuaScriptMgr.Push(L, b);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int FindObjectOfType(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        Object obj2 = Object.FindObjectOfType(LuaScriptMgr.GetTypeObject(L, 1));
        LuaScriptMgr.Push(L, obj2);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int FindObjectsOfType(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        Object[] o = Object.FindObjectsOfType(LuaScriptMgr.GetTypeObject(L, 1));
        LuaScriptMgr.PushArray(L, o);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_hideFlags(IntPtr L)
    {
        Object luaObject = (Object) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name hideFlags");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index hideFlags on a nil value");
            }
        }
        LuaScriptMgr.Push(L, (Enum) luaObject.get_hideFlags());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_name(IntPtr L)
    {
        Object luaObject = (Object) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name name");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index name on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_name());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetClassType(IntPtr L)
    {
        LuaScriptMgr.Push(L, classType);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetHashCode(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        int hashCode = ((Object) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Object")).GetHashCode();
        LuaScriptMgr.Push(L, hashCode);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetInstanceID(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        int instanceID = ((Object) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Object")).GetInstanceID();
        LuaScriptMgr.Push(L, instanceID);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Instantiate(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 1:
            {
                Object obj3 = Object.Instantiate(LuaScriptMgr.GetUnityObject(L, 1, typeof(Object)));
                LuaScriptMgr.Push(L, obj3);
                return 1;
            }
            case 3:
            {
                Object obj4 = LuaScriptMgr.GetUnityObject(L, 1, typeof(Object));
                Vector3 vector = LuaScriptMgr.GetVector3(L, 2);
                Quaternion quaternion = LuaScriptMgr.GetQuaternion(L, 3);
                Object obj5 = Object.Instantiate(obj4, vector, quaternion);
                LuaScriptMgr.Push(L, obj5);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Object.Instantiate");
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

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Lua_ToString(IntPtr L)
    {
        object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject != null)
        {
            LuaScriptMgr.Push(L, luaObject.ToString());
        }
        else
        {
            LuaScriptMgr.Push(L, "Table: UnityEngine.Object");
        }
        return 1;
    }

    public static void Register(IntPtr L)
    {
        LuaMethod[] regs = new LuaMethod[] { new LuaMethod("Equals", new LuaCSFunction(ObjectWrap.Equals)), new LuaMethod("GetHashCode", new LuaCSFunction(ObjectWrap.GetHashCode)), new LuaMethod("GetInstanceID", new LuaCSFunction(ObjectWrap.GetInstanceID)), new LuaMethod("Instantiate", new LuaCSFunction(ObjectWrap.Instantiate)), new LuaMethod("FindObjectsOfType", new LuaCSFunction(ObjectWrap.FindObjectsOfType)), new LuaMethod("FindObjectOfType", new LuaCSFunction(ObjectWrap.FindObjectOfType)), new LuaMethod("DontDestroyOnLoad", new LuaCSFunction(ObjectWrap.DontDestroyOnLoad)), new LuaMethod("ToString", new LuaCSFunction(ObjectWrap.ToString)), new LuaMethod("DestroyObject", new LuaCSFunction(ObjectWrap.DestroyObject)), new LuaMethod("DestroyImmediate", new LuaCSFunction(ObjectWrap.DestroyImmediate)), new LuaMethod("Destroy", new LuaCSFunction(ObjectWrap.Destroy)), new LuaMethod("New", new LuaCSFunction(ObjectWrap._CreateObject)), new LuaMethod("GetClassType", new LuaCSFunction(ObjectWrap.GetClassType)), new LuaMethod("__tostring", new LuaCSFunction(ObjectWrap.Lua_ToString)), new LuaMethod("__eq", new LuaCSFunction(ObjectWrap.Lua_Eq)) };
        LuaField[] fields = new LuaField[] { new LuaField("name", new LuaCSFunction(ObjectWrap.get_name), new LuaCSFunction(ObjectWrap.set_name)), new LuaField("hideFlags", new LuaCSFunction(ObjectWrap.get_hideFlags), new LuaCSFunction(ObjectWrap.set_hideFlags)) };
        LuaScriptMgr.RegisterLib(L, "UnityEngine.Object", typeof(Object), regs, fields, typeof(object));
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_hideFlags(IntPtr L)
    {
        Object luaObject = (Object) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name hideFlags");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index hideFlags on a nil value");
            }
        }
        luaObject.set_hideFlags((HideFlags) ((int) LuaScriptMgr.GetNetObject(L, 3, typeof(HideFlags))));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_name(IntPtr L)
    {
        Object luaObject = (Object) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name name");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index name on a nil value");
            }
        }
        luaObject.set_name(LuaScriptMgr.GetString(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int ToString(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        string str = ((Object) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Object")).ToString();
        LuaScriptMgr.Push(L, str);
        return 1;
    }
}

