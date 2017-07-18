namespace Assets.Scripts.GameSystem
{
    using Apollo;
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class CFriendView
    {
        private Tab _tab;
        private GameObject addFriendBtnGameObject;
        public AddFriendView addFriendView = new AddFriendView();
        public GameObject bgLineNode;
        private Text btnText;
        private CUIFormScript friendform;
        public CUIListScript friendListCom;
        public GameObject friendListNode;
        private Vector2 friendListPosOrg;
        private Rect friendListRectOrg;
        private Vector2 friendListSizeDeltaOrg;
        private FriendRecruitView friendRecruitView = new FriendRecruitView();
        public Text ifnoText;
        public GameObject imgNode;
        private GameObject info_node;
        public IntimacyRelationView intimacyRelationView = new IntimacyRelationView();
        private GameObject lbs_node;
        private Button lbsRefreshBtn;
        private Toggle localtionToggle;
        private GameObject m_IntimacyRelationBtn;
        private int m_lastMCLevel = -1;
        private CFriendModel.FriendType m_listFriendType;
        private FriendShower.ItemType m_listItemType;
        private GameObject m_QQboxBtn;
        public CFriendMentorTaskView mentorTaskView = new CFriendMentorTaskView();
        private Toggle nanToggle;
        private Toggle nvToggle;
        private GameObject rule_btn_node;
        private GameObject sns_add_switch;
        private GameObject sns_invite_btn;
        private GameObject sns_share_switch;
        private CUIListScript tablistScript;
        public TabMgr tabMgr = new TabMgr();
        public Verfication verficationView = new Verfication();

        private COMDT_FRIEND_INFO _get_current_info(int index)
        {
            return Singleton<CFriendContoller>.GetInstance().model.GetInfoAtIndex(this.m_listFriendType, index);
        }

        private void _refresh_black_list(CUIListScript listScript, List<CFriendModel.stBlackName> blackList)
        {
            if (listScript != null)
            {
                int count = blackList.Count;
                listScript.SetElementAmount(count);
                for (int i = 0; i < count; i++)
                {
                    CUIListElementScript elemenet = listScript.GetElemenet(i);
                    if ((elemenet != null) && listScript.IsElementInScrollArea(i))
                    {
                        FriendShower component = elemenet.GetComponent<FriendShower>();
                        CFriendModel.stBlackName info = blackList[i];
                        if (component != null)
                        {
                            UT.ShowBlackListData(ref info, component);
                        }
                    }
                }
            }
        }

        private void _refresh_LBS_list(CUIListScript listScript, ListView<CSDT_LBS_USER_INFO> LBSList)
        {
            if (listScript != null)
            {
                if (LBSList == null)
                {
                    listScript.SetElementAmount(0);
                }
                else
                {
                    int count = LBSList.Count;
                    listScript.SetElementAmount(count);
                }
            }
        }

        private void _refresh_list(CUIListScript listScript, ListView<COMDT_FRIEND_INFO> data_list, FriendShower.ItemType type, bool bShowNickName, CFriendModel.FriendType friend)
        {
            if (listScript != null)
            {
                this.m_listItemType = type;
                this.m_listFriendType = friend;
                int count = data_list.Count;
                switch (friend)
                {
                    case CFriendModel.FriendType.Mentor:
                    case CFriendModel.FriendType.Apprentice:
                        Singleton<CFriendContoller>.GetInstance().model.SortShower(friend);
                        break;
                }
                listScript.SetElementAmount(count);
            }
        }

        public void Clear()
        {
            if (this.tablistScript != null)
            {
                CUICommonSystem.DelRedDot(this.tablistScript.GetElemenet(0).get_gameObject());
                CUICommonSystem.DelRedDot(this.tablistScript.GetElemenet(1).get_gameObject());
                if (!CSysDynamicBlock.bSocialBlocked)
                {
                    CUICommonSystem.DelRedDot(this.tablistScript.GetElemenet(2).get_gameObject());
                }
            }
            this.bgLineNode = null;
            this.friendListNode = null;
            this.friendListCom = null;
            this.addFriendBtnGameObject = null;
            this.imgNode = null;
            this.info_node = null;
            this.btnText = null;
            this.ifnoText = null;
            this.friendform = null;
            this.sns_invite_btn = null;
            this.lbs_node = null;
            this.m_QQboxBtn = null;
            this.sns_share_switch = null;
            this.sns_add_switch = null;
            this.rule_btn_node = null;
            this.m_IntimacyRelationBtn = null;
            this.localtionToggle = this.nanToggle = (Toggle) (this.nvToggle = null);
            this.lbsRefreshBtn = null;
            this.tablistScript = null;
            if (this.addFriendView != null)
            {
                this.addFriendView.Clear();
            }
            if (this.intimacyRelationView != null)
            {
                this.intimacyRelationView.Clear();
            }
            if (this.mentorTaskView != null)
            {
                this.mentorTaskView.Clear();
            }
            if (this.friendRecruitView != null)
            {
                this.friendRecruitView.Clear();
            }
            if (this.tabMgr != null)
            {
                this.tabMgr.Clear();
            }
            this.m_lastMCLevel = -1;
        }

        public void CloseForm()
        {
            this.Clear();
        }

        public Tab GetSelectedTab()
        {
            if (this.tablistScript != null)
            {
                int selectedIndex = this.tablistScript.GetSelectedIndex();
                TabMgr.TabElement tabElement = Singleton<CFriendContoller>.GetInstance().view.tabMgr.GetTabElement(selectedIndex);
                if (tabElement != null)
                {
                    return tabElement.tab;
                }
            }
            return Tab.None;
        }

        public bool IsActive()
        {
            return (this.friendform != null);
        }

        public static void MentorPrivilege_Refresh(CUIFormScript form, int currDisLv)
        {
            string prefabPath = string.Format("{0}{1}.prefab", CUIUtility.s_Sprite_System_ShareUI_Dir, currDisLv.ToString());
            Image component = form.GetWidget(0).GetComponent<Image>();
            Image image = form.GetWidget(7).GetComponent<Image>();
            CUIUtility.SetImageSprite(component, prefabPath, null, true, false, false, false);
            CUIUtility.SetImageSprite(image, prefabPath, null, true, false, false, false);
            Text text = form.GetWidget(4).GetComponent<Text>();
            ResFamousMentor dataByKey = GameDataMgr.famousMentorDatabin.GetDataByKey((long) currDisLv);
            string[] args = new string[] { dataByKey.dwPoint.ToString() };
            text.set_text(Singleton<CTextManager>.GetInstance().GetText("Mentor_Requirement", args));
            form.GetWidget(3).GetComponent<Text>().set_text(dataByKey.szTitle);
        }

        public static void MentorPrivilege_SetMainList(CUIFormScript form)
        {
            form.GetWidget(8).GetComponent<CUIListScript>().SetElementAmount(GameDataMgr.famousMentorDatabin.count);
        }

        public static void MentorPrivilegeMainList_OnEnable(CUIEvent uievt)
        {
            ResFamousMentor dataByKey = GameDataMgr.famousMentorDatabin.GetDataByKey((long) (uievt.m_srcWidgetIndexInBelongedList + 1));
            if (dataByKey == null)
            {
                Debug.Log("MentorPrivilege_Refresh dont get famose mentor data!");
            }
            else
            {
                CUIListElementScript component = uievt.m_srcWidget.GetComponent<CUIListElementScript>();
                if (component != null)
                {
                    GameObject obj2 = component.get_transform().Find("Item/GiftObj/GiftsContainer").get_gameObject();
                    ResPropInfo info = GameDataMgr.itemDatabin.GetDataByKey(dataByKey.dwLvUpBonusPackage);
                    Image image = component.get_transform().Find("Item/NobeObj/imgMyPointBg/tipsRight/privilegeLvCount").GetComponent<Image>();
                    Image image2 = component.get_transform().Find("Item/GiftObj/imgMyPointBg/tipsRight/bonusLvCount").GetComponent<Image>();
                    string prefabPath = string.Format("{0}{1}.prefab", CUIUtility.s_Sprite_System_ShareUI_Dir, (uievt.m_srcWidgetIndexInBelongedList + 1).ToString());
                    CUIUtility.SetImageSprite(image, prefabPath, null, true, false, false, false);
                    CUIUtility.SetImageSprite(image2, prefabPath, null, true, false, false, false);
                    if (info != null)
                    {
                        ResRandomRewardStore store = GameDataMgr.randomRewardDB.GetDataByKey((long) ((int) info.EftParam[0]));
                        if (store != null)
                        {
                            int index = 0;
                            index = 0;
                            while (index < store.astRewardDetail.Length)
                            {
                                if (index >= 6)
                                {
                                    break;
                                }
                                Transform transform = obj2.get_transform().Find("Gift" + index);
                                if (store.astRewardDetail[index].dwItemID == 0)
                                {
                                    transform.get_gameObject().CustomSetActive(false);
                                }
                                else
                                {
                                    transform.get_gameObject().CustomSetActive(true);
                                    CUseable useable = CUseableManager.CreateUsableByRandowReward((RES_RANDOM_REWARD_TYPE) store.astRewardDetail[index].bItemType, (int) store.astRewardDetail[index].dwLowCnt, store.astRewardDetail[index].dwItemID);
                                    transform.get_gameObject().CustomSetActive(true);
                                    Transform transform2 = transform.FindChild(string.Format("Icon", new object[0]));
                                    Transform transform3 = transform.FindChild(string.Format("Text", new object[0]));
                                    Transform transform4 = transform.FindChild(string.Format("ExperienceCard", new object[0]));
                                    transform.FindChild(string.Format("TextNum", new object[0])).get_gameObject().GetComponent<Text>().set_text(store.astRewardDetail[index].dwLowCnt.ToString());
                                    if (transform4 != null)
                                    {
                                        transform4.get_gameObject().CustomSetActive(CItem.IsSkinExperienceCard(store.astRewardDetail[index].dwItemID) || CItem.IsHeroExperienceCard(store.astRewardDetail[index].dwItemID));
                                    }
                                    if ((transform2 != null) && (transform3 != null))
                                    {
                                        transform2.get_gameObject().GetComponent<Image>().SetSprite(useable.GetIconPath(), component.m_belongedFormScript, true, false, false, false);
                                        transform3.get_gameObject().GetComponent<Text>().set_text(useable.m_name);
                                    }
                                }
                                index++;
                            }
                            while (index < 6)
                            {
                                obj2.get_transform().Find("Gift" + index).get_gameObject().CustomSetActive(false);
                                index++;
                            }
                        }
                    }
                    GameObject obj3 = component.get_transform().Find("Item/NobeObj/privilegeContainer").get_gameObject();
                    for (int i = 0; i < 6; i++)
                    {
                        Transform transform7 = obj3.get_transform().Find("itemCell" + i);
                        if (string.IsNullOrEmpty(dataByKey.astPrivilegeIcon[i].szPrivilegeIcon))
                        {
                            transform7.get_gameObject().CustomSetActive(false);
                        }
                        else
                        {
                            transform7.get_gameObject().CustomSetActive(true);
                            Transform transform8 = transform7.FindChild("imgback");
                            Transform transform9 = transform7.FindChild("Text");
                            if ((transform8 != null) && (transform9 != null))
                            {
                                transform8.get_gameObject().GetComponent<Image>().SetSprite("UGUI/Sprite/Dynamic/mentor/" + dataByKey.astPrivilegeIcon[i].szPrivilegeIcon, component.m_belongedFormScript, true, false, false, false);
                                transform9.GetComponent<Text>().set_text(dataByKey.astPrivilegeIcon[i].szPrivilegeDesc);
                            }
                        }
                    }
                }
            }
        }

        public void On_Friend_Invite_SNS_Friend(CUIEvent uiEvent)
        {
            if (MonoSingleton<ShareSys>.instance.IsInstallPlatform())
            {
                string text = UT.GetText("Friend_Invite_SNS_Title");
                string desc = UT.GetText("Friend_Invite_SNS_Desc");
                Singleton<ApolloHelper>.GetInstance().ShareToFriend(text, desc);
            }
        }

        public void On_List_ElementEnable(CUIEvent uievent)
        {
            int srcWidgetIndexInBelongedList = uievent.m_srcWidgetIndexInBelongedList;
            FriendShower component = uievent.m_srcWidget.GetComponent<FriendShower>();
            if (component != null)
            {
                if (component.mentor_relationship != null)
                {
                    component.mentor_relationship.CustomSetActive(false);
                }
                if (component.mentor_graduation != null)
                {
                    component.mentor_graduation.CustomSetActive(false);
                }
                switch (this.CurTab)
                {
                    case Tab.Friend_Request:
                        UT.ShowFriendData(this._get_current_info(srcWidgetIndexInBelongedList), component, FriendShower.ItemType.Request, false, CFriendModel.FriendType.RequestFriend);
                        return;

                    case Tab.Friend_BlackList:
                    {
                        List<CFriendModel.stBlackName> blackList = Singleton<CFriendContoller>.instance.model.GetBlackList();
                        if (srcWidgetIndexInBelongedList < blackList.Count)
                        {
                            CFriendModel.stBlackName name = blackList[srcWidgetIndexInBelongedList];
                            if (component != null)
                            {
                                UT.ShowBlackListData(ref name, component);
                            }
                        }
                        return;
                    }
                    case Tab.Friend_LBS:
                    {
                        ListView<CSDT_LBS_USER_INFO> currentLBSList = Singleton<CFriendContoller>.instance.model.GetCurrentLBSList();
                        if ((currentLBSList != null) && (srcWidgetIndexInBelongedList < currentLBSList.Count))
                        {
                            CSDT_LBS_USER_INFO csdt_lbs_user_info = currentLBSList[srcWidgetIndexInBelongedList];
                            if (component != null)
                            {
                                UT.ShowLBSUserData(csdt_lbs_user_info, component);
                            }
                        }
                        return;
                    }
                }
                COMDT_FRIEND_INFO info = this._get_current_info(srcWidgetIndexInBelongedList);
                if (info != null)
                {
                    UT.ShowFriendData(info, component, this.m_listItemType, this.CurTab == Tab.Friend_SNS, this.m_listFriendType);
                    int srvFriendTypeFromFriendType = CFriendContoller.GetSrvFriendTypeFromFriendType(this.m_listFriendType);
                    if (srvFriendTypeFromFriendType >= 0)
                    {
                        component.sendHeartBtn_eventScript.m_onClickEventID = enUIEventID.Friend_SendCoin;
                        component.sendHeartBtn_eventScript.m_onClickEventParams.tag = srvFriendTypeFromFriendType;
                        component.sendHeartBtn_eventScript.m_onClickEventParams.commonUInt64Param1 = info.stUin.ullUid;
                        component.sendHeartBtn_eventScript.m_onClickEventParams.commonUInt64Param2 = info.stUin.dwLogicWorldId;
                    }
                }
            }
        }

        private void On_SearchFriend(CUIEvent uiEvent)
        {
            this.addFriendView.On_SearchFriend(uiEvent);
        }

        public void On_Tab_Change(int index)
        {
            TabMgr.TabElement tabElement = this.tabMgr.GetTabElement(index);
            if (tabElement != null)
            {
                this.CurTab = tabElement.tab;
                if (this.tablistScript != null)
                {
                    if (this.CurTab == Tab.Friend_LBS)
                    {
                        Singleton<CUINewFlagSystem>.GetInstance().DelNewFlag(this.tablistScript.GetElemenet(this.tabMgr.GetIndex(this.CurTab)).get_gameObject(), enNewFlagKey.New_PeopleNearby_V1, true);
                        CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                        if ((masterRoleInfo != null) && !masterRoleInfo.IsClientBitsSet(4))
                        {
                            Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("Friend_LBSFristTimeOpen_Tip"), enUIEventID.Friend_LBS_NoShare, enUIEventID.None, false);
                            masterRoleInfo.SetClientBits(4, true, true);
                        }
                    }
                    else if (this.CurTab == Tab.Mentor)
                    {
                        Singleton<CUINewFlagSystem>.GetInstance().DelNewFlag(this.tablistScript.GetElemenet(this.tabMgr.GetIndex(this.CurTab)).get_gameObject(), enNewFlagKey.New_Friend_Mentor_V1, true);
                    }
                    else if (this.CurTab == Tab.Friend_Recruit)
                    {
                        Singleton<CUINewFlagSystem>.GetInstance().DelNewFlag(this.tablistScript.GetElemenet(this.tabMgr.GetIndex(this.CurTab)).get_gameObject(), enNewFlagKey.New_Friend_Recruit_V1, true);
                    }
                    else if (this.CurTab == Tab.Friend)
                    {
                        Singleton<CUINewFlagSystem>.GetInstance().DelNewFlag(this.tablistScript.GetElemenet(this.tabMgr.GetIndex(this.CurTab)).get_gameObject(), enNewFlagKey.New_Friend_Friend_V1, true);
                        Transform transform = this.friendform.get_transform().FindChild("node/Relation");
                        if (transform != null)
                        {
                            Singleton<CUINewFlagSystem>.GetInstance().AddNewFlag(transform.get_gameObject(), enNewFlagKey.New_Friend_Relation_V1, enNewFlagPos.enTopRight, 1f, 0f, 0f, enNewFlagType.enNewFlag);
                        }
                    }
                }
            }
        }

        public void OnMentorTabChange(CUIEvent evt)
        {
            try
            {
                CUIListScript srcWidgetScript = evt.m_srcWidgetScript;
                CFriendContoller.enMentorTab tab = (CFriendContoller.enMentorTab) Convert.ToInt32(srcWidgetScript.GetSelectedElement().GetDataTag());
                CFriendModel.FriendType mentor = CFriendModel.FriendType.Mentor;
                FriendShower.ItemType normal = FriendShower.ItemType.Normal;
                GameObject widget = evt.m_srcFormScript.GetWidget(12);
                GameObject obj3 = evt.m_srcFormScript.GetWidget(11);
                CUIListScript component = null;
                if (obj3 != null)
                {
                    component = obj3.GetComponent<CUIListScript>();
                }
                bool isEnabled = false;
                CS_STUDENTLIST_TYPE cs_studentlist_type = CS_STUDENTLIST_TYPE.CS_STUDENTLIST_BROTHER;
                switch (tab)
                {
                    case CFriendContoller.enMentorTab.MentorAndClassmate:
                        mentor = CFriendModel.FriendType.Mentor;
                        normal = FriendShower.ItemType.Mentor;
                        isEnabled = Singleton<CFriendContoller>.GetInstance().m_mentorListOff[2].needShowMore();
                        if (component != null)
                        {
                            component.EnableExtraContent(isEnabled);
                        }
                        cs_studentlist_type = CS_STUDENTLIST_TYPE.CS_STUDENTLIST_BROTHER;
                        break;

                    case CFriendContoller.enMentorTab.Apprentice:
                        mentor = CFriendModel.FriendType.Apprentice;
                        normal = FriendShower.ItemType.Apprentice;
                        isEnabled = Singleton<CFriendContoller>.GetInstance().m_mentorListOff[1].needShowMore();
                        if (component != null)
                        {
                            component.EnableExtraContent(isEnabled);
                        }
                        cs_studentlist_type = CS_STUDENTLIST_TYPE.CS_STUDENTLIST_MINE;
                        goto Label_00E5;
                }
            Label_00E5:
                if (isEnabled)
                {
                    CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
                    uIEvent.m_eventID = enUIEventID.Mentor_GetMoreMentor;
                    uIEvent.m_eventParams.tag = (int) cs_studentlist_type;
                    widget.GetComponent<CUIEventScript>().SetUIEvent(enUIEventType.Click, uIEvent.m_eventID, uIEvent.m_eventParams);
                }
                ListView<COMDT_FRIEND_INFO> list = Singleton<CFriendContoller>.instance.model.GetList(mentor);
                if (list != null)
                {
                    this._refresh_list(this.friendListCom, list, normal, true, mentor);
                    this.friendListCom.get_gameObject().CustomSetActive(true);
                    if (this.info_node != null)
                    {
                        this.info_node.CustomSetActive(list.Count == 0);
                    }
                }
            }
            catch (Exception)
            {
                Debug.LogError("CFriendView + OnMentorTabChange()");
            }
        }

        public void OpenForm(CUIEvent uiEvent)
        {
            this.friendform = Singleton<CUIManager>.GetInstance().OpenForm(CFriendContoller.FriendFormPath, false, true);
            GameObject p = this.friendform.get_gameObject();
            GameObject widget = this.friendform.GetWidget(11);
            if (widget != null)
            {
                RectTransform transform = widget.get_transform() as RectTransform;
                this.friendListSizeDeltaOrg = transform.get_sizeDelta();
                this.friendListPosOrg = transform.get_anchoredPosition();
                this.friendListRectOrg = transform.get_rect();
            }
            this.imgNode = p.get_transform().Find("node/Image").get_gameObject();
            this.bgLineNode = p.get_transform().Find("node/Bg").get_gameObject();
            this.friendListNode = p.get_transform().Find("node/Image/FriendList").get_gameObject();
            this.friendListNode.CustomSetActive(true);
            this.friendListCom = this.friendListNode.GetComponent<CUIListScript>();
            this.addFriendBtnGameObject = Utility.FindChild(p, "node/Buttons/Add");
            this.info_node = p.get_transform().Find("node/Image/info_node").get_gameObject();
            this.info_node.CustomSetActive(false);
            this.ifnoText = Utility.GetComponetInChild<Text>(p, "node/Image/info_node/Text");
            this.ifnoText.set_text(Singleton<CTextManager>.instance.GetText("Friend_NoFriend_Tip"));
            string text = Singleton<CTextManager>.instance.GetText("FriendAdd_Tab_QQ");
            if (Singleton<ApolloHelper>.GetInstance().CurPlatform == ApolloPlatform.Wechat)
            {
                text = Singleton<CTextManager>.instance.GetText("FriendAdd_Tab_Weixin");
            }
            this.tablistScript = p.get_transform().Find("TopCommon/Panel_Menu/List").get_gameObject().GetComponent<CUIListScript>();
            Tab tab = !CSysDynamicBlock.bSocialBlocked ? Tab.Friend_SNS : Tab.Friend;
            if (!CSysDynamicBlock.bSocialBlocked)
            {
                this.tabMgr.Add(Tab.Friend_SNS, text);
                this.tabMgr.Add(Tab.Friend, UT.GetText("Friend_Title_List"));
                this.tabMgr.Add(Tab.Friend_Request, UT.GetText("Friend_Title_Requests"));
                this.tabMgr.Add(Tab.Friend_BlackList, "黑名单");
                this.tabMgr.Add(Tab.Friend_LBS, "附近的人");
                this.tabMgr.Add(Tab.Mentor, UT.GetText("Mentor_Title"));
                string b = Singleton<CTextManager>.instance.GetText("Friend_TabRecruit");
                if (!string.Equals("0", b))
                {
                    this.tabMgr.Add(Tab.Friend_Recruit, "好友招募");
                }
            }
            else
            {
                this.tabMgr.Add(Tab.Friend, UT.GetText("Friend_Title_List"));
                this.tabMgr.Add(Tab.Mentor, UT.GetText("Mentor_Title"));
                this.tabMgr.Add(Tab.Friend_Request, UT.GetText("Friend_Title_Requests"));
            }
            UT.SetTabList(this.tabMgr.GetTabTextList(), 0, this.tablistScript);
            this.btnText = Utility.GetComponetInChild<Text>(p, "node/Buttons/Invite/Text");
            this.sns_invite_btn = p.get_transform().Find("node/Buttons/Invite").get_gameObject();
            string str3 = Singleton<CTextManager>.instance.GetText("FriendAdd_Invite_Btn_QQ");
            if (Singleton<ApolloHelper>.GetInstance().CurPlatform == ApolloPlatform.Wechat)
            {
                str3 = Singleton<CTextManager>.instance.GetText("FriendAdd_Invite_Btn_Weixin");
            }
            this.btnText.set_text(str3);
            this.sns_invite_btn.CustomSetActive(false);
            this.rule_btn_node = p.get_transform().Find("btnRule").get_gameObject();
            this.lbs_node = p.get_transform().Find("node/LBSNode").get_gameObject();
            this.m_QQboxBtn = Utility.FindChild(p, "node/Buttons/QQBoxBtn");
            this.m_IntimacyRelationBtn = Utility.FindChild(p, "node/Relation");
            this.sns_share_switch = Utility.FindChild(p, "node/SnsNtfNode/SnsToggle");
            this.sns_add_switch = Utility.FindChild(p, "node/SnsNtfNode/AddToggle");
            this.sns_share_switch.CustomSetActive(false);
            this.localtionToggle = Utility.FindChild(p, "node/LBSNode/location").GetComponent<Toggle>();
            this.nanToggle = Utility.FindChild(p, "node/LBSNode/nan").GetComponent<Toggle>();
            this.nvToggle = Utility.FindChild(p, "node/LBSNode/nv").GetComponent<Toggle>();
            this.lbsRefreshBtn = Utility.FindChild(p, "node/LBSNode/Add").GetComponent<Button>();
            this.friendRecruitView.Init(Utility.FindChild(p, "node/zhaomu_node").get_gameObject());
            if (Singleton<ApolloHelper>.GetInstance().CurPlatform == ApolloPlatform.QQ)
            {
                Utility.GetComponetInChild<Text>(this.sns_share_switch, "Label").set_text(Singleton<CTextManager>.instance.GetText("Friend_SNS_NTF_Switch_Tips_1"));
            }
            else
            {
                Utility.GetComponetInChild<Text>(this.sns_share_switch, "Label").set_text(Singleton<CTextManager>.instance.GetText("Friend_SNS_NTF_Switch_Tips_2"));
            }
            this.sns_add_switch.CustomSetActive(false);
            this.tablistScript.m_alwaysDispatchSelectedChangeEvent = true;
            this.tablistScript.SelectElement(0, true);
            this.tablistScript.m_alwaysDispatchSelectedChangeEvent = false;
            this._tab = Tab.None;
            this.Refresh_Tab();
            this.CurTab = tab;
            CUIListElementScript elemenet = this.tablistScript.GetElemenet(this.tabMgr.GetIndex(Tab.Friend_LBS));
            if (elemenet != null)
            {
                Singleton<CUINewFlagSystem>.GetInstance().AddNewFlag(elemenet.get_gameObject(), enNewFlagKey.New_PeopleNearby_V1, enNewFlagPos.enTopRight, 1f, 0f, 0f, enNewFlagType.enNewFlag);
            }
            elemenet = this.tablistScript.GetElemenet(this.tabMgr.GetIndex(Tab.Mentor));
            if (elemenet != null)
            {
                Singleton<CUINewFlagSystem>.GetInstance().AddNewFlag(elemenet.get_gameObject(), enNewFlagKey.New_Friend_Mentor_V1, enNewFlagPos.enTopRight, 1f, 0f, 0f, enNewFlagType.enNewFlag);
            }
            elemenet = this.tablistScript.GetElemenet(this.tabMgr.GetIndex(Tab.Friend_Recruit));
            if (elemenet != null)
            {
                Singleton<CUINewFlagSystem>.GetInstance().AddNewFlag(elemenet.get_gameObject(), enNewFlagKey.New_Friend_Recruit_V1, enNewFlagPos.enTopRight, 1f, 0f, 0f, enNewFlagType.enNewFlag);
            }
            elemenet = this.tablistScript.GetElemenet(this.tabMgr.GetIndex(Tab.Friend));
            if (elemenet != null)
            {
                Singleton<CUINewFlagSystem>.GetInstance().AddNewFlag(elemenet.get_gameObject(), enNewFlagKey.New_Friend_Friend_V1, enNewFlagPos.enTopRight, 1f, 0f, 0f, enNewFlagType.enNewFlag);
            }
        }

        public void OpenMentorRequestForm()
        {
            CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(CFriendContoller.MentorRequestListFormPath, false, true);
        }

        public void OpenRelationForm()
        {
            this.intimacyRelationView.Open();
        }

        public void OpenSearchForm(int searchType)
        {
            this.addFriendView.Init(searchType);
        }

        public void Refresh()
        {
            this.Refresh_Tab();
            this.Refresh_SnsSwitch();
            this.Refresh_List(this.CurTab);
            if ((this.addFriendView != null) && this.addFriendView.bShow)
            {
                this.addFriendView.Refresh(-1);
            }
        }

        public void Refresh_List(Tab type)
        {
            if (this.friendListCom != null)
            {
                GameObject widget = this.friendform.GetWidget(6);
                GameObject obj3 = this.friendform.GetWidget(10);
                obj3.CustomSetActive(false);
                GameObject obj4 = this.friendform.GetWidget(1);
                GameObject obj5 = this.friendform.GetWidget(3);
                GameObject obj6 = this.friendform.GetWidget(5);
                GameObject obj7 = this.friendform.GetWidget(14);
                GameObject obj8 = this.friendform.GetWidget(13);
                obj4.CustomSetActive(type != Tab.Mentor);
                obj5.CustomSetActive(type == Tab.Mentor);
                obj6.CustomSetActive(false);
                GameObject obj9 = this.friendform.GetWidget(11);
                this.ifnoText = Utility.GetComponetInChild<Text>(widget, "info_node/Text");
                this.ifnoText.set_text(Singleton<CTextManager>.instance.GetText("Friend_NoFriend_Tip"));
                GameObject obj10 = this.friendform.GetWidget(12);
                if (obj9 != null)
                {
                    RectTransform transform = obj9.get_transform() as RectTransform;
                    transform.set_sizeDelta(this.friendListSizeDeltaOrg);
                    transform.set_anchoredPosition(this.friendListPosOrg);
                    CUIListScript component = obj9.GetComponent<CUIListScript>();
                    if (component != null)
                    {
                        component.EnableExtraContent(false);
                    }
                }
                widget.CustomSetActive(true);
                switch (type)
                {
                    case Tab.Friend_SNS:
                    {
                        ListView<COMDT_FRIEND_INFO> view = Singleton<CFriendContoller>.instance.model.GetList(CFriendModel.FriendType.SNS);
                        this._refresh_list(this.friendListCom, view, FriendShower.ItemType.Normal, true, CFriendModel.FriendType.SNS);
                        this.friendListCom.get_gameObject().CustomSetActive(true);
                        if ((this.info_node != null) && (view != null))
                        {
                            this.info_node.CustomSetActive(view.Count == 0);
                        }
                        break;
                    }
                    case Tab.Friend:
                    {
                        ListView<COMDT_FRIEND_INFO> view3 = Singleton<CFriendContoller>.instance.model.GetList(CFriendModel.FriendType.GameFriend);
                        this._refresh_list(this.friendListCom, view3, FriendShower.ItemType.Normal, false, CFriendModel.FriendType.GameFriend);
                        this.friendListCom.get_gameObject().CustomSetActive(true);
                        if ((this.info_node != null) && (view3 != null))
                        {
                            this.info_node.CustomSetActive(view3.Count == 0);
                        }
                        break;
                    }
                    case Tab.Mentor:
                    {
                        this.ifnoText.set_text(Singleton<CTextManager>.instance.GetText("Mentor_NoInfoTips"));
                        enMentorState mentorState = CFriendContoller.GetMentorState(0, null);
                        if (mentorState != enMentorState.None)
                        {
                            bool flag = (mentorState == enMentorState.IWantMentor) || (mentorState == enMentorState.IWantApprentice);
                            bool flag2 = (mentorState == enMentorState.IHasMentor) || (mentorState == enMentorState.IHasApprentice);
                            bool flag3 = Singleton<CFriendContoller>.GetInstance().HasMentor(null);
                            obj6.CustomSetActive(flag && !flag3);
                            widget.CustomSetActive(flag2 || flag3);
                            if (flag && !flag3)
                            {
                                uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 270).dwConfValue;
                                uint num2 = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x10d).dwConfValue;
                                uint key = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x10f).dwConfValue;
                                string szGradeDesc = string.Empty;
                                ResRankGradeConf dataByKey = GameDataMgr.rankGradeDatabin.GetDataByKey(key);
                                if (dataByKey != null)
                                {
                                    szGradeDesc = dataByKey.szGradeDesc;
                                }
                                GameObject obj11 = this.friendform.GetWidget(8);
                                GameObject obj12 = this.friendform.GetWidget(9);
                                string[] args = new string[] { dwConfValue.ToString() };
                                obj11.GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("ApprenticeIntroRequire", args));
                                string[] textArray2 = new string[] { num2.ToString(), szGradeDesc };
                                obj12.GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("MentorIntroRequire", textArray2));
                                obj7.GetComponent<Text>().set_text(Singleton<CUIManager>.GetInstance().GetRuleTextContent(0x13));
                                obj8.GetComponent<Text>().set_text(Singleton<CUIManager>.GetInstance().GetRuleTextContent(0x12));
                            }
                            else
                            {
                                RectTransform transform2 = widget.get_transform().Find("MentorSubTab") as RectTransform;
                                int amount = Singleton<CFriendContoller>.GetInstance().RefreshMentorTabData();
                                CUIListScript script2 = transform2.get_gameObject().GetComponent<CUIListScript>();
                                script2.get_gameObject().CustomSetActive(true);
                                script2.SetElementAmount(amount);
                                int index = 0;
                                for (int i = 0; i < CFriendContoller.s_mentorTabStr.Length; i++)
                                {
                                    if ((CFriendContoller.MentorTabMask & (((int) 1) << i)) != 0)
                                    {
                                        CUIListElementScript elemenet = script2.GetElemenet(index);
                                        Transform transform3 = elemenet.get_transform().Find("Text");
                                        if (transform3 != null)
                                        {
                                            transform3.GetComponent<Text>().set_text(CFriendContoller.s_mentorTabStr[i]);
                                        }
                                        elemenet.SetDataTag(i.ToString());
                                        index++;
                                    }
                                }
                                if (index != 0)
                                {
                                    script2.SelectElement(0, true);
                                }
                                RectTransform transform4 = obj3.get_transform() as RectTransform;
                                RectTransform transform5 = obj9.get_transform() as RectTransform;
                                transform5.set_sizeDelta(new Vector2(this.friendListSizeDeltaOrg.x, this.friendListSizeDeltaOrg.y - transform4.get_rect().get_height()));
                                transform5.set_anchoredPosition(new Vector2(this.friendListPosOrg.x, (this.friendListPosOrg.y - (transform4.get_rect().get_height() / 2f)) + 20f));
                                obj9.GetComponent<CUIListScript>().m_scrollAreaSize = new Vector2(this.friendListRectOrg.get_width(), this.friendListRectOrg.get_height() - transform4.get_rect().get_height());
                            }
                            Text text2 = obj5.get_transform().Find("BtnIWant/Text").GetComponent<Text>();
                            switch (mentorState)
                            {
                                case enMentorState.IWantMentor:
                                case enMentorState.IHasMentor:
                                {
                                    string[] textArray3 = new string[] { Singleton<CTextManager>.GetInstance().GetText("Mentor_GetMentor") };
                                    text2.set_text(Singleton<CTextManager>.GetInstance().GetText("Mentor_IWant", textArray3));
                                    break;
                                }
                                case enMentorState.IWantApprentice:
                                case enMentorState.IHasApprentice:
                                {
                                    string[] textArray4 = new string[] { Singleton<CTextManager>.GetInstance().GetText("Mentor_GetApprentice") };
                                    text2.set_text(Singleton<CTextManager>.GetInstance().GetText("Mentor_IWant", textArray4));
                                    this.RefuseAllInvalidMentorRequest();
                                    break;
                                }
                            }
                            break;
                        }
                        obj6.CustomSetActive(false);
                        widget.CustomSetActive(true);
                        if (this.info_node != null)
                        {
                            this.info_node.CustomSetActive(true);
                        }
                        return;
                    }
                    case Tab.Friend_Request:
                    {
                        ListView<COMDT_FRIEND_INFO> view2 = Singleton<CFriendContoller>.instance.model.GetList(CFriendModel.FriendType.RequestFriend);
                        this._refresh_list(this.friendListCom, view2, FriendShower.ItemType.Request, false, CFriendModel.FriendType.RequestFriend);
                        this.friendListCom.get_gameObject().CustomSetActive(true);
                        if ((this.info_node != null) && (view2 != null))
                        {
                            this.info_node.CustomSetActive(view2.Count == 0);
                        }
                        break;
                    }
                    case Tab.Friend_BlackList:
                    {
                        List<CFriendModel.stBlackName> blackList = Singleton<CFriendContoller>.instance.model.GetBlackList();
                        this._refresh_black_list(this.friendListCom, blackList);
                        this.friendListCom.get_gameObject().CustomSetActive(true);
                        if ((this.info_node != null) && (blackList != null))
                        {
                            this.info_node.CustomSetActive(blackList.Count == 0);
                        }
                        break;
                    }
                    case Tab.Friend_LBS:
                    {
                        ListView<CSDT_LBS_USER_INFO> currentLBSList = Singleton<CFriendContoller>.instance.model.GetCurrentLBSList();
                        this._refresh_LBS_list(this.friendListCom, currentLBSList);
                        this.friendListCom.get_gameObject().CustomSetActive(Singleton<CFriendContoller>.instance.model.EnableShareLocation);
                        if (this.info_node != null)
                        {
                            if (currentLBSList != null)
                            {
                                this.info_node.CustomSetActive(currentLBSList.Count == 0);
                                break;
                            }
                            this.info_node.CustomSetActive(true);
                        }
                        break;
                    }
                    case Tab.Friend_Recruit:
                        widget.CustomSetActive(false);
                        break;
                }
            }
        }

        public void Refresh_SnsSwitch()
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CFriendContoller.MentorRequestListFormPath);
                if (form != null)
                {
                    GameObject widget = form.GetWidget(1);
                    bool flag = CFriendModel.IsOnSnsSwitch(masterRoleInfo.snsSwitchBits, COM_REFUSE_TYPE.COM_REFUSE_TYPE_MASTERREQ);
                    widget.GetComponent<Toggle>().set_isOn(flag);
                    this.RefreshMentorReqList();
                }
                if ((this.sns_share_switch != null) && (this.sns_add_switch != null))
                {
                    this.sns_share_switch.CustomSetActive(false);
                    this.sns_add_switch.CustomSetActive(false);
                    if (!CSysDynamicBlock.bSocialBlocked)
                    {
                        switch (this.CurTab)
                        {
                            case Tab.Friend_SNS:
                            {
                                this.sns_share_switch.CustomSetActive(true);
                                bool flag2 = CFriendModel.IsOnSnsSwitch(masterRoleInfo.snsSwitchBits, COM_REFUSE_TYPE.COM_REFUSE_TYPE_DONOTE_AND_REC);
                                this.sns_share_switch.GetComponent<Toggle>().set_isOn(flag2);
                                break;
                            }
                            case Tab.Friend_Request:
                            {
                                this.sns_add_switch.CustomSetActive(true);
                                bool flag3 = CFriendModel.IsOnSnsSwitch(masterRoleInfo.snsSwitchBits, COM_REFUSE_TYPE.COM_REFUSE_TYPE_ADDFRIEND);
                                this.sns_add_switch.GetComponent<Toggle>().set_isOn(flag3);
                                break;
                            }
                        }
                    }
                }
            }
        }

        public void Refresh_Tab()
        {
            if (this.friendform != null)
            {
                CFriendModel model = Singleton<CFriendContoller>.GetInstance().model;
                int dataCount = model.GetDataCount(CFriendModel.FriendType.RequestFriend);
                int alertNum = model.GetDataCount(CFriendModel.FriendType.MentorRequestList);
                bool flag = model.FRData.HasRedDot();
                GameObject target = Utility.FindChild(this.friendform.get_gameObject(), "node/Relation");
                if (target != null)
                {
                    int num3 = this.tabMgr.GetIndex(Tab.Friend);
                    if (flag)
                    {
                        CUICommonSystem.AddRedDot(target, enRedDotPos.enTopRight, 0);
                        CUICommonSystem.AddRedDot(this.tablistScript.GetElemenet(num3).get_gameObject(), enRedDotPos.enTopRight, 0);
                    }
                    else
                    {
                        CUICommonSystem.DelRedDot(target);
                        CUICommonSystem.DelRedDot(this.tablistScript.GetElemenet(num3).get_gameObject());
                    }
                }
                bool flag2 = Singleton<CTaskSys>.instance.IsMentorTaskRedDot();
                int index = this.tabMgr.GetIndex(Tab.Mentor);
                GameObject obj3 = Utility.FindChild(this.friendform.get_gameObject(), "node/mentorButtons/BtnMentorQuest");
                if (obj3 != null)
                {
                    if (flag2)
                    {
                        CUICommonSystem.AddRedDot(obj3, enRedDotPos.enTopRight, 0);
                    }
                    else
                    {
                        CUICommonSystem.DelRedDot(obj3);
                    }
                }
                int num5 = this.tabMgr.GetIndex(Tab.Friend_Request);
                if (num5 != -1)
                {
                    if (dataCount > 0)
                    {
                        CUICommonSystem.AddRedDot(this.tablistScript.GetElemenet(num5).get_gameObject(), enRedDotPos.enTopRight, dataCount);
                    }
                    else
                    {
                        CUICommonSystem.DelRedDot(this.tablistScript.GetElemenet(num5).get_gameObject());
                    }
                    if (this.friendform != null)
                    {
                        Transform transform = this.friendform.GetWidget(3).get_transform().Find("BtnRequestList");
                        int num6 = this.tabMgr.GetIndex(Tab.Mentor);
                        if (alertNum > 0)
                        {
                            CUICommonSystem.AddRedDot(transform.get_gameObject(), enRedDotPos.enTopRight, alertNum);
                            if (num6 != -1)
                            {
                                CUICommonSystem.AddRedDot(this.tablistScript.GetElemenet(num6).get_gameObject(), enRedDotPos.enTopRight, alertNum);
                            }
                        }
                        else
                        {
                            CUICommonSystem.DelRedDot(transform.get_gameObject());
                            if (num6 != -1)
                            {
                                CUICommonSystem.DelRedDot(this.tablistScript.GetElemenet(num6).get_gameObject());
                            }
                        }
                        if (flag2)
                        {
                            CUICommonSystem.AddRedDot(this.tablistScript.GetElemenet(index).get_gameObject(), enRedDotPos.enTopRight, 0);
                        }
                    }
                }
            }
        }

        public void RefreshMentorReqList()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CFriendContoller.MentorRequestListFormPath);
            if (form != null)
            {
                GameObject widget = form.GetWidget(0);
                if (widget != null)
                {
                    CUIListScript component = widget.GetComponent<CUIListScript>();
                    ListView<COMDT_FRIEND_INFO> list = Singleton<CFriendContoller>.instance.model.GetList(CFriendModel.FriendType.MentorRequestList);
                    if (list != null)
                    {
                        this._refresh_list(component, list, FriendShower.ItemType.MentorRequest, false, CFriendModel.FriendType.MentorRequestList);
                    }
                    component.get_gameObject().CustomSetActive(true);
                }
            }
        }

        private void RefuseAllInvalidMentorRequest()
        {
            ListView<COMDT_FRIEND_INFO> list = Singleton<CFriendContoller>.GetInstance().model.GetList(CFriendModel.FriendType.MentorRequestList);
            if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    stFriendVerifyContent content = Singleton<CFriendContoller>.GetInstance().model.GetFriendVerifyData(list[i].stUin.ullUid, list[i].stUin.dwLogicWorldId, CFriendModel.enVerifyDataSet.Mentor);
                    if ((content != null) && (content.mentorType == 3))
                    {
                        CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x151c);
                        msg.stPkgData.stConfirmMasterReq.bConfirmType = 2;
                        msg.stPkgData.stConfirmMasterReq.stUin.ullUid = list[i].stUin.ullUid;
                        msg.stPkgData.stConfirmMasterReq.stUin.dwLogicWorldId = list[i].stUin.dwLogicWorldId;
                        msg.stPkgData.stConfirmMasterReq.bReqType = (byte) content.mentorType;
                        Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
                    }
                }
            }
        }

        public void Show_Search_Result(COMDT_FRIEND_INFO info)
        {
            if ((this.addFriendView != null) && this.addFriendView.bShow)
            {
                this.addFriendView.Record_SearchFriend(info);
                this.addFriendView.Show_Search_Result(info);
            }
        }

        public void SyncGenderToggleState()
        {
            if (this.nanToggle != null)
            {
                this.nanToggle.set_isOn((Singleton<CFriendContoller>.instance.model.fileter & 1) != 0);
            }
            if (this.nvToggle != null)
            {
                this.nvToggle.set_isOn((Singleton<CFriendContoller>.instance.model.fileter & 2) != 0);
            }
        }

        public void SyncLBSShareBtnState()
        {
            bool enableShareLocation = Singleton<CFriendContoller>.instance.model.EnableShareLocation;
            if (this.lbsRefreshBtn != null)
            {
                CUICommonSystem.SetButtonEnable(this.lbsRefreshBtn, enableShareLocation, enableShareLocation, true);
            }
            if (this.localtionToggle != null)
            {
                this.localtionToggle.set_isOn(enableShareLocation);
            }
        }

        public void Update()
        {
            if (this.verficationView != null)
            {
                this.verficationView.Update();
            }
            if ((this.lbsRefreshBtn != null) && (Singleton<CFriendContoller>.instance.startCooldownTimestamp > 0L))
            {
                uint num = (uint) (Singleton<FrameSynchr>.GetInstance().LogicFrameTick - Singleton<CFriendContoller>.instance.startCooldownTimestamp);
                Transform transform = this.lbsRefreshBtn.get_transform().Find("Text");
                if (transform != null)
                {
                    Text component = transform.GetComponent<Text>();
                    if (component != null)
                    {
                        int num2 = 0x2710 - ((int) num);
                        if (num2 > 0)
                        {
                            CUICommonSystem.SetButtonEnableWithShader(this.lbsRefreshBtn, false, true);
                            this.lbsRefreshBtn.set_enabled(false);
                            int num3 = num2 / 0x3e8;
                            component.set_text(string.Format(Singleton<CTextManager>.instance.GetText("LBS_Refresh_CDInfo"), num3.ToString()));
                        }
                        else
                        {
                            Singleton<CFriendContoller>.instance.startCooldownTimestamp = 0L;
                            component.set_text(Singleton<CTextManager>.instance.GetText("LBS_Refresh_CDInfoNormal"));
                            CUICommonSystem.SetButtonEnableWithShader(this.lbsRefreshBtn, Singleton<CFriendContoller>.instance.model.EnableShareLocation, true);
                        }
                    }
                }
            }
        }

        public Tab CurTab
        {
            get
            {
                return this._tab;
            }
            set
            {
                if (this._tab != value)
                {
                    string searchLBSZero;
                    this._tab = value;
                    this.bgLineNode.CustomSetActive(true);
                    this.imgNode.CustomSetActive(true);
                    this.sns_invite_btn.CustomSetActive(false);
                    this.lbs_node.CustomSetActive(false);
                    this.addFriendBtnGameObject.CustomSetActive(true);
                    this.m_IntimacyRelationBtn.CustomSetActive(false);
                    this.friendRecruitView.Hide();
                    this.Refresh_List(this.CurTab);
                    this.Refresh_SnsSwitch();
                    if ((this._tab == Tab.Friend) || (this._tab == Tab.Friend_Request))
                    {
                        this.addFriendBtnGameObject.CustomSetActive(!CSysDynamicBlock.bFriendBlocked);
                    }
                    if (this.m_QQboxBtn != null)
                    {
                        bool bActive = false;
                        long currentUTCTime = CRoleInfo.GetCurrentUTCTime();
                        if (MonoSingleton<BannerImageSys>.GetInstance().QQBOXInfo.isTimeValid(currentUTCTime))
                        {
                            bActive = true;
                        }
                        if ((ApolloConfig.platform == ApolloPlatform.QQ) || (ApolloConfig.platform == ApolloPlatform.WTLogin))
                        {
                            if (this._tab == Tab.Friend_SNS)
                            {
                                this.m_QQboxBtn.CustomSetActive(bActive);
                            }
                            else
                            {
                                this.m_QQboxBtn.CustomSetActive(false);
                            }
                        }
                        else
                        {
                            this.m_QQboxBtn.CustomSetActive(false);
                        }
                        if (CSysDynamicBlock.bLobbyEntryBlocked)
                        {
                            this.m_QQboxBtn.CustomSetActive(false);
                        }
                    }
                    CFriendModel model = Singleton<CFriendContoller>.GetInstance().model;
                    switch (this._tab)
                    {
                        case Tab.Friend_SNS:
                        {
                            if (this.ifnoText != null)
                            {
                                this.ifnoText.set_text(Singleton<CTextManager>.instance.GetText("Friend_NoFriend_Tip"));
                            }
                            int dataCount = model.GetDataCount(CFriendModel.FriendType.SNS);
                            if (this.info_node != null)
                            {
                                this.info_node.CustomSetActive(dataCount == 0);
                            }
                            if (this.sns_invite_btn != null)
                            {
                                this.sns_invite_btn.CustomSetActive(!CSysDynamicBlock.bSocialBlocked);
                            }
                            if (this.rule_btn_node != null)
                            {
                                this.rule_btn_node.CustomSetActive(false);
                            }
                            return;
                        }
                        case Tab.Friend:
                        {
                            if (this.ifnoText != null)
                            {
                                this.ifnoText.set_text(Singleton<CTextManager>.instance.GetText("Friend_NoFriend_Tip"));
                            }
                            int num2 = model.GetDataCount(CFriendModel.FriendType.GameFriend);
                            if (this.info_node != null)
                            {
                                this.info_node.CustomSetActive(num2 == 0);
                            }
                            if (this.rule_btn_node != null)
                            {
                                this.rule_btn_node.CustomSetActive(true);
                            }
                            this.m_IntimacyRelationBtn.CustomSetActive(true);
                            if (CSysDynamicBlock.bLobbyEntryBlocked)
                            {
                                CUICommonSystem.SetObjActive(this.rule_btn_node, false);
                                CUICommonSystem.SetObjActive(this.m_IntimacyRelationBtn, false);
                            }
                            return;
                        }
                        case Tab.Mentor:
                            if (this.rule_btn_node != null)
                            {
                                this.rule_btn_node.CustomSetActive(true);
                            }
                            if (CSysDynamicBlock.bLobbyEntryBlocked)
                            {
                                CUICommonSystem.SetObjActive(this.rule_btn_node, false);
                            }
                            return;

                        case Tab.Friend_Request:
                        {
                            if (this.ifnoText != null)
                            {
                                this.ifnoText.set_text(Singleton<CTextManager>.instance.GetText("Friend_NoRequest_Tip"));
                            }
                            int num4 = model.GetDataCount(CFriendModel.FriendType.RequestFriend);
                            if (this.info_node != null)
                            {
                                this.info_node.CustomSetActive(num4 == 0);
                            }
                            if (this.rule_btn_node != null)
                            {
                                this.rule_btn_node.CustomSetActive(false);
                            }
                            return;
                        }
                        case Tab.Friend_BlackList:
                        {
                            if (this.ifnoText != null)
                            {
                                this.ifnoText.set_text(Singleton<CTextManager>.instance.GetText("Friend_NoBlackList_Tip"));
                            }
                            int count = model.GetBlackList().Count;
                            if (this.info_node != null)
                            {
                                this.info_node.CustomSetActive(count == 0);
                            }
                            if (this.rule_btn_node != null)
                            {
                                this.rule_btn_node.CustomSetActive(false);
                            }
                            return;
                        }
                        case Tab.Friend_LBS:
                            searchLBSZero = string.Empty;
                            if (string.IsNullOrEmpty(model.searchLBSZero))
                            {
                                searchLBSZero = Singleton<CTextManager>.instance.GetText("Friend_NoLBSList_Tip");
                                break;
                            }
                            searchLBSZero = model.searchLBSZero;
                            break;

                        case Tab.Friend_Recruit:
                            this.bgLineNode.CustomSetActive(false);
                            this.imgNode.CustomSetActive(false);
                            this.rule_btn_node.CustomSetActive(false);
                            this.sns_invite_btn.CustomSetActive(false);
                            this.lbs_node.CustomSetActive(false);
                            this.addFriendBtnGameObject.CustomSetActive(false);
                            this.m_QQboxBtn.CustomSetActive(false);
                            this.info_node.CustomSetActive(false);
                            this.friendRecruitView.Show();
                            return;

                        default:
                            this.info_node.CustomSetActive(false);
                            this.rule_btn_node.CustomSetActive(false);
                            return;
                    }
                    if (this.ifnoText != null)
                    {
                        this.ifnoText.set_text(searchLBSZero);
                    }
                    this.rule_btn_node.CustomSetActive(false);
                    this.sns_invite_btn.CustomSetActive(false);
                    this.lbs_node.CustomSetActive(true);
                    this.addFriendBtnGameObject.CustomSetActive(false);
                    this.m_QQboxBtn.CustomSetActive(false);
                    this.SyncGenderToggleState();
                    this.SyncLBSShareBtnState();
                    int num6 = 0;
                    ListView<CSDT_LBS_USER_INFO> currentLBSList = Singleton<CFriendContoller>.instance.model.GetCurrentLBSList();
                    if (currentLBSList != null)
                    {
                        num6 = currentLBSList.Count;
                    }
                    this.info_node.CustomSetActive(num6 == 0);
                }
            }
        }

        public class AddFriendView
        {
            public bool bShow;
            public GameObject buttons_node;
            private GameObject info_text;
            private Text input;
            public int m_addFriendTypeSrv = 1;
            public CUIListScript recommandFriendListCom;
            private static readonly Vector2 recommandFriendListPos1 = new Vector2(40f, 190f);
            private static readonly Vector2 recommandFriendListPos2 = new Vector2(40f, 340f);
            private static readonly Vector2 recommandFriendListSize1 = new Vector2(-80f, 180f);
            private static readonly Vector2 recommandFriendListSize2 = new Vector2(-80f, 320f);
            public COMDT_FRIEND_INFO search_info_Game;
            private FriendShower searchFriendShower;

            public void Clear()
            {
                Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Friend_SerchFriend, new CUIEventManager.OnUIEventHandler(this.On_SearchFriend));
                Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Friend_Close_AddForm, new CUIEventManager.OnUIEventHandler(this.On_Friend_Close_AddForm));
                this.input = null;
                this.searchFriendShower = null;
                this.recommandFriendListCom = null;
                this.search_info_Game = null;
                this.buttons_node = null;
                this.bShow = false;
                Singleton<CFriendContoller>.GetInstance().search_info = null;
            }

            public void Clear_SearchFriend()
            {
                this.search_info_Game = null;
            }

            private FriendShower.ItemType GetItemTypeBySearchType(int searchType)
            {
                switch (searchType)
                {
                    case 1:
                        return FriendShower.ItemType.Add;

                    case 2:
                        return FriendShower.ItemType.AddMentor;

                    case 3:
                        return FriendShower.ItemType.AddApprentice;
                }
                return FriendShower.ItemType.Add;
            }

            public void Init(int searchType)
            {
                Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_SerchFriend, new CUIEventManager.OnUIEventHandler(this.On_SearchFriend));
                Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_Close_AddForm, new CUIEventManager.OnUIEventHandler(this.On_Friend_Close_AddForm));
                CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(CFriendContoller.AddFriendFormPath, false, true);
                this.input = script.get_transform().FindChild("GameObject/SearchFriend/InputField/Text").GetComponent<Text>();
                this.searchFriendShower = script.get_transform().FindChild("GameObject/SearchFriend/Result/Friend").GetComponent<FriendShower>();
                this.searchFriendShower.get_gameObject().CustomSetActive(false);
                this.recommandFriendListCom = Utility.GetComponetInChild<CUIListScript>(script.get_gameObject(), "GameObject/RecommandList");
                this.buttons_node = script.get_transform().FindChild("GameObject/Buttons").get_gameObject();
                this.info_text = script.get_transform().Find("GameObject/SearchFriend/Result/info").get_gameObject();
                if (this.info_text != null)
                {
                    this.info_text.CustomSetActive(false);
                }
                CFriendContoller.s_addViewtype = searchType;
                FriendSysNetCore.Send_Request_RecommandFriend_List(searchType);
                this.Refresh(-1);
                this.bShow = true;
                this.m_addFriendTypeSrv = searchType;
                GameObject widget = script.GetWidget(0);
                GameObject obj3 = script.GetWidget(1);
                GameObject obj4 = script.GetWidget(2);
                GameObject obj5 = script.GetWidget(3);
                string[] textArray1 = new string[4];
                textArray1[0] = string.Empty;
                textArray1[1] = Singleton<CTextManager>.GetInstance().GetText("Friend_AddTitle");
                string[] args = new string[] { Singleton<CTextManager>.GetInstance().GetText("Mentor_GetMentor") };
                textArray1[2] = Singleton<CTextManager>.GetInstance().GetText("Mentor_IWant", args);
                string[] textArray3 = new string[] { Singleton<CTextManager>.GetInstance().GetText("Mentor_GetApprentice") };
                textArray1[3] = Singleton<CTextManager>.GetInstance().GetText("Mentor_IWant", textArray3);
                string[] strArray = textArray1;
                string[] textArray4 = new string[4];
                textArray4[0] = string.Empty;
                string[] textArray5 = new string[] { Singleton<CTextManager>.GetInstance().GetText("chat_title_friend") };
                textArray4[1] = Singleton<CTextManager>.GetInstance().GetText("Mentor_InputReplacer", textArray5);
                string[] textArray6 = new string[] { Singleton<CTextManager>.GetInstance().GetText("Mentor_mentor") };
                textArray4[2] = Singleton<CTextManager>.GetInstance().GetText("Mentor_IWant", textArray6);
                string[] textArray7 = new string[] { Singleton<CTextManager>.GetInstance().GetText("Mentor_apprentice") };
                textArray4[3] = Singleton<CTextManager>.GetInstance().GetText("Mentor_IWant", textArray7);
                string[] strArray2 = textArray4;
                string[] textArray8 = new string[4];
                textArray8[0] = string.Empty;
                string[] textArray9 = new string[] { Singleton<CTextManager>.GetInstance().GetText("chat_title_friend") };
                textArray8[1] = Singleton<CTextManager>.GetInstance().GetText("Mentor_recommend", textArray9);
                string[] textArray10 = new string[] { Singleton<CTextManager>.GetInstance().GetText("Mentor_mentor") };
                textArray8[2] = Singleton<CTextManager>.GetInstance().GetText("Mentor_recommend", textArray10);
                string[] textArray11 = new string[] { Singleton<CTextManager>.GetInstance().GetText("Mentor_apprentice") };
                textArray8[3] = Singleton<CTextManager>.GetInstance().GetText("Mentor_recommend", textArray11);
                string[] strArray3 = textArray8;
                string[] textArray12 = new string[4];
                textArray12[0] = string.Empty;
                string[] textArray13 = new string[] { Singleton<CTextManager>.GetInstance().GetText("chat_title_friend") };
                textArray12[1] = Singleton<CTextManager>.GetInstance().GetText("Mentor_recommend", textArray13);
                string[] textArray14 = new string[] { Singleton<CTextManager>.GetInstance().GetText("Mentor_mentor") };
                textArray12[2] = Singleton<CTextManager>.GetInstance().GetText("Mentor_recommend", textArray14);
                string[] textArray15 = new string[] { Singleton<CTextManager>.GetInstance().GetText("Mentor_apprentice") };
                textArray12[3] = Singleton<CTextManager>.GetInstance().GetText("Mentor_recommend", textArray15);
                string[] strArray4 = textArray12;
                widget.GetComponent<Text>().set_text(strArray[searchType]);
                obj3.GetComponent<Text>().set_text(strArray2[searchType]);
                obj4.GetComponent<Text>().set_text(strArray3[searchType]);
                obj5.GetComponent<Text>().set_text(strArray4[searchType]);
            }

            public void On_Friend_Close_AddForm(CUIEvent uiEvent)
            {
                this.Clear();
            }

            public void On_SearchFriend(CUIEvent uiEvent)
            {
                this.Clear_SearchFriend();
                this.searchFriendShower.get_gameObject().CustomSetActive(false);
                if (string.IsNullOrEmpty(this.input.get_text()))
                {
                    Singleton<CUIManager>.GetInstance().OpenMessageBox(UT.GetText("Friend_Input_Tips"), false);
                }
                else
                {
                    FriendSysNetCore.Send_Serch_Player(this.input.get_text());
                }
                this.Refresh_Friend_Recommand_List_Pos();
            }

            public void Record_SearchFriend(COMDT_FRIEND_INFO info)
            {
                this.search_info_Game = info;
            }

            public void Refresh(int type = -1)
            {
                if (type == -1)
                {
                    type = (int) CFriendContoller.GetFriendType(CFriendContoller.s_addViewtype);
                }
                this.buttons_node.CustomSetActive(false);
                this.Show_Search_Game((CFriendModel.FriendType) type);
                this.Show_Search_Result(null);
            }

            public void Refresh_Friend_Recommand_List(CFriendModel.FriendType type)
            {
                if (this.recommandFriendListCom != null)
                {
                    Singleton<CFriendContoller>.GetInstance().model.FilterRecommendFriends();
                    int dataCount = Singleton<CFriendContoller>.GetInstance().model.GetDataCount(type);
                    this.recommandFriendListCom.SetElementAmount(dataCount);
                    COMDT_FRIEND_INFO info = null;
                    for (int i = 0; i < dataCount; i++)
                    {
                        info = Singleton<CFriendContoller>.GetInstance().model.GetInfoAtIndex(type, i);
                        if (info != null)
                        {
                            this.Refresh_Recomand_Friend(i, info, (int) type);
                        }
                    }
                }
            }

            public void Refresh_Friend_Recommand_List_Pos()
            {
                if ((this.recommandFriendListCom != null) && (this.searchFriendShower != null))
                {
                    RectTransform transform = this.recommandFriendListCom.get_transform() as RectTransform;
                    if (this.searchFriendShower.get_gameObject().get_activeSelf())
                    {
                        transform.set_anchoredPosition(recommandFriendListPos1);
                        transform.set_sizeDelta(recommandFriendListSize1);
                    }
                    else
                    {
                        transform.set_anchoredPosition(recommandFriendListPos2);
                        transform.set_sizeDelta(recommandFriendListSize2);
                    }
                }
            }

            public void Refresh_Recomand_Friend(int index, COMDT_FRIEND_INFO info, int type)
            {
                CUIListElementScript elemenet = this.recommandFriendListCom.GetElemenet(index);
                if (elemenet != null)
                {
                    elemenet.SetDataTag(type.ToString());
                    FriendShower component = elemenet.GetComponent<FriendShower>();
                    if (component != null)
                    {
                        UT.ShowFriendData(info, component, this.GetItemTypeBySearchType(this.m_addFriendTypeSrv), false, (CFriendModel.FriendType) type);
                    }
                }
            }

            private void Show_Search_Game(CFriendModel.FriendType type)
            {
                this.Refresh_Friend_Recommand_List(type);
                this.Refresh_Friend_Recommand_List_Pos();
            }

            public void Show_Search_Result(COMDT_FRIEND_INFO info)
            {
                COMDT_FRIEND_INFO comdt_friend_info = null;
                comdt_friend_info = this.search_info_Game;
                this.recommandFriendListCom.get_gameObject().CustomSetActive(true);
                if (comdt_friend_info == null)
                {
                    if (this.searchFriendShower != null)
                    {
                        this.searchFriendShower.get_gameObject().CustomSetActive(false);
                    }
                }
                else if (this.searchFriendShower != null)
                {
                    this.searchFriendShower.get_gameObject().CustomSetActive(true);
                    UT.ShowFriendData(comdt_friend_info, this.searchFriendShower, this.GetItemTypeBySearchType(this.m_addFriendTypeSrv), false, CFriendModel.FriendType.RequestFriend);
                    if (this.recommandFriendListCom.GetElementAmount() == 0)
                    {
                        this.recommandFriendListCom.get_gameObject().CustomSetActive(false);
                    }
                }
            }
        }

        public enum Tab
        {
            Friend_SNS,
            Friend,
            Mentor,
            Friend_Request,
            Friend_BlackList,
            Friend_LBS,
            Friend_Recruit,
            None
        }

        public class TabMgr
        {
            private ListView<TabElement> tabElements = new ListView<TabElement>();
            private List<string> tabTextList = new List<string>();

            public void Add(CFriendView.Tab tab, string text)
            {
                this.tabElements.Add(new TabElement(tab, text));
            }

            public void Clear()
            {
                this.tabElements.Clear();
                this.tabTextList.Clear();
            }

            public int GetIndex(CFriendView.Tab tabValue)
            {
                for (int i = 0; i < this.tabElements.Count; i++)
                {
                    TabElement element = this.tabElements[i];
                    if ((element != null) && (element.tab == tabValue))
                    {
                        return i;
                    }
                }
                return -1;
            }

            public TabElement GetTabElement(int index)
            {
                if ((index >= 0) && (index < this.tabElements.Count))
                {
                    return this.tabElements[index];
                }
                return null;
            }

            public List<string> GetTabTextList()
            {
                this.tabTextList.Clear();
                for (int i = 0; i < this.tabElements.Count; i++)
                {
                    TabElement element = this.tabElements[i];
                    if ((element != null) && !this.tabTextList.Contains(element.content))
                    {
                        this.tabTextList.Add(element.content);
                    }
                }
                return this.tabTextList;
            }

            public class TabElement
            {
                public string content;
                public CFriendView.Tab tab;

                public TabElement(CFriendView.Tab tab, string content)
                {
                    this.tab = tab;
                    this.content = content;
                }
            }
        }

        public class Verfication
        {
            private InputField _inputName;
            private uint dwLogicWorldId;
            private COM_ADD_FRIEND_TYPE m_addFriendSourceType;
            private int m_addFriendUseHeroId;
            private bool m_bAddSearchFirend;
            private ulong ullUid;
            private static int Verfication_ChatMaxLength = 15;

            public Verfication()
            {
                Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_Verify_Close, new CUIEventManager.OnUIEventHandler(this.On_Friend_Verify_Close));
                Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_Verify_Send, new CUIEventManager.OnUIEventHandler(this.On_Friend_Verify_Send));
            }

            public static string GetRandomMentorReuqestStr(int type = -1)
            {
                int num = Random.Range(0, 3);
                if (type == -1)
                {
                    switch (CFriendContoller.GetMentorState(0, null))
                    {
                        case enMentorState.IWantApprentice:
                        case enMentorState.IHasApprentice:
                            type = 3;
                            goto Label_0042;
                    }
                    type = 2;
                }
            Label_0042:
                switch (type)
                {
                    case 2:
                        return Singleton<CTextManager>.GetInstance().GetText("Mentor_requestMentor_" + num);

                    case 3:
                        return Singleton<CTextManager>.GetInstance().GetText("Mentor_requestApprentice_" + num);
                }
                return string.Empty;
            }

            private void On_Friend_Verify_Close(CUIEvent uievent)
            {
                this.ullUid = 0L;
                this.dwLogicWorldId = 0;
                this.m_bAddSearchFirend = false;
                this._inputName = null;
                this.m_addFriendSourceType = COM_ADD_FRIEND_TYPE.COM_ADD_FRIEND_NULL;
                this.m_addFriendUseHeroId = -1;
            }

            private void On_Friend_Verify_Send(CUIEvent uievent)
            {
                if (uievent == null)
                {
                    Singleton<CUIManager>.instance.CloseForm(CFriendContoller.VerifyFriendFormPath);
                }
                else
                {
                    CUIFormScript srcFormScript = uievent.m_srcFormScript;
                    if (srcFormScript == null)
                    {
                        Singleton<CUIManager>.instance.CloseForm(CFriendContoller.VerifyFriendFormPath);
                    }
                    else
                    {
                        InputField component = srcFormScript.GetWidget(0).GetComponent<InputField>();
                        if (component == null)
                        {
                            Singleton<CUIManager>.instance.CloseForm(CFriendContoller.VerifyFriendFormPath);
                        }
                        else
                        {
                            string veriyText = CUIUtility.RemoveEmoji(component.get_text()).Trim();
                            if (this.ullUid == 0)
                            {
                                Singleton<CUIManager>.instance.CloseForm(CFriendContoller.VerifyFriendFormPath);
                            }
                            else
                            {
                                if (this.m_bAddSearchFirend)
                                {
                                    FriendSysNetCore.Send_Request_BeFriend(this.ullUid, this.dwLogicWorldId, veriyText, this.m_addFriendSourceType, this.m_addFriendUseHeroId);
                                }
                                else
                                {
                                    FriendSysNetCore.Send_Request_BeFriend(this.ullUid, this.dwLogicWorldId, veriyText, this.m_addFriendSourceType, this.m_addFriendUseHeroId);
                                    Singleton<CFriendContoller>.instance.model.Remove(CFriendModel.FriendType.Recommend, this.ullUid, this.dwLogicWorldId);
                                }
                                this.ullUid = 0L;
                                this.dwLogicWorldId = 0;
                                this.m_bAddSearchFirend = false;
                                this._inputName = null;
                                this.m_addFriendSourceType = COM_ADD_FRIEND_TYPE.COM_ADD_FRIEND_NULL;
                                this.m_addFriendUseHeroId = -1;
                                Singleton<CUIManager>.instance.CloseForm(CFriendContoller.VerifyFriendFormPath);
                            }
                        }
                    }
                }
            }

            public void Open(ulong ullUid, uint dwLogicWorldId, bool bAddSearchFirend, COM_ADD_FRIEND_TYPE addFriendType = 0, int useHeroId = -1, bool onlyAddFriend = true)
            {
                if ((CFriendContoller.s_addViewtype == 1) || onlyAddFriend)
                {
                    CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(CFriendContoller.VerifyFriendFormPath, true, true);
                    if (script != null)
                    {
                        GameObject obj2 = script.GetWidget(0).get_gameObject();
                        if (obj2 != null)
                        {
                            this._inputName = obj2.GetComponent<InputField>();
                            if (this._inputName != null)
                            {
                                if (this._inputName.get_placeholder() != null)
                                {
                                    this._inputName.get_placeholder().get_gameObject().CustomSetActive(false);
                                }
                                string randomVerifyContent = Singleton<CFriendContoller>.instance.model.GetRandomVerifyContent();
                                if (!string.IsNullOrEmpty(randomVerifyContent))
                                {
                                    this._inputName.set_text(randomVerifyContent);
                                }
                                this.ullUid = ullUid;
                                this.dwLogicWorldId = dwLogicWorldId;
                                this.m_bAddSearchFirend = bAddSearchFirend;
                                this.m_addFriendSourceType = addFriendType;
                                this.m_addFriendUseHeroId = useHeroId;
                            }
                        }
                    }
                }
                else
                {
                    switch (CFriendContoller.s_addViewtype)
                    {
                        case 2:
                        case 3:
                        {
                            string title = null;
                            string stringPlacer = null;
                            string mentorStateString = Singleton<CFriendContoller>.GetInstance().GetMentorStateString();
                            if (mentorStateString != null)
                            {
                                string[] args = new string[] { mentorStateString };
                                title = Singleton<CTextManager>.GetInstance().GetText("Mentor_VerifyReqTitle", args);
                                string[] textArray2 = new string[] { mentorStateString };
                                stringPlacer = Singleton<CTextManager>.GetInstance().GetText("Mentor_VerifyReqReplacer", textArray2);
                            }
                            if (Singleton<CFriendContoller>.instance.m_mentorSelectedUin == null)
                            {
                                Singleton<CFriendContoller>.instance.m_mentorSelectedUin = new COMDT_ACNT_UNIQ();
                            }
                            Singleton<CFriendContoller>.instance.m_mentorSelectedUin.dwLogicWorldId = dwLogicWorldId;
                            Singleton<CFriendContoller>.instance.m_mentorSelectedUin.ullUid = ullUid;
                            COMDT_FRIEND_INFO comdt_friend_info = Singleton<CFriendContoller>.GetInstance().model.getFriendByUid(ullUid, CFriendModel.FriendType.Apprentice, dwLogicWorldId);
                            if (comdt_friend_info == null)
                            {
                                comdt_friend_info = Singleton<CFriendContoller>.GetInstance().model.getFriendByUid(ullUid, CFriendModel.FriendType.Mentor, dwLogicWorldId);
                            }
                            if (comdt_friend_info == null)
                            {
                                comdt_friend_info = Singleton<CFriendContoller>.GetInstance().model.getFriendByUid(ullUid, CFriendModel.FriendType.Apprentice, dwLogicWorldId);
                            }
                            if (comdt_friend_info == null)
                            {
                                Singleton<CUIManager>.GetInstance().OpenStringSenderBox(title, Singleton<CTextManager>.GetInstance().GetText("Mentor_VerifyReqDesc"), stringPlacer, new StringSendboxOnSend(Singleton<CFriendContoller>.GetInstance().OnMentorApplyVerifyBoxRetrun), GetRandomMentorReuqestStr(CFriendContoller.s_addViewtype));
                            }
                            else
                            {
                                Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Mentor_Err_AlreadyMentor"), false, 1.5f, null, new object[0]);
                            }
                            break;
                        }
                    }
                }
            }

            public void Update()
            {
                if (this._inputName != null)
                {
                    string str = this._inputName.get_text();
                    if ((str != null) && (str.Length > Verfication_ChatMaxLength))
                    {
                        this._inputName.DeactivateInputField();
                        this._inputName.set_text(str.Substring(0, Verfication_ChatMaxLength));
                        Singleton<CUIManager>.GetInstance().OpenMessageBox(string.Format(Singleton<CTextManager>.instance.GetText("chat_input_max"), Verfication_ChatMaxLength), false);
                    }
                }
            }

            public enum eVerficationFormWidget
            {
                DescText = 1,
                NameInputField = 0,
                None = -1
            }
        }
    }
}

