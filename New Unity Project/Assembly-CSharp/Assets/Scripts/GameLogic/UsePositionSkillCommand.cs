namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.GameKernal;
    using CSProtocol;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), FrameCSSYNCCommandClass(CSSYNC_TYPE_DEF.CSSYNC_CMD_USEPOSITIONSKILL)]
    public struct UsePositionSkillCommand : ICommandImplement
    {
        public VInt2 Position;
        public SkillSlotType SlotType;
        [FrameCommandCreator]
        public static IFrameCommand Creator(ref CSDT_GAMING_CSSYNCINFO msg)
        {
            FrameCommand<UsePositionSkillCommand> command = FrameCommandFactory.CreateCSSyncFrameCommand<UsePositionSkillCommand>();
            command.cmdData.SlotType = (SkillSlotType) msg.stCSSyncDt.stPositionSkill.chSlotType;
            command.cmdData.Position = CommonTools.ToVector2(msg.stCSSyncDt.stPositionSkill.stPosition);
            return command;
        }

        public bool TransProtocol(FRAME_CMD_PKG msg)
        {
            return true;
        }

        public bool TransProtocol(CSDT_GAMING_CSSYNCINFO msg)
        {
            msg.stCSSyncDt.stPositionSkill.chSlotType = (sbyte) this.SlotType;
            CommonTools.CSDTFromVector2(this.Position, ref msg.stCSSyncDt.stPositionSkill.stPosition);
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
                VInt3 pos = new VInt3(this.Position.x, 0, this.Position.y);
                VInt groundY = 0;
                if (PathfindingUtility.GetGroundY(pos, out groundY))
                {
                    pos.y = groundY.i;
                }
                param.Init(this.SlotType, pos);
                player.Captain.handle.ActorControl.CmdUseSkill(cmd, ref param);
            }
        }

        public void AwakeCommand(IFrameCommand cmd)
        {
        }
    }
}

