namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class CSymbolBuyPickDialog
    {
        private ResSymbolInfo _callContext;
        private RES_SHOPBUY_COINTYPE _coinType;
        private CUseable _coinUsb;
        private Text _costText;
        private uint _count;
        private Text _countText;
        private Text _descText;
        private GameObject _itemCell;
        private uint _maxCount;
        private OnConfirmBuyDelegate _onConfirm;
        private OnConfirmBuyCommonDelegate _onConfirmdCommon;
        private float _realDiscount;
        private GameObject _root;
        private CUIEvent _uieventPars;
        private CSymbolItem _usb;
        public const uint MAX_SYMBOL_CNT = 10;
        public static string s_Form_Path = "UGUI/Form/System/Symbol/Form_BuyPick_Symbol.prefab";
        private static CSymbolBuyPickDialog s_theDlg;

        public CSymbolBuyPickDialog(ResSymbolInfo symbolInfo, RES_SHOPBUY_COINTYPE coinType, float discount, OnConfirmBuyDelegate onConfirm, OnConfirmBuyCommonDelegate onConfirmCommon = new OnConfirmBuyCommonDelegate(), CUIEvent uieventPars = new CUIEvent())
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                CUseableContainer useableContainer = masterRoleInfo.GetUseableContainer(enCONTAINER_TYPE.ITEM);
                this._usb = useableContainer.GetUseableByBaseID(COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL, symbolInfo.dwID) as CSymbolItem;
                if (this._usb == null)
                {
                    this._usb = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL, symbolInfo.dwID, 0) as CSymbolItem;
                }
                if (this._usb != null)
                {
                    CUIFormScript formScript = Singleton<CUIManager>.GetInstance().OpenForm(s_Form_Path, false, true);
                    if (null != formScript)
                    {
                        this._root = formScript.get_gameObject();
                        this._itemCell = Utility.FindChild(this._root, "Panel/itemCell");
                        this._callContext = symbolInfo;
                        this._count = 1;
                        if (this._usb.m_stackCount >= 10L)
                        {
                            this._maxCount = 1;
                        }
                        else
                        {
                            this._maxCount = (uint) (10 - this._usb.m_stackCount);
                        }
                        this._onConfirm = onConfirm;
                        this._onConfirmdCommon = onConfirmCommon;
                        this._uieventPars = uieventPars;
                        this._coinType = coinType;
                        this._realDiscount = discount;
                        if (this._usb != null)
                        {
                            this._countText = Utility.GetComponetInChild<Text>(this._root, "Panel/Count");
                            this._costText = Utility.GetComponetInChild<Text>(this._root, "Panel/Cost");
                            CUICommonSystem.SetItemCell(formScript, this._itemCell, this._usb, true, false, false, false);
                            this._descText = Utility.GetComponetInChild<Text>(this._root, "Panel/itemCell/SymbolDesc");
                            if (this._descText != null)
                            {
                                this._descText.set_text(CSymbolSystem.GetSymbolAttString(symbolInfo.dwID, true));
                            }
                            this._coinUsb = CUseableManager.CreateCoinUseable(coinType, 0);
                            if (this._coinUsb != null)
                            {
                                Utility.GetComponetInChild<Image>(this._root, "Panel/Cost/CoinType").SetSprite(CUIUtility.GetSpritePrefeb(this._coinUsb.GetIconPath(), false, false), false);
                                Utility.GetComponetInChild<Text>(this._root, "Panel/Price").set_text(this._usb.GetBuyPrice(this._coinType).ToString());
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
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo == null)
            {
                this.OnClose(false);
            }
            else if (this._usb == null)
            {
                this.OnClose(false);
            }
            else if (masterRoleInfo.SymbolCoin < (this._count * this._usb.GetBuyPrice(this._coinType)))
            {
                string text = Singleton<CTextManager>.GetInstance().GetText("Symbol_Coin_Not_Enough_Tip");
                Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(text, enUIEventID.Symbol_Jump_To_MiShu, enUIEventID.None, false);
                this.OnClose(false);
            }
            else if ((this._usb.m_stackCount + this._count) > 10L)
            {
                string strContent = string.Format(Singleton<CTextManager>.GetInstance().GetText("Symbol_Buy_Count_Limit"), this._usb.m_stackCount, this._usb.m_name);
                Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(strContent, enUIEventID.BuyPick_ConfirmFactoryShopBuy, enUIEventID.BuyPick_Cancel, false);
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
            s_theDlg = null;
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

        public static void Show(ResSymbolInfo symbolInfo, RES_SHOPBUY_COINTYPE coinType, float discount, OnConfirmBuyDelegate onClose, OnConfirmBuyCommonDelegate onConfirmCommon = new OnConfirmBuyCommonDelegate(), CUIEvent uieventPars = new CUIEvent())
        {
            if (s_theDlg == null)
            {
                s_theDlg = new CSymbolBuyPickDialog(symbolInfo, coinType, discount, onClose, onConfirmCommon, uieventPars);
                if (s_theDlg._root == null)
                {
                    s_theDlg = null;
                }
            }
        }

        private void ValidateDynamic()
        {
            if (this._usb != null)
            {
                this._countText.set_text(this._count.ToString());
                this._costText.set_text((this._count * this._usb.GetBuyPrice(this._coinType)).ToString());
            }
        }

        public delegate void OnConfirmBuyCommonDelegate(CUIEvent uievent, uint count);

        public delegate void OnConfirmBuyDelegate(ResSymbolInfo symbol, uint count, bool needConfirm, CUIEvent uiEvent);
    }
}

