namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public class CAchieveInfo2
    {
        private static CAchieveInfo2 _masterAchieveInfo;
        private static CAchieveInfo2 _otherAchieveInfo2;
        public CTrophyRewardInfo LastDoneTrophyRewardInfo = null;
        public DictionaryView<uint, CAchieveItem2> m_AchiveItemDic = new DictionaryView<uint, CAchieveItem2>();
        public ListView<CAchieveItem2> m_AchiveItems = new ListView<CAchieveItem2>();
        private int m_logicWorldID;
        private ulong m_playerUUID;
        public ListView<CAchieveItem2> m_Trophies = new ListView<CAchieveItem2>();
        public List<uint> MostLatelyDoneAchievements = new List<uint>();
        public CAchieveItem2[] SelectedTrophies = new CAchieveItem2[3];
        public DictionaryView<uint, CTrophyRewardInfo> TrophyRewardDic = new DictionaryView<uint, CTrophyRewardInfo>();
        public CTrophyRewardInfo[] TrophyRewardInfoArr = new CTrophyRewardInfo[100];
        public uint TrophyRewardInfoArrCnt = 0;
        private static Dictionary<string, uint> worldRanks = new Dictionary<string, uint>();

        private CAchieveInfo2(int logicWorldID, ulong playerUUID)
        {
            this.m_logicWorldID = logicWorldID;
            this.m_playerUUID = playerUUID;
            this.InitLocalData();
        }

        public static void AddWorldRank(int logicWorldID, ulong playerUUID, uint rank)
        {
            string key = string.Format("{0}_{1}", logicWorldID, playerUUID);
            if (worldRanks.ContainsKey(key))
            {
                worldRanks[key] = rank;
            }
            else
            {
                worldRanks.Add(key, rank);
            }
        }

        public void ChangeAchieveState(ref COMDT_ACHIEVEMENT_DATA data)
        {
            if (this.m_AchiveItemDic.ContainsKey(data.dwID))
            {
                this.m_AchiveItemDic[data.dwID].State = (COM_ACHIEVEMENT_STATE) data.bState;
                this.m_AchiveItemDic[data.dwID].DoneTime = data.dwDoneTime;
                this.MostLatelyDoneAchievements.Add(data.dwID);
            }
        }

        private void Classify()
        {
            for (int i = 0; i < this.m_AchiveItems.Count; i++)
            {
                CAchieveItem2 item = this.m_AchiveItems[i];
                uint prevID = item.PrevID;
                if (this.m_AchiveItemDic.ContainsKey(prevID))
                {
                    this.m_AchiveItemDic[prevID].Next = item;
                    item.Prev = this.m_AchiveItemDic[prevID];
                }
                else
                {
                    this.m_Trophies.Add(item);
                }
            }
        }

        public static void Clear()
        {
            if (_masterAchieveInfo != null)
            {
                _masterAchieveInfo.ClearData();
                _masterAchieveInfo = null;
            }
            if (_otherAchieveInfo2 != null)
            {
                _otherAchieveInfo2.ClearData();
                _otherAchieveInfo2 = null;
            }
            worldRanks.Clear();
        }

        public void ClearData()
        {
            this.LastDoneTrophyRewardInfo = null;
            this.m_AchiveItems.Clear();
            this.m_AchiveItemDic.Clear();
            this.m_Trophies.Clear();
            this.MostLatelyDoneAchievements.Clear();
            Array.Clear(this.SelectedTrophies, 0, this.SelectedTrophies.Length);
            this.TrophyRewardDic.Clear();
            Array.Clear(this.TrophyRewardInfoArr, 0, this.TrophyRewardInfoArr.Length);
        }

        public static CAchieveInfo2 GetAchieveInfo(int logicWorldID, ulong playerUUID, bool isMaster = false)
        {
            if (((_masterAchieveInfo != null) && (_masterAchieveInfo.m_logicWorldID == logicWorldID)) && (_masterAchieveInfo.m_playerUUID == playerUUID))
            {
                isMaster = true;
            }
            if (isMaster)
            {
                if (_masterAchieveInfo == null)
                {
                    _masterAchieveInfo = new CAchieveInfo2(logicWorldID, playerUUID);
                }
                return _masterAchieveInfo;
            }
            if (_otherAchieveInfo2 == null)
            {
                _otherAchieveInfo2 = new CAchieveInfo2(logicWorldID, playerUUID);
                return _otherAchieveInfo2;
            }
            if ((_otherAchieveInfo2.m_logicWorldID != logicWorldID) || (_otherAchieveInfo2.m_playerUUID != playerUUID))
            {
                _otherAchieveInfo2 = new CAchieveInfo2(logicWorldID, playerUUID);
            }
            return _otherAchieveInfo2;
        }

        public CTrophyRewardInfo GetFirstTrophyRewardInfoAwardNotGot()
        {
            if (this.LastDoneTrophyRewardInfo != null)
            {
                ListView<CTrophyRewardInfo> trophyRewardInfoWithRewards = this.GetTrophyRewardInfoWithRewards();
                for (int i = 0; i < trophyRewardInfoWithRewards.Count; i++)
                {
                    CTrophyRewardInfo info = trophyRewardInfoWithRewards[i];
                    if (!info.HasGotAward())
                    {
                        return info;
                    }
                }
            }
            return null;
        }

        public static CAchieveInfo2 GetMasterAchieveInfo()
        {
            if (_masterAchieveInfo != null)
            {
                return _masterAchieveInfo;
            }
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo == null)
            {
                DebugHelper.Assert(false, "GetMasterAchieveInfo::Master Role Info Is Null");
                return GetAchieveInfo(0, 0L, true);
            }
            return GetAchieveInfo(masterRoleInfo.logicWorldID, masterRoleInfo.playerUllUID, true);
        }

        public uint GetTotalDonePoints()
        {
            uint num = 0;
            for (int i = 0; i < this.m_Trophies.Count; i++)
            {
                if (this.m_Trophies[i] != null)
                {
                    num += this.m_Trophies[i].GetTotalDonePoints();
                }
            }
            return num;
        }

        public ListView<CAchieveItem2> GetTrophies(enTrophyState state)
        {
            ListView<CAchieveItem2> view = new ListView<CAchieveItem2>();
            switch (state)
            {
                case enTrophyState.All:
                    for (int i = 0; i < this.m_Trophies.Count; i++)
                    {
                        CAchieveItem2 item = this.m_Trophies[i];
                        view.Add(item);
                    }
                    return view;

                case enTrophyState.Finish:
                    for (int j = 0; j < this.m_Trophies.Count; j++)
                    {
                        CAchieveItem2 item3 = this.m_Trophies[j];
                        if (item3.IsFinish())
                        {
                            view.Add(item3);
                        }
                    }
                    return view;

                case enTrophyState.UnFinish:
                    for (int k = 0; k < this.m_Trophies.Count; k++)
                    {
                        CAchieveItem2 item2 = this.m_Trophies[k];
                        if (!item2.IsFinish())
                        {
                            view.Add(item2);
                        }
                    }
                    return view;
            }
            return view;
        }

        public void GetTrophyProgress(ref uint cur, ref uint next)
        {
            cur = this.GetTotalDonePoints();
            if (this.LastDoneTrophyRewardInfo == null)
            {
                if (this.TrophyRewardInfoArrCnt > 0)
                {
                    next = this.TrophyRewardInfoArr[0].MaxPoint;
                }
                else
                {
                    next = 0;
                }
            }
            else
            {
                uint dwTrophyLvl = this.LastDoneTrophyRewardInfo.Cfg.dwTrophyLvl;
                while (true)
                {
                    if (!this.TrophyRewardDic.ContainsKey(dwTrophyLvl))
                    {
                        break;
                    }
                    CTrophyRewardInfo info = this.TrophyRewardDic[dwTrophyLvl];
                    if (cur < info.MaxPoint)
                    {
                        next = info.MaxPoint;
                        return;
                    }
                    dwTrophyLvl++;
                }
                next = this.LastDoneTrophyRewardInfo.MaxPoint;
            }
        }

        public CTrophyRewardInfo GetTrophyRewardInfoByIndex(int index)
        {
            if ((index >= 0) && (index < this.TrophyRewardInfoArrCnt))
            {
                return this.TrophyRewardInfoArr[index];
            }
            if (index >= this.TrophyRewardInfoArrCnt)
            {
                return this.TrophyRewardInfoArr[index - 1];
            }
            return this.TrophyRewardInfoArr[0];
        }

        public CTrophyRewardInfo GetTrophyRewardInfoByPoint(uint point)
        {
            for (int i = ((int) this.TrophyRewardInfoArrCnt) - 1; i >= 0; i--)
            {
                if ((this.TrophyRewardInfoArr[i] != null) && (this.TrophyRewardInfoArr[i].MaxPoint <= point))
                {
                    return this.TrophyRewardInfoArr[i];
                }
            }
            return null;
        }

        public ListView<CTrophyRewardInfo> GetTrophyRewardInfoWithRewards()
        {
            ListView<CTrophyRewardInfo> view = new ListView<CTrophyRewardInfo>();
            for (int i = 0; i < this.TrophyRewardInfoArrCnt; i++)
            {
                if ((this.TrophyRewardInfoArr[i] != null) && this.TrophyRewardInfoArr[i].IsRewardConfiged())
                {
                    view.Add(this.TrophyRewardInfoArr[i]);
                }
            }
            return view;
        }

        public uint GetWorldRank()
        {
            uint num = 0;
            string key = string.Format("{0}_{1}", this.m_logicWorldID, this.m_playerUUID);
            if (worldRanks.ContainsKey(key))
            {
                worldRanks.TryGetValue(key, out num);
            }
            return num;
        }

        public bool HasRewardNotGot()
        {
            for (int i = 0; i < this.TrophyRewardInfoArrCnt; i++)
            {
                if (((this.TrophyRewardInfoArr[i] != null) && (this.TrophyRewardInfoArr[i].State == TrophyState.Finished)) && this.TrophyRewardInfoArr[i].IsRewardConfiged())
                {
                    return true;
                }
            }
            return false;
        }

        private void InitLocalData()
        {
            GameDataMgr.achieveDatabin.Reload();
            Dictionary<long, object>.Enumerator enumerator = GameDataMgr.achieveDatabin.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<long, object> current = enumerator.Current;
                ResAchievement achievement = current.Value as ResAchievement;
                CAchieveItem2 item = new CAchieveItem2(ref achievement);
                this.m_AchiveItems.Add(item);
                if (!this.m_AchiveItemDic.ContainsKey(item.ID))
                {
                    this.m_AchiveItemDic.Add(item.ID, item);
                }
            }
            this.Classify();
            this.InitLocalTrophy();
        }

        private void InitLocalTrophy()
        {
            GameDataMgr.trophyDatabin.Reload();
            Dictionary<long, object>.Enumerator enumerator = GameDataMgr.trophyDatabin.GetEnumerator();
            uint index = 0;
            uint minPoint = 0;
            while (enumerator.MoveNext())
            {
                KeyValuePair<long, object> current = enumerator.Current;
                ResTrophyLvl resTrophy = current.Value as ResTrophyLvl;
                if ((resTrophy != null) && (index < this.TrophyRewardInfoArr.Length))
                {
                    this.TrophyRewardInfoArr[index] = new CTrophyRewardInfo(resTrophy, TrophyState.UnFinish, (int) index, minPoint);
                    minPoint = this.TrophyRewardInfoArr[index].MaxPoint;
                }
                if (!this.TrophyRewardDic.ContainsKey(this.TrophyRewardInfoArr[index].Cfg.dwTrophyLvl))
                {
                    this.TrophyRewardDic.Add(this.TrophyRewardInfoArr[index].Cfg.dwTrophyLvl, this.TrophyRewardInfoArr[index]);
                }
                index++;
            }
            this.TrophyRewardInfoArrCnt = index;
        }

        public void OnAchieveDoneDataChange(COMDT_ACHIEVEMENT_DONE_DATA doneData)
        {
            RES_ACHIEVE_DONE_TYPE dwDoneType = (RES_ACHIEVE_DONE_TYPE) doneData.dwDoneType;
            int num = (this.m_AchiveItems != null) ? this.m_AchiveItems.Count : 0;
            for (int i = 0; i < num; i++)
            {
                if ((this.m_AchiveItems != null) && (this.m_AchiveItems[i].DoneType == dwDoneType))
                {
                    this.m_AchiveItems[i].DoneCnt = doneData.iDoneCnt;
                }
            }
        }

        public void OnServerAchieveInfo(ref COMDT_ACHIEVEMENT_INFO svrAchieveInfo)
        {
            int num;
            uint index = 0;
            int[] numArray = new int[60];
            for (num = 0; num < svrAchieveInfo.dwDoneTypeNum; num++)
            {
                index = svrAchieveInfo.astDoneData[num].dwDoneType;
                numArray[index] = svrAchieveInfo.astDoneData[num].iDoneCnt;
            }
            for (num = 0; num < svrAchieveInfo.dwAchievementNum; num++)
            {
                uint dwID = svrAchieveInfo.astAchievementData[num].dwID;
                uint dwDoneTime = svrAchieveInfo.astAchievementData[num].dwDoneTime;
                COM_ACHIEVEMENT_STATE bState = (COM_ACHIEVEMENT_STATE) svrAchieveInfo.astAchievementData[num].bState;
                if (this.m_AchiveItemDic.ContainsKey(dwID))
                {
                    this.m_AchiveItemDic[dwID].DoneTime = dwDoneTime;
                    this.m_AchiveItemDic[dwID].State = bState;
                    if ((this.m_AchiveItemDic[dwID].DoneType >= RES_ACHIEVE_DONE_TYPE.RES_ACHIEVE_DONE_GET_GOLD) && (this.m_AchiveItemDic[dwID].DoneType < numArray.Length))
                    {
                        this.m_AchiveItemDic[dwID].DoneCnt = numArray[(int) this.m_AchiveItemDic[dwID].DoneType];
                    }
                }
            }
            this.OnServerTrophy(ref svrAchieveInfo.stTrophyLvlInfo);
            this.SetSelectedTrophies(ref svrAchieveInfo.ShowAchievement);
        }

        public void OnServerAchieveInfo(CSDT_SHOWACHIEVE_DETAIL[] selectedTrophyDetailss, uint trophyPoints)
        {
            Dictionary<long, object>.Enumerator enumerator = GameDataMgr.achieveDatabin.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<long, object> current = enumerator.Current;
                ResAchievement achievement = current.Value as ResAchievement;
                CAchieveItem2 item = new CAchieveItem2(ref achievement);
                this.m_AchiveItems.Add(item);
                if (!this.m_AchiveItemDic.ContainsKey(item.ID))
                {
                    this.m_AchiveItemDic.Add(item.ID, item);
                }
            }
            this.OnServerTrophy(trophyPoints);
            this.SetSelectedTrophies(ref selectedTrophyDetailss, true);
        }

        private void OnServerTrophy(uint trophyPoints)
        {
            for (int i = 0; i < this.TrophyRewardInfoArrCnt; i++)
            {
                if (trophyPoints < this.TrophyRewardInfoArr[i].MaxPoint)
                {
                    break;
                }
                this.TrophyRewardInfoArr[i].State = TrophyState.Finished;
                this.LastDoneTrophyRewardInfo = this.TrophyRewardInfoArr[i];
            }
        }

        private void OnServerTrophy(ref COMDT_TROPHY_INFO TrophyRewardsInfo)
        {
            this.TrophyRewardInfoArrCnt = Math.Min(this.TrophyRewardInfoArrCnt, 100);
            if (this.TrophyRewardInfoArrCnt == 0)
            {
                DebugHelper.Assert(false, "成就系统荣耀奖励配置为空!");
            }
            else
            {
                uint totalDonePoints = this.GetTotalDonePoints();
                int index = 0;
                int num3 = 0;
                while (index < this.TrophyRewardInfoArrCnt)
                {
                    if (index < TrophyRewardsInfo.bAlreadyGetRewardNoGapLvl)
                    {
                        this.TrophyRewardInfoArr[index].State = TrophyState.GotRewards;
                    }
                    else if (((index >= TrophyRewardsInfo.bAlreadyGetRewardNoGapLvl) && (index < (TrophyRewardsInfo.bAlreadyGetRewardNoGapLvl + TrophyRewardsInfo.bNum))) && (num3 < TrophyRewardsInfo.bNum))
                    {
                        this.TrophyRewardInfoArr[index].State = (TrophyRewardsInfo.szTrophyStateArray[num3] != 1) ? TrophyState.GotRewards : TrophyState.Finished;
                        num3++;
                    }
                    else if ((totalDonePoints > this.TrophyRewardInfoArr[index].MinPoint) && (totalDonePoints < this.TrophyRewardInfoArr[index].MaxPoint))
                    {
                        this.TrophyRewardInfoArr[index].State = TrophyState.OnGoing;
                    }
                    else if (totalDonePoints >= this.TrophyRewardInfoArr[index].MaxPoint)
                    {
                        this.TrophyRewardInfoArr[index].State = TrophyState.Finished;
                    }
                    else if (totalDonePoints <= this.TrophyRewardInfoArr[index].MinPoint)
                    {
                        this.TrophyRewardInfoArr[index].State = TrophyState.UnFinish;
                    }
                    if (this.TrophyRewardInfoArr[index].IsFinish())
                    {
                        this.LastDoneTrophyRewardInfo = this.TrophyRewardInfoArr[index];
                    }
                    index++;
                }
            }
        }

        private void SetSelectedTrophies(ref uint[] serverSelectedTrophies)
        {
            for (int i = 0; i < this.SelectedTrophies.Length; i++)
            {
                if (this.m_AchiveItemDic.ContainsKey(serverSelectedTrophies[i]))
                {
                    CAchieveItem2 item;
                    this.m_AchiveItemDic.TryGetValue(serverSelectedTrophies[i], out item);
                    if (item != null)
                    {
                        this.SelectedTrophies[i] = item.GetHead();
                    }
                }
                else
                {
                    this.SelectedTrophies[i] = null;
                }
            }
        }

        private void SetSelectedTrophies(ref CSDT_SHOWACHIEVE_DETAIL[] serverSelectedTrophies, bool setFinishRecursively = false)
        {
            for (int i = 0; i < this.SelectedTrophies.Length; i++)
            {
                if (this.m_AchiveItemDic.ContainsKey(serverSelectedTrophies[i].dwAchieveID))
                {
                    CAchieveItem2 headAndSetFinishRecursively;
                    this.m_AchiveItemDic.TryGetValue(serverSelectedTrophies[i].dwAchieveID, out headAndSetFinishRecursively);
                    if (headAndSetFinishRecursively != null)
                    {
                        if (setFinishRecursively)
                        {
                            headAndSetFinishRecursively = headAndSetFinishRecursively.GetHeadAndSetFinishRecursively(serverSelectedTrophies[i].dwDoneTime);
                        }
                        else
                        {
                            headAndSetFinishRecursively = headAndSetFinishRecursively.GetHead();
                        }
                        this.SelectedTrophies[i] = headAndSetFinishRecursively;
                    }
                }
                else
                {
                    this.SelectedTrophies[i] = null;
                }
            }
        }

        public void TrophyLevelUp(uint oldLevel, uint newLevel)
        {
            for (uint i = oldLevel + 1; i <= newLevel; i++)
            {
                if (this.TrophyRewardDic.ContainsKey(i))
                {
                    this.LastDoneTrophyRewardInfo = this.TrophyRewardDic[i];
                    this.LastDoneTrophyRewardInfo.State = TrophyState.Finished;
                }
            }
            uint key = newLevel + 1;
            if (this.TrophyRewardDic.ContainsKey(key))
            {
                this.TrophyRewardDic[key].State = TrophyState.OnGoing;
            }
        }
    }
}

