namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class CWatchSystem : Singleton<CWatchSystem>
    {
        private BattleStatView _battleStatView;
        private CUIListScript _camp1BaseList;
        private CUIListScript _camp1EquipList;
        private GameObject _camp1List;
        private CUIListScript _camp2BaseList;
        private CUIListScript _camp2EquipList;
        private GameObject _camp2List;
        private CUIFormScript _form;
        private HeroInfoHud _heroInfoHud;
        private DictionaryView<uint, HeroInfoItem> _heroWrapDict;
        private GameObject _hideBtn;
        private bool _isBottomFold;
        private bool _isCampFold_1;
        private bool _isCampFold_2;
        private bool _isViewHide;
        private bool _kdaChanged;
        private KillNotify _killNotify;
        private MinimapSys _miniMapSys;
        private PoolObjHandle<ActorRoot> _pickedHero;
        private ReplayControl _replayControl;
        private WatchScoreHud _scoreHud;
        private GameObject _showBtn;
        private CUIContainerScript _textHud;
        public static string s_watchUIForm = "UGUI/Form/Battle/Form_Watch.prefab";

        public void CloseForm()
        {
            if (null != this._form)
            {
                Singleton<CUIManager>.GetInstance().CloseForm(this._form);
                this._form = null;
            }
        }

        private void FocusHeroPicked(COM_PLAYERCAMP listCamp, int listIndex)
        {
            if (this._heroWrapDict != null)
            {
                HeroInfoItem item = null;
                DictionaryView<uint, HeroInfoItem>.Enumerator enumerator = this._heroWrapDict.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    KeyValuePair<uint, HeroInfoItem> current = enumerator.Current;
                    HeroInfoItem item2 = current.Value;
                    if ((item2.listCamp == listCamp) && (item2.listIndex == listIndex))
                    {
                        item = item2;
                        break;
                    }
                }
                if (item != null)
                {
                    MonoSingleton<CameraSystem>.instance.ToggleFreeDragCamera(false);
                    MonoSingleton<CameraSystem>.instance.SetFocusActor(item.HeroInfo.actorHero);
                    this._pickedHero = item.HeroInfo.actorHero;
                    this._heroInfoHud.SetPickHero(item.HeroInfo);
                }
                else
                {
                    this._pickedHero.Release();
                }
            }
        }

        public override void Init()
        {
        }

        private void InitCampInfoUIList(COM_PLAYERCAMP listCamp, List<HeroKDA> heroList, CUIListScript baseInfoUIList, CUIListScript equipInfoUIList)
        {
            if (((null != baseInfoUIList) && (heroList != null)) && (heroList.Count != 0))
            {
                baseInfoUIList.SetElementAmount(5);
                equipInfoUIList.SetElementAmount(5);
                for (int i = 0; i < 5; i++)
                {
                    GameObject baseInfoItem = baseInfoUIList.GetElemenet(i).get_gameObject();
                    GameObject equipInfoItem = equipInfoUIList.GetElemenet(i).get_gameObject();
                    if (i < heroList.Count)
                    {
                        HeroKDA okda = heroList[i];
                        HeroInfoItem item = new HeroInfoItem(listCamp, i, okda, baseInfoItem, equipInfoItem);
                        this._heroWrapDict.Add(okda.actorHero.handle.ObjID, item);
                    }
                    else
                    {
                        HeroInfoItem.MakeEmpty(baseInfoItem, equipInfoItem);
                    }
                }
            }
        }

        public void LateUpdate()
        {
            if (this._heroWrapDict != null)
            {
                if (!Singleton<WatchController>.GetInstance().IsWatching)
                {
                    Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Battle_CloseForm);
                    this.CloseForm();
                    Singleton<GameBuilder>.GetInstance().EndGame();
                }
                else
                {
                    DictionaryView<uint, HeroInfoItem>.Enumerator enumerator = this._heroWrapDict.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        KeyValuePair<uint, HeroInfoItem> current = enumerator.Current;
                        current.Value.LateUpdate();
                        if (this._kdaChanged)
                        {
                            KeyValuePair<uint, HeroInfoItem> pair2 = enumerator.Current;
                            pair2.Value.ValidateKDA();
                        }
                    }
                    if (this._heroInfoHud != null)
                    {
                        this._heroInfoHud.LateUpdate();
                        if (this._kdaChanged)
                        {
                            this._heroInfoHud.ValidateKDA();
                        }
                    }
                    if (this._scoreHud != null)
                    {
                        this._scoreHud.LateUpdate();
                    }
                    if (this._battleStatView != null)
                    {
                        this._battleStatView.LateUpdate();
                    }
                    if (this._replayControl != null)
                    {
                        this._replayControl.LateUpdate();
                    }
                    this._kdaChanged = false;
                }
            }
        }

        public void LoadForm()
        {
            this._form = Singleton<CUIManager>.GetInstance().OpenForm(s_watchUIForm, false, true);
        }

        private void OnBattleEquipChange(uint actorObjectID, stEquipInfo[] equips)
        {
            if (this._heroWrapDict.ContainsKey(actorObjectID))
            {
                this._heroWrapDict[actorObjectID].ValidateEquip();
            }
            if (actorObjectID == this._heroInfoHud.PickedHeroID)
            {
                this._heroInfoHud.ValidateEquip();
            }
        }

        private void OnBattleKDAChange()
        {
            this._kdaChanged = true;
        }

        private void OnBattleMoneyChange(PoolObjHandle<ActorRoot> actor, int changeValue, int currentValue, bool isIncome)
        {
            if (this._heroWrapDict.ContainsKey(actor.handle.ObjID))
            {
                this._heroWrapDict[actor.handle.ObjID].ValidateMoney();
            }
            this.ValidateCampMoney();
            if (actor.handle.ObjID == this._heroInfoHud.PickedHeroID)
            {
                this._heroInfoHud.ValidateMoney();
            }
        }

        public void OnCameraDraging(CUIEvent uiEvent)
        {
            if (MonoSingleton<CameraSystem>.GetInstance().enableLockedCamera)
            {
                MonoSingleton<CameraSystem>.instance.ToggleFreeDragCamera(true);
                if (this._pickedHero != 0)
                {
                    MonoSingleton<CameraSystem>.instance.MobaCamera.SetAbsoluteLockLocation((Vector3) this._pickedHero.handle.ActorControl.actorLocation);
                }
            }
            MonoSingleton<CameraSystem>.GetInstance().MoveCamera(-uiEvent.m_pointerEventData.get_delta().x, -uiEvent.m_pointerEventData.get_delta().y);
        }

        private void OnClickBottomFold(CUIEvent evt)
        {
            CUIAnimatorScript componetInChild = Utility.GetComponetInChild<CUIAnimatorScript>(this._form.get_gameObject(), "HeroInfoHud");
            CUIAnimatorScript script2 = Utility.GetComponetInChild<CUIAnimatorScript>(this._form.get_gameObject(), "ReplayControl");
            if ((componetInChild != null) && (script2 != null))
            {
                this._isBottomFold = !this._isBottomFold;
                componetInChild.PlayAnimator(!this._isBottomFold ? "Open" : "Close");
                script2.PlayAnimator(!this._isBottomFold ? "Open" : "Close");
            }
        }

        private void OnClickCampFold_1(CUIEvent evt)
        {
            CUIAnimatorScript componetInChild = Utility.GetComponetInChild<CUIAnimatorScript>(this._form.get_gameObject(), "CampInfoList_1/EquipInfoList");
            if (componetInChild != null)
            {
                this._isCampFold_1 = !this._isCampFold_1;
                componetInChild.PlayAnimator(!this._isCampFold_1 ? "Open" : "Close");
            }
        }

        private void OnClickCampFold_1_End(CUIEvent evt)
        {
            this._form.m_sgameGraphicRaycaster.RefreshGameObject(Utility.FindChild(this._form.get_gameObject(), "CampInfoList_1/EquipInfoList/FoldButton"));
        }

        private void OnClickCampFold_2(CUIEvent evt)
        {
            CUIAnimatorScript componetInChild = Utility.GetComponetInChild<CUIAnimatorScript>(this._form.get_gameObject(), "CampInfoList_2/EquipInfoList");
            if (componetInChild != null)
            {
                this._isCampFold_2 = !this._isCampFold_2;
                componetInChild.PlayAnimator(!this._isCampFold_2 ? "Open" : "Close");
            }
        }

        private void OnClickCampFold_2_End(CUIEvent evt)
        {
            this._form.m_sgameGraphicRaycaster.RefreshGameObject(Utility.FindChild(this._form.get_gameObject(), "CampInfoList_2/EquipInfoList/FoldButton"));
        }

        private void OnClickReplayTalk(CUIEvent evt)
        {
            Singleton<CUIManager>.GetInstance().OpenTips("barrageNotReady", true, 1.5f, null, new object[0]);
        }

        private void OnCloseForm(CUIEvent uiEvt)
        {
            this.UnRegisterEvents();
            if (this._heroWrapDict != null)
            {
                this._heroWrapDict.Clear();
                this._heroWrapDict = null;
            }
            if (this._miniMapSys != null)
            {
                this._miniMapSys.Clear();
                this._miniMapSys = null;
            }
            if (this._scoreHud != null)
            {
                this._scoreHud.Clear();
                this._scoreHud = null;
            }
            if (this._heroInfoHud != null)
            {
                this._heroInfoHud.Clear();
                this._heroInfoHud = null;
            }
            if (this._replayControl != null)
            {
                this._replayControl.Clear();
                this._replayControl = null;
            }
            if (this._killNotify != null)
            {
                this._killNotify.Clear();
                this._killNotify = null;
            }
            if (this._battleStatView != null)
            {
                this._battleStatView.Clear();
                this._battleStatView = null;
            }
            this._camp1List = null;
            this._camp2List = null;
            this._camp1BaseList = null;
            this._camp1EquipList = null;
            this._camp2BaseList = null;
            this._camp2EquipList = null;
            this._hideBtn = null;
            this._showBtn = null;
            this._form = null;
        }

        private void OnHeroEpChange(PoolObjHandle<ActorRoot> hero, int iCurVal, int iMaxVal)
        {
            if (hero.handle.ObjID == this._heroInfoHud.PickedHeroID)
            {
                this._heroInfoHud.ValidateEp();
            }
        }

        private void OnHeroHpChange(PoolObjHandle<ActorRoot> hero, int iCurVal, int iMaxVal)
        {
            if (hero.handle.ObjID == this._heroInfoHud.PickedHeroID)
            {
                this._heroInfoHud.ValidateHp();
            }
        }

        private void OnHeroLevelChange(PoolObjHandle<ActorRoot> hero, int level)
        {
            if (this._heroWrapDict.ContainsKey(hero.handle.ObjID))
            {
                this._heroWrapDict[hero.handle.ObjID].ValidateLevel();
            }
            if (hero.handle.ObjID == this._heroInfoHud.PickedHeroID)
            {
                this._heroInfoHud.ValidateLevel();
            }
        }

        public void OnPickCampList_1(CUIEvent uiEvent)
        {
            this.FocusHeroPicked(COM_PLAYERCAMP.COM_PLAYERCAMP_1, uiEvent.m_srcWidgetIndexInBelongedList);
        }

        public void OnPickCampList_2(CUIEvent uiEvent)
        {
            this.FocusHeroPicked(COM_PLAYERCAMP.COM_PLAYERCAMP_2, uiEvent.m_srcWidgetIndexInBelongedList);
        }

        private void OnQuitConfirm(CUIEvent uiEvent)
        {
            Singleton<WatchController>.GetInstance().Quit();
        }

        private void OnQuitWatch(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("confirmQuitWatch"), enUIEventID.Watch_QuitConfirm, enUIEventID.Watch_QuitCancel, false);
        }

        private void OnSkillCDChanged(ref DefaultSkillEventParam _param)
        {
            if ((this._heroInfoHud != null) && (_param.actor.handle.ObjID == this._heroInfoHud.PickedHeroID))
            {
                this._heroInfoHud.TheSkillHud.ValidateCD(_param.slot);
            }
        }

        private void OnSkillLevelUp(PoolObjHandle<ActorRoot> hero, byte bSlotType, byte bSkillLevel)
        {
            if ((this._heroInfoHud != null) && (hero.handle.ObjID == this._heroInfoHud.PickedHeroID))
            {
                this._heroInfoHud.TheSkillHud.ValidateLevel((SkillSlotType) bSlotType);
            }
        }

        private void OnToggleHide(CUIEvent uiEvent)
        {
            if ((this._form != null) && (this._form.get_gameObject() != null))
            {
                this._isViewHide = !this._isViewHide;
                float num = !this._isViewHide ? 1f : 0f;
                this._hideBtn.CustomSetActive(!this._isViewHide);
                this._showBtn.CustomSetActive(this._isViewHide);
                this._camp1List.GetComponent<CanvasGroup>().set_alpha(num);
                this._camp2List.GetComponent<CanvasGroup>().set_alpha(num);
                this._replayControl.Root.GetComponent<CanvasGroup>().set_alpha(num);
                if (this._isViewHide && !this._isBottomFold)
                {
                    this.OnClickBottomFold(null);
                }
            }
        }

        private void OnToggleStatView(CUIEvent evt)
        {
            if (this._battleStatView != null)
            {
                this._battleStatView.Toggle();
            }
        }

        public void OpenForm()
        {
            this._form = Singleton<CUIManager>.GetInstance().OpenForm(s_watchUIForm, false, true);
            DebugHelper.Assert(null != this._form, "CWathSystem:root form is null!");
            if (null != this._form)
            {
                SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
                ResDT_LevelCommonInfo info = CLevelCfgLogicManager.FindLevelConfigMultiGame(curLvelContext.iLevelID);
                this._scoreHud = new WatchScoreHud(Utility.FindChild(this._form.get_gameObject(), "ScoreBoard"));
                this._miniMapSys = new MinimapSys();
                this._miniMapSys.Init(this._form, Utility.FindChild(this._form.get_gameObject(), "MapPanel/Mini"), Utility.FindChild(this._form.get_gameObject(), "MapPanel/Big"), info, curLvelContext);
                this._camp1List = Utility.FindChild(this._form.get_gameObject(), "CampInfoList_1");
                this._camp1BaseList = Utility.GetComponetInChild<CUIListScript>(this._camp1List, "BaseInfoList");
                this._camp1EquipList = Utility.GetComponetInChild<CUIListScript>(this._camp1List, "EquipInfoList");
                this._camp2List = Utility.FindChild(this._form.get_gameObject(), "CampInfoList_2");
                this._camp2BaseList = Utility.GetComponetInChild<CUIListScript>(this._camp2List, "BaseInfoList");
                this._camp2EquipList = Utility.GetComponetInChild<CUIListScript>(this._camp2List, "EquipInfoList");
                this._hideBtn = Utility.FindChild(this._form.get_gameObject(), "PanelBtn/HideBtn");
                this._showBtn = Utility.FindChild(this._form.get_gameObject(), "PanelBtn/ShowBtn");
                this._heroInfoHud = new HeroInfoHud(Utility.FindChild(this._form.get_gameObject(), "HeroInfoHud"));
                this._replayControl = new ReplayControl(Utility.FindChild(this._form.get_gameObject(), "ReplayControl"));
                this._killNotify = new KillNotify();
                this._killNotify.Init(Utility.FindChild(this._form.get_gameObject(), "KillNotify_New"));
                this._killNotify.Hide();
                this._battleStatView = new BattleStatView();
                this._battleStatView.Init();
            }
        }

        private void RegisterEvents()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Watch_CameraDraging, new CUIEventManager.OnUIEventHandler(this.OnCameraDraging));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Watch_PickCampList_1, new CUIEventManager.OnUIEventHandler(this.OnPickCampList_1));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Watch_PickCampList_2, new CUIEventManager.OnUIEventHandler(this.OnPickCampList_2));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Watch_Quit, new CUIEventManager.OnUIEventHandler(this.OnQuitWatch));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Watch_ClickCampFold_1, new CUIEventManager.OnUIEventHandler(this.OnClickCampFold_1));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Watch_ClickCampFold_1_End, new CUIEventManager.OnUIEventHandler(this.OnClickCampFold_1_End));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Watch_ClickCampFold_2, new CUIEventManager.OnUIEventHandler(this.OnClickCampFold_2));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Watch_ClickCampFold_2_End, new CUIEventManager.OnUIEventHandler(this.OnClickCampFold_2_End));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Watch_ClickBottomFold, new CUIEventManager.OnUIEventHandler(this.OnClickBottomFold));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Watch_ClickReplayTalk, new CUIEventManager.OnUIEventHandler(this.OnClickReplayTalk));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Watch_HideView, new CUIEventManager.OnUIEventHandler(this.OnToggleHide));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Watch_QuitConfirm, new CUIEventManager.OnUIEventHandler(this.OnQuitConfirm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Watch_FormClosed, new CUIEventManager.OnUIEventHandler(this.OnCloseForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_ToggleStatView, new CUIEventManager.OnUIEventHandler(this.OnToggleStatView));
            Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int>("HeroSoulLevelChange", new Action<PoolObjHandle<ActorRoot>, int>(this, (IntPtr) this.OnHeroLevelChange));
            Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int, int, bool>("HeroGoldCoinInBattleChange", new Action<PoolObjHandle<ActorRoot>, int, int, bool>(this, (IntPtr) this.OnBattleMoneyChange));
            Singleton<EventRouter>.GetInstance().AddEventHandler<uint, stEquipInfo[]>("HeroEquipInBattleChange", new Action<uint, stEquipInfo[]>(this, (IntPtr) this.OnBattleEquipChange));
            Singleton<EventRouter>.instance.AddEventHandler(EventID.BATTLE_KDA_CHANGED_BY_ACTOR_DEAD, new Action(this, (IntPtr) this.OnBattleKDAChange));
            Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int, int>("HeroEnergyChange", new Action<PoolObjHandle<ActorRoot>, int, int>(this, (IntPtr) this.OnHeroEpChange));
            Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int, int>("HeroHpChange", new Action<PoolObjHandle<ActorRoot>, int, int>(this, (IntPtr) this.OnHeroHpChange));
            Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<DefaultSkillEventParam>(GameSkillEventDef.AllEvent_ChangeSkillCD, new GameSkillEvent<DefaultSkillEventParam>(this.OnSkillCDChanged));
            Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, byte, byte>("HeroSkillLevelUp", new Action<PoolObjHandle<ActorRoot>, byte, byte>(this, (IntPtr) this.OnSkillLevelUp));
        }

        public void StartFight()
        {
            if (this._heroWrapDict == null)
            {
                Singleton<CBattleSystem>.GetInstance().m_FormScript.SetActive(false);
                this._heroWrapDict = new DictionaryView<uint, HeroInfoItem>();
                List<HeroKDA> heroList = new List<HeroKDA>();
                List<HeroKDA> list2 = new List<HeroKDA>();
                DictionaryView<uint, PlayerKDA>.Enumerator enumerator = Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
                    PlayerKDA rkda = current.Value;
                    if (rkda.PlayerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
                    {
                        ListView<HeroKDA>.Enumerator enumerator2 = rkda.GetEnumerator();
                        while (enumerator2.MoveNext())
                        {
                            heroList.Add(enumerator2.Current);
                        }
                    }
                    else if (rkda.PlayerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_2)
                    {
                        ListView<HeroKDA>.Enumerator enumerator3 = rkda.GetEnumerator();
                        while (enumerator3.MoveNext())
                        {
                            list2.Add(enumerator3.Current);
                        }
                    }
                }
                this.InitCampInfoUIList(COM_PLAYERCAMP.COM_PLAYERCAMP_1, heroList, this._camp1BaseList, this._camp1EquipList);
                this.InitCampInfoUIList(COM_PLAYERCAMP.COM_PLAYERCAMP_2, list2, this._camp2BaseList, this._camp2EquipList);
                ActorRoot handle = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer().Captain.handle;
                this.FocusHeroPicked(handle.TheActorMeta.ActorCamp, this._heroWrapDict[handle.ObjID].listIndex);
                this.ValidateCampMoney();
                this.RegisterEvents();
                this._isBottomFold = false;
                this._isCampFold_1 = false;
                this.OnClickCampFold_1(null);
                this._isCampFold_2 = false;
                this.OnClickCampFold_2(null);
                this._isViewHide = false;
            }
        }

        private void UnRegisterEvents()
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Watch_CameraDraging, new CUIEventManager.OnUIEventHandler(this.OnCameraDraging));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Watch_PickCampList_1, new CUIEventManager.OnUIEventHandler(this.OnPickCampList_1));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Watch_PickCampList_2, new CUIEventManager.OnUIEventHandler(this.OnPickCampList_2));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Watch_Quit, new CUIEventManager.OnUIEventHandler(this.OnQuitWatch));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Watch_ClickCampFold_1, new CUIEventManager.OnUIEventHandler(this.OnClickCampFold_1));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Watch_ClickCampFold_1_End, new CUIEventManager.OnUIEventHandler(this.OnClickCampFold_1_End));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Watch_ClickCampFold_2, new CUIEventManager.OnUIEventHandler(this.OnClickCampFold_2));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Watch_ClickCampFold_2_End, new CUIEventManager.OnUIEventHandler(this.OnClickCampFold_2_End));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Watch_ClickBottomFold, new CUIEventManager.OnUIEventHandler(this.OnClickBottomFold));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Watch_ClickReplayTalk, new CUIEventManager.OnUIEventHandler(this.OnClickReplayTalk));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Watch_HideView, new CUIEventManager.OnUIEventHandler(this.OnToggleHide));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Watch_QuitConfirm, new CUIEventManager.OnUIEventHandler(this.OnQuitConfirm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Watch_FormClosed, new CUIEventManager.OnUIEventHandler(this.OnCloseForm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_ToggleStatView, new CUIEventManager.OnUIEventHandler(this.OnToggleStatView));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int>("HeroSoulLevelChange", new Action<PoolObjHandle<ActorRoot>, int>(this, (IntPtr) this.OnHeroLevelChange));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int, int, bool>("HeroGoldCoinInBattleChange", new Action<PoolObjHandle<ActorRoot>, int, int, bool>(this, (IntPtr) this.OnBattleMoneyChange));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<uint, stEquipInfo[]>("HeroEquipInBattleChange", new Action<uint, stEquipInfo[]>(this, (IntPtr) this.OnBattleEquipChange));
            Singleton<EventRouter>.instance.RemoveEventHandler(EventID.BATTLE_KDA_CHANGED_BY_ACTOR_DEAD, new Action(this, (IntPtr) this.OnBattleKDAChange));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int, int>("HeroEnergyChange", new Action<PoolObjHandle<ActorRoot>, int, int>(this, (IntPtr) this.OnHeroEpChange));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int, int>("HeroHpChange", new Action<PoolObjHandle<ActorRoot>, int, int>(this, (IntPtr) this.OnHeroHpChange));
            Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<DefaultSkillEventParam>(GameSkillEventDef.AllEvent_ChangeSkillCD, new GameSkillEvent<DefaultSkillEventParam>(this.OnSkillCDChanged));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, byte, byte>("HeroSkillLevelUp", new Action<PoolObjHandle<ActorRoot>, byte, byte>(this, (IntPtr) this.OnSkillLevelUp));
        }

        public void UpdateLogic(int delta)
        {
            if (this._heroInfoHud != null)
            {
                this._heroInfoHud.UpdateLogic(delta);
            }
        }

        private void ValidateCampMoney()
        {
            int num = 0;
            int num2 = 0;
            DictionaryView<uint, HeroInfoItem>.Enumerator enumerator = this._heroWrapDict.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<uint, HeroInfoItem> current = enumerator.Current;
                if (current.Value.listCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
                {
                    KeyValuePair<uint, HeroInfoItem> pair2 = enumerator.Current;
                    num += pair2.Value.HeroInfo.TotalCoin;
                }
                else
                {
                    KeyValuePair<uint, HeroInfoItem> pair3 = enumerator.Current;
                    if (pair3.Value.listCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_2)
                    {
                        KeyValuePair<uint, HeroInfoItem> pair4 = enumerator.Current;
                        num2 += pair4.Value.HeroInfo.TotalCoin;
                    }
                }
            }
            this._scoreHud.ValidateMoney(num, num2);
        }

        public CUIFormScript Form
        {
            get
            {
                return this._form;
            }
        }

        public bool IsFormShow
        {
            get
            {
                return (null != this._form);
            }
        }

        public KillNotify TheKillNotify
        {
            get
            {
                return this._killNotify;
            }
        }

        public MinimapSys TheMinimapSys
        {
            get
            {
                return this._miniMapSys;
            }
        }

        public CUIContainerScript TheTextHud
        {
            get
            {
                return this._textHud;
            }
        }
    }
}

