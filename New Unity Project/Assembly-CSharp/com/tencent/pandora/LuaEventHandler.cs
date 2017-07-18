namespace com.tencent.pandora
{
    using System;

    public class LuaEventHandler
    {
        public LuaFunction handler;

        public void handleEvent(object[] args)
        {
            this.handler.Call(args);
        }
    }
}

