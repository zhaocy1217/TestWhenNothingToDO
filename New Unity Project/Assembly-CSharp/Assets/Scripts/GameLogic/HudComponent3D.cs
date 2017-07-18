namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.GameKernal;
    using Assets.Scripts.GameSystem;
    using Assets.Scripts.UI;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class HudComponent3D : LogicComponent
    {
        private Sprite3D _bloodDecImage;
        private int _curShield1;
        private bool _isDecreasingHp;
        private int _lastMaxBarValue;
        private GameObject _mountPoint;
        private GameObject _shieldGo;
        private float _shieldImagWidth;
        private GameObject[] _statusGo;
        private static string[] _statusResPath = new string[] { "Prefab_Skill_Effects/tongyong_effects/UI_fx/Yinshen_tongyong_01" };
        private const int BASE_ATTACK_INTERVAL = 0x4e20;
        public bool bBossHpBar;
        private Sprite3D bigTower_spt3d;
        private bool bReviveTimerShow;
        private int CurAniIndex_ = -1;
        private const float DecSpeed = 0.0086f;
        private const int DEPTH = 30;
        public static int forceUpdateFrameCount = 2;
        public static int heroFrameCount = 2;
        private Animation[] HeroProficiencyAni_ = new Animation[3];
        private const int HeroProficiencyDelta = 3;
        private static readonly string[] HeroProficiencyIconNames_ = new string[] { "Hero_Icon3_Ani", "Hero_Icon4_Ani", "Hero_Icon5_Ani" };
        private const int HeroProficiencyNum = 3;
        private GameObject[] HeroProficiencyObj_ = new GameObject[3];
        private static readonly string[] HeroProficiencyShowNames_ = new string[] { "Hero_Icon3_Show", "Hero_Icon4_Show", "Hero_Icon5_Show" };
        public const string HUD_BLOOD_BLACK_BAR_PREFAB = "UI3D/Battle/BlackBarBlack.prefab";
        public const string HUD_BLOOD_EYE_PREFAB = "UI3D/Battle/BloodHudEye.prefab";
        public const string HUD_BLOOD_PREFAB = "UI3D/Battle/BloodHud.prefab";
        public const string HUD_HERO_PREFAB = "UI3D/Battle/Blood_Bar_Hero.prefab";
        public const string HUD_HONOR_PREFAB = "UI3D/Battle/Img_Badge_{0}_{1}.prefab";
        public int hudHeight;
        private int hudHeightOffset;
        public HudCompType HudType;
        private ulong LastBaseAttackTime;
        private VInt3 m_actorForward;
        private Vector3 m_actorPos;
        private bool m_bHeroSameCamp;
        private List<GameObject> m_blackBars = new List<GameObject>();
        private GameObject m_bloodBackObj;
        private bool m_bloodDirty;
        private GameObject m_bloodForeObj;
        private Sprite3D m_bloodImage;
        private Vector3 m_bloodPos;
        private int m_dirtyFlags;
        private GameObject m_effectRoot_small;
        private GameObject m_energyForeObj;
        private Sprite3D m_energyImage;
        private UI3DEventCom m_evtCom;
        public GameObject m_exclamationObj;
        public float m_exclamationObjOffsetY;
        private Transform m_heroHead_big_Trans;
        private Transform m_heroHead_small_Trans;
        private GameObject m_heroIconBG_big;
        private GameObject m_heroIconBG_small;
        public GameObject m_honor;
        public Animation m_honorAni;
        private GameObject m_hud;
        private int m_hudFrameStampOnCreated;
        private bool m_hudLogicVisible;
        private GameObject m_inOutEquipShopHud;
        private CUIContainerScript m_inOutEquipShopHudContainer;
        private GameObject m_mapPointer_big;
        private Transform m_mapPointer_big_Trans;
        private GameObject m_mapPointer_small;
        private Transform m_mapPointer_small_Trans;
        private Sprite3D m_outOfControlBar;
        private GameObject m_outOfControlGo;
        private ListView<CoutofControlInfo> m_outofControlList;
        private bool m_pointerLogicVisible;
        private Sprite3D m_reviveTimerBar;
        private GameObject m_reviveTimerObj;
        private int m_reviveTotalTime;
        private Image m_signalImage;
        private SkillTimerInfo m_skillTimeInfo;
        private Sprite3D m_skillTimerBar;
        private GameObject m_skillTimerObj;
        private Sprite3D m_soulImage;
        private TextMesh m_soulLevel;
        private CUIContainerScript m_textHudContainer;
        private GameObject m_textHudNode;
        private bool m_textLogicVisible;
        private Sprite3D m_timerImg;
        private Sprite3D m_VoiceIconImage;
        public static float OFFSET_HEIGHT = 400f;
        private const int OVERLAY_RENDER_QUEUE = 0xfa0;
        private RectTransform rtTransform;
        private Sprite3D smallTower_spt3d;
        public static int soilderFrameCount = 4;
        private Text textCom;
        private int txt_hud_offset_x;
        private int txt_hud_offset_y;

        private void AddDirtyFlag(enDirtyFlag dirtyFlag)
        {
            this.m_dirtyFlags |= dirtyFlag;
        }

        public void AddForceUpdateFlag()
        {
            this.AddDirtyFlag(enDirtyFlag.ForcePositionInMap);
        }

        public void AddInEquipShopHud()
        {
            if (this.m_inOutEquipShopHudContainer == null)
            {
                this.m_inOutEquipShopHudContainer = Singleton<CBattleSystem>.GetInstance().GetInOutEquipShopHudContainer();
            }
            if ((this.m_inOutEquipShopHudContainer != null) && (this.m_inOutEquipShopHud == null))
            {
                int element = this.m_inOutEquipShopHudContainer.GetElement();
                if (element >= 0)
                {
                    this.m_inOutEquipShopHud = this.m_inOutEquipShopHudContainer.GetElement(element);
                    if (this.m_inOutEquipShopHud != null)
                    {
                        this.m_inOutEquipShopHud.CustomSetActive((base.actor.Visible && base.actor.InCamera) && (!base.actor.ActorControl.IsDeadState || base.actor.TheStaticData.TheBaseAttribute.DeadControl));
                    }
                }
            }
        }

        public override void Born(ActorRoot owner)
        {
            base.Born(owner);
            if (base.actor.CharInfo != null)
            {
                this.hudHeight = base.actor.CharInfo.hudHeight;
                this.HudType = base.actor.CharInfo.HudType;
            }
            this.m_hudFrameStampOnCreated = 0;
            this.m_hud = null;
            this.m_bloodImage = null;
            this.m_timerImg = null;
            this.m_blackBars.Clear();
            this.m_soulImage = null;
            this.m_energyImage = null;
            this.m_soulLevel = null;
            this.m_outOfControlBar = null;
            this.m_outofControlList = null;
            this.m_outOfControlGo = null;
            this.m_skillTimerObj = null;
            this.m_skillTimerBar = null;
            this.m_skillTimeInfo = null;
            this.m_reviveTimerObj = null;
            this.m_reviveTimerBar = null;
            this.m_bloodBackObj = null;
            this.m_bloodForeObj = null;
            this.m_energyForeObj = null;
            this.m_reviveTotalTime = 0;
            this.hudHeightOffset = 0;
            this.bReviveTimerShow = false;
            MiniMapSysUT.RecycleMapGameObject(this.m_mapPointer_small);
            this.m_mapPointer_small = null;
            this.m_mapPointer_small_Trans = null;
            MiniMapSysUT.RecycleMapGameObject(this.m_heroHead_small_Trans);
            this.m_heroHead_small_Trans = null;
            MiniMapSysUT.RecycleMapGameObject(this.m_mapPointer_big);
            this.m_mapPointer_big = null;
            this.m_mapPointer_big_Trans = null;
            MiniMapSysUT.RecycleMapGameObject(this.m_heroHead_big_Trans);
            this.m_heroHead_big_Trans = null;
            MiniMapSysUT.UnRegisterEventCom(this.m_evtCom);
            this.m_evtCom = null;
            this.bigTower_spt3d = null;
            this.smallTower_spt3d = null;
            this.m_signalImage = null;
            this.m_actorPos = Vector3.get_zero();
            this.m_bloodPos = Vector3.get_zero();
            this.m_actorForward = VInt3.zero;
            this.m_bHeroSameCamp = false;
            this.m_heroIconBG_small = null;
            this.m_heroIconBG_big = null;
            this.m_textHudContainer = null;
            this.m_textHudNode = null;
            this.textCom = null;
            this.txt_hud_offset_x = 0;
            this.txt_hud_offset_y = 0;
            this.rtTransform = null;
            this.m_inOutEquipShopHudContainer = null;
            this.m_inOutEquipShopHud = null;
            this.m_effectRoot_small = null;
            this.m_dirtyFlags = 0;
        }

        private void ClearHeroProficiency()
        {
            Array.Clear(this.HeroProficiencyAni_, 0, this.HeroProficiencyAni_.Length);
            Array.Clear(this.HeroProficiencyObj_, 0, this.HeroProficiencyObj_.Length);
            this.CurAniIndex_ = -1;
        }

        public GameObject CreateSignalGameObject(string singlePath, Vector3 worldPosition)
        {
            GameObject obj2 = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD(singlePath, true, SceneObjType.Temp, worldPosition);
            obj2.CustomSetActive(true);
            return obj2;
        }

        public override void Deactive()
        {
            if ((this.HudType != HudCompType.Type_Hide) && (this.m_hud != null))
            {
                this.m_hud.CustomSetActive(false);
            }
            this.setHudLogicVisible(false);
            this.setPointerVisible(false, true);
            MiniMapSysUT.RecycleMapGameObject(this.m_mapPointer_small);
            this.m_mapPointer_small = null;
            this.m_mapPointer_small_Trans = null;
            MiniMapSysUT.RecycleMapGameObject(this.m_heroHead_small_Trans);
            this.m_heroHead_small_Trans = null;
            MiniMapSysUT.RecycleMapGameObject(this.m_mapPointer_big);
            this.m_mapPointer_big = null;
            this.m_mapPointer_big_Trans = null;
            MiniMapSysUT.RecycleMapGameObject(this.m_heroHead_big_Trans);
            this.m_heroHead_big_Trans = null;
            MiniMapSysUT.UnRegisterEventCom(this.m_evtCom);
            this.m_evtCom = null;
            base.Deactive();
        }

        public void EndHonorAni()
        {
            if ((this.m_honor != null) && (this.m_honorAni != null))
            {
                if (this.m_honorAni.get_isPlaying())
                {
                    this.m_honorAni.Stop();
                }
                if (this.m_honorAni.get_enabled())
                {
                    this.m_honorAni.set_enabled(false);
                }
                if (this.m_honor.get_layer() != LayerMask.NameToLayer("Hide"))
                {
                    this.m_honor.SetLayer("Hide", false);
                }
            }
        }

        public override void Fight()
        {
            if (!MonoSingleton<GameFramework>.instance.EditorPreviewMode && !GameObjMgr.isPreSpawnActors)
            {
                if ((this.HudType != HudCompType.Type_Hide) && (this.m_hud != null))
                {
                    this.m_hud.CustomSetActive(true);
                }
                this.ResetHudUI();
                DragonIcon.Check_Dragon_Born_Evt(base.actor, true);
            }
        }

        private void FillHeroProficiencyAni(int index)
        {
            GameObject obj2 = this.m_hud.get_transform().Find(HeroProficiencyIconNames_[index]).get_gameObject();
            Animation component = obj2.GetComponent<Animation>();
            obj2.CustomSetActive(false);
            this.HeroProficiencyAni_[index] = component;
            this.HeroProficiencyObj_[index] = obj2;
        }

        public static Quaternion GetActorForward_MiniMap(ref VInt3 forward)
        {
            float num = (Mathf.Atan2((float) forward.z, (float) forward.x) * 57.29578f) - 90f;
            if (Singleton<BattleLogic>.instance.GetCurLvelContext().m_isCameraFlip)
            {
                num -= 180f;
            }
            return Quaternion.AngleAxis(num, Vector3.get_forward());
        }

        public Sprite3D GetBigTower_Spt3D()
        {
            return this.bigTower_spt3d;
        }

        private Transform GetHudPanel(HudCompType hudType, bool isHostCamp)
        {
            string str = "Unknown_Panel";
            if (null == Moba_Camera.currentMobaCamera)
            {
                return null;
            }
            Camera currentCamera = Singleton<Camera_UI3D>.GetInstance().GetCurrentCamera();
            if (currentCamera == null)
            {
                return null;
            }
            Transform transform = currentCamera.get_transform().Find("Hud");
            if (transform == null)
            {
                transform = new GameObject("Hud").get_transform();
                transform.set_parent(currentCamera.get_transform());
                transform.set_localPosition(Vector3.get_zero());
                transform.set_localRotation(Quaternion.get_identity());
                transform.set_localScale(Vector3.get_one());
                string[] strArray = new string[] { enBattleFormWidget.Panel_EnemySoliderHud.ToString(), enBattleFormWidget.Panel_SelfSoliderHud.ToString(), enBattleFormWidget.Panel_EnemyHeroHud.ToString(), enBattleFormWidget.Panel_SelfHeroHud.ToString(), str };
                for (int i = 0; i < strArray.Length; i++)
                {
                    GameObject obj3 = new GameObject(strArray[i]);
                    Transform transform2 = obj3.get_transform();
                    transform2 = obj3.get_transform();
                    transform2.set_parent(transform);
                    transform2.set_localPosition(Vector3.get_zero());
                    transform2.set_localRotation(Quaternion.get_identity());
                    transform2.set_localScale(Vector3.get_one());
                }
            }
            string str2 = string.Empty;
            if (hudType == HudCompType.Type_Soldier)
            {
                str2 = !isHostCamp ? enBattleFormWidget.Panel_EnemySoliderHud.ToString() : enBattleFormWidget.Panel_SelfSoliderHud.ToString();
            }
            else if ((hudType == HudCompType.Type_Hero) || (hudType == HudCompType.Type_Organ))
            {
                str2 = !isHostCamp ? enBattleFormWidget.Panel_EnemyHeroHud.ToString() : enBattleFormWidget.Panel_SelfHeroHud.ToString();
            }
            if (string.IsNullOrEmpty(str2))
            {
                str2 = str;
            }
            Transform transform3 = transform.Find(str2);
            if (transform3 == null)
            {
            }
            return transform3;
        }

        public CUIContainerScript GetMapPointerContainer(bool bMiniMap)
        {
            MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
            if (theMinimapSys == null)
            {
                goto Label_022C;
            }
            GameObject mmpcJungle = null;
            if (base.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
            {
                mmpcJungle = !bMiniMap ? theMinimapSys.bmpcHero : theMinimapSys.mmpcHero;
            }
            else if (base.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_EYE)
            {
                mmpcJungle = !bMiniMap ? theMinimapSys.bmpcEye : theMinimapSys.mmpcEye;
            }
            else if (base.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster)
            {
                if (base.actor.ActorControl.GetActorSubType() == 2)
                {
                    switch (base.actor.ActorControl.GetActorSubSoliderType())
                    {
                        case 8:
                        case 9:
                        case 13:
                        case 7:
                            return null;

                        case 11:
                            mmpcJungle = !bMiniMap ? theMinimapSys.bmpcRedBuff : theMinimapSys.mmpcRedBuff;
                            goto Label_0219;

                        case 10:
                            mmpcJungle = !bMiniMap ? theMinimapSys.bmpcBlueBuff : theMinimapSys.mmpcBlueBuff;
                            goto Label_0219;
                    }
                    if (bMiniMap)
                    {
                        mmpcJungle = theMinimapSys.mmpcJungle;
                    }
                    else
                    {
                        mmpcJungle = theMinimapSys.bmpcJungle;
                    }
                }
                else if (bMiniMap)
                {
                    mmpcJungle = !base.actor.IsHostCamp() ? theMinimapSys.mmpcEnemy : theMinimapSys.mmpcAlies;
                }
                else
                {
                    mmpcJungle = !base.actor.IsHostCamp() ? theMinimapSys.bmpcEnemy : theMinimapSys.bmpcAlies;
                }
            }
            else if (base.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ)
            {
                if (((base.actor.TheStaticData.TheOrganOnlyInfo.OrganType != 1) && (base.actor.TheStaticData.TheOrganOnlyInfo.OrganType != 2)) && (base.actor.TheStaticData.TheOrganOnlyInfo.OrganType != 4))
                {
                    return null;
                }
                mmpcJungle = !bMiniMap ? theMinimapSys.bmpcOrgan : theMinimapSys.mmpcOrgan;
            }
        Label_0219:
            if (mmpcJungle != null)
            {
                return mmpcJungle.GetComponent<CUIContainerScript>();
            }
        Label_022C:
            return null;
        }

        public Vector3 GetSmallMapPointer_WorldPosition()
        {
            if (this.m_mapPointer_small != null)
            {
                return this.m_mapPointer_small.get_transform().get_position();
            }
            return new Vector3(0f, 0f, 0f);
        }

        public Sprite3D GetSmallTower_Spt3D()
        {
            return this.smallTower_spt3d;
        }

        private bool HasDirtyFlag(enDirtyFlag dirtyFlag)
        {
            return ((this.m_dirtyFlags & dirtyFlag) != 0);
        }

        public bool HasStatus(StatusHudType st)
        {
            DebugHelper.Assert(this._statusGo != null, "_statusGo ==null");
            int index = (int) st;
            return ((this._statusGo != null) && ((this._statusGo[index] != null) && this._statusGo[index].get_activeSelf()));
        }

        public void HideReviveTimer()
        {
            this.m_reviveTimerObj.CustomSetActive(false);
            this.m_bloodBackObj.CustomSetActive(true);
            this.m_bloodForeObj.CustomSetActive(true);
            this.m_energyForeObj.CustomSetActive(true);
            if (this.m_reviveTimerBar != null)
            {
                this.m_reviveTimerBar.fillAmount = 0f;
            }
            this.m_reviveTotalTime = 0;
            this.hudHeight -= this.hudHeightOffset;
            if (this.hudHeightOffset > 0)
            {
                this.ResetStatusHeight(this.hudHeight);
            }
            this.hudHeightOffset = 0;
            this.bReviveTimerShow = false;
        }

        public void HideStatus(StatusHudType st)
        {
            DebugHelper.Assert(this._statusGo != null, "_statusGo ==null");
            int index = (int) st;
            GameObject obj2 = (this._statusGo == null) ? null : this._statusGo[index];
            if (obj2 != null)
            {
                obj2.CustomSetActive(false);
            }
        }

        private void HudInit_MapPointer()
        {
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            if (this.m_mapPointer_small == null)
            {
                this.initHudIcon(curLvelContext, true, out this.m_mapPointer_small);
            }
            if (this.m_mapPointer_big == null)
            {
                this.initHudIcon(curLvelContext, false, out this.m_mapPointer_big);
            }
            MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
            if (theMinimapSys != null)
            {
                this.m_effectRoot_small = theMinimapSys.mmpcEffect;
            }
            if (curLvelContext != null)
            {
                this.setPointerVisible(curLvelContext.IsMobaMode(), true);
            }
        }

        public override void Init()
        {
            base.Init();
        }

        private void Init_TextHud(int hudSequence, bool bCenter = false)
        {
            this.m_textHudNode = this.m_textHudContainer.GetElement(hudSequence);
            if (this.m_textHudNode != null)
            {
                this.textCom = Utility.GetComponetInChild<Text>(this.m_textHudNode, "bg/Text");
                this.m_textHudNode.CustomSetActive(true);
            }
        }

        private void initHudIcon(SLevelContext levelContext, bool bMiniMap, out GameObject mapPointer)
        {
            mapPointer = null;
            this.m_bHeroSameCamp = base.actor.IsHostCamp();
            MiniMapSysUT.UnRegisterEventCom(this.m_evtCom);
            this.m_evtCom = null;
            mapPointer = MiniMapSysUT.GetMapGameObject(base.actor, bMiniMap, out this.m_evtCom);
            if (mapPointer != null)
            {
                mapPointer.CustomSetActive(false);
                if (bMiniMap)
                {
                    this.m_mapPointer_small_Trans = mapPointer.get_transform();
                }
                else
                {
                    this.m_mapPointer_big_Trans = mapPointer.get_transform();
                }
                Transform transform = mapPointer.get_transform();
                if (transform != null)
                {
                    if (base.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
                    {
                        GameObject obj2 = mapPointer.get_transform().Find("bg").get_gameObject();
                        if (bMiniMap)
                        {
                            this.m_heroIconBG_small = obj2;
                        }
                        else
                        {
                            this.m_heroIconBG_big = obj2;
                        }
                        DebugHelper.Assert(GameDataMgr.heroDatabin.GetDataByKey((uint) base.actor.TheActorMeta.ConfigId) != null);
                        string path = KillNotifyUT.GetHero_Icon(base.actor, true);
                        bool bSelf = base.actor.TheActorMeta.PlayerId == Singleton<GamePlayerCenter>.GetInstance().HostPlayerId;
                        GameObject obj3 = MiniMapSysUT.GetHeroIconObj(path, bMiniMap, bSelf, this.m_bHeroSameCamp);
                        if (obj3 != null)
                        {
                            obj3.CustomSetActive(!bMiniMap ? this.m_mapPointer_big_Trans.get_gameObject().get_activeSelf() : this.m_mapPointer_small_Trans.get_gameObject().get_activeSelf());
                            if (bMiniMap)
                            {
                                this.m_heroHead_small_Trans = obj3.get_transform();
                            }
                            else
                            {
                                this.m_heroHead_big_Trans = obj3.get_transform();
                            }
                        }
                        this.m_actorPos = base.actor.myTransform.get_position();
                        this.UpdateUIMap(ref this.m_actorPos);
                        BackCityCom_3DUI.SetVisible(mapPointer, false);
                    }
                    else if (base.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ)
                    {
                        Transform transform2 = mapPointer.get_transform().Find("icon");
                        if (transform2 != null)
                        {
                            Sprite3D component = transform2.GetComponent<Sprite3D>();
                            if (bMiniMap)
                            {
                                this.smallTower_spt3d = component;
                            }
                            else
                            {
                                this.bigTower_spt3d = component;
                            }
                        }
                        if ((base.actor.IsHostCamp() && (base.actor.TheStaticData.TheOrganOnlyInfo.OrganType == 2)) && !bMiniMap)
                        {
                            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorBeAttack, new RefAction<DefaultGameEventParam>(this.OnBaseAttacked));
                            Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorBeAttack, new RefAction<DefaultGameEventParam>(this.OnBaseAttacked));
                        }
                        transform.SetAsFirstSibling();
                        this.m_actorPos = base.actor.myTransform.get_position();
                        if ((levelContext != null) && levelContext.IsMobaMode())
                        {
                            this.UpdateUIMap(ref this.m_actorPos);
                        }
                    }
                    else if (base.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_EYE)
                    {
                        string prefabPath = !base.actor.IsHostCamp() ? MinimapSys.enemy_Eye : MinimapSys.self_Eye;
                        mapPointer.get_transform().Find("eye").get_gameObject().GetComponent<Image>().SetSprite(prefabPath, Singleton<CBattleSystem>.GetInstance().FormScript, true, false, false, false);
                        transform.SetAsFirstSibling();
                        this.m_actorPos = base.actor.myTransform.get_position();
                        this.UpdateUIMap(ref this.m_actorPos);
                    }
                    else if (base.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster)
                    {
                        switch (base.actor.ActorControl.GetActorSubSoliderType())
                        {
                            case 8:
                            case 9:
                            case 7:
                            case 13:
                                return;
                        }
                        transform.SetAsFirstSibling();
                        if ((levelContext != null) && levelContext.IsMobaMode())
                        {
                            this.m_actorPos = base.actor.myTransform.get_position();
                            this.UpdateUIMap(ref this.m_actorPos);
                        }
                    }
                    mapPointer.CustomSetActive(false);
                    this.m_pointerLogicVisible = false;
                }
            }
        }

        private void InitHudUI()
        {
            this.m_hudFrameStampOnCreated = CUIManager.s_uiSystemRenderFrameCounter;
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            if ((this.HudType != HudCompType.Type_Hide) && (this.m_hud == null))
            {
                bool flag = ((base.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero) || (this.HudType == HudCompType.Type_Hero)) && Singleton<BattleLogic>.GetInstance().m_LevelContext.IsSoulGrow();
                string prefabFullPath = !flag ? "UI3D/Battle/BloodHud.prefab" : "UI3D/Battle/Blood_Bar_Hero.prefab";
                if (this.HudType == HudCompType.Type_Eye)
                {
                    prefabFullPath = "UI3D/Battle/BloodHudEye.prefab";
                }
                bool flag2 = ActorHelper.IsHostCtrlActor(ref this.actorPtr);
                this.m_hud = Singleton<CGameObjectPool>.GetInstance().GetGameObject(prefabFullPath, enResourceType.BattleScene);
                DebugHelper.Assert(this.m_hud != null, "wtf?");
                if (this.m_hud == null)
                {
                    return;
                }
                this.m_timerImg = null;
                if (flag)
                {
                    Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int, int, int>("HeroSoulExpChange", new Action<PoolObjHandle<ActorRoot>, int, int, int>(this, (IntPtr) this.onSoulExpChange));
                    Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int, int>("HeroEnergyChange", new Action<PoolObjHandle<ActorRoot>, int, int>(this, (IntPtr) this.onEnergyExpChange));
                    Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int>("HeroSoulLevelChange", new Action<PoolObjHandle<ActorRoot>, int>(this, (IntPtr) this.onSoulLvlChange));
                    Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<LimitMoveEventParam>(GameSkillEventDef.AllEvent_LimitMove, new GameSkillEvent<LimitMoveEventParam>(this.OnPlayerLimitMove));
                    Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<LimitMoveEventParam>(GameSkillEventDef.AllEvent_CancelLimitMove, new GameSkillEvent<LimitMoveEventParam>(this.OnPlayerCancelLimitMove));
                    Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<SkillTimerEventParam>(GameSkillEventDef.AllEvent_SetSkillTimer, new GameSkillEvent<SkillTimerEventParam>(this.OnPlayerSkillTimer));
                    this.m_bloodImage = this.m_hud.get_transform().Find("BloodFore").GetComponent<Sprite3D>();
                    DebugHelper.Assert(this.m_bloodImage != null);
                    if (this.m_bloodImage != null)
                    {
                        if (flag2)
                        {
                            this.m_bloodImage.spriteName = "Battle_greenHp";
                        }
                        else if (base.actor.IsHostCamp())
                        {
                            this.m_bloodImage.spriteName = !Singleton<WatchController>.GetInstance().IsWatching ? "Battle_blueHp" : "Battle_greenHp";
                        }
                        else
                        {
                            this.m_bloodImage.spriteName = "Battle_redHp";
                        }
                        this._shieldGo = this.m_bloodImage.get_gameObject().get_transform().FindChild("Shield").get_gameObject();
                        this._shieldImagWidth = this.m_bloodImage.width;
                        this._shieldGo.CustomSetActive(false);
                        this._bloodDecImage = this.m_hud.get_transform().FindChild("BloodBack").get_gameObject().GetComponent<Sprite3D>();
                        this._curShield1 = 0;
                        if (base.actor.ValueComponent.actorHpTotal > 0)
                        {
                            this.m_bloodImage.fillAmount = ((float) base.actor.ValueComponent.actorHp) / ((float) base.actor.ValueComponent.actorHpTotal);
                        }
                        else
                        {
                            this.m_bloodImage.fillAmount = 0f;
                        }
                        int actorHpTotal = base.actor.ValueComponent.actorHpTotal;
                        if (this.IsPlayerCopy())
                        {
                            MonsterWrapper actorControl = base.actor.ActorControl as MonsterWrapper;
                            if ((actorControl != null) && (actorControl.hostActor != 0))
                            {
                                actorHpTotal = actorControl.hostActor.handle.ValueComponent.actorHpTotal;
                            }
                        }
                        this.SetBlackBar(actorHpTotal);
                        if (this._bloodDecImage != null)
                        {
                            this._bloodDecImage.fillAmount = this.m_bloodImage.fillAmount;
                        }
                    }
                    this.m_soulImage = this.m_hud.get_transform().Find("SoulFore").GetComponent<Sprite3D>();
                    DebugHelper.Assert(this.m_soulImage != null);
                    if (this.m_soulImage != null)
                    {
                        this.m_soulImage.fillAmount = 0f;
                    }
                    this.m_soulLevel = this.m_hud.get_transform().Find("SoulLevel").GetComponent<TextMesh>();
                    DebugHelper.Assert(this.m_soulLevel != null);
                    if (this.m_soulLevel != null)
                    {
                        this.m_soulLevel.set_text("0");
                        this.m_soulLevel.get_gameObject().GetComponent<MeshRenderer>().get_sharedMaterial().set_renderQueue(0x1194);
                    }
                    GameObject obj2 = Utility.FindChild(this.m_hud, "EnergyFore");
                    if (obj2 != null)
                    {
                        this.m_energyImage = obj2.GetComponent<Sprite3D>();
                    }
                    DebugHelper.Assert(this.m_energyImage != null);
                    if (this.m_energyImage != null)
                    {
                        if (!base.actor.ValueComponent.IsEnergyType(EnergyType.NoneResource))
                        {
                            this.m_energyImage.spriteName = EnergyCommon.GetSpriteName((int) base.actor.ValueComponent.mEnergy.energyType);
                            if (base.actor.ValueComponent.actorEpTotal > 0)
                            {
                                this.m_energyImage.fillAmount = ((float) base.actor.ValueComponent.actorEp) / ((float) base.actor.ValueComponent.actorEpTotal);
                            }
                            else
                            {
                                this.m_energyImage.fillAmount = 0f;
                            }
                        }
                        else
                        {
                            obj2.CustomSetActive(false);
                        }
                    }
                    this.m_outOfControlGo = this.m_hud.get_transform().Find("OutOfControl").get_gameObject();
                    this.m_outOfControlBar = this.m_outOfControlGo.get_transform().Find("OutOfControlFore").GetComponent<Sprite3D>();
                    DebugHelper.Assert(this.m_outOfControlBar != null);
                    if (this.m_outOfControlBar != null)
                    {
                        this.m_outOfControlGo.CustomSetActive(false);
                        this.m_outOfControlBar.fillAmount = 0f;
                    }
                    this.m_outofControlList = new ListView<CoutofControlInfo>();
                    this.m_skillTimerObj = Utility.FindChild(this.m_hud, "SkillTimer");
                    if (this.m_skillTimerObj != null)
                    {
                        this.m_skillTimerObj.CustomSetActive(false);
                        this.m_skillTimerBar = Utility.GetComponetInChild<Sprite3D>(this.m_skillTimerObj, "SkillTimerBar");
                        if (this.m_skillTimerBar != null)
                        {
                            this.m_skillTimerBar.fillAmount = 0f;
                        }
                    }
                    this.m_skillTimeInfo = new SkillTimerInfo(0, 0, 0L);
                    this.m_reviveTimerObj = Utility.FindChild(this.m_hud, "ReviveTimer");
                    if (this.m_reviveTimerObj != null)
                    {
                        this.m_reviveTimerObj.CustomSetActive(false);
                        this.m_reviveTimerBar = this.m_reviveTimerObj.GetComponent<Sprite3D>();
                        if (this.m_reviveTimerBar != null)
                        {
                            this.m_reviveTimerBar.fillAmount = 0f;
                        }
                    }
                    this.m_bloodBackObj = Utility.FindChild(this.m_hud, "BloodBack");
                    this.m_bloodForeObj = Utility.FindChild(this.m_hud, "BloodFore");
                    this.m_energyForeObj = Utility.FindChild(this.m_hud, "EnergyFore");
                    this.FillHeroProficiencyAni(0);
                    this.FillHeroProficiencyAni(1);
                    this.FillHeroProficiencyAni(2);
                    bool flag3 = (curLvelContext != null) && curLvelContext.IsMobaMode();
                    TextMesh component = this.m_hud.get_transform().Find("PlayerName").GetComponent<TextMesh>();
                    Player ownerPlayer = ActorHelper.GetOwnerPlayer(ref this.actorPtr);
                    if (this.IsPlayerCopy() && !base.actor.IsHostCamp())
                    {
                        MonsterWrapper wrapper2 = base.actor.ActorControl as MonsterWrapper;
                        if ((wrapper2 != null) && (wrapper2.hostActor != 0))
                        {
                            ownerPlayer = ActorHelper.GetOwnerPlayer(ref wrapper2.hostActor);
                        }
                    }
                    DebugHelper.Assert(component != null);
                    if (component != null)
                    {
                        if (flag3 && (ownerPlayer != null))
                        {
                            component.set_text(ownerPlayer.Name);
                            component.get_gameObject().CustomSetActive(true);
                        }
                        else
                        {
                            component.get_gameObject().CustomSetActive(false);
                        }
                        component.get_gameObject().GetComponent<MeshRenderer>().get_sharedMaterial().set_renderQueue(0x1194);
                    }
                    if (((curLvelContext != null) && curLvelContext.m_isShowHonor) && (ownerPlayer != null))
                    {
                        this.SetHonor(ownerPlayer.HonorId, ownerPlayer.HonorLevel);
                    }
                }
                else if (this.HudType != HudCompType.Type_Eye)
                {
                    ActorTypeDef actorType = base.actor.TheActorMeta.ActorType;
                    if (this.HudType == HudCompType.Type_Boss)
                    {
                        actorType = ActorTypeDef.Actor_Type_Hero;
                    }
                    Sprite3D sprited = this.m_hud.get_transform().Find("BloodHud").GetComponent<Sprite3D>();
                    sprited.spriteName = Enum.GetName(typeof(SpriteNameEnum), ActorTypeDef.Actor_Type_EYE * actorType);
                    sprited.SetNativeSize(Singleton<Camera_UI3D>.GetInstance().GetCurrentCamera(), 30f);
                    this.m_bloodImage = this.m_hud.get_transform().Find("Fore").GetComponent<Sprite3D>();
                    if (flag2)
                    {
                        this.m_bloodImage.spriteName = "bl_hero_self";
                    }
                    else
                    {
                        this.m_bloodImage.spriteName = Enum.GetName(typeof(SpriteNameEnum), (ActorTypeDef.Actor_Type_EYE * actorType) + (!base.actor.IsHostCamp() ? 2 : 1));
                    }
                    this.m_bloodImage.SetNativeSize(Singleton<Camera_UI3D>.GetInstance().GetCurrentCamera(), 30f);
                    if (base.actor.ValueComponent.actorHpTotal > 0)
                    {
                        this.m_bloodImage.fillAmount = ((float) base.actor.ValueComponent.actorHp) / ((float) base.actor.ValueComponent.actorHpTotal);
                    }
                    else
                    {
                        this.m_bloodImage.fillAmount = 0f;
                    }
                    bool flag4 = base.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ;
                    Sprite3D componetInChild = Utility.GetComponetInChild<Sprite3D>(this.m_hud, "Icon");
                    if (componetInChild != null)
                    {
                        if (flag4)
                        {
                            componetInChild.spriteName = !base.actor.IsHostCamp() ? SpriteNameEnum.bl_icon_organ_enemy.ToString() : SpriteNameEnum.bl_icon_organ_self.ToString();
                            componetInChild.SetNativeSize(Singleton<Camera_UI3D>.GetInstance().GetCurrentCamera(), 30f);
                            componetInChild.get_transform().set_localPosition(new Vector3((this.m_bloodImage.get_transform().get_localPosition().x - (0.5f * this.m_bloodImage.width)) - (0.25f * componetInChild.width), componetInChild.get_transform().get_localPosition().y, componetInChild.get_transform().get_localPosition().z));
                            componetInChild.get_gameObject().CustomSetActive(true);
                        }
                        else
                        {
                            componetInChild.get_gameObject().CustomSetActive(false);
                        }
                    }
                }
                else
                {
                    ActorTypeDef def2 = base.actor.TheActorMeta.ActorType;
                    this.m_hud.get_transform().Find("BloodHud").GetComponent<Sprite3D>().spriteName = Enum.GetName(typeof(SpriteNameEnum), ActorTypeDef.Actor_Type_EYE * def2);
                    this.m_bloodImage = this.m_hud.get_transform().Find("Fore").GetComponent<Sprite3D>();
                    this.m_bloodImage.spriteName = Enum.GetName(typeof(SpriteNameEnum), (ActorTypeDef.Actor_Type_EYE * def2) + (!base.actor.IsHostCamp() ? 2 : 1));
                    if (base.actor.ValueComponent.actorHpTotal > 0)
                    {
                        this.m_bloodImage.fillAmount = ((float) base.actor.ValueComponent.actorHp) / ((float) base.actor.ValueComponent.actorHpTotal);
                    }
                    else
                    {
                        this.m_bloodImage.fillAmount = 0f;
                    }
                    Sprite3D sprited4 = Utility.GetComponetInChild<Sprite3D>(this.m_hud, "Icon");
                    if (sprited4 != null)
                    {
                        sprited4.get_gameObject().CustomSetActive(false);
                    }
                    this.m_timerImg = this.m_hud.get_transform().Find("Timer").GetComponent<Sprite3D>();
                }
                Transform transform = this.m_hud.get_transform().Find("VoiceIcon");
                if (transform != null)
                {
                    this.m_VoiceIconImage = transform.GetComponent<Sprite3D>();
                    this.m_VoiceIconImage.get_gameObject().CustomSetActive(false);
                }
                Transform hudPanel = this.GetHudPanel(this.HudType, base.actor.IsHostCamp());
                if (hudPanel != null)
                {
                    this.m_hud.get_transform().SetParent(hudPanel, true);
                    this.m_hud.get_transform().set_localScale(Vector3.get_one());
                    this.m_hud.get_transform().set_localRotation(Quaternion.get_identity());
                }
                Singleton<Camera_UI3D>.GetInstance().GetCurrentCanvas().RefreshLayout(null);
                this.setHudLogicVisible(false);
                if (this.m_bloodDirty)
                {
                    this.m_bloodDirty = false;
                    bool bActive = (this.m_hudLogicVisible && base.actor.Visible) && base.actor.InCamera;
                    if (this.m_hud != null)
                    {
                        this.m_hud.CustomSetActive(bActive);
                    }
                }
            }
            if (((base.actor.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Organ) || base.actor.TheStaticData.TheOrganOnlyInfo.ShowInMinimap) && (!this.IsCallMonster() && (base.actor.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_EYE)))
            {
                this.HudInit_MapPointer();
            }
        }

        private void InitStatus()
        {
            this._statusGo = new GameObject[1];
            if (this._mountPoint == null)
            {
                this._mountPoint = new GameObject("MountPoint");
                this._mountPoint.get_transform().SetParent(base.actor.myTransform);
                this._mountPoint.get_transform().set_localPosition(Vector3.get_zero());
                this._mountPoint.get_transform().set_localScale(Vector3.get_one());
                this._mountPoint.get_transform().set_localRotation(Quaternion.get_identity());
            }
        }

        private bool IsCallMonster()
        {
            if (base.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster)
            {
                MonsterWrapper actorControl = base.actor.ActorControl as MonsterWrapper;
                if (actorControl != null)
                {
                    PoolObjHandle<ActorRoot> hostActor = actorControl.hostActor;
                    if ((hostActor != 0) && (hostActor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool IsNeedUpdateUI()
        {
            return ((CUIManager.s_uiSystemRenderFrameCounter - this.m_hudFrameStampOnCreated) <= 2);
        }

        private bool IsNormalJungle()
        {
            return ((((base.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster) && (base.actor.ActorControl.GetActorSubType() == 2)) && ((base.actor.ActorControl.GetActorSubSoliderType() != 7) && (base.actor.ActorControl.GetActorSubSoliderType() != 8))) && (base.actor.ActorControl.GetActorSubSoliderType() != 9));
        }

        private bool IsPlayerCopy()
        {
            return ((base.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster) && (this.HudType == HudCompType.Type_Hero));
        }

        public override void LateUpdate(int delta)
        {
            if (this.m_hud != null)
            {
                Vector3 actorPosition = base.actor.myTransform.get_position();
                if (this.m_actorPos != actorPosition)
                {
                    this.AddDirtyFlag(enDirtyFlag.PositionInMap);
                }
                if (this.HasDirtyFlag(enDirtyFlag.ForcePositionInMap))
                {
                    if (this.IsNormalJungle())
                    {
                        Vector3 bornPos = (Vector3) base.actor.BornPos;
                        this.UpdateUIMap(ref bornPos);
                    }
                    else
                    {
                        this.UpdateUIMap(ref actorPosition);
                        this.m_actorPos = actorPosition;
                    }
                    this.RemoveDirtyFlag(enDirtyFlag.ForcePositionInMap);
                    this.RemoveDirtyFlag(enDirtyFlag.PositionInMap);
                    this.RemoveDirtyFlag(enDirtyFlag.Immediate);
                }
                else if ((base.actor.Visible && !this.IsNormalJungle()) && (this.HasDirtyFlag(enDirtyFlag.PositionInMap) || this.IsNeedUpdateUI()))
                {
                    if ((base.actor.ActorControl.GetActorSubType() == 1) && ((((ulong) base.actor.ObjID) % ((long) soilderFrameCount)) == (((ulong) Singleton<FrameSynchr>.instance.CurFrameNum) % ((long) soilderFrameCount))))
                    {
                        this.UpdateUIMap(ref actorPosition);
                        this.m_actorPos = actorPosition;
                        this.RemoveDirtyFlag(enDirtyFlag.PositionInMap);
                    }
                    if ((base.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero) && (this.HasDirtyFlag(enDirtyFlag.Immediate) || ((((ulong) base.actor.ObjID) % ((long) heroFrameCount)) == (((ulong) Singleton<FrameSynchr>.instance.CurFrameNum) % ((long) heroFrameCount)))))
                    {
                        this.UpdateUIMap(ref actorPosition);
                        this.m_actorPos = actorPosition;
                        this.RemoveDirtyFlag(enDirtyFlag.PositionInMap);
                        this.RemoveDirtyFlag(enDirtyFlag.Immediate);
                    }
                }
                this.UpdateMiniMapHeroRotation();
                if (this.m_bloodDirty)
                {
                    this.m_bloodDirty = false;
                    bool bActive = (this.m_hudLogicVisible && base.actor.Visible) && base.actor.InCamera;
                    this.m_hud.CustomSetActive(bActive);
                    if ((this.m_signalImage != null) && (bActive != this.m_signalImage.get_gameObject().get_activeSelf()))
                    {
                        this.m_signalImage.get_gameObject().CustomSetActive(bActive);
                    }
                }
                if (((this.HudType != HudCompType.Type_Hide) && base.actor.Visible) && base.actor.InCamera)
                {
                    if (this.m_hud.get_activeSelf())
                    {
                        Vector3 vector3 = actorPosition;
                        vector3.y += this.hudHeight * 0.001f;
                        Vector3 bloodPosition = Camera.get_main().WorldToScreenPoint(vector3);
                        bool flag2 = this.m_bloodPos != bloodPosition;
                        this.m_bloodPos = bloodPosition;
                        if (this.IsNeedUpdateUI() || flag2)
                        {
                            this.UpdateUIHud(ref bloodPosition);
                        }
                    }
                    if (this.m_inOutEquipShopHud != null)
                    {
                        this.m_inOutEquipShopHud.CustomSetActive((base.actor.Visible && base.actor.InCamera) && (!base.actor.ActorControl.IsDeadState || base.actor.TheStaticData.TheBaseAttribute.DeadControl));
                        if (this.m_inOutEquipShopHud.get_activeSelf())
                        {
                            Vector3 vector5 = actorPosition;
                            vector5.y += this.hudHeight * 0.001f;
                            Vector3 screenPoint = Camera.get_main().WorldToScreenPoint(vector5);
                            FightForm fightForm = Singleton<CBattleSystem>.GetInstance().FightForm;
                            if ((fightForm != null) && (fightForm._formScript != null))
                            {
                                this.m_inOutEquipShopHud.get_transform().set_position(CUIUtility.ScreenToWorldPoint(fightForm._formScript.GetCamera(), screenPoint, screenPoint.z));
                            }
                        }
                    }
                    if (this.m_exclamationObj != null)
                    {
                        this.m_exclamationObj.get_transform().set_position(new Vector3(actorPosition.x, actorPosition.y + this.m_exclamationObjOffsetY, actorPosition.z));
                    }
                    if (this.m_outOfControlBar != null)
                    {
                        int index = 0;
                        while (index < this.m_outofControlList.Count)
                        {
                            CoutofControlInfo local1 = this.m_outofControlList[index];
                            local1.leftTime -= (int) (Time.get_deltaTime() * 1000f);
                            if (this.m_outofControlList[index].leftTime <= 0)
                            {
                                this.m_outofControlList.RemoveAt(index);
                            }
                            else
                            {
                                index++;
                            }
                        }
                        this.SetOutOfControlBar();
                    }
                    this.setReviveTimerBar();
                    this.setSkillTimerBar();
                    if ((this.CurAniIndex_ != -1) && !this.HeroProficiencyAni_[this.CurAniIndex_].get_isPlaying())
                    {
                        this.HeroProficiencyObj_[this.CurAniIndex_].CustomSetActive(false);
                    }
                    if (this._isDecreasingHp && ((this._bloodDecImage != null) && (this._bloodDecImage.fillAmount >= this.m_bloodImage.fillAmount)))
                    {
                        this._bloodDecImage.fillAmount -= 0.0086f;
                        if (this._bloodDecImage.fillAmount <= this.m_bloodImage.fillAmount)
                        {
                            this._bloodDecImage.fillAmount = this.m_bloodImage.fillAmount;
                            this._isDecreasingHp = false;
                        }
                    }
                }
            }
        }

        public void OnActorDead()
        {
            if (!base.actor.TheStaticData.TheBaseAttribute.DeadControl)
            {
                this.setHudLogicVisible(false);
                this.setTextLogicVisible(false);
                this.setPointerVisible(false, true);
                DragonIcon.Check_Dragon_Born_Evt(base.actor, false);
                TowerHitMgr towerHitMgr = Singleton<CBattleSystem>.GetInstance().TowerHitMgr;
                if ((base.actor != null) && (towerHitMgr != null))
                {
                    towerHitMgr.Remove(base.actor.ObjID);
                }
                if (this.m_evtCom != null)
                {
                    this.m_evtCom.isDead = true;
                }
            }
        }

        public void OnActorRevive()
        {
            if (this.m_evtCom != null)
            {
                this.m_evtCom.isDead = false;
            }
            if (this.bReviveTimerShow)
            {
                this.HideReviveTimer();
            }
            this.setPointerVisible(true, true);
        }

        private void OnBaseAttacked(ref DefaultGameEventParam evtParam)
        {
            if ((evtParam.src != 0) && (evtParam.src == base.actorPtr))
            {
                if (base.actor.IsHostCamp())
                {
                    if ((Singleton<FrameSynchr>.GetInstance().LogicFrameTick - this.LastBaseAttackTime) > 0x4e20L)
                    {
                        GameObject obj2 = this.m_mapPointer_small;
                        if (obj2 != null)
                        {
                            Animator component = obj2.GetComponent<Animator>();
                            if (component != null)
                            {
                                component.set_enabled(true);
                                component.Play("BaseTip", 0, 0f);
                            }
                        }
                        Singleton<CSoundManager>.GetInstance().PlayBattleSound2D("UI_retreat");
                        this.LastBaseAttackTime = Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
                    }
                    TowerHitMgr towerHitMgr = Singleton<CBattleSystem>.GetInstance().TowerHitMgr;
                    if (towerHitMgr != null)
                    {
                        towerHitMgr.TryActive(base.actor.ObjID, this.MapPointerSmall);
                    }
                }
                if ((this.bigTower_spt3d != null) && (this.smallTower_spt3d != null))
                {
                    float single = evtParam.src.handle.ValueComponent.GetHpRate().single;
                    this.bigTower_spt3d.fillAmount = single;
                    this.smallTower_spt3d.fillAmount = single;
                }
            }
        }

        private void onEnergyExpChange(PoolObjHandle<ActorRoot> act, int curVal, int maxVal)
        {
            if (((this.m_energyImage != null) && (act.handle == base.actor)) && (maxVal > 0))
            {
                if (curVal < 0)
                {
                    curVal = 0;
                }
                this.m_energyImage.fillAmount = ((float) curVal) / ((float) maxVal);
            }
        }

        private void OnPlayerCancelLimitMove(ref LimitMoveEventParam _param)
        {
            if ((_param.src != 0) && (_param.src.handle == base.actor))
            {
                for (int i = 0; i < this.m_outofControlList.Count; i++)
                {
                    if (this.m_outofControlList[i].combId == _param.combineID)
                    {
                        this.m_outofControlList.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        private void OnPlayerLimitMove(ref LimitMoveEventParam _param)
        {
            if ((_param.src != 0) && (_param.src.handle == base.actor))
            {
                DebugHelper.Assert(_param.totalTime > 0, "被控时间是0，还控个毛啊, combineid" + _param.combineID);
                if (_param.totalTime > 0)
                {
                    CoutofControlInfo item = new CoutofControlInfo(_param.combineID, _param.totalTime, _param.totalTime);
                    this.m_outofControlList.Add(item);
                }
            }
        }

        private void OnPlayerSkillTimer(ref SkillTimerEventParam _param)
        {
            if ((_param.src != 0) && (_param.src.handle == base.actor))
            {
                if (this.m_skillTimeInfo == null)
                {
                    this.m_skillTimeInfo = new SkillTimerInfo(_param.totalTime, _param.totalTime, _param.starTime);
                }
                else
                {
                    this.m_skillTimeInfo.setSkillTimerParam(_param.totalTime, _param.totalTime, _param.starTime);
                }
            }
        }

        private void OnSinglePlayEnd(GameObject go)
        {
            if (this.m_exclamationObj == go)
            {
                this.m_exclamationObj = null;
            }
        }

        private void onSoulExpChange(PoolObjHandle<ActorRoot> act, int changeValue, int curVal, int maxVal)
        {
            if ((this.m_soulImage != null) && (act.handle == base.actor))
            {
                this.m_soulImage.fillAmount = ((float) curVal) / ((float) maxVal);
            }
            else if (this.IsPlayerCopy())
            {
                MonsterWrapper actorControl = base.actor.ActorControl as MonsterWrapper;
                if (((actorControl != null) && (actorControl.hostActor != 0)) && ((actorControl.hostActor == act) && (this.m_soulImage != null)))
                {
                    this.m_soulImage.fillAmount = ((float) curVal) / ((float) maxVal);
                }
            }
        }

        private void onSoulLvlChange(PoolObjHandle<ActorRoot> act, int curVal)
        {
            if ((this.m_soulLevel != null) && (act.handle == base.actor))
            {
                this.m_soulLevel.set_text(curVal.ToString());
            }
            else if (this.IsPlayerCopy())
            {
                MonsterWrapper actorControl = base.actor.ActorControl as MonsterWrapper;
                if (((actorControl != null) && (actorControl.hostActor != 0)) && ((actorControl.hostActor == act) && (this.m_soulLevel != null)))
                {
                    this.m_soulLevel.set_text(curVal.ToString());
                }
            }
        }

        public override void OnUse()
        {
            base.OnUse();
            this.hudHeight = 0;
            this.HudType = HudCompType.Type_Hero;
            this.m_hudFrameStampOnCreated = 0;
            this.m_hud = null;
            this.m_bloodImage = null;
            this.m_timerImg = null;
            this.m_blackBars.Clear();
            this.m_soulImage = null;
            this.m_energyImage = null;
            this.m_soulLevel = null;
            this.m_outOfControlBar = null;
            this.m_outofControlList = null;
            this.m_outOfControlGo = null;
            this.m_skillTimerObj = null;
            this.m_skillTimerBar = null;
            this.m_skillTimeInfo = null;
            this.m_reviveTimerObj = null;
            this.m_reviveTimerBar = null;
            this.m_bloodBackObj = null;
            this.m_bloodForeObj = null;
            this.m_energyForeObj = null;
            this.m_reviveTotalTime = 0;
            this.hudHeightOffset = 0;
            this.bReviveTimerShow = false;
            MiniMapSysUT.RecycleMapGameObject(this.m_mapPointer_small);
            this.m_mapPointer_small = null;
            this.m_mapPointer_small_Trans = null;
            MiniMapSysUT.RecycleMapGameObject(this.m_heroHead_small_Trans);
            this.m_heroHead_small_Trans = null;
            MiniMapSysUT.RecycleMapGameObject(this.m_mapPointer_big);
            this.m_mapPointer_big = null;
            this.m_mapPointer_big_Trans = null;
            MiniMapSysUT.RecycleMapGameObject(this.m_heroHead_big_Trans);
            this.m_heroHead_big_Trans = null;
            MiniMapSysUT.UnRegisterEventCom(this.m_evtCom);
            this.m_evtCom = null;
            this.bigTower_spt3d = null;
            this.smallTower_spt3d = null;
            this.m_signalImage = null;
            this.m_actorPos = Vector3.get_zero();
            this.m_bloodPos = Vector3.get_zero();
            this.m_bloodDirty = false;
            this.m_actorForward = VInt3.zero;
            this.m_bHeroSameCamp = false;
            this.m_heroIconBG_small = null;
            this.m_heroIconBG_big = null;
            this.m_textHudContainer = null;
            this.m_textHudNode = null;
            this.textCom = null;
            this.txt_hud_offset_x = 0;
            this.txt_hud_offset_y = 0;
            this.rtTransform = null;
            this.m_exclamationObj = null;
            this.m_exclamationObjOffsetY = 0f;
            this.LastBaseAttackTime = 0L;
            this.m_inOutEquipShopHudContainer = null;
            this.m_inOutEquipShopHud = null;
            this.bBossHpBar = false;
            this.m_hudLogicVisible = false;
            this.m_textLogicVisible = false;
            this.m_pointerLogicVisible = false;
            this._statusGo = null;
            this._mountPoint = null;
            this._shieldGo = null;
            this._bloodDecImage = null;
            this._lastMaxBarValue = 0;
            this._curShield1 = 0;
            this._shieldImagWidth = 0f;
            this._isDecreasingHp = false;
            this.m_effectRoot_small = null;
            this.m_VoiceIconImage = null;
            this.ClearHeroProficiency();
            this.m_honor = null;
            this.m_honorAni = null;
            this.m_dirtyFlags = 0;
        }

        public void PlayHonorAni(int honorId, int honorLevel)
        {
            if ((this.m_honor != null) && (this.m_honorAni != null))
            {
                if (this.m_honor.get_layer() != LayerMask.NameToLayer("3DUI"))
                {
                    this.m_honor.SetLayer("3DUI", false);
                }
                if (!this.m_honorAni.get_enabled())
                {
                    this.m_honorAni.set_enabled(true);
                }
                if (!this.m_honorAni.get_isPlaying())
                {
                    this.m_honorAni.Play();
                }
            }
        }

        public void PlayMapEffect(MiniMapEffect mme)
        {
            if (this.m_effectRoot_small != null)
            {
                GameObject obj2 = Utility.FindChild(this.m_effectRoot_small, mme.ToString());
                SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
                if ((obj2 != null) && (curLvelContext != null))
                {
                    this.m_actorPos = base.actor.myTransform.get_position();
                    (obj2.get_transform() as RectTransform).set_anchoredPosition(new Vector2(this.m_actorPos.x * Singleton<CBattleSystem>.instance.world_UI_Factor_Small.x, this.m_actorPos.z * Singleton<CBattleSystem>.instance.world_UI_Factor_Small.y));
                    if (obj2.get_activeSelf())
                    {
                        obj2.CustomSetActive(false);
                    }
                    obj2.CustomSetActive(true);
                }
            }
        }

        public bool PlayProficiencyAni(uint proficiencyLevel)
        {
            if (this.m_hud == null)
            {
                return false;
            }
            if ((this.CurAniIndex_ != -1) && this.HeroProficiencyAni_[this.CurAniIndex_].get_isPlaying())
            {
                return false;
            }
            int index = this.TranslateProficiencyLevelToIndex(proficiencyLevel);
            if (((index >= 0) && (index < 3)) && (this.HeroProficiencyAni_[index] != null))
            {
                this.HeroProficiencyObj_[index].CustomSetActive(true);
                this.HeroProficiencyAni_[index].Play(HeroProficiencyShowNames_[index]);
                this.CurAniIndex_ = index;
            }
            return true;
        }

        public static void PreallocMapPointer(int aliesNum, int enemyNum)
        {
            MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
            if (theMinimapSys != null)
            {
                List<int> seqs = new List<int>(enemyNum);
                PreallocMapPointer(seqs, theMinimapSys.mmpcAlies, aliesNum);
                PreallocMapPointer(seqs, theMinimapSys.mmpcEnemy, enemyNum);
            }
        }

        private static void PreallocMapPointer(List<int> seqs, GameObject go, int num)
        {
            if (go != null)
            {
                seqs.Clear();
                CUIContainerScript component = go.GetComponent<CUIContainerScript>();
                for (int i = 0; i < num; i++)
                {
                    int element = component.GetElement();
                    if (element != -1)
                    {
                        seqs.Add(element);
                    }
                }
                for (int j = 0; j < seqs.Count; j++)
                {
                    component.RecycleElement(seqs[j]);
                }
            }
        }

        public static void Preload(ref ActorPreloadTab preloadTab)
        {
            for (int i = 0; i < _statusResPath.Length; i++)
            {
                preloadTab.AddParticle(_statusResPath[i]);
            }
            preloadTab.AddParticle("UI3D/Battle/Blood_Bar_Hero.prefab");
            preloadTab.AddParticle("UI3D/Battle/BloodHud.prefab");
            preloadTab.AddParticle("UI3D/Battle/BloodHudEye.prefab");
        }

        public override void Prepare()
        {
            if (!MonoSingleton<GameFramework>.instance.EditorPreviewMode)
            {
                this.InitHudUI();
                this.InitStatus();
            }
        }

        private void refreshHudVisible()
        {
            if (this.m_hud != null)
            {
                bool flag = (this.m_hudLogicVisible && base.actor.Visible) && base.actor.InCamera;
                if (flag != this.m_hud.get_activeSelf())
                {
                    this.m_bloodDirty = true;
                }
            }
        }

        public void RefreshMapPointerBig()
        {
            CUIFormScript formScript = Singleton<CBattleSystem>.GetInstance().FormScript;
            if (((formScript != null) && (formScript.m_sgameGraphicRaycaster != null)) && (this.m_mapPointer_big != null))
            {
                formScript.m_sgameGraphicRaycaster.RefreshGameObject(this.m_mapPointer_big);
            }
        }

        private void refreshTextVisible()
        {
            if (this.m_textHudNode != null)
            {
                bool bActive = (this.m_textLogicVisible && base.actor.Visible) && base.actor.InCamera;
                if (bActive != this.m_textHudNode.get_activeSelf())
                {
                    this.m_textHudNode.CustomSetActive(bActive);
                }
            }
        }

        private void RemoveDirtyFlag(enDirtyFlag dirtyFlag)
        {
            this.m_dirtyFlags &= ~dirtyFlag;
        }

        public void RemoveInEquipShopHud()
        {
            if ((this.m_inOutEquipShopHudContainer != null) && (this.m_inOutEquipShopHud != null))
            {
                this.m_inOutEquipShopHudContainer.RecycleElement(this.m_inOutEquipShopHud);
                this.m_inOutEquipShopHud = null;
            }
        }

        private void ResetHudUI()
        {
            if (((base.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster) || (base.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_EYE)) && (((this.HudType != HudCompType.Type_Hero) && (this.m_hud != null)) && (this.HudType != HudCompType.Type_Hide)))
            {
                bool isHostCamp = base.actor.IsHostCamp();
                this.m_bHeroSameCamp = isHostCamp;
                ActorTypeDef actorType = base.actor.TheActorMeta.ActorType;
                if (this.HudType == HudCompType.Type_Boss)
                {
                    actorType = ActorTypeDef.Actor_Type_Hero;
                }
                this.m_bloodImage.spriteName = Enum.GetName(typeof(SpriteNameEnum), (ActorTypeDef.Actor_Type_EYE * actorType) + (!isHostCamp ? 2 : 1));
                if (this.HudType != HudCompType.Type_Eye)
                {
                    this.m_bloodImage.SetNativeSize(Singleton<Camera_UI3D>.GetInstance().GetCurrentCamera(), 30f);
                }
                if (base.actor.ValueComponent.actorHpTotal > 0)
                {
                    this.m_bloodImage.fillAmount = ((float) base.actor.ValueComponent.actorHp) / ((float) base.actor.ValueComponent.actorHpTotal);
                }
                else
                {
                    this.m_bloodImage.fillAmount = 0f;
                }
                if (this.m_VoiceIconImage != null)
                {
                    this.m_VoiceIconImage.get_gameObject().CustomSetActive(false);
                }
                this.m_timerImg = null;
                if (this.HudType == HudCompType.Type_Eye)
                {
                    this.m_timerImg = this.m_hud.get_transform().Find("Timer").GetComponent<Sprite3D>();
                }
                Transform hudPanel = this.GetHudPanel(this.HudType, isHostCamp);
                if (hudPanel != null)
                {
                    this.m_hud.get_transform().SetParent(hudPanel, true);
                    this.m_hud.get_transform().set_localScale(Vector3.get_one());
                    this.m_hud.get_transform().set_localRotation(Quaternion.get_identity());
                }
                Singleton<Camera_UI3D>.GetInstance().GetCurrentCanvas().RefreshLayout(null);
                this.setHudLogicVisible(true);
                this.m_bloodDirty = true;
                this.HudInit_MapPointer();
                SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
                if (curLvelContext != null)
                {
                    this.setPointerVisible(curLvelContext.IsMobaMode(), true);
                }
                if (FogOfWar.enable && this.IsNormalJungle())
                {
                    this.m_mapPointer_small.CustomSetActive(true);
                    this.m_mapPointer_big.CustomSetActive(true);
                    if (this.m_heroHead_small_Trans != null)
                    {
                        this.m_heroHead_small_Trans.get_gameObject().CustomSetActive(true);
                    }
                    if (this.m_heroHead_big_Trans != null)
                    {
                        this.m_heroHead_big_Trans.get_gameObject().CustomSetActive(true);
                    }
                    if (base.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
                    {
                        this.AddDirtyFlag(enDirtyFlag.PositionInMap);
                        this.AddDirtyFlag(enDirtyFlag.Immediate);
                    }
                }
            }
        }

        private void ResetStatusHeight(int hight)
        {
            for (int i = 0; i < 1; i++)
            {
                GameObject obj2 = this._statusGo[i];
                if (obj2 != null)
                {
                    obj2.get_transform().set_localPosition(new Vector3(0f, (hight - OFFSET_HEIGHT) * 0.001f, 0f));
                }
            }
        }

        protected void SetBlackBar(int curMaxHpBarValue)
        {
            if (curMaxHpBarValue != this._lastMaxBarValue)
            {
                this._lastMaxBarValue = curMaxHpBarValue;
                int num = 0x3e8;
                int num2 = -1;
                while (num < curMaxHpBarValue)
                {
                    num2++;
                    if (num2 < this.m_blackBars.Count)
                    {
                        if (!this.m_blackBars[num2].get_activeSelf())
                        {
                            this.m_blackBars[num2].SetActive(true);
                        }
                    }
                    else
                    {
                        GameObject gameObject = Singleton<CGameObjectPool>.GetInstance().GetGameObject("UI3D/Battle/BlackBarBlack.prefab", enResourceType.BattleScene);
                        if (!gameObject.get_activeSelf())
                        {
                            gameObject.SetActive(true);
                        }
                        gameObject.get_transform().set_parent(this.m_bloodImage.get_transform());
                        gameObject.get_transform().SetSiblingIndex(this.m_bloodImage.get_transform().get_childCount() - 1);
                        gameObject.get_transform().set_localRotation(Quaternion.get_identity());
                        this.m_blackBars.Add(gameObject);
                        Singleton<Camera_UI3D>.GetInstance().GetCurrentCanvas().RefreshLayout(null);
                    }
                    bool flag = ((num2 + 1) % 5) == 0;
                    this.m_blackBars[num2].get_transform().set_localScale(!flag ? new Vector3(1f, 0.72f, 1f) : new Vector3(1.25f, 1f, 1f));
                    this.m_blackBars[num2].get_transform().set_localPosition(new Vector3((((float) num) / ((float) curMaxHpBarValue)) * this._shieldImagWidth, !flag ? 0.065f : 0f, 0f));
                    num += 0x3e8;
                }
                for (int i = num2 + 1; i < this.m_blackBars.Count; i++)
                {
                    this.m_blackBars[i].SetActive(false);
                }
            }
        }

        public void SetComVisible(bool bVisiable)
        {
            if (this._mountPoint != null)
            {
                this._mountPoint.CustomSetActive(bVisiable);
            }
            this.refreshHudVisible();
            this.refreshTextVisible();
            if ((base.actor.ActorControl.GetActorSubType() == 2) && (base.actor.TheActorMeta.ConfigId != Singleton<BattleLogic>.instance.DragonId))
            {
                this.setPointerVisible(bVisiable, true);
            }
            else
            {
                this.setPointerVisible(false, false);
            }
        }

        protected void SetHonor(int honorId = 0, int honorLevel = 0)
        {
            if (honorId != 0)
            {
                string prefabFullPath = string.Format("UI3D/Battle/Img_Badge_{0}_{1}.prefab", honorId, honorLevel);
                this.m_honor = Singleton<CGameObjectPool>.GetInstance().GetGameObject(prefabFullPath, enResourceType.BattleScene);
                DebugHelper.Assert(this.m_honor != null, string.Format("{0} doesn't exist!", prefabFullPath));
                if (this.m_honor != null)
                {
                    this.m_honorAni = this.m_honor.GetComponent<Animation>();
                    if (!this.m_honor.get_activeSelf())
                    {
                        this.m_honor.SetActive(true);
                    }
                    this.EndHonorAni();
                    this.m_honor.get_transform().set_parent(this.m_hud.get_transform());
                    Vector3 vector = this.m_hud.get_transform().get_position();
                    vector.y += -0.64f;
                    vector.x += 2.68f;
                    this.m_honor.get_transform().set_localPosition(vector);
                }
            }
        }

        private void setHudLogicVisible(bool isVisible)
        {
            this.m_hudLogicVisible = isVisible;
            this.refreshHudVisible();
        }

        public void SetOutOfControlBar()
        {
            if (this.m_outofControlList.Count <= 0)
            {
                this.m_outOfControlGo.CustomSetActive(false);
                if (this.m_outOfControlBar.fillAmount != 0f)
                {
                    this.m_outOfControlBar.fillAmount = 0f;
                }
            }
            else
            {
                CoutofControlInfo info = this.m_outofControlList[0];
                for (int i = 1; i < this.m_outofControlList.Count; i++)
                {
                    if (info.leftTime < this.m_outofControlList[i].leftTime)
                    {
                        info = this.m_outofControlList[i];
                    }
                }
                this.m_outOfControlGo.CustomSetActive(true);
                float num2 = ((float) info.leftTime) / ((float) info.totalTime);
                this.m_outOfControlBar.fillAmount = num2;
            }
        }

        private void setPointerVisible(bool show, bool isLogicSet)
        {
            bool flag2 = (!this.IsCallMonster() && (base.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster)) && (base.actor.ActorControl.GetActorSubType() == 2);
            bool flag3 = flag2 && (base.actor.TheActorMeta.ConfigId == Singleton<BattleLogic>.instance.DragonId);
            if (((flag2 && !flag3) && (isLogicSet && !this.m_pointerLogicVisible)) && (show && base.actor.Visible))
            {
                bool bActive = !base.actor.ActorControl.IsDeadState;
                this.m_mapPointer_small.CustomSetActive(bActive);
                this.m_mapPointer_big.CustomSetActive(bActive);
                if (this.m_heroHead_small_Trans != null)
                {
                    this.m_heroHead_small_Trans.get_gameObject().CustomSetActive(bActive);
                }
                if (this.m_heroHead_big_Trans != null)
                {
                    this.m_heroHead_big_Trans.get_gameObject().CustomSetActive(bActive);
                }
                this.m_pointerLogicVisible = false;
            }
            else
            {
                if (isLogicSet)
                {
                    this.m_pointerLogicVisible = show;
                }
                if (!flag2)
                {
                    bool flag5 = (this.m_pointerLogicVisible && base.actor.Visible) && (!base.actor.ActorControl.IsDeadState || base.actor.TheStaticData.TheBaseAttribute.DeadControl);
                    if ((base.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ) && !base.actor.ActorControl.IsDeadState)
                    {
                        flag5 = true;
                    }
                    if (this.m_mapPointer_small != null)
                    {
                        this.m_mapPointer_small.CustomSetActive(flag5);
                    }
                    if (this.m_mapPointer_big != null)
                    {
                        this.m_mapPointer_big.CustomSetActive(flag5);
                    }
                    if (this.m_heroHead_small_Trans != null)
                    {
                        this.m_heroHead_small_Trans.get_gameObject().CustomSetActive(flag5);
                    }
                    if (this.m_heroHead_big_Trans != null)
                    {
                        this.m_heroHead_big_Trans.get_gameObject().CustomSetActive(flag5);
                    }
                    if ((base.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero) && flag5)
                    {
                        this.AddDirtyFlag(enDirtyFlag.PositionInMap);
                        this.AddDirtyFlag(enDirtyFlag.Immediate);
                    }
                }
            }
        }

        private void setReviveTimerBar()
        {
            if ((this.bReviveTimerShow && (this.m_reviveTotalTime > 0)) && (this.m_reviveTimerBar != null))
            {
                this.m_reviveTimerBar.fillAmount = 1f - (((float) base.actor.ActorControl.ReviveCooldown) / ((float) this.m_reviveTotalTime));
            }
        }

        public void SetSelected(bool bHost)
        {
            if (bHost)
            {
                this.m_bloodImage.spriteName = "bl_hero_self";
            }
            else
            {
                this.m_bloodImage.spriteName = "bl_hero_mid";
            }
        }

        private void setSkillTimerBar()
        {
            if (((this.m_skillTimerObj != null) && (this.m_skillTimerBar != null)) && (this.m_skillTimeInfo != null))
            {
                if ((this.m_skillTimeInfo.totalTime <= 0) || (this.m_skillTimeInfo.leftTime <= 0))
                {
                    this.m_skillTimerObj.CustomSetActive(false);
                    if (this.m_skillTimerBar.fillAmount != 0f)
                    {
                        this.m_skillTimerBar.fillAmount = 0f;
                    }
                }
                else
                {
                    float num = ((float) this.m_skillTimeInfo.leftTime) / ((float) this.m_skillTimeInfo.totalTime);
                    this.m_skillTimerBar.fillAmount = num;
                    this.m_skillTimerObj.CustomSetActive(true);
                    ulong logicFrameTick = Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
                    this.m_skillTimeInfo.leftTime = this.m_skillTimeInfo.totalTime - ((int) (logicFrameTick - this.m_skillTimeInfo.starTime));
                }
            }
        }

        public void SetTextHud(string text, int txt_offset_x, int txt_offset_y, bool bCenter = false)
        {
            if (this.m_textHudContainer == null)
            {
                this.m_textHudContainer = Singleton<CBattleSystem>.GetInstance().TextHudContainer;
            }
            if ((this.m_textHudContainer != null) && (this.m_textHudNode == null))
            {
                this.Init_TextHud(this.m_textHudContainer.GetElement(), false);
            }
            this.txt_hud_offset_x = txt_offset_x;
            this.txt_hud_offset_y = txt_offset_y;
            bool flag = string.IsNullOrEmpty(text);
            if (flag)
            {
                this.setTextLogicVisible(false);
            }
            if (!flag && (this.m_textHudNode != null))
            {
                this.SetTextHudContent(text);
                this.setTextLogicVisible(true);
            }
        }

        private void SetTextHudContent(string txt)
        {
            this.textCom.set_text(txt);
        }

        private void setTextLogicVisible(bool isVisible)
        {
            this.m_textLogicVisible = isVisible;
            this.refreshTextVisible();
        }

        public void ShowHeadExclamationMark(string eftPath, float offset_height)
        {
            if (((!FogOfWar.enable || (base.actor.HorizonMarker == null)) || base.actor.HorizonMarker.IsVisibleFor(Singleton<GamePlayerCenter>.instance.GetHostPlayer().PlayerCamp)) && ((((Singleton<CBattleSystem>.GetInstance().FightForm == null) ? null : Singleton<CBattleSystem>.GetInstance().FightForm.GetSignalPanel()) != null) && (this.m_exclamationObj == null)))
            {
                Vector3 vector = base.actor.myTransform.get_position();
                Vector3 worldPosition = new Vector3(vector.x, vector.y + offset_height, vector.z);
                this.m_exclamationObj = this.CreateSignalGameObject(eftPath, worldPosition);
                this.m_exclamationObjOffsetY = offset_height;
                Singleton<CGameObjectPool>.GetInstance().RecycleGameObjectDelay(this.m_exclamationObj, 0x1388, new CGameObjectPool.OnDelayRecycleDelegate(this.OnSinglePlayEnd));
            }
        }

        public void ShowReviveTimer(int yOffset, int reviveTime)
        {
            this.m_reviveTimerObj.CustomSetActive(true);
            this.m_bloodBackObj.CustomSetActive(false);
            this.m_bloodForeObj.CustomSetActive(false);
            this.m_energyForeObj.CustomSetActive(false);
            if (this.m_reviveTimerBar != null)
            {
                this.m_reviveTimerBar.fillAmount = 0f;
            }
            this.m_reviveTotalTime = reviveTime;
            this.hudHeightOffset = yOffset;
            this.hudHeight += this.hudHeightOffset;
            if (this.hudHeightOffset > 0)
            {
                this.ResetStatusHeight(this.hudHeight);
            }
            this.bReviveTimerShow = true;
            this.setHudLogicVisible(true);
        }

        public void ShowStatus(StatusHudType st)
        {
            if (this._mountPoint != null)
            {
                this._mountPoint.CustomSetActive(base.actor.Visible);
                int index = (int) st;
                GameObject obj2 = this._statusGo[index];
                if (obj2 == null)
                {
                    Object content = Singleton<CResourceManager>.GetInstance().GetResource(_statusResPath[index], typeof(GameObject), enResourceType.BattleScene, false, false).m_content;
                    if (content != null)
                    {
                        obj2 = Object.Instantiate(content);
                        this._statusGo[index] = obj2;
                        if (obj2 != null)
                        {
                            obj2.get_transform().SetParent(this._mountPoint.get_transform());
                            obj2.get_transform().set_localPosition(new Vector3(0f, (this.hudHeight - OFFSET_HEIGHT) * 0.001f, 0f));
                        }
                    }
                }
                if (obj2 != null)
                {
                    obj2.CustomSetActive(true);
                }
            }
        }

        public void ShowVoiceIcon(bool bShow)
        {
            if (this.m_VoiceIconImage != null)
            {
                this.m_VoiceIconImage.get_gameObject().CustomSetActive(bShow);
            }
        }

        private int TranslateProficiencyLevelToIndex(uint proficiencyLevel)
        {
            return (((int) proficiencyLevel) - 3);
        }

        protected void TryToStartDecreaseHpEffect()
        {
            if (this._bloodDecImage != null)
            {
                if (this.m_bloodImage.fillAmount >= this._bloodDecImage.fillAmount)
                {
                    this._bloodDecImage.fillAmount = this.m_bloodImage.fillAmount;
                    this._isDecreasingHp = false;
                }
                if (!this._isDecreasingHp && (this.m_bloodImage.fillAmount < this._bloodDecImage.fillAmount))
                {
                    this._isDecreasingHp = true;
                }
            }
        }

        public override void Uninit()
        {
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int, int, int>("HeroSoulExpChange", new Action<PoolObjHandle<ActorRoot>, int, int, int>(this, (IntPtr) this.onSoulExpChange));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int, int>("HeroEnergyChange", new Action<PoolObjHandle<ActorRoot>, int, int>(this, (IntPtr) this.onEnergyExpChange));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int>("HeroSoulLevelChange", new Action<PoolObjHandle<ActorRoot>, int>(this, (IntPtr) this.onSoulLvlChange));
            Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<LimitMoveEventParam>(GameSkillEventDef.AllEvent_LimitMove, new GameSkillEvent<LimitMoveEventParam>(this.OnPlayerLimitMove));
            Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<LimitMoveEventParam>(GameSkillEventDef.AllEvent_CancelLimitMove, new GameSkillEvent<LimitMoveEventParam>(this.OnPlayerCancelLimitMove));
            Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<SkillTimerEventParam>(GameSkillEventDef.AllEvent_SetSkillTimer, new GameSkillEvent<SkillTimerEventParam>(this.OnPlayerSkillTimer));
            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorBeAttack, new RefAction<DefaultGameEventParam>(this.OnBaseAttacked));
            for (int i = 0; i < this.m_blackBars.Count; i++)
            {
                Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(this.m_blackBars[i]);
            }
            this.m_blackBars.Clear();
            if (this.m_honor != null)
            {
                Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(this.m_honor);
            }
            this.m_honor = null;
            this.m_honorAni = null;
            if (this.m_hud != null)
            {
                Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(this.m_hud);
            }
            this.m_hud = null;
            this.m_bloodImage = null;
            this.m_timerImg = null;
            this._shieldGo = null;
            this.m_soulImage = null;
            this.m_energyImage = null;
            this.m_soulLevel = null;
            this.m_outOfControlBar = null;
            this.m_outofControlList = null;
            this.m_outOfControlGo = null;
            this.m_skillTimerObj = null;
            this.m_skillTimerBar = null;
            this.m_skillTimeInfo = null;
            this.m_reviveTimerObj = null;
            this.m_reviveTimerBar = null;
            this.m_bloodBackObj = null;
            this.m_bloodForeObj = null;
            this.m_energyForeObj = null;
            this.m_reviveTotalTime = 0;
            this.hudHeightOffset = 0;
            this.bReviveTimerShow = false;
            MiniMapSysUT.RecycleMapGameObject(this.m_mapPointer_small);
            this.m_mapPointer_small = null;
            this.m_mapPointer_small_Trans = null;
            MiniMapSysUT.RecycleMapGameObject(this.m_heroHead_small_Trans);
            this.m_heroHead_small_Trans = null;
            MiniMapSysUT.RecycleMapGameObject(this.m_mapPointer_big);
            this.m_mapPointer_big = null;
            this.m_mapPointer_big_Trans = null;
            MiniMapSysUT.RecycleMapGameObject(this.m_heroHead_big_Trans);
            this.m_heroHead_big_Trans = null;
            MiniMapSysUT.UnRegisterEventCom(this.m_evtCom);
            this.m_evtCom = null;
            this.bigTower_spt3d = null;
            this.smallTower_spt3d = null;
            if ((this.m_textHudContainer != null) && (this.m_textHudNode != null))
            {
                this.m_textHudContainer.RecycleElement(this.m_textHudNode);
            }
            this.m_textHudContainer = null;
            this.m_textHudNode = null;
            this.textCom = null;
            this.txt_hud_offset_x = 0;
            this.txt_hud_offset_y = 0;
            this.rtTransform = null;
            if ((this.m_inOutEquipShopHudContainer != null) && (this.m_inOutEquipShopHud != null))
            {
                this.m_inOutEquipShopHudContainer.RecycleElement(this.m_inOutEquipShopHud);
            }
            this.m_inOutEquipShopHudContainer = null;
            this.m_inOutEquipShopHud = null;
            this._statusGo = null;
            if (this._mountPoint != null)
            {
                Object.Destroy(this._mountPoint);
                this._mountPoint = null;
            }
            this._shieldGo = null;
            this._bloodDecImage = null;
            this._curShield1 = 0;
            this._shieldImagWidth = 0f;
            this._isDecreasingHp = false;
            this._lastMaxBarValue = 0;
            this.m_effectRoot_small = null;
            this.m_actorForward = VInt3.zero;
            this.m_bHeroSameCamp = false;
            this.m_heroIconBG_small = null;
            this.m_heroIconBG_big = null;
            this.m_exclamationObj = null;
            this.m_exclamationObjOffsetY = 0f;
            this.ClearHeroProficiency();
            this.m_dirtyFlags = 0;
        }

        public void UpdateBloodBar(int curValue, int maxValue)
        {
            if ((this.m_hud != null) && (maxValue != 0))
            {
                if ((curValue == 0) || ((this.HudType == HudCompType.Type_Soldier) && (curValue >= maxValue)))
                {
                    if (!base.actor.TheStaticData.TheBaseAttribute.DeadControl)
                    {
                        this.setHudLogicVisible(false);
                    }
                    if (this.m_textHudNode != null)
                    {
                        this.setTextLogicVisible(false);
                    }
                }
                else
                {
                    this.setHudLogicVisible(true);
                    if (ActorHelper.IsHostCtrlActor(ref this.actorPtr) && ((this.m_hud.get_transform().get_parent().get_childCount() - 1) != this.m_hud.get_transform().GetSiblingIndex()))
                    {
                        this.m_hud.get_transform().SetAsLastSibling();
                    }
                    if (this.m_bloodImage != null)
                    {
                        this.m_bloodImage.fillAmount = ((float) curValue) / ((float) maxValue);
                        this.UpdateHpAndShieldBarEffect();
                        this.TryToStartDecreaseHpEffect();
                    }
                }
            }
        }

        protected void UpdateHpAndShieldBarEffect()
        {
            if (this._shieldGo != null)
            {
                this._shieldGo.CustomSetActive(this._curShield1 > 0);
                int curMaxHpBarValue = (this._curShield1 <= 0) ? base.actor.ValueComponent.actorHpTotal : Math.Max(base.actor.ValueComponent.actorHpTotal, this._curShield1 + base.actor.ValueComponent.actorHp);
                if (curMaxHpBarValue > 0f)
                {
                    this.m_bloodImage.fillAmount = ((float) base.actor.ValueComponent.actorHp) / ((float) curMaxHpBarValue);
                }
                else
                {
                    this.m_bloodImage.fillAmount = 0f;
                }
                this.SetBlackBar(curMaxHpBarValue);
                if (this._curShield1 > 0)
                {
                    float num2 = this._shieldImagWidth * this.m_bloodImage.fillAmount;
                    this._shieldGo.GetComponent<Sprite3D>().fillAmount = ((float) this._curShield1) / ((float) curMaxHpBarValue);
                    this._shieldGo.GetComponent<Transform>().set_localPosition(new Vector3(num2, 0f, 0f));
                }
            }
        }

        public override void UpdateLogic(int delta)
        {
        }

        public void UpdateMiniMapHeroRotation()
        {
            if ((base.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero) && this.m_bHeroSameCamp)
            {
                VInt3 forward = base.actor.forward;
                if (this.m_actorForward != forward)
                {
                    this.AddDirtyFlag(enDirtyFlag.ForwardInMap);
                    this.m_actorForward = forward;
                }
                if (base.actor.Visible && (this.IsNeedUpdateUI() || this.HasDirtyFlag(enDirtyFlag.ForwardInMap)))
                {
                    Quaternion quaternion = GetActorForward_MiniMap(ref this.m_actorForward);
                    if ((this.m_heroIconBG_small != null) && (this.m_heroIconBG_small.get_transform() != null))
                    {
                        this.m_heroIconBG_small.get_transform().set_rotation(quaternion);
                    }
                    if ((this.m_heroIconBG_big != null) && (this.m_heroIconBG_big.get_transform() != null))
                    {
                        this.m_heroIconBG_big.get_transform().set_rotation(quaternion);
                    }
                    this.RemoveDirtyFlag(enDirtyFlag.ForwardInMap);
                }
            }
        }

        public void UpdateShieldValue(ProtectType shieldType, int changeValue)
        {
            this._curShield1 += changeValue;
            if (this._curShield1 < 0)
            {
                this._curShield1 = 0;
            }
            this.UpdateHpAndShieldBarEffect();
        }

        public void UpdateTimerBar(int curValue, int totalValue)
        {
            if (this.m_timerImg != null)
            {
                this.m_timerImg.fillAmount = ((float) curValue) / ((float) totalValue);
            }
        }

        private void UpdateUIHud(ref Vector3 bloodPosition)
        {
            if (this.m_hud != null)
            {
                bloodPosition.Set(bloodPosition.x, bloodPosition.y, 30f);
                Camera currentCamera = Singleton<Camera_UI3D>.GetInstance().GetCurrentCamera();
                if (currentCamera != null)
                {
                    this.m_hud.get_transform().set_position(currentCamera.ScreenToWorldPoint(bloodPosition));
                    if (((this.m_textHudNode != null) && (Singleton<CBattleSystem>.GetInstance().FormScript != null)) && (!base.actor.ActorControl.IsDeadState || base.actor.TheStaticData.TheBaseAttribute.DeadControl))
                    {
                        Vector3 vector = CUIUtility.ScreenToWorldPoint(Singleton<CBattleSystem>.GetInstance().FormScript.GetCamera(), bloodPosition, this.m_textHudNode.get_transform().get_position().z);
                        this.m_textHudNode.get_transform().set_position(new Vector3(vector.x + this.txt_hud_offset_x, vector.y + this.txt_hud_offset_y, vector.z));
                    }
                }
            }
        }

        private void UpdateUIMap(ref Vector3 actorPosition)
        {
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            if ((curLvelContext != null) && curLvelContext.IsMobaMode())
            {
                this.UpdateUIMap_Inside(ref actorPosition);
            }
        }

        private void UpdateUIMap_Inside(ref Vector3 actorPosition)
        {
            if (this.m_mapPointer_small_Trans != null)
            {
                Vector3 vector = MiniMapSysUT.Set3DUIWorldPos_ByScreenPoint(ref actorPosition, true);
                this.m_mapPointer_small_Trans.set_position(vector);
                if (this.m_heroHead_small_Trans != null)
                {
                    this.m_heroHead_small_Trans.set_position(vector);
                }
            }
            if (this.m_mapPointer_big_Trans != null)
            {
                float num;
                float num2;
                Vector3 vector2 = MiniMapSysUT.Set3DUIWorldPos_ByScreenPoint(ref actorPosition, false, out num, out num2);
                this.m_mapPointer_big_Trans.set_position(vector2);
                Vector3 vector3 = this.m_mapPointer_big_Trans.get_localPosition();
                vector3.z = 0f;
                this.m_mapPointer_big_Trans.set_localPosition(vector3);
                if (this.m_heroHead_big_Trans != null)
                {
                    this.m_heroHead_big_Trans.set_position(vector2);
                    Vector3 vector4 = this.m_heroHead_big_Trans.get_localPosition();
                    vector4.z = 0f;
                    this.m_heroHead_big_Trans.set_localPosition(vector4);
                }
                if (this.m_evtCom != null)
                {
                    this.m_evtCom.m_screenSize.set_center(new Vector2(num, num2));
                }
            }
        }

        public GameObject MapPointerBig
        {
            get
            {
                return this.m_mapPointer_big;
            }
        }

        public GameObject MapPointerSmall
        {
            get
            {
                return this.m_mapPointer_small;
            }
        }

        public enum enDirtyFlag
        {
            ForcePositionInMap = 4,
            ForwardInMap = 2,
            Immediate = 8,
            PositionInMap = 1
        }
    }
}

