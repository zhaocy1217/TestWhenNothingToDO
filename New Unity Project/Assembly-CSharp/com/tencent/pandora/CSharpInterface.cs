namespace com.tencent.pandora
{
    using com.tencent.pandora.MiniJSON;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class CSharpInterface
    {
        public static void AddClick(GameObject go, LuaFunction luaFunc)
        {
            <AddClick>c__AnonStorey73 storey = new <AddClick>c__AnonStorey73();
            storey.luaFunc = luaFunc;
            storey.go = go;
            storey.go.GetComponent<Button>().get_onClick().RemoveAllListeners();
            storey.go.GetComponent<Button>().get_onClick().AddListener(new UnityAction(storey, (IntPtr) this.<>m__86));
        }

        public static void AddUGUIOnClickDown(GameObject go, LuaFunction luaFunc)
        {
            <AddUGUIOnClickDown>c__AnonStorey74 storey = new <AddUGUIOnClickDown>c__AnonStorey74();
            storey.luaFunc = luaFunc;
            storey.go = go;
            EventTriggerListener listener1 = EventTriggerListener.Get(storey.go);
            listener1.onDown = (EventTriggerListener.VoidDelegate) Delegate.Combine(listener1.onDown, new EventTriggerListener.VoidDelegate(storey.<>m__87));
        }

        public static void AddUGUIOnClickUp(GameObject go, LuaFunction luaFunc)
        {
            <AddUGUIOnClickUp>c__AnonStorey75 storey = new <AddUGUIOnClickUp>c__AnonStorey75();
            storey.luaFunc = luaFunc;
            storey.go = go;
            EventTriggerListener listener1 = EventTriggerListener.Get(storey.go);
            listener1.onUp = (EventTriggerListener.VoidDelegate) Delegate.Combine(listener1.onUp, new EventTriggerListener.VoidDelegate(storey.<>m__88));
        }

        public static bool AndroidPay(string jsonParams)
        {
            Logger.DEBUG(jsonParams);
            return MidasUtil.AndroidPay(jsonParams);
        }

        public static int AssembleFont(string panelName, string jsonFontTable)
        {
            return Pandora.Instance.GetResourceMgr().AssembleFont(panelName, jsonFontTable);
        }

        public static void AsyncDownloadImage(string url)
        {
            Logger.DEBUG(url);
            Pandora.Instance.GetPandoraImpl().ShowIMG(string.Empty, url, null, 0);
        }

        public static void AsyncSetImage(string panelName, string url, Image image, uint callId)
        {
            Logger.DEBUG("panelName=" + panelName + " url=" + url + " callId=" + callId.ToString());
            Pandora.Instance.GetPandoraImpl().ShowIMG(panelName, url, image, callId);
        }

        public static void CallBroker(uint callId, string strReqJson, int cmdId)
        {
            Logger.DEBUG("callId=" + callId.ToString() + " strReqJson=" + strReqJson + " cmdId=" + cmdId.ToString());
            Pandora.Instance.GetNetLogic().CallBroker(callId, strReqJson, cmdId);
        }

        public static void CallGame(string strCmdJson)
        {
            Logger.DEBUG(strCmdJson);
            Pandora.Instance.CallGame(strCmdJson);
        }

        public static void CreatePanel(uint callId, string panelName)
        {
            <CreatePanel>c__AnonStorey72 storey = new <CreatePanel>c__AnonStorey72();
            storey.panelName = panelName;
            storey.callId = callId;
            Logger.DEBUG("callId=" + storey.callId.ToString() + " panelName=" + storey.panelName);
            storey.pandoraImpl = Pandora.Instance.GetPandoraImpl();
            Action<bool> onCreatePanel = new Action<bool>(storey.<>m__85);
            storey.pandoraImpl.CreatePanel(storey.panelName, onCreatePanel);
        }

        public static void DestroyPanel(string panelName)
        {
            Logger.DEBUG("panelName=" + panelName);
            Pandora.Instance.GetPandoraImpl().DestroyPanel(panelName);
        }

        public static void DoCmdFromGame(string jsonData)
        {
            try
            {
                Logger.DEBUG("jsonData=" + jsonData);
                LuaScriptMgr luaScriptMgr = Pandora.Instance.GetLuaScriptMgr();
                if (luaScriptMgr != null)
                {
                    object[] args = new object[] { jsonData };
                    luaScriptMgr.CallLuaFunction("Common.DoCmdFromGame", args);
                }
            }
            catch (Exception exception)
            {
                Logger.ERROR(exception.StackTrace);
            }
        }

        public static void ExecCallback(uint callId, string result)
        {
            try
            {
                Logger.DEBUG("callId=" + callId.ToString() + " result=" + result);
                LuaScriptMgr luaScriptMgr = Pandora.Instance.GetLuaScriptMgr();
                if (luaScriptMgr != null)
                {
                    object[] args = new object[] { callId, result };
                    luaScriptMgr.CallLuaFunction("Common.ExecCallback", args);
                }
            }
            catch (Exception exception)
            {
                Logger.ERROR(exception.StackTrace);
            }
        }

        public static bool GetFunctionSwitch(string functionName)
        {
            return Pandora.Instance.GetPandoraImpl().GetFunctionSwitch(functionName);
        }

        public static Logger GetLogger()
        {
            return Pandora.Instance.GetLogger();
        }

        public static GameObject GetPanel(string panelName)
        {
            return Pandora.Instance.GetResourceMgr().GetPanel(panelName);
        }

        public static string GetPlatformDesc()
        {
            return Utils.GetPlatformDesc();
        }

        public static string GetSDKVersion()
        {
            return Pandora.Instance.GetSDKVersion();
        }

        public static bool GetTotalSwitch()
        {
            return Pandora.Instance.GetPandoraImpl().GetTotalSwitch();
        }

        public static UserData GetUserData()
        {
            return Pandora.Instance.GetUserData();
        }

        public static bool IOSPay(string jsonParams)
        {
            Logger.DEBUG(jsonParams);
            return false;
        }

        public static bool IsImageDownloaded(string url)
        {
            return Pandora.Instance.GetPandoraImpl().IsImgDownloaded(url);
        }

        public static void NotifyAndroidPayFinish(string jsonData)
        {
            try
            {
                Logger.DEBUG("jsonData=" + jsonData);
                LuaScriptMgr luaScriptMgr = Pandora.Instance.GetLuaScriptMgr();
                if (luaScriptMgr != null)
                {
                    object[] args = new object[] { jsonData };
                    luaScriptMgr.CallLuaFunction("Common.NotifyAndroidPayFinish", args);
                }
            }
            catch (Exception exception)
            {
                Logger.ERROR(exception.StackTrace);
            }
        }

        public static void NotifyCloseAllPanel()
        {
            try
            {
                LuaScriptMgr luaScriptMgr = Pandora.Instance.GetLuaScriptMgr();
                if (luaScriptMgr != null)
                {
                    luaScriptMgr.CallLuaFunction("Common.NotifyCloseAllPanel", new object[0]);
                }
            }
            catch (Exception exception)
            {
                Logger.ERROR(exception.StackTrace);
            }
        }

        public static void NotifyIOSPayFinish(string jsonData)
        {
            try
            {
                Logger.DEBUG("jsonData=" + jsonData);
                LuaScriptMgr luaScriptMgr = Pandora.Instance.GetLuaScriptMgr();
                if (luaScriptMgr != null)
                {
                    object[] args = new object[] { jsonData };
                    luaScriptMgr.CallLuaFunction("Common.NotifyIOSPayFinish", args);
                }
            }
            catch (Exception exception)
            {
                Logger.ERROR(exception.StackTrace);
            }
        }

        public static void NotifyPushData(string actionName, string jsonData)
        {
            try
            {
                Logger.DEBUG("actionName=" + actionName + " jsonData=" + jsonData);
                LuaScriptMgr luaScriptMgr = Pandora.Instance.GetLuaScriptMgr();
                if (luaScriptMgr != null)
                {
                    object[] args = new object[] { actionName, jsonData };
                    luaScriptMgr.CallLuaFunction("Common.NotifyPushData", args);
                }
            }
            catch (Exception exception)
            {
                Logger.ERROR(exception.StackTrace);
            }
        }

        public static string ReadCookie(string fileName)
        {
            return Utils.ReadCookie(fileName);
        }

        public static void SetPosition(GameObject go, float x, float y, float z)
        {
            go.get_transform().set_localPosition(new Vector3(x, y, z));
        }

        public static void SetPosZ(GameObject go, float z)
        {
            go.get_transform().set_localPosition(new Vector3(go.get_transform().get_localPosition().x, go.get_transform().get_localPosition().y, z));
        }

        public static void SetScale(GameObject go, float x, float y, float z)
        {
            go.get_transform().set_localScale(new Vector3(x, y, z));
        }

        public static void ShowGameImg(int djType, int djID, GameObject go, uint callId)
        {
            int num = Pandora.Instance.GetGameDjImage(go, djType, djID);
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary["djType"] = djType.ToString();
            dictionary["djID"] = djID.ToString();
            dictionary["RetCode"] = num.ToString();
            string result = Json.Serialize(dictionary);
            ExecCallback(callId, result);
        }

        public static void StreamReport(string msg, int reportType, int returnCode)
        {
            Logger.DEBUG("msg=" + msg + " reportType=" + reportType.ToString() + " returnCode=" + returnCode.ToString());
            Logger.REPORT(msg, reportType, returnCode);
        }

        public static bool WriteCookie(string fileName, string content)
        {
            return Utils.WriteCookie(fileName, content);
        }

        public static bool isApplePlatform
        {
            get
            {
                return (((Application.get_platform() == 8) || (Application.get_platform() == null)) || (Application.get_platform() == 1));
            }
        }

        [CompilerGenerated]
        private sealed class <AddClick>c__AnonStorey73
        {
            internal GameObject go;
            internal LuaFunction luaFunc;

            internal void <>m__86()
            {
                object[] args = new object[] { this.go };
                this.luaFunc.Call(args);
            }
        }

        [CompilerGenerated]
        private sealed class <AddUGUIOnClickDown>c__AnonStorey74
        {
            internal GameObject go;
            internal LuaFunction luaFunc;

            internal void <>m__87(GameObject o)
            {
                object[] args = new object[] { this.go };
                this.luaFunc.Call(args);
            }
        }

        [CompilerGenerated]
        private sealed class <AddUGUIOnClickUp>c__AnonStorey75
        {
            internal GameObject go;
            internal LuaFunction luaFunc;

            internal void <>m__88(GameObject o)
            {
                object[] args = new object[] { this.go };
                this.luaFunc.Call(args);
            }
        }

        [CompilerGenerated]
        private sealed class <CreatePanel>c__AnonStorey72
        {
            internal uint callId;
            internal PandoraImpl pandoraImpl;
            internal string panelName;

            internal void <>m__85(bool status)
            {
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                dictionary["RetCode"] = !status ? "-1" : "0";
                dictionary["PanelName"] = this.panelName;
                string result = Json.Serialize(dictionary);
                CSharpInterface.ExecCallback(this.callId, result);
                if (!status)
                {
                    Logger.ERROR(string.Empty);
                    this.pandoraImpl.DestroyPanel(this.panelName);
                }
            }
        }
    }
}

