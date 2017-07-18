namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [MessageHandlerClass]
    public class MySteryShop : Singleton<MySteryShop>
    {
        public COMDT_AKALISHOP_DETAIL m_AkaliShopDetail;
        public COMDT_ACNT_AKALISHOP_INFO m_AkaliShopInfoSvr;
        public byte m_bRequestBuyCount;
        public byte m_bRequestDiscount;
        public static string MYSTERY_ROLL_DISCOUNT_FORM_PATH = "UGUI/Form/System/Mall/Form_NewDiscount.prefab";

        public void ClearSvrData()
        {
            this.m_AkaliShopDetail = null;
            this.m_AkaliShopInfoSvr = null;
            this.m_bRequestDiscount = 0;
            this.m_bRequestBuyCount = 0;
        }

        public int GetDisCount()
        {
            if (this.m_AkaliShopDetail != null)
            {
                return this.m_AkaliShopDetail.iShowDiscount;
            }
            return -1;
        }

        public string GetDiscountNumIconPath(uint discount)
        {
            if ((discount > 0) && (discount < 100))
            {
                return (CUIUtility.s_Sprite_System_Mall_Dir + string.Format("Discount_Bg_N{0}", discount / 10));
            }
            return string.Format("{0}{1}", CUIUtility.s_Sprite_System_Mall_Dir, "Discount_Bg_WenHao");
        }

        private int GetProductID(uint itemID)
        {
            if (this.m_AkaliShopDetail != null)
            {
                for (int i = 0; i < this.m_AkaliShopDetail.bGoodsCnt; i++)
                {
                    if (this.m_AkaliShopDetail.astGoodsList[i].dwItemID == itemID)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public int GetTimeToClose()
        {
            if (this.m_AkaliShopDetail != null)
            {
                TimeSpan span = (TimeSpan) (Utility.ToUtcTime2Local((long) this.m_AkaliShopDetail.dwEndTime) - Utility.ToUtcTime2Local((long) CRoleInfo.GetCurrentUTCTime()));
                if (span.TotalSeconds > 0.0)
                {
                    return ((span.TotalSeconds <= 2147483647.0) ? ((int) span.TotalSeconds) : 0x7fffffff);
                }
            }
            return 0;
        }

        public override void Init()
        {
            base.Init();
            this.m_AkaliShopDetail = null;
            this.InitEvent();
        }

        private void InitEvent()
        {
            CUIEventManager instance = Singleton<CUIEventManager>.GetInstance();
            instance.AddUIEventListener(enUIEventID.Mall_Mystery_ItemEnable, new CUIEventManager.OnUIEventHandler(this.OnElementEnable));
            instance.AddUIEventListener(enUIEventID.Mall_Mystery_On_Roll_Discount, new CUIEventManager.OnUIEventHandler(this.OnRollDiscount));
            instance.AddUIEventListener(enUIEventID.Mall_Mystery_On_Open_Buy_Form, new CUIEventManager.OnUIEventHandler(this.OnOpenBuyItem));
            instance.AddUIEventListener(enUIEventID.Mall_Mystery_On_Buy_Item, new CUIEventManager.OnUIEventHandler(this.OnBuyItem));
            instance.AddUIEventListener(enUIEventID.Mall_Mystery_On_Confirm_Buy_Item, new CUIEventManager.OnUIEventHandler(this.OnConfirmuyItem));
            instance.AddUIEventListener(enUIEventID.Mall_Mystery_On_Time_End, new CUIEventManager.OnUIEventHandler(this.OnShopTimeEnd));
            instance.AddUIEventListener(enUIEventID.Mall_Mystery_On_Default_Item_Click, new CUIEventManager.OnUIEventHandler(this.OnDefaultItemClick));
            Singleton<EventRouter>.instance.AddEventHandler<uint>("HeroAdd", new Action<uint>(this.OnNtyAddHero));
            Singleton<EventRouter>.instance.AddEventHandler<uint, uint, uint>("HeroSkinAdd", new Action<uint, uint, uint>(this, (IntPtr) this.OnAddHeroSkin));
        }

        public bool IsGetDisCount()
        {
            if (this.m_AkaliShopInfoSvr == null)
            {
                return false;
            }
            return (this.m_bRequestDiscount > 0);
        }

        public bool IsShopAvailable()
        {
            return (((this.m_AkaliShopDetail != null) && (this.m_AkaliShopInfoSvr != null)) && (this.GetTimeToClose() > 0));
        }

        public void Load(CUIFormScript form)
        {
            CUICommonSystem.LoadUIPrefab("UGUI/Form/System/Mall/Mystery", "pnlMystery", form.GetWidget(3), form);
        }

        public bool Loaded(CUIFormScript form)
        {
            if (Utility.FindChild(form.GetWidget(3), "pnlMystery") == null)
            {
                return false;
            }
            return true;
        }

        private void OnAddHeroSkin(uint heroId, uint skinId, uint addReason)
        {
            this.UpdateUI();
        }

        private void OnBuyItem(CUIEvent uiEvent)
        {
            enPayType tag = (enPayType) uiEvent.m_eventParams.tag;
            uint payValue = uiEvent.m_eventParams.commonUInt32Param1;
            string goodName = string.Empty;
            COM_ITEM_TYPE com_item_type = COM_ITEM_TYPE.COM_OBJTYPE_NULL;
            uint uniSkinId = 0;
            if (uiEvent.m_eventParams.heroSkinParam.skinId != 0)
            {
                com_item_type = COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN;
                uniSkinId = CSkinInfo.GetSkinCfgId(uiEvent.m_eventParams.heroSkinParam.heroId, uiEvent.m_eventParams.heroSkinParam.skinId);
                ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(uniSkinId);
                DebugHelper.Assert(heroSkin != null, string.Format("找不到皮肤{0}的配置信息", uniSkinId));
                if (heroSkin == null)
                {
                    return;
                }
                goodName = StringHelper.UTF8BytesToString(ref heroSkin.szSkinName);
            }
            else if (uiEvent.m_eventParams.heroId != 0)
            {
                com_item_type = COM_ITEM_TYPE.COM_OBJTYPE_HERO;
                uniSkinId = uiEvent.m_eventParams.heroId;
                ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(uniSkinId);
                DebugHelper.Assert(dataByKey != null, string.Format("找不到英雄{0}的配置信息", uniSkinId));
                if (dataByKey == null)
                {
                    return;
                }
                goodName = StringHelper.UTF8BytesToString(ref dataByKey.szName);
            }
            else
            {
                DebugHelper.Assert(false, "神秘商店购买不支持该物品类型");
                return;
            }
            int productID = this.GetProductID(uniSkinId);
            if (productID < 0)
            {
                DebugHelper.Assert(false, string.Format("神秘商店找不到该物品{0}/{1}", Enum.GetName(typeof(COM_ITEM_TYPE), com_item_type), uniSkinId));
            }
            else
            {
                CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
                uIEvent.m_eventID = enUIEventID.Mall_Mystery_On_Confirm_Buy_Item;
                uIEvent.m_eventParams.tag = productID;
                CMallSystem.TryToPay(enPayPurpose.Buy, goodName, tag, payValue, uIEvent.m_eventID, ref uIEvent.m_eventParams, enUIEventID.None, false, true, false);
            }
        }

        private void OnConfirmuyItem(CUIEvent uiEvent)
        {
            if ((this.m_AkaliShopDetail != null) && (this.m_AkaliShopInfoSvr != null))
            {
                int tag = uiEvent.m_eventParams.tag;
                if (tag < 0)
                {
                    DebugHelper.Assert(false, "商品ID不能为0");
                    Singleton<CUIManager>.GetInstance().OpenTips("该商品无法购买", false, 1.5f, null, new object[0]);
                }
                else if (this.m_bRequestBuyCount >= this.m_AkaliShopDetail.bMaxBuyCnt)
                {
                    Singleton<CUIManager>.GetInstance().OpenTips("您的购买次数已达到神秘商店限购的次数，欢迎下次再来", false, 1.5f, null, new object[0]);
                }
                else
                {
                    this.RequestBuyItem((byte) tag);
                }
            }
        }

        private void OnDefaultItem(CUIEvent uiEvent)
        {
            CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
            GameObject srcWidget = uiEvent.m_srcWidget;
            if ((srcFormScript != null) && (srcWidget != null))
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                DebugHelper.Assert(masterRoleInfo != null, "master roleInfo is null");
                if (masterRoleInfo != null)
                {
                    GameObject obj3 = Utility.FindChild(srcWidget, "defualtItem");
                    if (obj3 != null)
                    {
                        obj3.CustomSetActive(true);
                    }
                    GameObject obj4 = Utility.FindChild(srcWidget, "heroItem");
                    if (obj4 != null)
                    {
                        obj4.CustomSetActive(false);
                    }
                    GameObject obj5 = Utility.FindChild(srcWidget, "imgExperienceMark");
                    if (obj5 != null)
                    {
                        obj5.CustomSetActive(false);
                    }
                    GameObject obj6 = Utility.FindChild(srcWidget, "ButtonGroup/BuyBtn/Text");
                    if (obj6 != null)
                    {
                        obj6.GetComponent<Text>().set_text("敬请期待");
                    }
                }
            }
        }

        private void OnDefaultItemClick(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.GetInstance().OpenMessageBox(Singleton<CTextManager>.GetInstance().GetText("Mall_Mystery_Default_Item_Click_Tips"), false);
        }

        private void OnElementEnable(CUIEvent uiEvent)
        {
            if (this.m_AkaliShopDetail != null)
            {
                if (!this.IsGetDisCount())
                {
                    this.OnDefaultItem(uiEvent);
                }
                else
                {
                    int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
                    if (srcWidgetIndexInBelongedList <= this.m_AkaliShopDetail.bGoodsCnt)
                    {
                        COMDT_AKALISHOP_GOODS productInfo = this.m_AkaliShopDetail.astGoodsList[srcWidgetIndexInBelongedList];
                        if (productInfo != null)
                        {
                            this.UpdateItem(uiEvent, productInfo);
                        }
                    }
                }
            }
        }

        public void OnNtyAddHero(uint id)
        {
            this.UpdateUI();
        }

        private void OnOpenBuyItem(CUIEvent uiEvent)
        {
            if (this.IsShopAvailable() && ((this.m_AkaliShopDetail != null) && (this.m_AkaliShopInfoSvr != null)))
            {
                int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
                if (srcWidgetIndexInBelongedList <= this.m_AkaliShopDetail.bGoodsCnt)
                {
                    COMDT_AKALISHOP_GOODS productInfo = this.m_AkaliShopDetail.astGoodsList[srcWidgetIndexInBelongedList];
                    if (productInfo != null)
                    {
                        this.OpenBuy(uiEvent.m_srcFormScript, ref productInfo);
                    }
                }
            }
        }

        private void OnRollDiscount(CUIEvent uiEvent)
        {
            if (!this.IsShopAvailable())
            {
                DebugHelper.Assert(false, "神秘商店未开启");
            }
            else if (this.IsGetDisCount())
            {
                DebugHelper.Assert(false, "随机折扣不能重复获取");
            }
            else
            {
                this.RequestDisCount();
            }
        }

        private void OnShopTimeEnd(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.GetInstance().OpenTips("神秘商店已关闭", false, 1.5f, null, new object[0]);
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mall_CloseForm);
        }

        public void OpenBuy(CUIFormScript form, ref COMDT_AKALISHOP_GOODS productInfo)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            DebugHelper.Assert(masterRoleInfo != null, "master roleInfo is null");
            if (masterRoleInfo != null)
            {
                switch (productInfo.wItemType)
                {
                    case 4:
                    {
                        ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(productInfo.dwItemID);
                        DebugHelper.Assert(dataByKey != null, "神秘商店配置的英雄ID有错，英雄表里不存在");
                        if (dataByKey != null)
                        {
                            if (masterRoleInfo.IsHaveHero(dataByKey.dwCfgID, false))
                            {
                                stUIEventParams par = new stUIEventParams();
                                par.openHeroFormPar.heroId = dataByKey.dwCfgID;
                                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.HeroInfo_OpenForm, par);
                                return;
                            }
                            stPayInfoSet payInfoSet = new stPayInfoSet();
                            payInfoSet.m_payInfoCount = 1;
                            payInfoSet.m_payInfos = new stPayInfo[1];
                            stPayInfo info3 = new stPayInfo();
                            info3.m_oriValue = productInfo.dwOrigPrice;
                            info3.m_payValue = productInfo.dwRealPrice;
                            info3.m_payType = enPayType.DianQuan;
                            payInfoSet.m_payInfos[0] = info3;
                            CHeroSkinBuyManager.OpenBuyHeroForm(form, dataByKey.dwCfgID, payInfoSet, enUIEventID.Mall_Mystery_On_Buy_Item);
                            break;
                        }
                        return;
                    }
                    case 7:
                    {
                        ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(productInfo.dwItemID);
                        DebugHelper.Assert(heroSkin != null, "神秘商店配置的皮肤ID有错，皮肤表里不存在");
                        if (heroSkin != null)
                        {
                            ResHeroCfgInfo info4 = GameDataMgr.heroDatabin.GetDataByKey(heroSkin.dwHeroID);
                            DebugHelper.Assert(info4 != null, "神秘商店配置的皮肤ID有错，皮肤对应的英雄不存在");
                            if (info4 == null)
                            {
                                return;
                            }
                            if (masterRoleInfo.IsHaveHeroSkin(heroSkin.dwHeroID, heroSkin.dwSkinID, false))
                            {
                                stUIEventParams params2 = new stUIEventParams();
                                params2.openHeroFormPar.heroId = heroSkin.dwHeroID;
                                params2.openHeroFormPar.skinId = heroSkin.dwSkinID;
                                params2.openHeroFormPar.openSrc = enHeroFormOpenSrc.SkinBuyClick;
                                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.HeroInfo_OpenForm, params2);
                                return;
                            }
                            if (masterRoleInfo.IsCanBuySkinButNotHaveHero(heroSkin.dwHeroID, heroSkin.dwSkinID))
                            {
                                stUIEventParams params3 = new stUIEventParams();
                                params3.heroId = heroSkin.dwHeroID;
                                Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(string.Format("暂未拥有英雄{0}，是否购买", StringHelper.UTF8BytesToString(ref info4.szName)), enUIEventID.Mall_Mystery_On_Buy_Hero_Not_Own, enUIEventID.None, params3, false);
                                return;
                            }
                            stPayInfoSet set2 = new stPayInfoSet();
                            set2.m_payInfoCount = 1;
                            set2.m_payInfos = new stPayInfo[1];
                            stPayInfo info5 = new stPayInfo();
                            info5.m_oriValue = productInfo.dwOrigPrice;
                            info5.m_payValue = productInfo.dwRealPrice;
                            info5.m_payType = enPayType.DianQuan;
                            set2.m_payInfos[0] = info5;
                            CHeroSkinBuyManager.OpenBuyHeroSkinForm(heroSkin.dwHeroID, heroSkin.dwSkinID, true, set2, enUIEventID.Mall_Mystery_On_Buy_Item);
                            break;
                        }
                        return;
                    }
                }
            }
        }

        [MessageHandler(0x57e)]
        public static void ReceiveAkAlishopDetail(CSPkg msg)
        {
            Singleton<MySteryShop>.GetInstance().m_AkaliShopDetail = msg.stPkgData.stAkaliShopInfo.stAkaliShopData;
            Singleton<MySteryShop>.GetInstance().m_AkaliShopInfoSvr = msg.stPkgData.stAkaliShopInfo.stAkaliShopBuy;
            Singleton<MySteryShop>.GetInstance().m_bRequestDiscount = msg.stPkgData.stAkaliShopInfo.stAkaliShopBuy.bAlreadyGet;
            Singleton<MySteryShop>.GetInstance().m_bRequestBuyCount = msg.stPkgData.stAkaliShopInfo.stAkaliShopBuy.bBuyCnt;
        }

        [MessageHandler(0x580)]
        public static void ReceiveBuyItem(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            if (msg.stPkgData.stAkaliShopBuyRsp.iResult == 0)
            {
                MySteryShop instance = Singleton<MySteryShop>.GetInstance();
                instance.m_bRequestBuyCount = (byte) (instance.m_bRequestBuyCount + 1);
                Singleton<MySteryShop>.GetInstance().UpdateUI();
                byte bBuyIdx = msg.stPkgData.stAkaliShopBuyRsp.bBuyIdx;
                if (bBuyIdx < Singleton<MySteryShop>.GetInstance().m_AkaliShopDetail.bGoodsCnt)
                {
                    uint dwItemID = Singleton<MySteryShop>.GetInstance().m_AkaliShopDetail.astGoodsList[bBuyIdx].dwItemID;
                    if (Singleton<MySteryShop>.GetInstance().m_AkaliShopDetail.astGoodsList[bBuyIdx].wItemType == 4)
                    {
                        CUICommonSystem.ShowNewHeroOrSkin(dwItemID, 0, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO, false, null, enFormPriority.Priority1, 0, 0);
                    }
                    else if (Singleton<MySteryShop>.GetInstance().m_AkaliShopDetail.astGoodsList[bBuyIdx].wItemType != 7)
                    {
                    }
                }
            }
            else
            {
                Singleton<CUIManager>.GetInstance().OpenTips(Utility.ProtErrCodeToStr(0x580, msg.stPkgData.stAkaliShopBuyRsp.iResult), false, 1.5f, null, new object[0]);
            }
        }

        [MessageHandler(0x582)]
        public static void ReceiveDiscount(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            if (msg.stPkgData.stAkaliShopFlagRsp.iResult == 0)
            {
                Singleton<MySteryShop>.GetInstance().m_bRequestDiscount = 1;
                Singleton<MySteryShop>.GetInstance().UpdateUI();
                CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(MYSTERY_ROLL_DISCOUNT_FORM_PATH, false, true);
                DebugHelper.Assert(script != null, "获得随机折扣form失败");
                if (script != null)
                {
                    CUICommonSystem.PlayAnimator(Utility.FindChild(script.get_gameObject(), "Panel_NewDiscount/Content/Discount"), string.Format("Discount_{0}", Singleton<MySteryShop>.GetInstance().m_AkaliShopDetail.iShowDiscount / 10));
                }
            }
        }

        private void RefreshBanner()
        {
            this.RefreshDiscount();
        }

        private void RefreshDiscount()
        {
            if ((this.m_AkaliShopDetail != null) && (this.m_AkaliShopInfoSvr != null))
            {
                CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
                if ((mallForm != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
                {
                    GameObject obj2 = Utility.FindChild(mallForm.get_gameObject(), "pnlBodyBg/pnlMystery/Content/Banner/PL_Content/GetDiscountBtn");
                    GameObject obj3 = Utility.FindChild(mallForm.get_gameObject(), "pnlBodyBg/pnlMystery/Content/Banner/PL_Content/BoughtLimit");
                    GameObject obj4 = Utility.FindChild(mallForm.get_gameObject(), "pnlBodyBg/pnlMystery/Content/Banner/Discount/");
                    GameObject obj5 = Utility.FindChild(mallForm.get_gameObject(), "pnlBodyBg/pnlMystery/Content/Banner/UnkownDiscount");
                    GameObject obj6 = Utility.FindChild(mallForm.get_gameObject(), "pnlBodyBg/pnlMystery/Content/Banner/DiscountAfter");
                    GameObject obj7 = Utility.FindChild(mallForm.get_gameObject(), "pnlBodyBg/pnlMystery/Content/Banner/DiscountAfter/DiscountBg");
                    if (this.IsGetDisCount())
                    {
                        obj2.CustomSetActive(false);
                        obj5.CustomSetActive(false);
                        obj3.CustomSetActive(true);
                        obj4.CustomSetActive(false);
                        obj7.CustomSetActive(true);
                        obj6.CustomSetActive(true);
                        if (obj7 != null)
                        {
                            Image componetInChild = Utility.GetComponetInChild<Image>(obj7, "Num");
                            if (componetInChild != null)
                            {
                                componetInChild.SetSprite(this.GetDiscountNumIconPath((uint) this.m_AkaliShopDetail.iShowDiscount), mallForm, true, false, false, false);
                            }
                        }
                        if (obj3 != null)
                        {
                            Text text = Utility.GetComponetInChild<Text>(obj3, "Cnt");
                            if (text != null)
                            {
                                text.set_text(string.Format("{0}/{1}", this.m_bRequestBuyCount, this.m_AkaliShopDetail.bMaxBuyCnt));
                            }
                        }
                    }
                    else
                    {
                        obj2.CustomSetActive(true);
                        obj5.CustomSetActive(true);
                        obj3.CustomSetActive(false);
                        obj4.CustomSetActive(true);
                        obj7.CustomSetActive(false);
                        obj6.CustomSetActive(false);
                    }
                }
            }
        }

        private void RefreshTimer()
        {
            if (this.IsShopAvailable())
            {
                CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
                if ((mallForm != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
                {
                    CUITimerScript componetInChild = Utility.GetComponetInChild<CUITimerScript>(mallForm.get_gameObject(), "pnlBodyBg/pnlMystery/Content/Banner/TimeLeft/Timer");
                    int timeToClose = this.GetTimeToClose();
                    if (timeToClose > 0x15180)
                    {
                        componetInChild.m_timerDisplayType = enTimerDisplayType.D_H_M_S;
                    }
                    else
                    {
                        componetInChild.m_timerDisplayType = enTimerDisplayType.H_M_S;
                    }
                    componetInChild.SetTotalTime((float) timeToClose);
                    componetInChild.StartTimer();
                }
            }
        }

        private void RequestBuyItem(byte buyIdx)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x57f);
            msg.stPkgData.stAkaliShopBuyReq.bBuyIdx = buyIdx;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        private void RequestDisCount()
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x581);
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public override void UnInit()
        {
            this.m_AkaliShopDetail = null;
            this.UnInitEvent();
            this.m_bRequestDiscount = 0;
            this.m_bRequestBuyCount = 0;
        }

        private void UnInitEvent()
        {
            CUIEventManager instance = Singleton<CUIEventManager>.GetInstance();
            instance.RemoveUIEventListener(enUIEventID.Mall_Mystery_ItemEnable, new CUIEventManager.OnUIEventHandler(this.OnElementEnable));
            instance.RemoveUIEventListener(enUIEventID.Mall_Mystery_On_Roll_Discount, new CUIEventManager.OnUIEventHandler(this.OnRollDiscount));
            instance.RemoveUIEventListener(enUIEventID.Mall_Mystery_On_Open_Buy_Form, new CUIEventManager.OnUIEventHandler(this.OnOpenBuyItem));
            instance.RemoveUIEventListener(enUIEventID.Mall_Mystery_On_Buy_Item, new CUIEventManager.OnUIEventHandler(this.OnBuyItem));
            instance.RemoveUIEventListener(enUIEventID.Mall_Mystery_On_Confirm_Buy_Item, new CUIEventManager.OnUIEventHandler(this.OnConfirmuyItem));
            instance.RemoveUIEventListener(enUIEventID.Mall_Mystery_On_Time_End, new CUIEventManager.OnUIEventHandler(this.OnShopTimeEnd));
            instance.RemoveUIEventListener(enUIEventID.Mall_Mystery_On_Default_Item_Click, new CUIEventManager.OnUIEventHandler(this.OnDefaultItemClick));
            Singleton<EventRouter>.instance.RemoveEventHandler<uint>("HeroAdd", new Action<uint>(this.OnNtyAddHero));
            Singleton<EventRouter>.instance.RemoveEventHandler<uint, uint, uint>("HeroSkinAdd", new Action<uint, uint, uint>(this, (IntPtr) this.OnAddHeroSkin));
        }

        private void UpdateItem(CUIEvent uiEvent, COMDT_AKALISHOP_GOODS productInfo)
        {
            if (productInfo != null)
            {
                CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
                GameObject srcWidget = uiEvent.m_srcWidget;
                if ((srcFormScript != null) && (srcWidget != null))
                {
                    CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                    DebugHelper.Assert(masterRoleInfo != null, "master roleInfo is null");
                    if (masterRoleInfo != null)
                    {
                        GameObject obj3 = Utility.FindChild(srcWidget, "heroItem");
                        if (obj3 == null)
                        {
                            DebugHelper.Assert(obj3 != null, "hero item is null");
                        }
                        else
                        {
                            GameObject obj4 = Utility.FindChild(srcWidget, "defualtItem");
                            if (obj4 != null)
                            {
                                obj4.CustomSetActive(false);
                            }
                            if (obj3 != null)
                            {
                                obj3.CustomSetActive(true);
                            }
                            Text componetInChild = Utility.GetComponetInChild<Text>(obj3, "heroDataPanel/heroNamePanel/heroNameText");
                            GameObject obj5 = Utility.FindChild(obj3, "heroDataPanel/heroNamePanel/heroSkinText");
                            if (obj5 != null)
                            {
                                Text component = obj5.GetComponent<Text>();
                                GameObject obj6 = Utility.FindChild(obj3, "tag");
                                if (obj6 != null)
                                {
                                    GameObject obj7 = Utility.FindChild(obj3, "profession");
                                    if (obj7 != null)
                                    {
                                        GameObject obj8 = Utility.FindChild(srcWidget, "imgExperienceMark");
                                        if (obj8 != null)
                                        {
                                            if (obj8 != null)
                                            {
                                                obj8.CustomSetActive(true);
                                            }
                                            GameObject obj9 = Utility.FindChild(obj3, "skinLabelImage");
                                            if (obj9 != null)
                                            {
                                                GameObject obj10 = Utility.FindChild(obj3, "heroDataPanel/heroPricePanel");
                                                if (obj10 != null)
                                                {
                                                    obj10.CustomSetActive(false);
                                                    GameObject obj11 = Utility.FindChild(srcWidget, "ButtonGroup/BuyBtn");
                                                    if (obj11 != null)
                                                    {
                                                        obj11.CustomSetActive(false);
                                                        Text text3 = Utility.GetComponetInChild<Text>(obj11, "Text");
                                                        Button button = obj11.GetComponent<Button>();
                                                        if (button != null)
                                                        {
                                                            CUIEventScript script2 = obj11.GetComponent<CUIEventScript>();
                                                            if (script2 != null)
                                                            {
                                                                script2.set_enabled(false);
                                                                button.set_enabled(false);
                                                                GameObject obj12 = Utility.FindChild(srcWidget, "ButtonGroup/LinkBtn");
                                                                if (obj12 != null)
                                                                {
                                                                    obj12.CustomSetActive(false);
                                                                    Text text4 = Utility.GetComponetInChild<Text>(obj12, "Text");
                                                                    Button button2 = obj12.GetComponent<Button>();
                                                                    if (button2 != null)
                                                                    {
                                                                        CUIEventScript script3 = obj12.GetComponent<CUIEventScript>();
                                                                        if (script3 != null)
                                                                        {
                                                                            script3.set_enabled(false);
                                                                            button2.set_enabled(false);
                                                                            COM_ITEM_TYPE wItemType = (COM_ITEM_TYPE) productInfo.wItemType;
                                                                            uint dwItemID = productInfo.dwItemID;
                                                                            switch (wItemType)
                                                                            {
                                                                                case COM_ITEM_TYPE.COM_OBJTYPE_HERO:
                                                                                {
                                                                                    ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(dwItemID);
                                                                                    DebugHelper.Assert(dataByKey != null, "神秘商店配置的英雄ID有错，英雄表里不存在");
                                                                                    if (dataByKey != null)
                                                                                    {
                                                                                        ResHeroShop shop = null;
                                                                                        GameDataMgr.heroShopInfoDict.TryGetValue(dataByKey.dwCfgID, out shop);
                                                                                        CUICommonSystem.SetHeroItemImage(uiEvent.m_srcFormScript, obj3, StringHelper.UTF8BytesToString(ref dataByKey.szImagePath), enHeroHeadType.enBust, false, true);
                                                                                        obj7.CustomSetActive(false);
                                                                                        obj9.CustomSetActive(false);
                                                                                        obj5.CustomSetActive(false);
                                                                                        if (componetInChild != null)
                                                                                        {
                                                                                            componetInChild.set_text(StringHelper.UTF8BytesToString(ref dataByKey.szName));
                                                                                        }
                                                                                        if (masterRoleInfo.IsHaveHero(dataByKey.dwCfgID, false))
                                                                                        {
                                                                                            obj11.CustomSetActive(true);
                                                                                            text3.set_text(Singleton<CTextManager>.GetInstance().GetText("Mall_Hero_State_Own"));
                                                                                            obj6.CustomSetActive(false);
                                                                                            obj8.CustomSetActive(false);
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            obj8.CustomSetActive(masterRoleInfo.IsValidExperienceHero(dataByKey.dwCfgID));
                                                                                            obj10.CustomSetActive(true);
                                                                                            obj11.CustomSetActive(true);
                                                                                            text3.set_text(Singleton<CTextManager>.GetInstance().GetText("Mall_Buy"));
                                                                                            script2.set_enabled(true);
                                                                                            button.set_enabled(true);
                                                                                            this.UpdateItemPricePnl(srcFormScript, obj10.get_transform(), obj6.get_transform(), productInfo);
                                                                                            stUIEventParams eventParams = new stUIEventParams();
                                                                                            eventParams.tag = uiEvent.m_srcWidgetIndexInBelongedList;
                                                                                            script2.SetUIEvent(enUIEventType.Click, enUIEventID.Mall_Mystery_On_Open_Buy_Form, eventParams);
                                                                                        }
                                                                                        break;
                                                                                    }
                                                                                    return;
                                                                                }
                                                                                case COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN:
                                                                                {
                                                                                    ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(dwItemID);
                                                                                    DebugHelper.Assert(heroSkin != null, "神秘商店配置的皮肤ID有错，皮肤表里不存在");
                                                                                    if (heroSkin != null)
                                                                                    {
                                                                                        ResHeroSkinShop shop2 = null;
                                                                                        GameDataMgr.skinShopInfoDict.TryGetValue(heroSkin.dwSkinID, out shop2);
                                                                                        ResHeroCfgInfo info3 = GameDataMgr.heroDatabin.GetDataByKey(heroSkin.dwHeroID);
                                                                                        DebugHelper.Assert(info3 != null, "神秘商店配置的皮肤ID有错，皮肤对应的英雄不存在");
                                                                                        if (info3 == null)
                                                                                        {
                                                                                            return;
                                                                                        }
                                                                                        CUICommonSystem.SetHeroItemImage(uiEvent.m_srcFormScript, obj3.get_gameObject(), heroSkin.szSkinPicID, enHeroHeadType.enBust, false, true);
                                                                                        obj7.CustomSetActive(false);
                                                                                        CUICommonSystem.SetHeroSkinLabelPic(uiEvent.m_srcFormScript, obj9, heroSkin.dwHeroID, heroSkin.dwSkinID);
                                                                                        obj5.CustomSetActive(true);
                                                                                        if (componetInChild != null)
                                                                                        {
                                                                                            componetInChild.set_text(StringHelper.UTF8BytesToString(ref info3.szName));
                                                                                        }
                                                                                        if (component != null)
                                                                                        {
                                                                                            component.set_text(StringHelper.UTF8BytesToString(ref heroSkin.szSkinName));
                                                                                        }
                                                                                        if (masterRoleInfo.IsHaveHeroSkin(heroSkin.dwHeroID, heroSkin.dwSkinID, false))
                                                                                        {
                                                                                            obj11.CustomSetActive(true);
                                                                                            text3.set_text(Singleton<CTextManager>.GetInstance().GetText("Mall_Skin_State_Own"));
                                                                                            obj6.CustomSetActive(false);
                                                                                            obj8.CustomSetActive(false);
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            obj8.CustomSetActive(masterRoleInfo.IsValidExperienceSkin(heroSkin.dwHeroID, heroSkin.dwSkinID));
                                                                                            obj10.CustomSetActive(true);
                                                                                            obj12.CustomSetActive(false);
                                                                                            this.UpdateItemPricePnl(srcFormScript, obj10.get_transform(), obj6.get_transform(), productInfo);
                                                                                            if (masterRoleInfo.IsCanBuySkinButNotHaveHero(heroSkin.dwHeroID, heroSkin.dwSkinID))
                                                                                            {
                                                                                                obj11.CustomSetActive(true);
                                                                                                script2.set_enabled(true);
                                                                                                text3.set_text(Singleton<CTextManager>.GetInstance().GetText("Mall_Skin_State_Buy_hero"));
                                                                                                button.set_enabled(true);
                                                                                                stUIEventParams params2 = new stUIEventParams();
                                                                                                params2.openHeroFormPar.heroId = heroSkin.dwHeroID;
                                                                                                params2.openHeroFormPar.skinId = heroSkin.dwSkinID;
                                                                                                params2.openHeroFormPar.openSrc = enHeroFormOpenSrc.SkinBuyClick;
                                                                                                script2.SetUIEvent(enUIEventType.Click, enUIEventID.HeroInfo_OpenForm, params2);
                                                                                                this.UpdateItemPricePnl(srcFormScript, obj10.get_transform(), obj6.get_transform(), productInfo);
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                obj12.CustomSetActive(false);
                                                                                                obj11.CustomSetActive(true);
                                                                                                text3.set_text(Singleton<CTextManager>.GetInstance().GetText("Mall_Buy"));
                                                                                                script2.set_enabled(true);
                                                                                                button.set_enabled(true);
                                                                                                stUIEventParams params3 = new stUIEventParams();
                                                                                                params3.tag = uiEvent.m_srcWidgetIndexInBelongedList;
                                                                                                script2.SetUIEvent(enUIEventType.Click, enUIEventID.Mall_Mystery_On_Open_Buy_Form, params3);
                                                                                            }
                                                                                        }
                                                                                        break;
                                                                                    }
                                                                                    return;
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void UpdateItemPricePnl(CUIFormScript form, Transform pricePnlTrans, Transform tagTrans, COMDT_AKALISHOP_GOODS productInfo)
        {
            GameObject obj2 = Utility.FindChild(pricePnlTrans.get_gameObject(), "oldPricePanel");
            Text componetInChild = Utility.GetComponetInChild<Text>(pricePnlTrans.get_gameObject(), "oldPricePanel/oldPriceText");
            Image image = Utility.GetComponetInChild<Image>(pricePnlTrans.get_gameObject(), "newPricePanel/costImage");
            Text text2 = Utility.GetComponetInChild<Text>(pricePnlTrans.get_gameObject(), "newPricePanel/newCostText");
            Text text3 = Utility.GetComponetInChild<Text>(tagTrans.get_gameObject(), "Text");
            tagTrans.get_gameObject().CustomSetActive(false);
            obj2.CustomSetActive(false);
            tagTrans.get_gameObject().CustomSetActive(true);
            float num = productInfo.dwItemDiscount / 10;
            if (Math.Abs((float) (num % 1f)) < float.Epsilon)
            {
                int num2 = (int) num;
                text3.set_text(string.Format("{0}折", num2.ToString("D")));
            }
            else
            {
                text3.set_text(string.Format("{0}折", num.ToString("0.0")));
            }
            obj2.CustomSetActive(true);
            componetInChild.set_text(productInfo.dwOrigPrice.ToString());
            text2.set_text(productInfo.dwRealPrice.ToString());
            image.SetSprite(CMallSystem.GetPayTypeIconPath(enPayType.DianQuan), form, true, false, false, false);
        }

        public void UpdateUI()
        {
            if ((Singleton<CMallSystem>.GetInstance().CurTab == CMallSystem.Tab.Mystery) && (this.m_AkaliShopDetail != null))
            {
                CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
                if (((mallForm != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen) && (Singleton<CMallSystem>.GetInstance().CurTab == CMallSystem.Tab.Mystery))
                {
                    Utility.FindChild(mallForm.get_gameObject(), "pnlBodyBg/pnlMystery").CustomSetActive(true);
                    GameObject obj2 = Utility.FindChild(mallForm.get_gameObject(), "pnlBodyBg/pnlMystery/Content/List");
                    GameObject obj3 = Utility.FindChild(mallForm.get_gameObject(), "pnlBodyBg/pnlMystery/Content/default");
                    if (!this.IsGetDisCount())
                    {
                        obj2.CustomSetActive(false);
                        obj3.CustomSetActive(true);
                    }
                    else
                    {
                        obj2.CustomSetActive(true);
                        obj3.CustomSetActive(false);
                        CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(mallForm.get_gameObject(), "pnlBodyBg/pnlMystery/Content/List");
                        if (componetInChild != null)
                        {
                            componetInChild.SetElementAmount(this.m_AkaliShopDetail.bGoodsCnt);
                        }
                    }
                    this.RefreshBanner();
                    this.RefreshTimer();
                }
            }
        }
    }
}

