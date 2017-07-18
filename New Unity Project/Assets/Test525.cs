using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Test525 : MonoBehaviour {

    const string baseURL = "file://D:/TestProjs/ABTest/New Unity Project/AB/";
    AssetBundle ab;
    float count = 0;
    AssetBundleManifest manifest;
    // Use this for initialization
    void Start()
    {


        Debug.LogError(Time.time);
     //   StartCoroutine(LoadManifest());
    }
    void Test()
    {
       
    }


    void Load ()
    {
        string path = @"D:\TestProjs\ABTest\New Unity Project\AB\assets\boxworld\Mesh_Prefab\Building\BUD_Building_A.prefab";
        var dependencies = manifest.GetAllDependencies(@"assets\boxworld\Mesh_Prefab\Building\BUD_Building_A.prefab");
        AssetBundle[] depens = new AssetBundle[dependencies.Length];
        Object[] depenObjs = new Object[dependencies.Length];
        for (int i = 0; i < dependencies.Length; i++)
        {
            depens[i] = AssetBundle.LoadFromFile(@"D:\TestProjs\ABTest\New Unity Project\AB\"+dependencies[i]);
            depenObjs[i] = depens[i].LoadAsset(dependencies[i]);
        }
        ab = AssetBundle.LoadFromFile(path);
        var obj = ab.LoadAsset("BUD_Building_A");
        GameObject go = GameObject.Instantiate(obj) as GameObject;
        for (int i = 0; i < depens.Length; i++)
        {
            depens[i].Unload(false);
        }
        //ab.Unload(false);

        //ab = AssetBundle.LoadFromFile(@"D:\TestProjs\ABTest\New Unity Project\AB\assets\boxworld\Canvas.prefab");
        //Debug.LogError(ab == null);
        //obj = ab.LoadAsset("Canvas");
        //go = GameObject.Instantiate(obj) as GameObject;
        //Debug.LogError(obj == null);
        //ab.Unload(false);

        //ab = AssetBundle.LoadFromFile(@"D:\TestProjs\ABTest\New Unity Project\AB\assets\boxworld\Canvas.prefab");
        //Debug.LogError(ab == null);
        //obj = ab.LoadAsset("Canvas");
        //go = GameObject.Instantiate(obj) as GameObject;
        //Debug.LogError(obj == null);
        //ab.Unload(false);
    }
    IEnumerator LoadManifest()
    {
        WWW www = new WWW(baseURL+"AB");
        yield return www;
        manifest = www.assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        www.assetBundle.Unload(false);
        www.Dispose();
        Load();
        yield return null;
    }
	
	// Update is called once per frame
	void Update () {
        count += Time.deltaTime;
        if(count>5)
        {
            ab.Unload(false);
        }
	}
}
