using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[AddComponentMenu("SGame/Effect/MaterialHurtEffect")]
public class MaterialHurtEffect : ActorComponent
{
    private Texture2D freezeTex;
    private int hlcIndex;
    private List<HighLitColor> hlcList = new List<HighLitColor>();
    private RGBCurve hurtCurve;
    private int[] m_effectCounter = new int[4];
    [NonSerialized, HideInInspector]
    public ListView<Material> mats;
    [HideInInspector]
    private bool meshChanged;
    private ListView<Material> oldMats;
    private float playerId;
    private ListView<SMaterialEffect_Base> playingEffects;
    private static int s_playerId;
    private Texture2D stoneTex;

    private void Awake()
    {
        this.hurtCurve = Singleton<CResourceManager>.GetInstance().GetResource("Shaders/Curve/RGBCurve_Hurt.asset", typeof(RGBCurve), enResourceType.BattleScene, false, false).m_content as RGBCurve;
        this.freezeTex = Singleton<CResourceManager>.GetInstance().GetResource("Prefab_Characters/Z_Other/Textures/Hero_BingDong.tga", typeof(Texture2D), enResourceType.BattleScene, false, false).m_content as Texture2D;
        this.stoneTex = Singleton<CResourceManager>.GetInstance().GetResource("Prefab_Characters/Z_Other/Textures/Hero_ShiHua.tga", typeof(Texture2D), enResourceType.BattleScene, false, false).m_content as Texture2D;
    }

    private bool CheckMats()
    {
        if (this.mats == null)
        {
            this.InitMats();
        }
        return (this.mats != null);
    }

    public SMaterialEffect_Base FindEffect(int type)
    {
        if (this.playingEffects != null)
        {
            for (int i = 0; i < this.playingEffects.Count; i++)
            {
                if (this.playingEffects[i].type == type)
                {
                    return this.playingEffects[i];
                }
            }
        }
        return null;
    }

    private T FindOrCreateEffect<T>(int type) where T: SMaterialEffect_Base, new()
    {
        if (this.playingEffects != null)
        {
            for (int i = 0; i < this.playingEffects.Count; i++)
            {
                SMaterialEffect_Base base2 = this.playingEffects[i];
                if (base2.type == type)
                {
                    if (base2 is T)
                    {
                        return (base2 as T);
                    }
                    base2.Stop();
                    base2.Release();
                    this.playingEffects.RemoveAt(i);
                    break;
                }
            }
        }
        T item = ClassObjPool<T>.Get();
        item.type = type;
        item.owner = this;
        item.bChkReset = false;
        item.AllocId();
        if (this.playingEffects == null)
        {
            this.playingEffects = new ListView<SMaterialEffect_Base>();
        }
        this.playingEffects.Add(item);
        return item;
    }

    private void InitMats()
    {
        this.mats = null;
        Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>();
        if ((componentsInChildren != null) && (componentsInChildren.Length != 0))
        {
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                Renderer renderer = componentsInChildren[i];
                if (((renderer != null) && (renderer.get_sharedMaterial() != null)) && renderer.get_sharedMaterial().HasProperty("_HurtColor"))
                {
                    if (this.mats == null)
                    {
                        this.mats = new ListView<Material>();
                    }
                    this.mats.Add(renderer.get_material());
                }
            }
            if (this.mats != null)
            {
                for (int j = 0; j < this.mats.Count; j++)
                {
                    this.mats[j].SetFloat("_PlayerId", this.playerId);
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (this.mats != null)
        {
            this.mats.Clear();
            this.mats = null;
        }
    }

    public void OnMeshChanged()
    {
        this.oldMats = this.mats;
        this.mats = null;
    }

    public int PlayFreezeEffect()
    {
        this.m_effectCounter[0]++;
        return this.PlayTexEffect(this.freezeTex, 1);
    }

    public int PlayHighLitEffect(Vector3 color)
    {
        if (!this.CheckMats())
        {
            return 0;
        }
        this.m_effectCounter[3]++;
        SMaterialEffect_HighLit lit = this.FindOrCreateEffect<SMaterialEffect_HighLit>(3);
        lit.color = color;
        lit.Play();
        HighLitColor item = new HighLitColor();
        item.id = ++this.hlcIndex;
        item.color = color;
        this.hlcList.Add(item);
        return item.id;
    }

    public void PlayHurtEffect()
    {
        if ((this.hurtCurve != null) && this.CheckMats())
        {
            SMaterialEffect_Curve curve = this.FindOrCreateEffect<SMaterialEffect_Curve>(0);
            if (curve.curve == null)
            {
                curve.curve = this.hurtCurve;
                curve.paramName = "_HurtColor";
                curve.shaderKeyword = "_HURT_EFFECT_ON";
                curve.Play();
            }
        }
    }

    public int PlayStoneEffect()
    {
        this.m_effectCounter[1]++;
        return this.PlayTexEffect(this.stoneTex, 1);
    }

    private int PlayTexEffect(Texture2D tex, int type)
    {
        if (tex == null)
        {
            return -1;
        }
        if (!this.CheckMats())
        {
            return -1;
        }
        SMaterialEffect_Tex tex2 = this.FindOrCreateEffect<SMaterialEffect_Tex>(type);
        if (tex2.tex != null)
        {
            tex2.Replay(tex);
        }
        else
        {
            tex2.tex = tex;
            tex2.texParamName = "_EffectTex";
            tex2.shaderKeyword = "_EFFECT_TEX_ON";
            tex2.enableFade = true;
            tex2.hasFadeFactor = true;
            tex2.fadeInterval = 0.08f;
            tex2.fadeParamName = "_EffectFactor";
            tex2.factorScale = 0.85f;
            tex2.Play();
        }
        return tex2.playingId;
    }

    public void SetTranslucent(bool b, bool bForbidFade = false)
    {
        int num = this.m_effectCounter[2];
        this.m_effectCounter[2] += !b ? -1 : 1;
        if (((num > 0) != (this.m_effectCounter[2] > 0)) && this.CheckMats())
        {
            if (this.m_effectCounter[2] > 0)
            {
                SMaterialEffect_Translucent translucent = this.FindOrCreateEffect<SMaterialEffect_Translucent>(2);
                translucent.enableFade = true;
                translucent.fadeInterval = 0.1f;
                translucent.minAlpha = 0.4f;
                translucent.Play();
            }
            else
            {
                SMaterialEffect_Translucent effect = this.FindEffect(2) as SMaterialEffect_Translucent;
                if (effect != null)
                {
                    if (effect.enableFade && !bForbidFade)
                    {
                        effect.BeginFadeOut();
                    }
                    else
                    {
                        this.StopEffect(effect);
                    }
                }
            }
        }
    }

    protected override void Start()
    {
        int num = Mathf.Max(s_playerId++ % 0xff, 1);
        this.playerId = ((float) num) / 255f;
        this.CheckMats();
    }

    private void StopEffect(SMaterialEffect_Base effect)
    {
        effect.Stop();
        effect.Release();
        this.playingEffects.Remove(effect);
    }

    public void StopFreezeEffect(int playingId)
    {
        this.m_effectCounter[0]--;
        if (this.m_effectCounter[0] <= 0)
        {
            this.StopTexEffect(1, playingId);
        }
    }

    public void StopHighLitEffect(int id)
    {
        if (this.CheckMats())
        {
            if (id > 0)
            {
                for (int i = 0; i < this.hlcList.Count; i++)
                {
                    HighLitColor color = this.hlcList[i];
                    if (color.id == id)
                    {
                        this.hlcList.RemoveAt(i);
                        break;
                    }
                }
            }
            this.m_effectCounter[3]--;
            SMaterialEffect_HighLit effect = this.FindEffect(3) as SMaterialEffect_HighLit;
            if (effect != null)
            {
                if ((this.hlcList.Count > 0) && (id > 0))
                {
                    HighLitColor color2 = this.hlcList[this.hlcList.Count - 1];
                    effect.color = color2.color;
                }
                if (this.m_effectCounter[3] <= 0)
                {
                    this.StopEffect(effect);
                    this.hlcIndex = 0;
                    this.hlcList.Clear();
                }
            }
        }
    }

    public void StopStoneEffect(int playingId)
    {
        this.m_effectCounter[1]--;
        if (this.m_effectCounter[1] <= 0)
        {
            this.StopTexEffect(1, playingId);
        }
    }

    private void StopTexEffect(int type, int playingId)
    {
        if (playingId >= 0)
        {
            SMaterialEffect_Tex effect = this.FindEffect(type) as SMaterialEffect_Tex;
            if ((effect != null) && ((playingId <= 0) || (effect.playingId == playingId)))
            {
                if (effect.enableFade)
                {
                    effect.BeginFadeOut();
                }
                else
                {
                    this.StopEffect(effect);
                }
            }
        }
    }

    private void Update()
    {
        this.CheckMats();
        if ((this.oldMats != null) && (this.mats != null))
        {
            if ((this.playingEffects != null) && (this.playingEffects.Count > 0))
            {
                for (int i = 0; i < this.playingEffects.Count; i++)
                {
                    this.playingEffects[i].OnMeshChanged(this.oldMats, this.mats);
                }
            }
            this.oldMats = null;
        }
        if ((this.playingEffects != null) && (this.playingEffects.Count > 0))
        {
            for (int j = 0; j < this.playingEffects.Count; j++)
            {
                SMaterialEffect_Base base3 = this.playingEffects[j];
                if (base3.Update())
                {
                    base3.Release();
                    this.playingEffects.RemoveAt(j);
                    j--;
                }
            }
        }
    }

    public bool IsTranslucent
    {
        get
        {
            return (this.m_effectCounter[2] > 0);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct HighLitColor
    {
        public int id;
        public Vector3 color;
    }
}

