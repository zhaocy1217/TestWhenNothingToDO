namespace Assets.Scripts.GameSystem
{
    using CSProtocol;
    using System;

    public interface ISetCellVisible
    {
        void SetVisible(VInt2 inPos, COM_PLAYERCAMP camp, bool bVisible);
    }
}

