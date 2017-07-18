namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.GameLogic.GameKernal;
    using Assets.Scripts.UI;
    using CSProtocol;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class BattleStatView
    {
        private int _defaultSelectIndex;
        private HeroItem[] _heroList0;
        private HeroItem[] _heroList1;
        private GameObject _root;
        private ChartView chartView;
        private const int HERO_MAX_NUM = 5;
        private GameObject heroView;
        private CUIListScript list;
        private bool m_battleHeroPropertyChange;
        private bool m_battleKDAChanged;
        private bool m_bListCampInited;
        private List<HeroKDA> m_heroListCamp1 = new List<HeroKDA>();
        private List<HeroKDA> m_heroListCamp2 = new List<HeroKDA>();
        private List<Player> m_playerListCamp1 = new List<Player>();
        private List<Player> m_playerListCamp2 = new List<Player>();
        private bool m_sortByCoin;
        private CUIFormScript m_statViewFormScript;
        private GameObject matchInfo;
        public static string s_battleStateViewUIForm = "UGUI/Form/Battle/Form_BattleStateView.prefab";
        private GameObject sortByCoinBtn;
        private GameObject valueInfo;

        public void Clear()
        {
            MonoSingleton<VoiceSys>.instance.ClearInBattleForbidMember();
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_BattleStatViewSortClick, new CUIEventManager.OnUIEventHandler(this.OnSortClick));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_CloseStatView, new CUIEventManager.OnUIEventHandler(this.onCloseClick));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BattleMatchInfo_InfoTypeChange, new CUIEventManager.OnUIEventHandler(this.OnBttleMatchInfoTabChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_StateViewClickHeroIcon, new CUIEventManager.OnUIEventHandler(this.OnBattle_StateViewClickHeroIcon));
            Singleton<EventRouter>.instance.RemoveEventHandler(EventID.BATTLE_KDA_CHANGED, new Action(this, (IntPtr) this.OnBattleKDAChanged));
            Singleton<EventRouter>.instance.RemoveEventHandler(EventID.BATTLE_HERO_PROPERTY_CHANGED, new Action(this, (IntPtr) this.OnBattleHeroPropertyChange));
            Singleton<CUIManager>.GetInstance().CloseForm(s_battleStateViewUIForm);
            this._root = null;
            this._heroList0 = null;
            this._heroList1 = null;
            this.m_heroListCamp1.Clear();
            this.m_heroListCamp2.Clear();
            this.m_playerListCamp1.Clear();
            this.m_playerListCamp2.Clear();
            this.m_bListCampInited = false;
            this.sortByCoinBtn = null;
            this._defaultSelectIndex = 0;
            this.m_statViewFormScript = null;
        }

        public void Hide()
        {
            if (null != this._root)
            {
                CUIFormScript statViewFormScript = this.m_statViewFormScript;
                if (statViewFormScript != null)
                {
                    statViewFormScript.Hide(enFormHideFlag.HideByCustom, true);
                    Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_ReviveTimeChange, new CUIEventManager.OnUIEventHandler(this.UpdateBattleState));
                    Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int>("HeroSoulLevelChange", new Action<PoolObjHandle<ActorRoot>, int>(this, (IntPtr) this.onSoulLvlChange));
                    if (this.chartView != null)
                    {
                        this.chartView.Hide();
                    }
                    Singleton<CUIParticleSystem>.instance.Show(null);
                }
            }
        }

        public void Init()
        {
            if (this.m_statViewFormScript == null)
            {
                CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(s_battleStateViewUIForm, false, true);
                this.m_statViewFormScript = script;
                this._root = script.get_gameObject().get_transform().Find("BattleStatView").get_gameObject();
                if (this._root != null)
                {
                    this._heroList0 = new HeroItem[5];
                    this._heroList1 = new HeroItem[5];
                    this.heroView = Utility.FindChild(this._root, "HeroView");
                    this.matchInfo = Utility.FindChild(this._root, "BattleMatchInfo");
                    this.valueInfo = Utility.FindChild(this._root, "HeroValueInfo");
                    GameObject obj2 = Utility.FindChild(this._root, "ChartView");
                    obj2.CustomSetActive(false);
                    if (Singleton<WatchController>.GetInstance().IsWatching)
                    {
                        this.chartView = new ChartView(obj2);
                    }
                    this.sortByCoinBtn = Utility.FindChild(script.get_gameObject(), "TopCommon/SortByCoin");
                    CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
                    if (masterRoleInfo != null)
                    {
                        this.m_sortByCoin = PlayerPrefs.GetInt(string.Format("Sgmae_Battle_SortByCoin_{0}", masterRoleInfo.playerUllUID)) > 0;
                    }
                    this.UpdateSortBtn();
                    if (((this.heroView != null) && (this.matchInfo != null)) && (this.valueInfo != null))
                    {
                        Animation component = this.matchInfo.get_transform().GetComponent<Animation>();
                        if (component != null)
                        {
                            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
                            if ((curLvelContext != null) && (!curLvelContext.m_bEnableOrnamentSlot || !curLvelContext.m_bEnableShopHorizonTab))
                            {
                                component.set_enabled(true);
                                component.Play();
                            }
                            else
                            {
                                component.set_enabled(false);
                            }
                        }
                        for (int i = 0; i < 5; i++)
                        {
                            this._heroList0[i] = new HeroItem(Utility.FindChild(this.heroView, "HeroList_0/" + i), Utility.FindChild(this.matchInfo, "HeroList_0/" + i), Utility.FindChild(this.valueInfo, "HeroList_0/" + i));
                            this._heroList1[i] = new HeroItem(Utility.FindChild(this.heroView, "HeroList_1/" + i), Utility.FindChild(this.matchInfo, "HeroList_1/" + i), Utility.FindChild(this.valueInfo, "HeroList_1/" + i));
                        }
                        GameObject listObj = script.get_gameObject().get_transform().Find("TopCommon/Panel_Menu/ListMenu").get_gameObject();
                        if (listObj != null)
                        {
                            string[] strArray;
                            this.list = listObj.GetComponent<CUIListScript>();
                            if (Singleton<WatchController>.GetInstance().IsWatching)
                            {
                                strArray = new string[] { Singleton<CTextManager>.GetInstance().GetText("BattleStateView_MatchInfo"), Singleton<CTextManager>.GetInstance().GetText("BattleStateView_HeroInfo"), Singleton<CTextManager>.GetInstance().GetText("BattleStateView_ExpTrend"), Singleton<CTextManager>.GetInstance().GetText("BattleStateView_EcoTrend") };
                            }
                            else
                            {
                                strArray = new string[] { Singleton<CTextManager>.GetInstance().GetText("BattleStateView_MatchInfo"), Singleton<CTextManager>.GetInstance().GetText("BattleStateView_HeroInfo") };
                            }
                            this._defaultSelectIndex = 0;
                            CUICommonSystem.InitMenuPanel(listObj, strArray, this._defaultSelectIndex, true);
                            this.heroView.CustomSetActive(true);
                            this.matchInfo.CustomSetActive(true);
                            this.valueInfo.CustomSetActive(false);
                            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_CloseStatView, new CUIEventManager.OnUIEventHandler(this.onCloseClick));
                            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BattleMatchInfo_InfoTypeChange, new CUIEventManager.OnUIEventHandler(this.OnBttleMatchInfoTabChange));
                            Singleton<EventRouter>.instance.AddEventHandler(EventID.BATTLE_KDA_CHANGED, new Action(this, (IntPtr) this.OnBattleKDAChanged));
                            Singleton<EventRouter>.instance.AddEventHandler(EventID.BATTLE_HERO_PROPERTY_CHANGED, new Action(this, (IntPtr) this.OnBattleHeroPropertyChange));
                            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_BattleStatViewSortClick, new CUIEventManager.OnUIEventHandler(this.OnSortClick));
                            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_StateViewClickHeroIcon, new CUIEventManager.OnUIEventHandler(this.OnBattle_StateViewClickHeroIcon));
                            this.Hide();
                        }
                    }
                }
            }
        }

        public void LateUpdate()
        {
            if (this.m_battleKDAChanged)
            {
                this.UpdateKDAView();
                this.m_battleKDAChanged = false;
            }
        }

        private void OnBattle_StateViewClickHeroIcon(CUIEvent uievent)
        {
            if (uievent.m_eventParams.commonUInt64Param1 != Singleton<GamePlayerCenter>.instance.GetHostPlayer().PlayerUId)
            {
                MonoSingleton<VoiceSys>.instance.SwitchForbidden(uievent.m_eventParams.commonUInt64Param1);
                this.RefreshVoiceStateIfNess();
            }
        }

        private void OnBattleHeroPropertyChange()
        {
            this.m_battleHeroPropertyChange = true;
        }

        private void OnBattleKDAChanged()
        {
            this.m_battleKDAChanged = true;
        }

        private void OnBttleMatchInfoTabChange(CUIEvent uiEvent)
        {
            CUIListScript component = uiEvent.m_srcWidget.GetComponent<CUIListScript>();
            if (component != null)
            {
                int selectedIndex = component.GetSelectedIndex();
                if ((selectedIndex >= 0) && (selectedIndex < component.GetElementAmount()))
                {
                    this._defaultSelectIndex = selectedIndex;
                    CUIFormScript statViewFormScript = this.m_statViewFormScript;
                    if (((statViewFormScript != null) && (statViewFormScript != null)) && (((this.heroView != null) || (this.matchInfo != null)) || (this.valueInfo != null)))
                    {
                        this.SortHeroAndPlayer();
                        if (this._defaultSelectIndex == 0)
                        {
                            this.heroView.CustomSetActive(true);
                            this.matchInfo.CustomSetActive(true);
                            this.valueInfo.CustomSetActive(false);
                            this.sortByCoinBtn.CustomSetActive(true);
                            this.UpdateKDAView();
                            if (this.chartView != null)
                            {
                                this.chartView.Hide();
                            }
                        }
                        else if (this._defaultSelectIndex == 1)
                        {
                            this.heroView.CustomSetActive(true);
                            this.matchInfo.CustomSetActive(false);
                            this.valueInfo.CustomSetActive(true);
                            this.sortByCoinBtn.CustomSetActive(true);
                            this.m_battleHeroPropertyChange = true;
                            this.UpdateBattleState(null);
                            if (this.chartView != null)
                            {
                                this.chartView.Hide();
                            }
                        }
                        else
                        {
                            this.heroView.CustomSetActive(false);
                            this.matchInfo.CustomSetActive(false);
                            this.valueInfo.CustomSetActive(false);
                            this.sortByCoinBtn.CustomSetActive(false);
                            this.m_battleHeroPropertyChange = false;
                            WatchForm watchForm = Singleton<CBattleSystem>.GetInstance().WatchForm;
                            if ((watchForm != null) && (this.chartView != null))
                            {
                                this.chartView.Show((this._defaultSelectIndex != 2) ? ChartView.ChartType.MoneyTrend : ChartView.ChartType.ExpTrend, (this._defaultSelectIndex != 2) ? watchForm.moneySample : watchForm.expSample, Singleton<CUIManager>.GetInstance().GetForm(s_battleStateViewUIForm));
                            }
                        }
                    }
                }
            }
        }

        private void onCloseClick(CUIEvent evt)
        {
            this.Hide();
        }

        private void OnSortClick(CUIEvent uievent)
        {
            this.SortByCoin = !this.SortByCoin;
        }

        private void onSoulLvlChange(PoolObjHandle<ActorRoot> act, int curVal)
        {
            if (this._root != null)
            {
                HeroItem[] itemArray = this._heroList0;
                for (int i = 0; i < itemArray.Length; i++)
                {
                    HeroItem item = itemArray[i];
                    if (((item != null) && item.Visible) && ((item.kdaData != null) && (item.kdaData.actorHero == act)))
                    {
                        item.level.set_text(curVal.ToString());
                    }
                    if (((i + 1) == itemArray.Length) && (itemArray == this._heroList0))
                    {
                        itemArray = this._heroList1;
                        i = -1;
                    }
                }
            }
        }

        public void RefreshVoiceStateIfNess()
        {
            CUIFormScript statViewFormScript = this.m_statViewFormScript;
            if ((statViewFormScript != null) && !statViewFormScript.IsHided())
            {
                int index = 0;
                Player curPlayer = null;
                HeroItem item = null;
                for (index = 0; index < this.m_playerListCamp1.Count; index++)
                {
                    if (index >= this._heroList0.Length)
                    {
                        break;
                    }
                    curPlayer = this.m_playerListCamp1[index];
                    item = this._heroList0[index];
                    item.updateHeroVoiceState(curPlayer);
                }
                for (index = 0; index < this.m_playerListCamp2.Count; index++)
                {
                    if (index >= this._heroList1.Length)
                    {
                        break;
                    }
                    curPlayer = this.m_playerListCamp2[index];
                    this._heroList1[index].updateHeroVoiceState(curPlayer);
                }
            }
        }

        public void Show()
        {
            if (null != this._root)
            {
                CUIFormScript statViewFormScript = this.m_statViewFormScript;
                if (statViewFormScript != null)
                {
                    statViewFormScript.Appear(enFormHideFlag.HideByCustom, true);
                    if (this.list != null)
                    {
                        this.list.SelectElement(-1, true);
                        this.list.SelectElement(this._defaultSelectIndex, true);
                    }
                    this.SortHeroAndPlayer();
                    this.UpdateBattleState(null);
                    this.UpdateKDAView();
                    Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_ReviveTimeChange, new CUIEventManager.OnUIEventHandler(this.UpdateBattleState));
                    Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int>("HeroSoulLevelChange", new Action<PoolObjHandle<ActorRoot>, int>(this, (IntPtr) this.onSoulLvlChange));
                    this.RefreshVoiceStateIfNess();
                    Singleton<CUIParticleSystem>.instance.Hide(null);
                }
            }
        }

        private int SortByCoinAndPos(Player left, Player right)
        {
            PlayerKDA playerKDA = Singleton<BattleStatistic>.instance.m_playerKDAStat.GetPlayerKDA(left.PlayerId);
            return (((Singleton<BattleStatistic>.instance.m_playerKDAStat.GetPlayerKDA(right.PlayerId).TotalCoin - playerKDA.TotalCoin) * 10) + (left.CampPos - right.CampPos));
        }

        private int SortByCoinAndPos(HeroKDA left, HeroKDA right)
        {
            return (((right.TotalCoin - left.TotalCoin) * 10) + (left.CampPos - right.CampPos));
        }

        private int SortByPos(Player left, Player right)
        {
            return (left.CampPos - right.CampPos);
        }

        private int SortByPos(HeroKDA left, HeroKDA right)
        {
            return (left.CampPos - right.CampPos);
        }

        private void SortHeroAndPlayer()
        {
            bool forceUpdate = true;
            if (((this.m_heroListCamp1.Count > 0) || (this.m_heroListCamp2.Count > 0)) || ((this.m_playerListCamp1.Count > 0) || (this.m_playerListCamp2.Count > 0)))
            {
                forceUpdate = false;
            }
            this.UpdateListCamp(forceUpdate);
            if (this.m_sortByCoin)
            {
                this.m_heroListCamp1.Sort(new Comparison<HeroKDA>(this.SortByCoinAndPos));
                this.m_heroListCamp2.Sort(new Comparison<HeroKDA>(this.SortByCoinAndPos));
                this.m_playerListCamp1.Sort(new Comparison<Player>(this.SortByCoinAndPos));
                this.m_playerListCamp2.Sort(new Comparison<Player>(this.SortByCoinAndPos));
            }
            else
            {
                this.m_heroListCamp1.Sort(new Comparison<HeroKDA>(this.SortByPos));
                this.m_heroListCamp2.Sort(new Comparison<HeroKDA>(this.SortByPos));
                this.m_playerListCamp1.Sort(new Comparison<Player>(this.SortByPos));
                this.m_playerListCamp2.Sort(new Comparison<Player>(this.SortByPos));
            }
        }

        public void Toggle()
        {
            if (this.Visible)
            {
                this.Hide();
            }
            else
            {
                this.Show();
            }
        }

        private void UpdateBattleState(CUIEvent evt = new CUIEvent())
        {
            if (null != this._root)
            {
                CUIFormScript statViewFormScript = this.m_statViewFormScript;
                if ((statViewFormScript != null) && !statViewFormScript.IsHided())
                {
                    int index = 0;
                    Player curPlayer = null;
                    HeroItem item = null;
                    for (index = 0; index < this.m_playerListCamp1.Count; index++)
                    {
                        if (index >= this._heroList0.Length)
                        {
                            break;
                        }
                        curPlayer = this.m_playerListCamp1[index];
                        item = this._heroList0[index];
                        item.updateReviceCD(curPlayer);
                        item.updateTalentSkillCD(curPlayer, this.m_statViewFormScript);
                        if (this.m_battleHeroPropertyChange)
                        {
                            item.updateHeroValue(curPlayer);
                        }
                    }
                    for (index = 0; index < this.m_playerListCamp2.Count; index++)
                    {
                        if (index >= this._heroList1.Length)
                        {
                            break;
                        }
                        curPlayer = this.m_playerListCamp2[index];
                        item = this._heroList1[index];
                        item.updateReviceCD(curPlayer);
                        item.updateTalentSkillCD(curPlayer, this.m_statViewFormScript);
                        if (this.m_battleHeroPropertyChange)
                        {
                            item.updateHeroValue(curPlayer);
                        }
                    }
                    if (this.m_battleHeroPropertyChange)
                    {
                        this.m_battleHeroPropertyChange = false;
                    }
                }
            }
        }

        private void UpdateKDAView()
        {
            if (null != this._root)
            {
                CUIFormScript statViewFormScript = this.m_statViewFormScript;
                if ((statViewFormScript != null) && !statViewFormScript.IsHided())
                {
                    int index = 0;
                    index = 0;
                    while (index < this.m_heroListCamp1.Count)
                    {
                        if (index < this._heroList0.Length)
                        {
                            this._heroList0[index].Visible = true;
                            this._heroList0[index].Validate(this.m_heroListCamp1[index]);
                        }
                        index++;
                    }
                    while (index < this._heroList0.Length)
                    {
                        this._heroList0[index].Visible = false;
                        index++;
                    }
                    index = 0;
                    index = 0;
                    while (index < this.m_heroListCamp2.Count)
                    {
                        if (index < this._heroList1.Length)
                        {
                            this._heroList1[index].Visible = true;
                            this._heroList1[index].Validate(this.m_heroListCamp2[index]);
                        }
                        index++;
                    }
                    while (index < this._heroList1.Length)
                    {
                        this._heroList1[index].Visible = false;
                        index++;
                    }
                }
            }
        }

        private void UpdateListCamp(bool forceUpdate)
        {
            if (forceUpdate || !this.m_bListCampInited)
            {
                this.m_playerListCamp1.Clear();
                this.m_playerListCamp2.Clear();
                List<Player> allPlayers = Singleton<GamePlayerCenter>.instance.GetAllPlayers();
                for (int i = 0; i < allPlayers.Count; i++)
                {
                    if (allPlayers[i].PlayerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
                    {
                        this.m_playerListCamp1.Add(allPlayers[i]);
                    }
                    else if (allPlayers[i].PlayerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_2)
                    {
                        this.m_playerListCamp2.Add(allPlayers[i]);
                    }
                }
                this.m_heroListCamp1.Clear();
                this.m_heroListCamp2.Clear();
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
                            this.m_heroListCamp1.Add(enumerator2.Current);
                        }
                    }
                    else if (rkda.PlayerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_2)
                    {
                        ListView<HeroKDA>.Enumerator enumerator3 = rkda.GetEnumerator();
                        while (enumerator3.MoveNext())
                        {
                            this.m_heroListCamp2.Add(enumerator3.Current);
                        }
                    }
                }
            }
        }

        private void UpdateSortBtn()
        {
            if (this.sortByCoinBtn != null)
            {
                Text componetInChild = Utility.GetComponetInChild<Text>(this.sortByCoinBtn, "Text");
                if (componetInChild != null)
                {
                    if (this.m_sortByCoin)
                    {
                        componetInChild.set_text(Singleton<CTextManager>.instance.GetText("Battle_Statistic_Sort_Coin"));
                    }
                    else
                    {
                        componetInChild.set_text(Singleton<CTextManager>.instance.GetText("Battle_Statistic_Sort_Common"));
                    }
                }
            }
        }

        public bool SortByCoin
        {
            get
            {
                return this.m_sortByCoin;
            }
            set
            {
                this.m_sortByCoin = value;
                this.m_battleHeroPropertyChange = true;
                this.UpdateSortBtn();
                this.SortHeroAndPlayer();
                this.UpdateKDAView();
                this.UpdateBattleState(null);
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
                if (masterRoleInfo != null)
                {
                    PlayerPrefs.SetInt(string.Format("Sgmae_Battle_SortByCoin_{0}", masterRoleInfo.playerUllUID), !value ? 0 : 1);
                    PlayerPrefs.Save();
                    CPlayerBehaviorStat.Plus(CPlayerBehaviorStat.BehaviorType.SortBYCoinBtnClick);
                }
            }
        }

        public bool Visible
        {
            get
            {
                CUIFormScript statViewFormScript = this.m_statViewFormScript;
                return ((statViewFormScript != null) && !statViewFormScript.IsHided());
            }
        }

        private class ChartView
        {
            private GameObject bannerExp;
            private GameObject bannerMoney;
            private Text camp1Data;
            private Text camp2Data;
            private CUIGraphLineScript drawArea;
            private GameObject root;
            private Text title;
            private const int X_TICK_MAX = 4;
            private Text[] xAxis;
            private const int Y_SECS_MUL = 100;
            private const int Y_SECS_NUM = 5;
            private const int Y_TICK_MAX = 8;
            private Text[] yAxis;

            public ChartView(GameObject root)
            {
                this.root = root;
                this.title = Utility.GetComponetInChild<Text>(root, "Title");
                this.bannerExp = Utility.FindChild(root, "Banner/Exp");
                this.bannerMoney = Utility.FindChild(root, "Banner/Coin");
                this.camp1Data = Utility.GetComponetInChild<Text>(root, "CmpBar/Camp1Data");
                this.camp2Data = Utility.GetComponetInChild<Text>(root, "CmpBar/Camp2Data");
                this.drawArea = Utility.GetComponetInChild<CUIGraphLineScript>(root, "DrawArea");
                this.xAxis = new Text[5];
                for (int i = 0; i <= 4; i++)
                {
                    this.xAxis[i] = Utility.GetComponetInChild<Text>(root, "xAxis/x_" + i);
                }
                this.yAxis = new Text[9];
                for (int j = 0; j <= 8; j++)
                {
                    this.yAxis[j] = Utility.GetComponetInChild<Text>(root, "yAxis/y_" + j);
                }
            }

            public void Hide()
            {
                this.root.CustomSetActive(false);
            }

            public void Show(ChartType chartType, SampleData data, CUIFormScript form)
            {
                this.root.CustomSetActive(true);
                string str = chartType.ToString();
                this.title.set_text(Singleton<CTextManager>.GetInstance().GetText(str + "_Title"));
                this.bannerExp.CustomSetActive(chartType == ChartType.ExpTrend);
                this.bannerMoney.CustomSetActive(chartType == ChartType.MoneyTrend);
                this.camp1Data.set_text(data.curDataLeft.ToString());
                this.camp2Data.set_text(data.curDataRight.ToString());
                float num = (data.count <= 1) ? 1f : (data.step * (data.count - 1));
                int num2 = Mathf.CeilToInt(num / 4f);
                for (int i = 0; i <= 4; i++)
                {
                    int num4 = i * num2;
                    this.xAxis[i].set_text(string.Format("{0:D2}:{1:D2}", num4 / 60, num4 % 60));
                }
                int num6 = Mathf.CeilToInt(((float) Math.Max(Math.Abs(data.min), Math.Abs(data.max))) / 5f);
                bool flag = (num6 % 100) == 0;
                num6 /= 100;
                if (!flag)
                {
                    num6++;
                }
                if (num6 < 1)
                {
                    num6 = 1;
                }
                num6 *= 100;
                int num7 = 4;
                for (int j = 1; j <= num7; j++)
                {
                    string str2 = "+" + (num6 * j);
                    this.yAxis[num7 + j].set_text(str2);
                    this.yAxis[num7 - j].set_text(str2);
                }
                RectTransform component = this.drawArea.GetComponent<RectTransform>();
                Vector3 vector = CUIUtility.WorldToScreenPoint(form.GetCamera(), component.get_position());
                float num9 = form.ChangeFormValueToScreen(component.get_rect().get_width());
                float num10 = form.ChangeFormValueToScreen(component.get_rect().get_height());
                vector.x -= num9 * 0.5f;
                float num11 = num9 / ((float) (num2 * 4));
                float num12 = (num10 * 0.5f) / ((float) (num6 * 5));
                Vector3[] vertexs = new Vector3[data.count];
                for (int k = 0; k < data.count; k++)
                {
                    vertexs[k] = new Vector3(vector.x + ((data.step * k) * num11), vector.y + (data[k] * num12));
                }
                this.drawArea.color = (chartType != ChartType.ExpTrend) ? Color.get_yellow() : Color.get_green();
                this.drawArea.thickness = 2f;
                this.drawArea.drawSpeed = 1000f;
                this.drawArea.SetVertexs(vertexs);
            }

            public enum ChartType
            {
                None,
                ExpTrend,
                MoneyTrend
            }
        }

        private class HeroItem
        {
            public Text assistNum;
            public GameObject ClickGameObject;
            public Text deadNum;
            public Image[] equipList = new Image[6];
            public Text heroAD;
            public Text heroADDef;
            public Text heroAP;
            public Text heroAPDef;
            public Text heroHP;
            public Text heroName;
            public Image horizonEquipImg;
            public Image icon;
            public HeroKDA kdaData;
            public Text killMon;
            public Text killNum;
            public Text level;
            public GameObject mineBg;
            public Text playerName;
            public Text reviveTime;
            public GameObject rootHeroView;
            public GameObject rootMatchInfo;
            public GameObject rootValueInfo;
            public GameObject talentSkill;
            public Text talentSkillCD;
            public Image talentSkillImage;
            public GameObject voiceIconsNode;

            public HeroItem(GameObject heroNode, GameObject matchNode, GameObject valueNode)
            {
                this.rootHeroView = heroNode;
                this.rootMatchInfo = matchNode;
                this.rootValueInfo = valueNode;
                this.ClickGameObject = Utility.FindChild(heroNode, "clickimg");
                this.icon = Utility.GetComponetInChild<Image>(heroNode, "HeadIcon");
                this.mineBg = Utility.FindChild(heroNode, "MineBg");
                this.level = Utility.GetComponetInChild<Text>(heroNode, "Level");
                this.playerName = Utility.GetComponetInChild<Text>(heroNode, "PlayerName");
                this.heroName = Utility.GetComponetInChild<Text>(heroNode, "HeroName");
                this.reviveTime = Utility.GetComponetInChild<Text>(heroNode, "ReviveTime");
                this.voiceIconsNode = Utility.FindChild(heroNode, "Voice");
                this.killNum = Utility.GetComponetInChild<Text>(matchNode, "KillNum");
                this.deadNum = Utility.GetComponetInChild<Text>(matchNode, "DeadNum");
                this.killMon = Utility.GetComponetInChild<Text>(matchNode, "KillMon");
                this.assistNum = Utility.GetComponetInChild<Text>(matchNode, "AssistNum");
                GameObject p = Utility.FindChild(matchNode, "TalentIcon");
                this.equipList[0] = Utility.GetComponetInChild<Image>(p, "img1");
                this.equipList[1] = Utility.GetComponetInChild<Image>(p, "img2");
                this.equipList[2] = Utility.GetComponetInChild<Image>(p, "img3");
                this.equipList[3] = Utility.GetComponetInChild<Image>(p, "img4");
                this.equipList[4] = Utility.GetComponetInChild<Image>(p, "img5");
                this.equipList[5] = Utility.GetComponetInChild<Image>(p, "img6");
                this.horizonEquipImg = Utility.GetComponetInChild<Image>(p, "img7");
                this.talentSkill = Utility.FindChild(matchNode, "TalentSkill");
                this.talentSkillImage = Utility.GetComponetInChild<Image>(this.talentSkill, "Image");
                this.talentSkillCD = Utility.GetComponetInChild<Text>(this.talentSkill, "TimeCD");
                this.heroHP = Utility.GetComponetInChild<Text>(valueNode, "ValueInfo/HP");
                this.heroAD = Utility.GetComponetInChild<Text>(valueNode, "ValueInfo/AD");
                this.heroAP = Utility.GetComponetInChild<Text>(valueNode, "ValueInfo/AP");
                this.heroADDef = Utility.GetComponetInChild<Text>(valueNode, "ValueInfo/ADDef");
                this.heroAPDef = Utility.GetComponetInChild<Text>(valueNode, "ValueInfo/APDef");
                heroNode.get_transform().FindChild("ReviveTime").get_gameObject().SetActive(true);
                matchNode.get_transform().FindChild("TalentSkill/TimeCD").get_gameObject().SetActive(true);
                this.kdaData = null;
            }

            private void _updateVoiceIcon(GameObject node, CS_VOICESTATE_TYPE value, bool bForbidden = false)
            {
                if (node != null)
                {
                    GameObject obj2 = node.get_transform().Find("AllOpen").get_gameObject();
                    GameObject obj3 = node.get_transform().Find("AllClose").get_gameObject();
                    GameObject obj4 = node.get_transform().Find("HalfOpen").get_gameObject();
                    GameObject obj5 = node.get_transform().Find("Forbidden").get_gameObject();
                    if (bForbidden)
                    {
                        obj2.CustomSetActive(false);
                        obj3.CustomSetActive(false);
                        obj4.CustomSetActive(false);
                        obj5.CustomSetActive(true);
                    }
                    else
                    {
                        obj5.CustomSetActive(false);
                        switch (value)
                        {
                            case CS_VOICESTATE_TYPE.CS_VOICESTATE_NONE:
                                obj2.CustomSetActive(false);
                                obj3.CustomSetActive(true);
                                obj4.CustomSetActive(false);
                                return;

                            case CS_VOICESTATE_TYPE.CS_VOICESTATE_PART:
                                obj2.CustomSetActive(false);
                                obj3.CustomSetActive(false);
                                obj4.CustomSetActive(true);
                                return;

                            case CS_VOICESTATE_TYPE.CS_VOICESTATE_FULL:
                                obj2.CustomSetActive(true);
                                obj3.CustomSetActive(false);
                                obj4.CustomSetActive(false);
                                return;
                        }
                    }
                }
            }

            private void SetEventParam(ulong uid)
            {
                if (this.ClickGameObject != null)
                {
                    CUIEventScript component = this.ClickGameObject.GetComponent<CUIEventScript>();
                    if (component != null)
                    {
                        component.m_onClickEventParams.commonUInt64Param1 = uid;
                    }
                }
            }

            public void updateHeroValue(Player curPlayer)
            {
                if (((curPlayer != null) && (curPlayer.Captain != 0)) && (curPlayer.Captain.handle.ValueComponent.mActorValue != null))
                {
                    ValueDataInfo[] actorValue = curPlayer.Captain.handle.ValueComponent.mActorValue.GetActorValue();
                    this.heroHP.set_text(string.Format("{0}", actorValue[5].totalValue));
                    this.heroAD.set_text(string.Format("{0}", actorValue[1].totalValue));
                    this.heroAP.set_text(string.Format("{0}", actorValue[2].totalValue));
                    this.heroADDef.set_text(string.Format("{0}", actorValue[3].totalValue));
                    this.heroAPDef.set_text(string.Format("{0}", actorValue[4].totalValue));
                }
            }

            public void updateHeroVoiceState(Player curPlayer)
            {
                if (((curPlayer != null) && (curPlayer.Captain != 0)) && (curPlayer.Captain.handle.ValueComponent.mActorValue != null))
                {
                    this.SetEventParam(curPlayer.PlayerUId);
                    Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
                    if (hostPlayer != null)
                    {
                        bool bActive = Singleton<GamePlayerCenter>.instance.IsAtSameCamp(hostPlayer.PlayerId, curPlayer.PlayerId);
                        this.voiceIconsNode.CustomSetActive(bActive);
                        if (bActive)
                        {
                            if (curPlayer.Computer && !Singleton<BattleLogic>.GetInstance().GetCurLvelContext().m_isWarmBattle)
                            {
                                this.voiceIconsNode.CustomSetActive(false);
                            }
                            if (hostPlayer.PlayerUId == curPlayer.PlayerUId)
                            {
                                this._updateVoiceIcon(this.voiceIconsNode, MonoSingleton<VoiceSys>.instance.curVoiceState, false);
                            }
                            else
                            {
                                CS_VOICESTATE_TYPE cs_voicestate_type = MonoSingleton<VoiceSys>.GetInstance().TryGetVoiceState(curPlayer.PlayerUId);
                                this._updateVoiceIcon(this.voiceIconsNode, cs_voicestate_type, MonoSingleton<VoiceSys>.instance.IsForbid(curPlayer.PlayerUId));
                            }
                        }
                    }
                }
            }

            public void updateReviceCD(Player curPlayer)
            {
                if (((curPlayer != null) && (curPlayer.Captain != 0)) && (curPlayer.Captain.handle.ActorControl != null))
                {
                    if (curPlayer.Captain.handle.ActorControl.IsDeadState)
                    {
                        this.reviveTime.set_text(SimpleNumericString.GetNumeric(Mathf.FloorToInt(curPlayer.Captain.handle.ActorControl.ReviveCooldown * 0.001f)));
                        this.icon.set_color(CUIUtility.s_Color_Grey);
                    }
                    else
                    {
                        this.reviveTime.set_text(string.Empty);
                        this.icon.set_color(CUIUtility.s_Color_Full);
                    }
                }
            }

            public void updateTalentSkillCD(Player curPlayer, CUIFormScript parentFormScript)
            {
                if (((curPlayer != null) && (curPlayer.Captain != 0)) && (curPlayer.Captain.handle.SkillControl != null))
                {
                    SkillSlot slot = curPlayer.Captain.handle.SkillControl.SkillSlotArray[5];
                    if (slot == null)
                    {
                        this.talentSkill.CustomSetActive(false);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(slot.SkillObj.IconName))
                        {
                            this.talentSkillImage.SetSprite(CUIUtility.s_Sprite_Dynamic_Skill_Dir + slot.SkillObj.IconName, parentFormScript, true, false, false, false);
                        }
                        this.talentSkill.CustomSetActive(true);
                        if (slot.CurSkillCD > 0)
                        {
                            this.talentSkillCD.set_text(SimpleNumericString.GetNumeric(Mathf.FloorToInt(((float) slot.CurSkillCD) * 0.001f)));
                            this.talentSkillImage.set_color(CUIUtility.s_Color_Grey);
                        }
                        else
                        {
                            this.talentSkillCD.set_text(string.Empty);
                            this.talentSkillImage.set_color(CUIUtility.s_Color_Full);
                        }
                    }
                }
            }

            public void Validate(HeroKDA kdaData)
            {
                if (kdaData != null)
                {
                    this.kdaData = kdaData;
                }
                if (this.kdaData != null)
                {
                    this.icon.SetSprite(string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Icon_Dir, CSkinInfo.GetHeroSkinPic((uint) this.kdaData.HeroId, 0)), Singleton<CBattleSystem>.instance.FormScript, true, false, false, false);
                    Player ownerPlayer = ActorHelper.GetOwnerPlayer(ref this.kdaData.actorHero);
                    this.playerName.set_text(ownerPlayer.Name);
                    this.heroName.set_text(this.kdaData.actorHero.handle.TheStaticData.TheResInfo.Name);
                    this.level.set_text(this.kdaData.actorHero.handle.ValueComponent.actorSoulLevel.ToString());
                    this.killNum.set_text(this.kdaData.numKill.ToString());
                    this.deadNum.set_text(this.kdaData.numDead.ToString());
                    this.killMon.set_text((this.kdaData.numKillMonster + this.kdaData.numKillSoldier).ToString());
                    this.killMon.set_text(this.kdaData.TotalCoin.ToString());
                    this.assistNum.set_text(this.kdaData.numAssist.ToString());
                    int num = 0;
                    for (int i = 0; i < 6; i++)
                    {
                        ushort equipID = this.kdaData.Equips[i].m_equipID;
                        if (equipID != 0)
                        {
                            CUICommonSystem.SetEquipIcon(equipID, this.equipList[num++].get_gameObject(), Singleton<CBattleSystem>.instance.FormScript);
                        }
                    }
                    for (int j = num; j < 6; j++)
                    {
                        this.equipList[j].get_gameObject().GetComponent<Image>().SetSprite(string.Format("{0}BattleSettle_EquipmentSpaceNew", CUIUtility.s_Sprite_Dynamic_Talent_Dir), Singleton<CBattleSystem>.instance.FormScript, true, false, false, false);
                    }
                    SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
                    if (((curLvelContext != null) && curLvelContext.m_bEnableOrnamentSlot) && curLvelContext.m_bEnableShopHorizonTab)
                    {
                        ushort horizonEquipId = this.kdaData.actorHero.handle.EquipComponent.m_horizonEquipId;
                        if (horizonEquipId == 0)
                        {
                            this.horizonEquipImg.SetSprite(string.Format("{0}BattleSettle_EquipmentSpaceNew", CUIUtility.s_Sprite_Dynamic_Talent_Dir), Singleton<CBattleSystem>.instance.FormScript, true, false, false, false);
                        }
                        else
                        {
                            CUICommonSystem.SetEquipIcon(horizonEquipId, this.horizonEquipImg.get_gameObject(), Singleton<CBattleSystem>.instance.FormScript);
                        }
                    }
                    if (ownerPlayer == Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer())
                    {
                        this.playerName.set_color(CUIUtility.s_Text_Color_Self);
                        this.mineBg.CustomSetActive(true);
                    }
                    else
                    {
                        if (ownerPlayer.PlayerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
                        {
                            this.playerName.set_color(CUIUtility.s_Text_Color_Camp_1);
                        }
                        else
                        {
                            this.playerName.set_color(CUIUtility.s_Text_Color_Camp_2);
                        }
                        this.mineBg.CustomSetActive(false);
                    }
                }
            }

            public bool Visible
            {
                get
                {
                    return ((((this.rootHeroView != null) && (this.rootMatchInfo != null)) && (this.rootValueInfo != null)) && ((this.rootHeroView.get_activeSelf() && this.rootMatchInfo.get_activeSelf()) && this.rootValueInfo.get_activeSelf()));
                }
                set
                {
                    if (((this.rootHeroView != null) && (this.rootMatchInfo != null)) && (this.rootValueInfo != null))
                    {
                        this.rootHeroView.CustomSetActive(value);
                        this.rootMatchInfo.CustomSetActive(value);
                        this.rootValueInfo.CustomSetActive(value);
                    }
                }
            }
        }
    }
}

