namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using ResData;
    using System;

    public class BulletSkill : BaseSkill
    {
        private bool bDeadRemove;
        public bool bManaged = true;
        private int bulletTypeId;
        public ResBulletCfgInfo cfgData;
        public int lifeTime;

        public void IgnoreUpperLimit()
        {
            this.bulletTypeId = 0;
        }

        public void Init(string _actionName, bool _bDeadRemove, int _bulletTypeId)
        {
            this.bDeadRemove = _bDeadRemove;
            base.ActionName = _actionName;
            this.bulletTypeId = _bulletTypeId;
        }

        public override void OnActionStoped(ref PoolObjHandle<Action> action)
        {
            base.OnActionStoped(ref action);
            if (!this.bManaged)
            {
                base.Release();
            }
        }

        public override void OnRelease()
        {
            this.bManaged = true;
            this.lifeTime = 0;
            this.bDeadRemove = false;
            this.cfgData = null;
            this.bulletTypeId = 0;
            base.OnRelease();
        }

        public override void OnUse()
        {
            base.OnUse();
            this.bManaged = true;
            this.lifeTime = 0;
            this.bDeadRemove = false;
            this.cfgData = null;
            this.bulletTypeId = 0;
        }

        public void UpdateLogic(int nDelta)
        {
            if (this.lifeTime > 0)
            {
                this.lifeTime -= nDelta;
                if (this.lifeTime <= 0)
                {
                    this.lifeTime = 0;
                    this.Stop();
                }
            }
        }

        public bool Use(PoolObjHandle<ActorRoot> user, SkillUseContext context)
        {
            base.skillContext.Copy(context);
            base.skillContext.Instigator = this;
            DebugHelper.Assert((bool) base.skillContext.Originator);
            if (!base.Use(user))
            {
                return false;
            }
            return true;
        }

        public int BulletTypeId
        {
            get
            {
                return this.bulletTypeId;
            }
        }

        public override bool isBullet
        {
            get
            {
                return true;
            }
        }

        public bool IsDeadRemove
        {
            get
            {
                return this.bDeadRemove;
            }
        }
    }
}

