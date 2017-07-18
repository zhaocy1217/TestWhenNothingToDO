namespace Assets.Scripts.GameSystem
{
    using CSProtocol;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class RoomInfo
    {
        private ListView<MemberInfo>[] campMemberList = new ListView<MemberInfo>[3];
        public uint dwRoomID;
        public uint dwRoomSeq;
        public int iRoomEntity;
        public RoomAttrib roomAttrib = new RoomAttrib();
        public PlayerUniqueID roomOwner = new PlayerUniqueID();
        public PlayerUniqueID selfInfo = new PlayerUniqueID();
        public uint selfObjID;

        public RoomInfo()
        {
            for (int i = 0; i < 3; i++)
            {
                this.campMemberList[i] = new ListView<MemberInfo>();
            }
        }

        public COM_PLAYERCAMP GetEnemyCamp(COM_PLAYERCAMP inCamp)
        {
            if (inCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
            {
                return COM_PLAYERCAMP.COM_PLAYERCAMP_2;
            }
            if (inCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_2)
            {
                return COM_PLAYERCAMP.COM_PLAYERCAMP_1;
            }
            return COM_PLAYERCAMP.COM_PLAYERCAMP_MID;
        }

        public int GetFreePos(COM_PLAYERCAMP camp, int maxPlayerNum)
        {
            ListView<MemberInfo> view = this[camp];
            if (view != null)
            {
                for (int i = 0; i < (maxPlayerNum / 2); i++)
                {
                    bool flag = false;
                    for (int j = 0; j < view.Count; j++)
                    {
                        MemberInfo info = view[j];
                        if ((info != null) && (info.dwPosOfCamp == i))
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public MemberInfo GetMasterMemberInfo()
        {
            if (this.selfObjID != 0)
            {
                return this.GetMemberInfo(this.selfObjID);
            }
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                return this.GetMemberInfo(masterRoleInfo.playerUllUID);
            }
            return null;
        }

        public MemberInfo GetMemberInfo(uint objID)
        {
            for (int i = 0; i < this.campMemberList.Length; i++)
            {
                ListView<MemberInfo> view = this.campMemberList[i];
                for (int j = 0; j < view.Count; j++)
                {
                    if (view[j].dwObjId == objID)
                    {
                        return view[j];
                    }
                }
            }
            return null;
        }

        public MemberInfo GetMemberInfo(ulong playerUid)
        {
            for (int i = 0; i < this.campMemberList.Length; i++)
            {
                ListView<MemberInfo> view = this.campMemberList[i];
                for (int j = 0; j < view.Count; j++)
                {
                    if (view[j].ullUid == playerUid)
                    {
                        return view[j];
                    }
                }
            }
            return null;
        }

        public MemberInfo GetMemberInfo(COM_PLAYERCAMP camp, int posOfCamp)
        {
            ListView<MemberInfo> view = this[camp];
            if (view != null)
            {
                for (int i = 0; i < view.Count; i++)
                {
                    if (view[i].dwPosOfCamp == posOfCamp)
                    {
                        return view[i];
                    }
                }
            }
            return null;
        }

        public COM_PLAYERCAMP GetSelfCamp()
        {
            for (int i = 0; i < this.campMemberList.Length; i++)
            {
                for (int j = 0; j < this.campMemberList[i].Count; j++)
                {
                    if (this.campMemberList[i][j].ullUid == this.selfInfo.ullUid)
                    {
                        return (COM_PLAYERCAMP) i;
                    }
                }
            }
            return COM_PLAYERCAMP.COM_PLAYERCAMP_MID;
        }

        public bool IsAllConfirmHeroByTeam(COM_PLAYERCAMP camp)
        {
            ListView<MemberInfo> view = this[camp];
            if (view == null)
            {
                return false;
            }
            for (int i = 0; i < view.Count; i++)
            {
                if (!view[i].isPrepare)
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsHaveHeroByID(MemberInfo mInfo, uint heroID)
        {
            if (mInfo.canUseHero != null)
            {
                int length = mInfo.canUseHero.Length;
                for (int i = 0; i < length; i++)
                {
                    if (mInfo.canUseHero[i] == heroID)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsHeroExist(uint heroID)
        {
            return (this.IsHeroExistWithCamp(COM_PLAYERCAMP.COM_PLAYERCAMP_1, heroID) || this.IsHeroExistWithCamp(COM_PLAYERCAMP.COM_PLAYERCAMP_2, heroID));
        }

        public bool IsHeroExistWithCamp(COM_PLAYERCAMP camp, uint heroID)
        {
            bool flag = false;
            ListView<MemberInfo> view = this[camp];
            if (view != null)
            {
                for (int i = 0; i < view.Count; i++)
                {
                    if ((view[i] != null) && (view[i].ChoiceHero == null))
                    {
                    }
                    for (int j = 0; j < view[i].ChoiceHero.Length; j++)
                    {
                        if (view[i].ChoiceHero[j].stBaseInfo.stCommonInfo.dwHeroID == heroID)
                        {
                            flag = true;
                            break;
                        }
                    }
                }
            }
            return flag;
        }

        public static bool IsSameMemeber(MemberInfo member, COM_PLAYERCAMP camp, int pos)
        {
            return (((member != null) && (member.camp == camp)) && (member.dwPosOfCamp == pos));
        }

        public void SortCampMemList(COM_PLAYERCAMP camp)
        {
            ListView<MemberInfo> view = this.campMemberList[(int) camp];
            SortedList<uint, MemberInfo> list = new SortedList<uint, MemberInfo>();
            ListView<MemberInfo>.Enumerator enumerator = view.GetEnumerator();
            while (enumerator.MoveNext())
            {
                MemberInfo current = enumerator.Current;
                uint dwPosOfCamp = current.dwPosOfCamp;
                list.Add(dwPosOfCamp, current);
            }
            this.campMemberList[(int) camp] = new ListView<MemberInfo>(list.Values);
        }

        public int CampListCount
        {
            get
            {
                return this.campMemberList.Length;
            }
        }

        public ListView<MemberInfo> this[COM_PLAYERCAMP camp]
        {
            get
            {
                int index = (int) camp;
                if ((index >= 0) && (index < this.campMemberList.Length))
                {
                    return this.campMemberList[index];
                }
                return null;
            }
        }

        public ListView<MemberInfo> this[int campIndex]
        {
            get
            {
                return this.campMemberList[campIndex];
            }
        }
    }
}

