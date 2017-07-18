namespace com.tencent.pandora
{
    using System;
    using UnityEngine;

    public class TimerInfo
    {
        public string className;
        public bool delete;
        public bool stop;
        public Object target;
        public long tick;

        public TimerInfo(string className, Object target)
        {
            this.className = className;
            this.target = target;
            this.delete = false;
        }
    }
}

