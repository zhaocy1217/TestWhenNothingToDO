namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;

    public class ShenFuSystem : Singleton<ShenFuSystem>
    {
        public Dictionary<int, ShenFuObjects> _shenFuTriggerPool = new Dictionary<int, ShenFuObjects>();
        public List<PoolObjHandle<CTailsman>> m_charmList = new List<PoolObjHandle<CTailsman>>();

        public void AddCharm(PoolObjHandle<CTailsman> inCharm)
        {
            if (inCharm != 0)
            {
                this.m_charmList.Add(inCharm);
            }
        }

        public void ClearAll()
        {
            Dictionary<int, ShenFuObjects>.Enumerator enumerator = this._shenFuTriggerPool.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<int, ShenFuObjects> current = enumerator.Current;
                ShenFuObjects objects = current.Value;
                if (objects.ShenFu != null)
                {
                    Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(objects.ShenFu);
                }
            }
            this._shenFuTriggerPool.Clear();
            while (this.m_charmList.Count > 0)
            {
                PoolObjHandle<CTailsman> handle = this.m_charmList[0];
                handle.handle.DoClearing();
            }
        }

        public override void Init()
        {
        }

        public void OnShenFuEffect(PoolObjHandle<ActorRoot> actor, uint shenFuId, AreaEventTrigger trigger, TriggerActionShenFu shenFu)
        {
            ShenFuObjects objects;
            if (this._shenFuTriggerPool.TryGetValue(trigger.ID, out objects))
            {
                if (objects.ShenFu != null)
                {
                    Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(objects.ShenFu);
                }
                this._shenFuTriggerPool.Remove(trigger.ID);
            }
            ShenFuInfo dataByKey = GameDataMgr.shenfuBin.GetDataByKey(shenFuId);
            if (dataByKey != null)
            {
                BufConsumer consumer = new BufConsumer(dataByKey.iBufId, actor, actor);
                if (consumer.Use())
                {
                }
            }
        }

        public void OnShenfuHalt(uint shenFuId, AreaEventTrigger trigger, TriggerActionShenFu shenFu)
        {
        }

        public void OnShenfuStart(uint shenFuId, AreaEventTrigger trigger, TriggerActionShenFu shenFu)
        {
            if ((trigger != null) && (shenFu != null))
            {
                ShenFuObjects objects = new ShenFuObjects();
                ShenFuInfo dataByKey = GameDataMgr.shenfuBin.GetDataByKey(shenFuId);
                if (dataByKey != null)
                {
                    trigger.Radius = (int) dataByKey.dwGetRadius;
                    string prefabName = StringHelper.UTF8BytesToString(ref dataByKey.szShenFuResPath);
                    objects.ShenFu = MonoSingleton<SceneMgr>.instance.InstantiateLOD(prefabName, false, SceneObjType.ActionRes, trigger.get_gameObject().get_transform().get_position());
                    this._shenFuTriggerPool.Add(trigger.ID, objects);
                    if (FogOfWar.enable)
                    {
                        COM_PLAYERCAMP playerCamp = Singleton<GamePlayerCenter>.instance.GetHostPlayer().PlayerCamp;
                        GameFowCollector.SetObjVisibleByFow(objects.ShenFu, Singleton<GameFowManager>.instance, playerCamp);
                    }
                }
            }
        }

        public void OnShenFuStopped(TriggerActionShenFu inAction)
        {
            ShenFuObjects objects;
            if ((inAction != null) && this._shenFuTriggerPool.TryGetValue(inAction.TriggerId, out objects))
            {
                if (objects.ShenFu != null)
                {
                    Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(objects.ShenFu);
                }
                this._shenFuTriggerPool.Remove(inAction.TriggerId);
            }
        }

        public void PreLoadResource(TriggerActionWrapper triggerActionWrapper, ref ActorPreloadTab loadInfo, LoaderHelper loadHelper)
        {
            if (triggerActionWrapper != null)
            {
                ShenFuInfo dataByKey = GameDataMgr.shenfuBin.GetDataByKey((long) triggerActionWrapper.UpdateUniqueId);
                if (dataByKey != null)
                {
                    AssetLoadBase base3 = new AssetLoadBase();
                    base3.assetPath = StringHelper.UTF8BytesToString(ref dataByKey.szShenFuResPath);
                    AssetLoadBase item = base3;
                    loadInfo.mesPrefabs.Add(item);
                    loadHelper.AnalyseSkillCombine(ref loadInfo, dataByKey.iBufId);
                }
            }
        }

        public void RemoveCharm(PoolObjHandle<CTailsman> inCharm)
        {
            if (inCharm != 0)
            {
                this.m_charmList.Remove(inCharm);
            }
        }

        public override void UnInit()
        {
            this.ClearAll();
        }

        public void UpdateLogic(int inDelta)
        {
            if (this.m_charmList.Count > 0)
            {
                for (int i = this.m_charmList.Count - 1; i >= 0; i--)
                {
                    PoolObjHandle<CTailsman> handle = this.m_charmList[i];
                    handle.handle.UpdateLogic(inDelta);
                }
            }
        }
    }
}

