using Assets.Scripts.Framework;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using ResData;
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class CAddSkillView
{
    public static readonly uint HeroID = 0x6d;
    public static readonly Color SELECTED_COLOR = Color.get_white();
    public static readonly Color UN_SELECTED_COLOR = new Color(0.3333333f, 0.5019608f, 0.5882353f);

    public static bool NewPlayerLevelUnlockAddSkill(int inNewLevel, int inOldLevel, out uint outSkillId)
    {
        outSkillId = 0;
        int num = GameDataMgr.addedSkiilDatabin.Count();
        for (int i = 0; i < num; i++)
        {
            ResSkillUnlock dataByIndex = GameDataMgr.addedSkiilDatabin.GetDataByIndex(i);
            if (dataByIndex != null)
            {
                int wAcntLevel = dataByIndex.wAcntLevel;
                if ((inNewLevel >= wAcntLevel) && (inOldLevel < wAcntLevel))
                {
                    outSkillId = dataByIndex.dwUnlockSkillID;
                    return true;
                }
            }
        }
        return false;
    }

    public static void OnRefresh(GameObject form, ushort addedSkillLevel)
    {
        CUIToggleListScript component = form.get_transform().Find("Panel_Grid/ToggleList").GetComponent<CUIToggleListScript>();
        CUIListElementScript elemenet = null;
        int selected = component.GetSelected();
        for (int i = 0; i < component.GetElementAmount(); i++)
        {
            elemenet = component.GetElemenet(i);
            if (i == selected)
            {
                elemenet.get_transform().Find("SkillNameTxt").GetComponent<Text>().set_color(SELECTED_COLOR);
            }
            else
            {
                elemenet.get_transform().Find("SkillNameTxt").GetComponent<Text>().set_color(UN_SELECTED_COLOR);
            }
        }
        ResSkillUnlock dataByKey = GameDataMgr.addedSkiilDatabin.GetDataByKey((uint) addedSkillLevel);
        uint dwUnlockSkillID = dataByKey.dwUnlockSkillID;
        ResSkillCfgInfo info = GameDataMgr.skillDatabin.GetDataByKey(dwUnlockSkillID);
        if (info == null)
        {
            DebugHelper.Assert(false, string.Format("ResSkillCfgInfo[{0}] can not be found!", dwUnlockSkillID));
        }
        else
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            bool flag = (masterRoleInfo == null) || (masterRoleInfo.PvpLevel < dataByKey.wAcntLevel);
            Image image = form.get_transform().Find("Panel_SkillDesc/IconImg").GetComponent<Image>();
            Image image2 = form.get_transform().Find("Panel_SkillDesc/ContentImg").GetComponent<Image>();
            Text text = form.get_transform().Find("Panel_SkillDesc/SkillNameTxt").GetComponent<Text>();
            Text text2 = form.get_transform().Find("Panel_SkillDesc/SkillNameTxt2").GetComponent<Text>();
            Text text3 = form.get_transform().Find("Panel_SkillDesc/SkillUnlockTxt").GetComponent<Text>();
            Text text4 = form.get_transform().Find("Panel_SkillDesc/SkillDescTxt").GetComponent<Text>();
            string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Skill_Dir, Utility.UTF8Convert(info.szIconPath));
            image.SetSprite(prefabPath, form.GetComponent<CUIFormScript>(), true, false, false, false);
            prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_AddedSkill_Dir, dwUnlockSkillID);
            image2.SetSprite(prefabPath, form.GetComponent<CUIFormScript>(), true, false, false, false);
            string str2 = Utility.UTF8Convert(info.szSkillName);
            text2.set_text(str2);
            text.set_text(str2);
            text3.set_text(string.Format("Lv.{0}", dataByKey.wAcntLevel));
            text4.set_text(CUICommonSystem.GetSkillDescLobby(info.szSkillDesc, HeroID));
            if (flag)
            {
                string[] args = new string[] { dataByKey.wAcntLevel.ToString() };
                text3.set_text(Singleton<CTextManager>.instance.GetText("Added_Skill_Common_Tips_3", args));
            }
            else
            {
                text3.set_text(Singleton<CTextManager>.instance.GetText("Added_Skill_Common_Tips_4"));
            }
        }
    }

    public static void OpenForm(GameObject form)
    {
        CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
        int amount = GameDataMgr.addedSkiilDatabin.Count();
        CUIToggleListScript component = form.get_transform().Find("Panel_Grid/ToggleList").GetComponent<CUIToggleListScript>();
        component.SetElementAmount(amount);
        CUIListElementScript elemenet = null;
        CUIEventScript script3 = null;
        ResSkillUnlock dataByIndex = null;
        ResSkillCfgInfo dataByKey = null;
        uint key = 0;
        bool bActive = true;
        form.get_transform().Find("Panel_TopBg/LevelText").GetComponent<Text>().set_text((masterRoleInfo == null) ? Singleton<CTextManager>.instance.GetText("Added_Skill_Common_Tips_2", new string[] { "1" }) : Singleton<CTextManager>.instance.GetText("Added_Skill_Common_Tips_2", new string[] { masterRoleInfo.PvpLevel.ToString() }));
        for (int i = 0; i < amount; i++)
        {
            elemenet = component.GetElemenet(i);
            script3 = elemenet.GetComponent<CUIEventScript>();
            dataByIndex = GameDataMgr.addedSkiilDatabin.GetDataByIndex(i);
            key = dataByIndex.dwUnlockSkillID;
            dataByKey = GameDataMgr.skillDatabin.GetDataByKey(key);
            bActive = (masterRoleInfo == null) || (masterRoleInfo.PvpLevel < dataByIndex.wAcntLevel);
            if (dataByKey != null)
            {
                string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Skill_Dir, Utility.UTF8Convert(dataByKey.szIconPath));
                Image image = elemenet.get_transform().Find("Icon").GetComponent<Image>();
                image.SetSprite(prefabPath, form.GetComponent<CUIFormScript>(), true, false, false, false);
                script3.m_onClickEventID = enUIEventID.AddedSkill_GetDetail;
                script3.m_onClickEventParams.tag = dataByIndex.wAcntLevel;
                elemenet.get_transform().Find("SkillNameTxt").GetComponent<Text>().set_text(Utility.UTF8Convert(dataByKey.szSkillName));
                elemenet.get_transform().Find("Lock").get_gameObject().CustomSetActive(bActive);
                image.set_color(!bActive ? Color.get_white() : CUIUtility.s_Color_GrayShader);
                if (bActive)
                {
                    string[] args = new string[] { dataByIndex.wAcntLevel.ToString() };
                    Utility.GetComponetInChild<Text>(elemenet.get_gameObject(), "Lock/Text").set_text(Singleton<CTextManager>.instance.GetText("Added_Skill_Common_Tips_3", args));
                }
            }
            else
            {
                DebugHelper.Assert(false, string.Format("ResSkillCfgInfo[{0}] can not be found!", key));
            }
        }
        dataByIndex = GameDataMgr.addedSkiilDatabin.GetDataByIndex(0);
        if (dataByIndex != null)
        {
            component.SelectElement(0, true);
            OnRefresh(form, dataByIndex.wAcntLevel);
        }
        if (CSysDynamicBlock.bLobbyEntryBlocked)
        {
            CUIListElementScript script5 = form.get_transform().FindChild("Panel_Grid/ToggleList").GetComponent<CUIToggleListScript>().GetElemenet(10);
            if (script5 != null)
            {
                script5.get_gameObject().CustomSetActive(false);
            }
            Transform transform = form.get_transform().FindChild("Skill-Send-Test");
            if (transform != null)
            {
                transform.get_gameObject().CustomSetActive(false);
            }
        }
    }
}

