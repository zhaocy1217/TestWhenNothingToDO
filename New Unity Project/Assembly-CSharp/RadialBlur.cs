using System;
using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(Camera))]
public class RadialBlur : MonoBehaviour
{
    public float blurScale = 50f;
    public Vector2 center = Vector2.get_zero();
    public float falloffExp = 1.5f;
    private Material material;
    public int numIterations = 2;

    public void LoadShaders()
    {
        if (this.material == null)
        {
            string str = "SGame_Post/RadialBlur";
            Shader shader = Shader.Find(str);
            if (shader == null)
            {
            }
            this.material = new Material(shader);
            this.material.set_hideFlags(13);
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        this.numIterations = Mathf.Clamp(this.numIterations, 1, 4);
        if (this.numIterations == 1)
        {
            Graphics.Blit(source, destination, this.material, 0);
        }
        else if (this.numIterations == 2)
        {
            RenderTexture temporary = RenderTexture.GetTemporary(source.get_width(), source.get_height());
            Graphics.Blit(source, temporary, this.material, 0);
            Graphics.Blit(temporary, destination, this.material, 0);
            RenderTexture.ReleaseTemporary(temporary);
        }
        else
        {
            RenderTexture[] textureArray = new RenderTexture[] { RenderTexture.GetTemporary(source.get_width(), source.get_height()), RenderTexture.GetTemporary(source.get_width(), source.get_height()) };
            int index = 0;
            RenderTexture texture2 = source;
            for (int i = 0; i < (this.numIterations - 1); i++)
            {
                Graphics.Blit(texture2, textureArray[index], this.material, 0);
                texture2 = textureArray[index];
                index = ++index % 2;
            }
            Graphics.Blit(texture2, destination, this.material, 0);
            RenderTexture.ReleaseTemporary(textureArray[0]);
            RenderTexture.ReleaseTemporary(textureArray[1]);
        }
    }

    protected void Start()
    {
        this.LoadShaders();
        this.UpdateParameters();
    }

    public void UpdateParameters()
    {
        if (this.material != null)
        {
            this.material.SetVector("_ScreenCenter", new Vector4(this.center.x, this.center.y, 0f, 0f));
            this.material.SetFloat("_FalloffExp", this.falloffExp);
            this.material.SetFloat("_BlurScale", this.blurScale);
        }
    }
}

