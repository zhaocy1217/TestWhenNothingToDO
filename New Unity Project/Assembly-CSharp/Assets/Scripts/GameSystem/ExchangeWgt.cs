namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class ExchangeWgt : ActivityWidget
    {
        private ListView<ExchangeElement> _elementList;
        private GameObject _elementTmpl;
        private const float SpacingY = 5f;

        public ExchangeWgt(GameObject node, ActivityView view) : base(node, view)
        {
            ListView<ActivityPhase> phaseList = view.activity.PhaseList;
            this._elementList = new ListView<ExchangeElement>();
            this._elementTmpl = Utility.FindChild(node, "Template");
            float num = this._elementTmpl.GetComponent<RectTransform>().get_rect().get_height();
            for (int i = 0; i < phaseList.Count; i++)
            {
                GameObject uiItem = null;
                uiItem = Object.Instantiate(this._elementTmpl);
                if (uiItem != null)
                {
                    uiItem.GetComponent<RectTransform>().set_sizeDelta(this._elementTmpl.GetComponent<RectTransform>().get_sizeDelta());
                    uiItem.get_transform().SetParent(this._elementTmpl.get_transform().get_parent());
                    uiItem.get_transform().set_localPosition(this._elementTmpl.get_transform().get_localPosition() + new Vector3(0f, -(num + 5f) * i, 0f));
                    uiItem.get_transform().set_localScale(Vector3.get_one());
                    uiItem.get_transform().set_localRotation(Quaternion.get_identity());
                    this._elementList.Add(new ExchangeElement(phaseList[(phaseList.Count - i) - 1] as ExchangePhase, uiItem, this, i));
                }
            }
            node.GetComponent<RectTransform>().set_sizeDelta(new Vector2(node.GetComponent<RectTransform>().get_sizeDelta().x, (num * phaseList.Count) + ((phaseList.Count - 1) * 5f)));
            this._elementTmpl.CustomSetActive(false);
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Activity_Exchange, new CUIEventManager.OnUIEventHandler(this.OnClickExchange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Activity_ExchangeConfirm, new CUIEventManager.OnUIEventHandler(this.OnClickExchangeConfirm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Activity_ExchangeCountReady, new CUIEventManager.OnUIEventHandler(this.OnClickExchangeCountSelectReady));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Activity_ExchangeHeroSkinConfirm, new CUIEventManager.OnUIEventHandler(this.OnExchangeHeroSkinConfirm));
            view.activity.OnTimeStateChange += new Activity.ActivityEvent(this.OnStateChange);
        }

        public override void Clear()
        {
            base.view.activity.OnTimeStateChange -= new Activity.ActivityEvent(this.OnStateChange);
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Activity_Exchange, new CUIEventManager.OnUIEventHandler(this.OnClickExchange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Activity_ExchangeConfirm, new CUIEventManager.OnUIEventHandler(this.OnClickExchangeConfirm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Activity_ExchangeCountReady, new CUIEventManager.OnUIEventHandler(this.OnClickExchangeCountSelectReady));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Activity_ExchangeHeroSkinConfirm, new CUIEventManager.OnUIEventHandler(this.OnExchangeHeroSkinConfirm));
            if (this._elementList != null)
            {
                for (int i = 0; i < this._elementList.Count; i++)
                {
                    if (this._elementList[i].uiItem != null)
                    {
                        Object.Destroy(this._elementList[i].uiItem);
                    }
                }
                this._elementList = null;
            }
            this._elementTmpl = null;
        }

        private void OnClickExchange(CUIEvent uiEvent)
        {
            if (this._elementList != null)
            {
                int num = (int) uiEvent.m_eventParams.commonUInt32Param1;
                if ((num >= 0) && (num < this._elementList.Count))
                {
                    ResDT_Item_Info stResItemInfo = this._elementList[num].phase.Config.stResItemInfo;
                    CUseable useable = CUseableManager.CreateUseable((COM_ITEM_TYPE) stResItemInfo.wItemType, stResItemInfo.dwItemID, 1);
                    bool flag = false;
                    int num2 = 0;
                    if (useable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_HERO)
                    {
                        CHeroItem item = (CHeroItem) useable;
                        if (item != null)
                        {
                            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                            if (masterRoleInfo != null)
                            {
                                flag = masterRoleInfo.IsHaveHero(item.m_baseID, true);
                            }
                        }
                    }
                    else if (useable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN)
                    {
                        CHeroSkin skin = (CHeroSkin) useable;
                        if (skin != null)
                        {
                            CRoleInfo info3 = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                            if (info3 != null)
                            {
                                flag = info3.IsHaveHeroSkin(skin.m_heroId, skin.m_skinId, false);
                            }
                        }
                    }
                    else if (useable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
                    {
                        CItem item2 = (CItem) useable;
                        if (item2 != null)
                        {
                            CRoleInfo info4 = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                            if (((item2.m_itemData != null) && (info4 != null)) && (item2.m_itemData.bType == 4))
                            {
                                int num3 = (int) item2.m_itemData.EftParam[0];
                                if (num3 > 0)
                                {
                                    ResRandomRewardStore dataByKey = GameDataMgr.randomRewardDB.GetDataByKey((long) num3);
                                    if (dataByKey != null)
                                    {
                                        for (int i = 0; i < dataByKey.astRewardDetail.Length; i++)
                                        {
                                            if ((dataByKey.astRewardDetail[i].bItemType == 0) || (dataByKey.astRewardDetail[i].bItemType >= 0x12))
                                            {
                                                break;
                                            }
                                            if (dataByKey.astRewardDetail[i].bItemType == 4)
                                            {
                                                if (info4.IsHaveHero(dataByKey.astRewardDetail[i].dwItemID, true))
                                                {
                                                    num2 = 1;
                                                    goto Label_025E;
                                                }
                                                num2 = 0;
                                                break;
                                            }
                                            if (dataByKey.astRewardDetail[i].bItemType == 11)
                                            {
                                                if (info4.IsHaveHeroSkin(dataByKey.astRewardDetail[i].dwItemID, true))
                                                {
                                                    num2 = 1;
                                                    goto Label_025E;
                                                }
                                                num2 = 0;
                                                break;
                                            }
                                            if ((dataByKey.astRewardDetail[i].bItemType > 0) && (dataByKey.astRewardDetail[i].bItemType < 0x12))
                                            {
                                                num2 = 0;
                                            }
                                        Label_025E:;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (flag)
                    {
                        string strContent = string.Format(Singleton<CTextManager>.GetInstance().GetText("ExchangeWgt_Hero_Tips"), useable.m_name);
                        Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(strContent, enUIEventID.Activity_ExchangeHeroSkinConfirm, enUIEventID.None, uiEvent.m_eventParams, false);
                    }
                    else if (num2 == 1)
                    {
                        string text = Singleton<CTextManager>.GetInstance().GetText("ExchangeWgt_Have_AllGift");
                        uiEvent.m_eventParams.taskId = 1;
                        Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(text, enUIEventID.Activity_ExchangeHeroSkinConfirm, enUIEventID.None, uiEvent.m_eventParams, false);
                    }
                    else
                    {
                        int maxExchangeCount = this._elementList[num].phase.GetMaxExchangeCount();
                        if (maxExchangeCount > 1)
                        {
                            stUIEventParams par = new stUIEventParams();
                            par.commonUInt16Param1 = (ushort) num;
                            Singleton<CUIManager>.GetInstance().OpenExchangeCountSelectForm(useable, maxExchangeCount, enUIEventID.Activity_ExchangeCountReady, par, 0, 0);
                        }
                        else
                        {
                            string str3 = (useable != null) ? useable.m_name : string.Empty;
                            stUIEventParams params2 = new stUIEventParams();
                            params2.commonUInt16Param1 = (ushort) num;
                            this._elementList[num].phase.SetExchangeCountOnce(1);
                            Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(string.Format(Singleton<CTextManager>.GetInstance().GetText("confirmExchange"), maxExchangeCount, str3), enUIEventID.Activity_ExchangeConfirm, enUIEventID.None, params2, false);
                        }
                    }
                }
            }
        }

        private void OnClickExchangeConfirm(CUIEvent uiEvent)
        {
            if (this._elementList != null)
            {
                int num = uiEvent.m_eventParams.commonUInt16Param1;
                if ((num >= 0) && (num < this._elementList.Count))
                {
                    this._elementList[num].phase.DrawReward();
                }
            }
        }

        private void OnClickExchangeCountSelectReady(CUIEvent uiEvent)
        {
            if (this._elementList != null)
            {
                uint num = uiEvent.m_eventParams.commonUInt32Param1;
                int num2 = uiEvent.m_eventParams.commonUInt16Param1;
                if ((num2 >= 0) && (num2 < this._elementList.Count))
                {
                    ResDT_Item_Info stResItemInfo = this._elementList[num2].phase.Config.stResItemInfo;
                    CUseable useable = CUseableManager.CreateUseable((COM_ITEM_TYPE) stResItemInfo.wItemType, stResItemInfo.dwItemID, 1);
                    string str = (useable != null) ? useable.m_name : string.Empty;
                    stUIEventParams par = new stUIEventParams();
                    par.commonUInt16Param1 = (ushort) num2;
                    this._elementList[num2].phase.SetExchangeCountOnce((int) num);
                    Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(string.Format(Singleton<CTextManager>.GetInstance().GetText("confirmExchange"), num, str), enUIEventID.Activity_ExchangeConfirm, enUIEventID.None, par, false);
                }
            }
        }

        private void OnExchangeHeroSkinConfirm(CUIEvent uiEvent)
        {
            if (this._elementList != null)
            {
                int num = (int) uiEvent.m_eventParams.commonUInt32Param1;
                if ((num >= 0) && (num < this._elementList.Count))
                {
                    ResDT_Item_Info stResItemInfo = this._elementList[num].phase.Config.stResItemInfo;
                    CUseable item = CUseableManager.CreateUseable((COM_ITEM_TYPE) stResItemInfo.wItemType, stResItemInfo.dwItemID, 1);
                    int maxExchangeCount = this._elementList[num].phase.GetMaxExchangeCount();
                    if (maxExchangeCount > 1)
                    {
                        stUIEventParams par = new stUIEventParams();
                        par.commonUInt16Param1 = (ushort) num;
                        Singleton<CUIManager>.GetInstance().OpenExchangeCountSelectForm(item, maxExchangeCount, enUIEventID.Activity_ExchangeCountReady, par, 0, 0);
                    }
                    else
                    {
                        string str = (item != null) ? item.m_name : string.Empty;
                        stUIEventParams params2 = new stUIEventParams();
                        params2.commonUInt16Param1 = (ushort) num;
                        this._elementList[num].phase.SetExchangeCountOnce(1);
                        Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Activity_ExchangeConfirm, params2);
                    }
                }
            }
        }

        private void OnStateChange(Activity acty)
        {
            this.Validate();
        }

        public override void Validate()
        {
            if (this._elementList != null)
            {
                for (int i = 0; i < this._elementList.Count; i++)
                {
                    this._elementList[i].Validate();
                }
            }
        }

        private class ExchangeElement
        {
            public int index;
            public ExchangeWgt owner;
            public ExchangePhase phase;
            public GameObject uiItem;

            public ExchangeElement(ExchangePhase phase, GameObject uiItem, ExchangeWgt owner, int index)
            {
                this.phase = phase;
                this.uiItem = uiItem;
                this.owner = owner;
                this.index = index;
                this.Validate();
            }

            public void Validate()
            {
                if ((this.phase != null) && (this.uiItem != null))
                {
                    this.uiItem.CustomSetActive(true);
                    ResDT_Item_Info info = null;
                    ResDT_Item_Info info2 = null;
                    ResDT_Item_Info stResItemInfo = null;
                    stResItemInfo = this.phase.Config.stResItemInfo;
                    if (this.phase.Config.bColItemCnt > 0)
                    {
                        info = this.phase.Config.astColItemInfo[0];
                    }
                    if (this.phase.Config.bColItemCnt > 1)
                    {
                        info2 = this.phase.Config.astColItemInfo[1];
                    }
                    CUseableContainer useableContainer = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().GetUseableContainer(enCONTAINER_TYPE.ITEM);
                    if (useableContainer != null)
                    {
                        int num = (info != null) ? useableContainer.GetUseableStackCount((COM_ITEM_TYPE) info.wItemType, info.dwItemID) : 0;
                        int num2 = (info2 != null) ? useableContainer.GetUseableStackCount((COM_ITEM_TYPE) info2.wItemType, info2.dwItemID) : 0;
                        if (stResItemInfo != null)
                        {
                            GameObject obj2 = this.uiItem.get_transform().FindChild("DuihuanBtn").get_gameObject();
                            obj2.GetComponent<CUIEventScript>().m_onClickEventParams.commonUInt32Param1 = (uint) this.index;
                            bool isEnable = this.owner.view.activity.timeState == Activity.TimeState.Going;
                            CUICommonSystem.SetButtonEnable(obj2.GetComponent<Button>(), isEnable, isEnable, true);
                            if (info != null)
                            {
                                CUseable useable2 = CUseableManager.CreateUseable((COM_ITEM_TYPE) info.wItemType, info.dwItemID, 1);
                                GameObject obj3 = this.uiItem.get_transform().FindChild("Panel/ItemCell1").get_gameObject();
                                CUICommonSystem.SetItemCell(this.owner.view.form.formScript, obj3, useable2, true, false, false, false);
                                int useableStackCount = useableContainer.GetUseableStackCount((COM_ITEM_TYPE) info.wItemType, info.dwItemID);
                                ushort wItemCnt = info.wItemCnt;
                                Text component = this.uiItem.get_transform().FindChild("Panel/ItemCell1/ItemCount").get_gameObject().GetComponent<Text>();
                                if (useableStackCount < wItemCnt)
                                {
                                    component.set_text(string.Format(Singleton<CTextManager>.GetInstance().GetText("Exchange_ItemNotEnoughCount"), useableStackCount, wItemCnt));
                                    CUICommonSystem.SetButtonEnable(obj2.GetComponent<Button>(), false, false, true);
                                }
                                else
                                {
                                    component.set_text(string.Format(Singleton<CTextManager>.GetInstance().GetText("Exchange_ItemCount"), useableStackCount, wItemCnt));
                                }
                            }
                            if (info2 != null)
                            {
                                CUseable useable3 = CUseableManager.CreateUseable((COM_ITEM_TYPE) info2.wItemType, info2.dwItemID, 1);
                                GameObject obj4 = this.uiItem.get_transform().FindChild("Panel/ItemCell2").get_gameObject();
                                obj4.CustomSetActive(true);
                                CUICommonSystem.SetItemCell(this.owner.view.form.formScript, obj4, useable3, true, false, false, false);
                                int num5 = useableContainer.GetUseableStackCount((COM_ITEM_TYPE) info2.wItemType, info2.dwItemID);
                                ushort num6 = info2.wItemCnt;
                                Text text2 = this.uiItem.get_transform().FindChild("Panel/ItemCell2/ItemCount").get_gameObject().GetComponent<Text>();
                                if (num5 < num6)
                                {
                                    text2.set_text(string.Format(Singleton<CTextManager>.GetInstance().GetText("Exchange_ItemNotEnoughCount"), num5, num6));
                                    CUICommonSystem.SetButtonEnable(obj2.GetComponent<Button>(), false, false, true);
                                }
                                else
                                {
                                    text2.set_text(string.Format(Singleton<CTextManager>.GetInstance().GetText("Exchange_ItemCount"), num5, num6));
                                }
                            }
                            else
                            {
                                this.uiItem.get_transform().FindChild("Panel/ItemCell2").get_gameObject().CustomSetActive(false);
                                this.uiItem.get_transform().FindChild("Panel/Add").get_gameObject().CustomSetActive(false);
                            }
                            CUseable itemUseable = CUseableManager.CreateUseable((COM_ITEM_TYPE) stResItemInfo.wItemType, stResItemInfo.dwItemID, stResItemInfo.wItemCnt);
                            GameObject itemCell = this.uiItem.get_transform().FindChild("Panel/GetItemCell").get_gameObject();
                            bool bActive = false;
                            if (itemUseable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_HERO)
                            {
                                CHeroItem item = (CHeroItem) itemUseable;
                                if (item != null)
                                {
                                    CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                                    if (masterRoleInfo != null)
                                    {
                                        bActive = masterRoleInfo.IsHaveHero(item.m_baseID, true);
                                    }
                                }
                            }
                            else if (itemUseable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN)
                            {
                                CHeroSkin skin = (CHeroSkin) itemUseable;
                                if (skin != null)
                                {
                                    CRoleInfo info6 = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                                    if (info6 != null)
                                    {
                                        bActive = info6.IsHaveHeroSkin(skin.m_heroId, skin.m_skinId, false);
                                    }
                                }
                            }
                            Transform transform = this.uiItem.get_transform().FindChild("Panel/GetItemCell/HaveItemFlag");
                            if ((transform != null) && (transform.get_gameObject() != null))
                            {
                                transform.get_gameObject().CustomSetActive(bActive);
                            }
                            if (itemUseable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
                            {
                                CItem item2 = itemUseable as CItem;
                                if ((item2 != null) && (item2.m_itemData.bIsView > 0))
                                {
                                    CUICommonSystem.SetItemCell(this.owner.view.form.formScript, itemCell, itemUseable, true, false, false, true);
                                }
                                else
                                {
                                    CUICommonSystem.SetItemCell(this.owner.view.form.formScript, itemCell, itemUseable, true, false, false, false);
                                    if (itemCell != null)
                                    {
                                        CUIEventScript script = itemCell.GetComponent<CUIEventScript>();
                                        if (script != null)
                                        {
                                            script.SetUIEvent(enUIEventType.Click, enUIEventID.None);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                CUICommonSystem.SetItemCell(this.owner.view.form.formScript, itemCell, itemUseable, true, false, false, false);
                                if (itemCell != null)
                                {
                                    CUIEventScript script2 = itemCell.GetComponent<CUIEventScript>();
                                    if (script2 != null)
                                    {
                                        script2.SetUIEvent(enUIEventType.Click, enUIEventID.None);
                                    }
                                }
                            }
                            ExchangeActivity activity = this.owner.view.activity as ExchangeActivity;
                            if (activity != null)
                            {
                                GameObject obj8 = this.uiItem.get_transform().FindChild("ExchangeCount").get_gameObject();
                                uint maxExchangeCount = activity.GetMaxExchangeCount(this.phase.Config.bIdx);
                                uint exchangeCount = activity.GetExchangeCount(this.phase.Config.bIdx);
                                if (maxExchangeCount > 0)
                                {
                                    obj8.CustomSetActive(true);
                                    obj8.GetComponent<Text>().set_text(string.Format(Singleton<CTextManager>.GetInstance().GetText("Exchange_TimeLimit"), exchangeCount, maxExchangeCount));
                                    if (exchangeCount >= maxExchangeCount)
                                    {
                                        CUICommonSystem.SetButtonEnable(obj2.GetComponent<Button>(), false, false, true);
                                    }
                                }
                                else
                                {
                                    obj8.CustomSetActive(false);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}

