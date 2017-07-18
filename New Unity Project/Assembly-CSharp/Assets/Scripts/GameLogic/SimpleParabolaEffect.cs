namespace Assets.Scripts.GameLogic
{
    using System;
    using System.Runtime.CompilerServices;

    public class SimpleParabolaEffect : IDropDownEffect
    {
        [CompilerGenerated]
        private VInt3 <Current>k__BackingField;
        [CompilerGenerated]
        private VInt3 <EndPos>k__BackingField;
        [CompilerGenerated]
        private DropItem <Item>k__BackingField;
        [CompilerGenerated]
        private VInt3 <StartPos>k__BackingField;
        [CompilerGenerated]
        private int <TimeDelta>k__BackingField;
        private bool bIsFinished;
        private int Height;
        private int Total;

        public SimpleParabolaEffect(VInt3 InStartPos, VInt3 InEndPos)
        {
            this.StartPos = InStartPos;
            this.EndPos = InEndPos;
            this.TimeDelta = 0;
            this.Total = MonoSingleton<GlobalConfig>.instance.DropItemFlyTime;
            this.Height = MonoSingleton<GlobalConfig>.instance.DropItemFlyHeight;
            DebugHelper.Assert(this.Total > 0);
            this.bIsFinished = false;
        }

        public void Bind(DropItem item)
        {
            this.Item = item;
            DebugHelper.Assert(this.Item != null);
            this.Item.SetLocation(this.StartPos);
            this.Current = this.StartPos;
        }

        private void Finish()
        {
            this.Current = this.EndPos;
            this.bIsFinished = true;
            if (this.Item != null)
            {
                this.Item.SetLocation(this.EndPos);
            }
        }

        public void OnUpdate(int delta)
        {
            this.TimeDelta += delta;
            if (this.TimeDelta >= this.Total)
            {
                this.Finish();
            }
            else
            {
                int num = IntMath.Lerp(this.StartPos.x, this.EndPos.x, this.TimeDelta, this.Total);
                int num2 = IntMath.Lerp(this.StartPos.z, this.EndPos.z, this.TimeDelta, this.Total);
                int num3 = 0;
                if ((this.TimeDelta << 1) < this.Total)
                {
                    num3 = IntMath.Lerp(this.StartPos.y, this.StartPos.y + this.Height, this.TimeDelta << 1, this.Total);
                }
                else
                {
                    num3 = IntMath.Lerp(this.StartPos.y + this.Height, this.EndPos.y, (this.TimeDelta << 1) - this.Total, this.Total);
                }
                this.Current = new VInt3(num, num3, num2);
                if (this.Item != null)
                {
                    this.Item.SetLocation(this.Current);
                }
            }
        }

        public VInt3 Current
        {
            [CompilerGenerated]
            get
            {
                return this.<Current>k__BackingField;
            }
            [CompilerGenerated]
            protected set
            {
                this.<Current>k__BackingField = value;
            }
        }

        public VInt3 EndPos
        {
            [CompilerGenerated]
            get
            {
                return this.<EndPos>k__BackingField;
            }
            [CompilerGenerated]
            protected set
            {
                this.<EndPos>k__BackingField = value;
            }
        }

        public bool isFinished
        {
            get
            {
                return this.bIsFinished;
            }
        }

        public DropItem Item
        {
            [CompilerGenerated]
            get
            {
                return this.<Item>k__BackingField;
            }
            [CompilerGenerated]
            protected set
            {
                this.<Item>k__BackingField = value;
            }
        }

        public VInt3 location
        {
            get
            {
                return this.Current;
            }
        }

        public VInt3 StartPos
        {
            [CompilerGenerated]
            get
            {
                return this.<StartPos>k__BackingField;
            }
            [CompilerGenerated]
            protected set
            {
                this.<StartPos>k__BackingField = value;
            }
        }

        public int TimeDelta
        {
            [CompilerGenerated]
            get
            {
                return this.<TimeDelta>k__BackingField;
            }
            [CompilerGenerated]
            protected set
            {
                this.<TimeDelta>k__BackingField = value;
            }
        }
    }
}

