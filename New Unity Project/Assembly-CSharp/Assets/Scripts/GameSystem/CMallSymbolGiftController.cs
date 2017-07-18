namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class CMallSymbolGiftController : Singleton<CMallSymbolGiftController>
    {
        [CompilerGenerated]
        private static CTimer.OnTimeUpHandler <>f__am$cache10;
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map3;
        public COMDT_REWARD_INFO[] astRewardList;
        public bool isSkipAni;
        public bool[] m_AdvSymbolFlag;
        public uint[] m_AdvSymbolIds;
        public bool[] m_AdvSymbolShowFlag;
        public byte m_CommonDrawSymbolCycle = 5;
        public byte m_CurrentRewardIdx;
        public bool[] m_ItemCellFlag;
        public byte m_SeniorDrawSymbolCycle = 5;
        public bool m_UIToggle;
        public const int MAX_SHOPDRAW_LIMIT = 0x3e8;
        public bool reqSent;
        public static int reqSentTimerSeq;
        public const string s_symbolGiftFormPath = "UGUI/Form/System/Mall/Form_Symbol_Gift.prefab";
        public static int[] SymbolGiftCommonDrawedCntInfo = new int[5];
        public static int[] SymbolGiftdrawCntLimit = new int[] { 0, 500, 500, 0x62, 0x31 };
        public static int[] SymbolGiftSeniorDrawedCntInfo = new int[5];

        public void Draw(CUIFormScript form)
        {
            this.InitElements(form);
            this.RefreshDesc(form);
            this.RefreshButtonView(form);
            this.ToggleActionMask(form, false, 30f);
            this.ToggleActionPanel(form, true);
            this.ToggleResMask(form, false);
            this.PlayLotteryAnimation(form, "Begin", true, false, false);
            Singleton<CSoundManager>.GetInstance().PostEvent("UI_buy_chou_jiguan", null);
            ShowLotteryResult(form, 0);
            Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.Mall_Sub_Module_Loaded);
        }

        private stPayInfo GetDrawPayInfo(RES_SHOPBUY_TYPE buyType, RES_SHOPDRAW_SUBTYPE subType)
        {
            stPayInfo info = new stPayInfo();
            ResShopInfo cfgShopInfo = CPurchaseSys.GetCfgShopInfo(buyType, (int) subType);
            DebugHelper.Assert(cfgShopInfo != null, string.Format("购买信息表，符文抽奖配置不对{0}:{1}", buyType, subType));
            if (cfgShopInfo != null)
            {
                info.m_payType = CMallSystem.ResBuyTypeToPayType(cfgShopInfo.bCoinType);
                info.m_payValue = cfgShopInfo.dwCoinPrice;
                info.m_oriValue = cfgShopInfo.dwCoinPrice;
                ResShopPromotion shopPromotion = CMallSystem.GetShopPromotion(buyType, subType);
                if (shopPromotion != null)
                {
                    info.m_payValue = shopPromotion.dwPrice;
                }
            }
            return info;
        }

        public static int GetTotalDrawedCnt(RES_SHOPBUY_TYPE drawType)
        {
            int num = 5;
            int num2 = 0;
            RES_SHOPBUY_TYPE res_shopbuy_type = drawType;
            if (res_shopbuy_type != RES_SHOPBUY_TYPE.RES_BUYTYPE_DRAWSYMBOLCOMMON)
            {
                if (res_shopbuy_type == RES_SHOPBUY_TYPE.RES_BUYTYPE_DRAWSYMBOLSENIOR)
                {
                    for (int j = 1; j < num; j++)
                    {
                        switch (((RES_SHOPDRAW_SUBTYPE) j))
                        {
                            case RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_FREE:
                            case RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_ONE:
                                num2 += SymbolGiftSeniorDrawedCntInfo[j];
                                break;

                            case RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_FIVE:
                                num2 += SymbolGiftSeniorDrawedCntInfo[j] * 5;
                                break;

                            case RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_TEN:
                                num2 += SymbolGiftSeniorDrawedCntInfo[j] * 10;
                                break;
                        }
                    }
                }
                return num2;
            }
            for (int i = 1; i < num; i++)
            {
                switch (((RES_SHOPDRAW_SUBTYPE) i))
                {
                    case RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_FREE:
                    case RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_ONE:
                        num2 += SymbolGiftCommonDrawedCntInfo[i];
                        break;

                    case RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_FIVE:
                        num2 += SymbolGiftCommonDrawedCntInfo[i] * 5;
                        break;

                    case RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_TEN:
                        num2 += SymbolGiftCommonDrawedCntInfo[i] * 10;
                        break;
                }
            }
            return num2;
        }

        private bool HasFreeDraw(COM_SHOP_DRAW_TYPE drawType)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo == null)
            {
                return false;
            }
            int index = (int) drawType;
            if ((index < 0) || (index > masterRoleInfo.m_freeDrawInfo.Length))
            {
                return false;
            }
            return (masterRoleInfo.m_freeDrawInfo[index].dwLeftFreeDrawCnt > 0);
        }

        public override void Init()
        {
            base.Init();
            this.m_ItemCellFlag = new bool[6];
            this.m_AdvSymbolFlag = new bool[6];
            this.m_AdvSymbolShowFlag = new bool[6];
            this.m_AdvSymbolIds = new uint[6];
            this.m_CurrentRewardIdx = 0;
            this.astRewardList = new COMDT_REWARD_INFO[5];
            this.reqSent = false;
            reqSentTimerSeq = -1;
            this.isSkipAni = false;
            CUIEventManager instance = Singleton<CUIEventManager>.GetInstance();
            instance.AddUIEventListener(enUIEventID.Lottery_Open_Form, new CUIEventManager.OnUIEventHandler(this.OnOpenSymbolGift));
            instance.AddUIEventListener(enUIEventID.Lottery_Close_Form, new CUIEventManager.OnUIEventHandler(this.OnCloseSymbolGift));
            instance.AddUIEventListener(enUIEventID.Lottery_Common_BuyOneSymbol, new CUIEventManager.OnUIEventHandler(this.OnLotteryCommonBuyOneSymbol));
            instance.AddUIEventListener(enUIEventID.Lottery_Common_BuyFiveSymbol, new CUIEventManager.OnUIEventHandler(this.OnLotteryCommonBuyFiveSymbol));
            instance.AddUIEventListener(enUIEventID.Lottery_Senior_BuyFreeSymbol, new CUIEventManager.OnUIEventHandler(this.OnLotterySeniorBuyFreeSymbol));
            instance.AddUIEventListener(enUIEventID.Lottery_Senior_BuyOneSymbol, new CUIEventManager.OnUIEventHandler(this.OnLotterySeniorBuyOneSymbol));
            instance.AddUIEventListener(enUIEventID.Lottery_Senior_BuyFiveSymbol, new CUIEventManager.OnUIEventHandler(this.OnLotterySeniorBuyFiveSymbol));
            instance.AddUIEventListener(enUIEventID.Lottery_BuySymbolConfirm, new CUIEventManager.OnUIEventHandler(this.OnLotteryBuySymbolConfirm));
            instance.AddUIEventListener(enUIEventID.Lottery_Show_Reward, new CUIEventManager.OnUIEventHandler(this.OnLotteryShowReward));
            instance.AddUIEventListener(enUIEventID.Lottery_Show_Reward_End, new CUIEventManager.OnUIEventHandler(this.OnLotteryShowRewardEnd));
            instance.AddUIEventListener(enUIEventID.Lottery_Show_Reward_Start, new CUIEventManager.OnUIEventHandler(this.OnLotteryShowRewardStart));
            instance.AddUIEventListener(enUIEventID.Lottery_Close_FX, new CUIEventManager.OnUIEventHandler(this.OnLotteryCloseFx));
            instance.AddUIEventListener(enUIEventID.Lottery_Symbol_Boom, new CUIEventManager.OnUIEventHandler(this.OnLotterySymbolBoom));
            instance.AddUIEventListener(enUIEventID.Lottery_Gold_Free_Draw_CD_UP, new CUIEventManager.OnUIEventHandler(this.OnGoldFreeDrawCdUp));
            instance.AddUIEventListener(enUIEventID.Lottery_Skip_Animation, new CUIEventManager.OnUIEventHandler(this.OnSkipAnimation));
            instance.AddUIEventListener(enUIEventID.Lottery_Action_Mask_Reset, new CUIEventManager.OnUIEventHandler(this.onActionMaskReset));
            Singleton<EventRouter>.instance.RemoveEventHandler<CSPkg>("ShopBuyDraw", new Action<CSPkg>(CMallSymbolGiftController.ReceiveLotteryRes));
            Singleton<EventRouter>.instance.AddEventHandler<CSPkg>("ShopBuyDraw", new Action<CSPkg>(CMallSymbolGiftController.ReceiveLotteryRes));
        }

        public void InitElements(CUIFormScript form)
        {
            if (form != null)
            {
                Singleton<CUIManager>.GetInstance().LoadUIScenePrefab(CUIUtility.s_lotterySceneBgPath, form);
                Utility.FindChild(form.get_gameObject(), "UIScene_Lottery").CustomSetActive(true);
                this.SetLotteryBool(form, "Server_Turn", false);
                this.SetLotteryBool(form, "Icon_Turn", false);
            }
        }

        public static bool IsAdvSymbol(COMDT_REWARD_INFO reward)
        {
            if (reward.bType == 6)
            {
                CSymbolItem item = CUseableManager.CreateUsableByServerType(COM_REWARDS_TYPE.COM_REWARDS_TYPE_SYMBOL, 1, reward.stRewardInfo.stSymbol.dwSymbolID) as CSymbolItem;
                if (item == null)
                {
                    return false;
                }
                if (item.m_grade >= 3)
                {
                    return true;
                }
            }
            return false;
        }

        private void onActionMaskReset(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Mall/Form_Symbol_Gift.prefab");
            if (form != null)
            {
                this.ToggleActionPanel(form, true);
            }
        }

        private void OnCloseSymbolGift(CUIEvent uiEvent)
        {
            Singleton<CSoundManager>.GetInstance().PostEvent("UI_buy_chou_jiguan_Stop", null);
        }

        private void OnGoldFreeDrawCdUp(CUIEvent uiEvent)
        {
            this.RefreshButtonView(uiEvent.m_srcFormScript);
        }

        private void OnLotteryBuySymbolConfirm(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Mall/Form_Symbol_Gift.prefab");
            if (form != null)
            {
                if (this.reqSent)
                {
                    Singleton<CUIManager>.GetInstance().OpenTips("请等待上一次抽奖结果服务器的返回", false, 1.5f, null, new object[0]);
                }
                else
                {
                    this.reqSent = true;
                    reqSentTimerSeq = Singleton<CTimerManager>.GetInstance().AddTimer(0x4e20, 1, delegate (int sequence) {
                        if (this.reqSent)
                        {
                            this.reqSent = false;
                        }
                    });
                    this.isSkipAni = false;
                    this.ToggleActionMask(form, true, 30f);
                    RES_SHOPBUY_TYPE tag = (RES_SHOPBUY_TYPE) uiEvent.m_eventParams.tag;
                    RES_SHOPDRAW_SUBTYPE lotterySubType = (RES_SHOPDRAW_SUBTYPE) uiEvent.m_eventParams.tag2;
                    this.ResetRewardItemCells();
                    this.SetLotteryBool(form, "Server_Turn", false);
                    this.SetLotteryBool(form, "Icon_Turn", false);
                    this.PlayLotteryAnimation(form, "Open", true, false, false);
                    this.ToggleActionPanel(form, false);
                    Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
                    this.SendLotteryMsg(tag, lotterySubType);
                }
            }
        }

        private void OnLotteryCloseFx(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Mall/Form_Symbol_Gift.prefab");
            if (form != null)
            {
                GameObject obj2 = Utility.FindChild(form.get_gameObject(), "UIScene_Lottery/Home_TransferGate");
                if (obj2 != null)
                {
                    CUIAnimatorScript component = obj2.GetComponent<CUIAnimatorScript>();
                    if (component != null)
                    {
                        component.m_eventIDs[1] = enUIEventID.None;
                    }
                }
                this.SetLotteryBool(form, "Icon_Turn", true);
                if (<>f__am$cache10 == null)
                {
                    <>f__am$cache10 = delegate (int sequence) {
                        Singleton<CSoundManager>.GetInstance().PostEvent("UI_buy_chou_close", null);
                    };
                }
                Singleton<CTimerManager>.GetInstance().AddTimer(0x3e8, 1, <>f__am$cache10);
                Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.Mall_Get_Product_OK);
                this.ToggleActionMask(form, false, 30f);
                this.ToggleActionPanel(form, true);
            }
        }

        private void OnLotteryCommonBuyFiveSymbol(CUIEvent uiEvent)
        {
            this.TryToPayForLotterySymbol(RES_SHOPBUY_TYPE.RES_BUYTYPE_DRAWSYMBOLCOMMON, RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_FIVE);
        }

        private void OnLotteryCommonBuyOneSymbol(CUIEvent uiEvent)
        {
            this.TryToPayForLotterySymbol(RES_SHOPBUY_TYPE.RES_BUYTYPE_DRAWSYMBOLCOMMON, RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_ONE);
        }

        private void OnLotterySeniorBuyFiveSymbol(CUIEvent uiEvent)
        {
            this.TryToPayForLotterySymbol(RES_SHOPBUY_TYPE.RES_BUYTYPE_DRAWSYMBOLSENIOR, RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_FIVE);
        }

        private void OnLotterySeniorBuyFreeSymbol(CUIEvent uiEvent)
        {
            this.TryToPayForLotterySymbol(RES_SHOPBUY_TYPE.RES_BUYTYPE_DRAWSYMBOLSENIOR, RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_FREE);
        }

        private void OnLotterySeniorBuyOneSymbol(CUIEvent uiEvent)
        {
            this.TryToPayForLotterySymbol(RES_SHOPBUY_TYPE.RES_BUYTYPE_DRAWSYMBOLSENIOR, RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_ONE);
        }

        private void OnLotteryShowReward(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Mall/Form_Symbol_Gift.prefab");
            if (form == null)
            {
                return;
            }
            GameObject obj2 = form.get_transform().Find("pnlBodyBg/pnlSymbolGift/showRewardTimer").get_gameObject();
            CUITimerScript component = null;
            if (obj2 != null)
            {
                component = obj2.GetComponent<CUITimerScript>();
            }
            if ((this.m_CurrentRewardIdx >= this.m_ItemCellFlag.Length) || (!this.m_ItemCellFlag[this.m_CurrentRewardIdx] && (this.m_CurrentRewardIdx > 0)))
            {
                this.ToggleSkipAnimationMask(form, false, 30f);
                if (component != null)
                {
                    component.SetTotalTime(2f);
                    component.SetOnChangedIntervalTime(100f);
                    component.ReStartTimer();
                }
                return;
            }
            GameObject obj3 = form.get_transform().Find(string.Format("pnlBodyBg/pnlSymbolGift/pnlResult/itemCell{0}", this.m_CurrentRewardIdx)).get_gameObject();
            if (obj3 != null)
            {
                if (this.m_ItemCellFlag[this.m_CurrentRewardIdx])
                {
                    Singleton<CSoundManager>.GetInstance().PostEvent("UI_buy_chou_fuwen_chuxian", null);
                    obj3.CustomSetActive(true);
                    this.StartTimer(obj3, 1.25f, 1f);
                }
                CUIAnimatorScript script3 = obj3.GetComponent<CUIAnimatorScript>();
                CUIEventScript componetInChild = Utility.GetComponetInChild<CUIEventScript>(obj3, string.Empty);
                if ((componetInChild != null) && (script3 != null))
                {
                    CSymbolItem iconUseable = componetInChild.m_onUpEventParams.iconUseable as CSymbolItem;
                    if (iconUseable != null)
                    {
                        int num = iconUseable.m_grade + 1;
                        switch (num)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                            case 5:
                                script3.SetInteger("Level", num);
                                goto Label_01BB;
                        }
                        script3.SetInteger("Level", 3);
                    }
                }
            }
        Label_01BB:
            this.m_CurrentRewardIdx = (byte) (this.m_CurrentRewardIdx + 1);
        }

        private void OnLotteryShowRewardEnd(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Mall/Form_Symbol_Gift.prefab");
            if (form != null)
            {
                CUseableContainer container = new CUseableContainer(enCONTAINER_TYPE.ITEM);
                for (int i = 0; i < this.m_AdvSymbolIds.Length; i++)
                {
                    if (this.m_AdvSymbolIds[i] != 0)
                    {
                        CUseable useable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL, this.m_AdvSymbolIds[i], 1);
                        container.Add(useable);
                    }
                }
                if (container.GetCurUseableCount() == 0)
                {
                    if (!this.isSkipAni)
                    {
                        Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Lottery_Close_FX);
                    }
                    else
                    {
                        Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.Mall_Get_Product_OK);
                        this.ToggleActionMask(form, false, 30f);
                        this.ToggleActionPanel(form, true);
                    }
                }
                else if (!this.isSkipAni)
                {
                    CUICommonSystem.ShowSymbol(container, enUIEventID.Lottery_Close_FX);
                }
                else
                {
                    CUICommonSystem.ShowSymbol(container, enUIEventID.Mall_Get_AWARD_CLOSE_FORM);
                    this.ToggleActionMask(form, false, 30f);
                    this.ToggleActionPanel(form, true);
                }
                ListView<CUseable> view = new ListView<CUseable>();
                for (int j = 0; j < this.m_ItemCellFlag.Length; j++)
                {
                    if (this.m_ItemCellFlag[j])
                    {
                        CUseable item = null;
                        if (j == 0)
                        {
                            item = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL, this.astRewardList[j].stRewardInfo.stSymbol.dwSymbolID, 1);
                        }
                        else
                        {
                            item = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL, this.astRewardList[j - 1].stRewardInfo.stSymbol.dwSymbolID, 1);
                        }
                        if (item != null)
                        {
                            view.Add(item);
                        }
                    }
                }
                int count = view.Count;
                if (count != 0)
                {
                    CUseable[] items = new CUseable[view.Count];
                    for (int k = 0; k < count; k++)
                    {
                        items[k] = view[k];
                    }
                    Singleton<CUIManager>.GetInstance().OpenAwardTip(items, Singleton<CTextManager>.GetInstance().GetText("gotAward"), false, enUIEventID.None, false, false, "Form_Award");
                }
            }
        }

        private void OnLotteryShowRewardStart(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Mall/Form_Symbol_Gift.prefab");
            if (form != null)
            {
                Singleton<CSoundManager>.GetInstance().PostEvent("UI_buy_chou_jiguan_menkai", null);
                if ((this.m_CurrentRewardIdx >= this.m_ItemCellFlag.Length) || (!this.m_ItemCellFlag[this.m_CurrentRewardIdx] && (this.m_CurrentRewardIdx > 0)))
                {
                    this.ToggleActionMask(form, false, 30f);
                    this.ToggleSkipAnimationMask(form, false, 30f);
                    this.ToggleResMask(form, false);
                    this.ToggleActionPanel(form, true);
                }
                else
                {
                    this.StartTimer(form.get_transform().Find(string.Format("pnlBodyBg/pnlSymbolGift/showRewardTimer", new object[0])).get_gameObject(), 1000f, 0.3f);
                }
            }
        }

        private void OnLotterySymbolBoom(CUIEvent uiEvent)
        {
            Singleton<CSoundManager>.GetInstance().PostEvent("UI_buy_chou_fuwen_bao", null);
        }

        private void OnMallTabChange()
        {
        }

        private void OnOpenSymbolGift(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/System/Mall/Form_Symbol_Gift.prefab", false, true);
            if (form != null)
            {
                this.Draw(form);
            }
        }

        private void OnSkipAnimation(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Mall/Form_Symbol_Gift.prefab");
            if (form != null)
            {
                this.isSkipAni = true;
                this.ToggleSkipAnimationMask(form, false, 30f);
                GameObject obj2 = Utility.FindChild(form.get_gameObject(), "UIScene_Lottery/Home_TransferGate");
                if (obj2 != null)
                {
                    obj2.GetComponent<CUIAnimatorScript>().m_eventIDs[1] = enUIEventID.None;
                    Singleton<CSoundManager>.GetInstance().PostEvent("Stop_erjijiemian", null);
                    this.PlayLotteryAnimation(form, "Close_1", true, false, false);
                    Singleton<CSoundManager>.GetInstance().PostEvent("UI_buy_chou_close", null);
                }
                this.StartTimer(form.get_transform().Find(string.Format("pnlBodyBg/pnlSymbolGift/showRewardTimer", new object[0])).get_gameObject(), 1000f, 0.04f);
            }
        }

        private void PlayLotteryAnimation(CUIFormScript form, string state, bool PlayAni = true, bool PlayFx = false, bool enableEndEvent = false)
        {
            string key = state;
            if (key != null)
            {
                int num;
                if (<>f__switch$map3 == null)
                {
                    Dictionary<string, int> dictionary = new Dictionary<string, int>(1);
                    dictionary.Add("Open", 0);
                    <>f__switch$map3 = dictionary;
                }
                if (<>f__switch$map3.TryGetValue(key, out num) && (num == 0))
                {
                    Singleton<CSoundManager>.GetInstance().PostEvent("Stop_UI_buy_chou_jiguan", null);
                    Singleton<CSoundManager>.GetInstance().PostEvent("UI_buy_chou_hero", null);
                }
            }
            GameObject target = Utility.FindChild(form.get_gameObject(), "UIScene_Lottery/Home_TransferGate");
            if (target != null)
            {
                CUIAnimatorScript component = target.GetComponent<CUIAnimatorScript>();
                if (component != null)
                {
                    component.Initialize(form);
                }
                if (enableEndEvent)
                {
                    if (component != null)
                    {
                        component.m_eventIDs[1] = enUIEventID.Lottery_Show_Reward_Start;
                    }
                }
                else if (component != null)
                {
                    component.m_eventIDs[1] = enUIEventID.None;
                }
                if ((component != null) && PlayAni)
                {
                    CUICommonSystem.PlayAnimator(target, state);
                }
            }
            GameObject obj3 = Utility.FindChild(form.get_gameObject(), "UIScene_Lottery/FX");
            if (obj3 != null)
            {
                CUIAnimatorScript script2 = obj3.GetComponent<CUIAnimatorScript>();
                if (script2 != null)
                {
                    script2.Initialize(form);
                    if (PlayFx)
                    {
                        CUICommonSystem.PlayAnimator(obj3, state);
                    }
                }
            }
        }

        public static void ReceiveLotteryRes(CSPkg msg)
        {
            Singleton<CMallSymbolGiftController>.GetInstance().reqSent = false;
            Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref reqSentTimerSeq);
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Mall/Form_Symbol_Gift.prefab");
            if (form != null)
            {
                CMallSymbolGiftController instance = Singleton<CMallSymbolGiftController>.GetInstance();
                SCPKG_CMD_SHOPBUY stShopBuyRsp = msg.stPkgData.stShopBuyRsp;
                RES_SHOPBUY_TYPE iBuyType = (RES_SHOPBUY_TYPE) stShopBuyRsp.iBuyType;
                RES_SHOPDRAW_SUBTYPE iBuySubType = (RES_SHOPDRAW_SUBTYPE) stShopBuyRsp.iBuySubType;
                switch (iBuyType)
                {
                    case RES_SHOPBUY_TYPE.RES_BUYTYPE_DRAWSYMBOLCOMMON:
                        if (iBuySubType == RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_FREE)
                        {
                            ResetFreeDrawCD(COM_SHOP_DRAW_TYPE.COM_SHOPDRAW_SYMBOLCOMMON);
                            Singleton<EventRouter>.GetInstance().BroadCastEvent<CMallSystem.Tab>(EventID.Mall_Refresh_Tab_Red_Dot, CMallSystem.Tab.Symbol_Make);
                            Singleton<CMallSymbolGiftController>.GetInstance().RefreshButtonView(form);
                        }
                        if ((iBuySubType >= RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_FREE) && (iBuySubType < RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_MAX))
                        {
                            SymbolGiftCommonDrawedCntInfo[(int) iBuySubType]++;
                            if (GetTotalDrawedCnt(RES_SHOPBUY_TYPE.RES_BUYTYPE_DRAWSYMBOLCOMMON) >= 0x3e8)
                            {
                                ResetDrawedCnt(RES_SHOPBUY_TYPE.RES_BUYTYPE_DRAWSYMBOLCOMMON);
                            }
                        }
                        break;

                    case RES_SHOPBUY_TYPE.RES_BUYTYPE_DRAWSYMBOLSENIOR:
                        if (iBuySubType == RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_FREE)
                        {
                            ResetFreeDrawCD(COM_SHOP_DRAW_TYPE.COM_SHOPDRAW_SYMBOLSENIOR);
                            Singleton<EventRouter>.GetInstance().BroadCastEvent<CMallSystem.Tab>(EventID.Mall_Refresh_Tab_Red_Dot, CMallSystem.Tab.Symbol_Make);
                            Singleton<CMallSymbolGiftController>.GetInstance().RefreshButtonView(form);
                        }
                        if ((iBuySubType >= RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_FREE) && (iBuySubType < RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_MAX))
                        {
                            SymbolGiftSeniorDrawedCntInfo[(int) iBuySubType]++;
                            if (GetTotalDrawedCnt(RES_SHOPBUY_TYPE.RES_BUYTYPE_DRAWSYMBOLSENIOR) >= 0x3e8)
                            {
                                ResetDrawedCnt(RES_SHOPBUY_TYPE.RES_BUYTYPE_DRAWSYMBOLSENIOR);
                            }
                        }
                        break;
                }
                Singleton<CMallSymbolGiftController>.GetInstance().RefreshDesc(form);
                byte index = 0;
                if (stShopBuyRsp.stBuyResult.bRewardCnt > 0)
                {
                    COMDT_REWARD_DETAIL comdt_reward_detail = null;
                    for (int i = 0; i < stShopBuyRsp.stBuyResult.bRewardCnt; i++)
                    {
                        comdt_reward_detail = stShopBuyRsp.stBuyResult.astRewardInfo[i];
                        if (comdt_reward_detail.bNum > 0)
                        {
                            instance.astRewardList[index] = comdt_reward_detail.astRewardDetail[comdt_reward_detail.bNum - 1];
                            index = (byte) (index + 1);
                        }
                    }
                }
                if (index > 0)
                {
                    Singleton<CMallSymbolGiftController>.GetInstance().ToggleSkipAnimationMask(form, true, 30f);
                    ShowLotteryResult(form, index);
                    Singleton<CMallSymbolGiftController>.GetInstance().SetLotteryBool(form, "Server_Turn", true);
                    GameObject obj2 = Utility.FindChild(form.get_gameObject(), "UIScene_Lottery/Home_TransferGate");
                    if (obj2 != null)
                    {
                        CUIAnimatorScript component = obj2.GetComponent<CUIAnimatorScript>();
                        if ((component != null) && (component.m_currentAnimatorStateName != null))
                        {
                            component.m_eventIDs[1] = enUIEventID.Lottery_Show_Reward_Start;
                        }
                        else
                        {
                            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Lottery_Show_Reward_Start);
                        }
                    }
                }
                else
                {
                    Singleton<CMallSymbolGiftController>.GetInstance().ResetRewardItemCells();
                    Singleton<CMallSymbolGiftController>.GetInstance().ToggleActionMask(form, false, 30f);
                    Singleton<CMallSymbolGiftController>.GetInstance().ToggleResMask(form, false);
                }
                Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.LOTTERY_GET_NEW_SYMBOL);
                Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            }
        }

        private void RefreshButtonView(CUIFormScript form)
        {
            if (form != null)
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if (masterRoleInfo != null)
                {
                    GameObject obj2 = form.get_transform().Find("pnlBodyBg/pnlSymbolGift/pnlAction/pnlCommonBuy/btnBuyOne/BuyButton").get_gameObject();
                    GameObject obj3 = form.get_transform().Find("pnlBodyBg/pnlSymbolGift/pnlAction/pnlCommonBuy/btnBuyFive/BuyButton").get_gameObject();
                    GameObject obj4 = form.get_transform().Find("pnlBodyBg/pnlSymbolGift/pnlAction/pnlSeniorBuy/btnBuyOne").get_gameObject();
                    GameObject obj5 = form.get_transform().Find("pnlBodyBg/pnlSymbolGift/pnlAction/pnlSeniorBuy/btnBuyOneFree").get_gameObject();
                    GameObject obj6 = form.get_transform().Find("pnlBodyBg/pnlSymbolGift/pnlAction/pnlSeniorBuy/btnBuyOne/BuyButton").get_gameObject();
                    GameObject obj7 = form.get_transform().Find("pnlBodyBg/pnlSymbolGift/pnlAction/pnlSeniorBuy/btnBuyFive/BuyButton").get_gameObject();
                    GameObject obj8 = form.get_transform().Find("pnlBodyBg/pnlSymbolGift/pnlAction/pnlSeniorBuy/pnlCd").get_gameObject();
                    GameObject obj9 = form.get_transform().Find("pnlBodyBg/pnlSymbolGift/pnlAction/pnlSeniorBuy/btnBuyOneFree/BuyButton").get_gameObject();
                    CUITimerScript componetInChild = Utility.GetComponetInChild<CUITimerScript>(form.get_gameObject(), "pnlBodyBg/pnlSymbolGift/pnlAction/pnlSeniorBuy/pnlCd/Timer");
                    stUIEventParams eventParams = new stUIEventParams();
                    if (obj2 != null)
                    {
                        stPayInfo drawPayInfo = this.GetDrawPayInfo(RES_SHOPBUY_TYPE.RES_BUYTYPE_DRAWSYMBOLCOMMON, RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_ONE);
                        CMallSystem.SetPayButton(form, obj2.get_transform() as RectTransform, drawPayInfo.m_payType, drawPayInfo.m_payValue, drawPayInfo.m_oriValue, enUIEventID.Lottery_Common_BuyOneSymbol, ref eventParams);
                    }
                    if (obj3 != null)
                    {
                        stPayInfo info3 = this.GetDrawPayInfo(RES_SHOPBUY_TYPE.RES_BUYTYPE_DRAWSYMBOLCOMMON, RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_FIVE);
                        CMallSystem.SetPayButton(form, obj3.get_transform() as RectTransform, info3.m_payType, info3.m_payValue, info3.m_oriValue, enUIEventID.Lottery_Common_BuyFiveSymbol, ref eventParams);
                    }
                    if (this.HasFreeDraw(COM_SHOP_DRAW_TYPE.COM_SHOPDRAW_SYMBOLSENIOR))
                    {
                        obj5.CustomSetActive(true);
                        obj4.CustomSetActive(false);
                        obj8.CustomSetActive(false);
                        if (obj9 != null)
                        {
                            stPayInfo info4 = this.GetDrawPayInfo(RES_SHOPBUY_TYPE.RES_BUYTYPE_DRAWSYMBOLSENIOR, RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_FREE);
                            CMallSystem.SetPayButton(form, obj9.get_transform() as RectTransform, info4.m_payType, info4.m_payValue, info4.m_oriValue, enUIEventID.Lottery_Senior_BuyFreeSymbol, ref eventParams);
                            Transform transform = obj9.get_transform().FindChild("PriceContainer/Price");
                            if (transform != null)
                            {
                                CUICommonSystem.AddRedDot(transform.get_gameObject(), enRedDotPos.enTopRight, 0);
                            }
                        }
                    }
                    else
                    {
                        obj5.CustomSetActive(false);
                        obj4.CustomSetActive(true);
                        obj8.CustomSetActive(true);
                        int num = Math.Max(0, masterRoleInfo.m_freeDrawInfo[4].dwLeftFreeDrawCD - CRoleInfo.GetCurrentUTCTime());
                        if (componetInChild != null)
                        {
                            componetInChild.m_eventIDs[1] = enUIEventID.None;
                            componetInChild.EndTimer();
                            componetInChild.m_eventIDs[1] = enUIEventID.Lottery_Gold_Free_Draw_CD_UP;
                            componetInChild.SetTotalTime((float) num);
                            componetInChild.StartTimer();
                        }
                        if (obj6 != null)
                        {
                            stPayInfo info5 = this.GetDrawPayInfo(RES_SHOPBUY_TYPE.RES_BUYTYPE_DRAWSYMBOLSENIOR, RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_ONE);
                            CMallSystem.SetPayButton(form, obj6.get_transform() as RectTransform, info5.m_payType, info5.m_payValue, info5.m_oriValue, enUIEventID.Lottery_Senior_BuyOneSymbol, ref eventParams);
                        }
                    }
                    if (obj7 != null)
                    {
                        stPayInfo info6 = this.GetDrawPayInfo(RES_SHOPBUY_TYPE.RES_BUYTYPE_DRAWSYMBOLSENIOR, RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_FIVE);
                        CMallSystem.SetPayButton(form, obj7.get_transform() as RectTransform, info6.m_payType, info6.m_payValue, info6.m_oriValue, enUIEventID.Lottery_Senior_BuyFiveSymbol, ref eventParams);
                    }
                }
            }
        }

        private void RefreshDesc(CUIFormScript form)
        {
            if (form != null)
            {
                Text componetInChild = Utility.GetComponetInChild<Text>(form.get_gameObject(), "pnlBodyBg/pnlSymbolGift/pnlAction/pnlCommonBuy/txtDes");
                Text text2 = Utility.GetComponetInChild<Text>(form.get_gameObject(), "pnlBodyBg/pnlSymbolGift/pnlAction/pnlSeniorBuy/txtDes");
                if (componetInChild != null)
                {
                    componetInChild.set_text(string.Format(Singleton<CTextManager>.GetInstance().GetText("Mall_Symbol_Gifts_Common_Buy_Dsc"), this.m_CommonDrawSymbolCycle - (GetTotalDrawedCnt(RES_SHOPBUY_TYPE.RES_BUYTYPE_DRAWSYMBOLCOMMON) % this.m_CommonDrawSymbolCycle)));
                }
                if (text2 != null)
                {
                    text2.set_text(string.Format(Singleton<CTextManager>.GetInstance().GetText("Mall_Symbol_Gifts_Senior_Buy_Dsc"), this.m_SeniorDrawSymbolCycle - (GetTotalDrawedCnt(RES_SHOPBUY_TYPE.RES_BUYTYPE_DRAWSYMBOLSENIOR) % this.m_SeniorDrawSymbolCycle)));
                }
            }
        }

        public static void ResetDrawedCnt(RES_SHOPBUY_TYPE drawType)
        {
            switch (drawType)
            {
                case RES_SHOPBUY_TYPE.RES_BUYTYPE_DRAWSYMBOLCOMMON:
                {
                    int num = 5;
                    for (int i = 1; i < num; i++)
                    {
                        if (SymbolGiftCommonDrawedCntInfo[i] > SymbolGiftdrawCntLimit[i])
                        {
                            SymbolGiftCommonDrawedCntInfo[i] = SymbolGiftdrawCntLimit[i];
                        }
                    }
                    break;
                }
                case RES_SHOPBUY_TYPE.RES_BUYTYPE_DRAWSYMBOLSENIOR:
                {
                    int num3 = 5;
                    for (int j = 1; j < num3; j++)
                    {
                        if (SymbolGiftSeniorDrawedCntInfo[j] > SymbolGiftdrawCntLimit[j])
                        {
                            SymbolGiftSeniorDrawedCntInfo[j] = SymbolGiftdrawCntLimit[j];
                        }
                    }
                    return;
                }
            }
        }

        public static void ResetFreeDrawCD(COM_SHOP_DRAW_TYPE drawType)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                int index = (int) drawType;
                if ((index >= 0) && (index <= masterRoleInfo.m_freeDrawInfo.Length))
                {
                    masterRoleInfo.m_freeDrawInfo[index].dwLeftFreeDrawCnt = 0;
                    int dwConfValue = 0;
                    switch (drawType)
                    {
                        case COM_SHOP_DRAW_TYPE.COM_SHOPDRAW_SYMBOLCOMMON:
                            dwConfValue = (int) GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x7a).dwConfValue;
                            break;

                        case COM_SHOP_DRAW_TYPE.COM_SHOPDRAW_SYMBOLSENIOR:
                            dwConfValue = (int) GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x7b).dwConfValue;
                            break;
                    }
                    masterRoleInfo.m_freeDrawInfo[index].dwLeftFreeDrawCD = CRoleInfo.GetCurrentUTCTime() + dwConfValue;
                    Singleton<EventRouter>.instance.BroadCastEvent(EventID.Mall_Set_Free_Draw_Timer);
                }
            }
        }

        private void ResetRewardItemCells()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Mall/Form_Symbol_Gift.prefab");
            if (form != null)
            {
                this.m_CurrentRewardIdx = 0;
                for (byte i = 0; i < this.m_ItemCellFlag.Length; i = (byte) (i + 1))
                {
                    this.m_ItemCellFlag[i] = false;
                    GameObject target = Utility.FindChild(form.get_gameObject(), string.Format("pnlBodyBg/pnlSymbolGift/pnlResult/itemCell{0}", i));
                    CUICommonSystem.PlayAnimator(target, "End");
                    target.CustomSetActive(false);
                    this.m_AdvSymbolFlag[i] = false;
                    this.m_AdvSymbolShowFlag[i] = true;
                    this.m_AdvSymbolIds[i] = 0;
                }
            }
        }

        private void SendLotteryMsg(RES_SHOPBUY_TYPE LotteryType, RES_SHOPDRAW_SUBTYPE LotterySubType)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x459);
            CSPKG_CMD_SHOPBUY cspkg_cmd_shopbuy = new CSPKG_CMD_SHOPBUY();
            cspkg_cmd_shopbuy.iBuyType = (int) LotteryType;
            cspkg_cmd_shopbuy.iBuySubType = (int) LotterySubType;
            msg.stPkgData.stShopBuyReq = cspkg_cmd_shopbuy;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        private void SetLotteryBool(CUIFormScript form, string name, bool value)
        {
            if ((form != null) && form.get_gameObject().get_activeSelf())
            {
                GameObject obj2 = Utility.FindChild(form.get_gameObject(), "UIScene_Lottery/Home_TransferGate");
                if (obj2 != null)
                {
                    CUIAnimatorScript component = obj2.GetComponent<CUIAnimatorScript>();
                    if (component != null)
                    {
                        component.Initialize(form);
                        component.SetBool(name, value);
                    }
                }
            }
        }

        private static void SetRewardItemCell(CUIFormScript formScript, byte itemCellIndex, COMDT_REWARD_INFO stRewardInfo)
        {
            CUseable itemUseable = null;
            GameObject itemCell = formScript.get_transform().Find(string.Format("pnlBodyBg/pnlSymbolGift/pnlResult/itemCell{0}", itemCellIndex)).get_gameObject();
            if (itemCell != null)
            {
                switch (stRewardInfo.bType)
                {
                    case 1:
                        itemUseable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, stRewardInfo.stRewardInfo.stItem.dwItemID, (int) stRewardInfo.stRewardInfo.stItem.dwCnt);
                        break;

                    case 2:
                        itemUseable = CUseableManager.CreateVirtualUseable(enVirtualItemType.enExp, stRewardInfo.stRewardInfo.stExp.bReserve);
                        break;

                    case 4:
                        itemUseable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMEQUIP, stRewardInfo.stRewardInfo.stEquip.dwEquipID, (int) stRewardInfo.stRewardInfo.stEquip.dwCnt);
                        break;

                    case 5:
                        itemUseable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_HERO, stRewardInfo.stRewardInfo.stHero.dwHeroID, (int) stRewardInfo.stRewardInfo.stHero.dwCnt);
                        break;

                    case 6:
                        itemUseable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL, stRewardInfo.stRewardInfo.stSymbol.dwSymbolID, 0);
                        break;

                    case 10:
                        itemUseable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN, stRewardInfo.stRewardInfo.stSkin.dwSkinID, (int) stRewardInfo.stRewardInfo.stSkin.dwCnt);
                        break;

                    case 11:
                        itemUseable = CUseableManager.CreateVirtualUseable(enVirtualItemType.enGoldCoin, (int) stRewardInfo.stRewardInfo.dwPvpCoin);
                        break;

                    case 13:
                        itemUseable = CUseableManager.CreateVirtualUseable(enVirtualItemType.enSkinCoin, (int) stRewardInfo.stRewardInfo.dwSkinCoin);
                        break;

                    case 14:
                        itemUseable = CUseableManager.CreateVirtualUseable(enVirtualItemType.enSymbolCoin, (int) stRewardInfo.stRewardInfo.dwSymbolCoin);
                        break;

                    case 0x10:
                        itemUseable = CUseableManager.CreateVirtualUseable(enVirtualItemType.enDianQuan, (int) stRewardInfo.stRewardInfo.dwDiamond);
                        break;

                    case 20:
                        itemUseable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_HEADIMG, stRewardInfo.stRewardInfo.stHeadImage.dwHeadImgID, (int) stRewardInfo.stRewardInfo.stHeadImage.dwGetTime);
                        break;
                }
                CUICommonSystem.SetItemCell(formScript, itemCell, itemUseable, true, false, false, false);
            }
        }

        private static void ShowLotteryResult(CUIFormScript form, byte bRewardCnt)
        {
            if (form != null)
            {
                CMallSymbolGiftController instance = Singleton<CMallSymbolGiftController>.GetInstance();
                instance.ResetRewardItemCells();
                Singleton<CMallSymbolGiftController>.GetInstance().ToggleResMask(form, true);
                byte num6 = bRewardCnt;
                if (num6 != 0)
                {
                    if (num6 != 1)
                    {
                        Random random = new Random();
                        int length = instance.astRewardList.Length;
                        while (length > 1)
                        {
                            length--;
                            int index = random.Next(length + 1);
                            COMDT_REWARD_INFO comdt_reward_info = instance.astRewardList[index];
                            instance.astRewardList[index] = instance.astRewardList[length];
                            instance.astRewardList[length] = comdt_reward_info;
                        }
                        instance.m_ItemCellFlag[0] = false;
                        instance.m_AdvSymbolFlag[0] = false;
                        instance.m_AdvSymbolShowFlag[0] = true;
                        instance.m_AdvSymbolIds[0] = 0;
                        for (byte i = 1; i < instance.m_ItemCellFlag.Length; i = (byte) (i + 1))
                        {
                            instance.m_ItemCellFlag[i] = true;
                            if (IsAdvSymbol(instance.astRewardList[i - 1]))
                            {
                                instance.m_AdvSymbolFlag[i] = true;
                                instance.m_AdvSymbolShowFlag[i] = false;
                                instance.m_AdvSymbolIds[i] = instance.astRewardList[i - 1].stRewardInfo.stSymbol.dwSymbolID;
                            }
                            else
                            {
                                instance.m_AdvSymbolFlag[i] = false;
                                instance.m_AdvSymbolShowFlag[i] = true;
                                instance.m_AdvSymbolIds[i] = 0;
                            }
                            if (instance.astRewardList[i - 1] != null)
                            {
                                SetRewardItemCell(form, i, instance.astRewardList[i - 1]);
                            }
                            else
                            {
                                instance.m_ItemCellFlag[i] = false;
                            }
                        }
                    }
                    else
                    {
                        instance.m_ItemCellFlag[0] = true;
                        if (IsAdvSymbol(instance.astRewardList[0]))
                        {
                            instance.m_AdvSymbolFlag[0] = true;
                            instance.m_AdvSymbolShowFlag[0] = false;
                            instance.m_AdvSymbolIds[0] = instance.astRewardList[0].stRewardInfo.stSymbol.dwSymbolID;
                        }
                        SetRewardItemCell(form, 0, instance.astRewardList[0]);
                        for (byte j = 1; j < instance.m_ItemCellFlag.Length; j = (byte) (j + 1))
                        {
                            instance.m_ItemCellFlag[j] = false;
                        }
                    }
                }
                else
                {
                    for (byte k = 0; k < instance.m_ItemCellFlag.Length; k = (byte) (k + 1))
                    {
                        instance.m_ItemCellFlag[k] = false;
                    }
                    Singleton<CMallSymbolGiftController>.GetInstance().ToggleActionMask(form, false, 30f);
                    Singleton<CMallSymbolGiftController>.GetInstance().ToggleResMask(form, false);
                }
            }
        }

        private void StartTimer(GameObject gameObject, float total = 0, float interval = 0f)
        {
            if (gameObject != null)
            {
                CUITimerScript component = gameObject.GetComponent<CUITimerScript>();
                if (component != null)
                {
                    if (total != 0f)
                    {
                        component.SetTotalTime(total);
                    }
                    if (interval > 0f)
                    {
                        component.SetOnChangedIntervalTime(interval);
                    }
                    component.StartTimer();
                }
            }
        }

        public void ToggleActionMask(CUIFormScript form, bool active, float totalTime = 30)
        {
            if (form != null)
            {
                GameObject widget = form.GetWidget(0);
                GameObject obj3 = form.GetWidget(4);
                CUITimerScript component = null;
                if (obj3 != null)
                {
                    component = obj3.GetComponent<CUITimerScript>();
                }
                if (widget != null)
                {
                    widget.CustomSetActive(active);
                    if (component != null)
                    {
                        if (active)
                        {
                            component.SetTotalTime(totalTime);
                            component.StartTimer();
                        }
                        else
                        {
                            component.EndTimer();
                        }
                    }
                }
            }
        }

        public void ToggleActionPanel(CUIFormScript form, bool active)
        {
            if (form != null)
            {
                GameObject target = Utility.FindChild(form.get_gameObject(), "pnlBodyBg/pnlSymbolGift/pnlAction");
                if (target != null)
                {
                    if (active)
                    {
                        CanvasGroup component = target.GetComponent<CanvasGroup>();
                        if ((component != null) && (component.get_alpha() < 1f))
                        {
                            CUICommonSystem.PlayAnimator(target, "Button_Up");
                        }
                    }
                    else
                    {
                        CUICommonSystem.PlayAnimator(target, "Button_Down");
                    }
                }
            }
        }

        public void ToggleResMask(CUIFormScript form, bool active)
        {
            if (form != null)
            {
                GameObject widget = form.GetWidget(3);
                if (widget != null)
                {
                    widget.CustomSetActive(active);
                }
            }
        }

        public void ToggleSkipAnimationMask(CUIFormScript form, bool active, float totalTime = 30)
        {
            if (form != null)
            {
                GameObject widget = form.GetWidget(5);
                GameObject obj3 = form.GetWidget(6);
                CUITimerScript component = null;
                if (obj3 != null)
                {
                    component = obj3.GetComponent<CUITimerScript>();
                }
                if (widget != null)
                {
                    widget.CustomSetActive(active);
                    if (component != null)
                    {
                        if (active)
                        {
                            component.SetTotalTime(totalTime);
                            component.StartTimer();
                        }
                        else
                        {
                            component.EndTimer();
                        }
                    }
                }
            }
        }

        private void TryToPayForLotterySymbol(RES_SHOPBUY_TYPE lotteryType, RES_SHOPDRAW_SUBTYPE lotterySubType)
        {
            if (Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Mall/Form_Symbol_Gift.prefab") != null)
            {
                if (Singleton<CMatchingSystem>.GetInstance().IsInMatching)
                {
                    Singleton<CUIManager>.GetInstance().OpenTips("PVP_Matching", true, 1.5f, null, new object[0]);
                }
                else
                {
                    stPayInfo drawPayInfo = this.GetDrawPayInfo(lotteryType, lotterySubType);
                    CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
                    uIEvent.m_eventID = enUIEventID.Lottery_BuySymbolConfirm;
                    uIEvent.m_eventParams.tag = (int) lotteryType;
                    uIEvent.m_eventParams.tag2 = (int) lotterySubType;
                    CMallSystem.TryToPay(enPayPurpose.Lottery, string.Empty, drawPayInfo.m_payType, drawPayInfo.m_payValue, uIEvent.m_eventID, ref uIEvent.m_eventParams, enUIEventID.None, false, true, false);
                }
            }
        }

        public override void UnInit()
        {
            base.UnInit();
            CUIEventManager instance = Singleton<CUIEventManager>.GetInstance();
            instance.RemoveUIEventListener(enUIEventID.Lottery_Open_Form, new CUIEventManager.OnUIEventHandler(this.OnOpenSymbolGift));
            instance.RemoveUIEventListener(enUIEventID.Lottery_Close_Form, new CUIEventManager.OnUIEventHandler(this.OnCloseSymbolGift));
            instance.RemoveUIEventListener(enUIEventID.Lottery_Common_BuyOneSymbol, new CUIEventManager.OnUIEventHandler(this.OnLotteryCommonBuyOneSymbol));
            instance.RemoveUIEventListener(enUIEventID.Lottery_Common_BuyFiveSymbol, new CUIEventManager.OnUIEventHandler(this.OnLotteryCommonBuyFiveSymbol));
            instance.RemoveUIEventListener(enUIEventID.Lottery_Senior_BuyFreeSymbol, new CUIEventManager.OnUIEventHandler(this.OnLotterySeniorBuyFreeSymbol));
            instance.RemoveUIEventListener(enUIEventID.Lottery_Senior_BuyOneSymbol, new CUIEventManager.OnUIEventHandler(this.OnLotterySeniorBuyOneSymbol));
            instance.RemoveUIEventListener(enUIEventID.Lottery_Senior_BuyFiveSymbol, new CUIEventManager.OnUIEventHandler(this.OnLotterySeniorBuyFiveSymbol));
            instance.RemoveUIEventListener(enUIEventID.Lottery_BuySymbolConfirm, new CUIEventManager.OnUIEventHandler(this.OnLotteryBuySymbolConfirm));
            instance.RemoveUIEventListener(enUIEventID.Lottery_Show_Reward, new CUIEventManager.OnUIEventHandler(this.OnLotteryShowReward));
            instance.RemoveUIEventListener(enUIEventID.Lottery_Show_Reward_End, new CUIEventManager.OnUIEventHandler(this.OnLotteryShowRewardEnd));
            instance.RemoveUIEventListener(enUIEventID.Lottery_Show_Reward_Start, new CUIEventManager.OnUIEventHandler(this.OnLotteryShowRewardStart));
            instance.RemoveUIEventListener(enUIEventID.Lottery_Close_FX, new CUIEventManager.OnUIEventHandler(this.OnLotteryCloseFx));
            instance.RemoveUIEventListener(enUIEventID.Lottery_Symbol_Boom, new CUIEventManager.OnUIEventHandler(this.OnLotterySymbolBoom));
            instance.RemoveUIEventListener(enUIEventID.Lottery_Gold_Free_Draw_CD_UP, new CUIEventManager.OnUIEventHandler(this.OnGoldFreeDrawCdUp));
            instance.RemoveUIEventListener(enUIEventID.Lottery_Skip_Animation, new CUIEventManager.OnUIEventHandler(this.OnSkipAnimation));
            instance.RemoveUIEventListener(enUIEventID.Lottery_Action_Mask_Reset, new CUIEventManager.OnUIEventHandler(this.onActionMaskReset));
            Singleton<EventRouter>.instance.RemoveEventHandler<CSPkg>("ShopBuyDraw", new Action<CSPkg>(CMallSymbolGiftController.ReceiveLotteryRes));
        }

        public enum enSymbolGiftFormWidget
        {
            Action_Mask,
            Top_Common,
            Body_Bg,
            Res_Mask,
            Action_Mask_Reset_Timer,
            Skip_Mask,
            Skip_Mask_Reset_Timer
        }
    }
}

