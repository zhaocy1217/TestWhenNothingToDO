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
    public class CAchievementSystem : Singleton<CAchievementSystem>
    {
        private static ListView<SCPKG_PVPBAN_NTF> CachedPunishMsg;
        private CAchieveItem2 m_CurAchieveItem;
        private int m_curAchievementType;
        private ListView<CAchieveItem2> m_CurAchieveSeries = new ListView<CAchieveItem2>();
        public enTypeMenu m_CurMenu;
        private CAchieveShareComponent m_shareComponent;
        private ListView<CTrophyRewardInfo> m_TrophyRewardInfoWithRewardList;
        private string[] m_TypeMenuNameKeys = new string[] { Singleton<CTextManager>.GetInstance().GetText("Achievement_Trophy_Menu_All"), Singleton<CTextManager>.GetInstance().GetText("Achievement_Trophy_Menu_Not_Done") };
        private const string OverviewFormPrefabPath = "UGUI/Form/System/Achieve/Form_Trophy_Overview.prefab";
        public const ushort TROPHY_RULE_ID = 0x10;
        private const string TrophyDetailFormPrefabPath = "UGUI/Form/System/Achieve/Form_Trophy_Detail.prefab";
        private const string TrophyRewardsFormPrefabPath = "UGUI/Form/System/Achieve/Form_Trophy_Rewards.prefab";

        public static void AddPunishMsg(SCPKG_PVPBAN_NTF msg)
        {
            if (CachedPunishMsg == null)
            {
                CachedPunishMsg = new ListView<SCPKG_PVPBAN_NTF>();
            }
            CachedPunishMsg.Add(msg);
            Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.ACHIEVE_RECEIVE_PVP_BAN_MSG);
        }

        private void HandleAchieveGetRankingAccountInfo(SCPKG_GET_RANKING_ACNT_INFO_RSP rsp)
        {
            if (rsp.stAcntRankingDetail.stOfSucc.bNumberType == 8)
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if (masterRoleInfo == null)
                {
                    DebugHelper.Assert(false, "HandleAchieveGetRankingAccountInfo::Master Role Info Is Null");
                    CAchieveInfo2.AddWorldRank(0, 0L, rsp.stAcntRankingDetail.stOfSucc.dwRankNo);
                }
                else
                {
                    CAchieveInfo2.AddWorldRank(masterRoleInfo.logicWorldID, masterRoleInfo.playerUllUID, rsp.stAcntRankingDetail.stOfSucc.dwRankNo);
                }
            }
        }

        private void HandleLobbyStateEnter()
        {
            Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.ACHIEVE_TROPHY_REWARD_INFO_STATE_CHANGE);
        }

        public override void Init()
        {
            base.Init();
            this.m_CurAchieveItem = null;
            this.m_shareComponent = new CAchieveShareComponent();
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Achievement_Open_Overview_Form, new CUIEventManager.OnUIEventHandler(this.OnAchievementOpenOverviewForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Achievement_ShowShareBtn, new CUIEventManager.OnUIEventHandler(this.OnAchievementShowShareBtn));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Achievement_Filter_Menu_Change, new CUIEventManager.OnUIEventHandler(this.OnMenuChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Achievement_Trophy_Enable, new CUIEventManager.OnUIEventHandler(this.OnTrophyEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Achievement_Trophy_Click, new CUIEventManager.OnUIEventHandler(this.OnTrophyClick));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Achievement_Item_Enable, new CUIEventManager.OnUIEventHandler(this.OnAchievementEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Achievement_Browse_All_Rewards, new CUIEventManager.OnUIEventHandler(this.OnBrowseAllRewards));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Achievement_Trophy_Reward_Info_Enable, new CUIEventManager.OnUIEventHandler(this.OnTrophyRewardInfoEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Achievement_Get_Trophy_Reward, new CUIEventManager.OnUIEventHandler(this.OnGetTrophyReward));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Achievement_Show_Rule, new CUIEventManager.OnUIEventHandler(this.OnShowRule));
            Singleton<EventRouter>.instance.AddEventHandler<SCPKG_GET_RANKING_ACNT_INFO_RSP>(EventID.ACHIEVE_GET_RANKING_ACCOUNT_INFO, new Action<SCPKG_GET_RANKING_ACNT_INFO_RSP>(this.HandleAchieveGetRankingAccountInfo));
            Singleton<EventRouter>.instance.AddEventHandler(EventID.LOBBY_STATE_ENTER, new Action(this, (IntPtr) this.HandleLobbyStateEnter));
            Singleton<EventRouter>.instance.AddEventHandler(EventID.ACHIEVE_TROPHY_REWARD_INFO_STATE_CHANGE, new Action(this, (IntPtr) this.OnTrophyStateChange));
            Singleton<EventRouter>.instance.AddEventHandler<RankingSystem.RankingType>("Ranking_List_Change", new Action<RankingSystem.RankingType>(this.RankingListChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.NOBE_LevelUp_Form_Close, new CUIEventManager.OnUIEventHandler(this.OnNobeLevelUpFormClose));
            Singleton<EventRouter>.instance.AddEventHandler(EventID.Mall_Get_Product_OK, new Action(this, (IntPtr) this.OnMallGetProductOk));
            Singleton<EventRouter>.instance.AddEventHandler(EventID.ACHIEVE_RECEIVE_PVP_BAN_MSG, new Action(this, (IntPtr) this.ProcessPvpBanMsg));
        }

        private void InitTypeMenu(CUIFormScript form)
        {
            int num = 2;
            CUIListScript component = form.GetWidget(0).GetComponent<CUIListScript>();
            component.SetElementAmount(2);
            for (int i = 0; i < num; i++)
            {
                GameObject widget = component.GetElemenet(i).GetWidget(0);
                if (widget != null)
                {
                    Text text = widget.GetComponent<Text>();
                    if (text != null)
                    {
                        text.set_text(Singleton<CTextManager>.GetInstance().GetText(this.m_TypeMenuNameKeys[i]));
                    }
                }
            }
            component.SelectElement(0, true);
        }

        private void OnAchievementEnable(CUIEvent uiEvent)
        {
            if (this.m_CurAchieveItem == null)
            {
                Singleton<CUIManager>.GetInstance().CloseForm("UGUI/Form/System/Achieve/Form_Trophy_Detail.prefab");
            }
            else
            {
                int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
                int achievementCnt = this.m_CurAchieveItem.GetAchievementCnt();
                CAchieveItem2 curAchieveItem = this.m_CurAchieveItem;
                bool bActive = srcWidgetIndexInBelongedList != (achievementCnt - 1);
                for (int i = 0; i < srcWidgetIndexInBelongedList; i++)
                {
                    curAchieveItem = curAchieveItem.Next;
                }
                CUIListElementScript srcWidgetScript = uiEvent.m_srcWidgetScript as CUIListElementScript;
                GameObject widget = srcWidgetScript.GetWidget(0);
                GameObject obj3 = srcWidgetScript.GetWidget(1);
                GameObject obj4 = srcWidgetScript.GetWidget(2);
                GameObject obj5 = srcWidgetScript.GetWidget(3);
                GameObject obj6 = srcWidgetScript.GetWidget(4);
                GameObject obj7 = srcWidgetScript.GetWidget(5);
                GameObject obj8 = srcWidgetScript.GetWidget(6);
                GameObject obj9 = srcWidgetScript.GetWidget(7);
                GameObject obj10 = srcWidgetScript.GetWidget(8);
                GameObject obj11 = srcWidgetScript.GetWidget(9);
                obj5.CustomSetActive(bActive);
                Image component = widget.GetComponent<Image>();
                Image image2 = obj10.GetComponent<Image>();
                Text text = obj3.GetComponent<Text>();
                Text text2 = obj4.GetComponent<Text>();
                Text text3 = obj11.GetComponent<Text>();
                Text text4 = obj6.GetComponent<Text>();
                Text text5 = obj7.GetComponent<Text>();
                if ((((component != null) && (text != null)) && ((text2 != null) && (text4 != null))) && (((text5 != null) && (image2 != null)) && (text3 != null)))
                {
                    component.SetSprite(CUIUtility.GetSpritePrefeb(curAchieveItem.GetAchieveImagePath(), false, false), false);
                    SetAchieveBaseIcon(obj10.get_transform(), curAchieveItem, null);
                    text.set_text(curAchieveItem.Cfg.szName);
                    text2.set_text(string.Format(Singleton<CTextManager>.GetInstance().GetText("Achievement_Score"), curAchieveItem.Cfg.dwPoint));
                    if (curAchieveItem.IsFinish())
                    {
                        if (curAchieveItem.DoneTime == 0)
                        {
                            obj11.CustomSetActive(false);
                        }
                        else
                        {
                            obj11.CustomSetActive(true);
                            text3.set_text(string.Format("{0:yyyy.M.d}", Utility.ToUtcTime2Local((long) curAchieveItem.DoneTime)));
                        }
                        text4.set_text(curAchieveItem.GetAchievementDesc());
                        text5.set_text(Singleton<CTextManager>.GetInstance().GetText("Achievement_Status_Done"));
                        obj8.CustomSetActive(true);
                        obj9.CustomSetActive(false);
                    }
                    else
                    {
                        obj11.CustomSetActive(false);
                        text4.set_text(curAchieveItem.GetAchievementDesc());
                        obj7.CustomSetActive(false);
                        obj8.CustomSetActive(false);
                        obj9.CustomSetActive(true);
                    }
                }
            }
        }

        private void OnAchievementOpenOverviewForm(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.GetInstance().CloseForm(CPlayerInfoSystem.sPlayerInfoFormPath);
            CUIFormScript form = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/System/Achieve/Form_Trophy_Overview.prefab", false, true);
            if (form != null)
            {
                this.InitTypeMenu(form);
                Singleton<CUINewFlagSystem>.instance.HideNewFlagForAchievementEntry();
                Singleton<CBattleGuideManager>.instance.OpenBannerDlgByBannerGuideId(0x12, null, false);
            }
        }

        private void OnAchievementShowShareBtn(CUIEvent uiEvent)
        {
            CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
            if (srcFormScript != null)
            {
                srcFormScript.GetWidget(2).CustomSetActive(true);
                srcFormScript.GetWidget(1).CustomSetActive(false);
            }
        }

        private void OnBrowseAllRewards(CUIEvent uiEvent)
        {
            CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/System/Achieve/Form_Trophy_Rewards.prefab", false, true);
            CAchieveInfo2 masterAchieveInfo = CAchieveInfo2.GetMasterAchieveInfo();
            CUIListScript component = script.GetWidget(0).GetComponent<CUIListScript>();
            if (component != null)
            {
                this.m_TrophyRewardInfoWithRewardList = masterAchieveInfo.GetTrophyRewardInfoWithRewards();
                this.m_TrophyRewardInfoWithRewardList.Sort(new CTrophyRewardInfoSort());
                component.SetElementAmount(this.m_TrophyRewardInfoWithRewardList.Count);
            }
        }

        private void OnGetTrophyReward(CUIEvent uiEvent)
        {
            int tag = uiEvent.m_eventParams.tag;
            CTrophyRewardInfo[] trophyRewardInfoArr = CAchieveInfo2.GetMasterAchieveInfo().TrophyRewardInfoArr;
            if ((tag >= 0) && (tag < trophyRewardInfoArr.Length))
            {
                CTrophyRewardInfo info2 = trophyRewardInfoArr[tag];
                this.SendGetTrophyRewardReq(info2.Cfg.dwTrophyLvl);
            }
        }

        private void OnMallGetProductOk()
        {
            Singleton<CAchievementSystem>.GetInstance().ProcessMostRecentlyDoneAchievements(true);
        }

        private void OnMenuChange(CUIEvent uiEvent)
        {
            CUIListScript srcWidgetScript = uiEvent.m_srcWidgetScript as CUIListScript;
            if (srcWidgetScript == null)
            {
                return;
            }
            int selectedIndex = srcWidgetScript.GetSelectedIndex();
            if ((selectedIndex < 0) || (selectedIndex > 2))
            {
                DebugHelper.Assert(false, "Achievement type form selected menu indx out of range!");
                return;
            }
            this.CurMenu = (enTypeMenu) selectedIndex;
            CAchieveInfo2 masterAchieveInfo = CAchieveInfo2.GetMasterAchieveInfo();
            this.m_CurAchieveSeries = new ListView<CAchieveItem2>();
            switch (this.CurMenu)
            {
                case enTypeMenu.All:
                    this.m_CurAchieveSeries = masterAchieveInfo.GetTrophies(enTrophyState.All);
                    this.m_CurAchieveSeries.Sort(new CAchieveSort());
                    break;

                case enTypeMenu.Not_Finish:
                    this.m_CurAchieveSeries = masterAchieveInfo.GetTrophies(enTrophyState.UnFinish);
                    this.m_CurAchieveSeries.Sort(new CAchieveSort());
                    goto Label_00AF;
            }
        Label_00AF:
            this.RefreshOverviewForm(uiEvent.m_srcFormScript);
        }

        private void OnNobeLevelUpFormClose(CUIEvent uiEvent)
        {
            Singleton<CAchievementSystem>.GetInstance().ProcessMostRecentlyDoneAchievements(true);
        }

        [MessageHandler(0x1133)]
        public static void OnNotifyAchieveDoneDataChange(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            SCPKG_ACHIEVEMENT_DONE_DATA_CHG_NTF stAchievementDoneDataChgNtf = msg.stPkgData.stAchievementDoneDataChgNtf;
            CAchieveInfo2.GetMasterAchieveInfo().OnAchieveDoneDataChange(stAchievementDoneDataChgNtf.stAchievementDoneData);
        }

        [MessageHandler(0x1132)]
        public static void OnNotifyAchieveStateChange(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            SCPKG_ACHIEVEMENT_STATE_CHG_NTF stAchievementStateChgNtf = msg.stPkgData.stAchievementStateChgNtf;
            CAchieveInfo2.GetMasterAchieveInfo().ChangeAchieveState(ref stAchievementStateChgNtf.stAchievementData);
            Singleton<CAchievementSystem>.GetInstance().ProcessMostRecentlyDoneAchievements(false);
            Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.ACHIEVE_TROPHY_REWARD_INFO_STATE_CHANGE);
        }

        [MessageHandler(0x157f)]
        public static void OnNotifyPvpBanMsg(CSPkg msg)
        {
            AddPunishMsg(msg.stPkgData.stPvPBanNtf);
        }

        [MessageHandler(0x113b)]
        public static void OnReceiveGetTrophyRewardRsp(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            SCPKG_GET_TROPHYLVL_REWARD_RSP stGetTrophyLvlmentRewardRsp = msg.stPkgData.stGetTrophyLvlmentRewardRsp;
            if (stGetTrophyLvlmentRewardRsp.iResult < 0)
            {
                Singleton<CUIManager>.GetInstance().OpenTips(string.Format("奖励领取失败，请稍后再试 {0}", stGetTrophyLvlmentRewardRsp.iResult), false, 1.5f, null, new object[0]);
            }
            else
            {
                CAchieveInfo2 masterAchieveInfo = CAchieveInfo2.GetMasterAchieveInfo();
                int index = ((int) stGetTrophyLvlmentRewardRsp.dwTrophyLvl) - 1;
                if ((index >= 0) && (index <= masterAchieveInfo.TrophyRewardInfoArrCnt))
                {
                    CTrophyRewardInfo info2 = masterAchieveInfo.TrophyRewardInfoArr[index];
                    CUseable[] trophyRewards = info2.GetTrophyRewards();
                    Singleton<CUIManager>.instance.OpenAwardTip(trophyRewards, null, false, enUIEventID.None, false, false, "Form_Award");
                    info2.State = TrophyState.GotRewards;
                    Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.ACHIEVE_TROPHY_REWARD_INFO_STATE_CHANGE);
                }
            }
        }

        [MessageHandler(0x1131)]
        public static void OnRecieveAchieveInfo(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            SCPKG_ACHIEVEMENT_INFO_NTF stGetAchievememtInfoNtf = msg.stPkgData.stGetAchievememtInfoNtf;
            CAchieveInfo2.GetMasterAchieveInfo().OnServerAchieveInfo(ref stGetAchievememtInfoNtf.stAchievementInfo);
        }

        private void OnShowRule(CUIEvent uiEvent)
        {
            ResRuleText dataByKey = null;
            ushort num = 0x10;
            dataByKey = GameDataMgr.s_ruleTextDatabin.GetDataByKey((uint) num);
            if (dataByKey != null)
            {
                string title = StringHelper.UTF8BytesToString(ref dataByKey.szTitle);
                string info = StringHelper.UTF8BytesToString(ref dataByKey.szContent);
                Singleton<CUIManager>.GetInstance().OpenInfoForm(title, info);
            }
        }

        private void OnTrophyClick(CUIEvent uiEvent)
        {
            CAchieveInfo2 masterAchieveInfo = CAchieveInfo2.GetMasterAchieveInfo();
            uint achievementID = uiEvent.m_eventParams.commonUInt32Param1;
            this.ShowTrophyDetail(masterAchieveInfo, achievementID);
        }

        private void OnTrophyEnable(CUIEvent uiEvent)
        {
            int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
            if ((srcWidgetIndexInBelongedList >= 0) && (srcWidgetIndexInBelongedList < this.m_CurAchieveSeries.Count))
            {
                CUIListElementScript srcWidgetScript = uiEvent.m_srcWidgetScript as CUIListElementScript;
                if (srcWidgetScript == null)
                {
                    DebugHelper.Assert(false, "achievement sery enable elementscript is null");
                }
                else
                {
                    CAchieveItem2 achieveItem = this.m_CurAchieveSeries[srcWidgetIndexInBelongedList];
                    CUIEventScript component = srcWidgetScript.GetComponent<CUIEventScript>();
                    stUIEventParams eventParams = new stUIEventParams();
                    eventParams.commonUInt32Param1 = achieveItem.Cfg.dwID;
                    component.SetUIEvent(enUIEventType.Click, enUIEventID.Achievement_Trophy_Click, eventParams);
                    GameObject widget = srcWidgetScript.GetWidget(0);
                    GameObject obj3 = srcWidgetScript.GetWidget(1);
                    GameObject obj4 = srcWidgetScript.GetWidget(2);
                    GameObject obj5 = srcWidgetScript.GetWidget(3);
                    GameObject obj6 = srcWidgetScript.GetWidget(4);
                    GameObject obj7 = srcWidgetScript.GetWidget(5);
                    obj3.CustomSetActive(false);
                    Image image = widget.GetComponent<Image>();
                    Image image2 = obj6.GetComponent<Image>();
                    Text text = obj4.GetComponent<Text>();
                    Text text2 = obj5.GetComponent<Text>();
                    if (((image != null) && (image2 != null)) && ((text != null) && (text2 != null)))
                    {
                        CAchieveItem2 item2 = achieveItem.TryToGetMostRecentlyDoneItem();
                        if (item2 == null)
                        {
                            image.SetSprite(CUIUtility.GetSpritePrefeb(achieveItem.GetAchieveImagePath(), false, false), false);
                            SetAchieveBaseIcon(obj6.get_transform(), achieveItem, null);
                            text.set_text(achieveItem.Cfg.szName);
                            text2.set_text(achieveItem.GetGotTimeText(false, false));
                            obj7.CustomSetActive(true);
                        }
                        else
                        {
                            image.SetSprite(CUIUtility.GetSpritePrefeb(item2.GetAchieveImagePath(), false, false), false);
                            SetAchieveBaseIcon(obj6.get_transform(), item2, null);
                            text.set_text(item2.Cfg.szName);
                            if (achieveItem == item2)
                            {
                                text2.set_text(achieveItem.GetGotTimeText(false, false));
                            }
                            else
                            {
                                text2.set_text(item2.GetGotTimeText(false, true));
                            }
                            obj7.CustomSetActive(false);
                        }
                    }
                }
            }
        }

        [MessageHandler(0x113c)]
        public static void OnTrophyLevelUp(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            SCPKG_NTF_TROPHYLVLUP stNtfTrophyLvlUp = msg.stPkgData.stNtfTrophyLvlUp;
            CAchieveInfo2.GetMasterAchieveInfo().TrophyLevelUp(stNtfTrophyLvlUp.dwOldTrophyLvl, stNtfTrophyLvlUp.dwNewTrophyLvl);
            Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.ACHIEVE_TROPHY_REWARD_INFO_STATE_CHANGE);
        }

        private void OnTrophyRewardInfoEnable(CUIEvent uiEvent)
        {
            int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
            int num2 = (this.m_TrophyRewardInfoWithRewardList != null) ? this.m_TrophyRewardInfoWithRewardList.Count : 0;
            if ((srcWidgetIndexInBelongedList >= 0) && (srcWidgetIndexInBelongedList < num2))
            {
                CTrophyRewardInfo info = this.m_TrophyRewardInfoWithRewardList[srcWidgetIndexInBelongedList];
                CUIListElementScript srcWidgetScript = uiEvent.m_srcWidgetScript as CUIListElementScript;
                if (srcWidgetScript == null)
                {
                    DebugHelper.Assert(false, "achievement reward enable elementscript is null");
                }
                else
                {
                    GameObject widget = srcWidgetScript.GetWidget(0);
                    GameObject obj3 = srcWidgetScript.GetWidget(1);
                    GameObject obj4 = srcWidgetScript.GetWidget(2);
                    GameObject obj5 = srcWidgetScript.GetWidget(3);
                    GameObject obj6 = srcWidgetScript.GetWidget(4);
                    GameObject obj7 = srcWidgetScript.GetWidget(5);
                    GameObject obj8 = srcWidgetScript.GetWidget(6);
                    GameObject obj9 = srcWidgetScript.GetWidget(7);
                    GameObject obj10 = srcWidgetScript.GetWidget(8);
                    GameObject obj11 = srcWidgetScript.GetWidget(9);
                    GameObject obj12 = srcWidgetScript.GetWidget(10);
                    if (info.HasGotAward())
                    {
                        widget.CustomSetActive(false);
                        obj3.CustomSetActive(true);
                        obj6.CustomSetActive(false);
                        obj9.CustomSetActive(false);
                        obj7.CustomSetActive(true);
                        obj7.GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("Achievement_Trophy_Reward_Status_Got"));
                        obj10.CustomSetActive(false);
                    }
                    else if (info.IsFinish())
                    {
                        uint num3 = 0;
                        for (int i = 0; i < 3; i++)
                        {
                            if (info.Cfg.astReqReward[i].dwRewardNum != 0)
                            {
                                num3++;
                            }
                        }
                        if (num3 == 0)
                        {
                            widget.CustomSetActive(false);
                            obj3.CustomSetActive(true);
                            obj6.CustomSetActive(false);
                            obj9.CustomSetActive(false);
                            obj7.CustomSetActive(true);
                            obj7.GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("Achievement_Trophy_Reward_Status_Done"));
                        }
                        else
                        {
                            widget.CustomSetActive(true);
                            obj3.CustomSetActive(false);
                            obj6.CustomSetActive(false);
                            obj9.CustomSetActive(true);
                            obj7.CustomSetActive(false);
                            CUIEventScript script2 = obj9.GetComponent<CUIEventScript>();
                            if (script2 != null)
                            {
                                stUIEventParams eventParams = new stUIEventParams();
                                eventParams.tag = info.Index;
                                script2.SetUIEvent(enUIEventType.Click, enUIEventID.Achievement_Get_Trophy_Reward, eventParams);
                            }
                        }
                        obj10.CustomSetActive(false);
                    }
                    else
                    {
                        widget.CustomSetActive(true);
                        obj3.CustomSetActive(false);
                        obj6.CustomSetActive(false);
                        obj9.CustomSetActive(false);
                        obj7.CustomSetActive(true);
                        Text text3 = obj7.GetComponent<Text>();
                        if (text3 != null)
                        {
                            text3.set_text((info.State != TrophyState.OnGoing) ? Singleton<CTextManager>.GetInstance().GetText("Achievement_Trophy_Reward_Status_Not_Done") : Singleton<CTextManager>.GetInstance().GetText("Achievement_Trophy_Reward_Status_OnGoing"));
                        }
                        obj10.CustomSetActive(false);
                    }
                    obj4.GetComponent<Image>().SetSprite(info.GetTrophyImagePath(), srcWidgetScript.m_belongedFormScript, true, false, false, false);
                    Text component = obj5.GetComponent<Text>();
                    if (component != null)
                    {
                        component.set_text(info.Cfg.szTrophyDesc);
                    }
                    CUIListScript script3 = obj8.GetComponent<CUIListScript>();
                    if (script3 != null)
                    {
                        CUseable[] trophyRewards = info.GetTrophyRewards();
                        script3.SetElementAmount(trophyRewards.Length);
                        for (int j = 0; j < trophyRewards.Length; j++)
                        {
                            GameObject itemCell = script3.GetElemenet(j).GetWidget(0);
                            if (itemCell != null)
                            {
                                CUseable itemUseable = trophyRewards[j];
                                if (itemUseable == null)
                                {
                                    script3.SetElementAmount(0);
                                    return;
                                }
                                if (trophyRewards.Length >= 5)
                                {
                                    CUICommonSystem.SetItemCell(script3.m_belongedFormScript, itemCell, itemUseable, false, false, false, false);
                                }
                                else
                                {
                                    CUICommonSystem.SetItemCell(script3.m_belongedFormScript, itemCell, itemUseable, true, false, false, false);
                                }
                                if (itemUseable.m_stackCount == 1)
                                {
                                    Utility.FindChild(itemCell, "cntBg").CustomSetActive(false);
                                    Utility.FindChild(itemCell, "lblIconCount").CustomSetActive(false);
                                }
                                else
                                {
                                    Utility.FindChild(itemCell, "cntBg").CustomSetActive(true);
                                    Utility.FindChild(itemCell, "lblIconCount").CustomSetActive(true);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void OnTrophyStateChange()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Achieve/Form_Trophy_Overview.prefab");
            if (Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Achieve/Form_Trophy_Rewards.prefab") != null)
            {
                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Achievement_Browse_All_Rewards);
            }
            if (form != null)
            {
                this.RefreshOverviewForm(form);
            }
        }

        public void ProcessMostRecentlyDoneAchievements(bool force = false)
        {
            if (this.m_shareComponent != null)
            {
                this.m_shareComponent.Process(force);
            }
        }

        public void ProcessPvpBanMsg()
        {
            if (((!Singleton<BattleLogic>.GetInstance().isRuning && !Singleton<CMatchingSystem>.GetInstance().IsInMatching) && (!Singleton<CMatchingSystem>.GetInstance().IsInMatchingTeam && (Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_CONFIRMBOX) == null))) && ((CachedPunishMsg != null) && (CachedPunishMsg.Count > 0)))
            {
                for (int i = CachedPunishMsg.Count - 1; i >= 0; i--)
                {
                    string strContent = string.Format(Singleton<CTextManager>.GetInstance().GetText("Achievement_10"), Utility.GetUtcToLocalTimeStringFormat((ulong) CachedPunishMsg[i].dwPunishEndTime, "yyyy年M月d日 H:m:s"));
                    Singleton<CUIManager>.GetInstance().OpenMessageBox(strContent, false);
                    CachedPunishMsg.RemoveAt(i);
                }
            }
        }

        private void RankingListChange(RankingSystem.RankingType rankType)
        {
            if (rankType == RankingSystem.RankingType.Achievement)
            {
                CSDT_RANKING_LIST_SUCC rankList = Singleton<RankingSystem>.GetInstance().GetRankList(RankingSystem.RankingType.Achievement);
                if (rankList != null)
                {
                    for (int i = 0; i < rankList.dwItemNum; i++)
                    {
                        CAchieveInfo2.AddWorldRank(rankList.astItemDetail[i].stExtraInfo.stDetailInfo.stAchievement.iLogicWorldId, rankList.astItemDetail[i].stExtraInfo.stDetailInfo.stAchievement.ullUid, (uint) (rankList.iStart + i));
                    }
                }
            }
        }

        private void RefreshOverviewForm(CUIFormScript overviewForm = new CUIFormScript())
        {
            if (overviewForm == null)
            {
                overviewForm = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Achieve/Form_Trophy_Overview.prefab");
            }
            if (overviewForm != null)
            {
                CUIListScript component = overviewForm.GetWidget(1).GetComponent<CUIListScript>();
                if (component != null)
                {
                    component.SetElementAmount(this.m_CurAchieveSeries.Count);
                }
                GameObject widget = overviewForm.GetWidget(2);
                GameObject obj3 = overviewForm.GetWidget(3);
                GameObject obj4 = overviewForm.GetWidget(7);
                GameObject obj5 = overviewForm.GetWidget(8);
                GameObject obj6 = overviewForm.GetWidget(9);
                GameObject obj7 = overviewForm.GetWidget(10);
                GameObject obj8 = overviewForm.GetWidget(4);
                GameObject obj9 = overviewForm.GetWidget(5);
                GameObject obj10 = overviewForm.GetWidget(6);
                if ((((widget == null) || (obj3 == null)) || ((obj4 == null) || (obj5 == null))) || (((obj8 == null) || (obj9 == null)) || (((obj10 == null) || (obj6 == null)) || (obj7 == null))))
                {
                    DebugHelper.Assert(false, "Some of Trophy overview form widgets is null");
                }
                else
                {
                    Text text = widget.GetComponent<Text>();
                    Text text2 = obj3.GetComponent<Text>();
                    Image image = obj7.GetComponent<Image>();
                    Image image2 = obj4.GetComponent<Image>();
                    Text text3 = obj5.GetComponent<Text>();
                    CAchieveInfo2 masterAchieveInfo = CAchieveInfo2.GetMasterAchieveInfo();
                    if (masterAchieveInfo.LastDoneTrophyRewardInfo != null)
                    {
                        image.SetSprite(masterAchieveInfo.LastDoneTrophyRewardInfo.GetTrophyImagePath(), overviewForm, true, false, false, false);
                    }
                    text.set_text((masterAchieveInfo.LastDoneTrophyRewardInfo != null) ? string.Format("{0}", masterAchieveInfo.LastDoneTrophyRewardInfo.Cfg.dwTrophyLvl) : "0");
                    if (masterAchieveInfo.GetWorldRank() == 0)
                    {
                        obj6.CustomSetActive(true);
                        obj3.CustomSetActive(false);
                    }
                    else
                    {
                        obj6.CustomSetActive(false);
                        obj3.CustomSetActive(true);
                        text2.set_text(masterAchieveInfo.GetWorldRank().ToString());
                    }
                    uint cur = 0;
                    uint next = 0;
                    masterAchieveInfo.GetTrophyProgress(ref cur, ref next);
                    CTrophyRewardInfo trophyRewardInfoByPoint = masterAchieveInfo.GetTrophyRewardInfoByPoint(cur);
                    CTrophyRewardInfo trophyRewardInfoByIndex = masterAchieveInfo.GetTrophyRewardInfoByIndex(trophyRewardInfoByPoint.Index + 1);
                    image2.set_fillAmount(Utility.Divide(cur - trophyRewardInfoByIndex.MinPoint, trophyRewardInfoByIndex.MaxPoint - trophyRewardInfoByIndex.MinPoint));
                    text3.set_text(string.Format("{0}/{1}", cur - trophyRewardInfoByIndex.MinPoint, trophyRewardInfoByIndex.MaxPoint - trophyRewardInfoByIndex.MinPoint));
                    Text text4 = obj8.GetComponent<Text>();
                    CUIListScript script2 = obj9.GetComponent<CUIListScript>();
                    CUIEventScript script3 = obj10.GetComponent<CUIEventScript>();
                    CTrophyRewardInfo firstTrophyRewardInfoAwardNotGot = masterAchieveInfo.GetFirstTrophyRewardInfoAwardNotGot();
                    if (firstTrophyRewardInfoAwardNotGot == null)
                    {
                        obj8.CustomSetActive(false);
                        obj10.CustomSetActive(false);
                        script2.SetElementAmount(0);
                    }
                    else
                    {
                        bool flag = false;
                        CUseable[] trophyRewards = firstTrophyRewardInfoAwardNotGot.GetTrophyRewards();
                        if (!firstTrophyRewardInfoAwardNotGot.HasGotAward() && firstTrophyRewardInfoAwardNotGot.IsFinish())
                        {
                            flag = true;
                        }
                        obj8.CustomSetActive(true);
                        text4.set_text(string.Format("{0}级奖励：", firstTrophyRewardInfoAwardNotGot.Cfg.dwTrophyLvl));
                        script2.SetElementAmount(trophyRewards.Length);
                        for (int i = 0; i < trophyRewards.Length; i++)
                        {
                            CUIListElementScript elemenet = script2.GetElemenet(i);
                            CUICommonSystem.SetItemCell(overviewForm, elemenet.GetWidget(0), trophyRewards[i], false, false, false, false);
                        }
                        obj10.CustomSetActive(true);
                        if (flag)
                        {
                            CUICommonSystem.SetButtonEnable(obj10.GetComponent<Button>(), true, true, true);
                            stUIEventParams eventParams = new stUIEventParams();
                            eventParams.tag = firstTrophyRewardInfoAwardNotGot.Index;
                            script3.SetUIEvent(enUIEventType.Click, enUIEventID.Achievement_Get_Trophy_Reward, eventParams);
                        }
                        else
                        {
                            CUICommonSystem.SetButtonEnable(obj10.GetComponent<Button>(), false, false, true);
                        }
                    }
                }
            }
        }

        public static void SendGetAchieveRewardReq(uint achieveId)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x1134);
            msg.stPkgData.stGetAchievementRewardReq = new CSPKG_GET_ACHIEVEMENT_REWARD_REQ();
            msg.stPkgData.stGetAchievementRewardReq.dwAchievementID = achieveId;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public void SendGetTrophyRewardReq(uint id)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x113a);
            msg.stPkgData.stGetTrophyLvlRewardReq.dwTrophyLvl = id;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public void SendReqGetRankingAcountInfo()
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0xa2c);
            msg.stPkgData.stGetRankingAcntInfoReq.bNumberType = 8;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        public static void SetAchieveBaseIcon(Transform trans, CAchieveItem2 achieveItem, CUIFormScript form = new CUIFormScript())
        {
            if ((achieveItem != null) && (trans != null))
            {
                trans.GetComponent<Image>().SetSprite(achieveItem.GetAchievementBgIconPath(), form, true, false, false, false);
                Transform transform = trans.Find("goldEffect");
                Transform transform2 = trans.Find("silverEffect");
                if (!achieveItem.IsFinish())
                {
                    transform.get_gameObject().CustomSetActive(false);
                    transform2.get_gameObject().CustomSetActive(false);
                }
                else
                {
                    uint dwAchieveLvl = achieveItem.Cfg.dwAchieveLvl;
                    if (dwAchieveLvl >= 3)
                    {
                        transform.get_gameObject().CustomSetActive(true);
                        transform2.get_gameObject().CustomSetActive(false);
                    }
                    else if (dwAchieveLvl == 2)
                    {
                        transform.get_gameObject().CustomSetActive(false);
                        transform2.get_gameObject().CustomSetActive(true);
                    }
                    else
                    {
                        transform.get_gameObject().CustomSetActive(false);
                        transform2.get_gameObject().CustomSetActive(false);
                    }
                }
            }
        }

        public void ShowTrophyDetail(CAchieveInfo2 achieveInfo, uint achievementID)
        {
            CAchieveItem2 item = null;
            if (achieveInfo.m_AchiveItemDic.ContainsKey(achievementID))
            {
                item = achieveInfo.m_AchiveItemDic[achievementID];
            }
            if (item != null)
            {
                this.m_CurAchieveItem = item.GetHead();
                int achievementCnt = this.m_CurAchieveItem.GetAchievementCnt();
                CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/System/Achieve/Form_Trophy_Detail.prefab", false, true);
                if (script != null)
                {
                    Text component = script.GetWidget(1).GetComponent<Text>();
                    if (component != null)
                    {
                        component.set_text(this.m_CurAchieveItem.Cfg.szName);
                    }
                    Text text2 = script.GetWidget(2).GetComponent<Text>();
                    if (text2 != null)
                    {
                        text2.set_text(this.m_CurAchieveItem.GetAchievementTips());
                    }
                    CUIListScript script2 = script.GetWidget(0).GetComponent<CUIListScript>();
                    if (script2 != null)
                    {
                        script2.SetElementAmount(achievementCnt);
                    }
                }
            }
        }

        public override void UnInit()
        {
            base.UnInit();
            this.m_CurAchieveItem = null;
            CachedPunishMsg = null;
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Achievement_Open_Overview_Form, new CUIEventManager.OnUIEventHandler(this.OnAchievementOpenOverviewForm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Achievement_ShowShareBtn, new CUIEventManager.OnUIEventHandler(this.OnAchievementShowShareBtn));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Achievement_Filter_Menu_Change, new CUIEventManager.OnUIEventHandler(this.OnMenuChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Achievement_Trophy_Enable, new CUIEventManager.OnUIEventHandler(this.OnTrophyEnable));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Achievement_Trophy_Click, new CUIEventManager.OnUIEventHandler(this.OnTrophyClick));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Achievement_Item_Enable, new CUIEventManager.OnUIEventHandler(this.OnAchievementEnable));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Achievement_Browse_All_Rewards, new CUIEventManager.OnUIEventHandler(this.OnBrowseAllRewards));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Achievement_Trophy_Reward_Info_Enable, new CUIEventManager.OnUIEventHandler(this.OnTrophyRewardInfoEnable));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Achievement_Get_Trophy_Reward, new CUIEventManager.OnUIEventHandler(this.OnGetTrophyReward));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Achievement_Show_Rule, new CUIEventManager.OnUIEventHandler(this.OnShowRule));
            Singleton<EventRouter>.instance.RemoveEventHandler<SCPKG_GET_RANKING_ACNT_INFO_RSP>(EventID.ACHIEVE_GET_RANKING_ACCOUNT_INFO, new Action<SCPKG_GET_RANKING_ACNT_INFO_RSP>(this.HandleAchieveGetRankingAccountInfo));
            Singleton<EventRouter>.instance.RemoveEventHandler(EventID.LOBBY_STATE_ENTER, new Action(this, (IntPtr) this.HandleLobbyStateEnter));
            Singleton<EventRouter>.instance.RemoveEventHandler(EventID.ACHIEVE_TROPHY_REWARD_INFO_STATE_CHANGE, new Action(this, (IntPtr) this.OnTrophyStateChange));
            Singleton<EventRouter>.instance.RemoveEventHandler<RankingSystem.RankingType>("Ranking_List_Change", new Action<RankingSystem.RankingType>(this.RankingListChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.NOBE_LevelUp_Form_Close, new CUIEventManager.OnUIEventHandler(this.OnNobeLevelUpFormClose));
            Singleton<EventRouter>.instance.RemoveEventHandler(EventID.Mall_Get_Product_OK, new Action(this, (IntPtr) this.OnMallGetProductOk));
            Singleton<EventRouter>.instance.RemoveEventHandler(EventID.ACHIEVE_RECEIVE_PVP_BAN_MSG, new Action(this, (IntPtr) this.ProcessPvpBanMsg));
            this.m_shareComponent = null;
        }

        public enTypeMenu CurMenu
        {
            get
            {
                return this.m_CurMenu;
            }
            set
            {
                this.m_CurMenu = value;
            }
        }

        public enum enOverviewFormWidget
        {
            Menu,
            SeryList,
            TrophyLevel,
            TrophyRank,
            NotGotRewardLevelText,
            NotGotRewards,
            GetRewardBtn,
            TrophyProgressImg,
            TrophyProgressTxt,
            NotInRank,
            Icon
        }

        public enum enTypeMenu
        {
            All,
            Not_Finish,
            Type_Max
        }
    }
}

