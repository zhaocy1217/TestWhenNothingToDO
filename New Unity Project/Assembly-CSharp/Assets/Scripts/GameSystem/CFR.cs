namespace Assets.Scripts.GameSystem
{
    using CSProtocol;
    using System;
    using System.Runtime.CompilerServices;

    public class CFR
    {
        private COMDT_FRIEND_INFO _friendInfo;
        [CompilerGenerated]
        private int <CDDays>k__BackingField;
        public bool bInShowChoiseRelaList;
        public bool bReciveOthersRequest;
        public bool bRedDot;
        public int choiseRelation = -1;
        public COM_INTIMACY_RELATION_CHG_TYPE op;
        public COM_INTIMACY_STATE state;
        private uint timeStamp;
        public ulong ulluid;
        public uint worldID;

        public CFR(ulong ulluid, uint worldId, COM_INTIMACY_STATE state, COM_INTIMACY_RELATION_CHG_TYPE op, uint timeStamp, bool bReciveOthersRequest)
        {
            this.SetData(ulluid, worldId, state, op, timeStamp, bReciveOthersRequest);
        }

        public void Clear()
        {
            this._friendInfo = null;
        }

        public static int GetCDDays(uint ts)
        {
            if (ts == 0)
            {
                return -1;
            }
            DateTime time = Utility.ToUtcTime2Local((long) ts).AddDays((double) Singleton<CFriendContoller>.instance.model.FRData.InitmacyLimitTime);
            DateTime time2 = Utility.ToUtcTime2Local((long) CRoleInfo.GetCurrentUTCTime());
            if (DateTime.Compare(time, time2) <= 0)
            {
                return -1;
            }
            TimeSpan span = (TimeSpan) (time - time2);
            if (span.Days < 1)
            {
                return 1;
            }
            return span.Days;
        }

        public void SetData(ulong ulluid, uint worldId, COM_INTIMACY_STATE state, COM_INTIMACY_RELATION_CHG_TYPE op, uint timeStamp, bool bReciveOthersRequest)
        {
            this.ulluid = ulluid;
            this.worldID = worldId;
            this.state = state;
            this.op = op;
            this.SetTimeStamp(timeStamp);
            this.bReciveOthersRequest = bReciveOthersRequest;
            this._friendInfo = null;
            COMDT_FRIEND_INFO friendInfo = this.friendInfo;
            if (bReciveOthersRequest && (((state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY_CONFIRM) || (state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY_DENY)) || ((state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER_CONFIRM) || (state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER_DENY))))
            {
                this.bRedDot = true;
                Singleton<EventRouter>.instance.BroadCastEvent("Friend_LobbyIconRedDot_Refresh");
            }
        }

        private void SetTimeStamp(uint ts)
        {
            this.timeStamp = ts;
            this.CDDays = GetCDDays(ts);
        }

        public int CDDays
        {
            [CompilerGenerated]
            get
            {
                return this.<CDDays>k__BackingField;
            }
            [CompilerGenerated]
            protected set
            {
                this.<CDDays>k__BackingField = value;
            }
        }

        public COMDT_FRIEND_INFO friendInfo
        {
            get
            {
                if (this._friendInfo == null)
                {
                    this._friendInfo = Singleton<CFriendContoller>.instance.model.getFriendByUid(this.ulluid, CFriendModel.FriendType.GameFriend, 0);
                }
                return this._friendInfo;
            }
        }
    }
}

