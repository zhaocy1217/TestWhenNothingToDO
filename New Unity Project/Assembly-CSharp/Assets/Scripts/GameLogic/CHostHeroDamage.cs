namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.GameKernal;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public class CHostHeroDamage
    {
        [CompilerGenerated]
        private static Comparison<DOUBLE_INT_INFO> <>f__am$cache7;
        [CompilerGenerated]
        private static Comparison<DOUBLE_INT_INFO> <>f__am$cache8;
        private PoolObjHandle<ActorRoot> m_actorHero;
        private PoolObjHandle<ActorRoot> m_actorKiller;
        private uint[] m_arrHurtValue = new uint[4];
        private int m_iDamageIntervalTimeMax = 0x2710;
        private int m_iDamageStatisticsTimeMax = 0x7530;
        private Dictionary<uint, DAMAGE_ACTOR_INFO> m_listDamageActorValue = new Dictionary<uint, DAMAGE_ACTOR_INFO>();
        private ulong m_ulHeroDeadTime;

        private void AddDamageValue(ref PoolObjHandle<ActorRoot> actor, SkillSlotType slotType, int iValue, HurtTypeDef hurtType)
        {
            if (((this.m_listDamageActorValue != null) && (slotType <= SkillSlotType.SLOT_SKILL_VALID)) && (actor != 0))
            {
                DAMAGE_ACTOR_INFO damage_actor_info;
                DOUBLE_INT_INFO[] double_int_infoArray;
                uint objID = actor.handle.ObjID;
                this.DeleteTimeoutDamageValue(objID, 0L);
                if (!this.m_listDamageActorValue.TryGetValue(objID, out damage_actor_info))
                {
                    damage_actor_info = new DAMAGE_ACTOR_INFO();
                    damage_actor_info.actorType = actor.handle.TheActorMeta.ActorType;
                    damage_actor_info.actorName = actor.handle.name;
                    damage_actor_info.ConfigId = actor.handle.TheActorMeta.ConfigId;
                    if (damage_actor_info.actorType == ActorTypeDef.Actor_Type_Monster)
                    {
                        MonsterWrapper wrapper = actor.handle.AsMonster();
                        damage_actor_info.bMonsterType = wrapper.GetActorSubType();
                        damage_actor_info.actorSubType = wrapper.GetActorSubSoliderType();
                    }
                    Player player = Singleton<GamePlayerCenter>.instance.GetPlayer(actor.handle.TheActorMeta.PlayerId);
                    if (player != null)
                    {
                        damage_actor_info.playerName = player.Name;
                    }
                    damage_actor_info.listDamage = new SortedList<ulong, DOUBLE_INT_INFO[]>();
                    this.m_listDamageActorValue.Add(objID, damage_actor_info);
                }
                ulong logicFrameTick = Singleton<FrameSynchr>.instance.LogicFrameTick;
                if (!damage_actor_info.listDamage.TryGetValue(logicFrameTick, out double_int_infoArray))
                {
                    double_int_infoArray = new DOUBLE_INT_INFO[12];
                    damage_actor_info.listDamage.Add(logicFrameTick, double_int_infoArray);
                }
                if (double_int_infoArray[(int) slotType].iValue == 0)
                {
                    double_int_infoArray[(int) slotType].iKey = (int) hurtType;
                }
                double_int_infoArray[(int) slotType].iValue += iValue;
                damage_actor_info.listDamage[logicFrameTick] = double_int_infoArray;
                this.m_listDamageActorValue[objID] = damage_actor_info;
            }
        }

        private void DeleteDamageValue(uint uiObjId)
        {
            if ((this.m_listDamageActorValue != null) && this.m_listDamageActorValue.ContainsKey(uiObjId))
            {
                this.m_listDamageActorValue.Remove(uiObjId);
            }
        }

        private void DeleteTimeoutDamageValue(uint uiObjId, ulong ulTime = 0)
        {
            if (this.m_listDamageActorValue != null)
            {
                DAMAGE_ACTOR_INFO damage_actor_info;
                ulong logicFrameTick = 0L;
                if (ulTime == 0)
                {
                    logicFrameTick = Singleton<FrameSynchr>.instance.LogicFrameTick;
                }
                else
                {
                    logicFrameTick = ulTime;
                }
                if (this.m_listDamageActorValue.TryGetValue(uiObjId, out damage_actor_info))
                {
                    int count = damage_actor_info.listDamage.Count;
                    if (count > 0)
                    {
                        ulong num3 = damage_actor_info.listDamage.Keys[count - 1];
                        if ((logicFrameTick - num3) > this.m_iDamageIntervalTimeMax)
                        {
                            damage_actor_info.listDamage.Clear();
                        }
                        else
                        {
                            IEnumerator<KeyValuePair<ulong, DOUBLE_INT_INFO[]>> enumerator = damage_actor_info.listDamage.GetEnumerator();
                            while (enumerator.MoveNext())
                            {
                                KeyValuePair<ulong, DOUBLE_INT_INFO[]> current = enumerator.Current;
                                ulong key = current.Key;
                                if ((logicFrameTick - key) <= this.m_iDamageStatisticsTimeMax)
                                {
                                    break;
                                }
                                damage_actor_info.listDamage.Remove(key);
                            }
                        }
                        this.m_listDamageActorValue[uiObjId] = damage_actor_info;
                    }
                }
            }
        }

        public bool GetActorDamage(uint uiObjId, ref DOUBLE_INT_INFO[] arrDamageInfo)
        {
            DAMAGE_ACTOR_INFO damage_actor_info;
            if (((this.m_listDamageActorValue == null) || (this.m_listDamageActorValue.Count <= 0)) || ((arrDamageInfo == null) || !this.m_listDamageActorValue.TryGetValue(uiObjId, out damage_actor_info)))
            {
                return false;
            }
            int[] numArray = new int[12];
            int count = damage_actor_info.listDamage.Count;
            if (count <= 0)
            {
                return false;
            }
            if ((this.m_ulHeroDeadTime - damage_actor_info.listDamage.Keys[count - 1]) > this.m_iDamageIntervalTimeMax)
            {
                return false;
            }
            IEnumerator<KeyValuePair<ulong, DOUBLE_INT_INFO[]>> enumerator = damage_actor_info.listDamage.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<ulong, DOUBLE_INT_INFO[]> current = enumerator.Current;
                if ((this.m_ulHeroDeadTime - current.Key) <= this.m_iDamageStatisticsTimeMax)
                {
                    KeyValuePair<ulong, DOUBLE_INT_INFO[]> pair2 = enumerator.Current;
                    DOUBLE_INT_INFO[] double_int_infoArray = pair2.Value;
                    if (double_int_infoArray != null)
                    {
                        for (int j = 0; j <= 11; j++)
                        {
                            numArray[j] += double_int_infoArray[j].iValue;
                        }
                    }
                }
            }
            for (int i = 0; i <= 11; i++)
            {
                arrDamageInfo[i].iValue = numArray[i];
                arrDamageInfo[i].iKey = i;
            }
            if (<>f__am$cache7 == null)
            {
                <>f__am$cache7 = delegate (DOUBLE_INT_INFO p1, DOUBLE_INT_INFO p2) {
                    return p2.iValue.CompareTo(p1.iValue);
                };
            }
            Array.Sort<DOUBLE_INT_INFO>(arrDamageInfo, <>f__am$cache7);
            return true;
        }

        public int GetAllActorsTotalDamageAndTopActorId(ref uint[] arrObjId, int iTopCount, ref uint uiAllTotalDamage, ref uint[] arrDiffTypeHurtValue)
        {
            int num = 0;
            uiAllTotalDamage = 0;
            if ((this.m_listDamageActorValue != null) && (this.m_listDamageActorValue.Count > 0))
            {
                int num2 = 0;
                int index = 0;
                DOUBLE_INT_INFO[] array = new DOUBLE_INT_INFO[this.m_listDamageActorValue.Count];
                Dictionary<uint, DAMAGE_ACTOR_INFO>.Enumerator enumerator = this.m_listDamageActorValue.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    num2 = 0;
                    KeyValuePair<uint, DAMAGE_ACTOR_INFO> current = enumerator.Current;
                    int count = current.Value.listDamage.Count;
                    if (count > 0)
                    {
                        KeyValuePair<uint, DAMAGE_ACTOR_INFO> pair2 = enumerator.Current;
                        if ((this.m_ulHeroDeadTime - pair2.Value.listDamage.Keys[count - 1]) <= this.m_iDamageIntervalTimeMax)
                        {
                            KeyValuePair<uint, DAMAGE_ACTOR_INFO> pair3 = enumerator.Current;
                            IEnumerator<KeyValuePair<ulong, DOUBLE_INT_INFO[]>> enumerator2 = pair3.Value.listDamage.GetEnumerator();
                            while (enumerator2.MoveNext())
                            {
                                KeyValuePair<ulong, DOUBLE_INT_INFO[]> pair4 = enumerator2.Current;
                                if ((this.m_ulHeroDeadTime - pair4.Key) <= this.m_iDamageStatisticsTimeMax)
                                {
                                    KeyValuePair<ulong, DOUBLE_INT_INFO[]> pair5 = enumerator2.Current;
                                    DOUBLE_INT_INFO[] double_int_infoArray = pair5.Value;
                                    if (double_int_infoArray != null)
                                    {
                                        for (int j = 0; j <= 11; j++)
                                        {
                                            num2 += double_int_infoArray[j].iValue;
                                            if ((double_int_infoArray[j].iKey >= 0) && (double_int_infoArray[j].iKey < 4))
                                            {
                                                arrDiffTypeHurtValue[double_int_infoArray[j].iKey] += (uint) double_int_infoArray[j].iValue;
                                            }
                                        }
                                    }
                                }
                            }
                            if (num2 > 0)
                            {
                                uiAllTotalDamage += num2;
                                if (this.m_actorKiller != 0)
                                {
                                    KeyValuePair<uint, DAMAGE_ACTOR_INFO> pair6 = enumerator.Current;
                                    if (this.m_actorKiller.handle.ObjID != pair6.Key)
                                    {
                                        KeyValuePair<uint, DAMAGE_ACTOR_INFO> pair7 = enumerator.Current;
                                        array[index].iKey = pair7.Key;
                                        array[index].iValue = num2;
                                        index++;
                                    }
                                }
                            }
                        }
                    }
                }
                if (<>f__am$cache8 == null)
                {
                    <>f__am$cache8 = delegate (DOUBLE_INT_INFO p1, DOUBLE_INT_INFO p2) {
                        return p2.iValue.CompareTo(p1.iValue);
                    };
                }
                Array.Sort<DOUBLE_INT_INFO>(array, <>f__am$cache8);
                for (int i = 0; (i < iTopCount) && (i < index); i++)
                {
                    if ((this.m_actorKiller == 0) || (array[i].iKey != this.m_actorKiller.handle.ObjID))
                    {
                        if (array[i].iValue <= 0)
                        {
                            return num;
                        }
                        arrObjId[i] = (uint) array[i].iKey;
                        num++;
                    }
                }
            }
            return num;
        }

        public bool GetDamageActorInfo(uint uiObjId, ref string actorName, ref string playerName, ref ActorTypeDef actorType, ref int iConfigId, ref byte actorSubType, ref byte bMonsterType)
        {
            DAMAGE_ACTOR_INFO damage_actor_info;
            if (((this.m_listDamageActorValue != null) && (this.m_listDamageActorValue.Count > 0)) && this.m_listDamageActorValue.TryGetValue(uiObjId, out damage_actor_info))
            {
                actorType = damage_actor_info.actorType;
                actorName = damage_actor_info.actorName;
                playerName = damage_actor_info.playerName;
                iConfigId = damage_actor_info.ConfigId;
                actorSubType = damage_actor_info.actorSubType;
                bMonsterType = damage_actor_info.bMonsterType;
                return true;
            }
            return false;
        }

        public ulong GetHostHeroDeadTime()
        {
            return this.m_ulHeroDeadTime;
        }

        public uint GetHostHeroObjId()
        {
            uint objID = 0;
            if (this.m_actorHero != 0)
            {
                objID = this.m_actorHero.handle.ObjID;
            }
            return objID;
        }

        public bool GetKillerObjId(ref uint uiObjId, ref ActorTypeDef actorType)
        {
            if (this.m_actorKiller != 0)
            {
                uiObjId = this.m_actorKiller.handle.ObjID;
                actorType = this.m_actorKiller.handle.TheActorMeta.ActorType;
                return true;
            }
            return false;
        }

        public int GetSkillSlotHurtType(uint uiObjId, SkillSlotType slotType)
        {
            DAMAGE_ACTOR_INFO damage_actor_info;
            if ((((slotType >= SkillSlotType.SLOT_SKILL_0) && (slotType <= SkillSlotType.SLOT_SKILL_VALID)) && ((this.m_listDamageActorValue != null) && (this.m_listDamageActorValue.Count > 0))) && this.m_listDamageActorValue.TryGetValue(uiObjId, out damage_actor_info))
            {
                IEnumerator<KeyValuePair<ulong, DOUBLE_INT_INFO[]>> enumerator = damage_actor_info.listDamage.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    KeyValuePair<ulong, DOUBLE_INT_INFO[]> current = enumerator.Current;
                    DOUBLE_INT_INFO[] double_int_infoArray = current.Value;
                    if ((double_int_infoArray != null) && (double_int_infoArray[(int) slotType].iValue != 0))
                    {
                        return double_int_infoArray[(int) slotType].iKey;
                    }
                }
            }
            return -1;
        }

        public void Init(PoolObjHandle<ActorRoot> actorRoot)
        {
            this.m_actorHero = actorRoot;
            this.m_iDamageIntervalTimeMax = (int) GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_DEADINFO_ATTACK_INTERVALTIME_MAX);
            this.m_iDamageStatisticsTimeMax = (int) GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_DEADINFO_STATTIME_MAX);
        }

        public void OnActorDamage(ref HurtEventResultInfo prm)
        {
            if (((prm.src == this.m_actorHero) && (prm.hurtInfo.hurtType != HurtTypeDef.Therapic)) && (prm.atker != 0))
            {
                this.AddDamageValue(ref prm.atker, prm.hurtInfo.atkSlot, prm.hurtTotal, prm.hurtInfo.hurtType);
            }
        }

        public void OnActorDead(ref GameDeadEventParam prm)
        {
            if (prm.src == this.m_actorHero)
            {
                if ((prm.orignalAtker != 0) && (prm.orignalAtker.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero))
                {
                    this.m_actorKiller = prm.orignalAtker;
                }
                else if (prm.src.handle.ActorControl.IsKilledByHero())
                {
                    this.m_actorKiller = prm.src.handle.ActorControl.LastHeroAtker;
                }
                else
                {
                    this.m_actorKiller = prm.atker;
                }
                this.m_ulHeroDeadTime = Singleton<FrameSynchr>.instance.LogicFrameTick;
            }
        }

        public void OnActorRevive(ref DefaultGameEventParam prm)
        {
            if (prm.src == this.m_actorHero)
            {
                if (this.m_listDamageActorValue != null)
                {
                    this.m_listDamageActorValue.Clear();
                }
                if (this.m_actorKiller != 0)
                {
                    this.m_actorKiller.Release();
                }
                this.m_ulHeroDeadTime = 0L;
            }
        }

        public void UnInit()
        {
            if (this.m_actorHero != 0)
            {
                this.m_actorHero.Release();
            }
            if (this.m_listDamageActorValue != null)
            {
                this.m_listDamageActorValue.Clear();
            }
        }
    }
}

