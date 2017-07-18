namespace com.tencent.pandora
{
    using System;
    using System.Collections.Generic;

    public class Message
    {
        public Action<int, Dictionary<string, object>> action;
        public Dictionary<string, object> content = new Dictionary<string, object>();
        public int status;
    }
}

