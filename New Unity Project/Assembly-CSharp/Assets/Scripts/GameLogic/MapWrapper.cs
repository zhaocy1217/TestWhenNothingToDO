namespace Assets.Scripts.GameLogic
{
    using AGE;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.DataCenter;
    using CSProtocol;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class MapWrapper : MonoBehaviour, IUpdateLogic
    {
        private ListView<DynamicChannel> channelList = new ListView<DynamicChannel>();
        private ListView<CommonSpawnGroup> commonSpawnGroups = new ListView<CommonSpawnGroup>();
        private bool m_bSoldierOverNum;
        public List<ObjTriggerKeyValuePair> objTriggerMultiMap = new List<ObjTriggerKeyValuePair>();
        private ListView<ReviveRegion> reviveAreas = new ListView<ReviveRegion>();
        private int SoldierActivateCountDelay1;
        private int SoldierActivateCountDelay1Seq = -1;
        private int SoldierActivateCountDelay2;
        private int SoldierActivateCountDelay2Seq = -1;
        private int SoldierActivateDelay;
        private int SoldierActivateDelaySeq = -1;
        private ListView<SoldierRegion> soldierAreas = new ListView<SoldierRegion>();
        [FriendlyName("刷兵总数限制下限")]
        public int SoldierOverNumLower;
        [FriendlyName("刷兵总数限制上限")]
        public int SoldierOverNumUpper;
        private ListView<SpawnGroup> spawnGroups = new ListView<SpawnGroup>();
        private ListView<AreaEventTrigger> triggerList = new ListView<AreaEventTrigger>();
        private List<PoolObjHandle<ActorRoot>> TrueMen;
        private ListView<WaypointsHolder> waypointsList = new ListView<WaypointsHolder>();
        private int WelcomeDelaySeq = -1;

        private void Awake()
        {
            Singleton<BattleLogic>.GetInstance().SetupMap(this);
        }

        private void CheckSoldierOverNumLower()
        {
            if ((this.SoldierOverNumUpper > 0) && (this.SoldierOverNumLower >= 0))
            {
                DebugHelper.Assert(this.m_bSoldierOverNum);
                if (this.TrueMen.Count <= this.SoldierOverNumLower)
                {
                    this.m_bSoldierOverNum = false;
                }
            }
        }

        private void CheckSoldierOverNumUpper()
        {
            if ((this.SoldierOverNumUpper > 0) && (this.SoldierOverNumLower >= 0))
            {
                DebugHelper.Assert(!this.m_bSoldierOverNum);
                if (this.TrueMen.Count >= this.SoldierOverNumUpper)
                {
                    this.m_bSoldierOverNum = true;
                }
            }
        }

        private void ClearSoldierActivateTimers()
        {
            this.SoldierActivateDelay = 0;
            this.SoldierActivateCountDelay1 = 0;
            this.SoldierActivateCountDelay2 = 0;
            if (this.SoldierActivateDelaySeq >= 0)
            {
                Singleton<CTimerManager>.instance.RemoveTimer(this.SoldierActivateDelaySeq);
                this.SoldierActivateDelaySeq = -1;
            }
            if (this.SoldierActivateCountDelay1Seq >= 0)
            {
                Singleton<CTimerManager>.instance.RemoveTimer(this.SoldierActivateCountDelay1Seq);
                this.SoldierActivateCountDelay1Seq = -1;
            }
            if (this.SoldierActivateCountDelay2Seq >= 0)
            {
                Singleton<CTimerManager>.instance.RemoveTimer(this.SoldierActivateCountDelay2Seq);
                this.SoldierActivateCountDelay2Seq = -1;
            }
            if (this.WelcomeDelaySeq >= 0)
            {
                Singleton<CTimerManager>.instance.RemoveTimer(this.WelcomeDelaySeq);
                this.WelcomeDelaySeq = -1;
            }
        }

        public bool DoesSoldierOverNum()
        {
            return this.m_bSoldierOverNum;
        }

        public void EnableSoldierRegion(bool bEnable)
        {
            ListView<SoldierRegion>.Enumerator enumerator = this.soldierAreas.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current != null)
                {
                    if (bEnable)
                    {
                        enumerator.Current.Startup();
                    }
                    else
                    {
                        enumerator.Current.Stop();
                    }
                }
            }
        }

        public void EnableSoldierRegion(bool bEnable, SoldierRegion inSoldierRegion)
        {
            if ((inSoldierRegion != null) && this.soldierAreas.Contains(inSoldierRegion))
            {
                if (bEnable)
                {
                    inSoldierRegion.Startup();
                }
                else
                {
                    inSoldierRegion.Stop();
                }
            }
        }

        public void EnableSoldierRegion(bool bEnable, int inWaveId)
        {
            if (inWaveId <= 0)
            {
                this.EnableSoldierRegion(bEnable);
            }
            else
            {
                ListView<SoldierRegion>.Enumerator enumerator = this.soldierAreas.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if ((enumerator.Current != null) && (enumerator.Current.WaveID == inWaveId))
                    {
                        if (bEnable)
                        {
                            enumerator.Current.Startup();
                        }
                        else
                        {
                            enumerator.Current.Stop();
                        }
                    }
                }
            }
        }

        public bool GetRevivePosDir(ref ActorMeta actorMeta, bool bGiveBirth, out VInt3 outPosWorld, out VInt3 outDirWorld)
        {
            outPosWorld = VInt3.zero;
            outDirWorld = VInt3.forward;
            if (Singleton<GamePlayerCenter>.instance.GetPlayer(actorMeta.PlayerId) != null)
            {
                ListView<ReviveRegion>.Enumerator enumerator = this.reviveAreas.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if ((enumerator.Current != null) && (actorMeta.ActorCamp == enumerator.Current.CampType))
                    {
                        int num = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.ServerDataProvider).Fast_GetActorServerDataBornIndex(ref actorMeta);
                        Transform transform = enumerator.Current.get_transform();
                        if (!bGiveBirth)
                        {
                            uint length = (uint) enumerator.Current.SubRevivePlaces.Length;
                            if (!enumerator.Current.OnlyBirth)
                            {
                                length++;
                                int num3 = FrameRandom.Random(length);
                                if ((0 < num3) && (enumerator.Current.SubRevivePlaces[num3 - 1] != null))
                                {
                                    transform = enumerator.Current.SubRevivePlaces[num3 - 1].get_transform();
                                }
                            }
                            else if (length >= 1)
                            {
                                int index = FrameRandom.Random(length);
                                if (enumerator.Current.SubRevivePlaces[index] != null)
                                {
                                    transform = enumerator.Current.SubRevivePlaces[index].get_transform();
                                }
                            }
                        }
                        Transform child = null;
                        if (transform != null)
                        {
                            if (num < transform.get_childCount())
                            {
                                child = transform.GetChild(num);
                            }
                            else
                            {
                                child = transform;
                            }
                        }
                        if (child != null)
                        {
                            outPosWorld = child.get_position();
                            outDirWorld = child.get_forward();
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public SoldierRegion GetSoldierRegion(COM_PLAYERCAMP camp, int routeID)
        {
            ListView<SoldierRegion>.Enumerator enumerator = this.soldierAreas.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SoldierRegion current = enumerator.Current;
                if ((current.CampType == camp) && (current.RouteID == routeID))
                {
                    return current;
                }
            }
            return null;
        }

        public SoldierRegion GetSoldirRegion()
        {
            MapWrapperAdd component = base.GetComponent<MapWrapperAdd>();
            if (component != null)
            {
                return component.CareSoldierRegion;
            }
            return null;
        }

        public ListView<SpawnGroup> GetSpawnGroups()
        {
            return this.spawnGroups;
        }

        public ListView<WaypointsHolder> GetWaypointsList(COM_PLAYERCAMP inCampType)
        {
            ListView<WaypointsHolder> view = new ListView<WaypointsHolder>();
            ListView<WaypointsHolder>.Enumerator enumerator = this.waypointsList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.CampType == inCampType)
                {
                    view.Add(enumerator.Current);
                }
            }
            return view;
        }

        private void OnDestroy()
        {
        }

        private void OnSoldierActivateCountDelay1(int inTimeSeq)
        {
            KillDetailInfo info = new KillDetailInfo();
            info.Type = KillDetailInfoType.Info_Type_Soldier_Activate_Countdown3;
            Singleton<EventRouter>.instance.BroadCastEvent<KillDetailInfo>(EventID.AchievementRecorderEvent, info);
            Singleton<CTimerManager>.instance.RemoveTimer(inTimeSeq);
            this.SoldierActivateCountDelay1Seq = -1;
        }

        private void OnSoldierActivateCountDelay2(int inTimeSeq)
        {
            KillDetailInfo info = new KillDetailInfo();
            info.Type = KillDetailInfoType.Info_Type_Soldier_Activate_Countdown5;
            Singleton<EventRouter>.instance.BroadCastEvent<KillDetailInfo>(EventID.AchievementRecorderEvent, info);
            Singleton<CTimerManager>.instance.RemoveTimer(inTimeSeq);
            this.SoldierActivateCountDelay2Seq = -1;
        }

        private void OnSoldierActivateDelay(int inTimeSeq)
        {
            KillDetailInfo info = new KillDetailInfo();
            info.Type = KillDetailInfoType.Info_Type_Soldier_Activate;
            Singleton<EventRouter>.instance.BroadCastEvent<KillDetailInfo>(EventID.AchievementRecorderEvent, info);
            Singleton<CTimerManager>.instance.RemoveTimer(inTimeSeq);
            this.SoldierActivateDelaySeq = -1;
        }

        private void OnWelcomeDelay(int inTimeSeq)
        {
            KillDetailInfo info = new KillDetailInfo();
            info.Type = KillDetailInfoType.Info_Type_Game_Start_Wel;
            Singleton<EventRouter>.instance.BroadCastEvent<KillDetailInfo>(EventID.AchievementRecorderEvent, info);
            Singleton<CTimerManager>.instance.RemoveTimer(inTimeSeq);
            this.WelcomeDelaySeq = -1;
        }

        public void Reset()
        {
            this.ClearSoldierActivateTimers();
            ListView<SoldierRegion>.Enumerator enumerator = this.soldierAreas.GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.Stop();
            }
            ListView<CommonSpawnGroup>.Enumerator enumerator2 = this.commonSpawnGroups.GetEnumerator();
            while (enumerator2.MoveNext())
            {
                enumerator2.Current.Stop();
            }
            ListView<SpawnGroup>.Enumerator enumerator3 = this.spawnGroups.GetEnumerator();
            while (enumerator3.MoveNext())
            {
                enumerator3.Current.Stop();
            }
            ListView<AreaEventTrigger>.Enumerator enumerator4 = this.triggerList.GetEnumerator();
            while (enumerator4.MoveNext())
            {
                enumerator4.Current.Stop();
            }
        }

        public void ResetSoldierRegion()
        {
            ListView<SoldierRegion>.Enumerator enumerator = this.soldierAreas.GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.ResetRegion();
            }
        }

        private void Start()
        {
            foreach (FuncRegion region in base.GetComponentsInChildren<FuncRegion>(true))
            {
                if (((region != null) && (region.get_enabled() && region.get_gameObject().get_activeInHierarchy())) && region.get_gameObject().get_activeSelf())
                {
                    if (region is ReviveRegion)
                    {
                        this.reviveAreas.Add(region as ReviveRegion);
                    }
                    else if (region is SoldierRegion)
                    {
                        this.soldierAreas.Add(region as SoldierRegion);
                    }
                    else if (region is CommonSpawnGroup)
                    {
                        this.commonSpawnGroups.Add(region as CommonSpawnGroup);
                    }
                    else if (region is SpawnGroup)
                    {
                        this.spawnGroups.Add(region as SpawnGroup);
                    }
                    else if (region is WaypointsHolder)
                    {
                        this.waypointsList.Add(region as WaypointsHolder);
                    }
                    else if (region is AreaEventTrigger)
                    {
                        AreaEventTrigger item = region as AreaEventTrigger;
                        this.triggerList.Add(item);
                        GameObject obj2 = region.get_gameObject();
                        ListView<AreaEventTrigger> triggers = null;
                        for (int i = this.objTriggerMultiMap.Count - 1; i >= 0; i--)
                        {
                            ObjTriggerKeyValuePair pair = this.objTriggerMultiMap[i];
                            if (pair.obj == obj2)
                            {
                                triggers = pair.triggers;
                                break;
                            }
                        }
                        if (triggers == null)
                        {
                            triggers = new ListView<AreaEventTrigger>();
                            ObjTriggerKeyValuePair pair2 = new ObjTriggerKeyValuePair();
                            pair2.obj = obj2;
                            pair2.triggers = triggers;
                            this.objTriggerMultiMap.Add(pair2);
                        }
                        triggers.Add(item);
                    }
                    else if (region is DynamicChannel)
                    {
                        this.channelList.Add(region as DynamicChannel);
                    }
                }
            }
            this.TrueMen = new List<PoolObjHandle<ActorRoot>>(Singleton<GameObjMgr>.GetInstance().SoldierActors);
        }

        public void Startup()
        {
            if (MTileHandlerHelper.Instance != null)
            {
                MTileHandlerHelper.Instance.UpdateLogic();
            }
            SoldierWave.ms_updatedFrameNum = 0;
            ListView<SoldierRegion>.Enumerator enumerator = this.soldierAreas.GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.Startup();
            }
            ListView<CommonSpawnGroup>.Enumerator enumerator2 = this.commonSpawnGroups.GetEnumerator();
            while (enumerator2.MoveNext())
            {
                enumerator2.Current.Startup();
            }
            ListView<SpawnGroup>.Enumerator enumerator3 = this.spawnGroups.GetEnumerator();
            while (enumerator3.MoveNext())
            {
                enumerator3.Current.Startup();
            }
            ListView<AreaEventTrigger>.Enumerator enumerator4 = this.triggerList.GetEnumerator();
            while (enumerator4.MoveNext())
            {
                enumerator4.Current.Startup();
            }
            ListView<DynamicChannel>.Enumerator enumerator5 = this.channelList.GetEnumerator();
            while (enumerator5.MoveNext())
            {
                enumerator5.Current.Startup();
            }
            SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
            this.ClearSoldierActivateTimers();
            if ((curLvelContext != null) && curLvelContext.IsMobaModeWithOutGuide())
            {
                this.SoldierActivateDelay = curLvelContext.m_soldierActivateDelay;
                this.SoldierActivateCountDelay1 = curLvelContext.m_soldierActivateCountDelay1;
                this.SoldierActivateCountDelay2 = curLvelContext.m_soldierActivateCountDelay2;
                this.WelcomeDelaySeq = Singleton<CTimerManager>.instance.AddTimer(500, 1, new CTimer.OnTimeUpHandler(this.OnWelcomeDelay), true);
            }
            if (this.SoldierActivateDelay > 0)
            {
                this.SoldierActivateDelaySeq = Singleton<CTimerManager>.instance.AddTimer(this.SoldierActivateDelay, 1, new CTimer.OnTimeUpHandler(this.OnSoldierActivateDelay), true);
            }
            if (this.SoldierActivateCountDelay1 > 0)
            {
                this.SoldierActivateCountDelay1Seq = Singleton<CTimerManager>.instance.AddTimer(this.SoldierActivateCountDelay1, 1, new CTimer.OnTimeUpHandler(this.OnSoldierActivateCountDelay1), true);
            }
            if (this.SoldierActivateCountDelay2 > 0)
            {
                this.SoldierActivateCountDelay2Seq = Singleton<CTimerManager>.instance.AddTimer(this.SoldierActivateCountDelay2, 1, new CTimer.OnTimeUpHandler(this.OnSoldierActivateCountDelay2), true);
            }
        }

        public void UpdateLogic(int delta)
        {
            Singleton<SceneManagement>.GetInstance().UpdateDirtyNodes();
            if (!this.m_bSoldierOverNum)
            {
                bool flag = true;
                int num = this.soldierAreas.Count;
                for (int m = 0; m < num; m++)
                {
                    SoldierRegion region = this.soldierAreas[m];
                    if (region.isStartup)
                    {
                        SoldierSpawnResult result = region.UpdateLogicSpec(delta);
                        flag &= result != SoldierSpawnResult.ShouldWaitSoldierInterval;
                    }
                }
                if (flag)
                {
                    this.CheckSoldierOverNumUpper();
                }
            }
            else
            {
                this.CheckSoldierOverNumLower();
            }
            int count = this.commonSpawnGroups.Count;
            for (int i = 0; i < count; i++)
            {
                CommonSpawnGroup group = this.commonSpawnGroups[i];
                if (group.isStartup)
                {
                    group.UpdateLogic(delta);
                }
            }
            for (int j = 0; j < this.spawnGroups.Count; j++)
            {
                SpawnGroup group2 = this.spawnGroups[j];
                if (group2.isStartup)
                {
                    group2.UpdateLogic(delta);
                }
            }
            int num6 = this.objTriggerMultiMap.Count;
            for (int k = 0; k < num6; k++)
            {
                ObjTriggerKeyValuePair pair = this.objTriggerMultiMap[k];
                GameObject obj2 = pair.obj;
                ListView<AreaEventTrigger> triggers = pair.triggers;
                if (((obj2 != null) && obj2.get_activeSelf()) && ((Singleton<FrameSynchr>.instance.CurFrameNum % 4) == (k % 4)))
                {
                    bool flag2 = false;
                    int num8 = triggers.Count;
                    for (int n = 0; n < num8; n++)
                    {
                        AreaEventTrigger trigger = triggers[n];
                        if ((trigger != null) && trigger.isStartup)
                        {
                            trigger.UpdateLogic(delta * 4);
                            flag2 |= trigger.bDoDeactivating;
                        }
                    }
                    if (flag2)
                    {
                        for (int num10 = 0; num10 < num8; num10++)
                        {
                            AreaEventTrigger trigger2 = triggers[num10];
                            if (trigger2 != null)
                            {
                                trigger2.DoSelfDeactivating();
                            }
                        }
                    }
                }
            }
        }

        public AGE.ActionHelper ActionHelper
        {
            get
            {
                return base.GetComponent<AGE.ActionHelper>();
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ObjTriggerKeyValuePair
        {
            public GameObject obj;
            public ListView<AreaEventTrigger> triggers;
        }
    }
}

