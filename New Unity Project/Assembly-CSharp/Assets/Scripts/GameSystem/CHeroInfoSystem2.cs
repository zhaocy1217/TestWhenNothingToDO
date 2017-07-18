namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.GameLogic.DataCenter;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    [MessageHandlerClass]
    public class CHeroInfoSystem2 : Singleton<CHeroInfoSystem2>
    {
        private const int c_maxSelectedHeroIDsBeforeGC = 3;
        public uint m_currentDisplayedHeroID;
        private bool m_curShow2DImage;
        protected string m_heroImgPath = string.Empty;
        private CUIFormScript m_heroInfoForm;
        protected string m_heroModelPath = string.Empty;
        private enHeroFormOpenSrc m_OpenSrc;
        private List<uint> m_selectedHeroIDs = new List<uint>();
        public IHeroData m_selectHeroData;
        public uint m_selectHeroID;
        public static string s_heroInfoFormPath = "UGUI/Form/System/HeroInfo/Form_HeroInfo2.prefab";
        public static string s_heroLeftPanel = "Panel_Left";
        public static string s_heroPropertyFormPath = "UGUI/Form/System/HeroInfo/Form_Hero_Property.prefab";
        public static string s_heroStoryFormPath = "UGUI/Form/System/HeroInfo/Form_Hero_Story.prefab";
        public static int s_maxBasePropVal = 10;
        public static int[] s_propArr = new int[0x25];
        public static string[] s_propImgArr = new string[0x25];
        public static int[] s_propPctAddArr = new int[0x25];
        public static int[] s_propPctArr = new int[0x25];
        public static int[] s_propValAddArr = new int[0x25];
        public static readonly string valForm1 = "<color=#60bd67>{0}</color>({1}+<color=#60bd67>{2}</color>)";
        public static readonly string valForm2 = "<color=#60bd67>{0}</color>";
        public static readonly string valForm3 = "<color=#60bd67>{0}</color>|{1}";
        public static readonly string valForm4 = "{0}|{1}";

        private void ColloctMem(uint heroID)
        {
            if (this.m_currentDisplayedHeroID != heroID)
            {
                this.m_currentDisplayedHeroID = heroID;
                if (this.m_selectedHeroIDs.Count >= 3)
                {
                    Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharShow");
                    Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharSkillIcon");
                    Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("ActorInfo");
                    Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharLoading");
                    Singleton<CResourceManager>.GetInstance().UnloadUnusedAssets();
                    this.m_selectedHeroIDs.Clear();
                }
                if ((this.m_currentDisplayedHeroID > 0) && !this.m_selectedHeroIDs.Contains(this.m_currentDisplayedHeroID))
                {
                    this.m_selectedHeroIDs.Add(this.m_currentDisplayedHeroID);
                }
            }
        }

        private static string GetFormPercentStr(int percent, bool isExtra)
        {
            if (isExtra)
            {
                return string.Format(valForm2, CUICommonSystem.GetValuePercent(percent));
            }
            return CUICommonSystem.GetValuePercent(percent);
        }

        private static string GetFormStr(float baseValue, float growValue)
        {
            if (growValue > 0f)
            {
                return string.Format(valForm1, baseValue + growValue, baseValue, growValue);
            }
            return baseValue.ToString();
        }

        public static string GetSkinCannotBuyStr(ResHeroSkin skinInfo)
        {
            ResHeroSkinShop shop = null;
            GameDataMgr.skinShopInfoDict.TryGetValue(skinInfo.dwID, out shop);
            if (shop != null)
            {
                return (!string.IsNullOrEmpty(shop.szGetPath) ? shop.szGetPath : Singleton<CTextManager>.GetInstance().GetText("Hero_SkinState_CannotBuy"));
            }
            return null;
        }

        public override void Init()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroInfo_ViewStory, new CUIEventManager.OnUIEventHandler(this.OnViewHeroStory));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroInfo_ViewProperty, new CUIEventManager.OnUIEventHandler(this.OnViewHeroProperty));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroInfo_OpenCustomEquipPanel, new CUIEventManager.OnUIEventHandler(this.OnOpenCustomEquipPanel));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroInfo_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_OpenForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroInfo_FormClose, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_FormClose));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroInfo_Appear, new CUIEventManager.OnUIEventHandler(this.OnHeroInfoFormApper));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroInfo_TurnLeft, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_TurnLeft));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroInfo_TurnRight, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_TurnRight));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroInfo_GotoRank_God, new CUIEventManager.OnUIEventHandler(this.OnHeroInfoGotoRankGod));
            Singleton<EventRouter>.instance.AddEventHandler<string>("HeroUnlockPvP", new Action<string>(this.OnHeroUnlockPvP));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroInfo_MenuSelect_Dummy, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_MenuSelect_Dummy));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroInfo_Material_Direct_Buy, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_MaterialDirectBuy));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroInfo_Material_Direct_Buy_Confirm, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_MaterialDirectBuyConfirm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroInfo_SkillTipOpen, new CUIEventManager.OnUIEventHandler(this.OnHeroSkillTipOpen));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroInfo_SkillTipClose, new CUIEventManager.OnUIEventHandler(this.OnHeroSkillTipClose));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSkin_ItemSelect, new CUIEventManager.OnUIEventHandler(this.OnHeroSkin_ItemSelect));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSkin_ItemEnable, new CUIEventManager.OnUIEventHandler(this.OnHeroSkin_ItemEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSkin_Wear, new CUIEventManager.OnUIEventHandler(this.OnHeroSkin_Wear));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroInfo_Show2DImage, new CUIEventManager.OnUIEventHandler(this.OnShow2DImage));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroInfo_Show3DImage, new CUIEventManager.OnUIEventHandler(this.OnShow3DImage));
            Singleton<EventRouter>.instance.AddEventHandler<uint>("HeroAdd", new Action<uint>(this.OnNtyAddHero));
            Singleton<EventRouter>.instance.AddEventHandler<uint, uint, uint>("HeroSkinAdd", new Action<uint, uint, uint>(this, (IntPtr) this.OnAddHeroSkin));
            Singleton<EventRouter>.instance.AddEventHandler(EventID.GLOBAL_REFRESH_TIME, new Action(null, (IntPtr) ResetDirectBuyLimit));
        }

        private void OnAddHeroSkin(uint heroId, uint skinId, uint addReason)
        {
            if ((addReason == 1) && !Singleton<CHeroSelectBaseSystem>.instance.m_isInHeroSelectState)
            {
                CUICommonSystem.ShowNewHeroOrSkin(heroId, skinId, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKIN, false, null, enFormPriority.Priority1, 0, 0);
            }
            if ((this.m_heroInfoForm != null) && (heroId == this.m_selectHeroID))
            {
                this.RefreshHeroInfoForm();
            }
        }

        public void OnHeroInfo_FormClose(CUIEvent uiEvent)
        {
            this.m_heroInfoForm = null;
            OutlineFilter.EnableSurfaceShaderOutline(false);
            DynamicShadow.DisableAllDynamicShadows();
            Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharShow");
            Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharSkillIcon");
            Singleton<CResourceManager>.GetInstance().UnloadUnusedAssets();
            this.m_selectedHeroIDs.Clear();
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.HeroInfo_SkillTipClose);
            Singleton<CSoundManager>.GetInstance().PostEvent("Stop_Show", null);
        }

        private void OnHeroInfo_MaterialDirectBuy(CUIEvent uiEvent)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                if (masterRoleInfo.MaterialDirectBuyLimit <= 0)
                {
                    Singleton<CUIManager>.GetInstance().OpenTips("Hero_Material_Direct_Buy_Limit_Tip", true, 1.5f, null, new object[0]);
                }
                else
                {
                    object[] args = new object[] { uiEvent.m_eventParams.iconUseable.m_dianQuanDirectBuy * uiEvent.m_eventParams.iconUseable.m_stackCount, uiEvent.m_eventParams.iconUseable.m_stackCount, uiEvent.m_eventParams.iconUseable.m_name, masterRoleInfo.MaterialDirectBuyLimit };
                    string strContent = string.Format(Singleton<CTextManager>.GetInstance().GetText("Hero_Material_Direct_Buy_Tip"), args);
                    Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(strContent, enUIEventID.HeroInfo_Material_Direct_Buy_Confirm, enUIEventID.None, uiEvent.m_eventParams, false);
                }
            }
        }

        private void OnHeroInfo_MaterialDirectBuyConfirm(CUIEvent uiEvent)
        {
            int num = (int) (uiEvent.m_eventParams.iconUseable.m_dianQuanDirectBuy * uiEvent.m_eventParams.iconUseable.m_stackCount);
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                if (num > masterRoleInfo.DianQuan)
                {
                    CUICommonSystem.OpenDianQuanNotEnoughTip();
                }
                else
                {
                    CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x10cd);
                    msg.stPkgData.stDirectBuyItemReq.bItemType = (byte) uiEvent.m_eventParams.iconUseable.m_type;
                    msg.stPkgData.stDirectBuyItemReq.dwItemID = uiEvent.m_eventParams.iconUseable.m_baseID;
                    msg.stPkgData.stDirectBuyItemReq.dwItemCnt = (uint) uiEvent.m_eventParams.iconUseable.m_stackCount;
                    Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
                }
            }
        }

        private void OnHeroInfo_MenuSelect_Dummy(CUIEvent uiEvent)
        {
            DebugHelper.Assert(true);
        }

        public void OnHeroInfo_OpenForm(CUIEvent uiEvent)
        {
            OutlineFilter.EnableSurfaceShaderOutline(true);
            this.m_selectHeroID = uiEvent.m_eventParams.openHeroFormPar.heroId;
            this.m_OpenSrc = uiEvent.m_eventParams.openHeroFormPar.openSrc;
            this.m_selectHeroData = CHeroDataFactory.CreateHeroData(this.m_selectHeroID);
            this.m_heroInfoForm = Singleton<CUIManager>.GetInstance().OpenForm(s_heroInfoFormPath, true, true);
            if (this.m_heroInfoForm != null)
            {
                if (GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_HEROINFO_IS_SHOW_2D_ID) == 0)
                {
                    this.m_heroInfoForm.GetWidget(0x12).CustomSetActive(false);
                }
                Singleton<CLobbySystem>.GetInstance().SetTopBarPriority(enFormPriority.Priority1);
                Singleton<CUIManager>.GetInstance().LoadUIScenePrefab(CUIUtility.s_heroSceneBgPath, this.m_heroInfoForm);
                this.SetCurModelShowType(false);
                this.RefreshHeroInfoForm();
                if (this.m_OpenSrc == enHeroFormOpenSrc.SkinBuyClick)
                {
                    this.SelectSkinElement(this.m_selectHeroID, uiEvent.m_eventParams.openHeroFormPar.skinId);
                }
                else
                {
                    uint heroWearSkinId = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetHeroWearSkinId(this.m_selectHeroID);
                    this.SelectSkinElement(this.m_selectHeroID, heroWearSkinId);
                }
                this.UpdateSwitchButton(this.m_heroInfoForm);
                DynamicShadow.EnableDynamicShow(this.m_heroInfoForm.get_gameObject(), true);
                this.SetVictoryTipsEvt();
                if (CSysDynamicBlock.bLobbyEntryBlocked)
                {
                    CUICommonSystem.SetObjActive(this.m_heroInfoForm.get_transform().FindChild("Panel_Left/Button_Goto_Rank"), false);
                    CUICommonSystem.SetObjActive(this.m_heroInfoForm.GetWidget(0x12), false);
                }
                GameObject widget = this.m_heroInfoForm.GetWidget(5);
                if (widget != null)
                {
                    widget.CustomSetActive(false);
                }
            }
        }

        public void OnHeroInfo_TurnLeft(CUIEvent uiEvent)
        {
            switch (this.m_OpenSrc)
            {
                case enHeroFormOpenSrc.HeroListClick:
                {
                    int heroIndexByConfigId = Singleton<CHeroOverviewSystem>.GetInstance().GetHeroIndexByConfigId(this.m_selectHeroID);
                    if (heroIndexByConfigId > 0)
                    {
                        int index = heroIndexByConfigId - 1;
                        this.m_selectHeroData = Singleton<CHeroOverviewSystem>.GetInstance().GetHeroDataBuyIndex(index);
                        if (this.m_selectHeroData != null)
                        {
                            uint heroWearSkinId = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetHeroWearSkinId(this.m_selectHeroData.cfgID);
                            this.SwitchHeroInfo(this.m_selectHeroData.cfgID, heroWearSkinId, true, true);
                            Singleton<CSoundManager>.GetInstance().PostEvent("UI_hero_page", null);
                        }
                    }
                    break;
                }
                case enHeroFormOpenSrc.HeroBuyClick:
                {
                    int num4 = Singleton<CMallSystem>.GetInstance().m_heroMallCtrl.GetHeroIndexByConfigId(this.m_selectHeroID);
                    if (num4 > 0)
                    {
                        int num5 = num4 - 1;
                        this.m_selectHeroData = Singleton<CMallSystem>.GetInstance().m_heroMallCtrl.GetHeroDataByIndex(num5);
                        if (this.m_selectHeroData != null)
                        {
                            uint skinId = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetHeroWearSkinId(this.m_selectHeroData.cfgID);
                            this.SwitchHeroInfo(this.m_selectHeroData.cfgID, skinId, true, true);
                            Singleton<CSoundManager>.GetInstance().PostEvent("UI_hero_page", null);
                        }
                    }
                    break;
                }
                case enHeroFormOpenSrc.SkinBuyClick:
                {
                    CUIListScript component = this.m_heroInfoForm.GetWidget(2).get_transform().Find("List_Skin").GetComponent<CUIListScript>();
                    if (component != null)
                    {
                        int selectedIndex = component.GetSelectedIndex();
                        uint skinIdByIndex = CSkinInfo.GetSkinIdByIndex(this.m_selectHeroID, selectedIndex);
                        uint skinCfgId = CSkinInfo.GetSkinCfgId(this.m_selectHeroID, skinIdByIndex);
                        int skinIndexByConfigId = Singleton<CMallSystem>.GetInstance().m_skinMallCtrl.GetSkinIndexByConfigId(skinCfgId);
                        if (skinIndexByConfigId > 0)
                        {
                            int num11 = skinIndexByConfigId - 1;
                            ResHeroSkin skinDataByIndex = Singleton<CMallSystem>.GetInstance().m_skinMallCtrl.GetSkinDataByIndex(num11);
                            if (skinDataByIndex != null)
                            {
                                uint heroId = 0;
                                uint num13 = 0;
                                CSkinInfo.ResolveHeroSkin(skinDataByIndex.dwID, out heroId, out num13);
                                if (heroId == 0)
                                {
                                    return;
                                }
                                this.m_selectHeroData = CHeroDataFactory.CreateHeroData(heroId);
                                this.SwitchHeroInfo(this.m_selectHeroData.cfgID, num13, false, false);
                                Singleton<CSoundManager>.GetInstance().PostEvent("UI_hero_page", null);
                            }
                        }
                        break;
                    }
                    return;
                }
            }
            this.SetVictoryTipsEvt();
        }

        public void OnHeroInfo_TurnRight(CUIEvent uiEvent)
        {
            switch (this.m_OpenSrc)
            {
                case enHeroFormOpenSrc.HeroListClick:
                {
                    int heroIndexByConfigId = Singleton<CHeroOverviewSystem>.GetInstance().GetHeroIndexByConfigId(this.m_selectHeroID);
                    if (heroIndexByConfigId < (Singleton<CHeroOverviewSystem>.GetInstance().GetHeroListCount() - 1))
                    {
                        int index = heroIndexByConfigId + 1;
                        this.m_selectHeroData = Singleton<CHeroOverviewSystem>.GetInstance().GetHeroDataBuyIndex(index);
                        if (this.m_selectHeroData != null)
                        {
                            uint heroWearSkinId = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetHeroWearSkinId(this.m_selectHeroData.cfgID);
                            this.SwitchHeroInfo(this.m_selectHeroData.cfgID, heroWearSkinId, true, true);
                            Singleton<CSoundManager>.GetInstance().PostEvent("UI_hero_page", null);
                        }
                    }
                    break;
                }
                case enHeroFormOpenSrc.HeroBuyClick:
                {
                    int num4 = Singleton<CMallSystem>.GetInstance().m_heroMallCtrl.GetHeroIndexByConfigId(this.m_selectHeroID);
                    if (num4 < (Singleton<CMallSystem>.GetInstance().m_heroMallCtrl.GetHeroListCount() - 1))
                    {
                        int num5 = num4 + 1;
                        this.m_selectHeroData = Singleton<CMallSystem>.GetInstance().m_heroMallCtrl.GetHeroDataByIndex(num5);
                        if (this.m_selectHeroData != null)
                        {
                            uint skinId = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetHeroWearSkinId(this.m_selectHeroData.cfgID);
                            this.SwitchHeroInfo(this.m_selectHeroData.cfgID, skinId, true, true);
                            Singleton<CSoundManager>.GetInstance().PostEvent("UI_hero_page", null);
                        }
                    }
                    break;
                }
                case enHeroFormOpenSrc.SkinBuyClick:
                {
                    CUIListScript component = this.m_heroInfoForm.GetWidget(2).get_transform().Find("List_Skin").GetComponent<CUIListScript>();
                    if (component != null)
                    {
                        int selectedIndex = component.GetSelectedIndex();
                        uint skinIdByIndex = CSkinInfo.GetSkinIdByIndex(this.m_selectHeroID, selectedIndex);
                        uint skinCfgId = CSkinInfo.GetSkinCfgId(this.m_selectHeroID, skinIdByIndex);
                        int skinIndexByConfigId = Singleton<CMallSystem>.GetInstance().m_skinMallCtrl.GetSkinIndexByConfigId(skinCfgId);
                        if (skinIndexByConfigId < (Singleton<CMallSystem>.GetInstance().m_skinMallCtrl.GetSkinListCount() - 1))
                        {
                            int num11 = skinIndexByConfigId + 1;
                            ResHeroSkin skinDataByIndex = Singleton<CMallSystem>.GetInstance().m_skinMallCtrl.GetSkinDataByIndex(num11);
                            if (skinDataByIndex != null)
                            {
                                uint heroId = 0;
                                uint num13 = 0;
                                CSkinInfo.ResolveHeroSkin(skinDataByIndex.dwID, out heroId, out num13);
                                if (heroId == 0)
                                {
                                    return;
                                }
                                this.m_selectHeroData = CHeroDataFactory.CreateHeroData(heroId);
                                this.SwitchHeroInfo(this.m_selectHeroData.cfgID, num13, false, false);
                                Singleton<CSoundManager>.GetInstance().PostEvent("UI_hero_page", null);
                            }
                        }
                        break;
                    }
                    return;
                }
            }
            this.SetVictoryTipsEvt();
        }

        private void OnHeroInfoFormApper(CUIEvent uiEvent)
        {
            if (this.m_heroInfoForm != null)
            {
                GameObject widget = this.m_heroInfoForm.GetWidget(2);
                if (null != widget)
                {
                    int selectedIndex = widget.get_transform().Find("List_Skin").GetComponent<CUIListScript>().GetSelectedIndex();
                    if (this.m_curShow2DImage)
                    {
                        this.Refresh2DImage(this.m_selectHeroID, (int) CSkinInfo.GetSkinIdByIndex(this.m_selectHeroID, selectedIndex));
                    }
                    else
                    {
                        this.Refresh3DModel(uiEvent.m_srcFormScript.get_gameObject(), this.m_selectHeroID, (int) CSkinInfo.GetSkinIdByIndex(this.m_selectHeroID, selectedIndex), false);
                    }
                }
            }
        }

        private void OnHeroInfoGotoRankGod(CUIEvent uiEvent)
        {
            Singleton<RankingSystem>.instance.OpenRankingForm(RankingSystem.RankingType.God, this.m_selectHeroData.cfgID);
        }

        private void OnHeroSkillTipClose(CUIEvent uiEvent)
        {
            if (null != this.m_heroInfoForm)
            {
                this.m_heroInfoForm.GetWidget(5).CustomSetActive(false);
            }
        }

        private void OnHeroSkillTipOpen(CUIEvent uiEvent)
        {
            if (null != this.m_heroInfoForm)
            {
                GameObject widget = this.m_heroInfoForm.GetWidget(5);
                CUICommonSystem.RefreshSkillLevelUpProperty(Utility.FindChild(widget, "SkillPropertyInfo"), ref uiEvent.m_eventParams.skillPropertyDesc, uiEvent.m_eventParams.skillSlotId);
                Text component = widget.get_transform().Find("skillNameText").GetComponent<Text>();
                component.set_text(uiEvent.m_eventParams.skillTipParam.skillName);
                widget.get_transform().Find("SkillDescribeText").GetComponent<Text>().set_text(uiEvent.m_eventParams.skillTipParam.strTipText);
                Text text3 = widget.get_transform().Find("SkillCDText").GetComponent<Text>();
                text3.set_text(uiEvent.m_eventParams.skillTipParam.skillCoolDown);
                text3.get_transform().Find("SkillEnergyCostText").GetComponent<Text>().set_text(uiEvent.m_eventParams.skillTipParam.skillEnergyCost);
                ushort[] skillEffect = uiEvent.m_eventParams.skillTipParam.skillEffect;
                GameObject obj4 = null;
                for (int i = 1; i <= 2; i++)
                {
                    obj4 = component.get_transform().Find(string.Format("EffectNode{0}", i)).get_gameObject();
                    if ((i <= skillEffect.Length) && (skillEffect[i - 1] != 0))
                    {
                        obj4.CustomSetActive(true);
                        obj4.GetComponent<Image>().SetSprite(CSkillData.GetEffectSlotBg((SkillEffectType) skillEffect[i - 1]), uiEvent.m_srcFormScript, true, false, false, false);
                        obj4.get_transform().Find("Text").GetComponent<Text>().set_text(CSkillData.GetEffectDesc((SkillEffectType) skillEffect[i - 1]));
                    }
                    else
                    {
                        obj4.CustomSetActive(false);
                    }
                }
                widget.CustomSetActive(true);
            }
        }

        private void OnHeroSkin_ItemEnable(CUIEvent uiEvent)
        {
            if (this.m_heroInfoForm != null)
            {
                GameObject widget = this.m_heroInfoForm.GetWidget(2);
                if (null != widget)
                {
                    int heroSkinCnt = CSkinInfo.GetHeroSkinCnt(this.m_selectHeroID);
                    int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
                    if ((srcWidgetIndexInBelongedList >= 0) && (srcWidgetIndexInBelongedList < heroSkinCnt))
                    {
                        ListView<ResHeroSkin> availableSkinByHeroId = CSkinInfo.GetAvailableSkinByHeroId(this.m_selectHeroID);
                        int selectedIndex = widget.get_transform().Find("List_Skin").GetComponent<CUIListScript>().GetSelectedIndex();
                        this.SetSkinListItem(uiEvent.m_srcWidget, availableSkinByHeroId[srcWidgetIndexInBelongedList], srcWidgetIndexInBelongedList == selectedIndex);
                    }
                }
            }
        }

        public void OnHeroSkin_ItemSelect(CUIEvent uiEvent)
        {
            if (null != this.m_heroInfoForm)
            {
                CUIListScript component = this.m_heroInfoForm.GetWidget(2).get_transform().Find("List_Skin").GetComponent<CUIListScript>();
                CUIListElementScript lastSelectedElement = component.GetLastSelectedElement();
                CUIListElementScript selectedElement = component.GetSelectedElement();
                int lastSelectedIndex = component.GetLastSelectedIndex();
                ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(this.m_selectHeroID, CSkinInfo.GetSkinIdByIndex(this.m_selectHeroID, lastSelectedIndex));
                if (lastSelectedElement != null)
                {
                    this.SetSkinListItem(lastSelectedElement.get_gameObject(), heroSkin, false);
                }
                int selectedIndex = component.GetSelectedIndex();
                if (selectedIndex != -1)
                {
                    uint skinIdByIndex = CSkinInfo.GetSkinIdByIndex(this.m_selectHeroID, selectedIndex);
                    heroSkin = CSkinInfo.GetHeroSkin(this.m_selectHeroID, skinIdByIndex);
                    if (selectedElement != null)
                    {
                        this.SetSkinListItem(selectedElement.get_gameObject(), heroSkin, true);
                    }
                    this.RefreshSkinFeaturePanel(this.m_selectHeroID, skinIdByIndex);
                    if (this.m_curShow2DImage)
                    {
                        this.Refresh2DImage(this.m_selectHeroID, (int) skinIdByIndex);
                    }
                    else
                    {
                        this.Refresh3DModel(uiEvent.m_srcFormScript.get_gameObject(), this.m_selectHeroID, (int) skinIdByIndex, true);
                        Singleton<CHeroAnimaSystem>.GetInstance().OnModePlayAnima("Come");
                    }
                    this.UpdateSwitchButton(this.m_heroInfoForm);
                    this.m_heroInfoForm.get_transform().Find(s_heroLeftPanel).get_gameObject().get_transform().Find("heroInfoPanel/heroTitleText").GetComponent<Text>().set_text(CHeroInfo.GetHeroTitle(this.m_selectHeroID, skinIdByIndex));
                }
            }
        }

        public void OnHeroSkin_Wear(CUIEvent uiEvent)
        {
            uint heroId = uiEvent.m_eventParams.heroSkinParam.heroId;
            uint skinId = uiEvent.m_eventParams.heroSkinParam.skinId;
            ReqWearHeroSkin(heroId, skinId, false);
        }

        public void OnHeroSkinBuySuc(uint heroId)
        {
            if ((this.m_heroInfoForm != null) && (heroId == this.m_selectHeroID))
            {
                this.RefreshSkinPanel();
            }
        }

        public void OnHeroSkinWearSuc(uint heroId, uint skinId)
        {
            if ((this.m_heroInfoForm != null) && (heroId == this.m_selectHeroID))
            {
                this.RefreshSkinPanel();
            }
        }

        private void OnHeroUnlockPvP(string heroName)
        {
            Singleton<CUIManager>.GetInstance().OpenTips(string.Format("PVP_Hero_Unlock_Tip", true, heroName), false, 1.5f, null, new object[0]);
        }

        public void OnNtyAddHero(uint id)
        {
            if (this.m_selectHeroID != 0)
            {
                this.m_selectHeroData = CHeroDataFactory.CreateHeroData(this.m_selectHeroID);
            }
            this.RefreshHeroInfoForm();
        }

        private void OnOpenCustomEquipPanel(CUIEvent uiEvent)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            DebugHelper.Assert(masterRoleInfo != null, "OnOpenCustomEquipPanel role is null");
            if (masterRoleInfo != null)
            {
                masterRoleInfo.m_customRecommendEquipsLastChangedHeroID = this.m_selectHeroID;
                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.CustomEquip_OpenForm);
            }
        }

        private void OnShow2DImage(CUIEvent uiEvent)
        {
            int selectedIndex = this.m_heroInfoForm.GetWidget(2).get_transform().Find("List_Skin").GetComponent<CUIListScript>().GetSelectedIndex();
            string str = string.Format("30{0}{1:D1}", this.m_selectHeroID, selectedIndex);
            CBinaryObject content = Singleton<CResourceManager>.GetInstance().GetResource("UGUI/Sprite/Dynamic/BustHeroLarge/" + str + ".bytes", typeof(TextAsset), enResourceType.UISprite, false, false).m_content as CBinaryObject;
            if (content == null)
            {
                Singleton<CUIManager>.GetInstance().OpenTips("HeroInfo_2DImageNotComplete", true, 1.5f, null, new object[0]);
            }
            else
            {
                this.Refresh2DImage(this.m_selectHeroID, (int) CSkinInfo.GetSkinIdByIndex(this.m_selectHeroID, selectedIndex));
            }
        }

        private void OnShow3DImage(CUIEvent uiEvent)
        {
            int selectedIndex = this.m_heroInfoForm.GetWidget(2).get_transform().Find("List_Skin").GetComponent<CUIListScript>().GetSelectedIndex();
            this.Refresh3DModel(uiEvent.m_srcFormScript.get_gameObject(), this.m_selectHeroID, (int) CSkinInfo.GetSkinIdByIndex(this.m_selectHeroID, selectedIndex), true);
            Singleton<CHeroAnimaSystem>.GetInstance().OnModePlayAnima("Come");
        }

        private void OnViewHeroProperty(CUIEvent uiEvent)
        {
            CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(s_heroPropertyFormPath, false, true);
            if (script != null)
            {
                CHeroInfo info2;
                Transform transform = script.get_transform().Find("Panel/Panel_Proterty").get_transform();
                GameObject root = transform.Find("Panel_BaseProp").get_gameObject();
                GameObject obj3 = transform.Find("Panel_AtkProp").get_gameObject();
                GameObject obj4 = transform.Find("Panel_DefProp").get_gameObject();
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                ValueDataInfo[] info = null;
                int level = 1;
                if ((masterRoleInfo != null) && masterRoleInfo.GetHeroInfo(this.m_selectHeroID, out info2, false))
                {
                    DebugHelper.Assert((info2 != null) && (info2.mActorValue != null));
                    if ((info2 != null) && (info2.mActorValue != null))
                    {
                        info = info2.mActorValue.GetActorValue();
                        level = info2.mActorValue.SoulLevel;
                    }
                }
                else
                {
                    ActorMeta meta2 = new ActorMeta();
                    meta2.ConfigId = (int) this.m_selectHeroID;
                    ActorMeta theActorMeta = meta2;
                    PropertyHelper helper = new PropertyHelper();
                    helper.InitValueDataArr(ref theActorMeta, true);
                    info = helper.GetActorValue();
                    level = helper.SoulLevel;
                }
                if (info != null)
                {
                    RefreshBasePropPanel(root, ref info, level, this.m_selectHeroID);
                    RefreshAtkPropPanel(obj3, ref info, level, this.m_selectHeroID);
                    RefreshDefPropPanel(obj4, ref info, level, this.m_selectHeroID);
                }
            }
        }

        protected void OnViewHeroStory(CUIEvent uiEvent)
        {
            ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(this.m_selectHeroID);
            if (dataByKey != null)
            {
                CUICommonSystem.OpenUrl(dataByKey.szStoryUrl, true, 0);
            }
        }

        [MessageHandler(0x71e)]
        public static void OnWearHeroSkinRsp(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            SCPKG_WEARHEROSKIN_RSP stWearHeroSkinRsp = msg.stPkgData.stWearHeroSkinRsp;
            if (stWearHeroSkinRsp.iResult == 0)
            {
                Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().OnWearHeroSkin(stWearHeroSkinRsp.dwHeroID, stWearHeroSkinRsp.dwSkinID);
                Singleton<CHeroInfoSystem2>.GetInstance().OnHeroSkinWearSuc(stWearHeroSkinRsp.dwHeroID, stWearHeroSkinRsp.dwSkinID);
                Singleton<CHeroSelectBaseSystem>.GetInstance().OnHeroSkinWearSuc(stWearHeroSkinRsp.dwHeroID, stWearHeroSkinRsp.dwSkinID);
            }
            else
            {
                CS_WEARHEROSKIN_ERRCODE iResult = (CS_WEARHEROSKIN_ERRCODE) stWearHeroSkinRsp.iResult;
                CTextManager instance = Singleton<CTextManager>.GetInstance();
                switch (iResult)
                {
                    case CS_WEARHEROSKIN_ERRCODE.CS_WEARHEROSKIN_NOOWNEDHERO:
                        Singleton<CUIManager>.GetInstance().OpenTips(instance.GetText("Hero_SkinWear_Hero_Not_Exist"), false, 1.5f, null, new object[0]);
                        break;

                    case CS_WEARHEROSKIN_ERRCODE.CS_WEARHEROSKIN_NOOWNEDSKIN:
                        Singleton<CUIManager>.GetInstance().OpenTips(instance.GetText("Hero_SkinWear_Skin_Not_Exist"), false, 1.5f, null, new object[0]);
                        return;
                }
            }
        }

        [MessageHandler(0x10ce)]
        public static void ReceiveMaterialDirectBuy(CSPkg msg)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x56).dwConfValue;
                masterRoleInfo.MaterialDirectBuyLimit = (byte) (dwConfValue - msg.stPkgData.stDirectBuyItemRsp.dwDirectBuyItemCnt);
            }
        }

        public void Refresh2DImage(uint heroID, int skinId)
        {
            this.ColloctMem(heroID);
            Image component = this.m_heroInfoForm.GetWidget(14).GetComponent<Image>();
            string str = string.Format("30{0}{1:D1}", this.m_selectHeroID, skinId);
            CBinaryObject content = Singleton<CResourceManager>.GetInstance().GetResource("UGUI/Sprite/Dynamic/BustHeroLarge/" + str + ".bytes", typeof(TextAsset), enResourceType.UISprite, false, false).m_content as CBinaryObject;
            if (content == null)
            {
                this.Refresh3DModel(this.m_heroInfoForm.get_gameObject(), heroID, skinId, true);
                Singleton<CHeroAnimaSystem>.GetInstance().OnModePlayAnima("Come");
            }
            else
            {
                byte[] data = content.m_data;
                Texture2D textured = new Texture2D(0, 0, 3, false);
                textured.LoadImage(data);
                Sprite sprite = Sprite.Create(textured, new Rect(0f, 0f, (float) textured.get_width(), (float) textured.get_height()), new Vector2(0.5f, 0.5f));
                component.set_sprite(sprite);
                this.m_heroImgPath = str;
                this.SetCurModelShowType(true);
                Singleton<CSoundManager>.GetInstance().PostEvent("Stop_Show", null);
            }
        }

        public void Refresh3DModel(GameObject root, uint heroID, int skinId, bool bInitAnima = false)
        {
            this.ColloctMem(heroID);
            GameObject obj3 = root.get_transform().Find(s_heroLeftPanel).get_gameObject().get_transform().Find("3DImage").get_gameObject();
            DebugHelper.Assert(obj3 != null);
            if (obj3 != null)
            {
                CUI3DImageScript component = obj3.GetComponent<CUI3DImageScript>();
                ObjNameData data = CUICommonSystem.GetHeroPrefabPath(heroID, skinId, true);
                if (!string.IsNullOrEmpty(this.m_heroModelPath))
                {
                    component.RemoveGameObject(this.m_heroModelPath);
                }
                this.m_heroModelPath = data.ObjectName;
                GameObject model = component.AddGameObject(this.m_heroModelPath, false, false);
                CHeroAnimaSystem instance = Singleton<CHeroAnimaSystem>.GetInstance();
                instance.Set3DModel(model);
                if (model != null)
                {
                    model.get_transform().set_rotation(Quaternion.Euler(0f, 180f, 0f));
                    if (data.ActorInfo != null)
                    {
                        model.get_transform().set_localScale(new Vector3(data.ActorInfo.LobbyScale, data.ActorInfo.LobbyScale, data.ActorInfo.LobbyScale));
                    }
                    if (bInitAnima)
                    {
                        instance.InitAnimatList();
                        instance.InitAnimatSoundList(this.m_selectHeroID, (uint) skinId);
                    }
                    this.SetCurModelShowType(false);
                }
            }
        }

        public static void RefreshAtkPropPanel(GameObject root, ref ValueDataInfo[] info, int level, uint heroId)
        {
            Transform transform = root.get_transform();
            Text component = transform.Find("TextL1").GetComponent<Text>();
            Text text2 = transform.Find("TextR1").GetComponent<Text>();
            Text text3 = transform.Find("TextL2").GetComponent<Text>();
            Text text4 = transform.Find("TextR2").GetComponent<Text>();
            Text text5 = transform.Find("TextL3").GetComponent<Text>();
            Text text6 = transform.Find("TextR3").GetComponent<Text>();
            Text text7 = transform.Find("TextL4").GetComponent<Text>();
            Text text8 = transform.Find("TextR4").GetComponent<Text>();
            Text text9 = transform.Find("TextL5").GetComponent<Text>();
            Text text10 = transform.Find("TextR5").GetComponent<Text>();
            Text text11 = transform.Find("TextL6").GetComponent<Text>();
            Text text12 = transform.Find("TextR6").GetComponent<Text>();
            Text text13 = transform.Find("TextL7").GetComponent<Text>();
            Text text14 = transform.Find("TextR7").GetComponent<Text>();
            Text text15 = transform.Find("TextL8").GetComponent<Text>();
            Text text16 = transform.Find("TextR8").GetComponent<Text>();
            Text text17 = transform.Find("TextL9").GetComponent<Text>();
            Text text18 = transform.Find("TextR9").GetComponent<Text>();
            Text text19 = transform.Find("TextL10").GetComponent<Text>();
            Text text20 = transform.Find("TextR10").GetComponent<Text>();
            int totalValue = 0;
            int percent = 0;
            ResBattleParam anyData = GameDataMgr.battleParam.GetAnyData();
            component.set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_MoveSpd"));
            MonoSingleton<GlobalConfig>.instance.bOnExternalSpeedPicker = true;
            text2.set_text(GetFormStr((float) (info[15].basePropertyValue / 10), (float) (info[15].extraPropertyValue / 10)));
            MonoSingleton<GlobalConfig>.instance.bOnExternalSpeedPicker = false;
            text3.set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_PhyArmorHurt"));
            text4.set_text(string.Format("{0}|{1}", GetFormStr((float) info[7].baseValue, (float) info[7].extraPropertyValue), GetFormPercentStr(info[0x22].totalValue, info[0x22].extraPropertyValue > 0)));
            text5.set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_MgcArmorHurt"));
            text6.set_text(string.Format("{0}|{1}", GetFormStr((float) info[8].baseValue, (float) info[8].extraPropertyValue), GetFormPercentStr(info[0x23].totalValue, info[0x23].extraPropertyValue > 0)));
            totalValue = info[0x1c].totalValue;
            percent = ((int) ((0x2710 * totalValue) / ((totalValue + (level * anyData.dwM_AttackSpeed)) + anyData.dwN_AttackSpeed))) + info[0x12].totalValue;
            text7.set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_AtkSpdLvl"));
            text8.set_text(GetFormPercentStr(percent, info[0x12].extraPropertyValue > 0));
            totalValue = info[0x18].totalValue;
            percent = ((int) ((0x2710 * totalValue) / ((totalValue + (level * anyData.dwM_Critical)) + anyData.dwN_Critical))) + info[6].totalValue;
            text9.set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_CritLvl"));
            text10.set_text(GetFormPercentStr(percent, info[6].extraPropertyValue > 0));
            percent = info[12].totalValue + 0x2710;
            text11.set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_CritEft"));
            text12.set_text(GetFormPercentStr(percent, info[12].extraPropertyValue > 0));
            totalValue = info[0x1a].totalValue;
            percent = ((int) ((0x2710 * totalValue) / ((totalValue + (level * anyData.dwM_PhysicsHemophagia)) + anyData.dwN_PhysicsHemophagia))) + info[9].totalValue;
            text13.set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_PhyVampLvl"));
            text14.set_text(GetFormPercentStr(percent, info[9].extraPropertyValue > 0));
            totalValue = info[0x1b].totalValue;
            percent = ((int) ((0x2710 * totalValue) / ((totalValue + (level * anyData.dwM_MagicHemophagia)) + anyData.dwN_MagicHemophagia))) + info[10].totalValue;
            text15.set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_MgcVampLvl"));
            text16.set_text(GetFormPercentStr(percent, info[10].extraPropertyValue > 0));
            text17.set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_CdReduce"));
            percent = info[20].totalValue;
            text18.set_text(GetFormPercentStr(percent, info[20].extraPropertyValue > 0));
            ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(heroId);
            if (dataByKey != null)
            {
                text19.set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_BaseAtkRange"));
                text20.set_text(Utility.UTF8Convert(dataByKey.szAttackRangeDesc));
            }
            else
            {
                text19.set_text(string.Empty);
                text20.set_text(string.Empty);
            }
        }

        private void RefreshBaseInfoPanel()
        {
            if (this.m_heroInfoForm != null)
            {
                GameObject widget = this.m_heroInfoForm.GetWidget(0);
                if (null != widget)
                {
                    ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(this.m_selectHeroID);
                    Transform transform = widget.get_transform();
                    transform.Find("jobTitleTxt").GetComponent<Text>().set_text(CHeroInfo.GetHeroJob(this.m_selectHeroID));
                    transform.Find("jobFeatureTxt").GetComponent<Text>().set_text(CHeroInfo.GetJobFeature(this.m_selectHeroID));
                    bool bActive = this.m_selectHeroData.bPlayerOwn && ((this.m_selectHeroData.proficiencyLV > 1) || (this.m_selectHeroData.proficiency > 0));
                    GameObject obj3 = transform.Find("heroProficiencyImg").get_gameObject();
                    Text component = transform.Find("proficiencyLevel").GetComponent<Text>();
                    Text text4 = transform.Find("proficiencyValTxt").GetComponent<Text>();
                    obj3.CustomSetActive(bActive);
                    component.get_gameObject().CustomSetActive(bActive);
                    if (bActive)
                    {
                        ResHeroProficiency heroProficiency = CHeroInfo.GetHeroProficiency(this.m_selectHeroData.heroType, this.m_selectHeroData.proficiencyLV);
                        if (heroProficiency != null)
                        {
                            component.set_text(heroProficiency.szTitle);
                            int maxProficiency = CHeroInfo.GetMaxProficiency();
                            if (this.m_selectHeroData.proficiencyLV >= maxProficiency)
                            {
                                text4.set_text(this.m_selectHeroData.proficiency.ToString());
                            }
                            else
                            {
                                text4.set_text(string.Format("{0}/{1}", this.m_selectHeroData.proficiency, heroProficiency.dwTopPoint));
                            }
                            CUICommonSystem.SetHeroProficiencyIconImage(this.m_heroInfoForm, obj3, this.m_selectHeroData.proficiencyLV);
                        }
                    }
                    else
                    {
                        text4.set_text("暂无");
                    }
                    GameObject progressBar = transform.Find("viabilityBar").get_gameObject();
                    transform.Find("viabilityText").GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_Viability"));
                    CUICommonSystem.SetProgressBarData(progressBar, (int) dataByKey.iViability, s_maxBasePropVal, false);
                    transform.Find("phyDamageText").GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_AtkDamage"));
                    CUICommonSystem.SetProgressBarData(transform.Find("phyDamageBar").get_gameObject(), (int) dataByKey.iPhyDamage, s_maxBasePropVal, false);
                    transform.Find("spellDamageText").GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_SkillFunc"));
                    CUICommonSystem.SetProgressBarData(transform.Find("spellDamageBar").get_gameObject(), (int) dataByKey.iSpellDamage, s_maxBasePropVal, false);
                    transform.Find("startedDiffText").GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_StartedDifficulty"));
                    CUICommonSystem.SetProgressBarData(transform.Find("startedDiffBar").get_gameObject(), dataByKey.iStartedDifficulty, s_maxBasePropVal, false);
                }
            }
        }

        public static void RefreshBasePropPanel(GameObject root, ref ValueDataInfo[] info, int level, uint heroId)
        {
            Transform transform = root.get_transform();
            Text component = transform.Find("TextL1").GetComponent<Text>();
            Text text2 = transform.Find("TextR1").GetComponent<Text>();
            Text text3 = transform.Find("TextL2").GetComponent<Text>();
            Text text4 = transform.Find("TextR2").GetComponent<Text>();
            Text text5 = transform.Find("TextL3").GetComponent<Text>();
            Text text6 = transform.Find("TextR3").GetComponent<Text>();
            Text text7 = transform.Find("TextL4").GetComponent<Text>();
            Text text8 = transform.Find("TextR4").GetComponent<Text>();
            Text text9 = transform.Find("TextL5").GetComponent<Text>();
            Text text10 = transform.Find("TextR5").GetComponent<Text>();
            Text text11 = transform.Find("TextL6").GetComponent<Text>();
            Text text12 = transform.Find("TextR6").GetComponent<Text>();
            Text text13 = transform.Find("TextL7").GetComponent<Text>();
            Text text14 = transform.Find("TextR7").GetComponent<Text>();
            Text text15 = transform.Find("TextL8").GetComponent<Text>();
            Text text16 = transform.Find("TextR8").GetComponent<Text>();
            component.set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_MaxHp"));
            text2.set_text(GetFormStr((float) info[5].basePropertyValue, (float) info[5].extraPropertyValue));
            text3.set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_PhyAtkPt"));
            text4.set_text(GetFormStr((float) info[1].basePropertyValue, (float) info[1].extraPropertyValue));
            text5.set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_MgcAtkPt"));
            text6.set_text(GetFormStr((float) info[2].basePropertyValue, (float) info[2].extraPropertyValue));
            ResBattleParam anyData = GameDataMgr.battleParam.GetAnyData();
            int totalValue = info[3].totalValue;
            int percent = (int) ((totalValue * 0x2710) / ((totalValue + (level * anyData.dwM_PhysicsDefend)) + anyData.dwN_PhysicsDefend));
            text7.set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_PhyDefPt"));
            text8.set_text(string.Format("{0}|{1}", GetFormStr((float) info[3].basePropertyValue, (float) info[3].extraPropertyValue), GetFormPercentStr(percent, info[3].extraPropertyValue > 0)));
            totalValue = info[4].totalValue;
            percent = (int) ((totalValue * 0x2710) / ((totalValue + (level * anyData.dwM_MagicDefend)) + anyData.dwN_MagicDefend));
            text9.set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_MgcDefPt"));
            text10.set_text(string.Format("{0}|{1}", GetFormStr((float) info[4].basePropertyValue, (float) info[4].extraPropertyValue), GetFormPercentStr(percent, info[4].extraPropertyValue > 0)));
            ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(heroId);
            uint energyType = (dataByKey != null) ? dataByKey.dwEnergyType : 0;
            text15.set_text(Singleton<CTextManager>.GetInstance().GetText(EnergyCommon.GetEnergyShowText(energyType, EnergyShowType.MaxValue)));
            text16.set_text(GetFormStr((float) info[0x20].basePropertyValue, (float) info[0x20].extraPropertyValue));
        }

        private void RefreshBuyHeroPanel()
        {
            if (null != this.m_heroInfoForm)
            {
                GameObject widget = this.m_heroInfoForm.GetWidget(3);
                bool bPlayerOwn = this.m_selectHeroData.bPlayerOwn;
                widget.CustomSetActive(!bPlayerOwn);
                if (!bPlayerOwn)
                {
                    ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(this.m_selectHeroID);
                    object[] inParameters = new object[] { this.m_selectHeroID };
                    DebugHelper.Assert(dataByKey != null, "Failed Find Hero Cfg {0}", inParameters);
                    if (dataByKey != null)
                    {
                        ResHeroShop shop = null;
                        GameDataMgr.heroShopInfoDict.TryGetValue(dataByKey.dwCfgID, out shop);
                        if (CSysDynamicBlock.bLobbyEntryBlocked && (shop != null))
                        {
                            shop.bIsBuyCoin = 1;
                            shop.bIsBuyCoupons = 1;
                        }
                        GameObject obj3 = widget.get_transform().Find("buyBtn").get_gameObject();
                        obj3.CustomSetActive(true);
                        Text component = widget.get_transform().Find("getWayText").GetComponent<Text>();
                        component.get_gameObject().CustomSetActive(false);
                        ResHeroPromotion resPromotion = CHeroDataFactory.CreateHeroData(this.m_selectHeroID).promotion();
                        stPayInfoSet payInfoSet = new stPayInfoSet();
                        payInfoSet = CMallSystem.GetPayInfoSetOfGood(dataByKey, resPromotion);
                        CHeroSkinBuyManager.SetHeroBuyPricePanel(this.m_heroInfoForm, widget, ref payInfoSet, this.m_selectHeroID, enUIEventID.None);
                        if (payInfoSet.m_payInfoCount <= 0)
                        {
                            obj3.CustomSetActive(false);
                            component.get_gameObject().CustomSetActive(true);
                            if (shop != null)
                            {
                                component.set_text(StringHelper.UTF8BytesToString(ref shop.szObtWay));
                            }
                            else
                            {
                                component.set_text(null);
                            }
                        }
                        if (obj3.get_activeSelf())
                        {
                            CUIEventScript script = obj3.GetComponent<CUIEventScript>();
                            stUIEventParams eventParams = new stUIEventParams();
                            eventParams.heroId = this.m_selectHeroID;
                            script.SetUIEvent(enUIEventType.Click, enUIEventID.HeroInfo_OpenBuyHeroForm, eventParams);
                        }
                    }
                }
            }
        }

        public static void RefreshDefPropPanel(GameObject root, ref ValueDataInfo[] info, int level, uint heroId)
        {
            Transform transform = root.get_transform();
            Text component = transform.Find("TextL1").GetComponent<Text>();
            Text text2 = transform.Find("TextR1").GetComponent<Text>();
            Text text3 = transform.Find("TextL2").GetComponent<Text>();
            Text text4 = transform.Find("TextR2").GetComponent<Text>();
            Text text5 = transform.Find("TextL3").GetComponent<Text>();
            Text text6 = transform.Find("TextR3").GetComponent<Text>();
            int totalValue = 0;
            int percent = 0;
            ResBattleParam anyData = GameDataMgr.battleParam.GetAnyData();
            component.set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_CtrlReduceLvl"));
            totalValue = info[0x1d].totalValue;
            percent = ((int) ((0x2710 * totalValue) / ((totalValue + (level * anyData.dwM_Tenacity)) + anyData.dwN_Tenacity))) + info[0x11].totalValue;
            text2.set_text(GetFormPercentStr(percent, info[0x11].extraPropertyValue > 0));
            text3.set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_HpRecover"));
            totalValue = info[0x10].totalValue;
            string str = string.Format(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_HpRecover_Desc"), totalValue);
            text4.set_text(GetFormStr((float) info[0x10].basePropertyValue, (float) info[0x10].extraPropertyValue));
            ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(heroId);
            uint energyType = (dataByKey != null) ? dataByKey.dwEnergyType : 0;
            text5.set_text(Singleton<CTextManager>.GetInstance().GetText(EnergyCommon.GetEnergyShowText(energyType, EnergyShowType.RecoverValue)));
            totalValue = info[0x21].totalValue;
            string str2 = string.Format(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_EpRecover_Desc"), totalValue);
            text6.set_text(GetFormStr((float) info[0x21].basePropertyValue, (float) info[0x21].extraPropertyValue));
        }

        private void RefreshExperiencePanel()
        {
            if (null != this.m_heroInfoForm)
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                GameObject widget = this.m_heroInfoForm.GetWidget(6);
                GameObject obj3 = this.m_heroInfoForm.GetWidget(8);
                widget.CustomSetActive(false);
                obj3.CustomSetActive(false);
                if (masterRoleInfo.IsValidExperienceHero(this.m_selectHeroID))
                {
                    CUICommonSystem.RefreshExperienceHeroLeftTime(widget, obj3, this.m_selectHeroID);
                }
            }
        }

        public void RefreshHeroInfoForm()
        {
            if (this.m_heroInfoForm != null)
            {
                this.RefreshBuyHeroPanel();
                this.RefreshModelBaseInfo();
                this.RefreshExperiencePanel();
                this.RefreshRightPanel();
            }
        }

        private void RefreshHeroSkinPrice(CUIFormScript formScript, Transform pricePanelTransform, uint skinId, ref stPayInfoSet payInfoSet)
        {
            if (pricePanelTransform != null)
            {
                CMallSystem.SetSkinBuyPricePanel(formScript, pricePanelTransform, ref payInfoSet.m_payInfos[0]);
            }
        }

        private void RefreshModelBaseInfo()
        {
            if (this.m_heroInfoForm != null)
            {
                GameObject widget = this.m_heroInfoForm.GetWidget(4);
                Text component = widget.get_transform().Find("heroNameText").GetComponent<Text>();
                Text text2 = widget.get_transform().Find("heroTitleText").GetComponent<Text>();
                GameObject root = widget.get_transform().Find("jobImage").get_gameObject();
                CUICommonSystem.SetHeroJob(this.m_heroInfoForm, root, (enHeroJobType) this.m_selectHeroData.heroType);
                component.set_text(this.m_selectHeroData.heroName);
                if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo() != null)
                {
                }
            }
        }

        private void RefreshRightPanel()
        {
            if (this.m_heroInfoForm != null)
            {
                this.RefreshBaseInfoPanel();
                this.RefreshSkillPanel();
                this.RefreshSkinPanel();
            }
        }

        private void RefreshSkillPanel()
        {
            if (this.m_heroInfoForm != null)
            {
                GameObject widget = this.m_heroInfoForm.GetWidget(1);
                if (null != widget)
                {
                    CUIListScript component = widget.get_transform().Find("List").get_gameObject().GetComponent<CUIListScript>();
                    component.SetElementAmount(4);
                    for (int i = 0; i < 4; i++)
                    {
                        CUIListElementScript elemenet = component.GetElemenet(i);
                        GameObject obj4 = elemenet.get_gameObject().get_transform().Find("Item_Skill").get_gameObject();
                        int iSkillID = this.m_selectHeroData.heroCfgInfo.astSkill[i].iSkillID;
                        ResSkillCfgInfo skillCfgInfo = CSkillData.GetSkillCfgInfo(iSkillID);
                        if (skillCfgInfo == null)
                        {
                            object[] inParameters = new object[] { iSkillID };
                            DebugHelper.Assert(false, "CHeroBaseInfoPanel.RefreshBaseDataPanel(): skillInfo is null, skillId={0}", inParameters);
                            break;
                        }
                        GameObject obj5 = obj4.get_transform().Find("skill_Icon").get_gameObject();
                        string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Skill_Dir, skillCfgInfo.szIconPath);
                        CUIUtility.SetImageSprite(obj5.GetComponent<Image>(), prefabPath, elemenet.m_belongedFormScript, true, false, false, false);
                        CUIEventScript script3 = obj4.GetComponent<CUIEventScript>();
                        stUIEventParams eventParams = new stUIEventParams();
                        eventParams.skillPropertyDesc = CUICommonSystem.ParseSkillLevelUpProperty(ref skillCfgInfo.astSkillPropertyDescInfo, this.m_selectHeroData.cfgID);
                        eventParams.skillSlotId = i;
                        eventParams.skillTipParam.skillName = skillCfgInfo.szSkillName;
                        eventParams.skillTipParam.strTipText = CUICommonSystem.GetSkillDescLobby(skillCfgInfo.szLobbySkillDesc, this.m_selectHeroData.cfgID);
                        eventParams.skillTipParam.skillCoolDown = (i != 0) ? Singleton<CTextManager>.instance.GetText("Skill_Cool_Down_Tips", new string[1]) : Singleton<CTextManager>.instance.GetText("Skill_Common_Effect_Type_5");
                        eventParams.skillTipParam.skillEnergyCost = (i != 0) ? Singleton<CTextManager>.instance.GetText(EnergyCommon.GetEnergyShowText(skillCfgInfo.bEnergyCostType, EnergyShowType.CostValue), new string[] { ((int) skillCfgInfo.iEnergyCost).ToString() }) : string.Empty;
                        eventParams.skillTipParam.skillEffect = skillCfgInfo.SkillEffectType;
                        script3.SetUIEvent(enUIEventType.Down, enUIEventID.HeroInfo_SkillTipOpen, eventParams);
                        script3.SetUIEvent(enUIEventType.HoldEnd, enUIEventID.HeroInfo_SkillTipClose, eventParams);
                        script3.SetUIEvent(enUIEventType.Click, enUIEventID.HeroInfo_SkillTipClose, eventParams);
                        script3.SetUIEvent(enUIEventType.DragEnd, enUIEventID.HeroInfo_SkillTipClose, eventParams);
                    }
                    component.SelectElement(0, true);
                }
            }
        }

        private void RefreshSkinFeaturePanel(uint heroId, uint skinId)
        {
            if (this.m_heroInfoForm != null)
            {
                GameObject widget = this.m_heroInfoForm.GetWidget(12);
                GameObject obj3 = this.m_heroInfoForm.GetWidget(0);
                GameObject obj4 = this.m_heroInfoForm.GetWidget(1);
                if (((null != widget) && (null != obj3)) && (null != obj4))
                {
                    Transform transform = widget.get_transform().Find("List");
                    if (transform != null)
                    {
                        CUIListScript component = transform.GetComponent<CUIListScript>();
                        int cnt = 0;
                        ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(heroId, skinId);
                        bool bActive = CSkinInfo.GetSkinFeatureCnt(heroId, skinId, out cnt);
                        widget.get_gameObject().CustomSetActive(bActive);
                        obj3.get_gameObject().CustomSetActive(!bActive);
                        obj4.get_gameObject().CustomSetActive(!bActive);
                        if (bActive)
                        {
                            component.SetElementAmount(cnt);
                            if (heroSkin != null)
                            {
                                for (int i = 0; i < cnt; i++)
                                {
                                    CUIListElementScript elemenet = component.GetElemenet(i);
                                    Transform transform2 = elemenet.get_transform().Find("featureImage");
                                    string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_SkinFeature_Dir, heroSkin.astFeature[i].szIconPath);
                                    transform2.GetComponent<Image>().SetSprite(prefabPath, this.m_heroInfoForm, true, false, false, false);
                                    elemenet.get_transform().Find("featureDescText").GetComponent<Text>().set_text(heroSkin.astFeature[i].szDesc);
                                }
                            }
                        }
                        else
                        {
                            component.SetElementAmount(0);
                        }
                    }
                }
            }
        }

        private void RefreshSkinPanel()
        {
            if (this.m_heroInfoForm != null)
            {
                GameObject widget = this.m_heroInfoForm.GetWidget(2);
                if (null != widget)
                {
                    CUIListScript component = widget.get_transform().Find("List_Skin").GetComponent<CUIListScript>();
                    ListView<ResHeroSkin> availableSkinByHeroId = CSkinInfo.GetAvailableSkinByHeroId(this.m_selectHeroID);
                    uint heroWearSkinId = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetHeroWearSkinId(this.m_selectHeroID);
                    int heroSkinCnt = CSkinInfo.GetHeroSkinCnt(this.m_selectHeroID);
                    component.SetElementAmount(heroSkinCnt);
                }
            }
        }

        public static void ReqWearHeroSkin(uint heroId, uint skinId, bool isSendGameSvr = false)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x71d);
            msg.stPkgData.stWearHeroSkinReq.dwHeroID = heroId;
            msg.stPkgData.stWearHeroSkinReq.dwSkinID = skinId;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public static void ResetDirectBuyLimit()
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x56).dwConfValue;
                masterRoleInfo.MaterialDirectBuyLimit = (byte) dwConfValue;
                Singleton<EventRouter>.instance.BroadCastEvent("MasterAttributesChanged");
            }
        }

        private void SelectSkinElement(uint heroId, uint skinId)
        {
            if (this.m_heroInfoForm != null)
            {
                GameObject widget = this.m_heroInfoForm.GetWidget(2);
                if (null != widget)
                {
                    int indexBySkinId = CSkinInfo.GetIndexBySkinId(heroId, skinId);
                    CUIListScript component = widget.get_transform().Find("List_Skin").GetComponent<CUIListScript>();
                    component.SelectElement(-1, true);
                    component.SelectElement(indexBySkinId, true);
                }
            }
        }

        private void SetBuyBtn(GameObject buyBtnObj, ResHeroSkin skinInfo)
        {
            buyBtnObj.CustomSetActive(true);
            CUIEventScript component = buyBtnObj.GetComponent<CUIEventScript>();
            stUIEventParams eventParams = new stUIEventParams();
            eventParams.heroSkinParam.heroId = skinInfo.dwHeroID;
            eventParams.heroSkinParam.skinId = skinInfo.dwSkinID;
            eventParams.heroSkinParam.isCanCharge = true;
            component.SetUIEvent(enUIEventType.Click, enUIEventID.HeroSkin_OpenBuySkinForm, eventParams);
        }

        private void SetCurModelShowType(bool curShow2DImage)
        {
            if (this.m_heroInfoForm != null)
            {
                this.m_curShow2DImage = curShow2DImage;
                GameObject widget = this.m_heroInfoForm.GetWidget(14);
                GameObject obj3 = this.m_heroInfoForm.GetWidget(15);
                widget.CustomSetActive(this.m_curShow2DImage);
                this.m_heroInfoForm.GetWidget(0x10).CustomSetActive(!this.m_curShow2DImage);
                this.m_heroInfoForm.GetWidget(0x11).CustomSetActive(this.m_curShow2DImage);
            }
        }

        private void SetPricePanel(GameObject pricePanelObj, ResHeroSkin skinInfo)
        {
            pricePanelObj.CustomSetActive(true);
            stPayInfoSet skinPayInfoSet = CSkinInfo.GetSkinPayInfoSet(skinInfo.dwHeroID, skinInfo.dwSkinID);
            this.RefreshHeroSkinPrice(this.m_heroInfoForm, pricePanelObj.get_transform(), skinInfo.dwSkinID, ref skinPayInfoSet);
        }

        protected void SetSkinListItem(GameObject listItem, ResHeroSkin skinInfo, bool bSelect)
        {
            if ((listItem != null) && (skinInfo != null))
            {
                Transform transform = listItem.get_transform().Find("skinItem");
                transform.Find("skinInfoPanel/skinNamePanel/skinNameText").GetComponent<Text>().set_text(StringHelper.UTF8BytesToString(ref skinInfo.szSkinName));
                transform.Find("skinInfoPanel/skinNamePanel/heroNameText").GetComponent<Text>().set_text(skinInfo.szHeroName);
                Text component = transform.Find("skinStateText").GetComponent<Text>();
                component.set_text(string.Empty);
                CUICommonSystem.SetHeroSkinLabelPic(this.m_heroInfoForm, transform.Find("skinLabelImage").get_gameObject(), skinInfo.dwHeroID, skinInfo.dwSkinID);
                Image image = transform.Find("skinIconImage").GetComponent<Image>();
                string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_BustHero_Dir, StringHelper.UTF8BytesToString(ref skinInfo.szSkinPicID));
                image.SetSprite(prefabPath, this.m_heroInfoForm, true, false, false, true);
                GameObject obj2 = transform.Find("buyButton").get_gameObject();
                GameObject obj3 = transform.Find("wearButton").get_gameObject();
                obj2.CustomSetActive(false);
                obj3.CustomSetActive(false);
                obj2.get_transform().Find("Text").GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("HeroInfo_BuyAndWear"));
                GameObject obj4 = transform.Find("skinInfoPanel/pricePanel").get_gameObject();
                obj4.CustomSetActive(false);
                GameObject txtSkinLeftTimeGo = transform.Find("txtSkinLeftTime").get_gameObject();
                GameObject timerGo = transform.Find("txtSkinLeftTime/Timer").get_gameObject();
                CUICommonSystem.RefreshExperienceSkinLeftTime(txtSkinLeftTimeGo, timerGo, skinInfo.dwHeroID, skinInfo.dwSkinID, Singleton<CTextManager>.GetInstance().GetText("ExpCard_ExpTime"));
                CTextManager instance = Singleton<CTextManager>.GetInstance();
                switch (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetHeroSkinState(this.m_selectHeroID, skinInfo.dwSkinID))
                {
                    case HeroSkinState.NormalHero_NormalSkin_Wear:
                    case HeroSkinState.LimitHero_NormalSkin_Wear:
                        this.SetSkinStateText(component, instance.GetText("Hero_SkinState_Wear"));
                        break;

                    case HeroSkinState.NormalHero_LimitSkin_Wear:
                        if (!CSkinInfo.IsCanBuy(skinInfo.dwHeroID, skinInfo.dwSkinID))
                        {
                            this.SetSkinStateText(component, GetSkinCannotBuyStr(skinInfo));
                            break;
                        }
                        this.SetBuyBtn(obj2, skinInfo);
                        this.SetSkinStateText(component, instance.GetText("Avatar_hero_buybutton"));
                        break;

                    case HeroSkinState.NormalHero_NormalSkin_Own:
                    case HeroSkinState.LimitHero_NormalSkin_Own:
                        if (bSelect)
                        {
                            this.SetWearBtn(obj3, skinInfo);
                        }
                        this.SetSkinStateText(component, instance.GetText("Hero_SkinState_Own"));
                        break;

                    case HeroSkinState.NormalHero_LimitSkin_Own:
                        if (bSelect)
                        {
                            this.SetWearBtn(obj3, skinInfo);
                        }
                        if (CSkinInfo.IsCanBuy(skinInfo.dwHeroID, skinInfo.dwSkinID))
                        {
                            this.SetPricePanel(obj4, skinInfo);
                        }
                        else
                        {
                            this.SetSkinStateText(component, GetSkinCannotBuyStr(skinInfo));
                        }
                        break;

                    case HeroSkinState.NormalHero_Skin_NotOwn:
                        if (!CSkinInfo.IsCanBuy(skinInfo.dwHeroID, skinInfo.dwSkinID))
                        {
                            this.SetSkinStateText(component, GetSkinCannotBuyStr(skinInfo));
                            break;
                        }
                        if (bSelect)
                        {
                            this.SetBuyBtn(obj2, skinInfo);
                        }
                        this.SetPricePanel(obj4, skinInfo);
                        break;

                    case HeroSkinState.LimitHero_LimitSkin_Wear:
                        this.SetSkinStateText(component, instance.GetText("ExpCard_In_Experience"));
                        break;

                    case HeroSkinState.LimitHero_LimitSkin_Own:
                        if (bSelect)
                        {
                            this.SetWearBtn(obj3, skinInfo);
                        }
                        if (CSkinInfo.IsCanBuy(skinInfo.dwHeroID, skinInfo.dwSkinID))
                        {
                            this.SetSkinStateText(component, instance.GetText("Hero_SkinState_UnUsable"));
                            this.SetPricePanel(obj4, skinInfo);
                        }
                        break;

                    case HeroSkinState.LimitHero_Skin_NotOwn:
                        if (!CSkinInfo.IsCanBuy(skinInfo.dwHeroID, skinInfo.dwSkinID))
                        {
                            this.SetSkinStateText(component, GetSkinCannotBuyStr(skinInfo));
                            break;
                        }
                        this.SetSkinStateText(component, instance.GetText("Hero_SkinState_UnUsable"));
                        this.SetPricePanel(obj4, skinInfo);
                        break;

                    case HeroSkinState.NoHero_Skin_Wear:
                        this.SetSkinStateText(component, instance.GetText("Hero_SkinState_UnUsable"));
                        break;

                    case HeroSkinState.NoHero_NormalSkin_Own:
                        this.SetSkinStateText(component, instance.GetText("Hero_SkinState_Own"));
                        break;

                    case HeroSkinState.NoHero_LimitSkin_Own:
                    case HeroSkinState.NoHero_Skin_NotOwn:
                        if (!CSkinInfo.IsCanBuy(skinInfo.dwHeroID, skinInfo.dwSkinID))
                        {
                            this.SetSkinStateText(component, GetSkinCannotBuyStr(skinInfo));
                            break;
                        }
                        this.SetSkinStateText(component, instance.GetText("Hero_SkinState_UnUsable"));
                        this.SetPricePanel(obj4, skinInfo);
                        break;
                }
            }
        }

        private void SetSkinStateText(Text skinStateText, string content)
        {
            skinStateText.set_text(content);
        }

        private void SetVictoryTipsEvt()
        {
            if (this.m_heroInfoForm != null)
            {
                GameObject widget = this.m_heroInfoForm.GetWidget(13);
                if (CBattleGuideManager.EnableHeroVictoryTips() && !CSysDynamicBlock.bLobbyEntryBlocked)
                {
                    string szName;
                    widget.CustomSetActive(true);
                    Transform transform = widget.get_transform();
                    string[] args = new string[] { this.m_selectHeroID.ToString() };
                    transform.FindChild("Btn").GetComponent<CUIEventScript>().m_onClickEventParams.tagStr = Singleton<CTextManager>.GetInstance().GetText("WinTrick_Url_Hero", args);
                    ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(this.m_selectHeroID);
                    if (dataByKey != null)
                    {
                        szName = dataByKey.szName;
                    }
                    else
                    {
                        szName = Singleton<CTextManager>.GetInstance().GetText("WinTrick_Tips_DefaultHeroName");
                    }
                    CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                    if (masterRoleInfo != null)
                    {
                        if (!masterRoleInfo.IsClientBitsSet(3))
                        {
                            CUICommonSystem.SetObjActive(transform.FindChild("Panel_Guide"), true);
                            string[] textArray2 = new string[] { szName };
                            CUICommonSystem.SetTextContent(transform.FindChild("Panel_Guide/Text"), Singleton<CTextManager>.GetInstance().GetText("WinTrick_Tips_text", textArray2));
                            masterRoleInfo.SetClientBits(3, true, true);
                        }
                        else
                        {
                            CUICommonSystem.SetObjActive(transform.FindChild("Panel_Guide"), false);
                        }
                    }
                }
                else
                {
                    widget.CustomSetActive(false);
                }
            }
        }

        private void SetWearBtn(GameObject wearBtnObj, ResHeroSkin skinInfo)
        {
            wearBtnObj.CustomSetActive(true);
            CUIEventScript component = wearBtnObj.GetComponent<CUIEventScript>();
            stUIEventParams eventParams = new stUIEventParams();
            eventParams.heroSkinParam.heroId = skinInfo.dwHeroID;
            eventParams.heroSkinParam.skinId = skinInfo.dwSkinID;
            component.SetUIEvent(enUIEventType.Click, enUIEventID.HeroSkin_Wear, eventParams);
        }

        private void SetWearExperienceSkinButton(CRoleInfo role, ResHeroSkin skinInfo, GameObject experienceBtnObj)
        {
            if (role.IsValidExperienceSkin(skinInfo.dwHeroID, skinInfo.dwSkinID))
            {
                experienceBtnObj.CustomSetActive(true);
                CUIEventScript component = experienceBtnObj.GetComponent<CUIEventScript>();
                stUIEventParams eventParams = new stUIEventParams();
                eventParams.heroSkinParam.heroId = skinInfo.dwHeroID;
                eventParams.heroSkinParam.skinId = skinInfo.dwSkinID;
                component.SetUIEvent(enUIEventType.Click, enUIEventID.HeroSkin_Wear, eventParams);
            }
        }

        private void SwitchHeroInfo(uint heroID, uint skinId, bool playAni = true, bool updateSwitchButton = true)
        {
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.HeroInfo_SkillTipClose);
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_heroInfoFormPath);
            if (form != null)
            {
                this.m_selectHeroID = heroID;
                if (updateSwitchButton)
                {
                    this.UpdateSwitchButton(form);
                }
                this.RefreshHeroInfoForm();
                this.SelectSkinElement(heroID, skinId);
            }
        }

        public override void UnInit()
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.HeroInfo_ViewStory, new CUIEventManager.OnUIEventHandler(this.OnViewHeroStory));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.HeroInfo_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_OpenForm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.HeroInfo_FormClose, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_FormClose));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.HeroInfo_TurnLeft, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_TurnLeft));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.HeroInfo_TurnRight, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_TurnRight));
            Singleton<EventRouter>.instance.RemoveEventHandler<string>("HeroUnlockPvP", new Action<string>(this.OnHeroUnlockPvP));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.HeroInfo_MenuSelect_Dummy, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_MenuSelect_Dummy));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.HeroInfo_Material_Direct_Buy, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_MaterialDirectBuy));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.HeroInfo_Material_Direct_Buy_Confirm, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_MaterialDirectBuyConfirm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.HeroInfo_Show2DImage, new CUIEventManager.OnUIEventHandler(this.OnShow2DImage));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.HeroInfo_Show3DImage, new CUIEventManager.OnUIEventHandler(this.OnShow3DImage));
            Singleton<EventRouter>.instance.RemoveEventHandler<uint>("HeroAdd", new Action<uint>(this.OnNtyAddHero));
            Singleton<EventRouter>.instance.RemoveEventHandler<uint, uint, uint>("HeroSkinAdd", new Action<uint, uint, uint>(this, (IntPtr) this.OnAddHeroSkin));
            Singleton<EventRouter>.instance.RemoveEventHandler(EventID.GLOBAL_REFRESH_TIME, new Action(null, (IntPtr) ResetDirectBuyLimit));
        }

        private void UpdateSwitchButton(CUIFormScript form)
        {
            if (this.m_heroInfoForm != null)
            {
                GameObject widget = this.m_heroInfoForm.GetWidget(10);
                GameObject obj3 = this.m_heroInfoForm.GetWidget(11);
                switch (this.m_OpenSrc)
                {
                    case enHeroFormOpenSrc.HeroListClick:
                    {
                        int heroIndexByConfigId = Singleton<CHeroOverviewSystem>.GetInstance().GetHeroIndexByConfigId(this.m_selectHeroID);
                        widget.CustomSetActive(heroIndexByConfigId > 0);
                        obj3.CustomSetActive(heroIndexByConfigId < (Singleton<CHeroOverviewSystem>.GetInstance().GetHeroListCount() - 1));
                        return;
                    }
                    case enHeroFormOpenSrc.HeroBuyClick:
                    {
                        int num2 = Singleton<CMallSystem>.GetInstance().m_heroMallCtrl.GetHeroIndexByConfigId(this.m_selectHeroID);
                        widget.CustomSetActive(num2 > 0);
                        obj3.CustomSetActive(num2 < (Singleton<CMallSystem>.GetInstance().m_heroMallCtrl.GetHeroListCount() - 1));
                        return;
                    }
                    case enHeroFormOpenSrc.SkinBuyClick:
                    {
                        CUIListScript component = this.m_heroInfoForm.GetWidget(2).get_transform().Find("List_Skin").GetComponent<CUIListScript>();
                        if (component != null)
                        {
                            int selectedIndex = component.GetSelectedIndex();
                            uint skinIdByIndex = CSkinInfo.GetSkinIdByIndex(this.m_selectHeroID, selectedIndex);
                            uint skinCfgId = CSkinInfo.GetSkinCfgId(this.m_selectHeroID, skinIdByIndex);
                            int skinIndexByConfigId = Singleton<CMallSystem>.GetInstance().m_skinMallCtrl.GetSkinIndexByConfigId(skinCfgId);
                            widget.CustomSetActive(skinIndexByConfigId > 0);
                            obj3.CustomSetActive(skinIndexByConfigId < (Singleton<CMallSystem>.GetInstance().m_skinMallCtrl.GetSkinListCount() - 1));
                            return;
                        }
                        return;
                    }
                    default:
                        widget.CustomSetActive(false);
                        obj3.CustomSetActive(false);
                        return;
                }
            }
        }
    }
}

