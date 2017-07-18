using com.tencent.pandora;
using System;
using UnityEngine.Events;
using UnityEngine.UI;

public class UnityEngine_UI_Button_ButtonClickedEventWrap
{
    private static Type classType = typeof(Button.ButtonClickedEvent);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _CreateUnityEngine_UI_Button_ButtonClickedEvent(IntPtr L)
    {
        if (LuaDLL.lua_gettop(L) == 0)
        {
            Button.ButtonClickedEvent o = new Button.ButtonClickedEvent();
            LuaScriptMgr.PushObject(L, o);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: UnityEngine.UI.Button.ButtonClickedEvent.New");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetClassType(IntPtr L)
    {
        LuaScriptMgr.Push(L, classType);
        return 1;
    }

    public static void Register(IntPtr L)
    {
        LuaMethod[] regs = new LuaMethod[] { new LuaMethod("New", new LuaCSFunction(UnityEngine_UI_Button_ButtonClickedEventWrap._CreateUnityEngine_UI_Button_ButtonClickedEvent)), new LuaMethod("GetClassType", new LuaCSFunction(UnityEngine_UI_Button_ButtonClickedEventWrap.GetClassType)) };
        LuaField[] fields = new LuaField[0];
        LuaScriptMgr.RegisterLib(L, "UnityEngine.UI.Button.ButtonClickedEvent", typeof(Button.ButtonClickedEvent), regs, fields, typeof(UnityEvent));
    }
}

