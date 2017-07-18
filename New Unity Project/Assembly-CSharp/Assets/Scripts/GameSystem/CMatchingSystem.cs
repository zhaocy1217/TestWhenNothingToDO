namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
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
    internal class CMatchingSystem : Singleton<CMatchingSystem>
    {
        [CompilerGenerated]
        private int <currentMapPlayerNum>k__BackingField;
        private bool bInMatching;
        private bool bInMatchingTeam;
        public CacheMathingInfo cacheMathingInfo = new CacheMathingInfo();
        public int confirmPlayerNum;
        private int m_lastReqMathTime;
        private uint mapId;
        private COM_BATTLE_MAP_TYPE mapType;
        private byte maxTeamNum;
        public static string PATH_MATCHING_CONFIRMBOX = "UGUI/Form/System/PvP/Matching/Form_MatchingConfirmBox.prefab";
        public static string PATH_MATCHING_ENTRY = "UGUI/Form/System/PvP/Form_PvPEntry.prefab";
        public static string PATH_MATCHING_INMATCHING = "UGUI/Form/System/PvP/Matching/Form_InMatching.prefab";
        public static string PATH_MATCHING_MULTI = "UGUI/Form/System/PvP/Matching/Form_MultiMatching.prefab";
        public static string PATH_MATCHING_WAITING = "UGUI/Form/System/PvP/Matching/Form_MatchWaiting.prefab";
        public static readonly string[] s_excludeForm = new string[] { CPlayerInfoSystem.sPlayerInfoFormPath, HeadIconSys.s_headImgChgForm, "UGUI/Form/Common/Form_NameChange.prefab", CPlayerCommonHeroInfoController.sPlayerInfoCommonHeroFormPath };
        public static int s_PVP_RULE_ID = 6;
        public static int s_TRAIN_RULE_ID = 7;
        public TeamInfo teamInfo = new TeamInfo();

        private bool CanReqMatch()
        {
            return ((CRoleInfo.GetCurrentUTCTime() - this.m_lastReqMathTime) > 1);
        }

        public void Clear()
        {
            this.bInMatchingTeam = false;
            this.bInMatching = false;
        }

        public static void CloseExcludeForm()
        {
            for (int i = 0; i < s_excludeForm.Length; i++)
            {
                Singleton<CUIManager>.instance.CloseForm(s_excludeForm[i]);
            }
        }

        private static void CloseInMatchingForm()
        {
            Singleton<CMatchingSystem>.GetInstance().bInMatching = false;
            Singleton<CMatchingSystem>.GetInstance().bInMatchingTeam = false;
            Singleton<CUIManager>.GetInstance().CloseForm(PATH_MATCHING_INMATCHING);
        }

        public void CloseMatchingConfirm()
        {
            Singleton<CUIManager>.GetInstance().CloseForm(PATH_MATCHING_CONFIRMBOX);
        }

        public TeamMember CreateTeamMemberInfo(COMDT_TEAMMEMBER_INFO info)
        {
            TeamMember member = new TeamMember();
            member.uID.ullUid = info.stMemberDetail.stMemberUniq.ullUid;
            member.uID.iGameEntity = info.stMemberDetail.stMemberUniq.iGameEntity;
            member.uID.iLogicWorldId = info.stMemberDetail.iMemberLogicWorldId;
            member.MemberName = StringHelper.UTF8BytesToString(ref info.stMemberDetail.szMemberName);
            member.dwMemberHeadId = info.stMemberDetail.dwMemberHeadId;
            member.dwMemberLevel = info.stMemberDetail.dwMemberLevel;
            member.dwPosOfTeam = info.dwPosOfTeam;
            member.bGradeOfRank = info.stMemberDetail.bGradeOfRank;
            member.snsHeadUrl = Utility.UTF8Convert(info.stMemberDetail.szMemberHeadUrl);
            return member;
        }

        public void EndGame()
        {
            this.teamInfo = new TeamInfo();
        }

        public void EntertainMentAddLock(GameObject btnObj)
        {
            if (btnObj != null)
            {
                Transform transform = btnObj.get_transform();
                if (!Singleton<CFunctionUnlockSys>.instance.FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_ENTERTAINMENT))
                {
                    transform.GetComponent<Button>().set_interactable(false);
                    transform.FindChild("Lock").get_gameObject().CustomSetActive(true);
                    ResSpecialFucUnlock dataByKey = GameDataMgr.specialFunUnlockDatabin.GetDataByKey((uint) 0x19);
                    transform.FindChild("Lock/Text").GetComponent<Text>().set_text(Utility.UTF8Convert(dataByKey.szLockedTip));
                }
                else
                {
                    transform.GetComponent<Button>().set_interactable(true);
                    transform.FindChild("Lock").get_gameObject().CustomSetActive(false);
                }
            }
        }

        public static uint Get1v1MapId()
        {
            return GetMapIDInner(2);
        }

        public static uint Get2v2MapId()
        {
            return GetMapIDInner(4);
        }

        public static uint Get3v3MapId()
        {
            return GetMapIDInner(6);
        }

        public static uint Get5v5MapId()
        {
            return GetMapIDInner(10);
        }

        public static uint GetCPMap3v3Id()
        {
            uint dwMapId = 0;
            Dictionary<long, object>.Enumerator enumerator = GameDataMgr.cpLevelDatabin.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<long, object> current = enumerator.Current;
                ResCounterPartLevelInfo info = (ResCounterPartLevelInfo) current.Value;
                if ((info.stLevelCommonInfo.bMaxAcntNum == 6) && (info.bIsSingle == 1))
                {
                    dwMapId = info.dwMapId;
                    break;
                }
            }
            DebugHelper.Assert(dwMapId > 0);
            return dwMapId;
        }

        private static uint GetMapIDInner(byte MaxAcntNum)
        {
            return 0;
        }

        public static bool HasBpGradeMember(TeamInfo data)
        {
            if (data == null)
            {
                return false;
            }
            bool flag = false;
            CLadderSystem instance = Singleton<CLadderSystem>.GetInstance();
            for (int i = 0; i < 5; i++)
            {
                TeamMember memberInfo = TeamInfo.GetMemberInfo(data, i);
                if ((memberInfo != null) && instance.IsUseBpMode(memberInfo.bGradeOfRank))
                {
                    flag = true;
                }
            }
            return flag;
        }

        private void HideHeroChooseGrp(CUIFormScript form)
        {
            GameObject widget = form.GetWidget(13);
            GameObject obj3 = form.GetWidget(14);
            widget.CustomSetActive(false);
            obj3.CustomSetActive(false);
        }

        private void HideRobotBtnGroup()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(PATH_MATCHING_ENTRY);
            if (form != null)
            {
                form.GetWidget(5).CustomSetActive(false);
                form.GetWidget(6).CustomSetActive(false);
                form.GetWidget(7).CustomSetActive(false);
                form.GetWidget(8).CustomSetActive(false);
                form.GetWidget(12).CustomSetActive(false);
            }
        }

        public override void Init()
        {
            base.Init();
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_OpenEntry, new CUIEventManager.OnUIEventHandler(this.OnMatchingRoom_OpenEntry));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_StartMulti, new CUIEventManager.OnUIEventHandler(this.OnMatching_StartMulti));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_LeaveTeam, new CUIEventManager.OnUIEventHandler(this.OnMatching_LeaveTeam));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_ReqLeave, new CUIEventManager.OnUIEventHandler(this.OnMatching_ReqLeave));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_KickPlayer, new CUIEventManager.OnUIEventHandler(this.OnMatching_KickPlayer));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_ConfirmMatch, new CUIEventManager.OnUIEventHandler(this.OnMatching_ConfirmGame));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_OpenConfirmBox, new CUIEventManager.OnUIEventHandler(this.OnMatching_OpenConfirmBox));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_Waiting, new CUIEventManager.OnUIEventHandler(this.onMatchWatingTimeUp));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_RuleView, new CUIEventManager.OnUIEventHandler(this.OnMatching_RuleView));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_ClickFogHelp, new CUIEventManager.OnUIEventHandler(this.OnClickFogHelp));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_BtnGroup_Click, new CUIEventManager.OnUIEventHandler(this.OnBtnGroupClick));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_BtnGroup_ClickClose, new CUIEventManager.OnUIEventHandler(this.OnBtnGroupClose));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_Robot_BtnGroup_Click, new CUIEventManager.OnUIEventHandler(this.OnRobotBtnGroupClick));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_Begin1v1, new CUIEventManager.OnUIEventHandler(this.OnMatching_Begin1v1));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_Begin3v3Team, new CUIEventManager.OnUIEventHandler(this.OnMatching_Begin3v3Multi));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_Begin5v5Team, new CUIEventManager.OnUIEventHandler(this.OnMatching_Begin5v5Multi));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.MatchingExt_BeginMelee, new CUIEventManager.OnUIEventHandler(this.OnMatching_BeginMeleeMulti));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.MatchingExt_BeginEnterTrainMent, new CUIEventManager.OnUIEventHandler(this.OnMatching_BeginEnterTainment));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_EnterTainMentMore, new CUIEventManager.OnUIEventHandler(this.onMatching_ClickEnterTrainMentMore));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_Begin5v5LadderIn2, new CUIEventManager.OnUIEventHandler(this.OnMatching_Begin5v5LadderIn2));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_Begin5v5LadderIn5, new CUIEventManager.OnUIEventHandler(this.OnMatching_Begin5v5LadderIn5));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_Robot1V1, new CUIEventManager.OnUIEventHandler(this.OnMatching_Robot1V1));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_RobotTeamVERSUS, new CUIEventManager.OnUIEventHandler(this.OnMatching_RobotTeamVERSUS));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_RobotTeamENTERTAINMENT, new CUIEventManager.OnUIEventHandler(this.OnMatching_RobotTeamENTERTAINMENT));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_Guide_1v1, new CUIEventManager.OnUIEventHandler(this.OnMatchingGuide1v1));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_Guide_3v3, new CUIEventManager.OnUIEventHandler(this.OnMatchingGuide3v3));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_Guide_5v5, new CUIEventManager.OnUIEventHandler(this.OnMatchingGuide5v5));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_Guide_Casting, new CUIEventManager.OnUIEventHandler(this.OnMatchingGuideCasting));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_Guide_Jungle, new CUIEventManager.OnUIEventHandler(this.OnMatchingGuideJungle));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_Training, new CUIEventManager.OnUIEventHandler(this.OnMatchingTraining));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_GuidePanel, new CUIEventManager.OnUIEventHandler(this.OnMatchingGuidePanel));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_GuideAdvance, new CUIEventManager.OnUIEventHandler(this.OnMatchingGuideAdvance));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_GuideAdvanceConfirm, new CUIEventManager.OnUIEventHandler(this.OpenGuideAdvancePage));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_Guide_1v1_ChooseHeroType, new CUIEventManager.OnUIEventHandler(this.OnMatchingGuide1v1ChooseHero));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Matching_Guide_5v5_ChooseHeroType, new CUIEventManager.OnUIEventHandler(this.OnMatchingGuide5v5ChooseHero));
            Singleton<EventRouter>.GetInstance().AddEventHandler<byte, string>(EventID.INVITE_TEAM_ERRCODE_NTF, new Action<byte, string>(this, (IntPtr) this.OnInviteErrCodeNtf));
        }

        public void InitTeamInfo(COMDT_TEAM_INFO teamData)
        {
            this.teamInfo.TeamId = teamData.dwTeamId;
            this.teamInfo.TeamSeq = teamData.dwTeamSeq;
            this.teamInfo.TeamEntity = teamData.iTeamEntity;
            this.teamInfo.TeamFeature = teamData.ullTeamFeature;
            this.teamInfo.stTeamInfo.bGameMode = teamData.stTeamInfo.bGameMode;
            this.teamInfo.stTeamInfo.bPkAI = teamData.stTeamInfo.bPkAI;
            this.teamInfo.stTeamInfo.bMapType = teamData.stTeamInfo.bMapType;
            this.teamInfo.stTeamInfo.dwMapId = teamData.stTeamInfo.dwMapId;
            this.teamInfo.stTeamInfo.bMaxTeamNum = teamData.stTeamInfo.bMaxTeamNum;
            this.teamInfo.stTeamInfo.iGradofRank = teamData.stTeamInfo.bGradeOfRank;
            this.mapId = teamData.stTeamInfo.dwMapId;
            this.mapType = (COM_BATTLE_MAP_TYPE) teamData.stTeamInfo.bMapType;
            this.maxTeamNum = teamData.stTeamInfo.bMaxTeamNum;
            this.teamInfo.stSelfInfo.ullUid = teamData.stSelfInfo.ullUid;
            this.teamInfo.stSelfInfo.iGameEntity = teamData.stSelfInfo.iGameEntity;
            this.teamInfo.stTeamMaster.ullUid = teamData.stTeamMaster.ullUid;
            this.teamInfo.stTeamMaster.iGameEntity = teamData.stTeamMaster.iGameEntity;
            this.teamInfo.MemInfoList.Clear();
            for (int i = 0; i < teamData.stMemInfo.dwMemNum; i++)
            {
                TeamMember item = this.CreateTeamMemberInfo(teamData.stMemInfo.astMemInfo[i]);
                this.teamInfo.MemInfoList.Add(item);
            }
        }

        private bool IsTraningLevelLocked(CRoleInfo roleInfo, int trainingLevelCompletedBit)
        {
            switch (trainingLevelCompletedBit)
            {
                case 0x53:
                    return !roleInfo.IsGuidedStateSet(0x62);

                case 0x54:
                    return !roleInfo.IsGuidedStateSet(0x55);

                case 0x55:
                    return !roleInfo.IsGuidedStateSet(0x53);

                case 0:
                    return false;

                case 0x62:
                    return !roleInfo.IsGuidedStateSet(0);
            }
            return false;
        }

        private static void MatchPunishmentWaiting(float time, int punishType)
        {
            GameObject obj4;
            CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(PATH_MATCHING_WAITING, false, true);
            if (script == null)
            {
                return;
            }
            GameObject widget = script.GetWidget(0);
            if (widget != null)
            {
                Text component = widget.GetComponent<Text>();
                if (component != null)
                {
                    component.set_text(Singleton<CTextManager>.GetInstance().GetText("FailToEnterQuque"));
                }
            }
            GameObject obj3 = script.GetWidget(1);
            if (obj3 != null)
            {
                Text text2 = obj3.GetComponent<Text>();
                if (text2 != null)
                {
                    switch (punishType)
                    {
                        case 1:
                            text2.set_text(Singleton<CTextManager>.GetInstance().GetText("PunishmentDescribe"));
                            goto Label_010D;

                        case 2:
                            text2.set_text(Singleton<CTextManager>.GetInstance().GetText("HangUpPunishmentDescribe"));
                            goto Label_010D;

                        case 3:
                            text2.set_text(Singleton<CTextManager>.GetInstance().GetText("CreditPunishmentDescribe"));
                            goto Label_010D;
                    }
                    text2.set_text(Singleton<CTextManager>.GetInstance().GetText("PunishmentDescribe"));
                }
            }
        Label_010D:
            obj4 = script.GetWidget(2);
            if (obj4 != null)
            {
                CUITimerScript script2 = obj4.GetComponent<CUITimerScript>();
                if (script2 != null)
                {
                    script2.SetTotalTime(time);
                    script2.StartTimer();
                }
            }
        }

        private void OnBtnGroupClick(CUIEvent uiEvent)
        {
            int tag = uiEvent.m_eventParams.tag;
            COM_CLIENT_PLAY_TYPE type = COM_CLIENT_PLAY_TYPE.COM_CLIENT_PLAY_NULL;
            switch (tag)
            {
                case 1:
                    type = COM_CLIENT_PLAY_TYPE.COM_CLIENT_PLAY_MATCH;
                    break;

                case 2:
                    type = COM_CLIENT_PLAY_TYPE.COM_CLIENT_PLAY_ENTERTAINMENT;
                    break;

                case 3:
                    type = COM_CLIENT_PLAY_TYPE.COM_CLIENT_PLAY_PKAI;
                    break;
            }
            if (!Singleton<SCModuleControl>.instance.GetActiveModule(type))
            {
                Singleton<CUIManager>.instance.OpenMessageBox(Singleton<SCModuleControl>.instance.PvpAndPvpOffTips, false);
            }
            else
            {
                CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
                if (null != srcFormScript)
                {
                    GameObject btnObj = null;
                    switch (tag)
                    {
                        case 1:
                            srcFormScript.GetWidget(3).CustomSetActive(true);
                            btnObj = srcFormScript.get_transform().FindChild("panelGroup2/btnGroup/Button3").get_gameObject();
                            break;

                        case 2:
                            srcFormScript.GetWidget(4).CustomSetActive(true);
                            btnObj = srcFormScript.get_transform().FindChild("panelGroup3/btnGroup/Button3").get_gameObject();
                            break;

                        case 3:
                            srcFormScript.GetWidget(11).CustomSetActive(true);
                            break;
                    }
                    srcFormScript.GetWidget(2).CustomSetActive(false);
                    this.EntertainMentAddLock(btnObj);
                    this.ShowBonusImage(srcFormScript);
                }
            }
        }

        private void OnBtnGroupClose(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(PATH_MATCHING_ENTRY);
            if (form != null)
            {
                form.GetWidget(3).CustomSetActive(false);
                form.GetWidget(4).CustomSetActive(false);
                form.GetWidget(9).CustomSetActive(false);
                form.GetWidget(11).CustomSetActive(false);
                form.GetWidget(10).CustomSetActive(true);
                if (CSysDynamicBlock.bLobbyEntryBlocked)
                {
                    form.GetWidget(10).CustomSetActive(false);
                }
                form.GetWidget(2).CustomSetActive(true);
                this.ShowBonusImage(form);
                this.HideRobotBtnGroup();
                Singleton<CMiShuSystem>.instance.CheckActPlayModeTipsForPvpEntry();
            }
        }

        private void OnClickFogHelp(CUIEvent uiEvent)
        {
            Singleton<CBattleGuideManager>.instance.OpenBannerDlgByBannerGuideId(0x10, null, false);
        }

        private void OnInviteErrCodeNtf(byte errorCode, string userName)
        {
            if (errorCode == 20)
            {
                CloseInMatchingForm();
            }
        }

        [MessageHandler(0x7ee)]
        public static void OnLeaveTeam(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            if (msg.stPkgData.stAcntLeaveRsp.bResult == 0)
            {
                Singleton<CMatchingSystem>.GetInstance().bInMatchingTeam = false;
                Singleton<CUIManager>.GetInstance().CloseForm(PATH_MATCHING_MULTI);
                Singleton<CTopLobbyEntry>.GetInstance().CloseForm();
                Singleton<CInviteSystem>.GetInstance().CloseInviteForm();
                MonoSingleton<ShareSys>.GetInstance().SendQQGameTeamStateChgMsg(ShareSys.QQGameTeamEventType.leave, COM_ROOM_TYPE.COM_ROOM_TYPE_NULL, 0, 0, string.Empty);
            }
            else
            {
                Singleton<CUIManager>.GetInstance().OpenTips(Utility.ProtErrCodeToStr(0x7ee, msg.stPkgData.stAcntLeaveRsp.bResult), false, 1.5f, null, new object[0]);
            }
        }

        private void OnMatching_Begin1v1(CUIEvent uiEvent)
        {
            if (this.CanReqMatch())
            {
                this.ResetMatchTime();
                this.cacheMathingInfo.uiEventId = uiEvent.m_eventID;
                this.cacheMathingInfo.mapId = uiEvent.m_eventParams.tagUInt;
                this.cacheMathingInfo.mapType = 1;
                this.cacheMathingInfo.CanGameAgain = true;
                ReqStartSingleMatching(uiEvent.m_eventParams.tagUInt, false, COM_BATTLE_MAP_TYPE.COM_BATTLE_MAP_TYPE_VERSUS);
            }
        }

        private void OnMatching_Begin3v3Multi(CUIEvent uiEvent)
        {
            if (!MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.onClick33Team, new uint[0]) && this.CanReqMatch())
            {
                this.ResetMatchTime();
                this.cacheMathingInfo.uiEventId = uiEvent.m_eventID;
                this.cacheMathingInfo.mapId = uiEvent.m_eventParams.tagUInt;
                this.cacheMathingInfo.mapType = 1;
                this.cacheMathingInfo.CanGameAgain = true;
                uint tagUInt = uiEvent.m_eventParams.tagUInt;
                ResDT_LevelCommonInfo pvpMapCommonInfo = CLevelCfgLogicManager.GetPvpMapCommonInfo(this.cacheMathingInfo.mapType, tagUInt);
                ReqCreateTeam(tagUInt, false, COM_BATTLE_MAP_TYPE.COM_BATTLE_MAP_TYPE_VERSUS, pvpMapCommonInfo.bMaxAcntNum / 2, COM_AI_LEVEL.COM_AI_LEVEL_MIDDLE_OF_NORMALBATTLE, false);
                Singleton<CNewbieAchieveSys>.GetInstance().trackFlag = CNewbieAchieveSys.TrackFlag.SINGLE_MATCH_3V3_ENTER;
            }
        }

        private void OnMatching_Begin5v5LadderIn2(CUIEvent uiEvent)
        {
            this.cacheMathingInfo.uiEventId = uiEvent.m_eventID;
            this.cacheMathingInfo.mapId = CLadderSystem.GetRankBattleMapID();
            this.cacheMathingInfo.mapType = 3;
            this.cacheMathingInfo.CanGameAgain = true;
            ReqCreateTeam(this.cacheMathingInfo.mapId, false, COM_BATTLE_MAP_TYPE.COM_BATTLE_MAP_TYPE_RANK, 2, COM_AI_LEVEL.COM_AI_LEVEL_MIDDLE_OF_NORMALBATTLE, false);
        }

        private void OnMatching_Begin5v5LadderIn5(CUIEvent uiEvent)
        {
            this.cacheMathingInfo.uiEventId = uiEvent.m_eventID;
            this.cacheMathingInfo.mapId = CLadderSystem.GetRankBattleMapID();
            this.cacheMathingInfo.mapType = 3;
            this.cacheMathingInfo.CanGameAgain = true;
            ReqCreateTeam(this.cacheMathingInfo.mapId, false, COM_BATTLE_MAP_TYPE.COM_BATTLE_MAP_TYPE_RANK, 5, COM_AI_LEVEL.COM_AI_LEVEL_MIDDLE_OF_NORMALBATTLE, false);
        }

        private void OnMatching_Begin5v5Multi(CUIEvent uiEvent)
        {
            if (this.CanReqMatch() && !MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.onClick55Team, new uint[0]))
            {
                uint result = 0;
                uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_PVP_5V5Miwu"), out result);
                if ((uiEvent.m_eventParams.tagUInt == result) && (uiEvent.m_srcFormScript != null))
                {
                    CUIEvent uiEventParam = new CUIEvent();
                    uiEventParam.m_eventID = uiEvent.m_eventID;
                    uiEventParam.m_eventParams.tagUInt = uiEvent.m_eventParams.tagUInt;
                    if (Singleton<CBattleGuideManager>.instance.OpenBannerDlgByBannerGuideId(15, uiEventParam, false))
                    {
                        return;
                    }
                }
                this.ResetMatchTime();
                this.cacheMathingInfo.uiEventId = uiEvent.m_eventID;
                this.cacheMathingInfo.mapId = uiEvent.m_eventParams.tagUInt;
                this.cacheMathingInfo.mapType = 1;
                this.cacheMathingInfo.CanGameAgain = true;
                uint tagUInt = uiEvent.m_eventParams.tagUInt;
                ResDT_LevelCommonInfo pvpMapCommonInfo = CLevelCfgLogicManager.GetPvpMapCommonInfo(this.cacheMathingInfo.mapType, tagUInt);
                ReqCreateTeam(tagUInt, false, COM_BATTLE_MAP_TYPE.COM_BATTLE_MAP_TYPE_VERSUS, pvpMapCommonInfo.bMaxAcntNum / 2, COM_AI_LEVEL.COM_AI_LEVEL_MIDDLE_OF_NORMALBATTLE, false);
                Singleton<CNewbieAchieveSys>.GetInstance().trackFlag = CNewbieAchieveSys.TrackFlag.SINGLE_MATCH_5V5_ENTER;
            }
        }

        private void OnMatching_BeginEnterTainment(CUIEvent uiEvent)
        {
            if (!uiEvent.m_eventParams.commonBool)
            {
                Singleton<CUIManager>.GetInstance().OpenTips("Matching_Tip_6", true, 1.5f, null, new object[0]);
            }
            else if (this.CanReqMatch())
            {
                bool flag = false;
                if ((uiEvent.m_srcFormScript != null) && (uiEvent.m_eventParams.tag == 0))
                {
                    flag = MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.onClickFireMatch, new uint[0]);
                }
                if (uiEvent.m_eventParams.tag == 1)
                {
                    CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
                    uIEvent.m_eventID = enUIEventID.MatchingExt_BeginEnterTrainMent;
                    uIEvent.m_eventParams.tag = uiEvent.m_eventParams.tag;
                    uIEvent.m_eventParams.tagUInt = uiEvent.m_eventParams.tagUInt;
                    uIEvent.m_eventParams.commonBool = uiEvent.m_eventParams.commonBool;
                    flag = Singleton<CBattleGuideManager>.GetInstance().OpenBannerDlgByBannerGuideId(2, uIEvent, false);
                }
                if (!flag)
                {
                    this.ResetMatchTime();
                    this.cacheMathingInfo.uiEventId = uiEvent.m_eventID;
                    this.cacheMathingInfo.mapId = uiEvent.m_eventParams.tagUInt;
                    this.cacheMathingInfo.mapType = 4;
                    this.cacheMathingInfo.CanGameAgain = true;
                    uint tagUInt = uiEvent.m_eventParams.tagUInt;
                    ResDT_LevelCommonInfo pvpMapCommonInfo = CLevelCfgLogicManager.GetPvpMapCommonInfo(this.cacheMathingInfo.mapType, tagUInt);
                    ReqCreateTeam(tagUInt, false, COM_BATTLE_MAP_TYPE.COM_BATTLE_MAP_TYPE_ENTERTAINMENT, pvpMapCommonInfo.bMaxAcntNum / 2, COM_AI_LEVEL.COM_AI_LEVEL_MIDDLE_OF_NORMALBATTLE, false);
                }
            }
        }

        private void OnMatching_BeginMeleeMulti(CUIEvent uiEvent)
        {
            if (!uiEvent.m_srcWidget.GetComponent<Button>().get_interactable())
            {
                ResSpecialFucUnlock dataByKey = GameDataMgr.specialFunUnlockDatabin.GetDataByKey((uint) 0x19);
                Singleton<CUIManager>.GetInstance().OpenTips(dataByKey.szLockedTip, false, 1.5f, null, new object[0]);
            }
            else if (this.CanReqMatch())
            {
                this.ResetMatchTime();
                this.cacheMathingInfo.uiEventId = uiEvent.m_eventID;
                this.cacheMathingInfo.mapId = uiEvent.m_eventParams.tagUInt;
                this.cacheMathingInfo.mapType = 4;
                this.cacheMathingInfo.CanGameAgain = true;
                uint tagUInt = uiEvent.m_eventParams.tagUInt;
                ResDT_LevelCommonInfo pvpMapCommonInfo = CLevelCfgLogicManager.GetPvpMapCommonInfo(this.cacheMathingInfo.mapType, tagUInt);
                ReqCreateTeam(tagUInt, false, COM_BATTLE_MAP_TYPE.COM_BATTLE_MAP_TYPE_ENTERTAINMENT, pvpMapCommonInfo.bMaxAcntNum / 2, COM_AI_LEVEL.COM_AI_LEVEL_MIDDLE_OF_NORMALBATTLE, false);
                Singleton<CNewbieAchieveSys>.GetInstance().trackFlag = CNewbieAchieveSys.TrackFlag.None;
            }
        }

        private void onMatching_ClickEnterTrainMentMore(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.GetInstance().OpenTips("Matching_Tip_5", true, 1.5f, null, new object[0]);
        }

        private void OnMatching_ConfirmGame(CUIEvent uiEvent)
        {
            Button component = uiEvent.m_srcWidget.GetComponent<Button>();
            if (component.get_interactable())
            {
                if (CFakePvPHelper.bInFakeConfirm)
                {
                    CFakePvPHelper.OnSelfConfirmed(uiEvent.m_srcFormScript.get_gameObject(), this.currentMapPlayerNum);
                }
                else
                {
                    SendMatchingConfirm();
                }
                component.set_interactable(false);
            }
        }

        private void OnMatching_KickPlayer(CUIEvent uiEvent)
        {
            if (this.IsSelfTeamMaster)
            {
                byte tag = (byte) uiEvent.m_eventParams.tag;
                for (int i = 0; i < this.teamInfo.MemInfoList.Count; i++)
                {
                    if (this.teamInfo.MemInfoList[i].dwPosOfTeam == tag)
                    {
                        ReqKickPlayer(this.teamInfo.MemInfoList[i].uID);
                        break;
                    }
                }
            }
            else
            {
                DebugHelper.Assert(false, "Not Team Master!");
            }
        }

        private void OnMatching_LeaveTeam(CUIEvent uiEvent)
        {
            if (this.bInMatchingTeam)
            {
                ReqLeaveTeam();
            }
            else
            {
                DebugHelper.Assert(false, "Not In Matching Team");
            }
        }

        private void OnMatching_OpenConfirmBox(CUIEvent uiEvent)
        {
            Utility.VibrateHelper();
            RoomInfo roomInfo = Singleton<CRoomSystem>.GetInstance().roomInfo;
            DebugHelper.Assert(roomInfo != null, "Room Info is NULL!!!");
            if (roomInfo != null)
            {
                this.currentMapPlayerNum = 0;
                if ((roomInfo.roomAttrib != null) && (roomInfo.roomAttrib.bMapType == 6))
                {
                    this.currentMapPlayerNum = CLevelCfgLogicManager.GetPvpMapCommonInfo(roomInfo.roomAttrib.bMapType, roomInfo.roomAttrib.dwMapId).bMaxAcntNum;
                }
                else
                {
                    this.currentMapPlayerNum = CLevelCfgLogicManager.GetPvpMapCommonInfo((byte) this.mapType, this.mapId).bMaxAcntNum;
                }
                this.confirmPlayerNum = 0;
                CUIFormScript form = Singleton<CUIManager>.GetInstance().OpenForm(PATH_MATCHING_CONFIRMBOX, false, true);
                CMatchingView.InitConfirmBox(form.get_gameObject(), this.currentMapPlayerNum, ref roomInfo, form);
                MonoSingleton<NewbieGuideManager>.GetInstance().StopCurrentGuide();
                MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.onOpenMatchingConfirmBox, new uint[0]);
            }
        }

        private void OnMatching_ReqLeave(CUIEvent uiEvent)
        {
            if (this.bInMatching)
            {
                ReqLeaveMatching(uiEvent != null);
            }
            else
            {
                DebugHelper.Assert(false, "Not In Matching");
            }
        }

        private void OnMatching_Robot1V1(CUIEvent uiEvent)
        {
            this.cacheMathingInfo.uiEventId = uiEvent.m_eventID;
            this.cacheMathingInfo.mapId = uiEvent.m_eventParams.tagUInt;
            this.cacheMathingInfo.mapType = 1;
            this.cacheMathingInfo.CanGameAgain = true;
            ReqStartSingleMatching(uiEvent.m_eventParams.tagUInt, true, COM_BATTLE_MAP_TYPE.COM_BATTLE_MAP_TYPE_VERSUS);
        }

        private void OnMatching_RobotTeamENTERTAINMENT(CUIEvent uiEvent)
        {
            this.cacheMathingInfo.uiEventId = uiEvent.m_eventID;
            this.cacheMathingInfo.mapId = uiEvent.m_eventParams.tagUInt;
            this.cacheMathingInfo.mapType = 4;
            this.cacheMathingInfo.AILevel = (COM_AI_LEVEL) uiEvent.m_eventParams.tag;
            this.cacheMathingInfo.CanGameAgain = true;
            uint tagUInt = uiEvent.m_eventParams.tagUInt;
            COM_AI_LEVEL tag = (COM_AI_LEVEL) uiEvent.m_eventParams.tag;
            COM_BATTLE_MAP_TYPE mapType = COM_BATTLE_MAP_TYPE.COM_BATTLE_MAP_TYPE_ENTERTAINMENT;
            ResDT_LevelCommonInfo pvpMapCommonInfo = CLevelCfgLogicManager.GetPvpMapCommonInfo(this.cacheMathingInfo.mapType, tagUInt);
            ReqCreateTeam(tagUInt, true, mapType, pvpMapCommonInfo.bMaxAcntNum / 2, tag, false);
        }

        private void OnMatching_RobotTeamVERSUS(CUIEvent uiEvent)
        {
            this.cacheMathingInfo.uiEventId = uiEvent.m_eventID;
            this.cacheMathingInfo.mapId = uiEvent.m_eventParams.tagUInt;
            this.cacheMathingInfo.mapType = 1;
            this.cacheMathingInfo.AILevel = (COM_AI_LEVEL) uiEvent.m_eventParams.tag;
            this.cacheMathingInfo.CanGameAgain = true;
            uint tagUInt = uiEvent.m_eventParams.tagUInt;
            COM_AI_LEVEL tag = (COM_AI_LEVEL) uiEvent.m_eventParams.tag;
            COM_BATTLE_MAP_TYPE mapType = COM_BATTLE_MAP_TYPE.COM_BATTLE_MAP_TYPE_VERSUS;
            ResDT_LevelCommonInfo pvpMapCommonInfo = CLevelCfgLogicManager.GetPvpMapCommonInfo(this.cacheMathingInfo.mapType, tagUInt);
            ReqCreateTeam(tagUInt, true, mapType, pvpMapCommonInfo.bMaxAcntNum / 2, tag, false);
        }

        private void OnMatching_RuleView(CUIEvent uiEvent)
        {
            int txtKey = s_PVP_RULE_ID;
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(PATH_MATCHING_ENTRY);
            if (form != null)
            {
                GameObject obj2 = form.get_transform().FindChild("panelGroup4").get_gameObject();
                if ((obj2 != null) && obj2.get_activeSelf())
                {
                    txtKey = s_TRAIN_RULE_ID;
                }
            }
            Singleton<CUIManager>.GetInstance().OpenInfoForm(txtKey);
        }

        private void OnMatching_StartMulti(CUIEvent uiEvent)
        {
            if (this.mapId > 0)
            {
                if ((this.teamInfo.stTeamInfo.bMapType == 3) && (this.teamInfo.stTeamInfo.bMaxTeamNum == 5))
                {
                    if (this.teamInfo.MemInfoList.Count != 5)
                    {
                        DebugHelper.Assert(this.teamInfo.MemInfoList.Count == 5, "房间人数不足5人，不能开始5人匹配！");
                    }
                    else
                    {
                        ReqStartMultiMatching();
                    }
                }
                else
                {
                    ReqStartMultiMatching();
                }
            }
        }

        private void OnMatchingGuide1v1(CUIEvent uiEvent)
        {
            CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
            if (CBattleGuideManager.CanSelectHeroOnTrainLevelEntry)
            {
                this.HideHeroChooseGrp(srcFormScript);
                srcFormScript.GetWidget(13).CustomSetActive(true);
            }
            else
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if (masterRoleInfo != null)
                {
                    LobbyLogic.ReqStartGuideLevel11(true, (uint) masterRoleInfo.acntMobaInfo.iSelectedHeroType);
                }
            }
        }

        private void OnMatchingGuide1v1ChooseHero(CUIEvent uiEvent)
        {
            LobbyLogic.ReqStartGuideLevel11(true, uiEvent.m_eventParams.tagUInt);
        }

        private void OnMatchingGuide3v3(CUIEvent uiEvent)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if ((masterRoleInfo == null) || !masterRoleInfo.IsGuidedStateSet(0x53))
            {
                Singleton<CUIManager>.GetInstance().OpenMessageBox(Singleton<CTextManager>.GetInstance().GetText("Trainlevel_Text_Lock_3"), false);
            }
            else
            {
                LobbyLogic.ReqStartGuideLevel33(true);
            }
        }

        private void OnMatchingGuide5v5(CUIEvent uiEvent)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if ((masterRoleInfo == null) || !masterRoleInfo.IsGuidedStateSet(0))
            {
                Singleton<CUIManager>.GetInstance().OpenMessageBox(Singleton<CTextManager>.GetInstance().GetText("Trainlevel_Text_Lock_1"), false);
            }
            else if (CBattleGuideManager.CanSelectHeroOnTrainLevelEntry)
            {
                CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
                this.HideHeroChooseGrp(srcFormScript);
                srcFormScript.GetWidget(14).CustomSetActive(true);
            }
            else
            {
                LobbyLogic.ReqStartGuideLevel55(true, (uint) masterRoleInfo.acntMobaInfo.iSelectedHeroType);
            }
        }

        private void OnMatchingGuide5v5ChooseHero(CUIEvent uiEvent)
        {
            LobbyLogic.ReqStartGuideLevel55(true, uiEvent.m_eventParams.tagUInt);
        }

        private void OnMatchingGuideAdvance(CUIEvent uiEvent)
        {
            if (GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x9b).dwConfValue > 0)
            {
                Singleton<CUIManager>.instance.OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("Tutorial_Wifi_Alert"), enUIEventID.Matching_GuideAdvanceConfirm, enUIEventID.Matching_GuideAdvanceCancel, false);
            }
            else
            {
                Singleton<CUIManager>.instance.OpenTips("Common_Not_Open", true, 1.5f, null, new object[0]);
            }
        }

        private void OnMatchingGuideCasting(CUIEvent uiEvent)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if ((masterRoleInfo == null) || !masterRoleInfo.IsGuidedStateSet(0x62))
            {
                Singleton<CUIManager>.GetInstance().OpenMessageBox(Singleton<CTextManager>.GetInstance().GetText("Trainlevel_Text_Lock_2"), false);
            }
            else
            {
                LobbyLogic.ReqStartGuideLevelCasting(masterRoleInfo.IsGuidedStateSet(0x53));
            }
        }

        private void OnMatchingGuideJungle(CUIEvent uiEvent)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if ((masterRoleInfo == null) || !masterRoleInfo.IsGuidedStateSet(0x55))
            {
                Singleton<CUIManager>.GetInstance().OpenMessageBox(Singleton<CTextManager>.GetInstance().GetText("Trainlevel_Text_Lock_4"), false);
            }
            else
            {
                LobbyLogic.ReqStartGuideLevelJungle(true);
            }
        }

        private void OnMatchingGuidePanel(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.instance.CloseForm(CRoomSystem.PATH_CREATE_ROOM);
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(PATH_MATCHING_ENTRY);
            if (null == form)
            {
                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Matching_OpenEntry);
            }
            form = Singleton<CUIManager>.instance.GetForm(PATH_MATCHING_ENTRY);
            if (form != null)
            {
                form.GetWidget(9).CustomSetActive(true);
                form.GetWidget(10).CustomSetActive(false);
                form.GetWidget(2).CustomSetActive(false);
                form.GetWidget(3).CustomSetActive(false);
                form.GetWidget(4).CustomSetActive(false);
                form.GetWidget(11).CustomSetActive(false);
                this.ShowAwards(form);
                this.ShowBonusImage(form);
                this.SetGuideEntryEvent(form);
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
                if (masterRoleInfo != null)
                {
                    masterRoleInfo.SetNewbieAchieve(0x11, true, true);
                }
            }
            MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.onEntryTrainLevelEntry, new uint[0]);
        }

        private void OnMatchingRoom_OpenEntry(CUIEvent uiEvent)
        {
            Singleton<CNewbieAchieveSys>.GetInstance().trackFlag = CNewbieAchieveSys.TrackFlag.None;
            if (Singleton<CFunctionUnlockSys>.instance.FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_PVPMODE))
            {
                if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo() != null)
                {
                    if (this.IsInMatching)
                    {
                        Singleton<CUIManager>.GetInstance().OpenTips("PVP_Matching", true, 1.5f, null, new object[0]);
                    }
                    else if (uiEvent.m_eventParams.tag != 0)
                    {
                        this.OpenPvPEntry((enPvPEntryFormWidget) uiEvent.m_eventParams.tag);
                    }
                    else
                    {
                        this.OpenPvPEntry(enPvPEntryFormWidget.None);
                    }
                }
            }
            else
            {
                ResSpecialFucUnlock dataByKey = GameDataMgr.specialFunUnlockDatabin.GetDataByKey((uint) 10);
                Singleton<CUIManager>.instance.OpenTips(Utility.UTF8Convert(dataByKey.szLockedTip), false, 1.5f, null, new object[0]);
            }
        }

        private void OnMatchingTraining(CUIEvent uiEvent)
        {
            ReqStartTrainingLevel();
        }

        public void onMatchWatingTimeUp(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.GetInstance().CloseForm(PATH_MATCHING_WAITING);
        }

        [MessageHandler(0x7f0)]
        public static void OnPlayerConfirmMatching(CSPkg msg)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(PATH_MATCHING_CONFIRMBOX);
            if (form != null)
            {
                CMatchingSystem instance = Singleton<CMatchingSystem>.GetInstance();
                instance.confirmPlayerNum++;
                CMatchingView.UpdateConfirmBox(form.get_gameObject(), msg.stPkgData.stRoomConfirmRsp.ullUid);
                RoomInfo roomInfo = Singleton<CRoomSystem>.GetInstance().roomInfo;
                if ((roomInfo != null) && roomInfo.roomAttrib.bWarmBattle)
                {
                    CFakePvPHelper.UpdateConfirmBox(form.get_gameObject(), Singleton<CMatchingSystem>.GetInstance().currentMapPlayerNum);
                }
            }
        }

        [MessageHandler(0x7e7)]
        public static void OnPlayerJoin(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            if (msg.stPkgData.stJoinTeamRsp.bErrCode == 0)
            {
                CMatchingSystem instance = Singleton<CMatchingSystem>.GetInstance();
                instance.mapId = msg.stPkgData.stJoinTeamRsp.stJoinRsp.stOfSucc.stTeamInfo.dwMapId;
                instance.mapType = (COM_BATTLE_MAP_TYPE) msg.stPkgData.stJoinTeamRsp.stJoinRsp.stOfSucc.stTeamInfo.bMapType;
                instance.maxTeamNum = msg.stPkgData.stJoinTeamRsp.stJoinRsp.stOfSucc.stTeamInfo.bMaxTeamNum;
                instance.bInMatchingTeam = true;
                instance.InitTeamInfo(msg.stPkgData.stJoinTeamRsp.stJoinRsp.stOfSucc);
                CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(PATH_MATCHING_MULTI, false, true);
                Singleton<CTopLobbyEntry>.GetInstance().OpenForm();
                Singleton<CInviteSystem>.GetInstance().OpenInviteForm(COM_INVITE_JOIN_TYPE.COM_INVITE_JOIN_TEAM);
                SetTeamData(script.get_gameObject(), instance.teamInfo);
                instance.cacheMathingInfo.CanGameAgain = instance.IsSelfTeamMaster;
                if (!instance.IsSelfTeamMaster)
                {
                    MonoSingleton<NewbieGuideManager>.instance.StopCurrentGuide();
                }
                if (MonoSingleton<ShareSys>.instance.IsQQGameTeamCreate())
                {
                    CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                    string roomStr = MonoSingleton<ShareSys>.instance.PackQQGameTeamData(masterRoleInfo.playerUllUID, instance.teamInfo.TeamId, instance.teamInfo.TeamSeq, instance.teamInfo.TeamEntity, instance.teamInfo.TeamFeature, masterRoleInfo.m_rankGrade, instance.teamInfo.stTeamInfo.bGameMode, instance.teamInfo.stTeamInfo.bPkAI, (byte) instance.cacheMathingInfo.AILevel, instance.teamInfo.stTeamInfo.bMaxTeamNum);
                    MonoSingleton<ShareSys>.instance.SendQQGameTeamStateChgMsg(ShareSys.QQGameTeamEventType.join, COM_ROOM_TYPE.COM_ROOM_TYPE_MATCH, instance.teamInfo.stTeamInfo.bMapType, instance.teamInfo.stTeamInfo.dwMapId, roomStr);
                }
                CloseExcludeForm();
            }
            else if (msg.stPkgData.stJoinTeamRsp.bErrCode == 0x11)
            {
                MatchPunishmentWaiting((float) msg.stPkgData.stJoinTeamRsp.stJoinRsp.stOfBePunished.dwLeftSec, msg.stPkgData.stJoinTeamRsp.stJoinRsp.stOfBePunished.bType);
            }
            else if (msg.stPkgData.stJoinTeamRsp.bErrCode == 0x16)
            {
                Singleton<CUIManager>.instance.OpenTips("HuoKenPlayModeNotOpenTip", true, 1.5f, null, new object[0]);
            }
            else
            {
                Singleton<CUIManager>.instance.OpenTips(Utility.ProtErrCodeToStr(0x7e7, msg.stPkgData.stJoinTeamRsp.bErrCode), false, 1.5f, null, new object[0]);
            }
        }

        public static void OnPlayerLeaveMatching()
        {
            CloseInMatchingForm();
        }

        private void OnRobotBtnGroupClick(CUIEvent uiEvent)
        {
            this.HideRobotBtnGroup();
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(PATH_MATCHING_ENTRY);
            if (form != null)
            {
                int tag = uiEvent.m_eventParams.tag;
                if (tag >= 0)
                {
                    if ((tag == 3) && !uiEvent.m_srcWidget.GetComponent<Button>().get_interactable())
                    {
                        ResSpecialFucUnlock dataByKey = GameDataMgr.specialFunUnlockDatabin.GetDataByKey((uint) 0x19);
                        Singleton<CUIManager>.GetInstance().OpenTips(dataByKey.szLockedTip, false, 1.5f, null, new object[0]);
                    }
                    else
                    {
                        GameObject widget = null;
                        switch (tag)
                        {
                            case 0:
                                widget = form.GetWidget(5);
                                break;

                            case 1:
                                widget = form.GetWidget(6);
                                break;

                            case 2:
                                widget = form.GetWidget(7);
                                break;

                            case 3:
                                widget = form.GetWidget(8);
                                break;

                            case 4:
                                widget = form.GetWidget(12);
                                break;
                        }
                        if (widget != null)
                        {
                            widget.CustomSetActive(true);
                        }
                        if (tag == 1)
                        {
                            MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.onClick3v3AI, new uint[0]);
                        }
                    }
                }
            }
        }

        [MessageHandler(0x7ed)]
        public static void OnSelfBeKicked(CSPkg msg)
        {
            Singleton<CMatchingSystem>.GetInstance().bInMatchingTeam = false;
            Singleton<CUIManager>.GetInstance().CloseForm(PATH_MATCHING_MULTI);
            Singleton<CTopLobbyEntry>.GetInstance().CloseForm();
            Singleton<CInviteSystem>.GetInstance().CloseInviteForm();
            CChatUT.LeaveRoom();
            Singleton<CChatController>.instance.ShowPanel(false, false);
            Singleton<CUIManager>.GetInstance().OpenTips("PVP_Kick_Tip", true, 1.5f, null, new object[0]);
            MonoSingleton<ShareSys>.GetInstance().SendQQGameTeamStateChgMsg(ShareSys.QQGameTeamEventType.leave, COM_ROOM_TYPE.COM_ROOM_TYPE_NULL, 0, 0, string.Empty);
        }

        [MessageHandler(0x7db)]
        public static void OnStartMatching(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            if (msg.stPkgData.stMatchRsp.bResult == 2)
            {
                Singleton<CUIManager>.GetInstance().CloseAllFormExceptLobby(true);
                Singleton<CTopLobbyEntry>.instance.CloseForm();
                Singleton<CMatchingSystem>.GetInstance().bInMatching = true;
                OpenInMatchingForm(msg.stPkgData.stMatchRsp.stMatchResDetail.stMatchProcess.dwWaitTime);
                Singleton<CChatController>.instance.model.channelMgr.Clear(EChatChannel.Team, 0L, 0);
                Singleton<CChatController>.instance.model.channelMgr.Clear(EChatChannel.Room, 0L, 0);
                Singleton<CChatController>.instance.model.channelMgr.SetChatTab(CChatChannelMgr.EChatTab.Normal);
                Singleton<CChatController>.instance.ShowPanel(true, false);
                Singleton<CChatController>.instance.view.UpView(false);
                Singleton<CChatController>.instance.model.sysData.ClearEntryText();
                if (msg.stPkgData.stMatchRsp.stMatchResDetail.stMatchProcess.bReason == 0x12)
                {
                    Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Err_NM_Othercancel"), false, 1.5f, null, new object[0]);
                }
                else if (msg.stPkgData.stMatchRsp.stMatchResDetail.stMatchProcess.bReason == 0x13)
                {
                    Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Err_NM_Otherexit"), false, 1.5f, null, new object[0]);
                    Singleton<CSoundManager>.GetInstance().PostEvent("UI_matching_lost", null);
                }
                MonoSingleton<ShareSys>.GetInstance().SendQQGameTeamStateChgMsg(ShareSys.QQGameTeamEventType.start, COM_ROOM_TYPE.COM_ROOM_TYPE_NULL, 0, 0, string.Empty);
            }
            else if (msg.stPkgData.stMatchRsp.bResult == 1)
            {
                CloseInMatchingForm();
                Singleton<CRoomSystem>.GetInstance().BuildRoomInfo(msg.stPkgData.stMatchRsp.stMatchResDetail.stMatchSucc);
                Singleton<CRoomSystem>.instance.SetRoomType(1);
                Singleton<CUIManager>.GetInstance().CloseAllFormExceptLobby(true);
            }
            else if (msg.stPkgData.stMatchRsp.bResult == 3)
            {
                CloseInMatchingForm();
                if (msg.stPkgData.stMatchRsp.stMatchResDetail.stMatchErr.iErrCode == 1)
                {
                    DateTime banTime = MonoSingleton<IDIPSys>.GetInstance().GetBanTime(COM_ACNT_BANTIME_TYPE.COM_ACNT_BANTIME_BANPLAYPVP);
                    object[] args = new object[] { banTime.Year, banTime.Month, banTime.Day, banTime.Hour, banTime.Minute };
                    string strContent = string.Format("您被禁止竞技！截止时间为{0}年{1}月{2}日{3}时{4}分", args);
                    Singleton<CUIManager>.GetInstance().OpenMessageBox(strContent, false);
                }
                else if (msg.stPkgData.stMatchRsp.stMatchResDetail.stMatchErr.iErrCode == 2)
                {
                    Singleton<CUIManager>.GetInstance().OpenMessageBox(Singleton<CTextManager>.instance.GetText("Union_Battle_Tips1"), false);
                }
                else if (msg.stPkgData.stMatchRsp.stMatchResDetail.stMatchErr.iErrCode == 3)
                {
                    Singleton<CUIManager>.GetInstance().OpenMessageBox(Singleton<CTextManager>.instance.GetText("Union_Battle_Tips4"), false);
                }
                else if (msg.stPkgData.stMatchRsp.stMatchResDetail.stMatchErr.iErrCode == 4)
                {
                    float dwLeftSec = msg.stPkgData.stMatchRsp.stMatchResDetail.stMatchErr.stErrParam.stBePunished.dwLeftSec;
                    int bType = msg.stPkgData.stMatchRsp.stMatchResDetail.stMatchErr.stErrParam.stBePunished.bType;
                    MatchPunishmentWaiting(dwLeftSec, bType);
                }
                else if (msg.stPkgData.stMatchRsp.stMatchResDetail.stMatchErr.iErrCode == 5)
                {
                    Singleton<CUIManager>.GetInstance().CloseForm(PATH_MATCHING_MULTI);
                    Singleton<CTopLobbyEntry>.GetInstance().CloseForm();
                    Singleton<CInviteSystem>.GetInstance().CloseInviteForm();
                    Singleton<CUIManager>.GetInstance().OpenMessageBox(Singleton<CTextManager>.instance.GetText("PVP_Matching_Errpr_1"), false);
                }
                else
                {
                    object[] replaceArr = new object[] { Utility.ProtErrCodeToStr(0x7db, msg.stPkgData.stMatchRsp.stMatchResDetail.stMatchErr.iErrCode) };
                    Singleton<CUIManager>.GetInstance().OpenTips("PVP_Matching_Errpr", true, 1f, null, replaceArr);
                }
            }
        }

        public void OnTeam_ShareFriend_Team(CUIEvent uiEvent)
        {
            CMatchingSystem instance = Singleton<CMatchingSystem>.GetInstance();
            if (instance != null)
            {
                uint mapId = instance.mapId;
                int mapType = (int) instance.mapType;
                int maxTeamNum = instance.maxTeamNum;
                string szName = string.Empty;
                string str2 = string.Empty;
                szName = CLevelCfgLogicManager.GetPvpMapCommonInfo((byte) mapType, mapId).szName;
                if (mapType == 3)
                {
                    if (maxTeamNum == 2)
                    {
                        str2 = Singleton<CTextManager>.instance.GetText("Common_Team_Player_Type_6");
                    }
                    else if (maxTeamNum == 5)
                    {
                        str2 = Singleton<CTextManager>.instance.GetText("Common_Team_Player_Type_7");
                    }
                }
                else
                {
                    str2 = Singleton<CTextManager>.instance.GetText(string.Format("Common_Team_Player_Type_{0}", maxTeamNum));
                }
                byte bInviterGradeOfRank = 0;
                if ((mapType == 3) && (maxTeamNum == 2))
                {
                    CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                    if (masterRoleInfo != null)
                    {
                        bInviterGradeOfRank = masterRoleInfo.m_rankGrade;
                    }
                }
                else
                {
                    bInviterGradeOfRank = (byte) this.teamInfo.stTeamInfo.iGradofRank;
                }
                byte iGradofRank = (byte) this.teamInfo.stTeamInfo.iGradofRank;
                string text = Singleton<CTextManager>.GetInstance().GetText("Share_Room_Info_Title");
                string[] args = new string[] { str2, szName };
                string desc = Singleton<CTextManager>.instance.GetText("Share_Room_Info_Desc", args);
                string roomInfo = MonoSingleton<ShareSys>.GetInstance().PackTeamData(this.teamInfo.stSelfInfo.ullUid, this.teamInfo.TeamId, this.teamInfo.TeamSeq, this.teamInfo.TeamEntity, this.teamInfo.TeamFeature, bInviterGradeOfRank, this.teamInfo.stTeamInfo.bGameMode, this.teamInfo.stTeamInfo.bPkAI, this.teamInfo.stTeamInfo.bMapType, this.teamInfo.stTeamInfo.dwMapId, (byte) this.cacheMathingInfo.AILevel, this.teamInfo.stTeamInfo.bMaxTeamNum, iGradofRank);
                Singleton<ApolloHelper>.GetInstance().InviteFriendToRoom(text, desc, roomInfo);
            }
        }

        [MessageHandler(0x7ea)]
        public static void OnTeamChange(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            TeamInfo teamInfo = Singleton<CMatchingSystem>.GetInstance().teamInfo;
            bool flag = false;
            if (msg.stPkgData.stTeamChgNtf.stChgDt.iChgType == 0)
            {
                TeamMember item = Singleton<CMatchingSystem>.GetInstance().CreateTeamMemberInfo(msg.stPkgData.stTeamChgNtf.stChgDt.stChgInfo.stPlayerAdd.stMemInfo);
                teamInfo.MemInfoList.Add(item);
                flag = true;
            }
            else if (msg.stPkgData.stTeamChgNtf.stChgDt.iChgType != 1)
            {
                if (msg.stPkgData.stTeamChgNtf.stChgDt.iChgType == 2)
                {
                    teamInfo.stTeamMaster.ullUid = msg.stPkgData.stTeamChgNtf.stChgDt.stChgInfo.stMasterChg.stNewMaster.ullUid;
                    teamInfo.stTeamMaster.iGameEntity = msg.stPkgData.stTeamChgNtf.stChgDt.stChgInfo.stMasterChg.stNewMaster.iGameEntity;
                    flag = true;
                }
            }
            else
            {
                COMDT_TEAM_MEMBER_UNIQ stLevelMember = msg.stPkgData.stTeamChgNtf.stChgDt.stChgInfo.stPlayerLeave.stLevelMember;
                for (int i = 0; i < teamInfo.MemInfoList.Count; i++)
                {
                    if ((teamInfo.MemInfoList[i].uID.ullUid == stLevelMember.ullUid) && (teamInfo.MemInfoList[i].uID.iGameEntity == stLevelMember.iGameEntity))
                    {
                        teamInfo.MemInfoList.RemoveAt(i);
                        break;
                    }
                }
                flag = true;
            }
            if (flag)
            {
                CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(PATH_MATCHING_MULTI);
                if (form != null)
                {
                    SetTeamData(form.get_gameObject(), teamInfo);
                }
            }
        }

        private void OpenGuideAdvancePage(CUIEvent uiEvent)
        {
            CUICommonSystem.OpenUrl("http://pvp.qq.com/ingame/all/video_stage.shtml?partition=" + MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID, true, 0);
        }

        private static void OpenInMatchingForm(uint preTime)
        {
            CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(PATH_MATCHING_INMATCHING, false, true);
            if (script != null)
            {
                Transform textObjTrans = script.get_transform().Find("Panel/Predict_Time");
                uint num = preTime / 60;
                uint num2 = preTime - (num * 60);
                uint globeValue = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_WAITTIME_MAX);
                if (preTime <= globeValue)
                {
                    string[] args = new string[] { num.ToString("D2"), num2.ToString("D2") };
                    CUICommonSystem.SetTextContent(textObjTrans, Singleton<CTextManager>.instance.GetText("Lobby_MatchTime_TitleShow", args));
                }
                else
                {
                    CUICommonSystem.SetTextContent(textObjTrans, Singleton<CTextManager>.instance.GetText("Lobby_MatchTime_OverShow"));
                }
            }
        }

        public void OpenPvPEntry(enPvPEntryFormWidget enOpenEntry = 3)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(PATH_MATCHING_ENTRY);
            if (form == null)
            {
                form = Singleton<CUIManager>.GetInstance().OpenForm(PATH_MATCHING_ENTRY, false, true);
                CMatchingView.InitMatchingEntry(form);
                this.OnBtnGroupClose(null);
            }
            if (enOpenEntry != enPvPEntryFormWidget.None)
            {
                CUICommonSystem.SetObjActive(form.GetWidget((int) enOpenEntry), true);
                CUICommonSystem.SetObjActive(form.GetWidget(2), false);
            }
            if (enOpenEntry == enPvPEntryFormWidget.GuideBtnGroup)
            {
                this.ShowAwards(form);
                this.SetGuideEntryEvent(form);
                form.GetWidget(10).CustomSetActive(false);
            }
            if (enOpenEntry == enPvPEntryFormWidget.PlayerBattleBtnGroupPanel)
            {
                GameObject btnObj = form.get_transform().FindChild("panelGroup2/btnGroup/Button3").get_gameObject();
                this.EntertainMentAddLock(btnObj);
            }
            else if (enOpenEntry == enPvPEntryFormWidget.ComputerBattleBtnGroupPanel)
            {
                GameObject obj3 = form.get_transform().FindChild("panelGroup3/btnGroup/Button3").get_gameObject();
                this.EntertainMentAddLock(obj3);
            }
            this.ShowBonusImage(form);
        }

        private CUseable QueryLevelAwardItem(int levelId)
        {
            ResLevelCfgInfo dataByKey = GameDataMgr.levelDatabin.GetDataByKey((long) levelId);
            if (dataByKey == null)
            {
                return null;
            }
            uint key = dataByKey.SettleIDDetail[0];
            ResCommonSettle settle = GameDataMgr.settleDatabin.GetDataByKey(key);
            if (settle == null)
            {
                return null;
            }
            uint dwRewardID = settle.astFirstCompleteReward[0].dwRewardID;
            ResRandomRewardStore inRewardInfo = GameDataMgr.randomRewardDB.GetDataByKey(dwRewardID);
            if (inRewardInfo == null)
            {
                return null;
            }
            return CUseableManager.GetUseableByRewardInfo(inRewardInfo);
        }

        public static void ReqCreateTeam(uint MapId, bool bPkAI, COM_BATTLE_MAP_TYPE mapType, int maxTeamNum, COM_AI_LEVEL npcAILevel = 2, bool isInviteFriendImmediately = false)
        {
            CInviteSystem.s_isInviteFriendImmidiately = isInviteFriendImmediately;
            DebugHelper.Assert(MapId != 0, "MapId Should not be 0!!!");
            if (MapId > 0)
            {
                Singleton<CMatchingSystem>.GetInstance().cacheMathingInfo.AILevel = npcAILevel;
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x7e6);
                Singleton<CMatchingSystem>.GetInstance().mapId = MapId;
                Singleton<CMatchingSystem>.GetInstance().mapType = mapType;
                Singleton<CMatchingSystem>.GetInstance().maxTeamNum = (byte) maxTeamNum;
                msg.stPkgData.stCreateTeamReq.stBaseInfo.bGameMode = 1;
                msg.stPkgData.stCreateTeamReq.stBaseInfo.dwMapId = MapId;
                msg.stPkgData.stCreateTeamReq.stBaseInfo.bMapType = (byte) mapType;
                msg.stPkgData.stCreateTeamReq.stBaseInfo.bPkAI = Convert.ToByte(!bPkAI ? COM_GAMEPK_TYPE.COM_GAMEPK_WITHOUT_AI : COM_GAMEPK_TYPE.COM_GAMEPK_WITH_AI);
                msg.stPkgData.stCreateTeamReq.stBaseInfo.bAILevel = Convert.ToByte(npcAILevel);
                msg.stPkgData.stCreateTeamReq.stBaseInfo.bGradeOfRank = 0;
                msg.stPkgData.stCreateTeamReq.stBaseInfo.bMaxTeamNum = (byte) maxTeamNum;
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
                Singleton<WatchController>.GetInstance().Stop();
            }
        }

        public static void ReqCreateTeamAndInvite(uint mapId, COM_BATTLE_MAP_TYPE mapType, CInviteSystem.stInviteInfo inviteInfo)
        {
            Singleton<CInviteSystem>.GetInstance().InviteInfo = inviteInfo;
            ReqCreateTeam(mapId, false, mapType, inviteInfo.maxTeamNum, COM_AI_LEVEL.COM_AI_LEVEL_MIDDLE_OF_NORMALBATTLE, true);
        }

        public static void ReqKickPlayer(PlayerUniqueID uid)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x7ec);
            msg.stPkgData.stOperTeamReq.stOper.iOperType = 2;
            msg.stPkgData.stOperTeamReq.stOper.stOperDetail.construct(2L);
            msg.stPkgData.stOperTeamReq.stOper.stOperDetail.stKickOutTeamMember.ullUid = uid.ullUid;
            msg.stPkgData.stOperTeamReq.stOper.stOperDetail.stKickOutTeamMember.iGameEntity = uid.iGameEntity;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public static void ReqLeaveMatching(bool bManual)
        {
            if (Singleton<CMatchingSystem>.GetInstance().bInMatching)
            {
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x3ff);
                msg.stPkgData.stQuitMultGameReq.bManualQuit = !bManual ? ((byte) 0) : ((byte) 1);
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
            }
        }

        public static void ReqLeaveTeam()
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x7eb);
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        public static void ReqStartMultiMatching()
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x7ec);
            msg.stPkgData.stOperTeamReq.stOper.iOperType = 1;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public static void ReqStartSingleMatching(uint MapId, bool bPkAI, COM_BATTLE_MAP_TYPE mapType = 1)
        {
            Singleton<CMatchingSystem>.GetInstance().mapId = MapId;
            Singleton<CMatchingSystem>.GetInstance().mapType = mapType;
            Singleton<CMatchingSystem>.GetInstance().maxTeamNum = 1;
            Singleton<CMatchingSystem>.GetInstance().cacheMathingInfo.AILevel = COM_AI_LEVEL.COM_AI_LEVEL_MIDDLE_OF_NORMALBATTLE;
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x7da);
            msg.stPkgData.stMatchReq.bMapType = Convert.ToByte(mapType);
            msg.stPkgData.stMatchReq.dwMapId = MapId;
            msg.stPkgData.stMatchReq.bIsPkAI = Convert.ToByte(!bPkAI ? COM_GAMEPK_TYPE.COM_GAMEPK_WITHOUT_AI : COM_GAMEPK_TYPE.COM_GAMEPK_WITH_AI);
            msg.stPkgData.stMatchReq.bGameMode = 1;
            msg.stPkgData.stMatchReq.bAILevel = Convert.ToByte(COM_AI_LEVEL.COM_AI_LEVEL_MIDDLE_OF_NORMALBATTLE);
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
            Singleton<WatchController>.GetInstance().Stop();
        }

        public static void ReqStartTrainingLevel()
        {
            int dwConfValue = (int) GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 120).dwConfValue;
            LobbyLogic.ReqStartGuideLevelSelHero(true, dwConfValue);
        }

        private void ResetMatchTime()
        {
            this.m_lastReqMathTime = CRoleInfo.GetCurrentUTCTime();
        }

        public static void SendMatchingConfirm()
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x7ef);
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        private void SetGuideEntryEvent(CUIFormScript form)
        {
            Transform transform = form.GetWidget(13).get_transform();
            Transform transform2 = form.GetWidget(14).get_transform();
            if ((transform != null) && (transform2 != null))
            {
                transform.FindChild("Button1").GetComponent<CUIMiniEventScript>().m_onClickEventParams.tagUInt = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_NEWBIE_RECOMMEND_HEROTYPE1);
                transform.FindChild("Button2").GetComponent<CUIMiniEventScript>().m_onClickEventParams.tagUInt = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_NEWBIE_RECOMMEND_HEROTYPE2);
                transform.FindChild("Button3").GetComponent<CUIMiniEventScript>().m_onClickEventParams.tagUInt = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_NEWBIE_RECOMMEND_HEROTYPE3);
                transform2.FindChild("Button1").GetComponent<CUIMiniEventScript>().m_onClickEventParams.tagUInt = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_NEWBIE_RECOMMEND_HEROTYPE1);
                transform2.FindChild("Button2").GetComponent<CUIMiniEventScript>().m_onClickEventParams.tagUInt = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_NEWBIE_RECOMMEND_HEROTYPE2);
                transform2.FindChild("Button3").GetComponent<CUIMiniEventScript>().m_onClickEventParams.tagUInt = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_NEWBIE_RECOMMEND_HEROTYPE3);
                transform.get_gameObject().CustomSetActive(false);
                transform2.get_gameObject().CustomSetActive(false);
            }
        }

        private static void SetItemCell(CUIFormScript formScript, GameObject itemCell, CUseable itemUseable)
        {
            Image component = itemCell.get_transform().Find("imgIcon").GetComponent<Image>();
            Text target = itemCell.get_transform().Find("lblIconCount").GetComponent<Text>();
            CUIUtility.SetImageSprite(component, itemUseable.GetIconPath(), formScript, true, false, false, false);
            target.set_text(itemUseable.m_stackCount.ToString());
            CUICommonSystem.AppendMultipleText(target, itemUseable.m_stackMulti);
            if (itemUseable.m_stackCount <= 0)
            {
                target.get_gameObject().CustomSetActive(false);
            }
        }

        public static void SetTeamData(GameObject root, TeamInfo data)
        {
            uint dwMapId = data.stTeamInfo.dwMapId;
            int bMapType = data.stTeamInfo.bMapType;
            int num3 = data.stTeamInfo.bMaxTeamNum * 2;
            ResDT_LevelCommonInfo pvpMapCommonInfo = CLevelCfgLogicManager.GetPvpMapCommonInfo((byte) bMapType, dwMapId);
            root.get_transform().Find("Panel_Main/MapInfo/txtMapName").get_gameObject().GetComponent<Text>().set_text(pvpMapCommonInfo.szName);
            if (bMapType == 3)
            {
                if (data.stTeamInfo.bMaxTeamNum == 2)
                {
                    root.get_transform().Find("Panel_Main/MapInfo/txtTeam").get_gameObject().GetComponent<Text>().set_text(Singleton<CTextManager>.instance.GetText("Common_Team_Player_Type_6"));
                }
                else if (data.stTeamInfo.bMaxTeamNum == 5)
                {
                    root.get_transform().Find("Panel_Main/MapInfo/txtTeam").get_gameObject().GetComponent<Text>().set_text(Singleton<CTextManager>.instance.GetText("Common_Team_Player_Type_7"));
                }
            }
            else
            {
                root.get_transform().Find("Panel_Main/MapInfo/txtTeam").get_gameObject().GetComponent<Text>().set_text(Singleton<CTextManager>.instance.GetText(string.Format("Common_Team_Player_Type_{0}", num3 / 2)));
            }
            Transform transform = root.get_transform().Find("Panel_Main/MapInfo/TextRank");
            Transform transform2 = root.get_transform().Find("Panel_Main/MapInfo/TextRankbg");
            if (transform != null)
            {
                if ((bMapType == 3) && (data.stTeamInfo.bMaxTeamNum == 5))
                {
                    transform.get_gameObject().CustomSetActive(true);
                    transform2.get_gameObject().CustomSetActive(true);
                    string text = Singleton<CTextManager>.GetInstance().GetText("Rank_Team_Grade_Limit");
                    byte rankBigGrade = CLadderSystem.GetRankBigGrade((byte) data.stTeamInfo.iGradofRank);
                    int num5 = 0;
                    string[] args = new string[3];
                    string rankBigGradeName = CLadderView.GetRankBigGradeName((byte) (rankBigGrade - 1));
                    if (!string.IsNullOrEmpty(rankBigGradeName))
                    {
                        args[num5++] = rankBigGradeName;
                    }
                    rankBigGradeName = CLadderView.GetRankBigGradeName(rankBigGrade);
                    if (!string.IsNullOrEmpty(rankBigGradeName))
                    {
                        args[num5++] = rankBigGradeName;
                    }
                    rankBigGradeName = CLadderView.GetRankBigGradeName((byte) (rankBigGrade + 1));
                    if (!string.IsNullOrEmpty(rankBigGradeName))
                    {
                        args[num5++] = rankBigGradeName;
                    }
                    if (num5 == 2)
                    {
                        transform.GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("Rank_Team_Grade_Limit_3", args));
                    }
                    else if (num5 == 3)
                    {
                        transform.GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("Rank_Team_Grade_Limit_2", args));
                    }
                }
                else
                {
                    transform.get_gameObject().CustomSetActive(false);
                    transform2.get_gameObject().CustomSetActive(false);
                }
            }
            GameObject obj2 = root.get_transform().Find("Panel_Main/Btn_Matching").get_gameObject();
            obj2.CustomSetActive(Singleton<CMatchingSystem>.GetInstance().IsSelfTeamMaster);
            GameObject item = root.get_transform().Find("Panel_Main/Players/Player1").get_gameObject();
            TeamMember memberInfo = TeamInfo.GetMemberInfo(data, 1);
            CMatchingView.SetPlayerSlotData(item, memberInfo, num3 >= 2);
            item = root.get_transform().Find("Panel_Main/Players/Player2").get_gameObject();
            memberInfo = TeamInfo.GetMemberInfo(data, 2);
            CMatchingView.SetPlayerSlotData(item, memberInfo, num3 >= 4);
            item = root.get_transform().Find("Panel_Main/Players/Player3").get_gameObject();
            memberInfo = TeamInfo.GetMemberInfo(data, 3);
            CMatchingView.SetPlayerSlotData(item, memberInfo, num3 >= 6);
            item = root.get_transform().Find("Panel_Main/Players/Player4").get_gameObject();
            memberInfo = TeamInfo.GetMemberInfo(data, 4);
            CMatchingView.SetPlayerSlotData(item, memberInfo, num3 >= 8);
            item = root.get_transform().Find("Panel_Main/Players/Player5").get_gameObject();
            memberInfo = TeamInfo.GetMemberInfo(data, 5);
            CMatchingView.SetPlayerSlotData(item, memberInfo, num3 >= 10);
            Button component = obj2.GetComponent<Button>();
            if (component != null)
            {
                if ((data.stTeamInfo.bMapType == 3) && (data.stTeamInfo.bMaxTeamNum == 5))
                {
                    bool isEnable = data.MemInfoList.Count == 5;
                    CUICommonSystem.SetButtonEnable(component, isEnable, isEnable, true);
                }
                else
                {
                    CUICommonSystem.SetButtonEnable(component, true, true, true);
                }
            }
            GameObject obj4 = root.get_transform().Find("Panel_Main/BPTag").get_gameObject();
            if (bMapType == 3)
            {
                obj4.CustomSetActive(HasBpGradeMember(data));
            }
            else
            {
                obj4.CustomSetActive(false);
            }
        }

        private void ShowAwards(CUIFormScript form)
        {
            if (form != null)
            {
                List<int> list = new List<int>();
                list.Add((int) GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x77).dwConfValue);
                list.Add((int) GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x79).dwConfValue);
                list.Add((int) GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x74).dwConfValue);
                list.Add((int) GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x75).dwConfValue);
                list.Add((int) GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x76).dwConfValue);
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
                List<int> list2 = new List<int>();
                list2.Add(0x53);
                list2.Add(0x54);
                list2.Add(0);
                list2.Add(0x55);
                list2.Add(0x62);
                int count = list.Count;
                for (int i = 0; i < count; i++)
                {
                    int levelId = list[i];
                    CUseable item = this.QueryLevelAwardItem(levelId);
                    string str = string.Format("panelGroup4/itemCell{0}", i + 2);
                    string str2 = string.Format("panelGroup4/Complete{0}", i + 2);
                    GameObject itemCell = form.get_transform().FindChild(str).get_gameObject();
                    GameObject itemComplete = form.get_transform().FindChild(str2).get_gameObject();
                    bool bFin = masterRoleInfo.IsGuidedStateSet(list2[i]);
                    this.ShowAwardTip(item, itemCell, form, bFin, itemComplete);
                    Transform transform = form.GetWidget(9).get_transform().FindChild(string.Format("btnGroup/Button{0}", i + 2));
                    if (this.IsTraningLevelLocked(masterRoleInfo, list2[i]))
                    {
                        transform.get_gameObject().GetComponent<Image>().set_color(CUIUtility.s_Color_GrayShader);
                        transform.FindChild("Lock").get_gameObject().CustomSetActive(true);
                    }
                    else
                    {
                        transform.get_gameObject().GetComponent<Image>().set_color(CUIUtility.s_Color_White);
                        transform.FindChild("Lock").get_gameObject().CustomSetActive(false);
                    }
                }
            }
        }

        private void ShowAwardTip(CUseable item, GameObject itemCell, CUIFormScript form, bool bFin, GameObject itemComplete)
        {
            if ((form != null) && (itemCell != null))
            {
                if (item != null)
                {
                    if (bFin)
                    {
                        itemCell.CustomSetActive(false);
                        itemComplete.CustomSetActive(true);
                    }
                    else
                    {
                        itemCell.CustomSetActive(true);
                        SetItemCell(form, itemCell, item);
                        itemComplete.CustomSetActive(false);
                    }
                }
                else
                {
                    itemCell.CustomSetActive(false);
                    itemComplete.CustomSetActive(bFin);
                }
            }
        }

        public void ShowBonusImage(CUIFormScript form)
        {
            if (form != null)
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
                GameObject obj2 = form.get_transform().FindChild("panelGroupBottom/ButtonTrain/ImageBonus").get_gameObject();
                if ((masterRoleInfo != null) && masterRoleInfo.IsTrainingLevelFin())
                {
                    obj2.CustomSetActive(false);
                }
                else
                {
                    obj2.CustomSetActive(true);
                }
            }
        }

        public int currentMapPlayerNum
        {
            [CompilerGenerated]
            get
            {
                return this.<currentMapPlayerNum>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                this.<currentMapPlayerNum>k__BackingField = value;
            }
        }

        public bool IsInMatching
        {
            get
            {
                return this.bInMatching;
            }
        }

        public bool IsInMatchingTeam
        {
            get
            {
                return this.bInMatchingTeam;
            }
        }

        public bool IsSelfTeamMaster
        {
            get
            {
                return ((this.teamInfo.stTeamMaster.ullUid == this.teamInfo.stSelfInfo.ullUid) && (this.teamInfo.stTeamMaster.iGameEntity == this.teamInfo.stSelfInfo.iGameEntity));
            }
        }
    }
}

