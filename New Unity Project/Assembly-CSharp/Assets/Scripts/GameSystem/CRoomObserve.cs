namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.UI;
    using CSProtocol;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public static class CRoomObserve
    {
        private static void OnClickFold(CUIEvent evt)
        {
            Animator componentInParent = evt.m_srcWidget.GetComponentInParent<Animator>();
            if (componentInParent != null)
            {
                CUIEventScript component = evt.m_srcWidget.GetComponent<CUIEventScript>();
                if (component.m_onClickEventParams.tag == 0)
                {
                    componentInParent.Play("Open");
                    component.m_onClickEventParams.tag = 1;
                }
                else
                {
                    componentInParent.Play("Close");
                    component.m_onClickEventParams.tag = 0;
                }
            }
        }

        private static void OnClickKick(CUIEvent evt)
        {
            CRoomSystem.ReqKickPlayer(COM_PLAYERCAMP.COM_PLAYERCAMP_MID, evt.m_eventParams.tag);
        }

        private static void OnClickSeat(CUIEvent evt)
        {
            CRoomSystem.ReqChangeCamp(COM_PLAYERCAMP.COM_PLAYERCAMP_MID, evt.m_eventParams.tag, COM_CHGROOMPOS_TYPE.COM_CHGROOMPOS_EMPTY);
        }

        private static void OnClickSwap(CUIEvent evt)
        {
            CRoomSystem.ReqChangeCamp(COM_PLAYERCAMP.COM_PLAYERCAMP_MID, evt.m_eventParams.tag, COM_CHGROOMPOS_TYPE.COM_CHGROOMPOS_PLAYER);
        }

        public static void RegisterEvents()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Room_Observe_Fold, new CUIEventManager.OnUIEventHandler(CRoomObserve.OnClickFold));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Room_Observe_Seat, new CUIEventManager.OnUIEventHandler(CRoomObserve.OnClickSeat));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Room_Observe_Kick, new CUIEventManager.OnUIEventHandler(CRoomObserve.OnClickKick));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Room_Observe_Swap, new CUIEventManager.OnUIEventHandler(CRoomObserve.OnClickSwap));
        }

        public static void SetObservers(GameObject root, int maxNum, ListView<MemberInfo> memberList, MemberInfo masterMember)
        {
            if (maxNum > 0)
            {
                root.CustomSetActive(true);
                CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(root, "SlotList");
                componetInChild.SetElementAmount(maxNum);
                MemberInfo[] infoArray = new MemberInfo[maxNum];
                for (int i = 0; i < memberList.Count; i++)
                {
                    MemberInfo info = memberList[i];
                    if ((info != null) && (info.dwPosOfCamp < maxNum))
                    {
                        infoArray[info.dwPosOfCamp] = info;
                    }
                }
                int num2 = 0;
                for (int j = 0; j < maxNum; j++)
                {
                    MemberInfo info2 = infoArray[j];
                    GameObject p = componetInChild.GetElemenet(j).get_gameObject();
                    bool bActive = null != info2;
                    CUIHttpImageScript script2 = Utility.GetComponetInChild<CUIHttpImageScript>(p, "Icon");
                    Text text = Utility.GetComponetInChild<Text>(p, "Name");
                    CUIEventScript script3 = Utility.GetComponetInChild<CUIEventScript>(p, "SitDown");
                    CUIEventScript script4 = Utility.GetComponetInChild<CUIEventScript>(p, "KickOut");
                    script2.get_gameObject().CustomSetActive(bActive);
                    text.get_gameObject().CustomSetActive(bActive);
                    script3.get_gameObject().CustomSetActive(!bActive);
                    script4.get_gameObject().CustomSetActive((bActive && (info2 != masterMember)) && Singleton<CRoomSystem>.GetInstance().IsSelfRoomOwner);
                    if (bActive)
                    {
                        text.set_text(info2.MemberName);
                        script2.SetImageUrl(Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(info2.MemberHeadUrl));
                        num2++;
                    }
                    if (script3.get_gameObject().get_activeSelf())
                    {
                        script3.m_onClickEventID = enUIEventID.Room_Observe_Seat;
                        script3.m_onClickEventParams.tag = j;
                    }
                    if (script4.get_gameObject().get_activeSelf())
                    {
                        script4.m_onClickEventID = enUIEventID.Room_Observe_Kick;
                        script4.m_onClickEventParams.tag = j;
                    }
                }
                Utility.GetComponetInChild<Text>(root, "PersonCount").set_text(num2 + "/" + maxNum);
            }
            else
            {
                root.CustomSetActive(false);
            }
        }

        public static void UnRegisterEvents()
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Room_Observe_Fold, new CUIEventManager.OnUIEventHandler(CRoomObserve.OnClickFold));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Room_Observe_Seat, new CUIEventManager.OnUIEventHandler(CRoomObserve.OnClickSeat));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Room_Observe_Kick, new CUIEventManager.OnUIEventHandler(CRoomObserve.OnClickKick));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Room_Observe_Swap, new CUIEventManager.OnUIEventHandler(CRoomObserve.OnClickSwap));
        }
    }
}

