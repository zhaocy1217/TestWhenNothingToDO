using Assets.Scripts.GameLogic;
using System;
using UnityEngine;

public class Camera_UI3D : Singleton<Camera_UI3D>
{
    private Camera m_camera;
    private Canvas3D m_canvas;

    public Camera GetCurrentCamera()
    {
        return this.m_camera;
    }

    public Canvas3DImpl GetCurrentCanvas()
    {
        return Singleton<Canvas3DImpl>.GetInstance();
    }

    public override void Init()
    {
        Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightOver, new RefAction<DefaultGameEventParam>(this.OnFightOver));
    }

    public void OnFightOver(ref DefaultGameEventParam prm)
    {
        if (this.m_camera != null)
        {
            Object.Destroy(this.m_camera.get_gameObject());
            this.m_camera = null;
        }
    }

    public void Reset()
    {
        GameObject obj2 = new GameObject("Camera_UI3D");
        MonoSingleton<SceneMgr>.GetInstance().AddToRoot(obj2, SceneObjType.Temp);
        obj2.get_transform().set_position(new Vector3(100f, 100f, 100f));
        this.m_camera = obj2.AddComponent<Camera>();
        this.m_camera.CopyFrom(Moba_Camera.currentMobaCamera);
        this.m_camera.set_orthographic(true);
        this.m_camera.set_orthographicSize(8f);
        string[] textArray1 = new string[] { "3DUI" };
        this.m_camera.set_cullingMask(LayerMask.GetMask(textArray1));
        this.m_camera.set_depth(this.m_camera.get_depth() + 1f);
        this.m_camera.set_clearFlags(4);
        obj2.set_tag("Untagged");
        this.m_canvas = obj2.AddComponent<Canvas3D>();
        obj2.get_transform().set_position(Vector3.get_zero());
        obj2.get_transform().set_localScale(Vector3.get_one());
        obj2.get_transform().set_localRotation(Quaternion.get_identity());
    }
}

