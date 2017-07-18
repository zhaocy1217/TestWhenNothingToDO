namespace com.tencent.pandora
{
    using System;
    using UnityEngine;

    public class Pdr : MonoBehaviour
    {
        public string GetTempPath()
        {
            return Application.get_temporaryCachePath();
        }

        public void Log(string strMsg)
        {
            Logger.d(strMsg);
        }

        private void Start()
        {
        }

        private void Update()
        {
        }
    }
}

