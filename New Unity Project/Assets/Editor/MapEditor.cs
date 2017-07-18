using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Test0717))]
public class MapEditor : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Test0717 t = target as Test0717;
        if (GUILayout.Button("Generate"))
        {
            t.Generate();
            var list = Resources.FindObjectsOfTypeAll(typeof(GameObject));
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i] != t.gameObject)
                {
                    list[i].hideFlags = HideFlags.HideInHierarchy;
                }
            }
        }
        if(GUILayout.Button("ShowElse"))
        {
            var list = Resources.FindObjectsOfTypeAll(typeof(GameObject));
            for (int i = 0; i < list.Length; i++)
            {
                if(list[i] != t.gameObject)
                {
                    list[i].hideFlags = HideFlags.None;
                }
            }
        }
    }
    bool edit = false;
    public void OnSceneGUI()
    {
        Test0717 t = target as Test0717;
        if (Event.current.type == EventType.mouseDown)
        {
            edit = !edit;
        }
        if (edit && Event.current.type == EventType.MouseMove)
        {
            t.Raycast(true);
        }
    }
}
