namespace Assets.Scripts.GameSystem
{
    using Apollo;
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

    public class CGuildInfoView : Singleton<CGuildInfoView>
    {
        [CompilerGenerated]
        private static Comparison<KeyValuePair<ulong, MemberRankInfo>> <>f__am$cache9;
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map1;
        public const string GuildApplyListCloseRedDotPrefKey = "Guild_ApplyList_CloseRedDot";
        public const string GuildApplyListPrefabPath = "UGUI/Form/System/Guild/Form_Guild_Apply_List.prefab";
        public const string GuildBuildingDetailPrefabPath = "UGUI/Form/System/Guild/Form_Guild_Building_Detail.prefab";
        public const string GuildDonatePrefabPath = "UGUI/Form/System/Guild/Form_Guild_Donate.prefab";
        public const string GuildExchangePositionPrefabPath = "UGUI/Form/System/Guild/Form_Guild_Exchange_Position.prefab";
        public const string GuildIconPrefabPath = "UGUI/Form/System/Guild/Form_Guild_Icon.prefab";
        public const string GuildInfoPrefabPath = "UGUI/Form/System/Guild/Form_Guild_Info.prefab";
        public const string GuildLogPrefabPath = "UGUI/Form/System/Guild/Form_Guild_Log.prefab";
        public const string GuildPreviewPrefabPath = "UGUI/Form/System/Guild/Form_Guild_Preview.prefab";
        public const string GuildRankPointRankPrefabPath = "UGUI/Form/System/Guild/Form_Guild_RankpointRank.prefab";
        private const int GuildRankpointRuleTextIndex = 9;
        public const string GuildSettingPrefabPath = "UGUI/Form/System/Guild/Form_Guild_Setting.prefab";
        public const string GuildSignSuccessPrefabPath = "UGUI/Form/System/Guild/Form_Guild_Sign_Success.prefab";
        public const string GuildSymbolFormPrefabPath = "UGUI/Form/System/Guild/Form_Guild_Symbol.prefab";
        private enGuildMemberListSortType m_curGuildMemberListSortType;
        private Tab m_curTab;
        private CUIFormScript m_form;
        private int m_lastSendReruitTime;
        private CGuildModel m_Model = Singleton<CGuildModel>.GetInstance();
        private static float s_rankpointRankListWidgetAnchoredPositionY;
        private static float s_rankpointRankListWidgetSizeDeltaY;
        private static Color s_Text_Color_Btn_Sign_Default = new Color(0.5882353f, 0.6784314f, 0.9960784f);

        public CGuildInfoView()
        {
            this.Init();
        }

        public void BindQQGroup()
        {
            IApolloSnsService iApolloSnsService = Utility.GetIApolloSnsService();
            if (iApolloSnsService != null)
            {
                string cUnionid = CGuildHelper.GetGroupGuildId().ToString();
                string guildName = CGuildHelper.GetGuildName();
                string cZoneid = CGuildHelper.GetGuildLogicWorldId().ToString();
                string bindQQGroupSignature = CGuildHelper.GetBindQQGroupSignature();
                iApolloSnsService.BindQQGroup(cUnionid, guildName, cZoneid, bindQQGroupSignature);
                if (this.m_form != null)
                {
                    GameObject widget = this.m_form.GetWidget(0x13);
                    if (widget != null)
                    {
                        widget.CustomSetActive(false);
                    }
                }
                Debug.Log("BindQQGroup: guild32BitsUid=" + cUnionid + "\nguildName=" + guildName + "\nguildZoneId=" + cZoneid + "\nguildSignatureMd5=" + bindQQGroupSignature);
            }
        }

        private uint CalculateRankNoOffset(enGuildRankpointRankListType rankListType, uint firstRankNo)
        {
            uint num = 0;
            if (rankListType == enGuildRankpointRankListType.SeasonSelf)
            {
                num = firstRankNo - 1;
            }
            return num;
        }

        private void ClearRankpointRankList(CUIFormScript rankForm)
        {
            rankForm.GetWidget(6).GetComponent<CUIListScript>().SetElementAmount(0);
        }

        public void CloseForm()
        {
            Singleton<CUIManager>.GetInstance().CloseForm("UGUI/Form/System/Guild/Form_Guild_Info.prefab");
        }

        public void DelGuildMatchBtnNewFlag()
        {
            if (this.m_form != null)
            {
                GameObject widget = this.m_form.GetWidget(20);
                Singleton<CUINewFlagSystem>.GetInstance().DelNewFlag(widget, enNewFlagKey.New_GuildMatch_V1, true);
            }
        }

        private void DisplayGuildMemberListSortBtn()
        {
            GameObject widget = this.m_form.GetWidget(0x22);
            GameObject obj3 = this.m_form.GetWidget(0x23);
            GameObject obj4 = this.m_form.GetWidget(0x24);
            GameObject obj5 = this.m_form.GetWidget(0x25);
            switch (this.m_curGuildMemberListSortType)
            {
                case enGuildMemberListSortType.Default:
                case enGuildMemberListSortType.PositionDesc:
                    widget.CustomSetActive(true);
                    obj3.CustomSetActive(false);
                    obj4.CustomSetActive(false);
                    obj5.CustomSetActive(false);
                    break;

                case enGuildMemberListSortType.WeekRankpointDesc:
                    widget.CustomSetActive(false);
                    obj3.CustomSetActive(true);
                    obj4.CustomSetActive(false);
                    obj5.CustomSetActive(false);
                    break;

                case enGuildMemberListSortType.SeasonRankpointDesc:
                    widget.CustomSetActive(false);
                    obj3.CustomSetActive(false);
                    obj4.CustomSetActive(true);
                    obj5.CustomSetActive(false);
                    break;

                case enGuildMemberListSortType.OnlineStatus:
                    widget.CustomSetActive(false);
                    obj3.CustomSetActive(false);
                    obj4.CustomSetActive(false);
                    obj5.CustomSetActive(true);
                    break;
            }
        }

        private uint GetGuildSendRecruitCd()
        {
            return GameDataMgr.guildMiscDatabin.GetDataByKey((uint) 0x34).dwConfValue;
        }

        private enGuildRankpointRankListType GetRankListTypeByTabSelectedIndex(CUIFormScript rankpointRankForm)
        {
            CUIListScript component = rankpointRankForm.GetWidget(5).GetComponent<CUIListScript>();
            CUIListScript script2 = rankpointRankForm.GetWidget(8).GetComponent<CUIListScript>();
            enGuildRankpointRankListType currentWeek = enGuildRankpointRankListType.CurrentWeek;
            if (component.GetSelectedIndex() == 0)
            {
                return enGuildRankpointRankListType.CurrentWeek;
            }
            if (component.GetSelectedIndex() == 1)
            {
                return enGuildRankpointRankListType.LastWeek;
            }
            if (component.GetSelectedIndex() == 2)
            {
                if (script2.GetSelectedIndex() == 0)
                {
                    return enGuildRankpointRankListType.SeasonSelf;
                }
                if (script2.GetSelectedIndex() == 1)
                {
                    currentWeek = enGuildRankpointRankListType.SeasonBest;
                }
            }
            return currentWeek;
        }

        private GuildMemInfo GetSelectedMemberInfo()
        {
            if (this.m_form != null)
            {
                if (this.CurTab == Tab.GuildInfo)
                {
                    int selectedIndex = this.m_form.GetWidget(0x17).GetComponent<CUIListScript>().GetSelectedIndex();
                    KeyValuePair<ulong, MemberRankInfo> pair = this.m_Model.RankpointMemberInfoList[selectedIndex];
                    return CGuildHelper.GetGuildMemberInfoByUid(pair.Key);
                }
                if (this.CurTab == Tab.GuildMember)
                {
                    int num2 = this.m_form.GetWidget(9).GetComponent<CUIListScript>().GetSelectedIndex();
                    return this.m_Model.CurrentGuildInfo.listMemInfo[num2];
                }
            }
            return null;
        }

        private int GetShowGuildIconCount()
        {
            if (Singleton<CGuildSystem>.GetInstance().IsInNormalGuild())
            {
                uint dwLastSeasonRankNo = this.m_Model.CurrentGuildInfo.GuildMatchInfo.dwLastSeasonRankNo;
                if ((0 < dwLastSeasonRankNo) && (dwLastSeasonRankNo <= 3))
                {
                    return ((GameDataMgr.guildIconDatabin.count - 3) + 1);
                }
            }
            return (GameDataMgr.guildIconDatabin.count - 3);
        }

        public override void Init()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Info_View_Change_Tab, new CUIEventManager.OnUIEventHandler(this.On_Tab_Change));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Guild_Member_List_Element_Enabled, new CUIEventManager.OnUIEventHandler(this.On_Guild_Member_List_Element_Enabled));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Member_Select, new CUIEventManager.OnUIEventHandler(this.On_Guild_Member_Select));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Add_Friend, new CUIEventManager.OnUIEventHandler(this.On_Guild_Add_Friend));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Donate, new CUIEventManager.OnUIEventHandler(this.On_Guild_Donate));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Donate_Confirm, new CUIEventManager.OnUIEventHandler(this.On_Guild_Donate_Confirm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Get_Guild_Dividend, new CUIEventManager.OnUIEventHandler(this.On_Guild_Get_Guild_Dividend));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Guild_Setting_Open, new CUIEventManager.OnUIEventHandler(this.On_Guild_Guild_Setting_Open));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Guild_Setting_Confirm, new CUIEventManager.OnUIEventHandler(this.On_Guild_Guild_Setting_Confirm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Guild_Setting_Open_Icon_Form, new CUIEventManager.OnUIEventHandler(this.On_Guild_Guild_Setting_Open_Icon_Form));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Guild_Setting_Guild_Icon_Selected, new CUIEventManager.OnUIEventHandler(this.On_Guild_Guild_Setting_Guild_Icon_Selected));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Open_Modify_Guild_Bulletin_Form, new CUIEventManager.OnUIEventHandler(this.On_Guild_Open_Modify_Guild_Bulletin_Form));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Confirm_Modify_Guild_Bulletin, new CUIEventManager.OnUIEventHandler(this.On_Guild_Confirm_Modify_Guild_Bulletin));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Extend_Member_Limit, new CUIEventManager.OnUIEventHandler(this.On_Guild_Extend_Member_Limit));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Extend_Member_Limit_Confirm, new CUIEventManager.OnUIEventHandler(this.On_Guild_Extend_Member_Limit_Confirm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Setting_Down_Join_Level_Num, new CUIEventManager.OnUIEventHandler(this.On_Guild_Setting_Down_Join_Level_Num));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Setting_Up_Join_Level_Num, new CUIEventManager.OnUIEventHandler(this.On_Guild_Setting_Up_Join_Level_Num));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Setting_Down_Join_Grade, new CUIEventManager.OnUIEventHandler(this.On_Guild_Setting_Down_Join_Grade));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Setting_Up_Join_Grade, new CUIEventManager.OnUIEventHandler(this.On_Guild_Setting_Up_Join_Grade));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Open_Apply_List_Form, new CUIEventManager.OnUIEventHandler(this.On_Guild_Open_Apply_List_Form));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Guild_Application_Pass, new CUIEventManager.OnUIEventHandler(this.On_Guild_Application_Pass));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Guild_Application_Reject, new CUIEventManager.OnUIEventHandler(this.On_Guild_Application_Reject));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_ApplyList_Save_Pref, new CUIEventManager.OnUIEventHandler(this.On_Guild_ApplyList_Save_Pref));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Apply_List_Element_Head_Selected, new CUIEventManager.OnUIEventHandler(this.On_Guild_Apply_List_Element_Head_Selected));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Apply_List_Element_Enabled, new CUIEventManager.OnUIEventHandler(this.On_Guild_Apply_List_Element_Enabled));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Need_Approval_Slider_Value_Changed, new CUIEventManager.OnUIEventHandler(this.On_Guild_Need_Approval_Slider_Value_Changed));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Recommend_Invite, new CUIEventManager.OnUIEventHandler(this.On_Guild_Recommend_Invite));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Recommend_Reject, new CUIEventManager.OnUIEventHandler(this.On_Guild_Recommend_Reject));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Guild_Apply_Quit, new CUIEventManager.OnUIEventHandler(this.On_Guild_Guild_Apply_Quit));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Guild_Apply_Quit_Confirm, new CUIEventManager.OnUIEventHandler(this.On_Guild_Guild_Apply_Quit_Confirm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Guild_Apply_Time_Up, new CUIEventManager.OnUIEventHandler(this.On_Guild_Guild_Apply_Time_Up));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Position_Appoint_Vice_Chairman, new CUIEventManager.OnUIEventHandler(this.OnGuildPositionAppointViceChairman));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Position_Appoint_Vice_Chairman_Confirm, new CUIEventManager.OnUIEventHandler(this.OnGuildPositionAppointViceChairmanConfirm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Position_Fire_Member, new CUIEventManager.OnUIEventHandler(this.OnGuildPositionFireMember));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Position_Fire_Member_Confirm, new CUIEventManager.OnUIEventHandler(this.OnGuildPositionFireMemberConfirm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Position_Chairman_Transfer, new CUIEventManager.OnUIEventHandler(this.OnGuildPositionChairmanTransfer));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Position_Chairman_Transfer_Confirm, new CUIEventManager.OnUIEventHandler(this.OnGuildPositionChairmanTransferConfirm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Position_Recommend_Self_As_Chairman, new CUIEventManager.OnUIEventHandler(this.OnGuildPositionRecommendSelfAsChairman));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Position_Recommend_Self_As_Chairman_Confirm, new CUIEventManager.OnUIEventHandler(this.OnGuildPositionRecommendSelfAsChairmanConfirm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Position_Agree_Self_Recommend, new CUIEventManager.OnUIEventHandler(this.OnGuildPositionAgreeSelfRecommend));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Position_Disagree_Self_Recommend, new CUIEventManager.OnUIEventHandler(this.OnGuildPositionDisagreeSelfRecommend));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Open_Transfer_Chairman_Without_Secure_Pwd_Confirm, new CUIEventManager.OnUIEventHandler(this.On_Guild_Open_Transfer_Chairman_Without_Secure_Pwd_Confirm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Real_Transfer_Chairman, new CUIEventManager.OnUIEventHandler(this.On_Guild_Real_Transfer_Chairman));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Rankpoint_Help, new CUIEventManager.OnUIEventHandler(this.On_Guild_Rankpoint_Help));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Rankpoint_Enter_Matching, new CUIEventManager.OnUIEventHandler(this.On_Guild_Rankpoint_Enter_Matching));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Rankpoint_Rank_List_Tab_Change, new CUIEventManager.OnUIEventHandler(this.On_Guild_Rankpoint_Rank_List_Tab_Change));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Rankpoint_Season_Rank_List_Tab_Change, new CUIEventManager.OnUIEventHandler(this.On_Guild_Rankpoint_Season_Rank_List_Tab_Change));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Rankpoint_Open_Rankpoint_Rank_Form, new CUIEventManager.OnUIEventHandler(this.On_Guild_Rankpoint_Open_Rankpoint_Rank_Form));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Rankpoint_Member_List_Element_Enabled, new CUIEventManager.OnUIEventHandler(this.On_Guild_Rankpoint_Member_List_Element_Enabled));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Rankpoint_Rank_List_Element_Enabled, new CUIEventManager.OnUIEventHandler(this.On_Guild_Rankpoint_Rank_List_Element_Enabled));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Preview_Request_Ranking_List, new CUIEventManager.OnUIEventHandler(this.On_Guild_Preview_Request_Ranking_List));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Preview_Guild_List_Element_Select, new CUIEventManager.OnUIEventHandler(this.On_Guild_Preview_Guild_List_Element_Select));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Preview_Guild_List_Element_Enabled, new CUIEventManager.OnUIEventHandler(this.On_Guild_Preview_Guild_List_Element_Enabled));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Preview_Request_More_Guild_List, new CUIEventManager.OnUIEventHandler(this.On_Guild_Preview_Request_More_Guild_List));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Guild_Search_In_Preview_Panel, new CUIEventManager.OnUIEventHandler(this.On_Guild_Guild_Search));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Accept_Invite, new CUIEventManager.OnUIEventHandler(this.On_Guild_Accept_Invite));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Sign, new CUIEventManager.OnUIEventHandler(this.On_Guild_Sign));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Open_Log_Form, new CUIEventManager.OnUIEventHandler(this.On_Guild_Open_Log_Form));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Member_List_Sort_Position_Desc, new CUIEventManager.OnUIEventHandler(this.On_Guild_Member_List_Sort_Position_Desc));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Member_List_Sort_Week_Rankpoint_Desc, new CUIEventManager.OnUIEventHandler(this.On_Guild_Member_List_Sort_Week_Rankpoint_Desc));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Member_List_Sort_Season_Rankpoint_Desc, new CUIEventManager.OnUIEventHandler(this.On_Guild_Member_List_Sort_Season_Rankpoint_Desc));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Member_List_Sort_Online_Status_Desc, new CUIEventManager.OnUIEventHandler(this.On_Guild_Member_List_Sort_Online_Status_Desc));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Recruit_Send_Recruit, new CUIEventManager.OnUIEventHandler(this.On_Guild_Recruit_Send_Recruit));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Recruit_Send_Recruit_CD_Timeout, new CUIEventManager.OnUIEventHandler(this.On_Guild_Recruit_Send_Recruit_CD_Timeout));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_BindQQGroup, new CUIEventManager.OnUIEventHandler(this.On_Guild_BindQQGroup));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_UnBindQQGroup, new CUIEventManager.OnUIEventHandler(this.On_Guild_UnBindQQGroup));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_JoinQQGroup, new CUIEventManager.OnUIEventHandler(this.On_Guild_JoinQQGroup));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_UnBindQQGroupConfirm, new CUIEventManager.OnUIEventHandler(this.On_Guild_UnBindQQGroupConfirm));
            IApolloSnsService iApolloSnsService = Utility.GetIApolloSnsService();
            if (iApolloSnsService != null)
            {
                iApolloSnsService.onBindGroupEvent += new OnBindGroupNotifyHandle(this.OnBindQQGroupNotify);
                iApolloSnsService.onQueryGroupInfoEvent += new OnQueryGroupInfoNotifyHandle(this.OnQueryQQGroupInfoNotify);
                iApolloSnsService.onUnBindGroupEvent += new OnUnbindGroupNotifyHandle(this.OnUnBindQQGroupNotify);
                iApolloSnsService.onQueryGroupKeyEvent += new OnQueryGroupKeyNotifyHandle(this.OnQueryQQGroupKeyNotify);
            }
        }

        public void InitPanel()
        {
            this.m_form.GetWidget(0).CustomSetActive(false);
            this.m_form.GetWidget(1).CustomSetActive(false);
            switch (this.CurTab)
            {
                case Tab.GuildInfo:
                    this.RefreshInfoPanel();
                    this.OpenAbdicateConfirm();
                    break;

                case Tab.GuildMember:
                    this.m_curGuildMemberListSortType = enGuildMemberListSortType.Default;
                    this.RefreshMemberPanel();
                    return;
            }
        }

        private void InitRankpointMemberList()
        {
            int count = this.m_Model.CurrentGuildInfo.listMemInfo.Count;
            this.m_Model.RankpointMemberInfoList.Clear();
            for (int i = 0; i < count; i++)
            {
                this.m_Model.RankpointMemberInfoList.Add(new KeyValuePair<ulong, MemberRankInfo>(this.m_Model.CurrentGuildInfo.listMemInfo[i].stBriefInfo.uulUid, this.m_Model.CurrentGuildInfo.listMemInfo[i].RankInfo));
            }
            if (<>f__am$cache9 == null)
            {
                <>f__am$cache9 = delegate (KeyValuePair<ulong, MemberRankInfo> info1, KeyValuePair<ulong, MemberRankInfo> info2) {
                    MemberRankInfo info = info1.Value;
                    MemberRankInfo info3 = info2.Value;
                    if (info.totalRankPoint == info3.totalRankPoint)
                    {
                        return -info.weekRankPoint.CompareTo(info3.weekRankPoint);
                    }
                    return -info.totalRankPoint.CompareTo(info3.totalRankPoint);
                };
            }
            this.m_Model.RankpointMemberInfoList.Sort(<>f__am$cache9);
        }

        private void InitRankpointRankTab(CUIFormScript rankpointRankForm = new CUIFormScript())
        {
            if (rankpointRankForm == null)
            {
                rankpointRankForm = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Guild/Form_Guild_RankpointRank.prefab");
            }
            string[] strArray = new string[] { Singleton<CTextManager>.GetInstance().GetText("Guild_Rankpoint_This_Week_Rank"), Singleton<CTextManager>.GetInstance().GetText("Guild_Rankpoint_Last_Week_Rank"), Singleton<CTextManager>.GetInstance().GetText("Guild_Rankpoint_Season_Rank") };
            CUIListScript component = rankpointRankForm.GetWidget(5).GetComponent<CUIListScript>();
            component.SetElementAmount(strArray.Length);
            for (int i = 0; i < strArray.Length; i++)
            {
                component.GetElemenet(i).get_transform().Find("Text").GetComponent<Text>().set_text(strArray[i]);
            }
            component.SelectElement(0, true);
        }

        private void InitRankpointSeasonRankTab(CUIFormScript rankpointRankForm = new CUIFormScript())
        {
            if (rankpointRankForm == null)
            {
                rankpointRankForm = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Guild/Form_Guild_RankpointRank.prefab");
            }
            string[] strArray = new string[] { Singleton<CTextManager>.GetInstance().GetText("Guild_Rankpoint_Current_Grade_Rank"), Singleton<CTextManager>.GetInstance().GetText("Guild_Rankpoint_Best_Rank") };
            GameObject widget = rankpointRankForm.GetWidget(8);
            widget.CustomSetActive(false);
            CUIListScript component = widget.GetComponent<CUIListScript>();
            component.SetElementAmount(strArray.Length);
            for (int i = 0; i < strArray.Length; i++)
            {
                component.GetElemenet(i).get_transform().Find("Text").GetComponent<Text>().set_text(strArray[i]);
            }
        }

        public void InitTab()
        {
            string[] strArray = new string[] { Singleton<CTextManager>.GetInstance().GetText("Guild_Info_Tab_Info"), Singleton<CTextManager>.GetInstance().GetText("Guild_Info_Tab_Member") };
            CUIListScript component = this.m_form.GetWidget(2).GetComponent<CUIListScript>();
            component.SetElementAmount(strArray.Length);
            for (int i = 0; i < component.m_elementAmount; i++)
            {
                component.GetElemenet(i).get_gameObject().get_transform().Find("Text").GetComponent<Text>().set_text(strArray[i]);
            }
            component.m_alwaysDispatchSelectedChangeEvent = true;
            component.SelectElement(0, true);
        }

        private bool IsApplyListCloseRedDotPrefOn()
        {
            return (PlayerPrefs.GetInt("Guild_ApplyList_CloseRedDot") > 0);
        }

        public bool IsApplyListFormShow()
        {
            return (Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Guild/Form_Guild_Apply_List.prefab") != null);
        }

        public bool IsRankpointRankFormShow()
        {
            return (Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Guild/Form_Guild_RankpointRank.prefab") != null);
        }

        public bool IsShow()
        {
            return (Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Guild/Form_Guild_Info.prefab") != null);
        }

        private bool IsShowApplyRedDot()
        {
            return ((CGuildSystem.HasManageAuthority() && !this.IsApplyListCloseRedDotPrefOn()) && !CGuildSystem.s_isApplyAndRecommendListEmpty);
        }

        private void On_Guild_Accept_Invite(CUIEvent uiEvent)
        {
            Singleton<EventRouter>.GetInstance().BroadCastEvent("Guild_Accept_Invite");
        }

        private void On_Guild_Add_Friend(CUIEvent uiEvent)
        {
            if (this.m_Model.CurrentSelectedMemberInfo != null)
            {
                Singleton<CFriendContoller>.instance.Open_Friend_Verify(this.m_Model.CurrentSelectedMemberInfo.stBriefInfo.uulUid, (uint) this.m_Model.CurrentSelectedMemberInfo.stBriefInfo.dwLogicWorldId, false, COM_ADD_FRIEND_TYPE.COM_ADD_FRIEND_NULL, -1, true);
            }
        }

        private void On_Guild_Application_Pass(CUIEvent uiEvent)
        {
            if (CGuildHelper.IsGuildMemberFull())
            {
                Singleton<CUIManager>.GetInstance().OpenTips("Guild_Our_Member_Full", true, 1.5f, null, new object[0]);
            }
            else
            {
                CUIListElementScript elemenet = uiEvent.m_srcWidgetBelongedListScript.GetComponent<CUIListScript>().GetElemenet(uiEvent.m_srcWidgetIndexInBelongedList);
                if (elemenet != null)
                {
                    ulong num;
                    Text component = elemenet.get_transform().Find("txtUidData").GetComponent<Text>();
                    if (ulong.TryParse(component.get_text(), out num))
                    {
                        Singleton<EventRouter>.GetInstance().BroadCastEvent<ulong, byte>("Guild_Approve", num, 1);
                    }
                    else
                    {
                        object[] inParameters = new object[] { component.get_text() };
                        DebugHelper.Assert(false, "CGuildInfoView.On_Guild_Application_Pass(): txtUid.text={0}", inParameters);
                    }
                }
            }
        }

        private void On_Guild_Application_Reject(CUIEvent uiEvent)
        {
            CUIListElementScript elemenet = uiEvent.m_srcWidgetBelongedListScript.GetComponent<CUIListScript>().GetElemenet(uiEvent.m_srcWidgetIndexInBelongedList);
            if (elemenet != null)
            {
                ulong num;
                Text component = elemenet.get_transform().Find("txtUidData").GetComponent<Text>();
                if (ulong.TryParse(component.get_text(), out num))
                {
                    Singleton<EventRouter>.GetInstance().BroadCastEvent<ulong, byte>("Guild_Approve", num, 0);
                }
                else
                {
                    object[] inParameters = new object[] { component.get_text() };
                    DebugHelper.Assert(false, "CGuildInfoView.On_Guild_Application_Reject(): txtUid.text={0}", inParameters);
                }
            }
        }

        private void On_Guild_Apply_List_Element_Enabled(CUIEvent uiEvent)
        {
            if (uiEvent.m_srcWidgetIndexInBelongedList < this.m_Model.GetRecommendInfoCount())
            {
                List<stRecommendInfo> recommendInfo = this.m_Model.GetRecommendInfo();
                CUIListElementScript srcWidgetScript = uiEvent.m_srcWidgetScript as CUIListElementScript;
                bool isAdmin = CGuildSystem.HasManageAuthority();
                if ((recommendInfo != null) && (srcWidgetScript != null))
                {
                    this.SetRecommendItem(srcWidgetScript, recommendInfo[uiEvent.m_srcWidgetIndexInBelongedList], isAdmin);
                }
            }
            else
            {
                List<stApplicantInfo> applicants = this.m_Model.GetApplicants();
                CUIListElementScript listElementScript = uiEvent.m_srcWidgetScript as CUIListElementScript;
                bool flag2 = CGuildSystem.HasManageAuthority();
                int num = uiEvent.m_srcWidgetIndexInBelongedList - this.m_Model.GetRecommendInfoCount();
                if (((applicants != null) && (listElementScript != null)) && (num < this.m_Model.GetApplicantsCount()))
                {
                    this.SetApplicantItem(listElementScript, applicants[num], flag2);
                }
            }
        }

        private void On_Guild_Apply_List_Element_Head_Selected(CUIEvent uiEvent)
        {
            if (uiEvent.m_srcWidgetIndexInBelongedList < this.m_Model.GetRecommendInfoCount())
            {
                List<stRecommendInfo> recommendInfo = this.m_Model.GetRecommendInfo();
                if (recommendInfo != null)
                {
                    stRecommendInfo info = recommendInfo[uiEvent.m_srcWidgetIndexInBelongedList];
                    Singleton<CPlayerInfoSystem>.GetInstance().ShowPlayerDetailInfo(info.uid, info.logicWorldID, CPlayerInfoSystem.DetailPlayerInfoSource.DefaultOthers, true);
                }
            }
            else
            {
                List<stApplicantInfo> applicants = this.m_Model.GetApplicants();
                if (applicants != null)
                {
                    stApplicantInfo info2 = applicants[uiEvent.m_srcWidgetIndexInBelongedList - this.m_Model.GetRecommendInfoCount()];
                    Singleton<CPlayerInfoSystem>.GetInstance().ShowPlayerDetailInfo(info2.stBriefInfo.uulUid, info2.stBriefInfo.dwLogicWorldId, CPlayerInfoSystem.DetailPlayerInfoSource.DefaultOthers, true);
                }
            }
        }

        private void On_Guild_ApplyList_Save_Pref(CUIEvent uiEvent)
        {
            CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
            if (srcFormScript != null)
            {
                Toggle component = srcFormScript.GetWidget(1).GetComponent<Toggle>();
                PlayerPrefs.SetInt("Guild_ApplyList_CloseRedDot", !component.get_isOn() ? 0 : 1);
                this.RefreshApplyRedDot();
            }
        }

        private void On_Guild_BindQQGroup(CUIEvent uiEvent)
        {
            Singleton<EventRouter>.GetInstance().BroadCastEvent("Guild_QQGroup_Request_Group_Guild_Id");
        }

        private void On_Guild_Confirm_Modify_Guild_Bulletin(CUIEvent uiEvent)
        {
            CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
            if (srcFormScript != null)
            {
                string str = CUIUtility.RemoveEmoji(srcFormScript.GetWidget(1).GetComponent<Text>().get_text()).Trim();
                if (string.IsNullOrEmpty(str))
                {
                    Singleton<CUIManager>.GetInstance().OpenTips("Guild_Input_Guild_Bulletin_Empty", true, 1.5f, null, new object[0]);
                }
                else
                {
                    Singleton<EventRouter>.GetInstance().BroadCastEvent<string>("Guild_Modify_Bulletin", str);
                }
            }
        }

        private void On_Guild_Donate(CUIEvent uiEvent)
        {
            this.OpenDonateForm();
        }

        private void On_Guild_Donate_Confirm(CUIEvent uiEvent)
        {
            RES_GUILD_DONATE_TYPE donateType = (RES_GUILD_DONATE_TYPE) 0;
            string key = uiEvent.m_srcWidget.get_gameObject().get_transform().get_parent().get_gameObject().get_name();
            if (key != null)
            {
                int num;
                if (<>f__switch$map1 == null)
                {
                    Dictionary<string, int> dictionary = new Dictionary<string, int>(4);
                    dictionary.Add("pnlPrimary", 0);
                    dictionary.Add("pnlIntermediate", 1);
                    dictionary.Add("pnlAdvanced", 2);
                    dictionary.Add("pnlKing", 3);
                    <>f__switch$map1 = dictionary;
                }
                if (<>f__switch$map1.TryGetValue(key, out num))
                {
                    switch (num)
                    {
                        case 0:
                            donateType = RES_GUILD_DONATE_TYPE.RES_GUILD_DONATE_TYPE_LOW;
                            goto Label_00C2;

                        case 1:
                            donateType = RES_GUILD_DONATE_TYPE.RES_GUILD_DONATE_TYPE_MID;
                            goto Label_00C2;

                        case 2:
                            donateType = RES_GUILD_DONATE_TYPE.RES_GUILD_DONATE_TYPE_HIGH;
                            goto Label_00C2;

                        case 3:
                            donateType = RES_GUILD_DONATE_TYPE.RES_GUILD_DONATE_TYPE_TOP;
                            goto Label_00C2;
                    }
                }
            }
            donateType = RES_GUILD_DONATE_TYPE.RES_GUILD_DONATE_TYPE_LOW;
        Label_00C2:
            this.m_Model.CurrentDonateType = donateType;
            if (CGuildHelper.IsDonateUseCoin(donateType))
            {
                if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GoldCoin < CGuildHelper.GetDonateCostCoin(donateType))
                {
                    CUICommonSystem.OpenGoldCoinNotEnoughTip();
                    return;
                }
            }
            else if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().DianQuan < CGuildHelper.GetDonateCostDianQuan(donateType))
            {
                CUICommonSystem.OpenDianQuanNotEnoughTip();
                return;
            }
            Singleton<EventRouter>.GetInstance().BroadCastEvent<RES_GUILD_DONATE_TYPE>("Request_Guild_Donate", donateType);
        }

        private void On_Guild_Extend_Member_Limit(CUIEvent uiEvent)
        {
            Singleton<BeaconHelper>.GetInstance().m_curBuyDianInfo.buy_dia_channel = "4";
            Singleton<BeaconHelper>.GetInstance().m_curBuyDianInfo.call_back_time = Time.get_time();
            Singleton<BeaconHelper>.GetInstance().m_curBuyPropInfo.buy_prop_channel = "4";
            Singleton<BeaconHelper>.GetInstance().m_curBuyPropInfo.buy_prop_id_time = Time.get_time();
            int bLevel = this.m_Model.CurrentGuildInfo.briefInfo.bLevel;
            if (CGuildHelper.IsGuildMaxLevel(bLevel))
            {
                Singleton<CUIManager>.GetInstance().OpenTips("Guild_Member_Count_Reach_Limit", true, 1.5f, null, new object[0]);
            }
            else
            {
                int upgradeCostDianQuanByLevel = CGuildHelper.GetUpgradeCostDianQuanByLevel(bLevel);
                int maxGuildMemberCountByLevel = CGuildHelper.GetMaxGuildMemberCountByLevel(bLevel + 1);
                string[] args = new string[] { upgradeCostDianQuanByLevel.ToString(), maxGuildMemberCountByLevel.ToString() };
                string text = Singleton<CTextManager>.GetInstance().GetText("Guild_Extend_Member_Limit_Confirm_Msg", args);
                Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(text, enUIEventID.Guild_Extend_Member_Limit_Confirm, enUIEventID.None, false);
            }
        }

        private void On_Guild_Extend_Member_Limit_Confirm(CUIEvent uiEvent)
        {
            int upgradeCostDianQuanByLevel = CGuildHelper.GetUpgradeCostDianQuanByLevel(this.m_Model.CurrentGuildInfo.briefInfo.bLevel);
            if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().DianQuan < upgradeCostDianQuanByLevel)
            {
                CUICommonSystem.OpenDianQuanNotEnoughTip();
            }
            else
            {
                Singleton<EventRouter>.GetInstance().BroadCastEvent("Guild_Request_Extend_Member_Limit");
            }
        }

        private void On_Guild_Get_Guild_Dividend(CUIEvent uiEvent)
        {
            if (this.m_Model.GetPlayerGuildMemberInfo().WeekDividend == 0)
            {
                Singleton<CUIManager>.GetInstance().OpenTips("Guild_No_Dividend_Can_Get", true, 1.5f, null, new object[0]);
            }
            else
            {
                Singleton<EventRouter>.GetInstance().BroadCastEvent("Guild_Get_Guild_Dividend");
            }
        }

        private void On_Guild_Guild_Apply_Quit(CUIEvent uiEvent)
        {
            if (((Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.guildState == COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_CHAIRMAN) && (this.m_Model.CurrentGuildInfo.briefInfo.bMemberNum > 1)) && CGuildHelper.IsSelfInGuildMemberList())
            {
                Singleton<CUIManager>.GetInstance().OpenTips("Guild_ChairMan_Quit_Tip", true, 1.5f, null, new object[0]);
            }
            else
            {
                string text;
                uint dwConfValue = GameDataMgr.guildMiscDatabin.GetDataByKey((uint) 7).dwConfValue;
                TimeSpan span = new TimeSpan(0, 0, 0, (int) dwConfValue);
                GuildMemInfo playerGuildMemberInfo = this.m_Model.GetPlayerGuildMemberInfo();
                uint num2 = (playerGuildMemberInfo != null) ? ((playerGuildMemberInfo.dwConstruct * GameDataMgr.guildMiscDatabin.GetDataByKey((uint) 20).dwConfValue) / 100) : 0;
                uint num3 = (playerGuildMemberInfo != null) ? (playerGuildMemberInfo.dwConstruct - num2) : 0;
                if ((Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.guildState == COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_CHAIRMAN) && CGuildHelper.IsSelfInGuildMemberList())
                {
                    text = Singleton<CTextManager>.GetInstance().GetText("Guild_Chairman_Quit_Confirm");
                }
                else
                {
                    string[] args = new string[] { span.TotalHours.ToString(), num2.ToString(), num3.ToString() };
                    text = Singleton<CTextManager>.GetInstance().GetText("Guild_Quit_Confirm", args);
                }
                Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(text, enUIEventID.Guild_Guild_Apply_Quit_Confirm, enUIEventID.Guild_Guild_Apply_Quit_Cancel, false);
            }
        }

        private void On_Guild_Guild_Apply_Quit_Confirm(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
            Singleton<EventRouter>.GetInstance().BroadCastEvent("Guild_Quit");
        }

        private void On_Guild_Guild_Apply_Time_Up(CUIEvent uiEvent)
        {
            CUIListElementScript elemenet = uiEvent.m_srcWidgetBelongedListScript.GetComponent<CUIListScript>().GetElemenet(uiEvent.m_srcWidgetIndexInBelongedList);
            if (elemenet != null)
            {
                ulong num;
                Text component = elemenet.get_transform().Find("txtUidData").GetComponent<Text>();
                if (ulong.TryParse(component.get_text(), out num))
                {
                    elemenet.get_gameObject().CustomSetActive(false);
                    Singleton<EventRouter>.GetInstance().BroadCastEvent<ulong>("Guild_Apply_Time_Up", num);
                }
                else
                {
                    object[] inParameters = new object[] { component.get_text() };
                    DebugHelper.Assert(false, "CGuildInfoView.On_Guild_Guild_Apply_Time_Up(): txtUid.text={0}", inParameters);
                }
            }
        }

        private void On_Guild_Guild_Search(CUIEvent uiEvent)
        {
            CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
            if (srcFormScript != null)
            {
                string str = srcFormScript.GetWidget(4).GetComponent<InputField>().get_text();
                if (!string.IsNullOrEmpty(str))
                {
                    Singleton<CGuildSystem>.GetInstance().SearchGuild(0L, str, 0, false);
                }
            }
        }

        private void On_Guild_Guild_Setting_Confirm(CUIEvent uiEvent)
        {
            CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
            if (srcFormScript != null)
            {
                int num4;
                int num5;
                Text component = srcFormScript.GetWidget(3).GetComponent<Text>();
                uint num = 0;
                if (component != null)
                {
                    try
                    {
                        num = Convert.ToUInt32(component.get_text());
                    }
                    catch (Exception exception)
                    {
                        object[] inParameters = new object[] { component.get_text(), exception.Message };
                        DebugHelper.Assert(false, "Failed convert form {0} to uint32, On_Guild_Guild_Setting_Confirm, Exception={1}", inParameters);
                    }
                }
                Singleton<EventRouter>.GetInstance().BroadCastEvent<uint>("Guild_Setting_Modify_Icon", num);
                uint num2 = Convert.ToUInt32(((int) srcFormScript.GetWidget(1).GetComponent<Slider>().get_value()) == 0);
                uint maxValue = uint.MaxValue;
                maxValue &= num2;
                Text text2 = srcFormScript.GetWidget(6).GetComponent<Text>();
                Text text3 = srcFormScript.GetWidget(7).GetComponent<Text>();
                if (!int.TryParse(text2.get_text(), out num4))
                {
                    object[] objArray2 = new object[] { text2.get_text() };
                    DebugHelper.Assert(false, "等级限制文本不是一个数字：{0}", objArray2);
                }
                if (!int.TryParse(text3.get_text(), out num5))
                {
                    object[] objArray3 = new object[] { text3.get_text() };
                    DebugHelper.Assert(false, "段位限制文本不是一个数字：{0}", objArray3);
                }
                Singleton<EventRouter>.GetInstance().BroadCastEvent<uint, int, int>("Request_Guild_Setting_Modify", maxValue, num4, num5);
            }
        }

        private void On_Guild_Guild_Setting_Guild_Icon_Selected(CUIEvent uiEvent)
        {
            CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
            if (srcFormScript != null)
            {
                CUIListScript srcWidgetScript = uiEvent.m_srcWidgetScript as CUIListScript;
                if (srcWidgetScript != null)
                {
                    int selectedIndex = srcWidgetScript.GetSelectedIndex();
                    if (selectedIndex == -1)
                    {
                        selectedIndex = 0;
                    }
                    CUIListElementScript elemenet = srcWidgetScript.GetElemenet(selectedIndex);
                    if (elemenet != null)
                    {
                        Text component = elemenet.get_transform().Find("imgIcon/txtIconIdData").GetComponent<Text>();
                        CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Guild/Form_Guild_Setting.prefab");
                        if (form != null)
                        {
                            form.GetWidget(3).GetComponent<Text>().set_text(component.get_text());
                            form.GetWidget(0).GetComponent<Image>().SetSprite(elemenet.get_transform().Find("imgIcon").GetComponent<Image>());
                        }
                        else
                        {
                            DebugHelper.Assert(false, "CGuildInfoView.On_Guild_Guild_Setting_Guild_Icon_Selected(): settingForm is null!!!");
                        }
                        srcFormScript.Close();
                    }
                }
            }
        }

        private void On_Guild_Guild_Setting_Open(CUIEvent uiEvent)
        {
            if (CGuildSystem.HasManageAuthority())
            {
                this.OpenSettingForm();
            }
        }

        private void On_Guild_Guild_Setting_Open_Icon_Form(CUIEvent uiEvent)
        {
            this.OpenGuildIconForm();
        }

        private void On_Guild_JoinQQGroup(CUIEvent uiEvent)
        {
            if (MonoSingleton<ShareSys>.GetInstance().IsInstallPlatform())
            {
                IApolloSnsService iApolloSnsService = Utility.GetIApolloSnsService();
                if (iApolloSnsService != null)
                {
                    if (!string.IsNullOrEmpty(this.m_Model.CurrentGuildInfo.groupKey))
                    {
                        iApolloSnsService.JoinQQGroup(this.m_Model.CurrentGuildInfo.groupKey);
                        Debug.Log("BindQQGroup: groupKey=" + this.m_Model.CurrentGuildInfo.groupKey);
                    }
                    else
                    {
                        Debug.Log("BindQQGroup: group key is null!!!");
                    }
                }
            }
        }

        private void On_Guild_Member_List_Element_Enabled(CUIEvent uiEvent)
        {
            if ((0 <= uiEvent.m_srcWidgetIndexInBelongedList) && (uiEvent.m_srcWidgetIndexInBelongedList < this.m_Model.CurrentGuildInfo.listMemInfo.Count))
            {
                GuildMemInfo memberInfo = this.m_Model.CurrentGuildInfo.listMemInfo[uiEvent.m_srcWidgetIndexInBelongedList];
                if (memberInfo != null)
                {
                    this.SetGuildMemberListItem(uiEvent.m_srcWidgetScript as CUIListElementScript, memberInfo);
                }
            }
        }

        private void On_Guild_Member_List_Sort_Online_Status_Desc(CUIEvent uiEvent)
        {
            if (this.m_curGuildMemberListSortType != enGuildMemberListSortType.OnlineStatus)
            {
                this.m_curGuildMemberListSortType = enGuildMemberListSortType.OnlineStatus;
                this.RefreshMemberList();
            }
        }

        private void On_Guild_Member_List_Sort_Position_Desc(CUIEvent uiEvent)
        {
            if (this.m_curGuildMemberListSortType != enGuildMemberListSortType.PositionDesc)
            {
                this.m_curGuildMemberListSortType = enGuildMemberListSortType.PositionDesc;
                this.RefreshMemberList();
            }
        }

        private void On_Guild_Member_List_Sort_Season_Rankpoint_Desc(CUIEvent uiEvent)
        {
            if (this.m_curGuildMemberListSortType != enGuildMemberListSortType.SeasonRankpointDesc)
            {
                this.m_curGuildMemberListSortType = enGuildMemberListSortType.SeasonRankpointDesc;
                this.RefreshMemberList();
            }
        }

        private void On_Guild_Member_List_Sort_Week_Rankpoint_Desc(CUIEvent uiEvent)
        {
            if (this.m_curGuildMemberListSortType != enGuildMemberListSortType.WeekRankpointDesc)
            {
                this.m_curGuildMemberListSortType = enGuildMemberListSortType.WeekRankpointDesc;
                this.RefreshMemberList();
            }
        }

        private void On_Guild_Member_Select(CUIEvent uiEvent)
        {
            this.m_Model.CurrentSelectedMemberInfo = this.GetSelectedMemberInfo();
            if (this.m_Model.CurrentSelectedMemberInfo != null)
            {
                if (this.m_Model.CurrentSelectedMemberInfo.stBriefInfo.uulUid == Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID)
                {
                    Singleton<CPlayerInfoSystem>.GetInstance().ShowPlayerDetailInfo(this.m_Model.CurrentSelectedMemberInfo.stBriefInfo.uulUid, this.m_Model.CurrentSelectedMemberInfo.stBriefInfo.dwLogicWorldId, CPlayerInfoSystem.DetailPlayerInfoSource.Self, true);
                }
                else
                {
                    bool isShowGuildAppointViceChairmanBtn = CGuildSystem.HasAppointViceChairmanAuthority() && CGuildSystem.CanBeAppointedToViceChairman(this.m_Model.CurrentSelectedMemberInfo.enPosition);
                    bool isShowGuildTransferPositionBtn = CGuildSystem.HasTransferPositionAuthority();
                    bool isShowGuildFireMemberBtn = CGuildSystem.HasFireMemberAuthority(this.m_Model.CurrentSelectedMemberInfo.enPosition);
                    Singleton<CPlayerInfoSystem>.GetInstance().ShowPlayerDetailInfo(this.m_Model.CurrentSelectedMemberInfo.stBriefInfo.uulUid, this.m_Model.CurrentSelectedMemberInfo.stBriefInfo.dwLogicWorldId, isShowGuildAppointViceChairmanBtn, isShowGuildTransferPositionBtn, isShowGuildFireMemberBtn);
                }
            }
        }

        private void On_Guild_Need_Approval_Slider_Value_Changed(CUIEvent uiEvent)
        {
            CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
            if (srcFormScript != null)
            {
                int sliderValue = (int) uiEvent.m_eventParams.sliderValue;
                srcFormScript.GetWidget(2).GetComponent<Text>().set_text((sliderValue != 0) ? Singleton<CTextManager>.GetInstance().GetText("Common_No") : Singleton<CTextManager>.GetInstance().GetText("Common_Yes"));
            }
        }

        private void On_Guild_Open_Apply_List_Form(CUIEvent uiEvent)
        {
            this.OpenApplyListForm();
            Singleton<CGuildInfoController>.GetInstance().RequestApplyList(0);
        }

        private void On_Guild_Open_Log_Form(CUIEvent uiEvent)
        {
            Singleton<CGuildInfoController>.GetInstance().RequestGetGuildEvent();
        }

        private void On_Guild_Open_Modify_Guild_Bulletin_Form(CUIEvent uiEvent)
        {
            CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
            if (srcFormScript != null)
            {
                Text component = srcFormScript.GetWidget(7).GetComponent<Text>();
                Singleton<CUIManager>.GetInstance().OpenEditForm(Singleton<CTextManager>.GetInstance().GetText("Guild_Modify_Bulletin"), component.get_text(), enUIEventID.Guild_Confirm_Modify_Guild_Bulletin);
            }
        }

        private void On_Guild_Open_Transfer_Chairman_Without_Secure_Pwd_Confirm(CUIEvent uiEvent)
        {
            if (string.IsNullOrEmpty(uiEvent.m_eventParams.pwd))
            {
                Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("Guild_Transfer_Chairman_Without_Secure_Pwd"), enUIEventID.Guild_Real_Transfer_Chairman, enUIEventID.None, false);
            }
            else
            {
                stUIEventParams par = new stUIEventParams();
                par.pwd = uiEvent.m_eventParams.pwd;
                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Guild_Real_Transfer_Chairman, par);
            }
        }

        private void On_Guild_Preview_Guild_List_Element_Enabled(CUIEvent uiEvent)
        {
            GuildInfo guildInfoByIndex = this.m_Model.GetGuildInfoByIndex(uiEvent.m_srcWidgetIndexInBelongedList);
            if (guildInfoByIndex != null)
            {
                this.SetPreviewGuildListItem(uiEvent.m_srcWidgetScript as CUIListElementScript, guildInfoByIndex);
            }
        }

        public void On_Guild_Preview_Guild_List_Element_Select(CUIEvent uiEvent)
        {
            CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
            if (srcFormScript != null)
            {
                CUIHttpImageScript component = srcFormScript.GetWidget(7).GetComponent<CUIHttpImageScript>();
                Image image = component.get_transform().Find("NobeIcon").GetComponent<Image>();
                Image image2 = component.get_transform().Find("NobeImag").GetComponent<Image>();
                Text text = srcFormScript.GetWidget(5).GetComponent<Text>();
                Text text2 = srcFormScript.GetWidget(8).GetComponent<Text>();
                Text text3 = srcFormScript.GetWidget(6).GetComponent<Text>();
                GameObject widget = srcFormScript.GetWidget(3);
                GuildInfo guildInfoByIndex = this.m_Model.GetGuildInfoByIndex(uiEvent.m_srcWidgetScript.GetSelectedIndex());
                if (guildInfoByIndex == null)
                {
                    object[] inParameters = new object[] { uiEvent.m_srcWidgetIndexInBelongedList };
                    DebugHelper.Assert(false, "CGuildInfoView.On_Guild_Preview_Guild_List_Element_Select(): guildInfo is null, uiEvent.m_srcWidgetIndexInBelongedList={0}", inParameters);
                }
                else
                {
                    component.SetImageUrl(CGuildHelper.GetHeadUrl(guildInfoByIndex.chairman.stBriefInfo.szHeadUrl));
                    MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(image, CGuildHelper.GetNobeLevel(guildInfoByIndex.chairman.stBriefInfo.uulUid, guildInfoByIndex.chairman.stBriefInfo.stVip.level), false);
                    MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(image2, CGuildHelper.GetNobeHeadIconId(guildInfoByIndex.chairman.stBriefInfo.uulUid, guildInfoByIndex.chairman.stBriefInfo.stVip.headIconId));
                    text.set_text(guildInfoByIndex.chairman.stBriefInfo.sName);
                    string[] args = new string[] { guildInfoByIndex.chairman.stBriefInfo.dwLevel.ToString() };
                    text2.set_text(Singleton<CTextManager>.GetInstance().GetText("Common_Level_Format", args));
                    text3.set_text(guildInfoByIndex.briefInfo.sBulletin);
                    widget.CustomSetActive(guildInfoByIndex.briefInfo.uulUid == this.m_Model.m_InviteGuildUuid);
                }
            }
        }

        private void On_Guild_Preview_Request_More_Guild_List(CUIEvent uiEvent)
        {
            CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
            if (srcFormScript != null)
            {
                int elementAmount = srcFormScript.GetWidget(0).GetComponent<CUIListScript>().GetElementAmount();
                if (elementAmount > 0)
                {
                    Singleton<EventRouter>.GetInstance().BroadCastEvent<int, int>("Request_Guild_List", elementAmount + 1, 20);
                }
            }
        }

        public void On_Guild_Preview_Request_Ranking_List(CUIEvent uiEvent)
        {
            Singleton<EventRouter>.GetInstance().BroadCastEvent<int, int>("Guild_Preview_Get_Ranking_List", 0, 20);
        }

        private void On_Guild_Rankpoint_Enter_Matching(CUIEvent uiEvent)
        {
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Matching_OpenEntry);
        }

        private void On_Guild_Rankpoint_Help(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.GetInstance().OpenInfoForm(9);
        }

        private void On_Guild_Rankpoint_Member_List_Element_Enabled(CUIEvent uiEvent)
        {
            CUIListElementScript srcWidgetScript = uiEvent.m_srcWidgetScript as CUIListElementScript;
            if (srcWidgetScript == null)
            {
                DebugHelper.Assert(false, "CGuildInfoView.On_Guild_Rankpoint_Member_List_Element_Enabled(): listElement is null!!!");
            }
            else
            {
                int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
                KeyValuePair<ulong, MemberRankInfo> pair = this.m_Model.RankpointMemberInfoList[srcWidgetIndexInBelongedList];
                ulong key = pair.Key;
                GuildMemInfo guildMemberInfoByUid = this.m_Model.GetGuildMemberInfoByUid(key);
                this.SetRankpointMemberItem(srcWidgetScript.get_transform(), guildMemberInfoByUid, srcWidgetIndexInBelongedList);
            }
        }

        private void On_Guild_Rankpoint_Open_Rankpoint_Rank_Form(CUIEvent uiEvent)
        {
            this.OpenRankpointRankForm();
        }

        private void On_Guild_Rankpoint_Rank_List_Element_Enabled(CUIEvent uiEvent)
        {
            CUIListElementScript srcWidgetScript = uiEvent.m_srcWidgetScript as CUIListElementScript;
            if (srcWidgetScript == null)
            {
                DebugHelper.Assert(false, "CGuildInfoView.On_Guild_Rankpoint_Rank_List_Element_Enabled(): listElement is null!!!");
            }
            else
            {
                int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
                enGuildRankpointRankListType rankListTypeByTabSelectedIndex = this.GetRankListTypeByTabSelectedIndex(uiEvent.m_srcFormScript);
                ListView<RankpointRankInfo> view = this.m_Model.RankpointRankInfoLists[(int) rankListTypeByTabSelectedIndex];
                if ((srcWidgetIndexInBelongedList < 0) || (srcWidgetIndexInBelongedList >= view.Count))
                {
                    object[] inParameters = new object[] { rankListTypeByTabSelectedIndex, srcWidgetIndexInBelongedList };
                    DebugHelper.Assert(false, "rankListType {0}, elementIndex {1} is out of list range", inParameters);
                }
                else if (view[srcWidgetIndexInBelongedList] == null)
                {
                    object[] objArray2 = new object[] { rankListTypeByTabSelectedIndex, srcWidgetIndexInBelongedList };
                    DebugHelper.Assert(false, "rankListType {0}, elementIndex {1} object is null", objArray2);
                }
                else
                {
                    this.SetRankpointGuildRankListItem(srcWidgetScript, view[srcWidgetIndexInBelongedList], rankListTypeByTabSelectedIndex, view[0].rankNo);
                }
            }
        }

        private void On_Guild_Rankpoint_Rank_List_Tab_Change(CUIEvent uiEvent)
        {
            this.RefreshRankpointRankList(uiEvent);
            this.SetRankpointRankListWidgetPositionAndSize(uiEvent);
        }

        private void On_Guild_Rankpoint_Season_Rank_List_Tab_Change(CUIEvent uiEvent)
        {
            this.RefreshRankpointSeasonRankList(uiEvent);
        }

        private void On_Guild_Real_Transfer_Chairman(CUIEvent uiEvent)
        {
            ulong uulUid = this.m_Model.CurrentSelectedMemberInfo.stBriefInfo.uulUid;
            byte num2 = 3;
            ulong playerUllUID = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID;
            string pwd = uiEvent.m_eventParams.pwd;
            Singleton<EventRouter>.GetInstance().BroadCastEvent<ulong, byte, ulong, string>("Guild_Position_Appoint", uulUid, num2, playerUllUID, pwd);
        }

        private void On_Guild_Recommend_Invite(CUIEvent uiEvent)
        {
            CUIListElementScript elemenet = uiEvent.m_srcWidgetBelongedListScript.GetComponent<CUIListScript>().GetElemenet(uiEvent.m_srcWidgetIndexInBelongedList);
            if (elemenet != null)
            {
                ulong num;
                Text component = elemenet.get_transform().Find("txtUidData").GetComponent<Text>();
                if (ulong.TryParse(component.get_text(), out num))
                {
                    Singleton<EventRouter>.GetInstance().BroadCastEvent<ulong>("Guild_Invite", num);
                    this.m_Model.RemoveRecommendInfo(num);
                    this.RefreshApplyListPanel();
                }
                else
                {
                    object[] inParameters = new object[] { component.get_text() };
                    DebugHelper.Assert(false, "CGuildInfoView.On_Guild_Recommend_Invite(): txtUid.text={0}", inParameters);
                }
            }
        }

        private void On_Guild_Recommend_Reject(CUIEvent uiEvent)
        {
            CUIListElementScript elemenet = uiEvent.m_srcWidgetBelongedListScript.GetComponent<CUIListScript>().GetElemenet(uiEvent.m_srcWidgetIndexInBelongedList);
            if (elemenet != null)
            {
                ulong num;
                Text component = elemenet.get_transform().Find("txtUidData").GetComponent<Text>();
                if (ulong.TryParse(component.get_text(), out num))
                {
                    Singleton<EventRouter>.GetInstance().BroadCastEvent<ulong>("Guild_Reject_Recommend", num);
                }
                else
                {
                    object[] inParameters = new object[] { component.get_text() };
                    DebugHelper.Assert(false, "CGuildInfoView.On_Guild_Recommend_Reject(): txtUid.text={0}", inParameters);
                }
            }
        }

        private void On_Guild_Recruit_Send_Recruit(CUIEvent uiEvent)
        {
            Singleton<CGuildInfoController>.GetInstance().RequestSendGuildRecruitReq();
            GameObject widget = this.m_form.GetWidget(0x27);
            widget.CustomSetActive(true);
            CUITimerScript component = widget.GetComponent<CUITimerScript>();
            uint guildSendRecruitCd = this.GetGuildSendRecruitCd();
            component.SetTotalTime((float) guildSendRecruitCd);
            component.StartTimer();
            this.m_lastSendReruitTime = CRoleInfo.GetCurrentUTCTime();
            CUICommonSystem.SetButtonEnable(this.m_form.GetWidget(0x26).GetComponent<Button>(), false, false, true);
            this.m_form.GetWidget(40).CustomSetActive(false);
        }

        private void On_Guild_Recruit_Send_Recruit_CD_Timeout(CUIEvent uiEvent)
        {
            this.m_form.GetWidget(40).CustomSetActive(true);
            CUICommonSystem.SetButtonEnable(this.m_form.GetWidget(0x26).GetComponent<Button>(), true, true, true);
            this.m_form.GetWidget(0x27).CustomSetActive(false);
        }

        private void On_Guild_Setting_Down_Join_Grade(CUIEvent uiEvent)
        {
            int num;
            CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
            Text component = srcFormScript.GetWidget(7).GetComponent<Text>();
            Text text2 = srcFormScript.GetWidget(8).GetComponent<Text>();
            if (int.TryParse(component.get_text(), out num))
            {
                int gradeLimit = num - 1;
                if (gradeLimit >= 1)
                {
                    component.set_text(gradeLimit.ToString());
                    text2.set_text(CGuildHelper.GetLadderGradeLimitText(gradeLimit));
                }
                if (gradeLimit <= 1)
                {
                    CUICommonSystem.SetButtonEnable(srcFormScript.GetWidget(12).GetComponent<Button>(), false, false, true);
                }
                CUICommonSystem.SetButtonEnable(srcFormScript.GetWidget(11).GetComponent<Button>(), true, true, true);
            }
        }

        private void On_Guild_Setting_Down_Join_Level_Num(CUIEvent uiEvent)
        {
            int num;
            CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
            Text component = srcFormScript.GetWidget(6).GetComponent<Text>();
            if (int.TryParse(component.get_text(), out num))
            {
                int num2 = num - 1;
                if (num2 >= CGuildHelper.GetGuildMemberMinPvpLevel())
                {
                    component.set_text(num2.ToString());
                }
                if (num2 <= CGuildHelper.GetGuildMemberMinPvpLevel())
                {
                    CUICommonSystem.SetButtonEnable(srcFormScript.GetWidget(10).GetComponent<Button>(), false, false, true);
                }
                CUICommonSystem.SetButtonEnable(srcFormScript.GetWidget(9).GetComponent<Button>(), true, true, true);
            }
        }

        private void On_Guild_Setting_Up_Join_Grade(CUIEvent uiEvent)
        {
            int num;
            CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
            Text component = srcFormScript.GetWidget(7).GetComponent<Text>();
            Text text2 = srcFormScript.GetWidget(8).GetComponent<Text>();
            if (int.TryParse(component.get_text(), out num))
            {
                int gradeLimit = num + 1;
                if (gradeLimit <= CLadderSystem.MAX_RANK_LEVEL)
                {
                    component.set_text(gradeLimit.ToString());
                    text2.set_text(CGuildHelper.GetLadderGradeLimitText(gradeLimit));
                }
                if (gradeLimit >= CLadderSystem.MAX_RANK_LEVEL)
                {
                    CUICommonSystem.SetButtonEnable(srcFormScript.GetWidget(11).GetComponent<Button>(), false, false, true);
                }
                CUICommonSystem.SetButtonEnable(srcFormScript.GetWidget(12).GetComponent<Button>(), true, true, true);
            }
        }

        private void On_Guild_Setting_Up_Join_Level_Num(CUIEvent uiEvent)
        {
            int num;
            CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
            Text component = srcFormScript.GetWidget(6).GetComponent<Text>();
            if (int.TryParse(component.get_text(), out num))
            {
                int num2 = num + 1;
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if ((masterRoleInfo != null) && (num2 <= masterRoleInfo.GetMaxPvpLevel()))
                {
                    component.set_text(num2.ToString());
                }
                if ((masterRoleInfo != null) && (num2 >= masterRoleInfo.GetMaxPvpLevel()))
                {
                    CUICommonSystem.SetButtonEnable(srcFormScript.GetWidget(9).GetComponent<Button>(), false, false, true);
                }
                CUICommonSystem.SetButtonEnable(srcFormScript.GetWidget(10).GetComponent<Button>(), true, true, true);
            }
        }

        private void On_Guild_Sign(CUIEvent uiEvent)
        {
            Singleton<EventRouter>.GetInstance().BroadCastEvent("Guild_Sign");
        }

        private void On_Guild_UnBindQQGroup(CUIEvent uiEvent)
        {
            string text = Singleton<CTextManager>.GetInstance().GetText("Guild_QQGroup_Unbind_Confirm");
            Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(text, enUIEventID.Guild_UnBindQQGroupConfirm, enUIEventID.None, false);
        }

        private void On_Guild_UnBindQQGroupConfirm(CUIEvent uiEvent)
        {
            IApolloSnsService iApolloSnsService = Utility.GetIApolloSnsService();
            if (iApolloSnsService != null)
            {
                string groupOpenId = this.m_Model.CurrentGuildInfo.groupOpenId;
                string cUnionid = CGuildHelper.GetGroupGuildId().ToString();
                if (!string.IsNullOrEmpty(groupOpenId))
                {
                    iApolloSnsService.UnbindQQGroup(groupOpenId, cUnionid);
                    Debug.Log("UnbindQQGroup: guildOpenId=" + groupOpenId + ", guild32BitsUid=" + cUnionid);
                }
                else
                {
                    Debug.Log("UnbindQQGroup: guildOpenId is null!!!");
                }
            }
        }

        public void On_Tab_Change(CUIEvent uiEvent)
        {
            CUIListScript component = uiEvent.m_srcWidget.GetComponent<CUIListScript>();
            if (component != null)
            {
                int selectedIndex = component.GetSelectedIndex();
                this.CurTab = (Tab) selectedIndex;
            }
            this.InitPanel();
        }

        private void OnBindQQGroupNotify(ApolloGroupResult groupRet)
        {
            Debug.Log("OnBindQQGroupNotify()");
            if (this.m_form != null)
            {
                GameObject widget = this.m_form.GetWidget(0x13);
                if (widget != null)
                {
                    widget.CustomSetActive(true);
                }
                GameObject obj3 = this.m_form.GetWidget(0x10);
                if (obj3 != null)
                {
                    obj3.CustomSetActive(false);
                }
                this.m_form.GetWidget(15).GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("Guild_QQGroup_Waiting_Status_Tip"));
            }
            Singleton<CTimerManager>.GetInstance().AddTimer(0x2710, 1, new CTimer.OnTimeUpHandler(this.QueryQQGroupInfo));
        }

        private void OnGuildPositionAgreeSelfRecommend(CUIEvent uiEvent)
        {
            ulong num = uiEvent.m_eventParams.commonUInt64Param1;
            Singleton<EventRouter>.GetInstance().BroadCastEvent<ulong, bool>("Guild_Position_Deal_Self_Recommend", num, true);
            this.m_Model.RemoveSelfRecommendInfo(num);
        }

        private void OnGuildPositionAppointViceChairman(CUIEvent uiEvent)
        {
            if (!CGuildSystem.HasAppointViceChairmanAuthority())
            {
                Singleton<CUIManager>.GetInstance().OpenTips("Guild_Appoint_You_Have_No_Authority", true, 1.5f, null, new object[0]);
            }
            else if (!CGuildSystem.CanBeAppointedToViceChairman(this.m_Model.CurrentSelectedMemberInfo.enPosition))
            {
                Singleton<CUIManager>.GetInstance().OpenTips("Guild_Appoint_Vice_Chairman_Position_Limit_Tip", true, 1.5f, null, new object[0]);
            }
            else if (CGuildHelper.IsViceChairmanFull())
            {
                List<ulong> list;
                List<string> list2;
                CGuildHelper.GetViceChairmanUidAndName(out list, out list2);
                this.OpenExchangePositionForm(list2, list);
            }
            else
            {
                string sName = this.m_Model.CurrentSelectedMemberInfo.stBriefInfo.sName;
                string[] args = new string[] { sName };
                string text = Singleton<CTextManager>.GetInstance().GetText("Guild_Appoint_Vice_Chairman_Confirm", args);
                Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(text, enUIEventID.Guild_Position_Appoint_Vice_Chairman_Confirm, enUIEventID.None, false);
            }
        }

        private void OnGuildPositionAppointViceChairmanConfirm(CUIEvent uiEvent)
        {
            ulong uulUid = this.m_Model.CurrentSelectedMemberInfo.stBriefInfo.uulUid;
            byte num2 = 4;
            ulong num3 = 0L;
            if (Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Guild/Form_Guild_Exchange_Position.prefab") != null)
            {
                num3 = ulong.Parse(uiEvent.m_srcFormScript.GetWidget(0).GetComponent<CUIListScript>().GetSelectedElement().get_transform().Find("Uid").GetComponent<Text>().get_text());
            }
            Singleton<EventRouter>.GetInstance().BroadCastEvent<ulong, byte, ulong, string>("Guild_Position_Appoint", uulUid, num2, num3, string.Empty);
        }

        private void OnGuildPositionChairmanTransfer(CUIEvent uiEvent)
        {
            if (!CGuildSystem.HasTransferPositionAuthority())
            {
                Singleton<CUIManager>.GetInstance().OpenTips("Guild_Only_Chairman_Can_Transfer_Position", true, 1.5f, null, new object[0]);
            }
            else if (this.m_Model.CurrentSelectedMemberInfo.stBriefInfo.uulUid == Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID)
            {
                Singleton<CUIManager>.GetInstance().OpenTips("Guild_Cannot_Transfer_To_Self", true, 1.5f, null, new object[0]);
            }
            else
            {
                string[] args = new string[] { this.m_Model.CurrentSelectedMemberInfo.stBriefInfo.sName };
                string text = Singleton<CTextManager>.GetInstance().GetText("Guild_Position_Transfer_Confirm", args);
                Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(text, enUIEventID.Guild_Position_Chairman_Transfer_Confirm, enUIEventID.None, Singleton<CTextManager>.GetInstance().GetText("Guild_Transfer_Btn"), string.Empty, false);
            }
        }

        private void OnGuildPositionChairmanTransferConfirm(CUIEvent uiEvent)
        {
            CSecurePwdSystem.TryToValidate(enOpPurpose.GUILD_TRANSFER_CHAIRMAN, enUIEventID.Guild_Open_Transfer_Chairman_Without_Secure_Pwd_Confirm, new stUIEventParams());
        }

        private void OnGuildPositionDisagreeSelfRecommend(CUIEvent uiEvent)
        {
            ulong num = uiEvent.m_eventParams.commonUInt64Param1;
            Singleton<EventRouter>.GetInstance().BroadCastEvent<ulong, bool>("Guild_Position_Deal_Self_Recommend", num, false);
            this.m_Model.RemoveSelfRecommendInfo(num);
        }

        private void OnGuildPositionFireMember(CUIEvent uiEvent)
        {
            if (!CGuildSystem.HasFireMemberAuthority(this.m_Model.CurrentSelectedMemberInfo.enPosition))
            {
                Singleton<CUIManager>.GetInstance().OpenTips("Guild_Fire_Member_You_Have_No_Authority", true, 1.5f, null, new object[0]);
            }
            else
            {
                string sName = this.m_Model.CurrentSelectedMemberInfo.stBriefInfo.sName;
                string[] args = new string[] { sName, sName, sName, sName };
                string text = Singleton<CTextManager>.GetInstance().GetText("Guild_Fire_Member_Confirm", args);
                Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(text, enUIEventID.Guild_Position_Fire_Member_Confirm, enUIEventID.None, false);
            }
        }

        private void OnGuildPositionFireMemberConfirm(CUIEvent uiEvent)
        {
            ulong uulUid = this.m_Model.CurrentSelectedMemberInfo.stBriefInfo.uulUid;
            Singleton<EventRouter>.GetInstance().BroadCastEvent<ulong>("Guild_Position_Confirm_Fire_Member", uulUid);
        }

        private void OnGuildPositionRecommendSelfAsChairman(CUIEvent uiEvent)
        {
            string[] args = new string[] { CGuildHelper.GetSelfRecommendTimeout().ToString() };
            string text = Singleton<CTextManager>.GetInstance().GetText("Guild_Recommend_Self_As_Chairman_Confirm", args);
            Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(text, enUIEventID.Guild_Position_Recommend_Self_As_Chairman_Confirm, enUIEventID.None, false);
        }

        private void OnGuildPositionRecommendSelfAsChairmanConfirm(CUIEvent uiEvent)
        {
            Singleton<EventRouter>.GetInstance().BroadCastEvent("Guild_Position_Confirm_Recommend_Self_As_Chairman");
        }

        private void OnQueryQQGroupInfoNotify(ApolloGroupResult groupRet)
        {
            string str;
            bool isBound = false;
            bool isSelfInGroup = false;
            if (groupRet.result == ApolloResult.Success)
            {
                isBound = true;
                isSelfInGroup = true;
                this.m_Model.CurrentGuildInfo.groupKey = groupRet.groupInfo.groupKey;
                if (!string.IsNullOrEmpty(groupRet.groupInfo.groupOpenid) && (this.m_Model.CurrentGuildInfo.groupOpenId != groupRet.groupInfo.groupOpenid))
                {
                    Debug.Log("Old groupOpenId=" + this.m_Model.CurrentGuildInfo.groupOpenId);
                    Singleton<EventRouter>.GetInstance().BroadCastEvent<string>("Guild_QQGroup_Set_Guild_Group_Open_Id", groupRet.groupInfo.groupOpenid);
                }
                this.m_Model.CurrentGuildInfo.groupOpenId = groupRet.groupInfo.groupOpenid;
                str = string.Empty;
            }
            else if (groupRet.errorCode == 0x7d2)
            {
                str = groupRet.errorCode + Singleton<CTextManager>.GetInstance().GetText("Guild_QQGroup_Query_Err_Group_Not_Bind");
            }
            else if (groupRet.errorCode == 0x7d3)
            {
                isBound = true;
                this.QueryQQGroupKey();
                object[] objArray1 = new object[] { groupRet.errorCode, Singleton<CTextManager>.GetInstance().GetText("Guild_QQGroup_Query_Err_Not_In_Group"), "groupOpenid=", this.m_Model.CurrentGuildInfo.groupOpenId };
                str = string.Concat(objArray1);
            }
            else if (groupRet.errorCode == 0x7d7)
            {
                str = groupRet.errorCode + Singleton<CTextManager>.GetInstance().GetText("Guild_QQGroup_Query_Err_Group_Not_Exist");
            }
            else
            {
                str = groupRet.errorCode + Singleton<CTextManager>.GetInstance().GetText("Guild_QQGroup_Query_Err_System_Error");
            }
            this.RefreshQQGroupPanel(isBound, !isBound ? string.Empty : groupRet.groupInfo.groupName, isSelfInGroup);
            Debug.Log(string.Concat(new object[] { str, "\nisBound=", isBound, ", isSelfInGroup=", isSelfInGroup, ", groupName=", groupRet.groupInfo.groupName }));
        }

        private void OnQueryQQGroupKeyNotify(ApolloGroupResult groupRet)
        {
            if (groupRet.result == ApolloResult.Success)
            {
                this.m_Model.CurrentGuildInfo.groupKey = groupRet.groupInfo.groupKey;
                Debug.Log("OnQueryQQGroupKeyNotify(): groupKey=" + this.m_Model.CurrentGuildInfo.groupKey);
            }
            else
            {
                Debug.Log(string.Concat(new object[] { "Query failed, result=", groupRet.result, ", errorCode=", groupRet.errorCode }));
            }
        }

        private void OnUnBindQQGroupNotify(ApolloGroupResult groupRet)
        {
            string text;
            bool isBound = true;
            if (groupRet.result == ApolloResult.Success)
            {
                isBound = false;
                text = Singleton<CTextManager>.GetInstance().GetText("Guild_QQGroup_UnBind_Success");
            }
            else if (groupRet.errorCode == 0x7d1)
            {
                text = groupRet.errorCode + Singleton<CTextManager>.GetInstance().GetText("Guild_QQGroup_UnBind_Err_QQ_Group_Not_Bind");
            }
            else if (groupRet.errorCode == 0x7d3)
            {
                text = groupRet.errorCode + Singleton<CTextManager>.GetInstance().GetText("Guild_QQGroup_UnBind_Err_Login_Session_Expired");
            }
            else if (groupRet.errorCode == 0x7d4)
            {
                text = groupRet.errorCode + Singleton<CTextManager>.GetInstance().GetText("Guild_QQGroup_UnBind_Err_Too_Many_Operations");
            }
            else if (groupRet.errorCode == 0x7d5)
            {
                text = groupRet.errorCode + Singleton<CTextManager>.GetInstance().GetText("Guild_QQGroup_UnBind_Err_Param_Error");
            }
            else
            {
                text = groupRet.errorCode + Singleton<CTextManager>.GetInstance().GetText("Guild_QQGroup_UnBind_Err_System_Error");
            }
            Singleton<CUIManager>.GetInstance().OpenTips(text, false, 1.5f, null, new object[0]);
            Debug.Log(text);
            this.RefreshQQGroupPanel(isBound, string.Empty, false);
        }

        public void OpenAbdicateConfirm()
        {
            if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.guildState == COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_CHAIRMAN)
            {
                for (int i = 0; i < this.m_Model.CurrentGuildInfo.listSelfRecommendInfo.Count; i++)
                {
                    this.OpenAbdicateConfirm(this.m_Model.CurrentGuildInfo.listSelfRecommendInfo[i].uid);
                }
            }
        }

        public void OpenAbdicateConfirm(ulong selfRecommendUid)
        {
            string sName = this.m_Model.GetGuildMemberInfoByUid(selfRecommendUid).stBriefInfo.sName;
            string[] args = new string[] { sName, CGuildHelper.GetSelfRecommendTimeout().ToString() };
            string text = Singleton<CTextManager>.GetInstance().GetText("Guild_Abdicate_Confirm", args);
            stUIEventParams param = new stUIEventParams();
            param.commonUInt64Param1 = selfRecommendUid;
            Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(text, enUIEventID.Guild_Position_Disagree_Self_Recommend, enUIEventID.Guild_Position_Agree_Self_Recommend, param, Singleton<CTextManager>.GetInstance().GetText("Common_Reject"), Singleton<CTextManager>.GetInstance().GetText("Common_Pass"), false);
        }

        private void OpenApplyListForm()
        {
            CUIFormScript applyListForm = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/System/Guild/Form_Guild_Apply_List.prefab", false, true);
            if (applyListForm != null)
            {
                this.RefreshApplyListCloseRedDotToggle(applyListForm);
            }
        }

        private void OpenDonateForm()
        {
            CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/System/Guild/Form_Guild_Donate.prefab", false, true);
            if (script != null)
            {
                uint dwConfValue = GameDataMgr.guildMiscDatabin.GetDataByKey((uint) 0x13).dwConfValue;
                uint donateCnt = this.m_Model.GetPlayerGuildMemberInfo().DonateCnt;
                string[] args = new string[] { donateCnt.ToString(), dwConfValue.ToString() };
                string text = Singleton<CTextManager>.GetInstance().GetText("Guild_Donate_Today_Times", args);
                script.GetWidget(0).GetComponent<Text>().set_text(text);
                script.GetWidget(1).GetComponent<Text>().set_text(CGuildHelper.GetDonateDescription(RES_GUILD_DONATE_TYPE.RES_GUILD_DONATE_TYPE_LOW));
                script.GetWidget(2).GetComponent<Text>().set_text(CGuildHelper.GetDonateDescription(RES_GUILD_DONATE_TYPE.RES_GUILD_DONATE_TYPE_MID));
                script.GetWidget(3).GetComponent<Text>().set_text(CGuildHelper.GetDonateDescription(RES_GUILD_DONATE_TYPE.RES_GUILD_DONATE_TYPE_HIGH));
                script.GetWidget(4).GetComponent<Text>().set_text(CGuildHelper.GetDonateDescription(RES_GUILD_DONATE_TYPE.RES_GUILD_DONATE_TYPE_TOP));
            }
        }

        public void OpenExchangePositionForm(List<string> names, List<ulong> uids)
        {
            CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/System/Guild/Form_Guild_Exchange_Position.prefab", false, true);
            if (script != null)
            {
                CUIListScript component = script.GetWidget(0).GetComponent<CUIListScript>();
                component.SetElementAmount(names.Count);
                for (int i = 0; i < names.Count; i++)
                {
                    Transform transform = component.GetElemenet(i).get_transform();
                    transform.Find("Name").GetComponent<Text>().set_text(names[i]);
                    ulong num2 = uids[i];
                    transform.Find("Uid").GetComponent<Text>().set_text(num2.ToString());
                }
                component.SelectElement(0, true);
            }
        }

        public void OpenForm()
        {
            if (!this.IsShow())
            {
                this.m_form = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/System/Guild/Form_Guild_Info.prefab", false, true);
                this.InitTab();
                this.RefreshApplyRedDot();
                Singleton<EventRouter>.GetInstance().BroadCastEvent<enGuildRankpointRankListType>("Guild_Request_Rankpoint_Rank_List", enGuildRankpointRankListType.CurrentWeek);
            }
        }

        public void OpenGuildIconForm()
        {
            <OpenGuildIconForm>c__AnonStorey51 storey = new <OpenGuildIconForm>c__AnonStorey51();
            storey.<>f__this = this;
            CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/System/Guild/Form_Guild_Icon.prefab", false, true);
            storey.listScript = script.GetWidget(0).GetComponent<CUIListScript>();
            int showGuildIconCount = this.GetShowGuildIconCount();
            storey.listScript.SetElementAmount(showGuildIconCount);
            storey.i = 0;
            GameDataMgr.guildIconDatabin.Accept(new Action<ResGuildIcon>(storey.<>m__3B));
            if (Singleton<CGuildSystem>.GetInstance().IsInNormalGuild())
            {
                storey.listScript.SetElementSelectChangedEvent(enUIEventID.Guild_Guild_Setting_Guild_Icon_Selected);
            }
        }

        public void OpenGuildPreviewForm(bool isHideListExtraContent = false)
        {
            Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/System/Guild/Form_Guild_Preview.prefab", false, true);
            this.RefreshPreviewForm(false);
        }

        public void OpenLogForm(SCPKG_GUILD_EVENT_RSP rsp)
        {
            CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/System/Guild/Form_Guild_Log.prefab", false, true);
            if (script != null)
            {
                script.GetWidget(1).CustomSetActive(rsp.bNum == 0);
                Text component = script.GetWidget(0).GetComponent<Text>();
                component.set_text(string.Empty);
                for (int i = rsp.bNum - 1; i >= 0; i--)
                {
                    uint dwEventTime = rsp.astContent[i].dwEventTime;
                    component.set_text(component.get_text() + ("<color=#717b93>[" + Utility.GetUtcToLocalTimeStringFormat((ulong) dwEventTime, "yyyy.M.d") + "]</color>") + StringHelper.UTF8BytesToString(ref rsp.astContent[i].szContent) + "\n");
                }
            }
        }

        public void OpenRankpointRankForm()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Guild/Form_Guild_RankpointRank.prefab");
            if (form == null)
            {
                form = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/System/Guild/Form_Guild_RankpointRank.prefab", false, true);
            }
            RectTransform transform = form.GetWidget(6).get_transform() as RectTransform;
            s_rankpointRankListWidgetAnchoredPositionY = transform.get_anchoredPosition().y;
            s_rankpointRankListWidgetSizeDeltaY = transform.get_sizeDelta().y;
            this.InitRankpointRankTab(form);
            this.InitRankpointSeasonRankTab(form);
        }

        private void OpenSettingForm()
        {
            CUIFormScript formScript = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/System/Guild/Form_Guild_Setting.prefab", false, true);
            if (formScript != null)
            {
                Image component = formScript.GetWidget(0).GetComponent<Image>();
                string prefabPath = CUIUtility.s_Sprite_Dynamic_GuildHead_Dir + this.m_Model.CurrentGuildInfo.briefInfo.dwHeadId;
                component.SetSprite(prefabPath, formScript, true, false, false, false);
                formScript.GetWidget(3).GetComponent<Text>().set_text(this.m_Model.CurrentGuildInfo.briefInfo.dwHeadId.ToString());
                Slider slider = formScript.GetWidget(1).GetComponent<Slider>();
                bool flag = CGuildHelper.IsGuildNeedApproval(this.m_Model.CurrentGuildInfo.briefInfo.dwSettingMask);
                slider.set_value(!flag ? ((float) 1) : ((float) 0));
                formScript.GetWidget(5).GetComponent<Text>().set_text(this.m_Model.CurrentGuildInfo.briefInfo.sName);
                formScript.GetWidget(4).get_gameObject().CustomSetActive(CGuildSystem.HasGuildNameChangeAuthority());
                formScript.GetWidget(6).GetComponent<Text>().set_text(this.m_Model.CurrentGuildInfo.briefInfo.LevelLimit.ToString());
                if (this.m_Model.CurrentGuildInfo.briefInfo.LevelLimit <= CGuildHelper.GetGuildMemberMinPvpLevel())
                {
                    CUICommonSystem.SetButtonEnable(formScript.GetWidget(10).GetComponent<Button>(), false, false, true);
                }
                formScript.GetWidget(8).GetComponent<Text>().set_text(CGuildHelper.GetLadderGradeLimitText(this.m_Model.CurrentGuildInfo.briefInfo.GradeLimit));
                if (this.m_Model.CurrentGuildInfo.briefInfo.GradeLimit <= 1)
                {
                    CUICommonSystem.SetButtonEnable(formScript.GetWidget(12).GetComponent<Button>(), false, false, true);
                }
                formScript.GetWidget(7).GetComponent<Text>().set_text(this.m_Model.CurrentGuildInfo.briefInfo.GradeLimit.ToString());
            }
        }

        public void OpenSignSuccessForm(string tip)
        {
            CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/System/Guild/Form_Guild_Sign_Success.prefab", false, true);
            if (script != null)
            {
                script.GetWidget(0).GetComponent<Text>().set_text(tip);
            }
        }

        public void QueryQQGroupInfo(int timerSequence)
        {
            IApolloSnsService iApolloSnsService = Utility.GetIApolloSnsService();
            if (iApolloSnsService != null)
            {
                string cUnionid = CGuildHelper.GetGroupGuildId().ToString();
                string cZoneid = CGuildHelper.GetGuildLogicWorldId().ToString();
                Debug.Log("QueryQQGroupInfo(): guild32bitsUid=" + cUnionid + ", zoneId=" + cZoneid);
                iApolloSnsService.QueryQQGroupInfo(cUnionid, cZoneid);
            }
        }

        private void QueryQQGroupKey()
        {
            IApolloSnsService iApolloSnsService = Utility.GetIApolloSnsService();
            if (iApolloSnsService != null)
            {
                Debug.Log("QueryQQGroupKey(): groupOpenId=" + this.m_Model.CurrentGuildInfo.groupOpenId);
                iApolloSnsService.QueryQQGroupKey(this.m_Model.CurrentGuildInfo.groupOpenId);
            }
        }

        private void RefreshApplyListCloseRedDotToggle(CUIFormScript applyListForm)
        {
            bool flag = this.IsApplyListCloseRedDotPrefOn();
            applyListForm.GetWidget(1).GetComponent<Toggle>().set_isOn(flag);
        }

        public void RefreshApplyListPanel()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Guild/Form_Guild_Apply_List.prefab");
            if (form != null)
            {
                CUIListScript component = form.GetWidget(0).GetComponent<CUIListScript>();
                bool flag = CGuildSystem.HasManageAuthority();
                int recommendInfoCount = this.m_Model.GetRecommendInfoCount();
                int applicantsCount = this.m_Model.GetApplicantsCount();
                List<stRecommendInfo> list = (recommendInfoCount != 0) ? this.m_Model.GetRecommendInfo() : null;
                List<stApplicantInfo> list2 = (applicantsCount != 0) ? this.m_Model.GetApplicants() : null;
                component.SetElementAmount(recommendInfoCount + applicantsCount);
                if ((recommendInfoCount + applicantsCount) == 0)
                {
                    CGuildSystem.s_isApplyAndRecommendListEmpty = true;
                    this.RefreshApplyRedDot();
                }
            }
        }

        public void RefreshApplyRedDot()
        {
            this.RefreshMemberTabRedDot();
            this.RefreshMemberPanelApplyBtnRedDot();
        }

        public void RefreshBulletinPanel()
        {
            if (this.m_form != null)
            {
                GuildInfo currentGuildInfo = this.m_Model.CurrentGuildInfo;
                if (currentGuildInfo == null)
                {
                    DebugHelper.Assert(false, "CGuildInfoView.RefreshInfoPanel(): guildInfo is null!!!");
                }
                else
                {
                    this.m_form.GetWidget(7).GetComponent<Text>().set_text(currentGuildInfo.briefInfo.sBulletin);
                    this.m_form.GetWidget(10).CustomSetActive(CGuildSystem.HasManageAuthority());
                }
            }
        }

        public void RefreshFacePanel()
        {
            if (this.m_form != null)
            {
                GuildInfo currentGuildInfo = this.m_Model.CurrentGuildInfo;
                if (currentGuildInfo == null)
                {
                    DebugHelper.Assert(false, "GuildInfo is null!!!");
                }
                else
                {
                    this.m_form.GetWidget(0x16).CustomSetActive(true);
                    Image component = this.m_form.GetWidget(3).GetComponent<Image>();
                    Text text = this.m_form.GetWidget(4).GetComponent<Text>();
                    Transform panelTransform = this.m_form.GetWidget(14).get_transform();
                    Text text2 = this.m_form.GetWidget(5).GetComponent<Text>();
                    string prefabPath = CUIUtility.s_Sprite_Dynamic_GuildHead_Dir + currentGuildInfo.briefInfo.dwHeadId;
                    component.SetSprite(prefabPath, this.m_form, true, false, false, false);
                    text.set_text(currentGuildInfo.briefInfo.sName);
                    CGuildHelper.SetStarLevelPanel(currentGuildInfo.star, panelTransform, this.m_form);
                    text2.set_text(currentGuildInfo.chairman.stBriefInfo.sName);
                }
            }
        }

        public void RefreshGuildMatchPanel()
        {
            if (this.m_form != null)
            {
                this.m_form.GetWidget(0x29).CustomSetActive(Singleton<CGuildMatchSystem>.GetInstance().IsInGuildMatchTime());
                GameObject widget = this.m_form.GetWidget(20);
                Singleton<CUINewFlagSystem>.GetInstance().AddNewFlag(widget, enNewFlagKey.New_GuildMatch_V1, enNewFlagPos.enTopLeft, 1f, 0f, 0f, enNewFlagType.enNewFlag);
            }
        }

        public void RefreshGuildName()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Guild/Form_Guild_Setting.prefab");
            if (form != null)
            {
                form.GetWidget(5).GetComponent<Text>().set_text(this.m_Model.CurrentGuildInfo.briefInfo.sName);
            }
            if (this.m_form != null)
            {
                this.m_form.GetWidget(4).GetComponent<Text>().set_text(this.m_Model.CurrentGuildInfo.briefInfo.sName);
            }
        }

        public void RefreshInfoPanel()
        {
            if ((this.m_form != null) && (this.CurTab == Tab.GuildInfo))
            {
                this.m_form.GetWidget(0).CustomSetActive(true);
                this.RefreshWeekRankNumPanel();
                this.RefreshInfoPanelSignBtn();
                this.RefreshRankpointMemberListPanel();
                this.RefreshRankpointInfoPanel();
                this.RefreshGuildMatchPanel();
                this.RefreshFacePanel();
                if (CSysDynamicBlock.bLobbyEntryBlocked)
                {
                    GameObject widget = this.m_form.GetWidget(20);
                    if (widget != null)
                    {
                        widget.CustomSetActive(false);
                    }
                }
            }
        }

        public void RefreshInfoPanelGuildBulletin()
        {
            if (this.m_form != null)
            {
                this.m_form.GetWidget(7).GetComponent<Text>().set_text(this.m_Model.CurrentGuildInfo.briefInfo.sBulletin);
            }
        }

        public void RefreshInfoPanelGuildIcon()
        {
            if (this.m_form != null)
            {
                Image component = this.m_form.GetWidget(3).GetComponent<Image>();
                string prefabPath = CUIUtility.s_Sprite_Dynamic_GuildHead_Dir + this.m_Model.CurrentGuildInfo.briefInfo.dwHeadId;
                component.SetSprite(prefabPath, this.m_form, true, false, false, false);
            }
        }

        public void RefreshInfoPanelGuildMemberCount()
        {
            if (this.m_form != null)
            {
                GameObject widget = this.m_form.GetWidget(6);
                if (widget == null)
                {
                    DebugHelper.Assert(false, "CGuildInfoView.RefreshInfoPanelGuildMemberCount(): guildMemberCountGo is null!!!");
                }
                else
                {
                    string[] args = new string[] { this.m_Model.CurrentGuildInfo.briefInfo.bMemberNum.ToString(), CGuildHelper.GetMaxGuildMemberCountByLevel(this.m_Model.CurrentGuildInfo.briefInfo.bLevel).ToString() };
                    widget.GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("Guild_Member_Count_Format", args));
                    this.m_form.GetWidget(11).CustomSetActive(!CGuildHelper.IsGuildMaxLevel(this.m_Model.CurrentGuildInfo.briefInfo.bLevel));
                }
            }
        }

        public void RefreshInfoPanelSignBtn()
        {
            if (this.m_form != null)
            {
                GameObject widget = this.m_form.GetWidget(0x11);
                Button component = widget.GetComponent<Button>();
                if (CGuildHelper.IsPlayerSigned())
                {
                    CUICommonSystem.DelRedDot(widget);
                    CUICommonSystem.SetButtonEnable(component, false, false, true);
                    CUICommonSystem.SetButtonName(widget, Singleton<CTextManager>.GetInstance().GetText("Common_Signed"));
                }
                else
                {
                    CUICommonSystem.AddRedDot(widget, enRedDotPos.enTopRight, 0);
                    CUICommonSystem.SetButtonEnable(component, true, true, true);
                    Text componentInChildren = widget.GetComponentInChildren<Text>();
                    if (componentInChildren != null)
                    {
                        componentInChildren.set_color(s_Text_Color_Btn_Sign_Default);
                    }
                    CUICommonSystem.SetButtonName(widget, Singleton<CTextManager>.GetInstance().GetText("Common_Sign"));
                }
            }
        }

        public void RefreshMemberCntPanel()
        {
            if (this.m_form != null)
            {
                GuildInfo currentGuildInfo = this.m_Model.CurrentGuildInfo;
                if (currentGuildInfo == null)
                {
                    DebugHelper.Assert(false, "GuildInfo is null!!!");
                }
                else
                {
                    this.m_form.GetWidget(11).CustomSetActive(!CGuildHelper.IsGuildMaxLevel(currentGuildInfo.briefInfo.bLevel));
                    string[] args = new string[] { currentGuildInfo.briefInfo.bMemberNum.ToString(), CGuildHelper.GetMaxGuildMemberCountByLevel(currentGuildInfo.briefInfo.bLevel).ToString() };
                    this.m_form.GetWidget(6).GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("Guild_Member_Count_Format", args));
                }
            }
        }

        private void RefreshMemberList()
        {
            if (this.m_form != null)
            {
                this.m_form.GetWidget(1).CustomSetActive(true);
                CUIListScript component = this.m_form.GetWidget(9).GetComponent<CUIListScript>();
                int bMemberNum = this.m_Model.CurrentGuildInfo.briefInfo.bMemberNum;
                if (bMemberNum > 0)
                {
                    this.SortGuildMemberList();
                }
                component.SetElementAmount(bMemberNum);
                component.m_alwaysDispatchSelectedChangeEvent = true;
                this.DisplayGuildMemberListSortBtn();
            }
        }

        public void RefreshMemberPanel()
        {
            if ((this.m_form != null) && (this.CurTab == Tab.GuildMember))
            {
                this.RefreshMemberList();
                this.RefreshMemberCntPanel();
                this.RefreshMemberPanelApplyBtnRedDot();
                this.m_form.GetWidget(0x15).CustomSetActive(CGuildSystem.HasWirteGuildMailAuthority());
                this.m_form.GetWidget(0x26).CustomSetActive(CGuildSystem.HasManageAuthority());
                GameObject widget = this.m_form.GetWidget(0x27);
                long num2 = this.GetGuildSendRecruitCd() - (CRoleInfo.GetCurrentUTCTime() - this.m_lastSendReruitTime);
                if ((this.m_lastSendReruitTime > 0) && (num2 > 0L))
                {
                    widget.CustomSetActive(true);
                    CUITimerScript component = widget.GetComponent<CUITimerScript>();
                    component.SetTotalTime((float) num2);
                    component.StartTimer();
                    this.m_form.GetWidget(40).CustomSetActive(false);
                }
                else
                {
                    widget.CustomSetActive(false);
                }
                this.m_form.GetWidget(8).CustomSetActive(CGuildSystem.HasManageAuthority());
                this.RefreshBulletinPanel();
                this.RefreshQQGroupPanel();
                this.RefreshFacePanel();
            }
        }

        public void RefreshMemberPanelApplyBtnRedDot()
        {
            if (this.m_form != null)
            {
                GameObject widget = this.m_form.GetWidget(13);
                if (this.IsShowApplyRedDot())
                {
                    CUICommonSystem.AddRedDot(widget, enRedDotPos.enTopRight, 0);
                }
                else
                {
                    CUICommonSystem.DelRedDot(widget);
                }
            }
        }

        public void RefreshMemberTabRedDot()
        {
            if (this.m_form != null)
            {
                GameObject target = this.m_form.GetWidget(2).GetComponent<CUIListScript>().GetElemenet(1).get_gameObject();
                if (this.IsShowApplyRedDot())
                {
                    CUICommonSystem.AddRedDot(target, enRedDotPos.enTopRight, 0);
                }
                else
                {
                    CUICommonSystem.DelRedDot(target);
                }
            }
        }

        public void RefreshPreviewForm(bool isHideListExtraContent = false)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Guild/Form_Guild_Preview.prefab");
            if (form != null)
            {
                GameObject widget = form.GetWidget(0);
                GameObject obj3 = form.GetWidget(1);
                GameObject obj4 = form.GetWidget(2);
                form.GetWidget(3).CustomSetActive(false);
                CUIListScript component = widget.GetComponent<CUIListScript>();
                int guildInfoCount = this.m_Model.GetGuildInfoCount();
                component.SetElementAmount(guildInfoCount);
                if (guildInfoCount > 0)
                {
                    component.SelectElement(0, true);
                    obj3.CustomSetActive(true);
                    obj4.CustomSetActive(true);
                }
                else
                {
                    obj3.CustomSetActive(false);
                    obj4.CustomSetActive(false);
                }
                if (isHideListExtraContent)
                {
                    component.HideExtraContent();
                }
            }
        }

        public void RefreshProfitPanel()
        {
            string[] args = new string[] { CGuildHelper.GetCoinProfitPercentage(CGuildHelper.GetGuildLevel()).ToString() };
            this.m_form.GetWidget(12).GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("Guild_Profit_Desc", args));
        }

        public void RefreshQQGroupPanel()
        {
            if (this.m_form != null)
            {
                this.m_form.GetWidget(0x13).CustomSetActive(false);
                if ((Singleton<ApolloHelper>.GetInstance().CurPlatform == ApolloPlatform.QQ) && (GameDataMgr.guildMiscDatabin.GetDataByKey((uint) 0x2d).dwConfValue > 0))
                {
                    Singleton<EventRouter>.GetInstance().BroadCastEvent("Guild_QQGroup_Refresh_QQGroup_Panel");
                }
            }
        }

        public void RefreshQQGroupPanel(bool isBound, string groupName = "", bool isSelfInGroup = false)
        {
            if (this.m_form != null)
            {
                this.m_form.GetWidget(0x13).CustomSetActive(true);
                Text component = this.m_form.GetWidget(15).GetComponent<Text>();
                Text text2 = this.m_form.GetWidget(0x12).GetComponent<Text>();
                CUIEventScript script = this.m_form.GetWidget(0x10).GetComponent<CUIEventScript>();
                if (isBound)
                {
                    component.set_text(Singleton<CTextManager>.GetInstance().GetText("Guild_QQ_Group") + "\n" + groupName);
                    if (CGuildSystem.HasManageQQGroupAuthority())
                    {
                        text2.set_text(Singleton<CTextManager>.GetInstance().GetText("Guild_QQ_Group_Unbind"));
                        script.SetUIEvent(enUIEventType.Click, enUIEventID.Guild_UnBindQQGroup);
                        script.get_gameObject().CustomSetActive(true);
                    }
                    else if (isSelfInGroup)
                    {
                        script.get_gameObject().CustomSetActive(false);
                    }
                    else
                    {
                        text2.set_text(Singleton<CTextManager>.GetInstance().GetText("Guild_QQ_Group_Add") + "\n" + groupName);
                        script.SetUIEvent(enUIEventType.Click, enUIEventID.Guild_JoinQQGroup);
                        script.get_gameObject().CustomSetActive(true);
                    }
                }
                else
                {
                    component.set_text(Singleton<CTextManager>.GetInstance().GetText("Guild_QQ_Group_Not_Bind"));
                    if (CGuildSystem.HasManageQQGroupAuthority())
                    {
                        text2.set_text(Singleton<CTextManager>.GetInstance().GetText("Guild_QQ_Group_Bind"));
                        script.SetUIEvent(enUIEventType.Click, enUIEventID.Guild_BindQQGroup);
                        script.get_gameObject().CustomSetActive(true);
                    }
                    else
                    {
                        script.get_gameObject().CustomSetActive(false);
                    }
                }
            }
        }

        public void RefreshRankpointInfoPanel()
        {
            if (this.m_form != null)
            {
                Text component = this.m_form.GetWidget(0x1f).GetComponent<Text>();
                Image image = this.m_form.GetWidget(0x19).GetComponent<Image>();
                Text text2 = this.m_form.GetWidget(0x1a).GetComponent<Text>();
                GameObject widget = this.m_form.GetWidget(0x1b);
                GameObject awardPanel = this.m_form.GetWidget(0x1c);
                Text text3 = this.m_form.GetWidget(0x1d).GetComponent<Text>();
                Text text4 = this.m_form.GetWidget(30).GetComponent<Text>();
                component.set_text(this.m_Model.CurrentGuildInfo.RankInfo.totalRankPoint.ToString());
                image.SetSprite(CGuildHelper.GetGradeIconPathByRankpointScore(this.m_Model.CurrentGuildInfo.RankInfo.totalRankPoint), this.m_form, true, false, false, false);
                text2.set_text(CGuildHelper.GetGradeName(this.m_Model.CurrentGuildInfo.RankInfo.totalRankPoint));
                this.RefreshProfitPanel();
                RankpointRankInfo playerGuildRankpointRankInfo = CGuildHelper.GetPlayerGuildRankpointRankInfo(enGuildRankpointRankListType.CurrentWeek);
                this.SetRankpointAwardPanel(this.m_form, widget, true, playerGuildRankpointRankInfo.rankNo, 0);
                this.SetRankpointAwardPanel(this.m_form, awardPanel, false, 0, CGuildHelper.GetGuildGrade());
                GuildMemInfo playerGuildMemberInfo = this.m_Model.GetPlayerGuildMemberInfo();
                if (playerGuildMemberInfo != null)
                {
                    text3.set_text(playerGuildMemberInfo.RankInfo.maxRankPoint.ToString());
                }
                text4.set_text(CGuildHelper.GetRankpointClearTimeFormatString());
            }
        }

        public void RefreshRankpointMemberListPanel()
        {
            if (this.m_form != null)
            {
                this.InitRankpointMemberList();
                CUIListScript component = this.m_form.GetWidget(0x17).GetComponent<CUIListScript>();
                component.SetElementAmount(this.m_Model.RankpointMemberInfoList.Count);
                component.m_alwaysDispatchSelectedChangeEvent = true;
                this.SetRankpointMemberPlayerItem();
            }
        }

        public void RefreshRankpointRankList(CUIEvent uiEvent = new CUIEvent())
        {
            CUIFormScript form;
            CUIListScript component;
            if (uiEvent == null)
            {
                form = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Guild/Form_Guild_RankpointRank.prefab");
                if (form == null)
                {
                    return;
                }
                component = form.GetWidget(5).GetComponent<CUIListScript>();
            }
            else
            {
                form = uiEvent.m_srcFormScript;
                component = uiEvent.m_srcWidget.GetComponent<CUIListScript>();
            }
            int selectedIndex = component.GetSelectedIndex();
            GameObject widget = form.GetWidget(8);
            switch (((enGuildRankpointRankListTab) selectedIndex))
            {
                case enGuildRankpointRankListTab.CurrentWeekRank:
                    widget.CustomSetActive(false);
                    this.SetRankpointRankList(enGuildRankpointRankListType.CurrentWeek, form);
                    break;

                case enGuildRankpointRankListTab.LastWeekRank:
                    widget.CustomSetActive(false);
                    this.SetRankpointRankList(enGuildRankpointRankListType.LastWeek, form);
                    break;

                case enGuildRankpointRankListTab.SeasonRank:
                    widget.CustomSetActive(true);
                    widget.GetComponent<CUIListScript>().SelectElement(0, true);
                    break;
            }
        }

        public void RefreshRankpointSeasonRankList(CUIEvent uiEvent = new CUIEvent())
        {
            CUIFormScript form;
            CUIListScript component;
            if (uiEvent == null)
            {
                form = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Guild/Form_Guild_RankpointRank.prefab");
                if (form == null)
                {
                    return;
                }
                component = form.GetWidget(8).GetComponent<CUIListScript>();
            }
            else
            {
                form = uiEvent.m_srcFormScript;
                component = uiEvent.m_srcWidget.GetComponent<CUIListScript>();
            }
            switch (component.GetSelectedIndex())
            {
                case 0:
                    this.SetRankpointRankList(enGuildRankpointRankListType.SeasonSelf, form);
                    break;

                case 1:
                    this.SetRankpointRankList(enGuildRankpointRankListType.SeasonBest, form);
                    break;
            }
        }

        public void RefreshSettingBtn()
        {
            if (this.m_form != null)
            {
                this.m_form.GetWidget(8).CustomSetActive(CGuildSystem.HasManageAuthority());
            }
        }

        public void RefreshWeekRankNumPanel()
        {
            GameObject widget = this.m_form.GetWidget(0x20);
            GameObject obj3 = this.m_form.GetWidget(0x21);
            RankpointRankInfo playerGuildRankpointRankInfo = CGuildHelper.GetPlayerGuildRankpointRankInfo(enGuildRankpointRankListType.CurrentWeek);
            if (playerGuildRankpointRankInfo.rankNo > 0)
            {
                widget.CustomSetActive(true);
                obj3.CustomSetActive(false);
                widget.GetComponent<Text>().set_text(playerGuildRankpointRankInfo.rankNo.ToString());
            }
            else
            {
                obj3.CustomSetActive(true);
                widget.CustomSetActive(false);
            }
        }

        private void SetApplicantItem(CUIListElementScript listElementScript, stApplicantInfo info, bool isAdmin)
        {
            int currentUTCTime = CRoleInfo.GetCurrentUTCTime();
            Transform transform = listElementScript.get_transform();
            Text component = transform.Find("txtUidData").GetComponent<Text>();
            CUIHttpImageScript script = transform.Find("imgHead").GetComponent<CUIHttpImageScript>();
            Image image = script.get_transform().Find("NobeIcon").GetComponent<Image>();
            Image image2 = script.get_transform().Find("NobeImag").GetComponent<Image>();
            Text text2 = transform.Find("txtName").GetComponent<Text>();
            Text text3 = transform.Find("txtLevel").GetComponent<Text>();
            Image image3 = transform.Find("imgGender").GetComponent<Image>();
            GameObject go = transform.Find("RankCon").get_gameObject();
            GameObject obj3 = transform.Find("btnGroup").get_gameObject();
            CUITimerScript script2 = transform.Find("Timer").GetComponent<CUITimerScript>();
            Text text4 = transform.Find("txtRecommender").GetComponent<Text>();
            script.SetImageUrl(CGuildHelper.GetHeadUrl(info.stBriefInfo.szHeadUrl));
            MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(image, (int) info.stBriefInfo.stVip.level, false);
            MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(image2, (int) info.stBriefInfo.stVip.headIconId);
            component.set_text(info.stBriefInfo.uulUid.ToString());
            text2.set_text(info.stBriefInfo.sName);
            string[] args = new string[] { info.stBriefInfo.dwLevel.ToString() };
            string text = Singleton<CTextManager>.GetInstance().GetText("Guild_Rankpoint_Level_Format", args);
            text3.set_text(text);
            text3.set_text(text);
            CUIUtility.SetGenderImageSprite(image3, info.stBriefInfo.gender);
            int num2 = CLadderSystem.ConvertEloToRank(info.stBriefInfo.dwScoreOfRank);
            CLadderView.ShowRankDetail(go, (byte) num2, info.stBriefInfo.dwClassOfRank, false, true);
            if (isAdmin)
            {
                obj3.CustomSetActive(true);
            }
            else
            {
                obj3.CustomSetActive(false);
            }
            int num3 = 0;
            int dwConfValue = (int) GameDataMgr.guildMiscDatabin.GetDataByKey((uint) 9).dwConfValue;
            if (info.dwApplyTime == 0)
            {
                num3 = dwConfValue;
            }
            else
            {
                num3 = (info.dwApplyTime + dwConfValue) - currentUTCTime;
                if (num3 < 0)
                {
                    num3 = 0;
                }
            }
            script2.SetTotalTime((float) num3);
            script2.StartTimer();
            text4.set_text(string.Empty);
        }

        private void SetGuildIcon(CUIListElementScript listElementScript, ResGuildIcon info)
        {
            string prefabPath = CUIUtility.s_Sprite_Dynamic_GuildHead_Dir + info.dwIcon;
            listElementScript.get_transform().Find("imgIcon").GetComponent<Image>().SetSprite(prefabPath, listElementScript.m_belongedFormScript, true, false, false, false);
            listElementScript.get_transform().Find("imgIcon/txtIconIdData").GetComponent<Text>().set_text(info.dwIcon.ToString());
        }

        private void SetGuildMemberListItem(CUIListElementScript listElementScript, GuildMemInfo memberInfo)
        {
            Transform transform = listElementScript.get_transform();
            CUIHttpImageScript component = transform.Find("imgHead").GetComponent<CUIHttpImageScript>();
            Image image = component.get_transform().Find("NobeIcon").GetComponent<Image>();
            Image image2 = component.get_transform().Find("NobeImag").GetComponent<Image>();
            Text text = transform.Find("txtName").GetComponent<Text>();
            GameObject obj2 = transform.Find("pnlPosition/imgPositionChairman").get_gameObject();
            GameObject obj3 = transform.Find("pnlPosition/imgPositionOther").get_gameObject();
            Text text2 = transform.Find("pnlPosition/txtPosition").GetComponent<Text>();
            GameObject obj4 = transform.Find("pnlPosition/imgLeader").get_gameObject();
            Text text3 = transform.Find("txtWeekRankpoint").GetComponent<Text>();
            Text text4 = transform.Find("txtSeasonRankpoint").GetComponent<Text>();
            Text text5 = transform.Find("txtOnline").GetComponent<Text>();
            component.SetImageUrl(CGuildHelper.GetHeadUrl(memberInfo.stBriefInfo.szHeadUrl));
            MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(image, CGuildHelper.GetNobeLevel(memberInfo.stBriefInfo.uulUid, memberInfo.stBriefInfo.stVip.level), false);
            MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(image2, CGuildHelper.GetNobeHeadIconId(memberInfo.stBriefInfo.uulUid, memberInfo.stBriefInfo.stVip.headIconId));
            text.set_text(memberInfo.stBriefInfo.sName);
            obj2.CustomSetActive(COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_CHAIRMAN == memberInfo.enPosition);
            obj3.CustomSetActive(COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_CHAIRMAN != memberInfo.enPosition);
            text2.set_text(CGuildHelper.GetPositionName(memberInfo.enPosition));
            obj4.CustomSetActive(Convert.ToBoolean(memberInfo.GuildMatchInfo.bIsLeader));
            text3.set_text(memberInfo.RankInfo.weekRankPoint.ToString());
            text4.set_text(memberInfo.RankInfo.totalRankPoint.ToString());
            if (CGuildHelper.IsMemberOnline(memberInfo))
            {
                text5.set_text(Singleton<CTextManager>.GetInstance().GetText("Common_Online"));
            }
            else
            {
                text5.set_text(Utility.GetRecentOnlineTimeString((int) memberInfo.LastLoginTime));
            }
        }

        private void SetPreviewGuildListItem(CUIListElementScript listElementScript, GuildInfo info)
        {
            Transform transform = listElementScript.get_transform();
            Image component = transform.Find("imgIconBg/imgIcon").GetComponent<Image>();
            Text text = transform.Find("pnlName/txtName").GetComponent<Text>();
            Image image = transform.Find("pnlName/imgGrade").GetComponent<Image>();
            Text text2 = transform.Find("txtMemCnt").GetComponent<Text>();
            GameObject obj2 = transform.Find("imgInvited").get_gameObject();
            Text text3 = transform.Find("txtChairmanName").GetComponent<Text>();
            Transform panelTransform = transform.Find("pnlStarLevel");
            Text text4 = transform.Find("pnlPoint/txtPointNum").GetComponent<Text>();
            string prefabPath = CUIUtility.s_Sprite_Dynamic_GuildHead_Dir + info.briefInfo.dwHeadId;
            component.SetSprite(prefabPath, this.m_form, true, false, false, false);
            text.set_text(info.briefInfo.sName);
            image.SetSprite(CGuildHelper.GetGradeIconPathByRankpointScore(info.RankInfo.totalRankPoint), this.m_form, true, false, false, false);
            text2.set_text(info.briefInfo.bMemberNum + "/" + CGuildHelper.GetMaxGuildMemberCountByLevel(info.briefInfo.bLevel));
            text3.set_text(info.chairman.stBriefInfo.sName);
            CGuildHelper.SetStarLevelPanel(info.star, panelTransform, listElementScript.m_belongedFormScript);
            text4.set_text(info.RankInfo.totalRankPoint.ToString());
            if ((info.briefInfo.uulUid == this.m_Model.m_InviteGuildUuid) && !Singleton<CGuildSystem>.GetInstance().IsInNormalGuild())
            {
                obj2.CustomSetActive(true);
            }
            else
            {
                obj2.CustomSetActive(false);
            }
        }

        private void SetRankpointAwardPanel(CUIFormScript form, GameObject awardPanel, bool isWeekAward, uint rankNo, uint grade)
        {
            if ((form != null) && (awardPanel != null))
            {
                Transform transform = awardPanel.get_transform();
                Image component = transform.Find("imgMoney").GetComponent<Image>();
                Text text = transform.Find("txtMoney").GetComponent<Text>();
                if ((component != null) && (text != null))
                {
                    if (isWeekAward)
                    {
                        component.SetSprite(CUIUtility.s_Sprite_Dynamic_Icon_Dir + "90001", form, true, false, false, false);
                        text.set_text(CGuildHelper.GetRankpointWeekAwardCoin(rankNo).ToString());
                    }
                    else
                    {
                        component.SetSprite(CUIUtility.s_Sprite_Dynamic_Icon_Dir + "90005", form, true, false, false, false);
                        text.set_text(CGuildHelper.GetRankpointSeasonAwardDiamond(grade).ToString());
                    }
                }
            }
        }

        private void SetRankpointGuildRankListItem(CUIListElementScript listElement, RankpointRankInfo info, enGuildRankpointRankListType rankListType, uint firstRankNo)
        {
            if (listElement == null)
            {
                DebugHelper.Assert(false, "listElement is null!!!");
            }
            else if (info == null)
            {
                DebugHelper.Assert(false, "rankpointRankInfo is null!!!");
            }
            else
            {
                Image component = listElement.get_transform().Find("imgGuildIconBg/imgGuildIcon").GetComponent<Image>();
                Text text = listElement.get_transform().Find("txtGuildName").GetComponent<Text>();
                Text text2 = listElement.get_transform().Find("pnlPoint/txtPointNum").GetComponent<Text>();
                Text text3 = listElement.get_transform().Find("txtMemberCount").GetComponent<Text>();
                Transform panelTransform = listElement.get_transform().Find("starLevel").get_transform();
                Image image = listElement.get_transform().Find("imgGrade").GetComponent<Image>();
                Transform rankTransform = listElement.get_transform().Find("rank").get_transform();
                CUICommonSystem.SetRankDisplay(info.rankNo - this.CalculateRankNoOffset(rankListType, firstRankNo), rankTransform);
                component.SetSprite(CUIUtility.s_Sprite_Dynamic_GuildHead_Dir + info.guildHeadId, listElement.m_belongedFormScript, true, false, false, false);
                text.set_text(info.guildName);
                text2.set_text(info.rankScore.ToString());
                string[] args = new string[] { info.memberNum.ToString(), CGuildHelper.GetMaxGuildMemberCountByLevel(info.guildLevel).ToString() };
                text3.set_text(Singleton<CTextManager>.GetInstance().GetText("Guild_Rankpoint_Member_Format", args));
                CGuildHelper.SetStarLevelPanel(info.star, panelTransform, listElement.m_belongedFormScript);
                image.SetSprite(CGuildHelper.GetGradeIconPathByRankpointScore(info.rankScore), listElement.m_belongedFormScript, true, false, false, false);
                GameObject awardPanel = listElement.get_transform().Find("pnlAward").get_gameObject();
                this.SetRankpointAwardPanel(listElement.m_belongedFormScript, awardPanel, CGuildHelper.IsWeekRankpointRank(rankListType), info.rankNo, CGuildHelper.GetGradeByRankpointScore(info.rankScore));
            }
        }

        private void SetRankpointMemberItem(Transform itemTransform, GuildMemInfo info, int elementIndex)
        {
            if (info == null)
            {
                DebugHelper.Assert(false, "guildMemInfo is null!!!");
            }
            else
            {
                CUIHttpImageScript component = itemTransform.Find("imgMemberIcon").GetComponent<CUIHttpImageScript>();
                Image image = component.get_transform().Find("NobeIcon").GetComponent<Image>();
                Image image2 = component.get_transform().Find("NobeImag").GetComponent<Image>();
                Text text = itemTransform.Find("txtMemberName").GetComponent<Text>();
                Text text2 = itemTransform.Find("txtSeasonPointNum").GetComponent<Text>();
                Text text3 = itemTransform.Find("txtWeekPointNum").GetComponent<Text>();
                Transform rankTransform = itemTransform.Find("rank").get_transform();
                CUICommonSystem.SetRankDisplay((uint) (elementIndex + 1), rankTransform);
                component.SetImageUrl(CGuildHelper.GetHeadUrl(info.stBriefInfo.szHeadUrl));
                MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(image, CGuildHelper.GetNobeLevel(info.stBriefInfo.uulUid, info.stBriefInfo.stVip.level), false);
                MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(image2, CGuildHelper.GetNobeHeadIconId(info.stBriefInfo.uulUid, info.stBriefInfo.stVip.headIconId));
                text.set_text(info.stBriefInfo.sName);
                text2.set_text(info.RankInfo.totalRankPoint.ToString());
                text3.set_text(info.RankInfo.weekRankPoint.ToString());
            }
        }

        private void SetRankpointMemberPlayerItem()
        {
            Transform itemTransform = this.m_form.GetWidget(0x18).get_transform();
            GuildMemInfo guildMemberInfoByUid = this.m_Model.GetGuildMemberInfoByUid(Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID);
            int rankpointMemberListPlayerIndex = CGuildHelper.GetRankpointMemberListPlayerIndex();
            this.SetRankpointMemberItem(itemTransform, guildMemberInfoByUid, rankpointMemberListPlayerIndex);
        }

        public void SetRankpointPlayerGuildRank(RankpointRankInfo info, CUIFormScript rankForm = new CUIFormScript())
        {
            if (rankForm == null)
            {
                rankForm = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Guild/Form_Guild_RankpointRank.prefab");
            }
            if ((rankForm != null) && (info != null))
            {
                Transform rankTransform = rankForm.GetWidget(7).get_transform();
                Image component = rankForm.GetWidget(0).GetComponent<Image>();
                Text text = rankForm.GetWidget(1).GetComponent<Text>();
                Text text2 = rankForm.GetWidget(2).GetComponent<Text>();
                Text text3 = rankForm.GetWidget(3).GetComponent<Text>();
                Transform panelTransform = rankForm.GetWidget(9).get_transform();
                Image image = rankForm.GetWidget(10).GetComponent<Image>();
                enGuildRankpointRankListType rankListTypeByTabSelectedIndex = this.GetRankListTypeByTabSelectedIndex(rankForm);
                if (((rankListTypeByTabSelectedIndex == enGuildRankpointRankListType.SeasonSelf) && (info.rankNo > 0)) && (this.m_Model.RankpointRankInfoLists[(int) rankListTypeByTabSelectedIndex].Count > 0))
                {
                    uint rankNo = this.m_Model.RankpointRankInfoLists[(int) rankListTypeByTabSelectedIndex][0].rankNo;
                    CUICommonSystem.SetRankDisplay(info.rankNo - this.CalculateRankNoOffset(rankListTypeByTabSelectedIndex, rankNo), rankTransform);
                }
                else
                {
                    CUICommonSystem.SetRankDisplay(info.rankNo, rankTransform);
                }
                component.SetSprite(CUIUtility.s_Sprite_Dynamic_GuildHead_Dir + info.guildHeadId, rankForm, true, false, false, false);
                text.set_text(info.guildName);
                text2.set_text(info.rankScore.ToString());
                string[] args = new string[] { info.memberNum.ToString(), CGuildHelper.GetMaxGuildMemberCountByLevel(info.guildLevel).ToString() };
                text3.set_text(Singleton<CTextManager>.GetInstance().GetText("Guild_Rankpoint_Member_Format", args));
                CGuildHelper.SetStarLevelPanel(info.star, panelTransform, rankForm);
                image.SetSprite(CGuildHelper.GetGradeIconPathByRankpointScore(info.rankScore), rankForm, true, false, false, false);
                GameObject awardPanel = rankForm.GetWidget(4).get_gameObject();
                this.SetRankpointAwardPanel(rankForm, awardPanel, CGuildHelper.IsWeekRankpointRank(rankListTypeByTabSelectedIndex), info.rankNo, CGuildHelper.GetGradeByRankpointScore(info.rankScore));
            }
        }

        private void SetRankpointRankList(enGuildRankpointRankListType rankListType, CUIFormScript rankForm)
        {
            if (this.m_Model.RankpointRankGottens[(int) rankListType])
            {
                CUIListScript component = rankForm.GetWidget(6).GetComponent<CUIListScript>();
                component.SetElementAmount(this.m_Model.RankpointRankInfoLists[(int) rankListType].Count);
                if (component.GetElementAmount() > 0)
                {
                    component.SelectElement(0, true);
                }
                RankpointRankInfo playerGuildRankpointRankInfo = CGuildHelper.GetPlayerGuildRankpointRankInfo(rankListType);
                this.SetRankpointPlayerGuildRank(playerGuildRankpointRankInfo, rankForm);
            }
            else
            {
                this.ClearRankpointRankList(rankForm);
                Singleton<EventRouter>.GetInstance().BroadCastEvent<enGuildRankpointRankListType>("Guild_Request_Rankpoint_Rank_List", rankListType);
            }
        }

        private void SetRankpointRankListWidgetPositionAndSize(CUIEvent uiEvent)
        {
            CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
            if (srcFormScript != null)
            {
                RectTransform transform = srcFormScript.GetWidget(6).get_transform() as RectTransform;
                int selectedIndex = srcFormScript.GetWidget(5).GetComponent<CUIListScript>().GetSelectedIndex();
                float num2 = (srcFormScript.GetWidget(8).get_transform() as RectTransform).get_rect().get_height() / 2f;
                switch (selectedIndex)
                {
                    case 0:
                    case 1:
                        transform.set_anchoredPosition(new Vector2(transform.get_anchoredPosition().x, s_rankpointRankListWidgetAnchoredPositionY + num2));
                        transform.set_sizeDelta(new Vector2(transform.get_sizeDelta().x, s_rankpointRankListWidgetSizeDeltaY + num2));
                        break;

                    default:
                        if (selectedIndex == 2)
                        {
                            transform.set_anchoredPosition(new Vector2(transform.get_anchoredPosition().x, s_rankpointRankListWidgetAnchoredPositionY));
                            transform.set_sizeDelta(new Vector2(transform.get_sizeDelta().x, s_rankpointRankListWidgetSizeDeltaY));
                        }
                        break;
                }
            }
        }

        private void SetRecommendItem(CUIListElementScript listElementScript, stRecommendInfo info, bool isAdmin)
        {
            Transform transform = listElementScript.get_transform();
            Text component = transform.Find("txtUidData").GetComponent<Text>();
            CUIHttpImageScript script = transform.Find("imgHead").GetComponent<CUIHttpImageScript>();
            Image image = script.get_transform().Find("NobeIcon").GetComponent<Image>();
            Image image2 = script.get_transform().Find("NobeImag").GetComponent<Image>();
            Text text2 = transform.Find("txtName").GetComponent<Text>();
            Text text3 = transform.Find("txtLevel").GetComponent<Text>();
            Image image3 = transform.Find("imgGender").GetComponent<Image>();
            GameObject go = transform.Find("RankCon").get_gameObject();
            Text text4 = transform.Find("txtRecommender").GetComponent<Text>();
            GameObject obj3 = transform.Find("btnGroup").get_gameObject();
            GameObject obj4 = transform.Find("btnGroup/btnPass").get_gameObject();
            GameObject obj5 = transform.Find("btnGroup/btnReject").get_gameObject();
            script.SetImageUrl(CGuildHelper.GetHeadUrl(info.headUrl));
            MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(image, (int) info.stVip.level, false);
            MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(image2, (int) info.stVip.headIconId);
            component.set_text(info.uid.ToString());
            text2.set_text(info.name);
            string[] args = new string[] { info.level.ToString() };
            text3.set_text(Singleton<CTextManager>.GetInstance().GetText("Guild_Rankpoint_Level_Format", args));
            CUIUtility.SetGenderImageSprite(image3, info.gender);
            int num = CLadderSystem.ConvertEloToRank(info.rankScore);
            CLadderView.ShowRankDetail(go, (byte) num, info.rankClass, false, true);
            text4.set_text(info.recommendName);
            if (isAdmin)
            {
                obj3.CustomSetActive(true);
                obj4.GetComponent<CUIEventScript>().SetUIEvent(enUIEventType.Click, enUIEventID.Guild_Recommend_Invite);
                obj4.get_transform().Find("Text").GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("Common_Invite"));
                obj5.GetComponent<CUIEventScript>().SetUIEvent(enUIEventType.Click, enUIEventID.Guild_Recommend_Reject);
            }
            else
            {
                obj3.CustomSetActive(false);
            }
        }

        private void SortGuildMemberList()
        {
            switch (this.m_curGuildMemberListSortType)
            {
                case enGuildMemberListSortType.Default:
                    this.m_Model.CurrentGuildInfo.listMemInfo.Sort(new Comparison<GuildMemInfo>(this.SortGuildMemberListPositionDesc));
                    break;

                case enGuildMemberListSortType.PositionDesc:
                    this.m_Model.CurrentGuildInfo.listMemInfo.Sort(new Comparison<GuildMemInfo>(this.SortGuildMemberListPositionDesc));
                    break;

                case enGuildMemberListSortType.WeekRankpointDesc:
                    this.m_Model.CurrentGuildInfo.listMemInfo.Sort(new Comparison<GuildMemInfo>(this.SortGuildMemberListWeekRankpointDesc));
                    break;

                case enGuildMemberListSortType.SeasonRankpointDesc:
                    this.m_Model.CurrentGuildInfo.listMemInfo.Sort(new Comparison<GuildMemInfo>(this.SortGuildMemberListSeasonRankpointDesc));
                    break;

                case enGuildMemberListSortType.OnlineStatus:
                    this.m_Model.CurrentGuildInfo.listMemInfo.Sort(new Comparison<GuildMemInfo>(this.SortGuildMemberListOnlineStatusDesc));
                    break;
            }
        }

        private int SortGuildMemberListDefault(GuildMemInfo info1, GuildMemInfo info2)
        {
            if ((info1.stBriefInfo.uulUid == Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID) && (info2.stBriefInfo.uulUid != Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID))
            {
                return -1;
            }
            if ((info1.stBriefInfo.uulUid != Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID) && (info2.stBriefInfo.uulUid == Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID))
            {
                return 1;
            }
            if (info1.enPosition.CompareTo(info2.enPosition) == 0)
            {
                return -info1.RankInfo.weekRankPoint.CompareTo(info2.RankInfo.weekRankPoint);
            }
            return info1.enPosition.CompareTo(info2.enPosition);
        }

        private int SortGuildMemberListOnlineStatusDesc(GuildMemInfo info1, GuildMemInfo info2)
        {
            if ((info1.stBriefInfo.dwGameEntity != 0) && (info2.stBriefInfo.dwGameEntity == 0))
            {
                return -1;
            }
            if ((info1.stBriefInfo.dwGameEntity == 0) && (info2.stBriefInfo.dwGameEntity != 0))
            {
                return 1;
            }
            return -info1.LastLoginTime.CompareTo(info2.LastLoginTime);
        }

        private int SortGuildMemberListPositionDesc(GuildMemInfo info1, GuildMemInfo info2)
        {
            return info1.enPosition.CompareTo(info2.enPosition);
        }

        private int SortGuildMemberListSeasonRankpointDesc(GuildMemInfo info1, GuildMemInfo info2)
        {
            return -info1.RankInfo.totalRankPoint.CompareTo(info2.RankInfo.totalRankPoint);
        }

        private int SortGuildMemberListWeekRankpointDesc(GuildMemInfo info1, GuildMemInfo info2)
        {
            return -info1.RankInfo.weekRankPoint.CompareTo(info2.RankInfo.weekRankPoint);
        }

        public override void UnInit()
        {
            IApolloSnsService iApolloSnsService = Utility.GetIApolloSnsService();
            if (iApolloSnsService != null)
            {
                iApolloSnsService.onBindGroupEvent -= new OnBindGroupNotifyHandle(this.OnBindQQGroupNotify);
                iApolloSnsService.onQueryGroupInfoEvent -= new OnQueryGroupInfoNotifyHandle(this.OnQueryQQGroupInfoNotify);
                iApolloSnsService.onUnBindGroupEvent -= new OnUnbindGroupNotifyHandle(this.OnUnBindQQGroupNotify);
                iApolloSnsService.onQueryGroupKeyEvent -= new OnQueryGroupKeyNotifyHandle(this.OnQueryQQGroupKeyNotify);
            }
        }

        public Tab CurTab
        {
            get
            {
                return this.m_curTab;
            }
            set
            {
                this.m_curTab = value;
                Singleton<EventRouter>.GetInstance().BroadCastEvent<Tab>("Guild_Info_Tab_Change", this.m_curTab);
            }
        }

        [CompilerGenerated]
        private sealed class <OpenGuildIconForm>c__AnonStorey51
        {
            internal CGuildInfoView <>f__this;
            internal int i;
            internal CUIListScript listScript;

            internal void <>m__3B(ResGuildIcon x)
            {
                if ((x.dwGuildMatchSeasonRankLimit == 0) || ((Singleton<CGuildSystem>.GetInstance().IsInNormalGuild() && (x.dwGuildMatchSeasonRankLimit > 0)) && (x.dwGuildMatchSeasonRankLimit == this.<>f__this.m_Model.CurrentGuildInfo.GuildMatchInfo.dwLastSeasonRankNo)))
                {
                    CUIListElementScript elemenet = this.listScript.GetElemenet(this.i);
                    this.<>f__this.SetGuildIcon(elemenet, x);
                    this.i++;
                }
            }
        }

        public enum enGuildApplyListFormWidget
        {
            Apply_Member_List,
            Close_Red_Dot_Toggle
        }

        public enum enGuildBuildingDetailFormWidget
        {
            Icon_Image,
            Info_Text,
            Guide_Text,
            Title_Text,
            Upgrade_Button
        }

        public enum enGuildDonateFormWidget
        {
            Times,
            Low_Des_Text,
            Middle_Des_Text,
            High_Des_Text,
            Top_Des_Text
        }

        public enum enGuildExchangePositionFormWidget
        {
            ViceChairman_List
        }

        public enum enGuildIconFormWidget
        {
            Icon_List
        }

        public enum enGuildInfoFormWidget
        {
            InfoPanel,
            MemberPanel,
            Tab_List,
            FacePanel_Guild_Head_Image,
            FacePanel_Guild_Name_Text,
            FacePanel_Guild_Chairman_Text,
            MemberPanel_Guild_Member_Count_Text,
            MemberPanel_Guild_Bulletin_Text,
            MemberPanel_Setting_Button,
            MemberPanel_Member_List,
            MemberPanel_Modify_Bulletin_Image,
            MemberPanel_Add_Member_Limit_Image,
            InfoPanel_Profit_Text,
            MemberPanel_Apply_List_Button,
            MemberPanel_StarLevel_Panel,
            MemberPanel_QQGroup_Info_Text,
            MemberPanel_QQGroup_Button,
            InfoPanel_Sign_Button,
            MemberPanel_QQGroup_Button_Text,
            MemberPanel_QQGroup_Panel,
            InfoPanel_Guild_Match_Button,
            MemberPanel_Mail_Button,
            FacePanel,
            InfoPanel_Rankpoint_Member_List,
            InfoPanel_Rankpoint_Self_Panel,
            InfoPanel_Grade_Icon_Image,
            InfoPanel_Grade_Name_Text,
            InfoPanel_Week_Award_Panel,
            InfoPanel_Season_Award_Panel,
            InfoPanel_Personal_Best_Num_Text,
            InfoPanel_Season_Clear_Time_Text,
            InfoPanel_Guild_Season_Point_Num_Text,
            InfoPanel_Guild_Week_Rank_Num_Text,
            InfoPanel_Guild_Week_Not_In_Rank_Text,
            MemberPanel_Sort_Position_Button_Image,
            MemberPanel_Sort_WeekRankpoint_Button_Image,
            MemberPanel_Sort_SeasonRankpoint_Button_Image,
            MemberPanel_Sort_OnlineStatus_Button_Image,
            MemberPanel_Recruit_Button,
            MemberPanel_Recruit_CD_Timer,
            MemberPanel_Recruit_Button_Text,
            InfoPanel_Guild_Match_Start_Flag_Image
        }

        public enum enGuildLogFormWidget
        {
            Content_Text,
            EmptyNode_Panel
        }

        private enum enGuildMemberListSortType
        {
            Default,
            PositionDesc,
            WeekRankpointDesc,
            SeasonRankpointDesc,
            OnlineStatus
        }

        public enum enGuildPreviewFormWidget
        {
            Guild_List,
            Guild_Chairman_Panel,
            Guild_Bulletin_Panel,
            Guild_Operation_Panel,
            Guild_Search_Guild_Input,
            Guild_Chairman_Name_Text,
            Guild_Bulletin_Text,
            Guild_Chairman_Head_Image,
            Guild_Chairman_Level_Text
        }

        public enum enGuildRankPointRankFormWidget
        {
            Guild_Icon_Image,
            Guild_Name_Text,
            Point_Num_Text,
            Member_Count_Text,
            Award_Panel,
            Rank_Tab_List,
            Rank_List,
            Rank_Panel,
            Season_Rank_Tab_List,
            Star_Panel,
            Grade_Image
        }

        public enum enGuildRankpointRankListTab
        {
            CurrentWeekRank,
            LastWeekRank,
            SeasonRank
        }

        public enum enGuildRankpointSeasonRankListTab
        {
            SelfRank,
            BestRank
        }

        public enum enGuildSettingFormWidget
        {
            Guild_Icon_Image,
            Need_Approval_Slider,
            Need_Approval_Slider_Front_Text,
            GuildIconIdText,
            Change_Name_Button,
            Guild_Name_Text,
            Join_Level_Text,
            Join_Grade_Index_Text,
            Join_Grade_Name_Text,
            Join_Level_Up_Button,
            Join_Level_Down_Button,
            Join_Grade_Up_Button,
            Join_Grade_Down_Button
        }

        public enum enGuildSignSuccessFormWidget
        {
            Sign_Success_Text
        }

        public enum enGuildSymbolFormWidget
        {
            Symbol_List,
            Icon_Image,
            Name_Text,
            Level_Text,
            Att_Text,
            Condition_Text,
            OpenOrUpgrade_Button_Text,
            OpenOrUpgrade_Button
        }

        public enum Tab
        {
            GuildInfo,
            GuildMember
        }
    }
}

