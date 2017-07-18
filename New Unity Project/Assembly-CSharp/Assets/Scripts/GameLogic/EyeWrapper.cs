namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic.GameKernal;
    using ResData;
    using System;
    using System.Runtime.CompilerServices;

    public class EyeWrapper : ObjWrapper
    {
        [CompilerGenerated]
        private ResMonsterCfgInfo <cfgInfo>k__BackingField;
        [CompilerGenerated]
        private int <lifeTimeTotal>k__BackingField;
        public bool bLifeTimeOver;
        private const string DeadAnimName = "Eye_Dead";
        private int lifeTime;

        public override void Born(ActorRoot owner)
        {
            base.actor = owner;
            base.actorPtr = new PoolObjHandle<ActorRoot>(base.actor);
            base.ClearNeedToHelpOther();
            base.ClearNeedSwitchTarget();
            base.m_curWaypointsHolder = null;
            base.m_curWaypointTarget = null;
            base.m_isCurWaypointEndPoint = false;
            base.m_isStartPoint = false;
            base.m_isControledByMan = true;
            base.m_isAutoAI = false;
            base.m_offline = false;
            base.m_followOther = false;
            base.m_leaderID = 0;
            base.m_isAttackedByEnemyHero = false;
            base.m_isAttacked = false;
            base.bForceNotRevive = false;
            base.actor.SkillControl = base.actor.CreateLogicComponent<SkillComponent>(base.actor);
            base.actor.ValueComponent = base.actor.CreateLogicComponent<ValueProperty>(base.actor);
            base.actor.HurtControl = base.actor.CreateLogicComponent<HurtComponent>(base.actor);
            base.actor.BuffHolderComp = base.actor.CreateLogicComponent<BuffHolderComponent>(base.actor);
            base.actor.AnimControl = base.actor.CreateLogicComponent<AnimPlayComponent>(base.actor);
            base.actor.HudControl = base.actor.CreateLogicComponent<HudComponent3D>(base.actor);
            if (FogOfWar.enable)
            {
                base.actor.HorizonMarker = base.actor.CreateLogicComponent<HorizonMarkerByFow>(base.actor);
            }
            else
            {
                base.actor.HorizonMarker = base.actor.CreateLogicComponent<HorizonMarker>(base.actor);
            }
            base.actor.MatHurtEffect = base.actor.CreateActorComponent<MaterialHurtEffect>(base.actor);
            if ((base.actor.MatHurtEffect != null) && (base.actor.MatHurtEffect.mats != null))
            {
                base.actor.MatHurtEffect.mats.Clear();
                base.actor.MatHurtEffect.mats = null;
            }
            this.cfgInfo = MonsterDataHelper.GetDataCfgInfo(base.actor.TheActorMeta.ConfigId, 1);
            this.bLifeTimeOver = false;
        }

        public override void Deactive()
        {
            base.Deactive();
        }

        public override void Fight()
        {
            base.Fight();
        }

        public override void FightOver()
        {
            base.FightOver();
        }

        public override string GetTypeName()
        {
            return "EyeWrapper";
        }

        public override void Init()
        {
            base.Init();
            if (FogOfWar.enable)
            {
                Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
                if (hostPlayer != null)
                {
                    if (base.actor.TheActorMeta.ActorCamp == hostPlayer.PlayerCamp)
                    {
                        base.actor.Visible = true;
                    }
                    else
                    {
                        VInt3 worldLoc = new VInt3(base.actor.location.x, base.actor.location.z, 0);
                        base.actor.Visible = Singleton<GameFowManager>.instance.IsSurfaceCellVisible(worldLoc, hostPlayer.PlayerCamp);
                    }
                }
            }
        }

        protected override void OnDead()
        {
            base.OnDead();
            if (true)
            {
                Singleton<GameObjMgr>.instance.RecycleActor(base.actorPtr, this.RecycleTime);
            }
            if (!string.IsNullOrEmpty("Eye_Dead"))
            {
                AnimPlayComponent animControl = base.actor.AnimControl;
                if (animControl != null)
                {
                    PlayAnimParam param = new PlayAnimParam();
                    param.animName = "Eye_Dead";
                    param.blendTime = 0f;
                    param.loop = false;
                    param.layer = 1;
                    param.speed = 1f;
                    param.cancelCurrent = true;
                    param.cancelAll = true;
                    animControl.Play(param);
                }
            }
            if (base.actor.HorizonMarker != null)
            {
                base.actor.HorizonMarker.SetTranslucentMark(HorizonConfig.HideMark.Skill, false, false);
                base.actor.HorizonMarker.SetTranslucentMark(HorizonConfig.HideMark.Jungle, false, false);
            }
        }

        public override void OnUse()
        {
            base.OnUse();
            this.lifeTime = 0;
            this.lifeTimeTotal = 0;
            this.cfgInfo = null;
            this.bLifeTimeOver = false;
        }

        public override void Prepare()
        {
            base.Prepare();
        }

        public override void Reactive()
        {
            base.Reactive();
            this.LifeTime = 0;
        }

        public override void Uninit()
        {
            base.Uninit();
        }

        public override void UpdateLogic(int delta)
        {
            base.updateAffectActors();
            if (this.lifeTime > 0)
            {
                this.lifeTime -= delta;
                if (this.lifeTime <= 0)
                {
                    this.lifeTime = 0;
                    this.bLifeTimeOver = true;
                    base.SetObjBehaviMode(ObjBehaviMode.State_Dead);
                }
                this.UpdateTimerBar();
            }
        }

        private void UpdateTimerBar()
        {
            if (base.actor.HudControl != null)
            {
                base.actor.HudControl.UpdateTimerBar(this.lifeTime, this.lifeTimeTotal);
            }
        }

        public ResMonsterCfgInfo cfgInfo
        {
            [CompilerGenerated]
            get
            {
                return this.<cfgInfo>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<cfgInfo>k__BackingField = value;
            }
        }

        public int LifeTime
        {
            get
            {
                return this.lifeTime;
            }
            set
            {
                this.lifeTime = value;
                this.lifeTimeTotal = value;
                this.UpdateTimerBar();
            }
        }

        public int lifeTimeTotal
        {
            [CompilerGenerated]
            get
            {
                return this.<lifeTimeTotal>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<lifeTimeTotal>k__BackingField = value;
            }
        }

        private int RecycleTime
        {
            get
            {
                if (this.cfgInfo != null)
                {
                    return this.cfgInfo.iRecyleTime;
                }
                return 0;
            }
        }
    }
}

