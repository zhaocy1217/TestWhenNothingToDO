namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.UI;
    using ResData;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class MiniMapSysUT
    {
        public const int ATK = 2;
        public const int DEF = 3;
        public static string Map_BaseAlice_prefab = "UI3D/Battle/MiniMap/Map_BaseAlice.prefab";
        public static string Map_BaseEnemy_prefab = "UI3D/Battle/MiniMap/Map_BaseEnemy.prefab";
        public static string Map_BlueBuff_prefab = "UI3D/Battle/MiniMap/Map_BlueBuff.prefab";
        public static string Map_EyeAlice_prefab = "UI3D/Battle/MiniMap/Map_EyeAlice.prefab";
        public static string Map_EyeEnemy_prefab = "UI3D/Battle/MiniMap/Map_EyeEnemy.prefab";
        public static string Map_HeroAlice_prefab = "UI3D/Battle/MiniMap/Map_HeroAlice.prefab";
        public static string Map_HeroEnemy_prefab = "UI3D/Battle/MiniMap/Map_HeroEnemy.prefab";
        public static string Map_HeroSelf_prefab = "UI3D/Battle/MiniMap/Map_HeroSelf.prefab";
        public static string Map_Jungle_prefab = "UI3D/Battle/MiniMap/Map_Jungle.prefab";
        public static string Map_OrganAlice_prefab = "UI3D/Battle/MiniMap/Map_OrganAlice.prefab";
        public static string Map_OrganEnemy_prefab = "UI3D/Battle/MiniMap/Map_OrganEnemy.prefab";
        public static string Map_RedBuff_prefab = "UI3D/Battle/MiniMap/Map_RedBuff.prefab";
        public static string Map_Signal_prefab = "UI3D/Battle/MiniMap/Map_Signal.prefab";
        public static string Map_SoilderAlice_prefab = "UI3D/Battle/MiniMap/Map_SoilderAlice.prefab";
        public static string Map_SoilderEnemy_prefab = "UI3D/Battle/MiniMap/Map_SoilderEnemy.prefab";
        public static string Map_Track_prefab = "UI3D/Battle/MiniMap/Map_Track.prefab";
        public const int PROTECT = 5;
        public static int UI3D_Depth = 30;

        public static Vector2 Get3DCameraGameObject_ScreenPos(GameObject obj)
        {
            Camera currentCamera = Singleton<Camera_UI3D>.GetInstance().GetCurrentCamera();
            if ((currentCamera != null) && (obj != null))
            {
                return currentCamera.WorldToScreenPoint(obj.get_transform().get_position());
            }
            return Vector2.get_zero();
        }

        public static enUIEventID GetEventId(MinimapSys.ElementType type)
        {
            if (type == MinimapSys.ElementType.Dragon_3)
            {
                return enUIEventID.BigMap_Click_3_long;
            }
            if (type == MinimapSys.ElementType.Dragon_5_big)
            {
                return enUIEventID.BigMap_Click_5_Dalong;
            }
            if (type == MinimapSys.ElementType.Dragon_5_small)
            {
                return enUIEventID.BigMap_Click_5_Xiaolong;
            }
            if (type == MinimapSys.ElementType.Tower)
            {
                return enUIEventID.BigMap_Click_Organ;
            }
            if (type == MinimapSys.ElementType.Base)
            {
                return enUIEventID.BigMap_Click_Organ;
            }
            if (type == MinimapSys.ElementType.Hero)
            {
                return enUIEventID.BigMap_Click_Hero;
            }
            if (type == MinimapSys.ElementType.Eye)
            {
                return enUIEventID.BigMap_Click_Eye;
            }
            return enUIEventID.None;
        }

        public static GameObject GetHeroIconObj(string path, bool bMiniMap, bool bSelf, bool bSameCamp)
        {
            MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
            if (theMinimapSys == null)
            {
                return null;
            }
            GameObject obj2 = null;
            if (bMiniMap)
            {
                if (bSelf)
                {
                    obj2 = theMinimapSys.mmHeroIconNode_Self;
                }
                else
                {
                    obj2 = !bSameCamp ? theMinimapSys.mmHeroIconNode_Enemy : theMinimapSys.mmHeroIconNode_Friend;
                }
            }
            else if (bSelf)
            {
                obj2 = theMinimapSys.bmHeroIconNode_Self;
            }
            else
            {
                obj2 = !bSameCamp ? theMinimapSys.bmHeroIconNode_Enemy : theMinimapSys.bmHeroIconNode_Friend;
            }
            GameObject gameObject = Singleton<CGameObjectPool>.GetInstance().GetGameObject(path, enResourceType.UI3DImage);
            if (gameObject != null)
            {
                if (obj2 != null)
                {
                    gameObject.get_transform().SetParent(obj2.get_transform(), true);
                }
                gameObject.get_transform().set_localScale(Vector3.get_one());
                gameObject.get_transform().set_localRotation(Quaternion.get_identity());
                float screenWidth = !bMiniMap ? (40f * Sprite3D.Ratio()) : (20f * Sprite3D.Ratio());
                NativeSizeLize(gameObject, screenWidth, screenWidth);
                Singleton<Camera_UI3D>.instance.GetCurrentCanvas().RefreshLayout(null);
            }
            return gameObject;
        }

        public static GameObject GetMapGameObject(ActorRoot actor, bool bMiniMap, out UI3DEventCom evtCom)
        {
            evtCom = null;
            float num = 1f;
            GameObject obj2 = null;
            string str = string.Empty;
            bool bAlien = actor.IsHostCamp();
            MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
            if (theMinimapSys == null)
            {
                return null;
            }
            GameObject gameObject = null;
            if (actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
            {
                str = !bAlien ? Map_HeroEnemy_prefab : Map_HeroAlice_prefab;
                if (actor.TheActorMeta.PlayerId == Singleton<GamePlayerCenter>.GetInstance().HostPlayerId)
                {
                    str = Map_HeroSelf_prefab;
                }
                obj2 = !bMiniMap ? theMinimapSys.bmpcHero : theMinimapSys.mmpcHero;
                if (!bMiniMap)
                {
                    if (evtCom == null)
                    {
                        evtCom = new UI3DEventCom();
                    }
                    SetEventComScreenSize(evtCom, 40f, 40f, 1f);
                    SetMapElement_EventParam(evtCom, actor.IsHostCamp(), MinimapSys.ElementType.Hero, actor.ObjID, (uint) actor.TheActorMeta.ConfigId);
                    if (evtCom != null)
                    {
                        theMinimapSys.UI3DEventMgr.Register(evtCom, UI3DEventMgr.EventComType.Hero);
                    }
                }
            }
            else if (actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_EYE)
            {
                str = !bAlien ? Map_EyeEnemy_prefab : Map_EyeAlice_prefab;
                obj2 = !bMiniMap ? theMinimapSys.bmpcEye : theMinimapSys.mmpcEye;
                num = !bMiniMap ? 1f : 0.5f;
                if (!bMiniMap)
                {
                    if (evtCom == null)
                    {
                        evtCom = new UI3DEventCom();
                    }
                    SetEventComScreenSize(evtCom, 30f, 18f, 1f);
                    SetMapElement_EventParam(evtCom, actor.IsHostCamp(), MinimapSys.ElementType.Eye, actor.ObjID, (uint) actor.TheActorMeta.ConfigId);
                    if (evtCom != null)
                    {
                        theMinimapSys.UI3DEventMgr.Register(evtCom, UI3DEventMgr.EventComType.Eye);
                    }
                }
            }
            else if (actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster)
            {
                if (actor.ActorControl.GetActorSubType() == 2)
                {
                    switch (actor.ActorControl.GetActorSubSoliderType())
                    {
                        case 8:
                        case 9:
                        case 13:
                        case 7:
                            return null;

                        case 11:
                            str = Map_RedBuff_prefab;
                            obj2 = !bMiniMap ? theMinimapSys.bmpcRedBuff : theMinimapSys.mmpcRedBuff;
                            num = !bMiniMap ? 1f : 0.6f;
                            goto Label_051E;

                        case 10:
                            str = Map_BlueBuff_prefab;
                            obj2 = !bMiniMap ? theMinimapSys.bmpcBlueBuff : theMinimapSys.mmpcBlueBuff;
                            num = !bMiniMap ? 1f : 0.6f;
                            goto Label_051E;
                    }
                    str = Map_Jungle_prefab;
                    obj2 = !bMiniMap ? theMinimapSys.bmpcJungle : theMinimapSys.mmpcJungle;
                    num = !bMiniMap ? 1.3f : 1f;
                }
                else
                {
                    if (bMiniMap)
                    {
                        obj2 = !bAlien ? theMinimapSys.mmpcEnemy : theMinimapSys.mmpcAlies;
                    }
                    else
                    {
                        obj2 = !bAlien ? theMinimapSys.bmpcEnemy : theMinimapSys.bmpcAlies;
                    }
                    str = !bAlien ? Map_SoilderEnemy_prefab : Map_SoilderAlice_prefab;
                    num = !bMiniMap ? 0.6f : 0.3f;
                }
            }
            else if (actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ)
            {
                if ((actor.TheStaticData.TheOrganOnlyInfo.OrganType == 1) || (actor.TheStaticData.TheOrganOnlyInfo.OrganType == 4))
                {
                    str = !bAlien ? Map_OrganEnemy_prefab : Map_OrganAlice_prefab;
                    obj2 = !bMiniMap ? theMinimapSys.bmpcOrgan : theMinimapSys.mmpcOrgan;
                    num = !bMiniMap ? 1f : 0.5f;
                    if (!bMiniMap)
                    {
                        if (evtCom == null)
                        {
                            evtCom = new UI3DEventCom();
                        }
                        SetEventComScreenSize(evtCom, 30f, 32f, 1f);
                        SetMapElement_EventParam(evtCom, bAlien, MinimapSys.ElementType.Tower, actor.ObjID, 0);
                        if (evtCom != null)
                        {
                            theMinimapSys.UI3DEventMgr.Register(evtCom, UI3DEventMgr.EventComType.Tower);
                        }
                        if (bAlien)
                        {
                            Singleton<CBattleSystem>.GetInstance().TowerHitMgr.Register(actor.ObjID, (RES_ORGAN_TYPE) actor.TheStaticData.TheOrganOnlyInfo.OrganType);
                        }
                    }
                }
                else
                {
                    if (actor.TheStaticData.TheOrganOnlyInfo.OrganType != 2)
                    {
                        return null;
                    }
                    str = !bAlien ? Map_BaseEnemy_prefab : Map_BaseAlice_prefab;
                    obj2 = !bMiniMap ? theMinimapSys.bmpcOrgan : theMinimapSys.mmpcOrgan;
                    num = !bMiniMap ? 1f : 0.5f;
                    if (!bMiniMap)
                    {
                        if (evtCom == null)
                        {
                            evtCom = new UI3DEventCom();
                        }
                        SetEventComScreenSize(evtCom, 30f, 32f, 1f);
                        SetMapElement_EventParam(evtCom, bAlien, MinimapSys.ElementType.Base, actor.ObjID, 0);
                        if (evtCom != null)
                        {
                            theMinimapSys.UI3DEventMgr.Register(evtCom, UI3DEventMgr.EventComType.Tower);
                        }
                        if (bAlien)
                        {
                            Singleton<CBattleSystem>.GetInstance().TowerHitMgr.Register(actor.ObjID, (RES_ORGAN_TYPE) actor.TheStaticData.TheOrganOnlyInfo.OrganType);
                        }
                    }
                }
            }
        Label_051E:
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            gameObject = Singleton<CGameObjectPool>.GetInstance().GetGameObject(str, enResourceType.BattleScene);
            if (obj2 == null)
            {
                return null;
            }
            if (gameObject != null)
            {
                gameObject.get_transform().SetParent(obj2.get_transform(), true);
                gameObject.get_transform().set_localScale(new Vector3(num, num, 1f));
                gameObject.get_transform().set_localRotation(Quaternion.get_identity());
                NativeSizeLize(gameObject);
            }
            if (actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
            {
                float screenWidth = !bMiniMap ? (65f * Sprite3D.Ratio()) : (34f * Sprite3D.Ratio());
                NativeSizeLize(gameObject, screenWidth, screenWidth);
            }
            return gameObject;
        }

        public static GameObject GetMapTrackObj(string prefabPath, bool bMiniMap)
        {
            MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
            if (theMinimapSys == null)
            {
                return null;
            }
            GameObject obj2 = !bMiniMap ? theMinimapSys.bmpcTrack : theMinimapSys.mmpcTrack;
            GameObject gameObject = Singleton<CGameObjectPool>.GetInstance().GetGameObject(prefabPath, enResourceType.BattleScene);
            if (gameObject != null)
            {
                if (obj2 != null)
                {
                    gameObject.get_transform().SetParent(obj2.get_transform(), true);
                }
                float num = !bMiniMap ? 1f : 0.7f;
                gameObject.get_transform().set_localScale(new Vector3(num, num, 1f));
                gameObject.get_transform().set_localRotation(Quaternion.get_identity());
                NativeSizeLize(gameObject);
            }
            return gameObject;
        }

        public static GameObject GetSignalGameObject(bool bMiniMap)
        {
            MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
            if (theMinimapSys == null)
            {
                return null;
            }
            GameObject obj2 = !bMiniMap ? theMinimapSys.bmpcSignal : theMinimapSys.mmpcSignal;
            GameObject gameObject = Singleton<CGameObjectPool>.GetInstance().GetGameObject(Map_Signal_prefab, enResourceType.BattleScene);
            if (gameObject != null)
            {
                if (obj2 != null)
                {
                    gameObject.get_transform().SetParent(obj2.get_transform(), true);
                }
                float num = !bMiniMap ? 1f : 0.5f;
                gameObject.get_transform().set_localScale(new Vector3(num, num, 1f));
                gameObject.get_transform().set_localRotation(Quaternion.get_identity());
                NativeSizeLize(gameObject);
            }
            return gameObject;
        }

        public static void NativeSizeLize(GameObject minimap3DUI)
        {
            Camera currentCamera = Singleton<Camera_UI3D>.GetInstance().GetCurrentCamera();
            if (currentCamera != null)
            {
                foreach (Sprite3D sprited in minimap3DUI.GetComponentsInChildren<Sprite3D>(true))
                {
                    if (sprited != null)
                    {
                        sprited.SetNativeSize(currentCamera, (float) UI3D_Depth);
                    }
                }
            }
        }

        public static void NativeSizeLize(GameObject minimap3DUI, float screenWidth, float screenHeight)
        {
            Camera currentCamera = Singleton<Camera_UI3D>.GetInstance().GetCurrentCamera();
            if (currentCamera != null)
            {
                foreach (Sprite3D sprited in minimap3DUI.GetComponentsInChildren<Sprite3D>(true))
                {
                    if (sprited != null)
                    {
                        sprited.SetNativeSize(currentCamera, (float) UI3D_Depth, screenWidth, screenHeight);
                    }
                }
            }
        }

        public static void RecycleMapGameObject(GameObject element)
        {
            Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(element);
        }

        public static void RecycleMapGameObject(Transform ts)
        {
            if (ts != null)
            {
                Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(ts.get_gameObject());
            }
        }

        public static void RefreshMapPointerBig(GameObject go)
        {
            CUIFormScript formScript = Singleton<CBattleSystem>.GetInstance().FormScript;
            if (((formScript != null) && (formScript.m_sgameGraphicRaycaster != null)) && (go != null))
            {
                formScript.m_sgameGraphicRaycaster.RefreshGameObject(go);
            }
        }

        public static Vector3 Set3DUIWorldPos_ByScreenPoint(ref Vector3 worldPos, bool bMiniMap)
        {
            float num;
            float num2;
            return Set3DUIWorldPos_ByScreenPoint(ref worldPos, bMiniMap, out num, out num2);
        }

        public static Vector3 Set3DUIWorldPos_ByScreenPoint(ref Vector3 worldPos, bool bMiniMap, out float finalScreenX, out float finalScreenY)
        {
            finalScreenX = finalScreenY = 0f;
            MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
            if (theMinimapSys == null)
            {
                return Vector3.get_zero();
            }
            Vector2 vector = !bMiniMap ? Singleton<CBattleSystem>.instance.world_UI_Factor_Big : Singleton<CBattleSystem>.instance.world_UI_Factor_Small;
            Vector2 vector2 = new Vector2(worldPos.x * vector.x, worldPos.z * vector.y);
            vector2.x *= Sprite3D.Ratio();
            vector2.y *= Sprite3D.Ratio();
            Vector2 vector3 = !bMiniMap ? theMinimapSys.GetBMFianlScreenPos() : theMinimapSys.GetMMFianlScreenPos();
            vector2.x += vector3.x;
            vector2.y += vector3.y;
            Vector3 vector4 = new Vector3(vector2.x, vector2.y, (float) UI3D_Depth);
            Camera currentCamera = Singleton<Camera_UI3D>.GetInstance().GetCurrentCamera();
            if (currentCamera == null)
            {
                return Vector3.get_zero();
            }
            finalScreenX = vector4.x;
            finalScreenY = vector4.y;
            return currentCamera.ScreenToWorldPoint(vector4);
        }

        public static void Set3DUIWorldPos_ByScreenPosition(GameObject uiObj_3dui, float screenX, float screenY)
        {
            if ((uiObj_3dui != null) && (Singleton<CBattleSystem>.GetInstance().TheMinimapSys != null))
            {
                Vector3 vector = new Vector3(screenX, screenY, (float) UI3D_Depth);
                Camera currentCamera = Singleton<Camera_UI3D>.GetInstance().GetCurrentCamera();
                if (currentCamera != null)
                {
                    uiObj_3dui.get_transform().set_position(currentCamera.ScreenToWorldPoint(vector));
                }
            }
        }

        private static void SetEventComScreenSize(UI3DEventCom evtCom, Sprite3D sptCom, float scale = 1f)
        {
            if (evtCom != null)
            {
                evtCom.m_screenSize.set_width((sptCom.textureWidth * scale) * Sprite3D.Ratio());
                evtCom.m_screenSize.set_height((sptCom.textureHeight * scale) * Sprite3D.Ratio());
            }
        }

        private static void SetEventComScreenSize(UI3DEventCom evtCom, float width, float height, float scale = 1f)
        {
            if (evtCom != null)
            {
                evtCom.m_screenSize.set_width((width * scale) * Sprite3D.Ratio());
                evtCom.m_screenSize.set_height((height * scale) * Sprite3D.Ratio());
            }
        }

        public static void SetMapElement_EventParam(UI3DEventCom evtCom, bool bAlien, MinimapSys.ElementType type, uint objID = 0, uint targetHeroID = 0)
        {
            if ((evtCom != null) && (type != MinimapSys.ElementType.None))
            {
                stUIEventParams params = new stUIEventParams();
                params.tag = !bAlien ? 0 : 1;
                params.tag2 = (int) type;
                params.tagUInt = objID;
                params.heroId = targetHeroID;
                if (((type == MinimapSys.ElementType.Dragon_3) || (type == MinimapSys.ElementType.Dragon_5_big)) || (type == MinimapSys.ElementType.Dragon_5_small))
                {
                    params.tag3 = 2;
                }
                else if ((type == MinimapSys.ElementType.Tower) || (type == MinimapSys.ElementType.Base))
                {
                    params.tag3 = !bAlien ? 2 : 3;
                }
                else if (type == MinimapSys.ElementType.Hero)
                {
                    params.tag3 = !bAlien ? 2 : 5;
                }
                evtCom.m_eventID = GetEventId(type);
                evtCom.m_eventParams = params;
            }
        }

        public static void SetMapElement_EventParam(CUIEventScript evtCom, bool bAlien, MinimapSys.ElementType type, uint objID = 0, uint targetHeroID = 0)
        {
            if ((evtCom != null) && (type != MinimapSys.ElementType.None))
            {
                stUIEventParams params = new stUIEventParams();
                params.tag = !bAlien ? 0 : 1;
                params.tag2 = (int) type;
                params.tagUInt = objID;
                params.heroId = targetHeroID;
                if (((type == MinimapSys.ElementType.Dragon_3) || (type == MinimapSys.ElementType.Dragon_5_big)) || (type == MinimapSys.ElementType.Dragon_5_small))
                {
                    params.tag3 = 2;
                }
                else if ((type == MinimapSys.ElementType.Tower) || (type == MinimapSys.ElementType.Base))
                {
                    params.tag3 = !bAlien ? 2 : 3;
                }
                else if (type == MinimapSys.ElementType.Hero)
                {
                    params.tag3 = !bAlien ? 2 : 5;
                }
                evtCom.m_onClickEventParams = params;
            }
        }

        public static void SetMapElement_EventParam(GameObject obj, bool bAlien, MinimapSys.ElementType type, uint objID = 0, uint targetHeroID = 0)
        {
            if ((type != MinimapSys.ElementType.None) && (obj != null))
            {
                CUIEventScript component = obj.GetComponent<CUIEventScript>();
                if (component != null)
                {
                    SetMapElement_EventParam(component, bAlien, type, objID, targetHeroID);
                }
            }
        }

        public static void UnRegisterEventCom(UI3DEventCom evtCom)
        {
            if (evtCom != null)
            {
                evtCom.isDead = true;
                MinimapSys theMinimapSys = Singleton<CBattleSystem>.instance.TheMinimapSys;
                if (theMinimapSys != null)
                {
                    UI3DEventMgr mgr = theMinimapSys.UI3DEventMgr;
                    if (mgr != null)
                    {
                        mgr.UnRegister(evtCom);
                    }
                }
            }
        }
    }
}

