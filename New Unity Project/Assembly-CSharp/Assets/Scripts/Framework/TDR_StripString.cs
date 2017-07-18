namespace Assets.Scripts.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEngine;

    public static class TDR_StripString
    {
        public static int bytes;
        public static int savedBytes;
        public static double time;

        public static void exec(Dictionary<long, object>.ValueCollection items, Type InKeyType, Type InValueType)
        {
            object[] array = new object[items.Count];
            items.CopyTo(array, 0);
            exec(array, InKeyType, InValueType);
        }

        public static void exec(object[] items, Type InKeyType, Type InValueType)
        {
            float num = Time.get_realtimeSinceStartup();
            foreach (FieldInfo info in InValueType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
            {
                if (info.FieldType.IsArray && (info.FieldType.GetElementType() == typeof(byte)))
                {
                    for (int i = 0; i < items.Length; i++)
                    {
                        object obj2 = items[i];
                        if (obj2 != null)
                        {
                            byte[] src = (byte[]) info.GetValue(obj2);
                            byte[] buffer2 = strip(src);
                            if (buffer2 != src)
                            {
                                info.SetValue(obj2, buffer2);
                            }
                        }
                    }
                }
            }
            float num4 = Time.get_realtimeSinceStartup();
            time += num4 - num;
        }

        private static byte[] strip(byte[] src)
        {
            if ((src == null) || (src.Length == 0))
            {
                return src;
            }
            int length = -1;
            for (int i = 0; i < src.Length; i++)
            {
                if (src[i] == 0)
                {
                    length = i;
                    break;
                }
            }
            if (length == -1)
            {
                return src;
            }
            byte[] destinationArray = new byte[length + 1];
            Array.Copy(src, destinationArray, length);
            destinationArray[length] = 0;
            bytes += length + 1;
            savedBytes += (src.Length - length) - 1;
            return destinationArray;
        }
    }
}

