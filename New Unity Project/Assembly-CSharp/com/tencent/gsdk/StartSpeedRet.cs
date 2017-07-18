namespace com.tencent.gsdk
{
    using System;

    public class StartSpeedRet
    {
        public string desc;
        public int flag;
        public int type;

        public StartSpeedRet(int type, int flag, string desc)
        {
            this.type = type;
            this.flag = flag;
            this.desc = desc;
        }

        public override string ToString()
        {
            object[] objArray1 = new object[] { "type: ", this.type, "flag: ", this.flag, "desc: ", this.desc, "\n" };
            return string.Concat(objArray1);
        }
    }
}

