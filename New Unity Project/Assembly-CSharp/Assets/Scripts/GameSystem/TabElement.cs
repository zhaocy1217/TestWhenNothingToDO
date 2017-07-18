namespace Assets.Scripts.GameSystem
{
    using System;
    using System.Runtime.InteropServices;

    public class TabElement
    {
        public byte camp;
        public uint cfgId;
        public string configContent;
        public string selfDefContent;
        public Type type;

        public TabElement(string selfDef = "")
        {
            this.type = Type.SelfDef;
            this.cfgId = 0;
            this.configContent = null;
            this.selfDefContent = selfDef;
        }

        public TabElement(uint cfgid, string configContent = "")
        {
            this.type = Type.Config;
            this.cfgId = cfgid;
            this.configContent = configContent;
            this.selfDefContent = null;
        }

        public TabElement Clone()
        {
            TabElement element = new TabElement(string.Empty);
            element.type = this.type;
            element.cfgId = this.cfgId;
            element.configContent = this.configContent;
            element.selfDefContent = this.selfDefContent;
            element.camp = this.camp;
            return element;
        }

        public enum Type
        {
            None,
            Config,
            SelfDef
        }
    }
}

