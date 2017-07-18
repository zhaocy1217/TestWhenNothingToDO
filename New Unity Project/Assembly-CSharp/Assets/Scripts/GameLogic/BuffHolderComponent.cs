namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public class BuffHolderComponent : LogicComponent
    {
        public bool bRemoveList = true;
        public BuffChangeSkillRule changeSkillRule;
        public BuffClearRule clearRule;
        private List<BuffSkill> delBuffList = new List<BuffSkill>(3);
        public BufferLogicEffect logicEffect;
        public BufferMarkRule markRule;
        public BuffOverlayRule overlayRule;
        public BuffProtectRule protectRule;
        public List<BuffSkill> SpawnedBuffList = new List<BuffSkill>();

        public void ActionRemoveBuff(BuffSkill inBuff)
        {
            if (this.SpawnedBuffList.Remove(inBuff))
            {
                PoolObjHandle<BuffSkill> handle = new PoolObjHandle<BuffSkill>(inBuff);
                this.protectRule.RemoveBuff(ref handle);
                this.logicEffect.RemoveBuff(ref handle);
                BuffChangeEventParam param = new BuffChangeEventParam(false, base.actorPtr, inBuff);
                Singleton<GameSkillEventSys>.GetInstance().SendEvent<BuffChangeEventParam>(GameSkillEventDef.AllEvent_BuffChange, base.actorPtr, ref param, GameSkillEventChannel.Channel_AllActor);
                inBuff.Release();
            }
        }

        public void AddBuff(BuffSkill inBuff)
        {
            this.SpawnedBuffList.Add(inBuff);
            this.protectRule.AddBuff(inBuff);
            this.logicEffect.AddBuff(inBuff);
            BuffChangeEventParam param = new BuffChangeEventParam(true, base.actorPtr, inBuff);
            Singleton<GameSkillEventSys>.GetInstance().SendEvent<BuffChangeEventParam>(GameSkillEventDef.AllEvent_BuffChange, base.actorPtr, ref param, GameSkillEventChannel.Channel_HostCtrlActor);
            if (((inBuff.cfgData != null) && (inBuff.cfgData.bIsAssistEffect == 1)) && ((inBuff.skillContext.Originator != 0) && (base.actor.ValueComponent.actorHp > 0)))
            {
                if (base.actor.TheActorMeta.ActorCamp == inBuff.skillContext.Originator.handle.TheActorMeta.ActorCamp)
                {
                    base.actor.ActorControl.AddHelpSelfActor(inBuff.skillContext.Originator);
                }
                else
                {
                    base.actor.ActorControl.AddHurtSelfActor(inBuff.skillContext.Originator);
                }
            }
        }

        public bool CheckHpConditional(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> target, int iParam)
        {
            if ((src == 0) || (target == 0))
            {
                return false;
            }
            ulong propertyHpRate = TargetProperty.GetPropertyHpRate((ActorRoot) src, RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP);
            ulong inSecond = TargetProperty.GetPropertyHpRate((ActorRoot) target, RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP);
            return SmartCompare.Compare<ulong>(propertyHpRate, inSecond, iParam);
        }

        private bool CheckTargetFromEnemy(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> target)
        {
            bool flag = false;
            if ((src != 0) && (target != 0))
            {
                long num2;
                long num3;
                if (src.handle.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Hero)
                {
                    return flag;
                }
                if (target.handle.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Monster)
                {
                    return (src.handle.TheActorMeta.ActorCamp != target.handle.TheActorMeta.ActorCamp);
                }
                MonsterWrapper wrapper = target.handle.AsMonster();
                if (wrapper == null)
                {
                    return flag;
                }
                RES_MONSTER_TYPE bMonsterType = (RES_MONSTER_TYPE) wrapper.cfgInfo.bMonsterType;
                if (bMonsterType == RES_MONSTER_TYPE.RES_MONSTER_TYPE_SOLDIERLINE)
                {
                    if (src.handle.TheActorMeta.ActorCamp != target.handle.TheActorMeta.ActorCamp)
                    {
                        flag = true;
                    }
                    return flag;
                }
                if (bMonsterType != RES_MONSTER_TYPE.RES_MONSTER_TYPE_JUNGLE)
                {
                    return flag;
                }
                switch (wrapper.GetActorSubSoliderType())
                {
                    case 8:
                    case 9:
                    case 7:
                    case 14:
                        return flag;

                    default:
                    {
                        num2 = 0L;
                        num3 = 0L;
                        VInt3 bornPos = target.handle.BornPos;
                        List<PoolObjHandle<ActorRoot>> organActors = Singleton<GameObjMgr>.instance.OrganActors;
                        int num5 = 0;
                        for (int i = 0; i < organActors.Count; i++)
                        {
                            PoolObjHandle<ActorRoot> handle = organActors[i];
                            if (handle.handle.TheStaticData.TheOrganOnlyInfo.OrganType == 2)
                            {
                                VInt3 location = handle.handle.location;
                                if (handle.handle.TheActorMeta.ActorCamp == src.handle.TheActorMeta.ActorCamp)
                                {
                                    VInt3 num8 = bornPos - location;
                                    num2 = num8.sqrMagnitudeLong2D;
                                }
                                else
                                {
                                    VInt3 num9 = bornPos - location;
                                    num3 = num9.sqrMagnitudeLong2D;
                                }
                                num5++;
                                if (num5 >= 2)
                                {
                                    break;
                                }
                            }
                        }
                        break;
                    }
                }
                if (num2 > num3)
                {
                    flag = true;
                }
            }
            return flag;
        }

        public bool CheckTargetSubType(int typeMask, int typeSubMask, PoolObjHandle<ActorRoot> target)
        {
            if (typeMask == 0)
            {
                return true;
            }
            if (target != 0)
            {
                int actorType = (int) target.handle.TheActorMeta.ActorType;
                if ((typeMask & (((int) 1) << actorType)) > 0)
                {
                    if (actorType != 1)
                    {
                        return true;
                    }
                    if (typeSubMask == 0)
                    {
                        return true;
                    }
                    if (target.handle.ActorControl.GetActorSubType() == typeSubMask)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool CheckTriggerCondtion(int conditionalType, int iParam, PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> target)
        {
            RES_EXTRACT_EFFECT_CONDITION_TYPE res_extract_effect_condition_type = (RES_EXTRACT_EFFECT_CONDITION_TYPE) conditionalType;
            if (res_extract_effect_condition_type != RES_EXTRACT_EFFECT_CONDITION_TYPE.RES_EXTRACT_EFFECT_CONDITIONAL_NONE)
            {
                if (res_extract_effect_condition_type == RES_EXTRACT_EFFECT_CONDITION_TYPE.RES_EXTRACT_EFFECT_CONDITIONAL_HPRATE)
                {
                    return this.CheckHpConditional(src, target, iParam);
                }
                return true;
            }
            return true;
        }

        public void ClearBuff()
        {
            BuffSkill skill;
            this.bRemoveList = false;
            for (int i = 0; i < this.SpawnedBuffList.Count; i++)
            {
                skill = this.SpawnedBuffList[i];
                if (skill != null)
                {
                    skill.Stop();
                }
            }
            if (this.protectRule != null)
            {
                this.protectRule.ClearBuff();
            }
            if (this.logicEffect != null)
            {
                this.logicEffect.ClearBuff();
            }
            for (int j = 0; j < this.SpawnedBuffList.Count; j++)
            {
                skill = this.SpawnedBuffList[j];
                if (skill != null)
                {
                    skill.Release();
                }
            }
            this.SpawnedBuffList.Clear();
            this.delBuffList.Clear();
            this.bRemoveList = true;
        }

        public void ClearEffectTypeBuff(int _typeMask)
        {
            if (this.SpawnedBuffList.Count != 0)
            {
                this.delBuffList = this.SpawnedBuffList;
                for (int i = 0; i < this.delBuffList.Count; i++)
                {
                    BuffSkill skill = this.delBuffList[i];
                    if ((_typeMask & (((int) 1) << skill.cfgData.bEffectType)) > 0)
                    {
                        skill.Stop();
                    }
                }
                if (this.markRule != null)
                {
                    this.markRule.ClearBufferMark(_typeMask);
                }
            }
        }

        public override void Deactive()
        {
            this.ClearBuff();
            base.Deactive();
        }

        private int DealDamageContionType(ref HurtDataInfo _hurt, int _hurtValue)
        {
            if (_hurt.atker != 0)
            {
                if (_hurt.iConditionType == 1)
                {
                    int nAddHp = (_hurtValue * _hurt.iConditionParam) / 0x2710;
                    _hurt.atker.handle.ActorControl.ReviveHp(nAddHp);
                    return _hurtValue;
                }
                if (((_hurt.iConditionType == 2) && (_hurt.target != 0)) && (_hurt.atker != 0))
                {
                    VInt3 num2 = _hurt.atker.handle.location - _hurt.target.handle.location;
                    int num4 = (int) (((num2.magnitude2D * _hurtValue) * _hurt.iConditionParam) / 0x989680L);
                    _hurtValue += num4;
                }
            }
            return _hurtValue;
        }

        public BuffSkill FindBuff(int inSkillCombineId)
        {
            if (this.SpawnedBuffList != null)
            {
                for (int i = 0; i < this.SpawnedBuffList.Count; i++)
                {
                    BuffSkill skill = this.SpawnedBuffList[i];
                    if ((skill != null) && (skill.SkillID == inSkillCombineId))
                    {
                        return skill;
                    }
                }
            }
            return null;
        }

        public int FindBuffCount(int inSkillCombineId)
        {
            int num = 0;
            for (int i = 0; i < this.SpawnedBuffList.Count; i++)
            {
                BuffSkill skill = this.SpawnedBuffList[i];
                if ((skill != null) && (skill.SkillID == inSkillCombineId))
                {
                    num++;
                }
            }
            return num;
        }

        public int GetCoinAddRate(PoolObjHandle<ActorRoot> _target)
        {
            int num = 0;
            BuffSkill skill = null;
            ResDT_SkillFunc outSkillFunc = null;
            if (_target != 0)
            {
                for (int i = 0; i < this.SpawnedBuffList.Count; i++)
                {
                    skill = this.SpawnedBuffList[i];
                    if ((skill != null) && skill.FindSkillFunc(0x47, out outSkillFunc))
                    {
                        int typeMask = skill.GetSkillFuncParam(outSkillFunc, 0, false);
                        int typeSubMask = skill.GetSkillFuncParam(outSkillFunc, 1, false);
                        int num5 = skill.GetSkillFuncParam(outSkillFunc, 2, false);
                        int num6 = skill.GetSkillFuncParam(outSkillFunc, 3, false);
                        if (this.CheckTargetSubType(typeMask, typeSubMask, _target))
                        {
                            bool flag = true;
                            if (num6 > 0)
                            {
                                flag = this.CheckTargetFromEnemy(base.actorPtr, _target);
                            }
                            if (flag)
                            {
                                num += num5;
                            }
                        }
                    }
                }
            }
            return num;
        }

        public int GetExtraHurtOutputRate(PoolObjHandle<ActorRoot> _attack)
        {
            int num = 0;
            BuffSkill skill = null;
            if (_attack == 0)
            {
                return 0;
            }
            for (int i = 0; i < _attack.handle.BuffHolderComp.SpawnedBuffList.Count; i++)
            {
                skill = _attack.handle.BuffHolderComp.SpawnedBuffList[i];
                num += this.OnConditionExtraHurt(skill, _attack);
                num += this.OnTargetExtraHurt(skill, _attack);
                num += this.OnControlExtraHurt(skill, _attack);
            }
            return num;
        }

        public int GetSoulExpAddRate(PoolObjHandle<ActorRoot> _target)
        {
            int num = 0;
            BuffSkill skill = null;
            ResDT_SkillFunc outSkillFunc = null;
            if (_target != 0)
            {
                for (int i = 0; i < this.SpawnedBuffList.Count; i++)
                {
                    skill = this.SpawnedBuffList[i];
                    if ((skill != null) && skill.FindSkillFunc(0x31, out outSkillFunc))
                    {
                        int typeMask = skill.GetSkillFuncParam(outSkillFunc, 0, false);
                        int typeSubMask = skill.GetSkillFuncParam(outSkillFunc, 1, false);
                        int num5 = skill.GetSkillFuncParam(outSkillFunc, 2, false);
                        int num6 = skill.GetSkillFuncParam(outSkillFunc, 3, false);
                        if (this.CheckTargetSubType(typeMask, typeSubMask, _target))
                        {
                            bool flag = true;
                            if (num6 > 0)
                            {
                                flag = this.CheckTargetFromEnemy(base.actorPtr, _target);
                            }
                            if (flag)
                            {
                                num += num5;
                            }
                        }
                    }
                }
            }
            return num;
        }

        public override void Init()
        {
            this.overlayRule = new BuffOverlayRule();
            this.clearRule = new BuffClearRule();
            this.protectRule = new BuffProtectRule();
            this.changeSkillRule = new BuffChangeSkillRule();
            this.markRule = new BufferMarkRule();
            this.logicEffect = new BufferLogicEffect();
            this.overlayRule.Init(this);
            this.clearRule.Init(this);
            this.protectRule.Init(this);
            this.changeSkillRule.Init(this);
            this.markRule.Init(this);
            this.logicEffect.Init(this);
            base.Init();
        }

        public bool IsExistSkillFuncType(int inSkillFuncType)
        {
            BuffSkill skill = null;
            ResDT_SkillFunc outSkillFunc = null;
            for (int i = 0; i < this.SpawnedBuffList.Count; i++)
            {
                skill = this.SpawnedBuffList[i];
                if ((skill != null) && skill.FindSkillFunc(inSkillFuncType, out outSkillFunc))
                {
                    return true;
                }
            }
            return false;
        }

        public void OnAssistEffect(ref PoolObjHandle<ActorRoot> deadActor)
        {
            BuffSkill skill = null;
            ResDT_SkillFunc outSkillFunc = null;
            for (int i = 0; i < this.SpawnedBuffList.Count; i++)
            {
                skill = this.SpawnedBuffList[i];
                if ((skill != null) && skill.FindSkillFunc(0x21, out outSkillFunc))
                {
                    int inSkillCombineId = skill.GetSkillFuncParam(outSkillFunc, 0, false);
                    int num3 = skill.GetSkillFuncParam(outSkillFunc, 1, false);
                    int num4 = skill.GetSkillFuncParam(outSkillFunc, 2, false);
                    int typeMask = skill.GetSkillFuncParam(outSkillFunc, 3, false);
                    int typeSubMask = skill.GetSkillFuncParam(outSkillFunc, 4, false);
                    if ((num4 == 2) && this.CheckTargetSubType(typeMask, typeSubMask, deadActor))
                    {
                        SkillUseParam inParam = new SkillUseParam();
                        inParam.Init();
                        inParam.SetOriginator(base.actorPtr);
                        inParam.bExposing = skill.skillContext.bExposing;
                        base.actor.SkillControl.SpawnBuff(base.actorPtr, ref inParam, inSkillCombineId, true);
                    }
                }
            }
        }

        private bool OnChangeExtraEffectSkillSlot(PoolObjHandle<ActorRoot> _attack, SkillSlotType _slotType, out SkillSlotType _outSlotType)
        {
            BuffSkill skill = null;
            ResDT_SkillFunc outSkillFunc = null;
            _outSlotType = _slotType;
            for (int i = 0; i < _attack.handle.BuffHolderComp.SpawnedBuffList.Count; i++)
            {
                skill = _attack.handle.BuffHolderComp.SpawnedBuffList[i];
                if ((skill != null) && skill.FindSkillFunc(0x4e, out outSkillFunc))
                {
                    int num2 = skill.GetSkillFuncParam(outSkillFunc, 0, false);
                    int num3 = skill.GetSkillFuncParam(outSkillFunc, 1, false);
                    if (_slotType == num2)
                    {
                        _outSlotType = (SkillSlotType) num3;
                        return true;
                    }
                }
            }
            return false;
        }

        private int OnConditionExtraHurt(BuffSkill _buffSkill, PoolObjHandle<ActorRoot> _attack)
        {
            int num = 0;
            ResDT_SkillFunc outSkillFunc = null;
            if ((_buffSkill != null) && _buffSkill.FindSkillFunc(0x2c, out outSkillFunc))
            {
                int num2 = _buffSkill.GetSkillFuncParam(outSkillFunc, 0, false);
                int num3 = _buffSkill.GetSkillFuncParam(outSkillFunc, 1, false);
                int num4 = _buffSkill.GetSkillFuncParam(outSkillFunc, 2, false);
                int num5 = _buffSkill.GetSkillFuncParam(outSkillFunc, 3, false);
                bool flag = num2 == 1;
                int num6 = !flag ? base.actor.ValueComponent.actorHp : _attack.handle.ValueComponent.actorHp;
                int num7 = !flag ? base.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalValue : _attack.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalValue;
                int num8 = (num6 * 0x2710) / num7;
                if (num4 == 1)
                {
                    if (num8 <= num3)
                    {
                        num = num5;
                    }
                    return num;
                }
                if ((num4 == 4) && (num8 >= num3))
                {
                    num = num5;
                }
            }
            return num;
        }

        private int OnControlExtraHurt(BuffSkill _buffSkill, PoolObjHandle<ActorRoot> _attack)
        {
            ResDT_SkillFunc outSkillFunc = null;
            if (((_buffSkill != null) && _buffSkill.FindSkillFunc(0x33, out outSkillFunc)) && (base.actor != null))
            {
                BuffSkill skill = null;
                for (int i = 0; i < base.actor.BuffHolderComp.SpawnedBuffList.Count; i++)
                {
                    skill = base.actor.BuffHolderComp.SpawnedBuffList[i];
                    if ((skill != null) && (skill.cfgData.bEffectType == 2))
                    {
                        return _buffSkill.GetSkillFuncParam(outSkillFunc, 0, false);
                    }
                }
            }
            return 0;
        }

        public int OnDamage(ref HurtDataInfo _hurt, int _hurtValue)
        {
            int num = _hurtValue;
            if (!_hurt.bLastHurt)
            {
                this.clearRule.CheckBuffClear(RES_SKILLFUNC_CLEAR_RULE.RES_SKILLFUNC_CLEAR_DAMAGE);
            }
            if (!_hurt.bExtraBuff)
            {
                SkillSlotType type;
                SkillSlotType atkSlot = _hurt.atkSlot;
                bool flag = this.OnChangeExtraEffectSkillSlot(_hurt.atker, _hurt.atkSlot, out type);
                if (flag)
                {
                    _hurt.atkSlot = type;
                }
                this.OnDamageTriggerEffect(_hurt.target, _hurt.atker);
                this.OnDamageExtraEffect(_hurt.atker, _hurt.atkSlot);
                if (flag)
                {
                    _hurt.atkSlot = atkSlot;
                }
            }
            num = (num * _hurt.iEffectFadeRate) / 0x2710;
            num = (num * _hurt.iOverlayFadeRate) / 0x2710;
            num = this.protectRule.ResistDamage(ref _hurt, num);
            num = BufferLogicEffect.OnDamageExtraEffect(ref _hurt, num);
            num = this.DealDamageContionType(ref _hurt, num);
            this.OnDamageExtraHurtFunc(ref _hurt, _hurt.atkSlot);
            return num;
        }

        private void OnDamageExtraEffect(PoolObjHandle<ActorRoot> _attack, SkillSlotType _slotType)
        {
            BuffSkill inBuff = null;
            ResDT_SkillFunc outSkillFunc = null;
            if (_attack != 0)
            {
                for (int i = 0; i < _attack.handle.BuffHolderComp.SpawnedBuffList.Count; i++)
                {
                    inBuff = _attack.handle.BuffHolderComp.SpawnedBuffList[i];
                    if ((inBuff != null) && inBuff.FindSkillFunc(0x21, out outSkillFunc))
                    {
                        bool flag = false;
                        bool flag2 = true;
                        int inSkillCombineId = inBuff.GetSkillFuncParam(outSkillFunc, 0, false);
                        int num5 = inBuff.GetSkillFuncParam(outSkillFunc, 1, false);
                        int num6 = inBuff.GetSkillFuncParam(outSkillFunc, 2, false);
                        int typeMask = inBuff.GetSkillFuncParam(outSkillFunc, 3, false);
                        int typeSubMask = inBuff.GetSkillFuncParam(outSkillFunc, 4, false);
                        int num9 = inBuff.GetSkillFuncParam(outSkillFunc, 5, false);
                        int conditionalType = inBuff.GetSkillFuncParam(outSkillFunc, 6, false);
                        int iParam = inBuff.GetSkillFuncParam(outSkillFunc, 7, false);
                        if (((num6 == 0) && this.CheckTargetSubType(typeMask, typeSubMask, base.actorPtr)) && ((conditionalType == 0) || this.CheckTriggerCondtion(conditionalType, iParam, _attack, base.actorPtr)))
                        {
                            if (num5 == 0)
                            {
                                flag = true;
                            }
                            else if ((num5 & (((int) 1) << _slotType)) > 0)
                            {
                                flag = true;
                            }
                            if (num9 > 0)
                            {
                                if ((Singleton<FrameSynchr>.GetInstance().LogicFrameTick - inBuff.controlTime) >= num9)
                                {
                                    flag2 = true;
                                }
                                else
                                {
                                    flag2 = false;
                                }
                            }
                            if ((flag && flag2) && ((num9 != -2) || !inBuff.IsNextDestroy()))
                            {
                                SkillUseParam inParam = new SkillUseParam();
                                inParam.Init();
                                inParam.SetOriginator(_attack);
                                inParam.bExposing = inBuff.skillContext.bExposing;
                                if (inBuff.skillContext != null)
                                {
                                    if (inBuff.skillContext.SlotType != SkillSlotType.SLOT_SKILL_COUNT)
                                    {
                                        inParam.SlotType = _slotType;
                                    }
                                    else
                                    {
                                        inParam.SlotType = inBuff.skillContext.SlotType;
                                    }
                                }
                                else
                                {
                                    inParam.SlotType = _slotType;
                                }
                                _attack.handle.SkillControl.SpawnBuff(base.actorPtr, ref inParam, inSkillCombineId, true);
                                inBuff.controlTime = Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
                                switch (num9)
                                {
                                    case -1:
                                    case -2:
                                        _attack.handle.BuffHolderComp.RemoveBuff(inBuff);
                                        break;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void OnDamageExtraHurtFunc(ref HurtDataInfo _hurt, SkillSlotType _slotType)
        {
            BuffSkill skill = null;
            ResDT_SkillFunc outSkillFunc = null;
            for (int i = 0; i < this.SpawnedBuffList.Count; i++)
            {
                skill = this.SpawnedBuffList[i];
                if ((skill != null) && skill.FindSkillFunc(80, out outSkillFunc))
                {
                    SkillSlotType type = (SkillSlotType) skill.GetSkillFuncParam(outSkillFunc, 0, false);
                    if (_slotType == type)
                    {
                        int num2 = skill.GetSkillFuncParam(outSkillFunc, 1, false);
                        _hurt.iReduceDamage += num2;
                    }
                }
            }
        }

        public void OnDamageExtraValueEffect(ref HurtDataInfo _hurt, PoolObjHandle<ActorRoot> _attack, SkillSlotType _slotType)
        {
            BuffSkill skill = null;
            ResDT_SkillFunc outSkillFunc = null;
            if (_attack != 0)
            {
                for (int i = 0; i < _attack.handle.BuffHolderComp.SpawnedBuffList.Count; i++)
                {
                    skill = _attack.handle.BuffHolderComp.SpawnedBuffList[i];
                    if (_hurt.hurtType == HurtTypeDef.Therapic)
                    {
                        if ((skill != null) && skill.FindSkillFunc(0x40, out outSkillFunc))
                        {
                            _hurt.iAddTotalHurtValueRate = _attack.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_RECOVERYGAINEFFECT].addRatio;
                            _hurt.iAddTotalHurtValue = _attack.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_RECOVERYGAINEFFECT].addValue;
                        }
                    }
                    else
                    {
                        if ((((_slotType == SkillSlotType.SLOT_SKILL_0) && (_attack.handle.SkillControl != null)) && (_attack.handle.SkillControl.bIsLastAtkUseSkill && (skill != null))) && skill.FindSkillFunc(0x3d, out outSkillFunc))
                        {
                            int num2 = skill.GetSkillFuncParam(outSkillFunc, 0, false);
                            int num3 = skill.GetSkillFuncParam(outSkillFunc, 1, false);
                            int num4 = skill.GetSkillFuncParam(outSkillFunc, 2, false);
                            int num5 = skill.GetSkillFuncParam(outSkillFunc, 3, false);
                            if (num2 == 1)
                            {
                                num3 = (num3 * _hurt.hurtValue) / 0x2710;
                                _hurt.hurtValue += num3;
                                _hurt.adValue += num4;
                                _hurt.apValue += num5;
                            }
                            else
                            {
                                _hurt.hurtValue += num3;
                                _hurt.attackInfo.iActorATT += num4;
                                _hurt.attackInfo.iActorINT += num5;
                            }
                        }
                        if (((_hurt.target != 0) && (_hurt.target.handle.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Organ)) && ((skill != null) && skill.FindSkillFunc(0x44, out outSkillFunc)))
                        {
                            int num6 = skill.GetSkillFuncParam(outSkillFunc, 0, false);
                            int num7 = skill.GetSkillFuncParam(outSkillFunc, 4, false);
                            int num8 = skill.GetSkillFuncParam(outSkillFunc, 5, false);
                            if (_hurt.target.handle.ValueComponent != null)
                            {
                                if (num6 == 1)
                                {
                                    num7 = (_hurt.target.handle.ValueComponent.actorHpTotal * num7) / 0x2710;
                                }
                                if (((_hurt.target.handle.ValueComponent.actorHp <= num7) && (_hurt.target.handle.ActorControl != null)) && ((Singleton<FrameSynchr>.instance.LogicFrameTick - _hurt.target.handle.ActorControl.lastExtraHurtByLowHpBuffTime) >= num8))
                                {
                                    _hurt.target.handle.ActorControl.lastExtraHurtByLowHpBuffTime = Singleton<FrameSynchr>.instance.LogicFrameTick;
                                    int num9 = skill.GetSkillFuncParam(outSkillFunc, 1, false);
                                    int num10 = skill.GetSkillFuncParam(outSkillFunc, 2, false);
                                    int num11 = skill.GetSkillFuncParam(outSkillFunc, 3, false);
                                    if (num6 == 1)
                                    {
                                        num9 = (num9 * _hurt.hurtValue) / 0x2710;
                                        num10 = (num10 * _hurt.adValue) / 0x2710;
                                        num11 = (num11 * _hurt.apValue) / 0x2710;
                                    }
                                    _hurt.hurtValue += num9;
                                    _hurt.adValue += num10;
                                    _hurt.apValue += num11;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void OnDamageTriggerEffect(PoolObjHandle<ActorRoot> _selfActor, PoolObjHandle<ActorRoot> _attacker)
        {
            BuffSkill inBuff = null;
            ResDT_SkillFunc outSkillFunc = null;
            if (((_selfActor != 0) && (_attacker != 0)) && (_attacker.handle.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Organ))
            {
                for (int i = 0; i < _selfActor.handle.BuffHolderComp.SpawnedBuffList.Count; i++)
                {
                    inBuff = _selfActor.handle.BuffHolderComp.SpawnedBuffList[i];
                    if ((inBuff != null) && inBuff.FindSkillFunc(0x54, out outSkillFunc))
                    {
                        int inSkillCombineId = inBuff.GetSkillFuncParam(outSkillFunc, 0, false);
                        if (inSkillCombineId > 0)
                        {
                            SkillUseParam inParam = new SkillUseParam();
                            inParam.Init();
                            inParam.SetOriginator(_selfActor);
                            inParam.bExposing = inBuff.skillContext.bExposing;
                            _selfActor.handle.SkillControl.SpawnBuff(_attacker, ref inParam, inSkillCombineId, true);
                            _selfActor.handle.BuffHolderComp.RemoveBuff(inBuff);
                        }
                    }
                }
            }
        }

        public void OnDead(PoolObjHandle<ActorRoot> _attack)
        {
            BuffSkill inBuff = null;
            ResDT_SkillFunc outSkillFunc = null;
            if (this.clearRule != null)
            {
                this.clearRule.CheckBuffNoClear(RES_SKILLFUNC_CLEAR_RULE.RES_SKILLFUNC_CLEAR_DEAD);
            }
            if (this.logicEffect != null)
            {
                this.logicEffect.Clear();
            }
            if (this.actorPtr.handle.ActorControl.IsKilledByHero())
            {
                _attack = this.actorPtr.handle.ActorControl.LastHeroAtker;
            }
            for (int i = 0; i < this.SpawnedBuffList.Count; i++)
            {
                inBuff = this.SpawnedBuffList[i];
                if ((inBuff != null) && inBuff.FindSkillFunc(0x20, out outSkillFunc))
                {
                    int reviveTime = inBuff.GetSkillFuncParam(outSkillFunc, 0, false);
                    int reviveLife = inBuff.GetSkillFuncParam(outSkillFunc, 1, false);
                    bool autoReset = inBuff.GetSkillFuncParam(outSkillFunc, 2, false) == 1;
                    bool bBaseRevive = inBuff.GetSkillFuncParam(outSkillFunc, 3, false) == 0;
                    bool bCDReset = inBuff.GetSkillFuncParam(outSkillFunc, 4, false) == 1;
                    base.actor.ActorControl.SetReviveContext(reviveTime, reviveLife, autoReset, bBaseRevive, bCDReset);
                    this.RemoveBuff(inBuff);
                }
                if ((((base.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero) && (_attack != 0)) && ((_attack.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero) && (inBuff != null))) && ((inBuff.cfgData != null) && (inBuff.cfgData.bIsInheritByKiller == 1)))
                {
                    this.RemoveBuff(inBuff);
                    SkillUseParam inParam = new SkillUseParam();
                    inParam.SetOriginator(_attack);
                    inParam.bExposing = inBuff.skillContext.bExposing;
                    _attack.handle.SkillControl.SpawnBuff(_attack, ref inParam, inBuff.SkillID, true);
                }
            }
            this.OnDeadExtraEffect(_attack);
            this.markRule.Clear();
        }

        private void OnDeadExtraEffect(PoolObjHandle<ActorRoot> _attack)
        {
            if (_attack != 0)
            {
                BuffSkill skill = null;
                ResDT_SkillFunc outSkillFunc = null;
                for (int i = 0; i < _attack.handle.BuffHolderComp.SpawnedBuffList.Count; i++)
                {
                    skill = _attack.handle.BuffHolderComp.SpawnedBuffList[i];
                    if ((skill != null) && skill.FindSkillFunc(0x21, out outSkillFunc))
                    {
                        int inSkillCombineId = skill.GetSkillFuncParam(outSkillFunc, 0, false);
                        int num3 = skill.GetSkillFuncParam(outSkillFunc, 1, false);
                        int num4 = skill.GetSkillFuncParam(outSkillFunc, 2, false);
                        int typeMask = skill.GetSkillFuncParam(outSkillFunc, 3, false);
                        int typeSubMask = skill.GetSkillFuncParam(outSkillFunc, 4, false);
                        if ((num4 == 1) && this.CheckTargetSubType(typeMask, typeSubMask, base.actorPtr))
                        {
                            SkillUseParam inParam = new SkillUseParam();
                            inParam.Init();
                            inParam.SetOriginator(_attack);
                            inParam.bExposing = skill.skillContext.bExposing;
                            _attack.handle.SkillControl.SpawnBuff(_attack, ref inParam, inSkillCombineId, true);
                        }
                    }
                }
            }
        }

        public int OnHurtBounceDamage(ref HurtDataInfo hurt, int hp)
        {
            if (hp <= 0)
            {
                return hp;
            }
            if ((hurt.atker == 0) || hurt.bBounceHurt)
            {
                return hp;
            }
            BuffSkill skill = null;
            ResDT_SkillFunc outSkillFunc = null;
            int num = hp;
            for (int i = 0; i < this.SpawnedBuffList.Count; i++)
            {
                skill = this.SpawnedBuffList[i];
                if (((skill != null) && skill.FindSkillFunc(0x53, out outSkillFunc)) && ((skill.GetSkillFuncParam(outSkillFunc, 2, false) & (((int) 1) << hurt.atker.handle.TheActorMeta.ActorType)) <= 0))
                {
                    int num4 = skill.GetSkillFuncParam(outSkillFunc, 0, false);
                    int num5 = skill.GetSkillFuncParam(outSkillFunc, 1, false);
                    int num6 = (num * num4) / 0x2710;
                    num -= num6;
                    HurtDataInfo info = new HurtDataInfo();
                    HurtAttackerInfo info2 = new HurtAttackerInfo();
                    info2.Init(hurt.target, hurt.atker);
                    info.atker = hurt.target;
                    info.target = hurt.atker;
                    info.attackInfo = info2;
                    info.atkSlot = SkillSlotType.SLOT_SKILL_VALID;
                    info.hurtType = (HurtTypeDef) num5;
                    info.extraHurtType = ExtraHurtTypeDef.ExtraHurt_Value;
                    info.hurtValue = num6;
                    info.adValue = 0;
                    info.apValue = 0;
                    info.hpValue = 0;
                    info.loseHpValue = 0;
                    info.iConditionType = 0;
                    info.iConditionParam = 0;
                    info.hurtCount = 0;
                    info.firstHemoFadeRate = 0;
                    info.followUpHemoFadeRate = 0;
                    info.iEffectCountInSingleTrigger = 0;
                    info.bExtraBuff = false;
                    info.gatherTime = 0;
                    info.bBounceHurt = true;
                    info.bLastHurt = false;
                    info.iAddTotalHurtValueRate = 0;
                    info.iAddTotalHurtValue = 0;
                    info.iEffectFadeRate = 0x2710;
                    info.iOverlayFadeRate = 0x2710;
                    int num7 = hurt.atker.handle.HurtControl.TakeBouncesDamage(ref info);
                }
            }
            return num;
        }

        private int OnTargetExtraHurt(BuffSkill _buffSkill, PoolObjHandle<ActorRoot> _attack)
        {
            int num = 0;
            ResDT_SkillFunc outSkillFunc = null;
            if ((_buffSkill != null) && _buffSkill.FindSkillFunc(0x30, out outSkillFunc))
            {
                int typeMask = _buffSkill.GetSkillFuncParam(outSkillFunc, 0, false);
                int typeSubMask = _buffSkill.GetSkillFuncParam(outSkillFunc, 1, false);
                int num4 = _buffSkill.GetSkillFuncParam(outSkillFunc, 2, false);
                if (this.CheckTargetSubType(typeMask, typeSubMask, base.actorPtr))
                {
                    num = num4;
                }
            }
            return num;
        }

        public override void OnUse()
        {
            base.OnUse();
            this.SpawnedBuffList.Clear();
            this.overlayRule = null;
            this.clearRule = null;
            this.protectRule = null;
            this.changeSkillRule = null;
            this.markRule = null;
            this.logicEffect = null;
            this.bRemoveList = true;
            this.delBuffList.Clear();
        }

        public override void Reactive()
        {
            base.Reactive();
            this.overlayRule.Init(this);
            this.clearRule.Init(this);
            this.protectRule.Init(this);
            this.changeSkillRule.Init(this);
            this.markRule.Init(this);
            this.logicEffect.Init(this);
        }

        public void RemoveBuff(BuffSkill inBuff)
        {
            if (this.SpawnedBuffList.Count != 0)
            {
                this.delBuffList = this.SpawnedBuffList;
                for (int i = 0; i < this.delBuffList.Count; i++)
                {
                    BuffSkill skill = this.delBuffList[i];
                    if (skill == inBuff)
                    {
                        BuffChangeEventParam param = new BuffChangeEventParam(false, base.actorPtr, inBuff);
                        Singleton<GameSkillEventSys>.GetInstance().SendEvent<BuffChangeEventParam>(GameSkillEventDef.AllEvent_BuffChange, base.actorPtr, ref param, GameSkillEventChannel.Channel_HostCtrlActor);
                        skill.Stop();
                        if (((inBuff.cfgData.bEffectType == 2) && (inBuff.cfgData.bShowType != 2)) && (base.actorPtr != 0))
                        {
                            LimitMoveEventParam param2 = new LimitMoveEventParam(0, inBuff.SkillID, base.actorPtr);
                            Singleton<GameSkillEventSys>.GetInstance().SendEvent<LimitMoveEventParam>(GameSkillEventDef.AllEvent_CancelLimitMove, base.actorPtr, ref param2, GameSkillEventChannel.Channel_AllActor);
                        }
                    }
                }
            }
        }

        public void RemoveBuff(int inSkillCombineId)
        {
            if (this.SpawnedBuffList.Count != 0)
            {
                this.delBuffList = this.SpawnedBuffList;
                for (int i = 0; i < this.delBuffList.Count; i++)
                {
                    BuffSkill skill = this.delBuffList[i];
                    if ((skill != null) && (skill.SkillID == inSkillCombineId))
                    {
                        skill.Stop();
                        if (((skill.cfgData.bEffectType == 2) && (skill.cfgData.bShowType != 2)) && (base.actorPtr != 0))
                        {
                            LimitMoveEventParam param = new LimitMoveEventParam(0, skill.SkillID, base.actorPtr);
                            Singleton<GameSkillEventSys>.GetInstance().SendEvent<LimitMoveEventParam>(GameSkillEventDef.AllEvent_CancelLimitMove, base.actorPtr, ref param, GameSkillEventChannel.Channel_AllActor);
                        }
                    }
                }
            }
        }

        public void RemoveSkillEffectGroup(int inGroupID)
        {
            if (this.SpawnedBuffList.Count != 0)
            {
                this.delBuffList = this.SpawnedBuffList;
                for (int i = 0; i < this.delBuffList.Count; i++)
                {
                    BuffSkill skill = this.delBuffList[i];
                    if (((skill != null) && (skill.cfgData != null)) && (skill.cfgData.iCroupID == inGroupID))
                    {
                        skill.Stop();
                        if (((skill.cfgData.bEffectType == 2) && (skill.cfgData.bShowType != 2)) && (base.actorPtr != 0))
                        {
                            LimitMoveEventParam param = new LimitMoveEventParam(0, skill.SkillID, base.actorPtr);
                            Singleton<GameSkillEventSys>.GetInstance().SendEvent<LimitMoveEventParam>(GameSkillEventDef.AllEvent_CancelLimitMove, base.actorPtr, ref param, GameSkillEventChannel.Channel_AllActor);
                        }
                    }
                }
            }
        }

        public override void UpdateLogic(int nDelta)
        {
            if (this.markRule != null)
            {
                this.markRule.UpdateLogic(nDelta);
            }
        }
    }
}

