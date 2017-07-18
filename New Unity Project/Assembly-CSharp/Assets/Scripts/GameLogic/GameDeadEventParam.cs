namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct GameDeadEventParam
    {
        public PoolObjHandle<ActorRoot> src;
        public PoolObjHandle<ActorRoot> atker;
        public PoolObjHandle<ActorRoot> orignalAtker;
        public PoolObjHandle<ActorRoot> logicAtker;
        public bool bImmediateRevive;
        public SpawnPoint spawnPoint;
        public GameDeadEventParam(PoolObjHandle<ActorRoot> _src, PoolObjHandle<ActorRoot> _atker, PoolObjHandle<ActorRoot> _orignalAtker, PoolObjHandle<ActorRoot> _logicAtker, bool bFlag, SpawnPoint _spawnPoint = new SpawnPoint())
        {
            this.src = _src;
            this.atker = _atker;
            this.orignalAtker = _orignalAtker;
            this.logicAtker = _logicAtker;
            this.bImmediateRevive = bFlag;
            this.spawnPoint = _spawnPoint;
        }
    }
}

