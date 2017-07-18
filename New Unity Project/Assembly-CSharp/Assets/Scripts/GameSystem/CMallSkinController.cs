namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class CMallSkinController
    {
        private enHeroJobType m_heroJobType;
        private bool m_notOwnFlag;
        private ListView<ResHeroSkin> m_skinList = new ListView<ResHeroSkin>();
        public const string MALL_SKIN_OWN_FLAG_KEY = "Mall_Skin_Own_Flag_Key";

        public void Draw(CUIFormScript form)
        {
            if (form != null)
            {
                GameObject obj2 = form.get_transform().Find("pnlBodyBg/pnlBuySkin").get_gameObject();
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
                    this.RefreshSkinOwnFlag();
                    CUICommonSystem.InitMenuPanel(transform.get_gameObject(), titleList, (int) this.m_heroJobType, true);
                }
            }
        }

        public ResHeroSkin GetSkinDataByIndex(int index)
        {
            if (((index >= 0) && (this.m_skinList != null)) && ((this.m_skinList[index] != null) && (index < this.m_skinList.Count)))
            {
                return this.m_skinList[index];
            }
            return null;
        }

        public int GetSkinIndexByConfigId(uint uniSkinID = 0)
        {
            if (uniSkinID != 0)
            {
                for (int i = 0; i < this.m_skinList.Count; i++)
                {
                    if ((this.m_skinList[i] != null) && (this.m_skinList[i].dwID == uniSkinID))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public int GetSkinListCount()
        {
            if (this.m_skinList == null)
            {
                return 0;
            }
            return this.m_skinList.Count;
        }

        public void Init()
        {
            CSkinInfo.InitHeroSkinDicData();
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_SkinItem_Enable, new CUIEventManager.OnUIEventHandler(this.OnSkinItemEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Skin_JobSelect, new CUIEventManager.OnUIEventHandler(this.OnSkinJobSelect));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Appear, new CUIEventManager.OnUIEventHandler(this.OnMallAppear));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Skin_Own_Flag_Change, new CUIEventManager.OnUIEventHandler(this.OnSkinOwnFlagChange));
            Singleton<EventRouter>.instance.AddEventHandler<uint>("HeroAdd", new Action<uint>(this.OnNtyHeroInfoChange));
            Singleton<EventRouter>.instance.AddEventHandler<uint, uint, uint>("HeroSkinAdd", new Action<uint, uint, uint>(this, (IntPtr) this.OnNtyHeroInfoChangeBySkinAdd));
            Singleton<EventRouter>.instance.AddEventHandler(EventID.Mall_Sort_Type_Changed, new Action(this, (IntPtr) this.OnSortTypeChanged));
            Singleton<EventRouter>.instance.AddEventHandler(EventID.SERVER_SKIN_DATABIN_READY, new Action(this, (IntPtr) this.OnServerSkinDatabinReady));
            this.m_notOwnFlag = PlayerPrefs.GetInt("Mall_Skin_Own_Flag_Key", 0) != 0;
        }

        public void Load(CUIFormScript form)
        {
            CUICommonSystem.LoadUIPrefab("UGUI/Form/System/Mall/BuySkin", "pnlBuySkin", form.GetWidget(3), form);
        }

        public bool Loaded(CUIFormScript form)
        {
            if (Utility.FindChild(form.GetWidget(3), "pnlBuySkin") == null)
            {
                return false;
            }
            return true;
        }

        private void OnMallAppear(CUIEvent uiEvent)
        {
            this.RefreshSkinListObject(uiEvent.m_srcFormScript);
        }

        private void OnNtyHeroInfoChange(uint heroId)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(Singleton<CMallSystem>.GetInstance().sMallFormPath);
            if (form != null)
            {
                this.RefreshSkinListObject(form);
            }
        }

        private void OnNtyHeroInfoChangeBySkinAdd(uint heroId, uint skinId, uint addReason)
        {
            if (Singleton<CUIManager>.GetInstance().GetForm(Singleton<CMallSystem>.GetInstance().sMallFormPath) != null)
            {
                this.OnNtyHeroInfoChange(heroId);
            }
        }

        private void OnServerSkinDatabinReady()
        {
            CSkinInfo.InitHeroSkinDicData();
        }

        private void OnSkinItemEnable(CUIEvent uiEvent)
        {
            int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
            if (((srcWidgetIndexInBelongedList >= 0) && (srcWidgetIndexInBelongedList < this.m_skinList.Count)) && (uiEvent.m_srcWidget != null))
            {
                CMallItemWidget component = uiEvent.m_srcWidget.GetComponent<CMallItemWidget>();
                if ((component != null) && (uiEvent.m_srcWidget != null))
                {
                    CMallItem item = new CMallItem(this.m_skinList[srcWidgetIndexInBelongedList].dwHeroID, this.m_skinList[srcWidgetIndexInBelongedList].dwSkinID, CMallItem.IconType.Normal);
                    Singleton<CMallSystem>.GetInstance().SetMallItem(component, item);
                }
            }
        }

        private void OnSkinJobSelect(CUIEvent uiEvent)
        {
            int selectedIndex = uiEvent.m_srcWidget.GetComponent<CUIListScript>().GetSelectedIndex();
            this.m_heroJobType = (enHeroJobType) selectedIndex;
            this.RefreshSkinListObject(uiEvent.m_srcFormScript);
        }

        private void OnSkinOwnFlagChange(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(Singleton<CMallSystem>.GetInstance().sMallFormPath);
            if (form != null)
            {
                GameObject obj2 = Utility.FindChild(form.GetWidget(3), "pnlBuySkin/ownFlag");
                if (obj2 != null)
                {
                    this.m_notOwnFlag = obj2.GetComponent<Toggle>().get_isOn();
                    PlayerPrefs.SetInt("Mall_Skin_Own_Flag_Key", !this.m_notOwnFlag ? 0 : 1);
                    PlayerPrefs.Save();
                }
                this.RefreshSkinOwnFlag();
                this.RefreshSkinListObject(form);
            }
        }

        private void OnSortTypeChanged()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(Singleton<CMallSystem>.GetInstance().sMallFormPath);
            if ((form != null) && (Singleton<CMallSystem>.GetInstance().CurTab == CMallSystem.Tab.Skin))
            {
                this.RefreshSkinListObject(form);
            }
        }

        private void RefreshSkinListObject(CUIFormScript form)
        {
            if (((form != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen) && (Singleton<CMallSystem>.GetInstance().CurTab == CMallSystem.Tab.Skin))
            {
                Transform transform = form.get_transform().Find("pnlBodyBg/pnlBuySkin");
                if (transform != null)
                {
                    this.ResetSkinList();
                    this.SortSkinList();
                    transform.Find("List").get_gameObject().GetComponent<CUIListScript>().SetElementAmount(this.m_skinList.Count);
                }
            }
        }

        private void RefreshSkinOwnFlag()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(Singleton<CMallSystem>.GetInstance().sMallFormPath);
            if (form != null)
            {
                GameObject obj2 = Utility.FindChild(form.GetWidget(3), "pnlBuySkin/ownFlag");
                if (obj2 != null)
                {
                    obj2.GetComponent<Toggle>().set_isOn(this.m_notOwnFlag);
                }
            }
        }

        private void ResetSkinList()
        {
            this.m_skinList.Clear();
            Dictionary<long, object>.Enumerator enumerator = GameDataMgr.heroSkinDatabin.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<long, object> current = enumerator.Current;
                ResHeroSkin skin = current.Value as ResHeroSkin;
                if (((skin != null) && (skin.dwSkinID != 0)) && GameDataMgr.IsSkinAvailableAtShop(skin.dwID))
                {
                    ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(skin.dwHeroID);
                    if ((dataByKey != null) && GameDataMgr.IsHeroAvailable(dataByKey.dwCfgID))
                    {
                        CMallItem item = new CMallItem(skin.dwHeroID, skin.dwSkinID, CMallItem.IconType.Normal);
                        if (((this.m_heroJobType == enHeroJobType.All) || (dataByKey.bMainJob == ((byte) this.m_heroJobType))) || (dataByKey.bMinorJob == ((byte) this.m_heroJobType)))
                        {
                            if (this.m_notOwnFlag)
                            {
                                if (!item.Owned(false))
                                {
                                    this.m_skinList.Add(skin);
                                }
                            }
                            else
                            {
                                this.m_skinList.Add(skin);
                            }
                        }
                    }
                }
            }
        }

        private void SortSkinList()
        {
            if (this.m_skinList != null)
            {
                this.m_skinList.Sort(CMallSortHelper.CreateSkinSorter());
                if (CMallSortHelper.CreateSkinSorter().IsDesc())
                {
                    this.m_skinList.Reverse();
                }
            }
        }

        public void UnInit()
        {
        }
    }
}

