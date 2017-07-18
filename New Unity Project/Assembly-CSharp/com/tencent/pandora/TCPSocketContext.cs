namespace com.tencent.pandora
{
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;

    public class TCPSocketContext
    {
        private int detectedPacketSize;
        private bool isOnConnectCalled;
        private static int kDefaultReceiveBufferCapacity = 0x20000;
        private static int kMaxPakcetSize = 0x400000;
        private int peekPacketSentSize;
        private Queue pendingPackets = new Queue();
        private byte[] receiveBuffer = new byte[kDefaultReceiveBufferCapacity];
        private int receiveBufferCapacity = kDefaultReceiveBufferCapacity;
        private int receivedLength;
        private TCPSocketHandler theHandler;
        private long theUniqueSocketId;

        public TCPSocketContext(long uniqueSocketId, TCPSocketHandler handler)
        {
            this.theUniqueSocketId = uniqueSocketId;
            this.theHandler = handler;
            this.theHandler.SetUniqueSocketId(uniqueSocketId);
        }

        private void AdjustReceiveBuffer()
        {
            int receiveBufferCapacity = this.receiveBufferCapacity;
            if (this.detectedPacketSize > 0)
            {
                if (this.detectedPacketSize > this.receiveBufferCapacity)
                {
                    receiveBufferCapacity = Math.Min(this.detectedPacketSize, kMaxPakcetSize);
                }
                else if (this.detectedPacketSize < (this.receiveBufferCapacity * 0.5))
                {
                    receiveBufferCapacity = Math.Max((int) (this.receiveBufferCapacity * 0.5), kDefaultReceiveBufferCapacity);
                }
            }
            else if ((this.receivedLength > (this.receiveBufferCapacity * 0.8)) && (this.receiveBufferCapacity < kMaxPakcetSize))
            {
                receiveBufferCapacity = Math.Min(kMaxPakcetSize, 2 * this.receiveBufferCapacity);
            }
            if (receiveBufferCapacity != this.receiveBufferCapacity)
            {
                byte[] destinationArray = new byte[receiveBufferCapacity];
                Array.Copy(this.receiveBuffer, destinationArray, this.receivedLength);
                this.receiveBuffer = destinationArray;
                this.receiveBufferCapacity = receiveBufferCapacity;
            }
        }

        public void Enqueue(Packet packet)
        {
            this.pendingPackets.Enqueue(packet);
        }

        public TCPSocketHandler GetHandler()
        {
            return this.theHandler;
        }

        public long GetUniqueSocketId()
        {
            return this.theUniqueSocketId;
        }

        public int HandleClose(out int discardDataSize)
        {
            Logger.DEBUG("Z12");
            discardDataSize = 0;
            IEnumerator enumerator = this.pendingPackets.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Packet current = (Packet) enumerator.Current;
                    discardDataSize += current.theContent.Length;
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
            this.pendingPackets.Clear();
            this.theHandler.OnClose();
            return 0;
        }

        public int HandleInputEvent()
        {
            Logger.DEBUG("Z4");
            if (!this.isOnConnectCalled)
            {
                this.theHandler.OnConnected();
                this.isOnConnectCalled = true;
                return 0;
            }
        Label_0029:
            this.AdjustReceiveBuffer();
            int leftBufferLen = this.receiveBufferCapacity - this.receivedLength;
            int num2 = NetLib.PandoraNet_AsyncRead(this.theUniqueSocketId, leftBufferLen);
            Logger.DEBUG("theUniqueSocketId=" + this.theUniqueSocketId.ToString() + " recvdLen=" + num2.ToString());
            if (num2 < 0)
            {
                if (num2 != -100)
                {
                    return -1;
                }
            }
            else
            {
                this.receivedLength += num2;
                int num3 = this.SplitReceiveDataToPackets();
                if (num3 <= 0)
                {
                    if (num3 < 0)
                    {
                        Logger.ERROR("Z6received data error");
                        return -2;
                    }
                    goto Label_0029;
                }
            }
            return 0;
        }

        public int HandleOutputEvent(out int sentDataSize)
        {
            if (!this.isOnConnectCalled)
            {
                this.theHandler.OnConnected();
                this.isOnConnectCalled = true;
            }
            sentDataSize = 0;
            if (this.pendingPackets.Count == 0)
            {
                return 0;
            }
            Packet packet = this.pendingPackets.Peek() as Packet;
            while (this.peekPacketSentSize < packet.theContent.Length)
            {
                int length = packet.theContent.Length - this.peekPacketSentSize;
                string encodedData = Convert.ToBase64String(packet.theContent, this.peekPacketSentSize, length);
                int num2 = NetLib.PandoraNet_AsyncWrite(encodedData.Length, encodedData, this.theUniqueSocketId);
                if (num2 < 0)
                {
                    if (num2 != -100)
                    {
                        return -1;
                    }
                    break;
                }
                this.peekPacketSentSize += num2;
            }
            if (this.peekPacketSentSize == packet.theContent.Length)
            {
                sentDataSize = this.peekPacketSentSize;
                this.peekPacketSentSize = 0;
                Packet thePacket = this.pendingPackets.Dequeue() as Packet;
                this.theHandler.OnSent(thePacket);
            }
            return this.pendingPackets.Count;
        }

        public void ReadDataCallback(int encodedDataLen, string encodedData)
        {
            Logger.DEBUG("theUniqueSocketId=" + this.theUniqueSocketId.ToString() + " encodedDataLen=" + encodedDataLen.ToString());
            byte[] sourceArray = Convert.FromBase64String(encodedData);
            Logger.DEBUG("decodedData.Length=[" + sourceArray.Length + "]");
            Array.Copy(sourceArray, 0, this.receiveBuffer, this.receivedLength, sourceArray.Length);
        }

        private int SplitReceiveDataToPackets()
        {
            int num = 0;
            while (this.receivedLength > 0)
            {
                int num2 = this.theHandler.DetectPacketSize(this.receiveBuffer, this.receivedLength);
                if (num2 > 0)
                {
                    if (num2 > kMaxPakcetSize)
                    {
                        Logger.ERROR("Z1detected packet size overflow " + num2.ToString());
                        return -1;
                    }
                    this.detectedPacketSize = num2;
                    if (this.detectedPacketSize > this.receivedLength)
                    {
                        return num;
                    }
                    Packet thePacket = new Packet();
                    thePacket.theCreateTimeMS = DateTime.Now.Millisecond;
                    thePacket.theContent = new byte[this.detectedPacketSize];
                    Array.Copy(this.receiveBuffer, 0, thePacket.theContent, 0, this.detectedPacketSize);
                    this.theHandler.OnReceived(thePacket);
                    num++;
                    Array.Copy(this.receiveBuffer, this.detectedPacketSize, this.receiveBuffer, 0, this.receivedLength - this.detectedPacketSize);
                    this.receivedLength -= this.detectedPacketSize;
                    this.detectedPacketSize = 0;
                }
                else
                {
                    if (num2 == 0)
                    {
                        if (this.receivedLength != kMaxPakcetSize)
                        {
                            return num;
                        }
                        Logger.ERROR("Z2format error");
                        return -2;
                    }
                    Logger.ERROR("Z3detect packet size error");
                    return -3;
                }
            }
            return num;
        }
    }
}

