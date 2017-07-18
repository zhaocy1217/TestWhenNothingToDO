namespace Assets.Scripts.GameLogic
{
    using System;

    public static class HorizonConfig
    {
        public static bool[,] RelationMap = new bool[,] { true, false, true, true, false, true, false, false };

        public enum HideMark
        {
            Jungle,
            Skill,
            COUNT,
            INVALID
        }

        public enum ShowMark
        {
            Jungle,
            Skill,
            Organ,
            COUNT,
            INVALID
        }
    }
}

