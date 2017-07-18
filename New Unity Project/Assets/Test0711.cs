using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Test0711 : MonoBehaviour {
    public RectTransform c_rt;
    public Camera uiCamera;
    public RectTransform layout;
    private VerticalLayoutGroup v_layout;
    public ScrollRect scrollRect;
    public RectTransform start;
    public RectTransform end;
    public GameObject Template;
    public Queue<RectTransform> pool = new Queue<RectTransform>();
    private LinkedList<RectTransform> list = new LinkedList<RectTransform>();
    public Vector2 scroll_posi = Vector2.zero;
    private float height;
    private RectTransform rt;
    private List<int> Data = new List<int>();
    public bool recycle;
    private Vector3[] temp = new Vector3[4];
    private Vector3[] scroll_rect_corner = new Vector3[4];
    // Use this for initialization
    public void Genearte ()
    {
        for (int i = 0; i < 100; i++)
        {
            Data.Add(i);
        }
        rt = scrollRect.GetComponent<RectTransform>();
        rt.GetWorldCorners(scroll_rect_corner);
        //for (int i = 0; i < scroll_rect_corner.Length; i++)//左下左上右上右下
        //{
        //    Debug.LogError(scroll_rect_corner[i]);
        //}
        height = rt.sizeDelta.y;
        scrollRect.onValueChanged.AddListener(OnValueChange);
        layout.sizeDelta = new Vector2(layout.sizeDelta.x, Data.Count*100);
        v_layout = layout.GetComponent<VerticalLayoutGroup>();
        if (recycle)
        {
            for (int i = 0; i < 5; i++)
            {
                AddLast(i);
            }
        }
        else
        {
            for (int i = 0; i < Data.Count; i++)
            {
                AddLast(i);
            }
        }
    }
    private RectTransform AddLast(int i)
    {
        RectTransform r = CreateNew();
        AssignValue(r.gameObject, i);
        r.SetAsLastSibling();
        list.AddLast(r);
        end.SetAsLastSibling();
        return r;
    }
    private RectTransform AddFirst(int i)
    {
        RectTransform r = CreateNew();
        AssignValue(r.gameObject, i);
        r.SetAsFirstSibling();
        list.AddFirst(r);
        start.SetAsFirstSibling();
        return r;
    }
    private RectTransform CreateNew()
    {
        if (pool.Count > 0)
        {
            var r = pool.Dequeue();
            r.gameObject.SetActive(true);
            return r;
        }
        else
        {
            GameObject g = GameObject.Instantiate(Template);
            g.SetActive(true);
            g.transform.parent = Template.transform.parent;
            g.transform.localScale = Vector3.one;
            g.transform.position = Template.transform.position;
            var r = g.GetComponent<RectTransform>();
            return r;
        }
    }
    private void AssignValue(GameObject g, int i)
    {
        var t = g.transform.Find("Text");
        var text = t.GetComponent<Text>();
        text.text = i.ToString();
    }
    private int GetValue(RectTransform g)
    {
        var t = g.transform.Find("Text");
        var text = t.GetComponent<Text>();
        string str = text.text;
        return int.Parse(str);
    }
    private float GetHeight(RectTransform r)
    {
        var b = RectTransformUtility.CalculateRelativeRectTransformBounds(r);
        float h = Mathf.Abs(b.extents.y * 2);
        return h;
    }
    private bool IsOverlay(RectTransform a, RectTransform b, Vector3 offset)
    {
        var sc = uiCamera.WorldToScreenPoint(b.transform.position + offset * c_rt.localScale.y);
        bool contain = RectTransformUtility.RectangleContainsScreenPoint(a, sc, uiCamera);
        return contain;
    }
    private bool AllVisible()
    {
        bool allVisible = true;
        var etor = list.GetEnumerator();
        while (etor.MoveNext())
        {
            etor.Current.GetWorldCorners(temp);
            if(temp[1].y<scroll_rect_corner[0].y || temp[0].y>scroll_rect_corner[1].y)
            {
                allVisible = false;
                break;
            }
        }
        etor.Dispose();
        return allVisible;
    }
    private void OnValueChange(Vector2 arg0)
    {
        if (recycle && arg0.y <= 1 && arg0.y >= 0 && list.Count > 0 )
        {
            int cout = 0;
            do
            {
                cout++;
                float h = GetHeight(list.First.Value);
                list.First.Value.GetWorldCorners(temp);
                if (temp[0].y > scroll_rect_corner[1].y)
                {
                    start.sizeDelta += new Vector2(0, 100);
                    list.First.Value.gameObject.SetActive(false);
                    pool.Enqueue(list.First.Value);
                    list.RemoveFirst();
                    LayoutRebuilder.ForceRebuildLayoutImmediate(layout);
                }
                list.First.Value.GetWorldCorners(temp);
                if (temp[1].y < scroll_rect_corner[1].y)
                {
                    int v = GetValue(list.First.Value);
                    start.sizeDelta -= new Vector2(0, 100);
                    var r = AddFirst(v - 1);
                    LayoutRebuilder.ForceRebuildLayoutImmediate(layout);
                }
                list.Last.Value.GetWorldCorners(temp);
                h = GetHeight(list.Last.Value);
                if (temp[1].y < scroll_rect_corner[0].y)
                {
                    list.Last.Value.gameObject.SetActive(false);
                    pool.Enqueue(list.Last.Value);
                    list.RemoveLast();
                    end.sizeDelta += new Vector2(0, 100);
                    LayoutRebuilder.ForceRebuildLayoutImmediate(layout);
                }
                list.Last.Value.GetWorldCorners(temp);
                if (temp[0].y > scroll_rect_corner[0].y)
                {
                    int v = GetValue(list.Last.Value);
                    AddLast(v + 1);
                    end.sizeDelta -= new Vector2(0, 100);
                    LayoutRebuilder.ForceRebuildLayoutImmediate(layout);
                }
            } while (!AllVisible() && cout < 50);
        }
        scroll_posi = arg0;
    }
}
