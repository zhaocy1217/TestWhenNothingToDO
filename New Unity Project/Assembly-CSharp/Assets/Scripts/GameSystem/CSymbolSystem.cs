namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    [MessageHandlerClass]
    public class CSymbolSystem : Singleton<CSymbolSystem>
    {
        public enSymbolMenuType m_selectMenuType;
        private ListView<CSymbolItem> m_symbolList = new ListView<CSymbolItem>();
        public CSymbolRecommendController m_symbolRcmdCtrl = new CSymbolRecommendController();
        public CSymbolWearController m_symbolWearCtrl = new CSymbolWearController();
        public static string s_symbolEquipModulePath = "UGUI/Form/System/Symbol/Panel_SymbolEquip.prefab";
        public static string s_symbolEquipPanel = "Panel_SymbolEquip";
        public static string s_symbolFormPath = "UGUI/Form/System/Symbol/Form_Symbol.prefab";
        public static string s_symbolMakeMallModulePath = "UGUI/Form/System/Symbol/Panel_SymbolMake_Shop.prefab";
        public static string s_symbolMakeModulePath = "UGUI/Form/System/Symbol/Panel_SymbolMake.prefab";
        public static string s_symbolMakePanel = "Panel_SymbolMake";
        public static int[] s_symbolPagePropArr = new int[0x25];
        public static int[] s_symbolPagePropPctArr = new int[0x25];
        public static int[] s_symbolPropPctAddArr = new int[0x25];
        public static int[] s_symbolPropValAddArr = new int[0x25];
        public static string s_symbolRecommendModulePath = "UGUI/Form/System/Symbol/Panel_SymbolRecommend.prefab";
        public static string s_symbolRecommendPanel = "Panel_SymbolRecommend";

        public void Clear()
        {
            this.m_selectMenuType = enSymbolMenuType.SymbolEquip;
            this.m_symbolWearCtrl.Clear();
            this.m_symbolRcmdCtrl.Clear();
        }

        public ListView<CSymbolItem> GetAllSymbolList()
        {
            return this.m_symbolList;
        }

        public static string GetSymbolAttString(CSymbolItem symbol, bool bPvp = true)
        {
            if (bPvp)
            {
                return CUICommonSystem.GetFuncEftDesc(ref symbol.m_SymbolData.astFuncEftList, true);
            }
            return CUICommonSystem.GetFuncEftDesc(ref symbol.m_SymbolData.astPveEftList, true);
        }

        public static string GetSymbolAttString(uint cfgId, bool bPvp = true)
        {
            ResSymbolInfo dataByKey = GameDataMgr.symbolInfoDatabin.GetDataByKey(cfgId);
            if (dataByKey == null)
            {
                return string.Empty;
            }
            if (bPvp)
            {
                return CUICommonSystem.GetFuncEftDesc(ref dataByKey.astFuncEftList, true);
            }
            return CUICommonSystem.GetFuncEftDesc(ref dataByKey.astPveEftList, true);
        }

        public CSymbolItem GetSymbolByObjID(ulong objID)
        {
            for (int i = 0; i < this.m_symbolList.Count; i++)
            {
                CSymbolItem item2 = this.m_symbolList[i];
                if ((item2 != null) && (item2.m_objID == objID))
                {
                    return item2;
                }
            }
            return null;
        }

        public int GetSymbolListIndex(uint symbolCfgId)
        {
            return this.m_symbolWearCtrl.GetSymbolListIndex(symbolCfgId);
        }

        public static void GetSymbolProp(uint symbolId, ref int[] propArr, ref int[] propPctArr, bool bPvp)
        {
            int index = 0;
            int num2 = 0x25;
            for (index = 0; index < num2; index++)
            {
                propArr[index] = 0;
                propPctArr[index] = 0;
            }
            ResSymbolInfo dataByKey = GameDataMgr.symbolInfoDatabin.GetDataByKey(symbolId);
            if (dataByKey != null)
            {
                ResDT_FuncEft_Obj[] objArray = !bPvp ? dataByKey.astPveEftList : dataByKey.astFuncEftList;
                for (int i = 0; i < objArray.Length; i++)
                {
                    if (((objArray[i].wType != 0) && (objArray[i].wType < 0x25)) && (objArray[i].iValue != 0))
                    {
                        if (objArray[i].bValType == 0)
                        {
                            propArr[objArray[i].wType] += objArray[i].iValue;
                        }
                        else if (objArray[i].bValType == 1)
                        {
                            propPctArr[objArray[i].wType] += objArray[i].iValue;
                        }
                    }
                }
            }
        }

        public override void Init()
        {
            this.m_symbolWearCtrl.Init(this);
            this.m_symbolRcmdCtrl.Init(this);
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnOpenSymbolForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_FormClose, new CUIEventManager.OnUIEventHandler(this.OnSymbolFormClose));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_OpenForm_ToMakeTab, new CUIEventManager.OnUIEventHandler(this.OnOpenSymbolFormToMakeTab));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_MenuSelect, new CUIEventManager.OnUIEventHandler(this.OnMenuSelect));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_Update_Sub_Module, new CUIEventManager.OnUIEventHandler(this.OnUpdateSubModule));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_Jump_To_MiShu, new CUIEventManager.OnUIEventHandler(this.OnJumpToMishu));
            Singleton<EventRouter>.instance.AddEventHandler("MasterSymbolCoinChanged", new Action(this, (IntPtr) this.OnSymbolCoinChanged));
        }

        public void LoadSubModule(CUIFormScript form, CUIEvent uiEvent = new CUIEvent())
        {
            bool flag = false;
            form.GetWidget(5).CustomSetActive(false);
            form.GetWidget(4).CustomSetActive(false);
            form.GetWidget(3).CustomSetActive(false);
            switch (this.m_selectMenuType)
            {
                case enSymbolMenuType.SymbolEquip:
                    flag = this.m_symbolWearCtrl.Loaded(form);
                    if (!flag)
                    {
                        form.GetWidget(8).CustomSetActive(true);
                        this.m_symbolWearCtrl.Load(form);
                    }
                    break;

                case enSymbolMenuType.SymbolRecommend:
                    flag = this.m_symbolRcmdCtrl.Loaded(form);
                    if (!flag)
                    {
                        form.GetWidget(8).CustomSetActive(true);
                        this.m_symbolRcmdCtrl.Load(form);
                    }
                    break;

                case enSymbolMenuType.SymbolMake:
                    Singleton<CSymbolMakeController>.GetInstance().Source = enSymbolMakeSource.SymbolManage;
                    flag = Singleton<CSymbolMakeController>.GetInstance().Loaded(form);
                    if (!flag)
                    {
                        form.GetWidget(8).CustomSetActive(true);
                        Singleton<CSymbolMakeController>.GetInstance().Load(form);
                    }
                    break;
            }
            uiEvent.m_srcFormScript.GetWidget(1).CustomSetActive(this.m_selectMenuType == enSymbolMenuType.SymbolEquip);
            uiEvent.m_srcFormScript.GetWidget(6).CustomSetActive(false);
            if (!flag)
            {
                GameObject widget = form.GetWidget(7);
                if (widget != null)
                {
                    CUITimerScript component = widget.GetComponent<CUITimerScript>();
                    if (component != null)
                    {
                        component.ReStartTimer();
                    }
                }
            }
            else
            {
                CUIEvent event2 = new CUIEvent();
                event2.m_eventID = enUIEventID.Symbol_Update_Sub_Module;
                event2.m_srcFormScript = form;
                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(event2);
            }
        }

        private void OnJumpToMishu(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.GetInstance().CloseForm(CSymbolMakeController.s_symbolTransformPath);
            stUIEventParams par = new stUIEventParams();
            par.tag = 3;
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Task_OpenForm, par);
        }

        private void OnMenuSelect(CUIEvent uiEvent)
        {
            CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
            if (srcFormScript != null)
            {
                CUIListScript component = uiEvent.m_srcWidget.GetComponent<CUIListScript>();
                if (null != component)
                {
                    int selectedIndex = component.GetSelectedIndex();
                    this.m_selectMenuType = (enSymbolMenuType) selectedIndex;
                    this.LoadSubModule(srcFormScript, uiEvent);
                }
            }
        }

        private void OnOpenSymbolForm(CUIEvent uiEvent)
        {
            this.m_selectMenuType = enSymbolMenuType.SymbolEquip;
            this.OpenSymbolForm();
            Singleton<CLobbySystem>.GetInstance().OnCheckSymbolEquipAlert();
            CMiShuSystem.SendUIClickToServer(enUIClickReprotID.rp_SymbolBtn);
        }

        private void OnOpenSymbolFormToMakeTab(CUIEvent uiEvent)
        {
            this.m_selectMenuType = enSymbolMenuType.SymbolMake;
            this.OpenSymbolForm();
        }

        private void OnSymbolCoinChanged()
        {
            RefreshSymbolCntText(false);
        }

        private void OnSymbolFormClose(CUIEvent uiEvent)
        {
            this.m_symbolRcmdCtrl.OnSymbolFormClose();
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Tips_ItemInfoClose);
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Tips_CommonInfoClose);
        }

        private void OnUpdateSubModule(CUIEvent uiEvent)
        {
            CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
            if (srcFormScript != null)
            {
                CUIListScript component = srcFormScript.GetWidget(0).GetComponent<CUIListScript>();
                srcFormScript.GetWidget(8).CustomSetActive(false);
                switch (this.m_selectMenuType)
                {
                    case enSymbolMenuType.SymbolEquip:
                        srcFormScript.GetWidget(5).CustomSetActive(true);
                        this.m_symbolWearCtrl.SwitchToSymbolWearPanel(srcFormScript);
                        break;

                    case enSymbolMenuType.SymbolRecommend:
                        srcFormScript.GetWidget(3).CustomSetActive(true);
                        this.m_symbolRcmdCtrl.SwitchToSymbolRcmdPanel(srcFormScript);
                        Singleton<CUINewFlagSystem>.GetInstance().DelNewFlag(component.GetElemenet(1).get_gameObject(), enNewFlagKey.New_BtnSymbolFlagKey_V1, true);
                        Singleton<CBattleGuideManager>.GetInstance().OpenBannerDlgByBannerGuideId(7, null, false);
                        break;

                    case enSymbolMenuType.SymbolMake:
                        srcFormScript.GetWidget(4).CustomSetActive(true);
                        Singleton<CSymbolMakeController>.GetInstance().SwitchToSymbolMakePanel(srcFormScript);
                        break;
                }
            }
        }

        private void OpenSymbolForm()
        {
            MonoSingleton<NewbieGuideManager>.GetInstance().SetNewbieBit(14, true, false);
            CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(s_symbolFormPath, false, true);
            if (script != null)
            {
                string text = Singleton<CTextManager>.GetInstance().GetText("Symbol_Sheet_Symbol");
                string str2 = Singleton<CTextManager>.GetInstance().GetText("SymbolRcmd_Tab");
                string str3 = Singleton<CTextManager>.GetInstance().GetText("Symbol_Sheet_Make");
                string[] titleList = new string[] { text, str2, str3 };
                GameObject widget = script.GetWidget(0);
                CUICommonSystem.InitMenuPanel(widget, titleList, (int) this.m_selectMenuType, true);
                CUIListScript component = widget.GetComponent<CUIListScript>();
                Singleton<CUINewFlagSystem>.GetInstance().AddNewFlag(component.GetElemenet(1).get_gameObject(), enNewFlagKey.New_BtnSymbolFlagKey_V1, enNewFlagPos.enTopRight, 1f, 0f, 0f, enNewFlagType.enNewFlag);
            }
        }

        [MessageHandler(0x46d)]
        public static void ReciveSymbolDatail(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            SCPKG_CMD_SYMBOLDETAIL stSymbolDetail = msg.stPkgData.stSymbolDetail;
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                masterRoleInfo.m_symbolInfo.SetData(stSymbolDetail.stSymbolInfo);
                Singleton<CSymbolSystem>.GetInstance().RefreshSymbolForm();
            }
            else
            {
                DebugHelper.Assert(false, "ReciveSymbolDatail Master RoleInfo is NULL!!!");
            }
        }

        public static void RefreshPropPanel(GameObject propPanel, ref int[] propArr, ref int[] propPctArr)
        {
            int num = 0x25;
            int amount = 0;
            for (int i = 0; i < num; i++)
            {
                if ((propArr[i] > 0) || (propPctArr[i] > 0))
                {
                    amount++;
                }
            }
            CUIListScript component = propPanel.GetComponent<CUIListScript>();
            component.SetElementAmount(amount);
            amount = 0;
            for (int j = 0; j < num; j++)
            {
                if ((propArr[j] > 0) || (propPctArr[j] > 0))
                {
                    CUIListElementScript elemenet = component.GetElemenet(amount);
                    DebugHelper.Assert(elemenet != null);
                    if (elemenet != null)
                    {
                        Text text = elemenet.get_gameObject().get_transform().Find("titleText").GetComponent<Text>();
                        Text text2 = elemenet.get_gameObject().get_transform().Find("valueText").GetComponent<Text>();
                        DebugHelper.Assert(text != null);
                        if (text != null)
                        {
                            text.set_text(CUICommonSystem.s_attNameList[j]);
                        }
                        DebugHelper.Assert(text2 != null);
                        if (text2 != null)
                        {
                            if (propArr[j] > 0)
                            {
                                if (CUICommonSystem.s_pctFuncEftList.IndexOf((uint) j) != -1)
                                {
                                    text2.set_text(string.Format("+{0}", CUICommonSystem.GetValuePercent(propArr[j] / 100)));
                                }
                                else
                                {
                                    text2.set_text(string.Format("+{0}", ((float) propArr[j]) / 100f));
                                }
                            }
                            else if (propPctArr[j] > 0)
                            {
                                text2.set_text(string.Format("+{0}", CUICommonSystem.GetValuePercent(propPctArr[j])));
                            }
                        }
                    }
                    amount++;
                }
            }
        }

        public static void RefreshSymbolCntText(bool forceShow = false)
        {
            GameObject widget = null;
            switch (Singleton<CSymbolMakeController>.GetInstance().Source)
            {
                case enSymbolMakeSource.SymbolManage:
                {
                    CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_symbolFormPath);
                    if (form == null)
                    {
                        return;
                    }
                    widget = form.GetWidget(6);
                    break;
                }
                case enSymbolMakeSource.Mall:
                {
                    CUIFormScript script2 = Singleton<CUIManager>.GetInstance().GetForm(Singleton<CMallSystem>.GetInstance().sMallFormPath);
                    if (script2 == null)
                    {
                        return;
                    }
                    widget = script2.GetWidget(13);
                    goto Label_007C;
                }
            }
        Label_007C:
            if (widget == null)
            {
                return;
            }
            Text component = widget.get_transform().Find("symbolCoinCntText").GetComponent<Text>();
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (component != null)
            {
                if (masterRoleInfo != null)
                {
                    component.set_text(masterRoleInfo.SymbolCoin.ToString());
                }
                else
                {
                    component.set_text(string.Empty);
                }
            }
            if (forceShow)
            {
                widget.CustomSetActive(true);
            }
        }

        public void RefreshSymbolForm()
        {
            if (Singleton<CUIManager>.GetInstance().GetForm(s_symbolFormPath) != null)
            {
                if (this.m_selectMenuType == enSymbolMenuType.SymbolEquip)
                {
                    this.m_symbolWearCtrl.RefreshSymbolEquipPanel();
                }
                else if (this.m_selectMenuType == enSymbolMenuType.SymbolMake)
                {
                    Singleton<CSymbolMakeController>.GetInstance().RefreshSymbolMakeForm();
                }
                else if (this.m_selectMenuType == enSymbolMenuType.SymbolRecommend)
                {
                    this.m_symbolRcmdCtrl.RefreshSymbolRcmdPanel();
                }
            }
        }

        public static void RefreshSymbolItem(ResSymbolInfo symbolInfo, GameObject widget, CUIFormScript form, enSymbolMakeSource source = 0)
        {
            if ((symbolInfo != null) && (widget != null))
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if (masterRoleInfo != null)
                {
                    CUseable useableByBaseID = masterRoleInfo.GetUseableContainer(enCONTAINER_TYPE.ITEM).GetUseableByBaseID(COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL, symbolInfo.dwID);
                    Image component = widget.get_transform().Find("iconImage").GetComponent<Image>();
                    Text text = widget.get_transform().Find("countText").GetComponent<Text>();
                    Text text2 = widget.get_transform().Find("nameText").GetComponent<Text>();
                    Text text3 = widget.get_transform().Find("descText").GetComponent<Text>();
                    Transform transform = widget.get_transform().Find("Price");
                    if (transform != null)
                    {
                        Text componetInChild = Utility.GetComponetInChild<Text>(transform.get_gameObject(), "Text");
                        if (componetInChild != null)
                        {
                            componetInChild.set_text(symbolInfo.dwMakeCoin.ToString());
                        }
                        CUIEventScript script = transform.GetComponent<CUIEventScript>();
                        if (script != null)
                        {
                            stUIEventParams eventParams = new stUIEventParams();
                            eventParams.symbolTransParam.symbolCfgInfo = symbolInfo;
                            script.SetUIEvent(enUIEventType.Click, enUIEventID.SymbolMake_ListItemClick, eventParams);
                        }
                    }
                    component.SetSprite(string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Icon_Dir, symbolInfo.dwIcon), form, true, false, false, false);
                    text.set_text((useableByBaseID == null) ? "0" : useableByBaseID.m_stackCount.ToString());
                    text2.set_text(symbolInfo.szName);
                    text3.set_text(GetSymbolAttString(symbolInfo.dwID, true));
                    CUIEventScript script2 = widget.GetComponent<CUIEventScript>();
                    if (script2 != null)
                    {
                        stUIEventParams params2 = new stUIEventParams();
                        params2.symbolTransParam.symbolCfgInfo = symbolInfo;
                        script2.SetUIEvent(enUIEventType.Click, enUIEventID.SymbolMake_ListItemClick, params2);
                    }
                    if (source == enSymbolMakeSource.Mall)
                    {
                        CUICommonSystem.PlayAnimator(widget, "Symbol_Normal");
                    }
                    else if (useableByBaseID != null)
                    {
                        CUICommonSystem.PlayAnimator(widget, "Symbol_Normal");
                    }
                    else
                    {
                        CUICommonSystem.PlayAnimator(widget, "Symbol_Disabled");
                    }
                }
            }
        }

        public static void RefreshSymbolPageProp(GameObject propListPanel, int pageIndex, bool bPvp = true)
        {
            if (propListPanel != null)
            {
                Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_symbolInfo.GetSymbolPageProp(pageIndex, ref s_symbolPagePropArr, ref s_symbolPagePropPctArr, bPvp);
                RefreshPropPanel(propListPanel, ref s_symbolPagePropArr, ref s_symbolPagePropPctArr);
            }
        }

        public static void RefreshSymbolPagePveEnhanceProp(GameObject propList, int pageIndex)
        {
            if (propList != null)
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                masterRoleInfo.m_symbolInfo.GetSymbolPageProp(pageIndex, ref s_symbolPagePropArr, ref s_symbolPagePropPctArr, true);
                masterRoleInfo.m_symbolInfo.GetSymbolPageProp(pageIndex, ref s_symbolPropValAddArr, ref s_symbolPropPctAddArr, false);
                int num = 0x25;
                for (int i = 0; i < num; i++)
                {
                    s_symbolPropValAddArr[i] -= s_symbolPagePropArr[i];
                    s_symbolPropPctAddArr[i] -= s_symbolPagePropPctArr[i];
                }
                RefreshPropPanel(propList, ref s_symbolPropValAddArr, ref s_symbolPropPctAddArr);
            }
        }

        public static void RefreshSymbolPropContent(GameObject propPanel, uint symbolId)
        {
            ResSymbolInfo dataByKey = GameDataMgr.symbolInfoDatabin.GetDataByKey(symbolId);
            if ((propPanel != null) && (dataByKey != null))
            {
                GetSymbolProp(symbolId, ref s_symbolPagePropArr, ref s_symbolPagePropPctArr, true);
                GetSymbolProp(symbolId, ref s_symbolPropValAddArr, ref s_symbolPropPctAddArr, false);
                int num = 0x25;
                for (int i = 0; i < num; i++)
                {
                    s_symbolPropValAddArr[i] -= s_symbolPagePropArr[i];
                    s_symbolPropPctAddArr[i] -= s_symbolPagePropPctArr[i];
                }
                RefreshPropPanel(propPanel.get_transform().Find("pvpPropPanel").Find("propList").get_gameObject(), ref s_symbolPagePropArr, ref s_symbolPagePropPctArr);
                RefreshPropPanel(propPanel.get_transform().Find("pveEnhancePropPanel").Find("propList").get_gameObject(), ref s_symbolPropValAddArr, ref s_symbolPropPctAddArr);
            }
        }

        public static void SendQuerySymbol()
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x46c);
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public void SetSymbolData()
        {
            CUseableContainer useableContainer = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetUseableContainer(enCONTAINER_TYPE.ITEM);
            int curUseableCount = useableContainer.GetCurUseableCount();
            CUseable useableByIndex = null;
            this.m_symbolList.Clear();
            int index = 0;
            for (index = 0; index < curUseableCount; index++)
            {
                useableByIndex = useableContainer.GetUseableByIndex(index);
                if (useableByIndex.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL)
                {
                    CSymbolItem item = (CSymbolItem) useableByIndex;
                    if (item != null)
                    {
                        this.m_symbolList.Add(item);
                    }
                }
            }
        }

        public override void UnInit()
        {
            this.m_symbolWearCtrl.UnInit();
            this.m_symbolRcmdCtrl.UnInit();
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Symbol_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnOpenSymbolForm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Symbol_FormClose, new CUIEventManager.OnUIEventHandler(this.OnSymbolFormClose));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Symbol_OpenForm_ToMakeTab, new CUIEventManager.OnUIEventHandler(this.OnOpenSymbolFormToMakeTab));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Symbol_MenuSelect, new CUIEventManager.OnUIEventHandler(this.OnMenuSelect));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Symbol_Update_Sub_Module, new CUIEventManager.OnUIEventHandler(this.OnUpdateSubModule));
            Singleton<EventRouter>.instance.RemoveEventHandler("MasterSymbolCoinChanged", new Action(this, (IntPtr) this.OnSymbolCoinChanged));
        }
    }
}

