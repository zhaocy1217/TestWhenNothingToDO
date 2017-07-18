namespace com.tencent.pandora
{
    using System;
    using System.Runtime.CompilerServices;

    public static class ObjectExtends
    {
        public static object RefObject(this object obj)
        {
            return new WeakReference(obj).Target;
        }
    }
}

