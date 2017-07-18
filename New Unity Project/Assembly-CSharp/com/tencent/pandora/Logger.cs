namespace com.tencent.pandora
{
    using com.tencent.pandora.MiniJSON;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Threading;
    using UnityEngine;

    public class Logger : MonoBehaviour
    {
        private static string curLogFileName = string.Empty;
        private static FileStream curLogFileStream = null;
        private static bool isDestroyed = false;
        private static object lockObj = new object();
        private static Queue logMsgQueue = Queue.Synchronized(new Queue());

        public static void DEBUG(string formatMsg)
        {
            object[] objArray1 = new object[] { "D", GetThreadId(), " ", GetFileName(), ":", GetLineNum(), ":", GetFuncName(), " ", formatMsg };
            string str = string.Concat(objArray1);
            string str2 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " " + str;
            LogUnit unit = new LogUnit();
            unit.level = LogLevel.kDEBUG;
            unit.msg = str2;
            Enqueue(unit);
        }

        private static void Enqueue(LogUnit unit)
        {
            logMsgQueue.Enqueue(unit);
        }

        public static void ERROR(string formatMsg)
        {
            object[] objArray1 = new object[] { "E", GetThreadId(), " ", GetFileName(), ":", GetLineNum(), ":", GetFuncName(), " ", formatMsg };
            string str = string.Concat(objArray1);
            string str2 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " " + str;
            LogUnit unit = new LogUnit();
            unit.level = LogLevel.kERROR;
            unit.msg = str2;
            Enqueue(unit);
        }

        private static string GetFileName()
        {
            try
            {
                StackTrace trace = new StackTrace(2, true);
                return trace.GetFrame(0).GetFileName();
            }
            catch (Exception)
            {
                return "#";
            }
        }

        private static string GetFuncName()
        {
            try
            {
                StackTrace trace = new StackTrace(2, true);
                return trace.GetFrame(0).GetMethod().Name;
            }
            catch (Exception)
            {
                return "#";
            }
        }

        private static int GetLineNum()
        {
            try
            {
                StackTrace trace = new StackTrace(2, true);
                return trace.GetFrame(0).GetFileLineNumber();
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static string GetThreadId()
        {
            return string.Empty;
        }

        public static void INFO(string formatMsg)
        {
            object[] objArray1 = new object[] { "I", GetThreadId(), " ", GetFileName(), ":", GetLineNum(), ":", GetFuncName(), " ", formatMsg };
            string str = string.Concat(objArray1);
            string str2 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " " + str;
            LogUnit unit = new LogUnit();
            unit.level = LogLevel.kINFO;
            unit.msg = str2;
            Enqueue(unit);
        }

        private void OnDestroy()
        {
            isDestroyed = true;
            if (curLogFileStream != null)
            {
                curLogFileStream.Close();
            }
        }

        public static void REPORT(string msg, int reportType, int returnCode)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary["str_respara"] = msg;
            dictionary["uint_report_type"] = reportType;
            dictionary["uint_toreturncode"] = returnCode;
            UserData userData = Pandora.Instance.GetUserData();
            dictionary["str_openid"] = userData.sOpenId;
            string str = Json.Serialize(dictionary);
            LogUnit unit = new LogUnit();
            unit.level = LogLevel.kREPORT;
            unit.msg = str;
            Enqueue(unit);
        }

        private void Start()
        {
        }

        private static void SyncWriteLog(string msg)
        {
            if (!isDestroyed)
            {
                object lockObj = Logger.lockObj;
                Monitor.Enter(lockObj);
                try
                {
                    string str = Pandora.Instance.GetLogPath() + "/log-" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
                    if (str != curLogFileName)
                    {
                        curLogFileName = str;
                        if (curLogFileStream != null)
                        {
                            curLogFileStream.Close();
                            curLogFileStream = null;
                        }
                        curLogFileStream = new FileStream(curLogFileName, FileMode.Append);
                    }
                    if (curLogFileStream == null)
                    {
                        curLogFileStream = new FileStream(curLogFileName, FileMode.Append);
                    }
                    byte[] bytes = Encoding.UTF8.GetBytes(msg + "\n");
                    curLogFileStream.Write(bytes, 0, bytes.Length);
                    curLogFileStream.Flush();
                }
                catch (Exception)
                {
                }
                finally
                {
                    Monitor.Exit(lockObj);
                }
            }
        }

        private void Update()
        {
            while (logMsgQueue.Count > 0)
            {
                LogUnit unit = logMsgQueue.Dequeue() as LogUnit;
                PandoraImpl pandoraImpl = Pandora.Instance.GetPandoraImpl();
                switch (unit.level)
                {
                    case LogLevel.kDEBUG:
                        if (pandoraImpl.GetIsDebug())
                        {
                            WriteLog(unit.msg);
                        }
                        break;

                    case LogLevel.kINFO:
                        if (pandoraImpl.GetIsDebug())
                        {
                            WriteLog(unit.msg);
                        }
                        break;

                    case LogLevel.kWARN:
                        if (pandoraImpl.GetIsDebug())
                        {
                            WriteLog(unit.msg);
                        }
                        break;

                    case LogLevel.kERROR:
                        if (pandoraImpl.GetIsDebug())
                        {
                            WriteLog(unit.msg);
                        }
                        break;

                    case LogLevel.kREPORT:
                        if (pandoraImpl.GetIsNetLog())
                        {
                            Pandora.Instance.GetNetLogic().StreamReport(unit.msg);
                        }
                        break;
                }
            }
        }

        public static void WARN(string formatMsg)
        {
            object[] objArray1 = new object[] { "W", GetThreadId(), " ", GetFileName(), ":", GetLineNum(), ":", GetFuncName(), " ", formatMsg };
            string str = string.Concat(objArray1);
            string str2 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " " + str;
            LogUnit unit = new LogUnit();
            unit.level = LogLevel.kWARN;
            unit.msg = str2;
            Enqueue(unit);
        }

        private static void WriteLog(string msg)
        {
            try
            {
                string str = Pandora.Instance.GetLogPath() + "/log-" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
                if (str != curLogFileName)
                {
                    curLogFileName = str;
                    if (curLogFileStream != null)
                    {
                        curLogFileStream.Close();
                        curLogFileStream = null;
                    }
                    curLogFileStream = new FileStream(curLogFileName, FileMode.Append);
                }
                if (curLogFileStream == null)
                {
                    curLogFileStream = new FileStream(curLogFileName, FileMode.Append);
                }
                byte[] bytes = Encoding.UTF8.GetBytes(msg + "\n");
                curLogFileStream.Write(bytes, 0, bytes.Length);
            }
            catch (Exception)
            {
            }
        }

        private enum LogLevel
        {
            kDEBUG = 1,
            kERROR = 4,
            kINFO = 2,
            kREPORT = 5,
            kWARN = 3
        }

        private class LogUnit
        {
            public Logger.LogLevel level;
            public string msg = string.Empty;
        }
    }
}

