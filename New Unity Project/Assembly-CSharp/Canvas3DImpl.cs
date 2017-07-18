using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Canvas3DImpl : Singleton<Canvas3DImpl>
{
    private DictionaryView<string, AutoAtlasInfo> m_atlas = new DictionaryView<string, AutoAtlasInfo>();
    private DictionaryView<int, Sprite3D> m_childSprites = new DictionaryView<int, Sprite3D>();
    private DictionaryView<int, TextMesh> m_childText = new DictionaryView<int, TextMesh>();
    private int m_depth;
    private bool m_needRebuildAtlas;
    private bool m_needRefreshLayout;

    private void _DoRebuildAtlas()
    {
        DictionaryView<string, AutoAtlasInfo>.Enumerator enumerator = this.m_atlas.GetEnumerator();
        while (enumerator.MoveNext())
        {
            KeyValuePair<string, AutoAtlasInfo> current = enumerator.Current;
            AutoAtlasInfo info = current.Value;
            if (info.needRebuildAtlas)
            {
                info.Rebuild();
            }
        }
    }

    private void _DoRefreshLayout(Transform root)
    {
        this.m_depth = 0;
        this.RefreshHierachy(root);
    }

    public void RebuildAtlasImmediately()
    {
        this._DoRebuildAtlas();
    }

    private void RefreshHierachy(Transform root)
    {
        if (root.get_gameObject().get_activeSelf())
        {
            for (int i = root.get_childCount() - 1; i >= 0; i--)
            {
                this.RefreshHierachy(root.GetChild(i));
            }
            Sprite3D component = null;
            if (this.m_childSprites.TryGetValue(root.GetInstanceID(), out component))
            {
                if (null != component)
                {
                    this.m_depth++;
                    component.depth = this.m_depth;
                }
            }
            else
            {
                component = root.GetComponent<Sprite3D>();
                this.m_childSprites.Add(root.GetInstanceID(), component);
                if (null != component)
                {
                    this.m_depth++;
                    component.depth = this.m_depth;
                }
            }
            TextMesh mesh = null;
            if (component == null)
            {
                if (this.m_childText.TryGetValue(root.GetInstanceID(), out mesh))
                {
                    if (null != mesh)
                    {
                        this.m_depth++;
                        mesh.set_offsetZ(((float) this.m_depth) / 10f);
                    }
                }
                else
                {
                    mesh = root.GetComponent<TextMesh>();
                    this.m_childText.Add(root.GetInstanceID(), mesh);
                    if (null != mesh)
                    {
                        this.m_depth++;
                        mesh.set_offsetZ(((float) this.m_depth) / 10f);
                    }
                }
            }
        }
    }

    public void RefreshLayout(Transform root = new Transform())
    {
        this.m_needRefreshLayout = true;
    }

    public void registerAutoAtlas(Sprite3D sprite)
    {
        if (sprite.texture != null)
        {
            if (string.IsNullOrEmpty(sprite.autoAtlasTag))
            {
                AtlasInfo.UVDetail uv = new AtlasInfo.UVDetail();
                uv.uvBL = new Vector2(0f, 0f);
                uv.uvTL = new Vector2(0f, 1f);
                uv.uvBR = new Vector2(1f, 0f);
                uv.uvTR = new Vector2(1f, 1f);
                uv.rotate = false;
                uv.x = 0;
                uv.y = 0;
                uv.width = sprite.texture.get_width();
                uv.height = sprite.texture.get_height();
                sprite.SetUV(uv);
            }
            else
            {
                AutoAtlasInfo info = null;
                if (!this.m_atlas.TryGetValue(sprite.autoAtlasTag, out info))
                {
                    info = new AutoAtlasInfo();
                    this.m_atlas.Add(sprite.autoAtlasTag, info);
                }
                info.Register(sprite);
                this.m_needRebuildAtlas = true;
            }
        }
    }

    public void registerSprite3D(Sprite3D sprite)
    {
        if (!this.m_childSprites.ContainsKey(sprite.get_transform().GetInstanceID()))
        {
            this.m_childSprites.Add(sprite.get_transform().GetInstanceID(), sprite);
        }
    }

    public void Reset()
    {
        this.m_childSprites.Clear();
        this.m_childText.Clear();
        this.m_atlas.Clear();
        Singleton<CResourceManager>.GetInstance().UnloadUnusedAssets();
    }

    public void unregisterAutoAtlas(Sprite3D sprite)
    {
        if ((sprite.texture != null) && !string.IsNullOrEmpty(sprite.autoAtlasTag))
        {
            AutoAtlasInfo info = null;
            if (this.m_atlas.TryGetValue(sprite.autoAtlasTag, out info))
            {
                info.Unregister(sprite);
            }
        }
    }

    public void unregisterSprite3d(Sprite3D sprite)
    {
        this.m_childSprites.Remove(sprite.get_transform().GetInstanceID());
    }

    public void Update(Transform root)
    {
        if (this.m_needRefreshLayout)
        {
            this._DoRefreshLayout(root);
            this.m_needRefreshLayout = false;
        }
        if (this.m_needRebuildAtlas)
        {
            this._DoRebuildAtlas();
            this.m_needRebuildAtlas = true;
        }
    }

    private class AutoAtlasInfo
    {
        public Texture2D altasAlpha;
        public Texture2D atlas;
        private int counter = 1;
        private Material mat;
        private bool needCompress;
        public bool needRebuildAtlas;
        private int padding;
        private HashSet<Sprite3D> sprites = new HashSet<Sprite3D>();
        private DictionaryView<int, AtlasInfo.UVDetail> textures = new DictionaryView<int, AtlasInfo.UVDetail>();
        private static int[] textureSize = new int[] { 0x80, 0x100, 0x200, 0x400 };
        private Dictionary<int, Texture2D> waitForCombineTextures = new Dictionary<int, Texture2D>();

        private bool Pack(int size)
        {
            int num = 0;
            int num2 = 0;
            int num3 = 0;
            int padding = this.padding;
            Vector2 vector = Vector2.get_zero();
            DictionaryView<int, AtlasInfo.UVDetail> view = new DictionaryView<int, AtlasInfo.UVDetail>();
            DictionaryView<int, AtlasInfo.UVDetail>.Enumerator enumerator = this.textures.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<int, AtlasInfo.UVDetail> current = enumerator.Current;
                int width = current.Value.width;
                KeyValuePair<int, AtlasInfo.UVDetail> pair2 = enumerator.Current;
                int height = pair2.Value.height;
                AtlasInfo.UVDetail detail = new AtlasInfo.UVDetail();
                detail.rotate = false;
                KeyValuePair<int, AtlasInfo.UVDetail> pair3 = enumerator.Current;
                view.Add(pair3.Key, detail);
                if ((((num3 + height) + padding) <= size) && (((num2 + width) + padding) <= size))
                {
                    detail.x = num2;
                    detail.y = num3;
                    detail.width = width;
                    detail.height = height;
                    num3 += height + padding;
                    if (num < ((num2 + width) + padding))
                    {
                        num = (num2 + width) + padding;
                    }
                }
                else
                {
                    if (((num + width) <= size) && (height <= size))
                    {
                        num2 = num;
                        detail.x = num2;
                        detail.y = 0;
                        detail.width = width;
                        detail.height = height;
                        num3 = height + padding;
                        num = (num2 + width) + padding;
                        continue;
                    }
                    return false;
                }
            }
            TextureFormat format = 5;
            if (this.needCompress)
            {
                format = 5;
            }
            Texture2D textured = new Texture2D(size, size, format, false);
            Color[] colorArray = new Color[textured.get_width() * textured.get_height()];
            textured.SetPixels(colorArray);
            textured.set_name(string.Concat(new object[] { "Auto_UI3D_Atlas_", size, "_", this.counter, "_format", format.ToString() }));
            this.counter++;
            enumerator.Reset();
            while (enumerator.MoveNext())
            {
                Texture2D atlas = null;
                KeyValuePair<int, AtlasInfo.UVDetail> pair4 = enumerator.Current;
                if (!this.waitForCombineTextures.TryGetValue(pair4.Key, out atlas))
                {
                    atlas = this.atlas;
                }
                KeyValuePair<int, AtlasInfo.UVDetail> pair5 = enumerator.Current;
                AtlasInfo.UVDetail detail2 = pair5.Value;
                KeyValuePair<int, AtlasInfo.UVDetail> pair6 = enumerator.Current;
                AtlasInfo.UVDetail detail3 = view[pair6.Key];
                Color[] colorArray2 = atlas.GetPixels(detail2.x, detail2.y, detail2.width, detail2.height);
                textured.SetPixels(detail3.x, detail3.y, detail2.width, detail2.height, colorArray2);
                textured.Apply(false, false);
                detail3.uvTL = new Vector2(((float) detail3.x) / ((float) textured.get_width()), ((float) (detail3.y + detail3.height)) / ((float) textured.get_height()));
                detail3.uvTR = new Vector2(((float) (detail3.x + detail3.width)) / ((float) textured.get_width()), ((float) (detail3.y + detail3.height)) / ((float) textured.get_height()));
                detail3.uvBL = new Vector2(((float) detail3.x) / ((float) textured.get_width()), ((float) detail3.y) / ((float) textured.get_height()));
                detail3.uvBR = new Vector2(((float) (detail3.x + detail3.width)) / ((float) textured.get_width()), ((float) detail3.y) / ((float) textured.get_height()));
            }
            this.textures = view;
            Object.Destroy(this.atlas);
            this.atlas = textured;
            Shader content = Singleton<CResourceManager>.GetInstance().GetResource("Shaders/UI/UI3D.shader", typeof(Shader), enResourceType.BattleScene, true, true).m_content as Shader;
            this.mat = new Material(content);
            this.mat.SetTexture("_MainTex", this.atlas);
            HashSet<Sprite3D>.Enumerator enumerator2 = this.sprites.GetEnumerator();
            while (enumerator2.MoveNext())
            {
                enumerator2.get_Current().SetMaterial(this.mat);
                enumerator2.get_Current().SetAutoAtlas(this.atlas, this.textures[enumerator2.get_Current().m_textureGUID]);
            }
            Dictionary<int, Texture2D>.Enumerator enumerator3 = this.waitForCombineTextures.GetEnumerator();
            this.waitForCombineTextures.Clear();
            Singleton<CResourceManager>.GetInstance().UnloadUnusedAssets();
            return true;
        }

        public void Rebuild()
        {
            this.needRebuildAtlas = false;
            bool flag = false;
            for (int i = 0; i < textureSize.Length; i++)
            {
                if ((this.atlas == null) || (textureSize[i] >= this.atlas.get_width()))
                {
                    flag = this.Pack(textureSize[i]);
                    if (flag)
                    {
                        break;
                    }
                }
            }
            if (!flag)
            {
                HashSet<Sprite3D>.Enumerator enumerator = this.sprites.GetEnumerator();
                enumerator.MoveNext();
                Debug.LogError("Dynamic Combine Atlas Failed, maybe too many pictures of atlas tag:\"" + enumerator.get_Current().autoAtlasTag + "\"");
            }
        }

        public void Register(Sprite3D sprite)
        {
            if ((null != sprite) && (null != sprite.texture))
            {
                int nativeTextureID = sprite.texture.GetNativeTextureID();
                sprite.m_textureGUID = nativeTextureID;
                AtlasInfo.UVDetail detail = null;
                this.padding = Mathf.Max(this.padding, sprite.padding);
                this.needCompress |= sprite.compress;
                if (this.textures.TryGetValue(nativeTextureID, out detail))
                {
                    this.sprites.Add(sprite);
                    if (null != this.mat)
                    {
                        sprite.SetMaterial(this.mat);
                    }
                    sprite.SetAutoAtlas(this.atlas, detail);
                }
                else
                {
                    detail = new AtlasInfo.UVDetail();
                    detail.width = 0;
                    detail.height = 0;
                    detail.width = sprite.texture.get_width();
                    detail.height = sprite.texture.get_height();
                    detail.rotate = false;
                    this.textures.Add(nativeTextureID, detail);
                    this.waitForCombineTextures.Add(nativeTextureID, sprite.texture);
                    this.needRebuildAtlas = true;
                    this.sprites.Add(sprite);
                }
            }
        }

        public void Unregister(Sprite3D sprite)
        {
            this.sprites.Remove(sprite);
            if (this.sprites.get_Count() == 0)
            {
                this.textures.Clear();
                if (this.mat != null)
                {
                    Object.Destroy(this.mat);
                }
                this.mat = null;
                if (this.atlas != null)
                {
                    Object.Destroy(this.atlas);
                }
                this.atlas = null;
            }
        }
    }
}

