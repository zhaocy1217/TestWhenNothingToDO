namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.UI;
    using CSProtocol;
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.UI;

    [MessageHandlerClass]
    public class SevenDayCheckSystem : Singleton<SevenDayCheckSystem>
    {
        private CheckInPhase _availablePhase;
        private CheckInActivity _curActivity;
        private CUIFormScript _form;
        [CompilerGenerated]
        private static Func<Activity, bool> <>f__am$cache5;
        [CompilerGenerated]
        private static Func<Activity, bool> <>f__am$cache6;
        public readonly string FormName = string.Format("{0}{1}", "UGUI/Form/System/", "SevenDayCheck/Form_SevenDayCheck.prefab");
        public bool IsShowingLoginOpen;

        protected void ActivityEvent(Activity acty)
        {
            this.UpdateCheckView();
        }

        internal void Clear()
        {
        }

        public override void Init()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SevenCheck_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnOpenSevenDayCheckForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SevenCheck_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseSevenDayCheckForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SevenCheck_Request, new CUIEventManager.OnUIEventHandler(this.OnRequeset));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SevenCheck_LoginOpen, new CUIEventManager.OnUIEventHandler(this.OnLoginOpen));
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
            Singleton<Day14CheckSystem>.GetInstance().OnLoginOpen(null);
        }

        protected void OnLoginOpen(CUIEvent uiEvent)
        {
            if (this._form == null)
            {
                bool flag = false;
                if (<>f__am$cache5 == null)
                {
                    <>f__am$cache5 = new Func<Activity, bool>(null, (IntPtr) <OnLoginOpen>m__75);
                }
                ListView<Activity> activityList = Singleton<ActivitySys>.GetInstance().GetActivityList(<>f__am$cache5);
                if ((activityList != null) && (activityList.Count > 0))
                {
                    this._curActivity = (CheckInActivity) activityList[0];
                    ListView<ActivityPhase> phaseList = this._curActivity.PhaseList;
                    for (int i = 0; i < phaseList.Count; i++)
                    {
                        if (phaseList[i].ReadyForGet)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (flag)
                    {
                        this._curActivity.OnMaskStateChange += new Assets.Scripts.GameSystem.Activity.ActivityEvent(this.ActivityEvent);
                        this._curActivity.OnTimeStateChange += new Assets.Scripts.GameSystem.Activity.ActivityEvent(this.ActivityEvent);
                        this._form = Singleton<CUIManager>.GetInstance().OpenForm(this.FormName, false, true);
                        this.UpdateCheckView();
                    }
                    else
                    {
                        this._curActivity = null;
                    }
                }
                if (!flag)
                {
                    Singleton<Day14CheckSystem>.GetInstance().OnLoginOpen(null);
                }
            }
        }

        protected void OnOpenSevenDayCheckForm(CUIEvent uiEvent)
        {
            if (this._form == null)
            {
                if (<>f__am$cache6 == null)
                {
                    <>f__am$cache6 = new Func<Activity, bool>(null, (IntPtr) <OnOpenSevenDayCheckForm>m__76);
                }
                ListView<Activity> activityList = Singleton<ActivitySys>.GetInstance().GetActivityList(<>f__am$cache6);
                if ((activityList != null) && (activityList.Count > 0))
                {
                    this._form = Singleton<CUIManager>.GetInstance().OpenForm(this.FormName, false, true);
                    this._curActivity = (CheckInActivity) activityList[0];
                    this._curActivity.OnMaskStateChange += new Assets.Scripts.GameSystem.Activity.ActivityEvent(this.ActivityEvent);
                    this._curActivity.OnTimeStateChange += new Assets.Scripts.GameSystem.Activity.ActivityEvent(this.ActivityEvent);
                    this.UpdateCheckView();
                }
            }
        }

        protected void OnRequeset(CUIEvent uiEvent)
        {
            if (((this._form != null) && (this._curActivity != null)) && (this._availablePhase != null))
            {
                this._availablePhase.DrawReward();
                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.SevenCheck_CloseForm);
            }
        }

        private void SetItem(CUseable usable, GameObject uiNode, bool received, bool ready, uint vipLv)
        {
            SevenDayCheckHelper component = uiNode.GetComponent<SevenDayCheckHelper>();
            CUIUtility.SetImageSprite(component.Icon.GetComponent<Image>(), usable.GetIconPath(), this._form, true, false, false, false);
            component.ItemName.GetComponent<Text>().set_text(usable.m_name);
            if (vipLv > 0)
            {
                component.NobeRoot.CustomSetActive(true);
                MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component.Nobe.GetComponent<Image>(), (int) vipLv, false);
            }
            else
            {
                component.NobeRoot.CustomSetActive(false);
            }
            if ((((usable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_HERO) || (usable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN)) || ((usable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP) && CItem.IsHeroExperienceCard(usable.m_baseID))) || ((usable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP) && CItem.IsSkinExChangeCoupons(usable.m_baseID)))
            {
                component.IconBg.CustomSetActive(true);
            }
            else
            {
                component.IconBg.CustomSetActive(false);
            }
            Transform transform = component.Tiyan.get_transform();
            if (transform != null)
            {
                if ((usable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP) && CItem.IsHeroExperienceCard(usable.m_baseID))
                {
                    transform.get_gameObject().CustomSetActive(true);
                    transform.GetComponent<Image>().SetSprite(CUIUtility.GetSpritePrefeb(CExperienceCardSystem.HeroExperienceCardMarkPath, false, false), false);
                }
                else if ((usable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP) && CItem.IsSkinExperienceCard(usable.m_baseID))
                {
                    transform.get_gameObject().CustomSetActive(true);
                    transform.GetComponent<Image>().SetSprite(CUIUtility.GetSpritePrefeb(CExperienceCardSystem.SkinExperienceCardMarkPath, false, false), false);
                }
                else
                {
                    transform.get_gameObject().CustomSetActive(false);
                }
            }
            Transform transform2 = component.ItemNumText.get_transform();
            if (transform2 != null)
            {
                Text target = transform2.GetComponent<Text>();
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
                    component.ItemNum.CustomSetActive(false);
                }
                else
                {
                    component.ItemNum.CustomSetActive(true);
                    component.ItemNumText.CustomSetActive(true);
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
            if (received)
            {
                component.GrayMask.CustomSetActive(true);
            }
            else
            {
                component.GrayMask.CustomSetActive(false);
            }
            if (ready)
            {
                component.Effect.CustomSetActive(true);
            }
            else
            {
                component.Effect.CustomSetActive(false);
            }
            CUIEventScript script = uiNode.GetComponent<CUIEventScript>();
            stUIEventParams params2 = new stUIEventParams();
            params2.iconUseable = usable;
            stUIEventParams eventParams = params2;
            script.SetUIEvent(enUIEventType.Down, enUIEventID.Tips_ItemInfoOpen, eventParams);
            script.SetUIEvent(enUIEventType.HoldEnd, enUIEventID.Tips_ItemInfoClose, eventParams);
            script.SetUIEvent(enUIEventType.Click, enUIEventID.Tips_ItemInfoClose, eventParams);
            script.SetUIEvent(enUIEventType.DragEnd, enUIEventID.Tips_ItemInfoClose, eventParams);
        }

        public override void UnInit()
        {
            base.UnInit();
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SevenCheck_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnOpenSevenDayCheckForm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SevenCheck_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseSevenDayCheckForm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SevenCheck_Request, new CUIEventManager.OnUIEventHandler(this.OnRequeset));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SevenCheck_LoginOpen, new CUIEventManager.OnUIEventHandler(this.OnLoginOpen));
        }

        private void UpdateCheckView()
        {
            if (this._curActivity != null)
            {
                ListView<ActivityPhase> phaseList = this._curActivity.PhaseList;
                Transform transform = this._form.get_gameObject().get_transform().FindChild("Panel/ItemContainer");
                bool isEnable = false;
                for (int i = 0; i < phaseList.Count; i++)
                {
                    CheckInPhase phase = phaseList[i] as CheckInPhase;
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
                        GameObject uiNode = transform.FindChild(string.Format("itemCell{0}", i + 1)).get_gameObject();
                        if (uiNode != null)
                        {
                            this.SetItem(usable, uiNode, marked, readyForGet, gameVipDoubleLv);
                        }
                    }
                    if (!isEnable && readyForGet)
                    {
                        isEnable = true;
                    }
                }
                CUICommonSystem.SetButtonEnable(this._form.get_gameObject().get_transform().FindChild("Panel/BtnCheck").GetComponent<Button>(), isEnable, isEnable, true);
                Transform transform3 = this._form.get_gameObject().get_transform().FindChild("Panel/MeinvPic");
                MonoSingleton<BannerImageSys>.GetInstance().TrySetCheckInImage(transform3.get_gameObject().GetComponent<Image>());
                this._form.get_gameObject().get_transform().FindChild("Panel/Title/Text").get_gameObject().GetComponent<Text>().set_text(Singleton<CTextManager>.instance.GetText("SevenCheckIn_Title"));
                this._form.get_gameObject().get_transform().FindChild("Panel/Desc").get_gameObject().GetComponent<Text>().set_text(Singleton<CTextManager>.instance.GetText("SevenCheckIn_Desc"));
                DateTime time = Utility.ToUtcTime2Local(this._curActivity.StartTime);
                DateTime time2 = Utility.ToUtcTime2Local(this._curActivity.CloseTime);
                string str = string.Format("{0}.{1}.{2}", time.Year, time.Month, time.Day);
                string str2 = string.Format("{0}.{1}.{2}", time2.Year, time2.Month, time2.Day);
                string[] args = new string[] { str, str2 };
                string text = Singleton<CTextManager>.instance.GetText("SevenCheckIn_Date", args);
                this._form.get_gameObject().get_transform().FindChild("Panel/Date").get_gameObject().GetComponent<Text>().set_text(text);
            }
        }
    }
}

