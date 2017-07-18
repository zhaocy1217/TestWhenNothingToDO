namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.UI;

    [MessageHandlerClass]
    public class Day14CheckSystem : Singleton<Day14CheckSystem>
    {
        private CheckInPhase _availablePhase;
        private CheckInActivity _curActivity;
        private CUIFormScript _form;
        [CompilerGenerated]
        private static Func<Activity, bool> <>f__am$cache9;
        public readonly string FormName = string.Format("{0}{1}", "UGUI/Form/System/", "SevenDayCheck/Form_14DayCheck.prefab");
        public bool IsShowingLoginOpen;
        private bool m_bOpenLink;
        private static string[] m_Days = new string[] { "一", "二", "三", "四", "五", "六", "七", "八", "九", "十", "十一", "十二", "十三", "十四" };
        private int[] m_DisplayIndex = new int[] { 1, 5, 7, 12 };
        private int m_SelectIDx;

        protected void ActivityEvent(Activity acty)
        {
        }

        internal void Clear()
        {
        }

        private void GetCheckParams()
        {
            if ((this._curActivity != null) && GameDataMgr.resWealParamDict.ContainsKey(this._curActivity.EntranceParam))
            {
                ResWealParam param = new ResWealParam();
                if (GameDataMgr.resWealParamDict.TryGetValue(this._curActivity.EntranceParam, out param))
                {
                    if (this.m_DisplayIndex.Length != param.dwNum)
                    {
                        this.m_DisplayIndex = new int[param.dwNum];
                    }
                    for (int i = 0; i < param.dwNum; i++)
                    {
                        this.m_DisplayIndex[i] = param.Param[i];
                    }
                }
            }
        }

        private static string GetDay(int i)
        {
            int length = m_Days.Length;
            if (i < length)
            {
                return m_Days[i];
            }
            return string.Empty;
        }

        public override void Init()
        {
            CUIEventManager instance = Singleton<CUIEventManager>.GetInstance();
            instance.AddUIEventListener(enUIEventID.Day14Check_OnItemEnable, new CUIEventManager.OnUIEventHandler(this.OnCheckItemEnable));
            instance.AddUIEventListener(enUIEventID.Day14Check_OnRequestCheck, new CUIEventManager.OnUIEventHandler(this.OnRequeset));
            instance.AddUIEventListener(enUIEventID.Day14Check_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseSevenDayCheckForm));
            instance.AddUIEventListener(enUIEventID.Day14Check_LeftUIItemEnable, new CUIEventManager.OnUIEventHandler(this.OnLeftUIItemEnable));
            instance.AddUIEventListener(enUIEventID.Day14Check_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnLoginOpen));
        }

        private void InitLeftUI()
        {
            this.GetCheckParams();
            if (this._curActivity != null)
            {
                int amount = 4;
                ListView<ActivityPhase> phaseList = this._curActivity.PhaseList;
                Transform transform = this._form.get_gameObject().get_transform().FindChild("Panel/LeftContainer");
                if (transform != null)
                {
                    CUIListScript component = transform.GetComponent<CUIListScript>();
                    if (component != null)
                    {
                        component.SetElementAmount(amount);
                    }
                }
            }
        }

        private void InitUI()
        {
            if (this._curActivity != null)
            {
                ListView<ActivityPhase> phaseList = this._curActivity.PhaseList;
                Transform transform = this._form.get_gameObject().get_transform().FindChild("Panel/ItemContainer");
                if (transform != null)
                {
                    CUIListScript component = transform.GetComponent<CUIListScript>();
                    if (component == null)
                    {
                        return;
                    }
                    if (component != null)
                    {
                        component.SetElementAmount(phaseList.Count);
                    }
                    if (this.m_SelectIDx < phaseList.Count)
                    {
                        component.MoveElementInScrollArea(this.m_SelectIDx, true);
                    }
                }
                DateTime time = Utility.ToUtcTime2Local(this._curActivity.StartTime);
                DateTime time2 = Utility.ToUtcTime2Local(this._curActivity.CloseTime);
                string str = string.Format("{0}.{1}.{2}", time.Year, time.Month, time.Day);
                string str2 = string.Format("{0}.{1}.{2}", time2.Year, time2.Month, time2.Day);
                string[] args = new string[] { str, str2 };
                string text = Singleton<CTextManager>.instance.GetText("SevenCheckIn_Date", args);
                this._form.get_gameObject().get_transform().FindChild("Panel/Date").get_gameObject().GetComponent<Text>().set_text(text);
                Transform transform2 = this._form.get_gameObject().get_transform().FindChild("Panel/TopPic/MaskBg");
                if (transform2 != null)
                {
                    MonoSingleton<BannerImageSys>.GetInstance().TrySet14CheckInImage(transform2.GetComponent<Image>());
                }
            }
        }

        private void OnCheckItemEnable(CUIEvent uiEvent)
        {
            if (this._curActivity != null)
            {
                int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
                ListView<ActivityPhase> phaseList = this._curActivity.PhaseList;
                bool flag = false;
                if ((srcWidgetIndexInBelongedList >= 0) && (srcWidgetIndexInBelongedList < phaseList.Count))
                {
                    CheckInPhase phase = phaseList[srcWidgetIndexInBelongedList] as CheckInPhase;
                    bool marked = phase.Marked;
                    bool readyForGet = phase.ReadyForGet;
                    if (readyForGet)
                    {
                        this._availablePhase = phase;
                    }
                    uint gameVipDoubleLv = phase.GetGameVipDoubleLv();
                    CUseable usable = phase.GetUseable(0);
                    if (usable != null)
                    {
                        Transform uiNode = uiEvent.m_srcWidget.get_transform();
                        if (uiNode != null)
                        {
                            this.SetItem(usable, uiNode, marked, readyForGet, gameVipDoubleLv, srcWidgetIndexInBelongedList);
                        }
                    }
                    if (!flag && readyForGet)
                    {
                        flag = true;
                    }
                }
            }
        }

        protected void OnCloseSevenDayCheckForm(CUIEvent uiEvent)
        {
            if (this._curActivity != null)
            {
                this._curActivity.OnMaskStateChange -= new Assets.Scripts.GameSystem.Activity.ActivityEvent(this.ActivityEvent);
                this._curActivity.OnTimeStateChange -= new Assets.Scripts.GameSystem.Activity.ActivityEvent(this.ActivityEvent);
                this._curActivity = null;
            }
            if (this._form != null)
            {
                Singleton<CUIManager>.GetInstance().CloseForm(this.FormName);
                this._form = null;
            }
        }

        private void OnLeftUIItemEnable(CUIEvent uiEvent)
        {
            if (this._curActivity != null)
            {
                int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
                ListView<ActivityPhase> phaseList = this._curActivity.PhaseList;
                Transform uiNode = uiEvent.m_srcWidget.get_transform();
                if (this.m_DisplayIndex.Length > srcWidgetIndexInBelongedList)
                {
                    int elemIdx = this.m_DisplayIndex[srcWidgetIndexInBelongedList];
                    elemIdx--;
                    if ((elemIdx >= 0) && (elemIdx < phaseList.Count))
                    {
                        CheckInPhase phase = phaseList[elemIdx] as CheckInPhase;
                        if (phase != null)
                        {
                            bool marked = phase.Marked;
                            bool readyForGet = phase.ReadyForGet;
                            if (readyForGet)
                            {
                                this._availablePhase = phase;
                            }
                            uint gameVipDoubleLv = phase.GetGameVipDoubleLv();
                            CUseable usable = phase.GetUseable(0);
                            if ((usable != null) && (uiNode != null))
                            {
                                this.SetLeftItem(usable, uiNode, marked, readyForGet, gameVipDoubleLv, elemIdx);
                            }
                        }
                    }
                }
            }
        }

        public void OnLoginOpen(CUIEvent uiEvent)
        {
            this.m_SelectIDx = 0;
            if (uiEvent == null)
            {
                this.m_bOpenLink = false;
            }
            else
            {
                this.m_bOpenLink = true;
            }
            if (this._form == null)
            {
                bool isEnable = false;
                if (<>f__am$cache9 == null)
                {
                    <>f__am$cache9 = new Func<Activity, bool>(null, (IntPtr) <OnLoginOpen>m__74);
                }
                ListView<Activity> activityList = Singleton<ActivitySys>.GetInstance().GetActivityList(<>f__am$cache9);
                if ((activityList != null) && (activityList.Count > 0))
                {
                    this._curActivity = (CheckInActivity) activityList[0];
                    ListView<ActivityPhase> phaseList = this._curActivity.PhaseList;
                    for (int i = 0; i < phaseList.Count; i++)
                    {
                        if (phaseList[i].ReadyForGet)
                        {
                            this.m_SelectIDx = i;
                            isEnable = true;
                            break;
                        }
                    }
                    if (isEnable || this.m_bOpenLink)
                    {
                        this._curActivity.OnMaskStateChange += new Assets.Scripts.GameSystem.Activity.ActivityEvent(this.ActivityEvent);
                        this._curActivity.OnTimeStateChange += new Assets.Scripts.GameSystem.Activity.ActivityEvent(this.ActivityEvent);
                        this._form = Singleton<CUIManager>.GetInstance().OpenForm(this.FormName, false, true);
                        this.InitUI();
                        this.InitLeftUI();
                        if (this._form != null)
                        {
                            CUICommonSystem.SetButtonEnable(this._form.get_gameObject().get_transform().FindChild("Panel/BtnCheck").GetComponent<Button>(), isEnable, isEnable, true);
                        }
                    }
                    else
                    {
                        this._curActivity = null;
                    }
                }
            }
        }

        protected void OnRequeset(CUIEvent uiEvent)
        {
            if (((this._form != null) && (this._curActivity != null)) && (this._availablePhase != null))
            {
                this._availablePhase.DrawReward();
                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Day14Check_CloseForm);
            }
        }

        private void SetItem(CUseable usable, Transform uiNode, bool received, bool ready, uint vipLv, int elemIdx)
        {
            Transform transform = uiNode.get_transform().FindChild("DayBg/DayText");
            if (transform != null)
            {
                transform.GetComponent<Text>().set_text(string.Format("第{0}天", GetDay(elemIdx)));
            }
            Transform transform2 = uiNode.get_transform().FindChild("ItemIcon");
            if (transform2 != null)
            {
                CUIUtility.SetImageSprite(transform2.GetComponent<Image>(), usable.GetIconPath(), this._form, true, false, false, false);
            }
            Transform transform3 = uiNode.get_transform().FindChild("ItemName");
            if (transform3 != null)
            {
                transform3.GetComponent<Text>().set_text(usable.m_name);
            }
            Transform transform4 = uiNode.get_transform().FindChild("Bg");
            if ((((usable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_HERO) || (usable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN)) || ((usable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP) && CItem.IsHeroExperienceCard(usable.m_baseID))) || ((usable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP) && CItem.IsSkinExChangeCoupons(usable.m_baseID)))
            {
                if (transform4 != null)
                {
                    transform4.get_gameObject().CustomSetActive(true);
                }
            }
            else if (transform4 != null)
            {
                transform4.get_gameObject().CustomSetActive(false);
            }
            Transform transform5 = uiNode.get_transform().FindChild("TiyanMask");
            if (transform5 != null)
            {
                if ((usable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP) && CItem.IsHeroExperienceCard(usable.m_baseID))
                {
                    transform5.get_gameObject().CustomSetActive(true);
                    transform5.GetComponent<Image>().SetSprite(CUIUtility.GetSpritePrefeb(CExperienceCardSystem.HeroExperienceCardMarkPath, false, false), false);
                }
                else if ((usable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP) && CItem.IsSkinExperienceCard(usable.m_baseID))
                {
                    transform5.get_gameObject().CustomSetActive(true);
                    transform5.GetComponent<Image>().SetSprite(CUIUtility.GetSpritePrefeb(CExperienceCardSystem.SkinExperienceCardMarkPath, false, false), false);
                }
                else
                {
                    transform5.get_gameObject().CustomSetActive(false);
                }
            }
            Transform transform6 = uiNode.get_transform().FindChild("ItemNum/ItemNumText");
            if (transform6 != null)
            {
                Text target = transform6.GetComponent<Text>();
                if (usable.m_stackCount < 0x2710)
                {
                    target.set_text(usable.m_stackCount.ToString());
                }
                else
                {
                    target.set_text((usable.m_stackCount / 0x2710) + "万");
                }
                CUICommonSystem.AppendMultipleText(target, usable.m_stackMulti);
                if (usable.m_stackCount <= 1)
                {
                    target.get_gameObject().CustomSetActive(false);
                    uiNode.get_transform().FindChild("ItemNum").get_gameObject().CustomSetActive(false);
                }
                else
                {
                    uiNode.get_transform().FindChild("ItemNum").get_gameObject().CustomSetActive(true);
                    transform6.get_gameObject().CustomSetActive(true);
                }
                if (usable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL)
                {
                    if (((CSymbolItem) usable).IsGuildSymbol())
                    {
                        target.set_text(string.Empty);
                    }
                    else
                    {
                        target.set_text(usable.GetSalableCount().ToString());
                    }
                }
            }
            Transform transform7 = uiNode.get_transform().FindChild("LingQuGou");
            if (transform7 != null)
            {
                if (received)
                {
                    transform7.get_gameObject().CustomSetActive(true);
                }
                else
                {
                    transform7.get_gameObject().CustomSetActive(false);
                }
            }
            Transform transform8 = uiNode.get_transform().FindChild("XiYou");
            if (transform8 != null)
            {
                if (ready)
                {
                    transform8.get_gameObject().CustomSetActive(true);
                    Transform transform9 = transform8.get_transform().FindChild("Bg/Text");
                    if (transform9 != null)
                    {
                        transform9.GetComponent<Text>().set_text(string.Format("第{0}天", GetDay(elemIdx)));
                    }
                }
                else
                {
                    transform8.get_gameObject().CustomSetActive(false);
                }
            }
            CUIEventScript component = uiNode.GetComponent<CUIEventScript>();
            stUIEventParams params2 = new stUIEventParams();
            params2.iconUseable = usable;
            stUIEventParams eventParams = params2;
            component.SetUIEvent(enUIEventType.Down, enUIEventID.Tips_ItemInfoOpen, eventParams);
            component.SetUIEvent(enUIEventType.HoldEnd, enUIEventID.Tips_ItemInfoClose, eventParams);
            component.SetUIEvent(enUIEventType.Click, enUIEventID.Tips_ItemInfoClose, eventParams);
            component.SetUIEvent(enUIEventType.DragEnd, enUIEventID.Tips_ItemInfoClose, eventParams);
        }

        private void SetLeftItem(CUseable usable, Transform uiNode, bool received, bool ready, uint vipLv, int elemIdx)
        {
            Transform transform = uiNode.get_transform().FindChild("ItemIcon");
            if (transform != null)
            {
                CUIUtility.SetImageSprite(transform.GetComponent<Image>(), usable.GetIconPath(), this._form, true, false, false, false);
            }
            Transform transform2 = uiNode.get_transform().FindChild("GotCeck");
            if (transform2 != null)
            {
                if (received)
                {
                    transform2.get_gameObject().CustomSetActive(true);
                }
                else
                {
                    transform2.get_gameObject().CustomSetActive(false);
                }
            }
            Transform transform3 = uiNode.get_transform().FindChild("TiyanMask");
            if (transform3 != null)
            {
                if ((usable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP) && CItem.IsHeroExperienceCard(usable.m_baseID))
                {
                    transform3.get_gameObject().CustomSetActive(true);
                    transform3.GetComponent<Image>().SetSprite(CUIUtility.GetSpritePrefeb(CExperienceCardSystem.HeroExperienceCardMarkPath, false, false), false);
                }
                else if ((usable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP) && CItem.IsSkinExperienceCard(usable.m_baseID))
                {
                    transform3.get_gameObject().CustomSetActive(true);
                    transform3.GetComponent<Image>().SetSprite(CUIUtility.GetSpritePrefeb(CExperienceCardSystem.SkinExperienceCardMarkPath, false, false), false);
                }
                else
                {
                    transform3.get_gameObject().CustomSetActive(false);
                }
            }
            Transform transform4 = uiNode.get_transform().FindChild("Bg");
            if ((((usable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_HERO) || (usable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN)) || ((usable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP) && CItem.IsHeroExperienceCard(usable.m_baseID))) || ((usable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP) && CItem.IsSkinExChangeCoupons(usable.m_baseID)))
            {
                if (transform4 != null)
                {
                    transform4.get_gameObject().CustomSetActive(true);
                }
            }
            else if (transform4 != null)
            {
                transform4.get_gameObject().CustomSetActive(false);
            }
            Transform transform5 = uiNode.get_transform().FindChild("Name");
            if (transform5 != null)
            {
                transform5.GetComponent<Text>().set_text(usable.m_name);
            }
            Transform transform6 = uiNode.get_transform().FindChild("Num");
            if (transform6 != null)
            {
                int num = elemIdx + 1;
                transform6.GetComponent<Text>().set_text(num.ToString());
            }
            CUIEventScript component = uiNode.GetComponent<CUIEventScript>();
            stUIEventParams params2 = new stUIEventParams();
            params2.iconUseable = usable;
            stUIEventParams eventParams = params2;
            component.SetUIEvent(enUIEventType.Down, enUIEventID.Tips_ItemInfoOpen, eventParams);
            component.SetUIEvent(enUIEventType.HoldEnd, enUIEventID.Tips_ItemInfoClose, eventParams);
            component.SetUIEvent(enUIEventType.Click, enUIEventID.Tips_ItemInfoClose, eventParams);
            component.SetUIEvent(enUIEventType.DragEnd, enUIEventID.Tips_ItemInfoClose, eventParams);
        }

        public override void UnInit()
        {
            base.UnInit();
            CUIEventManager instance = Singleton<CUIEventManager>.GetInstance();
            instance.RemoveUIEventListener(enUIEventID.Day14Check_OnItemEnable, new CUIEventManager.OnUIEventHandler(this.OnCheckItemEnable));
            instance.RemoveUIEventListener(enUIEventID.Day14Check_OnRequestCheck, new CUIEventManager.OnUIEventHandler(this.OnRequeset));
            instance.RemoveUIEventListener(enUIEventID.Day14Check_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseSevenDayCheckForm));
            instance.RemoveUIEventListener(enUIEventID.Day14Check_LeftUIItemEnable, new CUIEventManager.OnUIEventHandler(this.OnLeftUIItemEnable));
            instance.RemoveUIEventListener(enUIEventID.Day14Check_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnLoginOpen));
        }
    }
}

