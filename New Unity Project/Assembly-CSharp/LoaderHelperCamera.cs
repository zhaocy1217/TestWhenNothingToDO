using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class LoaderHelperCamera
{
    public GameObject camObj = new GameObject();
    public Vector3 invalidPos = new Vector3(9999f, 9999f, 9999f);
    private Dictionary<string, bool> loadedChecker = new Dictionary<string, bool>();
    public int objIndex;
    public List<Obj> objList = new List<Obj>();
    public GameObject rootObj = new GameObject();

    public LoaderHelperCamera()
    {
        this.camObj.get_transform().set_parent(this.rootObj.get_transform());
        this.rootObj.set_name("lhc");
        this.camObj.set_name("camera");
        Camera camera = this.camObj.AddComponent<Camera>();
        camera.get_transform().set_position(new Vector3(this.invalidPos.x, this.invalidPos.y, this.invalidPos.z - 100f));
        camera.set_depth(-200f);
        camera.set_clearFlags(2);
    }

    public void AddObj(string path, GameObject go)
    {
        if (go != null)
        {
            this.loadedChecker.Add(path, true);
            Obj item = new Obj();
            item.go = go;
            item.frame = Time.get_frameCount();
            this.objList.Add(item);
            DebugHelper.Assert(this.rootObj != null, "you add obj when rootObj is null");
            if (this.rootObj != null)
            {
                go.get_transform().SetParent(this.rootObj.get_transform());
            }
            go.get_transform().set_position(this.invalidPos);
        }
    }

    public void Destroy()
    {
        this.loadedChecker.Clear();
        this.objList.Clear();
        Object.Destroy(this.camObj);
        Object.Destroy(this.rootObj);
        this.camObj = null;
        this.rootObj = null;
    }

    public bool HasLoaded(string objPath)
    {
        return this.loadedChecker.ContainsKey(objPath);
    }

    public bool Update()
    {
        int num = Time.get_frameCount();
        for (int i = this.objIndex; i < this.objList.Count; i++)
        {
            Obj obj2 = this.objList[i];
            if ((num - obj2.frame) >= 5)
            {
                Singleton<CGameObjectPool>.instance.RecyclePreparedGameObject(obj2.go);
                obj2.go = null;
                this.objList[i] = obj2;
            }
            else
            {
                this.objIndex = i;
                return false;
            }
        }
        this.objIndex = this.objList.Count;
        return true;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Obj
    {
        public GameObject go;
        public int frame;
    }
}

