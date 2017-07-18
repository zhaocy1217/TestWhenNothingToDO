using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class Sprite3D : MonoBehaviour
{
    [SerializeField]
    public bool compress = true;
    [SerializeField]
    private EnumHoriontal m_alignHoriontal = EnumHoriontal.Center;
    [SerializeField]
    private EnumVertical m_alignVertical = EnumVertical.Middle;
    [SerializeField]
    private AtlasInfo m_atlas;
    [SerializeField]
    private Color m_color = Color.get_white();
    [SerializeField]
    private float m_depth = 1f;
    [SerializeField]
    private float m_fillAmount = 1f;
    [SerializeField]
    private EnumFillType m_fillType;
    [SerializeField]
    private float m_height = 1f;
    [NonSerialized]
    private string m_lastAtlasName;
    [NonSerialized]
    private Mesh m_mesh;
    [NonSerialized]
    private bool m_propchanged = true;
    [NonSerialized]
    private MeshRenderer m_render;
    [SerializeField]
    private uint m_segments = 50;
    [NonSerialized]
    private SpriteAttr m_spriteAttr;
    [SerializeField]
    private string m_spriteName;
    [SerializeField]
    private string m_tag;
    [SerializeField]
    private Texture2D m_texture;
    [NonSerialized]
    public int m_textureGUID;
    [SerializeField]
    private bool m_useAtlas = true;
    [NonSerialized]
    private AtlasInfo.UVDetail m_uv;
    [SerializeField]
    private float m_width = 1f;
    [SerializeField]
    public int padding;
    private static float S_Ratio = -1f;
    public static readonly int TRANSPARENT_RENDER_QUEUE = 0xbb8;

    private void Awake()
    {
        this.m_lastAtlasName = null;
        this.m_propchanged = true;
        this.m_depth = Mathf.Max(1f, this.m_depth);
        this.m_mesh = null;
        this.PrepareMesh();
        this.RefreshUVDetail();
    }

    private void GenerateHoriontalFillMesh()
    {
        this.m_fillAmount = Mathf.Clamp01(this.m_fillAmount);
        if (this.m_fillAmount > 0f)
        {
            Vector3 vector = base.get_transform().get_localPosition();
            this.m_mesh.MarkDynamic();
            float num = 0f;
            float num2 = 0f;
            if (this.m_alignHoriontal == EnumHoriontal.Center)
            {
                num = -0.5f * this.m_width;
            }
            else if (this.m_alignHoriontal == EnumHoriontal.Right)
            {
                num = -this.m_width;
            }
            if (this.m_alignVertical == EnumVertical.Middle)
            {
                num2 = -0.5f * this.m_height;
            }
            else if (this.m_alignVertical == EnumVertical.Top)
            {
                num2 = -this.m_height;
            }
            Vector3[] vectorArray = new Vector3[] { new Vector3(num, num2 + this.m_height, 0f), new Vector3(num + (this.m_width * this.m_fillAmount), num2 + this.m_height, 0f), new Vector3(num, num2, 0f), new Vector3(num + (this.m_width * this.m_fillAmount), num2, 0f) };
            Vector2[] vectorArray2 = new Vector2[] { this.m_uv.uvTL, this.m_uv.uvTL.Lerp(this.m_uv.uvTR, this.m_fillAmount), this.m_uv.uvBL, this.m_uv.uvBL.Lerp(this.m_uv.uvBR, this.m_fillAmount) };
            Color[] colorArray = new Color[] { this.m_color, this.m_color, this.m_color, this.m_color };
            int[] numArray = new int[] { 0, 1, 2, 3, 2, 1 };
            this.m_mesh.set_vertices(vectorArray);
            this.m_mesh.set_uv(vectorArray2);
            this.m_mesh.set_colors(colorArray);
            this.m_mesh.set_triangles(numArray);
            this.RecaculateDepth();
        }
    }

    public void GenerateMesh()
    {
        this.RefreshUVDetail();
        if (this.m_uv != null)
        {
            this.PrepareMesh();
            this.m_mesh.Clear();
            if (this.m_fillType == EnumFillType.Horiontal)
            {
                this.GenerateHoriontalFillMesh();
            }
            else if (this.m_fillType == EnumFillType.Vertical)
            {
                this.GenerateVerticalFillMesh();
            }
            else if (this.m_fillType == EnumFillType.Radial360)
            {
                this.GenerateRadial360FillMesh();
            }
        }
    }

    private void GenerateRadial360FillMesh()
    {
        this.m_fillAmount = Mathf.Clamp01(this.m_fillAmount);
        if (this.m_fillAmount > 0f)
        {
            Vector3 vector = base.get_transform().get_localPosition();
            this.m_mesh.MarkDynamic();
            float num = 0f;
            float num2 = 0f;
            if (this.m_alignHoriontal == EnumHoriontal.Center)
            {
                num = -0.5f * this.m_width;
            }
            else if (this.m_alignHoriontal == EnumHoriontal.Right)
            {
                num = -this.m_width;
            }
            if (this.m_alignVertical == EnumVertical.Middle)
            {
                num2 = -0.5f * this.m_height;
            }
            else if (this.m_alignVertical == EnumVertical.Top)
            {
                num2 = -this.m_height;
            }
            int num3 = ((int) (this.m_segments * this.m_fillAmount)) + 1;
            float num4 = (2f * (this.width + this.height)) / ((float) this.m_segments);
            Vector3[] vectorArray = new Vector3[num3 + 1];
            Vector2[] vectorArray2 = new Vector2[num3 + 1];
            vectorArray[0] = new Vector3(num + (0.5f * this.m_width), num2 + (0.5f * this.height), 0f);
            vectorArray2[0] = this.m_uv.uvTL.Lerp(this.m_uv.uvBR, 0.5f);
            int num5 = 0;
            int num6 = 0;
            float num7 = 0f;
            for (int i = 0; i < num3; i++)
            {
                switch (num5)
                {
                    case 0:
                    {
                        float num9 = (num + (0.5f * this.width)) + (num6 * num4);
                        if (num9 >= (num + this.width))
                        {
                            num7 = (num9 - num) - this.width;
                            num9 = num + this.width;
                            num5 = 1;
                            num6 = 1;
                        }
                        else
                        {
                            num6++;
                        }
                        vectorArray[i + 1] = new Vector3(num9, num2 + this.height, 0f);
                        break;
                    }
                    case 1:
                    {
                        float num10 = ((num2 + this.height) - (num6 * num4)) - num7;
                        if (num10 <= num2)
                        {
                            num7 = num2 - num10;
                            num10 = num2;
                            num5 = 2;
                            num6 = 1;
                        }
                        else
                        {
                            num6++;
                        }
                        vectorArray[i + 1] = new Vector3(num + this.width, num10, 0f);
                        break;
                    }
                    case 2:
                    {
                        float num11 = ((num + this.width) - (num6 * num4)) - num7;
                        if (num11 <= num)
                        {
                            num7 = num - num11;
                            num11 = num;
                            num5 = 3;
                            num6 = 1;
                        }
                        else
                        {
                            num6++;
                        }
                        vectorArray[i + 1] = new Vector3(num11, num2, 0f);
                        break;
                    }
                    case 3:
                    {
                        float num12 = (num2 + (num6 * num4)) + num7;
                        if (num12 >= (num2 + this.height))
                        {
                            num7 = (num12 - num2) - this.height;
                            num12 = num2 + this.height;
                            num5 = 4;
                            num6 = 1;
                        }
                        else
                        {
                            num6++;
                        }
                        vectorArray[i + 1] = new Vector3(num, num12);
                        break;
                    }
                    case 4:
                    {
                        float num13 = (num + (num6 * num4)) + num7;
                        if (num13 > (num + (0.5f * this.width)))
                        {
                            num13 = num + (0.5f * this.width);
                        }
                        num6++;
                        vectorArray[i + 1] = new Vector3(num13, num2 + this.height, 0f);
                        break;
                    }
                }
                float x = vectorArray[i + 1].x;
                float y = vectorArray[i + 1].y;
                vectorArray2[i + 1] = new Vector2(Mathf.Lerp(this.m_uv.uvTL.x, this.m_uv.uvTR.x, (x - num) / this.width), Mathf.Lerp(this.m_uv.uvBL.y, this.m_uv.uvTL.y, (y - num2) / this.height));
            }
            Color[] colorArray = new Color[num3 + 1];
            for (int j = 0; j < colorArray.Length; j++)
            {
                colorArray[j] = this.m_color;
            }
            int[] numArray = new int[(num3 - 1) * 3];
            for (int k = 0; k < (num3 - 1); k++)
            {
                numArray[k * 3] = 0;
                numArray[(k * 3) + 1] = k + 1;
                numArray[(k * 3) + 2] = k + 2;
            }
            this.m_mesh.set_vertices(vectorArray);
            this.m_mesh.set_uv(vectorArray2);
            this.m_mesh.set_colors(colorArray);
            this.m_mesh.set_triangles(numArray);
            this.RecaculateDepth();
        }
    }

    private void GenerateVerticalFillMesh()
    {
        this.m_fillAmount = Mathf.Clamp01(this.m_fillAmount);
        if (this.m_fillAmount > 0f)
        {
            Vector3 vector = base.get_transform().get_localPosition();
            this.m_mesh.MarkDynamic();
            float num = 0f;
            float num2 = 0f;
            if (this.m_alignHoriontal == EnumHoriontal.Center)
            {
                num = -0.5f * this.m_width;
            }
            else if (this.m_alignHoriontal == EnumHoriontal.Right)
            {
                num = -this.m_width;
            }
            if (this.m_alignVertical == EnumVertical.Middle)
            {
                num2 = -0.5f * this.m_height;
            }
            else if (this.m_alignVertical == EnumVertical.Top)
            {
                num2 = -this.m_height;
            }
            Vector3[] vectorArray = new Vector3[] { new Vector3(num, num2 + (this.m_height * this.m_fillAmount), 0f), new Vector3(num + this.m_width, num2 + (this.m_height * this.m_fillAmount), 0f), new Vector3(num, num2, 0f), new Vector3(num + this.m_width, num2, 0f) };
            Vector2[] vectorArray2 = new Vector2[] { this.m_uv.uvBL.Lerp(this.m_uv.uvTL, this.m_fillAmount), this.m_uv.uvBR.Lerp(this.m_uv.uvTR, this.m_fillAmount), this.m_uv.uvBL, this.m_uv.uvBR };
            Color[] colorArray = new Color[] { this.m_color, this.m_color, this.m_color, this.m_color };
            int[] numArray = new int[] { 0, 1, 2, 3, 2, 1 };
            this.m_mesh.set_vertices(vectorArray);
            this.m_mesh.set_uv(vectorArray2);
            this.m_mesh.set_colors(colorArray);
            this.m_mesh.set_triangles(numArray);
            this.RecaculateDepth();
        }
    }

    private void LateUpdate()
    {
        if (this.m_propchanged)
        {
            this.GenerateMesh();
            this.m_propchanged = false;
        }
    }

    private void OnDestroy()
    {
        Singleton<Canvas3DImpl>.GetInstance().unregisterSprite3d(this);
        Singleton<Canvas3DImpl>.GetInstance().unregisterAutoAtlas(this);
    }

    private void OnEnable()
    {
        Singleton<Canvas3DImpl>.GetInstance().RefreshLayout(null);
    }

    public void PrepareMesh()
    {
        if (this.m_mesh == null)
        {
            MeshFilter component = base.get_gameObject().GetComponent<MeshFilter>();
            if (null == component)
            {
                component = base.get_gameObject().AddComponent<MeshFilter>();
            }
            this.m_mesh = new Mesh();
            component.set_mesh(this.m_mesh);
            this.m_render = base.get_gameObject().GetComponent<MeshRenderer>();
            if (null == this.m_render)
            {
                this.m_render = base.get_gameObject().AddComponent<MeshRenderer>();
            }
        }
    }

    public static float Ratio()
    {
        if (S_Ratio == -1f)
        {
            int num = 960;
            int num2 = 640;
            S_Ratio = Mathf.Min(((float) Screen.get_height()) / ((float) num2), ((float) Screen.get_width()) / ((float) num));
        }
        return S_Ratio;
    }

    private void RecaculateDepth()
    {
        if (this.m_mesh != null)
        {
            Bounds bounds = new Bounds();
            bounds.set_center(new Vector3(0.5f * this.m_width, 0.5f * this.m_height, this.m_depth / 10f));
            bounds.set_size(new Vector3(this.m_width, this.m_height, (this.m_depth / 4f) * 2f));
            this.m_mesh.set_bounds(bounds);
        }
    }

    public void RefreshAtlasMaterial()
    {
        if (this.m_atlas != null)
        {
            this.m_render.set_sharedMaterial(this.m_atlas.material);
        }
    }

    public void RefreshAutoAtlasMaterial()
    {
        if (this.m_texture != null)
        {
            if (this.m_render.get_sharedMaterial() == null)
            {
                Shader content = Singleton<CResourceManager>.GetInstance().GetResource("Shaders/UI/UI3D.shader", typeof(Shader), enResourceType.BattleScene, true, true).m_content as Shader;
                Material material = new Material(content);
                material.SetTexture("_MainTex", this.texture);
                this.m_render.set_sharedMaterial(material);
            }
            else
            {
                this.m_render.get_sharedMaterial().SetTexture("_MainTex", this.texture);
            }
        }
    }

    private void RefreshUVDetail()
    {
        if ((null != this.m_atlas) && (this.m_lastAtlasName != this.m_spriteName))
        {
            this.SetUV(this.m_atlas.GetUV(this.m_spriteName));
            this.m_lastAtlasName = this.m_spriteName;
        }
    }

    public void SetAutoAtlas(Texture2D atlas, AtlasInfo.UVDetail uv)
    {
        this.m_texture = atlas;
        this.SetUV(uv);
    }

    public void SetMaterial(Material mat)
    {
        Material material = this.m_render.get_sharedMaterial();
        this.m_render.set_sharedMaterial(mat);
        Object.DestroyObject(material);
    }

    public void SetNativeSize(Camera camera, float depth)
    {
        float num = Ratio();
        this.RefreshUVDetail();
        if (camera != null)
        {
            Vector3 vector = camera.get_transform().TransformPoint(0f, 0f, depth);
            Vector3 vector2 = camera.WorldToScreenPoint(vector);
            Vector3 vector3 = new Vector3(vector2.x + (this.textureWidth * num), vector2.y, vector2.z);
            Vector3 vector4 = new Vector3(vector2.x, vector2.y + (this.textureHeight * num), vector2.z);
            Vector3 vector5 = camera.ScreenToWorldPoint(vector3);
            Vector3 vector6 = camera.ScreenToWorldPoint(vector4);
            this.width = Vector3.Distance(vector, vector5);
            this.height = Vector3.Distance(vector, vector6);
        }
    }

    public void SetNativeSize(Camera camera, float depth, float screenWidth, float screenHeight)
    {
        if (camera != null)
        {
            Vector3 vector = camera.get_transform().TransformPoint(0f, 0f, depth);
            Vector3 vector2 = camera.WorldToScreenPoint(vector);
            Vector3 vector3 = new Vector3(vector2.x + screenWidth, vector2.y, vector2.z);
            Vector3 vector4 = new Vector3(vector2.x, vector2.y + screenHeight, vector2.z);
            Vector3 vector5 = camera.ScreenToWorldPoint(vector3);
            Vector3 vector6 = camera.ScreenToWorldPoint(vector4);
            this.width = Vector3.Distance(vector, vector5);
            this.height = Vector3.Distance(vector, vector6);
        }
    }

    public static float SetRatio(int newWidth, int newHeight)
    {
        int num = 960;
        int num2 = 640;
        S_Ratio = Mathf.Min(((float) newHeight) / ((float) num2), ((float) newWidth) / ((float) num));
        return S_Ratio;
    }

    public void SetUV(AtlasInfo.UVDetail uv)
    {
        this.PrepareMesh();
        if ((((this.m_mesh == null) || (this.m_mesh.get_triangles().Length == 0)) || ((this.m_uv == null) || (this.m_uv.uvBL != uv.uvBL))) || (((this.m_uv.uvBR != uv.uvBR) || (this.m_uv.uvTL != uv.uvTL)) || (this.m_uv.uvTR != uv.uvTR)))
        {
            this.m_propchanged = true;
        }
        this.m_uv = uv;
        this.RefreshAutoAtlasMaterial();
    }

    private void Start()
    {
        Singleton<Canvas3DImpl>.GetInstance().registerSprite3D(this);
        Singleton<Canvas3DImpl>.GetInstance().registerAutoAtlas(this);
        this.RefreshAtlasMaterial();
    }

    private void Update()
    {
    }

    public EnumHoriontal alignHoriontal
    {
        get
        {
            return this.m_alignHoriontal;
        }
        set
        {
            if (this.m_alignHoriontal != value)
            {
                this.m_alignHoriontal = value;
                this.m_propchanged = true;
            }
        }
    }

    public EnumVertical alignVertical
    {
        get
        {
            return this.m_alignVertical;
        }
        set
        {
            if (this.m_alignVertical != value)
            {
                this.m_alignVertical = value;
                this.m_propchanged = true;
            }
        }
    }

    public AtlasInfo atlas
    {
        get
        {
            return this.m_atlas;
        }
        set
        {
            if (this.m_atlas != value)
            {
                this.m_atlas = value;
                this.useAtlas = true;
                this.m_propchanged = true;
            }
        }
    }

    public string autoAtlasTag
    {
        get
        {
            if (string.IsNullOrEmpty(this.m_tag))
            {
                return this.m_tag;
            }
            return this.m_tag.Trim();
        }
        set
        {
            if (this.m_tag != value)
            {
                this.m_tag = value;
                Singleton<Canvas3DImpl>.GetInstance().registerAutoAtlas(this);
                this.m_propchanged = true;
            }
        }
    }

    public Color color
    {
        get
        {
            return this.m_color;
        }
        set
        {
            if (this.m_color != value)
            {
                this.m_color = value;
                this.m_propchanged = true;
            }
        }
    }

    public float depth
    {
        get
        {
            return this.m_depth;
        }
        set
        {
            this.m_depth = value;
            this.RecaculateDepth();
        }
    }

    public float fillAmount
    {
        get
        {
            return this.m_fillAmount;
        }
        set
        {
            if (this.m_fillAmount != value)
            {
                this.m_fillAmount = value;
                this.m_propchanged = true;
            }
        }
    }

    public EnumFillType fillType
    {
        get
        {
            return this.m_fillType;
        }
        set
        {
            if (this.m_fillType != value)
            {
                this.m_fillType = value;
                this.m_propchanged = true;
            }
        }
    }

    public float HalfTextureHeight
    {
        get
        {
            return (this.textureHeight * 0.5f);
        }
    }

    public float HalfTextureWidth
    {
        get
        {
            return (this.textureWidth * 0.5f);
        }
    }

    public float height
    {
        get
        {
            return this.m_height;
        }
        set
        {
            if (this.m_height != value)
            {
                this.m_height = value;
                this.m_propchanged = true;
            }
        }
    }

    public uint segments
    {
        get
        {
            return this.m_segments;
        }
        set
        {
            if (this.m_segments != value)
            {
                this.m_segments = value;
                this.m_propchanged = true;
            }
        }
    }

    public string spriteName
    {
        get
        {
            return this.m_spriteName;
        }
        set
        {
            if (this.m_spriteName != value)
            {
                this.m_spriteName = value;
                this.m_propchanged = true;
            }
        }
    }

    public Texture2D texture
    {
        get
        {
            return this.m_texture;
        }
        set
        {
            if (this.m_texture != value)
            {
                this.m_texture = value;
                this.useAtlas = false;
                Singleton<Canvas3DImpl>.GetInstance().registerAutoAtlas(this);
                this.m_propchanged = true;
            }
        }
    }

    public int textureHeight
    {
        get
        {
            if (this.m_uv != null)
            {
                return this.m_uv.height;
            }
            if (this.m_texture != null)
            {
                return this.m_texture.get_height();
            }
            return 0;
        }
    }

    public int textureWidth
    {
        get
        {
            if (this.m_uv != null)
            {
                return this.m_uv.width;
            }
            if (this.m_texture != null)
            {
                return this.m_texture.get_width();
            }
            return 0;
        }
    }

    public bool useAtlas
    {
        get
        {
            return this.m_useAtlas;
        }
        set
        {
            if (value != this.m_useAtlas)
            {
                this.m_useAtlas = value;
                if (this.m_useAtlas)
                {
                    this.m_texture = null;
                }
                else
                {
                    this.m_atlas = null;
                }
            }
        }
    }

    public float width
    {
        get
        {
            return this.m_width;
        }
        set
        {
            if (this.m_width != value)
            {
                this.m_width = value;
                this.m_propchanged = true;
            }
        }
    }

    [Serializable]
    public enum EnumFillType
    {
        Horiontal,
        Vertical,
        Radial360
    }

    [Serializable]
    public enum EnumHoriontal
    {
        Left,
        Center,
        Right
    }

    [Serializable]
    public enum EnumVertical
    {
        Top,
        Middle,
        Bottom
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SpriteAttr
    {
        public int x;
        public int y;
        public int width;
        public int height;
    }
}

