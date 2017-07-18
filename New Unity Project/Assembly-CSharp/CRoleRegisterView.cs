using Assets.Scripts.Framework;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using ResData;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CRoleRegisterView
{
    public static void CloseGameDifSelectForm()
    {
        Singleton<CUIManager>.instance.CloseForm(CRoleRegisterSys.s_gameDifficultSelectFormPath);
    }

    public static void DeactivateInputField()
    {
        CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CRoleRegisterSys.s_roleCreateFormPath);
        if (form != null)
        {
            form.get_transform().FindChild("NameInputText").GetComponent<InputField>().DeactivateInputField();
        }
    }

    public static uint GetGuideLevelHeroTypeBtMobaHeroType(int mobaHeroType)
    {
        uint globeValue = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_NEWBIE_RECOMMEND_HEROTYPE1);
        uint num2 = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_NEWBIE_RECOMMEND_HEROTYPE2);
        uint num3 = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_NEWBIE_RECOMMEND_HEROTYPE3);
        int num4 = mobaHeroType;
        switch ((num4 + 1))
        {
            case 0:
            case 1:
            case 2:
                return globeValue;

            case 3:
                return num3;

            case 4:
            case 5:
                return globeValue;

            case 6:
                return num2;

            case 7:
                return num3;
        }
        return globeValue;
    }

    public static void OpenGameDifSelectForm()
    {
        CUIFormScript script = Singleton<CUIManager>.instance.OpenForm(CRoleRegisterSys.s_gameDifficultSelectFormPath, false, true);
        if (script != null)
        {
            Utility.FindChild(script.get_gameObject(), "ToggleGroup/Toggle1").GetComponent<CUIEventScript>().m_onClickEventParams.tag = 1;
            Utility.FindChild(script.get_gameObject(), "ToggleGroup/Toggle2").GetComponent<CUIEventScript>().m_onClickEventParams.tag = 2;
            Utility.FindChild(script.get_gameObject(), "ToggleGroup/Toggle3").GetComponent<CUIEventScript>().m_onClickEventParams.tag = 3;
            SetGameDifficult(0);
        }
    }

    public static void OpenHeroTypeSelectForm()
    {
        CUIFormScript script = Singleton<CUIManager>.instance.OpenForm(CRoleRegisterSys.s_heroTypeSelectFormPath, false, true);
        if (script != null)
        {
            Utility.FindChild(script.get_gameObject(), "ToggleGroup/Toggle1").GetComponent<CUIEventScript>().m_onClickEventParams.tagUInt = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_NEWBIE_RECOMMEND_HEROTYPE3);
            Utility.FindChild(script.get_gameObject(), "ToggleGroup/Toggle2").GetComponent<CUIEventScript>().m_onClickEventParams.tagUInt = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_NEWBIE_RECOMMEND_HEROTYPE1);
            Utility.FindChild(script.get_gameObject(), "ToggleGroup/Toggle3").GetComponent<CUIEventScript>().m_onClickEventParams.tagUInt = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_NEWBIE_RECOMMEND_HEROTYPE2);
            SetHeroType(0);
        }
    }

    public static void RefreshRecommendTips()
    {
        CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CRoleRegisterSys.s_heroTypeSelectFormPath);
        CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
        if ((form != null) && (masterRoleInfo != null))
        {
            GameObject obj2 = null;
            if (masterRoleInfo.acntMobaInfo.bMobaUsedType == 1)
            {
                uint guideLevelHeroTypeBtMobaHeroType = GetGuideLevelHeroTypeBtMobaHeroType(masterRoleInfo.acntMobaInfo.iRecommendHeroType);
                if (guideLevelHeroTypeBtMobaHeroType == GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_NEWBIE_RECOMMEND_HEROTYPE1))
                {
                    obj2 = form.GetWidget(0).get_transform().FindChild("TuiJian").get_gameObject();
                }
                else if (guideLevelHeroTypeBtMobaHeroType == GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_NEWBIE_RECOMMEND_HEROTYPE2))
                {
                    obj2 = form.GetWidget(1).get_transform().FindChild("TuiJian").get_gameObject();
                }
                else if (guideLevelHeroTypeBtMobaHeroType == GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_NEWBIE_RECOMMEND_HEROTYPE3))
                {
                    obj2 = form.GetWidget(2).get_transform().FindChild("TuiJian").get_gameObject();
                }
            }
            else if (masterRoleInfo.acntMobaInfo.bMobaUsedType == 2)
            {
                obj2 = form.GetWidget(0).get_transform().FindChild("TuiJian").get_gameObject();
            }
            obj2.CustomSetActive(true);
        }
    }

    public static void SetGameDifficult(int difficult)
    {
        CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CRoleRegisterSys.s_gameDifficultSelectFormPath);
        if (form != null)
        {
            GameObject obj2 = Utility.FindChild(form.get_gameObject(), "ConfirmBtn").get_gameObject();
            GameObject obj3 = Utility.FindChild(form.get_gameObject(), "Panel/LevelContent").get_gameObject();
            obj2.CustomSetActive(true);
            obj3.CustomSetActive(false);
            if (form != null)
            {
                obj2.GetComponent<CUIEventScript>().m_onClickEventParams.tag = difficult;
                switch (difficult)
                {
                    case 1:
                        obj2.GetComponent<CUIEventScript>().set_enabled(true);
                        obj2.GetComponent<Button>().set_interactable(true);
                        obj2.GetComponentInChildren<Text>().set_color(Color.get_white());
                        return;

                    case 2:
                        obj2.GetComponent<CUIEventScript>().set_enabled(true);
                        obj2.GetComponent<Button>().set_interactable(true);
                        obj2.GetComponentInChildren<Text>().set_color(Color.get_white());
                        return;

                    case 3:
                        obj2.GetComponent<CUIEventScript>().set_enabled(true);
                        obj2.GetComponent<Button>().set_interactable(true);
                        obj2.GetComponentInChildren<Text>().set_color(Color.get_white());
                        return;
                }
                obj2.GetComponent<CUIEventScript>().set_enabled(false);
                obj2.GetComponent<Button>().set_interactable(false);
                obj2.GetComponentInChildren<Text>().set_color(Color.get_gray());
            }
        }
    }

    public static void SetHeroType(uint heroType)
    {
        CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CRoleRegisterSys.s_heroTypeSelectFormPath);
        if (form != null)
        {
            GameObject obj2 = Utility.FindChild(form.get_gameObject(), "ConfirmBtn").get_gameObject();
            obj2.CustomSetActive(true);
            GameObject widget = form.GetWidget(0);
            GameObject obj4 = form.GetWidget(1);
            GameObject obj5 = form.GetWidget(2);
            obj2.GetComponent<CUIEventScript>().m_onClickEventParams.tag = (int) heroType;
            if (heroType == GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_NEWBIE_RECOMMEND_HEROTYPE1))
            {
                obj2.GetComponent<CUIEventScript>().set_enabled(true);
                obj2.GetComponent<Button>().set_interactable(true);
                obj2.GetComponentInChildren<Text>().set_color(Color.get_white());
                widget.get_transform().FindChild("Background/Checkmark").get_gameObject().CustomSetActive(true);
                obj4.get_transform().FindChild("Background/Checkmark").get_gameObject().CustomSetActive(false);
                obj5.get_transform().FindChild("Background/Checkmark").get_gameObject().CustomSetActive(false);
                widget.get_transform().FindChild("Mask").get_gameObject().CustomSetActive(false);
                obj4.get_transform().FindChild("Mask").get_gameObject().CustomSetActive(true);
                obj5.get_transform().FindChild("Mask").get_gameObject().CustomSetActive(true);
            }
            else if (heroType == GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_NEWBIE_RECOMMEND_HEROTYPE2))
            {
                obj2.GetComponent<CUIEventScript>().set_enabled(true);
                obj2.GetComponent<Button>().set_interactable(true);
                obj2.GetComponentInChildren<Text>().set_color(Color.get_white());
                widget.get_transform().FindChild("Background/Checkmark").get_gameObject().CustomSetActive(false);
                obj4.get_transform().FindChild("Background/Checkmark").get_gameObject().CustomSetActive(true);
                obj5.get_transform().FindChild("Background/Checkmark").get_gameObject().CustomSetActive(false);
                widget.get_transform().FindChild("Mask").get_gameObject().CustomSetActive(true);
                obj4.get_transform().FindChild("Mask").get_gameObject().CustomSetActive(false);
                obj5.get_transform().FindChild("Mask").get_gameObject().CustomSetActive(true);
            }
            else if (heroType == GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_NEWBIE_RECOMMEND_HEROTYPE3))
            {
                obj2.GetComponent<CUIEventScript>().set_enabled(true);
                obj2.GetComponent<Button>().set_interactable(true);
                obj2.GetComponentInChildren<Text>().set_color(Color.get_white());
                widget.get_transform().FindChild("Background/Checkmark").get_gameObject().CustomSetActive(false);
                obj4.get_transform().FindChild("Background/Checkmark").get_gameObject().CustomSetActive(false);
                obj5.get_transform().FindChild("Background/Checkmark").get_gameObject().CustomSetActive(true);
                widget.get_transform().FindChild("Mask").get_gameObject().CustomSetActive(true);
                obj4.get_transform().FindChild("Mask").get_gameObject().CustomSetActive(true);
                obj5.get_transform().FindChild("Mask").get_gameObject().CustomSetActive(false);
            }
            else
            {
                obj2.GetComponent<CUIEventScript>().set_enabled(false);
                obj2.GetComponent<Button>().set_interactable(false);
                obj2.GetComponentInChildren<Text>().set_color(Color.get_gray());
                widget.get_transform().FindChild("Background/Checkmark").get_gameObject().CustomSetActive(false);
                obj4.get_transform().FindChild("Background/Checkmark").get_gameObject().CustomSetActive(false);
                obj5.get_transform().FindChild("Background/Checkmark").get_gameObject().CustomSetActive(false);
                widget.get_transform().FindChild("Mask").get_gameObject().CustomSetActive(false);
                obj4.get_transform().FindChild("Mask").get_gameObject().CustomSetActive(false);
                obj5.get_transform().FindChild("Mask").get_gameObject().CustomSetActive(false);
            }
            obj2.GetComponent<CUIEventScript>().m_onClickEventParams.tagUInt = heroType;
        }
    }

    public static string RoleName
    {
        get
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CRoleRegisterSys.s_roleCreateFormPath);
            if (form != null)
            {
                return CUIUtility.RemoveEmoji(form.get_transform().FindChild("NameInputText").GetComponent<InputField>().get_text());
            }
            return string.Empty;
        }
        set
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CRoleRegisterSys.s_roleCreateFormPath);
            if (form != null)
            {
                InputField component = form.get_transform().FindChild("NameInputText").GetComponent<InputField>();
                component.set_text(value);
                component.MoveTextEnd(false);
            }
        }
    }

    public enum enHeroTypeSelectWidgets
    {
        enTankTypeToggle,
        enAdTypeToggle,
        enApTypeToggle
    }
}

