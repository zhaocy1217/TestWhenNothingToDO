namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    [MessageHandlerClass]
    public class CEquipSystem : Singleton<CEquipSystem>
    {
        private bool bEditEquip;
        public const int c_2ndEquipMaxCount = 3;
        public const int c_3rdEquipMaxCountPer2ndEquip = 2;
        public const int c_backEquipMaxCount = 20;
        private const int c_equipJudgeMaxCount = 12;
        public const int c_equipLevelMaxCount = 3;
        private const int c_judgeMarksCount = 5;
        private const int c_maxEquipCntPerLevel = 12;
        private float c_moveAnimaTime = 0.1f;
        public const int c_recommendEquipMaxCount = 6;
        private uint m_backEquipCount;
        private ushort[] m_backEquipIDs = new ushort[20];
        private bool m_bOwnHeroFlag = true;
        private enEquipUsage m_curEquipUsage = enEquipUsage.PhyAttack;
        private enHeroJobType m_curHeroJob;
        private CUIFormScript m_customEquipForm;
        private uint m_defaultCombinationID = 1;
        private Dictionary<long, ResRecommendEquipInBattle> m_defaultRecommendEquipsDictionary;
        private ushort[] m_editCustomRecommendEquips = new ushort[6];
        private uint m_editHeroID;
        private CUIFormScript m_equipJudgeForm;
        private List<ushort>[][] m_equipList;
        private Dictionary<uint, stEquipRankInfo> m_equipRankItemDetail = new Dictionary<uint, stEquipRankInfo>();
        private CEquipRelationPath m_equipRelationPath;
        private stEquipTree m_equipTree;
        private enGodEquipTab m_godEquipCurTab;
        private ListView<ResHeroCfgInfo> m_heroList = new ListView<ResHeroCfgInfo>();
        private static int m_judgeGodeIndex = -1;
        private uint m_reqRankHeroId;
        private bool m_revertDefaultEquip;
        private CEquipInfo m_selectedEquipInfoInEquipTree;
        private Transform m_selectedEquipItemInEquipTree;
        private CEquipInfo m_selEquipInfo;
        private Transform m_selEquipItemObj;
        private float m_uiCustomEquipContentHeight;
        private float m_uiEquipItemContentDefaultHeight;
        private float m_uiEquipItemContentHightDiff = 70f;
        private float m_uiEquipItemHeight;
        private bool m_useGodEquip;
        private static string s_ChooseHeroPath = "UGUI/Form/System/CustomRecommendEquip/Form_ChooseHero.prefab";
        public static string s_CustomRecommendEquipPath = "UGUI/Form/System/CustomRecommendEquip/Form_CustomRecommendEquip.prefab";
        private static Dictionary<ushort, CEquipInfo> s_equipInfoDictionary;
        private static string s_EquipJudgePath = "UGUI/Form/System/CustomRecommendEquip/Form_EquipJudge.prefab";
        private static string s_EquipListNodePrefabPath = "UGUI/Form/System/CustomRecommendEquip/Panel_EquipList.prefab";
        private static string s_EquipTreePath = "UGUI/Form/System/CustomRecommendEquip/Form_EquipTree.prefab";
        private static int s_equipUsageAmount = Enum.GetValues(typeof(enEquipUsage)).Length;
        private static string s_GodEquipPath = "UGUI/Form/System/CustomRecommendEquip/Form_GodEquip.prefab";

        private void ClearCurSelectEquipItem()
        {
            this.m_equipRelationPath.Reset();
            if (this.m_selEquipItemObj != null)
            {
                this.SetEquipItemSelectFlag(this.m_selEquipItemObj, false);
                this.m_selEquipItemObj = null;
            }
            this.m_selEquipInfo = null;
        }

        private void ClearEquipList()
        {
            for (int i = 0; i < s_equipUsageAmount; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    this.m_equipList[i][j].Clear();
                }
            }
        }

        public void CloseRightPanel()
        {
            if (null != this.m_customEquipForm)
            {
                this.m_customEquipForm.GetWidget(2).CustomSetActive(false);
                this.m_customEquipForm.GetWidget(3).CustomSetActive(false);
                this.m_customEquipForm.GetWidget(4).CustomSetActive(false);
            }
        }

        private void DefaultRecommendEquipsInVisitor(ResRecommendEquipInBattle resRecommendEquipInBattle)
        {
            long doubleKey = GameDataMgr.GetDoubleKey(resRecommendEquipInBattle.wHeroID, resRecommendEquipInBattle.wCombinationID);
            if (this.m_defaultRecommendEquipsDictionary.ContainsKey(doubleKey))
            {
                this.m_defaultRecommendEquipsDictionary.Remove(doubleKey);
            }
            if (resRecommendEquipInBattle.RecommendEquipID.Length == 6)
            {
                ResRecommendEquipInBattle battle = null;
                for (int i = 0; i < 6; i++)
                {
                    CEquipInfo info = null;
                    if ((resRecommendEquipInBattle.RecommendEquipID[i] == 0) || (s_equipInfoDictionary.TryGetValue(resRecommendEquipInBattle.RecommendEquipID[i], out info) && (info.m_resEquipInBattle.bInvalid <= 0)))
                    {
                        battle = resRecommendEquipInBattle;
                    }
                    else
                    {
                        Debug.LogError(string.Concat(new object[] { "Gao Mao a! tuijian zhuangbei limian tian le ge bu ke yong de zhuagnbei!!! HeroID = ", resRecommendEquipInBattle.wHeroID, ", CombineID = ", resRecommendEquipInBattle.wCombinationID, ", equipID = ", resRecommendEquipInBattle.RecommendEquipID[i] }));
                    }
                }
                this.m_defaultRecommendEquipsDictionary.Add(doubleKey, battle);
            }
            else
            {
                Debug.LogError(string.Concat(new object[] { "Gao Mao a! tuijian zhuangbei de shuliang dou meiyou tian dui!!! HeroID = ", resRecommendEquipInBattle.wHeroID, ", CombineID = ", resRecommendEquipInBattle.wCombinationID }));
            }
        }

        private void EditCustomRecommendEquipByGodEquip(int cnt, ref uint[] equipList)
        {
            if ((equipList != null) && (cnt <= equipList.Length))
            {
                for (int i = 0; i < 6; i++)
                {
                    this.m_editCustomRecommendEquips[i] = 0;
                }
                for (int j = 0; (j < cnt) && (j < 6); j++)
                {
                    this.m_editCustomRecommendEquips[j] = (ushort) equipList[j];
                }
            }
        }

        private void EquipInBattleInVisitor(ResEquipInBattle resEquipInBattle)
        {
            if (s_equipInfoDictionary.ContainsKey(resEquipInBattle.wID))
            {
                s_equipInfoDictionary.Remove(resEquipInBattle.wID);
            }
            s_equipInfoDictionary.Add(resEquipInBattle.wID, new CEquipInfo(resEquipInBattle.wID));
            if ((((resEquipInBattle.bUsage >= 0) && (resEquipInBattle.bUsage < s_equipUsageAmount)) && ((resEquipInBattle.bLevel >= 1) && (resEquipInBattle.bLevel <= 3))) && (resEquipInBattle.bInvalid == 0))
            {
                this.m_equipList[resEquipInBattle.bUsage][resEquipInBattle.bLevel - 1].Add(resEquipInBattle.wID);
            }
        }

        public ResRecommendEquipInBattle GetDefaultRecommendEquipInfo(uint heroID, uint combinationID)
        {
            ResRecommendEquipInBattle battle = null;
            long doubleKey = GameDataMgr.GetDoubleKey(heroID, combinationID);
            if (this.m_defaultRecommendEquipsDictionary.TryGetValue(doubleKey, out battle))
            {
                return battle;
            }
            return null;
        }

        public static CEquipInfo GetEquipInfo(ushort equipID)
        {
            CEquipInfo info = null;
            if (s_equipInfoDictionary.TryGetValue(equipID, out info))
            {
                return info;
            }
            return null;
        }

        public Dictionary<ushort, CEquipInfo> GetEquipInfoDictionary()
        {
            return s_equipInfoDictionary;
        }

        public List<ushort>[][] GetEquipList()
        {
            return this.m_equipList;
        }

        private GameObject GetEquipListNodeWidget(enEquipListNodeWidget widgetId)
        {
            if (this.m_customEquipForm != null)
            {
                GameObject widget = this.m_customEquipForm.GetWidget(5);
                if (widget != null)
                {
                    Transform transform = widget.get_transform().Find("Panel_EquipList");
                    if (transform != null)
                    {
                        CUIComponent component = transform.GetComponent<CUIComponent>();
                        if (component != null)
                        {
                            return component.GetWidget((int) widgetId);
                        }
                    }
                }
            }
            return null;
        }

        private void GetEquipTree(ushort equipID, ref stEquipTree equipTree)
        {
            equipTree.Create(equipID, s_equipInfoDictionary);
        }

        public override void Init()
        {
            this.m_equipList = new List<ushort>[s_equipUsageAmount][];
            for (int i = 0; i < s_equipUsageAmount; i++)
            {
                this.m_equipList[i] = new List<ushort>[3];
                for (int j = 0; j < 3; j++)
                {
                    this.m_equipList[i][j] = new List<ushort>();
                }
            }
            this.m_equipTree = new stEquipTree(3, 2, 20);
            this.m_equipRelationPath = new CEquipRelationPath();
            if (s_equipInfoDictionary == null)
            {
                s_equipInfoDictionary = new Dictionary<ushort, CEquipInfo>();
            }
            this.m_defaultRecommendEquipsDictionary = new Dictionary<long, ResRecommendEquipInBattle>();
            GameDataMgr.m_equipInBattleDatabin.Accept(new Action<ResEquipInBattle>(this.EquipInBattleInVisitor));
            this.InitializeBackEquipListForEachEquip();
            GameDataMgr.m_recommendEquipInBattleDatabin.Accept(new Action<ResRecommendEquipInBattle>(this.DefaultRecommendEquipsInVisitor));
            this.InitUIEventListener();
        }

        private void InitEquipItemHorizontalLine(Transform equipPanel, int level)
        {
            if (null != equipPanel)
            {
                Transform transform = null;
                Transform transform2 = null;
                Transform transform3 = null;
                int index = 0;
                for (int i = 0; i < 12; i++)
                {
                    transform = equipPanel.Find(string.Format("equipItem{0}", i));
                    transform2 = transform.Find("imgLineFront");
                    if (level > 1)
                    {
                        index = (level <= 2) ? 0 : 1;
                        this.m_equipRelationPath.InitializeHorizontalLine(index, i, CEquipLineSet.enHorizontalLineType.Right, transform2.get_gameObject());
                    }
                    transform3 = transform.Find("imgLineBack");
                    if (level < 3)
                    {
                        index = (level >= 2) ? 1 : 0;
                        this.m_equipRelationPath.InitializeHorizontalLine(index, i, CEquipLineSet.enHorizontalLineType.Left, transform3.get_gameObject());
                    }
                }
            }
        }

        private void InitEquipPathLine()
        {
            if (this.m_customEquipForm != null)
            {
                this.m_equipRelationPath.Clear();
                GameObject equipListNodeWidget = this.GetEquipListNodeWidget(enEquipListNodeWidget.EquipVerticalLinesPanel);
                if (null != equipListNodeWidget)
                {
                    Transform transform = equipListNodeWidget.get_transform();
                    for (int i = 0; i < 2; i++)
                    {
                        for (int j = 0; j < 11; j++)
                        {
                            this.m_equipRelationPath.InitializeVerticalLine(i, j, j + 1, transform.Find(string.Format("imgLine_{0}_{1}", i, j)).get_gameObject());
                        }
                    }
                    GameObject obj3 = this.GetEquipListNodeWidget(enEquipListNodeWidget.EquipPanel_Level1);
                    if (obj3 != null)
                    {
                        this.InitEquipItemHorizontalLine(obj3.get_transform(), 1);
                    }
                    GameObject obj4 = this.GetEquipListNodeWidget(enEquipListNodeWidget.EquipPanel_Level2);
                    if (obj4 != null)
                    {
                        this.InitEquipItemHorizontalLine(obj4.get_transform(), 2);
                    }
                    GameObject obj5 = this.GetEquipListNodeWidget(enEquipListNodeWidget.EquipPanel_Level3);
                    if (obj5 != null)
                    {
                        this.InitEquipItemHorizontalLine(obj5.get_transform(), 3);
                    }
                }
            }
        }

        private void InitializeBackEquipListForEachEquip()
        {
            Dictionary<ushort, CEquipInfo>.Enumerator enumerator = s_equipInfoDictionary.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<ushort, CEquipInfo> current = enumerator.Current;
                CEquipInfo info = current.Value;
                if ((info.m_resEquipInBattle != null) && (info.m_resEquipInBattle.bInvalid == 0))
                {
                    for (int i = 0; i < info.m_resEquipInBattle.PreEquipID.Length; i++)
                    {
                        if (info.m_resEquipInBattle.PreEquipID[i] > 0)
                        {
                            CEquipInfo info2 = null;
                            if (s_equipInfoDictionary.TryGetValue(info.m_resEquipInBattle.PreEquipID[i], out info2) && (info2.m_resEquipInBattle.bInvalid == 0))
                            {
                                info2.AddBackEquipID(info.m_equipID);
                            }
                        }
                    }
                }
            }
        }

        private void InitializeEditCustomRecommendEquips(uint heroID, ref bool useCustomRecommendEquips)
        {
            for (int i = 0; i < 6; i++)
            {
                this.m_editCustomRecommendEquips[i] = 0;
            }
            this.m_editHeroID = heroID;
            if (heroID != 0)
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if ((masterRoleInfo != null) && (masterRoleInfo.m_customRecommendEquipDictionary != null))
                {
                    ushort[] recommendEquipID = null;
                    useCustomRecommendEquips = false;
                    if (masterRoleInfo.m_customRecommendEquipDictionary.TryGetValue(heroID, out recommendEquipID))
                    {
                        useCustomRecommendEquips = true;
                    }
                    else
                    {
                        useCustomRecommendEquips = false;
                        ResRecommendEquipInBattle defaultRecommendEquipInfo = this.GetDefaultRecommendEquipInfo(heroID, 1);
                        if (defaultRecommendEquipInfo != null)
                        {
                            recommendEquipID = defaultRecommendEquipInfo.RecommendEquipID;
                        }
                    }
                    if (recommendEquipID != null)
                    {
                        bool flag = false;
                        for (int j = 0; j < 6; j++)
                        {
                            if (recommendEquipID[j] == 0)
                            {
                                this.m_editCustomRecommendEquips[j] = 0;
                            }
                            else
                            {
                                CEquipInfo info2 = null;
                                if (s_equipInfoDictionary.TryGetValue(recommendEquipID[j], out info2) && (info2.m_resEquipInBattle.bInvalid <= 0))
                                {
                                    this.m_editCustomRecommendEquips[j] = recommendEquipID[j];
                                }
                                else
                                {
                                    this.m_editCustomRecommendEquips[j] = 0;
                                    if (useCustomRecommendEquips)
                                    {
                                        flag = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void InitSysGodEquipTab()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_GodEquipPath);
            if (form != null)
            {
                GameObject widget = form.GetWidget(0);
                string[] strArray = new string[] { Singleton<CTextManager>.GetInstance().GetText("CustomEquip_Tab_RecommendSystem"), Singleton<CTextManager>.GetInstance().GetText("CustomEquip_Tab_RecommendGod") };
                CUIListScript component = widget.GetComponent<CUIListScript>();
                component.SetElementAmount(strArray.Length);
                for (int i = 0; i < strArray.Length; i++)
                {
                    component.GetElemenet(i).get_transform().Find("Text").GetComponent<Text>().set_text(strArray[i]);
                }
                component.SelectElement(0, true);
            }
        }

        private void InitUIEventListener()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnCustomEquipOpen));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_FormClose, new CUIEventManager.OnUIEventHandler(this.OnCustomEquipClose));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_UsageListSelect, new CUIEventManager.OnUIEventHandler(this.OnCustomEquipListSelect));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_ItemClick, new CUIEventManager.OnUIEventHandler(this.OnEuipItemClick));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_EditItemClick, new CUIEventManager.OnUIEventHandler(this.OnCustomEditItemClick));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_ViewEquipTree, new CUIEventManager.OnUIEventHandler(this.OnViewEquipTree));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_ChangeHero, new CUIEventManager.OnUIEventHandler(this.OnChangeHero));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_ModifyEquip, new CUIEventManager.OnUIEventHandler(this.OnModifyEquip));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_ConfirmModify, new CUIEventManager.OnUIEventHandler(this.OnConfirmModifyEquip));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_CancleModify, new CUIEventManager.OnUIEventHandler(this.OnCancleModifyEquip));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_AddEquip, new CUIEventManager.OnUIEventHandler(this.OnAddEquip));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_DeleteEquip, new CUIEventManager.OnUIEventHandler(this.OnDeleteEquip));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_ShowConfirmRevertDefaultTip, new CUIEventManager.OnUIEventHandler(this.OnShowConfirmRevertDefaultTip));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_RevertDefault, new CUIEventManager.OnUIEventHandler(this.OnRevertDefaultEquip));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_Expand, new CUIEventManager.OnUIEventHandler(this.OnExpandCustomEquip));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_PackUp, new CUIEventManager.OnUIEventHandler(this.OnPackUpCustomEquip));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_HeroTypeListSelect, new CUIEventManager.OnUIEventHandler(this.OnHeroTypeListSelect));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_HeroListItemEnable, new CUIEventManager.OnUIEventHandler(this.OnHeroListElementEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_HeroListItemClick, new CUIEventManager.OnUIEventHandler(this.OnHeroListElementClick));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_OwnFlagChange, new CUIEventManager.OnUIEventHandler(this.OnHeroOwnFlagChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_ViewGodEquip, new CUIEventManager.OnUIEventHandler(this.OnViewGodEquip));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_GodEquipItemEnable, new CUIEventManager.OnUIEventHandler(this.OnGodEquipItemEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_GodEquipUseBtnClick, new CUIEventManager.OnUIEventHandler(this.OnGodEquipUseBtnClick));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_GodEquipReqTimeOut, new CUIEventManager.OnUIEventHandler(this.OnReqGodEquipTimeOut));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_BackEquipListElementEnable, new CUIEventManager.OnUIEventHandler(this.OnBackEquipListElementEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_CircleTimerUp, new CUIEventManager.OnUIEventHandler(this.OnCircleTimerUp));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_OnEquipTreeClosed, new CUIEventManager.OnUIEventHandler(this.OnEquipTreeClosed));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_OnEquipTreeItemSelected, new CUIEventManager.OnUIEventHandler(this.OnEquipTreeItemSelected));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_OnBackEquipListSelectChanged, new CUIEventManager.OnUIEventHandler(this.OnBackEquipListSelectChanged));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_OnGodEquipTabChanged, new CUIEventManager.OnUIEventHandler(this.OnGodEquipTabChanged));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_GodEquipSysUseBtnClick, new CUIEventManager.OnUIEventHandler(this.OnGodEquipSysUseBtnClick));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_IWantJudgeBtnClick, new CUIEventManager.OnUIEventHandler(this.OnIWantJudgeBtnClick));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_JudgeMarkSubmitBtnClick, new CUIEventManager.OnUIEventHandler(this.OnJudgeSubmitBtnClick));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_OpenJudgeRule, new CUIEventManager.OnUIEventHandler(this.OnJudgeRuleBtnClick));
            Singleton<EventRouter>.GetInstance().AddEventHandler<SCPKG_GET_RANKING_LIST_RSP>(EventID.CUSTOM_EQUIP_RANK_LIST_GET, new Action<SCPKG_GET_RANKING_LIST_RSP>(this.OnGetCustomEquipRankList));
        }

        private bool IsCustomEquipPanelExpanded()
        {
            if (this.m_customEquipForm != null)
            {
                GameObject widget = this.m_customEquipForm.GetWidget(11);
                if (widget != null)
                {
                    return widget.get_activeSelf();
                }
            }
            return false;
        }

        private bool IsEditCustomRecommendEquipUseDefaultSetting()
        {
            if (this.m_editHeroID == 0)
            {
                return false;
            }
            ResRecommendEquipInBattle defaultRecommendEquipInfo = this.GetDefaultRecommendEquipInfo(this.m_editHeroID, 1);
            ushort[] recommendEquipID = null;
            if (defaultRecommendEquipInfo != null)
            {
                recommendEquipID = defaultRecommendEquipInfo.RecommendEquipID;
            }
            if (recommendEquipID == null)
            {
                return false;
            }
            for (int i = 0; i < 6; i++)
            {
                if (this.m_editCustomRecommendEquips[i] != recommendEquipID[i])
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsEquipListNodeExsit()
        {
            if (this.m_customEquipForm != null)
            {
                GameObject widget = this.m_customEquipForm.GetWidget(5);
                if (widget != null)
                {
                    return (widget.get_transform().Find("Panel_EquipList") != null);
                }
            }
            return false;
        }

        private bool IsHeroCustomEquip(uint heroId, ref int cnt)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            DebugHelper.Assert(masterRoleInfo != null, "CEquipSystem IsHeroCustomEquip role is null");
            if ((masterRoleInfo == null) || (masterRoleInfo.m_customRecommendEquipDictionary == null))
            {
                return false;
            }
            ushort[] numArray = null;
            cnt = 0;
            if (!masterRoleInfo.m_customRecommendEquipDictionary.TryGetValue(heroId, out numArray))
            {
                return false;
            }
            if (numArray != null)
            {
                for (int i = 0; i < numArray.Length; i++)
                {
                    CEquipInfo equipInfo = GetEquipInfo(numArray[i]);
                    if ((equipInfo != null) && (equipInfo.m_resEquipInBattle.bInvalid == 0))
                    {
                        cnt++;
                    }
                }
            }
            return true;
        }

        private void LoadEquipListNode()
        {
            if (this.m_customEquipForm != null)
            {
                GameObject widget = this.m_customEquipForm.GetWidget(5);
                if (widget != null)
                {
                    CUICommonSystem.LoadUIPrefab(s_EquipListNodePrefabPath, "Panel_EquipList", widget, this.m_customEquipForm);
                }
            }
        }

        private void OnAddEquip(CUIEvent uiEvent)
        {
            if (this.m_selEquipInfo == null)
            {
                Singleton<CUIManager>.GetInstance().OpenTips("CustomEquip_ChooseEquipTip", true, 1.5f, null, new object[0]);
            }
            else
            {
                int tag = uiEvent.m_eventParams.tag;
                if ((tag >= 0) && (tag < this.m_editCustomRecommendEquips.Length))
                {
                    this.m_editCustomRecommendEquips[tag] = this.m_selEquipInfo.m_equipID;
                }
                this.RefreshCustomEquips(false);
            }
        }

        private void OnBackEquipListElementEnable(CUIEvent uiEvent)
        {
            if (this.m_selectedEquipInfoInEquipTree != null)
            {
                int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
                if (((srcWidgetIndexInBelongedList >= 0) && (srcWidgetIndexInBelongedList < this.m_selectedEquipInfoInEquipTree.m_backEquipIDs.Count)) && (uiEvent.m_srcWidget != null))
                {
                    CEquipInfo equipInfo = GetEquipInfo(this.m_selectedEquipInfoInEquipTree.m_backEquipIDs[srcWidgetIndexInBelongedList]);
                    if (equipInfo == null)
                    {
                        DebugHelper.Assert(equipInfo != null, "GetEquipInfo is null equipId = " + this.m_selectedEquipInfoInEquipTree.m_backEquipIDs[srcWidgetIndexInBelongedList]);
                    }
                    else
                    {
                        this.RefreshEquipItemSimpleInfo(uiEvent.m_srcWidget.get_transform(), equipInfo);
                    }
                }
            }
        }

        private void OnBackEquipListSelectChanged(CUIEvent uiEvent)
        {
            if ((this.m_selectedEquipInfoInEquipTree != null) && (this.m_selectedEquipInfoInEquipTree.m_backEquipIDs != null))
            {
                int selectedIndex = (uiEvent.m_srcWidgetScript as CUIListScript).GetSelectedIndex();
                if ((selectedIndex >= 0) && (selectedIndex < this.m_selectedEquipInfoInEquipTree.m_backEquipIDs.Count))
                {
                    CEquipInfo equipInfo = GetEquipInfo(this.m_selectedEquipInfoInEquipTree.m_backEquipIDs[selectedIndex]);
                    if (equipInfo == null)
                    {
                        DebugHelper.Assert(equipInfo != null, "GetEquipInfo is null equipId = " + this.m_selectedEquipInfoInEquipTree.m_backEquipIDs[selectedIndex]);
                    }
                    else
                    {
                        this.RefreshEquipTreeForm(uiEvent.m_srcFormScript, equipInfo);
                    }
                }
            }
        }

        private void OnCancleModifyEquip(CUIEvent uiEvent)
        {
            this.bEditEquip = false;
            this.RefreshEquipCustomPanel(false);
        }

        private void OnChangeHero(CUIEvent uiEvent)
        {
            CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(s_ChooseHeroPath, false, true);
            if (null != script)
            {
                this.m_curHeroJob = enHeroJobType.All;
                GameObject widget = script.GetWidget(0);
                if (widget != null)
                {
                    GameObject obj3 = script.GetWidget(1);
                    if (obj3 != null)
                    {
                        obj3.GetComponent<Toggle>().set_isOn(this.m_bOwnHeroFlag);
                    }
                    string text = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_All");
                    string str2 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Tank");
                    string str3 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Soldier");
                    string str4 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Assassin");
                    string str5 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Master");
                    string str6 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Archer");
                    string str7 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Aid");
                    string[] titleList = new string[] { text, str2, str3, str4, str5, str6, str7 };
                    CUICommonSystem.InitMenuPanel(widget, titleList, (int) this.m_curHeroJob, true);
                }
            }
        }

        private void OnCircleTimerUp(CUIEvent uiEvent)
        {
            this.LoadEquipListNode();
            this.OnEquipListNodeLoaded();
        }

        private void OnConfirmModifyEquip(CUIEvent uiEvent)
        {
            this.SaveEditCustomRecommendEquip();
        }

        private void OnCustomEditItemClick(CUIEvent uiEvent)
        {
            CEquipInfo equipInfo = uiEvent.m_eventParams.battleEquipPar.equipInfo;
            Transform transform = uiEvent.m_srcWidget.get_transform();
            if ((equipInfo != null) && (null != this.m_customEquipForm))
            {
                this.ClearCurSelectEquipItem();
                if (equipInfo.m_resEquipInBattle.bUsage != ((byte) this.m_curEquipUsage))
                {
                    GameObject widget = this.m_customEquipForm.GetWidget(0);
                    if (widget != null)
                    {
                        CUIListScript component = widget.GetComponent<CUIListScript>();
                        if (component != null)
                        {
                            component.SelectElement(equipInfo.m_resEquipInBattle.bUsage - 1, true);
                        }
                    }
                }
                this.m_equipRelationPath.Display(equipInfo.m_equipID, this.m_equipList[(int) this.m_curEquipUsage], s_equipInfoDictionary);
                this.m_selEquipItemObj = transform;
                this.SetEquipItemSelectFlag(this.m_selEquipItemObj, true);
                this.RefreshRightPanel(equipInfo);
            }
        }

        private void OnCustomEquipClose(CUIEvent uiEvent)
        {
            this.ClearCurSelectEquipItem();
            this.m_equipRelationPath.Clear();
            this.m_customEquipForm = null;
            this.bEditEquip = false;
            this.m_useGodEquip = false;
            this.m_revertDefaultEquip = false;
        }

        private void OnCustomEquipListSelect(CUIEvent uiEvent)
        {
            this.ClearCurSelectEquipItem();
            this.CloseRightPanel();
            CUIListScript srcWidgetScript = uiEvent.m_srcWidgetScript;
            if (srcWidgetScript != null)
            {
                this.m_curEquipUsage = (enEquipUsage) (srcWidgetScript.GetSelectedIndex() + 1);
                this.RefreshEquipListPanel(true);
            }
        }

        private void OnCustomEquipOpen(CUIEvent uiEvent)
        {
            this.m_customEquipForm = Singleton<CUIManager>.GetInstance().OpenForm(s_CustomRecommendEquipPath, false, true);
            this.m_curEquipUsage = enEquipUsage.PhyAttack;
            this.bEditEquip = false;
            this.m_useGodEquip = false;
            this.m_revertDefaultEquip = false;
            if (this.m_customEquipForm != null)
            {
                if (this.IsEquipListNodeExsit())
                {
                    this.OnEquipListNodeLoaded();
                }
                else
                {
                    GameObject obj2 = this.m_customEquipForm.GetWidget(6);
                    if (obj2 != null)
                    {
                        obj2.CustomSetActive(true);
                        CUITimerScript component = obj2.GetComponent<CUITimerScript>();
                        if (component != null)
                        {
                            component.StartTimer();
                        }
                    }
                }
                if (this.m_uiCustomEquipContentHeight == 0f)
                {
                    GameObject obj3 = this.m_customEquipForm.GetWidget(12);
                    if (obj3 != null)
                    {
                        this.m_uiCustomEquipContentHeight = (obj3.get_transform() as RectTransform).get_rect().get_height();
                    }
                }
                GameObject widget = this.m_customEquipForm.GetWidget(0);
                CTextManager instance = Singleton<CTextManager>.GetInstance();
                string[] titleList = new string[] { instance.GetText("Equip_Usage_PhyAttack"), instance.GetText("Equip_Usage_MagicAttack"), instance.GetText("Equip_Usage_Defence"), instance.GetText("Equip_Usage_Move"), instance.GetText("Equip_Usage_Jungle") };
                CUICommonSystem.InitMenuPanel(widget, titleList, ((int) this.m_curEquipUsage) - 1, true);
                this.RefreshEquipCustomPanel(true);
                GameObject obj5 = this.m_customEquipForm.GetWidget(13);
                if (obj5 != null)
                {
                    obj5.CustomSetActive(!CSysDynamicBlock.bLobbyEntryBlocked);
                }
                MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.onEnterCustomRecommendEquip, new uint[0]);
            }
            CMiShuSystem.SendUIClickToServer(enUIClickReprotID.rp_BeizhanBtn);
            Singleton<CUINewFlagSystem>.instance.SetNewFlagFormCustomEquipShow(false);
        }

        private void OnDeleteEquip(CUIEvent uiEvent)
        {
            int tag = uiEvent.m_eventParams.tag;
            if ((tag >= 0) && (tag < this.m_editCustomRecommendEquips.Length))
            {
                this.m_editCustomRecommendEquips[tag] = 0;
            }
            this.RefreshCustomEquips(false);
        }

        private void OnEquipListNodeLoaded()
        {
            if (null != this.m_customEquipForm)
            {
                GameObject widget = this.m_customEquipForm.GetWidget(6);
                if (widget != null)
                {
                    widget.CustomSetActive(false);
                }
                if (this.m_uiEquipItemHeight == 0f)
                {
                    GameObject equipListNodeWidget = this.GetEquipListNodeWidget(enEquipListNodeWidget.EquipItem);
                    if (equipListNodeWidget != null)
                    {
                        this.m_uiEquipItemHeight = (equipListNodeWidget.get_transform() as RectTransform).get_rect().get_height();
                    }
                }
                if (this.m_uiEquipItemContentDefaultHeight == 0f)
                {
                    GameObject obj4 = this.GetEquipListNodeWidget(enEquipListNodeWidget.EquipItemContent);
                    if (obj4 != null)
                    {
                        this.m_uiEquipItemContentDefaultHeight = (obj4.get_transform() as RectTransform).get_rect().get_height();
                    }
                }
                if (this.m_uiCustomEquipContentHeight == 0f)
                {
                    GameObject obj5 = this.m_customEquipForm.GetWidget(12);
                    if (obj5 != null)
                    {
                        this.m_uiCustomEquipContentHeight = (obj5.get_transform() as RectTransform).get_rect().get_height();
                    }
                }
                this.InitEquipPathLine();
                this.RefreshEquipListPanel(true);
            }
        }

        private void OnEquipTreeClosed(CUIEvent uiEvent)
        {
            if (this.m_selectedEquipItemInEquipTree != null)
            {
                this.SetItemSelectedInEquipTree(this.m_selectedEquipItemInEquipTree, false);
                this.m_selectedEquipItemInEquipTree = null;
            }
            this.m_selectedEquipInfoInEquipTree = null;
        }

        private void OnEquipTreeItemSelected(CUIEvent uiEvent)
        {
            this.SelectItemInEquipTree(uiEvent.m_srcFormScript, uiEvent.m_srcWidget.get_transform(), uiEvent.m_eventParams.battleEquipPar.equipInfo);
        }

        private void OnEuipItemClick(CUIEvent uiEvent)
        {
            if (null != this.m_customEquipForm)
            {
                this.ClearCurSelectEquipItem();
                this.m_selEquipInfo = uiEvent.m_eventParams.battleEquipPar.equipInfo;
                this.m_selEquipItemObj = uiEvent.m_srcWidget.get_transform();
                if ((this.m_selEquipInfo != null) && (this.m_selEquipItemObj != null))
                {
                    this.SetEquipItemSelectFlag(this.m_selEquipItemObj, true);
                    this.RefreshRightPanel(this.m_selEquipInfo);
                    this.m_equipRelationPath.Display(this.m_selEquipInfo.m_equipID, this.m_equipList[(int) this.m_curEquipUsage], s_equipInfoDictionary);
                }
            }
        }

        private void OnExpandCustomEquip(CUIEvent uiEvent)
        {
            if (null != this.m_customEquipForm)
            {
                GameObject widget = this.m_customEquipForm.GetWidget(12);
                GameObject obj3 = this.m_customEquipForm.GetWidget(11);
                GameObject obj4 = this.m_customEquipForm.GetWidget(14);
                if (((widget != null) && (obj3 != null)) && (obj4 != null))
                {
                    <OnExpandCustomEquip>c__AnonStorey4B storeyb = new <OnExpandCustomEquip>c__AnonStorey4B();
                    obj3.CustomSetActive(true);
                    obj4.CustomSetActive(false);
                    LeanTween.cancel(widget);
                    storeyb.customContentRectTransform = widget.get_transform() as RectTransform;
                    Vector2 to = new Vector2(storeyb.customContentRectTransform.get_anchoredPosition().x, storeyb.customContentRectTransform.get_anchoredPosition().y + this.m_uiCustomEquipContentHeight);
                    LeanTween.value(widget, new Action<Vector2>(storeyb.<>m__35), storeyb.customContentRectTransform.get_anchoredPosition(), to, this.c_moveAnimaTime);
                }
                GameObject equipListNodeWidget = this.GetEquipListNodeWidget(enEquipListNodeWidget.EquipItemContent);
                if (equipListNodeWidget != null)
                {
                    RectTransform transform = equipListNodeWidget.get_transform() as RectTransform;
                    float x = transform.get_offsetMin().x;
                    float num2 = transform.get_offsetMin().y - this.m_uiEquipItemContentHightDiff;
                    transform.set_offsetMin(new Vector2(x, num2));
                }
            }
        }

        private void OnGetCustomEquipRankList(SCPKG_GET_RANKING_LIST_RSP rankListRsp)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            CSDT_RANKING_LIST_SUCC stOfSucc = rankListRsp.stRankingListDetail.stOfSucc;
            if (stOfSucc.bNumberType == 0x16)
            {
                stEquipRankInfo info = new stEquipRankInfo();
                info.equipRankItemCnt = (int) stOfSucc.dwItemNum;
                info.rankDetail = stOfSucc.astItemDetail;
                if (this.m_equipRankItemDetail.ContainsKey(this.m_reqRankHeroId))
                {
                    this.m_equipRankItemDetail[this.m_reqRankHeroId] = info;
                }
                else
                {
                    this.m_equipRankItemDetail.Add(this.m_reqRankHeroId, info);
                }
                this.RefreshGodEquipForm(this.m_reqRankHeroId);
            }
        }

        private void OnGodEquipItemEnable(CUIEvent uiEvent)
        {
            stEquipRankInfo info;
            if (this.m_equipRankItemDetail.TryGetValue(this.m_reqRankHeroId, out info))
            {
                int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
                if (((srcWidgetIndexInBelongedList >= 0) && (srcWidgetIndexInBelongedList < info.equipRankItemCnt)) && (info.rankDetail != null))
                {
                    CSDT_RANKING_LIST_ITEM_INFO csdt_ranking_list_item_info = info.rankDetail[srcWidgetIndexInBelongedList];
                    if ((csdt_ranking_list_item_info != null) && (null != uiEvent.m_srcWidget))
                    {
                        Transform transform = uiEvent.m_srcWidget.get_transform();
                        Transform transform2 = transform.Find("Bg");
                        Transform transform3 = transform.Find("BgNo1");
                        Transform transform4 = transform.Find("123No");
                        Transform transform5 = transform.Find("NumText");
                        Transform transform6 = transform.Find("winCntText");
                        Transform transform7 = transform.Find("playerJudgeText");
                        Transform transform8 = transform.Find("judgeMarkText");
                        transform3.get_gameObject().CustomSetActive(false);
                        transform2.get_gameObject().CustomSetActive(srcWidgetIndexInBelongedList >= 0);
                        transform4.GetChild(0).get_gameObject().CustomSetActive(0 == srcWidgetIndexInBelongedList);
                        transform4.GetChild(1).get_gameObject().CustomSetActive(1 == srcWidgetIndexInBelongedList);
                        transform4.GetChild(2).get_gameObject().CustomSetActive(2 == srcWidgetIndexInBelongedList);
                        transform5.get_gameObject().CustomSetActive(srcWidgetIndexInBelongedList > 2);
                        transform5.GetComponent<Text>().set_text(csdt_ranking_list_item_info.dwRankNo.ToString());
                        uint num2 = (csdt_ranking_list_item_info.dwRankScore <= 0x5f5e0ff) ? csdt_ranking_list_item_info.dwRankScore : 0x5f5e0ff;
                        float num3 = ((float) csdt_ranking_list_item_info.stExtraInfo.stDetailInfo.stCustomEquip.dwWinRate) / 100f;
                        float num4 = ((float) csdt_ranking_list_item_info.stExtraInfo.stDetailInfo.stCustomEquip.dwEvalScore) / 100f;
                        byte[] szEvalID = csdt_ranking_list_item_info.stExtraInfo.stDetailInfo.stCustomEquip.szEvalID;
                        int bEvalNum = csdt_ranking_list_item_info.stExtraInfo.stDetailInfo.stCustomEquip.bEvalNum;
                        string str = string.Empty;
                        for (int i = 0; i < bEvalNum; i++)
                        {
                            ResEquipEval dataByKey = GameDataMgr.m_recommendEquipJudge.GetDataByKey((uint) szEvalID[i]);
                            str = str + dataByKey.szDesc + " ";
                        }
                        string[] args = new string[] { num2.ToString(), num3 + "%", num4.ToString() };
                        transform6.GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("CustomEquip_WinCntText", args));
                        string[] textArray2 = new string[] { str };
                        transform7.GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("JudgeEquip_PlayerJudge", textArray2));
                        string[] textArray3 = new string[] { string.Empty, num4.ToString() };
                        transform8.GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("JudgeEquip_PlayerJudgeMark", textArray3));
                        int index = 0;
                        while (index < csdt_ranking_list_item_info.stExtraInfo.stDetailInfo.stCustomEquip.bEquipNum)
                        {
                            ushort equipID = (ushort) csdt_ranking_list_item_info.stExtraInfo.stDetailInfo.stCustomEquip.EquipID[index];
                            Transform transform9 = transform.Find("equipItem" + index);
                            if (transform9 != null)
                            {
                                Transform transform10 = transform9.Find("imgIcon");
                                CEquipInfo equipInfo = GetEquipInfo(equipID);
                                if (equipInfo != null)
                                {
                                    transform10.get_gameObject().CustomSetActive(true);
                                    transform10.GetComponent<Image>().SetSprite(equipInfo.m_equipIconPath, this.m_customEquipForm, true, false, false, false);
                                    string[] textArray4 = new string[] { equipInfo.m_equipName, equipInfo.m_equipPropertyDesc };
                                    string text = Singleton<CTextManager>.GetInstance().GetText("Equip_CommonTipsFormat", textArray4);
                                    CUICommonSystem.SetCommonTipsEvent(this.m_customEquipForm, transform10.get_gameObject(), text, enUseableTipsPos.enTop);
                                }
                                else
                                {
                                    transform10.get_gameObject().CustomSetActive(false);
                                }
                            }
                            index++;
                        }
                        while (index < 6)
                        {
                            Transform transform11 = transform.Find("equipItem" + index);
                            if (transform11 != null)
                            {
                                transform11.Find("imgIcon").get_gameObject().CustomSetActive(false);
                            }
                            index++;
                        }
                        Transform transform13 = transform.Find("useButton");
                        Transform transform14 = transform.Find("judgeButton");
                        if (transform13 != null)
                        {
                            CUIEventScript component = transform13.GetComponent<CUIEventScript>();
                            if (component != null)
                            {
                                stUIEventParams eventParams = new stUIEventParams();
                                eventParams.tag = srcWidgetIndexInBelongedList;
                                component.SetUIEvent(enUIEventType.Click, enUIEventID.CustomEquip_GodEquipUseBtnClick, eventParams);
                            }
                        }
                        if (transform14 != null)
                        {
                            CUIEventScript script2 = transform14.GetComponent<CUIEventScript>();
                            if (script2 != null)
                            {
                                stUIEventParams params2 = new stUIEventParams();
                                params2.tag = srcWidgetIndexInBelongedList;
                                params2.heroId = this.m_reqRankHeroId;
                                script2.SetUIEvent(enUIEventType.Click, enUIEventID.CustomEquip_IWantJudgeBtnClick, params2);
                            }
                        }
                    }
                }
            }
        }

        private void OnGodEquipSysUseBtnClick(CUIEvent uiEvent)
        {
            this.m_defaultCombinationID = (uint) uiEvent.m_eventParams.tag;
            ResRecommendEquipInBattle defaultRecommendEquipInfo = this.GetDefaultRecommendEquipInfo(this.m_editHeroID, this.m_defaultCombinationID);
            if (defaultRecommendEquipInfo != null)
            {
                for (int i = 0; i < 6; i++)
                {
                    this.m_editCustomRecommendEquips[i] = defaultRecommendEquipInfo.RecommendEquipID[i];
                }
            }
            this.SaveEditCustomRecommendEquip();
            Singleton<CUIManager>.GetInstance().CloseForm(s_GodEquipPath);
        }

        private void OnGodEquipTabChanged(CUIEvent uiEvent)
        {
            CUIListScript component = uiEvent.m_srcWidget.GetComponent<CUIListScript>();
            if (component == null)
            {
                DebugHelper.Assert(false, "CEquipSystem.OnGodEquipTabChanged(): lst is null!!!");
            }
            else
            {
                CUIListElementScript selectedElement = component.GetSelectedElement();
                CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_GodEquipPath);
                if (form != null)
                {
                    GameObject widget = form.GetWidget(1);
                    GameObject obj3 = form.GetWidget(2);
                    this.m_godEquipCurTab = (enGodEquipTab) component.GetSelectedIndex();
                    switch (this.m_godEquipCurTab)
                    {
                        case enGodEquipTab.EN_System:
                            widget.CustomSetActive(false);
                            obj3.CustomSetActive(true);
                            break;

                        case enGodEquipTab.EN_God:
                            widget.CustomSetActive(true);
                            obj3.CustomSetActive(false);
                            return;
                    }
                }
            }
        }

        private void OnGodEquipUseBtnClick(CUIEvent uiEvent)
        {
            stEquipRankInfo info;
            if (this.m_equipRankItemDetail.TryGetValue(this.m_reqRankHeroId, out info) && (((uiEvent.m_eventParams.tag >= 0) && (uiEvent.m_eventParams.tag < info.equipRankItemCnt)) && (info.rankDetail != null)))
            {
                CSDT_RANKING_LIST_ITEM_INFO csdt_ranking_list_item_info = info.rankDetail[uiEvent.m_eventParams.tag];
                this.EditCustomRecommendEquipByGodEquip(csdt_ranking_list_item_info.stExtraInfo.stDetailInfo.stCustomEquip.bEquipNum, ref csdt_ranking_list_item_info.stExtraInfo.stDetailInfo.stCustomEquip.EquipID);
                this.m_useGodEquip = true;
                this.SaveEditCustomRecommendEquip();
                Singleton<CUIManager>.GetInstance().CloseForm(s_GodEquipPath);
            }
        }

        private void OnHeroListElementClick(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.GetInstance().CloseForm(s_ChooseHeroPath);
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                this.ClearCurSelectEquipItem();
                this.CloseRightPanel();
                masterRoleInfo.m_customRecommendEquipsLastChangedHeroID = uiEvent.m_eventParams.heroId;
                this.RefreshEquipCustomPanel(true);
            }
        }

        private void OnHeroListElementEnable(CUIEvent uiEvent)
        {
            int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
            if ((srcWidgetIndexInBelongedList >= 0) && (srcWidgetIndexInBelongedList < this.m_heroList.Count))
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                DebugHelper.Assert(masterRoleInfo != null, "CEquipSystem OnHeroListElementEnable role is null");
                if ((masterRoleInfo != null) && (masterRoleInfo.m_customRecommendEquipDictionary != null))
                {
                    ResHeroCfgInfo info2 = this.m_heroList[srcWidgetIndexInBelongedList];
                    if ((info2 != null) && (uiEvent.m_srcWidget != null))
                    {
                        Transform transform = uiEvent.m_srcWidget.get_transform().Find("heroItemCell");
                        if (transform != null)
                        {
                            CUICommonSystem.SetHeroItemImage(uiEvent.m_srcFormScript, transform.get_gameObject(), info2.szImagePath, enHeroHeadType.enIcon, !masterRoleInfo.IsHaveHero(info2.dwCfgID, false), false);
                            CUIEventScript component = transform.GetComponent<CUIEventScript>();
                            if (component != null)
                            {
                                stUIEventParams eventParams = new stUIEventParams();
                                eventParams.heroId = info2.dwCfgID;
                                component.SetUIEvent(enUIEventType.Click, enUIEventID.CustomEquip_HeroListItemClick, eventParams);
                            }
                            Transform transform2 = transform.Find("equipedPanel");
                            if (transform2 != null)
                            {
                                int cnt = 0;
                                bool bActive = this.IsHeroCustomEquip(info2.dwCfgID, ref cnt);
                                transform2.get_gameObject().CustomSetActive(bActive);
                                if (bActive)
                                {
                                    Text text = transform2.Find("Text").GetComponent<Text>();
                                    if (cnt < 6)
                                    {
                                        string[] args = new string[] { cnt.ToString(), 6.ToString() };
                                        text.set_text(Singleton<CTextManager>.GetInstance().GetText("CustomEquip_EquipCnt", args));
                                    }
                                    else
                                    {
                                        text.set_text(Singleton<CTextManager>.GetInstance().GetText("CustomEquip_EquipComplete"));
                                    }
                                }
                            }
                            Transform transform3 = transform.Find("TxtFree");
                            if (transform3 != null)
                            {
                                transform3.get_gameObject().CustomSetActive(masterRoleInfo.IsFreeHero(info2.dwCfgID));
                            }
                        }
                    }
                }
            }
        }

        private void OnHeroOwnFlagChange(CUIEvent uiEvent)
        {
            this.RefreshHeroListPanel();
        }

        private void OnHeroTypeListSelect(CUIEvent uiEvent)
        {
            CUIListScript srcWidgetScript = uiEvent.m_srcWidgetScript;
            if (srcWidgetScript != null)
            {
                this.m_curHeroJob = (enHeroJobType) srcWidgetScript.GetSelectedIndex();
                this.RefreshHeroListPanel();
            }
        }

        private void OnIWantJudgeBtnClick(CUIEvent uiEvent)
        {
            stEquipRankInfo info;
            m_judgeGodeIndex = uiEvent.m_eventParams.tag;
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x14d1);
            if ((m_judgeGodeIndex >= 0) && this.m_equipRankItemDetail.TryGetValue(this.m_reqRankHeroId, out info))
            {
                CSDT_RANKING_LIST_ITEM_INFO csdt_ranking_list_item_info = info.rankDetail[m_judgeGodeIndex];
                msg.stPkgData.stGetEquipEvalReq.dwHeroId = this.m_reqRankHeroId;
                uint[] equipID = csdt_ranking_list_item_info.stExtraInfo.stDetailInfo.stCustomEquip.EquipID;
                for (int i = 0; i < csdt_ranking_list_item_info.stExtraInfo.stDetailInfo.stCustomEquip.bEquipNum; i++)
                {
                    msg.stPkgData.stGetEquipEvalReq.HeroEquipList[i] = equipID[i];
                }
            }
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        private void OnJudgeRuleBtnClick(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.GetInstance().OpenInfoForm(20);
        }

        private void OnJudgeSubmitBtnClick(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_EquipJudgePath);
            if (form != null)
            {
                CUIToggleListScript component = form.GetWidget(1).GetComponent<CUIToggleListScript>();
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x14cf);
                bool[] multiSelected = component.GetMultiSelected();
                int num = 0;
                for (int i = 0; i < multiSelected.Length; i++)
                {
                    if (multiSelected[i])
                    {
                        num++;
                    }
                }
                uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x123).dwConfValue;
                if (num > dwConfValue)
                {
                    string[] args = new string[] { "1", 5.ToString() };
                    Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("CustomEquip_JudgeTooMany", args), false, 1.5f, null, new object[0]);
                }
                else
                {
                    stEquipRankInfo info;
                    if (this.m_equipRankItemDetail.TryGetValue(this.m_reqRankHeroId, out info))
                    {
                        CSDT_RANKING_LIST_ITEM_INFO csdt_ranking_list_item_info = info.rankDetail[m_judgeGodeIndex];
                        uint[] equipID = csdt_ranking_list_item_info.stExtraInfo.stDetailInfo.stCustomEquip.EquipID;
                        for (int j = 0; j < csdt_ranking_list_item_info.stExtraInfo.stDetailInfo.stCustomEquip.bEquipNum; j++)
                        {
                            msg.stPkgData.stEquipEvalReq.stHeroEquipEval.HeroEquipList[j] = equipID[j];
                        }
                    }
                    msg.stPkgData.stEquipEvalReq.stHeroEquipEval.dwHeroId = this.m_editHeroID;
                    CUIListScript script3 = form.GetWidget(2).GetComponent<CUIListScript>();
                    if (script3.GetSelectedIndex() < 0)
                    {
                        Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("CustomEquip_JudgeNoMarks"), false, 1.5f, null, new object[0]);
                    }
                    else
                    {
                        int num5 = 0;
                        msg.stPkgData.stEquipEvalReq.stHeroEquipEval.stEvalInfo.bEvalNum = (byte) num;
                        for (int k = 0; k < multiSelected.Length; k++)
                        {
                            ResEquipEval dataByIndex = GameDataMgr.m_recommendEquipJudge.GetDataByIndex(k);
                            if (multiSelected[k])
                            {
                                msg.stPkgData.stEquipEvalReq.stHeroEquipEval.stEvalInfo.szEvalID[num5++] = dataByIndex.bEvalID;
                            }
                        }
                        msg.stPkgData.stEquipEvalReq.stHeroEquipEval.stEvalInfo.dwScore = (uint) (script3.GetSelectedIndex() + 1);
                        Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
                    }
                }
            }
        }

        private void OnModifyEquip(CUIEvent uiEvent)
        {
            this.bEditEquip = true;
            this.RefreshEquipCustomPanel(false);
        }

        private void OnPackUpCustomEquip(CUIEvent uiEvent)
        {
            <OnPackUpCustomEquip>c__AnonStorey4D storeyd = new <OnPackUpCustomEquip>c__AnonStorey4D();
            if (null != this.m_customEquipForm)
            {
                GameObject widget = this.m_customEquipForm.GetWidget(12);
                storeyd.equipCustomPanel = this.m_customEquipForm.GetWidget(11);
                storeyd.equipExpandBtn = this.m_customEquipForm.GetWidget(14);
                if (((widget != null) && (storeyd.equipCustomPanel != null)) && (storeyd.equipExpandBtn != null))
                {
                    <OnPackUpCustomEquip>c__AnonStorey4C storeyc = new <OnPackUpCustomEquip>c__AnonStorey4C();
                    storeyc.<>f__ref$77 = storeyd;
                    LeanTween.cancel(widget);
                    storeyc.customContentRectTransform = widget.get_transform() as RectTransform;
                    Vector2 to = new Vector2(storeyc.customContentRectTransform.get_anchoredPosition().x, storeyc.customContentRectTransform.get_anchoredPosition().y - this.m_uiCustomEquipContentHeight);
                    LeanTween.value(widget, new Action<Vector2>(storeyc.<>m__36), storeyc.customContentRectTransform.get_anchoredPosition(), to, this.c_moveAnimaTime).setOnComplete(new Action(storeyc, (IntPtr) this.<>m__37));
                }
                GameObject equipListNodeWidget = this.GetEquipListNodeWidget(enEquipListNodeWidget.EquipItemContent);
                if (equipListNodeWidget != null)
                {
                    RectTransform transform = equipListNodeWidget.get_transform() as RectTransform;
                    float x = transform.get_offsetMin().x;
                    float num2 = transform.get_offsetMin().y + this.m_uiEquipItemContentHightDiff;
                    transform.set_offsetMin(new Vector2(x, num2));
                }
            }
        }

        private void OnReqGodEquipTimeOut(CUIEvent uiEvent)
        {
            this.RefreshGodEquipForm(this.m_reqRankHeroId);
        }

        private void OnRevertDefaultEquip(CUIEvent uiEvent)
        {
            this.m_revertDefaultEquip = true;
            this.RevertEditCustomRecommendEquipToDefault();
            this.SaveEditCustomRecommendEquip();
        }

        private void OnShowConfirmRevertDefaultTip(CUIEvent uiEvent)
        {
            string text = Singleton<CTextManager>.GetInstance().GetText("CustomEquip_ConfirmRevertDefaultTip");
            Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(text, enUIEventID.CustomEquip_RevertDefault, enUIEventID.None, false);
        }

        private void OnViewEquipTree(CUIEvent uiEvent)
        {
            CEquipInfo equipInfo = uiEvent.m_eventParams.battleEquipPar.equipInfo;
            if (equipInfo != null)
            {
                CUIFormScript equipTreeForm = Singleton<CUIManager>.GetInstance().OpenForm(s_EquipTreePath, false, true);
                if (equipTreeForm != null)
                {
                    this.RefreshEquipTreeForm(equipTreeForm, equipInfo);
                }
            }
        }

        private void OnViewGodEquip(CUIEvent uiEvent)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            DebugHelper.Assert(masterRoleInfo != null, "OnViewGodEquip role is null");
            if (masterRoleInfo != null)
            {
                this.m_reqRankHeroId = masterRoleInfo.m_customRecommendEquipsLastChangedHeroID;
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0xa2a);
                msg.stPkgData.stGetRankingListReq.bNumberType = 0x16;
                msg.stPkgData.stGetRankingListReq.iSubType = (int) masterRoleInfo.m_customRecommendEquipsLastChangedHeroID;
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
                Singleton<CUIManager>.GetInstance().OpenForm(s_GodEquipPath, false, true);
                this.InitSysGodEquipTab();
                Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(Singleton<CTextManager>.GetInstance().GetText("CustomEquip_ReqGodEquipTip"), 10, enUIEventID.CustomEquip_GodEquipReqTimeOut);
            }
        }

        public void OpenTipsOnSvrRsp()
        {
            if (this.m_useGodEquip)
            {
                this.m_useGodEquip = false;
                Singleton<CUIManager>.GetInstance().OpenTips("CustomEquip_UseGodEquipTip", true, 1.5f, null, new object[0]);
            }
            else if (this.m_revertDefaultEquip)
            {
                this.m_revertDefaultEquip = false;
                Singleton<CUIManager>.GetInstance().OpenTips("CustomEquip_RevertDefaultTip", true, 1.5f, null, new object[0]);
            }
            else
            {
                Singleton<CUIManager>.GetInstance().OpenTips("EquipChange_Suc", true, 1.5f, null, new object[0]);
            }
        }

        [MessageHandler(0x14d0)]
        public static void RecieveSCEquipEvalRsp(CSPkg csPkg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_EquipJudgePath);
            if (form != null)
            {
                form.Close();
            }
            if (csPkg.stPkgData.stEquipEvalRsp.bResult == 0)
            {
                Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("GuildMatch_Appoint_Or_Leader_Success"), false, 1.5f, null, new object[0]);
            }
            else
            {
                Singleton<CUIManager>.GetInstance().OpenTips(Utility.ProtErrCodeToStr(0x14d0, csPkg.stPkgData.stEquipEvalRsp.bResult), false, 1.5f, null, new object[0]);
            }
        }

        [MessageHandler(0x14d2)]
        public static void RecieveSCGetEquipEvalRsp(CSPkg csPkg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            CSDT_HERO_EQUIPEVAL stHeroEquipEval = csPkg.stPkgData.stGetEquipEvalRsp.stHeroEquipEval;
            COMDT_EQUIPEVAL_PERACNT stEvalInfo = stHeroEquipEval.stEvalInfo;
            CUIFormScript formScript = Singleton<CUIManager>.GetInstance().OpenForm(s_EquipJudgePath, false, true);
            GameObject widget = formScript.GetWidget(0);
            GameObject obj3 = formScript.GetWidget(3);
            GameObject obj4 = formScript.GetWidget(4);
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_GodEquipPath);
            if (form != null)
            {
                try
                {
                    Transform transform = form.GetWidget(1).get_transform().Find("godEquipList/ScrollRect/Content/ListElement_" + m_judgeGodeIndex).get_transform();
                    Transform transform2 = transform.Find("judgeMarkText");
                    Transform transform3 = transform.Find("playerJudgeText");
                    obj3.GetComponent<Text>().set_text(transform2.GetComponent<Text>().get_text());
                    obj4.GetComponent<Text>().set_text(transform3.GetComponent<Text>().get_text());
                }
                catch (Exception)
                {
                }
            }
            if (widget != null)
            {
                for (int j = 0; j < 6; j++)
                {
                    Transform transform4 = widget.get_transform().Find("equipItem" + j);
                    if (transform4 != null)
                    {
                        Transform transform5 = transform4.Find("imgIcon");
                        if (stHeroEquipEval.HeroEquipList[j] == 0)
                        {
                            transform5.get_gameObject().CustomSetActive(false);
                        }
                        else
                        {
                            ushort equipID = (ushort) stHeroEquipEval.HeroEquipList[j];
                            CEquipInfo equipInfo = GetEquipInfo(equipID);
                            if (equipInfo != null)
                            {
                                transform5.get_gameObject().CustomSetActive(true);
                                transform5.GetComponent<Image>().SetSprite(equipInfo.m_equipIconPath, formScript, true, false, false, false);
                                string[] args = new string[] { equipInfo.m_equipName, equipInfo.m_equipPropertyDesc };
                                string text = Singleton<CTextManager>.GetInstance().GetText("Equip_CommonTipsFormat", args);
                                CUICommonSystem.SetCommonTipsEvent(formScript, transform5.get_gameObject(), text, enUseableTipsPos.enTop);
                            }
                            else
                            {
                                transform5.get_gameObject().CustomSetActive(false);
                            }
                        }
                    }
                }
            }
            GameObject obj6 = formScript.GetWidget(1);
            if (obj6 != null)
            {
                CUIToggleListScript script3 = obj6.get_transform().GetComponent<CUIToggleListScript>();
                int amount = Math.Min(GameDataMgr.m_recommendEquipJudge.Count(), 12);
                script3.SetElementAmount(amount);
                for (int k = 0; k < amount; k++)
                {
                    CUIToggleListElementScript elemenet = script3.GetElemenet(k);
                    ResEquipEval dataByIndex = GameDataMgr.m_recommendEquipJudge.GetDataByIndex(k);
                    if (elemenet != null)
                    {
                        CUICommonSystem.SetTextContent(elemenet.get_gameObject().get_transform().Find("JudgeNameTxt").get_gameObject(), dataByIndex.szDesc);
                    }
                    for (int m = 0; m < stEvalInfo.bEvalNum; m++)
                    {
                        if (dataByIndex.bEvalID == stEvalInfo.szEvalID[m])
                        {
                            script3.SetMultiSelected(k, true);
                            break;
                        }
                    }
                }
            }
            CUIListScript component = formScript.GetWidget(2).GetComponent<CUIListScript>();
            component.SetElementAmount(5);
            for (int i = 0; i < 5; i++)
            {
                string[] textArray2 = new string[] { (i + 1).ToString() };
                component.GetElemenet(i).get_transform().Find("Text").GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("CustomEquip_JudgeMarks", textArray2));
            }
            component.SelectElement(((int) stHeroEquipEval.stEvalInfo.dwScore) - 1, true);
        }

        [MessageHandler(0x1454)]
        public static void RecieveSCRecoverSystemEquipRsp(CSPkg csPkg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            if (csPkg.stPkgData.stRecoverSystemEquipChgRsp.bResult == 0)
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if (masterRoleInfo != null)
                {
                    uint dwHeroId = csPkg.stPkgData.stRecoverSystemEquipChgRsp.dwHeroId;
                    masterRoleInfo.m_customRecommendEquipsLastChangedHeroID = dwHeroId;
                    masterRoleInfo.m_customRecommendEquipDictionary.Remove(dwHeroId);
                    Singleton<CEquipSystem>.GetInstance().RefreshEquipCustomPanel(true);
                    Singleton<CEquipSystem>.GetInstance().OpenTipsOnSvrRsp();
                }
            }
        }

        [MessageHandler(0x1452)]
        public static void RecieveSCSelfDefineHeroEquipChgRsp(CSPkg csPkg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            if (csPkg.stPkgData.stSelfDefineHeroEquipChgRsp.bResult == 0)
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if (masterRoleInfo != null)
                {
                    uint dwHeroId = csPkg.stPkgData.stSelfDefineHeroEquipChgRsp.stHeroEquipChgInfo.dwHeroId;
                    masterRoleInfo.m_customRecommendEquipsLastChangedHeroID = dwHeroId;
                    ushort[] numArray = null;
                    if (masterRoleInfo.m_customRecommendEquipDictionary.TryGetValue(dwHeroId, out numArray))
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            numArray[i] = (ushort) csPkg.stPkgData.stSelfDefineHeroEquipChgRsp.stHeroEquipChgInfo.HeroEquipList[i];
                        }
                    }
                    else
                    {
                        numArray = new ushort[6];
                        for (int j = 0; j < 6; j++)
                        {
                            numArray[j] = (ushort) csPkg.stPkgData.stSelfDefineHeroEquipChgRsp.stHeroEquipChgInfo.HeroEquipList[j];
                        }
                        masterRoleInfo.m_customRecommendEquipDictionary.Add(dwHeroId, numArray);
                    }
                    Singleton<CEquipSystem>.GetInstance().RefreshEquipCustomPanel(true);
                    Singleton<CEquipSystem>.GetInstance().OpenTipsOnSvrRsp();
                }
            }
        }

        private void RefreshCustomEquipHero()
        {
            if (null != this.m_customEquipForm)
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                DebugHelper.Assert(masterRoleInfo != null, "RefreshEquipCustomPanel role is null");
                if (masterRoleInfo != null)
                {
                    GameObject widget = this.m_customEquipForm.GetWidget(7);
                    ResHeroCfgInfo dataByKey = null;
                    if (masterRoleInfo.m_customRecommendEquipsLastChangedHeroID == 0)
                    {
                        dataByKey = GameDataMgr.heroDatabin.GetDataByKey(masterRoleInfo.GetFirstHeroId());
                    }
                    else
                    {
                        dataByKey = GameDataMgr.heroDatabin.GetDataByKey(masterRoleInfo.m_customRecommendEquipsLastChangedHeroID);
                    }
                    if (dataByKey != null)
                    {
                        masterRoleInfo.m_customRecommendEquipsLastChangedHeroID = dataByKey.dwCfgID;
                        CUICommonSystem.SetHeroItemImage(this.m_customEquipForm, widget.get_gameObject(), dataByKey.szImagePath, enHeroHeadType.enIcon, false, false);
                    }
                }
            }
        }

        private void RefreshCustomEquips(bool bInitEditEquips)
        {
            if (null != this.m_customEquipForm)
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if (masterRoleInfo != null)
                {
                    bool useCustomRecommendEquips = false;
                    if (bInitEditEquips)
                    {
                        this.InitializeEditCustomRecommendEquips(masterRoleInfo.m_customRecommendEquipsLastChangedHeroID, ref useCustomRecommendEquips);
                    }
                    GameObject widget = this.m_customEquipForm.GetWidget(1);
                    for (int i = 0; i < this.m_editCustomRecommendEquips.Length; i++)
                    {
                        Transform equipItem = widget.get_transform().Find("equipItem" + i);
                        if (equipItem != null)
                        {
                            Transform transform2 = equipItem.Find("addButton");
                            Transform transform3 = equipItem.Find("deleteButton");
                            Transform transform4 = equipItem.Find("imgIcon");
                            CEquipInfo equipInfo = GetEquipInfo(this.m_editCustomRecommendEquips[i]);
                            Transform transform5 = equipItem.FindChild("imgActiveEquip");
                            Transform transform6 = equipItem.FindChild("imgEyeEquip");
                            if (equipInfo != null)
                            {
                                transform4.get_gameObject().CustomSetActive(true);
                                this.RefreshEquipItemSimpleInfo(equipItem, equipInfo);
                                CUIMiniEventScript component = equipItem.GetComponent<CUIMiniEventScript>();
                                if (component != null)
                                {
                                    component.m_onClickEventID = enUIEventID.CustomEquip_EditItemClick;
                                    component.m_onClickEventParams.battleEquipPar.equipInfo = equipInfo;
                                }
                                transform3.get_gameObject().CustomSetActive(this.bEditEquip);
                                transform2.get_gameObject().CustomSetActive(false);
                                if (this.bEditEquip)
                                {
                                    CUIMiniEventScript script2 = transform3.GetComponent<CUIMiniEventScript>();
                                    if (script2 != null)
                                    {
                                        script2.m_onClickEventID = enUIEventID.CustomEquip_DeleteEquip;
                                        script2.m_onClickEventParams.tag = i;
                                    }
                                }
                                if ((transform6 != null) && (transform5 != null))
                                {
                                    if (equipInfo.m_resEquipInBattle.dwActiveSkillID > 0)
                                    {
                                        if (equipInfo.m_resEquipInBattle.bUsage == 6)
                                        {
                                            transform6.get_gameObject().CustomSetActive(true);
                                            transform5.get_gameObject().CustomSetActive(false);
                                        }
                                        else
                                        {
                                            transform5.get_gameObject().CustomSetActive(true);
                                            transform6.get_gameObject().CustomSetActive(false);
                                        }
                                    }
                                    else
                                    {
                                        transform5.get_gameObject().CustomSetActive(false);
                                        transform6.get_gameObject().CustomSetActive(false);
                                    }
                                }
                            }
                            else
                            {
                                transform4.get_gameObject().CustomSetActive(false);
                                transform3.get_gameObject().CustomSetActive(false);
                                transform2.get_gameObject().CustomSetActive(this.bEditEquip);
                                this.SetEquipItemSelectFlag(equipItem, false);
                                if (this.bEditEquip)
                                {
                                    CUIMiniEventScript script3 = transform2.GetComponent<CUIMiniEventScript>();
                                    if (script3 != null)
                                    {
                                        script3.m_onClickEventID = enUIEventID.CustomEquip_AddEquip;
                                        script3.m_onClickEventParams.tag = i;
                                    }
                                }
                                if ((transform6 != null) && (transform5 != null))
                                {
                                    transform5.get_gameObject().CustomSetActive(false);
                                    transform6.get_gameObject().CustomSetActive(false);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void RefreshEquipBackList(Transform equipBackList, CEquipInfo equipInfo)
        {
            if (null != equipBackList)
            {
                CUIListScript component = equipBackList.GetComponent<CUIListScript>();
                if (component != null)
                {
                    component.SetElementAmount((equipInfo.m_backEquipIDs != null) ? equipInfo.m_backEquipIDs.Count : 0);
                    component.SelectElement(-1, false);
                }
            }
        }

        private void RefreshEquipCustomPanel(bool bRefreshHero)
        {
            if (null != this.m_customEquipForm)
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                DebugHelper.Assert(masterRoleInfo != null, "RefreshEquipCustomPanel role is null");
                if (masterRoleInfo != null)
                {
                    if (bRefreshHero)
                    {
                        this.RefreshCustomEquipHero();
                    }
                    this.RefreshCustomEquips(true);
                    GameObject widget = this.m_customEquipForm.GetWidget(8);
                    GameObject obj3 = this.m_customEquipForm.GetWidget(9);
                    GameObject obj4 = this.m_customEquipForm.GetWidget(10);
                    if (widget != null)
                    {
                        widget.CustomSetActive(!this.bEditEquip);
                    }
                    if (obj3 != null)
                    {
                        obj3.CustomSetActive(this.bEditEquip);
                    }
                    if (obj4 != null)
                    {
                        obj4.CustomSetActive(this.bEditEquip);
                    }
                }
            }
        }

        private void RefreshEquipItem(Transform equipItem, ushort equipID)
        {
            if (((null != equipItem) && (null != this.m_customEquipForm)) && (equipID != 0))
            {
                CEquipInfo equipInfo = GetEquipInfo(equipID);
                if (equipInfo == null)
                {
                    DebugHelper.Assert(equipInfo != null, "GetEquipInfo is null equipId = " + equipID);
                }
                else
                {
                    this.RefreshEquipItemSimpleInfo(equipItem, equipInfo);
                    ResEquipInBattle resEquipInBattle = equipInfo.m_resEquipInBattle;
                    if (resEquipInBattle != null)
                    {
                        equipItem.Find("imgIcon").GetComponent<Image>().SetSprite(equipInfo.m_equipIconPath, this.m_customEquipForm, true, false, false, false);
                        equipItem.Find("nameText").GetComponent<Text>().set_text(equipInfo.m_equipName);
                        equipItem.Find("priceText").GetComponent<Text>().set_text(((uint) resEquipInBattle.dwBuyPrice).ToString());
                        CUIMiniEventScript component = equipItem.GetComponent<CUIMiniEventScript>();
                        if (component != null)
                        {
                            component.m_onClickEventID = enUIEventID.CustomEquip_ItemClick;
                            component.m_onClickEventParams.battleEquipPar.equipInfo = equipInfo;
                        }
                        Transform transform = equipItem.FindChild("imgActiveEquip");
                        Transform transform2 = equipItem.FindChild("imgEyeEquip");
                        if ((transform2 != null) && (transform != null))
                        {
                            if (equipInfo.m_resEquipInBattle.dwActiveSkillID > 0)
                            {
                                if (equipInfo.m_resEquipInBattle.bUsage == 6)
                                {
                                    transform2.get_gameObject().CustomSetActive(true);
                                    transform.get_gameObject().CustomSetActive(false);
                                }
                                else
                                {
                                    transform.get_gameObject().CustomSetActive(true);
                                    transform2.get_gameObject().CustomSetActive(false);
                                }
                            }
                            else
                            {
                                transform.get_gameObject().CustomSetActive(false);
                                transform2.get_gameObject().CustomSetActive(false);
                            }
                        }
                    }
                }
            }
        }

        private void RefreshEquipItemSimpleInfo(Transform equipItem, CEquipInfo equipInfo)
        {
            if (((null != equipItem) && (equipInfo != null)) && (equipInfo.m_resEquipInBattle != null))
            {
                equipItem.Find("imgIcon").GetComponent<Image>().SetSprite(equipInfo.m_equipIconPath, this.m_customEquipForm, true, false, false, false);
            }
        }

        private void RefreshEquipLevelPanel(Transform equipPanel, int level)
        {
            if (null != equipPanel)
            {
                List<ushort> list = this.m_equipList[(int) this.m_curEquipUsage][level - 1];
                int count = list.Count;
                int num2 = 0;
                num2 = 0;
                while ((num2 < 12) && (num2 < count))
                {
                    Transform equipItem = equipPanel.Find(string.Format("equipItem{0}", num2));
                    this.RefreshEquipItem(equipItem, list[num2]);
                    CanvasGroup component = equipItem.GetComponent<CanvasGroup>();
                    if (component != null)
                    {
                        component.set_alpha(1f);
                        component.set_blocksRaycasts(true);
                    }
                    num2++;
                }
                while (num2 < 12)
                {
                    CanvasGroup group2 = equipPanel.Find(string.Format("equipItem{0}", num2)).GetComponent<CanvasGroup>();
                    if (group2 != null)
                    {
                        group2.set_alpha(0f);
                        group2.set_blocksRaycasts(false);
                    }
                    num2++;
                }
            }
        }

        private void RefreshEquipListPanel(bool isSwichUsage)
        {
            if (null != this.m_customEquipForm)
            {
                GameObject equipListNodeWidget = this.GetEquipListNodeWidget(enEquipListNodeWidget.EquipPanel_Level1);
                if (equipListNodeWidget != null)
                {
                    this.RefreshEquipLevelPanel(equipListNodeWidget.get_transform(), 1);
                }
                GameObject obj3 = this.GetEquipListNodeWidget(enEquipListNodeWidget.EquipPanel_Level2);
                if (obj3 != null)
                {
                    this.RefreshEquipLevelPanel(obj3.get_transform(), 2);
                }
                GameObject obj4 = this.GetEquipListNodeWidget(enEquipListNodeWidget.EquipPanel_Level3);
                if (obj4 != null)
                {
                    this.RefreshEquipLevelPanel(obj4.get_transform(), 3);
                }
                if (isSwichUsage)
                {
                    int count = 0;
                    List<ushort>[] listArray = this.m_equipList[(int) this.m_curEquipUsage];
                    for (int i = 0; i < listArray.Length; i++)
                    {
                        if (listArray[i].Count > count)
                        {
                            count = listArray[i].Count;
                        }
                    }
                    float num3 = this.m_uiEquipItemContentDefaultHeight - ((12 - count) * this.m_uiEquipItemHeight);
                    GameObject obj5 = this.GetEquipListNodeWidget(enEquipListNodeWidget.EquipItemContent);
                    if (obj5 != null)
                    {
                        RectTransform transform = obj5.get_transform() as RectTransform;
                        if (this.IsCustomEquipPanelExpanded())
                        {
                            transform.set_offsetMin(new Vector2(0f, -num3 - this.m_uiEquipItemContentHightDiff));
                        }
                        else
                        {
                            transform.set_offsetMin(new Vector2(0f, -num3));
                        }
                        transform.set_offsetMax(new Vector2(0f, 0f));
                    }
                }
            }
        }

        private void RefreshEquipTreeForm(CUIFormScript equipTreeForm, CEquipInfo equipInfo)
        {
            if ((null != equipTreeForm) && (equipInfo != null))
            {
                this.GetEquipTree(equipInfo.m_equipID, ref this.m_equipTree);
                GameObject widget = equipTreeForm.GetWidget(0);
                if (widget != null)
                {
                    this.RefreshEquipTreePanel(widget.get_transform(), ref this.m_equipTree, equipInfo);
                    Transform selectedItem = widget.get_transform().Find("rootEquipItem");
                    this.SelectItemInEquipTree(equipTreeForm, selectedItem, equipInfo);
                }
            }
        }

        private void RefreshEquipTreeItem(Transform equipItem, ushort equipID)
        {
            if (((null != equipItem) && (null != this.m_customEquipForm)) && (equipID != 0))
            {
                CEquipInfo equipInfo = GetEquipInfo(equipID);
                if (equipInfo == null)
                {
                    DebugHelper.Assert(equipInfo != null, "GetEquipInfo is null equipId = " + equipID);
                }
                else
                {
                    this.RefreshEquipItemSimpleInfo(equipItem, equipInfo);
                    if (equipInfo.m_resEquipInBattle != null)
                    {
                        CUIMiniEventScript component = equipItem.GetComponent<CUIMiniEventScript>();
                        if (component != null)
                        {
                            component.m_onClickEventID = enUIEventID.CustomEquip_OnEquipTreeItemSelected;
                            component.m_onClickEventParams.battleEquipPar.equipInfo = equipInfo;
                        }
                    }
                }
            }
        }

        private void RefreshEquipTreePanel(Transform equipTreePanel, ref stEquipTree equipTree, CEquipInfo equipInfo)
        {
            if ((null != equipTreePanel) && (equipInfo != null))
            {
                Transform equipItem = equipTreePanel.get_transform().Find("rootEquipItem");
                this.RefreshEquipTreeItem(equipItem, equipInfo.m_equipID);
                Transform lineGroupPanel = equipTreePanel.get_transform().Find("lineGroupPanel");
                this.RefreshLineGroupPanel(lineGroupPanel, 3, (int) equipTree.m_2ndEquipCount);
                Transform transform3 = equipTreePanel.get_transform().Find("preEquipGroupPanel");
                if (null != transform3)
                {
                    ushort equipID = 0;
                    for (int i = 0; i < 3; i++)
                    {
                        equipID = equipTree.m_2ndEquipIDs[i];
                        Transform transform4 = transform3.Find("preEquipGroup" + i);
                        if (transform4 != null)
                        {
                            transform4.get_gameObject().CustomSetActive(equipID > 0);
                            if (equipID > 0)
                            {
                                Transform transform5 = transform4.Find("2ndEquipItem");
                                this.RefreshEquipTreeItem(transform5, equipID);
                                lineGroupPanel = transform4.get_transform().Find("lineGroupPanel");
                                this.RefreshLineGroupPanel(lineGroupPanel, 2, (int) equipTree.m_3rdEquipCounts[i]);
                                ushort num3 = 0;
                                for (int j = 0; j < 2; j++)
                                {
                                    num3 = equipTree.m_3rdEquipIDs[i][j];
                                    Transform transform6 = transform4.Find("preEquipPanel/3rdEquipItem" + j);
                                    transform6.get_gameObject().CustomSetActive(num3 > 0);
                                    this.RefreshEquipTreeItem(transform6, num3);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void RefreshGodEquipForm(uint heroId)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_GodEquipPath);
            if (null != form)
            {
                Transform transform = form.get_transform().Find("Panel/godEquipPanel/godEquipList");
                if (null != transform)
                {
                    stEquipRankInfo info;
                    int amount = 0;
                    if (this.m_equipRankItemDetail.TryGetValue(heroId, out info))
                    {
                        amount = info.equipRankItemCnt;
                    }
                    CUIListScript component = transform.GetComponent<CUIListScript>();
                    if (component != null)
                    {
                        component.SetElementAmount(amount);
                    }
                    if (amount == 0)
                    {
                        Transform transform2 = form.get_transform().Find("Panel/godEquipPanel/info_node");
                        if (transform2 != null)
                        {
                            transform2.get_gameObject().CustomSetActive(true);
                        }
                    }
                    GameObject widget = form.GetWidget(2);
                    if (widget != null)
                    {
                        Transform transform3 = widget.get_transform();
                        if (transform3 != null)
                        {
                            Transform transform4 = transform3.Find("godEquipList");
                            if (transform4 != null)
                            {
                                component = transform4.GetComponent<CUIListScript>();
                                if ((component != null) && (component.GetElementAmount() == 0))
                                {
                                    ResRecommendEquipInBattle[] battleArray = new ResRecommendEquipInBattle[3];
                                    int num3 = 0;
                                    for (uint i = 1; i < 4; i++)
                                    {
                                        long doubleKey = GameDataMgr.GetDoubleKey(this.m_reqRankHeroId, i);
                                        if (this.m_defaultRecommendEquipsDictionary.TryGetValue(doubleKey, out battleArray[(int) ((IntPtr) (i - 1))]))
                                        {
                                            num3++;
                                        }
                                    }
                                    component.SetElementAmount(num3);
                                    int index = 0;
                                    for (int j = 0; j < 3; j++)
                                    {
                                        if (battleArray[j] != null)
                                        {
                                            CUIListElementScript elemenet = component.GetElemenet(index);
                                            if (elemenet != null)
                                            {
                                                Transform transform5 = elemenet.get_gameObject().get_transform();
                                                Transform transform6 = transform5.Find("TitleText");
                                                Transform transform7 = transform5.Find("tipsText");
                                                Transform transform8 = transform5.Find("useButton");
                                                if (transform8 != null)
                                                {
                                                    CUIEventScript script4 = transform8.GetComponent<CUIEventScript>();
                                                    if (script4 != null)
                                                    {
                                                        stUIEventParams eventParams = new stUIEventParams();
                                                        eventParams.tag = j + 1;
                                                        script4.SetUIEvent(enUIEventType.Click, enUIEventID.CustomEquip_GodEquipSysUseBtnClick, eventParams);
                                                    }
                                                }
                                                CUICommonSystem.SetTextContent(transform6.get_gameObject(), battleArray[j].szCombinationName);
                                                CUICommonSystem.SetTextContent(transform7.get_gameObject(), battleArray[j].szCombinationDesc);
                                                for (int k = 0; k < 6; k++)
                                                {
                                                    Transform transform9 = transform5.Find("equipItem" + k);
                                                    if (transform9 != null)
                                                    {
                                                        Transform transform10 = transform9.Find("imgIcon");
                                                        if (k >= battleArray[j].RecommendEquipID.Length)
                                                        {
                                                            transform10.get_gameObject().CustomSetActive(false);
                                                        }
                                                        else
                                                        {
                                                            ushort equipID = battleArray[j].RecommendEquipID[k];
                                                            CEquipInfo equipInfo = GetEquipInfo(equipID);
                                                            if (equipInfo != null)
                                                            {
                                                                transform10.get_gameObject().CustomSetActive(true);
                                                                transform10.GetComponent<Image>().SetSprite(equipInfo.m_equipIconPath, form, true, false, false, false);
                                                                string[] args = new string[] { equipInfo.m_equipName, equipInfo.m_equipPropertyDesc };
                                                                string text = Singleton<CTextManager>.GetInstance().GetText("Equip_CommonTipsFormat", args);
                                                                CUICommonSystem.SetCommonTipsEvent(form, transform10.get_gameObject(), text, enUseableTipsPos.enTop);
                                                            }
                                                            else
                                                            {
                                                                transform10.get_gameObject().CustomSetActive(false);
                                                            }
                                                        }
                                                    }
                                                }
                                                index++;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void RefreshHeroListPanel()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_ChooseHeroPath);
            if (null != form)
            {
                GameObject widget = form.GetWidget(1);
                if (widget != null)
                {
                    this.m_bOwnHeroFlag = widget.GetComponent<Toggle>().get_isOn();
                }
                this.ResetHeroList(this.m_curHeroJob, this.m_bOwnHeroFlag);
                GameObject obj3 = form.GetWidget(2);
                if (obj3 != null)
                {
                    obj3.GetComponent<CUIListScript>().SetElementAmount(this.m_heroList.Count);
                }
            }
        }

        private void RefreshLineGroupPanel(Transform lineGroupPanel, int maxLineCnt, int curLineCnt)
        {
            if (null != lineGroupPanel)
            {
                for (int i = 0; i < maxLineCnt; i++)
                {
                    Transform transform = lineGroupPanel.Find("linePanel" + i);
                    if (transform != null)
                    {
                        transform.get_gameObject().CustomSetActive((i + 1) == curLineCnt);
                    }
                }
            }
        }

        private void RefreshRightPanel(CEquipInfo equipInfo)
        {
            if ((null != this.m_customEquipForm) && (equipInfo != null))
            {
                GameObject widget = this.m_customEquipForm.GetWidget(2);
                if (widget != null)
                {
                    widget.CustomSetActive(true);
                    widget.get_transform().Find("equipNameText").GetComponent<Text>().set_text(equipInfo.m_equipName);
                    Text text2 = widget.get_transform().Find("Panel_euipProperty/equipPropertyDescText").GetComponent<Text>();
                    text2.set_text(equipInfo.m_equipPropertyDesc);
                    (text2.get_transform() as RectTransform).set_anchoredPosition(new Vector2(0f, 0f));
                }
                GameObject obj3 = this.m_customEquipForm.GetWidget(3);
                obj3.CustomSetActive(false);
                obj3.get_transform().Find("buyPriceText").GetComponent<Text>().set_text(((uint) equipInfo.m_resEquipInBattle.dwBuyPrice).ToString());
                GameObject obj4 = this.m_customEquipForm.GetWidget(4);
                obj4.CustomSetActive(true);
                CUIEventScript component = obj4.GetComponent<CUIEventScript>();
                if (component != null)
                {
                    stUIEventParams eventParams = new stUIEventParams();
                    eventParams.battleEquipPar.equipInfo = equipInfo;
                    component.SetUIEvent(enUIEventType.Click, enUIEventID.CustomEquip_ViewEquipTree, eventParams);
                }
            }
        }

        private void RefreshRightPanelInEquipTreeForm(CUIFormScript equipTreeFormScript, CEquipInfo equipInfo)
        {
            GameObject widget = equipTreeFormScript.GetWidget(2);
            if (widget != null)
            {
                widget.get_transform().Find("equipNameText").GetComponent<Text>().set_text(equipInfo.m_equipName);
                widget.get_transform().Find("equipPropertyDescText").GetComponent<Text>().set_text(equipInfo.m_equipPropertyDesc);
            }
            GameObject obj3 = equipTreeFormScript.GetWidget(3);
            if (obj3 != null)
            {
                obj3.GetComponent<Text>().set_text(((uint) equipInfo.m_resEquipInBattle.dwBuyPrice).ToString());
            }
        }

        private void ResetHeroList(enHeroJobType jobType, bool bOwn)
        {
            this.m_heroList.Clear();
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            DebugHelper.Assert(masterRoleInfo != null, "CEquipSystem ResetHeroList role is null");
            if (masterRoleInfo != null)
            {
                ListView<ResHeroCfgInfo> allHeroList = CHeroDataFactory.GetAllHeroList();
                for (int i = 0; i < allHeroList.Count; i++)
                {
                    if ((((jobType == enHeroJobType.All) || (allHeroList[i].bMainJob == ((byte) jobType))) || (allHeroList[i].bMinorJob == ((byte) jobType))) && (!bOwn || masterRoleInfo.IsHaveHero(allHeroList[i].dwCfgID, false)))
                    {
                        this.m_heroList.Add(allHeroList[i]);
                    }
                }
            }
        }

        private void RevertEditCustomRecommendEquipToDefault()
        {
            ResRecommendEquipInBattle defaultRecommendEquipInfo = this.GetDefaultRecommendEquipInfo(this.m_editHeroID, this.m_defaultCombinationID);
            ushort[] recommendEquipID = null;
            if (defaultRecommendEquipInfo != null)
            {
                recommendEquipID = defaultRecommendEquipInfo.RecommendEquipID;
            }
            if (recommendEquipID != null)
            {
                for (int i = 0; i < 6; i++)
                {
                    this.m_editCustomRecommendEquips[i] = recommendEquipID[i];
                }
            }
            else
            {
                for (int j = 0; j < 6; j++)
                {
                    this.m_editCustomRecommendEquips[j] = 0;
                }
            }
        }

        private void SaveEditCustomRecommendEquip()
        {
            if (this.m_editHeroID != 0)
            {
                this.bEditEquip = false;
                if (this.IsEditCustomRecommendEquipUseDefaultSetting())
                {
                    CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x1453);
                    msg.stPkgData.stRecoverSystemEquipChgReq.dwHeroId = this.m_editHeroID;
                    Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
                }
                else
                {
                    CSPkg pkg2 = NetworkModule.CreateDefaultCSPKG(0x1451);
                    pkg2.stPkgData.stSelfDefineHeroEquipChgReq.stHeroEquipChgInfo.dwHeroId = this.m_editHeroID;
                    for (int i = 0; i < 6; i++)
                    {
                        pkg2.stPkgData.stSelfDefineHeroEquipChgReq.stHeroEquipChgInfo.HeroEquipList[i] = this.m_editCustomRecommendEquips[i];
                    }
                    Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref pkg2, true);
                }
            }
        }

        private void SelectItemInEquipTree(CUIFormScript equipTreeFormScript, Transform selectedItem, CEquipInfo equipInfo)
        {
            if ((selectedItem != this.m_selectedEquipItemInEquipTree) || (equipInfo != this.m_selectedEquipInfoInEquipTree))
            {
                if (this.m_selectedEquipItemInEquipTree != null)
                {
                    this.SetItemSelectedInEquipTree(this.m_selectedEquipItemInEquipTree, false);
                }
                this.m_selectedEquipItemInEquipTree = selectedItem;
                this.m_selectedEquipInfoInEquipTree = equipInfo;
                if (this.m_selectedEquipItemInEquipTree != null)
                {
                    this.SetItemSelectedInEquipTree(this.m_selectedEquipItemInEquipTree, true);
                }
                this.RefreshRightPanelInEquipTreeForm(equipTreeFormScript, equipInfo);
                GameObject widget = equipTreeFormScript.GetWidget(1);
                if (widget != null)
                {
                    this.RefreshEquipBackList(widget.get_transform(), equipInfo);
                }
            }
        }

        private void SetEquipItemSelectFlag(Transform equipItemObj, bool bSelect)
        {
            if (equipItemObj != null)
            {
                Transform transform = equipItemObj.Find("selectImg");
                if (transform != null)
                {
                    transform.get_gameObject().CustomSetActive(bSelect);
                }
            }
        }

        private void SetItemSelectedInEquipTree(Transform equipItem, bool selected)
        {
            if (equipItem != null)
            {
                Transform transform = equipItem.Find("selectImg");
                if (transform != null)
                {
                    transform.get_gameObject().CustomSetActive(selected);
                }
            }
        }

        private void UinitUIEventListener()
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnCustomEquipOpen));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_FormClose, new CUIEventManager.OnUIEventHandler(this.OnCustomEquipClose));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_UsageListSelect, new CUIEventManager.OnUIEventHandler(this.OnCustomEquipListSelect));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_ItemClick, new CUIEventManager.OnUIEventHandler(this.OnEuipItemClick));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_EditItemClick, new CUIEventManager.OnUIEventHandler(this.OnCustomEditItemClick));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_ViewEquipTree, new CUIEventManager.OnUIEventHandler(this.OnViewEquipTree));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_ChangeHero, new CUIEventManager.OnUIEventHandler(this.OnChangeHero));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_ModifyEquip, new CUIEventManager.OnUIEventHandler(this.OnModifyEquip));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_ConfirmModify, new CUIEventManager.OnUIEventHandler(this.OnConfirmModifyEquip));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_CancleModify, new CUIEventManager.OnUIEventHandler(this.OnCancleModifyEquip));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_AddEquip, new CUIEventManager.OnUIEventHandler(this.OnAddEquip));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_DeleteEquip, new CUIEventManager.OnUIEventHandler(this.OnDeleteEquip));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_ShowConfirmRevertDefaultTip, new CUIEventManager.OnUIEventHandler(this.OnShowConfirmRevertDefaultTip));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_RevertDefault, new CUIEventManager.OnUIEventHandler(this.OnRevertDefaultEquip));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_Expand, new CUIEventManager.OnUIEventHandler(this.OnExpandCustomEquip));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_PackUp, new CUIEventManager.OnUIEventHandler(this.OnPackUpCustomEquip));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_ViewGodEquip, new CUIEventManager.OnUIEventHandler(this.OnViewGodEquip));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_HeroTypeListSelect, new CUIEventManager.OnUIEventHandler(this.OnHeroTypeListSelect));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_HeroListItemEnable, new CUIEventManager.OnUIEventHandler(this.OnHeroListElementEnable));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_HeroListItemClick, new CUIEventManager.OnUIEventHandler(this.OnHeroListElementClick));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_OwnFlagChange, new CUIEventManager.OnUIEventHandler(this.OnHeroOwnFlagChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_GodEquipItemEnable, new CUIEventManager.OnUIEventHandler(this.OnGodEquipItemEnable));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_GodEquipUseBtnClick, new CUIEventManager.OnUIEventHandler(this.OnGodEquipUseBtnClick));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_GodEquipReqTimeOut, new CUIEventManager.OnUIEventHandler(this.OnReqGodEquipTimeOut));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_BackEquipListElementEnable, new CUIEventManager.OnUIEventHandler(this.OnBackEquipListElementEnable));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_CircleTimerUp, new CUIEventManager.OnUIEventHandler(this.OnCircleTimerUp));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_OnEquipTreeClosed, new CUIEventManager.OnUIEventHandler(this.OnEquipTreeClosed));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_OnEquipTreeItemSelected, new CUIEventManager.OnUIEventHandler(this.OnEquipTreeItemSelected));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_OnBackEquipListSelectChanged, new CUIEventManager.OnUIEventHandler(this.OnBackEquipListSelectChanged));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_OnGodEquipTabChanged, new CUIEventManager.OnUIEventHandler(this.OnGodEquipTabChanged));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_GodEquipSysUseBtnClick, new CUIEventManager.OnUIEventHandler(this.OnGodEquipSysUseBtnClick));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_IWantJudgeBtnClick, new CUIEventManager.OnUIEventHandler(this.OnIWantJudgeBtnClick));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_JudgeMarkSubmitBtnClick, new CUIEventManager.OnUIEventHandler(this.OnJudgeSubmitBtnClick));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_OpenJudgeRule, new CUIEventManager.OnUIEventHandler(this.OnJudgeRuleBtnClick));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<SCPKG_GET_RANKING_LIST_RSP>(EventID.CUSTOM_EQUIP_RANK_LIST_GET, new Action<SCPKG_GET_RANKING_LIST_RSP>(this.OnGetCustomEquipRankList));
        }

        public override void UnInit()
        {
            this.UinitUIEventListener();
        }

        [CompilerGenerated]
        private sealed class <OnExpandCustomEquip>c__AnonStorey4B
        {
            internal RectTransform customContentRectTransform;

            internal void <>m__35(Vector2 pos)
            {
                this.customContentRectTransform.set_anchoredPosition(pos);
            }
        }

        [CompilerGenerated]
        private sealed class <OnPackUpCustomEquip>c__AnonStorey4C
        {
            internal CEquipSystem.<OnPackUpCustomEquip>c__AnonStorey4D <>f__ref$77;
            internal RectTransform customContentRectTransform;

            internal void <>m__36(Vector2 pos)
            {
                this.customContentRectTransform.set_anchoredPosition(pos);
            }

            internal void <>m__37()
            {
                this.<>f__ref$77.equipCustomPanel.CustomSetActive(false);
                this.<>f__ref$77.equipExpandBtn.CustomSetActive(true);
            }
        }

        [CompilerGenerated]
        private sealed class <OnPackUpCustomEquip>c__AnonStorey4D
        {
            internal GameObject equipCustomPanel;
            internal GameObject equipExpandBtn;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct stEquipRankInfo
        {
            public int equipRankItemCnt;
            public CSDT_RANKING_LIST_ITEM_INFO[] rankDetail;
        }
    }
}

