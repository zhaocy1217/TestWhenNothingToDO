namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using System;
    using UnityEngine;

    [EventCategory("Effect")]
    public class TriggerParticle : DurationEvent
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
        public bool bOnlyFollowPos;
        public bool bUseAttachBulletShape;
        public bool bUseSkin;
        public bool bUseSkinAdvance;
        public bool enableLayer;
        public bool enableTag;
        public int extend = 10;
        private Transform followTransform;
        public int iDelayDisappearTime;
        public int layer;
        [ObjectTemplate(new Type[] {  })]
        public int objectSpaceId = -1;
        private Vector3 offsetPosition;
        protected GameObject particleObject;
        private Transform particleTransform;
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
            TriggerParticle particle = ClassObjPool<TriggerParticle>.Get();
            particle.CopyData(this);
            return particle;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            TriggerParticle particle = src as TriggerParticle;
            this.targetId = particle.targetId;
            this.objectSpaceId = particle.objectSpaceId;
            this.VirtualAttachBulletId = particle.VirtualAttachBulletId;
            this.resourceName = particle.resourceName;
            this.bindPointName = particle.bindPointName;
            this.bindPosOffset = particle.bindPosOffset;
            this.bindRotOffset = particle.bindRotOffset;
            this.scaling = particle.scaling;
            this.bEnableOptCull = particle.bEnableOptCull;
            this.bBulletPos = particle.bBulletPos;
            this.bBulletDir = particle.bBulletDir;
            this.bBullerPosDir = particle.bBullerPosDir;
            this.enableLayer = particle.enableLayer;
            this.layer = particle.layer;
            this.enableTag = particle.enableTag;
            this.tag = particle.tag;
            this.applyActionSpeedToParticle = particle.applyActionSpeedToParticle;
            this.particleObject = particle.particleObject;
            this.extend = particle.extend;
            this.bOnlyFollowPos = particle.bOnlyFollowPos;
            this.bUseSkin = particle.bUseSkin;
            this.bUseSkinAdvance = particle.bUseSkinAdvance;
            this.iDelayDisappearTime = particle.iDelayDisappearTime;
            this.bUseAttachBulletShape = particle.bUseAttachBulletShape;
        }

        public override void Enter(Action _action, Track _track)
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
                    PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.targetId);
                    this.followTransform = transform;
                }
                else if (obj3 != null)
                {
                    transform2 = obj3.get_transform();
                }
            }
            else
            {
                GameObject obj4 = null;
                if (gameObject != null)
                {
                    obj4 = SubObject.FindSubObject(gameObject, this.bindPointName);
                    if (obj4 != null)
                    {
                        transform = obj4.get_transform();
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
                        transform2 = obj4.get_transform();
                    }
                    else if (gameObject != null)
                    {
                        transform2 = obj3.get_transform();
                    }
                }
            }
            if ((!this.bEnableOptCull || (transform2 == null)) || ((transform2.get_gameObject().get_layer() != LayerMask.NameToLayer("Hide")) || FogOfWar.enable))
            {
                string resourceName;
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
                        PoolObjHandle<ActorRoot> handle2 = _action.GetActorHandle(this.objectSpaceId);
                        if (handle2 != 0)
                        {
                            bindPosOffset = (Vector3) IntMath.Transform((VInt3) this.bindPosOffset, handle2.handle.forward, handle2.handle.location);
                            bindRotOffset = Quaternion.LookRotation((Vector3) handle2.handle.forward) * this.bindRotOffset;
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
                                    PoolObjHandle<ActorRoot> handle4 = _action.GetActorHandle(this.objectSpaceId);
                                    if (handle4 != 0)
                                    {
                                        location = (Vector3) handle4.handle.location;
                                    }
                                }
                                Vector3 vector3 = location - ((Vector3) originator.handle.location);
                                bindRotOffset = Quaternion.LookRotation(vector3) * this.bindRotOffset;
                            }
                        }
                    }
                }
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
                if (this.particleObject != null)
                {
                    this.particleTransform = this.particleObject.get_transform();
                }
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
                    this.particleTransform = this.particleObject.get_transform();
                }
                if (GameSettings.DynamicParticleLOD)
                {
                    GameSettings.ParticleLOD = particleLOD;
                }
                ParticleHelper.IncParticleActiveNumber();
                if (transform != null)
                {
                    if (!this.bOnlyFollowPos)
                    {
                        PoolObjHandle<ActorRoot> handle5 = (transform.get_gameObject() != gameObject) ? ActorHelper.GetActorRoot(transform.get_gameObject()) : _action.GetActorHandle(this.targetId);
                        this.particleTransform.set_parent(transform);
                    }
                    else
                    {
                        this.offsetPosition = bindPosOffset - transform.get_position();
                    }
                }
                if (isInit)
                {
                    if (this.enableLayer || this.enableTag)
                    {
                        Transform[] transformArray = this.particleObject.GetComponentsInChildren<Transform>();
                        for (int i = 0; i < transformArray.Length; i++)
                        {
                            if (this.enableLayer)
                            {
                                transformArray[i].get_gameObject().set_layer(this.layer);
                            }
                            if (this.enableTag)
                            {
                                transformArray[i].get_gameObject().set_tag(this.tag);
                            }
                        }
                    }
                    ParticleSystem[] componentsInChildren = this.particleObject.GetComponentsInChildren<ParticleSystem>();
                    if (componentsInChildren != null)
                    {
                        for (int j = 0; j < componentsInChildren.Length; j++)
                        {
                            ParticleSystem system1 = componentsInChildren[j];
                            system1.set_startSize(system1.get_startSize() * this.scaling.x);
                            ParticleSystem system2 = componentsInChildren[j];
                            system2.set_startLifetime(system2.get_startLifetime() * this.scaling.y);
                            ParticleSystem system3 = componentsInChildren[j];
                            system3.set_startSpeed(system3.get_startSpeed() * this.scaling.z);
                            Transform transform1 = componentsInChildren[j].get_transform();
                            transform1.set_localScale((Vector3) (transform1.get_localScale() * this.scaling.x));
                        }
                        ParticleSystemPoolComponent cachedComponent = Singleton<CGameObjectPool>.GetInstance().GetCachedComponent<ParticleSystemPoolComponent>(this.particleObject, true);
                        ParticleSystemPoolComponent.ParticleSystemCache[] cacheArray = new ParticleSystemPoolComponent.ParticleSystemCache[componentsInChildren.Length];
                        for (int k = 0; k < cacheArray.Length; k++)
                        {
                            cacheArray[k].par = componentsInChildren[k];
                            cacheArray[k].emmitState = componentsInChildren[k].get_enableEmission();
                        }
                        cachedComponent.cache = cacheArray;
                    }
                }
                else
                {
                    ParticleSystemPoolComponent component2 = Singleton<CGameObjectPool>.GetInstance().GetCachedComponent<ParticleSystemPoolComponent>(this.particleObject, false);
                    if (null != component2)
                    {
                        ParticleSystemPoolComponent.ParticleSystemCache[] cache = component2.cache;
                        if (cache != null)
                        {
                            for (int m = 0; m < cache.Length; m++)
                            {
                                if (cache[m].par.get_enableEmission() != cache[m].emmitState)
                                {
                                    cache[m].par.set_enableEmission(cache[m].emmitState);
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
                if (this.applyActionSpeedToParticle)
                {
                    _action.AddTempObject(Action.PlaySpeedAffectedType.ePSAT_Fx, this.particleObject);
                }
            }
        }

        public override void Leave(Action _action, Track _track)
        {
            if (this.particleObject != null)
            {
                if (this.iDelayDisappearTime > 0)
                {
                    ParticleSystemPoolComponent cachedComponent = Singleton<CGameObjectPool>.GetInstance().GetCachedComponent<ParticleSystemPoolComponent>(this.particleObject, false);
                    if (null != cachedComponent)
                    {
                        ParticleSystemPoolComponent.ParticleSystemCache[] cache = cachedComponent.cache;
                        if (cache != null)
                        {
                            for (int i = 0; i < cache.Length; i++)
                            {
                                cache[i].par.set_enableEmission(false);
                            }
                        }
                        MonoSingleton<SceneMgr>.GetInstance().AddToRoot(this.particleObject, SceneObjType.ActionRes);
                        Singleton<CGameObjectPool>.GetInstance().RecycleGameObjectDelay(this.particleObject, this.iDelayDisappearTime, new CGameObjectPool.OnDelayRecycleDelegate(TriggerParticle.OnDelayRecycleParticleCallback));
                    }
                }
                else
                {
                    if (FogOfWar.enable)
                    {
                        Singleton<GameFowManager>.instance.m_collector.RemoveVirtualParentParticle(this.particleObject);
                    }
                    this.particleTransform.set_position(new Vector3(10000f, 10000f, 10000f));
                    Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(this.particleObject);
                }
                ParticleHelper.DecParticleActiveNumber();
                if (this.applyActionSpeedToParticle)
                {
                    _action.RemoveTempObject(Action.PlaySpeedAffectedType.ePSAT_Fx, this.particleObject);
                }
            }
        }

        private static void OnDelayRecycleParticleCallback(GameObject recycleObj)
        {
            recycleObj.get_transform().set_position(new Vector3(10000f, 10000f, 10000f));
            if (FogOfWar.enable)
            {
                Singleton<GameFowManager>.instance.m_collector.RemoveVirtualParentParticle(recycleObj);
            }
        }

        public override void OnUse()
        {
            base.OnUse();
            this.targetId = 0;
            this.objectSpaceId = -1;
            this.VirtualAttachBulletId = -1;
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
            this.applyActionSpeedToParticle = true;
            this.particleObject = null;
            this.extend = 10;
            this.offsetPosition = Vector3.get_zero();
            this.followTransform = null;
            this.particleTransform = null;
            this.bOnlyFollowPos = false;
            this.bUseSkin = false;
            this.bUseSkinAdvance = false;
            this.iDelayDisappearTime = 0;
            this.bUseAttachBulletShape = false;
        }

        public override void Process(Action _action, Track _track, int _localTime)
        {
            if ((this.bOnlyFollowPos && (this.particleTransform != null)) && ((this.followTransform != null) && (this.particleObject != null)))
            {
                this.particleTransform.set_position(this.followTransform.get_position() + this.offsetPosition);
            }
            base.Process(_action, _track, _localTime);
        }

        public override bool SupportEditMode()
        {
            return true;
        }
    }
}

