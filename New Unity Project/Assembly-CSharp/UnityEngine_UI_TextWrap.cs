using com.tencent.pandora;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UnityEngine_UI_TextWrap
{
    private static Type classType = typeof(Text);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _CreateUnityEngine_UI_Text(IntPtr L)
    {
        LuaDLL.luaL_error(L, "UnityEngine.UI.Text class does not have a constructor function");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int CalculateLayoutInputHorizontal(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        ((Text) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Text")).CalculateLayoutInputHorizontal();
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int CalculateLayoutInputVertical(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        ((Text) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Text")).CalculateLayoutInputVertical();
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int FontTextureChanged(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        ((Text) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Text")).FontTextureChanged();
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_alignment(IntPtr L)
    {
        Text luaObject = (Text) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name alignment");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index alignment on a nil value");
            }
        }
        LuaScriptMgr.Push(L, (Enum) luaObject.get_alignment());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_cachedTextGenerator(IntPtr L)
    {
        Text luaObject = (Text) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name cachedTextGenerator");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index cachedTextGenerator on a nil value");
            }
        }
        LuaScriptMgr.PushObject(L, luaObject.get_cachedTextGenerator());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_cachedTextGeneratorForLayout(IntPtr L)
    {
        Text luaObject = (Text) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name cachedTextGeneratorForLayout");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index cachedTextGeneratorForLayout on a nil value");
            }
        }
        LuaScriptMgr.PushObject(L, luaObject.get_cachedTextGeneratorForLayout());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_defaultMaterial(IntPtr L)
    {
        Text luaObject = (Text) LuaScriptMgr.GetLuaObject(L, 1);
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
    private static int get_flexibleHeight(IntPtr L)
    {
        Text luaObject = (Text) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name flexibleHeight");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index flexibleHeight on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_flexibleHeight());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_flexibleWidth(IntPtr L)
    {
        Text luaObject = (Text) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name flexibleWidth");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index flexibleWidth on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_flexibleWidth());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_font(IntPtr L)
    {
        Text luaObject = (Text) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name font");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index font on a nil value");
            }
        }
        LuaScriptMgr.Push(L, (Object) luaObject.get_font());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_fontSize(IntPtr L)
    {
        Text luaObject = (Text) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name fontSize");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index fontSize on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_fontSize());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_fontStyle(IntPtr L)
    {
        Text luaObject = (Text) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name fontStyle");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index fontStyle on a nil value");
            }
        }
        LuaScriptMgr.Push(L, (Enum) luaObject.get_fontStyle());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_horizontalOverflow(IntPtr L)
    {
        Text luaObject = (Text) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name horizontalOverflow");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index horizontalOverflow on a nil value");
            }
        }
        LuaScriptMgr.Push(L, (Enum) luaObject.get_horizontalOverflow());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_layoutPriority(IntPtr L)
    {
        Text luaObject = (Text) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name layoutPriority");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index layoutPriority on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_layoutPriority());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_lineSpacing(IntPtr L)
    {
        Text luaObject = (Text) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name lineSpacing");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index lineSpacing on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_lineSpacing());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_mainTexture(IntPtr L)
    {
        Text luaObject = (Text) LuaScriptMgr.GetLuaObject(L, 1);
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
    private static int get_minHeight(IntPtr L)
    {
        Text luaObject = (Text) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name minHeight");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index minHeight on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_minHeight());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_minWidth(IntPtr L)
    {
        Text luaObject = (Text) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name minWidth");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index minWidth on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_minWidth());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_pixelsPerUnit(IntPtr L)
    {
        Text luaObject = (Text) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name pixelsPerUnit");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index pixelsPerUnit on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_pixelsPerUnit());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_preferredHeight(IntPtr L)
    {
        Text luaObject = (Text) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name preferredHeight");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index preferredHeight on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_preferredHeight());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_preferredWidth(IntPtr L)
    {
        Text luaObject = (Text) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name preferredWidth");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index preferredWidth on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_preferredWidth());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_resizeTextForBestFit(IntPtr L)
    {
        Text luaObject = (Text) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name resizeTextForBestFit");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index resizeTextForBestFit on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_resizeTextForBestFit());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_resizeTextMaxSize(IntPtr L)
    {
        Text luaObject = (Text) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name resizeTextMaxSize");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index resizeTextMaxSize on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_resizeTextMaxSize());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_resizeTextMinSize(IntPtr L)
    {
        Text luaObject = (Text) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name resizeTextMinSize");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index resizeTextMinSize on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_resizeTextMinSize());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_supportRichText(IntPtr L)
    {
        Text luaObject = (Text) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name supportRichText");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index supportRichText on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_supportRichText());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_text(IntPtr L)
    {
        Text luaObject = (Text) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name text");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index text on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_text());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_verticalOverflow(IntPtr L)
    {
        Text luaObject = (Text) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name verticalOverflow");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index verticalOverflow on a nil value");
            }
        }
        LuaScriptMgr.Push(L, (Enum) luaObject.get_verticalOverflow());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetClassType(IntPtr L)
    {
        LuaScriptMgr.Push(L, classType);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetGenerationSettings(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Text text = (Text) LuaScriptMgr.GetUnityObjectSelf(L, 1, "UnityEngine.UI.Text");
        Vector2 vector = LuaScriptMgr.GetVector2(L, 2);
        TextGenerationSettings generationSettings = text.GetGenerationSettings(vector);
        LuaScriptMgr.PushValue(L, generationSettings);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetTextAnchorPivot(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        TextAnchor anchor = (TextAnchor) ((int) LuaScriptMgr.GetNetObject(L, 1, typeof(TextAnchor)));
        Vector2 textAnchorPivot = Text.GetTextAnchorPivot(anchor);
        LuaScriptMgr.Push(L, textAnchorPivot);
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
        LuaMethod[] regs = new LuaMethod[] { new LuaMethod("FontTextureChanged", new LuaCSFunction(UnityEngine_UI_TextWrap.FontTextureChanged)), new LuaMethod("GetGenerationSettings", new LuaCSFunction(UnityEngine_UI_TextWrap.GetGenerationSettings)), new LuaMethod("GetTextAnchorPivot", new LuaCSFunction(UnityEngine_UI_TextWrap.GetTextAnchorPivot)), new LuaMethod("CalculateLayoutInputHorizontal", new LuaCSFunction(UnityEngine_UI_TextWrap.CalculateLayoutInputHorizontal)), new LuaMethod("CalculateLayoutInputVertical", new LuaCSFunction(UnityEngine_UI_TextWrap.CalculateLayoutInputVertical)), new LuaMethod("New", new LuaCSFunction(UnityEngine_UI_TextWrap._CreateUnityEngine_UI_Text)), new LuaMethod("GetClassType", new LuaCSFunction(UnityEngine_UI_TextWrap.GetClassType)), new LuaMethod("__eq", new LuaCSFunction(UnityEngine_UI_TextWrap.Lua_Eq)) };
        LuaField[] fields = new LuaField[] { 
            new LuaField("cachedTextGenerator", new LuaCSFunction(UnityEngine_UI_TextWrap.get_cachedTextGenerator), null), new LuaField("cachedTextGeneratorForLayout", new LuaCSFunction(UnityEngine_UI_TextWrap.get_cachedTextGeneratorForLayout), null), new LuaField("defaultMaterial", new LuaCSFunction(UnityEngine_UI_TextWrap.get_defaultMaterial), null), new LuaField("mainTexture", new LuaCSFunction(UnityEngine_UI_TextWrap.get_mainTexture), null), new LuaField("font", new LuaCSFunction(UnityEngine_UI_TextWrap.get_font), new LuaCSFunction(UnityEngine_UI_TextWrap.set_font)), new LuaField("text", new LuaCSFunction(UnityEngine_UI_TextWrap.get_text), new LuaCSFunction(UnityEngine_UI_TextWrap.set_text)), new LuaField("supportRichText", new LuaCSFunction(UnityEngine_UI_TextWrap.get_supportRichText), new LuaCSFunction(UnityEngine_UI_TextWrap.set_supportRichText)), new LuaField("resizeTextForBestFit", new LuaCSFunction(UnityEngine_UI_TextWrap.get_resizeTextForBestFit), new LuaCSFunction(UnityEngine_UI_TextWrap.set_resizeTextForBestFit)), new LuaField("resizeTextMinSize", new LuaCSFunction(UnityEngine_UI_TextWrap.get_resizeTextMinSize), new LuaCSFunction(UnityEngine_UI_TextWrap.set_resizeTextMinSize)), new LuaField("resizeTextMaxSize", new LuaCSFunction(UnityEngine_UI_TextWrap.get_resizeTextMaxSize), new LuaCSFunction(UnityEngine_UI_TextWrap.set_resizeTextMaxSize)), new LuaField("alignment", new LuaCSFunction(UnityEngine_UI_TextWrap.get_alignment), new LuaCSFunction(UnityEngine_UI_TextWrap.set_alignment)), new LuaField("fontSize", new LuaCSFunction(UnityEngine_UI_TextWrap.get_fontSize), new LuaCSFunction(UnityEngine_UI_TextWrap.set_fontSize)), new LuaField("horizontalOverflow", new LuaCSFunction(UnityEngine_UI_TextWrap.get_horizontalOverflow), new LuaCSFunction(UnityEngine_UI_TextWrap.set_horizontalOverflow)), new LuaField("verticalOverflow", new LuaCSFunction(UnityEngine_UI_TextWrap.get_verticalOverflow), new LuaCSFunction(UnityEngine_UI_TextWrap.set_verticalOverflow)), new LuaField("lineSpacing", new LuaCSFunction(UnityEngine_UI_TextWrap.get_lineSpacing), new LuaCSFunction(UnityEngine_UI_TextWrap.set_lineSpacing)), new LuaField("fontStyle", new LuaCSFunction(UnityEngine_UI_TextWrap.get_fontStyle), new LuaCSFunction(UnityEngine_UI_TextWrap.set_fontStyle)), 
            new LuaField("pixelsPerUnit", new LuaCSFunction(UnityEngine_UI_TextWrap.get_pixelsPerUnit), null), new LuaField("minWidth", new LuaCSFunction(UnityEngine_UI_TextWrap.get_minWidth), null), new LuaField("preferredWidth", new LuaCSFunction(UnityEngine_UI_TextWrap.get_preferredWidth), null), new LuaField("flexibleWidth", new LuaCSFunction(UnityEngine_UI_TextWrap.get_flexibleWidth), null), new LuaField("minHeight", new LuaCSFunction(UnityEngine_UI_TextWrap.get_minHeight), null), new LuaField("preferredHeight", new LuaCSFunction(UnityEngine_UI_TextWrap.get_preferredHeight), null), new LuaField("flexibleHeight", new LuaCSFunction(UnityEngine_UI_TextWrap.get_flexibleHeight), null), new LuaField("layoutPriority", new LuaCSFunction(UnityEngine_UI_TextWrap.get_layoutPriority), null)
         };
        LuaScriptMgr.RegisterLib(L, "UnityEngine.UI.Text", typeof(Text), regs, fields, typeof(MaskableGraphic));
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_alignment(IntPtr L)
    {
        Text luaObject = (Text) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name alignment");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index alignment on a nil value");
            }
        }
        luaObject.set_alignment((TextAnchor) ((int) LuaScriptMgr.GetNetObject(L, 3, typeof(TextAnchor))));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_font(IntPtr L)
    {
        Text luaObject = (Text) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name font");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index font on a nil value");
            }
        }
        luaObject.set_font((Font) LuaScriptMgr.GetUnityObject(L, 3, typeof(Font)));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_fontSize(IntPtr L)
    {
        Text luaObject = (Text) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name fontSize");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index fontSize on a nil value");
            }
        }
        luaObject.set_fontSize((int) LuaScriptMgr.GetNumber(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_fontStyle(IntPtr L)
    {
        Text luaObject = (Text) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name fontStyle");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index fontStyle on a nil value");
            }
        }
        luaObject.set_fontStyle((FontStyle) ((int) LuaScriptMgr.GetNetObject(L, 3, typeof(FontStyle))));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_horizontalOverflow(IntPtr L)
    {
        Text luaObject = (Text) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name horizontalOverflow");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index horizontalOverflow on a nil value");
            }
        }
        luaObject.set_horizontalOverflow((HorizontalWrapMode) ((int) LuaScriptMgr.GetNetObject(L, 3, typeof(HorizontalWrapMode))));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_lineSpacing(IntPtr L)
    {
        Text luaObject = (Text) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name lineSpacing");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index lineSpacing on a nil value");
            }
        }
        luaObject.set_lineSpacing((float) LuaScriptMgr.GetNumber(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_resizeTextForBestFit(IntPtr L)
    {
        Text luaObject = (Text) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name resizeTextForBestFit");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index resizeTextForBestFit on a nil value");
            }
        }
        luaObject.set_resizeTextForBestFit(LuaScriptMgr.GetBoolean(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_resizeTextMaxSize(IntPtr L)
    {
        Text luaObject = (Text) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name resizeTextMaxSize");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index resizeTextMaxSize on a nil value");
            }
        }
        luaObject.set_resizeTextMaxSize((int) LuaScriptMgr.GetNumber(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_resizeTextMinSize(IntPtr L)
    {
        Text luaObject = (Text) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name resizeTextMinSize");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index resizeTextMinSize on a nil value");
            }
        }
        luaObject.set_resizeTextMinSize((int) LuaScriptMgr.GetNumber(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_supportRichText(IntPtr L)
    {
        Text luaObject = (Text) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name supportRichText");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index supportRichText on a nil value");
            }
        }
        luaObject.set_supportRichText(LuaScriptMgr.GetBoolean(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_text(IntPtr L)
    {
        Text luaObject = (Text) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name text");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index text on a nil value");
            }
        }
        luaObject.set_text(LuaScriptMgr.GetString(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_verticalOverflow(IntPtr L)
    {
        Text luaObject = (Text) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name verticalOverflow");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index verticalOverflow on a nil value");
            }
        }
        luaObject.set_verticalOverflow((VerticalWrapMode) ((int) LuaScriptMgr.GetNetObject(L, 3, typeof(VerticalWrapMode))));
        return 0;
    }
}

