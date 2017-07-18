namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.GameKernal;
    using CSProtocol;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), FrameCommandClass(FRAMECMD_ID_DEF.FRAME_CMD_PLAYER_CHOOSE_EQUIPSKILL)]
    public struct PlayerChooseEquipSkillCommand : ICommandImplement
    {
        public int m_iEquipSlotIndex;
        [FrameCommandCreator]
        public static IFrameCommand Creator(ref FRAME_CMD_PKG msg)
        {
            FrameCommand<PlayerChooseEquipSkillCommand> command = FrameCommandFactory.CreateFrameCommand<PlayerChooseEquipSkillCommand>();
            command.cmdData.m_iEquipSlotIndex = msg.stCmdInfo.stCmdPlayerChooseEquipSkill.bEquipSlotIndex;
            return command;
        }

        public bool TransProtocol(FRAME_CMD_PKG msg)
        {
            msg.stCmdInfo.stCmdPlayerChooseEquipSkill.bEquipSlotIndex = (byte) this.m_iEquipSlotIndex;
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
                Singleton<CBattleSystem>.GetInstance().m_battleEquipSystem.ExecChooseEquipSkillCmd(this.m_iEquipSlotIndex, ref player.Captain);
            }
        }

        public void AwakeCommand(IFrameCommand cmd)
        {
        }
    }
}

