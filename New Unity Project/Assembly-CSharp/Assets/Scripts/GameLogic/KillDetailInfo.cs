namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using System;
    using System.Collections.Generic;

    public class KillDetailInfo
    {
        public List<uint> assistList;
        public bool bAllDead;
        public bool bPlayerSelf_KillOrKilled;
        public bool bSelfCamp;
        public KillDetailInfoType HeroContiKillType;
        public KillDetailInfoType HeroMultiKillType;
        public PoolObjHandle<ActorRoot> Killer;
        public KillDetailInfoType Type;
        public PoolObjHandle<ActorRoot> Victim;
    }
}

