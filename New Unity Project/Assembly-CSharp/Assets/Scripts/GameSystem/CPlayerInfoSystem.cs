namespace Assets.Scripts.GameSystem
{
    using Apollo;
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    [MessageHandlerClass]
    public class CPlayerInfoSystem : Singleton<CPlayerInfoSystem>
    {
        private bool _isShowGuildAppointViceChairmanBtn;
        private bool _isShowGuildFireMemberBtn;
        private bool _isShowGuildTransferPositionBtn;
        private DetailPlayerInfoSource _lastDetailSource;
        public const ushort CREDIT_RULE_ID = 11;
        private bool isShowPlayerInfoDirectly = true;
        private Tab m_CurTab;
        private CUIFormScript m_Form;
        private bool m_IsFormOpen;
        private CPlayerProfile m_PlayerProfile = new CPlayerProfile();
        public const ushort PlAYER_INFO_RULE_ID = 3;
        public static string sPlayerInfoFormPath = "UGUI/Form/System/Player/Form_Player_Info.prefab";

        [MessageHandler(0x4aa)]
        public static void ChangePersonSgin(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            if (msg.stPkgData.stSignatureRsp.dwResult != 0)
            {
                Singleton<CUIManager>.GetInstance().OpenMessageBox(Utility.ProtErrCodeToStr(0x4aa, (int) msg.stPkgData.stSignatureRsp.dwResult), false);
            }
            else
            {
                if (Singleton<CPlayerInfoSystem>.GetInstance().CurTab == Tab.Base_Info)
                {
                    Singleton<CPlayerInfoSystem>.GetInstance().UpdateBaseInfo();
                }
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if (masterRoleInfo != null)
                {
                    masterRoleInfo.PersonSign = Singleton<CPlayerInfoSystem>.instance.m_PlayerProfile.m_personSign;
                }
            }
        }

        private void DeepLinkClick(CUIEvent uiEvent)
        {
            if ((ApolloConfig.platform == ApolloPlatform.Wechat) && MonoSingleton<BannerImageSys>.GetInstance().DeepLinkInfo.bLoadSucc)
            {
                Debug.Log(string.Concat(new object[] { "deeplink ", MonoSingleton<BannerImageSys>.GetInstance().DeepLinkInfo.linkType, " ", MonoSingleton<BannerImageSys>.GetInstance().DeepLinkInfo.linkUrl }));
                Singleton<ApolloHelper>.GetInstance().OpenWeiXinDeeplink(MonoSingleton<BannerImageSys>.GetInstance().DeepLinkInfo.linkType, MonoSingleton<BannerImageSys>.GetInstance().DeepLinkInfo.linkUrl);
            }
        }

        private void DisplayCustomButton()
        {
            if (this.m_IsFormOpen && (this.m_Form != null))
            {
                GameObject widget = this.m_Form.GetWidget(1);
                if ((widget != null) && widget.get_activeSelf())
                {
                    GameObject obj3 = Utility.FindChild(widget, "pnlContainer/pnlHead/btnRename");
                    GameObject obj4 = Utility.FindChild(widget, "pnlContainer/pnlHead/btnShare");
                    switch (this._lastDetailSource)
                    {
                        case DetailPlayerInfoSource.DefaultOthers:
                            obj3.CustomSetActive(false);
                            obj4.CustomSetActive(false);
                            this.SetBaseInfoScrollable(false);
                            this.SetAllGuildBtnActive(widget, false);
                            this.SetAllFriendBtnActive(widget, false);
                            break;

                        case DetailPlayerInfoSource.Self:
                            obj3.CustomSetActive(false);
                            obj4.CustomSetActive(false);
                            this.SetBaseInfoScrollable(false);
                            this.SetAllGuildBtnActive(widget, false);
                            this.SetAppointMatchLeaderBtn();
                            this.SetAllFriendBtnActive(widget, false);
                            break;

                        case DetailPlayerInfoSource.Guild:
                            obj3.CustomSetActive(false);
                            obj4.CustomSetActive(false);
                            this.SetBaseInfoScrollable(false);
                            this.SetSingleGuildBtn(widget);
                            this.SetAllFriendBtnActive(widget, false);
                            break;
                    }
                }
            }
        }

        public CPlayerProfile GetProfile()
        {
            return this.m_PlayerProfile;
        }

        private void ImpResDetailInfo(CSPkg msg)
        {
            if (msg.stPkgData.stGetAcntDetailInfoRsp.iErrCode != 0)
            {
                Singleton<CUIManager>.GetInstance().OpenMessageBox(string.Format("Error Code {0}", msg.stPkgData.stGetAcntDetailInfoRsp.iErrCode), false);
            }
            else
            {
                this.m_PlayerProfile.ConvertServerDetailData(msg.stPkgData.stGetAcntDetailInfoRsp.stAcntDetail.stOfSucc);
                this.OpenForm();
            }
        }

        public override void Init()
        {
            base.Init();
            CUIEventManager instance = Singleton<CUIEventManager>.GetInstance();
            instance.AddUIEventListener(enUIEventID.Player_Info_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfo_OpenForm));
            instance.AddUIEventListener(enUIEventID.Player_Info_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfo_CloseForm));
            instance.AddUIEventListener(enUIEventID.Player_Info_Tab_Change, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfoTabChange));
            instance.AddUIEventListener(enUIEventID.Player_Info_Open_Pvp_Info, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfoOpenPvpInfo));
            instance.AddUIEventListener(enUIEventID.Player_Info_Open_Base_Info, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfoOpenBaseInfo));
            instance.AddUIEventListener(enUIEventID.Player_Info_Quit_Game, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfoQuitGame));
            instance.AddUIEventListener(enUIEventID.Player_Info_Quit_Game_Confirm, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfoQuitGameConfirm));
            instance.AddUIEventListener(enUIEventID.Player_Info_Most_Used_Hero_Item_Enable, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfoMostUsedHeroItemEnable));
            instance.AddUIEventListener(enUIEventID.Player_Info_Most_Used_Hero_Item_Click, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfoMostUsedHeroItemClick));
            instance.AddUIEventListener(enUIEventID.Player_Info_Show_Rule, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfoShowRule));
            instance.AddUIEventListener(enUIEventID.Player_Info_License_ListItem_Enable, new CUIEventManager.OnUIEventHandler(this.OnLicenseListItemEnable));
            instance.AddUIEventListener(enUIEventID.Player_Info_Update_Sub_Module, new CUIEventManager.OnUIEventHandler(this.OnUpdateSubModule));
            instance.AddUIEventListener(enUIEventID.WEB_IntegralHall, new CUIEventManager.OnUIEventHandler(this.OpenIntegralHall));
            instance.AddUIEventListener(enUIEventID.OPEN_QQ_Buluo, new CUIEventManager.OnUIEventHandler(this.OpenQQBuluo));
            instance.AddUIEventListener(enUIEventID.Player_Info_Achievement_Trophy_Click, new CUIEventManager.OnUIEventHandler(this.OnAchievementTrophyClick));
            this.m_IsFormOpen = false;
            this.m_CurTab = Tab.Base_Info;
            this.m_Form = null;
            instance.AddUIEventListener(enUIEventID.BuyPick_QQ_VIP, new CUIEventManager.OnUIEventHandler(this.OpenByQQVIP));
            instance.AddUIEventListener(enUIEventID.DeepLink_OnClick, new CUIEventManager.OnUIEventHandler(this.DeepLinkClick));
            Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.NOBE_STATE_CHANGE, new Action(this, (IntPtr) this.UpdateNobeHeadIdx));
            Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.HEAD_IMAGE_FLAG_CHANGE, new Action(this, (IntPtr) this.UpdateHeadFlag));
            Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.NAMECHANGE_PLAYER_NAME_CHANGE, new Action(this, (IntPtr) this.OnPlayerNameChange));
            Singleton<EventRouter>.GetInstance().AddEventHandler<byte, CAchieveItem2>(EventID.ACHIEVE_SERY_SELECT_DONE, new Action<byte, CAchieveItem2>(this, (IntPtr) this.OnTrophySelectDone));
            Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.GAMER_REDDOT_CHANGE, new Action(this, (IntPtr) this.UpdateXinYueBtn));
            Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.ACHIEVE_TROPHY_REWARD_INFO_STATE_CHANGE, new Action(this, (IntPtr) this.OnTrophyStateChange));
        }

        private void InitTab()
        {
            if ((this.m_Form != null) && this.m_IsFormOpen)
            {
                GameObject widget = this.m_Form.GetWidget(1);
                if ((widget != null) && widget.get_activeSelf())
                {
                    widget.CustomSetActive(false);
                }
                Tab[] values = (Tab[]) Enum.GetValues(typeof(Tab));
                string[] strArray = new string[values.Length];
                for (byte i = 0; i < values.Length; i = (byte) (i + 1))
                {
                    switch (values[i])
                    {
                        case Tab.Base_Info:
                            strArray[i] = Singleton<CTextManager>.GetInstance().GetText("Player_Info_Tab_Base_Info");
                            break;

                        case Tab.Pvp_Info:
                            strArray[i] = Singleton<CTextManager>.GetInstance().GetText("Player_Info_Tab_Pvp_Info");
                            break;

                        case Tab.Honor_Info:
                            strArray[i] = Singleton<CTextManager>.GetInstance().GetText("Player_Info_Tab_Honor_Info");
                            break;

                        case Tab.Common_Hero:
                            strArray[i] = Singleton<CTextManager>.GetInstance().GetText("Player_Info_Tab_Common_Hero_Info");
                            break;

                        case Tab.PvpHistory_Info:
                            strArray[i] = Singleton<CTextManager>.GetInstance().GetText("Player_Info_Tab_PvpHistory_Info");
                            break;

                        case Tab.PvpCreditScore_Info:
                            strArray[i] = Singleton<CTextManager>.GetInstance().GetText("Player_Info_Tab_Credit_Info");
                            break;

                        case Tab.Mentor_Info:
                            strArray[i] = Singleton<CTextManager>.GetInstance().GetText("Player_Info_Tab_Mentor_Info");
                            break;
                    }
                }
                CUIListScript component = this.m_Form.GetWidget(0).GetComponent<CUIListScript>();
                if (component != null)
                {
                    component.SetElementAmount(strArray.Length);
                    for (int j = 0; j < component.m_elementAmount; j++)
                    {
                        component.GetElemenet(j).get_gameObject().get_transform().Find("Text").GetComponent<Text>().set_text(strArray[j]);
                    }
                    component.m_alwaysDispatchSelectedChangeEvent = true;
                    component.SelectElement((int) this.CurTab, true);
                }
                Singleton<CUINewFlagSystem>.GetInstance().AddNewFlag(component.GetElemenet(2).get_gameObject(), enNewFlagKey.New_HonorInfo_V1, enNewFlagPos.enTopRight, 1f, 0f, 0f, enNewFlagType.enNewFlag);
                Singleton<CUINewFlagSystem>.GetInstance().AddNewFlag(component.GetElemenet(4).get_gameObject(), enNewFlagKey.New_PvpHistoryInfo_V1, enNewFlagPos.enTopRight, 1f, 0f, 0f, enNewFlagType.enNewFlag);
                Singleton<CUINewFlagSystem>.GetInstance().AddNewFlag(component.GetElemenet(6).get_gameObject(), enNewFlagKey.New_MentorInfo_V1, enNewFlagPos.enTopRight, 1f, 0f, 0f, enNewFlagType.enNewFlag);
            }
        }

        public static bool isSelf(ulong playerUllUID)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            return ((masterRoleInfo != null) && (masterRoleInfo.playerUllUID == playerUllUID));
        }

        public void LoadSubModule()
        {
            DebugHelper.Assert(this.m_Form != null, "Player Form Is Null");
            if (this.m_Form != null)
            {
                bool flag = false;
                GameObject widget = this.m_Form.GetWidget(10);
                GameObject obj3 = this.m_Form.GetWidget(9);
                if ((widget != null) && (obj3 != null))
                {
                    switch (this.m_CurTab)
                    {
                        case Tab.Honor_Info:
                            flag = Singleton<CPlayerHonorController>.GetInstance().Loaded(this.m_Form);
                            if (!flag)
                            {
                                widget.CustomSetActive(true);
                                Singleton<CPlayerHonorController>.GetInstance().Load(this.m_Form);
                                obj3.CustomSetActive(false);
                            }
                            break;

                        case Tab.PvpHistory_Info:
                            flag = Singleton<CPlayerPvpHistoryController>.GetInstance().Loaded(this.m_Form);
                            if (!flag)
                            {
                                widget.CustomSetActive(true);
                                Singleton<CPlayerPvpHistoryController>.GetInstance().Load(this.m_Form);
                                obj3.CustomSetActive(false);
                            }
                            break;

                        case Tab.PvpCreditScore_Info:
                            flag = Singleton<CPlayerCreaditScoreController>.GetInstance().Loaded(this.m_Form);
                            if (!flag)
                            {
                                widget.CustomSetActive(true);
                                Singleton<CPlayerCreaditScoreController>.GetInstance().Load(this.m_Form);
                                obj3.CustomSetActive(false);
                            }
                            break;

                        case Tab.Mentor_Info:
                            Singleton<CPlayerMentorInfoController>.instance.UpdateUI();
                            break;
                    }
                }
                if (!flag)
                {
                    GameObject obj4 = this.m_Form.GetWidget(11);
                    if (obj4 != null)
                    {
                        CUITimerScript component = obj4.GetComponent<CUITimerScript>();
                        if (component != null)
                        {
                            component.ReStartTimer();
                        }
                    }
                }
                else
                {
                    Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Player_Info_Update_Sub_Module);
                }
            }
        }

        private void OnAchievementTrophyClick(CUIEvent uiEvent)
        {
            CAchieveInfo2 achieveInfo = CAchieveInfo2.GetAchieveInfo(this.m_PlayerProfile.m_iLogicWorldId, this.m_PlayerProfile.m_uuid, false);
            uint achievementID = uiEvent.m_eventParams.commonUInt32Param1;
            Singleton<CAchievementSystem>.GetInstance().ShowTrophyDetail(achieveInfo, achievementID);
        }

        private void OnLicenseListItemEnable(CUIEvent uiEvent)
        {
            int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
            GameObject srcWidget = uiEvent.m_srcWidget;
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if ((masterRoleInfo != null) && (masterRoleInfo.m_licenseInfo != null))
            {
                CLicenseItem licenseItemByIndex = masterRoleInfo.m_licenseInfo.GetLicenseItemByIndex(srcWidgetIndexInBelongedList);
                if (((srcWidget != null) && (licenseItemByIndex != null)) && (licenseItemByIndex.m_resLicenseInfo != null))
                {
                    srcWidget.get_transform().Find("licenseIcon").GetComponent<Image>().SetSprite(string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Task_Dir, licenseItemByIndex.m_resLicenseInfo.szIconPath), this.m_Form, true, false, false, false);
                    srcWidget.get_transform().Find("licenseNameText").GetComponent<Text>().set_text(licenseItemByIndex.m_resLicenseInfo.szLicenseName);
                    Transform transform3 = srcWidget.get_transform().Find("licenseStateText");
                    if (licenseItemByIndex.m_getSecond > 0)
                    {
                        DateTime time = Utility.ToUtcTime2Local((long) licenseItemByIndex.m_getSecond);
                        transform3.GetComponent<Text>().set_text(string.Format("<color=#00d519>{0}/{1}/{2}</color>", time.Year, time.Month, time.Day));
                    }
                    else
                    {
                        transform3.GetComponent<Text>().set_text("<color=#fecb2f>未获得</color>");
                    }
                    srcWidget.get_transform().Find("licenseDescText").GetComponent<Text>().set_text(licenseItemByIndex.m_resLicenseInfo.szDesc);
                }
            }
        }

        private void OnPersonSignEndEdit(string personSign)
        {
            if (string.Compare(personSign, this.m_PlayerProfile.m_personSign) != 0)
            {
                this.m_PlayerProfile.m_personSign = personSign;
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x4a9);
                StringHelper.StringToUTF8Bytes(personSign, ref msg.stPkgData.stSignatureReq.szSignatureInfo);
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
            }
        }

        private void OnPlayerInfo_CloseForm(CUIEvent uiEvent)
        {
            if (this.m_IsFormOpen)
            {
                this.m_IsFormOpen = false;
                Singleton<CUIManager>.GetInstance().CloseForm(sPlayerInfoFormPath);
                this.m_Form = null;
                Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.PlayerInfoSystem_Form_Close);
            }
        }

        private void OnPlayerInfo_OpenForm(CUIEvent uiEvent)
        {
            this.ShowPlayerDetailInfo(0L, 0, DetailPlayerInfoSource.Self, true);
            CMiShuSystem.SendUIClickToServer(enUIClickReprotID.rp_HeroHeadBtn);
        }

        private void OnPlayerInfoMostUsedHeroItemClick(CUIEvent uiEvent)
        {
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Player_Info_CloseForm, uiEvent.m_eventParams);
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.HeroInfo_OpenForm, uiEvent.m_eventParams);
        }

        private void OnPlayerInfoMostUsedHeroItemEnable(CUIEvent uiEvent)
        {
            int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
            GameObject srcWidget = uiEvent.m_srcWidget;
            if (srcWidget != null)
            {
                GameObject listItem = srcWidget.get_transform().Find("heroItem").get_gameObject();
                ListView<COMDT_MOST_USED_HERO_INFO> view = this.m_PlayerProfile.MostUsedHeroList();
                if (srcWidgetIndexInBelongedList < view.Count)
                {
                    COMDT_MOST_USED_HERO_INFO heroInfo = view[srcWidgetIndexInBelongedList];
                    this.SetHeroItemData(uiEvent.m_srcFormScript, listItem, heroInfo);
                    Text componetInChild = Utility.GetComponetInChild<Text>(srcWidget, "usedCnt");
                    if (componetInChild != null)
                    {
                        componetInChild.set_text(string.Format(Singleton<CTextManager>.GetInstance().GetText("Player_Info_Games_Cnt_Label"), heroInfo.dwGameWinNum + heroInfo.dwGameLoseNum));
                    }
                }
            }
        }

        private void OnPlayerInfoOpenBaseInfo(CUIEvent uiEvent)
        {
            this.OpenBaseInfo();
        }

        private void OnPlayerInfoOpenPvpInfo(CUIEvent uiEvent)
        {
            this.OpenPvpInfo();
        }

        private void OnPlayerInfoQuitGame(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("Common_QuitGameTips"), enUIEventID.Player_Info_Quit_Game_Confirm, enUIEventID.None, false);
        }

        private void OnPlayerInfoQuitGameConfirm(CUIEvent uiEvent)
        {
            SGameApplication.Quit();
        }

        private void OnPlayerInfoShowRule(CUIEvent uiEvent)
        {
            ResRuleText dataByKey = null;
            ushort num = 0;
            if (this.m_CurTab == Tab.PvpCreditScore_Info)
            {
                num = 11;
            }
            else
            {
                num = 3;
            }
            dataByKey = GameDataMgr.s_ruleTextDatabin.GetDataByKey((uint) num);
            if (dataByKey != null)
            {
                string title = StringHelper.UTF8BytesToString(ref dataByKey.szTitle);
                string info = StringHelper.UTF8BytesToString(ref dataByKey.szContent);
                Singleton<CUIManager>.GetInstance().OpenInfoForm(title, info);
            }
        }

        private void OnPlayerInfoTabChange(CUIEvent uiEvent)
        {
            if ((this.m_Form != null) && this.m_IsFormOpen)
            {
                CUIListScript component = uiEvent.m_srcWidget.GetComponent<CUIListScript>();
                if (component != null)
                {
                    int selectedIndex = component.GetSelectedIndex();
                    this.CurTab = (Tab) selectedIndex;
                    GameObject widget = this.m_Form.GetWidget(1);
                    GameObject obj3 = this.m_Form.GetWidget(5);
                    GameObject obj4 = this.m_Form.GetWidget(7);
                    GameObject obj5 = this.m_Form.GetWidget(8);
                    GameObject obj6 = this.m_Form.GetWidget(10);
                    GameObject obj7 = this.m_Form.GetWidget(9);
                    GameObject obj8 = this.m_Form.GetWidget(12);
                    GameObject obj9 = this.m_Form.GetWidget(2);
                    GameObject obj10 = this.m_Form.GetWidget(0x15);
                    if (obj8 != null)
                    {
                        this.SetTitle(this.m_CurTab, obj8.get_transform());
                    }
                    Transform transform = this.m_Form.get_transform().Find("pnlBg/pnlBody/pnlHonorInfo");
                    GameObject obj11 = null;
                    if (transform != null)
                    {
                        obj11 = transform.get_gameObject();
                    }
                    Transform transform2 = this.m_Form.get_transform().Find("pnlBg/pnlBody/pnlPvPHistory");
                    GameObject obj12 = null;
                    if (transform2 != null)
                    {
                        obj12 = transform2.get_gameObject();
                    }
                    Transform transform3 = this.m_Form.get_transform().Find("pnlBg/pnlBody/pnlCreditScoreInfo");
                    GameObject obj13 = null;
                    if (transform3 != null)
                    {
                        obj13 = transform3.get_gameObject();
                    }
                    switch (this.m_CurTab)
                    {
                        case Tab.Base_Info:
                            obj7.CustomSetActive(true);
                            obj6.CustomSetActive(false);
                            widget.CustomSetActive(true);
                            obj3.CustomSetActive(false);
                            obj13.CustomSetActive(false);
                            obj4.CustomSetActive(false);
                            obj11.CustomSetActive(false);
                            obj12.CustomSetActive(false);
                            obj9.CustomSetActive(false);
                            obj10.CustomSetActive(false);
                            this.UpdateBaseInfo();
                            this.ProcessQQVIP(this.m_Form, true);
                            this.ProcessNobeHeadIDx(this.m_Form, true);
                            break;

                        case Tab.Pvp_Info:
                            widget.CustomSetActive(false);
                            obj3.CustomSetActive(false);
                            obj5.CustomSetActive(true);
                            obj13.CustomSetActive(false);
                            obj4.CustomSetActive(false);
                            obj11.CustomSetActive(false);
                            obj12.CustomSetActive(false);
                            obj9.CustomSetActive(true);
                            obj10.CustomSetActive(false);
                            this.UpdatePvpInfo2();
                            break;

                        case Tab.Honor_Info:
                            widget.CustomSetActive(false);
                            obj3.CustomSetActive(false);
                            obj5.CustomSetActive(true);
                            obj13.CustomSetActive(false);
                            obj4.CustomSetActive(false);
                            obj11.CustomSetActive(false);
                            obj12.CustomSetActive(false);
                            obj9.CustomSetActive(false);
                            obj10.CustomSetActive(false);
                            this.LoadSubModule();
                            MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.onClickGloryPoints, new uint[0]);
                            Singleton<CUINewFlagSystem>.GetInstance().DelNewFlag(component.GetElemenet(2).get_gameObject(), enNewFlagKey.New_HonorInfo_V1, true);
                            break;

                        case Tab.Common_Hero:
                            obj7.CustomSetActive(true);
                            obj6.CustomSetActive(false);
                            widget.CustomSetActive(false);
                            obj3.CustomSetActive(false);
                            obj13.CustomSetActive(false);
                            obj11.CustomSetActive(false);
                            obj5.CustomSetActive(true);
                            obj4.CustomSetActive(true);
                            obj12.CustomSetActive(false);
                            obj9.CustomSetActive(false);
                            obj10.CustomSetActive(false);
                            Singleton<CPlayerCommonHeroInfoController>.instance.UpdateUI();
                            break;

                        case Tab.PvpHistory_Info:
                            widget.CustomSetActive(false);
                            obj3.CustomSetActive(false);
                            obj5.CustomSetActive(true);
                            obj13.CustomSetActive(false);
                            obj4.CustomSetActive(false);
                            obj11.CustomSetActive(false);
                            obj12.CustomSetActive(false);
                            obj9.CustomSetActive(false);
                            obj10.CustomSetActive(false);
                            this.LoadSubModule();
                            Singleton<CUINewFlagSystem>.GetInstance().DelNewFlag(component.GetElemenet(4).get_gameObject(), enNewFlagKey.New_PvpHistoryInfo_V1, true);
                            break;

                        case Tab.PvpCreditScore_Info:
                            obj7.CustomSetActive(true);
                            obj6.CustomSetActive(false);
                            widget.CustomSetActive(false);
                            obj3.CustomSetActive(false);
                            obj5.CustomSetActive(true);
                            obj13.CustomSetActive(true);
                            obj4.CustomSetActive(false);
                            obj11.CustomSetActive(false);
                            obj12.CustomSetActive(false);
                            obj9.CustomSetActive(false);
                            obj10.CustomSetActive(false);
                            this.LoadSubModule();
                            break;

                        case Tab.Mentor_Info:
                            widget.CustomSetActive(false);
                            obj3.CustomSetActive(false);
                            obj5.CustomSetActive(false);
                            obj13.CustomSetActive(false);
                            obj4.CustomSetActive(false);
                            obj11.CustomSetActive(false);
                            obj12.CustomSetActive(false);
                            obj9.CustomSetActive(false);
                            obj10.CustomSetActive(true);
                            this.LoadSubModule();
                            Singleton<CUINewFlagSystem>.GetInstance().DelNewFlag(component.GetElemenet(6).get_gameObject(), enNewFlagKey.New_MentorInfo_V1, true);
                            break;
                    }
                    Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.PlayerInfoSystem_Tab_Change);
                }
            }
        }

        private void OnPlayerNameChange()
        {
            if (this.m_IsFormOpen && (this.m_Form != null))
            {
                GameObject widget = this.m_Form.GetWidget(1);
                if (widget != null)
                {
                    Text componetInChild = Utility.GetComponetInChild<Text>(widget, "pnlContainer/pnlHead/NameGroup/txtName");
                    CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                    if ((componetInChild != null) && (masterRoleInfo != null))
                    {
                        componetInChild.set_text(masterRoleInfo.Name);
                    }
                }
            }
        }

        private void OnTrophySelectDone(byte idx, CAchieveItem2 item)
        {
            CAchieveInfo2 masterAchieveInfo = CAchieveInfo2.GetMasterAchieveInfo();
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (idx < masterAchieveInfo.SelectedTrophies.Length)
            {
                masterAchieveInfo.SelectedTrophies[idx] = item;
                Singleton<CPlayerInfoSystem>.GetInstance().GetProfile().ConvertRoleInfoData(masterRoleInfo);
                this.UpdateBaseInfo();
            }
        }

        private void OnTrophyStateChange()
        {
            if ((this.m_IsFormOpen && (this.m_Form != null)) && (this.CurTab == Tab.Base_Info))
            {
                this.ProcessSelectedTrophies();
            }
        }

        private void OnUpdateSubModule(CUIEvent uiEvent)
        {
            DebugHelper.Assert(this.m_Form != null, "Player Form Is Null");
            if (this.m_Form != null)
            {
                GameObject widget = this.m_Form.GetWidget(10);
                this.m_Form.GetWidget(9).CustomSetActive(true);
                widget.CustomSetActive(false);
                switch (this.m_CurTab)
                {
                    case Tab.Honor_Info:
                        Singleton<CPlayerHonorController>.GetInstance().Draw(this.m_Form);
                        break;

                    case Tab.PvpHistory_Info:
                        Singleton<CPlayerPvpHistoryController>.GetInstance().Draw(this.m_Form);
                        break;

                    case Tab.PvpCreditScore_Info:
                        Singleton<CPlayerCreaditScoreController>.GetInstance().Draw(this.m_Form);
                        break;
                }
            }
        }

        public void OpenBaseInfo()
        {
            this.ShowPlayerDetailInfo(0L, 0, DetailPlayerInfoSource.Self, true);
        }

        private void OpenByQQVIP(CUIEvent uiEvent)
        {
            if ((ApolloConfig.platform == ApolloPlatform.QQ) || (ApolloConfig.platform == ApolloPlatform.WTLogin))
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if (masterRoleInfo != null)
                {
                    if (masterRoleInfo.HasVip(0x10))
                    {
                        Singleton<ApolloHelper>.GetInstance().PayQQVip("CJCLUBT", Singleton<CTextManager>.GetInstance().GetText("QQ_Vip_XuFei_Super_Vip"), 1);
                    }
                    else if (masterRoleInfo.HasVip(1))
                    {
                        Singleton<ApolloHelper>.GetInstance().PayQQVip("LTMCLUB", Singleton<CTextManager>.GetInstance().GetText("QQ_Vip_XuFei_Vip"), 1);
                    }
                    else
                    {
                        Singleton<ApolloHelper>.GetInstance().PayQQVip("LTMCLUB", Singleton<CTextManager>.GetInstance().GetText("QQ_Vip_Buy_Vip"), 1);
                    }
                }
            }
        }

        public void OpenForm()
        {
            if (this.m_IsFormOpen)
            {
                this.m_Form = Singleton<CUIManager>.GetInstance().GetForm(sPlayerInfoFormPath);
            }
            else
            {
                this.m_Form = Singleton<CUIManager>.GetInstance().OpenForm(sPlayerInfoFormPath, true, true);
            }
            this.m_IsFormOpen = true;
            this.CurTab = Tab.Base_Info;
            this.InitTab();
            Singleton<CPlayerPvpInfoController>.instance.InitUI();
            Singleton<CPlayerCommonHeroInfoController>.instance.InitCommonHeroUI();
        }

        private void OpenIntegralHall(CUIEvent uiEvent)
        {
            string str = "http://jfq.qq.com/comm/index_android.html";
            CUICommonSystem.OpenUrl(string.Format("{0}?partition={1}", str, MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID), true, 0);
        }

        public void OpenPvpInfo()
        {
            this.ShowPlayerDetailInfo(0L, 0, DetailPlayerInfoSource.Self, true);
        }

        private void OpenQQBuluo(CUIEvent uievent)
        {
            if ((ApolloConfig.platform == ApolloPlatform.QQ) || (ApolloConfig.platform == ApolloPlatform.WTLogin))
            {
                string strUrl = "http://xiaoqu.qq.com/cgi-bin/bar/qqgame/handle_ticket?redirect_url=http%3A%2F%2Fxiaoqu.qq.com%2Fmobile%2Fbarindex.html%3F%26_bid%3D%26_wv%3D1027%23bid%3D227061";
                CUICommonSystem.OpenUrl(strUrl, true, 0);
            }
            else if (ApolloConfig.platform == ApolloPlatform.Wechat)
            {
                string str2 = "http://game.weixin.qq.com/cgi-bin/h5/static/circle/index.html?jsapi=1&appid=wx95a3a4d7c627e07d&auth_type=2&ssid=12";
                CUICommonSystem.OpenUrl(str2, true, 1);
            }
        }

        public void ProcessCommonQQVip(GameObject parent)
        {
            if (parent != null)
            {
                GameObject obj2 = parent.get_transform().FindChild("QQSVipIcon").get_gameObject();
                parent.get_transform().FindChild("QQVipIcon").get_gameObject().CustomSetActive(false);
                obj2.CustomSetActive(false);
            }
        }

        private void ProcessNobeHeadIDx(CUIFormScript form, bool bshow)
        {
            GameObject obj2 = Utility.FindChild(form.get_gameObject(), "pnlBg/pnlBody/pnlBaseInfo/pnlContainer/pnlHead/changeNobeheadicon");
            if (!isSelf(this.m_PlayerProfile.m_uuid))
            {
                obj2.CustomSetActive(false);
            }
            else
            {
                if (CSysDynamicBlock.bVipBlock)
                {
                    bshow = false;
                }
                if (bshow)
                {
                    obj2.CustomSetActive(true);
                }
                else
                {
                    obj2.CustomSetActive(false);
                }
            }
        }

        private void ProcessQQVIP(CUIFormScript form, bool bShow)
        {
            if (form != null)
            {
                GameObject obj2 = Utility.FindChild(form.get_gameObject(), "pnlBg/pnlBody/pnlBaseInfo/pnlContainer/BtnGroup/QQVIPBtn");
                GameObject obj3 = Utility.FindChild(form.get_gameObject(), "pnlBg/pnlBody/pnlBaseInfo/pnlContainer/pnlHead/NameGroup/QQVipIcon");
                if (!isSelf(this.m_PlayerProfile.m_uuid))
                {
                    obj2.CustomSetActive(false);
                    MonoSingleton<NobeSys>.GetInstance().SetOtherQQVipHead(obj3.GetComponent<Image>(), (int) this.m_PlayerProfile.qqVipMask);
                }
                else
                {
                    GameObject obj4 = Utility.FindChild(form.get_gameObject(), "pnlBg/pnlBody/pnlBaseInfo/pnlContainer/BtnGroup/QQVIPBtn/Text");
                    if (!bShow)
                    {
                        obj2.CustomSetActive(false);
                        obj3.CustomSetActive(false);
                        obj4.CustomSetActive(false);
                    }
                    else
                    {
                        if (ApolloConfig.platform == ApolloPlatform.QQ)
                        {
                            obj2.CustomSetActive(true);
                        }
                        else
                        {
                            obj2.CustomSetActive(false);
                        }
                        obj4.CustomSetActive(true);
                        obj3.CustomSetActive(false);
                        if (CSysDynamicBlock.bLobbyEntryBlocked)
                        {
                            obj2.CustomSetActive(false);
                            obj3.CustomSetActive(false);
                            obj4.CustomSetActive(false);
                        }
                        else
                        {
                            if (ApolloConfig.platform == ApolloPlatform.QQ)
                            {
                                obj2.CustomSetActive(true);
                            }
                            else
                            {
                                obj2.CustomSetActive(false);
                            }
                            obj4.GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("QQ_Vip_Buy_Vip"));
                            if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo() != null)
                            {
                                MonoSingleton<NobeSys>.GetInstance().SetMyQQVipHead(obj3.GetComponent<Image>());
                            }
                        }
                    }
                }
            }
        }

        private void ProcessSelectedTrophies()
        {
            GameObject widget = this.m_Form.GetWidget(1);
            CAchieveInfo2 info = CAchieveInfo2.GetAchieveInfo(this.m_PlayerProfile.m_iLogicWorldId, this.m_PlayerProfile.m_uuid, false);
            bool isSelf = CPlayerInfoSystem.isSelf(this.m_PlayerProfile.m_uuid);
            bool flag2 = true;
            if (isSelf)
            {
                if (info.GetTrophies(enTrophyState.Finish).Count != 0)
                {
                    flag2 = false;
                }
            }
            else
            {
                for (int i = 0; i < this.m_PlayerProfile._selectedTrophies.Length; i++)
                {
                    if (this.m_PlayerProfile._selectedTrophies[i] != null)
                    {
                        flag2 = false;
                        break;
                    }
                }
            }
            ListView<CAchieveItem2> filteredTrophies = new ListView<CAchieveItem2>();
            if (isSelf)
            {
                ListView<CAchieveItem2> trophies = info.GetTrophies(enTrophyState.Finish);
                for (int j = trophies.Count - 1; j >= 0; j--)
                {
                    if ((trophies[j] != null) && (Array.IndexOf<CAchieveItem2>(this.m_PlayerProfile._selectedTrophies, trophies[j]) < 0))
                    {
                        filteredTrophies.Add(trophies[j]);
                    }
                }
            }
            CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(widget, "pnlContainer/pnlTrophy/List");
            if (componetInChild == null)
            {
                DebugHelper.Assert(false, "Player Info selectedTrophyListScript is null!");
            }
            else if (flag2)
            {
                componetInChild.SetElementAmount(0);
                if (isSelf)
                {
                    componetInChild.GetWidget(0).GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("Achievement_Player_Info_Selected_Trophies_Self_No_Data"));
                }
                else
                {
                    componetInChild.GetWidget(0).GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("Achievement_Player_Info_Selected_Trophies_Other_No_Data"));
                }
            }
            else
            {
                componetInChild.SetElementAmount(this.m_PlayerProfile._selectedTrophies.Length);
                Singleton<CTrophySelector>.GetInstance().SelectedTrophies = this.m_PlayerProfile._selectedTrophies;
                for (int k = 0; k < this.m_PlayerProfile._selectedTrophies.Length; k++)
                {
                    CUIListElementScript elemenet = componetInChild.GetElemenet(k);
                    this.RefreshSelectedAchieveElement(elemenet, this.m_PlayerProfile._selectedTrophies[k], k, isSelf, filteredTrophies);
                }
            }
        }

        private void RefreshLicenseInfoPanel(CUIFormScript form)
        {
            if (null != form)
            {
                GameObject widget = form.GetWidget(6);
                if (null != widget)
                {
                    CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                    if ((masterRoleInfo != null) && (masterRoleInfo.m_licenseInfo != null))
                    {
                        CUIListScript component = widget.GetComponent<CUIListScript>();
                        if (component != null)
                        {
                            component.SetElementAmount(masterRoleInfo.m_licenseInfo.m_licenseList.Count);
                        }
                    }
                }
            }
        }

        private void RefreshSelectedAchieveElement(CUIListElementScript elementScript, CAchieveItem2 item, int index, bool isSelf, ListView<CAchieveItem2> filteredTrophies)
        {
            GameObject widget = elementScript.GetWidget(0);
            Image component = widget.GetComponent<Image>();
            GameObject obj3 = elementScript.GetWidget(1);
            GameObject obj4 = elementScript.GetWidget(2);
            Text text = obj4.GetComponent<Text>();
            GameObject obj5 = elementScript.GetWidget(3);
            Text text2 = obj5.GetComponent<Text>();
            GameObject obj6 = elementScript.GetWidget(4);
            GameObject obj7 = elementScript.GetWidget(5);
            CUIEventScript script = elementScript.GetComponent<CUIEventScript>();
            if (item == null)
            {
                widget.CustomSetActive(false);
                obj7.CustomSetActive(false);
                obj6.CustomSetActive(isSelf && (filteredTrophies.Count > 0));
                obj3.CustomSetActive(false);
                obj4.CustomSetActive(false);
                obj5.CustomSetActive(!isSelf || (filteredTrophies.Count == 0));
                text2.set_text(!isSelf ? Singleton<CTextManager>.GetInstance().GetText("Achievement_Status_Not_Chosen") : Singleton<CTextManager>.GetInstance().GetText("Achievement_Status_Not_Done"));
                if (filteredTrophies.Count > 0)
                {
                    script.set_enabled(true);
                    stUIEventParams eventParams = new stUIEventParams();
                    eventParams.tag = index;
                    script.SetUIEvent(enUIEventType.Click, enUIEventID.Achievement_Change_Selected_Trophy, eventParams);
                }
                else
                {
                    script.set_enabled(false);
                }
            }
            else
            {
                widget.CustomSetActive(true);
                obj7.CustomSetActive(true);
                obj6.CustomSetActive(false);
                obj3.CustomSetActive(isSelf);
                obj4.CustomSetActive(true);
                obj5.CustomSetActive(true);
                CAchieveItem2 achieveItem = item.TryToGetMostRecentlyDoneItem();
                text.set_text(achieveItem.Cfg.szName);
                component.SetSprite(achieveItem.GetAchieveImagePath(), elementScript.m_belongedFormScript, true, false, false, false);
                CAchievementSystem.SetAchieveBaseIcon(obj7.get_transform(), achieveItem, elementScript.m_belongedFormScript);
                if (achieveItem.DoneTime == 0)
                {
                    text2.set_text(Singleton<CTextManager>.GetInstance().GetText("Achievement_Status_Done"));
                }
                else
                {
                    text2.set_text(string.Format("{0:yyyy.M.d} {1}", Utility.ToUtcTime2Local((long) achieveItem.DoneTime), Singleton<CTextManager>.GetInstance().GetText("Achievement_Status_Done")));
                }
                script.set_enabled(true);
                stUIEventParams params2 = new stUIEventParams();
                params2.commonUInt32Param1 = item.ID;
                script.SetUIEvent(enUIEventType.Click, enUIEventID.Player_Info_Achievement_Trophy_Click, params2);
                if (isSelf)
                {
                    CUIEventScript script2 = obj3.GetComponent<CUIEventScript>();
                    stUIEventParams params3 = new stUIEventParams();
                    params3.tag = index;
                    script2.SetUIEvent(enUIEventType.Click, enUIEventID.Achievement_Change_Selected_Trophy, params3);
                }
            }
        }

        private void ReqOtherPlayerDetailInfo(ulong ullUid, int iLogicWorldId)
        {
            if (ullUid > 0L)
            {
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0xa2e);
                msg.stPkgData.stGetAcntDetailInfoReq.ullUid = ullUid;
                msg.stPkgData.stGetAcntDetailInfoReq.iLogicWorldId = iLogicWorldId;
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
            }
        }

        [MessageHandler(0xa2f)]
        public static void ResPlyaerDetailInfo(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            if (msg.stPkgData.stGetAcntDetailInfoRsp.iErrCode == 0)
            {
                if (Singleton<CPlayerInfoSystem>.GetInstance().isShowPlayerInfoDirectly)
                {
                    Singleton<CPlayerInfoSystem>.instance.ImpResDetailInfo(msg);
                }
                else
                {
                    Singleton<EventRouter>.GetInstance().BroadCastEvent<CSPkg>(EventID.PlayerInfoSystem_Info_Received, msg);
                }
            }
            else
            {
                Singleton<CUIManager>.GetInstance().OpenTips(Utility.ProtErrCodeToStr(0xa2f, 0xa3), false, 1.5f, null, new object[0]);
            }
        }

        private void SetAllFriendBtnActive(GameObject root, bool isActive)
        {
            GameObject obj2 = Utility.FindChild(root, "pnlContainer/BtnGroup/btnSettings");
            GameObject obj3 = Utility.FindChild(root, "pnlContainer/BtnGroup/btnQuit");
            obj2.CustomSetActive(isActive);
            obj3.CustomSetActive(isActive);
        }

        private void SetAllGuildBtnActive(GameObject root, bool isActive)
        {
            GameObject obj2 = Utility.FindChild(root, "pnlContainer/BtnGroup/btnAddFriend");
            GameObject obj3 = Utility.FindChild(root, "pnlContainer/BtnGroup/btnAppointViceChairman");
            GameObject obj4 = Utility.FindChild(root, "pnlContainer/BtnGroup/btnAppointOrCancelMatchLeader");
            GameObject obj5 = Utility.FindChild(root, "pnlContainer/BtnGroup/btnTransferPosition");
            GameObject obj6 = Utility.FindChild(root, "pnlContainer/btnFireMember");
            obj2.CustomSetActive(isActive);
            obj3.CustomSetActive(isActive);
            obj4.CustomSetActive(isActive);
            obj5.CustomSetActive(isActive);
            obj6.CustomSetActive(isActive);
        }

        public void SetAppointMatchLeaderBtn()
        {
            if (this.m_Form != null)
            {
                GameObject widget = this.m_Form.GetWidget(14);
                if (CGuildSystem.HasAppointMatchLeaderAuthority())
                {
                    widget.CustomSetActive(true);
                    this.SetAppointMatchLeaderBtnText(widget);
                }
                else
                {
                    widget.CustomSetActive(false);
                }
            }
        }

        private void SetAppointMatchLeaderBtnText(GameObject btnAppointOrCancelMatchLeader)
        {
            CUICommonSystem.SetButtonName(btnAppointOrCancelMatchLeader, !CGuildHelper.IsGuildMatchLeaderPosition(this.m_PlayerProfile.m_uuid) ? Singleton<CTextManager>.GetInstance().GetText("GuildMatch_Apooint_Leader") : Singleton<CTextManager>.GetInstance().GetText("GuildMatch_Cancel_Leader"));
        }

        private void SetBaseInfoScrollable(bool scrollable = false)
        {
            if (this.m_IsFormOpen && (this.m_Form != null))
            {
                GameObject widget = this.m_Form.GetWidget(1);
                if ((widget != null) && widget.get_activeSelf())
                {
                    GameObject obj3 = Utility.FindChild(widget, "pnlContainer/pnlInfo/scrollRect");
                    if (obj3 != null)
                    {
                        RectTransform component = obj3.GetComponent<RectTransform>();
                        ScrollRect rect = obj3.GetComponent<ScrollRect>();
                        if (component != null)
                        {
                            if (scrollable)
                            {
                                component.set_offsetMin(new Vector2(component.get_offsetMin().x, 90f));
                            }
                            else
                            {
                                component.set_offsetMin(new Vector2(component.get_offsetMin().x, 0f));
                            }
                        }
                        if (rect != null)
                        {
                            rect.set_verticalNormalizedPosition(1f);
                        }
                    }
                }
            }
        }

        public void SetHeroItemData(CUIFormScript formScript, GameObject listItem, COMDT_MOST_USED_HERO_INFO heroInfo)
        {
            if ((listItem != null) && (heroInfo != null))
            {
                IHeroData data = CHeroDataFactory.CreateHeroData(heroInfo.dwHeroID);
                Transform transform = listItem.get_transform();
                ResHeroProficiency heroProficiency = CHeroInfo.GetHeroProficiency(data.heroType, (int) heroInfo.dwProficiencyLv);
                if (heroProficiency != null)
                {
                    listItem.GetComponent<Image>().SetSprite(string.Format("{0}{1}", "UGUI/Sprite/Dynamic/Quality/", StringHelper.UTF8BytesToString(ref heroProficiency.szImagePath)), formScript, true, false, false, false);
                }
                string heroSkinPic = CSkinInfo.GetHeroSkinPic(heroInfo.dwHeroID, 0);
                CUICommonSystem.SetHeroItemImage(formScript, listItem, heroSkinPic, enHeroHeadType.enIcon, false, false);
                GameObject root = transform.Find("profession").get_gameObject();
                CUICommonSystem.SetHeroJob(formScript, root, (enHeroJobType) data.heroType);
                transform.Find("heroNameText").GetComponent<Text>().set_text(data.heroName);
                CUIEventScript component = listItem.GetComponent<CUIEventScript>();
                stUIEventParams eventParams = new stUIEventParams();
                eventParams.openHeroFormPar.heroId = data.cfgID;
                eventParams.openHeroFormPar.openSrc = enHeroFormOpenSrc.HeroListClick;
                component.SetUIEvent(enUIEventType.Click, enUIEventID.Player_Info_Most_Used_Hero_Item_Click, eventParams);
            }
        }

        private void SetSingleGuildBtn(GameObject root)
        {
            GameObject obj2 = Utility.FindChild(root, "pnlContainer/BtnGroup/btnAddFriend");
            GameObject obj3 = Utility.FindChild(root, "pnlContainer/BtnGroup/btnAppointViceChairman");
            GameObject obj4 = Utility.FindChild(root, "pnlContainer/BtnGroup/btnTransferPosition");
            GameObject obj5 = Utility.FindChild(root, "pnlContainer/btnFireMember");
            obj2.CustomSetActive(true);
            obj3.CustomSetActive(this._isShowGuildAppointViceChairmanBtn);
            obj4.CustomSetActive(this._isShowGuildTransferPositionBtn);
            obj5.CustomSetActive(this._isShowGuildFireMemberBtn);
            this.SetAppointMatchLeaderBtn();
        }

        private void SetTitle(Tab tab, Transform titleTransform)
        {
            if (titleTransform != null)
            {
                Text component = titleTransform.GetComponent<Text>();
                if (component != null)
                {
                    switch (tab)
                    {
                        case Tab.Base_Info:
                            component.set_text(Singleton<CTextManager>.GetInstance().GetText("Player_Info_Tab_Base_Info"));
                            break;

                        case Tab.Pvp_Info:
                            component.set_text(Singleton<CTextManager>.GetInstance().GetText("Player_Info_Tab_Pvp_Info"));
                            break;

                        case Tab.Honor_Info:
                            component.set_text(Singleton<CTextManager>.GetInstance().GetText("Player_Info_Tab_Honor_Info"));
                            break;

                        case Tab.Common_Hero:
                            component.set_text(Singleton<CTextManager>.GetInstance().GetText("Player_Info_Tab_Common_Hero_Info"));
                            break;

                        case Tab.PvpHistory_Info:
                            component.set_text(Singleton<CTextManager>.GetInstance().GetText("Player_Info_Tab_PvpHistory_Info"));
                            break;

                        case Tab.PvpCreditScore_Info:
                            component.set_text(Singleton<CTextManager>.GetInstance().GetText("Player_Info_Tab_Credit_Info"));
                            break;

                        default:
                            component.set_text(Singleton<CTextManager>.GetInstance().GetText("Player_Info_Title"));
                            break;
                    }
                }
            }
        }

        public void ShowPlayerDetailInfo(ulong ullUid, int iLogicWorldId, DetailPlayerInfoSource sourceType = 0, bool isShowDirectly = true)
        {
            this._lastDetailSource = sourceType;
            if (this._lastDetailSource == DetailPlayerInfoSource.Self)
            {
                this.m_PlayerProfile.ConvertRoleInfoData(Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo());
                this.OpenForm();
            }
            else if (ullUid > 0L)
            {
                this.isShowPlayerInfoDirectly = isShowDirectly;
                this.ReqOtherPlayerDetailInfo(ullUid, iLogicWorldId);
            }
        }

        public void ShowPlayerDetailInfo(ulong ullUid, int iLogicWorldId, bool isShowGuildAppointViceChairmanBtn, bool isShowGuildTransferPositionBtn, bool isShowGuildFireMemberBtn)
        {
            this._isShowGuildAppointViceChairmanBtn = isShowGuildAppointViceChairmanBtn;
            this._isShowGuildTransferPositionBtn = isShowGuildTransferPositionBtn;
            this._isShowGuildFireMemberBtn = isShowGuildFireMemberBtn;
            this.ShowPlayerDetailInfo(ullUid, iLogicWorldId, DetailPlayerInfoSource.Guild, true);
        }

        public override void UnInit()
        {
            base.UnInit();
            CUIEventManager instance = Singleton<CUIEventManager>.GetInstance();
            instance.RemoveUIEventListener(enUIEventID.Player_Info_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfo_OpenForm));
            instance.RemoveUIEventListener(enUIEventID.Player_Info_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfo_CloseForm));
            instance.RemoveUIEventListener(enUIEventID.Player_Info_Open_Pvp_Info, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfoOpenPvpInfo));
            instance.RemoveUIEventListener(enUIEventID.Player_Info_Open_Base_Info, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfoOpenBaseInfo));
            instance.RemoveUIEventListener(enUIEventID.BuyPick_QQ_VIP, new CUIEventManager.OnUIEventHandler(this.OpenByQQVIP));
            instance.RemoveUIEventListener(enUIEventID.Player_Info_Quit_Game, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfoQuitGame));
            instance.RemoveUIEventListener(enUIEventID.Player_Info_Quit_Game_Confirm, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfoQuitGameConfirm));
            instance.RemoveUIEventListener(enUIEventID.Player_Info_Most_Used_Hero_Item_Enable, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfoMostUsedHeroItemEnable));
            instance.RemoveUIEventListener(enUIEventID.Player_Info_Most_Used_Hero_Item_Click, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfoMostUsedHeroItemClick));
            instance.RemoveUIEventListener(enUIEventID.Player_Info_Show_Rule, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfoShowRule));
            instance.RemoveUIEventListener(enUIEventID.Player_Info_License_ListItem_Enable, new CUIEventManager.OnUIEventHandler(this.OnLicenseListItemEnable));
            instance.RemoveUIEventListener(enUIEventID.Player_Info_Update_Sub_Module, new CUIEventManager.OnUIEventHandler(this.OnUpdateSubModule));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.NOBE_STATE_CHANGE, new Action(this, (IntPtr) this.UpdateNobeHeadIdx));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.HEAD_IMAGE_FLAG_CHANGE, new Action(this, (IntPtr) this.UpdateHeadFlag));
            instance.RemoveUIEventListener(enUIEventID.DeepLink_OnClick, new CUIEventManager.OnUIEventHandler(this.DeepLinkClick));
            instance.RemoveUIEventListener(enUIEventID.WEB_IntegralHall, new CUIEventManager.OnUIEventHandler(this.OpenIntegralHall));
            instance.RemoveUIEventListener(enUIEventID.OPEN_QQ_Buluo, new CUIEventManager.OnUIEventHandler(this.OpenQQBuluo));
            instance.RemoveUIEventListener(enUIEventID.Player_Info_Achievement_Trophy_Click, new CUIEventManager.OnUIEventHandler(this.OnAchievementTrophyClick));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<byte, CAchieveItem2>(EventID.ACHIEVE_SERY_SELECT_DONE, new Action<byte, CAchieveItem2>(this, (IntPtr) this.OnTrophySelectDone));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.GAMER_REDDOT_CHANGE, new Action(this, (IntPtr) this.UpdateXinYueBtn));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.ACHIEVE_TROPHY_REWARD_INFO_STATE_CHANGE, new Action(this, (IntPtr) this.OnTrophyStateChange));
        }

        private void UpdateBaseInfo()
        {
            if (this.m_IsFormOpen && (this.m_Form != null))
            {
                GameObject widget = this.m_Form.GetWidget(1);
                if (widget != null)
                {
                    this.DisplayCustomButton();
                    if (!CSysDynamicBlock.bSocialBlocked)
                    {
                        GameObject obj3 = Utility.FindChild(widget, "pnlContainer/pnlHead/pnlImg/HttpImage");
                        if ((obj3 != null) && !string.IsNullOrEmpty(this.m_PlayerProfile.HeadUrl()))
                        {
                            CUIHttpImageScript script = obj3.GetComponent<CUIHttpImageScript>();
                            script.SetImageUrl(this.m_PlayerProfile.HeadUrl());
                            MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(script.GetComponent<Image>(), (int) this.m_PlayerProfile.m_vipInfo.stGameVipClient.dwCurLevel, false);
                            GameObject obj4 = Utility.FindChild(widget, "pnlContainer/pnlHead/pnlImg/HttpImage/NobeImag");
                            if (obj4 != null)
                            {
                                MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(obj4.GetComponent<Image>(), (int) this.m_PlayerProfile.m_vipInfo.stGameVipClient.dwHeadIconId);
                                MonoSingleton<NobeSys>.GetInstance().SetHeadIconBkEffect(obj4.GetComponent<Image>(), (int) this.m_PlayerProfile.m_vipInfo.stGameVipClient.dwHeadIconId, this.m_Form, 0.9f);
                            }
                        }
                    }
                    this.UpdateHeadFlag();
                    COM_PRIVILEGE_TYPE privilegeType = this.m_PlayerProfile.PrivilegeType();
                    MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(Utility.FindChild(widget, "pnlContainer/pnlHead/NameGroup/WXGameCenterIcon"), privilegeType, ApolloPlatform.Wechat, true, false);
                    COM_PRIVILEGE_TYPE com_privilege_type2 = (privilegeType != COM_PRIVILEGE_TYPE.COM_PRIVILEGE_TYPE_WXGAMECENTER_LOGIN) ? COM_PRIVILEGE_TYPE.COM_PRIVILEGE_TYPE_WXGAMECENTER_LOGIN : COM_PRIVILEGE_TYPE.COM_PRIVILEGE_TYPE_NONE;
                    MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(Utility.FindChild(widget, "pnlContainer/WXGameCenter/WXGameCenterBtn"), com_privilege_type2, ApolloPlatform.Wechat, true, false);
                    MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(Utility.FindChild(widget, "pnlContainer/pnlHead/NameGroup/QQGameCenterIcon"), privilegeType, ApolloPlatform.QQ, true, false);
                    MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(Utility.FindChild(widget, "pnlContainer/QQGameCenter/QQGameCenterBtn"), privilegeType, ApolloPlatform.QQ, true, false);
                    COM_PRIVILEGE_TYPE com_privilege_type3 = (privilegeType != COM_PRIVILEGE_TYPE.COM_PRIVILEGE_TYPE_QQGAMECENTER_LOGIN) ? COM_PRIVILEGE_TYPE.COM_PRIVILEGE_TYPE_QQGAMECENTER_LOGIN : COM_PRIVILEGE_TYPE.COM_PRIVILEGE_TYPE_NONE;
                    MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(Utility.FindChild(widget, "pnlContainer/QQGameCenter/QQGameCenterGrey"), com_privilege_type3, ApolloPlatform.QQ, true, false);
                    MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(Utility.FindChild(widget, "pnlContainer/pnlHead/NameGroup/GuestGameCenterIcon"), privilegeType, ApolloPlatform.Guest, true, false);
                    MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(Utility.FindChild(widget, "pnlContainer/GuestGameCenter/GuestGameCenterBtn"), privilegeType, ApolloPlatform.Guest, true, false);
                    Text componetInChild = Utility.GetComponetInChild<Text>(widget, "pnlContainer/pnlHead/Level");
                    if (componetInChild != null)
                    {
                        componetInChild.set_text(string.Format(Singleton<CTextManager>.GetInstance().GetText("ranking_PlayerLevel"), this.m_PlayerProfile.PvpLevel()));
                    }
                    Text text2 = Utility.GetComponetInChild<Text>(widget, "pnlContainer/pnlInfo/labelGeiLiDuiYouCnt/numCnt");
                    if (text2 != null)
                    {
                        text2.set_text(this.m_PlayerProfile._geiLiDuiYou.ToString());
                    }
                    Text text3 = Utility.GetComponetInChild<Text>(widget, "pnlContainer/pnlInfo/labelKeJingDuiShouCnt/numCnt");
                    if (text3 != null)
                    {
                        text3.set_text(this.m_PlayerProfile._keJingDuiShou.ToString());
                    }
                    Text text4 = Utility.GetComponetInChild<Text>(widget, "pnlContainer/pnlInfo/labelHeroCnt/numCnt");
                    if (text4 != null)
                    {
                        text4.set_text(this.m_PlayerProfile.HeroCnt().ToString());
                    }
                    Text text5 = Utility.GetComponetInChild<Text>(widget, "pnlContainer/pnlInfo/labelSkinCnt/numCnt");
                    if (text5 != null)
                    {
                        text5.set_text(this.m_PlayerProfile.SkinCnt().ToString());
                    }
                    Image component = Utility.FindChild(widget, "pnlContainer/pnlHead/NameGroup/Gender").GetComponent<Image>();
                    component.get_gameObject().CustomSetActive(this.m_PlayerProfile.Gender() != COM_SNSGENDER.COM_SNSGENDER_NONE);
                    if (this.m_PlayerProfile.Gender() == COM_SNSGENDER.COM_SNSGENDER_MALE)
                    {
                        CUIUtility.SetImageSprite(component, string.Format("{0}icon/Ico_boy.prefab", "UGUI/Sprite/Dynamic/"), null, true, false, false, false);
                    }
                    else if (this.m_PlayerProfile.Gender() == COM_SNSGENDER.COM_SNSGENDER_FEMALE)
                    {
                        CUIUtility.SetImageSprite(component, string.Format("{0}icon/Ico_girl.prefab", "UGUI/Sprite/Dynamic/"), null, true, false, false, false);
                    }
                    Text text6 = Utility.GetComponetInChild<Text>(widget, "pnlContainer/pnlHead/NameGroup/txtName");
                    if (text6 != null)
                    {
                        text6.set_text(this.m_PlayerProfile.Name());
                    }
                    GameObject obj5 = Utility.FindChild(widget, "pnlContainer/pnlHead/Status/Rank");
                    if (this.m_PlayerProfile.GetRankGrade() == 0)
                    {
                        if (obj5 != null)
                        {
                            obj5.CustomSetActive(false);
                        }
                    }
                    else
                    {
                        obj5.CustomSetActive(true);
                        Image image2 = null;
                        Image image3 = null;
                        if (obj5 != null)
                        {
                            image2 = Utility.GetComponetInChild<Image>(obj5, "ImgRank");
                            image3 = Utility.GetComponetInChild<Image>(obj5, "ImgRank/ImgSubRank");
                        }
                        if (image2 != null)
                        {
                            string rankSmallIconPath = CLadderView.GetRankSmallIconPath(this.m_PlayerProfile.GetRankGrade(), this.m_PlayerProfile.GetRankClass());
                            image2.SetSprite(rankSmallIconPath, this.m_Form, true, false, false, false);
                        }
                        if (image3 != null)
                        {
                            string subRankSmallIconPath = CLadderView.GetSubRankSmallIconPath(this.m_PlayerProfile.GetRankGrade(), this.m_PlayerProfile.GetRankClass());
                            image3.SetSprite(subRankSmallIconPath, this.m_Form, true, false, false, false);
                        }
                    }
                    GameObject obj6 = Utility.FindChild(widget, "pnlContainer/pnlHead/Status/HisRank");
                    if (this.m_PlayerProfile.GetHistoryHighestRankGrade() == 0)
                    {
                        if (obj6 != null)
                        {
                            obj6.CustomSetActive(false);
                        }
                    }
                    else
                    {
                        obj6.CustomSetActive(true);
                        Image image4 = null;
                        Image image5 = null;
                        if (obj6 != null)
                        {
                            image4 = Utility.GetComponetInChild<Image>(obj6, "ImgRank");
                            image5 = Utility.GetComponetInChild<Image>(obj6, "ImgRank/ImgSubRank");
                        }
                        if (image4 != null)
                        {
                            string prefabPath = CLadderView.GetRankSmallIconPath(this.m_PlayerProfile.GetHistoryHighestRankGrade(), this.m_PlayerProfile.GetHistoryHighestRankClass());
                            image4.SetSprite(prefabPath, this.m_Form, true, false, false, false);
                        }
                        if (image5 != null)
                        {
                            string str4 = CLadderView.GetSubRankSmallIconPath(this.m_PlayerProfile.GetHistoryHighestRankGrade(), this.m_PlayerProfile.GetHistoryHighestRankClass());
                            image5.SetSprite(str4, this.m_Form, true, false, false, false);
                        }
                    }
                    Utility.FindChild(widget, "pnlContainer/pnlHead/Status/ExtraCoin").CustomSetActive(false);
                    Utility.FindChild(widget, "pnlContainer/pnlHead/Status/ExtraExp").CustomSetActive(false);
                    GameObject obj9 = Utility.FindChild(widget, "pnlContainer/pnlHead/GuildInfo/Name");
                    GameObject p = Utility.FindChild(widget, "pnlContainer/pnlHead/GuildInfo/Position");
                    if ((obj9 != null) && (p != null))
                    {
                        Text text7 = obj9.GetComponent<Text>();
                        Text text8 = Utility.GetComponetInChild<Text>(p, "Text");
                        if (!CGuildSystem.IsInNormalGuild(this.m_PlayerProfile.GuildState) || string.IsNullOrEmpty(this.m_PlayerProfile.GuildName))
                        {
                            if (text7 != null)
                            {
                                text7.set_text(Singleton<CTextManager>.GetInstance().GetText("PlayerInfo_Guild"));
                            }
                            p.CustomSetActive(false);
                        }
                        else
                        {
                            if (text7 != null)
                            {
                                text7.set_text(this.m_PlayerProfile.GuildName);
                            }
                            p.CustomSetActive(true);
                            if (text8 != null)
                            {
                                text8.set_text(CGuildHelper.GetPositionName(this.m_PlayerProfile.GuildState));
                            }
                        }
                    }
                    bool bActive = isSelf(this.m_PlayerProfile.m_uuid);
                    this.m_Form.GetWidget(3).CustomSetActive(bActive);
                    GameObject obj12 = this.m_Form.GetWidget(13);
                    InputField field = obj12.GetComponent<InputField>();
                    obj12.CustomSetActive(true);
                    if (field != null)
                    {
                        if (string.IsNullOrEmpty(this.m_PlayerProfile.m_personSign))
                        {
                            obj12.CustomSetActive(bActive);
                            field.set_text(string.Empty);
                        }
                        else
                        {
                            field.set_text(this.m_PlayerProfile.m_personSign);
                        }
                        if (bActive)
                        {
                            field.set_interactable(true);
                            field.get_onEndEdit().RemoveAllListeners();
                            field.get_onEndEdit().AddListener(new UnityAction<string>(this, (IntPtr) this.OnPersonSignEndEdit));
                        }
                        else
                        {
                            field.set_interactable(false);
                        }
                    }
                    GameObject obj13 = Utility.FindChild(widget, "pnlContainer/BtnGroup/JFQBtn");
                    if ((ApolloConfig.platform == ApolloPlatform.QQ) || (ApolloConfig.platform == ApolloPlatform.WTLogin))
                    {
                        if (!CSysDynamicBlock.bJifenHallBlock)
                        {
                            obj13.CustomSetActive(bActive);
                        }
                        else
                        {
                            obj13.CustomSetActive(false);
                        }
                    }
                    else
                    {
                        obj13.CustomSetActive(false);
                    }
                    GameObject obj14 = Utility.FindChild(widget, "pnlContainer/BtnGroup/BuLuoBtn");
                    if ((ApolloConfig.platform == ApolloPlatform.QQ) || (ApolloConfig.platform == ApolloPlatform.WTLogin))
                    {
                        obj14.CustomSetActive(bActive);
                        if (obj14 != null)
                        {
                            Transform transform = obj14.get_transform().Find("Text");
                            if (transform != null)
                            {
                                transform.GetComponent<Text>().set_text("QQ部落");
                            }
                        }
                    }
                    else if (ApolloConfig.platform == ApolloPlatform.Wechat)
                    {
                        if (MonoSingleton<PandroaSys>.GetInstance().m_bShowWeixinZone)
                        {
                            obj14.CustomSetActive(bActive);
                            if (obj14 != null)
                            {
                                Transform transform2 = obj14.get_transform().Find("Text");
                                if (transform2 != null)
                                {
                                    transform2.GetComponent<Text>().set_text("游戏圈");
                                }
                            }
                        }
                        else
                        {
                            obj14.CustomSetActive(false);
                        }
                    }
                    else
                    {
                        obj14.CustomSetActive(false);
                    }
                    if (MonoSingleton<BannerImageSys>.GetInstance().IsWaifaBlockChannel())
                    {
                        obj14.CustomSetActive(false);
                    }
                    GameObject obj15 = Utility.FindChild(widget, "pnlContainer/BtnGroup/DeepLinkBtn");
                    if ((ApolloConfig.platform == ApolloPlatform.QQ) || (ApolloConfig.platform == ApolloPlatform.WTLogin))
                    {
                        obj15.CustomSetActive(false);
                    }
                    else
                    {
                        long currentUTCTime = CRoleInfo.GetCurrentUTCTime();
                        if (MonoSingleton<BannerImageSys>.GetInstance().DeepLinkInfo.isTimeValid(currentUTCTime))
                        {
                            obj15.CustomSetActive(bActive);
                        }
                        else
                        {
                            obj15.CustomSetActive(false);
                        }
                    }
                    if (MonoSingleton<BannerImageSys>.GetInstance().IsWaifaBlockChannel())
                    {
                        obj15.CustomSetActive(false);
                    }
                    this.UpdateXinYueBtn();
                    if (CSysDynamicBlock.bLobbyEntryBlocked)
                    {
                        Transform transform3 = widget.get_transform().Find("pnlContainer/pnlHead/changeNobeheadicon");
                        if (transform3 != null)
                        {
                            transform3.get_gameObject().CustomSetActive(false);
                        }
                        Transform transform4 = widget.get_transform().Find("pnlContainer/BtnGroup/BuLuoBtn");
                        if (transform4 != null)
                        {
                            transform4.get_gameObject().CustomSetActive(false);
                        }
                        Transform transform5 = widget.get_transform().Find("pnlContainer/BtnGroup/QQVIPBtn");
                        if (transform5 != null)
                        {
                            transform5.get_gameObject().CustomSetActive(false);
                        }
                        Transform transform6 = widget.get_transform().Find("pnlContainer/BtnGroup/JFQBtn");
                        if (transform6 != null)
                        {
                            transform6.get_gameObject().CustomSetActive(false);
                        }
                    }
                    Image image = Utility.GetComponetInChild<Image>(widget, "pnlContainer/pnlTrophy/TrophyInfo/Image/Icon");
                    Text text9 = Utility.GetComponetInChild<Text>(widget, "pnlContainer/pnlTrophy/TrophyInfo/Trophy/Level");
                    GameObject obj16 = Utility.FindChild(widget, "pnlContainer/pnlTrophy/TrophyInfo/Trophy/Rank");
                    Text text10 = Utility.GetComponetInChild<Text>(widget, "pnlContainer/pnlTrophy/TrophyInfo/Trophy/Rank");
                    GameObject obj17 = Utility.FindChild(widget, "pnlContainer/pnlTrophy/TrophyInfo/Button");
                    GameObject obj18 = Utility.FindChild(widget, "pnlContainer/pnlTrophy/TrophyInfo/Trophy/txtNotInRank");
                    if (isSelf(this.m_PlayerProfile.m_uuid))
                    {
                        obj17.CustomSetActive(true);
                    }
                    else
                    {
                        obj17.CustomSetActive(false);
                    }
                    if (image != null)
                    {
                        CAchieveInfo2 info = CAchieveInfo2.GetAchieveInfo(this.m_PlayerProfile.m_iLogicWorldId, this.m_PlayerProfile.m_uuid, false);
                        if (info.LastDoneTrophyRewardInfo != null)
                        {
                            image.SetSprite(info.LastDoneTrophyRewardInfo.GetTrophyImagePath(), this.m_Form, true, false, false, false);
                        }
                    }
                    if (text9 != null)
                    {
                        text9.set_text(this.m_PlayerProfile._trophyRewardInfoLevel.ToString());
                    }
                    if (text10 != null)
                    {
                        if (this.m_PlayerProfile._trophyRank == 0)
                        {
                            obj18.CustomSetActive(true);
                            obj16.CustomSetActive(false);
                        }
                        else
                        {
                            obj18.CustomSetActive(false);
                            obj16.CustomSetActive(true);
                            text10.set_text(this.m_PlayerProfile._trophyRank.ToString());
                        }
                    }
                    this.ProcessSelectedTrophies();
                }
            }
        }

        private void UpdateHeadFlag()
        {
            if ((this.m_IsFormOpen && (this.m_Form != null)) && (this.m_Form.GetWidget(1) != null))
            {
                GameObject target = Utility.FindChild(this.m_Form.get_gameObject(), "pnlBg/pnlBody/pnlBaseInfo/pnlContainer/pnlHead/changeNobeheadicon");
                if (target != null)
                {
                    if (Singleton<HeadIconSys>.instance.UnReadFlagNum > 0)
                    {
                        CUICommonSystem.AddRedDot(target, enRedDotPos.enTopRight, 0);
                    }
                    else
                    {
                        CUICommonSystem.DelRedDot(target);
                    }
                }
            }
        }

        private void UpdateNobeHeadIdx()
        {
            if (this.m_IsFormOpen && (this.m_Form != null))
            {
                GameObject widget = this.m_Form.GetWidget(1);
                if (widget != null)
                {
                    GameObject obj3 = Utility.FindChild(widget, "pnlContainer/pnlHead/pnlImg/HttpImage/NobeImag");
                    if (obj3 != null)
                    {
                        MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(obj3.GetComponent<Image>(), (int) MonoSingleton<NobeSys>.GetInstance().m_vipInfo.stGameVipClient.dwHeadIconId);
                        MonoSingleton<NobeSys>.GetInstance().SetHeadIconBkEffect(obj3.GetComponent<Image>(), (int) MonoSingleton<NobeSys>.GetInstance().m_vipInfo.stGameVipClient.dwHeadIconId, this.m_Form, 0.9f);
                    }
                }
            }
        }

        private void UpdatePvpInfo2()
        {
            Singleton<CPlayerPvpInfoController>.instance.UpdateUI();
        }

        private void UpdateXinYueBtn()
        {
            if (this.m_IsFormOpen && (this.m_Form != null))
            {
                GameObject widget = this.m_Form.GetWidget(1);
                if (widget != null)
                {
                    Transform transform = widget.get_transform().Find("pnlContainer/BtnGroup/XYJLBBtn");
                    if (transform != null)
                    {
                        transform.get_gameObject().CustomSetActive(!CSysDynamicBlock.bLobbyEntryBlocked);
                        CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                        if (masterRoleInfo != null)
                        {
                            if (masterRoleInfo.ShowGameRedDot)
                            {
                                CUICommonSystem.AddRedDot(transform.get_gameObject(), enRedDotPos.enTopRight, 0);
                            }
                            else
                            {
                                CUICommonSystem.DelRedDot(transform.get_gameObject());
                            }
                        }
                    }
                }
            }
        }

        public Tab CurTab
        {
            get
            {
                return this.m_CurTab;
            }
            set
            {
                this.m_CurTab = value;
            }
        }

        public enum DetailPlayerInfoSource
        {
            DefaultOthers,
            Self,
            Guild
        }

        public enum enPlayerFormWidget
        {
            Tab,
            Base_Info_Tab,
            Pvp_Info_Tab,
            Change_Name_Button,
            CreditScore_Tab,
            License_Info_Tab,
            License_List,
            Common_Hero_info,
            Rule_Btn,
            Body,
            Juhua,
            Update_Sub_Module_Timer,
            Title,
            PersonSign,
            AppointOrCancelMatchLeader,
            ShareScreenShot
        }

        public enum Tab
        {
            Base_Info,
            Pvp_Info,
            Honor_Info,
            Common_Hero,
            PvpHistory_Info,
            PvpCreditScore_Info,
            Mentor_Info
        }
    }
}

