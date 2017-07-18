namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;

    public class Horizon : IUpdateLogic
    {
        private bool _enabled = false;
        private bool _fighting = false;
        private static int _globalSight;
        private static int exposeDurationHero;
        private static int exposeDurationNormal;
        private static int exposeRadius;
        private static int fakeSightRadius;
        private static int fowTowerSightRadius;
        private static int GlobalSightSqr_;
        private static int showmarkDurationHero;
        private static int soldierSightRadius;
        public const byte UPDATE_CYCLE = 8;

        public void FightOver()
        {
            this.Enabled = false;
            _globalSight = 0;
            GlobalSightSqr_ = 0;
        }

        public void FightStart()
        {
            this._fighting = true;
            QueryGlobalSight();
            this._enabled = Singleton<BattleLogic>.instance.GetCurLvelContext().m_horizonEnableMethod == EnableMethod.EnableAll;
        }

        public static int QueryAttackShowMarkDuration()
        {
            if ((showmarkDurationHero == 0) && (GameDataMgr.globalInfoDatabin != null))
            {
                ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x10b);
                if (dataByKey != null)
                {
                    showmarkDurationHero = (int) dataByKey.dwConfValue;
                }
            }
            return showmarkDurationHero;
        }

        public static int QueryExposeDurationHero()
        {
            if ((exposeDurationHero == 0) && (GameDataMgr.globalInfoDatabin != null))
            {
                ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0xe0);
                if (dataByKey != null)
                {
                    exposeDurationHero = (int) dataByKey.dwConfValue;
                }
            }
            return exposeDurationHero;
        }

        public static int QueryExposeDurationNormal()
        {
            if ((exposeDurationNormal == 0) && (GameDataMgr.globalInfoDatabin != null))
            {
                ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0xdf);
                if (dataByKey != null)
                {
                    exposeDurationNormal = (int) dataByKey.dwConfValue;
                }
            }
            return exposeDurationNormal;
        }

        public static int QueryExposeRadius()
        {
            if ((exposeRadius == 0) && (GameDataMgr.globalInfoDatabin != null))
            {
                ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0xde);
                if (dataByKey != null)
                {
                    exposeRadius = (int) dataByKey.dwConfValue;
                }
            }
            return exposeRadius;
        }

        public static int QueryFowTowerSightRadius()
        {
            if ((fowTowerSightRadius == 0) && (GameDataMgr.globalInfoDatabin != null))
            {
                ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x102);
                if (dataByKey != null)
                {
                    fowTowerSightRadius = (int) dataByKey.dwConfValue;
                }
            }
            return fowTowerSightRadius;
        }

        public static int QueryGlobalSight()
        {
            if ((_globalSight == 0) && (GameDataMgr.globalInfoDatabin != null))
            {
                ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x38);
                if (dataByKey != null)
                {
                    _globalSight = (int) dataByKey.dwConfValue;
                    GlobalSightSqr_ = _globalSight * _globalSight;
                }
            }
            return _globalSight;
        }

        public static int QueryMainActorFakeSightRadius()
        {
            if ((fakeSightRadius == 0) && (GameDataMgr.globalInfoDatabin != null))
            {
                ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0xf6);
                if (dataByKey != null)
                {
                    fakeSightRadius = (int) dataByKey.dwConfValue;
                }
            }
            return fakeSightRadius;
        }

        public static int QuerySoldierSightRadius()
        {
            if ((soldierSightRadius == 0) && (GameDataMgr.globalInfoDatabin != null))
            {
                ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x101);
                if (dataByKey != null)
                {
                    soldierSightRadius = (int) dataByKey.dwConfValue;
                }
            }
            return soldierSightRadius;
        }

        public void UpdateLogic(int delta)
        {
            if (this._enabled && this._fighting)
            {
                uint num = Singleton<FrameSynchr>.instance.CurFrameNum % 8;
                GameObjMgr instance = Singleton<GameObjMgr>.GetInstance();
                List<PoolObjHandle<ActorRoot>> gameActors = instance.GameActors;
                int count = gameActors.Count;
                for (int i = 0; i < count; i++)
                {
                    if (gameActors[i] != 0)
                    {
                        PoolObjHandle<ActorRoot> handle = gameActors[i];
                        ActorRoot root = handle.handle;
                        if (((root.ObjID % 8) == num) && (!root.ActorControl.IsDeadState || root.TheStaticData.TheBaseAttribute.DeadControl))
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                if (j != root.TheActorMeta.ActorCamp)
                                {
                                    COM_PLAYERCAMP actorCamp = root.TheActorMeta.ActorCamp;
                                    List<PoolObjHandle<ActorRoot>> campActors = instance.GetCampActors((COM_PLAYERCAMP) j);
                                    int num5 = campActors.Count;
                                    for (int k = 0; k < num5; k++)
                                    {
                                        if (campActors[k] != 0)
                                        {
                                            PoolObjHandle<ActorRoot> handle2 = campActors[k];
                                            ActorRoot root2 = handle2.handle;
                                            if (!root2.HorizonMarker.IsSightVisited(actorCamp))
                                            {
                                                long sightRadius = GlobalSightSqr_;
                                                if (root.HorizonMarker.SightRadius != 0)
                                                {
                                                    sightRadius = root.HorizonMarker.SightRadius;
                                                    sightRadius *= sightRadius;
                                                }
                                                VInt3 num8 = root2.location - root.location;
                                                if (num8.sqrMagnitudeLong2D < sightRadius)
                                                {
                                                    root2.HorizonMarker.VisitSight(actorCamp);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public bool Enabled
        {
            get
            {
                return this._enabled;
            }
            set
            {
                if (value != this._enabled)
                {
                    this._enabled = value;
                    List<PoolObjHandle<ActorRoot>> gameActors = Singleton<GameObjMgr>.GetInstance().GameActors;
                    int count = gameActors.Count;
                    for (int i = 0; i < count; i++)
                    {
                        PoolObjHandle<ActorRoot> handle = gameActors[i];
                        handle.handle.HorizonMarker.SetEnabled(this._enabled);
                    }
                }
            }
        }

        public enum EnableMethod
        {
            DisableAll,
            EnableAll,
            EnableMarkNoMist,
            INVALID
        }
    }
}

