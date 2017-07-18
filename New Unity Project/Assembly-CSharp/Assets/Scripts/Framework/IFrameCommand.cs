namespace Assets.Scripts.Framework
{
    using CSProtocol;
    using System;

    public interface IFrameCommand
    {
        void AwakeCommand();
        void ExecCommand();
        void OnReceive();
        void Preprocess();
        void Send();
        bool TransProtocol(CSDT_GAMING_CSSYNCINFO msg);
        bool TransProtocol(FRAME_CMD_PKG msg);

        uint cmdId { get; set; }

        byte cmdType { get; set; }

        uint frameNum { get; set; }

        bool isCSSync { get; set; }

        uint playerID { get; set; }

        byte sendCnt { get; set; }
    }
}

