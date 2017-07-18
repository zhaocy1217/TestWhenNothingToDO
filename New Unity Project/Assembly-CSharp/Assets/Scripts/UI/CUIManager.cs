namespace Assets.Scripts.UI
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameSystem;
    using Assets.Scripts.Sound;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class CUIManager : Singleton<CUIManager>
    {
        private enUIEventID _confirmId;
        private int _curMaxExchangeCount = 20;
        private ushort _curValidIndex;
        private uint _price;
        private uint _totalPirce;
        private const int c_formCameraDepth = 10;
        private const int c_formCameraMaskLayer = 5;
        private const string K_SEARCH_FORM_PATH = "UGUI/Form/Common/Search/Form_Search.prefab";
        private const string K_STR_SENDER_FORM_PATH = "UGUI/Form/Common/Search/Form_StringSender.prefab";
        private float m_deltaSearchResultHeight;
        private List<int> m_existFormSequences;
        private Camera m_formCamera;
        private ListView<CUIFormScript> m_forms;
        private int m_formSequence;
        private bool m_needSortForms;
        private bool m_needUpdateRaycasterAndHide;
        private ListView<CUIFormScript> m_pooledForms;
        private string m_recommendEvtCallBack;
        private string m_recommendEvtCallBackSingleEnable;
        private uint m_recommendHandlerCMD;
        private Vector2 m_searchBoxOrgSizeDetla;
        private string m_searchEvtCallBack;
        private uint m_searchHandlerCMD;
        private GameObject m_searchRecommendGo;
        private GameObject m_searchResultGo;
        private StringSendboxOnSend m_strSendboxCb;
        private EventSystem m_uiInputEventSystem;
        private GameObject m_uiRoot;
        public OnFormSorted onFormSorted;
        private static string s_formCameraName = "Camera_Form";
        private static string s_uiSceneName = "UI_Scene";
        public static int s_uiSystemRenderFrameCounter;

        public void AddToExistFormSequenceList(int formSequence)
        {
            if (this.m_existFormSequences != null)
            {
                this.m_existFormSequences.Add(formSequence);
            }
        }

        public void ClearEventGraphicsData()
        {
            MemberInfo[] member = typeof(GraphicRaycaster).GetMember("s_SortedGraphics", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.IgnoreCase);
            if ((member != null) && (member.Length == 1))
            {
                MemberInfo info = member[0];
                if ((info != null) && (info.MemberType == MemberTypes.Field))
                {
                    FieldInfo info2 = info as FieldInfo;
                    if (info2 != null)
                    {
                        List<Graphic> list = info2.GetValue(null) as List<Graphic>;
                        if (list != null)
                        {
                            list.Clear();
                        }
                    }
                }
            }
        }

        public void ClearFormPool()
        {
            for (int i = 0; i < this.m_pooledForms.Count; i++)
            {
                Object.Destroy(this.m_pooledForms[i].get_gameObject());
            }
            this.m_pooledForms.Clear();
        }

        public void CloseAllForm(string[] exceptFormNames = new string[](), bool closeImmediately = true, bool clearFormPool = true)
        {
            for (int i = 0; i < this.m_forms.Count; i++)
            {
                bool flag = true;
                if (exceptFormNames != null)
                {
                    for (int j = 0; j < exceptFormNames.Length; j++)
                    {
                        if (string.Equals(this.m_forms[i].m_formPath, exceptFormNames[j]))
                        {
                            flag = false;
                            break;
                        }
                    }
                }
                if (flag)
                {
                    this.m_forms[i].Close();
                }
            }
            if (closeImmediately)
            {
                int formIndex = 0;
                while (formIndex < this.m_forms.Count)
                {
                    if (this.m_forms[formIndex].IsNeedClose() || this.m_forms[formIndex].IsClosed())
                    {
                        if (this.m_forms[formIndex].IsNeedClose())
                        {
                            this.m_forms[formIndex].TurnToClosed(true);
                        }
                        this.RecycleForm(formIndex);
                    }
                    else
                    {
                        formIndex++;
                    }
                }
                if (exceptFormNames != null)
                {
                    this.ProcessFormList(true, true);
                }
            }
            if (clearFormPool)
            {
                this.ClearFormPool();
            }
        }

        public void CloseAllFormExceptLobby(bool closeImmediately = true)
        {
            string[] exceptFormNames = new string[] { CLobbySystem.LOBBY_FORM_PATH, CLobbySystem.SYSENTRY_FORM_PATH, CChatController.ChatFormPath, CLobbySystem.RANKING_BTN_FORM_PATH };
            Singleton<CUIManager>.GetInstance().CloseAllForm(exceptFormNames, closeImmediately, true);
        }

        public void CloseForm(CUIFormScript formScript)
        {
            for (int i = 0; i < this.m_forms.Count; i++)
            {
                if (this.m_forms[i] == formScript)
                {
                    this.m_forms[i].Close();
                }
            }
        }

        public void CloseForm(int formSequence)
        {
            for (int i = 0; i < this.m_forms.Count; i++)
            {
                if (this.m_forms[i].GetSequence() == formSequence)
                {
                    this.m_forms[i].Close();
                }
            }
        }

        public void CloseForm(string formPath)
        {
            for (int i = 0; i < this.m_forms.Count; i++)
            {
                if (string.Equals(this.m_forms[i].m_formPath, formPath))
                {
                    this.m_forms[i].Close();
                }
            }
        }

        public void CloseGroupForm(int group)
        {
            if (group != 0)
            {
                for (int i = 0; i < this.m_forms.Count; i++)
                {
                    if (this.m_forms[i].m_group == group)
                    {
                        this.m_forms[i].Close();
                    }
                }
            }
        }

        public void CloseMessageBox()
        {
            Singleton<CUIManager>.GetInstance().CloseForm("UGUI/Form/Common/Form_MessageBox.prefab");
        }

        public void CloseSendMsgAlert()
        {
            CUIEvent uiEvent = new CUIEvent();
            uiEvent.m_eventID = enUIEventID.Common_SendMsgAlertClose;
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uiEvent);
        }

        public void CloseSmallMessageBox()
        {
            Singleton<CUIManager>.GetInstance().CloseForm("UGUI/Form/Common/Form_SmallMessageBox.prefab");
        }

        public void CloseTips()
        {
            this.CloseForm("UGUI/Form/Common/Form_Tips.prefab");
        }

        private void CreateCamera()
        {
            GameObject obj2 = new GameObject(s_formCameraName);
            obj2.get_transform().SetParent(this.m_uiRoot.get_transform(), true);
            obj2.get_transform().set_localPosition(Vector3.get_zero());
            obj2.get_transform().set_localRotation(Quaternion.get_identity());
            obj2.get_transform().set_localScale(Vector3.get_one());
            Camera camera = obj2.AddComponent<Camera>();
            camera.set_orthographic(true);
            camera.set_orthographicSize(50f);
            camera.set_clearFlags(3);
            camera.set_cullingMask(0x20);
            camera.set_depth(10f);
            this.m_formCamera = camera;
        }

        private void CreateEventSystem()
        {
            this.m_uiInputEventSystem = Object.FindObjectOfType<EventSystem>();
            if (this.m_uiInputEventSystem == null)
            {
                GameObject obj2 = new GameObject("EventSystem");
                this.m_uiInputEventSystem = obj2.AddComponent<EventSystem>();
                obj2.AddComponent<TouchInputModule>();
            }
            this.m_uiInputEventSystem.get_gameObject().get_transform().set_parent(this.m_uiRoot.get_transform());
        }

        private GameObject CreateForm(string formPrefabPath, bool useFormPool)
        {
            GameObject obj2 = null;
            if (useFormPool)
            {
                for (int i = 0; i < this.m_pooledForms.Count; i++)
                {
                    if (string.Equals(formPrefabPath, this.m_pooledForms[i].m_formPath, StringComparison.OrdinalIgnoreCase))
                    {
                        this.m_pooledForms[i].Appear(enFormHideFlag.HideByCustom, true);
                        obj2 = this.m_pooledForms[i].get_gameObject();
                        this.m_pooledForms.RemoveAt(i);
                        break;
                    }
                }
            }
            if (obj2 == null)
            {
                GameObject content = Singleton<CResourceManager>.GetInstance().GetResource(formPrefabPath, typeof(GameObject), enResourceType.UIForm, false, false).m_content;
                if (content == null)
                {
                    return null;
                }
                obj2 = Object.Instantiate(content);
            }
            if (obj2 != null)
            {
                CUIFormScript component = obj2.GetComponent<CUIFormScript>();
                if (component != null)
                {
                    component.m_useFormPool = useFormPool;
                }
            }
            return obj2;
        }

        private void CreateUIRoot()
        {
            this.m_uiRoot = new GameObject("CUIManager");
            GameObject obj2 = GameObject.Find("BootObj");
            if (obj2 != null)
            {
                this.m_uiRoot.get_transform().set_parent(obj2.get_transform());
            }
        }

        private void CreateUISecene()
        {
            GameObject obj2 = new GameObject(s_uiSceneName);
            obj2.get_transform().set_parent(this.m_uiRoot.get_transform());
        }

        public void DisableInput()
        {
            if (this.m_uiInputEventSystem != null)
            {
                this.m_uiInputEventSystem.get_gameObject().CustomSetActive(false);
            }
        }

        public void EnableInput()
        {
            if (this.m_uiInputEventSystem != null)
            {
                this.m_uiInputEventSystem.get_gameObject().CustomSetActive(true);
            }
        }

        public EventSystem GetEventSystem()
        {
            return this.m_uiInputEventSystem;
        }

        public CUIFormScript GetForm(int formSequence)
        {
            for (int i = 0; i < this.m_forms.Count; i++)
            {
                if (((this.m_forms[i].GetSequence() == formSequence) && !this.m_forms[i].IsNeedClose()) && !this.m_forms[i].IsClosed())
                {
                    return this.m_forms[i];
                }
            }
            return null;
        }

        public CUIFormScript GetForm(string formPath)
        {
            for (int i = 0; i < this.m_forms.Count; i++)
            {
                if ((this.m_forms[i].m_formPath.Equals(formPath) && !this.m_forms[i].IsNeedClose()) && !this.m_forms[i].IsClosed())
                {
                    return this.m_forms[i];
                }
            }
            return null;
        }

        private string GetFormName(string formPath)
        {
            return CFileManager.EraseExtension(CFileManager.GetFullName(formPath));
        }

        public int GetFormOpenOrder(int formSequence)
        {
            int index = this.m_existFormSequences.IndexOf(formSequence);
            return ((index < 0) ? 0 : (index + 1));
        }

        public ListView<CUIFormScript> GetForms()
        {
            return this.m_forms;
        }

        public string GetRuleTextContent(int txtKey)
        {
            ResRuleText dataByKey = GameDataMgr.s_ruleTextDatabin.GetDataByKey((long) txtKey);
            if (dataByKey != null)
            {
                return StringHelper.UTF8BytesToString(ref dataByKey.szContent);
            }
            return null;
        }

        public string GetRuleTextTitle(int txtKey)
        {
            ResRuleText dataByKey = GameDataMgr.s_ruleTextDatabin.GetDataByKey((long) txtKey);
            if (dataByKey != null)
            {
                return StringHelper.UTF8BytesToString(ref dataByKey.szTitle);
            }
            return null;
        }

        public CUIFormScript GetTopForm()
        {
            CUIFormScript script = null;
            for (int i = 0; i < this.m_forms.Count; i++)
            {
                if (this.m_forms[i] != null)
                {
                    if (script == null)
                    {
                        script = this.m_forms[i];
                    }
                    else if (this.m_forms[i].GetSortingOrder() > script.GetSortingOrder())
                    {
                        script = this.m_forms[i];
                    }
                }
            }
            return script;
        }

        private CUIFormScript GetUnClosedForm(string formPath)
        {
            for (int i = 0; i < this.m_forms.Count; i++)
            {
                if (this.m_forms[i].m_formPath.Equals(formPath) && !this.m_forms[i].IsClosed())
                {
                    return this.m_forms[i];
                }
            }
            return null;
        }

        public bool HasForm()
        {
            return (this.m_forms.Count > 0);
        }

        public override void Init()
        {
            this.m_forms = new ListView<CUIFormScript>();
            this.m_pooledForms = new ListView<CUIFormScript>();
            this.m_formSequence = 0;
            this.m_existFormSequences = new List<int>();
            s_uiSystemRenderFrameCounter = 0;
            this.CreateUIRoot();
            this.CreateEventSystem();
            this.CreateCamera();
            this.CreateUISecene();
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.UI_OnFormPriorityChanged, new CUIEventManager.OnUIEventHandler(this.OnFormPriorityChanged));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.UI_OnFormVisibleChanged, new CUIEventManager.OnUIEventHandler(this.OnFormVisibleChanged));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Activity_OnAddExchangeCount, new CUIEventManager.OnUIEventHandler(this.OnAddExchangeCount));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Activity_OnDecreseExchangeCount, new CUIEventManager.OnUIEventHandler(this.OnDecreaseExchangeCount));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Activity_OnMaxExchangeCount, new CUIEventManager.OnUIEventHandler(this.OnMaxExchangeCount));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SearchBox_CloseForm, new CUIEventManager.OnUIEventHandler(this.SearchBox_OnClose));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.StrSenderBox_OnSend, new CUIEventManager.OnUIEventHandler(this.OnStringSenderBoxSend));
        }

        public bool IsTipsFormExist()
        {
            return (this.GetForm("UGUI/Form/Common/Form_Tips.prefab") != null);
        }

        public void LateUpdate()
        {
            for (int i = 0; i < this.m_forms.Count; i++)
            {
                this.m_forms[i].CustomLateUpdate();
            }
            s_uiSystemRenderFrameCounter++;
        }

        public void LoadSoundBank()
        {
            Singleton<CSoundManager>.GetInstance().LoadBank("UI", CSoundManager.BankType.Global);
        }

        public void LoadUIScenePrefab(string sceneName, CUIFormScript formScript)
        {
            if ((formScript != null) && !formScript.IsRelatedSceneExist(sceneName))
            {
                formScript.AddRelatedScene(CUICommonSystem.GetAnimation3DOjb(sceneName), sceneName);
            }
        }

        private void OnAddExchangeCount(CUIEvent uiEvent)
        {
            CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
            Text component = srcFormScript.get_gameObject().get_transform().FindChild("Panel/Count").get_gameObject().GetComponent<Text>();
            if (component != null)
            {
                int num = int.Parse(component.get_text()) + 1;
                num = Math.Min(Math.Min(num, this._curMaxExchangeCount), 300);
                component.set_text(num.ToString());
                Text text2 = srcFormScript.get_gameObject().get_transform().FindChild("Panel/Jifen").get_gameObject().GetComponent<Text>();
                if ((this._price > 0) && (this._totalPirce >= this._price))
                {
                    text2.set_text(string.Format(Singleton<CTextManager>.GetInstance().GetText("ExchangeNumSelectPoint"), this._price * num, this._totalPirce));
                    text2.get_gameObject().CustomSetActive(true);
                }
                else
                {
                    text2.get_gameObject().CustomSetActive(false);
                }
                Button btn = srcFormScript.get_gameObject().get_transform().FindChild("Panel/Button_Count_Down").get_gameObject().GetComponent<Button>();
                Button button2 = srcFormScript.get_gameObject().get_transform().FindChild("Panel/Button_Count_Up").get_gameObject().GetComponent<Button>();
                if (num == 1)
                {
                    CUICommonSystem.SetButtonEnable(btn, false, false, true);
                    CUICommonSystem.SetButtonEnable(button2, true, true, true);
                }
                else if (num == this._curMaxExchangeCount)
                {
                    CUICommonSystem.SetButtonEnable(btn, true, true, true);
                    CUICommonSystem.SetButtonEnable(button2, false, false, true);
                }
                else
                {
                    CUICommonSystem.SetButtonEnable(btn, true, true, true);
                    CUICommonSystem.SetButtonEnable(button2, true, true, true);
                }
                CUIEventScript script2 = srcFormScript.get_gameObject().get_transform().FindChild("Panel/Button_Exchange").get_gameObject().GetComponent<CUIEventScript>();
                stUIEventParams eventParams = new stUIEventParams();
                eventParams.commonUInt32Param1 = (uint) num;
                eventParams.commonUInt16Param1 = this._curValidIndex;
                script2.SetUIEvent(enUIEventType.Click, this._confirmId, eventParams);
            }
        }

        private void OnDecreaseExchangeCount(CUIEvent uiEvent)
        {
            CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
            Text component = srcFormScript.get_gameObject().get_transform().FindChild("Panel/Count").get_gameObject().GetComponent<Text>();
            if (component != null)
            {
                int num = int.Parse(component.get_text()) - 1;
                num = Math.Max(num, 1);
                component.set_text(num.ToString());
                Text text2 = srcFormScript.get_gameObject().get_transform().FindChild("Panel/Jifen").get_gameObject().GetComponent<Text>();
                if ((this._price > 0) && (this._totalPirce >= this._price))
                {
                    text2.set_text(string.Format(Singleton<CTextManager>.GetInstance().GetText("ExchangeNumSelectPoint"), this._price * num, this._totalPirce));
                    text2.get_gameObject().CustomSetActive(true);
                }
                else
                {
                    text2.get_gameObject().CustomSetActive(false);
                }
                Button btn = srcFormScript.get_gameObject().get_transform().FindChild("Panel/Button_Count_Down").get_gameObject().GetComponent<Button>();
                Button button2 = srcFormScript.get_gameObject().get_transform().FindChild("Panel/Button_Count_Up").get_gameObject().GetComponent<Button>();
                if (num == 1)
                {
                    CUICommonSystem.SetButtonEnable(btn, false, false, true);
                    CUICommonSystem.SetButtonEnable(button2, true, true, true);
                }
                else if (num == this._curMaxExchangeCount)
                {
                    CUICommonSystem.SetButtonEnable(btn, true, true, true);
                    CUICommonSystem.SetButtonEnable(button2, false, false, true);
                }
                else
                {
                    CUICommonSystem.SetButtonEnable(btn, true, true, true);
                    CUICommonSystem.SetButtonEnable(button2, true, true, true);
                }
                CUIEventScript script2 = srcFormScript.get_gameObject().get_transform().FindChild("Panel/Button_Exchange").get_gameObject().GetComponent<CUIEventScript>();
                stUIEventParams eventParams = new stUIEventParams();
                eventParams.commonUInt32Param1 = (uint) num;
                eventParams.commonUInt16Param1 = this._curValidIndex;
                script2.SetUIEvent(enUIEventType.Click, this._confirmId, eventParams);
            }
        }

        private void OnFormPriorityChanged(CUIEvent uiEvent)
        {
            this.m_needSortForms = true;
        }

        private void OnFormVisibleChanged(CUIEvent uiEvent)
        {
            this.m_needUpdateRaycasterAndHide = true;
        }

        private void OnMaxExchangeCount(CUIEvent uiEvent)
        {
            CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
            Text component = srcFormScript.get_gameObject().get_transform().FindChild("Panel/Count").get_gameObject().GetComponent<Text>();
            if (component != null)
            {
                component.set_text(this._curMaxExchangeCount.ToString());
                Text text2 = srcFormScript.get_gameObject().get_transform().FindChild("Panel/Jifen").get_gameObject().GetComponent<Text>();
                if ((this._price > 0) && (this._totalPirce >= this._price))
                {
                    text2.set_text(string.Format(Singleton<CTextManager>.GetInstance().GetText("ExchangeNumSelectPoint"), this._price * this._curMaxExchangeCount, this._totalPirce));
                    text2.get_gameObject().CustomSetActive(true);
                }
                else
                {
                    text2.get_gameObject().CustomSetActive(false);
                }
                Button btn = srcFormScript.get_gameObject().get_transform().FindChild("Panel/Button_Count_Down").get_gameObject().GetComponent<Button>();
                Button button2 = srcFormScript.get_gameObject().get_transform().FindChild("Panel/Button_Count_Up").get_gameObject().GetComponent<Button>();
                CUICommonSystem.SetButtonEnable(btn, true, true, true);
                CUICommonSystem.SetButtonEnable(button2, false, false, true);
                CUIEventScript script2 = srcFormScript.get_gameObject().get_transform().FindChild("Panel/Button_Exchange").get_gameObject().GetComponent<CUIEventScript>();
                stUIEventParams eventParams = new stUIEventParams();
                eventParams.commonUInt32Param1 = (uint) this._curMaxExchangeCount;
                eventParams.commonUInt16Param1 = this._curValidIndex;
                script2.SetUIEvent(enUIEventType.Click, this._confirmId, eventParams);
            }
        }

        public void OnStringSenderBoxSend(CUIEvent evt)
        {
            if (this.m_strSendboxCb != null)
            {
                Text component = evt.m_srcFormScript.GetWidget(0).get_transform().Find("Text").GetComponent<Text>();
                this.m_strSendboxCb(component.get_text());
            }
            this.CloseForm(evt.m_srcFormScript);
        }

        public void OpenAwardTip(CUseable[] items, string title = new string(), bool playSound = false, enUIEventID eventID = 0, bool displayAll = false, bool forceNotGoToBag = false, string formPath = "Form_Award")
        {
            if (items != null)
            {
                int num = 10;
                int amount = Mathf.Min(items.Length, num);
                CUIFormScript formScript = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/Common/" + formPath, false, true);
                if (formScript != null)
                {
                    formScript.get_transform().FindChild("btnGroup/Button_Back").GetComponent<CUIEventScript>().m_onClickEventID = eventID;
                    if (title != null)
                    {
                        Utility.GetComponetInChild<Text>(formScript.get_gameObject(), "bg/Title").set_text(title);
                    }
                    CUIListScript component = formScript.get_transform().FindChild("IconContainer").get_gameObject().GetComponent<CUIListScript>();
                    component.SetElementAmount(amount);
                    for (int i = 0; i < amount; i++)
                    {
                        if ((component.GetElemenet(i) != null) && (items[i] != null))
                        {
                            GameObject itemCell = component.GetElemenet(i).get_gameObject();
                            CUICommonSystem.SetItemCell(formScript, itemCell, items[i], true, displayAll, false, false);
                            itemCell.CustomSetActive(true);
                            itemCell.get_transform().FindChild("ItemName").GetComponent<Text>().set_text(items[i].m_name);
                            if (playSound)
                            {
                                COM_REWARDS_TYPE mapRewardType = items[i].MapRewardType;
                                if (mapRewardType != COM_REWARDS_TYPE.COM_REWARDS_TYPE_COIN)
                                {
                                    if (mapRewardType == COM_REWARDS_TYPE.COM_REWARDS_TYPE_AP)
                                    {
                                        goto Label_0164;
                                    }
                                    if (mapRewardType == COM_REWARDS_TYPE.COM_REWARDS_TYPE_DIAMOND)
                                    {
                                        goto Label_014E;
                                    }
                                }
                                else
                                {
                                    Singleton<CSoundManager>.GetInstance().PostEvent("UI_hall_add_coin", null);
                                }
                            }
                        }
                        continue;
                    Label_014E:
                        Singleton<CSoundManager>.GetInstance().PostEvent("UI_hall_add_diamond", null);
                        continue;
                    Label_0164:
                        Singleton<CSoundManager>.GetInstance().PostEvent("UI_hall_add_physical_power", null);
                    }
                    CUIEventScript script3 = formScript.get_transform().Find("btnGroup/Button_Use").GetComponent<CUIEventScript>();
                    script3.get_gameObject().CustomSetActive(false);
                    if ((!forceNotGoToBag && (amount == 1)) && (items[0].m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP))
                    {
                        CItem item = items[0] as CItem;
                        if (((item.m_itemData.bType == 4) || (item.m_itemData.bType == 1)) || (item.m_itemData.bType == 11))
                        {
                            CUseable useableByBaseID = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetUseableContainer(enCONTAINER_TYPE.ITEM).GetUseableByBaseID(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, item.m_baseID);
                            if (useableByBaseID != null)
                            {
                                script3.get_gameObject().CustomSetActive(true);
                                script3.m_onClickEventParams.iconUseable = useableByBaseID;
                                script3.m_onClickEventParams.tag = Mathf.Min(item.m_stackCount, useableByBaseID.m_stackCount);
                            }
                        }
                    }
                }
            }
        }

        public void OpenEditForm(string title, string editContent, enUIEventID confirmEventId = 0)
        {
            CUIFormScript script = this.OpenForm("UGUI/Form/Common/Form_Edit.prefab", false, true);
            DebugHelper.Assert(script != null, "CUIManager.OpenEditForm(): form == null!!!");
            if (script != null)
            {
                if (title != null)
                {
                    script.GetWidget(0).GetComponent<Text>().set_text(title);
                }
                if (editContent != null)
                {
                    script.GetWidget(1).GetComponent<Text>().set_text(editContent);
                }
                script.GetWidget(2).GetComponent<CUIEventScript>().SetUIEvent(enUIEventType.Click, confirmEventId);
            }
        }

        public void OpenExchangeCountSelectForm(CUseable item, int count, enUIEventID confirmId, stUIEventParams par, uint price = 0, uint totalPrice = 0)
        {
            if (item != null)
            {
                this._curMaxExchangeCount = Math.Min(count, 300);
                this._confirmId = confirmId;
                this._curValidIndex = par.commonUInt16Param1;
                this._price = price;
                this._totalPirce = totalPrice;
                CUIFormScript formScript = this.OpenForm("UGUI/Form/Common/Form_ExchangeCountSelect.prefab", false, true);
                if (formScript != null)
                {
                    int num = 1;
                    formScript.get_gameObject().get_transform().FindChild("Panel/Count").get_gameObject().GetComponent<Text>().set_text(num.ToString());
                    Text component = formScript.get_gameObject().get_transform().FindChild("Panel/Jifen").get_gameObject().GetComponent<Text>();
                    if ((this._price > 0) && (this._totalPirce >= this._price))
                    {
                        component.set_text(string.Format(Singleton<CTextManager>.GetInstance().GetText("ExchangeNumSelectPoint"), this._price, this._totalPirce));
                        component.get_gameObject().CustomSetActive(true);
                    }
                    else
                    {
                        component.get_gameObject().CustomSetActive(false);
                    }
                    Button btn = formScript.get_gameObject().get_transform().FindChild("Panel/Button_Count_Down").get_gameObject().GetComponent<Button>();
                    Button button2 = formScript.get_gameObject().get_transform().FindChild("Panel/Button_Count_Up").get_gameObject().GetComponent<Button>();
                    CUICommonSystem.SetButtonEnable(btn, false, false, true);
                    CUICommonSystem.SetButtonEnable(button2, true, true, true);
                    CUIUtility.SetImageSprite(formScript.get_gameObject().get_transform().FindChild("Panel/Slot/Icon").get_gameObject().GetComponent<Image>(), item.GetIconPath(), formScript, false, false, false, false);
                    formScript.get_gameObject().get_transform().FindChild("Panel/Name").get_gameObject().GetComponent<Text>().set_text(item.m_name);
                    Text text4 = formScript.get_gameObject().get_transform().FindChild("Panel/Slot/lblIconCount").get_gameObject().GetComponent<Text>();
                    text4.set_text(item.m_stackCount.ToString());
                    text4.get_gameObject().CustomSetActive(true);
                    CUIEventScript script2 = formScript.get_gameObject().get_transform().FindChild("Panel/Button_Exchange").get_gameObject().GetComponent<CUIEventScript>();
                    stUIEventParams eventParams = new stUIEventParams();
                    eventParams.commonUInt32Param1 = 1;
                    eventParams.commonUInt16Param1 = this._curValidIndex;
                    script2.SetUIEvent(enUIEventType.Click, this._confirmId, eventParams);
                }
            }
        }

        public CUIFormScript OpenForm(string formPath, bool useFormPool, bool useCameraRenderMode = true)
        {
            CUIFormScript unClosedForm = this.GetUnClosedForm(formPath);
            if ((unClosedForm != null) && unClosedForm.m_isSingleton)
            {
                this.RemoveFromExistFormSequenceList(unClosedForm.GetSequence());
                this.AddToExistFormSequenceList(this.m_formSequence);
                int formOpenOrder = this.GetFormOpenOrder(this.m_formSequence);
                unClosedForm.Open(this.m_formSequence, true, formOpenOrder);
                this.m_formSequence++;
                this.m_needSortForms = true;
                return unClosedForm;
            }
            GameObject obj2 = this.CreateForm(formPath, useFormPool);
            if (obj2 == null)
            {
                return null;
            }
            if (!obj2.get_activeSelf())
            {
                obj2.CustomSetActive(true);
            }
            string formName = this.GetFormName(formPath);
            obj2.set_name(formName);
            if (obj2.get_transform().get_parent() != this.m_uiRoot.get_transform())
            {
                obj2.get_transform().SetParent(this.m_uiRoot.get_transform());
            }
            unClosedForm = obj2.GetComponent<CUIFormScript>();
            if (unClosedForm != null)
            {
                this.AddToExistFormSequenceList(this.m_formSequence);
                int openOrder = this.GetFormOpenOrder(this.m_formSequence);
                unClosedForm.Open(formPath, !useCameraRenderMode ? null : this.m_formCamera, this.m_formSequence, false, openOrder);
                if (unClosedForm.m_group > 0)
                {
                    this.CloseGroupForm(unClosedForm.m_group);
                }
                this.m_forms.Add(unClosedForm);
            }
            this.m_formSequence++;
            this.m_needSortForms = true;
            return unClosedForm;
        }

        public void OpenInfoForm(int txtKey)
        {
            ResRuleText dataByKey = GameDataMgr.s_ruleTextDatabin.GetDataByKey((long) txtKey);
            if (dataByKey != null)
            {
                string title = StringHelper.UTF8BytesToString(ref dataByKey.szTitle);
                string info = StringHelper.UTF8BytesToString(ref dataByKey.szContent);
                Singleton<CUIManager>.GetInstance().OpenInfoForm(title, info);
            }
        }

        public void OpenInfoForm(string title = new string(), string info = new string())
        {
            CUIFormScript script = this.OpenForm("UGUI/Form/Common/Form_Info.prefab", false, true);
            DebugHelper.Assert(script != null, "CUIManager.OpenInfoForm(): form == null!!!");
            if (script != null)
            {
                if (title != null)
                {
                    script.GetWidget(0).GetComponent<Text>().set_text(title);
                }
                if (info != null)
                {
                    script.GetWidget(1).GetComponent<Text>().set_text(info);
                }
            }
        }

        public void OpenInputBox(string title, string inputTip, enUIEventID confirmID)
        {
            stUIEventParams par = new stUIEventParams();
            this.OpenInputBoxBase(title, inputTip, confirmID, enUIEventID.None, par, "确定", "取消");
        }

        public void OpenInputBox(string title, string inputTip, enUIEventID confirmID, stUIEventParams par)
        {
            this.OpenInputBoxBase(title, inputTip, confirmID, enUIEventID.None, par, "确定", "取消");
        }

        public void OpenInputBox(string title, string inputTip, enUIEventID confirmID, enUIEventID cancelID, stUIEventParams par)
        {
            this.OpenInputBoxBase(title, inputTip, confirmID, cancelID, par, "确定", "取消");
        }

        public void OpenInputBox(string title, string inputTip, enUIEventID confirmID, enUIEventID cancelID, stUIEventParams par, string confirmStr, string cancelStr)
        {
            this.OpenInputBoxBase(title, inputTip, confirmID, cancelID, par, confirmStr, cancelStr);
        }

        private void OpenInputBoxBase(string title, string inputTip, enUIEventID confirmID, enUIEventID cancelID, stUIEventParams par, string confirmStr = "确定", string cancelStr = "取消")
        {
            CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/Common/Form_InputBox.prefab", false, false);
            GameObject obj2 = null;
            if (script != null)
            {
                obj2 = script.get_gameObject();
            }
            if (obj2 != null)
            {
                GameObject obj3 = obj2.get_transform().Find("Panel/btnGroup/Button_Confirm").get_gameObject();
                obj3.GetComponentInChildren<Text>().set_text(confirmStr);
                GameObject obj4 = obj2.get_transform().Find("Panel/btnGroup/Button_Cancel").get_gameObject();
                obj4.GetComponentInChildren<Text>().set_text(cancelStr);
                obj2.get_transform().Find("Panel/title/Text").GetComponent<Text>().set_text(title);
                obj2.get_transform().Find("Panel/inputText/Placeholder").GetComponent<Text>().set_text(inputTip);
                CUIEventScript component = obj3.GetComponent<CUIEventScript>();
                CUIEventScript script3 = obj4.GetComponent<CUIEventScript>();
                component.SetUIEvent(enUIEventType.Click, confirmID, par);
                script3.SetUIEvent(enUIEventType.Click, cancelID, par);
            }
        }

        public void OpenMessageBox(string strContent, bool isContentLeftAlign = false)
        {
            stUIEventParams par = new stUIEventParams();
            this.OpenMessageBoxBase(strContent, false, enUIEventID.None, enUIEventID.None, par, isContentLeftAlign, string.Empty, string.Empty, string.Empty, 0, enUIEventID.None);
        }

        public void OpenMessageBox(string strContent, enUIEventID confirmID, bool isContentLeftAlign = false)
        {
            stUIEventParams par = new stUIEventParams();
            this.OpenMessageBoxBase(strContent, false, confirmID, enUIEventID.None, par, isContentLeftAlign, string.Empty, string.Empty, string.Empty, 0, enUIEventID.None);
        }

        public void OpenMessageBox(string strContent, enUIEventID confirmID, stUIEventParams par, bool isContentLeftAlign = false)
        {
            this.OpenMessageBoxBase(strContent, false, confirmID, enUIEventID.None, par, isContentLeftAlign, string.Empty, string.Empty, string.Empty, 0, enUIEventID.None);
        }

        public void OpenMessageBox(string strContent, enUIEventID confirmID, stUIEventParams par, string confirmStr, bool isContentLeftAlign = false)
        {
            this.OpenMessageBoxBase(strContent, false, confirmID, enUIEventID.None, par, isContentLeftAlign, confirmStr, string.Empty, string.Empty, 0, enUIEventID.None);
        }

        public void OpenMessageBox(string strContent, enUIEventID confirmID, stUIEventParams par, string confirmStr, string titleStr, bool isContentLeftAlign = false)
        {
            this.OpenMessageBoxBase(strContent, false, confirmID, enUIEventID.None, par, isContentLeftAlign, confirmStr, string.Empty, titleStr, 0, enUIEventID.None);
        }

        private void OpenMessageBoxBase(string strContent, bool isHaveCancelBtn, enUIEventID confirmID, enUIEventID cancelID, stUIEventParams par, bool isContentLeftAlign = false, string confirmStr = "", string cancelStr = "", string titleStr = "", int autoCloseTime = 0, enUIEventID timeUpID = 0)
        {
            CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/Common/Form_MessageBox.prefab", false, false);
            if (script != null)
            {
                GameObject obj2 = script.get_gameObject();
                if (obj2 != null)
                {
                    if (confirmStr == string.Empty)
                    {
                        confirmStr = Singleton<CTextManager>.GetInstance().GetText("Common_Confirm");
                    }
                    if (cancelStr == string.Empty)
                    {
                        cancelStr = Singleton<CTextManager>.GetInstance().GetText("Common_Cancel");
                    }
                    if (titleStr == string.Empty)
                    {
                        titleStr = Singleton<CTextManager>.GetInstance().GetText("Common_MsgBox_Title");
                    }
                    GameObject obj3 = obj2.get_transform().Find("Panel/Panel/btnGroup/Button_Confirm").get_gameObject();
                    obj3.GetComponentInChildren<Text>().set_text(confirmStr);
                    GameObject obj4 = obj2.get_transform().Find("Panel/Panel/btnGroup/Button_Cancel").get_gameObject();
                    obj4.GetComponentInChildren<Text>().set_text(cancelStr);
                    obj2.get_transform().Find("Panel/Panel/title/Text").get_gameObject().GetComponentInChildren<Text>().set_text(titleStr);
                    Text component = obj2.get_transform().Find("Panel/Panel/Text").GetComponent<Text>();
                    component.set_text(strContent);
                    if (!isHaveCancelBtn)
                    {
                        obj4.CustomSetActive(false);
                    }
                    else
                    {
                        obj4.CustomSetActive(true);
                    }
                    CUIEventScript script2 = obj3.GetComponent<CUIEventScript>();
                    CUIEventScript script3 = obj4.GetComponent<CUIEventScript>();
                    script2.SetUIEvent(enUIEventType.Click, confirmID, par);
                    script3.SetUIEvent(enUIEventType.Click, cancelID, par);
                    if (isContentLeftAlign)
                    {
                        component.set_alignment(3);
                    }
                    if (autoCloseTime != 0)
                    {
                        Transform transform = script.get_transform().Find("closeTimer");
                        if (transform != null)
                        {
                            CUITimerScript script4 = transform.GetComponent<CUITimerScript>();
                            if (script4 != null)
                            {
                                script4.SetTotalTime((float) autoCloseTime);
                                script4.StartTimer();
                                script4.m_eventIDs[1] = timeUpID;
                                script4.m_eventParams[1] = par;
                            }
                        }
                    }
                    this.CloseSendMsgAlert();
                }
            }
        }

        public void OpenMessageBoxWithCancel(string strContent, enUIEventID confirmID, enUIEventID cancelID, bool isContentLeftAlign = false)
        {
            this.OpenMessageBoxBase(strContent, true, confirmID, cancelID, new stUIEventParams(), isContentLeftAlign, string.Empty, string.Empty, string.Empty, 0, enUIEventID.None);
        }

        public void OpenMessageBoxWithCancel(string strContent, enUIEventID confirmID, enUIEventID cancelID, stUIEventParams par, bool isContentLeftAlign = false)
        {
            this.OpenMessageBoxBase(strContent, true, confirmID, cancelID, par, isContentLeftAlign, string.Empty, string.Empty, string.Empty, 0, enUIEventID.None);
        }

        public void OpenMessageBoxWithCancel(string strContent, enUIEventID confirmID, enUIEventID cancelID, string confirmStr, string cancelStr, bool isContentLeftAlign = false)
        {
            stUIEventParams par = new stUIEventParams();
            this.OpenMessageBoxBase(strContent, true, confirmID, cancelID, par, isContentLeftAlign, confirmStr, cancelStr, string.Empty, 0, enUIEventID.None);
        }

        public void OpenMessageBoxWithCancel(string strContent, enUIEventID confirmID, enUIEventID cancelID, stUIEventParams param, string confirmStr, string cancelStr, bool isContentLeftAlign = false)
        {
            this.OpenMessageBoxBase(strContent, true, confirmID, cancelID, param, isContentLeftAlign, confirmStr, cancelStr, string.Empty, 0, enUIEventID.None);
        }

        public void OpenMessageBoxWithCancelAndAutoClose(string strContent, enUIEventID confirmID, enUIEventID cancelID, stUIEventParams par, bool isContentLeftAlign = false, int autoCloseTime = 0, enUIEventID timeUpID = 0)
        {
            this.OpenMessageBoxBase(strContent, true, confirmID, cancelID, par, isContentLeftAlign, string.Empty, string.Empty, string.Empty, autoCloseTime, timeUpID);
        }

        public bool OpenSearchBox(string title, string recommendItemPath, string resultItemPath, uint recommendHandleCMD, uint searchHandleCMD, string evtCallbackSearch, string evtCallbackRecommend, string evtCallbackRecommendSingleEnable = new string())
        {
            if (this.GetForm("UGUI/Form/Common/Search/Form_Search.prefab") != null)
            {
                return false;
            }
            CUIFormScript script = this.OpenForm("UGUI/Form/Common/Search/Form_Search.prefab", false, true);
            GameObject widget = script.GetWidget(2);
            if (title != null)
            {
                widget.GetComponent<Text>().set_text(title);
            }
            this.m_searchResultGo = script.GetWidget(0);
            this.m_searchRecommendGo = script.GetWidget(1);
            GameObject content = Singleton<CResourceManager>.GetInstance().GetResource(recommendItemPath, typeof(GameObject), enResourceType.UIPrefab, false, false).m_content;
            content.get_transform().set_parent(this.m_searchRecommendGo.get_transform());
            Transform transform = this.m_searchResultGo.get_transform().Find("Result");
            this.m_deltaSearchResultHeight = (content.get_transform() as RectTransform).get_rect().get_height() - (transform as RectTransform).get_rect().get_height();
            this.m_searchResultGo.CustomSetActive(false);
            this.m_searchRecommendGo.CustomSetActive(false);
            NetMsgDelegate handler = new NetMsgDelegate(this.SearchBox_ResultHandler);
            this.m_searchHandlerCMD = searchHandleCMD;
            Singleton<NetworkModule>.GetInstance().RegisterMsgHandler(searchHandleCMD, handler);
            handler = new NetMsgDelegate(this.SearchBox_RecommendHandler);
            this.m_recommendHandlerCMD = recommendHandleCMD;
            Singleton<NetworkModule>.GetInstance().RegisterMsgHandler(recommendHandleCMD, handler);
            this.m_searchEvtCallBack = evtCallbackSearch;
            this.m_recommendEvtCallBack = evtCallbackRecommend;
            this.m_recommendEvtCallBackSingleEnable = evtCallbackRecommendSingleEnable;
            return true;
        }

        public void OpenSendMsgAlert(int autoCloseTime = 5, enUIEventID timeUpEventId = 0)
        {
            CUIEvent uiEvent = new CUIEvent();
            uiEvent.m_eventID = enUIEventID.Common_SendMsgAlertOpen;
            stUIEventParams params = new stUIEventParams();
            params.tag = autoCloseTime;
            params.tag2 = (int) timeUpEventId;
            uiEvent.m_eventParams = params;
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uiEvent);
        }

        public void OpenSendMsgAlert(string txtContent, int autoCloseTime = 10, enUIEventID timeUpEventId = 0)
        {
            CUIEvent uiEvent = new CUIEvent();
            uiEvent.m_eventID = enUIEventID.Common_SendMsgAlertOpen;
            stUIEventParams params = new stUIEventParams();
            params.tagStr = txtContent;
            params.tag = autoCloseTime;
            params.tag2 = (int) timeUpEventId;
            uiEvent.m_eventParams = params;
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uiEvent);
        }

        public void OpenSmallMessageBox(string strContent, bool isHaveCancelBtn, enUIEventID confirmID, enUIEventID cancelID, stUIEventParams par, int autoCloseTime = 0, enUIEventID closeTimeID = 0, string confirmStr = "", string cancelStr = "", bool isContentLeftAlign = false)
        {
            CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/Common/Form_SmallMessageBox.prefab", false, false);
            if (script != null)
            {
                GameObject obj2 = script.get_gameObject();
                if (obj2 != null)
                {
                    if (string.IsNullOrEmpty(confirmStr))
                    {
                        confirmStr = Singleton<CTextManager>.GetInstance().GetText("Common_Confirm");
                    }
                    if (string.IsNullOrEmpty(cancelStr))
                    {
                        cancelStr = Singleton<CTextManager>.GetInstance().GetText("Common_Cancel");
                    }
                    GameObject obj3 = obj2.get_transform().Find("Panel/Panel/btnGroup/Button_Confirm").get_gameObject();
                    obj3.GetComponentInChildren<Text>().set_text(confirmStr);
                    GameObject obj4 = obj2.get_transform().Find("Panel/Panel/btnGroup/Button_Cancel").get_gameObject();
                    obj4.GetComponentInChildren<Text>().set_text(cancelStr);
                    Text component = obj2.get_transform().Find("Panel/Panel/Text").GetComponent<Text>();
                    component.set_text(strContent);
                    if (!isHaveCancelBtn)
                    {
                        obj4.CustomSetActive(false);
                    }
                    else
                    {
                        obj4.CustomSetActive(true);
                    }
                    CUIEventScript script2 = obj3.GetComponent<CUIEventScript>();
                    CUIEventScript script3 = obj4.GetComponent<CUIEventScript>();
                    script2.SetUIEvent(enUIEventType.Click, confirmID, par);
                    script3.SetUIEvent(enUIEventType.Click, cancelID, par);
                    if (isContentLeftAlign)
                    {
                        component.set_alignment(3);
                    }
                    if (autoCloseTime != 0)
                    {
                        Transform transform = script.get_transform().Find("closeTimer");
                        if (transform != null)
                        {
                            CUITimerScript script4 = transform.GetComponent<CUITimerScript>();
                            if (script4 != null)
                            {
                                if (closeTimeID > enUIEventID.None)
                                {
                                    script4.m_eventIDs[1] = closeTimeID;
                                }
                                script4.SetTotalTime((float) autoCloseTime);
                                script4.StartTimer();
                            }
                        }
                    }
                    this.CloseSendMsgAlert();
                }
            }
        }

        public void OpenStringSenderBox(string title, string desc, string stringPlacer, StringSendboxOnSend onSendCallback, string defaultString = "")
        {
            if (this.GetForm("UGUI/Form/Common/Search/Form_StringSender.prefab") == null)
            {
                CUIFormScript script = this.OpenForm("UGUI/Form/Common/Search/Form_StringSender.prefab", true, true);
                if (title != null)
                {
                    script.GetWidget(2).GetComponent<Text>().set_text(title);
                }
                if (desc != null)
                {
                    script.GetWidget(1).GetComponent<Text>().set_text(desc);
                }
                if (stringPlacer != null)
                {
                    script.GetWidget(3).GetComponent<Text>().set_text(stringPlacer);
                }
                script.GetWidget(0).GetComponent<InputField>().set_text(defaultString);
                this.m_strSendboxCb = onSendCallback;
            }
        }

        public void OpenTips(string strContent, bool bReadDatabin = false, float timeDuration = 1.5f, GameObject referenceGameObject = new GameObject(), params object[] replaceArr)
        {
            string text = strContent;
            if (bReadDatabin)
            {
                text = Singleton<CTextManager>.GetInstance().GetText(strContent);
            }
            if (!string.IsNullOrEmpty(text))
            {
                if (replaceArr != null)
                {
                    try
                    {
                        text = string.Format(text, replaceArr);
                    }
                    catch (FormatException exception)
                    {
                        object[] inParameters = new object[] { text, exception.Message };
                        DebugHelper.Assert(false, "Format Exception for string \"{0}\", Exception:{1}", inParameters);
                    }
                }
                CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/Common/Form_Tips.prefab", false, false);
                if (script != null)
                {
                    script.get_gameObject().get_transform().Find("Panel/Text").GetComponent<Text>().set_text(text);
                }
                if ((script != null) && (referenceGameObject != null))
                {
                    RectTransform component = referenceGameObject.GetComponent<RectTransform>();
                    RectTransform transform2 = script.get_gameObject().get_transform().Find("Panel") as RectTransform;
                    if ((component != null) && (transform2 != null))
                    {
                        Vector3[] vectorArray = new Vector3[4];
                        component.GetWorldCorners(vectorArray);
                        float num = Math.Abs((float) (CUIUtility.WorldToScreenPoint(Singleton<CUIManager>.instance.FormCamera, vectorArray[2]).x - CUIUtility.WorldToScreenPoint(Singleton<CUIManager>.instance.FormCamera, vectorArray[0]).x));
                        float num2 = Math.Abs((float) (CUIUtility.WorldToScreenPoint(Singleton<CUIManager>.instance.FormCamera, vectorArray[2]).y - CUIUtility.WorldToScreenPoint(Singleton<CUIManager>.instance.FormCamera, vectorArray[0]).y));
                        Vector2 screenPoint = new Vector2(CUIUtility.WorldToScreenPoint(Singleton<CUIManager>.instance.FormCamera, vectorArray[0]).x + (num / 2f), CUIUtility.WorldToScreenPoint(Singleton<CUIManager>.instance.FormCamera, vectorArray[0]).y + (num2 / 2f));
                        transform2.set_position(CUIUtility.ScreenToWorldPoint(null, screenPoint, transform2.get_position().z));
                    }
                }
                if (script != null)
                {
                    CUITimerScript script2 = script.get_gameObject().get_transform().Find("Timer").GetComponent<CUITimerScript>();
                    script2.EndTimer();
                    script2.m_totalTime = timeDuration;
                    script2.StartTimer();
                }
                Singleton<CSoundManager>.instance.PostEvent("UI_Click", null);
            }
        }

        private void ProcessFormList(bool sort, bool handleInputAndHide)
        {
            if (sort)
            {
                this.m_forms.Sort();
                for (int i = 0; i < this.m_forms.Count; i++)
                {
                    int formOpenOrder = this.GetFormOpenOrder(this.m_forms[i].GetSequence());
                    this.m_forms[i].SetDisplayOrder(formOpenOrder);
                }
            }
            if (handleInputAndHide)
            {
                this.UpdateFormHided();
                this.UpdateFormRaycaster();
            }
            if (this.onFormSorted != null)
            {
                this.onFormSorted(this.m_forms);
            }
        }

        private void RecycleForm(CUIFormScript formScript)
        {
            if (formScript != null)
            {
                if (formScript.m_useFormPool)
                {
                    formScript.Hide(enFormHideFlag.HideByCustom, true);
                    this.m_pooledForms.Add(formScript);
                }
                else
                {
                    try
                    {
                        if (formScript.m_canvasScaler != null)
                        {
                            formScript.m_canvasScaler.set_enabled(false);
                        }
                        Object.Destroy(formScript.get_gameObject());
                    }
                    catch (Exception exception)
                    {
                        object[] inParameters = new object[] { formScript.get_name(), exception.Message, exception.StackTrace };
                        DebugHelper.Assert(false, "Error destroy {0} formScript gameObject: message: {1}, callstack: {2}", inParameters);
                    }
                }
            }
        }

        private void RecycleForm(int formIndex)
        {
            this.RemoveFromExistFormSequenceList(this.m_forms[formIndex].GetSequence());
            this.RecycleForm(this.m_forms[formIndex]);
            this.m_forms.RemoveAt(formIndex);
        }

        public void RemoveFromExistFormSequenceList(int formSequence)
        {
            if (this.m_existFormSequences != null)
            {
                this.m_existFormSequences.Remove(formSequence);
            }
        }

        public void SearchBox_OnClose(CUIEvent evt)
        {
            Singleton<NetworkModule>.GetInstance().RemoveMsgHandler(this.m_searchHandlerCMD);
            Singleton<NetworkModule>.GetInstance().RemoveMsgHandler(this.m_recommendHandlerCMD);
            this.m_searchEvtCallBack = null;
            this.m_recommendEvtCallBack = null;
            this.m_recommendEvtCallBackSingleEnable = null;
        }

        public void SearchBox_OnRecommendListEnable(CUIEvent evt)
        {
            if (this.m_recommendEvtCallBackSingleEnable != null)
            {
                Singleton<EventRouter>.GetInstance().BroadCastEvent<GameObject, CUIEvent>(this.m_recommendEvtCallBackSingleEnable, this.m_searchRecommendGo, evt);
            }
        }

        public void SearchBox_RecommendHandler(CSPkg msg)
        {
            if (this.m_recommendEvtCallBack != null)
            {
                Singleton<EventRouter>.GetInstance().BroadCastEvent<CSPkg, GameObject>(this.m_recommendEvtCallBack, msg, this.m_searchRecommendGo);
            }
        }

        public void SearchBox_ResultHandler(CSPkg msg)
        {
            if (this.m_searchEvtCallBack != null)
            {
                Singleton<EventRouter>.GetInstance().BroadCastEvent<CSPkg, GameObject>(this.m_searchEvtCallBack, msg, this.m_searchResultGo);
            }
            if (this.m_searchResultGo.get_activeInHierarchy())
            {
                RectTransform transform2 = this.m_searchResultGo.get_transform() as RectTransform;
                transform2.set_sizeDelta(new Vector2(transform2.get_sizeDelta().x, transform2.get_sizeDelta().y + this.m_deltaSearchResultHeight));
                RectTransform transform4 = this.m_searchRecommendGo.get_transform() as RectTransform;
                transform4.set_anchoredPosition(new Vector2(transform4.get_anchoredPosition().x, transform4.get_anchoredPosition().y + this.m_deltaSearchResultHeight));
                transform4.set_sizeDelta(new Vector2(transform4.get_sizeDelta().x, transform4.get_sizeDelta().y - this.m_deltaSearchResultHeight));
            }
        }

        public void Update()
        {
            int formIndex = 0;
            while (formIndex < this.m_forms.Count)
            {
                this.m_forms[formIndex].CustomUpdate();
                if (this.m_forms[formIndex].IsNeedClose())
                {
                    if (!this.m_forms[formIndex].TurnToClosed(false))
                    {
                        goto Label_009C;
                    }
                    this.RecycleForm(formIndex);
                    this.m_needSortForms = true;
                    continue;
                }
                if (this.m_forms[formIndex].IsClosed() && !this.m_forms[formIndex].IsInFadeOut())
                {
                    this.RecycleForm(formIndex);
                    this.m_needSortForms = true;
                    continue;
                }
            Label_009C:
                formIndex++;
            }
            if (this.m_needSortForms)
            {
                this.ProcessFormList(true, true);
            }
            else if (this.m_needUpdateRaycasterAndHide)
            {
                this.ProcessFormList(false, true);
            }
            this.m_needSortForms = false;
            this.m_needUpdateRaycasterAndHide = false;
        }

        private void UpdateFormHided()
        {
            bool flag = false;
            for (int i = this.m_forms.Count - 1; i >= 0; i--)
            {
                if (flag)
                {
                    this.m_forms[i].Hide(enFormHideFlag.HideByOtherForm, false);
                }
                else
                {
                    this.m_forms[i].Appear(enFormHideFlag.HideByOtherForm, false);
                }
                if ((!flag && !this.m_forms[i].IsHided()) && this.m_forms[i].m_hideUnderForms)
                {
                    flag = true;
                }
            }
        }

        private void UpdateFormRaycaster()
        {
            bool flag = true;
            for (int i = this.m_forms.Count - 1; i >= 0; i--)
            {
                if (!this.m_forms[i].m_disableInput && !this.m_forms[i].IsHided())
                {
                    GraphicRaycaster graphicRaycaster = this.m_forms[i].GetGraphicRaycaster();
                    if (graphicRaycaster != null)
                    {
                        graphicRaycaster.set_enabled(flag);
                    }
                    if (this.m_forms[i].m_isModal && flag)
                    {
                        flag = false;
                    }
                }
            }
        }

        public Camera FormCamera
        {
            get
            {
                return this.m_formCamera;
            }
        }

        public enum enEditFormWidgets
        {
            Title_Text,
            Input_Text,
            Confirm_Button
        }

        private enum enInfoFormWidgets
        {
            Title_Text,
            Info_Text
        }

        public delegate void OnFormSorted(ListView<CUIFormScript> inForms);
    }
}

