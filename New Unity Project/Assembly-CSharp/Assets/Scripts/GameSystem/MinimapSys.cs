namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.GameLogic.GameKernal;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class MinimapSys
    {
        private CUIFormScript _ownerForm;
        [CompilerGenerated]
        private GameObject <bmBGMap_3dui>k__BackingField;
        [CompilerGenerated]
        private Vector2 <bmFinalScreenSize>k__BackingField;
        [CompilerGenerated]
        private GameObject <bmHeroIconNode_Enemy>k__BackingField;
        [CompilerGenerated]
        private GameObject <bmHeroIconNode_Friend>k__BackingField;
        [CompilerGenerated]
        private GameObject <bmHeroIconNode_Self>k__BackingField;
        [CompilerGenerated]
        private GameObject <bmHeroIconNode>k__BackingField;
        [CompilerGenerated]
        private GameObject <bmpcAlies>k__BackingField;
        [CompilerGenerated]
        private GameObject <bmpcBlueBuff>k__BackingField;
        [CompilerGenerated]
        private GameObject <bmpcDragon_3dui>k__BackingField;
        [CompilerGenerated]
        private GameObject <bmpcDragon_ugui>k__BackingField;
        [CompilerGenerated]
        private GameObject <bmpcEffect>k__BackingField;
        [CompilerGenerated]
        private GameObject <bmpcEnemy>k__BackingField;
        [CompilerGenerated]
        private GameObject <bmpcEye>k__BackingField;
        [CompilerGenerated]
        private GameObject <bmpcHero>k__BackingField;
        [CompilerGenerated]
        private GameObject <bmpcJungle>k__BackingField;
        [CompilerGenerated]
        private GameObject <bmpcOrgan>k__BackingField;
        [CompilerGenerated]
        private GameObject <bmpcRedBuff>k__BackingField;
        [CompilerGenerated]
        private GameObject <bmpcSignal>k__BackingField;
        [CompilerGenerated]
        private GameObject <bmpcTrack>k__BackingField;
        [CompilerGenerated]
        private GameObject <bmRoot>k__BackingField;
        [CompilerGenerated]
        private GameObject <bmUGUIRoot>k__BackingField;
        [CompilerGenerated]
        private GameObject <mmBGMap_3dui>k__BackingField;
        [CompilerGenerated]
        private Vector2 <mmFinalScreenSize>k__BackingField;
        [CompilerGenerated]
        private GameObject <mmHeroIconNode_Enemy>k__BackingField;
        [CompilerGenerated]
        private GameObject <mmHeroIconNode_Friend>k__BackingField;
        [CompilerGenerated]
        private GameObject <mmHeroIconNode_Self>k__BackingField;
        [CompilerGenerated]
        private GameObject <mmHeroIconNode>k__BackingField;
        [CompilerGenerated]
        private GameObject <mmpcAlies>k__BackingField;
        [CompilerGenerated]
        private GameObject <mmpcBlueBuff>k__BackingField;
        [CompilerGenerated]
        private GameObject <mmpcDragon_3dui>k__BackingField;
        [CompilerGenerated]
        private GameObject <mmpcDragon_ugui>k__BackingField;
        [CompilerGenerated]
        private GameObject <mmpcEffect>k__BackingField;
        [CompilerGenerated]
        private GameObject <mmpcEnemy>k__BackingField;
        [CompilerGenerated]
        private GameObject <mmpcEye>k__BackingField;
        [CompilerGenerated]
        private GameObject <mmpcHero>k__BackingField;
        [CompilerGenerated]
        private GameObject <mmpcJungle>k__BackingField;
        [CompilerGenerated]
        private GameObject <mmpcOrgan>k__BackingField;
        [CompilerGenerated]
        private GameObject <mmpcRedBuff>k__BackingField;
        [CompilerGenerated]
        private GameObject <mmpcSignal>k__BackingField;
        [CompilerGenerated]
        private GameObject <mmpcTrack>k__BackingField;
        [CompilerGenerated]
        private GameObject <mmRoot>k__BackingField;
        [CompilerGenerated]
        private GameObject <mmUGUIRoot>k__BackingField;
        private Vector3 cachePos = Vector3.get_zero();
        private EMapType curMapType;
        public static float DEPTH = 30f;
        public static string enemy_base = "UGUI/Sprite/Battle/Img_Map_Base_Red";
        public static string enemy_base_outline = "UGUI/Sprite/Battle/Img_Map_Base_Red_outline";
        public static string enemy_Eye = "UGUI/Sprite/Battle/Img_Map_RedEye";
        public static string enemy_tower = "UGUI/Sprite/Battle/Img_Map_Tower_Red";
        public static string enemy_tower_outline = "UGUI/Sprite/Battle/Img_Map_Tower_Red_outline";
        private Vector2 m_bmFinalScreenPos;
        private Vector2 m_bmStandardScreenPos;
        private List<BornRecord> m_bornRecords = new List<BornRecord>();
        private SkillSlotType m_CurSelectedSlotType = SkillSlotType.SLOT_SKILL_COUNT;
        private DragonIcon m_dragonIcon;
        private int m_elementIndex;
        private List<ElementInMap> m_mapElements = new List<ElementInMap>();
        private Vector2 m_mmFinalScreenPos;
        private Vector2 m_mmStandardScreenPos;
        private COM_PLAYERCAMP m_playerCamp = COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT;
        private string miniMapPrefabPath = "UI3D/Battle/MiniMap/Mini_Map.prefab";
        public BackCityCom_3DUI MMiniMapBackCityCom_3Dui;
        public MiniMapCameraFrame_3DUI MMiniMapCameraFrame_3Dui;
        public MinimapSkillIndicator_3DUI MMinimapSkillIndicator_3Dui;
        public MiniMapTrack_3DUI MMiniMapTrack_3Dui;
        public static string self_base = "UGUI/Sprite/Battle/Img_Map_Base_Green";
        public static string self_base_outline = "UGUI/Sprite/Battle/Img_Map_Base_Green_outline";
        public static string self_Eye = "UGUI/Sprite/Battle/Img_Map_BlueEye";
        public static string self_tower = "UGUI/Sprite/Battle/Img_Map_Tower_Green";
        public static string self_tower_outline = "UGUI/Sprite/Battle/Img_Map_Tower_Green_outline";
        public Assets.Scripts.GameSystem.UI3DEventMgr UI3DEventMgr = new Assets.Scripts.GameSystem.UI3DEventMgr();

        public void ChangeBigMapSide(bool bLeft)
        {
            if (!Singleton<WatchController>.instance.IsWatching && (((this.bmBGMap_3dui != null) && (this.bmRoot != null)) && (this.bmUGUIRoot != null)))
            {
                Sprite3D component = this.bmBGMap_3dui.GetComponent<Sprite3D>();
                if (component == null)
                {
                }
                this.m_bmFinalScreenPos.x = !bLeft ? (Screen.get_width() - ((component.textureWidth * 0.5f) * Sprite3D.Ratio())) : ((component.textureWidth * 0.5f) * Sprite3D.Ratio());
                this.m_bmFinalScreenPos.y = Screen.get_height() - ((component.textureHeight * 0.5f) * Sprite3D.Ratio());
                if (bLeft)
                {
                    if (this.cachePos == Vector3.get_zero())
                    {
                        this.cachePos = this.Get3DUIObj_WorldPos(this.m_bmFinalScreenPos.x, this.m_bmFinalScreenPos.y);
                    }
                    component.get_transform().set_position(this.cachePos);
                }
                if (!bLeft)
                {
                    component.get_transform().set_position(this.Get3DUIObj_WorldPos(this.m_bmFinalScreenPos.x, this.m_bmFinalScreenPos.y));
                }
                List<PoolObjHandle<ActorRoot>> gameActors = Singleton<GameObjMgr>.GetInstance().GameActors;
                for (int i = 0; i < gameActors.Count; i++)
                {
                    PoolObjHandle<ActorRoot> handle = gameActors[i];
                    if (((handle != 0) && (handle.handle != null)) && (handle.handle.HudControl != null))
                    {
                        handle.handle.HudControl.AddForceUpdateFlag();
                    }
                }
                if (this._ownerForm != null)
                {
                    RectTransform transform = this.bmUGUIRoot.get_transform() as RectTransform;
                    transform.set_sizeDelta(new Vector2((float) component.textureWidth, (float) component.textureHeight));
                    float num2 = !bLeft ? (this._ownerForm.ChangeScreenValueToForm((float) Screen.get_width()) - (component.textureWidth * 0.5f)) : (component.textureWidth * 0.5f);
                    transform.set_anchoredPosition(new Vector2(num2, -component.textureHeight * 0.5f));
                }
                if (this.m_dragonIcon != null)
                {
                    this.m_dragonIcon.RefreshDragNode(bLeft, false);
                }
                MiniMapSysUT.RefreshMapPointerBig(this.bmUGUIRoot);
            }
        }

        public void ChangeMapPointerDepth(EMapType mapType)
        {
            if (mapType == EMapType.Skill)
            {
                this.bmpcOrgan.get_transform().set_localPosition(new Vector3(0f, 0f, -20f));
                this.bmpcEye.get_transform().set_localPosition(new Vector3(0f, 0f, -21f));
            }
            else
            {
                this.bmpcOrgan.get_transform().set_localPosition(Vector3.get_zero());
                this.bmpcEye.get_transform().set_localPosition(Vector3.get_zero());
            }
        }

        public void Clear()
        {
            this.cachePos = Vector3.get_zero();
            this.unRegEvent();
            if (this.MMiniMapTrack_3Dui != null)
            {
                this.MMiniMapTrack_3Dui.Clear();
                this.MMiniMapTrack_3Dui = null;
            }
            if (this.m_dragonIcon != null)
            {
                this.m_dragonIcon.Clear();
                this.m_dragonIcon = null;
            }
            if (this.MMiniMapCameraFrame_3Dui != null)
            {
                this.MMiniMapCameraFrame_3Dui.Clear();
                this.MMiniMapCameraFrame_3Dui = null;
            }
            if (this.MMinimapSkillIndicator_3Dui != null)
            {
                this.MMinimapSkillIndicator_3Dui.Clear();
                this.MMinimapSkillIndicator_3Dui = null;
            }
            this.mmRoot = null;
            this.bmRoot = null;
            this.mmBGMap_3dui = null;
            this.mmpcAlies = null;
            this.mmpcHero = null;
            this.mmpcRedBuff = null;
            this.mmpcBlueBuff = null;
            this.mmpcJungle = null;
            this.mmpcEnemy = null;
            this.mmpcOrgan = null;
            this.mmpcSignal = null;
            this.mmpcDragon_ugui = null;
            this.mmpcDragon_3dui = null;
            this.mmpcEffect = null;
            this.mmpcEye = null;
            this.mmpcTrack = null;
            this.mmHeroIconNode = null;
            this.mmHeroIconNode_Self = null;
            this.mmHeroIconNode_Friend = null;
            this.mmHeroIconNode_Enemy = null;
            this.bmBGMap_3dui = null;
            this.bmpcAlies = null;
            this.bmpcHero = null;
            this.bmpcRedBuff = null;
            this.bmpcBlueBuff = null;
            this.bmpcJungle = null;
            this.bmpcEnemy = null;
            this.bmpcOrgan = null;
            this.bmpcSignal = null;
            this.bmpcDragon_ugui = null;
            this.bmpcDragon_3dui = null;
            this.mmpcEffect = null;
            this.bmpcEye = null;
            this.bmpcTrack = null;
            this.bmHeroIconNode = null;
            this.bmHeroIconNode_Self = null;
            this.bmHeroIconNode_Friend = null;
            this.bmHeroIconNode_Enemy = null;
            this._ownerForm = null;
            this.m_playerCamp = COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT;
            this.m_elementIndex = 0;
            for (int i = 0; i < this.m_mapElements.Count; i++)
            {
                if (this.m_mapElements[i].smallElement != null)
                {
                    MiniMapSysUT.RecycleMapGameObject(this.m_mapElements[i].smallElement);
                    this.m_mapElements[i].smallElement = null;
                }
                if (this.m_mapElements[i].bigElement != null)
                {
                    MiniMapSysUT.RecycleMapGameObject(this.m_mapElements[i].bigElement);
                    this.m_mapElements[i].bigElement = null;
                }
            }
            this.m_mapElements.Clear();
            this.m_bornRecords.Clear();
        }

        public void ClearMapSkillStatus()
        {
            if (this.curMapType == EMapType.Skill)
            {
                this.Switch(EMapType.Mini);
            }
        }

        public EMapType CurMapType()
        {
            return this.curMapType;
        }

        private Vector3 Get3DUIObj_WorldPos(float finalScreenX, float finalScreenY)
        {
            Camera currentCamera = Singleton<Camera_UI3D>.GetInstance().GetCurrentCamera();
            if (currentCamera == null)
            {
                return Vector3.get_zero();
            }
            Vector3 vector = new Vector3(finalScreenX, finalScreenY, DEPTH);
            return currentCamera.ScreenToWorldPoint(vector);
        }

        public Vector2 GetBMFianlScreenPos()
        {
            return this.m_bmFinalScreenPos;
        }

        public Vector2 GetBMStandardScreenPos()
        {
            return this.m_bmStandardScreenPos;
        }

        private void GetCacheObj()
        {
            this.mmpcAlies = Utility.FindChild(this.mmRoot, "Container_MiniMapPointer_Alies");
            this.mmpcHero = Utility.FindChild(this.mmRoot, "Container_MiniMapPointer_Hero");
            this.mmpcRedBuff = Utility.FindChild(this.mmRoot, "Container_MiniMapPointer_RedBuff");
            this.mmpcBlueBuff = Utility.FindChild(this.mmRoot, "Container_MiniMapPointer_BlueBuff");
            this.mmpcJungle = Utility.FindChild(this.mmRoot, "Container_MiniMapPointer_Jungle");
            this.mmpcEnemy = Utility.FindChild(this.mmRoot, "Container_MiniMapPointer_Enemy");
            this.mmpcOrgan = Utility.FindChild(this.mmRoot, "Container_MiniMapPointer_Organ");
            this.mmpcSignal = Utility.FindChild(this.mmRoot, "Container_MiniMapPointer_Signal");
            this.mmpcDragon_ugui = Utility.FindChild(this._ownerForm.get_gameObject(), "MapPanel/Mini/Container_MiniMapPointer_Dragon");
            this.mmpcDragon_ugui.CustomSetActive(true);
            this.mmpcDragon_3dui = Utility.FindChild(this.mmRoot, "Container_MiniMapPointer_Dragon");
            this.mmpcEffect = Utility.FindChild(this.mmRoot, "BigMapEffectRoot");
            this.mmpcEye = Utility.FindChild(this.mmRoot, "Container_MiniMapPointer_Eye");
            this.mmpcTrack = Utility.FindChild(this.mmRoot, "Container_MiniMapPointer_Track");
            this.mmHeroIconNode = Utility.FindChild(this.mmRoot, "HeroIconNode");
            this.mmHeroIconNode_Self = Utility.FindChild(this.mmRoot, "HeroIconNode/self");
            this.mmHeroIconNode_Friend = Utility.FindChild(this.mmRoot, "HeroIconNode/friend");
            this.mmHeroIconNode_Enemy = Utility.FindChild(this.mmRoot, "HeroIconNode/enemy");
            this.bmpcAlies = Utility.FindChild(this.bmRoot, "Container_BigMapPointer_Alies");
            this.bmpcHero = Utility.FindChild(this.bmRoot, "Container_BigMapPointer_Hero");
            this.bmpcRedBuff = Utility.FindChild(this.bmRoot, "Container_BigMapPointer_RedBuff");
            this.bmpcBlueBuff = Utility.FindChild(this.bmRoot, "Container_BigMapPointer_BlueBuff");
            this.bmpcJungle = Utility.FindChild(this.bmRoot, "Container_BigMapPointer_Jungle");
            this.bmpcEnemy = Utility.FindChild(this.bmRoot, "Container_BigMapPointer_Enemy");
            this.bmpcOrgan = Utility.FindChild(this.bmRoot, "Container_BigMapPointer_Organ");
            this.bmpcSignal = Utility.FindChild(this.bmRoot, "Container_BigMapPointer_Signal");
            this.bmpcDragon_ugui = Utility.FindChild(this._ownerForm.get_gameObject(), "MapPanel/Big/Container_BigMapPointer_Dragon");
            this.bmpcDragon_ugui.CustomSetActive(true);
            this.bmpcDragon_3dui = Utility.FindChild(this.bmRoot, "Container_BigMapPointer_Dragon");
            this.mmpcEffect = Utility.FindChild(this.bmRoot, "BigMapEffectRoot");
            this.bmpcEye = Utility.FindChild(this.bmRoot, "Container_BigMapPointer_Eye");
            this.bmpcTrack = Utility.FindChild(this.bmRoot, "Container_BigMapPointer_Track");
            this.bmHeroIconNode = Utility.FindChild(this.bmRoot, "HeroIconNode");
            this.bmHeroIconNode_Self = Utility.FindChild(this.bmRoot, "HeroIconNode/self");
            this.bmHeroIconNode_Friend = Utility.FindChild(this.bmRoot, "HeroIconNode/friend");
            this.bmHeroIconNode_Enemy = Utility.FindChild(this.bmRoot, "HeroIconNode/enemy");
        }

        public Vector2 GetMMFianlScreenPos()
        {
            return this.m_mmFinalScreenPos;
        }

        public Vector2 GetMMStandardScreenPos()
        {
            return this.m_mmStandardScreenPos;
        }

        public void Init(CUIFormScript formObj, SLevelContext levelContext)
        {
            if (formObj != null)
            {
                this.m_playerCamp = Singleton<GamePlayerCenter>.instance.hostPlayerCamp;
                this._ownerForm = formObj;
                this.mmUGUIRoot = Utility.FindChild(formObj.get_gameObject(), "MapPanel/Mini");
                this.bmUGUIRoot = Utility.FindChild(formObj.get_gameObject(), "MapPanel/Big");
                if (!levelContext.IsMobaMode())
                {
                    this.mmUGUIRoot.CustomSetActive(false);
                    this.bmUGUIRoot.CustomSetActive(false);
                }
                else
                {
                    GameObject gameObject = Singleton<CGameObjectPool>.GetInstance().GetGameObject(this.miniMapPrefabPath, enResourceType.BattleScene);
                    DebugHelper.Assert(gameObject != null, "---minimap3DUI is null...");
                    gameObject.set_name("Mini_Map");
                    Camera currentCamera = Singleton<Camera_UI3D>.GetInstance().GetCurrentCamera();
                    if (currentCamera != null)
                    {
                        gameObject.get_transform().set_parent(currentCamera.get_transform());
                        gameObject.get_transform().set_localPosition(new Vector3(0f, 0f, DEPTH));
                        this.mmRoot = Utility.FindChild(gameObject.get_gameObject(), "Mini");
                        this.bmRoot = Utility.FindChild(gameObject.get_gameObject(), "Big");
                        if ((this.mmRoot != null) && (this.bmRoot != null))
                        {
                            this.mmRoot.CustomSetActive(true);
                            this.bmRoot.CustomSetActive(false);
                            string prefabFullPath = CUIUtility.s_Sprite_Dynamic_Map_Dir + levelContext.m_miniMapPath;
                            string str2 = CUIUtility.s_Sprite_Dynamic_Map_Dir + levelContext.m_bigMapPath;
                            this.mmBGMap_3dui = Singleton<CGameObjectPool>.GetInstance().GetGameObject(prefabFullPath, enResourceType.UI3DImage);
                            this.bmBGMap_3dui = Singleton<CGameObjectPool>.GetInstance().GetGameObject(str2, enResourceType.UI3DImage);
                            this.mmBGMap_3dui.get_transform().SetParent(this.mmRoot.get_transform(), true);
                            this.mmBGMap_3dui.get_transform().set_localScale(new Vector3(1f, 1f, 1f));
                            this.mmBGMap_3dui.get_transform().set_localRotation(Quaternion.get_identity());
                            this.bmBGMap_3dui.get_transform().SetParent(this.bmRoot.get_transform(), true);
                            this.bmBGMap_3dui.get_transform().set_localScale(new Vector3(1f, 1f, 1f));
                            this.bmBGMap_3dui.get_transform().set_localRotation(Quaternion.get_identity());
                            this.mmBGMap_3dui.get_transform().SetAsFirstSibling();
                            this.bmBGMap_3dui.get_transform().SetAsFirstSibling();
                            Singleton<Camera_UI3D>.GetInstance().GetCurrentCanvas().RefreshLayout(null);
                            MiniMapSysUT.NativeSizeLize(gameObject);
                            if (!levelContext.IsMobaMode())
                            {
                                this.mmRoot.SetActive(false);
                                this.bmRoot.SetActive(false);
                            }
                            else if (levelContext != null)
                            {
                                this.regEvent();
                                this.GetCacheObj();
                                Sprite3D component = this.mmBGMap_3dui.GetComponent<Sprite3D>();
                                Sprite3D sprite = this.bmBGMap_3dui.GetComponent<Sprite3D>();
                                if (levelContext.IsMobaMode())
                                {
                                    this.Switch(EMapType.Mini);
                                    if (component != null)
                                    {
                                        this.initWorldUITransformFactor(new Vector2((float) component.textureWidth, (float) component.textureHeight), levelContext, true, out Singleton<CBattleSystem>.instance.world_UI_Factor_Small, out Singleton<CBattleSystem>.instance.UI_world_Factor_Small, component);
                                    }
                                    if (sprite != null)
                                    {
                                        this.initWorldUITransformFactor(new Vector2((float) sprite.textureWidth, (float) sprite.textureHeight), levelContext, false, out Singleton<CBattleSystem>.instance.world_UI_Factor_Big, out Singleton<CBattleSystem>.instance.UI_world_Factor_Big, sprite);
                                    }
                                    if (component != null)
                                    {
                                        this.m_mmFinalScreenPos.x = (component.textureWidth * 0.5f) * Sprite3D.Ratio();
                                        this.m_mmFinalScreenPos.y = Screen.get_height() - ((component.textureHeight * 0.5f) * Sprite3D.Ratio());
                                        component.get_transform().set_position(this.Get3DUIObj_WorldPos(this.m_mmFinalScreenPos.x, this.m_mmFinalScreenPos.y));
                                        this.mmFinalScreenSize = new Vector2(component.textureWidth * Sprite3D.Ratio(), component.textureHeight * Sprite3D.Ratio());
                                    }
                                    if (sprite != null)
                                    {
                                        this.m_bmFinalScreenPos.x = (sprite.textureWidth * 0.5f) * Sprite3D.Ratio();
                                        this.m_bmFinalScreenPos.y = Screen.get_height() - ((sprite.textureHeight * 0.5f) * Sprite3D.Ratio());
                                        sprite.get_transform().set_position(this.Get3DUIObj_WorldPos(this.m_bmFinalScreenPos.x, this.m_bmFinalScreenPos.y));
                                        this.bmFinalScreenSize = new Vector2(sprite.textureWidth * Sprite3D.Ratio(), sprite.textureHeight * Sprite3D.Ratio());
                                    }
                                    if (component != null)
                                    {
                                        this.m_mmStandardScreenPos.x = component.textureWidth * 0.5f;
                                        this.m_mmStandardScreenPos.y = component.textureHeight * 0.5f;
                                    }
                                    if (sprite != null)
                                    {
                                        this.m_bmStandardScreenPos.x = sprite.textureWidth * 0.5f;
                                        this.m_bmStandardScreenPos.y = sprite.textureHeight * 0.5f;
                                    }
                                    RectTransform transform2 = this.mmUGUIRoot.get_transform() as RectTransform;
                                    transform2.set_sizeDelta(new Vector2((float) component.textureWidth, (float) component.textureHeight));
                                    transform2.set_anchoredPosition(new Vector2(component.textureWidth * 0.5f, -component.textureHeight * 0.5f));
                                    RectTransform transform3 = this.bmUGUIRoot.get_transform() as RectTransform;
                                    transform3.set_sizeDelta(new Vector2((float) sprite.textureWidth, (float) sprite.textureHeight));
                                    transform3.set_anchoredPosition(new Vector2(sprite.textureWidth * 0.5f, -sprite.textureHeight * 0.5f));
                                    MiniMapSysUT.RefreshMapPointerBig(this.mmUGUIRoot);
                                    MiniMapSysUT.RefreshMapPointerBig(this.bmUGUIRoot);
                                    if (levelContext.m_pvpPlayerNum == 6)
                                    {
                                        GameObject obj3 = Utility.FindChild(this._ownerForm.get_gameObject(), "MapPanel/DragonInfo");
                                        if (obj3 != null)
                                        {
                                            RectTransform transform4 = obj3.get_gameObject().get_transform() as RectTransform;
                                            transform4.set_anchoredPosition(new Vector2(transform2.get_anchoredPosition().x, transform4.get_anchoredPosition().y));
                                        }
                                    }
                                }
                                else
                                {
                                    this.Switch(EMapType.None);
                                }
                                this.curMapType = EMapType.Mini;
                                bool flag = false;
                                bool flag2 = false;
                                if (levelContext.m_pveLevelType == RES_LEVEL_TYPE.RES_LEVEL_TYPE_GUIDE)
                                {
                                    switch (levelContext.m_mapID)
                                    {
                                        case 2:
                                            flag = true;
                                            flag2 = false;
                                            break;

                                        case 3:
                                        case 6:
                                        case 7:
                                            flag = true;
                                            flag2 = true;
                                            break;
                                    }
                                }
                                else if ((levelContext.m_pvpPlayerNum == 6) || (levelContext.m_pvpPlayerNum == 10))
                                {
                                    flag = true;
                                    flag2 = levelContext.m_pvpPlayerNum == 10;
                                }
                                if (flag && (this.mmpcDragon_ugui != null))
                                {
                                    this.m_dragonIcon = new DragonIcon();
                                    this.m_dragonIcon.Init(this.mmpcDragon_ugui, this.bmpcDragon_ugui, this.mmpcDragon_3dui, this.bmpcDragon_3dui, flag2);
                                    this.mmpcDragon_ugui.CustomSetActive(true);
                                    this.bmpcDragon_ugui.CustomSetActive(true);
                                }
                                else
                                {
                                    this.mmpcDragon_ugui.CustomSetActive(false);
                                    this.bmpcDragon_ugui.CustomSetActive(false);
                                }
                                GameObject node = this.mmUGUIRoot.get_transform().Find("CameraFrame").get_gameObject();
                                if (node != null)
                                {
                                    this.MMiniMapCameraFrame_3Dui = new MiniMapCameraFrame_3DUI(node, (float) component.textureWidth, (float) component.textureHeight);
                                    this.MMiniMapCameraFrame_3Dui.SetFrameSize((CameraHeightType) GameSettings.CameraHeight);
                                    this.MMiniMapBackCityCom_3Dui = new BackCityCom_3DUI();
                                    this.MMiniMapTrack_3Dui = new MiniMapTrack_3DUI();
                                    this.MMinimapSkillIndicator_3Dui = new MinimapSkillIndicator_3DUI();
                                    GameObject miniTrackNode = Utility.FindChild(this.mmUGUIRoot, "Track");
                                    GameObject bigTrackNode = Utility.FindChild(this.bmUGUIRoot, "Track");
                                    this.MMinimapSkillIndicator_3Dui.Init(miniTrackNode, bigTrackNode);
                                    this.ChangeBigMapSide(true);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void initWorldUITransformFactor(Vector2 mapImgSize, SLevelContext levelContext, bool bMiniMap, out Vector2 world_UI_Factor, out Vector2 UI_world_Factor, Sprite3D sprite)
        {
            int num = !bMiniMap ? levelContext.m_bigMapWidth : levelContext.m_mapWidth;
            int num2 = !bMiniMap ? levelContext.m_bigMapHeight : levelContext.m_mapHeight;
            float num3 = mapImgSize.x / ((float) num);
            float num4 = mapImgSize.y / ((float) num2);
            world_UI_Factor = new Vector2(num3, num4);
            float num5 = ((float) num) / mapImgSize.x;
            float num6 = ((float) num2) / mapImgSize.y;
            UI_world_Factor = new Vector2(num5, num6);
            if (levelContext.m_isCameraFlip)
            {
                world_UI_Factor = new Vector2(-num3, -num4);
                UI_world_Factor = new Vector2(-num5, -num6);
            }
            if (null != sprite)
            {
                float num7 = !bMiniMap ? levelContext.m_bigMapFowScale : levelContext.m_mapFowScale;
                float num8 = !levelContext.m_isCameraFlip ? 1f : 0f;
                sprite.atlas.material.SetVector("_FowParams", new Vector4(num7, num8, 1f, 1f));
            }
        }

        public void Move_CameraToClickPosition(CUIEvent uiEvent)
        {
            if (MonoSingleton<CameraSystem>.GetInstance().enableLockedCamera)
            {
                MonoSingleton<CameraSystem>.instance.ToggleFreeDragCamera(true);
                if (Singleton<CBattleSystem>.GetInstance().WatchForm != null)
                {
                    Singleton<CBattleSystem>.GetInstance().WatchForm.OnCamerFreed();
                }
            }
            if (((uiEvent.m_srcFormScript != null) && (uiEvent.m_srcWidget != null)) && (uiEvent.m_pointerEventData != null))
            {
                Vector2 vector = uiEvent.m_pointerEventData.get_position();
                MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
                if (theMinimapSys != null)
                {
                    Vector2 mMFianlScreenPos = theMinimapSys.GetMMFianlScreenPos();
                    float num = vector.x - mMFianlScreenPos.x;
                    float num2 = vector.y - mMFianlScreenPos.y;
                    num = uiEvent.m_srcFormScript.ChangeScreenValueToForm(num);
                    num2 = uiEvent.m_srcFormScript.ChangeScreenValueToForm(num2);
                    float num3 = num * Singleton<CBattleSystem>.instance.UI_world_Factor_Small.x;
                    float num4 = num2 * Singleton<CBattleSystem>.instance.UI_world_Factor_Small.y;
                    if (MonoSingleton<CameraSystem>.instance.MobaCamera != null)
                    {
                        MonoSingleton<CameraSystem>.instance.MobaCamera.SetAbsoluteLockLocation(new Vector3(num3, 1f, num4));
                        if (this.MMiniMapCameraFrame_3Dui != null)
                        {
                            if (!this.MMiniMapCameraFrame_3Dui.IsCameraFrameShow)
                            {
                                this.MMiniMapCameraFrame_3Dui.Show();
                                this.MMiniMapCameraFrame_3Dui.ShowNormal();
                            }
                            this.MMiniMapCameraFrame_3Dui.SetPos(num, num2);
                        }
                    }
                }
            }
        }

        public void OnActorDamage(ref HurtEventResultInfo hri)
        {
            if (((this.MMiniMapCameraFrame_3Dui != null) && this.MMiniMapCameraFrame_3Dui.IsCameraFrameShow) && (hri.hurtInfo.hurtType != HurtTypeDef.Therapic))
            {
                Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
                if (((hri.src != 0) && (hostPlayer != null)) && ((hostPlayer.Captain != 0) && (hostPlayer.Captain == hri.src)))
                {
                    this.MMiniMapCameraFrame_3Dui.ShowRed();
                }
            }
        }

        private void OnActorDead(ref GameDeadEventParam prm)
        {
            if (ActorHelper.IsHostCtrlActor(ref prm.src) && (this.curMapType == EMapType.Skill))
            {
                this.Switch(EMapType.Mini);
            }
        }

        private void OnBigMap_Click_3_long(CUIEvent uievent)
        {
            this.send_signal(uievent, this.bmRoot, ElementType.Dragon_3, 0);
        }

        private void OnBigMap_Click_5_Dalong(CUIEvent uievent)
        {
            this.send_signal(uievent, this.bmRoot, ElementType.Dragon_5_big, 0);
        }

        private void OnBigMap_Click_5_Xiaolong(CUIEvent uievent)
        {
            this.send_signal(uievent, this.bmRoot, ElementType.Dragon_5_small, 0);
        }

        private void OnBigMap_Click_Eye(CUIEvent uievent)
        {
            this.send_signal(uievent, this.bmRoot, ElementType.None, 0);
        }

        private void OnBigMap_Click_Hero(CUIEvent uievent)
        {
            this.send_signal(uievent, this.bmRoot, ElementType.None, 0);
        }

        private void OnBigMap_Click_Map(CUIEvent uievent)
        {
            if (this.curMapType == EMapType.Skill)
            {
                if (!this.UI3DEventMgr.HandleSkillClickEvent(uievent.m_pointerEventData))
                {
                    this.send_signal(uievent, this.bmRoot, ElementType.None, 1);
                }
            }
            else if (!this.UI3DEventMgr.HandleClickEvent(uievent.m_pointerEventData))
            {
                this.send_signal(uievent, this.bmRoot, ElementType.None, 1);
            }
        }

        private void OnBigMap_Click_Organ(CUIEvent uievent)
        {
            this.send_signal(uievent, this.bmRoot, ElementType.None, 0);
        }

        private void OnBigMap_Open_BigMap(CUIEvent uievent)
        {
            CPlayerBehaviorStat.Plus(CPlayerBehaviorStat.BehaviorType.Battle_OpenBigMap);
            this.Switch(EMapType.Big);
            this.RefreshMapPointers();
        }

        private void OnBuildingAttacked(ref DefaultGameEventParam evtParam)
        {
            if (evtParam.src != 0)
            {
                ActorRoot handle = evtParam.src.handle;
                if ((handle != null) && HudUT.IsTower(handle))
                {
                    HudComponent3D hudControl = handle.HudControl;
                    if (hudControl != null)
                    {
                        GameObject mapPointerSmall = hudControl.MapPointerSmall;
                        TowerHitMgr towerHitMgr = Singleton<CBattleSystem>.GetInstance().TowerHitMgr;
                        if ((mapPointerSmall != null) && (towerHitMgr != null))
                        {
                            towerHitMgr.TryActive(handle.ObjID, mapPointerSmall);
                        }
                        Sprite3D sprited = hudControl.GetBigTower_Spt3D();
                        Sprite3D sprited2 = hudControl.GetSmallTower_Spt3D();
                        if (((sprited != null) && (sprited2 != null)) && (handle.ValueComponent != null))
                        {
                            float single = handle.ValueComponent.GetHpRate().single;
                            sprited.fillAmount = single;
                            sprited2.fillAmount = single;
                        }
                    }
                }
            }
        }

        private void OnCloseBigMap(CUIEvent uiEvent)
        {
            if (this.curMapType == EMapType.Big)
            {
                this.Switch(EMapType.Mini);
            }
        }

        private void OnDragMiniMap(CUIEvent uiEvent)
        {
            CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
            if (((uiEvent != null) && (srcFormScript != null)) && ((uiEvent.m_pointerEventData != null) && (uiEvent.m_srcWidget != null)))
            {
                if (MonoSingleton<CameraSystem>.GetInstance().enableLockedCamera)
                {
                    MonoSingleton<CameraSystem>.instance.ToggleFreeDragCamera(true);
                    if (Singleton<CBattleSystem>.GetInstance().WatchForm != null)
                    {
                        Singleton<CBattleSystem>.GetInstance().WatchForm.OnCamerFreed();
                    }
                }
                Vector2 vector = uiEvent.m_pointerEventData.get_position();
                MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
                if (theMinimapSys != null)
                {
                    Vector2 mMFianlScreenPos = theMinimapSys.GetMMFianlScreenPos();
                    float num = vector.x - mMFianlScreenPos.x;
                    float num2 = vector.y - mMFianlScreenPos.y;
                    num = uiEvent.m_srcFormScript.ChangeScreenValueToForm(num);
                    num2 = uiEvent.m_srcFormScript.ChangeScreenValueToForm(num2);
                    float num3 = num * Singleton<CBattleSystem>.instance.UI_world_Factor_Small.x;
                    float num4 = num2 * Singleton<CBattleSystem>.instance.UI_world_Factor_Small.y;
                    if (MonoSingleton<CameraSystem>.instance.MobaCamera != null)
                    {
                        MonoSingleton<CameraSystem>.instance.MobaCamera.SetAbsoluteLockLocation(new Vector3(num3, 1f, num4));
                    }
                    if (this.MMiniMapCameraFrame_3Dui != null)
                    {
                        if (!this.MMiniMapCameraFrame_3Dui.IsCameraFrameShow)
                        {
                            this.MMiniMapCameraFrame_3Dui.Show();
                            this.MMiniMapCameraFrame_3Dui.ShowNormal();
                        }
                        this.MMiniMapCameraFrame_3Dui.SetPos(num, num2);
                    }
                }
            }
        }

        private void OnDragMiniMapEnd(CUIEvent uievent)
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
            if (((!Singleton<WatchController>.GetInstance().IsWatching && (hostPlayer != null)) && ((hostPlayer.Captain != 0) && (hostPlayer.Captain.handle != null))) && ((hostPlayer.Captain.handle.ActorControl != null) && !hostPlayer.Captain.handle.ActorControl.IsDeadState))
            {
                MonoSingleton<CameraSystem>.instance.ToggleFreeDragCamera(false);
                if (this.MMiniMapCameraFrame_3Dui != null)
                {
                    this.MMiniMapCameraFrame_3Dui.Hide();
                }
            }
        }

        private void OnMiniMap_Click_Down(CUIEvent uievent)
        {
            SignalPanel theSignalPanel = Singleton<CBattleSystem>.GetInstance().TheSignalPanel;
            if (theSignalPanel == null)
            {
                this.Move_CameraToClickPosition(uievent);
            }
            else if (!theSignalPanel.IsUseSingalButton())
            {
                this.Move_CameraToClickPosition(uievent);
            }
        }

        private void OnMiniMap_Click_Up(CUIEvent uievent)
        {
            if (this.MMiniMapCameraFrame_3Dui != null)
            {
                this.MMiniMapCameraFrame_3Dui.Hide();
            }
            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
            if (((!Singleton<WatchController>.GetInstance().IsWatching && (hostPlayer != null)) && ((hostPlayer.Captain != 0) && (hostPlayer.Captain.handle.ActorControl != null))) && (!hostPlayer.Captain.handle.ActorControl.IsDeadState || hostPlayer.Captain.handle.TheStaticData.TheBaseAttribute.DeadControl))
            {
                MonoSingleton<CameraSystem>.instance.ToggleFreeDragCamera(false);
            }
        }

        private void OnMonsterGroupDead(ref GameDeadEventParam evtParam)
        {
            if (FogOfWar.enable && !Singleton<WatchController>.instance.IsWatching)
            {
                SpawnGroup spawnPoint = evtParam.spawnPoint as SpawnGroup;
                if (((evtParam.src != 0) && (evtParam.orignalAtker != 0)) && (spawnPoint != null))
                {
                    bool flag = (evtParam.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster) && (evtParam.src.handle.ActorControl.GetActorSubType() == 2);
                    byte actorSubSoliderType = evtParam.src.handle.ActorControl.GetActorSubSoliderType();
                    bool flag2 = ((actorSubSoliderType == 7) || (actorSubSoliderType == 8)) || (actorSubSoliderType == 9);
                    if ((flag && !flag2) && (Singleton<GamePlayerCenter>.instance.GetHostPlayer().PlayerCamp != evtParam.orignalAtker.handle.TheActorMeta.ActorCamp))
                    {
                        GameObject mapPointerSmall = evtParam.src.handle.HudControl.MapPointerSmall;
                        GameObject mapPointerBig = evtParam.src.handle.HudControl.MapPointerBig;
                        if ((mapPointerSmall == null) || (mapPointerBig == null))
                        {
                        }
                    }
                }
            }
        }

        public void RefreshMapPointers()
        {
            List<PoolObjHandle<ActorRoot>> heroActors = Singleton<GameObjMgr>.GetInstance().HeroActors;
            for (int i = 0; i < heroActors.Count; i++)
            {
                PoolObjHandle<ActorRoot> handle = heroActors[i];
                if (((handle != 0) && (handle.handle != null)) && (handle.handle.HudControl != null))
                {
                    handle.handle.HudControl.RefreshMapPointerBig();
                }
            }
        }

        private void regEvent()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BigMap_Open_BigMap, new CUIEventManager.OnUIEventHandler(this.OnBigMap_Open_BigMap));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_CloseBigMap, new CUIEventManager.OnUIEventHandler(this.OnCloseBigMap));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BigMap_Click_5_Dalong, new CUIEventManager.OnUIEventHandler(this.OnBigMap_Click_5_Dalong));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BigMap_Click_5_Xiaolong, new CUIEventManager.OnUIEventHandler(this.OnBigMap_Click_5_Xiaolong));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BigMap_Click_3_long, new CUIEventManager.OnUIEventHandler(this.OnBigMap_Click_3_long));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BigMap_Click_Organ, new CUIEventManager.OnUIEventHandler(this.OnBigMap_Click_Organ));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BigMap_Click_Hero, new CUIEventManager.OnUIEventHandler(this.OnBigMap_Click_Hero));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BigMap_Click_Map, new CUIEventManager.OnUIEventHandler(this.OnBigMap_Click_Map));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BigMap_Click_Eye, new CUIEventManager.OnUIEventHandler(this.OnBigMap_Click_Eye));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Down_MiniMap, new CUIEventManager.OnUIEventHandler(this.OnMiniMap_Click_Down));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Click_MiniMap_Up, new CUIEventManager.OnUIEventHandler(this.OnMiniMap_Click_Up));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Drag_SignalPanel, new CUIEventManager.OnUIEventHandler(this.OnDragMiniMap));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Drag_SignalPanel_End, new CUIEventManager.OnUIEventHandler(this.OnDragMiniMapEnd));
            Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorBeAttack, new RefAction<DefaultGameEventParam>(this.OnBuildingAttacked));
            Singleton<GameEventSys>.instance.AddEventHandler<GameDeadEventParam>(GameEventDef.Event_MonsterGroupDead, new RefAction<GameDeadEventParam>(this.OnMonsterGroupDead));
            Singleton<GameEventSys>.GetInstance().AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnActorDead));
        }

        private void Send_Position_Signal(CUIEvent uiEvent, GameObject img, byte signal_id, ElementType type, bool bBigMap = true)
        {
            SignalPanel panel = (Singleton<CBattleSystem>.GetInstance().FightForm == null) ? null : Singleton<CBattleSystem>.instance.FightForm.GetSignalPanel();
            if (panel != null)
            {
                MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
                if (theMinimapSys != null)
                {
                    Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
                    if (hostPlayer != null)
                    {
                        ActorRoot root = ((hostPlayer == null) || (hostPlayer.Captain == 0)) ? null : hostPlayer.Captain.handle;
                        if (root != null)
                        {
                            Vector2 vector = !bBigMap ? theMinimapSys.GetMMFianlScreenPos() : theMinimapSys.GetBMFianlScreenPos();
                            float num = uiEvent.m_pointerEventData.get_position().x - vector.x;
                            float num2 = uiEvent.m_pointerEventData.get_position().y - vector.y;
                            num = uiEvent.m_srcFormScript.ChangeScreenValueToForm(num);
                            num2 = uiEvent.m_srcFormScript.ChangeScreenValueToForm(num2);
                            VInt3 zero = VInt3.zero;
                            zero.x = (int) (num * Singleton<CBattleSystem>.GetInstance().UI_world_Factor_Big.x);
                            zero.y = (root == null) ? ((int) 0.15f) : ((int) ((Vector3) root.location).y);
                            zero.z = (int) (num2 * Singleton<CBattleSystem>.GetInstance().UI_world_Factor_Big.y);
                            panel.SendCommand_SignalMiniMap_Position(signal_id, zero, type);
                        }
                    }
                }
            }
        }

        private void send_signal(CUIEvent uiEvent, GameObject img, ElementType elementType = 0, int signal_id = 0)
        {
            if ((uiEvent != null) && (img != null))
            {
                byte num = (byte) uiEvent.m_eventParams.tag2;
                uint tagUInt = uiEvent.m_eventParams.tagUInt;
                if (signal_id == 0)
                {
                    signal_id = uiEvent.m_eventParams.tag3;
                }
                EMapType curMapType = this.curMapType;
                SkillSlotType curSelectedSlotType = this.m_CurSelectedSlotType;
                if (curMapType != EMapType.Skill)
                {
                    this.Switch(EMapType.Mini);
                }
                SignalPanel panel = (Singleton<CBattleSystem>.GetInstance().FightForm == null) ? null : Singleton<CBattleSystem>.instance.FightForm.GetSignalPanel();
                if (panel != null)
                {
                    if (curMapType == EMapType.Skill)
                    {
                        Singleton<CBattleSystem>.GetInstance().FightForm.m_skillButtonManager.SelectedMapTarget(curSelectedSlotType, tagUInt);
                        if (tagUInt != 0)
                        {
                            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
                            if ((hostPlayer != null) && (hostPlayer.Captain != 0))
                            {
                                SkillSlot skillSlot = hostPlayer.Captain.handle.SkillControl.GetSkillSlot(curSelectedSlotType);
                                if (skillSlot != null)
                                {
                                    PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(tagUInt);
                                    if ((actor != 0) && skillSlot.IsValidSkillTarget(ref actor))
                                    {
                                        this.m_CurSelectedSlotType = SkillSlotType.SLOT_SKILL_COUNT;
                                        this.Switch(EMapType.Mini);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        switch (num)
                        {
                            case 3:
                            case 1:
                            case 2:
                                panel.SendCommand_SignalMiniMap_Target((byte) signal_id, num, tagUInt);
                                return;
                        }
                        if (((elementType == ElementType.Dragon_3) || (elementType == ElementType.Dragon_5_big)) || (elementType == ElementType.Dragon_5_small))
                        {
                            this.Send_Position_Signal(uiEvent, img, 2, elementType, true);
                        }
                        else
                        {
                            this.Send_Position_Signal(uiEvent, img, (byte) signal_id, ElementType.None, true);
                        }
                    }
                }
            }
        }

        public void SetSkillBtnActive(bool active)
        {
            if (!Singleton<WatchController>.instance.IsWatching)
            {
                if (Singleton<CBattleSystem>.instance.FightForm != null)
                {
                    CSkillButtonManager skillButtonManager = Singleton<CBattleSystem>.instance.FightForm.m_skillButtonManager;
                    if (skillButtonManager != null)
                    {
                        skillButtonManager.SetSkillButtuonActive(SkillSlotType.SLOT_SKILL_2, active);
                        skillButtonManager.SetSkillButtuonActive(SkillSlotType.SLOT_SKILL_3, active);
                        if (active)
                        {
                            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
                            if (((hostPlayer != null) && (hostPlayer.Captain != 0)) && hostPlayer.Captain.handle.EquipComponent.GetEquipActiveSkillShowFlag(SkillSlotType.SLOT_SKILL_9))
                            {
                                skillButtonManager.SetSkillButtuonActive(SkillSlotType.SLOT_SKILL_9, true);
                            }
                        }
                        else
                        {
                            skillButtonManager.SetSkillButtuonActive(SkillSlotType.SLOT_SKILL_9, false);
                        }
                    }
                }
                this.SetSkillMapDragonEventActive(active);
            }
        }

        public void SetSkillMapDragonEventActive(bool bIsActive)
        {
            if (!Singleton<WatchController>.instance.IsWatching && (Singleton<CBattleSystem>.instance.FightForm != null))
            {
                GameObject bigMapDragonContainer = Singleton<CBattleSystem>.instance.FightForm.GetBigMapDragonContainer();
                if ((bigMapDragonContainer != null) && (bigMapDragonContainer.get_transform() != null))
                {
                    int num = bigMapDragonContainer.get_transform().get_childCount();
                    for (int i = 0; i < num; i++)
                    {
                        Transform child = bigMapDragonContainer.get_transform().GetChild(i);
                        if (child != null)
                        {
                            CUIEventScript component = child.GetComponent<CUIEventScript>();
                            if (component != null)
                            {
                                component.set_enabled(bIsActive);
                            }
                        }
                    }
                }
            }
        }

        public static Image SetTower_Image(bool bAlie, int value, GameObject mapPointer, CUIFormScript formScript)
        {
            if ((mapPointer == null) || (formScript == null))
            {
                return null;
            }
            Image component = mapPointer.GetComponent<Image>();
            Image image = mapPointer.get_transform().Find("front").GetComponent<Image>();
            if ((component == null) || (image == null))
            {
                return null;
            }
            if (value == 2)
            {
                component.SetSprite(!bAlie ? enemy_base : self_base, formScript, true, false, false, false);
                image.SetSprite(!bAlie ? enemy_base_outline : self_base_outline, formScript, true, false, false, false);
                return component;
            }
            if ((value == 1) || (value == 4))
            {
                component.SetSprite(!bAlie ? enemy_tower : self_tower, formScript, true, false, false, false);
                image.SetSprite(!bAlie ? enemy_tower_outline : self_tower_outline, formScript, true, false, false, false);
            }
            return component;
        }

        public void Switch(EMapType type)
        {
            this.curMapType = type;
            if (this._ownerForm != null)
            {
                GameObject widget = this._ownerForm.GetWidget(0x2d);
                if (this.curMapType == EMapType.Mini)
                {
                    CUICommonSystem.SetObjActive(this.mmRoot, true);
                    CUICommonSystem.SetObjActive(this.mmUGUIRoot, true);
                    CUICommonSystem.SetObjActive(this.bmRoot, false);
                    CUICommonSystem.SetObjActive(this.bmUGUIRoot, false);
                    widget.CustomSetActive(true);
                    SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
                    if ((curLvelContext != null) && curLvelContext.m_isShowTrainingHelper)
                    {
                        CTrainingHelper instance = Singleton<CTrainingHelper>.GetInstance();
                        instance.SetButtonActive(!instance.IsPanelActive());
                    }
                    this.SetSkillBtnActive(true);
                }
                else if (this.curMapType == EMapType.Big)
                {
                    CUICommonSystem.SetObjActive(this.mmRoot, false);
                    CUICommonSystem.SetObjActive(this.mmUGUIRoot, false);
                    CUICommonSystem.SetObjActive(this.bmRoot, true);
                    CUICommonSystem.SetObjActive(this.bmUGUIRoot, true);
                    widget.CustomSetActive(false);
                    SLevelContext context2 = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
                    if ((context2 != null) && context2.m_isShowTrainingHelper)
                    {
                        CTrainingHelper helper2 = Singleton<CTrainingHelper>.GetInstance();
                        if (helper2.IsOpenBtnActive())
                        {
                            helper2.SetButtonActive(false);
                        }
                    }
                    this.ChangeBigMapSide(true);
                    this.ChangeMapPointerDepth(this.curMapType);
                    this.SetSkillBtnActive(true);
                }
                else if (this.curMapType == EMapType.Skill)
                {
                    CUICommonSystem.SetObjActive(this.mmRoot, false);
                    CUICommonSystem.SetObjActive(this.mmUGUIRoot, false);
                    CUICommonSystem.SetObjActive(this.bmRoot, true);
                    CUICommonSystem.SetObjActive(this.bmUGUIRoot, true);
                    widget.CustomSetActive(false);
                    this.ChangeBigMapSide(false);
                    this.ChangeMapPointerDepth(this.curMapType);
                    this.SetSkillBtnActive(false);
                }
                else
                {
                    this.mmRoot.CustomSetActive(false);
                    CUICommonSystem.SetObjActive(this.mmUGUIRoot, false);
                    this.bmRoot.CustomSetActive(false);
                    CUICommonSystem.SetObjActive(this.bmUGUIRoot, false);
                    this.SetSkillBtnActive(true);
                }
                this.m_CurSelectedSlotType = SkillSlotType.SLOT_SKILL_COUNT;
                if (this.MMinimapSkillIndicator_3Dui != null)
                {
                    this.MMinimapSkillIndicator_3Dui.ForceUpdate();
                }
            }
        }

        public void Switch(EMapType type, SkillSlotType slotType)
        {
            this.Switch(type);
            this.m_CurSelectedSlotType = slotType;
        }

        private void unRegEvent()
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BigMap_Open_BigMap, new CUIEventManager.OnUIEventHandler(this.OnBigMap_Open_BigMap));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_CloseBigMap, new CUIEventManager.OnUIEventHandler(this.OnCloseBigMap));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BigMap_Click_5_Dalong, new CUIEventManager.OnUIEventHandler(this.OnBigMap_Click_5_Dalong));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BigMap_Click_5_Xiaolong, new CUIEventManager.OnUIEventHandler(this.OnBigMap_Click_5_Xiaolong));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BigMap_Click_3_long, new CUIEventManager.OnUIEventHandler(this.OnBigMap_Click_3_long));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BigMap_Click_Organ, new CUIEventManager.OnUIEventHandler(this.OnBigMap_Click_Organ));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BigMap_Click_Hero, new CUIEventManager.OnUIEventHandler(this.OnBigMap_Click_Hero));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BigMap_Click_Map, new CUIEventManager.OnUIEventHandler(this.OnBigMap_Click_Map));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BigMap_Click_Eye, new CUIEventManager.OnUIEventHandler(this.OnBigMap_Click_Eye));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Down_MiniMap, new CUIEventManager.OnUIEventHandler(this.OnMiniMap_Click_Down));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Click_MiniMap_Up, new CUIEventManager.OnUIEventHandler(this.OnMiniMap_Click_Up));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Drag_SignalPanel, new CUIEventManager.OnUIEventHandler(this.OnDragMiniMap));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Drag_SignalPanel_End, new CUIEventManager.OnUIEventHandler(this.OnDragMiniMapEnd));
            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorBeAttack, new RefAction<DefaultGameEventParam>(this.OnBuildingAttacked));
            Singleton<GameEventSys>.instance.RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_MonsterGroupDead, new RefAction<GameDeadEventParam>(this.OnMonsterGroupDead));
            Singleton<GameEventSys>.GetInstance().RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnActorDead));
        }

        public void Update()
        {
            if (this.MMiniMapCameraFrame_3Dui != null)
            {
                this.MMiniMapCameraFrame_3Dui.Update();
            }
            if (this.MMinimapSkillIndicator_3Dui != null)
            {
                this.MMinimapSkillIndicator_3Dui.Update();
            }
            if (FogOfWar.enable && !Singleton<WatchController>.instance.IsWatching)
            {
                if ((this.m_elementIndex >= 0) && (this.m_elementIndex < this.m_mapElements.Count))
                {
                    ElementInMap map = this.m_mapElements[this.m_elementIndex];
                    VInt3 worldLoc = new VInt3(map.pos.x, map.pos.z, 0);
                    if (Singleton<GameFowManager>.instance.IsSurfaceCellVisible(worldLoc, this.m_playerCamp))
                    {
                        if (map.smallElement != null)
                        {
                            MiniMapSysUT.RecycleMapGameObject(map.smallElement);
                            map.smallElement = null;
                        }
                        if (map.bigElement != null)
                        {
                            MiniMapSysUT.RecycleMapGameObject(map.bigElement);
                            map.bigElement = null;
                        }
                        this.m_mapElements.RemoveAt(this.m_elementIndex);
                    }
                    this.m_elementIndex++;
                }
                else
                {
                    this.m_elementIndex = 0;
                }
            }
        }

        public GameObject bmBGMap_3dui
        {
            [CompilerGenerated]
            get
            {
                return this.<bmBGMap_3dui>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<bmBGMap_3dui>k__BackingField = value;
            }
        }

        public Vector2 bmFinalScreenSize
        {
            [CompilerGenerated]
            get
            {
                return this.<bmFinalScreenSize>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<bmFinalScreenSize>k__BackingField = value;
            }
        }

        public GameObject bmHeroIconNode
        {
            [CompilerGenerated]
            get
            {
                return this.<bmHeroIconNode>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<bmHeroIconNode>k__BackingField = value;
            }
        }

        public GameObject bmHeroIconNode_Enemy
        {
            [CompilerGenerated]
            get
            {
                return this.<bmHeroIconNode_Enemy>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<bmHeroIconNode_Enemy>k__BackingField = value;
            }
        }

        public GameObject bmHeroIconNode_Friend
        {
            [CompilerGenerated]
            get
            {
                return this.<bmHeroIconNode_Friend>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<bmHeroIconNode_Friend>k__BackingField = value;
            }
        }

        public GameObject bmHeroIconNode_Self
        {
            [CompilerGenerated]
            get
            {
                return this.<bmHeroIconNode_Self>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<bmHeroIconNode_Self>k__BackingField = value;
            }
        }

        public GameObject bmpcAlies
        {
            [CompilerGenerated]
            get
            {
                return this.<bmpcAlies>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<bmpcAlies>k__BackingField = value;
            }
        }

        public GameObject bmpcBlueBuff
        {
            [CompilerGenerated]
            get
            {
                return this.<bmpcBlueBuff>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<bmpcBlueBuff>k__BackingField = value;
            }
        }

        public GameObject bmpcDragon_3dui
        {
            [CompilerGenerated]
            get
            {
                return this.<bmpcDragon_3dui>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<bmpcDragon_3dui>k__BackingField = value;
            }
        }

        public GameObject bmpcDragon_ugui
        {
            [CompilerGenerated]
            get
            {
                return this.<bmpcDragon_ugui>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<bmpcDragon_ugui>k__BackingField = value;
            }
        }

        public GameObject bmpcEffect
        {
            [CompilerGenerated]
            get
            {
                return this.<bmpcEffect>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<bmpcEffect>k__BackingField = value;
            }
        }

        public GameObject bmpcEnemy
        {
            [CompilerGenerated]
            get
            {
                return this.<bmpcEnemy>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<bmpcEnemy>k__BackingField = value;
            }
        }

        public GameObject bmpcEye
        {
            [CompilerGenerated]
            get
            {
                return this.<bmpcEye>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<bmpcEye>k__BackingField = value;
            }
        }

        public GameObject bmpcHero
        {
            [CompilerGenerated]
            get
            {
                return this.<bmpcHero>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<bmpcHero>k__BackingField = value;
            }
        }

        public GameObject bmpcJungle
        {
            [CompilerGenerated]
            get
            {
                return this.<bmpcJungle>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<bmpcJungle>k__BackingField = value;
            }
        }

        public GameObject bmpcOrgan
        {
            [CompilerGenerated]
            get
            {
                return this.<bmpcOrgan>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<bmpcOrgan>k__BackingField = value;
            }
        }

        public GameObject bmpcRedBuff
        {
            [CompilerGenerated]
            get
            {
                return this.<bmpcRedBuff>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<bmpcRedBuff>k__BackingField = value;
            }
        }

        public GameObject bmpcSignal
        {
            [CompilerGenerated]
            get
            {
                return this.<bmpcSignal>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<bmpcSignal>k__BackingField = value;
            }
        }

        public GameObject bmpcTrack
        {
            [CompilerGenerated]
            get
            {
                return this.<bmpcTrack>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<bmpcTrack>k__BackingField = value;
            }
        }

        public GameObject bmRoot
        {
            [CompilerGenerated]
            get
            {
                return this.<bmRoot>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<bmRoot>k__BackingField = value;
            }
        }

        public GameObject bmUGUIRoot
        {
            [CompilerGenerated]
            get
            {
                return this.<bmUGUIRoot>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<bmUGUIRoot>k__BackingField = value;
            }
        }

        public GameObject mmBGMap_3dui
        {
            [CompilerGenerated]
            get
            {
                return this.<mmBGMap_3dui>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<mmBGMap_3dui>k__BackingField = value;
            }
        }

        public Vector2 mmFinalScreenSize
        {
            [CompilerGenerated]
            get
            {
                return this.<mmFinalScreenSize>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<mmFinalScreenSize>k__BackingField = value;
            }
        }

        public GameObject mmHeroIconNode
        {
            [CompilerGenerated]
            get
            {
                return this.<mmHeroIconNode>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<mmHeroIconNode>k__BackingField = value;
            }
        }

        public GameObject mmHeroIconNode_Enemy
        {
            [CompilerGenerated]
            get
            {
                return this.<mmHeroIconNode_Enemy>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<mmHeroIconNode_Enemy>k__BackingField = value;
            }
        }

        public GameObject mmHeroIconNode_Friend
        {
            [CompilerGenerated]
            get
            {
                return this.<mmHeroIconNode_Friend>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<mmHeroIconNode_Friend>k__BackingField = value;
            }
        }

        public GameObject mmHeroIconNode_Self
        {
            [CompilerGenerated]
            get
            {
                return this.<mmHeroIconNode_Self>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<mmHeroIconNode_Self>k__BackingField = value;
            }
        }

        public GameObject mmpcAlies
        {
            [CompilerGenerated]
            get
            {
                return this.<mmpcAlies>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<mmpcAlies>k__BackingField = value;
            }
        }

        public GameObject mmpcBlueBuff
        {
            [CompilerGenerated]
            get
            {
                return this.<mmpcBlueBuff>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<mmpcBlueBuff>k__BackingField = value;
            }
        }

        public GameObject mmpcDragon_3dui
        {
            [CompilerGenerated]
            get
            {
                return this.<mmpcDragon_3dui>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<mmpcDragon_3dui>k__BackingField = value;
            }
        }

        public GameObject mmpcDragon_ugui
        {
            [CompilerGenerated]
            get
            {
                return this.<mmpcDragon_ugui>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<mmpcDragon_ugui>k__BackingField = value;
            }
        }

        public GameObject mmpcEffect
        {
            [CompilerGenerated]
            get
            {
                return this.<mmpcEffect>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<mmpcEffect>k__BackingField = value;
            }
        }

        public GameObject mmpcEnemy
        {
            [CompilerGenerated]
            get
            {
                return this.<mmpcEnemy>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<mmpcEnemy>k__BackingField = value;
            }
        }

        public GameObject mmpcEye
        {
            [CompilerGenerated]
            get
            {
                return this.<mmpcEye>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<mmpcEye>k__BackingField = value;
            }
        }

        public GameObject mmpcHero
        {
            [CompilerGenerated]
            get
            {
                return this.<mmpcHero>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<mmpcHero>k__BackingField = value;
            }
        }

        public GameObject mmpcJungle
        {
            [CompilerGenerated]
            get
            {
                return this.<mmpcJungle>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<mmpcJungle>k__BackingField = value;
            }
        }

        public GameObject mmpcOrgan
        {
            [CompilerGenerated]
            get
            {
                return this.<mmpcOrgan>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<mmpcOrgan>k__BackingField = value;
            }
        }

        public GameObject mmpcRedBuff
        {
            [CompilerGenerated]
            get
            {
                return this.<mmpcRedBuff>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<mmpcRedBuff>k__BackingField = value;
            }
        }

        public GameObject mmpcSignal
        {
            [CompilerGenerated]
            get
            {
                return this.<mmpcSignal>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<mmpcSignal>k__BackingField = value;
            }
        }

        public GameObject mmpcTrack
        {
            [CompilerGenerated]
            get
            {
                return this.<mmpcTrack>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<mmpcTrack>k__BackingField = value;
            }
        }

        public GameObject mmRoot
        {
            [CompilerGenerated]
            get
            {
                return this.<mmRoot>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<mmRoot>k__BackingField = value;
            }
        }

        public GameObject mmUGUIRoot
        {
            [CompilerGenerated]
            get
            {
                return this.<mmUGUIRoot>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<mmUGUIRoot>k__BackingField = value;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BornRecord
        {
            public VInt3 pos;
            public uint cfgId;
        }

        public class ElementInMap
        {
            public GameObject bigElement;
            public VInt3 pos;
            public GameObject smallElement;

            public ElementInMap(VInt3 pos, GameObject smallElement, GameObject bigElement)
            {
                this.pos = pos;
                this.smallElement = smallElement;
                this.bigElement = bigElement;
            }
        }

        public enum ElementType
        {
            None,
            Tower,
            Base,
            Hero,
            Dragon_5_big,
            Dragon_5_small,
            Dragon_3,
            Eye
        }

        public enum EMapType
        {
            None,
            Mini,
            Big,
            Skill
        }
    }
}

