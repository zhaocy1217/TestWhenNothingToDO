namespace com.tencent.pandora
{
    using com.tencent.pandora.MiniJSON;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class Pandora
    {
        private string cachePath = string.Empty;
        private Action<Dictionary<string, string>> callbackForGame;
        private Func<GameObject, int, int, int> callbackForGameDjImage;
        private string cookiePath = string.Empty;
        private string[] fontResources = new string[] { "Pandora/FontPrefabs/usefont" };
        private string imgPath = string.Empty;
        public static readonly Pandora Instance = new Pandora();
        private bool isInited;
        private string kSDKVersion = "YXZJ-Android-V13";
        private Logger logger;
        private string logPath = string.Empty;
        private GameObject pandoraGameObject;
        private PandoraImpl pandoraImpl;
        private GameObject pandoraParent;
        private int panelBaseDepth = 100;
        private GameObject panelParent;
        private string streamingAssetsPath = string.Empty;
        private string tempPath = string.Empty;
        private UserData userData = new UserData();

        public void CallGame(Dictionary<string, string> cmdDict)
        {
            Logger.DEBUG(string.Empty);
            if (this.callbackForGame != null)
            {
                this.callbackForGame(cmdDict);
            }
        }

        public void CallGame(string strCmdJson)
        {
            Logger.DEBUG(strCmdJson);
            try
            {
                Dictionary<string, object> dictionary = Json.Deserialize(strCmdJson) as Dictionary<string, object>;
                Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
                foreach (KeyValuePair<string, object> pair in dictionary)
                {
                    if (pair.Value == null)
                    {
                        Logger.DEBUG(strCmdJson);
                        dictionary2[pair.Key] = string.Empty;
                    }
                    else
                    {
                        dictionary2[pair.Key] = pair.Value as string;
                    }
                }
                if (this.callbackForGame != null)
                {
                    this.callbackForGame(dictionary2);
                }
            }
            catch (Exception exception)
            {
                Logger.ERROR(exception.Message + ":" + exception.StackTrace);
            }
        }

        public void CloseAllPanel()
        {
            Logger.DEBUG(string.Empty);
            if (this.pandoraImpl != null)
            {
                this.pandoraImpl.CloseAllPanel();
            }
        }

        public void Do(Dictionary<string, string> cmdDict)
        {
            Logger.DEBUG(Json.Serialize(cmdDict));
            if (this.pandoraImpl != null)
            {
                this.pandoraImpl.Do(cmdDict);
            }
        }

        public string GetCachePath()
        {
            return this.cachePath;
        }

        public string GetCookiePath()
        {
            return this.cookiePath;
        }

        public string[] GetFontResources()
        {
            return this.fontResources;
        }

        public int GetGameDjImage(GameObject go, int djType, int djID)
        {
            Logger.DEBUG(string.Empty);
            int num = -1;
            if (this.callbackForGameDjImage != null)
            {
                num = this.callbackForGameDjImage.Invoke(go, djType, djID);
            }
            return num;
        }

        public GameObject GetGameObject()
        {
            return this.pandoraGameObject;
        }

        public string GetImgPath()
        {
            return this.imgPath;
        }

        public Logger GetLogger()
        {
            return this.logger;
        }

        public string GetLogPath()
        {
            return this.logPath;
        }

        public LuaScriptMgr GetLuaScriptMgr()
        {
            return this.pandoraImpl.GetLuaScriptMgr();
        }

        public NetLogic GetNetLogic()
        {
            return this.pandoraImpl.GetNetLogic();
        }

        public PandoraImpl GetPandoraImpl()
        {
            return this.pandoraImpl;
        }

        public int GetPanelBaseDepth()
        {
            return this.panelBaseDepth;
        }

        public GameObject GetPanelParent()
        {
            if (this.panelParent == null)
            {
                GameObject obj2 = GameObject.Find("UI Root");
                if (obj2 != null)
                {
                    this.panelParent = obj2;
                }
            }
            return this.panelParent;
        }

        public ResourceMgr GetResourceMgr()
        {
            return this.pandoraImpl.GetResourceMgr();
        }

        public string GetSDKVersion()
        {
            return this.kSDKVersion;
        }

        public string GetStreamingAssetsPath()
        {
            return this.streamingAssetsPath;
        }

        public string GetTempPath()
        {
            return this.tempPath;
        }

        public bool GetTotalSwitch()
        {
            Logger.DEBUG(string.Empty);
            return ((this.pandoraImpl != null) && this.pandoraImpl.GetTotalSwitch());
        }

        public UserData GetUserData()
        {
            return this.userData;
        }

        public void Init()
        {
            if (!this.isInited)
            {
                string str = Application.get_temporaryCachePath();
                this.logPath = str + "/Pandora/logs";
                this.cachePath = str + "/Pandora/cache";
                this.imgPath = this.cachePath + "/imgs";
                this.cookiePath = this.cachePath + "/cookies";
                this.tempPath = str + "/Pandora/temp";
                this.streamingAssetsPath = Application.get_streamingAssetsPath() + "/Pandora";
                this.pandoraGameObject = new GameObject("Pandora GameObject");
                Object.DontDestroyOnLoad(this.pandoraGameObject);
                this.pandoraImpl = this.pandoraGameObject.AddComponent<PandoraImpl>();
                this.logger = this.pandoraGameObject.AddComponent<Logger>();
                this.pandoraGameObject.AddComponent<MidasUtil>();
                this.pandoraImpl.Init();
                this.isInited = true;
            }
        }

        public void Logout()
        {
            Logger.DEBUG(string.Empty);
            this.userData.Clear();
            if (this.pandoraImpl != null)
            {
                this.pandoraImpl.LogOut();
            }
        }

        public void SetCallback(Action<Dictionary<string, string>> action)
        {
            Logger.DEBUG(string.Empty);
            this.callbackForGame = action;
        }

        public void SetGetDjImageCallback(Func<GameObject, int, int, int> action)
        {
            Logger.DEBUG(string.Empty);
            this.callbackForGameDjImage = action;
        }

        public void SetPandoraParent(GameObject thePandoraParent)
        {
            Logger.DEBUG(string.Empty);
            this.pandoraParent = thePandoraParent;
            if (this.pandoraGameObject != null)
            {
                this.pandoraGameObject.get_transform().set_parent(this.pandoraParent.get_transform());
            }
        }

        public void SetPanelBaseDepth(int depth)
        {
            Logger.DEBUG(depth.ToString());
            this.panelBaseDepth = depth;
        }

        public void SetPanelParent(GameObject thePanelParent)
        {
            Logger.DEBUG(string.Empty);
            this.panelParent = thePanelParent;
        }

        public void SetUserData(Dictionary<string, string> dictPara)
        {
            Logger.DEBUG(Json.Serialize(dictPara));
            if (this.userData.IsRoleEmpty())
            {
                this.userData.Assgin(dictPara);
                if (this.pandoraImpl != null)
                {
                    this.pandoraImpl.Bootup();
                }
            }
            else
            {
                this.userData.Refresh(dictPara);
            }
        }
    }
}

