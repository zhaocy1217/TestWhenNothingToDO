namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class FriendShower : MonoBehaviour
    {
        public GameObject add_node;
        public GameObject addApprentice_node;
        public GameObject addMentor_node;
        public GameObject black_node;
        public GameObject ChatButton;
        public GameObject del_node;
        public Text distanceTxt;
        public uint dwLogicWorldID;
        public CUIFormScript formScript;
        public GameObject freeze;
        public GameObject full;
        public GameObject genderImage;
        public GameObject Giftutton;
        public Image headIcon;
        public GameObject HeadIconBack;
        public GameObject high;
        public CUIHttpImageScript HttpImage;
        public GameObject intimacyNode;
        public Text intimacyNum;
        public CUIEventScript inviteGuildBtn_eventScript;
        public Text inviteGuildBtnText;
        public Button inviteGuildButton;
        public GameObject lbs_node;
        public Button lbsAddFriendBtn;
        public GameObject lbsBodyNode;
        public Text LevelText;
        public GameObject low;
        public Text m_mentorFamousObj;
        public Text m_mentorTitleObj;
        public GameObject m_RankIconObj;
        public GameObject mentor_graduation;
        public GameObject mentor_relationship;
        public GameObject mentorInfo_node;
        public GameObject mid;
        public Text NameText;
        public GameObject nobeIcon;
        public GameObject normal_node;
        public Button OBButton;
        public Button PKButton;
        public GameObject platChannelIcon;
        public Image pvpIcon;
        public Text pvpText;
        public GameObject QQVipImage;
        public CUIEventScript reCallBtn_eventScript;
        public Button reCallButton;
        public Text reCallText;
        public GameObject request_node;
        public Text SendBtnText;
        public CUIEventScript sendHeartBtn_eventScript;
        public Button sendHeartButton;
        public Image sendHeartIcon;
        public Text time;
        public ulong ullUid;
        public Text VerifyText;
        public Text VipLevel;

        public void HideSendButton()
        {
            if (this.sendHeartButton != null)
            {
                this.sendHeartButton.get_gameObject().CustomSetActive(false);
            }
        }

        public void SetBGray(bool bGray)
        {
            UT.SetImage(this.headIcon, bGray);
        }

        public void SetFriendItemType(ItemType type, bool bShowDelete = true)
        {
            this.Showuid(this.ullUid, this.dwLogicWorldID);
            if (this.inviteGuildButton != null)
            {
                this.inviteGuildButton.get_gameObject().CustomSetActive(false);
            }
            this.add_node.CustomSetActive(false);
            this.normal_node.CustomSetActive(false);
            this.request_node.CustomSetActive(false);
            this.black_node.CustomSetActive(false);
            this.addApprentice_node.CustomSetActive(false);
            this.addMentor_node.CustomSetActive(false);
            this.lbs_node.CustomSetActive(false);
            this.mentorInfo_node.CustomSetActive(false);
            switch (type)
            {
                case ItemType.Add:
                    UT.SetAddNodeActive(this.add_node, CFriendModel.FriendType.Recommend, false);
                    break;

                case ItemType.Normal:
                case ItemType.Mentor:
                case ItemType.Apprentice:
                    this.normal_node.CustomSetActive(true);
                    if (this.del_node != null)
                    {
                        this.del_node.CustomSetActive(bShowDelete || (type != ItemType.Normal));
                    }
                    break;

                case ItemType.Request:
                case ItemType.MentorRequest:
                    this.request_node.CustomSetActive(true);
                    break;

                case ItemType.BlackList:
                    this.black_node.CustomSetActive(true);
                    break;

                case ItemType.LBS:
                    if (this.lbs_node != null)
                    {
                        this.lbs_node.CustomSetActive(true);
                    }
                    break;

                case ItemType.AddMentor:
                    this.mentorInfo_node.CustomSetActive(true);
                    this.addMentor_node.CustomSetActive(true);
                    break;

                case ItemType.AddApprentice:
                    this.addApprentice_node.CustomSetActive(true);
                    break;
            }
            if (this.del_node != null)
            {
                CUIEventScript component = this.del_node.GetComponent<CUIEventScript>();
                CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
                uIEvent.m_eventID = enUIEventID.Friend_DelFriend;
                uIEvent.m_eventParams.tag = (int) type;
                component.SetUIEvent(enUIEventType.Click, uIEvent.m_eventID, uIEvent.m_eventParams);
            }
            if (this.Giftutton != null)
            {
                CUIEventScript script2 = this.Giftutton.GetComponent<CUIEventScript>();
                CUIEvent event3 = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
                event3.m_eventID = enUIEventID.Friend_Gift;
                event3.m_eventParams.tag = (int) type;
                script2.SetUIEvent(enUIEventType.Click, event3.m_eventID, event3.m_eventParams);
            }
            if (this.ChatButton != null)
            {
                CUIEventScript script3 = this.ChatButton.GetComponent<CUIEventScript>();
                CUIEvent event4 = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
                event4.m_eventID = enUIEventID.Friend_Chat_Button;
                event4.m_eventParams.tag = (int) type;
                script3.SetUIEvent(enUIEventType.Click, event4.m_eventID, event4.m_eventParams);
            }
            if (this.VerifyText != null)
            {
                this.VerifyText.get_transform().get_parent().get_gameObject().CustomSetActive((type == ItemType.Request) || (type == ItemType.MentorRequest));
            }
            if (this.lbsBodyNode != null)
            {
                this.lbsBodyNode.get_gameObject().CustomSetActive(type == ItemType.LBS);
            }
        }

        public void ShowDistance(string txt)
        {
            if (this.distanceTxt != null)
            {
                this.distanceTxt.set_text(txt);
            }
        }

        public void ShowGameState(COM_ACNT_GAME_STATE state, bool bOnline)
        {
            if ((state != COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_IDLE) && bOnline)
            {
                this.ShowLastTime(true, "游戏中");
            }
        }

        public static void ShowGender(GameObject genderImage, COM_SNSGENDER genderType)
        {
            if (genderImage != null)
            {
                genderImage.get_gameObject().CustomSetActive(genderType != COM_SNSGENDER.COM_SNSGENDER_NONE);
                if (genderType == COM_SNSGENDER.COM_SNSGENDER_MALE)
                {
                    CUIUtility.SetImageSprite(genderImage.GetComponent<Image>(), string.Format("{0}icon/Ico_boy", "UGUI/Sprite/Dynamic/"), null, true, false, false, false);
                }
                else if (genderType == COM_SNSGENDER.COM_SNSGENDER_FEMALE)
                {
                    CUIUtility.SetImageSprite(genderImage.GetComponent<Image>(), string.Format("{0}icon/Ico_girl", "UGUI/Sprite/Dynamic/"), null, true, false, false, false);
                }
            }
        }

        public void ShowGenderType(COM_SNSGENDER genderType)
        {
            ShowGender(this.genderImage, genderType);
        }

        public void ShowIntimacyNum(int value, CFriendModel.EIntimacyType type, bool bFreeze, COM_INTIMACY_STATE state)
        {
            GameObject obj2 = this.full.get_transform().get_parent().FindChild("rela").get_gameObject();
            if (((state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY) || (state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER)) || ((state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY_DENY) || (state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER_DENY)))
            {
                this.intimacyNum.get_gameObject().CustomSetActive(false);
                this.freeze.CustomSetActive(false);
                this.full.CustomSetActive(false);
                this.high.CustomSetActive(false);
                this.mid.CustomSetActive(false);
                this.low.CustomSetActive(false);
                obj2.CustomSetActive(true);
                if ((state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY) || (state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY_DENY))
                {
                    Utility.FindChild(obj2, "jiyou").get_gameObject().CustomSetActive(true);
                    Utility.FindChild(obj2, "lianren").get_gameObject().CustomSetActive(false);
                    Text componetInChild = Utility.GetComponetInChild<Text>(obj2, "txt");
                    if (componetInChild != null)
                    {
                        componetInChild.get_gameObject().CustomSetActive(true);
                        componetInChild.set_text(UT.FRData().IntimRela_TypeColor_Gay);
                    }
                }
                else if ((state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER) || (state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER_DENY))
                {
                    Utility.FindChild(obj2, "jiyou").get_gameObject().CustomSetActive(false);
                    Utility.FindChild(obj2, "lianren").get_gameObject().CustomSetActive(true);
                    Text text2 = Utility.GetComponetInChild<Text>(obj2, "txt");
                    if (text2 != null)
                    {
                        text2.get_gameObject().CustomSetActive(true);
                        text2.set_text(UT.FRData().IntimRela_TypeColor_Lover);
                    }
                }
            }
            else
            {
                obj2.CustomSetActive(false);
                if (this.intimacyNum != null)
                {
                    this.intimacyNum.get_gameObject().CustomSetActive(true);
                    if (value >= Singleton<CFriendContoller>.instance.model.GetMaxIntimacyNum())
                    {
                        this.intimacyNum.set_text("Max");
                    }
                    else
                    {
                        this.intimacyNum.set_text(value.ToString());
                    }
                }
                if (bFreeze)
                {
                    this.freeze.CustomSetActive(true);
                    this.intimacyNum.set_color(CUIUtility.Intimacy_Freeze);
                }
                else
                {
                    if (this.freeze != null)
                    {
                        this.freeze.CustomSetActive(false);
                    }
                    if (type == CFriendModel.EIntimacyType.Low)
                    {
                        if (this.full != null)
                        {
                            this.full.CustomSetActive(false);
                        }
                        if (this.high != null)
                        {
                            this.high.CustomSetActive(false);
                        }
                        if (this.mid != null)
                        {
                            this.mid.CustomSetActive(false);
                        }
                        if (this.low != null)
                        {
                            this.low.CustomSetActive(true);
                        }
                        this.intimacyNum.set_color(CUIUtility.Intimacy_Low);
                    }
                    else if (type == CFriendModel.EIntimacyType.Middle)
                    {
                        if (this.full != null)
                        {
                            this.full.CustomSetActive(false);
                        }
                        if (this.high != null)
                        {
                            this.high.CustomSetActive(false);
                        }
                        if (this.mid != null)
                        {
                            this.mid.CustomSetActive(true);
                        }
                        if (this.low != null)
                        {
                            this.low.CustomSetActive(false);
                        }
                        this.intimacyNum.set_color(CUIUtility.Intimacy_Mid);
                    }
                    else if (type == CFriendModel.EIntimacyType.High)
                    {
                        if (this.full != null)
                        {
                            this.full.CustomSetActive(false);
                        }
                        if (this.high != null)
                        {
                            this.high.CustomSetActive(true);
                        }
                        if (this.mid != null)
                        {
                            this.mid.CustomSetActive(false);
                        }
                        if (this.low != null)
                        {
                            this.low.CustomSetActive(false);
                        }
                        this.intimacyNum.set_color(CUIUtility.Intimacy_High);
                    }
                    else if (type == CFriendModel.EIntimacyType.full)
                    {
                        if (this.full != null)
                        {
                            this.full.CustomSetActive(true);
                        }
                        if (this.high != null)
                        {
                            this.high.CustomSetActive(false);
                        }
                        if (this.mid != null)
                        {
                            this.mid.CustomSetActive(false);
                        }
                        if (this.low != null)
                        {
                            this.low.CustomSetActive(false);
                        }
                        this.intimacyNum.set_color(CUIUtility.Intimacy_Full);
                    }
                }
            }
        }

        public void ShowInviteButton(bool isShow, bool isEnable)
        {
            if (this.reCallButton != null)
            {
                if (CSysDynamicBlock.bFriendBlocked)
                {
                    this.reCallButton.get_gameObject().CustomSetActive(false);
                }
                else if (!isShow)
                {
                    this.reCallButton.get_gameObject().CustomSetActive(false);
                }
                else
                {
                    if (this.reCallText != null)
                    {
                        if (isEnable)
                        {
                            this.reCallText.set_text(Singleton<CTextManager>.instance.GetText("Friend_ReCall_Tips_1"));
                        }
                        else
                        {
                            this.reCallText.set_text(Singleton<CTextManager>.instance.GetText("Friend_ReCall_Tips_2"));
                        }
                    }
                    this.reCallButton.get_gameObject().CustomSetActive(true);
                    if (this.reCallBtn_eventScript != null)
                    {
                        this.reCallBtn_eventScript.SetUIEvent(enUIEventType.Click, enUIEventID.Friend_SNS_ReCall);
                    }
                    CUICommonSystem.SetButtonEnableWithShader(this.reCallButton, isEnable, true);
                }
            }
        }

        public void ShowinviteGuild(bool isShow, bool isEnable)
        {
            if (this.inviteGuildButton != null)
            {
                if (CSysDynamicBlock.bFriendBlocked && this.inviteGuildButton.get_gameObject().get_activeSelf())
                {
                    this.inviteGuildButton.get_gameObject().SetActive(false);
                }
                else if (!isShow)
                {
                    this.inviteGuildButton.get_gameObject().CustomSetActive(false);
                }
                else
                {
                    this.inviteGuildButton.get_gameObject().CustomSetActive(true);
                    CUICommonSystem.SetButtonEnableWithShader(this.inviteGuildButton, isEnable, true);
                    if (this.inviteGuildBtn_eventScript != null)
                    {
                        this.inviteGuildBtn_eventScript.SetUIEvent(enUIEventType.Click, enUIEventID.Friend_InviteGuild);
                    }
                }
            }
        }

        public void ShowLastTime(bool bShow, string text)
        {
            if (this.time != null)
            {
                this.time.get_gameObject().CustomSetActive(bShow);
                this.time.set_text(text);
            }
        }

        public void ShowLevel(uint level)
        {
            if (this.LevelText != null)
            {
                this.LevelText.set_text("LV." + level.ToString());
            }
        }

        public void ShowMentorSearchInfo(COMDT_FRIEND_INFO info, CFriendModel.FriendType friendType, ItemType type)
        {
            enMentorState mentorState = CFriendContoller.GetMentorState(0, null);
            bool bActive = UT.NeedShowGenderGradeByMentor(type, friendType) && ((mentorState == enMentorState.IWantMentor) || (mentorState == enMentorState.IHasMentor));
            if (this.m_mentorTitleObj != null)
            {
                this.m_mentorTitleObj.get_transform().get_parent().get_gameObject().CustomSetActive(true);
                this.m_mentorTitleObj.get_gameObject().CustomSetActive(bActive);
                string szTitle = string.Empty;
                try
                {
                    szTitle = GameDataMgr.famousMentorDatabin.GetDataByKey(info.dwMasterLvl).szTitle;
                }
                catch (Exception)
                {
                }
                this.m_mentorTitleObj.GetComponent<Text>().set_text(szTitle);
            }
            if (this.m_mentorFamousObj != null)
            {
                this.m_mentorFamousObj.get_gameObject().CustomSetActive(bActive);
                string[] args = new string[] { info.dwMasterLvl.ToString(), info.dwStudentNum.ToString() };
                this.m_mentorFamousObj.set_text(Singleton<CTextManager>.GetInstance().GetText("Mentor_LvNApprenticeCountInfo", args));
            }
            if (this.m_RankIconObj != null)
            {
                int num = CLadderSystem.ConvertEloToRank(info.RankVal[7]);
                CLadderView.ShowRankDetail(this.m_RankIconObj.get_transform().get_parent().get_gameObject(), (byte) num, Singleton<RankingSystem>.GetInstance().GetRankClass(info.stUin.ullUid), 1, false, true, false, false, true);
            }
            if (UT.NeedShowGenderGradeByMentor(type, friendType))
            {
                this.ShowGenderType((COM_SNSGENDER) info.bGender);
            }
        }

        public void ShowName(string name)
        {
            if (this.NameText != null)
            {
                this.NameText.set_text(name);
            }
            if (this.pvpText != null)
            {
                this.pvpText.get_gameObject().CustomSetActive(false);
            }
            if (this.pvpIcon != null)
            {
                this.pvpIcon.get_gameObject().CustomSetActive(false);
            }
            if (this.sendHeartIcon != null)
            {
                this.sendHeartIcon.get_gameObject().CustomSetActive(false);
            }
        }

        public void ShowOBButton(bool isShow)
        {
            if ((this.OBButton != null) && (this.OBButton.get_gameObject() != null))
            {
                this.OBButton.get_gameObject().CustomSetActive(isShow);
            }
        }

        public void ShowPKButton(bool b)
        {
            if (this.PKButton != null)
            {
                if (CSysDynamicBlock.bFriendBlocked && this.PKButton.get_gameObject().get_activeSelf())
                {
                    this.PKButton.get_gameObject().SetActive(false);
                }
                else if (!b)
                {
                    if (this.PKButton.get_gameObject().get_activeSelf())
                    {
                        this.PKButton.get_gameObject().SetActive(false);
                    }
                }
                else if (!this.PKButton.get_gameObject().get_activeSelf())
                {
                    this.PKButton.get_gameObject().SetActive(true);
                }
            }
        }

        public void ShowPlatChannelIcon(COMDT_FRIEND_INFO info)
        {
            if (this.platChannelIcon != null)
            {
                this.platChannelIcon.CustomSetActive(!Utility.IsSamePlatformWithSelf(info.stUin.dwLogicWorldId));
                if (CSysDynamicBlock.bLobbyEntryBlocked)
                {
                    this.platChannelIcon.CustomSetActive(false);
                }
            }
        }

        public void ShowPVP_Level(string text, string icon)
        {
        }

        public void ShowRank(byte RankGrade, uint RankClass)
        {
        }

        public void ShowRecommendGuild(bool isShow, bool isEnabled)
        {
            if (this.inviteGuildButton != null)
            {
                if (CSysDynamicBlock.bFriendBlocked && this.inviteGuildButton.get_gameObject().get_activeSelf())
                {
                    this.inviteGuildButton.get_gameObject().SetActive(false);
                }
                else if (!isShow)
                {
                    if (this.inviteGuildButton.get_gameObject().get_activeSelf())
                    {
                        this.inviteGuildButton.get_gameObject().CustomSetActive(false);
                    }
                }
                else
                {
                    if (!this.inviteGuildButton.get_gameObject().get_activeSelf())
                    {
                        this.inviteGuildButton.get_gameObject().CustomSetActive(true);
                    }
                    if (this.inviteGuildBtn_eventScript != null)
                    {
                        this.inviteGuildBtn_eventScript.SetUIEvent(enUIEventType.Click, enUIEventID.Friend_RecommendGuild);
                    }
                    if (isEnabled)
                    {
                        CUICommonSystem.SetButtonEnable(this.inviteGuildButton, true, true, true);
                        if (this.inviteGuildBtnText != null)
                        {
                            this.inviteGuildBtnText.set_text(Singleton<CFriendContoller>.instance.model.Guild_Recommend_txt);
                        }
                    }
                    else
                    {
                        CUICommonSystem.SetButtonEnable(this.inviteGuildButton, false, false, true);
                        if (this.inviteGuildBtnText != null)
                        {
                            this.inviteGuildBtnText.set_text(Singleton<CFriendContoller>.instance.model.Guild_Has_Recommended_txt);
                        }
                    }
                }
            }
        }

        public void ShowSendButton(bool bEnable)
        {
            if ((this.sendHeartButton != null) && (this.sendHeartButton.get_gameObject() != null))
            {
                CUICommonSystem.SetButtonEnableWithShader(this.sendHeartButton, bEnable, true);
                this.sendHeartButton.get_gameObject().CustomSetActive(true);
            }
        }

        public void ShowSendGiftBtn(bool bShow)
        {
            if (this.Giftutton != null)
            {
                if (CSysDynamicBlock.bFriendBlocked)
                {
                    this.Giftutton.CustomSetActive(false);
                }
                else
                {
                    this.Giftutton.CustomSetActive(bShow);
                }
            }
        }

        private void Showuid(ulong ullUid, uint dwLogicWorldId)
        {
            Transform transform = base.get_transform().FindChild("body/uid");
            if (Singleton<CFriendContoller>.instance.model.bShowUID)
            {
                if (transform != null)
                {
                    transform.get_gameObject().CustomSetActive(true);
                }
                if (transform != null)
                {
                    transform.GetComponent<Text>().set_text(string.Format("uid:{0},world:{1}", ullUid, this.dwLogicWorldID));
                }
            }
            else if (transform != null)
            {
                transform.get_gameObject().CustomSetActive(false);
            }
        }

        public void ShowVerify(string text)
        {
            if (this.VerifyText != null)
            {
                this.VerifyText.set_text(Singleton<CFriendContoller>.instance.model.friend_static_text + text);
            }
        }

        public void ShowVipLevel(uint level)
        {
            if (this.VipLevel != null)
            {
                this.VipLevel.set_text("VIP." + level.ToString());
            }
        }

        public enum ItemType
        {
            Add,
            Normal,
            Request,
            BlackList,
            LBS,
            MentorRequest,
            AddMentor,
            AddApprentice,
            Mentor,
            Apprentice
        }
    }
}

