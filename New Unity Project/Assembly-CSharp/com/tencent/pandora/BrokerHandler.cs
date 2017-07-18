namespace com.tencent.pandora
{
    using com.tencent.pandora.MiniJSON;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Text;

    public class BrokerHandler : TCPSocketHandler
    {
        private Action<int, Dictionary<string, object>> packetRecvdAction;
        private Action<int, Dictionary<string, object>> statusChangedAction;

        public BrokerHandler(Action<int, Dictionary<string, object>> statusChangedAction, Action<int, Dictionary<string, object>> packetRecvdAction)
        {
            this.statusChangedAction = statusChangedAction;
            this.packetRecvdAction = packetRecvdAction;
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
            try
            {
                string encodedCompressedData = Convert.ToBase64String(thePacket.theContent, 4, thePacket.theContent.Length - 4);
                byte[] bytes = Convert.FromBase64String(MinizLib.UnCompress(encodedCompressedData.Length, encodedCompressedData));
                string formatMsg = Encoding.UTF8.GetString(bytes);
                Logger.DEBUG(formatMsg);
                Message msg = new Message();
                msg.status = 0;
                msg.content = Json.Deserialize(formatMsg) as Dictionary<string, object>;
                msg.action = this.packetRecvdAction;
                Pandora.Instance.GetNetLogic().EnqueueResult(msg);
            }
            catch (Exception exception)
            {
                Logger.ERROR(exception.StackTrace);
            }
        }
    }
}

