namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.GameKernal;
    using CSProtocol;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), FrameCommandClass(FRAMECMD_ID_DEF.FRAME_CMD_PLAYLASTHITMODE)]
    public struct PlayLastHitModeCommand : ICommandImplement
    {
        public byte LastHitMode;
        [FrameCommandCreator]
        public static IFrameCommand Creator(ref FRAME_CMD_PKG msg)
        {
            FrameCommand<PlayLastHitModeCommand> command = FrameCommandFactory.CreateFrameCommand<PlayLastHitModeCommand>();
            command.cmdData.LastHitMode = msg.stCmdInfo.stCmdPlayLastHitMode.bLastHitMode;
            return command;
        }

        public bool TransProtocol(FRAME_CMD_PKG msg)
        {
            msg.stCmdInfo.stCmdPlayLastHitMode.bLastHitMode = this.LastHitMode;
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
            if (player != null)
            {
                player.SetLastHitMode((Assets.Scripts.Framework.LastHitMode) this.LastHitMode);
            }
        }

        public void AwakeCommand(IFrameCommand cmd)
        {
        }
    }
}

