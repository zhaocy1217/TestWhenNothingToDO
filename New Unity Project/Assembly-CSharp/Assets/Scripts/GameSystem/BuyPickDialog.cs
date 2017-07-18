namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class BuyPickDialog
    {
        private bool _bDynamicCorrectPrice;
        private bool _bHeroSkinGift;
        private CMallFactoryShopController.ShopProduct _callContext;
        private RES_SHOPBUY_COINTYPE _coinType;
        private CUseable _coinUsb;
        private Text _costText;
        private uint _count;
        private Text _countText;
        private Text _descText;
        private uint _heroSkinGiftCost;
        private uint _maxCount;
        private OnConfirmBuyDelegate _onConfirm;
        private OnConfirmBuyCommonDelegate _onConfirmdCommon;
        private float _realDiscount;
        private GameObject _root;
        private CUIEvent _uieventPars;
        private CUseable _usb;
        private bool m_bShowBigIcon;
        public static string s_Form_Path = "UGUI/Form/Common/Form_BuyPick.prefab";
        public static string s_Gift_Big_Icon_Form_Path = "UGUI/Form/Common/Form_BuyPick_Gift_Big_Icon.prefab";
        public static string s_Gift_Form_Path = "UGUI/Form/Common/Form_BuyPick_Gift.prefab";
        private static BuyPickDialog s_theDlg;

        public BuyPickDialog(COM_ITEM_TYPE type, uint id, RES_SHOPBUY_COINTYPE coinType, float discount, uint maxCount, OnConfirmBuyDelegate onConfirm, CMallFactoryShopController.ShopProduct callContext, OnConfirmBuyCommonDelegate onConfirmCommon = new OnConfirmBuyCommonDelegate(), CUIEvent uieventPars = new CUIEvent())
        {
            CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(s_Form_Path, false, true);
            if (null != script)
            {
                this._root = script.get_gameObject();
                this._usb = CUseableManager.CreateUseable(type, id, 0);
                this._bHeroSkinGift = false;
                this._bDynamicCorrectPrice = false;
                this._heroSkinGiftCost = 0;
                this._count = 1;
                this._maxCount = maxCount;
                if (this._maxCount == 0)
                {
                    this._maxCount = 0x3e7;
                }
                this._onConfirm = onConfirm;
                this._callContext = callContext;
                this._onConfirmdCommon = onConfirmCommon;
                this._uieventPars = uieventPars;
                this._coinType = coinType;
                this._realDiscount = discount;
                if (this._usb != null)
                {
                    this._countText = Utility.GetComponetInChild<Text>(this._root, "Panel/Count");
                    this._costText = Utility.GetComponetInChild<Text>(this._root, "Panel/Cost");
                    this._descText = Utility.GetComponetInChild<Text>(this._root, "Panel/Desc/Image/Text");
                    if (this._descText != null)
                    {
                        this._descText.set_text(this._usb.m_description);
                    }
                    Utility.GetComponetInChild<Image>(this._root, "Panel/Slot/Icon").SetSprite(CUIUtility.GetSpritePrefeb(this._usb.GetIconPath(), false, false), false);
                    Utility.GetComponetInChild<Text>(this._root, "Panel/Name").set_text(this._usb.m_name);
                    this._coinUsb = CUseableManager.CreateCoinUseable(coinType, 0);
                    if (this._coinUsb != null)
                    {
                        Utility.GetComponetInChild<Image>(this._root, "Panel/Cost/CoinType").SetSprite(CUIUtility.GetSpritePrefeb(this._coinUsb.GetIconPath(), false, false), false);
                        Utility.GetComponetInChild<Text>(this._root, "Panel/Price").set_text(CMallFactoryShopController.ShopProduct.SConvertWithRealDiscount(this._usb.GetBuyPrice(coinType), this._realDiscount).ToString());
                    }
                    Image componetInChild = Utility.GetComponetInChild<Image>(this._root, "Panel/Slot/imgExperienceMark");
                    if (componetInChild != null)
                    {
                        if ((this._usb.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP) && CItem.IsHeroExperienceCard(this._usb.m_baseID))
                        {
                            componetInChild.get_gameObject().CustomSetActive(true);
                            componetInChild.SetSprite(CUIUtility.GetSpritePrefeb(CExperienceCardSystem.HeroExperienceCardMarkPath, false, false), false);
                        }
                        else if ((this._usb.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP) && CItem.IsSkinExperienceCard(this._usb.m_baseID))
                        {
                            componetInChild.get_gameObject().CustomSetActive(true);
                            componetInChild.SetSprite(CUIUtility.GetSpritePrefeb(CExperienceCardSystem.SkinExperienceCardMarkPath, false, false), false);
                        }
                        else
                        {
                            componetInChild.get_gameObject().CustomSetActive(false);
                        }
                    }
                }
                Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BuyPick_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseForm));
                Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BuyPick_Add, new CUIEventManager.OnUIEventHandler(this.OnClickAdd));
                Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BuyPick_Dec, new CUIEventManager.OnUIEventHandler(this.OnClickDec));
                Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BuyPick_Max, new CUIEventManager.OnUIEventHandler(this.OnClickMax));
                Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BuyPick_Confirm, new CUIEventManager.OnUIEventHandler(this.OnClickConfirm));
                Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BuyPick_Cancel, new CUIEventManager.OnUIEventHandler(this.OnClickCancel));
                Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BuyPick_ConfirmFactoryShopBuy, new CUIEventManager.OnUIEventHandler(this.OnConfirmFactoryShopBuy));
                this.ValidateDynamic();
            }
        }

        public BuyPickDialog(bool isGift, COM_ITEM_TYPE type, uint id, RES_SHOPBUY_COINTYPE coinType, float discount, uint maxCount, OnConfirmBuyDelegate onConfirm, CMallFactoryShopController.ShopProduct callContext, OnConfirmBuyCommonDelegate onConfirmCommon = new OnConfirmBuyCommonDelegate(), CUIEvent uieventPars = new CUIEvent(), bool bfromFactoyShop = false)
        {
            CUIFormScript script = null;
            this.m_bShowBigIcon = (callContext != null) && (callContext.GetSpecialIconPath() != null);
            if (this.m_bShowBigIcon)
            {
                script = Singleton<CUIManager>.GetInstance().OpenForm(s_Gift_Big_Icon_Form_Path, false, true);
            }
            else
            {
                script = Singleton<CUIManager>.GetInstance().OpenForm(s_Gift_Form_Path, false, true);
            }
            if (null != script)
            {
                this._root = script.get_gameObject();
                this._usb = CUseableManager.CreateUseable(type, id, 0);
                this._count = 1;
                this._bHeroSkinGift = false;
                this._bDynamicCorrectPrice = false;
                this._heroSkinGiftCost = 0;
                this._maxCount = maxCount;
                if (this._maxCount == 0)
                {
                    this._maxCount = 0x3e7;
                }
                this._onConfirm = onConfirm;
                this._callContext = callContext;
                this._onConfirmdCommon = onConfirmCommon;
                this._uieventPars = uieventPars;
                this._coinType = coinType;
                this._realDiscount = discount;
                if (this._usb != null)
                {
                    this._countText = Utility.GetComponetInChild<Text>(this._root, "Panel/Count");
                    this._costText = Utility.GetComponetInChild<Text>(this._root, "Panel/Cost");
                    this._descText = Utility.GetComponetInChild<Text>(this._root, "Panel/lblDesc");
                    CItem item = new CItem(0L, id, 0, 0);
                    uint key = (uint) item.m_itemData.EftParam[0];
                    ResRandomRewardStore dataByKey = GameDataMgr.randomRewardDB.GetDataByKey(key);
                    ListView<CUseable> view = new ListView<CUseable>();
                    for (int i = 0; i < dataByKey.astRewardDetail.Length; i++)
                    {
                        if (dataByKey.astRewardDetail[i].bItemType != 0)
                        {
                            CUseable useable = CUseableManager.CreateUsableByRandowReward((RES_RANDOM_REWARD_TYPE) dataByKey.astRewardDetail[i].bItemType, (int) dataByKey.astRewardDetail[i].dwLowCnt, dataByKey.astRewardDetail[i].dwItemID);
                            if (useable != null)
                            {
                                view.Add(useable);
                            }
                        }
                    }
                    if (this._descText != null)
                    {
                        this._descText.set_text(item.m_description);
                    }
                    uint num3 = 0;
                    int num4 = 0;
                    if (this._usb.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
                    {
                        CItem item2 = (CItem) this._usb;
                        if (((item2 != null) && (item2.m_itemData != null)) && (item2.m_itemData.bType == 4))
                        {
                            this._bDynamicCorrectPrice = item2.m_itemData.EftParam[3] > 0f;
                        }
                    }
                    CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(script.get_gameObject(), "Panel/itemGroup");
                    componetInChild.SetElementAmount(view.Count);
                    for (int j = 0; j < view.Count; j++)
                    {
                        CUIListElementScript elemenet = componetInChild.GetElemenet(j);
                        this.UpdateElement(elemenet, view[j], this.m_bShowBigIcon);
                        if (view[j].m_type == COM_ITEM_TYPE.COM_OBJTYPE_HERO)
                        {
                            this._bHeroSkinGift = true;
                            CHeroItem item3 = view[j] as CHeroItem;
                            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                            if ((masterRoleInfo != null) && masterRoleInfo.IsOwnHero(item3.m_heroData.dwCfgID))
                            {
                                num3 += CHeroInfo.GetHeroCost(item3.m_heroData.dwCfgID, coinType);
                                num4++;
                            }
                        }
                        else if (view[j].m_type == COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN)
                        {
                            this._bHeroSkinGift = true;
                            CHeroSkin skin = view[j] as CHeroSkin;
                            CRoleInfo info2 = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                            if ((info2 != null) && info2.IsHaveHeroSkin(skin.m_heroId, skin.m_skinId, false))
                            {
                                num3 += CSkinInfo.GetHeroSkinCost(skin.m_heroId, skin.m_skinId, coinType);
                                num4++;
                            }
                        }
                    }
                    this._coinUsb = CUseableManager.CreateCoinUseable(coinType, 0);
                    if (this._coinUsb != null)
                    {
                        Utility.GetComponetInChild<Image>(this._root, "Panel/Cost/CoinType").SetSprite(CUIUtility.GetSpritePrefeb(this._coinUsb.GetIconPath(), false, false), false);
                    }
                    Text text = Utility.GetComponetInChild<Text>(this._root, "Panel/costDescText");
                    text.set_text(string.Empty);
                    if (this._bHeroSkinGift && this._bDynamicCorrectPrice)
                    {
                        uint buyPrice = this._usb.GetBuyPrice(coinType);
                        Button btn = Utility.GetComponetInChild<Button>(this._root, "Panel/Button_Sale");
                        if (num4 >= view.Count)
                        {
                            CUICommonSystem.SetButtonEnableWithShader(btn, false, true);
                            text.set_text(Singleton<CTextManager>.GetInstance().GetText("Gift_Can_Not_Buy_Tip"));
                            this._heroSkinGiftCost = 0;
                        }
                        else
                        {
                            CUICommonSystem.SetButtonEnableWithShader(btn, true, true);
                            text.set_text(Singleton<CTextManager>.GetInstance().GetText("Gift_Own_Hero_Skin_Tip"));
                            uint num7 = CMallFactoryShopController.ShopProduct.SConvertWithRealDiscount(buyPrice - num3, this._realDiscount);
                            if ((buyPrice >= num3) && (num7 >= (buyPrice / 10)))
                            {
                                this._heroSkinGiftCost = num7;
                            }
                            else
                            {
                                this._heroSkinGiftCost = buyPrice / 10;
                            }
                        }
                        if (this._callContext != null)
                        {
                            this._callContext.m_bChangeGiftPrice = true;
                            this._callContext.m_newGiftPrice = this._heroSkinGiftCost;
                        }
                    }
                }
                Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BuyPick_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseForm));
                Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BuyPick_Add, new CUIEventManager.OnUIEventHandler(this.OnClickAdd));
                Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BuyPick_Dec, new CUIEventManager.OnUIEventHandler(this.OnClickDec));
                Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BuyPick_Max, new CUIEventManager.OnUIEventHandler(this.OnClickMax));
                Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BuyPick_Confirm, new CUIEventManager.OnUIEventHandler(this.OnClickConfirm));
                Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BuyPick_Cancel, new CUIEventManager.OnUIEventHandler(this.OnClickCancel));
                Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BuyPick_ConfirmFactoryShopBuy, new CUIEventManager.OnUIEventHandler(this.OnConfirmFactoryShopBuy));
                this.ValidateDynamic();
            }
        }

        private void OnClickAdd(CUIEvent uiEvent)
        {
            if (this._count < this._maxCount)
            {
                this._count++;
                this.ValidateDynamic();
            }
        }

        private void OnClickCancel(CUIEvent uiEvent)
        {
            this.OnClose(false);
        }

        private void OnClickConfirm(CUIEvent uiEvent)
        {
            if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo() == null)
            {
                this.OnClose(false);
            }
            else
            {
                this.OnClose(true);
            }
        }

        private void OnClickDec(CUIEvent uiEvent)
        {
            if (this._count > 1)
            {
                this._count--;
                this.ValidateDynamic();
            }
        }

        private void OnClickMax(CUIEvent uiEvent)
        {
            if (this._count != this._maxCount)
            {
                this._count = this._maxCount;
                this.ValidateDynamic();
            }
        }

        private void OnClose(bool isOk)
        {
            if (isOk && (this._onConfirm != null))
            {
                this._onConfirm(this._callContext, this._count, false, this._uieventPars);
            }
            else if (isOk && (this._onConfirmdCommon != null))
            {
                this._onConfirmdCommon(this._uieventPars, this._count);
            }
            Singleton<CUIManager>.GetInstance().CloseForm(s_Form_Path);
            Singleton<CUIManager>.GetInstance().CloseForm(s_Gift_Form_Path);
            Singleton<CUIManager>.GetInstance().CloseForm(s_Gift_Big_Icon_Form_Path);
        }

        private void OnCloseForm(CUIEvent uiEvent)
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BuyPick_Add, new CUIEventManager.OnUIEventHandler(this.OnClickAdd));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BuyPick_Dec, new CUIEventManager.OnUIEventHandler(this.OnClickDec));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BuyPick_Max, new CUIEventManager.OnUIEventHandler(this.OnClickMax));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BuyPick_Confirm, new CUIEventManager.OnUIEventHandler(this.OnClickConfirm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BuyPick_Cancel, new CUIEventManager.OnUIEventHandler(this.OnClickCancel));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BuyPick_ConfirmFactoryShopBuy, new CUIEventManager.OnUIEventHandler(this.OnConfirmFactoryShopBuy));
            s_theDlg = null;
        }

        private void OnConfirmFactoryShopBuy(CUIEvent uiEvent)
        {
            this.OnClose(true);
        }

        public static void Show(COM_ITEM_TYPE type, uint id, RES_SHOPBUY_COINTYPE coinType, float discount, uint maxCount, OnConfirmBuyDelegate onClose, CMallFactoryShopController.ShopProduct callContext = new CMallFactoryShopController.ShopProduct(), OnConfirmBuyCommonDelegate onConfirmCommon = new OnConfirmBuyCommonDelegate(), CUIEvent uieventPars = new CUIEvent())
        {
            if (s_theDlg == null)
            {
                if (type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
                {
                    CItem item = new CItem(0L, id, 0, 0);
                    if (item.m_itemData.bIsView != 0)
                    {
                        s_theDlg = new BuyPickDialog(true, type, id, coinType, discount, maxCount, onClose, callContext, onConfirmCommon, uieventPars, false);
                    }
                    else
                    {
                        s_theDlg = new BuyPickDialog(type, id, coinType, discount, maxCount, onClose, callContext, onConfirmCommon, uieventPars);
                    }
                }
                else
                {
                    s_theDlg = new BuyPickDialog(type, id, coinType, discount, maxCount, onClose, callContext, onConfirmCommon, uieventPars);
                }
                if (s_theDlg._root == null)
                {
                    s_theDlg = null;
                }
            }
        }

        private void UpdateElement(CUIListElementScript elementScript, CUseable useable, bool isShowBigIcon)
        {
            CUIFormScript belongedFormScript = elementScript.m_belongedFormScript;
            GameObject widget = elementScript.GetWidget(0);
            GameObject obj3 = elementScript.GetWidget(1);
            GameObject obj4 = elementScript.GetWidget(2);
            if ((useable.m_type != COM_ITEM_TYPE.COM_OBJTYPE_HERO) && (useable.m_type != COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN))
            {
                isShowBigIcon = false;
            }
            if (!isShowBigIcon)
            {
                widget.CustomSetActive(true);
                obj3.CustomSetActive(false);
                if (useable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_HERO)
                {
                    CHeroItem item = useable as CHeroItem;
                    if (item == null)
                    {
                        return;
                    }
                    CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                    if ((masterRoleInfo != null) && masterRoleInfo.IsOwnHero(item.m_heroData.dwCfgID))
                    {
                        obj4.CustomSetActive(true);
                    }
                    else
                    {
                        obj4.CustomSetActive(false);
                    }
                }
                else if (useable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN)
                {
                    CHeroSkin skin = useable as CHeroSkin;
                    if (skin == null)
                    {
                        return;
                    }
                    CRoleInfo info2 = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                    if ((info2 != null) && info2.IsHaveHeroSkin(skin.m_heroId, skin.m_skinId, false))
                    {
                        obj4.CustomSetActive(true);
                    }
                    else
                    {
                        obj4.CustomSetActive(false);
                    }
                }
                else
                {
                    obj4.CustomSetActive(false);
                }
                CUICommonSystem.SetItemCell(elementScript.m_belongedFormScript, elementScript.GetWidget(0), useable, true, false, false, false);
            }
            else
            {
                widget.CustomSetActive(false);
                obj3.CustomSetActive(true);
                Image componetInChild = Utility.GetComponetInChild<Image>(obj3, "imageIcon");
                GameObject obj5 = Utility.FindChild(obj3, "skinLabelImage");
                GameObject obj6 = Utility.FindChild(obj3, "nameContainer/heroNameText");
                Text component = obj6.GetComponent<Text>();
                GameObject obj7 = Utility.FindChild(obj3, "nameContainer/heroSkinText");
                Text text2 = obj7.GetComponent<Text>();
                if (useable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_HERO)
                {
                    CHeroItem item2 = useable as CHeroItem;
                    if (item2 != null)
                    {
                        CRoleInfo info3 = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                        if ((info3 != null) && info3.IsOwnHero(item2.m_heroData.dwCfgID))
                        {
                            obj4.CustomSetActive(true);
                        }
                        else
                        {
                            obj4.CustomSetActive(false);
                        }
                        string prefabPath = CUIUtility.s_Sprite_Dynamic_BustHero_Dir + item2.m_iconID;
                        componetInChild.SetSprite(prefabPath, belongedFormScript, false, true, true, true);
                        obj6.CustomSetActive(true);
                        component.set_text(useable.m_name);
                        obj5.CustomSetActive(false);
                        obj7.CustomSetActive(false);
                    }
                }
                else if (useable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN)
                {
                    CHeroSkin skin2 = useable as CHeroSkin;
                    if (skin2 != null)
                    {
                        IHeroData data = CHeroDataFactory.CreateHeroData(skin2.m_heroId);
                        CRoleInfo info4 = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                        if ((info4 != null) && info4.IsHaveHeroSkin(skin2.m_heroId, skin2.m_skinId, false))
                        {
                            obj4.CustomSetActive(true);
                        }
                        else
                        {
                            obj4.CustomSetActive(false);
                        }
                        string str2 = CUIUtility.s_Sprite_Dynamic_BustHero_Dir + skin2.m_iconID;
                        componetInChild.SetSprite(str2, belongedFormScript, false, true, true, true);
                        obj6.CustomSetActive(true);
                        component.set_text(data.heroName);
                        obj5.CustomSetActive(true);
                        CUICommonSystem.SetHeroSkinLabelPic(belongedFormScript, obj5, skin2.m_heroId, skin2.m_skinId);
                        obj7.CustomSetActive(true);
                        text2.set_text(useable.m_name);
                    }
                }
            }
        }

        private void ValidateDynamic()
        {
            if (this._usb != null)
            {
                this._countText.set_text(this._count.ToString());
                if (this._bHeroSkinGift && this._bDynamicCorrectPrice)
                {
                    this._costText.set_text((this._count * this._heroSkinGiftCost).ToString());
                }
                else
                {
                    this._costText.set_text(CMallFactoryShopController.ShopProduct.SConvertWithRealDiscount(this._count * this._usb.GetBuyPrice(this._coinType), this._realDiscount).ToString());
                }
            }
        }

        public delegate void OnConfirmBuyCommonDelegate(CUIEvent uievent, uint count);

        public delegate void OnConfirmBuyDelegate(CMallFactoryShopController.ShopProduct shopProduct, uint count, bool needConfirm, CUIEvent uiEvent);
    }
}

