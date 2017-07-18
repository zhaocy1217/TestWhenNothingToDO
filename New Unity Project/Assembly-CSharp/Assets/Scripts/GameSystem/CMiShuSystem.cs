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

    [MessageHandlerClass]
    public class CMiShuSystem : Singleton<CMiShuSystem>
    {
        public void CheckActPlayModeTipsForLobby()
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CLobbySystem.LOBBY_FORM_PATH);
            if ((form != null) && !CSysDynamicBlock.bLobbyEntryBlocked)
            {
                Transform transform = form.get_transform().Find("BtnCon/PvpBtn/ActModePanel");
                if (transform != null)
                {
                    transform.SetAsLastSibling();
                    transform.get_gameObject().CustomSetActive(this.IsHaveEntertrainModeOpen());
                }
            }
        }

        public void CheckActPlayModeTipsForPvpEntry()
        {
            uint result = 0;
            uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_PVP_Fire"), out result);
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CMatchingSystem.PATH_MATCHING_ENTRY);
            if ((form != null) && !CSysDynamicBlock.bLobbyEntryBlocked)
            {
                Transform transform = form.get_transform().Find("panelGroup1/btnGroup/ButtonEntertain/ActModePanel");
                if (transform != null)
                {
                    transform.get_gameObject().CustomSetActive(this.IsHaveEntertrainModeOpen());
                }
            }
            if (form != null)
            {
                CUIMiniEventScript component = form.get_transform().FindChild("panelGroup5/btnGroup/Button1").GetComponent<CUIMiniEventScript>();
                CUIMiniEventScript script3 = form.get_transform().FindChild("panelGroup5/btnGroup/Button3").GetComponent<CUIMiniEventScript>();
                CUIMiniEventScript script4 = form.get_transform().FindChild("panelGroup5/btnGroup/Button4").GetComponent<CUIMiniEventScript>();
                this.refreshEntertainMentOpenStateUI(component.get_gameObject(), component.m_onClickEventParams.tagUInt);
                this.refreshEntertainMentOpenStateUI(script3.get_gameObject(), script3.m_onClickEventParams.tagUInt);
                this.refreshEntertainMentOpenStateUI(script4.get_gameObject(), script4.m_onClickEventParams.tagUInt);
                if (CSysDynamicBlock.bLobbyEntryBlocked)
                {
                    script4.get_gameObject().CustomSetActive(false);
                }
            }
        }

        public void CheckMiShuTalk(bool isRestarTimer = true)
        {
            bool flag = false;
            string szMiShuDesc = null;
            if (Singleton<CTaskSys>.instance.model.IsShowMainTaskTab_RedDotCount())
            {
                flag = true;
                szMiShuDesc = Singleton<CTextManager>.instance.GetText("Secretary_Reward_Tips");
            }
            else if (Singleton<CTaskSys>.instance.model.GetMaxIndex_TaskID_InState(enTaskTab.TAB_USUAL, CTask.State.Have_Done) != null)
            {
                flag = true;
                szMiShuDesc = Singleton<CTextManager>.instance.GetText("Secretary_Reward_Tips");
            }
            else
            {
                CTask task = Singleton<CTaskSys>.instance.model.GetMaxIndex_TaskID_InState(enTaskTab.TAB_USUAL, CTask.State.OnGoing);
                if (task != null)
                {
                    flag = true;
                    szMiShuDesc = task.m_resTask.szMiShuDesc;
                }
            }
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CLobbySystem.LOBBY_FORM_PATH);
            if (form != null)
            {
                Transform transform = form.get_transform().Find("LobbyBottom/Newbie/TalkFrame");
                Text component = form.get_transform().Find("LobbyBottom/Newbie/TalkFrame/Text").GetComponent<Text>();
                CUITimerScript script2 = form.get_transform().Find("LobbyBottom/Newbie/TalkFrame/Timer").GetComponent<CUITimerScript>();
                if (flag)
                {
                    transform.get_gameObject().CustomSetActive(true);
                    component.set_text(szMiShuDesc);
                    script2.ReStartTimer();
                }
                else
                {
                    transform.get_gameObject().CustomSetActive(false);
                    script2.EndTimer();
                }
                if (isRestarTimer)
                {
                    form.get_transform().Find("LobbyBottom/Newbie/Timer").GetComponent<CUITimerScript>().ReStartTimer();
                }
            }
        }

        public ResMiShuInfo[] GetResList(int TabIndex)
        {
            List<ResMiShuInfo> list = new List<ResMiShuInfo>();
            GameDataMgr.miShuLib.Reload();
            Dictionary<long, object>.Enumerator enumerator = GameDataMgr.miShuLib.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<long, object> current = enumerator.Current;
                ResMiShuInfo item = (ResMiShuInfo) current.Value;
                if (item.bType == TabIndex)
                {
                    list.Add(item);
                }
            }
            return list.ToArray();
        }

        public override void Init()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.MiShu_OnClickMiShu, new CUIEventManager.OnUIEventHandler(this.OnClickMiShu));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.MiShu_OnCheckTalk, new CUIEventManager.OnUIEventHandler(this.OnCheckMiShuTalk));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.MiShu_OnCloseTalk, new CUIEventManager.OnUIEventHandler(this.OnCloseTalk));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.MiShu_OnClickGoto, new CUIEventManager.OnUIEventHandler(this.OnClickGotoEntry));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.MiShu_OnCheckFirstWin, new CUIEventManager.OnUIEventHandler(this.OnCheckFirstWin));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.MiShu_OnPlayAnimation, new CUIEventManager.OnUIEventHandler(this.OnPlayMishuAnimation));
        }

        public void InitList(int TabIndex, CUIListScript list)
        {
            ResMiShuInfo[] resList = this.GetResList(TabIndex);
            list.SetElementAmount(resList.Length);
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            for (int i = 0; i < resList.Length; i++)
            {
                Transform transform = list.GetElemenet(i).get_transform();
                Image component = transform.Find("imgIcon").GetComponent<Image>();
                Text text = transform.Find("lblTitle").GetComponent<Text>();
                Text text2 = transform.Find("lblUnLock").GetComponent<Text>();
                Text text3 = transform.Find("lblDesc").GetComponent<Text>();
                Text text4 = transform.Find("lblCoinDesc").GetComponent<Text>();
                Button btn = transform.Find("btnGoto").GetComponent<Button>();
                component.SetSprite(CUIUtility.s_Sprite_Dynamic_Task_Dir + resList[i].dwIconID, list.m_belongedFormScript, true, false, false, false);
                text.set_text(resList[i].szName);
                text2.set_text(resList[i].szUnOpenDesc);
                text3.set_text(resList[i].szDesc);
                text4.set_text(string.Empty);
                this.InitSysBtn(btn, (RES_GAME_ENTRANCE_TYPE) resList[i].bGotoID, text2.get_gameObject(), text4.get_gameObject());
                text4.get_gameObject().CustomSetActive(false);
            }
        }

        private void InitSysBtn(Button btn, RES_GAME_ENTRANCE_TYPE entryType, GameObject txtObj, GameObject coinTextObj)
        {
            RES_SPECIALFUNCUNLOCK_TYPE type = CUICommonSystem.EntryTypeToUnlockType(entryType);
            if (Singleton<CFunctionUnlockSys>.instance.FucIsUnlock(type))
            {
                txtObj.CustomSetActive(false);
                coinTextObj.CustomSetActive(true);
                CUICommonSystem.SetButtonEnableWithShader(btn, true, true);
                btn.GetComponent<CUIEventScript>().m_onClickEventParams.tag = (int) entryType;
            }
            else
            {
                txtObj.CustomSetActive(true);
                coinTextObj.CustomSetActive(false);
                CUICommonSystem.SetButtonEnableWithShader(btn, false, true);
            }
        }

        public bool IsHaveEntertrainModeOpen()
        {
            uint[] numArray = new uint[3];
            uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_PVP_Fire"), out numArray[0]);
            uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_PVP_5V5Clone"), out numArray[1]);
            uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_PVP_5V5CD"), out numArray[2]);
            for (int i = 0; i < 3; i++)
            {
                if (CUICommonSystem.GetMatchOpenState(RES_BATTLE_MAP_TYPE.RES_BATTLE_MAP_TYPE_ENTERTAINMENT, numArray[i]).matchState == enMatchOpenState.enMatchOpen_InActiveTime)
                {
                    return true;
                }
            }
            return false;
        }

        public void OnCheckFirstWin(CUIEvent uiEvent)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CLobbySystem.LOBBY_FORM_PATH);
            if ((form != null) && (masterRoleInfo != null))
            {
                Transform transform = form.get_transform().Find("Award");
                Text component = transform.Find("lblFirstWin").GetComponent<Text>();
                Image image = transform.Find("Icon").GetComponent<Image>();
                CUITimerScript script2 = transform.Find("Timer").GetComponent<CUITimerScript>();
                if (!masterRoleInfo.IsFirstWinOpen())
                {
                    transform.get_gameObject().CustomSetActive(false);
                }
                else
                {
                    transform.get_gameObject().CustomSetActive(true);
                    float curFirstWinRemainingTimeSec = masterRoleInfo.GetCurFirstWinRemainingTimeSec();
                    if (curFirstWinRemainingTimeSec <= 0f)
                    {
                        component.set_text(Singleton<CTextManager>.GetInstance().GetText("Daily_Quest_FirstVictory"));
                        image.set_color(CUIUtility.s_Color_White);
                        script2.get_gameObject().CustomSetActive(false);
                    }
                    else
                    {
                        component.set_text(Singleton<CTextManager>.GetInstance().GetText("Daily_Quest_FirstVictoryCD"));
                        image.set_color(CUIUtility.s_Color_GrayShader);
                        script2.get_gameObject().CustomSetActive(true);
                        script2.SetTotalTime(curFirstWinRemainingTimeSec);
                        script2.StartTimer();
                    }
                }
            }
        }

        public void OnCheckMiShuTalk(CUIEvent uiEvent)
        {
            this.CheckMiShuTalk(true);
        }

        private void OnClickGotoEntry(CUIEvent uiEvent)
        {
            CUICommonSystem.JumpForm((RES_GAME_ENTRANCE_TYPE) uiEvent.m_eventParams.tag, 0, 0);
        }

        private void OnClickMiShu(CUIEvent uiEvent)
        {
            CUIEvent event2 = new CUIEvent();
            event2.m_eventID = enUIEventID.Task_OpenForm;
            event2.m_eventParams.tag = 1;
            if (Singleton<CTaskSys>.instance.model.IsShowMainTaskTab_RedDotCount())
            {
                event2.m_eventParams.tag = 0;
            }
            Singleton<CUIEventManager>.instance.DispatchUIEvent(event2);
            SendUIClickToServer(enUIClickReprotID.rp_MishuBtn);
            MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.onEnterMishu, new uint[0]);
            Singleton<CUINewFlagSystem>.GetInstance().HideNewFlagForMishuEntry();
        }

        public void OnCloseTalk(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CLobbySystem.LOBBY_FORM_PATH);
            if (form != null)
            {
                Transform transform = form.get_transform().Find("LobbyBottom/Newbie/TalkFrame");
                CUITimerScript component = form.get_transform().Find("LobbyBottom/Newbie/TalkFrame/Timer").GetComponent<CUITimerScript>();
                transform.get_gameObject().CustomSetActive(false);
                component.EndTimer();
            }
        }

        public void OnPlayMishuAnimation(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CLobbySystem.LOBBY_FORM_PATH);
            if (form != null)
            {
                Transform transform = form.get_transform().Find("LobbyBottom/Newbie/Image_DaJi");
                if (transform != null)
                {
                    CUICommonSystem.PlayAnimator(transform.get_gameObject(), "Blink_0" + Random.Range(1, 3));
                }
            }
        }

        public void refreshEntertainMentOpenStateUI(GameObject btn, uint mapId)
        {
            if (btn != null)
            {
                Transform transform = btn.get_transform();
                if (transform != null)
                {
                    Transform transform2 = transform.FindChild("Lock");
                    Transform transform3 = transform.FindChild("Open");
                    Transform transform4 = transform.FindChild("NotOpen");
                    stMatchOpenInfo matchOpenState = CUICommonSystem.GetMatchOpenState(RES_BATTLE_MAP_TYPE.RES_BATTLE_MAP_TYPE_ENTERTAINMENT, mapId);
                    CUIMiniEventScript component = transform.GetComponent<CUIMiniEventScript>();
                    if (((transform2 != null) && (transform3 != null)) && (component != null))
                    {
                        transform.get_gameObject().CustomSetActive(false);
                        transform2.get_gameObject().CustomSetActive(false);
                        transform3.get_gameObject().CustomSetActive(false);
                        if (transform4 != null)
                        {
                            transform4.get_gameObject().CustomSetActive(false);
                        }
                        component.m_onClickEventParams.commonBool = false;
                        if (matchOpenState.matchState == enMatchOpenState.enMatchOpen_InActiveTime)
                        {
                            CUICommonSystem.SetTextContent(transform3.Find("Text").get_gameObject(), matchOpenState.descStr);
                            transform3.get_gameObject().CustomSetActive(true);
                            transform.get_gameObject().CustomSetActive(true);
                            component.m_onClickEventParams.commonBool = true;
                            if (CSysDynamicBlock.bLobbyEntryBlocked)
                            {
                                Transform transform5 = transform3.FindChild("Image");
                                Transform transform6 = transform3.FindChild("TextOpen");
                                if ((transform5 != null) && (transform6 != null))
                                {
                                    transform5.get_gameObject().CustomSetActive(false);
                                    transform6.get_gameObject().CustomSetActive(false);
                                }
                            }
                        }
                        else if (matchOpenState.matchState == enMatchOpenState.enMatchOpen_NotInActiveTime)
                        {
                            CUICommonSystem.SetTextContent(transform2.Find("Text").get_gameObject(), matchOpenState.descStr);
                            transform2.get_gameObject().CustomSetActive(true);
                            transform.get_gameObject().CustomSetActive(true);
                        }
                        else if (transform4 != null)
                        {
                            transform.get_gameObject().CustomSetActive(true);
                            transform4.get_gameObject().CustomSetActive(true);
                        }
                    }
                }
            }
        }

        public static void SendReqCoinGetPathData()
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x502);
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        public static void SendUIClickToServer(enUIClickReprotID clickID)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x138a);
            msg.stPkgData.stCltActionStatistics.bActionType = 1;
            msg.stPkgData.stCltActionStatistics.stActionData.construct((long) msg.stPkgData.stCltActionStatistics.bActionType);
            msg.stPkgData.stCltActionStatistics.stActionData.stSecretary.iID = (int) clickID;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }
    }
}

