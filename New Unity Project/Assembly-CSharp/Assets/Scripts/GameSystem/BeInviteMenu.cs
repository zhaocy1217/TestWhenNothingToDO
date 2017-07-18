namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class BeInviteMenu
    {
        private CUIFormScript form;
        private GameObject listNode;
        private CUIListScript listScript;
        private List<string> m_configTexts = new List<string>();
        private InputField m_inputFiled;
        private GameObject m_sendBtn;
        private CUITimerScript timerScript;

        public void Clear()
        {
            this.listNode = null;
            this.form = null;
            this.listScript = null;
            this.m_inputFiled = null;
            this.timerScript = null;
            this.m_sendBtn = null;
        }

        private void InitEvt()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Invite_RefuseReason_ClickDown, new CUIEventManager.OnUIEventHandler(this.On_Invite_RefuseReason_ClickDown));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Invite_RefuseReason_ClickList, new CUIEventManager.OnUIEventHandler(this.On_Invite_RefuseReason_ClickList));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Invite_RefuseReason_Send, new CUIEventManager.OnUIEventHandler(this.On_Invite_RefuseReason_Send));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Invite_Form_Closed, new CUIEventManager.OnUIEventHandler(this.On_Invite_Form_Closed));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Invite_RefuseReason_ItemEnable, new CUIEventManager.OnUIEventHandler(this.On_InBattleMsg_ListElement_Enable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Invite_RefuseReason_BgClick, new CUIEventManager.OnUIEventHandler(this.ShowRefuseSendState));
        }

        public void LoadConfig()
        {
            bool flag = true;
            int num = 0;
            while (flag)
            {
                string key = string.Format("Invite_Refuse_Reason_{0}", num);
                string text = Singleton<CTextManager>.instance.GetText(key);
                if (!string.Equals(key, text))
                {
                    num++;
                    if (!this.m_configTexts.Contains(text))
                    {
                        this.m_configTexts.Add(text);
                    }
                }
                else
                {
                    flag = false;
                }
            }
            if (this.m_configTexts.Count == 0)
            {
                this.m_configTexts.Add("need config");
            }
        }

        public void On_InBattleMsg_ListElement_Enable(CUIEvent uievent)
        {
            int srcWidgetIndexInBelongedList = uievent.m_srcWidgetIndexInBelongedList;
            if ((srcWidgetIndexInBelongedList >= 0) && (srcWidgetIndexInBelongedList < this.m_configTexts.Count))
            {
                string str = this.m_configTexts[srcWidgetIndexInBelongedList];
                if (str != null)
                {
                    Text component = uievent.m_srcWidget.get_transform().Find("Text").GetComponent<Text>();
                    if ((component != null) && (str != null))
                    {
                        component.set_text(str);
                    }
                }
            }
        }

        private void On_Invite_Form_Closed(CUIEvent uievent)
        {
            this.UnInitEvt();
        }

        private void On_Invite_RefuseReason_ClickDown(CUIEvent uievent)
        {
            if (this.form.get_transform().Find("Panel/Panel/refuse").get_gameObject() != null)
            {
                if ((this.listNode != null) && this.listNode.get_gameObject().get_activeInHierarchy())
                {
                    this.ShowRefuseSendState(null);
                }
                else
                {
                    this.ShowRefuseListState();
                }
            }
            else
            {
                this.ShowRefuseListState();
            }
        }

        private void On_Invite_RefuseReason_ClickList(CUIEvent uievent)
        {
            this.ShowRefuseSendState(null);
            string str = this.m_configTexts[uievent.m_srcWidgetIndexInBelongedList];
            this.m_inputFiled.set_text(str);
        }

        private void On_Invite_RefuseReason_Send(CUIEvent uievent)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x7e5);
            msg.stPkgData.stInviteJoinGameRsp.bIndex = (byte) uievent.m_eventParams.tag;
            msg.stPkgData.stInviteJoinGameRsp.bResult = 14;
            msg.stPkgData.stInviteJoinGameRsp.szDenyReason = UT.String2Bytes(CUIUtility.RemoveEmoji(this.m_inputFiled.get_text()));
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
            Singleton<WatchController>.GetInstance().Stop();
            Singleton<CMailSys>.instance.AddFriendInviteMail(uievent, CMailSys.enProcessInviteType.Refuse);
        }

        public void Open(SCPKG_INVITE_JOIN_GAME_REQ info)
        {
            this.InitEvt();
            this.ShowNormal(info);
            if (this.listScript != null)
            {
                int count = this.m_configTexts.Count;
                this.listScript.SetElementAmount(count);
            }
        }

        public void ShowNormal(SCPKG_INVITE_JOIN_GAME_REQ info)
        {
            string str = CUIUtility.RemoveEmoji(StringHelper.UTF8BytesToString(ref info.stInviterInfo.szName));
            stUIEventParams eventParams = new stUIEventParams();
            eventParams.tag = info.bIndex;
            int result = 15;
            int.TryParse(Singleton<CTextManager>.instance.GetText("MessageBox_Close_Time"), out result);
            this.form = Singleton<CUIManager>.GetInstance().OpenForm(string.Format("{0}{1}", "UGUI/Form/Common/", "Form_BeInvited.prefab"), false, false);
            GameObject obj2 = this.form.get_transform().Find("Panel/Panel/normal").get_gameObject();
            if (obj2 != null)
            {
                obj2.CustomSetActive(true);
            }
            GameObject obj3 = this.form.get_transform().Find("Panel/Panel/refuse").get_gameObject();
            if (obj3 != null)
            {
                obj3.CustomSetActive(false);
            }
            this.listNode = this.form.get_transform().Find("Panel/Panel/refuse/reasonPanel/DropList").get_gameObject();
            this.listScript = this.listNode.get_transform().Find("List").GetComponent<CUIListScript>();
            this.m_inputFiled = this.form.get_transform().Find("Panel/Panel/refuse/reasonPanel/InputField").GetComponent<InputField>();
            this.m_sendBtn = this.form.get_transform().Find("Panel/Panel/refuse/btnGroup/Button_Send").get_gameObject();
            this.form.get_transform().Find("Panel/Panel/refuse/MatchInfo").GetComponent<Text>().set_text(Singleton<CTextManager>.instance.GetText("Invite_Refuse_Title"));
            if (this.form != null)
            {
                string str2 = null;
                string str3 = null;
                if (info.bInviteType == 1)
                {
                    ResDT_LevelCommonInfo pvpMapCommonInfo = CLevelCfgLogicManager.GetPvpMapCommonInfo(info.stInviteDetail.stRoomDetail.bMapType, info.stInviteDetail.stRoomDetail.dwMapId);
                    eventParams.heroId = info.stInviteDetail.stRoomDetail.bMapType;
                    eventParams.weakGuideId = info.stInviteDetail.stRoomDetail.dwMapId;
                    if (pvpMapCommonInfo != null)
                    {
                        string[] textArray1 = new string[] { (pvpMapCommonInfo.bMaxAcntNum / 2).ToString(), (pvpMapCommonInfo.bMaxAcntNum / 2).ToString(), Utility.UTF8Convert(pvpMapCommonInfo.szName) };
                        str2 = Singleton<CTextManager>.instance.GetText("Invite_Map_Desc", textArray1);
                    }
                    str3 = Singleton<CTextManager>.GetInstance().GetText("Invite_Match_Type_4");
                }
                else if (info.bInviteType == 2)
                {
                    ResDT_LevelCommonInfo info3 = CLevelCfgLogicManager.GetPvpMapCommonInfo(info.stInviteDetail.stTeamDetail.bMapType, info.stInviteDetail.stTeamDetail.dwMapId);
                    eventParams.heroId = info.stInviteDetail.stTeamDetail.bMapType;
                    eventParams.weakGuideId = info.stInviteDetail.stTeamDetail.dwMapId;
                    if (info3 != null)
                    {
                        string[] textArray2 = new string[] { (info3.bMaxAcntNum / 2).ToString(), (info3.bMaxAcntNum / 2).ToString(), Utility.UTF8Convert(info3.szName) };
                        str2 = Singleton<CTextManager>.instance.GetText("Invite_Map_Desc", textArray2);
                    }
                    if (info.stInviteDetail.stTeamDetail.bMapType == 3)
                    {
                        str3 = Singleton<CTextManager>.GetInstance().GetText("Invite_Match_Type_1");
                    }
                    else
                    {
                        str3 = Singleton<CTextManager>.GetInstance().GetText((info.stInviteDetail.stTeamDetail.bPkAI != 1) ? "Invite_Match_Type_3" : "Invite_Match_Type_2");
                    }
                }
                string[] args = new string[] { str3, str2 };
                string text = Singleton<CTextManager>.instance.GetText("Be_Invited_Tips", args);
                this.form.m_formWidgets[8].GetComponent<Text>().set_text(text);
                uint dwRelationMask = info.stInviterInfo.dwRelationMask;
                string str5 = null;
                if ((dwRelationMask & 1) > 0)
                {
                    str5 = Singleton<CTextManager>.instance.GetText("Invite_Src_Type_1");
                    eventParams.tag2 = 0;
                }
                else if ((dwRelationMask & 2) > 0)
                {
                    str5 = Singleton<CTextManager>.instance.GetText("Invite_Src_Type_4");
                    eventParams.tag2 = 1;
                }
                else if ((dwRelationMask & 4) > 0)
                {
                    str5 = Singleton<CTextManager>.instance.GetText("Invite_Src_Type_5");
                    eventParams.tag2 = 2;
                    CSDT_LBS_USER_INFO csdt_lbs_user_info = Singleton<CFriendContoller>.instance.model.GetLBSUserInfo(info.stInviterInfo.ullUid, info.stInviterInfo.dwLogicWorldID, CFriendModel.LBSGenderType.Both);
                    if (csdt_lbs_user_info != null)
                    {
                        eventParams.tagUInt = csdt_lbs_user_info.dwGameSvrEntity;
                    }
                }
                else
                {
                    str5 = Singleton<CTextManager>.instance.GetText("Invite_Src_Type_1");
                    eventParams.tag2 = 3;
                }
                string str6 = string.Format(Singleton<CTextManager>.instance.GetText("Be_Invited_FromType"), str5);
                this.form.m_formWidgets[6].GetComponent<Text>().set_text(str6);
                eventParams.tagStr = string.Format("<color=#FFFFFF>{0}</color> {1} {2}", str, text, str6);
                eventParams.commonUInt64Param1 = info.stInviterInfo.ullUid;
                eventParams.taskId = info.stInviterInfo.dwLogicWorldID;
                eventParams.tag3 = info.bInviteType;
                if (result != 0)
                {
                    Transform transform = this.form.get_transform().Find("closeTimer");
                    if (transform != null)
                    {
                        this.timerScript = transform.GetComponent<CUITimerScript>();
                        if (this.timerScript != null)
                        {
                            this.timerScript.set_enabled(true);
                            this.timerScript.SetTotalTime((float) result);
                            this.timerScript.StartTimer();
                            this.timerScript.m_eventIDs[1] = enUIEventID.Invite_TimeOut;
                            this.timerScript.m_eventParams[1] = eventParams;
                        }
                    }
                }
                this.form.m_formWidgets[0].GetComponent<CUIEventScript>().SetUIEvent(enUIEventType.Click, enUIEventID.Invite_RefuseReason_Send, eventParams);
                this.form.m_formWidgets[1].GetComponent<CUIEventScript>().SetUIEvent(enUIEventType.Click, enUIEventID.Invite_AcceptInvite, eventParams);
                this.form.m_formWidgets[5].GetComponent<Text>().set_text(str);
                COM_SNSGENDER bGender = (COM_SNSGENDER) info.stInviterInfo.bGender;
                Image component = this.form.m_formWidgets[4].GetComponent<Image>();
                component.get_gameObject().CustomSetActive(bGender != COM_SNSGENDER.COM_SNSGENDER_NONE);
                switch (bGender)
                {
                    case COM_SNSGENDER.COM_SNSGENDER_MALE:
                        CUIUtility.SetImageSprite(component, string.Format("{0}icon/Ico_boy.prefab", "UGUI/Sprite/Dynamic/"), null, true, false, false, false);
                        break;

                    case COM_SNSGENDER.COM_SNSGENDER_FEMALE:
                        CUIUtility.SetImageSprite(component, string.Format("{0}icon/Ico_girl.prefab", "UGUI/Sprite/Dynamic/"), null, true, false, false, false);
                        break;
                }
                this.form.m_formWidgets[3].GetComponent<CUIHttpImageScript>().SetImageUrl(Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(StringHelper.UTF8BytesToString(ref info.stInviterInfo.szHeadUrl)));
                MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(this.form.m_formWidgets[2].GetComponent<Image>(), (int) info.stInviterInfo.dwHeadImgId);
                MonoSingleton<NobeSys>.GetInstance().SetHeadIconBkEffect(this.form.m_formWidgets[2].GetComponent<Image>(), (int) info.stInviterInfo.dwHeadImgId, this.form, 1f);
                this.form.m_formWidgets[7].CustomSetActive(info.stInviterInfo.bGradeOfRank > 0);
                if (info.stInviterInfo.bGradeOfRank > 0)
                {
                    CLadderView.ShowRankDetail(this.form.m_formWidgets[7], info.stInviterInfo.bGradeOfRank, 0, 1, false, true, false, true, true);
                }
            }
        }

        public void ShowRefuse()
        {
            if (this.timerScript != null)
            {
                this.timerScript.set_enabled(false);
            }
            this.ShowRefuseSendState(null);
        }

        private void ShowRefuseListState()
        {
            GameObject obj2 = this.form.get_transform().Find("Panel/Panel/normal").get_gameObject();
            if (obj2 != null)
            {
                obj2.CustomSetActive(false);
            }
            GameObject obj3 = this.form.get_transform().Find("Panel/Panel/refuse").get_gameObject();
            if (obj3 != null)
            {
                obj3.CustomSetActive(true);
            }
            this.m_sendBtn.CustomSetActive(false);
            this.listNode.CustomSetActive(true);
            this.m_inputFiled.set_enabled(false);
        }

        private void ShowRefuseSendState(CUIEvent uievt = new CUIEvent())
        {
            GameObject obj2 = this.form.get_transform().Find("Panel/Panel/normal").get_gameObject();
            if (obj2 != null)
            {
                obj2.CustomSetActive(false);
            }
            GameObject obj3 = this.form.get_transform().Find("Panel/Panel/refuse").get_gameObject();
            if (obj3 != null)
            {
                obj3.CustomSetActive(true);
            }
            this.m_sendBtn.CustomSetActive(true);
            this.listNode.CustomSetActive(false);
            this.m_inputFiled.set_enabled(true);
            if (string.IsNullOrEmpty(this.m_inputFiled.get_text()))
            {
                this.m_inputFiled.set_text(this.m_configTexts[0]);
            }
        }

        private void UnInitEvt()
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Invite_RefuseReason_ClickDown, new CUIEventManager.OnUIEventHandler(this.On_Invite_RefuseReason_ClickDown));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Invite_RefuseReason_ClickList, new CUIEventManager.OnUIEventHandler(this.On_Invite_RefuseReason_ClickList));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Invite_RefuseReason_Send, new CUIEventManager.OnUIEventHandler(this.On_Invite_RefuseReason_Send));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Invite_Form_Closed, new CUIEventManager.OnUIEventHandler(this.On_Invite_Form_Closed));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Invite_RefuseReason_BgClick, new CUIEventManager.OnUIEventHandler(this.ShowRefuseSendState));
        }
    }
}

