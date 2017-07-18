namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Linq;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class FriendRecruitView
    {
        public GameObject beizhaomu_content;
        public const string FRDataChange = "FRDataChange";
        public GameObject node;
        private static int ruleId = 0x17;
        public GameObject supberRewardNode;
        public Text supberRewardNodeTxt;
        public GameObject zhaomu_content;
        public CUIListScript zhaomuzheList;
        private int zhaomuzheRewardCount = 3;
        public Text zm_benifit_exp;
        public Text zm_benifit_gold;
        public Text zm_boxTxt;
        public GameObject zm_progressNode;
        public Text zm_totalProgress;
        private static int zmzCount = 4;
        private static int zmzLevel = 20;

        public void Clear()
        {
            this.node = null;
            this.zhaomuzheList = null;
            this.zhaomu_content = null;
            this.zm_benifit_exp = null;
            this.zm_benifit_gold = null;
            this.zm_totalProgress = null;
            this.zm_progressNode = null;
            this.zm_boxTxt = null;
            this.supberRewardNode = null;
            this.supberRewardNodeTxt = null;
            this.beizhaomu_content = null;
            Singleton<EventRouter>.GetInstance().RemoveEventHandler("FRDataChange", new Action(this, (IntPtr) this.On_FRDataChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Friend_Recruit_zmzBtn, new CUIEventManager.OnUIEventHandler(this.On_Friend_Recruit_zmzBtn));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Friend_Recruit_bzmzBtn, new CUIEventManager.OnUIEventHandler(this.On_Friend_Recruit_bzmzBtn));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Friend_Recruit_bzmRoleBtn, new CUIEventManager.OnUIEventHandler(this.On_Friend_Recruit_bzmRoleBtn));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Friend_Recruit_RecruitBtn, new CUIEventManager.OnUIEventHandler(this.On_Friend_Recruit_RecruitBtn));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Friend_Recruit_zmzListEnable, new CUIEventManager.OnUIEventHandler(this.On_Friend_Recruit_zmzListEnable));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Friend_Recruit_zmzItemClickDown, new CUIEventManager.OnUIEventHandler(this.On_Friend_Recruit_zmzItemClickDown));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Friend_Recruit_TabChange, new CUIEventManager.OnUIEventHandler(this.On_Friend_Recruit_TabChange));
        }

        public void Hide()
        {
            this.node.CustomSetActive(false);
        }

        public void Init(GameObject node)
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_Recruit_zmzBtn, new CUIEventManager.OnUIEventHandler(this.On_Friend_Recruit_zmzBtn));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_Recruit_bzmzBtn, new CUIEventManager.OnUIEventHandler(this.On_Friend_Recruit_bzmzBtn));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_Recruit_bzmRoleBtn, new CUIEventManager.OnUIEventHandler(this.On_Friend_Recruit_bzmRoleBtn));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_Recruit_RecruitBtn, new CUIEventManager.OnUIEventHandler(this.On_Friend_Recruit_RecruitBtn));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_Recruit_zmzListEnable, new CUIEventManager.OnUIEventHandler(this.On_Friend_Recruit_zmzListEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_Recruit_zmzItemClickDown, new CUIEventManager.OnUIEventHandler(this.On_Friend_Recruit_zmzItemClickDown));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_Recruit_TabChange, new CUIEventManager.OnUIEventHandler(this.On_Friend_Recruit_TabChange));
            Singleton<EventRouter>.GetInstance().AddEventHandler("FRDataChange", new Action(this, (IntPtr) this.On_FRDataChange));
            this.node = node;
            this.zhaomu_content = node.get_transform().FindChild("zhaomu_content").get_gameObject();
            this.zm_benifit_exp = node.get_transform().FindChild("zhaomu_content/benift/exp/icon/txt").GetComponent<Text>();
            this.zm_benifit_gold = node.get_transform().FindChild("zhaomu_content/benift/gold/icon/txt").GetComponent<Text>();
            this.zm_totalProgress = node.get_transform().FindChild("zhaomu_content/progress_node/progress_txt").GetComponent<Text>();
            this.zm_progressNode = node.get_transform().FindChild("zhaomu_content/progress_node/progress").get_gameObject();
            this.zm_boxTxt = node.get_transform().FindChild("zhaomu_content/progress_node/icon/txt").GetComponent<Text>();
            this.zhaomuzheList = node.get_transform().FindChild("zhaomu_content/list").GetComponent<CUIListScript>();
            this.supberRewardNode = node.get_transform().FindChild("zhaomu_content/progress_node/superReward").get_gameObject();
            this.supberRewardNodeTxt = node.get_transform().FindChild("zhaomu_content/progress_node/superReward/box/num").GetComponent<Text>();
            this.beizhaomu_content = node.get_transform().FindChild("beizhaomu_content").get_gameObject();
            CUIListScript component = node.get_transform().FindChild("tab").GetComponent<CUIListScript>();
            string[] strArray = new string[] { Singleton<CTextManager>.instance.GetText("Friend_Rec_ZMZTitle"), Singleton<CTextManager>.instance.GetText("Friend_Rec_BZMZTitle") };
            UT.SetTabList(Enumerable.ToList<string>(strArray), 0, component);
            this.ShowNode(true);
        }

        private void On_FRDataChange()
        {
            if (this.zhaomu_content != null)
            {
                this.ShowNode(this.zhaomu_content.get_activeSelf());
            }
        }

        private void On_Friend_Recruit_bzmRoleBtn(CUIEvent uievent)
        {
            ResRuleText dataByKey = GameDataMgr.s_ruleTextDatabin.GetDataByKey((long) ruleId);
            if (dataByKey != null)
            {
                string title = StringHelper.UTF8BytesToString(ref dataByKey.szTitle);
                string info = StringHelper.UTF8BytesToString(ref dataByKey.szContent);
                Singleton<CUIManager>.GetInstance().OpenInfoForm(title, info);
            }
        }

        private void On_Friend_Recruit_bzmzBtn(CUIEvent uievent)
        {
        }

        private void On_Friend_Recruit_RecruitBtn(CUIEvent uievent)
        {
            MonoSingleton<ShareSys>.GetInstance().ShareRecruitFriend(Singleton<CTextManager>.GetInstance().GetText("ShareRecruit_Title"), Singleton<CTextManager>.GetInstance().GetText("ShareRecruit_Desc"));
        }

        private void On_Friend_Recruit_TabChange(CUIEvent uievent)
        {
            switch (uievent.m_srcWidget.GetComponent<CUIListScript>().GetSelectedIndex())
            {
                case 0:
                    this.ShowNode(true);
                    break;

                case 1:
                    this.ShowNode(false);
                    break;
            }
        }

        private void On_Friend_Recruit_zmzBtn(CUIEvent uievent)
        {
        }

        private void On_Friend_Recruit_zmzItemClickDown(CUIEvent uievent)
        {
            ushort tagUInt = (ushort) uievent.m_eventParams.tagUInt;
            ulong ullUid = uievent.m_eventParams.commonUInt64Param1;
            uint taskId = uievent.m_eventParams.taskId;
            COM_RECRUITMENT_TYPE weakGuideId = (COM_RECRUITMENT_TYPE) uievent.m_eventParams.weakGuideId;
            if (tagUInt != 0)
            {
                CFriendRecruit.RecruitReward superReward = null;
                CFriendRecruit friendRecruit = Singleton<CFriendContoller>.instance.model.friendRecruit;
                if (friendRecruit.SuperReward.rewardID == tagUInt)
                {
                    superReward = friendRecruit.SuperReward;
                }
                else
                {
                    superReward = friendRecruit.GetRecruitReward(ullUid, taskId, tagUInt);
                }
                if ((superReward != null) && (superReward.state == CFriendRecruit.RewardState.Getted))
                {
                    Singleton<CUIManager>.instance.OpenTips(UT.GetText("CS_HUOYUEDUREWARD_GETED"), false, 1.5f, null, new object[0]);
                }
                else if ((superReward != null) && (superReward.state == CFriendRecruit.RewardState.Keling))
                {
                    CFriendRecruitNetCore.Send_INTIMACY_RELATION_REQUEST(ullUid, taskId, tagUInt);
                }
                else
                {
                    CUseable usable = friendRecruit.GetUsable(tagUInt);
                    Singleton<CUICommonSystem>.instance.OpenUseableTips(usable, uievent.m_pointerEventData.get_pressPosition().x, uievent.m_pointerEventData.get_pressPosition().y, enUseableTipsPos.enTop);
                }
            }
        }

        public void On_Friend_Recruit_zmzListEnable(CUIEvent uievent)
        {
            if (uievent != null)
            {
                int srcWidgetIndexInBelongedList = uievent.m_srcWidgetIndexInBelongedList;
                ListView<CFriendRecruit.RecruitData> zhaoMuZheRewardList = Singleton<CFriendContoller>.GetInstance().model.friendRecruit.GetZhaoMuZheRewardList();
                if (zhaoMuZheRewardList != null)
                {
                    CFriendRecruit.RecruitData info = null;
                    if ((srcWidgetIndexInBelongedList >= 0) && (srcWidgetIndexInBelongedList < zhaoMuZheRewardList.Count))
                    {
                        info = zhaoMuZheRewardList[srcWidgetIndexInBelongedList];
                    }
                    if (((info != null) && (uievent.m_srcWidget != null)) && (uievent.m_srcWidget != null))
                    {
                        this.ShowZhaomuZhe_Item(uievent.m_srcWidget, info);
                    }
                }
            }
        }

        public void Refresh_ZhaomuZhe_List()
        {
            ListView<CFriendRecruit.RecruitData> zhaoMuZheRewardList = Singleton<CFriendContoller>.GetInstance().model.friendRecruit.GetZhaoMuZheRewardList();
            this.zhaomuzheList.SetElementAmount(zhaoMuZheRewardList.Count);
            for (int i = 0; i < zhaoMuZheRewardList.Count; i++)
            {
                CUIListElementScript elemenet = this.zhaomuzheList.GetElemenet(i);
                if (elemenet != null)
                {
                    this.ShowZhaomuZhe_Item(elemenet.get_gameObject(), zhaoMuZheRewardList[i]);
                }
            }
        }

        public void Show()
        {
            this.node.CustomSetActive(true);
            Singleton<CFriendContoller>.instance.model.friendRecruit.Check();
            this.On_FRDataChange();
        }

        public void Show_Award(GameObject node, ulong ullUid, uint dwLogicWorldId, COM_RECRUITMENT_TYPE type, ushort rewardID, CFriendRecruit.RewardState state, CUIFormScript formScript, bool bShowLevelNum = true)
        {
            Image component = node.get_transform().FindChild("box/icon").GetComponent<Image>();
            CUIEventScript script = component.GetComponent<CUIEventScript>();
            script.m_onDownEventParams.tagUInt = rewardID;
            script.m_onDownEventParams.commonUInt64Param1 = ullUid;
            script.m_onDownEventParams.taskId = dwLogicWorldId;
            script.m_onDownEventParams.weakGuideId = (byte) type;
            ResRecruitmentReward cfgReward = Singleton<CFriendContoller>.instance.model.friendRecruit.GetCfgReward(rewardID);
            if (cfgReward != null)
            {
                component.SetSprite(CUIUtility.s_Sprite_Dynamic_Icon_Dir + cfgReward.szIcon, formScript, true, false, false, false);
                if (bShowLevelNum)
                {
                    node.get_transform().FindChild("box/num").GetComponent<Text>().set_text(cfgReward.dwLevel.ToString());
                }
                bool bActive = state == CFriendRecruit.RewardState.Getted;
                node.get_transform().FindChild("box/mark").GetComponent<Image>().get_gameObject().CustomSetActive(bActive);
                Transform transform = node.get_transform().FindChild("icon");
                if (transform != null)
                {
                    this.ShowBar(transform.get_gameObject(), bActive);
                }
                bool flag2 = state == CFriendRecruit.RewardState.Keling;
                node.get_transform().FindChild("box/effect").get_gameObject().CustomSetActive(flag2);
                node.get_transform().FindChild("box").GetComponent<Animation>().set_enabled(flag2);
            }
        }

        private void Show_BeiZhouMoZhe_Reward()
        {
            if ((this.zhaomu_content != null) && (this.beizhaomu_content != null))
            {
                this.zhaomu_content.CustomSetActive(false);
                this.beizhaomu_content.CustomSetActive(true);
                CFriendRecruit friendRecruit = Singleton<CFriendContoller>.instance.model.friendRecruit;
                Text component = this.beizhaomu_content.get_transform().FindChild("info/text").GetComponent<Text>();
                Text text2 = this.beizhaomu_content.get_transform().FindChild("info/benift/exp/icon/txt").GetComponent<Text>();
                Text text3 = this.beizhaomu_content.get_transform().FindChild("info/benift/gold/icon/txt").GetComponent<Text>();
                if (text2 != null)
                {
                    text2.set_text(string.Format("+{0}%", friendRecruit.GetBeiZhaoMuZhe_RewardExp()));
                }
                if (text3 != null)
                {
                    text3.set_text(string.Format("+{0}%", friendRecruit.GetBeiZhaoMuZhe_RewardGold()));
                }
                GameObject obj2 = this.beizhaomu_content.get_transform().FindChild("info/user").get_gameObject();
                obj2.CustomSetActive(true);
                GameObject obj3 = obj2.get_transform().FindChild("default").get_gameObject();
                obj3.GetComponent<Text>().set_text(Singleton<CTextManager>.instance.GetText("Friend_Rec_Bei_User_DefalutTxt"));
                this.beizhaomu_content.get_transform().FindChild("info/reward/title/Text").GetComponent<Text>().set_text(Singleton<CTextManager>.instance.GetText("Friend_Rec_Bei_RewardTip"));
                GameObject obj4 = obj2.get_transform().FindChild("NameGroup").get_gameObject();
                CFriendRecruit.RecruitData beiZhaoMuZhe = friendRecruit.GetBeiZhaoMuZhe();
                Text text4 = obj2.get_transform().FindChild("Level").GetComponent<Text>();
                if (beiZhaoMuZhe.userInfo == null)
                {
                    component.set_text(Singleton<CTextManager>.instance.GetText("Friend_Rec_Bei_Role_NoData"));
                    obj3.CustomSetActive(true);
                    obj4.CustomSetActive(false);
                    text4.get_gameObject().CustomSetActive(false);
                }
                else
                {
                    component.set_text(Singleton<CTextManager>.instance.GetText("Friend_Rec_Bei_Role_HasData"));
                    obj3.CustomSetActive(false);
                    obj4.CustomSetActive(true);
                    text4.get_gameObject().CustomSetActive(true);
                    UT.SetHttpImage(obj2.get_transform().FindChild("pnlSnsHead/HttpImage").GetComponent<CUIHttpImageScript>(), beiZhaoMuZhe.userInfo.szHeadUrl);
                    text4.set_text(string.Format("Lv.{0}", beiZhaoMuZhe.userInfo.dwPvpLvl));
                    GameObject obj5 = obj2.get_transform().FindChild("pnlSnsHead/HttpImage/NobeIcon").get_gameObject();
                    if (obj5 != null)
                    {
                        MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(obj5.GetComponent<Image>(), (int) beiZhaoMuZhe.userInfo.stGameVip.dwCurLevel, false);
                    }
                    Text text5 = obj2.get_transform().FindChild("NameGroup/Name").GetComponent<Text>();
                    string str = UT.Bytes2String(beiZhaoMuZhe.userInfo.szUserName);
                    if (text5 != null)
                    {
                        text5.set_text(str);
                    }
                    FriendShower.ShowGender(obj2.get_transform().FindChild("NameGroup/Gender").get_gameObject(), (COM_SNSGENDER) beiZhaoMuZhe.userInfo.bGender);
                }
                CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CFriendContoller.FriendFormPath);
                GameObject obj7 = this.beizhaomu_content.get_transform().FindChild("info/reward").get_gameObject();
                CFriendRecruit.RecruitData data2 = friendRecruit.GetBeiZhaoMuZhe();
                ulong ullUid = data2.ullUid;
                uint dwLogicWorldId = data2.dwLogicWorldId;
                int num3 = Math.Min(4, data2.RewardList.Count);
                for (int i = 0; i < num3; i++)
                {
                    CFriendRecruit.RecruitReward reward = data2.RewardList[i];
                    GameObject node = obj7.get_transform().FindChild(string.Format("reward_{0}", i)).get_gameObject();
                    this.Show_Award(node, ullUid, dwLogicWorldId, COM_RECRUITMENT_TYPE.COM_RECRUITMENT_PASSIVE, reward.rewardID, reward.state, form, true);
                }
            }
        }

        private void Show_ZhouMoZhe_Reward()
        {
            CFriendRecruit friendRecruit = Singleton<CFriendContoller>.instance.model.friendRecruit;
            if (this.zm_benifit_exp != null)
            {
                this.zm_benifit_exp.set_text(string.Format("+{0}%", friendRecruit.GetZhaoMuZhe_RewardExp()));
            }
            if (this.zm_benifit_gold != null)
            {
                this.zm_benifit_gold.set_text(string.Format("+{0}%", friendRecruit.GetZhaoMuZhe_RewardGold()));
            }
            int num = friendRecruit.GetZhaoMuZhe_RewardProgress();
            int num2 = friendRecruit.GetZhaoMuZhe_RewardTotalCount();
            string[] args = new string[] { num.ToString(), num2.ToString() };
            string text = Singleton<CTextManager>.instance.GetText("Friend_Rec_zmz_whole_Progress", args);
            if (this.zm_totalProgress != null)
            {
                this.zm_totalProgress.set_text(text);
            }
            for (int i = 0; i < num; i++)
            {
                GameObject node = this.zm_progressNode.get_transform().FindChild(string.Format("icon_{0}", i)).get_gameObject();
                this.ShowBar(node, true);
            }
            for (int j = num; j < num2; j++)
            {
                GameObject obj3 = this.zm_progressNode.get_transform().FindChild(string.Format("icon_{0}", j)).get_gameObject();
                this.ShowBar(obj3, false);
            }
            if (this.supberRewardNodeTxt != null)
            {
                string[] textArray2 = new string[] { zmzCount.ToString(), zmzLevel.ToString() };
                this.supberRewardNodeTxt.set_text(Singleton<CTextManager>.instance.GetText("Friend_Rec_zmz_Progress", textArray2));
            }
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CFriendContoller.FriendFormPath);
            this.Show_Award(this.supberRewardNode, 0L, 0, COM_RECRUITMENT_TYPE.COM_RECRUITMENT_ACTIVE, friendRecruit.SuperReward.rewardID, friendRecruit.SuperReward.state, form, false);
            this.Refresh_ZhaomuZhe_List();
            this.zhaomu_content.CustomSetActive(true);
            this.beizhaomu_content.CustomSetActive(false);
        }

        private void ShowBar(GameObject node, bool bOutterShow)
        {
            if (node != null)
            {
                node.CustomSetActive(true);
                Transform transform = node.get_transform().FindChild("outer");
                if (transform != null)
                {
                    transform.get_gameObject().CustomSetActive(bOutterShow);
                }
            }
        }

        public void ShowNode(bool bZhaomuzhe)
        {
            if (bZhaomuzhe)
            {
                this.Show_ZhouMoZhe_Reward();
            }
            else
            {
                this.Show_BeiZhouMoZhe_Reward();
            }
        }

        public void ShowZhaomuZhe_Item(GameObject com, CFriendRecruit.RecruitData info)
        {
            if (info.userInfo != null)
            {
                com.get_transform().FindChild("user/hasData").get_gameObject().CustomSetActive(true);
                com.get_transform().FindChild("user/null").get_gameObject().CustomSetActive(false);
                com.get_transform().FindChild("user/hasData/Level").get_gameObject().CustomSetActive(true);
                UT.SetHttpImage(com.get_transform().FindChild("user/hasData/pnlSnsHead/HttpImage").GetComponent<CUIHttpImageScript>(), info.userInfo.szHeadUrl);
                com.get_transform().FindChild("user/hasData/Level").GetComponent<Text>().set_text(string.Format("Lv.{0}", info.userInfo.dwPvpLvl));
                GameObject obj2 = com.get_transform().FindChild("user/hasData/pnlSnsHead/HttpImage/NobeIcon").get_gameObject();
                if (obj2 != null)
                {
                    MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(obj2.GetComponent<Image>(), (int) info.userInfo.stGameVip.dwCurLevel, false);
                }
                Text component = com.get_transform().FindChild("user/hasData/NameGroup/Name").GetComponent<Text>();
                string str = UT.Bytes2String(info.userInfo.szUserName);
                if (component != null)
                {
                    component.set_text(str);
                }
                FriendShower.ShowGender(com.get_transform().FindChild("user/hasData/NameGroup/Gender").get_gameObject(), (COM_SNSGENDER) info.userInfo.bGender);
            }
            else
            {
                com.get_transform().FindChild("user/hasData").get_gameObject().CustomSetActive(false);
                com.get_transform().FindChild("user/null").get_gameObject().CustomSetActive(true);
            }
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CFriendContoller.FriendFormPath);
            int num = Math.Min(this.zhaomuzheRewardCount, info.RewardList.Count);
            for (int i = 0; i < num; i++)
            {
                Transform transform = com.get_transform().FindChild(string.Format("reward_{0}", i));
                DebugHelper.Assert(transform != null, "rewardNodeTS not null...");
                if (transform != null)
                {
                    CFriendRecruit.RecruitReward reward = info.RewardList[i];
                    this.Show_Award(transform.get_gameObject(), info.ullUid, info.dwLogicWorldId, COM_RECRUITMENT_TYPE.COM_RECRUITMENT_ACTIVE, reward.rewardID, reward.state, form, true);
                }
            }
        }

        public enum enRecruitWidget
        {
            Gender = 3,
            HttpImage = 0,
            Name = 2,
            NobeIcon = 1,
            None = -1
        }

        public enum Tab
        {
            None,
            Zhaomu_Reward,
            BeiZhaomu_Reward
        }
    }
}

