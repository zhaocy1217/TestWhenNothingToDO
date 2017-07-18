namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class CPlayerCommonHeroInfoController : Singleton<CPlayerCommonHeroInfoController>
    {
        private ListView<COMDT_MOST_USED_HERO_INFO> m_commonHeroList = new ListView<COMDT_MOST_USED_HERO_INFO>();
        private byte m_lastIsUp;
        private int m_lastSortIndex = -1;
        private static string[] sMainTitles = new string[2];
        public static string sPlayerInfoCommonHeroFormPath = "UGUI/Form/System/Player/Form_Player_Info_CommonHeroInfo.prefab";
        private static string[] sSortTitles = new string[7];
        private static string[] sSubTitles = new string[4];

        private int CommonHeroSortKda(COMDT_MOST_USED_HERO_INFO left, COMDT_MOST_USED_HERO_INFO right)
        {
            float num = (left.stStatisticDetail.dwNum <= 0) ? 0f : (CPlayerProfile.Divide(left.stStatisticDetail.astTypeDetail[0].ullKDAPct, left.stStatisticDetail.astTypeDetail[0].dwWinNum + left.stStatisticDetail.astTypeDetail[0].dwLoseNum) * 100f);
            float num2 = (right.stStatisticDetail.dwNum <= 0) ? 0f : (CPlayerProfile.Divide(right.stStatisticDetail.astTypeDetail[0].ullKDAPct, right.stStatisticDetail.astTypeDetail[0].dwWinNum + right.stStatisticDetail.astTypeDetail[0].dwLoseNum) * 100f);
            return (((this.m_lastIsUp != 1) ? 1 : -1) * (((int) num2) - ((int) num)));
        }

        private int CommonHeroSortOrder(COMDT_MOST_USED_HERO_INFO left, COMDT_MOST_USED_HERO_INFO right)
        {
            ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(left.dwHeroID);
            ResHeroCfgInfo info2 = GameDataMgr.heroDatabin.GetDataByKey(right.dwHeroID);
            if ((dataByKey == null) || (info2 == null))
            {
                return 0;
            }
            return (int) (((this.m_lastIsUp != 1) ? 1 : -1) * (info2.dwShowSortId - dataByKey.dwShowSortId));
        }

        private int CommonHeroSortUsedBeHurt(COMDT_MOST_USED_HERO_INFO left, COMDT_MOST_USED_HERO_INFO right)
        {
            float num = (left.stStatisticDetail.dwNum <= 0) ? 0f : (CPlayerProfile.Divide(left.stStatisticDetail.astTypeDetail[0].ullTotalBeHurt, left.stStatisticDetail.astTypeDetail[0].dwWinNum + left.stStatisticDetail.astTypeDetail[0].dwLoseNum) * 100f);
            float num2 = (right.stStatisticDetail.dwNum <= 0) ? 0f : (CPlayerProfile.Divide(right.stStatisticDetail.astTypeDetail[0].ullTotalBeHurt, right.stStatisticDetail.astTypeDetail[0].dwWinNum + right.stStatisticDetail.astTypeDetail[0].dwLoseNum) * 100f);
            return (((this.m_lastIsUp != 1) ? 1 : -1) * (((int) num2) - ((int) num)));
        }

        private int CommonHeroSortUsedCnt(COMDT_MOST_USED_HERO_INFO left, COMDT_MOST_USED_HERO_INFO right)
        {
            return (int) (((this.m_lastIsUp != 1) ? 1 : -1) * (((right.dwGameWinNum + right.dwGameLoseNum) - left.dwGameWinNum) - left.dwGameLoseNum));
        }

        private int CommonHeroSortUsedGold(COMDT_MOST_USED_HERO_INFO left, COMDT_MOST_USED_HERO_INFO right)
        {
            float num = (left.stStatisticDetail.dwNum <= 0) ? 0f : (CPlayerProfile.Divide(left.stStatisticDetail.astTypeDetail[0].dwGPM, left.stStatisticDetail.astTypeDetail[0].dwWinNum + left.stStatisticDetail.astTypeDetail[0].dwLoseNum) * 100f);
            float num2 = (right.stStatisticDetail.dwNum <= 0) ? 0f : (CPlayerProfile.Divide(right.stStatisticDetail.astTypeDetail[0].dwGPM, right.stStatisticDetail.astTypeDetail[0].dwWinNum + right.stStatisticDetail.astTypeDetail[0].dwLoseNum) * 100f);
            return (((this.m_lastIsUp != 1) ? 1 : -1) * (((int) num2) - ((int) num)));
        }

        private int CommonHeroSortUsedHurt(COMDT_MOST_USED_HERO_INFO left, COMDT_MOST_USED_HERO_INFO right)
        {
            float num = (left.stStatisticDetail.dwNum <= 0) ? 0f : (CPlayerProfile.Divide(left.stStatisticDetail.astTypeDetail[0].dwHurtPM, left.stStatisticDetail.astTypeDetail[0].dwWinNum + left.stStatisticDetail.astTypeDetail[0].dwLoseNum) * 100f);
            float num2 = (right.stStatisticDetail.dwNum <= 0) ? 0f : (CPlayerProfile.Divide(right.stStatisticDetail.astTypeDetail[0].dwHurtPM, right.stStatisticDetail.astTypeDetail[0].dwWinNum + right.stStatisticDetail.astTypeDetail[0].dwLoseNum) * 100f);
            return (((this.m_lastIsUp != 1) ? 1 : -1) * (((int) num2) - ((int) num)));
        }

        private int CommonHeroSortWins(COMDT_MOST_USED_HERO_INFO left, COMDT_MOST_USED_HERO_INFO right)
        {
            return (int) (((this.m_lastIsUp != 1) ? 1 : -1) * (((0x3e8 * right.dwGameWinNum) / (right.dwGameWinNum + right.dwGameLoseNum)) - ((0x3e8 * left.dwGameWinNum) / (left.dwGameWinNum + left.dwGameLoseNum))));
        }

        private static byte GetGameType(int subIndex)
        {
            if (subIndex != 0)
            {
                if (subIndex == 1)
                {
                    return 4;
                }
                if (subIndex == 2)
                {
                    return 5;
                }
                if (subIndex == 3)
                {
                    return 11;
                }
            }
            return 0;
        }

        public override void Init()
        {
            sMainTitles = new string[] { Singleton<CTextManager>.instance.GetText("Player_Info_Title_36"), Singleton<CTextManager>.instance.GetText("Player_Info_Title_37") };
            sSubTitles = new string[] { Singleton<CTextManager>.instance.GetText("Player_Info_Title_3"), Singleton<CTextManager>.instance.GetText("Player_Info_Title_4"), Singleton<CTextManager>.instance.GetText("Player_Info_Title_5"), Singleton<CTextManager>.instance.GetText("Player_Info_Title_9") };
            sSortTitles = new string[] { Singleton<CTextManager>.instance.GetText("Player_Info_Title_38"), Singleton<CTextManager>.instance.GetText("Player_Info_Title_39"), Singleton<CTextManager>.instance.GetText("Player_Info_Title_40"), Singleton<CTextManager>.instance.GetText("Player_Info_Title_42"), Singleton<CTextManager>.instance.GetText("Player_Info_Title_43"), Singleton<CTextManager>.instance.GetText("Player_Info_Title_44"), Singleton<CTextManager>.instance.GetText("Player_Info_Title_41") };
            Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.Player_Info_Common_Hero_Enable, new CUIEventManager.OnUIEventHandler(this.OnCommonHeroItemEnable));
            Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.Player_Info_Common_Hero_Main_List_Click, new CUIEventManager.OnUIEventHandler(this.OnCommonHeroMainListClick));
            Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.Player_Info_Common_Hero_Sub_List_Click, new CUIEventManager.OnUIEventHandler(this.OnCommonHeroSubListClick));
            Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.Player_Info_Common_Hero_Sort_List_Click, new CUIEventManager.OnUIEventHandler(this.OnCommonHeroSortListClick));
            Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.Player_Info_Common_Hero_Sub_List_Show, new CUIEventManager.OnUIEventHandler(this.OnCommonHeroSubListShow));
            Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.Player_Info_Common_Hero_Detail_List_Enable, new CUIEventManager.OnUIEventHandler(this.OnCommonHeroListEnable));
        }

        public void InitCommonHeroUI()
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CPlayerInfoSystem.sPlayerInfoFormPath);
            if (form != null)
            {
                CPlayerProfile profile = Singleton<CPlayerInfoSystem>.instance.GetProfile();
                GameObject widget = form.GetWidget(7);
                if (widget != null)
                {
                    CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(widget, "BaseList");
                    if (componetInChild != null)
                    {
                        int count = profile.MostUsedHeroList().Count;
                        uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0xb2).dwConfValue;
                        if (count > dwConfValue)
                        {
                            count = (int) dwConfValue;
                        }
                        componetInChild.SetElementAmount(count);
                    }
                }
            }
        }

        private void OnCommonHeroItemEnable(CUIEvent uiEvent)
        {
            CPlayerProfile profile = Singleton<CPlayerInfoSystem>.instance.GetProfile();
            int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
            GameObject p = Utility.FindChild(uiEvent.m_srcWidget, "heroItem");
            ListView<COMDT_MOST_USED_HERO_INFO> view = profile.MostUsedHeroList();
            if ((view != null) && (srcWidgetIndexInBelongedList < view.Count))
            {
                COMDT_MOST_USED_HERO_INFO comdt_most_used_hero_info = view[srcWidgetIndexInBelongedList];
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if (masterRoleInfo != null)
                {
                    IHeroData data = CHeroDataFactory.CreateHeroData(comdt_most_used_hero_info.dwHeroID);
                    GameObject proficiencyIcon = Utility.FindChild(p, "heroProficiencyImg");
                    GameObject proficiencyBg = Utility.FindChild(p, "heroProficiencyBgImg");
                    CUICommonSystem.SetHeroProficiencyIconImage(uiEvent.m_srcFormScript, proficiencyIcon, (int) comdt_most_used_hero_info.dwProficiencyLv);
                    CUICommonSystem.SetHeroProficiencyBgImage(uiEvent.m_srcFormScript, proficiencyBg, (int) comdt_most_used_hero_info.dwProficiencyLv, false);
                    if (!CPlayerInfoSystem.isSelf(profile.m_uuid))
                    {
                        CUICommonSystem.SetHeroItemImage(uiEvent.m_srcFormScript, p, CSkinInfo.GetHeroSkinPic(comdt_most_used_hero_info.dwHeroID, comdt_most_used_hero_info.dwSkinID), enHeroHeadType.enBust, false, true);
                    }
                    else
                    {
                        CUICommonSystem.SetHeroItemImage(uiEvent.m_srcFormScript, p, masterRoleInfo.GetHeroSkinPic(comdt_most_used_hero_info.dwHeroID), enHeroHeadType.enBust, false, true);
                    }
                    GameObject root = Utility.FindChild(p, "profession");
                    CUICommonSystem.SetHeroJob(uiEvent.m_srcFormScript, root, (enHeroJobType) data.heroType);
                    Utility.GetComponetInChild<Text>(p, "heroNameText").set_text(data.heroName);
                    string[] args = new string[] { (comdt_most_used_hero_info.dwGameWinNum + comdt_most_used_hero_info.dwGameLoseNum).ToString() };
                    Utility.GetComponetInChild<Text>(p, "TotalCount").set_text(Singleton<CTextManager>.instance.GetText("Player_Info_PVP_Total_Count", args));
                    string[] textArray2 = new string[] { CPlayerProfile.Round(CPlayerProfile.Divide(comdt_most_used_hero_info.dwGameWinNum, comdt_most_used_hero_info.dwGameWinNum + comdt_most_used_hero_info.dwGameLoseNum) * 100f) };
                    Utility.GetComponetInChild<Text>(p, "WinRate").set_text(Singleton<CTextManager>.instance.GetText("Player_Info_PVP_Win_Rate", textArray2));
                    ulong num2 = 0L;
                    ulong num3 = 0L;
                    ulong num4 = 0L;
                    uint num5 = 0;
                    COMDT_HERO_STATISTIC_DETAIL stStatisticDetail = comdt_most_used_hero_info.stStatisticDetail;
                    uint dwNum = stStatisticDetail.dwNum;
                    for (int i = 0; i < dwNum; i++)
                    {
                        COMDT_HERO_STATISTIC_INFO comdt_hero_statistic_info = stStatisticDetail.astTypeDetail[i];
                        num2 += comdt_hero_statistic_info.ullKDAPct;
                        num3 += comdt_hero_statistic_info.ullTotalHurtHero;
                        num4 += comdt_hero_statistic_info.ullTotalBeHurt;
                        num5 = (num5 + comdt_hero_statistic_info.dwWinNum) + comdt_hero_statistic_info.dwLoseNum;
                    }
                    num5 = (num5 != 0) ? num5 : 1;
                    string[] textArray3 = new string[1];
                    textArray3[0] = ((num2 / ((ulong) num5)) / ((ulong) 100L)).ToString("0.0");
                    Utility.GetComponetInChild<Text>(p, "AverKDA").set_text(Singleton<CTextManager>.instance.GetText("Player_Info_PVP_AverKDA", textArray3));
                    string[] textArray4 = new string[1];
                    textArray4[0] = (num3 / ((ulong) num5)).ToString("d");
                    Utility.GetComponetInChild<Text>(p, "AverHurt").set_text(Singleton<CTextManager>.instance.GetText("Player_Info_PVP_AverHurt", textArray4));
                    string[] textArray5 = new string[1];
                    textArray5[0] = (num4 / ((ulong) num5)).ToString("d");
                    Utility.GetComponetInChild<Text>(p, "AverTakenHurt").set_text(Singleton<CTextManager>.instance.GetText("Player_Info_PVP_AverTakenHurt", textArray5));
                }
            }
        }

        public void OnCommonHeroListEnable(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(sPlayerInfoCommonHeroFormPath);
            if (form != null)
            {
                int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
                GameObject srcWidget = uiEvent.m_srcWidget;
                if ((srcWidgetIndexInBelongedList >= 0) && (srcWidgetIndexInBelongedList < this.m_commonHeroList.Count))
                {
                    Text componetInChild = Utility.GetComponetInChild<Text>(srcWidget, "NameTxt");
                    Text text2 = Utility.GetComponetInChild<Text>(srcWidget, "UsedCntTxt");
                    Text text3 = Utility.GetComponetInChild<Text>(srcWidget, "WinsTxt");
                    Text text4 = Utility.GetComponetInChild<Text>(srcWidget, "KdaTxt");
                    Text text5 = Utility.GetComponetInChild<Text>(srcWidget, "GoldTxt");
                    Text text6 = Utility.GetComponetInChild<Text>(srcWidget, "HurtTxt");
                    Text text7 = Utility.GetComponetInChild<Text>(srcWidget, "BeHurtTxt");
                    COMDT_MOST_USED_HERO_INFO comdt_most_used_hero_info = this.m_commonHeroList[srcWidgetIndexInBelongedList];
                    ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(comdt_most_used_hero_info.dwHeroID);
                    string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Icon_Dir, CSkinInfo.GetHeroSkinPic(this.m_commonHeroList[srcWidgetIndexInBelongedList].dwHeroID, 0));
                    Utility.GetComponetInChild<Image>(srcWidget, "HeadImg").SetSprite(prefabPath, form, true, false, false, false);
                    Utility.GetComponetInChild<Text>(srcWidget, "NameTxt").set_text((dataByKey != null) ? dataByKey.szName : string.Empty);
                    uint num14 = comdt_most_used_hero_info.dwGameWinNum + comdt_most_used_hero_info.dwGameLoseNum;
                    Utility.GetComponetInChild<Text>(srcWidget, "UsedCntTxt").set_text(num14.ToString());
                    Utility.GetComponetInChild<Text>(srcWidget, "WinsTxt").set_text(CPlayerProfile.Divide(comdt_most_used_hero_info.dwGameWinNum, comdt_most_used_hero_info.dwGameWinNum + comdt_most_used_hero_info.dwGameLoseNum).ToString("P0"));
                    if (comdt_most_used_hero_info.stStatisticDetail.dwNum > 0)
                    {
                        byte gameType = GetGameType(Utility.GetComponetInChild<CUIListScript>(form.GetWidget(0), "SubMenuList/List").GetSelectedIndex());
                        uint b = 0;
                        uint num5 = 0;
                        uint num6 = 0;
                        uint a = 0;
                        uint num8 = 0;
                        uint num9 = 0;
                        uint num10 = 0;
                        uint num11 = 0;
                        ulong num12 = 0L;
                        for (int i = 0; i < comdt_most_used_hero_info.stStatisticDetail.dwNum; i++)
                        {
                            if ((gameType == 0) || (comdt_most_used_hero_info.stStatisticDetail.astTypeDetail[i].bGameType == gameType))
                            {
                                num5 += comdt_most_used_hero_info.stStatisticDetail.astTypeDetail[i].dwWinNum;
                                num6 += comdt_most_used_hero_info.stStatisticDetail.astTypeDetail[i].dwLoseNum;
                                a += comdt_most_used_hero_info.stStatisticDetail.astTypeDetail[i].dwKill;
                                num8 += comdt_most_used_hero_info.stStatisticDetail.astTypeDetail[i].dwDead;
                                num9 += comdt_most_used_hero_info.stStatisticDetail.astTypeDetail[i].dwAssist;
                                num10 += comdt_most_used_hero_info.stStatisticDetail.astTypeDetail[i].dwGPM;
                                num11 += comdt_most_used_hero_info.stStatisticDetail.astTypeDetail[i].dwHurtPM;
                                num12 += comdt_most_used_hero_info.stStatisticDetail.astTypeDetail[i].ullTotalBeHurt;
                            }
                        }
                        b = num5 + num6;
                        text4.set_text(string.Format("{0}/{1}/{2}", CPlayerProfile.Divide(a, b).ToString("F1"), CPlayerProfile.Divide(num8, b).ToString("F1"), CPlayerProfile.Divide(num9, b).ToString("F1")));
                        text5.set_text(CPlayerProfile.Divide(num10, b).ToString("F0"));
                        text6.set_text(CPlayerProfile.Divide(num11, b).ToString("F0"));
                        text7.set_text(CPlayerProfile.Divide(num12, b).ToString("F0"));
                    }
                    else
                    {
                        text4.set_text("0.0/0.0/0.0");
                        text5.set_text("0");
                        text6.set_text("0");
                        text7.set_text("0");
                    }
                }
            }
        }

        public void OnCommonHeroMainListClick(CUIEvent uiEvent)
        {
            this.OpenForm();
        }

        public void OnCommonHeroSortListClick(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(sPlayerInfoCommonHeroFormPath);
            if (form != null)
            {
                GameObject widget = form.GetWidget(0);
                CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(widget, "SubMenuList/List");
                int selectedIndex = Utility.GetComponetInChild<CUIListScript>(widget, "DetailList/MenuList").GetSelectedIndex();
                this.UpdateSortMenu();
                this.SrotCommonHeroList(selectedIndex);
                this.UpdateUI();
            }
        }

        public void OnCommonHeroSubListClick(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(sPlayerInfoCommonHeroFormPath);
            if (form != null)
            {
                byte gameType = GetGameType(Utility.GetComponetInChild<CUIListScript>(form.GetWidget(0), "SubMenuList/List").GetSelectedIndex());
                this.m_commonHeroList.Clear();
                ListView<COMDT_MOST_USED_HERO_INFO> view = Singleton<CPlayerInfoSystem>.instance.GetProfile().MostUsedHeroList();
                for (int i = 0; i < view.Count; i++)
                {
                    for (int j = 0; j < view[i].stStatisticDetail.dwNum; j++)
                    {
                        if (((gameType == 0) || (view[i].stStatisticDetail.astTypeDetail[j].bGameType == gameType)) && ((view[i].stStatisticDetail.astTypeDetail[j].dwWinNum + view[i].stStatisticDetail.astTypeDetail[j].dwLoseNum) > 0))
                        {
                            this.m_commonHeroList.Add(view[i]);
                            break;
                        }
                    }
                }
                this.UpdateUI();
            }
        }

        public void OnCommonHeroSubListShow(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(sPlayerInfoCommonHeroFormPath);
            if (form != null)
            {
                GameObject obj3 = Utility.FindChild(form.GetWidget(0), "SubMenuList/List");
                obj3.CustomSetActive(!obj3.get_activeSelf());
            }
        }

        public void OpenForm()
        {
            CUIFormScript script = Singleton<CUIManager>.instance.OpenForm(sPlayerInfoCommonHeroFormPath, false, true);
            if (script != null)
            {
                CPlayerProfile profile = Singleton<CPlayerInfoSystem>.instance.GetProfile();
                GameObject widget = script.GetWidget(0);
                if (widget != null)
                {
                    this.m_lastSortIndex = -1;
                    this.m_lastIsUp = 0;
                    CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(widget, "SubMenuList/List");
                    CUIListScript script3 = Utility.GetComponetInChild<CUIListScript>(widget, "DetailList/MenuList");
                    CUICommonSystem.InitMenuPanel(componetInChild.get_gameObject(), sSubTitles, 0, true);
                    CUICommonSystem.InitMenuPanel(script3.get_gameObject(), sSortTitles, 0, true);
                }
            }
        }

        private void SrotCommonHeroList(int sortIndex = 0)
        {
            if (sortIndex == 0)
            {
                this.m_commonHeroList.Sort(new Comparison<COMDT_MOST_USED_HERO_INFO>(this.CommonHeroSortOrder));
            }
            else if (sortIndex == 1)
            {
                this.m_commonHeroList.Sort(new Comparison<COMDT_MOST_USED_HERO_INFO>(this.CommonHeroSortUsedCnt));
            }
            else if (sortIndex == 2)
            {
                this.m_commonHeroList.Sort(new Comparison<COMDT_MOST_USED_HERO_INFO>(this.CommonHeroSortWins));
            }
            else if (sortIndex == 3)
            {
                this.m_commonHeroList.Sort(new Comparison<COMDT_MOST_USED_HERO_INFO>(this.CommonHeroSortUsedGold));
            }
            else if (sortIndex == 4)
            {
                this.m_commonHeroList.Sort(new Comparison<COMDT_MOST_USED_HERO_INFO>(this.CommonHeroSortUsedHurt));
            }
            else if (sortIndex == 5)
            {
                this.m_commonHeroList.Sort(new Comparison<COMDT_MOST_USED_HERO_INFO>(this.CommonHeroSortUsedBeHurt));
            }
            else if (sortIndex == 6)
            {
                this.m_commonHeroList.Sort(new Comparison<COMDT_MOST_USED_HERO_INFO>(this.CommonHeroSortKda));
            }
        }

        public override void UnInit()
        {
            Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.Player_Info_Common_Hero_Enable, new CUIEventManager.OnUIEventHandler(this.OnCommonHeroItemEnable));
            Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.Player_Info_Common_Hero_Main_List_Click, new CUIEventManager.OnUIEventHandler(this.OnCommonHeroMainListClick));
            Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.Player_Info_Common_Hero_Sub_List_Click, new CUIEventManager.OnUIEventHandler(this.OnCommonHeroSubListClick));
            Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.Player_Info_Common_Hero_Sort_List_Click, new CUIEventManager.OnUIEventHandler(this.OnCommonHeroSortListClick));
            Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.Player_Info_Common_Hero_Sub_List_Show, new CUIEventManager.OnUIEventHandler(this.OnCommonHeroSubListShow));
            Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.Player_Info_Common_Hero_Detail_List_Enable, new CUIEventManager.OnUIEventHandler(this.OnCommonHeroListEnable));
        }

        private void UpdateSortMenu()
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(sPlayerInfoCommonHeroFormPath);
            if (form != null)
            {
                CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(form.GetWidget(0), "DetailList/MenuList");
                int selectedIndex = componetInChild.GetSelectedIndex();
                if (this.m_lastSortIndex != selectedIndex)
                {
                    this.m_lastIsUp = 0;
                }
                else
                {
                    this.m_lastIsUp = (this.m_lastIsUp != 1) ? ((byte) 1) : ((byte) 0);
                }
                this.m_lastSortIndex = selectedIndex;
                int elementAmount = componetInChild.m_elementAmount;
                GameObject p = null;
                GameObject obj4 = null;
                GameObject obj5 = null;
                for (int i = 0; i < elementAmount; i++)
                {
                    p = componetInChild.GetElemenet(i).get_gameObject();
                    obj4 = Utility.FindChild(p, "Text/Up");
                    obj5 = Utility.FindChild(p, "Text/Down");
                    if (i == selectedIndex)
                    {
                        obj4.CustomSetActive(this.m_lastIsUp == 1);
                        obj5.CustomSetActive(this.m_lastIsUp != 1);
                    }
                    else
                    {
                        obj4.CustomSetActive(false);
                        obj5.CustomSetActive(false);
                    }
                }
            }
        }

        public void UpdateUI()
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(sPlayerInfoCommonHeroFormPath);
            if (form != null)
            {
                GameObject widget = form.GetWidget(0);
                CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(widget, "SubMenuList/List");
                CUIListScript script3 = Utility.GetComponetInChild<CUIListScript>(widget, "DetailList/MenuList");
                CUIListScript script4 = Utility.GetComponetInChild<CUIListScript>(widget, "DetailList/List");
                Text text = Utility.GetComponetInChild<Text>(widget, "SubMenuList/Button_Down/Text");
                int selectedIndex = componetInChild.GetSelectedIndex();
                int num2 = script3.GetSelectedIndex();
                componetInChild.get_gameObject().CustomSetActive(false);
                text.set_text(sSubTitles[selectedIndex]);
                script4.SetElementAmount(this.m_commonHeroList.Count);
            }
        }
    }
}

