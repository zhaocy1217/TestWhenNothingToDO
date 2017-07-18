namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.UI;
    using ResData;
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class CampaignForm : ActivityForm
    {
        private ListView<ActivityMenuItem> _actvMenuList;
        private int _initStep;
        private int _initTimer;
        private int _selectedIndex;
        private GameObject _title;
        private Image _titleImage;
        private Text _titleText;
        private CUIFormScript _uiForm;
        private CUIListScript _uiListMenu;
        private CampaignFormView _view;
        private ScrollRect _viewScroll;
        [CompilerGenerated]
        private static Func<Activity, bool> <>f__am$cache14;
        [CompilerGenerated]
        private static Comparison<Activity> <>f__am$cache15;
        private static int G_DISPLAYCOLS = 5;
        private int[] m_ActivtyTypeToTabIdx;
        private ListView<ActivityTabIndex>[] m_AllSelectActivityMenuList;
        private int m_nSelectActivityIndex;
        private int m_nUseActivityTabCount;
        private string[] m_strTitleList;
        private GameObject[] m_TitleListObj;
        private CUIListScript m_TitleMenuList;
        public static string s_formPath = (CUIUtility.s_Form_Activity_Dir + "Form_Activity.prefab");

        public CampaignForm(ActivitySys sys) : base(sys)
        {
            this.m_strTitleList = new string[] { "精彩活动", "游戏公告" };
            this.m_TitleListObj = new GameObject[2];
            this.m_AllSelectActivityMenuList = new ListView<ActivityTabIndex>[G_DISPLAYCOLS];
            this.m_ActivtyTypeToTabIdx = new int[G_DISPLAYCOLS];
            this._uiForm = null;
        }

        private void ClearActiveData()
        {
            for (int i = 0; i < this.m_AllSelectActivityMenuList.Length; i++)
            {
                this.m_AllSelectActivityMenuList[i] = new ListView<ActivityTabIndex>();
            }
            this.m_nUseActivityTabCount = 0;
            this.m_nSelectActivityIndex = 0;
        }

        public override void Close()
        {
            if (this._actvMenuList != null)
            {
                for (int i = 0; i < this._actvMenuList.Count; i++)
                {
                    this._actvMenuList[i].Clear();
                }
                this._actvMenuList = null;
            }
            if (this._view != null)
            {
                this._view.Clear();
                this._view = null;
            }
            if (null != this._uiForm)
            {
                Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Activity_Select, new CUIEventManager.OnUIEventHandler(this.OnSelectActivity));
                Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Activity_Select_TitleMenu, new CUIEventManager.OnUIEventHandler(this.OnSelectTitleMenu));
                CUIFormScript formScript = this._uiForm;
                this._uiForm = null;
                this._uiListMenu = null;
                this.m_TitleMenuList = null;
                this.m_TitleListObj = null;
                this.ClearActiveData();
                Singleton<CUIManager>.GetInstance().CloseForm(formScript);
                MonoSingleton<NobeSys>.GetInstance().ShowDelayNobeLoseTipsInfo();
                MonoSingleton<PandroaSys>.GetInstance().ShowPopNews();
            }
            MonoSingleton<IDIPSys>.GetInstance().OnCloseIDIPForm(null);
            Singleton<ActivitySys>.GetInstance().OnStateChange -= new ActivitySys.StateChangeDelegate(this.OnValidateActivityRedSpot);
            Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this._initTimer);
        }

        private GameObject GetActivityRedObj()
        {
            if ((this._uiForm != null) && ((this.m_TitleListObj != null) && (this.m_TitleListObj.Length >= 2)))
            {
                return this.m_TitleListObj[0];
            }
            return null;
        }

        public Image GetDynamicImage(DynamicAssets index)
        {
            return this._uiForm.GetWidget((int) index).GetComponent<Image>();
        }

        public GameObject GetIDIPRedObj()
        {
            if (this._uiForm != null)
            {
                int length = this.m_TitleListObj.Length;
                if ((this.m_TitleListObj != null) && ((this.m_nUseActivityTabCount + 1) == length))
                {
                    return this.m_TitleListObj[this.m_nUseActivityTabCount];
                }
            }
            return null;
        }

        private void InitSelectActivtyMenuData()
        {
            for (int i = 0; i < this.m_AllSelectActivityMenuList.Length; i++)
            {
                this.m_AllSelectActivityMenuList[i] = new ListView<ActivityTabIndex>();
            }
            this.m_nUseActivityTabCount = 0;
            this.m_nSelectActivityIndex = 0;
            if (<>f__am$cache14 == null)
            {
                <>f__am$cache14 = new Func<Activity, bool>(null, (IntPtr) <InitSelectActivtyMenuData>m__2C);
            }
            ListView<Activity> activityList = base.Sys.GetActivityList(<>f__am$cache14);
            if (<>f__am$cache15 == null)
            {
                <>f__am$cache15 = delegate (Activity l, Activity r) {
                    bool readyForGet = l.ReadyForGet;
                    bool flag2 = r.ReadyForGet;
                    if (readyForGet != flag2)
                    {
                        return !readyForGet ? 1 : -1;
                    }
                    bool completed = l.Completed;
                    bool flag4 = r.Completed;
                    if (completed != flag4)
                    {
                        return !completed ? -1 : 1;
                    }
                    if (l.FlagType != r.FlagType)
                    {
                        return (int) (r.FlagType - l.FlagType);
                    }
                    return (int) (l.Sequence - r.Sequence);
                };
            }
            activityList.Sort(<>f__am$cache15);
            for (int j = 0; j < activityList.Count; j++)
            {
                Activity activity = activityList[j];
                int tabID = activity.GetTabID();
                if ((tabID < 0) || (tabID >= G_DISPLAYCOLS))
                {
                    tabID = 0;
                }
                ActivityTabIndex item = new ActivityTabIndex();
                item.idx = tabID;
                item.m_Activity = activity;
                this.m_AllSelectActivityMenuList[tabID].Add(item);
            }
            int num4 = 0;
            for (int k = 0; k < this.m_AllSelectActivityMenuList.Length; k++)
            {
                if (this.m_AllSelectActivityMenuList[k].Count > 0)
                {
                    this.m_nUseActivityTabCount++;
                    this.m_ActivtyTypeToTabIdx[num4] = this.m_AllSelectActivityMenuList[k][0].idx;
                    num4++;
                }
            }
        }

        private void onInitTimer(int seq)
        {
            int num3;
            int num4;
            this._initStep = num4 = this._initStep + 1;
            switch (num4)
            {
                case 1:
                    this.InitSelectActivtyMenuData();
                    this._title = Utility.FindChild(this._uiForm.get_gameObject(), "Panel/Title");
                    this._titleText = Utility.GetComponetInChild<Text>(this._title, "Text");
                    this._titleImage = Utility.GetComponetInChild<Image>(this._title, "Image");
                    this._uiListMenu = Utility.GetComponetInChild<CUIListScript>(this._uiForm.get_gameObject(), "Panel/Panle_Activity/Menu/List");
                    this._viewScroll = Utility.GetComponetInChild<ScrollRect>(this._uiForm.get_gameObject(), "Panel/Panle_Activity/ScrollRect");
                    this._view = new CampaignFormView(Utility.FindChild(this._uiForm.get_gameObject(), "Panel/Panle_Activity/ScrollRect/Content"), this, null);
                    return;

                case 2:
                    this.m_TitleMenuList = Utility.GetComponetInChild<CUIListScript>(this._uiForm.get_gameObject(), "Panel/TitleMenu/List");
                    this.m_strTitleList = new string[this.m_nUseActivityTabCount + 1];
                    for (int i = 0; i < this.m_nUseActivityTabCount; i++)
                    {
                        if (i < 3)
                        {
                            this.m_strTitleList[i] = Singleton<ActivitySys>.GetInstance().m_ActivtyTabName[i];
                        }
                        else
                        {
                            this.m_strTitleList[i] = Singleton<ActivitySys>.GetInstance().m_ActivtyTabName[0];
                        }
                    }
                    this.m_strTitleList[this.m_nUseActivityTabCount] = "游戏公告";
                    this.m_TitleMenuList.SetElementAmount(this.m_strTitleList.Length);
                    this.m_TitleListObj = new GameObject[this.m_strTitleList.Length];
                    for (int j = 0; j < this.m_strTitleList.Length; j++)
                    {
                        CUIListElementScript elemenet = this.m_TitleMenuList.GetElemenet(j);
                        Text componetInChild = Utility.GetComponetInChild<Text>(elemenet.get_gameObject(), "Text");
                        if (componetInChild != null)
                        {
                            componetInChild.set_text(this.m_strTitleList[j]);
                        }
                        this.m_TitleListObj[j] = elemenet.get_gameObject();
                    }
                    this.m_TitleMenuList.SelectElement(0, true);
                    return;

                case 3:
                    this.UpdateBuildMenulistTabIdx(0);
                    return;

                case 4:
                {
                    num3 = -1;
                    bool flag = true;
                    while (++num3 < this._actvMenuList.Count)
                    {
                        if ((flag && this._actvMenuList[num3].activity.ReadyForGet) || (!flag && this._actvMenuList[num3].activity.ReadyForDot))
                        {
                            break;
                        }
                        if (flag && ((num3 + 1) == this._actvMenuList.Count))
                        {
                            num3 = -1;
                            flag = false;
                        }
                    }
                    break;
                }
                default:
                    return;
            }
            if (num3 >= this._actvMenuList.Count)
            {
                num3 = 0;
            }
            this._uiListMenu.SelectElement(num3, true);
            this.SelectMenuItem(num3);
            Singleton<ActivitySys>.GetInstance().OnStateChange += new ActivitySys.StateChangeDelegate(this.OnValidateActivityRedSpot);
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Activity_Select, new CUIEventManager.OnUIEventHandler(this.OnSelectActivity));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Activity_Select_TitleMenu, new CUIEventManager.OnUIEventHandler(this.OnSelectTitleMenu));
            this.UpdateTitelRedDot();
            Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this._initTimer);
            this._initStep = 0;
            Singleton<ActivitySys>.GetInstance().OnCampaignFormOpened();
        }

        private void OnSelectActivity(CUIEvent uiEvent)
        {
            this.SelectMenuItem(uiEvent.m_srcWidgetIndexInBelongedList);
            CUICommonSystem.CloseUseableTips();
        }

        private void OnSelectTitleMenu(CUIEvent uiEvent)
        {
            if (this._uiForm != null)
            {
                if (uiEvent.m_srcWidgetIndexInBelongedList < this.m_nUseActivityTabCount)
                {
                    this.UpdateBuildMenulistTabIdx(uiEvent.m_srcWidgetIndexInBelongedList);
                    Transform transform = this._uiForm.get_gameObject().get_transform().Find("Panel/Panle_Activity");
                    if (transform != null)
                    {
                        transform.get_gameObject().CustomSetActive(true);
                    }
                    Transform transform2 = this._uiForm.get_gameObject().get_transform().Find("Panel/Panle_IDIP");
                    if (transform2 != null)
                    {
                        transform2.get_gameObject().CustomSetActive(false);
                    }
                }
                else if (uiEvent.m_srcWidgetIndexInBelongedList == this.m_nUseActivityTabCount)
                {
                    Transform transform3 = this._uiForm.get_gameObject().get_transform().Find("Panel/Panle_Activity");
                    if (transform3 != null)
                    {
                        transform3.get_gameObject().CustomSetActive(false);
                    }
                    Transform transform4 = this._uiForm.get_gameObject().get_transform().Find("Panel/Panle_IDIP");
                    if (transform4 != null)
                    {
                        transform4.get_gameObject().CustomSetActive(true);
                    }
                    MonoSingleton<IDIPSys>.GetInstance().OnOpenIDIPForm(this._uiForm);
                }
            }
        }

        private void OnValidateActivityRedSpot()
        {
            this.UpdateTitelRedDot();
        }

        public override void Open()
        {
            this.ClearActiveData();
            if (null == this._uiForm)
            {
                this._uiForm = Singleton<CUIManager>.GetInstance().OpenForm(s_formPath, false, true);
                if (null != this._uiForm)
                {
                    this._initTimer = Singleton<CTimerManager>.GetInstance().AddTimer(80, 4, new CTimer.OnTimeUpHandler(this.onInitTimer));
                    this._initStep = 0;
                    this.PandroaUpdateBtn();
                }
            }
        }

        public void PandroaUpdateBtn()
        {
            if (this._uiForm != null)
            {
                MonoSingleton<PandroaSys>.GetInstance().ShowActiveActBoxBtn(this._uiForm);
            }
        }

        private void SelectMenuItem(int index)
        {
            if ((index < 0) || (index >= this._actvMenuList.Count))
            {
                this._titleImage.get_gameObject().CustomSetActive(false);
                this._titleText.get_gameObject().CustomSetActive(true);
                this._titleText.set_text(Singleton<CTextManager>.GetInstance().GetText("activityEmptyTitle"));
                this._view.SetActivity(null);
            }
            else if (index != this._selectedIndex)
            {
                this._selectedIndex = index;
                ActivityMenuItem item = this._actvMenuList[this._selectedIndex];
                string title = item.activity.Title;
                if (string.IsNullOrEmpty(title))
                {
                    this._title.CustomSetActive(false);
                }
                else
                {
                    this._title.CustomSetActive(true);
                    if (item.activity.IsImageTitle)
                    {
                        this._titleText.get_gameObject().CustomSetActive(false);
                        this._titleImage.get_gameObject().CustomSetActive(true);
                        this._titleImage.SetSprite(CUIUtility.GetSpritePrefeb(ActivitySys.SpriteRootDir + title, false, false), false);
                        this._titleImage.SetNativeSize();
                    }
                    else
                    {
                        this._titleImage.get_gameObject().CustomSetActive(false);
                        this._titleText.get_gameObject().CustomSetActive(true);
                        this._titleText.set_text(title);
                    }
                }
                this._view.SetActivity(item.activity);
                this._viewScroll.set_verticalNormalizedPosition(1f);
                this.Update();
                item.activity.Visited = true;
            }
        }

        public override void Update()
        {
            if (((this._view != null) && (this._uiForm != null)) && ((this._uiForm.get_gameObject() != null) && (this._initTimer == 0)))
            {
                this._view.Update();
            }
        }

        private void UpdateBuildMenulistTabIdx(int idx)
        {
            if (this._actvMenuList != null)
            {
                for (int i = 0; i < this._actvMenuList.Count; i++)
                {
                    this._actvMenuList[i].Clear();
                }
                this._actvMenuList = null;
            }
            this._actvMenuList = new ListView<ActivityMenuItem>();
            this._selectedIndex = -1;
            if (idx < this.m_ActivtyTypeToTabIdx.Length)
            {
                this.m_nSelectActivityIndex = this.m_ActivtyTypeToTabIdx[idx];
                if (this.m_nSelectActivityIndex < this.m_AllSelectActivityMenuList.Length)
                {
                    this._uiListMenu.SetElementAmount(this.m_AllSelectActivityMenuList[this.m_nSelectActivityIndex].Count);
                    for (int j = 0; j < this.m_AllSelectActivityMenuList[this.m_nSelectActivityIndex].Count; j++)
                    {
                        Activity actv = this.m_AllSelectActivityMenuList[this.m_nSelectActivityIndex][j].m_Activity;
                        ActivityMenuItem item = new ActivityMenuItem(this._uiListMenu.GetElemenet(j).get_gameObject(), actv);
                        this._actvMenuList.Add(item);
                    }
                    this.SelectMenuItem(0);
                    this._uiListMenu.SelectElement(0, true);
                }
            }
        }

        private void UpdateTitelRedDot()
        {
            if ((this._uiForm != null) && (this.m_TitleMenuList != null))
            {
                for (int i = 0; i < this.m_nUseActivityTabCount; i++)
                {
                    CUIListElementScript elemenet = this.m_TitleMenuList.GetElemenet(i);
                    if (elemenet != null)
                    {
                        int alertNum = 0;
                        for (int j = 0; j < this.m_AllSelectActivityMenuList[i].Count; j++)
                        {
                            Activity activity = this.m_AllSelectActivityMenuList[i][j].m_Activity;
                            if ((activity != null) && activity.ReadyForGet)
                            {
                                alertNum++;
                            }
                        }
                        if (alertNum > 0)
                        {
                            CUICommonSystem.AddRedDot(elemenet.get_gameObject(), enRedDotPos.enTopRight, alertNum);
                        }
                        else
                        {
                            CUICommonSystem.DelRedDot(elemenet.get_gameObject());
                        }
                    }
                }
            }
        }

        public override CUIFormScript formScript
        {
            get
            {
                return this._uiForm;
            }
        }

        public class ActivityMenuItem
        {
            public Activity activity;
            public Image flag;
            public Text flagText;
            public Image hotspot;
            public Text name;
            public GameObject root;

            public ActivityMenuItem(GameObject node, Activity actv)
            {
                this.root = node;
                this.activity = actv;
                this.name = Utility.GetComponetInChild<Text>(node, "Name");
                this.flag = Utility.GetComponetInChild<Image>(node, "Flag");
                this.flagText = Utility.GetComponetInChild<Text>(node, "Flag/Text");
                this.hotspot = Utility.GetComponetInChild<Image>(node, "Hotspot");
                this.activity.OnTimeStateChange += new Activity.ActivityEvent(this.OnStateChange);
                this.activity.OnMaskStateChange += new Activity.ActivityEvent(this.OnStateChange);
                this.Validate();
            }

            public void Clear()
            {
                this.activity.OnTimeStateChange -= new Activity.ActivityEvent(this.OnStateChange);
                this.activity.OnMaskStateChange -= new Activity.ActivityEvent(this.OnStateChange);
            }

            private void OnStateChange(Activity actv)
            {
                DebugHelper.Assert(this.hotspot != null, "hotspot != null");
                if (this.hotspot != null)
                {
                    this.hotspot.get_gameObject().SetActive(this.activity.ReadyForDot);
                }
            }

            public void Validate()
            {
                this.name.set_text(this.activity.Name);
                RES_WEAL_COLORBAR_TYPE flagType = this.activity.FlagType;
                if (flagType != RES_WEAL_COLORBAR_TYPE.RES_WEAL_COLORBAR_TYPE_NULL)
                {
                    this.flag.get_gameObject().CustomSetActive(true);
                    string key = flagType.ToString();
                    this.flag.SetSprite(CUIUtility.GetSpritePrefeb("UGUI/Sprite/Dynamic/Activity/" + key, false, false), false);
                    this.flagText.set_text(Singleton<CTextManager>.GetInstance().GetText(key));
                }
                else
                {
                    this.flag.get_gameObject().CustomSetActive(false);
                }
                this.hotspot.get_gameObject().CustomSetActive(this.activity.ReadyForDot);
            }
        }

        private class ActivityTabIndex
        {
            public int idx;
            public Activity m_Activity;
        }

        public enum DynamicAssets
        {
            ButtonBlueImage,
            ButtonYellowImage
        }
    }
}

