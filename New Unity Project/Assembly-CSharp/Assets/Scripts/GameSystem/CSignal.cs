namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.GameKernal;
    using Assets.Scripts.UI;
    using ResData;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class CSignal
    {
        public bool bSmall;
        public bool bUseCfgSound;
        public bool m_bUseSignal;
        private float m_duringTime;
        private GameObject m_effectInScene;
        public uint m_heroID;
        private float m_maxDuringTime;
        public uint m_playerID;
        public int m_signalID;
        public ResSignalInfo m_signalInfo;
        private GameObject m_signalInUi;
        private UIParticleInfo m_signalInUIEffect;
        private PoolObjHandle<ActorRoot> m_signalRelatedActor;
        private MinimapSys.ElementType m_type;
        public Vector3 m_worldPosition;

        public CSignal(PoolObjHandle<ActorRoot> followActor, int signalID, bool bUseSignal, bool bSmall, bool bUseCfgSound)
        {
            this.bSmall = true;
            this.m_playerID = 0;
            this.m_signalID = signalID;
            this.m_signalRelatedActor = followActor;
            this.bSmall = bSmall;
            this.bUseCfgSound = bUseCfgSound;
            this.m_worldPosition = Vector3.get_zero();
            this.m_bUseSignal = bUseSignal;
            this.m_effectInScene = null;
            this.m_signalInUIEffect = null;
            this.m_signalInUi = null;
        }

        public CSignal(uint playerID, int signalID, int worldPositionX, int worldPositionY, int worldPositionZ, bool bUseSignal, bool bSmall, bool bUseCfgSound)
        {
            this.bSmall = true;
            this.m_playerID = playerID;
            this.m_signalID = signalID;
            this.bSmall = bSmall;
            this.bUseCfgSound = bUseCfgSound;
            this.m_worldPosition = new Vector3((float) worldPositionX, (float) worldPositionY, (float) worldPositionZ);
            this.m_bUseSignal = bUseSignal;
            this.m_effectInScene = null;
            this.m_signalInUIEffect = null;
            this.m_signalInUi = null;
        }

        private bool bSignalFollowActor()
        {
            return ((((this.m_signalInfo != null) && (this.m_signalInfo.bSignalType == 1)) && (this.m_signalRelatedActor != 0)) && (this.m_signalInUi != null));
        }

        public void Dispose()
        {
            if (this.m_effectInScene != null)
            {
                Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(this.m_effectInScene);
                this.m_effectInScene = null;
            }
            MiniMapSysUT.RecycleMapGameObject(this.m_signalInUi);
            this.m_signalInUi = null;
            this.m_effectInScene = null;
            this.m_signalInUIEffect = null;
            this.m_bUseSignal = false;
        }

        public void Initialize(CUIFormScript formScript, ResSignalInfo signalInfo)
        {
            if (this.m_playerID > 0)
            {
                Player player = Singleton<GamePlayerCenter>.GetInstance().GetPlayer(this.m_playerID);
                if (player != null)
                {
                    this.m_signalRelatedActor = player.Captain;
                }
            }
            this.m_signalInfo = signalInfo;
            if ((this.m_signalInfo == null) || (formScript == null))
            {
                this.m_duringTime = 0f;
                this.m_maxDuringTime = 0f;
            }
            else
            {
                this.m_duringTime = 0f;
                this.m_maxDuringTime = this.m_signalInfo.bTime;
                if ((this.m_signalInfo.bSignalType == 0) && !string.IsNullOrEmpty(this.m_signalInfo.szSceneEffect))
                {
                    this.m_effectInScene = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD(this.m_signalInfo.szSceneEffect, true, SceneObjType.Temp, this.m_worldPosition);
                    this.m_effectInScene.CustomSetActive(true);
                }
                if (this.m_bUseSignal && !string.IsNullOrEmpty(this.m_signalInfo.szUIIcon))
                {
                    if (this.m_signalInUi == null)
                    {
                        this.m_signalInUi = MiniMapSysUT.GetSignalGameObject(true);
                    }
                    if (this.m_signalInUi != null)
                    {
                        float num;
                        float num2;
                        Vector3 worldPosition = this.m_worldPosition;
                        if ((this.m_signalInfo.bSignalType == 1) && (this.m_signalRelatedActor != 0))
                        {
                            worldPosition = (Vector3) this.m_signalRelatedActor.handle.location;
                        }
                        this.m_signalInUi.get_transform().set_position(MiniMapSysUT.Set3DUIWorldPos_ByScreenPoint(ref worldPosition, true, out num, out num2));
                        if ((!string.IsNullOrEmpty(this.m_signalInfo.szRealEffect) && (this.m_signalInUi != null)) && (Singleton<CBattleSystem>.instance.TheMinimapSys.CurMapType() == MinimapSys.EMapType.Mini))
                        {
                            Vector2 sigScreenPos = new Vector2(num, num2);
                            if (Singleton<CBattleSystem>.instance.FightForm.GetSignalPanel().CheckSignalPositionValid(sigScreenPos))
                            {
                                this.m_signalInUIEffect = Singleton<CUIParticleSystem>.instance.AddParticle(this.m_signalInfo.szRealEffect, (float) this.m_signalInfo.bTime, sigScreenPos);
                            }
                        }
                    }
                }
                if (this.bUseCfgSound)
                {
                    string str = StringHelper.UTF8BytesToString(ref this.m_signalInfo.szSound);
                    if (!string.IsNullOrEmpty(str))
                    {
                        Singleton<CSoundManager>.GetInstance().PlayBattleSound2D(str);
                    }
                }
            }
        }

        public bool IsNeedDisposed()
        {
            return (this.m_duringTime >= this.m_maxDuringTime);
        }

        public static void Preload(ref ActorPreloadTab preloadTab)
        {
            GameDataMgr.signalDatabin.Reload();
            ResSignalInfo info = null;
            Dictionary<long, object>.Enumerator enumerator = GameDataMgr.signalDatabin.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<long, object> current = enumerator.Current;
                info = (ResSignalInfo) current.Value;
                if ((info != null) && !string.IsNullOrEmpty(info.szRealEffect))
                {
                    preloadTab.AddSprite(info.szRealEffect);
                }
                if ((info != null) && !string.IsNullOrEmpty(info.szUIIcon))
                {
                    preloadTab.AddSprite(info.szUIIcon);
                }
            }
        }

        public void Update(CUIFormScript formScript, float deltaTime)
        {
            if (this.m_duringTime < this.m_maxDuringTime)
            {
                this.m_duringTime += deltaTime;
                if (this.bSignalFollowActor())
                {
                    float num;
                    float num2;
                    Vector3 location = (Vector3) this.m_signalRelatedActor.handle.location;
                    this.m_signalInUi.get_transform().set_position(MiniMapSysUT.Set3DUIWorldPos_ByScreenPoint(ref location, true, out num, out num2));
                    if ((this.m_signalInUIEffect != null) && (this.m_signalInUIEffect.parObj != null))
                    {
                        Vector2 screenPosition = new Vector2(num, num2);
                        Singleton<CUIParticleSystem>.GetInstance().SetParticleScreenPosition(this.m_signalInUIEffect, ref screenPosition);
                    }
                }
            }
        }
    }
}

