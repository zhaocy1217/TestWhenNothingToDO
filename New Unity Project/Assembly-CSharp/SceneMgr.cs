using AGE;
using Assets.Scripts.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

public class SceneMgr : MonoSingleton<SceneMgr>
{
    [CompilerGenerated]
    private static Predicate<string> <>f__am$cache8;
    private DictionaryView<string, GameObject> cachedPrefabs = new DictionaryView<string, GameObject>();
    private object[] commonEffects;
    private string[] emptyActorPrefabs;
    private int[] LIMIT_CONFIG = new int[] { 40, 40 };
    private const string LIMIT_CONFIG_FILE = "Config/ParticleLimit";
    public static string[] lod_postfix = new string[] { string.Empty, "_mid", "_low" };
    public bool m_dynamicLOD;
    private Dictionary<string, bool> m_resourcesNotExist = new Dictionary<string, bool>();
    private GameObject[] rootObjs;

    public SceneMgr()
    {
        string[] textArray1 = new string[7];
        textArray1[0] = "Prefab_Characters/EmptyHero";
        textArray1[1] = "Prefab_Characters/EmptyMonster";
        textArray1[3] = "Prefab_Characters/EmptyEye";
        textArray1[4] = "Prefab_Characters/EmptyBullet";
        this.emptyActorPrefabs = textArray1;
        this.commonEffects = new object[] { "Prefab_Skill_Effects/tongyong_effects/Indicator/lockt_01", 3, "Prefab_Skill_Effects/tongyong_effects/Siwang_tongyong/siwang_tongyong_01", 5, "Prefab_Skill_Effects/tongyong_effects/Indicator/select_b_01", 10, "prefab_skill_effects/tongyong_effects/tongyong_hurt/born_back_reborn/huicheng_tongyong_01", 5, "prefab_skill_effects/common_effects/jiasu_tongyong_01", 8 };
    }

    private GameObject _GetPooledGameObjLOD(string prefabName, bool isParticle, SceneObjType sceneObjType, Vector3 pos, Quaternion rot, bool useRotation, out bool isInit)
    {
        string realPath = null;
        isInit = false;
        if (this.m_resourcesNotExist.ContainsKey(prefabName))
        {
            return null;
        }
        this.GetPrefabLOD<GameObject>(prefabName, isParticle, out realPath);
        GameObject obj2 = null;
        if (useRotation)
        {
            obj2 = Singleton<CGameObjectPool>.GetInstance().GetGameObject(realPath, pos, rot, enResourceType.BattleScene, out isInit);
        }
        else
        {
            obj2 = Singleton<CGameObjectPool>.GetInstance().GetGameObject(realPath, pos, enResourceType.BattleScene, out isInit);
        }
        if (obj2 != null)
        {
            obj2.get_transform().SetParent(this.rootObjs[(int) sceneObjType].get_transform());
            obj2.get_transform().set_position(pos);
        }
        return obj2;
    }

    public GameObject AddCulling(GameObject obj, string name = "CullingParent")
    {
        if (null == obj)
        {
            return null;
        }
        GameObject obj2 = new GameObject(name);
        if (null == obj.get_transform().get_parent())
        {
            obj2.get_transform().set_position(obj.get_transform().get_position());
            obj2.get_transform().set_rotation(obj.get_transform().get_rotation());
        }
        else
        {
            obj2.get_transform().set_parent(obj.get_transform().get_parent());
            obj2.get_transform().set_localPosition(obj.get_transform().get_localPosition());
            obj2.get_transform().set_localRotation(obj.get_transform().get_localRotation());
        }
        obj.get_transform().set_parent(obj2.get_transform());
        ObjectCulling component = obj2.GetComponent<ObjectCulling>();
        if (null == component)
        {
            component = obj2.AddComponent<ObjectCulling>();
        }
        component.Init(obj);
        return obj2;
    }

    public void AddToRoot(GameObject obj, SceneObjType sceneObjType)
    {
        if (obj != null)
        {
            obj.get_transform().set_parent(this.rootObjs[(int) sceneObjType].get_transform());
        }
    }

    public void AddToRoot(GameObject obj, SceneObjType sceneObjType, Vector3 pos, Quaternion rot)
    {
        if (obj != null)
        {
            obj.get_transform().set_parent(this.rootObjs[(int) sceneObjType].get_transform());
            obj.get_transform().set_position(pos);
            obj.get_transform().set_rotation(rot);
        }
    }

    public void ClearAll()
    {
        if (ActionManager.Instance != null)
        {
            ActionManager.Instance.ForceStop();
        }
        if (this.rootObjs != null)
        {
            for (int i = 0; i < this.rootObjs.Length; i++)
            {
                this.ClearObjs((SceneObjType) i);
            }
        }
        this.cachedPrefabs.Clear();
        this.m_resourcesNotExist.Clear();
        UpdateShadowPlane.ClearCache();
        base.StartCoroutine(this.UnloadAssets_Coroutine());
    }

    private void ClearObjs(SceneObjType type)
    {
        GameObject obj2 = this.rootObjs[(int) type];
        while (obj2.get_transform().get_childCount() > 0)
        {
            GameObject obj3 = obj2.get_transform().GetChild(obj2.get_transform().get_childCount() - 1).get_gameObject();
            obj3.get_transform().set_parent(null);
            Object.Destroy(obj3);
        }
    }

    private int GetDynamicLod(int lod, bool isParticle)
    {
        if ((((this.m_dynamicLOD && isParticle) && ((lod != 2) && (this.LIMIT_CONFIG != null))) && ((lod < this.LIMIT_CONFIG.Length) && (lod >= 0))) && (ParticleHelper.GetParticleActiveNumber() >= this.LIMIT_CONFIG[lod]))
        {
            return 2;
        }
        return lod;
    }

    public GameObject GetPooledGameObjLOD(string prefabName, bool isParticle, SceneObjType sceneObjType, Vector3 pos)
    {
        bool isInit = false;
        return this.GetPooledGameObjLOD(prefabName, isParticle, sceneObjType, pos, out isInit);
    }

    public GameObject GetPooledGameObjLOD(string prefabName, bool isParticle, SceneObjType sceneObjType, Vector3 pos, Quaternion rot)
    {
        bool isInit = false;
        return this.GetPooledGameObjLOD(prefabName, isParticle, sceneObjType, pos, rot, out isInit);
    }

    public GameObject GetPooledGameObjLOD(string prefabName, bool isParticle, SceneObjType sceneObjType, Vector3 pos, out bool isInit)
    {
        return this._GetPooledGameObjLOD(prefabName, isParticle, sceneObjType, pos, Quaternion.get_identity(), false, out isInit);
    }

    public GameObject GetPooledGameObjLOD(string prefabName, bool isParticle, SceneObjType sceneObjType, Vector3 pos, Quaternion rot, out bool isInit)
    {
        return this._GetPooledGameObjLOD(prefabName, isParticle, sceneObjType, pos, rot, true, out isInit);
    }

    public Object GetPrefabLOD(string path, bool isParticle, out string realPath)
    {
        return this.GetPrefabLOD<Object>(path, isParticle, out realPath);
    }

    public T GetPrefabLOD<T>(string path, bool isParticle, out string realPath) where T: Object
    {
        int lod = !isParticle ? GameSettings.ModelLOD : GameSettings.ParticleLOD;
        if ((Singleton<BattleLogic>.GetInstance().GetCurLvelContext() != null) && !Singleton<BattleLogic>.GetInstance().GetCurLvelContext().IsMobaMode())
        {
            lod--;
        }
        lod = Mathf.Clamp(lod, 0, 2);
        if (GameSettings.DynamicParticleLOD)
        {
            lod = this.GetDynamicLod(lod, isParticle);
        }
        while (lod >= 0)
        {
            string str = path;
            string str2 = lod_postfix[lod];
            if (!string.IsNullOrEmpty(str2))
            {
                str = str + str2;
            }
            T local = this.LoadResource<T>(str);
            if (local != null)
            {
                realPath = str;
                return local;
            }
            lod--;
        }
        realPath = path;
        this.m_resourcesNotExist.Add(path, true);
        return null;
    }

    public GameObject GetRoot(SceneObjType sceneObjType)
    {
        return this.rootObjs[(int) sceneObjType];
    }

    protected override void Init()
    {
        this.InitObjs();
        this.InitConfig();
    }

    private void InitConfig()
    {
        CResource resource = Singleton<CResourceManager>.GetInstance().GetResource("Config/ParticleLimit", typeof(TextAsset), enResourceType.Numeric, false, true);
        if (resource != null)
        {
            CBinaryObject content = resource.m_content as CBinaryObject;
            if (null != content)
            {
                char[] separator = new char[] { '\r', '\n' };
                if (<>f__am$cache8 == null)
                {
                    <>f__am$cache8 = delegate (string x) {
                        return !string.IsNullOrEmpty(x);
                    };
                }
                string[] strArray = Array.FindAll<string>(StringHelper.ASCIIBytesToString(content.m_data).Split(separator), <>f__am$cache8);
                for (int i = 0; i < strArray.Length; i++)
                {
                    string str2 = strArray[i];
                    if (!string.IsNullOrEmpty(str2))
                    {
                        str2 = str2.Trim();
                        if (str2.Contains("//"))
                        {
                            str2 = str2.Substring(0, str2.IndexOf("//"));
                        }
                        str2 = str2.Trim();
                        if (!string.IsNullOrEmpty(str2))
                        {
                            char[] chArray2 = new char[] { ':' };
                            string[] strArray2 = str2.Split(chArray2);
                            if ((strArray2 == null) || (strArray2.Length != 2))
                            {
                                return;
                            }
                            int[] numArray = new int[2];
                            for (int j = 0; j < strArray2.Length; j++)
                            {
                                numArray[j] = Mathf.Abs(int.Parse(strArray2[j]));
                            }
                            if ((numArray[0] != 0) && (numArray[0] != 1))
                            {
                                return;
                            }
                            this.LIMIT_CONFIG[i] = numArray[1];
                        }
                    }
                }
            }
        }
    }

    private void InitObjs()
    {
        if (this.rootObjs == null)
        {
            string[] names = Enum.GetNames(typeof(SceneObjType));
            this.rootObjs = new GameObject[names.Length];
            for (int i = 0; i < names.Length; i++)
            {
                GameObject obj2 = new GameObject();
                obj2.get_transform().set_parent(base.get_gameObject().get_transform());
                obj2.set_name(names[i]);
                this.rootObjs[i] = obj2;
            }
        }
    }

    public GameObject InstantiateLOD(string prefabName, bool isParticle, SceneObjType sceneObjType)
    {
        string realPath = null;
        GameObject obj2 = this.GetPrefabLOD<GameObject>(prefabName, isParticle, out realPath);
        if (obj2 == null)
        {
            return null;
        }
        GameObject obj3 = Object.Instantiate(obj2) as GameObject;
        if (obj3 != null)
        {
            obj3.get_transform().set_parent(this.rootObjs[(int) sceneObjType].get_transform());
        }
        return obj3;
    }

    public GameObject InstantiateLOD(string prefabName, bool isParticle, SceneObjType sceneObjType, Vector3 pos)
    {
        GameObject obj2 = this.InstantiateLOD(prefabName, isParticle, sceneObjType);
        if (obj2 != null)
        {
            obj2.get_transform().set_position(pos);
        }
        return obj2;
    }

    public GameObject InstantiateLOD(string prefabName, bool isParticle, SceneObjType sceneObjType, Vector3 pos, Quaternion rot)
    {
        GameObject obj2 = this.InstantiateLOD(prefabName, isParticle, sceneObjType);
        if (obj2 != null)
        {
            obj2.get_transform().set_position(pos);
            obj2.get_transform().set_rotation(rot);
        }
        return obj2;
    }

    private T LoadResource<T>(string path) where T: Object
    {
        path = CFileManager.EraseExtension(path);
        return (Singleton<CResourceManager>.GetInstance().GetResource(path, typeof(T), enResourceType.BattleScene, true, false).m_content as T);
    }

    [DebuggerHidden]
    public IEnumerator PreloadCommonAssets()
    {
        <PreloadCommonAssets>c__IteratorD rd = new <PreloadCommonAssets>c__IteratorD();
        rd.<>f__this = this;
        return rd;
    }

    [DebuggerHidden]
    public IEnumerator PreloadCommonEffects()
    {
        <PreloadCommonEffects>c__IteratorE re = new <PreloadCommonEffects>c__IteratorE();
        re.<>f__this = this;
        return re;
    }

    public void PrepareGameObjectLOD(string path, bool isParticle, enResourceType type, int count)
    {
        string realPath = string.Empty;
        this.GetPrefabLOD(path, isParticle, out realPath);
        Singleton<CGameObjectPool>.instance.PrepareGameObject(realPath, type, count, true);
    }

    public GameObject Spawn(string name, SceneObjType sceneObjType)
    {
        GameObject obj2 = new GameObject();
        obj2.set_name(string.Format("{0}({1})", name, obj2.GetInstanceID()));
        obj2.get_transform().set_parent(this.rootObjs[(int) sceneObjType].get_transform());
        return obj2;
    }

    public GameObject Spawn(string name, SceneObjType sceneObjType, Vector3 position, Quaternion rotation)
    {
        GameObject obj2 = new GameObject();
        obj2.set_name(name);
        obj2.get_transform().set_position(position);
        obj2.get_transform().set_rotation(rotation);
        obj2.get_transform().set_parent(this.rootObjs[(int) sceneObjType].get_transform());
        return obj2;
    }

    public GameObject Spawn(string name, SceneObjType sceneObjType, Vector3 position, Vector3 forward)
    {
        GameObject obj2 = new GameObject();
        obj2.set_name(name);
        obj2.get_transform().set_position(position);
        obj2.get_transform().set_rotation(Quaternion.LookRotation(forward));
        obj2.get_transform().set_parent(this.rootObjs[(int) sceneObjType].get_transform());
        return obj2;
    }

    public GameObject Spawn(string name, SceneObjType sceneObjType, VInt3 position, VInt3 forward)
    {
        GameObject gameObject = null;
        string fullPathInResources = this.emptyActorPrefabs[(int) sceneObjType];
        if (fullPathInResources != null)
        {
            GameObject content = Singleton<CResourceManager>.GetInstance().GetResource(fullPathInResources, typeof(GameObject), enResourceType.BattleScene, true, false).m_content as GameObject;
            gameObject = Singleton<CGameObjectPool>.GetInstance().GetGameObject(fullPathInResources, enResourceType.BattleScene);
            if ((gameObject != null) && (content != null))
            {
                gameObject.set_layer(content.get_layer());
                gameObject.set_tag(content.get_tag());
            }
        }
        else
        {
            gameObject = new GameObject();
        }
        gameObject.set_name(name);
        gameObject.get_transform().set_position((Vector3) position);
        gameObject.get_transform().set_rotation(Quaternion.LookRotation((Vector3) forward));
        gameObject.get_transform().set_parent(this.rootObjs[(int) sceneObjType].get_transform());
        return gameObject;
    }

    [DebuggerHidden]
    private IEnumerator UnloadAssets_Coroutine()
    {
        return new <UnloadAssets_Coroutine>c__IteratorF();
    }

    [CompilerGenerated]
    private sealed class <PreloadCommonAssets>c__IteratorD : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal SceneMgr <>f__this;

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
                    if (!string.IsNullOrEmpty(this.<>f__this.emptyActorPrefabs[0]))
                    {
                        this.<>f__this.PrepareGameObjectLOD(this.<>f__this.emptyActorPrefabs[0], false, enResourceType.BattleScene, 6);
                    }
                    this.$current = new CWaitForNextFrame();
                    this.$PC = 1;
                    goto Label_015D;

                case 1:
                    if (!string.IsNullOrEmpty(this.<>f__this.emptyActorPrefabs[1]))
                    {
                        this.<>f__this.PrepareGameObjectLOD(this.<>f__this.emptyActorPrefabs[1], false, enResourceType.BattleScene, 30);
                    }
                    this.$current = new CWaitForNextFrame();
                    this.$PC = 2;
                    goto Label_015D;

                case 2:
                    if (!string.IsNullOrEmpty(this.<>f__this.emptyActorPrefabs[4]))
                    {
                        this.<>f__this.PrepareGameObjectLOD(this.<>f__this.emptyActorPrefabs[4], false, enResourceType.BattleScene, 50);
                    }
                    this.$current = new CWaitForNextFrame();
                    this.$PC = 3;
                    goto Label_015D;

                case 3:
                    if (!string.IsNullOrEmpty(this.<>f__this.emptyActorPrefabs[3]))
                    {
                        this.<>f__this.PrepareGameObjectLOD(this.<>f__this.emptyActorPrefabs[3], false, enResourceType.BattleScene, 10);
                    }
                    this.$current = new CWaitForNextFrame();
                    this.$PC = 4;
                    goto Label_015D;

                case 4:
                    this.$PC = -1;
                    break;
            }
            return false;
        Label_015D:
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
    private sealed class <PreloadCommonEffects>c__IteratorE : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal SceneMgr <>f__this;
        internal int <i>__0;

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
                    this.<i>__0 = 0;
                    break;

                case 1:
                    this.<i>__0 += 2;
                    break;

                default:
                    goto Label_00B3;
            }
            if (this.<i>__0 < this.<>f__this.commonEffects.Length)
            {
                this.<>f__this.PrepareGameObjectLOD((string) this.<>f__this.commonEffects[this.<i>__0], true, enResourceType.BattleScene, (int) this.<>f__this.commonEffects[this.<i>__0 + 1]);
                this.$current = new CHoldForSecond(0f);
                this.$PC = 1;
                return true;
            }
            this.$PC = -1;
        Label_00B3:
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
    private sealed class <UnloadAssets_Coroutine>c__IteratorF : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;

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
                    this.$current = 0;
                    this.$PC = 1;
                    goto Label_0062;

                case 1:
                    this.$current = Resources.UnloadUnusedAssets();
                    this.$PC = 2;
                    goto Label_0062;

                case 2:
                    GC.Collect();
                    this.$PC = -1;
                    break;
            }
            return false;
        Label_0062:
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
}

