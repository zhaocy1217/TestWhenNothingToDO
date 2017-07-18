namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic.GameKernal;
    using System;
    using System.Collections.Generic;

    public class AchievementRecorder
    {
        private bool bFristBlood;

        private KillDetailInfo _create(PoolObjHandle<ActorRoot> killer, PoolObjHandle<ActorRoot> victim, List<uint> assistList, KillDetailInfoType type, KillDetailInfoType HeroMultiKillType, KillDetailInfoType HeroContiKillType, bool bSelfCamp, bool bAllDead, bool bPlayerSelf_KillOrKilled)
        {
            KillDetailInfo info = new KillDetailInfo();
            info.Killer = killer;
            info.Victim = victim;
            info.assistList = assistList;
            info.Type = type;
            info.HeroMultiKillType = HeroMultiKillType;
            info.HeroContiKillType = HeroContiKillType;
            info.bSelfCamp = bSelfCamp;
            info.bAllDead = bAllDead;
            info.bPlayerSelf_KillOrKilled = bPlayerSelf_KillOrKilled;
            return info;
        }

        public void Clear()
        {
            this.bFristBlood = false;
            Singleton<GameEventSys>.instance.RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnActorDeath));
        }

        private bool IsAllDead(ref PoolObjHandle<ActorRoot> actorHandle)
        {
            if (actorHandle == 0)
            {
                return false;
            }
            List<Player> allCampPlayers = Singleton<GamePlayerCenter>.instance.GetAllCampPlayers(actorHandle.handle.TheActorMeta.ActorCamp);
            if (allCampPlayers.Count <= 1)
            {
                return false;
            }
            if (allCampPlayers != null)
            {
                for (int i = 0; i < allCampPlayers.Count; i++)
                {
                    Player player = allCampPlayers[i];
                    if ((player != null) && !player.IsAllHeroesDead())
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void OnActorDeath(ref GameDeadEventParam prm)
        {
            if (((prm.src != 0) && (prm.orignalAtker != 0)) && !prm.bImmediateRevive)
            {
                bool flag = false;
                switch (prm.src.handle.ActorControl.GetActorSubSoliderType())
                {
                    case 8:
                    case 9:
                        flag = true;
                        break;
                }
                if (((prm.src.handle.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Monster) || (prm.src.handle.TheActorMeta.ConfigId == Singleton<BattleLogic>.instance.DragonId)) || flag)
                {
                    KillDetailInfo info = this.OnActorDeathd(ref prm);
                    if (info != null)
                    {
                        Singleton<EventRouter>.instance.BroadCastEvent<KillDetailInfo>(EventID.AchievementRecorderEvent, info);
                    }
                }
            }
        }

        private KillDetailInfo OnActorDeathd(ref GameDeadEventParam param)
        {
            KillDetailInfo info = null;
            KillDetailInfoType type = KillDetailInfoType.Info_Type_None;
            KillDetailInfoType heroMultiKillType = KillDetailInfoType.Info_Type_None;
            KillDetailInfoType heroContiKillType = KillDetailInfoType.Info_Type_None;
            PoolObjHandle<ActorRoot> victim = new PoolObjHandle<ActorRoot>();
            PoolObjHandle<ActorRoot> killer = new PoolObjHandle<ActorRoot>();
            List<uint> assistList = new List<uint>();
            DefaultGameEventParam prm = new DefaultGameEventParam(param.src, param.atker, ref param.orignalAtker, ref param.logicAtker);
            HeroWrapper actorControl = null;
            HeroWrapper wrapper2 = null;
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            bool flag4 = false;
            bool flag5 = false;
            Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
            DebugHelper.Assert(hostPlayer != null, "Fatal error in OnActorDeadthd, HostPlayer is null!");
            if (hostPlayer != null)
            {
                DebugHelper.Assert((bool) hostPlayer.Captain, "Fatal error in OnActorDeadthd, Captain is null!");
            }
            bool bSelfCamp = hostPlayer.PlayerCamp == prm.orignalAtker.handle.TheActorMeta.ActorCamp;
            bool flag7 = false;
            uint objID = hostPlayer.Captain.handle.ObjID;
            flag7 = (objID == prm.src.handle.ObjID) || (objID == prm.orignalAtker.handle.ObjID);
            flag = prm.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero;
            flag2 = prm.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ;
            byte actorSubSoliderType = prm.src.handle.ActorControl.GetActorSubSoliderType();
            bool flag8 = false;
            bool flag9 = false;
            bool flag10 = false;
            if ((prm.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster) && (actorSubSoliderType == 8))
            {
                flag8 = true;
            }
            else if ((prm.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster) && (actorSubSoliderType == 9))
            {
                flag9 = true;
            }
            else if ((prm.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster) && (actorSubSoliderType == 7))
            {
                flag10 = true;
            }
            victim = prm.src;
            killer = prm.orignalAtker;
            if (flag)
            {
                actorControl = prm.src.handle.ActorControl as HeroWrapper;
                wrapper2 = prm.orignalAtker.handle.ActorControl as HeroWrapper;
                if (prm.orignalAtker.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
                {
                    if (prm.orignalAtker.handle.ObjID == objID)
                    {
                        flag7 = true;
                    }
                    flag3 = true;
                    bSelfCamp = hostPlayer.PlayerCamp == prm.orignalAtker.handle.TheActorMeta.ActorCamp;
                }
                else if (actorControl.IsKilledByHero())
                {
                    flag3 = true;
                    killer = actorControl.LastHeroAtker;
                    wrapper2 = actorControl.LastHeroAtker.handle.ActorControl as HeroWrapper;
                    if (killer.handle.ObjID == objID)
                    {
                        flag7 = true;
                    }
                    bSelfCamp = hostPlayer.PlayerCamp == killer.handle.TheActorMeta.ActorCamp;
                }
                else if (prm.orignalAtker.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ)
                {
                    flag4 = true;
                    flag3 = false;
                    bSelfCamp = hostPlayer.PlayerCamp == prm.orignalAtker.handle.TheActorMeta.ActorCamp;
                }
                else if (prm.orignalAtker.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster)
                {
                    flag5 = true;
                    flag3 = false;
                    bSelfCamp = hostPlayer.PlayerCamp == prm.orignalAtker.handle.TheActorMeta.ActorCamp;
                }
                if (flag && flag3)
                {
                    wrapper2.ContiDeadNum = 0;
                    wrapper2.ContiKillNum++;
                    if (wrapper2.IsInMultiKill())
                    {
                        wrapper2.MultiKillNum++;
                    }
                    else
                    {
                        wrapper2.MultiKillNum = 1;
                    }
                    wrapper2.UpdateLastKillTime();
                }
            }
            if (flag && flag3)
            {
                if ((victim != 0) && (victim.handle.ActorControl != null))
                {
                    List<KeyValuePair<uint, ulong>>.Enumerator enumerator = victim.handle.ActorControl.hurtSelfActorList.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        KeyValuePair<uint, ulong> current = enumerator.Current;
                        if (current.Key != killer.handle.ObjID)
                        {
                            KeyValuePair<uint, ulong> pair2 = enumerator.Current;
                            assistList.Add(pair2.Key);
                        }
                    }
                }
                if ((killer != 0) && (killer.handle.ActorControl != null))
                {
                    List<KeyValuePair<uint, ulong>>.Enumerator enumerator2 = killer.handle.ActorControl.helpSelfActorList.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        KeyValuePair<uint, ulong> pair3 = enumerator2.Current;
                        if (pair3.Key != killer.handle.ObjID)
                        {
                            KeyValuePair<uint, ulong> pair4 = enumerator2.Current;
                            assistList.Add(pair4.Key);
                        }
                    }
                }
                for (int i = 0; i < (assistList.Count - 1); i++)
                {
                    for (int j = i + 1; j < assistList.Count; j++)
                    {
                        if (assistList[i] == assistList[j])
                        {
                            assistList.RemoveAt(j);
                            j--;
                        }
                    }
                }
                bool flag11 = false;
                if (wrapper2.MultiKillNum == 2)
                {
                    flag11 = true;
                    heroMultiKillType = KillDetailInfoType.Info_Type_DoubleKill;
                    Singleton<GameEventSys>.GetInstance().SendEvent<DefaultGameEventParam>(GameEventDef.Event_DoubleKill, ref prm);
                }
                else if (wrapper2.MultiKillNum == 3)
                {
                    flag11 = true;
                    heroMultiKillType = KillDetailInfoType.Info_Type_TripleKill;
                    Singleton<GameEventSys>.GetInstance().SendEvent<DefaultGameEventParam>(GameEventDef.Event_TripleKill, ref prm);
                }
                else if (wrapper2.MultiKillNum == 4)
                {
                    flag11 = true;
                    heroMultiKillType = KillDetailInfoType.Info_Type_QuataryKill;
                    Singleton<GameEventSys>.GetInstance().SendEvent<DefaultGameEventParam>(GameEventDef.Event_QuataryKill, ref prm);
                }
                else if (wrapper2.MultiKillNum >= 5)
                {
                    flag11 = true;
                    heroMultiKillType = KillDetailInfoType.Info_Type_PentaKill;
                    Singleton<GameEventSys>.GetInstance().SendEvent<DefaultGameEventParam>(GameEventDef.Event_PentaKill, ref prm);
                }
                if (flag11 && (info == null))
                {
                    info = this._create(killer, victim, assistList, type, heroMultiKillType, type, bSelfCamp, false, flag7);
                }
            }
            if ((flag && flag3) && (actorControl.ContiKillNum >= 3))
            {
                if (actorControl.ContiKillNum >= 7)
                {
                    Singleton<GameEventSys>.GetInstance().SendEvent<DefaultGameEventParam>(GameEventDef.Event_OdysseyBeStopped, ref prm);
                }
                if (info == null)
                {
                    info = this._create(killer, victim, assistList, KillDetailInfoType.Info_Type_StopMultiKill, heroMultiKillType, type, bSelfCamp, false, flag7);
                }
            }
            if ((flag && flag3) && !this.bFristBlood)
            {
                this.bFristBlood = true;
                return this._create(killer, victim, assistList, KillDetailInfoType.Info_Type_First_Kill, heroMultiKillType, type, bSelfCamp, false, flag7);
            }
            if (flag2 && ((prm.src.handle.TheStaticData.TheOrganOnlyInfo.OrganType == 1) || (prm.src.handle.TheStaticData.TheOrganOnlyInfo.OrganType == 4)))
            {
                KillDetailInfo info2 = this._create(killer, victim, assistList, KillDetailInfoType.Info_Type_DestroyTower, type, type, bSelfCamp, false, flag7);
                if (prm.src.handle.TheStaticData.TheOrganOnlyInfo.DeadEnemySoldier > 0)
                {
                    Singleton<EventRouter>.instance.BroadCastEvent<KillDetailInfo>(EventID.AchievementRecorderEvent, info2);
                    return this._create(killer, victim, assistList, KillDetailInfoType.Info_Type_Soldier_Boosted, type, type, bSelfCamp, false, flag7);
                }
                return info2;
            }
            if (flag && flag3)
            {
                bool flag12 = false;
                if (wrapper2.ContiKillNum == 3)
                {
                    flag12 = true;
                    heroContiKillType = KillDetailInfoType.Info_Type_MonsterKill;
                }
                else if (wrapper2.ContiKillNum == 4)
                {
                    flag12 = true;
                    heroContiKillType = KillDetailInfoType.Info_Type_DominateBattle;
                }
                else if (wrapper2.ContiKillNum == 5)
                {
                    flag12 = true;
                    heroContiKillType = KillDetailInfoType.Info_Type_Legendary;
                }
                else if (wrapper2.ContiKillNum == 6)
                {
                    flag12 = true;
                    heroContiKillType = KillDetailInfoType.Info_Type_TotalAnnihilat;
                }
                else if (wrapper2.ContiKillNum >= 7)
                {
                    flag12 = true;
                    heroContiKillType = KillDetailInfoType.Info_Type_Odyssey;
                    Singleton<GameEventSys>.GetInstance().SendEvent<DefaultGameEventParam>(GameEventDef.Event_Odyssey, ref prm);
                }
                if (flag12 && (info == null))
                {
                    info = this._create(killer, victim, assistList, type, type, heroContiKillType, bSelfCamp, false, flag7);
                }
            }
            if (flag && this.IsAllDead(ref prm.src))
            {
                KillDetailInfo info3 = this._create(killer, victim, assistList, type, type, type, bSelfCamp, true, flag7);
                Singleton<EventRouter>.instance.BroadCastEvent<KillDetailInfo>(EventID.AchievementRecorderEvent, info3);
            }
            if (info != null)
            {
                return info;
            }
            if (flag && ((flag3 || flag4) || flag5))
            {
                return this._create(killer, victim, assistList, KillDetailInfoType.Info_Type_Kill, type, type, bSelfCamp, false, flag7);
            }
            if (flag10)
            {
                return this._create(killer, victim, assistList, KillDetailInfoType.Info_Type_Kill_3V3_Dragon, type, type, bSelfCamp, false, flag7);
            }
            if (flag9)
            {
                return this._create(killer, victim, assistList, KillDetailInfoType.Info_Type_Kill_5V5_SmallDragon, type, type, bSelfCamp, false, flag7);
            }
            if (flag8)
            {
                return this._create(killer, victim, assistList, KillDetailInfoType.Info_Type_Kill_5V5_BigDragon, type, type, bSelfCamp, false, flag7);
            }
            return null;
        }

        public void StartRecord()
        {
            this.Clear();
            Singleton<GameEventSys>.instance.AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnActorDeath));
        }
    }
}

