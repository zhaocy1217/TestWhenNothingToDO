namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using CSProtocol;
    using System;
    using System.Runtime.InteropServices;

    public class VDStat
    {
        private int CurrentIndex;
        private static readonly int s_MaxStamps = 11;
        private CampDataStamp[] Stamps = new CampDataStamp[s_MaxStamps];
        private static readonly uint StepCount = 0x708;

        public VDStat()
        {
            for (int i = 0; i < this.Stamps.Length; i++)
            {
                this.Stamps[i] = new CampDataStamp();
            }
        }

        public int CalcCampStat(COM_PLAYERCAMP InFrom, COM_PLAYERCAMP InTo)
        {
            return this.Stamps[this.CurrentIndex].CalcCampStat(InFrom, InTo);
        }

        public void Clear()
        {
            this.CurrentIndex = 0;
            for (int i = 0; i < this.Stamps.Length; i++)
            {
                if (this.Stamps[i] != null)
                {
                    this.Stamps[i].Clear();
                }
            }
            Singleton<EventRouter>.instance.RemoveEventHandler<PoolObjHandle<ActorRoot>, int, bool, PoolObjHandle<ActorRoot>>("HeroGoldCoinInBattleChange", new Action<PoolObjHandle<ActorRoot>, int, bool, PoolObjHandle<ActorRoot>>(this, (IntPtr) this.OnHeroGoldCoinChanged));
        }

        public void GetMaxCampStat(int InStampIndex, COM_PLAYERCAMP InFrom, COM_PLAYERCAMP InTo, out int OutMaxPositive, out int OutMaxNegative)
        {
            this.Stamps[InStampIndex].GetMaxCampStat(InFrom, InTo, out OutMaxPositive, out OutMaxNegative);
        }

        private void OnHeroGoldCoinChanged(PoolObjHandle<ActorRoot> InActor, int InChangedValue, bool bInIsIncome, PoolObjHandle<ActorRoot> target)
        {
            if (((InChangedValue > 0) && bInIsIncome) && (InActor != 0))
            {
                this.TryMoveToNext();
                if (((this.CurrentIndex >= 0) && (this.CurrentIndex < this.Stamps.Length)) && (this.Stamps[this.CurrentIndex] != null))
                {
                    this.Stamps[this.CurrentIndex].OnHeroGoldCoinChanged(ref InActor, InChangedValue, bInIsIncome, target);
                }
            }
        }

        public bool ShouldStat()
        {
            return Singleton<BattleLogic>.instance.GetCurLvelContext().IsMobaMode();
        }

        public void StartRecord()
        {
            this.Clear();
            if (this.ShouldStat())
            {
                Singleton<EventRouter>.instance.AddEventHandler<PoolObjHandle<ActorRoot>, int, bool, PoolObjHandle<ActorRoot>>("HeroGoldCoinInBattleChange", new Action<PoolObjHandle<ActorRoot>, int, bool, PoolObjHandle<ActorRoot>>(this, (IntPtr) this.OnHeroGoldCoinChanged));
            }
        }

        private void TryMoveToNext()
        {
            uint num = Singleton<FrameSynchr>.instance.CurFrameNum / StepCount;
            if ((this.CurrentIndex < num) && (num < this.Stamps.Length))
            {
                this.CurrentIndex = (int) num;
            }
        }

        public int count
        {
            get
            {
                return (this.CurrentIndex + 1);
            }
        }
    }
}

