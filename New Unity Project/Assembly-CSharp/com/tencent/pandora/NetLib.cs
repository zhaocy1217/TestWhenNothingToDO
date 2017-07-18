namespace com.tencent.pandora
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public class NetLib
    {
        private static Dictionary<long, TCPSocketContext> dictSocketContext = new Dictionary<long, TCPSocketContext>();
        private static long pendingPacketsLength = 0L;

        public static void AddCommand(Command cmd)
        {
            try
            {
                if (cmd is AddSocketCommand)
                {
                    AddSocketCommand command = cmd as AddSocketCommand;
                    long theUniqueSocketId = command.theUniqueSocketId;
                    TCPSocketContext context = new TCPSocketContext(command.theUniqueSocketId, command.theHandler);
                    if (!dictSocketContext.ContainsKey(theUniqueSocketId))
                    {
                        Logger.DEBUG(theUniqueSocketId.ToString() + " added");
                        dictSocketContext.Add(theUniqueSocketId, context);
                    }
                    else
                    {
                        TCPSocketContext context2 = dictSocketContext[theUniqueSocketId];
                        Logger.ERROR("NetThread::ProcessCommands socket handle conflict " + context.GetUniqueSocketId().ToString() + " VS " + context2.GetUniqueSocketId().ToString());
                    }
                }
                else if (cmd is SendPacketCommand)
                {
                    SendPacketCommand command2 = cmd as SendPacketCommand;
                    long key = command2.theUniqueSocketId;
                    if (dictSocketContext.ContainsKey(key))
                    {
                        TCPSocketContext context3 = dictSocketContext[key];
                        Packet packet = new Packet();
                        packet.theCreateTimeMS = command2.theCreateTimeMS;
                        packet.theContent = command2.theContent.Clone() as byte[];
                        context3.Enqueue(packet);
                        pendingPacketsLength += packet.theContent.Length;
                    }
                }
                else
                {
                    CloseSocketCommand command3 = cmd as CloseSocketCommand;
                    long num3 = command3.theUniqueSocketId;
                    if (dictSocketContext.ContainsKey(num3))
                    {
                        TCPSocketContext context4 = dictSocketContext[num3];
                        Logger.DEBUG(num3.ToString() + " removed");
                        int discardDataSize = 0;
                        context4.HandleClose(out discardDataSize);
                        pendingPacketsLength -= discardDataSize;
                        dictSocketContext.Remove(num3);
                        PandoraNet_Close(num3);
                    }
                    else
                    {
                        Logger.WARN("uniqueSocketId=" + num3.ToString() + " alreay missing");
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.ERROR(exception.StackTrace);
            }
        }

        [MonoPInvokeCallback(typeof(DataHandler))]
        public static void DataCallback(int encodedDataLen, [MarshalAs(UnmanagedType.LPStr)] string encodedData, long uniqueSocketId)
        {
            try
            {
                if (dictSocketContext.ContainsKey(uniqueSocketId))
                {
                    dictSocketContext[uniqueSocketId].ReadDataCallback(encodedDataLen, encodedData);
                }
            }
            catch (Exception exception)
            {
                Logger.ERROR(exception.StackTrace);
            }
        }

        public static void Destroy()
        {
            PandoraNet_Destroy();
        }

        public static void Drive()
        {
            List<long> list = new List<long>();
            List<long> list2 = new List<long>();
            foreach (KeyValuePair<long, TCPSocketContext> pair in dictSocketContext)
            {
                list.Add(pair.Key);
            }
            foreach (long num in list)
            {
                if (dictSocketContext.ContainsKey(num))
                {
                    long uniqueSocketId = num;
                    TCPSocketContext context = dictSocketContext[uniqueSocketId];
                    switch (PandoraNet_DoSelect(uniqueSocketId))
                    {
                        case 0:
                        {
                            continue;
                        }
                        case 1:
                        {
                            if (context.HandleInputEvent() < 0)
                            {
                                int num5 = 0;
                                context.HandleClose(out num5);
                                pendingPacketsLength -= num5;
                                list2.Add(uniqueSocketId);
                            }
                            continue;
                        }
                        case 2:
                        {
                            int sentDataSize = 0;
                            int num7 = context.HandleOutputEvent(out sentDataSize);
                            pendingPacketsLength -= sentDataSize;
                            if (num7 < 0)
                            {
                                int num8 = 0;
                                context.HandleClose(out num8);
                                pendingPacketsLength -= num8;
                                list2.Add(uniqueSocketId);
                            }
                            continue;
                        }
                    }
                    int discardDataSize = 0;
                    context.HandleClose(out discardDataSize);
                    pendingPacketsLength -= discardDataSize;
                    list2.Add(uniqueSocketId);
                }
            }
            foreach (long num10 in list2)
            {
                dictSocketContext.Remove(num10);
                PandoraNet_Close(num10);
            }
        }

        public static long GetPendingPacketsLength()
        {
            return pendingPacketsLength;
        }

        public static void Init()
        {
            PandoraNet_RegisterDataHandler(new DataHandler(NetLib.DataCallback));
            PandoraNet_RegisterLogHandler(new LogHandler(NetLib.LogCallback));
            PandoraNet_Init();
        }

        [MonoPInvokeCallback(typeof(LogHandler))]
        public static void LogCallback(int level, [MarshalAs(UnmanagedType.LPStr)] string logMsg)
        {
            try
            {
                switch (level)
                {
                    case 0:
                        Logger.DEBUG(logMsg);
                        return;

                    case 1:
                        Logger.INFO(logMsg);
                        return;

                    case 2:
                        Logger.WARN(logMsg);
                        return;

                    case 3:
                        Logger.ERROR(logMsg);
                        return;
                }
            }
            catch (Exception)
            {
            }
        }

        [DllImport("PandoraNet", CallingConvention=CallingConvention.Cdecl)]
        public static extern long PandoraNet_AsyncConnect([MarshalAs(UnmanagedType.LPStr)] string ipStr, ushort port);
        [DllImport("PandoraNet", CallingConvention=CallingConvention.Cdecl)]
        public static extern int PandoraNet_AsyncRead(long uniqueSocketId, int leftBufferLen);
        [DllImport("PandoraNet", CallingConvention=CallingConvention.Cdecl)]
        public static extern int PandoraNet_AsyncWrite(int encodedDataLen, [MarshalAs(UnmanagedType.LPStr)] string encodedData, long uniqueSocketId);
        [DllImport("PandoraNet", CallingConvention=CallingConvention.Cdecl)]
        public static extern void PandoraNet_Close(long uniqueSocketId);
        [DllImport("PandoraNet", CallingConvention=CallingConvention.Cdecl)]
        public static extern int PandoraNet_Destroy();
        [DllImport("PandoraNet", CallingConvention=CallingConvention.Cdecl)]
        public static extern int PandoraNet_DoSelect(long uniqueSocketId);
        [DllImport("PandoraNet", CallingConvention=CallingConvention.Cdecl)]
        public static extern int PandoraNet_Init();
        [DllImport("PandoraNet", CallingConvention=CallingConvention.Cdecl)]
        public static extern void PandoraNet_RegisterDataHandler([MarshalAs(UnmanagedType.FunctionPtr)] DataHandler handler);
        [DllImport("PandoraNet", CallingConvention=CallingConvention.Cdecl)]
        public static extern void PandoraNet_RegisterLogHandler([MarshalAs(UnmanagedType.FunctionPtr)] LogHandler handler);
        public static void Reset()
        {
            foreach (KeyValuePair<long, TCPSocketContext> pair in dictSocketContext)
            {
                PandoraNet_Close(pair.Key);
            }
            dictSocketContext.Clear();
            pendingPacketsLength = 0L;
        }

        public delegate void DataHandler(int encodedDataLen, [MarshalAs(UnmanagedType.LPStr)] string encodedData, long uniqueSocketId);

        public delegate void LogHandler(int level, [MarshalAs(UnmanagedType.LPStr)] string logMsg);
    }
}

