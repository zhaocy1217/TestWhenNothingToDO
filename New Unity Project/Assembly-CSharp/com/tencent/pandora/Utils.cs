namespace com.tencent.pandora
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using UnityEngine;

    public class Utils
    {
        private static int cacheVersion;

        public static void doDebugMem(string tag)
        {
            Debug.Log(getMemStr(tag));
        }

        public static string ExtractLuaName(string luaAssetBundleName)
        {
            Logger.DEBUG(luaAssetBundleName);
            string formatMsg = luaAssetBundleName.Replace(GetPlatformDesc() + "_", string.Empty);
            Logger.DEBUG(formatMsg);
            formatMsg = formatMsg.Replace("_lua.assetbundle", string.Empty);
            Logger.DEBUG(formatMsg);
            return formatMsg;
        }

        public static string GetBaseAtlasAssetBundleName()
        {
            return (GetPlatformDesc() + "__baseAtlas.assetbundle");
        }

        public static string GetBundleName(string moduleName)
        {
            return (GetPlatformDesc() + "_" + moduleName + ".assetbundle");
        }

        public static int GetCacheVersion()
        {
            if (cacheVersion == 0)
            {
                cacheVersion = NowSeconds();
            }
            return cacheVersion;
        }

        public static string GetFileName(string filePath)
        {
            try
            {
                string fileName = Path.GetFileName(filePath);
                if (fileName == null)
                {
                    return string.Empty;
                }
                return fileName;
            }
            catch (Exception exception)
            {
                Logger.ERROR(exception.StackTrace);
                return string.Empty;
            }
        }

        public static string getMemStr(string tag)
        {
            IntPtr ptr = AndroidJNI.FindClass("com/tencent/msdk/u3d/DebugMemInfo");
            IntPtr ptr2 = AndroidJNI.GetStaticMethodID(ptr, "getmem_result", "(Ljava/lang/String;)[B");
            jvalue[] jvalueArray = new jvalue[1];
            jvalueArray[0].l = AndroidJNI.NewStringUTF(tag);
            byte[] bytes = AndroidJNI.FromByteArray(AndroidJNI.CallStaticObjectMethod(ptr, ptr2, jvalueArray));
            return Encoding.Default.GetString(bytes);
        }

        public static string GetPlatformDesc()
        {
            if ((Application.get_platform() == 7) || (Application.get_platform() == 2))
            {
                return "pc";
            }
            if (Application.get_platform() == 11)
            {
                return "android";
            }
            if (Application.get_platform() == null)
            {
                return "mac";
            }
            return "ios";
        }

        public static bool IsLuaAssetBundle(string assetBundleName)
        {
            return assetBundleName.EndsWith("_lua.assetbundle");
        }

        public static int NowSeconds()
        {
            return (int) DateTime.UtcNow.Subtract(new DateTime(0x7b2, 1, 1, 0, 0, 0)).TotalSeconds;
        }

        public static bool ParseConfigData(Dictionary<string, object> content, ref Dictionary<string, object> result)
        {
            Logger.DEBUG(string.Empty);
            try
            {
                int num = 0;
                if (content.ContainsKey("id"))
                {
                    num = Convert.ToInt32(content["id"]);
                }
                int num2 = -1;
                if (content.ContainsKey("totalSwitch"))
                {
                    num2 = Convert.ToInt32(content["totalSwitch"]);
                }
                if (num2 == 0)
                {
                    result["ruleId"] = num;
                    result["totalSwitch"] = false;
                    return true;
                }
                if (num2 != 1)
                {
                    return false;
                }
                Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
                if (content.ContainsKey("function_switch"))
                {
                    string str = content["function_switch"] as string;
                    char[] separator = new char[] { ',' };
                    foreach (string str2 in str.Split(separator))
                    {
                        char[] chArray2 = new char[] { ':' };
                        string[] strArray3 = str2.Split(chArray2);
                        if (strArray3.Length == 2)
                        {
                            string str3 = strArray3[0];
                            int num4 = Convert.ToInt32(strArray3[1]);
                            dictionary[str3] = num4 == 1;
                        }
                    }
                }
                int num5 = 0;
                if (content.ContainsKey("isDebug"))
                {
                    num5 = Convert.ToInt32(content["isDebug"]);
                }
                int num6 = 1;
                if (content.ContainsKey("isNetLog"))
                {
                    num6 = Convert.ToInt32(content["isNetLog"]);
                }
                string str4 = string.Empty;
                string str5 = string.Empty;
                string str6 = string.Empty;
                ushort num7 = 0;
                if (content.ContainsKey("ip"))
                {
                    str4 = Convert.ToString(content["ip"]);
                }
                if (content.ContainsKey("port"))
                {
                    num7 = Convert.ToUInt16(content["port"]);
                }
                if (content.ContainsKey("cap_ip1"))
                {
                    str5 = Convert.ToString(content["cap_ip1"]);
                }
                if (content.ContainsKey("cap_ip2"))
                {
                    str6 = Convert.ToString(content["cap_ip2"]);
                }
                if ((((str4.Length == 0) && (str5.Length == 0)) && (str6.Length == 0)) || (num7 == 0))
                {
                    return false;
                }
                int length = -1;
                Dictionary<string, List<PandoraImpl.FileState>> dictionary2 = new Dictionary<string, List<PandoraImpl.FileState>>();
                List<string> list = new List<string>();
                HashSet<string> set = new HashSet<string>();
                if (content.ContainsKey("dependency"))
                {
                    string str7 = content["dependency"] as string;
                    char[] chArray3 = new char[] { '|' };
                    string[] strArray4 = str7.Split(chArray3);
                    length = strArray4.Length;
                    foreach (string str8 in strArray4)
                    {
                        char[] chArray4 = new char[] { ':' };
                        string[] strArray6 = str8.Split(chArray4);
                        if (strArray6.Length == 2)
                        {
                            string str9 = strArray6[0];
                            string str10 = strArray6[1];
                            char[] chArray5 = new char[] { ',' };
                            string[] strArray7 = str10.Split(chArray5);
                            List<PandoraImpl.FileState> list2 = new List<PandoraImpl.FileState>();
                            foreach (string str11 in strArray7)
                            {
                                PandoraImpl.FileState item = new PandoraImpl.FileState();
                                item.name = str11;
                                list2.Add(item);
                                if (!set.Contains(str11))
                                {
                                    list.Add(str11);
                                    set.Add(str11);
                                }
                            }
                            dictionary2[str9] = list2;
                        }
                    }
                }
                if ((length <= 0) || (length != dictionary2.Count))
                {
                    return false;
                }
                int num11 = -1;
                List<PandoraImpl.DownloadASTask> list3 = new List<PandoraImpl.DownloadASTask>();
                if (content.ContainsKey("sourcelist"))
                {
                    Dictionary<string, object> dictionary3 = content["sourcelist"] as Dictionary<string, object>;
                    if (((dictionary3 != null) && dictionary3.ContainsKey("count")) && dictionary3.ContainsKey("list"))
                    {
                        int num12 = Convert.ToInt32(dictionary3["count"]);
                        List<object> list4 = dictionary3["list"] as List<object>;
                        if (num12 == list4.Count)
                        {
                            num11 = num12;
                            HashSet<string> set2 = new HashSet<string>();
                            foreach (object obj2 in list4)
                            {
                                Dictionary<string, object> dictionary4 = obj2 as Dictionary<string, object>;
                                if ((dictionary4.ContainsKey("url") && dictionary4.ContainsKey("luacmd5")) && dictionary4.ContainsKey("size"))
                                {
                                    PandoraImpl.DownloadASTask task = new PandoraImpl.DownloadASTask();
                                    task.url = dictionary4["url"] as string;
                                    task.size = (int) ((long) dictionary4["size"]);
                                    task.md5 = dictionary4["luacmd5"] as string;
                                    task.name = Path.GetFileName(task.url);
                                    if ((((task.name != null) && (task.name.Length > 0)) && ((task.md5.Length > 0) && (task.size > 0))) && !set2.Contains(task.name))
                                    {
                                        list3.Add(task);
                                        set2.Add(task.name);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        num11 = 0;
                    }
                }
                if ((num11 < 0) || (num11 != list3.Count))
                {
                    return false;
                }
                result["ruleId"] = num;
                result["totalSwitch"] = true;
                result["isDebug"] = num5 == 1;
                result["isNetLog"] = num6 == 1;
                result["brokerHost"] = str4;
                result["brokerPort"] = num7;
                result["brokerAltIp1"] = str5;
                result["brokerAltIp2"] = str6;
                result["functionSwitches"] = dictionary;
                result["dependencyInfos"] = dictionary2;
                result["dependencyAll"] = list;
                result["pendingDownloadASTasks"] = list3;
                return true;
            }
            catch (Exception exception)
            {
                Logger.ERROR(exception.StackTrace);
            }
            return true;
        }

        public static string ReadCookie(string fileName)
        {
            try
            {
                return File.ReadAllText(Pandora.Instance.GetCookiePath() + "/" + fileName);
            }
            catch (Exception exception)
            {
                Logger.WARN(exception.Message);
                return string.Empty;
            }
        }

        public static byte[] ReadFileBytes(string filePath)
        {
            try
            {
                byte[] buffer = File.ReadAllBytes(filePath);
                if (buffer == null)
                {
                    return new byte[0];
                }
                return buffer;
            }
            catch (Exception exception)
            {
                Logger.ERROR(exception.StackTrace);
                return new byte[0];
            }
        }

        public static void ResetCacheVersion()
        {
            cacheVersion = 0;
        }

        public static bool WriteCookie(string fileName, string content)
        {
            try
            {
                File.WriteAllText(Pandora.Instance.GetCookiePath() + "/" + fileName, content);
                return true;
            }
            catch (Exception exception)
            {
                Logger.ERROR(exception.StackTrace);
                return false;
            }
        }
    }
}

