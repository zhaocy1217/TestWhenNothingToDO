namespace Assets.Scripts.GameSystem
{
    using CSProtocol;
    using System;

    [Serializable]
    public class GuildMemInfo
    {
        public uint CurrActive;
        public byte DonateCnt;
        public uint DonateNum;
        public uint dwConstruct;
        public uint dwGameStartTime;
        public COM_PLAYER_GUILD_STATE enPosition;
        public COM_ACNT_GAME_STATE GameState;
        public COMDT_MEMBER_GUILD_MATCH_INFO GuildMatchInfo = new COMDT_MEMBER_GUILD_MATCH_INFO();
        public uint JoinTime;
        public uint LastLoginTime;
        public MemberRankInfo RankInfo = new MemberRankInfo();
        public stGuildMemBriefInfo stBriefInfo;
        public uint TotalContruct;
        public uint WeekActive;
        public uint WeekDividend;

        public override bool Equals(object obj)
        {
            GuildMemInfo info = obj as GuildMemInfo;
            return ((info != null) && (this.stBriefInfo.uulUid == info.stBriefInfo.uulUid));
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public void Reset()
        {
            this.stBriefInfo.Reset();
            this.enPosition = COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_MEMBER;
            this.dwConstruct = 0;
            this.TotalContruct = 0;
            this.CurrActive = 0;
            this.WeekActive = 0;
            this.DonateCnt = 0;
            this.DonateNum = 0;
            this.WeekDividend = 0;
            this.RankInfo.Reset();
            this.GameState = COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_IDLE;
            this.LastLoginTime = 0;
            this.JoinTime = 0;
            this.GuildMatchInfo.bIsLeader = 0;
            this.GuildMatchInfo.ullTeamLeaderUid = 0L;
            this.GuildMatchInfo.dwScore = 0;
        }
    }
}

