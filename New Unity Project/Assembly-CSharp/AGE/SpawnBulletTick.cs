namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using ResData;
    using System;

    [EventCategory("MMGame/Skill")]
    public class SpawnBulletTick : TickEvent
    {
        [AssetReference(AssetRefType.Action)]
        public string ActionName;
        public bool bAgeImmeExcute;
        public bool bDeadRemove;
        public bool bSpawnBounceBullet;
        public int bulletTypeId;
        public int bulletUpperLimit;
        [AssetReference(AssetRefType.Action)]
        public string SpecialActionName = string.Empty;
        [ObjectTemplate(new Type[] {  })]
        public int targetId = -1;

        public override BaseEvent Clone()
        {
            SpawnBulletTick tick = ClassObjPool<SpawnBulletTick>.Get();
            tick.CopyData(this);
            return tick;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            SpawnBulletTick tick = src as SpawnBulletTick;
            this.targetId = tick.targetId;
            this.ActionName = tick.ActionName;
            this.SpecialActionName = tick.SpecialActionName;
            this.bDeadRemove = tick.bDeadRemove;
            this.bAgeImmeExcute = tick.bAgeImmeExcute;
            this.bulletUpperLimit = tick.bulletUpperLimit;
            this.bulletTypeId = tick.bulletTypeId;
            this.bSpawnBounceBullet = tick.bSpawnBounceBullet;
        }

        public override void Process(Action _action, Track _track)
        {
            PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.targetId);
            if (actorHandle == 0)
            {
                if (ActionManager.Instance.isPrintLog)
                {
                }
            }
            else
            {
                SkillComponent skillControl = actorHandle.handle.SkillControl;
                if (skillControl == null)
                {
                    if (ActionManager.Instance.isPrintLog)
                    {
                    }
                }
                else
                {
                    SkillUseContext refParamObject = _action.refParams.GetRefParamObject<SkillUseContext>("SkillContext");
                    if (refParamObject != null)
                    {
                        refParamObject.BulletPos = refParamObject.UseVector;
                        SkillRangeAppointType appointType = refParamObject.AppointType;
                        if (this.bSpawnBounceBullet)
                        {
                            appointType = refParamObject.AppointType;
                            refParamObject.AppointType = SkillRangeAppointType.Target;
                        }
                        if (!refParamObject.bSpecialUse)
                        {
                            if (this.ActionName != string.Empty)
                            {
                                skillControl.SpawnBullet(refParamObject, this.ActionName, this.bDeadRemove, this.bAgeImmeExcute, this.bulletTypeId, this.bulletUpperLimit);
                            }
                        }
                        else if (this.SpecialActionName != string.Empty)
                        {
                            skillControl.SpawnBullet(refParamObject, this.SpecialActionName, this.bDeadRemove, this.bAgeImmeExcute, this.bulletTypeId, this.bulletUpperLimit);
                        }
                        refParamObject.AppointType = appointType;
                    }
                }
            }
        }
    }
}

