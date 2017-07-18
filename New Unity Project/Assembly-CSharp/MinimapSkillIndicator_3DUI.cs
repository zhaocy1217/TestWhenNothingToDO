using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class MinimapSkillIndicator_3DUI
{
    [CompilerGenerated]
    private bool <BInited>k__BackingField;
    private Mask big_maskCom;
    private GameObject big_normalImgNode;
    private GameObject big_redImgNode;
    private bool m_bDirDirty;
    private bool m_bEnable;
    private bool m_bPosDirty;
    private Vector2 m_dir = Vector2.get_zero();
    private Vector3 m_pos = Vector3.get_zero();
    private Mask mini_maskCom;
    private GameObject mini_normalImgNode;
    private GameObject mini_redImgNode;

    public static void CancelIndicator()
    {
        MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
        if ((theMinimapSys != null) && ((theMinimapSys.MMinimapSkillIndicator_3Dui != null) && theMinimapSys.MMinimapSkillIndicator_3Dui.BInited))
        {
            theMinimapSys.MMinimapSkillIndicator_3Dui.SetEnable(false, true);
        }
    }

    public void Clear()
    {
        this.mini_normalImgNode = (GameObject) (this.mini_redImgNode = null);
        this.big_normalImgNode = (GameObject) (this.big_redImgNode = null);
        this.mini_maskCom = (Mask) (this.big_maskCom = null);
        this.BInited = false;
    }

    public void ForceUpdate()
    {
        this.m_dir = Vector2.get_zero();
        this.m_pos = Vector3.get_zero();
    }

    public void Init(GameObject miniTrackNode, GameObject bigTrackNode)
    {
        if ((miniTrackNode != null) && (bigTrackNode != null))
        {
            this.mini_normalImgNode = miniTrackNode.get_transform().Find("normal").get_gameObject();
            this.mini_redImgNode = miniTrackNode.get_transform().Find("red").get_gameObject();
            this.big_normalImgNode = bigTrackNode.get_transform().Find("normal").get_gameObject();
            this.big_redImgNode = bigTrackNode.get_transform().Find("red").get_gameObject();
            this.mini_maskCom = miniTrackNode.GetComponent<Mask>();
            this.big_maskCom = bigTrackNode.GetComponent<Mask>();
            this.SetEnable(false, true);
        }
    }

    public static void InitIndicator(string normalImg, string redImg, float imgHeight, float bigImgHeight)
    {
        MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
        if ((theMinimapSys != null) && ((theMinimapSys.MMinimapSkillIndicator_3Dui != null) && !theMinimapSys.MMinimapSkillIndicator_3Dui.BInited))
        {
            theMinimapSys.MMinimapSkillIndicator_3Dui.SetInitData(normalImg, redImg, imgHeight, bigImgHeight);
        }
    }

    public static bool IsIndicatorInited()
    {
        MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
        return ((theMinimapSys == null) || ((theMinimapSys.MMinimapSkillIndicator_3Dui == null) || theMinimapSys.MMinimapSkillIndicator_3Dui.BInited));
    }

    private void SetColor(bool bNormal)
    {
        if (Singleton<CBattleSystem>.GetInstance().TheMinimapSys != null)
        {
            this.mini_normalImgNode.CustomSetActive(bNormal);
            this.mini_redImgNode.CustomSetActive(!bNormal);
            this.big_normalImgNode.CustomSetActive(bNormal);
            this.big_redImgNode.CustomSetActive(!bNormal);
        }
    }

    public void SetEnable(bool bEnable, bool bForce = false)
    {
        if (bForce || (!bForce && (this.m_bEnable != bEnable)))
        {
            if (this.mini_maskCom != null)
            {
                this.mini_maskCom.set_enabled(bEnable);
            }
            if (this.big_maskCom != null)
            {
                this.big_maskCom.set_enabled(bEnable);
            }
            this.m_bEnable = bEnable;
            if (this.mini_normalImgNode != null)
            {
                this.mini_normalImgNode.get_transform().get_parent().get_gameObject().CustomSetActive(bEnable);
            }
            if (this.big_normalImgNode != null)
            {
                this.big_normalImgNode.get_transform().get_parent().get_gameObject().CustomSetActive(bEnable);
            }
        }
    }

    public static void SetIndicator(ref Vector3 forward, bool bSetEnable = false)
    {
        MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
        if ((theMinimapSys != null) && ((theMinimapSys.MMinimapSkillIndicator_3Dui != null) && theMinimapSys.MMinimapSkillIndicator_3Dui.BInited))
        {
            if (bSetEnable)
            {
                theMinimapSys.MMinimapSkillIndicator_3Dui.SetEnable(true, false);
            }
            theMinimapSys.MMinimapSkillIndicator_3Dui.SetIndicatorForward(ref forward, false);
        }
    }

    public static void SetIndicatorColor(bool bNormal)
    {
        MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
        if ((theMinimapSys != null) && ((theMinimapSys.MMinimapSkillIndicator_3Dui != null) && theMinimapSys.MMinimapSkillIndicator_3Dui.BInited))
        {
            theMinimapSys.MMinimapSkillIndicator_3Dui.SetColor(bNormal);
        }
    }

    public void SetIndicatorForward(ref Vector3 forward, bool bSetEnable = false)
    {
        float num = Mathf.Atan2(forward.z, forward.x) * 57.29578f;
        if (Singleton<BattleLogic>.instance.GetCurLvelContext().m_isCameraFlip)
        {
            num -= 180f;
        }
        Quaternion quaternion = Quaternion.AngleAxis(num, Vector3.get_forward());
        if (this.mini_normalImgNode != null)
        {
            this.mini_normalImgNode.get_transform().set_rotation(quaternion);
        }
        if (this.mini_redImgNode != null)
        {
            this.mini_redImgNode.get_transform().set_rotation(quaternion);
        }
        if (this.big_normalImgNode != null)
        {
            this.big_normalImgNode.get_transform().set_rotation(quaternion);
        }
        if (this.big_redImgNode != null)
        {
            this.big_redImgNode.get_transform().set_rotation(quaternion);
        }
    }

    public void SetInitData(string normalImg, string redImg, float smallImgHeight, float bigImgHeight)
    {
        if ((Singleton<CBattleSystem>.GetInstance().TheMinimapSys != null) && ((!string.IsNullOrEmpty(normalImg) && !string.IsNullOrEmpty(redImg)) && ((smallImgHeight != 0f) && (bigImgHeight != 0f))))
        {
            this.mini_normalImgNode.GetComponent<Image>().SetSprite(normalImg, Singleton<CBattleSystem>.instance.FightFormScript, true, false, false, false);
            this.mini_redImgNode.GetComponent<Image>().SetSprite(redImg, Singleton<CBattleSystem>.instance.FightFormScript, true, false, false, false);
            this.big_normalImgNode.GetComponent<Image>().SetSprite(normalImg, Singleton<CBattleSystem>.instance.FightFormScript, true, false, false, false);
            this.big_redImgNode.GetComponent<Image>().SetSprite(redImg, Singleton<CBattleSystem>.instance.FightFormScript, true, false, false, false);
            this.SetWidthHeight(this.mini_normalImgNode, 400f, smallImgHeight);
            this.SetWidthHeight(this.mini_redImgNode, 400f, smallImgHeight);
            this.SetWidthHeight(this.big_normalImgNode, 800f, bigImgHeight);
            this.SetWidthHeight(this.big_redImgNode, 800f, bigImgHeight);
            this.BInited = true;
        }
    }

    private void SetWidthHeight(GameObject obj, float width, float height)
    {
        if (obj != null)
        {
            RectTransform transform = obj.get_transform() as RectTransform;
            if (transform != null)
            {
                transform.set_sizeDelta(new Vector2(width, height));
            }
        }
    }

    public void Update()
    {
        MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
        if (theMinimapSys != null)
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
            if ((hostPlayer != null) && (hostPlayer.Captain != 0))
            {
                Vector3 location = (Vector3) hostPlayer.Captain.handle.location;
                this.m_bPosDirty = this.m_pos != location;
                if (theMinimapSys.CurMapType() == MinimapSys.EMapType.Mini)
                {
                    if (this.m_bPosDirty)
                    {
                        this.UpdatePosition(this.mini_normalImgNode, ref location, true);
                        this.UpdatePosition(this.mini_redImgNode, ref location, true);
                        this.m_pos = location;
                    }
                }
                else if ((theMinimapSys.CurMapType() == MinimapSys.EMapType.Big) && this.m_bPosDirty)
                {
                    this.UpdatePosition(this.big_normalImgNode, ref location, false);
                    this.UpdatePosition(this.big_redImgNode, ref location, false);
                    this.m_pos = location;
                }
            }
        }
    }

    public void Update(ref Vector2 dir)
    {
        if (dir != Vector2.get_zero())
        {
            MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
            if (theMinimapSys != null)
            {
                Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
                if ((hostPlayer != null) && (hostPlayer.Captain != 0))
                {
                    this.m_bDirDirty = this.m_dir != dir;
                    Vector3 location = (Vector3) hostPlayer.Captain.handle.location;
                    this.m_bPosDirty = this.m_pos != location;
                    bool bSmallMap = theMinimapSys.CurMapType() == MinimapSys.EMapType.Mini;
                    if (this.m_bDirDirty)
                    {
                        this.UpdateRotation(this.mini_normalImgNode, ref dir);
                        this.UpdateRotation(this.mini_redImgNode, ref dir);
                        this.UpdateRotation(this.big_normalImgNode, ref dir);
                        this.UpdateRotation(this.big_redImgNode, ref dir);
                        this.m_dir = dir;
                    }
                    if (this.m_bPosDirty)
                    {
                        this.UpdatePosition(this.mini_normalImgNode, ref location, bSmallMap);
                        this.UpdatePosition(this.mini_redImgNode, ref location, bSmallMap);
                        this.UpdatePosition(this.big_normalImgNode, ref location, bSmallMap);
                        this.UpdatePosition(this.big_redImgNode, ref location, bSmallMap);
                        this.m_pos = location;
                    }
                }
            }
        }
    }

    public static void UpdateIndicator(ref Vector2 dir, bool bSetEnable = false)
    {
        MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
        if ((theMinimapSys != null) && ((theMinimapSys.MMinimapSkillIndicator_3Dui != null) && theMinimapSys.MMinimapSkillIndicator_3Dui.BInited))
        {
            if (bSetEnable)
            {
                theMinimapSys.MMinimapSkillIndicator_3Dui.SetEnable(true, false);
            }
            theMinimapSys.MMinimapSkillIndicator_3Dui.Update(ref dir);
        }
    }

    private void UpdatePosition(GameObject node, ref Vector3 pos, bool bSmallMap)
    {
        if (node != null)
        {
            RectTransform transform = node.get_transform() as RectTransform;
            if (transform != null)
            {
                if (bSmallMap)
                {
                    transform.set_anchoredPosition(new Vector2(pos.x * Singleton<CBattleSystem>.instance.world_UI_Factor_Small.x, pos.z * Singleton<CBattleSystem>.instance.world_UI_Factor_Small.y));
                }
                else
                {
                    transform.set_anchoredPosition(new Vector2(pos.x * Singleton<CBattleSystem>.instance.world_UI_Factor_Big.x, pos.z * Singleton<CBattleSystem>.instance.world_UI_Factor_Big.y));
                }
            }
        }
    }

    private void UpdateRotation(GameObject node, ref Vector2 dir)
    {
        float num = Mathf.Atan2(dir.y, dir.x) * 57.29578f;
        Quaternion quaternion = Quaternion.AngleAxis(num, Vector3.get_forward());
        node.get_transform().set_rotation(quaternion);
    }

    public bool BInited
    {
        [CompilerGenerated]
        get
        {
            return this.<BInited>k__BackingField;
        }
        [CompilerGenerated]
        private set
        {
            this.<BInited>k__BackingField = value;
        }
    }
}

