namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using CSProtocol;
    using System;

    [MessageHandlerClass]
    public class FriendRelationNetCore
    {
        [MessageHandler(0x555)]
        public static void On_NTF_CHG_INTIMACY_CONFIRM(CSPkg msg)
        {
            SCPKG_CMD_NTF_CHG_INTIMACY_CONFIRM stNtfChgIntimacyConfirm = msg.stPkgData.stNtfChgIntimacyConfirm;
            if (stNtfChgIntimacyConfirm != null)
            {
                if (stNtfChgIntimacyConfirm.bRelationChgType == 1)
                {
                    if (stNtfChgIntimacyConfirm.bIntimacyState == 1)
                    {
                        CFriendRelationship.FRData.Add(stNtfChgIntimacyConfirm.stUin.ullUid, stNtfChgIntimacyConfirm.stUin.dwLogicWorldId, COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY, (COM_INTIMACY_RELATION_CHG_TYPE) stNtfChgIntimacyConfirm.bRelationChgType, 0, false);
                    }
                    if (stNtfChgIntimacyConfirm.bIntimacyState == 2)
                    {
                        CFriendRelationship.FRData.Add(stNtfChgIntimacyConfirm.stUin.ullUid, stNtfChgIntimacyConfirm.stUin.dwLogicWorldId, COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER, (COM_INTIMACY_RELATION_CHG_TYPE) stNtfChgIntimacyConfirm.bRelationChgType, 0, false);
                    }
                }
                if (stNtfChgIntimacyConfirm.bRelationChgType == 2)
                {
                    if (stNtfChgIntimacyConfirm.bIntimacyState == 1)
                    {
                        CFriendRelationship.FRData.Add(stNtfChgIntimacyConfirm.stUin.ullUid, stNtfChgIntimacyConfirm.stUin.dwLogicWorldId, COM_INTIMACY_STATE.COM_INTIMACY_STATE_VALUE_FULL, (COM_INTIMACY_RELATION_CHG_TYPE) stNtfChgIntimacyConfirm.bRelationChgType, stNtfChgIntimacyConfirm.dwTerminateTime, false);
                    }
                    if (stNtfChgIntimacyConfirm.bIntimacyState == 2)
                    {
                        CFriendRelationship.FRData.Add(stNtfChgIntimacyConfirm.stUin.ullUid, stNtfChgIntimacyConfirm.stUin.dwLogicWorldId, COM_INTIMACY_STATE.COM_INTIMACY_STATE_VALUE_FULL, (COM_INTIMACY_RELATION_CHG_TYPE) stNtfChgIntimacyConfirm.bRelationChgType, stNtfChgIntimacyConfirm.dwTerminateTime, false);
                    }
                }
            }
        }

        [MessageHandler(0x558)]
        public static void On_NTF_CHG_INTIMACY_DENY(CSPkg msg)
        {
            SCPKG_CMD_NTF_CHG_INTIMACY_DENY stNtfChgIntimacyDeny = msg.stPkgData.stNtfChgIntimacyDeny;
            if (stNtfChgIntimacyDeny != null)
            {
                COMDT_FRIEND_INFO gameOrSnsFriend = Singleton<CFriendContoller>.instance.model.GetGameOrSnsFriend(stNtfChgIntimacyDeny.stUin.ullUid, stNtfChgIntimacyDeny.stUin.dwLogicWorldId);
                string strContent = string.Empty;
                if (stNtfChgIntimacyDeny.bRelationChgType == 1)
                {
                    if (stNtfChgIntimacyDeny.bIntimacyState == 1)
                    {
                        if (gameOrSnsFriend != null)
                        {
                            strContent = string.Format(UT.FRData().IntimRela_Tips_DenyYourRequestGay, UT.Bytes2String(gameOrSnsFriend.szUserName));
                        }
                        CFriendRelationship.FRData.Add(stNtfChgIntimacyDeny.stUin.ullUid, stNtfChgIntimacyDeny.stUin.dwLogicWorldId, COM_INTIMACY_STATE.COM_INTIMACY_STATE_VALUE_FULL, COM_INTIMACY_RELATION_CHG_TYPE.COM_INTIMACY_RELATION_NULL, 0, false);
                    }
                    if (stNtfChgIntimacyDeny.bIntimacyState == 2)
                    {
                        if (gameOrSnsFriend != null)
                        {
                            strContent = string.Format(UT.FRData().IntimRela_Tips_DenyYourRequestLover, UT.Bytes2String(gameOrSnsFriend.szUserName));
                        }
                        CFriendRelationship.FRData.Add(stNtfChgIntimacyDeny.stUin.ullUid, stNtfChgIntimacyDeny.stUin.dwLogicWorldId, COM_INTIMACY_STATE.COM_INTIMACY_STATE_VALUE_FULL, COM_INTIMACY_RELATION_CHG_TYPE.COM_INTIMACY_RELATION_NULL, 0, false);
                    }
                }
                if (stNtfChgIntimacyDeny.bRelationChgType == 2)
                {
                    if (stNtfChgIntimacyDeny.bIntimacyState == 1)
                    {
                        if (gameOrSnsFriend != null)
                        {
                            strContent = string.Format(UT.FRData().IntimRela_Tips_DenyYourDelGay, UT.Bytes2String(gameOrSnsFriend.szUserName));
                        }
                        CFriendRelationship.FRData.Add(stNtfChgIntimacyDeny.stUin.ullUid, stNtfChgIntimacyDeny.stUin.dwLogicWorldId, COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY, COM_INTIMACY_RELATION_CHG_TYPE.COM_INTIMACY_RELATION_NULL, 0, false);
                    }
                    if (stNtfChgIntimacyDeny.bIntimacyState == 2)
                    {
                        if (gameOrSnsFriend != null)
                        {
                            strContent = string.Format(UT.FRData().IntimRela_Tips_DenyYourDelLover, UT.Bytes2String(gameOrSnsFriend.szUserName));
                        }
                        CFriendRelationship.FRData.Add(stNtfChgIntimacyDeny.stUin.ullUid, stNtfChgIntimacyDeny.stUin.dwLogicWorldId, COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER, COM_INTIMACY_RELATION_CHG_TYPE.COM_INTIMACY_RELATION_NULL, 0, false);
                    }
                }
                Singleton<CUIManager>.GetInstance().OpenTips(strContent, false, 1.5f, null, new object[0]);
            }
        }

        [MessageHandler(0x552)]
        public static void On_NTF_INTIMACY_RELATION_REQUEST(CSPkg msg)
        {
            SCPKG_CMD_NTF_INTIMACY_RELATION_REQUEST stNtfIntimacyRelationRequest = msg.stPkgData.stNtfIntimacyRelationRequest;
            if (stNtfIntimacyRelationRequest != null)
            {
                COM_INTIMACY_STATE state = COM_INTIMACY_STATE.COM_INTIMACY_STATE_NULL;
                if (stNtfIntimacyRelationRequest.bRelationChgType == 1)
                {
                    if (stNtfIntimacyRelationRequest.bIntimacyState == 1)
                    {
                        state = COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY_CONFIRM;
                    }
                    else if (stNtfIntimacyRelationRequest.bIntimacyState == 2)
                    {
                        state = COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER_CONFIRM;
                    }
                }
                else if (stNtfIntimacyRelationRequest.bRelationChgType == 2)
                {
                    if (stNtfIntimacyRelationRequest.bIntimacyState == 1)
                    {
                        state = COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY_DENY;
                    }
                    else if (stNtfIntimacyRelationRequest.bIntimacyState == 2)
                    {
                        state = COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER_DENY;
                    }
                }
                CFriendRelationship.FRData.Add(stNtfIntimacyRelationRequest.stUin.ullUid, stNtfIntimacyRelationRequest.stUin.dwLogicWorldId, state, (COM_INTIMACY_RELATION_CHG_TYPE) stNtfIntimacyRelationRequest.bRelationChgType, 0, true);
            }
        }

        [MessageHandler(0x554)]
        public static void On_Send_CHG_INTIMACY_CONFIRM(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            SCPKG_CMD_CHG_INTIMACY_CONFIRM stChgIntimacyConfirmRsp = msg.stPkgData.stChgIntimacyConfirmRsp;
            if (stChgIntimacyConfirmRsp.dwResult == 0)
            {
                if (stChgIntimacyConfirmRsp.bRelationChgType == 1)
                {
                    if (stChgIntimacyConfirmRsp.bIntimacyState == 1)
                    {
                        CFriendRelationship.FRData.Add(stChgIntimacyConfirmRsp.stUin.ullUid, stChgIntimacyConfirmRsp.stUin.dwLogicWorldId, COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY, (COM_INTIMACY_RELATION_CHG_TYPE) stChgIntimacyConfirmRsp.bRelationChgType, 0, false);
                    }
                    if (stChgIntimacyConfirmRsp.bIntimacyState == 2)
                    {
                        CFriendRelationship.FRData.Add(stChgIntimacyConfirmRsp.stUin.ullUid, stChgIntimacyConfirmRsp.stUin.dwLogicWorldId, COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER, (COM_INTIMACY_RELATION_CHG_TYPE) stChgIntimacyConfirmRsp.bRelationChgType, 0, false);
                    }
                }
                if (stChgIntimacyConfirmRsp.bRelationChgType == 2)
                {
                    if (stChgIntimacyConfirmRsp.bIntimacyState == 1)
                    {
                        CFriendRelationship.FRData.Add(stChgIntimacyConfirmRsp.stUin.ullUid, stChgIntimacyConfirmRsp.stUin.dwLogicWorldId, COM_INTIMACY_STATE.COM_INTIMACY_STATE_VALUE_FULL, (COM_INTIMACY_RELATION_CHG_TYPE) stChgIntimacyConfirmRsp.bRelationChgType, stChgIntimacyConfirmRsp.dwTerminateTime, false);
                    }
                    if (stChgIntimacyConfirmRsp.bIntimacyState == 2)
                    {
                        CFriendRelationship.FRData.Add(stChgIntimacyConfirmRsp.stUin.ullUid, stChgIntimacyConfirmRsp.stUin.dwLogicWorldId, COM_INTIMACY_STATE.COM_INTIMACY_STATE_VALUE_FULL, (COM_INTIMACY_RELATION_CHG_TYPE) stChgIntimacyConfirmRsp.bRelationChgType, stChgIntimacyConfirmRsp.dwTerminateTime, false);
                    }
                }
            }
            else
            {
                Singleton<CUIManager>.GetInstance().OpenTips(UT.ErrorCode_String(stChgIntimacyConfirmRsp.dwResult), false, 1.5f, null, new object[0]);
            }
        }

        [MessageHandler(0x557)]
        public static void On_Send_CHG_INTIMACY_DENY(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            SCPKG_CMD_CHG_INTIMACY_DENY stChgIntimacyDenyRsp = msg.stPkgData.stChgIntimacyDenyRsp;
            if (stChgIntimacyDenyRsp.dwResult == 0)
            {
            }
            else
            {
                Singleton<CUIManager>.GetInstance().OpenTips(UT.ErrorCode_String(stChgIntimacyDenyRsp.dwResult), false, 1.5f, null, new object[0]);
            }
        }

        [MessageHandler(0x551)]
        public static void On_Send_INTIMACY_RELATION_REQUEST(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            SCPKG_CMD_INTIMACY_RELATION_REQUEST stIntimacyRelationRequestRsp = msg.stPkgData.stIntimacyRelationRequestRsp;
            if (stIntimacyRelationRequestRsp.dwResult == 0)
            {
                string strContent = string.Empty;
                COM_INTIMACY_STATE state = COM_INTIMACY_STATE.COM_INTIMACY_STATE_NULL;
                if (stIntimacyRelationRequestRsp.bRelationChgType == 1)
                {
                    if (stIntimacyRelationRequestRsp.bIntimacyState == 1)
                    {
                        state = COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY_CONFIRM;
                        strContent = UT.FRData().IntimRela_Tips_SendRequestGaySuccess;
                    }
                    else if (stIntimacyRelationRequestRsp.bIntimacyState == 2)
                    {
                        state = COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER_CONFIRM;
                        strContent = UT.FRData().IntimRela_Tips_SendRequestLoverSuccess;
                    }
                }
                else if (stIntimacyRelationRequestRsp.bRelationChgType == 2)
                {
                    if (stIntimacyRelationRequestRsp.bIntimacyState == 1)
                    {
                        state = COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY_DENY;
                        strContent = UT.FRData().IntimRela_Tips_SendDelGaySuccess;
                    }
                    else if (stIntimacyRelationRequestRsp.bIntimacyState == 2)
                    {
                        state = COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER_DENY;
                        strContent = UT.FRData().IntimRela_Tips_SendDelLoverSuccess;
                    }
                }
                Singleton<CUIManager>.GetInstance().OpenTips(strContent, false, 1.5f, null, new object[0]);
                CFriendRelationship.FRData.Add(stIntimacyRelationRequestRsp.stUin.ullUid, stIntimacyRelationRequestRsp.stUin.dwLogicWorldId, state, (COM_INTIMACY_RELATION_CHG_TYPE) stIntimacyRelationRequestRsp.bRelationChgType, 0, false);
            }
            else
            {
                Singleton<CUIManager>.GetInstance().OpenTips(UT.ErrorCode_String(stIntimacyRelationRequestRsp.dwResult), false, 1.5f, null, new object[0]);
            }
        }

        public static void Send_CHG_INTIMACY_CONFIRM(ulong ulluid, uint worldID, COM_INTIMACY_STATE value, COM_INTIMACY_RELATION_CHG_TYPE type)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x553);
            msg.stPkgData.stChgIntimacyConfirmReq.stUin.ullUid = ulluid;
            msg.stPkgData.stChgIntimacyConfirmReq.stUin.dwLogicWorldId = worldID;
            msg.stPkgData.stChgIntimacyConfirmReq.bRelationChgType = (byte) type;
            msg.stPkgData.stChgIntimacyConfirmReq.bIntimacyState = (byte) value;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public static void Send_CHG_INTIMACY_DENY(ulong ulluid, uint worldID, COM_INTIMACY_STATE state, COM_INTIMACY_RELATION_CHG_TYPE type)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x556);
            msg.stPkgData.stChgIntimacyDenyReq.stUin.ullUid = ulluid;
            msg.stPkgData.stChgIntimacyDenyReq.stUin.dwLogicWorldId = worldID;
            msg.stPkgData.stChgIntimacyDenyReq.bRelationChgType = (byte) type;
            msg.stPkgData.stChgIntimacyDenyReq.bIntimacyState = (byte) state;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public static void Send_CHG_INTIMACYPRIORITY(COM_INTIMACY_STATE type)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x559);
            msg.stPkgData.stChgIntimacyPriorityReq.bIntimacyState = (byte) type;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
            Singleton<CFriendContoller>.GetInstance().model.FRData.SetFirstChoiseState(type);
        }

        public static void Send_INTIMACY_RELATION_REQUEST(ulong ulluid, uint worldID, COM_INTIMACY_STATE state, COM_INTIMACY_RELATION_CHG_TYPE chgType)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x550);
            msg.stPkgData.stIntimacyRelationRequestReq.stUin.ullUid = ulluid;
            msg.stPkgData.stIntimacyRelationRequestReq.stUin.dwLogicWorldId = worldID;
            msg.stPkgData.stIntimacyRelationRequestReq.bIntimacyState = (byte) state;
            msg.stPkgData.stIntimacyRelationRequestReq.bRelationChgType = (byte) chgType;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }
    }
}

