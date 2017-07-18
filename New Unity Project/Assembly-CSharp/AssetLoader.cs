using System;
using UnityEngine;

public class AssetLoader
{
    public bool preloading = true;

    public virtual void DestroyObject(Object obj)
    {
        if (obj is GameObject)
        {
            Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(obj as GameObject);
        }
        else
        {
            Object.Destroy(obj);
        }
    }

    public virtual Object Instantiate(Object original)
    {
        return Object.Instantiate(original);
    }

    public virtual Object Instantiate(Object original, Vector3 position, Quaternion rotation)
    {
        return Object.Instantiate(original, position, rotation);
    }

    public virtual Object Load(string path)
    {
        return Resources.Load(path);
    }

    public virtual Object LoadAge(string path)
    {
        return Singleton<CResourceManager>.GetInstance().GetResource(path + ".xml", typeof(TextAsset), enResourceType.BattleScene, false, false).m_content;
    }
}

