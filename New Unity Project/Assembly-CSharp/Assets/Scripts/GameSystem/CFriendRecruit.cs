namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class CFriendRecruit
    {
        [CompilerGenerated]
        private RecruitReward <SuperReward>k__BackingField;
        public static string DataChange = "FriendRecruitDataChange";
        private RecruitData m_beiZhaoMuZhe;
        private uint m_dwRecruiterRewardBits;
        private ListView<ResRecruitmentReward> m_rewardConfig = new ListView<ResRecruitmentReward>();
        private ListView<RecruitData> m_zhaoMuZhe = new ListView<RecruitData>();
        public static int Max_BeiZhaoMuZheCount = 1;
        public static int Max_BeiZhaoMuZheRewardCount = 4;
        public static int Max_ZhaoMuzheCount = 4;
        public static int Max_ZhaoMuzheRewarCount = 3;
        public DictionaryView<ushort, CUseable> useable_cfg = new DictionaryView<ushort, CUseable>();

        public void Check()
        {
            for (int i = 0; i < this.m_zhaoMuZhe.Count; i++)
            {
                RecruitData data = this.m_zhaoMuZhe[i];
                this.CheckCanGetReward(data);
            }
            this.CheckBeiZhaoMuZheReward();
            if (this.GetZhaoMuZhe_RewardProgress() == this.GetZhaoMuZhe_RewardTotalCount())
            {
                this.SuperReward.state = RewardState.Keling;
            }
        }

        public void CheckBeiZhaoMuZheReward()
        {
            if ((this.m_beiZhaoMuZhe != null) && (this.m_beiZhaoMuZhe.userInfo != null))
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
                for (int i = 0; i < this.m_beiZhaoMuZhe.RewardList.Count; i++)
                {
                    RecruitReward reward = this.m_beiZhaoMuZhe.RewardList[i];
                    if (reward.state == RewardState.Normal)
                    {
                        ResRecruitmentReward cfgReward = this.GetCfgReward(reward.rewardID);
                        if (((cfgReward != null) && (masterRoleInfo != null)) && (masterRoleInfo.PvpLevel >= cfgReward.dwLevel))
                        {
                            reward.state = RewardState.Keling;
                        }
                    }
                }
            }
        }

        public void CheckCanGetReward(RecruitData data)
        {
            for (int i = 0; i < data.RewardList.Count; i++)
            {
                RecruitReward reward = data.RewardList[i];
                if (reward.state == RewardState.Normal)
                {
                    ResRecruitmentReward cfgReward = this.GetCfgReward(reward.rewardID);
                    if (((cfgReward != null) && (data.userInfo != null)) && (data.userInfo.dwPvpLvl >= cfgReward.dwLevel))
                    {
                        reward.state = RewardState.Keling;
                    }
                }
            }
        }

        public void Clear()
        {
        }

        public RecruitData GetBeiZhaoMuZhe()
        {
            return this.m_beiZhaoMuZhe;
        }

        public int GetBeiZhaoMuZhe_RewardExp()
        {
            ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x132);
            return ((dataByKey == null) ? -1 : ((int) (dataByKey.dwConfValue / 100)));
        }

        public int GetBeiZhaoMuZhe_RewardGold()
        {
            ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x133);
            return ((dataByKey == null) ? -1 : ((int) (dataByKey.dwConfValue / 100)));
        }

        public ResRecruitmentReward GetCfgReward(ushort rewardID)
        {
            for (int i = 0; i < this.m_rewardConfig.Count; i++)
            {
                ResRecruitmentReward reward = this.m_rewardConfig[i];
                if (reward.wID == rewardID)
                {
                    return reward;
                }
            }
            return null;
        }

        public RecruitData GetRecruitData(ulong ullUid, uint dwLogicWorldId)
        {
            RecruitData zhaoMuZhe = this.GetZhaoMuZhe(ullUid, dwLogicWorldId);
            if (zhaoMuZhe != null)
            {
                return zhaoMuZhe;
            }
            if (this.m_beiZhaoMuZhe.IsEqual(ullUid, dwLogicWorldId))
            {
                return this.m_beiZhaoMuZhe;
            }
            return null;
        }

        public RecruitReward GetRecruitReward(ulong ullUid, uint dwLogicWorldId, ushort rewardID)
        {
            RecruitData zhaoMuZhe = this.GetZhaoMuZhe(ullUid, dwLogicWorldId);
            if (zhaoMuZhe != null)
            {
                return zhaoMuZhe.GetReward(rewardID);
            }
            if (this.m_beiZhaoMuZhe.IsEqual(ullUid, dwLogicWorldId))
            {
                return this.m_beiZhaoMuZhe.GetReward(rewardID);
            }
            return null;
        }

        public static RecruitReward GetReward(ushort rewardID, ListView<RecruitReward> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                RecruitReward reward = list[i];
                if ((reward != null) && (reward.rewardID == rewardID))
                {
                    return reward;
                }
            }
            return null;
        }

        public CUseable GetUsable(ushort id)
        {
            CUseable useable = null;
            this.useable_cfg.TryGetValue(id, out useable);
            if (useable == null)
            {
                ResRecruitmentReward cfgReward = this.GetCfgReward(id);
                useable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, cfgReward.dwRewardID, 0);
                this.useable_cfg.Add(id, useable);
            }
            return useable;
        }

        public RecruitData GetValidRecruitData()
        {
            for (int i = 0; i < this.m_zhaoMuZhe.Count; i++)
            {
                RecruitData data = this.m_zhaoMuZhe[i];
                if ((data != null) && (data.userInfo == null))
                {
                    return data;
                }
            }
            return null;
        }

        public RecruitData GetZhaoMuZhe(ulong ullUid, uint dwLogicWorldId)
        {
            for (int i = 0; i < this.m_zhaoMuZhe.Count; i++)
            {
                RecruitData data = this.m_zhaoMuZhe[i];
                if (data.IsEqual(ullUid, dwLogicWorldId))
                {
                    return data;
                }
            }
            return null;
        }

        public int GetZhaoMuZhe_RewardExp()
        {
            ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x132);
            return ((dataByKey == null) ? -1 : ((int) (dataByKey.dwConfValue / 100)));
        }

        public int GetZhaoMuZhe_RewardGold()
        {
            ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x133);
            return ((dataByKey == null) ? -1 : ((int) (dataByKey.dwConfValue / 100)));
        }

        public int GetZhaoMuZhe_RewardProgress()
        {
            int num = 0;
            for (int i = 0; i < this.m_zhaoMuZhe.Count; i++)
            {
                RecruitData data = this.m_zhaoMuZhe[i];
                if ((data != null) && data.IsGetAllReward())
                {
                    num++;
                }
            }
            return num;
        }

        public int GetZhaoMuZhe_RewardTotalCount()
        {
            return 4;
        }

        public ListView<RecruitData> GetZhaoMuZheRewardList()
        {
            return this.m_zhaoMuZhe;
        }

        public bool HasGetBeiZhaoMuZheReward(RES_RECRUIMENT_BITS type)
        {
            if (type == RES_RECRUIMENT_BITS.RES_RECRUIMENT_BITS_NONE)
            {
                return false;
            }
            uint num = ((uint) 1) << type;
            return ((num & this.m_dwRecruiterRewardBits) != 0);
        }

        public bool HasGetBeiZhaoMuZheReward(int v)
        {
            return this.HasGetBeiZhaoMuZheReward((RES_RECRUIMENT_BITS) v);
        }

        public void InitData()
        {
            for (int i = 0; i < this.m_zhaoMuZhe.Count; i++)
            {
                RecruitData data = this.m_zhaoMuZhe[i];
                if (data != null)
                {
                    data.Clear();
                }
            }
            this.m_zhaoMuZhe.Clear();
            if (this.m_beiZhaoMuZhe != null)
            {
                this.m_beiZhaoMuZhe.Clear();
            }
            this.m_beiZhaoMuZhe = null;
            for (int j = 0; j < Max_ZhaoMuzheCount; j++)
            {
                this.m_zhaoMuZhe.Add(new RecruitData(null, COM_RECRUITMENT_TYPE.COM_RECRUITMENT_ACTIVE));
            }
            this.m_beiZhaoMuZhe = new RecruitData(null, COM_RECRUITMENT_TYPE.COM_RECRUITMENT_PASSIVE);
        }

        public void LoadConfig()
        {
            Dictionary<long, object>.Enumerator enumerator = GameDataMgr.recruimentReward.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<long, object> current = enumerator.Current;
                ResRecruitmentReward item = (ResRecruitmentReward) current.Value;
                if (!this.m_rewardConfig.Contains(item))
                {
                    this.m_rewardConfig.Add(item);
                }
                if ((item.bRecruimentType == 1) && (item.bRewardBit == 5))
                {
                    this.SuperReward = null;
                    this.SuperReward = new RecruitReward(item.wID, RewardState.Normal);
                }
            }
            this.InitData();
            for (int i = 0; i < this.m_zhaoMuZhe.Count; i++)
            {
                RecruitData data = this.m_zhaoMuZhe[i];
                this.ParseConfig(data);
            }
            this.ParseConfig(this.m_beiZhaoMuZhe);
        }

        public void ParseConfig(RecruitData data)
        {
            for (int i = 0; i < this.m_rewardConfig.Count; i++)
            {
                ResRecruitmentReward reward = this.m_rewardConfig[i];
                if (((byte) data.type) == reward.bRecruimentType)
                {
                    RecruitReward reward2 = data.GetReward(reward.wID);
                    if ((reward2 == null) || (reward2.state != RewardState.Getted))
                    {
                        data.SetReward(reward.wID, RewardState.Normal);
                    }
                }
            }
        }

        public void ParseFriend(COMDT_FRIEND_INFO stFriendInfo, COMDT_RECRUITMENT_DATA stRecruitmentInfo)
        {
            if ((stFriendInfo != null) && (stRecruitmentInfo != null))
            {
                if (stRecruitmentInfo.bRecruitmentType == 1)
                {
                    COMDT_RECRUITMENT_ACTIVE stRecruitmentActive = stRecruitmentInfo.stRecruitmentInfo.stRecruitmentActive;
                    if (this.GetZhaoMuZhe(stFriendInfo.stUin.ullUid, stFriendInfo.stUin.dwLogicWorldId) == null)
                    {
                        this.SetZhaoMuZhe(stFriendInfo);
                    }
                    for (int i = 0; i < stRecruitmentActive.dwActiveRewardNum; i++)
                    {
                        ushort rewardID = stRecruitmentActive.ActiveRewardList[i];
                        this.SetZhaoMuZheRewardData(stFriendInfo, rewardID, RewardState.Getted);
                    }
                }
                else if (stRecruitmentInfo.bRecruitmentType == 2)
                {
                    this.m_beiZhaoMuZhe.userInfo = stFriendInfo;
                }
            }
        }

        public void RemoveRecruitData(ulong ullUid, uint dwLogicWorldId)
        {
            RecruitData recruitData = this.GetRecruitData(ullUid, dwLogicWorldId);
            if (recruitData != null)
            {
                recruitData.ResetReward();
                recruitData.userInfo = null;
            }
        }

        public void SetBeiZhaoMuZheRewardData(COMDT_FRIEND_INFO friendData)
        {
            this.m_beiZhaoMuZhe.userInfo = friendData;
        }

        public void SetBITS(RES_RECRUIMENT_BITS type, bool bGetted)
        {
            if (bGetted)
            {
                uint num = ((uint) 1) << type;
                this.m_dwRecruiterRewardBits |= num;
            }
            else
            {
                uint num2 = ((uint) 1) << type;
                num2 = ~num2;
                this.m_dwRecruiterRewardBits &= num2;
            }
        }

        public void SetRecruiterRewardBits(uint m_dwRecruiterRewardBits)
        {
            this.m_dwRecruiterRewardBits = m_dwRecruiterRewardBits;
            for (int i = 0; i < this.m_beiZhaoMuZhe.RewardList.Count; i++)
            {
                RecruitReward reward = this.m_beiZhaoMuZhe.RewardList[i];
                if (this.HasGetBeiZhaoMuZheReward(reward.cfg.bRewardBit))
                {
                    reward.state = RewardState.Getted;
                }
                else if ((this.m_beiZhaoMuZhe.userInfo != null) && (this.m_beiZhaoMuZhe.userInfo.dwPvpLvl >= reward.cfg.dwLevel))
                {
                    reward.state = RewardState.Keling;
                }
                else
                {
                    reward.state = RewardState.Normal;
                }
            }
        }

        public void SetZhaoMuZhe(COMDT_FRIEND_INFO info)
        {
            RecruitData zhaoMuZhe = this.GetZhaoMuZhe(info.stUin.ullUid, info.stUin.dwLogicWorldId);
            if (zhaoMuZhe != null)
            {
                zhaoMuZhe.userInfo = info;
            }
            else
            {
                RecruitData validRecruitData = this.GetValidRecruitData();
                if (validRecruitData != null)
                {
                    validRecruitData.userInfo = info;
                }
            }
        }

        public void SetZhaoMuZheRewardData(COMDT_FRIEND_INFO friendData, ushort rewardID, RewardState state)
        {
            RecruitData zhaoMuZhe = this.GetZhaoMuZhe(friendData.stUin.ullUid, friendData.stUin.dwLogicWorldId);
            if (zhaoMuZhe != null)
            {
                zhaoMuZhe.SetReward(rewardID, state);
            }
            else
            {
                RecruitData validRecruitData = this.GetValidRecruitData();
                if (validRecruitData != null)
                {
                    validRecruitData.userInfo = friendData;
                    validRecruitData.SetReward(rewardID, state);
                }
            }
        }

        public RecruitReward SuperReward
        {
            [CompilerGenerated]
            get
            {
                return this.<SuperReward>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<SuperReward>k__BackingField = value;
            }
        }

        public class RecruitData
        {
            private COMDT_FRIEND_INFO _userInfo;
            public ListView<CFriendRecruit.RecruitReward> RewardList = new ListView<CFriendRecruit.RecruitReward>();
            public COM_RECRUITMENT_TYPE type;

            public RecruitData(COMDT_FRIEND_INFO userInfo, COM_RECRUITMENT_TYPE type)
            {
                this.userInfo = userInfo;
                this.type = type;
            }

            public void Clear()
            {
                this._userInfo = null;
                for (int i = 0; i < this.RewardList.Count; i++)
                {
                    CFriendRecruit.RecruitReward reward = this.RewardList[i];
                    if (reward != null)
                    {
                        reward.Clear();
                    }
                }
                this.RewardList.Clear();
            }

            public CFriendRecruit.RecruitReward GetReward(ushort rewardID)
            {
                return CFriendRecruit.GetReward(rewardID, this.RewardList);
            }

            public bool IsEqual(ulong ullUid, uint dwLogicWorldId)
            {
                return (((this.userInfo != null) && (this.userInfo.stUin.ullUid == ullUid)) && (this.userInfo.stUin.dwLogicWorldId == dwLogicWorldId));
            }

            public bool IsGetAllReward()
            {
                for (int i = 0; i < this.RewardList.Count; i++)
                {
                    CFriendRecruit.RecruitReward reward = this.RewardList[i];
                    if (((reward != null) && (reward.state != CFriendRecruit.RewardState.Getted)) && (reward.state != CFriendRecruit.RewardState.Keling))
                    {
                        return false;
                    }
                }
                return true;
            }

            public void ResetReward()
            {
                for (int i = 0; i < this.RewardList.Count; i++)
                {
                    CFriendRecruit.RecruitReward reward = this.RewardList[i];
                    reward.state = CFriendRecruit.RewardState.Normal;
                }
                Singleton<EventRouter>.GetInstance().BroadCastEvent("FRDataChange");
            }

            public void SetReward(ushort rewardID, CFriendRecruit.RewardState state)
            {
                CFriendRecruit.RecruitReward reward = this.GetReward(rewardID);
                if (reward == null)
                {
                    this.RewardList.Add(new CFriendRecruit.RecruitReward(rewardID, state));
                }
                else
                {
                    reward.state = state;
                }
                Singleton<EventRouter>.GetInstance().BroadCastEvent("FRDataChange");
            }

            public uint dwLogicWorldId
            {
                get
                {
                    return ((this.userInfo != null) ? this.userInfo.stUin.dwLogicWorldId : 0);
                }
            }

            public ulong ullUid
            {
                get
                {
                    return ((this.userInfo != null) ? this.userInfo.stUin.ullUid : ((ulong) 0L));
                }
            }

            public COMDT_FRIEND_INFO userInfo
            {
                get
                {
                    return this._userInfo;
                }
                set
                {
                    this._userInfo = value;
                    Singleton<EventRouter>.GetInstance().BroadCastEvent("FRDataChange");
                }
            }
        }

        public class RecruitReward
        {
            public bool bActive;
            public ResRecruitmentReward cfg;
            public ushort rewardID;
            public CFriendRecruit.RewardState state;

            public RecruitReward(ushort rewardId, CFriendRecruit.RewardState state)
            {
                this.rewardID = rewardId;
                this.state = state;
                this.cfg = Singleton<CFriendContoller>.instance.model.friendRecruit.GetCfgReward(rewardId);
            }

            public void Clear()
            {
                this.cfg = null;
            }
        }

        public enum RewardState
        {
            Normal,
            Keling,
            Getted
        }
    }
}

