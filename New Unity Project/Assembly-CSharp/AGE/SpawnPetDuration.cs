namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using System;
    using UnityEngine;

    [EventCategory("MMGame/Skill")]
    public class SpawnPetDuration : DurationEvent
    {
        private PoolObjHandle<ActorRoot> actorObj;
        public Vector3 offset;
        public PetType petType;
        [AssetReference(AssetRefType.Prefab)]
        public string prefabName = string.Empty;
        [ObjectTemplate(new Type[] {  })]
        public int targetId;

        public override BaseEvent Clone()
        {
            SpawnPetDuration duration = ClassObjPool<SpawnPetDuration>.Get();
            duration.CopyData(this);
            return duration;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            SpawnPetDuration duration = src as SpawnPetDuration;
            this.targetId = duration.targetId;
            this.prefabName = duration.prefabName;
            this.offset = duration.offset;
            this.petType = duration.petType;
        }

        public override void Enter(Action _action, Track _track)
        {
            base.Enter(_action, _track);
            this.actorObj = _action.GetActorHandle(this.targetId);
            if ((this.actorObj != 0) && (this.actorObj.handle.PetControl != null))
            {
                this.actorObj.handle.PetControl.CreatePet(this.petType, this.prefabName, this.offset);
            }
        }

        public override void Leave(Action _action, Track _track)
        {
            if ((this.actorObj != 0) && (this.actorObj.handle.PetControl != null))
            {
                this.actorObj.handle.PetControl.DestoryPet(this.petType);
            }
            base.Leave(_action, _track);
        }

        public override void OnUse()
        {
            base.OnUse();
            this.targetId = 0;
            this.prefabName = string.Empty;
            this.offset = Vector3.get_zero();
            this.actorObj.Release();
        }

        public override void Process(Action _action, Track _track, int _localTime)
        {
            base.Process(_action, _track, _localTime);
        }

        public override bool SupportEditMode()
        {
            return true;
        }
    }
}

