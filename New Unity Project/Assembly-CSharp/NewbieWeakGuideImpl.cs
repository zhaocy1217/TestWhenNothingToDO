using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using ResData;
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

internal class NewbieWeakGuideImpl
{
    public const string FormWeakGuidePath = "UGUI/Form/System/Dialog/Form_WeakGuide";
    private NewbieGuideWeakConf m_conf;
    private CUIFormScript m_formWeakGuide;
    private GameObject m_guideTextObj;
    private GameObject m_guideTextStatic;
    private CUIFormScript m_originalForm;
    private GameObject m_parentObj;
    private static readonly string WEAK_BUBBLE_PATH = "UGUI/Form/System/Dialog/WeakGuideBubble.prefab";
    private static readonly string WEAK_EFFECT_PATH = "UGUI/Form/System/Dialog/WeakGuideHighlighter.prefab";

    private GameObject AddBubbleEffectInternal(GameObject effectParent, NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl, CUIFormScript inOriginalForm)
    {
        if (effectParent == null)
        {
            return null;
        }
        if (effectParent.get_transform().FindChild("WeakGuideBubble(Clone)") != null)
        {
            return null;
        }
        GameObject content = Singleton<CResourceManager>.GetInstance().GetResource(WEAK_BUBBLE_PATH, typeof(GameObject), enResourceType.UIPrefab, false, false).m_content as GameObject;
        GameObject p = Object.Instantiate(content) as GameObject;
        p.get_transform().SetParent(effectParent.get_transform());
        RectTransform transform = p.get_transform().FindChild("Image") as RectTransform;
        RectTransform transform2 = p.get_transform() as RectTransform;
        NewbieGuideSpecialTipConf specialTipConfig = Singleton<NewbieGuideDataManager>.GetInstance().GetSpecialTipConfig(conf.wSpecialTip);
        if (specialTipConfig != null)
        {
            CUICommonSystem.SetTextContent(transform2.FindChild("Text"), specialTipConfig.szTipText);
        }
        Vector2 vector = new Vector2();
        Vector2 vector2 = new Vector2();
        Vector2 vector3 = new Vector2();
        Vector3 vector4 = Vector3.get_one();
        Vector2 vector5 = new Vector2();
        switch (specialTipConfig.bSpecialTipPos)
        {
            case 0:
                vector.x = 0f;
                vector.y = 1f;
                vector2.x = 0f;
                vector2.y = 1f;
                vector4.y = -1f;
                vector3.x = 0f;
                vector3.y = 0f;
                vector5.x = 10f;
                vector5.y = 15f;
                break;

            case 1:
                vector.x = 0f;
                vector.y = 0f;
                vector2.x = 0f;
                vector2.y = 0f;
                vector3.x = 0f;
                vector3.y = 0f;
                vector5.x = 10f;
                vector5.y = -15f;
                break;

            case 2:
                vector.x = 1f;
                vector.y = 1f;
                vector2.x = 1f;
                vector2.y = 1f;
                vector4.y = -1f;
                vector3.x = 1f;
                vector3.y = 0f;
                vector5.x = -10f;
                vector5.y = 15f;
                break;

            case 3:
                vector.x = 1f;
                vector.y = 0f;
                vector2.x = 1f;
                vector2.y = 0f;
                vector3.x = 1f;
                vector3.y = 0f;
                vector5.x = -10f;
                vector5.y = -15f;
                break;
        }
        transform2.set_position(effectParent.get_transform().get_position());
        transform2.set_anchoredPosition(Vector2.get_zero());
        Vector2 vector6 = transform2.get_anchoredPosition();
        vector6.x += specialTipConfig.iOffsetX;
        vector6.y += specialTipConfig.iOffsetY;
        transform2.set_anchoredPosition(vector6);
        transform2.set_localScale(Vector3.get_one());
        transform.set_localScale(vector4);
        transform.set_anchorMin(vector);
        transform.set_anchorMax(vector2);
        transform.set_pivot(vector3);
        transform.set_anchoredPosition(vector5);
        CUITimerScript componetInChild = Utility.GetComponetInChild<CUITimerScript>(p, "Timer");
        if (componetInChild != null)
        {
            if (conf.Param[0] != 0)
            {
                componetInChild.SetTotalTime((float) conf.Param[0]);
            }
            componetInChild.StartTimer();
        }
        return p;
    }

    private void AddBubbleText(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
    {
        GameObject obj2 = this.m_formWeakGuide.get_transform().FindChild("GuideText").get_gameObject();
        obj2.CustomSetActive(true);
        NewbieGuideSpecialTipConf specialTipConfig = Singleton<NewbieGuideDataManager>.GetInstance().GetSpecialTipConfig(conf.wSpecialTip);
        CUICommonSystem.SetTextContent(obj2.get_transform().FindChild("RightSpecial/Text"), specialTipConfig.szTipText);
        RectTransform transform = obj2.get_transform() as RectTransform;
        Vector2 vector = new Vector2();
        vector.x = specialTipConfig.iOffsetX;
        vector.y = specialTipConfig.iOffsetY;
        transform.set_anchoredPosition(vector);
        CUITimerScript componetInChild = Utility.GetComponetInChild<CUITimerScript>(obj2, "Timer");
        if (conf.Param[0] != 0)
        {
            componetInChild.SetTotalTime((float) conf.Param[0]);
        }
        componetInChild.StartTimer();
    }

    public bool AddEffect(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl, out GameObject highligter)
    {
        GameObject obj2;
        highligter = null;
        switch (conf.dwType)
        {
            case 1:
                highligter = obj2 = this.ShowPvPEffect(conf, inControl);
                return (obj2 != null);

            case 2:
                highligter = obj2 = this.ShowPvEEffect(conf, inControl);
                return (obj2 != null);

            case 3:
                highligter = obj2 = this.ShowFullHeroPanelEffect(conf, inControl);
                return (obj2 != null);

            case 4:
                highligter = obj2 = this.ShowHeroSelConfirmEffect(conf, inControl);
                return (obj2 != null);

            case 5:
                highligter = obj2 = this.ShowHumanMatch33Effect(conf, inControl);
                return (obj2 != null);

            case 6:
                highligter = obj2 = this.ShowBattleHeroSelEffect(conf, inControl);
                return (obj2 != null);

            case 7:
                highligter = obj2 = this.ShowClickPVPBtnEffect(conf, inControl);
                return (obj2 != null);

            case 8:
                highligter = obj2 = this.ShowClickMatch55(conf, inControl);
                return (obj2 != null);

            case 9:
                highligter = obj2 = this.ShowClickStartMatch55(conf, inControl);
                return (obj2 != null);

            case 10:
                highligter = obj2 = this.ShowClickWinShare(conf, inControl);
                return (obj2 != null);

            case 11:
            {
                if ((conf.Param[0] != 0) && (conf.Param[1] != 0))
                {
                    int index = Random.Range(0, 2);
                    uint nextStep = conf.Param[index];
                    inControl.Complete(conf.dwID, nextStep);
                    return true;
                }
                object[] objArray1 = new object[] { conf.dwType };
                DebugHelper.Assert(false, "newbieguide Invalide config -- {0}", objArray1);
                return false;
            }
            case 12:
                highligter = obj2 = this.ShowClickRankBtn(conf, inControl);
                return (obj2 != null);

            case 13:
                highligter = obj2 = this.ShowClickTrainBtn(conf, inControl);
                return (obj2 != null);

            case 14:
                highligter = obj2 = this.ShowClickTrainWheelDisc(conf, inControl);
                return (obj2 != null);

            case 15:
                highligter = obj2 = this.ShowClickMatch55Melee(conf, inControl);
                return (obj2 != null);

            case 0x10:
                highligter = obj2 = this.ShowClickMatchingConfirmBoxConfirm(conf, inControl);
                return (obj2 != null);

            case 0x11:
                highligter = obj2 = this.ShowClickVictoryTipsBtn(conf, inControl);
                return (obj2 != null);

            case 0x12:
                highligter = obj2 = this.ShowClickSaveReplayKit(conf, inControl);
                return (obj2 != null);

            case 0x15:
                highligter = obj2 = this.ShowClickSymbolDraw(conf, inControl);
                return (obj2 != null);

            case 0x16:
                highligter = obj2 = this.ShowClickCommomEquip(conf, inControl);
                return (obj2 != null);

            case 0x17:
                this.ShowBubbleTipText(conf, inControl);
                return true;

            case 0x18:
                highligter = obj2 = this.ShowBackHomeTip(conf, inControl);
                return (obj2 != null);
        }
        object[] inParameters = new object[] { conf.dwType };
        DebugHelper.Assert(false, "Invalide NewbieGuideWeakGuideType -- {0}", inParameters);
        return false;
    }

    private GameObject AddEffectInternal(GameObject effectParent, NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl, CUIFormScript inOriginalForm)
    {
        GameObject content = Singleton<CResourceManager>.GetInstance().GetResource(WEAK_EFFECT_PATH, typeof(GameObject), enResourceType.UIPrefab, false, false).m_content as GameObject;
        GameObject obj3 = Object.Instantiate(content) as GameObject;
        obj3.get_transform().SetParent(effectParent.get_transform());
        Transform transform = effectParent.get_transform().FindChild("Panel");
        if ((transform != null) && transform.get_gameObject().get_activeInHierarchy())
        {
            obj3.get_transform().SetParent(transform);
        }
        obj3.get_transform().set_localScale(Vector3.get_one());
        obj3.get_transform().set_position(effectParent.get_transform().get_position());
        (obj3.get_transform() as RectTransform).set_anchoredPosition(new Vector2((float) conf.iOffsetHighLightX, (float) conf.iOffsetHighLightY));
        if (conf.bNotShowArrow != 0)
        {
            obj3.get_transform().FindChild("Panel/ImageFinger").get_gameObject().CustomSetActive(false);
        }
        this.AddEffectText(conf, effectParent, inOriginalForm);
        return obj3;
    }

    private void AddEffectText(NewbieGuideWeakConf conf, GameObject inParentObj, CUIFormScript inOriginalForm)
    {
        this.ClearEffectText();
        if ((conf != null) && (conf.wSpecialTip > 0))
        {
            NewbieGuideSpecialTipConf specialTipConfig = Singleton<NewbieGuideDataManager>.GetInstance().GetSpecialTipConfig(conf.wSpecialTip);
            if (specialTipConfig != null)
            {
                if (specialTipConfig.bGuideTextPos == 0)
                {
                    if (this.m_guideTextStatic != null)
                    {
                        this.m_guideTextStatic.CustomSetActive(true);
                        Transform transform = this.m_guideTextStatic.get_transform().FindChild("RightSpecial/Text");
                        if (transform != null)
                        {
                            Text component = transform.GetComponent<Text>();
                            if (component != null)
                            {
                                component.set_text(StringHelper.UTF8BytesToString(ref specialTipConfig.szTipText));
                            }
                        }
                    }
                }
                else
                {
                    this.m_guideTextObj = NewbieGuideBaseScript.InstantiateGuideText(specialTipConfig, inParentObj, this.m_formWeakGuide, inOriginalForm);
                }
            }
        }
        this.m_conf = conf;
        this.m_parentObj = inParentObj;
        this.m_originalForm = inOriginalForm;
        if (this.m_formWeakGuide != null)
        {
            this.m_formWeakGuide.SetPriority(this.m_originalForm.m_priority + 1);
        }
    }

    public void ClearEffectText()
    {
        this.m_guideTextStatic.CustomSetActive(false);
        if (this.m_guideTextObj != null)
        {
            this.m_guideTextObj.CustomSetActive(false);
            this.m_guideTextObj = null;
        }
    }

    public void CloseGuideForm()
    {
        if (this.m_formWeakGuide != null)
        {
            this.m_guideTextStatic.CustomSetActive(false);
            this.m_guideTextStatic = null;
            Singleton<CUIManager>.GetInstance().CloseForm(this.m_formWeakGuide);
            this.m_formWeakGuide = null;
        }
        this.m_guideTextStatic = null;
        this.m_parentObj = null;
        this.m_guideTextObj = null;
        this.m_originalForm = null;
    }

    public void Init()
    {
    }

    public void OpenGuideForm()
    {
        if (this.m_formWeakGuide == null)
        {
            this.m_formWeakGuide = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/System/Dialog/Form_WeakGuide", true, true);
            if (this.m_formWeakGuide != null)
            {
                Transform transform = this.m_formWeakGuide.get_transform().FindChild("GuideTextStatic");
                if (transform != null)
                {
                    this.m_guideTextStatic = transform.get_gameObject();
                    if (this.m_guideTextStatic != null)
                    {
                        this.m_guideTextStatic.CustomSetActive(false);
                    }
                }
            }
        }
    }

    public void RemoveEffectText(NewbieGuideWeakConf conf)
    {
        if (conf != null)
        {
            this.ClearEffectText();
        }
    }

    private GameObject ShowBackHomeTip(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
    {
        CSkillButtonManager manager = (Singleton<CBattleSystem>.GetInstance().FightForm != null) ? Singleton<CBattleSystem>.GetInstance().FightForm.m_skillButtonManager : null;
        if (manager != null)
        {
            SkillButton button = manager.GetButton(SkillSlotType.SLOT_SKILL_6);
            return this.AddBubbleEffectInternal(button.m_button, conf, inControl, Singleton<CBattleSystem>.GetInstance().FightFormScript);
        }
        return null;
    }

    private GameObject ShowBattleHeroSelEffect(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
    {
        CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CHeroSelectNormalSystem.s_heroSelectFormPath);
        if (form == null)
        {
            return null;
        }
        uint num = conf.Param[0];
        string str = string.Format("PanelLeft/ListHostHeroInfo/ScrollRect/Content/ListElement_{0}/heroItemCell", num);
        GameObject effectParent = form.get_gameObject().get_transform().Find(str).get_gameObject();
        return this.AddEffectInternal(effectParent, conf, inControl, form);
    }

    private void ShowBubbleTipText(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
    {
        this.AddBubbleText(conf, inControl);
    }

    private GameObject ShowClickCommomEquip(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
    {
        CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CEquipSystem.s_CustomRecommendEquipPath);
        if (form == null)
        {
            return null;
        }
        Transform transform = form.get_transform().FindChild("Panel_Main/Panel_EquipCustom/Panel_EquipCustomContent/godEquipButton");
        if (transform == null)
        {
            return null;
        }
        return this.AddBubbleEffectInternal(transform.get_gameObject(), conf, inControl, form);
    }

    private GameObject ShowClickMatch55(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
    {
        CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_ENTRY);
        if (form == null)
        {
            return null;
        }
        GameObject effectParent = form.get_gameObject().get_transform().FindChild("panelGroup2/btnGroup/Button4").get_gameObject();
        return this.AddEffectInternal(effectParent, conf, inControl, form);
    }

    private GameObject ShowClickMatch55Melee(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
    {
        CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_ENTRY);
        if (form == null)
        {
            return null;
        }
        GameObject effectParent = form.get_gameObject().get_transform().Find("panelGroup2/btnGroup/Button3").get_gameObject();
        return this.AddEffectInternal(effectParent, conf, inControl, form);
    }

    private GameObject ShowClickMatchingConfirmBoxConfirm(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
    {
        CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_CONFIRMBOX);
        if (form == null)
        {
            return null;
        }
        GameObject effectParent = form.get_gameObject().get_transform().Find("Panel/Panel/btnGroup/Button_Confirm").get_gameObject();
        return this.AddEffectInternal(effectParent, conf, inControl, form);
    }

    private GameObject ShowClickPVPBtnEffect(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
    {
        CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_ENTRY);
        if (form == null)
        {
            return null;
        }
        GameObject effectParent = form.get_gameObject().get_transform().FindChild("panelGroup1/btnGroup/Button1").get_gameObject();
        return this.AddEffectInternal(effectParent, conf, inControl, form);
    }

    private GameObject ShowClickRankBtn(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
    {
        CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CLobbySystem.LOBBY_FORM_PATH);
        if (form == null)
        {
            return null;
        }
        GameObject effectParent = form.get_gameObject().get_transform().FindChild("BtnCon/LadderBtn").get_gameObject();
        return this.AddEffectInternal(effectParent, conf, inControl, form);
    }

    private GameObject ShowClickSaveReplayKit(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
    {
        CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(SettlementSystem.SettlementFormName);
        if (form == null)
        {
            return null;
        }
        GameObject effectParent = form.get_transform().FindChild("Panel/ButtonGrid/BtnSaveReplay").get_gameObject();
        return this.AddEffectInternal(effectParent, conf, inControl, form);
    }

    private GameObject ShowClickStartMatch55(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
    {
        CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_MULTI);
        if (form == null)
        {
            return null;
        }
        GameObject effectParent = form.get_gameObject().get_transform().FindChild("Panel_Main/Btn_Matching").get_gameObject();
        return this.AddEffectInternal(effectParent, conf, inControl, form);
    }

    private GameObject ShowClickSymbolDraw(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
    {
        CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(Singleton<CMallSystem>.GetInstance().sMallFormPath);
        if (form == null)
        {
            return null;
        }
        Transform transform = form.get_transform().FindChild("pnlBodyBg/Panel_SymbolMake/Panel_SymbolDraw/btnJump");
        if (transform == null)
        {
            return null;
        }
        return this.AddBubbleEffectInternal(transform.get_gameObject(), conf, inControl, form);
    }

    private GameObject ShowClickTrainBtn(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
    {
        CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_ENTRY);
        if (form == null)
        {
            return null;
        }
        GameObject effectParent = form.get_gameObject().get_transform().Find("panelGroupBottom/ButtonTrain").get_gameObject();
        return this.AddEffectInternal(effectParent, conf, inControl, form);
    }

    private GameObject ShowClickTrainWheelDisc(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
    {
        CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_ENTRY);
        if (form == null)
        {
            return null;
        }
        GameObject effectParent = form.get_gameObject().get_transform().Find("panelGroup4/btnGroup/Button2").get_gameObject();
        return this.AddEffectInternal(effectParent, conf, inControl, form);
    }

    private GameObject ShowClickVictoryTipsBtn(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
    {
        CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(SettlementSystem.SettlementFormName);
        if (form == null)
        {
            return null;
        }
        Transform transform = form.GetWidget(0x18).get_transform();
        GameObject effectParent = transform.FindChild("Btn").get_gameObject();
        PlayerKDA hostKDA = null;
        if ((Singleton<BattleLogic>.GetInstance().battleStat != null) && (Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat != null))
        {
            hostKDA = Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat.GetHostKDA();
        }
        string text = string.Empty;
        if (hostKDA == null)
        {
            text = Singleton<CTextManager>.GetInstance().GetText("WinTrick_Tips_DefaultHeroName");
        }
        else
        {
            ListView<HeroKDA>.Enumerator enumerator = hostKDA.GetEnumerator();
            uint key = 0;
            while (enumerator.MoveNext())
            {
                HeroKDA current = enumerator.Current;
                if (current != null)
                {
                    key = (uint) current.HeroId;
                    break;
                }
            }
            ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(key);
            if (dataByKey != null)
            {
                text = dataByKey.szName;
            }
            else
            {
                text = Singleton<CTextManager>.GetInstance().GetText("WinTrick_Tips_DefaultHeroName");
            }
        }
        transform.FindChild("Panel_Guide").get_gameObject().CustomSetActive(true);
        string[] args = new string[] { text };
        transform.FindChild("Panel_Guide/Text").GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("WinTrick_Tips_text", args));
        return this.AddEffectInternal(effectParent, conf, inControl, form);
    }

    private GameObject ShowClickWinShare(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
    {
        CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(SettlementSystem.SettlementFormName);
        if (form == null)
        {
            return null;
        }
        GameObject effectParent = form.get_gameObject().get_transform().FindChild("Panel/ButtonGrid/ButtonShare").get_gameObject();
        return this.AddEffectInternal(effectParent, conf, inControl, form);
    }

    private GameObject ShowFullHeroPanelEffect(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
    {
        CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CHeroSelectNormalSystem.s_heroSelectFormPath);
        if (form == null)
        {
            return null;
        }
        GameObject effectParent = form.get_gameObject().get_transform().Find("PanelLeft/ListHostHeroInfo/btnOpenFullHeroPanel").get_gameObject();
        return this.AddEffectInternal(effectParent, conf, inControl, form);
    }

    private GameObject ShowHeroSelConfirmEffect(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
    {
        CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CHeroSelectNormalSystem.s_heroSelectFormPath);
        if (form == null)
        {
            return null;
        }
        GameObject effectParent = form.get_gameObject().get_transform().Find("PanelRight/btnConfirm").get_gameObject();
        return this.AddEffectInternal(effectParent, conf, inControl, form);
    }

    private GameObject ShowHumanMatch33Effect(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
    {
        CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_ENTRY);
        if (form == null)
        {
            return null;
        }
        GameObject effectParent = form.get_gameObject().get_transform().Find("panelGroup3/btnGroup/Button3").get_gameObject();
        return this.AddEffectInternal(effectParent, conf, inControl, form);
    }

    private GameObject ShowPvEEffect(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
    {
        CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CLobbySystem.LOBBY_FORM_PATH);
        if (form == null)
        {
            return null;
        }
        GameObject effectParent = form.get_gameObject().get_transform().Find("BtnCon/PveBtn").get_gameObject();
        return this.AddEffectInternal(effectParent, conf, inControl, form);
    }

    private GameObject ShowPvPEffect(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
    {
        CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CLobbySystem.LOBBY_FORM_PATH);
        if (form == null)
        {
            return null;
        }
        GameObject effectParent = form.get_gameObject().get_transform().Find("BtnCon/PvpBtn").get_gameObject();
        return this.AddEffectInternal(effectParent, conf, inControl, form);
    }

    public void UnInit()
    {
    }

    public void Update()
    {
        if (((this.m_conf == null) || (this.m_parentObj == null)) || ((this.m_guideTextObj == null) || (this.m_originalForm == null)))
        {
            if ((this.m_guideTextStatic != null) || (this.m_guideTextObj != null))
            {
                this.ClearEffectText();
            }
        }
        else
        {
            NewbieGuideSpecialTipConf specialTipConfig = Singleton<NewbieGuideDataManager>.GetInstance().GetSpecialTipConfig(this.m_conf.wSpecialTip);
            if ((specialTipConfig != null) && (specialTipConfig.bGuideTextPos > 0))
            {
                this.m_guideTextObj.CustomSetActive(this.m_parentObj.get_activeInHierarchy() && !this.m_originalForm.IsHided());
                NewbieGuideBaseScript.UpdateGuideTextPos(specialTipConfig, this.m_parentObj, this.m_formWeakGuide, this.m_originalForm, this.m_guideTextObj);
            }
        }
    }
}

