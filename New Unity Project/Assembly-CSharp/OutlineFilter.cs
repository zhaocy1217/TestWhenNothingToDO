using Assets.Scripts.Framework;
using System;
using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(Camera))]
public class OutlineFilter : MonoBehaviour
{
    public float blendFactor = 0.5f;
    public bool clearAlpha;
    private Material clearAlphaMat;
    public int filterType = 1;
    private Material material;
    [NonSerialized, HideInInspector]
    public Camera particlesCam;
    private static bool s_isRenderingParticles;

    private void ClearAlpha()
    {
        if ((this.clearAlphaMat != null) && (base.GetComponent<Camera>() != null))
        {
            GL.PushMatrix();
            this.clearAlphaMat.SetPass(0);
            GL.LoadOrtho();
            GL.Viewport(base.GetComponent<Camera>().get_pixelRect());
            GL.Begin(7);
            GL.TexCoord2(0f, 0f);
            GL.Vertex3(0f, 0f, 0f);
            GL.TexCoord2(0f, 1f);
            GL.Vertex3(0f, 1f, 0f);
            GL.TexCoord2(1f, 1f);
            GL.Vertex3(1f, 1f, 0f);
            GL.TexCoord2(1f, 0f);
            GL.Vertex3(1f, 0f, 0f);
            GL.End();
            GL.PopMatrix();
        }
    }

    public static void DisableOutlineFilter()
    {
        string[] textArray1 = new string[] { "Particles" };
        int mask = LayerMask.GetMask(textArray1);
        foreach (Camera camera in Object.FindObjectsOfType<Camera>())
        {
            if (camera != null)
            {
                OutlineFilter component = camera.GetComponent<OutlineFilter>();
                if (component != null)
                {
                    Object.Destroy(component);
                    camera.set_cullingMask(camera.get_cullingMask() | mask);
                    foreach (Camera camera2 in camera.GetComponentsInChildren<Camera>())
                    {
                        if ((camera2 != null) && (camera2 != camera))
                        {
                            GameObject obj2 = camera2.get_gameObject();
                            Object.Destroy(camera2);
                            Object.Destroy(obj2);
                        }
                    }
                }
            }
        }
    }

    private void drawParticles(RenderTexture colorRT, RenderTexture depthRT)
    {
        if (this.particlesCam != null)
        {
            try
            {
                this.particlesCam.set_enabled(true);
                RenderTexture texture = this.particlesCam.get_targetTexture();
                this.particlesCam.SetTargetBuffers(colorRT.get_colorBuffer(), depthRT.get_depthBuffer());
                this.particlesCam.Render();
                this.particlesCam.set_targetTexture(texture);
                this.particlesCam.set_enabled(false);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }
    }

    public static void EnableOutlineFilter()
    {
        string[] textArray1 = new string[] { "Scene" };
        int mask = LayerMask.GetMask(textArray1);
        string[] textArray2 = new string[] { "Particles" };
        int num2 = LayerMask.GetMask(textArray2);
        foreach (Camera camera in Object.FindObjectsOfType<Camera>())
        {
            if ((camera.get_cullingMask() & mask) != 0)
            {
                OutlineFilter filter = camera.get_gameObject().AddComponent<OutlineFilter>();
                camera.set_cullingMask(camera.get_cullingMask() & ~num2);
                GameObject obj2 = new GameObject();
                obj2.get_transform().set_parent(camera.get_transform());
                obj2.get_transform().set_localPosition(Vector3.get_zero());
                obj2.get_transform().set_localRotation(Quaternion.get_identity());
                obj2.set_name(camera.get_name() + " particles");
                Camera camera2 = obj2.AddComponent<Camera>();
                camera2.set_aspect(camera.get_aspect());
                camera2.set_backgroundColor(camera.get_backgroundColor());
                camera2.set_nearClipPlane(camera.get_nearClipPlane());
                camera2.set_farClipPlane(camera.get_farClipPlane());
                camera2.set_fieldOfView(camera.get_fieldOfView());
                camera2.set_orthographic(camera.get_orthographic());
                camera2.set_orthographicSize(camera.get_orthographicSize());
                camera2.set_pixelRect(camera.get_pixelRect());
                camera2.set_rect(camera.get_rect());
                camera2.set_clearFlags(4);
                camera2.set_cullingMask(num2);
                camera2.set_enabled(false);
                filter.particlesCam = camera2;
                filter.UpdateFilterType(false);
            }
        }
    }

    public static void EnableSurfaceShaderOutline(bool enable)
    {
        Shader.SetGlobalFloat("_SGamelGlobalAlphaModifier", !enable ? 1f : 0f);
    }

    public void LoadShaders()
    {
        if (this.material == null)
        {
            string str = "SGame_Post/OutlineFilter";
            Shader shader = Shader.Find(str);
            if (shader == null)
            {
            }
            this.material = new Material(shader);
            this.material.set_hideFlags(13);
        }
        if (this.clearAlphaMat == null)
        {
            string str2 = "SGame_Post/ClearAlpha";
            Shader shader2 = Shader.Find(str2);
            if (shader2 == null)
            {
            }
            this.clearAlphaMat = new Material(shader2);
            this.clearAlphaMat.set_hideFlags(13);
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!s_isRenderingParticles)
        {
            if (destination == null)
            {
                RenderTexture colorRT = RenderTexture.GetTemporary(source.get_width(), source.get_height(), 0);
                source.set_filterMode(0);
                Graphics.Blit(source, colorRT, this.material, 0);
                this.drawParticles(colorRT, source);
                Graphics.Blit(colorRT, destination);
                RenderTexture.ReleaseTemporary(colorRT);
            }
            else
            {
                Graphics.Blit(source, destination, this.material, 0);
                this.drawParticles(destination, source);
            }
        }
    }

    private void Start()
    {
        this.LoadShaders();
        this.UpdateParameters();
    }

    public void UpdateFilterType(bool heroShow)
    {
        int num = Mathf.Max(Screen.get_width(), Screen.get_height());
        int num2 = Mathf.Min(Screen.get_width(), Screen.get_height());
        bool flag = ((num >= 0x640) || (num2 >= 900)) || (Screen.get_dpi() >= 350f);
        bool flag2 = ((num >= 0x470) || (num2 >= 640)) || (Screen.get_dpi() >= 300f);
        if (GameSettings.ShouldReduceResolution())
        {
            flag = false;
            flag2 = true;
        }
        if (flag)
        {
            this.filterType = 2;
            this.blendFactor = 0.55f;
        }
        else if (flag2)
        {
            this.filterType = 1;
            this.blendFactor = 0.5f;
        }
        else
        {
            this.filterType = 1;
            this.blendFactor = 0.5f;
        }
        if (heroShow)
        {
            this.filterType++;
        }
        this.LoadShaders();
        this.UpdateParameters();
    }

    public void UpdateParameters()
    {
        if ((this.material != null) && (Camera.get_main() != null))
        {
            this.material.SetFloat("_BlendFactor", this.blendFactor);
            float num = 1f / Camera.get_main().get_pixelWidth();
            float num2 = 1f / Camera.get_main().get_pixelHeight();
            if (this.filterType == 0)
            {
                Vector4 vector = new Vector4(-num, 0f, num, 0f);
                Vector4 vector2 = new Vector4(0f, -num2, 0f, num2);
                this.material.SetVector("_TexelOffset0", vector);
                this.material.SetVector("_TexelOffset1", vector2);
                this.material.DisableKeyword("_HIGHQUALITY_ON");
            }
            else if (this.filterType == 1)
            {
                Vector4 vector3 = new Vector4(-num, num2, num, num2);
                Vector4 vector4 = new Vector4(-num, -num2, num, -num2);
                this.material.SetVector("_TexelOffset0", vector3);
                this.material.SetVector("_TexelOffset1", vector4);
                this.material.DisableKeyword("_HIGHQUALITY_ON");
            }
            else if (this.filterType == 2)
            {
                num *= 2f;
                num2 *= 2f;
                Vector4 vector5 = new Vector4(-num, 0f, num, 0f);
                Vector4 vector6 = new Vector4(0f, -num2, 0f, num2);
                this.material.SetVector("_TexelOffset0", vector5);
                this.material.SetVector("_TexelOffset1", vector6);
                this.material.DisableKeyword("_HIGHQUALITY_ON");
            }
            else if (this.filterType == 3)
            {
                num *= 2f;
                num2 *= 2f;
                Vector4 vector7 = new Vector4(-num, num2, num, num2);
                Vector4 vector8 = new Vector4(-num, -num2, num, -num2);
                this.material.SetVector("_TexelOffset0", vector7);
                this.material.SetVector("_TexelOffset1", vector8);
                this.material.DisableKeyword("_HIGHQUALITY_ON");
            }
            else
            {
                Vector4 vector9 = new Vector4(-num, 0f, num, 0f);
                Vector4 vector10 = new Vector4(0f, -num2, 0f, num2);
                Vector4 vector11 = new Vector4(-num, num2, num, num2);
                Vector4 vector12 = new Vector4(-num, -num2, num, -num2);
                this.material.SetVector("_TexelOffset0", vector9);
                this.material.SetVector("_TexelOffset1", vector10);
                this.material.SetVector("_TexelOffset2", vector11);
                this.material.SetVector("_TexelOffset3", vector12);
                this.material.EnableKeyword("_HIGHQUALITY_ON");
            }
        }
    }
}

