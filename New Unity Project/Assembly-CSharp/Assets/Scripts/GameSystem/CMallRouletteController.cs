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

    [MessageHandlerClass]
    public class CMallRouletteController : Singleton<CMallRouletteController>
    {
        [CompilerGenerated]
        private static Comparison<ResDT_LuckyDrawExternReward> <>f__am$cache19;
        [CompilerGenerated]
        private static Comparison<ResRareExchange> <>f__am$cache1A;
        [CompilerGenerated]
        private static Comparison<ResRareExchange> <>f__am$cache1B;
        [CompilerGenerated]
        private static Comparison<ResRareExchange> <>f__am$cache1C;
        public const byte ACCELERATE_STEPS = 4;
        public const byte CONTINUOUS_DRAW_MIN_STEPS = 1;
        public const byte DECELERATE_STEPS = 4;
        public const float FASTEST_SPEED = 0.03f;
        public const int LEAST_LOOPS = 2;
        private DictionaryView<uint, ListView<ResRareExchange>> m_CrySatlDic;
        private int[] m_CrystalItemID = new int[3];
        private byte m_CurContinousDrawSteps;
        private int m_CurLoops;
        private int m_CurRewardIdx;
        private int m_CurSpinCnt;
        private int m_CurSpinIdx;
        private Roulette_State m_CurState;
        private Tab m_CurTab;
        private DictionaryView<uint, ListView<ResDT_LuckyDrawExternReward>> m_ExternRewardDic;
        private bool m_GotAllUnusualItems;
        private bool m_IsClockwise;
        private bool m_IsContinousDraw;
        private bool m_IsLuckyBarInited;
        private SCPKG_LUCKYDRAW_RSP m_LuckyDrawRsp;
        private uint m_nCurCryStalTabID;
        private int m_nTotalCrySalTab;
        private DictionaryView<uint, ListView<ResLuckyDrawRewardForClient>> m_RewardDic;
        private ListView<CUseable> m_RewardList;
        public Dictionary<uint, uint> m_RewardPoolDic;
        private string m_tab0ImagePath = (CUIUtility.s_Sprite_Dynamic_Icon_Dir + "13020.prefab");
        private string m_tab1ImagePath = (CUIUtility.s_Sprite_Dynamic_Icon_Dir + "13021.prefab");
        private List<Tab> m_UsedTabs;
        public const int MAX_EXTERN_REWARD_LIST_CNT = 5;
        public const int MAX_LUCK_CNT = 200;
        public const float NORMAL_SPEED = 0.03f;
        public static int reqSentTimerSeq = -1;
        public const ushort ROULETTE_RULE_ID = 2;
        public const float SLOWEST_SPEED = 0.1f;
        public static string sMallFormCrystal = "UGUI/Form/System/Mall/Form_Crystal.prefab";

        public void DisplayTmpRewardList(bool isShow, int amount = 0)
        {
            CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
            if ((mallForm != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
            {
                GameObject p = Utility.FindChild(mallForm.get_gameObject(), "pnlBodyBg/pnlRoulette/tmpRewardList");
                CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(p, "List");
                if (componetInChild != null)
                {
                    if (isShow)
                    {
                        p.CustomSetActive(true);
                        Utility.FindChild(mallForm.get_gameObject(), "pnlBodyBg/pnlRoulette/Luck/Effect").CustomSetActive(false);
                        if (amount >= 1)
                        {
                            Singleton<CSoundManager>.GetInstance().PostEvent("UI_buy_chou_duobao_degao", null);
                            componetInChild.SetElementAmount(amount);
                            componetInChild.MoveElementInScrollArea(amount - 1, false);
                        }
                    }
                    else
                    {
                        p.CustomSetActive(false);
                    }
                }
                else
                {
                    p.CustomSetActive(false);
                }
            }
        }

        public void Draw(CUIFormScript form)
        {
            CUIEventManager instance = Singleton<CUIEventManager>.GetInstance();
            instance.AddUIEventListener(enUIEventID.Mall_Roulette_Tab_Change, new CUIEventManager.OnUIEventHandler(this.OnRouletteTabChange));
            instance.AddUIEventListener(enUIEventID.Mall_Roulette_Buy_One, new CUIEventManager.OnUIEventHandler(this.OnRouletteBuyOne));
            instance.AddUIEventListener(enUIEventID.Mall_Roulette_Buy_Five, new CUIEventManager.OnUIEventHandler(this.OnRouletteBuyFive));
            instance.AddUIEventListener(enUIEventID.Mall_Roulette_Buy_One_Confirm, new CUIEventManager.OnUIEventHandler(this.OnRouletteBuyOneConfirm));
            instance.AddUIEventListener(enUIEventID.Mall_Roulette_Buy_Five_Confirm, new CUIEventManager.OnUIEventHandler(this.OnRouletteBuyFiveConfirm));
            instance.AddUIEventListener(enUIEventID.Mall_Roulette_Spin_Change, new CUIEventManager.OnUIEventHandler(this.OnSpinInterval));
            instance.AddUIEventListener(enUIEventID.Mall_Roulette_Spin_End, new CUIEventManager.OnUIEventHandler(this.OnSpinEnd));
            instance.AddUIEventListener(enUIEventID.Mall_Roulette_Open_Extern_Reward_Tip, new CUIEventManager.OnUIEventHandler(this.OnOpenExternRewardTip));
            instance.AddUIEventListener(enUIEventID.Mall_Roulette_Draw_Extern_Reward, new CUIEventManager.OnUIEventHandler(this.OnDrawExternReward));
            instance.AddUIEventListener(enUIEventID.Mall_Roulette_Reset_Reward_Draw_Cnt, new CUIEventManager.OnUIEventHandler(this.OnDrawCntReset));
            instance.AddUIEventListener(enUIEventID.Mall_Roulette_Show_Reward_Item, new CUIEventManager.OnUIEventHandler(this.OnShowRewardItem));
            instance.AddUIEventListener(enUIEventID.Mall_Roulette_Show_Rule, new CUIEventManager.OnUIEventHandler(this.OnShowRule));
            instance.AddUIEventListener(enUIEventID.Mall_Roulette_Tmp_Reward_Enable, new CUIEventManager.OnUIEventHandler(this.OnTmpRewardEnable));
            instance.AddUIEventListener(enUIEventID.Mall_Skip_Animation, new CUIEventManager.OnUIEventHandler(this.OnSkipAnimation));
            instance.AddUIEventListener(enUIEventID.Mall_Roulette_Close_Award_Form, new CUIEventManager.OnUIEventHandler(this.OnCloseAwardForm));
            instance.AddUIEventListener(enUIEventID.Mall_Crystal_More, new CUIEventManager.OnUIEventHandler(this.OnCrystalMore));
            instance.AddUIEventListener(enUIEventID.Mall_Cystal_List_Item_Enable, new CUIEventManager.OnUIEventHandler(this.OnCrystalListEnable));
            instance.AddUIEventListener(enUIEventID.Mall_Crystal_On_Exchange, new CUIEventManager.OnUIEventHandler(this.OnCrystalExchange));
            instance.AddUIEventListener(enUIEventID.Mall_Crystal_On_Exchange_Confirm, new CUIEventManager.OnUIEventHandler(this.OnCrystalExchangeConfirm));
            instance.AddUIEventListener(enUIEventID.Mall_CryStal_On_TabChange, new CUIEventManager.OnUIEventHandler(this.OnCryStalOnTabChange));
            Singleton<EventRouter>.instance.AddEventHandler(EventID.Mall_Receive_Roulette_Data, new Action(this, (IntPtr) this.RefreshExternRewards));
            Singleton<EventRouter>.instance.AddEventHandler(EventID.Mall_Change_Tab, new Action(this, (IntPtr) this.OnMallTabChange));
            Singleton<EventRouter>.instance.AddEventHandler<uint>("HeroAdd", new Action<uint>(this.OnNtyAddHero));
            Singleton<EventRouter>.instance.AddEventHandler<uint, uint, uint>("HeroSkinAdd", new Action<uint, uint, uint>(this, (IntPtr) this.OnNtyAddSkin));
            this.InitElements();
            this.RefreshData(0);
            this.InitTab();
        }

        private int GetCryStalItemCount(uint tabID)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                CUseableContainer useableContainer = masterRoleInfo.GetUseableContainer(enCONTAINER_TYPE.ITEM);
                if (tabID < this.m_CrySatlDic.Count)
                {
                    ListView<ResRareExchange> view = this.m_CrySatlDic[tabID];
                    if (view.Count > 0)
                    {
                        uint dwRareItemID = view[0].dwRareItemID;
                        return useableContainer.GetUseableStackCount(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, dwRareItemID);
                    }
                }
            }
            return 0;
        }

        private int GetNextRefreshTime()
        {
            DateTime time = Utility.ToUtcTime2Local((long) CRoleInfo.GetCurrentUTCTime());
            DateTime time2 = new DateTime(0x7b2, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x67);
            int num = 0;
            if ((time.DayOfWeek - ((DayOfWeek) dataByKey.dwConfValue)) > DayOfWeek.Sunday)
            {
                num = 7 - ((int) (time.DayOfWeek - ((DayOfWeek) dataByKey.dwConfValue)));
            }
            else
            {
                num = (int) (((DayOfWeek) dataByKey.dwConfValue) - time.DayOfWeek);
            }
            DateTime time3 = new DateTime(time.Year, time.Month, time.Day, 0, 0, 0, DateTimeKind.Utc);
            time3 = time3.AddDays((double) num);
            int num2 = 0;
            int num3 = 0;
            ResGlobalInfo info2 = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x68);
            num2 = (int) (info2.dwConfValue / 100);
            num3 = (int) (info2.dwConfValue % 100);
            time3 = time3.AddSeconds((num2 * 0xe10) + (num3 * 60));
            if (time.Subtract(time3).TotalSeconds > 0.0)
            {
                time3 = time3.AddDays(7.0);
            }
            return (int) time3.Subtract(time).TotalSeconds;
        }

        public static stPayInfo GetPayInfo(RES_SHOPDRAW_SUBTYPE drawType = 2, Tab tab = -1)
        {
            ResLuckyDrawPrice price;
            stPayInfo info = new stPayInfo();
            info.m_payType = CMallSystem.ResBuyTypeToPayType(10);
            if (tab == Tab.None)
            {
                tab = Singleton<CMallRouletteController>.GetInstance().CurTab;
            }
            switch (tab)
            {
                case Tab.DianQuan:
                    info.m_payType = CMallSystem.ResBuyTypeToPayType(2);
                    break;

                case Tab.Diamond:
                    info.m_payType = CMallSystem.ResBuyTypeToPayType(10);
                    goto Label_0065;
            }
        Label_0065:
            price = new ResLuckyDrawPrice();
            RES_SHOPDRAW_SUBTYPE res_shopdraw_subtype = drawType;
            if (res_shopdraw_subtype == RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_ONE)
            {
                if (GameDataMgr.mallRoulettePriceDict.TryGetValue(info.m_payType, out price))
                {
                    info.m_payValue = price.dwSinglePrice;
                    info.m_oriValue = price.dwSinglePrice;
                }
                else
                {
                    info.m_payValue = uint.MaxValue;
                    info.m_oriValue = uint.MaxValue;
                }
            }
            else if (res_shopdraw_subtype == RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_FIVE)
            {
                if (GameDataMgr.mallRoulettePriceDict.TryGetValue(info.m_payType, out price))
                {
                    info.m_payValue = price.dwMultiPrice;
                    info.m_oriValue = price.dwMultiPrice;
                }
                else
                {
                    info.m_payValue = uint.MaxValue;
                    info.m_oriValue = uint.MaxValue;
                }
            }
            if (price != null)
            {
                ulong ullSalesStartTime = price.ullSalesStartTime;
                ulong ullSalesEndTime = price.ullSalesEndTime;
                if (ullSalesStartTime > ullSalesEndTime)
                {
                    ullSalesStartTime ^= ullSalesEndTime;
                    ullSalesEndTime ^= ullSalesStartTime;
                    ullSalesStartTime = ullSalesEndTime ^ ullSalesStartTime;
                }
                ulong currentUTCTime = (ulong) CRoleInfo.GetCurrentUTCTime();
                if ((ullSalesStartTime > currentUTCTime) || (currentUTCTime >= ullSalesEndTime))
                {
                    return info;
                }
                res_shopdraw_subtype = drawType;
                if (res_shopdraw_subtype != RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_ONE)
                {
                    if (res_shopdraw_subtype == RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_FIVE)
                    {
                        info.m_payValue = price.dwSalesMultiPrice;
                    }
                    return info;
                }
                info.m_payValue = price.dwSalesSinglePrice;
            }
            return info;
        }

        private ResDT_LuckyDrawPeriod GetPeriodCfg(ResLuckyDrawPrice price)
        {
            ulong currentUTCTime = (ulong) CRoleInfo.GetCurrentUTCTime();
            for (int i = 0; i < price.astLuckyDrawPeriod.Length; i++)
            {
                ulong ullStartDate = price.astLuckyDrawPeriod[i].ullStartDate;
                ulong ullEndDate = price.astLuckyDrawPeriod[i].ullEndDate;
                if (ullStartDate > ullEndDate)
                {
                    ullStartDate ^= ullEndDate;
                    ullEndDate ^= ullStartDate;
                    ullStartDate = ullEndDate ^ ullStartDate;
                }
                if ((ullStartDate <= currentUTCTime) && (currentUTCTime < ullEndDate))
                {
                    return price.astLuckyDrawPeriod[i];
                }
            }
            return null;
        }

        public ListView<CUseable> GetRewardUseables(SCPKG_LUCKYDRAW_RSP stLuckyDrawRsp)
        {
            ListView<CUseable> view = new ListView<CUseable>();
            if (stLuckyDrawRsp != null)
            {
                ResLuckyDrawRewardForClient client = new ResLuckyDrawRewardForClient();
                uint dwRewardPoolID = stLuckyDrawRsp.dwRewardPoolID;
                long key = 0L;
                for (int i = 0; i < stLuckyDrawRsp.bRewardCnt; i++)
                {
                    CHeroInfoData data2;
                    ResGlobalInfo dataByKey;
                    ResHeroSkin heroSkin;
                    ResGlobalInfo info3;
                    ResHeroSkinShop shop;
                    key = GameDataMgr.GetDoubleKey(dwRewardPoolID, stLuckyDrawRsp.szRewardIndex[i]);
                    if (!GameDataMgr.mallRouletteRewardDict.TryGetValue(key, out client))
                    {
                        goto Label_021A;
                    }
                    CUseable item = CUseableManager.CreateUsableByRandowReward((RES_RANDOM_REWARD_TYPE) client.dwItemType, (int) client.dwItemCnt, client.dwItemID);
                    if (item == null)
                    {
                        goto Label_021A;
                    }
                    switch (item.m_type)
                    {
                        case COM_ITEM_TYPE.COM_OBJTYPE_HERO:
                        {
                            IHeroData data = CHeroDataFactory.CreateHeroData(client.dwItemID);
                            if (!data.bPlayerOwn)
                            {
                                goto Label_01FD;
                            }
                            data2 = (CHeroInfoData) data;
                            dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x86);
                            DebugHelper.Assert(dataByKey != null, "global cfg databin err: hero exchange id doesnt exist");
                            if (dataByKey != null)
                            {
                                break;
                            }
                            return null;
                        }
                        case COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN:
                        {
                            uint heroId = 0;
                            uint skinId = 0;
                            CSkinInfo.ResolveHeroSkin(client.dwItemID, out heroId, out skinId);
                            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                            if ((masterRoleInfo == null) || !masterRoleInfo.IsHaveHeroSkin(heroId, skinId, false))
                            {
                                goto Label_01FD;
                            }
                            heroSkin = CSkinInfo.GetHeroSkin(heroId, skinId);
                            info3 = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x87);
                            DebugHelper.Assert(info3 != null, "global cfg databin err: hero skin exchange id doesnt exist");
                            if (info3 != null)
                            {
                                goto Label_01A5;
                            }
                            return null;
                        }
                        default:
                            goto Label_01FD;
                    }
                    uint dwConfValue = dataByKey.dwConfValue;
                    item = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, dwConfValue, (data2.m_info.shopCfgInfo == null) ? ((int) 1) : ((int) data2.m_info.shopCfgInfo.dwChgItemCnt));
                    item.ExtraFromType = 1;
                    item.ExtraFromData = (int) client.dwItemID;
                    goto Label_01FD;
                Label_01A5:
                    shop = null;
                    GameDataMgr.skinShopInfoDict.TryGetValue(heroSkin.dwID, out shop);
                    uint baseID = info3.dwConfValue;
                    item = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, baseID, (shop == null) ? ((int) 1) : ((int) shop.dwChgItemCnt));
                    item.ExtraFromType = 2;
                    item.ExtraFromData = (int) client.dwItemID;
                Label_01FD:
                    if (item != null)
                    {
                        item.m_itemSortNum = client.dwItemPreciousValue;
                        view.Add(item);
                    }
                Label_021A:;
                }
            }
            return view;
        }

        public override void Init()
        {
            base.Init();
            this.m_RewardDic = new DictionaryView<uint, ListView<ResLuckyDrawRewardForClient>>();
            this.m_ExternRewardDic = new DictionaryView<uint, ListView<ResDT_LuckyDrawExternReward>>();
            this.m_RewardPoolDic = new Dictionary<uint, uint>();
            this.m_CrySatlDic = new DictionaryView<uint, ListView<ResRareExchange>>();
            this.m_UsedTabs = new List<Tab>();
            this.m_CurLoops = 0;
            this.m_CurSpinIdx = 0;
            this.m_CurRewardIdx = 0;
            this.m_CurSpinCnt = 0;
            this.m_CurContinousDrawSteps = 0;
            this.m_GotAllUnusualItems = false;
            this.m_CurState = Roulette_State.NONE;
            this.m_IsContinousDraw = false;
            this.m_LuckyDrawRsp = null;
            this.m_IsLuckyBarInited = false;
            reqSentTimerSeq = -1;
            this.InitCryStalItemID();
        }

        private void InitCryStalData()
        {
            this.m_CrySatlDic.Clear();
            DictionaryView<uint, ResRareExchange>.Enumerator enumerator = GameDataMgr.rareExchangeDict.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<uint, ResRareExchange> current = enumerator.Current;
                ResRareExchange item = current.Value;
                uint dwTabID = item.dwTabID;
                if (!this.m_CrySatlDic.ContainsKey(dwTabID))
                {
                    ListView<ResRareExchange> view = new ListView<ResRareExchange>();
                    view.Add(item);
                    this.m_CrySatlDic.Add(dwTabID, view);
                }
                else
                {
                    this.m_CrySatlDic[dwTabID].Add(item);
                }
            }
            this.m_nTotalCrySalTab = this.m_CrySatlDic.Count;
            for (uint i = 0; i < this.m_nTotalCrySalTab; i++)
            {
                ListView<ResRareExchange> resultList = this.m_CrySatlDic[i];
                this.ProcessCrystalDataTabID(ref resultList);
            }
        }

        private void InitCryStalItemID()
        {
            DictionaryView<uint, ListView<ResRareExchange>> view = new DictionaryView<uint, ListView<ResRareExchange>>();
            DictionaryView<uint, ResRareExchange>.Enumerator enumerator = GameDataMgr.rareExchangeDict.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<uint, ResRareExchange> current = enumerator.Current;
                ResRareExchange item = current.Value;
                uint dwTabID = item.dwTabID;
                if (!view.ContainsKey(dwTabID))
                {
                    ListView<ResRareExchange> view2 = new ListView<ResRareExchange>();
                    view2.Add(item);
                    view.Add(dwTabID, view2);
                }
                else
                {
                    view[dwTabID].Add(item);
                }
            }
            if (this.m_CrystalItemID.Length < view.Count)
            {
                this.m_CrystalItemID = new int[view.Count];
            }
            for (uint i = 0; i < view.Count; i++)
            {
                ListView<ResRareExchange> view4 = view[i];
                if (view4.Count > 0)
                {
                    this.m_CrystalItemID[i] = (int) view4[0].dwRareItemID;
                }
                else
                {
                    this.m_CrystalItemID[i] = 0;
                }
            }
        }

        private void InitCryStalTabMenu(uint tabID)
        {
            string[] strArray = new string[] { "荣耀商店", "王者商店" };
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(sMallFormCrystal);
            if (form != null)
            {
                CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(form.get_gameObject(), "Panel/Tab");
                componetInChild.SetElementAmount(this.m_nTotalCrySalTab);
                for (int i = 0; i < this.m_nTotalCrySalTab; i++)
                {
                    Text text = Utility.GetComponetInChild<Text>(componetInChild.GetElemenet(i).get_gameObject(), "Text");
                    if ((text != null) && (i < strArray.Length))
                    {
                        text.set_text(strArray[i]);
                    }
                }
                componetInChild.SelectElement((int) tabID, true);
            }
        }

        public void InitElements()
        {
            CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
            if ((mallForm != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
            {
                Utility.FindChild(mallForm.get_gameObject(), "pnlBodyBg/pnlRoulette").CustomSetActive(true);
            }
        }

        private void InitTab()
        {
            CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
            if ((mallForm != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
            {
                this.m_UsedTabs.Clear();
                Tab[] values = (Tab[]) Enum.GetValues(typeof(Tab));
                for (byte i = 0; i < values.Length; i = (byte) (i + 1))
                {
                    if (values[i] != Tab.None)
                    {
                        switch (values[i])
                        {
                            case Tab.DianQuan:
                                if (this.m_RewardDic.ContainsKey(2))
                                {
                                    this.m_UsedTabs.Add(values[i]);
                                }
                                break;

                            case Tab.Diamond:
                                goto Label_0099;
                        }
                    }
                    continue;
                Label_0099:
                    if (this.m_RewardDic.ContainsKey(10))
                    {
                        this.m_UsedTabs.Add(values[i]);
                    }
                }
                DebugHelper.Assert(this.m_UsedTabs.Count != 0, "夺宝单价设定数据档不对");
                if (this.m_UsedTabs.Count == 0)
                {
                    Singleton<CUIManager>.GetInstance().CloseForm(mallForm);
                    Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Mall_Roulette_Init_Error"), false, 1.5f, null, new object[0]);
                }
                else
                {
                    CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(mallForm.get_gameObject(), "pnlBodyBg/pnlRoulette/Tab");
                    if (componetInChild != null)
                    {
                        CUIListElementScript elemenet = null;
                        componetInChild.SetElementAmount(this.m_UsedTabs.Count);
                        for (int j = 0; j < componetInChild.m_elementAmount; j++)
                        {
                            elemenet = componetInChild.GetElemenet(j);
                            if (elemenet != null)
                            {
                                Tab tab = this.m_UsedTabs[j];
                                Text component = elemenet.get_transform().Find("Text").GetComponent<Text>();
                                Image image = elemenet.get_transform().Find("Image").GetComponent<Image>();
                                RES_SHOPBUY_COINTYPE coinType = RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_NULL;
                                bool flag = false;
                                switch (tab)
                                {
                                    case Tab.DianQuan:
                                    {
                                        component.set_text(Singleton<CTextManager>.GetInstance().GetText("Mall_Roulette_DianQuan_Buy_Tab"));
                                        if (image != null)
                                        {
                                            image.SetSprite(CMallSystem.GetPayTypeIconPath(enPayType.DianQuan), mallForm, true, false, false, false);
                                        }
                                        coinType = RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_COUPONS;
                                        stPayInfo payInfo = GetPayInfo(RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_ONE, Tab.DianQuan);
                                        stPayInfo info4 = GetPayInfo(RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_FIVE, Tab.DianQuan);
                                        if ((payInfo.m_payValue < payInfo.m_oriValue) || (info4.m_payValue < info4.m_oriValue))
                                        {
                                            flag = true;
                                        }
                                        break;
                                    }
                                    case Tab.Diamond:
                                    {
                                        component.set_text(Singleton<CTextManager>.GetInstance().GetText("Mall_Roulette_Diamond_Buy_Tab"));
                                        if (image != null)
                                        {
                                            image.SetSprite(CMallSystem.GetPayTypeIconPath(enPayType.Diamond), mallForm, true, false, false, false);
                                        }
                                        coinType = RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_DIAMOND;
                                        stPayInfo info = GetPayInfo(RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_ONE, Tab.Diamond);
                                        stPayInfo info2 = GetPayInfo(RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_FIVE, Tab.Diamond);
                                        if ((info.m_payValue < info.m_oriValue) || (info2.m_payValue < info2.m_oriValue))
                                        {
                                            flag = true;
                                        }
                                        break;
                                    }
                                }
                                if ((this.IsProbabilityDoubled(coinType) || flag) && CUIRedDotSystem.IsShowRedDotByVersion(this.TabToRedID(tab)))
                                {
                                    CUIRedDotSystem.AddRedDot(elemenet.get_gameObject(), enRedDotPos.enTopRight, 0);
                                }
                            }
                        }
                        componetInChild.m_alwaysDispatchSelectedChangeEvent = true;
                        if (((this.CurTab == Tab.None) || (this.CurTab < Tab.DianQuan)) || (this.CurTab >= componetInChild.GetElementAmount()))
                        {
                            componetInChild.SelectElement(0, true);
                        }
                        else
                        {
                            componetInChild.SelectElement((int) this.CurTab, true);
                        }
                    }
                }
            }
        }

        public bool IsCryStalItem(uint itemID)
        {
            for (int i = 0; i < this.m_CrystalItemID.Length; i++)
            {
                if (this.m_CrystalItemID[i] == itemID)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsProbabilityDoubled(RES_SHOPBUY_COINTYPE coinType = 0)
        {
            if ((this.m_RewardDic == null) || (this.m_RewardDic.Count == 0))
            {
                this.RefreshData(0);
            }
            if (this.m_RewardDic != null)
            {
                DictionaryView<uint, ListView<ResLuckyDrawRewardForClient>>.Enumerator enumerator = this.m_RewardDic.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (coinType != RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_NULL)
                    {
                        KeyValuePair<uint, ListView<ResLuckyDrawRewardForClient>> pair = enumerator.Current;
                        if (pair.Key != ((byte) coinType))
                        {
                            continue;
                        }
                    }
                    KeyValuePair<uint, ListView<ResLuckyDrawRewardForClient>> current = enumerator.Current;
                    if (current.Value != null)
                    {
                        KeyValuePair<uint, ListView<ResLuckyDrawRewardForClient>> pair3 = enumerator.Current;
                        if (pair3.Value.Count > 0)
                        {
                            KeyValuePair<uint, ListView<ResLuckyDrawRewardForClient>> pair4 = enumerator.Current;
                            ListView<ResLuckyDrawRewardForClient> view = pair4.Value;
                            int count = view.Count;
                            for (int i = 0; i < count; i++)
                            {
                                if (view[i].bShowProbabilityDoubled > 0)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        public void Load(CUIFormScript form)
        {
            CUICommonSystem.LoadUIPrefab("UGUI/Form/System/Mall/Roulette", "pnlRoulette", form.GetWidget(3), form);
        }

        public bool Loaded(CUIFormScript form)
        {
            if (Utility.FindChild(form.GetWidget(3), "pnlRoulette") == null)
            {
                return false;
            }
            return true;
        }

        private void OnCloseAwardForm(CUIEvent uiEvent)
        {
            this.RefreshRewards();
            this.RefreshExternRewards();
            this.DisplayTmpRewardList(false, 0);
            if (reqSentTimerSeq <= 0)
            {
                Singleton<CMallSystem>.GetInstance().ToggleActionMask(false, 30f);
                this.ToggleBtnGroup(true);
            }
        }

        private void OnCrystalExchange(CUIEvent uiEvent)
        {
            string[] strArray = new string[] { "荣耀水晶", "王者水晶" };
            int cryStalItemCount = this.GetCryStalItemCount(this.m_nCurCryStalTabID);
            string str = string.Empty;
            if (this.m_nCurCryStalTabID < strArray.Length)
            {
                str = strArray[this.m_nCurCryStalTabID];
            }
            if (cryStalItemCount <= 0)
            {
                string strContent = string.Format(Singleton<CTextManager>.GetInstance().GetText("Crystal_Item_NotEnough"), str);
                Singleton<CUIManager>.GetInstance().OpenMessageBox(strContent, false);
            }
            else if (this.m_nCurCryStalTabID < this.m_CrySatlDic.Count)
            {
                RES_ITEM_TYPE tag = (RES_ITEM_TYPE) uiEvent.m_eventParams.tag;
                uint num2 = uiEvent.m_eventParams.commonUInt32Param1;
                uint num3 = uiEvent.m_eventParams.commonUInt16Param1;
                string tagStr = uiEvent.m_eventParams.tagStr;
                if ((num3 >= 0) && (num3 < this.m_CrySatlDic[this.m_nCurCryStalTabID].Count))
                {
                    ResRareExchange exchange = this.m_CrySatlDic[this.m_nCurCryStalTabID][(int) num3];
                    if (exchange.dwID == num2)
                    {
                        string str4 = string.Format(Singleton<CTextManager>.GetInstance().GetText("Confirm_CryStal_Buy_Text"), exchange.dwRareItemCnt, str, tagStr);
                        Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(str4, enUIEventID.Mall_Crystal_On_Exchange_Confirm, enUIEventID.None, uiEvent.m_eventParams, false);
                    }
                }
            }
        }

        private void OnCrystalExchangeConfirm(CUIEvent uiEvent)
        {
            uint rareID = uiEvent.m_eventParams.commonUInt32Param1;
            this.RequestBuyCrystal(rareID);
        }

        private void OnCrystalHeroItem(CUIEvent uiEvent, ushort resItemIdx, ResRareExchange resRareExchange)
        {
            if (resRareExchange != null)
            {
                CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
                GameObject srcWidget = uiEvent.m_srcWidget;
                if ((srcFormScript != null) && (srcWidget != null))
                {
                    ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(resRareExchange.dwExchangeID);
                    if (dataByKey != null)
                    {
                        Transform transform = srcWidget.get_transform();
                        Transform transform2 = transform.Find("heroItem");
                        transform2.get_gameObject().CustomSetActive(true);
                        transform.Find("heroSkinItem").get_gameObject().CustomSetActive(false);
                        Transform transform4 = transform2.Find("imagedefault");
                        if (transform4 != null)
                        {
                            transform4.get_gameObject().CustomSetActive(false);
                        }
                        Transform transform5 = transform2.Find("imageIcon");
                        if (transform5 != null)
                        {
                            transform5.get_gameObject().CustomSetActive(true);
                        }
                        GameObject root = transform2.Find("profession").get_gameObject();
                        CUICommonSystem.SetHeroItemImage(srcFormScript, transform2.get_gameObject(), StringHelper.UTF8BytesToString(ref dataByKey.szImagePath), enHeroHeadType.enBust, false, true);
                        CUICommonSystem.SetHeroJob(srcFormScript, root, (enHeroJobType) dataByKey.bMainJob);
                        Text component = transform2.Find("heroDataPanel/heroNamePanel/heroNameText").GetComponent<Text>();
                        string str = StringHelper.UTF8BytesToString(ref dataByKey.szName);
                        component.set_text(str);
                        CUIEventScript script2 = transform2.GetComponent<CUIEventScript>();
                        stUIEventParams eventParams = new stUIEventParams();
                        eventParams.openHeroFormPar.heroId = dataByKey.dwCfgID;
                        eventParams.openHeroFormPar.openSrc = enHeroFormOpenSrc.HeroBuyClick;
                        script2.SetUIEvent(enUIEventType.Click, enUIEventID.HeroInfo_OpenForm, eventParams);
                        script2.m_closeFormWhenClicked = true;
                        CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                        if (masterRoleInfo == null)
                        {
                            DebugHelper.Assert(masterRoleInfo != null, "Master RoleInfo Is Null");
                        }
                        else
                        {
                            GameObject obj4 = transform.Find("imgExperienceMark").get_gameObject();
                            GameObject obj5 = Utility.FindChild(transform2.get_gameObject(), "heroDataPanel/heroPricePanel");
                            if (obj5 == null)
                            {
                                DebugHelper.Assert(obj5 != null, "price panel is null");
                            }
                            else
                            {
                                obj5.CustomSetActive(false);
                                Transform transform6 = transform.Find("ButtonGroup/BuyBtn");
                                Button button = transform6.GetComponent<Button>();
                                Text text2 = transform6.Find("Text").GetComponent<Text>();
                                CUIEventScript script3 = transform6.GetComponent<CUIEventScript>();
                                transform6.get_gameObject().CustomSetActive(false);
                                script3.set_enabled(false);
                                button.set_enabled(false);
                                if (masterRoleInfo.IsHaveHero(dataByKey.dwCfgID, false))
                                {
                                    transform6.get_gameObject().CustomSetActive(true);
                                    text2.set_text(Singleton<CTextManager>.GetInstance().GetText("Mall_Hero_State_Own"));
                                    obj4.CustomSetActive(false);
                                    CUICommonSystem.PlayAnimator(srcWidget, "OnlyName");
                                }
                                else
                                {
                                    obj4.CustomSetActive(masterRoleInfo.IsValidExperienceHero(dataByKey.dwCfgID));
                                    obj5.CustomSetActive(true);
                                    Image componetInChild = Utility.GetComponetInChild<Image>(obj5, "pnlExchange/costImage");
                                    Text text3 = Utility.GetComponetInChild<Text>(obj5, "pnlExchange/costText");
                                    if ((componetInChild != null) && (text3 != null))
                                    {
                                        text3.set_text(resRareExchange.dwRareItemCnt.ToString());
                                        string prefabPath = this.m_tab0ImagePath;
                                        if (this.m_nCurCryStalTabID == 1)
                                        {
                                            prefabPath = this.m_tab1ImagePath;
                                        }
                                        if (componetInChild != null)
                                        {
                                            componetInChild.SetSprite(prefabPath, srcFormScript, true, false, false, false);
                                        }
                                        transform6.get_gameObject().CustomSetActive(true);
                                        text2.set_text(Singleton<CTextManager>.GetInstance().GetText("Crystal_Exchange_Btn"));
                                        script3.set_enabled(true);
                                        button.set_enabled(true);
                                        stUIEventParams params2 = new stUIEventParams();
                                        params2.tag = 7;
                                        params2.commonUInt32Param1 = resRareExchange.dwID;
                                        params2.commonUInt16Param1 = resItemIdx;
                                        params2.tagStr = str;
                                        script3.SetUIEvent(enUIEventType.Click, enUIEventID.Mall_Crystal_On_Exchange, params2);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void OnCrystalHeroSkinItem(CUIEvent uiEvent, ushort resItemIdx, ResRareExchange resRareExchange)
        {
            if (resRareExchange != null)
            {
                CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
                GameObject srcWidget = uiEvent.m_srcWidget;
                if ((srcFormScript != null) && (srcWidget != null))
                {
                    uint heroId = 0;
                    uint skinId = 0;
                    CSkinInfo.ResolveHeroSkin(resRareExchange.dwExchangeID, out heroId, out skinId);
                    ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(heroId, skinId);
                    if (heroSkin != null)
                    {
                        Transform transform = srcWidget.get_transform();
                        transform.Find("heroItem").get_gameObject().CustomSetActive(false);
                        Transform transform3 = transform.Find("heroSkinItem");
                        transform3.get_gameObject().CustomSetActive(true);
                        Text component = transform3.Find("heroDataPanel/heroNamePanel/heroNameText").GetComponent<Text>();
                        Text text2 = transform3.Find("heroDataPanel/heroNamePanel/heroSkinText").GetComponent<Text>();
                        CUICommonSystem.SetHeroItemImage(srcFormScript, transform3.get_gameObject(), heroSkin.szSkinPicID, enHeroHeadType.enBust, false, true);
                        CUICommonSystem.SetHeroSkinLabelPic(uiEvent.m_srcFormScript, transform3.Find("skinLabelImage").get_gameObject(), heroSkin.dwHeroID, heroSkin.dwSkinID);
                        ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(heroSkin.dwHeroID);
                        if (dataByKey != null)
                        {
                            component.set_text(dataByKey.szName);
                        }
                        text2.set_text(heroSkin.szSkinName);
                        CUIEventScript script2 = transform3.GetComponent<CUIEventScript>();
                        stUIEventParams eventParams = new stUIEventParams();
                        eventParams.openHeroFormPar.heroId = heroSkin.dwHeroID;
                        eventParams.openHeroFormPar.skinId = heroSkin.dwSkinID;
                        eventParams.openHeroFormPar.openSrc = enHeroFormOpenSrc.SkinBuyClick;
                        script2.SetUIEvent(enUIEventType.Click, enUIEventID.HeroInfo_OpenForm, eventParams);
                        script2.m_closeFormWhenClicked = true;
                        CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                        if (masterRoleInfo == null)
                        {
                            DebugHelper.Assert(masterRoleInfo != null, "Master RoleInfo Is Null");
                        }
                        else
                        {
                            Transform transform5 = transform.Find("ButtonGroup/BuyBtn");
                            Button button = transform5.GetComponent<Button>();
                            Text text3 = transform5.Find("Text").GetComponent<Text>();
                            CUIEventScript script3 = transform5.GetComponent<CUIEventScript>();
                            transform5.get_gameObject().CustomSetActive(false);
                            script3.set_enabled(false);
                            button.set_enabled(false);
                            GameObject obj3 = Utility.FindChild(transform3.get_gameObject(), "heroDataPanel/heroPricePanel");
                            if (obj3 == null)
                            {
                                DebugHelper.Assert(obj3 != null, "price panel is null");
                            }
                            else
                            {
                                obj3.CustomSetActive(false);
                                GameObject obj4 = transform.Find("imgExperienceMark").get_gameObject();
                                CTextManager instance = Singleton<CTextManager>.GetInstance();
                                if (masterRoleInfo.IsHaveHeroSkin(heroSkin.dwHeroID, heroSkin.dwSkinID, false))
                                {
                                    transform5.get_gameObject().CustomSetActive(true);
                                    text3.set_text(instance.GetText("Mall_Skin_State_Own"));
                                    obj4.CustomSetActive(false);
                                }
                                else
                                {
                                    obj4.CustomSetActive(masterRoleInfo.IsValidExperienceSkin(heroSkin.dwHeroID, heroSkin.dwSkinID));
                                    obj3.CustomSetActive(true);
                                    if (!masterRoleInfo.IsHaveHero(heroSkin.dwHeroID, false))
                                    {
                                        obj3.CustomSetActive(false);
                                        script3.set_enabled(true);
                                        transform5.get_gameObject().CustomSetActive(true);
                                        text3.set_text(Singleton<CTextManager>.GetInstance().GetText("Mall_Skin_State_Buy_hero"));
                                        button.set_enabled(true);
                                        script3.SetUIEvent(enUIEventType.Click, enUIEventID.HeroInfo_OpenForm, eventParams);
                                        script3.m_closeFormWhenClicked = true;
                                    }
                                    else
                                    {
                                        Image componetInChild = Utility.GetComponetInChild<Image>(obj3, "pnlExchange/costImage");
                                        Text text4 = Utility.GetComponetInChild<Text>(obj3, "pnlExchange/costText");
                                        if ((componetInChild != null) && (text4 != null))
                                        {
                                            string prefabPath = this.m_tab0ImagePath;
                                            if (this.m_nCurCryStalTabID == 1)
                                            {
                                                prefabPath = this.m_tab1ImagePath;
                                            }
                                            if (componetInChild != null)
                                            {
                                                componetInChild.SetSprite(prefabPath, srcFormScript, true, false, false, false);
                                            }
                                            text4.set_text(resRareExchange.dwRareItemCnt.ToString());
                                            script3.set_enabled(true);
                                            transform5.get_gameObject().CustomSetActive(true);
                                            text3.set_text(Singleton<CTextManager>.GetInstance().GetText("Crystal_Exchange_Btn"));
                                            button.set_enabled(true);
                                            stUIEventParams params2 = new stUIEventParams();
                                            params2.tag = 7;
                                            params2.commonUInt32Param1 = resRareExchange.dwID;
                                            params2.commonUInt16Param1 = resItemIdx;
                                            params2.tagStr = dataByKey.szName + "-" + heroSkin.szSkinName;
                                            script3.SetUIEvent(enUIEventType.Click, enUIEventID.Mall_Crystal_On_Exchange, params2);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void OnCrystalListEnable(CUIEvent uiEvent)
        {
            int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
            if (this.m_nCurCryStalTabID < this.m_CrySatlDic.Count)
            {
                if ((srcWidgetIndexInBelongedList < 0) || (srcWidgetIndexInBelongedList >= this.m_CrySatlDic[this.m_nCurCryStalTabID].Count))
                {
                    this.OnDefault(uiEvent);
                }
                else
                {
                    ResRareExchange resRareExchange = this.m_CrySatlDic[this.m_nCurCryStalTabID][srcWidgetIndexInBelongedList];
                    if (resRareExchange.wExchangeType == 4)
                    {
                        this.OnCrystalHeroItem(uiEvent, (ushort) srcWidgetIndexInBelongedList, resRareExchange);
                    }
                    else if (resRareExchange.wExchangeType == 7)
                    {
                        this.OnCrystalHeroSkinItem(uiEvent, (ushort) srcWidgetIndexInBelongedList, resRareExchange);
                    }
                }
            }
        }

        private void OnCrystalMore(CUIEvent uiEvent)
        {
            this.m_nCurCryStalTabID = 0;
            this.m_nTotalCrySalTab = 0;
            Singleton<CUIManager>.GetInstance().OpenForm(sMallFormCrystal, false, true);
            Singleton<CUINewFlagSystem>.GetInstance().DelNewFlag(uiEvent.m_srcWidget, enNewFlagKey.New_CrystalBtn_V1, true);
            this.RefreshCrystalUI(0);
        }

        private void OnCryStalOnTabChange(CUIEvent uiEvent)
        {
            if ((Singleton<CUIManager>.GetInstance().GetForm(sMallFormCrystal) != null) && (uiEvent.m_srcWidgetIndexInBelongedList < this.m_nTotalCrySalTab))
            {
                this.m_nCurCryStalTabID = (uint) uiEvent.m_srcWidgetIndexInBelongedList;
                this.UpdateCryStalTab(this.m_nCurCryStalTabID);
            }
        }

        private void OnDefault(CUIEvent uiEvent)
        {
            CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
            GameObject srcWidget = uiEvent.m_srcWidget;
            if ((srcFormScript != null) && (srcWidget != null))
            {
                Transform transform = srcWidget.get_transform();
                Transform transform2 = transform.Find("heroItem");
                transform2.get_gameObject().CustomSetActive(true);
                transform.Find("heroSkinItem").get_gameObject().CustomSetActive(false);
                Transform transform4 = transform2.Find("Mask");
                if (transform4 != null)
                {
                    transform4.get_gameObject().CustomSetActive(false);
                }
                Transform transform5 = transform2.Find("imagedefault");
                if (transform5 != null)
                {
                    transform5.get_gameObject().CustomSetActive(true);
                }
                Transform transform6 = transform2.Find("imageIcon");
                if (transform6 != null)
                {
                    transform6.get_gameObject().CustomSetActive(false);
                }
                transform2.Find("heroDataPanel/heroNamePanel/heroNameText").GetComponent<Text>().set_text(string.Empty);
                GameObject obj3 = transform.Find("imgExperienceMark").get_gameObject();
                GameObject obj4 = Utility.FindChild(transform2.get_gameObject(), "heroDataPanel/heroPricePanel");
                if (obj4 == null)
                {
                    DebugHelper.Assert(obj4 != null, "price panel is null");
                }
                else
                {
                    obj4.CustomSetActive(false);
                    Transform transform7 = transform.Find("ButtonGroup/BuyBtn");
                    Button component = transform7.GetComponent<Button>();
                    Text text2 = transform7.Find("Text").GetComponent<Text>();
                    CUIEventScript script2 = transform7.GetComponent<CUIEventScript>();
                    transform7.get_gameObject().CustomSetActive(false);
                    script2.set_enabled(false);
                    component.set_enabled(false);
                    transform7.get_gameObject().CustomSetActive(true);
                    text2.set_text("敬请期待");
                    obj3.CustomSetActive(false);
                }
            }
        }

        private void OnDrawCntReset(CUIEvent uiEvent)
        {
            CSDT_LUCKYDRAW_INFO csdt_luckydraw_info = new CSDT_LUCKYDRAW_INFO();
            enPayType diamond = enPayType.Diamond;
            switch (this.CurTab)
            {
                case Tab.DianQuan:
                    diamond = enPayType.Diamond;
                    break;

                case Tab.Diamond:
                    diamond = enPayType.Diamond;
                    break;
            }
            CMallSystem.luckyDrawDic.TryGetValue(diamond, out csdt_luckydraw_info);
            if (csdt_luckydraw_info != null)
            {
                csdt_luckydraw_info.dwDrawMask = 0;
                csdt_luckydraw_info.dwReachMask = 0;
                csdt_luckydraw_info.dwCnt = 0;
            }
            this.RefreshExternRewards();
            this.RefreshTimer();
        }

        private void OnDrawExternReward(CUIEvent uiEvent)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x12c3);
            msg.stPkgData.stLuckyDrawExternReq.bMoneyType = (byte) uiEvent.m_eventParams.tag2;
            msg.stPkgData.stLuckyDrawExternReq.bExternIndex = (byte) uiEvent.m_eventParams.tag;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        [MessageHandler(0x4a1)]
        public static void OnGetBuyCrystalMsg(CSPkg msg)
        {
            if (msg.stPkgData.stRareExchangeRsp.iResult == 0)
            {
                Singleton<CMallRouletteController>.GetInstance().RefreshCrystalPnl();
                for (uint i = 0; i < Singleton<CMallRouletteController>.GetInstance().m_CrySatlDic.Count; i++)
                {
                    for (int j = 0; j < Singleton<CMallRouletteController>.GetInstance().m_CrySatlDic[i].Count; j++)
                    {
                        ResRareExchange exchange = Singleton<CMallRouletteController>.GetInstance().m_CrySatlDic[i][j];
                        if ((exchange != null) && (exchange.dwID == msg.stPkgData.stRareExchangeRsp.dwID))
                        {
                            if (exchange.wExchangeType == 4)
                            {
                                CUICommonSystem.ShowNewHeroOrSkin(exchange.dwExchangeID, 0, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO, false, null, enFormPriority.Priority1, 0, 0);
                            }
                            else if (exchange.wExchangeType == 7)
                            {
                                uint dwExchangeID = exchange.dwExchangeID;
                                CUICommonSystem.ShowNewHeroOrSkin(0, dwExchangeID, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKIN, false, null, enFormPriority.Priority1, 0, 0);
                            }
                        }
                    }
                }
            }
            else
            {
                string strContent = string.Empty;
                if (msg.stPkgData.stRareExchangeRsp.iResult == 1)
                {
                    strContent = Singleton<CTextManager>.GetInstance().GetText("Err_CS_RAREEXCHANGE_ERR_SYS");
                }
                else if (msg.stPkgData.stRareExchangeRsp.iResult == 2)
                {
                    strContent = Singleton<CTextManager>.GetInstance().GetText("Err_CS_RAREEXCHANGE_ERR_ID");
                }
                else if (msg.stPkgData.stRareExchangeRsp.iResult == 3)
                {
                    strContent = Singleton<CTextManager>.GetInstance().GetText("Err_CS_RAREEXCHANGE_ERR_OUTDATE");
                }
                else if (msg.stPkgData.stRareExchangeRsp.iResult == 4)
                {
                    strContent = Singleton<CTextManager>.GetInstance().GetText("Err_CS_RAREEXCHANGE_ERR_DUP");
                }
                else if (msg.stPkgData.stRareExchangeRsp.iResult == 5)
                {
                    strContent = Singleton<CTextManager>.GetInstance().GetText("Err_CS_RAREEXCHANGE_ERR_LIMIT");
                }
                else if (msg.stPkgData.stRareExchangeRsp.iResult == 6)
                {
                    strContent = Singleton<CTextManager>.GetInstance().GetText("Err_CS_RAREEXCHANGE_ERR_EXCHANGE");
                }
                else if (msg.stPkgData.stRareExchangeRsp.iResult == 7)
                {
                    strContent = Singleton<CTextManager>.GetInstance().GetText("Err_CS_RAREEXCHANGE_ERR_STATE");
                }
                Singleton<CUIManager>.GetInstance().OpenTips(strContent, false, 1.5f, null, new object[0]);
            }
        }

        private void OnMallTabChange()
        {
            CUIEventManager instance = Singleton<CUIEventManager>.GetInstance();
            instance.RemoveUIEventListener(enUIEventID.Mall_Roulette_Tab_Change, new CUIEventManager.OnUIEventHandler(this.OnRouletteTabChange));
            instance.RemoveUIEventListener(enUIEventID.Mall_Roulette_Buy_One, new CUIEventManager.OnUIEventHandler(this.OnRouletteBuyOne));
            instance.RemoveUIEventListener(enUIEventID.Mall_Roulette_Buy_Five, new CUIEventManager.OnUIEventHandler(this.OnRouletteBuyFive));
            instance.RemoveUIEventListener(enUIEventID.Mall_Roulette_Buy_One_Confirm, new CUIEventManager.OnUIEventHandler(this.OnRouletteBuyOneConfirm));
            instance.RemoveUIEventListener(enUIEventID.Mall_Roulette_Buy_Five_Confirm, new CUIEventManager.OnUIEventHandler(this.OnRouletteBuyFiveConfirm));
            instance.RemoveUIEventListener(enUIEventID.Mall_Roulette_Spin_Change, new CUIEventManager.OnUIEventHandler(this.OnSpinInterval));
            instance.RemoveUIEventListener(enUIEventID.Mall_Roulette_Spin_End, new CUIEventManager.OnUIEventHandler(this.OnSpinEnd));
            instance.RemoveUIEventListener(enUIEventID.Mall_Roulette_Open_Extern_Reward_Tip, new CUIEventManager.OnUIEventHandler(this.OnOpenExternRewardTip));
            instance.RemoveUIEventListener(enUIEventID.Mall_Roulette_Draw_Extern_Reward, new CUIEventManager.OnUIEventHandler(this.OnDrawExternReward));
            instance.RemoveUIEventListener(enUIEventID.Mall_Roulette_Reset_Reward_Draw_Cnt, new CUIEventManager.OnUIEventHandler(this.OnDrawCntReset));
            instance.RemoveUIEventListener(enUIEventID.Mall_Roulette_Show_Reward_Item, new CUIEventManager.OnUIEventHandler(this.OnShowRewardItem));
            instance.RemoveUIEventListener(enUIEventID.Mall_Roulette_Show_Rule, new CUIEventManager.OnUIEventHandler(this.OnShowRule));
            instance.RemoveUIEventListener(enUIEventID.Mall_Roulette_Tmp_Reward_Enable, new CUIEventManager.OnUIEventHandler(this.OnTmpRewardEnable));
            instance.RemoveUIEventListener(enUIEventID.Mall_Skip_Animation, new CUIEventManager.OnUIEventHandler(this.OnSkipAnimation));
            instance.RemoveUIEventListener(enUIEventID.Mall_Roulette_Close_Award_Form, new CUIEventManager.OnUIEventHandler(this.OnCloseAwardForm));
            instance.RemoveUIEventListener(enUIEventID.Mall_Crystal_More, new CUIEventManager.OnUIEventHandler(this.OnCrystalMore));
            instance.RemoveUIEventListener(enUIEventID.Mall_Cystal_List_Item_Enable, new CUIEventManager.OnUIEventHandler(this.OnCrystalListEnable));
            instance.RemoveUIEventListener(enUIEventID.Mall_Crystal_On_Exchange, new CUIEventManager.OnUIEventHandler(this.OnCrystalExchange));
            instance.RemoveUIEventListener(enUIEventID.Mall_Crystal_On_Exchange_Confirm, new CUIEventManager.OnUIEventHandler(this.OnCrystalExchangeConfirm));
            instance.RemoveUIEventListener(enUIEventID.Mall_CryStal_On_TabChange, new CUIEventManager.OnUIEventHandler(this.OnCryStalOnTabChange));
            Singleton<EventRouter>.instance.RemoveEventHandler(EventID.Mall_Receive_Roulette_Data, new Action(this, (IntPtr) this.RefreshExternRewards));
            Singleton<EventRouter>.instance.RemoveEventHandler(EventID.Mall_Change_Tab, new Action(this, (IntPtr) this.OnMallTabChange));
            Singleton<EventRouter>.instance.RemoveEventHandler<uint>("HeroAdd", new Action<uint>(this.OnNtyAddHero));
            Singleton<EventRouter>.instance.RemoveEventHandler<uint, uint, uint>("HeroSkinAdd", new Action<uint, uint, uint>(this, (IntPtr) this.OnNtyAddSkin));
        }

        private void OnNtyAddHero(uint id)
        {
            this.RefreshRewards();
        }

        private void OnNtyAddSkin(uint heroId, uint skinId, uint addReason)
        {
            this.RefreshRewards();
        }

        private void OnOpenExternRewardTip(CUIEvent uiEvent)
        {
            if ((Singleton<CMallSystem>.GetInstance().m_MallForm != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
            {
                int tag = uiEvent.m_eventParams.tag;
                ListView<ResDT_LuckyDrawExternReward> view = new ListView<ResDT_LuckyDrawExternReward>();
                CSDT_LUCKYDRAW_INFO csdt_luckydraw_info = new CSDT_LUCKYDRAW_INFO();
                switch (this.CurTab)
                {
                    case Tab.DianQuan:
                        CMallSystem.luckyDrawDic.TryGetValue(enPayType.DianQuan, out csdt_luckydraw_info);
                        this.m_ExternRewardDic.TryGetValue(2, out view);
                        break;

                    case Tab.Diamond:
                        CMallSystem.luckyDrawDic.TryGetValue(enPayType.Diamond, out csdt_luckydraw_info);
                        this.m_ExternRewardDic.TryGetValue(10, out view);
                        break;
                }
                if (((view != null) && (view.Count != 0)) && (tag <= view.Count))
                {
                    ResDT_LuckyDrawExternReward reward = view[tag];
                    ResRandomRewardStore dataByKey = null;
                    ListView<CUseable> view2 = new ListView<CUseable>();
                    dataByKey = GameDataMgr.randomRewardDB.GetDataByKey(reward.dwRewardID);
                    if (dataByKey != null)
                    {
                        for (int i = 0; i < dataByKey.astRewardDetail.Length; i++)
                        {
                            if ((dataByKey.astRewardDetail[i].bItemType == 0) || (dataByKey.astRewardDetail[i].bItemType >= 0x12))
                            {
                                break;
                            }
                            ListView<CUseable> collection = CUseableManager.CreateUsableListByRandowReward((RES_RANDOM_REWARD_TYPE) dataByKey.astRewardDetail[i].bItemType, (int) dataByKey.astRewardDetail[i].dwLowCnt, dataByKey.astRewardDetail[i].dwItemID);
                            if ((collection != null) && (collection.Count > 0))
                            {
                                view2.AddRange(collection);
                            }
                        }
                        byte result = 0;
                        byte num4 = 0;
                        if (csdt_luckydraw_info != null)
                        {
                            string str = Convert.ToString((long) csdt_luckydraw_info.dwReachMask, 2).PadLeft(0x20, '0');
                            string str2 = Convert.ToString((long) csdt_luckydraw_info.dwDrawMask, 2).PadLeft(0x20, '0');
                            byte.TryParse(str.Substring(0x20 - (tag + 1), 1), out result);
                            byte.TryParse(str2.Substring(0x20 - (tag + 1), 1), out num4);
                        }
                        CUIFormScript formScript = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/System/Mall/Form_Roullete_ExternReward.prefab", false, true);
                        if (formScript != null)
                        {
                            stPayInfo payInfo = GetPayInfo(RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_ONE, Tab.None);
                            GameObject obj2 = Utility.FindChild(formScript.get_gameObject(), "Panel/btnGroup/btnBuyOne");
                            if (obj2 != null)
                            {
                                stUIEventParams eventParams = new stUIEventParams();
                                CMallSystem.SetPayButton(formScript, obj2.get_transform() as RectTransform, payInfo.m_payType, payInfo.m_payValue, payInfo.m_oriValue, enUIEventID.Mall_Roulette_Buy_One, ref eventParams);
                            }
                            Text componetInChild = Utility.GetComponetInChild<Text>(formScript.get_gameObject(), "Panel/Centent/Desc/Count");
                            if ((componetInChild != null) && (csdt_luckydraw_info != null))
                            {
                                componetInChild.set_text(csdt_luckydraw_info.dwCnt.ToString());
                            }
                            GameObject obj3 = Utility.FindChild(formScript.get_gameObject(), "Panel/Centent/BuyDesc");
                            GameObject obj4 = Utility.FindChild(formScript.get_gameObject(), "Panel/btnGroup/btnConfirm");
                            Text text2 = Utility.GetComponetInChild<Text>(formScript.get_gameObject(), "Panel/btnGroup/btnConfirm/Text");
                            Text text3 = Utility.GetComponetInChild<Text>(formScript.get_gameObject(), "Panel/Centent/BuyDesc/Count");
                            GameObject obj5 = Utility.FindChild(formScript.get_gameObject(), "Panel/btnGroup/btnGet");
                            if (result == 0)
                            {
                                obj3.CustomSetActive(true);
                                obj4.CustomSetActive(true);
                                obj5.CustomSetActive(false);
                                if (text2 != null)
                                {
                                    text2.set_text("去完成");
                                }
                                if ((text3 != null) && (csdt_luckydraw_info != null))
                                {
                                    text3.set_text(string.Format("{0}", reward.dwDrawCnt - csdt_luckydraw_info.dwCnt));
                                }
                            }
                            else
                            {
                                obj3.CustomSetActive(false);
                                if (num4 == 0)
                                {
                                    obj4.CustomSetActive(false);
                                    obj5.CustomSetActive(true);
                                    CUIEventScript component = obj5.GetComponent<CUIEventScript>();
                                    if (component == null)
                                    {
                                        component = obj5.AddComponent<CUIEventScript>();
                                        component.Initialize(formScript);
                                    }
                                    stUIEventParams params2 = new stUIEventParams();
                                    params2.tag = tag;
                                    params2.tag2 = (int) CMallSystem.PayTypeToResBuyCoinType(payInfo.m_payType);
                                    component.SetUIEvent(enUIEventType.Click, enUIEventID.Mall_Roulette_Draw_Extern_Reward, params2);
                                }
                                else
                                {
                                    obj4.CustomSetActive(true);
                                    if (text2 != null)
                                    {
                                        text2.set_text("已领取");
                                    }
                                    obj5.CustomSetActive(false);
                                }
                            }
                            int num5 = 5;
                            int num6 = Mathf.Min(view2.Count, num5);
                            for (int j = 0; j < num6; j++)
                            {
                                GameObject itemCell = Utility.FindChild(formScript.get_gameObject(), string.Format("Panel/Centent/itemCells/itemCell{0}", j + 1));
                                CUICommonSystem.SetItemCell(formScript, itemCell, view2[j], true, false, false, false);
                                itemCell.CustomSetActive(true);
                                itemCell.get_transform().FindChild("ItemName").GetComponent<Text>().set_text(view2[j].m_name);
                            }
                            for (int k = num6; k < num5; k++)
                            {
                                Utility.FindChild(formScript.get_gameObject(), string.Format("Panel/Centent/itemCells/itemCell{0}", k + 1)).CustomSetActive(false);
                            }
                        }
                    }
                }
            }
        }

        private void OnRouletteBuyFive(CUIEvent uiEvent)
        {
            if ((Singleton<CMallSystem>.GetInstance().m_MallForm != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
            {
                if (Singleton<CMatchingSystem>.GetInstance().IsInMatching)
                {
                    GameObject widget = Singleton<CMallSystem>.GetInstance().m_MallForm.GetWidget(3);
                    Singleton<CUIManager>.GetInstance().OpenTips("PVP_Matching", true, 1f, widget, new object[0]);
                }
                else
                {
                    stPayInfo payInfo = GetPayInfo(RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_FIVE, Tab.None);
                    CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
                    uIEvent.m_eventID = enUIEventID.Mall_Roulette_Buy_Five_Confirm;
                    if (payInfo.m_payType == enPayType.Diamond)
                    {
                        CMallSystem.TryToPay(enPayPurpose.Roulette, string.Empty, enPayType.DiamondAndDianQuan, payInfo.m_payValue, uIEvent.m_eventID, ref uIEvent.m_eventParams, enUIEventID.None, false, true, false);
                    }
                    else
                    {
                        CMallSystem.TryToPay(enPayPurpose.Roulette, string.Empty, payInfo.m_payType, payInfo.m_payValue, uIEvent.m_eventID, ref uIEvent.m_eventParams, enUIEventID.None, false, true, false);
                    }
                }
            }
        }

        private void OnRouletteBuyFiveConfirm(CUIEvent uiEvent)
        {
            CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
            if ((mallForm != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
            {
                Singleton<CMallSystem>.GetInstance().ToggleActionMask(true, 10f);
                int num = this.m_CurSpinIdx % 14;
                GameObject target = Utility.FindChild(mallForm.get_gameObject(), string.Format("pnlBodyBg/pnlRoulette/rewardBody/rewardContainer/itemCell{0}/halo", num));
                if (target != null)
                {
                    CUICommonSystem.PlayAnimator(target, "Roulette_Halo_Disappear");
                }
                this.m_IsContinousDraw = true;
                this.Spin(-1);
                this.ToggleBtnGroup(false);
                this.SendLotteryMsg(RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_FIVE);
            }
        }

        private void OnRouletteBuyOne(CUIEvent uiEvent)
        {
            if ((Singleton<CMallSystem>.GetInstance().m_MallForm != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
            {
                if (Singleton<CMatchingSystem>.GetInstance().IsInMatching)
                {
                    GameObject widget = Singleton<CMallSystem>.GetInstance().m_MallForm.GetWidget(3);
                    Singleton<CUIManager>.GetInstance().OpenTips("PVP_Matching", true, 1f, widget, new object[0]);
                }
                else
                {
                    stPayInfo payInfo = GetPayInfo(RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_ONE, Tab.None);
                    CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
                    uIEvent.m_eventID = enUIEventID.Mall_Roulette_Buy_One_Confirm;
                    if (payInfo.m_payType == enPayType.Diamond)
                    {
                        CMallSystem.TryToPay(enPayPurpose.Roulette, string.Empty, enPayType.DiamondAndDianQuan, payInfo.m_payValue, uIEvent.m_eventID, ref uIEvent.m_eventParams, enUIEventID.None, false, true, false);
                    }
                    else
                    {
                        CMallSystem.TryToPay(enPayPurpose.Roulette, string.Empty, payInfo.m_payType, payInfo.m_payValue, uIEvent.m_eventID, ref uIEvent.m_eventParams, enUIEventID.None, false, true, false);
                    }
                }
            }
        }

        private void OnRouletteBuyOneConfirm(CUIEvent uiEvent)
        {
            CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
            if ((mallForm != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
            {
                Singleton<CMallSystem>.GetInstance().ToggleActionMask(true, 10f);
                int num = this.m_CurSpinIdx % 14;
                GameObject target = Utility.FindChild(mallForm.get_gameObject(), string.Format("pnlBodyBg/pnlRoulette/rewardBody/rewardContainer/itemCell{0}/halo", num));
                if (target != null)
                {
                    CUICommonSystem.PlayAnimator(target, "Roulette_Halo_Disappear");
                }
                Singleton<CSoundManager>.GetInstance().PostEvent("UI_buy_chou_duobao_btu", null);
                this.m_IsContinousDraw = false;
                this.Spin(-1);
                this.ToggleBtnGroup(false);
                this.SendLotteryMsg(RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_ONE);
            }
        }

        private void OnRouletteTabChange(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.GetInstance().CloseTips();
            CUICommonSystem.CloseCommonTips();
            CUICommonSystem.CloseUseableTips();
            CUIListScript component = uiEvent.m_srcWidget.GetComponent<CUIListScript>();
            int selectedIndex = component.GetSelectedIndex();
            if (selectedIndex <= this.m_UsedTabs.Count)
            {
                int lastSelectedIndex = component.GetLastSelectedIndex();
                if ((lastSelectedIndex >= 0) && (lastSelectedIndex < this.m_UsedTabs.Count))
                {
                    Tab tab = this.m_UsedTabs[lastSelectedIndex];
                    RES_SHOPBUY_COINTYPE coinType = RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_NULL;
                    bool flag = false;
                    switch (tab)
                    {
                        case Tab.DianQuan:
                        {
                            coinType = RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_COUPONS;
                            stPayInfo payInfo = GetPayInfo(RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_ONE, Tab.DianQuan);
                            stPayInfo info4 = GetPayInfo(RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_FIVE, Tab.DianQuan);
                            if ((payInfo.m_payValue < payInfo.m_oriValue) || (info4.m_payValue < info4.m_oriValue))
                            {
                                flag = true;
                            }
                            break;
                        }
                        case Tab.Diamond:
                        {
                            coinType = RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_DIAMOND;
                            stPayInfo info = GetPayInfo(RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_ONE, Tab.Diamond);
                            stPayInfo info2 = GetPayInfo(RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_FIVE, Tab.Diamond);
                            if ((info.m_payValue < info.m_oriValue) || (info2.m_payValue < info2.m_oriValue))
                            {
                                flag = true;
                            }
                            break;
                        }
                    }
                    if (this.IsProbabilityDoubled(coinType) || flag)
                    {
                        CUIRedDotSystem.SetRedDotViewByVersion(this.TabToRedID(tab));
                        CUIListElementScript lastSelectedElement = component.GetLastSelectedElement();
                        if (lastSelectedElement != null)
                        {
                            CUICommonSystem.DelRedDot(lastSelectedElement.get_gameObject());
                        }
                    }
                }
                this.CurTab = this.m_UsedTabs[selectedIndex];
                this.RefreshRewards();
                this.m_IsLuckyBarInited = false;
                this.RefreshExternRewards();
                this.DisplayTmpRewardList(false, 0);
                this.RefreshButtonView();
                this.RefreshTimer();
            }
        }

        private void OnShowRewardItem(CUIEvent uiEvent)
        {
            if ((this.m_CurRewardIdx >= 0) && (this.m_CurRewardIdx < this.m_LuckyDrawRsp.bRewardCnt))
            {
                Utility.FindChild(uiEvent.m_srcFormScript.get_gameObject(), string.Format("Panel/Centent/itemCell{0}", this.m_CurRewardIdx + 1)).CustomSetActive(true);
                Singleton<CSoundManager>.GetInstance().PostEvent("UI_buy_chou_get_N", null);
                this.m_CurRewardIdx++;
            }
            else
            {
                (uiEvent.m_srcWidgetScript as CUITimerScript).EndTimer();
            }
        }

        private void OnShowRule(CUIEvent uiEvent)
        {
            ResRuleText dataByKey = GameDataMgr.s_ruleTextDatabin.GetDataByKey((uint) 2);
            if (dataByKey != null)
            {
                string title = StringHelper.UTF8BytesToString(ref dataByKey.szTitle);
                string info = StringHelper.UTF8BytesToString(ref dataByKey.szContent);
                Singleton<CUIManager>.GetInstance().OpenInfoForm(title, info);
            }
        }

        private void OnSkipAnimation(CUIEvent uiEvent)
        {
            this.m_CurState = Roulette_State.SKIP;
        }

        private void OnSpinEnd(CUIEvent uiEvent)
        {
            CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
            if ((mallForm != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
            {
                int tag = uiEvent.m_eventParams.tag;
                if (tag != -1)
                {
                    GameObject target = Utility.FindChild(mallForm.get_gameObject(), string.Format("pnlBodyBg/pnlRoulette/rewardBody/rewardContainer/itemCell{0}/halo", tag));
                    if (target != null)
                    {
                        CUICommonSystem.PlayAnimator(target, "Roulette_Halo_Focus");
                    }
                }
                if (((this.m_CurState == Roulette_State.CONTINUOUS_DRAW) && (this.m_LuckyDrawRsp != null)) && (this.m_CurRewardIdx < this.m_LuckyDrawRsp.bRewardCnt))
                {
                    Singleton<CMallSystem>.GetInstance().ToggleSkipAnimationMask(true, 30f);
                    Singleton<CMallSystem>.GetInstance().ToggleActionMask(false, 30f);
                    this.m_CurRewardIdx++;
                    this.DisplayTmpRewardList(true, this.m_CurRewardIdx);
                    Debug.Log(string.Format("五连抽第{0}次", this.m_CurRewardIdx));
                    if (this.m_CurRewardIdx < this.m_LuckyDrawRsp.bRewardCnt)
                    {
                        if (Math.Abs((int) (this.m_LuckyDrawRsp.szRewardIndex[this.m_CurRewardIdx] - tag)) > 7)
                        {
                            if ((this.m_LuckyDrawRsp.szRewardIndex[this.m_CurRewardIdx] - tag) > 0)
                            {
                                this.m_IsClockwise = false;
                            }
                            else
                            {
                                this.m_IsClockwise = true;
                            }
                        }
                        else if ((this.m_LuckyDrawRsp.szRewardIndex[this.m_CurRewardIdx] - tag) < 0)
                        {
                            this.m_IsClockwise = false;
                        }
                        else
                        {
                            this.m_IsClockwise = true;
                        }
                        this.m_IsClockwise = true;
                        Singleton<CTimerManager>.GetInstance().AddTimer(500, 1, delegate (int sequence) {
                            this.m_CurContinousDrawSteps = 0;
                            this.Spin(this.m_LuckyDrawRsp.szRewardIndex[this.m_CurRewardIdx]);
                        });
                        return;
                    }
                }
                CUITimerScript componetInChild = Utility.GetComponetInChild<CUITimerScript>(mallForm.get_gameObject(), "pnlBodyBg/pnlRoulette/timerSpin");
                if (componetInChild != null)
                {
                    componetInChild.m_eventIDs[1] = enUIEventID.None;
                    componetInChild.EndTimer();
                }
                Singleton<CMallSystem>.GetInstance().ToggleActionMask(true, 10f);
                Singleton<CMallSystem>.GetInstance().ToggleSkipAnimationMask(false, 30f);
                if ((this.m_LuckyDrawRsp != null) && (this.m_LuckyDrawRsp.bRewardCnt != 0))
                {
                    Singleton<CTimerManager>.GetInstance().AddTimer(600, 1, delegate (int sequence) {
                        string title = null;
                        switch (this.CurTab)
                        {
                            case Tab.DianQuan:
                                title = "点券夺宝奖励";
                                break;

                            case Tab.Diamond:
                                title = "钻石夺宝奖励";
                                break;
                        }
                        Singleton<CSoundManager>.GetInstance().PostEvent("Stop_UI_buy_chou_duobao_zhuan", null);
                        if (this.m_LuckyDrawRsp.bRewardCnt == 1)
                        {
                            this.OpenAwardTip(RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_ONE, this.m_RewardList, title, true, enUIEventID.None, false);
                        }
                        else
                        {
                            this.OpenAwardTip(RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_FIVE, this.m_RewardList, title, true, enUIEventID.None, false);
                        }
                        this.ShowHeroSkin(this.m_RewardList);
                    });
                }
            }
        }

        private void OnSpinInterval(CUIEvent uiEvent)
        {
            this.m_CurSpinCnt++;
            int num = Math.Abs((int) (this.m_CurSpinIdx % 14));
            if ((this.m_CurSpinCnt % 14) == 0)
            {
                this.m_CurLoops++;
            }
            CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
            if ((mallForm != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
            {
                GameObject target = Utility.FindChild(mallForm.get_gameObject(), string.Format("pnlBodyBg/pnlRoulette/rewardBody/rewardContainer/itemCell{0}/halo", num));
                if (target != null)
                {
                    CUICommonSystem.PlayAnimator(target, "Roulette_Halo_Appear");
                }
                CUITimerScript srcWidgetScript = uiEvent.m_srcWidgetScript as CUITimerScript;
                if (this.m_CurState == Roulette_State.ACCELERATE)
                {
                    float num2 = 0.1f - ((this.m_CurSpinCnt * 0.07f) / 4f);
                    if (num2 > 0.03f)
                    {
                        if (srcWidgetScript != null)
                        {
                            srcWidgetScript.SetOnChangedIntervalTime((num2 > 0.03f) ? num2 : 0.03f);
                        }
                    }
                    else
                    {
                        this.m_CurState = Roulette_State.UNIFORM;
                    }
                }
                if ((((this.m_CurState == Roulette_State.UNIFORM) && (uiEvent.m_eventParams.tag != -1)) && (this.m_CurLoops > 2)) && ((((uiEvent.m_eventParams.tag - 4) < 0) && (num == ((14 + uiEvent.m_eventParams.tag) - 4))) || (((uiEvent.m_eventParams.tag - 4) >= 0) && (num == (uiEvent.m_eventParams.tag - 4)))))
                {
                    this.m_CurState = Roulette_State.DECELERATE;
                    this.m_CurSpinCnt = 0;
                }
                if (this.m_CurState == Roulette_State.DECELERATE)
                {
                    float num3 = 0.03f + ((this.m_CurSpinCnt * 0.07f) / 4f);
                    if (((uiEvent.m_eventParams.tag == num) && (num3 >= 0.1f)) && (srcWidgetScript != null))
                    {
                        if (!this.m_IsContinousDraw)
                        {
                            this.m_CurState = Roulette_State.NONE;
                        }
                        else
                        {
                            this.m_CurState = Roulette_State.CONTINUOUS_DRAW;
                            this.DisplayTmpRewardList(true, 0);
                        }
                        srcWidgetScript.m_eventIDs[1] = enUIEventID.Mall_Roulette_Spin_End;
                        srcWidgetScript.m_eventParams[1].tag = uiEvent.m_eventParams.tag;
                        srcWidgetScript.SetOnChangedIntervalTime(0.03f);
                        srcWidgetScript.SetTotalTime(0f);
                        return;
                    }
                    if (srcWidgetScript != null)
                    {
                        srcWidgetScript.SetOnChangedIntervalTime((num3 < 0.1f) ? num3 : 0.1f);
                    }
                }
                if (((this.m_CurState == Roulette_State.CONTINUOUS_DRAW) || (this.m_CurState == Roulette_State.SKIP)) && ((uiEvent.m_eventParams.tag == num) && (this.m_CurContinousDrawSteps >= 1)))
                {
                    if (srcWidgetScript != null)
                    {
                        srcWidgetScript.m_eventIDs[1] = enUIEventID.Mall_Roulette_Spin_End;
                        srcWidgetScript.m_eventParams[1].tag = uiEvent.m_eventParams.tag;
                        srcWidgetScript.SetTotalTime(0f);
                    }
                }
                else
                {
                    if (this.m_IsClockwise)
                    {
                        this.m_CurSpinIdx++;
                    }
                    else
                    {
                        this.m_CurSpinIdx--;
                    }
                    this.m_CurContinousDrawSteps = (byte) (this.m_CurContinousDrawSteps + 1);
                }
            }
        }

        private void OnTmpRewardEnable(CUIEvent uiEvent)
        {
            int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
            if (((this.m_RewardList != null) && (srcWidgetIndexInBelongedList >= 0)) && (srcWidgetIndexInBelongedList <= this.m_RewardList.Count))
            {
                GameObject itemCell = uiEvent.m_srcWidget.get_transform().Find("itemCell").get_gameObject();
                CUseable itemUseable = this.m_RewardList[srcWidgetIndexInBelongedList];
                CUICommonSystem.SetItemCell(uiEvent.m_srcFormScript, itemCell, itemUseable, false, false, false, false);
            }
        }

        public void OpenAwardTip(RES_SHOPDRAW_SUBTYPE drawType, ListView<CUseable> items, string title = new string(), bool playSound = false, enUIEventID eventID = 0, bool displayAll = false)
        {
            if (items == null)
            {
                return;
            }
            int num = 5;
            int num2 = Mathf.Min(items.Count, num);
            this.m_CurRewardIdx = 0;
            CUIFormScript formScript = null;
            if (items.Count < 5)
            {
                formScript = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/System/Mall/Form_Roullete_Reward.prefab", false, true);
            }
            else
            {
                formScript = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/System/Mall/Form_Roullete_Reward_Five.prefab", false, true);
            }
            if (formScript == null)
            {
                Singleton<CUIManager>.GetInstance().OpenTips("Error get reward form failed", false, 1.5f, null, new object[0]);
                return;
            }
            CUIEventScript componetInChild = Utility.GetComponetInChild<CUIEventScript>(formScript.get_gameObject(), "Panel/Button_Back");
            if (componetInChild != null)
            {
                componetInChild.m_onClickEventID = eventID;
            }
            CUIEventScript script3 = Utility.GetComponetInChild<CUIEventScript>(formScript.get_gameObject(), "Panel/CloseBtn");
            if (script3 != null)
            {
                script3.m_onClickEventID = eventID;
            }
            GameObject obj2 = null;
            stPayInfo payInfo = new stPayInfo();
            stUIEventParams eventParams = new stUIEventParams();
            enUIEventID none = enUIEventID.None;
            switch (drawType)
            {
                case RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_ONE:
                    payInfo = GetPayInfo(RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_ONE, Tab.None);
                    Utility.FindChild(formScript.get_gameObject(), "Panel/btnBuyFive").CustomSetActive(false);
                    obj2 = Utility.FindChild(formScript.get_gameObject(), "Panel/btnBuyOne");
                    none = enUIEventID.Mall_Roulette_Buy_One;
                    break;

                case RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_FIVE:
                    payInfo = GetPayInfo(RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_FIVE, Tab.None);
                    Utility.FindChild(formScript.get_gameObject(), "Panel/btnBuyOne").CustomSetActive(false);
                    obj2 = Utility.FindChild(formScript.get_gameObject(), "Panel/btnBuyFive");
                    none = enUIEventID.Mall_Roulette_Buy_Five;
                    goto Label_017A;
            }
        Label_017A:
            obj2.CustomSetActive(true);
            if (obj2 != null)
            {
                CMallSystem.SetPayButton(formScript, obj2.get_transform() as RectTransform, payInfo.m_payType, payInfo.m_payValue, payInfo.m_oriValue, none, ref eventParams);
            }
            if (title != null)
            {
                Utility.GetComponetInChild<Text>(formScript.get_gameObject(), "Panel/bg/Title").set_text(title);
            }
            for (int i = 0; i < num2; i++)
            {
                formScript.get_transform().FindChild(string.Format("Panel/Centent/itemCell{0}", i + 1)).get_gameObject().CustomSetActive(false);
            }
            Transform transform = formScript.get_transform().Find("showRewardTimer");
            CUITimerScript component = null;
            if (transform != null)
            {
                component = transform.get_gameObject().GetComponent<CUITimerScript>();
            }
            if (component == null)
            {
                for (int j = 0; j < num2; j++)
                {
                    GameObject itemCell = formScript.get_transform().FindChild(string.Format("Panel/Centent/itemCell{0}", j + 1)).get_gameObject();
                    CUICommonSystem.SetItemCell(formScript, itemCell, items[j], true, displayAll, false, false);
                    if (items[j].m_itemSortNum > 0L)
                    {
                        Utility.FindChild(itemCell, "Effect_glow").CustomSetActive(true);
                    }
                    itemCell.CustomSetActive(true);
                    Singleton<CSoundManager>.GetInstance().PostEvent("UI_buy_chou_get_01", null);
                    if (playSound)
                    {
                        COM_REWARDS_TYPE mapRewardType = items[j].MapRewardType;
                        if (mapRewardType != COM_REWARDS_TYPE.COM_REWARDS_TYPE_COIN)
                        {
                            if (mapRewardType == COM_REWARDS_TYPE.COM_REWARDS_TYPE_AP)
                            {
                                goto Label_033B;
                            }
                            if (mapRewardType == COM_REWARDS_TYPE.COM_REWARDS_TYPE_DIAMOND)
                            {
                                goto Label_0325;
                            }
                        }
                        else
                        {
                            Singleton<CSoundManager>.GetInstance().PostEvent("UI_hall_add_coin", null);
                        }
                    }
                    continue;
                Label_0325:
                    Singleton<CSoundManager>.GetInstance().PostEvent("UI_hall_add_diamond", null);
                    continue;
                Label_033B:
                    Singleton<CSoundManager>.GetInstance().PostEvent("UI_hall_add_physical_power", null);
                }
            }
            else
            {
                for (int k = 0; k < num2; k++)
                {
                    GameObject obj8 = formScript.get_transform().FindChild(string.Format("Panel/Centent/itemCell{0}", k + 1)).get_gameObject();
                    CUICommonSystem.SetItemCell(formScript, obj8, items[k], true, displayAll, false, false);
                    if (items[k].m_itemSortNum > 0L)
                    {
                        Utility.FindChild(obj8, "Effect_glow").CustomSetActive(true);
                    }
                    obj8.CustomSetActive(false);
                }
                component.SetTotalTime(100f);
                component.StartTimer();
            }
        }

        private void ProcessCrystalDataTabID(ref ListView<ResRareExchange> resultList)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                ListView<ResRareExchange> view = new ListView<ResRareExchange>();
                ListView<ResRareExchange> view2 = new ListView<ResRareExchange>();
                ListView<ResRareExchange> view3 = new ListView<ResRareExchange>();
                DictionaryView<uint, ResRareExchange>.Enumerator enumerator = GameDataMgr.rareExchangeDict.GetEnumerator();
                uint currentUTCTime = (uint) CRoleInfo.GetCurrentUTCTime();
                for (int i = 0; i < resultList.Count; i++)
                {
                    ResRareExchange item = resultList[i];
                    if (item.wExchangeType == 4)
                    {
                        ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(item.dwExchangeID);
                        if (dataByKey != null)
                        {
                            if (masterRoleInfo.IsHaveHero(dataByKey.dwCfgID, false))
                            {
                                view3.Add(item);
                            }
                            else
                            {
                                view.Add(item);
                            }
                        }
                    }
                    else if (item.wExchangeType == 7)
                    {
                        uint heroId = 0;
                        uint skinId = 0;
                        CSkinInfo.ResolveHeroSkin(item.dwExchangeID, out heroId, out skinId);
                        ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(heroId, skinId);
                        if (heroSkin != null)
                        {
                            if (masterRoleInfo.IsHaveHeroSkin(heroSkin.dwHeroID, heroSkin.dwSkinID, false))
                            {
                                view3.Add(item);
                            }
                            else
                            {
                                view2.Add(item);
                            }
                        }
                    }
                }
                if (<>f__am$cache1A == null)
                {
                    <>f__am$cache1A = delegate (ResRareExchange a, ResRareExchange b) {
                        if ((a != null) || (b != null))
                        {
                            if ((a == null) && (b != null))
                            {
                                return 1;
                            }
                            if ((b == null) && (a != null))
                            {
                                return -1;
                            }
                            if (a.iSortID < b.iSortID)
                            {
                                return 1;
                            }
                            if (a.iSortID > b.iSortID)
                            {
                                return -1;
                            }
                        }
                        return 0;
                    };
                }
                view.Sort(<>f__am$cache1A);
                if (<>f__am$cache1B == null)
                {
                    <>f__am$cache1B = delegate (ResRareExchange a, ResRareExchange b) {
                        if ((a != null) || (b != null))
                        {
                            if ((a == null) && (b != null))
                            {
                                return 1;
                            }
                            if ((b == null) && (a != null))
                            {
                                return -1;
                            }
                            if (a.iSortID < b.iSortID)
                            {
                                return 1;
                            }
                            if (a.iSortID > b.iSortID)
                            {
                                return -1;
                            }
                        }
                        return 0;
                    };
                }
                view2.Sort(<>f__am$cache1B);
                if (<>f__am$cache1C == null)
                {
                    <>f__am$cache1C = delegate (ResRareExchange a, ResRareExchange b) {
                        if ((a != null) || (b != null))
                        {
                            if ((a == null) && (b != null))
                            {
                                return 1;
                            }
                            if ((b == null) && (a != null))
                            {
                                return -1;
                            }
                            if (a.wExchangeType > b.wExchangeType)
                            {
                                return 1;
                            }
                            if (a.wExchangeType < b.wExchangeType)
                            {
                                return -1;
                            }
                        }
                        return 0;
                    };
                }
                view3.Sort(<>f__am$cache1C);
                resultList.Clear();
                for (int j = 0; j < view.Count; j++)
                {
                    resultList.Add(view[j]);
                }
                for (int k = 0; k < view2.Count; k++)
                {
                    resultList.Add(view2[k]);
                }
                for (int m = 0; m < view3.Count; m++)
                {
                    resultList.Add(view3[m]);
                }
            }
        }

        [MessageHandler(0x12c4)]
        public static void ReceiveDrawExternRewardRes(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            if (msg.stPkgData.stLuckyDrawExternRsp.iResult != 0)
            {
                Singleton<CUIManager>.GetInstance().OpenTips(Utility.ProtErrCodeToStr(0x12c4, msg.stPkgData.stLuckyDrawExternRsp.iResult), false, 1.5f, null, new object[0]);
            }
            else
            {
                ListView<CUseable> useableListFromReward = CUseableManager.GetUseableListFromReward(msg.stPkgData.stLuckyDrawExternRsp.stReward);
                if (useableListFromReward.Count != 0)
                {
                    CUseable[] items = new CUseable[useableListFromReward.Count];
                    for (int i = 0; i < items.Length; i++)
                    {
                        items[i] = useableListFromReward[i];
                    }
                    Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.Mall_Receive_Roulette_Data);
                    Singleton<CUIManager>.GetInstance().OpenAwardTip(items, Singleton<CTextManager>.GetInstance().GetText("gotAward"), false, enUIEventID.None, false, false, "Form_Award");
                    Singleton<CMallRouletteController>.GetInstance().ShowHeroSkin(useableListFromReward);
                }
            }
        }

        [MessageHandler(0x12c2)]
        public static void ReceiveLotteryRes(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
            if ((mallForm != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
            {
                if (msg.stPkgData.stLuckyDrawRsp.iResult != 0)
                {
                    CUITimerScript componetInChild = Utility.GetComponetInChild<CUITimerScript>(mallForm.get_gameObject(), "pnlBodyBg/pnlRoulette/timerSpin");
                    if (componetInChild != null)
                    {
                        componetInChild.SetTotalTime(0f);
                    }
                    Singleton<CUIManager>.GetInstance().OpenTips(Utility.ProtErrCodeToStr(0x12c2, msg.stPkgData.stLuckyDrawRsp.iResult), false, 1.5f, null, new object[0]);
                    Singleton<CMallRouletteController>.GetInstance().ToggleBtnGroup(true);
                }
                else
                {
                    Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref reqSentTimerSeq);
                    uint num = 0;
                    if (Singleton<CMallRouletteController>.GetInstance().m_RewardPoolDic.TryGetValue(msg.stPkgData.stLuckyDrawRsp.bMoneyType, out num))
                    {
                        if (num != msg.stPkgData.stLuckyDrawRsp.dwRewardPoolID)
                        {
                            Singleton<CMallRouletteController>.GetInstance().RefreshData(msg.stPkgData.stLuckyDrawRsp.dwRewardPoolID);
                        }
                        else
                        {
                            Singleton<CMallRouletteController>.GetInstance().RefreshData(0);
                        }
                    }
                    Singleton<CMallRouletteController>.GetInstance().SetRewardData(msg.stPkgData.stLuckyDrawRsp);
                    int idx = msg.stPkgData.stLuckyDrawRsp.szRewardIndex[0];
                    Singleton<CMallRouletteController>.GetInstance().Spin(idx);
                }
            }
        }

        private void RefreshButtonView()
        {
            CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
            if ((mallForm != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
            {
                this.ToggleBtnGroup(true);
                GameObject obj2 = Utility.FindChild(mallForm.get_gameObject(), "pnlBodyBg/pnlRoulette/Luck/btnGroup/btnBuyOne");
                GameObject obj3 = Utility.FindChild(mallForm.get_gameObject(), "pnlBodyBg/pnlRoulette/Luck/btnGroup/btnBuyFive");
                if ((obj2 != null) && (obj3 != null))
                {
                    stPayInfo payInfo = GetPayInfo(RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_ONE, Tab.None);
                    stPayInfo info2 = GetPayInfo(RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_FIVE, Tab.None);
                    stUIEventParams eventParams = new stUIEventParams();
                    ResLuckyDrawPrice price = new ResLuckyDrawPrice();
                    switch (this.CurTab)
                    {
                        case Tab.DianQuan:
                            GameDataMgr.mallRoulettePriceDict.TryGetValue(enPayType.DianQuan, out price);
                            break;

                        case Tab.Diamond:
                            GameDataMgr.mallRoulettePriceDict.TryGetValue(enPayType.Diamond, out price);
                            break;
                    }
                    CMallSystem.SetPayButton(mallForm, obj2.get_transform() as RectTransform, payInfo.m_payType, payInfo.m_payValue, payInfo.m_oriValue, enUIEventID.Mall_Roulette_Buy_One, ref eventParams);
                    CMallSystem.SetPayButton(mallForm, obj3.get_transform() as RectTransform, payInfo.m_payType, info2.m_payValue, info2.m_oriValue, enUIEventID.Mall_Roulette_Buy_Five, ref eventParams);
                }
            }
        }

        private void RefreshCrystalPnl()
        {
            this.RefreshCrystalUI(this.m_nCurCryStalTabID);
        }

        private void RefreshCrystalUI(uint tabID)
        {
            this.m_nCurCryStalTabID = tabID;
            this.InitCryStalData();
            this.InitCryStalTabMenu(tabID);
            this.UpdateCryStalTab(tabID);
        }

        public void RefreshData(uint targetPoolId = 0)
        {
            if (this.m_RewardDic == null)
            {
                this.m_RewardDic = new DictionaryView<uint, ListView<ResLuckyDrawRewardForClient>>();
            }
            if (this.m_ExternRewardDic == null)
            {
                this.m_ExternRewardDic = new DictionaryView<uint, ListView<ResDT_LuckyDrawExternReward>>();
            }
            this.m_RewardDic.Clear();
            this.m_ExternRewardDic.Clear();
            DictionaryView<enPayType, ResLuckyDrawPrice>.Enumerator enumerator = GameDataMgr.mallRoulettePriceDict.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<enPayType, ResLuckyDrawPrice> current = enumerator.Current;
                ResLuckyDrawPrice price = current.Value;
                if (price != null)
                {
                    uint dwRewardPoolID = price.dwRewardPoolID;
                    if (targetPoolId != 0)
                    {
                        dwRewardPoolID = targetPoolId;
                    }
                    else
                    {
                        ResDT_LuckyDrawPeriod periodCfg = this.GetPeriodCfg(price);
                        if (periodCfg != null)
                        {
                            dwRewardPoolID = periodCfg.dwRewardPoolID;
                        }
                    }
                    DictionaryView<long, ResLuckyDrawRewardForClient>.Enumerator enumerator2 = GameDataMgr.mallRouletteRewardDict.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        KeyValuePair<long, ResLuckyDrawRewardForClient> pair2 = enumerator2.Current;
                        ResLuckyDrawRewardForClient item = pair2.Value;
                        if ((item != null) && (item.dwRewardPoolID == dwRewardPoolID))
                        {
                            if (!this.m_RewardDic.ContainsKey(price.bMoneyType))
                            {
                                this.m_RewardDic.Add(price.bMoneyType, new ListView<ResLuckyDrawRewardForClient>());
                            }
                            if (!this.m_RewardPoolDic.ContainsKey(price.bMoneyType))
                            {
                                this.m_RewardPoolDic.Add(price.bMoneyType, dwRewardPoolID);
                            }
                            else
                            {
                                uint num2 = 0;
                                if (this.m_RewardPoolDic.TryGetValue(price.bMoneyType, out num2))
                                {
                                    num2 = dwRewardPoolID;
                                }
                            }
                            ListView<ResLuckyDrawRewardForClient> view = new ListView<ResLuckyDrawRewardForClient>();
                            if (this.m_RewardDic.TryGetValue(price.bMoneyType, out view))
                            {
                                view.Add(item);
                            }
                        }
                    }
                    DictionaryView<uint, ResLuckyDrawExternReward>.Enumerator enumerator3 = GameDataMgr.mallRouletteExternRewardDict.GetEnumerator();
                    while (enumerator3.MoveNext())
                    {
                        KeyValuePair<uint, ResLuckyDrawExternReward> pair3 = enumerator3.Current;
                        ResLuckyDrawExternReward reward = pair3.Value;
                        if ((reward != null) && (reward.bMoneyType == price.bMoneyType))
                        {
                            if (!this.m_ExternRewardDic.ContainsKey(price.bMoneyType))
                            {
                                this.m_ExternRewardDic.Add(price.bMoneyType, new ListView<ResDT_LuckyDrawExternReward>());
                            }
                            ListView<ResDT_LuckyDrawExternReward> view2 = new ListView<ResDT_LuckyDrawExternReward>();
                            if (this.m_ExternRewardDic.TryGetValue(price.bMoneyType, out view2))
                            {
                                for (int i = 0; i < reward.bExternCnt; i++)
                                {
                                    ResDT_LuckyDrawExternReward reward2 = reward.astReward[i];
                                    view2.Add(reward2);
                                }
                                if (<>f__am$cache19 == null)
                                {
                                    <>f__am$cache19 = delegate (ResDT_LuckyDrawExternReward a, ResDT_LuckyDrawExternReward b) {
                                        if ((a == null) && (b == null))
                                        {
                                            return 0;
                                        }
                                        if ((a != null) || (b == null))
                                        {
                                            if ((b == null) && (a != null))
                                            {
                                                return 1;
                                            }
                                            if (a.dwDrawCnt < b.dwDrawCnt)
                                            {
                                                return -1;
                                            }
                                            if (a.dwDrawCnt == b.dwDrawCnt)
                                            {
                                                return 0;
                                            }
                                            if (a.dwDrawCnt > b.dwDrawCnt)
                                            {
                                                return 1;
                                            }
                                        }
                                        return -1;
                                    };
                                }
                                view2.Sort(<>f__am$cache19);
                            }
                        }
                    }
                }
            }
        }

        private void RefreshDrawCnt(enPayType payType, out CSDT_LUCKYDRAW_INFO drawInfo, int drawCnt = -1)
        {
            CMallSystem.luckyDrawDic.TryGetValue(payType, out drawInfo);
            CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
            if ((mallForm != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
            {
                Text componetInChild = Utility.GetComponetInChild<Text>(Utility.FindChild(mallForm.get_gameObject(), "pnlBodyBg/pnlRoulette/PL_Premiums"), "Top/Value");
                if (componetInChild != null)
                {
                    if (drawCnt != -1)
                    {
                        componetInChild.set_text(drawCnt.ToString());
                    }
                    else if (drawInfo != null)
                    {
                        componetInChild.set_text(drawInfo.dwCnt.ToString());
                    }
                }
            }
        }

        private void RefreshExternRewards()
        {
            CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
            if ((mallForm != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
            {
                GameObject p = Utility.FindChild(mallForm.get_gameObject(), "pnlBodyBg/pnlRoulette/PL_Premiums");
                ListView<ResDT_LuckyDrawExternReward> view = new ListView<ResDT_LuckyDrawExternReward>();
                CSDT_LUCKYDRAW_INFO drawInfo = new CSDT_LUCKYDRAW_INFO();
                if (p != null)
                {
                    if (Utility.GetComponetInChild<Text>(p, "Top/Value") != null)
                    {
                        switch (this.CurTab)
                        {
                            case Tab.DianQuan:
                                this.RefreshDrawCnt(enPayType.DianQuan, out drawInfo, -1);
                                this.RefreshLuck(enPayType.DianQuan, drawInfo);
                                this.m_ExternRewardDic.TryGetValue(2, out view);
                                break;

                            case Tab.Diamond:
                                this.RefreshDrawCnt(enPayType.Diamond, out drawInfo, -1);
                                this.RefreshLuck(enPayType.Diamond, drawInfo);
                                this.m_ExternRewardDic.TryGetValue(10, out view);
                                break;
                        }
                    }
                    if ((view == null) || (view.Count == 0))
                    {
                        p.CustomSetActive(false);
                    }
                    else
                    {
                        p.CustomSetActive(true);
                        int count = view.Count;
                        for (byte i = 0; i < 5; i = (byte) (i + 1))
                        {
                            GameObject obj3 = Utility.FindChild(p, string.Format("Award{0}", i));
                            if (i < view.Count)
                            {
                                if (obj3 != null)
                                {
                                    obj3.CustomSetActive(true);
                                    string str = Convert.ToString((long) drawInfo.dwReachMask, 2).PadLeft(0x20, '0');
                                    string str2 = Convert.ToString((long) drawInfo.dwDrawMask, 2).PadLeft(0x20, '0');
                                    byte result = 0;
                                    byte num4 = 0;
                                    byte.TryParse(str.Substring(0x20 - (i + 1), 1), out result);
                                    byte.TryParse(str2.Substring(0x20 - (i + 1), 1), out num4);
                                    ResDT_LuckyDrawExternReward reward = view[i];
                                    Text componetInChild = Utility.GetComponetInChild<Text>(obj3, "Value");
                                    Text text3 = Utility.GetComponetInChild<Text>(obj3, "Value/Text");
                                    if (componetInChild != null)
                                    {
                                        componetInChild.set_text(reward.dwDrawCnt.ToString());
                                    }
                                    if (text3 != null)
                                    {
                                        text3.set_text("个");
                                    }
                                    if ((result > 0) && (num4 == 0))
                                    {
                                        CUICommonSystem.PlayAnimator(obj3, "Premiums_Normal");
                                    }
                                    else if ((result > 0) && (num4 > 0))
                                    {
                                        CUICommonSystem.PlayAnimator(obj3, "Premiums_Disbled");
                                        if (componetInChild != null)
                                        {
                                            componetInChild.set_text(string.Empty);
                                        }
                                        if (text3 != null)
                                        {
                                            text3.set_text("已领取");
                                        }
                                    }
                                    else
                                    {
                                        CUICommonSystem.PlayAnimator(obj3, "Premiums_Disbled");
                                    }
                                    CUIEventScript component = obj3.GetComponent<CUIEventScript>();
                                    if (component == null)
                                    {
                                        component = obj3.AddComponent<CUIEventScript>();
                                        component.Initialize(mallForm);
                                    }
                                    stUIEventParams eventParams = new stUIEventParams();
                                    eventParams.tag = i;
                                    component.SetUIEvent(enUIEventType.Click, enUIEventID.Mall_Roulette_Open_Extern_Reward_Tip, eventParams);
                                }
                            }
                            else if (obj3 != null)
                            {
                                obj3.CustomSetActive(false);
                            }
                        }
                    }
                }
            }
        }

        private void RefreshLuck(enPayType payType, CSDT_LUCKYDRAW_INFO drawInfo)
        {
            CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
            if ((mallForm != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
            {
                ResLuckyDrawPrice price = new ResLuckyDrawPrice();
                int dwPreciousHighCnt = 200;
                switch (this.CurTab)
                {
                    case Tab.DianQuan:
                        DebugHelper.Assert(this.m_RewardDic.ContainsKey(2), "夺宝奖励池数据档错误");
                        if (!this.m_RewardDic.ContainsKey(2))
                        {
                            mallForm.Close();
                            return;
                        }
                        GameDataMgr.mallRoulettePriceDict.TryGetValue(enPayType.DianQuan, out price);
                        break;

                    case Tab.Diamond:
                        DebugHelper.Assert(this.m_RewardDic.ContainsKey(10), "夺宝奖励池数据档错误");
                        if (!this.m_RewardDic.ContainsKey(10))
                        {
                            mallForm.Close();
                            return;
                        }
                        GameDataMgr.mallRoulettePriceDict.TryGetValue(enPayType.Diamond, out price);
                        break;
                }
                DebugHelper.Assert(price != null, "商城夺宝配置档有错");
                if (price != null)
                {
                    ResDT_LuckyDrawPeriod periodCfg = this.GetPeriodCfg(price);
                    if (periodCfg != null)
                    {
                        dwPreciousHighCnt = (int) periodCfg.dwPreciousHighCnt;
                    }
                    else
                    {
                        dwPreciousHighCnt = (int) price.dwPreciousHighCnt;
                    }
                }
                int num2 = 0;
                if (drawInfo != null)
                {
                    num2 = (int) Math.Min((long) drawInfo.dwLuckyPoint, (long) dwPreciousHighCnt);
                }
                GameObject obj2 = Utility.FindChild(mallForm.get_gameObject(), "pnlBodyBg/pnlRoulette/Luck/LuckBar");
                GameObject obj3 = Utility.FindChild(mallForm.get_gameObject(), "pnlBodyBg/pnlRoulette/Luck/LuckComplete");
                if (this.m_GotAllUnusualItems)
                {
                    obj2.CustomSetActive(false);
                    Utility.FindChild(mallForm.get_gameObject(), "pnlBodyBg/pnlRoulette/Luck/Effect").CustomSetActive(false);
                    obj3.CustomSetActive(true);
                    if (this.m_IsLuckyBarInited)
                    {
                        Singleton<CSoundManager>.GetInstance().PostEvent("UI_common_gongxi", null);
                    }
                }
                else
                {
                    obj2.CustomSetActive(true);
                    obj3.CustomSetActive(false);
                    double num3 = ((float) num2) / (dwPreciousHighCnt + 40f);
                    Image componetInChild = Utility.GetComponetInChild<Image>(mallForm.get_gameObject(), "pnlBodyBg/pnlRoulette/Luck/LuckBar/BarBg/Image");
                    Image image2 = Utility.GetComponetInChild<Image>(mallForm.get_gameObject(), "pnlBodyBg/pnlRoulette/Luck/LuckBar/BarBg/Bar_Light");
                    Text text = Utility.GetComponetInChild<Text>(mallForm.get_gameObject(), "pnlBodyBg/pnlRoulette/Luck/LuckBar/Text");
                    if ((componetInChild != null) && (text != null))
                    {
                        float num4 = componetInChild.get_fillAmount();
                        componetInChild.set_fillAmount((float) num3);
                        image2.set_fillAmount((float) num3);
                        if (this.m_IsLuckyBarInited && (Math.Abs((float) (num4 - componetInChild.get_fillAmount())) > float.Epsilon))
                        {
                            CUICommonSystem.PlayAnimator(obj2, "BarLight_Anim");
                        }
                        text.set_text(num2.ToString());
                    }
                }
                this.m_IsLuckyBarInited = true;
            }
        }

        public void RefreshRewards()
        {
            if ((Singleton<CMallSystem>.GetInstance().m_MallForm != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
            {
                ResLuckyDrawPrice price = new ResLuckyDrawPrice();
                switch (this.CurTab)
                {
                    case Tab.DianQuan:
                    {
                        DebugHelper.Assert(this.m_RewardDic.ContainsKey(2), "夺宝奖励池数据档错误");
                        if (!this.m_RewardDic.ContainsKey(2))
                        {
                            return;
                        }
                        GameDataMgr.mallRoulettePriceDict.TryGetValue(enPayType.DianQuan, out price);
                        ListView<ResLuckyDrawRewardForClient> view2 = null;
                        this.m_RewardDic.TryGetValue(2, out view2);
                        if ((view2 == null) || (view2.Count == 0))
                        {
                            return;
                        }
                        this.SetRewardItemCells(view2);
                        break;
                    }
                    case Tab.Diamond:
                    {
                        DebugHelper.Assert(this.m_RewardDic.ContainsKey(10), "夺宝奖励池数据档错误");
                        if (!this.m_RewardDic.ContainsKey(10))
                        {
                            return;
                        }
                        GameDataMgr.mallRoulettePriceDict.TryGetValue(enPayType.Diamond, out price);
                        ListView<ResLuckyDrawRewardForClient> view = null;
                        this.m_RewardDic.TryGetValue(10, out view);
                        if ((view == null) || (view.Count == 0))
                        {
                            return;
                        }
                        this.SetRewardItemCells(view);
                        break;
                    }
                }
            }
        }

        private void RefreshTimer()
        {
            CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
            if ((mallForm != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
            {
                ListView<ResDT_LuckyDrawExternReward> view = new ListView<ResDT_LuckyDrawExternReward>();
                switch (this.CurTab)
                {
                    case Tab.DianQuan:
                        this.m_ExternRewardDic.TryGetValue(2, out view);
                        break;

                    case Tab.Diamond:
                        this.m_ExternRewardDic.TryGetValue(10, out view);
                        break;
                }
                GameObject obj2 = Utility.FindChild(mallForm.get_gameObject(), "pnlBodyBg/pnlRoulette/pnlRefresh");
                CUITimerScript componetInChild = Utility.GetComponetInChild<CUITimerScript>(mallForm.get_gameObject(), "pnlBodyBg/pnlRoulette/pnlRefresh/refreshTimer");
                if ((view == null) || (view.Count == 0))
                {
                    if (componetInChild != null)
                    {
                        componetInChild.EndTimer();
                    }
                    obj2.CustomSetActive(false);
                }
                else
                {
                    obj2.CustomSetActive(true);
                    if (componetInChild != null)
                    {
                        componetInChild.SetTotalTime((float) this.GetNextRefreshTime());
                        componetInChild.StartTimer();
                    }
                }
            }
        }

        private void RequestBuyCrystal(uint rareID)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x4a0);
            msg.stPkgData.stRareExchangeReq.dwID = rareID;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public void RequestTimeOut(int seq)
        {
            this.ToggleBtnGroup(true);
        }

        private void SendLotteryMsg(RES_SHOPDRAW_SUBTYPE drawType = 2)
        {
            this.m_LuckyDrawRsp = null;
            if (this.m_RewardList != null)
            {
                this.m_RewardList.Clear();
            }
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x12c1);
            CSPKG_LUCKYDRAW_REQ cspkg_luckydraw_req = new CSPKG_LUCKYDRAW_REQ();
            stPayInfo payInfo = GetPayInfo(drawType, Tab.None);
            if (payInfo.m_payType == enPayType.Diamond)
            {
                payInfo.m_payType = enPayType.DiamondAndDianQuan;
            }
            cspkg_luckydraw_req.bMoneyType = (byte) CMallSystem.PayTypeToResBuyCoinType(payInfo.m_payType);
            cspkg_luckydraw_req.bDrawType = (byte) drawType;
            msg.stPkgData.stLuckyDrawReq = cspkg_luckydraw_req;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
            reqSentTimerSeq = Singleton<CTimerManager>.GetInstance().AddTimer(0x2710, 1, new CTimer.OnTimeUpHandler(this.RequestTimeOut));
        }

        public void SetRewardData(SCPKG_LUCKYDRAW_RSP stLuckyDrawRsp)
        {
            this.m_CurLoops = 0;
            this.m_CurRewardIdx = 0;
            this.m_LuckyDrawRsp = stLuckyDrawRsp;
            this.m_RewardList = this.GetRewardUseables(this.m_LuckyDrawRsp);
        }

        private void SetRewardItemCells(ListView<ResLuckyDrawRewardForClient> rewardList)
        {
            CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
            if ((mallForm != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
            {
                byte num = 0;
                byte num2 = 0;
                for (byte i = 0; i < 14; i = (byte) (i + 1))
                {
                    GameObject p = Utility.FindChild(mallForm.get_gameObject(), string.Format("pnlBodyBg/pnlRoulette/rewardBody/rewardContainer/itemCell{0}", i));
                    if (p != null)
                    {
                        ResLuckyDrawRewardForClient client = rewardList[i];
                        GameObject obj3 = Utility.FindChild(p, "Bg");
                        Image componetInChild = Utility.GetComponetInChild<Image>(p, "icon");
                        GameObject obj4 = Utility.FindChild(p, "tag");
                        GameObject obj5 = Utility.FindChild(p, "XiYou");
                        Image image = Utility.GetComponetInChild<Image>(p, "tag");
                        CUseable useable = CUseableManager.CreateUsableByRandowReward((RES_RANDOM_REWARD_TYPE) client.dwItemType, (int) client.dwItemCnt, client.dwItemID);
                        Text text = Utility.GetComponetInChild<Text>(p, "cntBg/cnt");
                        GameObject obj6 = Utility.FindChild(p, "cntBg");
                        Text text2 = Utility.GetComponetInChild<Text>(p, "Name");
                        GameObject obj7 = Utility.FindChild(p, "imgExperienceMark");
                        GameObject obj8 = Utility.FindChild(p, "probabilityDoubled");
                        CUIEventScript component = p.GetComponent<CUIEventScript>();
                        if (component == null)
                        {
                            component = p.AddComponent<CUIEventScript>();
                            component.Initialize(mallForm);
                        }
                        bool owned = false;
                        if (client.dwItemType == 4)
                        {
                            stUIEventParams eventParams = new stUIEventParams();
                            eventParams.openHeroFormPar.heroId = client.dwItemID;
                            eventParams.openHeroFormPar.openSrc = enHeroFormOpenSrc.HeroBuyClick;
                            component.SetUIEvent(enUIEventType.Click, enUIEventID.HeroInfo_OpenForm, eventParams);
                            component.SetUIEvent(enUIEventType.Down, enUIEventID.None);
                            component.SetUIEvent(enUIEventType.HoldEnd, enUIEventID.None);
                            component.SetUIEvent(enUIEventType.DragEnd, enUIEventID.None);
                            if (CHeroDataFactory.CreateHeroData(client.dwItemID).bPlayerOwn)
                            {
                                owned = true;
                            }
                        }
                        else if (client.dwItemType == 11)
                        {
                            stUIEventParams params2 = new stUIEventParams();
                            uint heroId = 0;
                            uint skinId = 0;
                            CSkinInfo.ResolveHeroSkin(client.dwItemID, out heroId, out skinId);
                            params2.openHeroFormPar.heroId = heroId;
                            params2.openHeroFormPar.skinId = skinId;
                            params2.openHeroFormPar.openSrc = enHeroFormOpenSrc.SkinBuyClick;
                            component.SetUIEvent(enUIEventType.Click, enUIEventID.HeroInfo_OpenForm, params2);
                            component.SetUIEvent(enUIEventType.Down, enUIEventID.None);
                            component.SetUIEvent(enUIEventType.HoldEnd, enUIEventID.None);
                            component.SetUIEvent(enUIEventType.DragEnd, enUIEventID.None);
                            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                            if ((masterRoleInfo != null) && masterRoleInfo.IsHaveHeroSkin(heroId, skinId, false))
                            {
                                owned = true;
                            }
                        }
                        else
                        {
                            stUIEventParams params3 = new stUIEventParams();
                            params3.iconUseable = useable;
                            params3.tag = 0;
                            component.SetUIEvent(enUIEventType.Down, enUIEventID.Tips_ItemInfoOpen, params3);
                            component.SetUIEvent(enUIEventType.HoldEnd, enUIEventID.Tips_ItemInfoClose, params3);
                            component.SetUIEvent(enUIEventType.Click, enUIEventID.Tips_ItemInfoClose, params3);
                            component.SetUIEvent(enUIEventType.DragEnd, enUIEventID.Tips_ItemInfoClose, params3);
                        }
                        if (obj3 != null)
                        {
                            if ((((useable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_HERO) || (useable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN)) || ((useable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP) && CItem.IsHeroExperienceCard(useable.m_baseID))) || ((useable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP) && CItem.IsSkinExperienceCard(useable.m_baseID)))
                            {
                                obj3.CustomSetActive(true);
                            }
                            else
                            {
                                obj3.CustomSetActive(false);
                            }
                        }
                        if ((componetInChild != null) && (useable != null))
                        {
                            componetInChild.SetSprite(useable.GetIconPath(), mallForm, true, false, false, false);
                        }
                        if ((text != null) && (useable != null))
                        {
                            if (useable.m_stackCount <= 1)
                            {
                                obj6.CustomSetActive(false);
                            }
                            else
                            {
                                obj6.CustomSetActive(true);
                                text.set_text(useable.m_stackCount.ToString());
                            }
                        }
                        if ((text2 != null) && (useable != null))
                        {
                            text2.set_text(useable.m_name);
                        }
                        if (client.dwItemTag == 1)
                        {
                            obj5.CustomSetActive(true);
                            if (obj5 != null)
                            {
                                Transform transform = obj5.get_transform().Find("Text");
                                if (transform != null)
                                {
                                    Text text3 = transform.GetComponent<Text>();
                                    if ((text3 != null) && (useable != null))
                                    {
                                        text3.set_text(useable.m_name);
                                    }
                                }
                            }
                            num = (byte) (num + 1);
                            if (owned)
                            {
                                num2 = (byte) (num2 + 1);
                            }
                        }
                        else
                        {
                            obj5.CustomSetActive(false);
                        }
                        string productTagIconPath = CMallSystem.GetProductTagIconPath((int) client.dwItemTag, owned);
                        if (productTagIconPath == null)
                        {
                            obj4.CustomSetActive(false);
                        }
                        else
                        {
                            obj4.CustomSetActive(true);
                            image.SetSprite(productTagIconPath, mallForm, true, false, false, false);
                            Text text4 = Utility.GetComponetInChild<Text>(obj4, "Text");
                            if (text4 != null)
                            {
                                string str2 = string.Empty;
                                switch (client.dwItemTag)
                                {
                                    case 1:
                                    case 4:
                                        str2 = Singleton<CTextManager>.GetInstance().GetText("Common_Tag_Rare");
                                        break;

                                    case 2:
                                        str2 = Singleton<CTextManager>.GetInstance().GetText("Common_Tag_New");
                                        break;

                                    case 3:
                                        str2 = Singleton<CTextManager>.GetInstance().GetText("Common_Tag_Hot");
                                        break;

                                    default:
                                        str2 = string.Empty;
                                        break;
                                }
                                if (owned)
                                {
                                    text4.set_text("拥有");
                                }
                                else
                                {
                                    text4.set_text(str2);
                                }
                            }
                        }
                        if (obj7 != null)
                        {
                            obj7.CustomSetActive(false);
                            Image image3 = obj7.GetComponent<Image>();
                            if (image3 != null)
                            {
                                if (CItem.IsHeroExperienceCard(client.dwItemID))
                                {
                                    obj7.CustomSetActive(true);
                                    image3.SetSprite(CUIUtility.GetSpritePrefeb(CExperienceCardSystem.HeroExperienceCardMarkPath, false, false), false);
                                }
                                else if (CItem.IsSkinExperienceCard(client.dwItemID))
                                {
                                    obj7.CustomSetActive(true);
                                    image3.SetSprite(CUIUtility.GetSpritePrefeb(CExperienceCardSystem.SkinExperienceCardMarkPath, false, false), false);
                                }
                            }
                        }
                        if (client.bShowProbabilityDoubled > 0)
                        {
                            obj8.CustomSetActive(true);
                        }
                        else
                        {
                            obj8.CustomSetActive(false);
                        }
                    }
                }
                if (num == num2)
                {
                    this.m_GotAllUnusualItems = true;
                }
                else
                {
                    this.m_GotAllUnusualItems = false;
                }
            }
        }

        public void ShowHeroSkin(ListView<CUseable> items)
        {
            int count = items.Count;
            if (count != 0)
            {
                uint heroId = 0;
                uint skinId = 0;
                for (int i = 0; i < count; i++)
                {
                    switch (items[i].m_type)
                    {
                        case COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP:
                            if (items[i].ExtraFromType != 1)
                            {
                                break;
                            }
                            CUICommonSystem.ShowNewHeroOrSkin((uint) items[i].ExtraFromData, 0, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO, true, null, enFormPriority.Priority4, (uint) items[i].m_stackCount, 0);
                            goto Label_011E;

                        case COM_ITEM_TYPE.COM_OBJTYPE_HERO:
                            CUICommonSystem.ShowNewHeroOrSkin(items[i].m_baseID, 0, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO, false, null, enFormPriority.Priority4, 0, 0);
                            goto Label_011E;

                        case COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN:
                            CSkinInfo.ResolveHeroSkin(items[i].m_baseID, out heroId, out skinId);
                            CUICommonSystem.ShowNewHeroOrSkin(heroId, skinId, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKIN, false, null, enFormPriority.Priority4, 0, 0);
                            goto Label_011E;

                        default:
                            goto Label_011E;
                    }
                    if (items[i].ExtraFromType == 2)
                    {
                        CSkinInfo.ResolveHeroSkin((uint) items[i].ExtraFromData, out heroId, out skinId);
                        CUICommonSystem.ShowNewHeroOrSkin(heroId, skinId, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKIN, true, null, enFormPriority.Priority4, (uint) items[i].m_stackCount, 0);
                    }
                Label_011E:;
                }
            }
        }

        public void Spin(int idx = -1)
        {
            Singleton<CSoundManager>.GetInstance().PostEvent("Stop_UI_buy_chou_duobao_zhuan", null);
            Singleton<CSoundManager>.GetInstance().PostEvent("UI_buy_chou_duobao_zhuan", null);
            if (idx == -1)
            {
                this.m_CurSpinCnt = 0;
                this.m_CurState = Roulette_State.ACCELERATE;
                this.m_IsClockwise = true;
            }
            CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
            if ((mallForm != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
            {
                float time = 10f;
                CUITimerScript componetInChild = Utility.GetComponetInChild<CUITimerScript>(mallForm.get_gameObject(), "pnlBodyBg/pnlRoulette/timerSpin");
                if (componetInChild != null)
                {
                    componetInChild.SetTotalTime(time);
                    if (idx == -1)
                    {
                        componetInChild.SetOnChangedIntervalTime(0.1f);
                    }
                    componetInChild.m_eventParams[2].tag = idx;
                    componetInChild.m_eventParams[1].tag = idx;
                    componetInChild.StartTimer();
                }
            }
        }

        private enRedID TabToRedID(Tab tab)
        {
            Tab tab2 = tab;
            if (tab2 != Tab.DianQuan)
            {
                if (tab2 == Tab.Diamond)
                {
                    return enRedID.Mall_Roulette_Diamond_Tab;
                }
                return enRedID.Mall_Roulette_Coupons_Tab;
            }
            return enRedID.Mall_Roulette_Coupons_Tab;
        }

        public void ToggleBtnGroup(bool active)
        {
            CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
            if ((mallForm != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
            {
                GameObject obj2 = Utility.FindChild(mallForm.get_gameObject(), "pnlBodyBg/pnlRoulette/Luck/btnGroup");
                GameObject obj3 = Utility.FindChild(mallForm.get_gameObject(), "pnlBodyBg/pnlRoulette/Luck/Effect");
                if (active)
                {
                    obj2.CustomSetActive(true);
                    obj3.CustomSetActive(false);
                }
                else
                {
                    obj2.CustomSetActive(false);
                    if (!this.m_GotAllUnusualItems)
                    {
                        obj3.CustomSetActive(true);
                    }
                }
            }
        }

        public override void UnInit()
        {
            base.UnInit();
            this.m_UsedTabs.Clear();
            this.m_RewardDic.Clear();
            this.m_ExternRewardDic.Clear();
            this.m_RewardPoolDic.Clear();
            this.m_CrySatlDic.Clear();
            CUIEventManager instance = Singleton<CUIEventManager>.GetInstance();
            instance.RemoveUIEventListener(enUIEventID.Mall_Roulette_Tab_Change, new CUIEventManager.OnUIEventHandler(this.OnRouletteTabChange));
            instance.RemoveUIEventListener(enUIEventID.Mall_Roulette_Buy_One, new CUIEventManager.OnUIEventHandler(this.OnRouletteBuyOne));
            instance.RemoveUIEventListener(enUIEventID.Mall_Roulette_Buy_Five, new CUIEventManager.OnUIEventHandler(this.OnRouletteBuyFive));
            instance.RemoveUIEventListener(enUIEventID.Mall_Roulette_Buy_One_Confirm, new CUIEventManager.OnUIEventHandler(this.OnRouletteBuyOneConfirm));
            instance.RemoveUIEventListener(enUIEventID.Mall_Roulette_Buy_Five_Confirm, new CUIEventManager.OnUIEventHandler(this.OnRouletteBuyFiveConfirm));
            instance.RemoveUIEventListener(enUIEventID.Mall_Roulette_Spin_Change, new CUIEventManager.OnUIEventHandler(this.OnSpinInterval));
            instance.RemoveUIEventListener(enUIEventID.Mall_Roulette_Spin_End, new CUIEventManager.OnUIEventHandler(this.OnSpinEnd));
            instance.RemoveUIEventListener(enUIEventID.Mall_Roulette_Open_Extern_Reward_Tip, new CUIEventManager.OnUIEventHandler(this.OnOpenExternRewardTip));
            instance.RemoveUIEventListener(enUIEventID.Mall_Roulette_Draw_Extern_Reward, new CUIEventManager.OnUIEventHandler(this.OnDrawExternReward));
            instance.RemoveUIEventListener(enUIEventID.Mall_Roulette_Reset_Reward_Draw_Cnt, new CUIEventManager.OnUIEventHandler(this.OnDrawCntReset));
            instance.RemoveUIEventListener(enUIEventID.Mall_Roulette_Show_Reward_Item, new CUIEventManager.OnUIEventHandler(this.OnShowRewardItem));
            instance.RemoveUIEventListener(enUIEventID.Mall_Roulette_Show_Rule, new CUIEventManager.OnUIEventHandler(this.OnShowRule));
            instance.RemoveUIEventListener(enUIEventID.Mall_Roulette_Tmp_Reward_Enable, new CUIEventManager.OnUIEventHandler(this.OnTmpRewardEnable));
            instance.RemoveUIEventListener(enUIEventID.Mall_Skip_Animation, new CUIEventManager.OnUIEventHandler(this.OnSkipAnimation));
            instance.RemoveUIEventListener(enUIEventID.Mall_Roulette_Close_Award_Form, new CUIEventManager.OnUIEventHandler(this.OnCloseAwardForm));
            instance.RemoveUIEventListener(enUIEventID.Mall_Crystal_More, new CUIEventManager.OnUIEventHandler(this.OnCrystalMore));
            instance.RemoveUIEventListener(enUIEventID.Mall_Cystal_List_Item_Enable, new CUIEventManager.OnUIEventHandler(this.OnCrystalListEnable));
            instance.RemoveUIEventListener(enUIEventID.Mall_Crystal_On_Exchange, new CUIEventManager.OnUIEventHandler(this.OnCrystalExchange));
            instance.RemoveUIEventListener(enUIEventID.Mall_Crystal_On_Exchange_Confirm, new CUIEventManager.OnUIEventHandler(this.OnCrystalExchangeConfirm));
            instance.RemoveUIEventListener(enUIEventID.Mall_CryStal_On_TabChange, new CUIEventManager.OnUIEventHandler(this.OnCryStalOnTabChange));
            Singleton<EventRouter>.instance.RemoveEventHandler(EventID.Mall_Receive_Roulette_Data, new Action(this, (IntPtr) this.RefreshExternRewards));
            Singleton<EventRouter>.instance.RemoveEventHandler(EventID.Mall_Change_Tab, new Action(this, (IntPtr) this.OnMallTabChange));
            Singleton<EventRouter>.instance.RemoveEventHandler<uint>("HeroAdd", new Action<uint>(this.OnNtyAddHero));
            Singleton<EventRouter>.instance.RemoveEventHandler<uint, uint, uint>("HeroSkinAdd", new Action<uint, uint, uint>(this, (IntPtr) this.OnNtyAddSkin));
        }

        private void UpdateCryStalTab(uint tabID)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(sMallFormCrystal);
            if (form != null)
            {
                int cryStalItemCount = this.GetCryStalItemCount(tabID);
                Transform transform = form.get_transform().Find("Panel/pnlTotalMoney/Cnt");
                if (transform != null)
                {
                    transform.GetComponent<Text>().set_text(cryStalItemCount.ToString());
                }
                Transform transform2 = form.get_transform().Find("Panel/pnlTotalMoney/Image");
                if (transform2 != null)
                {
                    string prefabPath = this.m_tab0ImagePath;
                    if (tabID == 1)
                    {
                        prefabPath = this.m_tab1ImagePath;
                    }
                    Image image = transform2.GetComponent<Image>();
                    if (image != null)
                    {
                        image.SetSprite(prefabPath, form, true, false, false, false);
                    }
                }
                GameObject obj2 = Utility.FindChild(form.get_gameObject(), "Panel/HeroList");
                obj2.CustomSetActive(true);
                CUIListScript component = obj2.GetComponent<CUIListScript>();
                if ((component != null) && (tabID < this.m_CrySatlDic.Count))
                {
                    component.SetElementAmount(this.m_CrySatlDic[tabID].Count + 1);
                }
            }
        }

        public Tab CurTab
        {
            get
            {
                return this.m_CurTab;
            }
            set
            {
                this.m_CurTab = value;
            }
        }

        public enum Roulette_State
        {
            NONE,
            ACCELERATE,
            UNIFORM,
            DECELERATE,
            CONTINUOUS_DRAW,
            SKIP
        }

        public enum Tab
        {
            Diamond = 1,
            DianQuan = 0,
            None = -1
        }
    }
}

