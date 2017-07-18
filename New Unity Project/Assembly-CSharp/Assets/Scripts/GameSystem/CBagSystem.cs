namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    [MessageHandlerClass]
    public class CBagSystem : Singleton<CBagSystem>
    {
        public CUseableContainer m_ContainerAll = new CUseableContainer(enCONTAINER_TYPE.UNKNOWN);
        public CUseableContainer m_ContainerExpCard = new CUseableContainer(enCONTAINER_TYPE.UNKNOWN);
        public CUseableContainer m_ContainerGift = new CUseableContainer(enCONTAINER_TYPE.UNKNOWN);
        public CUseableContainer m_ContainerItem = new CUseableContainer(enCONTAINER_TYPE.UNKNOWN);
        public CUseableContainer m_ContainerRecentGet = new CUseableContainer(enCONTAINER_TYPE.UNKNOWN);
        public CUseableContainer m_ContainerSymbol = new CUseableContainer(enCONTAINER_TYPE.UNKNOWN);
        public int m_saleCount = 1;
        public CUseable m_selectUseable;
        public CUseableContainer m_selectUseableContainer;
        public enItemMenuType m_selectUseableType;
        public int m_useCount = 1;
        public static string s_bagAutoSaleFormPath = "UGUI/Form/System/Bag/Form_Bag_Sale_Item_OutTime.prefab";
        public static string s_bagFormPath = "UGUI/Form/System/Bag/Form_Bag.prefab";
        public static string s_bagSaleFormPath = "UGUI/Form/System/Bag/Form_Bag_Sale_Item.prefab";
        public static string s_bagUseFormPath = "UGUI/Form/System/Bag/Form_Bag_Use_Item.prefab";
        public static string s_itemGetSourceFormPath = "UGUI/Form/Common/Form_ItemGetSource.prefab";
        public static string s_openAwardpFormPath = "UGUI/Form/Common/Form_OpenAward.prefab";

        public static bool CanUseSkinExpCard(uint skinCfgId)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            if (masterRoleInfo == null)
            {
                DebugHelper.Assert(false, "IsHaveSkinExpCard role is null");
                return false;
            }
            if (!IsHaveSkinExpCard(skinCfgId))
            {
                return false;
            }
            uint heroId = 0;
            uint skinId = 0;
            CSkinInfo.ResolveHeroSkin(skinCfgId, out heroId, out skinId);
            return (masterRoleInfo.IsHaveHero(heroId, true) || masterRoleInfo.IsFreeHero(heroId));
        }

        private bool CheckNameChangeCard(CItem item)
        {
            if (item.m_itemData.bType == 1)
            {
                if (CItem.IsPlayerNameChangeCard(item.m_baseID))
                {
                    Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.NameChange_OpenPlayerNameChangeForm);
                    return true;
                }
                if (CItem.IsGuildNameChangeCard(item.m_baseID))
                {
                    if (CGuildSystem.HasGuildNameChangeAuthority())
                    {
                        Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.NameChange_OpenGuildNameChangeForm);
                    }
                    else
                    {
                        Singleton<CUIManager>.GetInstance().OpenTips("NameChange_GuildOnlyChairman", true, 1.5f, null, new object[0]);
                    }
                    return true;
                }
            }
            return false;
        }

        private void CheckOpenAutoSaleForm()
        {
            CUseableContainer autoSaleUseableContainer = null;
            uint salePrice = 0;
            autoSaleUseableContainer = this.GetAutoSaleUseableContainer(ref salePrice);
            if (autoSaleUseableContainer.GetCurUseableCount() > 0)
            {
                CUIFormScript formScript = Singleton<CUIManager>.instance.OpenForm(s_bagAutoSaleFormPath, false, true);
                if (formScript != null)
                {
                    CUIEventScript uIEventScript = CUICommonSystem.GetUIEventScript(formScript.get_transform().Find("btnGroup/Button_Use"));
                    if (uIEventScript != null)
                    {
                        uIEventScript.m_onClickEventParams.useableContainer = autoSaleUseableContainer;
                    }
                    CUIListScript uIListScript = CUICommonSystem.GetUIListScript(formScript.get_transform().Find("IconContainer"));
                    if (uIListScript != null)
                    {
                        int curUseableCount = autoSaleUseableContainer.GetCurUseableCount();
                        uIListScript.SetElementAmount(curUseableCount);
                        for (int i = 0; i < curUseableCount; i++)
                        {
                            CUICommonSystem.SetItemCell(formScript, uIListScript.GetElemenet(i).get_gameObject(), autoSaleUseableContainer.GetUseableByIndex(i), true, false, false, false);
                        }
                        CUICommonSystem.SetTextContent(formScript.get_transform().Find("bg/lblPrice"), salePrice.ToString());
                    }
                }
            }
        }

        public int FindItemIndex(uint inItemId)
        {
            int usebableIndexByUid = -1;
            CUseable useableByBaseID = this.m_ContainerAll.GetUseableByBaseID(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, inItemId);
            if (useableByBaseID != null)
            {
                usebableIndexByUid = this.m_ContainerAll.GetUsebableIndexByUid(useableByBaseID.m_objID);
            }
            return usebableIndexByUid;
        }

        private CUseableContainer GetAutoSaleUseableContainer(ref uint salePrice)
        {
            CUseableContainer container = new CUseableContainer(enCONTAINER_TYPE.ITEM);
            CUseableContainer useableContainer = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetUseableContainer(enCONTAINER_TYPE.ITEM);
            if (useableContainer != null)
            {
                int curUseableCount = useableContainer.GetCurUseableCount();
                CUseable useableByIndex = null;
                CItem item = null;
                ulong dtVal = 0L;
                DateTime time = Utility.ToUtcTime2Local((long) CRoleInfo.GetCurrentUTCTime());
                for (int i = 0; i < curUseableCount; i++)
                {
                    useableByIndex = useableContainer.GetUseableByIndex(i);
                    if (((useableByIndex.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP) && (useableByIndex.m_isSale > 0)) && (container.GetCurUseableCount() < 5))
                    {
                        item = useableByIndex as CItem;
                        dtVal = item.m_itemData.ullInvalidTime;
                        if (dtVal > 0L)
                        {
                            DateTime time2 = Utility.ULongToDateTime(dtVal);
                            if (DateTime.Compare(time, time2) > 0)
                            {
                                salePrice = (uint) (salePrice + (useableByIndex.m_coinSale * useableByIndex.m_stackCount));
                                container.Add(useableByIndex);
                            }
                        }
                    }
                }
            }
            return container;
        }

        private CUseableContainer GetContainerBySelectType(CUseableContainer allContainer)
        {
            CUseableContainer container = allContainer;
            int curUseableCount = allContainer.GetCurUseableCount();
            CUseable useableByIndex = null;
            if (this.m_selectUseableType == enItemMenuType.All)
            {
                this.m_ContainerAll.Clear();
                for (int j = 0; j < curUseableCount; j++)
                {
                    useableByIndex = allContainer.GetUseableByIndex(j);
                    if ((useableByIndex.m_type != COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL) || (useableByIndex.GetSalableCount() > 0))
                    {
                        this.m_ContainerAll.Add(useableByIndex);
                    }
                }
                return this.m_ContainerAll;
            }
            if (this.m_selectUseableType == enItemMenuType.RecentGet)
            {
                this.m_ContainerRecentGet.Clear();
                DateTime time = Utility.ToUtcTime2Local((long) CRoleInfo.GetCurrentUTCTime());
                for (int k = 0; k < curUseableCount; k++)
                {
                    useableByIndex = allContainer.GetUseableByIndex(k);
                    TimeSpan span = (TimeSpan) (time.Date - Utility.ToUtcTime2Local((long) useableByIndex.m_getTime).Date);
                    if ((span.Days <= 3) && ((useableByIndex.m_type != COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL) || (useableByIndex.GetSalableCount() > 0)))
                    {
                        this.m_ContainerRecentGet.Add(useableByIndex);
                    }
                }
                return this.m_ContainerRecentGet;
            }
            if (this.m_selectUseableType == enItemMenuType.Item)
            {
                this.m_ContainerItem.Clear();
                for (int m = 0; m < curUseableCount; m++)
                {
                    useableByIndex = allContainer.GetUseableByIndex(m);
                    if (useableByIndex.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
                    {
                        CItem useable = (CItem) useableByIndex;
                        if (((useable.m_itemData.bClass == 1) && (useable.m_itemData.bType != 4)) && (useable.m_itemData.bType != 10))
                        {
                            this.m_ContainerItem.Add(useable);
                        }
                    }
                }
                return this.m_ContainerItem;
            }
            if (this.m_selectUseableType == enItemMenuType.Gift)
            {
                this.m_ContainerGift.Clear();
                for (int n = 0; n < curUseableCount; n++)
                {
                    useableByIndex = allContainer.GetUseableByIndex(n);
                    if (useableByIndex.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
                    {
                        CItem item2 = (CItem) useableByIndex;
                        if ((item2.m_itemData.bClass == 1) && (item2.m_itemData.bType == 4))
                        {
                            this.m_ContainerGift.Add(item2);
                        }
                    }
                }
                return this.m_ContainerGift;
            }
            if (this.m_selectUseableType == enItemMenuType.ExpCard)
            {
                this.m_ContainerExpCard.Clear();
                for (int num6 = 0; num6 < curUseableCount; num6++)
                {
                    useableByIndex = allContainer.GetUseableByIndex(num6);
                    if (useableByIndex.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
                    {
                        CItem item3 = (CItem) useableByIndex;
                        if ((item3.m_itemData.bClass == 1) && (item3.m_itemData.bType == 10))
                        {
                            this.m_ContainerExpCard.Add(item3);
                        }
                    }
                }
                return this.m_ContainerExpCard;
            }
            if (this.m_selectUseableType != enItemMenuType.Symbol)
            {
                return container;
            }
            this.m_ContainerSymbol.Clear();
            for (int i = 0; i < curUseableCount; i++)
            {
                useableByIndex = allContainer.GetUseableByIndex(i);
                if ((useableByIndex.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL) && (useableByIndex.GetSalableCount() > 0))
                {
                    this.m_ContainerSymbol.Add(useableByIndex);
                }
            }
            return this.m_ContainerSymbol;
        }

        public static ListView<IHeroData> GetExpCardHeroList()
        {
            ListView<IHeroData> view = new ListView<IHeroData>();
            List<uint> list = new List<uint>();
            ListView<ResHeroCfgInfo> allHeroList = CHeroDataFactory.GetAllHeroList();
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                CUseableContainer useableContainer = masterRoleInfo.GetUseableContainer(enCONTAINER_TYPE.ITEM);
                int curUseableCount = useableContainer.GetCurUseableCount();
                uint heroID = 0;
                CUseable useableByIndex = null;
                CItem item = null;
                for (int i = 0; i < allHeroList.Count; i++)
                {
                    heroID = allHeroList[i].dwCfgID;
                    if (!CHeroDataFactory.IsHeroCanUse(heroID))
                    {
                        for (int k = 0; k < curUseableCount; k++)
                        {
                            useableByIndex = useableContainer.GetUseableByIndex(k);
                            if (useableByIndex.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
                            {
                                item = useableByIndex as CItem;
                                if (((((int) item.m_itemData.EftParam[0]) == 4) && (item.m_itemData.EftParam[1] == heroID)) && !list.Contains(heroID))
                                {
                                    list.Add(heroID);
                                }
                            }
                        }
                    }
                }
                for (int j = 0; j < list.Count; j++)
                {
                    view.Add(CHeroDataFactory.CreateHeroData(list[j]));
                }
            }
            return view;
        }

        public GameObject GetGuideItem()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_bagFormPath);
            if (form == null)
            {
                return null;
            }
            CUseableContainer selectUseableContainer = this.m_selectUseableContainer;
            for (int i = 0; i < selectUseableContainer.GetCurUseableCount(); i++)
            {
                CUseable useableByIndex = selectUseableContainer.GetUseableByIndex(i);
                if (useableByIndex.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
                {
                    CItem item = useableByIndex as CItem;
                    if ((item.m_itemData.bClass == 1) && (item.m_itemData.bType == 1))
                    {
                        CUIListScript component = form.get_gameObject().get_transform().Find("Panel_Right/List").get_gameObject().GetComponent<CUIListScript>();
                        if (component.GetElementAmount() > i)
                        {
                            return component.GetElemenet(i).get_gameObject().get_transform().Find("itemCell").get_gameObject();
                        }
                    }
                }
            }
            return null;
        }

        public override void Init()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Bag_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnBag_OpenForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Bag_OnCloseForm, new CUIEventManager.OnUIEventHandler(this.OnBag_OnCloseForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Bag_MenuSelect, new CUIEventManager.OnUIEventHandler(this.OnBag_MenuSelect));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Bag_ItemElement_Enable, new CUIEventManager.OnUIEventHandler(this.OnBag_ItemElement_Enable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Bag_SelectItem, new CUIEventManager.OnUIEventHandler(this.OnBag_SelectItem));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Bag_SaleItem, new CUIEventManager.OnUIEventHandler(this.OnBag_SaleItem));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Bag_UseItem, new CUIEventManager.OnUIEventHandler(this.OnBag_UseItem));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Bag_UseItemWithAnimation, new CUIEventManager.OnUIEventHandler(this.OnBag_UseItemWithAnimation));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Bag_UseItemWithAnimationConfirm, new CUIEventManager.OnUIEventHandler(this.OnBag_UseItemWithAnimationConfirm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Bag_OnUseItemAnimationPlayOver, new CUIEventManager.OnUIEventHandler(this.OnUseItemAnimationPlayOver));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Bag_OpenSaleForm, new CUIEventManager.OnUIEventHandler(this.OnBag_OpenSaleForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Bag_CloseSaleForm, new CUIEventManager.OnUIEventHandler(this.OnBag_CloseSaleForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Bag_SaleForm_CountDown, new CUIEventManager.OnUIEventHandler(this.OnBag_SaleForm_CountDown));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Bag_SaleForm_CountUp, new CUIEventManager.OnUIEventHandler(this.OnBag_SaleForm_CountUp));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Bag_SaleForm_CountMax, new CUIEventManager.OnUIEventHandler(this.OnBag_SaleForm_CountMax));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Bag_SaleForm_ConfirSale, new CUIEventManager.OnUIEventHandler(this.OnBag_SaleForm_ConfirmSale));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Bag_OnSecurePwdConfirmSale, new CUIEventManager.OnUIEventHandler(this.OnSecurePwdConfirmSale));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Bag_OpenUseForm, new CUIEventManager.OnUIEventHandler(this.OnBag_OpenUseForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Bag_UseForm_CountDown, new CUIEventManager.OnUIEventHandler(this.OnBag_UseForm_CountDown));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Bag_UseForm_CountUp, new CUIEventManager.OnUIEventHandler(this.OnBag_UseForm_CountUp));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Bag_UseForm_CountMax, new CUIEventManager.OnUIEventHandler(this.OnBag_UseForm_CountMax));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Tips_ItemSourceInfoOpen, new CUIEventManager.OnUIEventHandler(this.OnTips_ItemSourceInfoOpen));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Tips_ItemSourceElementClick, new CUIEventManager.OnUIEventHandler(this.OnTips_ItemSourceElementClick));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Bag_OnSecurePwdConfirmUse, new CUIEventManager.OnUIEventHandler(this.OnSecurePwdConfirmUseItem));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Bag_OnSecurePwdConfirmUseWithAnimation, new CUIEventManager.OnUIEventHandler(this.OnSecurePwdConfirmUseWithAnimation));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Bag_OnAutoSaleBtnClick, new CUIEventManager.OnUIEventHandler(this.OnClickAutoSaleBtn));
        }

        private void InitMenu(GameObject root)
        {
            string[] strArray = new string[] { "全部", "最近获得", "道具", "礼包", "体验卡", "铭文" };
            CUIListScript component = root.get_transform().Find("TopCommon/Panel_Menu/List").get_gameObject().GetComponent<CUIListScript>();
            component.SetElementAmount(strArray.Length);
            for (int i = 0; i < component.m_elementAmount; i++)
            {
                component.GetElemenet(i).get_gameObject().get_transform().Find("Text").GetComponent<Text>().set_text(strArray[i]);
            }
        }

        public static bool IsExpProp(COM_ITEM_TYPE itemType, uint itemId)
        {
            if (itemType != COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
            {
                return false;
            }
            ResPropInfo dataByKey = GameDataMgr.itemDatabin.GetDataByKey(itemId);
            return ((dataByKey != null) && (dataByKey.EftParam[0] == 1f));
        }

        public static bool IsHaveSkinExpCard(uint skinCfgId)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            if (masterRoleInfo == null)
            {
                DebugHelper.Assert(false, "IsHaveSkinExpCard role is null");
                return false;
            }
            CUseableContainer useableContainer = masterRoleInfo.GetUseableContainer(enCONTAINER_TYPE.ITEM);
            int curUseableCount = useableContainer.GetCurUseableCount();
            CUseable useableByIndex = null;
            CItem item = null;
            for (int i = 0; i < curUseableCount; i++)
            {
                useableByIndex = useableContainer.GetUseableByIndex(i);
                if (useableByIndex.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
                {
                    item = useableByIndex as CItem;
                    if (((item != null) && (((int) item.m_itemData.EftParam[0]) == 5)) && (item.m_itemData.EftParam[1] == skinCfgId))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void OnBag_CloseSaleForm(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.GetInstance().CloseForm(s_bagSaleFormPath);
        }

        private void OnBag_ItemElement_Enable(CUIEvent uiEvent)
        {
            int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
            GameObject itemCell = uiEvent.m_srcWidget.get_transform().Find("itemCell").get_gameObject();
            CUseable useableByIndex = this.m_selectUseableContainer.GetUseableByIndex(srcWidgetIndexInBelongedList);
            CUICommonSystem.SetItemCell(uiEvent.m_srcFormScript, itemCell, useableByIndex, false, false, false, false);
        }

        private void OnBag_MenuSelect(CUIEvent uiEvent)
        {
            GameObject obj2 = uiEvent.m_srcFormScript.get_gameObject();
            GameObject obj3 = obj2.get_transform().Find("Panel_Left").get_gameObject();
            int selectedIndex = uiEvent.m_srcWidget.GetComponent<CUIListScript>().GetSelectedIndex();
            int lastSelectedIndex = uiEvent.m_srcWidget.GetComponent<CUIListScript>().GetLastSelectedIndex();
            this.m_selectUseableType = (enItemMenuType) selectedIndex;
            CUseableContainer useableContainer = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetUseableContainer(enCONTAINER_TYPE.ITEM);
            if (useableContainer != null)
            {
                CUseableContainer containerBySelectType = this.GetContainerBySelectType(useableContainer);
                if (containerBySelectType != null)
                {
                    this.m_selectUseableContainer = containerBySelectType;
                    containerBySelectType.SortItemBag();
                    CUIListScript component = obj2.get_transform().Find("Panel_Right/List").get_gameObject().GetComponent<CUIListScript>();
                    component.SetElementAmount(this.m_selectUseableContainer.GetCurUseableCount());
                    if (component.m_elementAmount == 0)
                    {
                        obj3.CustomSetActive(false);
                    }
                    else
                    {
                        int index = component.GetSelectedIndex();
                        if (((index < 0) || (index >= component.m_elementAmount)) || (selectedIndex != lastSelectedIndex))
                        {
                            component.SelectElement(0, true);
                            this.RefreshSelectItem(uiEvent.m_srcFormScript, uiEvent.m_srcFormScript.get_gameObject(), 0);
                            component.MoveElementInScrollArea(0, true);
                        }
                        else
                        {
                            component.SelectElement(index, true);
                            this.RefreshSelectItem(uiEvent.m_srcFormScript, uiEvent.m_srcFormScript.get_gameObject(), index);
                            component.MoveElementInScrollArea(index, true);
                        }
                    }
                }
            }
        }

        private void OnBag_OnCloseForm(CUIEvent uiEvent)
        {
        }

        private void OnBag_OpenForm(CUIEvent uiEvent)
        {
            this.OpenBagForm();
            this.CheckOpenAutoSaleForm();
        }

        private void OnBag_OpenSaleForm(CUIEvent uiEvent)
        {
            this.m_saleCount = 1;
            CUIFormScript formScript = Singleton<CUIManager>.GetInstance().OpenForm(s_bagSaleFormPath, false, true);
            GameObject obj2 = formScript.get_gameObject();
            GameObject itemCell = obj2.get_transform().Find("Panel_Left/itemCell").get_gameObject();
            Text component = obj2.get_transform().Find("Panel_Left/lblName").GetComponent<Text>();
            Text text2 = obj2.get_transform().Find("Panel_Left/lblCount").GetComponent<Text>();
            Text text3 = obj2.get_transform().Find("Panel_Left/lblPrice1").GetComponent<Text>();
            Text text4 = obj2.get_transform().Find("Panel_Left/lblCount1").GetComponent<Text>();
            Text text5 = obj2.get_transform().Find("Panel_Left/lblPrice").GetComponent<Text>();
            CUICommonSystem.SetItemCell(formScript, itemCell, this.m_selectUseable, true, false, false, false);
            DebugHelper.Assert((((component != null) && (text2 != null)) && ((text3 != null) && (text4 != null))) && (text5 != null));
            component.set_text(this.m_selectUseable.m_name);
            text2.set_text(this.m_selectUseable.GetSalableCount().ToString());
            text3.set_text(this.m_selectUseable.m_coinSale.ToString());
            text4.set_text(this.m_saleCount + "/" + this.m_selectUseable.GetSalableCount());
            text5.set_text((this.m_saleCount * this.m_selectUseable.m_coinSale).ToString());
        }

        private void OnBag_OpenUseForm(CUIEvent uiEvent)
        {
            if (this.m_selectUseable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
            {
                CItem selectUseable = this.m_selectUseable as CItem;
                if ((selectUseable.m_itemData.bType == 4) && (selectUseable.m_itemData.EftParam[2] != 0f))
                {
                    this.PlayItemUseAnimation(this.m_selectUseable, 1, string.Empty);
                }
                else
                {
                    if (((selectUseable.m_itemData.bType == 1) || (selectUseable.m_itemData.bType == 7)) || (((selectUseable.m_itemData.bType == 4) || (selectUseable.m_itemData.bType == 10)) || (selectUseable.m_itemData.bType == 11)))
                    {
                        if (this.CheckNameChangeCard(selectUseable))
                        {
                            return;
                        }
                        this.m_useCount = 1;
                        if (selectUseable.m_itemData.bType == 11)
                        {
                            Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("Bag_Text_6"), enUIEventID.Bag_UseItem, enUIEventID.None, false);
                            return;
                        }
                        if (CItem.IsSkinExperienceCard(selectUseable.m_baseID))
                        {
                            uint skinCfgId = (uint) selectUseable.m_itemData.EftParam[1];
                            if (!CanUseSkinExpCard(skinCfgId))
                            {
                                Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Skin_Exp_Card_Can_Not_Use"), false, 1.5f, null, new object[0]);
                                return;
                            }
                        }
                        if (selectUseable.m_itemData.bIsBatchUse == 0)
                        {
                            SendItemUseMsgToServer(this.m_selectUseable.m_objID, 0, this.m_useCount, string.Empty);
                            return;
                        }
                        CUIFormScript formScript = Singleton<CUIManager>.GetInstance().OpenForm(s_bagUseFormPath, false, true);
                        GameObject obj2 = formScript.get_gameObject();
                        GameObject itemCell = obj2.get_transform().Find("Panel_Left/itemCell").get_gameObject();
                        Text component = obj2.get_transform().Find("Panel_Left/lblName").GetComponent<Text>();
                        Text text2 = obj2.get_transform().Find("Panel_Left/lblCount").GetComponent<Text>();
                        Text text3 = obj2.get_transform().Find("Panel_Left/lblCount1").GetComponent<Text>();
                        CUICommonSystem.SetItemCell(formScript, itemCell, this.m_selectUseable, true, false, false, false);
                        component.set_text(this.m_selectUseable.m_name);
                        text2.set_text(this.m_selectUseable.GetSalableCount().ToString());
                        if ((selectUseable.m_itemData.wBatchUseCnt > 0) && (selectUseable.m_itemData.wBatchUseCnt <= this.m_selectUseable.GetSalableCount()))
                        {
                            text3.set_text(this.m_useCount + "/" + selectUseable.m_itemData.wBatchUseCnt);
                        }
                        else
                        {
                            text3.set_text(this.m_useCount + "/" + this.m_selectUseable.GetSalableCount());
                        }
                    }
                    if (CItem.IsHeroExChangeCoupons(selectUseable.m_baseID))
                    {
                        Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mall_GoToRecommendHeroTab);
                    }
                    else if (CItem.IsSkinExChangeCoupons(selectUseable.m_baseID))
                    {
                        Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mall_GoToRecommendSkinTab);
                    }
                    else if (CItem.IsCryStalItem(selectUseable.m_baseID))
                    {
                        Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mall_GoToTreasureTab);
                    }
                    else if (selectUseable.m_itemData.bType == 8)
                    {
                        Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Union_Battle_ClickEntry);
                    }
                    else if (selectUseable.m_itemData.bType == 9)
                    {
                        stUIEventParams par = new stUIEventParams();
                        par.commonUInt32Param1 = selectUseable.m_itemData.dwID;
                        Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Speaker_Form_Open, par);
                    }
                }
            }
            else if (this.m_selectUseable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL)
            {
                if (Singleton<CFunctionUnlockSys>.instance.FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_SYMBOL))
                {
                    Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Symbol_OpenForm);
                }
                else
                {
                    Singleton<CUIManager>.GetInstance().OpenTips("Symbol_Lock_Tip", true, 1.5f, null, new object[0]);
                }
            }
        }

        private void OnBag_SaleForm_ConfirmSale(CUIEvent uiEvent)
        {
            if (this.m_selectUseable != null)
            {
                if (this.m_selectUseable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL)
                {
                    CSymbolItem selectUseable = this.m_selectUseable as CSymbolItem;
                    if ((selectUseable != null) && selectUseable.IsSaleNeedSecurePwd())
                    {
                        CSecurePwdSystem.TryToValidate(enOpPurpose.SALE_SYMBOL, enUIEventID.Bag_OnSecurePwdConfirmSale, new stUIEventParams());
                        return;
                    }
                }
                ListView<CSDT_ITEM_DELINFO> itemList = new ListView<CSDT_ITEM_DELINFO>();
                CSDT_ITEM_DELINFO csdt_item_delinfo = new CSDT_ITEM_DELINFO();
                csdt_item_delinfo.ullUniqueID = this.m_selectUseable.m_objID;
                csdt_item_delinfo.iItemCnt = (ushort) this.m_saleCount;
                itemList.Add(csdt_item_delinfo);
                SendItemSaleMsg(itemList, string.Empty);
            }
        }

        private void OnBag_SaleForm_CountDown(CUIEvent uiEvent)
        {
            GameObject obj2 = uiEvent.m_srcFormScript.get_gameObject();
            Text component = obj2.get_transform().Find("Panel_Left/lblCount1").GetComponent<Text>();
            Text text2 = obj2.get_transform().Find("Panel_Left/lblPrice").GetComponent<Text>();
            if (this.m_saleCount > 1)
            {
                this.m_saleCount--;
            }
            component.set_text(this.m_saleCount + "/" + this.m_selectUseable.GetSalableCount());
            text2.set_text((this.m_saleCount * this.m_selectUseable.m_coinSale).ToString());
        }

        private void OnBag_SaleForm_CountMax(CUIEvent uiEvent)
        {
            GameObject obj2 = uiEvent.m_srcFormScript.get_gameObject();
            Text component = obj2.get_transform().Find("Panel_Left/lblCount1").GetComponent<Text>();
            Text text2 = obj2.get_transform().Find("Panel_Left/lblPrice").GetComponent<Text>();
            int salableCount = this.m_selectUseable.GetSalableCount();
            this.m_saleCount = salableCount;
            component.set_text(this.m_saleCount + "/" + salableCount);
            text2.set_text((this.m_saleCount * this.m_selectUseable.m_coinSale).ToString());
        }

        private void OnBag_SaleForm_CountUp(CUIEvent uiEvent)
        {
            GameObject obj2 = uiEvent.m_srcFormScript.get_gameObject();
            Text component = obj2.get_transform().Find("Panel_Left/lblCount1").GetComponent<Text>();
            Text text2 = obj2.get_transform().Find("Panel_Left/lblPrice").GetComponent<Text>();
            int salableCount = this.m_selectUseable.GetSalableCount();
            if (this.m_saleCount < salableCount)
            {
                this.m_saleCount++;
            }
            component.set_text(this.m_saleCount + "/" + salableCount);
            text2.set_text((this.m_saleCount * this.m_selectUseable.m_coinSale).ToString());
        }

        private void OnBag_SaleItem(CUIEvent uiEvent)
        {
        }

        private void OnBag_SelectItem(CUIEvent uiEvent)
        {
            this.RefreshSelectItem(uiEvent.m_srcFormScript, uiEvent.m_srcFormScript.get_gameObject(), uiEvent.m_srcWidgetIndexInBelongedList);
        }

        private void OnBag_UseForm_CountDown(CUIEvent uiEvent)
        {
            Text component = uiEvent.m_srcFormScript.get_gameObject().get_transform().Find("Panel_Left/lblCount1").GetComponent<Text>();
            if (this.m_useCount > 1)
            {
                this.m_useCount--;
            }
            component.set_text(this.m_useCount + "/" + this.m_selectUseable.GetSalableCount());
        }

        private void OnBag_UseForm_CountMax(CUIEvent uiEvent)
        {
            Text component = uiEvent.m_srcFormScript.get_gameObject().get_transform().Find("Panel_Left/lblCount1").GetComponent<Text>();
            CItem selectUseable = this.m_selectUseable as CItem;
            int wBatchUseCnt = 1;
            if (((selectUseable != null) && (selectUseable.m_itemData != null)) && ((selectUseable.m_itemData.wBatchUseCnt > 0) && (selectUseable.m_itemData.wBatchUseCnt <= this.m_selectUseable.GetSalableCount())))
            {
                wBatchUseCnt = selectUseable.m_itemData.wBatchUseCnt;
            }
            else
            {
                wBatchUseCnt = this.m_selectUseable.GetSalableCount();
            }
            this.m_useCount = wBatchUseCnt;
            component.set_text(this.m_useCount + "/" + wBatchUseCnt);
        }

        private void OnBag_UseForm_CountUp(CUIEvent uiEvent)
        {
            Text component = uiEvent.m_srcFormScript.get_gameObject().get_transform().Find("Panel_Left/lblCount1").GetComponent<Text>();
            CItem selectUseable = this.m_selectUseable as CItem;
            int wBatchUseCnt = 1;
            if (((selectUseable != null) && (selectUseable.m_itemData != null)) && ((selectUseable.m_itemData.wBatchUseCnt > 0) && (selectUseable.m_itemData.wBatchUseCnt <= this.m_selectUseable.GetSalableCount())))
            {
                wBatchUseCnt = selectUseable.m_itemData.wBatchUseCnt;
            }
            else
            {
                wBatchUseCnt = this.m_selectUseable.GetSalableCount();
            }
            if (this.m_useCount < wBatchUseCnt)
            {
                this.m_useCount++;
            }
            component.set_text(this.m_useCount + "/" + wBatchUseCnt);
        }

        private void OnBag_UseItem(CUIEvent uiEvent)
        {
            if (this.m_selectUseable != null)
            {
                CItem selectUseable = this.m_selectUseable as CItem;
                if (selectUseable != null)
                {
                    if (selectUseable.m_itemData.bType == 11)
                    {
                        CSecurePwdSystem.TryToValidate(enOpPurpose.USE_BATTLERECORD_CARD, enUIEventID.Bag_OnSecurePwdConfirmUse, new stUIEventParams());
                    }
                    else
                    {
                        SendItemUseMsgToServer(this.m_selectUseable.m_objID, 0, this.m_useCount, string.Empty);
                    }
                }
            }
        }

        private void OnBag_UseItemWithAnimation(CUIEvent uiEvent)
        {
            CUseable iconUseable = uiEvent.m_eventParams.iconUseable;
            if (iconUseable != null)
            {
                if (iconUseable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
                {
                    CItem item = iconUseable as CItem;
                    if (((item != null) && (item.m_itemData != null)) && (item.m_itemData.bType == 11))
                    {
                        Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("Bag_Text_6"), enUIEventID.Bag_UseItemWithAnimationConfirm, enUIEventID.None, uiEvent.m_eventParams, false);
                        return;
                    }
                }
                this.PlayItemUseAnimation(uiEvent.m_eventParams.iconUseable, uiEvent.m_eventParams.tag, string.Empty);
            }
        }

        private void OnBag_UseItemWithAnimationConfirm(CUIEvent uiEvent)
        {
            CUseable iconUseable = uiEvent.m_eventParams.iconUseable;
            if (iconUseable != null)
            {
                if (iconUseable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
                {
                    CItem item = iconUseable as CItem;
                    if (((item != null) && (item.m_itemData != null)) && (item.m_itemData.bType == 11))
                    {
                        CSecurePwdSystem.TryToValidate(enOpPurpose.USE_BATTLERECORD_CARD, enUIEventID.Bag_OnSecurePwdConfirmUseWithAnimation, uiEvent.m_eventParams);
                        return;
                    }
                }
                this.PlayItemUseAnimation(uiEvent.m_eventParams.iconUseable, uiEvent.m_eventParams.tag, string.Empty);
            }
        }

        private void OnClickAutoSaleBtn(CUIEvent uiEvent)
        {
            CUseableContainer useableContainer = uiEvent.m_eventParams.useableContainer;
            if (useableContainer != null)
            {
                int curUseableCount = useableContainer.GetCurUseableCount();
                CUseable useableByIndex = null;
                ListView<CSDT_ITEM_DELINFO> itemList = new ListView<CSDT_ITEM_DELINFO>();
                for (int i = 0; i < curUseableCount; i++)
                {
                    useableByIndex = useableContainer.GetUseableByIndex(i);
                    CSDT_ITEM_DELINFO item = new CSDT_ITEM_DELINFO();
                    item.ullUniqueID = useableByIndex.m_objID;
                    item.iItemCnt = useableByIndex.m_stackCount;
                    itemList.Add(item);
                }
                SendItemSaleMsg(itemList, string.Empty);
            }
        }

        private void OnSecurePwdConfirmSale(CUIEvent uiEvent)
        {
            ListView<CSDT_ITEM_DELINFO> itemList = new ListView<CSDT_ITEM_DELINFO>();
            CSDT_ITEM_DELINFO item = new CSDT_ITEM_DELINFO();
            item.ullUniqueID = this.m_selectUseable.m_objID;
            item.iItemCnt = (ushort) this.m_saleCount;
            itemList.Add(item);
            SendItemSaleMsg(itemList, uiEvent.m_eventParams.pwd);
        }

        private void OnSecurePwdConfirmUseItem(CUIEvent uiEvent)
        {
            if (this.m_selectUseable != null)
            {
                SendItemUseMsgToServer(this.m_selectUseable.m_objID, 0, this.m_useCount, uiEvent.m_eventParams.pwd);
            }
        }

        private void OnSecurePwdConfirmUseWithAnimation(CUIEvent uiEvent)
        {
            this.PlayItemUseAnimation(uiEvent.m_eventParams.iconUseable, uiEvent.m_eventParams.tag, uiEvent.m_eventParams.pwd);
        }

        private void OnTips_ItemSourceElementClick(CUIEvent uiEvent)
        {
            stItemGetInfoParams itemGetInfoParams = uiEvent.m_eventParams.itemGetInfoParams;
            if (!itemGetInfoParams.isCanDo)
            {
                Singleton<CUIManager>.GetInstance().OpenTips(itemGetInfoParams.errorStr, false, 1.5f, null, new object[0]);
            }
            else
            {
                if (itemGetInfoParams.getType == 1)
                {
                    CUIEvent event2 = new CUIEvent();
                    event2.m_eventID = enUIEventID.Adv_OpenLevelForm;
                    event2.m_eventParams.tag = itemGetInfoParams.levelInfo.iCfgID;
                    Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(event2);
                }
                else if (itemGetInfoParams.getType != 2)
                {
                    if (itemGetInfoParams.getType == 3)
                    {
                        if (Singleton<MySteryShop>.GetInstance().IsShopAvailable())
                        {
                            Singleton<CMallSystem>.GetInstance().CurTab = CMallSystem.Tab.Mystery;
                        }
                        else
                        {
                            Singleton<CMallSystem>.GetInstance().CurTab = CMallSystem.Tab.Boutique;
                        }
                        CUIEvent event4 = new CUIEvent();
                        event4.m_eventID = enUIEventID.Mall_OpenForm;
                        event4.m_eventParams.tag = 1;
                        Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(event4);
                    }
                }
                else
                {
                    RES_SHOP_TYPE tag = (RES_SHOP_TYPE) uiEvent.m_eventParams.tag;
                    CUIEvent event3 = new CUIEvent();
                    switch (tag)
                    {
                        case RES_SHOP_TYPE.RES_SHOPTYPE_ARENA:
                            event3.m_eventID = enUIEventID.Shop_OpenArenaShop;
                            break;

                        case RES_SHOP_TYPE.RES_SHOPTYPE_BURNING:
                            event3.m_eventID = enUIEventID.Shop_OpenBurningShop;
                            break;
                    }
                    Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(event3);
                }
                Singleton<CUIManager>.GetInstance().CloseForm(s_itemGetSourceFormPath);
            }
        }

        private void OnTips_ItemSourceInfoOpen(CUIEvent uiEvent)
        {
            CUIFormScript formScript = Singleton<CUIManager>.GetInstance().OpenForm(s_itemGetSourceFormPath, false, true);
            CUseable iconUseable = uiEvent.m_eventParams.iconUseable;
            GameObject itemCell = formScript.get_gameObject().get_transform().Find("Panel/itemCell").get_gameObject();
            Text component = formScript.get_gameObject().get_transform().Find("Panel/lblName").GetComponent<Text>();
            Text text2 = formScript.get_gameObject().get_transform().Find("Panel/lblDesc").GetComponent<Text>();
            CUIListScript list = formScript.get_gameObject().get_transform().Find("Panel/List").GetComponent<CUIListScript>();
            CUICommonSystem.SetItemCell(formScript, itemCell, iconUseable, false, false, false, false);
            component.set_text(iconUseable.m_name);
            string[] values = new string[] { iconUseable.GetSalableCount().ToString() };
            text2.set_text(CUIUtility.StringReplace(iconUseable.m_description, values));
            CUICommonSystem.SetGetInfoToList(formScript, list, iconUseable);
        }

        private void OnUseItemAnimationPlayOver(CUIEvent uiEvent)
        {
            if (uiEvent.m_eventParams.iconUseable != null)
            {
                SendItemUseMsgToServer(uiEvent.m_eventParams.iconUseable.m_objID, 0, uiEvent.m_eventParams.tag, uiEvent.m_eventParams.pwd);
            }
        }

        public void OpenBagForm()
        {
            CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(s_bagFormPath, false, true);
            Singleton<CBagSystem>.GetInstance().m_selectUseableType = enItemMenuType.All;
            Singleton<CBagSystem>.GetInstance().InitMenu(script.get_gameObject());
            Singleton<CBagSystem>.GetInstance().RefreshBagForm();
        }

        private void PlayItemUseAnimation(CUseable item, int itemCount, string pwd = "")
        {
            if (item != null)
            {
                CItem item2 = (CItem) item;
                if (item2 != null)
                {
                    if ((item2.m_itemData.bType == 4) && (item2.m_itemData.EftParam[2] != 0f))
                    {
                        CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(s_openAwardpFormPath, false, true);
                        if (script != null)
                        {
                            CUITimerScript component = script.get_transform().Find("Timer").GetComponent<CUITimerScript>();
                            component.m_eventParams[1].iconUseable = item;
                            component.m_eventParams[1].tag = itemCount;
                            component.m_eventParams[1].pwd = pwd;
                            component.EndTimer();
                            component.StartTimer();
                        }
                    }
                    else if (!this.CheckNameChangeCard(item2))
                    {
                        SendItemUseMsgToServer(item.m_objID, 0, itemCount, pwd);
                    }
                }
            }
        }

        [MessageHandler(0x494)]
        public static void ReceiveItemUse(CSPkg msg)
        {
            uint dwPropID = msg.stPkgData.stPropUseRsp.dwPropID;
            CItem item = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, dwPropID, 0) as CItem;
            if ((item != null) && (item.m_itemData.bType == 11))
            {
                Singleton<CUIManager>.GetInstance().OpenTips(string.Format(Singleton<CTextManager>.GetInstance().GetText("Bag_Text_5"), item.m_name), false, 1.5f, null, new object[0]);
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if (masterRoleInfo != null)
                {
                    masterRoleInfo.CleanUpBattleRecord();
                }
            }
        }

        [MessageHandler(0x44e)]
        public static void ReciveItemAdd(CSPkg msg)
        {
            COMDT_ITEM_ADDLIST stAddList = msg.stPkgData.stItemAdd.stAddList;
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            CUseableContainer useableContainer = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetUseableContainer(enCONTAINER_TYPE.ITEM);
            if (useableContainer != null)
            {
                COMDT_ITEM_ADDINFO comdt_item_addinfo = null;
                int iCount = 0;
                for (int i = 0; i < stAddList.wItemCnt; i++)
                {
                    comdt_item_addinfo = stAddList.astItemList[i];
                    if (comdt_item_addinfo.wItemType == 2)
                    {
                        iCount = comdt_item_addinfo.stItemInfo.stPropInfo.iOverCnt;
                    }
                    else if (comdt_item_addinfo.wItemType == 3)
                    {
                        iCount = comdt_item_addinfo.stItemInfo.stEquipInfo.iOverCnt;
                    }
                    else if (comdt_item_addinfo.wItemType == 5)
                    {
                        iCount = comdt_item_addinfo.stItemInfo.stSymbolInfo.iOverCnt;
                    }
                    CUseable useable = useableContainer.Add((COM_ITEM_TYPE) comdt_item_addinfo.wItemType, comdt_item_addinfo.ullUniqueID, comdt_item_addinfo.dwItemID, iCount, useableContainer.GetMaxAddTime());
                    if ((useable != null) && (((useable != null) && (useable.m_stackCount >= useable.m_stackMax)) && ((useable.m_type != COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL) || !((CSymbolItem) useable).IsGuildSymbol())))
                    {
                        string[] args = new string[] { useable.m_name, useable.m_stackMax.ToString() };
                        Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Bag_Text_1", args), false, 1.5f, null, new object[0]);
                    }
                }
                switch (msg.stPkgData.stItemAdd.bAddReason)
                {
                }
                Singleton<CBagSystem>.GetInstance().RefreshBagForm();
                Singleton<CHeroInfoSystem2>.GetInstance().RefreshHeroInfoForm();
                Singleton<CSymbolSystem>.GetInstance().RefreshSymbolForm();
                Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
                Singleton<EventRouter>.instance.BroadCastEvent(EventID.BAG_ITEMS_UPDATE);
            }
        }

        [MessageHandler(0x44f)]
        public static void ReciveItemDel(CSPkg msg)
        {
            CSDT_ITEM_DELLIST stDelList = msg.stPkgData.stItemDel.stDelList;
            CUseableContainer useableContainer = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetUseableContainer(enCONTAINER_TYPE.ITEM);
            CSDT_ITEM_DELINFO csdt_item_delinfo = null;
            uint key = 0;
            for (int i = 0; i < stDelList.wItemCnt; i++)
            {
                csdt_item_delinfo = stDelList.astItemList[i];
                if (csdt_item_delinfo != null)
                {
                    CUseable useableByObjID = useableContainer.GetUseableByObjID(csdt_item_delinfo.ullUniqueID);
                    if (useableByObjID != null)
                    {
                        key = useableByObjID.m_baseID;
                    }
                    useableContainer.Remove(csdt_item_delinfo.ullUniqueID, csdt_item_delinfo.iItemCnt);
                }
            }
            switch (msg.stPkgData.stItemDel.bDelReason)
            {
                case 3:
                    Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Shop_Sale_Success"), false, 1.5f, null, new object[0]);
                    Singleton<CUIManager>.GetInstance().CloseForm(s_bagSaleFormPath);
                    Singleton<EventRouter>.instance.BroadCastEvent<CSDT_ITEM_DELLIST>(EventID.BAG_ITEM_SALED, stDelList);
                    Singleton<CSoundManager>.GetInstance().PostEvent("UI_backpack_sell", null);
                    break;

                case 4:
                    Singleton<CSoundManager>.GetInstance().PostEvent("UI_backpack_sell", null);
                    if (csdt_item_delinfo != null)
                    {
                        ResPropInfo dataByKey = GameDataMgr.itemDatabin.GetDataByKey(key);
                        if ((dataByKey != null) && (dataByKey.iUseShowTip != 0))
                        {
                            Singleton<CUIManager>.GetInstance().OpenTips(string.Format(Singleton<CTextManager>.GetInstance().GetText("ItemTakeEffectTip"), StringHelper.UTF8BytesToString(ref dataByKey.szName)), false, 1.5f, null, new object[0]);
                        }
                    }
                    break;

                case 7:
                    Singleton<CHeroInfoSystem2>.GetInstance().RefreshHeroInfoForm();
                    break;
            }
            Singleton<CBagSystem>.GetInstance().RefreshBagForm();
            Singleton<CSymbolSystem>.GetInstance().RefreshSymbolForm();
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            Singleton<EventRouter>.instance.BroadCastEvent(EventID.BAG_ITEMS_UPDATE);
        }

        [MessageHandler(0x455)]
        public static void ReciveItemList(CSPkg msg)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                CUseableContainer useableContainer = masterRoleInfo.GetUseableContainer(enCONTAINER_TYPE.ITEM);
                COMDT_ITEM_POSINFO comdt_item_posinfo = null;
                int iCount = 0;
                for (int i = 0; i < msg.stPkgData.stPkgDetail.stPkgInfo.wItemCnt; i++)
                {
                    comdt_item_posinfo = msg.stPkgData.stPkgDetail.stPkgInfo.astItemList[i];
                    if (comdt_item_posinfo.wItemType == 2)
                    {
                        iCount = comdt_item_posinfo.stItemInfo.stPropInfo.iOverCnt;
                    }
                    else if (comdt_item_posinfo.wItemType == 3)
                    {
                        iCount = comdt_item_posinfo.stItemInfo.stEquipInfo.iOverCnt;
                    }
                    else if (comdt_item_posinfo.wItemType == 5)
                    {
                        iCount = comdt_item_posinfo.stItemInfo.stSymbolInfo.iOverCnt;
                    }
                    useableContainer.Add((COM_ITEM_TYPE) comdt_item_posinfo.wItemType, comdt_item_posinfo.ullUniqueID, comdt_item_posinfo.dwItemID, iCount, comdt_item_posinfo.iAddUpdTime);
                }
                Singleton<EventRouter>.instance.BroadCastEvent(EventID.BAG_ITEMS_UPDATE);
            }
        }

        public void RefreshBagForm()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_bagFormPath);
            if (form != null)
            {
                CUIListScript component = form.get_gameObject().get_transform().Find("TopCommon/Panel_Menu/List").get_gameObject().GetComponent<CUIListScript>();
                component.m_alwaysDispatchSelectedChangeEvent = true;
                component.SelectElement((int) this.m_selectUseableType, true);
                component.m_alwaysDispatchSelectedChangeEvent = false;
            }
        }

        private void RefreshSelectItem(CUIFormScript formScript, GameObject root, int selectIndex)
        {
            this.m_selectUseable = this.m_selectUseableContainer.GetUseableByIndex(selectIndex);
            if (this.m_selectUseable != null)
            {
                GameObject obj2 = root.get_transform().Find("Panel_Left").get_gameObject();
                GameObject itemCell = root.get_transform().Find("Panel_Left/itemCell").get_gameObject();
                Text component = root.get_transform().Find("Panel_Left/lblName").GetComponent<Text>();
                CanvasGroup group = root.get_transform().Find("Panel_Left/pnlCountTitle").GetComponent<CanvasGroup>();
                Text text2 = group.get_transform().Find("lblCount").GetComponent<Text>();
                Text text3 = root.get_transform().Find("Panel_Left/lblDesc").GetComponent<Text>();
                Text text4 = root.get_transform().Find("Panel_Left/lblPrice").GetComponent<Text>();
                Text text5 = root.get_transform().Find("Panel_Left/lblPriceTitle").GetComponent<Text>();
                Image image = root.get_transform().Find("Panel_Left/imgGold").GetComponent<Image>();
                Button button = root.get_transform().Find("Panel_Left/BtnGroup/Button_Sale").GetComponent<Button>();
                Button button2 = root.get_transform().Find("Panel_Left/BtnGroup/Button_Use").GetComponent<Button>();
                Button button3 = root.get_transform().Find("Panel_Left/BtnGroup/Button_Source").GetComponent<Button>();
                CUICommonSystem.SetItemCell(formScript, itemCell, this.m_selectUseable, false, false, false, false);
                component.set_text(this.m_selectUseable.m_name);
                text2.set_text(this.m_selectUseable.GetSalableCount().ToString());
                text3.set_text(this.m_selectUseable.m_description);
                text4.set_text(this.m_selectUseable.m_coinSale.ToString());
                text4.get_gameObject().CustomSetActive(true);
                text5.get_gameObject().CustomSetActive(true);
                image.get_gameObject().CustomSetActive(true);
                button.get_gameObject().CustomSetActive(true);
                button2.get_gameObject().CustomSetActive(true);
                button3.get_gameObject().CustomSetActive(false);
                if (this.m_selectUseable.m_isSale <= 0)
                {
                    text4.get_gameObject().CustomSetActive(false);
                    text5.get_gameObject().CustomSetActive(false);
                    image.get_gameObject().CustomSetActive(false);
                    button.get_gameObject().CustomSetActive(false);
                }
                if (this.m_selectUseable.m_bCanUse <= 0)
                {
                    button2.get_gameObject().CustomSetActive(false);
                }
                if ((this.m_selectUseable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL) && ((CSymbolItem) this.m_selectUseable).IsGuildSymbol())
                {
                    group.set_alpha(0f);
                }
                else
                {
                    group.set_alpha(1f);
                }
                obj2.CustomSetActive(true);
            }
        }

        public static void SendItemSaleMsg(ListView<CSDT_ITEM_DELINFO> itemList, string pwd = "")
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x44d);
            CSDT_ITEM_DELLIST csdt_item_dellist = new CSDT_ITEM_DELLIST();
            csdt_item_dellist.wItemCnt = (ushort) itemList.Count;
            csdt_item_dellist.astItemList = LinqS.ToArray<CSDT_ITEM_DELINFO>(itemList);
            msg.stPkgData.stItemSale.stSaleList = csdt_item_dellist;
            StringHelper.StringToUTF8Bytes(pwd, ref msg.stPkgData.stItemSale.szPswdInfo);
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public static void SendItemUseMsgToServer(ulong itemID, uint heroID = 0, int useCount = 0, string pwd = "")
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x453);
            msg.stPkgData.stPropUse.ullUniqueID = itemID;
            msg.stPkgData.stPropUse.dwHeroID = heroID;
            msg.stPkgData.stPropUse.wItemCnt = (ushort) useCount;
            StringHelper.StringToUTF8Bytes(pwd, ref msg.stPkgData.stPropUse.szPswdInfo);
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public static void SetGetInfoToList(CUIFormScript formScript, CUIListScript list, CUseable itemUseable)
        {
            ResDT_ItemSrc_Info[] astSrcInfo = null;
            if (itemUseable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
            {
                astSrcInfo = ((CItem) itemUseable).m_itemData.astSrcInfo;
            }
            else if (itemUseable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMEQUIP)
            {
                astSrcInfo = ((CEquip) itemUseable).m_equipData.astSrcInfo;
            }
            else if (itemUseable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL)
            {
                astSrcInfo = ((CSymbolItem) itemUseable).m_SymbolData.astSrcInfo;
            }
            else
            {
                return;
            }
            int amount = 0;
            for (int i = 0; i < astSrcInfo.Length; i++)
            {
                if (astSrcInfo[i].bType == 0)
                {
                    break;
                }
                amount++;
            }
            if (list != null)
            {
                list.SetElementAmount(amount);
                for (int j = 0; j < amount; j++)
                {
                    Transform transform = list.GetElemenet(j).get_transform().Find("sourceCell");
                    Text component = transform.Find("lblName").GetComponent<Text>();
                    Text text2 = transform.Find("lblLevel").GetComponent<Text>();
                    Button button = transform.Find("lblOpen").GetComponent<Button>();
                    Text text3 = transform.Find("lblClose").GetComponent<Text>();
                    Text text4 = transform.Find("Elite").GetComponent<Text>();
                    byte bType = astSrcInfo[j].bType;
                    int dwID = (int) astSrcInfo[j].dwID;
                    bool bActive = false;
                    stUIEventParams eventParams = new stUIEventParams();
                    stItemGetInfoParams params2 = new stItemGetInfoParams();
                    eventParams.itemGetInfoParams = params2;
                    eventParams.itemGetInfoParams.getType = bType;
                    if (bType == 1)
                    {
                        ResLevelCfgInfo dataByKey = GameDataMgr.levelDatabin.GetDataByKey((long) dwID);
                        bActive = Singleton<CAdventureSys>.GetInstance().IsLevelOpen(dataByKey.iCfgID);
                        eventParams.itemGetInfoParams.levelInfo = dataByKey;
                        eventParams.itemGetInfoParams.isCanDo = bActive;
                        if (!bActive)
                        {
                            eventParams.itemGetInfoParams.errorStr = Singleton<CTextManager>.instance.GetText("Hero_Level_Not_Open");
                        }
                        component.set_text(StringHelper.UTF8BytesToString(ref dataByKey.szName));
                        text2.set_text(dataByKey.iChapterId + "-" + dataByKey.bLevelNo);
                        if (text4 != null)
                        {
                            text4.get_gameObject().CustomSetActive(dataByKey.bLevelDifficulty == 2);
                        }
                    }
                    button.get_gameObject().CustomSetActive(bActive);
                    text3.get_gameObject().CustomSetActive(!bActive);
                    CUIEventScript script = button.get_gameObject().GetComponent<CUIEventScript>();
                    if (script == null)
                    {
                        script = transform.get_gameObject().AddComponent<CUIEventScript>();
                        script.Initialize(formScript);
                    }
                    script.SetUIEvent(enUIEventType.Click, enUIEventID.Tips_ItemSourceElementClick, eventParams);
                }
                list.get_gameObject().CustomSetActive(true);
            }
        }

        public void UpdateBagRed()
        {
        }

        public static void UseHeroExpCard(uint heroID)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                CUseableContainer useableContainer = masterRoleInfo.GetUseableContainer(enCONTAINER_TYPE.ITEM);
                int curUseableCount = useableContainer.GetCurUseableCount();
                CItem item = null;
                CUseable useableByIndex = null;
                CItem item2 = null;
                for (int i = 0; i < curUseableCount; i++)
                {
                    useableByIndex = useableContainer.GetUseableByIndex(i);
                    if (useableByIndex.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
                    {
                        item2 = useableByIndex as CItem;
                        if ((((int) item2.m_itemData.EftParam[0]) == 4) && (item2.m_itemData.EftParam[1] == heroID))
                        {
                            if (item == null)
                            {
                                item = item2;
                            }
                            else if (item2.m_itemData.EftParam[2] < item.m_itemData.EftParam[2])
                            {
                                item = item2;
                            }
                        }
                    }
                }
                if (item != null)
                {
                    SendItemUseMsgToServer(item.m_objID, 0, 1, string.Empty);
                }
            }
        }

        public static void UseSkinExpCard(uint skinCfgId)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                if (!CanUseSkinExpCard(skinCfgId))
                {
                    Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Skin_Exp_Card_Can_Not_Use"), false, 1.5f, null, new object[0]);
                }
                else
                {
                    CUseableContainer useableContainer = masterRoleInfo.GetUseableContainer(enCONTAINER_TYPE.ITEM);
                    int curUseableCount = useableContainer.GetCurUseableCount();
                    CItem item = null;
                    CUseable useableByIndex = null;
                    CItem item2 = null;
                    for (int i = 0; i < curUseableCount; i++)
                    {
                        useableByIndex = useableContainer.GetUseableByIndex(i);
                        if (useableByIndex.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
                        {
                            item2 = useableByIndex as CItem;
                            if (((item2 != null) && (((int) item2.m_itemData.EftParam[0]) == 5)) && (item2.m_itemData.EftParam[1] == skinCfgId))
                            {
                                if (item == null)
                                {
                                    item = item2;
                                }
                                else if (item2.m_itemData.EftParam[2] < item.m_itemData.EftParam[2])
                                {
                                    item = item2;
                                }
                            }
                        }
                    }
                    if (item != null)
                    {
                        SendItemUseMsgToServer(item.m_objID, 0, 1, string.Empty);
                    }
                }
            }
        }
    }
}

