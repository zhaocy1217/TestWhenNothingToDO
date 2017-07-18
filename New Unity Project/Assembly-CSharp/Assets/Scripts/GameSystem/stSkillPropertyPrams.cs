namespace Assets.Scripts.GameSystem
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct stSkillPropertyPrams
    {
        public string name;
        public uint valueType;
        public float baseValue;
        public float growthValue;
    }
}

