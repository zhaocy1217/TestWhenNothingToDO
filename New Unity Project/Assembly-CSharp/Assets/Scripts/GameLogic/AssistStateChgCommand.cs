namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.GameKernal;
    using CSProtocol;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), FrameSCSYNCCommandClass(SC_FRAME_CMD_ID_DEF.SC_FRAME_CMD_ASSISTSTATECHG)]
    public struct AssistStateChgCommand : ICommandImplement
    {
        public byte m_chgType;
        public uint m_aiPlayerID;
        public uint m_masterPlayerID;
        public bool TransProtocol(FRAME_CMD_PKG msg)
        {
            return true;
        }

        public bool TransProtocol(CSDT_GAMING_CSSYNCINFO msg)
        {
            return true;
        }

        public void OnReceive(IFrameCommand cmd)
        {
        }

        public void Preprocess(IFrameCommand cmd)
        {
        }

        public void ExecCommand(IFrameCommand cmd)
        {
            Player player = Singleton<GamePlayerCenter>.instance.GetPlayer(this.m_aiPlayerID);
            Player player2 = Singleton<GamePlayerCenter>.instance.GetPlayer(this.m_masterPlayerID);
            if (((player != null) && (player.Captain != 0)) && ((player2 != null) && (player2.Captain != 0)))
            {
                if (this.m_chgType == 1)
                {
                    player.Captain.handle.ActorControl.SetFollowOther(true, player2.Captain.handle.ObjID);
                }
                else
                {
                    player.Captain.handle.ActorControl.SetFollowOther(false, 0);
                }
            }
        }

        public void AwakeCommand(IFrameCommand cmd)
        {
        }
    }
}

