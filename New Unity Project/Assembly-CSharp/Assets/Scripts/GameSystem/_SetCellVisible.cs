namespace Assets.Scripts.GameSystem
{
    using CSProtocol;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct _SetCellVisible : ISetCellVisible
    {
        private GameFowManager pFowMgr;
        private byte maxAlpha;
        public void Init()
        {
            this.pFowMgr = Singleton<GameFowManager>.instance;
            this.maxAlpha = 0xff;
        }

        public void SetVisible(VInt2 inPos, COM_PLAYERCAMP camp, bool bVisible)
        {
            if (this.pFowMgr != null)
            {
                GameFowManager.SetVisible(true, inPos.x, inPos.y, camp, camp == this.pFowMgr.m_hostPlayerCamp);
            }
        }

        public void SetBaseDataVisible(VInt2 inPos, COM_PLAYERCAMP camp, bool bVisible)
        {
            if (this.pFowMgr != null)
            {
                GameFowManager.SetBaseDataVisible(true, inPos.x, inPos.y, camp, camp == this.pFowMgr.m_hostPlayerCamp);
            }
        }
    }
}

