namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using System;
    using UnityEngine;

    [EventCategory("MMGame/Skill")]
    public class SetCameraHeightDuration : DurationCondition
    {
        public bool cutBackOnExit = true;
        public float heightRate = 1f;
        private bool setFinished;
        public int slerpTick = 500;

        public override bool Check(Action _action, Track _track)
        {
            return this.setFinished;
        }

        public override BaseEvent Clone()
        {
            SetCameraHeightDuration duration = ClassObjPool<SetCameraHeightDuration>.Get();
            duration.CopyData(this);
            return duration;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            SetCameraHeightDuration duration = src as SetCameraHeightDuration;
            this.slerpTick = duration.slerpTick;
            this.cutBackOnExit = duration.cutBackOnExit;
            this.setFinished = duration.setFinished;
            this.heightRate = duration.heightRate;
        }

        public override void Enter(Action _action, Track _track)
        {
            if (ActorHelper.IsHostCtrlActor(ref _action.refParams.GetRefParamObject<SkillUseContext>("SkillContext").Originator))
            {
                this.setFinished = false;
            }
            else
            {
                this.setFinished = true;
            }
            base.Enter(_action, _track);
        }

        public override void Leave(Action _action, Track _track)
        {
            if (this.cutBackOnExit)
            {
                MonoSingleton<CameraSystem>.instance.ZoomRateFromAge = 1f;
            }
            this.setFinished = true;
            base.Leave(_action, _track);
        }

        public override void OnUse()
        {
            base.OnUse();
            this.slerpTick = 500;
            this.cutBackOnExit = true;
            this.setFinished = false;
            this.heightRate = 1f;
        }

        public override void Process(Action _action, Track _track, int _localTime)
        {
            if (!this.setFinished)
            {
                if (_localTime >= this.slerpTick)
                {
                    MonoSingleton<CameraSystem>.instance.ZoomRateFromAge = this.heightRate;
                    this.setFinished = true;
                }
                else
                {
                    float num = Mathf.Lerp(1f, this.heightRate, ((float) _localTime) / ((float) this.slerpTick));
                    MonoSingleton<CameraSystem>.instance.ZoomRateFromAge = num;
                }
                base.Process(_action, _track, _localTime);
            }
        }

        public override bool SupportEditMode()
        {
            return true;
        }
    }
}

