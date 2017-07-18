namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.GameKernal;
    using CSProtocol;
    using ResData;
    using System;

    public class PlayerKDA
    {
        public int CampPos;
        public bool IsComputer;
        public bool IsHost;
        private bool m_bDisconnect;
        private bool m_bHangup;
        private bool m_bRevive;
        private bool m_bRunaway;
        public uint m_Camp1TowerFirstAttackTime;
        public uint m_Camp2TowerFirstAttackTime;
        public uint m_firstMoveTime;
        private ListView<HeroKDA> m_HeroKDA = new ListView<HeroKDA>();
        public ListView<CHostHeroDamage> m_hostHeroDamage = new ListView<CHostHeroDamage>();
        public uint m_lastReviveTime;
        public int m_nReviveCount;
        public uint[] m_reviveMoveTime = new uint[20];
        public COM_PLAYERCAMP PlayerCamp;
        public uint PlayerId;
        public int PlayerLv;
        public string PlayerName;
        public string PlayerOpenId;
        public ulong PlayerUid;
        public uint PlayerVipLv;
        public int WorldId;

        public void clear()
        {
            ListView<HeroKDA>.Enumerator enumerator = this.m_HeroKDA.GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.unInit();
            }
            this.m_HeroKDA.Clear();
            if (this.m_hostHeroDamage != null)
            {
                ListView<CHostHeroDamage>.Enumerator enumerator2 = this.m_hostHeroDamage.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    if (enumerator2.Current != null)
                    {
                        enumerator2.Current.UnInit();
                    }
                }
                this.m_hostHeroDamage.Clear();
            }
            Singleton<EventRouter>.instance.RemoveEventHandler<Player>(EventID.PlayerRunAway, new Action<Player>(this.onPlayerRunAway));
            Singleton<EventRouter>.instance.RemoveEventHandler<HANGUP_TYPE, uint>(EventID.HangupNtf, new Action<HANGUP_TYPE, uint>(this, (IntPtr) this.OnHangupNtf));
            Singleton<EventRouter>.instance.RemoveEventHandler<bool, uint>(EventID.DisConnectNtf, new Action<bool, uint>(this, (IntPtr) this.OnDisConnect));
            Singleton<EventRouter>.instance.RemoveEventHandler<Player>(EventID.FirstMoved, new Action<Player>(this.OnFirstMoved));
            Singleton<EventRouter>.instance.RemoveEventHandler<Player>(EventID.PlayerReviveTime, new Action<Player>(this.OnReviveTime));
            Singleton<EventRouter>.instance.RemoveEventHandler<COM_PLAYERCAMP>(EventID.CampTowerFirstAttackTime, new Action<COM_PLAYERCAMP>(this.OnCampTowerFirstAttackTime));
        }

        public ListView<HeroKDA>.Enumerator GetEnumerator()
        {
            return this.m_HeroKDA.GetEnumerator();
        }

        public uint GetPlayerCoinAtTimeWithType(int iTimeIndex, KDAStat.GET_COIN_CHANNEL_TYPE type)
        {
            uint num = 0;
            ListView<HeroKDA>.Enumerator enumerator = this.m_HeroKDA.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current == null)
                {
                    continue;
                }
                uint num2 = 0;
                if (enumerator.Current.m_arrCoinInfos[(int) type] != null)
                {
                    while (!enumerator.Current.m_arrCoinInfos[(int) type].TryGetValue((uint) iTimeIndex, out num2))
                    {
                        iTimeIndex--;
                        if (iTimeIndex < 0)
                        {
                            break;
                        }
                    }
                }
                num += num2;
            }
            return num;
        }

        public void initialize(Player player)
        {
            if (player != null)
            {
                this.clear();
                Singleton<EventRouter>.instance.AddEventHandler<Player>(EventID.PlayerRunAway, new Action<Player>(this.onPlayerRunAway));
                Singleton<EventRouter>.instance.AddEventHandler<HANGUP_TYPE, uint>(EventID.HangupNtf, new Action<HANGUP_TYPE, uint>(this, (IntPtr) this.OnHangupNtf));
                Singleton<EventRouter>.instance.AddEventHandler<bool, uint>(EventID.DisConnectNtf, new Action<bool, uint>(this, (IntPtr) this.OnDisConnect));
                Singleton<EventRouter>.instance.AddEventHandler<Player>(EventID.FirstMoved, new Action<Player>(this.OnFirstMoved));
                Singleton<EventRouter>.instance.AddEventHandler<Player>(EventID.PlayerReviveTime, new Action<Player>(this.OnReviveTime));
                Singleton<EventRouter>.instance.AddEventHandler<COM_PLAYERCAMP>(EventID.CampTowerFirstAttackTime, new Action<COM_PLAYERCAMP>(this.OnCampTowerFirstAttackTime));
                this.PlayerId = player.PlayerId;
                this.PlayerCamp = player.PlayerCamp;
                this.PlayerName = player.Name;
                this.IsComputer = player.Computer;
                this.WorldId = player.LogicWrold;
                this.PlayerUid = player.PlayerUId;
                this.IsHost = (Singleton<GamePlayerCenter>.instance.GetHostPlayer() != null) && (player.PlayerId == Singleton<GamePlayerCenter>.instance.GetHostPlayer().PlayerId);
                this.PlayerLv = player.Level;
                this.PlayerVipLv = player.VipLv;
                this.PlayerOpenId = player.OpenId;
                this.CampPos = player.CampPos;
                ReadonlyContext<PoolObjHandle<ActorRoot>>.Enumerator enumerator = player.GetAllHeroes().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current != 0)
                    {
                        HeroKDA item = new HeroKDA();
                        item.Initialize(enumerator.Current, this.CampPos);
                        this.m_HeroKDA.Add(item);
                        if (this.IsHost && (this.m_hostHeroDamage != null))
                        {
                            CHostHeroDamage damage = new CHostHeroDamage();
                            damage.Init(enumerator.Current);
                            this.m_hostHeroDamage.Add(damage);
                        }
                    }
                }
            }
        }

        private void OnCampTowerFirstAttackTime(COM_PLAYERCAMP comp)
        {
            if (comp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
            {
                if (this.m_Camp1TowerFirstAttackTime == 0)
                {
                    this.m_Camp1TowerFirstAttackTime = (uint) Singleton<FrameSynchr>.instance.LogicFrameTick;
                }
            }
            else if ((comp == COM_PLAYERCAMP.COM_PLAYERCAMP_2) && (this.m_Camp2TowerFirstAttackTime == 0))
            {
                this.m_Camp2TowerFirstAttackTime = (uint) Singleton<FrameSynchr>.instance.LogicFrameTick;
            }
        }

        private void OnDisConnect(bool bDisconnect, uint playerId)
        {
            if (playerId == this.PlayerId)
            {
                this.m_bDisconnect = bDisconnect;
            }
        }

        private void OnFirstMoved(Player player)
        {
            if (player.PlayerId == this.PlayerId)
            {
                if (!this.m_bRevive)
                {
                    this.m_firstMoveTime = (uint) Singleton<FrameSynchr>.instance.LogicFrameTick;
                }
                else if (this.m_nReviveCount <= 20)
                {
                    this.m_reviveMoveTime[this.m_nReviveCount - 1] = ((uint) Singleton<FrameSynchr>.instance.LogicFrameTick) - this.m_lastReviveTime;
                }
            }
        }

        private void OnHangupNtf(HANGUP_TYPE hangupType, uint playerId)
        {
            if (playerId == this.PlayerId)
            {
                this.m_bHangup = hangupType == HANGUP_TYPE.HANGUP_START;
            }
        }

        private void onPlayerRunAway(Player runningMan)
        {
            if (runningMan.PlayerId == this.PlayerId)
            {
                this.m_bRunaway = true;
            }
        }

        private void OnReviveTime(Player player)
        {
            if (player.PlayerId == this.PlayerId)
            {
                this.m_bRevive = true;
                this.m_lastReviveTime = (uint) Singleton<FrameSynchr>.instance.LogicFrameTick;
                this.m_nReviveCount++;
                player.m_bMoved = false;
            }
        }

        public bool bDisconnect
        {
            get
            {
                return this.m_bDisconnect;
            }
        }

        public bool bHangup
        {
            get
            {
                return this.m_bHangup;
            }
        }

        public bool bRunaway
        {
            get
            {
                return this.m_bRunaway;
            }
        }

        public float KDAValue
        {
            get
            {
                float num = 0f;
                ListView<HeroKDA>.Enumerator enumerator = this.m_HeroKDA.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    num += enumerator.Current.KDAValue;
                }
                return num;
            }
        }

        public int LegendaryNum
        {
            get
            {
                int num = 0;
                ListView<HeroKDA>.Enumerator enumerator = this.m_HeroKDA.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    num += enumerator.Current.LegendaryNum;
                }
                return num;
            }
        }

        public float MvpValue
        {
            get
            {
                float num2 = ((float) GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_MVP_RATE_HEROHURT)) / 10000f;
                float num3 = ((float) GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_MVP_RATE_COIN)) / 10000f;
                float num4 = ((float) GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_MVP_RATE_GODLIKE)) / 10000f;
                float num5 = ((float) GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_MVP_RATE_TRIPLEKILL)) / 10000f;
                float num6 = ((float) GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_MVP_RATE_TQUATARYKILL)) / 10000f;
                float num7 = ((float) GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_MVP_RATE_PENTAKILL)) / 10000f;
                CPlayerKDAStat playerKDAStat = Singleton<BattleStatistic>.instance.m_playerKDAStat;
                float teamKDA = playerKDAStat.GetTeamKDA(this.PlayerCamp);
                int teamKillNum = playerKDAStat.GetTeamKillNum(this.PlayerCamp);
                int teamDeadNum = playerKDAStat.GetTeamDeadNum(this.PlayerCamp);
                int teamHurt = playerKDAStat.GetTeamHurt(this.PlayerCamp);
                int teamBeHurt = playerKDAStat.GetTeamBeHurt(this.PlayerCamp);
                int teamCoin = playerKDAStat.GetTeamCoin(this.PlayerCamp);
                float num14 = 0f;
                float num15 = 0f;
                float num16 = 0f;
                float num17 = 0f;
                float num18 = 0f;
                float num19 = 0f;
                float num20 = 0f;
                float num21 = 0f;
                if (teamKDA > 0f)
                {
                    num14 = (this.KDAValue * 1f) / teamKDA;
                }
                if (teamKillNum > 0)
                {
                    num15 = ((this.numKill + this.numAssist) * 1f) / ((float) teamKillNum);
                }
                if (teamKillNum > 0)
                {
                    num16 = (this.numKill * 1f) / ((float) teamKillNum);
                }
                if (teamDeadNum > 0)
                {
                    num17 = (this.numDead * 1f) / ((float) teamDeadNum);
                }
                if (teamHurt > 0)
                {
                    num18 = (this.TotalHurt * num2) / ((float) teamHurt);
                }
                if (teamBeHurt > 0)
                {
                    num19 = (this.TotalBeHurt * num2) / ((float) teamBeHurt);
                }
                if (teamCoin > 0)
                {
                    num20 = (this.TotalCoin * num3) / ((float) teamCoin);
                }
                if (this.LegendaryNum > 0)
                {
                    num21 += num4;
                }
                if (this.PentaKillNum > 0)
                {
                    num21 += num7;
                }
                else if (this.QuataryKillNum > 0)
                {
                    num21 += num6;
                }
                else if (this.TripleKillNum > 0)
                {
                    num21 += num5;
                }
                return (((((((num14 + num15) + num16) - num17) + num18) + num19) + num20) + num21);
            }
        }

        public int numAssist
        {
            get
            {
                int num = 0;
                ListView<HeroKDA>.Enumerator enumerator = this.m_HeroKDA.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    num += enumerator.Current.numAssist;
                }
                return num;
            }
        }

        public int numDead
        {
            get
            {
                int num = 0;
                ListView<HeroKDA>.Enumerator enumerator = this.m_HeroKDA.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    num += enumerator.Current.numDead;
                }
                return num;
            }
        }

        public int numDestroyBase
        {
            get
            {
                int num = 0;
                ListView<HeroKDA>.Enumerator enumerator = this.m_HeroKDA.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    num += enumerator.Current.numDestroyBase;
                }
                return num;
            }
        }

        public int numKill
        {
            get
            {
                int num = 0;
                ListView<HeroKDA>.Enumerator enumerator = this.m_HeroKDA.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    num += enumerator.Current.numKill;
                }
                return num;
            }
        }

        public int numKillMonster
        {
            get
            {
                int num = 0;
                ListView<HeroKDA>.Enumerator enumerator = this.m_HeroKDA.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    num += enumerator.Current.numKillMonster;
                }
                return num;
            }
        }

        public int numKillOrgan
        {
            get
            {
                int num = 0;
                ListView<HeroKDA>.Enumerator enumerator = this.m_HeroKDA.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    num += enumerator.Current.numKillOrgan;
                }
                return num;
            }
        }

        public int numKillSoldier
        {
            get
            {
                int num = 0;
                ListView<HeroKDA>.Enumerator enumerator = this.m_HeroKDA.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    num += enumerator.Current.numKillSoldier;
                }
                return num;
            }
        }

        public int PentaKillNum
        {
            get
            {
                int num = 0;
                ListView<HeroKDA>.Enumerator enumerator = this.m_HeroKDA.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    num += enumerator.Current.PentaKillNum;
                }
                return num;
            }
        }

        public int QuataryKillNum
        {
            get
            {
                int num = 0;
                ListView<HeroKDA>.Enumerator enumerator = this.m_HeroKDA.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    num += enumerator.Current.QuataryKillNum;
                }
                return num;
            }
        }

        public int TotalBeHurt
        {
            get
            {
                int num = 0;
                ListView<HeroKDA>.Enumerator enumerator = this.m_HeroKDA.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    num += enumerator.Current.hurtTakenByEnemy;
                }
                return num;
            }
        }

        public int TotalCoin
        {
            get
            {
                int num = 0;
                ListView<HeroKDA>.Enumerator enumerator = this.m_HeroKDA.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    num += enumerator.Current.TotalCoin;
                }
                return num;
            }
        }

        public int TotalHurt
        {
            get
            {
                int num = 0;
                ListView<HeroKDA>.Enumerator enumerator = this.m_HeroKDA.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    num += enumerator.Current.hurtToEnemy;
                }
                return num;
            }
        }

        public int TripleKillNum
        {
            get
            {
                int num = 0;
                ListView<HeroKDA>.Enumerator enumerator = this.m_HeroKDA.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    num += enumerator.Current.TripleKillNum;
                }
                return num;
            }
        }
    }
}

