namespace Assets.Scripts.GameSystem
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public class SampleData
    {
        private int[] _datas;
        [CompilerGenerated]
        private int <count>k__BackingField;
        [CompilerGenerated]
        private int <curDataLeft>k__BackingField;
        [CompilerGenerated]
        private int <curDataRight>k__BackingField;
        [CompilerGenerated]
        private int <max>k__BackingField;
        [CompilerGenerated]
        private int <min>k__BackingField;
        [CompilerGenerated]
        private float <step>k__BackingField;
        public const int AUTO_GROW_STEP = 0x20;

        public SampleData(float step)
        {
            this.step = step;
            this._datas = new int[0x20];
            this.count = 0;
            this.min = 0;
            this.max = 0;
        }

        public void Add(int data)
        {
            int num;
            if (this.count >= this._datas.Length)
            {
                int[] dst = new int[this.count + 0x20];
                Buffer.BlockCopy(this._datas, 0, dst, 0, this.count * 4);
                this._datas = dst;
            }
            this.count = (num = this.count) + 1;
            this._datas[num] = data;
            if (data < this.min)
            {
                this.min = data;
            }
            if (data > this.max)
            {
                this.max = data;
            }
        }

        public void Clear(bool keepSize)
        {
            this.count = 0;
            this.min = 0;
            this.max = 0;
            if (!keepSize)
            {
                this._datas = new int[0x20];
            }
        }

        public void SetCurData(int left, int right)
        {
            this.curDataLeft = left;
            this.curDataRight = right;
            this.Add(left - right);
        }

        public int count
        {
            [CompilerGenerated]
            get
            {
                return this.<count>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<count>k__BackingField = value;
            }
        }

        public int curDataLeft
        {
            [CompilerGenerated]
            get
            {
                return this.<curDataLeft>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<curDataLeft>k__BackingField = value;
            }
        }

        public int curDataRight
        {
            [CompilerGenerated]
            get
            {
                return this.<curDataRight>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<curDataRight>k__BackingField = value;
            }
        }

        public int this[int index]
        {
            get
            {
                return this._datas[index];
            }
        }

        public int max
        {
            [CompilerGenerated]
            get
            {
                return this.<max>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<max>k__BackingField = value;
            }
        }

        public int min
        {
            [CompilerGenerated]
            get
            {
                return this.<min>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<min>k__BackingField = value;
            }
        }

        public float step
        {
            [CompilerGenerated]
            get
            {
                return this.<step>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.<step>k__BackingField = value;
            }
        }
    }
}

