namespace Assets.Scripts.GameLogic
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct SFilterMatchParam
    {
        public CommonSpawnGroup csg;
        public SpawnGroup sg;
        public HurtEventResultInfo hurtInfo;
        public SkillSlotType slot;
        public int intParam;
    }
}

