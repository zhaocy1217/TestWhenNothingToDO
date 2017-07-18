namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic.GameKernal;
    using CSProtocol;
    using System;
    using UnityEngine;

    public class BulletWrapper : ObjWrapper
    {
        private bool bMoveCollision;
        public bool m_bVisibleByFow;
        public bool m_bVisibleByShape;
        private int m_sightRadius;
        private int m_sightRange;
        private int moveDelta;
        private ListView<GameObject> SubParObjList_;

        public void AddSubParObj(GameObject inParObj)
        {
            if (inParObj != null)
            {
                this.SubParObjList_.Add(inParObj);
            }
        }

        public override void Born(ActorRoot owner)
        {
            base.actor = owner;
            base.actorPtr = new PoolObjHandle<ActorRoot>(base.actor);
        }

        private void ClearSubParObjs()
        {
            if (this.SubParObjList_ != null)
            {
                this.SubParObjList_.Clear();
                this.SubParObjList_ = null;
            }
        }

        public override void Deactive()
        {
            this.ClearSubParObjs();
            base.Deactive();
        }

        public override void Fight()
        {
        }

        public override void FightOver()
        {
        }

        public bool GetMoveCollisiong()
        {
            return this.bMoveCollision;
        }

        public int GetMoveDelta()
        {
            return this.moveDelta;
        }

        public override string GetTypeName()
        {
            return "BulletWrapper";
        }

        public override void Init()
        {
            base.Init();
            this.InitSubParObjList();
        }

        public void InitForInvisibleBullet()
        {
            if (base.actor != null)
            {
                base.gameObject.SetLayer("Actor", "Particles", true);
                base.actor.SkillControl = base.actor.CreateLogicComponent<SkillComponent>(base.actor);
                base.actor.BuffHolderComp = base.actor.CreateLogicComponent<BuffHolderComponent>(base.actor);
                if (FogOfWar.enable)
                {
                    base.actor.HorizonMarker = base.actor.CreateLogicComponent<HorizonMarkerByFow>(base.actor);
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
            }
        }

        private void InitSubParObjList()
        {
            if (this.SubParObjList_ == null)
            {
                this.SubParObjList_ = new ListView<GameObject>();
            }
        }

        public override void OnUse()
        {
            base.OnUse();
            this.moveDelta = 0;
            this.bMoveCollision = false;
            this.m_bVisibleByFow = false;
            this.m_bVisibleByShape = false;
            this.m_sightRadius = 0;
            this.m_sightRange = 0;
            this.ClearSubParObjs();
        }

        public override void Prepare()
        {
        }

        public override void Reactive()
        {
            base.Reactive();
            this.InitSubParObjList();
        }

        public void SetMoveCollision(bool _bUsed)
        {
            this.bMoveCollision = _bUsed;
        }

        public void SetMoveDelta(int _delta)
        {
            this.moveDelta = _delta;
        }

        public override void Uninit()
        {
            this.ClearSubParObjs();
            base.Uninit();
        }

        public void UninitForInvisibleBullet()
        {
            if (base.actor != null)
            {
                if (base.actor.BuffHolderComp != null)
                {
                    base.actor.BuffHolderComp.ClearBuff();
                }
                if (base.actor.HorizonMarker != null)
                {
                    COM_PLAYERCAMP[] othersCmp = BattleLogic.GetOthersCmp(base.actor.TheActorMeta.ActorCamp);
                    for (int i = 0; i < othersCmp.Length; i++)
                    {
                        if (base.actor.HorizonMarker.HasHideMark(othersCmp[i], HorizonConfig.HideMark.Skill))
                        {
                            base.actor.HorizonMarker.AddHideMark(othersCmp[i], HorizonConfig.HideMark.Skill, -1, true);
                        }
                    }
                    base.actor.HorizonMarker.SetTranslucentMark(HorizonConfig.HideMark.Skill, false, true);
                    base.actor.HorizonMarker.SetTranslucentMark(HorizonConfig.HideMark.Jungle, false, true);
                }
            }
        }

        public override void UpdateLogic(int delta)
        {
            int count = this.SubParObjList_.Count;
            if (count > 0)
            {
                for (int i = count - 1; i >= 0; i--)
                {
                    if (this.SubParObjList_[i] == null)
                    {
                        this.SubParObjList_.RemoveAt(i);
                    }
                }
            }
        }

        public void UpdateSubParObjVisibility(bool inVisible)
        {
            int count = this.SubParObjList_.Count;
            if (count > 0)
            {
                for (int i = count - 1; i >= 0; i--)
                {
                    GameObject go = this.SubParObjList_[i];
                    if (go == null)
                    {
                        this.SubParObjList_.RemoveAt(i);
                    }
                    else if (inVisible)
                    {
                        go.SetLayer("Actor", "Particles", true);
                    }
                    else
                    {
                        go.SetLayer("Hide", true);
                    }
                }
            }
        }

        public int SightRadius
        {
            get
            {
                return this.m_sightRadius;
            }
            set
            {
                if (this.m_sightRadius != value)
                {
                    this.m_sightRadius = value;
                    if (FogOfWar.enable)
                    {
                        Singleton<GameFowManager>.instance.m_pFieldObj.UnrealToGridX(this.m_sightRadius, out this.m_sightRange);
                    }
                }
            }
        }

        public int SightRange
        {
            get
            {
                return this.m_sightRange;
            }
            private set
            {
                this.m_sightRange = value;
            }
        }
    }
}

