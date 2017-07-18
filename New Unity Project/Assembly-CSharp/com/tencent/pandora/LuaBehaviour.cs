namespace com.tencent.pandora
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;

    public class LuaBehaviour : View
    {
        private AssetBundle bundle;
        private List<LuaFunction> buttons = new List<LuaFunction>();
        private string data;
        protected static bool initialize;

        public void AddClick(GameObject go, LuaFunction luafunc)
        {
        }

        public void AddUGUIClick(GameObject go, LuaFunction luafunc)
        {
            <AddUGUIClick>c__AnonStorey2F storeyf = new <AddUGUIClick>c__AnonStorey2F();
            storeyf.luafunc = luafunc;
            storeyf.go = go;
            storeyf.<>f__this = this;
            if (storeyf.go != null)
            {
                storeyf.go.GetComponent<Button>().get_onClick().AddListener(new UnityAction(storeyf, (IntPtr) this.<>m__1));
            }
        }

        protected void Awake()
        {
            object[] args = new object[] { base.get_gameObject() };
            this.CallMethod("Awake", args);
        }

        protected object[] CallMethod(string func, params object[] args)
        {
            if (!initialize)
            {
                return null;
            }
            return Util.CallMethod(base.get_name(), func, args);
        }

        public void ClearClick()
        {
            for (int i = 0; i < this.buttons.Count; i++)
            {
                if (this.buttons[i] != null)
                {
                    this.buttons[i].Dispose();
                    this.buttons[i] = null;
                }
            }
        }

        public GameObject GetGameObject(string name)
        {
            if (this.bundle == null)
            {
                return null;
            }
            return Util.LoadAsset(this.bundle, name);
        }

        protected void OnClick()
        {
            this.CallMethod("OnClick", new object[0]);
        }

        protected void OnClickEvent(GameObject go)
        {
            object[] args = new object[] { go };
            this.CallMethod("OnClick", args);
        }

        protected void OnDestroy()
        {
            if (this.bundle != null)
            {
                this.bundle.Unload(true);
                this.bundle = null;
            }
            this.ClearClick();
            base.LuaManager = null;
            Util.ClearMemory();
        }

        public void OnInit(AssetBundle bundle, string text = new string())
        {
            this.data = text;
            this.bundle = bundle;
        }

        protected void Start()
        {
            if ((base.LuaManager != null) && initialize)
            {
                LuaState lua = base.LuaManager.lua;
            }
            this.CallMethod("Start", new object[0]);
        }

        [CompilerGenerated]
        private sealed class <AddUGUIClick>c__AnonStorey2F
        {
            internal LuaBehaviour <>f__this;
            internal GameObject go;
            internal LuaFunction luafunc;

            internal void <>m__1()
            {
                object[] args = new object[] { this.go };
                this.luafunc.Call(args);
                this.<>f__this.buttons.Add(this.luafunc);
            }
        }
    }
}

