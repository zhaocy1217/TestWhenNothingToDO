namespace Assets.Scripts.GameSystem
{
    using ResData;
    using System;

    public class PointsExchangePhase : ActivityPhase
    {
        private uint _id;
        internal ushort _usedTimes;
        public ResDT_PointExchange Config;

        public PointsExchangePhase(Activity owner, uint id, ResDT_PointExchange config) : base(owner)
        {
            this._id = id;
            this.Config = config;
            this._usedTimes = 0;
        }

        public int GetMaxExchangeCount()
        {
            int num = 0;
            if (base.Owner.timeState != Activity.TimeState.Going)
            {
                return 0;
            }
            PointsExchangeActivity owner = base.Owner as PointsExchangeActivity;
            if (owner != null)
            {
                uint maxExchangeCount = owner.GetMaxExchangeCount((int) this.ID);
                uint exchangeCount = owner.GetExchangeCount((int) this.ID);
                uint dwPointCnt = this.Config.dwPointCnt;
                num = (int) (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().JiFen / dwPointCnt);
                if (maxExchangeCount > 0)
                {
                    num = Math.Min(num, (int) (maxExchangeCount - exchangeCount));
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
                PointsExchangeActivity owner = base.Owner as PointsExchangeActivity;
                if (owner != null)
                {
                    uint maxExchangeCount = owner.GetMaxExchangeCount((int) this.ID);
                    uint exchangeCount = owner.GetExchangeCount((int) this.ID);
                    uint dwPointCnt = this.Config.dwPointCnt;
                    flag = (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().JiFen >= dwPointCnt) && ((maxExchangeCount == 0) || (exchangeCount < maxExchangeCount));
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

