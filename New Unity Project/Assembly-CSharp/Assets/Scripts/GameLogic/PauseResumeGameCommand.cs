namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameSystem;
    using CSProtocol;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), FrameSCSYNCCommandClass(SC_FRAME_CMD_ID_DEF.SC_FRAME_CMD_PAUSE_RESUME_GAME)]
    public struct PauseResumeGameCommand : ICommandImplement
    {
        public byte PauseCommand;
        public byte PauseByCamp;
        public byte[] CampPauseTimes;
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
            if (this.PauseCommand == 2)
            {
                FrameSynchr instance = Singleton<FrameSynchr>.GetInstance();
                instance.PauseCancelCount++;
                Singleton<FrameSynchr>.GetInstance().SetSynchrRunning(true);
            }
        }

        public void ExecCommand(IFrameCommand cmd)
        {
            if (this.PauseCommand == 2)
            {
                FrameSynchr instance = Singleton<FrameSynchr>.GetInstance();
                instance.PauseCancelCount--;
            }
            PauseControl pauseControl = Singleton<CBattleSystem>.GetInstance().pauseControl;
            if (pauseControl != null)
            {
                pauseControl.ExecPauseCommand(this.PauseCommand, this.PauseByCamp, this.CampPauseTimes);
                if ((this.PauseCommand == 1) && (Singleton<FrameSynchr>.GetInstance().PauseCancelCount > 0))
                {
                    Singleton<FrameSynchr>.GetInstance().SetSynchrRunning(true);
                }
            }
        }

        public void Preprocess(IFrameCommand cmd)
        {
        }

        public void AwakeCommand(IFrameCommand cmd)
        {
        }
    }
}

