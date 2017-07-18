namespace Assets.Scripts.GameSystem
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct stGuildBriefInfo
    {
        public ulong uulUid;
        public string sName;
        public byte bLevel;
        public byte bMemberNum;
        public string sBulletin;
        public uint dwHeadId;
        public uint dwSettingMask;
        public uint Rank;
        public uint Ability;
        private byte levelLimit;
        private byte gradeLimit;
        public byte LevelLimit
        {
            get
            {
                return CGuildHelper.GetFixedGuildLevelLimit(this.levelLimit);
            }
            set
            {
                this.levelLimit = value;
            }
        }
        public byte GradeLimit
        {
            get
            {
                return CGuildHelper.GetFixedGuildGradeLimit(this.gradeLimit);
            }
            set
            {
                this.gradeLimit = value;
            }
        }
        public void Reset()
        {
            this.uulUid = 0L;
            this.sName = null;
            this.bLevel = 0;
            this.bMemberNum = 0;
            this.sBulletin = null;
            this.dwHeadId = 0;
            this.dwSettingMask = 0;
            this.Rank = 0;
            this.Ability = 0;
            this.LevelLimit = 0;
            this.GradeLimit = 0;
        }
    }
}

