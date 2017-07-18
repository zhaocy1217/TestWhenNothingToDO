namespace com.tencent.pandora
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class GameManager : LuaBehaviour
    {
        private List<string> downloadFiles = new List<string>();
        public static bool isInitUlua;

        public void Awake()
        {
        }

        private void FixedUpdate()
        {
            if (!Pandora.NotDoUpdate && ((base.LuaManager != null) && LuaBehaviour.initialize))
            {
                base.LuaManager.FixedUpdate();
            }
        }

        public void Init()
        {
            Object.DontDestroyOnLoad(base.get_gameObject());
            this.OnResourceInited();
        }

        public void InitGui()
        {
            string str = "UI Root";
            if (GameObject.Find(str) != null)
            {
            }
        }

        private void LateUpdate()
        {
            if (!Pandora.NotDoUpdate && ((base.LuaManager != null) && LuaBehaviour.initialize))
            {
                base.LuaManager.LateUpate();
            }
        }

        private void OnDestroy()
        {
            if (base.LuaManager != null)
            {
                base.LuaManager.Destroy();
            }
            Logger.d("~GameManager was destroyed");
        }

        public void OnResourceInited()
        {
            try
            {
                Logger.d(" LuaManager.Start()");
                base.LuaManager.Start();
                base.LuaManager.DoString("GameManager");
                LuaBehaviour.initialize = true;
                foreach (object obj2 in base.CallMethod("LuaScriptPanel", new object[0]))
                {
                    string str = obj2.ToString().Trim();
                    if (!string.IsNullOrEmpty(str))
                    {
                        str = str + "Panel";
                        base.LuaManager.DoString(str);
                    }
                }
                base.CallMethod("OnInitOK", new object[0]);
                Logger.d("init lua success");
                isInitUlua = true;
            }
            catch (Exception exception)
            {
                Logger.d("init lua error : " + exception.Message);
            }
        }

        private void Update()
        {
            if (!Pandora.NotDoUpdate && ((base.LuaManager != null) && LuaBehaviour.initialize))
            {
                base.LuaManager.Update();
            }
        }
    }
}

