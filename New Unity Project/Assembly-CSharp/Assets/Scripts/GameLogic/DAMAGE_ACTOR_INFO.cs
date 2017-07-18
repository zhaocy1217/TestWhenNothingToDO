namespace Assets.Scripts.GameLogic
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct DAMAGE_ACTOR_INFO
    {
        public string actorName;
        public string playerName;
        public ActorTypeDef actorType;
        public int ConfigId;
        public byte actorSubType;
        public byte bMonsterType;
        public SortedList<ulong, DOUBLE_INT_INFO[]> listDamage;
    }
}

