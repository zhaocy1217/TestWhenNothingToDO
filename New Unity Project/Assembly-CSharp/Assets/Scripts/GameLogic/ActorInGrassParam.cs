namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct ActorInGrassParam
    {
        public PoolObjHandle<ActorRoot> _src;
        public bool _bInGrass;
        public ActorInGrassParam(PoolObjHandle<ActorRoot> src, bool bInGrass)
        {
            this._src = src;
            this._bInGrass = bInGrass;
        }
    }
}

