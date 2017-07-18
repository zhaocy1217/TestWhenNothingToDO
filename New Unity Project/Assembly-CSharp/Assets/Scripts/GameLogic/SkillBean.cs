namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Framework;
    using ResData;
    using System;

    public class SkillBean
    {
        private int _beanAmount;
        private ResSkillBeanCfgInfo _cfgData;
        private int _deltaTime;
        private SkillSlot _skillSlot;

        public SkillBean(SkillSlot skillSlot)
        {
            this._skillSlot = skillSlot;
        }

        public void BeanUse()
        {
            if ((this._cfgData != null) && (this.BeanAmount > 0))
            {
                this.BeanAmount--;
            }
        }

        public bool ConsumeBean()
        {
            return (this._cfgData != null);
        }

        public int GetBeanAmount()
        {
            return this._beanAmount;
        }

        public void Init()
        {
            if (((this._skillSlot != null) && (this._skillSlot.Actor != 0)) && (this._skillSlot.Actor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero))
            {
                ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey((long) this._skillSlot.Actor.handle.TheActorMeta.ConfigId);
                int slotType = (int) this._skillSlot.SlotType;
                if (((dataByKey != null) && (slotType >= 1)) && (slotType <= 3))
                {
                    this._cfgData = GameDataMgr.skillBeanDatabin.GetDataByKey(dataByKey.astSkill[slotType].dwSkillBeanID);
                }
            }
        }

        public bool IsBeanEnough()
        {
            return ((this._cfgData == null) || (this.BeanAmount > 0));
        }

        public void UpdateLogic(int delta)
        {
            if ((this._cfgData != null) && (!this._skillSlot.Actor.handle.ActorControl.IsDeadState && (this.BeanAmount < this.upperLimit)))
            {
                if (this._deltaTime > 0)
                {
                    this._deltaTime -= delta;
                    this._deltaTime = (this._deltaTime <= 0) ? 0 : this._deltaTime;
                }
                if (this._deltaTime <= 0)
                {
                    this.BeanAmount++;
                    this._deltaTime = this.cdTime;
                }
            }
        }

        private int BeanAmount
        {
            get
            {
                return this._beanAmount;
            }
            set
            {
                if ((value >= 0) && (value <= this.upperLimit))
                {
                    this._beanAmount = value;
                    DefaultSkillEventParam param = new DefaultSkillEventParam(this._skillSlot.SlotType, this._beanAmount, this._skillSlot.Actor);
                    Singleton<GameSkillEventSys>.GetInstance().SendEvent<DefaultSkillEventParam>(GameSkillEventDef.Event_ChangeSkillBean, this._skillSlot.Actor, ref param, GameSkillEventChannel.Channel_HostCtrlActor);
                }
            }
        }

        private int cdTime
        {
            get
            {
                return (int) (this._cfgData.dwCDTime + ((((long) (this._skillSlot.GetSkillLevel() - 1)) / ((ulong) this._cfgData.dwCDTimeInterval)) * this._cfgData.iCDTimeIGrowth));
            }
        }

        private int upperLimit
        {
            get
            {
                if (this._cfgData.dwUpperLimitInterval > 0)
                {
                    return (int) (this._cfgData.dwUpperLimit + ((((long) (this._skillSlot.GetSkillLevel() - 1)) / ((ulong) this._cfgData.dwUpperLimitInterval)) * this._cfgData.dwUpperLimitGrowth));
                }
                return 0;
            }
        }
    }
}

