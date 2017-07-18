namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.GameLogic.GameKernal;
    using CSProtocol;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class HorizonMarkerByFow : HorizonMarkerBase
    {
        private CampMarker[] _campMarkers;
        private byte _translucentMarks;
        private bool[] m_bExposed;
        private bool m_bTranslucent;
        private int[] m_exposeCampArr;
        private VInt3[] m_exposedPos;
        private int m_exposeRadiusCache = -1;
        private int[] m_exposeTimerSeq;
        private int[] m_showmarkTimerSeq;
        private ListView<GameObject> SubParObjList_;

        public override void AddHideMark(COM_PLAYERCAMP targetCamp, HorizonConfig.HideMark hm, int count, bool bForbidFade = false)
        {
            if ((hm != HorizonConfig.HideMark.Jungle) && (targetCamp != COM_PLAYERCAMP.COM_PLAYERCAMP_MID))
            {
                if (targetCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT)
                {
                    for (int i = 0; i < this._campMarkers.Length; i++)
                    {
                        if (i != TranslateCampToIndex(base.actor.TheActorMeta.ActorCamp))
                        {
                            this._campMarkers[i].AddHideMark(hm, count);
                            Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
                            if ((hostPlayer != null) && (hostPlayer.PlayerCamp == TranslateIndexToCamp(i)))
                            {
                                this.RefreshVisible();
                            }
                        }
                    }
                }
                else
                {
                    this._campMarkers[TranslateCampToIndex(targetCamp)].AddHideMark(hm, count);
                    Player player2 = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
                    if ((player2 != null) && (targetCamp == player2.PlayerCamp))
                    {
                        this.RefreshVisible();
                    }
                }
            }
        }

        public override void AddShowMark(COM_PLAYERCAMP targetCamp, HorizonConfig.ShowMark sm, int count)
        {
            if (targetCamp != COM_PLAYERCAMP.COM_PLAYERCAMP_MID)
            {
                if (targetCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT)
                {
                    for (int i = 0; i < this._campMarkers.Length; i++)
                    {
                        if (i != TranslateCampToIndex(base.actor.TheActorMeta.ActorCamp))
                        {
                            this._campMarkers[i].AddShowMark(sm, count);
                            Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
                            if ((hostPlayer != null) && (hostPlayer.PlayerCamp == TranslateIndexToCamp(i)))
                            {
                                this.RefreshVisible();
                            }
                        }
                    }
                }
                else
                {
                    this._campMarkers[TranslateCampToIndex(targetCamp)].AddShowMark(sm, count);
                    Player player2 = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
                    if ((player2 != null) && (targetCamp == player2.PlayerCamp))
                    {
                        this.RefreshVisible();
                    }
                }
            }
        }

        public override void AddSubParObj(GameObject inParObj)
        {
            if (inParObj != null)
            {
                this.SubParObjList_.Add(inParObj);
            }
        }

        private void ClearExposeTimer()
        {
            int num = 2;
            for (int i = 0; i < num; i++)
            {
                if (this.m_exposeTimerSeq != null)
                {
                    if (this.m_exposeTimerSeq[i] >= 0)
                    {
                        Singleton<CTimerManager>.instance.RemoveTimer(this.m_exposeTimerSeq[i]);
                    }
                    this.m_exposeTimerSeq[i] = -1;
                }
                if (this.m_showmarkTimerSeq != null)
                {
                    if (this.m_showmarkTimerSeq[i] >= 0)
                    {
                        Singleton<CTimerManager>.instance.RemoveTimer(this.m_showmarkTimerSeq[i]);
                    }
                    this.m_showmarkTimerSeq[i] = -1;
                }
            }
            this.m_exposeTimerSeq = null;
            this.m_showmarkTimerSeq = null;
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
            this.ClearExposeTimer();
            this.ClearSubParObjs();
            base.Deactive();
        }

        public override void ExposeAndShowAsAttacker(COM_PLAYERCAMP attackeeCamp, bool bExposeAttacker)
        {
            if (attackeeCamp != COM_PLAYERCAMP.COM_PLAYERCAMP_MID)
            {
                if (bExposeAttacker && base.actor.ActorControl.DoesApplyExposingRule())
                {
                    this.ExposeAsAttacker(attackeeCamp, 0);
                }
                if (base.actor.ActorControl.DoesApplyShowmarkRule() && this.HasHideMark(attackeeCamp, HorizonConfig.HideMark.Skill))
                {
                    this.ShowAsAttacker(attackeeCamp, 0);
                }
            }
        }

        public override void ExposeAsAttacker(COM_PLAYERCAMP attackeeCamp, int inResetTime)
        {
            int index = TranslateCampToIndex(attackeeCamp);
            CTimerManager instance = Singleton<CTimerManager>.instance;
            if (this.SetExposeMark(true, attackeeCamp, base.actor.ActorControl.DoesIgnoreAlreadyLit()))
            {
                if (this.IsDuringExposing(attackeeCamp))
                {
                    if (inResetTime <= 0)
                    {
                        instance.ResetTimer(this.m_exposeTimerSeq[index]);
                    }
                    else if (instance.GetLeftTime(this.m_exposeTimerSeq[index]) < inResetTime)
                    {
                        instance.ResetTimerTotalTime(this.m_exposeTimerSeq[index], inResetTime);
                    }
                }
                else if (index == 0)
                {
                    this.m_exposeTimerSeq[index] = instance.AddTimer(base.actor.ActorControl.QueryExposeDuration(), 1, new CTimer.OnTimeUpHandler(this.OnExposeOverCampOne), true);
                }
                else
                {
                    this.m_exposeTimerSeq[index] = instance.AddTimer(base.actor.ActorControl.QueryExposeDuration(), 1, new CTimer.OnTimeUpHandler(this.OnExposeOverCampTwo), true);
                }
            }
        }

        public override int[] GetExposedCamps()
        {
            Array.Clear(this.m_exposeCampArr, 0, this.m_exposeCampArr.Length);
            if (base.actor.TheActorMeta.ActorCamp != COM_PLAYERCAMP.COM_PLAYERCAMP_MID)
            {
                this.m_exposeCampArr[TranslateCampToIndex(base.actor.TheActorMeta.ActorCamp)] = base.SightRange;
            }
            for (int i = 0; i < 2; i++)
            {
                if ((this.m_exposeCampArr[i] == 0) && this.IsExposedFor(TranslateIndexToCamp(i)))
                {
                    this.m_exposeCampArr[i] = this.ExposeRadiusCache;
                }
            }
            if (base.actor.TheActorMeta.ActorCamp != COM_PLAYERCAMP.COM_PLAYERCAMP_MID)
            {
                this.m_exposedPos[TranslateCampToIndex(base.actor.TheActorMeta.ActorCamp)] = new VInt3(base.actor.location.x, base.actor.location.z, 0);
            }
            return this.m_exposeCampArr;
        }

        public override VInt3[] GetExposedPos()
        {
            return this.m_exposedPos;
        }

        public override bool HasHideMark(COM_PLAYERCAMP targetCamp, HorizonConfig.HideMark hm)
        {
            if (targetCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_MID)
            {
                return false;
            }
            return this._campMarkers[TranslateCampToIndex(targetCamp)].HasHideMark(hm);
        }

        public override bool HasShowMark(COM_PLAYERCAMP targetCamp, HorizonConfig.ShowMark sm)
        {
            if (targetCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_MID)
            {
                return false;
            }
            return this._campMarkers[TranslateCampToIndex(targetCamp)].HasShowMark(sm);
        }

        public bool HasTranslucentMark(HorizonConfig.HideMark hm)
        {
            return ((this._translucentMarks & (((int) 1) << ((byte) hm))) > 0);
        }

        public override void Init()
        {
            base.Init();
            this._campMarkers = new CampMarker[2];
            for (int i = 0; i < this._campMarkers.Length; i++)
            {
                this._campMarkers[i] = new CampMarker();
            }
            this._translucentMarks = 0;
            this.m_bTranslucent = false;
            this.m_bExposed = new bool[2];
            Array.Clear(this.m_bExposed, 0, this.m_bExposed.Length);
            this.m_exposedPos = new VInt3[2];
            Array.Clear(this.m_exposedPos, 0, this.m_exposedPos.Length);
            this.m_exposeCampArr = new int[2];
            Array.Clear(this.m_exposeCampArr, 0, this.m_exposeCampArr.Length);
            this.ExposeRadiusCache = Horizon.QueryExposeRadius();
            this.InitExposeTimer();
            this.InitSubParObjList();
        }

        private void InitExposeTimer()
        {
            int num = 2;
            if (this.m_exposeTimerSeq == null)
            {
                this.m_exposeTimerSeq = new int[num];
            }
            if (this.m_showmarkTimerSeq == null)
            {
                this.m_showmarkTimerSeq = new int[num];
            }
            for (int i = 0; i < num; i++)
            {
                this.m_exposeTimerSeq[i] = -1;
                this.m_showmarkTimerSeq[i] = -1;
            }
        }

        private void InitSubParObjList()
        {
            if (this.SubParObjList_ == null)
            {
                this.SubParObjList_ = new ListView<GameObject>();
            }
        }

        private bool IsDuringExposing(COM_PLAYERCAMP inAttackeeCamp)
        {
            int index = TranslateCampToIndex(inAttackeeCamp);
            return (this.m_exposeTimerSeq[index] >= 0);
        }

        private bool IsDuringShowMark(COM_PLAYERCAMP inAttackeeCamp)
        {
            int index = TranslateCampToIndex(inAttackeeCamp);
            return (this.m_showmarkTimerSeq[index] >= 0);
        }

        private bool IsExposedFor(COM_PLAYERCAMP targetCamp)
        {
            if (targetCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_MID)
            {
                return false;
            }
            return this.m_bExposed[TranslateCampToIndex(targetCamp)];
        }

        public bool IsTransluent()
        {
            return (this._translucentMarks > 0);
        }

        public override bool IsVisibleFor(COM_PLAYERCAMP targetCamp)
        {
            if (targetCamp == base.actor.TheActorMeta.ActorCamp)
            {
                return true;
            }
            if (targetCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_MID)
            {
                return (!this.HasHideMark(COM_PLAYERCAMP.COM_PLAYERCAMP_1, HorizonConfig.HideMark.Skill) && !this.HasHideMark(COM_PLAYERCAMP.COM_PLAYERCAMP_2, HorizonConfig.HideMark.Skill));
            }
            return this._campMarkers[TranslateCampToIndex(targetCamp)].Visible;
        }

        private void OnExposeOverCampOne(int inTimeSeq)
        {
            if (inTimeSeq == this.m_exposeTimerSeq[0])
            {
                this.SetExposeMark(false, COM_PLAYERCAMP.COM_PLAYERCAMP_1, base.actor.ActorControl.DoesIgnoreAlreadyLit());
                Singleton<CTimerManager>.instance.RemoveTimer(inTimeSeq);
                this.m_exposeTimerSeq[0] = -1;
            }
        }

        private void OnExposeOverCampTwo(int inTimeSeq)
        {
            if (inTimeSeq == this.m_exposeTimerSeq[1])
            {
                this.SetExposeMark(false, COM_PLAYERCAMP.COM_PLAYERCAMP_2, base.actor.ActorControl.DoesIgnoreAlreadyLit());
                Singleton<CTimerManager>.instance.RemoveTimer(inTimeSeq);
                this.m_exposeTimerSeq[1] = -1;
            }
        }

        private void OnShowMarkOverCampOne(int inTimeSeq)
        {
            if (inTimeSeq == this.m_showmarkTimerSeq[0])
            {
                this.AddShowMark(COM_PLAYERCAMP.COM_PLAYERCAMP_1, HorizonConfig.ShowMark.Jungle, -1);
                Singleton<CTimerManager>.instance.RemoveTimer(inTimeSeq);
                this.m_showmarkTimerSeq[0] = -1;
            }
        }

        private void OnShowMarkOverCampTwo(int inTimeSeq)
        {
            if (inTimeSeq == this.m_showmarkTimerSeq[1])
            {
                this.AddShowMark(COM_PLAYERCAMP.COM_PLAYERCAMP_2, HorizonConfig.ShowMark.Jungle, -1);
                Singleton<CTimerManager>.instance.RemoveTimer(inTimeSeq);
                this.m_showmarkTimerSeq[1] = -1;
            }
        }

        public override void OnUse()
        {
            base.OnUse();
            this._campMarkers = null;
            this._translucentMarks = 0;
            this.m_bTranslucent = false;
            this.m_bExposed = null;
            this.m_exposedPos = null;
            this.m_exposeCampArr = null;
            this.m_exposeRadiusCache = -1;
            this.ClearExposeTimer();
            this.ClearSubParObjs();
        }

        public override void Reactive()
        {
            base.Reactive();
            if (this._campMarkers != null)
            {
                int length = this._campMarkers.Length;
                for (int i = 0; i < length; i++)
                {
                    this._campMarkers[i].Reactive();
                }
            }
            this._translucentMarks = 0;
            this.m_bTranslucent = false;
            Array.Clear(this.m_bExposed, 0, this.m_bExposed.Length);
            Array.Clear(this.m_exposedPos, 0, this.m_exposedPos.Length);
            Array.Clear(this.m_exposeCampArr, 0, this.m_exposeCampArr.Length);
            this.InitExposeTimer();
            this.InitSubParObjList();
        }

        private void RefreshTransluency(bool bForbidFade = false)
        {
            if (this.IsTransluent() != this.m_bTranslucent)
            {
                this.m_bTranslucent = !this.m_bTranslucent;
                this.RefreshTransluencyForce(bForbidFade);
            }
        }

        private void RefreshTransluencyForce(bool bForbidFade = false)
        {
            if (base.actor.MatHurtEffect != null)
            {
                base.actor.MatHurtEffect.SetTranslucent(this.m_bTranslucent, bForbidFade);
            }
        }

        private void RefreshVisible()
        {
            if (!Singleton<WatchController>.GetInstance().IsWatching)
            {
                Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
                if (hostPlayer != null)
                {
                    bool visible = base.actor.Visible;
                    base.actor.Visible = this.IsVisibleFor(hostPlayer.PlayerCamp);
                    if (visible != base.actor.Visible)
                    {
                        this.UpdateSubParObjVisibility(base.actor.Visible);
                    }
                }
            }
        }

        public override bool SetExposeMark(bool exposed, COM_PLAYERCAMP targetCamp, bool bIgnoreAlreadyLit)
        {
            if ((targetCamp == base.actor.TheActorMeta.ActorCamp) || (targetCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_MID))
            {
                return false;
            }
            if (targetCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT)
            {
                bool flag = false;
                for (int i = 0; i < this.m_bExposed.Length; i++)
                {
                    if ((!bIgnoreAlreadyLit || !exposed) || !this.IsVisibleFor(TranslateIndexToCamp(i)))
                    {
                        this.m_bExposed[i] = exposed;
                        if (exposed && (i != TranslateCampToIndex(base.actor.TheActorMeta.ActorCamp)))
                        {
                            this.m_exposedPos[i] = new VInt3(base.actor.location.x, base.actor.location.z, 0);
                        }
                        flag = true;
                    }
                }
                return flag;
            }
            if ((bIgnoreAlreadyLit && exposed) && this.IsVisibleFor(targetCamp))
            {
                return false;
            }
            int index = TranslateCampToIndex(targetCamp);
            this.m_bExposed[index] = exposed;
            if (exposed)
            {
                this.m_exposedPos[index] = new VInt3(base.actor.location.x, base.actor.location.z, 0);
            }
            return true;
        }

        public override void SetHideMark(COM_PLAYERCAMP targetCamp, HorizonConfig.HideMark hm, bool bSet)
        {
            if (targetCamp != COM_PLAYERCAMP.COM_PLAYERCAMP_MID)
            {
                if (targetCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT)
                {
                    for (int i = 0; i < this._campMarkers.Length; i++)
                    {
                        if (i != TranslateCampToIndex(base.actor.TheActorMeta.ActorCamp))
                        {
                            this._campMarkers[i].SetHideMark(hm, bSet);
                            Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
                            if ((hostPlayer != null) && (hostPlayer.PlayerCamp == TranslateIndexToCamp(i)))
                            {
                                this.RefreshVisible();
                            }
                        }
                    }
                }
                else
                {
                    this._campMarkers[TranslateCampToIndex(targetCamp)].SetHideMark(hm, bSet);
                    Player player2 = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
                    if ((player2 != null) && (targetCamp == player2.PlayerCamp))
                    {
                        this.RefreshVisible();
                    }
                }
            }
        }

        public override void SetTranslucentMark(HorizonConfig.HideMark hm, bool bSet, bool bForbidFade = false)
        {
            this.SetTranslucentMarkInternal(hm, bSet);
            this.RefreshTransluency(bForbidFade);
        }

        private void SetTranslucentMarkInternal(HorizonConfig.HideMark hm, bool bSet)
        {
            if (bSet)
            {
                this._translucentMarks = (byte) (this._translucentMarks | (((int) 1) << ((byte) hm)));
            }
            else
            {
                this._translucentMarks = (byte) (this._translucentMarks & ~(((int) 1) << ((byte) hm)));
            }
        }

        private void ShowAsAttacker(COM_PLAYERCAMP attackeeCamp, int inResetTime)
        {
            int index = TranslateCampToIndex(attackeeCamp);
            CTimerManager instance = Singleton<CTimerManager>.instance;
            if (this.IsDuringShowMark(attackeeCamp))
            {
                if (inResetTime <= 0)
                {
                    instance.ResetTimer(this.m_showmarkTimerSeq[index]);
                }
                else if (instance.GetLeftTime(this.m_showmarkTimerSeq[index]) < inResetTime)
                {
                    instance.ResetTimerTotalTime(this.m_showmarkTimerSeq[index], inResetTime);
                }
            }
            else
            {
                if (index == 0)
                {
                    this.m_showmarkTimerSeq[index] = instance.AddTimer(Horizon.QueryAttackShowMarkDuration(), 1, new CTimer.OnTimeUpHandler(this.OnShowMarkOverCampOne), true);
                }
                else
                {
                    this.m_showmarkTimerSeq[index] = instance.AddTimer(Horizon.QueryAttackShowMarkDuration(), 1, new CTimer.OnTimeUpHandler(this.OnShowMarkOverCampTwo), true);
                }
                this.AddShowMark(attackeeCamp, HorizonConfig.ShowMark.Jungle, 1);
            }
        }

        public static int TranslateCampToIndex(COM_PLAYERCAMP targetCamp)
        {
            return (((int) targetCamp) - 1);
        }

        public static COM_PLAYERCAMP TranslateIndexToCamp(int targetIndex)
        {
            return (COM_PLAYERCAMP) (targetIndex + 1);
        }

        public override void Uninit()
        {
            this.ClearExposeTimer();
            this.ClearSubParObjs();
            base.Uninit();
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

        private void UpdateSubParObjVisibility(bool inVisible)
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

        public int ExposeRadiusCache
        {
            get
            {
                return this.m_exposeRadiusCache;
            }
            private set
            {
                Singleton<GameFowManager>.instance.m_pFieldObj.UnrealToGridX(value, out this.m_exposeRadiusCache);
            }
        }

        internal class CampMarker
        {
            [CompilerGenerated]
            private byte[] <_hideMarks>k__BackingField;
            [CompilerGenerated]
            private byte[] <_showMarks>k__BackingField;

            public CampMarker()
            {
                this._hideMarks = new byte[2];
                this._showMarks = new byte[3];
            }

            public void AddHideMark(HorizonConfig.HideMark hm, int count)
            {
                if (count >= 0)
                {
                    this._hideMarks[(int) hm] = (byte) (this._hideMarks[(int) hm] + ((byte) count));
                }
                else
                {
                    byte num = (byte) -count;
                    if (this._hideMarks[(int) hm] < num)
                    {
                        this._hideMarks[(int) hm] = 0;
                    }
                    else
                    {
                        this._hideMarks[(int) hm] = (byte) (this._hideMarks[(int) hm] - num);
                    }
                }
            }

            public void AddShowMark(HorizonConfig.ShowMark sm, int count)
            {
                if (count >= 0)
                {
                    this._showMarks[(int) sm] = (byte) (this._showMarks[(int) sm] + ((byte) count));
                }
                else
                {
                    byte num = (byte) -count;
                    if (this._showMarks[(int) sm] < num)
                    {
                        this._showMarks[(int) sm] = 0;
                    }
                    else
                    {
                        this._showMarks[(int) sm] = (byte) (this._showMarks[(int) sm] - num);
                    }
                }
            }

            public bool HasHideMark()
            {
                int num = 2;
                for (int i = 0; i < num; i++)
                {
                    if (this._hideMarks[i] > 0)
                    {
                        return true;
                    }
                }
                return false;
            }

            public bool HasHideMark(HorizonConfig.HideMark hm)
            {
                return (this._hideMarks[(int) hm] > 0);
            }

            public bool HasShowMark()
            {
                int num = 3;
                for (int i = 0; i < num; i++)
                {
                    if (this._showMarks[i] > 0)
                    {
                        return true;
                    }
                }
                return false;
            }

            public bool HasShowMark(HorizonConfig.ShowMark sm)
            {
                return (this._showMarks[(int) sm] > 0);
            }

            public bool IsHideMarkOnly(HorizonConfig.HideMark hm)
            {
                int num = 0;
                int num2 = 0;
                for (int i = 0; i < this._hideMarks.Length; i++)
                {
                    if (i == hm)
                    {
                        num2 += this._hideMarks[i];
                    }
                    num += this._hideMarks[i];
                }
                return ((num2 > 0) && (num2 == num));
            }

            public void Reactive()
            {
                Array.Clear(this._hideMarks, 0, this._hideMarks.Length);
                Array.Clear(this._showMarks, 0, this._showMarks.Length);
            }

            public void SetHideMark(HorizonConfig.HideMark hm, bool bSet)
            {
                this._hideMarks[(int) hm] = !bSet ? ((byte) 0) : ((byte) 1);
            }

            public byte[] _hideMarks
            {
                [CompilerGenerated]
                get
                {
                    return this.<_hideMarks>k__BackingField;
                }
                [CompilerGenerated]
                private set
                {
                    this.<_hideMarks>k__BackingField = value;
                }
            }

            public byte[] _showMarks
            {
                [CompilerGenerated]
                get
                {
                    return this.<_showMarks>k__BackingField;
                }
                [CompilerGenerated]
                private set
                {
                    this.<_showMarks>k__BackingField = value;
                }
            }

            public bool Visible
            {
                get
                {
                    return (this.HasShowMark() || !this.HasHideMark());
                }
            }
        }
    }
}

