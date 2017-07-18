namespace Assets.Scripts.GameLogic
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct EQUIPACTIVESKILLSLOTINFO
    {
        public bool bIsFirstInit;
        public bool bIsFirstShow;
        public int iEquipSlotIndex;
        public EQUIPACTIVESKILLSLOTINFO(bool _bIsFirstInit, bool _bIsFirstShow, int _iEquipSlotIndex)
        {
            this.bIsFirstInit = _bIsFirstInit;
            this.bIsFirstShow = _bIsFirstShow;
            this.iEquipSlotIndex = _iEquipSlotIndex;
        }
    }
}

