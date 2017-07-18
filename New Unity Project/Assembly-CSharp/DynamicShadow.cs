using Assets.Scripts.Framework;
using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class DynamicShadow : MonoBehaviour
{
    private Material blurMat_;
    private Camera depthCam_;
    private Shader depthShader_;
    private RenderTexture[] depthTextures_;
    public LayerMask ShadowCastersMask = -1;
    public float ShadowIntensity = 0.5f;
    private static ListView<DynamicShadow> shadowList = new ListView<DynamicShadow>();
    private int shadowSize = 0x400;
    private bool supportShadow;
    private bool useDepthTex;

    private void CloseShadow()
    {
        if (this.depthTextures_ != null)
        {
            for (int i = 0; i < this.depthTextures_.Length; i++)
            {
                this.depthTextures_[i].Release();
                this.depthTextures_[i] = null;
            }
        }
        this.depthTextures_ = null;
        if (this.depthCam_ != null)
        {
            this.depthCam_.set_enabled(false);
        }
        this.depthCam_ = null;
        Shader.SetGlobalTexture("_SGameShadowTexture", null);
    }

    public static void DisableAllDynamicShadows()
    {
        for (int i = 0; i < shadowList.Count; i++)
        {
            DynamicShadow shadow = shadowList[i];
            if (shadow != null)
            {
                shadow.CloseShadow();
            }
        }
        shadowList.Clear();
        Shader.DisableKeyword("_SGAME_HEROSHOW_SHADOW_ON");
    }

    public static void EnableDynamicShow(GameObject go, bool enable)
    {
        if ((go != null) && (!enable || GameSettings.IsHighQuality))
        {
            DynamicShadow[] componentsInChildren = go.GetComponentsInChildren<DynamicShadow>();
            if ((componentsInChildren != null) && (componentsInChildren.Length != 0))
            {
                for (int i = 0; i < componentsInChildren.Length; i++)
                {
                    DynamicShadow item = componentsInChildren[i];
                    if (enable)
                    {
                        item.InitShadow();
                        if (!shadowList.Contains(item))
                        {
                            shadowList.Add(item);
                        }
                    }
                    else
                    {
                        item.CloseShadow();
                        shadowList.Remove(item);
                    }
                }
                if (enable)
                {
                    Shader.EnableKeyword("_SGAME_HEROSHOW_SHADOW_ON");
                }
                else if (shadowList.Count == 0)
                {
                    Shader.DisableKeyword("_SGAME_HEROSHOW_SHADOW_ON");
                }
            }
        }
    }

    public static void InitDefaultGlobalVariables()
    {
        Shader.DisableKeyword("_SGAME_HEROSHOW_SHADOW_ON");
        Shader.SetGlobalVector("_SGameShadowParams", new Vector4(-0.4862544f, -0.2707574f, 0.8308111f, 0.5f));
        Shader.SetGlobalTexture("_SGameShadowTexture", null);
    }

    private void InitShadow()
    {
        if (this.depthTextures_ == null)
        {
            this.depthShader_ = Shader.Find("SGame_Post/ShadowDepth");
            this.blurMat_ = new Material(Shader.Find("SGame_Post/ShadowBlur"));
            this.useDepthTex = false;
            this.supportShadow = false;
            int num = 0;
            this.supportShadow = true;
            num = 1;
            if (this.supportShadow)
            {
                Shader.EnableKeyword("_SGAME_HEROSHOW_SHADOW_ON");
                RenderTextureFormat format = !this.useDepthTex ? ((RenderTextureFormat) 0) : ((RenderTextureFormat) 1);
                this.depthTextures_ = new RenderTexture[num];
                bool flag = this.SupportRGBABilinear();
                for (int i = 0; i < this.depthTextures_.Length; i++)
                {
                    this.depthTextures_[i] = new RenderTexture(this.shadowSize, this.shadowSize, 0x18, format, 0);
                    this.depthTextures_[i].set_generateMips(false);
                    this.depthTextures_[i].set_filterMode(!flag ? ((FilterMode) 0) : ((FilterMode) 1));
                }
                this.depthCam_ = base.GetComponent<Camera>();
                if (this.depthCam_ == null)
                {
                    this.depthCam_ = base.get_gameObject().AddComponent<Camera>();
                    this.depthCam_.get_transform().set_localPosition(new Vector3(0f, 0f, -5f));
                }
                this.depthCam_.set_enabled(true);
                this.depthCam_.set_targetTexture(this.depthTextures_[0]);
                this.depthCam_.set_depth(-50f);
                this.depthCam_.set_farClipPlane(100f);
                this.depthCam_.set_backgroundColor(new Color(1f, 1f, 1f, 1f));
                this.depthCam_.set_cullingMask(this.ShadowCastersMask);
                this.depthCam_.set_clearFlags(2);
                this.depthCam_.SetReplacementShader(this.depthShader_, "RenderType");
            }
            else
            {
                Shader.DisableKeyword("_SGAME_HEROSHOW_SHADOW_ON");
            }
        }
    }

    private void LateUpdate()
    {
        if (((this.depthCam_ != null) && (this.depthTextures_ != null)) && this.supportShadow)
        {
            Matrix4x4 matrixx = GL.GetGPUProjectionMatrix(this.depthCam_.get_projectionMatrix(), false) * this.depthCam_.get_worldToCameraMatrix();
            Vector4 vector = base.get_transform().get_forward().get_normalized().toVec4(this.ShadowIntensity);
            Shader.SetGlobalMatrix("_SGameShadowMatrix", matrixx);
            Shader.SetGlobalVector("_SGameShadowParams", vector);
        }
    }

    private void OnDestroy()
    {
        this.CloseShadow();
    }

    private void OnPostRender()
    {
        if (((this.depthCam_ != null) && (this.depthTextures_ != null)) && this.supportShadow)
        {
            Shader.SetGlobalTexture("_SGameShadowTexture", this.depthTextures_[0]);
        }
    }

    private void OnPreRender()
    {
        Shader.SetGlobalTexture("_SGameShadowTexture", null);
    }

    private void Start()
    {
    }

    private bool SupportHighpFloat()
    {
        string str = SystemInfo.get_graphicsDeviceName().ToLower();
        if (!str.Contains("adreno"))
        {
            if (str.Contains("powervr") || str.Contains("sgx"))
            {
                return true;
            }
            if (str.Contains("nvidia") || str.Contains("tegra"))
            {
                return true;
            }
            if (!str.Contains("mali") && !str.Contains("arm"))
            {
                return false;
            }
        }
        return true;
    }

    private bool SupportRGBABilinear()
    {
        string str = SystemInfo.get_graphicsDeviceName().ToLower();
        if (!str.Contains("adreno"))
        {
            if (str.Contains("powervr") || str.Contains("sgx"))
            {
                return false;
            }
            if (str.Contains("nvidia") || str.Contains("tegra"))
            {
                return false;
            }
            if (!str.Contains("mali") && !str.Contains("arm"))
            {
                return false;
            }
        }
        return true;
    }
}

