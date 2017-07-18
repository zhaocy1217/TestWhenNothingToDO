namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using CSProtocol;
    using System;
    using System.Collections.Generic;

    public class CBattleDeadStat
    {
        public int enemyKillHeroMaxGap;
        private int[] m_arrHeroEnterCombatTime = new int[10];
        private int m_baojunEnterCombatTime;
        private int m_baronEnterCombatTime;
        private int m_bigDragonEnterCombatTime;
        private int m_deadMonsterNum;
        private List<DeadRecord> m_deadRecordList = new List<DeadRecord>(0x20);
        public uint m_uiFBTime;

        public void AddEventHandler()
        {
            Singleton<GameEventSys>.instance.AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnDeadRecord));
            Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorEnterCombat, new RefAction<DefaultGameEventParam>(this.OnEnterCombat));
        }

        public void Clear()
        {
            this.m_deadMonsterNum = 0;
            this.m_deadRecordList.Clear();
            this.RemoveEventHandler();
            this.enemyKillHeroMaxGap = 0;
            for (int i = 0; i < 10; i++)
            {
                this.m_arrHeroEnterCombatTime[0] = 0;
            }
        }

        public int GetActorAverageFightTime(COM_PLAYERCAMP camp, ActorTypeDef actorType, int cfgId)
        {
            int num = 0;
            int count = this.m_deadRecordList.Count;
            int num3 = 0;
            for (int i = 0; i < count; i++)
            {
                DeadRecord record = this.m_deadRecordList[i];
                if (camp == record.camp)
                {
                    DeadRecord record2 = this.m_deadRecordList[i];
                    if (actorType == record2.actorType)
                    {
                        DeadRecord record3 = this.m_deadRecordList[i];
                        if (cfgId == record3.cfgId)
                        {
                            num++;
                            DeadRecord record4 = this.m_deadRecordList[i];
                            num3 += record4.fightTime;
                        }
                    }
                }
            }
            return ((num != 0) ? (num3 / num) : 0);
        }

        public int GetAllMonsterDeadNum()
        {
            return this.m_deadMonsterNum;
        }

        public int GetBaronDeadCount()
        {
            int num = 0;
            for (int i = 0; i < this.m_deadRecordList.Count; i++)
            {
                DeadRecord record = this.m_deadRecordList[i];
                if (((record.actorType == ActorTypeDef.Actor_Type_Monster) && (record.actorSubType == 2)) && (record.actorSubSoliderType == 8))
                {
                    num++;
                }
            }
            return num;
        }

        public int GetBaronDeadCount(COM_PLAYERCAMP killerCamp)
        {
            int num = 0;
            for (int i = 0; i < this.m_deadRecordList.Count; i++)
            {
                DeadRecord record = this.m_deadRecordList[i];
                if (((killerCamp == record.killerCamp) && (record.actorType == ActorTypeDef.Actor_Type_Monster)) && ((record.actorSubType == 2) && (record.actorSubSoliderType == 8)))
                {
                    num++;
                }
            }
            return num;
        }

        public int GetBaronDeadTime(int index)
        {
            int num = 0;
            for (int i = 0; i < this.m_deadRecordList.Count; i++)
            {
                DeadRecord record = this.m_deadRecordList[i];
                if (((record.actorType == ActorTypeDef.Actor_Type_Monster) && (record.actorSubType == 2)) && (record.actorSubSoliderType == 8))
                {
                    if (num == index)
                    {
                        DeadRecord record2 = this.m_deadRecordList[i];
                        return record2.deadTime;
                    }
                    num++;
                }
            }
            return 0;
        }

        public int GetDeadNum(COM_PLAYERCAMP camp, ActorTypeDef actorType, int subType, int cfgId)
        {
            int num = 0;
            for (int i = 0; i < this.m_deadRecordList.Count; i++)
            {
                DeadRecord record = this.m_deadRecordList[i];
                if (camp == record.camp)
                {
                    DeadRecord record2 = this.m_deadRecordList[i];
                    if (actorType == record2.actorType)
                    {
                        DeadRecord record3 = this.m_deadRecordList[i];
                        if (subType == record3.actorSubType)
                        {
                            DeadRecord record4 = this.m_deadRecordList[i];
                            if (cfgId == record4.cfgId)
                            {
                                num++;
                            }
                        }
                    }
                }
            }
            return num;
        }

        public int GetDeadNumAtTime(COM_PLAYERCAMP camp, ActorTypeDef actorType, int subType, int deadTime)
        {
            int num = 0;
            for (int i = 0; i < this.m_deadRecordList.Count; i++)
            {
                DeadRecord record = this.m_deadRecordList[i];
                if (((camp == record.camp) && (record.actorType == actorType)) && ((record.actorSubType == subType) && (record.deadTime < deadTime)))
                {
                    num++;
                }
            }
            return num;
        }

        public int GetDeadTime(COM_PLAYERCAMP camp, ActorTypeDef actorType, int index)
        {
            int num = 0;
            for (int i = 0; i < this.m_deadRecordList.Count; i++)
            {
                DeadRecord record = this.m_deadRecordList[i];
                if (actorType == record.actorType)
                {
                    DeadRecord record2 = this.m_deadRecordList[i];
                    if (camp == record2.camp)
                    {
                        if (num == index)
                        {
                            DeadRecord record3 = this.m_deadRecordList[i];
                            return record3.deadTime;
                        }
                        num++;
                    }
                }
            }
            return 0;
        }

        public byte GetDestroyTowerCount(COM_PLAYERCAMP killerCamp, int TowerType)
        {
            byte num = 0;
            for (int i = 0; i < this.m_deadRecordList.Count; i++)
            {
                DeadRecord record = this.m_deadRecordList[i];
                if (((killerCamp == record.killerCamp) && (record.actorType == ActorTypeDef.Actor_Type_Organ)) && (record.actorSubType == TowerType))
                {
                    num = (byte) (num + 1);
                }
            }
            return num;
        }

        public int GetDragonDeadTime(int index)
        {
            int num = 0;
            for (int i = 0; i < this.m_deadRecordList.Count; i++)
            {
                DeadRecord record = this.m_deadRecordList[i];
                if (record.actorType == ActorTypeDef.Actor_Type_Monster)
                {
                    DeadRecord record2 = this.m_deadRecordList[i];
                    if (record2.actorSubType == 2)
                    {
                        DeadRecord record3 = this.m_deadRecordList[i];
                        if (record3.actorSubSoliderType != 9)
                        {
                            DeadRecord record4 = this.m_deadRecordList[i];
                            if (record4.actorSubSoliderType != 7)
                            {
                                goto Label_0095;
                            }
                        }
                        if (num == index)
                        {
                            DeadRecord record5 = this.m_deadRecordList[i];
                            return record5.deadTime;
                        }
                        num++;
                    Label_0095:;
                    }
                }
            }
            return 0;
        }

        public int GetEnemyKillHeroMaxGap(COM_PLAYERCAMP camp)
        {
            int num = 0;
            int num2 = 0;
            this.enemyKillHeroMaxGap = 0;
            COM_PLAYERCAMP com_playercamp = COM_PLAYERCAMP.COM_PLAYERCAMP_1;
            if (camp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
            {
                com_playercamp = COM_PLAYERCAMP.COM_PLAYERCAMP_2;
            }
            for (int i = 0; i < this.m_deadRecordList.Count; i++)
            {
                int num4;
                DeadRecord record = this.m_deadRecordList[i];
                if (record.camp == camp)
                {
                    DeadRecord record2 = this.m_deadRecordList[i];
                    if (record2.actorType == ActorTypeDef.Actor_Type_Hero)
                    {
                        DeadRecord record3 = this.m_deadRecordList[i];
                        if (record3.killerActorType == ActorTypeDef.Actor_Type_Hero)
                        {
                            num2++;
                            goto Label_00C8;
                        }
                    }
                }
                DeadRecord record4 = this.m_deadRecordList[i];
                if (record4.camp == com_playercamp)
                {
                    DeadRecord record5 = this.m_deadRecordList[i];
                    if (record5.actorType == ActorTypeDef.Actor_Type_Hero)
                    {
                        DeadRecord record6 = this.m_deadRecordList[i];
                        if (record6.killerActorType == ActorTypeDef.Actor_Type_Hero)
                        {
                            num++;
                        }
                    }
                }
            Label_00C8:
                num4 = num2 - num;
                if (num4 > this.enemyKillHeroMaxGap)
                {
                    this.enemyKillHeroMaxGap = num4;
                }
            }
            return this.enemyKillHeroMaxGap;
        }

        public int GetHeroDeadAtTime(uint playerID, int deadTime)
        {
            int num = 0;
            List<DeadRecord> list = new List<DeadRecord>();
            for (int i = 0; i < this.m_deadRecordList.Count; i++)
            {
                DeadRecord record = this.m_deadRecordList[i];
                if (((record.AttackPlayerID == playerID) && (record.actorType == ActorTypeDef.Actor_Type_Hero)) && (record.deadTime < deadTime))
                {
                    num++;
                }
            }
            return num;
        }

        public int GetKillBlueBaNumAtTime(uint playerID, int deadTime)
        {
            return this.GetKillSpecialMonsterNumAtTime(playerID, deadTime, 10);
        }

        public int GetKillDragonNum()
        {
            int num = 0;
            for (int i = 0; i < this.m_deadRecordList.Count; i++)
            {
                DeadRecord record = this.m_deadRecordList[i];
                if (record.actorType == ActorTypeDef.Actor_Type_Monster)
                {
                    DeadRecord record2 = this.m_deadRecordList[i];
                    if (record2.actorSubType == 2)
                    {
                        DeadRecord record3 = this.m_deadRecordList[i];
                        if (record3.actorSubSoliderType != 9)
                        {
                            DeadRecord record4 = this.m_deadRecordList[i];
                            if (record4.actorSubSoliderType != 7)
                            {
                                goto Label_0078;
                            }
                        }
                        num++;
                    Label_0078:;
                    }
                }
            }
            return num;
        }

        public int GetKillDragonNum(COM_PLAYERCAMP killerCamp)
        {
            int num = 0;
            for (int i = 0; i < this.m_deadRecordList.Count; i++)
            {
                DeadRecord record = this.m_deadRecordList[i];
                if (killerCamp == record.killerCamp)
                {
                    DeadRecord record2 = this.m_deadRecordList[i];
                    if (record2.actorType == ActorTypeDef.Actor_Type_Monster)
                    {
                        DeadRecord record3 = this.m_deadRecordList[i];
                        if (record3.actorSubType == 2)
                        {
                            DeadRecord record4 = this.m_deadRecordList[i];
                            if (record4.actorSubSoliderType != 9)
                            {
                                DeadRecord record5 = this.m_deadRecordList[i];
                                if (record5.actorSubSoliderType != 7)
                                {
                                    goto Label_0093;
                                }
                            }
                            num++;
                        Label_0093:;
                        }
                    }
                }
            }
            return num;
        }

        public int GetKillDragonNumAtTime(uint playerID, int deadTime)
        {
            int num = 0;
            List<DeadRecord> list = new List<DeadRecord>();
            for (int i = 0; i < this.m_deadRecordList.Count; i++)
            {
                DeadRecord record = this.m_deadRecordList[i];
                if (((((record.AttackPlayerID == playerID) && (record.actorType == ActorTypeDef.Actor_Type_Monster)) && (record.actorSubType == 2)) && ((record.actorSubSoliderType == 9) || (record.actorSubSoliderType == 7))) && (record.deadTime < deadTime))
                {
                    num++;
                }
            }
            return num;
        }

        public int GetKillRedBaNumAtTime(uint playerID, int deadTime)
        {
            return this.GetKillSpecialMonsterNumAtTime(playerID, deadTime, 11);
        }

        public int GetKillSpecialMonsterNumAtTime(uint playerID, int deadTime, byte bySoldierType)
        {
            int num = 0;
            for (int i = 0; i < this.m_deadRecordList.Count; i++)
            {
                DeadRecord record = this.m_deadRecordList[i];
                if ((((record.AttackPlayerID == playerID) && (record.actorType == ActorTypeDef.Actor_Type_Monster)) && ((record.actorSubType == 2) && (record.actorSubSoliderType == bySoldierType))) && (record.deadTime < deadTime))
                {
                    num++;
                }
            }
            return num;
        }

        public int GetMonsterDeadAtTime(uint playerID, int deadTime)
        {
            int num = 0;
            List<DeadRecord> list = new List<DeadRecord>();
            for (int i = 0; i < this.m_deadRecordList.Count; i++)
            {
                DeadRecord record = this.m_deadRecordList[i];
                if (((record.AttackPlayerID == playerID) && (record.actorType == ActorTypeDef.Actor_Type_Monster)) && ((record.actorSubType == 2) && (record.deadTime < deadTime)))
                {
                    num++;
                }
            }
            return num;
        }

        public int GetOrganTimeByOrder(int iOrder)
        {
            for (int i = 0; i < this.m_deadRecordList.Count; i++)
            {
                DeadRecord record = this.m_deadRecordList[i];
                if (record.actorType == ActorTypeDef.Actor_Type_Organ)
                {
                    DeadRecord record2 = this.m_deadRecordList[i];
                    if (record2.iOrder == iOrder)
                    {
                        DeadRecord record3 = this.m_deadRecordList[i];
                        return record3.deadTime;
                    }
                }
            }
            return 0;
        }

        public DeadRecord GetRecordAtIndex(COM_PLAYERCAMP camp, ActorTypeDef actorType, byte actorSubType, byte actorSubSoliderType, int index)
        {
            int num = 0;
            for (int i = 0; i < this.m_deadRecordList.Count; i++)
            {
                DeadRecord record = this.m_deadRecordList[i];
                if (record.camp == camp)
                {
                    DeadRecord record2 = this.m_deadRecordList[i];
                    if (record2.actorType == actorType)
                    {
                        DeadRecord record3 = this.m_deadRecordList[i];
                        if (record3.actorSubType == actorSubType)
                        {
                            DeadRecord record4 = this.m_deadRecordList[i];
                            if (record4.actorSubSoliderType == actorSubSoliderType)
                            {
                                if (num == index)
                                {
                                    return this.m_deadRecordList[i];
                                }
                                num++;
                            }
                        }
                    }
                }
            }
            return new DeadRecord();
        }

        public int GetSoldierDeadAtTime(uint playerID, int deadTime)
        {
            int num = 0;
            List<DeadRecord> list = new List<DeadRecord>();
            for (int i = 0; i < this.m_deadRecordList.Count; i++)
            {
                DeadRecord record = this.m_deadRecordList[i];
                if (((record.AttackPlayerID == playerID) && (record.actorType == ActorTypeDef.Actor_Type_Monster)) && ((record.actorSubType == 1) && (record.deadTime < deadTime)))
                {
                    num++;
                }
            }
            return num;
        }

        public int GetTotalNum(COM_PLAYERCAMP camp, ActorTypeDef actorType, byte actorSubType, byte actorSubSoliderType)
        {
            int num = 0;
            for (int i = 0; i < this.m_deadRecordList.Count; i++)
            {
                DeadRecord record = this.m_deadRecordList[i];
                if (record.camp == camp)
                {
                    DeadRecord record2 = this.m_deadRecordList[i];
                    if (record2.actorType == actorType)
                    {
                        DeadRecord record3 = this.m_deadRecordList[i];
                        if (record3.actorSubType == actorSubType)
                        {
                            DeadRecord record4 = this.m_deadRecordList[i];
                            if (record4.actorSubSoliderType == actorSubSoliderType)
                            {
                                num++;
                            }
                        }
                    }
                }
            }
            return num;
        }

        private void OnDeadRecord(ref GameDeadEventParam prm)
        {
            if (!prm.bImmediateRevive)
            {
                PoolObjHandle<ActorRoot> src = prm.src;
                PoolObjHandle<ActorRoot> logicAtker = prm.logicAtker;
                if ((src != 0) && (logicAtker != 0))
                {
                    if (src.handle.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Hero)
                    {
                        if (src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster)
                        {
                            DeadRecord item = new DeadRecord(src.handle.TheActorMeta.ActorCamp, src.handle.TheActorMeta.ActorType, src.handle.TheActorMeta.ConfigId, (int) Singleton<FrameSynchr>.instance.LogicFrameTick, logicAtker.handle.TheActorMeta.ActorCamp, logicAtker.handle.TheActorMeta.PlayerId, logicAtker.handle.TheActorMeta.ActorType);
                            if (src.handle.ActorControl != null)
                            {
                                item.actorSubType = src.handle.ActorControl.GetActorSubType();
                                item.actorSubSoliderType = src.handle.ActorControl.GetActorSubSoliderType();
                                if (item.actorSubType == 2)
                                {
                                    if (item.actorSubSoliderType == 7)
                                    {
                                        item.fightTime = ((int) Singleton<FrameSynchr>.instance.LogicFrameTick) - this.m_baojunEnterCombatTime;
                                    }
                                    else if (item.actorSubSoliderType == 8)
                                    {
                                        item.fightTime = ((int) Singleton<FrameSynchr>.instance.LogicFrameTick) - this.m_baronEnterCombatTime;
                                    }
                                    else if (item.actorSubSoliderType == 9)
                                    {
                                        item.fightTime = ((int) Singleton<FrameSynchr>.instance.LogicFrameTick) - this.m_bigDragonEnterCombatTime;
                                    }
                                }
                            }
                            this.m_deadRecordList.Add(item);
                            this.m_deadMonsterNum++;
                        }
                        else if ((src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ) && (((src.handle.TheStaticData.TheOrganOnlyInfo.OrganType == 1) || (src.handle.TheStaticData.TheOrganOnlyInfo.OrganType == 4)) || (src.handle.TheStaticData.TheOrganOnlyInfo.OrganType == 2)))
                        {
                            DeadRecord record3 = new DeadRecord(src.handle.TheActorMeta.ActorCamp, src.handle.TheActorMeta.ActorType, src.handle.TheActorMeta.ConfigId, (int) Singleton<FrameSynchr>.instance.LogicFrameTick, logicAtker.handle.TheActorMeta.ActorCamp, logicAtker.handle.TheActorMeta.PlayerId, logicAtker.handle.TheActorMeta.ActorType);
                            if (src.handle.ObjLinker != null)
                            {
                                record3.iOrder = src.handle.ObjLinker.BattleOrder;
                                record3.actorSubType = (byte) src.handle.TheStaticData.TheOrganOnlyInfo.OrganType;
                            }
                            this.m_deadRecordList.Add(record3);
                        }
                    }
                    else
                    {
                        DeadRecord record = new DeadRecord(src.handle.TheActorMeta.ActorCamp, src.handle.TheActorMeta.ActorType, src.handle.TheActorMeta.ConfigId, (int) Singleton<FrameSynchr>.instance.LogicFrameTick, logicAtker.handle.TheActorMeta.ActorCamp, logicAtker.handle.TheActorMeta.PlayerId, logicAtker.handle.TheActorMeta.ActorType);
                        List<PoolObjHandle<ActorRoot>> heroActors = Singleton<GameObjMgr>.instance.HeroActors;
                        int count = heroActors.Count;
                        for (int i = 0; i < count; i++)
                        {
                            PoolObjHandle<ActorRoot> handle3 = heroActors[i];
                            if ((handle3.handle.ObjID == prm.src.handle.ObjID) && (i < 10))
                            {
                                record.fightTime = ((int) Singleton<FrameSynchr>.instance.LogicFrameTick) - this.m_arrHeroEnterCombatTime[i];
                                break;
                            }
                        }
                        this.m_deadRecordList.Add(record);
                        if (this.m_uiFBTime == 0)
                        {
                            this.m_uiFBTime = (uint) Singleton<FrameSynchr>.instance.LogicFrameTick;
                        }
                    }
                }
            }
        }

        private void OnEnterCombat(ref DefaultGameEventParam prm)
        {
            if (((prm.src != 0) && (prm.src.handle.ActorControl != null)) && ((prm.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster) && (prm.src.handle.ActorControl.GetActorSubType() == 2)))
            {
                if (prm.src.handle.ActorControl.GetActorSubSoliderType() == 7)
                {
                    this.m_baojunEnterCombatTime = (int) Singleton<FrameSynchr>.instance.LogicFrameTick;
                }
                else if (prm.src.handle.ActorControl.GetActorSubSoliderType() == 8)
                {
                    this.m_baronEnterCombatTime = (int) Singleton<FrameSynchr>.instance.LogicFrameTick;
                }
                else if (prm.src.handle.ActorControl.GetActorSubSoliderType() == 9)
                {
                    this.m_bigDragonEnterCombatTime = (int) Singleton<FrameSynchr>.instance.LogicFrameTick;
                }
            }
            else if ((prm.src != 0) && (prm.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero))
            {
                List<PoolObjHandle<ActorRoot>> heroActors = Singleton<GameObjMgr>.instance.HeroActors;
                int count = heroActors.Count;
                for (int i = 0; i < count; i++)
                {
                    PoolObjHandle<ActorRoot> handle = heroActors[i];
                    if ((handle.handle.ObjID == prm.src.handle.ObjID) && (i < 10))
                    {
                        this.m_arrHeroEnterCombatTime[i] = (int) Singleton<FrameSynchr>.instance.LogicFrameTick;
                        break;
                    }
                }
            }
        }

        public void RemoveEventHandler()
        {
            Singleton<GameEventSys>.instance.RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnDeadRecord));
            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorEnterCombat, new RefAction<DefaultGameEventParam>(this.OnEnterCombat));
        }

        public void StartRecord()
        {
            this.Clear();
            this.AddEventHandler();
        }
    }
}

