namespace com.tencent.pandora
{
    using System;

    public abstract class TCPSocketHandler
    {
        private long theUniqueSocketId;

        protected TCPSocketHandler()
        {
        }

        public abstract int DetectPacketSize(byte[] receivedData, int dataLen);
        public long GetUniqueSocketId()
        {
            return this.theUniqueSocketId;
        }

        public virtual void OnClose()
        {
        }

        public virtual void OnConnected()
        {
        }

        public virtual void OnReceived(Packet thePacket)
        {
        }

        public virtual void OnSent(Packet thePacket)
        {
        }

        public void SetUniqueSocketId(long uniqueSocketId)
        {
            this.theUniqueSocketId = uniqueSocketId;
        }
    }
}

