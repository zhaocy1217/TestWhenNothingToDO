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
    public class CMallSystem : Singleton<CMallSystem>
    {
        private GiftCenter _giftCenter = new GiftCenter();
        public bool IsNewHeroShow;
        public static Dictionary<enPayType, CSDT_LUCKYDRAW_INFO> luckyDrawDic = new Dictionary<enPayType, CSDT_LUCKYDRAW_INFO>();
        private List<Tab> m_AvailableTabList;
        private Tab m_CurTab;
        private int m_FreeDrawSymbolTimerSeq = -1;
        public CMallHeroController m_heroMallCtrl = new CMallHeroController();
        public bool m_IsMallFormOpen;
        public CUIFormScript m_MallForm;
        public DateTime m_PlayerRegisterTime = new DateTime();
        public CMallSkinController m_skinMallCtrl = new CMallSkinController();
        private int m_TargetId;
        public static string[] s_payPurposeNameKeys = new string[] { "PayPurpose_Buy", "PayPurpose_Relive", "PayPurpose_GetSymbolGift", "PayPurpose_Roulette", "PayPurpose_Open", "PayPurpose_Chat", "PayPurpose_RecommendLottery" };
        public static string[] s_payTypeNameKeys = new string[] { "PayType_NotSupport", "PayType_GoldCoin", "PayType_DianQuan", "PayType_Diamond", "PayType_Diamond", "PayType_BurningCoin", "PayType_ArenaCoin", "PayType_GuildCoin", "PayType_SymbolCoin" };
        public string sMallFormPath = "UGUI/Form/System/Mall/Form_Mall.prefab";

        private static stPayInfoSet CalLowestPayInfoSet(params stPayInfoSet[] values)
        {
            Dictionary<enPayType, stPayInfo> dictionary = new Dictionary<enPayType, stPayInfo>();
            for (int i = 0; i < values.Length; i++)
            {
                stPayInfoSet set = values[i];
                for (int j = 0; j < set.m_payInfoCount; j++)
                {
                    if (set.m_payInfos[j].m_payValue != 0)
                    {
                        stPayInfo info = new stPayInfo();
                        if (dictionary.TryGetValue(set.m_payInfos[j].m_payType, out info))
                        {
                            if (set.m_payInfos[j].m_payValue < info.m_payValue)
                            {
                                dictionary[info.m_payType] = set.m_payInfos[j];
                            }
                        }
                        else
                        {
                            dictionary.Add(set.m_payInfos[j].m_payType, set.m_payInfos[j]);
                        }
                    }
                }
            }
            stPayInfoSet set2 = new stPayInfoSet(dictionary.Count);
            set2.m_payInfoCount = dictionary.Count;
            dictionary.Values.CopyTo(set2.m_payInfos, 0);
            return set2;
        }

        public static uint GetCurrencyValueFromRoleInfo(CRoleInfo roleInfo, enPayType payType)
        {
            switch (payType)
            {
                case enPayType.GoldCoin:
                    return roleInfo.GoldCoin;

                case enPayType.DianQuan:
                    return (uint) roleInfo.DianQuan;

                case enPayType.Diamond:
                    return roleInfo.Diamond;

                case enPayType.DiamondAndDianQuan:
                    return (roleInfo.Diamond + ((uint) roleInfo.DianQuan));

                case enPayType.BurningCoin:
                    return roleInfo.BurningCoin;

                case enPayType.ArenaCoin:
                    return roleInfo.ArenaCoin;

                case enPayType.GuildCoin:
                    return CGuildHelper.GetPlayerGuildConstruct();

                case enPayType.SymbolCoin:
                    return roleInfo.SymbolCoin;
            }
            return 0;
        }

        public static stPayInfoSet GetPayInfoSetOfGood(ResHeroCfgInfo resHeroCfgInfo)
        {
            ResHeroShop shop = null;
            GameDataMgr.heroShopInfoDict.TryGetValue(resHeroCfgInfo.dwCfgID, out shop);
            ResDT_RegisterSale_Info stRegisterSale = shop.stRegisterSale;
            bool flag = IsinRegisterSales(stRegisterSale);
            uint goldCoinValue = (shop == null) ? 1 : shop.dwBuyCoin;
            uint dianquanValue = (shop == null) ? 1 : shop.dwBuyCoupons;
            uint diamondValue = (shop == null) ? 1 : shop.dwBuyDiamond;
            if (flag && (shop != null))
            {
                if (stRegisterSale.dwBuyCoin < goldCoinValue)
                {
                    goldCoinValue = stRegisterSale.dwBuyCoin;
                }
                if (stRegisterSale.dwBuyCoupons < dianquanValue)
                {
                    dianquanValue = stRegisterSale.dwBuyCoupons;
                }
                if (stRegisterSale.dwBuyDiamond < diamondValue)
                {
                    diamondValue = stRegisterSale.dwBuyDiamond;
                }
            }
            return GetPayInfoSetOfGood((shop != null) && (shop.bIsBuyCoin > 0), goldCoinValue, (shop == null) ? 1 : shop.dwBuyCoin, (shop != null) && (shop.bIsBuyCoupons > 0), dianquanValue, (shop == null) ? 1 : shop.dwBuyCoupons, (shop != null) && (shop.bIsBuyDiamond > 0), diamondValue, (shop == null) ? 1 : shop.dwBuyDiamond, 0x2710);
        }

        public static stPayInfoSet GetPayInfoSetOfGood(ResHeroSkin resHeroSkin)
        {
            ResHeroSkinShop shop = null;
            GameDataMgr.skinShopInfoDict.TryGetValue(resHeroSkin.dwID, out shop);
            ResDT_RegisterSale_Info stRegisterSale = shop.stRegisterSale;
            bool flag = IsinRegisterSales(stRegisterSale);
            uint dianquanValue = (shop == null) ? 1 : shop.dwBuyCoupons;
            uint diamondValue = (shop == null) ? 1 : shop.dwBuyDiamond;
            if (flag && (shop != null))
            {
                if (stRegisterSale.dwBuyCoupons < dianquanValue)
                {
                    dianquanValue = stRegisterSale.dwBuyCoupons;
                }
                if (stRegisterSale.dwBuyDiamond < diamondValue)
                {
                    diamondValue = stRegisterSale.dwBuyDiamond;
                }
            }
            return GetPayInfoSetOfGood(false, 0, 0, (shop != null) && (shop.bIsBuyCoupons > 0), dianquanValue, (shop == null) ? 1 : shop.dwBuyCoupons, (shop != null) && (shop.bIsBuyDiamond > 0), diamondValue, (shop == null) ? 1 : shop.dwBuyDiamond, 0x2710);
        }

        public static stPayInfoSet GetPayInfoSetOfGood(ResHeroCfgInfo resHeroCfgInfo, ResHeroPromotion resPromotion)
        {
            ResHeroShop shop = null;
            GameDataMgr.heroShopInfoDict.TryGetValue(resHeroCfgInfo.dwCfgID, out shop);
            ResDT_RegisterSale_Info stRegisterSale = shop.stRegisterSale;
            bool flag = IsinRegisterSales(stRegisterSale);
            if (resPromotion != null)
            {
                uint dwBuyCoin = resPromotion.dwBuyCoin;
                uint dwBuyCoupons = resPromotion.dwBuyCoupons;
                uint dwBuyDiamond = resPromotion.dwBuyDiamond;
                if (flag)
                {
                    if (stRegisterSale.dwBuyCoin < dwBuyCoin)
                    {
                        dwBuyCoin = stRegisterSale.dwBuyCoin;
                    }
                    if (stRegisterSale.dwBuyCoupons < dwBuyCoupons)
                    {
                        dwBuyCoupons = stRegisterSale.dwBuyCoupons;
                    }
                    if (stRegisterSale.dwBuyDiamond < dwBuyDiamond)
                    {
                        dwBuyDiamond = stRegisterSale.dwBuyDiamond;
                    }
                }
                return GetPayInfoSetOfGood(resPromotion.bIsBuyCoin > 0, dwBuyCoin, (shop == null) ? 1 : shop.dwBuyCoin, resPromotion.bIsBuyCoupons > 0, dwBuyCoupons, (shop == null) ? 1 : shop.dwBuyCoupons, resPromotion.bIsBuyDiamond > 0, dwBuyDiamond, (shop == null) ? 1 : shop.dwBuyDiamond, 0x2710);
            }
            uint goldCoinValue = (shop == null) ? 1 : shop.dwBuyCoin;
            uint dianquanValue = (shop == null) ? 1 : shop.dwBuyCoupons;
            uint diamondValue = (shop == null) ? 1 : shop.dwBuyDiamond;
            if (flag && (shop != null))
            {
                if (stRegisterSale.dwBuyCoin < goldCoinValue)
                {
                    goldCoinValue = stRegisterSale.dwBuyCoin;
                }
                if (stRegisterSale.dwBuyCoupons < dianquanValue)
                {
                    dianquanValue = stRegisterSale.dwBuyCoupons;
                }
                if (stRegisterSale.dwBuyDiamond < diamondValue)
                {
                    diamondValue = stRegisterSale.dwBuyDiamond;
                }
            }
            return GetPayInfoSetOfGood((shop != null) && (shop.bIsBuyCoin > 0), goldCoinValue, (shop == null) ? 1 : shop.dwBuyCoin, (shop != null) && (shop.bIsBuyCoupons > 0), dianquanValue, (shop == null) ? 1 : shop.dwBuyCoupons, (shop != null) && (shop.bIsBuyDiamond > 0), diamondValue, (shop == null) ? 1 : shop.dwBuyDiamond, 0x2710);
        }

        public static stPayInfoSet GetPayInfoSetOfGood(ResHeroSkin resHeroSkin, ResSkinPromotion resPromotion)
        {
            ResHeroSkinShop shop = null;
            GameDataMgr.skinShopInfoDict.TryGetValue(resHeroSkin.dwID, out shop);
            ResDT_RegisterSale_Info stRegisterSale = shop.stRegisterSale;
            bool flag = IsinRegisterSales(stRegisterSale);
            if (resPromotion != null)
            {
                uint dwBuyCoupons = resPromotion.dwBuyCoupons;
                uint dwBuyDiamond = resPromotion.dwBuyDiamond;
                if (flag)
                {
                    if (stRegisterSale.dwBuyCoupons < dwBuyCoupons)
                    {
                        dwBuyCoupons = stRegisterSale.dwBuyCoupons;
                    }
                    if (stRegisterSale.dwBuyDiamond < dwBuyDiamond)
                    {
                        dwBuyDiamond = stRegisterSale.dwBuyDiamond;
                    }
                }
                return GetPayInfoSetOfGood(false, 0, 0, resPromotion.bIsBuyCoupons > 0, dwBuyCoupons, (shop == null) ? 1 : shop.dwBuyCoupons, resPromotion.bIsBuyDiamond > 0, dwBuyDiamond, (shop == null) ? 1 : shop.dwBuyDiamond, 0x2710);
            }
            uint dianquanValue = (shop == null) ? 1 : shop.dwBuyCoupons;
            uint diamondValue = (shop == null) ? 1 : shop.dwBuyDiamond;
            if (flag && (shop != null))
            {
                if (stRegisterSale.dwBuyCoupons < dianquanValue)
                {
                    dianquanValue = stRegisterSale.dwBuyCoupons;
                }
                if (stRegisterSale.dwBuyDiamond < diamondValue)
                {
                    diamondValue = stRegisterSale.dwBuyDiamond;
                }
            }
            return GetPayInfoSetOfGood(false, 0, 0, (shop != null) && (shop.bIsBuyCoupons > 0), dianquanValue, (shop == null) ? 1 : shop.dwBuyCoupons, (shop != null) && (shop.bIsBuyDiamond > 0), diamondValue, (shop == null) ? 1 : shop.dwBuyDiamond, 0x2710);
        }

        public static stPayInfoSet GetPayInfoSetOfGood(bool canUseGoldCoin, uint goldCoinValue, bool canUseDianQuan, uint dianquanValue, bool canUseDiamond, uint diamondValue, uint discount = 0x2710)
        {
            stPayInfoSet set = new stPayInfoSet(2);
            if (canUseGoldCoin)
            {
                set.m_payInfos[set.m_payInfoCount].m_payType = enPayType.GoldCoin;
                set.m_payInfos[set.m_payInfoCount].m_payValue = goldCoinValue;
                set.m_payInfos[set.m_payInfoCount].m_oriValue = goldCoinValue;
                set.m_payInfos[set.m_payInfoCount].m_discountForDisplay = 0x2710;
                set.m_payInfoCount++;
            }
            if (canUseDianQuan || canUseDiamond)
            {
                if (canUseDianQuan && !canUseDiamond)
                {
                    set.m_payInfos[set.m_payInfoCount].m_payType = enPayType.DianQuan;
                    set.m_payInfos[set.m_payInfoCount].m_payValue = dianquanValue;
                    set.m_payInfos[set.m_payInfoCount].m_oriValue = dianquanValue;
                    set.m_payInfos[set.m_payInfoCount].m_discountForDisplay = 0x2710;
                }
                else if (!canUseDianQuan && canUseDiamond)
                {
                    set.m_payInfos[set.m_payInfoCount].m_payType = enPayType.Diamond;
                    set.m_payInfos[set.m_payInfoCount].m_payValue = diamondValue;
                    set.m_payInfos[set.m_payInfoCount].m_oriValue = diamondValue;
                    set.m_payInfos[set.m_payInfoCount].m_discountForDisplay = 0x2710;
                }
                else if (canUseDianQuan && canUseDiamond)
                {
                    set.m_payInfos[set.m_payInfoCount].m_payType = enPayType.DiamondAndDianQuan;
                    set.m_payInfos[set.m_payInfoCount].m_payValue = diamondValue;
                    set.m_payInfos[set.m_payInfoCount].m_oriValue = diamondValue;
                    set.m_payInfos[set.m_payInfoCount].m_discountForDisplay = 0x2710;
                }
                set.m_payInfoCount++;
            }
            if (discount < 0x2710)
            {
                for (int i = 0; i < set.m_payInfoCount; i++)
                {
                    set.m_payInfos[i].m_payValue = (set.m_payInfos[i].m_payValue * discount) / 0x2710;
                    set.m_payInfos[i].m_discountForDisplay = discount;
                }
            }
            return set;
        }

        public static stPayInfoSet GetPayInfoSetOfGood(bool canUseGoldCoin, uint goldCoinValue, uint oriGoldCoinValue, bool canUseDianQuan, uint dianquanValue, uint oriDianquanValue, bool canUseDiamond, uint diamondValue, uint oriDiamondValue, uint discount = 0x2710)
        {
            stPayInfoSet set = new stPayInfoSet(2);
            if (canUseGoldCoin)
            {
                set.m_payInfos[set.m_payInfoCount].m_payType = enPayType.GoldCoin;
                set.m_payInfos[set.m_payInfoCount].m_payValue = goldCoinValue;
                set.m_payInfos[set.m_payInfoCount].m_oriValue = oriGoldCoinValue;
                set.m_payInfos[set.m_payInfoCount].m_discountForDisplay = discount / 0x3e8;
                set.m_payInfoCount++;
            }
            if (canUseDianQuan || canUseDiamond)
            {
                if (canUseDianQuan && !canUseDiamond)
                {
                    set.m_payInfos[set.m_payInfoCount].m_payType = enPayType.DianQuan;
                    set.m_payInfos[set.m_payInfoCount].m_payValue = dianquanValue;
                    set.m_payInfos[set.m_payInfoCount].m_oriValue = oriDianquanValue;
                    set.m_payInfos[set.m_payInfoCount].m_discountForDisplay = discount / 0x3e8;
                }
                else if (!canUseDianQuan && canUseDiamond)
                {
                    set.m_payInfos[set.m_payInfoCount].m_payType = enPayType.Diamond;
                    set.m_payInfos[set.m_payInfoCount].m_payValue = diamondValue;
                    set.m_payInfos[set.m_payInfoCount].m_oriValue = oriDiamondValue;
                    set.m_payInfos[set.m_payInfoCount].m_discountForDisplay = discount / 0x3e8;
                }
                else if (canUseDianQuan && canUseDiamond)
                {
                    set.m_payInfos[set.m_payInfoCount].m_payType = enPayType.DiamondAndDianQuan;
                    set.m_payInfos[set.m_payInfoCount].m_payValue = diamondValue;
                    set.m_payInfos[set.m_payInfoCount].m_oriValue = oriDiamondValue;
                    set.m_payInfos[set.m_payInfoCount].m_discountForDisplay = discount / 0x3e8;
                }
                set.m_payInfoCount++;
            }
            return set;
        }

        public static string GetPayTypeIconPath(enPayType payType)
        {
            switch (payType)
            {
                case enPayType.GoldCoin:
                    return "UGUI/Sprite/Common/GoldCoin.prefab";

                case enPayType.DianQuan:
                    return "UGUI/Sprite/Common/DianQuan.prefab";

                case enPayType.Diamond:
                case enPayType.DiamondAndDianQuan:
                    return "UGUI/Sprite/Common/Diamond.prefab";
            }
            return null;
        }

        public static string GetPayTypeText(enPayType payType)
        {
            switch (payType)
            {
                case enPayType.GoldCoin:
                    return Singleton<CTextManager>.GetInstance().GetText("Shop_Money_Type_GoldCoin");

                case enPayType.DianQuan:
                    return Singleton<CTextManager>.GetInstance().GetText("Shop_Money_Type_DianQuan");

                case enPayType.Diamond:
                case enPayType.DiamondAndDianQuan:
                    return Singleton<CTextManager>.GetInstance().GetText("Shop_Money_Type_Diamond");
            }
            return null;
        }

        public static string GetPriceTypeBuyString(enPayType payType)
        {
            switch (payType)
            {
                case enPayType.GoldCoin:
                    return Singleton<CTextManager>.GetInstance().GetText("Pay_As_GoldCoin");

                case enPayType.DianQuan:
                    return Singleton<CTextManager>.GetInstance().GetText("Pay_As_DianQuan");

                case enPayType.Diamond:
                case enPayType.DiamondAndDianQuan:
                    return Singleton<CTextManager>.GetInstance().GetText("Pay_As_Diamond");
            }
            return string.Empty;
        }

        public static string GetProductTagIconPath(int tagType, bool owned = false)
        {
            if (owned)
            {
                return "UGUI/Sprite/Common/Product_New.prefab";
            }
            switch (((RES_LUCKYDRAW_ITEMTAG) tagType))
            {
                case RES_LUCKYDRAW_ITEMTAG.RES_LUCKYDRAW_ITEMTAG_UNUSUAL:
                    return "UGUI/Sprite/Common/Product_Unusual.prefab";

                case RES_LUCKYDRAW_ITEMTAG.RES_LUCKYDRAW_ITEMTAG_NEW:
                    return "UGUI/Sprite/Common/Product_New.prefab";

                case RES_LUCKYDRAW_ITEMTAG.RES_LUCKYDRAW_ITEMTAG_HOT:
                    return "UGUI/Sprite/Common/Product_Hot.prefab";

                case RES_LUCKYDRAW_ITEMTAG.RES_LUCKYDRAW_ITEMTAG_DISCOUNT:
                    return "UGUI/Sprite/Common/Product_Discount.prefab";
            }
            return null;
        }

        public static string GetRegisterSalesHeroDay(ref ResHeroPromotion heroPromotion, ResHeroShop heroShop)
        {
            string text = null;
            ResDT_RegisterSale_Info stRegisterSale = heroShop.stRegisterSale;
            bool flag = IsinRegisterSales(stRegisterSale);
            if (!flag)
            {
                return text;
            }
            bool flag2 = true;
            if (heroPromotion != null)
            {
                uint dwBuyCoin = heroPromotion.dwBuyCoin;
                uint dwBuyCoupons = heroPromotion.dwBuyCoupons;
                uint dwBuyDiamond = heroPromotion.dwBuyDiamond;
                if (flag)
                {
                    if (stRegisterSale.dwBuyCoin > dwBuyCoin)
                    {
                        flag2 = false;
                    }
                    if (stRegisterSale.dwBuyCoupons > dwBuyCoupons)
                    {
                        flag2 = false;
                    }
                    if (stRegisterSale.dwBuyDiamond > dwBuyDiamond)
                    {
                        flag2 = false;
                    }
                }
            }
            else
            {
                uint num4 = (heroShop == null) ? 1 : heroShop.dwBuyCoin;
                uint num5 = (heroShop == null) ? 1 : heroShop.dwBuyCoupons;
                uint num6 = (heroShop == null) ? 1 : heroShop.dwBuyDiamond;
                if (flag && (heroShop != null))
                {
                    if (stRegisterSale.dwBuyCoin > num4)
                    {
                        flag2 = false;
                    }
                    if (stRegisterSale.dwBuyCoupons > num5)
                    {
                        flag2 = false;
                    }
                    if (stRegisterSale.dwBuyDiamond > num6)
                    {
                        flag2 = false;
                    }
                }
            }
            if (flag2)
            {
                DateTime time = Singleton<CMallSystem>.GetInstance().m_PlayerRegisterTime.AddSeconds((double) stRegisterSale.dwValidTime);
                DateTime time2 = Utility.ToUtcTime2Local((long) CRoleInfo.GetCurrentUTCTime());
                TimeSpan span = (TimeSpan) (time - time2);
                int num7 = (int) Math.Ceiling((double) (span.TotalSeconds / 86400.0));
                if (num7 > 0)
                {
                    string[] args = new string[] { num7.ToString() };
                    text = Singleton<CTextManager>.GetInstance().GetText("Mall_Promotion_Tag_1", args);
                }
                return text;
            }
            return null;
        }

        public static string GetRegisterSalesSkinDay(ref ResSkinPromotion heroPromotion, ResHeroSkinShop heroShop)
        {
            string text = null;
            ResDT_RegisterSale_Info stRegisterSale = heroShop.stRegisterSale;
            bool flag = IsinRegisterSales(stRegisterSale);
            if (!flag)
            {
                return text;
            }
            bool flag2 = true;
            if (heroPromotion != null)
            {
                uint dwBuyCoupons = heroPromotion.dwBuyCoupons;
                uint dwBuyDiamond = heroPromotion.dwBuyDiamond;
                if (flag)
                {
                    if (stRegisterSale.dwBuyCoupons > dwBuyCoupons)
                    {
                        flag2 = false;
                    }
                    if (stRegisterSale.dwBuyDiamond > dwBuyDiamond)
                    {
                        flag2 = false;
                    }
                }
            }
            else
            {
                uint num3 = (heroShop == null) ? 1 : heroShop.dwBuyCoupons;
                uint num4 = (heroShop == null) ? 1 : heroShop.dwBuyDiamond;
                if (flag && (heroShop != null))
                {
                    if (stRegisterSale.dwBuyCoupons > num3)
                    {
                        flag2 = false;
                    }
                    if (stRegisterSale.dwBuyDiamond > num4)
                    {
                        flag2 = false;
                    }
                }
            }
            if (flag2)
            {
                DateTime time = Singleton<CMallSystem>.GetInstance().m_PlayerRegisterTime.AddSeconds((double) stRegisterSale.dwValidTime);
                DateTime time2 = Utility.ToUtcTime2Local((long) CRoleInfo.GetCurrentUTCTime());
                TimeSpan span = (TimeSpan) (time - time2);
                int num5 = (int) Math.Ceiling((double) (span.TotalSeconds / 86400.0));
                if (num5 > 0)
                {
                    string[] args = new string[] { num5.ToString() };
                    text = Singleton<CTextManager>.GetInstance().GetText("Mall_Promotion_Tag_1", args);
                }
                return text;
            }
            return null;
        }

        public static ResShopPromotion GetShopPromotion(RES_SHOPBUY_TYPE type, RES_SHOPDRAW_SUBTYPE subType)
        {
            uint shopInfoCfgId = CPurchaseSys.GetShopInfoCfgId(type, (int) subType);
            if ((GameDataMgr.shopPromotionDict != null) && GameDataMgr.shopPromotionDict.ContainsKey(shopInfoCfgId))
            {
                ListView<ResShopPromotion> view = new ListView<ResShopPromotion>();
                if (GameDataMgr.shopPromotionDict.TryGetValue(shopInfoCfgId, out view))
                {
                    int currentUTCTime = CRoleInfo.GetCurrentUTCTime();
                    if (view != null)
                    {
                        for (int i = 0; i < view.Count; i++)
                        {
                            ResShopPromotion promotion = view[i];
                            if ((promotion.dwOnTimeGen <= currentUTCTime) && (currentUTCTime < promotion.dwOffTimeGen))
                            {
                                return promotion;
                            }
                        }
                    }
                    return null;
                }
            }
            return null;
        }

        public int GetTabIndex(Tab tab)
        {
            <GetTabIndex>c__AnonStorey61 storey = new <GetTabIndex>c__AnonStorey61();
            storey.tab = tab;
            return this.m_AvailableTabList.FindIndex(new Predicate<Tab>(storey.<>m__62));
        }

        public bool HasFreeDrawCnt()
        {
            return false;
        }

        public bool HasFreeDrawCnt(Tab tab)
        {
            return false;
        }

        public bool HasFreeDrawCnt(enRedID redID)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                if (redID == enRedID.Mall_SymbolTab)
                {
                    int index = 4;
                    if (masterRoleInfo.m_freeDrawInfo[index].dwLeftFreeDrawCnt > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override void Init()
        {
            base.Init();
            if (this.m_heroMallCtrl == null)
            {
                this.m_heroMallCtrl = new CMallHeroController();
            }
            this.m_heroMallCtrl.Init();
            if (this.m_skinMallCtrl == null)
            {
                this.m_skinMallCtrl = new CMallSkinController();
            }
            this.m_skinMallCtrl.Init();
            this.m_AvailableTabList = new List<Tab>(8);
            CUIEventManager instance = Singleton<CUIEventManager>.GetInstance();
            instance.AddUIEventListener(enUIEventID.Mall_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnMallOpenForm));
            instance.AddUIEventListener(enUIEventID.Mall_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnMallCloseForm));
            instance.AddUIEventListener(enUIEventID.Mall_Mall_Tab_Change, new CUIEventManager.OnUIEventHandler(this.OnMallTabChange));
            instance.AddUIEventListener(enUIEventID.Mall_GoToBoutiqueTab, new CUIEventManager.OnUIEventHandler(this.OnMallGotoBoutique));
            instance.AddUIEventListener(enUIEventID.HeroInfo_GotoMall, new CUIEventManager.OnUIEventHandler(this.OnMalOpenHeroTab));
            instance.AddUIEventListener(enUIEventID.Mall_Open_Factory_Shop_Tab, new CUIEventManager.OnUIEventHandler(this.OnMallOpenFactoryShopTab));
            instance.AddUIEventListener(enUIEventID.Mall_GoToSymbolTab, new CUIEventManager.OnUIEventHandler(this.OnMallGotoSymbolTab));
            instance.AddUIEventListener(enUIEventID.Mall_GoToMysteryTab, new CUIEventManager.OnUIEventHandler(this.OnMallGoToMysteryTab));
            instance.AddUIEventListener(enUIEventID.Mall_GoToSkinTab, new CUIEventManager.OnUIEventHandler(this.OnMallGotoSkinTab));
            instance.AddUIEventListener(enUIEventID.Mall_GoToTreasureTab, new CUIEventManager.OnUIEventHandler(this.OnMallGoToTreasureTab));
            instance.AddUIEventListener(enUIEventID.Mall_GotoCouponsTreasureTab, new CUIEventManager.OnUIEventHandler(this.OnMallGotoCouponsTreasureTab));
            instance.AddUIEventListener(enUIEventID.Mall_GotoDianmondTreasureTab, new CUIEventManager.OnUIEventHandler(this.OnMallGotoDiamondTreasureTab));
            instance.AddUIEventListener(enUIEventID.Mall_GoToRecommendHeroTab, new CUIEventManager.OnUIEventHandler(this.OnMallGoToRecommendHeroTab));
            instance.AddUIEventListener(enUIEventID.Mall_GoToRecommendSkinTab, new CUIEventManager.OnUIEventHandler(this.OnMallGoToRecommendSkinTab));
            instance.AddUIEventListener(enUIEventID.Mall_Action_Mask_Reset, new CUIEventManager.OnUIEventHandler(this.OnMallActionMaskReset));
            instance.AddUIEventListener(enUIEventID.Mall_Skip_Mask_Reset, new CUIEventManager.OnUIEventHandler(this.OnMallSkipMaskReset));
            instance.AddUIEventListener(enUIEventID.Mall_Update_Sub_Module, new CUIEventManager.OnUIEventHandler(this.OnUpdateSubModule));
            instance.AddUIEventListener(enUIEventID.Mall_Sort_Type_Select, new CUIEventManager.OnUIEventHandler(this.OnSelectSortType));
            instance.AddUIEventListener(enUIEventID.Mall_Sort_Type_Change, new CUIEventManager.OnUIEventHandler(this.OnSortTypeChange));
            instance.AddUIEventListener(enUIEventID.Mall_Buy_Product_Confirm, new CUIEventManager.OnUIEventHandler(this.OnConfirmBuyProduct));
            Singleton<EventRouter>.instance.AddEventHandler(EventID.Mall_Set_Free_Draw_Timer, new Action(this, (IntPtr) this.SetFreeDrawTimer));
            Singleton<EventRouter>.instance.AddEventHandler(EventID.Mall_Sub_Module_Loaded, new Action(this, (IntPtr) this.OnSubModuleLoaded));
            Singleton<EventRouter>.instance.AddEventHandler<Tab>(EventID.Mall_Refresh_Tab_Red_Dot, new Action<Tab>(this.RefreshTabRedDot));
            this.m_MallForm = null;
            this.m_IsMallFormOpen = false;
            this.m_CurTab = Tab.None;
            this.m_TargetId = 0;
            this._giftCenter.Init();
        }

        private void InitTab()
        {
            this.SetAvailableTabs();
            string[] strArray = new string[this.m_AvailableTabList.Count];
            for (byte i = 0; i < strArray.Length; i = (byte) (i + 1))
            {
                switch (this.m_AvailableTabList[i])
                {
                    case Tab.Mystery:
                        strArray[i] = Singleton<CTextManager>.GetInstance().GetText("Mall_Tab_Mystery");
                        break;

                    case Tab.Boutique:
                        strArray[i] = Singleton<CTextManager>.GetInstance().GetText("Mall_Tab_Boutique");
                        break;

                    case Tab.Recommend:
                        strArray[i] = Singleton<CTextManager>.GetInstance().GetText("Mall_Tab_New");
                        break;

                    case Tab.Hero:
                        strArray[i] = Singleton<CTextManager>.GetInstance().GetText("Mall_Hero_Buy_Tab");
                        break;

                    case Tab.Skin:
                        strArray[i] = Singleton<CTextManager>.GetInstance().GetText("Mall_Skin_Buy_Tab");
                        break;

                    case Tab.Symbol_Make:
                        strArray[i] = Singleton<CTextManager>.GetInstance().GetText("Mall_Tab_Symbol_Gifts");
                        break;

                    case Tab.Factory_Shop:
                        strArray[i] = Singleton<CTextManager>.GetInstance().GetText("Mall_Tab_Factory_Shop");
                        break;

                    case Tab.Roulette:
                        strArray[i] = Singleton<CTextManager>.GetInstance().GetText("Mall_Tab_Roulette");
                        break;
                }
            }
            GameObject widget = this.m_MallForm.GetWidget(0);
            CUIListScript script = (widget == null) ? null : widget.GetComponent<CUIListScript>();
            if (script != null)
            {
                CUIListElementScript elemenet = null;
                script.SetElementAmount(strArray.Length);
                for (int j = 0; j < script.m_elementAmount; j++)
                {
                    elemenet = script.GetElemenet(j);
                    if (elemenet != null)
                    {
                        elemenet.get_gameObject().get_transform().Find("Text").GetComponent<Text>().set_text(strArray[j]);
                        GameObject obj3 = Utility.FindChild(elemenet.get_gameObject(), "tag");
                        obj3.CustomSetActive(false);
                        if (obj3 != null)
                        {
                            if (((Tab) this.m_AvailableTabList[j]) == Tab.Mystery)
                            {
                                Text componetInChild = Utility.GetComponetInChild<Text>(obj3, "Text");
                                if (componetInChild != null)
                                {
                                    componetInChild.set_text(Singleton<CTextManager>.GetInstance().GetText("RES_WEAL_COLORBAR_TYPE_LIMIT"));
                                    obj3.CustomSetActive(true);
                                }
                            }
                            if ((((Tab) this.m_AvailableTabList[j]) == Tab.Symbol_Make) && (((GetShopPromotion(RES_SHOPBUY_TYPE.RES_BUYTYPE_DRAWSYMBOLCOMMON, RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_ONE) != null) || (GetShopPromotion(RES_SHOPBUY_TYPE.RES_BUYTYPE_DRAWSYMBOLCOMMON, RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_FIVE) != null)) || ((GetShopPromotion(RES_SHOPBUY_TYPE.RES_BUYTYPE_DRAWSYMBOLSENIOR, RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_ONE) != null) || (GetShopPromotion(RES_SHOPBUY_TYPE.RES_BUYTYPE_DRAWSYMBOLSENIOR, RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_FIVE) != null))))
                            {
                                Text text3 = Utility.GetComponetInChild<Text>(obj3, "Text");
                                if (text3 != null)
                                {
                                    text3.set_text(Singleton<CTextManager>.GetInstance().GetText("RES_WEAL_COLORBAR_TYPE_DISCOUNT"));
                                    obj3.CustomSetActive(true);
                                }
                            }
                            if (((Tab) this.m_AvailableTabList[j]) == Tab.Roulette)
                            {
                                stPayInfo payInfo = CMallRouletteController.GetPayInfo(RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_ONE, CMallRouletteController.Tab.Diamond);
                                stPayInfo info2 = CMallRouletteController.GetPayInfo(RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_FIVE, CMallRouletteController.Tab.Diamond);
                                stPayInfo info3 = CMallRouletteController.GetPayInfo(RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_ONE, CMallRouletteController.Tab.DianQuan);
                                stPayInfo info4 = CMallRouletteController.GetPayInfo(RES_SHOPDRAW_SUBTYPE.RES_DRAWSUBTYPE_FIVE, CMallRouletteController.Tab.DianQuan);
                                if (((payInfo.m_payValue < payInfo.m_oriValue) || (info2.m_payValue < info2.m_oriValue)) || ((info3.m_payValue < info3.m_oriValue) || (info4.m_payValue < info4.m_oriValue)))
                                {
                                    Text text4 = Utility.GetComponetInChild<Text>(obj3, "Text");
                                    if (text4 != null)
                                    {
                                        text4.set_text(Singleton<CTextManager>.GetInstance().GetText("RES_WEAL_COLORBAR_TYPE_DISCOUNT"));
                                        obj3.CustomSetActive(true);
                                    }
                                }
                                if (Singleton<CMallRouletteController>.GetInstance().IsProbabilityDoubled(RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_NULL))
                                {
                                    Text text5 = Utility.GetComponetInChild<Text>(obj3, "Text");
                                    if (text5 != null)
                                    {
                                        text5.set_text(Singleton<CTextManager>.GetInstance().GetText("RES_WEAL_COLORBAR_TYPE_PROBABILITY_DOUBLED"));
                                        obj3.CustomSetActive(true);
                                    }
                                }
                            }
                        }
                        if ((j >= 0) && (j < this.m_AvailableTabList.Count))
                        {
                            this.RefreshTabRedDot(this.m_AvailableTabList[j]);
                        }
                    }
                }
                int tabIndex = this.GetTabIndex(this.CurTab);
                if ((tabIndex >= 0) && (tabIndex < this.m_AvailableTabList.Count))
                {
                    script.SelectElement(tabIndex, true);
                }
                else
                {
                    script.SelectElement(0, true);
                }
            }
        }

        public static bool IsinRegisterSales(ResDT_RegisterSale_Info stRegisterSale)
        {
            if (stRegisterSale.bIsValid != 1)
            {
                return false;
            }
            if (string.IsNullOrEmpty(stRegisterSale.szStartTimeStr))
            {
                return false;
            }
            DateTime time = Utility.ToUtcTime2Local((long) stRegisterSale.dwStartTimeGen);
            TimeSpan span = (TimeSpan) (Singleton<CMallSystem>.GetInstance().m_PlayerRegisterTime - time);
            if (span.TotalMilliseconds < 0.0)
            {
                return false;
            }
            DateTime time2 = Singleton<CMallSystem>.GetInstance().m_PlayerRegisterTime.AddSeconds((double) stRegisterSale.dwValidTime);
            return (Utility.ToUtcTime2Local((long) CRoleInfo.GetCurrentUTCTime()) <= time2);
        }

        public void LoadSubModule()
        {
            DebugHelper.Assert(this.m_MallForm != null, "Mall Form Is Null");
            if (this.m_MallForm != null)
            {
                bool flag = false;
                if (this.m_MallForm.GetWidget(3) != null)
                {
                    switch (this.m_CurTab)
                    {
                        case Tab.Mystery:
                            this.m_MallForm.GetWidget(10).CustomSetActive(false);
                            flag = Singleton<MySteryShop>.GetInstance().Loaded(this.m_MallForm);
                            if (!flag)
                            {
                                this.m_MallForm.GetWidget(11).CustomSetActive(true);
                                Singleton<MySteryShop>.GetInstance().Load(this.m_MallForm);
                                this.m_MallForm.GetWidget(3).CustomSetActive(false);
                            }
                            break;

                        case Tab.Boutique:
                            this.m_MallForm.GetWidget(10).CustomSetActive(false);
                            flag = Singleton<CMallBoutiqueController>.GetInstance().Loaded(this.m_MallForm);
                            if (!flag)
                            {
                                this.m_MallForm.GetWidget(11).CustomSetActive(true);
                                Singleton<CMallBoutiqueController>.GetInstance().Load(this.m_MallForm);
                                this.m_MallForm.GetWidget(3).CustomSetActive(false);
                            }
                            break;

                        case Tab.Recommend:
                            this.m_MallForm.GetWidget(10).CustomSetActive(false);
                            flag = Singleton<CMallRecommendController>.GetInstance().Loaded(this.m_MallForm);
                            if (!flag)
                            {
                                this.m_MallForm.GetWidget(11).CustomSetActive(true);
                                Singleton<CMallRecommendController>.GetInstance().Load(this.m_MallForm);
                                this.m_MallForm.GetWidget(3).CustomSetActive(false);
                            }
                            break;

                        case Tab.Hero:
                            this.m_MallForm.GetWidget(10).CustomSetActive(true);
                            this.ResetSortTypeList();
                            Singleton<BeaconHelper>.GetInstance().m_curBuyDianInfo.buy_dia_channel = "5";
                            Singleton<BeaconHelper>.GetInstance().m_curBuyDianInfo.call_back_time = Time.get_time();
                            Singleton<BeaconHelper>.GetInstance().m_curBuyPropInfo.buy_prop_channel = "5";
                            Singleton<BeaconHelper>.GetInstance().m_curBuyPropInfo.buy_prop_id_time = Time.get_time();
                            flag = this.m_heroMallCtrl.Loaded(this.m_MallForm);
                            if (!flag)
                            {
                                this.m_MallForm.GetWidget(11).CustomSetActive(true);
                                this.m_heroMallCtrl.Load(this.m_MallForm);
                                this.m_MallForm.GetWidget(3).CustomSetActive(false);
                            }
                            break;

                        case Tab.Skin:
                            this.m_MallForm.GetWidget(10).CustomSetActive(true);
                            this.ResetSortTypeList();
                            Singleton<BeaconHelper>.GetInstance().m_curBuyDianInfo.buy_dia_channel = "6";
                            Singleton<BeaconHelper>.GetInstance().m_curBuyDianInfo.call_back_time = Time.get_time();
                            Singleton<BeaconHelper>.GetInstance().m_curBuyPropInfo.buy_prop_channel = "6";
                            Singleton<BeaconHelper>.GetInstance().m_curBuyPropInfo.buy_prop_id_time = Time.get_time();
                            flag = this.m_skinMallCtrl.Loaded(this.m_MallForm);
                            if (!flag)
                            {
                                this.m_MallForm.GetWidget(11).CustomSetActive(true);
                                this.m_skinMallCtrl.Load(this.m_MallForm);
                                this.m_MallForm.GetWidget(3).CustomSetActive(false);
                            }
                            break;

                        case Tab.Symbol_Make:
                            this.m_MallForm.GetWidget(10).CustomSetActive(false);
                            Singleton<BeaconHelper>.GetInstance().m_curBuyDianInfo.buy_dia_channel = "2";
                            Singleton<BeaconHelper>.GetInstance().m_curBuyDianInfo.call_back_time = Time.get_time();
                            Singleton<BeaconHelper>.GetInstance().m_curBuyPropInfo.buy_prop_channel = "2";
                            Singleton<BeaconHelper>.GetInstance().m_curBuyPropInfo.buy_prop_id_time = Time.get_time();
                            Singleton<CSymbolMakeController>.GetInstance().Source = enSymbolMakeSource.Mall;
                            flag = Singleton<CSymbolMakeController>.GetInstance().Loaded(this.m_MallForm);
                            if (!flag)
                            {
                                this.m_MallForm.GetWidget(11).CustomSetActive(true);
                                Singleton<CSymbolMakeController>.GetInstance().Load(this.m_MallForm);
                                this.m_MallForm.GetWidget(3).CustomSetActive(false);
                            }
                            break;

                        case Tab.Factory_Shop:
                            this.m_MallForm.GetWidget(10).CustomSetActive(false);
                            flag = Singleton<CMallFactoryShopController>.GetInstance().Loaded(this.m_MallForm);
                            if (!flag)
                            {
                                this.m_MallForm.GetWidget(11).CustomSetActive(true);
                                Singleton<CMallFactoryShopController>.GetInstance().Load(this.m_MallForm);
                                this.m_MallForm.GetWidget(3).CustomSetActive(false);
                            }
                            break;

                        case Tab.Roulette:
                            this.m_MallForm.GetWidget(10).CustomSetActive(false);
                            flag = Singleton<CMallRouletteController>.GetInstance().Loaded(this.m_MallForm);
                            if (!flag)
                            {
                                this.m_MallForm.GetWidget(11).CustomSetActive(true);
                                Singleton<CMallRouletteController>.GetInstance().Load(this.m_MallForm);
                                this.m_MallForm.GetWidget(3).CustomSetActive(false);
                            }
                            break;
                    }
                }
                if (!flag)
                {
                    GameObject widget = this.m_MallForm.GetWidget(8);
                    if (widget != null)
                    {
                        CUITimerScript component = widget.GetComponent<CUITimerScript>();
                        if (component != null)
                        {
                            component.ReStartTimer();
                        }
                    }
                }
                else
                {
                    Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mall_Update_Sub_Module);
                }
            }
        }

        private void OnConfirmBuyProduct(CUIEvent uiEvent)
        {
            uint key = (uint) uiEvent.m_eventParams.commonUInt64Param1;
            uint count = uiEvent.m_eventParams.commonUInt32Param1;
            CMallFactoryShopController.ShopProduct shopProduct = Singleton<CMallFactoryShopController>.GetInstance().GetProduct(key);
            if (shopProduct != null)
            {
                Singleton<CMallFactoryShopController>.GetInstance().RequestBuy(shopProduct, count);
            }
        }

        private void OnMallActionMaskReset(CUIEvent uiEvent)
        {
            GameObject widget = uiEvent.m_srcFormScript.GetWidget(1);
            if (widget != null)
            {
                widget.CustomSetActive(false);
            }
        }

        private void OnMallCloseForm(CUIEvent uiEvent)
        {
            Singleton<CSoundManager>.GetInstance().PostEvent("UI_buy_chou_jiguan_Stop", null);
            Singleton<CSoundManager>.GetInstance().PostEvent("Stop_Show", null);
            if (this.m_IsMallFormOpen)
            {
                this.m_IsMallFormOpen = false;
                Singleton<CUIManager>.GetInstance().CloseForm(this.sMallFormPath);
                Singleton<CResourceManager>.instance.UnloadUnusedAssets();
                Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.Mall_Close_Mall);
                Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.Mall_Entry_Del_RedDotCheck);
                Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.Mall_Set_Free_Draw_Timer);
                this.m_MallForm = null;
            }
        }

        private void OnMallGotoBoutique(CUIEvent uiEvent)
        {
            this.CurTab = Tab.Boutique;
            CUIEvent event2 = new CUIEvent();
            event2.m_eventID = enUIEventID.Mall_OpenForm;
            event2.m_eventParams.tag = 1;
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(event2);
        }

        private void OnMallGotoCouponsTreasureTab(CUIEvent uiEvent)
        {
            this.CurTab = Tab.Roulette;
            Singleton<CMallRouletteController>.GetInstance().CurTab = CMallRouletteController.Tab.DianQuan;
            CUIEvent event2 = new CUIEvent();
            event2.m_eventID = enUIEventID.Mall_OpenForm;
            event2.m_eventParams.tag = 1;
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(event2);
        }

        private void OnMallGotoDiamondTreasureTab(CUIEvent uiEvent)
        {
            this.CurTab = Tab.Roulette;
            Singleton<CMallRouletteController>.GetInstance().CurTab = CMallRouletteController.Tab.Diamond;
            CUIEvent event2 = new CUIEvent();
            event2.m_eventID = enUIEventID.Mall_OpenForm;
            event2.m_eventParams.tag = 1;
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(event2);
        }

        private void OnMallGoToMysteryTab(CUIEvent uiEvent)
        {
            if (Singleton<MySteryShop>.GetInstance().IsShopAvailable())
            {
                this.CurTab = Tab.Mystery;
                CUIEvent event2 = new CUIEvent();
                event2.m_eventID = enUIEventID.Mall_OpenForm;
                event2.m_eventParams.tag = 1;
                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(event2);
            }
        }

        private void OnMallGoToRecommendHeroTab(CUIEvent uiEvent)
        {
            this.CurTab = Tab.Recommend;
            Singleton<CMallRecommendController>.GetInstance().CurTab = CMallRecommendController.Tab.Hero;
            CUIEvent event2 = new CUIEvent();
            event2.m_eventID = enUIEventID.Mall_OpenForm;
            event2.m_eventParams.tag = 1;
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(event2);
        }

        private void OnMallGoToRecommendSkinTab(CUIEvent uiEvent)
        {
            this.CurTab = Tab.Recommend;
            Singleton<CMallRecommendController>.GetInstance().CurTab = CMallRecommendController.Tab.Skin;
            CUIEvent event2 = new CUIEvent();
            event2.m_eventID = enUIEventID.Mall_OpenForm;
            event2.m_eventParams.tag = 1;
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(event2);
        }

        private void OnMallGotoSkinTab(CUIEvent uiEvent)
        {
            this.CurTab = Tab.Skin;
            CUIEvent event2 = new CUIEvent();
            event2.m_eventID = enUIEventID.Mall_OpenForm;
            event2.m_eventParams.tag = 1;
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(event2);
        }

        private void OnMallGotoSymbolTab(CUIEvent uiEvent)
        {
            this.CurTab = Tab.Symbol_Make;
            CUIEvent event2 = new CUIEvent();
            event2.m_eventID = enUIEventID.Mall_OpenForm;
            event2.m_eventParams.tag = 1;
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(event2);
        }

        private void OnMallGoToTreasureTab(CUIEvent uiEvent)
        {
            this.CurTab = Tab.Roulette;
            CUIEvent event2 = new CUIEvent();
            event2.m_eventID = enUIEventID.Mall_OpenForm;
            event2.m_eventParams.tag = 1;
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(event2);
        }

        private void OnMallOpenFactoryShopTab(CUIEvent uiEvent)
        {
            this.CurTab = Tab.Factory_Shop;
            CUIEvent event2 = new CUIEvent();
            event2.m_eventID = enUIEventID.Mall_OpenForm;
            event2.m_eventParams.tag = 1;
            event2.m_eventParams.tag2 = uiEvent.m_eventParams.tag2;
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(event2);
        }

        public void OnMallOpenForm(CUIEvent uiEvent)
        {
            Singleton<BeaconHelper>.GetInstance().m_curBuyDianInfo.buy_dia_channel = "1";
            Singleton<BeaconHelper>.GetInstance().m_curBuyDianInfo.call_back_time = Time.get_time();
            Singleton<BeaconHelper>.GetInstance().m_curBuyPropInfo.buy_prop_channel = "1";
            Singleton<BeaconHelper>.GetInstance().m_curBuyPropInfo.buy_prop_id_time = Time.get_time();
            if (!CSysDynamicBlock.bLobbyEntryBlocked)
            {
                this.m_MallForm = Singleton<CUIManager>.GetInstance().OpenForm(this.sMallFormPath, false, true);
                if (this.m_MallForm != null)
                {
                    this.m_IsMallFormOpen = true;
                    if (uiEvent.m_eventParams.tag != 1)
                    {
                        if (Singleton<MySteryShop>.GetInstance().IsShopAvailable())
                        {
                            this.CurTab = Tab.Mystery;
                        }
                        else
                        {
                            this.CurTab = Tab.Boutique;
                        }
                    }
                    if (uiEvent.m_eventParams.tag2 != 0)
                    {
                        this.TargetID = uiEvent.m_eventParams.tag2;
                    }
                    else
                    {
                        this.TargetID = 0;
                    }
                    this.InitTab();
                    GameObject obj2 = this.m_MallForm.get_gameObject().get_transform().FindChild("TopCommon/Button_Gift").get_gameObject();
                    GameObject obj3 = this.m_MallForm.get_gameObject().get_transform().FindChild("TopCommon/Button_Crystal").get_gameObject();
                    Singleton<CUINewFlagSystem>.GetInstance().AddNewFlag(obj2, enNewFlagKey.New_GiftCenterBtn_V1, enNewFlagPos.enTopRight, 0.8f, 0f, 0f, enNewFlagType.enNewFlag);
                    Singleton<CUINewFlagSystem>.GetInstance().AddNewFlag(obj3, enNewFlagKey.New_CrystalBtn_V1, enNewFlagPos.enTopRight, 0.8f, -4f, 2f, enNewFlagType.enNewFlag);
                    Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.Mall_Entry_Del_RedDotCheck);
                }
            }
        }

        private void OnMallSkipMaskReset(CUIEvent uiEvent)
        {
            GameObject widget = uiEvent.m_srcFormScript.GetWidget(6);
            if (widget != null)
            {
                widget.CustomSetActive(false);
            }
        }

        private void OnMallTabChange(CUIEvent uiEvent)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            Singleton<CUIManager>.GetInstance().CloseTips();
            CUICommonSystem.CloseCommonTips();
            CUICommonSystem.CloseUseableTips();
            CUIListScript component = uiEvent.m_srcWidget.GetComponent<CUIListScript>();
            int selectedIndex = component.GetSelectedIndex();
            if ((selectedIndex < 0) || (selectedIndex >= this.m_AvailableTabList.Count))
            {
                selectedIndex = 0;
            }
            this.CurTab = this.m_AvailableTabList[selectedIndex];
            Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.Mall_Change_Tab);
            if (this.m_MallForm != null)
            {
                Transform transform = this.m_MallForm.get_transform().Find("TopCommon/Button_Gift");
                if (transform != null)
                {
                    transform.get_gameObject().CustomSetActive(this.CurTab == Tab.Boutique);
                }
                Transform transform2 = this.m_MallForm.get_transform().Find("TopCommon/Button_Crystal");
                if (transform2 != null)
                {
                    transform2.get_gameObject().CustomSetActive(this.CurTab == Tab.Roulette);
                }
                if (this.CurTab == Tab.Symbol_Make)
                {
                    GameObject widget = this.m_MallForm.GetWidget(13);
                    CSymbolSystem.RefreshSymbolCntText(true);
                    CUIEventScript script2 = widget.GetComponent<CUIEventScript>();
                    if (script2 != null)
                    {
                        CUseable useable = CUseableManager.CreateVirtualUseable(enVirtualItemType.enSymbolCoin, 0);
                        stUIEventParams eventParams = new stUIEventParams();
                        eventParams.iconUseable = useable;
                        eventParams.tag = 3;
                        script2.SetUIEvent(enUIEventType.Down, enUIEventID.Tips_ItemInfoOpen, eventParams);
                        script2.SetUIEvent(enUIEventType.HoldEnd, enUIEventID.Tips_ItemInfoClose, eventParams);
                        script2.SetUIEvent(enUIEventType.Click, enUIEventID.Tips_ItemInfoClose, eventParams);
                        script2.SetUIEvent(enUIEventType.DragEnd, enUIEventID.Tips_ItemInfoClose, eventParams);
                    }
                }
                else
                {
                    this.m_MallForm.GetWidget(13).CustomSetActive(false);
                }
            }
            if ((this.m_MallForm != null) && this.m_IsMallFormOpen)
            {
                Transform transform3 = this.m_MallForm.get_transform().Find("pnlBodyBg/pnlBoutique");
                if (transform3 != null)
                {
                    transform3.get_gameObject().CustomSetActive(false);
                }
                Transform transform4 = this.m_MallForm.get_transform().Find("pnlBodyBg/pnlRecommend");
                if (transform4 != null)
                {
                    transform4.get_gameObject().CustomSetActive(false);
                }
                Transform transform5 = this.m_MallForm.get_transform().Find("pnlBodyBg/pnlFactoryShop");
                if (transform5 != null)
                {
                    transform5.get_gameObject().CustomSetActive(false);
                }
                Transform transform6 = this.m_MallForm.get_transform().Find("pnlBodyBg/pnlBuyHero");
                if (transform6 != null)
                {
                    transform6.get_gameObject().CustomSetActive(false);
                }
                Transform transform7 = this.m_MallForm.get_transform().Find("pnlBodyBg/pnlBuySkin");
                if (transform7 != null)
                {
                    transform7.get_gameObject().CustomSetActive(false);
                }
                Transform transform8 = this.m_MallForm.get_transform().Find(string.Format("{0}{1}", "pnlBodyBg/", CSymbolSystem.s_symbolMakePanel));
                if (transform8 != null)
                {
                    transform8.get_gameObject().CustomSetActive(false);
                }
                Transform transform9 = this.m_MallForm.get_transform().Find("pnlBodyBg/pnlRoulette");
                if (transform9 != null)
                {
                    transform9.get_gameObject().CustomSetActive(false);
                }
                Transform transform10 = this.m_MallForm.get_transform().Find("pnlBodyBg/pnlMystery");
                if (transform10 != null)
                {
                    transform10.get_gameObject().CustomSetActive(false);
                }
            }
            this.LoadSubModule();
            if (component.GetLastSelectedIndex() != -1)
            {
                enRedID redID = TabToRedID(this.CurTab);
                if (CUIRedDotSystem.IsShowRedDotByVersion(redID))
                {
                    CUIListElementScript selectedElement = component.GetSelectedElement();
                    if (selectedElement != null)
                    {
                        CUIRedDotSystem.SetRedDotViewByVersion(redID);
                        CUICommonSystem.DelRedDot(selectedElement.get_gameObject());
                    }
                }
            }
        }

        private void OnMalOpenHeroTab(CUIEvent uiEvent)
        {
            this.CurTab = Tab.Hero;
            CUIEvent event2 = new CUIEvent();
            event2.m_eventID = enUIEventID.Mall_OpenForm;
            event2.m_eventParams.tag = 1;
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(event2);
        }

        private void OnSelectSortType(CUIEvent uiEvent)
        {
            GameObject widget = uiEvent.m_srcFormScript.GetWidget(10);
            if (widget != null)
            {
                Transform transform = widget.get_transform().Find("dropList/List");
                transform.get_gameObject().CustomSetActive(!transform.get_gameObject().get_activeSelf());
            }
        }

        private void OnSortTypeChange(CUIEvent uiEvent)
        {
            CUIListScript srcWidgetScript = uiEvent.m_srcWidgetScript as CUIListScript;
            if (srcWidgetScript != null)
            {
                int selectedIndex = srcWidgetScript.GetSelectedIndex();
                GameObject widget = uiEvent.m_srcFormScript.GetWidget(10);
                if (widget != null)
                {
                    widget.get_transform().Find("dropList/List").get_gameObject().CustomSetActive(false);
                    switch (this.CurTab)
                    {
                        case Tab.Hero:
                            if (Enum.IsDefined(typeof(CMallSortHelper.HeroSortType), selectedIndex))
                            {
                                CMallSortHelper.CreateHeroSorter().SetSortType((CMallSortHelper.HeroSortType) selectedIndex);
                                this.UpdateSortContainer();
                                Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.Mall_Sort_Type_Changed);
                            }
                            break;

                        case Tab.Skin:
                            if (Enum.IsDefined(typeof(CMallSortHelper.SkinSortType), selectedIndex))
                            {
                                CMallSortHelper.CreateSkinSorter().SetSortType((CMallSortHelper.SkinSortType) selectedIndex);
                                this.UpdateSortContainer();
                                Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.Mall_Sort_Type_Changed);
                            }
                            return;
                    }
                }
            }
        }

        public void OnSubModuleLoaded()
        {
        }

        private void OnUpdateSubModule(CUIEvent uiEvent)
        {
            this.m_MallForm.GetWidget(11).CustomSetActive(false);
            if (this.m_CurTab == Tab.Recommend)
            {
                this.m_MallForm.GetWidget(9).CustomSetActive(false);
                this.m_MallForm.GetWidget(12).CustomSetActive(true);
            }
            else
            {
                this.m_MallForm.GetWidget(9).CustomSetActive(true);
                this.m_MallForm.GetWidget(12).CustomSetActive(false);
            }
            this.m_MallForm.GetWidget(3).CustomSetActive(true);
            switch (this.m_CurTab)
            {
                case Tab.Mystery:
                    Singleton<MySteryShop>.GetInstance().UpdateUI();
                    break;

                case Tab.Boutique:
                    Singleton<CMallBoutiqueController>.GetInstance().Draw(this.m_MallForm);
                    break;

                case Tab.Recommend:
                    Singleton<CMallRecommendController>.GetInstance().Draw(this.m_MallForm);
                    break;

                case Tab.Hero:
                    this.m_heroMallCtrl.Draw(this.m_MallForm);
                    break;

                case Tab.Skin:
                    this.m_skinMallCtrl.Draw(this.m_MallForm);
                    break;

                case Tab.Symbol_Make:
                    Singleton<CSymbolMakeController>.GetInstance().SwitchToSymbolMakePanel(this.m_MallForm);
                    break;

                case Tab.Factory_Shop:
                    Singleton<CMallFactoryShopController>.GetInstance().Draw(this.m_MallForm);
                    break;

                case Tab.Roulette:
                    Singleton<CMallRouletteController>.GetInstance().Draw(this.m_MallForm);
                    break;
            }
        }

        public void OpenGiftCenter(ulong uId, uint worldId, bool isSns = false)
        {
            this._giftCenter.OpenGiftCenter(uId, worldId, isSns);
        }

        public static RES_SHOPBUY_COINTYPE PayTypeToResBuyCoinType(enPayType payType)
        {
            switch (payType)
            {
                case enPayType.GoldCoin:
                    return RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_PVPCOIN;

                case enPayType.DianQuan:
                    return RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_COUPONS;

                case enPayType.Diamond:
                    return RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_DIAMOND;

                case enPayType.DiamondAndDianQuan:
                    return RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_MIXPAY;

                case enPayType.BurningCoin:
                    return RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_BURNINGCOIN;

                case enPayType.ArenaCoin:
                    return RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_ARENACOIN;

                case enPayType.GuildCoin:
                    return RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_GUILDCOIN;

                case enPayType.SymbolCoin:
                    return RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_SYMBOLCOIN;
            }
            return RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_NULL;
        }

        [MessageHandler(0x47d)]
        public static void ReceivePropGift(CSPkg msg)
        {
            ListView<CUseable> useableListFromReward = CUseableManager.GetUseableListFromReward(msg.stPkgData.stGiftUseGet.stRewardInfo);
            int count = useableListFromReward.Count;
            if (count != 0)
            {
                Singleton<CUIManager>.GetInstance().OpenAwardTip(LinqS.ToArray<CUseable>(useableListFromReward), Singleton<CTextManager>.GetInstance().GetText("Bag_Text_4"), false, enUIEventID.None, false, false, "Form_Award");
                uint heroId = 0;
                uint skinId = 0;
                for (int i = 0; i < count; i++)
                {
                    switch (useableListFromReward[i].m_type)
                    {
                        case COM_ITEM_TYPE.COM_OBJTYPE_HERO:
                            CUICommonSystem.ShowNewHeroOrSkin(useableListFromReward[i].m_baseID, 0, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO, false, null, enFormPriority.Priority4, 0, 0);
                            break;

                        case COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN:
                            CSkinInfo.ResolveHeroSkin(useableListFromReward[i].m_baseID, out heroId, out skinId);
                            CUICommonSystem.ShowNewHeroOrSkin(heroId, skinId, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKIN, false, null, enFormPriority.Priority4, 0, 0);
                            break;

                        default:
                            if ((useableListFromReward[i].MapRewardType == COM_REWARDS_TYPE.COM_REWARDS_TYPE_ITEM) && ((useableListFromReward[i].ExtraFromType == 1) || (useableListFromReward[i].ExtraFromType == 2)))
                            {
                                if (useableListFromReward[i].ExtraFromType == 1)
                                {
                                    heroId = (uint) useableListFromReward[i].ExtraFromData;
                                    CUICommonSystem.ShowNewHeroOrSkin(heroId, 0, enUIEventID.Activity_HeroShow_Back, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO, true, null, enFormPriority.Priority4, (uint) useableListFromReward[i].m_stackCount, 0);
                                }
                                else if (useableListFromReward[i].ExtraFromType == 2)
                                {
                                    skinId = (uint) useableListFromReward[i].ExtraFromData;
                                    CUICommonSystem.ShowNewHeroOrSkin(0, skinId, enUIEventID.Activity_HeroShow_Back, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKIN, true, null, enFormPriority.Priority4, (uint) useableListFromReward[i].m_stackCount, 0);
                                }
                            }
                            break;
                    }
                }
            }
        }

        [MessageHandler(0x12c0)]
        public static void ReceiveRouletteDataNTF(CSPkg msg)
        {
            CSDT_LUCKYDRAW_INFO stDiamond = new CSDT_LUCKYDRAW_INFO();
            if (!luckyDrawDic.TryGetValue(enPayType.Diamond, out stDiamond))
            {
                stDiamond = msg.stPkgData.stLuckyDrawDataNtf.stDiamond;
                luckyDrawDic.Add(enPayType.Diamond, stDiamond);
            }
            else
            {
                stDiamond.dwCnt = msg.stPkgData.stLuckyDrawDataNtf.stDiamond.dwCnt;
                stDiamond.dwDrawMask = msg.stPkgData.stLuckyDrawDataNtf.stDiamond.dwDrawMask;
                stDiamond.dwReachMask = msg.stPkgData.stLuckyDrawDataNtf.stDiamond.dwReachMask;
                stDiamond.dwLuckyPoint = msg.stPkgData.stLuckyDrawDataNtf.stDiamond.dwLuckyPoint;
            }
            CSDT_LUCKYDRAW_INFO stCoupons = new CSDT_LUCKYDRAW_INFO();
            if (!luckyDrawDic.TryGetValue(enPayType.DianQuan, out stCoupons))
            {
                stCoupons = msg.stPkgData.stLuckyDrawDataNtf.stCoupons;
                luckyDrawDic.Add(enPayType.DianQuan, stCoupons);
            }
            else
            {
                stCoupons.dwCnt = msg.stPkgData.stLuckyDrawDataNtf.stCoupons.dwCnt;
                stCoupons.dwDrawMask = msg.stPkgData.stLuckyDrawDataNtf.stCoupons.dwDrawMask;
                stCoupons.dwReachMask = msg.stPkgData.stLuckyDrawDataNtf.stCoupons.dwReachMask;
                stCoupons.dwLuckyPoint = msg.stPkgData.stLuckyDrawDataNtf.stCoupons.dwLuckyPoint;
            }
            Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.Mall_Receive_Roulette_Data);
        }

        public void RefreshTabRedDot(Tab tab)
        {
            if ((this.m_MallForm != null) && this.m_IsMallFormOpen)
            {
                int tabIndex = this.GetTabIndex(tab);
                GameObject widget = this.m_MallForm.GetWidget(0);
                CUIListScript script = (widget == null) ? null : widget.GetComponent<CUIListScript>();
                if (script != null)
                {
                    CUIListElementScript elemenet = null;
                    int elementAmount = script.GetElementAmount();
                    if ((tabIndex >= 0) && (tabIndex < elementAmount))
                    {
                        elemenet = script.GetElemenet(tabIndex);
                        if (elemenet != null)
                        {
                            enRedID redID = TabToRedID(tab);
                            if (CUIRedDotSystem.IsShowRedDotByVersion(redID) || CUIRedDotSystem.IsShowRedDotByLogic(redID))
                            {
                                CUIRedDotSystem.AddRedDot(elemenet.get_gameObject(), enRedDotPos.enTopRight, 0);
                            }
                            if (((tab == Tab.Symbol_Make) && !CUIRedDotSystem.IsShowRedDotByLogic(redID)) && !CUIRedDotSystem.IsShowRedDotByVersion(redID))
                            {
                                CUIRedDotSystem.DelRedDot(elemenet.get_gameObject());
                            }
                            if ((tab == Tab.Mystery) && (!CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Mall_MysteryTab) || !CUIRedDotSystem.IsShowRedDotByLogic(enRedID.Mall_MysteryTab)))
                            {
                                CUIRedDotSystem.DelRedDot(elemenet.get_gameObject());
                            }
                        }
                    }
                }
            }
        }

        public static enPayType ResBuyTypeToPayType(int resShopBuyType)
        {
            switch (resShopBuyType)
            {
                case 2:
                    return enPayType.DianQuan;

                case 4:
                    return enPayType.GoldCoin;

                case 5:
                    return enPayType.BurningCoin;

                case 6:
                    return enPayType.ArenaCoin;

                case 8:
                    return enPayType.GuildCoin;

                case 9:
                    return enPayType.SymbolCoin;

                case 10:
                    return enPayType.Diamond;

                case 11:
                    return enPayType.DiamondAndDianQuan;
            }
            return enPayType.NotSupport;
        }

        private void ResetSortTypeList()
        {
            int num2;
            DebugHelper.Assert(this.m_MallForm != null, "Mall Form Is Null");
            if (this.m_MallForm == null)
            {
                return;
            }
            GameObject widget = this.m_MallForm.GetWidget(10);
            if (widget == null)
            {
                return;
            }
            Transform transform = widget.get_transform().Find("dropList/List");
            if (transform == null)
            {
                return;
            }
            CUIListScript component = transform.GetComponent<CUIListScript>();
            if (component == null)
            {
                return;
            }
            int amount = 0;
            switch (this.CurTab)
            {
                case Tab.Hero:
                    amount = 5;
                    component.SetElementAmount(amount);
                    break;

                case Tab.Skin:
                    amount = 4;
                    component.SetElementAmount(amount);
                    goto Label_00B0;
            }
        Label_00B0:
            num2 = 0;
            while (num2 < amount)
            {
                Text text;
                CUIListElementScript elemenet = component.GetElemenet(num2);
                if (elemenet != null)
                {
                    Transform transform2 = elemenet.get_transform().Find("Text");
                    if (transform2 != null)
                    {
                        text = transform2.GetComponent<Text>();
                        if (text != null)
                        {
                            switch (this.CurTab)
                            {
                                case Tab.Hero:
                                    text.set_text(CMallSortHelper.CreateHeroSorter().GetSortTypeName((CMallSortHelper.HeroSortType) num2));
                                    break;

                                case Tab.Skin:
                                    goto Label_0144;
                            }
                        }
                    }
                }
                goto Label_015C;
            Label_0144:
                text.set_text(CMallSortHelper.CreateSkinSorter().GetSortTypeName((CMallSortHelper.SkinSortType) num2));
            Label_015C:
                num2++;
            }
            component.m_alwaysDispatchSelectedChangeEvent = true;
            component.SelectElement(0, true);
        }

        private void SetAvailableTabs()
        {
            this.m_AvailableTabList.Clear();
            int num = 8;
            for (int i = 0; i < num; i++)
            {
                if (i == 0)
                {
                    if (Singleton<MySteryShop>.GetInstance().IsShopAvailable())
                    {
                        this.m_AvailableTabList.Add(Tab.Mystery);
                    }
                }
                else
                {
                    this.m_AvailableTabList.Add((Tab) i);
                }
            }
        }

        public void SetFreeDrawTimer()
        {
            if (Singleton<GameStateCtrl>.instance.GetCurrentState() is LobbyState)
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if (masterRoleInfo != null)
                {
                    int index = 4;
                    Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this.m_FreeDrawSymbolTimerSeq);
                    index = 4;
                    if ((index >= 0) && (index < masterRoleInfo.m_freeDrawInfo.Length))
                    {
                        long num3 = Math.Max(0, masterRoleInfo.m_freeDrawInfo[index].dwLeftFreeDrawCD - CRoleInfo.GetCurrentUTCTime()) * 0x3e8;
                        if (num3 < 0x7fffffffL)
                        {
                            this.m_FreeDrawSymbolTimerSeq = Singleton<CTimerManager>.GetInstance().AddTimer((int) num3, 1, new CTimer.OnTimeUpHandler(this.SymbolFreeDrawTimerHandler));
                        }
                    }
                }
            }
        }

        public void SetItemImage(CMallItemWidget itemWidget, CMallItem item)
        {
            if ((null != itemWidget) && (itemWidget.m_icon != null))
            {
                Image component = itemWidget.m_icon.GetComponent<Image>();
                component.set_color(CUIUtility.s_Color_White);
                component.SetSprite(item.Icon(), this.m_MallForm, false, true, true, true);
            }
        }

        public void SetItemLabel(CMallItemWidget itemWidget, CMallItem item, bool useSmallIcon = false)
        {
            CMallItem.IconType iconType = item.GetIconType();
            switch (item.Type())
            {
                case CMallItem.ItemType.Hero:
                {
                    if (iconType != CMallItem.IconType.Small)
                    {
                        itemWidget.m_skinLabel.CustomSetActive(false);
                        break;
                    }
                    string prefabPath = string.Format("{0}Common_slotBg{1}", "UGUI/Sprite/Common/", item.Grade());
                    CUIUtility.SetImageSprite(itemWidget.m_skinLabel.GetComponent<Image>(), prefabPath, this.m_MallForm, true, false, false, false);
                    break;
                }
                case CMallItem.ItemType.Skin:
                {
                    if (iconType != CMallItem.IconType.Small)
                    {
                        CUICommonSystem.SetHeroSkinLabelPic(this.m_MallForm, itemWidget.m_skinLabel, item.HeroID(), item.SkinID());
                        break;
                    }
                    string str2 = string.Format("{0}Common_slotBg{1}", "UGUI/Sprite/Common/", item.Grade());
                    CUIUtility.SetImageSprite(itemWidget.m_skinLabel.GetComponent<Image>(), str2, this.m_MallForm, true, false, false, false);
                    break;
                }
                case CMallItem.ItemType.Item:
                    if (iconType == CMallItem.IconType.Small)
                    {
                        string str3 = string.Format("{0}Common_slotBg{1}", "UGUI/Sprite/Common/", item.Grade());
                        CUIUtility.SetImageSprite(itemWidget.m_skinLabel.GetComponent<Image>(), str3, this.m_MallForm, true, false, false, false);
                    }
                    break;
            }
        }

        public void SetItemName(CMallItemWidget itemWidget, CMallItem item, CMallItemWidget.NamePosition position)
        {
            switch (position)
            {
                case CMallItemWidget.NamePosition.Top:
                    itemWidget.m_bottomNameContainer.CustomSetActive(false);
                    itemWidget.m_topNameContainer.CustomSetActive(true);
                    if (itemWidget.m_topNameLeftText != null)
                    {
                        itemWidget.m_topNameLeftText.GetComponent<Text>().set_text(item.FirstName());
                    }
                    if (itemWidget.m_topNameRightText != null)
                    {
                        Text component = itemWidget.m_topNameRightText.GetComponent<Text>();
                        if (string.IsNullOrEmpty(item.SecondName()))
                        {
                            itemWidget.m_topNameRightText.CustomSetActive(false);
                        }
                        else
                        {
                            itemWidget.m_topNameRightText.CustomSetActive(true);
                            component.set_text(item.SecondName());
                        }
                    }
                    break;

                case CMallItemWidget.NamePosition.Bottom:
                    itemWidget.m_topNameContainer.CustomSetActive(false);
                    itemWidget.m_bottomNameContainer.CustomSetActive(true);
                    if (itemWidget.m_bottomNameLeftText != null)
                    {
                        itemWidget.m_bottomNameLeftText.GetComponent<Text>().set_text(item.FirstName());
                    }
                    if (itemWidget.m_bottomNameRightText != null)
                    {
                        Text text4 = itemWidget.m_bottomNameRightText.GetComponent<Text>();
                        if (string.IsNullOrEmpty(item.SecondName()))
                        {
                            itemWidget.m_bottomNameRightText.CustomSetActive(false);
                        }
                        else
                        {
                            itemWidget.m_bottomNameRightText.CustomSetActive(true);
                            text4.set_text(item.SecondName());
                        }
                    }
                    return;
            }
        }

        public void SetItemPriceInfo(CMallItemWidget itemWidget, CMallItem item, ref stPayInfoSet payInfoSet)
        {
            if (itemWidget.m_priceContainer != null)
            {
                itemWidget.m_priceContainer.SetActive(true);
                CUIListScript component = itemWidget.m_priceContainer.GetComponent<CUIListScript>();
                component.SetElementAmount(payInfoSet.m_payInfoCount);
                CMallItem.OldPriceType oldPriceType = item.GetOldPriceType();
                if (payInfoSet.m_payInfoCount < 2)
                {
                    itemWidget.m_orTextContainer.CustomSetActive(false);
                }
                else
                {
                    itemWidget.m_orTextContainer.CustomSetActive(true);
                }
                for (int i = 0; i < payInfoSet.m_payInfoCount; i++)
                {
                    CUIListElementScript elemenet = component.GetElemenet(i);
                    if (elemenet == null)
                    {
                        break;
                    }
                    GameObject widget = elemenet.GetWidget(0);
                    GameObject obj3 = elemenet.GetWidget(1);
                    GameObject obj4 = elemenet.GetWidget(2);
                    GameObject obj5 = elemenet.GetWidget(4);
                    GameObject obj6 = elemenet.GetWidget(3);
                    GameObject obj7 = elemenet.GetWidget(5);
                    if (((((widget == null) || (obj3 == null)) || ((obj4 == null) || (obj5 == null))) || (obj6 == null)) || (obj7 == null))
                    {
                        break;
                    }
                    switch (oldPriceType)
                    {
                        case CMallItem.OldPriceType.None:
                        {
                            itemWidget.m_middleOrText.CustomSetActive(true);
                            itemWidget.m_bottomOrText.CustomSetActive(false);
                            widget.SetActive(false);
                            obj3.SetActive(false);
                            obj4.SetActive(false);
                            obj6.SetActive(true);
                            obj6.GetComponent<Text>().set_text(payInfoSet.m_payInfos[i].m_payValue.ToString());
                            obj7.GetComponent<Image>().SetSprite(GetPayTypeIconPath(payInfoSet.m_payInfos[i].m_payType), this.m_MallForm, true, false, false, false);
                            continue;
                        }
                        case CMallItem.OldPriceType.FirstOne:
                        {
                            itemWidget.m_middleOrText.CustomSetActive(false);
                            itemWidget.m_bottomOrText.CustomSetActive(true);
                            if (i != 0)
                            {
                                break;
                            }
                            obj3.SetActive(false);
                            obj6.SetActive(false);
                            widget.SetActive(true);
                            obj4.SetActive(true);
                            widget.GetComponent<Text>().set_text(payInfoSet.m_payInfos[i].m_oriValue.ToString());
                            obj4.GetComponent<Text>().set_text(payInfoSet.m_payInfos[i].m_payValue.ToString());
                            obj5.GetComponent<Image>().SetSprite(GetPayTypeIconPath(payInfoSet.m_payInfos[i].m_payType), this.m_MallForm, true, false, false, false);
                            continue;
                        }
                        case CMallItem.OldPriceType.SecondOne:
                        {
                            itemWidget.m_middleOrText.CustomSetActive(false);
                            itemWidget.m_bottomOrText.CustomSetActive(true);
                            if (i != 1)
                            {
                                goto Label_03BB;
                            }
                            obj3.SetActive(false);
                            obj6.SetActive(false);
                            widget.SetActive(true);
                            obj4.SetActive(true);
                            widget.GetComponent<Text>().set_text(payInfoSet.m_payInfos[i].m_oriValue.ToString());
                            obj4.GetComponent<Text>().set_text(payInfoSet.m_payInfos[i].m_payValue.ToString());
                            obj5.GetComponent<Image>().SetSprite(GetPayTypeIconPath(payInfoSet.m_payInfos[i].m_payType), this.m_MallForm, true, false, false, false);
                            continue;
                        }
                        case CMallItem.OldPriceType.Both:
                        {
                            itemWidget.m_middleOrText.CustomSetActive(true);
                            itemWidget.m_bottomOrText.CustomSetActive(false);
                            obj3.SetActive(false);
                            obj6.SetActive(false);
                            widget.SetActive(true);
                            obj4.SetActive(true);
                            widget.GetComponent<Text>().set_text(payInfoSet.m_payInfos[i].m_oriValue.ToString());
                            obj4.GetComponent<Text>().set_text(payInfoSet.m_payInfos[i].m_payValue.ToString());
                            obj5.GetComponent<Image>().SetSprite(GetPayTypeIconPath(payInfoSet.m_payInfos[i].m_payType), this.m_MallForm, true, false, false, false);
                            continue;
                        }
                        default:
                        {
                            continue;
                        }
                    }
                    obj3.SetActive(false);
                    widget.SetActive(false);
                    obj6.SetActive(false);
                    obj4.SetActive(true);
                    obj4.GetComponent<Text>().set_text(payInfoSet.m_payInfos[i].m_payValue.ToString());
                    obj5.GetComponent<Image>().SetSprite(GetPayTypeIconPath(payInfoSet.m_payInfos[i].m_payType), this.m_MallForm, true, false, false, false);
                    continue;
                Label_03BB:
                    obj3.SetActive(false);
                    widget.SetActive(false);
                    obj6.SetActive(false);
                    obj4.SetActive(true);
                    obj4.GetComponent<Text>().set_text(payInfoSet.m_payInfos[i].m_payValue.ToString());
                    obj5.GetComponent<Image>().SetSprite(GetPayTypeIconPath(payInfoSet.m_payInfos[i].m_payType), this.m_MallForm, true, false, false, false);
                }
            }
        }

        public void SetItemTag(CMallItemWidget itemWidget, CMallItem item, bool owned = false)
        {
            string iconPath = null;
            string text = null;
            if (item.TagInfo(ref iconPath, ref text, owned) && (itemWidget.m_tagContainer != null))
            {
                itemWidget.m_tagContainer.SetActive(true);
                itemWidget.m_tagContainer.GetComponent<Image>().SetSprite(iconPath, this.m_MallForm, false, true, true, false);
                if (itemWidget.m_tagText != null)
                {
                    itemWidget.m_tagText.GetComponent<Text>().set_text(text);
                }
            }
            else
            {
                itemWidget.m_tagContainer.CustomSetActive(false);
            }
        }

        public void SetMallItem(CMallItemWidget itemWidget, CMallItem item)
        {
            CMallItem.ItemType type;
            Button button;
            Text text;
            CUIEventScript script4;
            if (((item != null) && (this.m_MallForm != null)) && this.m_IsMallFormOpen)
            {
                type = item.Type();
                CMallItem.IconType iconType = item.GetIconType();
                bool flag = item.Owned(false);
                bool flag2 = item.CanSendFriend();
                this.SetItemImage(itemWidget, item);
                switch (type)
                {
                    case CMallItem.ItemType.Hero:
                    {
                        CUIEventScript component = itemWidget.m_item.GetComponent<CUIEventScript>();
                        stUIEventParams eventParams = new stUIEventParams();
                        eventParams.openHeroFormPar.heroId = item.HeroID();
                        eventParams.openHeroFormPar.openSrc = enHeroFormOpenSrc.HeroBuyClick;
                        component.SetUIEvent(enUIEventType.Click, enUIEventID.HeroInfo_OpenForm, eventParams);
                        break;
                    }
                    case CMallItem.ItemType.Skin:
                    {
                        CUIEventScript script2 = itemWidget.m_item.GetComponent<CUIEventScript>();
                        stUIEventParams params2 = new stUIEventParams();
                        params2.openHeroFormPar.heroId = item.HeroID();
                        params2.openHeroFormPar.skinId = item.SkinID();
                        params2.openHeroFormPar.openSrc = enHeroFormOpenSrc.SkinBuyClick;
                        script2.SetUIEvent(enUIEventType.Click, enUIEventID.HeroInfo_OpenForm, params2);
                        break;
                    }
                    case CMallItem.ItemType.Item:
                    {
                        CUIEventScript script3 = itemWidget.m_item.GetComponent<CUIEventScript>();
                        stUIEventParams params3 = new stUIEventParams();
                        params3.tag = item.ProductIdx();
                        script3.SetUIEvent(enUIEventType.Click, enUIEventID.Mall_Boutique_Factory_Product_Click, params3);
                        break;
                    }
                }
                this.SetItemLabel(itemWidget, item, false);
                stPayInfoSet payInfoSet = item.PayInfoSet();
                if ((payInfoSet.m_payInfoCount == 0) || flag)
                {
                    this.SetItemName(itemWidget, item, CMallItemWidget.NamePosition.Bottom);
                }
                else
                {
                    this.SetItemName(itemWidget, item, CMallItemWidget.NamePosition.Top);
                }
                button = null;
                text = null;
                Text text2 = null;
                script4 = null;
                Button button2 = null;
                Text text3 = null;
                CUIEventScript script5 = null;
                itemWidget.m_priceContainer.CustomSetActive(false);
                if (itemWidget.m_buyBtn != null)
                {
                    button = itemWidget.m_buyBtn.GetComponent<Button>();
                    text = itemWidget.m_buyBtnText.GetComponent<Text>();
                    script4 = itemWidget.m_buyBtn.GetComponent<CUIEventScript>();
                }
                if (itemWidget.m_buyBtnOwnedText != null)
                {
                    text2 = itemWidget.m_buyBtnOwnedText.GetComponent<Text>();
                }
                if (itemWidget.m_linkBtn != null)
                {
                    button2 = itemWidget.m_linkBtn.GetComponent<Button>();
                    text3 = itemWidget.m_linkBtnText.GetComponent<Text>();
                    script5 = itemWidget.m_linkBtn.GetComponent<CUIEventScript>();
                }
                itemWidget.m_linkBtn.CustomSetActive(false);
                if (!flag || flag2)
                {
                    if (!flag)
                    {
                        itemWidget.m_experienceMask.CustomSetActive(item.IsValidExperience());
                        this.SetItemTag(itemWidget, item, false);
                        if (payInfoSet.m_payInfoCount <= 0)
                        {
                            itemWidget.m_buyBtnOwnedText.CustomSetActive(false);
                            itemWidget.m_buyBtn.CustomSetActive(false);
                            itemWidget.m_orTextContainer.CustomSetActive(false);
                            itemWidget.m_linkBtn.CustomSetActive(true);
                            string str = item.ObtWay();
                            if (text3 != null)
                            {
                                if (string.IsNullOrEmpty(str))
                                {
                                    text3.set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_SkinState_CannotBuy"));
                                }
                                else
                                {
                                    text3.set_text(str);
                                }
                            }
                            byte num = item.ObtWayType();
                            if (num > 0)
                            {
                                if (((button2 != null) && (script5 != null)) && (text3 != null))
                                {
                                    button2.set_enabled(true);
                                    text3.set_text(item.ObtWay());
                                    script5.set_enabled(true);
                                    stUIEventParams params6 = new stUIEventParams();
                                    params6.tag = num;
                                    script5.SetUIEvent(enUIEventType.Click, enUIEventID.Mall_Jump_Form, params6);
                                }
                            }
                            else if (((button2 != null) && (script5 != null)) && (text3 != null))
                            {
                                button2.set_enabled(false);
                                script5.set_enabled(false);
                            }
                        }
                        else
                        {
                            itemWidget.m_buyBtn.CustomSetActive(true);
                            switch (type)
                            {
                                case CMallItem.ItemType.Hero:
                                    itemWidget.m_buyBtnOwnedText.CustomSetActive(false);
                                    if (((button != null) && (script4 != null)) && (text != null))
                                    {
                                        button.set_enabled(true);
                                        text.set_text(Singleton<CTextManager>.GetInstance().GetText("Mall_Buy"));
                                        script4.set_enabled(true);
                                        stUIEventParams params10 = new stUIEventParams();
                                        params10.heroId = item.HeroID();
                                        script4.SetUIEvent(enUIEventType.Click, enUIEventID.HeroInfo_OpenBuyHeroForm, params10);
                                    }
                                    this.SetItemPriceInfo(itemWidget, item, ref payInfoSet);
                                    return;

                                case CMallItem.ItemType.Skin:
                                {
                                    CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                                    DebugHelper.Assert(masterRoleInfo != null, "SetMallItem::Master Role Info Is Null");
                                    if (masterRoleInfo != null)
                                    {
                                        if (masterRoleInfo.IsCanBuySkinButNotHaveHero(item.HeroID(), item.SkinID()))
                                        {
                                            if (flag2)
                                            {
                                                if (text2 != null)
                                                {
                                                    text2.set_text(Singleton<CTextManager>.GetInstance().GetText("Mall_Skin_State_Buy_hero"));
                                                }
                                                itemWidget.m_buyBtnOwnedText.CustomSetActive(true);
                                                if (((button != null) && (script4 != null)) && (text != null))
                                                {
                                                    script4.set_enabled(true);
                                                    text.set_text(Singleton<CTextManager>.GetInstance().GetText("Mall_Buy_For_Friend"));
                                                    button.set_enabled(true);
                                                    stUIEventParams params7 = new stUIEventParams();
                                                    params7.heroSkinParam.heroId = item.HeroID();
                                                    params7.heroSkinParam.skinId = item.SkinID();
                                                    params7.heroSkinParam.isCanCharge = true;
                                                    script4.SetUIEvent(enUIEventType.Click, enUIEventID.HeroSkin_OpenBuySkinForm, params7);
                                                }
                                            }
                                            else
                                            {
                                                itemWidget.m_buyBtnOwnedText.CustomSetActive(false);
                                                if (((button != null) && (script4 != null)) && (text != null))
                                                {
                                                    script4.set_enabled(true);
                                                    text.set_text(Singleton<CTextManager>.GetInstance().GetText("Mall_Skin_State_Buy_hero"));
                                                    button.set_enabled(true);
                                                    stUIEventParams params8 = new stUIEventParams();
                                                    params8.openHeroFormPar.heroId = item.HeroID();
                                                    params8.openHeroFormPar.skinId = item.SkinID();
                                                    params8.openHeroFormPar.openSrc = enHeroFormOpenSrc.SkinBuyClick;
                                                    script4.SetUIEvent(enUIEventType.Click, enUIEventID.HeroInfo_OpenForm, params8);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            itemWidget.m_buyBtnOwnedText.CustomSetActive(false);
                                            if (((button != null) && (script4 != null)) && (text != null))
                                            {
                                                script4.set_enabled(true);
                                                text.set_text(Singleton<CTextManager>.GetInstance().GetText("Mall_Buy"));
                                                button.set_enabled(true);
                                                stUIEventParams params9 = new stUIEventParams();
                                                params9.heroSkinParam.heroId = item.HeroID();
                                                params9.heroSkinParam.skinId = item.SkinID();
                                                params9.heroSkinParam.isCanCharge = true;
                                                script4.SetUIEvent(enUIEventType.Click, enUIEventID.HeroSkin_OpenBuySkinForm, params9);
                                            }
                                        }
                                        this.SetItemPriceInfo(itemWidget, item, ref payInfoSet);
                                        return;
                                    }
                                    return;
                                }
                                case CMallItem.ItemType.Item:
                                {
                                    if (((button != null) && (script4 != null)) && (text != null))
                                    {
                                        button.set_enabled(true);
                                        text.set_text(Singleton<CTextManager>.GetInstance().GetText("Mall_Buy"));
                                        script4.set_enabled(true);
                                        stUIEventParams params11 = new stUIEventParams();
                                        params11.tag = item.ProductIdx();
                                        script4.SetUIEvent(enUIEventType.Click, enUIEventID.Mall_Boutique_Factory_Product_Click, params11);
                                    }
                                    CUIEventScript script6 = itemWidget.m_item.GetComponent<CUIEventScript>();
                                    stUIEventParams params12 = new stUIEventParams();
                                    params12.tag = item.ProductIdx();
                                    script6.SetUIEvent(enUIEventType.Click, enUIEventID.Mall_Boutique_Factory_Product_Click, params12);
                                    this.SetItemPriceInfo(itemWidget, item, ref payInfoSet);
                                    return;
                                }
                            }
                        }
                        return;
                    }
                    itemWidget.m_experienceMask.CustomSetActive(false);
                    if (iconType == CMallItem.IconType.Small)
                    {
                        itemWidget.m_specialOwnedText.CustomSetActive(true);
                        return;
                    }
                    itemWidget.m_specialOwnedText.CustomSetActive(false);
                    itemWidget.m_buyBtnOwnedText.CustomSetActive(true);
                    if (text2 != null)
                    {
                        switch (type)
                        {
                            case CMallItem.ItemType.Hero:
                                text2.set_text(Singleton<CTextManager>.GetInstance().GetText("Mall_Hero_State_Own"));
                                goto Label_03DC;

                            case CMallItem.ItemType.Skin:
                                text2.set_text(Singleton<CTextManager>.GetInstance().GetText("Mall_Skin_State_Own"));
                                goto Label_03DC;

                            case CMallItem.ItemType.Item:
                                goto Label_03DC;
                        }
                    }
                    goto Label_03DC;
                }
                if ((button != null) && (script4 != null))
                {
                    button.set_enabled(false);
                    script4.set_enabled(false);
                }
                itemWidget.m_priceContainer.CustomSetActive(false);
                itemWidget.m_buyBtn.CustomSetActive(true);
                itemWidget.m_orTextContainer.CustomSetActive(false);
                if (text != null)
                {
                    switch (type)
                    {
                        case CMallItem.ItemType.Hero:
                            text.set_text(Singleton<CTextManager>.GetInstance().GetText("Mall_Hero_State_Own"));
                            break;

                        case CMallItem.ItemType.Skin:
                            text.set_text(Singleton<CTextManager>.GetInstance().GetText("Mall_Skin_State_Own"));
                            break;
                    }
                }
                itemWidget.m_buyBtnOwnedText.CustomSetActive(false);
                this.SetItemTag(itemWidget, item, false);
                itemWidget.m_experienceMask.CustomSetActive(false);
                if (iconType == CMallItem.IconType.Small)
                {
                    itemWidget.m_specialOwnedText.CustomSetActive(true);
                }
                else
                {
                    itemWidget.m_specialOwnedText.CustomSetActive(false);
                }
            }
            return;
        Label_03DC:
            itemWidget.m_buyBtn.CustomSetActive(true);
            if (text != null)
            {
                text.set_text(Singleton<CTextManager>.GetInstance().GetText("Mall_Buy_For_Friend"));
            }
            itemWidget.m_orTextContainer.CustomSetActive(false);
            switch (type)
            {
                case CMallItem.ItemType.Hero:
                    if (button != null)
                    {
                        stUIEventParams params4 = new stUIEventParams();
                        params4.heroId = item.HeroID();
                        params4.commonUInt64Param1 = 0L;
                        params4.commonBool = false;
                        params4.commonUInt32Param1 = 0;
                        button.set_enabled(true);
                        script4.set_enabled(true);
                        script4.SetUIEvent(enUIEventType.Click, enUIEventID.HeroView_OpenBuyHeroForFriend, params4);
                    }
                    return;

                case CMallItem.ItemType.Skin:
                    if (button != null)
                    {
                        stUIEventParams params5 = new stUIEventParams();
                        params5.heroSkinParam.heroId = item.HeroID();
                        params5.heroSkinParam.skinId = item.SkinID();
                        params5.heroSkinParam.isCanCharge = true;
                        params5.commonUInt64Param1 = 0L;
                        params5.commonBool = false;
                        params5.commonUInt32Param1 = 0;
                        button.set_enabled(true);
                        script4.set_enabled(true);
                        script4.SetUIEvent(enUIEventType.Click, enUIEventID.HeroSkin_OpenBuyHeroSkinForFriend, params5);
                    }
                    return;

                case CMallItem.ItemType.Item:
                    return;
            }
        }

        public static void SetPayButton(CUIFormScript formScript, RectTransform buttonTransform, enPayType payType, uint payValue, enUIEventID eventID, ref stUIEventParams eventParams)
        {
            if ((formScript != null) && (buttonTransform != null))
            {
                Transform transform = buttonTransform.FindChild("Image");
                if (transform != null)
                {
                    Image image = transform.get_gameObject().GetComponent<Image>();
                    if (image != null)
                    {
                        image.SetSprite(GetPayTypeIconPath(payType), formScript, true, false, false, false);
                    }
                }
                Transform transform2 = buttonTransform.FindChild("Text");
                if (transform2 != null)
                {
                    Text text = transform2.get_gameObject().GetComponent<Text>();
                    if (text != null)
                    {
                        text.set_text((payValue <= 0) ? "免费" : payValue.ToString());
                    }
                }
                CUIEventScript component = buttonTransform.get_gameObject().GetComponent<CUIEventScript>();
                if (component != null)
                {
                    component.SetUIEvent(enUIEventType.Click, eventID, eventParams);
                }
            }
        }

        public static void SetPayButton(CUIFormScript formScript, RectTransform buttonTransform, enPayType payType, uint payValue, uint oriValue, enUIEventID eventID, ref stUIEventParams eventParams)
        {
            if ((formScript != null) && (buttonTransform != null))
            {
                Transform transform = buttonTransform.FindChild("Image");
                if (transform != null)
                {
                    Image image = transform.get_gameObject().GetComponent<Image>();
                    if (image != null)
                    {
                        image.SetSprite(GetPayTypeIconPath(payType), formScript, true, false, false, false);
                    }
                }
                Transform transform2 = buttonTransform.FindChild("PriceContainer/Price");
                if (transform2 != null)
                {
                    Text text = transform2.get_gameObject().GetComponent<Text>();
                    if (text != null)
                    {
                        text.set_text((payValue <= 0) ? "免费" : payValue.ToString());
                    }
                }
                Transform transform3 = buttonTransform.FindChild("PriceContainer/OriPrice");
                if (payValue >= oriValue)
                {
                    if (transform3 != null)
                    {
                        transform3.get_gameObject().CustomSetActive(false);
                    }
                }
                else if (transform3 != null)
                {
                    transform3.get_gameObject().CustomSetActive(true);
                    Text text2 = transform3.get_gameObject().GetComponent<Text>();
                    if (text2 != null)
                    {
                        text2.set_text((oriValue <= 0) ? "免费" : oriValue.ToString());
                    }
                }
                CUIEventScript component = buttonTransform.get_gameObject().GetComponent<CUIEventScript>();
                if (component != null)
                {
                    component.SetUIEvent(enUIEventType.Click, eventID, eventParams);
                }
            }
        }

        public static void SetSkinBuyPricePanel(CUIFormScript formScript, Transform pricePanel, ref stPayInfo payInfo)
        {
            if (pricePanel != null)
            {
                Transform transform = pricePanel.Find("newPricePanel");
                Transform transform2 = pricePanel.Find("oldPricePanel");
                if (transform != null)
                {
                    transform.get_gameObject().CustomSetActive(false);
                }
                if (transform2 != null)
                {
                    transform2.get_gameObject().CustomSetActive(false);
                }
                if (transform != null)
                {
                    transform.get_gameObject().CustomSetActive(true);
                    Transform costIcon = transform.Find("costImage");
                    CHeroSkinBuyManager.SetPayCostIcon(formScript, costIcon, payInfo.m_payType);
                    CHeroSkinBuyManager.SetPayCurrentPrice(transform.Find("newCostText"), payInfo.m_payValue);
                }
                if ((transform2 != null) && (payInfo.m_payValue != payInfo.m_oriValue))
                {
                    transform2.get_gameObject().CustomSetActive(true);
                    CHeroSkinBuyManager.SetPayCurrentPrice(transform2.Find("oldPriceText"), payInfo.m_oriValue);
                }
            }
        }

        public void SymbolFreeDrawTimerHandler(int seq)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                int index = 4;
                if ((masterRoleInfo.m_freeDrawInfo[index].dwLeftFreeDrawCnt == 0) && ((masterRoleInfo.m_freeDrawInfo[index].dwLeftFreeDrawCD - CRoleInfo.GetCurrentUTCTime()) <= 0))
                {
                    masterRoleInfo.m_freeDrawInfo[index].dwLeftFreeDrawCnt = 1;
                }
                Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.Mall_Entry_Add_RedDotCheck);
                Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref seq);
                this.m_FreeDrawSymbolTimerSeq = -1;
            }
        }

        public static enRedID TabToRedID(Tab tab)
        {
            enRedID did = (enRedID) tab;
            switch (tab)
            {
                case Tab.Mystery:
                    return enRedID.Mall_MysteryTab;

                case Tab.Boutique:
                    return enRedID.Mall_BoutiqueTab;

                case Tab.Recommend:
                    return enRedID.Mall_RecommendTab;

                case Tab.Hero:
                    return enRedID.Mall_HeroTab;

                case Tab.Skin:
                    return enRedID.Mall_HeroSkinTab;

                case Tab.Symbol_Make:
                    return enRedID.Mall_SymbolTab;

                case Tab.Factory_Shop:
                    return enRedID.Mall_SaleTab;

                case Tab.Roulette:
                    return enRedID.Mall_LotteryTab;
            }
            return did;
        }

        public void ToggleActionMask(bool active, float totalTime = 30)
        {
            CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
            if ((mallForm != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
            {
                GameObject widget = mallForm.GetWidget(1);
                GameObject obj3 = mallForm.GetWidget(5);
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

        public void ToggleSkipAnimationMask(bool active, float totalTime = 30)
        {
            CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
            if ((mallForm != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
            {
                GameObject widget = mallForm.GetWidget(6);
                GameObject obj3 = mallForm.GetWidget(7);
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

        public static void TryToPay(enPayPurpose payPurpose, string goodName, enPayType payType, uint payValue, enUIEventID confirmEventID, ref stUIEventParams confirmEventParams, enUIEventID cancelEventID, bool needConfirm, bool guideToAchieveDianQuan, bool noQuestionMark = false)
        {
            if (!ApolloConfig.payEnabled)
            {
                guideToAchieveDianQuan = false;
            }
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                if (GetCurrencyValueFromRoleInfo(masterRoleInfo, payType) >= payValue)
                {
                    string str = string.Empty;
                    if (payType == enPayType.DiamondAndDianQuan)
                    {
                        uint diamond = masterRoleInfo.Diamond;
                        uint dianQuan = (uint) masterRoleInfo.DianQuan;
                        if (diamond < payValue)
                        {
                            object[] args = new object[] { (payValue - diamond).ToString(), Singleton<CTextManager>.GetInstance().GetText(s_payTypeNameKeys[3]), (payValue - diamond).ToString(), Singleton<CTextManager>.GetInstance().GetText(s_payTypeNameKeys[2]) };
                            str = string.Format(Singleton<CTextManager>.GetInstance().GetText("MixPayNotice"), args);
                            needConfirm = true;
                        }
                    }
                    if (needConfirm)
                    {
                        string strContent = string.Empty;
                        if (noQuestionMark)
                        {
                            string[] textArray1 = new string[] { payValue.ToString(), Singleton<CTextManager>.GetInstance().GetText(s_payTypeNameKeys[(int) payType]), Singleton<CTextManager>.GetInstance().GetText(s_payPurposeNameKeys[(int) payPurpose]), goodName, str };
                            strContent = string.Format(Singleton<CTextManager>.GetInstance().GetText("ConfirmPayNoQuestionMark", textArray1), new object[0]);
                        }
                        else
                        {
                            string[] textArray2 = new string[] { payValue.ToString(), Singleton<CTextManager>.GetInstance().GetText(s_payTypeNameKeys[(int) payType]), Singleton<CTextManager>.GetInstance().GetText(s_payPurposeNameKeys[(int) payPurpose]), goodName, str };
                            strContent = string.Format(Singleton<CTextManager>.GetInstance().GetText("ConfirmPay", textArray2), new object[0]);
                        }
                        Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(strContent, confirmEventID, cancelEventID, confirmEventParams, false);
                    }
                    else
                    {
                        CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
                        uIEvent.m_eventID = confirmEventID;
                        uIEvent.m_eventParams = confirmEventParams;
                        Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uIEvent);
                    }
                }
                else if (((payType == enPayType.DianQuan) || (payType == enPayType.DiamondAndDianQuan)) && guideToAchieveDianQuan)
                {
                    string str3 = string.Format(Singleton<CTextManager>.GetInstance().GetText("CurrencyNotEnoughWithJumpToAchieve"), Singleton<CTextManager>.GetInstance().GetText((payType != enPayType.DiamondAndDianQuan) ? s_payTypeNameKeys[2] : "Currency_DiamondAndDianQuan"));
                    Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(str3, enUIEventID.Pay_OpenBuyDianQuanPanel, cancelEventID, false);
                }
                else
                {
                    string str4 = string.Format(Singleton<CTextManager>.GetInstance().GetText("CurrencyNotEnough"), Singleton<CTextManager>.GetInstance().GetText((payType != enPayType.DiamondAndDianQuan) ? s_payTypeNameKeys[(int) payType] : "Currency_DiamondAndDianQuan"));
                    Singleton<CUIManager>.GetInstance().OpenMessageBox(str4, cancelEventID, false);
                }
                Singleton<BeaconHelper>.GetInstance().m_curBuyPropInfo.buy_prop_id = goodName;
                Singleton<BeaconHelper>.GetInstance().m_curBuyPropInfo.buy_quantity = payValue.ToString();
                Singleton<BeaconHelper>.GetInstance().m_curBuyPropInfo.buy_prop_way = payType.ToString();
            }
        }

        public override void UnInit()
        {
            base.UnInit();
            this._giftCenter.UnInit();
            if (this.m_heroMallCtrl != null)
            {
                this.m_heroMallCtrl.UnInit();
            }
            if (this.m_skinMallCtrl != null)
            {
                this.m_skinMallCtrl.UnInit();
            }
            this.m_AvailableTabList = null;
            CUIEventManager instance = Singleton<CUIEventManager>.GetInstance();
            instance.RemoveUIEventListener(enUIEventID.Mall_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnMallOpenForm));
            instance.RemoveUIEventListener(enUIEventID.Mall_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnMallCloseForm));
            instance.RemoveUIEventListener(enUIEventID.Mall_Mall_Tab_Change, new CUIEventManager.OnUIEventHandler(this.OnMallTabChange));
            instance.RemoveUIEventListener(enUIEventID.Mall_GoToBoutiqueTab, new CUIEventManager.OnUIEventHandler(this.OnMallGotoBoutique));
            instance.RemoveUIEventListener(enUIEventID.HeroInfo_GotoMall, new CUIEventManager.OnUIEventHandler(this.OnMalOpenHeroTab));
            instance.RemoveUIEventListener(enUIEventID.Mall_Open_Factory_Shop_Tab, new CUIEventManager.OnUIEventHandler(this.OnMallOpenFactoryShopTab));
            instance.RemoveUIEventListener(enUIEventID.Mall_GoToSymbolTab, new CUIEventManager.OnUIEventHandler(this.OnMallGotoSymbolTab));
            instance.RemoveUIEventListener(enUIEventID.Mall_GoToMysteryTab, new CUIEventManager.OnUIEventHandler(this.OnMallGoToMysteryTab));
            instance.RemoveUIEventListener(enUIEventID.Mall_GoToSkinTab, new CUIEventManager.OnUIEventHandler(this.OnMallGotoSkinTab));
            instance.RemoveUIEventListener(enUIEventID.Mall_GoToTreasureTab, new CUIEventManager.OnUIEventHandler(this.OnMallGoToTreasureTab));
            instance.RemoveUIEventListener(enUIEventID.Mall_GotoCouponsTreasureTab, new CUIEventManager.OnUIEventHandler(this.OnMallGotoCouponsTreasureTab));
            instance.RemoveUIEventListener(enUIEventID.Mall_GotoDianmondTreasureTab, new CUIEventManager.OnUIEventHandler(this.OnMallGotoDiamondTreasureTab));
            instance.RemoveUIEventListener(enUIEventID.Mall_GoToRecommendHeroTab, new CUIEventManager.OnUIEventHandler(this.OnMallGoToRecommendHeroTab));
            instance.RemoveUIEventListener(enUIEventID.Mall_GoToRecommendSkinTab, new CUIEventManager.OnUIEventHandler(this.OnMallGoToRecommendSkinTab));
            instance.RemoveUIEventListener(enUIEventID.Mall_Action_Mask_Reset, new CUIEventManager.OnUIEventHandler(this.OnMallActionMaskReset));
            instance.RemoveUIEventListener(enUIEventID.Mall_Skip_Mask_Reset, new CUIEventManager.OnUIEventHandler(this.OnMallSkipMaskReset));
            instance.RemoveUIEventListener(enUIEventID.Mall_Buy_Product_Confirm, new CUIEventManager.OnUIEventHandler(this.OnConfirmBuyProduct));
            instance.RemoveUIEventListener(enUIEventID.Mall_Update_Sub_Module, new CUIEventManager.OnUIEventHandler(this.OnUpdateSubModule));
            instance.AddUIEventListener(enUIEventID.Mall_Sort_Type_Select, new CUIEventManager.OnUIEventHandler(this.OnSelectSortType));
            instance.AddUIEventListener(enUIEventID.Mall_Sort_Type_Change, new CUIEventManager.OnUIEventHandler(this.OnSortTypeChange));
            Singleton<EventRouter>.instance.RemoveEventHandler(EventID.Mall_Set_Free_Draw_Timer, new Action(this, (IntPtr) this.SetFreeDrawTimer));
            Singleton<EventRouter>.instance.RemoveEventHandler(EventID.Mall_Sub_Module_Loaded, new Action(this, (IntPtr) this.OnSubModuleLoaded));
            Singleton<EventRouter>.instance.RemoveEventHandler<Tab>(EventID.Mall_Refresh_Tab_Red_Dot, new Action<Tab>(this.RefreshTabRedDot));
            this.m_MallForm = null;
        }

        private void UpdateSortContainer()
        {
            DebugHelper.Assert(this.m_MallForm != null, "Mall Form Is Null");
            if (this.m_MallForm != null)
            {
                GameObject widget = this.m_MallForm.GetWidget(10);
                if ((widget != null) && (widget.get_transform().Find("dropList/Button_Down/Text") != null))
                {
                    switch (this.CurTab)
                    {
                        case Tab.Hero:
                        {
                            Text component = widget.get_transform().Find("dropList/Button_Down/Text").GetComponent<Text>();
                            if (component != null)
                            {
                                IMallSort<CMallSortHelper.HeroSortType> sort = CMallSortHelper.CreateHeroSorter();
                                component.set_text(sort.GetSortTypeName(sort.GetCurSortType()));
                            }
                            break;
                        }
                        case Tab.Skin:
                        {
                            Text text2 = widget.get_transform().Find("dropList/Button_Down/Text").GetComponent<Text>();
                            if (text2 != null)
                            {
                                IMallSort<CMallSortHelper.SkinSortType> sort2 = CMallSortHelper.CreateSkinSorter();
                                text2.set_text(sort2.GetSortTypeName(sort2.GetCurSortType()));
                            }
                            return;
                        }
                    }
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

        public int TargetID
        {
            get
            {
                int targetId = this.m_TargetId;
                this.m_TargetId = 0;
                return targetId;
            }
            set
            {
                this.m_TargetId = value;
            }
        }

        [CompilerGenerated]
        private sealed class <GetTabIndex>c__AnonStorey61
        {
            internal CMallSystem.Tab tab;

            internal bool <>m__62(CMallSystem.Tab t)
            {
                return (this.tab == t);
            }
        }

        public enum enMallFormWidget
        {
            Tab,
            Action_Mask,
            Top_Common,
            Body_Container,
            Lottery_Result_Mask,
            Action_Mask_Reset_Timer,
            Skip_Mask,
            Skip_Mask_Reset_Timer,
            Update_Sub_Module_Timer,
            BG,
            SortContainer,
            LoadingIcon,
            BG_2,
            SymbolCoinCntPanel
        }

        public enum Tab
        {
            Boutique = 1,
            Factory_Shop = 6,
            Hero = 3,
            Mystery = 0,
            None = -1,
            Recommend = 2,
            Roulette = 7,
            Skin = 4,
            Symbol_Make = 5,
            TabCount = 8
        }
    }
}

