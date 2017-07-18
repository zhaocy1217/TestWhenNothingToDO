namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using System;
    using UnityEngine;

    public abstract class BasePet
    {
        protected PoolObjHandle<ActorRoot> actorPtr;
        protected Animation animSet;
        private bool bActive;
        private string curAnimName;
        protected PetState curState;
        protected int deltaTime;
        protected GameObject meshObj;
        protected Vector3 moveDir;
        protected float moveSpeed;
        protected Vector3 offset;
        protected float offsetDistance;
        protected GameObject parentObj;
        protected Transform parentTrans;
        protected Transform petTrans;

        protected BasePet()
        {
        }

        protected bool CheckUpdate()
        {
            return (this.bActive && (this.meshObj != null));
        }

        public virtual void Create(string _prefabName, Vector3 _offset)
        {
            if (((this.actorPtr != 0) && (this.parentObj != null)) && (this.parentObj.get_transform() != null))
            {
                this.offset = _offset;
                this.offsetDistance = Vector3.Distance(new Vector3(0f, 0f, 0f), this.offset);
                this.deltaTime = 0;
                Vector3 pos = this.parentObj.get_transform().get_localToWorldMatrix().MultiplyPoint(this.offset);
                this.meshObj = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD(_prefabName, false, SceneObjType.ActionRes, pos, this.parentObj.get_transform().get_rotation());
                if (this.meshObj != null)
                {
                    this.animSet = this.meshObj.GetComponent<Animation>();
                    if (this.animSet != null)
                    {
                        this.animSet.Play("Idle");
                    }
                    this.petTrans = this.meshObj.get_transform();
                    this.parentTrans = this.parentObj.get_transform();
                    this.bActive = true;
                    this.curState = PetState.Idle;
                }
                else
                {
                    this.bActive = false;
                }
            }
        }

        public virtual void Destory()
        {
            if (this.bActive && (this.meshObj != null))
            {
                Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(this.meshObj);
                this.meshObj = null;
                this.bActive = false;
            }
        }

        public override bool Equals(object obj)
        {
            return (((obj != null) && (base.GetType() == obj.GetType())) && this.IsEquals((BasePet) obj));
        }

        public virtual void Init(ref PoolObjHandle<ActorRoot> _actor)
        {
            this.actorPtr = _actor;
            if (this.actorPtr != 0)
            {
                this.parentObj = this.actorPtr.handle.gameObject;
            }
        }

        private bool IsEquals(BasePet rhs)
        {
            return ((((((this.bActive == rhs.bActive) && (this.curAnimName == rhs.curAnimName)) && ((this.deltaTime == rhs.deltaTime) && (this.moveDir == rhs.moveDir))) && (((this.moveSpeed == rhs.moveSpeed) && (this.curState == rhs.curState)) && ((this.offsetDistance == rhs.offsetDistance) && (this.offset == rhs.offset)))) && (((this.meshObj == rhs.meshObj) && (this.animSet == rhs.animSet)) && (this.parentObj == rhs.parentObj))) && (this.actorPtr == rhs.actorPtr));
        }

        public virtual void LateUpdate(int nDelta)
        {
        }

        protected Quaternion ObjRotationLerp(Vector3 _moveDir, int nDelta)
        {
            return Quaternion.RotateTowards(this.meshObj.get_transform().get_rotation(), Quaternion.LookRotation(_moveDir), (float) nDelta);
        }

        public virtual void OnUse()
        {
            this.bActive = false;
            this.curAnimName = null;
            this.deltaTime = 0;
            this.moveDir = Vector3.get_zero();
            this.moveSpeed = 0f;
            this.curState = PetState.Idle;
            this.offset = Vector3.get_zero();
            this.offsetDistance = 0f;
            this.meshObj = null;
            this.animSet = null;
            this.parentObj = null;
            this.actorPtr.Release();
        }

        protected void PlayAnimation(string animName, float blendTime)
        {
            if (this.curAnimName != animName)
            {
                AnimationState state = this.animSet.CrossFadeQueued(animName, blendTime, 2);
                this.curAnimName = animName;
            }
        }
    }
}

