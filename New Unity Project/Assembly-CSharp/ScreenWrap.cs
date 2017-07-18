using com.tencent.pandora;
using System;
using UnityEngine;

public class ScreenWrap
{
    private static Type classType = typeof(Screen);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _CreateScreen(IntPtr L)
    {
        if (LuaDLL.lua_gettop(L) == 0)
        {
            Screen o = new Screen();
            LuaScriptMgr.PushObject(L, o);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Screen.New");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_autorotateToLandscapeLeft(IntPtr L)
    {
        LuaScriptMgr.Push(L, Screen.get_autorotateToLandscapeLeft());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_autorotateToLandscapeRight(IntPtr L)
    {
        LuaScriptMgr.Push(L, Screen.get_autorotateToLandscapeRight());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_autorotateToPortrait(IntPtr L)
    {
        LuaScriptMgr.Push(L, Screen.get_autorotateToPortrait());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_autorotateToPortraitUpsideDown(IntPtr L)
    {
        LuaScriptMgr.Push(L, Screen.get_autorotateToPortraitUpsideDown());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_currentResolution(IntPtr L)
    {
        LuaScriptMgr.PushValue(L, Screen.get_currentResolution());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_dpi(IntPtr L)
    {
        LuaScriptMgr.Push(L, Screen.get_dpi());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_fullScreen(IntPtr L)
    {
        LuaScriptMgr.Push(L, Screen.get_fullScreen());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_GetResolution(IntPtr L)
    {
        LuaScriptMgr.PushArray(L, Screen.get_GetResolution());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_height(IntPtr L)
    {
        LuaScriptMgr.Push(L, Screen.get_height());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_lockCursor(IntPtr L)
    {
        LuaScriptMgr.Push(L, Screen.get_lockCursor());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_orientation(IntPtr L)
    {
        LuaScriptMgr.Push(L, (Enum) Screen.get_orientation());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_resolutions(IntPtr L)
    {
        LuaScriptMgr.PushArray(L, Screen.get_resolutions());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_showCursor(IntPtr L)
    {
        LuaScriptMgr.Push(L, Screen.get_showCursor());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_sleepTimeout(IntPtr L)
    {
        LuaScriptMgr.Push(L, Screen.get_sleepTimeout());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_width(IntPtr L)
    {
        LuaScriptMgr.Push(L, Screen.get_width());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetClassType(IntPtr L)
    {
        LuaScriptMgr.Push(L, classType);
        return 1;
    }

    public static void Register(IntPtr L)
    {
        LuaMethod[] regs = new LuaMethod[] { new LuaMethod("SetResolution", new LuaCSFunction(ScreenWrap.SetResolution)), new LuaMethod("New", new LuaCSFunction(ScreenWrap._CreateScreen)), new LuaMethod("GetClassType", new LuaCSFunction(ScreenWrap.GetClassType)) };
        LuaField[] fields = new LuaField[] { new LuaField("resolutions", new LuaCSFunction(ScreenWrap.get_resolutions), null), new LuaField("GetResolution", new LuaCSFunction(ScreenWrap.get_GetResolution), null), new LuaField("currentResolution", new LuaCSFunction(ScreenWrap.get_currentResolution), null), new LuaField("showCursor", new LuaCSFunction(ScreenWrap.get_showCursor), new LuaCSFunction(ScreenWrap.set_showCursor)), new LuaField("lockCursor", new LuaCSFunction(ScreenWrap.get_lockCursor), new LuaCSFunction(ScreenWrap.set_lockCursor)), new LuaField("width", new LuaCSFunction(ScreenWrap.get_width), null), new LuaField("height", new LuaCSFunction(ScreenWrap.get_height), null), new LuaField("dpi", new LuaCSFunction(ScreenWrap.get_dpi), null), new LuaField("fullScreen", new LuaCSFunction(ScreenWrap.get_fullScreen), new LuaCSFunction(ScreenWrap.set_fullScreen)), new LuaField("autorotateToPortrait", new LuaCSFunction(ScreenWrap.get_autorotateToPortrait), new LuaCSFunction(ScreenWrap.set_autorotateToPortrait)), new LuaField("autorotateToPortraitUpsideDown", new LuaCSFunction(ScreenWrap.get_autorotateToPortraitUpsideDown), new LuaCSFunction(ScreenWrap.set_autorotateToPortraitUpsideDown)), new LuaField("autorotateToLandscapeLeft", new LuaCSFunction(ScreenWrap.get_autorotateToLandscapeLeft), new LuaCSFunction(ScreenWrap.set_autorotateToLandscapeLeft)), new LuaField("autorotateToLandscapeRight", new LuaCSFunction(ScreenWrap.get_autorotateToLandscapeRight), new LuaCSFunction(ScreenWrap.set_autorotateToLandscapeRight)), new LuaField("orientation", new LuaCSFunction(ScreenWrap.get_orientation), new LuaCSFunction(ScreenWrap.set_orientation)), new LuaField("sleepTimeout", new LuaCSFunction(ScreenWrap.get_sleepTimeout), new LuaCSFunction(ScreenWrap.set_sleepTimeout)) };
        LuaScriptMgr.RegisterLib(L, "UnityEngine.Screen", typeof(Screen), regs, fields, typeof(object));
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_autorotateToLandscapeLeft(IntPtr L)
    {
        Screen.set_autorotateToLandscapeLeft(LuaScriptMgr.GetBoolean(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_autorotateToLandscapeRight(IntPtr L)
    {
        Screen.set_autorotateToLandscapeRight(LuaScriptMgr.GetBoolean(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_autorotateToPortrait(IntPtr L)
    {
        Screen.set_autorotateToPortrait(LuaScriptMgr.GetBoolean(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_autorotateToPortraitUpsideDown(IntPtr L)
    {
        Screen.set_autorotateToPortraitUpsideDown(LuaScriptMgr.GetBoolean(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_fullScreen(IntPtr L)
    {
        Screen.set_fullScreen(LuaScriptMgr.GetBoolean(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_lockCursor(IntPtr L)
    {
        Screen.set_lockCursor(LuaScriptMgr.GetBoolean(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_orientation(IntPtr L)
    {
        Screen.set_orientation((ScreenOrientation) ((int) LuaScriptMgr.GetNetObject(L, 3, typeof(ScreenOrientation))));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_showCursor(IntPtr L)
    {
        Screen.set_showCursor(LuaScriptMgr.GetBoolean(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_sleepTimeout(IntPtr L)
    {
        Screen.set_sleepTimeout((int) LuaScriptMgr.GetNumber(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int SetResolution(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 3:
            {
                int number = (int) LuaScriptMgr.GetNumber(L, 1);
                int num3 = (int) LuaScriptMgr.GetNumber(L, 2);
                bool boolean = LuaScriptMgr.GetBoolean(L, 3);
                Screen.SetResolution(number, num3, boolean);
                return 0;
            }
            case 4:
            {
                int num4 = (int) LuaScriptMgr.GetNumber(L, 1);
                int num5 = (int) LuaScriptMgr.GetNumber(L, 2);
                bool flag2 = LuaScriptMgr.GetBoolean(L, 3);
                int num6 = (int) LuaScriptMgr.GetNumber(L, 4);
                Screen.SetResolution(num4, num5, flag2, num6);
                return 0;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Screen.SetResolution");
        return 0;
    }
}

