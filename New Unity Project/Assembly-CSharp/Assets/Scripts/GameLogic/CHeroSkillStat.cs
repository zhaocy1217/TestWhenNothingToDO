namespace Assets.Scripts.GameLogic
{
    using System;

    public class CHeroSkillStat
    {
        public void Clear()
        {
            Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<BuffChangeEventParam>(GameSkillEventDef.AllEvent_BuffChange, new GameSkillEvent<BuffChangeEventParam>(this.OnActorBuffSkillChange));
            Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<SpawnEyeEventParam>(GameSkillEventDef.Event_SpawnEye, new GameSkillEvent<SpawnEyeEventParam>(this.OnActorSpawnEye));
            Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_UpdateSkillUI, new GameSkillEvent<DefaultSkillEventParam>(this.OnActorChangeSkill));
        }

        private void OnActorBuffSkillChange(ref BuffChangeEventParam prm)
        {
            if (((((!prm.bIsAdd && ((prm.stBuffSkill != 0) && (prm.stBuffSkill.handle.skillContext != null))) && ((prm.stBuffSkill.handle.skillContext.Originator != 0) && (prm.stBuffSkill.handle.skillContext.TargetActor != 0))) && ((prm.stBuffSkill.handle.skillContext.SlotType >= SkillSlotType.SLOT_SKILL_1) && (prm.stBuffSkill.handle.skillContext.SlotType < SkillSlotType.SLOT_SKILL_COUNT))) && (prm.stBuffSkill.handle.cfgData.bEffectType == 2)) && ((((prm.stBuffSkill.handle.cfgData.bShowType == 1) || (prm.stBuffSkill.handle.cfgData.bShowType == 3)) || ((prm.stBuffSkill.handle.cfgData.bShowType == 4) || (prm.stBuffSkill.handle.cfgData.bShowType == 5))) || (prm.stBuffSkill.handle.cfgData.bShowType == 6)))
            {
                ulong num = Singleton<FrameSynchr>.GetInstance().LogicFrameTick - prm.stBuffSkill.handle.ulStartTime;
                if (prm.stBuffSkill.handle.skillContext.Originator.handle.SkillControl != null)
                {
                    prm.stBuffSkill.handle.skillContext.Originator.handle.SkillControl.stSkillStat.m_uiStunTime += (uint) num;
                }
                if (prm.stBuffSkill.handle.skillContext.TargetActor.handle.SkillControl != null)
                {
                    prm.stBuffSkill.handle.skillContext.TargetActor.handle.SkillControl.stSkillStat.m_uiBeStunnedTime += (uint) num;
                }
            }
        }

        private void OnActorChangeSkill(ref DefaultSkillEventParam prm)
        {
            if ((((prm.actor != 0) && (prm.actor.handle.SkillControl != null)) && (prm.actor.handle.SkillControl.stSkillStat != null)) && (prm.slot == SkillSlotType.SLOT_SKILL_7))
            {
                prm.actor.handle.SkillControl.stSkillStat.m_uiEyeSwitchTimes++;
            }
        }

        private void OnActorSpawnEye(ref SpawnEyeEventParam prm)
        {
            if (((prm.src != 0) && (prm.src.handle.SkillControl != null)) && (prm.src.handle.SkillControl.stSkillStat != null))
            {
                prm.src.handle.SkillControl.stSkillStat.m_uiRealSpawnEyeTimes++;
                if (prm.src.handle.SkillControl.stSkillStat.m_uiSpawnEyeTimes < 15)
                {
                    prm.src.handle.SkillControl.stSkillStat.stEyePostion[prm.src.handle.SkillControl.stSkillStat.m_uiSpawnEyeTimes] = prm.pos;
                    prm.src.handle.SkillControl.stSkillStat.m_uiSpawnEyeTimes++;
                }
            }
        }

        public void StartRecord()
        {
            this.Clear();
            Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<BuffChangeEventParam>(GameSkillEventDef.AllEvent_BuffChange, new GameSkillEvent<BuffChangeEventParam>(this.OnActorBuffSkillChange));
            Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<SpawnEyeEventParam>(GameSkillEventDef.Event_SpawnEye, new GameSkillEvent<SpawnEyeEventParam>(this.OnActorSpawnEye));
            Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_UpdateSkillUI, new GameSkillEvent<DefaultSkillEventParam>(this.OnActorChangeSkill));
        }
    }
}

