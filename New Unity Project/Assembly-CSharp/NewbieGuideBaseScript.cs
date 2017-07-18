using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using ResData;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class NewbieGuideBaseScript : MonoBehaviour
{
    [CompilerGenerated]
    private NewbieGuideScriptConf <currentConf>k__BackingField;
    [CompilerGenerated]
    private bool <isGuiding>k__BackingField;
    [CompilerGenerated]
    private bool <isInitialize>k__BackingField;
    public const string HighlightAreaMask = "UGUI/Form/System/Dialog/HighlightAreaMask.prefab";
    public const string HighlighterPath = "UGUI/Form/System/Dialog/WeakGuideHighlighter.prefab";
    private ECompleteType m_completeType;
    protected static List<GameObject> ms_guideTextList = new List<GameObject>();
    protected static List<GameObject> ms_highlighter = new List<GameObject>();
    protected static List<GameObject> ms_highlitGo = new List<GameObject>();
    private static CUIFormScript ms_originalForm = null;
    protected static List<GameObject> ms_originalGo = new List<GameObject>();
    private static Vector3 s_FlipNone = Vector3.get_one();
    private static Vector3 s_FlipX = new Vector3(-1f, 1f, 1f);
    private static Vector3 s_FlipXY = new Vector3(-1f, -1f, 1f);
    private static Vector3 s_FlipY = new Vector3(1f, -1f, 1f);

    public event NewbieGuideBaseScriptDelegate CompleteEvent;

    public event NewbieGuideBaseScriptDelegate onCompleteAll;

    private void AddDelegate()
    {
        if (this.IsDelegateClickEvent())
        {
            this.TryToAddClickEvent();
        }
        if (this.IsDelegateSpecialTip())
        {
            this.AddSpecialTip();
        }
        if (!this.IsShowGuideMask())
        {
            this.ShowGuideMask(false);
        }
    }

    protected void AddHighLightAnyWhere()
    {
        this.PreHighlight();
        this.m_completeType = ECompleteType.ClickAnyWhere;
        this.OpenGuideForm();
    }

    protected void AddHighLightAreaClickAnyWhere(GameObject baseGo, CUIFormScript inOriginalForm)
    {
        this.AddHighlightInternal(baseGo, inOriginalForm, false, false);
        List<GameObject>.Enumerator enumerator = ms_highlitGo.GetEnumerator();
        while (enumerator.MoveNext())
        {
            GameObject current = enumerator.Current;
            if (current != null)
            {
                RectTransform transform = current.get_transform() as RectTransform;
                GameObject content = Singleton<CResourceManager>.GetInstance().GetResource("UGUI/Form/System/Dialog/HighlightAreaMask.prefab", typeof(GameObject), enResourceType.UIPrefab, false, false).m_content as GameObject;
                if (content != null)
                {
                    GameObject obj4 = Object.Instantiate(content) as GameObject;
                    if (obj4 != null)
                    {
                        RectTransform transform2 = obj4.get_transform() as RectTransform;
                        transform2.SetParent(transform);
                        transform2.SetAsLastSibling();
                        transform2.set_localScale(s_FlipNone);
                        Transform transform3 = baseGo.get_transform();
                        if (transform3.get_parent().get_name() == "ScrollRect")
                        {
                            Rect rect = (transform3.get_parent().get_transform() as RectTransform).get_rect();
                            Vector2 vector = new Vector2(rect.get_width(), rect.get_height());
                            transform2.set_sizeDelta(vector);
                        }
                        else
                        {
                            transform2.set_sizeDelta(transform.get_sizeDelta());
                        }
                        transform2.set_position(transform.get_position());
                        transform2.set_anchoredPosition(Vector2.get_zero());
                    }
                }
            }
        }
        this.m_completeType = ECompleteType.ClickAnyWhere;
    }

    protected void AddHighLightGameObject(GameObject baseGo, bool isUI, CUIFormScript inOriginalForm, bool cloneEvent = true)
    {
        this.AddHighlightInternal(baseGo, inOriginalForm, cloneEvent, true);
    }

    private void AddHighlightInternal(GameObject baseGo, CUIFormScript inOriginalForm, bool cloneEvent, bool bShowFinger)
    {
        this.PreHighlight();
        if (baseGo != null)
        {
            ms_originalGo.Add(baseGo);
        }
        ms_originalForm = inOriginalForm;
        this.OpenGuideForm();
        if (NewbieGuideScriptControl.FormGuideMask == null)
        {
            NewbieGuideScriptControl.OpenGuideForm();
        }
        List<GameObject>.Enumerator enumerator = ms_originalGo.GetEnumerator();
        int num = 0;
        while (enumerator.MoveNext())
        {
            GameObject current = enumerator.Current;
            if (current != null)
            {
                GameObject widget = Object.Instantiate(current) as GameObject;
                if (widget != null)
                {
                    RectTransform transform = widget.get_transform() as RectTransform;
                    transform.SetParent(NewbieGuideScriptControl.FormGuideMask.get_transform());
                    transform.SetSiblingIndex(1);
                    transform.set_localScale(current.get_transform().get_localScale());
                    RectTransform transform2 = current.get_transform() as RectTransform;
                    transform.set_pivot(transform2.get_pivot());
                    transform.set_sizeDelta(transform2.get_sizeDelta());
                    LayoutElement component = current.GetComponent<LayoutElement>();
                    if ((component != null) && (transform.get_sizeDelta() == Vector2.get_zero()))
                    {
                        transform.set_sizeDelta(new Vector2(component.get_preferredWidth(), component.get_preferredHeight()));
                    }
                    transform.set_position(current.get_transform().get_position());
                    Vector2 screenPoint = CUIUtility.WorldToScreenPoint(inOriginalForm.GetCamera(), current.get_transform().get_position());
                    Vector3 worldPosition = CUIUtility.ScreenToWorldPoint(NewbieGuideScriptControl.FormGuideMask.GetCamera(), screenPoint, transform.get_position().z);
                    NewbieGuideScriptControl.FormGuideMask.InitializeWidgetPosition(widget, worldPosition);
                    widget.CustomSetActive(false);
                    if (cloneEvent)
                    {
                        CUIEventScript script = current.GetComponent<CUIEventScript>();
                        CUIEventScript script2 = widget.GetComponent<CUIEventScript>();
                        if ((script != null) && (script2 != null))
                        {
                            script2.m_onDownEventParams = script.m_onDownEventParams;
                            script2.m_onUpEventParams = script.m_onUpEventParams;
                            script2.m_onClickEventParams = script.m_onClickEventParams;
                            script2.m_onHoldStartEventParams = script.m_onHoldStartEventParams;
                            script2.m_onHoldEventParams = script.m_onHoldEventParams;
                            script2.m_onHoldEndEventParams = script.m_onHoldEndEventParams;
                            script2.m_onDragStartEventParams = script.m_onDragStartEventParams;
                            script2.m_onDragEventParams = script.m_onDragEventParams;
                            script2.m_onDragEndEventParams = script.m_onDragEndEventParams;
                            script2.m_onDropEventParams = script.m_onDropEventParams;
                            script2.m_closeFormWhenClicked = script.m_closeFormWhenClicked;
                            script2.m_belongedFormScript = script.m_belongedFormScript;
                            script2.m_belongedListScript = script.m_belongedListScript;
                            script2.m_indexInlist = script.m_indexInlist;
                        }
                        CUIMiniEventScript script3 = current.GetComponent<CUIMiniEventScript>();
                        CUIMiniEventScript script4 = widget.GetComponent<CUIMiniEventScript>();
                        if ((script3 != null) && (script4 != null))
                        {
                            script4.m_onDownEventParams = script3.m_onDownEventParams;
                            script4.m_onUpEventParams = script3.m_onUpEventParams;
                            script4.m_onClickEventParams = script3.m_onClickEventParams;
                            script4.m_closeFormWhenClicked = script3.m_closeFormWhenClicked;
                            script4.m_belongedFormScript = script3.m_belongedFormScript;
                            script4.m_belongedListScript = script3.m_belongedListScript;
                            script4.m_indexInlist = script3.m_indexInlist;
                        }
                    }
                    else
                    {
                        CUIEventScript script5 = widget.GetComponent<CUIEventScript>();
                        if (script5 != null)
                        {
                            script5.set_enabled(false);
                        }
                        CUIMiniEventScript script6 = widget.GetComponent<CUIMiniEventScript>();
                        if (script6 != null)
                        {
                            script6.set_enabled(false);
                        }
                    }
                    widget.CustomSetActive(true);
                    CUIAnimatorScript script7 = current.GetComponent<CUIAnimatorScript>();
                    if (script7 != null)
                    {
                        CUICommonSystem.PlayAnimator(widget, script7.m_currentAnimatorStateName);
                    }
                    ms_highlitGo.Add(widget);
                    if (bShowFinger)
                    {
                        GameObject content = Singleton<CResourceManager>.GetInstance().GetResource("UGUI/Form/System/Dialog/WeakGuideHighlighter.prefab", typeof(GameObject), enResourceType.UIPrefab, false, false).m_content as GameObject;
                        if (content != null)
                        {
                            GameObject item = Object.Instantiate(content) as GameObject;
                            if (item != null)
                            {
                                item.get_transform().SetParent(widget.get_transform());
                                Transform transform3 = item.get_transform();
                                switch (this.currentConf.wFlipType)
                                {
                                    case 0:
                                        transform3.set_localScale(s_FlipNone);
                                        break;

                                    case 1:
                                        transform3.set_localScale(s_FlipX);
                                        break;

                                    case 2:
                                        transform3.set_localScale(s_FlipY);
                                        break;

                                    case 3:
                                        transform3.set_localScale(s_FlipXY);
                                        break;
                                }
                                item.get_transform().set_position(widget.get_transform().get_position());
                                (item.get_transform() as RectTransform).set_anchoredPosition(new Vector2((float) this.currentConf.iOffsetHighLightX, (float) this.currentConf.iOffsetHighLightY));
                                if (!this.DoesShowArrow())
                                {
                                    item.get_transform().FindChild("Panel/ImageFinger").get_gameObject().CustomSetActive(false);
                                }
                                ms_highlighter.Add(item);
                            }
                        }
                    }
                    if ((num == 0) && (this.currentConf.wSpecialTip != 0))
                    {
                        NewbieGuideSpecialTipConf specialTipConfig = Singleton<NewbieGuideDataManager>.GetInstance().GetSpecialTipConfig(this.currentConf.wSpecialTip);
                        if ((specialTipConfig != null) && (specialTipConfig.bGuideTextPos > 0))
                        {
                            GameObject obj6 = InstantiateGuideText(specialTipConfig, widget, NewbieGuideScriptControl.FormGuideMask, inOriginalForm);
                            if (obj6 != null)
                            {
                                ms_guideTextList.Add(obj6);
                                obj6.CustomSetActive(false);
                            }
                        }
                    }
                    num++;
                }
            }
        }
    }

    protected void AddHighlightWaiting()
    {
        this.PreHighlight();
        this.m_completeType = ECompleteType.WaitOneWhile;
        this.OpenGuideForm();
    }

    private void AddSpecialTip()
    {
        if (NewbieGuideScriptControl.FormGuideMask != null)
        {
            NewbieGuideScriptControl.FormGuideMask.get_transform().FindChild("GuideTextStatic").get_transform().get_gameObject().CustomSetActive(false);
        }
        if (this.currentConf.wSpecialTip != 0)
        {
            if (NewbieGuideScriptControl.FormGuideMask == null)
            {
                NewbieGuideScriptControl.OpenGuideForm();
            }
            GameObject obj3 = NewbieGuideScriptControl.FormGuideMask.get_transform().FindChild("GuideTextStatic").get_transform().get_gameObject();
            obj3.CustomSetActive(false);
            NewbieGuideSpecialTipConf specialTipConfig = Singleton<NewbieGuideDataManager>.GetInstance().GetSpecialTipConfig(this.currentConf.wSpecialTip);
            if ((specialTipConfig != null) && (specialTipConfig.bGuideTextPos == 0))
            {
                obj3.CustomSetActive(true);
                obj3.get_transform().FindChild("RightSpecial/Text").get_transform().get_gameObject().GetComponent<Text>().set_text(StringHelper.UTF8BytesToString(ref specialTipConfig.szTipText));
            }
        }
    }

    private void AnyWhereClick(CUIEvent inUiEvent)
    {
        this.ClickHandler(inUiEvent);
    }

    private void Awake()
    {
    }

    protected virtual void Clear()
    {
        this.isGuiding = false;
        this.TryToResumeGame();
        this.ClearDelegate();
        this.ClearHighLightGameObject();
    }

    private void ClearDelegate()
    {
        if (this.IsDelegateClickEvent())
        {
            this.TryToRemoveClickEvent();
        }
        if (this.IsDelegateSpecialTip())
        {
            this.RemoveSpecialTip();
        }
        if (!this.IsShowGuideMask())
        {
            this.ShowGuideMask(true);
        }
    }

    private void ClearHighLightGameObject()
    {
        this.ClearHighlightInternal();
    }

    private void ClearHighlightInternal()
    {
        this.SetBackgroundTransparency(EOpacityLadder.Transparent);
        this.ClearHighlitObjs();
        ms_originalGo.Clear();
        ms_originalForm = null;
        this.m_completeType = ECompleteType.ClickButton;
    }

    private void ClearHighlitObjs()
    {
        List<GameObject> list = new List<GameObject>();
        list.AddRange(ms_highlitGo);
        list.AddRange(ms_highlighter);
        List<GameObject>.Enumerator enumerator = list.GetEnumerator();
        while (enumerator.MoveNext())
        {
            GameObject current = enumerator.Current;
            if (current != null)
            {
                current.get_transform().SetParent(null);
                Object.Destroy(current);
            }
        }
        ms_highlitGo.Clear();
        ms_highlighter.Clear();
        int count = ms_guideTextList.Count;
        for (int i = 0; i < count; i++)
        {
            GameObject obj3 = ms_guideTextList[i];
            if (obj3 != null)
            {
                obj3.CustomSetActive(false);
            }
        }
        ms_guideTextList.Clear();
    }

    protected virtual void ClickHandler(CUIEvent uiEvent)
    {
        this.CompleteHandler();
    }

    protected virtual void CompleteAllHandler()
    {
        this.Clear();
        if (this.IsDelegateAutoDispatchCompleteEvent())
        {
            this.DispatchCompleteAllEvent();
        }
    }

    protected virtual void CompleteHandler()
    {
        this.Clear();
        if (this.IsDelegateAutoDispatchCompleteEvent())
        {
            this.DispatchCompleteEvent();
        }
    }

    protected void DispatchCompleteAllEvent()
    {
        if (this.onCompleteAll != null)
        {
            this.onCompleteAll();
        }
    }

    protected void DispatchCompleteEvent()
    {
        if (this.CompleteEvent != null)
        {
            this.CompleteEvent();
        }
    }

    private bool DoesShowArrow()
    {
        return (this.currentConf.bNotShowArrow == 0);
    }

    protected void HideHighlighterAndText()
    {
        this.SetBackgroundTransparency(EOpacityLadder.Transparent);
        List<GameObject> list = new List<GameObject>();
        list.AddRange(ms_highlighter);
        List<GameObject>.Enumerator enumerator = list.GetEnumerator();
        while (enumerator.MoveNext())
        {
            GameObject current = enumerator.Current;
            if (current != null)
            {
                current.CustomSetActive(false);
            }
        }
        int count = ms_guideTextList.Count;
        for (int i = 0; i < count; i++)
        {
            GameObject obj3 = ms_guideTextList[i];
            if (obj3 != null)
            {
                obj3.CustomSetActive(false);
            }
        }
        ms_guideTextList.Clear();
    }

    protected virtual void Initialize()
    {
        if (!this.isInitialize)
        {
            this.TryToPauseGame();
            this.TryToPlayerSound();
            this.AddDelegate();
            this.isInitialize = true;
            this.isGuiding = true;
        }
    }

    public static GameObject InstantiateGuideText(NewbieGuideSpecialTipConf tipConf, GameObject inParentObj, CUIFormScript inGuideForm, CUIFormScript inOriginalForm)
    {
        if (((tipConf == null) || (inParentObj == null)) || ((inGuideForm == null) || (inOriginalForm == null)))
        {
            return null;
        }
        GameObject obj2 = inGuideForm.get_gameObject().get_transform().Find("GuideText").get_gameObject();
        if (obj2 != null)
        {
            Transform transform = obj2.get_transform().FindChild("RightSpecial/Text").get_transform();
            if (transform != null)
            {
                Text component = transform.get_gameObject().GetComponent<Text>();
                if (component != null)
                {
                    component.set_text(StringHelper.UTF8BytesToString(ref tipConf.szTipText));
                }
            }
            obj2.CustomSetActive(true);
        }
        return obj2;
    }

    protected virtual bool IsDelegateAutoDispatchCompleteEvent()
    {
        return true;
    }

    protected virtual bool IsDelegateClickEvent()
    {
        return false;
    }

    protected virtual bool IsDelegateSpecialTip()
    {
        return true;
    }

    protected virtual bool IsShowGuideMask()
    {
        return true;
    }

    public virtual bool IsTimeOutSkip()
    {
        return true;
    }

    private void OpenGuideForm()
    {
        this.SetBackgroundTransparency((EOpacityLadder) this.currentConf.bShowTransparency);
    }

    private void PreHighlight()
    {
        this.m_completeType = ECompleteType.ClickButton;
        ms_originalGo.Clear();
        ms_originalForm = null;
        this.ClearHighlitObjs();
        if ((NewbieGuideScriptControl.FormGuideMask != null) && (NewbieGuideScriptControl.FormGuideMask.get_gameObject() != null))
        {
            Transform transform = NewbieGuideScriptControl.FormGuideMask.get_gameObject().get_transform().Find("GuideText");
            if ((transform != null) && (transform.get_gameObject() != null))
            {
                transform.get_gameObject().CustomSetActive(false);
            }
        }
    }

    private void RemoveSpecialTip()
    {
        if (this.currentConf.wSpecialTip != 0)
        {
        }
    }

    protected void SetBackgroundTransparency(EOpacityLadder inLadder)
    {
        if ((NewbieGuideScriptControl.FormGuideMask != null) && (NewbieGuideScriptControl.FormGuideMask.get_transform() != null))
        {
            float num = 0f;
            switch (inLadder)
            {
                case EOpacityLadder.Standard:
                    num = 0.4f;
                    break;

                case EOpacityLadder.Transparent:
                    num = 0f;
                    break;

                case EOpacityLadder.Darker:
                    num = 0.7f;
                    break;
            }
            Transform transform = NewbieGuideScriptControl.FormGuideMask.get_transform().FindChild("Bg");
            if (transform != null)
            {
                Image component = transform.get_gameObject().GetComponent<Image>();
                if (component != null)
                {
                    component.set_color(new Color(0f, 0f, 0f, num));
                }
            }
        }
    }

    public void SetData(NewbieGuideScriptConf conf)
    {
        this.currentConf = conf;
    }

    private void ShowGuideMask(bool bShow)
    {
        if (bShow)
        {
            NewbieGuideScriptControl.OpenGuideForm();
        }
        else
        {
            NewbieGuideScriptControl.CloseGuideForm();
        }
    }

    private void Start()
    {
        this.Initialize();
    }

    public void Stop()
    {
        this.Clear();
    }

    private void TryToAddClickEvent()
    {
        List<GameObject>.Enumerator enumerator = ms_highlitGo.GetEnumerator();
        while (enumerator.MoveNext())
        {
            GameObject current = enumerator.Current;
            if (current != null)
            {
                CUIEventScript component = current.GetComponent<CUIEventScript>();
                if (component != null)
                {
                    component.onClick = (CUIEventScript.OnUIEventHandler) Delegate.Combine(component.onClick, new CUIEventScript.OnUIEventHandler(this.ClickHandler));
                }
                CUIMiniEventScript script2 = current.GetComponent<CUIMiniEventScript>();
                if (script2 != null)
                {
                    script2.onClick = (CUIMiniEventScript.OnUIEventHandler) Delegate.Combine(script2.onClick, new CUIMiniEventScript.OnUIEventHandler(this.ClickHandler));
                }
                if ((component == null) && (script2 == null))
                {
                    CUIMiniEventScript local1 = current.AddComponent<CUIMiniEventScript>();
                    local1.onClick = (CUIMiniEventScript.OnUIEventHandler) Delegate.Combine(local1.onClick, new CUIMiniEventScript.OnUIEventHandler(this.ClickHandler));
                }
            }
        }
        if (this.m_completeType == ECompleteType.ClickAnyWhere)
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Dialogue_AnyWhereClick, new CUIEventManager.OnUIEventHandler(this.AnyWhereClick));
        }
    }

    private void TryToPauseGame()
    {
        if (this.currentConf.bStopGame == 1)
        {
            Singleton<CBattleGuideManager>.GetInstance().PauseGame(this, false);
        }
    }

    private void TryToPlayerSound()
    {
        if (!string.IsNullOrEmpty(this.currentConf.szSoundFileName))
        {
            Singleton<CSoundManager>.GetInstance().PostEvent(this.currentConf.szSoundFileName, null);
        }
    }

    private void TryToRemoveClickEvent()
    {
        bool flag = false;
        int num = 0;
        List<GameObject>.Enumerator enumerator = ms_highlitGo.GetEnumerator();
        while (enumerator.MoveNext())
        {
            GameObject current = enumerator.Current;
            if (current != null)
            {
                CUIEventScript component = current.GetComponent<CUIEventScript>();
                if (component != null)
                {
                    component.onClick = (CUIEventScript.OnUIEventHandler) Delegate.Remove(component.onClick, new CUIEventScript.OnUIEventHandler(this.ClickHandler));
                    flag = true;
                }
                CUIMiniEventScript script2 = current.GetComponent<CUIMiniEventScript>();
                if (script2 != null)
                {
                    script2.onClick = (CUIMiniEventScript.OnUIEventHandler) Delegate.Remove(script2.onClick, new CUIMiniEventScript.OnUIEventHandler(this.ClickHandler));
                    flag = true;
                }
                Toggle toggle = current.GetComponent<Toggle>();
                if (toggle != null)
                {
                    toggle.set_isOn(false);
                    Toggle toggle2 = ms_originalGo[num].GetComponent<Toggle>();
                    if (toggle2 != null)
                    {
                        toggle2.set_isOn(true);
                    }
                }
                if (!flag)
                {
                }
                num++;
            }
        }
        if (this.m_completeType == ECompleteType.ClickAnyWhere)
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Dialogue_AnyWhereClick, new CUIEventManager.OnUIEventHandler(this.AnyWhereClick));
        }
    }

    private void TryToResumeGame()
    {
        if (this.isInitialize && (this.currentConf.bStopGame == 1))
        {
            Singleton<CBattleGuideManager>.GetInstance().ResumeGame(this);
        }
    }

    protected virtual void Update()
    {
        CUIFormScript formGuideMask = NewbieGuideScriptControl.FormGuideMask;
        if ((ms_originalForm != null) && (formGuideMask != null))
        {
            int count = ms_highlitGo.Count;
            DebugHelper.Assert(count <= ms_originalGo.Count);
            for (int i = 0; i < count; i++)
            {
                GameObject obj2 = ms_highlitGo[i];
                GameObject inParentObj = ms_originalGo[i];
                if ((obj2 != null) && (inParentObj != null))
                {
                    obj2.CustomSetActive(inParentObj.get_activeSelf());
                    Image component = inParentObj.GetComponent<Image>();
                    Image image2 = obj2.GetComponent<Image>();
                    if ((component != null) && (image2 != null))
                    {
                        image2.set_color(component.get_color());
                    }
                    RectTransform transform = obj2.get_transform() as RectTransform;
                    RectTransform transform2 = inParentObj.get_transform() as RectTransform;
                    transform.set_localScale(transform2.get_localScale());
                    transform.set_pivot(transform2.get_pivot());
                    transform.set_sizeDelta(transform2.get_sizeDelta());
                    LayoutElement element = transform2.GetComponent<LayoutElement>();
                    if ((element != null) && (transform.get_sizeDelta() == Vector2.get_zero()))
                    {
                        transform.set_sizeDelta(new Vector2(element.get_preferredWidth(), element.get_preferredHeight()));
                    }
                    transform.set_position(transform2.get_position());
                    Vector2 screenPoint = CUIUtility.WorldToScreenPoint(ms_originalForm.GetCamera(), transform2.get_position());
                    Vector3 vector2 = CUIUtility.ScreenToWorldPoint(NewbieGuideScriptControl.FormGuideMask.GetCamera(), screenPoint, transform.get_position().z);
                    if (i < ms_guideTextList.Count)
                    {
                        NewbieGuideSpecialTipConf specialTipConfig = Singleton<NewbieGuideDataManager>.GetInstance().GetSpecialTipConfig(this.currentConf.wSpecialTip);
                        if ((specialTipConfig != null) && (specialTipConfig.bGuideTextPos > 0))
                        {
                            ms_guideTextList[i].CustomSetActive(obj2.get_activeSelf());
                            UpdateGuideTextPos(specialTipConfig, inParentObj, formGuideMask, ms_originalForm, ms_guideTextList[i]);
                        }
                    }
                }
            }
        }
    }

    public static void UpdateGuideTextPos(NewbieGuideSpecialTipConf tipConf, GameObject inParentObj, CUIFormScript inGuideForm, CUIFormScript inOriginalForm, GameObject rootPanel)
    {
        if (((tipConf != null) && (inParentObj != null)) && ((inGuideForm != null) && (inOriginalForm != null)))
        {
            CUIFormScript script = inGuideForm;
            RectTransform transform = rootPanel.get_transform().FindChild("RightSpecial").get_transform();
            RectTransform transform2 = rootPanel.get_transform();
            Vector3 worldPoint = inParentObj.get_transform().get_position();
            Vector2 vector2 = CUIUtility.WorldToScreenPoint(inOriginalForm.GetCamera(), worldPoint);
            float x = vector2.x;
            float y = vector2.y;
            float num3 = 142f;
            float num4 = 85f;
            Vector2 vector3 = new Vector2(0f, 0f);
            switch (tipConf.bSpecialTipPos)
            {
                case 0:
                    vector3 = new Vector2(-num3, num4);
                    break;

                case 1:
                    vector3 = new Vector2(-num3, -num4);
                    break;

                case 2:
                    vector3 = new Vector2(num3, num4);
                    break;

                case 3:
                    vector3 = new Vector2(num3, -num4);
                    break;
            }
            if (tipConf.iOffsetX != 0)
            {
                vector3.x += tipConf.iOffsetX;
            }
            if (tipConf.iOffsetY != 0)
            {
                vector3.y += tipConf.iOffsetY;
            }
            vector3.x = script.ChangeFormValueToScreen(vector3.x);
            vector3.y = script.ChangeFormValueToScreen(vector3.y);
            transform2.set_sizeDelta(transform.get_sizeDelta());
            float num5 = transform2.get_rect().get_width() / 2f;
            num5 = script.ChangeFormValueToScreen(num5) + 3f;
            float num6 = transform2.get_rect().get_height() / 2f;
            num6 = script.ChangeFormValueToScreen(num6) + 3f;
            x += vector3.x;
            y += vector3.y;
            if (x < num5)
            {
                x = num5;
            }
            else if ((x + num5) > Screen.get_width())
            {
                x = Screen.get_width() - num5;
            }
            if (y < num6)
            {
                y = num6;
            }
            else if ((y + num6) > Screen.get_height())
            {
                y = Screen.get_height() - num6;
            }
            x = script.ChangeScreenValueToForm(x);
            y = script.ChangeScreenValueToForm(y);
            transform2.set_anchoredPosition(new Vector2(x, y));
        }
    }

    public NewbieGuideScriptConf currentConf
    {
        [CompilerGenerated]
        get
        {
            return this.<currentConf>k__BackingField;
        }
        [CompilerGenerated]
        protected set
        {
            this.<currentConf>k__BackingField = value;
        }
    }

    public bool isGuiding
    {
        [CompilerGenerated]
        get
        {
            return this.<isGuiding>k__BackingField;
        }
        [CompilerGenerated]
        protected set
        {
            this.<isGuiding>k__BackingField = value;
        }
    }

    public bool isInitialize
    {
        [CompilerGenerated]
        get
        {
            return this.<isInitialize>k__BackingField;
        }
        [CompilerGenerated]
        protected set
        {
            this.<isInitialize>k__BackingField = value;
        }
    }

    protected string logTitle
    {
        get
        {
            return ("[<color=cyan>新手引导</color>][<color=green>" + this.currentConf.dwID + "</color>]");
        }
    }

    private enum ECompleteType
    {
        ClickButton,
        ClickAnyWhere,
        WaitOneWhile
    }

    public enum eFlipType
    {
        FlipNone,
        FlipX,
        FlipY,
        FlipXY
    }

    public enum EOpacityLadder
    {
        Standard,
        Transparent,
        Darker,
        Count
    }

    public delegate void NewbieGuideBaseScriptDelegate();
}

