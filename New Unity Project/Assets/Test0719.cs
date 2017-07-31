using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
public class Test0719 : MonoBehaviour
{

    public MeshFilter mf;
    private int width = 9;
    private int height = 9;
    private float cell_width = .1f;
    private float cell_height = .1f;
    private int[,] data;
    public Material mat;
    private Mesh m;
    private List<GameObject> objs;
    void Start()
    {
        var settings = new TextGenerationSettings();
        TextGenerator tg = new TextGenerator();
        tg.Populate("做好了！点击获得<Color=#E75C3EFF>浆果汤</color>吧！", settings);
        //Generate();
    }

    public void Generate()
    {
        objs = new List<GameObject>(height * width);
        Vector3 offset = new Vector3(cell_width,0, cell_height) / 2;
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                GameObject g = GameObject.CreatePrimitive(PrimitiveType.Quad);
                g.GetComponent<MeshRenderer>().material = mat;
                g.transform.parent = transform;
                Vector3 lp = new Vector3(cell_width * j, 0, i * cell_height) + offset;
                g.transform.localPosition = lp;
                g.transform.localScale = new Vector3(cell_width, cell_height, 0);
                g.transform.localEulerAngles = new Vector3(90,0,0);
                var mf = g.GetComponent<MeshFilter>();
                Color[] cs = new Color[mf.mesh.vertexCount];
                for (int k = 0; k < cs.Length; k++)
                {
                    cs[k] = new Color(1,0,1,1);
                }
                mf.mesh.colors = cs;
                // g.hideFlags = HideFlags.HideInHierarchy;
                objs.Add(g);
            }
        }
        var bc = gameObject.GetComponent<BoxCollider>();
        if(bc == null)
        {
            bc = gameObject.AddComponent<BoxCollider>();
        }
        bc.center = new Vector3(cell_width * width, 0.1f, height * cell_height)/2;
        bc.size = new Vector3(cell_width * width, 0.1f, height * cell_height);
    }

    public void Clear()
    {
        List<Transform> list = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            var c = transform.GetChild(i);
            list.Add(c);
        }
        foreach (var item in list)
        {
            GameObject.DestroyImmediate(item.gameObject);
        }
        objs.Clear();
    }

    public void OnHit(Vector3 p)
    {
        Vector3 relative_p = p - gameObject.transform.position;
        int x = Mathf.FloorToInt(relative_p.x / cell_width);
        int z = Mathf.FloorToInt(relative_p.z / cell_height);
        float fmod_x = relative_p.x % cell_width;
        float fmod_z = relative_p.z % cell_height;
        if (fmod_x > cell_width / 2)
            x++;
        if (fmod_z > cell_height / 2)
            z++;
        Vector2 leftup = new Vector2(x-1, z);
        Vector2 rightup = new Vector2(x, z);
        Vector2 leftdown = new Vector2(x-1, z-1);
        Vector2 rightdown = new Vector2(x, z-1);
        AddWeight(leftdown, 1);
        AddWeight(rightdown, 2);
        AddWeight(leftup, 4);
        AddWeight(rightup, 8);
    }
    private void AddWeight(Vector2 v, int weight)
    {
        var mf = objs[(int)v.x + (int)v.y * width].GetComponent<MeshFilter>();
        Color[] cs = mf.mesh.colors;
        for (int i = 0; i < mf.mesh.vertexCount; i++)
        {
            if ((Mathf.RoundToInt(cs[i].g * 256) & weight) == 0)
            {
                cs[i].g += weight / 256f;
            }
        }
        mf.mesh.colors = cs;
    }
    public void Raycast(bool fromEditor)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);// HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        if(fromEditor)
        {
            ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        }
        var hits = Physics.RaycastAll(ray, 100);
        for (int i = 0; i < hits.Length; i++)
        {
            var hit = hits[i];
            var tran = gameObject.transform;
            if (tran == hit.transform)
            {
                OnHit(hit.point);
            }
        }
    }
    void OnMouseDrag()
    {
        Raycast(false);
    }
    void OnMouseMove()
    {
        Raycast(false);
    }
  
    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        for (int i = 0; i < height; i++)
        {
            Gizmos.DrawLine(transform.position + new Vector3(0,.1f,i*cell_height), transform.position + new Vector3(cell_width * (width-1), .1f, i * cell_height));
        }
        for (int i = 0; i < width; i++)
        {
            Gizmos.DrawLine(transform.position + new Vector3(i*cell_width, .1f, 0), transform.position + new Vector3(i * cell_width, .1f, (height-1) * cell_height));
        }
    }
}
