namespace com.tencent.pandora
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class ResourceManager : View
    {
        public bool atlasUpdate;
        public bool blUIResUnLoad;
        private List<DownTaskRunner> finished = new List<DownTaskRunner>();
        public Dictionary<string, TextAsset> luaFiles = new Dictionary<string, TextAsset>();
        public string m_atlasPath = "Pandora/Fonts/6l/ActionCenterAtlas/6lActionCenterAtlas";
        public string m_fontPath = "Pandora/Fonts/6l/ActionCenterFont/usefont";
        public Dictionary<string, Sprite> mdictAtlas = new Dictionary<string, Sprite>();
        public Dictionary<string, Object> mdictFont = new Dictionary<string, Object>();
        private float passTimeNow;
        public float passTimeSpan = 4f;
        public string resPath;
        private List<DownTaskRunner> runnner = new List<DownTaskRunner>();
        public bool startTimer;
        private Queue<DownTask> task = new Queue<DownTask>();
        private int taskMaxCount = 5;
        public bool useSA;
        private string verName;

        public void AssemplyUI(GameObject goParent = new GameObject())
        {
            GameObject obj2 = goParent;
            if ((obj2 == null) && (base.PanelMgr.pages.Count > 0))
            {
                foreach (GameObject obj3 in base.PanelMgr.pages.Values)
                {
                    if (obj3 != null)
                    {
                        obj2 = obj3;
                        break;
                    }
                }
            }
            if (this.mdictFont.Count == 0)
            {
                Logger.d("mdictFont.Count = 0");
            }
            else if (obj2 != null)
            {
                Text[] componentsInChildren = obj2.GetComponentsInChildren<Text>(true);
                if (componentsInChildren.Length > 0)
                {
                    Logger.d("Font for interface assignment");
                    for (int i = 0; i < componentsInChildren.Length; i++)
                    {
                        if (this.mdictFont["usefont"] != null)
                        {
                            componentsInChildren[i].set_font(this.mdictFont["usefont"] as Font);
                            componentsInChildren[i].set_text(componentsInChildren[i].get_text());
                        }
                    }
                }
            }
        }

        private void DownloadRes()
        {
            if ((this.runnner.Count < this.taskMaxCount) && (this.task.Count > 0))
            {
                this.runnner.Add(new DownTaskRunner(this.task.Dequeue()));
            }
            foreach (DownTaskRunner runner in this.runnner)
            {
                if (string.IsNullOrEmpty(runner.www.get_error()))
                {
                    if (runner.www.get_isDone())
                    {
                        Logger.d("The Task DownLoad OK : " + runner.www.get_url().ToString());
                        this.finished.Add(runner);
                        runner.task.onload.Invoke(runner.www, runner.task.tag);
                    }
                    else if (runner.task.ondownloading != null)
                    {
                        runner.task.ondownloading.Invoke(runner.www, runner.task.tag);
                    }
                    continue;
                }
                Logger.d("The Task DownLoad Failed : " + runner.www.get_url().ToString() + "  Error :  " + runner.www.get_error());
                if (runner.task.ondownerror != null)
                {
                    runner.task.ondownerror(new Exception(runner.www.get_error()));
                }
                this.finished.Add(runner);
            }
            foreach (DownTaskRunner runner2 in this.finished)
            {
                this.runnner.Remove(runner2);
            }
            this.finished.Clear();
        }

        public static string getPlatformDesc()
        {
            if ((Application.get_platform() == 7) || (Application.get_platform() == 2))
            {
                return "standalonewindows";
            }
            if (Application.get_platform() == 11)
            {
                return "android";
            }
            return "iphone";
        }

        private void Load(string path, string tag, Action<WWW, string> onLoad, Action<WWW, string> onLoaddowning, Action<Exception> onerror = new Action<Exception>())
        {
            this.task.Enqueue(new DownTask(path, tag, onLoad, onLoaddowning, onerror));
        }

        public void LoadBundleRes(string filename, Action<AssetBundle> reFunc, Action<Exception> onerror = new Action<Exception>())
        {
            <LoadBundleRes>c__AnonStorey31 storey = new <LoadBundleRes>c__AnonStorey31();
            storey.reFunc = reFunc;
            storey.<>f__this = this;
            Action<WWW, string> onLoad = new Action<WWW, string>(storey, (IntPtr) this.<>m__6);
            this.LoadFromRemote(filename, filename, onLoad, null, onerror);
        }

        private void LoadFromRemote(string filename, string tag, Action<WWW, string> onLoad, Action<WWW, string> onLoaddowning, Action<Exception> onerror = new Action<Exception>())
        {
            string[] textArray1 = new string[] { this.resPath, getPlatformDesc(), "_", filename, ".assetbundle" };
            string path = string.Concat(textArray1);
            Logger.d("start down task :" + path);
            this.Load(path, tag, onLoad, onLoaddowning, onerror);
        }

        public void LoadLua()
        {
            Action<AssetBundle> reFunc = delegate (AssetBundle bundle) {
                if (bundle != null)
                {
                    Object[] objArray = bundle.LoadAll();
                    if (objArray.Length > 0)
                    {
                        this.luaFiles.Clear();
                        for (int j = 0; j < objArray.Length; j++)
                        {
                            char[] separator = new char[] { '.' };
                            this.luaFiles[objArray[j].get_name().Split(separator)[0]] = objArray[j] as TextAsset;
                        }
                        bundle.Unload(false);
                        base.GameMgr.Init();
                    }
                    else
                    {
                        Logger.LogNetError(0x3ec, "lua.assetbundle do not have lua file");
                        Logger.d("lua.assetbundle do not have lua file");
                    }
                }
                else
                {
                    Logger.d("loaded lua assetbundle failed：bundle = null");
                }
            };
            this.LoadBundleRes("lua", reFunc, null);
        }

        private void LoadUIAtlas()
        {
        }

        private void LoadUIFont()
        {
            try
            {
                GameObject obj2 = Resources.Load(this.m_fontPath) as GameObject;
                Font font = obj2.GetComponent<Text>().get_font();
                if (font != null)
                {
                    this.mdictFont["usefont"] = font;
                }
                else
                {
                    Logger.d("Get Font is null, path : " + this.m_fontPath);
                }
            }
            catch (Exception exception)
            {
                Logger.d("LoadUIFont error :" + exception.Message);
                Pandora.SetCloseIcon();
            }
        }

        public void LoadUIRes(string atlasname = "")
        {
            Action<AssetBundle> reFunc = delegate (AssetBundle ab) {
                if (ab != null)
                {
                    Object[] objArray = ab.LoadAll(typeof(Sprite));
                    if (objArray != null)
                    {
                        for (int j = 0; j < objArray.Length; j++)
                        {
                            this.mdictAtlas[objArray[j].get_name()] = objArray[j] as Sprite;
                        }
                    }
                    ab.Unload(false);
                    this.AssemplyUI(null);
                }
                else
                {
                    Logger.d("load atlas  null");
                }
            };
            if (this.useSA)
            {
                if (atlasname != string.Empty)
                {
                    this.LoadBundleRes(atlasname, reFunc, null);
                }
                else
                {
                    Logger.d("altas name is null, check your code.");
                }
            }
            else if (atlasname != string.Empty)
            {
                this.LoadBundleRes(atlasname, reFunc, null);
            }
            else
            {
                Logger.d("altas name is null, check your code.");
            }
        }

        private void OnDestroy()
        {
            Logger.d("~ResourceManager was destroy!");
        }

        public void SetResPath()
        {
            this.resPath = Configer.m_CurHotUpdatePath;
            if (!string.IsNullOrEmpty(this.resPath))
            {
                try
                {
                    if (Directory.Exists(this.resPath) && (Directory.GetFiles(this.resPath).Length > 0))
                    {
                        this.resPath = "file://" + this.resPath;
                        return;
                    }
                }
                catch (Exception)
                {
                    this.useSA = true;
                }
            }
            this.useSA = true;
            this.resPath = Application.get_streamingAssetsPath() + "/vercache/";
        }

        private void Start()
        {
            this.verName = getPlatformDesc() + "_allver.ver.txt";
            this.SetResPath();
            string str = string.Empty;
            if (!string.IsNullOrEmpty(str))
            {
                this.LoadUIRes(str);
            }
            this.LoadUIFont();
            this.LoadLua();
        }

        private void UnloadUIRes(Action func = new Action())
        {
        }

        private void Update()
        {
            if (this.startTimer)
            {
                this.passTimeNow += Time.get_deltaTime();
                if (this.passTimeNow > this.passTimeSpan)
                {
                    this.UnloadUIRes(null);
                }
            }
            else
            {
                this.passTimeNow = 0f;
            }
            if (!Pandora.NotDoUpdate)
            {
                this.DownloadRes();
            }
        }

        [CompilerGenerated]
        private sealed class <LoadBundleRes>c__AnonStorey31
        {
            internal ResourceManager <>f__this;
            internal Action<AssetBundle> reFunc;

            internal void <>m__6(WWW w, string tag)
            {
                if (string.IsNullOrEmpty(w.get_error()))
                {
                    AssetBundle bundle = w.get_assetBundle();
                    if (bundle != null)
                    {
                        this.reFunc(bundle);
                    }
                    else
                    {
                        Logger.d("load BundleRes failed: bundle = null/ tag = " + tag + "/ path = " + w.get_url());
                        if (!this.<>f__this.get_name().Contains("lua"))
                        {
                            Pandora.SetCloseIcon();
                        }
                        this.reFunc(null);
                    }
                }
                else
                {
                    Logger.LogNetError(0x3ec, "load failed:" + w.get_error() + "/ tag = " + tag);
                    Logger.d("load BundleRes failed:" + w.get_error() + "/ tag = " + tag + "/ path = " + w.get_url());
                    if (!this.<>f__this.get_name().Contains("lua"))
                    {
                        Pandora.SetCloseIcon();
                    }
                    this.reFunc(null);
                }
            }
        }

        private class DownTask
        {
            public Action<Exception> ondownerror;
            public Action<WWW, string> ondownloading;
            public Action<WWW, string> onload;
            public string path;
            public string tag;

            public DownTask(string path, string tag, Action<WWW, string> onload, Action<WWW, string> ondownloading, Action<Exception> ondownError = new Action<Exception>())
            {
                this.path = path;
                this.tag = tag;
                this.onload = onload;
                this.ondownloading = ondownloading;
                this.ondownerror = ondownError;
            }
        }

        private class DownTaskRunner
        {
            public ResourceManager.DownTask task;
            public WWW www;

            public DownTaskRunner(ResourceManager.DownTask task)
            {
                this.task = task;
                this.www = new WWW(this.task.path);
                Logger.d("Sart Download Task From :" + this.task.path);
            }
        }
    }
}

