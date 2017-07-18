namespace com.tencent.pandora
{
    using com.tencent.pandora.MiniJSON;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class PandoraImpl : MonoBehaviour
    {
        private BootupStatus bootupStatus = BootupStatus.kInitial;
        private string brokerAltIp1 = string.Empty;
        private string brokerAltIp2 = string.Empty;
        private string brokerHost = string.Empty;
        private ushort brokerPort;
        private List<string> dependencyAll = new List<string>();
        private Dictionary<string, List<FileState>> dependencyInfos = new Dictionary<string, List<FileState>>();
        private HashSet<string> executedLuaAssetBundles = new HashSet<string>();
        private Dictionary<string, bool> functionSwitches = new Dictionary<string, bool>();
        private bool isDebug;
        private bool isDependencyLoading;
        private bool isNetLog = true;
        private int lastReBootTime = -1;
        private bool loadBaseAtlasSucc = true;
        private LuaScriptMgr luaMgr;
        private bool luaMgrInited;
        private int maxDownloadingTaskNum = 5;
        private NetLogic netLogic;
        private List<DownloadASTask> pendingDownloadASTasks = new List<DownloadASTask>();
        private ResourceMgr resMgr;
        private int retryDownloadASInterval = 5;
        private int ruleId;
        private Dictionary<string, List<ShowImgTask>> showImgTasks = new Dictionary<string, List<ShowImgTask>>();
        private bool totalSwitch;

        public void Bootup()
        {
            Logger.DEBUG(string.Empty);
            Action<int, Dictionary<string, object>> action = new Action<int, Dictionary<string, object>>(this, (IntPtr) this.<Bootup>m__96);
            this.bootupStatus = BootupStatus.kReadingConfig;
            this.netLogic.GetRemoteConfig(action);
        }

        public void CloseAllPanel()
        {
            Logger.DEBUG(string.Empty);
            if (this.luaMgrInited)
            {
                Logger.DEBUG(string.Empty);
                CSharpInterface.NotifyCloseAllPanel();
            }
        }

        public void CreatePanel(string panelName, Action<bool> onCreatePanel)
        {
            base.StartCoroutine(this.resMgr.CreatePanel(panelName, onCreatePanel));
        }

        public void DestroyPanel(string panelName)
        {
            this.resMgr.DestroyPanel(panelName);
        }

        public void Do(Dictionary<string, string> cmdDict)
        {
            try
            {
                Logger.DEBUG(string.Empty);
                if ((cmdDict.ContainsKey("type") && cmdDict.ContainsKey("content")) && cmdDict["type"].Equals("inMainSence"))
                {
                    if (cmdDict["content"].Equals("0"))
                    {
                        if (this.netLogic != null)
                        {
                            this.netLogic.SetDownloadingPaused(true);
                        }
                        Logger.INFO("SetDownloadingPaused=true");
                    }
                    else
                    {
                        if (this.netLogic != null)
                        {
                            this.netLogic.SetDownloadingPaused(false);
                        }
                        Logger.INFO("SetDownloadingPaused=false");
                    }
                }
                if (this.luaMgrInited)
                {
                    Logger.DEBUG(string.Empty);
                    CSharpInterface.DoCmdFromGame(Json.Serialize(cmdDict));
                }
            }
            catch (Exception exception)
            {
                Logger.ERROR(exception.Message);
            }
        }

        public object[] DoLoadedLua(string luaName)
        {
            Logger.DEBUG(luaName);
            string luaString = this.resMgr.GetLuaString(luaName);
            if (luaString != null)
            {
                return this.luaMgr.DoString(luaString);
            }
            return null;
        }

        private void FixedUpdate()
        {
            try
            {
                this.luaMgr.FixedUpdate();
            }
            catch (Exception exception)
            {
                Logger.ERROR(exception.Message);
            }
        }

        public bool GetFunctionSwitch(string name)
        {
            if (this.functionSwitches.ContainsKey(name))
            {
                return this.functionSwitches[name];
            }
            return false;
        }

        public bool GetIsDebug()
        {
            return this.isDebug;
        }

        public bool GetIsNetLog()
        {
            return this.isNetLog;
        }

        public LuaScriptMgr GetLuaScriptMgr()
        {
            return this.luaMgr;
        }

        public NetLogic GetNetLogic()
        {
            return this.netLogic;
        }

        public ResourceMgr GetResourceMgr()
        {
            return this.resMgr;
        }

        public bool GetTotalSwitch()
        {
            return this.totalSwitch;
        }

        private void HotUpdate()
        {
            if (this.bootupStatus == BootupStatus.kReadConfigDone)
            {
                Logger.DEBUG(string.Empty);
                this.resMgr.Init();
                this.resMgr.DeleteRedundantFiles(this.dependencyAll);
                List<DownloadASTask> list = new List<DownloadASTask>();
                foreach (DownloadASTask task in this.pendingDownloadASTasks)
                {
                    if (!this.dependencyAll.Contains(task.name))
                    {
                        list.Add(task);
                    }
                }
                foreach (DownloadASTask task2 in list)
                {
                    this.pendingDownloadASTasks.Remove(task2);
                }
                list.Clear();
                this.bootupStatus = BootupStatus.kLocalLoading;
            }
            if (this.bootupStatus == BootupStatus.kLocalLoading)
            {
                Logger.DEBUG(string.Empty);
                while (this.dependencyAll.Count > 0)
                {
                    <HotUpdate>c__AnonStorey81 storey = new <HotUpdate>c__AnonStorey81();
                    storey.<>f__this = this;
                    if (this.isDependencyLoading)
                    {
                        Logger.DEBUG("waiting local loading!");
                        return;
                    }
                    storey.first = this.dependencyAll[0];
                    string str = string.Empty;
                    storey.taskOfDependency = null;
                    foreach (DownloadASTask task3 in this.pendingDownloadASTasks)
                    {
                        if (storey.first == task3.name)
                        {
                            storey.taskOfDependency = task3;
                            str = task3.md5;
                            break;
                        }
                    }
                    Logger.DEBUG("first=" + storey.first + " md5=" + str);
                    if (this.resMgr.IsFileExistsInCache(storey.first, str) || (!this.resMgr.IsFileExistsInCache(storey.first, str) && (storey.taskOfDependency == null)))
                    {
                        Logger.DEBUG("first=" + storey.first + " md5=" + str);
                        Action<bool> callback = new Action<bool>(storey.<>m__97);
                        if (Utils.IsLuaAssetBundle(storey.first))
                        {
                            Logger.DEBUG(string.Empty);
                            this.isDependencyLoading = true;
                            base.StartCoroutine(this.resMgr.LoadLuaAssetBundle(storey.first, callback));
                        }
                        else
                        {
                            Logger.DEBUG(string.Empty);
                            callback(true);
                        }
                    }
                    else
                    {
                        this.resMgr.DeleteFile(storey.first);
                        this.dependencyAll.RemoveAt(0);
                    }
                }
                if (this.dependencyAll.Count == 0)
                {
                    Logger.DEBUG(string.Empty);
                    this.bootupStatus = BootupStatus.kLocalLoadDone;
                }
            }
            if ((this.bootupStatus == BootupStatus.kLocalLoadDone) || (this.bootupStatus == BootupStatus.kDownloading))
            {
                this.bootupStatus = BootupStatus.kDownloading;
                int num = 0;
                List<DownloadASTask> list2 = new List<DownloadASTask>(this.pendingDownloadASTasks);
                foreach (DownloadASTask task4 in list2)
                {
                    if (task4.isDownloading)
                    {
                        num++;
                    }
                }
                foreach (DownloadASTask task5 in list2)
                {
                    <HotUpdate>c__AnonStorey82 storey2 = new <HotUpdate>c__AnonStorey82();
                    storey2.<>f__this = this;
                    storey2.task = task5;
                    int num2 = Utils.NowSeconds() - storey2.task.lastDownloadTime;
                    if (!storey2.task.isDownloading && (num2 > this.retryDownloadASInterval))
                    {
                        Logger.DEBUG(string.Concat(new object[] { "task.url=", storey2.task.url, " task.size=", storey2.task.size, " task.md5=", storey2.task.md5 }));
                        Action<int, Dictionary<string, object>> action = new Action<int, Dictionary<string, object>>(storey2, (IntPtr) this.<>m__98);
                        storey2.task.isDownloading = true;
                        num++;
                        storey2.task.lastDownloadTime = Utils.NowSeconds();
                        string destFile = Pandora.Instance.GetCachePath() + "/" + Path.GetFileName(storey2.task.url);
                        this.netLogic.AddDownload(storey2.task.url, storey2.task.size, storey2.task.md5, destFile, 0, action);
                        if (num >= this.maxDownloadingTaskNum)
                        {
                            break;
                        }
                    }
                }
                if (this.pendingDownloadASTasks.Count == 0)
                {
                    Logger.DEBUG(string.Empty);
                    this.bootupStatus = BootupStatus.kDownloadDone;
                }
            }
        }

        public void Init()
        {
            try
            {
                this.luaMgr = new LuaScriptMgr();
                this.resMgr = new ResourceMgr();
                this.netLogic = new NetLogic();
                this.netLogic.Init();
                Directory.CreateDirectory(Pandora.Instance.GetCachePath());
                Directory.CreateDirectory(Pandora.Instance.GetImgPath());
                Directory.CreateDirectory(Pandora.Instance.GetCookiePath());
                Directory.CreateDirectory(Pandora.Instance.GetLogPath());
                Directory.CreateDirectory(Pandora.Instance.GetTempPath());
            }
            catch (Exception exception)
            {
                Logger.ERROR(exception.StackTrace);
            }
        }

        [DebuggerHidden]
        private IEnumerator InternalShowIMG(string panelName, string url, Object go, uint callId)
        {
            <InternalShowIMG>c__Iterator33 iterator = new <InternalShowIMG>c__Iterator33();
            iterator.panelName = panelName;
            iterator.url = url;
            iterator.callId = callId;
            iterator.go = go;
            iterator.<$>panelName = panelName;
            iterator.<$>url = url;
            iterator.<$>callId = callId;
            iterator.<$>go = go;
            iterator.<>f__this = this;
            return iterator;
        }

        public bool IsImgDownloaded(string url)
        {
            try
            {
                string fileName = Path.GetFileName(url);
                string[] textArray1 = new string[] { Pandora.Instance.GetImgPath(), "/", url.GetHashCode().ToString(), "-", fileName };
                if (File.Exists(string.Concat(textArray1)))
                {
                    return true;
                }
            }
            catch (Exception exception)
            {
                Logger.WARN(exception.StackTrace);
            }
            return false;
        }

        private void LateUpdate()
        {
            try
            {
                this.luaMgr.LateUpate();
            }
            catch (Exception exception)
            {
                Logger.ERROR(exception.Message);
            }
        }

        public void LogOut()
        {
            Logger.DEBUG(string.Empty);
            try
            {
                Utils.ResetCacheVersion();
                this.luaMgrInited = false;
                this.CloseAllPanel();
                this.netLogic.Logout();
                this.resMgr.Logout();
                this.luaMgr.Destroy();
                this.luaMgr = new LuaScriptMgr();
                this.lastReBootTime = -1;
                this.bootupStatus = BootupStatus.kInitial;
                base.StopAllCoroutines();
                this.ruleId = 0;
                this.totalSwitch = false;
                this.isDebug = false;
                this.isNetLog = true;
                this.brokerHost = string.Empty;
                this.brokerPort = 0;
                this.brokerAltIp1 = string.Empty;
                this.brokerAltIp2 = string.Empty;
                this.functionSwitches = new Dictionary<string, bool>();
                this.dependencyInfos = new Dictionary<string, List<FileState>>();
                this.dependencyAll = new List<string>();
                this.isDependencyLoading = false;
                this.pendingDownloadASTasks = new List<DownloadASTask>();
                this.showImgTasks = new Dictionary<string, List<ShowImgTask>>();
                this.executedLuaAssetBundles = new HashSet<string>();
            }
            catch (Exception exception)
            {
                Logger.ERROR(exception.Message + ":" + exception.StackTrace);
            }
        }

        private void OnApplicationQuit()
        {
            Logger.DEBUG(string.Empty);
            if (this.netLogic != null)
            {
                this.netLogic.Destroy();
            }
        }

        public void ShowIMG(string panelName, string url, Object go, uint callId)
        {
            <ShowIMG>c__AnonStorey83 storey = new <ShowIMG>c__AnonStorey83();
            storey.panelName = panelName;
            storey.url = url;
            storey.callId = callId;
            storey.<>f__this = this;
            try
            {
                Logger.DEBUG("panelName=" + storey.panelName + " url=" + storey.url + " callId=" + storey.callId.ToString());
                string fileName = Path.GetFileName(storey.url);
                string[] textArray2 = new string[] { Pandora.Instance.GetImgPath(), "/", storey.url.GetHashCode().ToString(), "-", fileName };
                string path = string.Concat(textArray2);
                if (File.Exists(path))
                {
                    Logger.DEBUG(string.Empty);
                    base.StartCoroutine(this.InternalShowIMG(storey.panelName, storey.url, go, storey.callId));
                }
                else
                {
                    ShowImgTask item = new ShowImgTask();
                    item.panelName = storey.panelName;
                    item.url = storey.url;
                    item.go = go;
                    item.callId = storey.callId;
                    if (!this.showImgTasks.ContainsKey(storey.url))
                    {
                        Logger.DEBUG(string.Empty);
                        Action<int, Dictionary<string, object>> action = new Action<int, Dictionary<string, object>>(storey, (IntPtr) this.<>m__99);
                        List<ShowImgTask> list = new List<ShowImgTask>();
                        list.Add(item);
                        this.showImgTasks[storey.url] = list;
                        this.netLogic.AddDownload(storey.url, 0, string.Empty, path, 0, action);
                    }
                    else
                    {
                        this.showImgTasks[storey.url].Add(item);
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.ERROR(exception.StackTrace);
            }
        }

        private void Start()
        {
        }

        private void TryDoLua(string newReadyAssetBundle)
        {
            try
            {
                Logger.DEBUG(newReadyAssetBundle);
                string fileName = Utils.GetPlatformDesc() + "_ulua_lua.assetbundle";
                if (this.resMgr.IsLuaAssetBundleLoaded(fileName) && !this.luaMgrInited)
                {
                    LuaStatic.Load = new ReadLuaFile(this.resMgr.GetLuaBytes);
                    this.luaMgr.DoFile = new LuaScriptMgr.FileExecutor(this.DoLoadedLua);
                    this.luaMgr.Start();
                    this.luaMgrInited = true;
                }
                if (this.luaMgrInited)
                {
                    foreach (KeyValuePair<string, List<FileState>> pair in this.dependencyInfos)
                    {
                        bool flag = true;
                        foreach (FileState state in pair.Value)
                        {
                            if (!state.isReady)
                            {
                                flag = false;
                            }
                        }
                        if (flag)
                        {
                            foreach (FileState state2 in pair.Value)
                            {
                                if ((Utils.IsLuaAssetBundle(state2.name) && !state2.name.Equals(fileName)) && !this.executedLuaAssetBundles.Contains(state2.name))
                                {
                                    Logger.DEBUG(state2.name);
                                    string name = Utils.ExtractLuaName(state2.name);
                                    this.luaMgr.DoFile(name);
                                    this.executedLuaAssetBundles.Add(state2.name);
                                }
                            }
                            continue;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.ERROR(exception.Message + "\n" + exception.StackTrace);
            }
        }

        private void Update()
        {
            try
            {
                if (this.netLogic != null)
                {
                    this.netLogic.Drive();
                }
                if (this.luaMgr != null)
                {
                    this.luaMgr.Update();
                }
                if (this.bootupStatus == BootupStatus.kReadConfigFailed)
                {
                    int num = Utils.NowSeconds();
                    if ((this.lastReBootTime + 10) < num)
                    {
                        this.lastReBootTime = num;
                        this.Bootup();
                    }
                }
                else if (((this.bootupStatus >= BootupStatus.kReadConfigDone) && this.totalSwitch) && this.loadBaseAtlasSucc)
                {
                    this.HotUpdate();
                }
            }
            catch (Exception exception)
            {
                Logger.ERROR(exception.StackTrace);
            }
        }

        [CompilerGenerated]
        private sealed class <HotUpdate>c__AnonStorey81
        {
            internal PandoraImpl <>f__this;
            internal string first;
            internal PandoraImpl.DownloadASTask taskOfDependency;

            internal void <>m__97(bool status)
            {
                this.<>f__this.isDependencyLoading = false;
                if (status)
                {
                    Logger.DEBUG(this.first + " loaded");
                    foreach (KeyValuePair<string, List<PandoraImpl.FileState>> pair in this.<>f__this.dependencyInfos)
                    {
                        foreach (PandoraImpl.FileState state in pair.Value)
                        {
                            if (state.name == this.first)
                            {
                                state.isReady = true;
                            }
                        }
                    }
                    this.<>f__this.TryDoLua(this.first);
                    this.<>f__this.dependencyAll.RemoveAt(0);
                    if (this.taskOfDependency != null)
                    {
                        this.<>f__this.pendingDownloadASTasks.Remove(this.taskOfDependency);
                    }
                }
                else
                {
                    Logger.ERROR(string.Empty);
                    this.<>f__this.dependencyAll.RemoveAt(0);
                    this.<>f__this.resMgr.DeleteFile(this.first);
                }
            }
        }

        [CompilerGenerated]
        private sealed class <HotUpdate>c__AnonStorey82
        {
            internal PandoraImpl <>f__this;
            internal PandoraImpl.DownloadASTask task;

            internal void <>m__98(int downloadRet, Dictionary<string, object> result)
            {
                Logger.DEBUG(string.Concat(new object[] { "task.url=", this.task.url, " task.size=", this.task.size, " task.md5=", this.task.md5 }));
                if (downloadRet == 0)
                {
                    Logger.DEBUG(string.Empty);
                    this.<>f__this.resMgr.AddCacheFileMeta(this.task.name, this.task.size, this.task.md5);
                    Action<bool> callback = delegate (bool status) {
                        this.task.isDownloading = false;
                        this.<>f__this.pendingDownloadASTasks.Remove(this.task);
                        if (status)
                        {
                            Logger.DEBUG(string.Empty);
                            foreach (KeyValuePair<string, List<PandoraImpl.FileState>> pair in this.<>f__this.dependencyInfos)
                            {
                                foreach (PandoraImpl.FileState state in pair.Value)
                                {
                                    if (state.name == this.task.name)
                                    {
                                        state.isReady = true;
                                    }
                                }
                            }
                            this.<>f__this.TryDoLua(this.task.name);
                            this.<>f__this.pendingDownloadASTasks.Remove(this.task);
                        }
                        else
                        {
                            Logger.ERROR(this.task.name + " load to mem failed!");
                            this.<>f__this.resMgr.DeleteFile(this.task.name);
                        }
                    };
                    if (Utils.IsLuaAssetBundle(this.task.name))
                    {
                        this.<>f__this.StartCoroutine(this.<>f__this.resMgr.LoadLuaAssetBundle(this.task.name, callback));
                    }
                    else
                    {
                        callback(true);
                    }
                }
                else
                {
                    Logger.ERROR(string.Empty);
                    this.task.isDownloading = false;
                }
            }

            internal void <>m__9A(bool status)
            {
                this.task.isDownloading = false;
                this.<>f__this.pendingDownloadASTasks.Remove(this.task);
                if (status)
                {
                    Logger.DEBUG(string.Empty);
                    foreach (KeyValuePair<string, List<PandoraImpl.FileState>> pair in this.<>f__this.dependencyInfos)
                    {
                        foreach (PandoraImpl.FileState state in pair.Value)
                        {
                            if (state.name == this.task.name)
                            {
                                state.isReady = true;
                            }
                        }
                    }
                    this.<>f__this.TryDoLua(this.task.name);
                    this.<>f__this.pendingDownloadASTasks.Remove(this.task);
                }
                else
                {
                    Logger.ERROR(this.task.name + " load to mem failed!");
                    this.<>f__this.resMgr.DeleteFile(this.task.name);
                }
            }
        }

        [CompilerGenerated]
        private sealed class <InternalShowIMG>c__Iterator33 : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal uint <$>callId;
            internal Object <$>go;
            internal string <$>panelName;
            internal string <$>url;
            internal PandoraImpl <>f__this;
            internal Dictionary<string, string> <dictResult>__5;
            internal Exception <ex>__8;
            internal Image <image>__6;
            internal string <imgName>__1;
            internal string <imgPath>__2;
            internal GameObject <panel>__0;
            internal string <result>__9;
            internal Texture2D <texture>__7;
            internal WWW <www>__4;
            internal string <wwwImgPath>__3;
            internal uint callId;
            internal Object go;
            internal string panelName;
            internal string url;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        Logger.DEBUG("panelName=" + this.panelName + " url=" + this.url + " callId=" + this.callId.ToString());
                        if (this.go != null)
                        {
                            this.<panel>__0 = this.<>f__this.resMgr.GetPanel(this.panelName);
                            if (this.<panel>__0 == null)
                            {
                                Logger.ERROR("panel " + this.panelName + " not exists");
                            }
                            else
                            {
                                Logger.DEBUG(string.Empty);
                                this.<imgName>__1 = Path.GetFileName(this.url);
                                string[] textArray2 = new string[] { Pandora.Instance.GetImgPath(), "/", this.url.GetHashCode().ToString(), "-", this.<imgName>__1 };
                                this.<imgPath>__2 = string.Concat(textArray2);
                                this.<wwwImgPath>__3 = "file://" + this.<imgPath>__2;
                                Logger.DEBUG("panel " + this.panelName + " wwwImgPath=" + this.<wwwImgPath>__3 + " begin");
                                this.<www>__4 = new WWW(this.<wwwImgPath>__3);
                                this.$current = this.<www>__4;
                                this.$PC = 1;
                                return true;
                            }
                            break;
                        }
                        break;

                    case 1:
                        Logger.DEBUG("panel " + this.panelName + " wwwImgPath=" + this.<wwwImgPath>__3 + " end");
                        this.<dictResult>__5 = new Dictionary<string, string>();
                        this.<dictResult>__5["PanelName"] = this.panelName;
                        this.<dictResult>__5["Url"] = this.url;
                        this.<dictResult>__5["RetCode"] = "-1";
                        if (string.IsNullOrEmpty(this.<www>__4.get_error()) && (this.<www>__4.get_bytes() != null))
                        {
                            try
                            {
                                Logger.DEBUG("panel " + this.panelName + " wwwImgPath=" + this.<wwwImgPath>__3 + " create sprite");
                                this.<image>__6 = this.go as Image;
                                this.<texture>__7 = new Texture2D((int) this.<image>__6.get_rectTransform().get_sizeDelta().x, (int) this.<image>__6.get_rectTransform().get_sizeDelta().y, 3, false);
                                this.<texture>__7.LoadImage(this.<www>__4.get_bytes());
                                this.<image>__6.set_sprite(Sprite.Create(this.<texture>__7, new Rect(0f, 0f, (float) this.<texture>__7.get_width(), (float) this.<texture>__7.get_height()), new Vector2(0f, 0f)));
                                Logger.DEBUG("panel " + this.panelName + " wwwImgPath=" + this.<wwwImgPath>__3 + " create sprite done");
                                this.<dictResult>__5["RetCode"] = "0";
                            }
                            catch (Exception exception)
                            {
                                this.<ex>__8 = exception;
                                Logger.ERROR(this.<ex>__8.StackTrace);
                            }
                        }
                        this.<result>__9 = Json.Serialize(this.<dictResult>__5);
                        CSharpInterface.ExecCallback(this.callId, this.<result>__9);
                        this.$PC = -1;
                        break;
                }
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <ShowIMG>c__AnonStorey83
        {
            internal PandoraImpl <>f__this;
            internal uint callId;
            internal string panelName;
            internal string url;

            internal void <>m__99(int status, Dictionary<string, object> content)
            {
                Logger.DEBUG("panelName=" + this.panelName + " url=" + this.url + " callId=" + this.callId.ToString());
                List<PandoraImpl.ShowImgTask> list = this.<>f__this.showImgTasks[this.url];
                this.<>f__this.showImgTasks.Remove(this.url);
                foreach (PandoraImpl.ShowImgTask task in list)
                {
                    this.<>f__this.StartCoroutine(this.<>f__this.InternalShowIMG(task.panelName, this.url, task.go, task.callId));
                }
            }
        }

        private enum BootupStatus
        {
            kDownloadDone = 7,
            kDownloading = 6,
            kInitial = -1,
            kLoadLoadFailed = 4,
            kLocalLoadDone = 5,
            kLocalLoading = 3,
            kReadConfigDone = 2,
            kReadConfigFailed = 1,
            kReadingConfig = 0
        }

        public class DownloadASTask
        {
            public bool isDownloading;
            public int lastDownloadTime = -1;
            public string md5 = string.Empty;
            public string name = string.Empty;
            public int size = -1;
            public string url = string.Empty;
        }

        public class FileState
        {
            public bool isReady;
            public string name = string.Empty;
        }

        public class ShowImgTask
        {
            public uint callId;
            public Object go;
            public string panelName = string.Empty;
            public string url = string.Empty;
        }
    }
}

