namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.UI;
    using CSProtocol;
    using System;
    using UnityEngine.UI;

    public class IntimacyRelationView
    {
        public const string FRDataChange = "FRDataChange";
        public CUIListScript listScript;

        public void Clear()
        {
            this.listScript = null;
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.IntimacyRela_Menu_Close, new CUIEventManager.OnUIEventHandler(this.On_IntimacyRela_Menu_Close));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.IntimacyRela_Show_Drop_List, new CUIEventManager.OnUIEventHandler(this.On_IntimacyRela_Show_Drop_List));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.IntimacyRela_Drop_ListElement_Click, new CUIEventManager.OnUIEventHandler(this.On_IntimacyRela_Drop_ListElement_Click));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.IntimacyRela_Drop_ListElement_Enable, new CUIEventManager.OnUIEventHandler(this.On_Drop_ListElement_Enable));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.IntimacyRela_OK, new CUIEventManager.OnUIEventHandler(this.On_IntimacyRela_OK));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.IntimacyRela_Cancle, new CUIEventManager.OnUIEventHandler(this.On_IntimacyRela_Cancle));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.IntimacyRela_Item_Enable, new CUIEventManager.OnUIEventHandler(this.On_IntimacyRelation_Item_Enable));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.IntimacyRela_FristChoise, new CUIEventManager.OnUIEventHandler(this.On_IntimacyRela_FristChoise));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.IntimacyRela_PrevState, new CUIEventManager.OnUIEventHandler(this.On_IntimacyRela_PrevState));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler("FRDataChange", new Action(this, (IntPtr) this.On_FRDataChange));
            ListView<CFR> list = Singleton<CFriendContoller>.GetInstance().model.FRData.GetList();
            for (int i = 0; i < list.Count; i++)
            {
                CFR cfr = list[i];
                cfr.choiseRelation = -1;
                cfr.bInShowChoiseRelaList = false;
                cfr.bRedDot = false;
            }
            if (Singleton<CFriendContoller>.instance.view != null)
            {
                Singleton<CFriendContoller>.instance.view.Refresh();
            }
            Singleton<EventRouter>.instance.BroadCastEvent("Friend_LobbyIconRedDot_Refresh");
        }

        public void On_Drop_ListElement_Enable(CUIEvent uievent)
        {
            int srcWidgetIndexInBelongedList = uievent.m_srcWidgetIndexInBelongedList;
            CUIEventScript component = uievent.m_srcWidgetScript.GetComponent<CUIEventScript>().m_widgets[0].GetComponent<CUIComponent>().m_widgets[7].GetComponent<CUIEventScript>();
            ulong ulluid = component.m_onClickEventParams.commonUInt64Param1;
            uint tagUInt = component.m_onClickEventParams.tagUInt;
            if (Singleton<CFriendContoller>.instance.model.FRData.GetCfr(ulluid, tagUInt) != null)
            {
                Text text = uievent.m_srcWidget.get_transform().Find("Text").GetComponent<Text>();
                if (text != null)
                {
                    CFriendRelationship.FRConfig cFGByIndex = Singleton<CFriendContoller>.instance.model.FRData.GetCFGByIndex(srcWidgetIndexInBelongedList);
                    if (cFGByIndex != null)
                    {
                        text.set_text(cFGByIndex.cfgRelaStr);
                    }
                }
            }
        }

        private void On_FRDataChange()
        {
            this.Refresh();
        }

        private void On_IntimacyRela_Cancle(CUIEvent uievent)
        {
            ulong ulluid = uievent.m_eventParams.commonUInt64Param1;
            uint tagUInt = uievent.m_eventParams.tagUInt;
            COM_INTIMACY_STATE tag = (COM_INTIMACY_STATE) uievent.m_eventParams.tag;
            int num3 = uievent.m_eventParams.tag2;
            Singleton<CFriendContoller>.instance.model.FRData.ResetChoiseRelaState(ulluid, tagUInt);
            switch (tag)
            {
                case COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY_CONFIRM:
                    FriendRelationNetCore.Send_CHG_INTIMACY_DENY(ulluid, tagUInt, COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY, COM_INTIMACY_RELATION_CHG_TYPE.COM_INTIMACY_RELATION_ADD);
                    CFriendRelationship.FRData.Add(ulluid, tagUInt, COM_INTIMACY_STATE.COM_INTIMACY_STATE_VALUE_FULL, COM_INTIMACY_RELATION_CHG_TYPE.COM_INTIMACY_RELATION_NULL, 0, false);
                    break;

                case COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER_CONFIRM:
                    FriendRelationNetCore.Send_CHG_INTIMACY_DENY(ulluid, tagUInt, COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER, COM_INTIMACY_RELATION_CHG_TYPE.COM_INTIMACY_RELATION_ADD);
                    CFriendRelationship.FRData.Add(ulluid, tagUInt, COM_INTIMACY_STATE.COM_INTIMACY_STATE_VALUE_FULL, COM_INTIMACY_RELATION_CHG_TYPE.COM_INTIMACY_RELATION_NULL, 0, false);
                    break;

                case COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY_DENY:
                    FriendRelationNetCore.Send_CHG_INTIMACY_DENY(ulluid, tagUInt, COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY, COM_INTIMACY_RELATION_CHG_TYPE.COM_INTIMACY_RELATION_DEL);
                    CFriendRelationship.FRData.Add(ulluid, tagUInt, COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY, COM_INTIMACY_RELATION_CHG_TYPE.COM_INTIMACY_RELATION_NULL, 0, false);
                    break;

                case COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER_DENY:
                    FriendRelationNetCore.Send_CHG_INTIMACY_DENY(ulluid, tagUInt, COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER, COM_INTIMACY_RELATION_CHG_TYPE.COM_INTIMACY_RELATION_DEL);
                    CFriendRelationship.FRData.Add(ulluid, tagUInt, COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER, COM_INTIMACY_RELATION_CHG_TYPE.COM_INTIMACY_RELATION_NULL, 0, false);
                    break;
            }
        }

        private void On_IntimacyRela_Delete_Relation(CUIEvent uievent)
        {
            ulong ulluid = uievent.m_eventParams.commonUInt64Param1;
            uint tagUInt = uievent.m_eventParams.tagUInt;
            CFR cfr = Singleton<CFriendContoller>.instance.model.FRData.GetCfr(ulluid, tagUInt);
            if (cfr != null)
            {
                if (cfr.state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY)
                {
                    FriendRelationNetCore.Send_INTIMACY_RELATION_REQUEST(ulluid, tagUInt, COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY, COM_INTIMACY_RELATION_CHG_TYPE.COM_INTIMACY_RELATION_DEL);
                }
                if (cfr.state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER)
                {
                    FriendRelationNetCore.Send_INTIMACY_RELATION_REQUEST(ulluid, tagUInt, COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER, COM_INTIMACY_RELATION_CHG_TYPE.COM_INTIMACY_RELATION_DEL);
                }
            }
        }

        private void On_IntimacyRela_Drop_ListElement_Click(CUIEvent uievent)
        {
            CUIComponent com = uievent.m_srcWidgetScript.m_widgets[0].GetComponent<CUIComponent>();
            CUIEventScript component = com.m_widgets[7].GetComponent<CUIEventScript>();
            ulong ulluid = component.m_onClickEventParams.commonUInt64Param1;
            uint tagUInt = component.m_onClickEventParams.tagUInt;
            CFR frData = Singleton<CFriendContoller>.instance.model.FRData.GetCfr(ulluid, tagUInt);
            if (frData != null)
            {
                frData.bInShowChoiseRelaList = false;
                frData.choiseRelation = uievent.m_srcWidgetIndexInBelongedList;
                IntimacyRelationViewUT.Show_Item(com, frData);
            }
        }

        private void On_IntimacyRela_FristChoise(CUIEvent uievent)
        {
            ulong ulluid = uievent.m_eventParams.commonUInt64Param1;
            uint tagUInt = uievent.m_eventParams.tagUInt;
            CFR cfr = Singleton<CFriendContoller>.instance.model.FRData.GetCfr(ulluid, tagUInt);
            if (cfr != null)
            {
                FriendRelationNetCore.Send_CHG_INTIMACYPRIORITY(cfr.state);
            }
        }

        private void On_IntimacyRela_Menu_Close(CUIEvent uievent)
        {
            this.Clear();
        }

        private void On_IntimacyRela_OK(CUIEvent uievent)
        {
            ulong ulluid = uievent.m_eventParams.commonUInt64Param1;
            uint tagUInt = uievent.m_eventParams.tagUInt;
            COM_INTIMACY_STATE tag = (COM_INTIMACY_STATE) uievent.m_eventParams.tag;
            int index = uievent.m_eventParams.tag2;
            if ((tag == COM_INTIMACY_STATE.COM_INTIMACY_STATE_VALUE_FULL) && (index != -1))
            {
                CFriendRelationship fRData = Singleton<CFriendContoller>.instance.model.FRData;
                CFriendRelationship.FRConfig cFGByIndex = fRData.GetCFGByIndex(index);
                if (cFGByIndex.state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY)
                {
                    if (fRData.FindFrist(COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY) == null)
                    {
                        FriendRelationNetCore.Send_INTIMACY_RELATION_REQUEST(ulluid, tagUInt, COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY, COM_INTIMACY_RELATION_CHG_TYPE.COM_INTIMACY_RELATION_ADD);
                        Singleton<CFriendContoller>.instance.model.FRData.ResetChoiseRelaState(ulluid, tagUInt);
                    }
                    else
                    {
                        Singleton<CUIManager>.instance.OpenTips(UT.FRData().IntimRela_Tips_AlreadyHasGay, true, 1.5f, null, new object[0]);
                    }
                }
                if (cFGByIndex.state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER)
                {
                    if (fRData.FindFrist(COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER) == null)
                    {
                        FriendRelationNetCore.Send_INTIMACY_RELATION_REQUEST(ulluid, tagUInt, COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER, COM_INTIMACY_RELATION_CHG_TYPE.COM_INTIMACY_RELATION_ADD);
                        Singleton<CFriendContoller>.instance.model.FRData.ResetChoiseRelaState(ulluid, tagUInt);
                    }
                    else
                    {
                        Singleton<CUIManager>.instance.OpenTips(UT.FRData().IntimRela_Tips_AlreadyHasLover, true, 1.5f, null, new object[0]);
                    }
                }
            }
            else
            {
                switch (tag)
                {
                    case COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY_CONFIRM:
                        FriendRelationNetCore.Send_CHG_INTIMACY_CONFIRM(ulluid, tagUInt, COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY, COM_INTIMACY_RELATION_CHG_TYPE.COM_INTIMACY_RELATION_ADD);
                        return;

                    case COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER_CONFIRM:
                        FriendRelationNetCore.Send_CHG_INTIMACY_CONFIRM(ulluid, tagUInt, COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER, COM_INTIMACY_RELATION_CHG_TYPE.COM_INTIMACY_RELATION_ADD);
                        return;

                    case COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY_DENY:
                        FriendRelationNetCore.Send_CHG_INTIMACY_CONFIRM(ulluid, tagUInt, COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY, COM_INTIMACY_RELATION_CHG_TYPE.COM_INTIMACY_RELATION_DEL);
                        return;

                    case COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER_DENY:
                        FriendRelationNetCore.Send_CHG_INTIMACY_CONFIRM(ulluid, tagUInt, COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER, COM_INTIMACY_RELATION_CHG_TYPE.COM_INTIMACY_RELATION_DEL);
                        return;
                }
            }
        }

        private void On_IntimacyRela_PrevState(CUIEvent uievent)
        {
            ulong ulluid = uievent.m_eventParams.commonUInt64Param1;
            uint tagUInt = uievent.m_eventParams.tagUInt;
            CFR cfr = Singleton<CFriendContoller>.instance.model.FRData.GetCfr(ulluid, tagUInt);
            if ((cfr.state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY_CONFIRM) && !cfr.bReciveOthersRequest)
            {
                cfr.state = COM_INTIMACY_STATE.COM_INTIMACY_STATE_VALUE_FULL;
            }
            if ((cfr.state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER_CONFIRM) && !cfr.bReciveOthersRequest)
            {
                cfr.state = COM_INTIMACY_STATE.COM_INTIMACY_STATE_VALUE_FULL;
            }
            if ((cfr.state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER_DENY) && !cfr.bReciveOthersRequest)
            {
                cfr.state = COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER;
            }
            if ((cfr.state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY_DENY) && !cfr.bReciveOthersRequest)
            {
                cfr.state = COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY;
            }
            Singleton<EventRouter>.GetInstance().BroadCastEvent("FRDataChange");
        }

        private void On_IntimacyRela_Show_Drop_List(CUIEvent uievent)
        {
            ulong ulluid = uievent.m_eventParams.commonUInt64Param1;
            uint tagUInt = uievent.m_eventParams.tagUInt;
            CFR frData = Singleton<CFriendContoller>.instance.model.FRData.GetCfr(ulluid, tagUInt);
            if (frData != null)
            {
                frData.bInShowChoiseRelaList = !frData.bInShowChoiseRelaList;
                CUIComponent com = uievent.m_srcWidgetScript.m_widgets[0].GetComponent<CUIComponent>();
                CUIListScript component = uievent.m_srcWidgetScript.m_widgets[1].GetComponent<CUIListScript>();
                if (component != null)
                {
                    component.SetElementAmount(2);
                }
                IntimacyRelationViewUT.Show_Item(com, frData);
            }
        }

        public void On_IntimacyRelation_Item_Enable(CUIEvent uievent)
        {
            if (uievent != null)
            {
                int srcWidgetIndexInBelongedList = uievent.m_srcWidgetIndexInBelongedList;
                ListView<CFR> list = Singleton<CFriendContoller>.GetInstance().model.FRData.GetList();
                if (list != null)
                {
                    CFR frData = null;
                    if ((srcWidgetIndexInBelongedList >= 0) && (srcWidgetIndexInBelongedList < list.Count))
                    {
                        frData = list[srcWidgetIndexInBelongedList];
                    }
                    if ((frData != null) && (uievent.m_srcWidget != null))
                    {
                        CUIComponent com = uievent.m_srcWidget.GetComponent<CUIComponent>();
                        if (com != null)
                        {
                            this.ShowIntimacyRelation_Item(com, frData);
                        }
                    }
                }
            }
        }

        public void Open()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.IntimacyRela_Menu_Close, new CUIEventManager.OnUIEventHandler(this.On_IntimacyRela_Menu_Close));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.IntimacyRela_Show_Drop_List, new CUIEventManager.OnUIEventHandler(this.On_IntimacyRela_Show_Drop_List));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.IntimacyRela_Drop_ListElement_Click, new CUIEventManager.OnUIEventHandler(this.On_IntimacyRela_Drop_ListElement_Click));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.IntimacyRela_Drop_ListElement_Enable, new CUIEventManager.OnUIEventHandler(this.On_Drop_ListElement_Enable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.IntimacyRela_OK, new CUIEventManager.OnUIEventHandler(this.On_IntimacyRela_OK));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.IntimacyRela_Cancle, new CUIEventManager.OnUIEventHandler(this.On_IntimacyRela_Cancle));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.IntimacyRela_Item_Enable, new CUIEventManager.OnUIEventHandler(this.On_IntimacyRelation_Item_Enable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.IntimacyRela_FristChoise, new CUIEventManager.OnUIEventHandler(this.On_IntimacyRela_FristChoise));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.IntimacyRela_Delete_Relation, new CUIEventManager.OnUIEventHandler(this.On_IntimacyRela_Delete_Relation));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.IntimacyRela_PrevState, new CUIEventManager.OnUIEventHandler(this.On_IntimacyRela_PrevState));
            Singleton<EventRouter>.GetInstance().AddEventHandler("FRDataChange", new Action(this, (IntPtr) this.On_FRDataChange));
            CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(CFriendContoller.IntimacyRelaFormPath, false, true);
            this.listScript = script.get_transform().FindChild("GameObject/list").GetComponent<CUIListScript>();
            script.get_transform().FindChild("GameObject/info/txt").GetComponent<Text>().set_text(UT.FRData().IntimRela_EmptyDataText);
            this.Refresh();
        }

        public void Refresh()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CFriendContoller.IntimacyRelaFormPath);
            if (form != null)
            {
                if (Singleton<CFriendContoller>.GetInstance().model.FRData.GetList().Count == 0)
                {
                    form.get_transform().FindChild("GameObject/info").get_gameObject().CustomSetActive(true);
                    form.get_transform().FindChild("GameObject/list").get_gameObject().CustomSetActive(false);
                }
                else
                {
                    form.get_transform().FindChild("GameObject/info").get_gameObject().CustomSetActive(false);
                    form.get_transform().FindChild("GameObject/list").get_gameObject().CustomSetActive(true);
                    this.Refresh_IntimacyRelation_List();
                }
            }
        }

        public void Refresh_IntimacyRelation_List()
        {
            ListView<CFR> list = Singleton<CFriendContoller>.GetInstance().model.FRData.GetList();
            this.listScript.SetElementAmount(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                CUIListElementScript elemenet = this.listScript.GetElemenet(i);
                this.ShowIntimacyRelation_Item(elemenet, list[i]);
            }
        }

        public void ShowIntimacyRelation_Item(CUIComponent com, CFR frData)
        {
            if ((com != null) && (frData != null))
            {
                IntimacyRelationViewUT.Show_Item(com, frData);
            }
        }
    }
}

