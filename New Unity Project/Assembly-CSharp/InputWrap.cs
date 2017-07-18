using com.tencent.pandora;
using System;
using UnityEngine;

public class InputWrap
{
    private static Type classType = typeof(Input);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _CreateInput(IntPtr L)
    {
        if (LuaDLL.lua_gettop(L) == 0)
        {
            Input o = new Input();
            LuaScriptMgr.PushObject(L, o);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Input.New");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_acceleration(IntPtr L)
    {
        LuaScriptMgr.Push(L, Input.get_acceleration());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_accelerationEventCount(IntPtr L)
    {
        LuaScriptMgr.Push(L, Input.get_accelerationEventCount());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_accelerationEvents(IntPtr L)
    {
        LuaScriptMgr.PushArray(L, Input.get_accelerationEvents());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_anyKey(IntPtr L)
    {
        LuaScriptMgr.Push(L, Input.get_anyKey());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_anyKeyDown(IntPtr L)
    {
        LuaScriptMgr.Push(L, Input.get_anyKeyDown());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_compass(IntPtr L)
    {
        LuaScriptMgr.PushObject(L, Input.get_compass());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_compensateSensors(IntPtr L)
    {
        LuaScriptMgr.Push(L, Input.get_compensateSensors());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_compositionCursorPos(IntPtr L)
    {
        LuaScriptMgr.Push(L, Input.get_compositionCursorPos());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_compositionString(IntPtr L)
    {
        LuaScriptMgr.Push(L, Input.get_compositionString());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_deviceOrientation(IntPtr L)
    {
        LuaScriptMgr.Push(L, (Enum) Input.get_deviceOrientation());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_gyro(IntPtr L)
    {
        LuaScriptMgr.PushObject(L, Input.get_gyro());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_imeCompositionMode(IntPtr L)
    {
        LuaScriptMgr.Push(L, (Enum) Input.get_imeCompositionMode());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_imeIsSelected(IntPtr L)
    {
        LuaScriptMgr.Push(L, Input.get_imeIsSelected());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_inputString(IntPtr L)
    {
        LuaScriptMgr.Push(L, Input.get_inputString());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_location(IntPtr L)
    {
        LuaScriptMgr.PushObject(L, Input.get_location());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_mousePosition(IntPtr L)
    {
        LuaScriptMgr.Push(L, Input.get_mousePosition());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_mousePresent(IntPtr L)
    {
        LuaScriptMgr.Push(L, Input.get_mousePresent());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_mouseScrollDelta(IntPtr L)
    {
        LuaScriptMgr.Push(L, Input.get_mouseScrollDelta());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_multiTouchEnabled(IntPtr L)
    {
        LuaScriptMgr.Push(L, Input.get_multiTouchEnabled());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_simulateMouseWithTouches(IntPtr L)
    {
        LuaScriptMgr.Push(L, Input.get_simulateMouseWithTouches());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_touchCount(IntPtr L)
    {
        LuaScriptMgr.Push(L, Input.get_touchCount());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_touches(IntPtr L)
    {
        LuaScriptMgr.PushArray(L, Input.get_touches());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_touchSupported(IntPtr L)
    {
        LuaScriptMgr.Push(L, Input.get_touchSupported());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetAccelerationEvent(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        int number = (int) LuaScriptMgr.GetNumber(L, 1);
        AccelerationEvent accelerationEvent = Input.GetAccelerationEvent(number);
        LuaScriptMgr.PushValue(L, accelerationEvent);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetAxis(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        float axis = Input.GetAxis(LuaScriptMgr.GetLuaString(L, 1));
        LuaScriptMgr.Push(L, axis);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetAxisRaw(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        float axisRaw = Input.GetAxisRaw(LuaScriptMgr.GetLuaString(L, 1));
        LuaScriptMgr.Push(L, axisRaw);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetButton(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        bool button = Input.GetButton(LuaScriptMgr.GetLuaString(L, 1));
        LuaScriptMgr.Push(L, button);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetButtonDown(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        bool buttonDown = Input.GetButtonDown(LuaScriptMgr.GetLuaString(L, 1));
        LuaScriptMgr.Push(L, buttonDown);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetButtonUp(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        bool buttonUp = Input.GetButtonUp(LuaScriptMgr.GetLuaString(L, 1));
        LuaScriptMgr.Push(L, buttonUp);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetClassType(IntPtr L)
    {
        LuaScriptMgr.Push(L, classType);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetJoystickNames(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 0);
        string[] joystickNames = Input.GetJoystickNames();
        LuaScriptMgr.PushArray(L, joystickNames);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetKey(IntPtr L)
    {
        int num = LuaDLL.lua_gettop(L);
        if ((num == 1) && LuaScriptMgr.CheckTypes(L, 1, typeof(KeyCode)))
        {
            KeyCode luaObject = (KeyCode) ((int) LuaScriptMgr.GetLuaObject(L, 1));
            bool key = Input.GetKey(luaObject);
            LuaScriptMgr.Push(L, key);
            return 1;
        }
        if ((num == 1) && LuaScriptMgr.CheckTypes(L, 1, typeof(string)))
        {
            bool b = Input.GetKey(LuaScriptMgr.GetString(L, 1));
            LuaScriptMgr.Push(L, b);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Input.GetKey");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetKeyDown(IntPtr L)
    {
        int num = LuaDLL.lua_gettop(L);
        if ((num == 1) && LuaScriptMgr.CheckTypes(L, 1, typeof(KeyCode)))
        {
            KeyCode luaObject = (KeyCode) ((int) LuaScriptMgr.GetLuaObject(L, 1));
            bool keyDown = Input.GetKeyDown(luaObject);
            LuaScriptMgr.Push(L, keyDown);
            return 1;
        }
        if ((num == 1) && LuaScriptMgr.CheckTypes(L, 1, typeof(string)))
        {
            bool b = Input.GetKeyDown(LuaScriptMgr.GetString(L, 1));
            LuaScriptMgr.Push(L, b);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Input.GetKeyDown");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetKeyUp(IntPtr L)
    {
        int num = LuaDLL.lua_gettop(L);
        if ((num == 1) && LuaScriptMgr.CheckTypes(L, 1, typeof(KeyCode)))
        {
            KeyCode luaObject = (KeyCode) ((int) LuaScriptMgr.GetLuaObject(L, 1));
            bool keyUp = Input.GetKeyUp(luaObject);
            LuaScriptMgr.Push(L, keyUp);
            return 1;
        }
        if ((num == 1) && LuaScriptMgr.CheckTypes(L, 1, typeof(string)))
        {
            bool b = Input.GetKeyUp(LuaScriptMgr.GetString(L, 1));
            LuaScriptMgr.Push(L, b);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Input.GetKeyUp");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetMouseButton(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        int number = (int) LuaScriptMgr.GetNumber(L, 1);
        bool mouseButton = Input.GetMouseButton(number);
        LuaScriptMgr.Push(L, mouseButton);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetMouseButtonDown(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        int number = (int) LuaScriptMgr.GetNumber(L, 1);
        bool mouseButtonDown = Input.GetMouseButtonDown(number);
        LuaScriptMgr.Push(L, mouseButtonDown);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetMouseButtonUp(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        int number = (int) LuaScriptMgr.GetNumber(L, 1);
        bool mouseButtonUp = Input.GetMouseButtonUp(number);
        LuaScriptMgr.Push(L, mouseButtonUp);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetTouch(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        int number = (int) LuaScriptMgr.GetNumber(L, 1);
        Touch touch = Input.GetTouch(number);
        LuaScriptMgr.Push(L, touch);
        return 1;
    }

    public static void Register(IntPtr L)
    {
        LuaMethod[] regs = new LuaMethod[] { 
            new LuaMethod("GetAxis", new LuaCSFunction(InputWrap.GetAxis)), new LuaMethod("GetAxisRaw", new LuaCSFunction(InputWrap.GetAxisRaw)), new LuaMethod("GetButton", new LuaCSFunction(InputWrap.GetButton)), new LuaMethod("GetButtonDown", new LuaCSFunction(InputWrap.GetButtonDown)), new LuaMethod("GetButtonUp", new LuaCSFunction(InputWrap.GetButtonUp)), new LuaMethod("GetKey", new LuaCSFunction(InputWrap.GetKey)), new LuaMethod("GetKeyDown", new LuaCSFunction(InputWrap.GetKeyDown)), new LuaMethod("GetKeyUp", new LuaCSFunction(InputWrap.GetKeyUp)), new LuaMethod("GetJoystickNames", new LuaCSFunction(InputWrap.GetJoystickNames)), new LuaMethod("GetMouseButton", new LuaCSFunction(InputWrap.GetMouseButton)), new LuaMethod("GetMouseButtonDown", new LuaCSFunction(InputWrap.GetMouseButtonDown)), new LuaMethod("GetMouseButtonUp", new LuaCSFunction(InputWrap.GetMouseButtonUp)), new LuaMethod("ResetInputAxes", new LuaCSFunction(InputWrap.ResetInputAxes)), new LuaMethod("GetAccelerationEvent", new LuaCSFunction(InputWrap.GetAccelerationEvent)), new LuaMethod("GetTouch", new LuaCSFunction(InputWrap.GetTouch)), new LuaMethod("New", new LuaCSFunction(InputWrap._CreateInput)), 
            new LuaMethod("GetClassType", new LuaCSFunction(InputWrap.GetClassType))
         };
        LuaField[] fields = new LuaField[] { 
            new LuaField("compensateSensors", new LuaCSFunction(InputWrap.get_compensateSensors), new LuaCSFunction(InputWrap.set_compensateSensors)), new LuaField("gyro", new LuaCSFunction(InputWrap.get_gyro), null), new LuaField("mousePosition", new LuaCSFunction(InputWrap.get_mousePosition), null), new LuaField("mouseScrollDelta", new LuaCSFunction(InputWrap.get_mouseScrollDelta), null), new LuaField("mousePresent", new LuaCSFunction(InputWrap.get_mousePresent), null), new LuaField("simulateMouseWithTouches", new LuaCSFunction(InputWrap.get_simulateMouseWithTouches), new LuaCSFunction(InputWrap.set_simulateMouseWithTouches)), new LuaField("anyKey", new LuaCSFunction(InputWrap.get_anyKey), null), new LuaField("anyKeyDown", new LuaCSFunction(InputWrap.get_anyKeyDown), null), new LuaField("inputString", new LuaCSFunction(InputWrap.get_inputString), null), new LuaField("acceleration", new LuaCSFunction(InputWrap.get_acceleration), null), new LuaField("accelerationEvents", new LuaCSFunction(InputWrap.get_accelerationEvents), null), new LuaField("accelerationEventCount", new LuaCSFunction(InputWrap.get_accelerationEventCount), null), new LuaField("touches", new LuaCSFunction(InputWrap.get_touches), null), new LuaField("touchCount", new LuaCSFunction(InputWrap.get_touchCount), null), new LuaField("touchSupported", new LuaCSFunction(InputWrap.get_touchSupported), null), new LuaField("multiTouchEnabled", new LuaCSFunction(InputWrap.get_multiTouchEnabled), new LuaCSFunction(InputWrap.set_multiTouchEnabled)), 
            new LuaField("location", new LuaCSFunction(InputWrap.get_location), null), new LuaField("compass", new LuaCSFunction(InputWrap.get_compass), null), new LuaField("deviceOrientation", new LuaCSFunction(InputWrap.get_deviceOrientation), null), new LuaField("imeCompositionMode", new LuaCSFunction(InputWrap.get_imeCompositionMode), new LuaCSFunction(InputWrap.set_imeCompositionMode)), new LuaField("compositionString", new LuaCSFunction(InputWrap.get_compositionString), null), new LuaField("imeIsSelected", new LuaCSFunction(InputWrap.get_imeIsSelected), null), new LuaField("compositionCursorPos", new LuaCSFunction(InputWrap.get_compositionCursorPos), new LuaCSFunction(InputWrap.set_compositionCursorPos))
         };
        LuaScriptMgr.RegisterLib(L, "UnityEngine.Input", typeof(Input), regs, fields, typeof(object));
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int ResetInputAxes(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 0);
        Input.ResetInputAxes();
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_compensateSensors(IntPtr L)
    {
        Input.set_compensateSensors(LuaScriptMgr.GetBoolean(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_compositionCursorPos(IntPtr L)
    {
        Input.set_compositionCursorPos(LuaScriptMgr.GetVector2(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_imeCompositionMode(IntPtr L)
    {
        Input.set_imeCompositionMode((IMECompositionMode) ((int) LuaScriptMgr.GetNetObject(L, 3, typeof(IMECompositionMode))));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_multiTouchEnabled(IntPtr L)
    {
        Input.set_multiTouchEnabled(LuaScriptMgr.GetBoolean(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_simulateMouseWithTouches(IntPtr L)
    {
        Input.set_simulateMouseWithTouches(LuaScriptMgr.GetBoolean(L, 3));
        return 0;
    }
}

