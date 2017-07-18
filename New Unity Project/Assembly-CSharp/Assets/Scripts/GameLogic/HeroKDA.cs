namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic.DataCenter;
    using CSProtocol;
    using System;
    using System.Collections.Generic;

    public class HeroKDA : KDAStat
    {
        public PoolObjHandle<ActorRoot> actorHero;
        public int CampPos;
        public uint commonSkillID;
        public stEquipInfo[] Equips = new stEquipInfo[6];
        public int HeroId;
        public uint SkinId;
        public int SoulLevel = 1;
        public COMDT_SETTLE_TALENT_INFO[] TalentArr = new COMDT_SETTLE_TALENT_INFO[5];

        protected override uint GetBeShieldProtectedValueToHeroMagic()
        {
            if (((this.actorHero != 0) && (this.actorHero.handle != null)) && ((this.actorHero.handle.BuffHolderComp != null) && (this.actorHero.handle.BuffHolderComp.protectRule != null)))
            {
                return this.actorHero.handle.BuffHolderComp.protectRule.BeProtectedValueToHeroMagic;
            }
            return 0;
        }

        protected override uint GetBeShieldProtectedValueToHeroPhys()
        {
            if (((this.actorHero != 0) && (this.actorHero.handle != null)) && ((this.actorHero.handle.BuffHolderComp != null) && (this.actorHero.handle.BuffHolderComp.protectRule != null)))
            {
                return this.actorHero.handle.BuffHolderComp.protectRule.BeProtectedValueToHeroPhys;
            }
            return 0;
        }

        protected override uint GetBeShieldProtectedValueToHeroReal()
        {
            if (((this.actorHero != 0) && (this.actorHero.handle != null)) && ((this.actorHero.handle.BuffHolderComp != null) && (this.actorHero.handle.BuffHolderComp.protectRule != null)))
            {
                return this.actorHero.handle.BuffHolderComp.protectRule.BeProtectedValueToHeroReal;
            }
            return 0;
        }

        protected override uint GetShieldProtectedValueFormHero()
        {
            if (((this.actorHero != 0) && (this.actorHero.handle != null)) && ((this.actorHero.handle.BuffHolderComp != null) && (this.actorHero.handle.BuffHolderComp.protectRule != null)))
            {
                return this.actorHero.handle.BuffHolderComp.protectRule.ProtectValueFromHero;
            }
            return 0;
        }

        protected override int GetTotalShieldProtectedValue()
        {
            if (((this.actorHero != 0) && (this.actorHero.handle != null)) && ((this.actorHero.handle.BuffHolderComp != null) && (this.actorHero.handle.BuffHolderComp.protectRule != null)))
            {
                return (int) this.actorHero.handle.BuffHolderComp.protectRule.GetProtectTotalValue();
            }
            return 0;
        }

        public void Initialize(PoolObjHandle<ActorRoot> actorRoot, int campPos)
        {
            this.CampPos = campPos;
            this.actorHero = actorRoot;
            this.HeroId = this.actorHero.handle.TheActorMeta.ConfigId;
            ActorServerData actorData = new ActorServerData();
            IGameActorDataProvider actorDataProvider = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.ServerDataProvider);
            if (actorDataProvider.GetActorServerData(ref actorRoot.handle.TheActorMeta, ref actorData))
            {
                this.SkinId = actorData.SkinId;
            }
            this.commonSkillID = 0;
            actorDataProvider.GetActorServerCommonSkillData(ref this.actorHero.handle.TheActorMeta, out this.commonSkillID);
            for (int i = 0; i < 5; i++)
            {
                this.TalentArr[i] = new COMDT_SETTLE_TALENT_INFO();
            }
            base.m_numKill = 0;
            base.m_numAssist = 0;
            base.m_numDead = 0;
            base._totalCoin = 0;
            base.m_iCoinFromKillMonster = 0;
            base.m_iCoinFromKillDragon = 0;
            base.m_iCoinFromKillHero = 0;
            base.m_iCoinFromKillSolider = 0;
            base.m_iCoinFromKillTower = 0;
            base.m_iCoinFromSystem = 0;
        }

        public void OnActorBattleCoinChanged(PoolObjHandle<ActorRoot> actor, int changeValue, bool isIncome, PoolObjHandle<ActorRoot> target)
        {
            if (!isIncome || !(actor == this.actorHero))
            {
                return;
            }
            uint uiCurIndex = 0;
            uint num2 = (uint) (Singleton<FrameSynchr>.instance.LogicFrameTick / ((ulong) 0x3e8L));
            if (num2 >= 60)
            {
                uiCurIndex = ((num2 - 60) / 30) + 1;
            }
            base._totalCoin += changeValue;
            this.SetCoinRecorderInfo(KDAStat.GET_COIN_CHANNEL_TYPE.GET_COIN_CHANNEL_TYPE_ALL, base._totalCoin, uiCurIndex);
            if (target != 0)
            {
                ActorTypeDef actorType = target.handle.TheActorMeta.ActorType;
                switch (actorType)
                {
                    case ActorTypeDef.Actor_Type_Hero:
                        base.m_iCoinFromKillHero += changeValue;
                        this.SetCoinRecorderInfo(KDAStat.GET_COIN_CHANNEL_TYPE.GET_COIN_CHANNEL_TYPE_KILLHERO, base.m_iCoinFromKillHero, uiCurIndex);
                        goto Label_01A8;

                    case ActorTypeDef.Actor_Type_Organ:
                        base.m_iCoinFromKillTower += changeValue;
                        this.SetCoinRecorderInfo(KDAStat.GET_COIN_CHANNEL_TYPE.GET_COIN_CHANNEL_TYPE_KILLTOWER, base.m_iCoinFromKillTower, uiCurIndex);
                        goto Label_01A8;
                }
                if (actorType == ActorTypeDef.Actor_Type_Monster)
                {
                    if (target.handle.ActorControl.GetActorSubType() == 1)
                    {
                        base.m_iCoinFromKillSolider += changeValue;
                        this.SetCoinRecorderInfo(KDAStat.GET_COIN_CHANNEL_TYPE.GET_COIN_CHANNEL_TYPE_KILLSOLIDER, base.m_iCoinFromKillSolider, uiCurIndex);
                    }
                    else
                    {
                        MonsterWrapper wrapper = target.handle.AsMonster();
                        if ((wrapper != null) && (wrapper.cfgInfo != null))
                        {
                            if (((wrapper.cfgInfo.bSoldierType == 7) || (wrapper.cfgInfo.bSoldierType == 9)) || (wrapper.cfgInfo.bSoldierType == 8))
                            {
                                base.m_iCoinFromKillDragon += changeValue;
                                this.SetCoinRecorderInfo(KDAStat.GET_COIN_CHANNEL_TYPE.GET_COIN_CHANNEL_TYPE_KILLDRAGON, base.m_iCoinFromKillDragon, uiCurIndex);
                            }
                            else
                            {
                                base.m_iCoinFromKillMonster += changeValue;
                                this.SetCoinRecorderInfo(KDAStat.GET_COIN_CHANNEL_TYPE.GET_COIN_CHANNEL_TYPE_KILLMONSTER, base.m_iCoinFromKillMonster, uiCurIndex);
                            }
                        }
                    }
                }
            }
            else
            {
                base.m_iCoinFromSystem += changeValue;
            }
        Label_01A8:
            base.uiLastRecordCoinIndex = uiCurIndex;
        }

        public void OnActorBeChosen(ref SkillChooseTargetEventParam prm)
        {
            if ((prm.src != 0) && (prm.src == this.actorHero))
            {
                if ((prm.atker != 0) && prm.src.handle.IsEnemyCamp(prm.atker.handle))
                {
                    base.m_uiBeChosenAsAttackTargetNum++;
                }
                else
                {
                    base.m_uiBeChosenAsHealTargetNum++;
                }
            }
        }

        public void OnActorDamage(ref HurtEventResultInfo prm)
        {
            if (prm.src == this.actorHero)
            {
                if (prm.hurtInfo.hurtType != HurtTypeDef.Therapic)
                {
                    base.m_uiBeAttackedNum++;
                    base.m_hurtTakenByEnemy += prm.hurtTotal;
                    if ((prm.atker != 0) && (prm.atker.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero))
                    {
                        base.m_hurtTakenByHero += prm.hurtTotal;
                    }
                    base.m_BeHurtMax = (base.m_BeHurtMax >= prm.hurtTotal) ? base.m_BeHurtMax : prm.hurtTotal;
                    if (base.m_BeHurtMin == -1)
                    {
                        base.m_BeHurtMin = prm.hurtTotal;
                    }
                    else
                    {
                        base.m_BeHurtMin = (base.m_BeHurtMin <= prm.hurtTotal) ? base.m_BeHurtMin : prm.hurtTotal;
                    }
                }
                else
                {
                    if ((prm.atker != 0) && (prm.atker.handle.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Organ))
                    {
                        base.m_uiHealNum++;
                        base.m_iBeHeal += prm.hurtTotal;
                        base.m_BeHealMax = (base.m_BeHealMax >= prm.hurtTotal) ? base.m_BeHealMax : prm.hurtTotal;
                        if (base.m_BeHealMin == -1)
                        {
                            base.m_BeHealMin = prm.hurtTotal;
                        }
                        else
                        {
                            base.m_BeHealMin = (base.m_BeHealMin <= prm.hurtTotal) ? base.m_BeHealMin : prm.hurtTotal;
                        }
                    }
                    if ((prm.atker != 0) && (prm.atker == this.actorHero))
                    {
                        base.m_iSelfHealNum++;
                        base.m_iSelfHealCount += prm.hurtTotal;
                        base.m_iSelfHealMax = (base.m_iSelfHealMax >= prm.hurtTotal) ? base.m_iSelfHealMax : prm.hurtTotal;
                        if (base.m_iSelfHealMin == -1)
                        {
                            base.m_iSelfHealMin = prm.hurtTotal;
                        }
                        else
                        {
                            base.m_iSelfHealMin = (base.m_iSelfHealMin <= prm.hurtTotal) ? base.m_iSelfHealMin : prm.hurtTotal;
                        }
                    }
                }
            }
            if ((prm.atker != 0) && (prm.atker == this.actorHero))
            {
                if (prm.hurtInfo.hurtType != HurtTypeDef.Therapic)
                {
                    if (prm.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
                    {
                        base.m_hurtToHero += prm.hurtTotal;
                        base.m_uiHurtToHeroNum++;
                        if (prm.hurtInfo.hurtType == HurtTypeDef.PhysHurt)
                        {
                            base.m_iPhysHurtToHero += prm.hurtTotal;
                        }
                        else if (prm.hurtInfo.hurtType == HurtTypeDef.MagicHurt)
                        {
                            base.m_iMagicHurtToHero += prm.hurtTotal;
                        }
                        else if (prm.hurtInfo.hurtType == HurtTypeDef.RealHurt)
                        {
                            base.m_iRealHurtToHero += prm.hurtTotal;
                        }
                    }
                    else if (prm.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ)
                    {
                        base.m_iHurtToOrgan += prm.hurtTotal;
                    }
                    base.m_hurtToEnemy += prm.hurtTotal;
                    base.m_uiHurtToEnemyNum++;
                    if ((prm.atker.handle.SkillControl.CurUseSkill != null) && (prm.atker.handle.SkillControl.CurUseSkill.SlotType == SkillSlotType.SLOT_SKILL_0))
                    {
                        base.m_Skill0HurtToEnemy += prm.hurtTotal;
                    }
                    if (prm.hurtInfo.atkSlot == SkillSlotType.SLOT_SKILL_COUNT)
                    {
                        base.m_iEquipeHurtValue += (uint) prm.hurtTotal;
                    }
                }
                else
                {
                    base.m_heal += prm.hurtTotal;
                }
                this.StatisticSkillInfo(prm);
                this.StatisticActorInfo(prm);
            }
            else if ((prm.atker != 0) && (prm.atker.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster))
            {
                MonsterWrapper actorControl = prm.atker.handle.ActorControl as MonsterWrapper;
                if ((((actorControl != null) && (actorControl.hostActor != 0)) && ((actorControl.hostActor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero) && (actorControl.spawnSkillSlot < SkillSlotType.SLOT_SKILL_COUNT))) && (((actorControl.hostActor.handle.SkillControl != null) && (actorControl.hostActor.handle.SkillControl.stSkillStat != null)) && (actorControl.hostActor.handle.SkillControl.stSkillStat.SkillStatistictInfo != null)))
                {
                    SKILLSTATISTICTINFO skillstatistictinfo1 = actorControl.hostActor.handle.SkillControl.stSkillStat.SkillStatistictInfo[(int) actorControl.spawnSkillSlot];
                    skillstatistictinfo1.iHurtTotal += prm.hurtTotal;
                }
            }
        }

        public void OnActorDead(ref GameDeadEventParam prm)
        {
            if (prm.src == this.actorHero)
            {
                base.recordDead(prm.src, prm.orignalAtker);
            }
            else if (prm.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
            {
                HeroWrapper actorControl = prm.src.handle.ActorControl as HeroWrapper;
                PoolObjHandle<ActorRoot> killer = new PoolObjHandle<ActorRoot>();
                bool flag = false;
                if ((prm.orignalAtker != 0) && (prm.orignalAtker.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero))
                {
                    flag = true;
                    killer = prm.orignalAtker;
                }
                else if (actorControl.IsKilledByHero())
                {
                    flag = true;
                    killer = actorControl.LastHeroAtker;
                }
                if (flag)
                {
                    if (killer == this.actorHero)
                    {
                        base.m_numKill++;
                        if (((this.actorHero.handle.ValueComponent.actorHp * 100) / this.actorHero.handle.ValueComponent.actorHpTotal) <= 10)
                        {
                            base.m_iKillHeroUnderTenPercent++;
                        }
                    }
                    else
                    {
                        base.recordAssist(prm.src, prm.orignalAtker, this.actorHero, killer);
                    }
                }
            }
            else if ((prm.orignalAtker != 0) && (prm.orignalAtker == this.actorHero))
            {
                if (prm.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster)
                {
                    if (prm.src.handle.ActorControl.GetActorSubType() == 2)
                    {
                        base.m_numKillMonster++;
                        MonsterWrapper wrapper2 = prm.src.handle.AsMonster();
                        if ((wrapper2 != null) && (wrapper2.cfgInfo != null))
                        {
                            if (wrapper2.cfgInfo.bSoldierType == 7)
                            {
                                base.m_numKillDragon++;
                                Singleton<EventRouter>.instance.BroadCastEvent(EventID.BATTLE_DRAGON_KILL_CHANGED);
                            }
                            else if (wrapper2.cfgInfo.bSoldierType == 9)
                            {
                                base.m_numKillDragon++;
                                Singleton<EventRouter>.instance.BroadCastEvent(EventID.BATTLE_DRAGON_KILL_CHANGED);
                            }
                            else if (wrapper2.cfgInfo.bSoldierType == 8)
                            {
                                base.m_numKillBaron++;
                                Singleton<EventRouter>.instance.BroadCastEvent(EventID.BATTLE_DRAGON_KILL_CHANGED);
                            }
                            else if (wrapper2.cfgInfo.bSoldierType == 10)
                            {
                                base.m_numKillBlueBa++;
                            }
                            else if (wrapper2.cfgInfo.bSoldierType == 11)
                            {
                                base.m_numKillRedBa++;
                            }
                        }
                    }
                    else if (prm.src.handle.ActorControl.GetActorSubType() == 1)
                    {
                        base.m_numKillSoldier++;
                    }
                    if (prm.src.handle.TheActorMeta.ConfigId != prm.src.handle.TheActorMeta.EnCId)
                    {
                        base.m_numKillFakeMonster++;
                    }
                }
                else if (prm.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ)
                {
                    if ((prm.src.handle.TheStaticData.TheOrganOnlyInfo.OrganType == 1) || (prm.src.handle.TheStaticData.TheOrganOnlyInfo.OrganType == 4))
                    {
                        base.m_numKillOrgan++;
                    }
                    else if (prm.src.handle.TheStaticData.TheOrganOnlyInfo.OrganType == 2)
                    {
                        base.m_numDestroyBase++;
                    }
                }
            }
        }

        public void OnActorDoubleKill(ref DefaultGameEventParam prm)
        {
            if ((prm.logicAtker != 0) && (prm.logicAtker == this.actorHero))
            {
                base.m_DoubleKillNum++;
            }
        }

        public void OnActorHemophagia(ref HemophagiaEventResultInfo prm)
        {
            if (prm.src == this.actorHero)
            {
                base.m_uiHealNum++;
                base.m_iBeHeal += prm.hpChanged;
                base.m_BeHealMax = (base.m_BeHealMax >= prm.hpChanged) ? base.m_BeHealMax : prm.hpChanged;
                if (base.m_BeHealMin == -1)
                {
                    base.m_BeHealMin = prm.hpChanged;
                }
                else
                {
                    base.m_BeHealMin = (base.m_BeHealMin <= prm.hpChanged) ? base.m_BeHealMin : prm.hpChanged;
                }
                base.m_iSelfHealNum++;
                base.m_iSelfHealCount += prm.hpChanged;
                base.m_iSelfHealMax = (base.m_iSelfHealMax >= prm.hpChanged) ? base.m_iSelfHealMax : prm.hpChanged;
                if (base.m_iSelfHealMin == -1)
                {
                    base.m_iSelfHealMin = prm.hpChanged;
                }
                else
                {
                    base.m_iSelfHealMin = (base.m_iSelfHealMin <= prm.hpChanged) ? base.m_iSelfHealMin : prm.hpChanged;
                }
            }
        }

        public void OnActorOdyssey(ref DefaultGameEventParam prm)
        {
            if ((prm.logicAtker != 0) && (prm.logicAtker == this.actorHero))
            {
                base.m_LegendaryNum++;
                if (base.m_LegendaryNum > 1)
                {
                    base.m_LegendaryNum = 1;
                }
            }
        }

        public void OnActorPentaKill(ref DefaultGameEventParam prm)
        {
            if ((prm.logicAtker != 0) && (prm.logicAtker == this.actorHero))
            {
                base.m_PentaKillNum++;
            }
        }

        public void OnActorQuataryKill(ref DefaultGameEventParam prm)
        {
            if ((prm.logicAtker != 0) && (prm.logicAtker == this.actorHero))
            {
                base.m_QuataryKillNum++;
            }
        }

        public void OnActorTripleKill(ref DefaultGameEventParam prm)
        {
            if ((prm.logicAtker != 0) && (prm.logicAtker == this.actorHero))
            {
                base.m_TripleKillNum++;
            }
        }

        public void OnHitTrigger(ref SkillChooseTargetEventParam prm)
        {
            if ((((prm.atker != 0) && (prm.atker == this.actorHero)) && ((prm.atker.handle.SkillControl != null) && (prm.atker.handle.SkillControl.stSkillStat != null))) && ((prm.atker.handle.SkillControl.stSkillStat.SkillStatistictInfo != null) && (prm.atker.handle.SkillControl.CurUseSkillSlot != null)))
            {
                SKILLSTATISTICTINFO skillstatistictinfo = prm.atker.handle.SkillControl.stSkillStat.SkillStatistictInfo[(int) prm.atker.handle.SkillControl.CurUseSkillSlot.SlotType];
                skillstatistictinfo.iHitCount = prm.iTargetCount;
                skillstatistictinfo.iTmpHitAllHurtCountIndex = 0;
                skillstatistictinfo.iTmpHitAllHurtTotal = 0;
                skillstatistictinfo.iHitCountMax = Math.Max(skillstatistictinfo.iHitCountMax, prm.iTargetCount);
                if (skillstatistictinfo.iHitCountMin == -1)
                {
                    skillstatistictinfo.iHitCountMin = prm.iTargetCount;
                }
                else
                {
                    skillstatistictinfo.iHitCountMin = Math.Min(skillstatistictinfo.iHitCountMin, prm.iTargetCount);
                }
            }
        }

        private void SetCoinRecorderInfo(KDAStat.GET_COIN_CHANNEL_TYPE type, int iValue, uint uiCurIndex)
        {
            if (base.m_arrCoinInfos != null)
            {
                if (base.m_arrCoinInfos[(int) type] == null)
                {
                    base.m_arrCoinInfos[(int) type] = new Dictionary<uint, uint>();
                }
                if (base.m_arrCoinInfos[(int) type] != null)
                {
                    if (uiCurIndex != base.uiLastRecordCoinIndex)
                    {
                        base.m_arrCoinInfos[(int) type].Add(uiCurIndex, (uint) iValue);
                    }
                    else
                    {
                        base.m_arrCoinInfos[(int) type].Remove(uiCurIndex);
                        base.m_arrCoinInfos[(int) type].Add(uiCurIndex, (uint) iValue);
                    }
                }
            }
        }

        public void StatisticActorInfo(HurtEventResultInfo prm)
        {
            ActorValueStatistic objValueStatistic = this.actorHero.handle.ValueComponent.ObjValueStatistic;
            if (objValueStatistic != null)
            {
                objValueStatistic.iActorLvl = Math.Max(objValueStatistic.iActorLvl, prm.hurtInfo.attackInfo.iActorLvl);
                objValueStatistic.iActorATT = Math.Max(objValueStatistic.iActorATT, prm.hurtInfo.attackInfo.iActorATT);
                objValueStatistic.iActorINT = Math.Max(objValueStatistic.iActorINT, prm.hurtInfo.attackInfo.iActorINT);
                objValueStatistic.iActorMaxHp = Math.Max(objValueStatistic.iActorMaxHp, prm.hurtInfo.attackInfo.iActorMaxHp);
                objValueStatistic.iActorMinHp = Math.Max(objValueStatistic.iActorMinHp, prm.src.handle.ValueComponent.actorHp);
                objValueStatistic.iDEFStrike = Math.Max(objValueStatistic.iDEFStrike, prm.hurtInfo.attackInfo.iDEFStrike);
                objValueStatistic.iRESStrike = Math.Max(objValueStatistic.iRESStrike, prm.hurtInfo.attackInfo.iRESStrike);
                objValueStatistic.iFinalHurt = Math.Max(objValueStatistic.iFinalHurt, prm.hurtInfo.attackInfo.iFinalHurt);
                objValueStatistic.iCritStrikeRate = Math.Max(objValueStatistic.iCritStrikeRate, prm.hurtInfo.attackInfo.iCritStrikeRate);
                objValueStatistic.iCritStrikeValue = Math.Max(objValueStatistic.iCritStrikeValue, prm.hurtInfo.attackInfo.iCritStrikeValue);
                objValueStatistic.iReduceCritStrikeRate = Math.Max(objValueStatistic.iReduceCritStrikeRate, prm.hurtInfo.attackInfo.iReduceCritStrikeRate);
                objValueStatistic.iReduceCritStrikeValue = Math.Max(objValueStatistic.iReduceCritStrikeValue, prm.hurtInfo.attackInfo.iReduceCritStrikeValue);
                objValueStatistic.iCritStrikeEff = Math.Max(objValueStatistic.iCritStrikeEff, prm.hurtInfo.attackInfo.iCritStrikeEff);
                objValueStatistic.iPhysicsHemophagiaRate = Math.Max(objValueStatistic.iPhysicsHemophagiaRate, prm.hurtInfo.attackInfo.iPhysicsHemophagiaRate);
                objValueStatistic.iMagicHemophagiaRate = Math.Max(objValueStatistic.iMagicHemophagiaRate, prm.hurtInfo.attackInfo.iMagicHemophagiaRate);
                objValueStatistic.iPhysicsHemophagia = Math.Max(objValueStatistic.iPhysicsHemophagia, prm.hurtInfo.attackInfo.iPhysicsHemophagia);
                objValueStatistic.iMagicHemophagia = Math.Max(objValueStatistic.iMagicHemophagia, prm.hurtInfo.attackInfo.iMagicHemophagia);
                objValueStatistic.iHurtOutputRate = Math.Max(objValueStatistic.iHurtOutputRate, prm.hurtInfo.attackInfo.iHurtOutputRate);
            }
        }

        public void StatisticSkillInfo(HurtEventResultInfo prm)
        {
            if ((prm.hurtInfo.atkSlot < SkillSlotType.SLOT_SKILL_COUNT) && (((this.actorHero != 0) && (this.actorHero.handle.SkillControl != null)) && ((this.actorHero.handle.SkillControl.stSkillStat != null) && (this.actorHero.handle.SkillControl.stSkillStat.SkillStatistictInfo != null))))
            {
                SKILLSTATISTICTINFO skillstatistictinfo = this.actorHero.handle.SkillControl.stSkillStat.SkillStatistictInfo[(int) prm.hurtInfo.atkSlot];
                if (prm.hurtInfo.hurtType != HurtTypeDef.Therapic)
                {
                    skillstatistictinfo.iHurtTotal += prm.hurtTotal;
                    if ((prm.src != 0) && (prm.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero))
                    {
                        skillstatistictinfo.iHurtToHeroTotal += prm.hurtTotal;
                        if (!skillstatistictinfo.bIsCurUseSkillHitHero)
                        {
                            skillstatistictinfo.bIsCurUseSkillHitHero = true;
                            skillstatistictinfo.iUseSkillHitHeroTimes++;
                        }
                        skillstatistictinfo.iHitHeroCount++;
                    }
                    skillstatistictinfo.iHurtMax = Math.Max(skillstatistictinfo.iHurtMax, prm.hurtTotal);
                    if (skillstatistictinfo.iHurtMin == -1)
                    {
                        skillstatistictinfo.iHurtMin = prm.hurtTotal;
                    }
                    else
                    {
                        skillstatistictinfo.iHurtMin = Math.Min(skillstatistictinfo.iHurtMin, prm.hurtTotal);
                    }
                    if (skillstatistictinfo.iTmpHitAllHurtCountIndex++ < skillstatistictinfo.iHitCount)
                    {
                        skillstatistictinfo.iTmpHitAllHurtTotal += prm.hurtTotal;
                    }
                    if (skillstatistictinfo.iTmpHitAllHurtCountIndex == skillstatistictinfo.iHitCount)
                    {
                        skillstatistictinfo.iHitAllHurtTotalMax = Math.Max(skillstatistictinfo.iHitAllHurtTotalMax, skillstatistictinfo.iTmpHitAllHurtTotal);
                        if (skillstatistictinfo.iHitAllHurtTotalMin == -1)
                        {
                            skillstatistictinfo.iHitAllHurtTotalMin = skillstatistictinfo.iTmpHitAllHurtTotal;
                        }
                        else
                        {
                            skillstatistictinfo.iHitAllHurtTotalMin = Math.Min(skillstatistictinfo.iHitAllHurtTotalMin, skillstatistictinfo.iTmpHitAllHurtTotal);
                        }
                    }
                    skillstatistictinfo.iadValue = Math.Max(skillstatistictinfo.iadValue, prm.hurtInfo.adValue);
                    skillstatistictinfo.iapValue = Math.Max(skillstatistictinfo.iapValue, prm.hurtInfo.apValue);
                    skillstatistictinfo.ihemoFadeRate = Math.Max(skillstatistictinfo.ihemoFadeRate, prm.hurtInfo.firstHemoFadeRate);
                    skillstatistictinfo.ihpValue = Math.Max(skillstatistictinfo.ihpValue, prm.hurtInfo.hpValue);
                    skillstatistictinfo.ihurtCount = Math.Max(skillstatistictinfo.ihurtCount, prm.hurtInfo.hurtCount);
                    skillstatistictinfo.ihurtValue = Math.Max(skillstatistictinfo.ihurtValue, prm.hurtInfo.hurtValue);
                    skillstatistictinfo.iloseHpValue = Math.Max(skillstatistictinfo.iloseHpValue, prm.hurtInfo.loseHpValue);
                }
            }
        }

        public void unInit()
        {
            this.HeroId = 0;
            this.SoulLevel = 1;
            for (int i = 0; i < 7; i++)
            {
                Dictionary<uint, uint> dictionary = base.m_arrCoinInfos[i];
                if (dictionary != null)
                {
                    dictionary.Clear();
                }
            }
        }
    }
}

