namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class PointsExchangeWgt : ActivityWidget
    {
        private ListView<PointsExchangeElement> _elementList;
        private GameObject _elementTmpl;
        private const float SpacingY = 5f;

        public PointsExchangeWgt(GameObject node, ActivityView view) : base(node, view)
        {
            ListView<ActivityPhase> phaseList = view.activity.PhaseList;
            this._elementList = new ListView<PointsExchangeElement>();
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
                    this._elementList.Add(new PointsExchangeElement(phaseList[i] as PointsExchangePhase, uiItem, this, i));
                }
            }
            node.GetComponent<RectTransform>().set_sizeDelta(new Vector2(node.GetComponent<RectTransform>().get_sizeDelta().x, (num * phaseList.Count) + ((phaseList.Count - 1) * 5f)));
            this._elementTmpl.CustomSetActive(false);
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Activity_PtExchange, new CUIEventManager.OnUIEventHandler(this.OnClickExchange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Activity_PtExchangeConfirm, new CUIEventManager.OnUIEventHandler(this.OnClickExchangeConfirm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Activity_PtExchangeCountReady, new CUIEventManager.OnUIEventHandler(this.OnClickExchangeCountSelectReady));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Activity_ExchangeHeroSkinConfirm, new CUIEventManager.OnUIEventHandler(this.OnExchangeHeroSkinConfirm));
            view.activity.OnTimeStateChange += new Activity.ActivityEvent(this.OnStateChange);
        }

        public override void Clear()
        {
            base.view.activity.OnTimeStateChange -= new Activity.ActivityEvent(this.OnStateChange);
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Activity_PtExchange, new CUIEventManager.OnUIEventHandler(this.OnClickExchange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Activity_PtExchangeConfirm, new CUIEventManager.OnUIEventHandler(this.OnClickExchangeConfirm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Activity_PtExchangeCountReady, new CUIEventManager.OnUIEventHandler(this.OnClickExchangeCountSelectReady));
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
                    uint dwResItemID = this._elementList[num].phase.Config.dwResItemID;
                    CUseable useable = CUseableManager.CreateUseable((COM_ITEM_TYPE) this._elementList[num].phase.Config.wResItemType, this._elementList[num].phase.Config.dwResItemID, this._elementList[num].phase.Config.wResItemCnt);
                    if (useable != null)
                    {
                        bool flag = false;
                        int num3 = 0;
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
                                CRoleInfo info2 = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                                if (info2 != null)
                                {
                                    flag = info2.IsHaveHeroSkin(skin.m_heroId, skin.m_skinId, false);
                                }
                            }
                        }
                        else if (useable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
                        {
                            CItem item2 = (CItem) useable;
                            if (item2 != null)
                            {
                                CRoleInfo info3 = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                                if (((item2.m_itemData != null) && (info3 != null)) && (item2.m_itemData.bType == 4))
                                {
                                    int num4 = (int) item2.m_itemData.EftParam[0];
                                    if (num4 > 0)
                                    {
                                        ResRandomRewardStore dataByKey = GameDataMgr.randomRewardDB.GetDataByKey((long) num4);
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
                                                    if (info3.IsHaveHero(dataByKey.astRewardDetail[i].dwItemID, true))
                                                    {
                                                        num3 = 1;
                                                        goto Label_02A9;
                                                    }
                                                    num3 = 0;
                                                    break;
                                                }
                                                if (dataByKey.astRewardDetail[i].bItemType == 11)
                                                {
                                                    if (info3.IsHaveHeroSkin(dataByKey.astRewardDetail[i].dwItemID, true))
                                                    {
                                                        num3 = 1;
                                                        goto Label_02A9;
                                                    }
                                                    num3 = 0;
                                                    break;
                                                }
                                                if ((dataByKey.astRewardDetail[i].bItemType > 0) && (dataByKey.astRewardDetail[i].bItemType < 0x12))
                                                {
                                                    num3 = 0;
                                                }
                                            Label_02A9:;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (flag)
                        {
                            string strContent = string.Format(Singleton<CTextManager>.GetInstance().GetText("ExchangeWgt_Hero_Tips"), useable.m_name);
                            uiEvent.m_eventParams.taskId = 0;
                            Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(strContent, enUIEventID.Activity_ExchangeHeroSkinConfirm, enUIEventID.None, uiEvent.m_eventParams, false);
                        }
                        else if (num3 == 1)
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
                                Singleton<CUIManager>.GetInstance().OpenExchangeCountSelectForm(useable, maxExchangeCount, enUIEventID.Activity_PtExchangeCountReady, par, this._elementList[num].phase.Config.dwPointCnt, Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().JiFen);
                            }
                            else
                            {
                                string str3 = (useable != null) ? useable.m_name : string.Empty;
                                stUIEventParams params2 = new stUIEventParams();
                                params2.commonUInt16Param1 = (ushort) num;
                                this._elementList[num].phase.SetExchangeCountOnce(1);
                                Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(string.Format(Singleton<CTextManager>.GetInstance().GetText("confirmExchange"), maxExchangeCount, str3), enUIEventID.Activity_PtExchangeConfirm, enUIEventID.None, params2, false);
                            }
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
                    uint dwResItemID = this._elementList[num2].phase.Config.dwResItemID;
                    CUseable useable = CUseableManager.CreateUseable((COM_ITEM_TYPE) this._elementList[num2].phase.Config.wResItemType, this._elementList[num2].phase.Config.dwResItemID, 1);
                    string str = (useable != null) ? useable.m_name : string.Empty;
                    stUIEventParams par = new stUIEventParams();
                    par.commonUInt16Param1 = (ushort) num2;
                    this._elementList[num2].phase.SetExchangeCountOnce((int) num);
                    Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(string.Format(Singleton<CTextManager>.GetInstance().GetText("confirmExchange"), num, str), enUIEventID.Activity_PtExchangeConfirm, enUIEventID.None, par, false);
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
                    uint dwResItemID = this._elementList[num].phase.Config.dwResItemID;
                    CUseable item = CUseableManager.CreateUseable((COM_ITEM_TYPE) this._elementList[num].phase.Config.wResItemType, this._elementList[num].phase.Config.dwResItemID, this._elementList[num].phase.Config.wResItemCnt);
                    int maxExchangeCount = this._elementList[num].phase.GetMaxExchangeCount();
                    if (maxExchangeCount > 1)
                    {
                        stUIEventParams par = new stUIEventParams();
                        par.commonUInt16Param1 = (ushort) num;
                        Singleton<CUIManager>.GetInstance().OpenExchangeCountSelectForm(item, maxExchangeCount, enUIEventID.Activity_PtExchangeCountReady, par, this._elementList[num].phase.Config.dwPointCnt, Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().JiFen);
                    }
                    else
                    {
                        string str = (item != null) ? item.m_name : string.Empty;
                        stUIEventParams params2 = new stUIEventParams();
                        params2.commonUInt16Param1 = (ushort) num;
                        this._elementList[num].phase.SetExchangeCountOnce(1);
                        Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Activity_PtExchangeConfirm, params2);
                    }
                }
            }
        }

        private void OnStateChange(Activity acty)
        {
            this.Validate();
            ActivityView view = base.view;
            if (view != null)
            {
                ListView<ActivityWidget> widgetList = view.WidgetList;
                for (int i = 0; i < widgetList.Count; i++)
                {
                    IntrodWidget widget = widgetList[i] as IntrodWidget;
                    if (widget != null)
                    {
                        widget.Validate();
                        break;
                    }
                }
            }
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

        private class PointsExchangeElement
        {
            public int index;
            public PointsExchangeWgt owner;
            public PointsExchangePhase phase;
            public GameObject uiItem;

            public PointsExchangeElement(PointsExchangePhase phase, GameObject uiItem, PointsExchangeWgt owner, int index)
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
                    ResDT_PointExchange config = this.phase.Config;
                    PointsExchangeActivity owner = this.phase.Owner as PointsExchangeActivity;
                    if ((owner != null) && (owner.PointsConfig != null))
                    {
                        ResWealPointExchange pointsConfig = owner.PointsConfig;
                        GameObject obj2 = this.uiItem.get_transform().FindChild("DuihuanBtn").get_gameObject();
                        obj2.GetComponent<CUIEventScript>().m_onClickEventParams.commonUInt32Param1 = (uint) this.index;
                        uint maxExchangeCount = owner.GetMaxExchangeCount(this.index);
                        uint exchangeCount = owner.GetExchangeCount(this.index);
                        uint dwPointCnt = config.dwPointCnt;
                        uint jiFen = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().JiFen;
                        bool isEnable = (jiFen >= dwPointCnt) && ((maxExchangeCount == 0) || (exchangeCount < maxExchangeCount));
                        if (this.owner.view.activity.timeState != Activity.TimeState.Going)
                        {
                            CUICommonSystem.SetButtonEnable(obj2.GetComponent<Button>(), false, false, true);
                        }
                        else
                        {
                            CUICommonSystem.SetButtonEnable(obj2.GetComponent<Button>(), isEnable, isEnable, true);
                        }
                        CUseable itemUseable = CUseableManager.CreateVirtualUseable(enVirtualItemType.enDianJuanJiFen, 1);
                        GameObject itemCell = this.uiItem.get_transform().FindChild("Panel/PointsCell").get_gameObject();
                        CUICommonSystem.SetItemCell(this.owner.view.form.formScript, itemCell, itemUseable, true, false, false, false);
                        CUseable useable2 = CUseableManager.CreateUseable((COM_ITEM_TYPE) config.wResItemType, config.dwResItemID, config.wResItemCnt);
                        GameObject obj4 = this.uiItem.get_transform().FindChild("Panel/GetItemCell").get_gameObject();
                        if (useable2.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
                        {
                            CItem item = useable2 as CItem;
                            if ((item != null) && (item.m_itemData.bIsView > 0))
                            {
                                CUICommonSystem.SetItemCell(this.owner.view.form.formScript, obj4, useable2, true, false, false, true);
                            }
                            else
                            {
                                CUICommonSystem.SetItemCell(this.owner.view.form.formScript, obj4, useable2, true, false, false, false);
                                if (obj4 != null)
                                {
                                    CUIEventScript script = obj4.GetComponent<CUIEventScript>();
                                    if (script != null)
                                    {
                                        script.SetUIEvent(enUIEventType.Click, enUIEventID.None);
                                    }
                                }
                            }
                        }
                        else
                        {
                            CUICommonSystem.SetItemCell(this.owner.view.form.formScript, obj4, useable2, true, false, false, false);
                            if (obj4 != null)
                            {
                                CUIEventScript script2 = obj4.GetComponent<CUIEventScript>();
                                if (script2 != null)
                                {
                                    script2.SetUIEvent(enUIEventType.Click, enUIEventID.None);
                                }
                            }
                        }
                        bool bActive = false;
                        if (useable2.m_type == COM_ITEM_TYPE.COM_OBJTYPE_HERO)
                        {
                            CHeroItem item2 = (CHeroItem) useable2;
                            if (item2 != null)
                            {
                                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                                if (masterRoleInfo != null)
                                {
                                    bActive = masterRoleInfo.IsHaveHero(item2.m_baseID, true);
                                }
                            }
                        }
                        else if (useable2.m_type == COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN)
                        {
                            CHeroSkin skin = (CHeroSkin) useable2;
                            if (skin != null)
                            {
                                CRoleInfo info3 = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                                if (info3 != null)
                                {
                                    bActive = info3.IsHaveHeroSkin(skin.m_heroId, skin.m_skinId, false);
                                }
                            }
                        }
                        Transform transform = this.uiItem.get_transform().FindChild("Panel/GetItemCell/HaveItemFlag");
                        if ((transform != null) && (transform.get_gameObject() != null))
                        {
                            transform.get_gameObject().CustomSetActive(bActive);
                        }
                        Text component = this.uiItem.get_transform().FindChild("Panel/PointsCell/ItemCount").get_gameObject().GetComponent<Text>();
                        if (jiFen < config.dwPointCnt)
                        {
                            component.set_text(string.Format(Singleton<CTextManager>.GetInstance().GetText("Exchange_ItemNotEnoughCount"), jiFen, dwPointCnt));
                            CUICommonSystem.SetButtonEnable(obj2.GetComponent<Button>(), false, false, true);
                        }
                        else
                        {
                            component.set_text(string.Format(Singleton<CTextManager>.GetInstance().GetText("Exchange_ItemCount"), jiFen, dwPointCnt));
                        }
                        GameObject obj5 = this.uiItem.get_transform().FindChild("ExchangeCount").get_gameObject();
                        if (maxExchangeCount > 0)
                        {
                            obj5.CustomSetActive(true);
                            obj5.GetComponent<Text>().set_text(string.Format(Singleton<CTextManager>.GetInstance().GetText("Exchange_TimeLimit"), exchangeCount, maxExchangeCount));
                        }
                        else
                        {
                            obj5.CustomSetActive(false);
                        }
                    }
                }
            }
        }
    }
}

