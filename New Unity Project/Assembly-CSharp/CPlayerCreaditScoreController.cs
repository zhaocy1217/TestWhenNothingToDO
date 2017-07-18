using Assets.Scripts.Framework;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using UnityEngine;
using UnityEngine.UI;

[MessageHandlerClass]
public class CPlayerCreaditScoreController : Singleton<CPlayerCreaditScoreController>
{
    private ResCreditLevelInfo m_CreditLevelInfo;

    public void Draw(CUIFormScript form)
    {
        this.UpdateCreditScore(form);
    }

    private string GetBgByCreditLevel(RES_CREDIT_LEVEL_TYPE level)
    {
        switch (level)
        {
            case RES_CREDIT_LEVEL_TYPE.RES_CREDIT_LEVEL_TYPE_POOR:
                return (CUIUtility.s_Sprite_Dynamic_Icon_Dir + "160006");

            case RES_CREDIT_LEVEL_TYPE.RES_CREDIT_LEVEL_TYPE_GOOD:
                return (CUIUtility.s_Sprite_Dynamic_Icon_Dir + "160007");

            case RES_CREDIT_LEVEL_TYPE.RES_CREDIT_LEVEL_TYPE_EXCELLENT:
                return (CUIUtility.s_Sprite_Dynamic_Icon_Dir + "160008");
        }
        return (CUIUtility.s_Sprite_Dynamic_Icon_Dir + "160008");
    }

    public ResCreditLevelInfo GetCreditLevelInfo(int creditLevel)
    {
        ResCreditLevelInfo dataByIndex = null;
        int count = GameDataMgr.creditLevelDatabin.count;
        for (int i = 0; i < count; i++)
        {
            dataByIndex = GameDataMgr.creditLevelDatabin.GetDataByIndex(i);
            if (dataByIndex.bCreditLevel == creditLevel)
            {
                return dataByIndex;
            }
            dataByIndex = null;
        }
        return dataByIndex;
    }

    public ResCreditLevelInfo GetCreditLevelInfoByScore(int creditScore)
    {
        ResCreditLevelInfo anyData = GameDataMgr.creditLevelDatabin.GetAnyData();
        int count = GameDataMgr.creditLevelDatabin.count;
        for (int i = 0; i < count; i++)
        {
            anyData = GameDataMgr.creditLevelDatabin.GetDataByIndex(i);
            if ((anyData.dwCreditThresholdLow <= creditScore) && (anyData.dwCreditThresholdHigh >= creditScore))
            {
                return anyData;
            }
        }
        return anyData;
    }

    private string GetSuffixByProportion(float proportion)
    {
        string str = "Green";
        if ((proportion * 100f) > 66f)
        {
            return "Red";
        }
        if ((proportion * 100f) > 33f)
        {
            str = "Yellow";
        }
        return str;
    }

    private string GetTipsByComplaintTypeAndProportion(uint type, float proportion)
    {
        string suffixByProportion = this.GetSuffixByProportion(proportion);
        COM_CHGCREDIT_TYPE com_chgcredit_type = (COM_CHGCREDIT_TYPE) type;
        return Singleton<CTextManager>.GetInstance().GetText(string.Format("Credit_Score_Tips_{0}_{1}", com_chgcredit_type, suffixByProportion));
    }

    private string GetTipsIconByProportion(float proportion)
    {
        if ((proportion * 100f) > 66f)
        {
            return (CUIUtility.s_Sprite_Dynamic_Icon_Dir + "160003");
        }
        if ((proportion * 100f) > 33f)
        {
            return (CUIUtility.s_Sprite_Dynamic_Icon_Dir + "160002");
        }
        return (CUIUtility.s_Sprite_Dynamic_Icon_Dir + "160001");
    }

    public override void Init()
    {
        base.Init();
        Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Player_Info_Credit_Score_Reward_Enable, new CUIEventManager.OnUIEventHandler(this.OnRewardEnable));
    }

    public void Load(CUIFormScript form)
    {
        if (form != null)
        {
            CUICommonSystem.LoadUIPrefab("UGUI/Form/System/Player/CreditScoreInfo", "pnlCreditScoreInfo", form.GetWidget(9), form);
        }
    }

    public bool Loaded(CUIFormScript form)
    {
        if (form == null)
        {
            return false;
        }
        GameObject widget = form.GetWidget(9);
        if (widget == null)
        {
            return false;
        }
        if (Utility.FindChild(widget, "pnlCreditScoreInfo") == null)
        {
            return false;
        }
        return true;
    }

    private void OnRewardEnable(CUIEvent uiEvent)
    {
        CUIListElementScript srcWidgetScript = uiEvent.m_srcWidgetScript as CUIListElementScript;
        if (srcWidgetScript != null)
        {
            int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
            if (((this.m_CreditLevelInfo != null) && (srcWidgetIndexInBelongedList >= 0)) && (srcWidgetIndexInBelongedList < this.m_CreditLevelInfo.astCreditRewardDetail.Length))
            {
                Text componetInChild = Utility.GetComponetInChild<Text>(srcWidgetScript.get_gameObject(), "itemCell/ItemName");
                Image image = Utility.GetComponetInChild<Image>(srcWidgetScript.get_gameObject(), "itemCell/imgIcon");
                componetInChild.set_text(this.m_CreditLevelInfo.astCreditRewardDetail[srcWidgetIndexInBelongedList].szCreditRewardItemDesc);
                image.SetSprite(string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Icon_Dir, this.m_CreditLevelInfo.astCreditRewardDetail[srcWidgetIndexInBelongedList].szCreditRewardItemIcon), uiEvent.m_srcFormScript, true, false, false, false);
            }
        }
    }

    public override void UnInit()
    {
        base.UnInit();
        this.m_CreditLevelInfo = null;
        Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Player_Info_Credit_Score_Reward_Enable, new CUIEventManager.OnUIEventHandler(this.OnRewardEnable));
    }

    private void UpdateCreditScore(CUIFormScript form)
    {
        if (form != null)
        {
            GameObject widget = form.GetWidget(9);
            if (widget != null)
            {
                GameObject obj3 = Utility.FindChild(widget, "pnlCreditScoreInfo");
                if (obj3 != null)
                {
                    obj3.CustomSetActive(true);
                    CPlayerProfile profile = Singleton<CPlayerInfoSystem>.GetInstance().GetProfile();
                    uint creditScore = profile.creditScore;
                    this.m_CreditLevelInfo = this.GetCreditLevelInfoByScore((int) creditScore);
                    if (this.m_CreditLevelInfo != null)
                    {
                        Text componetInChild = Utility.GetComponetInChild<Text>(obj3, "pnlContainer/pnlCreditScore/CreditValue/ScoreValue");
                        GameObject obj4 = Utility.FindChild(obj3, "pnlContainer/pnlCreditScore/CreditValue/CreditLevel/LevelValue");
                        GameObject p = Utility.FindChild(obj3, "pnlContainer/pnlCreditAward/SelfAward");
                        GameObject obj6 = Utility.FindChild(obj3, "pnlContainer/pnlCreditAward/ComplaintInfo");
                        if (componetInChild != null)
                        {
                            componetInChild.set_text(creditScore.ToString());
                        }
                        Image image = Utility.GetComponetInChild<Image>(obj3, "pnlContainer/pnlCreditScore/IconBg");
                        if (image != null)
                        {
                            image.SetSprite(this.GetBgByCreditLevel((RES_CREDIT_LEVEL_TYPE) this.m_CreditLevelInfo.bCreditLevel), form, true, false, false, false);
                        }
                        if (obj4 != null)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                obj4.get_transform().GetChild(i).get_gameObject().CustomSetActive(this.m_CreditLevelInfo.bCreditLevel > i);
                            }
                        }
                        if (p != null)
                        {
                            GameObject obj7 = Utility.FindChild(p, "Title-Red");
                            GameObject obj8 = Utility.FindChild(p, "TitleTxt_Red");
                            GameObject obj9 = Utility.FindChild(p, "Title-Blue");
                            GameObject obj10 = Utility.FindChild(p, "TitleTxt_Blue");
                            if (this.m_CreditLevelInfo.bCreditLevelResult == 0)
                            {
                                obj7.CustomSetActive(true);
                                obj8.CustomSetActive(true);
                                obj9.CustomSetActive(false);
                                obj10.CustomSetActive(false);
                                if (obj8 != null)
                                {
                                    obj8.GetComponent<Text>().set_text(Singleton<CTextManager>.instance.GetText("Credit_Punish_Title"));
                                }
                            }
                            else
                            {
                                obj7.CustomSetActive(false);
                                obj8.CustomSetActive(false);
                                obj9.CustomSetActive(true);
                                obj10.CustomSetActive(true);
                                if (obj10 != null)
                                {
                                    obj10.GetComponent<Text>().set_text(Singleton<CTextManager>.instance.GetText("Credit_Reward_Title"));
                                }
                            }
                            int amount = 0;
                            for (int j = 0; j < this.m_CreditLevelInfo.astCreditRewardDetail.Length; j++)
                            {
                                if (!string.IsNullOrEmpty(this.m_CreditLevelInfo.astCreditRewardDetail[j].szCreditRewardItemIcon))
                                {
                                    amount++;
                                }
                            }
                            CUIListScript script = Utility.GetComponetInChild<CUIListScript>(p, "pnlAward");
                            if (script != null)
                            {
                                script.SetElementAmount(amount);
                            }
                            if (amount > 0)
                            {
                                p.CustomSetActive(true);
                            }
                            else
                            {
                                p.CustomSetActive(false);
                            }
                        }
                        if (obj6 != null)
                        {
                            uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x12e).dwConfValue;
                            Text text4 = Utility.GetComponetInChild<Text>(obj6, "Progress/progressBg/txtProgress");
                            Image image2 = Utility.GetComponetInChild<Image>(obj6, "Tips/Image");
                            Text text5 = Utility.GetComponetInChild<Text>(obj6, "Tips/Text");
                            float proportion = Utility.Divide((uint) profile.sumDelCreditValue, dwConfValue);
                            GameObject obj11 = null;
                            if ((proportion * 100f) > 66f)
                            {
                                obj11 = Utility.FindChild(obj6, "Progress/progressBg/imgProgress_Red");
                                Utility.FindChild(obj6, "Progress/progressBg/imgProgress_Yellow").CustomSetActive(false);
                                Utility.FindChild(obj6, "Progress/progressBg/imgProgress_Green").CustomSetActive(false);
                            }
                            else if ((proportion * 100f) > 33f)
                            {
                                Utility.FindChild(obj6, "Progress/progressBg/imgProgress_Red").CustomSetActive(false);
                                obj11 = Utility.FindChild(obj6, "Progress/progressBg/imgProgress_Yellow");
                                Utility.FindChild(obj6, "Progress/progressBg/imgProgress_Green").CustomSetActive(false);
                            }
                            else
                            {
                                Utility.FindChild(obj6, "Progress/progressBg/imgProgress_Red").CustomSetActive(false);
                                Utility.FindChild(obj6, "Progress/progressBg/imgProgress_Yellow").CustomSetActive(false);
                                obj11 = Utility.FindChild(obj6, "Progress/progressBg/imgProgress_Green");
                            }
                            obj11.CustomSetActive(true);
                            obj11.GetComponent<Image>().set_fillAmount(proportion);
                            text4.set_text(string.Format("{0}/{1}", profile.sumDelCreditValue, dwConfValue));
                            image2.SetSprite(this.GetTipsIconByProportion(proportion), form, true, false, false, false);
                            text5.set_text(this.GetTipsByComplaintTypeAndProportion(profile.mostDelCreditType, proportion));
                        }
                    }
                }
            }
        }
    }
}

