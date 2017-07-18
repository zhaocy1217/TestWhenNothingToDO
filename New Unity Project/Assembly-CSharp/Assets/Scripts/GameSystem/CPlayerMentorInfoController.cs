namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [MessageHandlerClass]
    public class CPlayerMentorInfoController : Singleton<CPlayerMentorInfoController>
    {
        public int m_addViewtype = 3;
        public ResFamousMentor m_famousMentorData;
        private string mentorStateStr;

        private void FamousMentorInVisitor(ResFamousMentor resFmsMentor)
        {
            CPlayerProfile profile = Singleton<CPlayerInfoSystem>.instance.GetProfile();
            if (profile != null)
            {
                if (this.m_famousMentorData == null)
                {
                    this.m_famousMentorData = resFmsMentor;
                }
                else if ((this.m_famousMentorData.dwPoint < profile._mentorInfo.dwMasterPoint) && (resFmsMentor.dwPoint <= profile._mentorInfo.dwMasterPoint))
                {
                    this.m_famousMentorData = resFmsMentor;
                }
            }
        }

        public override void Init()
        {
            Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.Mentor_WatchHisMentor, new CUIEventManager.OnUIEventHandler(this.OnWatchHisMentor));
            Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.Mentor_OpenMentorPage, new CUIEventManager.OnUIEventHandler(this.OnOpenMentorPage));
            Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.Mentor_ApplyRequest, new CUIEventManager.OnUIEventHandler(this.OnApplyRequest));
            Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.Mentor_OpenMentorIntro, new CUIEventManager.OnUIEventHandler(this.OnOpenMentorIntro));
        }

        private void OnApplyRequest(CUIEvent uievt)
        {
            if (Singleton<CPlayerInfoSystem>.instance.GetProfile() != null)
            {
                string stringPlacer = null;
                string title = null;
                if (this.mentorStateStr != null)
                {
                    string[] args = new string[] { this.mentorStateStr };
                    title = Singleton<CTextManager>.GetInstance().GetText("Mentor_VerifyReqTitle", args);
                    string[] textArray2 = new string[] { this.mentorStateStr };
                    stringPlacer = Singleton<CTextManager>.GetInstance().GetText("Mentor_VerifyReqReplacer", textArray2);
                }
                Singleton<CUIManager>.GetInstance().OpenStringSenderBox(title, Singleton<CTextManager>.GetInstance().GetText("Mentor_VerifyReqDesc"), stringPlacer, new StringSendboxOnSend(this.OnMentorApplyVerifyBoxRetrun), CFriendView.Verfication.GetRandomMentorReuqestStr(-1));
            }
        }

        public void OnMentorApplyVerifyBoxRetrun(string str)
        {
            CPlayerProfile profile = Singleton<CPlayerInfoSystem>.instance.GetProfile();
            if (profile != null)
            {
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x151a);
                msg.stPkgData.stApplyMasterReq.bType = (byte) this.m_addViewtype;
                msg.stPkgData.stApplyMasterReq.stUin.dwLogicWorldId = (uint) profile.m_iLogicWorldId;
                msg.stPkgData.stApplyMasterReq.stUin.ullUid = profile.m_uuid;
                StringHelper.StringToUTF8Bytes(str, ref msg.stPkgData.stApplyMasterReq.szVerificationInfo);
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
            }
        }

        private void OnOpenMentorIntro(CUIEvent uievt)
        {
            Singleton<CUIManager>.GetInstance().OpenInfoForm(0x15);
        }

        private void OnOpenMentorPage(CUIEvent uievt)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CFriendContoller.FriendFormPath);
            if (form == null)
            {
                Singleton<CFriendContoller>.GetInstance().view.OpenForm(uievt);
                form = Singleton<CUIManager>.GetInstance().GetForm(CFriendContoller.FriendFormPath);
            }
            if (form != null)
            {
                form.get_transform().Find("TopCommon/Panel_Menu/List").get_gameObject().GetComponent<CUIListScript>().SelectElement(Singleton<CFriendContoller>.GetInstance().view.tabMgr.GetIndex(CFriendView.Tab.Mentor), true);
                uievt.m_srcFormScript.Close();
                Singleton<CUIManager>.GetInstance().CloseForm(RankingSystem.s_rankingForm);
            }
        }

        private void OnWatchHisMentor(CUIEvent uievt)
        {
            CPlayerProfile profile = Singleton<CPlayerInfoSystem>.instance.GetProfile();
            if (profile != null)
            {
                Singleton<CPlayerInfoSystem>.GetInstance().ShowPlayerDetailInfo(profile._mentorInfo.ullMasterUid, (int) profile._mentorInfo.dwMasterLogicWorldID, CPlayerInfoSystem.DetailPlayerInfoSource.DefaultOthers, true);
            }
        }

        private void SetTempNoData(CUIFormScript form, string mentorName)
        {
            GameObject widget = form.GetWidget(0x15);
            GameObject obj3 = form.GetWidget(0x12);
            GameObject obj4 = form.GetWidget(0x16);
            GameObject obj5 = form.GetWidget(0x17);
            GameObject obj6 = form.GetWidget(0x18);
            GameObject obj7 = form.GetWidget(0x1c);
            GameObject obj8 = form.GetWidget(0x1b);
            GameObject obj9 = form.GetWidget(0x19);
            GameObject obj10 = form.GetWidget(0x1a);
            GameObject obj11 = form.GetWidget(0x1d);
            GameObject obj12 = form.GetWidget(30);
            obj11.CustomSetActive(false);
            obj12.CustomSetActive(true);
            obj8.CustomSetActive(false);
            obj9.CustomSetActive(false);
            obj10.CustomSetActive(false);
            Text component = obj3.GetComponent<Text>();
            string text = Singleton<CTextManager>.GetInstance().GetText("Common_NoData");
            string[] args = new string[] { text, text, text, mentorName };
            component.set_text(Singleton<CTextManager>.GetInstance().GetText("Mentor_PlayerInfo", args));
            obj7.CustomSetActive(false);
        }

        public override void UnInit()
        {
            Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.Mentor_WatchHisMentor, new CUIEventManager.OnUIEventHandler(this.OnWatchHisMentor));
            Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.Mentor_OpenMentorPage, new CUIEventManager.OnUIEventHandler(this.OnOpenMentorPage));
            Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.Mentor_ApplyRequest, new CUIEventManager.OnUIEventHandler(this.OnApplyRequest));
            Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.Mentor_OpenMentorIntro, new CUIEventManager.OnUIEventHandler(this.OnOpenMentorIntro));
        }

        public void UpdateUI()
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CPlayerInfoSystem.sPlayerInfoFormPath);
            if (form == null)
            {
                return;
            }
            CPlayerProfile profile = Singleton<CPlayerInfoSystem>.instance.GetProfile();
            if (profile == null)
            {
                return;
            }
            this.m_famousMentorData = null;
            GameObject widget = form.GetWidget(0x15);
            GameObject obj3 = form.GetWidget(0x12);
            GameObject obj4 = form.GetWidget(0x13);
            GameObject obj5 = form.GetWidget(20);
            GameObject obj6 = form.GetWidget(0x16);
            GameObject obj7 = form.GetWidget(0x17);
            GameObject obj8 = form.GetWidget(0x18);
            GameObject obj9 = form.GetWidget(0x19);
            GameObject obj10 = form.GetWidget(0x1a);
            GameObject obj11 = form.GetWidget(0x1b);
            GameObject obj12 = form.GetWidget(0x1c);
            GameObject obj13 = form.GetWidget(0x1d);
            GameObject obj14 = form.GetWidget(30);
            obj13.CustomSetActive(true);
            obj14.CustomSetActive(false);
            string text = Singleton<CTextManager>.GetInstance().GetText("Common_NoData");
            obj7.CustomSetActive(!profile.isMasterData);
            obj8.CustomSetActive(profile._mentorInfo.ullMasterUid == 0L);
            obj6.CustomSetActive(profile._mentorInfo.ullMasterUid != 0L);
            enMentorState mentorState = CFriendContoller.GetMentorState(profile.PvpLevel(), null);
            if (!profile.isMasterData)
            {
                switch (mentorState)
                {
                    case enMentorState.IWantMentor:
                    case enMentorState.IHasMentor:
                        obj7.get_transform().Find("Text").GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("Mentor_ProposeApprentice"));
                        goto Label_01BC;

                    case enMentorState.IWantApprentice:
                    case enMentorState.IHasApprentice:
                        obj7.get_transform().Find("Text").GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("Mentor_ProposeMentor"));
                        goto Label_01BC;
                }
                obj7.CustomSetActive(false);
            }
        Label_01BC:
            if (profile._mentorInfo.dwStudentNum != 0)
            {
                obj4.GetComponent<Text>().set_text(profile._mentorInfo.dwStudentNum.ToString());
            }
            else
            {
                obj4.GetComponent<Text>().set_text(text);
            }
            if (profile._mentorInfo.dwFinishStudentNum != 0)
            {
                obj5.GetComponent<Text>().set_text(profile._mentorInfo.dwFinishStudentNum.ToString());
            }
            else
            {
                obj5.GetComponent<Text>().set_text(text);
            }
            string str2 = Utility.UTF8Convert(profile._mentorInfo.szRoleName);
            if (string.IsNullOrEmpty(str2))
            {
                str2 = text;
            }
            if (((mentorState == enMentorState.IWantMentor) || (mentorState == enMentorState.IHasMentor)) || (profile._mentorInfo.dwMasterPoint == 0))
            {
                this.SetTempNoData(form, str2);
            }
            else
            {
                obj11.CustomSetActive(true);
                obj9.CustomSetActive(true);
                obj10.CustomSetActive(true);
                GameDataMgr.famousMentorDatabin.Accept(new Action<ResFamousMentor>(this.FamousMentorInVisitor));
                if (this.m_famousMentorData == null)
                {
                    this.SetTempNoData(form, str2);
                    return;
                }
                obj12.CustomSetActive(true);
                obj9.GetComponent<Text>().set_text(this.m_famousMentorData.szTitle);
                string prefabPath = string.Format("{0}{1}.prefab", CUIUtility.s_Sprite_System_ShareUI_Dir, this.m_famousMentorData.iLevel.ToString());
                CUIUtility.SetImageSprite(obj12.GetComponent<Image>(), prefabPath, null, true, false, false, false);
                string[] args = new string[] { this.m_famousMentorData.iLevel.ToString(), this.m_famousMentorData.szTitle, profile._mentorInfo.dwMasterPoint.ToString(), str2 };
                obj3.GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("Mentor_PlayerInfo", args));
                ResFamousMentor dataByKey = GameDataMgr.famousMentorDatabin.GetDataByKey((uint) (this.m_famousMentorData.dwID + 1));
                if (dataByKey == null)
                {
                    obj10.GetComponent<Text>().set_text(profile._mentorInfo.dwMasterPoint + string.Empty);
                }
                else
                {
                    obj10.GetComponent<Text>().set_text(profile._mentorInfo.dwMasterPoint + "/" + dataByKey.dwPoint);
                }
                uint dwPoint = 0;
                if (this.m_famousMentorData.dwID != 1)
                {
                    ResFamousMentor mentor2 = GameDataMgr.famousMentorDatabin.GetDataByKey((uint) (this.m_famousMentorData.dwID - 1));
                    if (mentor2 != null)
                    {
                        dwPoint = mentor2.dwPoint;
                    }
                }
                if (this.m_famousMentorData.dwPoint == dwPoint)
                {
                    obj11.GetComponent<Image>().set_fillAmount(0f);
                }
                else
                {
                    obj11.GetComponent<Image>().set_fillAmount(((float) (profile._mentorInfo.dwMasterPoint - dwPoint)) / ((float) (this.m_famousMentorData.dwPoint - dwPoint)));
                }
            }
            this.mentorStateStr = null;
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            enMentorState state2 = CFriendContoller.GetMentorState(profile.PvpLevel(), null);
            enMentorState state3 = CFriendContoller.GetMentorState(masterRoleInfo.PvpLevel, null);
            if (((state2 == enMentorState.IHasApprentice) || (state2 == enMentorState.IWantApprentice)) && ((state3 == enMentorState.IHasMentor) || (state3 == enMentorState.IWantMentor)))
            {
                this.mentorStateStr = Singleton<CTextManager>.GetInstance().GetText("Mentor_GetMentor");
                this.m_addViewtype = 2;
                if ((masterRoleInfo.m_mentorInfo != null) && (masterRoleInfo.m_mentorInfo.ullMasterUid != 0))
                {
                    this.mentorStateStr = null;
                }
            }
            else if (((state3 == enMentorState.IHasApprentice) || (state3 == enMentorState.IWantApprentice)) && ((state2 == enMentorState.IHasMentor) || (state2 == enMentorState.IWantMentor)))
            {
                this.mentorStateStr = Singleton<CTextManager>.GetInstance().GetText("Mentor_GetApprentice");
                this.m_addViewtype = 3;
            }
            obj7.CustomSetActive(this.mentorStateStr != null);
        }
    }
}

