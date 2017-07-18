namespace com.tencent.pandora
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public class MinizLib
    {
        private static string lastMinizData = string.Empty;

        public static string Compress(int rawDataLen, string rawData)
        {
            PandoraNet_RegisterMinizHandler(new MinizHandler(MinizLib.MinizCallback));
            if (PandoraNet_Compress(rawDataLen, rawData) == 0)
            {
                return lastMinizData;
            }
            return string.Empty;
        }

        [MonoPInvokeCallback(typeof(MinizHandler))]
        public static void MinizCallback(int encodedDataLen, [MarshalAs(UnmanagedType.LPStr)] string encodedData)
        {
            lastMinizData = encodedData;
        }

        [DllImport("PandoraNet", CallingConvention=CallingConvention.Cdecl)]
        public static extern int PandoraNet_Compress(int rawDataLen, [MarshalAs(UnmanagedType.LPStr)] string rawData);
        [DllImport("PandoraNet", CallingConvention=CallingConvention.Cdecl)]
        public static extern void PandoraNet_RegisterMinizHandler([MarshalAs(UnmanagedType.FunctionPtr)] MinizHandler handler);
        [DllImport("PandoraNet", CallingConvention=CallingConvention.Cdecl)]
        public static extern int PandoraNet_UnCompress(int encodedCompressedDataLen, [MarshalAs(UnmanagedType.LPStr)] string encodedCompressedData);
        public static string UnCompress(int encodedCompressedDataLen, string encodedCompressedData)
        {
            PandoraNet_RegisterMinizHandler(new MinizHandler(MinizLib.MinizCallback));
            if (PandoraNet_UnCompress(encodedCompressedDataLen, encodedCompressedData) == 0)
            {
                return lastMinizData;
            }
            return string.Empty;
        }

        public delegate void MinizHandler(int encodedDataLen, [MarshalAs(UnmanagedType.LPStr)] string encodedData);
    }
}

