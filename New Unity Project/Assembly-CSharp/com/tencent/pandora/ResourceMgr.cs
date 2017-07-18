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

    public class ResourceMgr
    {
        private string cacheMetaFile = string.Empty;
        private HashSet<string> creatingPanels = new HashSet<string>();
        private Dictionary<string, object> dictCacheFileInfo = new Dictionary<string, object>();
        private Dictionary<string, Font> fonts = new Dictionary<string, Font>();
        private HashSet<string> loadedLuaAssetBundles = new HashSet<string>();
        private Dictionary<string, TextAsset> loadedLuaFiles = new Dictionary<string, TextAsset>();
        private Dictionary<string, AssetBundle> panelBundles = new Dictionary<string, AssetBundle>();
        private Dictionary<string, GameObject> panels = new Dictionary<string, GameObject>();

        public void AddCacheFileMeta(string fileName, int fileSize, string fileMD5)
        {
            try
            {
                Dictionary<string, object> dictionary = new Dictionary<string, object>();
                dictionary["name"] = fileName;
                dictionary["size"] = fileSize;
                dictionary["md5"] = fileMD5;
                this.dictCacheFileInfo[fileName] = dictionary;
                string formatMsg = Json.Serialize(this.dictCacheFileInfo);
                Logger.DEBUG(formatMsg);
                File.WriteAllText(this.cacheMetaFile, formatMsg);
            }
            catch (Exception exception)
            {
                Logger.ERROR(exception.StackTrace);
            }
        }

        public int AssembleFont(string panelName, string jsonFontTable)
        {
            Logger.DEBUG("panelName=" + panelName + " jsonFontTable=" + jsonFontTable);
            if (!this.panels.ContainsKey(panelName))
            {
                Logger.ERROR("panel " + panelName + " not found!");
                return -1;
            }
            string str = "GameFont";
            GameObject obj2 = this.panels[panelName];
            if (!this.fonts.ContainsKey(str.ToLower()))
            {
                Logger.ERROR("font " + str + " not found!");
                return -1;
            }
            Font font = this.fonts[str.ToLower()];
            foreach (Text text in obj2.GetComponentsInChildren<Text>(true))
            {
                Logger.DEBUG("Assemble " + str + " to Text");
                text.set_font(font);
            }
            return 0;
        }

        [DebuggerHidden]
        public IEnumerator CreatePanel(string panelName, Action<bool> onCreatePanel)
        {
            <CreatePanel>c__Iterator35 iterator = new <CreatePanel>c__Iterator35();
            iterator.panelName = panelName;
            iterator.onCreatePanel = onCreatePanel;
            iterator.<$>panelName = panelName;
            iterator.<$>onCreatePanel = onCreatePanel;
            iterator.<>f__this = this;
            return iterator;
        }

        public void DeleteFile(string fileName)
        {
            try
            {
                this.dictCacheFileInfo.Remove(fileName);
                string contents = Json.Serialize(this.dictCacheFileInfo);
                File.WriteAllText(this.cacheMetaFile, contents);
                File.Delete(Pandora.Instance.GetCachePath() + "/" + fileName);
            }
            catch (Exception exception)
            {
                Logger.ERROR(exception.StackTrace);
            }
        }

        public void DeleteRedundantFiles(List<string> dependencyAll)
        {
            Logger.DEBUG(string.Empty);
            try
            {
                List<string> list = new List<string>();
                foreach (KeyValuePair<string, object> pair in this.dictCacheFileInfo)
                {
                    if (!dependencyAll.Contains(pair.Key))
                    {
                        list.Add(pair.Key);
                    }
                }
                foreach (string str in list)
                {
                    Logger.DEBUG(str);
                    this.DeleteFile(str);
                }
                string cookiePath = Pandora.Instance.GetCookiePath();
                string imgPath = Pandora.Instance.GetImgPath();
                string[] files = Directory.GetFiles(cookiePath);
                string[] strArray2 = Directory.GetFiles(imgPath);
                foreach (string str4 in files)
                {
                    DateTime lastWriteTimeUtc = File.GetLastWriteTimeUtc(str4);
                    if (DateTime.UtcNow.Subtract(lastWriteTimeUtc).TotalDays > 7.0)
                    {
                        Logger.DEBUG(str4);
                        File.Delete(str4);
                    }
                }
                foreach (string str5 in strArray2)
                {
                    DateTime time2 = File.GetLastWriteTimeUtc(str5);
                    if (DateTime.UtcNow.Subtract(time2).TotalDays > 7.0)
                    {
                        Logger.DEBUG(str5);
                        File.Delete(str5);
                    }
                }
                foreach (string str7 in Directory.GetFiles(Pandora.Instance.GetTempPath()))
                {
                    DateTime time3 = File.GetLastWriteTimeUtc(str7);
                    if (DateTime.UtcNow.Subtract(time3).TotalDays > 3.0)
                    {
                        Logger.DEBUG(str7);
                        File.Delete(str7);
                    }
                }
                foreach (string str9 in Directory.GetFiles(Pandora.Instance.GetLogPath()))
                {
                    DateTime time4 = File.GetLastWriteTimeUtc(str9);
                    if (DateTime.UtcNow.Subtract(time4).TotalDays > 3.0)
                    {
                        Logger.DEBUG(str9);
                        File.Delete(str9);
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.ERROR(exception.StackTrace);
            }
        }

        public void DestroyPanel(string panelName)
        {
            Logger.DEBUG("panelName=" + panelName);
            if (this.panels.ContainsKey(panelName))
            {
                GameObject obj2 = this.panels[panelName];
                this.panels.Remove(panelName);
                if (obj2 != null)
                {
                    Object.Destroy(obj2);
                    obj2 = null;
                }
            }
            if (this.panelBundles.ContainsKey(panelName))
            {
                AssetBundle bundle = this.panelBundles[panelName];
                this.panelBundles.Remove(panelName);
                if (bundle != null)
                {
                    bundle.Unload(true);
                    bundle = null;
                }
            }
            Resources.UnloadUnusedAssets();
        }

        public List<GameObject> GetAllPanel()
        {
            List<GameObject> list = new List<GameObject>();
            foreach (KeyValuePair<string, GameObject> pair in this.panels)
            {
                list.Add(pair.Value);
            }
            return list;
        }

        public Font GetFont(string fontName)
        {
            if (this.fonts.ContainsKey(fontName))
            {
                return this.fonts[fontName];
            }
            return null;
        }

        public byte[] GetLuaBytes(string luaName)
        {
            Logger.DEBUG(luaName);
            string key = luaName.Replace(".lua", string.Empty);
            if (this.loadedLuaFiles.ContainsKey(key))
            {
                return this.loadedLuaFiles[key].get_bytes();
            }
            return null;
        }

        public string GetLuaString(string luaName)
        {
            Logger.DEBUG(luaName);
            string key = luaName.Replace(".lua", string.Empty);
            if (this.loadedLuaFiles.ContainsKey(key))
            {
                return this.loadedLuaFiles[key].get_text();
            }
            return null;
        }

        public GameObject GetPanel(string panelName)
        {
            if (this.panels.ContainsKey(panelName))
            {
                return this.panels[panelName];
            }
            return null;
        }

        private string GetPathForWWW(string assetBundleName)
        {
            try
            {
                if (this.dictCacheFileInfo.ContainsKey(assetBundleName))
                {
                    string str = Pandora.Instance.GetCachePath() + "/" + assetBundleName;
                    return ("file://" + str);
                }
                return (Pandora.Instance.GetStreamingAssetsPath() + "/" + assetBundleName);
            }
            catch (Exception exception)
            {
                Logger.ERROR(exception.StackTrace);
                return string.Empty;
            }
        }

        public void Init()
        {
            this.cacheMetaFile = Pandora.Instance.GetCachePath() + "/meta";
            this.LoadFonts();
            this.LoadCacheMeta();
        }

        public bool IsFileExistsInCache(string fileName, string fileMD5)
        {
            if (this.dictCacheFileInfo.ContainsKey(fileName))
            {
                Dictionary<string, object> dictionary = this.dictCacheFileInfo[fileName] as Dictionary<string, object>;
                if ((fileMD5.Length == 0) || ((dictionary["md5"] as string) == fileMD5))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsLuaAssetBundleLoaded(string fileName)
        {
            return this.loadedLuaAssetBundles.Contains(fileName);
        }

        private void LoadCacheMeta()
        {
            try
            {
                if (!File.Exists(this.cacheMetaFile))
                {
                    new FileStream(this.cacheMetaFile, FileMode.Create).Close();
                }
                string json = File.ReadAllText(this.cacheMetaFile);
                if (json != string.Empty)
                {
                    this.dictCacheFileInfo = Json.Deserialize(json) as Dictionary<string, object>;
                }
                Logger.DEBUG(json);
                if (this.dictCacheFileInfo == null)
                {
                    this.dictCacheFileInfo = new Dictionary<string, object>();
                }
            }
            catch (Exception exception)
            {
                Logger.ERROR(exception.StackTrace);
            }
        }

        private void LoadFonts()
        {
            Logger.DEBUG(string.Empty);
            string[] fontResources = Pandora.Instance.GetFontResources();
            for (int i = 0; i < fontResources.Length; i++)
            {
                GameObject obj2 = Resources.Load(fontResources[i]) as GameObject;
                if (obj2 != null)
                {
                    Font font = obj2.GetComponent<Text>().get_font();
                    if (font != null)
                    {
                        Logger.DEBUG(font.get_name() + " loaded!");
                        this.fonts[font.get_name().ToLower()] = font;
                    }
                    else
                    {
                        Logger.ERROR("Font of " + fontResources[i] + " is null");
                    }
                }
                else
                {
                    Logger.ERROR(fontResources[i] + " load error");
                }
            }
        }

        [DebuggerHidden]
        public IEnumerator LoadLuaAssetBundle(string assetBundleName, Action<bool> callback)
        {
            <LoadLuaAssetBundle>c__Iterator34 iterator = new <LoadLuaAssetBundle>c__Iterator34();
            iterator.assetBundleName = assetBundleName;
            iterator.callback = callback;
            iterator.<$>assetBundleName = assetBundleName;
            iterator.<$>callback = callback;
            iterator.<>f__this = this;
            return iterator;
        }

        public void Logout()
        {
            Logger.DEBUG(string.Empty);
            this.fonts.Clear();
            foreach (KeyValuePair<string, GameObject> pair in this.panels)
            {
                Logger.DEBUG(pair.Key);
                GameObject obj2 = pair.Value;
                if (obj2 != null)
                {
                    Object.Destroy(obj2);
                }
            }
            this.panels.Clear();
            foreach (KeyValuePair<string, AssetBundle> pair2 in this.panelBundles)
            {
                Logger.DEBUG(pair2.Key);
                AssetBundle bundle = pair2.Value;
                if (bundle != null)
                {
                    bundle.Unload(true);
                }
            }
            this.panelBundles.Clear();
            this.loadedLuaAssetBundles.Clear();
            this.loadedLuaFiles.Clear();
            this.dictCacheFileInfo.Clear();
        }

        [CompilerGenerated]
        private sealed class <CreatePanel>c__Iterator35 : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal Action<bool> <$>onCreatePanel;
            internal string <$>panelName;
            internal ResourceMgr <>f__this;
            internal string <assetBundleName>__0;
            internal AssetBundle <bundle>__3;
            internal GameObject <panel>__5;
            internal GameObject <panelParent>__6;
            internal GameObject <panelPrefab>__4;
            internal WWW <www>__2;
            internal string <wwwPath>__1;
            internal Action<bool> onCreatePanel;
            internal string panelName;

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
                        Logger.DEBUG(this.panelName);
                        if (!this.<>f__this.creatingPanels.Contains(this.panelName))
                        {
                            if (this.<>f__this.panels.ContainsKey(this.panelName))
                            {
                                Logger.DEBUG(string.Empty);
                                this.onCreatePanel(true);
                                goto Label_0337;
                            }
                            this.<>f__this.creatingPanels.Add(this.panelName);
                            this.<assetBundleName>__0 = Utils.GetBundleName(this.panelName);
                            this.<wwwPath>__1 = this.<>f__this.GetPathForWWW(this.<assetBundleName>__0);
                            Logger.DEBUG(this.<wwwPath>__1);
                            this.<www>__2 = new WWW(this.<wwwPath>__1);
                            this.$current = this.<www>__2;
                            this.$PC = 1;
                            goto Label_0339;
                        }
                        Logger.DEBUG(string.Empty);
                        this.onCreatePanel(false);
                        goto Label_0337;

                    case 1:
                        Logger.DEBUG(this.<wwwPath>__1);
                        if (!string.IsNullOrEmpty(this.<www>__2.get_error()))
                        {
                            break;
                        }
                        Logger.DEBUG(string.Empty);
                        this.<bundle>__3 = this.<www>__2.get_assetBundle();
                        if (this.<bundle>__3 == null)
                        {
                            break;
                        }
                        Logger.DEBUG(string.Empty);
                        this.<>f__this.panelBundles[this.panelName] = this.<bundle>__3;
                        this.<panelPrefab>__4 = this.<bundle>__3.Load(this.panelName, typeof(GameObject)) as GameObject;
                        this.$current = new WaitForEndOfFrame();
                        this.$PC = 2;
                        goto Label_0339;

                    case 2:
                        Logger.DEBUG(this.panelName);
                        if (this.<panelPrefab>__4 == null)
                        {
                            break;
                        }
                        Logger.DEBUG(string.Empty);
                        this.<panel>__5 = Object.Instantiate(this.<panelPrefab>__4) as GameObject;
                        if (this.<panel>__5 == null)
                        {
                            break;
                        }
                        Logger.DEBUG(string.Empty);
                        this.<panelParent>__6 = Pandora.Instance.GetPanelParent();
                        if (this.<panelParent>__6 != null)
                        {
                            this.<panel>__5.get_transform().set_parent(this.<panelParent>__6.get_transform());
                        }
                        this.<panel>__5.set_name(this.panelName);
                        this.<panel>__5.get_transform().set_localScale(Vector3.get_one());
                        this.<panel>__5.get_transform().set_localPosition(Vector3.get_zero());
                        this.<panel>__5.AddComponent<ImageMgr>();
                        this.<>f__this.panels[this.panelName] = this.<panel>__5;
                        this.$current = new WaitForEndOfFrame();
                        this.$PC = 3;
                        goto Label_0339;

                    case 3:
                        Logger.DEBUG(this.panelName);
                        this.<>f__this.creatingPanels.Remove(this.panelName);
                        this.onCreatePanel(true);
                        goto Label_0337;

                    default:
                        goto Label_0337;
                }
                Logger.ERROR(this.panelName);
                this.<>f__this.creatingPanels.Remove(this.panelName);
                this.onCreatePanel(false);
                this.$PC = -1;
            Label_0337:
                return false;
            Label_0339:
                return true;
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
        private sealed class <LoadLuaAssetBundle>c__Iterator34 : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal string <$>assetBundleName;
            internal Action<bool> <$>callback;
            internal ResourceMgr <>f__this;
            internal AssetBundle <bundle>__2;
            internal Object[] <files>__3;
            internal int <k>__4;
            internal string <key>__6;
            internal TextAsset <textAsset>__5;
            internal WWW <www>__1;
            internal string <wwwPath>__0;
            internal string assetBundleName;
            internal Action<bool> callback;

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
                        Logger.DEBUG(this.assetBundleName);
                        if (!this.<>f__this.loadedLuaAssetBundles.Contains(this.assetBundleName))
                        {
                            this.<wwwPath>__0 = this.<>f__this.GetPathForWWW(this.assetBundleName);
                            Logger.DEBUG(this.<wwwPath>__0);
                            this.<www>__1 = new WWW(this.<wwwPath>__0);
                            this.$current = this.<www>__1;
                            this.$PC = 1;
                            return true;
                        }
                        Logger.DEBUG(string.Empty);
                        this.callback(true);
                        goto Label_01D6;

                    case 1:
                        Logger.DEBUG(this.<wwwPath>__0);
                        if (!string.IsNullOrEmpty(this.<www>__1.get_error()))
                        {
                            Logger.ERROR(string.Empty);
                            this.callback(false);
                            break;
                        }
                        Logger.DEBUG(string.Empty);
                        this.<>f__this.loadedLuaAssetBundles.Add(this.assetBundleName);
                        this.<bundle>__2 = this.<www>__1.get_assetBundle();
                        this.<files>__3 = this.<bundle>__2.LoadAll();
                        this.<k>__4 = 0;
                        while (this.<k>__4 < this.<files>__3.Length)
                        {
                            this.<textAsset>__5 = this.<files>__3[this.<k>__4] as TextAsset;
                            this.<key>__6 = this.<textAsset>__5.get_name().Replace(".lua", string.Empty);
                            Logger.DEBUG(this.<key>__6);
                            this.<>f__this.loadedLuaFiles[this.<key>__6] = this.<textAsset>__5;
                            this.<k>__4++;
                        }
                        this.<bundle>__2.Unload(false);
                        this.callback(true);
                        break;

                    default:
                        goto Label_01D6;
                }
                this.$PC = -1;
            Label_01D6:
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
    }
}

