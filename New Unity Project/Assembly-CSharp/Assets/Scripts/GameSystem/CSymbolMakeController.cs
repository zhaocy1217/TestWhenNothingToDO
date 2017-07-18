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
    public class CSymbolMakeController : Singleton<CSymbolMakeController>
    {
        [CompilerGenerated]
        private enSymbolMakeSource <Source>k__BackingField;
        private int m_breakDetailIndex;
        private ushort m_breakLevelMask;
        private ListView<CBreakSymbolItem>[] m_breakSymbolList = new ListView<CBreakSymbolItem>[s_maxSymbolBreakLevel];
        private GameObject m_container;
        private ResSymbolInfo m_curTransformSymbol;
        private ListView<CSDT_SYMBOLOPT_INFO> m_svrBreakSymbolList = new ListView<CSDT_SYMBOLOPT_INFO>();
        private int m_symbolFilterLevel = 1;
        private enSymbolType m_symbolFilterType;
        private ListView<ResSymbolInfo> m_symbolMakeList = new ListView<ResSymbolInfo>();
        private static ListView<ResSymbolInfo> s_allSymbolCfgList = new ListView<ResSymbolInfo>();
        private static int s_breakSymbolCoinCnt = 0;
        public static int s_maxSymbolBreakLevel = 4;
        public static string s_symbolBreakDetailPath = "UGUI/Form/System/Symbol/Form_SymbolBreakDetail.prefab";
        public static string s_symbolBreakPath = "UGUI/Form/System/Symbol/Form_SymbolBreak.prefab";
        public static string s_symbolTransformPath = "UGUI/Form/System/Symbol/Form_SymbolTransform.prefab";

        private void BuySymbol(ResSymbolInfo symbol, uint count, bool needConfirm, CUIEvent uiEvent)
        {
            this.OnSendReqSymbolMake(symbol.dwID, (int) count);
        }

        private bool CheckSymbolBreak(CSymbolItem symbol, ushort breakLvlMask)
        {
            return (((symbol != null) && (symbol.m_SymbolData.wLevel < CSymbolInfo.s_maxSymbolLevel)) && ((symbol.m_stackCount > symbol.GetMaxWearCnt()) && (((((int) 1) << symbol.m_SymbolData.wLevel) & breakLvlMask) != 0)));
        }

        public void Clear()
        {
            this.ClearSymbolMakeData();
        }

        private void ClearBreakSymbolListData(int index)
        {
            if ((index >= 0) && (index < this.m_breakSymbolList.Length))
            {
                this.m_breakSymbolList[index].Clear();
            }
        }

        private void ClearSymbolMakeData()
        {
            this.m_symbolFilterLevel = 1;
            this.m_symbolFilterType = enSymbolType.All;
        }

        private int GetBreakExcessSymbolCoinCnt(ushort breakLvlMask = 0xffff)
        {
            int num = 0;
            ListView<CSymbolItem> allSymbolList = Singleton<CSymbolSystem>.GetInstance().GetAllSymbolList();
            for (int i = 0; i < allSymbolList.Count; i++)
            {
                if (this.CheckSymbolBreak(allSymbolList[i], breakLvlMask))
                {
                    num += (int) ((allSymbolList[i].m_stackCount - allSymbolList[i].GetMaxWearCnt()) * allSymbolList[i].m_SymbolData.dwBreakCoin);
                }
            }
            return num;
        }

        private int GetSelectBreakSymbolCoinCnt()
        {
            int num = 0;
            for (int i = 0; i < this.m_breakSymbolList.Length; i++)
            {
                if (this.m_breakSymbolList[i] != null)
                {
                    for (int j = 0; j < this.m_breakSymbolList[i].Count; j++)
                    {
                        if ((this.m_breakSymbolList[i][j].symbol != null) && this.m_breakSymbolList[i][j].bBreak)
                        {
                            num += (int) ((this.m_breakSymbolList[i][j].symbol.m_stackCount - this.m_breakSymbolList[i][j].symbol.GetMaxWearCnt()) * this.m_breakSymbolList[i][j].symbol.m_SymbolData.dwBreakCoin);
                        }
                    }
                }
            }
            return num;
        }

        private CSymbolItem GetSymbolByCfgID(uint cfgId)
        {
            ListView<CSymbolItem> allSymbolList = Singleton<CSymbolSystem>.GetInstance().GetAllSymbolList();
            for (int i = 0; i < allSymbolList.Count; i++)
            {
                CSymbolItem item2 = allSymbolList[i];
                if ((item2 != null) && (item2.m_baseID == cfgId))
                {
                    return item2;
                }
            }
            return null;
        }

        public override void Init()
        {
            this.InitSymbolCfgList();
            this.InitBreakSymbolList();
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolMake_TypeMenuSelect, new CUIEventManager.OnUIEventHandler(this.OnSymbolTypeMenuSelect));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolMake_LevelMenuSelect, new CUIEventManager.OnUIEventHandler(this.OnSymbolLevelMenuSelect));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolMake_ListItemEnable, new CUIEventManager.OnUIEventHandler(this.OnSymbolMakeListEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolMake_ListItemClick, new CUIEventManager.OnUIEventHandler(this.OnSymbolMakeListClick));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolMake_OnItemMakeClick, new CUIEventManager.OnUIEventHandler(this.OnMakeSymbolClick));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolMake_OnItemMakeConfirm, new CUIEventManager.OnUIEventHandler(this.OnItemMakeConfirm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolMake_OnItemBreakClick, new CUIEventManager.OnUIEventHandler(this.OnBreakSymbolClick));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolMake_OnItemBreakConfirm, new CUIEventManager.OnUIEventHandler(this.OnBreakSymbolConfirm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolMake_OnBreakExcessSymbolClick, new CUIEventManager.OnUIEventHandler(this.OnBreakExcessSymbolClick));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolMake_SelectBreakLvlItem, new CUIEventManager.OnUIEventHandler(this.OnSelectBreakLvlItem));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolMake_OnBreakExcessSymbolConfirm, new CUIEventManager.OnUIEventHandler(this.OnBreakExcessSymbolClickConfirm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolMake_ItemBreakAnimatorEnd, new CUIEventManager.OnUIEventHandler(this.OnItemBreakAnimatorEnd));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_BreakItemEnable, new CUIEventManager.OnUIEventHandler(this.OnBreakListItemEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_BreakListItemSelToggle, new CUIEventManager.OnUIEventHandler(this.OnBreakListItemSelToggle));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_OpenBreakDetailForm, new CUIEventManager.OnUIEventHandler(this.OnOpenBreakDetailForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_BreakDetailFormConfirm, new CUIEventManager.OnUIEventHandler(this.OnBreakDetailFormConfirm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_BreakDetailFormCancle, new CUIEventManager.OnUIEventHandler(this.OnBreakDetailFormCancle));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolMake_CoinNotEnoughGotoSymbolMall, new CUIEventManager.OnUIEventHandler(this.OnCoinNotEnoughGotoSymbolMall));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolMake_OnSecurePwdItemBreakConfirm, new CUIEventManager.OnUIEventHandler(this.OnSecurePwdBreakSymbolConfirm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolMake_OnSecurePwdBreakExcessSymbolConfirm, new CUIEventManager.OnUIEventHandler(this.OnSecurePwdBreakExcessSymbolConfirm));
            Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.LOTTERY_GET_NEW_SYMBOL, new Action(this, (IntPtr) this.OnGetNewSymbol));
        }

        private void InitBreakSymbolList()
        {
            for (int i = 0; i < this.m_breakSymbolList.Length; i++)
            {
                if (this.m_breakSymbolList[i] == null)
                {
                    this.m_breakSymbolList[i] = new ListView<CBreakSymbolItem>();
                }
                this.m_breakSymbolList[i].Clear();
            }
        }

        private void InitSymbolCfgList()
        {
            s_allSymbolCfgList.Clear();
            Dictionary<long, object>.Enumerator enumerator = GameDataMgr.symbolInfoDatabin.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<long, object> current = enumerator.Current;
                ResSymbolInfo item = (ResSymbolInfo) current.Value;
                if ((item != null) && (item.bIsMakeShow > 0))
                {
                    s_allSymbolCfgList.Add(item);
                }
            }
        }

        private bool IsAllSymbolBreak(int index)
        {
            if ((index >= 0) && (index < this.m_breakSymbolList.Length))
            {
                for (int i = 0; i < this.m_breakSymbolList[index].Count; i++)
                {
                    if (!this.m_breakSymbolList[index][i].bBreak)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public void Load(CUIFormScript form)
        {
            switch (this.Source)
            {
                case enSymbolMakeSource.SymbolManage:
                    CUICommonSystem.LoadUIPrefab(CSymbolSystem.s_symbolMakeModulePath, CSymbolSystem.s_symbolMakePanel, form.GetWidget(4), form);
                    break;

                case enSymbolMakeSource.Mall:
                    CUICommonSystem.LoadUIPrefab(CSymbolSystem.s_symbolMakeMallModulePath, CSymbolSystem.s_symbolMakePanel, form.GetWidget(3), form);
                    return;
            }
        }

        public bool Loaded(CUIFormScript form)
        {
            GameObject obj2 = null;
            switch (this.Source)
            {
                case enSymbolMakeSource.SymbolManage:
                    obj2 = Utility.FindChild(form.GetWidget(4), CSymbolSystem.s_symbolMakePanel);
                    break;

                case enSymbolMakeSource.Mall:
                    obj2 = Utility.FindChild(form.GetWidget(3), CSymbolSystem.s_symbolMakePanel);
                    goto Label_004E;
            }
        Label_004E:
            if (obj2 == null)
            {
                return false;
            }
            return true;
        }

        private void OnBreakDetailFormCancle(CUIEvent uiEvent)
        {
            if (((this.m_breakDetailIndex >= 0) && (this.m_breakDetailIndex < this.m_breakSymbolList.Length)) && (this.m_breakSymbolList[this.m_breakDetailIndex] != null))
            {
                for (int i = 0; i < this.m_breakSymbolList[this.m_breakDetailIndex].Count; i++)
                {
                    if (this.m_breakSymbolList[this.m_breakDetailIndex][i].bBreakToggle != this.m_breakSymbolList[this.m_breakDetailIndex][i].bBreak)
                    {
                        this.m_breakSymbolList[this.m_breakDetailIndex][i].bBreakToggle = this.m_breakSymbolList[this.m_breakDetailIndex][i].bBreak;
                    }
                }
            }
            Singleton<CUIManager>.GetInstance().CloseForm(s_symbolBreakDetailPath);
            this.RefreshSymbolBreakForm();
        }

        private void OnBreakDetailFormConfirm(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_symbolBreakDetailPath);
            if (null != form)
            {
                if (((this.m_breakDetailIndex >= 0) && (this.m_breakDetailIndex < this.m_breakSymbolList.Length)) && (this.m_breakSymbolList[this.m_breakDetailIndex] != null))
                {
                    for (int i = 0; i < this.m_breakSymbolList[this.m_breakDetailIndex].Count; i++)
                    {
                        if (this.m_breakSymbolList[this.m_breakDetailIndex][i].bBreak != this.m_breakSymbolList[this.m_breakDetailIndex][i].bBreakToggle)
                        {
                            this.m_breakSymbolList[this.m_breakDetailIndex][i].bBreak = this.m_breakSymbolList[this.m_breakDetailIndex][i].bBreakToggle;
                        }
                    }
                }
                Singleton<CUIManager>.GetInstance().CloseForm(s_symbolBreakDetailPath);
                this.RefreshSymbolBreakForm();
            }
        }

        private void OnBreakExcessSymbolClick(CUIEvent uiEvent)
        {
            CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(s_symbolBreakPath, false, true);
            if (script != null)
            {
                for (int i = 0; i < this.m_breakSymbolList.Length; i++)
                {
                    this.m_breakSymbolList[i].Clear();
                }
                this.m_breakLevelMask = 0;
                Transform transform = script.get_transform().Find("Panel_SymbolBreak/Panel_Content");
                for (int j = 0; j < s_maxSymbolBreakLevel; j++)
                {
                    GameObject obj2 = transform.Find(string.Format("breakElement{0}", j)).get_gameObject();
                    Transform transform2 = obj2.get_transform().Find("OnBreakBtn");
                    Transform transform3 = obj2.get_transform().Find("OnBreakBtn/Checkmark");
                    Transform transform4 = obj2.get_transform().Find("detailButton");
                    if ((transform2 != null) && (transform3 != null))
                    {
                        transform3.get_gameObject().CustomSetActive(false);
                        CUIEventScript component = transform2.GetComponent<CUIEventScript>();
                        if (component != null)
                        {
                            stUIEventParams eventParams = new stUIEventParams();
                            eventParams.tag = j;
                            component.SetUIEvent(enUIEventType.Click, enUIEventID.SymbolMake_SelectBreakLvlItem, eventParams);
                        }
                    }
                    if (transform4 != null)
                    {
                        transform4.get_gameObject().CustomSetActive(false);
                        CUIEventScript script3 = transform4.GetComponent<CUIEventScript>();
                        if (script3 != null)
                        {
                            stUIEventParams params2 = new stUIEventParams();
                            params2.tag = j;
                            script3.SetUIEvent(enUIEventType.Click, enUIEventID.Symbol_OpenBreakDetailForm, params2);
                        }
                    }
                }
                this.RefreshSymbolBreakForm();
            }
        }

        private void OnBreakExcessSymbolClickConfirm(CUIEvent uiEvent)
        {
            if (this.m_breakLevelMask == 0)
            {
                Singleton<CUIManager>.GetInstance().OpenTips("Symbol_Select_BreakLvl_Tip", true, 1.5f, null, new object[0]);
            }
            else
            {
                bool flag = this.SetBreakSymbolList();
                if (this.m_svrBreakSymbolList.Count > 0)
                {
                    if (flag)
                    {
                        CSecurePwdSystem.TryToValidate(enOpPurpose.BREAK_SYMBOL, enUIEventID.SymbolMake_OnSecurePwdBreakExcessSymbolConfirm, new stUIEventParams());
                    }
                    else
                    {
                        SendReqExcessSymbolBreak(this.m_svrBreakSymbolList, string.Empty);
                    }
                }
                else
                {
                    Singleton<CUIManager>.GetInstance().OpenTips("Symbol_No_More_Item_Break", true, 1.5f, null, new object[0]);
                }
            }
        }

        private void OnBreakListItemEnable(CUIEvent uiEvent)
        {
            if (((this.m_breakDetailIndex >= 0) && (this.m_breakDetailIndex < this.m_breakSymbolList.Length)) && ((this.m_breakSymbolList[this.m_breakDetailIndex] != null) && (null != uiEvent.m_srcWidget)))
            {
                int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
                if ((srcWidgetIndexInBelongedList >= 0) && (srcWidgetIndexInBelongedList < this.m_breakSymbolList[this.m_breakDetailIndex].Count))
                {
                    CBreakSymbolItem item = this.m_breakSymbolList[this.m_breakDetailIndex][srcWidgetIndexInBelongedList];
                    if ((item != null) && (item.symbol != null))
                    {
                        Transform transform = uiEvent.m_srcWidget.get_transform().Find("itemCell");
                        Singleton<CSymbolSystem>.GetInstance().m_symbolWearCtrl.SetSymbolListItem(uiEvent.m_srcFormScript, transform.get_gameObject(), item.symbol);
                        Transform transform2 = transform.Find("selectFlag");
                        if (transform2 != null)
                        {
                            CUIEventScript component = transform2.GetComponent<CUIEventScript>();
                            if (component != null)
                            {
                                stUIEventParams eventParams = new stUIEventParams();
                                eventParams.tag = srcWidgetIndexInBelongedList;
                                component.SetUIEvent(enUIEventType.Click, enUIEventID.Symbol_BreakListItemSelToggle, eventParams);
                            }
                            transform2.GetComponent<Toggle>().set_isOn(item.bBreakToggle);
                        }
                    }
                }
            }
        }

        private void OnBreakListItemSelToggle(CUIEvent uiEvent)
        {
            int tag = uiEvent.m_eventParams.tag;
            if ((tag >= 0) && (tag < this.m_breakSymbolList[this.m_breakDetailIndex].Count))
            {
                CBreakSymbolItem item = this.m_breakSymbolList[this.m_breakDetailIndex][tag];
                if (((item != null) && (item.symbol != null)) && (uiEvent.m_srcWidget != null))
                {
                    item.bBreakToggle = uiEvent.m_srcWidget.GetComponent<Toggle>().get_isOn();
                }
            }
        }

        private void OnBreakSymbolClick(CUIEvent uiEvent)
        {
            if (this.m_curTransformSymbol != null)
            {
                CSymbolItem symbolByCfgID = this.GetSymbolByCfgID(this.m_curTransformSymbol.dwID);
                if (symbolByCfgID == null)
                {
                    Singleton<CUIManager>.GetInstance().OpenTips("Symbol__Item_Not_Exist_Tip", true, 1.5f, null, new object[0]);
                }
                else if (symbolByCfgID.m_stackCount > symbolByCfgID.GetMaxWearCnt())
                {
                    string text = Singleton<CTextManager>.GetInstance().GetText("Symbol_Break_Tip");
                    Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(text, enUIEventID.SymbolMake_OnItemBreakConfirm, enUIEventID.None, false);
                }
                else
                {
                    CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                    if (masterRoleInfo != null)
                    {
                        string strContent = string.Format(Singleton<CTextManager>.GetInstance().GetText("Symbol_Break_Item_Wear_Tip"), masterRoleInfo.m_symbolInfo.GetMaxWearSymbolPageName(symbolByCfgID));
                        Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(strContent, enUIEventID.SymbolMake_OnItemBreakConfirm, enUIEventID.None, false);
                    }
                }
            }
        }

        private void OnBreakSymbolConfirm(CUIEvent uiEvent)
        {
            if (this.m_curTransformSymbol != null)
            {
                CSymbolItem symbolByCfgID = this.GetSymbolByCfgID(this.m_curTransformSymbol.dwID);
                if (symbolByCfgID != null)
                {
                    if (symbolByCfgID.m_SymbolData.bNeedPswd > 0)
                    {
                        CSecurePwdSystem.TryToValidate(enOpPurpose.BREAK_SYMBOL, enUIEventID.SymbolMake_OnSecurePwdItemBreakConfirm, new stUIEventParams());
                    }
                    else
                    {
                        this.OnSendReqSymbolBreak(symbolByCfgID.m_baseID, 1, string.Empty);
                    }
                }
            }
        }

        private void OnCoinNotEnoughGotoSymbolMall(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.GetInstance().CloseForm(s_symbolTransformPath);
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mall_GoToSymbolTab);
        }

        private void OnGetNewSymbol()
        {
            this.RefreshSymbolMakeForm();
        }

        private void OnItemBreakAnimatorEnd(CUIEvent uiEvent)
        {
            if (s_breakSymbolCoinCnt > 0)
            {
                CUseable useable = CUseableManager.CreateVirtualUseable(enVirtualItemType.enSymbolCoin, s_breakSymbolCoinCnt);
                if (useable != null)
                {
                    CUseable[] items = new CUseable[] { useable };
                    Singleton<CUIManager>.GetInstance().OpenAwardTip(items, null, false, enUIEventID.None, false, false, "Form_Award");
                }
                s_breakSymbolCoinCnt = 0;
            }
        }

        private void OnItemMakeConfirm(CUIEvent uiEvent)
        {
            if (this.m_curTransformSymbol != null)
            {
                this.OnSendReqSymbolMake(this.m_curTransformSymbol.dwID, 1);
            }
        }

        private void OnMakeSymbolClick(CUIEvent uiEvent)
        {
            if (this.m_curTransformSymbol != null)
            {
                if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().SymbolCoin >= this.m_curTransformSymbol.dwMakeCoin)
                {
                    CSymbolItem symbolByCfgID = this.GetSymbolByCfgID(this.m_curTransformSymbol.dwID);
                    if (symbolByCfgID != null)
                    {
                        if (symbolByCfgID.m_stackCount >= symbolByCfgID.m_SymbolData.iOverLimit)
                        {
                            Singleton<CUIManager>.GetInstance().OpenTips("Symbol_Make_MaxCnt_Limit", true, 1.5f, null, new object[0]);
                        }
                        else if (symbolByCfgID.m_stackCount >= CSymbolWearController.s_maxSameIDSymbolEquipNum)
                        {
                            string text = Singleton<CTextManager>.GetInstance().GetText("Symbol_Make_WearMaxLimit_Tip");
                            Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(text, enUIEventID.SymbolMake_OnItemMakeConfirm, enUIEventID.None, false);
                        }
                        else
                        {
                            this.OnSendReqSymbolMake(this.m_curTransformSymbol.dwID, 1);
                        }
                    }
                    else
                    {
                        this.OnSendReqSymbolMake(this.m_curTransformSymbol.dwID, 1);
                    }
                }
                else if (CSysDynamicBlock.bLobbyEntryBlocked)
                {
                    Singleton<CUIManager>.GetInstance().OpenTips("Symbol_DynamicBlock_Coin_Not_Enough_Tip", true, 1.5f, null, new object[0]);
                }
                else
                {
                    string strContent = Singleton<CTextManager>.GetInstance().GetText("Symbol_Coin_Not_Enough_Tip");
                    Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(strContent, enUIEventID.Symbol_Jump_To_MiShu, enUIEventID.None, false);
                }
            }
        }

        private void OnOpenBreakDetailForm(CUIEvent uiEvent)
        {
            CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(s_symbolBreakDetailPath, false, true);
            if (null != script)
            {
                int tag = uiEvent.m_eventParams.tag;
                Transform transform = script.get_transform().Find("Panel_SymbolBreak/Panel_Content/List");
                if (transform != null)
                {
                    CUIListScript component = transform.GetComponent<CUIListScript>();
                    if (((tag >= 0) && (tag < this.m_breakSymbolList.Length)) && (this.m_breakSymbolList[tag] != null))
                    {
                        this.m_breakDetailIndex = tag;
                        this.m_breakSymbolList[tag].Sort();
                        component.SetElementAmount(this.m_breakSymbolList[tag].Count);
                    }
                }
            }
        }

        private void OnReceiveBreakItemList()
        {
            for (int i = 0; i < this.m_breakSymbolList.Length; i++)
            {
                int index = 0;
                while (index < this.m_breakSymbolList[i].Count)
                {
                    if (this.m_breakSymbolList[i][index].bBreak)
                    {
                        this.m_breakSymbolList[i].RemoveAt(index);
                    }
                    else
                    {
                        index++;
                    }
                }
            }
        }

        private void OnSecurePwdBreakExcessSymbolConfirm(CUIEvent uiEvent)
        {
            if (this.m_breakLevelMask == 0)
            {
                Singleton<CUIManager>.GetInstance().OpenTips("Symbol_Select_BreakLvl_Tip", true, 1.5f, null, new object[0]);
            }
            else if (this.m_svrBreakSymbolList.Count > 0)
            {
                SendReqExcessSymbolBreak(this.m_svrBreakSymbolList, uiEvent.m_eventParams.pwd);
            }
            else
            {
                Singleton<CUIManager>.GetInstance().OpenTips("Symbol_No_More_Item_Break", true, 1.5f, null, new object[0]);
            }
        }

        private void OnSecurePwdBreakSymbolConfirm(CUIEvent uiEvent)
        {
            if (this.m_curTransformSymbol != null)
            {
                CSymbolItem symbolByCfgID = this.GetSymbolByCfgID(this.m_curTransformSymbol.dwID);
                if (symbolByCfgID != null)
                {
                    this.OnSendReqSymbolBreak(symbolByCfgID.m_baseID, 1, uiEvent.m_eventParams.pwd);
                }
            }
        }

        private void OnSelectBreakLvlItem(CUIEvent uiEvent)
        {
            int tag = uiEvent.m_eventParams.tag;
            CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(s_symbolBreakPath, false, true);
            if (script != null)
            {
                Transform transform = script.get_transform().Find(string.Format("Panel_SymbolBreak/Panel_Content/breakElement{0}", tag));
                if (transform != null)
                {
                    Transform transform2 = transform.get_transform().Find("OnBreakBtn/Checkmark");
                    Transform transform3 = transform.get_transform().Find("detailButton");
                    if (transform2 != null)
                    {
                        transform2.get_gameObject().CustomSetActive(!transform2.get_gameObject().get_activeSelf());
                        this.ClearBreakSymbolListData(tag);
                        if (transform2.get_gameObject().get_activeSelf())
                        {
                            this.SetBreakSymbolListData(tag);
                        }
                        if (transform3 != null)
                        {
                            transform3.get_gameObject().CustomSetActive(transform2.get_gameObject().get_activeSelf());
                        }
                    }
                }
                this.RefreshSymbolBreakForm();
            }
        }

        private void OnSendReqSymbolBreak(uint symbolId, int count = 1, string pwd = "")
        {
            if (Singleton<CSymbolSystem>.GetInstance().m_selectMenuType == enSymbolMenuType.SymbolRecommend)
            {
                SendReqSymbolBreak(Singleton<CSymbolSystem>.GetInstance().m_symbolRcmdCtrl.m_curHeroId, symbolId, count, pwd);
            }
            else
            {
                SendReqSymbolBreak(0, symbolId, count, pwd);
            }
        }

        private void OnSendReqSymbolMake(uint symbolId, int count = 1)
        {
            if (Singleton<CSymbolSystem>.GetInstance().m_selectMenuType == enSymbolMenuType.SymbolRecommend)
            {
                SendReqSymbolMake(Singleton<CSymbolSystem>.GetInstance().m_symbolRcmdCtrl.m_curHeroId, symbolId, count);
            }
            else
            {
                SendReqSymbolMake(0, symbolId, count);
            }
        }

        private void OnSymbolLevelMenuSelect(CUIEvent uiEvent)
        {
            CUIListScript component = uiEvent.m_srcWidget.GetComponent<CUIListScript>();
            this.m_symbolFilterLevel = component.GetSelectedIndex() + 1;
            this.SetSymbolMakeListData();
            this.RefreshSymbolMakeList();
        }

        private void OnSymbolMakeListClick(CUIEvent uiEvent)
        {
            this.m_curTransformSymbol = uiEvent.m_eventParams.symbolTransParam.symbolCfgInfo;
            switch (this.Source)
            {
                case enSymbolMakeSource.SymbolManage:
                    Singleton<CUIManager>.GetInstance().OpenForm(s_symbolTransformPath, false, true);
                    break;

                case enSymbolMakeSource.Mall:
                    CSymbolBuyPickDialog.Show(this.m_curTransformSymbol, RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_SYMBOLCOIN, 10000f, new CSymbolBuyPickDialog.OnConfirmBuyDelegate(this.BuySymbol), null, null);
                    goto Label_006B;
            }
        Label_006B:
            this.RefreshSymbolTransformForm();
        }

        private void OnSymbolMakeListEnable(CUIEvent uiEvent)
        {
            if ((uiEvent.m_srcWidgetIndexInBelongedList < 0) || (uiEvent.m_srcWidgetIndexInBelongedList >= this.m_symbolMakeList.Count))
            {
                DebugHelper.Assert(false, "OnSymbolMakeListEnable index out of range");
            }
            else
            {
                CSymbolSystem.RefreshSymbolItem(this.m_symbolMakeList[uiEvent.m_srcWidgetIndexInBelongedList], uiEvent.m_srcWidget, uiEvent.m_srcFormScript, this.Source);
            }
        }

        private void OnSymbolTypeMenuSelect(CUIEvent uiEvent)
        {
            int selectedIndex = uiEvent.m_srcWidget.GetComponent<CUIListScript>().GetSelectedIndex();
            this.m_symbolFilterType = (enSymbolType) selectedIndex;
            this.SetSymbolMakeListData();
            this.RefreshSymbolMakeList();
        }

        private void PlayBatchBreakAnimator()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_symbolBreakPath);
            if (null != form)
            {
                Transform transform = form.get_transform().Find("Panel_SymbolBreak/Panel_Content");
                for (int i = 0; i < s_maxSymbolBreakLevel; i++)
                {
                    GameObject obj2 = transform.Find(string.Format("breakElement{0}", i)).get_gameObject();
                    Transform transform2 = obj2.get_transform().Find("OnBreakBtn/Checkmark");
                    if ((transform2 != null) && transform2.get_gameObject().get_activeSelf())
                    {
                        CUICommonSystem.PlayAnimator(obj2.get_transform().Find("Img_Lv").get_gameObject(), "FenjieAnimation");
                    }
                }
                Singleton<CSoundManager>.GetInstance().PostEvent("UI_fuwen_fenjie_piliang", null);
            }
        }

        private void PlaySingleBreakAnimator()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_symbolTransformPath);
            if (null != form)
            {
                CUICommonSystem.PlayAnimator(form.get_transform().Find("Panel_SymbolTranform/Panel_Content/iconImage").get_gameObject(), "FenjieAnimation");
                Singleton<CSoundManager>.GetInstance().PostEvent("UI_fuwen_fenjie_dange", null);
            }
        }

        [MessageHandler(0x486)]
        public static void ReciveSymbolBreakRsp(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            SCPKG_CMD_SYMBOL_BREAK stSymbolBreakRsp = msg.stPkgData.stSymbolBreakRsp;
            int num = 0;
            for (int i = 0; i < stSymbolBreakRsp.wSymbolCnt; i++)
            {
                ResSymbolInfo dataByKey = GameDataMgr.symbolInfoDatabin.GetDataByKey(stSymbolBreakRsp.astSymbolList[i].dwSymbolID);
                if (dataByKey != null)
                {
                    num += (int) (dataByKey.dwBreakCoin * stSymbolBreakRsp.astSymbolList[i].iSymbolCnt);
                }
            }
            s_breakSymbolCoinCnt = num;
            if (num > 0)
            {
                if (stSymbolBreakRsp.bBreakType == 0)
                {
                    Singleton<CSymbolMakeController>.GetInstance().PlaySingleBreakAnimator();
                    Singleton<CSymbolMakeController>.GetInstance().RefreshSymbolTransformForm();
                }
                else if (stSymbolBreakRsp.bBreakType == 1)
                {
                    Singleton<CSymbolMakeController>.GetInstance().PlayBatchBreakAnimator();
                    Singleton<CSymbolMakeController>.GetInstance().OnReceiveBreakItemList();
                    Singleton<CSymbolMakeController>.GetInstance().RefreshSymbolBreakForm();
                }
            }
        }

        [MessageHandler(0x485)]
        public static void ReciveSymbolMakeRsp(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            SCPKG_CMD_SYMBOL_MAKE stSymbolMakeRsp = msg.stPkgData.stSymbolMakeRsp;
            if (stSymbolMakeRsp.iResult == 0)
            {
                CUseable useable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL, stSymbolMakeRsp.stSymbolInfo.dwSymbolID, stSymbolMakeRsp.stSymbolInfo.iSymbolCnt);
                if ((useable != null) && (((CSymbolItem) useable) != null))
                {
                    CUseableContainer container = new CUseableContainer(enCONTAINER_TYPE.ITEM);
                    container.Add(useable);
                    CUICommonSystem.ShowSymbol(container, enUIEventID.None);
                }
                Singleton<CSymbolMakeController>.GetInstance().RefreshSymbolTransformForm();
                Singleton<CSymbolMakeController>.GetInstance().RefreshSymbolMakeForm();
            }
        }

        private void RefreshSymbolBreakForm()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_symbolBreakPath);
            if (null != form)
            {
                Transform transform = form.get_transform().Find("Panel_SymbolBreak/Panel_Content");
                int num = 0;
                for (int i = 0; i < s_maxSymbolBreakLevel; i++)
                {
                    GameObject obj2 = transform.Find(string.Format("breakElement{0}", i)).get_gameObject();
                    Transform transform2 = obj2.get_transform().Find("OnBreakBtn/Checkmark");
                    Transform transform3 = obj2.get_transform().Find("OnBreakBtn/Text");
                    if ((transform2 != null) && transform2.get_gameObject().get_activeSelf())
                    {
                        num |= ((int) 1) << (i + 1);
                    }
                    if (transform3 != null)
                    {
                        if (this.IsAllSymbolBreak(i))
                        {
                            transform3.GetComponent<Text>().set_text(string.Format(Singleton<CTextManager>.GetInstance().GetText("Symbol_BreakAllItem"), i + 1));
                        }
                        else
                        {
                            transform3.GetComponent<Text>().set_text(string.Format(Singleton<CTextManager>.GetInstance().GetText("Symbol_BreakSomeItem"), i + 1));
                        }
                    }
                }
                this.m_breakLevelMask = (ushort) num;
                int selectBreakSymbolCoinCnt = this.GetSelectBreakSymbolCoinCnt();
                form.get_transform().Find("Panel_SymbolBreak/Panel_Bottom/Pl_countText/symbolCoinCntText").GetComponent<Text>().set_text(selectBreakSymbolCoinCnt.ToString());
            }
        }

        public void RefreshSymbolMakeForm()
        {
            CUIFormScript form = null;
            switch (this.Source)
            {
                case enSymbolMakeSource.SymbolManage:
                    form = Singleton<CUIManager>.GetInstance().GetForm(CSymbolSystem.s_symbolFormPath);
                    break;

                case enSymbolMakeSource.Mall:
                    form = Singleton<CMallSystem>.GetInstance().m_MallForm;
                    break;
            }
            if (form != null)
            {
                CTextManager instance = Singleton<CTextManager>.GetInstance();
                string[] titleList = new string[] { instance.GetText("Symbol_Prop_Tab_All"), instance.GetText("Symbol_Prop_Tab_Atk"), instance.GetText("Symbol_Prop_Tab_Hp"), instance.GetText("Symbol_Prop_Tab_Defense"), instance.GetText("Symbol_Prop_Tab_Function"), instance.GetText("Symbol_Prop_Tab_HpSteal"), instance.GetText("Symbol_Prop_Tab_AtkSpeed"), instance.GetText("Symbol_Prop_Tab_Crit"), instance.GetText("Symbol_Prop_Tab_Penetrate") };
                CUICommonSystem.InitMenuPanel(Utility.FindChild(this.m_container, "typeList"), titleList, (int) this.m_symbolFilterType, true);
                string[] strArray2 = new string[] { "1", "2", "3", "4", "5" };
                CUICommonSystem.InitMenuPanel(Utility.FindChild(this.m_container, "Panel_SymbolLevel/levelList"), strArray2, this.m_symbolFilterLevel - 1, true);
                Singleton<CSymbolSystem>.GetInstance().SetSymbolData();
                this.SetSymbolMakeListData();
                this.RefreshSymbolMakeList();
                this.SetAvailableBtn();
                Utility.GetComponetInChild<Text>(this.m_container, "Panel_SymbolBreak/breakCoinCntText").set_text(this.GetBreakExcessSymbolCoinCnt(0xffff).ToString());
            }
        }

        private void RefreshSymbolMakeList()
        {
            if (this.m_container != null)
            {
                Utility.GetComponetInChild<CUIListScript>(this.m_container, "symbolMakeList").SetElementAmount(this.m_symbolMakeList.Count);
            }
        }

        public void RefreshSymbolTransformForm()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_symbolTransformPath);
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (((form != null) && (this.m_curTransformSymbol != null)) && (masterRoleInfo != null))
            {
                GameObject obj2 = form.get_transform().Find("Panel_SymbolTranform/Panel_Content").get_gameObject();
                obj2.get_transform().Find("iconImage").GetComponent<Image>().SetSprite(string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Icon_Dir, this.m_curTransformSymbol.dwIcon), form, true, false, false, false);
                obj2.get_transform().Find("nameText").GetComponent<Text>().set_text(StringHelper.UTF8BytesToString(ref this.m_curTransformSymbol.szName));
                Text component = obj2.get_transform().Find("countText").GetComponent<Text>();
                component.set_text(string.Empty);
                int useableStackCount = 0;
                CUseableContainer useableContainer = masterRoleInfo.GetUseableContainer(enCONTAINER_TYPE.ITEM);
                if (useableContainer != null)
                {
                    useableStackCount = useableContainer.GetUseableStackCount(COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL, this.m_curTransformSymbol.dwID);
                    CTextManager instance = Singleton<CTextManager>.GetInstance();
                    component.set_text((useableStackCount <= 0) ? instance.GetText("Symbol_Not_Own") : string.Format(instance.GetText("Symbol_Own_Cnt"), useableStackCount));
                }
                CSymbolSystem.RefreshSymbolPropContent(obj2.get_transform().Find("symbolPropPanel").get_gameObject(), this.m_curTransformSymbol.dwID);
                obj2.get_transform().Find("makeCoinText").GetComponent<Text>().set_text(this.m_curTransformSymbol.dwMakeCoin.ToString());
                obj2.get_transform().Find("breakCoinText").GetComponent<Text>().set_text(this.m_curTransformSymbol.dwBreakCoin.ToString());
                GameObject obj4 = obj2.get_transform().Find("btnBreak").get_gameObject();
                if (useableStackCount <= 0)
                {
                    CUICommonSystem.SetButtonEnable(obj4.GetComponent<Button>(), false, false, true);
                }
                else
                {
                    CUICommonSystem.SetButtonEnable(obj4.GetComponent<Button>(), true, true, true);
                }
            }
        }

        public static void SendReqExcessSymbolBreak(ListView<CSDT_SYMBOLOPT_INFO> breakSymbolList, string pwd = "")
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x484);
            msg.stPkgData.stSymbolBreak.wSymbolCnt = (ushort) breakSymbolList.Count;
            msg.stPkgData.stSymbolBreak.bBreakType = 1;
            StringHelper.StringToUTF8Bytes(pwd, ref msg.stPkgData.stSymbolBreak.szPswdInfo);
            for (int i = 0; (i < breakSymbolList.Count) && (i < 400); i++)
            {
                msg.stPkgData.stSymbolBreak.astSymbolList[i] = breakSymbolList[i];
            }
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public static void SendReqSymbolBreak(uint heroId, uint symbolId, int cnt, string pwd = "")
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x484);
            msg.stPkgData.stSymbolBreak.dwBelongHeroID = heroId;
            msg.stPkgData.stSymbolBreak.wSymbolCnt = 1;
            msg.stPkgData.stSymbolBreak.bBreakType = 0;
            StringHelper.StringToUTF8Bytes(pwd, ref msg.stPkgData.stSymbolBreak.szPswdInfo);
            CSDT_SYMBOLOPT_INFO csdt_symbolopt_info = new CSDT_SYMBOLOPT_INFO();
            csdt_symbolopt_info.dwSymbolID = symbolId;
            csdt_symbolopt_info.iSymbolCnt = cnt;
            msg.stPkgData.stSymbolBreak.astSymbolList[0] = csdt_symbolopt_info;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public static void SendReqSymbolMake(uint heroId, uint symbolId, int cnt)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x483);
            msg.stPkgData.stSymbolMake.dwBelongHeroID = heroId;
            msg.stPkgData.stSymbolMake.stSymbolInfo.dwSymbolID = symbolId;
            msg.stPkgData.stSymbolMake.stSymbolInfo.iSymbolCnt = cnt;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        private void SetAvailableBtn()
        {
            if (this.m_container != null)
            {
                GameObject obj2 = Utility.FindChild(this.m_container, "Panel_SymbolBreak");
                GameObject obj3 = Utility.FindChild(this.m_container, "Panel_SymbolDraw");
                enSymbolMakeSource source = this.Source;
                if (source == enSymbolMakeSource.SymbolManage)
                {
                    obj2.CustomSetActive(true);
                    obj3.CustomSetActive(false);
                }
                else if (source == enSymbolMakeSource.Mall)
                {
                    obj2.CustomSetActive(false);
                    obj3.CustomSetActive(true);
                    GameObject target = Utility.FindChild(obj3, "btnJump");
                    if (Singleton<CMallSystem>.GetInstance().HasFreeDrawCnt(enRedID.Mall_SymbolTab))
                    {
                        CUIRedDotSystem.AddRedDot(target, enRedDotPos.enTopRight, 0);
                    }
                    else
                    {
                        CUIRedDotSystem.DelRedDot(target);
                    }
                }
                else
                {
                    obj2.CustomSetActive(true);
                    obj3.CustomSetActive(false);
                }
            }
        }

        private bool SetBreakSymbolList()
        {
            bool flag = false;
            this.m_svrBreakSymbolList.Clear();
            int length = this.m_breakSymbolList.Length;
            for (int i = 0; i < length; i++)
            {
                if (this.m_breakSymbolList[i] != null)
                {
                    for (int j = 0; j < this.m_breakSymbolList[i].Count; j++)
                    {
                        if ((this.m_breakSymbolList[i][j].symbol != null) && this.m_breakSymbolList[i][j].bBreak)
                        {
                            CSDT_SYMBOLOPT_INFO item = new CSDT_SYMBOLOPT_INFO();
                            item.dwSymbolID = this.m_breakSymbolList[i][j].symbol.m_baseID;
                            item.iSymbolCnt = this.m_breakSymbolList[i][j].symbol.m_stackCount - this.m_breakSymbolList[i][j].symbol.GetMaxWearCnt();
                            if (this.m_breakSymbolList[i][j].symbol.m_SymbolData.bNeedPswd > 0)
                            {
                                flag = true;
                            }
                            this.m_svrBreakSymbolList.Add(item);
                        }
                    }
                }
            }
            return flag;
        }

        private void SetBreakSymbolListData(int index)
        {
            if ((index >= 0) && (index < this.m_breakSymbolList.Length))
            {
                ListView<CSymbolItem> allSymbolList = Singleton<CSymbolSystem>.GetInstance().GetAllSymbolList();
                int count = allSymbolList.Count;
                ushort breakLvlMask = (ushort) (((int) 1) << (index + 1));
                for (int i = 0; i < count; i++)
                {
                    if (this.CheckSymbolBreak(allSymbolList[i], breakLvlMask))
                    {
                        CBreakSymbolItem item = new CBreakSymbolItem(allSymbolList[i], true);
                        this.m_breakSymbolList[index].Add(item);
                    }
                }
            }
        }

        private void SetContainer(CUIFormScript form)
        {
            switch (this.Source)
            {
                case enSymbolMakeSource.SymbolManage:
                    this.m_container = Utility.FindChild(form.GetWidget(4), CSymbolSystem.s_symbolMakePanel);
                    break;

                case enSymbolMakeSource.Mall:
                    this.m_container = Utility.FindChild(form.GetWidget(3), CSymbolSystem.s_symbolMakePanel);
                    return;
            }
        }

        private void SetSymbolMakeListData()
        {
            this.m_symbolMakeList.Clear();
            int count = s_allSymbolCfgList.Count;
            for (int i = 0; i < count; i++)
            {
                if (((s_allSymbolCfgList[i] != null) && (s_allSymbolCfgList[i].wLevel == this.m_symbolFilterLevel)) && ((this.m_symbolFilterType == enSymbolType.All) || ((s_allSymbolCfgList[i].wType & (((int) 1) << this.m_symbolFilterType)) != 0)))
                {
                    this.m_symbolMakeList.Add(s_allSymbolCfgList[i]);
                }
            }
        }

        public void SwitchToSymbolMakePanel(CUIFormScript form)
        {
            if ((form != null) && !form.IsClosed())
            {
                this.SetContainer(form);
                if (this.m_container != null)
                {
                    this.m_container.CustomSetActive(true);
                    this.ClearSymbolMakeData();
                    this.RefreshSymbolMakeForm();
                    this.ToggleTitle();
                    CSymbolSystem.RefreshSymbolCntText(true);
                    CUseable useable = CUseableManager.CreateVirtualUseable(enVirtualItemType.enSymbolCoin, 0);
                    stUIEventParams eventParams = new stUIEventParams();
                    eventParams.iconUseable = useable;
                    eventParams.tag = 3;
                    CUIEventScript component = form.GetWidget(6).GetComponent<CUIEventScript>();
                    if (component != null)
                    {
                        component.SetUIEvent(enUIEventType.Down, enUIEventID.Tips_ItemInfoOpen, eventParams);
                        component.SetUIEvent(enUIEventType.HoldEnd, enUIEventID.Tips_ItemInfoClose, eventParams);
                        component.SetUIEvent(enUIEventType.Click, enUIEventID.Tips_ItemInfoClose, eventParams);
                        component.SetUIEvent(enUIEventType.DragEnd, enUIEventID.Tips_ItemInfoClose, eventParams);
                    }
                    MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.onEntryMallSymbolMake, new uint[0]);
                }
            }
        }

        private void ToggleTitle()
        {
            if (this.m_container != null)
            {
                GameObject obj2 = Utility.FindChild(this.m_container, "Title");
                enSymbolMakeSource source = this.Source;
                if (source == enSymbolMakeSource.SymbolManage)
                {
                    obj2.CustomSetActive(true);
                }
                else if (source == enSymbolMakeSource.Mall)
                {
                    obj2.CustomSetActive(false);
                }
                else
                {
                    obj2.CustomSetActive(true);
                }
            }
        }

        public override void UnInit()
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolMake_TypeMenuSelect, new CUIEventManager.OnUIEventHandler(this.OnSymbolTypeMenuSelect));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolMake_LevelMenuSelect, new CUIEventManager.OnUIEventHandler(this.OnSymbolLevelMenuSelect));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolMake_ListItemEnable, new CUIEventManager.OnUIEventHandler(this.OnSymbolMakeListEnable));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolMake_ListItemClick, new CUIEventManager.OnUIEventHandler(this.OnSymbolMakeListClick));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolMake_OnItemMakeClick, new CUIEventManager.OnUIEventHandler(this.OnMakeSymbolClick));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolMake_OnItemMakeConfirm, new CUIEventManager.OnUIEventHandler(this.OnItemMakeConfirm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolMake_OnItemBreakClick, new CUIEventManager.OnUIEventHandler(this.OnBreakSymbolClick));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolMake_OnItemBreakConfirm, new CUIEventManager.OnUIEventHandler(this.OnBreakSymbolConfirm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolMake_OnBreakExcessSymbolClick, new CUIEventManager.OnUIEventHandler(this.OnBreakExcessSymbolClick));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolMake_SelectBreakLvlItem, new CUIEventManager.OnUIEventHandler(this.OnSelectBreakLvlItem));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolMake_OnBreakExcessSymbolConfirm, new CUIEventManager.OnUIEventHandler(this.OnBreakExcessSymbolClickConfirm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolMake_ItemBreakAnimatorEnd, new CUIEventManager.OnUIEventHandler(this.OnItemBreakAnimatorEnd));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Symbol_BreakItemEnable, new CUIEventManager.OnUIEventHandler(this.OnBreakListItemEnable));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Symbol_BreakListItemSelToggle, new CUIEventManager.OnUIEventHandler(this.OnBreakListItemSelToggle));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Symbol_OpenBreakDetailForm, new CUIEventManager.OnUIEventHandler(this.OnOpenBreakDetailForm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Symbol_BreakDetailFormConfirm, new CUIEventManager.OnUIEventHandler(this.OnBreakDetailFormConfirm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Symbol_BreakDetailFormCancle, new CUIEventManager.OnUIEventHandler(this.OnBreakDetailFormCancle));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolMake_CoinNotEnoughGotoSymbolMall, new CUIEventManager.OnUIEventHandler(this.OnCoinNotEnoughGotoSymbolMall));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolMake_OnSecurePwdItemBreakConfirm, new CUIEventManager.OnUIEventHandler(this.OnSecurePwdBreakSymbolConfirm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolMake_OnSecurePwdBreakExcessSymbolConfirm, new CUIEventManager.OnUIEventHandler(this.OnSecurePwdBreakExcessSymbolConfirm));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.LOTTERY_GET_NEW_SYMBOL, new Action(this, (IntPtr) this.OnGetNewSymbol));
        }

        public enSymbolMakeSource Source
        {
            [CompilerGenerated]
            get
            {
                return this.<Source>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.<Source>k__BackingField = value;
            }
        }
    }
}

