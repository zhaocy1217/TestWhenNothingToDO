namespace Assets.Scripts.GameLogic
{
    using AGE;
    using Assets.Scripts.Common;
    using ResData;
    using System;
    using UnityEngine;

    public abstract class BaseSkill : PooledClassObject
    {
        public string ActionName = string.Empty;
        public bool bAgeImmeExcute;
        protected PoolObjHandle<Action> curAction = new PoolObjHandle<Action>();
        private ActionStopDelegate OnActionStopDelegate;
        public SkillUseContext skillContext = new SkillUseContext();
        public int SkillID;

        protected BaseSkill()
        {
        }

        public SkillUseContext GetSkillUseContext()
        {
            if (this.curAction == 0)
            {
                return null;
            }
            return this.curAction.handle.refParams.GetRefParamObject<SkillUseContext>("SkillContext");
        }

        public PoolObjHandle<ActorRoot> GetTargetActor()
        {
            SkillUseContext skillUseContext = this.GetSkillUseContext();
            if (skillUseContext == null)
            {
                return new PoolObjHandle<ActorRoot>(null);
            }
            return skillUseContext.TargetActor;
        }

        public virtual void OnActionStoped(ref PoolObjHandle<Action> action)
        {
            action.handle.onActionStop -= this.OnActionStopDelegate;
            if ((this.curAction != 0) && (action == this.curAction))
            {
                this.curAction.Release();
            }
        }

        public override void OnRelease()
        {
            this.SkillID = 0;
            this.ActionName = string.Empty;
            this.curAction.Release();
            this.OnActionStopDelegate = null;
            base.OnRelease();
        }

        public override void OnUse()
        {
            base.OnUse();
            this.SkillID = 0;
            this.ActionName = string.Empty;
            this.curAction.Release();
            this.skillContext.Reset();
            this.bAgeImmeExcute = false;
            this.OnActionStopDelegate = new ActionStopDelegate(this.OnActionStoped);
        }

        public virtual void Stop()
        {
            if (this.curAction != 0)
            {
                this.curAction.handle.Stop(false);
                this.curAction.Release();
            }
        }

        public virtual bool Use(PoolObjHandle<ActorRoot> user)
        {
            return ((((user != 0) && (this.skillContext != null)) && !string.IsNullOrEmpty(this.ActionName)) && this.UseImpl(user));
        }

        public virtual bool Use(PoolObjHandle<ActorRoot> user, ref SkillUseParam param)
        {
            if (((user == 0) || (this.skillContext == null)) || string.IsNullOrEmpty(this.ActionName))
            {
                return false;
            }
            this.skillContext.Copy(ref param);
            return this.UseImpl(user);
        }

        private bool UseImpl(PoolObjHandle<ActorRoot> user)
        {
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            VInt3 forward = VInt3.forward;
            switch (this.skillContext.AppointType)
            {
                case SkillRangeAppointType.Auto:
                case SkillRangeAppointType.Target:
                    flag = true;
                    break;

                case SkillRangeAppointType.Pos:
                    flag2 = true;
                    break;

                case SkillRangeAppointType.Directional:
                    flag3 = true;
                    forward = this.skillContext.UseVector;
                    if (this.skillContext.TargetID != 0)
                    {
                        PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(this.skillContext.TargetID);
                        if (actor != 0)
                        {
                            Vector3 vector = actor.handle.myTransform.get_position() - user.handle.myTransform.get_position();
                            vector.y = 0f;
                            vector.Normalize();
                            forward = vector;
                        }
                    }
                    break;

                case SkillRangeAppointType.Track:
                    flag2 = true;
                    flag3 = true;
                    forward = this.skillContext.EndVector - this.skillContext.UseVector;
                    if (forward.sqrMagnitudeLong < 1L)
                    {
                        forward = VInt3.forward;
                    }
                    break;
            }
            if (flag && (this.skillContext.TargetActor == 0))
            {
                return false;
            }
            if (flag)
            {
                GameObject[] objArray1 = new GameObject[] { user.handle.gameObject, this.skillContext.TargetActor.handle.gameObject };
                this.curAction = new PoolObjHandle<Action>(ActionManager.Instance.PlayAction(this.ActionName, true, false, objArray1));
            }
            else
            {
                GameObject[] objArray2 = new GameObject[] { user.handle.gameObject };
                this.curAction = new PoolObjHandle<Action>(ActionManager.Instance.PlayAction(this.ActionName, true, false, objArray2));
            }
            if (this.curAction == 0)
            {
                return false;
            }
            this.curAction.handle.onActionStop += this.OnActionStopDelegate;
            this.curAction.handle.refParams.AddRefParam("SkillObj", this);
            this.curAction.handle.refParams.AddRefParam("SkillContext", this.skillContext);
            if (flag)
            {
                this.curAction.handle.refParams.AddRefParam("TargetActor", this.skillContext.TargetActor);
            }
            if (flag2)
            {
                this.curAction.handle.refParams.SetRefParam("_TargetPos", this.skillContext.UseVector);
            }
            if (flag3)
            {
                this.curAction.handle.refParams.SetRefParam("_TargetDir", forward);
            }
            this.curAction.handle.refParams.SetRefParam("_BulletPos", this.skillContext.BulletPos);
            this.curAction.handle.refParams.SetRefParam("_BulletUseDir", user.handle.forward);
            if (this.bAgeImmeExcute)
            {
                this.curAction.handle.UpdateLogic((int) Singleton<FrameSynchr>.GetInstance().FrameDelta);
            }
            return true;
        }

        public PoolObjHandle<Action> CurAction
        {
            get
            {
                return this.curAction;
            }
        }

        public virtual bool isBuff
        {
            get
            {
                return false;
            }
        }

        public virtual bool isBullet
        {
            get
            {
                return false;
            }
        }

        public bool isFinish
        {
            get
            {
                return (this.curAction == 0);
            }
        }
    }
}

