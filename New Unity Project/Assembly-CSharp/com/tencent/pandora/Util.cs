namespace com.tencent.pandora
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using UnityEngine;

    public class Util
    {
        private static List<string> luaPaths = new List<string>();

        public static T Add<T>(GameObject go) where T: Component
        {
            if (go == null)
            {
                return null;
            }
            T[] components = go.GetComponents<T>();
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] != null)
                {
                    Object.Destroy(components[i]);
                }
            }
            return go.get_gameObject().AddComponent<T>();
        }

        public static T Add<T>(Transform go) where T: Component
        {
            return Add<T>(go.get_gameObject());
        }

        public static void AddClick(GameObject go, object luafuc)
        {
        }

        public static Component AddComponent(GameObject go, string assembly, string classname)
        {
            Type type = Assembly.Load(assembly).GetType(assembly + "." + classname);
            return go.AddComponent(type);
        }

        public static void AddLuaPath(string path)
        {
            if (!luaPaths.Contains(path))
            {
                if (!path.EndsWith("/"))
                {
                    path = path + "/";
                }
                luaPaths.Add(path);
            }
        }

        public static string AppContentPath()
        {
            switch (Application.get_platform())
            {
                case 8:
                    return (Application.get_dataPath() + "/Raw/");

                case 11:
                    return ("jar:file://" + Application.get_dataPath() + "!/assets/");
            }
            return (Application.get_dataPath() + "/StreamingAssets/");
        }

        public static object[] CallMethod(string module, string func, params object[] args)
        {
            try
            {
                LuaScriptMgr manager = AppFacade.Instance.GetManager<LuaScriptMgr>("LuaScriptMgr");
                if (manager == null)
                {
                    return null;
                }
                string name = (module + "." + func).Replace("(Clone)", string.Empty);
                return manager.CallLuaFunction(name, args);
            }
            catch (Exception exception)
            {
                Logger.LogNetError(1, "CallMethond Error:" + exception.Message + "," + exception.StackTrace);
                return null;
            }
        }

        public static bool CheckEnvironment()
        {
            return true;
        }

        private static int CheckRuntimeFile()
        {
            if (Application.get_isEditor())
            {
                string luaWrapPath = AppConst.LuaWrapPath;
                if (!Directory.Exists(luaWrapPath))
                {
                    return -2;
                }
                if (Directory.GetFiles(luaWrapPath).Length == 0)
                {
                    return -2;
                }
            }
            return 0;
        }

        public static GameObject Child(GameObject go, string subnode)
        {
            return Child(go.get_transform(), subnode);
        }

        public static GameObject Child(Transform go, string subnode)
        {
            Transform transform = go.FindChild(subnode);
            if (transform == null)
            {
                return null;
            }
            return transform.get_gameObject();
        }

        public static void ClearChild(Transform go)
        {
            if (go != null)
            {
                for (int i = go.get_childCount() - 1; i >= 0; i--)
                {
                    Object.Destroy(go.GetChild(i).get_gameObject());
                }
            }
        }

        public static void ClearMemory()
        {
            GC.Collect();
            Resources.UnloadUnusedAssets();
            LuaScriptMgr manager = AppFacade.Instance.GetManager<LuaScriptMgr>("LuaScriptMgr");
            if ((manager != null) && (manager.lua != null))
            {
                manager.LuaGC(new string[0]);
            }
        }

        public static string Decode(string message)
        {
            byte[] bytes = Convert.FromBase64String(message);
            return Encoding.GetEncoding("utf-8").GetString(bytes);
        }

        public static string Encode(string message)
        {
            return Convert.ToBase64String(Encoding.GetEncoding("utf-8").GetBytes(message));
        }

        public static float Float(object o)
        {
            return (float) Math.Round((double) Convert.ToSingle(o), 2);
        }

        public static T Get<T>(Component go, string subnode) where T: Component
        {
            return go.get_transform().FindChild(subnode).GetComponent<T>();
        }

        public static T Get<T>(GameObject go, string subnode) where T: Component
        {
            if (go != null)
            {
                Transform transform = go.get_transform().FindChild(subnode);
                if (transform != null)
                {
                    return transform.GetComponent<T>();
                }
            }
            return null;
        }

        public static T Get<T>(Transform go, string subnode) where T: Component
        {
            if (go != null)
            {
                Transform transform = go.FindChild(subnode);
                if (transform != null)
                {
                    return transform.GetComponent<T>();
                }
            }
            return null;
        }

        public static string getassetbundle(string name)
        {
            return (LocalPath.GetCurFilePath("www") + name);
        }

        public static string GetFileText(string path)
        {
            return File.ReadAllText(path);
        }

        public static int GetInt(string key)
        {
            return PlayerPrefs.GetInt(GetKey(key));
        }

        public static string GetKey(string key)
        {
            return ("6l_" + AppConst.UserId + "_" + key);
        }

        public static string GetString(string key)
        {
            return PlayerPrefs.GetString(GetKey(key));
        }

        public static long GetTime()
        {
            DateTime time2 = new DateTime(0x7b2, 1, 1, 0, 0, 0);
            TimeSpan span = new TimeSpan(DateTime.UtcNow.Ticks - time2.Ticks);
            return (long) span.TotalMilliseconds;
        }

        public static string HashToMD5Hex(string sourceStr)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(sourceStr);
            using (MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider())
            {
                byte[] buffer2 = provider.ComputeHash(bytes);
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < buffer2.Length; i++)
                {
                    builder.Append(buffer2[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(GetKey(key));
        }

        public static int Int(object o)
        {
            return Convert.ToInt32(o);
        }

        public static bool IsNumber(string strNumber)
        {
            Regex regex = new Regex("[^0-9]");
            return !regex.IsMatch(strNumber);
        }

        public static bool IsNumeric(string str)
        {
            if ((str == null) || (str.Length == 0))
            {
                return false;
            }
            for (int i = 0; i < str.Length; i++)
            {
                if (!char.IsNumber(str[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public static GameObject LoadAsset(AssetBundle bundle, string name)
        {
            return (bundle.Load(name, typeof(GameObject)) as GameObject);
        }

        public static GameObject LoadPrefab(string name)
        {
            return (Resources.Load(name, typeof(GameObject)) as GameObject);
        }

        public static void Log(string str)
        {
        }

        public static void LogError(string str)
        {
        }

        public static void LogWarning(string str)
        {
        }

        public static long Long(object o)
        {
            return Convert.ToInt64(o);
        }

        public static string LuaPath(string name)
        {
            Logger.d("try to get luaPath :" + name);
            string dataPath = DataPath;
            if (name.ToLower().EndsWith(".lua"))
            {
                int length = name.LastIndexOf('.');
                name = name.Substring(0, length);
            }
            name = name.Replace('.', '/');
            if (luaPaths.Count == 0)
            {
                AddLuaPath(dataPath + "Lua/");
            }
            dataPath = SearchLuaPath(name + ".lua");
            Logger.d("执行lua文件" + dataPath);
            return dataPath;
        }

        public static string md5(string source)
        {
            MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
            byte[] bytes = Encoding.UTF8.GetBytes(source);
            byte[] buffer2 = provider.ComputeHash(bytes, 0, bytes.Length);
            provider.Clear();
            string str = string.Empty;
            for (int i = 0; i < buffer2.Length; i++)
            {
                str = str + Convert.ToString(buffer2[i], 0x10).PadLeft(2, '0');
            }
            return str.PadLeft(0x20, '0');
        }

        public static string md5file(string file)
        {
            string str;
            try
            {
                FileStream inputStream = new FileStream(file, FileMode.Open);
                byte[] buffer = new MD5CryptoServiceProvider().ComputeHash(inputStream);
                inputStream.Close();
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < buffer.Length; i++)
                {
                    builder.Append(buffer[i].ToString("x2"));
                }
                str = builder.ToString();
            }
            catch (Exception exception)
            {
                throw new Exception("md5file() fail, error:" + exception.Message);
            }
            return str;
        }

        public static GameObject Peer(GameObject go, string subnode)
        {
            return Peer(go.get_transform(), subnode);
        }

        public static GameObject Peer(Transform go, string subnode)
        {
            Transform transform = go.get_parent().FindChild(subnode);
            if (transform == null)
            {
                return null;
            }
            return transform.get_gameObject();
        }

        public static int Random(int min, int max)
        {
            return UnityEngine.Random.Range(min, max);
        }

        public static float Random(float min, float max)
        {
            return UnityEngine.Random.Range(min, max);
        }

        public static void RemoveData(string key)
        {
            PlayerPrefs.DeleteKey(GetKey(key));
        }

        public static void RemoveLuaPath(string path)
        {
            luaPaths.Remove(path);
        }

        public static string SearchLuaPath(string fileName)
        {
            string path = fileName;
            for (int i = 0; i < luaPaths.Count; i++)
            {
                path = luaPaths[i] + fileName;
                if (File.Exists(path))
                {
                    return path;
                }
                Logger.d("SearchLuaPath no exist :" + fileName);
            }
            return path;
        }

        public static void SetInt(string key, int value)
        {
            string str = GetKey(key);
            PlayerPrefs.DeleteKey(str);
            PlayerPrefs.SetInt(str, value);
        }

        public static void SetString(string key, string value)
        {
            string str = GetKey(key);
            PlayerPrefs.DeleteKey(str);
            PlayerPrefs.SetString(str, value);
        }

        public static string Uid(string uid)
        {
            int num = uid.LastIndexOf('_');
            return uid.Remove(0, num + 1);
        }

        public static void Vibrate()
        {
        }

        public static string DataPath
        {
            get
            {
                if (Directory.Exists(Application.get_temporaryCachePath() + "/vercache/Lua"))
                {
                    return (Application.get_temporaryCachePath() + "/vercache/");
                }
                return (Application.get_streamingAssetsPath() + "/vercache/");
            }
        }

        public static bool isApplePlatform
        {
            get
            {
                return (((Application.get_platform() == 8) || (Application.get_platform() == null)) || (Application.get_platform() == 1));
            }
        }

        public static bool isFight
        {
            get
            {
                return (Application.get_loadedLevelName().CompareTo("fight") == 0);
            }
        }

        public static bool isLogin
        {
            get
            {
                return (Application.get_loadedLevelName().CompareTo("login") == 0);
            }
        }

        public static bool isMain
        {
            get
            {
                return (Application.get_loadedLevelName().CompareTo("main") == 0);
            }
        }

        public static bool IsWifi
        {
            get
            {
                return (Application.get_internetReachability() == 2);
            }
        }

        public static bool NetAvailable
        {
            get
            {
                return (Application.get_internetReachability() != 0);
            }
        }
    }
}

