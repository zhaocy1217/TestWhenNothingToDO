namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.GameKernal;
    using CSProtocol;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), FrameCommandClass(FRAMECMD_ID_DEF.FRAME_CMD_PLAYER_IN_OUT_EQUIPSHOP)]
    public struct PlayerInOutEquipShopCommand : ICommandImplement
    {
        public byte m_inOut;
        [FrameCommandCreator]
        public static IFrameCommand Creator(ref FRAME_CMD_PKG msg)
        {
            FrameCommand<PlayerInOutEquipShopCommand> command = FrameCommandFactory.CreateFrameCommand<PlayerInOutEquipShopCommand>();
            command.cmdData.m_inOut = msg.stCmdInfo.stCmdPlayerInOutEquipShop.bInOut;
            return command;
        }

        public bool TransProtocol(FRAME_CMD_PKG msg)
        {
            msg.stCmdInfo.stCmdPlayerInOutEquipShop.bInOut = this.m_inOut;
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
            Player player = Singleton<GamePlayerCenter>.GetInstance().GetPlayer(cmd.playerID);
            if ((player != null) && (player.Captain != 0))
            {
                Singleton<CBattleSystem>.GetInstance().m_battleEquipSystem.ExecuteInOutEquipShopFrameCommand(this.m_inOut, ref player.Captain);
            }
        }

        public void AwakeCommand(IFrameCommand cmd)
        {
        }
    }
}

