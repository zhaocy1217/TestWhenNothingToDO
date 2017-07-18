namespace Assets.Scripts.GameLogic
{
    using AGE;
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.DataCenter;
    using Assets.Scripts.GameLogic.GameKernal;
    using Assets.Scripts.GameSystem;
    using Assets.Scripts.Sound;
    using Assets.Scripts.UI;
    using behaviac;
    using ResData;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class GameLoader : MonoSingleton<GameLoader>
    {
        private int _nProgress;
        public List<ActorMeta> actorList = new List<ActorMeta>();
        private List<ActorPreloadTab> actorPreload;
        private List<ActorPreloadTab> ageRefAssetsList;
        private float coroutineTime;
        public bool isLoadStart;
        private ArrayList levelArtistList = new ArrayList();
        private ArrayList levelDesignList = new ArrayList();
        private ArrayList levelList = new ArrayList();
        private LoadCompleteDelegate LoadCompleteEvent;
        private LoadProgressDelegate LoadProgressEvent;
        private CCoroutine m_handle_AnalyseResPreload;
        private CCoroutine m_handle_CoroutineLoad;
        private CCoroutine m_handle_LoadActorAssets;
        private CCoroutine m_handle_LoadAgeRecursiveAssets;
        private CCoroutine m_handle_LoadArtistLevel;
        private CCoroutine m_handle_LoadCommonAssetBundle;
        private CCoroutine m_handle_LoadCommonAssets;
        private CCoroutine m_handle_LoadCommonEffect;
        private CCoroutine m_handle_LoadDesignLevel;
        private CCoroutine m_handle_LoadNoActorAssets;
        private CCoroutine m_handle_PreSpawnSoldiers;
        private CCoroutine m_handle_SpawnDynamicActor;
        private CCoroutine m_handle_SpawnStaticActor;
        private ActorPreloadTab noActorPreLoad;
        private static GameSerializer s_serializer = new GameSerializer();
        private static Dictionary<string, string> s_vertexShaderMap;
        private List<string> soundBankList = new List<string>();
        public ListView<ActorConfig> staticActors = new ListView<ActorConfig>();

        static GameLoader()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("S_Game_Scene/Cloth_Lightmap_Wind", "S_Game_Scene/Light_VertexLit/Cloth_Lightmap_Wind");
            dictionary.Add("S_Game_Scene/Cloth_Wind", "S_Game_Scene/Light_VertexLit/Cloth_Wind");
            dictionary.Add("S_Game_Effects/Scroll2TexBend_LightMap", "S_Game_Effects/Light_VertexLit/Scroll2TexBend_LightMap");
            dictionary.Add("S_Game_Effects/Scroll2TexBend", "S_Game_Effects/Light_VertexLit/Scroll2TexBend");
            dictionary.Add("S_Game_Scene/Diffuse_NotFog", "S_Game_Scene/Light_VertexLit/Diffuse_NotFog");
            s_vertexShaderMap = dictionary;
        }

        public void AddActor(ref ActorMeta actorMeta)
        {
            this.actorList.Add(actorMeta);
        }

        public void AddArtistSerializedLevel(string name)
        {
            this.levelArtistList.Add(name);
        }

        public void AddDesignSerializedLevel(string name)
        {
            this.levelDesignList.Add(name);
        }

        public void AddLevel(string name)
        {
            this.levelList.Add(name);
        }

        public void AddSoundBank(string name)
        {
            this.soundBankList.Add(name);
        }

        public void AddStaticActor(ActorConfig actor)
        {
            this.staticActors.Add(actor);
        }

        public void AdvanceStopLoad()
        {
            if (this.isLoadStart)
            {
                Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("ActorInfo");
                Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharIcon");
                Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharBattle");
                Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharShow");
                Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharLoading");
                Singleton<CResourceManager>.GetInstance().UnloadUnusedAssets();
                GC.Collect();
                Singleton<EventRouter>.instance.BroadCastEvent(EventID.ADVANCE_STOP_LOADING);
            }
            this.ResetLoader();
        }

        [DebuggerHidden]
        private IEnumerator AnalyseActorAssets(LoaderHelper loadHelper)
        {
            <AnalyseActorAssets>c__Iterator1A iteratora = new <AnalyseActorAssets>c__Iterator1A();
            iteratora.loadHelper = loadHelper;
            iteratora.<$>loadHelper = loadHelper;
            iteratora.<>f__this = this;
            return iteratora;
        }

        [DebuggerHidden]
        private IEnumerator AnalyseAgeRecursiveAssets(LoaderHelper loadHelper)
        {
            <AnalyseAgeRecursiveAssets>c__Iterator1E iteratore = new <AnalyseAgeRecursiveAssets>c__Iterator1E();
            iteratore.loadHelper = loadHelper;
            iteratore.<$>loadHelper = loadHelper;
            iteratore.<>f__this = this;
            return iteratore;
        }

        [DebuggerHidden]
        private IEnumerator AnalyseNoActorAssets(LoaderHelper loadHelper)
        {
            <AnalyseNoActorAssets>c__Iterator1C iteratorc = new <AnalyseNoActorAssets>c__Iterator1C();
            iteratorc.loadHelper = loadHelper;
            iteratorc.<$>loadHelper = loadHelper;
            iteratorc.<>f__this = this;
            return iteratorc;
        }

        [DebuggerHidden]
        private IEnumerator AnalyseResourcePreload(LoaderHelper loadHelper)
        {
            <AnalyseResourcePreload>c__Iterator19 iterator = new <AnalyseResourcePreload>c__Iterator19();
            iterator.loadHelper = loadHelper;
            iterator.<$>loadHelper = loadHelper;
            iterator.<>f__this = this;
            return iterator;
        }

        private static void ChangeToVertexLit()
        {
            foreach (Renderer renderer in GameObject.Find("Artist").GetComponentsInChildren<Renderer>())
            {
                if ((null != renderer) && (renderer.get_sharedMaterials() != null))
                {
                    for (int i = 0; i < renderer.get_sharedMaterials().Length; i++)
                    {
                        if ((null != renderer.get_sharedMaterials()[i]) && (null != renderer.get_sharedMaterials()[i].get_shader()))
                        {
                            string str = ChangeVertexShader(renderer.get_sharedMaterials()[i].get_shader().get_name());
                            renderer.get_sharedMaterials()[i].set_shader(Shader.Find(str));
                        }
                    }
                }
            }
        }

        private static string ChangeVertexShader(string oldShader)
        {
            if (s_vertexShaderMap.ContainsKey(oldShader))
            {
                return s_vertexShaderMap[oldShader];
            }
            if (oldShader.Contains("S_Game_Scene/Light/"))
            {
                return oldShader.Replace("S_Game_Scene/Light/", "S_Game_Scene/Light_VertexLit/");
            }
            return oldShader;
        }

        private static List<List<string>> composite(string[] input)
        {
            List<List<string>> list = new List<List<string>>();
            if (input.Length > 8)
            {
                throw new Exception("only support less than 8 words");
            }
            for (byte i = 0; i < Mathf.Pow(2f, (float) input.Length); i = (byte) (i + 1))
            {
                List<string> item = new List<string>();
                for (int j = 0; j < input.Length; j++)
                {
                    if (((i >> j) & 1) > 0)
                    {
                        item.Add(input[j]);
                    }
                }
                list.Add(item);
            }
            return list;
        }

        [DebuggerHidden]
        private IEnumerator CoroutineLoad()
        {
            <CoroutineLoad>c__Iterator23 iterator = new <CoroutineLoad>c__Iterator23();
            iterator.<>f__this = this;
            return iterator;
        }

        public void Load(LoadProgressDelegate progress, LoadCompleteDelegate finish)
        {
            if (!this.isLoadStart)
            {
                Debug.Log("GameLoader Start Load");
                this.LoadProgressEvent = progress;
                this.LoadCompleteEvent = finish;
                this._nProgress = 0;
                this.isLoadStart = true;
                this.m_handle_CoroutineLoad = Singleton<CCoroutineManager>.GetInstance().StartCoroutine(this.CoroutineLoad());
            }
        }

        [DebuggerHidden]
        private IEnumerator LoadActorAssets(LHCWrapper InWrapper)
        {
            <LoadActorAssets>c__Iterator1B iteratorb = new <LoadActorAssets>c__Iterator1B();
            iteratorb.InWrapper = InWrapper;
            iteratorb.<$>InWrapper = InWrapper;
            iteratorb.<>f__this = this;
            return iteratorb;
        }

        [DebuggerHidden]
        private IEnumerator LoadAgeRecursiveAssets(LHCWrapper InWrapper)
        {
            <LoadAgeRecursiveAssets>c__Iterator1F iteratorf = new <LoadAgeRecursiveAssets>c__Iterator1F();
            iteratorf.InWrapper = InWrapper;
            iteratorf.<$>InWrapper = InWrapper;
            iteratorf.<>f__this = this;
            return iteratorf;
        }

        [DebuggerHidden]
        private IEnumerator LoadArtistLevel(LoaderHelperWrapper InWrapper)
        {
            <LoadArtistLevel>c__Iterator16 iterator = new <LoadArtistLevel>c__Iterator16();
            iterator.InWrapper = InWrapper;
            iterator.<$>InWrapper = InWrapper;
            iterator.<>f__this = this;
            return iterator;
        }

        [DebuggerHidden]
        private IEnumerator LoadCommonAssetBundle(LoaderHelperWrapper InWrapper)
        {
            <LoadCommonAssetBundle>c__Iterator14 iterator = new <LoadCommonAssetBundle>c__Iterator14();
            iterator.InWrapper = InWrapper;
            iterator.<$>InWrapper = InWrapper;
            iterator.<>f__this = this;
            return iterator;
        }

        [DebuggerHidden]
        private IEnumerator LoadCommonAssets(LoaderHelperWrapper InWrapper)
        {
            <LoadCommonAssets>c__Iterator18 iterator = new <LoadCommonAssets>c__Iterator18();
            iterator.InWrapper = InWrapper;
            iterator.<$>InWrapper = InWrapper;
            iterator.<>f__this = this;
            return iterator;
        }

        [DebuggerHidden]
        private IEnumerator LoadCommonEffect(LoaderHelperWrapper InWrapper)
        {
            <LoadCommonEffect>c__Iterator15 iterator = new <LoadCommonEffect>c__Iterator15();
            iterator.InWrapper = InWrapper;
            iterator.<$>InWrapper = InWrapper;
            iterator.<>f__this = this;
            return iterator;
        }

        [DebuggerHidden]
        private IEnumerator LoadDesignLevel(LoaderHelperWrapper InWrapper)
        {
            <LoadDesignLevel>c__Iterator17 iterator = new <LoadDesignLevel>c__Iterator17();
            iterator.InWrapper = InWrapper;
            iterator.<$>InWrapper = InWrapper;
            iterator.<>f__this = this;
            return iterator;
        }

        [DebuggerHidden]
        private IEnumerator LoadNoActorAssets(LHCWrapper InWrapper)
        {
            <LoadNoActorAssets>c__Iterator1D iteratord = new <LoadNoActorAssets>c__Iterator1D();
            iteratord.InWrapper = InWrapper;
            iteratord.<$>InWrapper = InWrapper;
            iteratord.<>f__this = this;
            return iteratord;
        }

        [DebuggerHidden]
        private IEnumerator PreSpawnSoldiers(LoaderHelperWrapper InWrapper)
        {
            <PreSpawnSoldiers>c__Iterator22 iterator = new <PreSpawnSoldiers>c__Iterator22();
            iterator.InWrapper = InWrapper;
            iterator.<$>InWrapper = InWrapper;
            iterator.<>f__this = this;
            return iterator;
        }

        private void ReleaseMemoryIfNeed()
        {
            if (DeviceCheckSys.GetAvailMemoryMegaBytes() <= 100)
            {
                Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("ActorInfo");
                Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharIcon");
                Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharBattle");
                Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharShow");
                Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharLoading");
                Resources.UnloadUnusedAssets();
                GC.Collect();
            }
        }

        public void ResetLoader()
        {
            this.levelList.Clear();
            this.actorList.Clear();
            this.levelDesignList.Clear();
            this.levelArtistList.Clear();
            this.soundBankList.Clear();
            this.staticActors.Clear();
            this._nProgress = 0;
            if (this.isLoadStart)
            {
                Singleton<CCoroutineManager>.GetInstance().StopCoroutine(this.m_handle_PreSpawnSoldiers, true);
                Singleton<CCoroutineManager>.GetInstance().StopCoroutine(this.m_handle_SpawnDynamicActor, true);
                Singleton<CCoroutineManager>.GetInstance().StopCoroutine(this.m_handle_SpawnStaticActor, true);
                Singleton<CCoroutineManager>.GetInstance().StopCoroutine(this.m_handle_LoadAgeRecursiveAssets, true);
                Singleton<CCoroutineManager>.GetInstance().StopCoroutine(this.m_handle_LoadNoActorAssets, true);
                Singleton<CCoroutineManager>.GetInstance().StopCoroutine(this.m_handle_LoadActorAssets, true);
                Singleton<CCoroutineManager>.GetInstance().StopCoroutine(this.m_handle_LoadCommonAssets, true);
                Singleton<CCoroutineManager>.GetInstance().StopCoroutine(this.m_handle_LoadDesignLevel, true);
                Singleton<CCoroutineManager>.GetInstance().StopCoroutine(this.m_handle_LoadArtistLevel, true);
                Singleton<CCoroutineManager>.GetInstance().StopCoroutine(this.m_handle_LoadCommonAssetBundle, true);
                Singleton<CCoroutineManager>.GetInstance().StopCoroutine(this.m_handle_LoadCommonEffect, true);
                Singleton<CCoroutineManager>.GetInstance().StopCoroutine(this.m_handle_CoroutineLoad, true);
                Singleton<CCoroutineManager>.GetInstance().StopCoroutine(this.m_handle_AnalyseResPreload, true);
                this.isLoadStart = false;
            }
        }

        private bool ShouldYieldReturn()
        {
            return ((Time.get_realtimeSinceStartup() - this.coroutineTime) > 0.08f);
        }

        [DebuggerHidden]
        private IEnumerator SpawnDynamicActor(LoaderHelperWrapper InWrapper)
        {
            <SpawnDynamicActor>c__Iterator21 iterator = new <SpawnDynamicActor>c__Iterator21();
            iterator.InWrapper = InWrapper;
            iterator.<$>InWrapper = InWrapper;
            iterator.<>f__this = this;
            return iterator;
        }

        [DebuggerHidden]
        private IEnumerator SpawnStaticActor(LoaderHelperWrapper InWrapper)
        {
            <SpawnStaticActor>c__Iterator20 iterator = new <SpawnStaticActor>c__Iterator20();
            iterator.InWrapper = InWrapper;
            iterator.<$>InWrapper = InWrapper;
            iterator.<>f__this = this;
            return iterator;
        }

        private void UpdateProgress(LoaderHelperCamera lhc, int oldProgress, int duty, int index, int count)
        {
            this.coroutineTime = Time.get_realtimeSinceStartup();
            this.nProgress = oldProgress + ((duty * index) / count);
            this.LoadProgressEvent(this.nProgress * 0.0001f);
            if (lhc != null)
            {
                lhc.Update();
            }
        }

        public int nProgress
        {
            get
            {
                return this._nProgress;
            }
            set
            {
                if (value >= this._nProgress)
                {
                    this._nProgress = value;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <AnalyseActorAssets>c__Iterator1A : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal LoaderHelper <$>loadHelper;
            internal GameLoader <>f__this;
            internal LoaderHelper loadHelper;

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
                        this.<>f__this.actorPreload = this.loadHelper.GetActorPreload();
                        this.$current = new CHoldForSecond(0f);
                        this.$PC = 1;
                        return true;

                    case 1:
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
        private sealed class <AnalyseAgeRecursiveAssets>c__Iterator1E : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal LoaderHelper <$>loadHelper;
            internal GameLoader <>f__this;
            internal Action <action>__15;
            internal Action <action>__6;
            internal Action <action>__7;
            internal int <configID>__14;
            internal int <configID>__4;
            internal int <i>__0;
            internal int <idx>__11;
            internal int <j>__1;
            internal ActorPreloadTab <loadInfo>__2;
            internal int <markID>__13;
            internal int <markID>__3;
            internal int <numPasses>__9;
            internal Dictionary<object, AssetRefType> <refAssets>__16;
            internal Dictionary<object, AssetRefType> <refAssets>__5;
            internal Dictionary<object, AssetRefType> <refAssets>__8;
            internal ActorPreloadTab <restAssets>__12;
            internal List<ActorPreloadTab> <restAssetsList>__10;
            internal LoaderHelper loadHelper;

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
                        if (this.<>f__this.ageRefAssetsList == null)
                        {
                            this.<>f__this.ageRefAssetsList = new List<ActorPreloadTab>();
                        }
                        this.<>f__this.ageRefAssetsList.Clear();
                        this.<i>__0 = 0;
                        this.<j>__1 = 0;
                        this.<i>__0 = 0;
                        while (this.<i>__0 < this.<>f__this.actorPreload.Count)
                        {
                            this.<loadInfo>__2 = this.<>f__this.actorPreload[this.<i>__0];
                            this.<markID>__3 = this.<loadInfo>__2.MarkID;
                            this.<configID>__4 = this.<loadInfo>__2.theActor.ConfigId;
                            this.<refAssets>__5 = this.loadHelper.GetRefAssets(this.<markID>__3, this.<configID>__4);
                            this.<j>__1 = 0;
                            while (this.<j>__1 < this.<loadInfo>__2.ageActions.Count)
                            {
                                AssetLoadBase base2 = this.<loadInfo>__2.ageActions[this.<j>__1];
                                this.<action>__6 = MonoSingleton<ActionManager>.instance.LoadActionResource(base2.assetPath);
                                this.$current = new CHoldForSecond(0f);
                                this.$PC = 1;
                                goto Label_0440;
                            Label_0131:
                                if (this.<action>__6 != null)
                                {
                                    this.<action>__6.GetAssociatedResources(this.<refAssets>__5, this.<markID>__3);
                                }
                                this.<j>__1++;
                            }
                            this.<i>__0++;
                        }
                        this.<i>__0 = 0;
                        while (this.<i>__0 < this.<>f__this.noActorPreLoad.ageActions.Count)
                        {
                            AssetLoadBase base3 = this.<>f__this.noActorPreLoad.ageActions[this.<i>__0];
                            this.<action>__7 = MonoSingleton<ActionManager>.instance.LoadActionResource(base3.assetPath);
                            this.$current = new CHoldForSecond(0f);
                            this.$PC = 2;
                            goto Label_0440;
                        Label_024C:
                            this.<i>__0++;
                        }
                        this.<numPasses>__9 = 10;
                        this.<i>__0 = 0;
                        while (this.<i>__0 < this.<numPasses>__9)
                        {
                            this.<restAssetsList>__10 = this.loadHelper.AnalyseAgeRefAssets(this.loadHelper.ageRefAssets2);
                            this.<>f__this.ageRefAssetsList.AddRange(this.<restAssetsList>__10);
                            this.loadHelper.ageRefAssets2.Clear();
                            this.<idx>__11 = 0;
                            while (this.<idx>__11 < this.<restAssetsList>__10.Count)
                            {
                                this.<restAssets>__12 = this.<restAssetsList>__10[this.<idx>__11];
                                this.<markID>__13 = this.<restAssets>__12.MarkID;
                                this.<configID>__14 = this.<restAssets>__12.theActor.ConfigId;
                                this.<j>__1 = 0;
                                while (this.<j>__1 < this.<restAssets>__12.ageActions.Count)
                                {
                                    AssetLoadBase base4 = this.<restAssets>__12.ageActions[this.<j>__1];
                                    this.<action>__15 = MonoSingleton<ActionManager>.instance.LoadActionResource(base4.assetPath);
                                    this.$current = new CHoldForSecond(0f);
                                    this.$PC = 4;
                                    goto Label_0440;
                                Label_0370:
                                    if (this.<action>__15 != null)
                                    {
                                        this.<refAssets>__16 = this.loadHelper.GetRefAssets(this.<markID>__13, this.<configID>__14);
                                        this.<action>__15.GetAssociatedResources(this.<refAssets>__16, this.<markID>__13);
                                    }
                                    this.<j>__1++;
                                }
                                this.<idx>__11++;
                            }
                            this.<i>__0++;
                        }
                        this.$current = new CHoldForSecond(0f);
                        this.$PC = 5;
                        goto Label_0440;

                    case 1:
                        goto Label_0131;

                    case 2:
                        if (this.<action>__7 == null)
                        {
                            goto Label_024C;
                        }
                        this.<refAssets>__8 = this.loadHelper.GetRefAssets(0, 0);
                        this.<action>__7.GetAssociatedResources(this.<refAssets>__8, 0);
                        this.$current = new CHoldForSecond(0f);
                        this.$PC = 3;
                        goto Label_0440;

                    case 3:
                        goto Label_024C;

                    case 4:
                        goto Label_0370;

                    case 5:
                        this.$PC = -1;
                        break;
                }
                return false;
            Label_0440:
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
        private sealed class <AnalyseNoActorAssets>c__Iterator1C : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal LoaderHelper <$>loadHelper;
            internal GameLoader <>f__this;
            internal int <i>__0;
            internal ActorMeta <meta>__1;
            internal string <path>__2;
            internal LoaderHelper loadHelper;

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
                        if (this.<>f__this.noActorPreLoad == null)
                        {
                            this.<>f__this.noActorPreLoad = new ActorPreloadTab();
                        }
                        this.$current = Singleton<CCoroutineManager>.GetInstance().StartCoroutine(this.loadHelper.GetGlobalPreload(this.<>f__this.noActorPreLoad));
                        this.$PC = 1;
                        return true;

                    case 1:
                        this.<i>__0 = 0;
                        while (this.<i>__0 < this.<>f__this.actorList.Count)
                        {
                            this.<meta>__1 = this.<>f__this.actorList[this.<i>__0];
                            if (this.<meta>__1.ActorType == ActorTypeDef.Actor_Type_Hero)
                            {
                                this.<path>__2 = KillNotifyUT.GetHero_Icon(ref this.<meta>__1, false);
                                if (!string.IsNullOrEmpty(this.<path>__2))
                                {
                                    this.<>f__this.noActorPreLoad.AddSprite(this.<path>__2);
                                }
                                this.<path>__2 = KillNotifyUT.GetHero_Icon(ref this.<meta>__1, true);
                                if (!string.IsNullOrEmpty(this.<path>__2))
                                {
                                    this.<>f__this.noActorPreLoad.AddMesh(this.<path>__2);
                                }
                            }
                            this.<i>__0++;
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

        [CompilerGenerated]
        private sealed class <AnalyseResourcePreload>c__Iterator19 : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal LoaderHelper <$>loadHelper;
            internal GameLoader <>f__this;
            internal LoaderHelper loadHelper;

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
                        this.$current = Singleton<CCoroutineManager>.GetInstance().StartCoroutine(this.<>f__this.AnalyseActorAssets(this.loadHelper));
                        this.$PC = 1;
                        goto Label_00B9;

                    case 1:
                        this.$current = Singleton<CCoroutineManager>.GetInstance().StartCoroutine(this.<>f__this.AnalyseNoActorAssets(this.loadHelper));
                        this.$PC = 2;
                        goto Label_00B9;

                    case 2:
                        this.$current = Singleton<CCoroutineManager>.GetInstance().StartCoroutine(this.<>f__this.AnalyseAgeRecursiveAssets(this.loadHelper));
                        this.$PC = 3;
                        goto Label_00B9;

                    case 3:
                        this.$PC = -1;
                        break;
                }
                return false;
            Label_00B9:
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
        private sealed class <CoroutineLoad>c__Iterator23 : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal GameLoader <>f__this;
            internal Animator <animator>__9;
            internal List<string> <anims>__10;
            internal GameObject <go>__7;
            internal GameObject <go2>__8;
            internal int <i>__0;
            internal int <i>__11;
            internal int <i>__2;
            internal int <j>__12;
            internal LoaderHelperCamera <lhc>__4;
            internal LoaderHelper <loadHelper>__3;
            internal AsyncOperation <op>__1;
            internal CUIFormScript <uiForm>__5;
            internal CUIFormScript <uiFormKillNotify>__6;

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
                        this.$current = new CWaitForNextFrame();
                        this.$PC = 1;
                        goto Label_0E13;

                    case 1:
                    {
                        Singleton<GameDataMgr>.GetInstance().UnloadAllDataBin();
                        GC.Collect();
                        DynamicShadow.DisableAllDynamicShadows();
                        DynamicShadow.InitDefaultGlobalVariables();
                        Singleton<CUIManager>.GetInstance().ClearFormPool();
                        Singleton<CGameObjectPool>.GetInstance().ClearPooledObjects();
                        enResourceType[] resourceTypes = new enResourceType[5];
                        resourceTypes[1] = enResourceType.UI3DImage;
                        resourceTypes[2] = enResourceType.UIForm;
                        resourceTypes[3] = enResourceType.UIPrefab;
                        resourceTypes[4] = enResourceType.UISprite;
                        Singleton<CResourceManager>.GetInstance().RemoveCachedResources(resourceTypes);
                        this.<>f__this.nProgress = 200;
                        this.<>f__this.LoadProgressEvent(this.<>f__this.nProgress * 0.0001f);
                        this.$current = new CWaitForNextFrame();
                        this.$PC = 2;
                        goto Label_0E13;
                    }
                    case 2:
                        this.<>f__this.nProgress = 300;
                        this.<>f__this.LoadProgressEvent(this.<>f__this.nProgress * 0.0001f);
                        this.$current = new CWaitForNextFrame();
                        this.$PC = 3;
                        goto Label_0E13;

                    case 3:
                        this.<>f__this.nProgress = 400;
                        this.<>f__this.LoadProgressEvent(this.<>f__this.nProgress * 0.0001f);
                        this.$current = new CWaitForNextFrame();
                        this.$PC = 4;
                        goto Label_0E13;

                    case 4:
                        if (this.<>f__this.levelList.Count == 0)
                        {
                            this.<>f__this.levelList.Add("EmptyScene");
                        }
                        PlaneShadowSettings.SetDefault();
                        FogOfWarSettings.SetDefault();
                        this.<>f__this.nProgress = 500;
                        this.<>f__this.LoadProgressEvent(this.<>f__this.nProgress * 0.0001f);
                        Singleton<CResourceManager>.GetInstance().UnloadUnusedAssets();
                        this.$current = new CWaitForNextFrame();
                        this.$PC = 5;
                        goto Label_0E13;

                    case 5:
                        this.<i>__0 = 0;
                        goto Label_02F1;

                    case 6:
                        goto Label_02D3;

                    case 7:
                        if (((this.<>f__this.levelArtistList.Count > 0) || (this.<>f__this.levelDesignList.Count > 0)) && (Camera.get_allCameras() != null))
                        {
                            this.<i>__2 = 0;
                            while (this.<i>__2 < Camera.get_allCameras().Length)
                            {
                                if (Camera.get_main() != null)
                                {
                                    Object.Destroy(Camera.get_allCameras()[this.<i>__2].get_gameObject());
                                }
                                this.<i>__2++;
                            }
                        }
                        this.<>f__this.nProgress = 0x3e8;
                        this.<>f__this.LoadProgressEvent(this.<>f__this.nProgress * 0.0001f);
                        this.$current = new CWaitForNextFrame();
                        this.$PC = 8;
                        goto Label_0E13;

                    case 8:
                    {
                        this.<loadHelper>__3 = new LoaderHelper();
                        this.<lhc>__4 = new LoaderHelperCamera();
                        GameLoader.LoaderHelperWrapper inWrapper = new GameLoader.LoaderHelperWrapper();
                        inWrapper.loadHelper = this.<loadHelper>__3;
                        inWrapper.duty = 350;
                        this.<>f__this.m_handle_LoadCommonAssetBundle = Singleton<CCoroutineManager>.GetInstance().StartCoroutine(this.<>f__this.LoadCommonAssetBundle(inWrapper));
                        this.$current = this.<>f__this.m_handle_LoadCommonAssetBundle;
                        this.$PC = 9;
                        goto Label_0E13;
                    }
                    case 9:
                    {
                        GameLoader.LoaderHelperWrapper wrapper2 = new GameLoader.LoaderHelperWrapper();
                        wrapper2.loadHelper = this.<loadHelper>__3;
                        wrapper2.duty = 150;
                        this.<>f__this.m_handle_LoadCommonEffect = Singleton<CCoroutineManager>.GetInstance().StartCoroutine(this.<>f__this.LoadCommonEffect(wrapper2));
                        this.$current = this.<>f__this.m_handle_LoadCommonEffect;
                        this.$PC = 10;
                        goto Label_0E13;
                    }
                    case 10:
                    {
                        GameLoader.LoaderHelperWrapper wrapper3 = new GameLoader.LoaderHelperWrapper();
                        wrapper3.loadHelper = this.<loadHelper>__3;
                        wrapper3.duty = 0x3e8;
                        this.<>f__this.m_handle_LoadArtistLevel = Singleton<CCoroutineManager>.GetInstance().StartCoroutine(this.<>f__this.LoadArtistLevel(wrapper3));
                        this.$current = this.<>f__this.m_handle_LoadArtistLevel;
                        this.$PC = 11;
                        goto Label_0E13;
                    }
                    case 11:
                        this.$current = new CWaitForNextFrame();
                        this.$PC = 12;
                        goto Label_0E13;

                    case 12:
                    {
                        this.<>f__this.ReleaseMemoryIfNeed();
                        GameLoader.LoaderHelperWrapper wrapper4 = new GameLoader.LoaderHelperWrapper();
                        wrapper4.loadHelper = this.<loadHelper>__3;
                        wrapper4.duty = 500;
                        this.<>f__this.m_handle_LoadDesignLevel = Singleton<CCoroutineManager>.GetInstance().StartCoroutine(this.<>f__this.LoadDesignLevel(wrapper4));
                        this.$current = this.<>f__this.m_handle_LoadDesignLevel;
                        this.$PC = 13;
                        goto Label_0E13;
                    }
                    case 13:
                        this.$current = new CWaitForNextFrame();
                        this.$PC = 14;
                        goto Label_0E13;

                    case 14:
                    {
                        this.<>f__this.ReleaseMemoryIfNeed();
                        FogOfWar.PreBeginLevel();
                        GameLoader.LoaderHelperWrapper wrapper5 = new GameLoader.LoaderHelperWrapper();
                        wrapper5.loadHelper = this.<loadHelper>__3;
                        wrapper5.duty = 500;
                        this.<>f__this.m_handle_LoadCommonAssets = Singleton<CCoroutineManager>.GetInstance().StartCoroutine(this.<>f__this.LoadCommonAssets(wrapper5));
                        this.$current = this.<>f__this.m_handle_LoadCommonAssets;
                        this.$PC = 15;
                        goto Label_0E13;
                    }
                    case 15:
                        this.$current = new CWaitForNextFrame();
                        this.$PC = 0x10;
                        goto Label_0E13;

                    case 0x10:
                        this.<>f__this.ReleaseMemoryIfNeed();
                        this.<>f__this.m_handle_AnalyseResPreload = Singleton<CCoroutineManager>.GetInstance().StartCoroutine(this.<>f__this.AnalyseResourcePreload(this.<loadHelper>__3));
                        this.$current = this.<>f__this.m_handle_AnalyseResPreload;
                        this.$PC = 0x11;
                        goto Label_0E13;

                    case 0x11:
                    {
                        Singleton<CCoroutineManager>.GetInstance().StartCoroutine(this.<loadHelper>__3.ReduceSkillRefDatabin());
                        GameLoader.LHCWrapper wrapper6 = new GameLoader.LHCWrapper();
                        wrapper6.lhc = this.<lhc>__4;
                        wrapper6.loadHelper = this.<loadHelper>__3;
                        wrapper6.duty = 0x9c4;
                        this.<>f__this.m_handle_LoadActorAssets = Singleton<CCoroutineManager>.GetInstance().StartCoroutine(this.<>f__this.LoadActorAssets(wrapper6));
                        this.$current = this.<>f__this.m_handle_LoadActorAssets;
                        this.$PC = 0x12;
                        goto Label_0E13;
                    }
                    case 0x12:
                        this.$current = new CWaitForNextFrame();
                        this.$PC = 0x13;
                        goto Label_0E13;

                    case 0x13:
                    {
                        this.<>f__this.ReleaseMemoryIfNeed();
                        GameLoader.LHCWrapper wrapper7 = new GameLoader.LHCWrapper();
                        wrapper7.lhc = this.<lhc>__4;
                        wrapper7.loadHelper = this.<loadHelper>__3;
                        wrapper7.duty = 0x3e8;
                        this.<>f__this.m_handle_LoadNoActorAssets = Singleton<CCoroutineManager>.GetInstance().StartCoroutine(this.<>f__this.LoadNoActorAssets(wrapper7));
                        this.$current = this.<>f__this.m_handle_LoadNoActorAssets;
                        this.$PC = 20;
                        goto Label_0E13;
                    }
                    case 20:
                        this.$current = new CWaitForNextFrame();
                        this.$PC = 0x15;
                        goto Label_0E13;

                    case 0x15:
                    {
                        this.<>f__this.ReleaseMemoryIfNeed();
                        GameLoader.LHCWrapper wrapper8 = new GameLoader.LHCWrapper();
                        wrapper8.lhc = this.<lhc>__4;
                        wrapper8.loadHelper = this.<loadHelper>__3;
                        wrapper8.duty = 0x76c;
                        this.<>f__this.m_handle_LoadAgeRecursiveAssets = Singleton<CCoroutineManager>.GetInstance().StartCoroutine(this.<>f__this.LoadAgeRecursiveAssets(wrapper8));
                        this.$current = this.<>f__this.m_handle_LoadAgeRecursiveAssets;
                        this.$PC = 0x16;
                        goto Label_0E13;
                    }
                    case 0x16:
                        this.$current = new CWaitForNextFrame();
                        this.$PC = 0x17;
                        goto Label_0E13;

                    case 0x17:
                        this.<>f__this.ReleaseMemoryIfNeed();
                        goto Label_08B6;

                    case 0x18:
                        goto Label_08B6;

                    case 0x19:
                        this.$current = new CWaitForNextFrame();
                        this.$PC = 0x1a;
                        goto Label_0E13;

                    case 0x1a:
                    {
                        this.<>f__this.ReleaseMemoryIfNeed();
                        GameLoader.LoaderHelperWrapper wrapper10 = new GameLoader.LoaderHelperWrapper();
                        wrapper10.loadHelper = this.<loadHelper>__3;
                        wrapper10.duty = 200;
                        this.<>f__this.m_handle_SpawnDynamicActor = Singleton<CCoroutineManager>.GetInstance().StartCoroutine(this.<>f__this.SpawnDynamicActor(wrapper10));
                        this.$current = this.<>f__this.m_handle_SpawnDynamicActor;
                        this.$PC = 0x1b;
                        goto Label_0E13;
                    }
                    case 0x1b:
                        this.$current = new CWaitForNextFrame();
                        this.$PC = 0x1c;
                        goto Label_0E13;

                    case 0x1c:
                        this.<>f__this.ReleaseMemoryIfNeed();
                        FogOfWar.BeginLevel();
                        GC.Collect();
                        this.$current = new CWaitForNextFrame();
                        this.$PC = 0x1d;
                        goto Label_0E13;

                    case 0x1d:
                        this.$current = new CWaitForNextFrame();
                        this.$PC = 30;
                        goto Label_0E13;

                    case 30:
                    {
                        GameLoader.LoaderHelperWrapper wrapper11 = new GameLoader.LoaderHelperWrapper();
                        wrapper11.loadHelper = this.<loadHelper>__3;
                        wrapper11.duty = 100;
                        this.<>f__this.m_handle_PreSpawnSoldiers = Singleton<CCoroutineManager>.GetInstance().StartCoroutine(this.<>f__this.PreSpawnSoldiers(wrapper11));
                        this.$current = this.<>f__this.m_handle_PreSpawnSoldiers;
                        this.$PC = 0x1f;
                        goto Label_0E13;
                    }
                    case 0x1f:
                        this.$current = new CWaitForNextFrame();
                        this.$PC = 0x20;
                        goto Label_0E13;

                    case 0x20:
                        this.<>f__this.ReleaseMemoryIfNeed();
                        this.<>f__this.nProgress = 0x2648;
                        this.<>f__this.LoadProgressEvent(this.<>f__this.nProgress * 0.0001f);
                        this.$current = new CWaitForNextFrame();
                        this.$PC = 0x21;
                        goto Label_0E13;

                    case 0x21:
                        if (GameSettings.AllowOutlineFilter)
                        {
                            OutlineFilter.EnableOutlineFilter();
                        }
                        this.$current = new CWaitForNextFrame();
                        this.$PC = 0x22;
                        goto Label_0E13;

                    case 0x22:
                        Shader.WarmupAllShaders();
                        this.$current = new CWaitForNextFrame();
                        this.$PC = 0x23;
                        goto Label_0E13;

                    case 0x23:
                        Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("ActorInfo");
                        Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharIcon");
                        Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharBattle");
                        Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharShow");
                        Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharLoading");
                        Singleton<CResourceManager>.GetInstance().UnloadUnusedAssets();
                        this.<>f__this.nProgress = 0x26ac;
                        this.<>f__this.LoadProgressEvent(this.<>f__this.nProgress * 0.0001f);
                        this.$current = new CWaitForNextFrame();
                        this.$PC = 0x24;
                        goto Label_0E13;

                    case 0x24:
                        this.$current = new CWaitForNextFrame();
                        this.$PC = 0x25;
                        goto Label_0E13;

                    case 0x25:
                        this.<uiForm>__5 = Singleton<CBattleSystem>.GetInstance().LoadForm(!Singleton<WatchController>.GetInstance().IsWatching ? CBattleSystem.FormType.Fight : CBattleSystem.FormType.Watch);
                        if (!Singleton<WatchController>.GetInstance().IsWatching)
                        {
                            SignalPanel.GetSignalTipsListScript();
                        }
                        this.<uiFormKillNotify>__6 = KillNotify.GetKillNotifyFormScript();
                        if (this.<uiFormKillNotify>__6 != null)
                        {
                            this.<go>__7 = this.<uiFormKillNotify>__6.get_gameObject().FindChildBFS("KillNotify_New");
                            this.<go2>__8 = this.<go>__7.FindChildBFS("KillNotify_Sub");
                            this.<go>__7.CustomSetActive(true);
                            this.<go2>__8.CustomSetActive(true);
                            this.<animator>__9 = this.<go2>__8.GetComponent<Animator>();
                            this.<animator>__9.set_enabled(true);
                            this.<anims>__10 = KillNotifyUT.GetAllAnimations();
                            this.<i>__11 = 0;
                            while (this.<i>__11 < this.<anims>__10.Count)
                            {
                                this.<animator>__9.Play(this.<anims>__10[this.<i>__11]);
                                this.<j>__12 = 0;
                                while (this.<j>__12 < 6)
                                {
                                    this.<animator>__9.Update(0.5f);
                                    this.<j>__12++;
                                }
                                this.$current = new CHoldForSecond(0f);
                                this.$PC = 0x26;
                                goto Label_0E13;
                            Label_0D22:
                                this.<i>__11++;
                            }
                            this.<animator>__9.set_enabled(false);
                        }
                        Singleton<BattleSkillHudControl>.CreateInstance();
                        this.<>f__this.nProgress = 0x2710;
                        this.<>f__this.LoadProgressEvent(this.<>f__this.nProgress * 0.0001f);
                        this.$current = new CWaitForNextFrame();
                        this.$PC = 0x27;
                        goto Label_0E13;

                    case 0x26:
                        goto Label_0D22;

                    case 0x27:
                        Singleton<GameDataMgr>.GetInstance().ReloadDataBinOnFighting();
                        GC.Collect();
                        this.<>f__this.actorPreload = null;
                        this.<>f__this.noActorPreLoad = null;
                        this.<>f__this.ageRefAssetsList.Clear();
                        this.<>f__this.ageRefAssetsList = null;
                        this.<>f__this.isLoadStart = false;
                        this.<>f__this.LoadCompleteEvent();
                        Debug.Log("GameLoader Finish Load");
                        this.$PC = -1;
                        goto Label_0E11;

                    default:
                        goto Label_0E11;
                }
            Label_02D3:
                while (!this.<op>__1.get_isDone())
                {
                    this.$current = new CHoldForSecond(0f);
                    this.$PC = 6;
                    goto Label_0E13;
                }
                this.<i>__0++;
            Label_02F1:
                if (this.<i>__0 < this.<>f__this.levelList.Count)
                {
                    this.<op>__1 = Application.LoadLevelAsync((string) this.<>f__this.levelList[this.<i>__0]);
                    goto Label_02D3;
                }
                this.<>f__this.nProgress = 600;
                this.<>f__this.LoadProgressEvent(this.<>f__this.nProgress * 0.0001f);
                this.$current = new CWaitForNextFrame();
                this.$PC = 7;
                goto Label_0E13;
            Label_08B6:
                if (!this.<lhc>__4.Update())
                {
                    this.$current = new CWaitForNextFrame();
                    this.$PC = 0x18;
                }
                else
                {
                    this.<lhc>__4.Destroy();
                    this.<lhc>__4 = null;
                    GameLoader.LoaderHelperWrapper wrapper9 = new GameLoader.LoaderHelperWrapper();
                    wrapper9.loadHelper = this.<loadHelper>__3;
                    wrapper9.duty = 200;
                    this.<>f__this.m_handle_SpawnStaticActor = Singleton<CCoroutineManager>.GetInstance().StartCoroutine(this.<>f__this.SpawnStaticActor(wrapper9));
                    this.$current = this.<>f__this.m_handle_SpawnStaticActor;
                    this.$PC = 0x19;
                }
                goto Label_0E13;
            Label_0E11:
                return false;
            Label_0E13:
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
        private sealed class <LoadActorAssets>c__Iterator1B : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal GameLoader.LHCWrapper <$>InWrapper;
            internal GameLoader <>f__this;
            internal ActorMeta <actorMeta>__7;
            internal List<List<string>> <allComposites>__16;
            internal string <btPath>__53;
            internal int <count>__4;
            internal int <duty>__1;
            internal ResHeroCfgInfo <heroCfgInfo>__9;
            internal Player <hostPlayer>__8;
            internal int <i>__13;
            internal int <i>__17;
            internal int <i>__38;
            internal int <i>__41;
            internal int <i>__6;
            internal string <iconPath>__12;
            internal int <index>__5;
            internal int <j>__10;
            internal int <j>__19;
            internal int <j>__22;
            internal int <j>__25;
            internal int <j>__28;
            internal int <j>__31;
            internal int <j>__34;
            internal int <j>__37;
            internal int <j>__44;
            internal int <j>__50;
            internal int <j>__51;
            internal int <j>__52;
            internal string[] <keywords>__15;
            internal string <label>__18;
            internal LoaderHelperCamera <lhc>__0;
            internal LoaderHelper <loadHelper>__2;
            internal ActorPreloadTab <loadInfo>__14;
            internal ActorPreloadTab <loadInfo>__40;
            internal ActorPreloadTab <loadInfo>__42;
            internal int <lod>__47;
            internal Material <mat0>__21;
            internal Material <mat1>__24;
            internal Material <mat2>__27;
            internal Material <mat3>__30;
            internal Material <mat4>__33;
            internal Material <mat5>__36;
            internal GameObject <obj0>__20;
            internal GameObject <obj1>__23;
            internal GameObject <obj2>__26;
            internal GameObject <obj3>__29;
            internal GameObject <obj4>__32;
            internal GameObject <obj5>__35;
            internal int <oldProgress>__3;
            internal int <originalParticleLOD>__45;
            internal string <parPathKey>__48;
            internal string <parPathKey>__49;
            internal ResSkillCfgInfo <skillCfgInfo>__11;
            internal int <targetParticleOLD>__46;
            internal GameObject <tmpObj>__39;
            internal GameObject <tmpObj>__43;
            internal GameLoader.LHCWrapper InWrapper;

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
                        this.<lhc>__0 = this.InWrapper.lhc;
                        this.<duty>__1 = this.InWrapper.duty;
                        this.<loadHelper>__2 = this.InWrapper.loadHelper;
                        this.<oldProgress>__3 = this.<>f__this.nProgress;
                        this.<count>__4 = this.<>f__this.actorPreload.Count;
                        this.<index>__5 = 0;
                        this.<i>__6 = 0;
                        while (this.<i>__6 < this.<>f__this.actorPreload.Count)
                        {
                            this.<actorMeta>__7 = this.<>f__this.actorPreload[this.<i>__6].theActor;
                            this.<hostPlayer>__8 = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
                            if (this.<hostPlayer>__8.PlayerId == this.<actorMeta>__7.PlayerId)
                            {
                                this.<heroCfgInfo>__9 = GameDataMgr.heroDatabin.GetDataByKey((long) this.<actorMeta>__7.ConfigId);
                                this.<j>__10 = 0;
                                while (this.<j>__10 < this.<heroCfgInfo>__9.astSkill.Length)
                                {
                                    this.<skillCfgInfo>__11 = GameDataMgr.skillDatabin.GetDataByKey((long) this.<heroCfgInfo>__9.astSkill[this.<j>__10].iSkillID);
                                    object[] inParameters = new object[] { this.<heroCfgInfo>__9.astSkill[this.<j>__10].iSkillID };
                                    DebugHelper.Assert(this.<skillCfgInfo>__11 != null, "Failed Found skill config id = {0}", inParameters);
                                    this.<iconPath>__12 = string.Empty;
                                    if (this.<skillCfgInfo>__11 != null)
                                    {
                                        this.<iconPath>__12 = StringHelper.UTF8BytesToString(ref this.<skillCfgInfo>__11.szIconPath);
                                    }
                                    if (string.IsNullOrEmpty(this.<iconPath>__12))
                                    {
                                        goto Label_02BE;
                                    }
                                    this.<iconPath>__12 = CUIUtility.s_Sprite_Dynamic_Skill_Dir + this.<iconPath>__12;
                                    this.$current = new CHoldForSecond(0f);
                                    this.$PC = 1;
                                    goto Label_1120;
                                Label_0203:
                                    Singleton<CGameObjectPool>.GetInstance().PrepareGameObject(this.<iconPath>__12, enResourceType.BattleScene, 1, true);
                                    if (this.<iconPath>__12.EndsWith("0"))
                                    {
                                        this.<iconPath>__12 = this.<iconPath>__12.Substring(0, this.<iconPath>__12.Length - 1);
                                        Singleton<CGameObjectPool>.GetInstance().PrepareGameObject(this.<iconPath>__12 + "1", enResourceType.BattleScene, 1, false);
                                        Singleton<CGameObjectPool>.GetInstance().PrepareGameObject(this.<iconPath>__12 + "2", enResourceType.BattleScene, 1, false);
                                        Singleton<CGameObjectPool>.GetInstance().PrepareGameObject(this.<iconPath>__12 + "3", enResourceType.BattleScene, 1, false);
                                        Singleton<CGameObjectPool>.GetInstance().PrepareGameObject(this.<iconPath>__12 + "4", enResourceType.BattleScene, 1, false);
                                    }
                                Label_02BE:
                                    this.<j>__10++;
                                }
                            }
                            this.<i>__6++;
                        }
                        this.<i>__13 = 0;
                        while (this.<i>__13 < this.<>f__this.actorPreload.Count)
                        {
                            this.<loadInfo>__14 = this.<>f__this.actorPreload[this.<i>__13];
                            this.<count>__4 += (((this.<loadInfo>__14.parPrefabs.Count + this.<loadInfo>__14.mesPrefabs.Count) + this.<loadInfo>__14.soundBanks.Count) + this.<loadInfo>__14.ageActions.Count) + this.<loadInfo>__14.behaviorXml.Count;
                            this.<i>__13++;
                        }
                        this.<keywords>__15 = new string[] { "_LIGHT_TEX_ON", "_HURT_EFFECT_ON", "_EFFECT_TEX_ON", "_RIM_COLOR_ON" };
                        this.<allComposites>__16 = GameLoader.composite(this.<keywords>__15);
                        this.<i>__17 = 0;
                        while (this.<i>__17 < this.<allComposites>__16.Count)
                        {
                            this.<label>__18 = string.Empty;
                            this.<j>__19 = 0;
                            while (this.<j>__19 < this.<allComposites>__16[this.<i>__17].Count)
                            {
                                this.<label>__18 = this.<label>__18 + this.<allComposites>__16[this.<i>__17][this.<j>__19] + ",";
                                this.<j>__19++;
                            }
                            this.<obj0>__20 = Singleton<CGameObjectPool>.instance.GetGameObject("HelperResources/Cube", enResourceType.BattleScene);
                            this.<mat0>__21 = new Material(Shader.Find("S_Game_Hero/Hero_Battle (Occlusion)"));
                            this.<j>__22 = 0;
                            while (this.<j>__22 < this.<allComposites>__16[this.<i>__17].Count)
                            {
                                this.<mat0>__21.EnableKeyword(this.<allComposites>__16[this.<i>__17][this.<j>__22]);
                                this.<j>__22++;
                            }
                            this.<obj0>__20.get_renderer().set_material(this.<mat0>__21);
                            this.<obj1>__23 = Singleton<CGameObjectPool>.instance.GetGameObject("HelperResources/Cube", enResourceType.BattleScene);
                            this.<mat1>__24 = new Material(Shader.Find("S_Game_Hero/Hero_Battle (Shadow) (Occlusion)"));
                            this.<j>__25 = 0;
                            while (this.<j>__25 < this.<allComposites>__16[this.<i>__17].Count)
                            {
                                this.<mat1>__24.EnableKeyword(this.<allComposites>__16[this.<i>__17][this.<j>__25]);
                                this.<j>__25++;
                            }
                            this.<obj1>__23.get_renderer().set_material(this.<mat1>__24);
                            this.<obj2>__26 = Singleton<CGameObjectPool>.instance.GetGameObject("HelperResources/Cube", enResourceType.BattleScene);
                            this.<mat2>__27 = new Material(Shader.Find("S_Game_Hero/Hero_Battle (Shadow) (Translucent)"));
                            this.<j>__28 = 0;
                            while (this.<j>__28 < this.<allComposites>__16[this.<i>__17].Count)
                            {
                                this.<mat2>__27.EnableKeyword(this.<allComposites>__16[this.<i>__17][this.<j>__28]);
                                this.<j>__28++;
                            }
                            this.<obj2>__26.get_renderer().set_material(this.<mat2>__27);
                            this.<obj3>__29 = Singleton<CGameObjectPool>.instance.GetGameObject("HelperResources/Cube", enResourceType.BattleScene);
                            this.<mat3>__30 = new Material(Shader.Find("S_Game_Hero/Hero_Battle (Shadow)"));
                            this.<j>__31 = 0;
                            while (this.<j>__31 < this.<allComposites>__16[this.<i>__17].Count)
                            {
                                this.<mat3>__30.EnableKeyword(this.<allComposites>__16[this.<i>__17][this.<j>__31]);
                                this.<j>__31++;
                            }
                            this.<obj3>__29.get_renderer().set_material(this.<mat3>__30);
                            this.<obj4>__32 = Singleton<CGameObjectPool>.instance.GetGameObject("HelperResources/Cube", enResourceType.BattleScene);
                            this.<mat4>__33 = new Material(Shader.Find("S_Game_Hero/Hero_Battle (Translucent)"));
                            this.<j>__34 = 0;
                            while (this.<j>__34 < this.<allComposites>__16[this.<i>__17].Count)
                            {
                                this.<mat4>__33.EnableKeyword(this.<allComposites>__16[this.<i>__17][this.<j>__34]);
                                this.<j>__34++;
                            }
                            this.<obj4>__32.get_renderer().set_material(this.<mat4>__33);
                            this.<obj5>__35 = Singleton<CGameObjectPool>.instance.GetGameObject("HelperResources/Cube", enResourceType.BattleScene);
                            this.<mat5>__36 = new Material(Shader.Find("S_Game_Hero/Hero_Battle"));
                            this.<j>__37 = 0;
                            while (this.<j>__37 < this.<allComposites>__16[this.<i>__17].Count)
                            {
                                this.<mat5>__36.EnableKeyword(this.<allComposites>__16[this.<i>__17][this.<j>__37]);
                                this.<j>__37++;
                            }
                            this.<obj5>__35.get_renderer().set_material(this.<mat5>__36);
                            this.<lhc>__0.AddObj("cube0" + this.<label>__18, this.<obj0>__20);
                            this.<lhc>__0.AddObj("cube1" + this.<label>__18, this.<obj1>__23);
                            this.<lhc>__0.AddObj("cube2" + this.<label>__18, this.<obj2>__26);
                            this.<lhc>__0.AddObj("cube3" + this.<label>__18, this.<obj3>__29);
                            this.<lhc>__0.AddObj("cube4" + this.<label>__18, this.<obj4>__32);
                            this.<lhc>__0.AddObj("cube5" + this.<label>__18, this.<obj5>__35);
                            this.<i>__17++;
                        }
                        this.<i>__38 = 0;
                        while (this.<i>__38 < this.<>f__this.actorPreload.Count)
                        {
                            this.<tmpObj>__39 = null;
                            this.<loadInfo>__40 = this.<>f__this.actorPreload[this.<i>__38];
                            if ((this.<loadInfo>__40.modelPrefab.assetPath == null) || this.InWrapper.lhc.HasLoaded(this.<loadInfo>__40.modelPrefab.assetPath))
                            {
                                goto Label_0A27;
                            }
                            this.<tmpObj>__39 = Singleton<CGameObjectPool>.instance.GetGameObject(this.<loadInfo>__40.modelPrefab.assetPath, enResourceType.BattleScene);
                            if (this.<loadInfo>__40.modelPrefab.assetPath.ToLower().Contains("prefab_hero"))
                            {
                            }
                            this.$current = new CHoldForSecond(0f);
                            this.$PC = 2;
                            goto Label_1120;
                        Label_0A06:
                            this.<lhc>__0.AddObj(this.<loadInfo>__40.modelPrefab.assetPath, this.<tmpObj>__39);
                        Label_0A27:
                            this.<i>__38++;
                        }
                        this.<i>__41 = 0;
                        while (this.<i>__41 < this.<>f__this.actorPreload.Count)
                        {
                            this.<loadInfo>__42 = this.<>f__this.actorPreload[this.<i>__41];
                            this.<tmpObj>__43 = null;
                            this.<j>__44 = 0;
                            while (this.<j>__44 < this.<loadInfo>__42.parPrefabs.Count)
                            {
                                AssetLoadBase base2 = this.<loadInfo>__42.parPrefabs[this.<j>__44];
                                if (!this.<lhc>__0.HasLoaded(base2.assetPath))
                                {
                                    if (GameSettings.DynamicParticleLOD)
                                    {
                                        this.<originalParticleLOD>__45 = GameSettings.ParticleLOD;
                                        this.<targetParticleOLD>__46 = this.<originalParticleLOD>__45;
                                        if (((Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer() != null) && (this.<loadInfo>__42.theActor.PlayerId == Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer().PlayerId)) && (this.<targetParticleOLD>__46 > 1))
                                        {
                                            this.<targetParticleOLD>__46 = 1;
                                        }
                                        this.<lod>__47 = this.<targetParticleOLD>__46;
                                        while (this.<lod>__47 <= 2)
                                        {
                                            if ((this.<targetParticleOLD>__46 != 0) || (this.<lod>__47 != 1))
                                            {
                                                AssetLoadBase base3 = this.<loadInfo>__42.parPrefabs[this.<j>__44];
                                                this.<parPathKey>__48 = base3.assetPath + "_lod_" + this.<lod>__47;
                                                if (!this.<lhc>__0.HasLoaded(this.<parPathKey>__48))
                                                {
                                                    GameSettings.ParticleLOD = this.<lod>__47;
                                                    AssetLoadBase base4 = this.<loadInfo>__42.parPrefabs[this.<j>__44];
                                                    this.<tmpObj>__43 = MonoSingleton<SceneMgr>.instance.GetPooledGameObjLOD(base4.assetPath, true, SceneObjType.ActionRes, Vector3.get_zero());
                                                    this.<lhc>__0.AddObj(this.<parPathKey>__48, this.<tmpObj>__43);
                                                    this.$current = new CHoldForSecond(0f);
                                                    this.$PC = 3;
                                                    goto Label_1120;
                                                }
                                            }
                                        Label_0C0F:
                                            this.<lod>__47++;
                                        }
                                        GameSettings.ParticleLOD = this.<originalParticleLOD>__45;
                                    }
                                    else
                                    {
                                        AssetLoadBase base5 = this.<loadInfo>__42.parPrefabs[this.<j>__44];
                                        this.<parPathKey>__49 = base5.assetPath;
                                        if (!this.<lhc>__0.HasLoaded(this.<parPathKey>__49))
                                        {
                                            AssetLoadBase base6 = this.<loadInfo>__42.parPrefabs[this.<j>__44];
                                            this.<tmpObj>__43 = MonoSingleton<SceneMgr>.instance.GetPooledGameObjLOD(base6.assetPath, true, SceneObjType.ActionRes, Vector3.get_zero());
                                            this.<lhc>__0.AddObj(this.<parPathKey>__49, this.<tmpObj>__43);
                                            this.$current = new CHoldForSecond(0f);
                                            this.$PC = 4;
                                            goto Label_1120;
                                        }
                                    }
                                }
                            Label_0CDD:
                                this.<index>__5++;
                                this.<>f__this.UpdateProgress(this.<lhc>__0, this.<oldProgress>__3, this.<duty>__1, this.<index>__5, this.<count>__4);
                                this.$current = new CHoldForSecond(0f);
                                this.$PC = 5;
                                goto Label_1120;
                            Label_0D30:
                                this.<j>__44++;
                            }
                            this.<j>__50 = 0;
                            while (this.<j>__50 < this.<loadInfo>__42.mesPrefabs.Count)
                            {
                                AssetLoadBase base7 = this.<loadInfo>__42.mesPrefabs[this.<j>__50];
                                if (!this.<lhc>__0.HasLoaded(base7.assetPath))
                                {
                                    AssetLoadBase base8 = this.<loadInfo>__42.mesPrefabs[this.<j>__50];
                                    this.<tmpObj>__43 = MonoSingleton<SceneMgr>.instance.GetPooledGameObjLOD(base8.assetPath, false, SceneObjType.ActionRes, Vector3.get_zero());
                                    AssetLoadBase base9 = this.<loadInfo>__42.mesPrefabs[this.<j>__50];
                                    this.<lhc>__0.AddObj(base9.assetPath, this.<tmpObj>__43);
                                    this.$current = new CHoldForSecond(0f);
                                    this.$PC = 6;
                                    goto Label_1120;
                                }
                            Label_0E16:
                                this.<index>__5++;
                                this.<>f__this.UpdateProgress(this.<lhc>__0, this.<oldProgress>__3, this.<duty>__1, this.<index>__5, this.<count>__4);
                                this.$current = new CHoldForSecond(0f);
                                this.$PC = 7;
                                goto Label_1120;
                            Label_0E69:
                                this.<j>__50++;
                            }
                            this.<j>__51 = 0;
                            while (this.<j>__51 < this.<loadInfo>__42.soundBanks.Count)
                            {
                                AssetLoadBase base10 = this.<loadInfo>__42.soundBanks[this.<j>__51];
                                Singleton<CSoundManager>.instance.LoadBank(base10.assetPath, CSoundManager.BankType.Battle);
                                this.$current = new CHoldForSecond(0f);
                                this.$PC = 8;
                                goto Label_1120;
                            Label_0F38:
                                this.<j>__51++;
                            }
                            this.<j>__52 = 0;
                            while (this.<j>__52 < this.<loadInfo>__42.behaviorXml.Count)
                            {
                                AssetLoadBase base11 = this.<loadInfo>__42.behaviorXml[this.<j>__52];
                                this.<btPath>__53 = base11.assetPath;
                                Workspace.Load(this.<btPath>__53, false);
                                this.$current = new CHoldForSecond(0f);
                                this.$PC = 10;
                                goto Label_1120;
                            Label_1010:
                                this.<j>__52++;
                            }
                            this.<index>__5++;
                            this.<>f__this.UpdateProgress(this.<lhc>__0, this.<oldProgress>__3, this.<duty>__1, this.<index>__5, this.<count>__4);
                            this.$current = new CHoldForSecond(0f);
                            this.$PC = 12;
                            goto Label_1120;
                        Label_108D:
                            this.<i>__41++;
                        }
                        this.<>f__this.UpdateProgress(this.<lhc>__0, this.<oldProgress>__3, this.<duty>__1, 1, 1);
                        this.$current = new CWaitForNextFrame();
                        this.$PC = 13;
                        goto Label_1120;

                    case 1:
                        goto Label_0203;

                    case 2:
                        goto Label_0A06;

                    case 3:
                        goto Label_0C0F;

                    case 4:
                        goto Label_0CDD;

                    case 5:
                        goto Label_0D30;

                    case 6:
                        goto Label_0E16;

                    case 7:
                        goto Label_0E69;

                    case 8:
                        this.<index>__5++;
                        this.<>f__this.UpdateProgress(this.<lhc>__0, this.<oldProgress>__3, this.<duty>__1, this.<index>__5, this.<count>__4);
                        this.$current = new CHoldForSecond(0f);
                        this.$PC = 9;
                        goto Label_1120;

                    case 9:
                        goto Label_0F38;

                    case 10:
                        this.<index>__5++;
                        this.<>f__this.UpdateProgress(this.<lhc>__0, this.<oldProgress>__3, this.<duty>__1, this.<index>__5, this.<count>__4);
                        this.$current = new CHoldForSecond(0f);
                        this.$PC = 11;
                        goto Label_1120;

                    case 11:
                        goto Label_1010;

                    case 12:
                        goto Label_108D;

                    case 13:
                        this.<>f__this.UpdateProgress(this.<lhc>__0, this.<oldProgress>__3, this.<duty>__1, 1, 1);
                        this.<>f__this.ReleaseMemoryIfNeed();
                        this.$PC = -1;
                        break;
                }
                return false;
            Label_1120:
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
        private sealed class <LoadAgeRecursiveAssets>c__Iterator1F : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal GameLoader.LHCWrapper <$>InWrapper;
            internal GameLoader <>f__this;
            internal Action <action>__29;
            internal string <assetPath>__16;
            internal int <configID>__13;
            internal int <count>__5;
            internal int <currentParticleLOD>__17;
            internal int <duty>__1;
            internal GameObject <gameObj>__26;
            internal int <idx>__10;
            internal int <idx>__8;
            internal int <index>__6;
            internal int <j>__14;
            internal int <j>__21;
            internal int <j>__24;
            internal int <j>__28;
            internal LoaderHelperCamera <lhc>__0;
            internal LoaderHelper <loadHelper>__2;
            internal int <lod>__18;
            internal int <markID>__12;
            internal int <oldProgress>__3;
            internal string <parPathKey>__19;
            internal string <parPathKey>__20;
            internal int <progress>__7;
            internal ActorPreloadTab <restAssets>__11;
            internal ActorPreloadTab <restAssets>__9;
            internal SpriteRenderer <sr>__27;
            internal int <subDuty>__4;
            internal CResource <tempObj>__25;
            internal ListView<Texture> <textures>__23;
            internal GameObject <tmpObj>__15;
            internal GameObject <tmpObj>__22;
            internal GameLoader.LHCWrapper InWrapper;

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
                        this.<lhc>__0 = this.InWrapper.lhc;
                        this.<duty>__1 = this.InWrapper.duty;
                        this.<loadHelper>__2 = this.InWrapper.loadHelper;
                        this.<oldProgress>__3 = this.<>f__this.nProgress;
                        this.<subDuty>__4 = 1;
                        this.<count>__5 = 0;
                        this.<index>__6 = 0;
                        this.<progress>__7 = this.<>f__this.nProgress;
                        this.<idx>__8 = 0;
                        while (this.<idx>__8 < this.<>f__this.ageRefAssetsList.Count)
                        {
                            this.<restAssets>__9 = this.<>f__this.ageRefAssetsList[this.<idx>__8];
                            this.<count>__5 += (this.<restAssets>__9.parPrefabs.Count + this.<restAssets>__9.mesPrefabs.Count) + this.<restAssets>__9.ageActions.Count;
                            this.<idx>__8++;
                        }
                        this.<idx>__10 = 0;
                        while (this.<idx>__10 < this.<>f__this.ageRefAssetsList.Count)
                        {
                            this.<restAssets>__11 = this.<>f__this.ageRefAssetsList[this.<idx>__10];
                            this.<markID>__12 = this.<restAssets>__11.MarkID;
                            this.<configID>__13 = this.<restAssets>__11.theActor.ConfigId;
                            this.<j>__14 = 0;
                            while (this.<j>__14 < this.<restAssets>__11.parPrefabs.Count)
                            {
                                this.<tmpObj>__15 = null;
                                AssetLoadBase base2 = this.<restAssets>__11.parPrefabs[this.<j>__14];
                                this.<assetPath>__16 = base2.assetPath;
                                if (GameSettings.DynamicParticleLOD)
                                {
                                    this.<currentParticleLOD>__17 = GameSettings.ParticleLOD;
                                    this.<lod>__18 = this.<currentParticleLOD>__17;
                                    while (this.<lod>__18 <= 2)
                                    {
                                        if ((this.<currentParticleLOD>__17 != 0) || (this.<lod>__18 != 1))
                                        {
                                            this.<parPathKey>__19 = this.<assetPath>__16 + "_lod_" + this.<lod>__18;
                                            if (!this.<lhc>__0.HasLoaded(this.<parPathKey>__19))
                                            {
                                                GameSettings.ParticleLOD = this.<lod>__18;
                                                this.<tmpObj>__15 = MonoSingleton<SceneMgr>.instance.GetPooledGameObjLOD(this.<assetPath>__16, true, SceneObjType.ActionRes, Vector3.get_zero());
                                                this.<lhc>__0.AddObj(this.<parPathKey>__19, this.<tmpObj>__15);
                                                this.$current = new CHoldForSecond(0f);
                                                this.$PC = 1;
                                                goto Label_086A;
                                            }
                                        }
                                    Label_029D:
                                        this.<lod>__18++;
                                    }
                                    GameSettings.ParticleLOD = this.<currentParticleLOD>__17;
                                }
                                else
                                {
                                    this.<parPathKey>__20 = this.<assetPath>__16;
                                    if (!this.<lhc>__0.HasLoaded(this.<parPathKey>__20))
                                    {
                                        this.<tmpObj>__15 = MonoSingleton<SceneMgr>.instance.GetPooledGameObjLOD(this.<assetPath>__16, true, SceneObjType.ActionRes, Vector3.get_zero());
                                        this.<lhc>__0.AddObj(this.<parPathKey>__20, this.<tmpObj>__15);
                                        this.$current = new CHoldForSecond(0f);
                                        this.$PC = 2;
                                        goto Label_086A;
                                    }
                                }
                            Label_0339:
                                this.<index>__6++;
                                this.<>f__this.UpdateProgress(this.<lhc>__0, this.<progress>__7, this.<subDuty>__4, this.<index>__6, this.<count>__5);
                                this.$current = new CHoldForSecond(0f);
                                this.$PC = 3;
                                goto Label_086A;
                            Label_038C:
                                this.<j>__14++;
                            }
                            this.<j>__21 = 0;
                            while (this.<j>__21 < this.<restAssets>__11.mesPrefabs.Count)
                            {
                                this.<tmpObj>__22 = null;
                                AssetLoadBase base3 = this.<restAssets>__11.mesPrefabs[this.<j>__21];
                                if (!this.<lhc>__0.HasLoaded(base3.assetPath))
                                {
                                    AssetLoadBase base4 = this.<restAssets>__11.mesPrefabs[this.<j>__21];
                                    this.<tmpObj>__22 = MonoSingleton<SceneMgr>.instance.GetPooledGameObjLOD(base4.assetPath, false, SceneObjType.ActionRes, Vector3.get_zero());
                                    AssetLoadBase base5 = this.<restAssets>__11.mesPrefabs[this.<j>__21];
                                    this.<lhc>__0.AddObj(base5.assetPath, this.<tmpObj>__22);
                                    this.$current = new CHoldForSecond(0f);
                                    this.$PC = 4;
                                    goto Label_086A;
                                }
                            Label_0477:
                                this.<index>__6++;
                                this.<>f__this.UpdateProgress(this.<lhc>__0, this.<progress>__7, this.<subDuty>__4, this.<index>__6, this.<count>__5);
                                this.$current = new CHoldForSecond(0f);
                                this.$PC = 5;
                                goto Label_086A;
                            Label_04CA:
                                this.<j>__21++;
                            }
                            this.<textures>__23 = new ListView<Texture>();
                            this.<j>__24 = 0;
                            while (this.<j>__24 < this.<restAssets>__11.spritePrefabs.Count)
                            {
                                AssetLoadBase base6 = this.<restAssets>__11.spritePrefabs[this.<j>__24];
                                this.<tempObj>__25 = Singleton<CResourceManager>.instance.GetResource(base6.assetPath, typeof(GameObject), enResourceType.UIPrefab, true, false);
                                this.$current = new CHoldForSecond(0f);
                                this.$PC = 6;
                                goto Label_086A;
                            Label_0562:
                                if (((this.<tempObj>__25 != null) && (this.<tempObj>__25.m_content != null)) && (this.<tempObj>__25.m_content is GameObject))
                                {
                                    this.<gameObj>__26 = this.<tempObj>__25.m_content;
                                    this.<sr>__27 = this.<gameObj>__26.GetComponent<SpriteRenderer>();
                                    if (((this.<sr>__27 != null) && (this.<sr>__27.get_sprite() != null)) && (this.<sr>__27.get_sprite().get_texture() != null))
                                    {
                                        this.<textures>__23.Add(this.<sr>__27.get_sprite().get_texture());
                                    }
                                    this.$current = new CHoldForSecond(0f);
                                    this.$PC = 7;
                                    goto Label_086A;
                                }
                            Label_0638:
                                this.<index>__6++;
                                this.<>f__this.UpdateProgress(this.<lhc>__0, this.<progress>__7, this.<subDuty>__4, this.<index>__6, this.<count>__5);
                                this.$current = new CHoldForSecond(0f);
                                this.$PC = 8;
                                goto Label_086A;
                            Label_068B:
                                this.<j>__24++;
                            }
                            this.<textures>__23.Clear();
                            this.<j>__28 = 0;
                            while (this.<j>__28 < this.<restAssets>__11.ageActions.Count)
                            {
                                AssetLoadBase base7 = this.<restAssets>__11.ageActions[this.<j>__28];
                                this.<action>__29 = MonoSingleton<ActionManager>.instance.LoadActionResource(base7.assetPath);
                                this.$current = new CHoldForSecond(0f);
                                this.$PC = 9;
                                goto Label_086A;
                            Label_0717:
                                this.<index>__6++;
                                this.<>f__this.UpdateProgress(this.<lhc>__0, this.<progress>__7, this.<subDuty>__4, this.<index>__6, this.<count>__5);
                                this.<j>__28++;
                            }
                            this.<idx>__10++;
                        }
                        this.<>f__this.ReleaseMemoryIfNeed();
                        this.<>f__this.UpdateProgress(this.<lhc>__0, this.<progress>__7, this.<subDuty>__4, 1, 1);
                        this.$current = new CHoldForSecond(0f);
                        this.$PC = 10;
                        goto Label_086A;

                    case 1:
                        goto Label_029D;

                    case 2:
                        goto Label_0339;

                    case 3:
                        goto Label_038C;

                    case 4:
                        goto Label_0477;

                    case 5:
                        goto Label_04CA;

                    case 6:
                        goto Label_0562;

                    case 7:
                        goto Label_0638;

                    case 8:
                        goto Label_068B;

                    case 9:
                        goto Label_0717;

                    case 10:
                        this.<>f__this.UpdateProgress(this.<lhc>__0, this.<progress>__7, this.<subDuty>__4, 1, 1);
                        this.<>f__this.UpdateProgress(this.<lhc>__0, this.<oldProgress>__3, this.<duty>__1, 1, 1);
                        this.$current = new CHoldForSecond(0f);
                        this.$PC = 11;
                        goto Label_086A;

                    case 11:
                        this.<>f__this.UpdateProgress(this.<lhc>__0, this.<oldProgress>__3, this.<duty>__1, 1, 1);
                        this.$PC = -1;
                        break;
                }
                return false;
            Label_086A:
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
        private sealed class <LoadArtistLevel>c__Iterator16 : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal GameLoader.LoaderHelperWrapper <$>InWrapper;
            internal GameLoader <>f__this;
            internal string <artAssetNameHigh>__2;
            internal string <artAssetNameLow>__4;
            internal string <artAssetNameMid>__3;
            internal GameObject <artRoot>__10;
            internal int <duty>__14;
            internal string <fullPath>__7;
            internal ObjectHolder <holder>__9;
            internal int <i>__1;
            internal int <j>__13;
            internal LevelResAsset <levelArtist>__6;
            internal string[] <levelNames>__5;
            internal int <lod>__8;
            internal int <oldProgress>__0;
            internal ParticleSystem[] <psArray>__12;
            internal Transform <staticRoot>__11;
            internal GameLoader.LoaderHelperWrapper InWrapper;

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
                        this.<oldProgress>__0 = this.<>f__this.nProgress;
                        this.<i>__1 = 0;
                        goto Label_042C;

                    case 1:
                        if (null == this.<levelArtist>__6)
                        {
                            goto Label_01A5;
                        }
                        goto Label_01BF;

                    case 2:
                        this.<artRoot>__10 = this.<holder>__9.obj as GameObject;
                        if (null != this.<artRoot>__10)
                        {
                            this.<staticRoot>__11 = this.<artRoot>__10.get_transform().Find("StaticMesh");
                            if (null != this.<staticRoot>__11)
                            {
                                StaticBatchingUtility.Combine(this.<staticRoot>__11.get_gameObject());
                                this.$current = new CHoldForSecond(0f);
                                this.$PC = 3;
                                goto Label_04C1;
                            }
                            goto Label_02EA;
                        }
                        Debug.LogError("美术场景SceneExport/Artist/" + this.<>f__this.levelArtistList[this.<i>__1] + ".asset有错误！请检查！");
                        goto Label_041E;

                    case 3:
                        goto Label_02EA;

                    case 4:
                        this.<psArray>__12 = this.<artRoot>__10.GetComponentsInChildren<ParticleSystem>();
                        this.$current = new CHoldForSecond(0f);
                        this.$PC = 5;
                        goto Label_04C1;

                    case 5:
                        this.<j>__13 = 0;
                        while (this.<j>__13 < this.<psArray>__12.Length)
                        {
                            if (((this.<psArray>__12[this.<j>__13] != null) && this.<psArray>__12[this.<j>__13].get_gameObject().get_activeSelf()) && (this.<psArray>__12[this.<j>__13].get_transform().get_parent() != null))
                            {
                                MonoSingleton<SceneMgr>.GetInstance().AddCulling(this.<psArray>__12[this.<j>__13].get_transform().get_gameObject(), "ParticleCulling_" + this.<j>__13);
                                this.$current = new CHoldForSecond(0f);
                                this.$PC = 6;
                                goto Label_04C1;
                            }
                        Label_03FD:
                            this.<j>__13++;
                        }
                        goto Label_041E;

                    case 6:
                        goto Label_03FD;

                    case 7:
                        this.$PC = -1;
                        goto Label_04BF;

                    default:
                        goto Label_04BF;
                }
            Label_01BF:
                if (null == this.<levelArtist>__6)
                {
                    Debug.LogError("错误，没有找到导出的美术场景SceneExport/Artist/" + this.<>f__this.levelArtistList[this.<i>__1] + ".asset");
                    goto Label_041E;
                }
                this.<holder>__9 = new ObjectHolder();
                this.$current = Singleton<CCoroutineManager>.GetInstance().StartCoroutine(GameLoader.s_serializer.LoadAsync(this.<levelArtist>__6, this.<holder>__9));
                this.$PC = 2;
                goto Label_04C1;
            Label_02EA:
                Singleton<CResourceManager>.GetInstance().RemoveCachedResource(this.<fullPath>__7);
                this.$current = new CHoldForSecond(0f);
                this.$PC = 4;
                goto Label_04C1;
            Label_041E:
                this.<i>__1++;
            Label_042C:
                if (this.<i>__1 < this.<>f__this.levelArtistList.Count)
                {
                    this.<artAssetNameHigh>__2 = this.<>f__this.levelArtistList[this.<i>__1] + "/" + this.<>f__this.levelArtistList[this.<i>__1];
                    this.<artAssetNameMid>__3 = this.<artAssetNameHigh>__2.Replace("_High", "_Mid");
                    this.<artAssetNameLow>__4 = this.<artAssetNameHigh>__2.Replace("_High", "_Low");
                    this.<levelNames>__5 = new string[] { this.<artAssetNameHigh>__2, this.<artAssetNameMid>__3, this.<artAssetNameLow>__4 };
                    this.<levelArtist>__6 = null;
                    this.<fullPath>__7 = string.Empty;
                    this.<lod>__8 = GameSettings.ModelLOD;
                    this.<lod>__8 = Mathf.Clamp(this.<lod>__8, 0, 2);
                    while (this.<lod>__8 >= 0)
                    {
                        this.<fullPath>__7 = "SceneExport/Artist/" + this.<levelNames>__5[this.<lod>__8] + ".asset";
                        this.<levelArtist>__6 = Singleton<CResourceManager>.GetInstance().GetResource(this.<fullPath>__7, typeof(LevelResAsset), enResourceType.BattleScene, false, true).m_content;
                        this.$current = new CHoldForSecond(0f);
                        this.$PC = 1;
                        goto Label_04C1;
                    Label_01A5:
                        this.<lod>__8--;
                    }
                    goto Label_01BF;
                }
                Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("Scene");
                this.<duty>__14 = this.InWrapper.duty;
                this.<>f__this.nProgress = this.<oldProgress>__0 + this.<duty>__14;
                this.<>f__this.LoadProgressEvent(this.<>f__this.nProgress * 0.0001f);
                this.$current = new CWaitForNextFrame();
                this.$PC = 7;
                goto Label_04C1;
            Label_04BF:
                return false;
            Label_04C1:
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
        private sealed class <LoadCommonAssetBundle>c__Iterator14 : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal GameLoader.LoaderHelperWrapper <$>InWrapper;
            internal GameLoader <>f__this;
            internal int <duty>__0;
            internal int <oldProgress>__1;
            internal GameLoader.LoaderHelperWrapper InWrapper;

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
                        this.<duty>__0 = this.InWrapper.duty;
                        this.<oldProgress>__1 = this.<>f__this.nProgress;
                        Singleton<CResourceManager>.GetInstance().LoadAssetBundle("AssetBundle/Hero_CommonRes.assetbundle");
                        this.$current = new CHoldForSecond(0f);
                        this.$PC = 1;
                        goto Label_017C;

                    case 1:
                        Singleton<CResourceManager>.GetInstance().LoadAssetBundle("AssetBundle/Skill_CommonEffect1.assetbundle");
                        this.$current = new CHoldForSecond(0f);
                        this.$PC = 2;
                        goto Label_017C;

                    case 2:
                        Singleton<CResourceManager>.GetInstance().LoadAssetBundle("AssetBundle/Skill_CommonEffect2.assetbundle");
                        this.$current = new CHoldForSecond(0f);
                        this.$PC = 3;
                        goto Label_017C;

                    case 3:
                        Singleton<CResourceManager>.GetInstance().LoadAssetBundle("AssetBundle/Systems_Effects.assetbundle");
                        this.$current = new CHoldForSecond(0f);
                        this.$PC = 4;
                        goto Label_017C;

                    case 4:
                        Singleton<CResourceManager>.GetInstance().LoadAssetBundle("AssetBundle/UGUI_Talent.assetbundle");
                        Singleton<CResourceManager>.GetInstance().LoadAssetBundle("AssetBundle/UGUI_Map.assetbundle");
                        this.<>f__this.nProgress = this.<oldProgress>__1 + this.<duty>__0;
                        this.<>f__this.LoadProgressEvent(this.<>f__this.nProgress * 0.0001f);
                        this.$current = new CHoldForSecond(0f);
                        this.$PC = 5;
                        goto Label_017C;

                    case 5:
                        this.$PC = -1;
                        break;
                }
                return false;
            Label_017C:
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
        private sealed class <LoadCommonAssets>c__Iterator18 : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal GameLoader.LoaderHelperWrapper <$>InWrapper;
            internal GameLoader <>f__this;
            internal int <duty>__2;
            internal int <i>__1;
            internal int <oldProgress>__0;
            internal GameLoader.LoaderHelperWrapper InWrapper;

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
                        this.<oldProgress>__0 = this.<>f__this.nProgress;
                        this.<i>__1 = 0;
                        break;

                    case 1:
                        this.<i>__1++;
                        break;

                    case 2:
                        this.<duty>__2 = this.InWrapper.duty;
                        this.<>f__this.nProgress = this.<oldProgress>__0 + this.<duty>__2;
                        this.<>f__this.LoadProgressEvent(this.<>f__this.nProgress * 0.0001f);
                        this.$current = new CWaitForNextFrame();
                        this.$PC = 3;
                        goto Label_013E;

                    case 3:
                        this.$PC = -1;
                        goto Label_013C;

                    default:
                        goto Label_013C;
                }
                if (this.<i>__1 < this.<>f__this.soundBankList.Count)
                {
                    Singleton<CSoundManager>.instance.LoadBank(this.<>f__this.soundBankList[this.<i>__1], CSoundManager.BankType.Battle);
                    this.$current = new CHoldForSecond(0f);
                    this.$PC = 1;
                }
                else
                {
                    MonoSingleton<SceneMgr>.instance.PreloadCommonAssets();
                    this.$current = new CHoldForSecond(0f);
                    this.$PC = 2;
                }
                goto Label_013E;
            Label_013C:
                return false;
            Label_013E:
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
        private sealed class <LoadCommonEffect>c__Iterator15 : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal GameLoader.LoaderHelperWrapper <$>InWrapper;
            internal GameLoader <>f__this;
            internal int <duty>__0;
            internal int <oldProgress>__1;
            internal GameLoader.LoaderHelperWrapper InWrapper;

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
                        this.<duty>__0 = this.InWrapper.duty;
                        this.<oldProgress>__1 = this.<>f__this.nProgress;
                        Singleton<CCoroutineManager>.GetInstance().StartCoroutine(MonoSingleton<SceneMgr>.instance.PreloadCommonEffects());
                        this.<>f__this.nProgress = this.<oldProgress>__1 + this.<duty>__0;
                        this.<>f__this.LoadProgressEvent(this.<>f__this.nProgress * 0.0001f);
                        this.$current = new CWaitForNextFrame();
                        this.$PC = 1;
                        return true;

                    case 1:
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
        private sealed class <LoadDesignLevel>c__Iterator17 : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal GameLoader.LoaderHelperWrapper <$>InWrapper;
            internal GameLoader <>f__this;
            internal CBinaryObject <binaryObject>__6;
            internal string <desgineAssetNameHigh>__2;
            internal string <desgineAssetNameLow>__4;
            internal string <desgineAssetNameMid>__3;
            internal GameObject <designRoot>__10;
            internal int <duty>__12;
            internal string <fullPath>__7;
            internal ObjectHolder <holder>__9;
            internal int <i>__1;
            internal string[] <levelNames>__5;
            internal int <lod>__8;
            internal int <oldProgress>__0;
            internal Transform <staticRoot>__11;
            internal GameLoader.LoaderHelperWrapper InWrapper;

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
                        this.<oldProgress>__0 = this.<>f__this.nProgress;
                        this.<i>__1 = 0;
                        goto Label_02F4;

                    case 1:
                        if (null == this.<binaryObject>__6)
                        {
                            goto Label_01A1;
                        }
                        goto Label_01BB;

                    case 2:
                        this.<designRoot>__10 = this.<holder>__9.obj as GameObject;
                        if (null != this.<designRoot>__10)
                        {
                            this.<staticRoot>__11 = this.<designRoot>__10.get_transform().Find("StaticMesh");
                            if (null != this.<staticRoot>__11)
                            {
                                StaticBatchingUtility.Combine(this.<staticRoot>__11.get_gameObject());
                                this.$current = new CHoldForSecond(0f);
                                this.$PC = 3;
                                goto Label_03CB;
                            }
                            goto Label_02D6;
                        }
                        Debug.LogError("策划场景SceneExport/Design/" + this.<>f__this.levelDesignList[this.<i>__1] + ".bytes有错误！请检查！");
                        goto Label_02E6;

                    case 3:
                        goto Label_02D6;

                    case 4:
                        Singleton<SceneManagement>.GetInstance().InitScene();
                        this.$current = new CHoldForSecond(0f);
                        this.$PC = 5;
                        goto Label_03CB;

                    case 5:
                        this.<duty>__12 = this.InWrapper.duty;
                        this.<>f__this.nProgress = this.<oldProgress>__0 + this.<duty>__12;
                        this.<>f__this.LoadProgressEvent(this.<>f__this.nProgress * 0.0001f);
                        this.$current = new CWaitForNextFrame();
                        this.$PC = 6;
                        goto Label_03CB;

                    case 6:
                        this.$PC = -1;
                        goto Label_03C9;

                    default:
                        goto Label_03C9;
                }
            Label_01BB:
                if (null == this.<binaryObject>__6)
                {
                    Debug.LogError("错误，没有找到导出的策划场景" + this.<desgineAssetNameHigh>__2);
                    goto Label_02E6;
                }
                this.<holder>__9 = new ObjectHolder();
                this.$current = Singleton<CCoroutineManager>.GetInstance().StartCoroutine(GameLoader.s_serializer.LoadAsync(this.<binaryObject>__6.m_data, this.<holder>__9));
                this.$PC = 2;
                goto Label_03CB;
            Label_02D6:
                Singleton<CResourceManager>.GetInstance().RemoveCachedResource(this.<fullPath>__7);
            Label_02E6:
                this.<i>__1++;
            Label_02F4:
                if (this.<i>__1 < this.<>f__this.levelDesignList.Count)
                {
                    this.<desgineAssetNameHigh>__2 = this.<>f__this.levelDesignList[this.<i>__1] + "/" + this.<>f__this.levelDesignList[this.<i>__1];
                    this.<desgineAssetNameMid>__3 = this.<desgineAssetNameHigh>__2.Replace("_High", "_Mid");
                    this.<desgineAssetNameLow>__4 = this.<desgineAssetNameHigh>__2.Replace("_High", "_Low");
                    this.<levelNames>__5 = new string[] { this.<desgineAssetNameHigh>__2, this.<desgineAssetNameMid>__3, this.<desgineAssetNameLow>__4 };
                    this.<binaryObject>__6 = null;
                    this.<fullPath>__7 = string.Empty;
                    this.<lod>__8 = GameSettings.ModelLOD;
                    this.<lod>__8 = Mathf.Clamp(this.<lod>__8, 0, 2);
                    while (this.<lod>__8 >= 0)
                    {
                        this.<fullPath>__7 = "SceneExport/Design/" + this.<levelNames>__5[this.<lod>__8] + ".bytes";
                        this.<binaryObject>__6 = Singleton<CResourceManager>.GetInstance().GetResource(this.<fullPath>__7, typeof(TextAsset), enResourceType.BattleScene, false, true).m_content as CBinaryObject;
                        this.$current = new CHoldForSecond(0f);
                        this.$PC = 1;
                        goto Label_03CB;
                    Label_01A1:
                        this.<lod>__8--;
                    }
                    goto Label_01BB;
                }
                Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("Scene");
                this.$current = new CHoldForSecond(0f);
                this.$PC = 4;
                goto Label_03CB;
            Label_03C9:
                return false;
            Label_03CB:
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
        private sealed class <LoadNoActorAssets>c__Iterator1D : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal GameLoader.LHCWrapper <$>InWrapper;
            internal GameLoader <>f__this;
            internal string <assetPath>__8;
            internal int <count>__4;
            internal int <currentParticleLOD>__9;
            internal int <duty>__1;
            internal GameObject <gameObj>__18;
            internal int <index>__5;
            internal int <j>__13;
            internal int <j>__16;
            internal int <j>__6;
            internal LoaderHelperCamera <lhc>__0;
            internal LoaderHelper <loadHelper>__2;
            internal int <lod>__10;
            internal int <oldProgress>__3;
            internal string <parPathKey>__11;
            internal string <parPathKey>__12;
            internal SpriteRenderer <sr>__19;
            internal CResource <tempObj>__17;
            internal ListView<Texture> <textures>__15;
            internal GameObject <tmpObj>__14;
            internal GameObject <tmpObj>__7;
            internal GameLoader.LHCWrapper InWrapper;

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
                        this.<lhc>__0 = this.InWrapper.lhc;
                        this.<duty>__1 = this.InWrapper.duty;
                        this.<loadHelper>__2 = this.InWrapper.loadHelper;
                        this.<oldProgress>__3 = this.<>f__this.nProgress;
                        this.<count>__4 = (this.<>f__this.noActorPreLoad.parPrefabs.Count + this.<>f__this.noActorPreLoad.mesPrefabs.Count) + this.<>f__this.noActorPreLoad.spritePrefabs.Count;
                        this.<index>__5 = 0;
                        this.<j>__6 = 0;
                        goto Label_02F6;

                    case 1:
                        goto Label_01F9;

                    case 2:
                        goto Label_0295;

                    case 3:
                        this.<j>__6++;
                        goto Label_02F6;

                    case 4:
                        goto Label_03F2;

                    case 5:
                        goto Label_0445;

                    case 6:
                        goto Label_04F2;

                    case 7:
                        this.<index>__5++;
                        this.<>f__this.UpdateProgress(this.<lhc>__0, this.<oldProgress>__3, this.<duty>__1, this.<index>__5, this.<count>__4);
                        this.$current = new CHoldForSecond(0f);
                        this.$PC = 8;
                        goto Label_06BE;

                    case 8:
                        goto Label_061B;

                    case 9:
                        this.<>f__this.UpdateProgress(this.<lhc>__0, this.<oldProgress>__3, this.<duty>__1, 1, 1);
                        this.$PC = -1;
                        goto Label_06BC;

                    default:
                        goto Label_06BC;
                }
            Label_0295:
                this.<index>__5++;
                this.<>f__this.UpdateProgress(this.<lhc>__0, this.<oldProgress>__3, this.<duty>__1, this.<index>__5, this.<count>__4);
                this.$current = new CHoldForSecond(0f);
                this.$PC = 3;
                goto Label_06BE;
            Label_02F6:
                if (this.<j>__6 < this.<>f__this.noActorPreLoad.parPrefabs.Count)
                {
                    this.<tmpObj>__7 = null;
                    AssetLoadBase base2 = this.<>f__this.noActorPreLoad.parPrefabs[this.<j>__6];
                    this.<assetPath>__8 = base2.assetPath;
                    if (!this.<lhc>__0.HasLoaded(this.<assetPath>__8))
                    {
                        if (GameSettings.DynamicParticleLOD)
                        {
                            this.<currentParticleLOD>__9 = GameSettings.ParticleLOD;
                            this.<lod>__10 = this.<currentParticleLOD>__9;
                            while (this.<lod>__10 <= 2)
                            {
                                if ((this.<currentParticleLOD>__9 != 0) || (this.<lod>__10 != 1))
                                {
                                    this.<parPathKey>__11 = this.<assetPath>__8 + "_lod_" + this.<lod>__10;
                                    if (!this.<lhc>__0.HasLoaded(this.<parPathKey>__11))
                                    {
                                        GameSettings.ParticleLOD = this.<lod>__10;
                                        this.<tmpObj>__7 = MonoSingleton<SceneMgr>.instance.GetPooledGameObjLOD(this.<assetPath>__8, true, SceneObjType.ActionRes, Vector3.get_zero());
                                        this.<lhc>__0.AddObj(this.<parPathKey>__11, this.<tmpObj>__7);
                                        this.$current = new CHoldForSecond(0f);
                                        this.$PC = 1;
                                        goto Label_06BE;
                                    }
                                }
                            Label_01F9:
                                this.<lod>__10++;
                            }
                            GameSettings.ParticleLOD = this.<currentParticleLOD>__9;
                        }
                        else
                        {
                            this.<parPathKey>__12 = this.<assetPath>__8;
                            if (!this.<lhc>__0.HasLoaded(this.<parPathKey>__12))
                            {
                                this.<tmpObj>__7 = MonoSingleton<SceneMgr>.instance.GetPooledGameObjLOD(this.<assetPath>__8, true, SceneObjType.ActionRes, Vector3.get_zero());
                                this.<lhc>__0.AddObj(this.<parPathKey>__12, this.<tmpObj>__7);
                                this.$current = new CHoldForSecond(0f);
                                this.$PC = 2;
                                goto Label_06BE;
                            }
                        }
                    }
                    goto Label_0295;
                }
                this.<>f__this.ReleaseMemoryIfNeed();
                this.<j>__13 = 0;
                while (this.<j>__13 < this.<>f__this.noActorPreLoad.mesPrefabs.Count)
                {
                    this.<tmpObj>__14 = null;
                    AssetLoadBase base3 = this.<>f__this.noActorPreLoad.mesPrefabs[this.<j>__13];
                    if (!this.<lhc>__0.HasLoaded(base3.assetPath))
                    {
                        AssetLoadBase base4 = this.<>f__this.noActorPreLoad.mesPrefabs[this.<j>__13];
                        this.<tmpObj>__14 = MonoSingleton<SceneMgr>.instance.GetPooledGameObjLOD(base4.assetPath, false, SceneObjType.ActionRes, Vector3.get_zero());
                        AssetLoadBase base5 = this.<>f__this.noActorPreLoad.mesPrefabs[this.<j>__13];
                        this.<lhc>__0.AddObj(base5.assetPath, this.<tmpObj>__14);
                        this.$current = new CHoldForSecond(0f);
                        this.$PC = 4;
                        goto Label_06BE;
                    }
                Label_03F2:
                    this.<index>__5++;
                    this.<>f__this.UpdateProgress(this.<lhc>__0, this.<oldProgress>__3, this.<duty>__1, this.<index>__5, this.<count>__4);
                    this.$current = new CHoldForSecond(0f);
                    this.$PC = 5;
                    goto Label_06BE;
                Label_0445:
                    this.<j>__13++;
                }
                this.<>f__this.ReleaseMemoryIfNeed();
                this.<textures>__15 = new ListView<Texture>();
                this.<j>__16 = 0;
                while (this.<j>__16 < this.<>f__this.noActorPreLoad.spritePrefabs.Count)
                {
                    AssetLoadBase base6 = this.<>f__this.noActorPreLoad.spritePrefabs[this.<j>__16];
                    this.<tempObj>__17 = Singleton<CResourceManager>.instance.GetResource(base6.assetPath, typeof(GameObject), enResourceType.UIPrefab, true, false);
                    this.$current = new CHoldForSecond(0f);
                    this.$PC = 6;
                    goto Label_06BE;
                Label_04F2:
                    if (((this.<tempObj>__17 != null) && (this.<tempObj>__17.m_content != null)) && (this.<tempObj>__17.m_content is GameObject))
                    {
                        this.<gameObj>__18 = this.<tempObj>__17.m_content;
                        this.<sr>__19 = this.<gameObj>__18.GetComponent<SpriteRenderer>();
                        if (((this.<sr>__19 != null) && (this.<sr>__19.get_sprite() != null)) && (this.<sr>__19.get_sprite().get_texture() != null))
                        {
                            this.<textures>__15.Add(this.<sr>__19.get_sprite().get_texture());
                        }
                    }
                    this.$current = new CHoldForSecond(0f);
                    this.$PC = 7;
                    goto Label_06BE;
                Label_061B:
                    this.<j>__16++;
                }
                this.<textures>__15.Clear();
                this.<>f__this.ReleaseMemoryIfNeed();
                this.<>f__this.UpdateProgress(this.<lhc>__0, this.<oldProgress>__3, this.<duty>__1, 1, 1);
                this.$current = new CWaitForNextFrame();
                this.$PC = 9;
                goto Label_06BE;
            Label_06BC:
                return false;
            Label_06BE:
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
        private sealed class <PreSpawnSoldiers>c__Iterator22 : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal GameLoader.LoaderHelperWrapper <$>InWrapper;
            internal GameLoader <>f__this;
            internal ActorMeta <actorMeta>__9;
            internal string <actorName>__11;
            internal int <count>__10;
            internal int <duty>__1;
            internal int <i>__3;
            internal int <i>__7;
            internal int <j>__12;
            internal PoolObjHandle<ActorRoot> <monster>__13;
            internal ActorRoot <monsterActor>__14;
            internal int <num>__5;
            internal int <oldProgress>__0;
            internal ActorPreloadTab <preloadTab>__4;
            internal ActorPreloadTab <preloadTab>__8;
            internal int <spawnCountTotal>__2;
            internal int <spawnIndex>__6;
            internal GameLoader.LoaderHelperWrapper InWrapper;

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
                        this.<oldProgress>__0 = this.<>f__this.nProgress;
                        this.<duty>__1 = this.InWrapper.duty;
                        this.<spawnCountTotal>__2 = 0;
                        this.<i>__3 = 0;
                        while (this.<i>__3 < this.<>f__this.actorPreload.Count)
                        {
                            this.<preloadTab>__4 = this.<>f__this.actorPreload[this.<i>__3];
                            this.<num>__5 = Mathf.Max(Mathf.RoundToInt(this.<preloadTab>__4.spawnCnt), 1);
                            if (this.<preloadTab>__4.theActor.ActorType == ActorTypeDef.Actor_Type_Monster)
                            {
                                this.<spawnCountTotal>__2 += this.<num>__5;
                            }
                            this.<i>__3++;
                        }
                        GameObjMgr.isPreSpawnActors = true;
                        this.<spawnIndex>__6 = 0;
                        this.<i>__7 = 0;
                        while (this.<i>__7 < this.<>f__this.actorPreload.Count)
                        {
                            this.<preloadTab>__8 = this.<>f__this.actorPreload[this.<i>__7];
                            this.<actorMeta>__9 = this.<preloadTab>__8.theActor;
                            if (this.<actorMeta>__9.ActorType == ActorTypeDef.Actor_Type_Monster)
                            {
                                this.<count>__10 = Mathf.Max(Mathf.RoundToInt(this.<preloadTab>__8.spawnCnt), 1);
                                this.<actorName>__11 = null;
                                this.<j>__12 = 0;
                                while (this.<j>__12 < this.<count>__10)
                                {
                                    this.<monster>__13 = Singleton<GameObjMgr>.GetInstance().SpawnActorEx(null, ref this.<actorMeta>__9, VInt3.zero, VInt3.forward, false, true);
                                    this.$current = new CHoldForSecond(0f);
                                    this.$PC = 1;
                                    goto Label_03F6;
                                Label_027C:
                                    if (this.<actorName>__11 == null)
                                    {
                                        this.<actorName>__11 = this.<monsterActor>__14.TheStaticData.TheResInfo.Name;
                                    }
                                    Singleton<GameObjMgr>.instance.AddToCache(this.<monster>__13);
                                Label_02B3:
                                    this.<>f__this.UpdateProgress(null, this.<oldProgress>__0, this.<duty>__1, ++this.<spawnIndex>__6, this.<spawnCountTotal>__2);
                                    this.$current = new CHoldForSecond(0f);
                                    this.$PC = 5;
                                    goto Label_03F6;
                                Label_02FE:
                                    this.<j>__12++;
                                }
                            }
                            this.<i>__7++;
                        }
                        this.<>f__this.nProgress = this.<oldProgress>__0 + this.<duty>__1;
                        this.<>f__this.LoadProgressEvent(this.<>f__this.nProgress * 0.0001f);
                        this.$current = new CHoldForSecond(0f);
                        this.$PC = 6;
                        goto Label_03F6;

                    case 1:
                        if (this.<monster>__13 == 0)
                        {
                            goto Label_02B3;
                        }
                        this.<monsterActor>__14 = this.<monster>__13.handle;
                        this.<monsterActor>__14.InitActor();
                        this.$current = new CHoldForSecond(0f);
                        this.$PC = 2;
                        goto Label_03F6;

                    case 2:
                        this.<monsterActor>__14.PrepareFight();
                        this.$current = new CHoldForSecond(0f);
                        this.$PC = 3;
                        goto Label_03F6;

                    case 3:
                        this.<monsterActor>__14.gameObject.set_name(this.<monsterActor>__14.TheStaticData.TheResInfo.Name);
                        this.<monsterActor>__14.StartFight();
                        this.$current = new CHoldForSecond(0f);
                        this.$PC = 4;
                        goto Label_03F6;

                    case 4:
                        goto Label_027C;

                    case 5:
                        goto Label_02FE;

                    case 6:
                        GameObjMgr.isPreSpawnActors = false;
                        HudComponent3D.PreallocMapPointer(20, 40);
                        this.$current = new CHoldForSecond(0f);
                        this.$PC = 7;
                        goto Label_03F6;

                    case 7:
                        SObjPool<SRefParam>.Alloc(0x400);
                        this.$current = new CHoldForSecond(0f);
                        this.$PC = 8;
                        goto Label_03F6;

                    case 8:
                        this.$PC = -1;
                        break;
                }
                return false;
            Label_03F6:
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
        private sealed class <SpawnDynamicActor>c__Iterator21 : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal GameLoader.LoaderHelperWrapper <$>InWrapper;
            internal GameLoader <>f__this;
            internal PoolObjHandle<ActorRoot> <actor>__6;
            internal ActorMeta <actorMeta>__3;
            internal VInt3 <bornDir>__5;
            internal VInt3 <bornPos>__4;
            internal int <duty>__0;
            internal int <i>__2;
            internal int <oldProgress>__1;
            internal GameLoader.LoaderHelperWrapper InWrapper;

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
                        this.<duty>__0 = this.InWrapper.duty;
                        this.<oldProgress>__1 = this.<>f__this.nProgress;
                        this.<i>__2 = 0;
                        goto Label_01DD;

                    case 1:
                        goto Label_00FE;

                    case 2:
                        if (this.<actor>__6 != 0)
                        {
                            Singleton<GameObjMgr>.GetInstance().HoldDynamicActor(this.<actor>__6);
                        }
                        this.<>f__this.nProgress = this.<oldProgress>__1 + ((this.<duty>__0 * (this.<i>__2 + 1)) / this.<>f__this.actorList.Count);
                        this.<>f__this.LoadProgressEvent(this.<>f__this.nProgress * 0.0001f);
                        this.$current = new CHoldForSecond(0f);
                        this.$PC = 3;
                        goto Label_0257;

                    case 3:
                        this.<i>__2++;
                        goto Label_01DD;

                    case 4:
                        this.$PC = -1;
                        goto Label_0255;

                    default:
                        goto Label_0255;
                }
            Label_00FE:
                this.<actor>__6 = Singleton<GameObjMgr>.instance.SpawnActorEx(null, ref this.<actorMeta>__3, this.<bornPos>__4, this.<bornDir>__5, false, true);
                this.$current = new CHoldForSecond(0f);
                this.$PC = 2;
                goto Label_0257;
            Label_01DD:
                if (this.<i>__2 < this.<>f__this.actorList.Count)
                {
                    this.<actorMeta>__3 = this.<>f__this.actorList[this.<i>__2];
                    this.<bornPos>__4 = new VInt3();
                    this.<bornDir>__5 = new VInt3();
                    if (this.<actorMeta>__3.ActorType == ActorTypeDef.Actor_Type_Hero)
                    {
                        DebugHelper.Assert(Singleton<BattleLogic>.instance.mapLogic != null, "what? BattleLogic.instance.mapLogic==null");
                        Singleton<BattleLogic>.GetInstance().mapLogic.GetRevivePosDir(ref this.<actorMeta>__3, true, out this.<bornPos>__4, out this.<bornDir>__5);
                        this.$current = new CHoldForSecond(0f);
                        this.$PC = 1;
                        goto Label_0257;
                    }
                    goto Label_00FE;
                }
                this.<>f__this.nProgress = this.<oldProgress>__1 + this.<duty>__0;
                this.<>f__this.LoadProgressEvent(this.<>f__this.nProgress * 0.0001f);
                this.$current = new CHoldForSecond(0f);
                this.$PC = 4;
                goto Label_0257;
            Label_0255:
                return false;
            Label_0257:
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
        private sealed class <SpawnStaticActor>c__Iterator20 : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal GameLoader.LoaderHelperWrapper <$>InWrapper;
            internal GameLoader <>f__this;
            internal PoolObjHandle<ActorRoot> <actor>__6;
            internal ActorMeta <actorMeta>__3;
            internal VInt3 <bornDir>__5;
            internal VInt3 <bornPos>__4;
            internal int <duty>__0;
            internal int <i>__2;
            internal int <oldProgress>__1;
            internal GameLoader.LoaderHelperWrapper InWrapper;

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
                        this.<duty>__0 = this.InWrapper.duty;
                        this.<oldProgress>__1 = this.<>f__this.nProgress;
                        this.<i>__2 = 0;
                        break;

                    case 1:
                        if (this.<actor>__6 != 0)
                        {
                            Singleton<GameObjMgr>.GetInstance().HoldStaticActor(this.<actor>__6);
                        }
                        this.<>f__this.nProgress = this.<oldProgress>__1 + ((this.<duty>__0 * (this.<i>__2 + 1)) / this.<>f__this.staticActors.Count);
                        this.<>f__this.LoadProgressEvent(this.<>f__this.nProgress * 0.0001f);
                        this.$current = new CHoldForSecond(0f);
                        this.$PC = 2;
                        goto Label_02B1;

                    case 2:
                        this.<i>__2++;
                        break;

                    case 3:
                        this.<>f__this.staticActors.Clear();
                        this.$PC = -1;
                        goto Label_02AF;

                    default:
                        goto Label_02AF;
                }
                if (this.<i>__2 < this.<>f__this.staticActors.Count)
                {
                    this.<actorMeta>__3 = new ActorMeta();
                    this.<actorMeta>__3.ActorType = this.<>f__this.staticActors[this.<i>__2].ActorType;
                    this.<actorMeta>__3.ConfigId = this.<>f__this.staticActors[this.<i>__2].ConfigID;
                    this.<actorMeta>__3.ActorCamp = this.<>f__this.staticActors[this.<i>__2].CmpType;
                    this.<bornPos>__4 = this.<>f__this.staticActors[this.<i>__2].get_transform().get_position();
                    this.<bornDir>__5 = this.<>f__this.staticActors[this.<i>__2].get_transform().get_forward();
                    this.<actor>__6 = Singleton<GameObjMgr>.instance.SpawnActorEx(this.<>f__this.staticActors[this.<i>__2].get_gameObject(), ref this.<actorMeta>__3, this.<bornPos>__4, this.<bornDir>__5, false, true);
                    this.$current = new CHoldForSecond(0f);
                    this.$PC = 1;
                }
                else
                {
                    this.<>f__this.nProgress = this.<oldProgress>__1 + this.<duty>__0;
                    this.<>f__this.LoadProgressEvent(this.<>f__this.nProgress * 0.0001f);
                    this.$current = new CHoldForSecond(0f);
                    this.$PC = 3;
                }
                goto Label_02B1;
            Label_02AF:
                return false;
            Label_02B1:
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

        [StructLayout(LayoutKind.Sequential)]
        private struct LHCWrapper
        {
            public LoaderHelperCamera lhc;
            public LoaderHelper loadHelper;
            public int duty;
        }

        public delegate void LoadCompleteDelegate();

        [StructLayout(LayoutKind.Sequential)]
        private struct LoaderHelperWrapper
        {
            public LoaderHelper loadHelper;
            public int duty;
        }

        public delegate void LoadProgressDelegate(float progress);
    }
}

