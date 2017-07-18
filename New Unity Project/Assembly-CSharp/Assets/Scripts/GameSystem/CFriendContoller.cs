namespace Assets.Scripts.GameSystem
{
    using Apollo;
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class CFriendContoller : Singleton<CFriendContoller>
    {
        [CompilerGenerated]
        private ulong <startCooldownTimestamp>k__BackingField;
        public static string AddFriendFormPath = "UGUI/Form/System/Friend/AddFriend.prefab";
        public static string AddMentorFormPath = "UGUI/Form/System/Friend/Form_AddMentor.prefab";
        private GameObject com;
        public const string Evt_Mentor_OnGetRecommend = "Evt_Mentor_GetRecommend";
        public const string Evt_Mentor_OnGetResult = "Evt_Mentor_GetResult";
        public static string FriendFormPath = "UGUI/Form/System/Friend/FriendForm.prefab";
        public static string IntimacyRelaFormPath = "UGUI/Form/System/Friend/Form_IntimacyRela.prefab";
        public string IntimacyUpInfo = string.Empty;
        public int IntimacyUpValue;
        public int m_currMentorPrivilegeLv = 1;
        public static SCPKG_MASTERSTUDENT_INFO m_mentorInfo = null;
        public CMentorListOffset[] m_mentorListOff = new CMentorListOffset[3];
        private int m_mentorPrivilegePage = 1;
        public COMDT_ACNT_UNIQ m_mentorSelectedUin;
        public static string MentorPrivilegeFormPath = "UGUI/Form/System/Friend/Form_MentorPrivilege.prefab";
        public const string MentorRecommendNodeName = "MentorRecommendList";
        public static string MentorRequestListFormPath = "UGUI/Form/System/Friend/Form_MentorRequestList.prefab";
        public const string MentorSearchRecommendPath = "UGUI/Form/System/Friend/MentorRecommendList.prefab";
        public const string MentorSearchResultNodeName = "MentorResult";
        public const string MentorSearchResultPath = "UGUI/Form/System/Friend/MentorResult.prefab";
        public static int MentorTabMask = 0;
        public static string MentorTaskFormPath = "UGUI/Form/System/Friend/Form_MentorTask.prefab";
        public CFriendModel model = new CFriendModel();
        public static int s_addViewtype = 3;
        public static string[] s_mentorTabName = new string[] { "Mentor_TabMentorNClassmate", "Mentor_TabApprentice" };
        public static string[] s_mentorTabStr = null;
        public COMDT_FRIEND_INFO search_info;
        public static string VerifyFriendFormPath = "UGUI/Form/System/Friend/Form_FriendVerification.prefab";
        public CFriendView view = new CFriendView();

        public CFriendContoller()
        {
            this.m_mentorListOff[1] = new CMentorListOffset();
            this.m_mentorListOff[2] = new CMentorListOffset();
        }

        private void Add_And_Refresh(CFriendModel.FriendType type, COMDT_FRIEND_INFO data)
        {
            bool flag;
            this.model.Add(type, data, false);
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            CFriendModel.FriendType type2 = type;
            switch (type2)
            {
                case CFriendModel.FriendType.Mentor:
                    if (m_mentorInfo != null)
                    {
                        m_mentorInfo.stMaster = new CSDT_FRIEND_INFO();
                        m_mentorInfo.stMaster.stFriendInfo = Utility.DeepCopyByReflection<COMDT_FRIEND_INFO>(data);
                        masterRoleInfo.m_mentorInfo.szRoleName = m_mentorInfo.stMaster.stFriendInfo.szUserName;
                        CPlayerProfile profile = Singleton<CPlayerInfoSystem>.instance.GetProfile();
                        if ((profile != null) && (profile._mentorInfo != null))
                        {
                            profile._mentorInfo.szRoleName = data.szUserName;
                        }
                        Singleton<CPlayerMentorInfoController>.GetInstance().UpdateUI();
                    }
                    goto Label_01E3;

                case CFriendModel.FriendType.Apprentice:
                    if (m_mentorInfo == null)
                    {
                        goto Label_01E3;
                    }
                    flag = true;
                    for (int i = 0; i < m_mentorInfo.bStudentNum; i++)
                    {
                        if ((m_mentorInfo.astStudentList[i].stFriendInfo.stUin.dwLogicWorldId == data.stUin.dwLogicWorldId) && (m_mentorInfo.astStudentList[i].stFriendInfo.stUin.ullUid == data.stUin.ullUid))
                        {
                            flag = false;
                            break;
                        }
                    }
                    break;

                case CFriendModel.FriendType.MentorRequestList:
                    this.view.RefreshMentorReqList();
                    return;

                default:
                    if (type2 == CFriendModel.FriendType.GameFriend)
                    {
                        Singleton<CFriendContoller>.GetInstance().model.SortGameFriend();
                    }
                    goto Label_01E3;
            }
            if (flag)
            {
                m_mentorInfo.bStudentNum = (byte) (m_mentorInfo.bStudentNum + 1);
                m_mentorInfo.astStudentList[m_mentorInfo.bStudentNum - 1] = new CSDT_FRIEND_INFO();
                m_mentorInfo.astStudentList[m_mentorInfo.bStudentNum - 1].stFriendInfo = Utility.DeepCopyByReflection<COMDT_FRIEND_INFO>(data);
                masterRoleInfo.m_mentorInfo.dwStudentNum++;
            }
        Label_01E3:
            if (this.view.IsActive())
            {
                this.view.Refresh();
            }
        }

        public void ClearAll()
        {
            this.model.ClearAll();
            this.IntimacyUpInfo = string.Empty;
            this.IntimacyUpValue = 0;
            this.search_info = null;
            this.com = null;
            this.SetFilter(3);
        }

        public bool FilterSameFriend(COMDT_FRIEND_INFO info, ListView<COMDT_FRIEND_INFO> friendList)
        {
            if (friendList != null)
            {
                for (int i = 0; i < friendList.Count; i++)
                {
                    if (friendList[i].stUin.ullUid == info.stUin.ullUid)
                    {
                        if (friendList[i].dwLastLoginTime < info.dwLastLoginTime)
                        {
                            friendList[i] = info;
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        public static CFriendModel.FriendType GetFriendType(int searchType)
        {
            int num = searchType;
            if ((num != 2) && (num != 3))
            {
                return CFriendModel.FriendType.Recommend;
            }
            return CFriendModel.FriendType.MentorRecommend;
        }

        public static CFriendModel.FriendType GetFriendTypeFromItemType(FriendShower.ItemType itemType)
        {
            FriendShower.ItemType type = itemType;
            if (type != FriendShower.ItemType.Mentor)
            {
                if (type == FriendShower.ItemType.Apprentice)
                {
                    return CFriendModel.FriendType.Apprentice;
                }
                return CFriendModel.FriendType.GameFriend;
            }
            return CFriendModel.FriendType.Mentor;
        }

        public static int GetFriendTypeFromSrvFriendType(COM_FRIEND_TYPE srvFriendType)
        {
            switch (srvFriendType)
            {
                case COM_FRIEND_TYPE.COM_FRIEND_TYPE_GAME:
                    return 1;

                case COM_FRIEND_TYPE.COM_FRIEND_TYPE_SNS:
                    return 4;

                case COM_FRIEND_TYPE.COM_FRIEND_TYPE_STUDENT:
                    return 7;

                case COM_FRIEND_TYPE.COM_FRIEND_TYPE_MASTER:
                    return 6;
            }
            return -1;
        }

        private ulong GetFriendUid(CUIEvent uiEvent)
        {
            FriendShower component = uiEvent.m_srcWidget.get_transform().get_parent().get_parent().get_parent().GetComponent<FriendShower>();
            if (component == null)
            {
                return 0L;
            }
            COMDT_FRIEND_INFO comdt_friend_info = null;
            if (this.view.GetSelectedTab() == CFriendView.Tab.Mentor)
            {
                comdt_friend_info = this.model.GetInfo(CFriendModel.FriendType.Mentor, component.ullUid, component.dwLogicWorldID);
                if (comdt_friend_info == null)
                {
                    comdt_friend_info = this.model.GetInfo(CFriendModel.FriendType.Apprentice, component.ullUid, component.dwLogicWorldID);
                }
            }
            else
            {
                CFriendModel.FriendType type = (this.view.GetSelectedTab() != CFriendView.Tab.Friend_SNS) ? CFriendModel.FriendType.GameFriend : CFriendModel.FriendType.SNS;
                comdt_friend_info = this.model.GetInfo(type, component.ullUid, component.dwLogicWorldID);
            }
            if (comdt_friend_info == null)
            {
                return 0L;
            }
            return comdt_friend_info.stUin.ullUid;
        }

        public static uint GetMentorGradeLimit()
        {
            return GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x10f).dwConfValue;
        }

        public static int GetMentorServerState()
        {
            switch (GetMentorState(0, null))
            {
                case enMentorState.IWantMentor:
                case enMentorState.IHasMentor:
                    return 3;

                case enMentorState.IWantApprentice:
                case enMentorState.IHasApprentice:
                    return 2;
            }
            return -1;
        }

        public static enMentorState GetMentorState(uint iLevel = 0, SCPKG_MASTERSTUDENT_INFO info = new SCPKG_MASTERSTUDENT_INFO())
        {
            if (iLevel == 0)
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if (m_mentorInfo == null)
                {
                    return enMentorState.None;
                }
                if (masterRoleInfo != null)
                {
                    iLevel = masterRoleInfo.Level;
                }
            }
            if (info == null)
            {
                info = m_mentorInfo;
            }
            if (iLevel != 0)
            {
                uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 270).dwConfValue;
                if (iLevel <= dwConfValue)
                {
                    if (!Singleton<CFriendContoller>.GetInstance().HasMentor(info))
                    {
                        return enMentorState.IWantMentor;
                    }
                    return enMentorState.IHasMentor;
                }
                uint num2 = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x10d).dwConfValue;
                if (iLevel >= num2)
                {
                    if (!Singleton<CFriendContoller>.GetInstance().HasApprentice(info))
                    {
                        return enMentorState.IWantApprentice;
                    }
                    return enMentorState.IHasApprentice;
                }
            }
            return enMentorState.None;
        }

        public string GetMentorStateString()
        {
            switch (GetMentorState(0, null))
            {
                case enMentorState.IWantMentor:
                case enMentorState.IHasMentor:
                    return Singleton<CTextManager>.GetInstance().GetText("Mentor_GetMentor");

                case enMentorState.IWantApprentice:
                case enMentorState.IHasApprentice:
                    return Singleton<CTextManager>.GetInstance().GetText("Mentor_GetApprentice");
            }
            return null;
        }

        private FriendShower GetParentFriendShower(Transform node)
        {
            FriendShower component = node.GetComponent<FriendShower>();
            if (component != null)
            {
                return component;
            }
            if (node.get_parent() != null)
            {
                return this.GetParentFriendShower(node.get_parent());
            }
            return null;
        }

        public static int GetSrvFriendTypeFromFriendType(CFriendModel.FriendType friendType)
        {
            switch (friendType)
            {
                case CFriendModel.FriendType.GameFriend:
                    return 1;

                case CFriendModel.FriendType.SNS:
                    return 2;

                case CFriendModel.FriendType.Mentor:
                    return 6;

                case CFriendModel.FriendType.Apprentice:
                    return 5;
            }
            return -1;
        }

        public static int GetSrvMentorTypeByItemType(FriendShower.ItemType itemType)
        {
            switch (itemType)
            {
                case FriendShower.ItemType.AddMentor:
                case FriendShower.ItemType.Mentor:
                    return 2;

                case FriendShower.ItemType.AddApprentice:
                case FriendShower.ItemType.Apprentice:
                    return 3;
            }
            return 0;
        }

        private void Handle_CoinSend_Data(CSDT_FRIEND_INFO info)
        {
            this.Update_Send_Coin_Data(info.stFriendInfo.stUin, info.ullDonateAPSec);
        }

        private void Handle_CoinSend_Data(CSDT_SNS_FRIEND_INFO info)
        {
            this.Update_Send_Coin_Data(info.stSnsFrindInfo.stUin, (ulong) info.dwDonateTime);
        }

        private void Handle_CoinSend_Data_Apprentice(CSDT_FRIEND_INFO info)
        {
            this.Update_Send_Coin_Data(info.stFriendInfo.stUin, info.ullDonateAPSec);
        }

        private void Handle_CoinSend_Data_Mentor(CSDT_FRIEND_INFO info)
        {
            this.Update_Send_Coin_Data(info.stFriendInfo.stUin, info.ullDonateAPSec);
        }

        private void Handle_Invite_Data(COMDT_ACNT_UNIQ uin)
        {
            this.model.SnsReCallData.Add(uin, COM_FRIEND_TYPE.COM_FRIEND_TYPE_SNS);
        }

        public bool HasApprentice(SCPKG_MASTERSTUDENT_INFO info = new SCPKG_MASTERSTUDENT_INFO())
        {
            if ((info != null) && (info != m_mentorInfo))
            {
                return (info.bStudentNum != 0);
            }
            ListView<COMDT_FRIEND_INFO> list = Singleton<CFriendContoller>.instance.model.GetList(CFriendModel.FriendType.Apprentice);
            if (list == null)
            {
                return false;
            }
            return (list.Count != 0);
        }

        public bool HasMentor(SCPKG_MASTERSTUDENT_INFO info = new SCPKG_MASTERSTUDENT_INFO())
        {
            if ((info != null) && (info != m_mentorInfo))
            {
                return (info.stMaster.stFriendInfo.stUin.ullUid != 0L);
            }
            ListView<COMDT_FRIEND_INFO> list = Singleton<CFriendContoller>.instance.model.GetList(CFriendModel.FriendType.Mentor);
            if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (((list[i].bStudentType & 240) >> 4) != 2)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override void Init()
        {
            base.Init();
            this.InitEvent();
        }

        private void InitEvent()
        {
            Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_List", new Action<CSPkg>(this.On_FriendSys_Friend_List));
            Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_Request_List", new Action<CSPkg>(this.On_FriendSys_Friend_Request_List));
            Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_Recommand_List", new Action<CSPkg>(this.On_FriendSys_Friend_Recommand_List));
            Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_Search", new Action<CSPkg>(this.On_FriendSys_Friend_Search));
            Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_RequestBeFriend", new Action<CSPkg>(this.On_FriendSys_Friend_RequestBeFriend));
            Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg, CFriendModel.FriendType>("Friend_Confrim", new Action<CSPkg, CFriendModel.FriendType>(this, (IntPtr) this.On_FriendSys_Friend_Confrim));
            Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_Deny", new Action<CSPkg>(this.On_FriendSys_Friend_Deny));
            Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_Delete", new Action<CSPkg>(this.On_FriendSys_Friend_Delete));
            Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_ADD_NTF", new Action<CSPkg>(this.On_FriendSys_Friend_ADD_NTF));
            Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_Delete_NTF", new Action<CSPkg>(this.On_FriendSys_Friend_Delete_NTF));
            Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_Request_NTF", new Action<CSPkg>(this.On_FriendSys_Friend_Request_NTF));
            Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_Login_NTF", new Action<CSPkg>(this.On_Friend_Login_NTF));
            Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_GAME_STATE_NTF", new Action<CSPkg>(this.On_Friend_GAME_STATE_NTF));
            Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_SNS_STATE_NTF", new Action<CSPkg>(this.On_Friend_SNS_STATE_NTF));
            Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_SNS_CHG_PROFILE", new Action<CSPkg>(this.On_Friend_SNS_CHG_PROFILE));
            Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_SNS_NICNAME_NTF", new Action<CSPkg>(this.On_Friend_SNS_NICKNAME_NTF));
            Singleton<EventRouter>.GetInstance().AddEventHandler<float, float>("Friend_LBS_Location_Calced", new Action<float, float>(this, (IntPtr) this.On_Friend_LBS_Location_Calced));
            Singleton<EventRouter>.GetInstance().AddEventHandler("Friend_RecommandFriend_Refresh", new Action(this, (IntPtr) this.On_Friend_RecommandFriend_Refresh));
            Singleton<EventRouter>.GetInstance().AddEventHandler("Friend_FriendList_Refresh", new Action(this, (IntPtr) this.On_Friend_FriendList_Refresh));
            Singleton<EventRouter>.GetInstance().AddEventHandler("Friend_SNSFriendList_Refresh", new Action(this, (IntPtr) this.On_Friend_SNSFriendList_Refresh));
            Singleton<EventRouter>.GetInstance().AddEventHandler("Friend_LBS_User_Refresh", new Action(this, (IntPtr) this.On_Friend_LBS_User_Refresh));
            Singleton<EventRouter>.GetInstance().AddEventHandler("Guild_Invite_Success", new Action(this, (IntPtr) this.On_GuildSys_Guild_Invite_Success));
            Singleton<EventRouter>.GetInstance().AddEventHandler("Guild_Recommend_Success", new Action(this, (IntPtr) this.On_GuildSys_Guild_Recommend_Success));
            Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.NEWDAY_NTF, new Action(this, (IntPtr) this.OnNewDayNtf));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_OpenForm, new CUIEventManager.OnUIEventHandler(this.On_OpenForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_CloseForm, new CUIEventManager.OnUIEventHandler(this.On_CloseForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_Tab_Change, new CUIEventManager.OnUIEventHandler(this.On_TabChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_OpenAddFriendForm, new CUIEventManager.OnUIEventHandler(this.On_OpenAddFriendForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_RequestBeFriend, new CUIEventManager.OnUIEventHandler(this.On_AddFriend));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_Accept_RequestFriend, new CUIEventManager.OnUIEventHandler(this.On_Accept_RequestFriend));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_Refuse_RequestFriend, new CUIEventManager.OnUIEventHandler(this.On_Refuse_RequestFriend));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_DelFriend, new CUIEventManager.OnUIEventHandler(this.On_DelFriend));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_Tab_Friend, new CUIEventManager.OnUIEventHandler(this.On_Friend_Tab_Friend));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_Tab_FriendRequest, new CUIEventManager.OnUIEventHandler(this.On_Friend_Tab_FriendRequest));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_DelFriend_OK, new CUIEventManager.OnUIEventHandler(this.On_DelFriend_OK));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_DelFriend_Cancle, new CUIEventManager.OnUIEventHandler(this.On_Friend_DelFriend_Cancle));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_SendCoin, new CUIEventManager.OnUIEventHandler(this.On_Friend_SendCoin));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_SNS_SendCoin, new CUIEventManager.OnUIEventHandler(this.On_SNSFriend_SendCoin));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_InviteGuild, new CUIEventManager.OnUIEventHandler(this.On_Friend_InviteGuild));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_RecommendGuild, new CUIEventManager.OnUIEventHandler(this.On_Friend_RecommendGuild));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_CheckInfo, new CUIEventManager.OnUIEventHandler(this.On_Friend_CheckInfo));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_Recommend_CheckInfo, new CUIEventManager.OnUIEventHandler(this.On_FriendRecommend_CheckInfo));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_List_ElementEnable, new CUIEventManager.OnUIEventHandler(this.On_Friend_List_ElementEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_Invite_SNS_Friend, new CUIEventManager.OnUIEventHandler(this.On_Friend_Invite_SNS_Friend));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_Share_SendCoin, new CUIEventManager.OnUIEventHandler(this.On_Friend_Share_SendCoin));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_SNS_ReCall, new CUIEventManager.OnUIEventHandler(this.On_Friend_SNS_ReCall));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_OB_Click, new CUIEventManager.OnUIEventHandler(this.On_Friend_OB_Click));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.QQBOX_OnClick, new CUIEventManager.OnUIEventHandler(this.QQBox_OnClick));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_SNS_Share_Switch_Click, new CUIEventManager.OnUIEventHandler(this.OnShareToggle));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_SNS_Add_Switch_Click, new CUIEventManager.OnUIEventHandler(this.OnAddToggle));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_Chat_Button, new CUIEventManager.OnUIEventHandler(this.OnFriend_Chat_Button));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_Show_Rule, new CUIEventManager.OnUIEventHandler(this.OnFriend_Show_Rule));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_Gift, new CUIEventManager.OnUIEventHandler(this.OnFriend_Gift_Button));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_CancleBlock, new CUIEventManager.OnUIEventHandler(this.OnCancleBlockBtn));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_CancleBlock_Ok, new CUIEventManager.OnUIEventHandler(this.OnCancleBlockBtnOK));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_LBS_NoShare, new CUIEventManager.OnUIEventHandler(this.OnFriend_LBS_NoShare));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_LBS_Nan, new CUIEventManager.OnUIEventHandler(this.OnFriend_LBS_Nan));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_LBS_Nv, new CUIEventManager.OnUIEventHandler(this.OnFriend_LBS_Nv));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_LBS_Refresh, new CUIEventManager.OnUIEventHandler(this.OnFriend_LBS_Refresh));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_LBS_CheckInfo, new CUIEventManager.OnUIEventHandler(this.OnFriend_LBS_CheckInfo));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_Room_AddFriend, new CUIEventManager.OnUIEventHandler(this.OnFriend_Room_AddFriend));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.IntimacyRela_Menu_Btn_Click, new CUIEventManager.OnUIEventHandler(this.OnIntimacyRela_Menu_Btn_Click));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mentor_MentorQuest, new CUIEventManager.OnUIEventHandler(this.OnMentorTask_Btn_Click));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mentor_IWant, new CUIEventManager.OnUIEventHandler(this.OnMentor_IWant));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mentor_FriendFormTabChange, new CUIEventManager.OnUIEventHandler(this.OnMentorTabChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mentor_RequestListOnEnable, new CUIEventManager.OnUIEventHandler(this.On_MentorRequestListEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mentor_AcceptRequest, new CUIEventManager.OnUIEventHandler(this.OnMentor_AcceptRequest));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mentor_RefuseRequest, new CUIEventManager.OnUIEventHandler(this.OnMentor_RefuseRequest));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mentor_AddMentor, new CUIEventManager.OnUIEventHandler(this.On_AddMentor));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mentor_OpenRequestList, new CUIEventManager.OnUIEventHandler(this.OnMentor_RequestListClick));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mentor_CloseRequestList, new CUIEventManager.OnUIEventHandler(this.OnMentorRequestListClose));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mentor_OpenPrivilegePage, new CUIEventManager.OnUIEventHandler(this.OnMentor_OpenPrivilegePage));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mentor_PrivilegePageLeft, new CUIEventManager.OnUIEventHandler(this.OnMentor_PrivilegeLeftClick));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mentor_PrivilegePageRight, new CUIEventManager.OnUIEventHandler(this.OnMentor_PrivilegeRightClick));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mentor_PrivilegeListEnable, new CUIEventManager.OnUIEventHandler(this.OnMentor_PrivilegeListEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mentor_GetMoreMentor, new CUIEventManager.OnUIEventHandler(this.OnMentorGetMoreMentorList));
        }

        private void On_Accept_RequestFriend(CUIEvent uievent)
        {
            FriendShower component = uievent.m_srcWidget.get_transform().get_parent().get_parent().get_parent().GetComponent<FriendShower>();
            if (component != null)
            {
                COMDT_FRIEND_INFO comdt_friend_info = this.model.GetInfo(CFriendModel.FriendType.RequestFriend, component.ullUid, component.dwLogicWorldID);
                if (comdt_friend_info != null)
                {
                    FriendSysNetCore.Send_Confrim_BeFriend(comdt_friend_info.stUin);
                }
            }
        }

        private void On_AddFriend(CUIEvent evt)
        {
            if (evt.m_srcFormScript.m_formPath.Equals(AddFriendFormPath))
            {
                this.On_AddSomthing(evt, CFriendModel.FriendType.Recommend, false);
            }
            else if (evt.m_srcFormScript.m_formPath.Equals(FriendFormPath))
            {
                s_addViewtype = 1;
                this.On_AddSomthing(evt, (CFriendModel.FriendType) evt.m_eventParams.tag, true);
            }
        }

        private void On_AddMentor(CUIEvent evt)
        {
            this.On_AddSomthing(evt, CFriendModel.FriendType.MentorRecommend, false);
        }

        public void On_AddSomthing(CUIEvent evt, CFriendModel.FriendType friendType, bool ignoreSearchReasult = false)
        {
            COMDT_FRIEND_INFO stLbsUserInfo = Singleton<CFriendContoller>.GetInstance().search_info;
            int num = 0;
            enMentorState mentorState = GetMentorState(0, null);
            if (this.model != null)
            {
                if (((evt.m_srcWidgetBelongedListScript == null) && (stLbsUserInfo != null)) && !ignoreSearchReasult)
                {
                    num = CLadderSystem.ConvertEloToRank(stLbsUserInfo.RankVal[7]);
                    if (((mentorState == enMentorState.IWantMentor) || (mentorState == enMentorState.IHasMentor)) && (((stLbsUserInfo != null) && (num < GetMentorGradeLimit())) && ((friendType == CFriendModel.FriendType.MentorRequestList) || (friendType == CFriendModel.FriendType.MentorRecommend))))
                    {
                        Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Mentor_Err_CannotGetApprentice"), false, 1.5f, null, new object[0]);
                    }
                    else
                    {
                        this.Open_Friend_Verify(stLbsUserInfo.stUin.ullUid, stLbsUserInfo.stUin.dwLogicWorldId, true, COM_ADD_FRIEND_TYPE.COM_ADD_FRIEND_NULL, -1, false);
                    }
                }
                else
                {
                    FriendShower parentFriendShower = this.GetParentFriendShower(evt.m_srcWidget.get_transform());
                    if (parentFriendShower != null)
                    {
                        stLbsUserInfo = this.model.GetInfo(friendType, parentFriendShower.ullUid, parentFriendShower.dwLogicWorldID);
                        if (stLbsUserInfo != null)
                        {
                            num = CLadderSystem.ConvertEloToRank(stLbsUserInfo.RankVal[7]);
                            if (((mentorState == enMentorState.IWantMentor) || (mentorState == enMentorState.IHasMentor)) && (((stLbsUserInfo != null) && (num < GetMentorGradeLimit())) && ((friendType == CFriendModel.FriendType.MentorRequestList) || (friendType == CFriendModel.FriendType.MentorRecommend))))
                            {
                                Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Mentor_Err_CannotGetApprentice"), false, 1.5f, null, new object[0]);
                                return;
                            }
                        }
                        else
                        {
                            CSDT_LBS_USER_INFO csdt_lbs_user_info = this.model.GetLBSUserInfo(parentFriendShower.ullUid, parentFriendShower.dwLogicWorldID, CFriendModel.LBSGenderType.Both);
                            if (csdt_lbs_user_info != null)
                            {
                                stLbsUserInfo = csdt_lbs_user_info.stLbsUserInfo;
                            }
                        }
                        if (stLbsUserInfo != null)
                        {
                            if ((this.view != null) && (this.view.CurTab == CFriendView.Tab.Friend_LBS))
                            {
                                this.Open_Friend_Verify(stLbsUserInfo.stUin.ullUid, stLbsUserInfo.stUin.dwLogicWorldId, false, COM_ADD_FRIEND_TYPE.COM_ADD_FRIEND_LBS, -1, false);
                            }
                            else
                            {
                                this.Open_Friend_Verify(stLbsUserInfo.stUin.ullUid, stLbsUserInfo.stUin.dwLogicWorldId, false, COM_ADD_FRIEND_TYPE.COM_ADD_FRIEND_NULL, -1, false);
                            }
                        }
                    }
                }
            }
        }

        private void On_CloseForm(CUIEvent uiEvent)
        {
            this.view.CloseForm();
        }

        private void On_DelFriend(CUIEvent evt)
        {
            this.com = evt.m_srcWidget;
            string text = UT.GetText("Friend_Tips_DelFriend");
            switch (evt.m_eventParams.tag)
            {
                case 8:
                case 9:
                    text = Singleton<CTextManager>.GetInstance().GetText("Mentor_DelConfirm");
                    break;
            }
            Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(text, enUIEventID.Friend_DelFriend_OK, enUIEventID.Friend_DelFriend_Cancle, evt.m_eventParams, false);
        }

        private void On_DelFriend_OK(CUIEvent evt)
        {
            FriendShower component = this.com.get_transform().get_parent().get_parent().get_parent().GetComponent<FriendShower>();
            if (component != null)
            {
                COMDT_FRIEND_INFO info = this.model.GetInfo(GetFriendTypeFromItemType((FriendShower.ItemType) evt.m_eventParams.tag), component.ullUid, component.dwLogicWorldID);
                if (info != null)
                {
                    FriendSysNetCore.Send_Del_Friend(info, (FriendShower.ItemType) evt.m_eventParams.tag);
                }
            }
        }

        private void On_Friend_CheckInfo(CUIEvent uievent)
        {
            FriendShower component = uievent.m_srcWidget.get_transform().get_parent().get_parent().get_parent().GetComponent<FriendShower>();
            if (component != null)
            {
                Singleton<CPlayerInfoSystem>.instance.ShowPlayerDetailInfo(component.ullUid, (int) component.dwLogicWorldID, CPlayerInfoSystem.DetailPlayerInfoSource.DefaultOthers, true);
            }
        }

        private void On_Friend_DelFriend_Cancle(CUIEvent evt)
        {
        }

        private void On_Friend_FriendList_Refresh()
        {
            if ((this.view != null) && this.view.IsActive())
            {
                this.view.Refresh();
            }
        }

        private void On_Friend_GAME_STATE_NTF(CSPkg msg)
        {
            SCPKG_CMD_NTF_FRIEND_GAME_STATE stNtfFriendGameState = msg.stPkgData.stNtfFriendGameState;
            this.model.SetFriendGameState(stNtfFriendGameState.stAcntUniq.ullUid, stNtfFriendGameState.stAcntUniq.dwLogicWorldId, (COM_ACNT_GAME_STATE) stNtfFriendGameState.bGameState, stNtfFriendGameState.dwGameStartTime, string.Empty, false);
            if ((this.view != null) && this.view.IsActive())
            {
                this.view.Refresh();
            }
            Singleton<EventRouter>.instance.BroadCastEvent(EventID.Friend_Game_State_Change);
        }

        private void On_Friend_Invite_SNS_Friend(CUIEvent uievent)
        {
            if ((this.view != null) && this.view.IsActive())
            {
                this.view.On_Friend_Invite_SNS_Friend(uievent);
            }
        }

        private void On_Friend_InviteGuild(CUIEvent uiEvent)
        {
            Singleton<EventRouter>.GetInstance().BroadCastEvent<ulong>("Guild_Invite", this.GetFriendUid(uiEvent));
        }

        private void On_Friend_LBS_Location_Calced(float n, float e)
        {
        }

        private void On_Friend_LBS_User_Refresh()
        {
            if ((this.view != null) && this.view.IsActive())
            {
                this.view.Refresh();
            }
        }

        private void On_Friend_List_ElementEnable(CUIEvent uievent)
        {
            if ((this.view != null) && this.view.IsActive())
            {
                this.view.On_List_ElementEnable(uievent);
            }
        }

        private void On_Friend_Login_NTF(CSPkg msg)
        {
            SCPKG_CMD_NTF_FRIEND_LOGIN_STATUS stNtfFriendLoginStatus = msg.stPkgData.stNtfFriendLoginStatus;
            CFriendModel.FriendType type = (stNtfFriendLoginStatus.bFriendType != 1) ? CFriendModel.FriendType.SNS : CFriendModel.FriendType.GameFriend;
            COMDT_FRIEND_INFO info = Singleton<CFriendContoller>.GetInstance().model.GetInfo(type, stNtfFriendLoginStatus.stUin);
            if (info != null)
            {
                info.bIsOnline = stNtfFriendLoginStatus.bLoginStatus;
                info.dwLastLoginTime = (uint) CRoleInfo.GetCurrentUTCTime();
                Singleton<CFriendContoller>.GetInstance().model.SortGameFriend();
                if ((this.view != null) && this.view.IsActive())
                {
                    this.view.Refresh();
                }
            }
        }

        private void On_Friend_OB_Click(CUIEvent uievent)
        {
            FriendShower component = uievent.m_srcWidget.get_transform().get_parent().get_parent().get_parent().GetComponent<FriendShower>();
            if (component != null)
            {
                COMDT_FRIEND_INFO comdt_friend_info = this.model.GetInfo(CFriendModel.FriendType.SNS, component.ullUid, component.dwLogicWorldID);
                if (comdt_friend_info == null)
                {
                    comdt_friend_info = this.model.GetInfo(CFriendModel.FriendType.GameFriend, component.ullUid, component.dwLogicWorldID);
                }
                if (comdt_friend_info != null)
                {
                    Singleton<COBSystem>.instance.OBFriend(comdt_friend_info.stUin);
                }
            }
        }

        private void On_Friend_RecommandFriend_Refresh()
        {
        }

        private void On_Friend_RecommendGuild(CUIEvent uiEvent)
        {
            Singleton<EventRouter>.GetInstance().BroadCastEvent<ulong>("Guild_Recommend", this.GetFriendUid(uiEvent));
        }

        private void On_Friend_SendCoin(CUIEvent uievent)
        {
            if (uievent.m_eventParams.tag == -1)
            {
                Debug.LogError("Error server friend type");
            }
            COMDT_ACNT_UNIQ uniq = new COMDT_ACNT_UNIQ();
            uniq.ullUid = uievent.m_eventParams.commonUInt64Param1;
            uniq.dwLogicWorldId = (uint) uievent.m_eventParams.commonUInt64Param2;
            FriendSysNetCore.Send_FriendCoin(uniq, (COM_FRIEND_TYPE) uievent.m_eventParams.tag);
        }

        private void On_Friend_Share_SendCoin(CUIEvent uievent)
        {
            try
            {
                if (MonoSingleton<ShareSys>.instance.IsInstallPlatform())
                {
                    string openId = uievent.m_eventParams.snsFriendEventParams.openId;
                    Singleton<ApolloHelper>.instance.ShareSendHeart(openId, Singleton<CTextManager>.instance.GetText("Common_Sns_Tips_1"), Singleton<CTextManager>.instance.GetText("Common_Sns_Tips_2"), ShareSys.SNS_SHARE_SEND_HEART);
                    Singleton<CUIManager>.instance.OpenTips("Common_Sns_Tips_7", true, 1.5f, null, new object[0]);
                }
            }
            catch (Exception exception)
            {
                object[] inParameters = new object[] { exception.Message };
                DebugHelper.Assert(false, "Exception in On_Friend_Share_SendCoin, {0}", inParameters);
            }
        }

        private void On_Friend_SNS_CHG_PROFILE(CSPkg msg)
        {
            SCPKG_CHG_SNS_FRIEND_PROFILE stSnsFriendChgProfile = msg.stPkgData.stSnsFriendChgProfile;
            this.model.Add(CFriendModel.FriendType.SNS, stSnsFriendChgProfile.stSnsFrindInfo, true);
            this.model.SetFriendGameState(stSnsFriendChgProfile.stSnsFrindInfo.stUin.ullUid, stSnsFriendChgProfile.stSnsFrindInfo.stUin.dwLogicWorldId, (COM_ACNT_GAME_STATE) stSnsFriendChgProfile.bGameState, 0, string.Empty, true);
            this.model.SortSNSFriend();
            if ((this.view != null) && this.view.IsActive())
            {
                this.view.Refresh();
            }
            Singleton<EventRouter>.instance.BroadCastEvent(EventID.Friend_Game_State_Change);
        }

        private void On_Friend_SNS_NICKNAME_NTF(CSPkg msg)
        {
            SCPKG_NTF_SNS_NICKNAME stNtfSnsNickName = msg.stPkgData.stNtfSnsNickName;
            uint num = Math.Min(stNtfSnsNickName.dwSnsFriendNum, 100);
            for (int i = 0; i < num; i++)
            {
                if (!CFriendModel.RemarkNames.ContainsKey(stNtfSnsNickName.astSnsNameList[i].ullUid))
                {
                    CFriendModel.RemarkNames.Add(stNtfSnsNickName.astSnsNameList[i].ullUid, CUIUtility.RemoveEmoji(UT.Bytes2String(stNtfSnsNickName.astSnsNameList[i].szNickName)));
                }
            }
            if ((this.view != null) && this.view.IsActive())
            {
                this.view.Refresh();
            }
        }

        private void On_Friend_SNS_ReCall(CUIEvent uievent)
        {
            FriendShower component = uievent.m_srcWidget.get_transform().get_parent().get_parent().get_parent().GetComponent<FriendShower>();
            if (component != null)
            {
                COMDT_FRIEND_INFO comdt_friend_info = this.model.GetInfo(CFriendModel.FriendType.SNS, component.ullUid, component.dwLogicWorldID);
                if (comdt_friend_info != null)
                {
                    FriendSysNetCore.ReCallSnsFriend(comdt_friend_info.stUin, COM_FRIEND_TYPE.COM_FRIEND_TYPE_SNS);
                    if (!CFriendModel.IsOnSnsSwitch(comdt_friend_info.dwRefuseFriendBits, COM_REFUSE_TYPE.COM_REFUSE_TYPE_DONOTE_AND_REC))
                    {
                        this.ShareTo_SNSFriend_ReCall(Utility.UTF8Convert(comdt_friend_info.szOpenId));
                    }
                }
            }
        }

        private void On_Friend_SNS_STATE_NTF(CSPkg msg)
        {
            SCPKG_NTF_SNS_FRIEND stNtfSnsFriend = msg.stPkgData.stNtfSnsFriend;
            uint currentUTCTime = (uint) CRoleInfo.GetCurrentUTCTime();
            uint num2 = 0x15180 * GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x9e).dwConfValue;
            ListView<COMDT_FRIEND_INFO> list = this.model.GetList(CFriendModel.FriendType.SNS);
            for (int i = 0; i < stNtfSnsFriend.dwSnsFriendNum; i++)
            {
                CSDT_SNS_FRIEND_INFO info = stNtfSnsFriend.astSnsFriendList[i];
                if ((info != null) && !this.FilterSameFriend(info.stSnsFrindInfo, list))
                {
                    this.model.Add(CFriendModel.FriendType.SNS, info.stSnsFrindInfo, false);
                    this.model.SetFriendGameState(info.stSnsFrindInfo.stUin.ullUid, info.stSnsFrindInfo.stUin.dwLogicWorldId, (COM_ACNT_GAME_STATE) info.bGameState, info.dwGameStartTime, UT.Bytes2String(info.szNickName), false);
                    this.Handle_CoinSend_Data(info);
                }
            }
            this.model.SortSNSFriend();
            if ((this.view != null) && this.view.IsActive())
            {
                this.view.Refresh();
            }
            Singleton<EventRouter>.instance.BroadCastEvent(EventID.Friend_Game_State_Change);
        }

        private void On_Friend_SNSFriendList_Refresh()
        {
            if (((this.view != null) && (this.view.addFriendView != null)) && this.view.addFriendView.bShow)
            {
                this.view.addFriendView.Refresh(3);
            }
        }

        private void On_Friend_Tab_Friend(CUIEvent uievent)
        {
            if ((this.view != null) && this.view.IsActive())
            {
                this.view.On_Tab_Change(0);
            }
        }

        private void On_Friend_Tab_FriendRequest(CUIEvent uievent)
        {
            if ((this.view != null) && this.view.IsActive())
            {
                this.view.On_Tab_Change(1);
            }
        }

        private void On_FriendRecommend_CheckInfo(CUIEvent uievent)
        {
            FriendShower component = uievent.m_srcWidget.get_transform().get_parent().get_parent().GetComponent<FriendShower>();
            if (component != null)
            {
                Singleton<CPlayerInfoSystem>.instance.ShowPlayerDetailInfo(component.ullUid, (int) component.dwLogicWorldID, CPlayerInfoSystem.DetailPlayerInfoSource.DefaultOthers, true);
            }
        }

        private void On_FriendSys_Friend_ADD_NTF(CSPkg msg)
        {
            SCPKG_CMD_NTF_FRIEND_ADD stNtfFriendAdd = msg.stPkgData.stNtfFriendAdd;
            if (this.model.IsBlack(stNtfFriendAdd.stUserInfo.stUin.ullUid, stNtfFriendAdd.stUserInfo.stUin.dwLogicWorldId))
            {
                this.model.RemoveFriendBlack(stNtfFriendAdd.stUserInfo.stUin.ullUid, stNtfFriendAdd.stUserInfo.stUin.dwLogicWorldId);
            }
            this.model.RemoveFriendVerifyContent(stNtfFriendAdd.stUserInfo.stUin.ullUid, stNtfFriendAdd.stUserInfo.stUin.dwLogicWorldId, CFriendModel.enVerifyDataSet.Friend);
            this.model.AddFriendIntimacy(stNtfFriendAdd.stUserInfo.stUin, stNtfFriendAdd.stIntimacData);
            if (stNtfFriendAdd.bFriendSrcType == 2)
            {
                this.model.friendRecruit.SetZhaoMuZhe(stNtfFriendAdd.stUserInfo);
            }
            this.Add_And_Refresh(CFriendModel.FriendType.GameFriend, stNtfFriendAdd.stUserInfo);
        }

        private void On_FriendSys_Friend_Confrim(CSPkg msg, CFriendModel.FriendType friendType)
        {
            CFriendView view;
            CFriendModel.FriendType type = friendType;
            if (type != CFriendModel.FriendType.RequestFriend)
            {
                if (type != CFriendModel.FriendType.MentorRequestList)
                {
                    return;
                }
            }
            else
            {
                SCPKG_CMD_ADD_FRIEND_CONFIRM stFriendAddConfirmRsp = msg.stPkgData.stFriendAddConfirmRsp;
                COMDT_FRIEND_INFO stUserInfo = stFriendAddConfirmRsp.stUserInfo;
                if (stFriendAddConfirmRsp.dwResult == 0)
                {
                    Singleton<CUIManager>.GetInstance().OpenTips(string.Format(UT.GetText("Friend_Tips_BeFriend_Ok"), UT.Bytes2String(stUserInfo.szUserName)), false, 1.5f, null, new object[0]);
                    Singleton<CFriendContoller>.GetInstance().model.Remove(CFriendModel.FriendType.RequestFriend, stUserInfo.stUin);
                    Singleton<CFriendContoller>.GetInstance().model.Add(CFriendModel.FriendType.GameFriend, stUserInfo, false);
                    if (this.model.IsBlack(stUserInfo.stUin.ullUid, stUserInfo.stUin.dwLogicWorldId))
                    {
                        this.model.RemoveFriendBlack(stUserInfo.stUin.ullUid, stUserInfo.stUin.dwLogicWorldId);
                    }
                    this.model.AddFriendIntimacy(stUserInfo.stUin, stFriendAddConfirmRsp.stIntimacData);
                    Singleton<CFriendContoller>.GetInstance().model.SortGameFriend();
                    if (stFriendAddConfirmRsp.bFriendSrcType == 2)
                    {
                        this.model.friendRecruit.SetBeiZhaoMuZheRewardData(stUserInfo);
                    }
                }
                else
                {
                    Singleton<CUIManager>.GetInstance().OpenTips(UT.ErrorCode_String(stFriendAddConfirmRsp.dwResult), false, 1.5f, null, new object[0]);
                    this.Remove_And_Refresh(CFriendModel.FriendType.RequestFriend, stUserInfo.stUin);
                }
                goto Label_017E;
            }
            SCPKG_CONFIRM_MASTER_RSP stConfirmMasterRsp = msg.stPkgData.stConfirmMasterRsp;
            Singleton<CFriendContoller>.GetInstance().model.Remove(CFriendModel.FriendType.MentorRequestList, stConfirmMasterRsp.stUin);
        Label_017E:
            view = Singleton<CFriendContoller>.GetInstance().view;
            if ((view != null) && view.IsActive())
            {
                view.Refresh();
            }
        }

        private void On_FriendSys_Friend_Delete(CSPkg msg)
        {
            SCPKG_CMD_DEL_FRIEND stFriendDelRsp = msg.stPkgData.stFriendDelRsp;
            if (stFriendDelRsp.dwResult == 0)
            {
                this.Remove_And_Refresh(CFriendModel.FriendType.GameFriend, stFriendDelRsp.stUin);
            }
        }

        private void On_FriendSys_Friend_Delete_NTF(CSPkg msg)
        {
            SCPKG_CMD_NTF_FRIEND_DEL stNtfFriendDel = msg.stPkgData.stNtfFriendDel;
            this.Remove_And_Refresh(CFriendModel.FriendType.GameFriend, stNtfFriendDel.stUin);
        }

        private void On_FriendSys_Friend_Deny(CSPkg msg)
        {
            SCPKG_CMD_ADD_FRIEND_DENY stFriendAddDenyRsp = msg.stPkgData.stFriendAddDenyRsp;
            if (stFriendAddDenyRsp.dwResult == 0)
            {
                this.Remove_And_Refresh(CFriendModel.FriendType.RequestFriend, stFriendAddDenyRsp.stUin);
            }
            else
            {
                Singleton<CUIManager>.GetInstance().OpenTips(UT.ErrorCode_String(stFriendAddDenyRsp.dwResult), false, 1.5f, null, new object[0]);
                this.Remove_And_Refresh(CFriendModel.FriendType.RequestFriend, stFriendAddDenyRsp.stUin);
            }
        }

        private void On_FriendSys_Friend_List(CSPkg msg)
        {
            SCPKG_CMD_LIST_FRIEND stFriendListRsp = msg.stPkgData.stFriendListRsp;
            if (stFriendListRsp != null)
            {
                int num = Mathf.Min(stFriendListRsp.astFrindList.Length, (int) stFriendListRsp.dwFriendNum);
                for (int i = 0; i < num; i++)
                {
                    CSDT_FRIEND_INFO info = stFriendListRsp.astFrindList[i];
                    this.model.Add(CFriendModel.FriendType.GameFriend, info.stFriendInfo, false);
                    this.Handle_CoinSend_Data(info);
                    this.model.SetFriendGameState(info.stFriendInfo.stUin.ullUid, info.stFriendInfo.stUin.dwLogicWorldId, (COM_ACNT_GAME_STATE) info.bGameState, info.dwGameStartTime, string.Empty, false);
                    this.model.AddFriendIntimacy(info.stFriendInfo.stUin, info.stIntimacyData);
                    if (info.stRecruitmentInfo != null)
                    {
                        this.model.friendRecruit.ParseFriend(info.stFriendInfo, info.stRecruitmentInfo);
                    }
                }
                Singleton<CFriendContoller>.GetInstance().model.SortGameFriend();
                if ((this.view != null) && this.view.IsActive())
                {
                    this.view.Refresh();
                }
                Singleton<EventRouter>.GetInstance().BroadCastEvent("Rank_Friend_List");
            }
        }

        private void On_FriendSys_Friend_Recommand_List(CSPkg msg)
        {
            SCPKG_CMD_LIST_FREC stFRecRsp = msg.stPkgData.stFRecRsp;
            CFriendModel.FriendType friendType = GetFriendType(stFRecRsp.bType);
            this.model.Clear(friendType);
            if (stFRecRsp.dwResult == 0)
            {
                for (int i = 0; i < stFRecRsp.dwAcntNum; i++)
                {
                    COMDT_FRIEND_INFO data = stFRecRsp.astAcnts[i];
                    if ((friendType != CFriendModel.FriendType.GameFriend) || (this.model.getFriendByUid(data.stUin.ullUid, CFriendModel.FriendType.GameFriend, 0) == null))
                    {
                        this.model.Add(friendType, data, false);
                    }
                }
            }
            else
            {
                Singleton<CUIManager>.GetInstance().OpenTips(UT.ErrorCode_String(stFRecRsp.dwResult), false, 1.5f, null, new object[0]);
            }
            if (this.view != null)
            {
                this.view.addFriendView.Refresh_Friend_Recommand_List(friendType);
            }
        }

        private void On_FriendSys_Friend_Request_List(CSPkg msg)
        {
            SCPKG_CMD_LIST_FRIENDREQ stFriendReqListRsp = msg.stPkgData.stFriendReqListRsp;
            if (stFriendReqListRsp != null)
            {
                int num = Mathf.Min(stFriendReqListRsp.astVerificationList.Length, (int) stFriendReqListRsp.dwFriendReqNum);
                for (int i = 0; i < num; i++)
                {
                    CSDT_VERIFICATION_INFO data = stFriendReqListRsp.astVerificationList[i];
                    if (data.bIntimacyState == 0)
                    {
                        this.model.Add(CFriendModel.FriendType.RequestFriend, data.stFriendInfo, false);
                        this.model.AddFriendVerifyContent(data.stFriendInfo.stUin.ullUid, data.stFriendInfo.stUin.dwLogicWorldId, StringHelper.BytesToString(data.szVerificationInfo), CFriendModel.enVerifyDataSet.Friend, data.stUserSource, 0);
                    }
                    else
                    {
                        Debug.Log("---FR On_FriendSys_Friend_Request_List");
                        this.model.FRData.ProcessOtherRequest(data, true);
                    }
                }
                if ((this.view != null) && this.view.IsActive())
                {
                    this.view.Refresh();
                }
            }
        }

        private void On_FriendSys_Friend_Request_NTF(CSPkg msg)
        {
            SCPKG_CMD_NTF_FRIEND_REQUEST stNtfFriendRequest = msg.stPkgData.stNtfFriendRequest;
            if (stNtfFriendRequest != null)
            {
                this.model.AddFriendVerifyContent(stNtfFriendRequest.stUserInfo.stUin.ullUid, stNtfFriendRequest.stUserInfo.stUin.dwLogicWorldId, StringHelper.BytesToString(stNtfFriendRequest.szVerificationInfo), CFriendModel.enVerifyDataSet.Friend, stNtfFriendRequest.stUserSource, 0);
                this.Add_And_Refresh(CFriendModel.FriendType.RequestFriend, stNtfFriendRequest.stUserInfo);
            }
        }

        private void On_FriendSys_Friend_RequestBeFriend(CSPkg msg)
        {
            SCPKG_CMD_ADD_FRIEND stFriendAddRsp = msg.stPkgData.stFriendAddRsp;
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            Singleton<EventRouter>.instance.BroadCastEvent("Friend_SNSFriendList_Refresh");
            UT.ShowFriendNetResult(stFriendAddRsp.dwResult, UT.FriendResultType.RequestBeFriend);
        }

        private void On_FriendSys_Friend_Search(CSPkg msg)
        {
            SCPKG_CMD_SEARCH_PLAYER stFriendSearchPlayerRsp = msg.stPkgData.stFriendSearchPlayerRsp;
            if (stFriendSearchPlayerRsp.dwResult == 0)
            {
                this.search_info = stFriendSearchPlayerRsp.stUserInfo;
                if (this.view != null)
                {
                    this.view.Show_Search_Result(stFriendSearchPlayerRsp.stUserInfo);
                }
            }
            else
            {
                Singleton<CUIManager>.GetInstance().OpenTips(UT.ErrorCode_String(stFriendSearchPlayerRsp.dwResult), false, 1.5f, null, new object[0]);
            }
            this.view.addFriendView.Refresh_Friend_Recommand_List_Pos();
        }

        private void On_GuildSys_Guild_Invite_Success()
        {
            if ((this.view != null) && this.view.IsActive())
            {
                this.view.Refresh();
            }
        }

        private void On_GuildSys_Guild_Recommend_Success()
        {
            if ((this.view != null) && this.view.IsActive())
            {
                this.view.Refresh();
            }
        }

        public void On_Mentor_Login_NTF(CSPkg msg)
        {
            CFriendModel.FriendType mentor;
            SCPKG_CMD_NTF_MASTERSTUDENT_LOGIN_STATUS stMasterStudentLoginNtf = msg.stPkgData.stMasterStudentLoginNtf;
            switch (stMasterStudentLoginNtf.bType)
            {
                case 2:
                    mentor = CFriendModel.FriendType.Mentor;
                    break;

                case 3:
                    mentor = CFriendModel.FriendType.Apprentice;
                    break;

                default:
                    return;
            }
            COMDT_FRIEND_INFO info = Singleton<CFriendContoller>.GetInstance().model.GetInfo(mentor, stMasterStudentLoginNtf.stUin);
            if (info != null)
            {
                info.bIsOnline = stMasterStudentLoginNtf.bLoginStatus;
                info.dwLastLoginTime = (uint) CRoleInfo.GetCurrentUTCTime();
                Singleton<CFriendContoller>.GetInstance().model.SortGameFriend();
                if ((this.view != null) && this.view.IsActive())
                {
                    this.view.Refresh();
                }
            }
        }

        private void On_MentorRequestListEnable(CUIEvent uievt)
        {
            COMDT_FRIEND_INFO infoAtIndex = Singleton<CFriendContoller>.GetInstance().model.GetInfoAtIndex(CFriendModel.FriendType.MentorRequestList, uievt.m_srcWidgetIndexInBelongedList);
            FriendShower component = uievt.m_srcWidget.GetComponent<FriendShower>();
            UT.ShowFriendData(infoAtIndex, component, FriendShower.ItemType.MentorRequest, false, CFriendModel.FriendType.MentorRequestList);
        }

        private void On_OpenAddFriendForm(CUIEvent uiEvent)
        {
            this.view.OpenSearchForm(1);
        }

        private void On_OpenForm(CUIEvent uiEvent)
        {
            this.view.OpenForm(uiEvent);
            CMiShuSystem.SendUIClickToServer(enUIClickReprotID.rp_FrientBtn);
            Singleton<CBattleGuideManager>.GetInstance().OpenBannerDlgByBannerGuideId(5, null, false);
            Singleton<CUINewFlagSystem>.instance.SetNewFlagForFriendEntry(false);
        }

        private void On_Refuse_RequestFriend(CUIEvent uievent)
        {
            FriendShower component = uievent.m_srcWidget.get_transform().get_parent().get_parent().get_parent().GetComponent<FriendShower>();
            if (component != null)
            {
                COMDT_FRIEND_INFO comdt_friend_info = this.model.GetInfo(CFriendModel.FriendType.RequestFriend, component.ullUid, component.dwLogicWorldID);
                if (comdt_friend_info != null)
                {
                    FriendSysNetCore.Send_DENY_BeFriend(comdt_friend_info.stUin);
                }
            }
        }

        public void On_SCID_CMD_BLACKLIST(CSPkg msg)
        {
            SCPKG_CMD_BLACKLIST stBlackListProfile = msg.stPkgData.stBlackListProfile;
            for (int i = 0; i < stBlackListProfile.wBlackListNum; i++)
            {
                COMDT_FRIEND_INFO info = stBlackListProfile.astBlackList[i];
                if (info != null)
                {
                    this.model.AddFriendBlack(info);
                }
            }
        }

        private void On_SNSFriend_SendCoin(CUIEvent uievent)
        {
            ulong ullUid = uievent.m_eventParams.commonUInt64Param1;
            uint dwLogicWorldID = (uint) uievent.m_eventParams.commonUInt64Param2;
            COMDT_FRIEND_INFO comdt_friend_info = this.model.GetInfo(CFriendModel.FriendType.SNS, ullUid, dwLogicWorldID);
            if (comdt_friend_info != null)
            {
                FriendSysNetCore.Send_FriendCoin(comdt_friend_info.stUin, COM_FRIEND_TYPE.COM_FRIEND_TYPE_SNS);
            }
        }

        private void On_TabChange(CUIEvent uievent)
        {
            if ((this.view != null) && this.view.IsActive())
            {
                int selectedIndex = uievent.m_srcWidget.GetComponent<CUIListScript>().GetSelectedIndex();
                this.view.On_Tab_Change(selectedIndex);
            }
        }

        private void OnAddToggle(CUIEvent uievent)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                bool flag = CFriendModel.IsOnSnsSwitch(masterRoleInfo.snsSwitchBits, COM_REFUSE_TYPE.COM_REFUSE_TYPE_ADDFRIEND);
                int num = 2;
                masterRoleInfo.snsSwitchBits ^= (uint) num;
                FriendSysNetCore.Send_Request_Sns_Switch(COM_REFUSE_TYPE.COM_REFUSE_TYPE_ADDFRIEND, flag ? 0 : 1);
            }
        }

        private void OnCancleBlockBtn(CUIEvent uiEvent)
        {
            this.com = uiEvent.m_srcWidget.get_transform().get_parent().get_parent().get_parent().get_gameObject();
            if (this.com != null)
            {
                FriendShower component = this.com.GetComponent<FriendShower>();
                if (component != null)
                {
                    string blackName = this.model.GetBlackName(component.ullUid, component.dwLogicWorldID);
                    string[] args = new string[] { blackName };
                    string strContent = string.Format(Singleton<CTextManager>.instance.GetText("Black_CancleBlockTip", args), new object[0]);
                    Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(strContent, enUIEventID.Friend_CancleBlock_Ok, enUIEventID.Friend_CancleBlock_Cancle, false);
                }
            }
        }

        private void OnCancleBlockBtnOK(CUIEvent evt)
        {
            FriendShower component = this.com.GetComponent<FriendShower>();
            if (component != null)
            {
                FriendSysNetCore.Send_Cancle_Block(component.ullUid, component.dwLogicWorldID);
            }
        }

        public void OnChangeIntimacy(CSPkg msg)
        {
            if (msg.stPkgData.stNtfChgIntimacy.dwResult == 0)
            {
                ushort num;
                CFriendModel.EIntimacyType type;
                bool flag;
                SCPKG_CMD_NTF_CHG_INTIMACY stNtfChgIntimacy = msg.stPkgData.stNtfChgIntimacy;
                if (Singleton<CFriendContoller>.instance.model.GetFriendIntimacy(stNtfChgIntimacy.stUin.ullUid, stNtfChgIntimacy.stUin.dwLogicWorldId, out num, out type, out flag))
                {
                    int num2 = stNtfChgIntimacy.stIntimacData.wIntimacyValue - num;
                    if (num2 > 0)
                    {
                        COMDT_FRIEND_INFO gameOrSnsFriend = this.model.GetGameOrSnsFriend(stNtfChgIntimacy.stUin.ullUid, stNtfChgIntimacy.stUin.dwLogicWorldId);
                        if (gameOrSnsFriend != null)
                        {
                            if (!Singleton<BattleLogic>.instance.isRuning)
                            {
                                this.IntimacyUpInfo = string.Format(UT.GetText("Intimacy_UpInfo"), UT.Bytes2String(gameOrSnsFriend.szUserName), num2);
                                Singleton<CUIManager>.GetInstance().OpenTips(this.IntimacyUpInfo, false, 1.5f, null, new object[0]);
                                this.IntimacyUpInfo = string.Empty;
                                this.IntimacyUpValue = 0;
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(this.IntimacyUpInfo))
                                {
                                    this.IntimacyUpInfo = string.Format(" {0}", UT.Bytes2String(gameOrSnsFriend.szUserName));
                                }
                                else
                                {
                                    this.IntimacyUpInfo = this.IntimacyUpInfo + string.Format(", {0}", UT.Bytes2String(gameOrSnsFriend.szUserName));
                                }
                                this.IntimacyUpValue = num2;
                            }
                        }
                    }
                }
                this.model.AddFriendIntimacy(stNtfChgIntimacy.stUin, stNtfChgIntimacy.stIntimacData);
                Singleton<CFriendContoller>.GetInstance().model.SortGameFriend();
                if ((this.view != null) && this.view.IsActive())
                {
                    this.view.Refresh();
                }
            }
            else if (msg.stPkgData.stNtfChgIntimacy.dwResult == 170)
            {
                this.IntimacyUpInfo = "跟朋友的亲密度已满";
                if (!Singleton<BattleLogic>.instance.isRuning)
                {
                    Singleton<CUIManager>.GetInstance().OpenTips(this.IntimacyUpInfo, false, 1.5f, null, new object[0]);
                    this.IntimacyUpInfo = string.Empty;
                }
                SCPKG_CMD_NTF_CHG_INTIMACY scpkg_cmd_ntf_chg_intimacy2 = msg.stPkgData.stNtfChgIntimacy;
                this.model.AddFriendIntimacy(scpkg_cmd_ntf_chg_intimacy2.stUin, scpkg_cmd_ntf_chg_intimacy2.stIntimacData);
                Singleton<CFriendContoller>.GetInstance().model.SortGameFriend();
                if ((this.view != null) && this.view.IsActive())
                {
                    this.view.Refresh();
                }
            }
        }

        private void OnFriend_Chat_Button(CUIEvent uievent)
        {
            FriendShower component = uievent.m_srcWidget.get_transform().get_parent().get_parent().get_parent().GetComponent<FriendShower>();
            if (component != null)
            {
                COMDT_FRIEND_INFO info = this.model.GetInfo(GetFriendTypeFromItemType((FriendShower.ItemType) uievent.m_eventParams.tag), component.ullUid, component.dwLogicWorldID);
                if (info == null)
                {
                    info = this.model.GetInfo(CFriendModel.FriendType.SNS, component.ullUid, component.dwLogicWorldID);
                    if (info == null)
                    {
                        return;
                    }
                }
                Singleton<CChatController>.instance.Jump2FriendChat(info);
            }
        }

        private void OnFriend_Gift_Button(CUIEvent uievent)
        {
            FriendShower component = uievent.m_srcWidget.get_transform().get_parent().get_parent().get_parent().GetComponent<FriendShower>();
            if (component != null)
            {
                if (!Utility.IsSamePlatformWithSelf(component.dwLogicWorldID))
                {
                    Singleton<CUIManager>.GetInstance().OpenTips("CS_PRESENTHEROSKIN_INVALID_PLAT", true, 1.5f, null, new object[0]);
                }
                else
                {
                    bool isSns = false;
                    if (this.model.GetInfo(GetFriendTypeFromItemType((FriendShower.ItemType) uievent.m_eventParams.tag), component.ullUid, component.dwLogicWorldID) == null)
                    {
                        isSns = true;
                        if (this.model.GetInfo(CFriendModel.FriendType.SNS, component.ullUid, component.dwLogicWorldID) == null)
                        {
                            return;
                        }
                    }
                    Singleton<CMallSystem>.GetInstance().OpenGiftCenter(component.ullUid, component.dwLogicWorldID, isSns);
                }
            }
        }

        private void OnFriend_LBS_CheckInfo(CUIEvent evt)
        {
            FriendShower component = evt.m_srcWidget.get_transform().get_parent().get_parent().get_parent().GetComponent<FriendShower>();
            if (component != null)
            {
                Singleton<CPlayerInfoSystem>.instance.ShowPlayerDetailInfo(component.ullUid, (int) component.dwLogicWorldID, CPlayerInfoSystem.DetailPlayerInfoSource.DefaultOthers, true);
            }
        }

        private void OnFriend_LBS_Nan(CUIEvent evt)
        {
            CFriendModel model = Singleton<CFriendContoller>.instance.model;
            if (model.fileter != 1)
            {
                this.SetFilter(model.NegFlag(model.fileter, 0));
            }
            else
            {
                this.SetFilter(model.fileter);
            }
        }

        private void OnFriend_LBS_NoShare(CUIEvent evt)
        {
            Singleton<CFriendContoller>.instance.model.EnableShareLocation = !Singleton<CFriendContoller>.instance.model.EnableShareLocation;
            bool enableShareLocation = Singleton<CFriendContoller>.instance.model.EnableShareLocation;
            FriendSysNetCore.Send_Request_Sns_Switch(COM_REFUSE_TYPE.COM_REFUSE_TYPE_LBSSHARE, !enableShareLocation ? 0 : 1);
            if (this.view != null)
            {
                this.view.SyncLBSShareBtnState();
                this.view.Refresh_List(this.view.CurTab);
            }
            if (!enableShareLocation)
            {
                FriendSysNetCore.Send_Clear_Location();
            }
            else if (!MonoSingleton<GPSSys>.instance.bGetGPSData)
            {
                MonoSingleton<GPSSys>.instance.StartGPS();
                Singleton<CUIManager>.instance.OpenTips("正在定位，请稍后...", false, 1.5f, null, new object[0]);
            }
            else
            {
                FriendSysNetCore.Send_Report_Clt_Location(MonoSingleton<GPSSys>.instance.iLongitude, MonoSingleton<GPSSys>.instance.iLatitude);
            }
        }

        private void OnFriend_LBS_Nv(CUIEvent evt)
        {
            CFriendModel model = Singleton<CFriendContoller>.instance.model;
            if (model.fileter != 2)
            {
                this.SetFilter(model.NegFlag(model.fileter, 1));
            }
            else
            {
                this.SetFilter(model.fileter);
            }
        }

        private void OnFriend_LBS_Refresh(CUIEvent evt)
        {
            int iLongitude = MonoSingleton<GPSSys>.instance.iLongitude;
            int iLatitude = MonoSingleton<GPSSys>.instance.iLatitude;
            bool isShowAlert = evt.m_eventParams.tag == 0;
            if (!CSysDynamicBlock.bFriendBlocked && ((iLongitude == 0) || (iLatitude == 0)))
            {
                string text = Singleton<CTextManager>.instance.GetText("LBS_Location_Error");
                Singleton<CUIManager>.instance.OpenTips(text, false, 1.5f, null, new object[0]);
                Singleton<CFriendContoller>.instance.model.searchLBSZero = text;
                if ((this.view != null) && (this.view.ifnoText != null))
                {
                    this.view.ifnoText.set_text(text);
                }
            }
            FriendSysNetCore.Send_Search_LBS_Req(this.model.GetLBSGenterFilter(), iLongitude, iLatitude, isShowAlert);
            this.startCooldownTimestamp = Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
        }

        private void OnFriend_Room_AddFriend(CUIEvent evt)
        {
            if ((evt.m_eventParams.commonUInt64Param1 != 0) && (evt.m_eventParams.commonUInt32Param1 != 0))
            {
                this.Open_Friend_Verify(evt.m_eventParams.commonUInt64Param1, evt.m_eventParams.commonUInt32Param1, false, COM_ADD_FRIEND_TYPE.COM_ADD_FRIEND_NULL, -1, true);
            }
        }

        private void OnFriend_Show_Rule(CUIEvent uievent)
        {
            ResRuleText dataByKey = GameDataMgr.s_ruleTextDatabin.GetDataByKey((uint) 12);
            if ((this.view != null) && (this.view.CurTab == CFriendView.Tab.Mentor))
            {
                dataByKey = GameDataMgr.s_ruleTextDatabin.GetDataByKey((uint) 0x15);
            }
            if (dataByKey != null)
            {
                string title = StringHelper.UTF8BytesToString(ref dataByKey.szTitle);
                string info = StringHelper.UTF8BytesToString(ref dataByKey.szContent);
                Singleton<CUIManager>.GetInstance().OpenInfoForm(title, info);
            }
        }

        private void OnIntimacyRela_Menu_Btn_Click(CUIEvent evt)
        {
            if ((this.view != null) && (this.view.intimacyRelationView != null))
            {
                this.view.intimacyRelationView.Open();
            }
            Singleton<CUINewFlagSystem>.GetInstance().DelNewFlag(evt.m_srcWidget, enNewFlagKey.New_Friend_Relation_V1, true);
        }

        public void OnMasterStudentInfo(CSPkg msg)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            SCPKG_MASTERSTUDENT_INFO stMasterStudentInfo = msg.stPkgData.stMasterStudentInfo;
            m_mentorInfo = stMasterStudentInfo;
            if (stMasterStudentInfo.stMaster.stFriendInfo.stUin.ullUid != 0)
            {
                stMasterStudentInfo.stMaster.stFriendInfo.bStudentType = (byte) (stMasterStudentInfo.stMaster.stFriendInfo.bStudentType | 0);
                this.model.Add(CFriendModel.FriendType.Mentor, stMasterStudentInfo.stMaster.stFriendInfo, false);
                this.Handle_CoinSend_Data_Mentor(stMasterStudentInfo.stMaster);
                this.model.SetFriendGameState(stMasterStudentInfo.stMaster.stFriendInfo.stUin.ullUid, stMasterStudentInfo.stMaster.stFriendInfo.stUin.dwLogicWorldId, (COM_ACNT_GAME_STATE) stMasterStudentInfo.stMaster.bGameState, stMasterStudentInfo.stMaster.dwGameStartTime, string.Empty, false);
                this.model.AddFriendIntimacy(stMasterStudentInfo.stMaster.stFriendInfo.stUin, stMasterStudentInfo.stMaster.stIntimacyData);
            }
            for (int i = 0; i < stMasterStudentInfo.bStudentNum; i++)
            {
                stMasterStudentInfo.astStudentList[i].stFriendInfo.bStudentType = (byte) (stMasterStudentInfo.astStudentList[i].stFriendInfo.bStudentType | 0x10);
                this.model.Add(CFriendModel.FriendType.Apprentice, stMasterStudentInfo.astStudentList[i].stFriendInfo, false);
                this.Handle_CoinSend_Data_Apprentice(stMasterStudentInfo.astStudentList[i]);
                this.model.SetFriendGameState(stMasterStudentInfo.astStudentList[i].stFriendInfo.stUin.ullUid, stMasterStudentInfo.astStudentList[i].stFriendInfo.stUin.dwLogicWorldId, (COM_ACNT_GAME_STATE) stMasterStudentInfo.astStudentList[i].bGameState, stMasterStudentInfo.astStudentList[i].dwGameStartTime, string.Empty, false);
                this.model.AddFriendIntimacy(stMasterStudentInfo.astStudentList[i].stFriendInfo.stUin, stMasterStudentInfo.astStudentList[i].stIntimacyData);
            }
            enMentorState mentorState = GetMentorState(0, null);
            if ((this.view != null) && this.view.IsActive())
            {
                this.view.Refresh();
            }
        }

        private void OnMentor_AcceptRequest(CUIEvent uievent)
        {
            FriendShower component = uievent.m_srcWidget.get_transform().get_parent().get_parent().get_parent().GetComponent<FriendShower>();
            this.SendMentorRequestConfirmRefuse(component, COM_MASTERCOMFIRM_TYPE.COM_MASTERCOMFIRM_AGREE);
        }

        public void OnMentor_GetAccountData(CSPkg msg)
        {
            CPlayerProfile profile = Singleton<CPlayerInfoSystem>.instance.GetProfile();
            if (profile._mentorInfo != null)
            {
                profile._mentorInfo.dwMasterPoint = msg.stPkgData.stMasterAcntDataNtf.dwMasterPoint;
                profile._mentorInfo.dwFinishStudentNum = msg.stPkgData.stMasterAcntDataNtf.dwFinishStudentNum;
                profile._mentorInfo.dwStudentNum = msg.stPkgData.stMasterAcntDataNtf.dwProcessStudentNum;
            }
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if ((masterRoleInfo != null) && (masterRoleInfo.m_mentorInfo != null))
            {
                masterRoleInfo.m_mentorInfo.dwMasterPoint = msg.stPkgData.stMasterAcntDataNtf.dwMasterPoint;
                masterRoleInfo.m_mentorInfo.dwFinishStudentNum = msg.stPkgData.stMasterAcntDataNtf.dwFinishStudentNum;
                masterRoleInfo.m_mentorInfo.dwStudentNum = msg.stPkgData.stMasterAcntDataNtf.dwProcessStudentNum;
            }
            Singleton<CPlayerMentorInfoController>.GetInstance().UpdateUI();
        }

        public void OnMentor_GetStudentList(CSPkg msg)
        {
            CFriendModel.FriendType apprentice = CFriendModel.FriendType.Apprentice;
            int bListType = msg.stPkgData.stGetStudentListRsp.bListType;
            enMentorRelationType schoolmate = enMentorRelationType.schoolmate;
            switch (((CS_STUDENTLIST_TYPE) bListType))
            {
                case CS_STUDENTLIST_TYPE.CS_STUDENTLIST_MINE:
                    apprentice = CFriendModel.FriendType.Apprentice;
                    schoolmate = enMentorRelationType.apprentice;
                    break;

                case CS_STUDENTLIST_TYPE.CS_STUDENTLIST_BROTHER:
                    apprentice = CFriendModel.FriendType.Mentor;
                    schoolmate = enMentorRelationType.schoolmate;
                    break;
            }
            this.m_mentorListOff[bListType].m_type = (CS_STUDENTLIST_TYPE) bListType;
            this.m_mentorListOff[bListType].m_isEnd = msg.stPkgData.stGetStudentListRsp.dwOver != 0;
            for (int i = 0; i < msg.stPkgData.stGetStudentListRsp.dwStudentNum; i++)
            {
                COMDT_FRIEND_INFO data = msg.stPkgData.stGetStudentListRsp.astStudentList[i];
                data.bStudentType = (byte) (data.bStudentType | ((byte) (((int) schoolmate) << 4)));
                this.model.Add(apprentice, data, false);
            }
            if ((this.view != null) && this.view.IsActive())
            {
                this.view.Refresh();
            }
        }

        public void OnMentor_GraduateNtf(CSPkg msg)
        {
            switch (msg.stPkgData.stGraduateNtf.bType)
            {
                case 1:
                    if (m_mentorInfo != null)
                    {
                        m_mentorInfo.bStudentType = 2;
                    }
                    break;

                case 2:
                {
                    COMDT_FRIEND_INFO comdt_friend_info = this.model.getFriendByUid(msg.stPkgData.stGraduateNtf.stStudentUin.ullUid, CFriendModel.FriendType.Apprentice, 0);
                    if (comdt_friend_info != null)
                    {
                        comdt_friend_info.bStudentType = (byte) ((comdt_friend_info.bStudentType & 240) | 2);
                    }
                    CPlayerProfile profile = Singleton<CPlayerInfoSystem>.instance.GetProfile();
                    if (profile != null)
                    {
                        profile._mentorInfo.dwFinishStudentNum++;
                    }
                    CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                    if ((masterRoleInfo != null) && (masterRoleInfo.m_mentorInfo != null))
                    {
                        masterRoleInfo.m_mentorInfo.dwFinishStudentNum++;
                    }
                    break;
                }
            }
        }

        private void OnMentor_IWant(CUIEvent evt)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x10f).dwConfValue;
            switch (GetMentorState(0, null))
            {
                case enMentorState.IWantMentor:
                    this.view.OpenSearchForm(2);
                    break;

                case enMentorState.IWantApprentice:
                case enMentorState.IHasApprentice:
                    if (masterRoleInfo.m_rankHistoryHighestGrade >= dwConfValue)
                    {
                        this.view.OpenSearchForm(3);
                        break;
                    }
                    Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Mentor_Err_Grade2LowAsMentor"), false, 1.5f, null, new object[0]);
                    break;

                case enMentorState.IHasMentor:
                    Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Mentor_Err_HasMentor"), false, 1.5f, null, new object[0]);
                    break;
            }
        }

        private void OnMentor_OpenPrivilegePage(CUIEvent uievt)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().OpenForm(MentorPrivilegeFormPath, false, true);
            CFriendView.MentorPrivilege_SetMainList(form);
            CFriendView.MentorPrivilege_Refresh(form, 1);
            this.m_mentorPrivilegePage = 1;
            form.GetWidget(5).CustomSetActive(false);
        }

        private void OnMentor_PrivilegeLeftClick(CUIEvent uievt)
        {
            CUIFormScript srcFormScript = uievt.m_srcFormScript;
            CUIListScript component = srcFormScript.GetWidget(8).GetComponent<CUIListScript>();
            GameObject widget = srcFormScript.GetWidget(5);
            GameObject obj3 = srcFormScript.GetWidget(6);
            this.m_mentorPrivilegePage--;
            if (this.m_mentorPrivilegePage <= 1)
            {
                this.m_mentorPrivilegePage = 1;
                widget.CustomSetActive(false);
            }
            else
            {
                widget.CustomSetActive(true);
            }
            obj3.CustomSetActive(true);
            component.MoveElementInScrollArea(this.m_mentorPrivilegePage - 1, false);
            CFriendView.MentorPrivilege_Refresh(srcFormScript, this.m_mentorPrivilegePage);
        }

        private void OnMentor_PrivilegeListEnable(CUIEvent uievt)
        {
            CUIListScript component = uievt.m_srcFormScript.GetWidget(8).GetComponent<CUIListScript>();
            CFriendView.MentorPrivilegeMainList_OnEnable(uievt);
        }

        private void OnMentor_PrivilegeRightClick(CUIEvent uievt)
        {
            CUIFormScript srcFormScript = uievt.m_srcFormScript;
            CUIListScript component = srcFormScript.GetWidget(8).GetComponent<CUIListScript>();
            int count = GameDataMgr.famousMentorDatabin.count;
            GameObject widget = srcFormScript.GetWidget(5);
            GameObject obj3 = srcFormScript.GetWidget(6);
            this.m_mentorPrivilegePage++;
            if (this.m_mentorPrivilegePage >= count)
            {
                this.m_mentorPrivilegePage = count;
                obj3.CustomSetActive(false);
            }
            else
            {
                obj3.CustomSetActive(true);
            }
            widget.CustomSetActive(true);
            component.MoveElementInScrollArea(this.m_mentorPrivilegePage - 1, false);
            CFriendView.MentorPrivilege_Refresh(srcFormScript, this.m_mentorPrivilegePage);
        }

        private void OnMentor_RefuseRequest(CUIEvent uievent)
        {
            FriendShower component = uievent.m_srcWidget.get_transform().get_parent().get_parent().get_parent().GetComponent<FriendShower>();
            this.SendMentorRequestConfirmRefuse(component, COM_MASTERCOMFIRM_TYPE.COM_MASTERCOMFIRM_DENY);
        }

        public void OnMentor_Reqest_NTF(CSPkg msg)
        {
            SCPKG_MASTERREQ_NTF stMasterReqNtf = msg.stPkgData.stMasterReqNtf;
            if (stMasterReqNtf != null)
            {
                this.model.AddFriendVerifyContent(stMasterReqNtf.stInfo.stReqUserInfo.stUin.ullUid, stMasterReqNtf.stInfo.stReqUserInfo.stUin.dwLogicWorldId, StringHelper.BytesToString(stMasterReqNtf.stInfo.szVerificationInfo), CFriendModel.enVerifyDataSet.Mentor, null, stMasterReqNtf.stInfo.bType);
                this.Add_And_Refresh(CFriendModel.FriendType.MentorRequestList, stMasterReqNtf.stInfo.stReqUserInfo);
            }
        }

        public void OnMentor_RequestList(CSPkg msg)
        {
            SCPKG_MASTERREQ_LIST stMasterReqList = msg.stPkgData.stMasterReqList;
            if (stMasterReqList != null)
            {
                for (int i = 0; i < stMasterReqList.dwNum; i++)
                {
                    this.model.Add(CFriendModel.FriendType.MentorRequestList, stMasterReqList.astMasterReqList[i].stReqUserInfo, false);
                    this.model.AddFriendVerifyContent(stMasterReqList.astMasterReqList[i].stReqUserInfo.stUin.ullUid, stMasterReqList.astMasterReqList[i].stReqUserInfo.stUin.dwLogicWorldId, StringHelper.BytesToString(stMasterReqList.astMasterReqList[i].szVerificationInfo), CFriendModel.enVerifyDataSet.Mentor, null, stMasterReqList.astMasterReqList[i].bType);
                }
                if ((this.view != null) && this.view.IsActive())
                {
                    this.view.Refresh();
                }
            }
        }

        private void OnMentor_RequestListClick(CUIEvent evt)
        {
            this.view.OpenMentorRequestForm();
            this.view.Refresh();
        }

        public void OnMentorApplyVerifyBoxRetrun(string str)
        {
            if (this.m_mentorSelectedUin != null)
            {
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x151a);
                msg.stPkgData.stApplyMasterReq.bType = (byte) s_addViewtype;
                msg.stPkgData.stApplyMasterReq.stUin.dwLogicWorldId = this.m_mentorSelectedUin.dwLogicWorldId;
                msg.stPkgData.stApplyMasterReq.stUin.ullUid = this.m_mentorSelectedUin.ullUid;
                StringHelper.StringToUTF8Bytes(str, ref msg.stPkgData.stApplyMasterReq.szVerificationInfo);
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
            }
        }

        private void OnMentorGetMoreMentorList(CUIEvent uievt)
        {
            FriendSysNetCore.SendGetStudentListReq((CS_STUDENTLIST_TYPE) uievt.m_eventParams.tag);
        }

        public void OnMentorInfoAdd(CSPkg msg)
        {
            SCPKG_MASTERSTUDENT_ADD stMasterStudentAdd = msg.stPkgData.stMasterStudentAdd;
            if (Utility.IsCanShowPrompt())
            {
                string[] args = new string[] { UT.Bytes2String(stMasterStudentAdd.stDetailInfo.szUserName) };
                Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Mentor_RelationshipMade", args), false, 1.5f, null, new object[0]);
            }
            switch (stMasterStudentAdd.bType)
            {
                case 2:
                    this.Add_And_Refresh(CFriendModel.FriendType.Mentor, stMasterStudentAdd.stDetailInfo);
                    break;

                case 3:
                    this.Add_And_Refresh(CFriendModel.FriendType.Apprentice, stMasterStudentAdd.stDetailInfo);
                    break;
            }
        }

        public void OnMentorInfoRemove(CSPkg msg)
        {
            SCPKG_MASTERSTUDENT_DEL stMasterStudentDel = msg.stPkgData.stMasterStudentDel;
            switch (stMasterStudentDel.bType)
            {
                case 2:
                    this.Remove_And_Refresh(CFriendModel.FriendType.Mentor, stMasterStudentDel.stUin);
                    break;

                case 3:
                    this.Remove_And_Refresh(CFriendModel.FriendType.Apprentice, stMasterStudentDel.stUin);
                    break;
            }
        }

        private void OnMentorRequestListClose(CUIEvent uievent)
        {
            GameObject widget = uievent.m_srcFormScript.GetWidget(1);
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                bool flag = CFriendModel.IsOnSnsSwitch(masterRoleInfo.snsSwitchBits, COM_REFUSE_TYPE.COM_REFUSE_TYPE_MASTERREQ);
                uint num = 8;
                bool flag2 = widget.GetComponent<Toggle>().get_isOn();
                if (flag != flag2)
                {
                    if (flag2)
                    {
                        masterRoleInfo.snsSwitchBits |= num;
                    }
                    else
                    {
                        masterRoleInfo.snsSwitchBits &= ~num;
                    }
                }
                FriendSysNetCore.Send_Request_Sns_Switch(COM_REFUSE_TYPE.COM_REFUSE_TYPE_MASTERREQ, !flag ? 0 : 1);
            }
        }

        private void OnMentorTabChange(CUIEvent evt)
        {
            this.view.OnMentorTabChange(evt);
        }

        private void OnMentorTask_Btn_Click(CUIEvent evt)
        {
            if ((this.view != null) && (this.view.mentorTaskView != null))
            {
                this.view.mentorTaskView.Open();
            }
        }

        private void OnNewDayNtf()
        {
            this.model.SnsReCallData.Clear();
            this.model.HeartData.Clear();
            if ((this.view != null) && this.view.IsActive())
            {
                this.view.Refresh();
            }
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                masterRoleInfo.getFriendCoinCnt = 0;
                Singleton<CMailSys>.instance.UpdateUnReadNum();
            }
        }

        public void OnReCallFriendNtf(CSPkg msg)
        {
            uint currentUTCTime = (uint) CRoleInfo.GetCurrentUTCTime();
            uint num2 = 0x15180 * GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x9e).dwConfValue;
            for (int i = 0; i < msg.stPkgData.stNtfRecallFirend.wRecallNum; i++)
            {
                COMDT_RECALL_FRIEND comdt_recall_friend = msg.stPkgData.stNtfRecallFirend.astRecallAcntList[i];
                if (comdt_recall_friend != null)
                {
                    this.Handle_Invite_Data(comdt_recall_friend.stAcntUniq);
                }
            }
            Singleton<CFriendContoller>.instance.model.friendRecruit.SetRecruiterRewardBits(msg.stPkgData.stNtfRecallFirend.dwRecruiterRewardBits);
            Singleton<CFriendContoller>.GetInstance().model.FRData.SetFirstChoiseState(msg.stPkgData.stNtfRecallFirend.bIntimacyRelationPrior);
        }

        public void OnSCPKG_CMD_BLACKFORFRIENDREQ(CSPkg msg)
        {
        }

        public void OnSCPKG_CMD_CANCEL_DEFRIEND(CSPkg msg)
        {
            SCPKG_CMD_CANCEL_DEFRIEND stCancelDeFriendRsp = msg.stPkgData.stCancelDeFriendRsp;
            if (stCancelDeFriendRsp.dwResult == 0)
            {
                this.model.RemoveFriendBlack(stCancelDeFriendRsp.stUin.ullUid, stCancelDeFriendRsp.stUin.dwLogicWorldId);
                if ((this.view != null) && this.view.IsActive())
                {
                    this.view.Refresh();
                }
            }
            else
            {
                Singleton<CUIManager>.instance.OpenTips("---black OnSCPKG_CMD_CANCEL_DEFRIEND error code:" + stCancelDeFriendRsp.dwResult, false, 1.5f, null, new object[0]);
            }
        }

        public void OnSCPKG_CMD_DEFRIEND(CSPkg msg)
        {
            SCPKG_CMD_DEFRIEND stDeFriendRsp = msg.stPkgData.stDeFriendRsp;
            if (stDeFriendRsp.dwResult == 0)
            {
                COMDT_FRIEND_INFO comdt_friend_info;
                COMDT_CHAT_PLAYER_INFO comdt_chat_player_info;
                this.model.GetUser(stDeFriendRsp.stUin.ullUid, stDeFriendRsp.stUin.dwLogicWorldId, out comdt_friend_info, out comdt_chat_player_info);
                if (comdt_friend_info == null)
                {
                    if (comdt_chat_player_info != null)
                    {
                        this.model.AddFriendBlack(comdt_chat_player_info, stDeFriendRsp.bGender, stDeFriendRsp.dwLastLoginTime);
                        if ((this.view != null) && this.view.IsActive())
                        {
                            this.view.Refresh();
                        }
                    }
                    else
                    {
                        DebugHelper.Assert(false, string.Concat(new object[] { "---black 找到不到 ulluid:", stDeFriendRsp.stUin.ullUid, ",worldID:", stDeFriendRsp.stUin.dwLogicWorldId, ",对应的玩家数据" }));
                    }
                }
                else
                {
                    this.model.AddFriendBlack(comdt_friend_info);
                    if ((this.view != null) && this.view.IsActive())
                    {
                        this.view.Refresh();
                    }
                }
                Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.PlayerBlock_Success);
            }
            else
            {
                Singleton<CUIManager>.GetInstance().OpenTips(UT.ErrorCode_String(stDeFriendRsp.dwResult), false, 1.5f, null, new object[0]);
            }
        }

        private void OnShareToggle(CUIEvent uievent)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                bool flag = CFriendModel.IsOnSnsSwitch(masterRoleInfo.snsSwitchBits, COM_REFUSE_TYPE.COM_REFUSE_TYPE_DONOTE_AND_REC);
                int num = 1;
                masterRoleInfo.snsSwitchBits ^= (uint) num;
                FriendSysNetCore.Send_Request_Sns_Switch(COM_REFUSE_TYPE.COM_REFUSE_TYPE_DONOTE_AND_REC, flag ? 0 : 1);
            }
        }

        public void OnSnsSwitchNtf(CSPkg msg)
        {
            COMDT_ACNT_UNIQ stUin = msg.stPkgData.stNtfRefuseRecall.stUin;
            COMDT_FRIEND_INFO comdt_friend_info = this.model.getFriendByUid(stUin.ullUid, CFriendModel.FriendType.SNS, 0);
            if (comdt_friend_info != null)
            {
                comdt_friend_info.dwRefuseFriendBits = msg.stPkgData.stNtfRefuseRecall.dwRefuseFriendBits;
            }
        }

        public void OnSnsSwitchRsp(CSPkg msg)
        {
            if (msg.stPkgData.stRefuseRecallRsp.dwResult == 0)
            {
            }
        }

        public void Open_Friend_Verify(ulong ullUid, uint dwLogicWorldId, bool bAddSearchFirend, COM_ADD_FRIEND_TYPE addFriendType = 0, int useHeroId = -1, bool onlyAddFriend = true)
        {
            if ((this.view != null) && (this.view.verficationView != null))
            {
                this.view.verficationView.Open(ullUid, dwLogicWorldId, bAddSearchFirend, addFriendType, useHeroId, onlyAddFriend);
            }
        }

        private void QQBox_OnClick(CUIEvent uievent)
        {
            if (ApolloConfig.platform == ApolloPlatform.QQ)
            {
                MonoSingleton<IDIPSys>.GetInstance().RequestQQBox();
            }
        }

        public int RefreshMentorTabData()
        {
            int num = 0;
            MentorTabMask = 0;
            if (s_mentorTabStr == null)
            {
                s_mentorTabStr = new string[2];
                for (int i = 0; i < 2; i++)
                {
                    s_mentorTabStr[i] = Singleton<CTextManager>.GetInstance().GetText(s_mentorTabName[i]);
                }
            }
            if (m_mentorInfo != null)
            {
                if (Singleton<CFriendContoller>.GetInstance().HasMentor(null))
                {
                    MentorTabMask |= 1;
                    num++;
                }
                if (Singleton<CFriendContoller>.GetInstance().HasApprentice(null))
                {
                    MentorTabMask |= 2;
                    num++;
                }
            }
            return num;
        }

        public void Remove_And_Refresh(CFriendModel.FriendType type, COMDT_ACNT_UNIQ uniq)
        {
            this.model.Remove(type, uniq);
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            switch (type)
            {
                case CFriendModel.FriendType.Mentor:
                    if (m_mentorInfo != null)
                    {
                        m_mentorInfo.stMaster = new CSDT_FRIEND_INFO();
                        masterRoleInfo.m_mentorInfo.ullMasterUid = 0L;
                        masterRoleInfo.m_mentorInfo.dwMasterLogicWorldID = 0;
                        CPlayerProfile profile = Singleton<CPlayerInfoSystem>.instance.GetProfile();
                        if ((profile != null) && (profile._mentorInfo != null))
                        {
                            profile._mentorInfo.szRoleName = null;
                        }
                        Singleton<CPlayerMentorInfoController>.GetInstance().UpdateUI();
                    }
                    break;

                case CFriendModel.FriendType.Apprentice:
                    if (m_mentorInfo != null)
                    {
                        int num = -1;
                        for (int i = 0; i < m_mentorInfo.bStudentNum; i++)
                        {
                            if ((m_mentorInfo.astStudentList[i].stFriendInfo.stUin.dwLogicWorldId == uniq.dwLogicWorldId) && (m_mentorInfo.astStudentList[i].stFriendInfo.stUin.ullUid == uniq.ullUid))
                            {
                                num = i;
                                break;
                            }
                        }
                        if (num != -1)
                        {
                            for (int j = num; j < (m_mentorInfo.bStudentNum - 1); j++)
                            {
                                m_mentorInfo.astStudentList[j] = m_mentorInfo.astStudentList[j + 1];
                            }
                            m_mentorInfo.bStudentNum = (byte) (m_mentorInfo.bStudentNum - 1);
                        }
                    }
                    goto Label_0175;
            }
        Label_0175:
            if (this.view.IsActive())
            {
                this.view.Refresh();
            }
        }

        private void SendMentorRequestConfirmRefuse(FriendShower shower, COM_MASTERCOMFIRM_TYPE confType)
        {
            if (shower != null)
            {
                COMDT_FRIEND_INFO comdt_friend_info = this.model.GetInfo(CFriendModel.FriendType.MentorRequestList, shower.ullUid, shower.dwLogicWorldID);
                if (comdt_friend_info != null)
                {
                    stFriendVerifyContent content = this.model.GetFriendVerifyData(comdt_friend_info.stUin.ullUid, comdt_friend_info.stUin.dwLogicWorldId, CFriendModel.enVerifyDataSet.Mentor);
                    if (content != null)
                    {
                        CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x151c);
                        msg.stPkgData.stConfirmMasterReq.bConfirmType = (byte) confType;
                        msg.stPkgData.stConfirmMasterReq.stUin.ullUid = comdt_friend_info.stUin.ullUid;
                        msg.stPkgData.stConfirmMasterReq.stUin.dwLogicWorldId = comdt_friend_info.stUin.dwLogicWorldId;
                        msg.stPkgData.stConfirmMasterReq.bReqType = (byte) content.mentorType;
                        Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
                    }
                }
            }
        }

        public void SetFilter(uint value)
        {
            this.model.fileter = value;
            if (this.view != null)
            {
                this.view.SyncGenderToggleState();
                this.view.Refresh_List(this.view.CurTab);
            }
        }

        public void ShareTo_SNSFriend_ReCall(string openId)
        {
            if (MonoSingleton<ShareSys>.instance.IsInstallPlatform())
            {
                string text = Singleton<CTextManager>.instance.GetText("Common_Sns_Tips_11");
                string desc = Singleton<CTextManager>.instance.GetText("Common_Sns_Tips_12");
                Singleton<ApolloHelper>.instance.ShareInviteFriend(openId, text, desc, ShareSys.SNS_SHARE_RECALL_FRIEND);
                Singleton<CUIManager>.instance.OpenTips("Common_Sns_Tips_13", true, 1.5f, null, new object[0]);
            }
        }

        public void Update()
        {
            this.view.Update();
        }

        private void Update_Send_Coin_Data(COMDT_ACNT_UNIQ uin, ulong donateAPSec)
        {
            DateTime time = Utility.ToUtcTime2Local((long) donateAPSec);
            DateTime time2 = Utility.ToUtcTime2Local((long) CRoleInfo.GetCurrentUTCTime());
            if (((time2.Year == time.Year) && (time2.Month == time.Month)) && (time2.Day == time.Day))
            {
                this.model.HeartData.Add(uin);
            }
            if (((time2.Year >= time.Year) && ((time2.Year != time.Year) || (time2.Month >= time.Month))) && (((time2.Year != time.Year) || (time2.Month != time.Month)) || (time2.Day < time.Day)))
            {
            }
        }

        public ulong startCooldownTimestamp
        {
            [CompilerGenerated]
            get
            {
                return this.<startCooldownTimestamp>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.<startCooldownTimestamp>k__BackingField = value;
            }
        }

        public enum enMentorTab
        {
            MentorAndClassmate,
            Apprentice,
            Count
        }
    }
}

