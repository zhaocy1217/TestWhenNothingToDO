using com.tencent.pandora;
using System;
using System.Collections;
using UnityEngine;

public class AnimationWrap
{
    private static Type classType = typeof(Animation);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _CreateAnimation(IntPtr L)
    {
        if (LuaDLL.lua_gettop(L) == 0)
        {
            Animation animation = new Animation();
            LuaScriptMgr.Push(L, (Object) animation);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Animation.New");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int AddClip(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 3:
            {
                Animation animation = (Animation) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Animation");
                AnimationClip clip = LuaScriptMgr.GetUnityObject(L, 2, typeof(AnimationClip));
                string luaString = LuaScriptMgr.GetLuaString(L, 3);
                animation.AddClip(clip, luaString);
                return 0;
            }
            case 5:
            {
                Animation animation2 = (Animation) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Animation");
                AnimationClip clip2 = LuaScriptMgr.GetUnityObject(L, 2, typeof(AnimationClip));
                string str2 = LuaScriptMgr.GetLuaString(L, 3);
                int number = (int) LuaScriptMgr.GetNumber(L, 4);
                int num3 = (int) LuaScriptMgr.GetNumber(L, 5);
                animation2.AddClip(clip2, str2, number, num3);
                return 0;
            }
            case 6:
            {
                Animation animation3 = (Animation) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Animation");
                AnimationClip clip3 = LuaScriptMgr.GetUnityObject(L, 2, typeof(AnimationClip));
                string str3 = LuaScriptMgr.GetLuaString(L, 3);
                int num4 = (int) LuaScriptMgr.GetNumber(L, 4);
                int num5 = (int) LuaScriptMgr.GetNumber(L, 5);
                bool boolean = LuaScriptMgr.GetBoolean(L, 6);
                animation3.AddClip(clip3, str3, num4, num5, boolean);
                return 0;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Animation.AddClip");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Blend(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 2:
            {
                Animation animation = (Animation) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Animation");
                string luaString = LuaScriptMgr.GetLuaString(L, 2);
                animation.Blend(luaString);
                return 0;
            }
            case 3:
            {
                Animation animation2 = (Animation) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Animation");
                string str2 = LuaScriptMgr.GetLuaString(L, 2);
                float number = (float) LuaScriptMgr.GetNumber(L, 3);
                animation2.Blend(str2, number);
                return 0;
            }
            case 4:
            {
                Animation animation3 = (Animation) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Animation");
                string str3 = LuaScriptMgr.GetLuaString(L, 2);
                float num3 = (float) LuaScriptMgr.GetNumber(L, 3);
                float num4 = (float) LuaScriptMgr.GetNumber(L, 4);
                animation3.Blend(str3, num3, num4);
                return 0;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Animation.Blend");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int CrossFade(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 2:
            {
                Animation animation = (Animation) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Animation");
                string luaString = LuaScriptMgr.GetLuaString(L, 2);
                animation.CrossFade(luaString);
                return 0;
            }
            case 3:
            {
                Animation animation2 = (Animation) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Animation");
                string str2 = LuaScriptMgr.GetLuaString(L, 2);
                float number = (float) LuaScriptMgr.GetNumber(L, 3);
                animation2.CrossFade(str2, number);
                return 0;
            }
            case 4:
            {
                Animation animation3 = (Animation) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Animation");
                string str3 = LuaScriptMgr.GetLuaString(L, 2);
                float num3 = (float) LuaScriptMgr.GetNumber(L, 3);
                PlayMode mode = (PlayMode) ((int) LuaScriptMgr.GetNetObject(L, 4, typeof(PlayMode)));
                animation3.CrossFade(str3, num3, mode);
                return 0;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Animation.CrossFade");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int CrossFadeQueued(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 2:
            {
                Animation animation = (Animation) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Animation");
                string luaString = LuaScriptMgr.GetLuaString(L, 2);
                AnimationState state = animation.CrossFadeQueued(luaString);
                LuaScriptMgr.Push(L, (TrackedReference) state);
                return 1;
            }
            case 3:
            {
                Animation animation2 = (Animation) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Animation");
                string str2 = LuaScriptMgr.GetLuaString(L, 2);
                float number = (float) LuaScriptMgr.GetNumber(L, 3);
                AnimationState state2 = animation2.CrossFadeQueued(str2, number);
                LuaScriptMgr.Push(L, (TrackedReference) state2);
                return 1;
            }
            case 4:
            {
                Animation animation3 = (Animation) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Animation");
                string str3 = LuaScriptMgr.GetLuaString(L, 2);
                float num3 = (float) LuaScriptMgr.GetNumber(L, 3);
                QueueMode mode = (QueueMode) ((int) LuaScriptMgr.GetNetObject(L, 4, typeof(QueueMode)));
                AnimationState state3 = animation3.CrossFadeQueued(str3, num3, mode);
                LuaScriptMgr.Push(L, (TrackedReference) state3);
                return 1;
            }
            case 5:
            {
                Animation animation4 = (Animation) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Animation");
                string str4 = LuaScriptMgr.GetLuaString(L, 2);
                float num4 = (float) LuaScriptMgr.GetNumber(L, 3);
                QueueMode mode2 = (QueueMode) ((int) LuaScriptMgr.GetNetObject(L, 4, typeof(QueueMode)));
                PlayMode mode3 = (PlayMode) ((int) LuaScriptMgr.GetNetObject(L, 5, typeof(PlayMode)));
                AnimationState state4 = animation4.CrossFadeQueued(str4, num4, mode2, mode3);
                LuaScriptMgr.Push(L, (TrackedReference) state4);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Animation.CrossFadeQueued");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_animatePhysics(IntPtr L)
    {
        Animation luaObject = (Animation) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name animatePhysics");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index animatePhysics on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_animatePhysics());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_clip(IntPtr L)
    {
        Animation luaObject = (Animation) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name clip");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index clip on a nil value");
            }
        }
        LuaScriptMgr.Push(L, (Object) luaObject.get_clip());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_cullingType(IntPtr L)
    {
        Animation luaObject = (Animation) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name cullingType");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index cullingType on a nil value");
            }
        }
        LuaScriptMgr.Push(L, (Enum) luaObject.get_cullingType());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_isPlaying(IntPtr L)
    {
        Animation luaObject = (Animation) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name isPlaying");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index isPlaying on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_isPlaying());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_Item(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Animation animation = (Animation) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Animation");
        string luaString = LuaScriptMgr.GetLuaString(L, 2);
        AnimationState state = animation.get_Item(luaString);
        LuaScriptMgr.Push(L, (TrackedReference) state);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_localBounds(IntPtr L)
    {
        Animation luaObject = (Animation) LuaScriptMgr.GetLuaObject(L, 1);
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
    private static int get_playAutomatically(IntPtr L)
    {
        Animation luaObject = (Animation) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name playAutomatically");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index playAutomatically on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_playAutomatically());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_wrapMode(IntPtr L)
    {
        Animation luaObject = (Animation) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name wrapMode");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index wrapMode on a nil value");
            }
        }
        LuaScriptMgr.Push(L, (Enum) luaObject.get_wrapMode());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetClassType(IntPtr L)
    {
        LuaScriptMgr.Push(L, classType);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetClip(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Animation animation = (Animation) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Animation");
        string luaString = LuaScriptMgr.GetLuaString(L, 2);
        AnimationClip clip = animation.GetClip(luaString);
        LuaScriptMgr.Push(L, (Object) clip);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetClipCount(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        int clipCount = ((Animation) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Animation")).GetClipCount();
        LuaScriptMgr.Push(L, clipCount);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetEnumerator(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        IEnumerator o = ((Animation) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Animation")).GetEnumerator();
        LuaScriptMgr.Push(L, o);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int IsPlaying(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Animation animation = (Animation) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Animation");
        string luaString = LuaScriptMgr.GetLuaString(L, 2);
        bool b = animation.IsPlaying(luaString);
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
    private static int Play(IntPtr L)
    {
        int num = LuaDLL.lua_gettop(L);
        if (num == 1)
        {
            bool b = ((Animation) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Animation")).Play();
            LuaScriptMgr.Push(L, b);
            return 1;
        }
        if ((num == 2) && LuaScriptMgr.CheckTypes(L, 1, typeof(Animation), typeof(string)))
        {
            Animation animation2 = (Animation) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Animation");
            string str = LuaScriptMgr.GetString(L, 2);
            bool flag2 = animation2.Play(str);
            LuaScriptMgr.Push(L, flag2);
            return 1;
        }
        if ((num == 2) && LuaScriptMgr.CheckTypes(L, 1, typeof(Animation), typeof(PlayMode)))
        {
            Animation animation3 = (Animation) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Animation");
            PlayMode luaObject = (PlayMode) ((int) LuaScriptMgr.GetLuaObject(L, 2));
            bool flag3 = animation3.Play(luaObject);
            LuaScriptMgr.Push(L, flag3);
            return 1;
        }
        if (num == 3)
        {
            Animation animation4 = (Animation) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Animation");
            string luaString = LuaScriptMgr.GetLuaString(L, 2);
            PlayMode mode2 = (PlayMode) ((int) LuaScriptMgr.GetNetObject(L, 3, typeof(PlayMode)));
            bool flag4 = animation4.Play(luaString, mode2);
            LuaScriptMgr.Push(L, flag4);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Animation.Play");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int PlayQueued(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 2:
            {
                Animation animation = (Animation) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Animation");
                string luaString = LuaScriptMgr.GetLuaString(L, 2);
                AnimationState state = animation.PlayQueued(luaString);
                LuaScriptMgr.Push(L, (TrackedReference) state);
                return 1;
            }
            case 3:
            {
                Animation animation2 = (Animation) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Animation");
                string str2 = LuaScriptMgr.GetLuaString(L, 2);
                QueueMode mode = (QueueMode) ((int) LuaScriptMgr.GetNetObject(L, 3, typeof(QueueMode)));
                AnimationState state2 = animation2.PlayQueued(str2, mode);
                LuaScriptMgr.Push(L, (TrackedReference) state2);
                return 1;
            }
            case 4:
            {
                Animation animation3 = (Animation) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Animation");
                string str3 = LuaScriptMgr.GetLuaString(L, 2);
                QueueMode mode2 = (QueueMode) ((int) LuaScriptMgr.GetNetObject(L, 3, typeof(QueueMode)));
                PlayMode mode3 = (PlayMode) ((int) LuaScriptMgr.GetNetObject(L, 4, typeof(PlayMode)));
                AnimationState state3 = animation3.PlayQueued(str3, mode2, mode3);
                LuaScriptMgr.Push(L, (TrackedReference) state3);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Animation.PlayQueued");
        return 0;
    }

    public static void Register(IntPtr L)
    {
        LuaMethod[] regs = new LuaMethod[] { 
            new LuaMethod("Stop", new LuaCSFunction(AnimationWrap.Stop)), new LuaMethod("Rewind", new LuaCSFunction(AnimationWrap.Rewind)), new LuaMethod("Sample", new LuaCSFunction(AnimationWrap.Sample)), new LuaMethod("IsPlaying", new LuaCSFunction(AnimationWrap.IsPlaying)), new LuaMethod("get_Item", new LuaCSFunction(AnimationWrap.get_Item)), new LuaMethod("Play", new LuaCSFunction(AnimationWrap.Play)), new LuaMethod("CrossFade", new LuaCSFunction(AnimationWrap.CrossFade)), new LuaMethod("Blend", new LuaCSFunction(AnimationWrap.Blend)), new LuaMethod("CrossFadeQueued", new LuaCSFunction(AnimationWrap.CrossFadeQueued)), new LuaMethod("PlayQueued", new LuaCSFunction(AnimationWrap.PlayQueued)), new LuaMethod("AddClip", new LuaCSFunction(AnimationWrap.AddClip)), new LuaMethod("RemoveClip", new LuaCSFunction(AnimationWrap.RemoveClip)), new LuaMethod("GetClipCount", new LuaCSFunction(AnimationWrap.GetClipCount)), new LuaMethod("SyncLayer", new LuaCSFunction(AnimationWrap.SyncLayer)), new LuaMethod("GetEnumerator", new LuaCSFunction(AnimationWrap.GetEnumerator)), new LuaMethod("GetClip", new LuaCSFunction(AnimationWrap.GetClip)), 
            new LuaMethod("New", new LuaCSFunction(AnimationWrap._CreateAnimation)), new LuaMethod("GetClassType", new LuaCSFunction(AnimationWrap.GetClassType)), new LuaMethod("__eq", new LuaCSFunction(AnimationWrap.Lua_Eq))
         };
        LuaField[] fields = new LuaField[] { new LuaField("clip", new LuaCSFunction(AnimationWrap.get_clip), new LuaCSFunction(AnimationWrap.set_clip)), new LuaField("playAutomatically", new LuaCSFunction(AnimationWrap.get_playAutomatically), new LuaCSFunction(AnimationWrap.set_playAutomatically)), new LuaField("wrapMode", new LuaCSFunction(AnimationWrap.get_wrapMode), new LuaCSFunction(AnimationWrap.set_wrapMode)), new LuaField("isPlaying", new LuaCSFunction(AnimationWrap.get_isPlaying), null), new LuaField("animatePhysics", new LuaCSFunction(AnimationWrap.get_animatePhysics), new LuaCSFunction(AnimationWrap.set_animatePhysics)), new LuaField("cullingType", new LuaCSFunction(AnimationWrap.get_cullingType), new LuaCSFunction(AnimationWrap.set_cullingType)), new LuaField("localBounds", new LuaCSFunction(AnimationWrap.get_localBounds), new LuaCSFunction(AnimationWrap.set_localBounds)) };
        LuaScriptMgr.RegisterLib(L, "UnityEngine.Animation", typeof(Animation), regs, fields, typeof(Behaviour));
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int RemoveClip(IntPtr L)
    {
        int num = LuaDLL.lua_gettop(L);
        if ((num == 2) && LuaScriptMgr.CheckTypes(L, 1, typeof(Animation), typeof(string)))
        {
            Animation animation = (Animation) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Animation");
            string str = LuaScriptMgr.GetString(L, 2);
            animation.RemoveClip(str);
            return 0;
        }
        if ((num == 2) && LuaScriptMgr.CheckTypes(L, 1, typeof(Animation), typeof(AnimationClip)))
        {
            Animation animation2 = (Animation) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Animation");
            AnimationClip luaObject = (AnimationClip) LuaScriptMgr.GetLuaObject(L, 2);
            animation2.RemoveClip(luaObject);
            return 0;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Animation.RemoveClip");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Rewind(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 1:
                ((Animation) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Animation")).Rewind();
                return 0;

            case 2:
            {
                Animation animation2 = (Animation) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Animation");
                string luaString = LuaScriptMgr.GetLuaString(L, 2);
                animation2.Rewind(luaString);
                return 0;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Animation.Rewind");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Sample(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        ((Animation) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Animation")).Sample();
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_animatePhysics(IntPtr L)
    {
        Animation luaObject = (Animation) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name animatePhysics");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index animatePhysics on a nil value");
            }
        }
        luaObject.set_animatePhysics(LuaScriptMgr.GetBoolean(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_clip(IntPtr L)
    {
        Animation luaObject = (Animation) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name clip");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index clip on a nil value");
            }
        }
        luaObject.set_clip((AnimationClip) LuaScriptMgr.GetUnityObject(L, 3, typeof(AnimationClip)));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_cullingType(IntPtr L)
    {
        Animation luaObject = (Animation) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name cullingType");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index cullingType on a nil value");
            }
        }
        luaObject.set_cullingType((AnimationCullingType) ((int) LuaScriptMgr.GetNetObject(L, 3, typeof(AnimationCullingType))));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_localBounds(IntPtr L)
    {
        Animation luaObject = (Animation) LuaScriptMgr.GetLuaObject(L, 1);
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
    private static int set_playAutomatically(IntPtr L)
    {
        Animation luaObject = (Animation) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name playAutomatically");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index playAutomatically on a nil value");
            }
        }
        luaObject.set_playAutomatically(LuaScriptMgr.GetBoolean(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_wrapMode(IntPtr L)
    {
        Animation luaObject = (Animation) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name wrapMode");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index wrapMode on a nil value");
            }
        }
        luaObject.set_wrapMode((WrapMode) ((int) LuaScriptMgr.GetNetObject(L, 3, typeof(WrapMode))));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Stop(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 1:
                ((Animation) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Animation")).Stop();
                return 0;

            case 2:
            {
                Animation animation2 = (Animation) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Animation");
                string luaString = LuaScriptMgr.GetLuaString(L, 2);
                animation2.Stop(luaString);
                return 0;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Animation.Stop");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int SyncLayer(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Animation animation = (Animation) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Animation");
        int number = (int) LuaScriptMgr.GetNumber(L, 2);
        animation.SyncLayer(number);
        return 0;
    }
}

