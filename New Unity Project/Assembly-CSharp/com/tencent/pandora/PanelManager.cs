namespace com.tencent.pandora
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class PanelManager : View
    {
        private float _time;
        [CompilerGenerated]
        private static Action<Exception> <>f__am$cache5;
        private Dictionary<string, bool> bundleLock = new Dictionary<string, bool>();
        private Dictionary<string, AssetBundle> bundles = new Dictionary<string, AssetBundle>();
        public Dictionary<string, GameObject> pages = new Dictionary<string, GameObject>();
        private Transform parent;

        public void ClosePanel(string name)
        {
            base.ResManager.startTimer = true;
            name = name + "Panel";
            if (this.pages.ContainsKey(name) && (this.pages[name] != null))
            {
                if (name == "PopNewsFacePanel")
                {
                    NotificationCenter.DefaultCenter().PostNotification(this.pages[name].get_transform(), "OnTplayPopNewsClosePL");
                }
                else
                {
                    NotificationCenter.DefaultCenter().PostNotification(this.pages[name].get_transform(), "OnTplayPopNewsCloseACT");
                }
                Object.Destroy(this.pages[name]);
                if (this.bundles[name] != null)
                {
                    this.bundles[name].Unload(true);
                }
            }
        }

        public void CreatePanel(string name, LuaFunction func = new LuaFunction())
        {
            <CreatePanel>c__AnonStorey30 storey = new <CreatePanel>c__AnonStorey30();
            storey.name = name;
            storey.func = func;
            storey.<>f__this = this;
            this._time = Time.get_time();
            base.ResManager.startTimer = false;
            if (Pandora.goParent != null)
            {
                Logger.d("Pandora.GetInstance().goParent != null");
                this.parent = Pandora.goParent.get_transform();
            }
            else
            {
                Logger.d("Pandora.GetInstance().goParent == null ,parent == UI Root");
                Pandora.SetCloseIcon();
            }
            Action<AssetBundle> reFunc = new Action<AssetBundle>(storey.<>m__2);
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = delegate (Exception ex) {
                    Logger.d("load panel bundle return error :" + ex.Message);
                    Pandora.SetCloseIcon();
                };
            }
            Action<Exception> onerror = <>f__am$cache5;
            storey.name = storey.name + "Panel";
            if ((this.bundleLock != null) && this.bundleLock.ContainsKey(storey.name))
            {
                if (!this.bundleLock[storey.name])
                {
                    this.bundleLock[storey.name] = true;
                    base.ResManager.LoadBundleRes(storey.name, reFunc, onerror);
                }
            }
            else
            {
                this.bundleLock[storey.name] = true;
                base.ResManager.LoadBundleRes(storey.name, reFunc, onerror);
            }
        }

        [DebuggerHidden]
        private IEnumerator StartCreatePanel(string name, AssetBundle bundle, LuaFunction func = new LuaFunction())
        {
            <StartCreatePanel>c__Iterator0 iterator = new <StartCreatePanel>c__Iterator0();
            iterator.bundle = bundle;
            iterator.name = name;
            iterator.func = func;
            iterator.<$>bundle = bundle;
            iterator.<$>name = name;
            iterator.<$>func = func;
            iterator.<>f__this = this;
            return iterator;
        }

        private Transform Parent
        {
            get
            {
                if (this.parent == null)
                {
                    GameObject obj2 = GameObject.Find("UI Root");
                    if (obj2 != null)
                    {
                        this.parent = obj2.get_transform();
                    }
                }
                return this.parent;
            }
        }

        [CompilerGenerated]
        private sealed class <CreatePanel>c__AnonStorey30
        {
            internal PanelManager <>f__this;
            internal LuaFunction func;
            internal string name;

            internal void <>m__2(AssetBundle ab)
            {
                if (ab == null)
                {
                    Logger.LogNetError(0x3ec, "load panel " + this.name + "failed ,call lua function OnCreate with null parameter");
                    Logger.d("load panel " + this.name + "failed ,call lua function OnCreate with null parameter");
                    Pandora.SetCloseIcon();
                }
                else
                {
                    AssetBundle bundle = ab;
                    this.<>f__this.bundles[this.name] = bundle;
                    this.<>f__this.StartCoroutine(this.<>f__this.StartCreatePanel(this.name, bundle, this.func));
                    Logger.d("PanelManager: Lua call me , CreatePanel .." + this.name);
                }
            }
        }

        [CompilerGenerated]
        private sealed class <StartCreatePanel>c__Iterator0 : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal AssetBundle <$>bundle;
            internal LuaFunction <$>func;
            internal string <$>name;
            internal PanelManager <>f__this;
            internal GameObject <go>__1;
            internal GameObject <prefab>__0;
            internal AssetBundle bundle;
            internal LuaFunction func;
            internal string name;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.<prefab>__0 = Util.LoadAsset(this.bundle, this.name);
                        this.$current = new WaitForEndOfFrame();
                        this.$PC = 1;
                        goto Label_01D6;

                    case 1:
                        if ((this.<>f__this.Parent.FindChild(this.name) == null) && (this.<prefab>__0 != null))
                        {
                            this.<go>__1 = Object.Instantiate(this.<prefab>__0) as GameObject;
                            this.<go>__1.set_name(this.name);
                            this.<go>__1.get_transform().set_parent(this.<>f__this.Parent);
                            this.<go>__1.get_transform().set_localScale(Vector3.get_one());
                            this.<go>__1.get_transform().set_localPosition(Vector3.get_zero());
                            this.$current = new WaitForEndOfFrame();
                            this.$PC = 2;
                            goto Label_01D6;
                        }
                        break;

                    case 2:
                        this.<go>__1.AddComponent<LuaBehaviour>().OnInit(this.bundle, null);
                        this.<>f__this.pages[this.name] = this.<go>__1;
                        this.<>f__this.bundleLock[this.name] = false;
                        this.<>f__this.ResManager.AssemplyUI(null);
                        if (this.func != null)
                        {
                            object[] args = new object[] { this.<go>__1 };
                            this.func.Call(args);
                        }
                        Logger.d("open panel" + this.name + " success");
                        Logger.d("panel open time ：" + (Time.get_time() - this.<>f__this._time));
                        this.$PC = -1;
                        break;
                }
                return false;
            Label_01D6:
                return true;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }
    }
}

