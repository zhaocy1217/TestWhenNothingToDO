namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [MessageHandlerClass]
    public class CHeroOverviewSystem : Singleton<CHeroOverviewSystem>
    {
        public const string HERO_OVERVIEW_HERO_OWN_FLAG_KEY = "Hero_Overview_Hero_Own_Flag_Key";
        private bool m_bSortDesc;
        protected ListView<IHeroData> m_heroList = new ListView<IHeroData>();
        private CMallSortHelper.HeroViewSortType m_heroSortType;
        private bool m_ownFlag;
        protected enHeroJobType m_selectHeroType;
        public static string s_heroViewFormPath = "UGUI/Form/System/HeroInfo/Form_Hero_Overview.prefab";

        private void FilterHeroData(bool bOnlySort)
        {
            ListView<ResHeroCfgInfo> allHeroList = CHeroDataFactory.GetAllHeroList();
            if (!bOnlySort)
            {
                this.m_heroList.Clear();
                for (int i = 0; i < allHeroList.Count; i++)
                {
                    if (((this.m_selectHeroType == enHeroJobType.All) || (allHeroList[i].bMainJob == ((byte) this.m_selectHeroType))) || (allHeroList[i].bMinorJob == ((byte) this.m_selectHeroType)))
                    {
                        IHeroData data = CHeroDataFactory.CreateHeroData(allHeroList[i].dwCfgID);
                        CMallItem item = new CMallItem(data.cfgID, CMallItem.IconType.Normal);
                        if (this.m_ownFlag)
                        {
                            if (item.Owned(false))
                            {
                                this.m_heroList.Add(data);
                            }
                        }
                        else
                        {
                            this.m_heroList.Add(data);
                        }
                    }
                }
            }
            HeroViewSortImp comparer = CMallSortHelper.CreateHeroViewSorter();
            if (comparer != null)
            {
                comparer.SetSortType(this.m_heroSortType);
                this.m_heroList.Sort(comparer);
                if (this.m_bSortDesc)
                {
                    this.m_heroList.Reverse();
                }
            }
        }

        public IHeroData GetHeroDataBuyIndex(int index)
        {
            if ((index >= 0) && (index < this.m_heroList.Count))
            {
                return this.m_heroList[index];
            }
            return null;
        }

        public int GetHeroIndexByConfigId(uint inCfgId)
        {
            for (int i = 0; i < this.m_heroList.Count; i++)
            {
                IHeroData data = this.m_heroList[i];
                if (data.cfgID == inCfgId)
                {
                    return i;
                }
            }
            return 0;
        }

        public int GetHeroListCount()
        {
            return this.m_heroList.Count;
        }

        public override void Init()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroView_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnOpenHeroViewForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroView_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnHeroView_CloseForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroView_ItemEnable, new CUIEventManager.OnUIEventHandler(this.OnHeroView_ItemEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroView_MenuSelect, new CUIEventManager.OnUIEventHandler(this.OnHeroView_MenuSelect));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroInfo_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_CloseForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroView_SortTypeClick, new CUIEventManager.OnUIEventHandler(this.OnHeroSortTypeClick));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroView_SortTypeSelect, new CUIEventManager.OnUIEventHandler(this.OnHeroSortTypeSelect));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroView_Own_Flag_Change, new CUIEventManager.OnUIEventHandler(this.OnHeroOwnFlagChange));
            Singleton<EventRouter>.instance.AddEventHandler<uint>("HeroAdd", new Action<uint>(this.OnNtyAddHero));
            this.m_ownFlag = PlayerPrefs.GetInt("Hero_Overview_Hero_Own_Flag_Key", 0) != 0;
        }

        private void OnHeroInfo_CloseForm(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_heroViewFormPath);
            if (form != null)
            {
                Singleton<CLobbySystem>.GetInstance().SetTopBarPriority(enFormPriority.Priority0);
                SortHeroList(ref this.m_heroList, this.m_heroSortType, this.m_bSortDesc);
                form.get_transform().Find("Panel_Hero/List").get_gameObject().GetComponent<CUIListScript>().SetElementAmount(this.m_heroList.Count);
            }
        }

        private void OnHeroInfo_Compose(CUIEvent uiEvent)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x456);
            msg.stPkgData.stItemComp.wTargetType = 4;
            msg.stPkgData.stItemComp.dwTargetID = uiEvent.m_eventParams.heroId;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        private void OnHeroInfoChange(uint heroId)
        {
            CHeroInfo info2;
            if ((Singleton<CUIManager>.GetInstance().GetForm(s_heroViewFormPath) != null) && Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetHeroInfo(heroId, out info2, false))
            {
                this.RefreshHeroListElement(heroId);
            }
        }

        private void OnHeroOwnFlagChange(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_heroViewFormPath);
            if (form != null)
            {
                GameObject widget = form.GetWidget(3);
                if (widget != null)
                {
                    this.m_ownFlag = widget.GetComponent<Toggle>().get_isOn();
                    PlayerPrefs.SetInt("Hero_Overview_Hero_Own_Flag_Key", !this.m_ownFlag ? 0 : 1);
                    PlayerPrefs.Save();
                }
                this.RefreshHeroOwnFlag();
                this.ResetHeroListData(true);
            }
        }

        private void OnHeroSortTypeClick(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_heroViewFormPath);
            if (form != null)
            {
                GameObject widget = form.GetWidget(2);
                if (widget != null)
                {
                    widget.CustomSetActive(!widget.get_activeSelf());
                }
            }
        }

        private void OnHeroSortTypeSelect(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_heroViewFormPath);
            if (form != null)
            {
                GameObject widget = form.GetWidget(2);
                if (widget != null)
                {
                    widget.CustomSetActive(false);
                }
                CMallSortHelper.HeroViewSortType srcWidgetIndexInBelongedList = (CMallSortHelper.HeroViewSortType) uiEvent.m_srcWidgetIndexInBelongedList;
                CMallSortHelper.CreateHeroViewSorter().SetSortType(srcWidgetIndexInBelongedList);
                if (this.m_heroSortType != srcWidgetIndexInBelongedList)
                {
                    this.m_bSortDesc = false;
                    this.m_heroSortType = srcWidgetIndexInBelongedList;
                    this.ResetHeroListData(true);
                }
                else if (srcWidgetIndexInBelongedList != CMallSortHelper.HeroViewSortType.Default)
                {
                    this.m_bSortDesc = !this.m_bSortDesc;
                    this.ResetHeroListData(false);
                }
                GameObject obj3 = form.GetWidget(1);
                if (obj3 != null)
                {
                    obj3.GetComponent<Text>().set_text(CMallSortHelper.CreateHeroViewSorter().GetSortTypeName(this.m_heroSortType));
                }
            }
        }

        public void OnHeroView_CloseForm(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.GetInstance().CloseForm(s_heroViewFormPath);
            Singleton<CLobbySystem>.GetInstance().SetTopBarPriority(enFormPriority.Priority1);
        }

        public void OnHeroView_ItemEnable(CUIEvent uiEvent)
        {
            int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
            if ((srcWidgetIndexInBelongedList >= 0) && (srcWidgetIndexInBelongedList < this.m_heroList.Count))
            {
                GameObject listItem = uiEvent.m_srcWidget.get_transform().Find("heroItem").get_gameObject();
                SetPveHeroItemData(uiEvent.m_srcFormScript, listItem, this.m_heroList[srcWidgetIndexInBelongedList]);
            }
        }

        public void OnHeroView_MenuSelect(CUIEvent uiEvent)
        {
            int selectedIndex = uiEvent.m_srcWidget.GetComponent<CUIListScript>().GetSelectedIndex();
            if (selectedIndex != this.m_selectHeroType)
            {
                this.m_selectHeroType = (enHeroJobType) selectedIndex;
                this.ResetHeroListData(true);
            }
        }

        public void OnNtyAddHero(uint id)
        {
            this.ResetHeroListData(true);
        }

        public void OnOpenHeroViewForm(CUIEvent uiEvent)
        {
            CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(s_heroViewFormPath, false, true);
            if (script != null)
            {
                Singleton<CLobbySystem>.GetInstance().SetTopBarPriority(enFormPriority.Priority0);
                this.m_selectHeroType = enHeroJobType.All;
                CMallSortHelper.CreateHeroViewSorter().SetSortType(this.m_heroSortType);
                this.ResetHeroListData(true);
                string text = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_All");
                string str2 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Tank");
                string str3 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Soldier");
                string str4 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Assassin");
                string str5 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Master");
                string str6 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Archer");
                string str7 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Aid");
                string[] titleList = new string[] { text, str2, str3, str4, str5, str6, str7 };
                CUICommonSystem.InitMenuPanel(script.get_transform().Find("Panel_Menu/List").get_gameObject(), titleList, (int) this.m_selectHeroType, true);
                this.RefreshHeroOwnFlag();
                this.ResetHeroSortTypeList();
                CMiShuSystem.SendUIClickToServer(enUIClickReprotID.rp_HeroListBtn);
            }
        }

        private void RefreshHeroListElement(uint heroId)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_heroViewFormPath);
            if (form != null)
            {
                CUIListScript component = form.get_gameObject().get_transform().Find("Panel_Hero/List").get_gameObject().GetComponent<CUIListScript>();
                for (int i = 0; i < this.m_heroList.Count; i++)
                {
                    if (this.m_heroList[i].cfgID == heroId)
                    {
                        CUIListElementScript elemenet = component.GetElemenet(i);
                        if (elemenet != null)
                        {
                            GameObject listItem = elemenet.get_gameObject().get_transform().Find("heroItem").get_gameObject();
                            SetPveHeroItemData(form, listItem, this.m_heroList[i]);
                        }
                        break;
                    }
                }
            }
        }

        private void RefreshHeroOwnFlag()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_heroViewFormPath);
            if (form != null)
            {
                GameObject widget = form.GetWidget(3);
                if (widget != null)
                {
                    widget.GetComponent<Toggle>().set_isOn(this.m_ownFlag);
                }
            }
        }

        private void ResetHeroListData(bool bResetData = true)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_heroViewFormPath);
            if (form != null)
            {
                CUIListScript component = form.get_gameObject().get_transform().Find("Panel_Hero/List").get_gameObject().GetComponent<CUIListScript>();
                this.FilterHeroData(!bResetData);
                component.SetElementAmount(this.m_heroList.Count);
            }
        }

        private void ResetHeroSortTypeList()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_heroViewFormPath);
            if (form != null)
            {
                GameObject widget = form.GetWidget(2);
                if (widget != null)
                {
                    CUIListScript component = widget.GetComponent<CUIListScript>();
                    if (component != null)
                    {
                        int amount = 4;
                        component.SetElementAmount(amount);
                        for (int i = 0; i < amount; i++)
                        {
                            Transform transform = component.GetElemenet(i).get_transform().Find("Text");
                            if (transform != null)
                            {
                                transform.GetComponent<Text>().set_text(CMallSortHelper.CreateHeroViewSorter().GetSortTypeName((CMallSortHelper.HeroViewSortType) i));
                            }
                        }
                        component.SelectElement((int) this.m_heroSortType, true);
                    }
                    widget.CustomSetActive(false);
                }
                GameObject obj3 = form.GetWidget(1);
                if (obj3 != null)
                {
                    obj3.GetComponent<Text>().set_text(CMallSortHelper.CreateHeroViewSorter().GetSortTypeName(this.m_heroSortType));
                }
            }
        }

        public static void SetPveHeroItemData(CUIFormScript formScript, GameObject listItem, IHeroData data)
        {
            if ((listItem != null) && (data != null))
            {
                bool bPlayerOwn = data.bPlayerOwn;
                Transform transform = listItem.get_transform();
                Transform transform2 = transform.Find("heroProficiencyImg");
                Transform transform3 = transform.Find("heroProficiencyBgImg");
                CUICommonSystem.SetHeroProficiencyIconImage(formScript, transform2.get_gameObject(), data.proficiencyLV);
                CUICommonSystem.SetHeroProficiencyBgImage(formScript, transform3.get_gameObject(), data.proficiencyLV, false);
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if (masterRoleInfo != null)
                {
                    transform3.GetComponent<Image>().set_color(!masterRoleInfo.IsHaveHero(data.cfgID, true) ? CUIUtility.s_Color_GrayShader : Color.get_white());
                    transform2.GetComponent<Image>().set_color(!masterRoleInfo.IsHaveHero(data.cfgID, true) ? CUIUtility.s_Color_GrayShader : Color.get_white());
                }
                bool flag2 = false;
                bool bActive = false;
                bool flag4 = false;
                if (masterRoleInfo != null)
                {
                    flag2 = masterRoleInfo.IsFreeHero(data.cfgID);
                    bActive = masterRoleInfo.IsCreditFreeHero(data.cfgID);
                    flag4 = masterRoleInfo.IsValidExperienceHero(data.cfgID);
                    CUICommonSystem.SetHeroItemImage(formScript, listItem, masterRoleInfo.GetHeroSkinPic(data.cfgID), enHeroHeadType.enBust, (!bPlayerOwn && !flag2) && !flag4, true);
                }
                GameObject root = transform.Find("profession").get_gameObject();
                CUICommonSystem.SetHeroJob(formScript, root, (enHeroJobType) data.heroType);
                transform.Find("heroNameText").GetComponent<Text>().set_text(data.heroName);
                Transform transform4 = transform.Find("TxtFree");
                Transform transform5 = transform.Find("TxtCreditFree");
                if (transform4 != null)
                {
                    transform4.get_gameObject().CustomSetActive(flag2 && !bActive);
                }
                if (transform5 != null)
                {
                    transform5.get_gameObject().CustomSetActive(bActive);
                }
                transform.Find("imgExperienceMark").get_gameObject().CustomSetActive(data.IsValidExperienceHero());
                CUIEventScript component = listItem.GetComponent<CUIEventScript>();
                stUIEventParams eventParams = new stUIEventParams();
                eventParams.openHeroFormPar.heroId = data.cfgID;
                eventParams.openHeroFormPar.openSrc = enHeroFormOpenSrc.HeroListClick;
                component.SetUIEvent(enUIEventType.Click, enUIEventID.HeroInfo_OpenForm, eventParams);
            }
        }

        public static void SortHeroList(ref ListView<IHeroData> heroList, CMallSortHelper.HeroViewSortType sortType = 0, bool bDesc = false)
        {
            if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo() != null)
            {
                HeroViewSortImp comparer = CMallSortHelper.CreateHeroViewSorter();
                if (comparer != null)
                {
                    comparer.SetSortType(sortType);
                    heroList.Sort(comparer);
                }
                if (bDesc)
                {
                    heroList.Reverse();
                }
            }
        }

        public override void UnInit()
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.HeroView_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnOpenHeroViewForm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.HeroView_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnHeroView_CloseForm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.HeroView_ItemEnable, new CUIEventManager.OnUIEventHandler(this.OnHeroView_ItemEnable));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.HeroView_MenuSelect, new CUIEventManager.OnUIEventHandler(this.OnHeroView_MenuSelect));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.HeroInfo_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_CloseForm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.HeroView_SortTypeClick, new CUIEventManager.OnUIEventHandler(this.OnHeroSortTypeClick));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.HeroView_SortTypeSelect, new CUIEventManager.OnUIEventHandler(this.OnHeroSortTypeSelect));
            Singleton<EventRouter>.instance.RemoveEventHandler<uint>("HeroAdd", new Action<uint>(this.OnNtyAddHero));
        }
    }
}

