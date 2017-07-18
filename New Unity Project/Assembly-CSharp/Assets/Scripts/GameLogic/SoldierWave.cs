namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.DataCenter;
    using Assets.Scripts.GameLogic.GameKernal;
    using Assets.Scripts.GameSystem;
    using CSProtocol;
    using ResData;
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class SoldierWave
    {
        [CompilerGenerated]
        private int <Index>k__BackingField;
        [CompilerGenerated]
        private SoldierRegion <Region>k__BackingField;
        [CompilerGenerated]
        private ResSoldierWaveInfo <WaveInfo>k__BackingField;
        private bool bInIdleState;
        private int curTick;
        private int firstTick = -1;
        private int idleTick;
        private bool isCannonNotified;
        public static uint ms_updatedFrameNum;
        private int preSpawnTick;
        public int repeatCount = 1;
        public SoldierSelector Selector = new SoldierSelector();

        public SoldierWave(SoldierRegion InRegion, ResSoldierWaveInfo InWaveInfo, int InIndex)
        {
            this.Region = InRegion;
            this.WaveInfo = InWaveInfo;
            this.Index = InIndex;
            DebugHelper.Assert((this.Region != null) && (InWaveInfo != null));
            this.isCannonNotified = false;
            this.Reset();
        }

        public void CloneState(SoldierWave sw)
        {
            this.curTick = sw.curTick;
            this.preSpawnTick = sw.preSpawnTick;
            this.firstTick = sw.firstTick;
            this.bInIdleState = sw.bInIdleState;
            this.idleTick = sw.idleTick;
            this.bInIdleState = sw.bInIdleState;
            this.isCannonNotified = sw.isCannonNotified;
        }

        public void Reset()
        {
            this.curTick = this.preSpawnTick = 0;
            this.firstTick = -1;
            this.repeatCount = 1;
            this.bInIdleState = false;
            this.idleTick = 0;
            this.Selector.Reset(this.WaveInfo);
        }

        private void SpawnSoldier(uint SoldierID)
        {
            ResMonsterCfgInfo dataCfgInfoByCurLevelDiff = MonsterDataHelper.GetDataCfgInfoByCurLevelDiff((int) SoldierID);
            if (dataCfgInfoByCurLevelDiff != null)
            {
                if (CActorInfo.GetActorInfo(StringHelper.UTF8BytesToString(ref dataCfgInfoByCurLevelDiff.szCharacterInfo), enResourceType.BattleScene) != null)
                {
                    Transform transform = this.Region.get_transform();
                    COM_PLAYERCAMP campType = this.Region.CampType;
                    ActorMeta meta2 = new ActorMeta();
                    meta2.ConfigId = (int) SoldierID;
                    meta2.ActorType = ActorTypeDef.Actor_Type_Monster;
                    meta2.ActorCamp = campType;
                    ActorMeta actorMeta = meta2;
                    VInt3 pos = transform.get_position();
                    VInt3 dir = transform.get_forward();
                    PoolObjHandle<ActorRoot> actor = new PoolObjHandle<ActorRoot>();
                    if (!Singleton<GameObjMgr>.GetInstance().TryGetFromCache(ref actor, ref actorMeta))
                    {
                        actor = Singleton<GameObjMgr>.GetInstance().SpawnActorEx(null, ref actorMeta, pos, dir, false, true);
                        if (actor != 0)
                        {
                            actor.handle.InitActor();
                            actor.handle.PrepareFight();
                            Singleton<GameObjMgr>.instance.AddActor(actor);
                            actor.handle.StartFight();
                        }
                    }
                    else
                    {
                        ActorRoot handle = actor.handle;
                        handle.TheActorMeta.ActorCamp = actorMeta.ActorCamp;
                        handle.ReactiveActor(pos, dir);
                    }
                    if (actor != 0)
                    {
                        if (this.Region.AttackRoute != null)
                        {
                            actor.handle.ActorControl.AttackAlongRoute(this.Region.AttackRoute.GetComponent<WaypointsHolder>());
                        }
                        else if (this.Region.finalTarget != null)
                        {
                            FrameCommand<AttackPositionCommand> cmd = FrameCommandFactory.CreateFrameCommand<AttackPositionCommand>();
                            cmd.cmdId = 1;
                            cmd.cmdData.WorldPos = new VInt3(this.Region.finalTarget.get_transform().get_position());
                            actor.handle.ActorControl.CmdAttackMoveToDest(cmd, cmd.cmdData.WorldPos);
                        }
                        if (!this.isCannonNotified && (this.WaveInfo.bType == 1))
                        {
                            KillNotify theKillNotify = Singleton<CBattleSystem>.GetInstance().TheKillNotify;
                            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
                            if ((theKillNotify != null) && (hostPlayer != null))
                            {
                                bool bSrcAllies = hostPlayer.PlayerCamp == actor.handle.TheActorMeta.ActorCamp;
                                if (bSrcAllies)
                                {
                                    KillInfo killInfo = new KillInfo((hostPlayer.PlayerCamp != COM_PLAYERCAMP.COM_PLAYERCAMP_1) ? KillNotify.red_cannon_icon : KillNotify.blue_cannon_icon, null, KillDetailInfoType.Info_Type_Cannon_Spawned, bSrcAllies, false, ActorTypeDef.Invalid);
                                    theKillNotify.AddKillInfo(ref killInfo);
                                    this.isCannonNotified = true;
                                }
                            }
                        }
                    }
                }
                if (this.Region.bTriggerEvent)
                {
                    SoldierWaveParam prm = new SoldierWaveParam(this.Index, this.repeatCount, this.Region.GetNextRepeatTime(false));
                    Singleton<GameEventSys>.instance.SendEvent<SoldierWaveParam>(GameEventDef.Event_SoldierWaveNextRepeat, ref prm);
                }
            }
        }

        public SoldierSpawnResult Update(int delta)
        {
            this.firstTick = (this.firstTick != -1) ? this.firstTick : this.curTick;
            this.curTick += delta;
            if (this.bInIdleState)
            {
                if ((this.curTick - this.idleTick) < this.WaveInfo.dwIntervalTick)
                {
                    return SoldierSpawnResult.ShouldWaitInterval;
                }
                this.bInIdleState = false;
                this.Selector.Reset(this.WaveInfo);
                this.repeatCount++;
            }
            if (this.curTick < this.WaveInfo.dwStartWatiTick)
            {
                return SoldierSpawnResult.ShouldWaitStart;
            }
            if ((((this.curTick - this.firstTick) >= this.WaveInfo.dwRepeatTimeTick) && (this.WaveInfo.dwRepeatTimeTick > 0)) && (!this.Region.bForceCompleteSpawn || (this.Region.bForceCompleteSpawn && this.Selector.isFinished)))
            {
                return SoldierSpawnResult.Finish;
            }
            if ((this.curTick - this.preSpawnTick) >= MonoSingleton<GlobalConfig>.instance.SoldierWaveInterval)
            {
                if (this.Selector.isFinished)
                {
                    if ((this.WaveInfo.dwRepeatNum != 0) && (this.repeatCount >= this.WaveInfo.dwRepeatNum))
                    {
                        return SoldierSpawnResult.Finish;
                    }
                    this.bInIdleState = true;
                    this.idleTick = this.curTick;
                    return SoldierSpawnResult.ShouldWaitInterval;
                }
                if (ms_updatedFrameNum < Singleton<FrameSynchr>.instance.CurFrameNum)
                {
                    uint soldierID = this.Selector.NextSoldierID();
                    DebugHelper.Assert(soldierID != 0);
                    this.SpawnSoldier(soldierID);
                    this.preSpawnTick = this.curTick;
                    ms_updatedFrameNum = Singleton<FrameSynchr>.instance.CurFrameNum;
                }
            }
            return SoldierSpawnResult.ShouldWaitSoldierInterval;
        }

        public int Index
        {
            [CompilerGenerated]
            get
            {
                return this.<Index>k__BackingField;
            }
            [CompilerGenerated]
            protected set
            {
                this.<Index>k__BackingField = value;
            }
        }

        public bool IsInIdle
        {
            get
            {
                return this.bInIdleState;
            }
        }

        public SoldierRegion Region
        {
            [CompilerGenerated]
            get
            {
                return this.<Region>k__BackingField;
            }
            [CompilerGenerated]
            protected set
            {
                this.<Region>k__BackingField = value;
            }
        }

        public ResSoldierWaveInfo WaveInfo
        {
            [CompilerGenerated]
            get
            {
                return this.<WaveInfo>k__BackingField;
            }
            [CompilerGenerated]
            protected set
            {
                this.<WaveInfo>k__BackingField = value;
            }
        }
    }
}

