using com.tencent.pandora;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnityEngine_UI_GraphicWrap
{
    private static Type classType = typeof(Graphic);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _CreateUnityEngine_UI_Graphic(IntPtr L)
    {
        LuaDLL.luaL_error(L, "UnityEngine.UI.Graphic class does not have a constructor function");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int CrossFadeAlpha(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 4);
        Graphic graphic = (Graphic) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Graphic");
        float number = (float) LuaScriptMgr.GetNumber(L, 2);
        float num2 = (float) LuaScriptMgr.GetNumber(L, 3);
        bool boolean = LuaScriptMgr.GetBoolean(L, 4);
        graphic.CrossFadeAlpha(number, num2, boolean);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int CrossFadeColor(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 5);
        Graphic graphic = (Graphic) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Graphic");
        Color color = LuaScriptMgr.GetColor(L, 2);
        float number = (float) LuaScriptMgr.GetNumber(L, 3);
        bool boolean = LuaScriptMgr.GetBoolean(L, 4);
        bool flag2 = LuaScriptMgr.GetBoolean(L, 5);
        graphic.CrossFadeColor(color, number, boolean, flag2);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_canvas(IntPtr L)
    {
        Graphic luaObject = (Graphic) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name canvas");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index canvas on a nil value");
            }
        }
        LuaScriptMgr.Push(L, (Object) luaObject.get_canvas());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_canvasRenderer(IntPtr L)
    {
        Graphic luaObject = (Graphic) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name canvasRenderer");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index canvasRenderer on a nil value");
            }
        }
        LuaScriptMgr.Push(L, (Object) luaObject.get_canvasRenderer());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_color(IntPtr L)
    {
        Graphic luaObject = (Graphic) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name color");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index color on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_color());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_defaultGraphicMaterial(IntPtr L)
    {
        LuaScriptMgr.Push(L, (Object) Graphic.get_defaultGraphicMaterial());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_defaultMaterial(IntPtr L)
    {
        Graphic luaObject = (Graphic) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name defaultMaterial");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index defaultMaterial on a nil value");
            }
        }
        LuaScriptMgr.Push(L, (Object) luaObject.get_defaultMaterial());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_depth(IntPtr L)
    {
        Graphic luaObject = (Graphic) LuaScriptMgr.GetLuaObject(L, 1);
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
    private static int get_mainTexture(IntPtr L)
    {
        Graphic luaObject = (Graphic) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name mainTexture");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index mainTexture on a nil value");
            }
        }
        LuaScriptMgr.Push(L, (Object) luaObject.get_mainTexture());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_material(IntPtr L)
    {
        Graphic luaObject = (Graphic) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name material");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index material on a nil value");
            }
        }
        LuaScriptMgr.Push(L, (Object) luaObject.get_material());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_materialForRendering(IntPtr L)
    {
        Graphic luaObject = (Graphic) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name materialForRendering");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index materialForRendering on a nil value");
            }
        }
        LuaScriptMgr.Push(L, (Object) luaObject.get_materialForRendering());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_rectTransform(IntPtr L)
    {
        Graphic luaObject = (Graphic) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name rectTransform");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index rectTransform on a nil value");
            }
        }
        LuaScriptMgr.Push(L, (Object) luaObject.get_rectTransform());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetClassType(IntPtr L)
    {
        LuaScriptMgr.Push(L, classType);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetPixelAdjustedRect(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        Rect pixelAdjustedRect = ((Graphic) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Graphic")).GetPixelAdjustedRect();
        LuaScriptMgr.PushValue(L, pixelAdjustedRect);
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
    private static int PixelAdjustPoint(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Graphic graphic = (Graphic) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Graphic");
        Vector2 vector = LuaScriptMgr.GetVector2(L, 2);
        Vector2 vector2 = graphic.PixelAdjustPoint(vector);
        LuaScriptMgr.Push(L, vector2);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Raycast(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 3);
        Graphic graphic = (Graphic) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Graphic");
        Vector2 vector = LuaScriptMgr.GetVector2(L, 2);
        Camera camera = LuaScriptMgr.GetUnityObject(L, 3, typeof(Camera));
        bool b = graphic.Raycast(vector, camera);
        LuaScriptMgr.Push(L, b);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Rebuild(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Graphic graphic = (Graphic) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Graphic");
        CanvasUpdate update = (CanvasUpdate) ((int) LuaScriptMgr.GetNetObject(L, 2, typeof(CanvasUpdate)));
        graphic.Rebuild(update);
        return 0;
    }

    public static void Register(IntPtr L)
    {
        LuaMethod[] regs = new LuaMethod[] { 
            new LuaMethod("SetAllDirty", new LuaCSFunction(UnityEngine_UI_GraphicWrap.SetAllDirty)), new LuaMethod("SetLayoutDirty", new LuaCSFunction(UnityEngine_UI_GraphicWrap.SetLayoutDirty)), new LuaMethod("SetVerticesDirty", new LuaCSFunction(UnityEngine_UI_GraphicWrap.SetVerticesDirty)), new LuaMethod("SetMaterialDirty", new LuaCSFunction(UnityEngine_UI_GraphicWrap.SetMaterialDirty)), new LuaMethod("Rebuild", new LuaCSFunction(UnityEngine_UI_GraphicWrap.Rebuild)), new LuaMethod("SetNativeSize", new LuaCSFunction(UnityEngine_UI_GraphicWrap.SetNativeSize)), new LuaMethod("Raycast", new LuaCSFunction(UnityEngine_UI_GraphicWrap.Raycast)), new LuaMethod("PixelAdjustPoint", new LuaCSFunction(UnityEngine_UI_GraphicWrap.PixelAdjustPoint)), new LuaMethod("GetPixelAdjustedRect", new LuaCSFunction(UnityEngine_UI_GraphicWrap.GetPixelAdjustedRect)), new LuaMethod("CrossFadeColor", new LuaCSFunction(UnityEngine_UI_GraphicWrap.CrossFadeColor)), new LuaMethod("CrossFadeAlpha", new LuaCSFunction(UnityEngine_UI_GraphicWrap.CrossFadeAlpha)), new LuaMethod("RegisterDirtyLayoutCallback", new LuaCSFunction(UnityEngine_UI_GraphicWrap.RegisterDirtyLayoutCallback)), new LuaMethod("UnregisterDirtyLayoutCallback", new LuaCSFunction(UnityEngine_UI_GraphicWrap.UnregisterDirtyLayoutCallback)), new LuaMethod("RegisterDirtyVerticesCallback", new LuaCSFunction(UnityEngine_UI_GraphicWrap.RegisterDirtyVerticesCallback)), new LuaMethod("UnregisterDirtyVerticesCallback", new LuaCSFunction(UnityEngine_UI_GraphicWrap.UnregisterDirtyVerticesCallback)), new LuaMethod("RegisterDirtyMaterialCallback", new LuaCSFunction(UnityEngine_UI_GraphicWrap.RegisterDirtyMaterialCallback)), 
            new LuaMethod("UnregisterDirtyMaterialCallback", new LuaCSFunction(UnityEngine_UI_GraphicWrap.UnregisterDirtyMaterialCallback)), new LuaMethod("New", new LuaCSFunction(UnityEngine_UI_GraphicWrap._CreateUnityEngine_UI_Graphic)), new LuaMethod("GetClassType", new LuaCSFunction(UnityEngine_UI_GraphicWrap.GetClassType)), new LuaMethod("__eq", new LuaCSFunction(UnityEngine_UI_GraphicWrap.Lua_Eq))
         };
        LuaField[] fields = new LuaField[] { new LuaField("defaultGraphicMaterial", new LuaCSFunction(UnityEngine_UI_GraphicWrap.get_defaultGraphicMaterial), null), new LuaField("color", new LuaCSFunction(UnityEngine_UI_GraphicWrap.get_color), new LuaCSFunction(UnityEngine_UI_GraphicWrap.set_color)), new LuaField("depth", new LuaCSFunction(UnityEngine_UI_GraphicWrap.get_depth), null), new LuaField("rectTransform", new LuaCSFunction(UnityEngine_UI_GraphicWrap.get_rectTransform), null), new LuaField("canvas", new LuaCSFunction(UnityEngine_UI_GraphicWrap.get_canvas), null), new LuaField("canvasRenderer", new LuaCSFunction(UnityEngine_UI_GraphicWrap.get_canvasRenderer), null), new LuaField("defaultMaterial", new LuaCSFunction(UnityEngine_UI_GraphicWrap.get_defaultMaterial), null), new LuaField("material", new LuaCSFunction(UnityEngine_UI_GraphicWrap.get_material), new LuaCSFunction(UnityEngine_UI_GraphicWrap.set_material)), new LuaField("materialForRendering", new LuaCSFunction(UnityEngine_UI_GraphicWrap.get_materialForRendering), null), new LuaField("mainTexture", new LuaCSFunction(UnityEngine_UI_GraphicWrap.get_mainTexture), null) };
        LuaScriptMgr.RegisterLib(L, "UnityEngine.UI.Graphic", typeof(Graphic), regs, fields, typeof(UIBehaviour));
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int RegisterDirtyLayoutCallback(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Graphic graphic = (Graphic) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Graphic");
        UnityAction action = null;
        if (LuaDLL.lua_type(L, 2) != LuaTypes.LUA_TFUNCTION)
        {
            action = (UnityAction) LuaScriptMgr.GetNetObject(L, 2, typeof(UnityAction));
        }
        else
        {
            <RegisterDirtyLayoutCallback>c__AnonStorey51 storey = new <RegisterDirtyLayoutCallback>c__AnonStorey51();
            storey.func = LuaScriptMgr.GetLuaFunction(L, 2);
            action = new UnityAction(storey, (IntPtr) this.<>m__21);
        }
        graphic.RegisterDirtyLayoutCallback(action);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int RegisterDirtyMaterialCallback(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Graphic graphic = (Graphic) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Graphic");
        UnityAction action = null;
        if (LuaDLL.lua_type(L, 2) != LuaTypes.LUA_TFUNCTION)
        {
            action = (UnityAction) LuaScriptMgr.GetNetObject(L, 2, typeof(UnityAction));
        }
        else
        {
            <RegisterDirtyMaterialCallback>c__AnonStorey55 storey = new <RegisterDirtyMaterialCallback>c__AnonStorey55();
            storey.func = LuaScriptMgr.GetLuaFunction(L, 2);
            action = new UnityAction(storey, (IntPtr) this.<>m__25);
        }
        graphic.RegisterDirtyMaterialCallback(action);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int RegisterDirtyVerticesCallback(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Graphic graphic = (Graphic) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Graphic");
        UnityAction action = null;
        if (LuaDLL.lua_type(L, 2) != LuaTypes.LUA_TFUNCTION)
        {
            action = (UnityAction) LuaScriptMgr.GetNetObject(L, 2, typeof(UnityAction));
        }
        else
        {
            <RegisterDirtyVerticesCallback>c__AnonStorey53 storey = new <RegisterDirtyVerticesCallback>c__AnonStorey53();
            storey.func = LuaScriptMgr.GetLuaFunction(L, 2);
            action = new UnityAction(storey, (IntPtr) this.<>m__23);
        }
        graphic.RegisterDirtyVerticesCallback(action);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_color(IntPtr L)
    {
        Graphic luaObject = (Graphic) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name color");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index color on a nil value");
            }
        }
        luaObject.set_color(LuaScriptMgr.GetColor(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_material(IntPtr L)
    {
        Graphic luaObject = (Graphic) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name material");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index material on a nil value");
            }
        }
        luaObject.set_material((Material) LuaScriptMgr.GetUnityObject(L, 3, typeof(Material)));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int SetAllDirty(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        ((Graphic) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Graphic")).SetAllDirty();
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int SetLayoutDirty(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        ((Graphic) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Graphic")).SetLayoutDirty();
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int SetMaterialDirty(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        ((Graphic) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Graphic")).SetMaterialDirty();
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int SetNativeSize(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        ((Graphic) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Graphic")).SetNativeSize();
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int SetVerticesDirty(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        ((Graphic) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Graphic")).SetVerticesDirty();
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int UnregisterDirtyLayoutCallback(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Graphic graphic = (Graphic) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Graphic");
        UnityAction action = null;
        if (LuaDLL.lua_type(L, 2) != LuaTypes.LUA_TFUNCTION)
        {
            action = (UnityAction) LuaScriptMgr.GetNetObject(L, 2, typeof(UnityAction));
        }
        else
        {
            <UnregisterDirtyLayoutCallback>c__AnonStorey52 storey = new <UnregisterDirtyLayoutCallback>c__AnonStorey52();
            storey.func = LuaScriptMgr.GetLuaFunction(L, 2);
            action = new UnityAction(storey, (IntPtr) this.<>m__22);
        }
        graphic.UnregisterDirtyLayoutCallback(action);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int UnregisterDirtyMaterialCallback(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Graphic graphic = (Graphic) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Graphic");
        UnityAction action = null;
        if (LuaDLL.lua_type(L, 2) != LuaTypes.LUA_TFUNCTION)
        {
            action = (UnityAction) LuaScriptMgr.GetNetObject(L, 2, typeof(UnityAction));
        }
        else
        {
            <UnregisterDirtyMaterialCallback>c__AnonStorey56 storey = new <UnregisterDirtyMaterialCallback>c__AnonStorey56();
            storey.func = LuaScriptMgr.GetLuaFunction(L, 2);
            action = new UnityAction(storey, (IntPtr) this.<>m__26);
        }
        graphic.UnregisterDirtyMaterialCallback(action);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int UnregisterDirtyVerticesCallback(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Graphic graphic = (Graphic) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Graphic");
        UnityAction action = null;
        if (LuaDLL.lua_type(L, 2) != LuaTypes.LUA_TFUNCTION)
        {
            action = (UnityAction) LuaScriptMgr.GetNetObject(L, 2, typeof(UnityAction));
        }
        else
        {
            <UnregisterDirtyVerticesCallback>c__AnonStorey54 storey = new <UnregisterDirtyVerticesCallback>c__AnonStorey54();
            storey.func = LuaScriptMgr.GetLuaFunction(L, 2);
            action = new UnityAction(storey, (IntPtr) this.<>m__24);
        }
        graphic.UnregisterDirtyVerticesCallback(action);
        return 0;
    }

    [CompilerGenerated]
    private sealed class <RegisterDirtyLayoutCallback>c__AnonStorey51
    {
        internal LuaFunction func;

        internal void <>m__21()
        {
            this.func.Call();
        }
    }

    [CompilerGenerated]
    private sealed class <RegisterDirtyMaterialCallback>c__AnonStorey55
    {
        internal LuaFunction func;

        internal void <>m__25()
        {
            this.func.Call();
        }
    }

    [CompilerGenerated]
    private sealed class <RegisterDirtyVerticesCallback>c__AnonStorey53
    {
        internal LuaFunction func;

        internal void <>m__23()
        {
            this.func.Call();
        }
    }

    [CompilerGenerated]
    private sealed class <UnregisterDirtyLayoutCallback>c__AnonStorey52
    {
        internal LuaFunction func;

        internal void <>m__22()
        {
            this.func.Call();
        }
    }

    [CompilerGenerated]
    private sealed class <UnregisterDirtyMaterialCallback>c__AnonStorey56
    {
        internal LuaFunction func;

        internal void <>m__26()
        {
            this.func.Call();
        }
    }

    [CompilerGenerated]
    private sealed class <UnregisterDirtyVerticesCallback>c__AnonStorey54
    {
        internal LuaFunction func;

        internal void <>m__24()
        {
            this.func.Call();
        }
    }
}

