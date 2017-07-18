namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.GameKernal;
    using CSProtocol;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), FrameCSSYNCCommandClass(CSSYNC_TYPE_DEF.CSSYNC_CMD_USEDIRECTIONALSKILL)]
    public struct UseDirectionalSkillCommand : ICommandImplement
    {
        public short Degree;
        public SkillSlotType SlotType;
        public uint dwObjectID;
        [FrameCommandCreator]
        public static IFrameCommand Creator(ref CSDT_GAMING_CSSYNCINFO msg)
        {
            FrameCommand<UseDirectionalSkillCommand> command = FrameCommandFactory.CreateCSSyncFrameCommand<UseDirectionalSkillCommand>();
            command.cmdData.Degree = msg.stCSSyncDt.stDirectionSkill.nDegree;
            command.cmdData.SlotType = (SkillSlotType) msg.stCSSyncDt.stDirectionSkill.chSlotType;
            command.cmdData.dwObjectID = msg.stCSSyncDt.stDirectionSkill.dwObjectID;
            return command;
        }

        public bool TransProtocol(FRAME_CMD_PKG msg)
        {
            return true;
        }

        public bool TransProtocol(CSDT_GAMING_CSSYNCINFO msg)
        {
            msg.stCSSyncDt.stDirectionSkill.nDegree = this.Degree;
            msg.stCSSyncDt.stDirectionSkill.chSlotType = (sbyte) this.SlotType;
            msg.stCSSyncDt.stDirectionSkill.dwObjectID = this.dwObjectID;
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
                SkillUseParam param = new SkillUseParam();
                VInt3 inVec = VInt3.right.RotateY(this.Degree);
                param.Init(this.SlotType, inVec, false, this.dwObjectID);
                player.Captain.handle.ActorControl.CmdUseSkill(cmd, ref param);
            }
        }

        public void AwakeCommand(IFrameCommand cmd)
        {
        }
    }
}

