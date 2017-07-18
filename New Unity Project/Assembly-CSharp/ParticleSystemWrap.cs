using com.tencent.pandora;
using System;
using UnityEngine;

public class ParticleSystemWrap
{
    private static Type classType = typeof(ParticleSystem);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _CreateParticleSystem(IntPtr L)
    {
        if (LuaDLL.lua_gettop(L) == 0)
        {
            ParticleSystem system = new ParticleSystem();
            LuaScriptMgr.Push(L, (Object) system);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: ParticleSystem.New");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Clear(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 1:
                ((ParticleSystem) LuaScriptMgr.GetUnityObjectSelf(L, 1, "ParticleSystem")).Clear();
                return 0;

            case 2:
            {
                ParticleSystem system2 = (ParticleSystem) LuaScriptMgr.GetUnityObjectSelf(L, 1, "ParticleSystem");
                bool boolean = LuaScriptMgr.GetBoolean(L, 2);
                system2.Clear(boolean);
                return 0;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: ParticleSystem.Clear");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Emit(IntPtr L)
    {
        int num = LuaDLL.lua_gettop(L);
        if ((num == 2) && LuaScriptMgr.CheckTypes(L, 1, typeof(ParticleSystem), typeof(ParticleSystem.Particle)))
        {
            ParticleSystem system = (ParticleSystem) LuaScriptMgr.GetUnityObjectSelf(L, 1, "ParticleSystem");
            ParticleSystem.Particle luaObject = (ParticleSystem.Particle) LuaScriptMgr.GetLuaObject(L, 2);
            system.Emit(luaObject);
            return 0;
        }
        if ((num == 2) && LuaScriptMgr.CheckTypes(L, 1, typeof(ParticleSystem), typeof(int)))
        {
            ParticleSystem system2 = (ParticleSystem) LuaScriptMgr.GetUnityObjectSelf(L, 1, "ParticleSystem");
            int num2 = (int) LuaDLL.lua_tonumber(L, 2);
            system2.Emit(num2);
            return 0;
        }
        if (num == 6)
        {
            ParticleSystem system3 = (ParticleSystem) LuaScriptMgr.GetUnityObjectSelf(L, 1, "ParticleSystem");
            Vector3 vector = LuaScriptMgr.GetVector3(L, 2);
            Vector3 vector2 = LuaScriptMgr.GetVector3(L, 3);
            float number = (float) LuaScriptMgr.GetNumber(L, 4);
            float num4 = (float) LuaScriptMgr.GetNumber(L, 5);
            Color32 color = (Color32) LuaScriptMgr.GetNetObject(L, 6, typeof(Color32));
            system3.Emit(vector, vector2, number, num4, color);
            return 0;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: ParticleSystem.Emit");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_duration(IntPtr L)
    {
        ParticleSystem luaObject = (ParticleSystem) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name duration");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index duration on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_duration());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_emissionRate(IntPtr L)
    {
        ParticleSystem luaObject = (ParticleSystem) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name emissionRate");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index emissionRate on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_emissionRate());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_enableEmission(IntPtr L)
    {
        ParticleSystem luaObject = (ParticleSystem) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name enableEmission");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index enableEmission on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_enableEmission());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_gravityModifier(IntPtr L)
    {
        ParticleSystem luaObject = (ParticleSystem) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name gravityModifier");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index gravityModifier on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_gravityModifier());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_isPaused(IntPtr L)
    {
        ParticleSystem luaObject = (ParticleSystem) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name isPaused");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index isPaused on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_isPaused());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_isPlaying(IntPtr L)
    {
        ParticleSystem luaObject = (ParticleSystem) LuaScriptMgr.GetLuaObject(L, 1);
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
    private static int get_isStopped(IntPtr L)
    {
        ParticleSystem luaObject = (ParticleSystem) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name isStopped");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index isStopped on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_isStopped());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_loop(IntPtr L)
    {
        ParticleSystem luaObject = (ParticleSystem) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name loop");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index loop on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_loop());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_maxParticles(IntPtr L)
    {
        ParticleSystem luaObject = (ParticleSystem) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name maxParticles");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index maxParticles on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_maxParticles());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_particleCount(IntPtr L)
    {
        ParticleSystem luaObject = (ParticleSystem) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name particleCount");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index particleCount on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_particleCount());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_playbackSpeed(IntPtr L)
    {
        ParticleSystem luaObject = (ParticleSystem) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name playbackSpeed");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index playbackSpeed on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_playbackSpeed());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_playOnAwake(IntPtr L)
    {
        ParticleSystem luaObject = (ParticleSystem) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name playOnAwake");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index playOnAwake on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_playOnAwake());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_randomSeed(IntPtr L)
    {
        ParticleSystem luaObject = (ParticleSystem) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name randomSeed");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index randomSeed on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_randomSeed());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_safeCollisionEventSize(IntPtr L)
    {
        ParticleSystem luaObject = (ParticleSystem) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name safeCollisionEventSize");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index safeCollisionEventSize on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_safeCollisionEventSize());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_simulationSpace(IntPtr L)
    {
        ParticleSystem luaObject = (ParticleSystem) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name simulationSpace");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index simulationSpace on a nil value");
            }
        }
        LuaScriptMgr.Push(L, (Enum) luaObject.get_simulationSpace());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_startColor(IntPtr L)
    {
        ParticleSystem luaObject = (ParticleSystem) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name startColor");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index startColor on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_startColor());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_startDelay(IntPtr L)
    {
        ParticleSystem luaObject = (ParticleSystem) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name startDelay");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index startDelay on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_startDelay());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_startLifetime(IntPtr L)
    {
        ParticleSystem luaObject = (ParticleSystem) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name startLifetime");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index startLifetime on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_startLifetime());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_startRotation(IntPtr L)
    {
        ParticleSystem luaObject = (ParticleSystem) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name startRotation");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index startRotation on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_startRotation());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_startSize(IntPtr L)
    {
        ParticleSystem luaObject = (ParticleSystem) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name startSize");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index startSize on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_startSize());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_startSpeed(IntPtr L)
    {
        ParticleSystem luaObject = (ParticleSystem) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name startSpeed");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index startSpeed on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_startSpeed());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_time(IntPtr L)
    {
        ParticleSystem luaObject = (ParticleSystem) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name time");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index time on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_time());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetClassType(IntPtr L)
    {
        LuaScriptMgr.Push(L, classType);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetCollisionEvents(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 3);
        ParticleSystem system = (ParticleSystem) LuaScriptMgr.GetUnityObjectSelf(L, 1, "ParticleSystem");
        GameObject obj2 = LuaScriptMgr.GetUnityObject(L, 2, typeof(GameObject));
        ParticleSystem.CollisionEvent[] arrayObject = LuaScriptMgr.GetArrayObject<ParticleSystem.CollisionEvent>(L, 3);
        int collisionEvents = system.GetCollisionEvents(obj2, arrayObject);
        LuaScriptMgr.Push(L, collisionEvents);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetParticles(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        ParticleSystem system = (ParticleSystem) LuaScriptMgr.GetUnityObjectSelf(L, 1, "ParticleSystem");
        ParticleSystem.Particle[] arrayObject = LuaScriptMgr.GetArrayObject<ParticleSystem.Particle>(L, 2);
        int particles = system.GetParticles(arrayObject);
        LuaScriptMgr.Push(L, particles);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int IsAlive(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 1:
            {
                bool b = ((ParticleSystem) LuaScriptMgr.GetUnityObjectSelf(L, 1, "ParticleSystem")).IsAlive();
                LuaScriptMgr.Push(L, b);
                return 1;
            }
            case 2:
            {
                ParticleSystem system2 = (ParticleSystem) LuaScriptMgr.GetUnityObjectSelf(L, 1, "ParticleSystem");
                bool boolean = LuaScriptMgr.GetBoolean(L, 2);
                bool flag3 = system2.IsAlive(boolean);
                LuaScriptMgr.Push(L, flag3);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: ParticleSystem.IsAlive");
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
    private static int Pause(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 1:
                ((ParticleSystem) LuaScriptMgr.GetUnityObjectSelf(L, 1, "ParticleSystem")).Pause();
                return 0;

            case 2:
            {
                ParticleSystem system2 = (ParticleSystem) LuaScriptMgr.GetUnityObjectSelf(L, 1, "ParticleSystem");
                bool boolean = LuaScriptMgr.GetBoolean(L, 2);
                system2.Pause(boolean);
                return 0;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: ParticleSystem.Pause");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Play(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 1:
                ((ParticleSystem) LuaScriptMgr.GetUnityObjectSelf(L, 1, "ParticleSystem")).Play();
                return 0;

            case 2:
            {
                ParticleSystem system2 = (ParticleSystem) LuaScriptMgr.GetUnityObjectSelf(L, 1, "ParticleSystem");
                bool boolean = LuaScriptMgr.GetBoolean(L, 2);
                system2.Play(boolean);
                return 0;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: ParticleSystem.Play");
        return 0;
    }

    public static void Register(IntPtr L)
    {
        LuaMethod[] regs = new LuaMethod[] { new LuaMethod("SetParticles", new LuaCSFunction(ParticleSystemWrap.SetParticles)), new LuaMethod("GetParticles", new LuaCSFunction(ParticleSystemWrap.GetParticles)), new LuaMethod("GetCollisionEvents", new LuaCSFunction(ParticleSystemWrap.GetCollisionEvents)), new LuaMethod("Simulate", new LuaCSFunction(ParticleSystemWrap.Simulate)), new LuaMethod("Play", new LuaCSFunction(ParticleSystemWrap.Play)), new LuaMethod("Stop", new LuaCSFunction(ParticleSystemWrap.Stop)), new LuaMethod("Pause", new LuaCSFunction(ParticleSystemWrap.Pause)), new LuaMethod("Clear", new LuaCSFunction(ParticleSystemWrap.Clear)), new LuaMethod("IsAlive", new LuaCSFunction(ParticleSystemWrap.IsAlive)), new LuaMethod("Emit", new LuaCSFunction(ParticleSystemWrap.Emit)), new LuaMethod("New", new LuaCSFunction(ParticleSystemWrap._CreateParticleSystem)), new LuaMethod("GetClassType", new LuaCSFunction(ParticleSystemWrap.GetClassType)), new LuaMethod("__eq", new LuaCSFunction(ParticleSystemWrap.Lua_Eq)) };
        LuaField[] fields = new LuaField[] { 
            new LuaField("startDelay", new LuaCSFunction(ParticleSystemWrap.get_startDelay), new LuaCSFunction(ParticleSystemWrap.set_startDelay)), new LuaField("isPlaying", new LuaCSFunction(ParticleSystemWrap.get_isPlaying), null), new LuaField("isStopped", new LuaCSFunction(ParticleSystemWrap.get_isStopped), null), new LuaField("isPaused", new LuaCSFunction(ParticleSystemWrap.get_isPaused), null), new LuaField("loop", new LuaCSFunction(ParticleSystemWrap.get_loop), new LuaCSFunction(ParticleSystemWrap.set_loop)), new LuaField("playOnAwake", new LuaCSFunction(ParticleSystemWrap.get_playOnAwake), new LuaCSFunction(ParticleSystemWrap.set_playOnAwake)), new LuaField("time", new LuaCSFunction(ParticleSystemWrap.get_time), new LuaCSFunction(ParticleSystemWrap.set_time)), new LuaField("duration", new LuaCSFunction(ParticleSystemWrap.get_duration), null), new LuaField("playbackSpeed", new LuaCSFunction(ParticleSystemWrap.get_playbackSpeed), new LuaCSFunction(ParticleSystemWrap.set_playbackSpeed)), new LuaField("particleCount", new LuaCSFunction(ParticleSystemWrap.get_particleCount), null), new LuaField("safeCollisionEventSize", new LuaCSFunction(ParticleSystemWrap.get_safeCollisionEventSize), null), new LuaField("enableEmission", new LuaCSFunction(ParticleSystemWrap.get_enableEmission), new LuaCSFunction(ParticleSystemWrap.set_enableEmission)), new LuaField("emissionRate", new LuaCSFunction(ParticleSystemWrap.get_emissionRate), new LuaCSFunction(ParticleSystemWrap.set_emissionRate)), new LuaField("startSpeed", new LuaCSFunction(ParticleSystemWrap.get_startSpeed), new LuaCSFunction(ParticleSystemWrap.set_startSpeed)), new LuaField("startSize", new LuaCSFunction(ParticleSystemWrap.get_startSize), new LuaCSFunction(ParticleSystemWrap.set_startSize)), new LuaField("startColor", new LuaCSFunction(ParticleSystemWrap.get_startColor), new LuaCSFunction(ParticleSystemWrap.set_startColor)), 
            new LuaField("startRotation", new LuaCSFunction(ParticleSystemWrap.get_startRotation), new LuaCSFunction(ParticleSystemWrap.set_startRotation)), new LuaField("startLifetime", new LuaCSFunction(ParticleSystemWrap.get_startLifetime), new LuaCSFunction(ParticleSystemWrap.set_startLifetime)), new LuaField("gravityModifier", new LuaCSFunction(ParticleSystemWrap.get_gravityModifier), new LuaCSFunction(ParticleSystemWrap.set_gravityModifier)), new LuaField("maxParticles", new LuaCSFunction(ParticleSystemWrap.get_maxParticles), new LuaCSFunction(ParticleSystemWrap.set_maxParticles)), new LuaField("simulationSpace", new LuaCSFunction(ParticleSystemWrap.get_simulationSpace), new LuaCSFunction(ParticleSystemWrap.set_simulationSpace)), new LuaField("randomSeed", new LuaCSFunction(ParticleSystemWrap.get_randomSeed), new LuaCSFunction(ParticleSystemWrap.set_randomSeed))
         };
        LuaScriptMgr.RegisterLib(L, "UnityEngine.ParticleSystem", typeof(ParticleSystem), regs, fields, typeof(Component));
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_emissionRate(IntPtr L)
    {
        ParticleSystem luaObject = (ParticleSystem) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name emissionRate");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index emissionRate on a nil value");
            }
        }
        luaObject.set_emissionRate((float) LuaScriptMgr.GetNumber(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_enableEmission(IntPtr L)
    {
        ParticleSystem luaObject = (ParticleSystem) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name enableEmission");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index enableEmission on a nil value");
            }
        }
        luaObject.set_enableEmission(LuaScriptMgr.GetBoolean(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_gravityModifier(IntPtr L)
    {
        ParticleSystem luaObject = (ParticleSystem) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name gravityModifier");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index gravityModifier on a nil value");
            }
        }
        luaObject.set_gravityModifier((float) LuaScriptMgr.GetNumber(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_loop(IntPtr L)
    {
        ParticleSystem luaObject = (ParticleSystem) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name loop");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index loop on a nil value");
            }
        }
        luaObject.set_loop(LuaScriptMgr.GetBoolean(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_maxParticles(IntPtr L)
    {
        ParticleSystem luaObject = (ParticleSystem) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name maxParticles");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index maxParticles on a nil value");
            }
        }
        luaObject.set_maxParticles((int) LuaScriptMgr.GetNumber(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_playbackSpeed(IntPtr L)
    {
        ParticleSystem luaObject = (ParticleSystem) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name playbackSpeed");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index playbackSpeed on a nil value");
            }
        }
        luaObject.set_playbackSpeed((float) LuaScriptMgr.GetNumber(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_playOnAwake(IntPtr L)
    {
        ParticleSystem luaObject = (ParticleSystem) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name playOnAwake");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index playOnAwake on a nil value");
            }
        }
        luaObject.set_playOnAwake(LuaScriptMgr.GetBoolean(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_randomSeed(IntPtr L)
    {
        ParticleSystem luaObject = (ParticleSystem) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name randomSeed");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index randomSeed on a nil value");
            }
        }
        luaObject.set_randomSeed((uint) LuaScriptMgr.GetNumber(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_simulationSpace(IntPtr L)
    {
        ParticleSystem luaObject = (ParticleSystem) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name simulationSpace");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index simulationSpace on a nil value");
            }
        }
        luaObject.set_simulationSpace((ParticleSystemSimulationSpace) ((int) LuaScriptMgr.GetNetObject(L, 3, typeof(ParticleSystemSimulationSpace))));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_startColor(IntPtr L)
    {
        ParticleSystem luaObject = (ParticleSystem) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name startColor");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index startColor on a nil value");
            }
        }
        luaObject.set_startColor(LuaScriptMgr.GetColor(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_startDelay(IntPtr L)
    {
        ParticleSystem luaObject = (ParticleSystem) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name startDelay");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index startDelay on a nil value");
            }
        }
        luaObject.set_startDelay((float) LuaScriptMgr.GetNumber(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_startLifetime(IntPtr L)
    {
        ParticleSystem luaObject = (ParticleSystem) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name startLifetime");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index startLifetime on a nil value");
            }
        }
        luaObject.set_startLifetime((float) LuaScriptMgr.GetNumber(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_startRotation(IntPtr L)
    {
        ParticleSystem luaObject = (ParticleSystem) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name startRotation");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index startRotation on a nil value");
            }
        }
        luaObject.set_startRotation((float) LuaScriptMgr.GetNumber(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_startSize(IntPtr L)
    {
        ParticleSystem luaObject = (ParticleSystem) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name startSize");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index startSize on a nil value");
            }
        }
        luaObject.set_startSize((float) LuaScriptMgr.GetNumber(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_startSpeed(IntPtr L)
    {
        ParticleSystem luaObject = (ParticleSystem) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name startSpeed");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index startSpeed on a nil value");
            }
        }
        luaObject.set_startSpeed((float) LuaScriptMgr.GetNumber(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_time(IntPtr L)
    {
        ParticleSystem luaObject = (ParticleSystem) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name time");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index time on a nil value");
            }
        }
        luaObject.set_time((float) LuaScriptMgr.GetNumber(L, 3));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int SetParticles(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 3);
        ParticleSystem system = (ParticleSystem) LuaScriptMgr.GetUnityObjectSelf(L, 1, "ParticleSystem");
        ParticleSystem.Particle[] arrayObject = LuaScriptMgr.GetArrayObject<ParticleSystem.Particle>(L, 2);
        int number = (int) LuaScriptMgr.GetNumber(L, 3);
        system.SetParticles(arrayObject, number);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Simulate(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 2:
            {
                ParticleSystem system = (ParticleSystem) LuaScriptMgr.GetUnityObjectSelf(L, 1, "ParticleSystem");
                float number = (float) LuaScriptMgr.GetNumber(L, 2);
                system.Simulate(number);
                return 0;
            }
            case 3:
            {
                ParticleSystem system2 = (ParticleSystem) LuaScriptMgr.GetUnityObjectSelf(L, 1, "ParticleSystem");
                float num3 = (float) LuaScriptMgr.GetNumber(L, 2);
                bool boolean = LuaScriptMgr.GetBoolean(L, 3);
                system2.Simulate(num3, boolean);
                return 0;
            }
            case 4:
            {
                ParticleSystem system3 = (ParticleSystem) LuaScriptMgr.GetUnityObjectSelf(L, 1, "ParticleSystem");
                float num4 = (float) LuaScriptMgr.GetNumber(L, 2);
                bool flag2 = LuaScriptMgr.GetBoolean(L, 3);
                bool flag3 = LuaScriptMgr.GetBoolean(L, 4);
                system3.Simulate(num4, flag2, flag3);
                return 0;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: ParticleSystem.Simulate");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Stop(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 1:
                ((ParticleSystem) LuaScriptMgr.GetUnityObjectSelf(L, 1, "ParticleSystem")).Stop();
                return 0;

            case 2:
            {
                ParticleSystem system2 = (ParticleSystem) LuaScriptMgr.GetUnityObjectSelf(L, 1, "ParticleSystem");
                bool boolean = LuaScriptMgr.GetBoolean(L, 2);
                system2.Stop(boolean);
                return 0;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: ParticleSystem.Stop");
        return 0;
    }
}

