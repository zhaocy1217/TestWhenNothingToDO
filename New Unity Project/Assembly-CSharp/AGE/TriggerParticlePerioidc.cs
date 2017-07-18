namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [EventCategory("Effect")]
    public class TriggerParticlePerioidc : TriggerParticle
    {
        public bool bAutoDestruct = true;
        [AssetReference(AssetRefType.Particle)]
        public string FinalEffectName;
        [AssetReference(AssetRefType.Particle)]
        public string InitialEffectName;
        private int intervalTimer;
        private int lastTime;
        private List<GameObject> NonAutoDestructParList = new List<GameObject>();
        [AssetReference(AssetRefType.Particle)]
        public string PeriodicEffectName;
        public int PeriodicInterval = 0x3e8;

        public override BaseEvent Clone()
        {
            TriggerParticlePerioidc perioidc = ClassObjPool<TriggerParticlePerioidc>.Get();
            perioidc.CopyData(this);
            return perioidc;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            TriggerParticlePerioidc perioidc = src as TriggerParticlePerioidc;
            this.InitialEffectName = perioidc.InitialEffectName;
            this.PeriodicEffectName = perioidc.PeriodicEffectName;
            this.FinalEffectName = perioidc.FinalEffectName;
            this.bAutoDestruct = perioidc.bAutoDestruct;
            this.PeriodicInterval = perioidc.PeriodicInterval;
            this.intervalTimer = perioidc.intervalTimer;
            this.lastTime = perioidc.lastTime;
            this.NonAutoDestructParList = perioidc.NonAutoDestructParList;
        }

        private void DestroyParObj(Action _action, GameObject inParObj)
        {
            if (inParObj != null)
            {
                inParObj.get_transform().SetParent(null);
                ActionManager.DestroyGameObjectFromAction(_action, inParObj);
                if (base.applyActionSpeedToParticle)
                {
                    _action.RemoveTempObject(Action.PlaySpeedAffectedType.ePSAT_Fx, inParObj);
                }
            }
        }

        public override void Enter(Action _action, Track _track)
        {
            base.resourceName = this.InitialEffectName;
            base.Enter(_action, _track);
            if (this.PeriodicInterval > 0)
            {
                this.lastTime = 0;
                this.intervalTimer = 0;
            }
        }

        private GameObject InstParObj(string prefabName, Action _action, bool bCheckParLife)
        {
            Vector3 bindPosOffset = base.bindPosOffset;
            Quaternion bindRotOffset = base.bindRotOffset;
            GameObject gameObject = _action.GetGameObject(base.targetId);
            GameObject obj3 = _action.GetGameObject(base.objectSpaceId);
            Transform transform = null;
            Transform transform2 = null;
            if (base.bindPointName.Length == 0)
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
                Transform transform3 = null;
                if (gameObject != null)
                {
                    transform3 = SubObject.FindSubObject(gameObject, base.bindPointName).get_transform();
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
                    transform3 = SubObject.FindSubObject(obj3, base.bindPointName).get_transform();
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
            if (transform != null)
            {
                bindPosOffset = transform.get_localToWorldMatrix().MultiplyPoint(base.bindPosOffset);
                bindRotOffset = transform.get_rotation() * base.bindRotOffset;
            }
            else if (transform2 != null)
            {
                bindPosOffset = transform2.get_localToWorldMatrix().MultiplyPoint(base.bindPosOffset);
                bindRotOffset = transform2.get_rotation() * base.bindRotOffset;
            }
            if ((transform2 != null) && (transform2.get_gameObject().get_layer() == LayerMask.NameToLayer("Hide")))
            {
                return null;
            }
            bool isInit = false;
            GameObject item = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD(prefabName, true, SceneObjType.ActionRes, bindPosOffset, bindRotOffset, out isInit);
            if (item == null)
            {
                return null;
            }
            if (transform != null)
            {
                PoolObjHandle<ActorRoot> handle = (transform.get_gameObject() != gameObject) ? ActorHelper.GetActorRoot(transform.get_gameObject()) : _action.GetActorHandle(base.targetId);
                if ((handle != 0) && (handle.handle.ActorMesh != null))
                {
                    item.get_transform().SetParent(handle.handle.ActorMesh.get_transform());
                }
                else
                {
                    item.get_transform().SetParent(transform);
                }
            }
            string layerName = "Particles";
            if ((transform != null) && (transform.get_gameObject().get_layer() == LayerMask.NameToLayer("Hide")))
            {
                layerName = "Hide";
            }
            base.particleObject.SetLayer(layerName, false);
            if (!bCheckParLife)
            {
                this.NonAutoDestructParList.Add(item);
            }
            if (isInit)
            {
                ParticleHelper.Init(base.particleObject, base.scaling);
            }
            if (base.applyActionSpeedToParticle)
            {
                _action.AddTempObject(Action.PlaySpeedAffectedType.ePSAT_Fx, item);
            }
            return item;
        }

        public override void Leave(Action _action, Track _track)
        {
            base.Leave(_action, _track);
            if (this.FinalEffectName.Length > 0)
            {
                this.InstParObj(this.FinalEffectName, _action, true);
            }
            List<GameObject>.Enumerator enumerator = this.NonAutoDestructParList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                this.DestroyParObj(_action, enumerator.Current);
            }
            this.NonAutoDestructParList.Clear();
        }

        public override void OnUse()
        {
            base.OnUse();
            this.InitialEffectName = string.Empty;
            this.PeriodicEffectName = string.Empty;
            this.FinalEffectName = string.Empty;
            this.bAutoDestruct = true;
            this.PeriodicInterval = 0x3e8;
            this.intervalTimer = 0;
            this.lastTime = 0;
            this.NonAutoDestructParList.Clear();
        }

        public override void Process(Action _action, Track _track, int _localTime)
        {
            if (this.PeriodicInterval > 0)
            {
                int num = _localTime - this.lastTime;
                this.lastTime = _localTime;
                this.intervalTimer += num;
                if (this.intervalTimer >= this.PeriodicInterval)
                {
                    this.intervalTimer = 0;
                    if (this.PeriodicEffectName.Length > 0)
                    {
                        this.InstParObj(this.PeriodicEffectName, _action, this.bAutoDestruct);
                    }
                }
            }
        }
    }
}

