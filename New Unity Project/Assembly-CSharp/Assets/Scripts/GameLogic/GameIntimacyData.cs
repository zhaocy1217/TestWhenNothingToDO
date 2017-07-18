namespace Assets.Scripts.GameLogic
{
    using CSProtocol;
    using System;

    public class GameIntimacyData
    {
        public COM_INTIMACY_STATE state;
        public string title;
        public ulong ulluid;
        public uint worldId;

        public GameIntimacyData(COM_INTIMACY_STATE state, ulong ulluid, uint worldId, string title)
        {
            this.state = state;
            this.ulluid = ulluid;
            this.worldId = worldId;
            this.title = title;
        }
    }
}

