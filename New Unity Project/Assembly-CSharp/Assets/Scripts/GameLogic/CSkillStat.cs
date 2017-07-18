namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using System;

    public class CSkillStat
    {
        public PoolObjHandle<ActorRoot> actorHero;
        public uint m_uiBeStunnedTime;
        public uint m_uiEyeSwitchTimes;
        public uint m_uiRealSpawnEyeTimes;
        public uint m_uiSpawnEyeTimes;
        public uint m_uiStunTime;
        public SKILLSTATISTICTINFO[] SkillStatistictInfo = new SKILLSTATISTICTINFO[10];
        public VInt3[] stEyePostion = new VInt3[15];

        public int GetStunSkillNum()
        {
            int num = 0;
            if (this.actorHero == 0)
            {
                return 0;
            }
            SkillSlot[] skillSlotArray = this.actorHero.handle.SkillControl.SkillSlotArray;
            if (skillSlotArray == null)
            {
                return 0;
            }
            for (int i = 0; i < 10; i++)
            {
                if ((((skillSlotArray[i] != null) && (skillSlotArray[i].SkillObj != null)) && (skillSlotArray[i].SkillObj.cfgData != null)) && (skillSlotArray[i].SkillObj.cfgData.bIsStunSkill == 1))
                {
                    num++;
                }
            }
            return num;
        }

        public void Initialize(PoolObjHandle<ActorRoot> _actorHero)
        {
            this.m_uiStunTime = 0;
            this.m_uiBeStunnedTime = 0;
            this.actorHero = _actorHero;
            for (int i = 0; i < 10; i++)
            {
                this.SkillStatistictInfo[i] = new SKILLSTATISTICTINFO(0);
            }
            this.m_uiSpawnEyeTimes = 0;
            this.m_uiRealSpawnEyeTimes = 0;
            this.m_uiEyeSwitchTimes = 0;
        }

        public void UnInit()
        {
        }

        public uint BeStunTime
        {
            get
            {
                return this.m_uiBeStunnedTime;
            }
        }

        public uint StunTime
        {
            get
            {
                return this.m_uiStunTime;
            }
        }
    }
}

