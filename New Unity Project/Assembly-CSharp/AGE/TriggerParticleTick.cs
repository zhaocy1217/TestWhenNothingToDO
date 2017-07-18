namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using System;
    using UnityEngine;

    [EventCategory("Effect")]
    public class TriggerParticleTick : TickEvent
    {
        public bool applyActionSpeedToParticle = true;
        public bool bBullerPosDir;
        public bool bBulletDir;
        public bool bBulletPos;
        public bool bEnableOptCull = true;
        [SubObject]
        public string bindPointName = string.Empty;
        public Vector3 bindPosOffset = new Vector3(0f, 0f, 0f);
        public Quaternion bindRotOffset = new Quaternion(0f, 0f, 0f, 1f);
        public bool bUseAttachBulletShape;
        public bool bUseSkin;
        public bool bUseSkinAdvance;
        public bool enableLayer;
        public bool enableMaxLimit;
        public bool enableTag;
        public int extend = 10;
        public int layer;
        public float lifeTime = 2f;
        public int LimitType = -1;
        public int MaxLimit = 10;
        [ObjectTemplate(new Type[] {  })]
        public int objectSpaceId = -1;
        private GameObject particleObject;
        [AssetReference(AssetRefType.Particle)]
        public string resourceName = string.Empty;
        public Vector3 scaling = new Vector3(1f, 1f, 1f);
        public string tag = string.Empty;
        [ObjectTemplate(new Type[] {  })]
        public int targetId;
        [ObjectTemplate(new Type[] {  })]
        public int VirtualAttachBulletId = -1;

        public override BaseEvent Clone()
        {
            TriggerParticleTick tick = ClassObjPool<TriggerParticleTick>.Get();
            tick.CopyData(this);
            return tick;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            TriggerParticleTick tick = src as TriggerParticleTick;
            this.targetId = tick.targetId;
            this.objectSpaceId = tick.objectSpaceId;
            this.VirtualAttachBulletId = tick.VirtualAttachBulletId;
            this.resourceName = tick.resourceName;
            this.lifeTime = tick.lifeTime;
            this.bindPointName = tick.bindPointName;
            this.bindPosOffset = tick.bindPosOffset;
            this.bindRotOffset = tick.bindRotOffset;
            this.scaling = tick.scaling;
            this.bEnableOptCull = tick.bEnableOptCull;
            this.bBulletPos = tick.bBulletPos;
            this.bBulletDir = tick.bBulletDir;
            this.bBullerPosDir = tick.bBullerPosDir;
            this.enableLayer = tick.enableLayer;
            this.layer = tick.layer;
            this.enableTag = tick.enableTag;
            this.tag = tick.tag;
            this.enableMaxLimit = tick.enableMaxLimit;
            this.MaxLimit = tick.MaxLimit;
            this.LimitType = tick.LimitType;
            this.applyActionSpeedToParticle = tick.applyActionSpeedToParticle;
            this.particleObject = tick.particleObject;
            this.extend = tick.extend;
            this.bUseSkin = tick.bUseSkin;
            this.bUseSkinAdvance = tick.bUseSkinAdvance;
            this.bUseAttachBulletShape = tick.bUseAttachBulletShape;
        }

        public static void OnRecycleTickObj(GameObject obj)
        {
            ParticleHelper.DecParticleActiveNumber();
            if (FogOfWar.enable)
            {
                Singleton<GameFowManager>.instance.m_collector.RemoveVirtualParentParticle(obj);
            }
        }

        public override void OnUse()
        {
            base.OnUse();
            this.targetId = 0;
            this.objectSpaceId = -1;
            this.VirtualAttachBulletId = -1;
            this.lifeTime = 5f;
            this.resourceName = string.Empty;
            this.bindPointName = string.Empty;
            this.bindPosOffset = new Vector3(0f, 0f, 0f);
            this.bindRotOffset = new Quaternion(0f, 0f, 0f, 1f);
            this.scaling = new Vector3(1f, 1f, 1f);
            this.bEnableOptCull = true;
            this.bBulletPos = false;
            this.bBulletDir = false;
            this.bBullerPosDir = false;
            this.enableLayer = false;
            this.layer = 0;
            this.enableTag = false;
            this.tag = string.Empty;
            this.enableMaxLimit = false;
            this.MaxLimit = 10;
            this.LimitType = -1;
            this.applyActionSpeedToParticle = true;
            this.particleObject = null;
            this.extend = 10;
            this.bUseSkinAdvance = false;
            this.bUseAttachBulletShape = false;
        }

        public override void Process(Action _action, Track _track)
        {
            if (!MonoSingleton<Reconnection>.GetInstance().isProcessingRelayRecover)
            {
                SkillUseContext refParamObject = null;
                Vector3 bindPosOffset = this.bindPosOffset;
                Quaternion bindRotOffset = this.bindRotOffset;
                GameObject gameObject = _action.GetGameObject(this.targetId);
                GameObject obj3 = _action.GetGameObject(this.objectSpaceId);
                Transform transform = null;
                Transform transform2 = null;
                if (this.bindPointName.Length == 0)
                {
                    if (gameObject != null)
                    {
                        transform = gameObject.get_transform();
                    }
                    else if (obj3 != null)
                    {
                        transform2 = obj3.get_transform();
                    }
                }
                else
                {
                    GameObject obj4 = null;
                    Transform transform3 = null;
                    if (gameObject != null)
                    {
                        obj4 = SubObject.FindSubObject(gameObject, this.bindPointName);
                        if (obj4 != null)
                        {
                            transform3 = obj4.get_transform();
                        }
                        if (transform3 != null)
                        {
                            transform = transform3;
                        }
                        else if (gameObject != null)
                        {
                            transform = gameObject.get_transform();
                        }
                    }
                    else if (obj3 != null)
                    {
                        obj4 = SubObject.FindSubObject(obj3, this.bindPointName);
                        if (obj4 != null)
                        {
                            transform3 = obj4.get_transform();
                        }
                        if (transform3 != null)
                        {
                            transform2 = transform3;
                        }
                        else if (gameObject != null)
                        {
                            transform2 = obj3.get_transform();
                        }
                    }
                }
                if (this.bBulletPos)
                {
                    VInt3 zero = VInt3.zero;
                    _action.refParams.GetRefParam("_BulletPos", ref zero);
                    bindPosOffset = (Vector3) zero;
                    bindRotOffset = Quaternion.get_identity();
                    if (this.bBulletDir)
                    {
                        VInt3 num2 = VInt3.zero;
                        if (_action.refParams.GetRefParam("_BulletDir", ref num2))
                        {
                            bindRotOffset = Quaternion.LookRotation((Vector3) num2);
                        }
                    }
                }
                else if (transform != null)
                {
                    bindPosOffset = transform.get_localToWorldMatrix().MultiplyPoint(this.bindPosOffset);
                    bindRotOffset = transform.get_rotation() * this.bindRotOffset;
                }
                else if (transform2 != null)
                {
                    if (obj3 != null)
                    {
                        PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.objectSpaceId);
                        if (actorHandle != 0)
                        {
                            bindPosOffset = (Vector3) IntMath.Transform((VInt3) this.bindPosOffset, actorHandle.handle.forward, actorHandle.handle.location);
                            bindRotOffset = Quaternion.LookRotation((Vector3) actorHandle.handle.forward) * this.bindRotOffset;
                        }
                    }
                    else
                    {
                        bindPosOffset = transform2.get_localToWorldMatrix().MultiplyPoint(this.bindPosOffset);
                        bindRotOffset = transform2.get_rotation() * this.bindRotOffset;
                    }
                    if (this.bBulletDir)
                    {
                        VInt3 num3 = VInt3.zero;
                        if (_action.refParams.GetRefParam("_BulletDir", ref num3))
                        {
                            bindRotOffset = Quaternion.LookRotation((Vector3) num3) * this.bindRotOffset;
                        }
                    }
                    else if (this.bBullerPosDir)
                    {
                        refParamObject = _action.refParams.GetRefParamObject<SkillUseContext>("SkillContext");
                        if (refParamObject != null)
                        {
                            PoolObjHandle<ActorRoot> originator = refParamObject.Originator;
                            if (originator != 0)
                            {
                                Vector3 location = transform2.get_position();
                                if (obj3 != null)
                                {
                                    PoolObjHandle<ActorRoot> handle3 = _action.GetActorHandle(this.objectSpaceId);
                                    if (handle3 != 0)
                                    {
                                        location = (Vector3) handle3.handle.location;
                                    }
                                }
                                Vector3 vector3 = location - ((Vector3) originator.handle.location);
                                bindRotOffset = Quaternion.LookRotation(vector3) * this.bindRotOffset;
                            }
                        }
                    }
                }
                if (((!this.bEnableOptCull || (transform2 == null)) || ((transform2.get_gameObject().get_layer() != LayerMask.NameToLayer("Hide")) || FogOfWar.enable)) && ((!this.bEnableOptCull || !MonoSingleton<GlobalConfig>.instance.bEnableParticleCullOptimize) || MonoSingleton<CameraSystem>.instance.CheckVisiblity(new Bounds(bindPosOffset, new Vector3((float) this.extend, (float) this.extend, (float) this.extend)))))
                {
                    string resourceName;
                    bool isInit = false;
                    if (this.bUseSkin)
                    {
                        resourceName = SkinResourceHelper.GetResourceName(_action, this.resourceName, this.bUseSkinAdvance);
                    }
                    else
                    {
                        resourceName = this.resourceName;
                    }
                    if (refParamObject == null)
                    {
                        refParamObject = _action.refParams.GetRefParamObject<SkillUseContext>("SkillContext");
                    }
                    bool flag2 = true;
                    int particleLOD = GameSettings.ParticleLOD;
                    if (GameSettings.DynamicParticleLOD)
                    {
                        if (((refParamObject != null) && (refParamObject.Originator != 0)) && (refParamObject.Originator.handle.TheActorMeta.PlayerId == Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer().PlayerId))
                        {
                            flag2 = false;
                        }
                        if (!flag2 && (particleLOD > 1))
                        {
                            GameSettings.ParticleLOD = 1;
                        }
                        MonoSingleton<SceneMgr>.GetInstance().m_dynamicLOD = flag2;
                    }
                    this.particleObject = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD(resourceName, true, SceneObjType.ActionRes, bindPosOffset, bindRotOffset, out isInit);
                    if (GameSettings.DynamicParticleLOD)
                    {
                        MonoSingleton<SceneMgr>.GetInstance().m_dynamicLOD = false;
                    }
                    if (this.particleObject == null)
                    {
                        if (GameSettings.DynamicParticleLOD)
                        {
                            MonoSingleton<SceneMgr>.GetInstance().m_dynamicLOD = flag2;
                        }
                        this.particleObject = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD(this.resourceName, true, SceneObjType.ActionRes, bindPosOffset, bindRotOffset, out isInit);
                        if (GameSettings.DynamicParticleLOD)
                        {
                            MonoSingleton<SceneMgr>.GetInstance().m_dynamicLOD = false;
                        }
                        if (this.particleObject == null)
                        {
                            if (GameSettings.DynamicParticleLOD)
                            {
                                GameSettings.ParticleLOD = particleLOD;
                            }
                            return;
                        }
                    }
                    if (GameSettings.DynamicParticleLOD)
                    {
                        GameSettings.ParticleLOD = particleLOD;
                    }
                    if (this.particleObject != null)
                    {
                        ParticleHelper.IncParticleActiveNumber();
                        if (transform != null)
                        {
                            PoolObjHandle<ActorRoot> handle4 = (transform.get_gameObject() != gameObject) ? ActorHelper.GetActorRoot(transform.get_gameObject()) : _action.GetActorHandle(this.targetId);
                            if ((handle4 != 0) && (handle4.handle.ActorMesh != null))
                            {
                                this.particleObject.get_transform().set_parent(handle4.handle.ActorMesh.get_transform());
                            }
                            else
                            {
                                this.particleObject.get_transform().set_parent(transform.get_parent());
                                if (((handle4 != 0) && (handle4.handle.TheActorMeta.ActorCamp != Singleton<GamePlayerCenter>.instance.GetHostPlayer().PlayerCamp)) && FogOfWar.enable)
                                {
                                    if (handle4.handle.HorizonMarker != null)
                                    {
                                        handle4.handle.HorizonMarker.AddSubParObj(this.particleObject);
                                    }
                                    else
                                    {
                                        BulletWrapper actorControl = handle4.handle.ActorControl as BulletWrapper;
                                        if (actorControl != null)
                                        {
                                            actorControl.AddSubParObj(this.particleObject);
                                        }
                                    }
                                }
                            }
                        }
                        string layerName = "Particles";
                        if ((transform != null) && (transform.get_gameObject().get_layer() == LayerMask.NameToLayer("Hide")))
                        {
                            layerName = "Hide";
                        }
                        if (((transform == null) && (transform2 != null)) && FogOfWar.enable)
                        {
                            PoolObjHandle<ActorRoot> actorRoot = ActorHelper.GetActorRoot(transform2.get_gameObject());
                            if ((actorRoot != 0) && (actorRoot.handle.TheActorMeta.ActorCamp != Singleton<GamePlayerCenter>.instance.GetHostPlayer().PlayerCamp))
                            {
                                if (transform2.get_gameObject().get_layer() == LayerMask.NameToLayer("Hide"))
                                {
                                    layerName = "Hide";
                                }
                                PoolObjHandle<ActorRoot> inAttachActor = _action.GetActorHandle(this.VirtualAttachBulletId);
                                Singleton<GameFowManager>.instance.m_collector.AddVirtualParentParticle(this.particleObject, inAttachActor, this.bUseAttachBulletShape);
                            }
                        }
                        this.particleObject.SetLayer(layerName, false);
                        if (isInit)
                        {
                            ParticleHelper.Init(this.particleObject, this.scaling);
                        }
                        Singleton<CGameObjectPool>.GetInstance().RecycleGameObjectDelay(this.particleObject, Mathf.Max(_action.length, (int) (this.lifeTime * 1000f)), new CGameObjectPool.OnDelayRecycleDelegate(TriggerParticleTick.OnRecycleTickObj));
                        if (this.applyActionSpeedToParticle && (this.particleObject != null))
                        {
                            _action.AddTempObject(Action.PlaySpeedAffectedType.ePSAT_Fx, this.particleObject);
                        }
                    }
                }
            }
        }

        public override bool SupportEditMode()
        {
            return true;
        }
    }
}

