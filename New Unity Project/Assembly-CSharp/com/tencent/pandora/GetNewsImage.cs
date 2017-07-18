namespace com.tencent.pandora
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class GetNewsImage : MonoBehaviour
    {
        public static Dictionary<string, bool> dicFailImgs = new Dictionary<string, bool>();
        public static Dictionary<string, bool> dicSuccImgs = new Dictionary<string, bool>();
        public bool isLoading;
        public int iSourceImgHeight;
        public int iSourceImgWidth;
        public bool isResize;
        private int iTryNum;
        public List<string> ListPreToLoading = new List<string>();
        public int m_nDownLoadNumber;
        private string mPreFix = string.Empty;
        private string path = FileUtils.GetPersistentFilePath();
        public Dictionary<string, Texture> picDic = new Dictionary<string, Texture>();
        private string strCallCurLuaFile = string.Empty;
        public float waitTimeOut = 3f;

        public event callbackFuc OnFailCallBack;

        public event Callback OnFailCallBackWithTex;

        public event callbackFuc OnSuccCallBack;

        public event Callback OnSuccCallBackWithTex;

        private void addNumber()
        {
            this.m_nDownLoadNumber++;
        }

        private void Awake()
        {
            try
            {
                if (!Directory.Exists(Application.get_temporaryCachePath() + "/TPlayCache/"))
                {
                    Directory.CreateDirectory(Application.get_temporaryCachePath() + "/TPlayCache/");
                }
            }
            catch (Exception exception)
            {
                Logger.d(exception.ToString());
            }
        }

        public void BreakCoroutine()
        {
            base.StopAllCoroutines();
        }

        public void CacheImage(string url)
        {
            if (!this.isImageCached(url))
            {
                base.StartCoroutine(this.DownloadCacheImage(url));
            }
            else
            {
                dicSuccImgs.Add(url, true);
            }
        }

        private void CallLuaFailInfo(string strUrl)
        {
            if (this.strCallCurLuaFile != string.Empty)
            {
                try
                {
                    object[] args = new object[] { strUrl };
                    Util.CallMethod(this.strCallCurLuaFile, "OnPicGetFail", args);
                }
                catch (Exception exception)
                {
                    Logger.d("CallLuaFailInfo:" + exception.Message);
                }
            }
        }

        private void CallLuaFinshInfo(string strUrl)
        {
            if (this.strCallCurLuaFile != string.Empty)
            {
                try
                {
                    object[] args = new object[] { strUrl };
                    Util.CallMethod(this.strCallCurLuaFile, "OnPicGetFish", args);
                }
                catch (Exception exception)
                {
                    Logger.d("CallLuaFailInfo:" + exception.Message);
                }
            }
        }

        private bool checkIsJpg(string strUrl)
        {
            char[] separator = new char[] { '.' };
            string[] strArray = strUrl.Split(separator);
            return ((strArray.Length > 0) && (strArray[strArray.Length - 1].ToLower() == "jpg"));
        }

        [DebuggerHidden]
        private IEnumerator DownloadCacheImage(string url)
        {
            <DownloadCacheImage>c__Iterator4 iterator = new <DownloadCacheImage>c__Iterator4();
            iterator.url = url;
            iterator.<$>url = url;
            iterator.<>f__this = this;
            return iterator;
        }

        [DebuggerHidden]
        private IEnumerator DownloadImage(string url, Image texture)
        {
            <DownloadImage>c__Iterator2 iterator = new <DownloadImage>c__Iterator2();
            iterator.url = url;
            iterator.texture = texture;
            iterator.<$>url = url;
            iterator.<$>texture = texture;
            iterator.<>f__this = this;
            return iterator;
        }

        public int downloadTotalCnt()
        {
            return this.m_nDownLoadNumber;
        }

        public string getImageCacheFile(string url)
        {
            if (this.path == string.Empty)
            {
                this.path = FileUtils.GetPersistentFilePath();
            }
            string str = string.Empty;
            if ((url != null) && (url != string.Empty))
            {
                str = url.GetHashCode().ToString();
            }
            return (this.path + this.mPreFix + "_" + str);
        }

        public bool isImageCached(string url)
        {
            if (!File.Exists(this.getImageCacheFile(url)))
            {
                return false;
            }
            if (new FileInfo(this.getImageCacheFile(url)).Length < 150L)
            {
                return false;
            }
            return true;
        }

        public static int IsImgDownSucc(string strImgUrl)
        {
            if (dicSuccImgs.ContainsKey(strImgUrl))
            {
                return 0;
            }
            if (dicFailImgs.ContainsKey(strImgUrl))
            {
                return -1;
            }
            return -2;
        }

        [DebuggerHidden]
        private IEnumerator LoadLocalImage(string url, Image texture)
        {
            <LoadLocalImage>c__Iterator3 iterator = new <LoadLocalImage>c__Iterator3();
            iterator.url = url;
            iterator.texture = texture;
            iterator.<$>url = url;
            iterator.<$>texture = texture;
            iterator.<>f__this = this;
            return iterator;
        }

        private void OnDestroy()
        {
            if (this.picDic != null)
            {
                Logger.d("内存中存在" + this.picDic.Count + "张图片");
                if (this.picDic.Count > 0)
                {
                    foreach (Texture texture in this.picDic.Values)
                    {
                        Resources.UnloadAsset(texture);
                    }
                }
                this.picDic.Clear();
                Logger.d("图片已清理");
            }
            Logger.d("Img OnDestroy");
        }

        public void PreDownImg()
        {
            this.iTryNum++;
            if (this.ListPreToLoading.Count != 0)
            {
                string item = this.ListPreToLoading[0];
                this.ListPreToLoading.Remove(item);
                if (this.isImageCached(item))
                {
                    if (!dicSuccImgs.ContainsKey(item))
                    {
                        dicSuccImgs.Add(item, true);
                    }
                    this.PreDownImg();
                }
                else
                {
                    base.StartCoroutine(this.DownloadCacheImage(item));
                }
            }
        }

        public void resetnumber()
        {
            this.m_nDownLoadNumber = 0;
        }

        public void SetAsyncImageAll(string url, Image tex, string nowFileName)
        {
            this.isResize = false;
            this.SetLuaFileName(nowFileName);
            if (!this.isImageCached(url))
            {
                base.StartCoroutine(this.DownloadImage(url, tex));
            }
            else
            {
                base.StartCoroutine(this.LoadLocalImage(url, tex));
            }
        }

        public void SetLuaFileName(string strFileName)
        {
            this.strCallCurLuaFile = strFileName;
        }

        public void setPreFix(int iCatchType)
        {
        }

        [CompilerGenerated]
        private sealed class <DownloadCacheImage>c__Iterator4 : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal string <$>url;
            internal GetNewsImage <>f__this;
            internal AssetBundle <ab>__5;
            internal Exception <ex>__3;
            internal Exception <ex2>__4;
            internal Texture2D <image>__1;
            internal byte[] <pngData>__2;
            internal WWW <www>__0;
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
                        this.<www>__0 = new WWW(this.url);
                        this.$current = this.<www>__0;
                        this.$PC = 1;
                        return true;

                    case 1:
                        this.<>f__this.isLoading = false;
                        if (this.<www>__0.get_error() != null)
                        {
                            if (!GetNewsImage.dicFailImgs.ContainsKey(this.url))
                            {
                                GetNewsImage.dicFailImgs.Add(this.url, false);
                            }
                            Logger.e("cache error:" + this.<www>__0.get_error() + "," + this.url);
                            break;
                        }
                        this.<image>__1 = this.<www>__0.get_texture();
                        try
                        {
                            this.<pngData>__2 = null;
                            if (this.<>f__this.checkIsJpg(this.url))
                            {
                                this.<pngData>__2 = this.<image>__1.EncodeToJPG();
                            }
                            else
                            {
                                this.<pngData>__2 = this.<image>__1.EncodeToPNG();
                            }
                            File.WriteAllBytes(this.<>f__this.getImageCacheFile(this.url), this.<pngData>__2);
                            if (!GetNewsImage.dicSuccImgs.ContainsKey(this.url))
                            {
                                GetNewsImage.dicSuccImgs.Add(this.url, true);
                            }
                        }
                        catch (Exception exception)
                        {
                            this.<ex>__3 = exception;
                            if (!GetNewsImage.dicFailImgs.ContainsKey(this.url))
                            {
                                GetNewsImage.dicFailImgs.Add(this.url, false);
                            }
                            Logger.d(this.<ex>__3.ToString());
                            try
                            {
                                File.Delete(this.<>f__this.getImageCacheFile(this.url));
                            }
                            catch (Exception exception2)
                            {
                                this.<ex2>__4 = exception2;
                                Logger.d(this.<ex2>__4.ToString());
                            }
                        }
                        this.<ab>__5 = this.<www>__0.get_assetBundle();
                        this.<www>__0.Dispose();
                        if (this.<ab>__5 != null)
                        {
                            this.<ab>__5.Unload(true);
                        }
                        this.<image>__1 = null;
                        this.<www>__0 = null;
                        break;

                    default:
                        goto Label_0226;
                }
                this.<>f__this.PreDownImg();
                this.$PC = -1;
            Label_0226:
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
        private sealed class <DownloadImage>c__Iterator2 : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal Image <$>texture;
            internal string <$>url;
            internal GetNewsImage <>f__this;
            internal AssetBundle <ab>__4;
            internal Exception <ex>__2;
            internal Exception <ex2>__3;
            internal byte[] <pngData>__1;
            internal WWW <www>__0;
            internal Image texture;
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
                        this.<www>__0 = new WWW(this.url);
                        this.$current = this.<www>__0;
                        this.$PC = 1;
                        return true;

                    case 1:
                        this.<>f__this.isLoading = false;
                        if (this.<www>__0.get_error() != null)
                        {
                            this.<>f__this.addNumber();
                            this.<>f__this.CallLuaFailInfo(this.url);
                            Logger.e("down fail:" + this.<www>__0.get_error() + "," + this.url);
                            break;
                        }
                        this.texture.get_rectTransform().set_sizeDelta(new Vector2((float) this.<www>__0.get_texture().get_width(), (float) this.<www>__0.get_texture().get_height()));
                        this.texture.set_sprite(Sprite.Create(this.<www>__0.get_texture(), new Rect(0f, 0f, (float) this.<www>__0.get_texture().get_width(), (float) this.<www>__0.get_texture().get_height()), new Vector2(0f, 0f)));
                        Logger.d("down load ok............");
                        try
                        {
                            this.<pngData>__1 = null;
                            if (this.<>f__this.checkIsJpg(this.url))
                            {
                            }
                            this.<pngData>__1 = this.<www>__0.get_bytes();
                            File.WriteAllBytes(this.<>f__this.getImageCacheFile(this.url), this.<pngData>__1);
                        }
                        catch (Exception exception)
                        {
                            this.<ex>__2 = exception;
                            Logger.d("save img data to cache failed :" + this.<ex>__2.ToString());
                            try
                            {
                                File.Delete(this.<>f__this.getImageCacheFile(this.url));
                            }
                            catch (Exception exception2)
                            {
                                this.<ex2>__3 = exception2;
                                Logger.d(this.<ex2>__3.ToString());
                            }
                        }
                        this.<ab>__4 = this.<www>__0.get_assetBundle();
                        this.<www>__0.Dispose();
                        if (this.<ab>__4 != null)
                        {
                            this.<ab>__4.Unload(true);
                        }
                        this.<www>__0 = null;
                        this.<>f__this.CallLuaFinshInfo(this.url);
                        break;

                    default:
                        goto Label_0253;
                }
                this.$PC = -1;
            Label_0253:
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
        private sealed class <LoadLocalImage>c__Iterator3 : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal Image <$>texture;
            internal string <$>url;
            internal GetNewsImage <>f__this;
            internal AssetBundle <ab>__2;
            internal string <filePath>__0;
            internal WWW <www>__1;
            internal Image texture;
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
                        this.<filePath>__0 = "file://" + this.<>f__this.getImageCacheFile(this.url);
                        Logger.d("getting local image:" + this.<filePath>__0);
                        this.<www>__1 = new WWW(this.<filePath>__0);
                        this.$current = this.<www>__1;
                        this.$PC = 1;
                        return true;

                    case 1:
                        this.<>f__this.isLoading = false;
                        this.texture.get_rectTransform().set_sizeDelta(new Vector2((float) this.<www>__1.get_texture().get_width(), (float) this.<www>__1.get_texture().get_height()));
                        this.texture.set_sprite(Sprite.Create(this.<www>__1.get_texture(), new Rect(0f, 0f, (float) this.<www>__1.get_texture().get_width(), (float) this.<www>__1.get_texture().get_height()), new Vector2(0f, 0f)));
                        this.<ab>__2 = this.<www>__1.get_assetBundle();
                        this.<www>__1.Dispose();
                        if (this.<ab>__2 != null)
                        {
                            this.<ab>__2.Unload(true);
                        }
                        this.<www>__1 = null;
                        if (this.<>f__this.OnSuccCallBackWithTex != null)
                        {
                        }
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

        public delegate void Callback(string strImgUrl, Texture2D goTex);

        public delegate void callbackFuc(string strImgUrl, GameObject goTex);
    }
}

