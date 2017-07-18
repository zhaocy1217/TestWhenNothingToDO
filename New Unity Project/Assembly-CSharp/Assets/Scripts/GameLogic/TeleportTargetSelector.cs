namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic.GameKernal;
    using Assets.Scripts.GameSystem;
    using Assets.Scripts.UI;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    internal class TeleportTargetSelector : Singleton<TeleportTargetSelector>
    {
        private Plane curPlane;
        public bool m_bConfirmed = true;
        private Dictionary<PoolObjHandle<ActorRoot>, GameObject> m_CachedTeleportGameObjects;
        private List<PoolObjHandle<ActorRoot>> m_CanTeleportActorList;
        public bool m_ClickDownStatus;
        private bool m_NeedConfirm;
        private bool m_TargetSkillEnable;
        private Ray screenRay;
        public const string TeleportNotePrefabName = "Prefab_Skill_Effects/tongyong_effects/Indicator/select_b_01";

        private void ClearTeleportTarget()
        {
            this.m_CanTeleportActorList = new List<PoolObjHandle<ActorRoot>>();
            this.m_bConfirmed = false;
            this.RefreshTeleportTargetEffect();
        }

        public override void Init()
        {
            this.curPlane = new Plane(new Vector3(0f, 1f, 0f), 0f);
            this.m_CanTeleportActorList = new List<PoolObjHandle<ActorRoot>>();
            this.m_CachedTeleportGameObjects = new Dictionary<PoolObjHandle<ActorRoot>, GameObject>();
            this.m_TargetSkillEnable = false;
            this.m_ClickDownStatus = false;
            this.m_NeedConfirm = false;
            this.m_bConfirmed = false;
            this.RegEvent();
        }

        private void OnActorDead(ref GameDeadEventParam prm)
        {
            if (ActorHelper.IsHostCtrlActor(ref prm.src))
            {
                this.ClearTeleportTarget();
            }
        }

        public void OnClickBattleScene(Vector2 _screenPos)
        {
            if ((!Singleton<WatchController>.instance.IsWatching && (!this.m_NeedConfirm || this.m_bConfirmed)) && (!MonoSingleton<CameraSystem>.GetInstance().enableLockedCamera || this.m_ClickDownStatus))
            {
                Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
                if (((hostPlayer != null) && (hostPlayer.Captain != 0)) && ((hostPlayer.Captain.handle.ActorControl != null) && !hostPlayer.Captain.handle.ActorControl.IsDeadState))
                {
                    float num;
                    uint targetObjId = 0;
                    Ray ray = Camera.get_main().ScreenPointToRay(_screenPos);
                    if (this.curPlane.Raycast(ray, ref num))
                    {
                        Vector3 point = ray.GetPoint(num);
                        if (Singleton<CBattleSystem>.GetInstance().FightForm != null)
                        {
                            SkillSlotType type;
                            CSkillButtonManager skillButtonManager = Singleton<CBattleSystem>.GetInstance().FightForm.m_skillButtonManager;
                            if ((skillButtonManager != null) && skillButtonManager.HasMapSlectTargetSkill(out type))
                            {
                                targetObjId = Singleton<TeleportTargetSearcher>.GetInstance().SearchNearestCanTeleportTarget(ref hostPlayer.Captain, (VInt3) point, 0xbb8);
                                if (targetObjId != 0)
                                {
                                    skillButtonManager.SelectedMapTarget(targetObjId);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void OnDragMiniMap(CUIEvent uievent)
        {
            this.m_ClickDownStatus = true;
        }

        private void OnDragMiniMapEnd(CUIEvent uievent)
        {
            this.ClearTeleportTarget();
        }

        private void OnMiniMap_Click_Down(CUIEvent uievent)
        {
            this.m_ClickDownStatus = true;
        }

        private void OnMiniMap_Click_Up(CUIEvent uievent)
        {
            this.m_ClickDownStatus = false;
            this.ClearTeleportTarget();
        }

        private void OnPlayerSkillDisable()
        {
            this.ClearTeleportTarget();
        }

        private void OnPlayerSkillEnable()
        {
            if (!MonoSingleton<CameraSystem>.GetInstance().enableLockedCamera)
            {
                this.UpdateTeleportTargetList();
                this.RefreshTeleportTargetEffect();
            }
        }

        private void RefreshSkillEnableState()
        {
            if (Singleton<CBattleSystem>.GetInstance().FightForm != null)
            {
                SkillSlotType type;
                CSkillButtonManager skillButtonManager = Singleton<CBattleSystem>.GetInstance().FightForm.m_skillButtonManager;
                if (skillButtonManager.HasMapSlectTargetSkill(out type))
                {
                    SkillSlot slot = null;
                    Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
                    if (((hostPlayer != null) && (hostPlayer.Captain != 0)) && hostPlayer.Captain.handle.SkillControl.TryGetSkillSlot(type, out slot))
                    {
                        this.m_NeedConfirm = skillButtonManager.GetSkillJoystickMode(type) == enSkillJoystickMode.MapSelectOther;
                        if (this.m_TargetSkillEnable != slot.IsEnableSkillSlot())
                        {
                            this.m_TargetSkillEnable = !this.m_TargetSkillEnable;
                            if (this.m_TargetSkillEnable)
                            {
                                this.OnPlayerSkillEnable();
                            }
                            else
                            {
                                this.OnPlayerSkillDisable();
                            }
                        }
                    }
                }
            }
        }

        private void RefreshTeleportTargetEffect()
        {
            List<PoolObjHandle<ActorRoot>> list = new List<PoolObjHandle<ActorRoot>>();
            Dictionary<PoolObjHandle<ActorRoot>, GameObject>.Enumerator enumerator = this.m_CachedTeleportGameObjects.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<PoolObjHandle<ActorRoot>, GameObject> current = enumerator.Current;
                if (!this.m_CanTeleportActorList.Contains(current.Key))
                {
                    if (current.Value != null)
                    {
                        Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(current.Value);
                    }
                    list.Add(current.Key);
                }
            }
            for (int i = 0; i < list.Count; i++)
            {
                PoolObjHandle<ActorRoot> key = list[i];
                if (this.m_CachedTeleportGameObjects.ContainsKey(key))
                {
                    this.m_CachedTeleportGameObjects.Remove(key);
                }
            }
            if (this.m_NeedConfirm)
            {
                if (this.m_bConfirmed)
                {
                    for (int j = 0; j < this.m_CanTeleportActorList.Count; j++)
                    {
                        PoolObjHandle<ActorRoot> handle2 = this.m_CanTeleportActorList[j];
                        if (!this.m_CachedTeleportGameObjects.ContainsKey(handle2))
                        {
                            this.ShowTeleportNoteEffect(handle2);
                        }
                    }
                }
            }
            else
            {
                for (int k = 0; k < this.m_CanTeleportActorList.Count; k++)
                {
                    PoolObjHandle<ActorRoot> handle3 = this.m_CanTeleportActorList[k];
                    if (!this.m_CachedTeleportGameObjects.ContainsKey(handle3))
                    {
                        this.ShowTeleportNoteEffect(handle3);
                    }
                }
            }
            if (this.m_NeedConfirm)
            {
                this.ShowSkillButtonFlowEffect((((this.m_CanTeleportActorList.Count > 0) && !this.m_bConfirmed) && this.m_ClickDownStatus) && this.m_TargetSkillEnable);
            }
            else
            {
                this.ShowSkillButtonFlowEffect((this.m_CachedTeleportGameObjects.Count > 0) && this.m_TargetSkillEnable);
            }
        }

        private void RegEvent()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Down_MiniMap, new CUIEventManager.OnUIEventHandler(this.OnMiniMap_Click_Down));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Click_MiniMap_Up, new CUIEventManager.OnUIEventHandler(this.OnMiniMap_Click_Up));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Drag_SignalPanel, new CUIEventManager.OnUIEventHandler(this.OnDragMiniMap));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Drag_SignalPanel_End, new CUIEventManager.OnUIEventHandler(this.OnDragMiniMapEnd));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnPanelCameraClickDown, new CUIEventManager.OnUIEventHandler(this.OnMiniMap_Click_Down));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnPanelCameraClickUp, new CUIEventManager.OnUIEventHandler(this.OnMiniMap_Click_Up));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnCameraAxisPushed, new CUIEventManager.OnUIEventHandler(this.OnMiniMap_Click_Down));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnCameraAxisReleased, new CUIEventManager.OnUIEventHandler(this.OnMiniMap_Click_Up));
            Singleton<GameEventSys>.GetInstance().AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnActorDead));
        }

        private void ShowSkillButtonFlowEffect(bool show)
        {
            if ((Singleton<CBattleSystem>.GetInstance().FightForm != null) && (Singleton<CBattleSystem>.GetInstance().FightForm.m_skillButtonManager != null))
            {
                SkillSlotType type;
                CSkillButtonManager skillButtonManager = Singleton<CBattleSystem>.GetInstance().FightForm.m_skillButtonManager;
                if (skillButtonManager.HasMapSlectTargetSkill(out type))
                {
                    SkillButton button = skillButtonManager.GetButton(type);
                    if ((button != null) && (button.m_button != null))
                    {
                        skillButtonManager.SetButtonFlowLight(button.m_button, show);
                    }
                }
            }
        }

        public void ShowTeleportNoteEffect(PoolObjHandle<ActorRoot> actorRoot)
        {
            if (!this.m_CachedTeleportGameObjects.ContainsKey(actorRoot))
            {
                bool isInit = false;
                GameObject obj2 = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD("Prefab_Skill_Effects/tongyong_effects/Indicator/select_b_01", false, SceneObjType.ActionRes, Vector3.get_zero(), Quaternion.get_identity(), out isInit);
                if (obj2 != null)
                {
                    obj2.get_transform().SetParent(actorRoot.handle.myTransform);
                    obj2.get_transform().set_localPosition(Vector3.get_zero());
                    obj2.get_transform().set_localRotation(Quaternion.get_identity());
                }
                this.m_CachedTeleportGameObjects[actorRoot] = obj2;
            }
        }

        public override void UnInit()
        {
            this.m_CanTeleportActorList.Clear();
            this.m_CachedTeleportGameObjects.Clear();
            this.UnregEvent();
        }

        private void UnregEvent()
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Down_MiniMap, new CUIEventManager.OnUIEventHandler(this.OnMiniMap_Click_Down));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Click_MiniMap_Up, new CUIEventManager.OnUIEventHandler(this.OnMiniMap_Click_Up));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Drag_SignalPanel, new CUIEventManager.OnUIEventHandler(this.OnDragMiniMap));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Drag_SignalPanel_End, new CUIEventManager.OnUIEventHandler(this.OnDragMiniMapEnd));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnPanelCameraClickDown, new CUIEventManager.OnUIEventHandler(this.OnMiniMap_Click_Down));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnPanelCameraClickUp, new CUIEventManager.OnUIEventHandler(this.OnMiniMap_Click_Up));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnCameraAxisPushed, new CUIEventManager.OnUIEventHandler(this.OnMiniMap_Click_Down));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnCameraAxisReleased, new CUIEventManager.OnUIEventHandler(this.OnMiniMap_Click_Up));
            Singleton<GameEventSys>.GetInstance().RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnActorDead));
        }

        public void Update()
        {
            if (!Singleton<WatchController>.instance.IsWatching)
            {
                this.RefreshSkillEnableState();
                this.UpdateTargetAfterClickDown();
            }
        }

        private void UpdateTargetAfterClickDown()
        {
            if (this.m_ClickDownStatus)
            {
                this.UpdateTeleportTargetList();
                this.RefreshTeleportTargetEffect();
            }
        }

        private void UpdateTeleportTargetList()
        {
            SkillSlotType type;
            this.m_CanTeleportActorList.Clear();
            if (Singleton<CBattleSystem>.GetInstance().FightForm.m_skillButtonManager.HasMapSlectTargetSkill(out type))
            {
                SkillSlot slot = null;
                Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
                if ((((hostPlayer != null) && (hostPlayer.Captain != 0)) && hostPlayer.Captain.handle.SkillControl.TryGetSkillSlot(type, out slot)) && slot.IsEnableSkillSlot())
                {
                    List<PoolObjHandle<ActorRoot>> gameActors = Singleton<GameObjMgr>.instance.GameActors;
                    int count = gameActors.Count;
                    for (int i = 0; i < count; i++)
                    {
                        PoolObjHandle<ActorRoot> item = gameActors[i];
                        if ((item != 0) && (item != hostPlayer.Captain))
                        {
                            ActorRoot handle = item.handle;
                            if (((!handle.ActorControl.IsDeadState && handle.IsHostCamp()) && handle.InCamera) && ((slot.SkillObj.cfgData.dwSkillTargetFilter & (((int) 1) << handle.TheActorMeta.ActorType)) <= 0L))
                            {
                                this.m_CanTeleportActorList.Add(item);
                            }
                        }
                    }
                }
            }
        }
    }
}

