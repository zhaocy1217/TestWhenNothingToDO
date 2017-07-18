namespace Assets.Scripts.GameSystem
{
    using CSProtocol;
    using ResData;
    using System;

    public class ExchangePhase : ActivityPhase
    {
        private uint _id;
        internal ushort _usedTimes;
        public ResDT_WealExchagne_Info Config;

        public ExchangePhase(Activity owner, ResDT_WealExchagne_Info config) : base(owner)
        {
            this._id = config.bIdx;
            this.Config = config;
            this._usedTimes = 0;
        }

        public bool CheckExchange()
        {
            ExchangeActivity owner = base.Owner as ExchangeActivity;
            uint maxExchangeCount = 0;
            uint exchangeCount = 0;
            if (owner != null)
            {
                maxExchangeCount = owner.GetMaxExchangeCount(this.Config.bIdx);
                exchangeCount = owner.GetExchangeCount(this.Config.bIdx);
            }
            return (exchangeCount < maxExchangeCount);
        }

        public int GetMaxExchangeCount()
        {
            int num = 0;
            ResDT_Item_Info info = null;
            ResDT_Item_Info info2 = null;
            if (this.Config.bColItemCnt > 0)
            {
                info = this.Config.astColItemInfo[0];
            }
            if (this.Config.bColItemCnt > 1)
            {
                info2 = this.Config.astColItemInfo[1];
            }
            CUseableContainer useableContainer = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().GetUseableContainer(enCONTAINER_TYPE.ITEM);
            if (useableContainer != null)
            {
                if (info != null)
                {
                    uint dwItemID = info.dwItemID;
                    ushort wItemType = info.wItemType;
                    int useableStackCount = useableContainer.GetUseableStackCount((COM_ITEM_TYPE) wItemType, dwItemID);
                    ushort wItemCnt = info.wItemCnt;
                    int num6 = useableStackCount / wItemCnt;
                    num = num6;
                }
                if (info2 != null)
                {
                    uint baseID = info2.dwItemID;
                    ushort num8 = info2.wItemType;
                    int num9 = useableContainer.GetUseableStackCount((COM_ITEM_TYPE) num8, baseID);
                    ushort num10 = info2.wItemCnt;
                    int num11 = num9 / num10;
                    num = Math.Min(num, num11);
                }
                ExchangeActivity owner = base.Owner as ExchangeActivity;
                if (owner != null)
                {
                    uint maxExchangeCount = 0;
                    uint exchangeCount = 0;
                    maxExchangeCount = owner.GetMaxExchangeCount(this.Config.bIdx);
                    exchangeCount = owner.GetExchangeCount(this.Config.bIdx);
                    if (maxExchangeCount > 0)
                    {
                        num = Math.Min(num, (int) (maxExchangeCount - exchangeCount));
                    }
                }
            }
            return num;
        }

        public override int CloseTime
        {
            get
            {
                return 0;
            }
        }

        public override uint ID
        {
            get
            {
                return this._id;
            }
        }

        public override bool ReadyForGet
        {
            get
            {
                bool flag = false;
                if ((base.Owner.timeState != Activity.TimeState.Going) || (this.Config.bIsShowHotSpot == 0))
                {
                    return false;
                }
                ResDT_Item_Info info = null;
                ResDT_Item_Info info2 = null;
                if (this.Config.bColItemCnt > 0)
                {
                    info = this.Config.astColItemInfo[0];
                }
                if (this.Config.bColItemCnt > 1)
                {
                    info2 = this.Config.astColItemInfo[1];
                }
                CUseableContainer useableContainer = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().GetUseableContainer(enCONTAINER_TYPE.ITEM);
                if (useableContainer == null)
                {
                    return false;
                }
                flag = true;
                if (info != null)
                {
                    uint dwItemID = info.dwItemID;
                    ushort wItemType = info.wItemType;
                    int useableStackCount = useableContainer.GetUseableStackCount((COM_ITEM_TYPE) wItemType, dwItemID);
                    ushort wItemCnt = info.wItemCnt;
                    if (useableStackCount < wItemCnt)
                    {
                        flag = false;
                    }
                }
                if (info2 != null)
                {
                    uint baseID = info2.dwItemID;
                    ushort num6 = info2.wItemType;
                    int num7 = useableContainer.GetUseableStackCount((COM_ITEM_TYPE) num6, baseID);
                    ushort num8 = info2.wItemCnt;
                    if (num7 < num8)
                    {
                        flag = false;
                    }
                }
                if (!this.CheckExchange())
                {
                    flag = false;
                }
                return flag;
            }
        }

        public override uint RewardID
        {
            get
            {
                return 0;
            }
        }

        public override int StartTime
        {
            get
            {
                return 0;
            }
        }
    }
}

