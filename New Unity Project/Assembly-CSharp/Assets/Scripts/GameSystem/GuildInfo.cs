namespace Assets.Scripts.GameSystem
{
    using CSProtocol;
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class GuildInfo
    {
        public byte bBuildingCount;
        public stGuildBriefInfo briefInfo = new stGuildBriefInfo();
        public GuildMemInfo chairman = new GuildMemInfo();
        public uint dwActive;
        public uint dwCoinPool;
        public uint dwGuildMoney;
        public uint groupGuildId;
        public string groupKey;
        public string groupOpenId;
        public COMDT_GUILD_MATCH_INFO GuildMatchInfo = new COMDT_GUILD_MATCH_INFO();
        public ListView<COMDT_GUILD_MATCH_OB_INFO> GuildMatchObInfos = new ListView<COMDT_GUILD_MATCH_OB_INFO>();
        public List<GuildBuildingInfo> listBuildingInfo = new List<GuildBuildingInfo>();
        [NonSerialized]
        public ListView<GuildMemInfo> listMemInfo = new ListView<GuildMemInfo>();
        public List<GuildSelfRecommendInfo> listSelfRecommendInfo = new List<GuildSelfRecommendInfo>();
        public GuildRankInfo RankInfo = new GuildRankInfo();
        public uint star;
        public ulong uulCreatedTime;

        public void Reset()
        {
            this.uulCreatedTime = 0L;
            this.dwActive = 0;
            this.dwCoinPool = 0;
            this.dwGuildMoney = 0;
            this.chairman.Reset();
            this.briefInfo.Reset();
            this.listMemInfo.Clear();
            this.listBuildingInfo.Clear();
            this.listSelfRecommendInfo.Clear();
            this.RankInfo.Reset();
            this.star = 0;
            this.groupKey = null;
            this.groupOpenId = null;
            this.GuildMatchInfo.dwScore = 0;
            this.GuildMatchInfo.dwWeekScore = 0;
            this.GuildMatchInfo.dwLastRankNo = 0;
            this.GuildMatchInfo.dwUpdRankNoTime = 0;
            this.GuildMatchInfo.dwLastSeasonRankNo = 0;
            this.GuildMatchObInfos.Clear();
        }
    }
}

