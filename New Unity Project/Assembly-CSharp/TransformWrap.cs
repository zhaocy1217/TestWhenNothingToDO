using com.tencent.pandora;
using System;
using System.Collections;
using UnityEngine;

public class TransformWrap
{
    private static Type classType = typeof(Transform);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _CreateTransform(IntPtr L)
    {
        LuaDLL.luaL_error(L, "Transform class does not have a constructor function");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int DetachChildren(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        ((Transform) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Transform")).DetachChildren();
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Find(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Transform transform = (Transform) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Transform");
        string luaString = LuaScriptMgr.GetLuaString(L, 2);
        Transform transform2 = transform.Find(luaString);
        LuaScriptMgr.Push(L, (Object) transform2);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int FindChild(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Transform transform = (Transform) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Transform");
        string luaString = LuaScriptMgr.GetLuaString(L, 2);
        Transform transform2 = transform.FindChild(luaString);
        LuaScriptMgr.Push(L, (Object) transform2);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_childCount(IntPtr L)
    {
        Transform luaObject = (Transform) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name childCount");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index childCount on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_childCount());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_eulerAngles(IntPtr L)
    {
        Transform luaObject = (Transform) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name eulerAngles");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index eulerAngles on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_eulerAngles());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_forward(IntPtr L)
    {
        Transform luaObject = (Transform) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name forward");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index forward on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_forward());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_hasChanged(IntPtr L)
    {
        Transform luaObject = (Transform) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name hasChanged");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index hasChanged on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_hasChanged());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_localEulerAngles(IntPtr L)
    {
        Transform luaObject = (Transform) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name localEulerAngles");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index localEulerAngles on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_localEulerAngles());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_localPosition(IntPtr L)
    {
        Transform luaObject = (Transform) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name localPosition");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index localPosition on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_localPosition());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_localRotation(IntPtr L)
    {
        Transform luaObject = (Transform) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name localRotation");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index localRotation on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_localRotation());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_localScale(IntPtr L)
    {
        Transform luaObject = (Transform) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name localScale");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index localScale on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_localScale());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_localToWorldMatrix(IntPtr L)
    {
        Transform luaObject = (Transform) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name localToWorldMatrix");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index localToWorldMatrix on a nil value");
            }
        }
        LuaScriptMgr.PushValue(L, luaObject.get_localToWorldMatrix());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_lossyScale(IntPtr L)
    {
        Transform luaObject = (Transform) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name lossyScale");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index lossyScale on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_lossyScale());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_parent(IntPtr L)
    {
        Transform luaObject = (Transform) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name parent");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index parent on a nil value");
            }
        }
        LuaScriptMgr.Push(L, (Object) luaObject.get_parent());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_position(IntPtr L)
    {
        Transform luaObject = (Transform) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name position");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index position on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_position());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_right(IntPtr L)
    {
        Transform luaObject = (Transform) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name right");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index right on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_right());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_root(IntPtr L)
    {
        Transform luaObject = (Transform) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name root");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index root on a nil value");
            }
        }
        LuaScriptMgr.Push(L, (Object) luaObject.get_root());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_rotation(IntPtr L)
    {
        Transform luaObject = (Transform) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name rotation");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index rotation on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_rotation());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_up(IntPtr L)
    {
        Transform luaObject = (Transform) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name up");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index up on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_up());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_worldToLocalMatrix(IntPtr L)
    {
        Transform luaObject = (Transform) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name worldToLocalMatrix");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index worldToLocalMatrix on a nil value");
            }
        }
        LuaScriptMgr.PushValue(L, luaObject.get_worldToLocalMatrix());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetChild(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Transform transform = (Transform) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Transform");
        int number = (int) LuaScriptMgr.GetNumber(L, 2);
        Transform child = transform.GetChild(number);
        LuaScriptMgr.Push(L, (Object) child);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetClassType(IntPtr L)
    {
        LuaScriptMgr.Push(L, classType);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetEnumerator(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        IEnumerator o = ((Transform) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Transform")).GetEnumerator();
        LuaScriptMgr.Push(L, o);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetSiblingIndex(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        int siblingIndex = ((Transform) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Transform")).GetSiblingIndex();
        LuaScriptMgr.Push(L, siblingIndex);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int InverseTransformDirection(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 2:
            {
                Transform transform = (Transform) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Transform");
                Vector3 vector = LuaScriptMgr.GetVector3(L, 2);
                Vector3 vector2 = transform.InverseTransformDirection(vector);
                LuaScriptMgr.Push(L, vector2);
                return 1;
            }
            case 4:
            {
                Transform transform2 = (Transform) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Transform");
                float number = (float) LuaScriptMgr.GetNumber(L, 2);
                float num3 = (float) LuaScriptMgr.GetNumber(L, 3);
                float num4 = (float) LuaScriptMgr.GetNumber(L, 4);
                Vector3 vector3 = transform2.InverseTransformDirection(number, num3, num4);
                LuaScriptMgr.Push(L, vector3);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Transform.InverseTransformDirection");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int InverseTransformPoint(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 2:
            {
                Transform transform = (Transform) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Transform");
                Vector3 vector = LuaScriptMgr.GetVector3(L, 2);
                Vector3 vector2 = transform.InverseTransformPoint(vector);
                LuaScriptMgr.Push(L, vector2);
                return 1;
            }
            case 4:
            {
                Transform transform2 = (Transform) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Transform");
                float number = (float) LuaScriptMgr.GetNumber(L, 2);
                float num3 = (float) LuaScriptMgr.GetNumber(L, 3);
                float num4 = (float) LuaScriptMgr.GetNumber(L, 4);
                Vector3 vector3 = transform2.InverseTransformPoint(number, num3, num4);
                LuaScriptMgr.Push(L, vector3);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Transform.InverseTransformPoint");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int InverseTransformVector(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 2:
            {
                Transform transform = (Transform) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Transform");
                Vector3 vector = LuaScriptMgr.GetVector3(L, 2);
                Vector3 vector2 = transform.InverseTransformVector(vector);
                LuaScriptMgr.Push(L, vector2);
                return 1;
            }
            case 4:
            {
                Transform transform2 = (Transform) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Transform");
                float number = (float) LuaScriptMgr.GetNumber(L, 2);
                float num3 = (float) LuaScriptMgr.GetNumber(L, 3);
                float num4 = (float) LuaScriptMgr.GetNumber(L, 4);
                Vector3 vector3 = transform2.InverseTransformVector(number, num3, num4);
                LuaScriptMgr.Push(L, vector3);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Transform.InverseTransformVector");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int IsChildOf(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Transform transform = (Transform) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Transform");
        Transform transform2 = LuaScriptMgr.GetUnityObject(L, 2, typeof(Transform));
        bool b = transform.IsChildOf(transform2);
        LuaScriptMgr.Push(L, b);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int LookAt(IntPtr L)
    {
        int num = LuaDLL.lua_gettop(L);
        if ((num == 2) && LuaScriptMgr.CheckTypes(L, 1, typeof(Transform), typeof(LuaTable)))
        {
            Transform transform = (Transform) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Transform");
            Vector3 vector = LuaScriptMgr.GetVector3(L, 2);
            transform.LookAt(vector);
            return 0;
        }
        if ((num == 2) && LuaScriptMgr.CheckTypes(L, 1, typeof(Transform), typeof(Transform)))
        {
            Transform transform2 = (Transform) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Transform");
            Transform luaObject = (Transform) LuaScriptMgr.GetLuaObject(L, 2);
            transform2.LookAt(luaObject);
            return 0;
        }
        if ((num == 3) && LuaScriptMgr.CheckTypes(L, 1, typeof(Transform), typeof(LuaTable), typeof(LuaTable)))
        {
            Transform transform4 = (Transform) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Transform");
            Vector3 vector2 = LuaScriptMgr.GetVector3(L, 2);
            Vector3 vector3 = LuaScriptMgr.GetVector3(L, 3);
            transform4.LookAt(vector2, vector3);
            return 0;
        }
        if ((num == 3) && LuaScriptMgr.CheckTypes(L, 1, typeof(Transform), typeof(Transform), typeof(LuaTable)))
        {
            Transform transform5 = (Transform) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Transform");
            Transform transform6 = (Transform) LuaScriptMgr.GetLuaObject(L, 2);
            Vector3 vector4 = LuaScriptMgr.GetVector3(L, 3);
            transform5.LookAt(transform6, vector4);
            return 0;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Transform.LookAt");
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
        LuaMethod[] regs = new LuaMethod[] { 
            new LuaMethod("SetParent", new LuaCSFunction(TransformWrap.SetParent)), new LuaMethod("Translate", new LuaCSFunction(TransformWrap.Translate)), new LuaMethod("Rotate", new LuaCSFunction(TransformWrap.Rotate)), new LuaMethod("RotateAround", new LuaCSFunction(TransformWrap.RotateAround)), new LuaMethod("LookAt", new LuaCSFunction(TransformWrap.LookAt)), new LuaMethod("TransformDirection", new LuaCSFunction(TransformWrap.TransformDirection)), new LuaMethod("InverseTransformDirection", new LuaCSFunction(TransformWrap.InverseTransformDirection)), new LuaMethod("TransformVector", new LuaCSFunction(TransformWrap.TransformVector)), new LuaMethod("InverseTransformVector", new LuaCSFunction(TransformWrap.InverseTransformVector)), new LuaMethod("TransformPoint", new LuaCSFunction(TransformWrap.TransformPoint)), new LuaMethod("InverseTransformPoint", new LuaCSFunction(TransformWrap.InverseTransformPoint)), new LuaMethod("DetachChildren", new LuaCSFunction(TransformWrap.DetachChildren)), new LuaMethod("SetAsFirstSibling", new LuaCSFunction(TransformWrap.SetAsFirstSibling)), new LuaMethod("SetAsLastSibling", new LuaCSFunction(TransformWrap.SetAsLastSibling)), new LuaMethod("SetSiblingIndex", new LuaCSFunction(TransformWrap.SetSiblingIndex)), new LuaMethod("GetSiblingIndex", new LuaCSFunction(TransformWrap.GetSiblingIndex)), 
            new LuaMethod("Find", new LuaCSFunction(TransformWrap.Find)), new LuaMethod("IsChildOf", new LuaCSFunction(TransformWrap.IsChildOf)), new LuaMethod("FindChild", new LuaCSFunction(TransformWrap.FindChild)), new LuaMethod("GetEnumerator", new LuaCSFunction(TransformWrap.GetEnumerator)), new LuaMethod("GetChild", new LuaCSFunction(TransformWrap.GetChild)), new LuaMethod("New", new LuaCSFunction(TransformWrap._CreateTransform)), new LuaMethod("GetClassType", new LuaCSFunction(TransformWrap.GetClassType)), new LuaMethod("__eq", new LuaCSFunction(TransformWrap.Lua_Eq))
         };
        LuaField[] fields = new LuaField[] { 
            new LuaField("position", new LuaCSFunction(TransformWrap.get_position), new LuaCSFunction(TransformWrap.set_position)), new LuaField("localPosition", new LuaCSFunction(TransformWrap.get_localPosition), new LuaCSFunction(TransformWrap.set_localPosition)), new LuaField("eulerAngles", new LuaCSFunction(TransformWrap.get_eulerAngles), new LuaCSFunction(TransformWrap.set_eulerAngles)), new LuaField("localEulerAngles", new LuaCSFunction(TransformWrap.get_localEulerAngles), new LuaCSFunction(TransformWrap.set_localEulerAngles)), new LuaField("right", new LuaCSFunction(TransformWrap.get_right), new LuaCSFunction(TransformWrap.set_right)), new LuaField("up", new LuaCSFunction(TransformWrap.get_up), new LuaCSFunction(TransformWrap.set_up)), new LuaField("forward", new LuaCSFunction(TransformWrap.get_forward), new LuaCSFunction(TransformWrap.set_forward)), new LuaField("rotation", new LuaCSFunction(TransformWrap.get_rotation), new LuaCSFunction(TransformWrap.set_rotation)), new LuaField("localRotation", new LuaCSFunction(TransformWrap.get_localRotation), new LuaCSFunction(TransformWrap.set_localRotation)), new LuaField("localScale", new LuaCSFunction(TransformWrap.get_localScale), new LuaCSFunction(TransformWrap.set_localScale)), new LuaField("parent", new LuaCSFunction(TransformWrap.get_parent), new LuaCSFunction(TransformWrap.set_parent)), new LuaField("worldToLocalMatrix", new LuaCSFunction(TransformWrap.get_worldToLocalMatrix), null), new LuaField("localToWorldMatrix", new LuaCSFunction(TransformWrap.get_localToWorldMatrix), null), new LuaField("root", new LuaCSFunction(TransformWrap.get_root), null), new LuaField("childCount", new LuaCSFunction(TransformWrap.get_childCount), null), new LuaField("lossyScale", new LuaCSFunction(TransformWrap.get_lossyScale), null), 
            new LuaField("hasChanged", new LuaCSFunction(TransformWrap.get_hasChanged), new LuaCSFunction(TransformWrap.set_hasChanged))
         };
        LuaScriptMgr.RegisterLib(L, "UnityEngine.Transform", typeof(Transform), regs, fields, typeof(Component));
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Rotate(IntPtr L)
    {
        int num = LuaDLL.lua_gettop(L);
        if (num == 2)
        {
            Transform transform = (Transform) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Transform");
            Vector3 vector = LuaScriptMgr.GetVector3(L, 2);
            transform.Rotate(vector);
            return 0;
        }
        if ((num == 3) && LuaScriptMgr.CheckTypes(L, 1, typeof(Transform), typeof(LuaTable), typeof(float)))
        {
            Transform transform2 = (Transform) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Transform");
            Vector3 vector2 = LuaScriptMgr.GetVector3(L, 2);
            float num2 = (float) LuaDLL.lua_tonumber(L, 3);
            transform2.Rotate(vector2, num2);
            return 0;
        }
        if ((num == 3) && LuaScriptMgr.CheckTypes(L, 1, typeof(Transform), typeof(LuaTable), typeof(Space)))
        {
            Transform transform3 = (Transform) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Transform");
            Vector3 vector3 = LuaScriptMgr.GetVector3(L, 2);
            Space luaObject = (Space) ((int) LuaScriptMgr.GetLuaObject(L, 3));
            transform3.Rotate(vector3, luaObject);
            return 0;
        }
        if ((num == 4) && LuaScriptMgr.CheckTypes(L, 1, typeof(Transform), typeof(LuaTable), typeof(float), typeof(Space)))
        {
            Transform transform4 = (Transform) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Transform");
            Vector3 vector4 = LuaScriptMgr.GetVector3(L, 2);
            float num3 = (float) LuaDLL.lua_tonumber(L, 3);
            Space space2 = (Space) ((int) LuaScriptMgr.GetLuaObject(L, 4));
            transform4.Rotate(vector4, num3, space2);
            return 0;
        }
        if ((num == 4) && LuaScriptMgr.CheckTypes(L, 1, typeof(Transform), typeof(float), typeof(float), typeof(float)))
        {
            Transform transform5 = (Transform) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Transform");
            float num4 = (float) LuaDLL.lua_tonumber(L, 2);
            float num5 = (float) LuaDLL.lua_tonumber(L, 3);
            float num6 = (float) LuaDLL.lua_tonumber(L, 4);
            transform5.Rotate(num4, num5, num6);
            return 0;
        }
        if (num == 5)
        {
            Transform transform6 = (Transform) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Transform");
            float number = (float) LuaScriptMgr.GetNumber(L, 2);
            float num8 = (float) LuaScriptMgr.GetNumber(L, 3);
            float num9 = (float) LuaScriptMgr.GetNumber(L, 4);
            Space space3 = (Space) ((int) LuaScriptMgr.GetNetObject(L, 5, typeof(Space)));
            transform6.Rotate(number, num8, num9, space3);
            return 0;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Transform.Rotate");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int RotateAround(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 4);
        Transform transform = (Transform) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Transform");
        Vector3 vector = LuaScriptMgr.GetVector3(L, 2);
        Vector3 vector2 = LuaScriptMgr.GetVector3(L, 3);
        float number = (float) LuaScriptMgr.GetNumber(L, 4);
        transform.RotateAround(vector, vector2, number);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_eulerAngles(IntPtr L)
    {
        Transform luaObject = (Transform) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name eulerAngles");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index eulerAngles on a nil value");
            }
        }
        luaObject.set_eulerAngles(LuaScriptMgr.GetVector3(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_forward(IntPtr L)
    {
        Transform luaObject = (Transform) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name forward");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index forward on a nil value");
            }
        }
        luaObject.set_forward(LuaScriptMgr.GetVector3(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_hasChanged(IntPtr L)
    {
        Transform luaObject = (Transform) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name hasChanged");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index hasChanged on a nil value");
            }
        }
        luaObject.set_hasChanged(LuaScriptMgr.GetBoolean(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_localEulerAngles(IntPtr L)
    {
        Transform luaObject = (Transform) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name localEulerAngles");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index localEulerAngles on a nil value");
            }
        }
        luaObject.set_localEulerAngles(LuaScriptMgr.GetVector3(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_localPosition(IntPtr L)
    {
        Transform luaObject = (Transform) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name localPosition");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index localPosition on a nil value");
            }
        }
        luaObject.set_localPosition(LuaScriptMgr.GetVector3(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_localRotation(IntPtr L)
    {
        Transform luaObject = (Transform) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name localRotation");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index localRotation on a nil value");
            }
        }
        luaObject.set_localRotation(LuaScriptMgr.GetQuaternion(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_localScale(IntPtr L)
    {
        Transform luaObject = (Transform) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name localScale");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index localScale on a nil value");
            }
        }
        luaObject.set_localScale(LuaScriptMgr.GetVector3(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_parent(IntPtr L)
    {
        Transform luaObject = (Transform) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name parent");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index parent on a nil value");
            }
        }
        luaObject.set_parent((Transform) LuaScriptMgr.GetUnityObject(L, 3, typeof(Transform)));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_position(IntPtr L)
    {
        Transform luaObject = (Transform) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name position");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index position on a nil value");
            }
        }
        luaObject.set_position(LuaScriptMgr.GetVector3(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_right(IntPtr L)
    {
        Transform luaObject = (Transform) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name right");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index right on a nil value");
            }
        }
        luaObject.set_right(LuaScriptMgr.GetVector3(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_rotation(IntPtr L)
    {
        Transform luaObject = (Transform) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name rotation");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index rotation on a nil value");
            }
        }
        luaObject.set_rotation(LuaScriptMgr.GetQuaternion(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_up(IntPtr L)
    {
        Transform luaObject = (Transform) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name up");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index up on a nil value");
            }
        }
        luaObject.set_up(LuaScriptMgr.GetVector3(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int SetAsFirstSibling(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        ((Transform) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Transform")).SetAsFirstSibling();
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int SetAsLastSibling(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        ((Transform) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Transform")).SetAsLastSibling();
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int SetParent(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 2:
            {
                Transform transform = (Transform) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Transform");
                Transform transform2 = LuaScriptMgr.GetUnityObject(L, 2, typeof(Transform));
                transform.SetParent(transform2);
                return 0;
            }
            case 3:
            {
                Transform transform3 = (Transform) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Transform");
                Transform transform4 = LuaScriptMgr.GetUnityObject(L, 2, typeof(Transform));
                bool boolean = LuaScriptMgr.GetBoolean(L, 3);
                transform3.SetParent(transform4, boolean);
                return 0;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Transform.SetParent");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int SetSiblingIndex(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Transform transform = (Transform) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Transform");
        int number = (int) LuaScriptMgr.GetNumber(L, 2);
        transform.SetSiblingIndex(number);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int TransformDirection(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 2:
            {
                Transform transform = (Transform) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Transform");
                Vector3 vector = LuaScriptMgr.GetVector3(L, 2);
                Vector3 vector2 = transform.TransformDirection(vector);
                LuaScriptMgr.Push(L, vector2);
                return 1;
            }
            case 4:
            {
                Transform transform2 = (Transform) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Transform");
                float number = (float) LuaScriptMgr.GetNumber(L, 2);
                float num3 = (float) LuaScriptMgr.GetNumber(L, 3);
                float num4 = (float) LuaScriptMgr.GetNumber(L, 4);
                Vector3 vector3 = transform2.TransformDirection(number, num3, num4);
                LuaScriptMgr.Push(L, vector3);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Transform.TransformDirection");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int TransformPoint(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 2:
            {
                Transform transform = (Transform) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Transform");
                Vector3 vector = LuaScriptMgr.GetVector3(L, 2);
                Vector3 vector2 = transform.TransformPoint(vector);
                LuaScriptMgr.Push(L, vector2);
                return 1;
            }
            case 4:
            {
                Transform transform2 = (Transform) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Transform");
                float number = (float) LuaScriptMgr.GetNumber(L, 2);
                float num3 = (float) LuaScriptMgr.GetNumber(L, 3);
                float num4 = (float) LuaScriptMgr.GetNumber(L, 4);
                Vector3 vector3 = transform2.TransformPoint(number, num3, num4);
                LuaScriptMgr.Push(L, vector3);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Transform.TransformPoint");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int TransformVector(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 2:
            {
                Transform transform = (Transform) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Transform");
                Vector3 vector = LuaScriptMgr.GetVector3(L, 2);
                Vector3 vector2 = transform.TransformVector(vector);
                LuaScriptMgr.Push(L, vector2);
                return 1;
            }
            case 4:
            {
                Transform transform2 = (Transform) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Transform");
                float number = (float) LuaScriptMgr.GetNumber(L, 2);
                float num3 = (float) LuaScriptMgr.GetNumber(L, 3);
                float num4 = (float) LuaScriptMgr.GetNumber(L, 4);
                Vector3 vector3 = transform2.TransformVector(number, num3, num4);
                LuaScriptMgr.Push(L, vector3);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Transform.TransformVector");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Translate(IntPtr L)
    {
        int num = LuaDLL.lua_gettop(L);
        if (num == 2)
        {
            Transform transform = (Transform) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Transform");
            Vector3 vector = LuaScriptMgr.GetVector3(L, 2);
            transform.Translate(vector);
            return 0;
        }
        if ((num == 3) && LuaScriptMgr.CheckTypes(L, 1, typeof(Transform), typeof(LuaTable), typeof(Transform)))
        {
            Transform transform2 = (Transform) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Transform");
            Vector3 vector2 = LuaScriptMgr.GetVector3(L, 2);
            Transform luaObject = (Transform) LuaScriptMgr.GetLuaObject(L, 3);
            transform2.Translate(vector2, luaObject);
            return 0;
        }
        if ((num == 3) && LuaScriptMgr.CheckTypes(L, 1, typeof(Transform), typeof(LuaTable), typeof(Space)))
        {
            Transform transform4 = (Transform) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Transform");
            Vector3 vector3 = LuaScriptMgr.GetVector3(L, 2);
            Space space = (Space) ((int) LuaScriptMgr.GetLuaObject(L, 3));
            transform4.Translate(vector3, space);
            return 0;
        }
        if (num == 4)
        {
            Transform transform5 = (Transform) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Transform");
            float number = (float) LuaScriptMgr.GetNumber(L, 2);
            float num3 = (float) LuaScriptMgr.GetNumber(L, 3);
            float num4 = (float) LuaScriptMgr.GetNumber(L, 4);
            transform5.Translate(number, num3, num4);
            return 0;
        }
        if ((num == 5) && LuaScriptMgr.CheckTypes(L, 1, typeof(Transform), typeof(float), typeof(float), typeof(float), typeof(Transform)))
        {
            Transform transform6 = (Transform) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Transform");
            float num5 = (float) LuaDLL.lua_tonumber(L, 2);
            float num6 = (float) LuaDLL.lua_tonumber(L, 3);
            float num7 = (float) LuaDLL.lua_tonumber(L, 4);
            Transform transform7 = (Transform) LuaScriptMgr.GetLuaObject(L, 5);
            transform6.Translate(num5, num6, num7, transform7);
            return 0;
        }
        if ((num == 5) && LuaScriptMgr.CheckTypes(L, 1, typeof(Transform), typeof(float), typeof(float), typeof(float), typeof(Space)))
        {
            Transform transform8 = (Transform) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Transform");
            float num8 = (float) LuaDLL.lua_tonumber(L, 2);
            float num9 = (float) LuaDLL.lua_tonumber(L, 3);
            float num10 = (float) LuaDLL.lua_tonumber(L, 4);
            Space space2 = (Space) ((int) LuaScriptMgr.GetLuaObject(L, 5));
            transform8.Translate(num8, num9, num10, space2);
            return 0;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Transform.Translate");
        return 0;
    }
}

