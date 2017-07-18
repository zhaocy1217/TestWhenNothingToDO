using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

internal class SceneInterpolation : MonoBehaviour
{
    [CompilerGenerated]
    private static Comparison<Camera> <>f__am$cacheB;
    private int activedCamera = -1;
    private Camera camera_pp;
    private Camera camera_scene0;
    private Camera camera_scene1;
    private List<RestoreCameraClearFlags> cameraClearFlagsList = new List<RestoreCameraClearFlags>();
    private List<Camera> cameraList = new List<Camera>();
    private float factor;
    public float FadeTime = 2f;
    private SceneInterpolationRT interpolationRT;
    private RenderTexture rt0;
    private RenderTexture rt1;

    private void DuplicateCamera(Camera src, Camera dest)
    {
        dest.get_transform().set_parent(src.get_transform());
        dest.get_transform().set_localPosition(Vector3.get_zero());
        dest.get_transform().set_localRotation(Quaternion.get_identity());
        dest.get_transform().set_localScale(Vector3.get_one());
        dest.set_aspect(src.get_aspect());
        dest.set_backgroundColor(src.get_backgroundColor());
        dest.set_nearClipPlane(src.get_nearClipPlane());
        dest.set_farClipPlane(src.get_farClipPlane());
        dest.set_fieldOfView(src.get_fieldOfView());
        dest.set_orthographic(src.get_orthographic());
        dest.set_orthographicSize(src.get_orthographicSize());
        dest.set_pixelRect(src.get_pixelRect());
        dest.set_rect(src.get_rect());
    }

    public void Play()
    {
        this.factor = 0f;
        this.cameraClearFlagsList.Clear();
        this.cameraList.Clear();
        string[] textArray1 = new string[] { "Scene", "Scene2" };
        int mask = LayerMask.GetMask(textArray1);
        Object[] objArray = Object.FindObjectsOfType(typeof(Camera));
        if (objArray != null)
        {
            for (int i = 0; i < objArray.Length; i++)
            {
                Camera item = objArray[i] as Camera;
                if ((item.get_cullingMask() & mask) != 0)
                {
                    item.set_cullingMask(item.get_cullingMask() & ~mask);
                    this.cameraList.Add(item);
                }
            }
            if (<>f__am$cacheB == null)
            {
                <>f__am$cacheB = delegate (Camera a, Camera b) {
                    if (a.get_depth() < b.get_depth())
                    {
                        return -1;
                    }
                    if (a.get_depth() > b.get_depth())
                    {
                        return 1;
                    }
                    return 0;
                };
            }
            this.cameraList.Sort(<>f__am$cacheB);
        }
        GameObject obj2 = new GameObject();
        GameObject obj3 = new GameObject();
        GameObject obj4 = new GameObject();
        int num3 = -500;
        this.camera_scene0 = obj2.AddComponent<Camera>();
        string[] textArray2 = new string[] { "Scene" };
        this.camera_scene0.set_cullingMask(LayerMask.GetMask(textArray2));
        this.camera_scene0.set_depth((float) num3--);
        this.camera_scene1 = obj3.AddComponent<Camera>();
        string[] textArray3 = new string[] { "Scene2" };
        this.camera_scene1.set_cullingMask(LayerMask.GetMask(textArray3));
        this.camera_scene1.set_depth((float) num3--);
        this.camera_pp = obj4.AddComponent<Camera>();
        this.camera_pp.set_cullingMask(0);
        this.camera_pp.set_depth((float) num3--);
        if (this.UpdateCamera())
        {
            int num4 = Mathf.RoundToInt(this.camera_scene0.get_pixelWidth());
            int num5 = Mathf.RoundToInt(this.camera_scene0.get_pixelHeight());
            this.rt0 = new RenderTexture(num4, num5, 0x18);
            this.rt1 = new RenderTexture(num4, num5, 0x18);
            this.camera_scene0.set_targetTexture(this.rt0);
            this.camera_scene1.set_targetTexture(this.rt1);
            this.interpolationRT = this.camera_pp.get_gameObject().AddComponent<SceneInterpolationRT>();
            this.interpolationRT.rt0 = this.rt0;
            this.interpolationRT.rt1 = this.rt1;
            this.interpolationRT.factor = 0f;
            Camera camera2 = this.cameraList[this.activedCamera];
            this.camera_scene0.set_clearFlags(camera2.get_clearFlags());
            this.camera_scene1.set_clearFlags(camera2.get_clearFlags());
            this.camera_pp.set_clearFlags(camera2.get_clearFlags());
            for (int j = 0; j < this.cameraList.Count; j++)
            {
                Camera camera3 = this.cameraList[j];
                if ((camera3.get_clearFlags() == 1) || (camera3.get_clearFlags() == 2))
                {
                    RestoreCameraClearFlags flags = new RestoreCameraClearFlags();
                    flags.camera = camera3;
                    flags.flags = camera3.get_clearFlags();
                    this.cameraClearFlagsList.Add(flags);
                    camera3.set_clearFlags(3);
                }
            }
        }
    }

    public void Restore()
    {
        string[] textArray1 = new string[] { "Scene" };
        int mask = LayerMask.GetMask(textArray1);
        string[] textArray2 = new string[] { "Scene2" };
        int num2 = LayerMask.GetMask(textArray2);
        Object[] objArray = Object.FindObjectsOfType(typeof(Camera));
        if (objArray != null)
        {
            for (int i = 0; i < objArray.Length; i++)
            {
                Camera camera = objArray[i] as Camera;
                if ((camera.get_cullingMask() & num2) != 0)
                {
                    camera.set_cullingMask(camera.get_cullingMask() & ~num2);
                    camera.set_cullingMask(camera.get_cullingMask() | mask);
                }
            }
        }
    }

    public void Stop()
    {
        if (this.interpolationRT != null)
        {
            Object.Destroy(this.interpolationRT);
            this.interpolationRT = null;
        }
        if (this.camera_scene0 != null)
        {
            Object.Destroy(this.camera_scene0.get_gameObject());
            this.camera_scene0 = null;
        }
        if (this.camera_scene1 != null)
        {
            Object.Destroy(this.camera_scene1.get_gameObject());
            this.camera_scene1 = null;
        }
        if (this.camera_pp != null)
        {
            Object.Destroy(this.camera_pp.get_gameObject());
            this.camera_pp = null;
        }
        if (this.rt0 != null)
        {
            this.rt0.Release();
            Object.Destroy(this.rt0);
            this.rt0 = null;
        }
        if (this.rt1 != null)
        {
            this.rt1.Release();
            Object.Destroy(this.rt0);
            this.rt1 = null;
        }
        string[] textArray1 = new string[] { "Scene2" };
        int mask = LayerMask.GetMask(textArray1);
        for (int i = 0; i < this.cameraClearFlagsList.Count; i++)
        {
            RestoreCameraClearFlags flags = this.cameraClearFlagsList[i];
            if (flags.camera != null)
            {
                flags.camera.set_clearFlags(flags.flags);
            }
        }
        for (int j = 0; j < this.cameraList.Count; j++)
        {
            Camera camera = this.cameraList[j];
            if (camera != null)
            {
                camera.set_cullingMask(camera.get_cullingMask() | mask);
            }
        }
        this.cameraClearFlagsList.Clear();
        this.cameraList.Clear();
    }

    private void Update()
    {
        this.UpdateCamera();
        if (this.interpolationRT != null)
        {
            float num = Mathf.Max(0.01f, this.FadeTime);
            this.factor += Time.get_deltaTime() / num;
            this.interpolationRT.factor = Mathf.Clamp01(this.factor);
            if (this.factor > 1f)
            {
                this.Stop();
            }
        }
    }

    private bool UpdateCamera()
    {
        int num = -1;
        for (int i = 0; i < this.cameraList.Count; i++)
        {
            Camera camera = this.cameraList[i];
            if ((camera.get_enabled() && camera.get_gameObject().get_activeSelf()) && camera.get_gameObject().get_activeInHierarchy())
            {
                num = i;
                break;
            }
        }
        if (num == -1)
        {
            return false;
        }
        this.activedCamera = num;
        Camera src = this.cameraList[this.activedCamera];
        this.DuplicateCamera(src, this.camera_scene0);
        this.DuplicateCamera(src, this.camera_scene1);
        this.DuplicateCamera(src, this.camera_pp);
        return true;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct RestoreCameraClearFlags
    {
        public Camera camera;
        public CameraClearFlags flags;
    }
}

