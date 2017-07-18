namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameSystem;
    using System;

    [EventCategory("MMGame/Skill")]
    public class MiniMapTrackDuration : DurationEvent
    {
        private PoolObjHandle<ActorRoot> actorObj;
        private uint actorObjID;
        public string iconPath = string.Empty;
        [ObjectTemplate(new Type[] {  })]
        public int targetId;

        public override BaseEvent Clone()
        {
            MiniMapTrackDuration duration = ClassObjPool<MiniMapTrackDuration>.Get();
            duration.CopyData(this);
            return duration;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            MiniMapTrackDuration duration = src as MiniMapTrackDuration;
            this.targetId = duration.targetId;
            this.iconPath = duration.iconPath;
        }

        public override void Enter(Action _action, Track _track)
        {
            base.Enter(_action, _track);
            this.actorObj = _action.GetActorHandle(this.targetId);
            if (this.actorObj != 0)
            {
                this.actorObjID = this.actorObj.handle.ObjID;
                MiniMapTrack_3DUI.Prepare(this.actorObj, this.iconPath);
            }
        }

        public override void Leave(Action _action, Track _track)
        {
            base.Leave(_action, _track);
            MiniMapTrack_3DUI.Recyle(this.actorObjID);
        }

        public override void OnUse()
        {
            base.OnUse();
            this.targetId = 0;
            this.actorObj.Release();
        }

        public override void Process(Action _action, Track _track, int _localTime)
        {
            base.Process(_action, _track, _localTime);
            if (this.actorObj != 0)
            {
                MiniMapTrack_3DUI.SetTrackPosition(this.actorObj, this.iconPath);
            }
        }

        public override bool SupportEditMode()
        {
            return true;
        }
    }
}

