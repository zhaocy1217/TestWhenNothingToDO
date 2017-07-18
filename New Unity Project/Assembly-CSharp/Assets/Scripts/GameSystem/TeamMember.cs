namespace Assets.Scripts.GameSystem
{
    using System;

    public class TeamMember
    {
        public byte bGradeOfRank;
        public uint dwMemberHeadId;
        public uint dwMemberLevel;
        public uint dwPosOfTeam;
        public string MemberName;
        public string snsHeadUrl;
        public PlayerUniqueID uID = new PlayerUniqueID();
    }
}

