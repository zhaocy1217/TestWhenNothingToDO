namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct SpawnEyeEventParam
    {
        public PoolObjHandle<ActorRoot> src;
        public VInt3 pos;
        public SpawnEyeEventParam(PoolObjHandle<ActorRoot> _src, VInt3 _pos)
        {
            this.src = _src;
            this.pos = _pos;
        }
    }
}

