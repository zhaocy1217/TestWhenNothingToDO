namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.UI;
    using CSProtocol;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Text;
    using UnityEngine;
    using UnityEngine.UI;

    public class UT
    {
        public static void Add2List<T>(T data, ListView<T> list)
        {
            if ((data != null) && (list != null))
            {
                list.Add(data);
            }
        }

        public static bool BEqual_ACNT_UNIQ(COMDT_ACNT_UNIQ a, COMDT_ACNT_UNIQ b, bool ingore_worldid = false)
        {
            if (!ingore_worldid)
            {
                return ((a.ullUid == b.ullUid) && (a.dwLogicWorldId == b.dwLogicWorldId));
            }
            return (a.ullUid == b.ullUid);
        }

        public static string Bytes2String(string str)
        {
            return str;
        }

        public static string Bytes2String(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes).TrimEnd(new char[1]);
        }

        public static int CalcDeltaHorus(uint fromT, uint toT)
        {
            DateTime time = Utility.ToUtcTime2Local((long) fromT);
            DateTime time2 = Utility.ToUtcTime2Local((long) toT);
            if (DateTime.Compare(time2, time) > 0)
            {
                TimeSpan span = (TimeSpan) (time2 - time);
                return Math.Max((int) span.TotalHours, 1);
            }
            return 0;
        }

        public static void Check_AddHeartCD(COMDT_ACNT_UNIQ uniq)
        {
            if (Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo() != null)
            {
                Singleton<CFriendContoller>.GetInstance().model.HeartData.Add(uniq);
            }
        }

        public static void Check_AddReCallCD(COMDT_ACNT_UNIQ uniq, COM_FRIEND_TYPE friendType)
        {
            Singleton<CFriendContoller>.GetInstance().model.SnsReCallData.Add(uniq, friendType);
        }

        public static void CheckGPS()
        {
            if (Singleton<CFriendContoller>.instance.model.EnableShareLocation && !MonoSingleton<GPSSys>.instance.bGetGPSData)
            {
                MonoSingleton<GPSSys>.instance.StartGPS();
            }
        }

        public static bool DebugBattleMiniMapConfigInfo(SLevelContext data)
        {
            if (data == null)
            {
                return false;
            }
            if (!data.IsMobaMode())
            {
                return false;
            }
            bool flag = true;
            string str = string.Format("---地图，id:{0},name:{1},", data.m_mapID, data.m_levelName);
            if (string.IsNullOrEmpty(data.m_miniMapPath))
            {
                str = str + string.Format("缩略图路径配置为空,", new object[0]);
                flag = false;
            }
            if (string.IsNullOrEmpty(data.m_bigMapPath))
            {
                str = str + string.Format("地图路径配置为空,", new object[0]);
                flag = false;
            }
            if (data.m_mapWidth == 0)
            {
                str = str + string.Format("地图宽度配置为空,", new object[0]);
                flag = false;
            }
            if (data.m_mapHeight == 0)
            {
                str = str + string.Format("地图高度配置为空,", new object[0]);
                flag = false;
            }
            if (!flag)
            {
                return false;
            }
            return flag;
        }

        public static string ErrorCode_String(uint dwResult)
        {
            switch (dwResult)
            {
                case 0xab:
                    return GetText("CS_ERR_FRIEND_IN_BLACK");

                case 0xae:
                    return GetText("CS_ERR_LBS_LIMIT");

                case 0xaf:
                    return GetText("CS_ERR_LBSSERECH_TIMELIMIT");

                case 180:
                    return GetText("CS_ERR_FRIEND_ADD_LOCK");

                case 0xb5:
                    return GetText("CS_ERR_FRIEND_ADD_TOO_OFTEN");

                case 0xbb:
                    return GetText("CS_ERR_INTIMACY_REQUEST_SELF");

                case 0xbc:
                    return GetText("CS_ERR_INTIMACY_REQUEST_TIME_LIMIT");

                case 0xbd:
                    return GetText("CS_ERR_INTIMACY_VALUE_NOTENOUGH");

                case 190:
                    return GetText("CS_ERR_INTIMACY_REQUEST_REPEATED");

                case 0xbf:
                    return GetText("CS_ERR_INTIMACY_RELATION_NUM_EXCEED");

                case 0xc0:
                    return GetText("CS_ERR_PEER_INTIMACY_RELATION_NUM_EXCEED");

                case 0xc1:
                    return GetText("CS_ERR_INTIMACY_RELATION_EXSIST");

                case 0xc2:
                    return GetText("CS_ERR_INTIMACY_RELATION_NOTEXIST");

                case 0xc3:
                    return GetText("CS_ERR_INTIMACY_RELATION_OTHER");

                case 0xc4:
                    return GetText("CS_ERR_INTIMACY_RELATION_OFTEN");

                case 0x90:
                    return GetText("Friend_CS_ERR_FRIEND_RECALL_REPEATED");

                case 0x91:
                    return GetText("Friend_CS_ERR_FRIEND_EXCEED");

                case 0x92:
                    return GetText("Friend_CS_ERR_FRIEND_RECALL_TIME_LIMIT");

                case 0x97:
                    return GetText("CS_ERR_REFUSE_RECALL_REPEATED");

                case 0x98:
                    return GetText("CS_ERR_REFUSE_ADDFRIEND");

                case 0x9a:
                    return GetText("CS_ERR_VERIFICATION_ILLEGAL");

                case 160:
                    return GetText("CS_ERR_DEFRIEND_REPEATED");

                case 0xa1:
                    return GetText("CS_ERR_BLACKLIST_NOT_EXSIST");

                case 0xa2:
                    return GetText("CS_ERR_BLACKLIST_EXCEED");

                case 0xa3:
                    return GetText("CS_ERR_FRIEND_INVALID_PLAT");

                case 0x65:
                    return GetText("Friend_CS_ERR_FRIEND_TCAPLUS_ERR");

                case 0x66:
                    return GetText("Friend_CS_ERR_FRIEND_RECORD_NOT_EXSIST");

                case 0x67:
                    return GetText("Friend_CS_ERR_FRIEND_NUM_EXCEED");

                case 0x68:
                    return GetText("Friend_CS_ERR_PEER_FRIEND_NUM_EXCEED");

                case 0x69:
                    return GetText("Friend_CS_ERR_FRIEND_DONATE_AP_EXCEED");

                case 0x6a:
                    return GetText("Friend_CS_ERR_FRIEND_RECV_AP_EXCEED");

                case 0x6b:
                    return GetText("Friend_CS_ERR_FRIEND_ADD_FRIEND_DENY");

                case 0x6c:
                    return GetText("Friend_CS_ERR_FRIEND_ADD_FRIEND_SELF");

                case 0x6d:
                    return GetText("Friend_CS_ERR_FRIEND_ADD_FRIEND_EXSIST");

                case 110:
                    return GetText("Friend_CS_ERR_FRIEND_REQ_REPEATED");

                case 0x6f:
                    return GetText("Friend_CS_ERR_FRIEND_NOT_EXSIST");

                case 0x70:
                    return GetText("Friend_CS_ERR_FRIEND_SEND_MAIL");

                case 0x71:
                    return GetText("Friend_CS_ERR_FRIEND_DONATE_REPEATED");

                case 0x72:
                    return GetText("Friend_CS_ERR_FRIEND_AP_FULL");

                case 0x74:
                    return GetText("Friend_CS_ERR_FRIEND_ADD_FRIEND_ZONE");

                case 0x75:
                    return GetText("Friend_CS_ERR_FRIEND_OTHER");

                case 0x25:
                    return GetText("CS_ERR_RECRUITER_LEVELLIMIT");

                case 0x2a:
                    return GetText("CS_ERR_RECRUIT_INVITECODE");

                case 0x2b:
                    return GetText("CS_ERR_RECRUIT_LEVELLIMIT");

                case 0x2c:
                    return GetText("CS_ERR_RECRUIT_NOTONEPLAT");

                case 0x2d:
                    return GetText("CS_ERR_PASSIVE_RECRUIT_NUMLIMIT");

                case 0x2e:
                    return GetText("CS_ERR_ACTIVE_RECRUIT_NUMLIMIT");

                case 0x2f:
                    return GetText("CS_ERR_RECRUIT_OTHER");

                case 0x30:
                    return GetText("CS_ERR_RECRUIT_SELF");

                case 0x31:
                    return GetText("CS_ERR_RECRUIT_CLOSING");

                case 2:
                    return GetText("Friend_CS_ERR_STARTSINGLEGAME_FAIL");

                case 4:
                    return GetText("Friend_CS_ERR_FINSINGLEGAME_FAIL");

                case 5:
                    return GetText("Friend_CS_ERR_QUITMULTGAME_FAIL");

                case 6:
                    return GetText("Friend_CS_ERR_REGISTER_NAME_DUP_FAIL");

                case 7:
                    return GetText("Friend_CS_ERR_SHOULD_REFRESH_TASK");

                case 8:
                    return GetText("Friend_CS_ERR_COMMIT_ERR");
            }
            return string.Format(GetText("Friend_CS_ERR_FRIEND_DEFAULT"), dwResult);
        }

        public static CFriendRelationship FRData()
        {
            return Singleton<CFriendContoller>.instance.model.FRData;
        }

        public static string GetDistance(uint distance)
        {
            if (distance >= 0x3e8)
            {
                return string.Format("{0}公里", distance / 0x3e8);
            }
            return string.Format("{0}米", distance);
        }

        public static string GetFriendResultTypeString(FriendResultType type)
        {
            if (type != FriendResultType.RequestBeFriend)
            {
                return string.Empty;
            }
            return GetText("Friend_Tips_Send_BeFriend_Ok");
        }

        public static string GetFriendSourceDesc(ulong friendUid, COMDT_FRIEND_SOURCE friendSource)
        {
            if (friendSource != null)
            {
                COM_ADD_FRIEND_TYPE bAddFriendType = (COM_ADD_FRIEND_TYPE) friendSource.bAddFriendType;
                if (bAddFriendType == COM_ADD_FRIEND_TYPE.COM_ADD_FRIEND_PVP)
                {
                    string heroName = CHeroInfo.GetHeroName(friendSource.stAddFriendInfo.stPvp.dwHeroID);
                    if (!string.IsNullOrEmpty(heroName))
                    {
                        string[] args = new string[] { heroName };
                        return Singleton<CTextManager>.GetInstance().GetText("Friend_Apply_Play_With_You_Tip", args);
                    }
                    object[] inParameters = new object[] { friendSource.stAddFriendInfo.stPvp.dwHeroID };
                    DebugHelper.Assert(false, "好友来源是PVP，但却获取不了英雄名，heroId={0}", inParameters);
                }
                else
                {
                    if (CGuildHelper.IsInSameGuild(friendUid))
                    {
                        return Singleton<CTextManager>.GetInstance().GetText("Friend_Apply_Same_Guild_Tip");
                    }
                    if (bAddFriendType == COM_ADD_FRIEND_TYPE.COM_ADD_FRIEND_LBS)
                    {
                        return Singleton<CTextManager>.GetInstance().GetText("Friend_Apply_Nearby_You_Tip");
                    }
                }
            }
            return string.Empty;
        }

        public static string GetText(string key)
        {
            return Singleton<CTextManager>.instance.GetText(key);
        }

        public static string GetTimeString(uint time)
        {
            string str = string.Empty;
            DateTime time2 = Utility.ToUtcTime2Local((long) time);
            DateTime time3 = Utility.ToUtcTime2Local((long) CRoleInfo.GetCurrentUTCTime());
            if (DateTime.Compare(time3, time2) <= 0)
            {
                return str;
            }
            TimeSpan span = (TimeSpan) (time3 - time2);
            if (span.Days == 0)
            {
                if (span.Hours == 0)
                {
                    return string.Format(GetText("Friend_Tips_lastTime_min"), span.Minutes);
                }
                return string.Format(GetText("Friend_Tips_lastTime_hour_min"), span.Hours, span.Minutes);
            }
            int days = span.Days;
            if (days > 7)
            {
                days = 7;
            }
            return string.Format(GetText("Friend_Tips_lastTime_day"), days);
        }

        public static void If_Null_Error<T>(T v) where T: class
        {
            if (v == null)
            {
            }
        }

        public static bool IsFreeze(uint lastTime)
        {
            DateTime time = Utility.ToUtcTime2Local((long) lastTime);
            DateTime time2 = Utility.ToUtcTime2Local((long) CRoleInfo.GetCurrentUTCTime());
            if (DateTime.Compare(time2, time) > 0)
            {
                TimeSpan span = (TimeSpan) (time2 - time);
                if (span.Days >= Singleton<CFriendContoller>.instance.model.freezeDayCount)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool NeedShowGenderGradeByMentor(FriendShower.ItemType type, CFriendModel.FriendType friendType)
        {
            return ((((friendType == CFriendModel.FriendType.MentorRecommend) || (type == FriendShower.ItemType.AddMentor)) || (type == FriendShower.ItemType.AddApprentice)) || (friendType == CFriendModel.FriendType.MentorRequestList));
        }

        public static void ResetTimer(int timer, bool bPause)
        {
            Singleton<CTimerManager>.GetInstance().PauseTimer(timer);
            Singleton<CTimerManager>.GetInstance().ResetTimer(timer);
            if (!bPause)
            {
                Singleton<CTimerManager>.GetInstance().ResumeTimer(timer);
            }
        }

        public static void SetAddNodeActive(GameObject addnode, CFriendModel.FriendType friendType, bool ifGrey = false)
        {
            if (addnode != null)
            {
                addnode.CustomSetActive(true);
                Transform transform = addnode.get_transform().Find("add_btn");
                if (transform != null)
                {
                    GameObject obj2 = transform.get_gameObject();
                    if (obj2 != null)
                    {
                        obj2.CustomSetActive(!ifGrey);
                        stUIEventParams eventParams = new stUIEventParams();
                        eventParams.tag = (int) friendType;
                        CUIEventScript component = obj2.GetComponent<CUIEventScript>();
                        if (component != null)
                        {
                            component.SetUIEvent(enUIEventType.Click, enUIEventID.Friend_RequestBeFriend, eventParams);
                        }
                    }
                }
            }
        }

        public static void SetChatFace(CUIFormScript formScript, Image img, int index)
        {
            img.SetSprite(string.Format("UGUI/Sprite/Dynamic/ChatFace/{0}", index), formScript, true, false, false, false);
        }

        public static void SetHttpImage(CUIHttpImageScript HttpImage, string url)
        {
            if ((HttpImage != null) && !CSysDynamicBlock.bFriendBlocked)
            {
                if (url == null)
                {
                    url = string.Empty;
                }
                if (HttpImage.get_gameObject().get_activeSelf())
                {
                    HttpImage.SetImageUrl(Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(url));
                }
            }
        }

        public static void SetHttpImage(CUIHttpImageScript HttpImage, byte[] szHeadUrl)
        {
            if (szHeadUrl == null)
            {
                SetHttpImage(HttpImage, string.Empty);
            }
            else
            {
                SetHttpImage(HttpImage, Bytes2String(szHeadUrl));
            }
        }

        public static void SetImage(Image img, bool bGray)
        {
            if (img != null)
            {
                img.set_color(!bGray ? new Color(1f, 1f, 1f, 1f) : new Color(0f, 1f, 1f));
            }
        }

        public static void SetListIndex(CUIListScript com, int index)
        {
            com.m_alwaysDispatchSelectedChangeEvent = true;
            com.SelectElement(index, true);
            com.m_alwaysDispatchSelectedChangeEvent = false;
        }

        public static void SetShow(CanvasGroup cg, bool bShow)
        {
            if (cg != null)
            {
                if (bShow)
                {
                    cg.set_alpha(1f);
                    cg.set_blocksRaycasts(true);
                }
                else
                {
                    cg.set_alpha(0f);
                    cg.set_blocksRaycasts(false);
                }
            }
        }

        public static void SetTabList(List<string> titles, int start_index, CUIListScript tablistScript)
        {
            if (tablistScript != null)
            {
                DebugHelper.Assert(start_index < titles.Count, "SetTabList, should start_index < titles.Count");
                tablistScript.SetElementAmount(titles.Count);
                for (int i = 0; i < tablistScript.m_elementAmount; i++)
                {
                    tablistScript.GetElemenet(i).get_gameObject().get_transform().FindChild("Text").GetComponent<Text>().set_text(titles[i]);
                }
                tablistScript.m_alwaysDispatchSelectedChangeEvent = true;
                tablistScript.SelectElement(start_index, true);
                tablistScript.m_alwaysDispatchSelectedChangeEvent = false;
            }
        }

        public static void ShowBlackListData(ref CFriendModel.stBlackName info, FriendShower com)
        {
            if (com != null)
            {
                com.ullUid = info.ullUid;
                com.dwLogicWorldID = info.dwLogicWorldId;
                com.HttpImage.SetImageUrl(Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(info.szHeadUrl));
                if (com.nobeIcon != null)
                {
                    com.nobeIcon.CustomSetActive(false);
                }
                if (com.HeadIconBack != null)
                {
                    com.HeadIconBack.CustomSetActive(false);
                }
                if (com.QQVipImage != null)
                {
                    com.QQVipImage.CustomSetActive(false);
                }
                com.SetFriendItemType(FriendShower.ItemType.BlackList, true);
                com.SetBGray(false);
                com.ShowName(info.name);
                com.ShowLevel(info.dwPvpLvl);
                com.ShowLastTime(true, GetTimeString(info.dwLastLoginTime));
                com.intimacyNode.CustomSetActive(false);
                com.ShowGenderType((COM_SNSGENDER) info.bGender);
            }
        }

        public static void ShowFriendData(COMDT_FRIEND_INFO info, FriendShower com, FriendShower.ItemType type, bool bShowNickName, CFriendModel.FriendType friendType)
        {
            if (info != null)
            {
                string str3;
                com.ullUid = info.stUin.ullUid;
                com.dwLogicWorldID = info.stUin.dwLogicWorldId;
                SetHttpImage(com.HttpImage, info.szHeadUrl);
                if (com.nobeIcon != null)
                {
                    MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(com.nobeIcon.GetComponent<Image>(), (int) info.stGameVip.dwCurLevel, false);
                }
                if (com.HeadIconBack != null)
                {
                    MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(com.HeadIconBack.GetComponent<Image>(), (int) info.stGameVip.dwHeadIconId);
                }
                if (com.QQVipImage != null)
                {
                    MonoSingleton<NobeSys>.GetInstance().SetOtherQQVipHead(com.QQVipImage.GetComponent<Image>(), (int) info.dwQQVIPMask);
                }
                if (((type == FriendShower.ItemType.Normal) && (friendType == CFriendModel.FriendType.GameFriend)) || ((type == FriendShower.ItemType.Mentor) || (type == FriendShower.ItemType.Apprentice)))
                {
                    ushort num;
                    CFriendModel.EIntimacyType type2;
                    bool flag;
                    CFriendModel model = Singleton<CFriendContoller>.instance.model;
                    if (model.GetFriendIntimacy(info.stUin.ullUid, info.stUin.dwLogicWorldId, out num, out type2, out flag))
                    {
                        if (num == 0)
                        {
                            com.intimacyNode.CustomSetActive(false);
                        }
                        else
                        {
                            com.intimacyNode.CustomSetActive(true);
                            CFR cfr = model.FRData.GetCfr(info.stUin.ullUid, info.stUin.dwLogicWorldId);
                            if (cfr != null)
                            {
                                com.ShowIntimacyNum(num, type2, flag, cfr.state);
                            }
                            else
                            {
                                com.ShowIntimacyNum(num, type2, flag, COM_INTIMACY_STATE.COM_INTIMACY_STATE_NULL);
                            }
                        }
                    }
                    else
                    {
                        com.intimacyNode.CustomSetActive(false);
                    }
                }
                else
                {
                    com.intimacyNode.CustomSetActive(false);
                }
                com.SetFriendItemType(type, !bShowNickName);
                bool flag2 = ((((type == FriendShower.ItemType.Normal) || (type == FriendShower.ItemType.Mentor)) || ((type == FriendShower.ItemType.Apprentice) || (type == FriendShower.ItemType.AddMentor))) || (type == FriendShower.ItemType.AddApprentice)) || (type == FriendShower.ItemType.MentorRequest);
                com.SetBGray(flag2 && (info.bIsOnline != 1));
                com.ShowLevel(info.dwPvpLvl);
                com.ShowVipLevel(info.dwVipLvl);
                com.ShowLastTime(info.bIsOnline != 1, GetTimeString(info.dwLastLoginTime));
                switch (type)
                {
                    case FriendShower.ItemType.Request:
                    {
                        string text = Singleton<CFriendContoller>.instance.model.GetFriendVerifyContent(info.stUin.ullUid, info.stUin.dwLogicWorldId, CFriendModel.enVerifyDataSet.Friend);
                        com.ShowVerify(text);
                        break;
                    }
                    case FriendShower.ItemType.MentorRequest:
                    {
                        string str2 = Singleton<CFriendContoller>.instance.model.GetFriendVerifyContent(info.stUin.ullUid, info.stUin.dwLogicWorldId, CFriendModel.enVerifyDataSet.Mentor);
                        com.ShowVerify(str2);
                        break;
                    }
                }
                CFriendModel.FriendInGame friendInGaming = Singleton<CFriendContoller>.instance.model.GetFriendInGaming(info.stUin.ullUid, info.stUin.dwLogicWorldId);
                if (friendInGaming == null)
                {
                    str3 = Bytes2String(info.szUserName);
                    com.ShowGameState(COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_IDLE, info.bIsOnline == 1);
                }
                else
                {
                    if (!string.IsNullOrEmpty(friendInGaming.NickName) && bShowNickName)
                    {
                        str3 = string.Format("{0}({1})", Bytes2String(info.szUserName), friendInGaming.NickName);
                    }
                    else
                    {
                        str3 = Bytes2String(info.szUserName);
                    }
                    com.ShowGameState(friendInGaming.State, info.bIsOnline == 1);
                }
                switch (type)
                {
                    case FriendShower.ItemType.Request:
                    {
                        string friendSourceDesc = GetFriendSourceDesc(info.stUin.ullUid, Singleton<CFriendContoller>.instance.model.GetFriendVerifySource(info.stUin.ullUid, info.stUin.dwLogicWorldId, CFriendModel.enVerifyDataSet.Friend));
                        if (!string.IsNullOrEmpty(friendSourceDesc))
                        {
                            str3 = str3 + " " + friendSourceDesc;
                        }
                        break;
                    }
                    case FriendShower.ItemType.MentorRequest:
                    {
                        COMDT_FRIEND_SOURCE friendSource = Singleton<CFriendContoller>.instance.model.GetFriendVerifySource(info.stUin.ullUid, info.stUin.dwLogicWorldId, CFriendModel.enVerifyDataSet.Mentor);
                        if (friendSource != null)
                        {
                            string str5 = GetFriendSourceDesc(info.stUin.ullUid, friendSource);
                            if (!string.IsNullOrEmpty(str5))
                            {
                                str3 = str3 + " " + str5;
                            }
                        }
                        break;
                    }
                }
                com.ShowName(str3);
                if (Singleton<CGuildSystem>.GetInstance().CanInvite(info))
                {
                    if (Singleton<CGuildSystem>.GetInstance().HasInvited(info.stUin.ullUid))
                    {
                        com.ShowinviteGuild(true, false);
                    }
                    else
                    {
                        com.ShowinviteGuild(true, true);
                    }
                }
                else if (Singleton<CGuildSystem>.GetInstance().CanRecommend(info))
                {
                    if (Singleton<CGuildSystem>.GetInstance().HasRecommended(info.stUin.ullUid))
                    {
                        com.ShowRecommendGuild(true, false);
                    }
                    else
                    {
                        com.ShowRecommendGuild(true, true);
                    }
                }
                else
                {
                    com.ShowinviteGuild(false, false);
                }
                bool bEnable = Singleton<CFriendContoller>.instance.model.HeartData.BCanSendHeart(info.stUin);
                com.ShowSendButton(bEnable);
                if (CSysDynamicBlock.bSocialBlocked)
                {
                    com.HideSendButton();
                }
                if ((com.m_mentorTitleObj != null) && (com.m_mentorTitleObj.get_transform().get_parent() != null))
                {
                    com.m_mentorTitleObj.get_transform().get_parent().get_gameObject().CustomSetActive(false);
                }
                if (NeedShowGenderGradeByMentor(type, friendType))
                {
                    com.ShowMentorSearchInfo(info, friendType, type);
                    com.HideSendButton();
                }
                else if ((friendType == CFriendModel.FriendType.Mentor) || (friendType == CFriendModel.FriendType.Apprentice))
                {
                    com.ShowInviteButton(false, false);
                }
                else if (CSysDynamicBlock.bSocialBlocked)
                {
                    com.ShowInviteButton(false, false);
                }
                else if (friendType == CFriendModel.FriendType.GameFriend)
                {
                    com.ShowInviteButton(false, false);
                }
                else if (friendType == CFriendModel.FriendType.SNS)
                {
                    bool isShow = CFriendReCallData.BLose(info.stUin, COM_FRIEND_TYPE.COM_FRIEND_TYPE_SNS);
                    bool flag5 = Singleton<CFriendContoller>.instance.model.SnsReCallData.BInCd(info.stUin, COM_FRIEND_TYPE.COM_FRIEND_TYPE_SNS);
                    bool flag6 = CFriendModel.IsOnSnsSwitch(info.dwRefuseFriendBits, COM_REFUSE_TYPE.COM_REFUSE_TYPE_DONOTE_AND_REC);
                    com.ShowInviteButton(isShow, !flag5 && !flag6);
                }
                if (Singleton<COBSystem>.instance.IsInOBFriendList(info.stUin.ullUid))
                {
                    com.ShowOBButton(true);
                }
                else
                {
                    com.ShowOBButton(false);
                }
                com.ShowGenderType((COM_SNSGENDER) info.bGender);
                com.ShowPlatChannelIcon(info);
                com.ShowSendGiftBtn(true);
                CFriendModel.FriendType type5 = friendType;
                if ((type5 == CFriendModel.FriendType.Mentor) || (type5 == CFriendModel.FriendType.Apprentice))
                {
                    enMentorRelationType type3 = (enMentorRelationType) (info.bStudentType >> 4);
                    int num2 = info.bStudentType & 15;
                    switch (type3)
                    {
                        case enMentorRelationType.mentor:
                            if (com.mentor_relationship != null)
                            {
                                com.mentor_relationship.CustomSetActive(friendType == CFriendModel.FriendType.Mentor);
                                com.mentor_relationship.get_transform().Find("MentorRelationText").GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("Mentor_mentor"));
                            }
                            break;

                        case enMentorRelationType.schoolmate:
                            if (com.mentor_relationship != null)
                            {
                                com.mentor_relationship.CustomSetActive(friendType == CFriendModel.FriendType.Mentor);
                                com.mentor_relationship.get_transform().Find("MentorRelationText").GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("Mentor_schoolmate"));
                            }
                            if (com.normal_node != null)
                            {
                                com.normal_node.CustomSetActive(true);
                            }
                            for (int i = 0; i < com.normal_node.get_transform().get_childCount(); i++)
                            {
                                Transform child = com.normal_node.get_transform().GetChild(i);
                                if (child != null)
                                {
                                    child.get_gameObject().CustomSetActive(false);
                                }
                            }
                            break;
                    }
                    int num5 = num2;
                    if (num5 != 1)
                    {
                        if ((num5 == 2) && (com.mentor_graduation != null))
                        {
                            com.mentor_graduation.CustomSetActive((friendType == CFriendModel.FriendType.Apprentice) || (type3 == enMentorRelationType.schoolmate));
                            com.mentor_graduation.get_transform().Find("MentorStatusText").GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("Mentor_Graduated"));
                            if (com.normal_node != null)
                            {
                                com.normal_node.CustomSetActive(true);
                            }
                            for (int j = 0; j < com.normal_node.get_transform().get_childCount(); j++)
                            {
                                Transform transform2 = com.normal_node.get_transform().GetChild(j);
                                if (transform2 != null)
                                {
                                    transform2.get_gameObject().CustomSetActive(false);
                                }
                            }
                            if (com.del_node != null)
                            {
                                com.del_node.CustomSetActive(type3 == enMentorRelationType.apprentice);
                            }
                        }
                    }
                    else if (com.mentor_graduation != null)
                    {
                        com.mentor_graduation.CustomSetActive((friendType == CFriendModel.FriendType.Apprentice) || (type3 == enMentorRelationType.schoolmate));
                        com.mentor_graduation.get_transform().Find("MentorStatusText").GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("Mentor_InStudy"));
                    }
                    CFriendModel model2 = Singleton<CFriendContoller>.instance.model;
                    SetAddNodeActive(com.add_node, friendType, model2.IsSnsFriend(com.ullUid, com.dwLogicWorldID) || model2.IsGameFriend(com.ullUid, com.dwLogicWorldID));
                }
            }
        }

        public static void ShowFriendNetResult(uint dwResult, FriendResultType type)
        {
            string strContent = string.Empty;
            if (dwResult == 0)
            {
                strContent = GetFriendResultTypeString(type);
            }
            else
            {
                strContent = ErrorCode_String(dwResult);
            }
            Singleton<CUIManager>.GetInstance().OpenTips(strContent, false, 1.5f, null, new object[0]);
        }

        public static void ShowLBSUserData(CSDT_LBS_USER_INFO info, FriendShower com)
        {
            if (com != null)
            {
                com.ullUid = info.stLbsUserInfo.stUin.ullUid;
                com.dwLogicWorldID = info.stLbsUserInfo.stUin.dwLogicWorldId;
                if (info.stLbsUserInfo.szHeadUrl != null)
                {
                    string url = Bytes2String(info.stLbsUserInfo.szHeadUrl);
                    com.HttpImage.SetImageUrl(Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(url));
                }
                if (com.nobeIcon != null)
                {
                    MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(com.nobeIcon.GetComponent<Image>(), (int) info.stLbsUserInfo.stGameVip.dwCurLevel, false);
                }
                if (com.HeadIconBack != null)
                {
                    MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(com.HeadIconBack.GetComponent<Image>(), (int) info.stLbsUserInfo.stGameVip.dwHeadIconId);
                }
                if (com.QQVipImage != null)
                {
                    MonoSingleton<NobeSys>.GetInstance().SetOtherQQVipHead(com.QQVipImage.GetComponent<Image>(), (int) info.stLbsUserInfo.dwQQVIPMask);
                }
                com.intimacyNode.CustomSetActive(false);
                com.SetFriendItemType(FriendShower.ItemType.LBS, true);
                com.ShowName(Bytes2String(info.stLbsUserInfo.szUserName));
                com.ShowLevel(info.stLbsUserInfo.dwPvpLvl);
                com.ShowLastTime(true, GetTimeString(info.stLbsUserInfo.dwLastLoginTime));
                com.ShowGenderType((COM_SNSGENDER) info.stLbsUserInfo.bGender);
                com.ShowDistance(GetDistance(info.dwDistance));
                com.SetBGray(info.stLbsUserInfo.bIsOnline != 1);
                if (com.platChannelIcon != null)
                {
                    com.platChannelIcon.CustomSetActive(false);
                }
                if (com.lbsAddFriendBtn != null)
                {
                    CFriendModel model = Singleton<CFriendContoller>.instance.model;
                    if (model.IsSnsFriend(com.ullUid, com.dwLogicWorldID) || model.IsGameFriend(com.ullUid, com.dwLogicWorldID))
                    {
                        CUICommonSystem.SetButtonEnable(com.lbsAddFriendBtn, false, false, true);
                    }
                    else
                    {
                        CUICommonSystem.SetButtonEnable(com.lbsAddFriendBtn, true, true, true);
                    }
                }
                GameObject hisRankGo = com.get_gameObject().get_transform().Find("body/LBS/Rank").get_gameObject();
                GameObject obj3 = com.get_gameObject().get_transform().Find("body/LBS/HisRank").get_gameObject();
                ShowRank(com.formScript, hisRankGo, info.bGradeOfRank, info.stLbsUserInfo.dwRankClass);
                ShowRank(com.formScript, obj3, info.bMaxGradeOfRank, info.stLbsUserInfo.dwRankClass);
            }
        }

        public static void ShowRank(CUIFormScript form, GameObject HisRankGo, byte RankGrade, uint RankClass)
        {
            if ((form != null) && (HisRankGo != null))
            {
                if (RankGrade == 0)
                {
                    if (HisRankGo != null)
                    {
                        HisRankGo.CustomSetActive(false);
                    }
                }
                else
                {
                    HisRankGo.CustomSetActive(true);
                    Image componetInChild = null;
                    Image image2 = null;
                    if (HisRankGo != null)
                    {
                        componetInChild = Utility.GetComponetInChild<Image>(HisRankGo, "ImgRank");
                        image2 = Utility.GetComponetInChild<Image>(HisRankGo, "ImgRank/ImgSubRank");
                    }
                    if (componetInChild != null)
                    {
                        string rankSmallIconPath = CLadderView.GetRankSmallIconPath(RankGrade, RankClass);
                        componetInChild.SetSprite(rankSmallIconPath, form, true, false, false, false);
                    }
                    if (image2 != null)
                    {
                        string subRankSmallIconPath = CLadderView.GetSubRankSmallIconPath(RankGrade, RankClass);
                        image2.SetSprite(subRankSmallIconPath, form, true, false, false, false);
                    }
                }
            }
        }

        public static void ShowSNSFriendData(COMDT_SNS_FRIEND_INFO info, FriendShower com)
        {
            com.ullUid = info.ullUid;
            com.dwLogicWorldID = info.dwLogicWorldId;
            string url = Bytes2String(info.szHeadUrl);
            com.HttpImage.SetImageUrl(Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(url));
            com.SetFriendItemType(FriendShower.ItemType.Add, true);
            com.SetBGray(false);
            com.ShowName(string.Format("{0}({1})", Bytes2String(info.szRoleName), Bytes2String(info.szNickName)));
            com.ShowLevel(info.dwPvpLvl);
            com.ShowVipLevel(info.dwPvpLvl);
            com.ShowLastTime(true, GetTimeString(info.dwLastLoginTime));
        }

        public static byte[] String2Bytes(string name)
        {
            return Encoding.UTF8.GetBytes(name);
        }

        public enum FriendResultType
        {
            RequestBeFriend
        }
    }
}

