using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using CSProtocol;
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class CMailView
{
    private GameObject m_allDeleteMsgCenterButton;
    private GameObject m_allReceiveFriButton;
    private GameObject m_allReceiveSysButton;
    private CUIFormScript m_CUIForm;
    private CUIListScript m_CUIListScriptTab;
    private COM_MAIL_TYPE m_curMailtype = COM_MAIL_TYPE.COM_MAIL_SYSTEM;
    private int m_friUnReadNum;
    private int m_msgUnReadNum;
    private GameObject m_panelFri;
    private GameObject m_panelMsg;
    private GameObject m_panelSys;
    private GameObject m_SysDeleteBtn;
    private int m_sysUnReadNum;

    public void Close()
    {
        Singleton<CUIManager>.GetInstance().CloseForm(CMailSys.MAIL_FORM_PATH);
        this.OnClose();
    }

    public int GetFriendMailListSelectedIndex()
    {
        if (this.m_panelFri != null)
        {
            CUIListScript component = this.m_panelFri.get_transform().Find("List").GetComponent<CUIListScript>();
            if (component != null)
            {
                return component.GetSelectedIndex();
            }
        }
        return -1;
    }

    private bool GetOtherPlayerState(COM_INVITE_RELATION_TYPE type, ulong uid, uint dwLogicWorldID, out string stateStr, out string headURL)
    {
        headURL = string.Empty;
        if (type == COM_INVITE_RELATION_TYPE.COM_INVITE_RELATION_FRIEND)
        {
            CFriendModel.FriendInGame friendInGaming = Singleton<CFriendContoller>.instance.model.GetFriendInGaming(uid, dwLogicWorldID);
            COMDT_FRIEND_INFO gameOrSnsFriend = Singleton<CFriendContoller>.instance.model.GetGameOrSnsFriend(uid, dwLogicWorldID);
            if (gameOrSnsFriend == null)
            {
                stateStr = Singleton<CMailSys>.instance.offlineStr;
                return false;
            }
            headURL = Utility.UTF8Convert(gameOrSnsFriend.szHeadUrl);
            if (gameOrSnsFriend.bIsOnline == 1)
            {
                stateStr = Singleton<CMailSys>.instance.onlineStr;
            }
            else
            {
                stateStr = Singleton<CMailSys>.instance.offlineStr;
                return false;
            }
            if (friendInGaming != null)
            {
                if ((friendInGaming.State == COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_SINGLEGAME) || (friendInGaming.State == COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_MULTIGAME))
                {
                    if (friendInGaming.startTime > 0)
                    {
                        stateStr = Singleton<CMailSys>.instance.gamingStr;
                    }
                    else
                    {
                        stateStr = string.Format("<color=#ffff00>{0}</color>", Singleton<CTextManager>.instance.GetText("Common_Gaming_NoTime"));
                    }
                }
                else if (friendInGaming.State == COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_TEAM)
                {
                    stateStr = string.Format("<color=#ffff00>{0}</color>", Singleton<CTextManager>.instance.GetText("Common_Teaming"));
                }
            }
            return (gameOrSnsFriend.bIsOnline == 1);
        }
        if (type == COM_INVITE_RELATION_TYPE.COM_INVITE_RELATION_GUILDMEMBER)
        {
            GuildMemInfo guildMemberInfoByUid = CGuildHelper.GetGuildMemberInfoByUid(uid);
            if (guildMemberInfoByUid == null)
            {
                stateStr = Singleton<CMailSys>.instance.offlineStr;
                return false;
            }
            headURL = guildMemberInfoByUid.stBriefInfo.szHeadUrl;
            if (!CGuildHelper.IsMemberOnline(guildMemberInfoByUid))
            {
                stateStr = Singleton<CMailSys>.instance.offlineStr;
                return false;
            }
            stateStr = Singleton<CMailSys>.instance.onlineStr;
            if ((guildMemberInfoByUid.GameState == COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_SINGLEGAME) || (guildMemberInfoByUid.GameState == COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_MULTIGAME))
            {
                if (guildMemberInfoByUid.dwGameStartTime > 0)
                {
                    stateStr = Singleton<CMailSys>.instance.gamingStr;
                }
                else
                {
                    stateStr = string.Format("<color=#ffff00>{0}</color>", Singleton<CTextManager>.instance.GetText("Common_Gaming_NoTime"));
                }
            }
            return true;
        }
        if (type == COM_INVITE_RELATION_TYPE.COM_INVITE_RELATION_LBS)
        {
            CSDT_LBS_USER_INFO csdt_lbs_user_info = Singleton<CFriendContoller>.instance.model.GetLBSUserInfo(uid, dwLogicWorldID, CFriendModel.LBSGenderType.Both);
            if (csdt_lbs_user_info == null)
            {
                stateStr = Singleton<CMailSys>.instance.offlineStr;
                return false;
            }
            headURL = Utility.UTF8Convert(csdt_lbs_user_info.stLbsUserInfo.szHeadUrl);
            if (csdt_lbs_user_info.stLbsUserInfo.bIsOnline == 1)
            {
                stateStr = Singleton<CMailSys>.instance.onlineStr;
            }
            else
            {
                stateStr = Singleton<CMailSys>.instance.offlineStr;
            }
            return (csdt_lbs_user_info.stLbsUserInfo.bIsOnline == 1);
        }
        stateStr = Singleton<CMailSys>.instance.offlineStr;
        return false;
    }

    private string GetProcessTypeString(CMailSys.enProcessInviteType type)
    {
        switch (type)
        {
            case CMailSys.enProcessInviteType.Refuse:
                return Singleton<CMailSys>.instance.inviteRefuseStr;

            case CMailSys.enProcessInviteType.Accept:
                return Singleton<CMailSys>.instance.inviteAcceptStr;

            case CMailSys.enProcessInviteType.NoProcess:
                return Singleton<CMailSys>.instance.inviteNoProcessStr;
        }
        return "error";
    }

    public void OnClose()
    {
        this.m_CUIForm = null;
        this.m_CUIListScriptTab = null;
        this.m_panelFri = null;
        this.m_panelSys = null;
        this.m_panelMsg = null;
        this.m_SysDeleteBtn = null;
        this.m_allReceiveSysButton = null;
        this.m_allReceiveFriButton = null;
        this.m_allDeleteMsgCenterButton = null;
    }

    public void Open(COM_MAIL_TYPE mailType)
    {
        this.m_CUIForm = Singleton<CUIManager>.GetInstance().OpenForm(CMailSys.MAIL_FORM_PATH, false, true);
        if (this.m_CUIForm != null)
        {
            this.m_CUIListScriptTab = this.m_CUIForm.get_transform().FindChild("TopCommon/Panel_Menu/ListMenu").GetComponent<CUIListScript>();
            this.m_CUIListScriptTab.SetElementAmount(3);
            this.m_CUIListScriptTab.GetElemenet(0).get_transform().FindChild("Text").GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("Mail_Friend"));
            this.m_CUIListScriptTab.GetElemenet(1).get_transform().FindChild("Text").GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("Mail_System"));
            this.m_CUIListScriptTab.GetElemenet(2).get_transform().FindChild("Text").GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("Mail_MsgCenter"));
            this.m_CUIListScriptTab.GetElemenet(0).GetComponent<CUIEventScript>().SetUIEvent(enUIEventType.Click, enUIEventID.Mail_TabFriend);
            this.m_CUIListScriptTab.GetElemenet(1).GetComponent<CUIEventScript>().SetUIEvent(enUIEventType.Click, enUIEventID.Mail_TabSystem);
            this.m_CUIListScriptTab.GetElemenet(2).GetComponent<CUIEventScript>().SetUIEvent(enUIEventType.Click, enUIEventID.Mail_TabMsgCenter);
            this.m_panelFri = this.m_CUIForm.get_transform().FindChild("PanelFriMail").get_gameObject();
            this.m_panelSys = this.m_CUIForm.get_transform().FindChild("PanelSysMail").get_gameObject();
            this.m_panelMsg = this.m_CUIForm.get_transform().FindChild("PanelMsgMail").get_gameObject();
            this.m_SysDeleteBtn = this.m_panelSys.get_transform().FindChild("ButtonGrid/DeleteButton").get_gameObject();
            this.m_allReceiveSysButton = this.m_panelSys.get_transform().FindChild("ButtonGrid/AllReceiveButton").get_gameObject();
            this.m_allReceiveFriButton = this.m_panelFri.get_transform().FindChild("AllReceiveButton").get_gameObject();
            this.m_allDeleteMsgCenterButton = this.m_panelMsg.get_transform().FindChild("AllDeleteButton").get_gameObject();
            this.SetUnReadNum(COM_MAIL_TYPE.COM_MAIL_FRIEND, this.m_friUnReadNum);
            this.SetUnReadNum(COM_MAIL_TYPE.COM_MAIL_SYSTEM, this.m_sysUnReadNum);
            this.SetUnReadNum(COM_MAIL_TYPE.COM_MAIL_FRIEND_INVITE, this.m_msgUnReadNum);
            this.CurMailType = mailType;
        }
    }

    public void SetActiveTab(int index)
    {
        if (index == 0)
        {
            this.m_panelFri.CustomSetActive(true);
            this.m_panelSys.CustomSetActive(false);
            this.m_panelMsg.CustomSetActive(false);
        }
        else if (index == 1)
        {
            this.m_panelFri.CustomSetActive(false);
            this.m_panelSys.CustomSetActive(true);
            this.m_panelMsg.CustomSetActive(false);
        }
        else if (index == 2)
        {
            this.m_panelFri.CustomSetActive(false);
            this.m_panelSys.CustomSetActive(false);
            this.m_panelMsg.CustomSetActive(true);
        }
    }

    private void SetEventParams(CUIEventScript com, CMail mail)
    {
        com.m_onClickEventParams.heroId = mail.bMapType;
        com.m_onClickEventParams.weakGuideId = mail.dwMapId;
        com.m_onClickEventParams.tag2 = mail.relationType;
        com.m_onClickEventParams.tagUInt = mail.dwGameSvrEntity;
        com.m_onClickEventParams.commonUInt64Param1 = mail.uid;
        com.m_onClickEventParams.taskId = mail.dwLogicWorldID;
        com.m_onClickEventParams.tag3 = mail.inviteType;
    }

    private void SetEventParams(GameObject node, CMail mail)
    {
        if ((node != null) && (mail != null))
        {
            Transform transform = node.get_transform().FindChild("invite_btn");
            if (transform != null)
            {
                CUIEventScript component = transform.GetComponent<CUIEventScript>();
                if (component != null)
                {
                    this.SetEventParams(component, mail);
                }
            }
        }
    }

    public void SetUnReadNum(COM_MAIL_TYPE mailtype, int unReadNum)
    {
        if (mailtype == COM_MAIL_TYPE.COM_MAIL_FRIEND)
        {
            this.m_friUnReadNum = unReadNum;
        }
        else if (mailtype == COM_MAIL_TYPE.COM_MAIL_SYSTEM)
        {
            this.m_sysUnReadNum = unReadNum;
        }
        else if (mailtype == COM_MAIL_TYPE.COM_MAIL_FRIEND_INVITE)
        {
            this.m_msgUnReadNum = unReadNum;
        }
        if (this.m_CUIListScriptTab != null)
        {
            if ((mailtype == COM_MAIL_TYPE.COM_MAIL_FRIEND) && (this.m_CUIListScriptTab.GetElemenet(0) != null))
            {
                if (unReadNum > 9)
                {
                    CUICommonSystem.AddRedDot(this.m_CUIListScriptTab.GetElemenet(0).get_gameObject(), enRedDotPos.enTopRight, 0);
                }
                else if (unReadNum > 0)
                {
                    CUICommonSystem.AddRedDot(this.m_CUIListScriptTab.GetElemenet(0).get_gameObject(), enRedDotPos.enTopRight, unReadNum);
                }
                else
                {
                    CUICommonSystem.DelRedDot(this.m_CUIListScriptTab.GetElemenet(0).get_gameObject());
                }
            }
            else if ((mailtype == COM_MAIL_TYPE.COM_MAIL_SYSTEM) && (this.m_CUIListScriptTab.GetElemenet(1) != null))
            {
                if (unReadNum > 9)
                {
                    CUICommonSystem.AddRedDot(this.m_CUIListScriptTab.GetElemenet(1).get_gameObject(), enRedDotPos.enTopRight, 0);
                }
                else if (unReadNum > 0)
                {
                    CUICommonSystem.AddRedDot(this.m_CUIListScriptTab.GetElemenet(1).get_gameObject(), enRedDotPos.enTopRight, unReadNum);
                }
                else
                {
                    CUICommonSystem.DelRedDot(this.m_CUIListScriptTab.GetElemenet(1).get_gameObject());
                }
            }
            else if ((mailtype == COM_MAIL_TYPE.COM_MAIL_FRIEND_INVITE) && (this.m_CUIListScriptTab.GetElemenet(2) != null))
            {
                if (unReadNum > 0)
                {
                    CUICommonSystem.AddRedDot(this.m_CUIListScriptTab.GetElemenet(2).get_gameObject(), enRedDotPos.enTopRight, 0);
                }
                else
                {
                    CUICommonSystem.DelRedDot(this.m_CUIListScriptTab.GetElemenet(2).get_gameObject());
                }
            }
        }
    }

    public void UpdateListElenment(GameObject element, CMail mail)
    {
        int currentUTCTime = CRoleInfo.GetCurrentUTCTime();
        Text component = element.get_transform().FindChild("Title").GetComponent<Text>();
        Text text2 = element.get_transform().FindChild("MailTime").GetComponent<Text>();
        GameObject obj2 = element.get_transform().FindChild("New").get_gameObject();
        GameObject obj3 = element.get_transform().FindChild("ReadMailIcon").get_gameObject();
        GameObject obj4 = element.get_transform().FindChild("UnReadMailIcon").get_gameObject();
        GameObject obj5 = element.get_transform().FindChild("CoinImg").get_gameObject();
        Text text3 = element.get_transform().FindChild("From").GetComponent<Text>();
        CUIHttpImageScript script = element.get_transform().FindChild("HeadBg/imgHead").GetComponent<CUIHttpImageScript>();
        GameObject obj6 = null;
        Text text4 = null;
        Transform transform = element.get_transform().FindChild("OnlineBg");
        if (transform != null)
        {
            obj6 = transform.get_gameObject();
        }
        Transform transform2 = element.get_transform().FindChild("Online");
        if (transform2 != null)
        {
            text4 = transform2.GetComponent<Text>();
        }
        component.set_text(mail.subject);
        text2.set_text(Utility.GetTimeBeforString((long) mail.sendTime, (long) currentUTCTime));
        bool bActive = mail.mailState == COM_MAIL_STATE.COM_MAIL_UNREAD;
        obj2.CustomSetActive(bActive);
        if (mail.mailType == COM_MAIL_TYPE.COM_MAIL_SYSTEM)
        {
            obj3.CustomSetActive(!bActive);
            obj4.CustomSetActive(bActive);
            text3.set_text(string.Empty);
            script.get_gameObject().CustomSetActive(false);
            obj5.SetActive(false);
            obj6.CustomSetActive(false);
            if (text4 != null)
            {
                text4.get_gameObject().CustomSetActive(false);
            }
        }
        else if (mail.mailType == COM_MAIL_TYPE.COM_MAIL_FRIEND)
        {
            obj6.CustomSetActive(false);
            if (text4 != null)
            {
                text4.get_gameObject().CustomSetActive(false);
            }
            obj3.CustomSetActive(false);
            obj4.CustomSetActive(false);
            text3.set_text(mail.from);
            script.get_gameObject().CustomSetActive(true);
            if (mail.subType == 3)
            {
                obj5.CustomSetActive(false);
                script.SetImageSprite(CGuildHelper.GetGuildHeadPath(), this.m_CUIForm);
            }
            else
            {
                obj5.CustomSetActive(true);
                if (!CSysDynamicBlock.bFriendBlocked)
                {
                    COMDT_FRIEND_INFO comdt_friend_info = Singleton<CFriendContoller>.instance.model.getFriendByName(mail.from, CFriendModel.FriendType.GameFriend);
                    if (comdt_friend_info == null)
                    {
                        comdt_friend_info = Singleton<CFriendContoller>.instance.model.getFriendByName(mail.from, CFriendModel.FriendType.SNS);
                    }
                    if (comdt_friend_info != null)
                    {
                        string url = Utility.UTF8Convert(comdt_friend_info.szHeadUrl);
                        script.SetImageUrl(Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(url));
                    }
                }
            }
        }
        else if (mail.mailType == COM_MAIL_TYPE.COM_MAIL_FRIEND_INVITE)
        {
            string str2;
            string str3;
            obj6.CustomSetActive(true);
            if (text4 != null)
            {
                text4.get_gameObject().CustomSetActive(true);
            }
            obj3.CustomSetActive(false);
            obj4.CustomSetActive(false);
            text3.set_text(string.Empty);
            script.get_gameObject().CustomSetActive(true);
            obj5.SetActive(false);
            Transform transform3 = element.get_transform().FindChild("invite_btn");
            GameObject obj7 = null;
            if (transform3 != null)
            {
                obj7 = transform3.get_gameObject();
            }
            if (mail.relationType == 1)
            {
                GuildMemInfo guildMemberInfoByUid = CGuildHelper.GetGuildMemberInfoByUid(mail.uid);
                Singleton<CMailSys>.instance.AddGuildMemInfo(guildMemberInfoByUid);
            }
            this.SetEventParams(element, mail);
            bool flag2 = !this.GetOtherPlayerState((COM_INVITE_RELATION_TYPE) mail.relationType, mail.uid, mail.dwLogicWorldID, out str2, out str3);
            string processTypeString = this.GetProcessTypeString((CMailSys.enProcessInviteType) mail.processType);
            component.set_text(string.Format("{0} {1}", mail.subject, processTypeString));
            if (text4 != null)
            {
                text4.set_text(str2);
            }
            script.SetImageUrl(Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(str3));
            if (flag2)
            {
                CUIUtility.GetComponentInChildren<Image>(script.get_gameObject()).set_color(CUIUtility.s_Color_GrayShader);
            }
            else
            {
                CUIUtility.GetComponentInChildren<Image>(script.get_gameObject()).set_color(CUIUtility.s_Color_Full);
            }
            obj7.CustomSetActive(!flag2);
        }
    }

    public void UpdateMailList(COM_MAIL_TYPE mailtype, ListView<CMail> mailList)
    {
        if ((this.m_CUIForm != null) && (mailList != null))
        {
            CUIListElementScript elemenet = null;
            CUIListScript component = null;
            int currentUTCTime = CRoleInfo.GetCurrentUTCTime();
            int num2 = -1;
            if (mailtype == COM_MAIL_TYPE.COM_MAIL_FRIEND)
            {
                component = this.m_CUIForm.get_transform().FindChild("PanelFriMail/List").GetComponent<CUIListScript>();
                component.SetElementAmount(mailList.Count);
                for (int i = 0; i < mailList.Count; i++)
                {
                    elemenet = component.GetElemenet(i);
                    if ((elemenet != null) && (elemenet.get_gameObject() != null))
                    {
                        this.UpdateListElenment(elemenet.get_gameObject(), mailList[i]);
                    }
                    if ((num2 == -1) && (mailList[i].subType == 1))
                    {
                        num2 = i;
                    }
                }
                this.m_allReceiveFriButton.CustomSetActive(num2 >= 0);
            }
            else if (mailtype == COM_MAIL_TYPE.COM_MAIL_SYSTEM)
            {
                this.m_CUIForm.get_transform().FindChild("PanelSysMail/List").GetComponent<CUIListScript>().SetElementAmount(mailList.Count);
                for (int j = 0; j < mailList.Count; j++)
                {
                    if ((elemenet != null) && (elemenet.get_gameObject() != null))
                    {
                        this.UpdateListElenment(elemenet.get_gameObject(), mailList[j]);
                    }
                    if ((num2 == -1) && (mailList[j].subType == 2))
                    {
                        num2 = j;
                    }
                }
                this.m_allReceiveSysButton.CustomSetActive(num2 >= 0);
                this.m_SysDeleteBtn.CustomSetActive(mailList.Count > 0);
            }
            else if (mailtype == COM_MAIL_TYPE.COM_MAIL_FRIEND_INVITE)
            {
                this.m_CUIForm.get_transform().FindChild("PanelMsgMail/List").GetComponent<CUIListScript>().SetElementAmount(mailList.Count);
                for (int k = 0; k < mailList.Count; k++)
                {
                    if ((elemenet != null) && (elemenet.get_gameObject() != null))
                    {
                        this.UpdateListElenment(elemenet.get_gameObject(), mailList[k]);
                    }
                }
                this.m_allDeleteMsgCenterButton.CustomSetActive(mailList.Count > 0);
            }
        }
    }

    public COM_MAIL_TYPE CurMailType
    {
        get
        {
            return this.m_curMailtype;
        }
        set
        {
            if (this.m_CUIListScriptTab != null)
            {
                this.m_curMailtype = value;
                if (this.m_curMailtype == COM_MAIL_TYPE.COM_MAIL_FRIEND)
                {
                    this.SetActiveTab(0);
                    this.m_CUIListScriptTab.SelectElement(0, true);
                }
                else if (this.m_curMailtype == COM_MAIL_TYPE.COM_MAIL_SYSTEM)
                {
                    this.SetActiveTab(1);
                    this.m_CUIListScriptTab.SelectElement(1, true);
                }
                else if (this.m_curMailtype == COM_MAIL_TYPE.COM_MAIL_FRIEND_INVITE)
                {
                    this.SetActiveTab(2);
                    this.m_CUIListScriptTab.SelectElement(2, true);
                }
            }
        }
    }
}

