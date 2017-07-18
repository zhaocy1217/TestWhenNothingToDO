namespace com.tencent.pandora
{
    using System;
    using System.Net;

    public class NetFrame
    {
        private static int kMaxPendingPacketsLength = 0x2000000;

        public long AsyncConnect(IPAddress address, ushort port, TCPSocketHandler handler)
        {
            Logger.DEBUG("Y1");
            long num = NetLib.PandoraNet_AsyncConnect(address.ToString(), port);
            if (num < 0L)
            {
                Logger.ERROR("NetLib.AsyncConnect ret=" + num.ToString());
                return -1L;
            }
            AddSocketCommand cmd = new AddSocketCommand();
            cmd.theUniqueSocketId = num;
            cmd.theHandler = handler;
            NetLib.AddCommand(cmd);
            return num;
        }

        public void Close(long uniqueSocketId)
        {
            Logger.DEBUG("Y5" + uniqueSocketId.ToString());
            CloseSocketCommand cmd = new CloseSocketCommand();
            cmd.theUniqueSocketId = uniqueSocketId;
            NetLib.AddCommand(cmd);
        }

        public void Destroy()
        {
            NetLib.Destroy();
        }

        public void Drive()
        {
            NetLib.Drive();
        }

        public void Init()
        {
            NetLib.Init();
        }

        public void Reset()
        {
            NetLib.Reset();
        }

        public int SendPacket(long uniqueSocketId, byte[] content)
        {
            Logger.DEBUG("Y6" + uniqueSocketId.ToString());
            long pendingPacketsLength = NetLib.GetPendingPacketsLength();
            if (kMaxPendingPacketsLength < (pendingPacketsLength + content.Length))
            {
                Logger.ERROR("Y7pending pakcets overflow");
                return -1;
            }
            if (content.Length == 0)
            {
                Logger.ERROR("Y8empty content is not allowed");
                return -2;
            }
            SendPacketCommand cmd = new SendPacketCommand();
            cmd.theUniqueSocketId = uniqueSocketId;
            cmd.theContent = content.Clone() as byte[];
            cmd.theCreateTimeMS = DateTime.Now.Millisecond;
            NetLib.AddCommand(cmd);
            return 0;
        }
    }
}

