namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.UI;
    using ResData;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class CMallHeroController
    {
        private enHeroJobType m_heroJobType;
        private ListView<ResHeroCfgInfo> m_heroList = new ListView<ResHeroCfgInfo>();
        private bool m_notOwnFlag;
        public const string MALL_HERO_OWN_FLAG_KEY = "Mall_Hero_Own_Flag_Key";

        public void Draw(CUIFormScript form)
        {
            if (form != null)
            {
                GameObject obj2 = form.get_transform().Find("pnlBodyBg/pnlBuyHero").get_gameObject();
                if (obj2 != null)
                {
                    obj2.CustomSetActive(true);
                    this.m_heroJobType = enHeroJobType.All;
                    string text = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_All");
                    string str2 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Tank");
                    string str3 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Soldier");
                    string str4 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Assassin");
                    string str5 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Master");
                    string str6 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Archer");
                    string str7 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Aid");
                    string[] titleList = new string[] { text, str2, str3, str4, str5, str6, str7 };
                    Transform transform = obj2.get_transform().Find("MenuList");
                    this.RefreshHeroOwnFlag();
                    CUICommonSystem.InitMenuPanel(transform.get_gameObject(), titleList, (int) this.m_heroJobType, true);
                }
            }
        }

        public IHeroData GetHeroDataByIndex(int index)
        {
            if (((index >= 0) && (this.m_heroList != null)) && ((this.m_heroList[index] != null) && (index < this.m_heroList.Count)))
            {
                return CHeroDataFactory.CreateHeroData(this.m_heroList[index].dwCfgID);
            }
            return null;
        }

        public int GetHeroIndexByConfigId(uint heroID = 0)
        {
            if (heroID != 0)
            {
                for (int i = 0; i < this.m_heroList.Count; i++)
                {
                    if ((this.m_heroList[i] != null) && (this.m_heroList[i].dwCfgID == heroID))
                    {
                        return i;
                    }
                }
            }
            return 0;
        }

        public int GetHeroListCount()
        {
            if (this.m_heroList == null)
            {
                return 0;
            }
            return this.m_heroList.Count;
        }

        public void Init()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_HeroItem_Enable, new CUIEventManager.OnUIEventHandler(this.OnHeroItemEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Hero_JobSelect, new CUIEventManager.OnUIEventHandler(this.OnHeroJobSelect));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Appear, new CUIEventManager.OnUIEventHandler(this.OnMallAppear));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Jump_Form, new CUIEventManager.OnUIEventHandler(this.OnMallJumpForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Hero_Own_Flag_Change, new CUIEventManager.OnUIEventHandler(this.OnHeroOwnFlagChange));
            Singleton<EventRouter>.instance.AddEventHandler<uint>("HeroAdd", new Action<uint>(this.OnNtyAddHero));
            Singleton<EventRouter>.instance.AddEventHandler(EventID.Mall_Sort_Type_Changed, new Action(this, (IntPtr) this.OnSortTypeChanged));
            this.m_notOwnFlag = PlayerPrefs.GetInt("Mall_Hero_Own_Flag_Key", 0) != 0;
        }

        public void Load(CUIFormScript form)
        {
            CUICommonSystem.LoadUIPrefab("UGUI/Form/System/Mall/BuyHero", "pnlBuyHero", form.GetWidget(3), form);
        }

        public bool Loaded(CUIFormScript form)
        {
            if (Utility.FindChild(form.GetWidget(3), "pnlBuyHero") == null)
            {
                return false;
            }
            return true;
        }

        private void OnHeroItemEnable(CUIEvent uiEvent)
        {
            int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
            if (((srcWidgetIndexInBelongedList >= 0) && (srcWidgetIndexInBelongedList < this.m_heroList.Count)) && (uiEvent.m_srcWidget != null))
            {
                CMallItemWidget component = uiEvent.m_srcWidget.GetComponent<CMallItemWidget>();
                if ((component != null) && (uiEvent.m_srcWidget != null))
                {
                    CMallItem item = new CMallItem(this.m_heroList[srcWidgetIndexInBelongedList].dwCfgID, CMallItem.IconType.Normal);
                    Singleton<CMallSystem>.GetInstance().SetMallItem(component, item);
                }
            }
        }

        private void OnHeroJobSelect(CUIEvent uiEvent)
        {
            int selectedIndex = uiEvent.m_srcWidget.GetComponent<CUIListScript>().GetSelectedIndex();
            this.m_heroJobType = (enHeroJobType) selectedIndex;
            this.RefreshHeroListObject(uiEvent.m_srcFormScript);
        }

        private void OnHeroOwnFlagChange(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(Singleton<CMallSystem>.GetInstance().sMallFormPath);
            if (form != null)
            {
                GameObject obj2 = Utility.FindChild(form.GetWidget(3), "pnlBuyHero/ownFlag");
                if (obj2 != null)
                {
                    this.m_notOwnFlag = obj2.GetComponent<Toggle>().get_isOn();
                    PlayerPrefs.SetInt("Mall_Hero_Own_Flag_Key", !this.m_notOwnFlag ? 0 : 1);
                    PlayerPrefs.Save();
                }
                this.RefreshHeroOwnFlag();
                this.RefreshHeroListObject(form);
            }
        }

        private void OnMallAppear(CUIEvent uiEvent)
        {
            this.RefreshHeroListObject(uiEvent.m_srcFormScript);
        }

        private void OnMallJumpForm(CUIEvent uiEvent)
        {
            CUICommonSystem.JumpForm((RES_GAME_ENTRANCE_TYPE) uiEvent.m_eventParams.tag, 0, 0);
        }

        private void OnNtyAddHero(uint heroId)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(Singleton<CMallSystem>.GetInstance().sMallFormPath);
            if (form != null)
            {
                this.RefreshHeroListObject(form);
            }
        }

        private void OnSortTypeChanged()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(Singleton<CMallSystem>.GetInstance().sMallFormPath);
            if ((form != null) && (Singleton<CMallSystem>.GetInstance().CurTab == CMallSystem.Tab.Hero))
            {
                this.RefreshHeroListObject(form);
            }
        }

        private void RefreshHeroListObject(CUIFormScript form)
        {
            if ((form != null) && (Singleton<CMallSystem>.GetInstance().CurTab == CMallSystem.Tab.Hero))
            {
                this.ResetHeroList();
                this.SortHeroList();
                CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(form.get_gameObject(), "pnlBodyBg/pnlBuyHero/List");
                if (componetInChild != null)
                {
                    componetInChild.SetElementAmount(this.m_heroList.Count);
                }
            }
        }

        private void RefreshHeroOwnFlag()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(Singleton<CMallSystem>.GetInstance().sMallFormPath);
            if (form != null)
            {
                GameObject obj2 = Utility.FindChild(form.GetWidget(3), "pnlBuyHero/ownFlag");
                if (obj2 != null)
                {
                    obj2.GetComponent<Toggle>().set_isOn(this.m_notOwnFlag);
                }
            }
        }

        private void ResetHeroList()
        {
            this.m_heroList.Clear();
            ListView<ResHeroCfgInfo> allHeroListAtShop = CHeroDataFactory.GetAllHeroListAtShop();
            for (int i = 0; i < allHeroListAtShop.Count; i++)
            {
                CMallItem item = new CMallItem(allHeroListAtShop[i].dwCfgID, CMallItem.IconType.Normal);
                if (((this.m_heroJobType == enHeroJobType.All) || (allHeroListAtShop[i].bMainJob == ((byte) this.m_heroJobType))) || (allHeroListAtShop[i].bMinorJob == ((byte) this.m_heroJobType)))
                {
                    if (this.m_notOwnFlag)
                    {
                        if (!item.Owned(false))
                        {
                            this.m_heroList.Add(allHeroListAtShop[i]);
                        }
                    }
                    else
                    {
                        this.m_heroList.Add(allHeroListAtShop[i]);
                    }
                }
            }
        }

        private void SortHeroList()
        {
            if (this.m_heroList != null)
            {
                this.m_heroList.Sort(CMallSortHelper.CreateHeroSorter());
                if (CMallSortHelper.CreateHeroSorter().IsDesc())
                {
                    this.m_heroList.Reverse();
                }
            }
        }

        public void UnInit()
        {
        }
    }
}

