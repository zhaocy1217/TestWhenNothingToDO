namespace Assets.Scripts.GameSystem
{
    using CSProtocol;
    using ResData;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class CUseable
    {
        public int ExtraFromData;
        public int ExtraFromType;
        public int m_addTime;
        public uint m_arenaCoinBuy;
        public uint m_baseID;
        public byte m_bCanUse = 1;
        public uint m_burningCoinBuy;
        public uint m_coinSale;
        public string m_description = string.Empty;
        public uint m_diamondBuy;
        public uint m_dianQuanBuy;
        public uint m_dianQuanDirectBuy;
        public ulong m_getTime;
        public uint m_goldCoinBuy;
        public byte m_grade;
        public uint m_guildCoinBuy;
        public uint m_iconID;
        public byte m_isBatchUse;
        public byte m_isSale;
        public ulong m_itemSortNum;
        public string m_name = string.Empty;
        public ulong m_objID;
        public uint m_skinCoinBuy;
        public int m_stackCount = 1;
        public int m_stackMax;
        public int m_stackMulti;
        public COM_ITEM_TYPE m_type;

        public uint GetBuyPrice(RES_SHOPBUY_COINTYPE coinType)
        {
            switch (coinType)
            {
                case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_COUPONS:
                    return this.m_dianQuanBuy;

                case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_PVPCOIN:
                    return this.m_goldCoinBuy;

                case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_BURNINGCOIN:
                    return this.m_burningCoinBuy;

                case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_ARENACOIN:
                    return this.m_arenaCoinBuy;

                case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_SKINCOIN:
                    return this.m_skinCoinBuy;

                case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_GUILDCOIN:
                    return this.m_guildCoinBuy;

                case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_DIAMOND:
                case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_MIXPAY:
                    return this.m_diamondBuy;
            }
            return 0;
        }

        public virtual string GetIconPath()
        {
            return string.Empty;
        }

        public static int GetMultiple(ref COMDT_REWARD_MULTIPLE_DETAIL multipleDetail, CUseable usb)
        {
            for (int i = 0; i < multipleDetail.bNum; i++)
            {
                COMDT_REWARD_MULTIPLE_INFO comdt_reward_multiple_info = multipleDetail.astMultiple[i];
                if (comdt_reward_multiple_info.wRewardType == ((ushort) usb.MapRewardType))
                {
                    int multipleValue = GetMultipleValue(comdt_reward_multiple_info.stNewMultipleInfo, 1);
                    float num3 = ((float) multipleValue) / 10000f;
                    int num4 = 0;
                    if (multipleValue != 0)
                    {
                        if (num3 > 0f)
                        {
                            num4 = (int) (num3 + 0.9999f);
                        }
                        else if (num3 < 0f)
                        {
                            num4 = (int) (num3 - 0.9999f);
                        }
                    }
                    if (comdt_reward_multiple_info.wRewardType == 1)
                    {
                        if (((CItem) usb).m_itemData.bClass == comdt_reward_multiple_info.dwRewardTypeParam)
                        {
                            return num4;
                        }
                    }
                    else
                    {
                        return num4;
                    }
                }
            }
            return 0;
        }

        public static int GetMultiple(uint baseVal, ref COMDT_REWARD_MULTIPLE_DETAIL multipleDetail, ushort rewardType, short subType = -1)
        {
            for (int i = 0; i < multipleDetail.bNum; i++)
            {
                COMDT_REWARD_MULTIPLE_INFO comdt_reward_multiple_info = multipleDetail.astMultiple[i];
                if ((comdt_reward_multiple_info.wRewardType == rewardType) && ((subType < 0) || (((uint) subType) == comdt_reward_multiple_info.dwRewardTypeParam)))
                {
                    int num2 = 0;
                    for (int j = 0; j < comdt_reward_multiple_info.stNewMultipleInfo.dwMultipleNum; j++)
                    {
                        switch (comdt_reward_multiple_info.stNewMultipleInfo.astMultipleData[j].bOperator)
                        {
                            case 0:
                                num2 += comdt_reward_multiple_info.stNewMultipleInfo.astMultipleData[j].iValue;
                                break;

                            case 1:
                            {
                                double num4 = (baseVal * comdt_reward_multiple_info.stNewMultipleInfo.astMultipleData[j].iValue) / 10000.0;
                                if (num4 > 0.0)
                                {
                                    num2 += (int) (num4 + 0.9999);
                                }
                                else if (num4 < 0.0)
                                {
                                    num2 += (int) (num4 - 0.9999);
                                }
                                break;
                            }
                        }
                    }
                    return num2;
                }
            }
            return 0;
        }

        public static uint GetMultipleInfo(out COMDT_MULTIPLE_DATA[] multipleData, ref COMDT_REWARD_MULTIPLE_DETAIL multipleDetail, ushort rewardType, short subType = -1)
        {
            for (int i = 0; i < multipleDetail.bNum; i++)
            {
                COMDT_REWARD_MULTIPLE_INFO comdt_reward_multiple_info = multipleDetail.astMultiple[i];
                if ((comdt_reward_multiple_info.wRewardType == rewardType) && ((subType < 0) || (((uint) subType) == comdt_reward_multiple_info.dwRewardTypeParam)))
                {
                    multipleData = comdt_reward_multiple_info.stNewMultipleInfo.astMultipleData;
                    return comdt_reward_multiple_info.stNewMultipleInfo.dwMultipleNum;
                }
            }
            multipleData = null;
            return 0;
        }

        public static int GetMultipleValue(COMDT_MULTIPLE_INFO_NEW multipleInfo, int multiType)
        {
            for (int i = 0; i < multipleInfo.dwMultipleNum; i++)
            {
                if (multipleInfo.astMultipleData[i].iType == multiType)
                {
                    return multipleInfo.astMultipleData[i].iValue;
                }
            }
            return 0;
        }

        public static uint GetQqVipExtraCoin(uint totalCoin, ref COMDT_REWARD_MULTIPLE_DETAIL multipleDetail, ushort rewardType)
        {
            for (int i = 0; i < multipleDetail.bNum; i++)
            {
                COMDT_REWARD_MULTIPLE_INFO comdt_reward_multiple_info = multipleDetail.astMultiple[i];
                if (comdt_reward_multiple_info.wRewardType == rewardType)
                {
                    int multipleValue = GetMultipleValue(comdt_reward_multiple_info.stNewMultipleInfo, 1);
                    float num4 = ((float) GetMultipleValue(comdt_reward_multiple_info.stNewMultipleInfo, 2)) / 10000f;
                    float num5 = (((float) totalCoin) / ((((float) multipleValue) / 10000f) + num4)) * num4;
                    if (num5 > 0f)
                    {
                        return (uint) (num5 + 0.9999f);
                    }
                    if (num5 < 0f)
                    {
                        return (uint) (num5 - 0.9999f);
                    }
                }
            }
            return 0;
        }

        public virtual int GetSalableCount()
        {
            return this.m_stackCount;
        }

        public void ResetTime()
        {
            this.m_getTime = this.m_addTime + ((ulong) Time.get_time());
        }

        public void SetMultiple(ref COMDT_REWARD_MULTIPLE_DETAIL multipleDetail, bool preCond = true)
        {
            this.m_stackMulti = !preCond ? 0 : GetMultiple(ref multipleDetail, this);
        }

        public virtual bool HasOwnMax
        {
            get
            {
                return false;
            }
        }

        public virtual COM_REWARDS_TYPE MapRewardType
        {
            get
            {
                return COM_REWARDS_TYPE.COM_REWARDS_TYPE_MAX;
            }
        }
    }
}

