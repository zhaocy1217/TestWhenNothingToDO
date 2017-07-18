namespace com.tencent.pandora
{
    using System;
    using System.Net;
    using System.Text;

    internal class MsdkTea
    {
        private static uint kDelta = 0x9e3779b9;
        private static int kLogRounds = 4;
        private static int kRounds = 0x10;
        private static int kSaltLen = 2;
        private static int kZeroLen = 7;

        public static string Decode(byte[] encodedDataBytes)
        {
            byte[] bytes = Encoding.UTF8.GetBytes("msdkmsdkmsdkmsdk");
            int length = encodedDataBytes.Length;
            byte[] outBuf = new byte[length + 1];
            if (oi_symmetry_decrypt2(encodedDataBytes, encodedDataBytes.Length, bytes, outBuf, ref length) != 0)
            {
                return string.Empty;
            }
            return Encoding.UTF8.GetString(outBuf, 0, length);
        }

        public static string Encode(string rawData)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(rawData);
            byte[] key = Encoding.UTF8.GetBytes("msdkmsdkmsdkmsdk");
            int outBufLen = 0;
            byte[] outBuf = new byte[bytes.Length + 0x12];
            oi_symmetry_encrypt2(bytes, bytes.Length, key, outBuf, ref outBufLen);
            return Convert.ToBase64String(outBuf, 0, outBufLen);
        }

        public static int oi_symmetry_decrypt2(byte[] inBuf, int inBufLen, byte[] key, byte[] outBuf, ref int outBufLen)
        {
            int num7;
            int sourceIndex = 0;
            int num2 = 0;
            byte[] buffer = new byte[8];
            byte[] buffer2 = new byte[8];
            byte[] destinationArray = new byte[8];
            byte[] buffer4 = new byte[8];
            int num8 = 0;
            if (((inBufLen % 8) != 0) || (inBufLen < 0x10))
            {
                return -1;
            }
            TeaDecryptECB(inBuf, key, buffer);
            int num3 = buffer[0] & 7;
            int index = (((inBufLen - 1) - num3) - kSaltLen) - kZeroLen;
            if ((outBufLen < index) || (index < 0))
            {
                return -1;
            }
            outBufLen = index;
            for (index = 0; index < 8; index++)
            {
                buffer2[index] = 0;
            }
            Array.Copy(inBuf, 0, buffer4, 0, 8);
            sourceIndex += 8;
            num8 += 8;
            int num5 = 1;
            num5 += num3;
            index = 1;
            while (index <= kSaltLen)
            {
                if (num5 < 8)
                {
                    num5++;
                    index++;
                }
                else if (num5 == 8)
                {
                    Array.Copy(buffer4, 0, destinationArray, 0, 8);
                    Array.Copy(inBuf, sourceIndex, buffer4, 0, 8);
                    num7 = 0;
                    while (num7 < 8)
                    {
                        if ((num8 + num7) >= inBufLen)
                        {
                            return -1;
                        }
                        buffer[num7] = (byte) (buffer[num7] ^ inBuf[sourceIndex + num7]);
                        num7++;
                    }
                    TeaDecryptECB(buffer, key, buffer);
                    sourceIndex += 8;
                    num8 += 8;
                    num5 = 0;
                }
            }
            int num4 = outBufLen;
            while (num4 != 0)
            {
                if (num5 < 8)
                {
                    outBuf[num2++] = (byte) (buffer[num5] ^ destinationArray[num5]);
                    num5++;
                    num4--;
                }
                else if (num5 == 8)
                {
                    Array.Copy(buffer4, 0, destinationArray, 0, 8);
                    Array.Copy(inBuf, sourceIndex, buffer4, 0, 8);
                    num7 = 0;
                    while (num7 < 8)
                    {
                        if ((num8 + num7) >= inBufLen)
                        {
                            return -1;
                        }
                        buffer[num7] = (byte) (buffer[num7] ^ inBuf[sourceIndex + num7]);
                        num7++;
                    }
                    TeaDecryptECB(buffer, key, buffer);
                    sourceIndex += 8;
                    num8 += 8;
                    num5 = 0;
                }
            }
            index = 1;
            while (index <= kZeroLen)
            {
                if (num5 < 8)
                {
                    if ((buffer[num5] ^ destinationArray[num5]) != 0)
                    {
                        return -1;
                    }
                    num5++;
                    index++;
                }
                else if (num5 == 8)
                {
                    Array.Copy(buffer4, 0, destinationArray, 0, 8);
                    Array.Copy(inBuf, sourceIndex, buffer4, 0, 8);
                    for (num7 = 0; num7 < 8; num7++)
                    {
                        if ((num8 + num7) >= inBufLen)
                        {
                            return -1;
                        }
                        buffer[num7] = (byte) (buffer[num7] ^ inBuf[sourceIndex + num7]);
                    }
                    TeaDecryptECB(buffer, key, buffer);
                    sourceIndex += 8;
                    num8 += 8;
                    num5 = 0;
                }
            }
            return 0;
        }

        public static void oi_symmetry_encrypt2(byte[] inBuf, int inBufLen, byte[] key, byte[] outBuf, ref int outBufLen)
        {
            int num6;
            int num7;
            int num = 0;
            int outBufStartPos = 0;
            byte[] buffer = new byte[8];
            byte[] buffer2 = new byte[8];
            byte[] destinationArray = new byte[8];
            int num3 = ((inBufLen + 1) + kSaltLen) + kZeroLen;
            int num4 = num3 % 8;
            if (num4 != 0)
            {
                num4 = 8 - num4;
            }
            Random random = new Random();
            buffer[0] = (byte) ((random.Next(0x100) & 0xf8) | num4);
            int num5 = 1;
            while (num4-- != 0)
            {
                buffer[num5++] = (byte) random.Next(0x100);
            }
            for (num6 = 0; num6 < 8; num6++)
            {
                buffer2[num6] = 0;
            }
            outBufLen = 0;
            num6 = 1;
            while (num6 <= kSaltLen)
            {
                if (num5 < 8)
                {
                    buffer[num5++] = (byte) random.Next(0x100);
                    num6++;
                }
                if (num5 == 8)
                {
                    num7 = 0;
                    while (num7 < 8)
                    {
                        buffer[num7] = (byte) (buffer[num7] ^ destinationArray[num7]);
                        num7++;
                    }
                    TeaEncryptECB(buffer, key, outBuf, outBufStartPos);
                    num7 = 0;
                    while (num7 < 8)
                    {
                        outBuf[outBufStartPos + num7] = (byte) (outBuf[outBufStartPos + num7] ^ buffer2[num7]);
                        num7++;
                    }
                    num7 = 0;
                    while (num7 < 8)
                    {
                        buffer2[num7] = buffer[num7];
                        num7++;
                    }
                    num5 = 0;
                    Array.Copy(outBuf, outBufStartPos, destinationArray, 0, 8);
                    outBufLen += 8;
                    outBufStartPos += 8;
                }
            }
            while (inBufLen != 0)
            {
                if (num5 < 8)
                {
                    buffer[num5++] = inBuf[num++];
                    inBufLen--;
                }
                if (num5 == 8)
                {
                    num7 = 0;
                    while (num7 < 8)
                    {
                        buffer[num7] = (byte) (buffer[num7] ^ destinationArray[num7]);
                        num7++;
                    }
                    TeaEncryptECB(buffer, key, outBuf, outBufStartPos);
                    num7 = 0;
                    while (num7 < 8)
                    {
                        outBuf[outBufStartPos + num7] = (byte) (outBuf[outBufStartPos + num7] ^ buffer2[num7]);
                        num7++;
                    }
                    num7 = 0;
                    while (num7 < 8)
                    {
                        buffer2[num7] = buffer[num7];
                        num7++;
                    }
                    num5 = 0;
                    Array.Copy(outBuf, outBufStartPos, destinationArray, 0, 8);
                    outBufLen += 8;
                    outBufStartPos += 8;
                }
            }
            num6 = 1;
            while (num6 <= kZeroLen)
            {
                if (num5 < 8)
                {
                    buffer[num5++] = 0;
                    num6++;
                }
                if (num5 == 8)
                {
                    num7 = 0;
                    while (num7 < 8)
                    {
                        buffer[num7] = (byte) (buffer[num7] ^ destinationArray[num7]);
                        num7++;
                    }
                    TeaEncryptECB(buffer, key, outBuf, outBufStartPos);
                    num7 = 0;
                    while (num7 < 8)
                    {
                        outBuf[outBufStartPos + num7] = (byte) (outBuf[outBufStartPos + num7] ^ buffer2[num7]);
                        num7++;
                    }
                    for (num7 = 0; num7 < 8; num7++)
                    {
                        buffer2[num7] = buffer[num7];
                    }
                    num5 = 0;
                    Array.Copy(outBuf, outBufStartPos, destinationArray, 0, 8);
                    outBufLen += 8;
                    outBufStartPos += 8;
                }
            }
        }

        public static int oi_symmetry_encrypt2_len(int nInBufLen)
        {
            int num = ((nInBufLen + 1) + kSaltLen) + kZeroLen;
            int num2 = num % 8;
            if (num2 != 0)
            {
                num2 = 8 - num2;
            }
            return (num + num2);
        }

        private static void TeaDecryptECB(byte[] inBuf, byte[] key, byte[] outBuf)
        {
            int num4;
            uint[] numArray = new uint[4];
            uint num = (uint) IPAddress.NetworkToHostOrder(BitConverter.ToInt32(inBuf, 0));
            uint num2 = (uint) IPAddress.NetworkToHostOrder(BitConverter.ToInt32(inBuf, 4));
            for (num4 = 0; num4 < 4; num4++)
            {
                numArray[num4] = (uint) IPAddress.NetworkToHostOrder(BitConverter.ToInt32(key, num4 * 4));
            }
            uint num3 = kDelta << kLogRounds;
            for (num4 = 0; num4 < kRounds; num4++)
            {
                num2 -= (((num << 4) + numArray[2]) ^ (num + num3)) ^ ((num >> 5) + numArray[3]);
                num -= (((num2 << 4) + numArray[0]) ^ (num2 + num3)) ^ ((num2 >> 5) + numArray[1]);
                num3 -= kDelta;
            }
            byte[] bytes = BitConverter.GetBytes((uint) IPAddress.HostToNetworkOrder((int) num));
            byte[] sourceArray = BitConverter.GetBytes((uint) IPAddress.HostToNetworkOrder((int) num2));
            Array.Copy(bytes, 0, outBuf, 0, 4);
            Array.Copy(sourceArray, 0, outBuf, 4, 4);
        }

        private static void TeaEncryptECB(byte[] inBuf, byte[] key, byte[] outBuf, int outBufStartPos)
        {
            int num4;
            uint[] numArray = new uint[4];
            uint num = (uint) IPAddress.NetworkToHostOrder(BitConverter.ToInt32(inBuf, 0));
            uint num2 = (uint) IPAddress.NetworkToHostOrder(BitConverter.ToInt32(inBuf, 4));
            for (num4 = 0; num4 < 4; num4++)
            {
                numArray[num4] = (uint) IPAddress.NetworkToHostOrder(BitConverter.ToInt32(key, num4 * 4));
            }
            uint num3 = 0;
            for (num4 = 0; num4 < kRounds; num4++)
            {
                num3 += kDelta;
                num += (((num2 << 4) + numArray[0]) ^ (num2 + num3)) ^ ((num2 >> 5) + numArray[1]);
                num2 += (((num << 4) + numArray[2]) ^ (num + num3)) ^ ((num >> 5) + numArray[3]);
            }
            byte[] bytes = BitConverter.GetBytes((uint) IPAddress.HostToNetworkOrder((int) num));
            byte[] sourceArray = BitConverter.GetBytes((uint) IPAddress.HostToNetworkOrder((int) num2));
            Array.Copy(bytes, 0, outBuf, outBufStartPos, 4);
            Array.Copy(sourceArray, 0, outBuf, outBufStartPos + 4, 4);
        }
    }
}

