namespace Assets.Scripts.GameSystem
{
    using CSProtocol;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct FOWSurfCell : ISetCellVisible
    {
        public uint[] data;
        public byte xMin;
        public byte xMax;
        public byte yMin;
        public byte yMax;
        public byte cellsPerRow
        {
            get
            {
                return (byte) ((this.xMax - this.xMin) + 1);
            }
        }
        public bool bValid
        {
            get
            {
                return (this.data != null);
            }
        }
        public int GetDataSize()
        {
            int num = (this.xMax - this.xMin) + 1;
            int num2 = (this.yMax - this.yMin) + 1;
            int num3 = num * num2;
            int num4 = (num3 / 0x20) + (((num3 % 0x20) == 0) ? 0 : 1);
            return (num4 * 4);
        }

        public void Init(bool bValid)
        {
            DebugHelper.Assert(this.data == null);
            DebugHelper.Assert(this.xMax > 0);
            DebugHelper.Assert(this.yMax > 0);
            int dataSize = this.GetDataSize();
            if (bValid)
            {
                this.data = new uint[dataSize / 4];
            }
        }

        public void SetVisible(VInt2 inPos, COM_PLAYERCAMP camp, bool visible)
        {
            inPos.x -= this.xMin;
            inPos.y -= this.yMin;
            int num = (inPos.y * this.cellsPerRow) + inPos.x;
            int index = num >> 5;
            uint num3 = ((uint) 1) << num;
            if (visible)
            {
                this.data[index] |= num3;
            }
            else
            {
                this.data[index] &= ~num3;
            }
        }

        public bool GetVisible(int x, int y)
        {
            x -= this.xMin;
            y -= this.yMin;
            int num = (y * this.cellsPerRow) + x;
            int index = num >> 5;
            uint num3 = ((uint) 1) << num;
            return ((this.data[index] & num3) != 0);
        }
    }
}

