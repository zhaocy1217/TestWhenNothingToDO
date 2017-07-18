using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class UnityExtension
{
    public static List<GameObject> bfsList = new List<GameObject>();

    public static GameObject FindChildBFS(this GameObject gameObj, string name)
    {
        if (gameObj == null)
        {
            return null;
        }
        bfsList.Add(gameObj);
        GameObject obj2 = null;
        int num = 0;
        while (num < bfsList.Count)
        {
            GameObject obj3 = bfsList[num++];
            if (obj3.get_name() == name)
            {
                obj2 = obj3;
                break;
            }
            int num2 = obj3.get_transform().get_childCount();
            for (int i = 0; i < num2; i++)
            {
                GameObject item = obj3.get_transform().GetChild(i).get_gameObject();
                bfsList.Add(item);
            }
        }
        bfsList.Clear();
        return obj2;
    }

    public static GameObject FindChildBFS(this GameObject gameObj, FindChildDelegate findChild)
    {
        if ((gameObj == null) || (findChild == null))
        {
            return null;
        }
        bfsList.Add(gameObj);
        GameObject obj2 = null;
        int num = 0;
        while (num < bfsList.Count)
        {
            GameObject obj3 = bfsList[num++];
            if (findChild(obj3))
            {
                obj2 = obj3;
                break;
            }
            int num2 = obj3.get_transform().get_childCount();
            for (int i = 0; i < num2; i++)
            {
                GameObject item = obj3.get_transform().GetChild(i).get_gameObject();
                bfsList.Add(item);
            }
        }
        bfsList.Clear();
        return obj2;
    }

    public delegate bool FindChildDelegate(GameObject obj);
}

