namespace Assets.Scripts.UI
{
    using Assets.Scripts.Framework;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    [ExecuteInEditMode]
    public class CUIFormScript : MonoBehaviour, IComparable
    {
        private const int c_openOrderMask = 10;
        private const int c_overlayOrderMask = 0x2710;
        private const int c_priorityOrderMask = 0x3e8;
        public bool m_alwaysKeepVisible;
        private ListView<CASyncLoadedImage> m_asyncLoadedImages;
        private Canvas m_canvas;
        [HideInInspector]
        public CanvasScaler m_canvasScaler;
        [HideInInspector]
        public int m_clickedEventDispatchedCounter;
        public string[] m_closedWwiseEvents = new string[] { "UI_Default_Close_Window" };
        private enFormPriority m_defaultPriority;
        public bool m_disableInput;
        public bool m_enableMultiClickedEvent = true;
        [HideInInspector]
        public enUIEventID[] m_eventIDs = new enUIEventID[Enum.GetValues(typeof(enFormEventType)).Length];
        public stUIEventParams[] m_eventParams = new stUIEventParams[Enum.GetValues(typeof(enFormEventType)).Length];
        public string m_formFadeInAnimationName = string.Empty;
        private CUIComponent m_formFadeInAnimationScript;
        public enFormFadeAnimationType m_formFadeInAnimationType;
        public string m_formFadeOutAnimationName = string.Empty;
        private CUIComponent m_formFadeOutAnimationScript;
        public enFormFadeAnimationType m_formFadeOutAnimationType;
        [HideInInspector]
        public string m_formPath;
        public GameObject[] m_formWidgets = new GameObject[0];
        public bool m_fullScreenBG;
        private GraphicRaycaster m_graphicRaycaster;
        public int m_group;
        private int m_hideFlags;
        public bool m_hideUnderForms;
        private List<stInitWidgetPosition> m_initWidgetPositions;
        private bool m_isClosed;
        private bool m_isHided;
        private bool m_isInFadeIn;
        private bool m_isInFadeOut;
        private bool m_isInitialized;
        public bool m_isModal;
        private bool m_isNeedClose;
        public bool m_isSingleton;
        private Dictionary<string, GameObject> m_loadedSpriteDictionary;
        public string[] m_openedWwiseEvents = new string[] { "UI_Default_Open_Window" };
        private int m_openOrder;
        public enFormPriority m_priority;
        public Vector2 m_referenceResolution = new Vector2(960f, 640f);
        [HideInInspector]
        private ListView<ListView<Camera>> m_relatedSceneCamera;
        [HideInInspector]
        private ListView<GameObject> m_relatedScenes;
        private int m_renderFrameStamp;
        [HideInInspector]
        public enUIEventID m_revertToHideEvent;
        [HideInInspector]
        public enUIEventID m_revertToVisibleEvent;
        private int m_sequence;
        [NonSerialized, HideInInspector]
        public SGameGraphicRaycaster m_sgameGraphicRaycaster;
        private int m_sortingOrder;
        private ListView<CUIComponent> m_uiComponents;
        [HideInInspector]
        public bool m_useFormPool;

        public void AddASyncLoadedImage(Image image, string prefabPath, bool needCached, bool unloadBelongedAssetBundleAfterLoaded, bool isShowSpecMatrial = false)
        {
            if (this.m_asyncLoadedImages == null)
            {
                this.m_asyncLoadedImages = new ListView<CASyncLoadedImage>();
            }
            if (this.m_loadedSpriteDictionary == null)
            {
                this.m_loadedSpriteDictionary = new Dictionary<string, GameObject>();
            }
            for (int i = 0; i < this.m_asyncLoadedImages.Count; i++)
            {
                if (this.m_asyncLoadedImages[i].m_image == image)
                {
                    this.m_asyncLoadedImages[i].m_prefabPath = prefabPath;
                    return;
                }
            }
            CASyncLoadedImage item = new CASyncLoadedImage(image, prefabPath, needCached, unloadBelongedAssetBundleAfterLoaded, isShowSpecMatrial);
            this.m_asyncLoadedImages.Add(item);
        }

        public void AddRelatedScene(GameObject scene, string sceneName)
        {
            scene.set_name(sceneName);
            scene.get_transform().SetParent(base.get_gameObject().get_transform());
            scene.get_transform().set_localPosition(Vector3.get_zero());
            scene.get_transform().set_localRotation(Quaternion.get_identity());
            this.m_relatedScenes.Add(scene);
            this.m_relatedSceneCamera.Add(new ListView<Camera>());
            this.AddRelatedSceneCamera(this.m_relatedSceneCamera.Count - 1, scene);
        }

        public void AddRelatedSceneCamera(int index, GameObject go)
        {
            if (((index >= 0) && (index < this.m_relatedSceneCamera.Count)) && (go != null))
            {
                Camera component = go.GetComponent<Camera>();
                if (component != null)
                {
                    this.m_relatedSceneCamera[index].Add(component);
                }
                for (int i = 0; i < go.get_transform().get_childCount(); i++)
                {
                    this.AddRelatedSceneCamera(index, go.get_transform().GetChild(i).get_gameObject());
                }
            }
        }

        public void AddUIComponent(CUIComponent uiComponent)
        {
            if ((uiComponent != null) && !this.m_uiComponents.Contains(uiComponent))
            {
                this.m_uiComponents.Add(uiComponent);
            }
        }

        public void Appear(enFormHideFlag hideFlag = 1, bool dispatchVisibleChangedEvent = true)
        {
            if (!this.m_alwaysKeepVisible)
            {
                this.m_hideFlags &= ~hideFlag;
                if ((this.m_hideFlags == 0) && this.m_isHided)
                {
                    this.m_isHided = false;
                    if (this.m_canvas != null)
                    {
                        this.m_canvas.set_enabled(true);
                        this.m_canvas.set_sortingOrder(this.m_sortingOrder);
                    }
                    if ((this.m_graphicRaycaster != null) && !this.m_disableInput)
                    {
                        this.m_graphicRaycaster.set_enabled(true);
                    }
                    for (int i = 0; i < this.m_relatedScenes.Count; i++)
                    {
                        CUIUtility.SetGameObjectLayer(this.m_relatedScenes[i], 0x12);
                        this.SetSceneCameraEnable(i, true);
                    }
                    this.AppearComponent();
                    this.DispatchRevertVisibleFormEvent();
                    if (dispatchVisibleChangedEvent)
                    {
                        this.DispatchVisibleChangedEvent();
                    }
                }
            }
        }

        private void AppearComponent()
        {
            for (int i = 0; i < this.m_uiComponents.Count; i++)
            {
                this.m_uiComponents[i].Appear();
            }
        }

        private void Awake()
        {
            this.m_uiComponents = new ListView<CUIComponent>();
            this.m_relatedScenes = new ListView<GameObject>();
            this.m_relatedSceneCamera = new ListView<ListView<Camera>>();
            this.InitializeCanvas();
        }

        private int CalculateSortingOrder(enFormPriority priority, int openOrder)
        {
            if ((openOrder * 10) >= 0x3e8)
            {
                openOrder = openOrder % 100;
            }
            return (((!this.IsOverlay() ? 0 : 0x2710) + (priority * ((enFormPriority) 0x3e8))) + (openOrder * 10));
        }

        public float ChangeFormValueToScreen(float value)
        {
            if (this.m_canvasScaler.get_matchWidthOrHeight() == 0f)
            {
                return ((value * Screen.get_width()) / this.m_canvasScaler.get_referenceResolution().x);
            }
            if (this.m_canvasScaler.get_matchWidthOrHeight() == 1f)
            {
                return ((value * Screen.get_height()) / this.m_canvasScaler.get_referenceResolution().y);
            }
            return value;
        }

        public float ChangeScreenValueToForm(float value)
        {
            if (this.m_canvasScaler.get_matchWidthOrHeight() == 0f)
            {
                return ((value * this.m_canvasScaler.get_referenceResolution().x) / ((float) Screen.get_width()));
            }
            if (this.m_canvasScaler.get_matchWidthOrHeight() == 1f)
            {
                return ((value * this.m_canvasScaler.get_referenceResolution().y) / ((float) Screen.get_height()));
            }
            return value;
        }

        public void Close()
        {
            if (!this.m_isNeedClose)
            {
                this.m_isNeedClose = true;
                this.DispatchFormEvent(enFormEventType.Close);
                for (int i = 0; i < this.m_closedWwiseEvents.Length; i++)
                {
                    if (!string.IsNullOrEmpty(this.m_closedWwiseEvents[i]))
                    {
                        Singleton<CSoundManager>.GetInstance().PostEvent(this.m_closedWwiseEvents[i], null);
                    }
                }
                this.CloseComponent();
            }
        }

        private void CloseComponent()
        {
            for (int i = 0; i < this.m_uiComponents.Count; i++)
            {
                this.m_uiComponents[i].Close();
            }
        }

        public int CompareTo(object obj)
        {
            CUIFormScript script = obj as CUIFormScript;
            if (this.m_sortingOrder > script.m_sortingOrder)
            {
                return 1;
            }
            if (this.m_sortingOrder == script.m_sortingOrder)
            {
                return 0;
            }
            return -1;
        }

        public void CustomLateUpdate()
        {
            if (this.m_initWidgetPositions != null)
            {
                int index = 0;
                while (index < this.m_initWidgetPositions.Count)
                {
                    stInitWidgetPosition position = this.m_initWidgetPositions[index];
                    if ((this.m_renderFrameStamp - position.m_renderFrameStamp) <= 1)
                    {
                        if (position.m_widget != null)
                        {
                            position.m_widget.get_transform().set_position(position.m_worldPosition);
                        }
                    }
                    else
                    {
                        this.m_initWidgetPositions.RemoveAt(index);
                        continue;
                    }
                    index++;
                }
            }
            this.UpdateASyncLoadedImage();
            this.m_clickedEventDispatchedCounter = 0;
            this.m_renderFrameStamp++;
        }

        public void CustomUpdate()
        {
            this.UpdateFadeIn();
            this.UpdateFadeOut();
        }

        private void DispatchChangeFormPriorityEvent()
        {
            CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
            uIEvent.m_eventID = enUIEventID.UI_OnFormPriorityChanged;
            uIEvent.m_srcFormScript = this;
            uIEvent.m_srcWidget = null;
            uIEvent.m_srcWidgetScript = null;
            uIEvent.m_srcWidgetBelongedListScript = null;
            uIEvent.m_srcWidgetIndexInBelongedList = 0;
            uIEvent.m_pointerEventData = null;
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uIEvent);
        }

        private void DispatchFormEvent(enFormEventType formEventType)
        {
            if (this.m_eventIDs[(int) formEventType] != enUIEventID.None)
            {
                CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
                uIEvent.m_eventID = this.m_eventIDs[(int) formEventType];
                uIEvent.m_eventParams = this.m_eventParams[(int) formEventType];
                uIEvent.m_srcFormScript = this;
                uIEvent.m_srcWidget = null;
                uIEvent.m_srcWidgetScript = null;
                uIEvent.m_srcWidgetBelongedListScript = null;
                uIEvent.m_srcWidgetIndexInBelongedList = 0;
                uIEvent.m_pointerEventData = null;
                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uIEvent);
            }
        }

        private void DispatchRevertVisibleFormEvent()
        {
            if (this.m_revertToVisibleEvent != enUIEventID.None)
            {
                CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
                uIEvent.m_eventID = this.m_revertToVisibleEvent;
                uIEvent.m_srcFormScript = this;
                uIEvent.m_srcWidget = null;
                uIEvent.m_srcWidgetScript = null;
                uIEvent.m_srcWidgetBelongedListScript = null;
                uIEvent.m_srcWidgetIndexInBelongedList = 0;
                uIEvent.m_pointerEventData = null;
                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uIEvent);
            }
        }

        private void DispatchVisibleChangedEvent()
        {
            CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
            uIEvent.m_eventID = enUIEventID.UI_OnFormVisibleChanged;
            uIEvent.m_srcFormScript = this;
            uIEvent.m_srcWidget = null;
            uIEvent.m_srcWidgetScript = null;
            uIEvent.m_srcWidgetBelongedListScript = null;
            uIEvent.m_srcWidgetIndexInBelongedList = 0;
            uIEvent.m_pointerEventData = null;
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uIEvent);
        }

        public Camera GetCamera()
        {
            if ((this.m_canvas != null) && (this.m_canvas.get_renderMode() != null))
            {
                return this.m_canvas.get_worldCamera();
            }
            return null;
        }

        public GraphicRaycaster GetGraphicRaycaster()
        {
            return this.m_graphicRaycaster;
        }

        public Vector2 GetReferenceResolution()
        {
            return ((this.m_canvasScaler != null) ? this.m_canvasScaler.get_referenceResolution() : Vector2.get_zero());
        }

        public float GetScreenScaleValue()
        {
            float num = 1f;
            RectTransform component = base.GetComponent<RectTransform>();
            if ((component != null) && (this.m_canvasScaler.get_matchWidthOrHeight() == 0f))
            {
                num = (component.get_rect().get_width() / component.get_rect().get_height()) / (this.m_canvasScaler.get_referenceResolution().x / this.m_canvasScaler.get_referenceResolution().y);
            }
            return num;
        }

        public int GetSequence()
        {
            return this.m_sequence;
        }

        public int GetSortingOrder()
        {
            return this.m_sortingOrder;
        }

        public GameObject GetWidget(int index)
        {
            if ((index >= 0) && (index < this.m_formWidgets.Length))
            {
                return this.m_formWidgets[index];
            }
            return null;
        }

        public void Hide(enFormHideFlag hideFlag = 1, bool dispatchVisibleChangedEvent = true)
        {
            if (!this.m_alwaysKeepVisible)
            {
                this.m_hideFlags |= hideFlag;
                if ((this.m_hideFlags != 0) && !this.m_isHided)
                {
                    this.m_isHided = true;
                    if (this.m_canvas != null)
                    {
                        this.m_canvas.set_enabled(false);
                    }
                    if (this.m_graphicRaycaster != null)
                    {
                        this.m_graphicRaycaster.set_enabled(false);
                    }
                    for (int i = 0; i < this.m_relatedScenes.Count; i++)
                    {
                        CUIUtility.SetGameObjectLayer(this.m_relatedScenes[i], 0x1f);
                        this.SetSceneCameraEnable(i, false);
                    }
                    this.HideComponent();
                    if (this.m_revertToHideEvent != enUIEventID.None)
                    {
                        Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(this.m_revertToHideEvent);
                    }
                    if (dispatchVisibleChangedEvent)
                    {
                        this.DispatchVisibleChangedEvent();
                    }
                }
            }
        }

        private void HideComponent()
        {
            for (int i = 0; i < this.m_uiComponents.Count; i++)
            {
                this.m_uiComponents[i].Hide();
            }
        }

        public void Initialize()
        {
            if (!this.m_isInitialized)
            {
                this.m_defaultPriority = this.m_priority;
                this.InitializeComponent(base.get_gameObject());
                this.m_isInitialized = true;
            }
        }

        public void InitializeCanvas()
        {
            this.m_canvas = base.get_gameObject().GetComponent<Canvas>();
            this.m_canvasScaler = base.get_gameObject().GetComponent<CanvasScaler>();
            this.m_graphicRaycaster = base.GetComponent<GraphicRaycaster>();
            if ((this.m_graphicRaycaster != null) && this.m_disableInput)
            {
                this.m_graphicRaycaster.set_enabled(false);
            }
            this.m_sgameGraphicRaycaster = this.m_graphicRaycaster as SGameGraphicRaycaster;
            this.MatchScreen();
        }

        public void InitializeComponent(GameObject root)
        {
            CUIComponent[] components = root.GetComponents<CUIComponent>();
            if ((components != null) && (components.Length > 0))
            {
                for (int j = 0; j < components.Length; j++)
                {
                    components[j].Initialize(this);
                }
            }
            for (int i = 0; i < root.get_transform().get_childCount(); i++)
            {
                this.InitializeComponent(root.get_transform().GetChild(i).get_gameObject());
            }
        }

        public void InitializeWidgetPosition(int widgetIndex, Vector3 worldPosition)
        {
            this.InitializeWidgetPosition(this.GetWidget(widgetIndex), worldPosition);
        }

        public void InitializeWidgetPosition(GameObject widget, Vector3 worldPosition)
        {
            if (this.m_initWidgetPositions == null)
            {
                this.m_initWidgetPositions = new List<stInitWidgetPosition>();
            }
            stInitWidgetPosition item = new stInitWidgetPosition();
            item.m_renderFrameStamp = this.m_renderFrameStamp;
            item.m_widget = widget;
            item.m_worldPosition = worldPosition;
            this.m_initWidgetPositions.Add(item);
        }

        public bool IsCanvasEnabled()
        {
            if (this.m_canvas == null)
            {
                return false;
            }
            return this.m_canvas.get_enabled();
        }

        public bool IsClosed()
        {
            return this.m_isClosed;
        }

        public bool IsHided()
        {
            return this.m_isHided;
        }

        public bool IsInFadeIn()
        {
            return this.m_isInFadeIn;
        }

        public bool IsInFadeOut()
        {
            return this.m_isInFadeOut;
        }

        public bool IsNeedClose()
        {
            return this.m_isNeedClose;
        }

        public bool IsNeedFadeIn()
        {
            return (((GameSettings.RenderQuality != SGameRenderQuality.Low) && (this.m_formFadeInAnimationType != enFormFadeAnimationType.None)) && !string.IsNullOrEmpty(this.m_formFadeInAnimationName));
        }

        public bool IsNeedFadeOut()
        {
            return false;
        }

        private bool IsOverlay()
        {
            if (this.m_canvas == null)
            {
                return false;
            }
            return ((this.m_canvas.get_renderMode() == null) || (this.m_canvas.get_worldCamera() == null));
        }

        public bool IsRelatedSceneExist(string sceneName)
        {
            for (int i = 0; i < this.m_relatedScenes.Count; i++)
            {
                if (string.Equals(sceneName, this.m_relatedScenes[i].get_name()))
                {
                    return true;
                }
            }
            return false;
        }

        public void MatchScreen()
        {
            if (this.m_canvasScaler != null)
            {
                this.m_canvasScaler.set_referenceResolution(this.m_referenceResolution);
                if ((((float) Screen.get_width()) / this.m_canvasScaler.get_referenceResolution().x) > (((float) Screen.get_height()) / this.m_canvasScaler.get_referenceResolution().y))
                {
                    if (this.m_fullScreenBG)
                    {
                        this.m_canvasScaler.set_matchWidthOrHeight(0f);
                    }
                    else
                    {
                        this.m_canvasScaler.set_matchWidthOrHeight(1f);
                    }
                }
                else if (this.m_fullScreenBG)
                {
                    this.m_canvasScaler.set_matchWidthOrHeight(1f);
                }
                else
                {
                    this.m_canvasScaler.set_matchWidthOrHeight(0f);
                }
            }
        }

        private void OnDestroy()
        {
            if (this.m_asyncLoadedImages != null)
            {
                this.m_asyncLoadedImages.Clear();
                this.m_asyncLoadedImages = null;
            }
            if (this.m_loadedSpriteDictionary != null)
            {
                this.m_loadedSpriteDictionary.Clear();
                this.m_loadedSpriteDictionary = null;
            }
        }

        public void Open(int sequence, bool exist, int openOrder)
        {
            this.m_isNeedClose = false;
            this.m_isClosed = false;
            this.m_isInFadeIn = false;
            this.m_isInFadeOut = false;
            this.m_clickedEventDispatchedCounter = 0;
            this.m_sequence = sequence;
            this.SetDisplayOrder(openOrder);
            this.m_renderFrameStamp = 0;
            if (!exist)
            {
                this.Initialize();
                this.DispatchFormEvent(enFormEventType.Open);
                for (int i = 0; i < this.m_openedWwiseEvents.Length; i++)
                {
                    if (!string.IsNullOrEmpty(this.m_openedWwiseEvents[i]))
                    {
                        Singleton<CSoundManager>.GetInstance().PostEvent(this.m_openedWwiseEvents[i], null);
                    }
                }
                if (this.IsNeedFadeIn())
                {
                    this.StartFadeIn();
                }
            }
        }

        public void Open(string formPath, Camera camera, int sequence, bool exist, int openOrder)
        {
            this.m_formPath = formPath;
            if (this.m_canvas != null)
            {
                this.m_canvas.set_worldCamera(camera);
                if (camera == null)
                {
                    if (this.m_canvas.get_renderMode() != null)
                    {
                        this.m_canvas.set_renderMode(0);
                    }
                }
                else if (this.m_canvas.get_renderMode() != 1)
                {
                    this.m_canvas.set_renderMode(1);
                }
                this.m_canvas.set_pixelPerfect(true);
            }
            this.RefreshCanvasScaler();
            this.Open(sequence, exist, openOrder);
        }

        private void RefreshCanvasScaler()
        {
            try
            {
                if (this.m_canvasScaler != null)
                {
                    this.m_canvasScaler.set_enabled(false);
                    this.m_canvasScaler.set_enabled(true);
                }
            }
            catch (Exception exception)
            {
                object[] inParameters = new object[] { base.get_name(), exception.Message, exception.StackTrace };
                DebugHelper.Assert(false, "Error form {0}: message: {1}, callstack: {2}", inParameters);
            }
        }

        public void RemoveUIComponent(CUIComponent uiComponent)
        {
            if (this.m_uiComponents.Contains(uiComponent))
            {
                this.m_uiComponents.Remove(uiComponent);
            }
        }

        public void RestorePriority()
        {
            this.SetPriority(this.m_defaultPriority);
        }

        public void SetActive(bool active)
        {
            base.get_gameObject().CustomSetActive(active);
            if (active)
            {
                this.Appear(enFormHideFlag.HideByCustom, true);
            }
            else
            {
                this.Hide(enFormHideFlag.HideByCustom, true);
            }
        }

        private void SetComponentSortingOrder(int sortingOrder)
        {
            for (int i = 0; i < this.m_uiComponents.Count; i++)
            {
                this.m_uiComponents[i].SetSortingOrder(sortingOrder);
            }
        }

        public void SetDisplayOrder(int openOrder)
        {
            object[] inParameters = new object[] { openOrder };
            DebugHelper.Assert(openOrder > 0, "openOrder = {0}, 该值必须大于0", inParameters);
            this.m_openOrder = openOrder;
            if (this.m_canvas != null)
            {
                this.m_sortingOrder = this.CalculateSortingOrder(this.m_priority, this.m_openOrder);
                this.m_canvas.set_sortingOrder(this.m_sortingOrder);
                try
                {
                    if (this.m_canvas.get_enabled())
                    {
                        this.m_canvas.set_enabled(false);
                        this.m_canvas.set_enabled(true);
                    }
                }
                catch (Exception exception)
                {
                    object[] objArray2 = new object[] { base.get_name(), exception.Message, exception.StackTrace };
                    DebugHelper.Assert(false, "Error form {0}: message: {1}, callstack: {2}", objArray2);
                }
            }
            this.SetComponentSortingOrder(this.m_sortingOrder);
        }

        public void SetPriority(enFormPriority priority)
        {
            if (this.m_priority != priority)
            {
                this.m_priority = priority;
                this.SetDisplayOrder(this.m_openOrder);
                this.DispatchChangeFormPriorityEvent();
            }
        }

        public void SetSceneCameraEnable(int index, bool bEnable)
        {
            if (((index >= 0) && (index < this.m_relatedSceneCamera.Count)) && (this.m_relatedSceneCamera[index] != null))
            {
                for (int i = 0; i < this.m_relatedSceneCamera[index].Count; i++)
                {
                    if (this.m_relatedSceneCamera[index][i] != null)
                    {
                        this.m_relatedSceneCamera[index][i].set_enabled(bEnable);
                    }
                }
            }
        }

        private void StartFadeIn()
        {
            if ((this.m_formFadeInAnimationType != enFormFadeAnimationType.None) && !string.IsNullOrEmpty(this.m_formFadeInAnimationName))
            {
                switch (this.m_formFadeInAnimationType)
                {
                    case enFormFadeAnimationType.Animation:
                        this.m_formFadeInAnimationScript = base.get_gameObject().GetComponent<CUIAnimationScript>();
                        if (this.m_formFadeInAnimationScript != null)
                        {
                            ((CUIAnimationScript) this.m_formFadeInAnimationScript).PlayAnimation(this.m_formFadeInAnimationName, true);
                            this.m_isInFadeIn = true;
                        }
                        break;

                    case enFormFadeAnimationType.Animator:
                        this.m_formFadeInAnimationScript = base.get_gameObject().GetComponent<CUIAnimatorScript>();
                        if (this.m_formFadeInAnimationScript != null)
                        {
                            ((CUIAnimatorScript) this.m_formFadeInAnimationScript).PlayAnimator(this.m_formFadeInAnimationName);
                            this.m_isInFadeIn = true;
                        }
                        return;
                }
            }
        }

        private void StartFadeOut()
        {
            if ((this.m_formFadeOutAnimationType != enFormFadeAnimationType.None) && !string.IsNullOrEmpty(this.m_formFadeOutAnimationName))
            {
                switch (this.m_formFadeOutAnimationType)
                {
                    case enFormFadeAnimationType.Animation:
                        this.m_formFadeOutAnimationScript = base.get_gameObject().GetComponent<CUIAnimationScript>();
                        if (this.m_formFadeOutAnimationScript != null)
                        {
                            ((CUIAnimationScript) this.m_formFadeOutAnimationScript).PlayAnimation(this.m_formFadeOutAnimationName, true);
                            this.m_isInFadeOut = true;
                        }
                        break;

                    case enFormFadeAnimationType.Animator:
                        this.m_formFadeOutAnimationScript = base.get_gameObject().GetComponent<CUIAnimatorScript>();
                        if (this.m_formFadeOutAnimationScript != null)
                        {
                            ((CUIAnimatorScript) this.m_formFadeOutAnimationScript).PlayAnimator(this.m_formFadeOutAnimationName);
                            this.m_isInFadeOut = true;
                        }
                        return;
                }
            }
        }

        public bool TurnToClosed(bool ignoreFadeOut)
        {
            this.m_isNeedClose = false;
            this.m_isClosed = true;
            Singleton<EventRouter>.GetInstance().BroadCastEvent<string>(EventID.UI_FORM_CLOSED, this.m_formPath);
            if (ignoreFadeOut || !this.IsNeedFadeOut())
            {
                return true;
            }
            this.StartFadeOut();
            return false;
        }

        private void UpdateASyncLoadedImage()
        {
            if (this.m_asyncLoadedImages != null)
            {
                bool flag = false;
                int index = 0;
                while (index < this.m_asyncLoadedImages.Count)
                {
                    CASyncLoadedImage image = this.m_asyncLoadedImages[index];
                    Image image2 = image.m_image;
                    if (image2 != null)
                    {
                        GameObject obj2 = null;
                        string prefabPath = image.m_prefabPath;
                        if (!this.m_loadedSpriteDictionary.TryGetValue(prefabPath, out obj2) && !flag)
                        {
                            obj2 = CUIUtility.GetSpritePrefeb(prefabPath, image.m_needCached, image.m_unloadBelongedAssetBundleAfterLoaded);
                            this.m_loadedSpriteDictionary.Add(prefabPath, obj2);
                            flag = true;
                        }
                        if (obj2 != null)
                        {
                            image2.set_color(new Color(image2.get_color().r, image2.get_color().g, image2.get_color().b, 1f));
                            image2.SetSprite(obj2, image.m_isShowSpecMatrial);
                            this.m_asyncLoadedImages.RemoveAt(index);
                        }
                        else
                        {
                            index++;
                        }
                    }
                    else
                    {
                        this.m_asyncLoadedImages.RemoveAt(index);
                    }
                }
            }
        }

        private void UpdateFadeIn()
        {
            if (this.m_isInFadeIn)
            {
                switch (this.m_formFadeInAnimationType)
                {
                    case enFormFadeAnimationType.Animation:
                        if ((this.m_formFadeInAnimationScript == null) || ((CUIAnimationScript) this.m_formFadeInAnimationScript).IsAnimationStopped(this.m_formFadeInAnimationName))
                        {
                            this.m_isInFadeIn = false;
                        }
                        break;

                    case enFormFadeAnimationType.Animator:
                        if ((this.m_formFadeInAnimationScript == null) || ((CUIAnimatorScript) this.m_formFadeInAnimationScript).IsAnimationStopped(this.m_formFadeInAnimationName))
                        {
                            this.m_isInFadeIn = false;
                        }
                        return;
                }
            }
        }

        private void UpdateFadeOut()
        {
            if (this.m_isInFadeOut)
            {
                switch (this.m_formFadeOutAnimationType)
                {
                    case enFormFadeAnimationType.Animation:
                        if ((this.m_formFadeOutAnimationScript == null) || ((CUIAnimationScript) this.m_formFadeOutAnimationScript).IsAnimationStopped(this.m_formFadeOutAnimationName))
                        {
                            this.m_isInFadeOut = false;
                        }
                        break;

                    case enFormFadeAnimationType.Animator:
                        if ((this.m_formFadeOutAnimationScript == null) || ((CUIAnimatorScript) this.m_formFadeOutAnimationScript).IsAnimationStopped(this.m_formFadeOutAnimationName))
                        {
                            this.m_isInFadeOut = false;
                        }
                        return;
                }
            }
        }

        private class CASyncLoadedImage
        {
            public Image m_image;
            public bool m_isShowSpecMatrial;
            public bool m_needCached;
            public string m_prefabPath;
            public bool m_unloadBelongedAssetBundleAfterLoaded;

            public CASyncLoadedImage(Image image, string prefabPath, bool needCached, bool unloadBelongedAssetBundleAfterLoaded, bool isShowSpecMatrial = false)
            {
                this.m_image = image;
                this.m_prefabPath = prefabPath;
                this.m_needCached = needCached;
                this.m_unloadBelongedAssetBundleAfterLoaded = unloadBelongedAssetBundleAfterLoaded;
                this.m_isShowSpecMatrial = isShowSpecMatrial;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct stInitWidgetPosition
        {
            public int m_renderFrameStamp;
            public GameObject m_widget;
            public Vector3 m_worldPosition;
        }
    }
}

