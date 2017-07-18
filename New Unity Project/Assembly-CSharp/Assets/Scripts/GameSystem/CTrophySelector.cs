namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [MessageHandlerClass]
    public class CTrophySelector : Singleton<CTrophySelector>
    {
        private ListView<CAchieveItem2> m_CurTrophies;
        public CAchieveItem2 NewTrophy;
        public CAchieveItem2[] SelectedTrophies;
        public const string sSelectorFormPath = "UGUI/Form/System/Achieve/Form_Trophy_Select.prefab";
        public byte TargetReplaceIdx;

        public override void Init()
        {
            base.Init();
            this.m_CurTrophies = new ListView<CAchieveItem2>();
            this.SelectedTrophies = new CAchieveItem2[3];
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Achievement_Change_Selected_Trophy, new CUIEventManager.OnUIEventHandler(this.OnChangeAchievement));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Achievement_Selector_Trophy_Enable, new CUIEventManager.OnUIEventHandler(this.OnTrophyEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Achievement_Selector_Trophy_Select, new CUIEventManager.OnUIEventHandler(this.OnTrophySelectChange));
        }

        private void OnChangeAchievement(CUIEvent uiEvent)
        {
            CAchieveInfo2 masterAchieveInfo = CAchieveInfo2.GetMasterAchieveInfo();
            ListView<CAchieveItem2> view = new ListView<CAchieveItem2>();
            this.m_CurTrophies = masterAchieveInfo.GetTrophies(enTrophyState.Finish);
            for (int i = this.m_CurTrophies.Count - 1; i >= 0; i--)
            {
                if ((this.m_CurTrophies[i] != null) && (Array.IndexOf<CAchieveItem2>(this.SelectedTrophies, this.m_CurTrophies[i]) < 0))
                {
                    view.Add(this.m_CurTrophies[i]);
                }
            }
            if (view.Count == 0)
            {
                Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Achievement_Trophy_Select_Err_1"), false, 1.5f, null, new object[0]);
            }
            else
            {
                if ((uiEvent.m_eventParams.tag >= 0) && (uiEvent.m_eventParams.tag < this.SelectedTrophies.Length))
                {
                    this.TargetReplaceIdx = (byte) uiEvent.m_eventParams.tag;
                }
                else
                {
                    Singleton<CUIManager>.GetInstance().OpenTips("数据异常，请稍后重试", false, 1.5f, null, new object[0]);
                    return;
                }
                CUIFormScript form = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/System/Achieve/Form_Trophy_Select.prefab", false, true);
                this.RefreshAchievementSelectForm(form);
            }
        }

        private void OnTrophyEnable(CUIEvent uiEvent)
        {
            int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
            if ((srcWidgetIndexInBelongedList >= 0) && (srcWidgetIndexInBelongedList < this.m_CurTrophies.Count))
            {
                CUIListElementScript srcWidgetScript = uiEvent.m_srcWidgetScript as CUIListElementScript;
                if (srcWidgetScript == null)
                {
                    DebugHelper.Assert(false, "achievement selector sery enable elementscript is null");
                }
                else
                {
                    CAchieveItem2 item = this.m_CurTrophies[srcWidgetIndexInBelongedList];
                    GameObject widget = srcWidgetScript.GetWidget(0);
                    GameObject obj3 = srcWidgetScript.GetWidget(1);
                    GameObject obj4 = srcWidgetScript.GetWidget(2);
                    GameObject obj5 = srcWidgetScript.GetWidget(3);
                    GameObject obj6 = srcWidgetScript.GetWidget(4);
                    if (Array.IndexOf<CAchieveItem2>(this.SelectedTrophies, item) >= 0)
                    {
                        obj3.CustomSetActive(true);
                    }
                    else
                    {
                        obj3.CustomSetActive(false);
                    }
                    Image component = widget.GetComponent<Image>();
                    Image image2 = obj6.GetComponent<Image>();
                    Text text = obj4.GetComponent<Text>();
                    Text text2 = obj5.GetComponent<Text>();
                    if (((component != null) && (image2 != null)) && ((text != null) && (text2 != null)))
                    {
                        CAchieveItem2 achieveItem = item.TryToGetMostRecentlyDoneItem();
                        if (achieveItem == null)
                        {
                            component.SetSprite(CUIUtility.GetSpritePrefeb(item.GetAchieveImagePath(), false, false), false);
                            CAchievementSystem.SetAchieveBaseIcon(obj6.get_transform(), item, null);
                            text.set_text(item.Cfg.szName);
                            text2.set_text(item.GetGotTimeText(false, false));
                        }
                        else
                        {
                            component.SetSprite(CUIUtility.GetSpritePrefeb(achieveItem.GetAchieveImagePath(), false, false), false);
                            CAchievementSystem.SetAchieveBaseIcon(obj6.get_transform(), achieveItem, null);
                            text.set_text(achieveItem.Cfg.szName);
                            text2.set_text(achieveItem.GetGotTimeText(false, false));
                        }
                    }
                }
            }
        }

        private void OnTrophySelectChange(CUIEvent uiEvent)
        {
            CUIListScript srcWidgetScript = uiEvent.m_srcWidgetScript as CUIListScript;
            if (srcWidgetScript != null)
            {
                int selectedIndex = srcWidgetScript.GetSelectedIndex();
                if (((uiEvent.m_srcFormScript != null) && (selectedIndex >= 0)) && (selectedIndex < this.m_CurTrophies.Count))
                {
                    CAchieveItem2 item = this.m_CurTrophies[selectedIndex];
                    if ((item != null) && (Array.IndexOf<CAchieveItem2>(this.SelectedTrophies, item) < 0))
                    {
                        this.NewTrophy = item;
                        CAchieveItem2 item2 = this.NewTrophy.TryToGetMostRecentlyDoneItem();
                        if (item2 != null)
                        {
                            this.SendChgAchieveReq(item2.ID, this.TargetReplaceIdx);
                        }
                        else
                        {
                            Singleton<CUIManager>.GetInstance().OpenTips(string.Format("{0}{1}", Singleton<CTextManager>.GetInstance().GetText("Achievement_Trophy_Select_Err_2"), -4), false, 1.5f, null, new object[0]);
                        }
                    }
                }
            }
        }

        [MessageHandler(0x113e)]
        public static void ReceiveChgTrophyRsp(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            if (msg.stPkgData.stRspAchieveShow.iResult < 0)
            {
                Singleton<CUIManager>.GetInstance().CloseForm("UGUI/Form/System/Achieve/Form_Trophy_Select.prefab");
                Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Achievement_Trophy_Select_Err_3"), false, 1.5f, null, new object[0]);
            }
            else
            {
                CTrophySelector instance = Singleton<CTrophySelector>.GetInstance();
                Singleton<CUIManager>.GetInstance().CloseForm("UGUI/Form/System/Achieve/Form_Trophy_Select.prefab");
                Singleton<EventRouter>.GetInstance().BroadCastEvent<byte, CAchieveItem2>(EventID.ACHIEVE_SERY_SELECT_DONE, msg.stPkgData.stRspAchieveShow.bIndex, instance.NewTrophy);
            }
        }

        private void RefreshAchievementSelectForm(CUIFormScript form)
        {
            if (form != null)
            {
                form.GetWidget(0).GetComponent<CUIListScript>().SetElementAmount(this.m_CurTrophies.Count);
            }
        }

        private void SendChgAchieveReq(uint id, byte idx)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x113d);
            msg.stPkgData.stReqAchieveShow.dwAchieveID = id;
            msg.stPkgData.stReqAchieveShow.bIndex = idx;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public override void UnInit()
        {
            base.UnInit();
            this.m_CurTrophies.RemoveRange(0, this.m_CurTrophies.Count);
            this.m_CurTrophies = null;
            this.SelectedTrophies = null;
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Achievement_Change_Selected_Trophy, new CUIEventManager.OnUIEventHandler(this.OnChangeAchievement));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Achievement_Selector_Trophy_Enable, new CUIEventManager.OnUIEventHandler(this.OnTrophyEnable));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Achievement_Selector_Trophy_Select, new CUIEventManager.OnUIEventHandler(this.OnTrophySelectChange));
        }
    }
}

