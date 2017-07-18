namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.Sound;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class CHeroSelectBanPickSystem : Singleton<CHeroSelectBanPickSystem>
    {
        public const int c_banHeroCountMax = 3;
        private const float c_countDownCheckTime = 6.1f;
        private ListView<IHeroData> m_banHeroList;
        private ListView<IHeroData> m_canUseHeroListByJob = new ListView<IHeroData>();
        private enHeroJobType m_heroSelectJobType;
        private IHeroData m_selectBanHeroData;
        public static string s_heroSelectFormPath = "UGUI/Form/System/HeroSelect/Form_HeroSelectBanPick.prefab";
        public static string s_symbolPropPanelPath = "Bottom/Panel_SymbolProp";

        private void CenterHeroItemEnable(CUIEvent uiEvent)
        {
            CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
            CUIListScript srcWidgetBelongedListScript = uiEvent.m_srcWidgetBelongedListScript;
            GameObject srcWidget = uiEvent.m_srcWidget;
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
            if (((srcFormScript != null) && (srcWidgetBelongedListScript != null)) && (((srcWidget != null) && (masterRoleInfo != null)) && (srcWidgetIndexInBelongedList >= 0)))
            {
                IHeroData data = this.m_canUseHeroListByJob[srcWidgetIndexInBelongedList];
                if (data != null)
                {
                    CUIListElementScript component = srcWidget.GetComponent<CUIListElementScript>();
                    if (component != null)
                    {
                        GameObject item = srcWidget.get_transform().Find("heroItemCell").get_gameObject();
                        GameObject obj4 = item.get_transform().Find("TxtFree").get_gameObject();
                        GameObject obj5 = item.get_transform().Find("TxtCreditFree").get_gameObject();
                        GameObject obj6 = item.get_transform().Find("imgExperienceMark").get_gameObject();
                        Transform transform = item.get_transform().Find("expCardPanel");
                        CUIEventScript script4 = item.GetComponent<CUIEventScript>();
                        CUIEventScript script5 = srcWidget.GetComponent<CUIEventScript>();
                        obj4.CustomSetActive(false);
                        obj5.CustomSetActive(false);
                        obj6.CustomSetActive(false);
                        transform.get_gameObject().CustomSetActive(false);
                        script4.set_enabled(false);
                        script5.set_enabled(false);
                        if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enPick)
                        {
                            bool flag = masterRoleInfo.IsFreeHero(data.cfgID);
                            bool bActive = masterRoleInfo.IsCreditFreeHero(data.cfgID);
                            obj4.CustomSetActive(flag && !bActive);
                            obj5.CustomSetActive(bActive);
                            if (masterRoleInfo.IsValidExperienceHero(data.cfgID))
                            {
                                obj6.CustomSetActive(true);
                            }
                            else
                            {
                                obj6.CustomSetActive(false);
                            }
                        }
                        MemberInfo masterMemberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMasterMemberInfo();
                        if ((masterMemberInfo != null) && ((Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enBan) || (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enPick)))
                        {
                            if (Singleton<CHeroSelectBaseSystem>.instance.IsCurBanOrPickMember(masterMemberInfo))
                            {
                                if (Singleton<CHeroSelectBaseSystem>.instance.IsBanByHeroID(data.cfgID) || Singleton<CHeroSelectBaseSystem>.instance.IsHeroExist(data.cfgID))
                                {
                                    CUICommonSystem.SetHeroItemData(srcFormScript, item, data, enHeroHeadType.enIcon, true, true);
                                }
                                else
                                {
                                    script4.set_enabled(true);
                                    script5.set_enabled(true);
                                    CUICommonSystem.SetHeroItemData(srcFormScript, item, data, enHeroHeadType.enIcon, false, true);
                                }
                            }
                            else
                            {
                                CUICommonSystem.SetHeroItemData(srcFormScript, item, data, enHeroHeadType.enIcon, true, true);
                            }
                            if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enBan)
                            {
                                if (this.m_selectBanHeroData != null)
                                {
                                    if (data.cfgID == this.m_selectBanHeroData.heroCfgInfo.dwCfgID)
                                    {
                                        component.ChangeDisplay(true);
                                    }
                                    else
                                    {
                                        component.ChangeDisplay(false);
                                    }
                                }
                                else
                                {
                                    component.ChangeDisplay(false);
                                }
                            }
                            else if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enPick)
                            {
                                if (Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroCount > 0)
                                {
                                    if (data.cfgID == Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList[0])
                                    {
                                        component.ChangeDisplay(true);
                                    }
                                    else
                                    {
                                        component.ChangeDisplay(false);
                                    }
                                }
                                else
                                {
                                    component.ChangeDisplay(false);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void CloseForm()
        {
            Singleton<CUIManager>.GetInstance().CloseForm(s_heroSelectFormPath);
        }

        private void HeroSelect_ConfirmHeroSelect(CUIEvent uiEvent)
        {
            if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enBan)
            {
                if (this.m_selectBanHeroData != null)
                {
                    CHeroSelectBaseSystem.SendBanHeroMsg(this.m_selectBanHeroData.cfgID);
                }
            }
            else if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enPick)
            {
                CHeroSelectBaseSystem.SendMuliPrepareToBattleMsg();
            }
        }

        private void HeroSelect_OnClose(CUIEvent uiEvent)
        {
            this.m_banHeroList = null;
            this.m_selectBanHeroData = null;
            this.m_heroSelectJobType = enHeroJobType.All;
            this.m_canUseHeroListByJob.Clear();
            Singleton<CHeroSelectBaseSystem>.instance.Clear();
            Singleton<CSoundManager>.GetInstance().UnLoadBank("Music_BanPick", CSoundManager.BankType.Lobby);
            Singleton<CSoundManager>.GetInstance().UnLoadBank("Newguide_Voice_BanPick", CSoundManager.BankType.Lobby);
        }

        private void HeroSelect_OnSkinSelect(CUIEvent uiEvent)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            uint heroId = Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList[0];
            uint tagUInt = uiEvent.m_eventParams.tagUInt;
            bool commonBool = uiEvent.m_eventParams.commonBool;
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_heroSelectFormPath);
            if (form != null)
            {
                Transform transform = form.get_gameObject().get_transform().Find("PanelCenter/ListHostSkinInfo");
                Transform transform2 = form.get_gameObject().get_transform().Find("PanelCenter/ListHostSkinInfo/panelEffect/List");
                if ((transform != null) && (transform2 != null))
                {
                    CUIListScript component = transform.GetComponent<CUIListScript>();
                    if (masterRoleInfo.IsCanUseSkin(heroId, tagUInt))
                    {
                        this.InitSkinEffect(transform2.get_gameObject(), heroId, tagUInt);
                    }
                    else
                    {
                        component.SelectElement(component.GetLastSelectedIndex(), true);
                    }
                    if (masterRoleInfo.IsCanUseSkin(heroId, tagUInt))
                    {
                        if (masterRoleInfo.GetHeroWearSkinId(heroId) != tagUInt)
                        {
                            CHeroInfoSystem2.ReqWearHeroSkin(heroId, tagUInt, true);
                        }
                    }
                    else
                    {
                        CHeroSkinBuyManager.OpenBuyHeroSkinForm3D(heroId, tagUInt, false);
                    }
                }
            }
        }

        private void HeroSelect_SelectHero(CUIEvent uiEvent)
        {
            Singleton<CSoundManager>.GetInstance().PostEvent("UI_BanPick_Swicth", null);
            MemberInfo masterMemberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMasterMemberInfo();
            if (masterMemberInfo != null)
            {
                int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
                if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep != enBanPickStep.enSwap)
                {
                    if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enBan)
                    {
                        if (((srcWidgetIndexInBelongedList >= 0) && (srcWidgetIndexInBelongedList < this.m_canUseHeroListByJob.Count)) && Singleton<CHeroSelectBaseSystem>.instance.IsCurBanOrPickMember(masterMemberInfo))
                        {
                            this.m_selectBanHeroData = this.m_canUseHeroListByJob[srcWidgetIndexInBelongedList];
                            this.RefreshCenter();
                        }
                    }
                    else if ((Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enPick) && ((srcWidgetIndexInBelongedList >= 0) && (srcWidgetIndexInBelongedList < this.m_canUseHeroListByJob.Count)))
                    {
                        IHeroData data = this.m_canUseHeroListByJob[srcWidgetIndexInBelongedList];
                        if (data != null)
                        {
                            CHeroSelectBaseSystem.SendHeroSelectMsg(0, 0, data.cfgID);
                        }
                    }
                }
            }
        }

        private void HeroSelect_SwapHeroAllow(CUIEvent uiEvent)
        {
            CHeroSelectBaseSystem.SendSwapAcceptHeroMsg(1);
        }

        private void HeroSelect_SwapHeroCanel(CUIEvent uiEvent)
        {
            if (Singleton<CHeroSelectBaseSystem>.instance.m_swapState == enSwapHeroState.enSwapAllow)
            {
                CHeroSelectBaseSystem.SendSwapAcceptHeroMsg(0);
            }
            else if (Singleton<CHeroSelectBaseSystem>.instance.m_swapState == enSwapHeroState.enReqing)
            {
                CHeroSelectBaseSystem.SendCanelSwapHeroMsg();
            }
        }

        private void HeroSelect_SwapHeroReq(CUIEvent uiEvent)
        {
            CHeroSelectBaseSystem.SendSwapHeroMsg(uiEvent.m_eventParams.tagUInt);
        }

        public override void Init()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_BanPick_FormClose, new CUIEventManager.OnUIEventHandler(this.HeroSelect_OnClose));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_BanPick_HeroJobMenuSelect, new CUIEventManager.OnUIEventHandler(this.OnHeroJobMenuSelect));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_BanPick_SkinSelect, new CUIEventManager.OnUIEventHandler(this.HeroSelect_OnSkinSelect));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_BanPick_SelectHero, new CUIEventManager.OnUIEventHandler(this.HeroSelect_SelectHero));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_BanPick_CenterHeroItemEnable, new CUIEventManager.OnUIEventHandler(this.CenterHeroItemEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_BanPick_ConfirmHeroSelect, new CUIEventManager.OnUIEventHandler(this.HeroSelect_ConfirmHeroSelect));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_BanPick_SwapHeroReq, new CUIEventManager.OnUIEventHandler(this.HeroSelect_SwapHeroReq));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_BanPick_SwapHeroAllow, new CUIEventManager.OnUIEventHandler(this.HeroSelect_SwapHeroAllow));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_BanPick_SwapHeroCanel, new CUIEventManager.OnUIEventHandler(this.HeroSelect_SwapHeroCanel));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_BanPick_Symbol_PageDownBtnClick, new CUIEventManager.OnUIEventHandler(this.OnSymbolPageDownBtnClick));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_BanPick_SymbolPageSelect, new CUIEventManager.OnUIEventHandler(this.OnHeroSymbolPageSelect));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_BanPick_Symbol_ViewProp_Down, new CUIEventManager.OnUIEventHandler(this.OnOpenSymbolProp));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_BanPick_Symbol_ViewProp_Up, new CUIEventManager.OnUIEventHandler(this.OnCloseSymbolProp));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_BanPick_AddedSkillOpenForm, new CUIEventManager.OnUIEventHandler(this.OnOpenAddedSkillPanel));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_BanPick_AddedSkillSelected, new CUIEventManager.OnUIEventHandler(this.OnSelectedAddedSkill));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_BanPick_AddedSkillConfirm, new CUIEventManager.OnUIEventHandler(this.OnConfirmAddedSkill));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_BanPick_AddedSkillCloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseAddedSkillPanel));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_BanPick_OnTimerCountDown, new CUIEventManager.OnUIEventHandler(this.OnTimerCountDown));
        }

        public void InitAddedSkillPanel()
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(s_heroSelectFormPath);
            if ((form != null) && (Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo() != null))
            {
                if (CAddSkillSys.IsSelSkillAvailable())
                {
                    CUIToggleListScript component = form.get_transform().Find("PanelAddSkill/ToggleList").GetComponent<CUIToggleListScript>();
                    CUIListElementScript elemenet = null;
                    CUIEventScript script4 = null;
                    ResSkillUnlock dataByIndex = null;
                    ResSkillCfgInfo dataByKey = null;
                    uint key = 0;
                    ListView<ResSkillUnlock> selSkillAvailable = CAddSkillSys.GetSelSkillAvailable(Singleton<CHeroSelectBaseSystem>.instance.m_mapUnUseSkill);
                    component.SetElementAmount(selSkillAvailable.Count);
                    int index = 0;
                    for (int i = 0; i < selSkillAvailable.Count; i++)
                    {
                        elemenet = component.GetElemenet(i);
                        script4 = elemenet.GetComponent<CUIEventScript>();
                        dataByIndex = selSkillAvailable[i];
                        key = dataByIndex.dwUnlockSkillID;
                        dataByKey = GameDataMgr.skillDatabin.GetDataByKey(key);
                        if (dataByKey != null)
                        {
                            script4.m_onClickEventID = enUIEventID.HeroSelect_BanPick_AddedSkillSelected;
                            script4.m_onClickEventParams.tag = (int) dataByIndex.dwUnlockSkillID;
                            string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Skill_Dir, Utility.UTF8Convert(dataByKey.szIconPath));
                            elemenet.get_transform().Find("Icon").GetComponent<Image>().SetSprite(prefabPath, form.GetComponent<CUIFormScript>(), true, false, false, false);
                            elemenet.get_transform().Find("SkillNameTxt").GetComponent<Text>().set_text(Utility.UTF8Convert(dataByKey.szSkillName));
                        }
                        else
                        {
                            DebugHelper.Assert(false, string.Format("ResSkillCfgInfo[{0}] can not be found!", key));
                        }
                    }
                    component.SelectElement(index, true);
                    dataByIndex = GameDataMgr.addedSkiilDatabin.GetDataByIndex(index);
                }
                form.get_transform().Find("Bottom/AddedSkillItem").get_gameObject().CustomSetActive(false);
                form.get_transform().Find("PanelAddSkill").get_gameObject().CustomSetActive(false);
            }
        }

        public void InitBanHeroList(CUIListScript listScript, COM_PLAYERCAMP camp)
        {
            List<uint> banHeroList = Singleton<CHeroSelectBaseSystem>.instance.GetBanHeroList(camp);
            listScript.SetElementAmount(Singleton<CHeroSelectBaseSystem>.instance.m_banHeroTeamMaxCount);
            IHeroData data = null;
            for (int i = 0; i < banHeroList.Count; i++)
            {
                Transform transform = listScript.GetElemenet(i).get_transform();
                data = CHeroDataFactory.CreateHeroData(banHeroList[i]);
                if (data != null)
                {
                    CUICommonSystem.SetObjActive(transform.get_transform().Find("imageIcon"), true);
                    CUICommonSystem.SetHeroItemData(listScript.m_belongedFormScript, transform.get_gameObject(), data, enHeroHeadType.enBustCircle, false, true);
                }
            }
        }

        private void InitFullHeroListData(ListView<IHeroData> sourceList)
        {
            this.m_canUseHeroListByJob.Clear();
            ListView<IHeroData> view = sourceList;
            for (int i = 0; i < view.Count; i++)
            {
                if (((this.m_heroSelectJobType == enHeroJobType.All) || (view[i].heroCfgInfo.bMainJob == ((byte) this.m_heroSelectJobType))) || (view[i].heroCfgInfo.bMinorJob == ((byte) this.m_heroSelectJobType)))
                {
                    this.m_canUseHeroListByJob.Add(view[i]);
                }
            }
            CHeroOverviewSystem.SortHeroList(ref this.m_canUseHeroListByJob, Singleton<CHeroSelectBaseSystem>.instance.m_sortType, false);
        }

        public void InitHeroList(CUIFormScript form, bool isResetSelect = false)
        {
            CUIListScript component = form.get_transform().Find("PanelCenter/ListHostHeroInfo").GetComponent<CUIListScript>();
            if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enBan)
            {
                this.InitFullHeroListData(this.m_banHeroList);
                component.SetElementAmount(this.m_canUseHeroListByJob.Count);
            }
            else if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enPick)
            {
                this.InitFullHeroListData(Singleton<CHeroSelectBaseSystem>.instance.m_canUseHeroList);
                component.SetElementAmount(this.m_canUseHeroListByJob.Count);
            }
            else
            {
                component.get_gameObject().CustomSetActive(false);
            }
            Button btn = form.get_transform().Find("PanelCenter/ListHostHeroInfo/btnConfirmSelectHero").GetComponent<Button>();
            MemberInfo masterMemberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMasterMemberInfo();
            if ((Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep != enBanPickStep.enSwap) && (masterMemberInfo.camp != COM_PLAYERCAMP.COM_PLAYERCAMP_MID))
            {
                if (masterMemberInfo == null)
                {
                    return;
                }
                if ((Singleton<CHeroSelectBaseSystem>.instance.IsCurBanOrPickMember(masterMemberInfo) && (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enBan)) && (this.m_selectBanHeroData != null))
                {
                    CUICommonSystem.SetButtonEnableWithShader(btn, true, true);
                }
                else if ((Singleton<CHeroSelectBaseSystem>.instance.IsCurBanOrPickMember(masterMemberInfo) && (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enPick)) && (Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList[0] != 0))
                {
                    CUICommonSystem.SetButtonEnableWithShader(btn, true, true);
                }
                else
                {
                    CUICommonSystem.SetButtonEnableWithShader(btn, false, true);
                }
                if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enBan)
                {
                    CUICommonSystem.SetButtonName(btn.get_gameObject(), Singleton<CTextManager>.instance.GetText("BP_SureButton_1"));
                }
                else if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enPick)
                {
                    CUICommonSystem.SetButtonName(btn.get_gameObject(), Singleton<CTextManager>.instance.GetText("BP_SureButton_2"));
                }
            }
            else
            {
                CUICommonSystem.SetButtonEnableWithShader(btn, false, true);
            }
            if (isResetSelect)
            {
                component.SelectElement(-1, true);
            }
        }

        public void InitMenu(bool isResetListSelect = false)
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(s_heroSelectFormPath);
            if (form != null)
            {
                GameObject menuObj = form.get_gameObject().get_transform().Find("PanelCenter/TabListHero").get_gameObject();
                GameObject obj3 = form.get_gameObject().get_transform().Find("PanelCenter/TabListSkin").get_gameObject();
                string[] strTitleList = new string[] { Singleton<CTextManager>.instance.GetText("Choose_Skin") };
                string[] strArray2 = new string[] { Singleton<CTextManager>.GetInstance().GetText("Hero_Job_All"), Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Tank"), Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Soldier"), Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Assassin"), Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Master"), Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Archer"), Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Aid") };
                Transform targetTrans = form.get_transform().Find("PanelCenter/ListHostHeroInfo");
                Transform transform2 = form.get_gameObject().get_transform().Find("PanelCenter/ListHostSkinInfo");
                if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enBan)
                {
                    this.InitSubMenu(menuObj, strArray2, true);
                    this.InitSubMenu(obj3, strTitleList, false);
                    CUICommonSystem.SetObjActive(targetTrans, true);
                    CUICommonSystem.SetObjActive(transform2, false);
                }
                else if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enPick)
                {
                    if (Singleton<CHeroSelectBaseSystem>.instance.m_isSelectConfirm)
                    {
                        this.InitSubMenu(menuObj, strArray2, false);
                        this.InitSubMenu(obj3, strTitleList, true);
                        CUICommonSystem.SetObjActive(targetTrans, false);
                        CUICommonSystem.SetObjActive(transform2, true);
                    }
                    else
                    {
                        this.InitSubMenu(menuObj, strArray2, true);
                        this.InitSubMenu(obj3, strTitleList, false);
                        CUICommonSystem.SetObjActive(targetTrans, true);
                        CUICommonSystem.SetObjActive(transform2, false);
                    }
                }
                else if ((Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enSwap) || Singleton<CHeroSelectBaseSystem>.instance.m_isSelectConfirm)
                {
                    this.InitSubMenu(menuObj, strArray2, false);
                    this.InitSubMenu(obj3, strTitleList, true);
                    CUICommonSystem.SetObjActive(targetTrans, false);
                    CUICommonSystem.SetObjActive(transform2, true);
                }
                this.ResetHeroSelectJobType();
                if (isResetListSelect)
                {
                    this.InitHeroList(form, true);
                }
            }
        }

        private void InitSkinEffect(GameObject objList, uint heroID, uint skinID)
        {
            CSkinInfo.GetHeroSkinProp(heroID, skinID, ref CHeroSelectBaseSystem.s_propArr, ref CHeroSelectBaseSystem.s_propPctArr, ref CHeroSelectBaseSystem.s_propImgArr);
            CUICommonSystem.SetListProp(objList, ref CHeroSelectBaseSystem.s_propArr, ref CHeroSelectBaseSystem.s_propPctArr);
        }

        public void InitSkinList(CUIFormScript form, uint customHeroID = 0)
        {
            if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep != enBanPickStep.enBan)
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if (Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMasterMemberInfo() != null)
                {
                    uint heroId = Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList[0];
                    if (customHeroID != 0)
                    {
                        heroId = customHeroID;
                    }
                    ListView<ResHeroSkin> view = new ListView<ResHeroSkin>();
                    ListView<ResHeroSkin> collection = new ListView<ResHeroSkin>();
                    int index = -1;
                    ResHeroSkin item = null;
                    if (heroId != 0)
                    {
                        ListView<ResHeroSkin> availableSkinByHeroId = CSkinInfo.GetAvailableSkinByHeroId(heroId);
                        for (int i = 0; i < availableSkinByHeroId.Count; i++)
                        {
                            item = availableSkinByHeroId[i];
                            if (masterRoleInfo.IsCanUseSkin(heroId, item.dwSkinID) || CBagSystem.CanUseSkinExpCard(item.dwID))
                            {
                                view.Add(item);
                            }
                            else
                            {
                                collection.Add(item);
                            }
                            if (masterRoleInfo.GetHeroWearSkinId(heroId) == item.dwSkinID)
                            {
                                index = view.Count - 1;
                            }
                        }
                        view.AddRange(collection);
                    }
                    Transform transform = form.get_gameObject().get_transform().Find("PanelCenter/ListHostSkinInfo");
                    Transform transform2 = form.get_gameObject().get_transform().Find("PanelCenter/ListHostSkinInfo/panelEffect");
                    if (transform != null)
                    {
                        CUIListScript[] scriptArray1 = new CUIListScript[] { transform.GetComponent<CUIListScript>() };
                        foreach (CUIListScript script in scriptArray1)
                        {
                            script.SetElementAmount(view.Count);
                            for (int j = 0; j < view.Count; j++)
                            {
                                CUIListElementScript elemenet = script.GetElemenet(j);
                                Transform transform3 = script.GetElemenet(j).get_transform();
                                Image component = transform3.Find("imageIcon").GetComponent<Image>();
                                Image image = transform3.Find("imageIconGray").GetComponent<Image>();
                                Text text = transform3.Find("lblName").GetComponent<Text>();
                                GameObject obj2 = transform3.Find("imgExperienceMark").get_gameObject();
                                Transform transform4 = transform3.Find("expCardPanel");
                                item = view[j];
                                bool bActive = masterRoleInfo.IsValidExperienceSkin(heroId, item.dwSkinID);
                                obj2.CustomSetActive(bActive);
                                bool flag2 = !masterRoleInfo.IsCanUseSkin(heroId, item.dwSkinID) && CBagSystem.CanUseSkinExpCard(item.dwID);
                                RectTransform transform5 = text.get_transform();
                                RectTransform transform6 = transform4;
                                if (flag2)
                                {
                                    transform4.get_gameObject().CustomSetActive(true);
                                    transform5.set_anchoredPosition(new Vector2(transform5.get_anchoredPosition().x, transform6.get_anchoredPosition().y + transform5.get_rect().get_height()));
                                }
                                else
                                {
                                    transform4.get_gameObject().CustomSetActive(false);
                                    transform5.set_anchoredPosition(new Vector2(transform5.get_anchoredPosition().x, transform6.get_anchoredPosition().y));
                                }
                                if (masterRoleInfo.IsCanUseSkin(heroId, item.dwSkinID) || CBagSystem.CanUseSkinExpCard(item.dwID))
                                {
                                    component.get_gameObject().CustomSetActive(true);
                                    image.get_gameObject().CustomSetActive(false);
                                    elemenet.set_enabled(true);
                                }
                                else
                                {
                                    component.get_gameObject().CustomSetActive(false);
                                    image.get_gameObject().CustomSetActive(true);
                                    elemenet.set_enabled(false);
                                }
                                GameObject prefab = CUIUtility.GetSpritePrefeb(CUIUtility.s_Sprite_Dynamic_Icon_Dir + StringHelper.UTF8BytesToString(ref item.szSkinPicID), true, true);
                                component.SetSprite(prefab, false);
                                image.SetSprite(prefab, false);
                                text.set_text(StringHelper.UTF8BytesToString(ref item.szSkinName));
                                CUIEventScript script3 = transform3.GetComponent<CUIEventScript>();
                                stUIEventParams eventParams = new stUIEventParams();
                                eventParams.tagUInt = item.dwSkinID;
                                eventParams.commonBool = bActive;
                                script3.SetUIEvent(enUIEventType.Click, enUIEventID.HeroSelect_BanPick_SkinSelect, eventParams);
                            }
                            script.SelectElement(index, true);
                        }
                        transform2.get_gameObject().CustomSetActive(false);
                    }
                }
            }
        }

        private void InitSubMenu(GameObject menuObj, string[] strTitleList, bool isShow)
        {
            if (isShow)
            {
                CUICommonSystem.InitMenuPanel(menuObj, strTitleList, 0, false);
                CUICommonSystem.SetObjActive(menuObj, true);
            }
            else
            {
                CUICommonSystem.SetObjActive(menuObj, false);
            }
        }

        public void InitSystem(CUIFormScript form)
        {
            CUICommonSystem.SetObjActive(form.get_transform().Find("Top/Timer/CountDownMovie"), false);
            this.InitAddedSkillPanel();
            this.InitMenu(false);
            Singleton<CReplayKitSys>.GetInstance().InitReplayKit(form.get_transform().Find("ReplayKit"), true, true);
        }

        public void InitTeamHeroList(CUIListScript listScript, COM_PLAYERCAMP camp)
        {
            <InitTeamHeroList>c__AnonStorey53 storey = new <InitTeamHeroList>c__AnonStorey53();
            storey.listScript = listScript;
            List<uint> teamHeroList = Singleton<CHeroSelectBaseSystem>.instance.GetTeamHeroList(camp);
            storey.listScript.SetElementAmount(teamHeroList.Count);
            MemberInfo masterMemberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMasterMemberInfo();
            ListView<MemberInfo> view = null;
            MemberInfo mInfo = null;
            Transform transform = null;
            uint id = 0;
            IHeroData data = null;
            if (masterMemberInfo != null)
            {
                for (int i = 0; i < teamHeroList.Count; i++)
                {
                    <InitTeamHeroList>c__AnonStorey52 storey2 = new <InitTeamHeroList>c__AnonStorey52();
                    storey2.<>f__ref$83 = storey;
                    view = Singleton<CHeroSelectBaseSystem>.instance.roomInfo[camp];
                    mInfo = view[i];
                    id = teamHeroList[i];
                    if ((view == null) || (mInfo == null))
                    {
                        return;
                    }
                    transform = storey.listScript.GetElemenet(i).get_transform();
                    GameObject obj2 = transform.Find("BgState/NormalBg").get_gameObject();
                    GameObject obj3 = transform.Find("BgState/NextBg").get_gameObject();
                    GameObject obj4 = transform.Find("BgState/CurrentBg").get_gameObject();
                    CUITimerScript component = transform.Find("BgState/CurrentBg/Timer").GetComponent<CUITimerScript>();
                    obj2.CustomSetActive(false);
                    obj3.CustomSetActive(false);
                    obj4.CustomSetActive(false);
                    component.get_gameObject().CustomSetActive(false);
                    if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enSwap)
                    {
                        obj2.CustomSetActive(true);
                    }
                    else
                    {
                        if ((Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep != enBanPickStep.enBan) && (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep != enBanPickStep.enPick))
                        {
                            return;
                        }
                        if (Singleton<CHeroSelectBaseSystem>.instance.IsCurBanOrPickMember(mInfo))
                        {
                            obj4.CustomSetActive(true);
                            component.get_gameObject().CustomSetActive(true);
                            if (!component.IsRunning())
                            {
                                component.SetTotalTime((float) (Singleton<CHeroSelectBaseSystem>.instance.m_curBanPickInfo.stCurState.dwTimeout / 0x3e8));
                                component.ReStartTimer();
                            }
                        }
                        else if (Singleton<CHeroSelectBaseSystem>.instance.IsNextBanOrPickMember(mInfo))
                        {
                            obj3.CustomSetActive(true);
                            component.EndTimer();
                        }
                        else
                        {
                            obj2.CustomSetActive(true);
                            component.EndTimer();
                        }
                    }
                    GameObject item = transform.Find("heroItemCell").get_gameObject();
                    Text text = item.get_transform().Find("lblName").get_gameObject().GetComponent<Text>();
                    GameObject obj6 = transform.Find("heroItemCell/readyIcon").get_gameObject();
                    Image image = item.get_transform().Find("imageIcon").get_gameObject().GetComponent<Image>();
                    if (id != 0)
                    {
                        data = CHeroDataFactory.CreateHeroData(id);
                        if (data != null)
                        {
                            CUICommonSystem.SetHeroItemData(storey.listScript.m_belongedFormScript, item, data, enHeroHeadType.enIcon, false, true);
                        }
                        image.get_gameObject().CustomSetActive(true);
                    }
                    if (mInfo.camp == masterMemberInfo.camp)
                    {
                        if (mInfo == masterMemberInfo)
                        {
                            string[] args = new string[] { mInfo.MemberName };
                            text.set_text(Singleton<CTextManager>.instance.GetText("Pvp_PlayerName", args));
                        }
                        else
                        {
                            text.set_text(mInfo.MemberName);
                        }
                    }
                    else
                    {
                        string[] textArray2 = new string[] { (mInfo.dwPosOfCamp + 1).ToString() };
                        text.set_text(Singleton<CTextManager>.instance.GetText("Matching_Tip_9", textArray2));
                    }
                    obj6.CustomSetActive(mInfo.isPrepare);
                    CUICommonSystem.SetObjActive(item.get_transform().Find("VoiceIcon"), false);
                    Button button = transform.Find("ExchangeBtn").GetComponent<Button>();
                    if (masterMemberInfo.camp != camp)
                    {
                        button.get_gameObject().CustomSetActive(false);
                    }
                    else if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep != enBanPickStep.enSwap)
                    {
                        button.get_gameObject().CustomSetActive(false);
                    }
                    else if ((Singleton<CHeroSelectBaseSystem>.instance.m_swapState != enSwapHeroState.enReqing) && (mInfo != masterMemberInfo))
                    {
                        if (Singleton<CHeroSelectBaseSystem>.instance.roomInfo.IsHaveHeroByID(masterMemberInfo, id) && Singleton<CHeroSelectBaseSystem>.instance.roomInfo.IsHaveHeroByID(mInfo, masterMemberInfo.ChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID))
                        {
                            button.get_gameObject().CustomSetActive(true);
                            CUIEventScript script2 = button.GetComponent<CUIEventScript>();
                            if (script2 != null)
                            {
                                script2.m_onClickEventParams.tagUInt = mInfo.dwObjId;
                            }
                        }
                        else
                        {
                            button.get_gameObject().CustomSetActive(false);
                        }
                    }
                    else
                    {
                        button.get_gameObject().CustomSetActive(false);
                    }
                    storey2.selSkillCell = transform.Find("selSkillItemCell").get_gameObject();
                    storey2.selSkillID = mInfo.ChoiceHero[0].stBaseInfo.stCommonInfo.stSkill.dwSelSkillID;
                    if ((storey2.selSkillID != 0) && (camp == masterMemberInfo.camp))
                    {
                        GameDataMgr.addedSkiilDatabin.Accept(new Action<ResSkillUnlock>(storey2.<>m__3D));
                    }
                    else
                    {
                        storey2.selSkillCell.get_gameObject().CustomSetActive(false);
                    }
                    if (mInfo.camp == masterMemberInfo.camp)
                    {
                        Transform transform2 = transform.Find("RecentUseHeroPanel");
                        if (transform2 != null)
                        {
                            if ((Singleton<CHeroSelectBaseSystem>.instance.IsCurBanOrPickMember(mInfo) || Singleton<CHeroSelectBaseSystem>.instance.IsPickedMember(mInfo)) || (Singleton<CHeroSelectBaseSystem>.instance.gameType != enSelectGameType.enLadder))
                            {
                                transform2.get_gameObject().CustomSetActive(false);
                                storey2.selSkillCell.CustomSetActive(storey2.selSkillCell.get_activeSelf());
                            }
                            else
                            {
                                storey2.selSkillCell.CustomSetActive(false);
                                transform2.get_gameObject().CustomSetActive(true);
                                for (int j = 0; (j < 3) && (j < mInfo.recentUsedHero.astHeroInfo.Length); j++)
                                {
                                    Transform transform3 = transform2.get_transform().FindChild(string.Format("Element{0}", j));
                                    if (((transform3 != null) && !CLadderSystem.IsRecentUsedHeroMaskSet(ref mInfo.recentUsedHero.dwCtrlMask, COM_RECENT_USED_HERO_MASK.COM_RECENT_USED_HERO_HIDE)) && ((j < mInfo.recentUsedHero.dwHeroNum) && (mInfo.recentUsedHero.astHeroInfo[j].dwHeroID != 0)))
                                    {
                                        CUICommonSystem.SetObjActive(transform3.get_transform().Find("imageIcon"), true);
                                        IHeroData data2 = CHeroDataFactory.CreateHeroData(mInfo.recentUsedHero.astHeroInfo[j].dwHeroID);
                                        CUICommonSystem.SetHeroItemData(storey.listScript.m_belongedFormScript, transform3.get_gameObject(), data2, enHeroHeadType.enBustCircle, false, true);
                                    }
                                    else
                                    {
                                        CUICommonSystem.SetObjActive(transform3.get_transform().Find("imageIcon"), false);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Transform transform4 = transform.Find("RecentUseHeroPanel");
                        if (transform4 != null)
                        {
                            transform4.get_gameObject().CustomSetActive(false);
                        }
                    }
                }
            }
        }

        public void OnCloseAddedSkillPanel(CUIEvent uiEvent)
        {
            this.SetEffectNoteVisiable(true);
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(s_heroSelectFormPath);
            if (form != null)
            {
                form.get_transform().Find("PanelAddSkill").get_gameObject().CustomSetActive(false);
                if (Singleton<CHeroSelectBaseSystem>.instance.IsMultilMode())
                {
                    Singleton<CChatController>.instance.Set_Show_Bottom(true);
                    Singleton<CChatController>.instance.SetEntryNodeVoiceBtnShowable(true);
                }
            }
        }

        private void OnCloseSymbolProp(CUIEvent uiEvent)
        {
            this.SetEffectNoteVisiable(true);
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_heroSelectFormPath);
            if (form != null)
            {
                form.get_gameObject().get_transform().Find(s_symbolPropPanelPath).get_gameObject().get_gameObject().CustomSetActive(false);
            }
        }

        public void OnConfirmAddedSkill(CUIEvent uiEvent)
        {
            uint num = Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList[0];
            uint tag = (uint) uiEvent.m_eventParams.tag;
            if (((num == 0) || (Singleton<CHeroSelectBaseSystem>.instance.m_mapUnUseSkill == null)) || !CAddSkillSys.IsSelSkillAvailable(Singleton<CHeroSelectBaseSystem>.instance.m_mapUnUseSkill, tag))
            {
                DebugHelper.Assert(false, string.Format("CHeroSelectBanPickSystem heroID[{0}] addedSkillID[{1}]", num, tag));
            }
            else
            {
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x48e);
                msg.stPkgData.stUnlockSkillSelReq.dwHeroID = num;
                msg.stPkgData.stUnlockSkillSelReq.dwSkillID = tag;
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
            }
            this.OnCloseAddedSkillPanel(null);
        }

        private void OnHeroJobMenuSelect(CUIEvent uiEvent)
        {
            int selectedIndex = uiEvent.m_srcWidget.GetComponent<CUIListScript>().GetSelectedIndex();
            this.m_heroSelectJobType = (enHeroJobType) selectedIndex;
            this.InitHeroList(uiEvent.m_srcFormScript, false);
        }

        public void OnHeroSkinWearSuc(uint heroId, uint skinId)
        {
            this.RefreshCenter();
        }

        public void OnHeroSymbolPageSelect(CUIEvent uiEvent)
        {
            int selectedIndex = uiEvent.m_srcWidget.GetComponent<CUIListScript>().GetSelectedIndex();
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            uint id = Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList[0];
            if (id != 0)
            {
                CHeroInfo info2;
                uiEvent.m_srcFormScript.get_gameObject().get_transform().Find("Bottom").Find("Panel_SymbolChange/DropList/List").get_gameObject().CustomSetActive(false);
                bool flag = masterRoleInfo.GetHeroInfo(id, out info2, true);
                if (flag && (selectedIndex != info2.m_selectPageIndex))
                {
                    CHeroSelectBaseSystem.SendHeroSelectSymbolPage(id, selectedIndex, false);
                }
                else if ((!flag && masterRoleInfo.IsFreeHero(id)) && (selectedIndex != masterRoleInfo.GetFreeHeroSymbolId(id)))
                {
                    CHeroSelectBaseSystem.SendHeroSelectSymbolPage(id, selectedIndex, false);
                }
            }
        }

        public void OnOpenAddedSkillPanel(CUIEvent uiEvent)
        {
            this.SetEffectNoteVisiable(false);
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(s_heroSelectFormPath);
            if (form != null)
            {
                form.get_transform().Find("PanelAddSkill").get_gameObject().CustomSetActive(true);
                if (Singleton<CHeroSelectBaseSystem>.instance.IsMultilMode() || Singleton<CHeroSelectBaseSystem>.instance.IsSingleWarmBattle())
                {
                    Singleton<CChatController>.instance.Hide_SelectChat_MidNode();
                    Singleton<CChatController>.instance.Set_Show_Bottom(false);
                    Singleton<CChatController>.instance.SetEntryNodeVoiceBtnShowable(false);
                }
            }
        }

        private void OnOpenSymbolProp(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_heroSelectFormPath);
            if (form != null)
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                uint id = Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList[0];
                if (id != 0)
                {
                    CHeroInfo info2;
                    if (masterRoleInfo.GetHeroInfo(id, out info2, true))
                    {
                        this.OpenSymbolPropPanel(form, info2.m_selectPageIndex);
                    }
                    else if (masterRoleInfo.IsFreeHero(id))
                    {
                        int freeHeroSymbolId = masterRoleInfo.GetFreeHeroSymbolId(id);
                        this.OpenSymbolPropPanel(form, freeHeroSymbolId);
                    }
                }
            }
        }

        public void OnSelectedAddedSkill(CUIEvent uiEvent)
        {
            uint heroId = Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList[0];
            if (heroId != 0)
            {
                CUIFormScript form = Singleton<CUIManager>.instance.GetForm(s_heroSelectFormPath);
                if (form != null)
                {
                    uint tag = (uint) uiEvent.m_eventParams.tag;
                    form.get_transform().Find("PanelAddSkill/btnConfirm").GetComponent<CUIEventScript>().m_onClickEventParams.tag = (int) tag;
                    ResSkillCfgInfo dataByKey = GameDataMgr.skillDatabin.GetDataByKey(tag);
                    if (dataByKey != null)
                    {
                        string skillDescLobby = CUICommonSystem.GetSkillDescLobby(dataByKey.szSkillDesc, heroId);
                        form.get_transform().Find("PanelAddSkill/AddSkillTitletxt").GetComponent<Text>().set_text(dataByKey.szSkillName);
                        form.get_transform().Find("PanelAddSkill/AddSkilltxt").GetComponent<Text>().set_text(skillDescLobby);
                    }
                }
            }
        }

        public void OnSymbolPageChange()
        {
            this.RefreshSymbolPage();
        }

        public void OnSymbolPageDownBtnClick(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_heroSelectFormPath);
            if (form != null)
            {
                Transform transform = form.get_gameObject().get_transform().Find("Bottom/Panel_SymbolChange/DropList/List");
                this.SetEffectNoteVisiable(transform.get_gameObject().get_activeSelf());
                transform.get_gameObject().CustomSetActive(!transform.get_gameObject().get_activeSelf());
            }
        }

        private void OnTimerCountDown(CUIEvent uiEvent)
        {
            if ((uiEvent.m_srcFormScript != null) && (uiEvent.m_srcWidget != null))
            {
                Transform transform = uiEvent.m_srcFormScript.get_transform().Find("Top/Timer/CountDownMovie");
                CUITimerScript component = uiEvent.m_srcWidget.GetComponent<CUITimerScript>();
                if ((component.GetCurrentTime() <= 6.1f) && !transform.get_gameObject().get_activeSelf())
                {
                    transform.get_gameObject().CustomSetActive(true);
                    component.get_gameObject().CustomSetActive(false);
                    Singleton<CSoundManager>.GetInstance().PostEvent("UI_daojishi", null);
                    Singleton<CSoundManager>.GetInstance().PostEvent("Play_sys_ban_5", null);
                }
            }
        }

        public void OpenForm()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().OpenForm(s_heroSelectFormPath, false, true);
            if ((form != null) && (Singleton<CHeroSelectBaseSystem>.instance.roomInfo != null))
            {
                this.m_banHeroList = CHeroDataFactory.GetBanHeroList();
                this.InitSystem(form);
                this.RefreshAll();
                Singleton<CSoundManager>.GetInstance().LoadBank("Music_BanPick", CSoundManager.BankType.Lobby);
                Singleton<CSoundManager>.GetInstance().LoadBank("Newguide_Voice_BanPick", CSoundManager.BankType.Lobby);
            }
        }

        private void OpenSymbolPropPanel(CUIFormScript form, int pageIndex)
        {
            this.SetEffectNoteVisiable(false);
            GameObject obj2 = form.get_transform().Find(s_symbolPropPanelPath).get_gameObject();
            CSymbolSystem.RefreshSymbolPageProp(obj2.get_gameObject().get_transform().Find("basePropPanel").get_gameObject().get_transform().Find("List").get_gameObject(), pageIndex, true);
            obj2.get_gameObject().CustomSetActive(true);
        }

        public void PlayCurrentBgAnimation()
        {
            if (Singleton<CUIManager>.instance.GetForm(s_heroSelectFormPath) != null)
            {
                CSDT_BAN_PICK_STATE_INFO stCurState = Singleton<CHeroSelectBaseSystem>.instance.m_curBanPickInfo.stCurState;
                for (int i = 0; i < stCurState.bPosNum; i++)
                {
                    MemberInfo memberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMemberInfo((COM_PLAYERCAMP) stCurState.bCamp, stCurState.szPosList[i]);
                    if (memberInfo != null)
                    {
                        Transform teamPlayerElement = Singleton<CHeroSelectBaseSystem>.instance.GetTeamPlayerElement(memberInfo.ullUid, memberInfo.camp);
                        if (teamPlayerElement == null)
                        {
                            return;
                        }
                        CUICommonSystem.PlayAnimation(teamPlayerElement.Find("BgState/CurrentBg"), null);
                    }
                }
            }
        }

        public void PlayStepTitleAnimation()
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(s_heroSelectFormPath);
            if (form != null)
            {
                CUICommonSystem.PlayAnimation(form.get_transform().Find("Top/Tips"), null);
            }
        }

        public void RefreshAddedSkillItem()
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(s_heroSelectFormPath);
            if (form != null)
            {
                GameObject obj2 = form.get_transform().Find("Bottom/AddedSkillItem").get_gameObject();
                obj2.CustomSetActive(false);
                uint heroId = Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList[0];
                MemberInfo masterMemberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMasterMemberInfo();
                if ((CAddSkillSys.IsSelSkillAvailable() && (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep != enBanPickStep.enBan)) && ((heroId != 0) && (masterMemberInfo != null)))
                {
                    uint dwSelSkillID = masterMemberInfo.ChoiceHero[0].stBaseInfo.stCommonInfo.stSkill.dwSelSkillID;
                    ResSkillCfgInfo dataByKey = GameDataMgr.skillDatabin.GetDataByKey(dwSelSkillID);
                    bool flag = true;
                    if (dataByKey == null)
                    {
                        DebugHelper.Assert(false, string.Format("ResSkillCfgInfo[{0}] can not be found!", dwSelSkillID));
                    }
                    else
                    {
                        obj2.CustomSetActive(true);
                        string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Skill_Dir, Utility.UTF8Convert(dataByKey.szIconPath));
                        obj2.get_transform().Find("Icon").GetComponent<Image>().SetSprite(prefabPath, form, true, false, false, false);
                        string skillDescLobby = CUICommonSystem.GetSkillDescLobby(dataByKey.szSkillDesc, heroId);
                        if (flag)
                        {
                            form.get_transform().Find("PanelAddSkill/AddSkillTitletxt").GetComponent<Text>().set_text(dataByKey.szSkillName);
                            form.get_transform().Find("PanelAddSkill/AddSkilltxt").GetComponent<Text>().set_text(skillDescLobby);
                            form.get_transform().Find("PanelAddSkill/btnConfirm").GetComponent<CUIEventScript>().m_onClickEventParams.tag = (int) dwSelSkillID;
                            ListView<ResSkillUnlock> selSkillAvailable = CAddSkillSys.GetSelSkillAvailable(Singleton<CHeroSelectBaseSystem>.instance.m_mapUnUseSkill);
                            for (int i = 0; i < selSkillAvailable.Count; i++)
                            {
                                if (selSkillAvailable[i].dwUnlockSkillID == dwSelSkillID)
                                {
                                    form.get_transform().Find("PanelAddSkill/ToggleList").GetComponent<CUIToggleListScript>().SelectElement(i, true);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void RefreshAll()
        {
            this.RefreshTop();
            this.RefreshBottom();
            this.RefreshLeft();
            this.RefreshRight();
            this.RefreshCenter();
            this.RefreshSwapPanel();
        }

        public void RefreshBottom()
        {
            this.RefreshSymbolPage();
            this.RefreshAddedSkillItem();
        }

        public void RefreshCenter()
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(s_heroSelectFormPath);
            if (form != null)
            {
                this.InitHeroList(form, false);
                this.InitSkinList(form, 0);
            }
        }

        public void RefreshLeft()
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(s_heroSelectFormPath);
            if (form != null)
            {
                CUIListScript component = form.get_transform().Find("PanelLeft/TeamHeroInfo").GetComponent<CUIListScript>();
                MemberInfo masterMemberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMasterMemberInfo();
                if (masterMemberInfo != null)
                {
                    COM_PLAYERCAMP camp = masterMemberInfo.camp;
                    if (camp == COM_PLAYERCAMP.COM_PLAYERCAMP_MID)
                    {
                        camp = COM_PLAYERCAMP.COM_PLAYERCAMP_1;
                    }
                    this.InitTeamHeroList(component, camp);
                }
            }
        }

        public void RefreshRight()
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(s_heroSelectFormPath);
            if (form != null)
            {
                MemberInfo masterMemberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMasterMemberInfo();
                if (masterMemberInfo != null)
                {
                    COM_PLAYERCAMP enemyCamp;
                    CUIListScript component = form.get_transform().Find("PanelRight/TeamHeroInfo").GetComponent<CUIListScript>();
                    if (masterMemberInfo.camp == COM_PLAYERCAMP.COM_PLAYERCAMP_MID)
                    {
                        enemyCamp = COM_PLAYERCAMP.COM_PLAYERCAMP_2;
                    }
                    else
                    {
                        enemyCamp = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetEnemyCamp(masterMemberInfo.camp);
                    }
                    this.InitTeamHeroList(component, enemyCamp);
                }
            }
        }

        public void RefreshSwapPanel()
        {
            if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enSwap)
            {
                CUIFormScript form = Singleton<CUIManager>.instance.GetForm(s_heroSelectFormPath);
                if (form != null)
                {
                    Transform transform = form.get_transform().Find("PanelSwap/PanelSwapHero");
                    transform.get_gameObject().CustomSetActive(false);
                    if ((Singleton<CHeroSelectBaseSystem>.instance.m_swapState != enSwapHeroState.enIdle) && (Singleton<CHeroSelectBaseSystem>.instance.m_swapInfo != null))
                    {
                        MemberInfo masterMemberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMasterMemberInfo();
                        MemberInfo memberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMemberInfo(Singleton<CHeroSelectBaseSystem>.instance.m_swapInfo.dwActiveObjID);
                        MemberInfo info3 = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMemberInfo(Singleton<CHeroSelectBaseSystem>.instance.m_swapInfo.dwPassiveObjID);
                        if (((masterMemberInfo != null) && (memberInfo != null)) && (info3 != null))
                        {
                            IHeroData data = CHeroDataFactory.CreateHeroData(masterMemberInfo.ChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID);
                            IHeroData data2 = CHeroDataFactory.CreateHeroData(memberInfo.ChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID);
                            IHeroData data3 = CHeroDataFactory.CreateHeroData(info3.ChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID);
                            if (((data != null) && (data2 != null)) && (data3 != null))
                            {
                                GameObject item = transform.Find("heroItemCell1").get_gameObject();
                                GameObject obj3 = transform.Find("heroItemCell2").get_gameObject();
                                GameObject obj4 = transform.Find("btnConfirmSwap").get_gameObject();
                                GameObject obj5 = transform.Find("btnConfirmSwapCanel").get_gameObject();
                                if (Singleton<CHeroSelectBaseSystem>.instance.m_swapState == enSwapHeroState.enSwapAllow)
                                {
                                    CUICommonSystem.SetHeroItemData(form, item, data2, enHeroHeadType.enIcon, false, true);
                                    CUICommonSystem.SetHeroItemData(form, obj3, data, enHeroHeadType.enIcon, false, true);
                                    obj4.CustomSetActive(true);
                                    obj5.CustomSetActive(true);
                                }
                                else
                                {
                                    CUICommonSystem.SetHeroItemData(form, item, data3, enHeroHeadType.enIcon, false, true);
                                    CUICommonSystem.SetHeroItemData(form, obj3, data, enHeroHeadType.enIcon, false, true);
                                    obj4.CustomSetActive(false);
                                    obj5.CustomSetActive(true);
                                }
                                RectTransform teamPlayerElement = Singleton<CHeroSelectBaseSystem>.instance.GetTeamPlayerElement(masterMemberInfo.ullUid, masterMemberInfo.camp) as RectTransform;
                                if (teamPlayerElement != null)
                                {
                                    (transform.get_transform() as RectTransform).set_anchoredPosition(new Vector2(teamPlayerElement.get_anchoredPosition().x + teamPlayerElement.get_rect().get_width(), teamPlayerElement.get_anchoredPosition().y));
                                    transform.get_gameObject().CustomSetActive(true);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void RefreshSymbolPage()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_heroSelectFormPath);
            if (form != null)
            {
                uint key = Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList[0];
                Transform transform = form.get_gameObject().get_transform().Find("Bottom/Panel_SymbolChange");
                transform.get_gameObject().CustomSetActive(false);
                if ((Singleton<CFunctionUnlockSys>.GetInstance().FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_SYMBOL) && (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep != enBanPickStep.enBan)) && (GameDataMgr.heroDatabin.GetDataByKey(key) != null))
                {
                    CHeroInfo info2;
                    CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                    int selectIndex = 0;
                    if (masterRoleInfo.GetHeroInfo(key, out info2, true))
                    {
                        selectIndex = info2.m_selectPageIndex;
                    }
                    else if (masterRoleInfo.IsFreeHero(key))
                    {
                        selectIndex = masterRoleInfo.GetFreeHeroSymbolId(key);
                    }
                    transform.get_gameObject().CustomSetActive(true);
                    SetPageDropListDataByHeroSelect(transform.get_gameObject(), selectIndex);
                }
                else
                {
                    transform.get_gameObject().CustomSetActive(false);
                }
            }
        }

        public void RefreshTop()
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(s_heroSelectFormPath);
            if (form != null)
            {
                Transform transform = form.get_transform().Find("Top/Timer");
                Transform transform2 = form.get_transform().Find("Top/Tips");
                Text component = form.get_transform().Find("Top/Tips/lblTitle").GetComponent<Text>();
                if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enBan)
                {
                    component.get_gameObject().CustomSetActive(true);
                    transform2.get_gameObject().CustomSetActive(true);
                    transform.get_gameObject().CustomSetActive(false);
                    component.set_text(Singleton<CTextManager>.instance.GetText("BP_Title_1"));
                }
                else if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enPick)
                {
                    component.get_gameObject().CustomSetActive(true);
                    transform2.get_gameObject().CustomSetActive(true);
                    transform.get_gameObject().CustomSetActive(false);
                    component.set_text(Singleton<CTextManager>.instance.GetText("BP_Title_2"));
                }
                else
                {
                    transform2.get_gameObject().CustomSetActive(false);
                    transform.get_gameObject().CustomSetActive(true);
                }
                CUIListScript listScript = form.get_transform().Find("Top/LeftListBan").GetComponent<CUIListScript>();
                CUIListScript script3 = form.get_transform().Find("Top/RightListBan").GetComponent<CUIListScript>();
                MemberInfo masterMemberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMasterMemberInfo();
                if (masterMemberInfo != null)
                {
                    COM_PLAYERCAMP camp;
                    COM_PLAYERCAMP enemyCamp;
                    if (masterMemberInfo.camp == COM_PLAYERCAMP.COM_PLAYERCAMP_MID)
                    {
                        camp = COM_PLAYERCAMP.COM_PLAYERCAMP_1;
                        enemyCamp = COM_PLAYERCAMP.COM_PLAYERCAMP_2;
                    }
                    else
                    {
                        camp = masterMemberInfo.camp;
                        enemyCamp = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetEnemyCamp(masterMemberInfo.camp);
                    }
                    this.InitBanHeroList(listScript, camp);
                    this.InitBanHeroList(script3, enemyCamp);
                }
            }
        }

        private void ResetHeroSelectJobType()
        {
            this.m_heroSelectJobType = enHeroJobType.All;
        }

        private void SetEffectNoteVisiable(bool isShow)
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(s_heroSelectFormPath);
            if (form != null)
            {
                CUIListScript component = form.get_transform().Find("PanelLeft/TeamHeroInfo").GetComponent<CUIListScript>();
                CUIListScript script3 = form.get_transform().Find("PanelRight/TeamHeroInfo").GetComponent<CUIListScript>();
                CUIListScript[] scriptArray = new CUIListScript[] { component, script3 };
                int elementAmount = 0;
                CUIListScript script4 = null;
                for (int i = 0; i < scriptArray.Length; i++)
                {
                    script4 = scriptArray[i];
                    elementAmount = script4.GetElementAmount();
                    for (int j = 0; j < elementAmount; j++)
                    {
                        CUICommonSystem.SetObjActive(script4.GetElemenet(j).get_transform().Find("BgState/CurrentBg/UI_BR_effect"), isShow);
                    }
                }
            }
        }

        public static void SetPageDropListDataByHeroSelect(GameObject panelObj, int selectIndex)
        {
            if (panelObj != null)
            {
                CUIListScript component = panelObj.get_transform().Find("DropList/List").GetComponent<CUIListScript>();
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                component.SetElementAmount(masterRoleInfo.m_symbolInfo.m_pageCount);
                for (int i = 0; i < masterRoleInfo.m_symbolInfo.m_pageCount; i++)
                {
                    CUIListElementScript elemenet = component.GetElemenet(i);
                    elemenet.get_gameObject().get_transform().Find("Text").GetComponent<Text>().set_text(masterRoleInfo.m_symbolInfo.GetSymbolPageName(i));
                    elemenet.get_gameObject().get_transform().Find("SymbolLevel/Text").GetComponent<Text>().set_text(masterRoleInfo.m_symbolInfo.GetSymbolPageMaxLvl(i).ToString());
                }
                component.SelectElement(selectIndex, true);
                panelObj.get_transform().Find("DropList/Button_Down/Text").GetComponent<Text>().set_text(masterRoleInfo.m_symbolInfo.GetSymbolPageName(selectIndex));
                panelObj.get_transform().Find("DropList/Button_Down/SymbolLevel/Text").GetComponent<Text>().set_text(masterRoleInfo.m_symbolInfo.GetSymbolPageMaxLvl(selectIndex).ToString());
                panelObj.get_transform().Find("DropList/Button_Down/SymbolLevel/Text").GetComponent<Text>().set_text(masterRoleInfo.m_symbolInfo.GetSymbolPageMaxLvl(selectIndex).ToString());
            }
        }

        public void StartEndTimer(int totlaTimes)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_heroSelectFormPath);
            if (form != null)
            {
                Transform transform = form.get_transform().Find("Top/Timer/CountDown");
                if (transform != null)
                {
                    CUITimerScript component = transform.GetComponent<CUITimerScript>();
                    if (component != null)
                    {
                        component.SetTotalTime((float) totlaTimes);
                        component.m_timerType = enTimerType.CountDown;
                        component.ReStartTimer();
                        component.get_gameObject().CustomSetActive(true);
                    }
                }
            }
        }

        [CompilerGenerated]
        private sealed class <InitTeamHeroList>c__AnonStorey52
        {
            internal CHeroSelectBanPickSystem.<InitTeamHeroList>c__AnonStorey53 <>f__ref$83;
            internal GameObject selSkillCell;
            internal uint selSkillID;

            internal void <>m__3D(ResSkillUnlock rule)
            {
                if ((rule != null) && (rule.dwUnlockSkillID == this.selSkillID))
                {
                    ResSkillCfgInfo dataByKey = GameDataMgr.skillDatabin.GetDataByKey(this.selSkillID);
                    if (dataByKey != null)
                    {
                        string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Skill_Dir, Utility.UTF8Convert(dataByKey.szIconPath));
                        this.selSkillCell.get_transform().Find("Icon").GetComponent<Image>().SetSprite(prefabPath, this.<>f__ref$83.listScript.m_belongedFormScript, true, false, false, false);
                        this.selSkillCell.CustomSetActive(true);
                    }
                    else
                    {
                        DebugHelper.Assert(false, string.Format("SelSkill ResSkillCfgInfo[{0}] can not be find!!", this.selSkillID));
                    }
                }
            }
        }

        [CompilerGenerated]
        private sealed class <InitTeamHeroList>c__AnonStorey53
        {
            internal CUIListScript listScript;
        }
    }
}

