namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.UI;
    using CSProtocol;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class IntimacyRelationViewUT
    {
        public static string GetMiddleTextStr(COM_INTIMACY_STATE state)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            string str = string.Empty;
            if (state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY)
            {
                str = UT.FRData().IntimRela_Type_Gay;
                return string.Format(UT.FRData().IntimRela_Tips_MidText, masterRoleInfo.Name, str);
            }
            if (state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER)
            {
                str = UT.FRData().IntimRela_Type_Lover;
                return string.Format(UT.FRData().IntimRela_Tips_MidText, masterRoleInfo.Name, str);
            }
            return string.Empty;
        }

        private static void Process_Bottm_Btns(bool bShow, GameObject node, ulong ullUid, uint dwLogicWorldId)
        {
            GameObject obj2 = node.get_transform().FindChild("Button_Send").get_gameObject();
            GameObject obj3 = node.get_transform().FindChild("Button_Cancel").get_gameObject();
            SetEvtParam(obj2, ullUid, dwLogicWorldId);
            SetEvtParam(obj3, ullUid, dwLogicWorldId);
            obj2.CustomSetActive(bShow);
            obj2.CustomSetActive(bShow);
        }

        public static void Set_1_Button(CUIComponent com, bool bShow, string txt = "")
        {
            com.m_widgets[12].CustomSetActive(bShow);
            com.m_widgets[13].GetComponent<Text>().set_text(txt);
        }

        public static void Set_2_Button(CUIComponent com, bool bLeftShow, bool bRightShow, string left = "", string right = "")
        {
            com.m_widgets[0x10].CustomSetActive(bLeftShow);
            com.m_widgets[14].CustomSetActive(bRightShow);
            com.m_widgets[0x11].GetComponent<Text>().set_text(left);
            com.m_widgets[15].GetComponent<Text>().set_text(right);
        }

        public static void Set_Bottom_Text(CUIComponent com, bool bShow, string txt = "")
        {
            com.m_widgets[0x12].CustomSetActive(bShow);
            com.m_widgets[0x12].GetComponent<Text>().set_text(txt);
        }

        public static void Set_DoSelect_Button(CUIComponent com, bool bShow, string txt = "")
        {
            com.m_widgets[0x16].CustomSetActive(bShow);
            com.m_widgets[0x17].GetComponent<Text>().set_text(txt);
        }

        public static void Set_Drop_List(CUIComponent com, bool bShow)
        {
            com.m_widgets[8].CustomSetActive(bShow);
        }

        public static void Set_Drop_Text(CUIComponent com, bool bShow, CFR frData)
        {
            com.m_widgets[9].CustomSetActive(bShow);
            string cfgRelaStr = string.Empty;
            CFriendRelationship.FRConfig cFGByIndex = Singleton<CFriendContoller>.instance.model.FRData.GetCFGByIndex(frData.choiseRelation);
            if (cFGByIndex != null)
            {
                cfgRelaStr = cFGByIndex.cfgRelaStr;
            }
            else
            {
                cfgRelaStr = UT.FRData().IntimRela_Tips_SelectRelation;
            }
            com.m_widgets[9].GetComponent<Text>().set_text(cfgRelaStr);
        }

        public static void Set_Middle_Text(CUIComponent com, bool bShow, string txt = "")
        {
            com.m_widgets[10].CustomSetActive(bShow);
            com.m_widgets[10].GetComponent<Text>().set_text(txt);
        }

        public static void Set_ReSelect_Button(CUIComponent com, bool bShow, string txt = "")
        {
            com.m_widgets[20].CustomSetActive(bShow);
            com.m_widgets[0x15].GetComponent<Text>().set_text(txt);
        }

        private static void SetButtonParam(GameObject obj, CFR frData)
        {
            CUIEventScript component = obj.GetComponent<CUIEventScript>();
            if (component != null)
            {
                component.m_onClickEventParams.commonUInt64Param1 = frData.ulluid;
                component.m_onClickEventParams.tagUInt = frData.worldID;
                component.m_onClickEventParams.tag = (int) frData.state;
                component.m_onClickEventParams.tag2 = frData.choiseRelation;
            }
        }

        private static void SetEvtParam(GameObject obj, ulong ullUid, uint dwLogicWorldId)
        {
            CUIEventScript component = obj.GetComponent<CUIEventScript>();
            component.m_onClickEventParams.commonUInt64Param2 = ullUid;
            component.m_onClickEventParams.taskId = dwLogicWorldId;
        }

        public static void Show_Item(CUIComponent com, CFR frData)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            if (((masterRoleInfo == null) || (masterRoleInfo.playerUllUID != frData.ulluid)) || (masterRoleInfo.logicWorldID != frData.worldID))
            {
                Show_Item_Top(com, frData);
                Show_Item_Middle(com, frData);
                Show_Item_Bottom(com, frData);
                frData.bRedDot = false;
            }
        }

        public static void Show_Item_Bottom(CUIComponent com, CFR frData)
        {
            SetButtonParam(com.m_widgets[12], frData);
            SetButtonParam(com.m_widgets[14], frData);
            SetButtonParam(com.m_widgets[0x10], frData);
            SetButtonParam(com.m_widgets[20], frData);
            SetButtonParam(com.m_widgets[0x16], frData);
            int cDDays = frData.CDDays;
            if (cDDays != -1)
            {
                com.m_widgets[11].CustomSetActive(true);
                Set_2_Button(com, false, false, string.Empty, string.Empty);
                Set_1_Button(com, false, string.Empty);
                Set_ReSelect_Button(com, false, string.Empty);
                Set_DoSelect_Button(com, false, string.Empty);
                Set_Bottom_Text(com, true, UT.FRData().IntimRela_Tips_RelaHasDel);
            }
            else
            {
                if ((frData.state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_VALUE_FULL) && (cDDays == -1))
                {
                    com.m_widgets[11].CustomSetActive(true);
                    string txt = UT.FRData().IntimRela_Tips_OK;
                    string left = UT.FRData().IntimRela_Tips_Cancle;
                    Set_DoSelect_Button(com, frData.choiseRelation != -1, txt);
                    Set_2_Button(com, false, false, left, txt);
                    Set_1_Button(com, false, string.Empty);
                    Set_ReSelect_Button(com, false, string.Empty);
                    Set_Bottom_Text(com, false, string.Empty);
                }
                if ((frData.state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY) || (frData.state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER))
                {
                    com.m_widgets[11].CustomSetActive(true);
                    Set_2_Button(com, false, false, string.Empty, string.Empty);
                    Set_1_Button(com, false, string.Empty);
                    Set_ReSelect_Button(com, false, string.Empty);
                    Set_DoSelect_Button(com, false, string.Empty);
                    if (Singleton<CFriendContoller>.instance.model.FRData.GetFirstChoiseState() == frData.state)
                    {
                        Set_Bottom_Text(com, true, UT.FRData().IntimRela_AleadyFristChoise);
                    }
                    else
                    {
                        Set_Bottom_Text(com, false, string.Empty);
                        Set_1_Button(com, true, UT.FRData().IntimRela_DoFristChoise);
                    }
                }
                if ((frData.state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY_CONFIRM) || (frData.state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER_CONFIRM))
                {
                    if (frData.bReciveOthersRequest)
                    {
                        com.m_widgets[11].CustomSetActive(true);
                        Set_2_Button(com, true, true, UT.FRData().IntimRela_Tips_Cancle, UT.FRData().IntimRela_Tips_OK);
                        Set_1_Button(com, false, string.Empty);
                        Set_Bottom_Text(com, false, string.Empty);
                        Set_ReSelect_Button(com, false, string.Empty);
                        Set_DoSelect_Button(com, false, string.Empty);
                    }
                    else
                    {
                        com.m_widgets[11].CustomSetActive(true);
                        Set_2_Button(com, false, false, string.Empty, string.Empty);
                        Set_1_Button(com, false, string.Empty);
                        Set_Bottom_Text(com, false, UT.FRData().IntimRela_Tips_Wait4TargetRspReqRela);
                        Set_ReSelect_Button(com, true, UT.FRData().IntimRela_ReselectRelation);
                        Set_DoSelect_Button(com, false, string.Empty);
                    }
                }
                if ((frData.state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY_DENY) || (frData.state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER_DENY))
                {
                    if (frData.bReciveOthersRequest)
                    {
                        com.m_widgets[11].CustomSetActive(true);
                        Set_2_Button(com, true, true, UT.FRData().IntimRela_Tips_Cancle, UT.FRData().IntimRela_Tips_OK);
                        Set_1_Button(com, false, string.Empty);
                        Set_Bottom_Text(com, false, string.Empty);
                        Set_ReSelect_Button(com, false, string.Empty);
                        Set_DoSelect_Button(com, false, string.Empty);
                    }
                    else
                    {
                        com.m_widgets[11].CustomSetActive(true);
                        Set_2_Button(com, false, false, string.Empty, string.Empty);
                        Set_1_Button(com, false, string.Empty);
                        Set_Bottom_Text(com, false, UT.FRData().IntimRela_Tips_Wait4TargetRspDelRela);
                        Set_ReSelect_Button(com, true, UT.FRData().IntimRela_ReDelRelation);
                        Set_DoSelect_Button(com, false, string.Empty);
                    }
                }
            }
        }

        public static void Show_Item_Middle(CUIComponent com, CFR frData)
        {
            SetButtonParam(com.m_widgets[0x10], frData);
            SetButtonParam(com.m_widgets[14], frData);
            SetButtonParam(com.m_widgets[12], frData);
            SetButtonParam(com.m_widgets[7], frData);
            int cDDays = frData.CDDays;
            if (cDDays != -1)
            {
                com.m_widgets[5].CustomSetActive(true);
                com.m_widgets[6].CustomSetActive(false);
                Set_Middle_Text(com, true, string.Format(UT.FRData().IntimRela_CD_CountDown, cDDays));
            }
            else
            {
                if ((frData.state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_VALUE_FULL) && (cDDays == -1))
                {
                    com.m_widgets[5].CustomSetActive(true);
                    com.m_widgets[6].CustomSetActive(true);
                    com.m_widgets[10].CustomSetActive(false);
                    Set_Drop_Text(com, !frData.bInShowChoiseRelaList, frData);
                    Set_Drop_List(com, frData.bInShowChoiseRelaList);
                    Set_Middle_Text(com, false, string.Empty);
                }
                if ((frData.state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY) || (frData.state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER))
                {
                    com.m_widgets[5].CustomSetActive(true);
                    com.m_widgets[6].CustomSetActive(false);
                    Set_Middle_Text(com, true, GetMiddleTextStr(frData.state));
                }
                if ((frData.state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY_CONFIRM) || (frData.state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER_CONFIRM))
                {
                    if (frData.bReciveOthersRequest)
                    {
                        com.m_widgets[5].CustomSetActive(true);
                        com.m_widgets[6].CustomSetActive(false);
                        string str = string.Empty;
                        if (frData.state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY_CONFIRM)
                        {
                            str = UT.FRData().IntimRela_Type_Gay;
                        }
                        if (frData.state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER_CONFIRM)
                        {
                            str = UT.FRData().IntimRela_Type_Lover;
                        }
                        string txt = string.Format(UT.FRData().IntimRela_Tips_ReceiveOtherReqRela, UT.Bytes2String(frData.friendInfo.szUserName), str);
                        Set_Middle_Text(com, true, txt);
                    }
                    else
                    {
                        com.m_widgets[5].CustomSetActive(true);
                        com.m_widgets[6].CustomSetActive(false);
                        Set_Middle_Text(com, true, UT.FRData().IntimRela_Tips_Wait4TargetRspReqRela);
                    }
                }
                if ((frData.state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY_DENY) || (frData.state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER_DENY))
                {
                    if (frData.bReciveOthersRequest)
                    {
                        com.m_widgets[5].CustomSetActive(true);
                        com.m_widgets[6].CustomSetActive(false);
                        string str3 = string.Empty;
                        if (frData.state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY_DENY)
                        {
                            str3 = UT.FRData().IntimRela_Type_Gay;
                        }
                        if (frData.state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER_DENY)
                        {
                            str3 = UT.FRData().IntimRela_Type_Lover;
                        }
                        Set_Middle_Text(com, true, string.Format(UT.FRData().IntimRela_Tips_ReceiveOtherDelRela, UT.Bytes2String(frData.friendInfo.szUserName), str3));
                    }
                    else
                    {
                        com.m_widgets[5].CustomSetActive(true);
                        com.m_widgets[6].CustomSetActive(false);
                        Set_Middle_Text(com, true, UT.FRData().IntimRela_Tips_Wait4TargetRspDelRela);
                    }
                }
            }
        }

        public static void Show_Item_Top(CUIComponent com, CFR frData)
        {
            SetButtonParam(com.m_widgets[0x13], frData);
            if ((frData.state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY) || (frData.state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER))
            {
                com.m_widgets[0x13].CustomSetActive(true);
            }
            else
            {
                com.m_widgets[0x13].CustomSetActive(false);
            }
            COMDT_FRIEND_INFO friendInfo = frData.friendInfo;
            if (friendInfo != null)
            {
                UT.SetHttpImage(com.m_widgets[1].GetComponent<CUIHttpImageScript>(), friendInfo.szHeadUrl);
                GameObject obj2 = com.m_widgets[2];
                if (obj2 != null)
                {
                    MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(obj2.GetComponent<Image>(), (int) friendInfo.stGameVip.dwCurLevel, false);
                }
                Text component = com.m_widgets[3].GetComponent<Text>();
                string str = UT.Bytes2String(friendInfo.szUserName);
                if (component != null)
                {
                    component.set_text(str);
                }
                GameObject genderImage = com.m_widgets[4];
                FriendShower.ShowGender(genderImage, (COM_SNSGENDER) friendInfo.bGender);
            }
        }
    }
}

