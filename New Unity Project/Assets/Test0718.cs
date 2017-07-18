using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test0718 : MonoBehaviour {

    public AnimationCurve edge_ac;
    public MeshRenderer mr;
    private int size = 128;
	// Use this for initialization
	void Start ()
    {
        int half = size / 2;
        int forth = half / 2;
        Texture2D t = new Texture2D(size, size);
        t.filterMode = FilterMode.Point;
        Color[] colors = new Color[size * size];
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                int index = i * size + j;
                int x = j - half;
                int y = i - half;
                float dot = Vector2.Dot(new Vector2(x, y).normalized, Vector2.right);
                if(y<0)
                {
                    dot *= -1;
                    dot -= 2;
                }
                dot += 2;
                dot /= 3;
                var e = edge_ac.Evaluate(dot);
                int dis = Mathf.CeilToInt (e * forth);
                if(dis*dis >= x*x + y*y)
                {
                    colors[index] = Color.red;
                }
                else
                {
                    colors[index] = Color.black;
                }

            }
        }
        t.SetPixels(colors);
        t.Apply();
        Material mat = mr.material;
        mat.SetTexture("_MainTex", t);
        Debug.LogError(sw.ElapsedMilliseconds);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
