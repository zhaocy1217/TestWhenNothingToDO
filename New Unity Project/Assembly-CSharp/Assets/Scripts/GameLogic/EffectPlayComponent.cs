namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic.GameKernal;
    using CSProtocol;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class EffectPlayComponent : LogicComponent
    {
        private bool m_bPlayed;
        private GameObject m_kingOfKillerEff;
        private GameObject m_skillGestureGuide;
        private const string s_dyingGoldEffectPath = "Prefab_Skill_Effects/Systems_Effects/GoldenCoin_UI_01";
        public static string s_heroHunHurtPath = "Prefab_Skill_Effects/tongyong_effects/tongyong_hurt/Hun_hurt_01";
        public static string s_heroSoulLevelUpEftPath = "Prefab_Skill_Effects/tongyong_effects/tongyong_hurt/shengji_tongongi_01";
        public static string s_heroSuckEftPath = "Prefab_Skill_Effects/tongyong_effects/tongyong_hurt/Hunqiu_01";
        private const string s_ImmediateReviveEftPath = "Prefab_Skill_Effects/tongyong_effects/Huanling_Effect/fuhuodun_buff_01";
        private const string s_IntimacyStateGuy = "Prefab_Skill_Effects/tongyong_effects/tongyong_hurt/born_back_reborn/chusheng_JY_01";
        private const string s_IntimacyStateLover = "Prefab_Skill_Effects/tongyong_effects/tongyong_hurt/born_back_reborn/chusheng_LR_01";
        private const string s_kingOfKiller = "Prefab_Skill_Effects/Systems_Effects/huangguan_buff_01";
        private const string s_skillGesture = "Prefab_Skill_Effects/tongyong_effects/Indicator/Arrow_3_move";
        private const string s_skillGesture3 = "Prefab_Skill_Effects/tongyong_effects/Indicator/Arrow_2_move";
        private const string s_skillGestureCancel = "Prefab_Skill_Effects/tongyong_effects/Indicator/Arrow_3_move_red";
        public static int s_suckSoulMSec = 0x3e8;
        private List<DuraEftPlayParam> soulSuckObjList = new List<DuraEftPlayParam>();

        public void ApplyIntimacyEffect()
        {
            if (!this.m_bPlayed)
            {
                Player player = Singleton<GamePlayerCenter>.instance.GetPlayer(this.actorPtr.handle.TheActorMeta.PlayerId);
                if ((player != null) && (player.IntimacyData != null))
                {
                    string str = string.Empty;
                    if (player.IntimacyData.state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY)
                    {
                        str = "Prefab_Skill_Effects/tongyong_effects/tongyong_hurt/born_back_reborn/chusheng_JY_01";
                    }
                    else if (player.IntimacyData.state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER)
                    {
                        str = "Prefab_Skill_Effects/tongyong_effects/tongyong_hurt/born_back_reborn/chusheng_LR_01";
                    }
                    if (!string.IsNullOrEmpty(str))
                    {
                        Vector3 pos = base.actor.myTransform.get_position();
                        GameObject pooledGameObject = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD(str, true, SceneObjType.ActionRes, pos);
                        if (pooledGameObject != null)
                        {
                            this.m_bPlayed = true;
                            pooledGameObject.get_transform().SetParent(base.actor.myTransform);
                            pooledGameObject.SetActive(true);
                            Singleton<CGameObjectPool>.GetInstance().RecycleGameObjectDelay(pooledGameObject, 0x1b58, null);
                            foreach (ParticleSystem system in pooledGameObject.GetComponentsInChildren<ParticleSystem>())
                            {
                                if (system != null)
                                {
                                    system.Play(true);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ClearVariables()
        {
            this.m_skillGestureGuide = null;
            this.m_kingOfKillerEff = null;
            for (int i = 0; i < this.soulSuckObjList.Count; i++)
            {
                DuraEftPlayParam param = this.soulSuckObjList[i];
                Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(param.EftObj);
            }
            this.soulSuckObjList.Clear();
        }

        public override void Deactive()
        {
            this.ClearVariables();
            base.Deactive();
        }

        public void EndKingOfKillerEffect()
        {
            if (this.m_kingOfKillerEff != null)
            {
                Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(this.m_kingOfKillerEff);
                this.m_kingOfKillerEff = null;
            }
        }

        public void EndSkillGestureEffect()
        {
            if (this.m_skillGestureGuide != null)
            {
                Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(this.m_skillGestureGuide);
                this.m_skillGestureGuide = null;
            }
        }

        public override void Fight()
        {
        }

        public override void Init()
        {
            base.Init();
            base.actor.ValueComponent.SoulLevelChgEvent += new ValueChangeDelegate(this.OnHeroSoulLevelChange);
            base.actor.ActorControl.eventActorDead += new ActorDeadEventHandler(this.onActorDead);
        }

        public override void LateUpdate(int nDelta)
        {
        }

        public void onActorDead(ref GameDeadEventParam prm)
        {
            if ((prm.bImmediateRevive && (base.actorPtr != 0)) && (base.actorPtr == prm.src))
            {
                GameObject go = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD("Prefab_Skill_Effects/tongyong_effects/Huanling_Effect/fuhuodun_buff_01", true, SceneObjType.ActionRes, this.actorPtr.handle.myTransform.get_position());
                if (go != null)
                {
                    string layerName = "Particles";
                    if (this.actorPtr.handle.gameObject.get_layer() == LayerMask.NameToLayer("Hide"))
                    {
                        layerName = "Hide";
                    }
                    go.SetLayer(layerName, false);
                    go.get_transform().SetParent(this.actorPtr.handle.myTransform);
                    ParticleSystem component = go.GetComponent<ParticleSystem>();
                    if (component != null)
                    {
                        component.Play(true);
                    }
                    Singleton<CGameObjectPool>.GetInstance().RecycleGameObjectDelay(go, 0x1388, null);
                }
            }
        }

        private void OnHeroSoulLevelChange()
        {
            if (((base.actorPtr != 0) && !this.actorPtr.handle.ActorControl.IsDeadState) && ((this.actorPtr.handle.Visible && this.actorPtr.handle.InCamera) && (this.actorPtr.handle.ValueComponent.actorSoulLevel > 1)))
            {
                Vector3 pos = base.actor.myTransform.get_position();
                pos = new Vector3(pos.x, pos.y + 0.24f, pos.z);
                Quaternion rot = Quaternion.Euler(-90f, 0f, 0f);
                bool isInit = false;
                string levelUpEftPath = ((HeroWrapper) this.actorPtr.handle.ActorControl).GetLevelUpEftPath(this.actorPtr.handle.ValueComponent.actorSoulLevel);
                GameObject pooledGameObject = null;
                if (!string.IsNullOrEmpty(levelUpEftPath))
                {
                    pooledGameObject = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD(levelUpEftPath, true, SceneObjType.ActionRes, pos, rot, out isInit);
                }
                if (null == pooledGameObject)
                {
                    pooledGameObject = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD(s_heroSoulLevelUpEftPath, true, SceneObjType.ActionRes, pos, rot, out isInit);
                }
                if (null != pooledGameObject)
                {
                    Singleton<CGameObjectPool>.GetInstance().RecycleGameObjectDelay(pooledGameObject, 0x1388, null);
                    Transform transform = (base.actor.ActorMesh == null) ? base.actor.myTransform : base.actor.ActorMesh.get_transform();
                    string layerName = "Particles";
                    if ((transform != null) && (transform.get_gameObject().get_layer() == LayerMask.NameToLayer("Hide")))
                    {
                        layerName = "Hide";
                    }
                    pooledGameObject.get_transform().SetParent(transform);
                    pooledGameObject.SetLayer(layerName, false);
                    Singleton<CSoundManager>.instance.PlayBattleSound("Level_Up", base.actorPtr, base.gameObject);
                }
            }
        }

        public override void OnUse()
        {
            base.OnUse();
            this.ClearVariables();
        }

        public void PlayDyingGoldEffect(PoolObjHandle<ActorRoot> inActor)
        {
            if (((inActor != 0) && (inActor.handle.CharInfo != null)) && (inActor.handle.gameObject != null))
            {
                float num = inActor.handle.CharInfo.iBulletHeight * 0.001f;
                Vector3 pos = inActor.handle.myTransform.get_localToWorldMatrix().MultiplyPoint(new Vector3(0f, num + 1f, 0f));
                Quaternion rot = Quaternion.Euler(-90f, 0f, 0f);
                GameObject pooledGameObject = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD("Prefab_Skill_Effects/Systems_Effects/GoldenCoin_UI_01", true, SceneObjType.ActionRes, pos, rot);
                if (pooledGameObject != null)
                {
                    Singleton<CGameObjectPool>.GetInstance().RecycleGameObjectDelay(pooledGameObject, 0x1388, null);
                }
            }
        }

        public void PlayHunHurtEft()
        {
            if (!base.actor.ActorControl.IsDeadState)
            {
                float num = base.actor.CharInfo.iBulletHeight * 0.001f;
                Vector3 pos = base.actor.myTransform.get_localToWorldMatrix().MultiplyPoint(new Vector3(0f, num, 0f));
                Quaternion rot = Quaternion.Euler(-90f, 0f, 0f);
                GameObject pooledGameObject = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD(s_heroHunHurtPath, true, SceneObjType.ActionRes, pos, rot);
                if (pooledGameObject != null)
                {
                    Singleton<CGameObjectPool>.GetInstance().RecycleGameObjectDelay(pooledGameObject, 0x1388, null);
                }
            }
        }

        public void PlaySuckSoulEft(PoolObjHandle<ActorRoot> src)
        {
            if ((src != 0) && ((ActorHelper.IsCaptainActor(ref this.actorPtr) && base.actor.Visible) && base.actor.InCamera))
            {
                float num = src.handle.CharInfo.iBulletHeight * 0.001f;
                Vector3 pos = src.handle.myTransform.get_localToWorldMatrix().MultiplyPoint(new Vector3(0f, 0f, num));
                Quaternion rot = Quaternion.Euler(-90f, 0f, 0f);
                GameObject obj2 = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD(s_heroSuckEftPath, true, SceneObjType.ActionRes, pos, rot);
                if (obj2 != null)
                {
                    DuraEftPlayParam item = new DuraEftPlayParam();
                    item.EftObj = obj2;
                    item.RemainMSec = s_suckSoulMSec;
                    this.soulSuckObjList.Add(item);
                }
            }
        }

        public static void Preload(ref ActorPreloadTab preloadTab)
        {
            preloadTab.AddParticle(s_heroSoulLevelUpEftPath);
            preloadTab.AddParticle(s_heroSuckEftPath);
            preloadTab.AddParticle(s_heroHunHurtPath);
            preloadTab.AddParticle("Prefab_Skill_Effects/Systems_Effects/GoldenCoin_UI_01");
            preloadTab.AddParticle("Prefab_Skill_Effects/tongyong_effects/Indicator/Arrow_3_move");
            preloadTab.AddParticle("Prefab_Skill_Effects/tongyong_effects/Indicator/Arrow_2_move");
            preloadTab.AddParticle("Prefab_Skill_Effects/tongyong_effects/Indicator/Arrow_3_move_red");
            preloadTab.AddParticle("Prefab_Skill_Effects/Systems_Effects/huangguan_buff_01");
            preloadTab.AddParticle("Prefab_Skill_Effects/tongyong_effects/Huanling_Effect/fuhuodun_buff_01");
            preloadTab.AddParticle("Prefab_Skill_Effects/tongyong_effects/tongyong_hurt/born_back_reborn/chusheng_JY_01");
            preloadTab.AddParticle("Prefab_Skill_Effects/tongyong_effects/tongyong_hurt/born_back_reborn/chusheng_LR_01");
            string str = "Prefab_Skill_Effects/tongyong_effects/Indicator/";
            preloadTab.AddParticle(str + "blin_01");
            preloadTab.AddParticle(str + "blin_01_a");
            preloadTab.AddParticle(str + "blin_01_b");
            preloadTab.AddParticle(str + "blin_01_c");
        }

        public void StartKingOfKillerEffect()
        {
            if (base.actorPtr != 0)
            {
                this.EndKingOfKillerEffect();
                Quaternion rot = Quaternion.LookRotation(Vector3.get_right());
                Vector3 pos = base.actor.myTransform.get_position();
                pos.y += 3.9f;
                GameObject obj2 = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD("Prefab_Skill_Effects/Systems_Effects/huangguan_buff_01", true, SceneObjType.ActionRes, pos, rot);
                if (obj2 != null)
                {
                    obj2.get_transform().SetParent(base.actor.myTransform);
                    obj2.SetActive(true);
                }
                this.m_kingOfKillerEff = obj2;
            }
        }

        public void StartSkillGestureEffect()
        {
            this.StartSkillGestureEffectShared(SkillSlotType.SLOT_SKILL_2, "Prefab_Skill_Effects/tongyong_effects/Indicator/Arrow_3_move");
        }

        public void StartSkillGestureEffect3()
        {
            this.StartSkillGestureEffectShared(SkillSlotType.SLOT_SKILL_3, "Prefab_Skill_Effects/tongyong_effects/Indicator/Arrow_2_move");
        }

        public void StartSkillGestureEffectCancel()
        {
            this.StartSkillGestureEffectShared(SkillSlotType.SLOT_SKILL_2, "Prefab_Skill_Effects/tongyong_effects/Indicator/Arrow_3_move_red");
        }

        private void StartSkillGestureEffectShared(SkillSlotType inSlotType, string guidePath)
        {
            if (base.actorPtr != 0)
            {
                SkillSlot skillSlot = base.actor.SkillControl.GetSkillSlot(inSlotType);
                if (skillSlot != null)
                {
                    this.EndSkillGestureEffect();
                    Quaternion rot = Quaternion.LookRotation(Vector3.get_right());
                    Vector3 pos = base.actor.myTransform.get_position();
                    pos.y += 0.3f;
                    GameObject obj2 = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD(guidePath, true, SceneObjType.ActionRes, pos, rot);
                    if (obj2 != null)
                    {
                        obj2.get_transform().SetParent(base.actor.myTransform);
                        obj2.SetActive(true);
                        skillSlot.skillIndicator.SetPrefabScaler(obj2, (int) skillSlot.SkillObj.cfgData.iGuideDistance);
                    }
                    this.m_skillGestureGuide = obj2;
                }
            }
        }

        public override void Uninit()
        {
            base.Uninit();
            base.actor.ValueComponent.SoulLevelChgEvent -= new ValueChangeDelegate(this.OnHeroSoulLevelChange);
            base.actor.ActorControl.eventActorDead -= new ActorDeadEventHandler(this.onActorDead);
        }

        public override void UpdateLogic(int delta)
        {
            if (this.soulSuckObjList.Count != 0)
            {
                int index = 0;
                while (index < this.soulSuckObjList.Count)
                {
                    DuraEftPlayParam eftParam = this.soulSuckObjList[index];
                    if (this.UpdateSuckSoulEftMove(ref eftParam, delta))
                    {
                        Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(eftParam.EftObj);
                        this.soulSuckObjList.RemoveAt(index);
                        this.PlayHunHurtEft();
                    }
                    else
                    {
                        this.soulSuckObjList[index] = eftParam;
                        index++;
                    }
                }
            }
        }

        private bool UpdateSuckSoulEftMove(ref DuraEftPlayParam eftParam, int delta)
        {
            Vector3 vector = base.actor.myTransform.get_position();
            vector.y += base.actor.CharInfo.iBulletHeight * 0.001f;
            if (eftParam.EftObj == null)
            {
                return true;
            }
            Vector3 vector2 = eftParam.EftObj.get_transform().get_position();
            eftParam.EftObj.get_transform().set_position(Vector3.Slerp(vector2, vector, ((float) delta) / ((float) eftParam.RemainMSec)));
            eftParam.RemainMSec -= delta;
            return (eftParam.RemainMSec <= 0);
        }
    }
}

