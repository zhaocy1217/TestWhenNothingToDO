namespace com.tencent.pandora
{
    using System;
    using System.IO;
    using UnityEngine;

    public static class LuaStatic
    {
        public static string init_luanet = "local metatable = {}\n            local rawget = rawget\n            local debug = debug\n            local import_type = luanet.import_type\n            local load_assembly = luanet.load_assembly\n            luanet.error, luanet.type = error, type\n            -- Lookup a .NET identifier component.\n            function metatable:__index(key) -- key is e.g. 'Form'\n            -- Get the fully-qualified name, e.g. 'System.Windows.Forms.Form'\n            local fqn = rawget(self,'.fqn')\n            fqn = ((fqn and fqn .. '.') or '') .. key\n\n            -- Try to find either a luanet function or a CLR type\n            local obj = rawget(luanet,key) or import_type(fqn)\n\n            -- If key is neither a luanet function or a CLR type, then it is simply\n            -- an identifier component.\n            if obj == nil then\n                -- It might be an assembly, so we load it too.\n                    pcall(load_assembly,fqn)\n                    obj = { ['.fqn'] = fqn }\n            setmetatable(obj, metatable)\n            end\n\n            -- Cache this lookup\n            rawset(self, key, obj)\n            return obj\n            end\n\n            -- A non-type has been called; e.g. foo = System.Foo()\n            function metatable:__call(...)\n            error('No such type: ' .. rawget(self,'.fqn'), 2)\n            end\n\n            -- This is the root of the .NET namespace\n            luanet['.fqn'] = false\n            setmetatable(luanet, metatable)\n\n            -- Preload the mscorlib assembly\n            luanet.load_assembly('mscorlib')\n\n            function traceback(msg)                \n                return debug.traceback(msg, 1)                \n            end";
        public static ReadLuaFile Load = new ReadLuaFile(LuaStatic.DefaultLoader);
        public static WriteLuaLog LuaLog = new WriteLuaLog(LuaStatic.DefaultLuaLogger);

        private static byte[] DefaultLoader(string path)
        {
            byte[] buffer = null;
            if (File.Exists(path))
            {
                buffer = File.ReadAllBytes(path);
            }
            return buffer;
        }

        private static void DefaultLuaLogger(string msg)
        {
            Debug.Log(msg);
        }

        [MonoPInvokeCallback(typeof(LuaCSFunction))]
        public static int dofile(IntPtr L)
        {
            string name = string.Empty;
            name = LuaDLL.lua_tostring(L, 1);
            if (name.ToLower().EndsWith(".lua"))
            {
                int length = name.LastIndexOf('.');
                name = name.Substring(0, length);
            }
            name = name.Replace('.', '/') + ".lua";
            int num2 = LuaDLL.lua_gettop(L);
            byte[] buff = Load(name);
            if ((buff != null) && (LuaDLL.luaL_loadbuffer(L, buff, buff.Length, name) == 0))
            {
                LuaDLL.lua_call(L, 0, LuaDLL.LUA_MULTRET);
            }
            return (LuaDLL.lua_gettop(L) - num2);
        }

        [MonoPInvokeCallback(typeof(LuaCSFunction))]
        public static int importWrap(IntPtr L)
        {
            string str = string.Empty;
            str = LuaDLL.lua_tostring(L, 1).Replace('.', '_');
            if (!string.IsNullOrEmpty(str))
            {
                LuaBinder.Bind(L, str);
            }
            return 0;
        }

        [MonoPInvokeCallback(typeof(LuaCSFunction))]
        public static int loader(IntPtr L)
        {
            string name = string.Empty;
            name = LuaDLL.lua_tostring(L, 1);
            if (name.ToLower().EndsWith(".lua"))
            {
                int length = name.LastIndexOf('.');
                name = name.Substring(0, length);
            }
            name = name.Replace('.', '/') + ".lua";
            LuaScriptMgr mgrFromLuaState = LuaScriptMgr.GetMgrFromLuaState(L);
            if (mgrFromLuaState == null)
            {
                return 0;
            }
            LuaDLL.lua_pushstdcallcfunction(L, mgrFromLuaState.lua.tracebackFunction, 0);
            int oldTop = LuaDLL.lua_gettop(L);
            byte[] buff = Load(name);
            if (buff == null)
            {
                if (!name.Contains("mobdebug"))
                {
                    Debug.LogError("Loader lua file failed: " + name);
                }
                LuaDLL.lua_pop(L, 1);
                return 0;
            }
            if (LuaDLL.luaL_loadbuffer(L, buff, buff.Length, name) != 0)
            {
                mgrFromLuaState.lua.ThrowExceptionFromError(oldTop);
                LuaDLL.lua_pop(L, 1);
            }
            return 1;
        }

        [MonoPInvokeCallback(typeof(LuaCSFunction))]
        public static int loadfile(IntPtr L)
        {
            return loader(L);
        }

        [MonoPInvokeCallback(typeof(LuaCSFunction))]
        public static int panic(IntPtr L)
        {
            string message = string.Format("unprotected error in call to Lua API ({0})", LuaDLL.lua_tostring(L, -1));
            LuaDLL.lua_pop(L, 1);
            throw new LuaException(message);
        }

        [MonoPInvokeCallback(typeof(LuaCSFunction))]
        public static int print(IntPtr L)
        {
            int num = LuaDLL.lua_gettop(L);
            string str = string.Empty;
            LuaDLL.lua_getglobal(L, "tostring");
            for (int i = 1; i <= num; i++)
            {
                LuaDLL.lua_pushvalue(L, -1);
                LuaDLL.lua_pushvalue(L, i);
                LuaDLL.lua_call(L, 1, 1);
                if (i > 1)
                {
                    str = str + "\t";
                }
                str = str + LuaDLL.lua_tostring(L, -1);
                LuaDLL.lua_pop(L, 1);
            }
            LuaLog("LUA: " + str);
            return 0;
        }

        [MonoPInvokeCallback(typeof(LuaCSFunction))]
        public static int traceback(IntPtr L)
        {
            LuaDLL.lua_getglobal(L, "debug");
            LuaDLL.lua_getfield(L, -1, "traceback");
            LuaDLL.lua_pushvalue(L, 1);
            LuaDLL.lua_pushnumber(L, 2.0);
            LuaDLL.lua_call(L, 2, 1);
            return 1;
        }
    }
}

