namespace com.tencent.pandora
{
    using System;
    using System.Net;

    public class ATMHandler : TCPSocketHandler
    {
        private Action<int, Dictionary<string, object>> statusChangedAction;

        public ATMHandler(Action<int, Dictionary<string, object>> statusChangedAction)
        {
            this.statusChangedAction = statusChangedAction;
        }

        public override int DetectPacketSize(byte[] receivedData, int dataLen)
        {
            Logger.DEBUG(string.Empty);
            if (dataLen < 4)
            {
                return 0;
            }
            return (IPAddress.NetworkToHostOrder(BitConverter.ToInt32(receivedData, 0)) + 4);
        }

        public override void OnClose()
        {
            Logger.DEBUG(string.Empty);
            Message msg = new Message();
            msg.status = -1;
            msg.content["uniqueSocketId"] = base.GetUniqueSocketId();
            msg.action = this.statusChangedAction;
            Pandora.Instance.GetNetLogic().EnqueueResult(msg);
        }

        public override void OnConnected()
        {
            Logger.DEBUG(string.Empty);
            Message msg = new Message();
            msg.status = 0;
            msg.content["uniqueSocketId"] = base.GetUniqueSocketId();
            msg.action = this.statusChangedAction;
            Pandora.Instance.GetNetLogic().EnqueueResult(msg);
        }

        public override void OnReceived(Packet thePacket)
        {
            Logger.DEBUG(string.Empty);
        }
    }
}

