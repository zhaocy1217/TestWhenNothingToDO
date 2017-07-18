namespace behaviac
{
    using System;
    using System.Diagnostics;
    using UnityEngine;

    public static class Debug
    {
        [Conditional("UNITY_EDITOR"), Conditional("BEHAVIAC_DEBUG")]
        public static void Break(string msg)
        {
            Debug.Break();
        }

        [Conditional("BEHAVIAC_DEBUG"), Conditional("UNITY_EDITOR")]
        public static void Check(bool b)
        {
            if (!b)
            {
            }
        }

        [Conditional("BEHAVIAC_DEBUG"), Conditional("UNITY_EDITOR")]
        public static void Check(bool b, string message)
        {
            if (!b)
            {
            }
        }

        [Conditional("UNITY_EDITOR"), Conditional("BEHAVIAC_DEBUG")]
        public static void Log(string message)
        {
            Debug.Log(message);
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogError(Exception ex)
        {
            Debug.LogError(ex.Message);
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogError(string message)
        {
            Debug.LogError(message);
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogWarning(string message)
        {
            Debug.LogWarning(message);
        }
    }
}

