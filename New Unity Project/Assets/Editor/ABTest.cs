using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ABTest : Editor {

    public static Dictionary<Object, int> refCount = new Dictionary<Object, int>();
    public static List<GameObject> caculatedGO = new List<GameObject>(); 
    public static List<System.Type> types = new List<System.Type>() {typeof(Texture2D), typeof(Mesh), typeof(Material), typeof(Shader), typeof(AnimationClip)};
    [MenuItem("Tools/BuildAB")]
    public static void BuildAB()
    {
        var path = Path.Combine(Application.dataPath, @"..\AB");
        Debug.LogError(path);
        BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows64);
    }

    [MenuItem("Tools/AB")]
    public static void AB()
    {
        string basePath = Application.dataPath + "/BoxWorld";
        Directory.GetFiles(basePath, "");
        var paths = AssetDatabase.GetAllAssetPaths();
        List<string> filterPaths = new List<string>(paths.Length);
        foreach (var path in paths)
        {
            if(path.Contains("Assets/BoxWorld"))
            {
                filterPaths.Add(path);
                var asset = AssetDatabase.LoadAssetAtPath(path, typeof(Object));

                if (asset is GameObject)
                {
                    var go = asset as GameObject;
                    DoGameObject(go, path);
                }
                else if (asset is SceneAsset)
                {
                    var scene = asset as SceneAsset;
                    DoScene(scene, path);
                }
            }
        }
    }
    static AssetBundle ab;
    [MenuItem("Tools/Caculate")]
    public static void Compare()
    {
        
        //WWW www = new WWW("file:///D:/TestProjs/ABTest/New Unity Project/AB/AB");
        //while (!www.isDone)
        //{
            
        //}
        //var manifest = (AssetBundleManifest)www.assetBundle.LoadAsset("AB");
    //    manifest.
        if(ab == null)
            ab = AssetBundle.LoadFromFile(@"D:\TestProjs\ABTest\New Unity Project\AB\assets\boxworld\Canvas.prefab");
        Debug.LogError(ab == null);
        var obj = ab.LoadAsset("Canvas");
        GameObject go = GameObject.Instantiate(obj) as GameObject;
        Debug.LogError(obj==null);
        ab.Unload(false);
    }

    public static void SetAssetBundleName(string ap, Object obj)
    {
        AssetImporter ai = AssetImporter.GetAtPath(ap);
        if (ai == null) Debug.LogError(ap + "  " + obj.GetType());
        else
            ai.assetBundleName = ap;

    }
    static void DoScene(SceneAsset scene, string path)
    {
        SetAssetBundleName(path, scene);
        foreach (Object obj in EditorUtility.CollectDependencies(new Object[] { scene }))
        {
            if(obj is GameObject && !caculatedGO.Contains(obj as GameObject))
            {
                caculatedGO.Add(obj as GameObject);
                var ap = AssetDatabase.GetAssetPath(obj);
                DoGameObject(obj as GameObject, ap);
            }
        }
    }
    static void DoGameObject(GameObject go, string path)
    {
        SetAssetBundleName(path, go);
        foreach (Object obj in EditorUtility.CollectDependencies(new Object[] { go }))
        {
            if (types.Contains(obj.GetType()))
            {
                var ap = AssetDatabase.GetAssetPath(obj);
                if (!refCount.ContainsKey(obj)) refCount[obj] = 0;
                refCount[obj]++;
                if (refCount[obj] > 1)
                {
                    SetAssetBundleName(ap, obj);
                }
            }
        }
    }
    public static void DoMeshRenderer(GameObject go, MeshRenderer mr)
    {
        for (int i = 0; i < mr.materials.Length; i++)
        {
            DoMaterial(mr.materials[i]);
        }
    }
    public static void DoMaterial(Material mat)
    {
        
    }

}
