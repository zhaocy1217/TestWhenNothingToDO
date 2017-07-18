namespace Assets.Scripts.GameLogic
{
    using ResData;
    using System;
    using System.Runtime.CompilerServices;

    public class SoldierSelector
    {
        [CompilerGenerated]
        private int <CurrentCount>k__BackingField;
        [CompilerGenerated]
        private int <CurrentIndex>k__BackingField;
        [CompilerGenerated]
        private int <TotalCount>k__BackingField;
        private ResSoldierTypeInfo[] SoldierConfigList;
        public uint StatTotalCount;

        public uint NextSoldierID()
        {
            if (this.CurrentCount < this.SoldierConfigList[this.CurrentIndex].dwSoldierNum)
            {
                this.CurrentCount++;
                this.TotalCount++;
                return this.SoldierConfigList[this.CurrentIndex].dwSoldierID;
            }
            this.CurrentCount = 0;
            this.CurrentIndex++;
            for (int i = this.CurrentIndex; i < 5; i++)
            {
                if (this.CurrentCount < this.SoldierConfigList[i].dwSoldierNum)
                {
                    this.CurrentCount++;
                    this.TotalCount++;
                    this.CurrentIndex = i;
                    return this.SoldierConfigList[i].dwSoldierID;
                }
            }
            return 0;
        }

        public void Reset(ResSoldierWaveInfo InWaveInfo)
        {
            this.CurrentIndex = 0;
            this.TotalCount = 0;
            this.CurrentCount = 0;
            this.StatTotalCount = 0;
            this.SoldierConfigList = InWaveInfo.astNormalSoldierInfo;
            for (int i = 0; i < 5; i++)
            {
                this.StatTotalCount += this.SoldierConfigList[i].dwSoldierNum;
            }
        }

        public int CurrentCount
        {
            [CompilerGenerated]
            get
            {
                return this.<CurrentCount>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<CurrentCount>k__BackingField = value;
            }
        }

        public int CurrentIndex
        {
            [CompilerGenerated]
            get
            {
                return this.<CurrentIndex>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<CurrentIndex>k__BackingField = value;
            }
        }

        public bool isFinished
        {
            get
            {
                return (this.TotalCount >= this.StatTotalCount);
            }
        }

        public int TotalCount
        {
            [CompilerGenerated]
            get
            {
                return this.<TotalCount>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<TotalCount>k__BackingField = value;
            }
        }
    }
}

