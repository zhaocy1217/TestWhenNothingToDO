namespace Assets.Scripts.GameSystem
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct stBattleEquipParams
    {
        public CEquipInfo equipInfo;
        public int pos;
        public int m_indexInQuicklyBuyList;
    }
}

