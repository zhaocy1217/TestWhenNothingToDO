namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.GameKernal;
    using CSProtocol;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), FrameCSSYNCCommandClass(CSSYNC_TYPE_DEF.CSSYNC_CMD_BASEATTACK)]
    public struct UseCommonAttackCommand : ICommandImplement
    {
        public sbyte Start;
        public uint uiRealObjID;
        [FrameCommandCreator]
        public static IFrameCommand Creator(ref CSDT_GAMING_CSSYNCINFO msg)
        {
            FrameCommand<UseCommonAttackCommand> command = FrameCommandFactory.CreateCSSyncFrameCommand<UseCommonAttackCommand>();
            command.cmdData.Start = (sbyte) msg.stCSSyncDt.stBaseAttack.bStart;
            command.cmdData.uiRealObjID = msg.stCSSyncDt.stBaseAttack.dwObjectID;
            return command;
        }

        public bool TransProtocol(FRAME_CMD_PKG msg)
        {
            return true;
        }

        public bool TransProtocol(CSDT_GAMING_CSSYNCINFO msg)
        {
            msg.stCSSyncDt.stBaseAttack.bStart = (byte) this.Start;
            msg.stCSSyncDt.stBaseAttack.dwObjectID = this.uiRealObjID;
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
                player.Captain.handle.ActorControl.CmdCommonAttackMode(cmd, this.Start, this.uiRealObjID);
                Singleton<CBattleSystem>.GetInstance().m_battleEquipSystem.ExecuteInOutEquipShopFrameCommand(0, ref player.Captain);
            }
        }

        public void AwakeCommand(IFrameCommand cmd)
        {
        }
    }
}

