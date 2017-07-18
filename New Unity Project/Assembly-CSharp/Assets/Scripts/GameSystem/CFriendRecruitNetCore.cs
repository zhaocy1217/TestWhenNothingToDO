namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using CSProtocol;
    using ResData;
    using System;

    [MessageHandlerClass]
    public class CFriendRecruitNetCore
    {
        [MessageHandler(0x55e)]
        public static void On_CHG_RECRUITMENT_NTF(CSPkg msg)
        {
            SCPKG_CHG_RECRUITMENT_NTF stChgRecruitmentNtf = msg.stPkgData.stChgRecruitmentNtf;
            CFriendRecruit friendRecruit = Singleton<CFriendContoller>.instance.model.friendRecruit;
            if (stChgRecruitmentNtf.bChgRecruitmentType == 1)
            {
                COMDT_FRIEND_INFO info = Singleton<CFriendContoller>.instance.model.GetInfo(CFriendModel.FriendType.GameFriend, stChgRecruitmentNtf.stUin.ullUid, stChgRecruitmentNtf.stUin.dwLogicWorldId);
                if (info != null)
                {
                    if (stChgRecruitmentNtf.bChgRecruitmentType == 1)
                    {
                        friendRecruit.SetZhaoMuZhe(info);
                    }
                    if (stChgRecruitmentNtf.bChgRecruitmentType == 2)
                    {
                        friendRecruit.SetBeiZhaoMuZheRewardData(info);
                    }
                }
            }
            else if (stChgRecruitmentNtf.bChgRecruitmentType == 2)
            {
                friendRecruit.RemoveRecruitData(stChgRecruitmentNtf.stUin.ullUid, stChgRecruitmentNtf.stUin.dwLogicWorldId);
            }
        }

        [MessageHandler(0x55d)]
        public static void On_RECRUITMENT_ERR_NTF(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            SCPKG_RECRUITMENT_ERR_NTF stRecruitmentErrNtf = msg.stPkgData.stRecruitmentErrNtf;
            if (stRecruitmentErrNtf.dwErrorCode != 0)
            {
                Singleton<CUIManager>.GetInstance().OpenTips(UT.ErrorCode_String(stRecruitmentErrNtf.dwErrorCode), false, 1.5f, null, new object[0]);
            }
        }

        [MessageHandler(0x55c)]
        public static void On_RECRUITMENT_REWARD_ERR_NTF(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            SCPKG_RECRUITMENT_REWARD_ERR_NTF stRecruitmentRewardErrNtf = msg.stPkgData.stRecruitmentRewardErrNtf;
            string strContent = string.Empty;
            switch (stRecruitmentRewardErrNtf.dwErrorCode)
            {
                case 1:
                    strContent = Singleton<CTextManager>.instance.GetText("CS_RECRUITMENTWARD_NOTINTABLE");
                    break;

                case 2:
                    strContent = Singleton<CTextManager>.instance.GetText("CS_RECRUITMENTWARD_LEVEL_NOTENOUGH");
                    break;

                case 3:
                    strContent = Singleton<CTextManager>.instance.GetText("CS_RECRUITMENTWARD_NOTRECRUITMENT");
                    break;

                case 4:
                    strContent = Singleton<CTextManager>.instance.GetText("CS_RECRUITMENTWARD_GETED");
                    break;

                case 5:
                    strContent = Singleton<CTextManager>.instance.GetText("CS_RECRUITMENTWARD_OTHER");
                    break;

                case 6:
                    strContent = Singleton<CTextManager>.instance.GetText("CS_RECRUITMENTWARD_SUPER");
                    break;

                default:
                    strContent = string.Format("no RECRUITMENT REWARD ERR code:{0}", stRecruitmentRewardErrNtf.dwErrorCode);
                    break;
            }
            Singleton<CUIManager>.GetInstance().OpenTips(strContent, false, 1.5f, null, new object[0]);
        }

        [MessageHandler(0x55b)]
        public static void On_Send_INTIMACY_RELATION_REQUEST(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            SCPKG_RECRUITMENT_REWARD_RSP stRecruitmentRewardRsp = msg.stPkgData.stRecruitmentRewardRsp;
            CFriendRecruit friendRecruit = Singleton<CFriendContoller>.instance.model.friendRecruit;
            ResRecruitmentReward cfgReward = friendRecruit.GetCfgReward(stRecruitmentRewardRsp.wRecruitRewardId);
            if (cfgReward.bRewardType == 2)
            {
                friendRecruit.SetBITS((RES_RECRUIMENT_BITS) cfgReward.bRewardBit, true);
            }
            friendRecruit.GetRecruitData(stRecruitmentRewardRsp.stUin.ullUid, stRecruitmentRewardRsp.stUin.dwLogicWorldId).SetReward(stRecruitmentRewardRsp.wRecruitRewardId, CFriendRecruit.RewardState.Getted);
            if (friendRecruit.SuperReward.rewardID == stRecruitmentRewardRsp.wRecruitRewardId)
            {
                friendRecruit.SuperReward.state = CFriendRecruit.RewardState.Getted;
            }
        }

        public static void Send_INTIMACY_RELATION_REQUEST(ulong ulluid, uint worldID, ushort rewardID)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x55a);
            msg.stPkgData.stRecruitmentRewardReq.stUin.ullUid = ulluid;
            msg.stPkgData.stRecruitmentRewardReq.stUin.dwLogicWorldId = worldID;
            msg.stPkgData.stRecruitmentRewardReq.wRecruitRewardId = rewardID;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }
    }
}

