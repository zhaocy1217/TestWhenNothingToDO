namespace Assets.Scripts.GameLogic
{
    using ResData;
    using System;

    [SkillBaseSelectTarget(SkillTargetRule.NextSkillTarget)]
    public class SkillSelectedNextSkillTarget : SkillBaseSelectTarget
    {
        private ActorRoot GetNextSkillTarget(SkillSlot UseSlot)
        {
            for (int i = 0; i < UseSlot.NextSkillTargetIDs.Count; i++)
            {
                ActorRoot actor = (ActorRoot) Singleton<GameObjMgr>.GetInstance().GetActor(UseSlot.NextSkillTargetIDs[i]);
                if (((actor != null) && (((UseSlot.SkillObj.cfgData.dwSkillTargetFilter & (((int) 1) << actor.TheActorMeta.ActorType)) <= 0L) && actor.HorizonMarker.IsVisibleFor(UseSlot.Actor.handle.TheActorMeta.ActorCamp))) && (UseSlot.Actor.handle.CanAttack(actor) && DistanceSearchCondition.Fit(actor, UseSlot.Actor.handle, UseSlot.SkillObj.GetMaxSearchDistance(UseSlot.GetSkillLevel()))))
                {
                    return actor;
                }
            }
            return null;
        }

        public override ActorRoot SelectTarget(SkillSlot UseSlot)
        {
            return this.GetNextSkillTarget(UseSlot);
        }

        public override VInt3 SelectTargetDir(SkillSlot UseSlot)
        {
            ActorRoot nextSkillTarget = this.GetNextSkillTarget(UseSlot);
            if (nextSkillTarget != null)
            {
                VInt3 num = nextSkillTarget.location - UseSlot.Actor.handle.location;
                num.y = 0;
                return num.NormalizeTo(0x3e8);
            }
            return UseSlot.Actor.handle.forward;
        }
    }
}

